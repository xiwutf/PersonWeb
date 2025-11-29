-- ============================================
-- 工具合集示例数据插入脚本
-- 将工具合集数据插入到 tool_collection 表中
-- 注意：这些数据可以通过前端管理界面进行修改和删除
-- ============================================

-- ============================================
-- 插入工具合集数据（如果不存在）
-- ============================================
-- 注意：需要先确保 tool_collection 表存在
-- 如果表不存在，请先执行 toolbox_marketplace_tables.sql 创建表

INSERT INTO `tool_collection` (`id`, `name`, `slug`, `description`, `cover_image`, `price`, `original_price`, `tool_count`, `purchase_count`, `is_featured`, `sort_order`, `status`, `created_at`, `updated_at`) VALUES
(1, '前端开发工具包', 'frontend-dev-kit', '包含 Vue.js、Nuxt.js、TypeScript 等前端开发必备工具，一站式解决前端开发需求', NULL, 299.00, 499.00, 5, 128, 1, 1, 'published', NOW(), NOW()),
(2, '后端开发工具包', 'backend-dev-kit', '包含 .NET Core、Node.js、Entity Framework 等后端开发工具，提升开发效率', NULL, 349.00, 599.00, 6, 95, 1, 2, 'published', NOW(), NOW()),
(3, '全栈开发工具包', 'fullstack-dev-kit', '前端+后端完整工具包，适合全栈开发者，一次购买全部拥有', NULL, 599.00, 1098.00, 11, 67, 1, 3, 'published', NOW(), NOW()),
(4, 'AI 开发工具包', 'ai-dev-kit', '包含 LangChain、OpenAI API、Python 等 AI 开发工具，助力 AI 应用开发', NULL, 399.00, 699.00, 4, 52, 0, 4, 'published', NOW(), NOW()),
(5, '数据库管理工具包', 'database-toolkit', '包含 MySQL、PostgreSQL、Redis 等数据库管理工具，提升数据库操作效率', NULL, 249.00, 449.00, 3, 38, 0, 5, 'published', NOW(), NOW()),
(6, '开发效率工具包', 'productivity-toolkit', '精选开发效率工具，包含代码生成、API 测试、文档生成等实用工具', NULL, 199.00, 349.00, 7, 89, 0, 6, 'published', NOW(), NOW())
ON DUPLICATE KEY UPDATE 
    `name`=VALUES(`name`),
    `slug`=VALUES(`slug`),
    `description`=VALUES(`description`),
    `cover_image`=VALUES(`cover_image`),
    `price`=VALUES(`price`),
    `original_price`=VALUES(`original_price`),
    `tool_count`=VALUES(`tool_count`),
    `purchase_count`=VALUES(`purchase_count`),
    `is_featured`=VALUES(`is_featured`),
    `sort_order`=VALUES(`sort_order`),
    `status`=VALUES(`status`),
    `updated_at`=NOW();

-- ============================================
-- 插入工具合集关联数据（如果不存在）
-- ============================================
-- 注意：需要先确保 tool_collection_item 表存在，并且有对应的工具数据
-- 这里使用实际存在的工具ID（从 tool 表中查询，假设至少有9个工具）
-- 如果工具ID不存在，这些关联数据可能无法插入，但不影响合集数据的显示
-- 建议：先执行 remove_foreign_keys_tool_collection.sql 移除外键约束

-- 前端开发工具包（使用实际存在的工具ID，取前5个工具）
INSERT INTO `tool_collection_item` (`collection_id`, `tool_id`, `sort_order`, `created_at`)
SELECT 1, t.`id`, (@row_number := @row_number + 1), NOW()
FROM `tool` t
CROSS JOIN (SELECT @row_number := 0) r
WHERE t.`status` = 'published'
ORDER BY t.`id`
LIMIT 5
ON DUPLICATE KEY UPDATE `sort_order`=VALUES(`sort_order`);

-- 后端开发工具包（使用实际存在的工具ID，跳过前5个，取接下来6个工具）
INSERT INTO `tool_collection_item` (`collection_id`, `tool_id`, `sort_order`, `created_at`)
SELECT 2, t.`id`, (@row_number := @row_number + 1), NOW()
FROM `tool` t
CROSS JOIN (SELECT @row_number := 0) r
WHERE t.`status` = 'published'
ORDER BY t.`id`
LIMIT 5, 6
ON DUPLICATE KEY UPDATE `sort_order`=VALUES(`sort_order`);

-- 全栈开发工具包（包含前端和后端工具，使用前6个工具）
INSERT INTO `tool_collection_item` (`collection_id`, `tool_id`, `sort_order`, `created_at`)
SELECT 3, t.`id`, (@row_number := @row_number + 1), NOW()
FROM `tool` t
CROSS JOIN (SELECT @row_number := 0) r
WHERE t.`status` = 'published'
ORDER BY t.`id`
LIMIT 6
ON DUPLICATE KEY UPDATE `sort_order`=VALUES(`sort_order`);

-- AI 开发工具包（使用实际存在的工具ID，取接下来4个工具，如果不足则重复使用前面的）
INSERT INTO `tool_collection_item` (`collection_id`, `tool_id`, `sort_order`, `created_at`)
SELECT 4, t.`id`, (@row_number := @row_number + 1), NOW()
FROM `tool` t
CROSS JOIN (SELECT @row_number := 0) r
WHERE t.`status` = 'published'
ORDER BY t.`id`
LIMIT 6, 4
ON DUPLICATE KEY UPDATE `sort_order`=VALUES(`sort_order`);

-- 数据库管理工具包（使用实际存在的工具ID，取接下来3个工具，如果不足则重复使用前面的）
INSERT INTO `tool_collection_item` (`collection_id`, `tool_id`, `sort_order`, `created_at`)
SELECT 5, t.`id`, (@row_number := @row_number + 1), NOW()
FROM `tool` t
CROSS JOIN (SELECT @row_number := 0) r
WHERE t.`status` = 'published'
ORDER BY t.`id`
LIMIT 7, 3
ON DUPLICATE KEY UPDATE `sort_order`=VALUES(`sort_order`);

-- 开发效率工具包（使用实际存在的工具ID，如果不足7个，则重复使用前面的工具）
-- 这里使用前9个工具中的任意7个（循环使用）
INSERT INTO `tool_collection_item` (`collection_id`, `tool_id`, `sort_order`, `created_at`)
SELECT 6, t.`id`, (@row_number := @row_number + 1), NOW()
FROM (
    SELECT `id` FROM `tool` WHERE `status` = 'published' ORDER BY `id` LIMIT 9
) t
CROSS JOIN (SELECT @row_number := 0) r
ORDER BY t.`id`
LIMIT 7
ON DUPLICATE KEY UPDATE `sort_order`=VALUES(`sort_order`);

