-- 知识库相关数据库表结构
-- 
-- 设计原则：
-- 1. 不使用外键约束，通过逻辑关联维护表间关系
-- 2. 关联关系由应用层维护，便于后期维护和扩展
-- 3. 为关联字段创建索引以提升查询性能
-- 4. 详细说明请参考 database/DESIGN_PRINCIPLES.md

-- 个人知识库表
-- 注意：根据数据库设计原则，不使用外键约束
-- author_id 通过逻辑关联到 user.id，关联关系由应用层维护
CREATE TABLE IF NOT EXISTS `knowledge_base` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(255) NOT NULL COMMENT '标题',
  `content` LONGTEXT NULL COMMENT '内容（Markdown）',
  `category` VARCHAR(50) NULL COMMENT '分类：开发笔记/踩坑记录/想法灵感',
  `tags` VARCHAR(500) NULL COMMENT '标签（JSON数组或逗号分隔）',
  `version` INT NOT NULL DEFAULT 1 COMMENT '版本号',
  `parent_id` BIGINT NULL COMMENT '父版本ID（版本历史）',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '状态：0-草稿 1-已发布 2-已归档',
  `view_count` INT NOT NULL DEFAULT 0 COMMENT '查看次数',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `author_id` BIGINT NULL COMMENT '作者ID（逻辑关联到 user.id）',
  PRIMARY KEY (`id`),
  INDEX `idx_category` (`category`),
  INDEX `idx_status` (`status`),
  INDEX `idx_created_at` (`created_at`)
  -- 注意：不使用外键约束，author_id 通过逻辑关联到 user.id
  -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='个人知识库表';

-- 成长轨迹时间线表
CREATE TABLE IF NOT EXISTS `timeline_event` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `year` INT NOT NULL COMMENT '年份',
  `title` VARCHAR(255) NOT NULL COMMENT '标题',
  `description` TEXT NULL COMMENT '描述',
  `icon` VARCHAR(50) NULL COMMENT '图标（emoji或图标类名）',
  `color` VARCHAR(50) NULL COMMENT '颜色主题',
  `sort_order` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '状态：0-隐藏 1-显示',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_year` (`year`),
  INDEX `idx_status` (`status`),
  INDEX `idx_sort_order` (`sort_order`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='成长轨迹时间线表';

-- 访客分析扩展表
CREATE TABLE IF NOT EXISTS `visitor_analytics` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `visitor_id` VARCHAR(36) NULL COMMENT '访客ID',
  `ip` VARCHAR(45) NULL COMMENT 'IP地址',
  `country` VARCHAR(100) NULL COMMENT '国家',
  `region` VARCHAR(100) NULL COMMENT '地区',
  `city` VARCHAR(100) NULL COMMENT '城市',
  `referrer` VARCHAR(500) NULL COMMENT '来源页面',
  `search_keyword` VARCHAR(255) NULL COMMENT '搜索关键词',
  `device_type` VARCHAR(50) NULL COMMENT '设备类型：desktop/mobile/tablet',
  `browser` VARCHAR(100) NULL COMMENT '浏览器',
  `os` VARCHAR(100) NULL COMMENT '操作系统',
  `path` VARCHAR(255) NULL COMMENT '访问路径',
  `session_start` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '会话开始时间',
  `session_end` DATETIME NULL COMMENT '会话结束时间',
  `page_views` INT NOT NULL DEFAULT 1 COMMENT '页面浏览量',
  `is_online` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否在线（最近5分钟有活动）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_visitor_id` (`visitor_id`),
  INDEX `idx_ip` (`ip`),
  INDEX `idx_country` (`country`),
  INDEX `idx_path` (`path`),
  INDEX `idx_session_start` (`session_start`),
  INDEX `idx_is_online` (`is_online`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='访客分析扩展表';

