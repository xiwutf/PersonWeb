-- 快速迁移：为 article 表添加 source_type 字段
-- MySQL/MariaDB

ALTER TABLE article
ADD COLUMN IF NOT EXISTS source_type VARCHAR(50) NOT NULL DEFAULT 'manual';

CREATE INDEX IF NOT EXISTS idx_article_source_type ON article(source_type);