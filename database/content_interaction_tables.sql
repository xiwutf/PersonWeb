-- 内容互动 V1：通用评论 + 点赞记录
-- 执行方式：在 personweb 库中运行本脚本（可重复执行）

-- ============================================
-- 1. comments — 通用评论
-- ============================================
CREATE TABLE IF NOT EXISTS `comments` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '评论ID',
    `target_type` VARCHAR(32) NOT NULL COMMENT '内容类型：article/reading/quote/moment',
    `target_id` VARCHAR(180) NOT NULL COMMENT '内容业务 ID（如文章 slug）',
    `parent_id` BIGINT DEFAULT NULL COMMENT '父评论ID（V1 暂不启用回复）',
    `nickname` VARCHAR(30) NOT NULL COMMENT '昵称',
    `email` VARCHAR(120) NOT NULL COMMENT '邮箱（仅审核用，游客端不返回）',
    `content` TEXT NOT NULL COMMENT '评论正文',
    `status` VARCHAR(20) NOT NULL DEFAULT 'pending' COMMENT 'pending/approved/rejected',
    `ip` VARCHAR(50) DEFAULT NULL COMMENT '提交 IP',
    `visitor_key` VARCHAR(100) DEFAULT NULL COMMENT '访客标识（可选）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_comments_target` (`target_type`, `target_id`),
    INDEX `idx_comments_status` (`status`),
    INDEX `idx_comments_created` (`created_at`),
    INDEX `idx_comments_parent` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='通用内容评论';

-- ============================================
-- 2. content_likes — 点赞记录
-- ============================================
CREATE TABLE IF NOT EXISTS `content_likes` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '点赞ID',
    `target_type` VARCHAR(32) NOT NULL COMMENT '内容类型',
    `target_id` VARCHAR(180) NOT NULL COMMENT '内容业务 ID',
    `visitor_key` VARCHAR(100) NOT NULL COMMENT '访客标识 aven_visitor_id',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_like_target_visitor` (`target_type`, `target_id`, `visitor_key`),
    INDEX `idx_likes_target` (`target_type`, `target_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='内容点赞记录';
