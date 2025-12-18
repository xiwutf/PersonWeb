"""
智能取名助手服务
处理名字生成、评分等业务逻辑
"""

import json
import re
import uuid
from typing import List, Optional, Dict, Any

from app.core.llm_client import llm_client
from app.core.app_logging import logger
from app.core.prompt_loader import load_prompt, render_prompt, PromptLoadError, PromptRenderError
from app.models.dto import (
    NameGenerateRequest,
    NameGenerateResponseData,
    NameItem,
    NameScores
)


class NameToolService:
    """智能取名助手服务类"""
    
    async def generate_names(
        self,
        request: NameGenerateRequest
    ) -> NameGenerateResponseData:
        """
        生成名字
        
        Args:
            request: 生成名字请求
            
        Returns:
            NameGenerateResponseData: 包含生成的名字列表和 traceId
        """
        # 生成或使用传入的 traceId
        trace_id = request.traceId or str(uuid.uuid4()).replace("-", "")
        
        logger.info(
            f"生成名字: type={request.type}, style={request.style}, "
            f"traceId={trace_id}"
        )
        
        # 构建 Prompt
        system_prompt = self._build_system_prompt()
        user_prompt = self._build_user_prompt(request, trace_id)
        
        # 调用大模型
        result = await llm_client.chat(
            prompt=user_prompt,
            system_prompt=system_prompt,
            temperature=0.8,  # 提高创造性
            max_tokens=3000  # 生成20个名字需要较多token
        )
        
        # 解析 AI 返回的 JSON
        response = self._parse_ai_response(result["reply"], trace_id)
        
        # 去重处理
        response.items = self._deduplicate_names(response.items)
        
        # 如果不足20个，尝试补生成一次
        if len(response.items) < 20:
            logger.warning(
                f"生成的名字数量不足20个（{len(response.items)}），尝试补生成"
            )
            supplement_response = await self._generate_supplement(
                request, trace_id, response.items
            )
            response.items.extend(supplement_response.items)
            response.items = self._deduplicate_names(response.items)
        
        # 限制为20个
        response.items = response.items[:20]
        
        # 验证禁用词
        if request.banned:
            banned_words = [
                w.strip() for w in request.banned.split(",")
                if w.strip()
            ]
            response.items = self._filter_banned_words(response.items, banned_words)
        
        logger.info(f"名字生成完成: count={len(response.items)}, traceId={trace_id}")
        
        return response
    
    def _build_system_prompt(self) -> str:
        """
        构建系统提示词
        
        从文件加载：app/prompts/name_tool/system_v1.txt
        
        Returns:
            str: 系统提示词
            
        Raises:
            PromptLoadError: Prompt 文件加载失败
        """
        try:
            return load_prompt("name_tool/system_v1.txt")
        except PromptLoadError as e:
            logger.error(f"加载系统 Prompt 失败: {e}")
            raise
    
    def _build_user_prompt(self, request: NameGenerateRequest, trace_id: str) -> str:
        """
        构建用户提示词
        
        从文件加载并渲染：app/prompts/name_tool/user_v1.txt
        
        Args:
            request: 生成名字请求
            trace_id: 追踪 ID
            
        Returns:
            str: 渲染后的用户提示词
            
        Raises:
            PromptLoadError: Prompt 文件加载失败
            PromptRenderError: Prompt 渲染失败
        """
        try:
            # 加载模板
            template = load_prompt("name_tool/user_v1.txt")
            
            # 准备变量
            vars_dict = {
                "type": request.type,
                "style": ", ".join(request.style),
                "gender": request.gender or "未指定",
                "length": request.length or "未指定",
                "keywords": request.keywords or "无",
                "language": request.language or "自动",
                "banned": request.banned or "无",
                "traceId": trace_id
            }
            
            # 渲染模板
            return render_prompt(template, **vars_dict)
        except (PromptLoadError, PromptRenderError) as e:
            logger.error(f"构建用户 Prompt 失败: {e}")
            raise
    
    def _parse_ai_response(self, ai_response: str, trace_id: str) -> NameGenerateResponseData:
        """解析 AI 返回的 JSON"""
        try:
            # 尝试提取 JSON（去除可能的 Markdown 代码块标记）
            json_text = ai_response.strip()
            
            # 移除可能的 Markdown 代码块标记
            if json_text.startswith("```json"):
                json_text = json_text[7:]
            elif json_text.startswith("```"):
                json_text = json_text[3:]
            
            if json_text.endswith("```"):
                json_text = json_text[:-3]
            
            json_text = json_text.strip()
            
            # 尝试解析 JSON
            data = json.loads(json_text)
            
            # 验证数据结构
            if "items" not in data:
                raise ValueError("AI 返回的 JSON 缺少 items 字段")
            
            if not isinstance(data["items"], list):
                raise ValueError("AI 返回的 items 不是数组")
            
            if len(data["items"]) == 0:
                raise ValueError("AI 返回的名字列表为空")
            
            # 转换为响应模型
            items = []
            for item_data in data["items"]:
                try:
                    item = NameItem(
                        name=item_data["name"],
                        totalScore=item_data.get("totalScore", 0),
                        scores=NameScores(
                            memorability=item_data.get("scores", {}).get("memorability", 0),
                            uniqueness=item_data.get("scores", {}).get("uniqueness", 0),
                            fit=item_data.get("scores", {}).get("fit", 0),
                            aesthetics=item_data.get("scores", {}).get("aesthetics", 0)
                        ),
                        reason=item_data.get("reason", ""),
                        tags=item_data.get("tags", [])
                    )
                    items.append(item)
                except Exception as e:
                    logger.warning(f"解析名字项失败: {item_data}, 错误: {e}")
                    continue
            
            if len(items) == 0:
                raise ValueError("解析后没有有效的名字项")
            
            response = NameGenerateResponseData(
                traceId=trace_id,
                items=items
            )
            
            return response
            
        except json.JSONDecodeError as e:
            logger.error(f"解析 AI 返回的 JSON 失败: {e}, 响应: {ai_response[:500]}")
            raise ValueError(f"AI 返回的 JSON 格式错误: {e}")
        except Exception as e:
            logger.error(f"处理 AI 响应失败: {e}, 响应: {ai_response[:500]}")
            raise
    
    def _deduplicate_names(self, items: List[NameItem]) -> List[NameItem]:
        """去重处理"""
        seen = set()
        result = []
        
        for item in items:
            name_lower = item.name.lower()
            if name_lower not in seen:
                seen.add(name_lower)
                result.append(item)
        
        return result
    
    async def _generate_supplement(
        self,
        request: NameGenerateRequest,
        trace_id: str,
        existing_items: List[NameItem]
    ) -> NameGenerateResponseData:
        """补生成（当数量不足时）"""
        existing_names = [item.name.lower() for item in existing_items]
        needed_count = 20 - len(existing_items)
        
        # 修改提示词，要求生成不同的名字
        supplement_prompt = self._build_user_prompt(request, trace_id)
        supplement_prompt += f"\n\n注意：已生成以下名字，请避免重复：{', '.join(existing_names[:10])}"
        supplement_prompt += f"\n请再生成 {needed_count} 个不同的名字。"
        
        system_prompt = self._build_system_prompt()
        
        result = await llm_client.chat(
            prompt=supplement_prompt,
            system_prompt=system_prompt,
            temperature=0.8,
            max_tokens=2000
        )
        
        response = self._parse_ai_response(result["reply"], trace_id)
        
        # 过滤掉已存在的名字
        existing_names_set = set(existing_names)
        response.items = [
            item for item in response.items
            if item.name.lower() not in existing_names_set
        ]
        
        return response
    
    def _filter_banned_words(
        self,
        items: List[NameItem],
        banned_words: List[str]
    ) -> List[NameItem]:
        """过滤禁用词"""
        if not banned_words:
            return items
        
        banned_lower = [w.lower() for w in banned_words]
        
        result = []
        for item in items:
            name_lower = item.name.lower()
            if not any(banned in name_lower for banned in banned_lower):
                result.append(item)
        
        return result

