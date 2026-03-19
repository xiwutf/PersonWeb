-- 最基础的SQL脚本：添加 source_type 字段
-- 逐条执行，避免批量语句出错

-- 第1步：添加字段
ALTER TABLE article ADD COLUMN IF NOT EXISTS source_type VARCHAR(50) NOT NULL DEFAULT 'manual';

-- 第2步：创建索引（如果索引不存在）
SET @db_name = DATABASE();
SET @tb_name = 'article';
SET @idx_name = 'idx_article_source_type';

-- 检查索引是否存在，不存在则创建
SET @create_index = CONCAT('CREATE INDEX IF NOT EXISTS ', @idx_name, ' ON ', @tb_name, '(source_type);');
PREPARE stmt FROM @create_index;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 第3步：添加字段注释
ALTER TABLE article
MODIFY COLUMN source_type VARCHAR(50) NOT NULL DEFAULT 'manual'
COMMENT '来源类型：manual（手动创建）、ai_generated（AI生成）、ai_optimized（AI优化）、imported（导入）';

-- 查看结果
SELECT 'Migration completed successfully!' as message;