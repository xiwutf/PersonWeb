using System.Text.Json;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services;

/// <summary>
/// AI 智能体服务基类
/// 提供统一的 AI 调用和日志记录功能
/// </summary>
public abstract class AiAgentService : IAiAgentService
{
    protected readonly AiServiceClient AiClient;
    protected readonly AppDbContext DbContext;
    protected readonly ILogger<AiAgentService> Logger;

    protected AiAgentService(
        AiServiceClient aiClient,
        AppDbContext dbContext,
        ILogger<AiAgentService> logger)
    {
        AiClient = aiClient;
        DbContext = dbContext;
        Logger = logger;
    }

    /// <summary>
    /// 调用 AI 服务并记录日志
    /// </summary>
    public async Task<string> CallAiAsync(
        string agentType,
        string prompt,
        Dictionary<string, object>? meta = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AiAgentLog
        {
            AgentType = agentType,
            RequestPayload = JsonSerializer.Serialize(new
            {
                Prompt = prompt,
                Meta = meta
            }),
            CreatedAt = DateTime.Now
        };

        try
        {
            Logger.LogInformation("调用 AI 智能体: AgentType={AgentType}, PromptLength={Length}",
                agentType, prompt.Length);

            // 调用 AI 服务
            var chatRequest = new ChatRequestDto
            {
                UserId = "system",
                Message = prompt,
                Meta = meta
            };

            var response = await AiClient.ChatAsync(chatRequest, cancellationToken);
            var reply = response.Reply;

            // 记录成功日志
            log.Success = true;
            log.ResponsePayload = JsonSerializer.Serialize(new
            {
                Reply = reply,
                Model = response.Model,
                Usage = response.Usage
            });

            Logger.LogInformation("AI 智能体调用成功: AgentType={AgentType}, ReplyLength={Length}",
                agentType, reply.Length);

            return reply;
        }
        catch (Exception ex)
        {
            // 记录失败日志
            log.Success = false;
            log.ErrorMessage = ex.Message;
            log.ResponsePayload = JsonSerializer.Serialize(new
            {
                Error = ex.Message,
                StackTrace = ex.StackTrace
            });

            Logger.LogError(ex, "AI 智能体调用失败: AgentType={AgentType}", agentType);

            throw;
        }
        finally
        {
            // 保存日志到数据库（不阻塞主流程，失败不影响业务）
            try
            {
                DbContext.AiAgentLogs.Add(log);
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // 日志记录失败不应该影响主流程
                Logger.LogWarning(ex, "保存 AI 智能体日志失败: AgentType={AgentType}, Error={Error}", 
                    agentType, ex.Message);
            }
        }
    }
}

