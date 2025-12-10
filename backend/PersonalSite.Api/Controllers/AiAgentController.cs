using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Services;
using System;

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
    private readonly SupportAgentService _supportAgentService;
    private readonly PersonalAssistantService _personalAssistantService;
    private readonly QuotationAgentService _quotationAgentService;
    private readonly AppDbContext _dbContext;
    private readonly AiServiceClient _aiServiceClient;
    private readonly ILogger<AiAgentController> _logger;

    public AiAgentController(
        ContentAgentService contentAgentService,
        DemoAgentService demoAgentService,
        LeadAgentService leadAgentService,
        SupportAgentService supportAgentService,
        PersonalAssistantService personalAssistantService,
        QuotationAgentService quotationAgentService,
        AppDbContext dbContext,
        AiServiceClient aiServiceClient,
        ILogger<AiAgentController> logger)
    {
        _contentAgentService = contentAgentService;
        _demoAgentService = demoAgentService;
        _leadAgentService = leadAgentService;
        _supportAgentService = supportAgentService;
        _personalAssistantService = personalAssistantService;
        _quotationAgentService = quotationAgentService;
        _dbContext = dbContext;
        _aiServiceClient = aiServiceClient;
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

    /// <summary>
    /// AI 服务健康检查接口
    /// GET /api/ai/health
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous] // 允许匿名访问，用于状态检查
    public async Task<IActionResult> HealthCheck()
    {
        try
        {
            // 调用 AiServiceClient 的健康检查
            var isHealthy = await _aiServiceClient.HealthCheckAsync();
            
            return Ok(new
            {
                Success = isHealthy,
                Status = isHealthy ? "running" : "unavailable",
                Message = isHealthy ? "AI 服务运行正常" : "AI 服务不可用",
                Timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI 服务健康检查失败");
            return Ok(new
            {
                Success = false,
                Status = "error",
                Message = $"健康检查失败: {ex.Message}",
                Timestamp = DateTime.Now
            });
        }
    }

    /// <summary>
    /// 获取 AI 智能体调用日志
    /// GET /api/ai/logs
    /// </summary>
    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs(
        [FromQuery] string? agentType = null,
        [FromQuery] bool? success = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = _dbContext.AiAgentLogs.AsQueryable();

            // 筛选智能体类型
            if (!string.IsNullOrEmpty(agentType))
            {
                query = query.Where(l => l.AgentType == agentType);
            }

            // 筛选成功/失败
            if (success.HasValue)
            {
                query = query.Where(l => l.Success == success.Value);
            }

            // 总数
            var total = await query.CountAsync();

            // 分页查询
            var logs = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                List = logs,
                Total = total,
                Page = page,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取 AI 智能体日志失败");
            return StatusCode(500, new
            {
                Success = false,
                Message = "获取日志失败",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// 客服问答接口
    /// POST /api/ai/support/answer
    /// </summary>
    [HttpPost("support/answer")]
    [AllowAnonymous] // 允许匿名访问，前台访客可以使用
    public async Task<IActionResult> SupportAnswer([FromBody] SupportAnswerRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.Question))
            {
                return BadRequest(new { Success = false, Message = "问题不能为空" });
            }

            var result = await _supportAgentService.AnswerQuestionAsync(request);

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
            _logger.LogError(ex, "客服问答失败");
            return StatusCode(500, new
            {
                Success = false,
                Message = "客服问答失败",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// 个人助理对话接口
    /// POST /api/ai/assistant/chat
    /// </summary>
    [HttpPost("assistant/chat")]
    public async Task<IActionResult> AssistantChat([FromBody] AssistantChatRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(new { Success = false, Message = "消息不能为空" });
            }

            // 从当前登录用户获取 UserId（这里简化处理，实际应从 JWT Token 中获取）
            // TODO: 从 HttpContext.User 获取当前用户 ID
            if (request.UserId <= 0)
            {
                return BadRequest(new { Success = false, Message = "用户 ID 无效" });
            }

            var result = await _personalAssistantService.ChatAsync(request);

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
            _logger.LogError(ex, "个人助理对话失败");
            return StatusCode(500, new
            {
                Success = false,
                Message = "个人助理对话失败",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// 获取个人助理会话列表
    /// GET /api/ai/assistant/sessions
    /// </summary>
    [HttpGet("assistant/sessions")]
    public async Task<IActionResult> GetAssistantSessions([FromQuery] long userId)
    {
        try
        {
            var sessions = await _dbContext.AssistantSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.UpdatedAt)
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.CreatedAt,
                    s.UpdatedAt,
                    MessageCount = s.Messages.Count
                })
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Sessions = sessions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取会话列表失败");
            return StatusCode(500, new { Success = false, Message = "获取会话列表失败" });
        }
    }

    /// <summary>
    /// 获取会话消息列表
    /// GET /api/ai/assistant/sessions/{sessionId}/messages
    /// </summary>
    [HttpGet("assistant/sessions/{sessionId}/messages")]
    public async Task<IActionResult> GetSessionMessages(long sessionId, [FromQuery] long userId)
    {
        try
        {
            var session = await _dbContext.AssistantSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

            if (session == null)
            {
                return NotFound(new { Success = false, Message = "会话不存在" });
            }

            var messages = await _dbContext.AssistantMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new
                {
                    m.Id,
                    m.Role,
                    m.Content,
                    m.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Messages = messages
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取消息列表失败");
            return StatusCode(500, new { Success = false, Message = "获取消息列表失败" });
        }
    }

    /// <summary>
    /// 自动报价接口
    /// POST /api/ai/quotation/generate
    /// </summary>
    [HttpPost("quotation/generate")]
    public async Task<IActionResult> GenerateQuotation([FromBody] QuotationRequest request)
    {
        try
        {
            if (request == null || request.LeadId <= 0)
            {
                return BadRequest(new { Success = false, Message = "LeadId 无效" });
            }

            var result = await _quotationAgentService.GenerateQuotationAsync(request);

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
            _logger.LogError(ex, "自动报价失败");
            return StatusCode(500, new
            {
                Success = false,
                Message = "自动报价失败",
                ErrorMessage = ex.Message
            });
        }
    }
}

