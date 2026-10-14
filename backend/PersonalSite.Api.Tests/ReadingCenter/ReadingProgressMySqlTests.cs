using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services.Reading;
using Xunit;

namespace PersonalSite.Api.Tests.ReadingCenter;

public sealed class ReadingMySqlProgressFactAttribute : FactAttribute
{
    public ReadingMySqlProgressFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("READING_MYSQL_VERIFY_CONFIG") == null)
            Skip = "Opt-in real MySQL transactional verification; set READING_MYSQL_VERIFY_CONFIG.";
    }
}

public class ReadingProgressMySqlTests
{
    [ReadingMySqlProgressFact]
    public async Task ExistingMySqlSchema_ProgressWritesQueriesAndStaleRevision_RollBack()
    {
        var config = JsonDocument.Parse(await File.ReadAllTextAsync(Environment.GetEnvironmentVariable("READING_MYSQL_VERIFY_CONFIG")!));
        var connection = config.RootElement.GetProperty("ConnectionStrings").GetProperty("Default").GetString()!;
        var options = new DbContextOptionsBuilder<AppDbContext>().UseMySql(connection, ServerVersion.Parse("8.0.36-mysql")).Options;
        await using var db = new AppDbContext(options);
        await db.Database.OpenConnectionAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        using var environment = new ReadingTestEnvironment();
        ReadingLibraryService Service(AppDbContext context) => new(context,
            new PrivateBookStorage(environment, Options.Create(new ReadingLibraryOptions())),
            new EpubImportValidator(Options.Create(new ReadingLibraryOptions())), NullLogger<ReadingLibraryService>.Instance);
        var owner = Guid.NewGuid().ToString();
        db.ReadingBooks.Add(new ReadingBook { OwnerKey = owner, Title = "Temporary progress transaction verification",
            Sha256 = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"), StorageKey = "transaction-only.epub" });
        await db.SaveChangesAsync();
        var id = (await db.ReadingBooks.SingleAsync(b => b.OwnerKey == owner)).Id;
        var service = Service(db);
        var input = new SaveReadingProgressRequest { ContentVersion = 1, Cfi = "epubcfi(/6/2!/4/2/1:8)", ChapterHref = "chapter.xhtml", Progression = .6, SessionId = Guid.NewGuid().ToString() };
        var first = await service.SaveProgressAsync(owner, id, input, default);
        Assert.Equal(1, first.Revision);
        Assert.Equal(first, await service.SaveProgressAsync(owner, id, input, default));
        Assert.Equal(first, await service.ProgressAsync(owner, id, default));
        Assert.Equal(.6, (await service.ListAsync(owner, 1, 24, null, default)).Items.Single().Progression);
        Assert.Equal(id, (await service.ContinueAsync(owner, default))!.Book.Id);
        input.SessionId = Guid.NewGuid().ToString();
        Assert.Equal(409, (await Assert.ThrowsAsync<ReadingLibraryException>(() => service.SaveProgressAsync(owner, id, input, default))).StatusCode);
        // Two EF contexts load revision 1. MySQL UPDATE ... WHERE revision=1 must reject the stale context.
        var sharedOptions = new DbContextOptionsBuilder<AppDbContext>().UseMySql(db.Database.GetDbConnection(), ServerVersion.Parse("8.0.36-mysql")).Options;
        await using var staleDb = new AppDbContext(sharedOptions);
        await staleDb.Database.UseTransactionAsync(transaction.GetDbTransaction());
        var stale = await staleDb.ReadingProgress.SingleAsync(p => p.OwnerKey == owner && p.BookId == id);
        input.ExpectedRevision = 1; input.Progression = .2; input.Cfi = "epubcfi(/6/2!/4/2/1:2)";
        Assert.Equal(2, (await service.SaveProgressAsync(owner, id, input, default)).Revision);
        // Concurrent identical retry must acknowledge the committed write, not show a false conflict.
        Assert.Equal(2, (await Service(staleDb).SaveProgressAsync(owner, id, input, default)).Revision);
        staleDb.ReadingProgress.Attach(stale);
        staleDb.Entry(stale).Property(p => p.Revision).OriginalValue = 1;
        stale.Revision++; stale.Cfi = "epubcfi(/6/2!/4/2/1:40)";
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => staleDb.SaveChangesAsync());
        Assert.Equal(.2, (await service.ProgressAsync(owner, id, default))!.Progression);
        await transaction.RollbackAsync();
        Assert.False(await db.ReadingBooks.AsNoTracking().AnyAsync(b => b.OwnerKey == owner));
    }
}
