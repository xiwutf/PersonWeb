using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace PersonalSite.Api.Services.Reading;

// Reuse the installed Calibre CLI; only validated EPUB bytes enter normal import/storage.
public sealed class CalibreBookConverter(PrivateBookStorage storage, IOptions<ReadingLibraryOptions> options,
    ILogger<CalibreBookConverter> logger)
{
    private readonly SemaphoreSlim gate = new(1, 1);

    public async Task<T> ConvertAsync<T>(IFormFile upload, Func<IFormFile, Task<T>> import, CancellationToken ct)
    {
        if (!await gate.WaitAsync(0, ct))
            throw new ReadingLibraryException(429, "conversion_busy", "Another MOBI conversion is running. Retry shortly.");
        string? workspace = null;
        try
        {
            workspace = storage.CreateConversionDirectory();
            var source = Path.Combine(workspace, "input.mobi");
            var target = Path.Combine(workspace, "output.epub");
            await using (var input = upload.OpenReadStream())
            await using (var output = new FileStream(source, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 81920, FileOptions.Asynchronous))
            {
                var buffer = new byte[81920]; int count;
                while ((count = await input.ReadAsync(buffer, ct)) > 0)
                {
                    if (output.Length + count > ReadingLibraryOptions.MaxUploadBytes)
                        throw new ReadingLibraryException(413, "upload_too_large", "Maximum book size is 100 MiB.");
                    await output.WriteAsync(buffer.AsMemory(0, count), ct);
                }
                if (output.Length != upload.Length || output.Length < 78)
                    throw InvalidMobi();
                output.Position = 60;
                var signature = new byte[8]; await output.ReadExactlyAsync(signature, ct);
                if (!signature.AsSpan().SequenceEqual("BOOKMOBI"u8)) throw InvalidMobi();
            }
            var executable = ResolveExecutable(options.Value.CalibreExecutable);
            await RunAsync(executable, source, target, workspace, ct);
            if ((File.GetAttributes(target) & FileAttributes.ReparsePoint) != 0)
                throw new ReadingLibraryException(422, "conversion_failed", "Converted file cannot be a symbolic link.");
            try { CalibreEpubSanitizer.SanitizeFile(target); }
            catch (ReadingLibraryException) { throw; }
            catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException or System.Xml.XmlException)
            {
                logger.LogWarning(ex, "Failed to sanitize Calibre EPUB before import");
                throw new ReadingLibraryException(422, "conversion_failed", "Converted EPUB could not be prepared for import.");
            }
            await using var converted = new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous);
            if (converted.Length <= 0 || converted.Length > ReadingLibraryOptions.MaxUploadBytes)
                throw new ReadingLibraryException(413, "conversion_limit", "Converted EPUB exceeds the size limit.");
            var file = new FormFile(converted, 0, converted.Length, "file", "converted.epub")
                { Headers = new HeaderDictionary(), ContentType = "application/epub+zip" };
            try { return await import(file); }
            catch (ReadingLibraryException ex) when (ex.StatusCode == 422)
            {
                // Calibre 输出常含嵌入字体/复杂 CSS，被 strict-v2 拒绝；对用户标明是 MOBI 转换后的校验失败。
                throw new ReadingLibraryException(422, "conversion_rejected",
                    "Converted EPUB failed security validation after MOBI conversion.");
            }
        }
        finally
        {
            if (workspace != null)
            {
                try { storage.DeleteConversionDirectory(workspace); }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                { logger.LogError(ex, "Failed to remove private conversion workspace"); }
            }
            gate.Release();
        }
    }

    public static string ResolveExecutable(string configured)
    {
        if (!string.IsNullOrWhiteSpace(configured))
        {
            if (Path.IsPathFullyQualified(configured) && File.Exists(configured)) return configured;
            throw MissingConverter();
        }
        var name = OperatingSystem.IsWindows() ? "ebook-convert.exe" : "ebook-convert";
        var directories = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator).ToList();
        if (OperatingSystem.IsWindows()) directories.Insert(0, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Calibre2"));
        foreach (var directory in directories.Where(d => !string.IsNullOrWhiteSpace(d)))
        {
            var candidate = Path.Combine(directory.Trim('"'), name);
            if (Path.IsPathFullyQualified(candidate) && File.Exists(candidate)) return candidate;
        }
        throw MissingConverter();
    }

    private async Task RunAsync(string executable, string source, string target, string workspace, CancellationToken ct)
    {
        var limits = options.Value;
        if (limits.ConversionTimeoutSeconds is < 1 or > 600 || limits.ConversionMemoryBytes < 64 * 1024 * 1024)
            throw new ReadingLibraryException(503, "conversion_config", "Invalid conversion limits.");
        var start = new ProcessStartInfo(executable) { UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = workspace };
        // Prefer a lean EPUB so PersonWeb strict-v2 validation can accept Calibre output.
        foreach (var argument in new[] {
            source, target,
            "--no-svg-cover", "--no-default-epub-cover",
            "--epub-version", "2",
            "--disable-font-rescaling",
            "--filter-css", "font-family,color,background-color,background,float",
        }) start.ArgumentList.Add(argument);
        foreach (var variable in new[] { "CALIBRE_TEMP_DIR", "CALIBRE_CONFIG_DIRECTORY", "CALIBRE_CACHE_DIRECTORY" })
        {
            var folder = Path.Combine(workspace, variable); Directory.CreateDirectory(folder); start.Environment[variable] = folder;
        }
        start.Environment["CALIBRE_ALLOW_PYTHON_TEMPLATES"] = "0";
        using var process = new Process { StartInfo = start };
        try { process.Start(); }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        { logger.LogWarning(ex, "Calibre could not start"); throw MissingConverter(); }
        var stdout = Drain(process.StandardOutput); var stderr = Drain(process.StandardError);
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(ct);
        deadline.CancelAfter(TimeSpan.FromSeconds(limits.ConversionTimeoutSeconds));
        try
        {
            var exited = process.WaitForExitAsync(deadline.Token);
            while (!exited.IsCompleted)
            {
                await Task.WhenAny(exited, Task.Delay(200, deadline.Token));
                deadline.Token.ThrowIfCancellationRequested();
                if (process.HasExited) break;
                process.Refresh();
                if (process.WorkingSet64 > limits.ConversionMemoryBytes || WorkspaceBytes(workspace) > 3 * ReadingLibraryOptions.MaxUploadBytes)
                    throw new ReadingLibraryException(413, "conversion_limit", "Conversion resource limit exceeded.");
            }
            await exited;
            if (process.ExitCode != 0 || !File.Exists(target))
                throw new ReadingLibraryException(422, "conversion_failed", "MOBI conversion failed. DRM or damaged books are not supported.");
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { throw new ReadingLibraryException(504, "conversion_timeout", "MOBI conversion timed out."); }
        finally
        {
            if (!process.HasExited) { process.Kill(entireProcessTree: true); await process.WaitForExitAsync(CancellationToken.None); }
            await Task.WhenAll(stdout, stderr);
        }
    }

    private static long WorkspaceBytes(string workspace)
    {
        long total = 0; var count = 0;
        foreach (var file in Directory.EnumerateFiles(workspace, "*", new EnumerationOptions { RecurseSubdirectories = true, AttributesToSkip = FileAttributes.ReparsePoint }))
        {
            if (++count > 8000) return long.MaxValue;
            try { total += new FileInfo(file).Length; } catch (FileNotFoundException) { }
        }
        return total;
    }
    private static async Task Drain(StreamReader reader)
    {
        var buffer = new char[4096];
        while (await reader.ReadAsync(buffer) > 0) { } // No unbounded logs or private paths in HTTP errors.
    }
    private static ReadingLibraryException MissingConverter() => new(503, "converter_unavailable", "Install Calibre or configure ReadingLibrary:CalibreExecutable.");
    private static ReadingLibraryException InvalidMobi() => new(422, "invalid_mobi", "Invalid MOBI file.");
}
