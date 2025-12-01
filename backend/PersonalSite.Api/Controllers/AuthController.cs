using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Utils;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Ok(ApiResponse<LoginResponse>.Error("用户名或密码错误", 401));
        }

        if (user.Status == 0)
        {
            return Ok(ApiResponse<LoginResponse>.Error("账号已被禁用", 403));
        }

        // 更新登录时间
        user.LastLoginTime = DateTime.Now;
        user.LastLoginIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        await _context.SaveChangesAsync();

        // 生成 Token
        var token = GenerateJwtToken(user);

        return Ok(ApiResponse<LoginResponse>.Success(new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role
        }));
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<User>>> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        // 不返回密码哈希
        user.PasswordHash = "";

        return Ok(ApiResponse<User>.Success(user));
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        // 获取当前用户ID
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        // 验证请求参数
        if (string.IsNullOrEmpty(request.OldPassword))
        {
            return BadRequest(ApiResponse.Error("当前密码不能为空"));
        }

        if (string.IsNullOrEmpty(request.NewPassword))
        {
            return BadRequest(ApiResponse.Error("新密码不能为空"));
        }

        if (request.NewPassword.Length < 6)
        {
            return BadRequest(ApiResponse.Error("新密码长度至少6位字符"));
        }

        // 查找用户
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(ApiResponse.Error("用户不存在"));
        }

        // 验证当前密码
        if (!PasswordHelper.VerifyPassword(request.OldPassword, user.PasswordHash))
        {
            return BadRequest(ApiResponse.Error("当前密码错误"));
        }

        // 检查新密码是否与旧密码相同
        if (PasswordHelper.VerifyPassword(request.NewPassword, user.PasswordHash))
        {
            return BadRequest(ApiResponse.Error("新密码不能与当前密码相同"));
        }

        // 更新密码
        user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "密码修改成功"));
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
