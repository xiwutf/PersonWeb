using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 友情链接控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FriendLinksController : ControllerBase
{
    private readonly AppDbContext _context;

    public FriendLinksController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取友情链接列表（公开接口，只返回启用的链接）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<FriendLink>>>> GetFriendLinks()
    {
        var links = await _context.FriendLinks
            .Where(l => l.Status == 1)
            .OrderBy(l => l.SortOrder)
            .ThenByDescending(l => l.CreatedAt)
            .ToListAsync();

        return Ok(ApiResponse<List<FriendLink>>.Success(links));
    }

    /// <summary>
    /// 获取所有友情链接（管理接口，包含禁用的）
    /// </summary>
    [HttpGet("all")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetAllFriendLinks()
    {
        var links = await _context.FriendLinks
            .OrderBy(l => l.SortOrder)
            .ThenByDescending(l => l.CreatedAt)
            .ToListAsync();

        return Ok(ApiResponse.Success(new { Total = links.Count, List = links }));
    }

    /// <summary>
    /// 获取单个友情链接
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<FriendLink>>> GetFriendLink(long id)
    {
        var link = await _context.FriendLinks.FindAsync(id);
        if (link == null)
        {
            return Ok(ApiResponse<FriendLink>.Error("链接不存在", 404));
        }

        return Ok(ApiResponse<FriendLink>.Success(link));
    }

    /// <summary>
    /// 创建友情链接
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<FriendLink>>> CreateFriendLink([FromBody] FriendLinkRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Ok(ApiResponse<FriendLink>.Error("链接名称不能为空", 400));
        }

        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return Ok(ApiResponse<FriendLink>.Error("链接地址不能为空", 400));
        }

        // 验证 URL 格式
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
        {
            return Ok(ApiResponse<FriendLink>.Error("链接地址格式不正确", 400));
        }

        var link = new FriendLink
        {
            Name = request.Name.Trim(),
            Url = request.Url.Trim(),
            Description = request.Description?.Trim(),
            LogoUrl = request.LogoUrl?.Trim(),
            SortOrder = request.SortOrder,
            Status = request.Status,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.FriendLinks.Add(link);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFriendLink), new { id = link.Id }, ApiResponse<FriendLink>.Success(link));
    }

    /// <summary>
    /// 更新友情链接
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<FriendLink>>> UpdateFriendLink(long id, [FromBody] FriendLinkRequest request)
    {
        var link = await _context.FriendLinks.FindAsync(id);
        if (link == null)
        {
            return Ok(ApiResponse<FriendLink>.Error("链接不存在", 404));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Ok(ApiResponse<FriendLink>.Error("链接名称不能为空", 400));
        }

        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return Ok(ApiResponse<FriendLink>.Error("链接地址不能为空", 400));
        }

        // 验证 URL 格式
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
        {
            return Ok(ApiResponse<FriendLink>.Error("链接地址格式不正确", 400));
        }

        link.Name = request.Name.Trim();
        link.Url = request.Url.Trim();
        link.Description = request.Description?.Trim();
        link.LogoUrl = request.LogoUrl?.Trim();
        link.SortOrder = request.SortOrder;
        link.Status = request.Status;
        link.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<FriendLink>.Success(link));
    }

    /// <summary>
    /// 删除友情链接
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteFriendLink(long id)
    {
        var link = await _context.FriendLinks.FindAsync(id);
        if (link == null)
        {
            return Ok(ApiResponse.Error("链接不存在", 404));
        }

        _context.FriendLinks.Remove(link);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "删除成功"));
    }
}

/// <summary>
/// 友情链接请求 DTO
/// </summary>
public class FriendLinkRequest
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public int SortOrder { get; set; } = 0;
    public sbyte Status { get; set; } = 1;
}

