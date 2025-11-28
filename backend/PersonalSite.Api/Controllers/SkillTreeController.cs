using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 技能树控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SkillTreeController : ControllerBase
{
    private readonly AppDbContext _context;

    public SkillTreeController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有技能分类
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<List<SkillCategory>>>> GetCategories()
    {
        var categories = await _context.SkillCategories
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

        return Ok(ApiResponse<List<SkillCategory>>.Success(categories));
    }

    /// <summary>
    /// 获取技能树（包含所有分类和技能）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetSkillTree()
    {
        var categories = await _context.SkillCategories
            .Include(c => c.Skills)
            .ThenInclude(s => s.Ratings.OrderByDescending(r => r.RecordedAt).Take(1))
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

        var result = categories.Select(c => new
        {
            c.Id,
            c.Name,
            c.Icon,
            c.Color,
            Skills = c.Skills.OrderBy(s => s.SortOrder).Select(s => new
            {
                s.Id,
                s.Name,
                s.Description,
                s.Icon,
                CurrentRating = s.Ratings.FirstOrDefault()?.Rating ?? 0,
                LastRatingDate = s.Ratings.FirstOrDefault()?.RecordedAt
            })
        });

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 获取技能详情（包含评级历史和学习日志）
    /// </summary>
    [HttpGet("skills/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetSkill(long id)
    {
        var skill = await _context.Skills
            .Include(s => s.Category)
            .Include(s => s.Ratings.OrderByDescending(r => r.RecordedAt))
            .Include(s => s.LearningLogs.OrderByDescending(l => l.LearnedAt))
            .FirstOrDefaultAsync(s => s.Id == id);

        if (skill == null)
        {
            return Ok(ApiResponse.Error("技能不存在", 404));
        }

        var result = new
        {
            skill.Id,
            skill.Name,
            skill.Description,
            skill.Icon,
            Category = skill.Category != null ? new
            {
                skill.Category.Id,
                skill.Category.Name,
                skill.Category.Icon,
                skill.Category.Color
            } : null,
            Ratings = skill.Ratings.Select(r => new
            {
                r.Id,
                r.Rating,
                r.Notes,
                r.RecordedAt
            }),
            LearningLogs = skill.LearningLogs.Select(l => new
            {
                l.Id,
                l.Title,
                l.Content,
                l.Duration,
                l.ResourceType,
                l.ResourceUrl,
                l.LearnedAt
            })
        };

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 创建技能分类
    /// </summary>
    [HttpPost("categories")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SkillCategory>>> CreateCategory([FromBody] SkillCategoryRequest request)
    {
        var category = new SkillCategory
        {
            Name = request.Name,
            Icon = request.Icon,
            Color = request.Color,
            SortOrder = request.SortOrder ?? 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.SkillCategories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<SkillCategory>.Success(category));
    }

    /// <summary>
    /// 创建技能
    /// </summary>
    [HttpPost("skills")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Skill>>> CreateSkill([FromBody] SkillRequest request)
    {
        var skill = new Skill
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon,
            SortOrder = request.SortOrder ?? 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<Skill>.Success(skill));
    }

    /// <summary>
    /// 记录技能评级
    /// </summary>
    [HttpPost("skills/{id}/ratings")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SkillRating>>> AddRating(long id, [FromBody] SkillRatingRequest request)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null)
        {
            return Ok(ApiResponse.Error("技能不存在", 404));
        }

        if (request.Rating < 1 || request.Rating > 10)
        {
            return Ok(ApiResponse.Error("评级必须在 1-10 之间", 400));
        }

        var rating = new SkillRating
        {
            SkillId = id,
            Rating = request.Rating,
            Notes = request.Notes,
            RecordedAt = request.RecordedAt ?? DateTime.Now,
            CreatedAt = DateTime.Now
        };

        _context.SkillRatings.Add(rating);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<SkillRating>.Success(rating));
    }

    /// <summary>
    /// 添加学习日志
    /// </summary>
    [HttpPost("skills/{id}/learning-logs")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LearningLog>>> AddLearningLog(long id, [FromBody] LearningLogRequest request)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null)
        {
            return Ok(ApiResponse.Error("技能不存在", 404));
        }

        var log = new LearningLog
        {
            SkillId = id,
            Title = request.Title,
            Content = request.Content,
            Duration = request.Duration,
            ResourceType = request.ResourceType,
            ResourceUrl = request.ResourceUrl,
            LearnedAt = request.LearnedAt ?? DateTime.Now,
            CreatedAt = DateTime.Now
        };

        _context.LearningLogs.Add(log);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<LearningLog>.Success(log));
    }

    /// <summary>
    /// 获取技能雷达图数据（用于对比不同时间点）
    /// </summary>
    [HttpGet("radar")]
    public async Task<ActionResult<ApiResponse<object>>> GetRadarData(
        [FromQuery] long? categoryId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = _context.Skills.AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(s => s.CategoryId == categoryId.Value);
        }

        var skills = await query
            .Include(s => s.Ratings)
            .ToListAsync();

        // 如果没有指定日期范围，使用最近一次评级
        if (!startDate.HasValue && !endDate.HasValue)
        {
            var result = skills.Select(s => new
            {
                s.Id,
                s.Name,
                Rating = s.Ratings.OrderByDescending(r => r.RecordedAt).FirstOrDefault()?.Rating ?? 0
            });

            return Ok(ApiResponse.Success(result));
        }

        // 如果指定了日期范围，返回该范围内的评级
        var start = startDate ?? DateTime.Now.AddMonths(-3);
        var end = endDate ?? DateTime.Now;

        var ratings = await _context.SkillRatings
            .Where(r => r.RecordedAt >= start && r.RecordedAt <= end)
            .Include(r => r.Skill)
            .GroupBy(r => r.SkillId)
            .Select(g => new
            {
                SkillId = g.Key,
                SkillName = g.First().Skill!.Name,
                AverageRating = g.Average(r => r.Rating),
                MaxRating = g.Max(r => r.Rating),
                MinRating = g.Min(r => r.Rating)
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(ratings));
    }
}

// DTOs
public class SkillCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int? SortOrder { get; set; }
}

public class SkillRequest
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int? SortOrder { get; set; }
}

public class SkillRatingRequest
{
    public decimal Rating { get; set; }
    public string? Notes { get; set; }
    public DateTime? RecordedAt { get; set; }
}

public class LearningLogRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int? Duration { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceUrl { get; set; }
    public DateTime? LearnedAt { get; set; }
}

