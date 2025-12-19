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
    public DbSet<DcaPlan> DcaPlans { get; set; }
    public DbSet<DcaExecution> DcaExecutions { get; set; }
    public DbSet<PriceAlert> PriceAlerts { get; set; }
    public DbSet<Asset> Assets { get; set; }
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
    
    // 更新日志和未来规划
    public DbSet<Changelog> Changelogs { get; set; }
    public DbSet<Roadmap> Roadmaps { get; set; }
    
    // 商业化功能表
    public DbSet<ThemeStore> ThemeStores { get; set; }
    public DbSet<ThemeFile> ThemeFiles { get; set; }
    public DbSet<ThemePurchase> ThemePurchases { get; set; }
    public DbSet<ApiUser> ApiUsers { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<ApiCall> ApiCalls { get; set; }
    public DbSet<ApiBilling> ApiBillings { get; set; }
    public DbSet<MembershipType> MembershipTypes { get; set; }
    
    // 仪表盘指标表
    public DbSet<DashboardMetric> DashboardMetrics { get; set; }
    public DbSet<UserMembership> UserMemberships { get; set; }
    public DbSet<PaidContent> PaidContents { get; set; }
    public DbSet<ContentPurchase> ContentPurchases { get; set; }
    public DbSet<UserPage> UserPages { get; set; }
    public DbSet<PageComponent> PageComponents { get; set; }
    public DbSet<ComponentLibrary> ComponentLibraries { get; set; }
    public DbSet<PageTemplate> PageTemplates { get; set; }
    
    // 文档知识管家 Agent 相关表
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentChunk> DocumentChunks { get; set; }
    public DbSet<DocumentQuery> DocumentQueries { get; set; }
    public DbSet<SideProject> SideProjects { get; set; }
    public DbSet<SideProjectRequirement> SideProjectRequirements { get; set; }
    public DbSet<SideProjectTask> SideProjectTasks { get; set; }
    public DbSet<SideProjectMilestone> SideProjectMilestones { get; set; }
    public DbSet<SideProjectLog> SideProjectLogs { get; set; }
    public DbSet<SideProjectAttachment> SideProjectAttachments { get; set; }
    
    // 订单和咨询相关表
    public DbSet<Order> Orders { get; set; }
    public DbSet<PreSaleConsultation> PreSaleConsultations { get; set; }
    
    // AI 智能体相关表
    public DbSet<AiAgentLog> AiAgentLogs { get; set; }
    public DbSet<SupportConfig> SupportConfigs { get; set; }
    public DbSet<AssistantSession> AssistantSessions { get; set; }
    public DbSet<AssistantMessage> AssistantMessages { get; set; }
    
    // 智能取名助手相关表
    public DbSet<NameFavorite> NameFavorites { get; set; }
    
    // 关系跟进相关表
    public DbSet<RelationPerson> RelationPersons { get; set; }
    public DbSet<RelationInteraction> RelationInteractions { get; set; }
    public DbSet<RelationTask> RelationTasks { get; set; }

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
            
        // 配置 Project 实体的列名映射（将驼峰命名映射到下划线命名）
        // 数据库表名是 projects（小写），列名使用下划线命名
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("projects"); // 表名是小写的 projects
            
            // 配置所有字段的列名映射（从驼峰命名映射到下划线命名）
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Title).HasColumnName("Title");
            entity.Property(p => p.Description).HasColumnName("Description");
            entity.Property(p => p.CoverUrl).HasColumnName("CoverUrl");
            entity.Property(p => p.DemoUrl).HasColumnName("DemoUrl");
            entity.Property(p => p.GithubUrl).HasColumnName("GithubUrl");
            entity.Property(p => p.Status).HasColumnName("Status");
            entity.Property(p => p.TechStack).HasColumnName("TechStack");
            entity.Property(p => p.Content).HasColumnName("Content");
            entity.Property(p => p.ViewCount).HasColumnName("view_count");
            
            // 配置 AI 相关字段的列名（下划线命名）
            entity.Property(p => p.AiTitle).HasColumnName("ai_title");
            entity.Property(p => p.AiHighlights).HasColumnName("ai_highlights");
            entity.Property(p => p.AiDescription).HasColumnName("ai_description");
            entity.Property(p => p.AiScenarios).HasColumnName("ai_scenarios");
            entity.Property(p => p.AiTargetUsers).HasColumnName("ai_target_users");
            entity.Property(p => p.AiShortCardText).HasColumnName("ai_short_card_text");
            
            entity.Property(p => p.CreatedAt).HasColumnName("CreatedAt");
            entity.Property(p => p.UpdatedAt).HasColumnName("UpdatedAt");
        });

        // 配置 SideProject 相关实体的关系
        modelBuilder.Entity<SideProjectRequirement>()
            .HasOne(r => r.Project)
            .WithMany(p => p.Requirements)
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SideProjectTask>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SideProjectMilestone>()
            .HasOne(m => m.Project)
            .WithMany(p => p.Milestones)
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SideProjectLog>()
            .HasOne(l => l.Project)
            .WithMany(p => p.Logs)
            .HasForeignKey(l => l.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SideProjectAttachment>()
            .HasOne(a => a.Project)
            .WithMany(p => p.Attachments)
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // 配置索引
        modelBuilder.Entity<SideProjectTask>()
            .HasIndex(t => new { t.ProjectId, t.SortOrder });

        modelBuilder.Entity<SideProjectLog>()
            .HasIndex(l => new { l.ProjectId, l.CreatedAt });

        // 配置关系跟进相关实体的关系
        modelBuilder.Entity<RelationInteraction>()
            .HasOne(i => i.Person)
            .WithMany(p => p.Interactions)
            .HasForeignKey(i => i.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RelationTask>()
            .HasOne(t => t.Person)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // 配置索引
        modelBuilder.Entity<RelationPerson>()
            .HasIndex(p => p.Stage);
        
        modelBuilder.Entity<RelationPerson>()
            .HasIndex(p => p.LastContactAt);
        
        modelBuilder.Entity<RelationInteraction>()
            .HasIndex(i => new { i.PersonId, i.OccurredAt });
        
        modelBuilder.Entity<RelationTask>()
            .HasIndex(t => new { t.PersonId, t.Status });
    }
}
