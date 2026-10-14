using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("reading_sessions")]
public sealed class ReadingSession
{
    [Key, MaxLength(64), Column("token_hash")] public string TokenHash { get; set; } = "";
    [Required, MaxLength(36), Column("owner_key", TypeName = "char")] public string OwnerKey { get; set; } = "";
    [Required, MaxLength(64), Column("credential_version")] public string CredentialVersion { get; set; } = "";
    [Column("created_at")] public DateTime CreatedAt { get; set; }
    [Column("expires_at")] public DateTime ExpiresAt { get; set; }
    [Column("absolute_expires_at")] public DateTime AbsoluteExpiresAt { get; set; }
    [Column("revoked_at")] public DateTime? RevokedAt { get; set; }
}
