using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 项目创建/更新请求 DTO
/// </summary>
public class ProjectRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? CoverUrl { get; set; }

    [MaxLength(200)]
    public string? DemoUrl { get; set; }

    [MaxLength(200)]
    public string? GithubUrl { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Active, Completed, Archived

    public string? TechStack { get; set; } // JSON array or comma separated

    public string? Content { get; set; } // 源稿（Markdown，可选）

    public string? ContentHtml { get; set; } // 发布态 HTML（推荐 AI 直接写入）
}

