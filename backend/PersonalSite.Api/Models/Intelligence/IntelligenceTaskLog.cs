using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PersonalSite.Api.Models;

/// <summary>情报任务执行日志</summary>
[Table("intelligence_task_log")]
public class IntelligenceTaskLog
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("task_name")]
    public string TaskName { get; set; } = string.Empty; // collect | analyze | generate_report

    [MaxLength(50)]
    [Column("task_type")]
    public string? TaskType { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "running"; // running | success | failed

    [Required]
    [Column("start_time")]
    public DateTime StartTime { get; set; } = DateTime.Now;

    [Column("end_time")]
    public DateTime? EndTime { get; set; }

    [Column("message", TypeName = "text")]
    public string? Message { get; set; }

    [Column("detail_json", TypeName = "json")]
    public string? DetailJson { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>详细信息对象</summary>
    [NotMapped]
    public Dictionary<string, object>? Detail
    {
        get => string.IsNullOrEmpty(DetailJson) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(DetailJson);
        set => DetailJson = value == null ? null : JsonSerializer.Serialize(value);
    }
}
