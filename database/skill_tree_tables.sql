-- 技能树系统数据库表
-- 用于记录技能分类、能力评级、学习日志等

-- 1. 技能分类表
CREATE TABLE IF NOT EXISTS `skill_category` (
  `id` BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `name` VARCHAR(50) NOT NULL COMMENT '分类名称（前端/后端/数据库/云服务/AI）',
  `icon` VARCHAR(50) NULL COMMENT '图标',
  `color` VARCHAR(20) NULL COMMENT '颜色代码',
  `sort_order` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY `uk_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='技能分类表';

-- 2. 技能表
CREATE TABLE IF NOT EXISTS `skill` (
  `id` BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `category_id` BIGINT NOT NULL COMMENT '分类ID（逻辑关联到 skill_category.id）',
  `name` VARCHAR(100) NOT NULL COMMENT '技能名称',
  `description` TEXT NULL COMMENT '技能描述',
  `icon` VARCHAR(50) NULL COMMENT '图标',
  `current_rating` DECIMAL(3,1) DEFAULT 0.0 COMMENT '当前评分（0-10）',
  `target_rating` DECIMAL(3,1) DEFAULT NULL COMMENT '目标评分',
  `sort_order` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  KEY `idx_category_id` (`category_id`),
  KEY `idx_sort_order` (`sort_order`)
  -- 注意：不使用外键约束，category_id 通过逻辑关联到 skill_category.id
  -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='技能表';

-- 3. 技能评级表（记录不同时间点的技能评级）
CREATE TABLE IF NOT EXISTS `skill_rating` (
  `id` BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `skill_id` BIGINT NOT NULL COMMENT '技能ID',
  `rating` DECIMAL(3,1) NOT NULL COMMENT '能力评级（1-10分，可小数）',
  `notes` TEXT NULL COMMENT '备注说明',
  `recorded_at` DATETIME NOT NULL COMMENT '记录时间',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  KEY `idx_skill_id` (`skill_id`),
  KEY `idx_recorded_at` (`recorded_at`)
  -- 注意：不使用外键约束，skill_id 通过逻辑关联到 skill.id
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='技能评级表';

-- 4. 学习日志表
CREATE TABLE IF NOT EXISTS `learning_log` (
  `id` BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `skill_id` BIGINT NOT NULL COMMENT '技能ID',
  `title` VARCHAR(200) NOT NULL COMMENT '学习内容标题',
  `content` TEXT NULL COMMENT '学习内容详情',
  `duration` INT NULL COMMENT '学习时长（分钟）',
  `resource_type` VARCHAR(50) NULL COMMENT '资源类型（视频/文档/实践/课程）',
  `resource_url` VARCHAR(500) NULL COMMENT '资源链接',
  `learned_at` DATETIME NOT NULL COMMENT '学习时间',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  KEY `idx_skill_id` (`skill_id`),
  KEY `idx_learned_at` (`learned_at`)
  -- 注意：不使用外键约束，skill_id 通过逻辑关联到 skill.id
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='学习日志表';

-- 插入默认技能分类
INSERT INTO `skill_category` (`name`, `icon`, `color`, `sort_order`) VALUES
('前端开发', '💻', '#3b82f6', 1),
('后端开发', '⚙️', '#10b981', 2),
('数据库', '🗄️', '#f59e0b', 3),
('云服务', '☁️', '#8b5cf6', 4),
('AI/ML', '🤖', '#ec4899', 5),
('DevOps', '🔧', '#6366f1', 6),
('其他', '📚', '#6b7280', 99)
ON DUPLICATE KEY UPDATE `name`=`name`;

