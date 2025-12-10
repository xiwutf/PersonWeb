-- 客服智能体配置表
CREATE TABLE IF NOT EXISTS `support_config` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `config_key` VARCHAR(100) NOT NULL COMMENT '配置键（例如：system_prompt, faq_list）',
  `config_value` TEXT COMMENT '配置值（JSON 或文本）',
  `description` VARCHAR(500) COMMENT '配置描述',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_config_key` (`config_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客服智能体配置表';

-- 插入默认配置
INSERT INTO `support_config` (`config_key`, `config_value`, `description`) VALUES
('system_prompt', '你是溪午听风个人网站的智能客服助手。你的任务是友好、专业地回答访客的问题，帮助他们了解服务内容、项目开发、工具使用等信息。请保持简洁明了的回答风格。', '客服智能体系统提示词'),
('faq_list', '[]', 'FAQ 列表（JSON 格式）')
ON DUPLICATE KEY UPDATE `config_value` = VALUES(`config_value`);

