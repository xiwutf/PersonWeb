using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Api.Models;

public class Project
{
    public Guid Id { get; set; }

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

    public string? Content { get; set; } // 源稿：Markdown 或历史正文

    public string? ContentHtml { get; set; } // 发布态 HTML（前台优先）

    public int ViewCount { get; set; } = 0; // 访问量统计

    // AI 生成字段
    [MaxLength(200)]
    public string? AiTitle { get; set; }

    public string? AiHighlights { get; set; } // JSON 或分号分隔字符串

    public string? AiDescription { get; set; } // Markdown 格式

    public string? AiScenarios { get; set; }

    [MaxLength(500)]
    public string? AiTargetUsers { get; set; }

    [MaxLength(200)]
    public string? AiShortCardText { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
