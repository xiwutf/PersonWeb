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

    public string? Content { get; set; } // Markdown content

    public int ViewCount { get; set; } = 0; // 访问量统计

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
