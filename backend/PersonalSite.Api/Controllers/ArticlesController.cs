using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Security.Claims;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ArticlesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取文章列表
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="status">状态筛选 (0-草稿 1-已发布 2-下线)</param>
    /// <param name="sourceType">来源类型筛选 (manual/ai_generated/ai_optimized/imported)</param>
    /// <param name="categoryId">分类ID筛选</param>
    /// <param name="keyword">关键词搜索</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetArticles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] sbyte? status = null,
        [FromQuery] string? sourceType = null,
        [FromQuery] long? categoryId = null,
        [FromQuery] string? keyword = null)
    {
        var query = _context.Articles.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (!string.IsNullOrEmpty(sourceType))
        {
            query = query.Where(a => a.SourceType == sourceType);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(a => a.Title.Contains(keyword) || (a.Summary != null && a.Summary.Contains(keyword)));
        }

        var total = await query.CountAsync();
        
        var articles = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Slug,
                a.Summary,
                a.CoverUrl,
                a.Status,
                a.SourceType,
                a.CreatedAt,
                a.PublishTime,
                CategoryName = a.Category != null ? a.Category.Name : null
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(new { Total = total, List = articles }));
    }

    /// <summary>
    /// 获取文章详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Article>>> GetArticle(long id)
    {
        var article = await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
        {
            return Ok(ApiResponse<Article>.Error("文章不存在", 404));
        }
        
        return Ok(ApiResponse<Article>.Success(article));
    }

    /// <summary>
    /// 根据 Slug 获取文章详情
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<Article>>> GetArticleBySlug(string slug)
    {
        var article = await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (article == null)
        {
            return Ok(ApiResponse<Article>.Error("文章不存在", 404));
        }
        
        return Ok(ApiResponse<Article>.Success(article));
    }

    /// <summary>
    /// 创建/更新文章
    /// </summary>
    /// <param name="article"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize] 
    public async Task<ActionResult<ApiResponse>> SaveArticle([FromBody] Article article)
    {
        try
        {
            // 如果 ID 为 0，则为新建
            if (article.Id == 0)
            {
                // 检查 Slug 是否重复（如果提供了 Slug）
                if (!string.IsNullOrEmpty(article.Slug))
                {
                    var slugExists = await _context.Articles.AnyAsync(a => a.Slug == article.Slug);
                    if (slugExists)
                    {
                        return Ok(ApiResponse.Error($"URL Slug '{article.Slug}' 已存在，请使用其他值", 400));
                    }
                }

                article.CreatedAt = DateTime.Now;
                article.UpdatedAt = DateTime.Now;
                
                // 自动填充发布时间
                if (article.Status == 1 && article.PublishTime == null)
                {
                    article.PublishTime = DateTime.Now;
                }

                // 设置作者
                var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId > 0) article.AuthorId = userId;

                _context.Articles.Add(article);
                await _context.SaveChangesAsync();
                return Ok(ApiResponse.Success(new { id = article.Id }, "创建成功"));
            }
            else
            {
                // 更新
                var existing = await _context.Articles.FindAsync(article.Id);
                if (existing == null) return Ok(ApiResponse.Error("文章不存在", 404));

                // 检查 Slug 是否重复（如果 Slug 有变化）
                if (!string.IsNullOrEmpty(article.Slug) && existing.Slug != article.Slug)
                {
                    var slugExists = await _context.Articles.AnyAsync(a => a.Slug == article.Slug && a.Id != article.Id);
                    if (slugExists)
                    {
                        return Ok(ApiResponse.Error($"URL Slug '{article.Slug}' 已存在，请使用其他值", 400));
                    }
                }

                // 创建版本历史（保存旧版本）
                var history = new Article
                {
                    Title = existing.Title,
                    Slug = existing.Slug + $"-v{existing.Version}", // 避免slug冲突
                    Summary = existing.Summary,
                    ContentMd = existing.ContentMd,
                    ContentHtml = existing.ContentHtml,
                    CoverUrl = existing.CoverUrl,
                    CategoryId = existing.CategoryId,
                    Status = 2, // 下线（历史版本）
                    Version = existing.Version,
                    ParentId = existing.Id,
                    CreatedAt = existing.CreatedAt,
                    UpdatedAt = existing.UpdatedAt,
                    AuthorId = existing.AuthorId
                };
                _context.Articles.Add(history);

                // 更新当前版本
                existing.Title = article.Title;
                existing.Slug = article.Slug;
                existing.Summary = article.Summary;
                existing.ContentMd = article.ContentMd;
                existing.ContentHtml = article.ContentHtml;
                existing.CoverUrl = article.CoverUrl;
                existing.CategoryId = article.CategoryId;
                existing.Version++;
                existing.UpdatedAt = DateTime.Now;

                // 状态变更逻辑
                if (existing.Status != 1 && article.Status == 1 && existing.PublishTime == null)
                {
                    existing.PublishTime = DateTime.Now;
                }
                existing.Status = article.Status;

                await _context.SaveChangesAsync();
                return Ok(ApiResponse.Success(null, "更新成功"));
            }
        }
        catch (DbUpdateException ex)
        {
            // 处理数据库约束错误（如唯一索引冲突）
            if (ex.InnerException != null && ex.InnerException.Message.Contains("Duplicate entry"))
            {
                return Ok(ApiResponse.Error("URL Slug 已存在，请使用其他值", 400));
            }
            return Ok(ApiResponse.Error($"保存失败: {ex.Message}", 500));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"保存失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除文章
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteArticle(long id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null)
        {
            return Ok(ApiResponse.Error("文章不存在", 404));
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "删除成功"));
    }

    /// <summary>
    /// 获取文章版本历史
    /// </summary>
    [HttpGet("{id}/versions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetVersions(long id)
    {
        var versions = await _context.Articles
            .Where(a => a.ParentId == id || a.Id == id)
            .OrderByDescending(a => a.Version)
            .Select(a => new
            {
                a.Id,
                a.Version,
                a.Title,
                a.UpdatedAt,
                a.Status
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(versions));
    }

    /// <summary>
    /// 获取指定版本的文章内容
    /// </summary>
    [HttpGet("{id}/versions/{versionId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Article>>> GetVersion(long id, long versionId)
    {
        var version = await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == versionId && (a.ParentId == id || a.Id == id));

        if (version == null)
        {
            return Ok(ApiResponse<Article>.Error("版本不存在", 404));
        }

        return Ok(ApiResponse<Article>.Success(version));
    }

    /// <summary>
    /// 恢复指定版本
    /// </summary>
    [HttpPost("{id}/versions/{versionId}/restore")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RestoreVersion(long id, long versionId)
    {
        var current = await _context.Articles.FindAsync(id);
        if (current == null)
        {
            return Ok(ApiResponse.Error("文章不存在", 404));
        }

        var version = await _context.Articles.FindAsync(versionId);
        if (version == null || (version.ParentId != id && version.Id != id))
        {
            return Ok(ApiResponse.Error("版本不存在", 404));
        }

        // 创建当前版本的备份
        var backup = new Article
        {
            Title = current.Title,
            Slug = current.Slug + $"-v{current.Version}",
            Summary = current.Summary,
            ContentMd = current.ContentMd,
            ContentHtml = current.ContentHtml,
            CoverUrl = current.CoverUrl,
            CategoryId = current.CategoryId,
            Status = 2,
            Version = current.Version,
            ParentId = current.Id,
            CreatedAt = current.CreatedAt,
            UpdatedAt = current.UpdatedAt,
            AuthorId = current.AuthorId
        };
        _context.Articles.Add(backup);

        // 恢复版本内容
        current.Title = version.Title;
        current.Summary = version.Summary;
        current.ContentMd = version.ContentMd;
        current.ContentHtml = version.ContentHtml;
        current.CoverUrl = version.CoverUrl;
        current.CategoryId = version.CategoryId;
        current.Version++;
        current.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success(null, "恢复成功"));
    }

    /// <summary>
    /// 内容中枢总览接口
    /// </summary>
    [HttpGet("overview")]
    public async Task<ActionResult<ApiResponse<object>>> GetContentHubOverview()
    {
        try
        {
            // 文章统计
            var articleStats = new
            {
                Total = await _context.Articles.CountAsync(),
                Draft = await _context.Articles.CountAsync(a => a.Status == 0),
                Published = await _context.Articles.CountAsync(a => a.Status == 1),
                Offline = await _context.Articles.CountAsync(a => a.Status == 2),
                AiGenerated = await _context.Articles.CountAsync(a => a.SourceType == "ai_generated"),
                AiOptimized = await _context.Articles.CountAsync(a => a.SourceType == "ai_optimized"),
                Manual = await _context.Articles.CountAsync(a => a.SourceType == "manual")
            };

            // 最近更新的文章（取最新的5条）
            var recentArticles = await _context.Articles
                .Where(a => a.Status != 2) // 排除下线的
                .OrderByDescending(a => a.UpdatedAt)
                .Take(5)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Status,
                    a.SourceType,
                    a.UpdatedAt,
                    CategoryName = a.Category != null ? a.Category.Name : null
                })
                .ToListAsync();

            // 处理返回的数据（在内存中进行 switch 操作）
            var processedArticles = recentArticles.Select(a => new
            {
                a.Id,
                a.Title,
                a.Status,
                a.SourceType,
                a.UpdatedAt,
                TypeName = a.CategoryName ?? "未分类",
                SourceTypeName = a.SourceType switch
                {
                    "manual" => "手动创建",
                    "ai_generated" => "AI生成",
                    "ai_optimized" => "AI优化",
                    "imported" => "导入",
                    _ => "未知"
                },
                StatusName = a.Status switch
                {
                    0 => "草稿",
                    1 => "已发布",
                    2 => "下线",
                    _ => "未知"
                }
            }).ToList();

            // 项目统计（Project.Status: Active/Completed/Archived）
            var projectStats = new
            {
                Total = await _context.Projects.CountAsync(),
                Published = await _context.Projects.CountAsync(p => p.Status == "Active" || p.Status == "Completed")
            };

            // 工具统计（Tool.Status: draft/published）
            var toolStats = new
            {
                Total = await _context.Tools.CountAsync(),
                Published = await _context.Tools.CountAsync(t => t.Status == "published")
            };

            // 文档统计（Document.Status: pending/processing/completed/failed）
            var docStats = new
            {
                Total = await _context.Documents.CountAsync(),
                Published = await _context.Documents.CountAsync(d => d.Status == "completed")
            };

            return Ok(ApiResponse.Success(new
            {
                Articles = articleStats,
                RecentArticles = processedArticles,
                Projects = projectStats,
                Tools = toolStats,
                Documents = docStats
            }));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"获取总览数据失败: {ex.Message}"));
        }
    }
}
