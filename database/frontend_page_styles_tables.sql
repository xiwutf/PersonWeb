-- ============================================
-- 前端页面样式配置表
-- 用于在后台管理系统中配置前端页面的 CSS 样式
-- ============================================

-- 1. 页面样式配置表
CREATE TABLE IF NOT EXISTS `frontend_page_style` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `page_key` VARCHAR(100) NOT NULL COMMENT '页面标识，如 tools, blog, life, projects, about',
  `page_name` VARCHAR(100) NOT NULL COMMENT '页面名称，如 工具页面、博客页面',
  `style_config` JSON NOT NULL COMMENT '样式配置 JSON，包含所有 CSS 变量和样式规则',
  `enabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
  `is_default` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否是默认样式',
  `version` INT NOT NULL DEFAULT 1 COMMENT '样式版本号，用于缓存控制',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uq_page_key` (`page_key`),
  INDEX `idx_enabled` (`enabled`),
  INDEX `idx_is_default` (`is_default`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='前端页面样式配置表';

-- 2. 样式变量表（用于存储可配置的 CSS 变量）
CREATE TABLE IF NOT EXISTS `frontend_style_variable` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `page_key` VARCHAR(100) NOT NULL COMMENT '页面标识',
  `variable_key` VARCHAR(100) NOT NULL COMMENT '变量键名，如 primary-color, background-color',
  `variable_value` TEXT NOT NULL COMMENT '变量值，如 #0f172a, rgba(30, 41, 59, 0.3)',
  `variable_type` VARCHAR(50) NOT NULL DEFAULT 'color' COMMENT '变量类型：color, size, spacing, font, etc.',
  `description` VARCHAR(255) NULL COMMENT '变量描述',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uq_page_variable` (`page_key`, `variable_key`),
  INDEX `idx_page_key` (`page_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='前端样式变量表';

-- 3. 样式规则表（用于存储自定义 CSS 规则）
CREATE TABLE IF NOT EXISTS `frontend_style_rule` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `page_key` VARCHAR(100) NOT NULL COMMENT '页面标识',
  `selector` VARCHAR(255) NOT NULL COMMENT 'CSS 选择器，如 .tools-card, .blog-post-title',
  `css_properties` JSON NOT NULL COMMENT 'CSS 属性 JSON，如 {"color": "#fff", "font-size": "1.5rem"}',
  `priority` INT NOT NULL DEFAULT 0 COMMENT '优先级，数字越大优先级越高',
  `enabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
  `description` VARCHAR(255) NULL COMMENT '规则描述',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_page_key` (`page_key`),
  INDEX `idx_selector` (`selector`),
  INDEX `idx_enabled` (`enabled`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='前端样式规则表';

-- 插入默认样式配置示例（tools 页面）
INSERT IGNORE INTO `frontend_page_style` (`page_key`, `page_name`, `style_config`, `enabled`, `is_default`) VALUES
('tools', '工具页面', '{
  "primaryColor": "#f97316",
  "secondaryColor": "#dc2626",
  "backgroundColor": "#0f172a",
  "textColor": "#e2e8f0",
  "cardBackground": "rgba(30, 41, 59, 0.3)",
  "borderColor": "rgba(255, 255, 255, 0.05)",
  "borderRadius": "1.5rem",
  "fontFamily": "\"Outfit\", sans-serif"
}', 1, 1),
('blog', '博客页面', '{
  "primaryColor": "#3b82f6",
  "secondaryColor": "#10b981",
  "backgroundColor": "#0f172a",
  "textColor": "#e2e8f0",
  "cardBackground": "rgba(30, 41, 59, 0.3)",
  "borderColor": "rgba(255, 255, 255, 0.05)",
  "borderRadius": "1rem",
  "fontFamily": "\"Outfit\", sans-serif"
}', 1, 1),
('life', '生活随笔页面', '{
  "primaryColor": "#f59e0b",
  "secondaryColor": "#f97316",
  "backgroundColor": "#0f172a",
  "textColor": "#e2e8f0",
  "cardBackground": "rgba(30, 41, 59, 0.4)",
  "borderColor": "rgba(255, 255, 255, 0.05)",
  "borderRadius": "1rem",
  "fontFamily": "\"Outfit\", sans-serif"
}', 1, 1),
('projects', '项目页面', '{
  "primaryColor": "#3b82f6",
  "secondaryColor": "#10b981",
  "backgroundColor": "#ffffff",
  "textColor": "#111827",
  "cardBackground": "#ffffff",
  "borderColor": "#e5e7eb",
  "borderRadius": "0.5rem",
  "fontFamily": "system-ui, sans-serif"
}', 1, 1),
('about', '关于页面', '{
  "primaryColor": "#8b5cf6",
  "secondaryColor": "#ec4899",
  "backgroundColor": "#0f172a",
  "textColor": "#e2e8f0",
  "cardBackground": "rgba(30, 41, 59, 0.3)",
  "borderColor": "rgba(255, 255, 255, 0.05)",
  "borderRadius": "1.5rem",
  "fontFamily": "\"Outfit\", sans-serif"
}', 1, 1);

