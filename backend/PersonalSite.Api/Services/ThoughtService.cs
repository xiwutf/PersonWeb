using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>
/// 思维记录服务：CRUD + 请求 AI 批注
/// </summary>
public class ThoughtService : IThoughtService
{
    private const int SummaryMaxLength = 50;
    private readonly AppDbContext _context;
    private readonly AiServiceClient _aiClient;

    public ThoughtService(AppDbContext context, AiServiceClient aiClient)
    {
        _context = context;
        _aiClient = aiClient;
    }

    /// <inheritdoc />
    public async Task<(int Total, List<ThoughtListItemDto> List)> GetPageAsync(int page, int pageSize, string? keyword, CancellationToken cancellationToken = default)
    {
        var query = _context.ThoughtRecords.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(t => t.Content.Contains(keyword));
        }

        int total = await query.CountAsync(cancellationToken);

        // 先查 Id、Content、Status、CreatedAt，摘要在内存中截取，避免 EF 转 SQL 的兼容问题
        var rows = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new { t.Id, t.Content, t.Status, t.CreatedAt })
            .ToListAsync(cancellationToken);

        var list = rows.Select(t =>
        {
            string summary = string.IsNullOrEmpty(t.Content)
                ? ""
                : (t.Content.Length <= SummaryMaxLength ? t.Content : t.Content.Substring(0, SummaryMaxLength) + "…");
            return new ThoughtListItemDto
            {
                Id = t.Id,
                Summary = summary,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            };
        }).ToList();

        return (total, list);
    }

    /// <inheritdoc />
    public async Task<ThoughtDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ThoughtRecords.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return entity == null ? null : ToDto(entity);
    }

    /// <inheritdoc />
    public async Task<ThoughtDto> CreateAsync(string content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("内容不能为空", nameof(content));
        }

        var entity = new ThoughtRecord
        {
            Content = content.Trim(),
            Status = 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _context.ThoughtRecords.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    /// <inheritdoc />
    public async Task<ThoughtDto?> UpdateAsync(long id, string content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("内容不能为空", nameof(content));
        }

        var entity = await _context.ThoughtRecords.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity == null) return null;

        entity.Content = content.Trim();
        entity.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    /// <inheritdoc />
    public async Task<ThoughtDto?> GenerateAiCommentAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ThoughtRecords.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity == null) return null;

        string commentMd = await _aiClient.ThoughtCommentAsync(entity.Content, cancellationToken);

        entity.AiComment = commentMd;
        entity.Status = 1;
        entity.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    private static ThoughtDto ToDto(ThoughtRecord t)
    {
        return new ThoughtDto
        {
            Id = t.Id,
            Content = t.Content,
            AiComment = t.AiComment,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        };
    }
}
