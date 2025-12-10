-- 个人助理会话表
CREATE TABLE IF NOT EXISTS `assistant_sessions` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL COMMENT '用户 ID（管理员）',
  `title` VARCHAR(200) NOT NULL COMMENT '会话标题（可自动生成）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='个人助理会话表';

-- 个人助理消息表
CREATE TABLE IF NOT EXISTS `assistant_messages` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `session_id` BIGINT NOT NULL COMMENT '会话 ID',
  `role` VARCHAR(20) NOT NULL COMMENT '角色：User / Assistant',
  `content` TEXT NOT NULL COMMENT '消息内容',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`id`),
  INDEX `idx_session_id` (`session_id`),
  INDEX `idx_created_at` (`created_at`),
  CONSTRAINT `fk_assistant_messages_session` FOREIGN KEY (`session_id`) REFERENCES `assistant_sessions` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='个人助理消息表';

