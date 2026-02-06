-- ============================================
-- 思维记录表（thought_records）
-- 用于「随手写 + AI 批注」模块
-- ============================================

CREATE TABLE IF NOT EXISTS `thought_records` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `content` LONGTEXT NOT NULL COMMENT '原文内容',
  `ai_comment` LONGTEXT DEFAULT NULL COMMENT 'AI 批注（Markdown）',
  `status` INT NOT NULL DEFAULT 0 COMMENT '状态：0-未批注，1-已批注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  KEY `idx_thought_records_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='思维记录表';
