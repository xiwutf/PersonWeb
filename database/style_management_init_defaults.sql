-- 样式管理系统 - 初始化默认样式数据
-- 为所有分类初始化常用的默认样式

-- ============================================
-- 按钮样式 (Button Styles)
-- ============================================
INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '主要按钮',
    'button-primary',
    'btn btn-primary',
    'rgba(59, 130, 246, 1)',
    'rgba(59, 130, 246, 1)',
    '#ffffff',
    '0.875rem',
    '500',
    '0.5rem 1rem',
    '0.375rem',
    '1px',
    '主要操作按钮样式',
    1,
    1
FROM `style_category` c WHERE c.code = 'button'
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

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '次要按钮',
    'button-secondary',
    'btn btn-secondary',
    'rgba(255, 255, 255, 0.1)',
    'rgba(255, 255, 255, 0.2)',
    'rgba(255, 255, 255, 0.9)',
    '0.875rem',
    '500',
    '0.5rem 1rem',
    '0.375rem',
    '1px',
    '次要操作按钮样式',
    2,
    1
FROM `style_category` c WHERE c.code = 'button'
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

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '成功按钮',
    'button-success',
    'btn btn-success',
    'rgba(34, 197, 94, 1)',
    'rgba(34, 197, 94, 1)',
    '#ffffff',
    '0.875rem',
    '500',
    '0.5rem 1rem',
    '0.375rem',
    '1px',
    '成功操作按钮样式',
    3,
    1
FROM `style_category` c WHERE c.code = 'button'
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

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '危险按钮',
    'button-danger',
    'btn btn-danger',
    'rgba(239, 68, 68, 1)',
    'rgba(239, 68, 68, 1)',
    '#ffffff',
    '0.875rem',
    '500',
    '0.5rem 1rem',
    '0.375rem',
    '1px',
    '危险操作按钮样式',
    4,
    1
FROM `style_category` c WHERE c.code = 'button'
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

-- ============================================
-- 表格样式 (Table Styles)
-- ============================================
INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '表格表头',
    'table-header',
    'table-header',
    'rgba(255, 255, 255, 0.05)',
    'rgba(255, 255, 255, 0.1)',
    'rgba(255, 255, 255, 0.9)',
    '0.875rem',
    '600',
    '0.75rem 1rem',
    '0',
    '1px',
    '表格表头样式',
    1,
    1
FROM `style_category` c WHERE c.code = 'table'
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

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '表格单元格',
    'table-cell',
    'table-cell',
    'transparent',
    'rgba(255, 255, 255, 0.1)',
    'rgba(255, 255, 255, 0.8)',
    '0.875rem',
    '400',
    '0.75rem 1rem',
    '0',
    '1px',
    '表格单元格样式',
    2,
    1
FROM `style_category` c WHERE c.code = 'table'
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

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '表格行悬停',
    'table-row-hover',
    'table-row-hover',
    'rgba(255, 255, 255, 0.05)',
    'rgba(255, 255, 255, 0.1)',
    'rgba(255, 255, 255, 0.9)',
    '0.875rem',
    '400',
    '0.75rem 1rem',
    '0',
    '1px',
    '表格行悬停样式',
    3,
    1
FROM `style_category` c WHERE c.code = 'table'
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

-- ============================================
-- 卡片样式 (Card Styles)
-- ============================================
INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '默认卡片',
    'card-default',
    'card',
    'rgba(255, 255, 255, 0.05)',
    'rgba(255, 255, 255, 0.1)',
    'rgba(255, 255, 255, 0.9)',
    '1rem',
    '400',
    '1.5rem',
    '0.5rem',
    '1px',
    '默认卡片容器样式',
    1,
    1
FROM `style_category` c WHERE c.code = 'card'
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

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '玻璃拟态卡片',
    'card-glass',
    'card card-glass',
    'rgba(255, 255, 255, 0.1)',
    'rgba(255, 255, 255, 0.2)',
    'rgba(255, 255, 255, 0.9)',
    '1rem',
    '400',
    '1.5rem',
    '1rem',
    '1px',
    '玻璃拟态效果卡片样式',
    2,
    1
FROM `style_category` c WHERE c.code = 'card'
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

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '悬停卡片',
    'card-hover',
    'card card-hover',
    'rgba(255, 255, 255, 0.05)',
    'rgba(255, 255, 255, 0.1)',
    'rgba(255, 255, 255, 0.9)',
    '1rem',
    '400',
    '1.5rem',
    '0.5rem',
    '1px',
    '带悬停效果的卡片样式',
    3,
    1
FROM `style_category` c WHERE c.code = 'card'
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

-- ============================================
-- 输入框样式 (Input Styles)
-- ============================================
INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '默认输入框',
    'input-default',
    'input input-default',
    'rgba(255, 255, 255, 0.1)',
    'rgba(255, 255, 255, 0.2)',
    'rgba(255, 255, 255, 0.9)',
    '0.875rem',
    '400',
    '0.5rem 0.75rem',
    '0.375rem',
    '1px',
    '默认输入框样式',
    1,
    1
FROM `style_category` c WHERE c.code = 'input'
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

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '聚焦输入框',
    'input-focus',
    'input input-focus',
    'rgba(255, 255, 255, 0.15)',
    'rgba(59, 130, 246, 0.6)',
    'rgba(255, 255, 255, 0.9)',
    '0.875rem',
    '400',
    '0.5rem 0.75rem',
    '0.375rem',
    '2px',
    '聚焦状态的输入框样式',
    2,
    1
FROM `style_category` c WHERE c.code = 'input'
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

-- ============================================
-- 分页样式 (Pagination Styles)
-- ============================================
INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '分页按钮',
    'pagination-button',
    'pagination-button',
    'rgba(255, 255, 255, 0.1)',
    'rgba(255, 255, 255, 0.2)',
    'rgba(255, 255, 255, 0.9)',
    '0.875rem',
    '500',
    '0.5rem 0.75rem',
    '0.375rem',
    '1px',
    '分页按钮样式',
    1,
    1
FROM `style_category` c WHERE c.code = 'pagination'
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

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '分页激活',
    'pagination-active',
    'pagination-active',
    'rgba(59, 130, 246, 1)',
    'rgba(59, 130, 246, 1)',
    '#ffffff',
    '0.875rem',
    '600',
    '0.5rem 0.75rem',
    '0.375rem',
    '1px',
    '分页激活状态样式',
    2,
    1
FROM `style_category` c WHERE c.code = 'pagination'
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

-- ============================================
-- 字体样式 (Font Styles)
-- ============================================
INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '标题字体',
    'font-heading',
    'font-heading',
    NULL,
    NULL,
    'rgba(255, 255, 255, 0.95)',
    '1.5rem',
    '700',
    NULL,
    NULL,
    NULL,
    '标题字体样式',
    1,
    1
FROM `style_category` c WHERE c.code = 'font'
ON DUPLICATE KEY UPDATE 
    `name` = VALUES(`name`),
    `css_class` = VALUES(`css_class`),
    `text_color` = VALUES(`text_color`),
    `font_size` = VALUES(`font_size`),
    `font_weight` = VALUES(`font_weight`),
    `description` = VALUES(`description`),
    `sort` = VALUES(`sort`);

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '正文字体',
    'font-body',
    'font-body',
    NULL,
    NULL,
    'rgba(255, 255, 255, 0.8)',
    '1rem',
    '400',
    NULL,
    NULL,
    NULL,
    '正文字体样式',
    2,
    1
FROM `style_category` c WHERE c.code = 'font'
ON DUPLICATE KEY UPDATE 
    `name` = VALUES(`name`),
    `css_class` = VALUES(`css_class`),
    `text_color` = VALUES(`text_color`),
    `font_size` = VALUES(`font_size`),
    `font_weight` = VALUES(`font_weight`),
    `description` = VALUES(`description`),
    `sort` = VALUES(`sort`);

INSERT INTO `style_definition` (`category_id`, `name`, `code`, `css_class`, `background_color`, `border_color`, `text_color`, `font_size`, `font_weight`, `padding`, `border_radius`, `border_width`, `description`, `sort`, `is_active`) 
SELECT 
    c.id,
    '小号字体',
    'font-small',
    'font-small',
    NULL,
    NULL,
    'rgba(255, 255, 255, 0.6)',
    '0.875rem',
    '400',
    NULL,
    NULL,
    NULL,
    '小号字体样式',
    3,
    1
FROM `style_category` c WHERE c.code = 'font'
ON DUPLICATE KEY UPDATE 
    `name` = VALUES(`name`),
    `css_class` = VALUES(`css_class`),
    `text_color` = VALUES(`text_color`),
    `font_size` = VALUES(`font_size`),
    `font_weight` = VALUES(`font_weight`),
    `description` = VALUES(`description`),
    `sort` = VALUES(`sort`);

