-- ============================================
-- 为 side_project 表添加 completed_at 字段
-- ============================================

ALTER TABLE `side_project`
ADD COLUMN `completed_at` DATETIME NULL COMMENT '完成时间（状态变为已完成时写入）' AFTER `end_time`;

-- 为已完成的项目设置 completed_at（如果 end_time 存在则使用 end_time，否则使用 updated_at）
UPDATE `side_project`
SET `completed_at` = COALESCE(`end_time`, `updated_at`)
WHERE `status` = 1 AND `completed_at` IS NULL;

