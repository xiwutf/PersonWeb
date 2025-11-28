using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// PageBuilder SaaS 控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PageBuilderController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<PageBuilderController> _logger;

    public PageBuilderController(AppDbContext context, ILogger<PageBuilderController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 创建页面
    /// </summary>
    [HttpPost("pages")]
    public async Task<ActionResult<ApiResponse<object>>> CreatePage([FromBody] CreatePageRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(ApiResponse.Error("UserId is required", 400));
            }

            // 检查slug是否已存在
            var existingPage = await _context.UserPages
                .FirstOrDefaultAsync(p => p.UserId == request.UserId && p.Slug == request.Slug);

            if (existingPage != null)
            {
                return BadRequest(ApiResponse.Error("页面标识已存在", 400));
            }

            var page = new UserPage
            {
                UserId = request.UserId,
                Name = request.Name,
                Slug = request.Slug,
                Domain = request.Domain,
                TemplateId = request.TemplateId,
                LayoutConfig = request.LayoutConfig != null ? JsonSerializer.Serialize(request.LayoutConfig) : null,
                SeoConfig = request.SeoConfig != null ? JsonSerializer.Serialize(request.SeoConfig) : null,
                Status = "draft"
            };

            _context.UserPages.Add(page);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new
            {
                PageId = page.Id,
                Slug = page.Slug,
                Message = "页面创建成功"
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建页面失败");
            return StatusCode(500, ApiResponse.Error($"创建页面失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取用户页面列表
    /// </summary>
    [HttpPost("pages/list")]
    public async Task<ActionResult<ApiResponse<object>>> GetUserPages([FromBody] UserIdRequest request)
    {
        try
        {
            var pages = await _context.UserPages
                .Where(p => p.UserId == request.UserId)
                .OrderByDescending(p => p.UpdatedAt)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Slug,
                    p.Domain,
                    p.Status,
                    p.ViewCount,
                    p.PublishedAt,
                    p.CreatedAt,
                    p.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(pages));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取页面列表失败");
            return StatusCode(500, ApiResponse.Error($"获取页面列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取页面详情（包含组件）
    /// </summary>
    [HttpGet("pages/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetPage(long id)
    {
        try
        {
            var page = await _context.UserPages
                .Include(p => p.Components)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (page == null)
            {
                return NotFound(ApiResponse.Error("页面不存在", 404));
            }

            var components = page.Components
                .OrderBy(c => c.Position)
                .Select(c => new
                {
                    c.Id,
                    c.ComponentType,
                    c.ComponentId,
                    c.Position,
                    Config = !string.IsNullOrEmpty(c.Config) ? JsonSerializer.Deserialize<object>(c.Config) : null,
                    Style = !string.IsNullOrEmpty(c.Style) ? JsonSerializer.Deserialize<object>(c.Style) : null,
                    c.ParentId
                })
                .ToList();

            var result = new
            {
                page.Id,
                page.Name,
                page.Slug,
                page.Domain,
                page.Status,
                page.ViewCount,
                LayoutConfig = !string.IsNullOrEmpty(page.LayoutConfig) 
                    ? JsonSerializer.Deserialize<object>(page.LayoutConfig) 
                    : null,
                SeoConfig = !string.IsNullOrEmpty(page.SeoConfig) 
                    ? JsonSerializer.Deserialize<object>(page.SeoConfig) 
                    : null,
                Components = components,
                page.PublishedAt,
                page.CreatedAt,
                page.UpdatedAt
            };

            return Ok(ApiResponse.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取页面详情失败");
            return StatusCode(500, ApiResponse.Error($"获取页面详情失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 保存页面组件
    /// </summary>
    [HttpPost("pages/{pageId}/components")]
    public async Task<ActionResult<ApiResponse<object>>> SaveComponents(
        long pageId, 
        [FromBody] SaveComponentsRequest request)
    {
        try
        {
            var page = await _context.UserPages.FindAsync(pageId);
            if (page == null)
            {
                return NotFound(ApiResponse.Error("页面不存在", 404));
            }

            // 删除旧组件
            var oldComponents = await _context.PageComponents
                .Where(c => c.PageId == pageId)
                .ToListAsync();
            _context.PageComponents.RemoveRange(oldComponents);

            // 添加新组件
            foreach (var comp in request.Components)
            {
                var component = new PageComponent
                {
                    PageId = pageId,
                    ComponentType = comp.ComponentType,
                    ComponentId = comp.ComponentId,
                    Position = comp.Position,
                    Config = comp.Config != null ? JsonSerializer.Serialize(comp.Config) : null,
                    Style = comp.Style != null ? JsonSerializer.Serialize(comp.Style) : null,
                    ParentId = comp.ParentId
                };
                _context.PageComponents.Add(component);
            }

            page.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { Message = "组件保存成功" }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存组件失败");
            return StatusCode(500, ApiResponse.Error($"保存组件失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 发布页面
    /// </summary>
    [HttpPost("pages/{id}/publish")]
    public async Task<ActionResult<ApiResponse<object>>> PublishPage(long id, [FromBody] UserIdRequest request)
    {
        try
        {
            var page = await _context.UserPages
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == request.UserId);

            if (page == null)
            {
                return NotFound(ApiResponse.Error("页面不存在", 404));
            }

            page.Status = "published";
            page.PublishedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new
            {
                Message = "页面发布成功",
                PublishedAt = page.PublishedAt
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布页面失败");
            return StatusCode(500, ApiResponse.Error($"发布页面失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取组件库
    /// </summary>
    [HttpGet("components")]
    public async Task<ActionResult<ApiResponse<object>>> GetComponents(
        [FromQuery] string? category = null,
        [FromQuery] bool? isPremium = null)
    {
        try
        {
            var query = _context.ComponentLibraries.Where(c => c.IsActive);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category == category);
            }

            if (isPremium.HasValue)
            {
                query = query.Where(c => c.IsPremium == isPremium.Value);
            }

            var components = await query
                .OrderBy(c => c.Category)
                .ThenBy(c => c.Name)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Type,
                    c.Category,
                    ConfigSchema = !string.IsNullOrEmpty(c.ConfigSchema) 
                        ? JsonSerializer.Deserialize<object>(c.ConfigSchema) 
                        : null,
                    c.PreviewImage,
                    c.IsPremium,
                    c.Price,
                    c.UsageCount
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(components));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取组件库失败");
            return StatusCode(500, ApiResponse.Error($"获取组件库失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取模板库
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<ApiResponse<object>>> GetTemplates(
        [FromQuery] string? category = null)
    {
        try
        {
            var query = _context.PageTemplates.Where(t => t.IsActive);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(t => t.Category == category);
            }

            var templates = await query
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Category,
                    t.Description,
                    t.PreviewImage,
                    t.IsPremium,
                    t.Price,
                    t.UsageCount
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(templates));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取模板库失败");
            return StatusCode(500, ApiResponse.Error($"获取模板库失败: {ex.Message}", 500));
        }
    }
}

// 请求模型
public class CreatePageRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public long? TemplateId { get; set; }
    public Dictionary<string, object>? LayoutConfig { get; set; }
    public Dictionary<string, object>? SeoConfig { get; set; }
}

public class SaveComponentsRequest
{
    public List<ComponentData> Components { get; set; } = new();
}

public class ComponentData
{
    public string ComponentType { get; set; } = string.Empty;
    public string? ComponentId { get; set; }
    public int Position { get; set; }
    public Dictionary<string, object>? Config { get; set; }
    public Dictionary<string, object>? Style { get; set; }
    public long? ParentId { get; set; }
}

