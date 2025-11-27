using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

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
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetArticles(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] sbyte? status = null,
        [FromQuery] long? categoryId = null)
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
                // a.ViewCount, // 数据库中无此字段
                a.Status,
                a.CreatedAt,
                CategoryName = a.Category != null ? a.Category.Name : null
            })
            .ToListAsync();

        return Ok(new { Total = total, Items = articles });
    }

    /// <summary>
    /// 获取文章详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticle(long id)
    {
        var article = await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        // 增加浏览量逻辑需要调整，因为数据库没有 ViewCount 字段
        // 可以考虑记录到 pv_uv_daily 表，或者忽略
        
        return Ok(article);
    }

    /// <summary>
    /// 创建文章
    /// </summary>
    /// <param name="article"></param>
    /// <returns></returns>
    [HttpPost]
    // [Authorize] 
    public async Task<IActionResult> CreateArticle([FromBody] Article article)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        article.CreatedAt = DateTime.Now;
        article.UpdatedAt = DateTime.Now;

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
    }

    /// <summary>
    /// 更新文章
    /// </summary>
    /// <param name="id"></param>
    /// <param name="article"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    // [Authorize]
    public async Task<IActionResult> UpdateArticle(long id, [FromBody] Article article)
    {
        if (id != article.Id)
        {
            return BadRequest();
        }

        var existingArticle = await _context.Articles.FindAsync(id);
        if (existingArticle == null)
        {
            return NotFound();
        }

        existingArticle.Title = article.Title;
        existingArticle.Slug = article.Slug;
        existingArticle.Summary = article.Summary;
        existingArticle.ContentMd = article.ContentMd;
        existingArticle.ContentHtml = article.ContentHtml;
        existingArticle.CoverUrl = article.CoverUrl;
        existingArticle.Status = article.Status;
        existingArticle.CategoryId = article.CategoryId;
        existingArticle.UpdatedAt = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Articles.Any(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// 删除文章
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    // [Authorize]
    public async Task<IActionResult> DeleteArticle(long id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
