using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Api.Models;

public class Metric
{
    public Guid Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public int Steps { get; set; }

    public double Sleep { get; set; }

    public double Weight { get; set; }

    public decimal NetWorth { get; set; }

    public int Energy { get; set; } // 0-100

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
