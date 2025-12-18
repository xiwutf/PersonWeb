"""
FastAPI 应用主入口
"""

import uuid
from fastapi import FastAPI, Request
from fastapi.middleware.cors import CORSMiddleware
from app.core.config import get_settings
from app.core.logging import setup_logging
from app.api.router import api_router
from app.services.observability.traces import generate_trace_id

# 获取配置
settings = get_settings()

# 配置日志
setup_logging(settings.LOG_LEVEL)

# 创建 FastAPI 应用
app = FastAPI(
    title=settings.APP_NAME,
    version="1.0.0",
    description="Python AI 中台 - 提供统一的 AI 能力服务"
)

# 配置 CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # 生产环境应该限制具体域名
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.middleware("http")
async def trace_id_middleware(request: Request, call_next):
    """
    TraceId 中间件
    为每个请求生成/提取 traceId 并注入到 request.state
    """
    # 优先从 Header 获取
    trace_id = request.headers.get("X-Trace-Id")
    
    # 如果没有，生成新的
    if not trace_id:
        trace_id = generate_trace_id()
    
    # 注入到 request.state
    request.state.trace_id = trace_id
    
    # 继续处理请求
    response = await call_next(request)
    
    # 在响应头中添加 traceId（可选）
    response.headers["X-Trace-Id"] = trace_id
    
    return response


# 注册 API 路由
app.include_router(api_router)


@app.get("/healthz")
async def health_check():
    """
    健康检查接口
    
    Returns:
        dict: {"ok": true}
    """
    return {"ok": True}


@app.get("/")
async def root():
    """
    根路径
    
    Returns:
        dict: 服务信息
    """
    return {
        "service": settings.APP_NAME,
        "version": "1.0.0",
        "status": "running"
    }


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(
        "app.main:app",
        host="0.0.0.0",
        port=8001,
        reload=True
    )

