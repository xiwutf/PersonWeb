using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("visit_logs")]
public class VisitLog
{
    [Key]
    [Column("id")]
    [MaxLength(36)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column("visitor_id")]
    [MaxLength(36)]
    [Required]
    public string VisitorId { get; set; } = string.Empty;

    [Column("ip")]
    [MaxLength(45)]
    public string? Ip { get; set; }

    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [Column("path")]
    [MaxLength(255)]
    public string? Path { get; set; }

    [Column("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
