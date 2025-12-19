"""
请求/响应数据模型定义
使用 Pydantic 定义所有 API 的请求和响应模型
"""

from pydantic import BaseModel, Field
from typing import Optional, Dict, List, Any


# ==================== 通用响应模型 ====================

class BaseResponse(BaseModel):
    """基础响应模型"""
    success: bool
    error_code: Optional[str] = None
    message: str = ""
    data: Optional[Any] = None


# ==================== 健康检查相关 ====================

class HealthResponse(BaseModel):
    """健康检查响应数据"""
    status: str
    service: str
    version: str
    timestamp: Optional[str] = None


# ==================== 聊天相关 ====================

class ChatRequest(BaseModel):
    """聊天请求模型"""
    user_id: str = Field(..., description="用户 ID")
    session_id: Optional[str] = Field(None, description="会话 ID，用于上下文管理")
    message: str = Field(..., min_length=1, description="用户输入的消息")
    model: Optional[str] = Field(None, description="指定使用的模型")
    meta: Optional[Dict[str, Any]] = Field(None, description="额外信息，如来源、UA 等")


class ChatUsage(BaseModel):
    """Token 使用量"""
    prompt_tokens: int = Field(..., alias="promptTokens")
    completion_tokens: int = Field(..., alias="completionTokens")
    total_tokens: int = Field(..., alias="totalTokens")
    
    class Config:
        populate_by_name = True  # 允许使用字段名或别名


class ChatResponseData(BaseModel):
    """聊天响应数据"""
    reply: str = Field(..., description="模型回复文本")
    model: str = Field(..., description="实际使用的模型名称")
    usage: Optional[ChatUsage] = Field(None, description="Token 使用量")


# ==================== 网站聊天相关 ====================

class WebsiteChatMessage(BaseModel):
    """网站聊天消息"""
    role: str = Field(..., description="角色: system | user | assistant")
    content: str = Field(..., description="消息内容")


class WebsiteChatRequest(BaseModel):
    """网站聊天请求模型"""
    messages: List[WebsiteChatMessage] = Field(..., min_items=1, description="消息列表（包含 system 和 user 消息）")
    scene: str = Field("website-chat", description="场景标识")
    extra: Optional[Dict[str, Any]] = Field(None, description="额外信息，如页面、traceId 等")


class WebsiteChatResponseData(BaseModel):
    """网站聊天响应数据"""
    content: str = Field(..., description="AI 回复内容")
    usage: Optional[ChatUsage] = Field(None, description="Token 使用量")
    
    class Config:
        populate_by_name = True  # 允许使用字段名或别名


# ==================== AI 工具相关 ====================

class SummarizeRequest(BaseModel):
    """文章摘要请求模型"""
    text: str = Field(..., min_length=1, description="原文内容")
    max_length: Optional[int] = Field(100, ge=10, le=500, description="期望摘要长度")


class SummarizeResponseData(BaseModel):
    """摘要响应数据"""
    summary: str = Field(..., description="生成的摘要")


class TitleAndTagsRequest(BaseModel):
    """标题和标签生成请求模型"""
    text: str = Field(..., min_length=1, description="文章内容")
    max_tags: Optional[int] = Field(5, ge=1, le=20, description="标签数量上限")


class TitleAndTagsResponseData(BaseModel):
    """标题和标签响应数据"""
    title: str = Field(..., description="生成的标题")
    tags: List[str] = Field(..., description="生成的标签列表")


class CodeExplainRequest(BaseModel):
    """代码解释请求模型"""
    code: str = Field(..., min_length=1, description="代码内容")
    language: Optional[str] = Field(None, description="编程语言")


class CodeExplainResponseData(BaseModel):
    """代码解释响应数据"""
    explanation: str = Field(..., description="代码功能与逻辑的中文解释")


# ==================== RAG 知识库相关 ====================

class UpsertDocRequest(BaseModel):
    """文档入库/更新请求模型"""
    doc_id: str = Field(..., description="文档 ID")
    user_id: str = Field(..., description="用户 ID")
    title: str = Field(..., description="文档标题")
    content: str = Field(..., min_length=1, description="文档内容")
    tags: Optional[List[str]] = Field(None, description="文档标签")
    url: Optional[str] = Field(None, description="文档 URL")


class UpsertDocResponseData(BaseModel):
    """文档入库响应数据"""
    doc_id: str
    success: bool


class RelevantDoc(BaseModel):
    """相关文档信息"""
    doc_id: str
    title: str
    url: Optional[str] = None
    score: float = Field(..., ge=0.0, le=1.0, description="相似度分数")


class RAGQueryRequest(BaseModel):
    """知识库问答请求模型"""
    user_id: str = Field(..., description="用户 ID")
    query: str = Field(..., min_length=1, description="查询问题")
    top_k: Optional[int] = Field(5, ge=1, le=20, description="返回相关文档数量")


class RAGQueryResponseData(BaseModel):
    """知识库问答响应数据"""
    answer: str = Field(..., description="生成的答案")
    relevant_docs: List[RelevantDoc] = Field(..., description="相关文档列表")


# ==================== 文档知识管家 Agent 相关 ====================

class DocumentProcessRequest(BaseModel):
    """文档处理请求模型"""
    document_id: str = Field(..., description="文档 ID")
    file_path: str = Field(..., description="文件路径")
    file_type: str = Field(..., description="文件类型 (pdf, docx, txt, etc.)")
    user_id: str = Field(..., description="用户 ID")


class DocumentChunkData(BaseModel):
    """文档分段数据"""
    index: int = Field(..., description="分段索引")
    content: str = Field(..., description="分段内容")
    summary: Optional[str] = Field(None, description="分段摘要")
    metadata: Optional[Dict[str, Any]] = Field(None, description="元数据")
    vector_id: Optional[str] = Field(None, description="向量数据库中的 ID")


class DocumentProcessResponseData(BaseModel):
    """文档处理响应数据"""
    document_id: str = Field(..., description="文档 ID")
    summary: str = Field(..., description="文档摘要")
    knowledge_structure: str = Field(..., description="知识结构 (JSON 字符串)")
    total_chunks: int = Field(..., description="分段总数")
    chunks: List[DocumentChunkData] = Field(..., description="分段列表")


class DocumentQueryRequest(BaseModel):
    """文档问答请求模型"""
    document_id: str = Field(..., description="文档 ID")
    user_id: str = Field(..., description="用户 ID")
    query: str = Field(..., min_length=1, description="用户问题")
    top_k: int = Field(5, ge=1, le=20, description="返回相关文档片段数量")


class RelevantChunk(BaseModel):
    """相关文档片段"""
    chunk_id: int = Field(..., description="分段 ID")
    chunk_index: int = Field(..., description="分段索引")
    content: str = Field(..., description="内容片段")
    summary: Optional[str] = Field(None, description="分段摘要")
    score: float = Field(..., ge=0.0, le=1.0, description="相似度分数")


class DocumentQueryResponseData(BaseModel):
    """文档问答响应数据"""
    answer: str = Field(..., description="AI 生成的答案")
    relevant_chunks: List[RelevantChunk] = Field(..., description="相关文档片段列表")
    confidence: Optional[float] = Field(None, ge=0.0, le=1.0, description="置信度")


# ==================== 智能取名助手相关 ====================

class NameScores(BaseModel):
    """名字评分维度"""
    memorability: int = Field(..., ge=0, le=100, description="好记度 0-100")
    uniqueness: int = Field(..., ge=0, le=100, description="独特性 0-100")
    fit: int = Field(..., ge=0, le=100, description="贴合度 0-100")
    aesthetics: int = Field(..., ge=0, le=100, description="美观度 0-100")


class NameItem(BaseModel):
    """名字项"""
    name: str = Field(..., description="名字")
    totalScore: int = Field(..., ge=0, le=100, description="总分 0-100")
    scores: NameScores = Field(..., description="评分维度")
    reason: str = Field(..., max_length=40, description="评分理由（不超过40字）")
    tags: Optional[List[str]] = Field(None, description="标签，如['古风', '短', '偏霸气']")


class NameGenerateRequest(BaseModel):
    """生成名字请求模型"""
    type: str = Field(..., description="取名类型: game | nickname | english")
    style: List[str] = Field(..., min_items=1, description="风格列表")
    gender: Optional[str] = Field(None, description="性别: 男/女/中性")
    length: Optional[str] = Field(None, description="名字长度: 短/中/长")
    keywords: Optional[str] = Field(None, description="关键词，多个用逗号分隔")
    language: Optional[str] = Field("自动", description="语言: 中文/英文/混合/自动")
    banned: Optional[str] = Field(None, description="禁用词，多个用逗号分隔")
    traceId: Optional[str] = Field(None, description="追踪ID，用于'再来一批'时保持风格一致")


class NameGenerateResponseData(BaseModel):
    """生成名字响应数据"""
    traceId: str = Field(..., description="追踪ID")
    items: List[NameItem] = Field(..., min_items=1, max_items=20, description="生成的名字列表（20个）")


# ==================== 关系跟进助理相关 ====================

class RelationPersonInfo(BaseModel):
    """关系对象信息"""
    nickname: str = Field(..., description="昵称")
    stage: Optional[int] = Field(None, description="阶段：0=新认识, 1=熟悉中, 2=准备约见, 3=已见面, 4=升温中, 5=观察期, 6=已结束")
    tags: Optional[List[str]] = Field(None, description="标签列表")
    last_contact_at: Optional[str] = Field(None, description="上次联系时间")
    last_meet_at: Optional[str] = Field(None, description="上次见面时间")
    current_next_action: Optional[str] = Field(None, description="当前下一步行动")


class RelationInteractionInfo(BaseModel):
    """互动信息"""
    type: int = Field(..., description="互动类型：0=文字, 1=语音, 2=通话, 3=见面, 4=其他")
    occurred_at: str = Field(..., description="发生时间")
    summary: str = Field(..., description="摘要")
    chat_text: Optional[str] = Field(None, description="聊天片段（可选）")


class RelationUserPreference(BaseModel):
    """用户偏好"""
    user_goal: Optional[str] = Field(None, description="用户目标：例如：尽快约见/稳步推进/先观察/礼貌结束")
    user_style: Optional[str] = Field(None, description="用户风格：例如：幽默/认真/简洁/温柔")
    time_constraints: Optional[str] = Field(None, description="时间约束：例如：本周很忙/只能周末")


class RelationFollowupRequest(BaseModel):
    """关系跟进请求模型"""
    person: RelationPersonInfo = Field(..., description="对象信息")
    history_key_points: Optional[str] = Field(None, description="历史关键信息（可选）")
    interaction: RelationInteractionInfo = Field(..., description="本次互动")
    user_preference: Optional[RelationUserPreference] = Field(None, description="用户偏好（可选）")


class RelationSignals(BaseModel):
    """信号分类"""
    positive: List[str] = Field(default_factory=list, description="积极信号")
    neutral: List[str] = Field(default_factory=list, description="中性信号")
    negative: List[str] = Field(default_factory=list, description="消极信号")


class RelationPreferences(BaseModel):
    """偏好更新"""
    likes: List[str] = Field(default_factory=list, description="喜欢/在意的点")
    dislikes: List[str] = Field(default_factory=list, description="不喜欢/雷点")


class RelationSummary(BaseModel):
    """互动总结"""
    one_line: str = Field(..., description="一句话总结")
    key_facts: List[str] = Field(default_factory=list, description="关键事实")
    signals: RelationSignals = Field(default_factory=RelationSignals, description="信号分类")
    preferences_updates: RelationPreferences = Field(default_factory=RelationPreferences, description="偏好更新")
    my_commitments: List[str] = Field(default_factory=list, description="我方承诺")
    risks: List[str] = Field(default_factory=list, description="潜在风险")


class RelationNextAction(BaseModel):
    """下一步行动"""
    title: str = Field(..., description="行动标题")
    why: str = Field(..., description="原因")
    when: str = Field(..., description="建议时间")
    priority: int = Field(..., ge=1, le=3, description="优先级 1-3（1最高）")


class RelationMessageDraft(BaseModel):
    """消息草案"""
    scene: str = Field(..., description="场景")
    text: str = Field(..., description="可直接发送的消息")


class RelationStageSuggestion(BaseModel):
    """阶段建议"""
    current: str = Field(..., description="当前阶段")
    suggested: str = Field(..., description="建议阶段")
    reason: str = Field(default="", description="调整原因")


class RelationHeatScoreHint(BaseModel):
    """热度分数提示"""
    delta: int = Field(default=0, description="热度变化")
    reason: str = Field(default="", description="变化原因")


class RelationFollowupResponseData(BaseModel):
    """关系跟进响应数据"""
    summary: RelationSummary = Field(..., description="互动总结")
    next_actions: List[RelationNextAction] = Field(default_factory=list, description="下一步行动列表")
    message_drafts: List[RelationMessageDraft] = Field(default_factory=list, description="消息草案列表")
    followup_questions: List[str] = Field(default_factory=list, description="后续问题")
    stage_suggestion: RelationStageSuggestion = Field(..., description="阶段建议")
    heat_score_hint: RelationHeatScoreHint = Field(..., description="热度分数提示")
    raw_text: Optional[str] = Field(None, description="原始回复文本（用于调试）")