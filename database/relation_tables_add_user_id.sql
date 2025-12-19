-- ============================================
-- 关系跟进模块 - 添加 UserId 字段（支持多租户）
-- ============================================
-- 目的：为关系跟进模块添加 UserId 字段，支持多租户（多用户）模式
-- 说明：UserId 字段可为空，以兼容单用户模式；在多租户模式下，可以通过该字段区分不同用户的数据

-- 为 relation_person 表添加 user_id 字段
ALTER TABLE `relation_person` 
ADD COLUMN `user_id` VARCHAR(100) NULL COMMENT '用户ID（支持多租户，可为空以兼容单用户模式）' AFTER `id`,
ADD INDEX `idx_user_id` (`user_id`);

-- 为 relation_interaction 表添加 user_id 字段
ALTER TABLE `relation_interaction` 
ADD COLUMN `user_id` VARCHAR(100) NULL COMMENT '用户ID（支持多租户，可为空以兼容单用户模式）' AFTER `id`,
ADD INDEX `idx_user_id` (`user_id`);

-- 为 relation_task 表添加 user_id 字段
ALTER TABLE `relation_task` 
ADD COLUMN `user_id` VARCHAR(100) NULL COMMENT '用户ID（支持多租户，可为空以兼容单用户模式）' AFTER `id`,
ADD INDEX `idx_user_id` (`user_id`);

-- 如果需要回滚，可以使用以下语句：
-- ALTER TABLE `relation_person` DROP COLUMN `user_id`;
-- ALTER TABLE `relation_interaction` DROP COLUMN `user_id`;
-- ALTER TABLE `relation_task` DROP COLUMN `user_id`;

