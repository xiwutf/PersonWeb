-- 风格切换系统数据库表结构
-- 用于管理多种主题风格和动态背景

-- ============================================
-- 1. ThemeStyles 表 - 主题风格定义
-- ============================================
CREATE TABLE IF NOT EXISTS `theme_style` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '主题ID',
    `name` VARCHAR(50) NOT NULL COMMENT '主题名称（如：默认风格、极简风等）',
    `code` VARCHAR(50) NOT NULL COMMENT '主题代码（如：default、minimal、cyberpunk等）',
    `display_name` VARCHAR(100) NOT NULL COMMENT '显示名称',
    `description` VARCHAR(255) DEFAULT NULL COMMENT '主题描述',
    `preview_image` VARCHAR(500) DEFAULT NULL COMMENT '预览图URL',
    `is_enabled` TINYINT DEFAULT 1 COMMENT '是否启用：0-禁用 1-启用',
    `is_default` TINYINT DEFAULT 0 COMMENT '是否默认：0-否 1-是',
    `sort` INT DEFAULT 0 COMMENT '排序',
    `style_config` TEXT DEFAULT NULL COMMENT '样式配置（JSON格式）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_code` (`code`),
    INDEX `idx_is_enabled` (`is_enabled`),
    INDEX `idx_is_default` (`is_default`),
    INDEX `idx_sort` (`sort`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主题风格表';

-- ============================================
-- 2. BackgroundEffects 表 - 动态背景效果
-- ============================================
CREATE TABLE IF NOT EXISTS `background_effect` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '背景效果ID',
    `name` VARCHAR(50) NOT NULL COMMENT '效果名称（如：粒子流、噪声纹理等）',
    `code` VARCHAR(50) NOT NULL COMMENT '效果代码（如：particles、noise、waves等）',
    `display_name` VARCHAR(100) NOT NULL COMMENT '显示名称',
    `description` VARCHAR(255) DEFAULT NULL COMMENT '效果描述',
    `preview_image` VARCHAR(500) DEFAULT NULL COMMENT '预览图URL',
    `effect_config` TEXT DEFAULT NULL COMMENT '效果配置（JSON格式）',
    `is_enabled` TINYINT DEFAULT 1 COMMENT '是否启用：0-禁用 1-启用',
    `sort` INT DEFAULT 0 COMMENT '排序',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_code` (`code`),
    INDEX `idx_is_enabled` (`is_enabled`),
    INDEX `idx_sort` (`sort`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='动态背景效果表';

-- ============================================
-- 3. ThemeSettings 表 - 主题设置（全局配置）
-- ============================================
CREATE TABLE IF NOT EXISTS `theme_setting` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '设置ID',
    `setting_key` VARCHAR(50) NOT NULL COMMENT '设置键',
    `setting_value` TEXT DEFAULT NULL COMMENT '设置值（JSON格式）',
    `description` VARCHAR(255) DEFAULT NULL COMMENT '设置描述',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_setting_key` (`setting_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主题设置表';

-- ============================================
-- 4. UserThemePreferences 表 - 用户主题偏好
-- ============================================
CREATE TABLE IF NOT EXISTS `user_theme_preference` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '偏好ID',
    `visitor_id` VARCHAR(36) NOT NULL COMMENT '访客ID',
    `theme_code` VARCHAR(50) DEFAULT NULL COMMENT '主题代码',
    `background_effect_code` VARCHAR(50) DEFAULT NULL COMMENT '背景效果代码',
    `auto_switch` TINYINT DEFAULT 0 COMMENT '自动切换：0-手动 1-自动',
    `switch_interval` INT DEFAULT 0 COMMENT '切换间隔（分钟，0表示不自动切换）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_visitor_id` (`visitor_id`),
    INDEX `idx_theme_code` (`theme_code`),
    INDEX `idx_background_effect_code` (`background_effect_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户主题偏好表';

-- ============================================
-- 初始化默认主题风格
-- ============================================
INSERT INTO `theme_style` (`name`, `code`, `display_name`, `description`, `is_enabled`, `is_default`, `sort`, `style_config`) VALUES 
('默认风格', 'default', '默认风格', '经典的默认风格，简洁优雅', 1, 1, 1, '{"primaryColor": "#3b82f6", "bgColor": "#ffffff", "textColor": "#1f2937"}'),
('极简风', 'minimal', '极简风', '极简主义设计，留白丰富', 1, 0, 2, '{"primaryColor": "#000000", "bgColor": "#ffffff", "textColor": "#000000", "minimal": true}'),
('赛博朋克风', 'cyberpunk', '赛博朋克风', '霓虹灯效果，未来感十足', 1, 0, 3, '{"primaryColor": "#00ffff", "bgColor": "#0a0a0a", "textColor": "#00ffff", "neon": true}'),
('暗黑科技风', 'dark-tech', '暗黑科技风', '深色背景，科技感强', 1, 0, 4, '{"primaryColor": "#3b82f6", "bgColor": "#0f172a", "textColor": "#e2e8f0", "tech": true}'),
('卡通手绘风', 'cartoon', '卡通/手绘风', '可爱的手绘风格，轻松活泼', 1, 0, 5, '{"primaryColor": "#f59e0b", "bgColor": "#fef3c7", "textColor": "#78350f", "handdrawn": true}'),
('实验室3D风', 'lab-3d', '实验室 3D 风', '3D立体效果，实验室风格', 1, 0, 6, '{"primaryColor": "#8b5cf6", "bgColor": "#1e1b4b", "textColor": "#c7d2fe", "threeD": true}')
ON DUPLICATE KEY UPDATE `name`=`name`;

-- ============================================
-- 初始化动态背景效果
-- ============================================
INSERT INTO `background_effect` (`name`, `code`, `display_name`, `description`, `is_enabled`, `sort`, `effect_config`) VALUES 
('粒子流', 'particles', '粒子流背景', '动态粒子流动效果', 1, 1, '{"type": "particles", "count": 100, "speed": 1, "color": "#3b82f6"}'),
('噪声纹理', 'noise', '落点噪声纹理', '随机噪声纹理效果', 1, 2, '{"type": "noise", "intensity": 0.5, "speed": 0.1}'),
('波浪动画', 'waves', '波浪/山峦曲线动画', '流畅的波浪动画效果', 1, 3, '{"type": "waves", "amplitude": 50, "frequency": 0.02, "speed": 0.5}'),
('星空背景', 'stars', '顶部星空背景', '闪烁的星空效果', 1, 4, '{"type": "stars", "count": 200, "twinkleSpeed": 2}')
ON DUPLICATE KEY UPDATE `name`=`name`;

-- ============================================
-- 初始化主题设置
-- ============================================
INSERT INTO `theme_setting` (`setting_key`, `setting_value`, `description`) VALUES 
('allow_user_switch', 'true', '允许用户手动切换主题'),
('auto_switch_enabled', 'false', '是否启用自动切换'),
('auto_switch_interval', '30', '自动切换间隔（分钟）'),
('default_theme', 'default', '默认主题代码'),
('default_background', 'particles', '默认背景效果代码')
ON DUPLICATE KEY UPDATE `setting_key`=`setting_key`;

