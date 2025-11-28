-- 样式管理系统修复脚本
-- 解决字段长度不足和初始化数据问题

-- ============================================
-- 1. 修改字段长度（如果表已存在）
-- ============================================
ALTER TABLE `style_definition` 
MODIFY COLUMN `background_color` VARCHAR(50) DEFAULT NULL COMMENT '背景颜色（RGBA格式）',
MODIFY COLUMN `border_color` VARCHAR(50) DEFAULT NULL COMMENT '边框颜色（RGBA格式）',
MODIFY COLUMN `text_color` VARCHAR(50) DEFAULT NULL COMMENT '文字颜色（HEX格式）';

-- ============================================
-- 2. 重新插入样式数据（如果之前插入失败）
-- ============================================

-- 删除可能存在的旧数据（可选，如果数据已存在且需要更新）
-- DELETE FROM `style_definition` WHERE `code` IN ('tag-info', 'tag-success', 'tag-warning', 'tag-error', 'tag-default');

-- 插入信息标签
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

-- 插入成功标签
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

-- 插入警告标签
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

-- 插入错误标签
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

-- 插入默认标签
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

