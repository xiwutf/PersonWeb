using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services.Reading;
using Xunit;

namespace PersonalSite.Api.Tests.ReadingCenter;

public class ReadingLibraryServiceTests
{
    private static AppDbContext Db(SaveChangesInterceptor? interceptor = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        if (interceptor != null) options.AddInterceptors(interceptor);
        return new(options.Options);
    }

    private static ReadingLibraryService Service(AppDbContext db, ReadingTestEnvironment environment) => new(db,
        new PrivateBookStorage(environment, Options.Create(new ReadingLibraryOptions())),
        new EpubImportValidator(Options.Create(new ReadingLibraryOptions())), NullLogger<ReadingLibraryService>.Instance);

    internal static FormFile Upload(byte[] bytes, string name = "book.epub") => new(new MemoryStream(bytes), 0, bytes.Length, "file", name)
        { Headers = new HeaderDictionary(), ContentType = "application/epub+zip" };

    [Fact]
    public async Task Import_DeduplicatesPrivateBytes_AndDoesNotExposeStoragePath()
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db();
        var service = Service(db, environment);
        var bytes = EpubTestFiles.Create();
        var first = await service.ImportAsync("11111111-1111-4111-8111-111111111111", Upload(bytes), default);
        var second = await service.ImportAsync("11111111-1111-4111-8111-111111111111", Upload(bytes), default);
        Assert.Equal(first.Id, second.Id);
        Assert.Single(await db.ReadingBooks.ToListAsync());
        Assert.Equal("strict-v2", (await db.ReadingBooks.SingleAsync()).ValidationProfile);
        Assert.Single(Directory.GetFiles(environment.PrivateRoot));
        Assert.Empty(Directory.GetFiles(environment.WebRootPath, "*", SearchOption.AllDirectories));
        await using var content = await service.OpenAsync("11111111-1111-4111-8111-111111111111", first.Id, default);
        using var actual = new MemoryStream();
        await content.CopyToAsync(actual);
        Assert.Equal(bytes, actual.ToArray());
    }

    [Fact]
    public async Task OtherOwner_CannotGetBookOrContent_AndCannotListIt()
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db();
        var service = Service(db, environment);
        var book = await service.ImportAsync("11111111-1111-4111-8111-111111111111", Upload(EpubTestFiles.Create()), default);
        Assert.Equal(404, (await Assert.ThrowsAsync<ReadingLibraryException>(() => service.GetAsync("22222222-2222-4222-8222-222222222222", book.Id, default))).StatusCode);
        Assert.Equal(404, (await Assert.ThrowsAsync<ReadingLibraryException>(() => service.OpenAsync("22222222-2222-4222-8222-222222222222", book.Id, default))).StatusCode);
        Assert.Empty((await service.ListAsync("22222222-2222-4222-8222-222222222222", 1, 24, null, default)).Items);
    }

    [Fact]
    public async Task InvalidContent_LeavesNoRowsOrTemporaryFiles()
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db();
        var service = Service(db, environment);
        // 损坏/非法 ZIP 无法清洗修复，必须失败且不留库与临时文件。
        await Assert.ThrowsAsync<ReadingLibraryException>(() => service.ImportAsync("11111111-1111-4111-8111-111111111111",
            Upload(Encoding.UTF8.GetBytes("not-an-epub")), default));
        Assert.Empty(await db.ReadingBooks.ToListAsync());
        Assert.Empty(Directory.GetFiles(environment.PrivateRoot));
    }

    [Fact]
    public async Task CalibreLikeEpub_IsAcceptedAfterSanitize()
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db();
        var service = Service(db, environment);
        var path = Path.Combine(Path.GetTempPath(), "import-calibre-" + Guid.NewGuid().ToString("N") + ".epub");
        try
        {
            // 复用 sanitizer 测试夹具：含字体 / !important / rgb()，直接上传应先拒后洗再过。
            File.WriteAllBytes(path, CreateCalibreLikeUploadBytes());
            var book = await service.ImportAsync("11111111-1111-4111-8111-111111111111",
                Upload(await File.ReadAllBytesAsync(path)), default);
            Assert.Equal("穷查理宝典", book.Title);
            Assert.Single(await db.ReadingBooks.ToListAsync());
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    private static byte[] CreateCalibreLikeUploadBytes()
    {
        // 与 CalibreEpubSanitizerTests 夹具同构，避免测试互相引用私有方法。
        using var stream = new MemoryStream();
        using (var zip = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            void Write(string name, byte[] bytes, System.IO.Compression.CompressionLevel level = System.IO.Compression.CompressionLevel.Optimal)
            {
                var entry = zip.CreateEntry(name, level);
                using var output = entry.Open();
                output.Write(bytes);
            }
            Write("mimetype", Encoding.ASCII.GetBytes("application/epub+zip"), System.IO.Compression.CompressionLevel.NoCompression);
            Write("META-INF/container.xml", Encoding.UTF8.GetBytes("""
                <?xml version="1.0"?>
                <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
                  <rootfiles><rootfile full-path="content.opf" media-type="application/oebps-package+xml"/></rootfiles>
                </container>
                """));
            Write("content.opf", Encoding.UTF8.GetBytes("""
                <?xml version="1.0" encoding="utf-8"?>
                <package xmlns="http://www.idpf.org/2007/opf" unique-identifier="id" version="2.0">
                  <metadata xmlns:dc="http://purl.org/dc/elements/1.1/">
                    <dc:title>穷查理宝典</dc:title><dc:creator>Charlie</dc:creator><dc:language>zh</dc:language>
                  </metadata>
                  <manifest>
                    <item id="css" href="stylesheet.css" media-type="text/css"/>
                    <item id="font" href="fonts/Book.ttf" media-type="application/x-font-ttf"/>
                    <item id="c1" href="chapter.xhtml" media-type="application/xhtml+xml"/>
                  </manifest>
                  <spine><itemref idref="c1"/></spine>
                </package>
                """));
            Write("stylesheet.css", Encoding.UTF8.GetBytes("@font-face{font-family:X;src:url(fonts/Book.ttf)} p{color:rgb(0,0,0)!important;margin:0}"));
            Write("fonts/Book.ttf", new byte[] { 0, 1, 2, 3 });
            Write("chapter.xhtml", Encoding.UTF8.GetBytes("""
                <?xml version="1.0" encoding="utf-8"?>
                <html xmlns="http://www.w3.org/1999/xhtml"><head><title>t</title>
                <link rel="stylesheet" href="stylesheet.css"/></head>
                <body><p>正文</p></body></html>
                """));
        }
        return stream.ToArray();
    }

    [Fact]
    public async Task DatabaseFailure_CleansCommittedFile()
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db(new FailSave());
        await Assert.ThrowsAsync<DbUpdateException>(() => Service(db, environment).ImportAsync("11111111-1111-4111-8111-111111111111",
            Upload(EpubTestFiles.Create()), default));
        Assert.Empty(Directory.GetFiles(environment.PrivateRoot));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LostCommitAcknowledgement_DoesNotDeletePersistedBook(bool cancellation)
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db(new LostAcknowledgement(cancellation));
        var service = Service(db, environment);
        var bytes = EpubTestFiles.Create();
        var book = await service.ImportAsync("11111111-1111-4111-8111-111111111111", Upload(bytes), default);
        Assert.Single(await db.ReadingBooks.ToListAsync());
        Assert.Single(Directory.GetFiles(environment.PrivateRoot));
        await using var stream = await service.OpenAsync("11111111-1111-4111-8111-111111111111", book.Id, default);
        using var actual = new MemoryStream();
        await stream.CopyToAsync(actual);
        Assert.Equal(bytes, actual.ToArray());
    }

    [Fact]
    public async Task Cancellation_CleansTemporaryFile()
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Service(db, environment).ImportAsync("11111111-1111-4111-8111-111111111111",
            Upload(EpubTestFiles.Create()), cancellation.Token));
        Assert.Empty(Directory.GetFiles(environment.PrivateRoot));
    }

    [Fact]
    public async Task OversizedFile_IsRejectedBeforeStorage()
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db();
        var file = new FormFile(Stream.Null, 0, ReadingLibraryOptions.MaxUploadBytes + 1, "file", "book.epub");
        var error = await Assert.ThrowsAsync<ReadingLibraryException>(() => Service(db, environment).ImportAsync("11111111-1111-4111-8111-111111111111", file, default));
        Assert.Equal(413, error.StatusCode);
        Assert.False(Directory.Exists(environment.PrivateRoot));
    }

    [Fact]
    public async Task MissingContent_ReturnsControlledNotFound()
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db();
        var service = Service(db, environment);
        var book = await service.ImportAsync("11111111-1111-4111-8111-111111111111", Upload(EpubTestFiles.Create()), default);
        File.Delete(Directory.GetFiles(environment.PrivateRoot).Single());
        Assert.Equal("content_missing", (await Assert.ThrowsAsync<ReadingLibraryException>(() => service.OpenAsync("11111111-1111-4111-8111-111111111111", book.Id, default))).Code);
    }

    [Fact]
    public async Task List_PaginatesAndSortsByActualReadingTime()
    {
        using var environment = new ReadingTestEnvironment();
        await using var db = Db();
        var service = Service(db, environment);
        var first = await service.ImportAsync("11111111-1111-4111-8111-111111111111", Upload(EpubTestFiles.Create("<p>first</p>")), default);
        await service.ImportAsync("11111111-1111-4111-8111-111111111111", Upload(EpubTestFiles.Create("<p>second</p>")), default);
        db.ReadingProgress.Add(new ReadingProgress { OwnerKey = "11111111-1111-4111-8111-111111111111", BookId = first.Id,
            ContentVersion = 1, Cfi = "epubcfi(/6/2!/4/2/1:0)", Progression = 0.25, LastReadAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
        var page = await service.ListAsync("11111111-1111-4111-8111-111111111111", 1, 1, "测试", default);
        Assert.Equal(2, page.Total);
        Assert.Equal(first.Id, Assert.Single(page.Items).Id);
        Assert.Equal(0.25, page.Items[0].Progression);
        Assert.Single((await service.ListAsync("11111111-1111-4111-8111-111111111111", 2, 1, null, default)).Items);
        await Assert.ThrowsAsync<ReadingLibraryException>(() => service.ListAsync("11111111-1111-4111-8111-111111111111", 0, 24, null, default));
    }

    [Fact]
    public void Storage_RejectsPublicRootsAndTraversalKeys()
    {
        using var environment = new ReadingTestEnvironment();
        Assert.Throws<InvalidOperationException>(() => new PrivateBookStorage(environment,
            Options.Create(new ReadingLibraryOptions { StorageRoot = "wwwroot/books" })));
        Assert.Throws<InvalidOperationException>(() => new PrivateBookStorage(environment,
            Options.Create(new ReadingLibraryOptions { StorageRoot = "../../public/books" })));
        var storage = new PrivateBookStorage(environment, Options.Create(new ReadingLibraryOptions()));
        Assert.Throws<IOException>(() => storage.Open("../../secret.epub"));
        Assert.Throws<IOException>(() => storage.Open("invalid.epub"));
    }

    [Fact]
    public void Model_UsesOwnershipForeignKeysAndConcurrencyTokens()
    {
        using var db = Db();
        foreach (var type in new[] { typeof(ReadingProgress), typeof(ReadingAnnotation) })
        {
            var entity = db.Model.FindEntityType(type)!;
            Assert.True(entity.FindProperty("Revision")!.IsConcurrencyToken);
            Assert.Contains(entity.GetForeignKeys(), fk => fk.Properties.Select(p => p.Name).SequenceEqual(new[] { "OwnerKey", "BookId" }));
        }
        foreach (var type in new[] { typeof(ReadingBook), typeof(ReadingProgress), typeof(ReadingAnnotation), typeof(ReadingPreference), typeof(ReadingSession) })
        {
            var entity = db.Model.FindEntityType(type)!;
            Assert.Equal(typeof(string), entity.FindProperty("OwnerKey")!.ClrType);
            Assert.DoesNotContain(entity.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType == typeof(User));
        }
    }

    [Fact]
    public async Task Annotations_CreateListAndRejectOtherOwner()
    {
        using ReadingTestEnvironment environment = new();
        await using AppDbContext db = Db();
        ReadingLibraryService service = Service(db, environment);
        string owner = "11111111-1111-4111-8111-111111111111";
        ReadingBookDto book = await service.ImportAsync(owner, Upload(EpubTestFiles.Create()), default);
        string requestId = Guid.NewGuid().ToString();
        CreateReadingAnnotationRequest input = new()
        {
            ContentVersion = book.ContentVersion,
            CfiRange = "epubcfi(/6/2!/4/2,/1:0,/1:8)",
            ChapterHref = "chapter.xhtml",
            Quote = "selected quote",
            Note = "private note",
            Kind = "note",
            Color = "yellow",
            RequestId = requestId,
        };
        ReadingAnnotationDto created = await service.CreateAnnotationAsync(owner, book.Id, input, default);
        Assert.Equal(created, await service.CreateAnnotationAsync(owner, book.Id, input, default));
        Assert.Equal("private note", Assert.Single(await service.ListAnnotationsAsync(owner, book.Id, default)).Note);
        ReadingAnnotationFeedItem feed = Assert.Single(await service.ListAnnotationFeedAsync(owner, default));
        Assert.Equal(book.Title, feed.BookTitle);
        Assert.Equal("private note", feed.Note);
        Assert.Empty(await service.ListAnnotationFeedAsync("22222222-2222-4222-8222-222222222222", default));
        Assert.Equal(404, (await Assert.ThrowsAsync<ReadingLibraryException>(() =>
            service.ListAnnotationsAsync("22222222-2222-4222-8222-222222222222", book.Id, default))).StatusCode);
        input.RequestId = Guid.NewGuid().ToString();
        input.Note = "";
        Assert.Equal(400, (await Assert.ThrowsAsync<ReadingLibraryException>(() =>
            service.CreateAnnotationAsync(owner, book.Id, input, default))).StatusCode);
    }

    private sealed class FailSave : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default) =>
            throw new DbUpdateException("Simulated database failure");
    }

    private sealed class LostAcknowledgement(bool cancellation) : SaveChangesInterceptor
    {
        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
            CancellationToken cancellationToken = default) => cancellation
            ? throw new OperationCanceledException("Commit succeeded, acknowledgement canceled")
            : throw new DbUpdateException("Commit succeeded, acknowledgement lost");
    }
}
