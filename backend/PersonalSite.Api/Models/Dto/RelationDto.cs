namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 关系对象列表项 DTO
/// </summary>
public class RelationPersonListItemDto
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string? Source { get; set; }
    public string? City { get; set; }
    public List<string> Tags { get; set; } = new();
    public int Stage { get; set; }
    public int HeatScore { get; set; }
    public DateTime? LastContactAt { get; set; }
    public DateTime? LastMeetAt { get; set; }
    public string? NextAction { get; set; }
    public DateTime? RemindAt { get; set; }
    public DateTime? ObservationStartedAt { get; set; }
    public DateTime? ObservationExpectedEndAt { get; set; }
    public DateTime? ObservationLastRemindedAt { get; set; }
    public string? ObservationReason { get; set; }
    public bool ObservationDecisionPending { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 关系对象详情 DTO
/// </summary>
public class RelationPersonDetailDto
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string? Source { get; set; }
    public string? City { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Preferences { get; set; }
    public int Stage { get; set; }
    public int HeatScore { get; set; }
    public DateTime? LastContactAt { get; set; }
    public DateTime? LastMeetAt { get; set; }
    public string? NextAction { get; set; }
    public DateTime? RemindAt { get; set; }
    public string? Notes { get; set; }
    public DateTime? ObservationStartedAt { get; set; }
    public DateTime? ObservationExpectedEndAt { get; set; }
    public DateTime? ObservationLastRemindedAt { get; set; }
    public string? ObservationReason { get; set; }
    public bool ObservationDecisionPending { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建关系对象请求 DTO
/// </summary>
public class CreateRelationPersonDto
{
    public string Nickname { get; set; } = string.Empty;
    public string? Source { get; set; }
    public string? City { get; set; }
    public List<string>? Tags { get; set; }
    public string? Preferences { get; set; }
    public int Stage { get; set; } = 0;
    public string? Notes { get; set; }
}

/// <summary>
/// 更新关系对象请求 DTO
/// </summary>
public class UpdateRelationPersonDto
{
    public string? Nickname { get; set; }
    public string? Source { get; set; }
    public string? City { get; set; }
    public List<string>? Tags { get; set; }
    public string? Preferences { get; set; }
    public int? Stage { get; set; }
    public int? HeatScore { get; set; }
    public string? NextAction { get; set; }
    public DateTime? RemindAt { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 互动记录 DTO
/// </summary>
public class RelationInteractionDto
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public int Type { get; set; }
    public DateTime OccurredAt { get; set; }
    public string Summary { get; set; } = string.Empty;
    public Dictionary<string, object>? KeyPoints { get; set; }
    public int? MyFeeling { get; set; }
    public string? AiSuggestion { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建互动记录请求 DTO
/// </summary>
public class CreateRelationInteractionDto
{
    public int Type { get; set; } = 0;
    public DateTime OccurredAt { get; set; } = DateTime.Now;
    public string Summary { get; set; } = string.Empty;
    public Dictionary<string, object>? KeyPoints { get; set; }
    public int? MyFeeling { get; set; }
}

/// <summary>
/// 更新互动记录请求 DTO
/// </summary>
public class UpdateRelationInteractionDto
{
    public int? Type { get; set; }
    public DateTime? OccurredAt { get; set; }
    public string? Summary { get; set; }
    public Dictionary<string, object>? KeyPoints { get; set; }
    public int? MyFeeling { get; set; }
    public string? AiSuggestion { get; set; }
}

/// <summary>
/// 任务 DTO
/// </summary>
public class RelationTaskDto
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? DueAt { get; set; }
    public int Priority { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建任务请求 DTO
/// </summary>
public class CreateRelationTaskDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime? DueAt { get; set; }
    public int Priority { get; set; } = 1;
}

/// <summary>
/// 更新任务请求 DTO
/// </summary>
public class UpdateRelationTaskDto
{
    public string? Title { get; set; }
    public DateTime? DueAt { get; set; }
    public int? Priority { get; set; }
    public int? Status { get; set; }
}

/// <summary>
/// AI 总结请求 DTO
/// </summary>
public class RelationAiSummarizeRequest
{
    public RelationPersonInfoDto Person { get; set; } = new();
    public string? HistoryKeyPoints { get; set; }
    public RelationInteractionInfoDto Interaction { get; set; } = new();
    public RelationUserPreferenceDto? UserPreference { get; set; }
}

/// <summary>
/// 关系对象信息 DTO（用于 AI 请求）
/// </summary>
public class RelationPersonInfoDto
{
    public string Nickname { get; set; } = string.Empty;
    public int? Stage { get; set; }
    public List<string>? Tags { get; set; }
    public string? LastContactAt { get; set; }
    public string? LastMeetAt { get; set; }
    public string? CurrentNextAction { get; set; }
}

/// <summary>
/// 互动信息 DTO（用于 AI 请求）
/// </summary>
public class RelationInteractionInfoDto
{
    public int Type { get; set; }
    public string OccurredAt { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? ChatText { get; set; }
}

/// <summary>
/// 用户偏好 DTO（用于 AI 请求）
/// </summary>
public class RelationUserPreferenceDto
{
    public string? UserGoal { get; set; }
    public string? UserStyle { get; set; }
    public string? TimeConstraints { get; set; }
}

/// <summary>
/// AI 总结响应 DTO（完整结构）
/// </summary>
public class RelationAiSummarizeResponse
{
    public RelationSummaryDto? Summary { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("next_actions")]
    public List<RelationNextActionDto>? NextActions { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("message_drafts")]
    public List<RelationMessageDraftDto>? MessageDrafts { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("followup_questions")]
    public List<string>? FollowupQuestions { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("stage_suggestion")]
    public RelationStageSuggestionDto? StageSuggestion { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("heat_score_hint")]
    public RelationHeatScoreHintDto? HeatScoreHint { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("raw_text")]
    public string? RawText { get; set; }
}

/// <summary>
/// 总结 DTO
/// </summary>
public class RelationSummaryDto
{
    [System.Text.Json.Serialization.JsonPropertyName("one_line")]
    public string? OneLine { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("key_facts")]
    public List<string>? KeyFacts { get; set; }
    
    public RelationSignalsDto? Signals { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("preferences_updates")]
    public RelationPreferencesDto? PreferencesUpdates { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("my_commitments")]
    public List<string>? MyCommitments { get; set; }
    
    public List<string>? Risks { get; set; }
}

/// <summary>
/// 信号分类 DTO
/// </summary>
public class RelationSignalsDto
{
    public List<string>? Positive { get; set; }
    public List<string>? Neutral { get; set; }
    public List<string>? Negative { get; set; }
}

/// <summary>
/// 偏好更新 DTO
/// </summary>
public class RelationPreferencesDto
{
    public List<string>? Likes { get; set; }
    public List<string>? Dislikes { get; set; }
}

/// <summary>
/// 下一步行动 DTO
/// </summary>
public class RelationNextActionDto
{
    public string? Title { get; set; }
    public string? Why { get; set; }
    public string? When { get; set; }
    public int Priority { get; set; }
}

/// <summary>
/// 消息草案 DTO
/// </summary>
public class RelationMessageDraftDto
{
    public string? Scene { get; set; }
    public string? Text { get; set; }
}

/// <summary>
/// 阶段建议 DTO
/// </summary>
public class RelationStageSuggestionDto
{
    public string? Current { get; set; }
    public string? Suggested { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// 热度分数提示 DTO
/// </summary>
public class RelationHeatScoreHintDto
{
    public int Delta { get; set; }
    public string? Reason { get; set; }
}

