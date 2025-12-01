-- ============================================
-- 网站内容数据丰富脚本
-- 插入更多文章、项目、工具等示例数据
-- 执行前请确保相关表已创建
-- ============================================

-- ============================================
-- 1. 确保有分类数据
-- ============================================
INSERT INTO `category` (`name`, `slug`, `sort`) VALUES
('技术博客', 'tech', 1),
('生活随笔', 'life', 2),
('项目分享', 'project', 3),
('工具推荐', 'tool', 4),
('AI探索', 'ai', 5)
ON DUPLICATE KEY UPDATE `name`=VALUES(`name`), `slug`=VALUES(`slug`), `sort`=VALUES(`sort`);

-- ============================================
-- 2. 插入技术博客文章
-- ============================================
INSERT INTO `article` (
    `title`, `slug`, `summary`, `content_md`, `content_html`,
    `category_id`, `status`, `publish_time`, `view_count`, `created_at`, `updated_at`
) VALUES
(
    'Vue 3 Composition API 深度解析',
    'vue3-composition-api-deep-dive',
    '深入探讨 Vue 3 Composition API 的设计理念、使用场景和最佳实践，帮助你更好地理解和使用这个强大的特性。',
    '# Vue 3 Composition API 深度解析\n\n## 什么是 Composition API\n\nComposition API 是 Vue 3 引入的一套新的 API，它允许我们使用函数式的方式来组织组件逻辑。\n\n## 核心优势\n\n1. **更好的逻辑复用**：通过组合函数，可以轻松地在多个组件间共享逻辑\n2. **更好的类型推导**：TypeScript 支持更加完善\n3. **更灵活的组织方式**：可以按照逻辑而非选项来组织代码\n\n## 基本使用\n\n```javascript\nimport { ref, computed, watch } from \'vue\'\n\nexport default {\n  setup() {\n    const count = ref(0)\n    const doubleCount = computed(() => count.value * 2)\n    \n    watch(count, (newVal) => {\n      console.log(\'count changed:\', newVal)\n    })\n    \n    return { count, doubleCount }\n  }\n}\n```\n\n## 最佳实践\n\n- 使用 `setup` 语法糖简化代码\n- 合理使用 `ref` 和 `reactive`\n- 通过组合式函数提取可复用逻辑',
    '<h1>Vue 3 Composition API 深度解析</h1><p>深入探讨 Vue 3 Composition API 的设计理念、使用场景和最佳实践。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    '2024-12-15 10:00:00',
    156,
    NOW(),
    NOW()
),
(
    'Nuxt 3 服务端渲染性能优化实战',
    'nuxt3-ssr-performance',
    '分享在 Nuxt 3 项目中优化服务端渲染性能的实践经验，包括代码分割、缓存策略、资源优化等技巧。',
    '# Nuxt 3 服务端渲染性能优化实战\n\n## 性能优化的重要性\n\n在 SSR 应用中，首屏加载速度直接影响用户体验和 SEO 效果。\n\n## 优化策略\n\n### 1. 代码分割\n\n使用动态导入减少初始包大小：\n\n```javascript\nconst HeavyComponent = () => import(\'~/components/HeavyComponent.vue\')\n```\n\n### 2. 缓存策略\n\n- 页面级缓存\n- 组件级缓存\n- API 响应缓存\n\n### 3. 资源优化\n\n- 图片懒加载\n- 字体子集化\n- CSS 优化',
    '<h1>Nuxt 3 服务端渲染性能优化实战</h1><p>分享在 Nuxt 3 项目中优化服务端渲染性能的实践经验。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    '2024-12-10 14:30:00',
    203,
    NOW(),
    NOW()
),
(
    'TypeScript 在大型项目中的应用实践',
    'typescript-in-large-projects',
    '探讨如何在大型前端项目中有效使用 TypeScript，包括类型设计、模块组织、工具链配置等。',
    '# TypeScript 在大型项目中的应用实践\n\n## 为什么选择 TypeScript\n\n- 类型安全\n- 更好的 IDE 支持\n- 重构更安全\n\n## 类型设计原则\n\n1. 优先使用接口而非类型别名\n2. 合理使用泛型\n3. 避免过度使用 `any`\n\n## 项目组织\n\n- 类型定义集中管理\n- 使用命名空间组织相关类型\n- 导出类型而非实现',
    '<h1>TypeScript 在大型项目中的应用实践</h1><p>探讨如何在大型前端项目中有效使用 TypeScript。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    '2024-12-05 09:15:00',
    178,
    NOW(),
    NOW()
),
(
    'Tailwind CSS 设计系统构建指南',
    'tailwind-design-system',
    '如何使用 Tailwind CSS 构建可扩展的设计系统，包括主题配置、组件抽象、工具类扩展等。',
    '# Tailwind CSS 设计系统构建指南\n\n## 设计系统的重要性\n\n统一的设计系统可以提升开发效率和用户体验。\n\n## 配置主题\n\n在 `tailwind.config.js` 中定义设计令牌：\n\n```javascript\nmodule.exports = {\n  theme: {\n    extend: {\n      colors: {\n        primary: {\n          50: \'#f0f9ff\',\n          // ...\n        }\n      }\n    }\n  }\n}\n```\n\n## 组件抽象\n\n将常用的样式组合抽象为组件，减少重复代码。',
    '<h1>Tailwind CSS 设计系统构建指南</h1><p>如何使用 Tailwind CSS 构建可扩展的设计系统。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    '2024-11-28 16:00:00',
    142,
    NOW(),
    NOW()
),
(
    'Entity Framework Core 性能优化技巧',
    'ef-core-performance',
    '分享在使用 Entity Framework Core 时提升查询性能的实用技巧，包括查询优化、批量操作、缓存策略等。',
    '# Entity Framework Core 性能优化技巧\n\n## 常见性能问题\n\n- N+1 查询问题\n- 过度加载数据\n- 缺少索引\n\n## 优化方法\n\n### 1. 使用 Include 预加载\n\n```csharp\nvar blogs = context.Blogs\n    .Include(b => b.Posts)\n    .ToList();\n```\n\n### 2. 使用 AsNoTracking\n\n对于只读查询，使用 `AsNoTracking()` 可以提升性能。\n\n### 3. 批量操作\n\n使用 `AddRange` 和 `SaveChanges` 批量插入数据。',
    '<h1>Entity Framework Core 性能优化技巧</h1><p>分享在使用 Entity Framework Core 时提升查询性能的实用技巧。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    '2024-11-20 11:30:00',
    189,
    NOW(),
    NOW()
),
(
    'AI 辅助编程：从 Copilot 到自定义 Agent',
    'ai-assisted-programming',
    '探索 AI 在编程领域的应用，从 GitHub Copilot 到自定义 AI Agent，分享提升开发效率的实践。',
    '# AI 辅助编程：从 Copilot 到自定义 Agent\n\n## AI 编程工具的发展\n\n从代码补全到智能 Agent，AI 正在改变我们的编程方式。\n\n## 工具对比\n\n- **GitHub Copilot**：代码补全和生成\n- **Cursor**：AI 驱动的编辑器\n- **自定义 Agent**：针对特定场景的 AI 助手\n\n## 实践建议\n\n1. 合理使用 AI 工具，不要过度依赖\n2. 理解生成的代码，确保质量\n3. 针对项目特点定制 AI Agent',
    '<h1>AI 辅助编程：从 Copilot 到自定义 Agent</h1><p>探索 AI 在编程领域的应用。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'ai' LIMIT 1),
    1,
    '2024-12-01 13:00:00',
    267,
    NOW(),
    NOW()
)
ON DUPLICATE KEY UPDATE 
    `title`=VALUES(`title`),
    `summary`=VALUES(`summary`),
    `content_md`=VALUES(`content_md`),
    `content_html`=VALUES(`content_html`),
    `view_count`=VALUES(`view_count`);

-- ============================================
-- 3. 插入生活随笔文章
-- ============================================
INSERT INTO `article` (
    `title`, `slug`, `summary`, `content_md`, `content_html`,
    `category_id`, `status`, `publish_time`, `view_count`, `created_at`, `updated_at`
) VALUES
(
    '2024 年终总结：技术成长与生活感悟',
    '2024-year-end-summary',
    '回顾 2024 年的技术学习和生活经历，分享成长路上的收获与思考。',
    '# 2024 年终总结：技术成长与生活感悟\n\n## 技术成长\n\n这一年，我深入学习了 Vue 3、Nuxt 3 等前端技术，也探索了 AI 在开发中的应用。\n\n## 生活感悟\n\n工作之余，学会了更好地平衡工作与生活，找到了属于自己的节奏。\n\n## 展望未来\n\n2025 年，希望能在 AI 应用开发领域有更深的探索，同时保持对技术的热情。',
    '<h1>2024 年终总结：技术成长与生活感悟</h1><p>回顾 2024 年的技术学习和生活经历。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'life' LIMIT 1),
    1,
    '2024-12-20 10:00:00',
    89,
    NOW(),
    NOW()
),
(
    '阅读的力量：从技术文档到人文书籍',
    'reading-power',
    '分享从只读技术文档到涉猎人文书籍的阅读转变，以及阅读带来的思考和成长。',
    '# 阅读的力量：从技术文档到人文书籍\n\n## 阅读的转变\n\n从只关注技术文档，到慢慢开始阅读文学、历史、哲学类书籍。\n\n## 阅读的收获\n\n阅读让我看到了更广阔的世界，也让我在代码之外找到了另一种表达方式。\n\n## 推荐书单\n\n- 《代码大全》\n- 《人月神话》\n- 《黑客与画家》',
    '<h1>阅读的力量：从技术文档到人文书籍</h1><p>分享从只读技术文档到涉猎人文书籍的阅读转变。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'life' LIMIT 1),
    1,
    '2024-11-15 15:00:00',
    124,
    NOW(),
    NOW()
)
ON DUPLICATE KEY UPDATE 
    `title`=VALUES(`title`),
    `summary`=VALUES(`summary`),
    `content_md`=VALUES(`content_md`),
    `content_html`=VALUES(`content_html`);

-- ============================================
-- 4. 插入项目数据（已移除与 complete_content_data.sql 重复的项目）
-- ============================================
-- 注意：以下项目已在 complete_content_data.sql 中存在，已移除：
-- - "个人网站 V2"（与 "个人数字资产平台（本网站）" 重复）
-- - "访客分析系统"（与 "访客分析系统（Analytics）" 重复）
-- 
-- 注意：根据表结构，字段名可能大小写敏感，请根据实际表结构调整
INSERT INTO `projects` (
    `Id`, `Title`, `Description`, `CoverUrl`, `DemoUrl`, `GithubUrl`,
    `Status`, `TechStack`, `Content`, `CreatedAt`, `UpdatedAt`
) VALUES
(
    UUID(),
    'AI 创作助手',
    '基于 OpenAI API 开发的 AI 创作工具，支持文章生成、代码优化、翻译等多种功能。',
    NULL,
    NULL,
    'https://github.com/xifg/ai-assistant',
    'Active',
    '["Vue 3", "TypeScript", "OpenAI API", "Node.js"]',
    '# AI 创作助手\n\n## 项目简介\n\n一个功能丰富的 AI 创作工具，帮助提升创作效率。\n\n## 主要功能\n\n- 文章生成\n- 代码优化\n- 多语言翻译\n- 内容改写',
    NOW(),
    NOW()
),
(
    UUID(),
    '工具市场平台',
    '一个工具分享和交易平台，用户可以发布、购买和使用各种实用工具。',
    NULL,
    NULL,
    NULL,
    'Active',
    '["Nuxt 3", ".NET Core", "MySQL", "Redis"]',
    '# 工具市场平台\n\n## 项目简介\n\n一个工具分享和交易平台。\n\n## 主要功能\n\n- 工具发布\n- 工具购买\n- 使用统计\n- 评价系统',
    NOW(),
    NOW()
),
(
    UUID(),
    '主题商店',
    '网站主题展示和交易平台，用户可以浏览、购买和下载各种主题。',
    NULL,
    NULL,
    NULL,
    'Active',
    '["Nuxt 3", ".NET Core", "MySQL"]',
    '# 主题商店\n\n## 项目简介\n\n网站主题展示和交易平台。\n\n## 主要功能\n\n- 主题展示\n- 主题购买\n- 主题管理',
    DATE_SUB(NOW(), INTERVAL 60 DAY),
    NOW()
)
ON DUPLICATE KEY UPDATE 
    `Title`=VALUES(`Title`),
    `Description`=VALUES(`Description`),
    `Status`=VALUES(`Status`);

-- ============================================
-- 5. 插入工具数据（如果工具表存在）
-- ============================================
-- 注意：需要先确保 tool_category 表有数据
INSERT INTO `tool` (
    `name`, `slug`, `icon`, `description`, `detailed_description`,
    `price`, `is_free`, `status`, `tags`, `created_at`, `updated_at`
) VALUES
(
    'Markdown 转 HTML',
    'markdown-to-html',
    '📝',
    '快速将 Markdown 文本转换为 HTML 格式，支持多种 Markdown 语法。',
    '## 功能特点\n\n- 支持标准 Markdown 语法\n- 支持代码高亮\n- 支持表格、列表等复杂格式\n- 实时预览\n\n## 使用说明\n\n1. 输入或粘贴 Markdown 文本\n2. 实时查看 HTML 预览\n3. 复制生成的 HTML 代码',
    0,
    1,
    'published',
    '["工具", "Markdown", "转换"]',
    NOW(),
    NOW()
),
(
    'JSON 格式化工具',
    'json-formatter',
    '🔧',
    '在线 JSON 格式化、验证和美化工具，支持压缩、展开、验证等功能。',
    '## 功能特点\n\n- JSON 格式化\n- JSON 验证\n- JSON 压缩\n- 语法高亮\n\n## 使用说明\n\n1. 输入或粘贴 JSON 文本\n2. 点击格式化\n3. 查看格式化结果',
    0,
    1,
    'published',
    '["工具", "JSON", "格式化"]',
    NOW(),
    NOW()
),
(
    'Base64 编解码工具',
    'base64-encoder',
    '🔐',
    '快速进行 Base64 编码和解码，支持文本和图片。',
    '## 功能特点\n\n- Base64 编码\n- Base64 解码\n- 支持文本和图片\n- 批量处理\n\n## 使用说明\n\n1. 选择编码或解码\n2. 输入内容\n3. 获取结果',
    0,
    1,
    'published',
    '["工具", "Base64", "编码"]',
    NOW(),
    NOW()
),
(
    '颜色选择器',
    'color-picker',
    '🎨',
    '强大的颜色选择工具，支持多种颜色格式和调色板。',
    '## 功能特点\n\n- 多种颜色格式（HEX、RGB、HSL）\n- 调色板功能\n- 颜色历史记录\n- 颜色对比\n\n## 使用说明\n\n1. 选择颜色\n2. 查看不同格式\n3. 复制颜色值',
    0,
    1,
    'published',
    '["工具", "颜色", "设计"]',
    NOW(),
    NOW()
),
(
    '二维码生成器',
    'qr-code-generator',
    '📱',
    '快速生成二维码，支持文本、链接、WiFi 等多种类型。',
    '## 功能特点\n\n- 多种内容类型\n- 自定义样式\n- 批量生成\n- 下载功能\n\n## 使用说明\n\n1. 选择内容类型\n2. 输入内容\n3. 生成并下载',
    0,
    1,
    'published',
    '["工具", "二维码", "生成"]',
    NOW(),
    NOW()
),
(
    '时间戳转换工具',
    'timestamp-converter',
    '⏰',
    'Unix 时间戳与日期时间相互转换，支持多种时区。',
    '## 功能特点\n\n- 时间戳转日期\n- 日期转时间戳\n- 多时区支持\n- 格式化选项\n\n## 使用说明\n\n1. 输入时间戳或日期\n2. 选择时区\n3. 查看转换结果',
    0,
    1,
    'published',
    '["工具", "时间戳", "转换"]',
    NOW(),
    NOW()
),
(
    '文本差异对比工具',
    'text-diff',
    '📊',
    '对比两个文本的差异，高亮显示新增、删除和修改的内容。',
    '## 功能特点\n\n- 文本差异对比\n- 高亮显示差异\n- 行号显示\n- 导出结果\n\n## 使用说明\n\n1. 输入两个文本\n2. 点击对比\n3. 查看差异结果',
    0,
    1,
    'published',
    '["工具", "对比", "文本"]',
    NOW(),
    NOW()
),
(
    'URL 参数解析工具',
    'url-parser',
    '🔗',
    '解析 URL 参数，支持参数提取、修改和重新生成 URL。',
    '## 功能特点\n\n- URL 参数解析\n- 参数编辑\n- URL 生成\n- 参数验证\n\n## 使用说明\n\n1. 输入 URL\n2. 查看解析结果\n3. 编辑参数\n4. 生成新 URL',
    0,
    1,
    'published',
    '["工具", "URL", "解析"]',
    NOW(),
    NOW()
)
ON DUPLICATE KEY UPDATE 
    `name`=VALUES(`name`),
    `description`=VALUES(`description`),
    `detailed_description`=VALUES(`detailed_description`);

-- ============================================
-- 6. 验证数据插入
-- ============================================
SELECT '文章数量' AS '类型', COUNT(*) AS '数量' FROM `article` WHERE `status` = 1
UNION ALL
SELECT '项目数量', COUNT(*) FROM `projects` WHERE `Status` = 'Active'
UNION ALL
SELECT '工具数量', COUNT(*) FROM `tool` WHERE `status` = 'published';

