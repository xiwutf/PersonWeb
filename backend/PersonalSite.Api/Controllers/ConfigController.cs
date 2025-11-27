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
}

public class ConfigUpdateDto
{
    public string Value { get; set; } = string.Empty;
}
