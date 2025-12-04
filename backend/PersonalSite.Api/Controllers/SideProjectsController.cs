using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 兼职项目控制器
/// </summary>
[ApiController]
[Route("api/side-projects")]
public class SideProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SideProjectsController(AppDbContext context)
    {
        _context = context;
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
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new SideProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ClientName = p.ClientName,
                ClientContact = p.ClientContact,
                Source = p.Source,
                Category = p.Category,
                IncomeType = p.IncomeType,
                TechStack = p.TechStack,
                BudgetMin = p.BudgetMin,
                BudgetMax = p.BudgetMax,
                PriceFinal = p.PriceFinal,
                Status = p.Status,
                StartTime = p.StartTime,
                EndTime = p.EndTime,
                IsPublic = p.IsPublic,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

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

        var dto = new SideProjectDto
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
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

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
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.SideProjects.Add(project);
        await _context.SaveChangesAsync();

        var result = new SideProjectDto
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
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

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
        project.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        var result = new SideProjectDto
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
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

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
}

