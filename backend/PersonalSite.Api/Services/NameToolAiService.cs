using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>
/// 智能取名助手 AI 服务实现
/// 通过 HTTP 调用 Python AI 服务
/// </summary>
public class NameToolAiService : INameToolAiService
{
    private readonly HttpClient _httpClient;
    private readonly AiServiceOptions _options;
    private readonly ILogger<NameToolAiService> _logger;

    public NameToolAiService(
        HttpClient httpClient,
        IOptions<AiServiceOptions> options,
        ILogger<NameToolAiService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        // 配置 HttpClient
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    /// <summary>
    /// 生成名字
    /// 通过 HTTP 调用 Python AI 服务
    /// </summary>
    public async Task<NameGenerateResponseDto> GenerateAsync(
        NameGenerateRequestDto request,
        string? traceId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "调用 Python AI 服务生成名字: Type={Type}, Style={Style}, TraceId={TraceId}",
                request.Type, string.Join(",", request.Style), traceId);

            // 设置 traceId
            if (!string.IsNullOrEmpty(traceId))
            {
                request.TraceId = traceId;
            }

            // 添加内部调用 Token
            var internalToken = _options.InternalToken ?? "default-internal-token-change-in-production";
            
            // 构建完整 URL
            var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? _options.BaseUrl.TrimEnd('/');
            var requestUri = $"{baseAddress}/name/generate";
            
            _logger.LogDebug("请求 URL: {RequestUri}", requestUri);
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(request)
            };
            requestMessage.Headers.Add("X-Internal-Token", internalToken);

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            // 解析 Python 服务返回的响应
            var result = await response.Content.ReadFromJsonAsync<AiServiceResponse<JsonElement>>(
                cancellationToken: cancellationToken);

            if (result == null || !result.Success)
            {
                throw new Exception($"Python AI 服务返回错误: {result?.ErrorCode} - {result?.Message}");
            }

            // JsonElement 不能直接与 null 比较，需要使用 ValueKind 检查
            if (result.Data.ValueKind == JsonValueKind.Null || result.Data.ValueKind == JsonValueKind.Undefined)
            {
                throw new Exception("Python AI 服务返回数据为空");
            }

            // 将 JsonElement 转换为 NameGenerateResponseDto
            // Python 返回的 data 是 NameGenerateResponseData 的字典形式
            var responseDto = JsonSerializer.Deserialize<NameGenerateResponseDto>(
                result.Data.GetRawText(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            if (responseDto == null)
            {
                throw new Exception("Python AI 服务返回数据解析失败");
            }

            _logger.LogInformation(
                "Python AI 服务调用成功: TraceId={TraceId}, ItemsCount={Count}",
                responseDto.TraceId, responseDto.Items?.Count ?? 0);

            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用 Python AI 服务失败");
            throw;
        }
    }
}

