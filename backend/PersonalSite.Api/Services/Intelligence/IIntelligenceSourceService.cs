using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>情报来源服务接口</summary>
public interface IIntelligenceSourceService
{
    Task<List<SourceResponseDto>> GetListAsync(CancellationToken cancellationToken = default);
    Task<SourceResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SourceResponseDto> CreateAsync(SourceRequestDto dto, CancellationToken cancellationToken = default);
    Task<SourceResponseDto?> UpdateAsync(int id, SourceRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ToggleEnabledAsync(int id, bool enabled, CancellationToken cancellationToken = default);
    Task<List<IntelligenceSource>> GetEnabledSourcesAsync(CancellationToken cancellationToken = default);
}
