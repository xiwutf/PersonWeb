-- ============================================
-- 商业化功能数据库表结构
-- ============================================

-- ============================================
-- 1. 主题商店相关表
-- ============================================

-- 主题商店表
CREATE TABLE IF NOT EXISTS `theme_store` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `name` VARCHAR(200) NOT NULL COMMENT '主题名称',
  `slug` VARCHAR(200) NOT NULL UNIQUE COMMENT '主题标识',
  `description` TEXT COMMENT '主题描述',
  `preview_image` VARCHAR(500) COMMENT '预览图',
  `preview_images` JSON COMMENT '多张预览图',
  `price` DECIMAL(10, 2) DEFAULT 0.00 COMMENT '价格（0表示免费）',
  `is_free` BOOLEAN DEFAULT FALSE COMMENT '是否免费',
  `category` VARCHAR(50) COMMENT '分类（dark/light/minimal/creative等）',
  `author_id` BIGINT COMMENT '作者ID',
  `author_name` VARCHAR(100) COMMENT '作者名称',
  `download_count` INT DEFAULT 0 COMMENT '下载次数',
  `purchase_count` INT DEFAULT 0 COMMENT '购买次数',
  `rating` DECIMAL(3, 2) DEFAULT 0.00 COMMENT '评分（0-5）',
  `rating_count` INT DEFAULT 0 COMMENT '评分人数',
  `status` VARCHAR(20) DEFAULT 'draft' COMMENT '状态（draft/published/archived）',
  `tags` JSON COMMENT '标签',
  `features` JSON COMMENT '特性列表',
  `version` VARCHAR(20) DEFAULT '1.0.0' COMMENT '版本号',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_category` (`category`),
  INDEX `idx_status` (`status`),
  INDEX `idx_price` (`price`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='主题商店表';

-- 主题文件表
CREATE TABLE IF NOT EXISTS `theme_files` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `theme_id` BIGINT NOT NULL COMMENT '主题ID',
  `file_type` VARCHAR(50) NOT NULL COMMENT '文件类型（css/js/config等）',
  `file_path` VARCHAR(500) NOT NULL COMMENT '文件路径',
  `file_size` BIGINT COMMENT '文件大小（字节）',
  `version` VARCHAR(20) DEFAULT '1.0.0' COMMENT '版本号',
  `checksum` VARCHAR(64) COMMENT '文件校验和',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`theme_id`) REFERENCES `theme_store`(`id`) ON DELETE CASCADE,
  INDEX `idx_theme_id` (`theme_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='主题文件表';

-- 主题购买记录表
CREATE TABLE IF NOT EXISTS `theme_purchases` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `user_id` VARCHAR(100) NOT NULL COMMENT '用户ID',
  `theme_id` BIGINT NOT NULL COMMENT '主题ID',
  `purchase_type` VARCHAR(20) DEFAULT 'one_time' COMMENT '购买类型（one_time/subscription）',
  `price` DECIMAL(10, 2) NOT NULL COMMENT '购买价格',
  `payment_method` VARCHAR(50) COMMENT '支付方式',
  `payment_status` VARCHAR(20) DEFAULT 'pending' COMMENT '支付状态（pending/paid/failed/refunded）',
  `payment_id` VARCHAR(200) COMMENT '支付订单号',
  `expires_at` DATETIME COMMENT '过期时间（订阅类型）',
  `is_active` BOOLEAN DEFAULT TRUE COMMENT '是否激活',
  `purchased_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`theme_id`) REFERENCES `theme_store`(`id`),
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_theme_id` (`theme_id`),
  INDEX `idx_payment_status` (`payment_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='主题购买记录表';

-- ============================================
-- 2. API Key 管理系统相关表
-- ============================================

-- API 用户表
CREATE TABLE IF NOT EXISTS `api_users` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `email` VARCHAR(200) NOT NULL UNIQUE COMMENT '邮箱',
  `password_hash` VARCHAR(255) NOT NULL COMMENT '密码哈希',
  `name` VARCHAR(100) COMMENT '用户名',
  `plan_type` VARCHAR(50) DEFAULT 'free' COMMENT '套餐类型（free/basic/pro/enterprise）',
  `status` VARCHAR(20) DEFAULT 'active' COMMENT '状态（active/suspended/cancelled）',
  `total_calls` BIGINT DEFAULT 0 COMMENT '总调用次数',
  `free_calls_used` BIGINT DEFAULT 0 COMMENT '已使用免费额度',
  `paid_calls` BIGINT DEFAULT 0 COMMENT '付费调用次数',
  `last_call_at` DATETIME COMMENT '最后调用时间',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_email` (`email`),
  INDEX `idx_plan_type` (`plan_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='API用户表';

-- API Key 表
CREATE TABLE IF NOT EXISTS `api_keys` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL COMMENT '用户ID',
  `api_key` VARCHAR(100) NOT NULL UNIQUE COMMENT 'API Key',
  `api_secret` VARCHAR(255) NOT NULL COMMENT 'API Secret（加密存储）',
  `name` VARCHAR(100) COMMENT 'Key名称',
  `rate_limit` INT DEFAULT 100 COMMENT '速率限制（次/分钟）',
  `daily_limit` INT DEFAULT 10000 COMMENT '每日限制',
  `expires_at` DATETIME COMMENT '过期时间',
  `is_active` BOOLEAN DEFAULT TRUE COMMENT '是否激活',
  `last_used_at` DATETIME COMMENT '最后使用时间',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`user_id`) REFERENCES `api_users`(`id`) ON DELETE CASCADE,
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_api_key` (`api_key`),
  INDEX `idx_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='API Key表';

-- API 调用记录表
CREATE TABLE IF NOT EXISTS `api_calls` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `api_key_id` BIGINT NOT NULL COMMENT 'API Key ID',
  `user_id` BIGINT COMMENT '用户ID',
  `endpoint` VARCHAR(200) NOT NULL COMMENT 'API端点',
  `method` VARCHAR(10) DEFAULT 'POST' COMMENT 'HTTP方法',
  `status_code` INT COMMENT '响应状态码',
  `response_time` INT COMMENT '响应时间（毫秒）',
  `request_size` INT COMMENT '请求大小（字节）',
  `response_size` INT COMMENT '响应大小（字节）',
  `cost` DECIMAL(10, 6) DEFAULT 0 COMMENT '调用成本',
  `ip_address` VARCHAR(50) COMMENT 'IP地址',
  `user_agent` VARCHAR(500) COMMENT 'User Agent',
  `called_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`api_key_id`) REFERENCES `api_keys`(`id`),
  INDEX `idx_api_key_id` (`api_key_id`),
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_called_at` (`called_at`),
  INDEX `idx_endpoint` (`endpoint`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='API调用记录表';

-- API 计费记录表
CREATE TABLE IF NOT EXISTS `api_billing` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL COMMENT '用户ID',
  `period` VARCHAR(20) NOT NULL COMMENT '计费周期（2024-12）',
  `total_calls` BIGINT DEFAULT 0 COMMENT '总调用次数',
  `free_calls` BIGINT DEFAULT 0 COMMENT '免费调用次数',
  `paid_calls` BIGINT DEFAULT 0 COMMENT '付费调用次数',
  `amount` DECIMAL(10, 2) DEFAULT 0.00 COMMENT '费用',
  `status` VARCHAR(20) DEFAULT 'pending' COMMENT '状态（pending/paid/overdue）',
  `paid_at` DATETIME COMMENT '支付时间',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`user_id`) REFERENCES `api_users`(`id`),
  UNIQUE KEY `uk_user_period` (`user_id`, `period`),
  INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='API计费记录表';

-- ============================================
-- 3. 付费文章/会员专区相关表
-- ============================================

-- 会员类型表
CREATE TABLE IF NOT EXISTS `membership_types` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL COMMENT '会员名称',
  `code` VARCHAR(50) NOT NULL UNIQUE COMMENT '会员代码',
  `price` DECIMAL(10, 2) NOT NULL COMMENT '价格',
  `duration_days` INT COMMENT '时长（天，NULL表示永久）',
  `features` JSON COMMENT '功能特性',
  `description` TEXT COMMENT '描述',
  `is_active` BOOLEAN DEFAULT TRUE COMMENT '是否启用',
  `sort_order` INT DEFAULT 0 COMMENT '排序',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_code` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员类型表';

-- 用户会员表
CREATE TABLE IF NOT EXISTS `user_memberships` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `user_id` VARCHAR(100) NOT NULL COMMENT '用户ID',
  `membership_type_id` BIGINT NOT NULL COMMENT '会员类型ID',
  `start_date` DATETIME NOT NULL COMMENT '开始日期',
  `end_date` DATETIME COMMENT '结束日期（NULL表示永久）',
  `status` VARCHAR(20) DEFAULT 'active' COMMENT '状态（active/expired/cancelled）',
  `auto_renew` BOOLEAN DEFAULT FALSE COMMENT '自动续费',
  `purchased_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '购买时间',
  FOREIGN KEY (`membership_type_id`) REFERENCES `membership_types`(`id`),
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_end_date` (`end_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户会员表';

-- 付费内容表
CREATE TABLE IF NOT EXISTS `paid_content` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `content_type` VARCHAR(50) NOT NULL COMMENT '内容类型（article/video/tool/resource）',
  `content_id` BIGINT NOT NULL COMMENT '内容ID',
  `title` VARCHAR(200) NOT NULL COMMENT '标题',
  `price` DECIMAL(10, 2) DEFAULT 0.00 COMMENT '价格（0表示免费）',
  `is_member_only` BOOLEAN DEFAULT FALSE COMMENT '仅会员可访问',
  `required_membership` VARCHAR(50) COMMENT '所需会员类型',
  `unlock_requirements` JSON COMMENT '解锁要求',
  `preview_content` TEXT COMMENT '预览内容',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_content` (`content_type`, `content_id`),
  INDEX `idx_price` (`price`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='付费内容表';

-- 内容购买记录表
CREATE TABLE IF NOT EXISTS `content_purchases` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `user_id` VARCHAR(100) NOT NULL COMMENT '用户ID',
  `content_id` BIGINT NOT NULL COMMENT '内容ID',
  `content_type` VARCHAR(50) NOT NULL COMMENT '内容类型',
  `price` DECIMAL(10, 2) NOT NULL COMMENT '购买价格',
  `purchase_type` VARCHAR(20) DEFAULT 'one_time' COMMENT '购买类型（one_time/membership）',
  `payment_method` VARCHAR(50) COMMENT '支付方式',
  `payment_status` VARCHAR(20) DEFAULT 'pending' COMMENT '支付状态',
  `payment_id` VARCHAR(200) COMMENT '支付订单号',
  `purchased_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_content` (`content_type`, `content_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='内容购买记录表';

-- ============================================
-- 4. PageBuilder SaaS 相关表
-- ============================================

-- 用户页面表
CREATE TABLE IF NOT EXISTS `user_pages` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `user_id` VARCHAR(100) NOT NULL COMMENT '用户ID',
  `name` VARCHAR(200) NOT NULL COMMENT '页面名称',
  `slug` VARCHAR(200) NOT NULL COMMENT '页面标识',
  `domain` VARCHAR(200) COMMENT '自定义域名',
  `template_id` BIGINT COMMENT '模板ID',
  `layout_config` JSON COMMENT '布局配置',
  `seo_config` JSON COMMENT 'SEO配置',
  `status` VARCHAR(20) DEFAULT 'draft' COMMENT '状态（draft/published/archived）',
  `published_at` DATETIME COMMENT '发布时间',
  `view_count` INT DEFAULT 0 COMMENT '访问次数',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY `uk_user_slug` (`user_id`, `slug`),
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户页面表';

-- 页面组件表
CREATE TABLE IF NOT EXISTS `page_components` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `page_id` BIGINT NOT NULL COMMENT '页面ID',
  `component_type` VARCHAR(100) NOT NULL COMMENT '组件类型',
  `component_id` VARCHAR(100) COMMENT '组件ID（用于拖拽）',
  `position` INT DEFAULT 0 COMMENT '位置顺序',
  `config` JSON COMMENT '组件配置',
  `style` JSON COMMENT '样式配置',
  `parent_id` BIGINT COMMENT '父组件ID',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (`page_id`) REFERENCES `user_pages`(`id`) ON DELETE CASCADE,
  INDEX `idx_page_id` (`page_id`),
  INDEX `idx_position` (`position`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='页面组件表';

-- 组件库表
CREATE TABLE IF NOT EXISTS `component_library` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `name` VARCHAR(200) NOT NULL COMMENT '组件名称',
  `type` VARCHAR(100) NOT NULL COMMENT '组件类型',
  `category` VARCHAR(50) COMMENT '分类',
  `config_schema` JSON COMMENT '配置模式',
  `preview_image` VARCHAR(500) COMMENT '预览图',
  `is_premium` BOOLEAN DEFAULT FALSE COMMENT '是否付费',
  `price` DECIMAL(10, 2) DEFAULT 0.00 COMMENT '价格',
  `usage_count` INT DEFAULT 0 COMMENT '使用次数',
  `is_active` BOOLEAN DEFAULT TRUE COMMENT '是否启用',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_category` (`category`),
  INDEX `idx_type` (`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='组件库表';

-- 页面模板表
CREATE TABLE IF NOT EXISTS `page_templates` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `name` VARCHAR(200) NOT NULL COMMENT '模板名称',
  `category` VARCHAR(50) COMMENT '分类',
  `description` TEXT COMMENT '描述',
  `preview_image` VARCHAR(500) COMMENT '预览图',
  `components` JSON COMMENT '组件配置',
  `is_premium` BOOLEAN DEFAULT FALSE COMMENT '是否付费',
  `price` DECIMAL(10, 2) DEFAULT 0.00 COMMENT '价格',
  `usage_count` INT DEFAULT 0 COMMENT '使用次数',
  `is_active` BOOLEAN DEFAULT TRUE COMMENT '是否启用',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='页面模板表';

-- ============================================
-- 初始化数据
-- ============================================

-- 插入默认会员类型
INSERT INTO `membership_types` (`name`, `code`, `price`, `duration_days`, `features`, `description`) VALUES
('月度会员', 'monthly', 29.90, 30, '["unlock_articles", "unlock_videos", "ai_tools_1000"]', '月度会员，解锁所有付费内容'),
('年度会员', 'yearly', 299.00, 365, '["unlock_articles", "unlock_videos", "ai_tools_unlimited", "exclusive_resources"]', '年度会员，享受更多特权'),
('VIP会员', 'vip', 599.00, 365, '["unlock_all", "priority_support", "custom_theme", "api_access"]', 'VIP会员，解锁所有功能');

-- 插入默认组件库
INSERT INTO `component_library` (`name`, `type`, `category`, `is_premium`, `price`) VALUES
('标题组件', 'heading', 'basic', FALSE, 0.00),
('文本组件', 'text', 'basic', FALSE, 0.00),
('图片组件', 'image', 'basic', FALSE, 0.00),
('按钮组件', 'button', 'basic', FALSE, 0.00),
('视频组件', 'video', 'media', TRUE, 9.90),
('轮播图组件', 'carousel', 'media', TRUE, 19.90),
('表单组件', 'form', 'interactive', TRUE, 29.90);

