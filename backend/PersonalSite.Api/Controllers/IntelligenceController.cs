using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>情报中心控制器</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IntelligenceController : ControllerBase
{
    private readonly IIntelligenceSourceService _sourceService;
    private readonly IIntelligenceContentService _contentService;
    private readonly IIntelligenceReportService _reportService;
    private readonly IIntelligenceTaskService _taskService;

    public IntelligenceController(
        IIntelligenceSourceService sourceService,
        IIntelligenceContentService contentService,
        IIntelligenceReportService reportService,
        IIntelligenceTaskService taskService)
    {
        _sourceService = sourceService;
        _contentService = contentService;
        _reportService = reportService;
        _taskService = taskService;
    }

    // ==================== 来源管理 ====================

    [HttpGet("sources")]
    public async Task<ActionResult<ApiResponse<List<SourceResponseDto>>>> GetSources(CancellationToken cancellationToken)
    {
        var result = await _sourceService.GetListAsync(cancellationToken);
        return Ok(ApiResponse<List<SourceResponseDto>>.Success(result));
    }

    [HttpGet("sources/{id}")]
    public async Task<ActionResult<ApiResponse<SourceResponseDto>>> GetSource(int id, CancellationToken cancellationToken)
    {
        var result = await _sourceService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return Ok(ApiResponse<SourceResponseDto>.Error("来源不存在"));
        return Ok(ApiResponse<SourceResponseDto>.Success(result));
    }

    [HttpPost("sources")]
    public async Task<ActionResult<ApiResponse<SourceResponseDto>>> CreateSource([FromBody] SourceRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.SourceName))
            return Ok(ApiResponse<SourceResponseDto>.Error("来源名称不能为空"));

        var result = await _sourceService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<SourceResponseDto>.Success(result));
    }

    [HttpPut("sources/{id}")]
    public async Task<ActionResult<ApiResponse<SourceResponseDto>>> UpdateSource(int id, [FromBody] SourceRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.SourceName))
            return Ok(ApiResponse<SourceResponseDto>.Error("来源名称不能为空"));

        var result = await _sourceService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
            return Ok(ApiResponse<SourceResponseDto>.Error("来源不存在"));

        return Ok(ApiResponse<SourceResponseDto>.Success(result));
    }

    [HttpDelete("sources/{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteSource(int id, CancellationToken cancellationToken = default)
    {
        var result = await _sourceService.DeleteAsync(id, cancellationToken);
        if (!result)
            return Ok(ApiResponse.Error("删除失败，来源不存在"));

        return Ok(ApiResponse.Success());
    }

    [HttpPut("sources/{id}/enabled")]
    public async Task<ActionResult<ApiResponse>> ToggleSource(int id, [FromBody] bool enabled, CancellationToken cancellationToken = default)
    {
        var result = await _sourceService.ToggleEnabledAsync(id, enabled, cancellationToken);
        if (!result)
            return Ok(ApiResponse.Error("操作失败，来源不存在"));

        return Ok(ApiResponse.Success());
    }

    // ==================== 内容查询 ====================

    [HttpGet("contents")]
    public async Task<ActionResult<ApiResponse<ContentListResponseDto>>> GetContents([FromQuery] ContentQueryDto dto, CancellationToken cancellationToken = default)
    {
        var (total, list) = await _contentService.GetPageAsync(dto, cancellationToken);
        return Ok(ApiResponse<ContentListResponseDto>.Success(new ContentListResponseDto { Total = total, List = list }));
    }

    [HttpGet("contents/{id}")]
    public async Task<ActionResult<ApiResponse<ContentItemDto>>> GetContent(int id, CancellationToken cancellationToken = default)
    {
        var result = await _contentService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return Ok(ApiResponse<ContentItemDto>.Error("内容不存在"));

        return Ok(ApiResponse<ContentItemDto>.Success(result));
    }

    [HttpGet("contents/stats")]
    public async Task<ActionResult<ApiResponse<List<CategoryStatDto>>>> GetCategoryStats(CancellationToken cancellationToken = default)
    {
        var result = await _contentService.GetCategoryStatsAsync(cancellationToken);
        return Ok(ApiResponse<List<CategoryStatDto>>.Success(result));
    }

    // ==================== 日报查询 ====================

    [HttpGet("reports/daily")]
    public async Task<ActionResult<ApiResponse<object>>> GetReports([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (total, list) = await _reportService.GetListAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiResponse<object>.Success(new { Total = total, List = list }));
    }

    [HttpGet("reports/daily/{id}")]
    public async Task<ActionResult<ApiResponse<ReportResponseDto>>> GetReport(int id, CancellationToken cancellationToken = default)
    {
        var result = await _reportService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return Ok(ApiResponse<ReportResponseDto>.Error("日报不存在"));

        return Ok(ApiResponse<ReportResponseDto>.Success(result));
    }

    [HttpGet("reports/daily/latest")]
    public async Task<ActionResult<ApiResponse<ReportResponseDto>>> GetLatestReport(CancellationToken cancellationToken = default)
    {
        var result = await _reportService.GetLatestAsync(cancellationToken);
        if (result == null)
            return Ok(ApiResponse<ReportResponseDto>.Error("日报不存在"));
        return Ok(ApiResponse<ReportResponseDto>.Success(result));
    }

    // ==================== 任务管理 ====================

    [HttpPost("tasks/collect")]
    public async Task<ActionResult<ApiResponse<TaskTriggerResponseDto>>> RunCollectTask(CancellationToken cancellationToken = default)
    {
        var result = await _taskService.TriggerCollectAsync(cancellationToken);
        return Ok(ApiResponse<TaskTriggerResponseDto>.Success(result));
    }

    [HttpPost("tasks/analyze")]
    public async Task<ActionResult<ApiResponse<TaskTriggerResponseDto>>> RunAnalyzeTask(CancellationToken cancellationToken = default)
    {
        var result = await _taskService.TriggerAnalyzeAsync(cancellationToken);
        return Ok(ApiResponse<TaskTriggerResponseDto>.Success(result));
    }

    [HttpPost("tasks/generate-daily-report")]
    public async Task<ActionResult<ApiResponse<TaskTriggerResponseDto>>> RunGenerateReport(CancellationToken cancellationToken = default)
    {
        var result = await _taskService.TriggerGenerateReportAsync(cancellationToken);
        return Ok(ApiResponse<TaskTriggerResponseDto>.Success(result));
    }

    [HttpGet("tasks/logs")]
    public async Task<ActionResult<ApiResponse<object>>> GetTaskLogs([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (total, list) = await _taskService.GetLogsAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiResponse<object>.Success(new { Total = total, List = list }));
    }

    // ==================== 仪表盘 ====================

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetDashboard(CancellationToken cancellationToken = default)
    {
        var result = await _contentService.GetDashboardStatsAsync(cancellationToken);
        return Ok(ApiResponse<DashboardStatsDto>.Success(result));
    }
}
