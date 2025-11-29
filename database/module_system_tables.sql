-- 模块化系统数据库表结构
-- 用于管理各个功能模块的启用/禁用和配置
-- 
-- 设计原则：
-- 1. 不使用外键约束，通过逻辑关联维护表间关系
-- 2. 关联关系由应用层维护，便于后期维护和扩展
-- 3. 为关联字段创建索引以提升查询性能
-- 4. 详细说明请参考 database/DESIGN_PRINCIPLES.md

-- ============================================
-- 1. Modules 表 - 模块定义
-- ============================================
CREATE TABLE IF NOT EXISTS `module` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '模块ID',
    `module_key` VARCHAR(50) NOT NULL COMMENT '模块唯一标识（如：blog、projects、ai-lab等）',
    `module_name` VARCHAR(100) NOT NULL COMMENT '模块名称（如：技术博客、项目展示等）',
    `module_version` VARCHAR(20) DEFAULT '1.0.0' COMMENT '模块版本',
    `description` TEXT DEFAULT NULL COMMENT '模块描述',
    `author` VARCHAR(100) DEFAULT NULL COMMENT '模块作者',
    `icon` VARCHAR(100) DEFAULT NULL COMMENT '模块图标（FontAwesome类名）',
    `category` VARCHAR(50) DEFAULT NULL COMMENT '模块分类（如：content、tool、experiment等）',
    `dependencies` TEXT DEFAULT NULL COMMENT '依赖的其他模块（JSON数组）',
    `routes` TEXT DEFAULT NULL COMMENT '模块路由配置（JSON格式）',
    `components` TEXT DEFAULT NULL COMMENT '模块组件列表（JSON数组）',
    `permissions` TEXT DEFAULT NULL COMMENT '模块权限配置（JSON格式）',
    `config_schema` TEXT DEFAULT NULL COMMENT '模块配置Schema（JSON格式）',
    `is_enabled` TINYINT DEFAULT 1 COMMENT '是否启用：0-禁用 1-启用',
    `is_core` TINYINT DEFAULT 0 COMMENT '是否核心模块：0-否 1-是（核心模块不能禁用）',
    `sort` INT DEFAULT 0 COMMENT '排序',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_module_key` (`module_key`),
    INDEX `idx_is_enabled` (`is_enabled`),
    INDEX `idx_category` (`category`),
    INDEX `idx_sort` (`sort`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='模块定义表';

-- ============================================
-- 2. ModuleConfigs 表 - 模块配置
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- module_key 通过逻辑关联到 module.module_key，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `module_config` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '配置ID',
    `module_key` VARCHAR(50) NOT NULL COMMENT '模块标识（逻辑关联到 module.module_key）',
    `config_key` VARCHAR(100) NOT NULL COMMENT '配置键',
    `config_value` TEXT DEFAULT NULL COMMENT '配置值（JSON格式）',
    `description` VARCHAR(255) DEFAULT NULL COMMENT '配置描述',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_module_config` (`module_key`, `config_key`),
    INDEX `idx_module_key` (`module_key`)
    -- 注意：不使用外键约束，module_key 通过逻辑关联到 module.module_key
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='模块配置表';

-- ============================================
-- 初始化核心模块
-- ============================================
INSERT INTO `module` (`module_key`, `module_name`, `module_version`, `description`, `icon`, `category`, `is_enabled`, `is_core`, `sort`, `routes`, `components`) VALUES 
('core', '核心模块', '1.0.0', '系统核心功能，包含基础布局、导航等', 'fas fa-cog', 'core', 1, 1, 0, 
 '["/", "/about"]', 
 '["Header", "Footer", "ThemeSwitcher"]'),
 
('blog', '技术博客', '1.0.0', '技术文章和博客功能', 'fas fa-blog', 'content', 1, 0, 1,
 '["/blog", "/blog/[id]"]',
 '["MarkdownEditor", "GiscusComments"]'),
 
('projects', '项目展示', '1.0.0', '项目作品展示功能', 'fas fa-project-diagram', 'content', 1, 0, 2,
 '["/projects", "/projects/[id]"]',
 '["Project3DSpace"]'),
 
('tools', '插件工具', '1.0.0', 'Revit插件和工具展示', 'fas fa-tools', 'tool', 1, 0, 3,
 '["/tools", "/tools/[slug]"]',
 '["ToolsFilter"]'),
 
('ai-lab', 'AI实验室', '1.0.0', 'AI智能体和实验功能', 'fas fa-brain', 'experiment', 1, 0, 4,
 '["/ai", "/ai/[type]/[slug]"]',
 '["AIAssistant", "AICharacter"]'),
 
('lab-3d', '3D实验室', '1.0.0', '3D场景和交互实验', 'fas fa-cube', 'experiment', 1, 0, 5,
 '["/lab"]',
 '["Scene3D", "Immersive3DScene"]'),
 
('time-capsule', '时间胶囊', '1.0.0', '时间胶囊留言墙功能', 'fas fa-clock', 'interaction', 1, 0, 6,
 '[]',
 '["TimeCapsuleWall", "TimeCapsuleDisplay"]'),
 
('knowledge', '知识库', '1.0.0', '知识库管理功能', 'fas fa-book', 'content', 1, 0, 7,
 '["/knowledge"]',
 '[]'),
 
('timeline', '时间线', '1.0.0', '成长轨迹时间线', 'fas fa-history', 'content', 1, 0, 8,
 '["/timeline"]',
 '["Timeline"]'),
 
('skills', '技能树', '1.0.0', '技能树展示功能', 'fas fa-sitemap', 'content', 1, 0, 9,
 '["/skills"]',
 '[]'),
 
('life', '生活随笔', '1.0.0', '生活随笔和日常记录', 'fas fa-coffee', 'content', 1, 0, 10,
 '["/life", "/life/[...slug]"]',
 '[]'),
 
('english', '英语学习', '1.0.0', '英语学习功能', 'fas fa-language', 'tool', 1, 0, 11,
 '["/english"]',
 '["DailyWord", "QuickQuiz", "StreakTracker"]'),
 
('game', '小游戏', '1.0.0', '互动小游戏', 'fas fa-gamepad', 'experiment', 1, 0, 12,
 '["/game"]',
 '["DodgeGame"]'),
 
('showcase', '展示墙', '1.0.0', '个性化展示墙（瀑布流）', 'fas fa-th', 'content', 1, 0, 13,
 '["/showcase"]',
 '[]'),
 
('dashboard', '仪表盘', '1.0.0', '数据仪表盘', 'fas fa-chart-line', 'tool', 1, 0, 14,
 '["/dashboard"]',
 '[]'),
 
('search', '全站搜索', '1.0.0', '全站内容搜索功能', 'fas fa-search', 'tool', 1, 0, 15,
 '["/search"]',
 '[]'),
 
('about', '关于我', '1.0.0', '个人介绍和关于页面', 'fas fa-user', 'content', 1, 0, 16,
 '["/about"]',
 '[]')
ON DUPLICATE KEY UPDATE `module_name`=`module_name`;

