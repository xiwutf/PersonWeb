-- 简单版本的SQL脚本：添加 source_type 字段
-- 适用于 MySQL/MariaDB

-- 步骤1：检查并添加字段（如果不存在）
SET @sql = (SELECT IF(
  (SELECT COUNT(*) FROM information_schema.columns
   WHERE table_schema = DATABASE()
   AND table_name = 'article'
   AND column_name = 'source_type') > 0,
  'SELECT ''Column source_type already exists'' as message;',
  'ALTER TABLE article ADD COLUMN source_type VARCHAR(50) NOT NULL DEFAULT ''manual'';'
));
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 步骤2：创建索引（如果不存在）
SET @sql = (SELECT IF(
  (SELECT COUNT(*) FROM information_schema.statistics
   WHERE table_schema = DATABASE()
   AND table_name = 'article'
   AND index_name = 'idx_article_source_type') > 0,
  'SELECT ''Index already exists'' as message;',
  'CREATE INDEX idx_article_source_type ON article(source_type);'
));
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 步骤3：添加或更新字段注释
ALTER TABLE article
MODIFY COLUMN source_type VARCHAR(50) NOT NULL DEFAULT 'manual'
COMMENT '来源类型：manual（手动创建）、ai_generated（AI生成）、ai_optimized（AI优化）、imported（导入）';

-- 完成提示
SELECT 'Migration completed successfully!' as result;