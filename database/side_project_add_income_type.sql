-- ============================================
-- 为 side_project 表添加收入类型字段
-- ============================================

-- 添加 income_type 字段（收入类型：development=软件开发, investment=投资）
ALTER TABLE `side_project` 
ADD COLUMN `income_type` VARCHAR(50) NULL DEFAULT 'development' COMMENT '收入类型（development=软件开发, investment=投资）' AFTER `category`,
ADD INDEX `idx_income_type` (`income_type`);

-- 更新现有数据，默认设置为软件开发
UPDATE `side_project` SET `income_type` = 'development' WHERE `income_type` IS NULL;

-- 插入投资收入示例数据
INSERT INTO `side_project` (
  `title`, `description`, `client_name`, `source`, `category`, 
  `income_type`, `price_final`, `status`, 
  `start_time`, `end_time`, `is_public`
) VALUES
(
  '股票投资收益',
  '投资某科技公司股票，获得分红和股价上涨收益',
  '某科技公司',
  '自主投资',
  '股票',
  'investment',
  15000.00,
  1,
  '2024-01-15 00:00:00',
  '2024-03-20 00:00:00',
  0
),
(
  '基金投资收益',
  '投资混合型基金，获得稳定收益',
  'XX基金公司',
  '自主投资',
  '基金',
  'investment',
  8000.00,
  1,
  '2024-02-01 00:00:00',
  '2024-06-30 00:00:00',
  0
),
(
  '加密货币投资',
  '投资比特币和以太坊，获得收益',
  '加密货币交易所',
  '自主投资',
  '加密货币',
  'investment',
  25000.00,
  1,
  '2024-03-01 00:00:00',
  '2024-08-15 00:00:00',
  0
);

