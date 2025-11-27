using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 按天 PV/UV 统计表
/// </summary>
[Table("pv_uv_daily")]
public class PvUvDaily
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("stat_date")]
    public DateTime StatDate { get; set; }

    [Column("pv")]
    public int Pv { get; set; } = 0;

    [Column("uv")]
    public int Uv { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
