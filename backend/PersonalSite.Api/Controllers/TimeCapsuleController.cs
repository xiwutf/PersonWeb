using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeCapsuleController : ControllerBase
{
    private readonly AppDbContext _context;

    public TimeCapsuleController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 提交时间胶囊
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> Submit([FromBody] TimeCapsuleRequest request)
    {
        try
        {
            // 确保字段映射正确（支持小写字段名）
            var capsule = new TimeCapsule
            {
                Content = request.Content ?? string.Empty,
                VisitorId = request.VisitorId,
                VisitorName = request.VisitorName,
                Status = 0, // 0: 待审核
                CreatedAt = DateTime.Now
            };

            _context.TimeCapsules.Add(capsule);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = capsule.Id }, "时间胶囊已提交，等待审核"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"提交失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取已展示的时间胶囊列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _context.TimeCapsules.Where(t => t.Status == 1); // 1: 已展示

        var total = await query.CountAsync();
        var list = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new
            {
                t.Id,
                t.Content,
                t.VisitorName,
                t.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(new { Total = total, List = list }));
    }

    /// <summary>
    /// 获取待审核的时间胶囊（管理员）
    /// </summary>
    [HttpGet("pending")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetPending([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _context.TimeCapsules.Where(t => t.Status == 0); // 0: 待审核

        var total = await query.CountAsync();
        var list = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(ApiResponse.Success(new { Total = total, List = list }));
    }

    /// <summary>
    /// 获取所有时间胶囊（管理员，支持按状态筛选）
    /// </summary>
    [HttpGet("all")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] sbyte? status = null)
    {
        var query = _context.TimeCapsules.AsQueryable();
        
        // 如果指定了状态，则筛选
        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        var total = await query.CountAsync();
        var list = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new
            {
                t.Id,
                t.Content,
                t.VisitorId,
                t.VisitorName,
                t.Status,
                t.CreatedAt,
                t.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(new { Total = total, List = list }));
    }

    /// <summary>
    /// 获取时间胶囊统计数据（管理员）
    /// </summary>
    [HttpGet("stats")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        var pending = await _context.TimeCapsules.CountAsync(t => t.Status == 0);
        var approved = await _context.TimeCapsules.CountAsync(t => t.Status == 1);
        var rejected = await _context.TimeCapsules.CountAsync(t => t.Status == 2);

        return Ok(ApiResponse.Success(new
        {
            Pending = pending,
            Approved = approved,
            Rejected = rejected,
            Total = pending + approved + rejected
        }));
    }

    /// <summary>
    /// 审核时间胶囊（管理员）
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Approve(long id)
    {
        var capsule = await _context.TimeCapsules.FindAsync(id);
        if (capsule == null)
        {
            return Ok(ApiResponse.Error("时间胶囊不存在", 404));
        }

        capsule.Status = 1; // 1: 已展示
        capsule.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "已通过审核"));
    }

    /// <summary>
    /// 拒绝时间胶囊（管理员）
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Reject(long id)
    {
        var capsule = await _context.TimeCapsules.FindAsync(id);
        if (capsule == null)
        {
            return Ok(ApiResponse.Error("时间胶囊不存在", 404));
        }

        capsule.Status = 2; // 2: 已拒绝
        capsule.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "已拒绝"));
    }

    /// <summary>
    /// 删除时间胶囊（管理员）
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ActionName("Delete")]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        var capsule = await _context.TimeCapsules.FindAsync(id);
        if (capsule == null)
        {
            return Ok(ApiResponse.Error("时间胶囊不存在", 404));
        }

        _context.TimeCapsules.Remove(capsule);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "删除成功"));
    }
}

public class TimeCapsuleRequest
{
    public string Content { get; set; } = string.Empty;
    public string? VisitorId { get; set; }
    public string? VisitorName { get; set; }
}

