-- ============================================
-- 更新日志和未来规划表结构
-- ============================================

-- 更新日志表
CREATE TABLE IF NOT EXISTS `changelog` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '日志ID',
    `version` VARCHAR(50) DEFAULT NULL COMMENT '版本号（如：2025.11.29）',
    `date` DATE NOT NULL COMMENT '更新日期',
    `title` VARCHAR(255) NOT NULL COMMENT '更新标题',
    `description` TEXT DEFAULT NULL COMMENT '更新描述（Markdown格式）',
    `items` TEXT DEFAULT NULL COMMENT '更新项列表（JSON数组）',
    `category` VARCHAR(50) DEFAULT 'feature' COMMENT '分类：feature-功能, fix-修复, improvement-改进',
    `status` TINYINT DEFAULT 1 COMMENT '状态：0-隐藏 1-显示',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_date` (`date`),
    INDEX `idx_status` (`status`),
    INDEX `idx_category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='更新日志表';

-- 未来规划表
CREATE TABLE IF NOT EXISTS `roadmap` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '规划ID',
    `title` VARCHAR(255) NOT NULL COMMENT '规划标题',
    `description` TEXT DEFAULT NULL COMMENT '规划描述',
    `items` TEXT DEFAULT NULL COMMENT '规划项列表（JSON数组）',
    `timeline` VARCHAR(50) NOT NULL COMMENT '时间线：short-短期, medium-中期, long-长期',
    `priority` INT DEFAULT 0 COMMENT '优先级：0-低, 1-中, 2-高, 3-紧急',
    `status` VARCHAR(50) DEFAULT 'planned' COMMENT '状态：planned-计划中, in_progress-进行中, completed-已完成, cancelled-已取消',
    `start_date` DATE DEFAULT NULL COMMENT '开始日期',
    `target_date` DATE DEFAULT NULL COMMENT '目标日期',
    `completed_date` DATE DEFAULT NULL COMMENT '完成日期',
    `progress` INT DEFAULT 0 COMMENT '进度百分比 0-100',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_timeline` (`timeline`),
    INDEX `idx_status` (`status`),
    INDEX `idx_priority` (`priority`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='未来规划表';

