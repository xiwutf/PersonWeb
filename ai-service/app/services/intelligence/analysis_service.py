"""
AI 分析服务
负责调用大模型对采集的内容进行分类、摘要、标签提取等分析
"""
import json
from typing import Dict, Any, List, Optional
from app.core.llm_client import LLMClient
from app.core.config import settings
from app.core.app_logging import logger
from app.core.db_client import execute_query, update_task_status


# 分析 Prompt 模板
ANALYSIS_SYSTEM_PROMPT = """你是一个专业的情报分析师，负责分析收集到的技术、商业、投资等相关内容。

## 分析任务

请对提供的内容进行分析，输出以下信息：

1. **分类**：将内容归类到以下类别之一
   - AI技术
   - 软件开发
   - 商业机会
   - 投资理财
   - 认知成长
   - 其他

2. **摘要**：用一句话概括内容核心（不超过100字）

3. **核心要点**：提取 3-5 个关键信息点，每个不超过 50 字

4. **标签**：提取 3-5 个相关标签，标签要简洁明了

5. **相关性评分**：0-100 分，评估内容与个人发展的相关程度

6. **学习价值**：高/中/低，评估内容对个人知识积累的价值

7. **商业价值**：高/中/低，评估内容的商业应用潜力

8. **行动建议**：根据内容给出具体行动建议（可选）

## 输出格式

请以 JSON 格式输出，不要包含任何其他文字：

```json
{
  "category": "分类",
  "summary": "摘要",
  "core_points": ["要点1", "要点2", "要点3"],
  "tags": ["标签1", "标签2", "标签3"],
  "relevance_score": 85,
  "learning_value": "高",
  "business_value": "中",
  "action_suggestion": "建议行动"
}
```

## 注意事项

- 标签使用中文，简洁有力
- 评分要客观合理
- 如果内容与个人发展无关，相关性评分为 0
"""


def analyze_content(content_id: int, title: str, content: str, category_hint: str = None) -> Optional[Dict[str, Any]]:
    """
    分析单条内容

    Args:
        content_id: 内容 ID
        title: 内容标题
        content: 内容文本
        category_hint: 分类提示（可选）

    Returns:
        分析结果字典，失败返回 None
    """
    try:
        logger.info(f"开始分析内容 ID: {content_id}, 标题: {title}")

        # 准备用户消息
        user_message = f"""请分析以下内容：

标题：{title}

分类提示：{category_hint or '未指定'}

内容：
{content[:4000]}  # 限制内容长度
"""

        # 调用 LLM
        llm_client = LLMClient()
        response = llm_client.chat(
            system_prompt=ANALYSIS_SYSTEM_PROMPT,
            user_message=user_message,
            temperature=0.3,
            max_tokens=1000
        )

        if not response:
            logger.error(f"分析内容失败: LLM 返回空结果")
            return None

        # 解析 JSON 响应
        try:
            # 提取 JSON（可能在代码块中）
            json_text = response.strip()
            if json_text.startswith('```'):
                json_text = '\n'.join(json_text.split('\n')[1:-1])
                if json_text.startswith('json'):
                    json_text = json_text[4:]

            analysis_result = json.loads(json_text)

            # 验证必需字段
            required_fields = ['category', 'summary', 'tags', 'relevance_score', 'learning_value', 'business_value']
            missing_fields = [f for f in required_fields if f not in analysis_result]
            if missing_fields:
                logger.warning(f"分析结果缺少字段: {missing_fields}")
                # 设置默认值
                for field in missing_fields:
                    if field in ['relevance_score']:
                        analysis_result[field] = 50
                    elif field in ['learning_value', 'business_value']:
                        analysis_result[field] = '中'
                    elif field == 'category':
                        analysis_result[field] = '其他'
                    elif field == 'summary':
                        analysis_result[field] = '暂无摘要'
                    elif field == 'tags':
                        analysis_result[field] = []

            # 保存到数据库
            save_analysis(content_id, analysis_result)

            logger.info(f"内容 {content_id} 分析完成: {analysis_result['category']}")
            return analysis_result

        except json.JSONDecodeError as e:
            logger.error(f"解析分析结果 JSON 失败: {e}, 响应: {response[:200]}")
            return None

    except Exception as e:
        logger.error(f"分析内容失败: {e}")
        return None


def save_analysis(content_id: int, analysis: Dict[str, Any]) -> bool:
    """
    保存分析结果到数据库

    Args:
        content_id: 内容 ID
        analysis: 分析结果字典

    Returns:
        是否保存成功
    """
    try:
        # 序列化 JSON 字段
        core_points_json = json.dumps(analysis.get('core_points', []), ensure_ascii=False)
        tags_json = json.dumps(analysis.get('tags', []), ensure_ascii=False)

        sql = """
            INSERT INTO intelligence_analysis
            (content_id, category, summary, core_points_json, tags_json,
             relevance_score, learning_value, business_value, action_suggestion,
             model_name, created_at, updated_at)
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, NOW(), NOW())
            ON DUPLICATE KEY UPDATE
                category = VALUES(category),
                summary = VALUES(summary),
                core_points_json = VALUES(core_points_json),
                tags_json = VALUES(tags_json),
                relevance_score = VALUES(relevance_score),
                learning_value = VALUES(learning_value),
                business_value = VALUES(business_value),
                action_suggestion = VALUES(action_suggestion),
                model_name = VALUES(model_name),
                updated_at = NOW()
        """

        model_name = f"{settings.LLM_PROVIDER}-{settings.DEFAULT_MODEL_NAME}"

        execute_query(sql, (
            content_id,
            analysis.get('category', '其他'),
            analysis.get('summary', ''),
            core_points_json,
            tags_json,
            analysis.get('relevance_score', 50),
            analysis.get('learning_value', '中'),
            analysis.get('business_value', '中'),
            analysis.get('action_suggestion'),
            model_name
        ), fetch=False)

        # 更新内容的分析状态
        update_content_analyze_status(content_id, 'success')

        return True

    except Exception as e:
        logger.error(f"保存分析结果失败: {e}")
        # 更新分析状态为失败
        update_content_analyze_status(content_id, 'failed')
        return False


def update_content_analyze_status(content_id: int, status: str):
    """更新内容的分析状态"""
    try:
        sql = "UPDATE intelligence_content SET analyze_status = %s, updated_at = NOW() WHERE id = %s"
        execute_query(sql, (status, content_id), fetch=False)
    except Exception as e:
        logger.error(f"更新分析状态失败: {e}")


def get_pending_contents(limit: int = 100) -> List[Dict[str, Any]]:
    """
    获取待分析的内容

    Args:
        limit: 最大数量

    Returns:
        待分析的内容列表
    """
    try:
        sql = """
            SELECT id, title, raw_text
            FROM intelligence_content
            WHERE fetch_status = 'success'
              AND analyze_status IN ('pending', 'failed')
            ORDER BY created_at DESC
            LIMIT %s
        """
        return execute_query(sql, (limit,))
    except Exception as e:
        logger.error(f"获取待分析内容失败: {e}")
        return []
