-- ============================================
-- 检查重复项目脚本
-- 用于查找数据库中标题相似或重复的项目
-- ============================================

-- 1. 查找完全相同的标题
SELECT 
    `Title`, 
    COUNT(*) AS `count`,
    GROUP_CONCAT(`Id` ORDER BY `CreatedAt` SEPARATOR ', ') AS `ids`
FROM `projects`
GROUP BY `Title`
HAVING COUNT(*) > 1;

-- 2. 查找标题相似的项目（可能重复）
-- 例如："个人数字资产平台" 和 "个人网站系统"
SELECT 
    `Id`,
    `Title`,
    `Description`,
    `Status`,
    `CreatedAt`
FROM `projects`
WHERE `Title` LIKE '%个人网站%' 
   OR `Title` LIKE '%个人数字资产%'
   OR `Title` LIKE '%AI%助手%'
   OR `Title` LIKE '%AI%创作%'
   OR `Title` LIKE '%访客分析%'
ORDER BY `Title`, `CreatedAt`;

-- 3. 统计所有项目
SELECT 
    COUNT(*) AS `total_projects`,
    COUNT(DISTINCT `Title`) AS `unique_titles`
FROM `projects`;

