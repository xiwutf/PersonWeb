-- 工具商城系统数据库表结构
-- 
-- 设计原则：
-- 1. 不使用外键约束，通过逻辑关联维护表间关系
-- 2. 关联关系由应用层维护，便于后期维护和扩展
-- 3. 为关联字段创建索引以提升查询性能
-- 4. 详细说明请参考 database/DESIGN_PRINCIPLES.md

-- ============================================
-- 1. ToolCategory 表 - 工具分类
-- ============================================
CREATE TABLE IF NOT EXISTS `tool_category` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '分类ID',
    `name` VARCHAR(100) NOT NULL COMMENT '分类名称',
    `slug` VARCHAR(100) NOT NULL COMMENT '分类标识（URL友好）',
    `icon` VARCHAR(50) DEFAULT NULL COMMENT '分类图标',
    `description` TEXT DEFAULT NULL COMMENT '分类描述',
    `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
    `is_active` TINYINT(1) DEFAULT 1 COMMENT '是否启用',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_slug` (`slug`),
    INDEX `idx_is_active` (`is_active`),
    INDEX `idx_sort_order` (`sort_order`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工具分类表';

-- ============================================
-- 2. Tool 表 - 工具主表
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- category_id 通过逻辑关联到 tool_category.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `tool` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '工具ID',
    `category_id` BIGINT DEFAULT NULL COMMENT '分类ID（逻辑关联到 tool_category.id）',
    `name` VARCHAR(200) NOT NULL COMMENT '工具名称',
    `slug` VARCHAR(200) NOT NULL COMMENT '工具标识（URL友好）',
    `icon` VARCHAR(50) DEFAULT NULL COMMENT '工具图标',
    `description` TEXT DEFAULT NULL COMMENT '工具描述',
    `detailed_description` LONGTEXT DEFAULT NULL COMMENT '详细描述（Markdown格式）',
    `cover_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图片URL',
    `demo_url` VARCHAR(500) DEFAULT NULL COMMENT '演示地址',
    `price` DECIMAL(10,2) DEFAULT 0.00 COMMENT '价格（元）',
    `original_price` DECIMAL(10,2) DEFAULT NULL COMMENT '原价（用于显示折扣）',
    `is_free` TINYINT(1) DEFAULT 0 COMMENT '是否免费',
    `is_premium` TINYINT(1) DEFAULT 0 COMMENT '是否高级工具',
    `purchase_count` INT DEFAULT 0 COMMENT '购买次数',
    `use_count` INT DEFAULT 0 COMMENT '使用次数',
    `rating` DECIMAL(3,2) DEFAULT 0.00 COMMENT '评分（0-5）',
    `rating_count` INT DEFAULT 0 COMMENT '评分人数',
    `status` VARCHAR(20) DEFAULT 'draft' COMMENT '状态：draft-草稿, published-已发布, archived-已归档',
    `api_endpoint` VARCHAR(500) DEFAULT NULL COMMENT 'API接口地址',
    `api_documentation` TEXT DEFAULT NULL COMMENT 'API文档（JSON格式）',
    `tags` JSON DEFAULT NULL COMMENT '标签（JSON数组）',
    `features` JSON DEFAULT NULL COMMENT '功能特性（JSON数组）',
    `requirements` TEXT DEFAULT NULL COMMENT '使用要求',
    `version` VARCHAR(50) DEFAULT '1.0.0' COMMENT '版本号',
    `author` VARCHAR(100) DEFAULT NULL COMMENT '作者',
    `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `published_at` DATETIME DEFAULT NULL COMMENT '发布时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_slug` (`slug`),
    INDEX `idx_category_id` (`category_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_is_free` (`is_free`),
    INDEX `idx_is_premium` (`is_premium`),
    INDEX `idx_purchase_count` (`purchase_count`),
    INDEX `idx_rating` (`rating`)
    -- 注意：不使用外键约束，category_id 通过逻辑关联到 tool_category.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工具表';

-- ============================================
-- 3. ToolPurchase 表 - 工具购买记录
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- tool_id 通过逻辑关联到 tool.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `tool_purchase` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '购买记录ID',
    `tool_id` BIGINT NOT NULL COMMENT '工具ID（逻辑关联到 tool.id）',
    `user_id` VARCHAR(100) DEFAULT NULL COMMENT '用户ID（访客ID或管理员ID）',
    `purchase_type` VARCHAR(20) DEFAULT 'one_time' COMMENT '购买类型：one_time-一次性, subscription-订阅, lifetime-终身',
    `price` DECIMAL(10,2) NOT NULL COMMENT '购买价格',
    `payment_method` VARCHAR(50) DEFAULT NULL COMMENT '支付方式',
    `payment_status` VARCHAR(20) DEFAULT 'pending' COMMENT '支付状态：pending-待支付, paid-已支付, refunded-已退款',
    `expires_at` DATETIME DEFAULT NULL COMMENT '过期时间（订阅类型）',
    `is_active` TINYINT(1) DEFAULT 1 COMMENT '是否有效',
    `purchased_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '购买时间',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`id`),
    INDEX `idx_tool_id` (`tool_id`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_payment_status` (`payment_status`),
    INDEX `idx_is_active` (`is_active`),
    INDEX `idx_purchased_at` (`purchased_at`)
    -- 注意：不使用外键约束，tool_id 通过逻辑关联到 tool.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工具购买记录表';

-- ============================================
-- 4. ToolUsage 表 - 工具使用记录
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- tool_id 通过逻辑关联到 tool.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `tool_usage` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '使用记录ID',
    `tool_id` BIGINT NOT NULL COMMENT '工具ID（逻辑关联到 tool.id）',
    `user_id` VARCHAR(100) DEFAULT NULL COMMENT '用户ID',
    `usage_type` VARCHAR(50) DEFAULT 'web' COMMENT '使用类型：web-网页, api-API调用, embed-嵌入',
    `request_data` JSON DEFAULT NULL COMMENT '请求数据（JSON格式）',
    `response_data` JSON DEFAULT NULL COMMENT '响应数据（JSON格式，可选）',
    `execution_time` INT DEFAULT NULL COMMENT '执行时间（毫秒）',
    `status` VARCHAR(20) DEFAULT 'success' COMMENT '状态：success-成功, error-失败, timeout-超时',
    `error_message` TEXT DEFAULT NULL COMMENT '错误信息',
    `ip_address` VARCHAR(50) DEFAULT NULL COMMENT 'IP地址',
    `user_agent` VARCHAR(500) DEFAULT NULL COMMENT 'User Agent',
    `referrer` VARCHAR(500) DEFAULT NULL COMMENT '来源页面',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '使用时间',
    PRIMARY KEY (`id`),
    INDEX `idx_tool_id` (`tool_id`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_usage_type` (`usage_type`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
    -- 注意：不使用外键约束，tool_id 通过逻辑关联到 tool.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工具使用记录表';

-- ============================================
-- 5. ToolApiKey 表 - 工具API密钥
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- tool_id 通过逻辑关联到 tool.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `tool_api_key` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'API密钥ID',
    `tool_id` BIGINT NOT NULL COMMENT '工具ID（逻辑关联到 tool.id）',
    `user_id` VARCHAR(100) NOT NULL COMMENT '用户ID',
    `api_key` VARCHAR(100) NOT NULL COMMENT 'API密钥（加密存储）',
    `key_name` VARCHAR(100) DEFAULT NULL COMMENT '密钥名称（用户自定义）',
    `rate_limit` INT DEFAULT 1000 COMMENT '速率限制（每小时请求数）',
    `usage_count` INT DEFAULT 0 COMMENT '使用次数',
    `last_used_at` DATETIME DEFAULT NULL COMMENT '最后使用时间',
    `expires_at` DATETIME DEFAULT NULL COMMENT '过期时间',
    `is_active` TINYINT(1) DEFAULT 1 COMMENT '是否启用',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_api_key` (`api_key`),
    INDEX `idx_tool_id` (`tool_id`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_is_active` (`is_active`)
    -- 注意：不使用外键约束，tool_id 通过逻辑关联到 tool.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工具API密钥表';

-- ============================================
-- 6. ToolCollection 表 - 工具合集
-- ============================================
CREATE TABLE IF NOT EXISTS `tool_collection` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '合集ID',
    `name` VARCHAR(200) NOT NULL COMMENT '合集名称',
    `slug` VARCHAR(200) NOT NULL COMMENT '合集标识',
    `description` TEXT DEFAULT NULL COMMENT '合集描述',
    `cover_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图片',
    `price` DECIMAL(10,2) DEFAULT 0.00 COMMENT '合集价格（打包价）',
    `original_price` DECIMAL(10,2) DEFAULT NULL COMMENT '原价（单独购买总价）',
    `tool_count` INT DEFAULT 0 COMMENT '包含工具数量',
    `purchase_count` INT DEFAULT 0 COMMENT '购买次数',
    `is_featured` TINYINT(1) DEFAULT 0 COMMENT '是否推荐',
    `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
    `status` VARCHAR(20) DEFAULT 'draft' COMMENT '状态：draft-草稿, published-已发布',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_slug` (`slug`),
    INDEX `idx_is_featured` (`is_featured`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工具合集表';

-- ============================================
-- 7. ToolCollectionItem 表 - 工具合集关联表
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- collection_id 通过逻辑关联到 tool_collection.id
-- tool_id 通过逻辑关联到 tool.id
-- 关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `tool_collection_item` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '关联ID',
    `collection_id` BIGINT NOT NULL COMMENT '合集ID（逻辑关联到 tool_collection.id）',
    `tool_id` BIGINT NOT NULL COMMENT '工具ID（逻辑关联到 tool.id）',
    `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_collection_tool` (`collection_id`, `tool_id`),
    INDEX `idx_collection_id` (`collection_id`),
    INDEX `idx_tool_id` (`tool_id`)
    -- 注意：不使用外键约束
    -- collection_id 通过逻辑关联到 tool_collection.id
    -- tool_id 通过逻辑关联到 tool.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工具合集关联表';

-- ============================================
-- 8. ToolReview 表 - 工具评价
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- tool_id 通过逻辑关联到 tool.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `tool_review` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '评价ID',
    `tool_id` BIGINT NOT NULL COMMENT '工具ID（逻辑关联到 tool.id）',
    `user_id` VARCHAR(100) NOT NULL COMMENT '用户ID',
    `rating` INT NOT NULL COMMENT '评分（1-5）',
    `comment` TEXT DEFAULT NULL COMMENT '评价内容',
    `is_verified_purchase` TINYINT(1) DEFAULT 0 COMMENT '是否已验证购买',
    `helpful_count` INT DEFAULT 0 COMMENT '有用数',
    `status` VARCHAR(20) DEFAULT 'pending' COMMENT '状态：pending-待审核, approved-已通过, rejected-已拒绝',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_tool_id` (`tool_id`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_rating` (`rating`),
    INDEX `idx_status` (`status`)
    -- 注意：不使用外键约束，tool_id 通过逻辑关联到 tool.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工具评价表';

-- ============================================
-- 9. ToolAnalytics 表 - 工具分析统计（汇总表）
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- tool_id 通过逻辑关联到 tool.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `tool_analytics` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '统计ID',
    `tool_id` BIGINT NOT NULL COMMENT '工具ID（逻辑关联到 tool.id）',
    `date` DATE NOT NULL COMMENT '统计日期',
    `view_count` INT DEFAULT 0 COMMENT '查看次数',
    `use_count` INT DEFAULT 0 COMMENT '使用次数',
    `api_call_count` INT DEFAULT 0 COMMENT 'API调用次数',
    `purchase_count` INT DEFAULT 0 COMMENT '购买次数',
    `revenue` DECIMAL(10,2) DEFAULT 0.00 COMMENT '收入（元）',
    `avg_execution_time` INT DEFAULT NULL COMMENT '平均执行时间（毫秒）',
    `error_count` INT DEFAULT 0 COMMENT '错误次数',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_tool_date` (`tool_id`, `date`),
    INDEX `idx_tool_id` (`tool_id`),
    INDEX `idx_date` (`date`)
    -- 注意：不使用外键约束，tool_id 通过逻辑关联到 tool.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工具分析统计表';

-- ============================================
-- 初始化数据：工具分类
-- ============================================
INSERT INTO `tool_category` (`name`, `slug`, `icon`, `description`, `sort_order`) VALUES
('文本处理', 'text-processing', '📝', '文本编辑、格式化、转换等工具', 1),
('图片处理', 'image-processing', '🖼️', '图片编辑、压缩、转换等工具', 2),
('代码工具', 'code-tools', '💻', '代码格式化、生成、分析等工具', 3),
('数据工具', 'data-tools', '📊', '数据处理、分析、转换等工具', 4),
('AI工具', 'ai-tools', '🤖', 'AI相关工具和助手', 5),
('开发工具', 'dev-tools', '🔧', '开发者常用工具', 6),
('其他工具', 'other', '🛠️', '其他实用工具', 99)
ON DUPLICATE KEY UPDATE `name`=`name`;

