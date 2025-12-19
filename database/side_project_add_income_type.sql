-- ============================================
-- 为 side_project 表添加收入类型字段
-- ============================================

-- 添加 income_type 字段（收入类型：development=软件开发, investment=投资）
ALTER TABLE `side_project` 
ADD COLUMN `income_type` VARCHAR(50) NULL DEFAULT 'development' COMMENT '收入类型（development=软件开发, investment=投资）' AFTER `category`,
ADD INDEX `idx_income_type` (`income_type`);

-- 更新现有数据，默认设置为软件开发
UPDATE `side_project` SET `income_type` = 'development' WHERE `income_type` IS NULL;

-- 示例数据已删除，请使用真实数据

