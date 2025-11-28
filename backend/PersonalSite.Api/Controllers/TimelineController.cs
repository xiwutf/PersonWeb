using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimelineController : ControllerBase
{
    private readonly AppDbContext _context;

    public TimelineController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取时间线列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetList()
    {
        var events = await _context.TimelineEvents
            .Where(t => t.Status == 1)
            .OrderBy(t => t.Year)
            .ThenBy(t => t.SortOrder)
            .Select(t => new
            {
                t.Id,
                t.Year,
                t.Title,
                t.Description,
                t.Icon,
                t.Color
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(events));
    }

    /// <summary>
    /// 创建时间线事件
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Create([FromBody] TimelineEvent evt)
    {
        evt.CreatedAt = DateTime.Now;
        evt.UpdatedAt = DateTime.Now;

        _context.TimelineEvents.Add(evt);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(new { id = evt.Id }, "创建成功"));
    }

    /// <summary>
    /// 更新时间线事件
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Update(long id, [FromBody] TimelineEvent evt)
    {
        var existing = await _context.TimelineEvents.FindAsync(id);
        if (existing == null)
        {
            return Ok(ApiResponse.Error("未找到", 404));
        }

        existing.Year = evt.Year;
        existing.Title = evt.Title;
        existing.Description = evt.Description;
        existing.Icon = evt.Icon;
        existing.Color = evt.Color;
        existing.SortOrder = evt.SortOrder;
        existing.Status = evt.Status;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "更新成功"));
    }

    /// <summary>
    /// 删除时间线事件
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        var evt = await _context.TimelineEvents.FindAsync(id);
        if (evt == null)
        {
            return Ok(ApiResponse.Error("未找到", 404));
        }

        _context.TimelineEvents.Remove(evt);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "删除成功"));
    }
}

