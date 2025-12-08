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
    /// 
    /// 重构说明（2024-12-XX）：
    /// - 现在只支持 light 和 dark 两个主题
    /// - 如果数据库中是旧值（tech-blue、paper、forest 等），会自动映射为 light 或 dark
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
        
        // 标准化主题（映射旧主题为 light 或 dark）
        theme = NormalizeTheme(theme);
        
        // 如果映射后的主题不是 light 或 dark，回退到 light
        if (theme != "light" && theme != "dark")
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
    /// 
    /// 重构说明（2024-12-XX）：
    /// - 现在只支持 light 和 dark 两个主题
    /// - 如果传入旧主题（tech-blue、paper、forest 等），会返回 400 错误
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("theme")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ThemeResponse>>> SetTheme([FromBody] SetThemeDto dto)
    {
        // 校验 theme 值必须是 light 或 dark
        if (dto.Theme != "light" && dto.Theme != "dark")
        {
            return BadRequest(ApiResponse<ThemeResponse>.Error("主题值必须是 'light' 或 'dark'", 400));
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
                Description = "全站主题配置（light/dark）",
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

    /// <summary>
    /// 获取模块主题配置 + 全局主题
    /// 前台也可能需要（例如预加载所有模块主题），允许匿名访问
    /// </summary>
    /// <returns></returns>
    [HttpGet("module-themes")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ModuleThemesResponse>>> GetModuleThemes()
    {
        // 1. 获取全局主题（并标准化为 light 或 dark）
        var globalThemeConfig = await _context.SiteConfigs.FindAsync("site_theme");
        var globalTheme = NormalizeTheme(globalThemeConfig?.ConfigValue ?? "light");

        // 2. 获取所有模块主题配置（ConfigKey 以 "module_theme:" 开头）
        var moduleThemeConfigs = await _context.SiteConfigs
            .Where(c => c.ConfigKey != null && c.ConfigKey.StartsWith("module_theme:"))
            .ToListAsync();

        var modules = new List<ModuleThemeConfigDto>();
        foreach (var config in moduleThemeConfigs)
        {
            // 解析 ModuleId（冒号后的部分）
            var moduleId = config.ConfigKey!.Substring("module_theme:".Length);
            // 标准化模块主题（映射旧主题为 light 或 dark）
            var normalizedModuleTheme = string.IsNullOrEmpty(config.ConfigValue) 
                ? null 
                : NormalizeTheme(config.ConfigValue);
            
            modules.Add(new ModuleThemeConfigDto
            {
                ModuleId = moduleId,
                Theme = normalizedModuleTheme
            });
        }

        // 3. 可选主题列表
        // 重构说明（2024-12-XX）：现在只支持 light 和 dark 两个主题
        var availableThemes = new List<string> { "light", "dark" };

        return Ok(ApiResponse<ModuleThemesResponse>.Success(new ModuleThemesResponse
        {
            GlobalTheme = globalTheme,
            Modules = modules,
            AvailableThemes = availableThemes
        }));
    }

    /// <summary>
    /// 保存模块主题配置
    /// 需要管理员权限，后台管理界面调用此接口修改模块主题
    /// </summary>
    /// <param name="dtoList">模块主题配置列表</param>
    /// <returns></returns>
    [HttpPut("module-themes")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ModuleThemesResponse>>> SetModuleThemes([FromBody] List<ModuleThemeConfigDto> dtoList)
    {
        // 可选主题列表（用于校验）
        // 重构说明（2024-12-XX）：现在只支持 light 和 dark 两个主题
        var availableThemes = new List<string> { "light", "dark" };

        foreach (var dto in dtoList)
        {
            // 校验 ModuleId 非空
            if (string.IsNullOrWhiteSpace(dto.ModuleId))
            {
                return BadRequest(ApiResponse<ModuleThemesResponse>.Error("ModuleId 不能为空", 400));
            }

            // 如果 Theme 非空，校验是否为合法主题
            if (!string.IsNullOrEmpty(dto.Theme) && !availableThemes.Contains(dto.Theme))
            {
                return BadRequest(ApiResponse<ModuleThemesResponse>.Error($"主题 '{dto.Theme}' 不在支持的主题列表中", 400));
            }

            var configKey = $"module_theme:{dto.ModuleId}";
            var config = await _context.SiteConfigs.FindAsync(configKey);

            if (string.IsNullOrEmpty(dto.Theme))
            {
                // Theme 为空，删除记录或设置为空（这里选择删除，前端约定空 = 跟随全局）
                if (config != null)
                {
                    _context.SiteConfigs.Remove(config);
                }
            }
            else
            {
                // Theme 非空，写入/更新记录
                if (config == null)
                {
                    config = new SiteConfig
                    {
                        ConfigKey = configKey,
                        ConfigValue = dto.Theme,
                        Description = $"模块 {dto.ModuleId} 的主题配置",
                        UpdatedAt = DateTime.Now
                    };
                    _context.SiteConfigs.Add(config);
                }
                else
                {
                    config.ConfigValue = dto.Theme;
                    config.UpdatedAt = DateTime.Now;
                }
            }
        }

        await _context.SaveChangesAsync();

        // 返回最新结果
        return await GetModuleThemes();
    }

    /// <summary>
    /// 读取某个主题的 tokens
    /// 只给后台编辑界面用，需要管理员权限
    /// </summary>
    /// <param name="themeKey">主题键，例如 "light", "dark", "tech-blue"</param>
    /// <returns></returns>
    [HttpGet("theme-tokens/{themeKey}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ThemeTokensDto>>> GetThemeTokens(string themeKey)
    {
        // 校验 themeKey 是否合法
        // 重构说明（2024-12-XX）：现在只支持 light 和 dark 两个主题
        var availableThemes = new List<string> { "light", "dark" };
        if (!availableThemes.Contains(themeKey))
        {
            return BadRequest(ApiResponse<ThemeTokensDto>.Error($"主题 '{themeKey}' 不在支持的主题列表中", 400));
        }

        var configKey = $"theme_tokens:{themeKey}";
        var config = await _context.SiteConfigs.FindAsync(configKey);

        Dictionary<string, string> tokens = new Dictionary<string, string>();

        if (config != null && !string.IsNullOrEmpty(config.ConfigValue))
        {
            try
            {
                // 解析 JSON 为 Dictionary<string, string>
                tokens = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(config.ConfigValue) 
                    ?? new Dictionary<string, string>();
            }
            catch
            {
                // JSON 解析失败，返回空字典
                tokens = new Dictionary<string, string>();
            }
        }

        return Ok(ApiResponse<ThemeTokensDto>.Success(new ThemeTokensDto
        {
            ThemeKey = themeKey,
            Tokens = tokens
        }));
    }

    /// <summary>
    /// 保存某个主题的 tokens
    /// 需要管理员权限，后台编辑界面调用此接口保存主题 tokens
    /// </summary>
    /// <param name="themeKey">主题键，应与请求体中的 ThemeKey 一致</param>
    /// <param name="dto">主题 tokens DTO</param>
    /// <returns></returns>
    [HttpPut("theme-tokens/{themeKey}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ThemeTokensDto>>> SetThemeTokens(string themeKey, [FromBody] ThemeTokensDto dto)
    {
        // 校验 themeKey 是否合法
        // 重构说明（2024-12-XX）：现在只支持 light 和 dark 两个主题
        var availableThemes = new List<string> { "light", "dark" };
        if (!availableThemes.Contains(themeKey))
        {
            return BadRequest(ApiResponse<ThemeTokensDto>.Error($"主题 '{themeKey}' 不在支持的主题列表中", 400));
        }

        // 校验请求体中的 ThemeKey 与路由参数一致
        if (dto.ThemeKey != themeKey)
        {
            return BadRequest(ApiResponse<ThemeTokensDto>.Error("路由参数中的 themeKey 与请求体中的 ThemeKey 不一致", 400));
        }

        // 基本校验：tokens 键值非空
        if (dto.Tokens == null)
        {
            return BadRequest(ApiResponse<ThemeTokensDto>.Error("Tokens 不能为空", 400));
        }

        // 过滤掉空键或空值
        var validTokens = dto.Tokens
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Key) && !string.IsNullOrWhiteSpace(kv.Value))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        // 将 tokens 序列化为 JSON
        var jsonValue = System.Text.Json.JsonSerializer.Serialize(validTokens);

        var configKey = $"theme_tokens:{themeKey}";
        var config = await _context.SiteConfigs.FindAsync(configKey);

        if (config == null)
        {
            config = new SiteConfig
            {
                ConfigKey = configKey,
                ConfigValue = jsonValue,
                Description = $"主题 {themeKey} 的 tokens 配置",
                UpdatedAt = DateTime.Now
            };
            _context.SiteConfigs.Add(config);
        }
        else
        {
            config.ConfigValue = jsonValue;
            config.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        // 返回更新后的结果
        return Ok(ApiResponse<ThemeTokensDto>.Success(new ThemeTokensDto
        {
            ThemeKey = themeKey,
            Tokens = validTokens
        }));
    }

    /// <summary>
    /// 主题映射函数：将旧主题名称映射为 light 或 dark
    /// 
    /// 映射规则：
    /// - 如果主题名包含 "light" 或 "paper" → 映射为 "light"
    /// - 如果主题名包含 "dark" 或 "tech" 或 "forest" 或 "hybrid" → 映射为 "dark"
    /// - 其他情况 → 默认映射为 "dark"
    /// 
    /// 用途：
    /// - 兼容数据库中的旧主题配置（tech-blue、paper、forest、hybrid-super 等）
    /// - 避免因旧数据导致主题切换失败
    /// </summary>
    /// <param name="theme">主题名称（可能是旧主题名）</param>
    /// <returns>标准化后的主题名称（"light" 或 "dark"）</returns>
    private static string NormalizeTheme(string? theme)
    {
        if (string.IsNullOrWhiteSpace(theme))
        {
            return "light";
        }

        var themeLower = theme.ToLowerInvariant();

        // 如果已经是 light 或 dark，直接返回
        if (themeLower == "light" || themeLower == "dark")
        {
            return themeLower;
        }

        // 根据关键词映射
        if (themeLower.Contains("light") || themeLower.Contains("paper"))
        {
            return "light";
        }

        // 深色主题关键词
        if (themeLower.Contains("dark") ||
            themeLower.Contains("tech") ||
            themeLower.Contains("forest") ||
            themeLower.Contains("hybrid") ||
            themeLower.Contains("lab"))
        {
            return "dark";
        }

        // 默认返回 dark（更安全，避免浅色主题在深色背景下不可见）
        return "dark";
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

/// <summary>
/// 模块主题配置 DTO
/// 用于配置某个模块的独立主题，null/空表示跟随全局主题
/// </summary>
public class ModuleThemeConfigDto
{
    /// <summary>
    /// 模块唯一标识，如 "home_hero", "ai_lab"
    /// </summary>
    public string ModuleId { get; set; } = string.Empty;

    /// <summary>
    /// 主题键，如 "light" | "dark" | "tech-blue"，null/空表示跟随全局主题
    /// </summary>
    public string? Theme { get; set; }
}

/// <summary>
/// 模块主题配置响应 DTO
/// 包含全局主题、所有模块的主题配置、以及可选主题列表
/// </summary>
public class ModuleThemesResponse
{
    /// <summary>
    /// 当前全局主题
    /// </summary>
    public string GlobalTheme { get; set; } = string.Empty;

    /// <summary>
    /// 当前所有模块的主题配置
    /// </summary>
    public List<ModuleThemeConfigDto> Modules { get; set; } = new List<ModuleThemeConfigDto>();

    /// <summary>
    /// 可选主题列表，例如 ["light", "dark", "tech-blue"]
    /// </summary>
    public List<string> AvailableThemes { get; set; } = new List<string>();
}

/// <summary>
/// 主题 tokens DTO
/// 用于读/写某个主题的样式 token 配置（颜色、圆角、阴影等）
/// </summary>
public class ThemeTokensDto
{
    /// <summary>
    /// 主题键，例如 "light", "dark", "tech-blue"
    /// </summary>
    public string ThemeKey { get; set; } = string.Empty;

    /// <summary>
    /// Token 键值对，key 例如 "color.bg.body"，value 例如 "#ffffff"
    /// </summary>
    public Dictionary<string, string> Tokens { get; set; } = new Dictionary<string, string>();
}
