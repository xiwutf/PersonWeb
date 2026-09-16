using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>
/// 阅读 Inbox：ingest 幂等写入 + 管理端 CRUD
/// </summary>
public class ReadingService : IReadingService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly AppDbContext _context;

    public ReadingService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<(bool Created, ReadingEntryDto Entry)> IngestMindtraceAsync(
        MindtraceIngestRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentException("payload required");
        }

        string externalId = (request.ExternalId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(externalId))
        {
            throw new ArgumentException("externalId required");
        }

        string quote = request.SelectedText ?? string.Empty;
        string note = request.Note ?? string.Empty;
        if (string.IsNullOrWhiteSpace(quote) && string.IsNullOrWhiteSpace(note))
        {
            throw new ArgumentException("selectedText or note required");
        }

        const string sourceType = "mindtrace";
        List<string> tags = NormalizeTags(request.Tags);
        DateTime? sourceCreatedAt = ParseOptionalDate(request.CreatedAt)
            ?? ParseOptionalDate(request.UpdatedAt);
        DateTime now = DateTime.Now;

        ReadingEntry? existing = await _context.ReadingEntries
            .FirstOrDefaultAsync(
                e => e.SourceType == sourceType && e.ExternalId == externalId,
                cancellationToken);

        if (existing == null)
        {
            var entity = new ReadingEntry
            {
                SourceType = sourceType,
                ExternalId = externalId,
                Quote = quote,
                Note = note,
                SourceTitle = request.PageTitle ?? string.Empty,
                SourceUrl = request.PageUrl ?? string.Empty,
                SourceCreatedAt = sourceCreatedAt,
                ImportedAt = now,
                UpdatedAt = now,
                Tags = SerializeTags(tags),
                Status = "inbox",
                Visibility = "private",
            };
            _context.ReadingEntries.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return (true, ToDto(entity));
        }

        // 幂等更新：不重置 Status / Visibility / ImportedAt
        existing.Quote = quote;
        existing.Note = note;
        existing.SourceTitle = request.PageTitle ?? string.Empty;
        existing.SourceUrl = request.PageUrl ?? string.Empty;
        existing.SourceCreatedAt = sourceCreatedAt;
        existing.Tags = SerializeTags(tags);
        existing.UpdatedAt = now;
        await _context.SaveChangesAsync(cancellationToken);
        return (false, ToDto(existing));
    }

    /// <inheritdoc />
    public async Task<List<ReadingEntryDto>> ListAsync(
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ReadingEntry> query = _context.ReadingEntries.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(status))
        {
            string normalized = status.Trim().ToLowerInvariant();
            query = query.Where(e => e.Status == normalized);
        }

        List<ReadingEntry> rows = await query
            .OrderByDescending(e => e.ImportedAt)
            .ThenByDescending(e => e.Id)
            .ToListAsync(cancellationToken);

        return rows.Select(ToDto).ToList();
    }

    /// <inheritdoc />
    public async Task<List<ReadingEntryDto>> ListPublicAsync(
        CancellationToken cancellationToken = default)
    {
        List<ReadingEntry> rows = await _context.ReadingEntries
            .AsNoTracking()
            .Where(e => e.Status == "published" && e.Visibility == "public")
            .OrderByDescending(e => e.UpdatedAt)
            .ThenByDescending(e => e.Id)
            .ToListAsync(cancellationToken);

        return rows.Select(ToDto).ToList();
    }

    /// <inheritdoc />
    public async Task<ReadingEntryDto?> ApplyActionAsync(
        long id,
        string action,
        CancellationToken cancellationToken = default)
    {
        string normalized = (action ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized != "publish" && normalized != "archive")
        {
            throw new ArgumentException("action must be publish or archive");
        }

        ReadingEntry? entity = await _context.ReadingEntries
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity == null)
        {
            return null;
        }

        if (normalized == "publish")
        {
            entity.Status = "published";
            entity.Visibility = "public";
        }
        else
        {
            entity.Status = "archived";
            entity.Visibility = "private";
        }

        entity.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        ReadingEntry? entity = await _context.ReadingEntries
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity == null)
        {
            return false;
        }

        _context.ReadingEntries.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static List<string> NormalizeTags(List<string>? tags)
    {
        if (tags == null || tags.Count == 0)
        {
            return new List<string>();
        }

        return tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    private static string SerializeTags(List<string> tags)
    {
        return JsonSerializer.Serialize(tags, JsonOptions);
    }

    private static List<string> DeserializeTags(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new List<string>();
        }

        try
        {
            List<string>? parsed = JsonSerializer.Deserialize<List<string>>(raw, JsonOptions);
            return parsed ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static DateTime? ParseOptionalDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime parsed))
        {
            return parsed;
        }

        return null;
    }

    private static ReadingEntryDto ToDto(ReadingEntry entity)
    {
        return new ReadingEntryDto
        {
            Id = entity.Id,
            SourceType = entity.SourceType,
            ExternalId = entity.ExternalId,
            Quote = entity.Quote,
            Note = entity.Note,
            SourceTitle = entity.SourceTitle,
            SourceUrl = entity.SourceUrl,
            SourceCreatedAt = entity.SourceCreatedAt,
            ImportedAt = entity.ImportedAt,
            UpdatedAt = entity.UpdatedAt,
            Tags = DeserializeTags(entity.Tags),
            Status = entity.Status,
            Visibility = entity.Visibility,
        };
    }
}
