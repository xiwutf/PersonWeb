using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models
{
    [Table("monthly_kpi")]
    public class MonthlyKpi
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("goal_id")]
        public long GoalId { get; set; }

        [Required]
        [Column("year")]
        public int Year { get; set; }

        [Required]
        [Column("month")]
        public int Month { get; set; } // 1-12

        [Required]
        [MaxLength(255)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("target_value", TypeName = "DECIMAL(10,2)")]
        public decimal? TargetValue { get; set; }

        [Column("current_value", TypeName = "DECIMAL(10,2)")]
        public decimal CurrentValue { get; set; } = 0;

        [MaxLength(50)]
        [Column("unit")]
        public string? Unit { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "pending"; // pending, in_progress, completed

        [Column("progress")]
        public int Progress { get; set; } = 0; // 0-100

        [Column("notes", TypeName = "TEXT")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // 导航属性
        [ForeignKey("GoalId")]
        public virtual Goal? Goal { get; set; }
    }
}

