namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 数据分析汇总响应 DTO
/// </summary>
public class SideProjectAnalyticsSummaryDto
{
    /// <summary>
    /// KPI 指标
    /// </summary>
    public AnalyticsKpisDto Kpis { get; set; } = new();

    /// <summary>
    /// 项目状态聚合
    /// </summary>
    public List<StatusAggregationDto> StatusAgg { get; set; } = new();

    /// <summary>
    /// 月度收入趋势
    /// </summary>
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();

    /// <summary>
    /// 交付周期统计
    /// </summary>
    public List<DeliveryCycleDto> DeliveryCycle { get; set; } = new();

    /// <summary>
    /// 客户贡献 Top10
    /// </summary>
    public List<CustomerTopDto> CustomerTop { get; set; } = new();

    /// <summary>
    /// 即将到期项目（<=7天）
    /// </summary>
    public List<ProjectBriefDto> RiskDueSoon { get; set; } = new();

    /// <summary>
    /// 逾期项目列表
    /// </summary>
    public List<ProjectBriefDto> RiskOverdue { get; set; } = new();
}

/// <summary>
/// KPI 指标 DTO
/// </summary>
public class AnalyticsKpisDto
{
    /// <summary>
    /// 项目总数
    /// </summary>
    public int Totals { get; set; }

    /// <summary>
    /// 进行中项目数
    /// </summary>
    public int InProgressCount { get; set; }

    /// <summary>
    /// 逾期项目数
    /// </summary>
    public int OverdueCount { get; set; }

    /// <summary>
    /// 卡住项目数
    /// </summary>
    public int BlockedCount { get; set; }

    /// <summary>
    /// 本期已收金额
    /// </summary>
    public decimal ReceivedSum { get; set; }

    /// <summary>
    /// 本期待收金额
    /// </summary>
    public decimal ReceivableSum { get; set; }
}

/// <summary>
/// 状态聚合 DTO
/// </summary>
public class StatusAggregationDto
{
    /// <summary>
    /// 状态值（0=进行中, 1=已完成, 2=待付款, 3=已取消）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// 项目数量
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 金额汇总
    /// </summary>
    public decimal AmountSum { get; set; }
}

/// <summary>
/// 月度收入 DTO
/// </summary>
public class MonthlyRevenueDto
{
    /// <summary>
    /// 月份（格式：YYYY-MM）
    /// </summary>
    public string Month { get; set; } = string.Empty;

    /// <summary>
    /// 已收金额汇总
    /// </summary>
    public decimal ReceivedSum { get; set; }
}

/// <summary>
/// 交付周期 DTO
/// </summary>
public class DeliveryCycleDto
{
    /// <summary>
    /// 分组名称（项目类型或时间段）
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// 平均交付天数
    /// </summary>
    public double AvgDays { get; set; }

    /// <summary>
    /// 项目数量
    /// </summary>
    public int Count { get; set; }
}

/// <summary>
/// 客户贡献 Top DTO
/// </summary>
public class CustomerTopDto
{
    /// <summary>
    /// 客户名称
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 项目数量
    /// </summary>
    public int ProjectCount { get; set; }

    /// <summary>
    /// 已收金额汇总
    /// </summary>
    public decimal ReceivedSum { get; set; }
}

/// <summary>
/// 项目简要信息 DTO（用于风险列表）
/// </summary>
public class ProjectBriefDto
{
    /// <summary>
    /// 项目ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// 截止日期
    /// </summary>
    public DateTime? DeadlineAt { get; set; }

    /// <summary>
    /// 剩余天数（负数表示已逾期）
    /// </summary>
    public int? DaysRemaining { get; set; }

    /// <summary>
    /// 下一步行动
    /// </summary>
    public string? NextAction { get; set; }

    /// <summary>
    /// 项目金额
    /// </summary>
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// 阻塞原因
    /// </summary>
    public string? BlockReason { get; set; }

    /// <summary>
    /// 逾期天数（仅逾期项目有值）
    /// </summary>
    public int? OverdueDays { get; set; }
}

