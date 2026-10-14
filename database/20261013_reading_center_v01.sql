-- Reading center schema v0.1 (2026-10-13), MySQL 8 / InnoDB / utf8mb4.
-- Select the existing PersonWeb database before sourcing this file.
-- Prerequisite: user(id BIGINT signed) exists. No existing tables/data are modified.
-- CREATE IF NOT EXISTS supports repeat execution, but does NOT repair schema drift.
-- Apply reviewed SQL manually; application startup never creates these tables.

CREATE TABLE IF NOT EXISTS `reading_books` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'Book ID',
  `owner_id` BIGINT NOT NULL COMMENT 'Real site owner account',
  `title` VARCHAR(512) NOT NULL COMMENT 'EPUB title',
  `author` VARCHAR(512) NOT NULL DEFAULT '' COMMENT 'EPUB creator',
  `language` VARCHAR(64) NOT NULL DEFAULT '' COMMENT 'EPUB language',
  `sha256` VARCHAR(64) NOT NULL COMMENT 'Immutable validated publication hash',
  `storage_key` VARCHAR(64) NOT NULL COMMENT 'Private key, never a public URL',
  `file_size` BIGINT NOT NULL COMMENT 'Archive bytes',
  `content_version` INT NOT NULL DEFAULT 1 COMMENT 'Immutable content version',
  `validation_profile` VARCHAR(32) NOT NULL DEFAULT 'strict-v1' COMMENT 'Validation policy version',
  `created_at` DATETIME(6) NOT NULL COMMENT 'UTC created time',
  `updated_at` DATETIME(6) NOT NULL COMMENT 'UTC modified time',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_reading_book_owner_id` (`owner_id`, `id`),
  UNIQUE KEY `uk_reading_book_owner_hash` (`owner_id`, `sha256`),
  CONSTRAINT `fk_reading_book_owner` FOREIGN KEY (`owner_id`) REFERENCES `user` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Private EPUB books';

CREATE TABLE IF NOT EXISTS `reading_progress` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'Progress ID',
  `owner_id` BIGINT NOT NULL COMMENT 'Book owner',
  `book_id` BIGINT NOT NULL COMMENT 'Book ID',
  `content_version` INT NOT NULL COMMENT 'CFI content version',
  `cfi` VARCHAR(4096) NOT NULL COMMENT 'Full EPUB CFI',
  `chapter_href` VARCHAR(1024) NOT NULL DEFAULT '' COMMENT 'Package-relative chapter',
  `progression` DOUBLE NOT NULL DEFAULT 0 COMMENT 'Display only, range 0..1',
  `revision` BIGINT NOT NULL DEFAULT 0 COMMENT 'Optimistic concurrency revision',
  `last_session_id` VARCHAR(64) NOT NULL DEFAULT '' COMMENT 'Reader session',
  `last_read_at` DATETIME(6) NOT NULL COMMENT 'UTC actual reading time',
  `updated_at` DATETIME(6) NOT NULL COMMENT 'UTC save time',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_reading_progress_owner_book` (`owner_id`, `book_id`),
  KEY `idx_reading_progress_recent` (`owner_id`, `last_read_at`),
  CONSTRAINT `fk_reading_progress_book` FOREIGN KEY (`owner_id`, `book_id`)
    REFERENCES `reading_books` (`owner_id`, `id`) ON DELETE CASCADE,
  CONSTRAINT `chk_reading_progress_fraction` CHECK (`progression` >= 0 AND `progression` <= 1)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Confirmed private reading position';

CREATE TABLE IF NOT EXISTS `reading_annotations` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'Annotation ID',
  `owner_id` BIGINT NOT NULL COMMENT 'Book owner',
  `book_id` BIGINT NOT NULL COMMENT 'Book ID',
  `content_version` INT NOT NULL COMMENT 'Anchor content version',
  `kind` VARCHAR(16) NOT NULL DEFAULT 'highlight' COMMENT 'highlight or note',
  `cfi_range` VARCHAR(4096) NOT NULL COMMENT 'Full EPUB CFI range',
  `chapter_href` VARCHAR(1024) NOT NULL DEFAULT '' COMMENT 'Chapter fallback',
  `quote` LONGTEXT NOT NULL COMMENT 'Selected original text',
  `prefix` VARCHAR(512) NOT NULL DEFAULT '' COMMENT 'Text preceding quote',
  `suffix` VARCHAR(512) NOT NULL DEFAULT '' COMMENT 'Text following quote',
  `note` LONGTEXT NOT NULL COMMENT 'Private note',
  `color` VARCHAR(16) NOT NULL DEFAULT 'yellow' COMMENT 'Semantic highlight color',
  `anchor_status` VARCHAR(16) NOT NULL DEFAULT 'valid' COMMENT 'valid or unresolved',
  `request_id` VARCHAR(64) NOT NULL COMMENT 'Idempotent creation request',
  `revision` BIGINT NOT NULL DEFAULT 0 COMMENT 'Optimistic concurrency revision',
  `created_at` DATETIME(6) NOT NULL COMMENT 'UTC created time',
  `updated_at` DATETIME(6) NOT NULL COMMENT 'UTC modified time',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_reading_annotation_request` (`owner_id`, `book_id`, `request_id`),
  CONSTRAINT `fk_reading_annotation_book` FOREIGN KEY (`owner_id`, `book_id`)
    REFERENCES `reading_books` (`owner_id`, `id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Private CFI-linked annotations';

CREATE TABLE IF NOT EXISTS `reading_preferences` (
  `owner_id` BIGINT NOT NULL COMMENT 'Real site owner account',
  `font_family` VARCHAR(64) NOT NULL DEFAULT 'serif' COMMENT 'Reader font preference',
  `font_size` INT NOT NULL DEFAULT 18 COMMENT 'Font size in CSS pixels',
  `line_height` DOUBLE NOT NULL DEFAULT 1.6 COMMENT 'Line height multiplier',
  `theme` VARCHAR(16) NOT NULL DEFAULT 'paper' COMMENT 'Reader theme',
  `revision` BIGINT NOT NULL DEFAULT 0 COMMENT 'Optimistic concurrency revision',
  `updated_at` DATETIME(6) NOT NULL COMMENT 'UTC modified time',
  PRIMARY KEY (`owner_id`),
  CONSTRAINT `fk_reading_preferences_owner` FOREIGN KEY (`owner_id`) REFERENCES `user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Private reading preferences';
