-- ============================================
-- 技能表结构更新脚本（简化版）
-- 直接添加字段，如果字段已存在会报错，可以忽略
-- ============================================

-- 添加 current_rating 字段
ALTER TABLE `skill` 
ADD COLUMN `current_rating` DECIMAL(3,1) DEFAULT 0.0 COMMENT '当前评分（0-10）' AFTER `icon`;

-- 添加 target_rating 字段
ALTER TABLE `skill` 
ADD COLUMN `target_rating` DECIMAL(3,1) DEFAULT NULL COMMENT '目标评分' AFTER `current_rating`;

-- 注意：如果字段已存在，会报错 "Duplicate column name"，可以忽略
-- 如果报错，说明字段已经存在，无需再次添加

