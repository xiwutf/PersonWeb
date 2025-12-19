using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 副业项目数据分析控制器
/// </summary>
[ApiController]
[Route("api/side-projects/analytics")]
public class SideProjectAnalyticsController : ControllerBase
{
    private readonly SideProjectAnalyticsService _analyticsService;
    private readonly ILogger<SideProjectAnalyticsController> _logger;

    public SideProjectAnalyticsController(
        SideProjectAnalyticsService analyticsService,
        ILogger<SideProjectAnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// 获取数据分析汇总
    /// </summary>
    [HttpGet("summary")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectAnalyticsSummaryDto>>> GetSummary(
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromQuery] string? type,
        [FromQuery] bool? isPublic)
    {
        try
        {
            var summary = await _analyticsService.GetAnalyticsSummaryAsync(start, end, type, isPublic);
            return Ok(ApiResponse.Success(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据分析汇总失败");
            return StatusCode(500, ApiResponse<SideProjectAnalyticsSummaryDto>.Error("获取数据分析汇总失败: " + ex.Message, 500));
        }
    }
}

