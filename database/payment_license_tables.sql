-- 支付和许可证系统数据库表结构
-- 用于管理模块销售、订单、许可证等商业化功能

-- ============================================
-- 1. orders 表 - 订单信息
-- ============================================
CREATE TABLE IF NOT EXISTS `order` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '订单ID',
    `order_number` VARCHAR(50) NOT NULL COMMENT '订单编号',
    `user_id` BIGINT NOT NULL COMMENT '用户ID',
    `module_key` VARCHAR(50) NOT NULL COMMENT '模块标识',
    `version` VARCHAR(20) NOT NULL COMMENT '购买的版本',
    `type` ENUM('module', 'subscription', 'trial') NOT NULL COMMENT '购买类型',
    `price` DECIMAL(10,2) NOT NULL COMMENT '订单金额',
    `currency` VARCHAR(3) DEFAULT 'CNY' COMMENT '货币',
    `status` ENUM('pending', 'paid', 'cancelled', 'refunded', 'expired') DEFAULT 'pending' COMMENT '订单状态',
    `payment_method` VARCHAR(50) DEFAULT NULL COMMENT '支付方式',
    `payment_gateway` VARCHAR(50) DEFAULT NULL COMMENT '支付网关',
    `transaction_id` VARCHAR(100) DEFAULT NULL COMMENT '交易ID',
    `license_key` VARCHAR(100) DEFAULT NULL COMMENT '许可证密钥',
    `valid_from` DATETIME DEFAULT NULL COMMENT '有效期开始',
    `valid_until` DATETIME DEFAULT NULL COMMENT '有效期结束',
    `max_activations` INT DEFAULT 1 COMMENT '最大激活次数',
    `activations_used` INT DEFAULT 0 COMMENT '已使用激活次数',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_order_number` (`order_number`),
    UNIQUE KEY `uk_license_key` (`license_key`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_module_key` (`module_key`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='订单表';

-- ============================================
-- 2. licenses 表 - 许可证管理
-- ============================================
CREATE TABLE IF NOT EXISTS `license` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '许可证ID',
    `license_key` VARCHAR(100) NOT NULL COMMENT '许可证密钥',
    `order_id` BIGINT NOT NULL COMMENT '关联的订单ID',
    `module_key` VARCHAR(50) NOT NULL COMMENT '模块标识',
    `user_id` BIGINT NOT NULL COMMENT '用户ID',
    `type` ENUM('permanent', 'subscription', 'trial') NOT NULL COMMENT '许可证类型',
    `valid_from` DATETIME NOT NULL COMMENT '有效期开始',
    `valid_until` DATETIME DEFAULT NULL COMMENT '有效期结束',
    `max_activations` INT DEFAULT 1 COMMENT '最大激活次数',
    `activations_used` INT DEFAULT 0 COMMENT '已使用激活次数',
    `status` ENUM('active', 'expired', 'revoked') DEFAULT 'active' COMMENT '许可证状态',
    `device_fingerprint` VARCHAR(255) DEFAULT NULL COMMENT '设备指纹',
    `last_activated_at` DATETIME DEFAULT NULL COMMENT '最后激活时间',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_license_key` (`license_key`),
    INDEX `idx_order_id` (`order_id`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_module_key` (`module_key`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='许可证表';

-- ============================================
-- 3. license_activations 表 - 许可证激活记录
-- ============================================
CREATE TABLE IF NOT EXISTS `license_activation` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '激活记录ID',
    `license_key` VARCHAR(100) NOT NULL COMMENT '许可证密钥',
    `device_id` VARCHAR(100) NOT NULL COMMENT '设备ID',
    `device_name` VARCHAR(255) DEFAULT NULL COMMENT '设备名称',
    `ip_address` VARCHAR(45) DEFAULT NULL COMMENT 'IP地址',
    `user_agent` TEXT DEFAULT NULL COMMENT '用户代理',
    `activated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '激活时间',
    PRIMARY KEY (`id`),
    INDEX `idx_license_key` (`license_key`),
    INDEX `idx_device_id` (`device_id`),
    INDEX `idx_activated_at` (`activated_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='许可证激活记录表';

-- ============================================
-- 4. payment_transactions 表 - 支付交易记录
-- ============================================
CREATE TABLE IF NOT EXISTS `payment_transaction` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '交易ID',
    `order_id` BIGINT NOT NULL COMMENT '订单ID',
    `gateway` VARCHAR(50) NOT NULL COMMENT '支付网关',
    `gateway_transaction_id` VARCHAR(100) NOT NULL COMMENT '网关交易ID',
    `amount` DECIMAL(10,2) NOT NULL COMMENT '交易金额',
    `currency` VARCHAR(3) DEFAULT 'CNY' COMMENT '货币',
    `status` ENUM('pending', 'success', 'failed', 'cancelled') NOT NULL COMMENT '交易状态',
    `response_data` JSON DEFAULT NULL COMMENT '网关响应数据',
    `error_message` TEXT DEFAULT NULL COMMENT '错误信息',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_gateway_transaction_id` (`gateway`, `gateway_transaction_id`),
    INDEX `idx_order_id` (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='支付交易记录表';

-- ============================================
-- 5. refunds 表 - 退款记录
-- ============================================
CREATE TABLE IF NOT EXISTS `refund` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '退款ID',
    `order_id` BIGINT NOT NULL COMMENT '订单ID',
    `transaction_id` VARCHAR(100) NOT NULL COMMENT '退款交易ID',
    `amount` DECIMAL(10,2) NOT NULL COMMENT '退款金额',
    `reason` TEXT DEFAULT NULL COMMENT '退款原因',
    `status` ENUM('pending', 'success', 'failed') DEFAULT 'pending' COMMENT '退款状态',
    `processed_at` DATETIME DEFAULT NULL COMMENT '处理时间',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`id`),
    INDEX `idx_order_id` (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='退款记录表';

-- ============================================
-- 6. subscription_plans 表 - 订阅计划
-- ============================================
CREATE TABLE IF NOT EXISTS `subscription_plan` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '计划ID',
    `name` VARCHAR(100) NOT NULL COMMENT '计划名称',
    `key` VARCHAR(50) NOT NULL COMMENT '计划标识',
    `description` TEXT DEFAULT NULL COMMENT '计划描述',
    `price_monthly` DECIMAL(10,2) DEFAULT NULL COMMENT '月费价格',
    `price_yearly` DECIMAL(10,2) DEFAULT NULL COMMENT '年费价格',
    `modules_included` JSON DEFAULT NULL COMMENT '包含的模块',
    `features` JSON DEFAULT NULL COMMENT '功能特性',
    `max_downloads` INT DEFAULT 100 COMMENT '最大下载次数',
    `support_level` ENUM('basic', 'standard', 'premium') DEFAULT 'basic' COMMENT '支持级别',
    `trial_days` INT DEFAULT 0 COMMENT '试用期天数',
    `is_active` TINYINT DEFAULT 1 COMMENT '是否启用',
    `sort_order` INT DEFAULT 0 COMMENT '排序',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_plan_key` (`key`),
    INDEX `idx_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='订阅计划表';

-- ============================================
-- 7. user_subscriptions 表 - 用户订阅
-- ============================================
CREATE TABLE IF NOT EXISTS `user_subscription` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '订阅ID',
    `user_id` BIGINT NOT NULL COMMENT '用户ID',
    `plan_key` VARCHAR(50) NOT NULL COMMENT '计划标识',
    `status` ENUM('active', 'cancelled', 'expired') DEFAULT 'active' COMMENT '订阅状态',
    `billing_cycle` ENUM('monthly', 'yearly') NOT NULL COMMENT '计费周期',
    `current_period_start` DATETIME NOT NULL COMMENT '当前周期开始',
    `current_period_end` DATETIME NOT NULL COMMENT '当前周期结束',
    `cancelled_at` DATETIME DEFAULT NULL COMMENT '取消时间',
    `expires_at` DATETIME DEFAULT NULL COMMENT '过期时间',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_user_plan` (`user_id`, `plan_key`, `status`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户订阅表';

-- ============================================
-- 8. coupons 表 - 优惠码
-- ============================================
CREATE TABLE IF NOT EXISTS `coupon` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '优惠码ID',
    `code` VARCHAR(50) NOT NULL COMMENT '优惠码',
    `type` ENUM('fixed', 'percentage') NOT NULL COMMENT '优惠类型',
    `value` DECIMAL(10,2) NOT NULL COMMENT '优惠值',
    `max_uses` INT DEFAULT NULL COMMENT '最大使用次数',
    `uses` INT DEFAULT 0 COMMENT '已使用次数',
    `valid_from` DATETIME DEFAULT NULL COMMENT '生效时间',
    `valid_until` DATETIME DEFAULT NULL COMMENT '失效时间',
    `module_key` VARCHAR(50) DEFAULT NULL COMMENT '适用模块（null表示全站）',
    `min_amount` DECIMAL(10,2) DEFAULT NULL COMMENT '最小订单金额',
    `description` TEXT DEFAULT NULL COMMENT '描述',
    `is_active` TINYINT DEFAULT 1 COMMENT '是否启用',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_coupon_code` (`code`),
    INDEX `idx_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='优惠码表';

-- ============================================
-- 初始化数据 - 添加默认订阅计划和优惠码
-- ============================================
INSERT IGNORE INTO `subscription_plan` (
    `name`, `key`, `description`, `price_monthly`, `price_yearly`,
    `max_downloads`, `support_level`, `trial_days`, `sort_order`
) VALUES
('基础版', 'basic', '适合个人开发者，包含基础模块', 29.00, 290.00, 100, 'basic', 7, 1),
('专业版', 'professional', '适合小型团队，包含所有模块', 99.00, 990.00, 1000, 'standard', 14, 2),
('企业版', 'enterprise', '适合大型企业，定制化服务', 299.00, 2990.00, 10000, 'premium', 30, 3);

INSERT IGNORE INTO `coupon` (
    `code`, `type`, `value`, `max_uses`, `valid_until`, `description`
) VALUES
('WELCOME10', 'percentage', 10.00, 100, DATE_ADD(NOW(), INTERVAL 1 YEAR), '新用户优惠10%'),
('LAUNCH20', 'fixed', 20.00, 500, DATE_ADD(NOW(), INTERVAL 6 MONTH), '首发优惠20元');