using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 模拟数据控制器
/// 提供前端页面需要的模拟数据
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MockDataController : ControllerBase
{
    private readonly AppDbContext _context;

    public MockDataController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取技能树数据
    /// </summary>
    [HttpGet("skill-tree")]
    public async Task<ActionResult<ApiResponse<object>>> GetSkillTree()
    {
        var categories = await _context.SkillCategories
            .Include(c => c.Skills)
            .OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Icon,
                c.Color,
                Skills = c.Skills.OrderBy(s => s.SortOrder).Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Icon,
                    s.Description,
                    CurrentRating = (double)s.CurrentRating,
                    TargetRating = s.TargetRating.HasValue ? (double?)s.TargetRating.Value : null
                }).ToList()
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(categories));
    }

    /// <summary>
    /// 获取技能分类列表
    /// </summary>
    [HttpGet("skill-categories")]
    public async Task<ActionResult<ApiResponse<object>>> GetSkillCategories()
    {
        var categories = await _context.SkillCategories
            .OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Icon,
                c.Color
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(categories));
    }

    /// <summary>
    /// 获取仪表盘指标数据
    /// </summary>
    /// <param name="days">获取最近N天的数据，默认7天</param>
    [HttpGet("dashboard-metrics")]
    public async Task<ActionResult<ApiResponse<object>>> GetDashboardMetrics([FromQuery] int days = 7)
    {
        var startDate = DateTime.Today.AddDays(-days + 1);
        
        var metrics = await _context.DashboardMetrics
            .Where(m => m.Date >= startDate)
            .OrderBy(m => m.Date)
            .Select(m => new
            {
                Date = m.Date.ToString("yyyy-MM-dd"),
                Steps = m.Steps,
                Sleep = (double)m.Sleep,
                Weight = m.Weight.HasValue ? (double?)m.Weight.Value : null,
                NetWorth = m.NetWorth.HasValue ? (double?)m.NetWorth.Value : null,
                Energy = m.Energy
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(metrics));
    }

    /// <summary>
    /// 获取工具列表（用于工具页面）
    /// </summary>
    [HttpGet("tools")]
    public async Task<ActionResult<ApiResponse<object>>> GetTools()
    {
        var toolsData = await _context.Tools
            .Where(t => t.Status == "published")
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                _path = $"/tools/{t.Slug}",
                title = t.Name,
                slug = t.Slug,
                description = t.Description ?? "",
                price = (double)t.Price,
                tagsJson = t.Tags,
                buy_link = "#",
                date = t.CreatedAt
            })
            .ToListAsync();

        var tools = toolsData.Select(t => new
        {
            t._path,
            t.title,
            t.slug,
            t.description,
            t.price,
            tags = !string.IsNullOrEmpty(t.tagsJson) 
                ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(t.tagsJson) ?? new List<string>()
                : new List<string>(),
            t.buy_link,
            t.date
        }).ToList();

        return Ok(ApiResponse.Success(tools));
    }

    /// <summary>
    /// 获取生活随笔列表
    /// </summary>
    [HttpGet("life-posts")]
    public async Task<ActionResult<ApiResponse<object>>> GetLifePosts()
    {
        // 获取"生活随笔"分类ID
        var lifeCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == "life" || c.Name == "生活随笔");

        if (lifeCategory == null)
        {
            return Ok(ApiResponse.Success(new List<object>()));
        }

        var posts = await _context.Articles
            .Where(a => a.CategoryId == lifeCategory.Id && a.Status == 1)
            .OrderByDescending(a => a.PublishTime ?? a.CreatedAt)
            .Select(a => new
            {
                _path = $"/life/{a.Slug ?? a.Id.ToString()}",
                title = a.Title,
                slug = a.Slug ?? a.Id.ToString(),
                description = a.Summary ?? "",
                date = a.PublishTime ?? a.CreatedAt,
                category = lifeCategory.Name,
                tags = new List<string>(), // 可以从其他字段解析
                cover = (string?)null
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(posts));
    }
}

