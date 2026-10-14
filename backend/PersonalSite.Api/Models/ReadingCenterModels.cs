using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("reading_books")]
public class ReadingBook
{
    [Key, Column("id")] public long Id { get; set; }
    [Required, MaxLength(36), Column("owner_key", TypeName = "char")] public string OwnerKey { get; set; } = "";
    [Required, MaxLength(512), Column("title")] public string Title { get; set; } = "";
    [MaxLength(512), Column("author")] public string Author { get; set; } = "";
    [MaxLength(64), Column("language")] public string Language { get; set; } = "";
    [Required, MaxLength(64), Column("sha256")] public string Sha256 { get; set; } = "";
    [Required, MaxLength(64), Column("storage_key")] public string StorageKey { get; set; } = "";
    [Column("file_size")] public long FileSize { get; set; }
    [Column("content_version")] public int ContentVersion { get; set; } = 1;
    [MaxLength(32), Column("validation_profile")] public string ValidationProfile { get; set; } = "strict-v1";
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// Contracts for the next reading stage; no public publishing relationship.
[Table("reading_progress")]
public class ReadingProgress
{
    [Key, Column("id")] public long Id { get; set; }
    [Required, MaxLength(36), Column("owner_key", TypeName = "char")] public string OwnerKey { get; set; } = "";
    [Column("book_id")] public long BookId { get; set; }
    [Column("content_version")] public int ContentVersion { get; set; }
    [Required, MaxLength(4096), Column("cfi")] public string Cfi { get; set; } = "";
    [MaxLength(1024), Column("chapter_href")] public string ChapterHref { get; set; } = "";
    [Column("progression")] public double Progression { get; set; }
    [Column("revision"), ConcurrencyCheck] public long Revision { get; set; }
    [MaxLength(64), Column("last_session_id")] public string LastSessionId { get; set; } = "";
    [Column("last_read_at")] public DateTime LastReadAt { get; set; }
    [Column("updated_at")] public DateTime UpdatedAt { get; set; }
}

[Table("reading_annotations")]
public class ReadingAnnotation
{
    [Key, Column("id")] public long Id { get; set; }
    [Required, MaxLength(36), Column("owner_key", TypeName = "char")] public string OwnerKey { get; set; } = "";
    [Column("book_id")] public long BookId { get; set; }
    [Column("content_version")] public int ContentVersion { get; set; }
    [MaxLength(16), Column("kind")] public string Kind { get; set; } = "highlight";
    [Required, MaxLength(4096), Column("cfi_range")] public string CfiRange { get; set; } = "";
    [MaxLength(1024), Column("chapter_href")] public string ChapterHref { get; set; } = "";
    [Column("quote", TypeName = "longtext")] public string Quote { get; set; } = "";
    [MaxLength(512), Column("prefix")] public string Prefix { get; set; } = "";
    [MaxLength(512), Column("suffix")] public string Suffix { get; set; } = "";
    [Column("note", TypeName = "longtext")] public string Note { get; set; } = "";
    [MaxLength(16), Column("color")] public string Color { get; set; } = "yellow";
    [MaxLength(16), Column("anchor_status")] public string AnchorStatus { get; set; } = "valid";
    [Required, MaxLength(64), Column("request_id")] public string RequestId { get; set; } = "";
    [Column("revision"), ConcurrencyCheck] public long Revision { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[Table("reading_preferences")]
public class ReadingPreference
{
    [Key, MaxLength(36), Column("owner_key", TypeName = "char")] public string OwnerKey { get; set; } = "";
    [MaxLength(64), Column("font_family")] public string FontFamily { get; set; } = "serif";
    [Column("font_size")] public int FontSize { get; set; } = 18;
    [Column("line_height")] public double LineHeight { get; set; } = 1.6;
    [MaxLength(16), Column("theme")] public string Theme { get; set; } = "paper";
    [Column("revision"), ConcurrencyCheck] public long Revision { get; set; }
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
