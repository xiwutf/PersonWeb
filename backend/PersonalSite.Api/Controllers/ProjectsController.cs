using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Project>>>> GetProjects()
    {
        var projects = await _context.Projects
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return Ok(ApiResponse<List<Project>>.Success(projects));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Project>>> GetProject(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return NotFound();
        
        // 增加访问量（不等待保存，异步处理）
        project.ViewCount++;
        _ = Task.Run(async () =>
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // 静默失败，不影响响应
            }
        });
        
        return Ok(ApiResponse<Project>.Success(project));
    }

    /// <summary>
    /// 记录项目访问（用于前端统计）
    /// </summary>
    [HttpPost("{id}/view")]
    public async Task<ActionResult<ApiResponse<object>>> RecordView(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse.Error("项目不存在", 404));
        }

        project.ViewCount++;
        project.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(new { ViewCount = project.ViewCount }));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Project>>> CreateProject(Project project)
    {
        project.Id = Guid.NewGuid();
        project.CreatedAt = DateTime.Now;
        project.UpdatedAt = DateTime.Now;
        
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, ApiResponse<Project>.Success(project));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Project>>> UpdateProject(Guid id, Project project)
    {
        if (id != project.Id) return BadRequest();

        var existing = await _context.Projects.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Title = project.Title;
        existing.Description = project.Description;
        existing.CoverUrl = project.CoverUrl;
        existing.DemoUrl = project.DemoUrl;
        existing.GithubUrl = project.GithubUrl;
        existing.Status = project.Status;
        existing.TechStack = project.TechStack;
        existing.Content = project.Content;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<Project>.Success(existing));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProject(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<bool>.Success(true));
    }

    /// <summary>
    /// 获取项目访问趋势
    /// </summary>
    [HttpGet("{id}/trends")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetTrends(
        Guid id,
        [FromQuery] int days = 30)
    {
        // 这里简化实现，实际应该从访问日志表中统计
        // 目前返回模拟数据，后续可以集成真实的访问日志统计
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse.Error("项目不存在", 404));
        }

        // 生成模拟趋势数据（实际应该从 visit_logs 表统计）
        var trends = new List<object>();
        var random = new Random();
        var baseViews = project.ViewCount / days;
        
        for (int i = days - 1; i >= 0; i--)
        {
            var date = DateTime.Now.AddDays(-i).Date;
            trends.Add(new
            {
                Date = date.ToString("yyyy-MM-dd"),
                Views = baseViews + random.Next(-5, 10) // 模拟波动
            });
        }

        return Ok(ApiResponse.Success(new
        {
            ProjectId = id,
            ProjectTitle = project.Title,
            TotalViews = project.ViewCount,
            Trends = trends
        }));
    }

    /// <summary>
    /// 获取所有项目的访问统计
    /// </summary>
    [HttpGet("stats")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        var projects = await _context.Projects
            .OrderByDescending(p => p.ViewCount)
            .Take(10)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.ViewCount,
                p.Status
            })
            .ToListAsync();

        var totalViews = await _context.Projects.SumAsync(p => p.ViewCount);
        var totalProjects = await _context.Projects.CountAsync();

        return Ok(ApiResponse.Success(new
        {
            TotalProjects = totalProjects,
            TotalViews = totalViews,
            TopProjects = projects
        }));
    }
}
