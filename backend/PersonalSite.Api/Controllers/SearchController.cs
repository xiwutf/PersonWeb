using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 全文搜索控制器
/// 使用 MySQL 全文索引进行搜索
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly AppDbContext _context;

    public SearchController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 全文搜索
    /// </summary>
    /// <param name="keyword">搜索关键词</param>
    /// <param name="type">搜索类型：all/articles/projects/knowledge</param>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> Search(
        [FromQuery] string keyword,
        [FromQuery] string type = "all",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return Ok(ApiResponse.Error("搜索关键词不能为空", 400));
        }

        // 转义特殊字符，防止 SQL 注入
        var escapedKeyword = keyword.Replace("'", "''").Replace("\\", "\\\\");

        var results = new SearchResults
        {
            Keyword = keyword,
            Type = type,
            Total = 0,
            Articles = new List<SearchResultItem>(),
            Projects = new List<SearchResultItem>(),
            KnowledgeBases = new List<SearchResultItem>()
        };

        // 使用 LIKE 搜索（兼容性更好，全文索引需要特殊配置）
        // 搜索文章
        if (type == "all" || type == "articles")
        {
            var articleQuery = _context.Articles
                .Where(a => a.Status == 1) // 只搜索已发布的文章
                .Where(a => a.Title.Contains(keyword) ||
                           (a.Summary != null && a.Summary.Contains(keyword)) ||
                           (a.ContentMd != null && a.ContentMd.Contains(keyword)));

            var articleTotal = await articleQuery.CountAsync();
            var articles = await articleQuery
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.Category)
                .Select(a => new SearchResultItem
                {
                    Id = a.Id.ToString(),
                    Title = a.Title,
                    Summary = a.Summary ?? "",
                    Content = a.ContentMd ?? "",
                    Type = "article",
                    Url = $"/blog/{a.Slug ?? a.Id.ToString()}",
                    CreatedAt = a.CreatedAt,
                    Category = a.Category != null ? a.Category.Name : null
                })
                .ToListAsync();

            results.Articles = articles;
            results.Total += articleTotal;
        }

        // 搜索项目
        if (type == "all" || type == "projects")
        {
            var projectQuery = _context.Projects
                .Where(p => p.Title.Contains(keyword) ||
                           (p.Description != null && p.Description.Contains(keyword)) ||
                           (p.Content != null && p.Content.Contains(keyword)));

            var projectTotal = await projectQuery.CountAsync();
            var projects = await projectQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new SearchResultItem
                {
                    Id = p.Id.ToString(),
                    Title = p.Title,
                    Summary = p.Description ?? "",
                    Content = p.Content ?? "",
                    Type = "project",
                    Url = $"/projects/detail-{p.Id}",
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            results.Projects = projects;
            results.Total += projectTotal;
        }

        // 搜索知识库
        if (type == "all" || type == "knowledge")
        {
            var knowledgeQuery = _context.KnowledgeBases
                .Where(k => k.Status == 1) // 只搜索已发布的知识库
                .Where(k => k.Title.Contains(keyword) ||
                           (k.Content != null && k.Content.Contains(keyword)));

            var knowledgeTotal = await knowledgeQuery.CountAsync();
            var knowledgeBases = await knowledgeQuery
                .OrderByDescending(k => k.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(k => new SearchResultItem
                {
                    Id = k.Id.ToString(),
                    Title = k.Title,
                    Summary = "",
                    Content = k.Content ?? "",
                    Type = "knowledge",
                    Url = $"/knowledge/{k.Id}",
                    CreatedAt = k.CreatedAt,
                    Category = k.Category
                })
                .ToListAsync();

            results.KnowledgeBases = knowledgeBases;
            results.Total += knowledgeTotal;
        }

        return Ok(ApiResponse.Success(results));
    }
}

/// <summary>
/// 搜索结果
/// </summary>
public class SearchResults
{
    public string Keyword { get; set; } = string.Empty;
    public string Type { get; set; } = "all";
    public int Total { get; set; }
    public List<SearchResultItem> Articles { get; set; } = new();
    public List<SearchResultItem> Projects { get; set; } = new();
    public List<SearchResultItem> KnowledgeBases { get; set; } = new();
}

/// <summary>
/// 搜索结果项
/// </summary>
public class SearchResultItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Category { get; set; }
}

