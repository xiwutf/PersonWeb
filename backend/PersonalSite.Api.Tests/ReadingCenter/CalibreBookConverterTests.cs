using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Data;
using PersonalSite.Api.Services.Reading;
using Xunit;

namespace PersonalSite.Api.Tests.ReadingCenter;

public sealed class CalibreAvailableFactAttribute : FactAttribute
{
    public CalibreAvailableFactAttribute()
    {
        try { CalibreBookConverter.ResolveExecutable(""); }
        catch (ReadingLibraryException) { Skip = "Installed Calibre ebook-convert is required for real conversion verification."; }
    }
}

public sealed class ConversionLifecycleFactAttribute : FactAttribute
{
    public ConversionLifecycleFactAttribute()
    {
        if (CalibreBookConverterTests.TestPython() == null)
            Skip = "An existing Python executable is required only for the harmless child-process lifecycle fixture.";
    }
}

public class CalibreBookConverterTests
{
    public static string? TestPython()
    {
        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
        foreach (var name in OperatingSystem.IsWindows() ? new[] { "python.exe" } : new[] { "python3", "python" })
        {
            if (string.IsNullOrWhiteSpace(directory)) continue;
            var path = Path.Combine(directory.Trim('"'), name);
            if (Path.IsPathFullyQualified(path) && File.Exists(path) && new FileInfo(path).Length > 0) return path;
        }
        return null;
    }

    private static byte[] SleepingProcessFixture()
    {
        // Python is only a deterministic, harmless stand-in for a running converter process.
        var prefix = "import time\n#";
        return Encoding.ASCII.GetBytes(prefix + new string(' ', 60 - prefix.Length)
            + "BOOKMOBI\nopen('started.marker', 'w').write('ready')\ntime.sleep(30)\n");
    }

    [ConversionLifecycleFact]
    public async Task RunningConversionCancellation_KillsProcessCleansWorkspaceAndReleasesGate()
    {
        using var environment = new ReadingTestEnvironment();
        var options = Options.Create(new ReadingLibraryOptions { CalibreExecutable = TestPython()! });
        var storage = new PrivateBookStorage(environment, options);
        var converter = new CalibreBookConverter(storage, options, NullLogger<CalibreBookConverter>.Instance);
        using var canceled = new CancellationTokenSource();
        var task = converter.ConvertAsync(ReadingLibraryServiceTests.Upload(SleepingProcessFixture(), "test.mobi"), _ => Task.FromResult(1), canceled.Token);
        var deadline = Stopwatch.StartNew();
        try
        {
            while (!Directory.GetFiles(environment.PrivateRoot, "started.marker", SearchOption.AllDirectories).Any())
            {
                if (task.IsCompleted || deadline.Elapsed > TimeSpan.FromSeconds(10)) throw new Exception("Child fixture did not start.");
                await Task.Delay(20);
            }
            canceled.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
            Assert.Empty(Directory.GetFileSystemEntries(environment.PrivateRoot));
            var next = await Assert.ThrowsAsync<ReadingLibraryException>(() => converter.ConvertAsync(
                ReadingLibraryServiceTests.Upload(new byte[5], "test.mobi"), _ => Task.FromResult(1), default));
            Assert.Equal("invalid_mobi", next.Code);
        }
        finally { canceled.Cancel(); try { await task; } catch { } }
    }

    [ConversionLifecycleFact]
    public async Task RunningConversionTimeout_KillsProcessAndCleansWorkspace()
    {
        using var environment = new ReadingTestEnvironment();
        var options = Options.Create(new ReadingLibraryOptions { CalibreExecutable = TestPython()!, ConversionTimeoutSeconds = 1 });
        var storage = new PrivateBookStorage(environment, options);
        var converter = new CalibreBookConverter(storage, options, NullLogger<CalibreBookConverter>.Instance);
        var ex = await Assert.ThrowsAsync<ReadingLibraryException>(() => converter.ConvertAsync(
            ReadingLibraryServiceTests.Upload(SleepingProcessFixture(), "test.mobi"), _ => Task.FromResult(1), default));
        Assert.Equal(504, ex.StatusCode); Assert.Equal("conversion_timeout", ex.Code);
        Assert.Empty(Directory.GetFileSystemEntries(environment.PrivateRoot));
    }
    [CalibreAvailableFact]
    public async Task RealCalibre_MobiUploadProducesValidatedPrivateEpub_AndCleansWorkspace()
    {
        using var environment = new ReadingTestEnvironment();
        var options = Options.Create(new ReadingLibraryOptions());
        var storage = new PrivateBookStorage(environment, options);
        var converter = new CalibreBookConverter(storage, options, NullLogger<CalibreBookConverter>.Instance);
        var workspace = storage.CreateConversionDirectory();
        byte[] mobi;
        try
        {
            var source = Path.Combine(workspace, "fixture.epub"); var target = Path.Combine(workspace, "fixture.mobi");
            await File.WriteAllBytesAsync(source, EpubTestFiles.Create());
            var start = new ProcessStartInfo(CalibreBookConverter.ResolveExecutable("")) { UseShellExecute = false, CreateNoWindow = true,
                RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = workspace };
            foreach (var arg in new[] { source, target }) start.ArgumentList.Add(arg);
            start.Environment["CALIBRE_CONFIG_DIRECTORY"] = Path.Combine(workspace, "config");
            start.Environment["CALIBRE_TEMP_DIR"] = workspace;
            start.Environment["CALIBRE_CACHE_DIRECTORY"] = Path.Combine(workspace, "cache");
            using var process = Process.Start(start)!;
            var stdout = process.StandardOutput.ReadToEndAsync(); var stderr = process.StandardError.ReadToEndAsync();
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            try { await process.WaitForExitAsync(timeout.Token); }
            finally { if (!process.HasExited) { process.Kill(true); await process.WaitForExitAsync(); } }
            var log = await stdout + await stderr;
            Assert.True(process.ExitCode == 0, log);
            mobi = await File.ReadAllBytesAsync(target);
        }
        finally { storage.DeleteConversionDirectory(workspace); }
        await using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var service = new ReadingLibraryService(db, storage, new EpubImportValidator(options), NullLogger<ReadingLibraryService>.Instance, converter);
        var book = await service.ImportAsync("11111111-1111-4111-8111-111111111111", ReadingLibraryServiceTests.Upload(mobi, "test.mobi"), default);
        Assert.Equal(1, book.ContentVersion);
        await using var content = await service.OpenAsync("11111111-1111-4111-8111-111111111111", book.Id, default);
        Assert.NotEmpty(new EpubImportValidator(options).Validate(content).Title);
        Assert.Single(await db.ReadingBooks.ToListAsync());
        Assert.Single(Directory.GetFiles(environment.PrivateRoot));
        Assert.Empty(Directory.GetDirectories(environment.PrivateRoot));
        Assert.Empty(Directory.GetFiles(environment.WebRootPath, "*", SearchOption.AllDirectories));
    }

    [Fact]
    public async Task MissingConverter_FailsWithoutLeavingPrivateFiles()
    {
        using var environment = new ReadingTestEnvironment();
        var settings = new ReadingLibraryOptions { CalibreExecutable = Path.Combine(environment.ContentRootPath, "missing-ebook-convert.exe") };
        var options = Options.Create(settings); var storage = new PrivateBookStorage(environment, options);
        var converter = new CalibreBookConverter(storage, options, NullLogger<CalibreBookConverter>.Instance);
        var input = new byte[80]; Encoding.ASCII.GetBytes("BOOKMOBI").CopyTo(input, 60);
        var ex = await Assert.ThrowsAsync<ReadingLibraryException>(() => converter.ConvertAsync(
            ReadingLibraryServiceTests.Upload(input, "test.mobi"), _ => Task.FromResult(1), default));
        Assert.Equal(503, ex.StatusCode);
        if (Directory.Exists(environment.PrivateRoot)) Assert.Empty(Directory.GetFileSystemEntries(environment.PrivateRoot));
    }

    [Fact]
    public async Task MalformedMobi_IsRejectedEvenWhenCalibreIsNotInstalled()
    {
        using var environment = new ReadingTestEnvironment();
        var options = Options.Create(new ReadingLibraryOptions { CalibreExecutable = Path.Combine(environment.ContentRootPath, "missing-converter.exe") });
        var storage = new PrivateBookStorage(environment, options);
        var converter = new CalibreBookConverter(storage, options, NullLogger<CalibreBookConverter>.Instance);
        var ex = await Assert.ThrowsAsync<ReadingLibraryException>(() => converter.ConvertAsync(
            ReadingLibraryServiceTests.Upload(Encoding.UTF8.GetBytes("not a mobi"), "test.mobi"), _ => Task.FromResult(1), default));
        Assert.Equal(422, ex.StatusCode); Assert.Equal("invalid_mobi", ex.Code);
        Assert.Empty(Directory.GetFileSystemEntries(environment.PrivateRoot));
    }

    [CalibreAvailableFact]
    public async Task CalibreRejectsBrokenPalmDatabase_AndCancellationDoesNotImportAnything()
    {
        using var environment = new ReadingTestEnvironment();
        var options = Options.Create(new ReadingLibraryOptions()); var storage = new PrivateBookStorage(environment, options);
        var converter = new CalibreBookConverter(storage, options, NullLogger<CalibreBookConverter>.Instance);
        var corrupt = new byte[80]; Encoding.ASCII.GetBytes("BOOKMOBI").CopyTo(corrupt, 60);
        var imported = false;
        Task<int> Import(Microsoft.AspNetCore.Http.IFormFile _) { imported = true; return Task.FromResult(1); }
        var ex = await Assert.ThrowsAsync<ReadingLibraryException>(() => converter.ConvertAsync(ReadingLibraryServiceTests.Upload(corrupt, "test.mobi"), Import, default));
        Assert.Equal("conversion_failed", ex.Code); Assert.False(imported);
        using var canceled = new CancellationTokenSource(); canceled.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => converter.ConvertAsync(ReadingLibraryServiceTests.Upload(corrupt, "test.mobi"), Import, canceled.Token));
        Assert.Empty(Directory.GetFileSystemEntries(environment.PrivateRoot));
    }
}
