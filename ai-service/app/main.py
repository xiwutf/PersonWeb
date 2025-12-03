"""
FastAPI 应用主入口
负责初始化应用、挂载路由、配置中间件等
"""

from fastapi import FastAPI, Request, status
from fastapi.responses import JSONResponse
from fastapi.middleware.cors import CORSMiddleware
import time
from contextlib import asynccontextmanager

from app.core.config import settings
from app.core.logging import setup_logging, logger
from app.api import health, chat, tools, rag, document


@asynccontextmanager
async def lifespan(app: FastAPI):
    """应用生命周期管理"""
    # 启动时执行
    setup_logging()
    logger.info("AI 服务启动中...")
    logger.info(f"服务名称: {settings.SERVICE_NAME}")
    logger.info(f"版本: {settings.VERSION}")
    yield
    # 关闭时执行
    logger.info("AI 服务关闭中...")


# 创建 FastAPI 应用实例
app = FastAPI(
    title=settings.SERVICE_NAME,
    version=settings.VERSION,
    description="AI 能力服务，提供聊天、工具、RAG 等功能",
    lifespan=lifespan
)

# 配置 CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.ALLOWED_ORIGINS,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


# 请求日志中间件
@app.middleware("http")
async def log_requests(request: Request, call_next):
    """记录请求日志"""
    start_time = time.time()
    
    # 记录请求信息
    logger.info(
        f"请求开始: {request.method} {request.url.path} - "
        f"客户端: {request.client.host if request.client else 'unknown'}"
    )
    
    try:
        response = await call_next(request)
        process_time = time.time() - start_time
        
        # 记录响应信息
        logger.info(
            f"请求完成: {request.method} {request.url.path} - "
            f"状态码: {response.status_code} - 耗时: {process_time:.3f}s"
        )
        
        # 添加响应头
        response.headers["X-Process-Time"] = str(process_time)
        return response
    except Exception as e:
        process_time = time.time() - start_time
        logger.error(
            f"请求异常: {request.method} {request.url.path} - "
            f"错误: {str(e)} - 耗时: {process_time:.3f}s"
        )
        raise


# 全局异常处理
@app.exception_handler(Exception)
async def global_exception_handler(request: Request, exc: Exception):
    """全局异常处理器"""
    logger.error(f"未处理的异常: {str(exc)}", exc_info=True)
    return JSONResponse(
        status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
        content={
            "success": False,
            "error_code": "INTERNAL_ERROR",
            "message": "服务器内部错误",
            "data": None
        }
    )


# 挂载路由
app.include_router(health.router, tags=["健康检查"])
app.include_router(chat.router, prefix="/api/ai", tags=["聊天"])
app.include_router(tools.router, prefix="/api/ai", tags=["AI 工具"])
app.include_router(rag.router, prefix="/api/ai", tags=["RAG 知识库"])
app.include_router(document.router, prefix="/api/ai", tags=["文档知识管家"])


@app.get("/")
async def root():
    """根路径"""
    return {
        "success": True,
        "data": {
            "service": settings.SERVICE_NAME,
            "version": settings.VERSION,
            "status": "running"
        },
        "error_code": None,
        "message": ""
    }


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(
        "app.main:app",
        host="0.0.0.0",
        port=8001,
        reload=True
    )

