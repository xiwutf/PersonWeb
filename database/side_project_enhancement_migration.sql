-- ============================================
-- 副业项目管理模块增强迁移
-- 添加阶段、进度、优先级、截止时间等字段，以及相关子表
-- ============================================

-- 1. 扩展 side_project 表，添加新字段
ALTER TABLE `side_project`
  ADD COLUMN `stage` VARCHAR(50) NULL COMMENT '阶段：待开始/进行中/卡住/待验收/已完成' AFTER `is_public`,
  ADD COLUMN `progress` INT NULL COMMENT '进度 0-100' AFTER `stage`,
  ADD COLUMN `is_progress_manual` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '进度是否手动覆盖（false=自动计算，true=手动设置）' AFTER `progress`,
  ADD COLUMN `priority` INT NULL COMMENT '优先级：0=低，1=中，2=高，3=紧急' AFTER `is_progress_manual`,
  ADD COLUMN `deadline_at` DATETIME NULL COMMENT '截止时间' AFTER `priority`,
  ADD COLUMN `next_action` VARCHAR(500) NULL COMMENT '下一步行动' AFTER `deadline_at`,
  ADD COLUMN `blocked` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否阻塞' AFTER `next_action`,
  ADD COLUMN `block_reason` VARCHAR(1000) NULL COMMENT '阻塞原因' AFTER `blocked`,
  ADD COLUMN `total_amount` DECIMAL(18,2) NULL COMMENT '总金额' AFTER `block_reason`,
  ADD COLUMN `received_amount` DECIMAL(18,2) NULL COMMENT '已收款金额' AFTER `total_amount`;

-- 添加索引
ALTER TABLE `side_project`
  ADD INDEX `idx_stage` (`stage`),
  ADD INDEX `idx_priority` (`priority`),
  ADD INDEX `idx_deadline_at` (`deadline_at`),
  ADD INDEX `idx_blocked` (`blocked`);

-- 2. 创建 side_project_requirement 表（项目需求）
CREATE TABLE IF NOT EXISTS `side_project_requirement` (
  `id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '主键ID',
  `project_id` INT NOT NULL COMMENT '项目ID',
  `scope_in` TEXT NULL COMMENT '范围内需求',
  `scope_out` TEXT NULL COMMENT '范围外需求',
  `acceptance_criteria` TEXT NULL COMMENT '验收标准',
  `deliverables` TEXT NULL COMMENT '交付物（JSON格式或文本）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_project_id` (`project_id`),
  CONSTRAINT `fk_requirement_project` FOREIGN KEY (`project_id`) REFERENCES `side_project` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='项目需求表';

-- 3. 创建 side_project_task 表（项目任务）
CREATE TABLE IF NOT EXISTS `side_project_task` (
  `id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '主键ID',
  `project_id` INT NOT NULL COMMENT '项目ID',
  `title` VARCHAR(200) NOT NULL COMMENT '任务标题',
  `description` TEXT NULL COMMENT '任务描述',
  `status` INT NOT NULL DEFAULT 0 COMMENT '状态（0=未开始，1=进行中，2=已完成，3=已取消）',
  `priority` INT NULL COMMENT '优先级：0=低，1=中，2=高，3=紧急',
  `due_at` DATETIME NULL COMMENT '截止时间',
  `est_hours` DECIMAL(10,2) NULL COMMENT '预计工时（小时）',
  `act_hours` DECIMAL(10,2) NULL COMMENT '实际工时（小时）',
  `sort_order` INT NOT NULL DEFAULT 0 COMMENT '排序顺序',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_project_id` (`project_id`),
  INDEX `idx_project_sort` (`project_id`, `sort_order`),
  INDEX `idx_status` (`status`),
  INDEX `idx_due_at` (`due_at`),
  CONSTRAINT `fk_task_project` FOREIGN KEY (`project_id`) REFERENCES `side_project` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='项目任务表';

-- 4. 创建 side_project_milestone 表（项目里程碑）
CREATE TABLE IF NOT EXISTS `side_project_milestone` (
  `id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '主键ID',
  `project_id` INT NOT NULL COMMENT '项目ID',
  `title` VARCHAR(200) NOT NULL COMMENT '里程碑标题',
  `due_at` DATETIME NULL COMMENT '截止时间',
  `status` INT NOT NULL DEFAULT 0 COMMENT '状态（0=未完成，1=已完成）',
  `notes` TEXT NULL COMMENT '备注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_project_id` (`project_id`),
  INDEX `idx_due_at` (`due_at`),
  CONSTRAINT `fk_milestone_project` FOREIGN KEY (`project_id`) REFERENCES `side_project` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='项目里程碑表';

-- 5. 创建 side_project_log 表（项目沟通记录）
CREATE TABLE IF NOT EXISTS `side_project_log` (
  `id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '主键ID',
  `project_id` INT NOT NULL COMMENT '项目ID',
  `channel` VARCHAR(50) NULL COMMENT '沟通渠道：微信/邮件/电话/会议/其他',
  `content` TEXT NULL COMMENT '沟通内容',
  `next_todo` VARCHAR(500) NULL COMMENT '下一步待办',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  INDEX `idx_project_id` (`project_id`),
  INDEX `idx_project_created` (`project_id`, `created_at`),
  CONSTRAINT `fk_log_project` FOREIGN KEY (`project_id`) REFERENCES `side_project` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='项目沟通记录表';

-- 6. 创建 side_project_attachment 表（项目附件）
CREATE TABLE IF NOT EXISTS `side_project_attachment` (
  `id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '主键ID',
  `project_id` INT NOT NULL COMMENT '项目ID',
  `type` VARCHAR(50) NULL COMMENT '附件类型：文档/图片/代码/其他',
  `name` VARCHAR(500) NOT NULL COMMENT '附件名称',
  `url` VARCHAR(1000) NOT NULL COMMENT '附件URL或路径',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  INDEX `idx_project_id` (`project_id`),
  CONSTRAINT `fk_attachment_project` FOREIGN KEY (`project_id`) REFERENCES `side_project` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='项目附件表';

-- 7. 更新现有项目的阶段（根据状态）
UPDATE `side_project` 
SET `stage` = CASE 
  WHEN `status` = 1 THEN '已完成'
  WHEN `status` = 2 THEN '待验收'
  WHEN `status` = 0 THEN '进行中'
  ELSE '待开始'
END
WHERE `stage` IS NULL;

