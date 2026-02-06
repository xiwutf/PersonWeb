using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>
/// 思维记录服务接口
/// </summary>
public interface IThoughtService
{
    /// <summary>分页查询，支持关键词</summary>
    Task<(int Total, List<ThoughtListItemDto> List)> GetPageAsync(int page, int pageSize, string? keyword, CancellationToken cancellationToken = default);

    /// <summary>按 ID 获取详情</summary>
    Task<ThoughtDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>创建一条记录</summary>
    Task<ThoughtDto> CreateAsync(string content, CancellationToken cancellationToken = default);

    /// <summary>更新原文</summary>
    Task<ThoughtDto?> UpdateAsync(long id, string content, CancellationToken cancellationToken = default);

    /// <summary>请求 AI 批注并写回，返回更新后的 DTO</summary>
    Task<ThoughtDto?> GenerateAiCommentAsync(long id, CancellationToken cancellationToken = default);
}
