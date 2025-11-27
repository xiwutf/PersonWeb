-- 完整的数据库表结构 SQL 脚本
-- 数据库: personal_site
-- 用于手动维护数据库表结构

-- ============================================
-- 1. Projects 表
-- 注意：Project 实体类没有 [Table] 属性，EF Core 默认使用复数形式
-- 但根据 MySQL 命名约定，建议使用小写表名 projects
-- ============================================
CREATE TABLE IF NOT EXISTS `projects` (
    `Id` CHAR(36) NOT NULL COMMENT '项目ID (GUID)',
    `Title` VARCHAR(100) NOT NULL COMMENT '项目标题',
    `Description` VARCHAR(500) DEFAULT '' COMMENT '项目描述',
    `CoverUrl` VARCHAR(200) DEFAULT NULL COMMENT '封面图片URL',
    `DemoUrl` VARCHAR(200) DEFAULT NULL COMMENT '演示地址',
    `GithubUrl` VARCHAR(200) DEFAULT NULL COMMENT 'GitHub仓库地址',
    `Status` VARCHAR(50) DEFAULT 'Active' COMMENT '项目状态: Active, Completed, Archived',
    `TechStack` TEXT DEFAULT NULL COMMENT '技术栈 (JSON数组或逗号分隔)',
    `Content` LONGTEXT DEFAULT NULL COMMENT '项目内容 (Markdown格式)',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_created_at` (`CreatedAt`),
    INDEX `idx_status` (`Status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='项目表';

-- ============================================
-- 2. Categories 表（如果不存在）
-- 注意：实体类使用 [Table("category")]，所以表名是小写 category
-- ============================================
CREATE TABLE IF NOT EXISTS `category` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '分类ID',
    `name` VARCHAR(50) NOT NULL COMMENT '分类名称',
    `slug` VARCHAR(100) DEFAULT NULL COMMENT '分类别名',
    `sort` INT DEFAULT 0 COMMENT '排序',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_name` (`name`),
    INDEX `idx_sort` (`sort`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='分类表';

-- ============================================
-- 3. Articles 表（如果不存在）
-- 注意：实体类使用 [Table("article")]，所以表名是小写 article
-- ============================================
CREATE TABLE IF NOT EXISTS `article` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '文章ID',
    `title` VARCHAR(255) NOT NULL COMMENT '文章标题',
    `slug` VARCHAR(255) DEFAULT NULL COMMENT '文章别名',
    `summary` VARCHAR(500) DEFAULT NULL COMMENT '文章摘要',
    `content_md` LONGTEXT DEFAULT NULL COMMENT 'Markdown内容',
    `content_html` LONGTEXT DEFAULT NULL COMMENT 'HTML内容',
    `cover_url` VARCHAR(500) DEFAULT NULL COMMENT '封面图片URL',
    `category_id` BIGINT DEFAULT NULL COMMENT '分类ID',
    `status` TINYINT DEFAULT 1 COMMENT '状态: 0-草稿 1-已发布 2-下线',
    `author_id` BIGINT DEFAULT NULL COMMENT '作者ID',
    `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
    `view_count` INT DEFAULT 0 COMMENT '浏览次数',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_slug` (`slug`),
    INDEX `idx_category_id` (`category_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`),
    FOREIGN KEY (`category_id`) REFERENCES `category`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='文章表';

-- ============================================
-- 4. SiteConfigs 表（如果不存在）
-- 注意：实体类使用 [Table("site_config")]，所以表名是小写下划线 site_config
-- ============================================
CREATE TABLE IF NOT EXISTS `site_config` (
    `config_key` VARCHAR(100) NOT NULL COMMENT '配置键',
    `config_value` TEXT DEFAULT NULL COMMENT '配置值',
    `description` VARCHAR(255) DEFAULT NULL COMMENT '配置描述',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`config_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='站点配置表';

-- ============================================
-- 5. VisitLogs 表（如果不存在，用于 Stats API）
-- 注意：实体类使用 [Table("visit_logs")]，所以表名是小写下划线 visit_logs
-- ============================================
CREATE TABLE IF NOT EXISTS `visit_logs` (
    `id` VARCHAR(36) NOT NULL COMMENT '访问日志ID (GUID)',
    `visitor_id` VARCHAR(36) NOT NULL COMMENT '访客ID',
    `ip` VARCHAR(45) DEFAULT NULL COMMENT 'IP地址',
    `user_agent` TEXT DEFAULT NULL COMMENT '用户代理',
    `path` VARCHAR(255) DEFAULT NULL COMMENT '访问路径',
    `timestamp` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '访问时间',
    PRIMARY KEY (`id`),
    INDEX `idx_visitor_id` (`visitor_id`),
    INDEX `idx_timestamp` (`timestamp`),
    INDEX `idx_path` (`path`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='访问日志表';

-- ============================================
-- 6. Users 表（如果不存在，用于管理员登录）
-- 注意：实体类使用 [Table("user")]，所以表名是小写 user
-- ============================================
CREATE TABLE IF NOT EXISTS `user` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '用户ID',
    `username` VARCHAR(50) NOT NULL COMMENT '用户名',
    `password_hash` VARCHAR(255) NOT NULL COMMENT '密码哈希',
    `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
    `role` VARCHAR(20) NOT NULL DEFAULT 'admin' COMMENT '角色',
    `status` TINYINT DEFAULT 1 COMMENT '状态: 0-禁用 1-启用',
    `last_login_time` DATETIME DEFAULT NULL COMMENT '最后登录时间',
    `last_login_ip` VARCHAR(50) DEFAULT NULL COMMENT '最后登录IP',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户表';

-- ============================================
-- 初始化数据（可选）
-- ============================================

-- 插入默认管理员（密码需要先哈希，这里只是示例）
-- INSERT INTO `Users` (`Username`, `PasswordHash`) VALUES 
-- ('admin', 'your-hashed-password-here')
-- ON DUPLICATE KEY UPDATE `Username`=`Username`;

-- 插入默认分类（使用小写表名）
INSERT INTO `category` (`name`, `slug`, `sort`) VALUES 
('技术文章', 'tech', 1),
('生活随笔', 'life', 2),
('项目总结', 'project', 3)
ON DUPLICATE KEY UPDATE `name`=`name`;

-- 插入默认配置（使用小写下划线表名）
INSERT INTO `site_config` (`config_key`, `config_value`) VALUES 
('site_title', '溪午听风 - 个人开发者网站'),
('site_subtitle', '开发让生活更高效，代码就是我的魔方'),
('icp_record', ''),
('home_page_size', '10')
ON DUPLICATE KEY UPDATE `config_key`=`config_key`;

