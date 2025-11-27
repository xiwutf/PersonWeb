using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<SiteConfig> SiteConfigs { get; set; }
    public DbSet<OperationLog> OperationLogs { get; set; }
    public DbSet<PvUvDaily> PvUvDailies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置多对多关系 article_tag
        modelBuilder.Entity<Article>()
            .HasMany(a => a.Tags)
            .WithMany(t => t.Articles)
            .UsingEntity<Dictionary<string, object>>(
                "article_tag",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("tag_id"),
                j => j.HasOne<Article>().WithMany().HasForeignKey("article_id")
            );
            
        // 索引配置
        modelBuilder.Entity<Article>()
            .HasIndex(a => a.Slug)
            .IsUnique();
            
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
            
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();
            
        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();
    }
}
