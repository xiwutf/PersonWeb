-- ============================================
-- 兼职接单项目表（SideProject）
-- ============================================

-- 创建 side_project 表
CREATE TABLE IF NOT EXISTS `side_project` (
  `id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '主键ID',
  `title` VARCHAR(200) NOT NULL COMMENT '项目标题',
  `description` TEXT NULL COMMENT '项目描述',
  `client_name` VARCHAR(200) NULL COMMENT '客户名称（可匿名）',
  `client_contact` VARCHAR(200) NULL COMMENT '客户联系方式（后台可见）',
  `source` VARCHAR(100) NULL COMMENT '客户来源（朋友介绍/平台/公司/其他）',
  `category` VARCHAR(100) NULL COMMENT '项目类型（网站/小程序/AI/其他）',
  `tech_stack` VARCHAR(200) NULL COMMENT '技术栈（以逗号分隔：Vue,.NET,Python）',
  `budget_min` DECIMAL(18,2) NULL COMMENT '预算下限',
  `budget_max` DECIMAL(18,2) NULL COMMENT '预算上限',
  `price_final` DECIMAL(18,2) NULL COMMENT '实际成交金额',
  `status` INT NOT NULL DEFAULT 0 COMMENT '状态（0 进行中 / 1 已完成 / 2 待付款 / 3 已取消）',
  `start_time` DATETIME NULL COMMENT '开始时间',
  `end_time` DATETIME NULL COMMENT '结束时间',
  `is_public` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否公开展示',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_status` (`status`),
  INDEX `idx_category` (`category`),
  INDEX `idx_created_at` (`created_at`),
  INDEX `idx_is_public` (`is_public`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='兼职项目表';

