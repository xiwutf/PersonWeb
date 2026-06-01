using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MetricsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Metric>>>> GetMetrics()
    {
        var rows = await _context.DashboardMetrics
            .OrderBy(m => m.Date)
            .ToListAsync();

        var metrics = rows.Select(MapToMetric).ToList();
        return Ok(ApiResponse<List<Metric>>.Success(metrics));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Metric>>> CreateOrUpdateMetric(Metric metric)
    {
        var existing = await _context.DashboardMetrics
            .FirstOrDefaultAsync(m => m.Date.Date == metric.Date.Date);

        DashboardMetric saved;
        if (existing != null)
        {
            existing.Steps = metric.Steps;
            existing.Sleep = (decimal)metric.Sleep;
            existing.Weight = (decimal)metric.Weight;
            existing.NetWorth = metric.NetWorth;
            existing.Energy = metric.Energy;
            existing.UpdatedAt = DateTime.Now;
            saved = existing;
        }
        else
        {
            saved = new DashboardMetric
            {
                Date = metric.Date.Date,
                Steps = metric.Steps,
                Sleep = (decimal)metric.Sleep,
                Weight = (decimal)metric.Weight,
                NetWorth = metric.NetWorth,
                Energy = metric.Energy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            _context.DashboardMetrics.Add(saved);
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<Metric>.Success(MapToMetric(saved)));
    }

    private static Metric MapToMetric(DashboardMetric source)
    {
        return new Metric
        {
            Id = Guid.Empty,
            Date = source.Date,
            Steps = source.Steps,
            Sleep = (double)source.Sleep,
            Weight = source.Weight.HasValue ? (double)source.Weight.Value : 0,
            NetWorth = source.NetWorth ?? 0,
            Energy = source.Energy,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
        };
    }
}
