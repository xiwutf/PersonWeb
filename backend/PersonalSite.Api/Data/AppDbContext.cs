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
    public DbSet<VisitLog> VisitLogs { get; set; }
    public DbSet<PageStat> PageStats { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Metric> Metrics { get; set; }
    public DbSet<TimeCapsule> TimeCapsules { get; set; }
    public DbSet<KnowledgeBase> KnowledgeBases { get; set; }
    public DbSet<TimelineEvent> TimelineEvents { get; set; }
    public DbSet<VisitorAnalytics> VisitorAnalytics { get; set; }
    public DbSet<UserBehavior> UserBehaviors { get; set; }
    public DbSet<Investment> Investments { get; set; }
    public DbSet<InvestmentTransaction> InvestmentTransactions { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<FriendLink> FriendLinks { get; set; }
    public DbSet<SkillCategory> SkillCategories { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<SkillRating> SkillRatings { get; set; }
    public DbSet<LearningLog> LearningLogs { get; set; }

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
