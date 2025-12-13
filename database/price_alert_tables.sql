-- 价格提醒相关数据库表结构
-- 
-- 设计原则：
-- 1. 不使用外键约束，通过逻辑关联维护表间关系
-- 2. 关联关系由应用层维护，便于后期维护和扩展
-- 3. 为关联字段创建索引以提升查询性能

-- 价格提醒表
CREATE TABLE IF NOT EXISTS `price_alert` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(20) NOT NULL COMMENT '股票/基金代码',
  `name` VARCHAR(100) NOT NULL COMMENT '名称',
  `type` VARCHAR(20) NOT NULL DEFAULT 'fund' COMMENT '类型：stock/fund',
  `target_price` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '目标价格',
  `alert_type` VARCHAR(20) NOT NULL DEFAULT 'above' COMMENT '提醒类型：above/below/both',
  `current_price` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '当前价格',
  `is_triggered` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否已触发',
  `triggered_at` DATETIME NULL COMMENT '触发时间',
  `is_active` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
  `notification_sent` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否已发送通知',
  `notes` TEXT NULL COMMENT '备注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_code` (`code`),
  INDEX `idx_type` (`type`),
  INDEX `idx_is_active` (`is_active`),
  INDEX `idx_is_triggered` (`is_triggered`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='价格提醒表';
