"""
AI 工具服务
处理摘要、标题标签、代码解释等工具类业务逻辑
"""

from typing import Optional
from app.core.llm_client import llm_client
from app.core.logging import logger


class ToolsService:
    """AI 工具服务类"""
    
    async def summarize(
        self,
        text: str,
        max_length: Optional[int] = 100
    ) -> str:
        """
        生成文章摘要
        
        Args:
            text: 原文内容
            max_length: 期望摘要长度
            
        Returns:
            str: 生成的摘要
        """
        logger.info(f"生成摘要: text_length={len(text)}, max_length={max_length}")
        
        # 构建摘要提示词
        prompt = f"""请为以下文章生成一个简洁的摘要，摘要长度控制在 {max_length} 字以内：

{text}

摘要："""
        
        # 调用大模型
        result = await llm_client.chat(
            prompt=prompt,
            system_prompt="你是一个专业的文本摘要生成助手，擅长提取文章的核心要点。"
        )
        
        summary = result["reply"]
        logger.info(f"摘要生成完成: summary_length={len(summary)}")
        
        return summary
    
    async def generate_title_and_tags(
        self,
        text: str,
        max_tags: Optional[int] = 5
    ) -> dict:
        """
        生成文章标题和标签
        
        Args:
            text: 文章内容
            max_tags: 标签数量上限
            
        Returns:
            dict: 包含 title 和 tags 的字典
        """
        logger.info(f"生成标题标签: text_length={len(text)}, max_tags={max_tags}")
        
        # 构建提示词
        prompt = f"""请为以下文章生成一个吸引人的标题和 {max_tags} 个相关标签：

{text}

请以 JSON 格式返回，格式如下：
{{
    "title": "生成的标题",
    "tags": ["标签1", "标签2", ...]
}}"""
        
        # 调用大模型
        result = await llm_client.chat(
            prompt=prompt,
            system_prompt="你是一个专业的内容分析助手，擅长为文章生成标题和标签。"
        )
        
        # TODO: 解析 JSON 响应（当前为模拟实现）
        # 实际应该解析 result["reply"] 中的 JSON
        title = f"【模拟标题】{text[:30]}..."
        tags = [f"标签{i+1}" for i in range(max_tags)]
        
        logger.info(f"标题标签生成完成: title={title}, tags_count={len(tags)}")
        
        return {
            "title": title,
            "tags": tags
        }
    
    async def explain_code(
        self,
        code: str,
        language: Optional[str] = None
    ) -> str:
        """
        解释代码功能
        
        Args:
            code: 代码内容
            language: 编程语言
            
        Returns:
            str: 代码的中文解释
        """
        logger.info(f"解释代码: code_length={len(code)}, language={language}")
        
        # 构建提示词
        lang_info = f"（{language} 语言）" if language else ""
        prompt = f"""请用中文详细解释以下{lang_info}代码的功能和逻辑：

```{language or ''}
{code}
```

请用通俗易懂的中文解释代码的作用、主要逻辑和关键步骤："""
        
        # 调用大模型
        result = await llm_client.chat(
            prompt=prompt,
            system_prompt="你是一个专业的代码解释助手，擅长用中文解释代码的功能和逻辑。"
        )
        
        explanation = result["reply"]
        logger.info(f"代码解释完成: explanation_length={len(explanation)}")
        
        return explanation

