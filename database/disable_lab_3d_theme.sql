-- 禁用实验室 3D 风主题
-- 执行此脚本可以禁用数据库中已存在的 lab-3d 主题

UPDATE `theme_style` 
SET `is_enabled` = 0, 
    `updated_at` = NOW()
WHERE `code` = 'lab-3d';

-- 如果用户当前使用的是 lab-3d 主题，将其重置为默认主题
UPDATE `user_theme_preference` 
SET `theme_code` = 'default',
    `updated_at` = NOW()
WHERE `theme_code` = 'lab-3d';

