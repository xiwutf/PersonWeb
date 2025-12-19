"""
关系跟进助理服务
处理关系跟进相关的业务逻辑，包括互动总结和建议生成
"""

import json
import re
from typing import Dict, Any, Tuple, Optional, List

from app.core.llm_client import llm_client
from app.core.app_logging import logger
from app.core.prompt_loader import load_prompt, render_prompt, PromptLoadError, PromptRenderError
from app.models.dto import (
    RelationFollowupRequest,
    RelationFollowupResponseData,
    RelationSummary,
    RelationSignals,
    RelationPreferences,
    RelationNextAction,
    RelationMessageDraft,
    RelationStageSuggestion,
    RelationHeatScoreHint
)


class RelationFollowupService:
    """关系跟进助理服务类"""
    
    async def summarize(
        self,
        request: RelationFollowupRequest
    ) -> RelationFollowupResponseData:
        """
        生成互动总结和建议
        
        Args:
            request: 关系跟进请求数据
            
        Returns:
            RelationFollowupResponseData: 包含总结、建议、消息草案等的响应数据
        """
        logger.info(
            f"生成关系跟进总结: nickname={request.person.nickname}, "
            f"stage={request.person.stage}, interaction_type={request.interaction.type}"
        )
        
        # 构建 Prompt
        system_prompt, user_prompt = self._build_prompts(request)
        
        # 调用大模型
        result = await llm_client.chat(
            prompt=user_prompt,
            system_prompt=system_prompt,
            temperature=0.7,
            max_tokens=3000
        )
        
        # 解析 AI 返回的 JSON
        response = self._parse_ai_response(result["reply"])
        
        logger.info(f"关系跟进总结生成完成: nickname={request.person.nickname}")
        
        return response
    
    def _build_prompts(
        self,
        request: RelationFollowupRequest
    ) -> Tuple[str, str]:
        """
        构建系统提示词和用户提示词
        
        Args:
            request: 关系跟进请求数据
            
        Returns:
            Tuple[str, str]: (system_prompt, user_prompt)
            
        Raises:
            PromptLoadError: Prompt 文件加载失败
            PromptRenderError: Prompt 渲染失败
        """
        try:
            # 加载系统 Prompt
            system_prompt = load_prompt("relation_followup/system_v1.txt")
        except PromptLoadError as e:
            logger.error(f"加载系统 Prompt 失败: {e}")
            raise
        
        try:
            # 加载用户 Prompt 模板
            template = load_prompt("relation_followup/user_v1.txt")
            
            # 准备阶段文本
            stages = ["新认识", "熟悉中", "准备约见", "已见面", "升温中", "观察期", "已结束"]
            stage_text = stages[request.person.stage] if request.person.stage is not None and 0 <= request.person.stage < len(stages) else "未知"
            
            # 准备互动类型文本
            interaction_types = ["文字", "语音", "通话", "见面", "其他"]
            interaction_type_text = interaction_types[request.interaction.type] if 0 <= request.interaction.type < len(interaction_types) else "未知"
            
            # 准备变量
            vars_dict = {
                "nickname": request.person.nickname,
                "stage": stage_text,
                "tags": ", ".join(request.person.tags) if request.person.tags else "无",
                "last_contact_at": request.person.last_contact_at or "无",
                "last_meet_at": request.person.last_meet_at or "无",
                "current_next_action": request.person.current_next_action or "无",
                "history_key_points": request.history_key_points or "无",
                "interaction_type": interaction_type_text,
                "occurred_at": request.interaction.occurred_at,
                "interaction_summary": request.interaction.summary,
                "chat_text": request.interaction.chat_text or "",
                "user_goal": request.user_preference.user_goal if request.user_preference else "未指定",
                "user_style": request.user_preference.user_style if request.user_preference else "未指定",
                "time_constraints": request.user_preference.time_constraints if request.user_preference else "无"
            }
            
            # 渲染模板
            user_prompt = render_prompt(template, **vars_dict)
            
            return system_prompt, user_prompt
            
        except (PromptLoadError, PromptRenderError) as e:
            logger.error(f"构建用户 Prompt 失败: {e}")
            raise
    
    def _parse_ai_response(
        self,
        ai_response: str
    ) -> RelationFollowupResponseData:
        """
        解析 AI 返回的 JSON
        
        Args:
            ai_response: AI 返回的文本
            
        Returns:
            RelationFollowupResponseData: 解析后的响应数据
            
        Raises:
            ValueError: JSON 解析失败或数据格式错误
        """
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
            
            # 尝试直接解析 JSON
            try:
                data = json.loads(json_text)
            except json.JSONDecodeError:
                # 如果直接解析失败，尝试用正则提取最外层 JSON 对象
                logger.warning("直接解析 JSON 失败，尝试用正则提取")
                json_text = self._extract_json_from_text(json_text)
                if json_text:
                    data = json.loads(json_text)
                else:
                    raise ValueError("无法从响应中提取有效 JSON")
            
            # 验证并转换数据（使用 validate_json 方法确保字段齐全）
            validated_data = self._validate_json(data)
            
            # 构建响应对象
            # 构建响应对象
            response = RelationFollowupResponseData(
                summary=RelationSummary(
                    one_line=validated_data.get("summary", {}).get("one_line", ""),
                    key_facts=validated_data.get("summary", {}).get("key_facts", []),
                    signals=RelationSignals(
                        positive=validated_data.get("summary", {}).get("signals", {}).get("positive", []),
                        neutral=validated_data.get("summary", {}).get("signals", {}).get("neutral", []),
                        negative=validated_data.get("summary", {}).get("signals", {}).get("negative", [])
                    ),
                    preferences_updates=RelationPreferences(
                        likes=validated_data.get("summary", {}).get("preferences_updates", {}).get("likes", []),
                        dislikes=validated_data.get("summary", {}).get("preferences_updates", {}).get("dislikes", [])
                    ),
                    my_commitments=validated_data.get("summary", {}).get("my_commitments", []),
                    risks=validated_data.get("summary", {}).get("risks", [])
                ),
                next_actions=[
                    RelationNextAction(
                        title=action.get("title", ""),
                        why=action.get("why", ""),
                        when=action.get("when", ""),
                        priority=action.get("priority", 1)
                    )
                    for action in validated_data.get("next_actions", [])
                ],
                message_drafts=self._ensure_message_drafts_count([
                    RelationMessageDraft(
                        scene=draft.get("scene", ""),
                        text=draft.get("text", "")
                    )
                    for draft in validated_data.get("message_drafts", [])
                ]),
                followup_questions=validated_data.get("followup_questions", [])[:2],  # 最多 2 条
                stage_suggestion=RelationStageSuggestion(
                    current=validated_data.get("stage_suggestion", {}).get("current", ""),
                    suggested=validated_data.get("stage_suggestion", {}).get("suggested", ""),
                    reason=validated_data.get("stage_suggestion", {}).get("reason", "")
                ),
                heat_score_hint=RelationHeatScoreHint(
                    delta=validated_data.get("heat_score_hint", {}).get("delta", 0),
                    reason=validated_data.get("heat_score_hint", {}).get("reason", "")
                ),
                raw_text=ai_response
            )
            
            # 安全边界检查：过滤不当内容
            response = self._filter_inappropriate_content(response)
            
            return response
            
        except json.JSONDecodeError as e:
            logger.error(f"解析 AI 返回的 JSON 失败: {e}, 响应: {ai_response[:500]}")
            # JSON 解析失败时返回 fallback 结构
            return self._get_fallback_response(ai_response)
        except Exception as e:
            logger.error(f"处理 AI 响应失败: {e}, 响应: {ai_response[:500]}")
            # 其他异常也返回 fallback 结构
            return self._get_fallback_response(ai_response)
    
    def _validate_json(self, data: Dict[str, Any]) -> Dict[str, Any]:
        """
        校验 JSON 字段齐全，缺失字段补默认空值
        禁止抛异常导致前端崩溃
        
        Args:
            data: 解析后的 JSON 数据
            
        Returns:
            Dict[str, Any]: 验证后的数据（确保所有必需字段都存在）
        """
        # 默认结构
        fallback = {
            "summary": {
                "one_line": "",
                "key_facts": [],
                "signals": {
                    "positive": [],
                    "neutral": [],
                    "negative": []
                },
                "preferences_updates": {
                    "likes": [],
                    "dislikes": []
                },
                "my_commitments": [],
                "risks": []
            },
            "next_actions": [],
            "message_drafts": [],
            "followup_questions": [],
            "stage_suggestion": {
                "current": "",
                "suggested": "",
                "reason": ""
            },
            "heat_score_hint": {
                "delta": 0,
                "reason": ""
            }
        }
        
        # 递归合并数据，确保所有字段都存在
        def merge_dict(default: Dict, source: Dict) -> Dict:
            result = default.copy()
            for key, value in source.items():
                if key in result:
                    if isinstance(result[key], dict) and isinstance(value, dict):
                        result[key] = merge_dict(result[key], value)
                    else:
                        result[key] = value
            return result
        
        try:
            validated = merge_dict(fallback, data)
            return validated
        except Exception as e:
            logger.warning(f"验证 JSON 数据时出错，使用 fallback: {e}")
            return fallback
    
    def _extract_json_from_text(self, text: str) -> Optional[str]:
        """
        从文本中提取 JSON 对象（使用正则和括号匹配）
        
        Args:
            text: 可能包含 JSON 的文本
            
        Returns:
            Optional[str]: 提取出的 JSON 字符串，如果失败返回 None
        """
        # 尝试找到最外层的 JSON 对象（从第一个 { 开始）
        start_idx = text.find('{')
        if start_idx == -1:
            return None
        
        # 使用括号匹配找到对应的结束位置
        brace_count = 0
        for i in range(start_idx, len(text)):
            if text[i] == '{':
                brace_count += 1
            elif text[i] == '}':
                brace_count -= 1
                if brace_count == 0:
                    # 找到了完整的 JSON 对象
                    return text[start_idx:i+1]
        
        # 如果没有找到完整的对象，尝试用正则提取
        pattern = r'\{[^{}]*(?:\{[^{}]*\}[^{}]*)*\}'
        matches = re.findall(pattern, text)
        if matches:
            # 返回最长的匹配（可能是最完整的）
            return max(matches, key=len)
        
        return None
    
    def _ensure_message_drafts_count(self, drafts: List[RelationMessageDraft]) -> List[RelationMessageDraft]:
        """
        确保 message_drafts 有且仅有 3 条（不足则补齐空占位）
        
        Args:
            drafts: 当前的消息草案列表
            
        Returns:
            List[RelationMessageDraft]: 补齐后的列表（固定 3 条）
        """
        result = drafts.copy() if drafts else []
        
        # 确保有 3 条
        scenes = ["轻量联系", "推进/确认", "保守/留余地"]
        while len(result) < 3:
            index = len(result)
            result.append(RelationMessageDraft(
                scene=scenes[index] if index < len(scenes) else f"场景{index+1}",
                text=""  # 空文本占位
            ))
        
        # 如果超过 3 条，只保留前 3 条
        return result[:3]
    
    def _filter_inappropriate_content(self, response: RelationFollowupResponseData) -> RelationFollowupResponseData:
        """
        安全边界检查：过滤明显不当的内容
        
        Args:
            response: 原始响应数据
            
        Returns:
            RelationFollowupResponseData: 过滤后的响应数据
        """
        # 不当内容关键词（示例，可根据实际情况扩展）
        inappropriate_keywords = [
            "跟踪", "监视", "骚扰", "强迫", "威胁",
            "pua", "控制", "操控", "欺骗", "套路",
            "死缠烂打", "纠缠不休"
        ]
        
        # 检查 message_drafts 中的文本
        filtered_drafts = []
        has_inappropriate = False
        
        for draft in response.message_drafts:
            text_lower = (draft.text or "").lower()
            # 检查是否包含不当关键词
            if any(keyword in text_lower for keyword in inappropriate_keywords):
                has_inappropriate = True
                logger.warning(f"检测到不当内容，过滤消息草案: {draft.text[:50]}...")
                # 不添加到结果中
                continue
            filtered_drafts.append(draft)
        
        # 如果检测到不当内容，清空 message_drafts 并添加提示
        if has_inappropriate:
            logger.warning("检测到不当内容，清空消息草案")
            response.message_drafts = [
                RelationMessageDraft(
                    scene="提示",
                    text="建议调整为更礼貌的表达方式"
                )
            ] * 3
        else:
            # 确保仍有 3 条
            response.message_drafts = self._ensure_message_drafts_count(filtered_drafts)
        
        return response
    
    def _get_fallback_response(self, raw_text: str) -> RelationFollowupResponseData:
        """
        获取 fallback 响应（JSON 解析失败时使用）
        
        Args:
            raw_text: 原始响应文本
            
        Returns:
            RelationFollowupResponseData: fallback 响应数据
        """
        logger.warning("使用 fallback 响应结构")
        
        return RelationFollowupResponseData(
            summary=RelationSummary(
                one_line="AI 响应解析失败，请查看原始文本",
                key_facts=[],
                signals=RelationSignals(),
                preferences_updates=RelationPreferences(),
                my_commitments=[],
                risks=[]
            ),
            next_actions=[],
            message_drafts=self._ensure_message_drafts_count([]),
            followup_questions=[],
            stage_suggestion=RelationStageSuggestion(
                current="",
                suggested="",
                reason=""
            ),
            heat_score_hint=RelationHeatScoreHint(
                delta=0,
                reason=""
            ),
            raw_text=raw_text
        )

