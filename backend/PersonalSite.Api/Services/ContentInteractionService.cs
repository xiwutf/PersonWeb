using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services.Cache;

namespace PersonalSite.Api.Services;

public interface IContentInteractionService
{
    Task<InteractionSummaryDto> GetSummaryAsync(string targetType, string targetId, string? visitorId);
    Task<(bool created, int likeCount)> LikeAsync(string targetType, string targetId, string visitorId);
    Task<(bool removed, int likeCount)> UnlikeAsync(string targetType, string targetId, string visitorId);
    Task<List<PublicCommentDto>> ListApprovedCommentsAsync(string targetType, string targetId);
    Task<ContentComment> CreateCommentAsync(CreateCommentRequestDto dto, string? ip);
    Task<AdminCommentListDto> ListAdminCommentsAsync(string? status, string? targetType, int page, int pageSize);
    Task<ContentComment?> ApproveAsync(long id);
    Task<ContentComment?> RejectAsync(long id);
    Task<bool> DeleteAsync(long id);
}

public class ContentInteractionService : IContentInteractionService
{
    public static readonly HashSet<string> AllowedTargetTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "article",
        "reading",
        "quote",
        "moment",
    };

    private static readonly Regex HtmlTagRegex = new("<[^>]*>", RegexOptions.Compiled);
    private static readonly Regex MultiSpaceRegex = new(@"\s{2,}", RegexOptions.Compiled);

    private readonly AppDbContext _db;
    private readonly ICacheService _cache;

    public ContentInteractionService(AppDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<InteractionSummaryDto> GetSummaryAsync(string targetType, string targetId, string? visitorId)
    {
        (string type, string id) = NormalizeTarget(targetType, targetId);

        int likeCount = await _db.ContentLikes.AsNoTracking()
            .CountAsync(x => x.TargetType == type && x.TargetId == id);

        int commentCount = await _db.ContentComments.AsNoTracking()
            .CountAsync(x => x.TargetType == type && x.TargetId == id && x.Status == "approved");

        bool liked = false;
        if (!string.IsNullOrWhiteSpace(visitorId))
        {
            string key = NormalizeVisitorKey(visitorId);
            liked = await _db.ContentLikes.AsNoTracking()
                .AnyAsync(x => x.TargetType == type && x.TargetId == id && x.VisitorKey == key);
        }

        return new InteractionSummaryDto
        {
            LikeCount = likeCount,
            Liked = liked,
            CommentCount = commentCount,
        };
    }

    public async Task<(bool created, int likeCount)> LikeAsync(string targetType, string targetId, string visitorId)
    {
        (string type, string id) = NormalizeTarget(targetType, targetId);
        string key = NormalizeVisitorKey(visitorId);

        bool exists = await _db.ContentLikes
            .AnyAsync(x => x.TargetType == type && x.TargetId == id && x.VisitorKey == key);

        bool created = false;
        if (!exists)
        {
            _db.ContentLikes.Add(new ContentLike
            {
                TargetType = type,
                TargetId = id,
                VisitorKey = key,
                CreatedAt = DateTime.Now,
            });
            try
            {
                await _db.SaveChangesAsync();
                created = true;
            }
            catch (DbUpdateException)
            {
                // 并发下唯一约束冲突视为已点赞
                _db.ChangeTracker.Clear();
            }
        }

        int likeCount = await _db.ContentLikes.AsNoTracking()
            .CountAsync(x => x.TargetType == type && x.TargetId == id);
        return (created, likeCount);
    }

    public async Task<(bool removed, int likeCount)> UnlikeAsync(string targetType, string targetId, string visitorId)
    {
        (string type, string id) = NormalizeTarget(targetType, targetId);
        string key = NormalizeVisitorKey(visitorId);

        ContentLike? row = await _db.ContentLikes
            .FirstOrDefaultAsync(x => x.TargetType == type && x.TargetId == id && x.VisitorKey == key);

        bool removed = false;
        if (row != null)
        {
            _db.ContentLikes.Remove(row);
            await _db.SaveChangesAsync();
            removed = true;
        }

        int likeCount = await _db.ContentLikes.AsNoTracking()
            .CountAsync(x => x.TargetType == type && x.TargetId == id);
        return (removed, likeCount);
    }

    public async Task<List<PublicCommentDto>> ListApprovedCommentsAsync(string targetType, string targetId)
    {
        (string type, string id) = NormalizeTarget(targetType, targetId);

        return await _db.ContentComments.AsNoTracking()
            .Where(c => c.TargetType == type && c.TargetId == id && c.Status == "approved")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new PublicCommentDto
            {
                Id = c.Id,
                Nickname = c.Nickname,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
            })
            .ToListAsync();
    }

    public async Task<ContentComment> CreateCommentAsync(CreateCommentRequestDto dto, string? ip)
    {
        // TODO(V1): 若后续接入 Cloudflare Turnstile，在此校验 CaptchaToken
        (string type, string id) = NormalizeTarget(dto.TargetType, dto.TargetId);

        string nickname = SanitizePlainText(dto.Nickname ?? string.Empty).Trim();
        string email = (dto.Email ?? string.Empty).Trim();
        string content = SanitizePlainText(dto.Content ?? string.Empty).Trim();

        if (nickname.Length < 2 || nickname.Length > 30)
        {
            throw new ArgumentException("昵称长度需为 2～30 字");
        }

        if (!IsValidEmail(email) || email.Length > 120)
        {
            throw new ArgumentException("邮箱格式不正确");
        }

        if (content.Length < 2 || content.Length > 1000)
        {
            throw new ArgumentException("回应内容长度需为 2～1000 字");
        }

        string? visitorKey = string.IsNullOrWhiteSpace(dto.VisitorId)
            ? null
            : NormalizeVisitorKey(dto.VisitorId);

        await EnsureCommentRateLimitAsync(ip, visitorKey);

        ContentComment comment = new ContentComment
        {
            TargetType = type,
            TargetId = id,
            ParentId = null,
            Nickname = nickname,
            Email = email,
            Content = content,
            Status = "pending",
            Ip = ip,
            VisitorKey = visitorKey,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        _db.ContentComments.Add(comment);
        await _db.SaveChangesAsync();
        return comment;
    }

    public async Task<AdminCommentListDto> ListAdminCommentsAsync(string? status, string? targetType, int page, int pageSize)
    {
        int safePage = Math.Max(1, page);
        int safeSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<ContentComment> query = _db.ContentComments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
        {
            string st = status.Trim().ToLowerInvariant();
            query = query.Where(c => c.Status == st);
        }

        if (!string.IsNullOrWhiteSpace(targetType))
        {
            string tt = targetType.Trim().ToLowerInvariant();
            query = query.Where(c => c.TargetType == tt);
        }

        int total = await query.CountAsync();
        List<AdminCommentDto> list = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((safePage - 1) * safeSize)
            .Take(safeSize)
            .Select(c => new AdminCommentDto
            {
                Id = c.Id,
                TargetType = c.TargetType,
                TargetId = c.TargetId,
                Nickname = c.Nickname,
                Email = c.Email,
                Content = c.Content,
                Status = c.Status,
                Ip = c.Ip,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
            .ToListAsync();

        return new AdminCommentListDto { Total = total, List = list };
    }

    public async Task<ContentComment?> ApproveAsync(long id)
    {
        ContentComment? comment = await _db.ContentComments.FindAsync(id);
        if (comment == null) return null;

        comment.Status = "approved";
        comment.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return comment;
    }

    public async Task<ContentComment?> RejectAsync(long id)
    {
        ContentComment? comment = await _db.ContentComments.FindAsync(id);
        if (comment == null) return null;

        comment.Status = "rejected";
        comment.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        ContentComment? comment = await _db.ContentComments.FindAsync(id);
        if (comment == null) return false;

        _db.ContentComments.Remove(comment);
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task EnsureCommentRateLimitAsync(string? ip, string? visitorKey)
    {
        string rateKey = !string.IsNullOrEmpty(visitorKey)
            ? $"comment-rate:visitor:{visitorKey}"
            : $"comment-rate:ip:{ip ?? "unknown"}";

        CommentRateInfo? info = await _cache.GetAsync<CommentRateInfo>(rateKey);
        DateTime now = DateTime.UtcNow;

        if (info == null || (now - info.WindowStart).TotalSeconds >= 60)
        {
            info = new CommentRateInfo { Count = 1, WindowStart = now };
            await _cache.SetAsync(rateKey, info, TimeSpan.FromMinutes(2));
            return;
        }

        if (info.Count >= 3)
        {
            throw new InvalidOperationException("提交过于频繁，请稍后再试");
        }

        info.Count += 1;
        await _cache.SetAsync(rateKey, info, TimeSpan.FromMinutes(2));
    }

    public static (string type, string id) NormalizeTarget(string? targetType, string? targetId)
    {
        string type = (targetType ?? string.Empty).Trim().ToLowerInvariant();
        string id = (targetId ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(type) || !AllowedTargetTypes.Contains(type))
        {
            throw new ArgumentException("不支持的内容类型");
        }

        if (string.IsNullOrEmpty(id) || id.Length > 180 || id.Contains('/') || id.Contains('\\') || id.Contains('\0'))
        {
            throw new ArgumentException("无效的内容 ID");
        }

        return (type, id);
    }

    public static string NormalizeVisitorKey(string visitorId)
    {
        string key = (visitorId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(key) || key.Length > 100)
        {
            throw new ArgumentException("无效的访客标识");
        }
        return key;
    }

    public static string SanitizePlainText(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        string noTags = HtmlTagRegex.Replace(input, string.Empty);
        string decoded = System.Net.WebUtility.HtmlDecode(noTags);
        return MultiSpaceRegex.Replace(decoded.Replace('\0', ' '), " ");
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            MailAddress addr = new MailAddress(email);
            return addr.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private sealed class CommentRateInfo
    {
        public int Count { get; set; }
        public DateTime WindowStart { get; set; }
    }
}
