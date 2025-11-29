using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 前端页面样式配置控制器
/// </summary>
[ApiController]
[Route("api/frontend-styles")]
public class FrontendStyleController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<FrontendStyleController> _logger;

    public FrontendStyleController(AppDbContext context, ILogger<FrontendStyleController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取页面样式配置
    /// </summary>
    [HttpGet("page/{pageKey}")]
    public async Task<ActionResult<ApiResponse<object>>> GetPageStyle(string pageKey)
    {
        try
        {
            var style = await _context.Set<FrontendPageStyle>()
                .FirstOrDefaultAsync(s => s.PageKey == pageKey && s.Enabled);

            if (style == null)
            {
                return NotFound(ApiResponse.Error("页面样式配置不存在", 404));
            }

            var styleConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(style.StyleConfig);

            return Ok(ApiResponse.Success(new
            {
                pageKey = style.PageKey,
                pageName = style.PageName,
                styleConfig,
                enabled = style.Enabled,
                version = style.Version
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取页面样式配置失败: {PageKey}", pageKey);
            return StatusCode(500, ApiResponse.Error($"获取页面样式配置失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取所有页面样式配置
    /// </summary>
    [HttpGet("pages")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetAllPageStyles()
    {
        try
        {
            var styles = await _context.Set<FrontendPageStyle>()
                .OrderBy(s => s.PageKey)
                .Select(s => new
                {
                    s.Id,
                    s.PageKey,
                    s.PageName,
                    s.Enabled,
                    s.IsDefault,
                    s.Version,
                    s.CreatedAt,
                    s.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(styles));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有页面样式配置失败");
            return StatusCode(500, ApiResponse.Error($"获取所有页面样式配置失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 创建或更新页面样式配置
    /// </summary>
    [HttpPut("page/{pageKey}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdatePageStyle(string pageKey, [FromBody] UpdatePageStyleDto dto)
    {
        try
        {
            var style = await _context.Set<FrontendPageStyle>()
                .FirstOrDefaultAsync(s => s.PageKey == pageKey);

            var styleConfigJson = JsonSerializer.Serialize(dto.StyleConfig);

            if (style == null)
            {
                style = new FrontendPageStyle
                {
                    PageKey = pageKey,
                    PageName = dto.PageName ?? pageKey,
                    StyleConfig = styleConfigJson,
                    Enabled = true,
                    Version = 1
                };
                _context.Set<FrontendPageStyle>().Add(style);
            }
            else
            {
                style.PageName = dto.PageName ?? style.PageName;
                style.StyleConfig = styleConfigJson;
                style.Version += 1;
                style.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新页面样式配置失败: {PageKey}", pageKey);
            return StatusCode(500, ApiResponse.Error($"更新页面样式配置失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取页面样式变量
    /// </summary>
    [HttpGet("variables/{pageKey}")]
    public async Task<ActionResult<ApiResponse<object>>> GetStyleVariables(string pageKey)
    {
        try
        {
            var variables = await _context.Set<FrontendStyleVariable>()
                .Where(v => v.PageKey == pageKey)
                .OrderBy(v => v.VariableKey)
                .Select(v => new
                {
                    v.Id,
                    v.PageKey,
                    v.VariableKey,
                    v.VariableValue,
                    v.VariableType,
                    v.Description
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(variables));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取样式变量失败: {PageKey}", pageKey);
            return StatusCode(500, ApiResponse.Error($"获取样式变量失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 创建或更新样式变量
    /// </summary>
    [HttpPut("variables/{pageKey}/{variableKey}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateStyleVariable(
        string pageKey,
        string variableKey,
        [FromBody] UpdateStyleVariableDto dto)
    {
        try
        {
            var variable = await _context.Set<FrontendStyleVariable>()
                .FirstOrDefaultAsync(v => v.PageKey == pageKey && v.VariableKey == variableKey);

            if (variable == null)
            {
                variable = new FrontendStyleVariable
                {
                    PageKey = pageKey,
                    VariableKey = variableKey,
                    VariableValue = dto.VariableValue,
                    VariableType = dto.VariableType ?? "color",
                    Description = dto.Description
                };
                _context.Set<FrontendStyleVariable>().Add(variable);
            }
            else
            {
                variable.VariableValue = dto.VariableValue;
                variable.VariableType = dto.VariableType ?? variable.VariableType;
                variable.Description = dto.Description;
                variable.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新样式变量失败");
            return StatusCode(500, ApiResponse.Error($"更新样式变量失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取页面样式规则
    /// </summary>
    [HttpGet("rules/{pageKey}")]
    public async Task<ActionResult<ApiResponse<object>>> GetStyleRules(string pageKey)
    {
        try
        {
            var rules = await _context.Set<FrontendStyleRule>()
                .Where(r => r.PageKey == pageKey && r.Enabled)
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.Selector)
                .Select(r => new
                {
                    r.Id,
                    r.PageKey,
                    r.Selector,
                    r.CssProperties,
                    r.Priority,
                    r.Enabled,
                    r.Description
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(rules));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取样式规则失败: {PageKey}", pageKey);
            return StatusCode(500, ApiResponse.Error($"获取样式规则失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 创建或更新样式规则
    /// </summary>
    [HttpPut("rules/{pageKey}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateStyleRule(string pageKey, [FromBody] UpdateStyleRuleDto dto)
    {
        try
        {
            var rule = await _context.Set<FrontendStyleRule>()
                .FirstOrDefaultAsync(r => r.PageKey == pageKey && r.Id == dto.Id);

            var cssPropertiesJson = JsonSerializer.Serialize(dto.CssProperties);

            if (rule == null)
            {
                rule = new FrontendStyleRule
                {
                    PageKey = pageKey,
                    Selector = dto.Selector,
                    CssProperties = cssPropertiesJson,
                    Priority = dto.Priority ?? 0,
                    Enabled = dto.Enabled ?? true,
                    Description = dto.Description
                };
                _context.Set<FrontendStyleRule>().Add(rule);
            }
            else
            {
                rule.Selector = dto.Selector;
                rule.CssProperties = cssPropertiesJson;
                rule.Priority = dto.Priority ?? rule.Priority;
                rule.Enabled = dto.Enabled ?? rule.Enabled;
                rule.Description = dto.Description;
                rule.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新样式规则失败");
            return StatusCode(500, ApiResponse.Error($"更新样式规则失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除样式规则
    /// </summary>
    [HttpDelete("rules/{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteStyleRule(long id)
    {
        try
        {
            var rule = await _context.Set<FrontendStyleRule>().FindAsync(id);
            if (rule == null)
            {
                return NotFound(ApiResponse.Error("样式规则不存在", 404));
            }

            _context.Set<FrontendStyleRule>().Remove(rule);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除样式规则失败");
            return StatusCode(500, ApiResponse.Error($"删除样式规则失败: {ex.Message}", 500));
        }
    }
}

// DTOs
public class UpdatePageStyleDto
{
    public string? PageName { get; set; }
    public Dictionary<string, object> StyleConfig { get; set; } = new();
}

public class UpdateStyleVariableDto
{
    public string VariableValue { get; set; } = string.Empty;
    public string? VariableType { get; set; }
    public string? Description { get; set; }
}

public class UpdateStyleRuleDto
{
    public long? Id { get; set; }
    public string Selector { get; set; } = string.Empty;
    public Dictionary<string, string> CssProperties { get; set; } = new();
    public int? Priority { get; set; }
    public bool? Enabled { get; set; }
    public string? Description { get; set; }
}

// 实体类（需要在 Data/AppDbContext.cs 中添加 DbSet）
public class FrontendPageStyle
{
    public long Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string PageName { get; set; } = string.Empty;
    public string StyleConfig { get; set; } = "{}";
    public bool Enabled { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public int Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class FrontendStyleVariable
{
    public long Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string VariableKey { get; set; } = string.Empty;
    public string VariableValue { get; set; } = string.Empty;
    public string VariableType { get; set; } = "color";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class FrontendStyleRule
{
    public long Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string Selector { get; set; } = string.Empty;
    public string CssProperties { get; set; } = "{}";
    public int Priority { get; set; } = 0;
    public bool Enabled { get; set; } = true;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

