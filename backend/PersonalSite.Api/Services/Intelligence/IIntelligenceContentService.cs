using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>情报内容服务接口</summary>
public interface IIntelligenceContentService
{
    Task<(int Total, List<ContentItemDto> List)> GetPageAsync(ContentQueryDto dto, CancellationToken cancellationToken = default);
    Task<ContentItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<CategoryStatDto>> GetCategoryStatsAsync(CancellationToken cancellationToken = default);
    Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}
