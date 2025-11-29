    -- 样式管理系统数据库表结构
    -- 用于统一管理后台样式配置
    -- 
    -- 设计原则：
    -- 1. 不使用外键约束，通过逻辑关联维护表间关系
    -- 2. 关联关系由应用层维护，便于后期维护和扩展
    -- 3. 为关联字段创建索引以提升查询性能
    -- 4. 详细说明请参考 database/DESIGN_PRINCIPLES.md

    -- ============================================
    -- 1. StyleCategories 表 - 样式分类
    -- ============================================
    CREATE TABLE IF NOT EXISTS `style_category` (
        `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '分类ID',
        `name` VARCHAR(50) NOT NULL COMMENT '分类名称（如：标签、按钮、表格等）',
        `code` VARCHAR(50) NOT NULL COMMENT '分类代码（如：tag、button、table等）',
        `description` VARCHAR(255) DEFAULT NULL COMMENT '分类描述',
        `sort` INT DEFAULT 0 COMMENT '排序',
        `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
        `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
        PRIMARY KEY (`id`),
        UNIQUE KEY `uk_code` (`code`),
        INDEX `idx_sort` (`sort`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='样式分类表';

    -- ============================================
    -- 2. StyleDefinitions 表 - 样式定义
    -- ============================================
    -- 注意：根据数据库设计原则，不使用外键约束
    -- category_id 通过逻辑关联到 style_category.id，关联关系由应用层维护
    CREATE TABLE IF NOT EXISTS `style_definition` (
        `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '样式ID',
        `category_id` BIGINT NOT NULL COMMENT '分类ID（逻辑关联到 style_category.id）',
        `name` VARCHAR(100) NOT NULL COMMENT '样式名称（如：信息标签、成功标签等）',
        `code` VARCHAR(100) NOT NULL COMMENT '样式代码（如：tag-info、tag-success等）',
        `css_class` VARCHAR(100) NOT NULL COMMENT 'CSS类名',
        `background_color` VARCHAR(50) DEFAULT NULL COMMENT '背景颜色（RGBA格式）',
        `border_color` VARCHAR(50) DEFAULT NULL COMMENT '边框颜色（RGBA格式）',
        `text_color` VARCHAR(50) DEFAULT NULL COMMENT '文字颜色（HEX格式）',
        `font_size` VARCHAR(20) DEFAULT NULL COMMENT '字体大小',
        `font_weight` VARCHAR(20) DEFAULT NULL COMMENT '字体粗细',
        `padding` VARCHAR(20) DEFAULT NULL COMMENT '内边距',
        `border_radius` VARCHAR(20) DEFAULT NULL COMMENT '圆角',
        `border_width` VARCHAR(20) DEFAULT NULL COMMENT '边框宽度',
        `custom_css` TEXT DEFAULT NULL COMMENT '自定义CSS（JSON格式）',
        `description` VARCHAR(255) DEFAULT NULL COMMENT '样式描述',
        `is_active` TINYINT DEFAULT 1 COMMENT '是否启用：0-禁用 1-启用',
        `sort` INT DEFAULT 0 COMMENT '排序',
        `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
        `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
        PRIMARY KEY (`id`),
        UNIQUE KEY `uk_code` (`code`),
        INDEX `idx_category_id` (`category_id`),
        INDEX `idx_is_active` (`is_active`)
        -- 注意：不使用外键约束，category_id 通过逻辑关联到 style_category.id
        -- 关联关系由应用层维护，便于后期维护和扩展
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='样式定义表';

    -- ============================================
    -- 3. StyleUsage 表 - 样式使用统计
    -- ============================================
    -- 注意：根据数据库设计原则，不使用外键约束
    -- style_id 通过逻辑关联到 style_definition.id，关联关系由应用层维护
    CREATE TABLE IF NOT EXISTS `style_usage` (
        `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '使用记录ID',
        `style_id` BIGINT NOT NULL COMMENT '样式ID（逻辑关联到 style_definition.id）',
        `page_path` VARCHAR(255) NOT NULL COMMENT '页面路径（如：/admin/articles）',
        `component_name` VARCHAR(100) DEFAULT NULL COMMENT '组件名称（如：ArticleList）',
        `usage_count` INT DEFAULT 1 COMMENT '使用次数',
        `last_used_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '最后使用时间',
        `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
        `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
        PRIMARY KEY (`id`),
        UNIQUE KEY `uk_style_page_component` (`style_id`, `page_path`, `component_name`),
        INDEX `idx_style_id` (`style_id`),
        INDEX `idx_page_path` (`page_path`)
        -- 注意：不使用外键约束，style_id 通过逻辑关联到 style_definition.id
        -- 关联关系由应用层维护，便于后期维护和扩展
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='样式使用统计表';

    -- ============================================
    -- 初始化默认样式分类
    -- ============================================
    INSERT INTO `style_category` (`name`, `code`, `description`, `sort`) VALUES 
    ('标签', 'tag', '各种标签样式（如：信息、成功、警告、错误等）', 1),
    ('按钮', 'button', '按钮样式（如：主要、次要、链接等）', 2),
    ('表格', 'table', '表格相关样式（如：表头、单元格、行等）', 3),
    ('卡片', 'card', '卡片容器样式', 4),
    ('输入框', 'input', '输入框样式', 5),
    ('分页', 'pagination', '分页组件样式', 6),
    ('字体', 'font', '字体样式（如：菜单字体、内容字体等）', 7)
    ON DUPLICATE KEY UPDATE `name`=`name`;

    -- ============================================
    -- 初始化默认样式定义
    -- ============================================
    -- 标签样式
    INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`) 
    SELECT 
        c.id,
        '信息标签',
        'tag-info',
        'tag tag-info',
        'rgba(59, 130, 246, 0.3)',
        'rgba(59, 130, 246, 0.6)',
        '#bfdbfe',
        '0.75rem',
        '500',
        '0.25rem 0.5rem',
        '0.25rem',
        '1px',
        '信息类标签样式',
        1
    FROM `style_category` c WHERE c.code = 'tag'
    ON DUPLICATE KEY UPDATE 
        `name` = VALUES(`name`),
        `css_class` = VALUES(`css_class`),
        `background_color` = VALUES(`background_color`),
        `border_color` = VALUES(`border_color`),
        `text_color` = VALUES(`text_color`),
        `font_size` = VALUES(`font_size`),
        `font_weight` = VALUES(`font_weight`),
        `padding` = VALUES(`padding`),
        `border_radius` = VALUES(`border_radius`),
        `border_width` = VALUES(`border_width`),
        `description` = VALUES(`description`),
        `sort` = VALUES(`sort`);

    INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`) 
    SELECT 
        c.id,
        '成功标签',
        'tag-success',
        'tag tag-success',
        'rgba(34, 197, 94, 0.3)',
        'rgba(34, 197, 94, 0.6)',
        '#a7f3d0',
        '0.75rem',
        '500',
        '0.25rem 0.5rem',
        '0.25rem',
        '1px',
        '成功类标签样式',
        2
    FROM `style_category` c WHERE c.code = 'tag'
    ON DUPLICATE KEY UPDATE 
        `name` = VALUES(`name`),
        `css_class` = VALUES(`css_class`),
        `background_color` = VALUES(`background_color`),
        `border_color` = VALUES(`border_color`),
        `text_color` = VALUES(`text_color`),
        `font_size` = VALUES(`font_size`),
        `font_weight` = VALUES(`font_weight`),
        `padding` = VALUES(`padding`),
        `border_radius` = VALUES(`border_radius`),
        `border_width` = VALUES(`border_width`),
        `description` = VALUES(`description`),
        `sort` = VALUES(`sort`);

    INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`) 
    SELECT 
        c.id,
        '警告标签',
        'tag-warning',
        'tag tag-warning',
        'rgba(251, 191, 36, 0.3)',
        'rgba(251, 191, 36, 0.6)',
        '#fde68a',
        '0.75rem',
        '500',
        '0.25rem 0.5rem',
        '0.25rem',
        '1px',
        '警告类标签样式',
        3
    FROM `style_category` c WHERE c.code = 'tag'
    ON DUPLICATE KEY UPDATE 
        `name` = VALUES(`name`),
        `css_class` = VALUES(`css_class`),
        `background_color` = VALUES(`background_color`),
        `border_color` = VALUES(`border_color`),
        `text_color` = VALUES(`text_color`),
        `font_size` = VALUES(`font_size`),
        `font_weight` = VALUES(`font_weight`),
        `padding` = VALUES(`padding`),
        `border_radius` = VALUES(`border_radius`),
        `border_width` = VALUES(`border_width`),
        `description` = VALUES(`description`),
        `sort` = VALUES(`sort`);

    INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`) 
    SELECT 
        c.id,
        '错误标签',
        'tag-error',
        'tag tag-error',
        'rgba(239, 68, 68, 0.3)',
        'rgba(239, 68, 68, 0.6)',
        '#fecaca',
        '0.75rem',
        '500',
        '0.25rem 0.5rem',
        '0.25rem',
        '1px',
        '错误类标签样式',
        4
    FROM `style_category` c WHERE c.code = 'tag'
    ON DUPLICATE KEY UPDATE 
        `name` = VALUES(`name`),
        `css_class` = VALUES(`css_class`),
        `background_color` = VALUES(`background_color`),
        `border_color` = VALUES(`border_color`),
        `text_color` = VALUES(`text_color`),
        `font_size` = VALUES(`font_size`),
        `font_weight` = VALUES(`font_weight`),
        `padding` = VALUES(`padding`),
        `border_radius` = VALUES(`border_radius`),
        `border_width` = VALUES(`border_width`),
        `description` = VALUES(`description`),
        `sort` = VALUES(`sort`);

    INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`) 
    SELECT 
        c.id,
        '默认标签',
        'tag-default',
        'tag tag-default',
        'rgba(255, 255, 255, 0.15)',
        'rgba(255, 255, 255, 0.3)',
        'rgba(255, 255, 255, 0.9)',
        '0.75rem',
        '500',
        '0.25rem 0.5rem',
        '0.25rem',
        '1px',
        '默认标签样式',
        5
    FROM `style_category` c WHERE c.code = 'tag'
    ON DUPLICATE KEY UPDATE 
        `name` = VALUES(`name`),
        `css_class` = VALUES(`css_class`),
        `background_color` = VALUES(`background_color`),
        `border_color` = VALUES(`border_color`),
        `text_color` = VALUES(`text_color`),
        `font_size` = VALUES(`font_size`),
        `font_weight` = VALUES(`font_weight`),
        `padding` = VALUES(`padding`),
        `border_radius` = VALUES(`border_radius`),
        `border_width` = VALUES(`border_width`),
        `description` = VALUES(`description`),
        `sort` = VALUES(`sort`);

