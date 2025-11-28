using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Utils;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminStyleController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminStyleController> _logger;

    public AdminStyleController(AppDbContext context, ILogger<AdminStyleController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有全局风格
    /// </summary>
    [HttpGet("global")]
    public async Task<ActionResult<ApiResponse<List<AdminGlobalStyle>>>> GetGlobalStyles()
    {
        var styles = await _context.AdminGlobalStyles
            .Where(s => s.Enabled)
            .OrderByDescending(s => s.IsDefault)
            .ThenByDescending(s => s.CreatedAt)
            .ToListAsync();

        return Ok(ApiResponse<List<AdminGlobalStyle>>.Success(styles));
    }

    /// <summary>
    /// 获取当前默认全局风格
    /// </summary>
    [HttpGet("global/current")]
    public async Task<ActionResult<ApiResponse<AdminGlobalStyle>>> GetCurrentGlobalStyle()
    {
        var style = await _context.AdminGlobalStyles
            .FirstOrDefaultAsync(s => s.IsDefault && s.Enabled);

        if (style == null)
        {
            // 返回默认风格
            style = await _context.AdminGlobalStyles
                .FirstOrDefaultAsync(s => s.StyleKey == "dark-tech");
        }

        if (style == null)
        {
            return NotFound(ApiResponse<AdminGlobalStyle>.Error("未找到默认风格", 404));
        }

        return Ok(ApiResponse<AdminGlobalStyle>.Success(style));
    }

    /// <summary>
    /// 设置默认全局风格
    /// </summary>
    [HttpPost("global/{id}/set-default")]
    public async Task<ActionResult<ApiResponse>> SetDefaultGlobalStyle(long id)
    {
        var style = await _context.AdminGlobalStyles.FindAsync(id);
        if (style == null || !style.Enabled)
        {
            return NotFound(ApiResponse.Error("风格不存在或已禁用", 404));
        }

        // 清除其他默认标志
        var defaultStyles = await _context.AdminGlobalStyles
            .Where(s => s.IsDefault)
            .ToListAsync();
        foreach (var s in defaultStyles)
        {
            s.IsDefault = false;
        }

        style.IsDefault = true;
        style.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 创建或更新全局风格
    /// </summary>
    [HttpPost("global")]
    public async Task<ActionResult<ApiResponse<AdminGlobalStyle>>> CreateOrUpdateGlobalStyle([FromBody] AdminGlobalStyleDto dto)
    {
        AdminGlobalStyle style;

        if (dto.Id > 0)
        {
            style = await _context.AdminGlobalStyles.FindAsync(dto.Id);
            if (style == null)
            {
                return NotFound(ApiResponse<AdminGlobalStyle>.Error("风格不存在", 404));
            }

            // 检查 styleKey 是否被其他记录使用
            if (style.StyleKey != dto.StyleKey)
            {
                var exists = await _context.AdminGlobalStyles
                    .AnyAsync(s => s.StyleKey == dto.StyleKey && s.Id != dto.Id);
                if (exists)
                {
                    return BadRequest(ApiResponse<AdminGlobalStyle>.Error("风格标识已存在", 400));
                }
            }

            style.StyleKey = dto.StyleKey;
            style.StyleName = dto.StyleName;
            style.Description = dto.Description;
            style.StyleConfig = dto.StyleConfig ?? "{}";
            style.PreviewImage = dto.PreviewImage;
            style.Enabled = dto.Enabled;
            style.UpdatedAt = DateTime.Now;
        }
        else
        {
            var exists = await _context.AdminGlobalStyles
                .AnyAsync(s => s.StyleKey == dto.StyleKey);
            if (exists)
            {
                return BadRequest(ApiResponse<AdminGlobalStyle>.Error("风格标识已存在", 400));
            }

            style = new AdminGlobalStyle
            {
                StyleKey = dto.StyleKey,
                StyleName = dto.StyleName,
                Description = dto.Description,
                StyleConfig = dto.StyleConfig ?? "{}",
                PreviewImage = dto.PreviewImage,
                Enabled = dto.Enabled,
                IsDefault = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.AdminGlobalStyles.Add(style);
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<AdminGlobalStyle>.Success(style));
    }

    /// <summary>
    /// 删除全局风格
    /// </summary>
    [HttpDelete("global/{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteGlobalStyle(long id)
    {
        var style = await _context.AdminGlobalStyles.FindAsync(id);
        if (style == null)
        {
            return NotFound(ApiResponse.Error("风格不存在", 404));
        }

        if (style.IsDefault)
        {
            return BadRequest(ApiResponse.Error("不能删除默认风格", 400));
        }

        _context.AdminGlobalStyles.Remove(style);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 获取所有模块风格
    /// </summary>
    [HttpGet("module")]
    public async Task<ActionResult<ApiResponse<List<AdminModuleStyle>>>> GetModuleStyles()
    {
        var styles = await _context.AdminModuleStyles
            .Where(s => s.Enabled)
            .OrderBy(s => s.ModuleKey)
            .ToListAsync();

        return Ok(ApiResponse<List<AdminModuleStyle>>.Success(styles));
    }

    /// <summary>
    /// 获取指定模块的风格
    /// </summary>
    [HttpGet("module/{moduleKey}")]
    public async Task<ActionResult<ApiResponse<AdminModuleStyle>>> GetModuleStyle(string moduleKey)
    {
        var style = await _context.AdminModuleStyles
            .FirstOrDefaultAsync(s => s.ModuleKey == moduleKey && s.Enabled);

        if (style == null)
        {
            return NotFound(ApiResponse<AdminModuleStyle>.Error("模块风格不存在", 404));
        }

        return Ok(ApiResponse<AdminModuleStyle>.Success(style));
    }

    /// <summary>
    /// 创建或更新模块风格
    /// </summary>
    [HttpPost("module")]
    public async Task<ActionResult<ApiResponse<AdminModuleStyle>>> CreateOrUpdateModuleStyle([FromBody] AdminModuleStyleDto dto)
    {
        AdminModuleStyle style;

        if (dto.Id > 0)
        {
            style = await _context.AdminModuleStyles.FindAsync(dto.Id);
            if (style == null)
            {
                return NotFound(ApiResponse<AdminModuleStyle>.Error("风格不存在", 404));
            }

            if (style.ModuleKey != dto.ModuleKey)
            {
                var exists = await _context.AdminModuleStyles
                    .AnyAsync(s => s.ModuleKey == dto.ModuleKey && s.Id != dto.Id);
                if (exists)
                {
                    return BadRequest(ApiResponse<AdminModuleStyle>.Error("模块标识已存在", 400));
                }
            }

            style.ModuleKey = dto.ModuleKey;
            style.ModuleName = dto.ModuleName;
            style.StyleConfig = dto.StyleConfig ?? "{}";
            style.Enabled = dto.Enabled;
            style.UpdatedAt = DateTime.Now;
        }
        else
        {
            var exists = await _context.AdminModuleStyles
                .AnyAsync(s => s.ModuleKey == dto.ModuleKey);
            if (exists)
            {
                return BadRequest(ApiResponse<AdminModuleStyle>.Error("模块标识已存在", 400));
            }

            style = new AdminModuleStyle
            {
                ModuleKey = dto.ModuleKey,
                ModuleName = dto.ModuleName,
                StyleConfig = dto.StyleConfig ?? "{}",
                Enabled = dto.Enabled,
                IsDefault = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.AdminModuleStyles.Add(style);
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<AdminModuleStyle>.Success(style));
    }

    /// <summary>
    /// 删除模块风格
    /// </summary>
    [HttpDelete("module/{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteModuleStyle(long id)
    {
        var style = await _context.AdminModuleStyles.FindAsync(id);
        if (style == null)
        {
            return NotFound(ApiResponse.Error("风格不存在", 404));
        }

        _context.AdminModuleStyles.Remove(style);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }
}

// DTOs
public class AdminGlobalStyleDto
{
    public long Id { get; set; }
    public string StyleKey { get; set; } = string.Empty;
    public string StyleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? StyleConfig { get; set; }
    public string? PreviewImage { get; set; }
    public bool Enabled { get; set; } = true;
}

public class AdminModuleStyleDto
{
    public long Id { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string? StyleConfig { get; set; }
    public bool Enabled { get; set; } = true;
}

