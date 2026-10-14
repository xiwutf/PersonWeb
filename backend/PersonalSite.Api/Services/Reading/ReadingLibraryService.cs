using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services.Reading;

public sealed class ReadingLibraryService(AppDbContext db, PrivateBookStorage storage,
    EpubImportValidator validator, ILogger<ReadingLibraryService> logger, CalibreBookConverter? converter = null)
{
    public async Task<ReadingBookDto> ImportAsync(string ownerKey, IFormFile file, CancellationToken ct)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (file.Length <= 0 || extension is not (".epub" or ".mobi"))
            throw new ReadingLibraryException(422, "invalid_epub", "Select a non-empty EPUB or MOBI file.");
        if (file.Length > ReadingLibraryOptions.MaxUploadBytes)
            throw new ReadingLibraryException(413, "upload_too_large", "Maximum EPUB size is 100 MiB.");
        if (extension == ".mobi")
        {
            if (converter == null) throw new ReadingLibraryException(503, "converter_unavailable", "Calibre conversion is not configured.");
            return await converter.ConvertAsync(file, converted => ImportAsync(ownerKey, converted, ct), ct);
        }

        string? temporaryKey = null, committedKey = null;
        try
        {
            var temporary = storage.CreateTemporary();
            temporaryKey = temporary.Key;
            EpubMetadata metadata;
            string hash;
            long size;
            await using (var destination = temporary.Stream)
            {
                await using var input = file.OpenReadStream();
                var buffer = new byte[81920];
                int read;
                while ((read = await input.ReadAsync(buffer, ct)) > 0)
                {
                    if (destination.Length + read > ReadingLibraryOptions.MaxUploadBytes)
                        throw new ReadingLibraryException(413, "upload_too_large", "Maximum EPUB size is 100 MiB.");
                    await destination.WriteAsync(buffer.AsMemory(0, read), ct);
                }
                size = destination.Length;
                if (size == 0 || size != file.Length)
                    throw new ReadingLibraryException(422, "invalid_epub", "Upload size does not match file data.");
            }
            // Calibre 导出的 EPUB 常含嵌入字体/复杂 CSS；先校验，失败后再清洗一次（不放宽安全边界）。
            ct.ThrowIfCancellationRequested();
            metadata = await ValidateImportedEpubAsync(temporaryKey, sizeUpdater: value => size = value, ct);
            ct.ThrowIfCancellationRequested();
            await using (var destination = storage.Open(temporaryKey))
                hash = Convert.ToHexString(await SHA256.HashDataAsync(destination, ct)).ToLowerInvariant();
            var existing = await db.ReadingBooks.AsNoTracking().FirstOrDefaultAsync(b => b.OwnerKey == ownerKey && b.Sha256 == hash, ct);
            if (existing != null) return ToDto(existing);
            committedKey = storage.Commit(temporaryKey);
            temporaryKey = null;
            var book = new ReadingBook {
                OwnerKey = ownerKey, Title = metadata.Title, Author = metadata.Author, Language = metadata.Language,
                ValidationProfile = "strict-v2",
                Sha256 = hash, StorageKey = committedKey, FileSize = size
            };
            db.ReadingBooks.Add(book);
            try { await db.SaveChangesAsync(ct); }
            catch (Exception saveError) when (saveError is DbUpdateException or OperationCanceledException)
            {
                // Reconcile duplicate imports and ambiguous commits (e.g. lost acknowledgement).
                // Do not use the aborted request token to decide whether a committed file is orphaned.
                db.Entry(book).State = EntityState.Detached;
                ReadingBook? duplicate;
                try
                {
                    using var reconciliation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    duplicate = await db.ReadingBooks.AsNoTracking().FirstOrDefaultAsync(
                        b => b.OwnerKey == ownerKey && b.Sha256 == hash, reconciliation.Token);
                }
                catch (Exception reconciliationError)
                {
                    // If database state cannot be established, keep bytes for operator reconciliation.
                    // Keeping an orphan is safer than deleting an already committed publication.
                    logger.LogError(reconciliationError, "Import state uncertain; preserve private file {StorageKey} for reconciliation", committedKey);
                    committedKey = null;
                    throw;
                }
                if (duplicate == null) throw;
                if (duplicate.StorageKey == committedKey) committedKey = null;
                return ToDto(duplicate);
            }
            committedKey = null; // database now owns this file
            return ToDto(book);
        }
        finally
        {
            // Including cancellation, malformed archive and failed DB writes.
            Cleanup(temporaryKey);
            Cleanup(committedKey);
        }
    }

    private async Task<EpubMetadata> ValidateImportedEpubAsync(string temporaryKey, Action<long> sizeUpdater, CancellationToken ct)
    {
        try
        {
            await using var stream = storage.Open(temporaryKey);
            return validator.Validate(stream);
        }
        catch (ReadingLibraryException ex) when (IsCalibreStyleRejection(ex.Code))
        {
            try { CalibreEpubSanitizer.SanitizeFile(storage.AbsolutePath(temporaryKey)); }
            catch (ReadingLibraryException) { throw; }
            catch (Exception sanitizeError) when (sanitizeError is IOException or InvalidDataException
                or UnauthorizedAccessException or System.Xml.XmlException)
            {
                logger.LogWarning(sanitizeError, "EPUB sanitize failed after validation rejection {Code}", ex.Code);
                throw;
            }
            ct.ThrowIfCancellationRequested();
            await using var cleaned = storage.Open(temporaryKey);
            long cleanedSize = cleaned.Length;
            if (cleanedSize <= 0 || cleanedSize > ReadingLibraryOptions.MaxUploadBytes)
                throw new ReadingLibraryException(413, "upload_too_large", "Maximum EPUB size is 100 MiB.");
            sizeUpdater(cleanedSize);
            return validator.Validate(cleaned);
        }
    }

    private static bool IsCalibreStyleRejection(string code) =>
        code is "unsupported_css" or "unsupported_epub" or "unsafe_content";

    private void Cleanup(string? key)
    {
        if (key == null) return;
        try { storage.Delete(key); }
        catch (IOException ex) { logger.LogError(ex, "Failed to clean private reading file {StorageKey}", key); }
        catch (UnauthorizedAccessException ex) { logger.LogError(ex, "Failed to clean private reading file {StorageKey}", key); }
    }

    public async Task<ReadingBookPage> ListAsync(string ownerKey, int page, int pageSize, string? search, CancellationToken ct)
    {
        if (page < 1 || pageSize is < 1 or > 100 || page > 100000 || (search?.Length ?? 0) > 100)
            throw new ReadingLibraryException(400, "invalid_query", "Invalid pagination or search.");
        var books = db.ReadingBooks.AsNoTracking().Where(b => b.OwnerKey == ownerKey);
        if (!string.IsNullOrWhiteSpace(search)) books = books.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
        var total = await books.CountAsync(ct);
        var rows = await (
            from book in books
            join progress in db.ReadingProgress.AsNoTracking().Where(p => p.OwnerKey == ownerKey)
                on new { book.Id, book.ContentVersion } equals new { Id = progress.BookId, progress.ContentVersion } into progresses
            from progress in progresses.DefaultIfEmpty()
            orderby (progress == null ? (DateTime?)null : progress.LastReadAt) descending, book.CreatedAt descending, book.Id descending
            select new ReadingBookDto(book.Id, book.Title, book.Author, book.Language, book.FileSize,
                book.ContentVersion, book.Sha256, book.CreatedAt,
                progress == null ? null : progress.LastReadAt, progress == null ? null : progress.Progression)
        ).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new(total, rows);
    }

    public async Task<ReadingBookDto> GetAsync(string ownerKey, long id, CancellationToken ct)
    {
        var book = await OwnedBook(ownerKey, id, ct);
        var progress = await db.ReadingProgress.AsNoTracking().FirstOrDefaultAsync(p => p.OwnerKey == ownerKey && p.BookId == id && p.ContentVersion == book.ContentVersion, ct);
        return ToDto(book) with { LastReadAt = progress?.LastReadAt, Progression = progress?.Progression };
    }

    public async Task<FileStream> OpenAsync(string ownerKey, long id, CancellationToken ct)
    {
        var book = await OwnedBook(ownerKey, id, ct);
        try { return storage.Open(book.StorageKey); }
        catch (FileNotFoundException) { throw new ReadingLibraryException(404, "content_missing", "Book content is unavailable."); }
        catch (DirectoryNotFoundException) { throw new ReadingLibraryException(404, "content_missing", "Book content is unavailable."); }
    }

    public async Task<(byte[] Bytes, string ContentType)> CoverAsync(string ownerKey, long id, CancellationToken ct)
    {
        await using var stream = await OpenAsync(ownerKey, id, ct);
        ct.ThrowIfCancellationRequested();
        return validator.ReadCover(stream)
            ?? throw new ReadingLibraryException(404, "cover_missing", "This book has no supported cover.");
    }

    private async Task<ReadingBook> OwnedBook(string ownerKey, long id, CancellationToken ct) =>
        await db.ReadingBooks.AsNoTracking().FirstOrDefaultAsync(b => b.OwnerKey == ownerKey && b.Id == id, ct)
        ?? throw new ReadingLibraryException(404, "book_not_found", "Book not found.");

    public async Task<ReadingProgressDto?> ProgressAsync(string ownerKey, long id, CancellationToken ct)
    {
        await OwnedBook(ownerKey, id, ct);
        var row = await db.ReadingProgress.AsNoTracking().FirstOrDefaultAsync(p => p.OwnerKey == ownerKey && p.BookId == id, ct);
        return row == null ? null : ProgressDto(row);
    }

    public async Task<ReadingContinueDto?> ContinueAsync(string ownerKey, CancellationToken ct)
    {
        var row = await db.ReadingProgress.AsNoTracking().Where(p => p.OwnerKey == ownerKey
            && db.ReadingBooks.Any(b => b.OwnerKey == ownerKey && b.Id == p.BookId && b.ContentVersion == p.ContentVersion))
            .OrderByDescending(p => p.LastReadAt).ThenByDescending(p => p.Id).FirstOrDefaultAsync(ct);
        if (row == null) return null;
        var book = await OwnedBook(ownerKey, row.BookId, ct);
        return new(ToDto(book) with { LastReadAt = row.LastReadAt, Progression = row.Progression }, ProgressDto(row));
    }

    public async Task<ReadingProgressDto> SaveProgressAsync(string ownerKey, long id, SaveReadingProgressRequest input, CancellationToken ct)
    {
        var book = await OwnedBook(ownerKey, id, ct);
        if (input.ContentVersion != book.ContentVersion)
            throw new ReadingLibraryException(409, "content_version_mismatch", "Book content changed; reopen before saving.");
        if (string.IsNullOrEmpty(input.Cfi) || !System.Text.RegularExpressions.Regex.IsMatch(input.Cfi, @"^epubcfi\(/\d+(?:/\d+(?:\[[^\]\r\n]*\])?)*![^\r\n]+\)$")
            || input.Cfi.Length > 4096 || !double.IsFinite(input.Progression) || input.Progression is < 0 or > 1
            || input.ExpectedRevision < 0 || input.ExpectedRevision == long.MaxValue
            || !Guid.TryParse(input.SessionId, out _) || input.ChapterHref.Length > 1024
            || input.ChapterHref.Contains(':') || input.ChapterHref.StartsWith('/') || input.ChapterHref.Contains("\\"))
            throw new ReadingLibraryException(400, "invalid_progress", "Invalid EPUB location or revision.");
        var row = await db.ReadingProgress.FirstOrDefaultAsync(p => p.OwnerKey == ownerKey && p.BookId == id, ct);
        // A lost HTTP acknowledgement may be retried without advancing revision/time twice.
        if (Acknowledges(row, input)) return ProgressDto(row!);
        if ((row?.Revision ?? 0) != input.ExpectedRevision)
            throw new ReadingLibraryException(409, "progress_conflict", "Another device updated progress.");
        if (row == null)
        {
            row = new ReadingProgress { OwnerKey = ownerKey, BookId = id };
            db.ReadingProgress.Add(row);
        }
        row.ContentVersion = input.ContentVersion; row.Cfi = input.Cfi; row.ChapterHref = input.ChapterHref;
        row.Progression = input.Progression; row.Revision = input.ExpectedRevision + 1; row.LastSessionId = input.SessionId;
        // Match MySQL DATETIME(6) precision so initial acknowledgement and retry agree exactly.
        var now = DateTime.UtcNow;
        row.LastReadAt = row.UpdatedAt = new DateTime(now.Ticks - now.Ticks % 10, DateTimeKind.Utc);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException)
        {
            db.Entry(row).State = EntityState.Detached;
            var committed = await db.ReadingProgress.AsNoTracking().FirstOrDefaultAsync(p => p.OwnerKey == ownerKey && p.BookId == id, ct);
            if (Acknowledges(committed, input)) return ProgressDto(committed!);
            if (committed != null)
                throw new ReadingLibraryException(409, "progress_conflict", "Another device updated progress.");
            throw;
        }
        return ProgressDto(row);
    }

    private static bool Acknowledges(ReadingProgress? row, SaveReadingProgressRequest input) =>
        row != null && row.Revision == input.ExpectedRevision + 1 && row.LastSessionId == input.SessionId
        && row.Cfi == input.Cfi && row.ChapterHref == input.ChapterHref && row.ContentVersion == input.ContentVersion
        && row.Progression == input.Progression;

    private static ReadingProgressDto ProgressDto(ReadingProgress row) => new(row.BookId, row.ContentVersion, row.Cfi,
        row.ChapterHref, row.Progression, row.Revision, DateTime.SpecifyKind(row.LastReadAt, DateTimeKind.Utc),
        DateTime.SpecifyKind(row.UpdatedAt, DateTimeKind.Utc));

    public async Task<IReadOnlyList<ReadingAnnotationDto>> ListAnnotationsAsync(string ownerKey, long id, CancellationToken ct)
    {
        await OwnedBook(ownerKey, id, ct);
        List<ReadingAnnotation> rows = await db.ReadingAnnotations.AsNoTracking()
            .Where(a => a.OwnerKey == ownerKey && a.BookId == id)
            .OrderByDescending(a => a.CreatedAt).ThenByDescending(a => a.Id)
            .ToListAsync(ct);
        return rows.Select(AnnotationDto).ToList();
    }

    public async Task<IReadOnlyList<ReadingAnnotationFeedItem>> ListAnnotationFeedAsync(string ownerKey, CancellationToken ct)
    {
        const int limit = 500;
        List<ReadingAnnotationFeedItem> rows = await (
            from note in db.ReadingAnnotations.AsNoTracking()
            join book in db.ReadingBooks.AsNoTracking() on new { note.OwnerKey, note.BookId } equals new { book.OwnerKey, BookId = book.Id }
            where note.OwnerKey == ownerKey
            orderby note.CreatedAt descending, note.Id descending
            select new ReadingAnnotationFeedItem(
                note.Id, note.BookId, book.Title, book.Author, note.ContentVersion, note.Kind,
                note.CfiRange, note.ChapterHref, note.Quote, note.Note,
                DateTime.SpecifyKind(note.CreatedAt, DateTimeKind.Utc),
                DateTime.SpecifyKind(note.UpdatedAt, DateTimeKind.Utc))
        ).Take(limit).ToListAsync(ct);
        return rows;
    }

    public async Task<ReadingAnnotationDto> CreateAnnotationAsync(string ownerKey, long id,
        CreateReadingAnnotationRequest input, CancellationToken ct)
    {
        ReadingBook book = await OwnedBook(ownerKey, id, ct);
        ValidateAnnotation(input, book.ContentVersion);
        ReadingAnnotation? existing = await db.ReadingAnnotations.AsNoTracking()
            .FirstOrDefaultAsync(a => a.OwnerKey == ownerKey && a.BookId == id && a.RequestId == input.RequestId, ct);
        if (existing != null) return AnnotationDto(existing);

        DateTime now = DateTime.UtcNow;
        DateTime stamped = new(now.Ticks - now.Ticks % 10, DateTimeKind.Utc);
        ReadingAnnotation row = new()
        {
            OwnerKey = ownerKey,
            BookId = id,
            ContentVersion = input.ContentVersion,
            Kind = input.Kind,
            CfiRange = input.CfiRange.Trim(),
            ChapterHref = input.ChapterHref ?? "",
            Quote = input.Quote ?? "",
            Prefix = input.Prefix ?? "",
            Suffix = input.Suffix ?? "",
            Note = input.Note ?? "",
            Color = input.Color,
            AnchorStatus = "valid",
            RequestId = input.RequestId,
            Revision = 1,
            CreatedAt = stamped,
            UpdatedAt = stamped,
        };
        db.ReadingAnnotations.Add(row);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException)
        {
            db.Entry(row).State = EntityState.Detached;
            ReadingAnnotation? committed = await db.ReadingAnnotations.AsNoTracking()
                .FirstOrDefaultAsync(a => a.OwnerKey == ownerKey && a.BookId == id && a.RequestId == input.RequestId, ct);
            if (committed != null) return AnnotationDto(committed);
            throw;
        }
        return AnnotationDto(row);
    }

    private static void ValidateAnnotation(CreateReadingAnnotationRequest input, int contentVersion)
    {
        if (input.ContentVersion != contentVersion)
            throw new ReadingLibraryException(409, "content_version_mismatch", "Book content changed; reopen before saving.");
        string cfi = (input.CfiRange ?? "").Trim();
        string chapter = input.ChapterHref ?? "";
        string quote = input.Quote ?? "";
        string note = input.Note ?? "";
        if (cfi.Length == 0 || cfi.Length > 4096 || !cfi.StartsWith("epubcfi(", StringComparison.Ordinal)
            || !cfi.EndsWith(')') || cfi.Contains('\r') || cfi.Contains('\n')
            || !Guid.TryParse(input.RequestId, out _)
            || chapter.Length > 1024 || chapter.Contains(':') || chapter.StartsWith('/') || chapter.Contains('\\')
            || quote.Length > 20000 || note.Length > 20000 || note.Trim().Length == 0
            || (input.Kind is not ("highlight" or "note"))
            || (input.Color is not ("yellow" or "green" or "blue" or "pink")))
            throw new ReadingLibraryException(400, "invalid_annotation", "Invalid note location or content.");
    }

    private static ReadingAnnotationDto AnnotationDto(ReadingAnnotation row) => new(
        row.Id, row.BookId, row.ContentVersion, row.Kind, row.CfiRange, row.ChapterHref, row.Quote, row.Note,
        row.Color, row.RequestId, row.Revision,
        DateTime.SpecifyKind(row.CreatedAt, DateTimeKind.Utc),
        DateTime.SpecifyKind(row.UpdatedAt, DateTimeKind.Utc));

    private static ReadingBookDto ToDto(ReadingBook b) => new(b.Id, b.Title, b.Author, b.Language,
        b.FileSize, b.ContentVersion, b.Sha256, b.CreatedAt, null, null);
}
