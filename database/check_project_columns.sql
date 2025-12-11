-- 检查 projects 表的列结构
-- 用于确认哪些列已存在，哪些需要添加

-- 查看表结构
DESCRIBE `projects`;

-- 或者使用以下查询查看所有列名
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT, COLUMN_COMMENT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'personal_site' 
  AND TABLE_NAME = 'projects'
ORDER BY ORDINAL_POSITION;

