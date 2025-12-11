-- 为 Projects 表添加 AI 生成字段
-- 注意：EF Core 可能使用驼峰命名（PascalCase）或下划线命名（snake_case）
-- 此脚本同时支持两种命名方式

-- 方案 1: 如果表名是 Projects（复数，驼峰命名）
-- 检查并添加列（使用驼峰命名）
ALTER TABLE `Projects` 
ADD COLUMN IF NOT EXISTS `AiTitle` VARCHAR(200) NULL COMMENT 'AI 生成的标题' AFTER `Title`,
ADD COLUMN IF NOT EXISTS `AiHighlights` TEXT NULL COMMENT 'AI 生成的亮点（JSON 或分号分隔）' AFTER `Description`,
ADD COLUMN IF NOT EXISTS `AiDescription` LONGTEXT NULL COMMENT 'AI 生成的详情描述（Markdown）' AFTER `AiHighlights`,
ADD COLUMN IF NOT EXISTS `AiScenarios` TEXT NULL COMMENT 'AI 生成的使用场景' AFTER `AiDescription`,
ADD COLUMN IF NOT EXISTS `AiTargetUsers` VARCHAR(500) NULL COMMENT 'AI 生成的目标用户' AFTER `AiScenarios`,
ADD COLUMN IF NOT EXISTS `AiShortCardText` VARCHAR(200) NULL COMMENT 'AI 生成的卡片短文本' AFTER `AiTargetUsers`,
ADD COLUMN IF NOT EXISTS `ViewCount` INT DEFAULT 0 COMMENT '访问量统计' AFTER `Content`;

-- 方案 2: 如果表名是 projects（小写，下划线命名）
-- 检查并添加列（使用下划线命名）
ALTER TABLE `projects` 
ADD COLUMN IF NOT EXISTS `ai_title` VARCHAR(200) NULL COMMENT 'AI 生成的标题' AFTER `Title`,
ADD COLUMN IF NOT EXISTS `ai_highlights` TEXT NULL COMMENT 'AI 生成的亮点（JSON 或分号分隔）' AFTER `Description`,
ADD COLUMN IF NOT EXISTS `ai_description` LONGTEXT NULL COMMENT 'AI 生成的详情描述（Markdown）' AFTER `ai_highlights`,
ADD COLUMN IF NOT EXISTS `ai_scenarios` TEXT NULL COMMENT 'AI 生成的使用场景' AFTER `ai_description`,
ADD COLUMN IF NOT EXISTS `ai_target_users` VARCHAR(500) NULL COMMENT 'AI 生成的目标用户' AFTER `ai_scenarios`,
ADD COLUMN IF NOT EXISTS `ai_short_card_text` VARCHAR(200) NULL COMMENT 'AI 生成的卡片短文本' AFTER `ai_target_users`,
ADD COLUMN IF NOT EXISTS `view_count` INT DEFAULT 0 COMMENT '访问量统计' AFTER `Content`;

-- 注意：MySQL 的 ADD COLUMN IF NOT EXISTS 语法在某些版本中可能不支持
-- 如果执行失败，请使用以下方式手动检查并添加：

-- 检查列是否存在的存储过程方式（适用于不支持 IF NOT EXISTS 的 MySQL 版本）
-- 或者直接执行以下语句，如果列已存在会报错，可以忽略

