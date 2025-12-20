-- ============================================
-- 关系跟进模块：添加观察期追踪字段
-- ============================================

-- 为 relation_person 表添加观察期相关字段
ALTER TABLE `relation_person`
ADD COLUMN `observation_started_at` DATETIME NULL COMMENT '观察期开始时间',
ADD COLUMN `observation_expected_end_at` DATETIME NULL COMMENT '观察期预计结束时间（默认开始后7天）',
ADD COLUMN `observation_last_reminded_at` DATETIME NULL COMMENT '观察期上次提醒时间',
ADD COLUMN `observation_reason` VARCHAR(500) NULL COMMENT '进入观察期的原因',
ADD COLUMN `observation_decision_pending` BOOLEAN NOT NULL DEFAULT FALSE COMMENT '是否等待观察期结束决策',
ADD INDEX `idx_observation_started_at` (`observation_started_at`),
ADD INDEX `idx_observation_decision_pending` (`observation_decision_pending`);

