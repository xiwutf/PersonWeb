using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThemeController : ControllerBase
{
    private readonly AppDbContext _context;

    public ThemeController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有启用的主题风格
    /// </summary>
    [HttpGet("themes")]
    public async Task<ActionResult<ApiResponse<List<ThemeStyle>>>> GetThemes()
    {
        var themes = await _context.ThemeStyles
            .Where(t => t.IsEnabled)
            .OrderBy(t => t.Sort)
            .ToListAsync();
        return Ok(ApiResponse<List<ThemeStyle>>.Success(themes));
    }

    /// <summary>
    /// 获取当前默认主题
    /// </summary>
    [HttpGet("themes/default")]
    public async Task<ActionResult<ApiResponse<ThemeStyle>>> GetDefaultTheme()
    {
        var theme = await _context.ThemeStyles
            .FirstOrDefaultAsync(t => t.IsDefault && t.IsEnabled);

        if (theme == null)
        {
            theme = await _context.ThemeStyles
                .FirstOrDefaultAsync(t => t.Code == "default" && t.IsEnabled);
        }

        if (theme == null)
        {
            return NotFound(ApiResponse<ThemeStyle>.Error("未找到默认主题", 404));
        }

        return Ok(ApiResponse<ThemeStyle>.Success(theme));
    }

    /// <summary>
    /// 获取所有启用的背景效果
    /// </summary>
    [HttpGet("backgrounds")]
    public async Task<ActionResult<ApiResponse<List<BackgroundEffect>>>> GetBackgrounds()
    {
        var backgrounds = await _context.BackgroundEffects
            .Where(b => b.IsEnabled)
            .OrderBy(b => b.Sort)
            .ToListAsync();
        return Ok(ApiResponse<List<BackgroundEffect>>.Success(backgrounds));
    }

    /// <summary>
    /// 获取主题设置
    /// </summary>
    [HttpGet("settings")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, string>>>> GetSettings()
    {
        var settings = await _context.ThemeSettings.ToListAsync();
        var dict = settings.ToDictionary(s => s.SettingKey, s => s.SettingValue ?? "");
        return Ok(ApiResponse<Dictionary<string, string>>.Success(dict));
    }

    /// <summary>
    /// 获取或创建用户主题偏好
    /// </summary>
    [HttpPost("preference")]
    public async Task<ActionResult<ApiResponse<UserThemePreference>>> GetOrCreatePreference([FromBody] UserPreferenceRequest request)
    {
        var preference = await _context.UserThemePreferences
            .FirstOrDefaultAsync(p => p.VisitorId == request.VisitorId);

        if (preference == null)
        {
            // 获取默认设置
            var defaultTheme = await _context.ThemeSettings
                .FirstOrDefaultAsync(s => s.SettingKey == "default_theme");
            var defaultBg = await _context.ThemeSettings
                .FirstOrDefaultAsync(s => s.SettingKey == "default_background");

            preference = new UserThemePreference
            {
                VisitorId = request.VisitorId,
                ThemeCode = defaultTheme?.SettingValue ?? "default",
                BackgroundEffectCode = defaultBg?.SettingValue ?? "particles",
                AutoSwitch = false,
                SwitchInterval = 0,
                CreatedAt = DateTime.Now
            };
            _context.UserThemePreferences.Add(preference);
            await _context.SaveChangesAsync();
        }

        return Ok(ApiResponse<UserThemePreference>.Success(preference));
    }

    /// <summary>
    /// 更新用户主题偏好
    /// </summary>
    [HttpPost("preference/update")]
    public async Task<ActionResult<ApiResponse>> UpdatePreference([FromBody] UpdatePreferenceRequest request)
    {
        var preference = await _context.UserThemePreferences
            .FirstOrDefaultAsync(p => p.VisitorId == request.VisitorId);

        if (preference == null)
        {
            preference = new UserThemePreference
            {
                VisitorId = request.VisitorId,
                CreatedAt = DateTime.Now
            };
            _context.UserThemePreferences.Add(preference);
        }

        if (request.ThemeCode != null)
        {
            preference.ThemeCode = request.ThemeCode;
        }
        if (request.BackgroundEffectCode != null)
        {
            preference.BackgroundEffectCode = request.BackgroundEffectCode;
        }
        if (request.AutoSwitch.HasValue)
        {
            preference.AutoSwitch = request.AutoSwitch.Value;
        }
        if (request.SwitchInterval.HasValue)
        {
            preference.SwitchInterval = request.SwitchInterval.Value;
        }

        preference.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }

    // ========== 后台管理接口 ==========

    /// <summary>
    /// 获取所有主题（后台管理）
    /// </summary>
    [HttpGet("admin/themes")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ThemeStyle>>>> GetAllThemes()
    {
        var themes = await _context.ThemeStyles
            .OrderBy(t => t.Sort)
            .ToListAsync();
        return Ok(ApiResponse<List<ThemeStyle>>.Success(themes));
    }

    /// <summary>
    /// 创建或更新主题
    /// </summary>
    [HttpPost("admin/themes")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ThemeStyle>>> CreateOrUpdateTheme([FromBody] ThemeStyleDto dto)
    {
        ThemeStyle theme;

        if (dto.Id > 0)
        {
            theme = await _context.ThemeStyles.FindAsync(dto.Id);
            if (theme == null)
            {
                return NotFound(ApiResponse<ThemeStyle>.Error("主题不存在", 404));
            }
        }
        else
        {
            var exists = await _context.ThemeStyles
                .AnyAsync(t => t.Code == dto.Code);
            if (exists)
            {
                return BadRequest(ApiResponse<ThemeStyle>.Error("主题代码已存在", 400));
            }

            theme = new ThemeStyle
            {
                CreatedAt = DateTime.Now
            };
            _context.ThemeStyles.Add(theme);
        }

        theme.Name = dto.Name;
        theme.Code = dto.Code;
        theme.DisplayName = dto.DisplayName;
        theme.Description = dto.Description;
        theme.PreviewImage = dto.PreviewImage;
        theme.IsEnabled = dto.IsEnabled;
        theme.IsDefault = dto.IsDefault;
        theme.Sort = dto.Sort;
        theme.StyleConfig = dto.StyleConfig;
        theme.UpdatedAt = DateTime.Now;

        // 如果设置为默认，清除其他默认标志
        if (dto.IsDefault)
        {
            var defaultThemes = await _context.ThemeStyles
                .Where(t => t.IsDefault && t.Id != theme.Id)
                .ToListAsync();
            foreach (var t in defaultThemes)
            {
                t.IsDefault = false;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<ThemeStyle>.Success(theme));
    }

    /// <summary>
    /// 设置默认主题
    /// </summary>
    [HttpPost("admin/themes/{id}/set-default")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> SetDefaultTheme(long id)
    {
        var theme = await _context.ThemeStyles.FindAsync(id);
        if (theme == null || !theme.IsEnabled)
        {
            return NotFound(ApiResponse.Error("主题不存在或已禁用", 404));
        }

        // 清除其他默认标志
        var defaultThemes = await _context.ThemeStyles
            .Where(t => t.IsDefault)
            .ToListAsync();
        foreach (var t in defaultThemes)
        {
            t.IsDefault = false;
        }

        theme.IsDefault = true;
        theme.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 删除主题
    /// </summary>
    [HttpDelete("admin/themes/{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteTheme(long id)
    {
        var theme = await _context.ThemeStyles.FindAsync(id);
        if (theme == null)
        {
            return NotFound(ApiResponse.Error("主题不存在", 404));
        }

        _context.ThemeStyles.Remove(theme);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }
}

// DTOs
public class UserPreferenceRequest
{
    public string VisitorId { get; set; } = string.Empty;
}

public class UpdatePreferenceRequest
{
    public string VisitorId { get; set; } = string.Empty;
    public string? ThemeCode { get; set; }
    public string? BackgroundEffectCode { get; set; }
    public bool? AutoSwitch { get; set; }
    public int? SwitchInterval { get; set; }
}

public class ThemeStyleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PreviewImage { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public int Sort { get; set; }
    public string? StyleConfig { get; set; }
}

