using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>
/// 副业项目数据分析服务
/// </summary>
public class SideProjectAnalyticsService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SideProjectAnalyticsService> _logger;

    public SideProjectAnalyticsService(AppDbContext context, ILogger<SideProjectAnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取数据分析汇总
    /// </summary>
    public async Task<SideProjectAnalyticsSummaryDto> GetAnalyticsSummaryAsync(
        DateTime? startDate,
        DateTime? endDate,
        string? projectType,
        bool? isPublic)
    {
        var now = DateTime.Now;

        // 构建基础查询
        var query = _context.SideProjects.AsQueryable();

        // 时间范围筛选：使用 StartTime、EndTime 或 CreatedAt
        if (startDate.HasValue || endDate.HasValue)
        {
            query = query.Where(p =>
                (startDate == null || (p.StartTime ?? p.CreatedAt) >= startDate.Value) &&
                (endDate == null || (p.StartTime ?? p.CreatedAt) <= endDate.Value));
        }

        // 项目类型筛选
        if (!string.IsNullOrEmpty(projectType))
        {
            query = query.Where(p => p.Category == projectType || p.IncomeType == projectType);
        }

        // 是否公开筛选
        if (isPublic.HasValue)
        {
            query = query.Where(p => p.IsPublic == isPublic.Value);
        }

        var projects = await query.ToListAsync();

        // 计算 KPI
        var kpis = new AnalyticsKpisDto
        {
            Totals = projects.Count,
            InProgressCount = projects.Count(p => p.Status == 0),
            OverdueCount = projects.Count(p => p.DeadlineAt.HasValue && p.DeadlineAt.Value < now && p.Status != 1 && p.Status != 3),
            BlockedCount = projects.Count(p => p.Blocked),
            ReceivedSum = projects.Sum(p => p.ReceivedAmount ?? 0),
            ReceivableSum = projects
                .Where(p => p.Status != 1 && p.Status != 3 && p.TotalAmount.HasValue)
                .Sum(p => (p.TotalAmount ?? 0) - (p.ReceivedAmount ?? 0))
        };

        // 状态聚合
        var statusAgg = projects
            .GroupBy(p => p.Status)
            .Select(g => new StatusAggregationDto
            {
                Status = g.Key,
                StatusName = GetStatusName(g.Key),
                Count = g.Count(),
                AmountSum = g.Sum(p => p.TotalAmount ?? 0)
            })
            .OrderBy(s => s.Status)
            .ToList();

        // 月度收入趋势（近12个月）
        var monthlyRevenue = await GetMonthlyRevenueAsync(startDate, endDate, projectType, isPublic);

        // 交付周期统计（按项目类型）
        var deliveryCycle = projects
            .Where(p => p.StartTime.HasValue && (p.EndTime.HasValue || p.CompletedAt.HasValue))
            .GroupBy(p => p.Category ?? "未分类")
            .Select(g => new DeliveryCycleDto
            {
                GroupName = g.Key,
                AvgDays = g.Average(p =>
                {
                    var endDate = p.EndTime ?? p.CompletedAt;
                    if (!p.StartTime.HasValue || !endDate.HasValue) return 0;
                    return (endDate.Value - p.StartTime.Value).TotalDays;
                }),
                Count = g.Count()
            })
            .Where(d => d.AvgDays > 0)
            .OrderByDescending(d => d.Count)
            .Take(10)
            .ToList();

        // 客户贡献 Top10
        var customerTop = projects
            .Where(p => !string.IsNullOrEmpty(p.ClientName) && p.ReceivedAmount.HasValue && p.ReceivedAmount > 0)
            .GroupBy(p => p.ClientName!)
            .Select(g => new CustomerTopDto
            {
                CustomerName = g.Key,
                ProjectCount = g.Count(),
                ReceivedSum = g.Sum(p => p.ReceivedAmount ?? 0)
            })
            .OrderByDescending(c => c.ReceivedSum)
            .Take(10)
            .ToList();

        // 即将到期项目（<=7天）
        var sevenDaysLater = now.AddDays(7);
        var riskDueSoon = projects
            .Where(p => p.DeadlineAt.HasValue
                && p.DeadlineAt.Value >= now
                && p.DeadlineAt.Value <= sevenDaysLater
                && p.Status != 1 && p.Status != 3)
            .Select(p => new ProjectBriefDto
            {
                Id = p.Id,
                Title = p.Title,
                ClientName = p.ClientName,
                DeadlineAt = p.DeadlineAt,
                DaysRemaining = (int)Math.Ceiling((p.DeadlineAt!.Value - now).TotalDays),
                NextAction = p.NextAction,
                TotalAmount = p.TotalAmount
            })
            .OrderBy(p => p.DeadlineAt)
            .ToList();

        // 逾期项目
        var riskOverdue = projects
            .Where(p => p.DeadlineAt.HasValue
                && p.DeadlineAt.Value < now
                && p.Status != 1 && p.Status != 3)
            .Select(p => new ProjectBriefDto
            {
                Id = p.Id,
                Title = p.Title,
                ClientName = p.ClientName,
                DeadlineAt = p.DeadlineAt,
                OverdueDays = (int)Math.Ceiling((now - p.DeadlineAt!.Value).TotalDays),
                BlockReason = p.BlockReason,
                TotalAmount = p.TotalAmount
            })
            .OrderByDescending(p => p.OverdueDays)
            .ToList();

        return new SideProjectAnalyticsSummaryDto
        {
            Kpis = kpis,
            StatusAgg = statusAgg,
            MonthlyRevenue = monthlyRevenue,
            DeliveryCycle = deliveryCycle,
            CustomerTop = customerTop,
            RiskDueSoon = riskDueSoon,
            RiskOverdue = riskOverdue
        };
    }

    /// <summary>
    /// 获取月度收入趋势（近12个月）
    /// </summary>
    private async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(
        DateTime? startDate,
        DateTime? endDate,
        string? projectType,
        bool? isPublic)
    {
        var query = _context.SideProjects.AsQueryable();

        // 应用筛选条件
        if (startDate.HasValue || endDate.HasValue)
        {
            query = query.Where(p =>
                (startDate == null || (p.StartTime ?? p.CreatedAt) >= startDate.Value) &&
                (endDate == null || (p.StartTime ?? p.CreatedAt) <= endDate.Value));
        }

        if (!string.IsNullOrEmpty(projectType))
        {
            query = query.Where(p => p.Category == projectType || p.IncomeType == projectType);
        }

        if (isPublic.HasValue)
        {
            query = query.Where(p => p.IsPublic == isPublic.Value);
        }

        // 获取近12个月的数据
        var twelveMonthsAgo = DateTime.Now.AddMonths(-12);
        var projects = await query
            .Where(p => p.ReceivedAmount.HasValue && p.ReceivedAmount > 0)
            .Where(p => (p.CompletedAt ?? p.UpdatedAt) >= twelveMonthsAgo)
            .ToListAsync();

        // 按月份分组
        var monthlyData = projects
            .GroupBy(p =>
            {
                var date = p.CompletedAt ?? p.UpdatedAt;
                return date.ToString("yyyy-MM");
            })
            .Select(g => new MonthlyRevenueDto
            {
                Month = g.Key,
                ReceivedSum = g.Sum(p => p.ReceivedAmount ?? 0)
            })
            .OrderBy(m => m.Month)
            .ToList();

        // 确保近12个月都有数据（缺失的月份补0）
        var result = new List<MonthlyRevenueDto>();
        for (int i = 11; i >= 0; i--)
        {
            var month = DateTime.Now.AddMonths(-i).ToString("yyyy-MM");
            var existing = monthlyData.FirstOrDefault(m => m.Month == month);
            result.Add(existing ?? new MonthlyRevenueDto { Month = month, ReceivedSum = 0 });
        }

        return result;
    }

    /// <summary>
    /// 获取状态名称
    /// </summary>
    private string GetStatusName(int status)
    {
        return status switch
        {
            0 => "进行中",
            1 => "已完成",
            2 => "待付款",
            3 => "已取消",
            _ => "未知"
        };
    }
}

