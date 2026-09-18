using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;
using PersonalSite.Api.Services.Cache;
using Xunit;

namespace PersonalSite.Api.Tests;

public class ContentInteractionServiceTests
{
    private static (AppDbContext db, ContentInteractionService service) CreateSut()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        AppDbContext db = new AppDbContext(options);
        ICacheService cache = new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()));
        return (db, new ContentInteractionService(db, cache));
    }

    private static CreateCommentRequestDto SampleComment(
        string targetType = "article",
        string targetId = "hello-world",
        string visitorId = "visitor-a")
    {
        return new CreateCommentRequestDto
        {
            TargetType = targetType,
            TargetId = targetId,
            Nickname = "溪午访客",
            Email = "guest@example.com",
            Content = "这篇很有启发。",
            VisitorId = visitorId,
        };
    }

    [Fact]
    public async Task CreateComment_Succeeds_AsPending()
    {
        (AppDbContext db, ContentInteractionService service) = CreateSut();

        ContentComment comment = await service.CreateCommentAsync(SampleComment(), "127.0.0.1");

        Assert.Equal("pending", comment.Status);
        Assert.Equal("article", comment.TargetType);
        Assert.Equal("hello-world", comment.TargetId);
        Assert.DoesNotContain("<", comment.Content);
    }

    [Fact]
    public async Task PendingComment_NotVisibleToPublic()
    {
        (_, ContentInteractionService service) = CreateSut();
        await service.CreateCommentAsync(SampleComment(), "127.0.0.1");

        List<PublicCommentDto> list = await service.ListApprovedCommentsAsync("article", "hello-world");
        Assert.Empty(list);
    }

    [Fact]
    public async Task ApprovedComment_VisibleToPublic_WithoutEmail()
    {
        (_, ContentInteractionService service) = CreateSut();
        ContentComment created = await service.CreateCommentAsync(SampleComment(), "127.0.0.1");
        await service.ApproveAsync(created.Id);

        List<PublicCommentDto> list = await service.ListApprovedCommentsAsync("article", "hello-world");
        Assert.Single(list);
        Assert.Equal("溪午访客", list[0].Nickname);
        Assert.Equal("这篇很有启发。", list[0].Content);
        // PublicCommentDto 本身不含 Email 字段，这里确认投影正常
        Assert.True(typeof(PublicCommentDto).GetProperty("Email") == null);
    }

    [Fact]
    public async Task RejectedComment_NotVisibleToPublic()
    {
        (_, ContentInteractionService service) = CreateSut();
        ContentComment created = await service.CreateCommentAsync(SampleComment(), "127.0.0.1");
        await service.ApproveAsync(created.Id);
        await service.RejectAsync(created.Id);

        List<PublicCommentDto> list = await service.ListApprovedCommentsAsync("article", "hello-world");
        Assert.Empty(list);
    }

    [Fact]
    public async Task Admin_CanApproveAndReject()
    {
        (_, ContentInteractionService service) = CreateSut();
        ContentComment created = await service.CreateCommentAsync(SampleComment(), "127.0.0.1");

        ContentComment? approved = await service.ApproveAsync(created.Id);
        Assert.NotNull(approved);
        Assert.Equal("approved", approved!.Status);

        ContentComment? rejected = await service.RejectAsync(created.Id);
        Assert.NotNull(rejected);
        Assert.Equal("rejected", rejected!.Status);
    }

    [Fact]
    public async Task Like_SameVisitor_DoesNotDuplicate()
    {
        (_, ContentInteractionService service) = CreateSut();

        (bool firstCreated, int count1) = await service.LikeAsync("article", "hello-world", "visitor-a");
        (bool secondCreated, int count2) = await service.LikeAsync("article", "hello-world", "visitor-a");

        Assert.True(firstCreated);
        Assert.False(secondCreated);
        Assert.Equal(1, count1);
        Assert.Equal(1, count2);
    }

    [Fact]
    public async Task Unlike_Works()
    {
        (_, ContentInteractionService service) = CreateSut();
        await service.LikeAsync("article", "hello-world", "visitor-a");

        (bool removed, int count) = await service.UnlikeAsync("article", "hello-world", "visitor-a");

        Assert.True(removed);
        Assert.Equal(0, count);

        InteractionSummaryDto summary = await service.GetSummaryAsync("article", "hello-world", "visitor-a");
        Assert.False(summary.Liked);
        Assert.Equal(0, summary.LikeCount);
    }

    [Fact]
    public async Task DifferentTargets_DoNotInterfere()
    {
        (_, ContentInteractionService service) = CreateSut();

        await service.LikeAsync("article", "a", "visitor-a");
        await service.LikeAsync("reading", "a", "visitor-a");
        await service.CreateCommentAsync(SampleComment("article", "a", "v1"), "1.1.1.1");
        ContentComment readingComment = await service.CreateCommentAsync(SampleComment("reading", "a", "v2"), "1.1.1.2");
        await service.ApproveAsync(readingComment.Id);

        InteractionSummaryDto article = await service.GetSummaryAsync("article", "a", "visitor-a");
        InteractionSummaryDto reading = await service.GetSummaryAsync("reading", "a", "visitor-a");

        Assert.Equal(1, article.LikeCount);
        Assert.True(article.Liked);
        Assert.Equal(0, article.CommentCount);

        Assert.Equal(1, reading.LikeCount);
        Assert.True(reading.Liked);
        Assert.Equal(1, reading.CommentCount);
    }

    [Fact]
    public void Sanitize_StripsHtml()
    {
        string cleaned = ContentInteractionService.SanitizePlainText("<script>alert(1)</script>你好");
        Assert.DoesNotContain("<script>", cleaned);
        Assert.Contains("你好", cleaned);
    }
}
