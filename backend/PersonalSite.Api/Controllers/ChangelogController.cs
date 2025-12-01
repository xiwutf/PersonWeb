using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChangelogController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChangelogController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有更新日志（前台公开接口）
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<ChangelogDto>>>> GetAll([FromQuery] string? category = null)
    {
        try
        {
            var query = _context.Changelogs.Where(c => c.Status == 1);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category == category);
            }

            var changelogs = await query
                .OrderByDescending(c => c.Date)
                .ThenByDescending(c => c.CreatedAt)
                .ToListAsync();

            var dtos = changelogs.Select(c => new ChangelogDto
            {
                Id = c.Id,
                Version = c.Version,
                Date = c.Date.ToString("yyyy-MM-dd"),
                Title = c.Title,
                Description = c.Description,
                Items = ParseJsonArray(c.Items),
                Category = c.Category
            }).ToList();

            return Ok(ApiResponse<List<ChangelogDto>>.Success(dtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<ChangelogDto>>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取单条更新日志
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ChangelogDto>>> GetById(long id)
    {
        try
        {
            var changelog = await _context.Changelogs.FindAsync(id);
            if (changelog == null || changelog.Status == 0)
            {
                return NotFound(ApiResponse<ChangelogDto>.Error("更新日志不存在", 404));
            }

            var dto = new ChangelogDto
            {
                Id = changelog.Id,
                Version = changelog.Version,
                Date = changelog.Date.ToString("yyyy-MM-dd"),
                Title = changelog.Title,
                Description = changelog.Description,
                Items = ParseJsonArray(changelog.Items),
                Category = changelog.Category
            };

            return Ok(ApiResponse<ChangelogDto>.Success(dto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ChangelogDto>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 创建更新日志（管理员）
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ChangelogDto>>> Create([FromBody] CreateChangelogDto dto)
    {
        try
        {
            var changelog = new Changelog
            {
                Version = dto.Version,
                Date = DateTime.Parse(dto.Date),
                Title = dto.Title,
                Description = dto.Description,
                Items = dto.Items != null ? System.Text.Json.JsonSerializer.Serialize(dto.Items) : null,
                Category = dto.Category ?? "feature",
                Status = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Changelogs.Add(changelog);
            await _context.SaveChangesAsync();

            var result = new ChangelogDto
            {
                Id = changelog.Id,
                Version = changelog.Version,
                Date = changelog.Date.ToString("yyyy-MM-dd"),
                Title = changelog.Title,
                Description = changelog.Description,
                Items = ParseJsonArray(changelog.Items),
                Category = changelog.Category
            };

            return Ok(ApiResponse<ChangelogDto>.Success(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ChangelogDto>.Error($"创建失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新更新日志（管理员）
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ChangelogDto>>> Update(long id, [FromBody] UpdateChangelogDto dto)
    {
        try
        {
            var changelog = await _context.Changelogs.FindAsync(id);
            if (changelog == null)
            {
                return NotFound(ApiResponse<ChangelogDto>.Error("更新日志不存在", 404));
            }

            if (dto.Version != null) changelog.Version = dto.Version;
            if (dto.Date != null) changelog.Date = DateTime.Parse(dto.Date);
            if (dto.Title != null) changelog.Title = dto.Title;
            if (dto.Description != null) changelog.Description = dto.Description;
            if (dto.Items != null) changelog.Items = System.Text.Json.JsonSerializer.Serialize(dto.Items);
            if (dto.Category != null) changelog.Category = dto.Category;
            if (dto.Status.HasValue) changelog.Status = dto.Status.Value;
            changelog.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var result = new ChangelogDto
            {
                Id = changelog.Id,
                Version = changelog.Version,
                Date = changelog.Date.ToString("yyyy-MM-dd"),
                Title = changelog.Title,
                Description = changelog.Description,
                Items = ParseJsonArray(changelog.Items),
                Category = changelog.Category
            };

            return Ok(ApiResponse<ChangelogDto>.Success(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ChangelogDto>.Error($"更新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除更新日志（管理员）
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        try
        {
            var changelog = await _context.Changelogs.FindAsync(id);
            if (changelog == null)
            {
                return NotFound(ApiResponse.Error("更新日志不存在", 404));
            }

            _context.Changelogs.Remove(changelog);
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

public class ChangelogDto
{
    public long Id { get; set; }
    public string? Version { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Items { get; set; } = new();
    public string Category { get; set; } = "feature";
}

public class CreateChangelogDto
{
    public string? Version { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Items { get; set; }
    public string? Category { get; set; }
}

public class UpdateChangelogDto
{
    public string? Version { get; set; }
    public string? Date { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<string>? Items { get; set; }
    public string? Category { get; set; }
    public sbyte? Status { get; set; }
}

