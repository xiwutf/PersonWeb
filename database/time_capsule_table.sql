-- 时间胶囊表
CREATE TABLE IF NOT EXISTS `time_capsule` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `content` TEXT NOT NULL COMMENT '胶囊内容',
  `visitor_id` VARCHAR(36) NULL COMMENT '访客ID',
  `visitor_name` VARCHAR(50) NULL COMMENT '访客名称',
  `status` TINYINT NOT NULL DEFAULT 0 COMMENT '状态：0-待审核 1-已展示 2-已拒绝',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='时间胶囊表';

