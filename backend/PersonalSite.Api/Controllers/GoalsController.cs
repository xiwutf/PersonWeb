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
    public class GoalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GoalsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取年度目标列表
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetGoals(
            [FromQuery] int? year = null,
            [FromQuery] string? status = null,
            [FromQuery] string? category = null)
        {
            var query = _context.Goals.AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(g => g.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(g => g.Status == status);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(g => g.Category == category);
            }

            var goals = await query
                .OrderByDescending(g => g.Year)
                .ThenByDescending(g => g.CreatedAt)
                .Include(g => g.MonthlyKpis)
                .ToListAsync();

            return Ok(ApiResponse<object>.Success(new
            {
                Items = goals,
                Total = goals.Count
            }));
        }

        /// <summary>
        /// 获取目标详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Goal>>> GetGoal(long id)
        {
            var goal = await _context.Goals
                .Include(g => g.MonthlyKpis)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (goal == null)
            {
                return NotFound(ApiResponse<Goal>.Error("目标不存在"));
            }

            return Ok(ApiResponse<Goal>.Success(goal));
        }

        /// <summary>
        /// 创建年度目标
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Goal>>> CreateGoal([FromBody] Goal goal)
        {
            if (string.IsNullOrEmpty(goal.Title))
            {
                return BadRequest(ApiResponse<Goal>.Error("目标标题不能为空"));
            }

            if (goal.Year <= 0)
            {
                return BadRequest(ApiResponse<Goal>.Error("目标年份无效"));
            }

            // 计算初始进度
            if (goal.TargetValue.HasValue && goal.TargetValue.Value > 0)
            {
                goal.Progress = (int)((goal.CurrentValue / goal.TargetValue.Value) * 100);
            }

            goal.CreatedAt = DateTime.Now;
            goal.UpdatedAt = DateTime.Now;

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Goal>.Success(goal, "目标创建成功"));
        }

        /// <summary>
        /// 更新目标
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Goal>>> UpdateGoal(long id, [FromBody] Goal goal)
        {
            var existingGoal = await _context.Goals.FindAsync(id);
            if (existingGoal == null)
            {
                return NotFound(ApiResponse<Goal>.Error("目标不存在"));
            }

            existingGoal.Title = goal.Title;
            existingGoal.Description = goal.Description;
            existingGoal.Category = goal.Category;
            existingGoal.TargetValue = goal.TargetValue;
            existingGoal.CurrentValue = goal.CurrentValue;
            existingGoal.Unit = goal.Unit;
            existingGoal.Status = goal.Status;
            existingGoal.StartDate = goal.StartDate;
            existingGoal.EndDate = goal.EndDate;
            existingGoal.UpdatedAt = DateTime.Now;

            // 重新计算进度
            if (existingGoal.TargetValue.HasValue && existingGoal.TargetValue.Value > 0)
            {
                existingGoal.Progress = (int)Math.Min(100, Math.Max(0, 
                    (existingGoal.CurrentValue / existingGoal.TargetValue.Value) * 100));
            }

            // 如果进度达到 100%，自动标记为已完成
            if (existingGoal.Progress >= 100 && existingGoal.Status != "completed")
            {
                existingGoal.Status = "completed";
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Goal>.Success(existingGoal, "目标更新成功"));
        }

        /// <summary>
        /// 更新目标当前值
        /// </summary>
        [HttpPatch("{id}/value")]
        public async Task<ActionResult<ApiResponse<Goal>>> UpdateCurrentValue(long id, [FromBody] decimal value)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return NotFound(ApiResponse<Goal>.Error("目标不存在"));
            }

            goal.CurrentValue = value;

            // 重新计算进度
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

            return Ok(ApiResponse<Goal>.Success(goal, "目标值更新成功"));
        }

        /// <summary>
        /// 删除目标
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteGoal(long id)
        {
            var goal = await _context.Goals
                .Include(g => g.MonthlyKpis)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (goal == null)
            {
                return NotFound(ApiResponse.Error("目标不存在"));
            }

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "目标删除成功"));
        }

        /// <summary>
        /// 获取目标统计
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<object>>> GetStats([FromQuery] int? year = null)
        {
            var query = _context.Goals.AsQueryable();
            if (year.HasValue)
            {
                query = query.Where(g => g.Year == year.Value);
            }

            var total = await query.CountAsync();
            var active = await query.CountAsync(g => g.Status == "active");
            var completed = await query.CountAsync(g => g.Status == "completed");
            var archived = await query.CountAsync(g => g.Status == "archived");

            var avgProgress = await query
                .Where(g => g.Status == "active")
                .AverageAsync(g => (double?)g.Progress) ?? 0;

            return Ok(ApiResponse.Success(new
            {
                Total = total,
                Active = active,
                Completed = completed,
                Archived = archived,
                AverageProgress = Math.Round(avgProgress, 1)
            }));
        }
    }
}

