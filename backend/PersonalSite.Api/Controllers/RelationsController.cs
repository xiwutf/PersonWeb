using System.Text.Json;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 关系跟进控制器
/// </summary>
[ApiController]
[Route("api/relations")]
[Authorize]
public class RelationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<RelationsController> _logger;
    private readonly HttpClient _httpClient;
    private readonly AiServiceOptions _aiServiceOptions;
    private readonly ObservationPeriodService _observationService;

    public RelationsController(
        AppDbContext context, 
        ILogger<RelationsController> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<AiServiceOptions> aiServiceOptions,
        ObservationPeriodService observationService)
    {
        _context = context;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _aiServiceOptions = aiServiceOptions.Value;
        _observationService = observationService;
        
        // 配置 HttpClient
        _httpClient.BaseAddress = new Uri(_aiServiceOptions.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_aiServiceOptions.TimeoutSeconds);
    }

    /// <summary>
    /// 获取关系对象列表（支持搜索、筛选、排序）
    /// </summary>
    [HttpGet("persons")]
    public async Task<ActionResult<ApiResponse<object>>> GetPersons(
        [FromQuery] int? stage = null,
        [FromQuery] string? q = null,
        [FromQuery] string? tag = null,
        [FromQuery] string? sort = "lastContact") // lastContact/heat/recent
    {
        var query = _context.RelationPersons.AsQueryable();

        // 阶段筛选
        if (stage.HasValue)
        {
            query = query.Where(p => p.Stage == stage.Value);
        }

        // 关键词搜索
        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(p => p.Nickname.Contains(q) || 
                (p.Source != null && p.Source.Contains(q)) ||
                (p.Notes != null && p.Notes.Contains(q)));
        }

        // 标签筛选
        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(p => p.Tags != null && p.Tags.Contains(tag));
        }

        // 排序
        switch (sort)
        {
            case "heat":
                query = query.OrderByDescending(p => p.HeatScore);
                break;
            case "recent":
                query = query.OrderByDescending(p => p.RemindAt ?? DateTime.MinValue);
                break;
            case "lastContact":
            default:
                // 最久未联系：LastContactAt 为 null 或最早的在前面
                query = query.OrderBy(p => p.LastContactAt ?? DateTime.MinValue);
                break;
        }

        var items = await query
            .Select(p => new RelationPersonListItemDto
            {
                Id = p.Id,
                Nickname = p.Nickname,
                Source = p.Source,
                City = p.City,
                Tags = ParseJsonArray<string>(p.Tags) ?? new List<string>(),
                Stage = p.Stage,
                HeatScore = p.HeatScore,
                LastContactAt = p.LastContactAt,
                LastMeetAt = p.LastMeetAt,
                NextAction = p.NextAction,
                RemindAt = p.RemindAt,
                ObservationStartedAt = p.ObservationStartedAt,
                ObservationExpectedEndAt = p.ObservationExpectedEndAt,
                ObservationLastRemindedAt = p.ObservationLastRemindedAt,
                ObservationReason = p.ObservationReason,
                ObservationDecisionPending = p.ObservationDecisionPending,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(items));
    }

    /// <summary>
    /// 获取关系对象详情
    /// </summary>
    [HttpGet("persons/{id}")]
    public async Task<ActionResult<ApiResponse<RelationPersonDetailDto>>> GetPerson(Guid id)
    {
        var person = await _context.RelationPersons.FindAsync(id);
        if (person == null)
        {
            return Ok(ApiResponse<RelationPersonDetailDto>.Error("对象不存在", 404));
        }

        var dto = new RelationPersonDetailDto
        {
            Id = person.Id,
            Nickname = person.Nickname,
            Source = person.Source,
            City = person.City,
            Tags = ParseJsonArray<string>(person.Tags) ?? new List<string>(),
            Preferences = person.Preferences,
            Stage = person.Stage,
            HeatScore = person.HeatScore,
            LastContactAt = person.LastContactAt,
            LastMeetAt = person.LastMeetAt,
            NextAction = person.NextAction,
            RemindAt = person.RemindAt,
            Notes = person.Notes,
            ObservationStartedAt = person.ObservationStartedAt,
            ObservationExpectedEndAt = person.ObservationExpectedEndAt,
            ObservationLastRemindedAt = person.ObservationLastRemindedAt,
            ObservationReason = person.ObservationReason,
            ObservationDecisionPending = person.ObservationDecisionPending,
            CreatedAt = person.CreatedAt,
            UpdatedAt = person.UpdatedAt
        };

        return Ok(ApiResponse<RelationPersonDetailDto>.Success(dto));
    }

    /// <summary>
    /// 创建关系对象
    /// </summary>
    [HttpPost("persons")]
    public async Task<ActionResult<ApiResponse<RelationPersonDetailDto>>> CreatePerson([FromBody] CreateRelationPersonDto dto)
    {
        var person = new RelationPerson
        {
            Id = Guid.NewGuid(),
            Nickname = dto.Nickname,
            Source = dto.Source,
            City = dto.City,
            Tags = dto.Tags != null ? JsonSerializer.Serialize(dto.Tags) : null,
            Preferences = dto.Preferences,
            Stage = dto.Stage,
            Notes = dto.Notes,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.RelationPersons.Add(person);
        await _context.SaveChangesAsync();

        var result = new RelationPersonDetailDto
        {
            Id = person.Id,
            Nickname = person.Nickname,
            Source = person.Source,
            City = person.City,
            Tags = ParseJsonArray<string>(person.Tags) ?? new List<string>(),
            Preferences = person.Preferences,
            Stage = person.Stage,
            HeatScore = person.HeatScore,
            LastContactAt = person.LastContactAt,
            LastMeetAt = person.LastMeetAt,
            NextAction = person.NextAction,
            RemindAt = person.RemindAt,
            Notes = person.Notes,
            CreatedAt = person.CreatedAt,
            UpdatedAt = person.UpdatedAt
        };

        return Ok(ApiResponse<RelationPersonDetailDto>.Success(result));
    }

    /// <summary>
    /// 更新关系对象
    /// </summary>
    [HttpPut("persons/{id}")]
    public async Task<ActionResult<ApiResponse<RelationPersonDetailDto>>> UpdatePerson(Guid id, [FromBody] UpdateRelationPersonDto dto)
    {
        var person = await _context.RelationPersons.FindAsync(id);
        if (person == null)
        {
            return Ok(ApiResponse<RelationPersonDetailDto>.Error("对象不存在", 404));
        }

        if (!string.IsNullOrEmpty(dto.Nickname))
            person.Nickname = dto.Nickname;
        if (dto.Source != null)
            person.Source = dto.Source;
        if (dto.City != null)
            person.City = dto.City;
        if (dto.Tags != null)
            person.Tags = JsonSerializer.Serialize(dto.Tags);
        if (dto.Preferences != null)
            person.Preferences = dto.Preferences;
        if (dto.Stage.HasValue)
            person.Stage = dto.Stage.Value;
        if (dto.HeatScore.HasValue)
            person.HeatScore = dto.HeatScore.Value;
        if (dto.NextAction != null)
            person.NextAction = dto.NextAction;
        if (dto.RemindAt.HasValue)
            person.RemindAt = dto.RemindAt;
        if (dto.Notes != null)
            person.Notes = dto.Notes;

        person.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        var result = new RelationPersonDetailDto
        {
            Id = person.Id,
            Nickname = person.Nickname,
            Source = person.Source,
            City = person.City,
            Tags = ParseJsonArray<string>(person.Tags) ?? new List<string>(),
            Preferences = person.Preferences,
            Stage = person.Stage,
            HeatScore = person.HeatScore,
            LastContactAt = person.LastContactAt,
            LastMeetAt = person.LastMeetAt,
            NextAction = person.NextAction,
            RemindAt = person.RemindAt,
            Notes = person.Notes,
            CreatedAt = person.CreatedAt,
            UpdatedAt = person.UpdatedAt
        };

        return Ok(ApiResponse<RelationPersonDetailDto>.Success(result));
    }

    /// <summary>
    /// 删除关系对象
    /// </summary>
    [HttpDelete("persons/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePerson(Guid id)
    {
        var person = await _context.RelationPersons.FindAsync(id);
        if (person == null)
        {
            return Ok(ApiResponse.Error("对象不存在", 404));
        }

        _context.RelationPersons.Remove(person);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null));
    }

    /// <summary>
    /// 获取对象的互动记录列表
    /// </summary>
    [HttpGet("persons/{personId}/interactions")]
    public async Task<ActionResult<ApiResponse<List<RelationInteractionDto>>>> GetInteractions(Guid personId)
    {
        var interactions = await _context.RelationInteractions
            .Where(i => i.PersonId == personId)
            .OrderByDescending(i => i.OccurredAt)
            .Select(i => new RelationInteractionDto
            {
                Id = i.Id,
                PersonId = i.PersonId,
                Type = i.Type,
                OccurredAt = i.OccurredAt,
                Summary = i.Summary,
                KeyPoints = ParseJsonObject(i.KeyPoints),
                MyFeeling = i.MyFeeling,
                AiSuggestion = i.AiSuggestion,
                CreatedAt = i.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<RelationInteractionDto>>.Success(interactions));
    }

    /// <summary>
    /// 创建互动记录
    /// </summary>
    [HttpPost("persons/{personId}/interactions")]
    public async Task<ActionResult<ApiResponse<RelationInteractionDto>>> CreateInteraction(
        Guid personId, 
        [FromBody] CreateRelationInteractionDto dto)
    {
        var person = await _context.RelationPersons.FindAsync(personId);
        if (person == null)
        {
            return Ok(ApiResponse<RelationInteractionDto>.Error("对象不存在", 404));
        }

        var interaction = new RelationInteraction
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Type = dto.Type,
            OccurredAt = dto.OccurredAt,
            Summary = dto.Summary,
            KeyPoints = dto.KeyPoints != null ? JsonSerializer.Serialize(dto.KeyPoints) : null,
            MyFeeling = dto.MyFeeling,
            CreatedAt = DateTime.Now
        };

        // 更新对象的最后联系时间
        person.LastContactAt = dto.OccurredAt;
        
        // 如果是见面类型，更新最后见面时间
        if (dto.Type == 3) // 见面
        {
            person.LastMeetAt = dto.OccurredAt;
        }

        person.UpdatedAt = DateTime.Now;

        _context.RelationInteractions.Add(interaction);
        await _context.SaveChangesAsync();

        var result = new RelationInteractionDto
        {
            Id = interaction.Id,
            PersonId = interaction.PersonId,
            Type = interaction.Type,
            OccurredAt = interaction.OccurredAt,
            Summary = interaction.Summary,
            KeyPoints = ParseJsonObject(interaction.KeyPoints),
            MyFeeling = interaction.MyFeeling,
            AiSuggestion = interaction.AiSuggestion,
            CreatedAt = interaction.CreatedAt
        };

        return Ok(ApiResponse<RelationInteractionDto>.Success(result));
    }

    /// <summary>
    /// 更新互动记录
    /// </summary>
    [HttpPut("interactions/{id}")]
    public async Task<ActionResult<ApiResponse<RelationInteractionDto>>> UpdateInteraction(
        Guid id, 
        [FromBody] UpdateRelationInteractionDto dto)
    {
        var interaction = await _context.RelationInteractions.FindAsync(id);
        if (interaction == null)
        {
            return Ok(ApiResponse<RelationInteractionDto>.Error("互动记录不存在", 404));
        }

        if (dto.Type.HasValue)
            interaction.Type = dto.Type.Value;
        if (dto.OccurredAt.HasValue)
            interaction.OccurredAt = dto.OccurredAt.Value;
        if (!string.IsNullOrEmpty(dto.Summary))
            interaction.Summary = dto.Summary;
        if (dto.KeyPoints != null)
            interaction.KeyPoints = JsonSerializer.Serialize(dto.KeyPoints);
        if (dto.MyFeeling.HasValue)
            interaction.MyFeeling = dto.MyFeeling;
        if (dto.AiSuggestion != null)
            interaction.AiSuggestion = dto.AiSuggestion;

        await _context.SaveChangesAsync();

        var result = new RelationInteractionDto
        {
            Id = interaction.Id,
            PersonId = interaction.PersonId,
            Type = interaction.Type,
            OccurredAt = interaction.OccurredAt,
            Summary = interaction.Summary,
            KeyPoints = ParseJsonObject(interaction.KeyPoints),
            MyFeeling = interaction.MyFeeling,
            AiSuggestion = interaction.AiSuggestion,
            CreatedAt = interaction.CreatedAt
        };

        return Ok(ApiResponse<RelationInteractionDto>.Success(result));
    }

    /// <summary>
    /// 删除互动记录
    /// </summary>
    [HttpDelete("interactions/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteInteraction(Guid id)
    {
        var interaction = await _context.RelationInteractions.FindAsync(id);
        if (interaction == null)
        {
            return Ok(ApiResponse.Error("互动记录不存在", 404));
        }

        _context.RelationInteractions.Remove(interaction);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null));
    }

    /// <summary>
    /// 获取对象的任务列表
    /// </summary>
    [HttpGet("persons/{personId}/tasks")]
    public async Task<ActionResult<ApiResponse<List<RelationTaskDto>>>> GetTasks(Guid personId)
    {
        var tasks = await _context.RelationTasks
            .Where(t => t.PersonId == personId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new RelationTaskDto
            {
                Id = t.Id,
                PersonId = t.PersonId,
                Title = t.Title,
                DueAt = t.DueAt,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<RelationTaskDto>>.Success(tasks));
    }

    /// <summary>
    /// 创建任务
    /// </summary>
    [HttpPost("persons/{personId}/tasks")]
    public async Task<ActionResult<ApiResponse<RelationTaskDto>>> CreateTask(
        Guid personId, 
        [FromBody] CreateRelationTaskDto dto)
    {
        var person = await _context.RelationPersons.FindAsync(personId);
        if (person == null)
        {
            return Ok(ApiResponse<RelationTaskDto>.Error("对象不存在", 404));
        }

        var task = new RelationTask
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Title = dto.Title,
            DueAt = dto.DueAt,
            Priority = dto.Priority,
            Status = 0, // todo
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.RelationTasks.Add(task);
        await _context.SaveChangesAsync();

        var result = new RelationTaskDto
        {
            Id = task.Id,
            PersonId = task.PersonId,
            Title = task.Title,
            DueAt = task.DueAt,
            Priority = task.Priority,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return Ok(ApiResponse<RelationTaskDto>.Success(result));
    }

    /// <summary>
    /// 更新任务
    /// </summary>
    [HttpPut("tasks/{id}")]
    public async Task<ActionResult<ApiResponse<RelationTaskDto>>> UpdateTask(
        Guid id, 
        [FromBody] UpdateRelationTaskDto dto)
    {
        var task = await _context.RelationTasks.FindAsync(id);
        if (task == null)
        {
            return Ok(ApiResponse<RelationTaskDto>.Error("任务不存在", 404));
        }

        if (!string.IsNullOrEmpty(dto.Title))
            task.Title = dto.Title;
        if (dto.DueAt.HasValue)
            task.DueAt = dto.DueAt;
        if (dto.Priority.HasValue)
            task.Priority = dto.Priority.Value;
        if (dto.Status.HasValue)
            task.Status = dto.Status.Value;

        task.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        var result = new RelationTaskDto
        {
            Id = task.Id,
            PersonId = task.PersonId,
            Title = task.Title,
            DueAt = task.DueAt,
            Priority = task.Priority,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return Ok(ApiResponse<RelationTaskDto>.Success(result));
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    [HttpDelete("tasks/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTask(Guid id)
    {
        var task = await _context.RelationTasks.FindAsync(id);
        if (task == null)
        {
            return Ok(ApiResponse.Error("任务不存在", 404));
        }

        _context.RelationTasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null));
    }

    /// <summary>
    /// AI 生成互动总结和建议
    /// 调用 Python FastAPI AI 服务
    /// </summary>
    [HttpPost("ai/summarize")]
    public async Task<ActionResult<ApiResponse<RelationAiSummarizeResponse>>> AiSummarize(
        [FromBody] RelationAiSummarizeRequest request)
    {
        try
        {
            _logger.LogInformation(
                "收到 AI 总结请求: PersonId={PersonId}, SummaryLength={Length}",
                request.Person.Nickname, request.Interaction.Summary?.Length ?? 0);

            // 如果请求中没有 Person 信息，需要从数据库查询
            if (request.Person.Nickname == null || string.IsNullOrEmpty(request.Interaction.Summary))
            {
                return Ok(ApiResponse<RelationAiSummarizeResponse>.Error("对象昵称和互动摘要不能为空", 400));
            }

            // 构建 Python AI 服务请求
            var pythonRequest = new
            {
                person = new
                {
                    nickname = request.Person.Nickname,
                    stage = request.Person.Stage,
                    tags = request.Person.Tags ?? new List<string>(),
                    last_contact_at = request.Person.LastContactAt,
                    last_meet_at = request.Person.LastMeetAt,
                    current_next_action = request.Person.CurrentNextAction
                },
                history_key_points = request.HistoryKeyPoints,
                interaction = new
                {
                    type = request.Interaction.Type,
                    occurred_at = request.Interaction.OccurredAt,
                    summary = request.Interaction.Summary,
                    chat_text = request.Interaction.ChatText
                },
                user_preference = request.UserPreference != null ? new
                {
                    user_goal = request.UserPreference.UserGoal,
                    user_style = request.UserPreference.UserStyle,
                    time_constraints = request.UserPreference.TimeConstraints
                } : null
            };

            // 调用 Python AI 服务
            var internalToken = _aiServiceOptions.InternalToken ?? "default-internal-token-change-in-production";
            var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? _aiServiceOptions.BaseUrl.TrimEnd('/');
            var requestUri = $"{baseAddress}/relation/summarize";

            _logger.LogDebug("调用 Python AI 服务: URL={Url}", requestUri);

            var requestMessage = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(pythonRequest)
            };
            requestMessage.Headers.Add("X-Internal-Token", internalToken);

            var response = await _httpClient.SendAsync(requestMessage);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Python AI 服务返回错误: StatusCode={StatusCode}, Response={Response}",
                    response.StatusCode, errorContent);
                return Ok(ApiResponse<RelationAiSummarizeResponse>.Error(
                    $"AI 服务返回错误: {response.StatusCode}", 500));
            }

            // 解析 Python 服务返回的响应
            // Python 返回的是 snake_case，需要配置 JsonSerializerOptions 进行映射
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // 忽略大小写
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase // 允许 camelCase 映射
            };
            var result = await response.Content.ReadFromJsonAsync<Services.AiServiceResponse<RelationAiSummarizeResponse>>(jsonOptions);

            if (result == null || !result.Success)
            {
                _logger.LogError("Python AI 服务返回错误: {ErrorCode} - {Message}",
                    result?.ErrorCode, result?.Message);
                return Ok(ApiResponse<RelationAiSummarizeResponse>.Error(
                    $"AI 服务返回错误: {result?.Message ?? "未知错误"}", 500));
            }

            if (result.Data == null)
            {
                _logger.LogError("Python AI 服务返回数据为空");
                return Ok(ApiResponse<RelationAiSummarizeResponse>.Error("AI 服务返回数据为空", 500));
            }

            _logger.LogInformation("Python AI 服务调用成功");

            return Ok(ApiResponse<RelationAiSummarizeResponse>.Success(result.Data));
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "调用 Python AI 服务失败（网络错误）");
            return Ok(ApiResponse<RelationAiSummarizeResponse>.Error(
                "AI 服务暂时不可用，请稍后重试", 500));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI 总结接口异常");
            return Ok(ApiResponse<RelationAiSummarizeResponse>.Error(
                $"AI 服务错误: {ex.Message}", 500));
        }
    }

    // 辅助方法：解析 JSON 数组
    private static List<T>? ParseJsonArray<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<List<T>>(json);
        }
        catch
        {
            return null;
        }
    }

    // 辅助方法：解析 JSON 对象
    private static Dictionary<string, object>? ParseJsonObject(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
        catch
        {
            return null;
        }
    }

    // ========== 观察期相关 API ==========

    /// <summary>
    /// 检查是否应该建议进入观察期
    /// </summary>
    [HttpGet("persons/{id}/observation/suggestion")]
    public async Task<ActionResult<ApiResponse<object>>> GetObservationSuggestion(Guid id)
    {
        var userId = GetUserId();
        var suggestion = await _observationService.CheckObservationSuggestionAsync(userId, id);
        
        if (suggestion == null)
        {
            return Ok(ApiResponse.Success(null, "无需进入观察期"));
        }

        return Ok(ApiResponse.Success(suggestion));
    }

    /// <summary>
    /// 开始观察期
    /// </summary>
    [HttpPost("persons/{id}/observation/start")]
    public async Task<ActionResult<ApiResponse<object>>> StartObservation(
        Guid id,
        [FromBody] StartObservationRequest request)
    {
        var userId = GetUserId();
        var success = await _observationService.StartObservationPeriodAsync(
            userId, 
            id, 
            request.Reason, 
            request.DurationDays);

        if (!success)
        {
            return Ok(ApiResponse<object>.Error("无法开始观察期", 400));
        }

        // 返回更新后的对象信息
        var person = await _context.RelationPersons.FindAsync(id);
        return Ok(ApiResponse.Success(new { message = "已进入观察期" }));
    }

    /// <summary>
    /// 获取观察期提醒列表
    /// </summary>
    [HttpGet("observation/reminders")]
    public async Task<ActionResult<ApiResponse<object>>> GetObservationReminders()
    {
        var userId = GetUserId();
        
        // 先标记需要决策的观察期
        await _observationService.MarkDecisionPendingAsync(userId);
        
        // 获取提醒列表
        var reminders = await _observationService.GetObservationRemindersAsync(userId);
        
        return Ok(ApiResponse.Success(reminders));
    }

    /// <summary>
    /// 标记观察期提醒已查看
    /// </summary>
    [HttpPost("persons/{id}/observation/reminder/viewed")]
    public async Task<ActionResult<ApiResponse<object>>> MarkReminderViewed(Guid id)
    {
        var userId = GetUserId();
        await _observationService.MarkReminderViewedAsync(userId, id);
        return Ok(ApiResponse.Success(new { message = "已标记为已查看" }));
    }

    /// <summary>
    /// 处理观察期结束决策
    /// </summary>
    [HttpPost("persons/{id}/observation/decision")]
    public async Task<ActionResult<ApiResponse<object>>> HandleObservationDecision(
        Guid id,
        [FromBody] ObservationDecisionRequest request)
    {
        var userId = GetUserId();
        
        if (!Enum.TryParse<ObservationDecision>(request.Decision, true, out var decision))
        {
            return Ok(ApiResponse<object>.Error("无效的决策类型", 400));
        }

        var success = await _observationService.HandleObservationDecisionAsync(
            userId, 
            id, 
            decision, 
            request.Reason);

        if (!success)
        {
            return Ok(ApiResponse<object>.Error("无法处理决策", 400));
        }

        return Ok(ApiResponse.Success(new { message = "决策已保存" }));
    }

    /// <summary>
    /// 获取用户ID（辅助方法）
    /// </summary>
    private string? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim?.Value;
    }
}

/// <summary>
/// 开始观察期请求
/// </summary>
public class StartObservationRequest
{
    public string? Reason { get; set; }
    public int? DurationDays { get; set; }
}

/// <summary>
/// 观察期决策请求
/// </summary>
public class ObservationDecisionRequest
{
    public string Decision { get; set; } = string.Empty; // "Continue", "Downgrade", "End"
    public string? Reason { get; set; }
}

