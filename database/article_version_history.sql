-- 为文章表添加版本历史支持
-- 添加 version 和 parent_id 字段

ALTER TABLE `article` 
ADD COLUMN `version` INT NOT NULL DEFAULT 1 COMMENT '版本号' AFTER `view_count`,
ADD COLUMN `parent_id` BIGINT NULL COMMENT '父版本ID（版本历史关联）' AFTER `version`;

-- 添加索引
CREATE INDEX `idx_parent_id` ON `article` (`parent_id`);
CREATE INDEX `idx_version` ON `article` (`version`);

