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
        var metrics = await _context.Metrics
            .OrderBy(m => m.Date)
            .ToListAsync();
        return Ok(ApiResponse<List<Metric>>.Success(metrics));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Metric>>> CreateOrUpdateMetric(Metric metric)
    {
        // Check if exists for date
        var existing = await _context.Metrics
            .FirstOrDefaultAsync(m => m.Date.Date == metric.Date.Date);

        if (existing != null)
        {
            existing.Steps = metric.Steps;
            existing.Sleep = metric.Sleep;
            existing.Weight = metric.Weight;
            existing.NetWorth = metric.NetWorth;
            existing.Energy = metric.Energy;
            existing.UpdatedAt = DateTime.Now;
        }
        else
        {
            metric.Id = Guid.NewGuid();
            metric.CreatedAt = DateTime.Now;
            metric.UpdatedAt = DateTime.Now;
            _context.Metrics.Add(metric);
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<Metric>.Success(metric));
    }
}
