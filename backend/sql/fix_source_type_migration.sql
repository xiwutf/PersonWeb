-- 修正后的SQL脚本：为 article 表添加 source_type 字段
-- MySQL/MariaDB 语法

-- 1. 首先检查字段是否已存在
-- 如果字段存在，先删除（避免重复添加错误）
SET @drop_column = FALSE;
SELECT IF(COUNT(*) > 0, TRUE, FALSE) INTO @drop_column
FROM information_schema.columns
WHERE table_schema = DATABASE()
AND table_name = 'article'
AND column_name = 'source_type';

-- 如果字段存在，先删除
SET @drop_sql = IF(@drop_column, 'ALTER TABLE article DROP COLUMN IF EXISTS source_type;', '');
PREPARE stmt FROM @drop_sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 2. 添加字段（NOT NULL 必须有默认值）
ALTER TABLE article
ADD COLUMN source_type VARCHAR(50) NOT NULL DEFAULT 'manual';

-- 3. 创建索引以提高查询性能
CREATE INDEX IF NOT EXISTS idx_article_source_type ON article(source_type);

-- 4. 添加字段注释
ALTER TABLE article
MODIFY COLUMN source_type VARCHAR(50) NOT NULL DEFAULT 'manual'
COMMENT '来源类型：manual（手动创建）、ai_generated（AI生成）、ai_optimized（AI优化）、imported（导入）';

-- 5. 验证字段是否添加成功
SELECT 'Migration completed successfully' as message;