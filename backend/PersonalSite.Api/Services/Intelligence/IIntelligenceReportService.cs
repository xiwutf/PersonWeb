using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>情报报告服务接口</summary>
public interface IIntelligenceReportService
{
    Task<(int Total, List<ReportResponseDto> List)> GetListAsync(int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<ReportResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ReportResponseDto?> GetLatestAsync(CancellationToken cancellationToken = default);
}
