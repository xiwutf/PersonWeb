using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Enums;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>
/// 通知生成服务
/// 负责根据业务规则生成站内提醒
/// </summary>
public class NotificationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(AppDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 生成所有符合条件的提醒
    /// </summary>
    public async Task GenerateNotificationsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var now = DateTime.Now;

        try
        {
            // 先测试数据库连接
            if (!await TestDatabaseConnectionAsync())
            {
                _logger.LogWarning("数据库连接失败，跳过提醒生成");
                return;
            }

            // 1. 项目截止前3天提醒
            await GenerateProjectDueSoonNotificationsAsync(today, now);

            // 2. 任务到期当天提醒
            await GenerateTaskDueTodayNotificationsAsync(today, now);

            // 3. 项目卡住超过2天提醒
            await GenerateProjectBlockedTooLongNotificationsAsync(today, now);

            _logger.LogInformation("提醒生成完成");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "数据库更新错误：{Message}", dbEx.Message);
            if (dbEx.InnerException != null)
            {
                _logger.LogError(dbEx.InnerException, "内部异常：{Message}", dbEx.InnerException.Message);
            }
            throw;
        }
        catch (System.Data.Common.DbException dbEx)
        {
            _logger.LogError(dbEx, "数据库连接错误：{Message}", dbEx.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成提醒时发生错误：{Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// 测试数据库连接
    /// </summary>
    private async Task<bool> TestDatabaseConnectionAsync()
    {
        try
        {
            // 尝试执行一个简单的查询来测试连接
            await _context.Database.CanConnectAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据库连接测试失败：{Message}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 生成项目截止前3天提醒
    /// </summary>
    private async Task GenerateProjectDueSoonNotificationsAsync(DateOnly today, DateTime now)
    {
        try
        {
            // 查询符合条件的项目：DeadlineAt 不为空，且 Status != 已完成(1)/已取消(3)
            // 且今天 >= (DeadlineAt - 3天) 且未过期
            var threeDaysLater = now.AddDays(3).Date;
            var projects = await _context.SideProjects
                .Where(p => p.DeadlineAt.HasValue
                    && p.Status != 1 // 已完成
                    && p.Status != 3 // 已取消（假设3是已取消）
                    && p.DeadlineAt.Value.Date >= now.Date
                    && p.DeadlineAt.Value.Date <= threeDaysLater)
                .ToListAsync();

            foreach (var project in projects)
            {
                if (!project.DeadlineAt.HasValue) continue;

                var daysUntilDeadline = (project.DeadlineAt.Value.Date - now.Date).Days;
                if (daysUntilDeadline < 0 || daysUntilDeadline > 3) continue;

                // 检查是否已存在今天的提醒（去重）
                var existingNotification = await _context.SideNotifications
                    .FirstOrDefaultAsync(n => n.Type == NotificationType.ProjectDueSoon
                        && n.EntityType == "SideProject"
                        && n.EntityId == project.Id
                        && n.OccurDate == today
                        && !n.IsDismissed
                        && (n.SnoozeUntil == null || n.SnoozeUntil <= now));

                var payload = new
                {
                    ProjectId = project.Id,
                    ProjectTitle = project.Title,
                    DeadlineAt = project.DeadlineAt.Value.ToString("yyyy-MM-dd"),
                    DaysUntilDeadline = daysUntilDeadline,
                    NextAction = project.NextAction ?? "请更新下一步"
                };

                if (existingNotification != null)
                {
                    // 更新现有提醒
                    existingNotification.LastTriggeredAt = now;
                    existingNotification.TriggerCount++;
                    existingNotification.PayloadJson = JsonSerializer.Serialize(payload);
                    existingNotification.Title = $"项目即将到期：{project.Title}";
                    existingNotification.Content = $"距离截止日期还有 {daysUntilDeadline} 天（{project.DeadlineAt.Value:yyyy-MM-dd}），下一步：{project.NextAction ?? "请更新下一步"}";
                    existingNotification.Severity = daysUntilDeadline == 0 ? NotificationSeverity.Danger : NotificationSeverity.Warning;
                }
                else
                {
                    // 创建新提醒
                    var notification = new SideNotification
                    {
                        Type = NotificationType.ProjectDueSoon,
                        Title = $"项目即将到期：{project.Title}",
                        Content = $"距离截止日期还有 {daysUntilDeadline} 天（{project.DeadlineAt.Value:yyyy-MM-dd}），下一步：{project.NextAction ?? "请更新下一步"}",
                        Severity = daysUntilDeadline == 0 ? NotificationSeverity.Danger : NotificationSeverity.Warning,
                        EntityType = "SideProject",
                        EntityId = project.Id,
                        PayloadJson = JsonSerializer.Serialize(payload),
                        OccurDate = today,
                        FirstTriggeredAt = now,
                        LastTriggeredAt = now,
                        TriggerCount = 1
                    };

                    _context.SideNotifications.Add(notification);
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成项目截止前3天提醒时发生错误：{Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// 生成任务到期当天提醒
    /// </summary>
    private async Task GenerateTaskDueTodayNotificationsAsync(DateOnly today, DateTime now)
    {
        try
        {
            // 查询今天到期的任务，且状态不是已完成(2)
            var todayStart = today.ToDateTime(TimeOnly.MinValue);
            var todayEnd = today.ToDateTime(TimeOnly.MaxValue);

            var tasks = await _context.SideProjectTasks
                .Include(t => t.Project)
                .Where(t => t.DueAt.HasValue
                    && t.DueAt.Value >= todayStart
                    && t.DueAt.Value <= todayEnd
                    && t.Status != 2) // 2 = 已完成
                .ToListAsync();

            foreach (var task in tasks)
            {
                if (!task.DueAt.HasValue) continue;

                // 检查是否已存在今天的提醒（去重）
                var existingNotification = await _context.SideNotifications
                    .FirstOrDefaultAsync(n => n.Type == NotificationType.TaskDueToday
                        && n.EntityType == "SideProjectTask"
                        && n.EntityId == task.Id
                        && n.OccurDate == today
                        && !n.IsDismissed
                        && (n.SnoozeUntil == null || n.SnoozeUntil <= now));

                var payload = new
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    ProjectId = task.ProjectId,
                    ProjectTitle = task.Project?.Title ?? "",
                    DueAt = task.DueAt.Value.ToString("yyyy-MM-dd")
                };

                if (existingNotification != null)
                {
                    // 更新现有提醒
                    existingNotification.LastTriggeredAt = now;
                    existingNotification.TriggerCount++;
                    existingNotification.PayloadJson = JsonSerializer.Serialize(payload);
                }
                else
                {
                    // 创建新提醒
                    var notification = new SideNotification
                    {
                        Type = NotificationType.TaskDueToday,
                        Title = $"任务今天到期：{task.Title}",
                        Content = $"所属项目：{task.Project?.Title ?? "未知"}，到期日：{task.DueAt.Value:yyyy-MM-dd}，请及时处理",
                        Severity = NotificationSeverity.Warning,
                        EntityType = "SideProjectTask",
                        EntityId = task.Id,
                        PayloadJson = JsonSerializer.Serialize(payload),
                        OccurDate = today,
                        FirstTriggeredAt = now,
                        LastTriggeredAt = now,
                        TriggerCount = 1
                    };

                    _context.SideNotifications.Add(notification);
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成任务到期当天提醒时发生错误：{Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// 生成项目卡住超过2天提醒
    /// </summary>
    private async Task GenerateProjectBlockedTooLongNotificationsAsync(DateOnly today, DateTime now)
    {
        try
        {
            // 查询卡住超过2天的项目
            var twoDaysAgo = now.AddDays(-2);
            var projects = await _context.SideProjects
                .Where(p => p.Blocked
                    && p.BlockedAt.HasValue
                    && p.BlockedAt.Value <= twoDaysAgo
                    && p.Status != 1 // 已完成
                    && p.Status != 3) // 已取消（假设3是已取消）
                .ToListAsync();

            foreach (var project in projects)
            {
                if (!project.BlockedAt.HasValue) continue;

                var blockedDays = (now - project.BlockedAt.Value).Days;
                if (blockedDays < 2) continue;

                // 检查是否已存在今天的提醒（去重）
                var existingNotification = await _context.SideNotifications
                    .FirstOrDefaultAsync(n => n.Type == NotificationType.ProjectBlockedTooLong
                        && n.EntityType == "SideProject"
                        && n.EntityId == project.Id
                        && n.OccurDate == today
                        && !n.IsDismissed
                        && (n.SnoozeUntil == null || n.SnoozeUntil <= now));

                var payload = new
                {
                    ProjectId = project.Id,
                    ProjectTitle = project.Title,
                    BlockedDays = blockedDays,
                    BlockReason = project.BlockReason ?? "未填写"
                };

                if (existingNotification != null)
                {
                    // 更新现有提醒
                    existingNotification.LastTriggeredAt = now;
                    existingNotification.TriggerCount++;
                    existingNotification.PayloadJson = JsonSerializer.Serialize(payload);
                    existingNotification.Title = $"项目卡住超过 {blockedDays} 天：{project.Title}";
                    existingNotification.Content = $"阻塞原因：{project.BlockReason ?? "未填写"}，建议：记录下一步并推动确认";
                }
                else
                {
                    // 创建新提醒
                    var notification = new SideNotification
                    {
                        Type = NotificationType.ProjectBlockedTooLong,
                        Title = $"项目卡住超过 {blockedDays} 天：{project.Title}",
                        Content = $"阻塞原因：{project.BlockReason ?? "未填写"}，建议：记录下一步并推动确认",
                        Severity = NotificationSeverity.Danger,
                        EntityType = "SideProject",
                        EntityId = project.Id,
                        PayloadJson = JsonSerializer.Serialize(payload),
                        OccurDate = today,
                        FirstTriggeredAt = now,
                        LastTriggeredAt = now,
                        TriggerCount = 1
                    };

                    _context.SideNotifications.Add(notification);
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成项目卡住超过2天提醒时发生错误：{Message}", ex.Message);
            throw;
        }
    }
}

