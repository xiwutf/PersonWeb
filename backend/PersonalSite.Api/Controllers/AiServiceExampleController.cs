using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// AI 服务调用示例控制器
/// 演示如何在业务代码中使用 AiServiceClient
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AiServiceExampleController : ControllerBase
{
    private readonly AiServiceClient _aiServiceClient;
    private readonly ILogger<AiServiceExampleController> _logger;

    public AiServiceExampleController(
        AiServiceClient aiServiceClient,
        ILogger<AiServiceExampleController> logger)
    {
        _aiServiceClient = aiServiceClient;
        _logger = logger;
    }

    /// <summary>
    /// 示例：调用 AI 聊天接口
    /// </summary>
    [HttpPost("chat-example")]
    public async Task<IActionResult> ChatExample([FromBody] ChatRequestDto request)
    {
        try
        {
            var result = await _aiServiceClient.ChatAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用 AI 聊天接口失败");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// 示例：生成文章摘要
    /// </summary>
    [HttpPost("summarize-example")]
    public async Task<IActionResult> SummarizeExample([FromBody] SummarizeRequestDto request)
    {
        try
        {
            var summary = await _aiServiceClient.SummarizeAsync(request.Text, request.MaxLength);
            return Ok(new { success = true, data = new { summary } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用 AI 摘要接口失败");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// 示例：生成标题和标签
    /// </summary>
    [HttpPost("title-tags-example")]
    public async Task<IActionResult> TitleTagsExample([FromBody] TitleAndTagsRequestDto request)
    {
        try
        {
            var result = await _aiServiceClient.GenerateTitleAndTagsAsync(request.Text, request.MaxTags);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用 AI 标题标签接口失败");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// 示例：解释代码
    /// </summary>
    [HttpPost("code-explain-example")]
    public async Task<IActionResult> CodeExplainExample([FromBody] CodeExplainRequestDto request)
    {
        try
        {
            var explanation = await _aiServiceClient.ExplainCodeAsync(request.Code, request.Language);
            return Ok(new { success = true, data = new { explanation } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用 AI 代码解释接口失败");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// 健康检查
    /// </summary>
    [HttpGet("health")]
    public async Task<IActionResult> HealthCheck()
    {
        var isHealthy = await _aiServiceClient.HealthCheckAsync();
        return Ok(new { success = true, data = new { healthy = isHealthy } });
    }
}

