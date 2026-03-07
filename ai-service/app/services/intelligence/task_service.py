"""
任务服务
负责协调情报采集、分析、日报生成等任务的执行
"""
import uuid
from datetime import datetime
from typing import Dict, Any

from app.core.db_client import save_task_log, update_task_status
from app.core.app_logging import logger
from app.services.intelligence.source_service import get_enabled_sources, update_last_fetch_time
from app.services.intelligence.collector_service import collect_from_rss, collect_from_web, save_collected_items, update_fetch_status
from app.services.intelligence.cleaner_service import clean_text, is_valid_content
from app.services.intelligence.analysis_service import analyze_content, get_pending_contents
from app.services.intelligence.report_service import generate_and_save_report


def run_collect_task() -> str:
    """
    执行采集任务

    Returns:
        任务 ID
    """
    task_id = str(uuid.uuid4())
    task_name = "情报采集任务"
    task_type = "collect"

    logger.info(f"开始执行采集任务: {task_id}")

    # 创建任务日志
    log_id = save_task_log(task_name, task_type, "running", "任务开始执行")

    try:
        # 获取启用的来源
        sources = get_enabled_sources()

        if not sources:
            update_task_status(log_id, "failed", end_time=True, message="没有启用的情报来源")
            return task_id

        total_collected = 0

        # 遍历来源进行采集
        for source in sources:
            try:
                logger.info(f"正在采集来源: {source['source_name']}")

                # 根据类型采集
                if source['source_type'] == 'RSS':
                    items = collect_from_rss(source)
                elif source['source_type'] == 'WEB':
                    items = collect_from_web(source)
                else:
                    logger.warning(f"未知的来源类型: {source['source_type']}")
                    continue

                # 保存采集的内容
                if items:
                    saved_count = save_collected_items(items)
                    total_collected += saved_count

                # 更新来源的最后抓取时间
                update_last_fetch_time(source['id'])

            except Exception as e:
                logger.error(f"采集来源 {source['source_name']} 失败: {e}")
                continue

        # 更新任务状态
        message = f"采集完成，共采集 {total_collected} 条新内容"
        update_task_status(log_id, "success", end_time=True, message=message)

        logger.info(f"采集任务完成: {message}")
        return task_id

    except Exception as e:
        logger.error(f"采集任务执行失败: {e}")
        update_task_status(log_id, "failed", end_time=True, message=f"任务失败: {str(e)}")
        return task_id


def run_analyze_task() -> str:
    """
    执行分析任务

    Returns:
        任务 ID
    """
    task_id = str(uuid.uuid4())
    task_name = "情报分析任务"
    task_type = "analyze"

    logger.info(f"开始执行分析任务: {task_id}")

    # 创建任务日志
    log_id = save_task_log(task_name, task_type, "running", "任务开始执行")

    try:
        # 获取待分析的内容
        pending_contents = get_pending_contents(limit=100)

        if not pending_contents:
            update_task_status(log_id, "success", end_time=True, message="没有待分析的内容")
            return task_id

        success_count = 0
        failed_count = 0

        # 分析每条内容
        for content in pending_contents:
            try:
                # 清洗内容
                clean_content = clean_text(content['raw_text'])

                if not is_valid_content(clean_content):
                    logger.warning(f"内容 ID {content['id']} 无效，跳过分析")
                    failed_count += 1
                    continue

                # 调用 AI 分析
                result = analyze_content(
                    content_id=content['id'],
                    title=content['title'],
                    content=clean_content
                )

                if result:
                    success_count += 1
                else:
                    failed_count += 1

            except Exception as e:
                logger.error(f"分析内容 ID {content['id']} 失败: {e}")
                failed_count += 1
                continue

        # 更新任务状态
        message = f"分析完成，成功 {success_count} 条，失败 {failed_count} 条"
        update_task_status(log_id, "success", end_time=True, message=message)

        logger.info(f"分析任务完成: {message}")
        return task_id

    except Exception as e:
        logger.error(f"分析任务执行失败: {e}")
        update_task_status(log_id, "failed", end_time=True, message=f"任务失败: {str(e)}")
        return task_id


def run_generate_report_task() -> str:
    """
    执行日报生成任务

    Returns:
        任务 ID
    """
    task_id = str(uuid.uuid4())
    task_name = "日报生成任务"
    task_type = "generate_report"

    logger.info(f"开始执行日报生成任务: {task_id}")

    # 创建任务日志
    log_id = save_task_log(task_name, task_type, "running", "任务开始执行")

    try:
        # 生成并保存日报
        report = generate_and_save_report()

        if report:
            message = f"日报生成成功，包含 {report['item_count']} 条内容"
            update_task_status(log_id, "success", end_time=True, message=message)
        else:
            message = "日报生成失败"
            update_task_status(log_id, "failed", end_time=True, message=message)

        logger.info(f"日报生成任务完成: {message}")
        return task_id

    except Exception as e:
        logger.error(f"日报生成任务执行失败: {e}")
        update_task_status(log_id, "failed", end_time=True, message=f"任务失败: {str(e)}")
        return task_id
