using System.Text.Json.Serialization;

namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 名字评分维度
/// </summary>
public class NameScoresDto
{
    [JsonPropertyName("memorability")]
    public int Memorability { get; set; } // 好记度 0-100

    [JsonPropertyName("uniqueness")]
    public int Uniqueness { get; set; } // 独特性 0-100

    [JsonPropertyName("fit")]
    public int Fit { get; set; } // 贴合度 0-100

    [JsonPropertyName("aesthetics")]
    public int Aesthetics { get; set; } // 美观度 0-100
}

/// <summary>
/// 名字项 DTO
/// </summary>
public class NameItemDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("totalScore")]
    public int TotalScore { get; set; } // 总分 0-100

    [JsonPropertyName("scores")]
    public NameScoresDto Scores { get; set; } = new();

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty; // 评分理由（不超过40字）

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; } // 标签，如["古风", "短", "偏霸气"]
}

/// <summary>
/// 生成名字请求 DTO
/// </summary>
public class NameGenerateRequestDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty; // game | nickname | english

    [JsonPropertyName("style")]
    public List<string> Style { get; set; } = new(); // 风格列表

    [JsonPropertyName("gender")]
    public string? Gender { get; set; } // 男/女/中性

    [JsonPropertyName("length")]
    public string? Length { get; set; } // 短/中/长

    [JsonPropertyName("keywords")]
    public string? Keywords { get; set; } // 多个关键词用逗号分隔

    [JsonPropertyName("language")]
    public string? Language { get; set; } // 中文/英文/混合/自动

    [JsonPropertyName("banned")]
    public string? Banned { get; set; } // 禁用词，多个用逗号分隔

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; } // 用于"再来一批"时保持风格一致
}

/// <summary>
/// 生成名字响应 DTO
/// </summary>
public class NameGenerateResponseDto
{
    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<NameItemDto> Items { get; set; } = new();
}

/// <summary>
/// 创建收藏请求 DTO
/// </summary>
public class FavoriteCreateRequestDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("style")]
    public List<string> Style { get; set; } = new();

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("totalScore")]
    public int TotalScore { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("scores")]
    public NameScoresDto Scores { get; set; } = new();

    [JsonPropertyName("requestMeta")]
    public Dictionary<string, object>? RequestMeta { get; set; } // 保存输入条件摘要
}

/// <summary>
/// 收藏项响应 DTO
/// </summary>
public class NameFavoriteDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("style")]
    public string Style { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("totalScore")]
    public int TotalScore { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("metaJson")]
    public string? MetaJson { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 收藏列表响应 DTO
/// </summary>
public class FavoriteListResponseDto
{
    [JsonPropertyName("items")]
    public List<NameFavoriteDto> Items { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
}

