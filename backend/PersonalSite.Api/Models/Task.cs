using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models
{
    [Table("task")]
    public class TaskItem
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description", TypeName = "TEXT")]
        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "pending"; // pending, in_progress, completed, cancelled

        [Column("priority")]
        public int Priority { get; set; } = 0; // 0-低, 1-中, 2-高, 3-紧急

        [MaxLength(50)]
        [Column("category")]
        public string? Category { get; set; }

        [MaxLength(500)]
        [Column("tags")]
        public string? Tags { get; set; }

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("estimated_hours", TypeName = "DECIMAL(5,2)")]
        public decimal? EstimatedHours { get; set; }

        [Column("actual_hours", TypeName = "DECIMAL(5,2)")]
        public decimal? ActualHours { get; set; }

        [Column("progress")]
        public int Progress { get; set; } = 0; // 0-100

        [Column("parent_id")]
        public long? ParentId { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // 导航属性
        [ForeignKey("ParentId")]
        public virtual TaskItem? Parent { get; set; }

        public virtual ICollection<TaskItem>? SubTasks { get; set; }
    }
}

