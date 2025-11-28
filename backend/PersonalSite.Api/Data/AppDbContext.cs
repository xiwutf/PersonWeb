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
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<MonthlyKpi> MonthlyKpis { get; set; }
    public DbSet<HomeStyle> HomeStyles { get; set; }
    public DbSet<AdminModuleStyle> AdminModuleStyles { get; set; }
    public DbSet<AdminGlobalStyle> AdminGlobalStyles { get; set; }
    public DbSet<StyleCategory> StyleCategories { get; set; }
    public DbSet<StyleDefinition> StyleDefinitions { get; set; }
    public DbSet<StyleUsage> StyleUsages { get; set; }
    public DbSet<ThemeStyle> ThemeStyles { get; set; }
    public DbSet<BackgroundEffect> BackgroundEffects { get; set; }
    public DbSet<ThemeSetting> ThemeSettings { get; set; }
    public DbSet<UserThemePreference> UserThemePreferences { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<ModuleConfig> ModuleConfigs { get; set; }
    public DbSet<VisitorMessage> VisitorMessages { get; set; }
    public DbSet<VisitorFootprint> VisitorFootprints { get; set; }
    public DbSet<VisitorBubble> VisitorBubbles { get; set; }
    public DbSet<VisitorLevel> VisitorLevels { get; set; }
    public DbSet<VisitorBehavior> VisitorBehaviors { get; set; }
    public DbSet<VisitorChallenge> VisitorChallenges { get; set; }
    public DbSet<VisitorChallengeParticipant> VisitorChallengeParticipants { get; set; }
    public DbSet<VisitorAchievement> VisitorAchievements { get; set; }
    public DbSet<VisitorTriggerEvent> VisitorTriggerEvents { get; set; }
    public DbSet<ToolCategory> ToolCategories { get; set; }
    public DbSet<Tool> Tools { get; set; }
    public DbSet<ToolPurchase> ToolPurchases { get; set; }
    public DbSet<ToolUsage> ToolUsages { get; set; }
    public DbSet<ToolApiKey> ToolApiKeys { get; set; }
    public DbSet<ToolCollection> ToolCollections { get; set; }
    public DbSet<ToolCollectionItem> ToolCollectionItems { get; set; }
    public DbSet<ToolReview> ToolReviews { get; set; }
    public DbSet<ToolAnalytics> ToolAnalytics { get; set; }

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
            
        // 配置 Module 和 ModuleConfig 的关系
        // ModuleKey 作为替代键（Alternate Key）
        modelBuilder.Entity<Module>()
            .HasAlternateKey(m => m.ModuleKey);
            
        // 配置 ModuleConfig 使用 ModuleKey 作为外键
        modelBuilder.Entity<ModuleConfig>()
            .HasOne(mc => mc.Module)
            .WithMany(m => m.ModuleConfigs)
            .HasForeignKey(mc => mc.ModuleKey)
            .HasPrincipalKey(m => m.ModuleKey)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
