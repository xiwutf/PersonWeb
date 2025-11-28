-- ============================================
-- 示例数据插入脚本
-- 数据库: personal_site
-- 用途: 为各个数据表添加示例数据，方便开发和测试
-- ============================================

-- ============================================
-- 1. 分类表 (category) - 如果还没有数据
-- ============================================
INSERT INTO `category` (`name`, `slug`, `sort`) VALUES 
('技术文章', 'tech', 1),
('生活随笔', 'life', 2),
('项目总结', 'project', 3),
('学习笔记', 'study', 4),
('工具推荐', 'tools', 5)
ON DUPLICATE KEY UPDATE `name`=`name`;

-- ============================================
-- 2. 文章表 (article) - 插入示例文章
-- ============================================
-- 注意：需要先有分类数据，category_id 需要对应实际的分类ID
INSERT INTO `article` (`title`, `slug`, `summary`, `content_md`, `content_html`, `category_id`, `status`, `publish_time`, `view_count`, `version`, `parent_id`) VALUES 
(
    '欢迎来到我的个人网站',
    'welcome-to-my-site',
    '这是我的个人网站，记录技术学习、项目经验和生活感悟。',
    '# 欢迎来到我的个人网站\n\n这是我的个人网站，记录技术学习、项目经验和生活感悟。\n\n## 关于我\n\n我是一名全栈开发者，热爱编程和技术分享。\n\n## 网站功能\n\n- 📝 技术文章\n- 🚀 项目展示\n- 📚 知识库\n- 💬 时间胶囊',
    '<h1>欢迎来到我的个人网站</h1><p>这是我的个人网站，记录技术学习、项目经验和生活感悟。</p><h2>关于我</h2><p>我是一名全栈开发者，热爱编程和技术分享。</p><h2>网站功能</h2><ul><li>📝 技术文章</li><li>🚀 项目展示</li><li>📚 知识库</li><li>💬 时间胶囊</li></ul>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    NOW(),
    128,
    1,
    NULL
),
(
    'Vue 3 Composition API 使用指南',
    'vue3-composition-api-guide',
    '详细介绍 Vue 3 Composition API 的使用方法和最佳实践。',
    '# Vue 3 Composition API 使用指南\n\n## 什么是 Composition API\n\nComposition API 是 Vue 3 引入的新特性，提供了更好的逻辑复用和代码组织方式。\n\n## 基本用法\n\n```javascript\nimport { ref, computed, onMounted } from ''vue''\n\nexport default {\n  setup() {\n    const count = ref(0)\n    const doubleCount = computed(() => count.value * 2)\n    \n    onMounted(() => {\n      console.log(''组件已挂载'')\n    })\n    \n    return { count, doubleCount }\n  }\n}\n```',
    '<h1>Vue 3 Composition API 使用指南</h1><p>详细介绍 Vue 3 Composition API 的使用方法和最佳实践。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    DATE_SUB(NOW(), INTERVAL 5 DAY),
    256,
    1,
    NULL
),
(
    'Nuxt 3 项目部署实战',
    'nuxt3-deployment',
    '从零开始部署 Nuxt 3 项目到生产环境的完整流程。',
    '# Nuxt 3 项目部署实战\n\n## 准备工作\n\n1. 构建项目\n2. 配置服务器\n3. 设置 Nginx\n\n## 部署步骤\n\n详细步骤...',
    '<h1>Nuxt 3 项目部署实战</h1><p>从零开始部署 Nuxt 3 项目到生产环境的完整流程。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    DATE_SUB(NOW(), INTERVAL 10 DAY),
    512,
    1,
    NULL
)
ON DUPLICATE KEY UPDATE `title`=`title`;

-- ============================================
-- 3. 项目表 (Projects) - 插入示例项目
-- ============================================
INSERT INTO `Projects` (`Id`, `Title`, `Description`, `CoverUrl`, `DemoUrl`, `GithubUrl`, `Status`, `TechStack`, `Content`, `ViewCount`) VALUES 
(
    UUID(),
    '个人网站系统',
    '基于 Nuxt 3 和 .NET 8 开发的个人网站，包含文章管理、项目展示、知识库等功能。',
    'https://via.placeholder.com/800x400',
    'https://xifg.com.cn',
    'https://github.com/yourusername/personweb',
    'Active',
    '["Nuxt 3", "Vue 3", ".NET 8", "MySQL", "Tailwind CSS"]',
    '# 个人网站系统\n\n## 项目简介\n\n这是一个功能完整的个人网站系统，包含以下特性：\n\n- 📝 文章管理系统\n- 🚀 项目展示\n- 📚 知识库\n- 💬 时间胶囊\n- 📊 数据统计\n\n## 技术栈\n\n- 前端：Nuxt 3 + Vue 3 + Tailwind CSS\n- 后端：.NET 8 WebAPI\n- 数据库：MySQL\n- 部署：Nginx + PM2',
    1024
),
(
    UUID(),
    'AI 智能助手',
    '基于大语言模型的智能助手系统，支持多轮对话、知识问答等功能。',
    'https://via.placeholder.com/800x400',
    NULL,
    'https://github.com/yourusername/ai-assistant',
    'Active',
    '["Python", "FastAPI", "OpenAI API", "React"]',
    '# AI 智能助手\n\n## 项目简介\n\n基于大语言模型的智能助手系统。\n\n## 功能特性\n\n- 多轮对话\n- 知识问答\n- 代码生成\n- 文本总结',
    512
),
(
    UUID(),
    '投资管理系统',
    '个人投资记录和统计分析系统，支持股票、基金等多种投资类型。',
    'https://via.placeholder.com/800x400',
    NULL,
    NULL,
    'Completed',
    '["Vue 3", "Node.js", "MySQL", "Chart.js"]',
    '# 投资管理系统\n\n## 项目简介\n\n个人投资记录和统计分析系统。',
    256
)
ON DUPLICATE KEY UPDATE `Title`=`Title`;

-- ============================================
-- 4. 知识库表 (knowledge_base) - 插入示例知识
-- ============================================
INSERT INTO `knowledge_base` (`title`, `content`, `category`, `tags`, `version`, `parent_id`, `status`, `view_count`) VALUES 
(
    'MySQL 索引优化实践',
    '# MySQL 索引优化实践\n\n## 索引类型\n\n- B-Tree 索引\n- 全文索引\n- 哈希索引\n\n## 优化建议\n\n1. 合理使用索引\n2. 避免过度索引\n3. 定期分析表',
    '开发笔记',
    '["MySQL", "数据库", "优化"]',
    1,
    NULL,
    1,
    128
),
(
    'Docker 容器化部署指南',
    '# Docker 容器化部署指南\n\n## Dockerfile 编写\n\n```dockerfile\nFROM node:18-alpine\nWORKDIR /app\nCOPY package*.json ./\nRUN npm install\nCOPY . .\nEXPOSE 3000\nCMD ["npm", "start"]\n```',
    '开发笔记',
    '["Docker", "部署", "DevOps"]',
    1,
    NULL,
    1,
    64
),
(
    '前端性能优化技巧',
    '# 前端性能优化技巧\n\n## 优化方向\n\n1. 代码分割\n2. 懒加载\n3. 缓存策略\n4. CDN 加速',
    '开发笔记',
    '["前端", "性能优化", "Vue"]',
    1,
    NULL,
    1,
    96
)
ON DUPLICATE KEY UPDATE `title`=`title`;

-- ============================================
-- 5. 时间线事件表 (timeline_event) - 插入示例事件
-- ============================================
INSERT INTO `timeline_event` (`year`, `title`, `description`, `icon`, `color`, `sort_order`, `status`) VALUES 
(
    2024,
    '完成个人网站开发',
    '使用 Nuxt 3 和 .NET 8 开发了完整的个人网站系统',
    '🚀',
    'blue',
    1,
    1
),
(
    2024,
    '学习 AI 大模型应用',
    '深入学习了 ChatGPT API 和各种 AI 应用开发',
    '🤖',
    'purple',
    2,
    1
),
(
    2023,
    '开始全栈开发',
    '从后端开发转向全栈开发，学习前端技术栈',
    '💻',
    'green',
    3,
    1
),
(
    2022,
    '毕业参加工作',
    '正式成为一名软件工程师',
    '🎓',
    'orange',
    4,
    1
)
ON DUPLICATE KEY UPDATE `title`=`title`;

-- ============================================
-- 6. 投资记录表 (investment) - 插入示例投资数据
-- ============================================
INSERT INTO `investment` (`code`, `name`, `type`, `quantity`, `cost_price`, `current_price`, `total_cost`, `market_value`, `profit_loss`, `profit_rate`, `notes`) VALUES 
(
    '000001',
    '平安银行',
    'stock',
    1000.0000,
    12.5000,
    13.2000,
    12500.0000,
    13200.0000,
    700.0000,
    5.6000,
    '银行股，长期持有'
),
(
    '510300',
    '沪深300ETF',
    'fund',
    500.0000,
    4.2000,
    4.3500,
    2100.0000,
    2175.0000,
    75.0000,
    3.5714,
    '指数基金，定投策略'
),
(
    '600519',
    '贵州茅台',
    'stock',
    100.0000,
    1800.0000,
    1750.0000,
    180000.0000,
    175000.0000,
    -5000.0000,
    -2.7778,
    '白酒龙头，短期回调'
)
ON DUPLICATE KEY UPDATE `name`=`name`;

-- ============================================
-- 7. 时间胶囊表 (time_capsule) - 插入示例胶囊
-- ============================================
INSERT INTO `time_capsule` (`content`, `visitor_id`, `visitor_name`, `status`) VALUES 
(
    '这是一个很棒的个人网站！内容很丰富，设计也很漂亮。',
    UUID(),
    '访客A',
    1
),
(
    '希望网站越来越好，期待更多技术分享！',
    UUID(),
    '访客B',
    1
),
(
    '网站功能很完善，学到了很多东西。',
    UUID(),
    '访客C',
    1
),
(
    '新留言，等待审核中...',
    UUID(),
    '访客D',
    0
)
ON DUPLICATE KEY UPDATE `content`=`content`;

-- ============================================
-- 8. 友情链接表 (friend_links) - 插入示例链接
-- ============================================
INSERT INTO `friend_links` (`name`, `url`, `description`, `logo_url`, `sort_order`, `status`) VALUES 
(
    'Vue.js 官方文档',
    'https://vuejs.org',
    'Vue.js 官方文档和教程',
    'https://vuejs.org/logo.svg',
    1,
    1
),
(
    'Nuxt.js 官方文档',
    'https://nuxt.com',
    'Nuxt.js 官方文档',
    'https://nuxt.com/icon.png',
    2,
    1
),
(
    'MDN Web 文档',
    'https://developer.mozilla.org',
    'Web 开发技术文档',
    NULL,
    3,
    1
),
(
    'GitHub',
    'https://github.com',
    '代码托管平台',
    NULL,
    4,
    1
)
ON DUPLICATE KEY UPDATE `name`=`name`;

-- ============================================
-- 9. 用户行为记录表 (user_behavior) - 插入示例行为
-- ============================================
INSERT INTO `user_behavior` (`user_id`, `behavior_type`, `target_id`, `target_title`, `tags`, `category`, `duration`) VALUES 
(
    UUID(),
    'read_article',
    '1',
    '欢迎来到我的个人网站',
    '["技术", "介绍"]',
    '技术文章',
    120
),
(
    UUID(),
    'view_book',
    '1',
    'MySQL 索引优化实践',
    '["MySQL", "数据库"]',
    '开发笔记',
    300
),
(
    UUID(),
    'read_article',
    '2',
    'Vue 3 Composition API 使用指南',
    '["Vue", "前端"]',
    '技术文章',
    180
)
ON DUPLICATE KEY UPDATE `behavior_type`=`behavior_type`;

-- ============================================
-- 10. 站点配置表 (site_config) - 插入/更新配置
-- ============================================
INSERT INTO `site_config` (`config_key`, `config_value`, `description`) VALUES 
('site_title', '溪午听风 - 个人开发者网站', '网站标题'),
('site_subtitle', '开发让生活更高效，代码就是我的魔方', '网站副标题'),
('site_keywords', '个人网站,技术博客,全栈开发,Vue,Nuxt', '网站关键词'),
('site_description', '个人技术博客，分享技术学习、项目经验和生活感悟', '网站描述'),
('icp_record', '', 'ICP备案号'),
('home_page_size', '10', '首页文章数量'),
('github_url', 'https://github.com/yourusername', 'GitHub地址'),
('email', 'your@email.com', '联系邮箱')
ON DUPLICATE KEY UPDATE `config_value`=VALUES(`config_value`);

-- ============================================
-- 完成提示
-- ============================================
SELECT '示例数据插入完成！' AS message;

