using System.Net.Http;
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
    public string BaseUrl { get; set; } = "http://localhost:8001/api/ai";
    
    /// <summary>
    /// 内部调用 Token（用于调用 Python Agent）
    /// </summary>
    public string? InternalToken { get; set; }
    
    /// <summary>
    /// 请求超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 300; // 文档处理可能需要较长时间
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
/// Python AI 服务的 WebsiteChat 响应格式
/// </summary>
internal class WebsiteChatResponseDto
{
    [System.Text.Json.Serialization.JsonPropertyName("content")]
    public string Content { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("usage")]
    public WebsiteChatUsageDto? Usage { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
}

/// <summary>
/// WebsiteChat 的 Usage 格式
/// </summary>
internal class WebsiteChatUsageDto
{
    [System.Text.Json.Serialization.JsonPropertyName("promptTokens")]
    public int PromptTokens { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("completionTokens")]
    public int CompletionTokens { get; set; }
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

            // Python AI 服务使用 /website-chat 端点，需要转换请求格式
            // AI 服务期望的格式：{ messages: [{role, content}], scene, extra }
            var extra = new Dictionary<string, object>
            {
                ["userId"] = request.UserId,
                ["sessionId"] = request.SessionId ?? "",
            };

            // 如果有 Meta 数据，合并到 extra 中
            if (request.Meta != null)
            {
                foreach (var kvp in request.Meta)
                {
                    extra[kvp.Key] = kvp.Value;
                }
            }

            // 构建 messages 数组（根据场景决定是否包含 system 消息）
            var messages = new List<object>();
            
            // 如果有场景信息，可以添加 system 消息
            var scene = extra.ContainsKey("scene") ? extra["scene"]?.ToString() : "website-chat";
            
            // 添加 user 消息
            messages.Add(new
            {
                role = "user",
                content = request.Message
            });

            var websiteChatRequest = new
            {
                messages = messages,
                scene = scene,
                extra = extra
            };

            // 添加内部调用 Token（从配置中读取）
            var internalToken = _options.InternalToken ?? "default-internal-token-change-in-production";
            
            // 构建完整的相对路径，确保路径正确拼接
            // BaseAddress 是 http://localhost:8001/api/ai，相对路径应该是 "/website-chat"（带前导斜杠）
            // 或者使用 "website-chat" 但确保 BaseAddress 以斜杠结尾
            // 使用前导斜杠的方式更可靠
            var relativePath = "/website-chat";
            var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? _options.BaseUrl.TrimEnd('/');
            var fullUrl = $"{baseAddress}{relativePath}";
            _logger.LogInformation("BaseAddress: {BaseAddress}, 相对路径: {RelativePath}, 完整 URL: {FullUrl}", 
                _httpClient.BaseAddress, relativePath, fullUrl);
            
            // 构建请求消息，添加内部 Token 请求头
            // 注意：使用完整 URL 而不是相对路径，避免 HttpClient 的 BaseAddress 拼接问题
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, fullUrl)
            {
                Content = JsonContent.Create(websiteChatRequest)
            };
            requestMessage.Headers.Add("X-Internal-Token", internalToken);

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            
            // 如果失败，记录详细信息
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("AI 服务请求失败: StatusCode={StatusCode}, URL={RequestUri}, Response={Response}",
                    response.StatusCode, fullUrl, errorContent);
            }
            
            response.EnsureSuccessStatusCode();

            // AI 服务返回的是 BaseResponse 包装格式：{ success, data, error_code, message }
            // 其中 data 是 WebsiteChatResponseDto
            var baseResponse = await response.Content.ReadFromJsonAsync<AiServiceResponse<WebsiteChatResponseDto>>(
                cancellationToken: cancellationToken);

            if (baseResponse == null || !baseResponse.Success || baseResponse.Data == null)
            {
                var errorMsg = baseResponse?.Message ?? "AI 服务返回数据为空";
                _logger.LogError("AI 服务返回错误: Success={Success}, Message={Message}, ErrorCode={ErrorCode}",
                    baseResponse?.Success ?? false, errorMsg, baseResponse?.ErrorCode);
                throw new Exception($"AI 服务返回错误: {errorMsg}");
            }

            var result = baseResponse.Data;

            // 转换为 ChatResponseDto 格式
            return new ChatResponseDto
            {
                Reply = result.Content ?? "",
                Model = request.Model ?? "default",
                Usage = result.Usage != null ? new TokenUsage
                {
                    PromptTokens = result.Usage.PromptTokens,
                    CompletionTokens = result.Usage.CompletionTokens,
                    TotalTokens = result.Usage.PromptTokens + result.Usage.CompletionTokens
                } : null
            };
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
            // 如果是连接被拒绝或超时等常见错误，使用 Debug 级别（服务可能未启动）
            // 其他错误使用 Warning 级别
            var isConnectionError = ex is HttpRequestException httpEx && 
                (httpEx.InnerException?.GetType().Name == "SocketException" || 
                 ex.Message.Contains("连接", StringComparison.OrdinalIgnoreCase) || 
                 ex.Message.Contains("Connection", StringComparison.OrdinalIgnoreCase) ||
                 ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                 ex.Message.Contains("超时", StringComparison.OrdinalIgnoreCase) ||
                 ex.Message.Contains("拒绝", StringComparison.OrdinalIgnoreCase) ||
                 ex.Message.Contains("refused", StringComparison.OrdinalIgnoreCase));
            
            if (isConnectionError)
            {
                _logger.LogDebug(ex, "AI 服务健康检查失败（服务可能未启动）");
            }
            else
            {
                _logger.LogWarning(ex, "AI 服务健康检查失败");
            }
            return false;
        }
    }

    /// <summary>
    /// 处理文档（解析+分段+摘要+知识结构）
    /// </summary>
    public async Task<DocumentProcessResponseDto> ProcessDocumentAsync(
        string documentId,
        string filePath,
        string fileType,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("调用文档处理接口: DocumentId={DocumentId}, FileType={FileType}, FilePath={FilePath}",
                documentId, fileType, filePath);

            var request = new DocumentProcessRequestDto
            {
                DocumentId = documentId,
                FilePath = filePath,
                FileType = fileType,
                UserId = userId
            };

            // 添加内部调用 Token（从配置中读取）
            var internalToken = _options.InternalToken ?? "default-internal-token-change-in-production";
            
            // 构建完整 URL（确保路径正确拼接）
            // 注意：如果 BaseAddress 是 http://localhost:8001/api/ai，relativePath 应该是 "document/process"（不带前导斜杠）
            // 这样会得到 http://localhost:8001/api/ai/document/process
            var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? _options.BaseUrl.TrimEnd('/');
            var requestUri = $"{baseAddress}/document/process";
            _logger.LogInformation("BaseAddress: {BaseAddress}, 请求 URL: {RequestUri}", 
                _httpClient.BaseAddress, requestUri);
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(request)
            };
            requestMessage.Headers.Add("X-Internal-Token", internalToken);

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AiServiceResponse<DocumentProcessResponseDto>>(
                cancellationToken: cancellationToken);

            if (result == null || !result.Success)
            {
                throw new Exception($"AI 服务返回错误: {result?.ErrorCode} - {result?.Message}");
            }

            return result.Data ?? throw new Exception("AI 服务返回数据为空");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用文档处理接口失败");
            throw;
        }
    }

    /// <summary>
    /// 文档问答
    /// </summary>
    public async Task<AiServiceDocumentQueryResponseDto> QueryDocumentAsync(
        string documentId,
        string userId,
        string query,
        int topK = 5,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("调用文档问答接口: DocumentId={DocumentId}, QueryLength={Length}",
                documentId, query.Length);

            var request = new DocumentQueryRequestDto
            {
                DocumentId = documentId,
                UserId = userId,
                Query = query,
                TopK = topK
            };

            // 添加内部调用 Token
            var internalToken = _options.InternalToken ?? "default-internal-token-change-in-production";
            
            // 构建完整 URL（确保路径正确拼接）
            var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? _options.BaseUrl.TrimEnd('/');
            var requestUri = $"{baseAddress}/document/query";
            _logger.LogInformation("BaseAddress: {BaseAddress}, 请求 URL: {RequestUri}", 
                _httpClient.BaseAddress, requestUri);
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(request)
            };
            requestMessage.Headers.Add("X-Internal-Token", internalToken);

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AiServiceResponse<AiServiceDocumentQueryResponseDto>>(
                cancellationToken: cancellationToken);

            if (result == null || !result.Success)
            {
                throw new Exception($"AI 服务返回错误: {result?.ErrorCode} - {result?.Message}");
            }

            return result.Data ?? throw new Exception("AI 服务返回数据为空");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用文档问答接口失败");
            throw;
        }
    }

    /// <summary>
    /// 请求思维批注（调用 python-ai POST /api/ai/thought_comment，鉴权头为 X-Internal-Key）
    /// </summary>
    public async Task<string> ThoughtCommentAsync(string content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("原文内容不能为空", nameof(content));
        }

        string internalKey = _options.InternalToken ?? "default-internal-token-change-in-production";
        string baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? _options.BaseUrl.TrimEnd('/');
        string requestUri = $"{baseAddress}/thought_comment";

        var requestBody = new { content = content };
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(requestBody)
        };
        // python-ai 内部接口使用 X-Internal-Key，复用配置项 InternalToken 的值
        requestMessage.Headers.Add("X-Internal-Key", internalKey);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(requestMessage, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "调用思维批注接口网络失败");
            throw new Exception("AI 服务不可用或网络超时，请稍后重试", ex);
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken != cancellationToken)
        {
            _logger.LogError(ex, "调用思维批注接口超时");
            throw new Exception("AI 批注请求超时，请稍后重试", ex);
        }

        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("思维批注接口返回错误: StatusCode={StatusCode}, Response={Response}",
                response.StatusCode, responseBody);
            string message = "AI 批注服务返回错误";
            try
            {
                var err = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(responseBody);
                if (err.TryGetProperty("detail", out var detail))
                    message = detail.GetString() ?? message;
            }
            catch { /* 忽略解析失败 */ }
            throw new Exception($"{message}（{(int)response.StatusCode}）");
        }

        try
        {
            var node = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(responseBody);
            if (node.TryGetProperty("comment_md", out var commentMd))
                return commentMd.GetString() ?? string.Empty;
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "解析思维批注响应失败: {Response}", responseBody);
        }

        throw new Exception("AI 批注返回格式异常，未包含 comment_md");
    }
}

/// <summary>
/// 文档处理请求 DTO
/// </summary>
public class DocumentProcessRequestDto
{
    [System.Text.Json.Serialization.JsonPropertyName("document_id")]
    public string DocumentId { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("file_path")]
    public string FilePath { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("file_type")]
    public string FileType { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("user_id")]
    public string UserId { get; set; } = "";
}

/// <summary>
/// 文档处理响应 DTO（AI 服务返回）
/// </summary>
public class DocumentProcessResponseDto
{
    [System.Text.Json.Serialization.JsonPropertyName("document_id")]
    public string DocumentId { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("summary")]
    public string Summary { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("knowledge_structure")]
    public string KnowledgeStructure { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("total_chunks")]
    public int TotalChunks { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("chunks")]
    public List<AiServiceDocumentChunkDto> Chunks { get; set; } = new();
}

/// <summary>
/// 文档分段 DTO（AI 服务返回）
/// </summary>
public class AiServiceDocumentChunkDto
{
    [System.Text.Json.Serialization.JsonPropertyName("index")]
    public int Index { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("content")]
    public string Content { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("summary")]
    public string? Summary { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("metadata")]
    public System.Text.Json.JsonElement? Metadata { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("vector_id")]
    public string? VectorId { get; set; }
}

/// <summary>
/// 文档问答请求 DTO
/// </summary>
public class DocumentQueryRequestDto
{
    [System.Text.Json.Serialization.JsonPropertyName("document_id")]
    public string DocumentId { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("user_id")]
    public string UserId { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("query")]
    public string Query { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("top_k")]
    public int TopK { get; set; } = 5;
}

/// <summary>
/// 文档问答响应 DTO（AI 服务返回）
/// </summary>
public class AiServiceDocumentQueryResponseDto
{
    [System.Text.Json.Serialization.JsonPropertyName("answer")]
    public string Answer { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("relevant_chunks")]
    public List<AiServiceRelevantChunkDto> RelevantChunks { get; set; } = new();
    
    [System.Text.Json.Serialization.JsonPropertyName("confidence")]
    public decimal? Confidence { get; set; }
}

/// <summary>
/// 相关文档片段 DTO（AI 服务返回）
/// </summary>
public class AiServiceRelevantChunkDto
{
    [System.Text.Json.Serialization.JsonPropertyName("chunk_id")]
    public long ChunkId { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("chunk_index")]
    public int ChunkIndex { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("content")]
    public string Content { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("summary")]
    public string? Summary { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("score")]
    public decimal Score { get; set; }
}

