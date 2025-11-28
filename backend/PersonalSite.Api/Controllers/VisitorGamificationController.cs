using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 访客互动式玩法控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VisitorGamificationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<VisitorGamificationController> _logger;

    // 等级配置：每个等级需要的经验值
    private static readonly Dictionary<int, (int MinExp, int MaxExp, string Title, string Badge)> LevelConfig = new()
    {
        { 1, (0, 99, "普通访客", "👤") },
        { 2, (100, 249, "探索者", "🔍") },
        { 3, (250, 499, "活跃用户", "⭐") },
        { 4, (500, 999, "老朋友", "🤝") },
        { 5, (1000, 1999, "忠实粉丝", "💎") },
        { 10, (2000, int.MaxValue, "致敬探索者", "👑") }
    };

    // 行为经验值配置
    private static readonly Dictionary<string, int> BehaviorExpConfig = new()
    {
        { "scroll_to_bottom", 5 },
        { "avatar_hover", 2 },
        { "idle_10s", 3 },
        { "use_tool", 10 },
        { "comment", 15 },
        { "share", 20 },
        { "unlock_egg", 50 },
        { "complete_game", 30 }
    };

    public VisitorGamificationController(AppDbContext context, ILogger<VisitorGamificationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取访客等级信息
    /// </summary>
    [HttpPost("level")]
    public async Task<ActionResult<ApiResponse<object>>> GetVisitorLevel([FromBody] VisitorIdRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.VisitorId))
            {
                return BadRequest(ApiResponse.Error("VisitorId is required", 400));
            }

            var level = await _context.VisitorLevels
                .FirstOrDefaultAsync(v => v.VisitorId == request.VisitorId);

            if (level == null)
            {
                // 创建新访客等级记录
                level = new VisitorLevel
                {
                    VisitorId = request.VisitorId,
                    Level = 1,
                    Experience = 0,
                    TotalExperience = 0,
                    Title = "普通访客",
                    Badge = "👤"
                };
                _context.VisitorLevels.Add(level);
                await _context.SaveChangesAsync();
            }

            // 计算下一级所需经验
            var nextLevelExp = GetNextLevelExp(level.Level);
            var progress = nextLevelExp > 0 
                ? (double)level.Experience / nextLevelExp * 100 
                : 100;

            return Ok(ApiResponse.Success(new
            {
                level.Level,
                level.Experience,
                level.TotalExperience,
                level.Title,
                level.Badge,
                NextLevelExp = nextLevelExp,
                Progress = Math.Round(progress, 2)
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取访客等级失败");
            return StatusCode(500, ApiResponse.Error($"获取等级失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 记录访客行为并触发事件
    /// </summary>
    [HttpPost("behavior")]
    public async Task<ActionResult<ApiResponse<object>>> RecordBehavior([FromBody] VisitorBehaviorRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.VisitorId) || string.IsNullOrEmpty(request.BehaviorType))
            {
                return BadRequest(ApiResponse.Error("VisitorId and BehaviorType are required", 400));
            }

            // 获取经验值
            var expGained = BehaviorExpConfig.GetValueOrDefault(request.BehaviorType, 0);

            // 记录行为
            var behavior = new VisitorBehavior
            {
                VisitorId = request.VisitorId,
                BehaviorType = request.BehaviorType,
                BehaviorData = request.BehaviorData != null 
                    ? JsonSerializer.Serialize(request.BehaviorData) 
                    : null,
                ExperienceGained = expGained,
                TriggeredEvent = request.TriggeredEvent
            };
            _context.VisitorBehaviors.Add(behavior);

            // 更新等级
            var level = await _context.VisitorLevels
                .FirstOrDefaultAsync(v => v.VisitorId == request.VisitorId);

            if (level == null)
            {
                level = new VisitorLevel
                {
                    VisitorId = request.VisitorId,
                    Level = 1,
                    Experience = 0,
                    TotalExperience = 0
                };
                _context.VisitorLevels.Add(level);
            }

            // 增加经验值
            level.Experience += expGained;
            level.TotalExperience += expGained;

            // 检查是否升级
            var newLevel = CalculateLevel(level.TotalExperience);
            var leveledUp = newLevel > level.Level;
            
            if (leveledUp)
            {
                level.Level = newLevel;
                var levelInfo = GetLevelInfo(newLevel);
                level.Title = levelInfo.Title;
                level.Badge = levelInfo.Badge;
            }

            level.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            // 记录触发事件（如果有）
            if (!string.IsNullOrEmpty(request.TriggeredEvent))
            {
                var triggerEvent = new VisitorTriggerEvent
                {
                    VisitorId = request.VisitorId,
                    TriggerType = request.TriggeredEvent,
                    TriggerContext = request.BehaviorData != null
                        ? JsonSerializer.Serialize(new { request.Path, request.BehaviorData })
                        : JsonSerializer.Serialize(new { request.Path })
                };
                _context.VisitorTriggerEvents.Add(triggerEvent);
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse.Success(new
            {
                ExperienceGained = expGained,
                LeveledUp = leveledUp,
                NewLevel = leveledUp ? newLevel : (int?)null,
                CurrentLevel = level.Level,
                CurrentExp = level.Experience,
                TriggeredEvent = request.TriggeredEvent
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录行为失败");
            return StatusCode(500, ApiResponse.Error($"记录行为失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 参与挑战
    /// </summary>
    [HttpPost("challenge/participate")]
    public async Task<ActionResult<ApiResponse<object>>> ParticipateChallenge([FromBody] ChallengeParticipateRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.VisitorId) || string.IsNullOrEmpty(request.ChallengeCode))
            {
                return BadRequest(ApiResponse.Error("VisitorId and ChallengeCode are required", 400));
            }

            var challenge = await _context.VisitorChallenges
                .FirstOrDefaultAsync(c => c.ChallengeCode == request.ChallengeCode && c.Status == "active");

            if (challenge == null)
            {
                return NotFound(ApiResponse.Error("挑战不存在或已结束", 404));
            }

            // 检查是否已参与（防止重复计数）
            var existingParticipant = await _context.VisitorChallengeParticipants
                .FirstOrDefaultAsync(p => 
                    p.ChallengeId == challenge.Id && 
                    p.VisitorId == request.VisitorId &&
                    p.CreatedAt.Date == DateTime.Today);

            if (existingParticipant != null)
            {
                return Ok(ApiResponse.Success(new
                {
                    Message = "今日已参与",
                    Challenge = new
                    {
                        challenge.ChallengeCode,
                        challenge.ChallengeName,
                        challenge.CurrentCount,
                        challenge.TargetCount,
                        Progress = (double)challenge.CurrentCount / challenge.TargetCount * 100
                    }
                }));
            }

            // 记录参与
            var participant = new VisitorChallengeParticipant
            {
                ChallengeId = challenge.Id,
                VisitorId = request.VisitorId,
                ActionType = request.ActionType ?? "button_press",
                ActionData = request.ActionData != null
                    ? JsonSerializer.Serialize(request.ActionData)
                    : null,
                ContributedCount = request.ContributedCount ?? 1
            };
            _context.VisitorChallengeParticipants.Add(participant);

            // 更新挑战计数
            challenge.CurrentCount += participant.ContributedCount;

            // 检查是否完成
            var completed = challenge.CurrentCount >= challenge.TargetCount;
            if (completed)
            {
                challenge.Status = "completed";
                challenge.CompletedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new
            {
                Message = completed ? "挑战完成！" : "参与成功",
                Challenge = new
                {
                    challenge.ChallengeCode,
                    challenge.ChallengeName,
                    challenge.CurrentCount,
                    challenge.TargetCount,
                    Progress = Math.Round((double)challenge.CurrentCount / challenge.TargetCount * 100, 2),
                    Completed = completed,
                    Reward = completed ? challenge.RewardDescription : null
                }
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "参与挑战失败");
            return StatusCode(500, ApiResponse.Error($"参与挑战失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取挑战列表
    /// </summary>
    [HttpGet("challenges")]
    public async Task<ActionResult<ApiResponse<object>>> GetChallenges()
    {
        try
        {
            var challenges = await _context.VisitorChallenges
                .Where(c => c.Status == "active")
                .OrderBy(c => c.StartedAt)
                .ToListAsync();

            var result = challenges.Select(c => new
            {
                c.ChallengeCode,
                c.ChallengeName,
                c.ChallengeType,
                c.TargetCount,
                c.CurrentCount,
                Progress = Math.Round((double)c.CurrentCount / c.TargetCount * 100, 2),
                c.RewardDescription,
                c.StartedAt,
                c.ExpiresAt
            });

            return Ok(ApiResponse.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取挑战列表失败");
            return StatusCode(500, ApiResponse.Error($"获取挑战列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取访客成就列表
    /// </summary>
    [HttpPost("achievements")]
    public async Task<ActionResult<ApiResponse<object>>> GetAchievements([FromBody] VisitorIdRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.VisitorId))
            {
                return BadRequest(ApiResponse.Error("VisitorId is required", 400));
            }

            var achievements = await _context.VisitorAchievements
                .Where(a => a.VisitorId == request.VisitorId)
                .OrderByDescending(a => a.UnlockedAt)
                .ToListAsync();

            var result = achievements.Select(a => new
            {
                a.AchievementCode,
                a.AchievementName,
                a.AchievementDescription,
                a.Icon,
                a.UnlockedAt
            });

            return Ok(ApiResponse.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取成就列表失败");
            return StatusCode(500, ApiResponse.Error($"获取成就列表失败: {ex.Message}", 500));
        }
    }

    // 辅助方法：计算等级
    private int CalculateLevel(int totalExp)
    {
        foreach (var (level, (minExp, maxExp, _, _)) in LevelConfig.OrderByDescending(x => x.Key))
        {
            if (totalExp >= minExp)
            {
                return level;
            }
        }
        return 1;
    }

    // 辅助方法：获取等级信息
    private (string Title, string Badge) GetLevelInfo(int level)
    {
        if (LevelConfig.TryGetValue(level, out var info))
        {
            return (info.Title, info.Badge);
        }
        return ("普通访客", "👤");
    }

    // 辅助方法：获取下一级所需经验
    private int GetNextLevelExp(int currentLevel)
    {
        var nextLevel = currentLevel + 1;
        if (LevelConfig.TryGetValue(nextLevel, out var info))
        {
            return info.MinExp;
        }
        return 0; // 已满级
    }
}

// 请求模型
public class VisitorIdRequest
{
    public string VisitorId { get; set; } = string.Empty;
}

public class VisitorBehaviorRequest
{
    public string VisitorId { get; set; } = string.Empty;
    public string BehaviorType { get; set; } = string.Empty;
    public Dictionary<string, object>? BehaviorData { get; set; }
    public string? TriggeredEvent { get; set; }
    public string? Path { get; set; }
}

public class ChallengeParticipateRequest
{
    public string VisitorId { get; set; } = string.Empty;
    public string ChallengeCode { get; set; } = string.Empty;
    public string? ActionType { get; set; }
    public Dictionary<string, object>? ActionData { get; set; }
    public int? ContributedCount { get; set; }
}

