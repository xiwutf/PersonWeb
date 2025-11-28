using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MonthlyKpisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MonthlyKpisController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取月度 KPI 列表
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetMonthlyKpis(
            [FromQuery] long? goalId = null,
            [FromQuery] int? year = null,
            [FromQuery] int? month = null,
            [FromQuery] string? status = null)
        {
            var query = _context.MonthlyKpis.AsQueryable();

            if (goalId.HasValue)
            {
                query = query.Where(k => k.GoalId == goalId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(k => k.Year == year.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(k => k.Month == month.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(k => k.Status == status);
            }

            var kpis = await query
                .Include(k => k.Goal)
                .OrderByDescending(k => k.Year)
                .ThenByDescending(k => k.Month)
                .ToListAsync();

            return Ok(ApiResponse<object>.Success(new
            {
                Items = kpis,
                Total = kpis.Count
            }));
        }

        /// <summary>
        /// 获取 KPI 详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MonthlyKpi>>> GetMonthlyKpi(long id)
        {
            var kpi = await _context.MonthlyKpis
                .Include(k => k.Goal)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kpi == null)
            {
                return NotFound(ApiResponse<MonthlyKpi>.Error("KPI 不存在"));
            }

            return Ok(ApiResponse<MonthlyKpi>.Success(kpi));
        }

        /// <summary>
        /// 创建月度 KPI
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<MonthlyKpi>>> CreateMonthlyKpi([FromBody] MonthlyKpi kpi)
        {
            if (string.IsNullOrEmpty(kpi.Title))
            {
                return BadRequest(ApiResponse<MonthlyKpi>.Error("KPI 标题不能为空"));
            }

            if (kpi.GoalId <= 0)
            {
                return BadRequest(ApiResponse<MonthlyKpi>.Error("目标ID无效"));
            }

            // 验证目标是否存在
            var goal = await _context.Goals.FindAsync(kpi.GoalId);
            if (goal == null)
            {
                return NotFound(ApiResponse<MonthlyKpi>.Error("关联的目标不存在"));
            }

            // 设置年份（从目标获取）
            if (kpi.Year <= 0)
            {
                kpi.Year = goal.Year;
            }

            // 验证月份
            if (kpi.Month < 1 || kpi.Month > 12)
            {
                return BadRequest(ApiResponse<MonthlyKpi>.Error("月份必须在 1-12 之间"));
            }

            // 计算初始进度
            if (kpi.TargetValue.HasValue && kpi.TargetValue.Value > 0)
            {
                kpi.Progress = (int)((kpi.CurrentValue / kpi.TargetValue.Value) * 100);
            }

            kpi.CreatedAt = DateTime.Now;
            kpi.UpdatedAt = DateTime.Now;

            _context.MonthlyKpis.Add(kpi);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<MonthlyKpi>.Success(kpi, "KPI 创建成功"));
        }

        /// <summary>
        /// 批量创建月度 KPI（从年度目标自动分解）
        /// </summary>
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse<object>>> CreateMonthlyKpisBatch([FromBody] BatchCreateKpiRequest request)
        {
            var goal = await _context.Goals.FindAsync(request.GoalId);
            if (goal == null)
            {
                return NotFound(ApiResponse.Error("目标不存在"));
            }

            // 检查是否已存在该目标的月度 KPI
            var existingKpis = await _context.MonthlyKpis
                .Where(k => k.GoalId == request.GoalId && k.Year == request.Year)
                .ToListAsync();

            if (existingKpis.Any())
            {
                return BadRequest(ApiResponse.Error("该年度目标已存在月度 KPI，请先删除后再创建"));
            }

            var kpis = new List<MonthlyKpi>();
            var monthlyTarget = goal.TargetValue.HasValue ? goal.TargetValue.Value / 12 : 0;

            for (int month = 1; month <= 12; month++)
            {
                var kpi = new MonthlyKpi
                {
                    GoalId = request.GoalId,
                    Year = request.Year,
                    Month = month,
                    Title = $"{goal.Title} - {month}月",
                    TargetValue = monthlyTarget > 0 ? monthlyTarget : null,
                    CurrentValue = 0,
                    Unit = goal.Unit,
                    Status = "pending",
                    Progress = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                kpis.Add(kpi);
            }

            _context.MonthlyKpis.AddRange(kpis);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new
            {
                Created = kpis.Count,
                Items = kpis
            }, "批量创建 KPI 成功"));
        }

        /// <summary>
        /// 更新 KPI
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<MonthlyKpi>>> UpdateMonthlyKpi(long id, [FromBody] MonthlyKpi kpi)
        {
            var existingKpi = await _context.MonthlyKpis.FindAsync(id);
            if (existingKpi == null)
            {
                return NotFound(ApiResponse<MonthlyKpi>.Error("KPI 不存在"));
            }

            existingKpi.Title = kpi.Title;
            existingKpi.TargetValue = kpi.TargetValue;
            existingKpi.CurrentValue = kpi.CurrentValue;
            existingKpi.Unit = kpi.Unit;
            existingKpi.Status = kpi.Status;
            existingKpi.Notes = kpi.Notes;
            existingKpi.UpdatedAt = DateTime.Now;

            // 重新计算进度
            if (existingKpi.TargetValue.HasValue && existingKpi.TargetValue.Value > 0)
            {
                existingKpi.Progress = (int)Math.Min(100, Math.Max(0,
                    (existingKpi.CurrentValue / existingKpi.TargetValue.Value) * 100));
            }

            // 如果进度达到 100%，自动标记为已完成
            if (existingKpi.Progress >= 100 && existingKpi.Status != "completed")
            {
                existingKpi.Status = "completed";
            }

            await _context.SaveChangesAsync();

            // 更新关联目标的当前值（可选：自动汇总所有 KPI）
            await UpdateGoalFromKpis(existingKpi.GoalId);

            return Ok(ApiResponse<MonthlyKpi>.Success(existingKpi, "KPI 更新成功"));
        }

        /// <summary>
        /// 更新 KPI 当前值
        /// </summary>
        [HttpPatch("{id}/value")]
        public async Task<ActionResult<ApiResponse<MonthlyKpi>>> UpdateCurrentValue(long id, [FromBody] decimal value)
        {
            var kpi = await _context.MonthlyKpis.FindAsync(id);
            if (kpi == null)
            {
                return NotFound(ApiResponse<MonthlyKpi>.Error("KPI 不存在"));
            }

            kpi.CurrentValue = value;

            // 重新计算进度
            if (kpi.TargetValue.HasValue && kpi.TargetValue.Value > 0)
            {
                kpi.Progress = (int)Math.Min(100, Math.Max(0,
                    (kpi.CurrentValue / kpi.TargetValue.Value) * 100));
            }

            // 如果进度达到 100%，自动标记为已完成
            if (kpi.Progress >= 100 && kpi.Status != "completed")
            {
                kpi.Status = "completed";
            }

            kpi.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            // 更新关联目标的当前值
            await UpdateGoalFromKpis(kpi.GoalId);

            return Ok(ApiResponse<MonthlyKpi>.Success(kpi, "KPI 值更新成功"));
        }

        /// <summary>
        /// 删除 KPI
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMonthlyKpi(long id)
        {
            var kpi = await _context.MonthlyKpis.FindAsync(id);
            if (kpi == null)
            {
                return NotFound(ApiResponse.Error("KPI 不存在"));
            }

            var goalId = kpi.GoalId;
            _context.MonthlyKpis.Remove(kpi);
            await _context.SaveChangesAsync();

            // 更新关联目标的当前值
            await UpdateGoalFromKpis(goalId);

            return Ok(ApiResponse.Success(null, "KPI 删除成功"));
        }

        /// <summary>
        /// 根据 KPI 更新目标的当前值（自动汇总）
        /// </summary>
        private async Task UpdateGoalFromKpis(long goalId)
        {
            var goal = await _context.Goals.FindAsync(goalId);
            if (goal == null) return;

            // 汇总所有 KPI 的当前值
            var totalValue = await _context.MonthlyKpis
                .Where(k => k.GoalId == goalId)
                .SumAsync(k => k.CurrentValue);

            goal.CurrentValue = totalValue;

            // 重新计算目标进度
            if (goal.TargetValue.HasValue && goal.TargetValue.Value > 0)
            {
                goal.Progress = (int)Math.Min(100, Math.Max(0,
                    (goal.CurrentValue / goal.TargetValue.Value) * 100));
            }

            // 如果进度达到 100%，自动标记为已完成
            if (goal.Progress >= 100 && goal.Status != "completed")
            {
                goal.Status = "completed";
            }

            goal.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 批量创建 KPI 请求
    /// </summary>
    public class BatchCreateKpiRequest
    {
        public long GoalId { get; set; }
        public int Year { get; set; }
    }
}

