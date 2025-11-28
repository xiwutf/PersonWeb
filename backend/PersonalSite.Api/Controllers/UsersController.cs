using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Utils;

namespace PersonalSite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,
                    u.Status,
                    u.LastLoginTime,
                    u.LastLoginIp,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Success(users));
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetUser(long id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,
                    u.Status,
                    u.LastLoginTime,
                    u.LastLoginIp,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(ApiResponse.Error("用户不存在"));
            }

            return Ok(ApiResponse<object>.Success(user));
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrEmpty(request.Username))
            {
                return BadRequest(ApiResponse.Error("用户名不能为空"));
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(ApiResponse.Error("密码不能为空"));
            }

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(ApiResponse.Error("用户名已存在"));
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                Email = request.Email,
                Role = request.Role ?? "admin",
                Status = request.Status,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.Status,
                user.CreatedAt
            }, "用户创建成功"));
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUser(long id, [FromBody] UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse.Error("用户不存在"));
            }

            // 检查用户名是否重复
            if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
            {
                if (await _context.Users.AnyAsync(u => u.Username == request.Username && u.Id != id))
                {
                    return BadRequest(ApiResponse.Error("用户名已存在"));
                }
                user.Username = request.Username;
            }

            // 更新密码（如果提供）
            if (!string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = PasswordHelper.HashPassword(request.Password);
            }

            if (request.Email != null)
            {
                user.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.Role))
            {
                user.Role = request.Role;
            }

            if (request.Status.HasValue)
            {
                user.Status = request.Status.Value;
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.Status
            }, "用户更新成功"));
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse.Error("用户不存在"));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "用户删除成功"));
        }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Role { get; set; }
        public sbyte Status { get; set; } = 1;
    }

    public class UpdateUserRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public sbyte? Status { get; set; }
    }
}

