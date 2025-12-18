"""
自定义异常类
"""


class AIHubException(Exception):
    """AI 中台基础异常"""
    pass


class LLMException(AIHubException):
    """LLM 调用异常"""
    pass


class LLMTimeoutException(LLMException):
    """LLM 调用超时"""
    pass


class LLMNetworkException(LLMException):
    """LLM 网络异常"""
    pass


class LLMAPIException(LLMException):
    """LLM API 返回错误"""
    pass


class PromptException(AIHubException):
    """Prompt 加载/渲染异常"""
    pass


class SceneException(AIHubException):
    """Scene 处理异常"""
    pass


class SecurityException(AIHubException):
    """安全验证异常"""
    pass

