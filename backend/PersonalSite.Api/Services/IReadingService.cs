using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>
/// 阅读 Inbox 服务
/// </summary>
public interface IReadingService
{
    Task<(bool Created, ReadingEntryDto Entry)> IngestMindtraceAsync(
        MindtraceIngestRequest request,
        CancellationToken cancellationToken = default);

    Task<List<ReadingEntryDto>> ListAsync(
        string? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>公开站：仅 published + public</summary>
    Task<List<ReadingEntryDto>> ListPublicAsync(
        CancellationToken cancellationToken = default);

    Task<ReadingEntryDto?> ApplyActionAsync(
        long id,
        string action,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
