namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 认知说明书列表项 DTO
/// </summary>
public class CognitionDocListItemDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Status { get; set; } = "draft";
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 认知说明书详情 DTO
/// </summary>
public class CognitionDocDetailDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string ContentMd { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建认知说明书请求 DTO
/// </summary>
public class CognitionDocCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Summary { get; set; }
    public string ContentMd { get; set; } = string.Empty;
    public string? Status { get; set; }
}

/// <summary>
/// 更新认知说明书请求 DTO
/// </summary>
public class CognitionDocUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string ContentMd { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
}

/// <summary>
/// 发布状态更新请求 DTO
/// </summary>
public class CognitionDocPublishDto
{
    public string Status { get; set; } = "published";
}
