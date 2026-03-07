using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using System.Text.Json;

namespace PersonalSite.Api.Services;

/// <summary>情报来源服务实现</summary>
public class IntelligenceSourceService : IIntelligenceSourceService
{
    private readonly AppDbContext _context;

    public IntelligenceSourceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SourceResponseDto>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var sources = await _context.IntelligenceSources
            .OrderBy(s => s.Priority)
            .ThenByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return sources.Select(s => new SourceResponseDto
        {
            Id = s.Id,
            SourceName = s.SourceName,
            SourceType = s.SourceType,
            SourceUrl = s.SourceUrl,
            Category = s.Category,
            Tags = s.Tags,
            Priority = s.Priority,
            Enabled = s.Enabled,
            FetchIntervalMinutes = s.FetchIntervalMinutes,
            LastFetchTime = s.LastFetchTime,
            Remark = s.Remark
        }).ToList();
    }

    public async Task<SourceResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var source = await _context.IntelligenceSources
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (source == null)
            return null;

        return new SourceResponseDto
        {
            Id = source.Id,
            SourceName = source.SourceName,
            SourceType = source.SourceType,
            SourceUrl = source.SourceUrl,
            Category = source.Category,
            Tags = source.Tags,
            Priority = source.Priority,
            Enabled = source.Enabled,
            FetchIntervalMinutes = source.FetchIntervalMinutes,
            LastFetchTime = source.LastFetchTime,
            Remark = source.Remark
        };
    }

    public async Task<SourceResponseDto> CreateAsync(SourceRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new IntelligenceSource
        {
            SourceName = dto.SourceName,
            SourceType = dto.SourceType,
            SourceUrl = dto.SourceUrl,
            Category = dto.Category,
            TagsJson = dto.Tags.Count > 0 ? JsonSerializer.Serialize(dto.Tags) : null,
            Priority = dto.Priority,
            Enabled = dto.Enabled,
            FetchIntervalMinutes = dto.FetchIntervalMinutes,
            Remark = dto.Remark
        };

        _context.IntelligenceSources.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SourceResponseDto
        {
            Id = entity.Id,
            SourceName = entity.SourceName,
            SourceType = entity.SourceType,
            SourceUrl = entity.SourceUrl,
            Category = entity.Category,
            Tags = dto.Tags,
            Priority = entity.Priority,
            Enabled = entity.Enabled,
            FetchIntervalMinutes = entity.FetchIntervalMinutes,
            LastFetchTime = null,
            Remark = entity.Remark
        };
    }

    public async Task<SourceResponseDto?> UpdateAsync(int id, SourceRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.IntelligenceSources
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (entity == null)
            return null;

        entity.SourceName = dto.SourceName;
        entity.SourceType = dto.SourceType;
        entity.SourceUrl = dto.SourceUrl;
        entity.Category = dto.Category;
        entity.TagsJson = dto.Tags.Count > 0 ? JsonSerializer.Serialize(dto.Tags) : null;
        entity.Priority = dto.Priority;
        entity.Enabled = dto.Enabled;
        entity.FetchIntervalMinutes = dto.FetchIntervalMinutes;
        entity.Remark = dto.Remark;
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return new SourceResponseDto
        {
            Id = entity.Id,
            SourceName = entity.SourceName,
            SourceType = entity.SourceType,
            SourceUrl = entity.SourceUrl,
            Category = entity.Category,
            Tags = dto.Tags,
            Priority = entity.Priority,
            Enabled = entity.Enabled,
            FetchIntervalMinutes = entity.FetchIntervalMinutes,
            LastFetchTime = entity.LastFetchTime,
            Remark = entity.Remark
        };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.IntelligenceSources
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (entity == null)
            return false;

        _context.IntelligenceSources.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ToggleEnabledAsync(int id, bool enabled, CancellationToken cancellationToken = default)
    {
        var entity = await _context.IntelligenceSources
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (entity == null)
            return false;

        entity.Enabled = enabled;
        entity.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<IntelligenceSource>> GetEnabledSourcesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.IntelligenceSources
            .Where(s => s.Enabled)
            .OrderBy(s => s.Priority)
            .ToListAsync(cancellationToken);
    }
}
