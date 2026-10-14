using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Data;

internal static class ReadingCenterModelConfiguration
{
    public static void Configure(ModelBuilder builder)
    {
        // MySqlConnector reads CHAR(36) as Guid; keep the domain OwnerKey as a string.
        foreach (var type in new[] { typeof(ReadingBook), typeof(ReadingProgress), typeof(ReadingAnnotation), typeof(ReadingPreference), typeof(ReadingSession) })
        {
            builder.Entity(type).Property<string>("OwnerKey")
                .HasConversion<Guid>().HasColumnType("char(36)");
        }
        builder.Entity<ReadingSession>().HasIndex(s => s.ExpiresAt);
        var books = builder.Entity<ReadingBook>();
        books.HasAlternateKey(b => new { b.OwnerKey, b.Id });
        books.HasIndex(b => new { b.OwnerKey, b.Sha256 }).IsUnique();
        var progress = builder.Entity<ReadingProgress>();
        progress.HasIndex(p => new { p.OwnerKey, p.BookId }).IsUnique();
        progress.HasIndex(p => new { p.OwnerKey, p.LastReadAt });
        progress.HasOne<ReadingBook>().WithMany().HasForeignKey(p => new { p.OwnerKey, p.BookId })
            .HasPrincipalKey(b => new { b.OwnerKey, b.Id }).OnDelete(DeleteBehavior.Cascade);

        var annotations = builder.Entity<ReadingAnnotation>();
        annotations.HasIndex(a => new { a.OwnerKey, a.BookId, a.RequestId }).IsUnique();
        annotations.HasOne<ReadingBook>().WithMany().HasForeignKey(a => new { a.OwnerKey, a.BookId })
            .HasPrincipalKey(b => new { b.OwnerKey, b.Id }).OnDelete(DeleteBehavior.Cascade);
    }
}
