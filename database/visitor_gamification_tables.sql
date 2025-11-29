-- 访客互动式玩法系统数据库表结构
-- 
-- 设计原则：
-- 1. 不使用外键约束，通过逻辑关联维护表间关系
-- 2. 关联关系由应用层维护，便于后期维护和扩展
-- 3. 为关联字段创建索引以提升查询性能
-- 4. 详细说明请参考 database/DESIGN_PRINCIPLES.md

-- ============================================
-- 1. VisitorLevel 表 - 访客等级系统
-- ============================================
CREATE TABLE IF NOT EXISTS `visitor_level` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '等级ID',
    `visitor_id` VARCHAR(100) NOT NULL COMMENT '访客ID',
    `level` INT NOT NULL DEFAULT 1 COMMENT '当前等级',
    `experience` INT NOT NULL DEFAULT 0 COMMENT '当前经验值',
    `total_experience` INT NOT NULL DEFAULT 0 COMMENT '累计经验值',
    `title` VARCHAR(50) DEFAULT NULL COMMENT '等级称号（如：普通访客、老朋友、致敬探索者）',
    `badge` VARCHAR(50) DEFAULT NULL COMMENT '等级徽章图标',
    `unlocked_features` JSON DEFAULT NULL COMMENT '解锁的功能（JSON数组）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_visitor_id` (`visitor_id`),
    INDEX `idx_level` (`level`),
    INDEX `idx_experience` (`experience`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='访客等级表';

-- ============================================
-- 2. VisitorBehavior 表 - 访客行为记录（用于触发事件和计算经验）
-- ============================================
CREATE TABLE IF NOT EXISTS `visitor_behavior` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '行为ID',
    `visitor_id` VARCHAR(100) NOT NULL COMMENT '访客ID',
    `behavior_type` VARCHAR(50) NOT NULL COMMENT '行为类型：scroll_to_bottom-滚动到底, avatar_hover-头像悬停, idle_10s-闲置10秒, use_tool-使用工具, comment-评论, share-分享, unlock_egg-解锁彩蛋, complete_game-完成游戏',
    `behavior_data` JSON DEFAULT NULL COMMENT '行为数据（JSON格式，存储额外信息）',
    `experience_gained` INT DEFAULT 0 COMMENT '获得经验值',
    `triggered_event` VARCHAR(50) DEFAULT NULL COMMENT '触发的效果事件（如：skill_tree_glow, avatar_blink, assistant_greet）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`id`),
    INDEX `idx_visitor_id` (`visitor_id`),
    INDEX `idx_behavior_type` (`behavior_type`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='访客行为记录表';

-- ============================================
-- 3. VisitorChallenge 表 - 访客合作挑战
-- ============================================
CREATE TABLE IF NOT EXISTS `visitor_challenge` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '挑战ID',
    `challenge_code` VARCHAR(50) NOT NULL COMMENT '挑战代码（如：button_press_10, collective_goal_100）',
    `challenge_name` VARCHAR(100) NOT NULL COMMENT '挑战名称',
    `challenge_type` VARCHAR(50) NOT NULL COMMENT '挑战类型：button_press-按钮按下, collective_goal-集体目标',
    `target_count` INT NOT NULL COMMENT '目标数量（如：10个访客同时按按钮）',
    `current_count` INT NOT NULL DEFAULT 0 COMMENT '当前计数',
    `status` VARCHAR(20) DEFAULT 'active' COMMENT '状态：active-进行中, completed-已完成, expired-已过期',
    `reward_description` TEXT DEFAULT NULL COMMENT '奖励描述',
    `unlocked_content` JSON DEFAULT NULL COMMENT '解锁的内容（JSON格式）',
    `started_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '开始时间',
    `completed_at` DATETIME DEFAULT NULL COMMENT '完成时间',
    `expires_at` DATETIME DEFAULT NULL COMMENT '过期时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_challenge_code` (`challenge_code`),
    INDEX `idx_status` (`status`),
    INDEX `idx_challenge_type` (`challenge_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='访客合作挑战表';

-- ============================================
-- 4. VisitorChallengeParticipant 表 - 挑战参与者
-- ============================================
-- 注意：根据数据库设计原则，不使用外键约束
-- challenge_id 通过逻辑关联到 visitor_challenge.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `visitor_challenge_participant` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '参与记录ID',
    `challenge_id` BIGINT NOT NULL COMMENT '挑战ID（逻辑关联到 visitor_challenge.id）',
    `visitor_id` VARCHAR(100) NOT NULL COMMENT '访客ID',
    `action_type` VARCHAR(50) NOT NULL COMMENT '动作类型（如：button_press）',
    `action_data` JSON DEFAULT NULL COMMENT '动作数据',
    `contributed_count` INT DEFAULT 1 COMMENT '贡献的计数',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '参与时间',
    PRIMARY KEY (`id`),
    INDEX `idx_challenge_id` (`challenge_id`),
    INDEX `idx_visitor_id` (`visitor_id`),
    INDEX `idx_created_at` (`created_at`)
    -- 注意：不使用外键约束，challenge_id 通过逻辑关联到 visitor_challenge.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='挑战参与者表';

-- ============================================
-- 5. VisitorAchievement 表 - 访客成就系统
-- ============================================
CREATE TABLE IF NOT EXISTS `visitor_achievement` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '成就ID',
    `visitor_id` VARCHAR(100) NOT NULL COMMENT '访客ID',
    `achievement_code` VARCHAR(50) NOT NULL COMMENT '成就代码',
    `achievement_name` VARCHAR(100) NOT NULL COMMENT '成就名称',
    `achievement_description` TEXT DEFAULT NULL COMMENT '成就描述',
    `icon` VARCHAR(100) DEFAULT NULL COMMENT '成就图标',
    `unlocked_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '解锁时间',
    PRIMARY KEY (`id`),
    INDEX `idx_visitor_id` (`visitor_id`),
    INDEX `idx_achievement_code` (`achievement_code`),
    UNIQUE KEY `uk_visitor_achievement` (`visitor_id`, `achievement_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='访客成就表';

-- ============================================
-- 6. VisitorTriggerEvent 表 - 触发事件记录（用于统计和分析）
-- ============================================
CREATE TABLE IF NOT EXISTS `visitor_trigger_event` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '事件ID',
    `visitor_id` VARCHAR(100) NOT NULL COMMENT '访客ID',
    `trigger_type` VARCHAR(50) NOT NULL COMMENT '触发类型：skill_tree_glow-技能树发光, avatar_blink-头像眨眼, assistant_greet-助手问候',
    `trigger_context` JSON DEFAULT NULL COMMENT '触发上下文（JSON格式，存储页面路径、时间等）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '触发时间',
    PRIMARY KEY (`id`),
    INDEX `idx_visitor_id` (`visitor_id`),
    INDEX `idx_trigger_type` (`trigger_type`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='触发事件记录表';

-- ============================================
-- 初始化数据：等级配置（可在后端代码中定义，这里仅作示例）
-- ============================================
-- 等级配置示例：
-- Level 1: 普通访客 (0-99 exp)
-- Level 5: 老朋友 (500-999 exp)
-- Level 10: 致敬探索者 (2000+ exp)

-- ============================================
-- 初始化数据：挑战配置
-- ============================================
INSERT INTO `visitor_challenge` (`challenge_code`, `challenge_name`, `challenge_type`, `target_count`, `status`, `reward_description`) VALUES
('button_press_10', '10人同时按按钮', 'button_press', 10, 'active', '触发全屏烟花效果'),
('collective_goal_100', '集体目标100', 'collective_goal', 100, 'active', '解锁隐藏内容')
ON DUPLICATE KEY UPDATE `challenge_name`=`challenge_name`;

