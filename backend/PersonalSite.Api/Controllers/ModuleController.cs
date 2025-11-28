using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModuleController : ControllerBase
{
    private readonly AppDbContext _context;

    public ModuleController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有启用的模块
    /// </summary>
    [HttpGet("enabled")]
    public async Task<ActionResult<ApiResponse<List<Module>>>> GetEnabledModules()
    {
        try
        {
            var modules = await _context.Modules
                .Where(m => m.IsEnabled)
                .OrderBy(m => m.Sort)
                .ToListAsync();
            return Ok(ApiResponse<List<Module>>.Success(modules));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<Module>>.Error($"获取启用模块失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取所有模块（后台管理）
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<Module>>>> GetAllModules()
    {
        try
        {
            var modules = await _context.Modules
                .OrderBy(m => m.Sort)
                .ToListAsync();
            return Ok(ApiResponse<List<Module>>.Success(modules));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<Module>>.Error($"获取模块列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取单个模块
    /// </summary>
    [HttpGet("{moduleKey}")]
    public async Task<ActionResult<ApiResponse<Module>>> GetModule(string moduleKey)
    {
        var module = await _context.Modules
            .FirstOrDefaultAsync(m => m.ModuleKey == moduleKey);

        if (module == null)
        {
            return NotFound(ApiResponse<Module>.Error("模块不存在", 404));
        }

        return Ok(ApiResponse<Module>.Success(module));
    }

    /// <summary>
    /// 获取模块配置
    /// </summary>
    [HttpGet("{moduleKey}/configs")]
    public async Task<ActionResult<ApiResponse<List<ModuleConfig>>>> GetModuleConfigs(string moduleKey)
    {
        var configs = await _context.ModuleConfigs
            .Where(c => c.ModuleKey == moduleKey)
            .ToListAsync();
        return Ok(ApiResponse<List<ModuleConfig>>.Success(configs));
    }

    /// <summary>
    /// 设置模块配置
    /// </summary>
    [HttpPost("config")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> SetModuleConfig([FromBody] ModuleConfigDto dto)
    {
        // 检查模块是否存在
        var module = await _context.Modules
            .FirstOrDefaultAsync(m => m.ModuleKey == dto.ModuleKey);
        if (module == null)
        {
            return NotFound(ApiResponse.Error("模块不存在", 404));
        }

        var config = await _context.ModuleConfigs
            .FirstOrDefaultAsync(c => c.ModuleKey == dto.ModuleKey && c.ConfigKey == dto.ConfigKey);

        if (config == null)
        {
            config = new ModuleConfig
            {
                ModuleKey = dto.ModuleKey,
                ConfigKey = dto.ConfigKey,
                CreatedAt = DateTime.Now
            };
            _context.ModuleConfigs.Add(config);
        }

        config.ConfigValue = dto.ConfigValue is string 
            ? dto.ConfigValue.ToString() 
            : System.Text.Json.JsonSerializer.Serialize(dto.ConfigValue);
        config.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 启用模块
    /// </summary>
    [HttpPost("{moduleKey}/enable")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> EnableModule(string moduleKey)
    {
        var module = await _context.Modules
            .FirstOrDefaultAsync(m => m.ModuleKey == moduleKey);

        if (module == null)
        {
            return NotFound(ApiResponse.Error("模块不存在", 404));
        }

        // 检查依赖
        if (!string.IsNullOrEmpty(module.Dependencies))
        {
            try
            {
                var dependencies = System.Text.Json.JsonSerializer.Deserialize<List<string>>(module.Dependencies);
                if (dependencies != null && dependencies.Any())
                {
                    var disabledDeps = await _context.Modules
                        .Where(m => dependencies.Contains(m.ModuleKey) && !m.IsEnabled)
                        .Select(m => m.ModuleName)
                        .ToListAsync();

                    if (disabledDeps.Any())
                    {
                        return BadRequest(ApiResponse.Error($"请先启用依赖模块: {string.Join(", ", disabledDeps)}", 400));
                    }
                }
            }
            catch
            {
                // 忽略依赖解析错误
            }
        }

        module.IsEnabled = true;
        module.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 禁用模块
    /// </summary>
    [HttpPost("{moduleKey}/disable")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DisableModule(string moduleKey)
    {
        var module = await _context.Modules
            .FirstOrDefaultAsync(m => m.ModuleKey == moduleKey);

        if (module == null)
        {
            return NotFound(ApiResponse.Error("模块不存在", 404));
        }

        if (module.IsCore)
        {
            return BadRequest(ApiResponse.Error("核心模块不能禁用", 400));
        }

        // 检查是否有其他模块依赖此模块
        var dependentModules = await _context.Modules
            .Where(m => m.Dependencies != null && m.Dependencies.Contains(moduleKey) && m.IsEnabled)
            .Select(m => m.ModuleName)
            .ToListAsync();

        if (dependentModules.Any())
        {
            return BadRequest(ApiResponse.Error($"以下模块依赖此模块，请先禁用: {string.Join(", ", dependentModules)}", 400));
        }

        module.IsEnabled = false;
        module.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// 创建或更新模块
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Module>>> CreateOrUpdateModule([FromBody] ModuleDto dto)
    {
        Module module;

        if (dto.Id > 0)
        {
            var foundModule = await _context.Modules.FindAsync(dto.Id);
            if (foundModule == null)
            {
                return NotFound(ApiResponse<Module>.Error("模块不存在", 404));
            }
            module = foundModule;
        }
        else
        {
            var exists = await _context.Modules
                .AnyAsync(m => m.ModuleKey == dto.ModuleKey);
            if (exists)
            {
                return BadRequest(ApiResponse<Module>.Error("模块标识已存在", 400));
            }

            module = new Module
            {
                CreatedAt = DateTime.Now
            };
            _context.Modules.Add(module);
        }

        module.ModuleKey = dto.ModuleKey;
        module.ModuleName = dto.ModuleName;
        module.ModuleVersion = dto.ModuleVersion;
        module.Description = dto.Description;
        module.Author = dto.Author;
        module.Icon = dto.Icon;
        module.Category = dto.Category;
        module.Dependencies = dto.Dependencies != null 
            ? System.Text.Json.JsonSerializer.Serialize(dto.Dependencies) 
            : null;
        module.Routes = dto.Routes != null 
            ? System.Text.Json.JsonSerializer.Serialize(dto.Routes) 
            : null;
        module.Components = dto.Components != null 
            ? System.Text.Json.JsonSerializer.Serialize(dto.Components) 
            : null;
        module.Permissions = dto.Permissions != null 
            ? System.Text.Json.JsonSerializer.Serialize(dto.Permissions) 
            : null;
        module.ConfigSchema = dto.ConfigSchema != null 
            ? System.Text.Json.JsonSerializer.Serialize(dto.ConfigSchema) 
            : null;
        module.IsEnabled = dto.IsEnabled;
        module.IsCore = dto.IsCore;
        module.Sort = dto.Sort;
        module.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<Module>.Success(module));
    }

    /// <summary>
    /// 删除模块
    /// </summary>
    [HttpDelete("{moduleKey}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteModule(string moduleKey)
    {
        var module = await _context.Modules
            .FirstOrDefaultAsync(m => m.ModuleKey == moduleKey);

        if (module == null)
        {
            return NotFound(ApiResponse.Error("模块不存在", 404));
        }

        if (module.IsCore)
        {
            return BadRequest(ApiResponse.Error("核心模块不能删除", 400));
        }

        _context.Modules.Remove(module);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success());
    }
}

// DTOs
public class ModuleConfigDto
{
    public string ModuleKey { get; set; } = string.Empty;
    public string ConfigKey { get; set; } = string.Empty;
    public object? ConfigValue { get; set; }
}

public class ModuleDto
{
    public long Id { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string ModuleVersion { get; set; } = "1.0.0";
    public string? Description { get; set; }
    public string? Author { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public List<string>? Dependencies { get; set; }
    public List<object>? Routes { get; set; }
    public List<object>? Components { get; set; }
    public Dictionary<string, object>? Permissions { get; set; }
    public Dictionary<string, object>? ConfigSchema { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsCore { get; set; } = false;
    public int Sort { get; set; }
}

