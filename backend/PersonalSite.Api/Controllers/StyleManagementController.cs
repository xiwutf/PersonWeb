using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StyleManagementController : ControllerBase
{
    private readonly AppDbContext _context;

    public StyleManagementController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有样式分类
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<List<StyleCategory>>>> GetCategories()
    {
        var categories = await _context.StyleCategories
            .OrderBy(c => c.Sort)
            .ToListAsync();
        return Ok(ApiResponse<List<StyleCategory>>.Success(categories));
    }

    /// <summary>
    /// 获取指定分类下的所有样式
    /// </summary>
    [HttpGet("category/{categoryId}/styles")]
    public async Task<ActionResult<ApiResponse<List<StyleDefinition>>>> GetStylesByCategory(long categoryId)
    {
        var styles = await _context.StyleDefinitions
            .Where(s => s.CategoryId == categoryId && s.IsActive)
            .OrderBy(s => s.Sort)
            .ToListAsync();
        return Ok(ApiResponse<List<StyleDefinition>>.Success(styles));
    }

    /// <summary>
    /// 获取所有样式（按分类分组）
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, List<StyleDefinition>>>>> GetAllStyles()
    {
        var categories = await _context.StyleCategories
            .Include(c => c.StyleDefinitions.Where(s => s.IsActive))
            .OrderBy(c => c.Sort)
            .ToListAsync();

        var result = categories.ToDictionary(
            c => c.Code,
            c => c.StyleDefinitions.OrderBy(s => s.Sort).ToList()
        );

        return Ok(ApiResponse<Dictionary<string, List<StyleDefinition>>>.Success(result));
    }

    /// <summary>
    /// 根据代码获取样式
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<ApiResponse<StyleDefinition>>> GetStyleByCode(string code)
    {
        var style = await _context.StyleDefinitions
            .FirstOrDefaultAsync(s => s.Code == code && s.IsActive);

        if (style == null)
        {
            return NotFound(ApiResponse<StyleDefinition>.Error("样式不存在", 404));
        }

        return Ok(ApiResponse<StyleDefinition>.Success(style));
    }

    /// <summary>
    /// 创建或更新样式
    /// </summary>
    [HttpPost("style")]
    public async Task<ActionResult<ApiResponse<StyleDefinition>>> CreateOrUpdateStyle([FromBody] StyleDefinitionDto dto)
    {
        StyleDefinition style;

        if (dto.Id > 0)
        {
            var foundStyle = await _context.StyleDefinitions.FindAsync(dto.Id);
            if (foundStyle == null)
            {
                return NotFound(ApiResponse<StyleDefinition>.Error("样式不存在", 404));
            }
            style = foundStyle;

            // 检查代码是否被其他记录使用
            if (style.Code != dto.Code)
            {
                var exists = await _context.StyleDefinitions
                    .AnyAsync(s => s.Code == dto.Code && s.Id != dto.Id);
                if (exists)
                {
                    return BadRequest(ApiResponse<StyleDefinition>.Error("样式代码已存在", 400));
                }
            }
        }
        else
        {
            var exists = await _context.StyleDefinitions
                .AnyAsync(s => s.Code == dto.Code);
            if (exists)
            {
                return BadRequest(ApiResponse<StyleDefinition>.Error("样式代码已存在", 400));
            }

            style = new StyleDefinition
            {
                CreatedAt = DateTime.Now
            };
            _context.StyleDefinitions.Add(style);
        }

        // 更新属性
        style.CategoryId = dto.CategoryId;
        style.Name = dto.Name;
        style.Code = dto.Code;
        style.CssClass = dto.CssClass;
        style.BackgroundColor = dto.BackgroundColor;
        style.BorderColor = dto.BorderColor;
        style.TextColor = dto.TextColor;
        style.FontSize = dto.FontSize;
        style.FontWeight = dto.FontWeight;
        style.Padding = dto.Padding;
        style.BorderRadius = dto.BorderRadius;
        style.BorderWidth = dto.BorderWidth;
        style.CustomCss = dto.CustomCss;
        style.Description = dto.Description;
        style.IsActive = dto.IsActive;
        style.Sort = dto.Sort;
        style.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<StyleDefinition>.Success(style));
    }

    /// <summary>
    /// 删除样式
    /// </summary>
    [HttpDelete("style/{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteStyle(long id)
    {
        var style = await _context.StyleDefinitions.FindAsync(id);
        if (style == null)
        {
            return NotFound(ApiResponse.Error("样式不存在", 404));
        }

        _context.StyleDefinitions.Remove(style);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 记录样式使用
    /// </summary>
    [HttpPost("usage")]
    public async Task<ActionResult<ApiResponse>> RecordUsage([FromBody] StyleUsageDto dto)
    {
        var usage = await _context.StyleUsages
            .FirstOrDefaultAsync(u => u.StyleId == dto.StyleId 
                && u.PagePath == dto.PagePath 
                && u.ComponentName == dto.ComponentName);

        if (usage == null)
        {
            usage = new StyleUsage
            {
                StyleId = dto.StyleId,
                PagePath = dto.PagePath,
                ComponentName = dto.ComponentName,
                UsageCount = 1,
                LastUsedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };
            _context.StyleUsages.Add(usage);
        }
        else
        {
            usage.UsageCount++;
            usage.LastUsedAt = DateTime.Now;
            usage.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 获取样式使用统计
    /// </summary>
    [HttpGet("usage/stats")]
    public async Task<ActionResult<ApiResponse<List<StyleUsageStatsResponse>>>> GetUsageStats()
    {
        var stats = await _context.StyleUsages
            .Include(u => u.Style)
            .ThenInclude(s => s!.Category)
            .GroupBy(u => u.StyleId)
            .Select(g => new StyleUsageStatsResponse
            {
                StyleId = g.Key,
                StyleCode = g.First().Style!.Code,
                StyleName = g.First().Style!.Name,
                CategoryName = g.First().Style!.Category!.Name,
                TotalUsage = g.Sum(u => u.UsageCount),
                PageCount = g.Select(u => u.PagePath).Distinct().Count(),
                ComponentCount = g.Where(u => u.ComponentName != null).Select(u => u.ComponentName).Distinct().Count(),
                Pages = g.Select(u => u.PagePath).Distinct().ToList(),
                Components = g.Where(u => u.ComponentName != null).Select(u => u.ComponentName!).Distinct().ToList(),
                LastUsedAt = g.Max(u => u.LastUsedAt)
            })
            .OrderByDescending(s => s.TotalUsage)
            .ToListAsync();

        return Ok(ApiResponse<List<StyleUsageStatsResponse>>.Success(stats));
    }

    /// <summary>
    /// 获取指定样式的使用详情
    /// </summary>
    [HttpGet("style/{id}/usage")]
    public async Task<ActionResult<ApiResponse<List<StyleUsage>>>> GetStyleUsage(long id)
    {
        var usages = await _context.StyleUsages
            .Where(u => u.StyleId == id)
            .OrderByDescending(u => u.UsageCount)
            .ToListAsync();

        return Ok(ApiResponse<List<StyleUsage>>.Success(usages));
    }

    /// <summary>
    /// 初始化默认样式（为所有分类创建默认样式）
    /// </summary>
    [HttpPost("init-defaults")]
    public async Task<ActionResult<ApiResponse<int>>> InitDefaultStyles()
    {
        var count = 0;

        // 获取所有分类
        var categories = await _context.StyleCategories.ToListAsync();

        foreach (var category in categories)
        {
            // 检查该分类是否已有样式
            var hasStyles = await _context.StyleDefinitions
                .AnyAsync(s => s.CategoryId == category.Id);
            
            if (hasStyles) continue; // 如果已有样式，跳过

            // 根据分类代码创建默认样式
            var defaultStyles = GetDefaultStylesForCategory(category.Code, category.Id);
            
            foreach (var style in defaultStyles)
            {
                // 检查样式代码是否已存在
                var exists = await _context.StyleDefinitions
                    .AnyAsync(s => s.Code == style.Code);
                
                if (!exists)
                {
                    _context.StyleDefinitions.Add(style);
                    count++;
                }
            }
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<int>.Success(count));
    }

    /// <summary>
    /// 根据分类代码获取默认样式列表
    /// </summary>
    private List<StyleDefinition> GetDefaultStylesForCategory(string categoryCode, long categoryId)
    {
        var styles = new List<StyleDefinition>();
        var now = DateTime.Now;

        switch (categoryCode)
        {
            case "button":
                styles.AddRange(new[]
                {
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "主要按钮",
                        Code = "button-primary",
                        CssClass = "btn btn-primary",
                        BackgroundColor = "rgba(59, 130, 246, 1)",
                        BorderColor = "rgba(59, 130, 246, 1)",
                        TextColor = "#ffffff",
                        FontSize = "0.875rem",
                        FontWeight = "500",
                        Padding = "0.5rem 1rem",
                        BorderRadius = "0.375rem",
                        BorderWidth = "1px",
                        Description = "主要操作按钮样式",
                        Sort = 1,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "次要按钮",
                        Code = "button-secondary",
                        CssClass = "btn btn-secondary",
                        BackgroundColor = "rgba(255, 255, 255, 0.1)",
                        BorderColor = "rgba(255, 255, 255, 0.2)",
                        TextColor = "rgba(255, 255, 255, 0.9)",
                        FontSize = "0.875rem",
                        FontWeight = "500",
                        Padding = "0.5rem 1rem",
                        BorderRadius = "0.375rem",
                        BorderWidth = "1px",
                        Description = "次要操作按钮样式",
                        Sort = 2,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "成功按钮",
                        Code = "button-success",
                        CssClass = "btn btn-success",
                        BackgroundColor = "rgba(34, 197, 94, 1)",
                        BorderColor = "rgba(34, 197, 94, 1)",
                        TextColor = "#ffffff",
                        FontSize = "0.875rem",
                        FontWeight = "500",
                        Padding = "0.5rem 1rem",
                        BorderRadius = "0.375rem",
                        BorderWidth = "1px",
                        Description = "成功操作按钮样式",
                        Sort = 3,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "危险按钮",
                        Code = "button-danger",
                        CssClass = "btn btn-danger",
                        BackgroundColor = "rgba(239, 68, 68, 1)",
                        BorderColor = "rgba(239, 68, 68, 1)",
                        TextColor = "#ffffff",
                        FontSize = "0.875rem",
                        FontWeight = "500",
                        Padding = "0.5rem 1rem",
                        BorderRadius = "0.375rem",
                        BorderWidth = "1px",
                        Description = "危险操作按钮样式",
                        Sort = 4,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                });
                break;

            case "table":
                styles.AddRange(new[]
                {
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "表格表头",
                        Code = "table-header",
                        CssClass = "table-header",
                        BackgroundColor = "rgba(255, 255, 255, 0.05)",
                        BorderColor = "rgba(255, 255, 255, 0.1)",
                        TextColor = "rgba(255, 255, 255, 0.9)",
                        FontSize = "0.875rem",
                        FontWeight = "600",
                        Padding = "0.75rem 1rem",
                        BorderRadius = "0",
                        BorderWidth = "1px",
                        Description = "表格表头样式",
                        Sort = 1,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "表格单元格",
                        Code = "table-cell",
                        CssClass = "table-cell",
                        BackgroundColor = "transparent",
                        BorderColor = "rgba(255, 255, 255, 0.1)",
                        TextColor = "rgba(255, 255, 255, 0.8)",
                        FontSize = "0.875rem",
                        FontWeight = "400",
                        Padding = "0.75rem 1rem",
                        BorderRadius = "0",
                        BorderWidth = "1px",
                        Description = "表格单元格样式",
                        Sort = 2,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "表格行悬停",
                        Code = "table-row-hover",
                        CssClass = "table-row-hover",
                        BackgroundColor = "rgba(255, 255, 255, 0.05)",
                        BorderColor = "rgba(255, 255, 255, 0.1)",
                        TextColor = "rgba(255, 255, 255, 0.9)",
                        FontSize = "0.875rem",
                        FontWeight = "400",
                        Padding = "0.75rem 1rem",
                        BorderRadius = "0",
                        BorderWidth = "1px",
                        Description = "表格行悬停样式",
                        Sort = 3,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                });
                break;

            case "card":
                styles.AddRange(new[]
                {
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "默认卡片",
                        Code = "card-default",
                        CssClass = "card",
                        BackgroundColor = "rgba(255, 255, 255, 0.05)",
                        BorderColor = "rgba(255, 255, 255, 0.1)",
                        TextColor = "rgba(255, 255, 255, 0.9)",
                        FontSize = "1rem",
                        FontWeight = "400",
                        Padding = "1.5rem",
                        BorderRadius = "0.5rem",
                        BorderWidth = "1px",
                        Description = "默认卡片容器样式",
                        Sort = 1,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "玻璃拟态卡片",
                        Code = "card-glass",
                        CssClass = "card card-glass",
                        BackgroundColor = "rgba(255, 255, 255, 0.1)",
                        BorderColor = "rgba(255, 255, 255, 0.2)",
                        TextColor = "rgba(255, 255, 255, 0.9)",
                        FontSize = "1rem",
                        FontWeight = "400",
                        Padding = "1.5rem",
                        BorderRadius = "1rem",
                        BorderWidth = "1px",
                        Description = "玻璃拟态效果卡片样式",
                        Sort = 2,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "悬停卡片",
                        Code = "card-hover",
                        CssClass = "card card-hover",
                        BackgroundColor = "rgba(255, 255, 255, 0.05)",
                        BorderColor = "rgba(255, 255, 255, 0.1)",
                        TextColor = "rgba(255, 255, 255, 0.9)",
                        FontSize = "1rem",
                        FontWeight = "400",
                        Padding = "1.5rem",
                        BorderRadius = "0.5rem",
                        BorderWidth = "1px",
                        Description = "带悬停效果的卡片样式",
                        Sort = 3,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                });
                break;

            case "input":
                styles.AddRange(new[]
                {
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "默认输入框",
                        Code = "input-default",
                        CssClass = "input input-default",
                        BackgroundColor = "rgba(255, 255, 255, 0.1)",
                        BorderColor = "rgba(255, 255, 255, 0.2)",
                        TextColor = "rgba(255, 255, 255, 0.9)",
                        FontSize = "0.875rem",
                        FontWeight = "400",
                        Padding = "0.5rem 0.75rem",
                        BorderRadius = "0.375rem",
                        BorderWidth = "1px",
                        Description = "默认输入框样式",
                        Sort = 1,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "聚焦输入框",
                        Code = "input-focus",
                        CssClass = "input input-focus",
                        BackgroundColor = "rgba(255, 255, 255, 0.15)",
                        BorderColor = "rgba(59, 130, 246, 0.6)",
                        TextColor = "rgba(255, 255, 255, 0.9)",
                        FontSize = "0.875rem",
                        FontWeight = "400",
                        Padding = "0.5rem 0.75rem",
                        BorderRadius = "0.375rem",
                        BorderWidth = "2px",
                        Description = "聚焦状态的输入框样式",
                        Sort = 2,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                });
                break;

            case "pagination":
                styles.AddRange(new[]
                {
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "分页按钮",
                        Code = "pagination-button",
                        CssClass = "pagination-button",
                        BackgroundColor = "rgba(255, 255, 255, 0.1)",
                        BorderColor = "rgba(255, 255, 255, 0.2)",
                        TextColor = "rgba(255, 255, 255, 0.9)",
                        FontSize = "0.875rem",
                        FontWeight = "500",
                        Padding = "0.5rem 0.75rem",
                        BorderRadius = "0.375rem",
                        BorderWidth = "1px",
                        Description = "分页按钮样式",
                        Sort = 1,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "分页激活",
                        Code = "pagination-active",
                        CssClass = "pagination-active",
                        BackgroundColor = "rgba(59, 130, 246, 1)",
                        BorderColor = "rgba(59, 130, 246, 1)",
                        TextColor = "#ffffff",
                        FontSize = "0.875rem",
                        FontWeight = "600",
                        Padding = "0.5rem 0.75rem",
                        BorderRadius = "0.375rem",
                        BorderWidth = "1px",
                        Description = "分页激活状态样式",
                        Sort = 2,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                });
                break;

            case "font":
                styles.AddRange(new[]
                {
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "标题字体",
                        Code = "font-heading",
                        CssClass = "font-heading",
                        BackgroundColor = null,
                        BorderColor = null,
                        TextColor = "rgba(255, 255, 255, 0.95)",
                        FontSize = "1.5rem",
                        FontWeight = "700",
                        Padding = null,
                        BorderRadius = null,
                        BorderWidth = null,
                        Description = "标题字体样式",
                        Sort = 1,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "正文字体",
                        Code = "font-body",
                        CssClass = "font-body",
                        BackgroundColor = null,
                        BorderColor = null,
                        TextColor = "rgba(255, 255, 255, 0.8)",
                        FontSize = "1rem",
                        FontWeight = "400",
                        Padding = null,
                        BorderRadius = null,
                        BorderWidth = null,
                        Description = "正文字体样式",
                        Sort = 2,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new StyleDefinition
                    {
                        CategoryId = categoryId,
                        Name = "小号字体",
                        Code = "font-small",
                        CssClass = "font-small",
                        BackgroundColor = null,
                        BorderColor = null,
                        TextColor = "rgba(255, 255, 255, 0.6)",
                        FontSize = "0.875rem",
                        FontWeight = "400",
                        Padding = null,
                        BorderRadius = null,
                        BorderWidth = null,
                        Description = "小号字体样式",
                        Sort = 3,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                });
                break;
        }

        return styles;
    }
}

// DTOs
public class StyleDefinitionDto
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
    public string? BackgroundColor { get; set; }
    public string? BorderColor { get; set; }
    public string? TextColor { get; set; }
    public string? FontSize { get; set; }
    public string? FontWeight { get; set; }
    public string? Padding { get; set; }
    public string? BorderRadius { get; set; }
    public string? BorderWidth { get; set; }
    public string? CustomCss { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int Sort { get; set; }
}

public class StyleUsageDto
{
    public long StyleId { get; set; }
    public string PagePath { get; set; } = string.Empty;
    public string? ComponentName { get; set; }
}

public class StyleUsageStatsResponse
{
    public long StyleId { get; set; }
    public string StyleCode { get; set; } = string.Empty;
    public string StyleName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int TotalUsage { get; set; }
    public int PageCount { get; set; }
    public int ComponentCount { get; set; }
    public List<string> Pages { get; set; } = new();
    public List<string> Components { get; set; } = new();
    public DateTime? LastUsedAt { get; set; }
}

