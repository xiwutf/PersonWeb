using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services.Reading;

/// <summary>
/// 阅读伴读 AI 代理：复用 Python website-chat，密钥仅留在服务端。
/// </summary>
public sealed class ReadingCompanionAiService
{
    private const string SystemPrompt =
        "你是私人电子书阅读助手。根据用户给出的原文片段解释含义、难词或背景。" +
        "回答简洁、准确，使用中文。不要编造原文没有的情节。若信息不足请直接说明。";

    private readonly HttpClient httpClient;
    private readonly AiServiceOptions options;
    private readonly ILogger<ReadingCompanionAiService> logger;

    public ReadingCompanionAiService(IHttpClientFactory httpClientFactory, IOptions<AiServiceOptions> aiOptions,
        ILogger<ReadingCompanionAiService> logger)
    {
        httpClient = httpClientFactory.CreateClient();
        options = aiOptions.Value;
        this.logger = logger;
        httpClient.Timeout = TimeSpan.FromSeconds(Math.Clamp(options.TimeoutSeconds, 10, 120));
    }

    public async Task<ReadingExplainDto> ExplainAsync(ReadingExplainRequest input, CancellationToken cancellationToken)
    {
        string quote = (input.Quote ?? "").Trim();
        string question = string.IsNullOrWhiteSpace(input.Question)
            ? "请解释这段原文的含义，并说明关键词语或难句。"
            : input.Question.Trim();
        if (quote.Length == 0 || quote.Length > 4000 || question.Length > 2000)
            throw new ReadingLibraryException(400, "invalid_explain", "Quote or question is invalid.");
        if (input.History is { Count: > 10 })
            throw new ReadingLibraryException(400, "invalid_explain", "Conversation history is too long.");

        List<object> messages = new()
        {
            new { role = "system", content = SystemPrompt },
            new { role = "user", content = "原文：\n" + quote }
        };
        if (input.History != null)
        {
            foreach (ReadingChatTurn turn in input.History)
            {
                string role = turn.Role?.Trim().ToLowerInvariant() ?? "";
                string content = (turn.Content ?? "").Trim();
                if ((role != "user" && role != "assistant") || content.Length == 0 || content.Length > 8000)
                    throw new ReadingLibraryException(400, "invalid_explain", "Invalid conversation history.");
                messages.Add(new { role, content });
            }
        }
        messages.Add(new { role = "user", content = question });

        string internalToken = options.InternalToken ?? "default-internal-token-change-in-production";
        string baseAddress = (options.BaseUrl ?? "http://localhost:8001/api/ai").TrimEnd('/');
        string requestUri = $"{baseAddress}/website-chat";
        object payload = new { messages, scene = "reading-companion", extra = new { page = "library-reader" } };

        try
        {
            using HttpRequestMessage request = new(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.TryAddWithoutValidation("X-Internal-Token", internalToken);
            using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Reading companion AI upstream failed: {Status}", response.StatusCode);
                throw new ReadingLibraryException(502, "ai_unavailable", "AI explanation is temporarily unavailable.");
            }

            // ReadFromJsonAsync 使用 Web 默认：camelCase + 大小写不敏感，对齐 Python BaseResponse。
            AiServiceResponse<CompanionChatPayload>? result =
                await response.Content.ReadFromJsonAsync<AiServiceResponse<CompanionChatPayload>>(cancellationToken);
            string? reply = result?.Data?.Content?.Trim();
            if (result is not { Success: true } || string.IsNullOrEmpty(reply))
            {
                logger.LogWarning("Reading companion AI returned empty payload: {Message}", result?.Message);
                throw new ReadingLibraryException(502, "ai_unavailable", "AI explanation is temporarily unavailable.");
            }
            return new ReadingExplainDto(reply);
        }
        catch (ReadingLibraryException) { throw; }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Reading companion AI proxy failed");
            throw new ReadingLibraryException(502, "ai_unavailable", "AI explanation is temporarily unavailable.");
        }
    }

    private sealed class CompanionChatPayload
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = "";
    }
}
