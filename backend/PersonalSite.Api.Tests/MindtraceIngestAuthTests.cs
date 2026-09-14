using PersonalSite.Api.Utils;
using Xunit;

namespace PersonalSite.Api.Tests;

public class MindtraceIngestAuthTests
{
    [Fact]
    public void Validate_WhenTokenNotConfigured_Returns503()
    {
        (int StatusCode, string Message)? result = MindtraceIngestAuth.Validate(
            "Bearer anything",
            "  ");

        Assert.NotNull(result);
        Assert.Equal(503, result!.Value.StatusCode);
        Assert.Contains("not configured", result.Value.Message);
    }

    [Fact]
    public void Validate_WhenTokenMissingOrWrong_Returns401()
    {
        Assert.Equal(401, MindtraceIngestAuth.Validate(null, "secret")!.Value.StatusCode);
        Assert.Equal(401, MindtraceIngestAuth.Validate("Bearer wrong", "secret")!.Value.StatusCode);
        Assert.Equal(401, MindtraceIngestAuth.Validate("Token secret", "secret")!.Value.StatusCode);
    }

    [Fact]
    public void Validate_WhenTokenMatches_ReturnsNull()
    {
        Assert.Null(MindtraceIngestAuth.Validate("Bearer secret", "secret"));
        Assert.Null(MindtraceIngestAuth.Validate("bearer secret", "secret"));
    }
}
