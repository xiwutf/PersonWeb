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
