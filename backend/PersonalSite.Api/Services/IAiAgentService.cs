namespace PersonalSite.Api.Services;

/// <summary>
/// AI 智能体服务接口
/// 所有智能体服务都应实现此接口，提供统一的调用和日志记录能力
/// </summary>
public interface IAiAgentService
{
    /// <summary>
    /// 调用 AI 服务并记录日志
    /// </summary>
    /// <param name="agentType">智能体类型（Content/Demo/Lead）</param>
    /// <param name="prompt">提示词</param>
    /// <param name="meta">元数据（可选）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>AI 返回的文本内容</returns>
    Task<string> CallAiAsync(
        string agentType,
        string prompt,
        Dictionary<string, object>? meta = null,
        CancellationToken cancellationToken = default);
}

