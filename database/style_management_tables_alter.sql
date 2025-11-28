-- 修改样式定义表的字段长度
-- 用于修复 RGBA 颜色值长度不足的问题

ALTER TABLE `style_definition` 
MODIFY COLUMN `background_color` VARCHAR(50) DEFAULT NULL COMMENT '背景颜色（RGBA格式）',
MODIFY COLUMN `border_color` VARCHAR(50) DEFAULT NULL COMMENT '边框颜色（RGBA格式）',
MODIFY COLUMN `text_color` VARCHAR(50) DEFAULT NULL COMMENT '文字颜色（HEX格式）';

