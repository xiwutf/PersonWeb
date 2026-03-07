"""
采集服务
负责从 RSS 和网页源采集情报内容
"""
import feedparser
import requests
from bs4 import BeautifulSoup
from typing import List, Dict, Any
from datetime import datetime
import hashlib
import json

from app.core.db_client import execute_query, execute_batch
from app.core.app_logging import logger
from app.services.intelligence.source_service import update_last_fetch_time


def collect_from_rss(source: Dict[str, Any]) -> List[Dict[str, Any]]:
    """
    从 RSS 源采集内容

    Args:
        source: 来源配置字典

    Returns:
        采集到的内容列表
    """
    items = []

    try:
        logger.info(f"开始采集 RSS 源: {source['source_name']} - {source['source_url']}")

        # 解析 RSS
        feed = feedparser.parse(source['source_url'])

        for entry in feed.entries:
            try:
                # 提取基本信息
                title = entry.get('title', '')[:200]  # 限制标题长度
                original_url = entry.get('link', '')[:1000]

                if not original_url:
                    continue

                # 发布时间
                publish_time = None
                if hasattr(entry, 'published_parsed') and entry.published_parsed:
                    publish_time = datetime(*entry.published_parsed[:6])

                # 作者
                author = entry.get('author', '')[:200] if hasattr(entry, 'author') else None

                # 内容
                raw_text = ''
                if hasattr(entry, 'content'):
                    content = entry.content[0]
                    raw_text = content.value if hasattr(content, 'value') else str(content)
                elif hasattr(entry, 'summary'):
                    raw_text = entry.summary
                elif hasattr(entry, 'description'):
                    raw_text = entry.description

                # 限制原始文本长度
                if len(raw_text) > 100000:  # 100KB
                    raw_text = raw_text[:100000]

                # 计算内容哈希（用于去重）
                content_hash = hashlib.md5(
                    f"{title}{original_url}{raw_text}".encode('utf-8')
                ).hexdigest()

                items.append({
                    'source_id': source['id'],
                    'title': title,
                    'original_url': original_url,
                    'publish_time': publish_time,
                    'author': author,
                    'raw_text': raw_text,
                    'content_hash': content_hash,
                    'fetch_status': 'pending'
                })

            except Exception as e:
                logger.warning(f"解析 RSS 条目失败: {e}")
                continue

        logger.info(f"RSS 源 {source['source_name']} 采集到 {len(items)} 条内容")
        return items

    except Exception as e:
        logger.error(f"采集 RSS 源失败 {source['source_name']}: {e}")
        return []


def collect_from_web(source: Dict[str, Any]) -> List[Dict[str, Any]]:
    """
    从网页源采集内容（简化版：仅获取页面内容）

    Args:
        source: 来源配置字典

    Returns:
        采集到的内容列表
    """
    items = []

    try:
        logger.info(f"开始采集网页源: {source['source_name']} - {source['source_url']}")

        # 请求网页
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
        }
        response = requests.get(source['source_url'], headers=headers, timeout=30)
        response.raise_for_status()

        # 解析页面
        soup = BeautifulSoup(response.text, 'html.parser')

        # 提取标题
        title = soup.find('title')
        title_text = title.get_text().strip() if title else source['source_name']
        title_text = title_text[:200]  # 限制长度

        # 提取主要内容
        body = soup.find('body')
        raw_text = body.get_text(separator='\n', strip=True) if body else ''
        raw_text = raw_text[:100000]  # 限制 100KB

        # 计算内容哈希
        content_hash = hashlib.md5(
            f"{title_text}{source['source_url']}{raw_text}".encode('utf-8')
        ).hexdigest()

        items.append({
            'source_id': source['id'],
            'title': title_text,
            'original_url': source['source_url'],
            'publish_time': None,
            'author': None,
            'raw_text': raw_text,
            'content_hash': content_hash,
            'fetch_status': 'pending'
        })

        logger.info(f"网页源 {source['source_name']} 采集到 1 条内容")
        return items

    except Exception as e:
        logger.error(f"采集网页源失败 {source['source_name']}: {e}")
        return []


def save_collected_items(items: List[Dict[str, Any]]) -> int:
    """
    保存采集的内容到数据库（自动去重）

    Args:
        items: 内容列表

    Returns:
        新增的内容数量
    """
    if not items:
        return 0

    try:
        # 先查询已存在的内容哈希，实现去重
        hashes = [item['content_hash'] for item in items]
        placeholders = ','.join(['%s'] * len(hashes))
        sql = f"SELECT content_hash FROM intelligence_content WHERE content_hash IN ({placeholders})"
        existing_hashes = set(row['content_hash'] for row in execute_query(sql, tuple(hashes)))

        # 过滤掉已存在的内容
        new_items = [item for item in items if item['content_hash'] not in existing_hashes]

        if not new_items:
            logger.info("所有内容已存在，跳过保存")
            return 0

        # 批量插入新内容
        insert_sql = """
            INSERT INTO intelligence_content
            (source_id, title, original_url, publish_time, author, raw_text, content_hash,
             fetch_status, analyze_status, created_at, updated_at)
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, NOW(), NOW())
        """

        params_list = []
        for item in new_items:
            params_list.append((
                item['source_id'],
                item['title'],
                item['original_url'],
                item['publish_time'],
                item['author'],
                item['raw_text'],
                item['content_hash'],
                item['fetch_status'],
                'pending'
            ))

        affected_rows = execute_batch(insert_sql, params_list)
        logger.info(f"保存了 {affected_rows} 条新内容")
        return affected_rows

    except Exception as e:
        logger.error(f"保存采集内容失败: {e}")
        raise


def update_fetch_status(content_id: int, status: str):
    """更新内容的抓取状态"""
    try:
        sql = "UPDATE intelligence_content SET fetch_status = %s, updated_at = NOW() WHERE id = %s"
        execute_query(sql, (status, content_id), fetch=False)
    except Exception as e:
        logger.error(f"更新抓取状态失败: {e}")
