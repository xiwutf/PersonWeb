-- 后台管理风格系统表结构

-- 1. 模块风格表
CREATE TABLE IF NOT EXISTS `admin_module_style` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `module_key` VARCHAR(50) NOT NULL UNIQUE COMMENT '模块标识，如 articles, projects, tasks',
  `module_name` VARCHAR(100) NOT NULL COMMENT '模块名称，如 文章管理',
  `style_config` JSON NOT NULL COMMENT '风格配置 JSON，包含颜色、字体、背景等',
  `enabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
  `is_default` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否是默认风格',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uq_module_key` (`module_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='后台模块风格表';

-- 2. 全局后台风格表
CREATE TABLE IF NOT EXISTS `admin_global_style` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `style_key` VARCHAR(50) NOT NULL UNIQUE COMMENT '风格标识，如 dark-tech, light-modern',
  `style_name` VARCHAR(100) NOT NULL COMMENT '风格名称，如 暗黑科技风',
  `description` TEXT NULL COMMENT '风格描述',
  `style_config` JSON NOT NULL COMMENT '全局风格配置 JSON',
  `preview_image` VARCHAR(255) NULL COMMENT '预览图URL',
  `enabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
  `is_default` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否是当前默认风格',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uq_style_key` (`style_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='后台全局风格表';

-- 插入默认全局风格
INSERT IGNORE INTO `admin_global_style` (`style_key`, `style_name`, `description`, `style_config`, `enabled`, `is_default`) VALUES
('dark-tech', '暗黑科技风', '深色背景，科技感网格，适合技术类后台', 
  JSON_OBJECT(
    'background', JSON_OBJECT(
      'type', 'gradient',
      'colors', JSON_ARRAY('#0f172a', '#1e293b', '#334155'),
      'grid', JSON_OBJECT('enabled', true, 'opacity', 0.03, 'size', 40)
    ),
    'sidebar', JSON_OBJECT(
      'bgColor', '#1e293b',
      'textColor', '#ffffff',
      'activeBgColor', '#3b82f6',
      'hoverBgColor', '#334155'
    ),
    'header', JSON_OBJECT(
      'bgColor', '#ffffff',
      'textColor', '#1e293b',
      'borderColor', '#e2e8f0'
    ),
    'card', JSON_OBJECT(
      'bgColor', '#ffffff',
      'borderColor', '#e2e8f0',
      'shadow', '0 1px 3px rgba(0,0,0,0.1)'
    ),
    'primaryColor', '#3b82f6',
    'secondaryColor', '#8b5cf6',
    'accentColor', '#06b6d4'
  ), 
  1, 1);

-- 插入默认模块风格示例
INSERT IGNORE INTO `admin_module_style` (`module_key`, `module_name`, `style_config`, `enabled`, `is_default`) VALUES
('articles', '文章管理', 
  JSON_OBJECT(
    'icon', 'fas fa-file-alt',
    'iconColor', '#3b82f6',
    'bgGradient', JSON_ARRAY('#dbeafe', '#bfdbfe'),
    'accentColor', '#2563eb'
  ), 
  1, 1),
('projects', '项目管理', 
  JSON_OBJECT(
    'icon', 'fas fa-project-diagram',
    'iconColor', '#8b5cf6',
    'bgGradient', JSON_ARRAY('#f3e8ff', '#e9d5ff'),
    'accentColor', '#7c3aed'
  ), 
  1, 1),
('tasks', '任务管理', 
  JSON_OBJECT(
    'icon', 'fas fa-tasks',
    'iconColor', '#10b981',
    'bgGradient', JSON_ARRAY('#d1fae5', '#a7f3d0'),
    'accentColor', '#059669'
  ), 
  1, 1);

