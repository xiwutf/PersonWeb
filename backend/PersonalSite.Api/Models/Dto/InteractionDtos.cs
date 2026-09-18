using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Api.Models.Dto;

public class InteractionSummaryDto
{
    public int LikeCount { get; set; }
    public bool Liked { get; set; }
    public int CommentCount { get; set; }
}

public class LikeRequestDto
{
    [Required]
    [MaxLength(100)]
    public string VisitorId { get; set; } = string.Empty;
}

public class PublicCommentDto
{
    public long Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateCommentRequestDto
{
    [Required]
    [MaxLength(32)]
    public string TargetType { get; set; } = string.Empty;

    [Required]
    [MaxLength(180)]
    public string TargetId { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Nickname { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    /// <summary>可选访客标识，用于频率限制</summary>
    [MaxLength(100)]
    public string? VisitorId { get; set; }

    /// <summary>Turnstile token 预留位（V1 未接入时可空）</summary>
    [MaxLength(2048)]
    public string? CaptchaToken { get; set; }
}

public class AdminCommentDto
{
    public long Id { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Ip { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class AdminCommentListDto
{
    public int Total { get; set; }
    public List<AdminCommentDto> List { get; set; } = new();
}
