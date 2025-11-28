using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/admin/home-style")]
[Authorize]
public class HomeStyleController : ControllerBase
{
    private readonly AppDbContext _context;

    public HomeStyleController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取全部风格
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<HomeStyle>>>> GetAll()
    {
        var styles = await _context.HomeStyles.OrderByDescending(s => s.IsDefault).ThenByDescending(s => s.CreatedAt).ToListAsync();
        return Ok(ApiResponse<List<HomeStyle>>.Success(styles));
    }

    /// <summary>
    /// 获取单个风格
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<HomeStyle>>> GetById(long id)
    {
        var style = await _context.HomeStyles.FindAsync(id);
        if (style == null)
        {
            return NotFound(ApiResponse<HomeStyle>.Error("风格不存在", 404));
        }
        return Ok(ApiResponse<HomeStyle>.Success(style));
    }

    /// <summary>
    /// 新增或编辑风格
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<HomeStyle>>> CreateOrUpdate([FromBody] HomeStyleDto dto)
    {
        HomeStyle style;

        if (dto.Id > 0)
        {
            // 编辑
            var foundStyle = await _context.HomeStyles.FindAsync(dto.Id);
            if (foundStyle == null)
            {
                return NotFound(ApiResponse<HomeStyle>.Error("风格不存在", 404));
            }
            style = foundStyle;

            // 检查 styleKey 是否被其他记录使用
            if (style.StyleKey != dto.StyleKey)
            {
                var exists = await _context.HomeStyles.AnyAsync(s => s.StyleKey == dto.StyleKey && s.Id != dto.Id);
                if (exists)
                {
                    return BadRequest(ApiResponse<HomeStyle>.Error("风格标识已存在", 400));
                }
            }

            style.StyleKey = dto.StyleKey;
            style.Name = dto.Name;
            style.Description = dto.Description;
            style.PreviewImage = dto.PreviewImage;
            style.Enabled = dto.Enabled;
            style.UpdatedAt = DateTime.Now;
        }
        else
        {
            // 新增
            var exists = await _context.HomeStyles.AnyAsync(s => s.StyleKey == dto.StyleKey);
            if (exists)
            {
                return BadRequest(ApiResponse<HomeStyle>.Error("风格标识已存在", 400));
            }

            style = new HomeStyle
            {
                StyleKey = dto.StyleKey,
                Name = dto.Name,
                Description = dto.Description,
                PreviewImage = dto.PreviewImage,
                Enabled = dto.Enabled,
                IsDefault = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.HomeStyles.Add(style);
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<HomeStyle>.Success(style));
    }

    /// <summary>
    /// 删除风格
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        var style = await _context.HomeStyles.FindAsync(id);
        if (style == null)
        {
            return NotFound(ApiResponse.Error("风格不存在", 404));
        }

        // 如果是默认风格，不能删除
        if (style.IsDefault)
        {
            return BadRequest(ApiResponse.Error("不能删除当前启用的风格，请先切换其他风格", 400));
        }

        _context.HomeStyles.Remove(style);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 设置为当前首页风格
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id}/set-default")]
    public async Task<ActionResult<ApiResponse>> SetDefault(long id)
    {
        var style = await _context.HomeStyles.FindAsync(id);
        if (style == null)
        {
            return NotFound(ApiResponse.Error("风格不存在", 404));
        }

        if (!style.Enabled)
        {
            return BadRequest(ApiResponse.Error("不能启用已禁用的风格", 400));
        }

        // 清除其他默认标志
        var defaultStyles = await _context.HomeStyles.Where(s => s.IsDefault).ToListAsync();
        foreach (var s in defaultStyles)
        {
            s.IsDefault = false;
        }

        // 设置当前为默认
        style.IsDefault = true;
        style.UpdatedAt = DateTime.Now;

        // 更新 SiteConfig
        var config = await _context.SiteConfigs.FindAsync("homeStyle");
        if (config == null)
        {
            config = new SiteConfig
            {
                ConfigKey = "homeStyle",
                ConfigValue = style.StyleKey,
                Description = "当前启用的首页风格",
                UpdatedAt = DateTime.Now
            };
            _context.SiteConfigs.Add(config);
        }
        else
        {
            config.ConfigValue = style.StyleKey;
            config.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success());
    }
}

public class HomeStyleDto
{
    public long Id { get; set; }
    public string StyleKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PreviewImage { get; set; }
    public bool Enabled { get; set; } = true;
}

