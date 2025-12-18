"""
API 路由注册
"""

from fastapi import APIRouter
from app.api.v1 import website_chat

# 创建 v1 路由
v1_router = APIRouter(prefix="/ai", tags=["AI 服务"])

# 注册 v1 路由
v1_router.include_router(website_chat.router)

# 创建主路由
api_router = APIRouter(prefix="/api")

# 注册 v1 路由到主路由
api_router.include_router(v1_router)

