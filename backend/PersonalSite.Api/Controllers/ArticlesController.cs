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
    /// <param name="categoryId">分类ID筛选</param>
    /// <param name="keyword">关键词搜索</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetArticles(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] sbyte? status = null,
        [FromQuery] long? categoryId = null,
        [FromQuery] string? keyword = null)
    {
        var query = _context.Articles.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
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

                existing.Title = article.Title;
                existing.Slug = article.Slug;
                existing.Summary = article.Summary;
                existing.ContentMd = article.ContentMd;
                existing.ContentHtml = article.ContentHtml;
                existing.CoverUrl = article.CoverUrl;
                existing.CategoryId = article.CategoryId;
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
}
