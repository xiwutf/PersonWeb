-- ============================================
-- 智能取名助手相关表
-- 创建时间: 2024-12-XX
-- ============================================

-- ============================================
-- name_favorite 表：名字收藏表
-- ============================================
CREATE TABLE IF NOT EXISTS `name_favorite` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '收藏ID',
    `name` VARCHAR(100) NOT NULL COMMENT '名字',
    `type` VARCHAR(20) NOT NULL COMMENT '取名类型: game | nickname | english',
    `style` VARCHAR(200) DEFAULT NULL COMMENT '风格（多风格用逗号存储）',
    `language` VARCHAR(20) DEFAULT NULL COMMENT '语言',
    `total_score` INT NOT NULL DEFAULT 0 COMMENT '总分 (0-100)',
    `reason` VARCHAR(500) DEFAULT NULL COMMENT '评分理由',
    `meta_json` TEXT DEFAULT NULL COMMENT '存储维度分、输入条件等（JSON格式）',
    `owner_key` VARCHAR(64) DEFAULT NULL COMMENT '所有者标识（预留，当前用于匿名收藏）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_name` (`name`),
    INDEX `idx_type` (`type`),
    INDEX `idx_owner_key` (`owner_key`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='名字收藏表';

