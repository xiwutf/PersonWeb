using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;
using Xunit;

namespace PersonalSite.Api.Tests;

public class ReadingServiceTests
{
    private static AppDbContext CreateDb()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static MindtraceIngestRequest SamplePayload(string externalId = "ext-1")
    {
        return new MindtraceIngestRequest
        {
            Version = "1",
            Source = "mindtrace",
            Type = "thought",
            ExternalId = externalId,
            SelectedText = "A quote",
            Note = "My thought",
            PageTitle = "Essay",
            PageUrl = "https://example.com/post",
            CreatedAt = "2026-09-14T08:00:00.000Z",
            Tags = new List<string> { "知识系统" },
        };
    }

    [Fact]
    public async Task Ingest_FirstTime_CreatesInboxPrivate()
    {
        await using AppDbContext db = CreateDb();
        var service = new ReadingService(db);

        (bool created, ReadingEntryDto entry) = await service.IngestMindtraceAsync(SamplePayload());

        Assert.True(created);
        Assert.Equal("mindtrace", entry.SourceType);
        Assert.Equal("ext-1", entry.ExternalId);
        Assert.Equal("A quote", entry.Quote);
        Assert.Equal("My thought", entry.Note);
        Assert.Equal("inbox", entry.Status);
        Assert.Equal("private", entry.Visibility);
        Assert.Contains("知识系统", entry.Tags);
    }

    [Fact]
    public async Task Ingest_SameExternalId_UpdatesContent_KeepsStatusVisibilityImportedAt()
    {
        await using AppDbContext db = CreateDb();
        var service = new ReadingService(db);

        (_, ReadingEntryDto first) = await service.IngestMindtraceAsync(SamplePayload());
        ReadingEntryDto? published = await service.ApplyActionAsync(first.Id, "publish");
        Assert.NotNull(published);
        DateTime importedAt = published!.ImportedAt;

        MindtraceIngestRequest second = SamplePayload();
        second.SelectedText = "Updated quote";
        second.Note = "Updated note";
        second.PageTitle = "New title";
        second.Tags = new List<string> { "阅读" };

        (bool created, ReadingEntryDto entry) = await service.IngestMindtraceAsync(second);

        Assert.False(created);
        Assert.Equal(first.Id, entry.Id);
        Assert.Equal("Updated quote", entry.Quote);
        Assert.Equal("Updated note", entry.Note);
        Assert.Equal("New title", entry.SourceTitle);
        Assert.Equal("published", entry.Status);
        Assert.Equal("public", entry.Visibility);
        Assert.Equal(importedAt, entry.ImportedAt);
        Assert.Equal(new[] { "阅读" }, entry.Tags);
        Assert.Single(await service.ListAsync());
    }

    [Fact]
    public async Task Ingest_MissingExternalId_Throws()
    {
        await using AppDbContext db = CreateDb();
        var service = new ReadingService(db);
        MindtraceIngestRequest payload = SamplePayload();
        payload.ExternalId = " ";

        await Assert.ThrowsAsync<ArgumentException>(() => service.IngestMindtraceAsync(payload));
    }

    [Fact]
    public async Task Ingest_EmptyContent_Throws()
    {
        await using AppDbContext db = CreateDb();
        var service = new ReadingService(db);
        MindtraceIngestRequest payload = SamplePayload();
        payload.SelectedText = "";
        payload.Note = "  ";

        await Assert.ThrowsAsync<ArgumentException>(() => service.IngestMindtraceAsync(payload));
    }

    [Fact]
    public async Task List_Publish_Archive_Delete_Work()
    {
        await using AppDbContext db = CreateDb();
        var service = new ReadingService(db);
        (_, ReadingEntryDto entry) = await service.IngestMindtraceAsync(SamplePayload("a"));
        await service.IngestMindtraceAsync(SamplePayload("b"));

        List<ReadingEntryDto> all = await service.ListAsync();
        Assert.Equal(2, all.Count);

        ReadingEntryDto? published = await service.ApplyActionAsync(entry.Id, "publish");
        Assert.Equal("published", published!.Status);
        Assert.Equal("public", published.Visibility);

        List<ReadingEntryDto> publishedOnly = await service.ListAsync("published");
        Assert.Single(publishedOnly);

        ReadingEntryDto? archived = await service.ApplyActionAsync(entry.Id, "archive");
        Assert.Equal("archived", archived!.Status);
        Assert.Equal("private", archived.Visibility);

        bool deleted = await service.DeleteAsync(entry.Id);
        Assert.True(deleted);
        Assert.Single(await service.ListAsync());
    }

    [Fact]
    public async Task ApplyAction_Invalid_Throws()
    {
        await using AppDbContext db = CreateDb();
        var service = new ReadingService(db);
        (_, ReadingEntryDto entry) = await service.IngestMindtraceAsync(SamplePayload());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ApplyActionAsync(entry.Id, "share"));
    }
}
