-- 定投计划相关数据库表结构
-- 
-- 设计原则：
-- 1. 不使用外键约束，通过逻辑关联维护表间关系
-- 2. 关联关系由应用层维护，便于后期维护和扩展
-- 3. 为关联字段创建索引以提升查询性能

-- 定投计划表
CREATE TABLE IF NOT EXISTS `dca_plan` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(20) NOT NULL COMMENT '股票/基金代码',
  `name` VARCHAR(100) NOT NULL COMMENT '名称',
  `type` VARCHAR(20) NOT NULL DEFAULT 'fund' COMMENT '类型：stock/fund',
  `amount` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '每次定投金额',
  `frequency` VARCHAR(20) NOT NULL DEFAULT 'monthly' COMMENT '频率：daily/weekly/monthly/quarterly',
  `next_execution_date` DATETIME NULL COMMENT '下次执行日期',
  `last_execution_date` DATETIME NULL COMMENT '上次执行日期',
  `total_executions` INT NOT NULL DEFAULT 0 COMMENT '总执行次数',
  `total_invested` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '累计投入金额',
  `is_active` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
  `start_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '开始日期',
  `end_date` DATETIME NULL COMMENT '结束日期（可选）',
  `notes` TEXT NULL COMMENT '备注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_code` (`code`),
  INDEX `idx_type` (`type`),
  INDEX `idx_is_active` (`is_active`),
  INDEX `idx_next_execution_date` (`next_execution_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='定投计划表';

-- 定投执行记录表
-- 注意：根据数据库设计原则，不使用外键约束
-- dca_plan_id 通过逻辑关联到 dca_plan.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `dca_execution` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `dca_plan_id` BIGINT NOT NULL COMMENT '定投计划ID（逻辑关联到 dca_plan.id）',
  `execution_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '执行日期',
  `amount` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '执行金额',
  `price` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '执行时的价格',
  `quantity` DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT '买入数量',
  `status` VARCHAR(20) NOT NULL DEFAULT 'pending' COMMENT '状态：pending/completed/failed',
  `error_message` TEXT NULL COMMENT '错误信息',
  `notes` TEXT NULL COMMENT '备注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_dca_plan_id` (`dca_plan_id`),
  INDEX `idx_execution_date` (`execution_date`),
  INDEX `idx_status` (`status`)
  -- 注意：不使用外键约束，dca_plan_id 通过逻辑关联到 dca_plan.id
  -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='定投执行记录表';
