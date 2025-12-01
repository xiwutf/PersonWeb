-- ============================================
-- 完整网站内容数据插入脚本
-- 包含：配置、文章、项目、更新日志、未来规划
-- ============================================

-- ============================================
-- 1. 插入站点配置（Slogan、个人简介等）
-- ============================================
INSERT INTO `site_config` (`config_key`, `config_value`, `description`) VALUES
-- 首页 Slogan（选择其中一个或混合使用）
('home_slogan', '个人 × AI × 创造力 = 新一代创作者平台', '首页顶部 Slogan'),
('home_slogan_alt_1', '打造属于自己的数字资产与超级个人平台', '首页 Slogan 备选1'),
('home_slogan_alt_2', '以技术为羽翼，构建我与世界的连接方式', '首页 Slogan 备选2'),
('home_slogan_alt_3', 'Every Line of Code Builds My Future', '首页 Slogan 备选3'),

-- 身份标签
('home_tags', '软件开发 · 算法研发 · 独立开发者 · 创作者 · AI 工具构建者 · 数字资产探索者', '首页身份标签'),

-- 个人简介（Hero 文案）
('home_hero_intro', '我是一名软件与算法开发者，正在打造一个能持续增长的个人数字资产平台。这里记录我的作品、思考、实验项目和未来计划。同时，我正探索 AI 技术如何助力个人成长、创作与商业化。', '首页个人简介'),
('home_platform_desc', '这里是一套围绕个人 IP 构建的创作操作系统：文章 / 项目 / 工具 / AI 实验室', '平台定位描述'),

-- 关于我页面内容
('about_intro', '27 岁软件开发者，专注 Web 开发、算法研究、AI 应用与数字资产构建。', '关于我一句话介绍'),
('about_detail', '我来自河北，职业是软件研发与算法开发。我热爱创造、编程和探索 AI 技术带来的可能性。我现在正在构建一个长期项目：让一个普通开发者也能拥有属于自己的数字资产与平台级能力。这个网站是第一步，它将成为：我的作品集、我的思考记录、我的知识库、我的 AI 产品的入口、我的独立开发品牌。未来，我希望通过技术与 AI，让自己实现更高的效率、更自由的工作方式与更大的创造力。', '关于我详细介绍'),
('about_skills', '前端：Vue3、Nuxt、Tailwind | 后端：.NET、Java、Python（AI 服务） | 数据库：MySQL、Redis（计划中） | 云服务：阿里云 ECS、OSS、RDS | 擅长：系统设计、算法逻辑、AI 产品形态探索', '技能栈'),
('about_goals', '打造一个具备平台能力的个人站 | 持续输出作品与工具 | 打造自己的 AI 产品线 | 积累长期有价值的数字资产 | 实现财务自由的技术路径', '目标'),

-- AI 中心介绍
('ai_center_intro', '我正在构建一个面向个人与创作者的 AI 工具集，包括：对话式智能体、文档助手（Doc AI）、知识库系统（KB）、AI 生产力工具、开放 API（待上线）', 'AI 中心介绍')

ON DUPLICATE KEY UPDATE 
    `config_value`=VALUES(`config_value`),
    `description`=VALUES(`description`);

-- ============================================
-- 2. 确保有分类数据
-- ============================================
INSERT INTO `category` (`name`, `slug`, `sort`) VALUES
('技术博客', 'tech', 1),
('生活随笔', 'life', 2),
('项目分享', 'project', 3),
('AI探索', 'ai', 4)
ON DUPLICATE KEY UPDATE `name`=VALUES(`name`), `slug`=VALUES(`slug`), `sort`=VALUES(`sort`);

-- ============================================
-- 3. 插入文章数据（5篇）
-- ============================================
INSERT INTO `article` (
    `title`, `slug`, `summary`, `content_md`, `content_html`,
    `category_id`, `status`, `publish_time`, `view_count`, `created_at`, `updated_at`
) VALUES
(
    '为什么我要打造自己的数字资产平台？',
    'why-build-digital-asset-platform',
    'AI 时代个人品牌的重要性，个人生产力的指数级提升，平台与内容累积带来的长期收益，我为什么认为个人网站不是展示，而是资产。',
    '# 为什么我要打造自己的数字资产平台？\n\n## AI 时代个人品牌的重要性\n\n在 AI 快速发展的今天，个人品牌和数字资产变得越来越重要。一个属于自己的平台，不仅是展示窗口，更是长期积累的资产。\n\n## 个人生产力的指数级提升\n\n通过构建自己的平台，我可以：\n\n- 集中管理所有作品和思考\n- 建立个人知识库\n- 打造 AI 工具集\n- 实现自动化工作流\n\n## 平台与内容累积带来的长期收益\n\n每一个项目、每一篇文章、每一个工具，都是数字资产的一部分。这些资产会随着时间累积，产生复利效应。\n\n## 个人网站不是展示，而是资产\n\n传统观念认为个人网站只是展示作品的地方，但我认为它应该是：\n\n- 可积累的数字资产\n- 可扩展的平台能力\n- 可商业化的产品基础\n- 个人品牌的核心载体',
    '<h1>为什么我要打造自己的数字资产平台？</h1><p>AI 时代个人品牌的重要性，个人生产力的指数级提升，平台与内容累积带来的长期收益。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    '2024-12-01 10:00:00',
    234,
    NOW(),
    NOW()
),
(
    'AI 会取代开发者吗？我的真实思考',
    'will-ai-replace-developers',
    'AI 不会替代掌握 AI 的人。开发者角色从"写代码"变成"构建系统 + 调用智能体"。我如何用 AI 提升效率，为什么构建 AI 中心是我下一步。',
    '# AI 会取代开发者吗？我的真实思考\n\n## AI 不会替代掌握 AI 的人\n\nAI 工具正在改变开发方式，但不会完全替代开发者。真正会被替代的是那些不学习、不进步的开发者。\n\n## 开发者角色的转变\n\n从"写代码"到"构建系统 + 调用智能体"：\n\n- **以前**：手动编写每一行代码\n- **现在**：设计系统架构，用 AI 辅助实现\n- **未来**：构建智能体工作流，让 AI 自主执行任务\n\n## 我如何用 AI 提升效率\n\n1. **代码生成**：使用 Copilot 和 Cursor 加速开发\n2. **文档编写**：AI 辅助生成技术文档\n3. **问题排查**：AI 帮助快速定位问题\n4. **架构设计**：AI 提供设计建议和最佳实践\n\n## 为什么构建 AI 中心是我下一步\n\n- 探索 AI 在个人生产力中的应用\n- 打造自己的 AI 工具集\n- 为未来商业化做准备',
    '<h1>AI 会取代开发者吗？我的真实思考</h1><p>AI 不会替代掌握 AI 的人。开发者角色正在转变。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'ai' LIMIT 1),
    1,
    '2024-11-25 14:30:00',
    189,
    NOW(),
    NOW()
),
(
    '我的个人网站从 0 到 1 的搭建过程',
    'personal-website-0-to-1',
    '技术栈选择，为什么用 Vue3 + .NET + Python，阿里云部署、自动化流程，遇到的问题与解决方案。',
    '# 我的个人网站从 0 到 1 的搭建过程\n\n## 技术栈选择\n\n### 前端：Vue3 + Nuxt3\n\n- **Vue3**：现代化的前端框架，Composition API 让代码更清晰\n- **Nuxt3**：服务端渲染，SEO 友好，开发体验优秀\n- **Tailwind CSS**：快速构建美观界面\n\n### 后端：.NET Core\n\n- 性能优秀，生态完善\n- 与前端分离，便于扩展\n- 支持多种数据库\n\n### AI 服务：Python\n\n- 独立的 Python 服务处理 AI 相关功能\n- 文本生成、向量化、文档问答\n- 第三方 AI 模型统一调用\n\n## 阿里云部署\n\n- **ECS**：服务器托管\n- **OSS**：静态资源存储\n- **RDS**：数据库服务\n\n## 遇到的问题与解决方案\n\n1. **SSR 兼容性**：解决 Naive UI 的 SSR 问题\n2. **性能优化**：代码分割、缓存策略\n3. **数据同步**：前后端数据格式统一',
    '<h1>我的个人网站从 0 到 1 的搭建过程</h1><p>技术栈选择，为什么用 Vue3 + .NET + Python。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'tech' LIMIT 1),
    1,
    '2024-11-20 09:15:00',
    156,
    NOW(),
    NOW()
),
(
    '未来 5 年我要打造哪些产品？（路线图）',
    '5-year-roadmap',
    'AI 工具、开发者服务、自我成长相关产品、平台生态构想。',
    '# 未来 5 年我要打造哪些产品？（路线图）\n\n## AI 工具\n\n- 对话式智能体\n- 文档助手（Doc AI）\n- 知识库系统（KB）\n- AI 生产力工具集\n\n## 开发者服务\n\n- 开发者工具箱\n- API 网关服务\n- 代码生成工具\n- 自动化部署平台\n\n## 自我成长相关产品\n\n- 个人知识管理系统\n- 学习路径规划\n- 技能树可视化\n- 成长轨迹追踪\n\n## 平台生态构想\n\n- 个人品牌建设工具\n- 内容创作平台\n- 数字资产管理\n- 商业化 SaaS 模块',
    '<h1>未来 5 年我要打造哪些产品？（路线图）</h1><p>AI 工具、开发者服务、自我成长相关产品、平台生态构想。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'project' LIMIT 1),
    1,
    '2024-11-15 16:00:00',
    178,
    NOW(),
    NOW()
),
(
    '一个普通人的独立开发之路',
    'indie-developer-journey',
    '如何提高技术与思维，如何选择项目，如何构建自己可持续的产品线。',
    '# 一个普通人的独立开发之路\n\n## 如何提高技术与思维\n\n1. **持续学习**：跟上技术发展趋势\n2. **实践项目**：通过实际项目积累经验\n3. **思考总结**：记录学习过程和思考\n4. **社区交流**：参与开源社区，学习他人经验\n\n## 如何选择项目\n\n- **解决真实问题**：从自己的需求出发\n- **技术可行性**：评估技术难度和资源\n- **长期价值**：考虑项目的可持续性\n- **个人兴趣**：选择自己感兴趣的方向\n\n## 如何构建可持续的产品线\n\n1. **MVP 快速验证**：先做最小可行产品\n2. **迭代优化**：根据反馈持续改进\n3. **积累资产**：每个项目都是数字资产\n4. **形成生态**：产品之间相互关联\n\n## 我的经验\n\n从个人网站开始，逐步扩展到工具集、AI 服务，最终目标是构建一个完整的个人平台生态。',
    '<h1>一个普通人的独立开发之路</h1><p>如何提高技术与思维，如何选择项目，如何构建自己可持续的产品线。</p>',
    (SELECT `id` FROM `category` WHERE `slug` = 'life' LIMIT 1),
    1,
    '2024-11-10 11:30:00',
    145,
    NOW(),
    NOW()
)
ON DUPLICATE KEY UPDATE 
    `title`=VALUES(`title`),
    `summary`=VALUES(`summary`),
    `content_md`=VALUES(`content_md`),
    `content_html`=VALUES(`content_html`);

-- ============================================
-- 4. 插入项目数据（5个）
-- ============================================
INSERT INTO `projects` (
    `Id`, `Title`, `Description`, `CoverUrl`, `DemoUrl`, `GithubUrl`,
    `Status`, `TechStack`, `Content`, `CreatedAt`, `UpdatedAt`
) VALUES
(
    UUID(),
    '个人数字资产平台（本网站）',
    '用于展示我的能力、工具、产品与 AI 作品的主平台。功能包括：访客分析、后台管理、文章系统、配置中心、AI 网关等。',
    NULL,
    'https://xifg.dev',
    NULL,
    'Active',
    '["Nuxt 3", "Vue 3", "TypeScript", "Tailwind CSS", ".NET Core", "MySQL"]',
    '# 个人数字资产平台\n\n## 项目简介\n\n这是我在构建的长期项目，目标是打造一个具备平台能力的个人网站。\n\n## 主要功能\n\n- **访客分析系统**：实时统计访问数据\n- **后台管理系统**：内容管理、配置管理\n- **文章系统**：技术博客、生活随笔\n- **配置中心**：主题切换、模块管理\n- **AI 网关**：统一 AI 服务接口\n\n## 技术栈\n\n- 前端：Nuxt 3 + Vue 3 + TypeScript + Tailwind CSS\n- 后端：.NET Core + Entity Framework Core\n- 数据库：MySQL\n- 部署：阿里云 ECS + OSS + RDS\n\n## 进度\n\n长期迭代中，持续完善功能和体验。',
    NOW(),
    NOW()
),
(
    UUID(),
    'AI Service（Python 服务）',
    '独立的 Python AI 模型服务，将处理：文本生成、文本向量化、文档问答、智能体工作流、第三方 AI 模型统一调用。',
    NULL,
    NULL,
    NULL,
    'Active',
    '["Python", "FastAPI", "OpenAI API", "LangChain", "Vector DB"]',
    '# AI Service\n\n## 项目简介\n\n独立的 Python AI 服务，为个人平台提供 AI 能力支持。\n\n## 主要功能\n\n- **文本生成**：基于大语言模型的文本生成\n- **文本向量化**：将文本转换为向量，支持语义搜索\n- **文档问答**：上传文档后可以问答\n- **智能体工作流**：构建复杂的 AI 工作流\n- **统一调用接口**：整合多个 AI 模型服务\n\n## 技术栈\n\n- Python 3.11+\n- FastAPI：Web 框架\n- LangChain：AI 应用框架\n- Vector Database：向量数据库\n\n## 进度\n\n架构设计完成，正在开发中。',
    DATE_SUB(NOW(), INTERVAL 30 DAY),
    NOW()
),
(
    UUID(),
    '访客分析系统（Analytics）',
    '一个完整的数据统计系统：PV/UV 统计、趋势图、地区分布、设备/浏览器分布、最新访客记录。',
    NULL,
    NULL,
    NULL,
    'Completed',
    '["Vue 3", ".NET Core", "MySQL", "Chart.js"]',
    '# 访客分析系统\n\n## 项目简介\n\n实时访客行为分析系统，提供详细的访问统计数据。\n\n## 主要功能\n\n- **PV/UV 统计**：页面访问量和独立访客数\n- **趋势图**：访问趋势可视化\n- **地区分布**：访客地理位置分布\n- **设备/浏览器分布**：设备类型和浏览器统计\n- **最新访客记录**：实时访客记录\n\n## 技术栈\n\n- 前端：Vue 3 + Chart.js\n- 后端：.NET Core\n- 数据库：MySQL\n\n## 进度\n\n已完成并上线使用。',
    DATE_SUB(NOW(), INTERVAL 60 DAY),
    DATE_SUB(NOW(), INTERVAL 10 DAY)
),
(
    UUID(),
    '开发者工具箱（Tools）',
    '轻量、实用、无需登录的在线工具：JSON 格式化、文本比较、正则测试器、图片压缩、Markdown 编辑器等。',
    NULL,
    NULL,
    NULL,
    'Active',
    '["Vue 3", "TypeScript", "Tailwind CSS"]',
    '# 开发者工具箱\n\n## 项目简介\n\n轻量、实用、永久免费的在线工具集合。\n\n## 已上线工具\n\n- JSON 格式化工具\n- 文本对比工具\n- 正则表达式测试器\n- Base64 编解码工具\n- 时间戳转换工具\n- URL 参数解析工具\n\n## 计划中工具\n\n- 图片压缩工具\n- Markdown 编辑器\n- 颜色选择器\n- 二维码生成器\n\n## 技术栈\n\n- Vue 3 + TypeScript\n- Tailwind CSS\n- 纯前端实现，无需后端\n\n## 进度\n\n持续扩充中。',
    DATE_SUB(NOW(), INTERVAL 45 DAY),
    NOW()
),
(
    UUID(),
    '实验室（Labs）',
    '用于尝试创造性的可视化工具与前端交互实验：3D 模型浏览器、粒子系统、动态背景实验、创意动画组件、AI 增强型交互工具。',
    NULL,
    NULL,
    NULL,
    'Active',
    '["Three.js", "Vue 3", "WebGL", "Canvas API"]',
    '# 实验室\n\n## 项目简介\n\n一个专门用于放置创意实验和可视化工具的空间。\n\n## 计划中的实验\n\n- **3D 模型浏览器**：Three.js 实现的 3D 场景\n- **粒子系统**：动态粒子效果\n- **动态背景实验**：创意背景动画\n- **创意动画组件**：独特的交互组件\n- **AI 增强型交互工具**：结合 AI 的交互实验\n\n## 技术栈\n\n- Three.js：3D 图形库\n- WebGL：图形渲染\n- Canvas API：2D 图形\n- Vue 3：前端框架\n\n## 进度\n\n正在规划中，第一批实验即将上线。',
    DATE_SUB(NOW(), INTERVAL 20 DAY),
    NOW()
)
ON DUPLICATE KEY UPDATE 
    `Title`=VALUES(`Title`),
    `Description`=VALUES(`Description`),
    `Status`=VALUES(`Status`);

-- ============================================
-- 5. 插入更新日志数据
-- ============================================
INSERT INTO `changelog` (
    `version`, `date`, `title`, `description`, `items`, `category`, `status`
) VALUES
(
    '2025.11.29',
    '2025-11-29',
    '访客分析与主题系统优化',
    '完成访客 IP 获取逻辑修复，新增主题切换后端配置接口，完善后台管理主题选择，Analytics 数据字段结构优化。',
    '["完成访客 IP 获取逻辑修复", "新增主题切换后端配置接口", "完善后台管理主题选择", "Analytics 数据字段结构优化"]',
    'feature',
    1
),
(
    '2025.11.28',
    '2025-11-28',
    'AI Service 架构初始化',
    '完成 AI Service 部署文档，初始化 Python 服务架构，添加网站风格切换方案讨论，优化首页布局与卡片结构。',
    '["完成 AI Service 部署文档", "初始化 Python 服务架构", "添加网站风格切换方案讨论", "优化首页布局与卡片结构"]',
    'feature',
    1
),
(
    '2025.11.27',
    '2025-11-27',
    '数据库与后台管理系统',
    '完成数据库初始化，新增 Projects 与 Metrics 表，配置 OSS 上传与 CI/CD 自动部署，完成后台管理系统初版。',
    '["完成数据库初始化", "新增 Projects 与 Metrics 表", "配置 OSS 上传与 CI/CD 自动部署", "完成后台管理系统初版"]',
    'feature',
    1
),
(
    '2025.11.26',
    '2025-11-26',
    '访客记录系统上线',
    '完成访客记录系统（VisitLogs），添加浏览器/设备/地域解析逻辑，添加后台趋势图接口。',
    '["完成访客记录系统（VisitLogs）", "添加浏览器/设备/地域解析逻辑", "添加后台趋势图接口"]',
    'feature',
    1
)
ON DUPLICATE KEY UPDATE 
    `title`=VALUES(`title`),
    `description`=VALUES(`description`),
    `items`=VALUES(`items`);

-- ============================================
-- 6. 插入未来规划数据
-- ============================================
INSERT INTO `roadmap` (
    `title`, `description`, `items`, `timeline`, `priority`, `status`, `progress`
) VALUES
(
    '短期规划（1 个月）',
    '首页风格升级、Tools 工具箱扩展、AI Service 接入、实验室上线。',
    '["首页风格升级（支持用户风格切换）", "Tools 工具箱上线更多工具", "AI Service 接入第三方模型", "实验室上线第一批视觉实验"]',
    'short',
    2,
    'in_progress',
    30
),
(
    '中期规划（3-6 个月）',
    'AI 中心上线、文档问答、用户系统、开发者 API。',
    '["AI 中心上线 3 个智能体", "支持 AI 文档问答", "用户系统（登录/数据同步）", "开发者 API（BETA）"]',
    'medium',
    2,
    'planned',
    0
),
(
    '长期规划（1 年）',
    '完整的个人 AI 平台、创作者工具集、商业化 SaaS 模块、多人协作能力。',
    '["完整的个人 AI 平台", "创作者工具集", "可商业化的 SaaS 模块", "多人协作与自定义工作流能力"]',
    'long',
    1,
    'planned',
    0
)
ON DUPLICATE KEY UPDATE 
    `description`=VALUES(`description`),
    `items`=VALUES(`items`),
    `status`=VALUES(`status`),
    `progress`=VALUES(`progress`);

-- ============================================
-- 7. 验证数据插入
-- ============================================
SELECT '配置项数量' AS '类型', COUNT(*) AS '数量' FROM `site_config`
UNION ALL
SELECT '文章数量', COUNT(*) FROM `article` WHERE `status` = 1
UNION ALL
SELECT '项目数量', COUNT(*) FROM `projects` WHERE `Status` = 'Active'
UNION ALL
SELECT '更新日志数量', COUNT(*) FROM `changelog` WHERE `status` = 1
UNION ALL
SELECT '未来规划数量', COUNT(*) FROM `roadmap`;

