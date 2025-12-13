-- 资产相关数据库表结构
-- 
-- 设计原则：
-- 1. 不使用外键约束，通过逻辑关联维护表间关系
-- 2. 关联关系由应用层维护，便于后期维护和扩展
-- 3. 为关联字段创建索引以提升查询性能

-- 资产记录表（银行卡、存单等非投资类资产）
CREATE TABLE IF NOT EXISTS `asset` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL COMMENT '资产名称（如：招商银行储蓄卡、定期存单等）',
  `asset_type` VARCHAR(50) NOT NULL DEFAULT 'bank_card' COMMENT '资产类型：bank_card/deposit/cash/other',
  `institution` VARCHAR(100) NULL COMMENT '机构名称（如：招商银行、工商银行等）',
  `amount` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '资产金额',
  `currency` VARCHAR(50) NOT NULL DEFAULT 'CNY' COMMENT '货币类型（CNY/USD等）',
  `interest_rate` DECIMAL(5, 2) NOT NULL DEFAULT 0 COMMENT '利率（年化，百分比）',
  `maturity_date` DATETIME NULL COMMENT '到期日期（存单等）',
  `notes` TEXT NULL COMMENT '备注',
  `is_active` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_asset_type` (`asset_type`),
  INDEX `idx_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='资产记录表';
