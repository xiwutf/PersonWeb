"""
WebsiteChat API 测试
"""

import pytest
from fastapi.testclient import TestClient
from unittest.mock import patch, AsyncMock
from app.main import app


@pytest.fixture
def client():
    """测试客户端"""
    return TestClient(app)


@pytest.fixture
def mock_llm_client():
    """Mock LLM 客户端"""
    with patch("app.services.scenes.website_chat.get_llm_client") as mock:
        mock_client = AsyncMock()
        mock_client.chat = AsyncMock(return_value=(
            "这是 AI 回复内容",
            {"promptTokens": 100, "completionTokens": 50}
        ))
        mock.return_value = mock_client
        yield mock_client


def test_website_chat_with_user_input(client, mock_llm_client):
    """测试使用 user_input 的网站聊天接口"""
    response = client.post(
        "/api/ai/website-chat",
        json={
            "user_input": "你好",
            "extra": {"page": "home"}
        },
        headers={
            "X-Internal-Key": "dev-bypass"  # dev 环境允许放行
        }
    )
    
    assert response.status_code == 200
    data = response.json()
    
    # 验证返回结构
    assert "content" in data
    assert "usage" in data
    assert "traceId" in data
    
    # 验证内容
    assert data["content"] == "这是 AI 回复内容"
    assert data["usage"]["promptTokens"] == 100
    assert data["usage"]["completionTokens"] == 50
    assert isinstance(data["traceId"], str)


def test_website_chat_with_messages(client, mock_llm_client):
    """测试使用 messages 的网站聊天接口"""
    response = client.post(
        "/api/ai/website-chat",
        json={
            "messages": [
                {"role": "user", "content": "你好"}
            ]
        },
        headers={
            "X-Internal-Key": "dev-bypass"
        }
    )
    
    assert response.status_code == 200
    data = response.json()
    
    # 验证返回结构
    assert "content" in data
    assert "usage" in data
    assert "traceId" in data


def test_website_chat_missing_input(client):
    """测试缺少输入的情况"""
    response = client.post(
        "/api/ai/website-chat",
        json={},
        headers={
            "X-Internal-Key": "dev-bypass"
        }
    )
    
    assert response.status_code == 400
    assert "必须提供" in response.json()["detail"]


def test_website_chat_with_trace_id(client, mock_llm_client):
    """测试带 traceId 的请求"""
    trace_id = "test-trace-id-123"
    response = client.post(
        "/api/ai/website-chat",
        json={
            "user_input": "你好",
            "extra": {"traceId": trace_id}
        },
        headers={
            "X-Internal-Key": "dev-bypass",
            "X-Trace-Id": "header-trace-id"  # Header 中的 traceId
        }
    )
    
    assert response.status_code == 200
    data = response.json()
    
    # 应该使用 extra.traceId（优先级更高）
    assert data["traceId"] == trace_id

