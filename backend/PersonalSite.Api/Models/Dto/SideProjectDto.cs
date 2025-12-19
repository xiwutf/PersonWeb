namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 兼职项目 DTO（用于列表/详情返回）
/// </summary>
public class SideProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ClientName { get; set; }
    public string? ClientContact { get; set; }
    public string? Source { get; set; }
    public string? Category { get; set; }
    public string? IncomeType { get; set; }
    public string? TechStack { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public decimal? PriceFinal { get; set; }
    public int Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsPublic { get; set; }
    public string? Stage { get; set; }
    public int? Progress { get; set; }
    public bool IsProgressManual { get; set; }
    public int? Priority { get; set; }
    public DateTime? DeadlineAt { get; set; }
    public string? NextAction { get; set; }
    public bool Blocked { get; set; }
    public string? BlockReason { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? ReceivedAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// 创建兼职项目 DTO
/// </summary>
public class CreateSideProjectDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ClientName { get; set; }
    public string? ClientContact { get; set; }
    public string? Source { get; set; }
    public string? Category { get; set; }
    public string? IncomeType { get; set; }
    public string? TechStack { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public decimal? PriceFinal { get; set; }
    public int Status { get; set; } = 0;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsPublic { get; set; } = false;
    public string? Stage { get; set; }
    public int? Progress { get; set; }
    public bool IsProgressManual { get; set; } = false;
    public int? Priority { get; set; }
    public DateTime? DeadlineAt { get; set; }
    public string? NextAction { get; set; }
    public bool Blocked { get; set; } = false;
    public string? BlockReason { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? ReceivedAmount { get; set; }
}

/// <summary>
/// 更新兼职项目 DTO
/// </summary>
public class UpdateSideProjectDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ClientName { get; set; }
    public string? ClientContact { get; set; }
    public string? Source { get; set; }
    public string? Category { get; set; }
    public string? IncomeType { get; set; }
    public string? TechStack { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public decimal? PriceFinal { get; set; }
    public int? Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool? IsPublic { get; set; }
    public string? Stage { get; set; }
    public int? Progress { get; set; }
    public bool? IsProgressManual { get; set; }
    public int? Priority { get; set; }
    public DateTime? DeadlineAt { get; set; }
    public string? NextAction { get; set; }
    public bool? Blocked { get; set; }
    public string? BlockReason { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? ReceivedAmount { get; set; }
}

/// <summary>
/// 看板汇总数据 DTO
/// </summary>
public class ProjectDashboardSummaryDto
{
    public decimal TotalIncome { get; set; }
    public int TotalProjects { get; set; }
    public decimal AvgProjectPrice { get; set; }
    public double AvgDurationDays { get; set; }
    // 按收入类型分类统计
    public decimal DevelopmentIncome { get; set; } // 软件开发收入
    public int DevelopmentProjects { get; set; } // 软件开发项目数
    public decimal InvestmentIncome { get; set; } // 投资收入
    public int InvestmentProjects { get; set; } // 投资项目数
}

/// <summary>
/// 收入趋势点 DTO
/// </summary>
public class IncomeTrendPointDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Income { get; set; }
}

/// <summary>
/// 类型分布项 DTO
/// </summary>
public class CategoryDistributionItemDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Income { get; set; }
}

/// <summary>
/// 技术栈分布项 DTO
/// </summary>
public class TechStackDistributionItemDto
{
    public string Tech { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Income { get; set; }
}

/// <summary>
/// 客户来源项 DTO
/// </summary>
public class ClientSourceItemDto
{
    public string Source { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Income { get; set; }
}

/// <summary>
/// 项目周期分布项 DTO
/// </summary>
public class DurationBucketItemDto
{
    public string BucketName { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// 项目需求 DTO
/// </summary>
public class SideProjectRequirementDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string? ScopeIn { get; set; }
    public string? ScopeOut { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Deliverables { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 项目任务 DTO
/// </summary>
public class SideProjectTaskDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public int? Priority { get; set; }
    public DateTime? DueAt { get; set; }
    public decimal? EstHours { get; set; }
    public decimal? ActHours { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 项目里程碑 DTO
/// </summary>
public class SideProjectMilestoneDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? DueAt { get; set; }
    public int Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 项目沟通记录 DTO
/// </summary>
public class SideProjectLogDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string? Channel { get; set; }
    public string? Content { get; set; }
    public string? NextTodo { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 项目附件 DTO
/// </summary>
public class SideProjectAttachmentDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string? Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 项目详情 DTO（包含所有子实体）
/// </summary>
public class SideProjectDetailDto : SideProjectDto
{
    public List<SideProjectRequirementDto> Requirements { get; set; } = new();
    public List<SideProjectTaskDto> Tasks { get; set; } = new();
    public List<SideProjectMilestoneDto> Milestones { get; set; } = new();
    public List<SideProjectLogDto> Logs { get; set; } = new();
    public List<SideProjectAttachmentDto> Attachments { get; set; } = new();
}

/// <summary>
/// 仪表盘数据 DTO
/// </summary>
public class SideProjectDashboardDto
{
    public List<SideProjectTaskDto> TodayTasks { get; set; } = new(); // 今日待办（跨项目任务）
    public List<SideProjectDto> InProgressProjects { get; set; } = new(); // 进行中项目
    public List<SideProjectDto> BlockedProjects { get; set; } = new(); // 卡住项目
    public List<SideProjectMilestoneDto> ThisWeekMilestones { get; set; } = new(); // 本周里程碑
    public decimal TotalIncome { get; set; } // 收入汇总
    public decimal ThisMonthIncome { get; set; } // 本月收入
}

