-- ============================================
-- 情报中心（Intelligence）相关表结构
-- 数据库: personal_site
-- 创建时间: 2026-03-07
-- ============================================

-- 1. 情报来源表
CREATE TABLE IF NOT EXISTS `intelligence_source` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '来源ID',
    `source_name` VARCHAR(200) NOT NULL COMMENT '来源名称',
    `source_type` VARCHAR(20) NOT NULL COMMENT '来源类型: RSS | WEB',
    `source_url` VARCHAR(500) NOT NULL COMMENT '来源地址',
    `category` VARCHAR(100) NOT NULL COMMENT '分类',
    `tags_json` JSON DEFAULT NULL COMMENT '标签 (JSON)',
    `priority` INT NOT NULL DEFAULT 0 COMMENT '优先级',
    `enabled` BOOLEAN NOT NULL DEFAULT TRUE COMMENT '是否启用',
    `fetch_interval_minutes` INT NOT NULL DEFAULT 60 COMMENT '抓取间隔（分钟）',
    `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
    `last_fetch_time` DATETIME DEFAULT NULL COMMENT '最后抓取时间',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_enabled` (`enabled`),
    INDEX `idx_priority` (`priority`),
    INDEX `idx_last_fetch` (`last_fetch_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='情报来源表';

-- 2. 采集内容表
CREATE TABLE IF NOT EXISTS `intelligence_content` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '内容ID',
    `source_id` BIGINT NOT NULL COMMENT '来源ID',
    `title` VARCHAR(200) NOT NULL COMMENT '标题',
    `original_url` VARCHAR(1000) NOT NULL COMMENT '原始URL',
    `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
    `author` VARCHAR(200) DEFAULT NULL COMMENT '作者',
    `raw_text` LONGTEXT DEFAULT NULL COMMENT '原始文本',
    `clean_text` LONGTEXT DEFAULT NULL COMMENT '清洗后文本',
    `content_hash` VARCHAR(64) NOT NULL COMMENT '内容哈希（用于去重）',
    `fetch_status` VARCHAR(20) NOT NULL DEFAULT 'pending' COMMENT '抓取状态: pending | success | failed',
    `analyze_status` VARCHAR(20) NOT NULL DEFAULT 'pending' COMMENT '分析状态: pending | processing | success | failed',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_source_id` (`source_id`),
    INDEX `idx_content_hash` (`content_hash`),
    INDEX `idx_publish_time` (`publish_time`),
    INDEX `idx_analyze_status` (`analyze_status`),
    INDEX `idx_fetch_status` (`fetch_status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='采集内容表';

-- 3. AI 分析结果表
CREATE TABLE IF NOT EXISTS `intelligence_analysis` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '分析ID',
    `content_id` BIGINT NOT NULL COMMENT '内容ID',
    `category` VARCHAR(50) NOT NULL COMMENT '分类: AI技术|软件开发|商业机会|投资理财|认知成长|其他',
    `summary` LONGTEXT DEFAULT NULL COMMENT '摘要',
    `core_points_json` JSON DEFAULT NULL COMMENT '核心要点 (JSON)',
    `tags_json` JSON DEFAULT NULL COMMENT '标签 (JSON)',
    `relevance_score` INT NOT NULL DEFAULT 0 COMMENT '相关性评分 0-100',
    `learning_value` VARCHAR(10) NOT NULL DEFAULT '中' COMMENT '学习价值: 高|中|低',
    `business_value` VARCHAR(10) NOT NULL DEFAULT '中' COMMENT '商业价值: 高|中|低',
    `action_suggestion` LONGTEXT DEFAULT NULL COMMENT '行动建议',
    `model_name` VARCHAR(100) DEFAULT NULL COMMENT '使用的模型名称',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_content_id` (`content_id`),
    INDEX `idx_category` (`category`),
    INDEX `idx_relevance_score` (`relevance_score`),
    INDEX `idx_learning_value` (`learning_value`),
    INDEX `idx_business_value` (`business_value`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='AI 分析结果表';

-- 4. 每日报告表
CREATE TABLE IF NOT EXISTS `intelligence_daily_report` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '报告ID',
    `report_date` DATE NOT NULL COMMENT '报告日期',
    `title` VARCHAR(200) NOT NULL COMMENT '报告标题',
    `content_markdown` LONGTEXT DEFAULT NULL COMMENT 'Markdown 内容',
    `item_count` INT NOT NULL DEFAULT 0 COMMENT '包含内容数',
    `generated_at` DATETIME DEFAULT NULL COMMENT '生成时间',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_report_date` (`report_date`),
    INDEX `idx_generated_at` (`generated_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='每日报告表';

-- 5. 任务执行日志表
CREATE TABLE IF NOT EXISTS `intelligence_task_log` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '日志ID',
    `task_name` VARCHAR(100) NOT NULL COMMENT '任务名称: collect|analyze|generate_report',
    `task_type` VARCHAR(50) DEFAULT NULL COMMENT '任务类型',
    `status` VARCHAR(20) NOT NULL DEFAULT 'running' COMMENT '状态: running | success | failed',
    `start_time` DATETIME NOT NULL COMMENT '开始时间',
    `end_time` DATETIME DEFAULT NULL COMMENT '结束时间',
    `message` TEXT DEFAULT NULL COMMENT '消息',
    `detail_json` JSON DEFAULT NULL COMMENT '详细信息 (JSON)',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`id`),
    INDEX `idx_task_name` (`task_name`),
    INDEX `idx_start_time` (`start_time`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='任务执行日志表';

-- ============================================
-- 初始化示例数据
-- ============================================

-- 插入示例来源数据
INSERT INTO `intelligence_source`
    (`source_name`, `source_type`, `source_url`, `category`, `tags_json`, `priority`, `enabled`, `fetch_interval_minutes`)
VALUES
    ('36氪', 'RSS', 'https://36kr.com/feed', 'AI技术', '["AI", "科技", "创新"]', 1, TRUE, 60),
    ('少数派', 'RSS', 'https://sspai.com/feed', '软件开发', '["效率工具", "Mac", "iOS"]', 2, TRUE, 120),
    ('阮一峰的网络日志', 'WEB', 'https://www.ruanyifeng.com/blog/', '认知成长', '["个人成长", "思考", "学习"]', 3, TRUE, 180);
