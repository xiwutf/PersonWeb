using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 媒体文件表
/// </summary>
[Table("media_file")]
public class MediaFile
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("url")]
    public string Url { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column("file_name")]
    public string? FileName { get; set; }

    [MaxLength(50)]
    [Column("file_type")]
    public string? FileType { get; set; }

    [Column("size")]
    public long? Size { get; set; }

    [Column("uploader_id")]
    public long? UploaderId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey("UploaderId")]
    public User? Uploader { get; set; }
}
