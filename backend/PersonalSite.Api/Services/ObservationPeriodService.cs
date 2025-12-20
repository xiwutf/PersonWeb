using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services;

/// <summary>
/// 观察期提醒服务
/// </summary>
public class ObservationPeriodService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ObservationPeriodService> _logger;

    public ObservationPeriodService(
        AppDbContext context,
        ILogger<ObservationPeriodService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 检查是否应该建议进入观察期
    /// </summary>
    public async Task<ObservationSuggestion?> CheckObservationSuggestionAsync(string? userId, Guid personId)
    {
        var query = _context.RelationPersons
            .Include(p => p.Interactions.OrderByDescending(i => i.OccurredAt).Take(10))
            .Where(p => p.Id == personId);
            
        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(p => p.UserId == userId);
        }
        
        var person = await query.FirstOrDefaultAsync();

        if (person == null || person.Stage == 5 || person.Stage == 6) // 已在观察期或已结束
        {
            return null;
        }

        var reasons = new List<string>();

        // 条件1：长时间未联系（14天以上）
        if (person.LastContactAt.HasValue)
        {
            var daysSinceContact = (DateTime.Now - person.LastContactAt.Value).TotalDays;
            if (daysSinceContact >= 14)
            {
                reasons.Add($"已 {Math.Floor(daysSinceContact)} 天未联系");
            }
        }
        else if (person.CreatedAt < DateTime.Now.AddDays(-7))
        {
            // 创建7天后仍未联系
            var daysSinceCreated = (DateTime.Now - person.CreatedAt).TotalDays;
            reasons.Add($"创建后 {Math.Floor(daysSinceCreated)} 天仍未联系");
        }

        // 条件2：最近互动质量下降（负面感受增多）
        var recentInteractions = person.Interactions
            .Where(i => i.OccurredAt >= DateTime.Now.AddDays(-30))
            .ToList();

        if (recentInteractions.Count >= 3)
        {
            var negativeCount = recentInteractions.Count(i => i.MyFeeling == 2);
            var positiveCount = recentInteractions.Count(i => i.MyFeeling == 0);
            
            if (negativeCount > positiveCount && negativeCount >= 2)
            {
                reasons.Add("最近互动中负面感受较多");
            }
        }

        // 条件3：热度分数持续下降
        if (person.HeatScore < 30 && person.Stage >= 3) // 已见面但热度很低
        {
            reasons.Add("热度分数较低");
        }

        if (reasons.Any())
        {
            return new ObservationSuggestion
            {
                PersonId = personId,
                Reasons = reasons,
                SuggestedDurationDays = 7 // 默认观察7天
            };
        }

        return null;
    }

    /// <summary>
    /// 开始观察期
    /// </summary>
    public async Task<bool> StartObservationPeriodAsync(string? userId, Guid personId, string? reason, int? durationDays = null)
    {
        var query = _context.RelationPersons.Where(p => p.Id == personId);
        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(p => p.UserId == userId);
        }
        var person = await query.FirstOrDefaultAsync();

        if (person == null || person.Stage == 6) // 已结束的不再进入观察期
        {
            return false;
        }

        var now = DateTime.Now;
        var expectedEnd = now.AddDays(durationDays ?? 7);

        person.Stage = 5; // 观察期
        person.ObservationStartedAt = now;
        person.ObservationExpectedEndAt = expectedEnd;
        person.ObservationReason = reason;
        person.ObservationDecisionPending = false; // 刚开始，还未到决策时间
        person.ObservationLastRemindedAt = null;
        person.UpdatedAt = now;

        await _context.SaveChangesAsync();
        _logger.LogInformation("开始观察期: PersonId={PersonId}, Reason={Reason}, ExpectedEnd={ExpectedEnd}", 
            personId, reason, expectedEnd);

        return true;
    }

    /// <summary>
    /// 检查观察期状态并返回需要提醒的对象
    /// </summary>
    public async Task<List<ObservationReminder>> GetObservationRemindersAsync(string? userId)
    {
        var now = DateTime.Now;
        var query = _context.RelationPersons
            .Where(p => p.Stage == 5 && // 观察期
                       p.ObservationStartedAt.HasValue);
        
        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(p => p.UserId == userId);
        }
        
        var persons = await query.ToListAsync();

        var reminders = new List<ObservationReminder>();

        foreach (var person in persons)
        {
            if (!person.ObservationExpectedEndAt.HasValue)
                continue;

            var daysInObservation = (now - person.ObservationStartedAt!.Value).TotalDays;
            var daysUntilEnd = (person.ObservationExpectedEndAt.Value - now).TotalDays;

            // 需要决策：观察期已结束
            if (person.ObservationExpectedEndAt.Value <= now && person.ObservationDecisionPending)
            {
                reminders.Add(new ObservationReminder
                {
                    PersonId = person.Id,
                    PersonNickname = person.Nickname,
                    Type = ObservationReminderType.DecisionRequired,
                    DaysInObservation = (int)Math.Floor(daysInObservation),
                    Reason = person.ObservationReason
                });
            }
            // 需要提醒：观察期即将结束（剩余2天或更少）
            else if (daysUntilEnd <= 2 && daysUntilEnd > 0)
            {
                // 如果上次提醒是1天前或更早，或者从未提醒过，则需要提醒
                if (!person.ObservationLastRemindedAt.HasValue ||
                    (now - person.ObservationLastRemindedAt.Value).TotalDays >= 1)
                {
                    reminders.Add(new ObservationReminder
                    {
                        PersonId = person.Id,
                        PersonNickname = person.Nickname,
                        Type = ObservationReminderType.EndingSoon,
                        DaysInObservation = (int)Math.Floor(daysInObservation),
                        DaysUntilEnd = (int)Math.Ceiling(daysUntilEnd),
                        Reason = person.ObservationReason
                    });
                }
            }
            // 持续提醒：每3天提醒一次观察期状态
            else if (daysUntilEnd > 2)
            {
                if (!person.ObservationLastRemindedAt.HasValue ||
                    (now - person.ObservationLastRemindedAt.Value).TotalDays >= 3)
                {
                    reminders.Add(new ObservationReminder
                    {
                        PersonId = person.Id,
                        PersonNickname = person.Nickname,
                        Type = ObservationReminderType.OnGoing,
                        DaysInObservation = (int)Math.Floor(daysInObservation),
                        DaysUntilEnd = (int)Math.Ceiling(daysUntilEnd),
                        Reason = person.ObservationReason
                    });
                }
            }
        }

        return reminders;
    }

    /// <summary>
    /// 标记观察期提醒已查看
    /// </summary>
    public async Task MarkReminderViewedAsync(string? userId, Guid personId)
    {
        var query = _context.RelationPersons.Where(p => p.Id == personId);
        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(p => p.UserId == userId);
        }
        var person = await query.FirstOrDefaultAsync();

        if (person != null && person.Stage == 5)
        {
            person.ObservationLastRemindedAt = DateTime.Now;
            person.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 处理观察期结束决策
    /// </summary>
    public async Task<bool> HandleObservationDecisionAsync(
        string? userId,
        Guid personId,
        ObservationDecision decision,
        string? reason = null)
    {
        var query = _context.RelationPersons.Where(p => p.Id == personId);
        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(p => p.UserId == userId);
        }
        var person = await query.FirstOrDefaultAsync();

        if (person == null || person.Stage != 5 || !person.ObservationDecisionPending)
        {
            return false;
        }

        var now = DateTime.Now;

        switch (decision)
        {
            case ObservationDecision.Continue:
                // 继续：退出观察期，恢复到之前的阶段（根据实际情况判断，这里简化为"熟悉中"）
                person.Stage = 1; // 熟悉中
                person.ObservationDecisionPending = false;
                person.ObservationStartedAt = null;
                person.ObservationExpectedEndAt = null;
                person.ObservationReason = null;
                break;

            case ObservationDecision.Downgrade:
                // 降级：保持观察期，延长观察时间
                person.ObservationExpectedEndAt = now.AddDays(7); // 再观察7天
                person.ObservationDecisionPending = false;
                person.ObservationReason = reason ?? person.ObservationReason;
                break;

            case ObservationDecision.End:
                // 结束：标记为已结束
                person.Stage = 6; // 已结束
                person.ObservationDecisionPending = false;
                person.ObservationStartedAt = null;
                person.ObservationExpectedEndAt = null;
                if (!string.IsNullOrEmpty(reason))
                {
                    person.ObservationReason = reason;
                }
                break;
        }

        person.UpdatedAt = now;
        await _context.SaveChangesAsync();

        _logger.LogInformation("观察期决策完成: PersonId={PersonId}, Decision={Decision}, Reason={Reason}",
            personId, decision, reason);

        return true;
    }

    /// <summary>
    /// 标记观察期需要决策（当观察期结束时调用）
    /// </summary>
    public async Task MarkDecisionPendingAsync(string? userId)
    {
        var now = DateTime.Now;
        var query = _context.RelationPersons
            .Where(p => p.Stage == 5 &&
                       p.ObservationExpectedEndAt.HasValue &&
                       p.ObservationExpectedEndAt.Value <= now &&
                       !p.ObservationDecisionPending);
        
        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(p => p.UserId == userId);
        }
        
        var persons = await query.ToListAsync();

        foreach (var person in persons)
        {
            person.ObservationDecisionPending = true;
            person.UpdatedAt = now;
        }

        if (persons.Any())
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("标记 {Count} 个对象需要观察期决策", persons.Count);
        }
    }
}

/// <summary>
/// 观察期建议
/// </summary>
public class ObservationSuggestion
{
    public Guid PersonId { get; set; }
    public List<string> Reasons { get; set; } = new();
    public int SuggestedDurationDays { get; set; } = 7;
}

/// <summary>
/// 观察期提醒
/// </summary>
public class ObservationReminder
{
    public Guid PersonId { get; set; }
    public string PersonNickname { get; set; } = string.Empty;
    public ObservationReminderType Type { get; set; }
    public int DaysInObservation { get; set; }
    public int? DaysUntilEnd { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// 观察期提醒类型
/// </summary>
public enum ObservationReminderType
{
    OnGoing,        // 持续观察中
    EndingSoon,     // 即将结束
    DecisionRequired // 需要决策
}

/// <summary>
/// 观察期结束决策
/// </summary>
public enum ObservationDecision
{
    Continue,   // 继续（退出观察期）
    Downgrade,  // 降级（延长观察期）
    End         // 结束（标记为已结束）
}

