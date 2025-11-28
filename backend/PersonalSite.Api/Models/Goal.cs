using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models
{
    [Table("goal")]
    public class Goal
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("year")]
        public int Year { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description", TypeName = "TEXT")]
        public string? Description { get; set; }

        [MaxLength(50)]
        [Column("category")]
        public string? Category { get; set; }

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
        public string Status { get; set; } = "active"; // active, completed, archived

        [Column("progress")]
        public int Progress { get; set; } = 0; // 0-100

        [Column("start_date", TypeName = "DATE")]
        public DateTime? StartDate { get; set; }

        [Column("end_date", TypeName = "DATE")]
        public DateTime? EndDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // 导航属性
        public virtual ICollection<MonthlyKpi>? MonthlyKpis { get; set; }
    }
}

