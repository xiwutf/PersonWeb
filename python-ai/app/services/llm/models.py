"""
LLM 模型默认参数配置
"""

# 模型默认参数表
MODEL_DEFAULTS = {
    "gpt-4o-mini": {
        "temperature": 0.7,
        "max_tokens": 2000,
    },
    "gpt-4o": {
        "temperature": 0.7,
        "max_tokens": 4000,
    },
    "gpt-3.5-turbo": {
        "temperature": 0.7,
        "max_tokens": 2000,
    },
    "deepseek-chat": {
        "temperature": 0.7,
        "max_tokens": 2000,
    },
    "default": {
        "temperature": 0.7,
        "max_tokens": 2000,
    },
}


def get_model_defaults(model: str) -> dict:
    """
    获取模型的默认参数
    
    Args:
        model: 模型名称
        
    Returns:
        dict: 默认参数（temperature, max_tokens）
    """
    return MODEL_DEFAULTS.get(model, MODEL_DEFAULTS["default"])

