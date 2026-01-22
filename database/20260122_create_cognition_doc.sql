-- ============================================
-- 认知说明书表（cognition_doc）
-- 创建时间: 2026-01-22
-- ============================================

CREATE TABLE IF NOT EXISTS `cognition_doc` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `title` VARCHAR(200) NOT NULL COMMENT '标题',
  `slug` VARCHAR(200) NOT NULL COMMENT 'URL 别名（唯一）',
  `summary` VARCHAR(500) DEFAULT NULL COMMENT '摘要',
  `content_md` LONGTEXT NOT NULL COMMENT 'Markdown 内容',
  `status` VARCHAR(20) NOT NULL DEFAULT 'draft' COMMENT '状态：draft-草稿, published-已发布',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_cognition_doc_slug` (`slug`),
  KEY `idx_cognition_doc_status` (`status`),
  KEY `idx_cognition_doc_updated_at` (`updated_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='认知说明书表';
