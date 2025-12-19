-- ============================================
-- 关系跟进模块表（Dating CRM / Relationship Tracker）
-- ============================================

-- 创建 relation_person 表（关系对象表）
CREATE TABLE IF NOT EXISTS `relation_person` (
  `id` CHAR(36) NOT NULL PRIMARY KEY COMMENT '主键ID（GUID）',
  `nickname` VARCHAR(100) NOT NULL COMMENT '昵称（必填）',
  `source` VARCHAR(200) NULL COMMENT '来源（如：朋友介绍/社交软件/活动等）',
  `city` VARCHAR(100) NULL COMMENT '城市',
  `tags` TEXT NULL COMMENT '标签（JSON数组字符串）',
  `preferences` TEXT NULL COMMENT '喜好/雷点/关键点',
  `stage` INT NOT NULL DEFAULT 0 COMMENT '阶段：0=新认识, 1=熟悉中, 2=准备约见, 3=已见面, 4=升温中, 5=观察期, 6=已结束',
  `heat_score` INT NOT NULL DEFAULT 0 COMMENT '热度分数（0-100）',
  `last_contact_at` DATETIME NULL COMMENT '最后联系时间',
  `last_meet_at` DATETIME NULL COMMENT '最后见面时间',
  `next_action` VARCHAR(500) NULL COMMENT '下一步行动',
  `remind_at` DATETIME NULL COMMENT '提醒时间',
  `notes` TEXT NULL COMMENT '备注',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_stage` (`stage`),
  INDEX `idx_last_contact_at` (`last_contact_at`),
  INDEX `idx_remind_at` (`remind_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='关系对象表';

-- 创建 relation_interaction 表（互动记录表）
CREATE TABLE IF NOT EXISTS `relation_interaction` (
  `id` CHAR(36) NOT NULL PRIMARY KEY COMMENT '主键ID（GUID）',
  `person_id` CHAR(36) NOT NULL COMMENT '关联的对象ID',
  `type` INT NOT NULL DEFAULT 0 COMMENT '互动类型：0=文字, 1=语音, 2=通话, 3=见面, 4=其他',
  `occurred_at` DATETIME NOT NULL COMMENT '发生时间',
  `summary` TEXT NOT NULL COMMENT '摘要（必填）',
  `key_points` TEXT NULL COMMENT '要点/承诺/情绪（JSON对象）',
  `my_feeling` INT NULL COMMENT '我的感受：0=正, 1=中, 2=负',
  `ai_suggestion` TEXT NULL COMMENT 'AI建议',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  INDEX `idx_person_id_occurred_at` (`person_id`, `occurred_at`),
  INDEX `idx_type` (`type`),
  CONSTRAINT `fk_interaction_person` FOREIGN KEY (`person_id`) REFERENCES `relation_person` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='互动记录表';

-- 创建 relation_task 表（任务表）
CREATE TABLE IF NOT EXISTS `relation_task` (
  `id` CHAR(36) NOT NULL PRIMARY KEY COMMENT '主键ID（GUID）',
  `person_id` CHAR(36) NOT NULL COMMENT '关联的对象ID',
  `title` VARCHAR(500) NOT NULL COMMENT '任务标题',
  `due_at` DATETIME NULL COMMENT '截止时间',
  `priority` INT NOT NULL DEFAULT 1 COMMENT '优先级：0=低, 1=中, 2=高, 3=紧急',
  `status` INT NOT NULL DEFAULT 0 COMMENT '状态：0=todo, 1=done, 2=hold',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_person_id_status` (`person_id`, `status`),
  INDEX `idx_due_at` (`due_at`),
  CONSTRAINT `fk_task_person` FOREIGN KEY (`person_id`) REFERENCES `relation_person` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='任务表';

