using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace PersonalSite.Api.Services;

/// <summary>
/// AI 服务客户端配置选项
/// </summary>
public class AiServiceOptions
{
    /// <summary>
    /// AI 服务基础 URL
    /// </summary>
    public string BaseUrl { get; set; } = "http://api.xifg.com.cn/_internal/ai";
    
    /// <summary>
    /// 请求超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;
}

/// <summary>
/// AI 服务统一响应格式
/// </summary>
public class AiServiceResponse<T>
{
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }
    public string Message { get; set; } = "";
    public T? Data { get; set; }
}

/// <summary>
/// 聊天请求模型
/// </summary>
public class ChatRequestDto
{
    public string UserId { get; set; } = "";
    public string? SessionId { get; set; }
    public string Message { get; set; } = "";
    public string? Model { get; set; }
    public Dictionary<string, object>? Meta { get; set; }
}

/// <summary>
/// 聊天响应模型
/// </summary>
public class ChatResponseDto
{
    public string Reply { get; set; } = "";
    public string Model { get; set; } = "";
    public TokenUsage? Usage { get; set; }
}

/// <summary>
/// Token 使用量
/// </summary>
public class TokenUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}

/// <summary>
/// 摘要请求模型
/// </summary>
public class SummarizeRequestDto
{
    public string Text { get; set; } = "";
    public int? MaxLength { get; set; } = 100;
}

/// <summary>
/// 摘要响应模型
/// </summary>
public class SummarizeResponseDto
{
    public string Summary { get; set; } = "";
}

/// <summary>
/// 标题和标签请求模型
/// </summary>
public class TitleAndTagsRequestDto
{
    public string Text { get; set; } = "";
    public int? MaxTags { get; set; } = 5;
}

/// <summary>
/// 标题和标签响应模型
/// </summary>
public class TitleAndTagsResponseDto
{
    public string Title { get; set; } = "";
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// 代码解释请求模型
/// </summary>
public class CodeExplainRequestDto
{
    public string Code { get; set; } = "";
    public string? Language { get; set; }
}

/// <summary>
/// 代码解释响应模型
/// </summary>
public class CodeExplainResponseDto
{
    public string Explanation { get; set; } = "";
}

/// <summary>
/// AI 服务客户端
/// 用于调用 Python AI Service 的 HTTP 客户端
/// </summary>
public class AiServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly AiServiceOptions _options;
    private readonly ILogger<AiServiceClient> _logger;

    public AiServiceClient(
        HttpClient httpClient,
        IOptions<AiServiceOptions> options,
        ILogger<AiServiceClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        // 配置 HttpClient
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    /// <summary>
    /// 调用聊天接口
    /// </summary>
    public async Task<ChatResponseDto> ChatAsync(ChatRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("调用 AI 聊天接口: UserId={UserId}, MessageLength={Length}",
                request.UserId, request.Message.Length);

            var response = await _httpClient.PostAsJsonAsync("/chat", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AiServiceResponse<ChatResponseDto>>(
                cancellationToken: cancellationToken);

            if (result == null || !result.Success)
            {
                throw new Exception($"AI 服务返回错误: {result?.ErrorCode} - {result?.Message}");
            }

            return result.Data ?? throw new Exception("AI 服务返回数据为空");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用 AI 聊天接口失败");
            throw;
        }
    }

    /// <summary>
    /// 生成文章摘要
    /// </summary>
    public async Task<string> SummarizeAsync(string text, int? maxLength = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("调用 AI 摘要接口: TextLength={Length}, MaxLength={MaxLength}",
                text.Length, maxLength);

            var request = new SummarizeRequestDto
            {
                Text = text,
                MaxLength = maxLength
            };

            var response = await _httpClient.PostAsJsonAsync("/tools/summarize", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AiServiceResponse<SummarizeResponseDto>>(
                cancellationToken: cancellationToken);

            if (result == null || !result.Success)
            {
                throw new Exception($"AI 服务返回错误: {result?.ErrorCode} - {result?.Message}");
            }

            return result.Data?.Summary ?? throw new Exception("AI 服务返回摘要为空");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用 AI 摘要接口失败");
            throw;
        }
    }

    /// <summary>
    /// 生成标题和标签
    /// </summary>
    public async Task<TitleAndTagsResponseDto> GenerateTitleAndTagsAsync(
        string text,
        int? maxTags = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("调用 AI 标题标签接口: TextLength={Length}, MaxTags={MaxTags}",
                text.Length, maxTags);

            var request = new TitleAndTagsRequestDto
            {
                Text = text,
                MaxTags = maxTags
            };

            var response = await _httpClient.PostAsJsonAsync("/tools/title-and-tags", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AiServiceResponse<TitleAndTagsResponseDto>>(
                cancellationToken: cancellationToken);

            if (result == null || !result.Success)
            {
                throw new Exception($"AI 服务返回错误: {result?.ErrorCode} - {result?.Message}");
            }

            return result.Data ?? throw new Exception("AI 服务返回数据为空");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用 AI 标题标签接口失败");
            throw;
        }
    }

    /// <summary>
    /// 解释代码
    /// </summary>
    public async Task<string> ExplainCodeAsync(string code, string? language = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("调用 AI 代码解释接口: CodeLength={Length}, Language={Language}",
                code.Length, language);

            var request = new CodeExplainRequestDto
            {
                Code = code,
                Language = language
            };

            var response = await _httpClient.PostAsJsonAsync("/tools/code-explain", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AiServiceResponse<CodeExplainResponseDto>>(
                cancellationToken: cancellationToken);

            if (result == null || !result.Success)
            {
                throw new Exception($"AI 服务返回错误: {result?.ErrorCode} - {result?.Message}");
            }

            return result.Data?.Explanation ?? throw new Exception("AI 服务返回解释为空");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用 AI 代码解释接口失败");
            throw;
        }
    }

    /// <summary>
    /// 健康检查
    /// </summary>
    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI 服务健康检查失败");
            return false;
        }
    }
}

