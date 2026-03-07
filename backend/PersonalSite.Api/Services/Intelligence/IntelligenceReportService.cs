using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>情报报告服务实现</summary>
public class IntelligenceReportService : IIntelligenceReportService
{
    private readonly AppDbContext _context;

    public IntelligenceReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int Total, List<ReportResponseDto> List)> GetListAsync(int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _context.IntelligenceDailyReports.AsNoTracking();
        int total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.ReportDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var list = items.Select(r => new ReportResponseDto
        {
            Id = r.Id,
            ReportDate = r.ReportDate,
            Title = r.Title,
            ContentMarkdown = r.ContentMarkdown ?? "",
            ItemCount = r.ItemCount,
            GeneratedAt = r.GeneratedAt
        }).ToList();

        return (total, list);
    }

    public async Task<ReportResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var report = await _context.IntelligenceDailyReports
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (report == null)
            return null;

        return new ReportResponseDto
        {
            Id = report.Id,
            ReportDate = report.ReportDate,
            Title = report.Title,
            ContentMarkdown = report.ContentMarkdown ?? "",
            ItemCount = report.ItemCount,
            GeneratedAt = report.GeneratedAt
        };
    }

    public async Task<ReportResponseDto?> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        var report = await _context.IntelligenceDailyReports
            .AsNoTracking()
            .OrderByDescending(r => r.ReportDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (report == null)
            return null;

        return new ReportResponseDto
        {
            Id = report.Id,
            ReportDate = report.ReportDate,
            Title = report.Title,
            ContentMarkdown = report.ContentMarkdown ?? "",
            ItemCount = report.ItemCount,
            GeneratedAt = report.GeneratedAt
        };
    }
}
