-- 为 article 表添加 source_type 字段
-- 添加来源类型字段：manual（手动创建）、ai_generated（AI生成）、ai_optimized（AI优化）、imported（导入）

-- 添加字段（允许为空，默认值为 'manual'）
ALTER TABLE article
ADD COLUMN IF NOT EXISTS source_type VARCHAR(50) NOT NULL DEFAULT 'manual';

-- 创建索引以提高查询性能
CREATE INDEX IF NOT EXISTS idx_article_source_type ON article(source_type);

-- 更新现有数据的来源类型（默认为手动创建）
UPDATE article
SET source_type = 'manual'
WHERE source_type IS NULL;

-- 添加注释
COMMENT ON COLUMN article.source_type IS '来源类型：manual（手动创建）、ai_generated（AI生成）、ai_optimized（AI优化）、imported（导入）';