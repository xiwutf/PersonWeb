-- 为 PreSaleConsultation 表添加 AI 分析字段

ALTER TABLE `pre_sale_consultations` 
ADD COLUMN IF NOT EXISTS `summary` TEXT COMMENT 'AI 生成的摘要' AFTER `updated_at`,
ADD COLUMN IF NOT EXISTS `tags` VARCHAR(500) COMMENT 'AI 生成的标签（JSON 数组或逗号分隔字符串）' AFTER `summary`,
ADD COLUMN IF NOT EXISTS `score` INT COMMENT 'AI 评分（0-100）' AFTER `tags`,
ADD COLUMN IF NOT EXISTS `ai_recommendation` TEXT COMMENT 'AI 推荐建议' AFTER `score`;

