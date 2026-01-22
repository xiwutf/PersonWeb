using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using System.Text.RegularExpressions;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CognitionDocsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CognitionDocsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 生成 Slug（从标题自动生成）
    /// </summary>
    private string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return $"doc-{DateTime.Now:yyyyMMddHHmmss}";
        }

        // 转换为小写
        var slug = title.ToLowerInvariant();

        // 替换中文字符为拼音（简化处理：先用时间戳兜底）
        // 这里先做最简单的处理：空格转横线，去特殊字符
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        // 如果处理后的 slug 为空或包含中文，使用时间戳
        if (string.IsNullOrEmpty(slug) || Regex.IsMatch(slug, @"[\u4e00-\u9fa5]"))
        {
            slug = $"doc-{DateTime.Now:yyyyMMddHHmmss}";
        }

        return slug;
    }

    /// <summary>
    /// 确保 Slug 唯一（如果重复则添加数字后缀）
    /// </summary>
    private async Task<string> EnsureUniqueSlug(string slug, long? excludeId = null)
    {
        var baseSlug = slug;
        var counter = 1;
        var finalSlug = slug;

        while (await _context.CognitionDocs.AnyAsync(d => 
            d.Slug == finalSlug && (excludeId == null || d.Id != excludeId.Value)))
        {
            finalSlug = $"{baseSlug}-{counter}";
            counter++;
        }

        return finalSlug;
    }

    /// <summary>
    /// 获取认知说明书列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetCognitionDocs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? keyword = null)
    {
        var query = _context.CognitionDocs.AsQueryable();

        // 状态筛选
        if (!string.IsNullOrEmpty(status) && status != "all")
        {
            query = query.Where(d => d.Status == status);
        }

        // 关键词搜索（匹配标题或 slug）
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(d => d.Title.Contains(keyword) || 
                                    d.Slug.Contains(keyword) ||
                                    (d.Summary != null && d.Summary.Contains(keyword)));
        }

        var total = await query.CountAsync();

        var docs = await query
            .OrderByDescending(d => d.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new CognitionDocListItemDto
            {
                Id = d.Id,
                Title = d.Title,
                Slug = d.Slug,
                Summary = d.Summary,
                Status = d.Status,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(new { Total = total, List = docs }));
    }

    /// <summary>
    /// 根据 ID 获取认知说明书详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CognitionDocDetailDto>>> GetCognitionDoc(long id)
    {
        var doc = await _context.CognitionDocs.FindAsync(id);

        if (doc == null)
        {
            return Ok(ApiResponse<CognitionDocDetailDto>.Error("认知说明书不存在", 404));
        }

        var dto = new CognitionDocDetailDto
        {
            Id = doc.Id,
            Title = doc.Title,
            Slug = doc.Slug,
            Summary = doc.Summary,
            ContentMd = doc.ContentMd,
            Status = doc.Status,
            CreatedAt = doc.CreatedAt,
            UpdatedAt = doc.UpdatedAt
        };

        return Ok(ApiResponse<CognitionDocDetailDto>.Success(dto));
    }

    /// <summary>
    /// 根据 Slug 获取已发布的认知说明书详情（前台使用）
    /// </summary>
    [HttpGet("by-slug/{slug}")]
    public async Task<ActionResult<ApiResponse<CognitionDocDetailDto>>> GetCognitionDocBySlug(string slug)
    {
        var doc = await _context.CognitionDocs
            .FirstOrDefaultAsync(d => d.Slug == slug && d.Status == "published");

        if (doc == null)
        {
            return Ok(ApiResponse<CognitionDocDetailDto>.Error("认知说明书不存在或未发布", 404));
        }

        var dto = new CognitionDocDetailDto
        {
            Id = doc.Id,
            Title = doc.Title,
            Slug = doc.Slug,
            Summary = doc.Summary,
            ContentMd = doc.ContentMd,
            Status = doc.Status,
            CreatedAt = doc.CreatedAt,
            UpdatedAt = doc.UpdatedAt
        };

        return Ok(ApiResponse<CognitionDocDetailDto>.Success(dto));
    }

    /// <summary>
    /// 创建认知说明书
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateCognitionDoc([FromBody] CognitionDocCreateDto dto)
    {
        try
        {
            // 生成或验证 Slug
            var slug = !string.IsNullOrWhiteSpace(dto.Slug) 
                ? dto.Slug 
                : GenerateSlug(dto.Title);

            // 确保 Slug 唯一
            slug = await EnsureUniqueSlug(slug);

            var doc = new CognitionDoc
            {
                Title = dto.Title,
                Slug = slug,
                Summary = dto.Summary,
                ContentMd = dto.ContentMd,
                Status = dto.Status ?? "draft",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.CognitionDocs.Add(doc);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = doc.Id }, "创建成功"));
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("Duplicate entry"))
            {
                return Ok(ApiResponse.Error("URL Slug 已存在，请使用其他值", 400));
            }
            return Ok(ApiResponse.Error($"创建失败: {ex.Message}", 500));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"创建失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新认知说明书
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateCognitionDoc(long id, [FromBody] CognitionDocUpdateDto dto)
    {
        try
        {
            var doc = await _context.CognitionDocs.FindAsync(id);
            if (doc == null)
            {
                return Ok(ApiResponse.Error("认知说明书不存在", 404));
            }

            // 检查 Slug 是否重复（如果 Slug 有变化）
            if (doc.Slug != dto.Slug)
            {
                var slugExists = await _context.CognitionDocs
                    .AnyAsync(d => d.Slug == dto.Slug && d.Id != id);
                if (slugExists)
                {
                    return Ok(ApiResponse.Error($"URL Slug '{dto.Slug}' 已存在，请使用其他值", 400));
                }
            }

            // 更新字段
            doc.Title = dto.Title;
            doc.Slug = dto.Slug;
            doc.Summary = dto.Summary;
            doc.ContentMd = dto.ContentMd;
            doc.Status = dto.Status;
            doc.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "更新成功"));
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("Duplicate entry"))
            {
                return Ok(ApiResponse.Error("URL Slug 已存在，请使用其他值", 400));
            }
            return Ok(ApiResponse.Error($"更新失败: {ex.Message}", 500));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"更新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除认知说明书
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteCognitionDoc(long id)
    {
        var doc = await _context.CognitionDocs.FindAsync(id);
        if (doc == null)
        {
            return Ok(ApiResponse.Error("认知说明书不存在", 404));
        }

        _context.CognitionDocs.Remove(doc);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "删除成功"));
    }

    /// <summary>
    /// 发布/撤回认知说明书
    /// </summary>
    [HttpPatch("{id}/publish")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> PublishCognitionDoc(long id, [FromBody] CognitionDocPublishDto dto)
    {
        var doc = await _context.CognitionDocs.FindAsync(id);
        if (doc == null)
        {
            return Ok(ApiResponse.Error("认知说明书不存在", 404));
        }

        if (dto.Status != "published" && dto.Status != "draft")
        {
            return Ok(ApiResponse.Error("状态值无效，只能是 'published' 或 'draft'", 400));
        }

        doc.Status = dto.Status;
        doc.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, dto.Status == "published" ? "发布成功" : "已撤回"));
    }
}
