-- 为 Projects 表添加 AI 生成字段
-- 使用标准的 MySQL 语法，兼容所有版本

-- 首先检查表是否存在，然后添加列
-- 注意：如果列已存在，会报错，可以安全忽略

-- 如果表名是 Projects（EF Core 默认复数形式，驼峰命名）
ALTER TABLE `Projects` 
ADD COLUMN `AiTitle` VARCHAR(200) NULL COMMENT 'AI 生成的标题' AFTER `Title`,
ADD COLUMN `AiHighlights` TEXT NULL COMMENT 'AI 生成的亮点（JSON 或分号分隔）' AFTER `Description`,
ADD COLUMN `AiDescription` LONGTEXT NULL COMMENT 'AI 生成的详情描述（Markdown）' AFTER `AiHighlights`,
ADD COLUMN `AiScenarios` TEXT NULL COMMENT 'AI 生成的使用场景' AFTER `AiDescription`,
ADD COLUMN `AiTargetUsers` VARCHAR(500) NULL COMMENT 'AI 生成的目标用户' AFTER `AiScenarios`,
ADD COLUMN `AiShortCardText` VARCHAR(200) NULL COMMENT 'AI 生成的卡片短文本' AFTER `AiTargetUsers`;

-- 如果 ViewCount 列不存在，也添加它
ALTER TABLE `Projects` 
ADD COLUMN `ViewCount` INT DEFAULT 0 COMMENT '访问量统计' AFTER `Content`;

-- 如果上面的表名不对，尝试小写的 projects
-- ALTER TABLE `projects` 
-- ADD COLUMN `ai_title` VARCHAR(200) NULL COMMENT 'AI 生成的标题' AFTER `Title`,
-- ADD COLUMN `ai_highlights` TEXT NULL COMMENT 'AI 生成的亮点（JSON 或分号分隔）' AFTER `Description`,
-- ADD COLUMN `ai_description` LONGTEXT NULL COMMENT 'AI 生成的详情描述（Markdown）' AFTER `ai_highlights`,
-- ADD COLUMN `ai_scenarios` TEXT NULL COMMENT 'AI 生成的使用场景' AFTER `ai_description`,
-- ADD COLUMN `ai_target_users` VARCHAR(500) NULL COMMENT 'AI 生成的目标用户' AFTER `ai_scenarios`,
-- ADD COLUMN `ai_short_card_text` VARCHAR(200) NULL COMMENT 'AI 生成的卡片短文本' AFTER `ai_target_users`,
-- ADD COLUMN `view_count` INT DEFAULT 0 COMMENT '访问量统计' AFTER `Content`;

