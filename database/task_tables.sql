-- 任务/目标追踪系统数据库表结构
-- 数据库: personal_site

-- ============================================
-- 1. 任务表（Task）
-- ============================================
CREATE TABLE IF NOT EXISTS `task` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(255) NOT NULL COMMENT '任务标题',
  `description` TEXT NULL COMMENT '任务描述',
  `status` VARCHAR(20) NOT NULL DEFAULT 'pending' COMMENT '任务状态: pending/in_progress/completed/cancelled',
  `priority` INT NOT NULL DEFAULT 0 COMMENT '优先级: 0-低, 1-中, 2-高, 3-紧急',
  `category` VARCHAR(50) NULL COMMENT '任务分类',
  `tags` VARCHAR(500) NULL COMMENT '标签，逗号分隔',
  `due_date` DATETIME NULL COMMENT '截止日期',
  `completed_at` DATETIME NULL COMMENT '完成时间',
  `estimated_hours` DECIMAL(5,2) NULL COMMENT '预计工时（小时）',
  `actual_hours` DECIMAL(5,2) NULL COMMENT '实际工时（小时）',
  `progress` INT NOT NULL DEFAULT 0 COMMENT '进度百分比 0-100',
  `parent_id` BIGINT NULL COMMENT '父任务ID（支持子任务）',
  `sort_order` INT NOT NULL DEFAULT 0 COMMENT '排序顺序',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_task_status` (`status`),
  INDEX `idx_task_priority` (`priority`),
  INDEX `idx_task_due_date` (`due_date`),
  INDEX `idx_task_category` (`category`),
  INDEX `idx_task_parent` (`parent_id`),
  INDEX `idx_task_created` (`created_at`),
  CONSTRAINT `fk_task_parent`
    FOREIGN KEY (`parent_id`)
    REFERENCES `task` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='任务表';

-- ============================================
-- 2. 年度目标表（Goal）
-- ============================================
CREATE TABLE IF NOT EXISTS `goal` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `year` INT NOT NULL COMMENT '目标年份',
  `title` VARCHAR(255) NOT NULL COMMENT '目标标题',
  `description` TEXT NULL COMMENT '目标描述',
  `category` VARCHAR(50) NULL COMMENT '目标分类（工作/学习/生活等）',
  `target_value` DECIMAL(10,2) NULL COMMENT '目标数值（如：完成10个项目）',
  `current_value` DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT '当前数值',
  `unit` VARCHAR(50) NULL COMMENT '单位（如：个/篇/小时）',
  `status` VARCHAR(20) NOT NULL DEFAULT 'active' COMMENT '状态: active/completed/archived',
  `progress` INT NOT NULL DEFAULT 0 COMMENT '进度百分比 0-100',
  `start_date` DATE NULL COMMENT '开始日期',
  `end_date` DATE NULL COMMENT '结束日期',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_goal_year` (`year`),
  INDEX `idx_goal_status` (`status`),
  INDEX `idx_goal_category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='年度目标表';

-- ============================================
-- 3. 月度 KPI 表（Monthly KPI）
-- ============================================
CREATE TABLE IF NOT EXISTS `monthly_kpi` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `goal_id` BIGINT NOT NULL COMMENT '关联的年度目标ID',
  `year` INT NOT NULL COMMENT '年份',
  `month` INT NOT NULL COMMENT '月份 1-12',
  `title` VARCHAR(255) NOT NULL COMMENT 'KPI 标题',
  `target_value` DECIMAL(10,2) NULL COMMENT '目标数值',
  `current_value` DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT '当前数值',
  `unit` VARCHAR(50) NULL COMMENT '单位',
  `status` VARCHAR(20) NOT NULL DEFAULT 'pending' COMMENT '状态: pending/in_progress/completed',
  `progress` INT NOT NULL DEFAULT 0 COMMENT '进度百分比 0-100',
  `notes` TEXT NULL COMMENT '备注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_kpi_goal` (`goal_id`),
  INDEX `idx_kpi_year_month` (`year`, `month`),
  INDEX `idx_kpi_status` (`status`),
  CONSTRAINT `fk_kpi_goal`
    FOREIGN KEY (`goal_id`)
    REFERENCES `goal` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='月度KPI表';

-- ============================================
-- 4. 任务时间记录表（Task Time Log）
-- ============================================
CREATE TABLE IF NOT EXISTS `task_time_log` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `task_id` BIGINT NOT NULL COMMENT '任务ID',
  `log_date` DATE NOT NULL COMMENT '记录日期',
  `hours` DECIMAL(5,2) NOT NULL COMMENT '工时（小时）',
  `notes` TEXT NULL COMMENT '备注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_time_log_task` (`task_id`),
  INDEX `idx_time_log_date` (`log_date`),
  CONSTRAINT `fk_time_log_task`
    FOREIGN KEY (`task_id`)
    REFERENCES `task` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='任务时间记录表';

