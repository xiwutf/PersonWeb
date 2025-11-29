using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly AppDbContext _context;

    public ConfigController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取全部配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<Dictionary<string, string>>>> GetAll()
    {
        var configs = await _context.SiteConfigs.ToListAsync();
        var dict = configs.ToDictionary(c => c.ConfigKey, c => c.ConfigValue ?? "");
        return Ok(ApiResponse<Dictionary<string, string>>.Success(dict));
    }

    /// <summary>
    /// 更新配置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{key}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Update(string key, [FromBody] ConfigUpdateDto dto)
    {
        var config = await _context.SiteConfigs.FindAsync(key);
        if (config == null)
        {
            // 如果不存在则创建
            config = new SiteConfig
            {
                ConfigKey = key,
                ConfigValue = dto.Value,
                UpdatedAt = DateTime.Now
            };
            _context.SiteConfigs.Add(config);
        }
        else
        {
            config.ConfigValue = dto.Value;
            config.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 获取当前启用的首页风格
    /// </summary>
    /// <returns></returns>
    [HttpGet("home-style")]
    public async Task<ActionResult<ApiResponse<HomeStyleResponse>>> GetHomeStyle()
    {
        var config = await _context.SiteConfigs.FindAsync("homeStyle");
        var styleKey = config?.ConfigValue ?? "dark-lab";

        return Ok(ApiResponse<HomeStyleResponse>.Success(new HomeStyleResponse
        {
            Style = styleKey
        }));
    }

    /// <summary>
    /// 设置当前启用的首页风格
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("home-style")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> SetHomeStyle([FromBody] SetHomeStyleDto dto)
    {
        // 验证风格是否存在
        var style = await _context.HomeStyles.FirstOrDefaultAsync(s => s.StyleKey == dto.Style && s.Enabled);
        if (style == null)
        {
            return BadRequest(ApiResponse.Error("风格不存在或已禁用", 400));
        }

        // 更新 SiteConfig
        var config = await _context.SiteConfigs.FindAsync("homeStyle");
        if (config == null)
        {
            config = new SiteConfig
            {
                ConfigKey = "homeStyle",
                ConfigValue = dto.Style,
                Description = "当前启用的首页风格",
                UpdatedAt = DateTime.Now
            };
            _context.SiteConfigs.Add(config);
        }
        else
        {
            config.ConfigValue = dto.Style;
            config.UpdatedAt = DateTime.Now;
        }

        // 更新 HomeStyle 的 is_default 标志
        var defaultStyles = await _context.HomeStyles.Where(s => s.IsDefault).ToListAsync();
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
    /// 获取当前全站主题
    /// 这是前台用来初始化全站主题的配置接口，允许匿名访问
    /// </summary>
    /// <returns></returns>
    [HttpGet("theme")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ThemeResponse>>> GetTheme()
    {
        // 从 SiteConfig 表中读取 ConfigKey == "site_theme" 的记录
        var config = await _context.SiteConfigs.FindAsync("site_theme");
        
        // 如果不存在，则返回默认主题 "light"
        var theme = config?.ConfigValue ?? "light";
        
        // 只返回合法值 "light" | "dark" | "tech-blue"，否则也回退到 "light"
        if (theme != "light" && theme != "dark" && theme != "tech-blue")
        {
            theme = "light";
        }

        return Ok(ApiResponse<ThemeResponse>.Success(new ThemeResponse
        {
            Theme = theme
        }));
    }

    /// <summary>
    /// 设置当前全站主题
    /// 需要管理员权限，后台管理界面调用此接口修改全站主题
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("theme")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ThemeResponse>>> SetTheme([FromBody] SetThemeDto dto)
    {
        // 校验 theme 值必须在 "light" | "dark" | "tech-blue" 三者之一
        if (dto.Theme != "light" && dto.Theme != "dark" && dto.Theme != "tech-blue")
        {
            return BadRequest(ApiResponse<ThemeResponse>.Error("主题值必须是 light、dark 或 tech-blue 之一", 400));
        }

        // 使用 SiteConfig 表进行 Upsert：ConfigKey = "site_theme"，ConfigValue = theme
        var config = await _context.SiteConfigs.FindAsync("site_theme");
        if (config == null)
        {
            // 如果不存在则创建
            config = new SiteConfig
            {
                ConfigKey = "site_theme",
                ConfigValue = dto.Theme,
                Description = "全站主题配置（light/dark/tech-blue）",
                UpdatedAt = DateTime.Now
            };
            _context.SiteConfigs.Add(config);
        }
        else
        {
            // 如果存在则更新
            config.ConfigValue = dto.Theme;
            config.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        // 返回更新后的主题信息
        return Ok(ApiResponse<ThemeResponse>.Success(new ThemeResponse
        {
            Theme = dto.Theme
        }));
    }
}

public class ConfigUpdateDto
{
    public string Value { get; set; } = string.Empty;
}

public class HomeStyleResponse
{
    public string Style { get; set; } = string.Empty;
}

public class SetHomeStyleDto
{
    public string Style { get; set; } = string.Empty;
}

/// <summary>
/// 主题响应 DTO
/// </summary>
public class ThemeResponse
{
    public string Theme { get; set; } = string.Empty;
}

/// <summary>
/// 设置主题请求 DTO
/// </summary>
public class SetThemeDto
{
    public string Theme { get; set; } = string.Empty;
}
