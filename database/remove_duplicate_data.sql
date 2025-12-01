-- ============================================
-- 清理重复数据脚本
-- 用于删除 enrich_content_data.sql 中与 complete_content_data.sql 重复的项目
-- ============================================

-- 删除重复的项目（保留 complete_content_data.sql 中的版本）
-- 1. 删除 "个人网站 V2"（与 "个人数字资产平台（本网站）" 重复）
DELETE FROM `projects` 
WHERE `Title` = '个人网站 V2';

-- 2. 删除 "访客分析系统"（与 "访客分析系统（Analytics）" 重复）
DELETE FROM `projects` 
WHERE `Title` = '访客分析系统' 
AND `Title` != '访客分析系统（Analytics）';

-- 注意：如果还有其他重复的项目，请手动检查并删除

-- 验证删除结果
SELECT '剩余项目数量' AS '类型', COUNT(*) AS '数量' FROM `projects`;

