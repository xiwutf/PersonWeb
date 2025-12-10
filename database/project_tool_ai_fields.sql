-- 为 Project 和 Tool 表添加 AI 生成字段

-- Project 表添加 AI 字段
ALTER TABLE `project` 
ADD COLUMN IF NOT EXISTS `ai_title` VARCHAR(200) COMMENT 'AI 生成的标题' AFTER `title`,
ADD COLUMN IF NOT EXISTS `ai_highlights` TEXT COMMENT 'AI 生成的亮点（JSON 或分号分隔）' AFTER `description`,
ADD COLUMN IF NOT EXISTS `ai_description` LONGTEXT COMMENT 'AI 生成的详情描述（Markdown）' AFTER `ai_highlights`,
ADD COLUMN IF NOT EXISTS `ai_scenarios` TEXT COMMENT 'AI 生成的使用场景' AFTER `ai_description`,
ADD COLUMN IF NOT EXISTS `ai_target_users` VARCHAR(500) COMMENT 'AI 生成的目标用户' AFTER `ai_scenarios`,
ADD COLUMN IF NOT EXISTS `ai_short_card_text` VARCHAR(200) COMMENT 'AI 生成的卡片短文本' AFTER `ai_target_users`;

-- Tool 表添加 AI 字段
ALTER TABLE `tool` 
ADD COLUMN IF NOT EXISTS `ai_title` VARCHAR(200) COMMENT 'AI 生成的标题' AFTER `name`,
ADD COLUMN IF NOT EXISTS `ai_highlights` TEXT COMMENT 'AI 生成的亮点（JSON 或分号分隔）' AFTER `description`,
ADD COLUMN IF NOT EXISTS `ai_description` LONGTEXT COMMENT 'AI 生成的详情描述（Markdown）' AFTER `ai_highlights`,
ADD COLUMN IF NOT EXISTS `ai_scenarios` TEXT COMMENT 'AI 生成的使用场景' AFTER `ai_description`,
ADD COLUMN IF NOT EXISTS `ai_target_users` VARCHAR(500) COMMENT 'AI 生成的目标用户' AFTER `ai_scenarios`,
ADD COLUMN IF NOT EXISTS `ai_short_card_text` VARCHAR(200) COMMENT 'AI 生成的卡片短文本' AFTER `ai_target_users`;

