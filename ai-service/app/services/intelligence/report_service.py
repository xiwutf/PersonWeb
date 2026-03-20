"""
日报生成服务
负责基于当日分析结果生成每日情报简报
"""
import json
from datetime import datetime, timedelta
from typing import List, Dict, Any, Optional
from app.core.llm_client import LLMClient
from app.core.db_client import execute_query
from app.core.app_logging import logger


# 日报生成 Prompt 模板
DAILY_REPORT_SYSTEM_PROMPT = """你是一个专业的情报简报编辑，负责将今日收集到的情报整理成一份简洁的每日简报。

## 简报结构

请按以下结构组织日报：

# 今日情报简报 - YYYY-MM-DD

## 一、今日重点
- 列出 3-5 条最重要、最紧急的情报

## 二、技术动态
- 列出技术领域的重要更新和趋势
- 每条 50 字以内

## 三、商业机会
- 列出有潜力的商业机会或合作可能
- 每条 50 字以内

## 四、值得沉淀的知识
- 列出有价值、需要记录的知识点
- 每条 50 字以内

## 五、今日结论
- 总结今日情报的整体趋势或关键洞察
- 50-100 字

## 编辑原则

1. 精简高效：每条情报控制在 50 字以内
2. 突出重点：优先展示高相关性、高价值的情报
3. 逻辑清晰：按类别组织，便于快速浏览
4. 实用导向：注重可操作性而非信息堆砌

## 注意事项

- 请以 Markdown 格式输出，不要包含代码块标记
- 如果某部分没有内容，输出"暂无"
- 不要添加额外的说明文字
"""


def get_today_contents(report_date: str = None) -> List[Dict[str, Any]]:
    """
    获取指定日期的分析内容

    Args:
        report_date: 报告日期 (YYYY-MM-DD)，默认为今天

    Returns:
        分析内容列表
    """
    if not report_date:
        report_date = datetime.now().strftime('%Y-%m-%d')

    try:
        # 计算日期范围（当天 00:00:00 到 23:59:59）
        start_date = f"{report_date} 00:00:00"
        end_date = f"{report_date} 23:59:59"

        sql = """
            SELECT c.id, c.title, c.original_url, c.publish_time,
                   a.category, a.summary, a.tags_json, a.relevance_score,
                   a.learning_value, a.business_value
            FROM intelligence_content c
            JOIN intelligence_analysis a ON c.id = a.content_id
            WHERE c.fetch_status = 'success'
              AND c.analyze_status = 'success'
              AND c.created_at >= %s
              AND c.created_at <= %s
            ORDER BY a.relevance_score DESC, c.created_at DESC
            LIMIT 50
        """

        contents = execute_query(sql, (start_date, end_date))

        # 解析 tags_json
        for content in contents:
            if content.get('tags_json'):
                try:
                    content['tags'] = json.loads(content['tags_json'])
                except json.JSONDecodeError:
                    content['tags'] = []
            else:
                content['tags'] = []

        logger.info(f"获取到 {len(contents)} 条 {report_date} 的分析内容")
        return contents

    except Exception as e:
        logger.error(f"获取今日分析内容失败: {e}")
        return []


def generate_daily_report(report_date: str = None) -> Optional[str]:
    """
    生成每日情报简报

    Args:
        report_date: 报告日期 (YYYY-MM-DD)，默认为今天

    Returns:
        生成的 Markdown 内容，失败返回 None
    """
    if not report_date:
        report_date = datetime.now().strftime('%Y-%m-%d')

    try:
        logger.info(f"开始生成 {report_date} 的日报")

        # 获取当日内容
        contents = get_today_contents(report_date)

        if not contents:
            logger.warning(f"{report_date} 没有可生成日报的内容")
            return f"# 今日情报简报 - {report_date}\n\n暂无内容。"

        # 准备上下文信息
        # 按分类分组
        by_category = {}
        for content in contents:
            category = content.get('category', '其他')
            if category not in by_category:
                by_category[category] = []
            by_category[category].append(content)

        # 按价值筛选高价值内容
        high_value_contents = [
            c for c in contents
            if c.get('relevance_score', 0) >= 70 or c.get('learning_value') == '高'
        ]

        # 准备内容摘要（限制长度）
        content_summary = []
        for i, content in enumerate(high_value_contents[:20]):
            content_summary.append(f"{i+1}. [{content.get('category', '其他')}] {content.get('title', '')}")
            if content.get('summary'):
                content_summary.append(f"   摘要: {content['summary'][:100]}")

        summary_text = "\n".join(content_summary)
        user_message = (
            "今日收集到的情报摘要如下：\n\n"
            + summary_text
            + f"\n\n请根据以上内容生成一份每日情报简报，日期为：{report_date}"
        )

        # 调用 LLM
        llm_client = LLMClient()
        response = llm_client.chat(
            system_prompt=DAILY_REPORT_SYSTEM_PROMPT,
            user_message=user_message,
            temperature=0.5,
            max_tokens=2000
        )

        if not response:
            logger.error(f"生成日报失败: LLM 返回空结果")
            return None

        # 清理响应（移除可能的代码块标记）
        markdown = response.strip()
        if markdown.startswith('```'):
            markdown = '\n'.join(markdown.split('\n')[1:-1])
            if markdown.startswith('markdown'):
                markdown = markdown[9:]

        logger.info(f"{report_date} 日报生成成功，长度: {len(markdown)}")
        return markdown

    except Exception as e:
        logger.error(f"生成日报失败: {e}")
        return None


def save_daily_report(report_date: str, content: str, item_count: int) -> int:
    """
    保存日报到数据库

    Args:
        report_date: 报告日期 (YYYY-MM-DD)
        content: Markdown 内容
        item_count: 包含的内容数量

    Returns:
        日报 ID
    """
    try:
        # 检查是否已存在当日日报
        check_sql = "SELECT id FROM intelligence_daily_report WHERE report_date = %s"
        existing = execute_query(check_sql, (report_date,))

        if existing:
            # 更新现有日报
            update_sql = """
                UPDATE intelligence_daily_report
                SET title = %s, content_markdown = %s, item_count = %s, generated_at = NOW(), updated_at = NOW()
                WHERE report_date = %s
            """
            title = f"情报简报 - {report_date}"
            execute_query(update_sql, (title, content, item_count, report_date), fetch=False)
            logger.info(f"更新了 {report_date} 的日报")
            return existing[0]['id']
        else:
            # 创建新日报
            insert_sql = """
                INSERT INTO intelligence_daily_report
                (report_date, title, content_markdown, item_count, generated_at, created_at, updated_at)
                VALUES (%s, %s, %s, %s, NOW(), NOW(), NOW())
            """
            title = f"情报简报 - {report_date}"
            execute_query(insert_sql, (report_date, title, content, item_count), fetch=False)

            # 获取插入的 ID
            result = execute_query("SELECT LAST_INSERT_ID() as id")
            logger.info(f"创建了 {report_date} 的日报")
            return result[0]['id'] if result else 0

    except Exception as e:
        logger.error(f"保存日报失败: {e}")
        return 0


def generate_and_save_report(report_date: str = None) -> Optional[Dict[str, Any]]:
    """
    生成并保存日报

    Args:
        report_date: 报告日期 (YYYY-MM-DD)，默认为今天

    Returns:
        包含 id, report_date, title, content 的字典，失败返回 None
    """
    if not report_date:
        report_date = datetime.now().strftime('%Y-%m-%d')

    try:
        # 获取当日内容数量
        contents = get_today_contents(report_date)
        item_count = len(contents)

        if item_count == 0:
            return {
                'report_date': report_date,
                'title': f'情报简报 - {report_date}',
                'content': f'# 今日情报简报 - {report_date}\n\n暂无内容。',
                'item_count': 0
            }

        # 生成日报内容
        content = generate_daily_report(report_date)
        if not content:
            return None

        # 保存到数据库
        report_id = save_daily_report(report_date, content, item_count)

        return {
            'id': report_id,
            'report_date': report_date,
            'title': f'情报简报 - {report_date}',
            'content': content,
            'item_count': item_count
        }

    except Exception as e:
        logger.error(f"生成并保存日报失败: {e}")
        return None
