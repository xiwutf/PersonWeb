-- 错误日志表
CREATE TABLE IF NOT EXISTS `error_log` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `error_type` VARCHAR(50) NOT NULL COMMENT '错误类型: JavaScript, API, Server',
  `error_message` VARCHAR(500) NOT NULL COMMENT '错误消息',
  `error_stack` TEXT NULL COMMENT '错误堆栈',
  `error_url` VARCHAR(500) NULL COMMENT '错误发生的URL',
  `user_ip` VARCHAR(45) NULL COMMENT '用户IP',
  `user_agent` VARCHAR(500) NULL COMMENT '用户代理',
  `visitor_id` VARCHAR(36) NULL COMMENT '访客ID',
  `metadata` TEXT NULL COMMENT '额外信息（JSON格式）',
  `status` TINYINT NOT NULL DEFAULT 0 COMMENT '状态: 0-未处理 1-已处理 2-已忽略',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `resolved_at` DATETIME NULL COMMENT '处理时间',
  PRIMARY KEY (`id`),
  INDEX `idx_error_type` (`error_type`),
  INDEX `idx_status` (`status`),
  INDEX `idx_created_at` (`created_at`),
  INDEX `idx_visitor_id` (`visitor_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='错误日志表';

