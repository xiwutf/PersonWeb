namespace PersonalSite.Api.Models.Enums;

/// <summary>
/// 提醒类型枚举
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// 项目即将到期（截止前3天）
    /// </summary>
    ProjectDueSoon = 1,

    /// <summary>
    /// 任务今天到期
    /// </summary>
    TaskDueToday = 2,

    /// <summary>
    /// 项目卡住超过2天
    /// </summary>
    ProjectBlockedTooLong = 3
}

