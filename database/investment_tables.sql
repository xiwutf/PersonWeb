-- 投资相关数据库表结构
-- 
-- 设计原则：
-- 1. 不使用外键约束，通过逻辑关联维护表间关系
-- 2. 关联关系由应用层维护，便于后期维护和扩展
-- 3. 为关联字段创建索引以提升查询性能
-- 4. 详细说明请参考 database/DESIGN_PRINCIPLES.md

-- 用户行为记录表（用于 AI 推荐）
CREATE TABLE IF NOT EXISTS `user_behavior` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` VARCHAR(36) NULL COMMENT '用户ID',
  `behavior_type` VARCHAR(50) NOT NULL COMMENT '行为类型：read_article/view_book/add_tag等',
  `target_id` VARCHAR(255) NULL COMMENT '目标ID',
  `target_title` VARCHAR(500) NULL COMMENT '目标标题',
  `tags` VARCHAR(500) NULL COMMENT '相关标签',
  `category` VARCHAR(500) NULL COMMENT '分类',
  `duration` INT NULL COMMENT '停留时长（秒）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_behavior_type` (`behavior_type`),
  INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户行为记录表';

-- 投资记录表
CREATE TABLE IF NOT EXISTS `investment` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(20) NOT NULL COMMENT '股票/基金代码',
  `name` VARCHAR(100) NOT NULL COMMENT '名称',
  `type` VARCHAR(20) NOT NULL DEFAULT 'stock' COMMENT '类型：stock/fund',
  `quantity` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '持仓数量',
  `cost_price` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '成本价',
  `current_price` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '当前价',
  `total_cost` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '总成本',
  `market_value` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '市值',
  `profit_loss` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '盈亏',
  `profit_rate` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '盈亏比例',
  `notes` TEXT NULL COMMENT '备注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`),
  INDEX `idx_type` (`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='投资记录表';

-- 投资交易记录表
-- 注意：根据数据库设计原则，不使用外键约束
-- investment_id 通过逻辑关联到 investment.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `investment_transaction` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `investment_id` BIGINT NOT NULL COMMENT '投资ID（逻辑关联到 investment.id）',
  `transaction_type` VARCHAR(20) NOT NULL COMMENT '交易类型：buy/sell',
  `quantity` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '交易数量',
  `price` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '交易价格',
  `amount` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '交易金额',
  `fee` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '手续费',
  `transaction_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '交易日期',
  `notes` TEXT NULL COMMENT '备注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_investment_id` (`investment_id`),
  INDEX `idx_transaction_date` (`transaction_date`)
  -- 注意：不使用外键约束，investment_id 通过逻辑关联到 investment.id
  -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='投资交易记录表';

