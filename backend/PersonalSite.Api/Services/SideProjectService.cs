using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services;

/// <summary>
/// 副业项目服务层
/// 处理项目进度计算、逾期判定、阻塞判定等业务逻辑
/// </summary>
public class SideProjectService
{
    private readonly AppDbContext _context;

    public SideProjectService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 计算项目进度（基于任务完成度）
    /// </summary>
    /// <param name="projectId">项目ID</param>
    /// <returns>进度百分比（0-100）</returns>
    public async Task<int> CalculateProgressAsync(int projectId)
    {
        var tasks = await _context.SideProjectTasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        if (tasks.Count == 0)
        {
            return 0; // 没有任务时进度为0
        }

        // 计算已完成任务数
        var completedTasks = tasks.Count(t => t.Status == 2); // 2=已完成

        // 计算进度百分比
        var progress = (int)Math.Round((double)completedTasks / tasks.Count * 100);

        return Math.Max(0, Math.Min(100, progress)); // 确保在0-100范围内
    }

    /// <summary>
    /// 更新项目进度（如果不是手动覆盖）
    /// </summary>
    public async Task UpdateProgressIfAutoAsync(int projectId)
    {
        var project = await _context.SideProjects.FindAsync(projectId);
        if (project == null || project.IsProgressManual)
        {
            return; // 项目不存在或已手动覆盖，不自动更新
        }

        var progress = await CalculateProgressAsync(projectId);
        project.Progress = progress;
        project.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 判断项目是否逾期
    /// </summary>
    public bool IsOverdue(SideProject project)
    {
        if (!project.DeadlineAt.HasValue)
        {
            return false; // 没有截止时间，不算逾期
        }

        return DateTime.Now > project.DeadlineAt.Value && project.Status != 1; // 1=已完成
    }

    /// <summary>
    /// 判断任务是否逾期
    /// </summary>
    public bool IsTaskOverdue(SideProjectTask task)
    {
        if (!task.DueAt.HasValue)
        {
            return false;
        }

        return DateTime.Now > task.DueAt.Value && task.Status != 2; // 2=已完成
    }

    /// <summary>
    /// 判断项目是否应该被标记为阻塞
    /// </summary>
    public async Task<bool> ShouldBeBlockedAsync(int projectId)
    {
        var project = await _context.SideProjects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
        {
            return false;
        }

        // 如果有明确的阻塞标记，返回true
        if (project.Blocked && !string.IsNullOrEmpty(project.BlockReason))
        {
            return true;
        }

        // 检查是否有逾期任务且没有下一步行动
        var hasOverdueTasks = project.Tasks.Any(t => IsTaskOverdue(t));
        if (hasOverdueTasks && string.IsNullOrEmpty(project.NextAction))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取项目的下一个行动项（从任务或项目中提取）
    /// </summary>
    public async Task<string?> GetNextActionAsync(int projectId)
    {
        var project = await _context.SideProjects
            .Include(p => p.Tasks.Where(t => t.Status != 2)) // 未完成的任务
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
        {
            return null;
        }

        // 如果项目有明确的下一步行动，返回它
        if (!string.IsNullOrEmpty(project.NextAction))
        {
            return project.NextAction;
        }

        // 否则返回最早截止的未完成任务
        var nextTask = project.Tasks
            .Where(t => t.Status != 2 && t.DueAt.HasValue)
            .OrderBy(t => t.DueAt)
            .FirstOrDefault();

        return nextTask?.Title;
    }

    /// <summary>
    /// 同步项目状态与阶段
    /// </summary>
    public async Task SyncStatusAndStageAsync(int projectId)
    {
        var project = await _context.SideProjects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
        {
            return;
        }

        // 根据状态和阻塞情况更新阶段
        if (project.Status == 1) // 已完成
        {
            project.Stage = "已完成";
        }
        else if (project.Blocked)
        {
            project.Stage = "卡住";
        }
        else if (project.Status == 0) // 进行中
        {
            project.Stage = "进行中";
        }
        else if (project.Status == 2) // 待付款
        {
            project.Stage = "待验收";
        }
        else
        {
            project.Stage = "待开始";
        }

        project.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
    }
}

