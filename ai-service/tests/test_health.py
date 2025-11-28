"""
健康检查接口测试
"""

import pytest
from httpx import AsyncClient
from app.main import app


@pytest.mark.asyncio
async def test_health_endpoint():
    """测试健康检查接口"""
    async with AsyncClient(app=app, base_url="http://test") as client:
        # 测试 /health 端点
        response = await client.get("/health")
        assert response.status_code == 200
        
        data = response.json()
        assert data["success"] is True
        assert data["data"]["status"] == "ok"
        assert data["data"]["service"] == "ai-service"
        assert "version" in data["data"]


@pytest.mark.asyncio
async def test_health_api_endpoint():
    """测试 /api/ai/health 端点"""
    async with AsyncClient(app=app, base_url="http://test") as client:
        response = await client.get("/api/ai/health")
        assert response.status_code == 200
        
        data = response.json()
        assert data["success"] is True
        assert data["data"]["status"] == "ok"

