using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Api.Models.Dto;

// Storage paths, owner identifiers and original filenames are never returned.
public record ReadingBookDto(long Id, string Title, string Author, string Language,
    long FileSize, int ContentVersion, string ContentHash, DateTime CreatedAt,
    DateTime? LastReadAt, double? Progression);
public record ReadingBookPage(int Total, IReadOnlyList<ReadingBookDto> Items);

public record ReadingProgressDto(long BookId, int ContentVersion, string Cfi, string ChapterHref,
    double Progression, long Revision, DateTime LastReadAt, DateTime UpdatedAt);
public record ReadingContinueDto(ReadingBookDto Book, ReadingProgressDto Progress);

// Annotations remain deferred. SessionId identifies one progress write, retained on retry.
public class SaveReadingProgressRequest
{
    [Range(1, int.MaxValue)] public int ContentVersion { get; set; }
    [Required, StringLength(4096)] public string Cfi { get; set; } = "";
    [StringLength(1024)] public string ChapterHref { get; set; } = "";
    [Range(0, 1)] public double Progression { get; set; }
    [Range(0, long.MaxValue)] public long ExpectedRevision { get; set; }
    [Required, StringLength(64)] public string SessionId { get; set; } = "";
}

public class CreateReadingAnnotationRequest
{
    [Range(1, int.MaxValue)] public int ContentVersion { get; set; }
    [Required, StringLength(4096)] public string CfiRange { get; set; } = "";
    [Required, StringLength(64)] public string RequestId { get; set; } = "";
    [StringLength(1024)] public string ChapterHref { get; set; } = "";
    [StringLength(20000)] public string Quote { get; set; } = "";
    [StringLength(512)] public string Prefix { get; set; } = "";
    [StringLength(512)] public string Suffix { get; set; } = "";
    [StringLength(20000)] public string Note { get; set; } = "";
    [RegularExpression("highlight|note")] public string Kind { get; set; } = "highlight";
    [RegularExpression("yellow|green|blue|pink")] public string Color { get; set; } = "yellow";
}

// V0.1 笔记：不含 AI 解释；AI 仅会话内展示。
public record ReadingAnnotationDto(long Id, long BookId, int ContentVersion, string Kind, string CfiRange,
    string ChapterHref, string Quote, string Note, string Color, string RequestId, long Revision,
    DateTime CreatedAt, DateTime UpdatedAt);

// Life「读书」双视图：扁平时间线 + 前端按书分组。
public record ReadingAnnotationFeedItem(long Id, long BookId, string BookTitle, string BookAuthor,
    int ContentVersion, string Kind, string CfiRange, string ChapterHref, string Quote, string Note,
    DateTime CreatedAt, DateTime UpdatedAt);

public class ReadingChatTurn
{
    [Required, RegularExpression("user|assistant")] public string Role { get; set; } = "user";
    [Required, StringLength(8000)] public string Content { get; set; } = "";
}

// 阅读伴读解释：服务端代理，密钥不进浏览器；失败不影响阅读/笔记/进度。
public class ReadingExplainRequest
{
    [Required, StringLength(4000, MinimumLength = 1)] public string Quote { get; set; } = "";
    [StringLength(2000)] public string Question { get; set; } = "";
    [MaxLength(10)] public List<ReadingChatTurn>? History { get; set; }
}

public record ReadingExplainDto(string Reply);
