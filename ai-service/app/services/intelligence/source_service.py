"""
情报来源服务
负责从数据库读取启用的情报来源配置
"""
from typing import List, Dict, Any
from app.core.db_client import execute_query
from app.core.app_logging import logger


def get_enabled_sources() -> List[Dict[str, Any]]:
    """
    获取所有启用的情报来源

    Returns:
        来源列表，每个来源包含 id, source_name, source_type, source_url 等字段
    """
    try:
        sql = """
            SELECT id, source_name, source_type, source_url, category, tags_json,
                   priority, fetch_interval_minutes, remark, last_fetch_time
            FROM intelligence_source
            WHERE enabled = TRUE
            ORDER BY priority ASC, id ASC
        """
        sources = execute_query(sql)

        # 解析 tags_json
        for source in sources:
            if source.get('tags_json'):
                import json
                try:
                    source['tags'] = json.loads(source['tags_json'])
                except json.JSONDecodeError:
                    source['tags'] = []
            else:
                source['tags'] = []

        logger.info(f"获取到 {len(sources)} 个启用的情报来源")
        return sources
    except Exception as e:
        logger.error(f"获取情报来源失败: {e}")
        raise


def update_last_fetch_time(source_id: int):
    """更新来源的最后抓取时间"""
    try:
        sql = "UPDATE intelligence_source SET last_fetch_time = NOW() WHERE id = %s"
        execute_query(sql, (source_id,), fetch=False)
    except Exception as e:
        logger.error(f"更新来源最后抓取时间失败: {e}")
