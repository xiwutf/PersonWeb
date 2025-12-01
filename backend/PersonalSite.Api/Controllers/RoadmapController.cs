using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoadmapController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoadmapController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有未来规划（前台公开接口）
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<RoadmapDto>>>> GetAll([FromQuery] string? timeline = null, [FromQuery] string? status = null)
    {
        try
        {
            var query = _context.Roadmaps.AsQueryable();

            if (!string.IsNullOrEmpty(timeline))
            {
                query = query.Where(r => r.Timeline == timeline);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            var roadmaps = await query
                .OrderBy(r => r.Timeline)
                .ThenByDescending(r => r.Priority)
                .ThenBy(r => r.CreatedAt)
                .ToListAsync();

            var dtos = roadmaps.Select(r => new RoadmapDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Items = ParseJsonArray(r.Items),
                Timeline = r.Timeline,
                Priority = r.Priority,
                Status = r.Status,
                StartDate = r.StartDate?.ToString("yyyy-MM-dd"),
                TargetDate = r.TargetDate?.ToString("yyyy-MM-dd"),
                CompletedDate = r.CompletedDate?.ToString("yyyy-MM-dd"),
                Progress = r.Progress
            }).ToList();

            return Ok(ApiResponse<List<RoadmapDto>>.Success(dtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<RoadmapDto>>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取单条未来规划
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RoadmapDto>>> GetById(long id)
    {
        try
        {
            var roadmap = await _context.Roadmaps.FindAsync(id);
            if (roadmap == null)
            {
                return NotFound(ApiResponse<RoadmapDto>.Error("未来规划不存在", 404));
            }

            var dto = new RoadmapDto
            {
                Id = roadmap.Id,
                Title = roadmap.Title,
                Description = roadmap.Description,
                Items = ParseJsonArray(roadmap.Items),
                Timeline = roadmap.Timeline,
                Priority = roadmap.Priority,
                Status = roadmap.Status,
                StartDate = roadmap.StartDate?.ToString("yyyy-MM-dd"),
                TargetDate = roadmap.TargetDate?.ToString("yyyy-MM-dd"),
                CompletedDate = roadmap.CompletedDate?.ToString("yyyy-MM-dd"),
                Progress = roadmap.Progress
            };

            return Ok(ApiResponse<RoadmapDto>.Success(dto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<RoadmapDto>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 创建未来规划（管理员）
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<RoadmapDto>>> Create([FromBody] CreateRoadmapDto dto)
    {
        try
        {
            var roadmap = new Roadmap
            {
                Title = dto.Title,
                Description = dto.Description,
                Items = dto.Items != null ? System.Text.Json.JsonSerializer.Serialize(dto.Items) : null,
                Timeline = dto.Timeline ?? "short",
                Priority = dto.Priority ?? 0,
                Status = dto.Status ?? "planned",
                StartDate = dto.StartDate != null ? DateTime.Parse(dto.StartDate) : null,
                TargetDate = dto.TargetDate != null ? DateTime.Parse(dto.TargetDate) : null,
                Progress = dto.Progress ?? 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Roadmaps.Add(roadmap);
            await _context.SaveChangesAsync();

            var result = new RoadmapDto
            {
                Id = roadmap.Id,
                Title = roadmap.Title,
                Description = roadmap.Description,
                Items = ParseJsonArray(roadmap.Items),
                Timeline = roadmap.Timeline,
                Priority = roadmap.Priority,
                Status = roadmap.Status,
                StartDate = roadmap.StartDate?.ToString("yyyy-MM-dd"),
                TargetDate = roadmap.TargetDate?.ToString("yyyy-MM-dd"),
                Progress = roadmap.Progress
            };

            return Ok(ApiResponse<RoadmapDto>.Success(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<RoadmapDto>.Error($"创建失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新未来规划（管理员）
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<RoadmapDto>>> Update(long id, [FromBody] UpdateRoadmapDto dto)
    {
        try
        {
            var roadmap = await _context.Roadmaps.FindAsync(id);
            if (roadmap == null)
            {
                return NotFound(ApiResponse<RoadmapDto>.Error("未来规划不存在", 404));
            }

            if (dto.Title != null) roadmap.Title = dto.Title;
            if (dto.Description != null) roadmap.Description = dto.Description;
            if (dto.Items != null) roadmap.Items = System.Text.Json.JsonSerializer.Serialize(dto.Items);
            if (dto.Timeline != null) roadmap.Timeline = dto.Timeline;
            if (dto.Priority.HasValue) roadmap.Priority = dto.Priority.Value;
            if (dto.Status != null) roadmap.Status = dto.Status;
            if (dto.StartDate != null) roadmap.StartDate = DateTime.Parse(dto.StartDate);
            if (dto.TargetDate != null) roadmap.TargetDate = DateTime.Parse(dto.TargetDate);
            if (dto.Progress.HasValue) roadmap.Progress = dto.Progress.Value;
            if (dto.Status == "completed" && roadmap.CompletedDate == null)
            {
                roadmap.CompletedDate = DateTime.Now;
            }
            roadmap.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var result = new RoadmapDto
            {
                Id = roadmap.Id,
                Title = roadmap.Title,
                Description = roadmap.Description,
                Items = ParseJsonArray(roadmap.Items),
                Timeline = roadmap.Timeline,
                Priority = roadmap.Priority,
                Status = roadmap.Status,
                StartDate = roadmap.StartDate?.ToString("yyyy-MM-dd"),
                TargetDate = roadmap.TargetDate?.ToString("yyyy-MM-dd"),
                CompletedDate = roadmap.CompletedDate?.ToString("yyyy-MM-dd"),
                Progress = roadmap.Progress
            };

            return Ok(ApiResponse<RoadmapDto>.Success(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<RoadmapDto>.Error($"更新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除未来规划（管理员）
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        try
        {
            var roadmap = await _context.Roadmaps.FindAsync(id);
            if (roadmap == null)
            {
                return NotFound(ApiResponse.Error("未来规划不存在", 404));
            }

            _context.Roadmaps.Remove(roadmap);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"删除失败: {ex.Message}", 500));
        }
    }

    private List<string> ParseJsonArray(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return new List<string>();

        try
        {
            var items = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);
            return items ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}

public class RoadmapDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Items { get; set; } = new();
    public string Timeline { get; set; } = "short";
    public int Priority { get; set; }
    public string Status { get; set; } = "planned";
    public string? StartDate { get; set; }
    public string? TargetDate { get; set; }
    public string? CompletedDate { get; set; }
    public int Progress { get; set; }
}

public class CreateRoadmapDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Items { get; set; }
    public string? Timeline { get; set; }
    public int? Priority { get; set; }
    public string? Status { get; set; }
    public string? StartDate { get; set; }
    public string? TargetDate { get; set; }
    public int? Progress { get; set; }
}

public class UpdateRoadmapDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<string>? Items { get; set; }
    public string? Timeline { get; set; }
    public int? Priority { get; set; }
    public string? Status { get; set; }
    public string? StartDate { get; set; }
    public string? TargetDate { get; set; }
    public int? Progress { get; set; }
}

