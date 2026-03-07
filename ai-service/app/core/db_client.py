"""
MySQL 数据库连接客户端
用于情报中心模块读写数据库
"""
import pymysql
from typing import List, Dict, Any, Optional
from contextlib import contextmanager
from app.core.config import settings
from app.core.app_logging import logger
import json


@contextmanager
def get_db_connection():
    """获取数据库连接上下文管理器"""
    conn = pymysql.connect(
        host='localhost',
        port=3306,
        database='personal_site',
        user='root',
        password='',
        charset='utf8mb4',
        cursorclass=pymysql.cursors.DictCursor
    )
    try:
        yield conn
    finally:
        conn.close()


def execute_query(sql: str, params: tuple = None, fetch: bool = True) -> List[Dict]:
    """执行查询语句"""
    try:
        with get_db_connection() as conn:
            with conn.cursor() as cursor:
                cursor.execute(sql, params or ())
                if fetch:
                    return cursor.fetchall()
                conn.commit()
                return []
    except Exception as e:
        logger.error(f"数据库查询错误: {e}")
        raise


def execute_batch(sql: str, params_list: List[tuple]) -> int:
    """批量执行语句"""
    try:
        with get_db_connection() as conn:
            with conn.cursor() as cursor:
                affected_rows = cursor.executemany(sql, params_list)
                conn.commit()
                return affected_rows
    except Exception as e:
        logger.error(f"数据库批量执行错误: {e}")
        raise


def save_task_log(task_name: str, task_type: str, status: str, message: str, detail: dict = None) -> int:
    """保存任务日志"""
    try:
        sql = """
            INSERT INTO intelligence_task_log
            (task_name, task_type, status, start_time, message, detail_json, created_at)
            VALUES (%s, %s, %s, NOW(), %s, %s, NOW())
        """
        detail_json = json.dumps(detail, ensure_ascii=False) if detail else None
        execute_query(sql, (task_name, task_type, status, message, detail_json), fetch=False)

        # 获取最后插入的 ID
        sql = "SELECT LAST_INSERT_ID() as id"
        result = execute_query(sql)
        return result[0]['id'] if result else 0
    except Exception as e:
        logger.error(f"保存任务日志失败: {e}")
        return 0


def update_task_status(task_id: int, status: str, end_time: bool = False, message: str = None, detail: dict = None):
    """更新任务状态"""
    try:
        sql = "UPDATE intelligence_task_log SET status = %s"
        params = [status]

        if end_time:
            sql += ", end_time = NOW()"
        if message:
            sql += ", message = %s"
            params.append(message)
        if detail:
            sql += ", detail_json = %s"
            params.append(json.dumps(detail, ensure_ascii=False))

        sql += " WHERE id = %s"
        params.append(task_id)

        execute_query(sql, tuple(params), fetch=False)
    except Exception as e:
        logger.error(f"更新任务状态失败: {e}")
        raise
