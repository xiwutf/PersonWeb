-- 友情链接表
CREATE TABLE IF NOT EXISTS `friend_links` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `name` VARCHAR(100) NOT NULL COMMENT '链接名称',
  `url` VARCHAR(500) NOT NULL COMMENT '链接地址',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '链接描述',
  `logo_url` VARCHAR(500) DEFAULT NULL COMMENT 'Logo 地址',
  `sort_order` INT DEFAULT 0 COMMENT '排序顺序（数字越小越靠前）',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用 1-启用',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_status` (`status`),
  INDEX `idx_sort_order` (`sort_order`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='友情链接表';

