using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>
/// 智能取名助手 AI 服务接口
/// </summary>
public interface INameToolAiService
{
    /// <summary>
    /// 生成名字
    /// </summary>
    Task<NameGenerateResponseDto> GenerateAsync(
        NameGenerateRequestDto request,
        string? traceId = null,
        CancellationToken cancellationToken = default);
}

