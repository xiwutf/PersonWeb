-- ============================================
-- 清理重复项目脚本
-- 删除重复或相似的项目，保留最早创建的版本
-- ============================================

-- 注意：执行前请先备份数据库！

-- 1. 删除完全重复的项目（保留最早创建的）
-- 删除 "个人网站 V2"（如果存在，与 "个人数字资产平台（本网站）" 重复）
DELETE FROM `projects` 
WHERE `Title` = '个人网站 V2';

-- 2. 删除 "访客分析系统"（如果存在，与 "访客分析系统（Analytics）" 重复）
DELETE FROM `projects` 
WHERE `Title` = '访客分析系统' 
AND `Title` != '访客分析系统（Analytics）';

-- 3. 删除 "个人网站系统"（如果存在，与 "个人数字资产平台（本网站）" 相似）
-- 注意：这个需要手动确认，因为可能不是完全重复
-- 如果确认是重复的，可以取消下面的注释
DELETE FROM `projects` 
WHERE `Title` = '个人网站系统'
AND EXISTS (
  SELECT 1 FROM `projects` p2 
  WHERE p2.`Title` LIKE '%个人数字资产平台%'
);

-- 4. 删除 "AI 智能助手"（如果存在，与 "AI 创作助手" 相似）
-- 注意：这个需要手动确认，因为可能不是完全重复
-- 如果确认是重复的，可以取消下面的注释
DELETE FROM `projects` 
WHERE `Title` = 'AI 智能助手'
AND EXISTS (
  SELECT 1 FROM `projects` p2 
  WHERE p2.`Title` LIKE '%AI 创作助手%'
);

-- 5. 验证删除结果
SELECT 
    `Id`,
    `Title`,
    `Description`,
    `Status`,
    `CreatedAt`
FROM `projects`
ORDER BY `Title`, `CreatedAt`;

