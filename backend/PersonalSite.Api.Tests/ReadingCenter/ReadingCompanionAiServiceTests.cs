using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;
using PersonalSite.Api.Services.Reading;
using Xunit;

namespace PersonalSite.Api.Tests.ReadingCenter;

public class ReadingCompanionAiServiceTests
{
    [Fact]
    public async Task Explain_ReturnsUpstreamReply_AndMapsFailure()
    {
        ReadingCompanionAiService ok = Create(new StubHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    success = true,
                    data = new { content = "这是解释" },
                    message = ""
                }), Encoding.UTF8, "application/json")
            }));
        ReadingExplainDto reply = await ok.ExplainAsync(new ReadingExplainRequest { Quote = "原文", Question = "什么意思" }, default);
        Assert.Equal("这是解释", reply.Reply);

        ReadingCompanionAiService failed = Create(new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.BadGateway)));
        ReadingLibraryException error = await Assert.ThrowsAsync<ReadingLibraryException>(() =>
            failed.ExplainAsync(new ReadingExplainRequest { Quote = "原文" }, default));
        Assert.Equal(502, error.StatusCode);
        Assert.Equal("ai_unavailable", error.Code);
    }

    [Fact]
    public async Task Explain_IncludesSurroundingContextInUpstreamPayload()
    {
        string? body = null;
        ReadingCompanionAiService service = Create(new StubHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    success = true,
                    data = new { content = "结合上下文解释" },
                    message = ""
                }), Encoding.UTF8, "application/json")
            };
        }));
        await service.ExplainAsync(new ReadingExplainRequest
        {
            Quote = "注意力",
            Context = "注意力 > 时间 > 金钱。注意力才是你所拥有的最宝贵的财富。",
            Question = "注意力是什么",
        }, default);
        using JsonDocument document = JsonDocument.Parse(body!);
        string joined = string.Join('\n', document.RootElement.GetProperty("messages")
            .EnumerateArray().Select(item => item.GetProperty("content").GetString() ?? ""));
        Assert.Contains("选中原文", joined, StringComparison.Ordinal);
        Assert.Contains("周围上下文", joined, StringComparison.Ordinal);
        Assert.Contains("注意力 > 时间 > 金钱", joined, StringComparison.Ordinal);
    }

    private static ReadingCompanionAiService Create(HttpMessageHandler handler)
    {
        return new ReadingCompanionAiService(
            new FixedFactory(new HttpClient(handler)),
            Options.Create(new AiServiceOptions { BaseUrl = "http://ai.test/api/ai", InternalToken = "token", TimeoutSeconds = 30 }),
            NullLogger<ReadingCompanionAiService>.Instance);
    }

    private sealed class FixedFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(respond(request));
    }
}
