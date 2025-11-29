-- ============================================
-- 仪表盘指标表
-- 用于存储数字孪生仪表盘的健康指标数据
-- ============================================

CREATE TABLE IF NOT EXISTS `dashboard_metric` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '指标ID',
    `date` DATE NOT NULL COMMENT '日期',
    `steps` INT DEFAULT 0 COMMENT '步数',
    `sleep` DECIMAL(4,1) DEFAULT 0.0 COMMENT '睡眠时长（小时）',
    `weight` DECIMAL(5,2) DEFAULT NULL COMMENT '体重（kg）',
    `net_worth` DECIMAL(12,2) DEFAULT NULL COMMENT '净资产',
    `energy` INT DEFAULT 0 COMMENT '精力值（0-100）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_date` (`date`),
    INDEX `idx_date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='仪表盘指标表';

