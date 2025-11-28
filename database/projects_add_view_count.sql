-- 为 projects 表添加 view_count 字段
-- 如果字段已存在，此语句会失败，可以忽略

ALTER TABLE `projects` 
ADD COLUMN `ViewCount` INT NOT NULL DEFAULT 0 COMMENT '访问量统计' AFTER `Content`;

-- 如果需要，也可以添加索引以优化查询
-- CREATE INDEX `idx_view_count` ON `projects` (`ViewCount`);

