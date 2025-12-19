using PersonalSite.Api.Services;

namespace PersonalSite.Api.Services;

/// <summary>
/// 通知生成后台服务
/// 定时执行提醒生成任务（每10分钟执行一次）
/// </summary>
public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(10); // 每10分钟执行一次

    public NotificationBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<NotificationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("通知生成后台服务已启动，将每 {Period} 分钟执行一次", _period.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 使用作用域创建服务，确保每次使用新的 DbContext
                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                    await notificationService.GenerateNotificationsAsync();
                }

                // 等待指定时间后再次执行
                await Task.Delay(_period, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "通知生成后台服务执行时发生错误");
                // 发生错误时等待一段时间再重试，避免频繁失败
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        _logger.LogInformation("通知生成后台服务已停止");
    }
}

