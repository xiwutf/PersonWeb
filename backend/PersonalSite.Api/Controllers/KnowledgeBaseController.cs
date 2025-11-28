using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KnowledgeBaseController : ControllerBase
{
    private readonly AppDbContext _context;

    public KnowledgeBaseController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取知识库列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetList(
        [FromQuery] string? category,
        [FromQuery] string? tag,
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.KnowledgeBases.Where(k => k.Status == 1);

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(k => k.Category == category);
        }

        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(k => k.Tags != null && k.Tags.Contains(tag));
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(k => k.Title.Contains(keyword) || (k.Content != null && k.Content.Contains(keyword)));
        }

        var total = await query.CountAsync();
        var list = await query
            .OrderByDescending(k => k.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(k => new
            {
                k.Id,
                k.Title,
                k.Category,
                k.Tags,
                k.ViewCount,
                k.CreatedAt,
                k.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(new { Total = total, List = list }));
    }

    /// <summary>
    /// 获取知识库详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetDetail(long id)
    {
        var item = await _context.KnowledgeBases.FindAsync(id);
        if (item == null)
        {
            return Ok(ApiResponse.Error("未找到", 404));
        }

        // 增加查看次数
        item.ViewCount++;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(item));
    }

    /// <summary>
    /// 创建知识库条目
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Create([FromBody] KnowledgeBaseRequest request)
    {
        try
        {
            var item = new KnowledgeBase
            {
                Title = request.Title ?? string.Empty,
                Content = request.Content,
                Category = request.Category,
                Tags = request.Tags,
                Version = 1,
                Status = 1,
                ViewCount = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.KnowledgeBases.Add(item);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = item.Id }, "创建成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"创建失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新知识库条目
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Update(long id, [FromBody] KnowledgeBaseRequest request)
    {
        try
        {
            var existing = await _context.KnowledgeBases.FindAsync(id);
            if (existing == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            // 创建版本历史
            var history = new KnowledgeBase
            {
                Title = existing.Title,
                Content = existing.Content,
                Category = existing.Category,
                Tags = existing.Tags,
                Version = existing.Version,
                ParentId = existing.Id,
                Status = 2, // 归档
                CreatedAt = existing.CreatedAt,
                UpdatedAt = existing.UpdatedAt
            };
            _context.KnowledgeBases.Add(history);

            // 更新当前版本
            existing.Title = request.Title ?? existing.Title;
            existing.Content = request.Content ?? existing.Content;
            existing.Category = request.Category ?? existing.Category;
            existing.Tags = request.Tags ?? existing.Tags;
            existing.Version++;
            existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "更新成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"更新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除知识库条目
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        var item = await _context.KnowledgeBases.FindAsync(id);
        if (item == null)
        {
            return Ok(ApiResponse.Error("未找到", 404));
        }

        _context.KnowledgeBases.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "删除成功"));
    }

    /// <summary>
    /// 获取版本历史
    /// </summary>
    [HttpGet("{id}/versions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetVersions(long id)
    {
        var versions = await _context.KnowledgeBases
            .Where(k => k.ParentId == id || k.Id == id)
            .OrderByDescending(k => k.Version)
            .Select(k => new
            {
                k.Id,
                k.Version,
                k.Title,
                k.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(versions));
    }
}

public class KnowledgeBaseRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
}

