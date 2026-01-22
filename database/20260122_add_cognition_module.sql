-- ============================================
-- 添加认知说明书模块
-- 创建时间: 2026-01-22
-- ============================================

-- 插入认知说明书模块
INSERT INTO `module` (
  `module_key`, 
  `module_name`, 
  `module_version`, 
  `description`, 
  `icon`, 
  `category`, 
  `is_enabled`, 
  `is_core`, 
  `sort`, 
  `routes`, 
  `components`
) VALUES (
  'cognition',
  '认知说明书',
  '1.0.0',
  '个人认知使用说明书，记录认知系统、思维模型和使用方法',
  'fas fa-brain',
  'content',
  1,
  0,
  17,
  '["/cognition", "/cognition/[slug]"]',
  '[]'
)
ON DUPLICATE KEY UPDATE 
  `module_name` = '认知说明书',
  `description` = '个人认知使用说明书，记录认知系统、思维模型和使用方法',
  `icon` = 'fas fa-brain',
  `category` = 'content',
  `routes` = '["/cognition", "/cognition/[slug]"]',
  `updated_at` = CURRENT_TIMESTAMP;
