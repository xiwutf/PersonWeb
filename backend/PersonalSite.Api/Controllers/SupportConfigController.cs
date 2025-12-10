using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 客服配置控制器
/// </summary>
[ApiController]
[Route("api/admin/ai/support-config")]
[Authorize] // 需要认证
public class SupportConfigController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<SupportConfigController> _logger;

    public SupportConfigController(
        AppDbContext dbContext,
        ILogger<SupportConfigController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// 获取客服配置
    /// GET /api/admin/ai/support-config
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetConfig()
    {
        try
        {
            var systemPromptConfig = await _dbContext.SupportConfigs
                .FirstOrDefaultAsync(c => c.ConfigKey == "system_prompt");
            var faqConfig = await _dbContext.SupportConfigs
                .FirstOrDefaultAsync(c => c.ConfigKey == "faq_list");

            var systemPrompt = systemPromptConfig?.ConfigValue ?? "";
            var faqList = new List<FaqItem>();

            if (faqConfig != null && !string.IsNullOrEmpty(faqConfig.ConfigValue))
            {
                try
                {
                    faqList = JsonSerializer.Deserialize<List<FaqItem>>(faqConfig.ConfigValue) ?? new List<FaqItem>();
                }
                catch
                {
                    faqList = new List<FaqItem>();
                }
            }

            return Ok(new
            {
                SystemPrompt = systemPrompt,
                FaqList = faqList
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客服配置失败");
            return StatusCode(500, new { Success = false, Message = "获取配置失败" });
        }
    }

    /// <summary>
    /// 保存客服配置
    /// POST /api/admin/ai/support-config
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SaveConfig([FromBody] SupportConfigRequest request)
    {
        try
        {
            // 保存系统提示词
            var systemPromptConfig = await _dbContext.SupportConfigs
                .FirstOrDefaultAsync(c => c.ConfigKey == "system_prompt");

            if (systemPromptConfig == null)
            {
                systemPromptConfig = new SupportConfig
                {
                    ConfigKey = "system_prompt",
                    ConfigValue = request.SystemPrompt ?? "",
                    Description = "客服智能体系统提示词",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _dbContext.SupportConfigs.Add(systemPromptConfig);
            }
            else
            {
                systemPromptConfig.ConfigValue = request.SystemPrompt ?? "";
                systemPromptConfig.UpdatedAt = DateTime.Now;
            }

            // 保存 FAQ 列表
            var faqConfig = await _dbContext.SupportConfigs
                .FirstOrDefaultAsync(c => c.ConfigKey == "faq_list");

            var faqJson = JsonSerializer.Serialize(request.FaqList ?? new List<FaqItem>());

            if (faqConfig == null)
            {
                faqConfig = new SupportConfig
                {
                    ConfigKey = "faq_list",
                    ConfigValue = faqJson,
                    Description = "FAQ 列表（JSON 格式）",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _dbContext.SupportConfigs.Add(faqConfig);
            }
            else
            {
                faqConfig.ConfigValue = faqJson;
                faqConfig.UpdatedAt = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { Success = true, Message = "配置保存成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存客服配置失败");
            return StatusCode(500, new { Success = false, Message = "保存配置失败" });
        }
    }
}

/// <summary>
/// 客服配置请求 DTO
/// </summary>
public class SupportConfigRequest
{
    public string? SystemPrompt { get; set; }
    public List<FaqItem>? FaqList { get; set; }
}

