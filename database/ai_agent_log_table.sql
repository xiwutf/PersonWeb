-- AI 智能体调用日志表
-- 用于记录所有 AI 调用的请求和响应，便于监控和调试

CREATE TABLE IF NOT EXISTS `ai_agent_log` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `agent_type` VARCHAR(50) NOT NULL COMMENT '智能体类型：Content（内容生成）、Demo（Demo上架）、Lead（线索处理）',
  `request_payload` TEXT COMMENT '请求内容（JSON 格式）',
  `response_payload` TEXT COMMENT '响应内容（JSON 格式）',
  `success` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否成功',
  `error_message` VARCHAR(1000) COMMENT '错误信息（如果失败）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`id`),
  INDEX `idx_agent_type` (`agent_type`),
  INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='AI 智能体调用日志表';

