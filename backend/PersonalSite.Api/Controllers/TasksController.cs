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
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetTasks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] int? priority = null,
            [FromQuery] string? category = null,
            [FromQuery] string? keyword = null)
        {
            var query = _context.Tasks.AsQueryable();

            // 筛选
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(t => t.Category == category);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.Title.Contains(keyword) || 
                    (t.Description != null && t.Description.Contains(keyword)));
            }

            // 只获取顶级任务（没有父任务）
            query = query.Where(t => t.ParentId == null);

            var total = await query.CountAsync();
            var tasks = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(t => t.SubTasks)
                .ToListAsync();

            return Ok(ApiResponse<object>.Success(new
            {
                Items = tasks,
                Total = total,
                Page = page,
                PageSize = pageSize
            }));
        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TaskItem>>> GetTask(long id)
        {
            var task = await _context.Tasks
                .Include(t => t.SubTasks)
                .Include(t => t.Parent)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound(ApiResponse<TaskItem>.Error("任务不存在"));
            }

            return Ok(ApiResponse<TaskItem>.Success(task));
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TaskItem>>> CreateTask([FromBody] TaskItem task)
        {
            if (string.IsNullOrEmpty(task.Title))
            {
                return BadRequest(ApiResponse<TaskItem>.Error("任务标题不能为空"));
            }

            task.CreatedAt = DateTime.Now;
            task.UpdatedAt = DateTime.Now;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<TaskItem>.Success(task, "任务创建成功"));
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TaskItem>>> UpdateTask(long id, [FromBody] TaskItem task)
        {
            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound(ApiResponse<TaskItem>.Error("任务不存在"));
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.Priority = task.Priority;
            existingTask.Category = task.Category;
            existingTask.Tags = task.Tags;
            existingTask.DueDate = task.DueDate;
            existingTask.EstimatedHours = task.EstimatedHours;
            existingTask.Progress = task.Progress;
            existingTask.ParentId = task.ParentId;
            existingTask.SortOrder = task.SortOrder;
            existingTask.UpdatedAt = DateTime.Now;

            // 如果状态变为已完成，设置完成时间
            if (task.Status == "completed" && existingTask.CompletedAt == null)
            {
                existingTask.CompletedAt = DateTime.Now;
                existingTask.Progress = 100;
            }
            else if (task.Status != "completed")
            {
                existingTask.CompletedAt = null;
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<TaskItem>.Success(existingTask, "任务更新成功"));
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTask(long id)
        {
            var task = await _context.Tasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound(ApiResponse.Error("任务不存在"));
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "任务删除成功"));
        }

        /// <summary>
        /// 更新任务进度
        /// </summary>
        [HttpPatch("{id}/progress")]
        public async Task<ActionResult<ApiResponse<TaskItem>>> UpdateProgress(long id, [FromBody] int progress)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(ApiResponse<TaskItem>.Error("任务不存在"));
            }

            if (progress < 0 || progress > 100)
            {
                return BadRequest(ApiResponse<TaskItem>.Error("进度值必须在 0-100 之间"));
            }

            task.Progress = progress;
            if (progress == 100 && task.Status != "completed")
            {
                task.Status = "completed";
                task.CompletedAt = DateTime.Now;
            }
            else if (progress < 100 && task.Status == "completed")
            {
                task.Status = "in_progress";
                task.CompletedAt = null;
            }

            task.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<TaskItem>.Success(task, "进度更新成功"));
        }

        /// <summary>
        /// 获取任务统计
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<object>>> GetStats()
        {
            var total = await _context.Tasks.CountAsync();
            var pending = await _context.Tasks.CountAsync(t => t.Status == "pending");
            var inProgress = await _context.Tasks.CountAsync(t => t.Status == "in_progress");
            var completed = await _context.Tasks.CountAsync(t => t.Status == "completed");
            var overdue = await _context.Tasks.CountAsync(t => 
                t.DueDate != null && 
                t.DueDate < DateTime.Now && 
                t.Status != "completed");

            var avgProgress = await _context.Tasks
                .Where(t => t.Status != "completed")
                .AverageAsync(t => (double?)t.Progress) ?? 0;

            return Ok(ApiResponse.Success(new
            {
                Total = total,
                Pending = pending,
                InProgress = inProgress,
                Completed = completed,
                Overdue = overdue,
                AverageProgress = Math.Round(avgProgress, 1)
            }));
        }
    }
}

