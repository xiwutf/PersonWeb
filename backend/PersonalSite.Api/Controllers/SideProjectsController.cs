using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 兼职项目控制器
/// </summary>
[ApiController]
[Route("api/side-projects")]
public class SideProjectsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly SideProjectService _projectService;

    public SideProjectsController(AppDbContext context, SideProjectService projectService)
    {
        _context = context;
        _projectService = projectService;
    }

    /// <summary>
    /// 获取兼职项目列表（分页、筛选）
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? status = null,
        [FromQuery] string? category = null,
        [FromQuery] string? incomeType = null,
        [FromQuery] string? keyword = null)
    {
        var query = _context.SideProjects.AsQueryable();

        // 状态筛选
        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        // 类型筛选
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        // 收入类型筛选
        if (!string.IsNullOrEmpty(incomeType))
        {
            query = query.Where(p => p.IncomeType == incomeType);
        }

        // 关键词搜索
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(p => p.Title.Contains(keyword) || 
                (p.Description != null && p.Description.Contains(keyword)));
        }

        var total = await query.CountAsync();
        var projects = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = projects.Select(p => MapToDto(p)).ToList();

        return Ok(ApiResponse.Success(new { Total = total, List = items }));
    }

    /// <summary>
    /// 获取兼职项目详情
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectDto>>> GetDetail(int id)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<SideProjectDto>.Error("项目不存在", 404));
        }

        var dto = MapToDto(project);

        return Ok(ApiResponse<SideProjectDto>.Success(dto));
    }

    /// <summary>
    /// 创建兼职项目
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectDto>>> Create([FromBody] CreateSideProjectDto dto)
    {
        var project = new SideProject
        {
            Title = dto.Title,
            Description = dto.Description,
            ClientName = dto.ClientName,
            ClientContact = dto.ClientContact,
            Source = dto.Source,
            Category = dto.Category,
            IncomeType = dto.IncomeType ?? "development",
            TechStack = dto.TechStack,
            BudgetMin = dto.BudgetMin,
            BudgetMax = dto.BudgetMax,
            PriceFinal = dto.PriceFinal,
            Status = dto.Status,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsPublic = dto.IsPublic,
            Stage = dto.Stage,
            Progress = dto.Progress ?? 0,
            IsProgressManual = dto.IsProgressManual,
            Priority = dto.Priority,
            DeadlineAt = dto.DeadlineAt,
            NextAction = dto.NextAction,
            Blocked = dto.Blocked,
            BlockReason = dto.BlockReason,
            TotalAmount = dto.TotalAmount,
            ReceivedAmount = dto.ReceivedAmount,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.SideProjects.Add(project);
        await _context.SaveChangesAsync();

        // 同步状态和阶段
        await _projectService.SyncStatusAndStageAsync(project.Id);

        var result = MapToDto(project);

        return CreatedAtAction(nameof(GetDetail), new { id = project.Id }, ApiResponse<SideProjectDto>.Success(result));
    }

    /// <summary>
    /// 更新兼职项目
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectDto>>> Update(int id, [FromBody] UpdateSideProjectDto dto)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<SideProjectDto>.Error("项目不存在", 404));
        }

        // 更新字段（仅更新提供的字段）
        if (dto.Title != null) project.Title = dto.Title;
        if (dto.Description != null) project.Description = dto.Description;
        if (dto.ClientName != null) project.ClientName = dto.ClientName;
        if (dto.ClientContact != null) project.ClientContact = dto.ClientContact;
        if (dto.Source != null) project.Source = dto.Source;
        if (dto.Category != null) project.Category = dto.Category;
        if (dto.IncomeType != null) project.IncomeType = dto.IncomeType;
        if (dto.TechStack != null) project.TechStack = dto.TechStack;
        if (dto.BudgetMin.HasValue) project.BudgetMin = dto.BudgetMin;
        if (dto.BudgetMax.HasValue) project.BudgetMax = dto.BudgetMax;
        if (dto.PriceFinal.HasValue) project.PriceFinal = dto.PriceFinal;
        if (dto.Status.HasValue) project.Status = dto.Status.Value;
        if (dto.StartTime.HasValue) project.StartTime = dto.StartTime;
        if (dto.EndTime.HasValue) project.EndTime = dto.EndTime;
        if (dto.IsPublic.HasValue) project.IsPublic = dto.IsPublic.Value;
        if (dto.Stage != null) project.Stage = dto.Stage;
        if (dto.Progress.HasValue) project.Progress = dto.Progress;
        if (dto.IsProgressManual.HasValue) project.IsProgressManual = dto.IsProgressManual.Value;
        if (dto.Priority.HasValue) project.Priority = dto.Priority;
        if (dto.DeadlineAt.HasValue) project.DeadlineAt = dto.DeadlineAt;
        if (dto.NextAction != null) project.NextAction = dto.NextAction;
        if (dto.Blocked.HasValue) project.Blocked = dto.Blocked.Value;
        if (dto.BlockReason != null) project.BlockReason = dto.BlockReason;
        if (dto.TotalAmount.HasValue) project.TotalAmount = dto.TotalAmount;
        if (dto.ReceivedAmount.HasValue) project.ReceivedAmount = dto.ReceivedAmount;
        project.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        // 同步状态和阶段
        await _projectService.SyncStatusAndStageAsync(project.Id);

        var result = MapToDto(project);

        return Ok(ApiResponse<SideProjectDto>.Success(result));
    }

    /// <summary>
    /// 删除兼职项目
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<bool>.Error("项目不存在", 404));
        }

        _context.SideProjects.Remove(project);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<bool>.Success(true));
    }

    #region 数据看板接口

    /// <summary>
    /// 获取看板汇总数据
    /// </summary>
    [HttpGet("dashboard/summary")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ProjectDashboardSummaryDto>>> GetDashboardSummary(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? incomeType = null)
    {
        var query = _context.SideProjects.AsQueryable();

        // 时间范围筛选
        if (from.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= to.Value);
        }

        // 收入类型筛选
        if (!string.IsNullOrEmpty(incomeType))
        {
            query = query.Where(p => p.IncomeType == incomeType);
        }

        // 只统计已完成的项目
        var completedProjects = query.Where(p => p.Status == 1 && p.PriceFinal.HasValue);

        // 调试：检查总项目数
        var allProjectsCount = await query.CountAsync();
        var completedProjectsCount = await completedProjects.CountAsync();
        
        Console.WriteLine($"[Dashboard Summary] 总项目数: {allProjectsCount}, 已完成项目数: {completedProjectsCount}");
        Console.WriteLine($"[Dashboard Summary] 时间范围: from={from}, to={to}, incomeType={incomeType}");

        var totalIncome = await completedProjects
            .SumAsync(p => p.PriceFinal ?? 0);
        
        // 按收入类型分类统计
        var developmentProjects = completedProjects.Where(p => p.IncomeType == "development" || p.IncomeType == null);
        var investmentProjects = completedProjects.Where(p => p.IncomeType == "investment");
        
        var developmentIncome = await developmentProjects.SumAsync(p => p.PriceFinal ?? 0);
        var developmentCount = await developmentProjects.CountAsync();
        var investmentIncome = await investmentProjects.SumAsync(p => p.PriceFinal ?? 0);
        var investmentCount = await investmentProjects.CountAsync();

        var totalProjects = await completedProjects.CountAsync();

        var avgPrice = totalProjects > 0 
            ? await completedProjects.AverageAsync(p => p.PriceFinal ?? 0)
            : 0;

        // 计算平均项目周期（天数）
        var projectsWithDuration = await completedProjects
            .Where(p => p.StartTime.HasValue && p.EndTime.HasValue)
            .Select(p => new { p.StartTime, p.EndTime })
            .ToListAsync();

        var avgDurationDays = projectsWithDuration.Count > 0
            ? projectsWithDuration.Average(p => (p.EndTime!.Value - p.StartTime!.Value).TotalDays)
            : 0;

        var summary = new ProjectDashboardSummaryDto
        {
            TotalIncome = totalIncome,
            TotalProjects = totalProjects,
            AvgProjectPrice = (decimal)avgPrice,
            AvgDurationDays = avgDurationDays,
            DevelopmentIncome = developmentIncome,
            DevelopmentProjects = developmentCount,
            InvestmentIncome = investmentIncome,
            InvestmentProjects = investmentCount
        };

        Console.WriteLine($"[Dashboard Summary] 返回数据: TotalIncome={summary.TotalIncome}, TotalProjects={summary.TotalProjects}, AvgProjectPrice={summary.AvgProjectPrice}, AvgDurationDays={summary.AvgDurationDays}");

        return Ok(ApiResponse<ProjectDashboardSummaryDto>.Success(summary));
    }

    /// <summary>
    /// 获取收入趋势
    /// </summary>
    [HttpGet("dashboard/income-trend")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<IncomeTrendPointDto>>>> GetIncomeTrend(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string granularity = "month",
        [FromQuery] string? incomeType = null)
    {
        var query = _context.SideProjects
            .Where(p => p.Status == 1 && p.PriceFinal.HasValue && p.EndTime.HasValue);

        // 收入类型筛选
        if (!string.IsNullOrEmpty(incomeType))
        {
            query = query.Where(p => p.IncomeType == incomeType);
        }

        if (from.HasValue)
        {
            query = query.Where(p => p.EndTime >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(p => p.EndTime <= to.Value);
        }

        var projects = await query
            .Select(p => new { p.EndTime, p.PriceFinal })
            .ToListAsync();

        var trend = new List<IncomeTrendPointDto>();

        if (granularity == "day")
        {
            // 按天统计
            var grouped = projects
                .GroupBy(p => p.EndTime!.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => new IncomeTrendPointDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Income = g.Sum(p => p.PriceFinal ?? 0)
                });
            trend = grouped.ToList();
        }
        else
        {
            // 按月统计
            var grouped = projects
                .GroupBy(p => new { p.EndTime!.Value.Year, p.EndTime!.Value.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new IncomeTrendPointDto
                {
                    Date = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Income = g.Sum(p => p.PriceFinal ?? 0)
                });
            trend = grouped.ToList();
        }

        return Ok(ApiResponse<List<IncomeTrendPointDto>>.Success(trend));
    }

    /// <summary>
    /// 获取项目类型分布
    /// </summary>
    [HttpGet("dashboard/category-distribution")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<CategoryDistributionItemDto>>>> GetCategoryDistribution(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? incomeType = null)
    {
        var query = _context.SideProjects
            .Where(p => p.Status == 1 && p.PriceFinal.HasValue && !string.IsNullOrEmpty(p.Category));

        // 收入类型筛选
        if (!string.IsNullOrEmpty(incomeType))
        {
            query = query.Where(p => p.IncomeType == incomeType);
        }

        if (from.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= to.Value);
        }

        var distribution = await query
            .GroupBy(p => p.Category!)
            .Select(g => new CategoryDistributionItemDto
            {
                Category = g.Key,
                Count = g.Count(),
                Income = g.Sum(p => p.PriceFinal ?? 0)
            })
            .OrderByDescending(d => d.Income)
            .ToListAsync();

        return Ok(ApiResponse<List<CategoryDistributionItemDto>>.Success(distribution));
    }

    /// <summary>
    /// 获取技术栈分布
    /// </summary>
    [HttpGet("dashboard/tech-distribution")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<TechStackDistributionItemDto>>>> GetTechDistribution(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? incomeType = null)
    {
        var query = _context.SideProjects
            .Where(p => p.Status == 1 && p.PriceFinal.HasValue && !string.IsNullOrEmpty(p.TechStack));

        // 收入类型筛选（技术栈只统计软件开发）
        if (string.IsNullOrEmpty(incomeType))
        {
            query = query.Where(p => p.IncomeType == "development" || p.IncomeType == null);
        }
        else if (incomeType == "development")
        {
            query = query.Where(p => p.IncomeType == "development" || p.IncomeType == null);
        }
        // investment 类型不显示技术栈

        if (from.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= to.Value);
        }

        var projects = await query
            .Select(p => new { p.TechStack, p.PriceFinal })
            .ToListAsync();

        // 展开技术栈（逗号分隔）
        var techDict = new Dictionary<string, TechStackDistributionItemDto>();

        foreach (var project in projects)
        {
            if (string.IsNullOrEmpty(project.TechStack)) continue;

            var techs = project.TechStack.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t));

            foreach (var tech in techs)
            {
                if (!techDict.ContainsKey(tech))
                {
                    techDict[tech] = new TechStackDistributionItemDto
                    {
                        Tech = tech,
                        Count = 0,
                        Income = 0
                    };
                }
                techDict[tech].Count++;
                techDict[tech].Income += project.PriceFinal ?? 0;
            }
        }

        var distribution = techDict.Values
            .OrderByDescending(d => d.Income)
            .ToList();

        return Ok(ApiResponse<List<TechStackDistributionItemDto>>.Success(distribution));
    }

    /// <summary>
    /// 获取客户来源分布
    /// </summary>
    [HttpGet("dashboard/client-source")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ClientSourceItemDto>>>> GetClientSource(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? incomeType = null)
    {
        var query = _context.SideProjects
            .Where(p => p.Status == 1 && p.PriceFinal.HasValue && !string.IsNullOrEmpty(p.Source));

        // 收入类型筛选
        if (!string.IsNullOrEmpty(incomeType))
        {
            query = query.Where(p => p.IncomeType == incomeType);
        }

        if (from.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= to.Value);
        }

        var distribution = await query
            .GroupBy(p => p.Source!)
            .Select(g => new ClientSourceItemDto
            {
                Source = g.Key,
                Count = g.Count(),
                Income = g.Sum(p => p.PriceFinal ?? 0)
            })
            .OrderByDescending(d => d.Income)
            .ToListAsync();

        return Ok(ApiResponse<List<ClientSourceItemDto>>.Success(distribution));
    }

    /// <summary>
    /// 获取项目周期分布
    /// </summary>
    [HttpGet("dashboard/duration-buckets")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<DurationBucketItemDto>>>> GetDurationBuckets(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var query = _context.SideProjects
            .Where(p => p.Status == 1 && p.StartTime.HasValue && p.EndTime.HasValue);

        if (from.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= to.Value);
        }

        var projects = await query
            .Select(p => new { p.StartTime, p.EndTime })
            .ToListAsync();

        var buckets = new Dictionary<string, int>
        {
            { "0-7天", 0 },
            { "7-15天", 0 },
            { "15-30天", 0 },
            { "30-60天", 0 },
            { "60-90天", 0 },
            { "90天以上", 0 }
        };

        foreach (var project in projects)
        {
            var days = (project.EndTime!.Value - project.StartTime!.Value).TotalDays;
            if (days <= 7)
                buckets["0-7天"]++;
            else if (days <= 15)
                buckets["7-15天"]++;
            else if (days <= 30)
                buckets["15-30天"]++;
            else if (days <= 60)
                buckets["30-60天"]++;
            else if (days <= 90)
                buckets["60-90天"]++;
            else
                buckets["90天以上"]++;
        }

        var result = buckets
            .Select(kvp => new DurationBucketItemDto
            {
                BucketName = kvp.Key,
                Count = kvp.Value
            })
            .ToList();

        return Ok(ApiResponse<List<DurationBucketItemDto>>.Success(result));
    }

    #endregion

    #region 前台公开展示接口

    /// <summary>
    /// 获取公开展示的项目列表（匿名访问）
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetPublicList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.SideProjects
            .Where(p => p.IsPublic);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Description,
                p.ClientName, // 客户名称可以显示
                // ClientContact 不返回（敏感信息）
                p.Source,
                p.Category,
                p.TechStack,
                // PriceFinal 不返回（敏感信息）
                p.Status,
                p.StartTime,
                p.EndTime,
                p.CreatedAt,
                p.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(new { Total = total, List = items }));
    }

    /// <summary>
    /// 获取公开展示的项目详情（匿名访问）
    /// </summary>
    [HttpGet("public/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetPublicDetail(int id)
    {
        var project = await _context.SideProjects
            .Where(p => p.Id == id && p.IsPublic)
            .FirstOrDefaultAsync();

        if (project == null)
        {
            return Ok(ApiResponse.Error("项目不存在或未公开", 404));
        }

        var result = new
        {
            project.Id,
            project.Title,
            project.Description,
            project.ClientName,
            // ClientContact 不返回
            project.Source,
            project.Category,
            project.TechStack,
            // PriceFinal 不返回
            project.Status,
            project.StartTime,
            project.EndTime,
            project.CreatedAt,
            project.UpdatedAt
        };

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 获取公开看板汇总数据（匿名访问，只统计公开项目）
    /// </summary>
    [HttpGet("public/dashboard/summary")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetPublicDashboardSummary()
    {
        // 只统计公开且已完成的项目
        var query = _context.SideProjects
            .Where(p => p.IsPublic && p.Status == 1);

        var totalProjects = await query.CountAsync();
        
        // 不返回收入等敏感信息，只返回项目数量统计
        var categoryStats = await query
            .Where(p => !string.IsNullOrEmpty(p.Category))
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        var techStats = await query
            .Where(p => !string.IsNullOrEmpty(p.TechStack))
            .SelectMany(p => p.TechStack!.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()))
            .GroupBy(t => t)
            .Select(g => new
            {
                Tech = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var result = new
        {
            TotalProjects = totalProjects,
            CategoryStats = categoryStats,
            TechStats = techStats
        };

        return Ok(ApiResponse.Success(result));
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 将实体映射为DTO
    /// </summary>
    private SideProjectDto MapToDto(SideProject project)
    {
        return new SideProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ClientName = project.ClientName,
            ClientContact = project.ClientContact,
            Source = project.Source,
            Category = project.Category,
            IncomeType = project.IncomeType,
            TechStack = project.TechStack,
            BudgetMin = project.BudgetMin,
            BudgetMax = project.BudgetMax,
            PriceFinal = project.PriceFinal,
            Status = project.Status,
            StartTime = project.StartTime,
            EndTime = project.EndTime,
            IsPublic = project.IsPublic,
            Stage = project.Stage,
            Progress = project.Progress,
            IsProgressManual = project.IsProgressManual,
            Priority = project.Priority,
            DeadlineAt = project.DeadlineAt,
            NextAction = project.NextAction,
            Blocked = project.Blocked,
            BlockReason = project.BlockReason,
            TotalAmount = project.TotalAmount,
            ReceivedAmount = project.ReceivedAmount,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }

    #endregion

    #region 新的Dashboard接口

    /// <summary>
    /// 获取副业管理首页数据
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectDashboardDto>>> GetDashboard()
    {
        var today = DateTime.Today;
        var endOfWeek = today.AddDays(7 - (int)today.DayOfWeek);

        // 今日待办（跨项目任务，截止日期为今天）
        var todayTasks = await _context.SideProjectTasks
            .Include(t => t.Project)
            .Where(t => t.DueAt.HasValue && t.DueAt.Value.Date == today && t.Status != 2)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueAt)
            .Select(t => new SideProjectTaskDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueAt = t.DueAt,
                EstHours = t.EstHours,
                ActHours = t.ActHours,
                SortOrder = t.SortOrder,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        // 进行中项目
        var inProgressProjectEntities = await _context.SideProjects
            .Where(p => p.Status == 0 && !p.Blocked)
            .OrderByDescending(p => p.Priority)
            .ThenByDescending(p => p.UpdatedAt)
            .ToListAsync();
        var inProgressProjects = inProgressProjectEntities.Select(p => MapToDto(p)).ToList();

        // 卡住项目
        var blockedProjectEntities = await _context.SideProjects
            .Where(p => p.Blocked || (p.DeadlineAt.HasValue && p.DeadlineAt.Value < DateTime.Now && p.Status != 1))
            .OrderBy(p => p.DeadlineAt)
            .ToListAsync();
        var blockedProjects = blockedProjectEntities.Select(p => MapToDto(p)).ToList();

        // 本周里程碑
        var thisWeekMilestones = await _context.SideProjectMilestones
            .Include(m => m.Project)
            .Where(m => m.DueAt.HasValue && m.DueAt.Value >= today && m.DueAt.Value <= endOfWeek)
            .OrderBy(m => m.DueAt)
            .Select(m => new SideProjectMilestoneDto
            {
                Id = m.Id,
                ProjectId = m.ProjectId,
                Title = m.Title,
                DueAt = m.DueAt,
                Status = m.Status,
                Notes = m.Notes,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();

        // 收入汇总（本月）
        var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var totalIncome = await _context.SideProjects
            .Where(p => p.Status == 1 && p.PriceFinal.HasValue && p.EndTime >= startOfMonth)
            .SumAsync(p => p.PriceFinal ?? 0);

        var thisMonthIncome = await _context.SideProjects
            .Where(p => p.Status == 1 && p.PriceFinal.HasValue && p.EndTime >= startOfMonth && p.EndTime <= DateTime.Now)
            .SumAsync(p => p.PriceFinal ?? 0);

        var dashboard = new SideProjectDashboardDto
        {
            TodayTasks = todayTasks,
            InProgressProjects = inProgressProjects,
            BlockedProjects = blockedProjects,
            ThisWeekMilestones = thisWeekMilestones,
            TotalIncome = totalIncome,
            ThisMonthIncome = thisMonthIncome
        };

        return Ok(ApiResponse<SideProjectDashboardDto>.Success(dashboard));
    }

    #endregion

    #region 项目详情接口

    /// <summary>
    /// 获取项目详情（包含所有子实体）
    /// </summary>
    [HttpGet("{id}/detail")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectDetailDto>>> GetProjectDetail(int id)
    {
        var project = await _context.SideProjects
            .Include(p => p.Requirements)
            .Include(p => p.Tasks)
            .Include(p => p.Milestones)
            .Include(p => p.Logs.OrderByDescending(l => l.CreatedAt))
            .Include(p => p.Attachments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return Ok(ApiResponse<SideProjectDetailDto>.Error("项目不存在", 404));
        }

        var detail = new SideProjectDetailDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ClientName = project.ClientName,
            ClientContact = project.ClientContact,
            Source = project.Source,
            Category = project.Category,
            IncomeType = project.IncomeType,
            TechStack = project.TechStack,
            BudgetMin = project.BudgetMin,
            BudgetMax = project.BudgetMax,
            PriceFinal = project.PriceFinal,
            Status = project.Status,
            StartTime = project.StartTime,
            EndTime = project.EndTime,
            IsPublic = project.IsPublic,
            Stage = project.Stage,
            Progress = project.Progress,
            IsProgressManual = project.IsProgressManual,
            Priority = project.Priority,
            DeadlineAt = project.DeadlineAt,
            NextAction = project.NextAction,
            Blocked = project.Blocked,
            BlockReason = project.BlockReason,
            TotalAmount = project.TotalAmount,
            ReceivedAmount = project.ReceivedAmount,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Requirements = project.Requirements.Select(r => new SideProjectRequirementDto
            {
                Id = r.Id,
                ProjectId = r.ProjectId,
                ScopeIn = r.ScopeIn,
                ScopeOut = r.ScopeOut,
                AcceptanceCriteria = r.AcceptanceCriteria,
                Deliverables = r.Deliverables,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList(),
            Tasks = project.Tasks.OrderBy(t => t.SortOrder).Select(t => new SideProjectTaskDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueAt = t.DueAt,
                EstHours = t.EstHours,
                ActHours = t.ActHours,
                SortOrder = t.SortOrder,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList(),
            Milestones = project.Milestones.Select(m => new SideProjectMilestoneDto
            {
                Id = m.Id,
                ProjectId = m.ProjectId,
                Title = m.Title,
                DueAt = m.DueAt,
                Status = m.Status,
                Notes = m.Notes,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList(),
            Logs = project.Logs.Select(l => new SideProjectLogDto
            {
                Id = l.Id,
                ProjectId = l.ProjectId,
                Channel = l.Channel,
                Content = l.Content,
                NextTodo = l.NextTodo,
                CreatedAt = l.CreatedAt
            }).ToList(),
            Attachments = project.Attachments.Select(a => new SideProjectAttachmentDto
            {
                Id = a.Id,
                ProjectId = a.ProjectId,
                Type = a.Type,
                Name = a.Name,
                Url = a.Url,
                CreatedAt = a.CreatedAt
            }).ToList()
        };

        return Ok(ApiResponse<SideProjectDetailDto>.Success(detail));
    }

    /// <summary>
    /// 更新项目状态（状态流转）
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectDto>>> UpdateStatus(int id, [FromBody] int status)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<SideProjectDto>.Error("项目不存在", 404));
        }

        project.Status = status;
        project.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        // 同步状态和阶段
        await _projectService.SyncStatusAndStageAsync(project.Id);

        return Ok(ApiResponse<SideProjectDto>.Success(MapToDto(project)));
    }

    /// <summary>
    /// 更新项目进度（进度覆盖/取消覆盖）
    /// </summary>
    [HttpPatch("{id}/progress")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectDto>>> UpdateProgress(int id, [FromBody] UpdateProgressDto dto)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<SideProjectDto>.Error("项目不存在", 404));
        }

        project.IsProgressManual = dto.IsManual;
        if (dto.IsManual && dto.Progress.HasValue)
        {
            project.Progress = Math.Max(0, Math.Min(100, dto.Progress.Value));
        }
        else
        {
            // 自动计算进度
            project.Progress = await _projectService.CalculateProgressAsync(id);
        }

        project.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<SideProjectDto>.Success(MapToDto(project)));
    }

    /// <summary>
    /// 更新进度DTO
    /// </summary>
    public class UpdateProgressDto
    {
        public bool IsManual { get; set; }
        public int? Progress { get; set; }
    }

    #endregion

    #region 看板接口

    /// <summary>
    /// 获取看板数据（按状态分组）
    /// </summary>
    [HttpGet("kanban")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetKanban()
    {
        // 查询所有项目（包括已完成的），用于看板展示
        var projectEntities = await _context.SideProjects
            .OrderByDescending(p => p.Status == 1) // 已完成的排在后面
            .ThenBy(p => p.Priority ?? 0) // 优先级排序（null值排在前面）
            .ThenByDescending(p => p.UpdatedAt)
            .ToListAsync();
        
        var projects = projectEntities.Select(p => MapToDto(p)).ToList();

        // 按阶段分组
        var kanban = new
        {
            Pending = projects.Where(p => (p.Stage == "待开始" || string.IsNullOrEmpty(p.Stage)) && p.Status != 1).ToList(),
            InProgress = projects.Where(p => p.Stage == "进行中" && p.Status != 1).ToList(),
            Blocked = projects.Where(p => (p.Stage == "卡住" || p.Blocked) && p.Status != 1).ToList(),
            PendingReview = projects.Where(p => p.Stage == "待验收" && p.Status != 1).ToList(),
            Completed = projects.Where(p => p.Stage == "已完成" || p.Status == 1).ToList() // 已完成的项目
        };

        return Ok(ApiResponse.Success(kanban));
    }

    #endregion

    #region 需求管理接口

    /// <summary>
    /// 获取项目需求
    /// </summary>
    [HttpGet("{id}/requirement")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectRequirementDto>>> GetRequirement(int id)
    {
        var requirement = await _context.SideProjectRequirements
            .FirstOrDefaultAsync(r => r.ProjectId == id);

        if (requirement == null)
        {
            return Ok(ApiResponse<SideProjectRequirementDto>.Error("需求不存在", 404));
        }

        var dto = new SideProjectRequirementDto
        {
            Id = requirement.Id,
            ProjectId = requirement.ProjectId,
            ScopeIn = requirement.ScopeIn,
            ScopeOut = requirement.ScopeOut,
            AcceptanceCriteria = requirement.AcceptanceCriteria,
            Deliverables = requirement.Deliverables,
            CreatedAt = requirement.CreatedAt,
            UpdatedAt = requirement.UpdatedAt
        };

        return Ok(ApiResponse<SideProjectRequirementDto>.Success(dto));
    }

    /// <summary>
    /// 创建或更新项目需求
    /// </summary>
    [HttpPut("{id}/requirement")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectRequirementDto>>> UpsertRequirement(int id, [FromBody] UpsertRequirementDto dto)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<SideProjectRequirementDto>.Error("项目不存在", 404));
        }

        var requirement = await _context.SideProjectRequirements
            .FirstOrDefaultAsync(r => r.ProjectId == id);

        if (requirement == null)
        {
            requirement = new SideProjectRequirement
            {
                ProjectId = id,
                ScopeIn = dto.ScopeIn,
                ScopeOut = dto.ScopeOut,
                AcceptanceCriteria = dto.AcceptanceCriteria,
                Deliverables = dto.Deliverables,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.SideProjectRequirements.Add(requirement);
        }
        else
        {
            requirement.ScopeIn = dto.ScopeIn;
            requirement.ScopeOut = dto.ScopeOut;
            requirement.AcceptanceCriteria = dto.AcceptanceCriteria;
            requirement.Deliverables = dto.Deliverables;
            requirement.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        var result = new SideProjectRequirementDto
        {
            Id = requirement.Id,
            ProjectId = requirement.ProjectId,
            ScopeIn = requirement.ScopeIn,
            ScopeOut = requirement.ScopeOut,
            AcceptanceCriteria = requirement.AcceptanceCriteria,
            Deliverables = requirement.Deliverables,
            CreatedAt = requirement.CreatedAt,
            UpdatedAt = requirement.UpdatedAt
        };

        return Ok(ApiResponse<SideProjectRequirementDto>.Success(result));
    }

    public class UpsertRequirementDto
    {
        public string? ScopeIn { get; set; }
        public string? ScopeOut { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string? Deliverables { get; set; }
    }

    #endregion

    #region 任务管理接口

    /// <summary>
    /// 获取项目任务列表
    /// </summary>
    [HttpGet("{id}/tasks")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<SideProjectTaskDto>>>> GetTasks(int id, [FromQuery] int? status = null)
    {
        var query = _context.SideProjectTasks.Where(t => t.ProjectId == id);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        var tasks = await query
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.DueAt)
            .Select(t => new SideProjectTaskDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueAt = t.DueAt,
                EstHours = t.EstHours,
                ActHours = t.ActHours,
                SortOrder = t.SortOrder,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<SideProjectTaskDto>>.Success(tasks));
    }

    /// <summary>
    /// 创建任务
    /// </summary>
    [HttpPost("{id}/tasks")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectTaskDto>>> CreateTask(int id, [FromBody] CreateTaskDto dto)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<SideProjectTaskDto>.Error("项目不存在", 404));
        }

        // 获取最大排序值
        var maxSortOrder = await _context.SideProjectTasks
            .Where(t => t.ProjectId == id)
            .Select(t => (int?)t.SortOrder)
            .MaxAsync() ?? 0;

        var task = new SideProjectTask
        {
            ProjectId = id,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status ?? 0,
            Priority = dto.Priority,
            DueAt = dto.DueAt,
            EstHours = dto.EstHours,
            ActHours = dto.ActHours,
            SortOrder = maxSortOrder + 1,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.SideProjectTasks.Add(task);
        await _context.SaveChangesAsync();

        // 如果项目不是手动覆盖进度，自动更新进度
        if (!project.IsProgressManual)
        {
            await _projectService.UpdateProgressIfAutoAsync(id);
        }

        var result = new SideProjectTaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueAt = task.DueAt,
            EstHours = task.EstHours,
            ActHours = task.ActHours,
            SortOrder = task.SortOrder,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return CreatedAtAction(nameof(GetTasks), new { id }, ApiResponse<SideProjectTaskDto>.Success(result));
    }

    /// <summary>
    /// 更新任务
    /// </summary>
    [HttpPut("{id}/tasks/{taskId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectTaskDto>>> UpdateTask(int id, int taskId, [FromBody] UpdateTaskDto dto)
    {
        var task = await _context.SideProjectTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == id);

        if (task == null)
        {
            return Ok(ApiResponse<SideProjectTaskDto>.Error("任务不存在", 404));
        }

        if (dto.Title != null) task.Title = dto.Title;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.Status.HasValue) task.Status = dto.Status.Value;
        if (dto.Priority.HasValue) task.Priority = dto.Priority;
        if (dto.DueAt.HasValue) task.DueAt = dto.DueAt;
        if (dto.EstHours.HasValue) task.EstHours = dto.EstHours;
        if (dto.ActHours.HasValue) task.ActHours = dto.ActHours;
        if (dto.SortOrder.HasValue) task.SortOrder = dto.SortOrder.Value;
        task.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        // 如果项目不是手动覆盖进度，自动更新进度
        var project = await _context.SideProjects.FindAsync(id);
        if (project != null && !project.IsProgressManual)
        {
            await _projectService.UpdateProgressIfAutoAsync(id);
        }

        var result = new SideProjectTaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueAt = task.DueAt,
            EstHours = task.EstHours,
            ActHours = task.ActHours,
            SortOrder = task.SortOrder,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return Ok(ApiResponse<SideProjectTaskDto>.Success(result));
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    [HttpDelete("{id}/tasks/{taskId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTask(int id, int taskId)
    {
        var task = await _context.SideProjectTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == id);

        if (task == null)
        {
            return Ok(ApiResponse<bool>.Error("任务不存在", 404));
        }

        _context.SideProjectTasks.Remove(task);
        await _context.SaveChangesAsync();

        // 如果项目不是手动覆盖进度，自动更新进度
        var project = await _context.SideProjects.FindAsync(id);
        if (project != null && !project.IsProgressManual)
        {
            await _projectService.UpdateProgressIfAutoAsync(id);
        }

        return Ok(ApiResponse<bool>.Success(true));
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public DateTime? DueAt { get; set; }
        public decimal? EstHours { get; set; }
        public decimal? ActHours { get; set; }
    }

    public class UpdateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public DateTime? DueAt { get; set; }
        public decimal? EstHours { get; set; }
        public decimal? ActHours { get; set; }
        public int? SortOrder { get; set; }
    }

    #endregion

    #region 里程碑管理接口

    /// <summary>
    /// 获取项目里程碑列表
    /// </summary>
    [HttpGet("{id}/milestones")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<SideProjectMilestoneDto>>>> GetMilestones(int id)
    {
        var milestones = await _context.SideProjectMilestones
            .Where(m => m.ProjectId == id)
            .OrderBy(m => m.DueAt)
            .Select(m => new SideProjectMilestoneDto
            {
                Id = m.Id,
                ProjectId = m.ProjectId,
                Title = m.Title,
                DueAt = m.DueAt,
                Status = m.Status,
                Notes = m.Notes,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<SideProjectMilestoneDto>>.Success(milestones));
    }

    /// <summary>
    /// 创建里程碑
    /// </summary>
    [HttpPost("{id}/milestones")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectMilestoneDto>>> CreateMilestone(int id, [FromBody] CreateMilestoneDto dto)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<SideProjectMilestoneDto>.Error("项目不存在", 404));
        }

        var milestone = new SideProjectMilestone
        {
            ProjectId = id,
            Title = dto.Title,
            DueAt = dto.DueAt,
            Status = dto.Status ?? 0,
            Notes = dto.Notes,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.SideProjectMilestones.Add(milestone);
        await _context.SaveChangesAsync();

        var result = new SideProjectMilestoneDto
        {
            Id = milestone.Id,
            ProjectId = milestone.ProjectId,
            Title = milestone.Title,
            DueAt = milestone.DueAt,
            Status = milestone.Status,
            Notes = milestone.Notes,
            CreatedAt = milestone.CreatedAt,
            UpdatedAt = milestone.UpdatedAt
        };

        return CreatedAtAction(nameof(GetMilestones), new { id }, ApiResponse<SideProjectMilestoneDto>.Success(result));
    }

    /// <summary>
    /// 更新里程碑
    /// </summary>
    [HttpPut("{id}/milestones/{milestoneId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectMilestoneDto>>> UpdateMilestone(int id, int milestoneId, [FromBody] UpdateMilestoneDto dto)
    {
        var milestone = await _context.SideProjectMilestones
            .FirstOrDefaultAsync(m => m.Id == milestoneId && m.ProjectId == id);

        if (milestone == null)
        {
            return Ok(ApiResponse<SideProjectMilestoneDto>.Error("里程碑不存在", 404));
        }

        if (dto.Title != null) milestone.Title = dto.Title;
        if (dto.DueAt.HasValue) milestone.DueAt = dto.DueAt;
        if (dto.Status.HasValue) milestone.Status = dto.Status.Value;
        if (dto.Notes != null) milestone.Notes = dto.Notes;
        milestone.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        var result = new SideProjectMilestoneDto
        {
            Id = milestone.Id,
            ProjectId = milestone.ProjectId,
            Title = milestone.Title,
            DueAt = milestone.DueAt,
            Status = milestone.Status,
            Notes = milestone.Notes,
            CreatedAt = milestone.CreatedAt,
            UpdatedAt = milestone.UpdatedAt
        };

        return Ok(ApiResponse<SideProjectMilestoneDto>.Success(result));
    }

    /// <summary>
    /// 删除里程碑
    /// </summary>
    [HttpDelete("{id}/milestones/{milestoneId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMilestone(int id, int milestoneId)
    {
        var milestone = await _context.SideProjectMilestones
            .FirstOrDefaultAsync(m => m.Id == milestoneId && m.ProjectId == id);

        if (milestone == null)
        {
            return Ok(ApiResponse<bool>.Error("里程碑不存在", 404));
        }

        _context.SideProjectMilestones.Remove(milestone);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<bool>.Success(true));
    }

    public class CreateMilestoneDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime? DueAt { get; set; }
        public int? Status { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateMilestoneDto
    {
        public string? Title { get; set; }
        public DateTime? DueAt { get; set; }
        public int? Status { get; set; }
        public string? Notes { get; set; }
    }

    #endregion

    #region 沟通记录管理接口

    /// <summary>
    /// 获取项目沟通记录列表
    /// </summary>
    [HttpGet("{id}/logs")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<SideProjectLogDto>>>> GetLogs(int id)
    {
        var logs = await _context.SideProjectLogs
            .Where(l => l.ProjectId == id)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new SideProjectLogDto
            {
                Id = l.Id,
                ProjectId = l.ProjectId,
                Channel = l.Channel,
                Content = l.Content,
                NextTodo = l.NextTodo,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<SideProjectLogDto>>.Success(logs));
    }

    /// <summary>
    /// 创建沟通记录
    /// </summary>
    [HttpPost("{id}/logs")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectLogDto>>> CreateLog(int id, [FromBody] CreateLogDto dto)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<SideProjectLogDto>.Error("项目不存在", 404));
        }

        var log = new SideProjectLog
        {
            ProjectId = id,
            Channel = dto.Channel,
            Content = dto.Content,
            NextTodo = dto.NextTodo,
            CreatedAt = DateTime.Now
        };

        _context.SideProjectLogs.Add(log);
        await _context.SaveChangesAsync();

        // 如果有下一步行动，更新项目
        if (!string.IsNullOrEmpty(dto.NextTodo))
        {
            project.NextAction = dto.NextTodo;
            project.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        var result = new SideProjectLogDto
        {
            Id = log.Id,
            ProjectId = log.ProjectId,
            Channel = log.Channel,
            Content = log.Content,
            NextTodo = log.NextTodo,
            CreatedAt = log.CreatedAt
        };

        return CreatedAtAction(nameof(GetLogs), new { id }, ApiResponse<SideProjectLogDto>.Success(result));
    }

    /// <summary>
    /// 删除沟通记录
    /// </summary>
    [HttpDelete("{id}/logs/{logId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteLog(int id, int logId)
    {
        var log = await _context.SideProjectLogs
            .FirstOrDefaultAsync(l => l.Id == logId && l.ProjectId == id);

        if (log == null)
        {
            return Ok(ApiResponse<bool>.Error("沟通记录不存在", 404));
        }

        _context.SideProjectLogs.Remove(log);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<bool>.Success(true));
    }

    public class CreateLogDto
    {
        public string? Channel { get; set; }
        public string? Content { get; set; }
        public string? NextTodo { get; set; }
    }

    #endregion

    #region 附件管理接口

    /// <summary>
    /// 获取项目附件列表
    /// </summary>
    [HttpGet("{id}/attachments")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<SideProjectAttachmentDto>>>> GetAttachments(int id)
    {
        var attachments = await _context.SideProjectAttachments
            .Where(a => a.ProjectId == id)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new SideProjectAttachmentDto
            {
                Id = a.Id,
                ProjectId = a.ProjectId,
                Type = a.Type,
                Name = a.Name,
                Url = a.Url,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<SideProjectAttachmentDto>>.Success(attachments));
    }

    /// <summary>
    /// 创建附件
    /// </summary>
    [HttpPost("{id}/attachments")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SideProjectAttachmentDto>>> CreateAttachment(int id, [FromBody] CreateAttachmentDto dto)
    {
        var project = await _context.SideProjects.FindAsync(id);
        if (project == null)
        {
            return Ok(ApiResponse<SideProjectAttachmentDto>.Error("项目不存在", 404));
        }

        var attachment = new SideProjectAttachment
        {
            ProjectId = id,
            Type = dto.Type,
            Name = dto.Name,
            Url = dto.Url,
            CreatedAt = DateTime.Now
        };

        _context.SideProjectAttachments.Add(attachment);
        await _context.SaveChangesAsync();

        var result = new SideProjectAttachmentDto
        {
            Id = attachment.Id,
            ProjectId = attachment.ProjectId,
            Type = attachment.Type,
            Name = attachment.Name,
            Url = attachment.Url,
            CreatedAt = attachment.CreatedAt
        };

        return CreatedAtAction(nameof(GetAttachments), new { id }, ApiResponse<SideProjectAttachmentDto>.Success(result));
    }

    /// <summary>
    /// 删除附件
    /// </summary>
    [HttpDelete("{id}/attachments/{attachmentId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAttachment(int id, int attachmentId)
    {
        var attachment = await _context.SideProjectAttachments
            .FirstOrDefaultAsync(a => a.Id == attachmentId && a.ProjectId == id);

        if (attachment == null)
        {
            return Ok(ApiResponse<bool>.Error("附件不存在", 404));
        }

        _context.SideProjectAttachments.Remove(attachment);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<bool>.Success(true));
    }

    public class CreateAttachmentDto
    {
        public string? Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    #endregion
}

