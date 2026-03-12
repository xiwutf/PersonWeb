-- 模块版本管理数据库表结构
-- 用于管理模块的不同版本和历史记录

-- ============================================
-- 1. module_versions 表 - 模块版本
-- ============================================
CREATE TABLE IF NOT EXISTS `module_version` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '版本ID',
    `module_key` VARCHAR(50) NOT NULL COMMENT '模块标识',
    `version` VARCHAR(20) NOT NULL COMMENT '版本号（遵循语义化版本）',
    `package_url` VARCHAR(255) NOT NULL COMMENT '模块包下载链接',
    `package_size` INT NOT NULL COMMENT '包大小（字节）',
    `checksum` VARCHAR(64) NOT NULL COMMENT '文件校验和（SHA-256）',
    `changelog` TEXT COMMENT '版本变更日志',
    `is_latest` TINYINT DEFAULT 0 COMMENT '是否为最新版本：0-否 1-是',
    `is_stable` TINYINT DEFAULT 1 COMMENT '是否稳定版本：0-否 1-是',
    `published_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '发布时间',
    `created_by` VARCHAR(100) DEFAULT NULL COMMENT '发布者',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_module_version` (`module_key`, `version`),
    INDEX `idx_module_key` (`module_key`),
    INDEX `idx_version` (`version`),
    INDEX `idx_is_latest` (`is_latest`),
    INDEX `idx_published_at` (`published_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='模块版本表';

-- ============================================
-- 2. module_downloads 表 - 下载统计
-- ============================================
CREATE TABLE IF NOT EXISTS `module_download` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '下载记录ID',
    `module_key` VARCHAR(50) NOT NULL COMMENT '模块标识',
    `version` VARCHAR(20) NOT NULL COMMENT '下载的版本号',
    `user_id` BIGINT DEFAULT NULL COMMENT '用户ID（如果已登录）',
    `ip_address` VARCHAR(45) DEFAULT NULL COMMENT 'IP地址',
    `user_agent` TEXT DEFAULT NULL COMMENT '用户代理',
    `downloaded_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '下载时间',
    PRIMARY KEY (`id`),
    INDEX `idx_module_version` (`module_key`, `version`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_downloaded_at` (`downloaded_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='模块下载记录表';

-- ============================================
-- 3. module_reviews 表 - 模块评价
-- ============================================
CREATE TABLE IF NOT EXISTS `module_review` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '评价ID',
    `module_key` VARCHAR(50) NOT NULL COMMENT '模块标识',
    `version` VARCHAR(20) NOT NULL COMMENT '评价的版本号',
    `user_id` BIGINT NOT NULL COMMENT '用户ID',
    `rating` TINYINT NOT NULL COMMENT '评分（1-5）',
    `title` VARCHAR(100) DEFAULT NULL COMMENT '评价标题',
    `content` TEXT DEFAULT NULL COMMENT '评价内容',
    `is_verified` TINYINT DEFAULT 0 COMMENT '是否已验证（实际购买）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_module_user_version` (`module_key`, `user_id`, `version`),
    INDEX `idx_module_key` (`module_key`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_rating` (`rating`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='模块评价表';

-- ============================================
-- 4. module_compatibility 表 - 兼容性信息
-- ============================================
CREATE TABLE IF NOT EXISTS `module_compatibility` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '兼容性记录ID',
    `module_key` VARCHAR(50) NOT NULL COMMENT '模块标识',
    `version` VARCHAR(20) NOT NULL COMMENT '模块版本',
    `nuxt_version_min` VARCHAR(20) DEFAULT NULL COMMENT '最小Nuxt版本',
    `nuxt_version_max` VARCHAR(20) DEFAULT NULL COMMENT '最大Nuxt版本',
    `node_version_min` VARCHAR(20) DEFAULT NULL COMMENT '最小Node.js版本',
    `browser_compatibility` JSON DEFAULT NULL COMMENT '浏览器兼容性',
    `dependencies` JSON DEFAULT NULL COMMENT '依赖说明',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_module_version` (`module_key`, `version`),
    INDEX `idx_module_key` (`module_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='模块兼容性信息表';

-- ============================================
-- 初始化数据 - 添加现有模块的最新版本
-- ============================================
INSERT IGNORE INTO `module_version` (
    `module_key`, `version`, `package_url`, `package_size`,
    `checksum`, `is_latest`, `is_stable`, `changelog`, `created_by`
) VALUES
('ai-assistant', '1.0.0', '/uploads/modules/ai-assistant/v1.0.0.zip', 1024000,
 'sha256:abc123...', 1, 1,
 '初始版本发布\n- 添加AI对话功能\n- 支持GPT-3.5/4模型',
 'admin'),
('blog', '1.0.0', '/uploads/modules/blog/v1.0.0.zip', 856000,
 'sha256:def456...', 1, 1,
 '博客系统初始版本\n- 支持Markdown编辑\n- 集成评论系统',
 'admin');