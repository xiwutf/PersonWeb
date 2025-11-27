using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;


    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<Category>>>> GetList()
    {
        var list = await _context.Categories.OrderBy(c => c.Sort).ToListAsync();
        return Ok(ApiResponse<List<Category>>.Success(list));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Category>>> Create([FromBody] Category category)
    {
        if (await _context.Categories.AnyAsync(c => c.Name == category.Name))
        {
            return Ok(ApiResponse<Category>.Error("分类名称已存在"));
        }

        category.CreatedAt = DateTime.Now;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<Category>.Success(category));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Category>>> Update(long id, [FromBody] Category category)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        if (await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id != id))
        {
            return Ok(ApiResponse<Category>.Error("分类名称已存在"));
        }

        existing.Name = category.Name;
        existing.Slug = category.Slug;
        existing.Sort = category.Sort;
        
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<Category>.Success(existing));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        // 检查是否被文章引用
        if (await _context.Articles.AnyAsync(a => a.CategoryId == id))
        {
            return Ok(ApiResponse<object>.Error("该分类下还有文章，无法删除"));
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(null));
    }
}
