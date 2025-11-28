-- ============================================
-- HomeStyle 表：首页风格管理
-- ============================================
CREATE TABLE IF NOT EXISTS `home_style` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '主键',
    `style_key` VARCHAR(50) NOT NULL COMMENT '风格标识（如：dark-lab、light-portfolio）',
    `name` VARCHAR(100) NOT NULL COMMENT '风格名称（中文名）',
    `description` VARCHAR(500) DEFAULT NULL COMMENT '风格描述',
    `preview_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图URL',
    `enabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否可被使用：0-禁用 1-启用',
    `is_default` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否是当前启用风格：0-否 1-是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_style_key` (`style_key`),
    INDEX `idx_enabled` (`enabled`),
    INDEX `idx_is_default` (`is_default`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='首页风格表';

-- ============================================
-- 初始化默认风格数据
-- ============================================
INSERT INTO `home_style` (`style_key`, `name`, `description`, `preview_image`, `enabled`, `is_default`) VALUES
('dark-lab', '深色实验室', '深色科技风格，适合展示技术内容', '/images/home-styles/dark-lab.jpg', 1, 1),
('light-portfolio', '浅色作品集', '简洁明亮的作品集风格，适合展示项目', '/images/home-styles/light-portfolio.jpg', 1, 0)
ON DUPLICATE KEY UPDATE `name`=`name`;

-- ============================================
-- 在 SiteConfig 中设置默认首页风格
-- ============================================
INSERT INTO `site_config` (`config_key`, `config_value`, `description`) VALUES
('homeStyle', 'dark-lab', '当前启用的首页风格')
ON DUPLICATE KEY UPDATE `config_value`='dark-lab', `description`='当前启用的首页风格';

