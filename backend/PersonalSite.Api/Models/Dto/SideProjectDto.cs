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
    public string? TechStack { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public decimal? PriceFinal { get; set; }
    public int Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
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
    public string? TechStack { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public decimal? PriceFinal { get; set; }
    public int Status { get; set; } = 0;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsPublic { get; set; } = false;
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
    public string? TechStack { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public decimal? PriceFinal { get; set; }
    public int? Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool? IsPublic { get; set; }
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

