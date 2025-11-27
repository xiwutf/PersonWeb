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
        return Ok(ApiResponse<Project>.Success(project));
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
}
