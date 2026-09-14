-- ============================================
-- 阅读 Inbox（reading_entries）
-- MindTrace 外部发布接入，默认 private / inbox
-- ============================================

USE personal_site;

CREATE TABLE IF NOT EXISTS `reading_entries` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `source_type` VARCHAR(64) NOT NULL COMMENT '来源类型，如 mindtrace',
  `external_id` VARCHAR(191) NOT NULL COMMENT '外部唯一 ID（MindTrace thought id）',
  `quote` LONGTEXT NOT NULL COMMENT '摘录 / selectedText',
  `note` LONGTEXT NOT NULL COMMENT '想法 / note',
  `source_title` VARCHAR(512) NOT NULL DEFAULT '' COMMENT '来源页面标题',
  `source_url` VARCHAR(2048) NOT NULL DEFAULT '' COMMENT '来源 URL',
  `source_created_at` DATETIME NULL DEFAULT NULL COMMENT '来源侧创建时间',
  `imported_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '首次导入时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最近更新时间',
  `tags` TEXT NULL COMMENT '标签 JSON 数组',
  `status` VARCHAR(32) NOT NULL DEFAULT 'inbox' COMMENT 'inbox | archived | published',
  `visibility` VARCHAR(32) NOT NULL DEFAULT 'private' COMMENT 'private | public',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_reading_source_external` (`source_type`, `external_id`),
  KEY `idx_reading_status` (`status`),
  KEY `idx_reading_imported_at` (`imported_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='阅读 Inbox（外部发布）';
