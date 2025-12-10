using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// AI 智能体控制器
/// 统一管理所有智能体的 API 端点
/// </summary>
[ApiController]
[Route("api/ai")]
[Authorize] // 需要认证
public class AiAgentController : ControllerBase
{
    private readonly ContentAgentService _contentAgentService;
    private readonly DemoAgentService _demoAgentService;
    private readonly LeadAgentService _leadAgentService;
    private readonly ILogger<AiAgentController> _logger;

    public AiAgentController(
        ContentAgentService contentAgentService,
        DemoAgentService demoAgentService,
        LeadAgentService leadAgentService,
        ILogger<AiAgentController> logger)
    {
        _contentAgentService = contentAgentService;
        _demoAgentService = demoAgentService;
        _leadAgentService = leadAgentService;
        _logger = logger;
    }

    /// <summary>
    /// 内容生成接口
    /// POST /api/ai/content/generate
    /// </summary>
    [HttpPost("content/generate")]
    public async Task<IActionResult> GenerateContent([FromBody] ContentGenerationRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { Success = false, Message = "请求参数不能为空" });
            }

            var result = await _contentAgentService.GenerateContentAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "内容生成失败");
            return StatusCode(500, new
            {
                Success = false,
                Message = "内容生成失败",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Demo 描述生成接口
    /// POST /api/ai/demo/describe
    /// </summary>
    [HttpPost("demo/describe")]
    public async Task<IActionResult> GenerateDemoDescription([FromBody] DemoDescriptionRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { Success = false, Message = "请求参数不能为空" });
            }

            if (!request.ProjectId.HasValue && !request.ToolId.HasValue)
            {
                return BadRequest(new { Success = false, Message = "必须提供 ProjectId 或 ToolId" });
            }

            var result = await _demoAgentService.GenerateDescriptionAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Demo 描述生成失败");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Demo 描述生成失败",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// 线索分析接口
    /// POST /api/ai/leads/analyze
    /// </summary>
    [HttpPost("leads/analyze")]
    public async Task<IActionResult> AnalyzeLead([FromBody] LeadAnalysisRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { Success = false, Message = "请求参数不能为空" });
            }

            if (request.LeadId <= 0)
            {
                return BadRequest(new { Success = false, Message = "LeadId 无效" });
            }

            var result = await _leadAgentService.AnalyzeLeadAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "线索分析失败");
            return StatusCode(500, new
            {
                Success = false,
                Message = "线索分析失败",
                ErrorMessage = ex.Message
            });
        }
    }
}

