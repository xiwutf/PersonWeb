-- ============================================
-- 副业项目站内提醒功能 - 数据库迁移
-- ============================================

-- 1. 为 side_project 表添加 blocked_at 字段（用于记录阻塞时间）
ALTER TABLE `side_project`
ADD COLUMN `blocked_at` DATETIME NULL COMMENT '阻塞时间（用于计算卡住天数）' AFTER `blocked`;

-- 2. 创建 side_notification 表
CREATE TABLE IF NOT EXISTS `side_notification` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `user_id` INT NULL COMMENT '用户ID（可先固定为空或默认管理员）',
  `type` INT NOT NULL COMMENT '提醒类型：1=项目即将到期, 2=任务今天到期, 3=项目卡住超过2天',
  `title` VARCHAR(100) NOT NULL COMMENT '提醒标题',
  `content` VARCHAR(500) NULL COMMENT '提醒内容',
  `severity` INT NOT NULL DEFAULT 1 COMMENT '严重程度：1=信息, 2=警告, 3=危险/紧急',
  `entity_type` VARCHAR(50) NOT NULL COMMENT '实体类型（SideProject / SideProjectTask）',
  `entity_id` BIGINT NOT NULL COMMENT '实体ID',
  `payload_json` TEXT NULL COMMENT '负载数据（JSON格式）',
  `is_read` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否已读',
  `read_at` DATETIME NULL COMMENT '已读时间',
  `is_dismissed` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否已忽略',
  `dismissed_at` DATETIME NULL COMMENT '忽略时间',
  `snooze_until` DATETIME NULL COMMENT '延后到某个时间再出现',
  `occur_date` DATE NOT NULL COMMENT '发生日期（用于去重，同一天同实体同类型只一条）',
  `first_triggered_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '首次触发时间',
  `last_triggered_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '最后触发时间',
  `trigger_count` INT NOT NULL DEFAULT 1 COMMENT '触发次数',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`id`),
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_type_entity` (`type`, `entity_type`, `entity_id`),
  INDEX `idx_occur_date` (`occur_date`),
  INDEX `idx_is_read` (`is_read`),
  INDEX `idx_is_dismissed` (`is_dismissed`),
  INDEX `idx_snooze_until` (`snooze_until`),
  INDEX `idx_created_at` (`created_at`),
  -- 唯一索引：同一天同实体同类型只一条（用于去重）
  UNIQUE KEY `uk_type_entity_occur` (`type`, `entity_type`, `entity_id`, `occur_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='副业项目站内提醒表';

