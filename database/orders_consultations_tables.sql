-- ============================================
-- 订单和咨询系统数据库表结构
-- ============================================
-- 
-- 设计原则：
-- 1. 不使用外键约束，通过逻辑关联维护表间关系
-- 2. 关联关系由应用层维护，便于后期维护和扩展
-- 3. 为关联字段创建索引以提升查询性能
-- 4. 详细说明请参考 database/DESIGN_PRINCIPLES.md

-- ============================================
-- 1. Orders 表 - 订单表
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- product_id 通过逻辑关联到 tool.id（工具作为商品），关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `orders` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '订单ID',
    `order_no` VARCHAR(50) NOT NULL COMMENT '订单编号（格式：日期-随机数，如：20251208-XXXX）',
    `product_id` BIGINT NOT NULL COMMENT '商品ID（逻辑关联到 tool.id）',
    `product_name_snapshot` VARCHAR(200) NOT NULL COMMENT '下单时商品名称快照',
    `price_snapshot` DECIMAL(18,2) DEFAULT NULL COMMENT '下单时价格快照（为空表示面议）',
    `quantity` INT NOT NULL DEFAULT 1 COMMENT '数量',
    `total_amount` DECIMAL(18,2) DEFAULT NULL COMMENT '总金额',
    `customer_name` VARCHAR(50) NOT NULL COMMENT '客户姓名',
    `customer_phone` VARCHAR(50) DEFAULT NULL COMMENT '客户手机号',
    `customer_wechat` VARCHAR(50) DEFAULT NULL COMMENT '客户微信号',
    `customer_email` VARCHAR(100) DEFAULT NULL COMMENT '客户邮箱',
    `remark` TEXT DEFAULT NULL COMMENT '需求说明/备注',
    `status` INT NOT NULL DEFAULT 0 COMMENT '订单状态：0-待确认, 1-进行中, 2-已完成, 3-已关闭',
    `internal_note` TEXT DEFAULT NULL COMMENT '内部备注（后台使用）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_order_no` (`order_no`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_customer_phone` (`customer_phone`),
    INDEX `idx_customer_email` (`customer_email`),
    INDEX `idx_customer_wechat` (`customer_wechat`),
    INDEX `idx_created_at` (`created_at`)
    -- 注意：不使用外键约束，product_id 通过逻辑关联到 tool.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='订单表';

-- ============================================
-- 2. PreSaleConsultations 表 - 购买前咨询表
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- product_id 通过逻辑关联到 tool.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `pre_sale_consultations` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '咨询ID',
    `product_id` BIGINT NOT NULL COMMENT '商品ID（逻辑关联到 tool.id）',
    `product_name_snapshot` VARCHAR(200) NOT NULL COMMENT '咨询时商品名称快照',
    `customer_name` VARCHAR(50) NOT NULL COMMENT '客户姓名',
    `customer_phone` VARCHAR(50) DEFAULT NULL COMMENT '客户手机号',
    `customer_wechat` VARCHAR(50) DEFAULT NULL COMMENT '客户微信号',
    `customer_email` VARCHAR(100) DEFAULT NULL COMMENT '客户邮箱',
    `budget_range` VARCHAR(50) DEFAULT NULL COMMENT '预算范围（如：<500, 500-1000, 1000-3000, 待评估）',
    `expected_deadline` VARCHAR(50) DEFAULT NULL COMMENT '期望完成时间（如：3天内, 一周内, 两周内, 不着急）',
    `requirement_description` TEXT NOT NULL COMMENT '需求描述（用户自由描述）',
    `status` INT NOT NULL DEFAULT 0 COMMENT '咨询状态：0-新咨询, 1-已联系, 2-已转为订单, 3-已关闭/无意向',
    `internal_note` TEXT DEFAULT NULL COMMENT '内部备注（后台使用）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_customer_phone` (`customer_phone`),
    INDEX `idx_customer_email` (`customer_email`),
    INDEX `idx_customer_wechat` (`customer_wechat`),
    INDEX `idx_created_at` (`created_at`)
    -- 注意：不使用外键约束，product_id 通过逻辑关联到 tool.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='购买前咨询表';

-- ============================================
-- 3. 扩展 Tool 表字段
-- ============================================
-- 为 tool 表添加新字段以支持订单和咨询功能
-- 注意：MySQL 不支持 ADD COLUMN IF NOT EXISTS 语法
-- 如果字段已存在，执行会报错，可以忽略该错误继续执行其他语句

-- 添加是否支持在线下单字段
-- 如果字段已存在会报错，可以忽略
ALTER TABLE `tool` 
ADD COLUMN `enable_online_order` TINYINT(1) DEFAULT 0 COMMENT '是否显示"立即下单"按钮' AFTER `is_premium`;

-- 添加适合人群字段
-- 如果字段已存在会报错，可以忽略
ALTER TABLE `tool` 
ADD COLUMN `fit_for` TEXT DEFAULT NULL COMMENT '适合人群（文本或JSON）' AFTER `requirements`;

-- 添加不适合情况字段
-- 如果字段已存在会报错，可以忽略
ALTER TABLE `tool` 
ADD COLUMN `not_fit_for` TEXT DEFAULT NULL COMMENT '不适合情况（文本或JSON）' AFTER `fit_for`;

-- 添加交付类型字段（可选）
-- 如果字段已存在会报错，可以忽略
ALTER TABLE `tool` 
ADD COLUMN `delivery_type` VARCHAR(50) DEFAULT NULL COMMENT '交付类型（如：即时交付、定制开发等）' AFTER `not_fit_for`;

-- 添加预计交付时间字段（可选）
-- 如果字段已存在会报错，可以忽略
ALTER TABLE `tool` 
ADD COLUMN `estimated_delivery_time` VARCHAR(100) DEFAULT NULL COMMENT '预计交付时间（如：1-3天、一周内等）' AFTER `delivery_type`;

