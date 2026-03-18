🅐 全局视觉体系升级（Global Visual System）

目标：让整个首页具备“深空实验室 × Vision Pro 雾态 × 创作者平台的极简风”

✅ 任务：

新增 global 背景层（BackgroundLayer.vue）

基于 CSS 渐变（radial-gradient + linear-gradient）

混合深色科幻蓝紫 + 极光色（青绿）

默认透明度低，作为深色模式主背景

加入全局流动光效（BlurField.vue）

背景加一层 “macOS stage manager 风格的雾状模糊光带”

使用 CSS filter: blur(80px) + opacity 动画

引入 glassmorphism（玻璃态）统一样式

全站卡片统一使用

backdrop-filter: blur(12px) saturate(180%);
background: rgba(255,255,255,0.06);
border: 1px solid rgba(255,255,255,0.12);


深色和浅色模式自动变体

卡片统一圆角：24px

阴影统一：柔光白 + 科技感蓝边缘光

=========================================================
🅑 Hero 区域（Super Hero）重做
🎯 目标：

一个全屏、高端、具有“未来 AI 平台气质”的入口。

🎨 设计元素（让 Cursor 按此实现）：

背景：深空粒子场（Three.js / Vanta.js 都可）

前景主体采用半透明玻璃态容器

文本布局更接近苹果 Vision Pro 的“空间层次布局”

✅ 任务：
1. 新建组件：HeroSuper.vue

包含元素：

（1）品牌标题区
主标题：Build the Future With AI × You
副标题：Your Personal AI Creation Platform

（2）身份标签自动轮播

例如：

AI Creator

Indie Developer

Software Engineer

Algorithm Researcher

用 Motion One 或 Framer Motion 语义动画

（3）英雄头像（半 3D 风格卡片）

可以用你的照片/AI头像

背景加入光晕 + 粒子喷射

卡片采用轻微 3D 倾斜动画（tilt）

（4）CTA 按钮（两个）

Explore Platform

Enter AI Playground

（5）底部提示

自定义 Scroll Indicator（发光流线型箭头）

=========================================================
🅒 平台入口的三大模块（Platform Gate Cards）

紧贴在 Hero 下方，以“三座发光卡片”的形式展示平台能力。

平台三大入口：

AI Laboratory（AI 实验室）

Creator Toolbox（创作者工具箱）

Knowledge Hub（知识中心 / 博客）

每个卡片采用：

玻璃态 + 未来光边 + 微交互漂浮动画

图标可用 Lucide icons 或 Lottie 动效

✅ 任务：

新建组件：

PlatformEntryCards.vue

内含三个子卡片：PlatformCard.vue

=========================================================
🅓 Bento Grid V3（全新超豪华展示区）
🎯 目标：

打造有深度、有科技感、可以展示不同内容类型的动态网格布局。

参考风格：

Notion 风

Apple Glass UI

Sci-fi Bento

Bento 结构设计（Cursor 需要按此布局）：

网格示例：

特色内容 (2x2 大卡)	项目 (1x1)	工具 (1x1)
博客精选 (1x1)	AI Lab (1x2 细长卡)	时间胶囊入口
每种卡片都有不同材质与动效：
特色内容卡（Featured）

大卡片

背景采用流体渐变动画

项目卡（Projects）

轻量玻璃

图标+简介+hover光效片状扩散

AI Lab 卡（长条卡片）

水平滚动的标签

科幻 HUD 边框

✅ 任务：

建立统一组件体系：

components/Bento/
    BentoGrid.vue
    BentoCardFeatured.vue
    BentoCardProject.vue
    BentoCardAI.vue
    BentoCardBlog.vue

=========================================================
🅔 时间胶囊弹幕墙（Time Capsule Wall）升级
🎯 目标：

一个流动的、未来感的、带玻璃 UI 的弹幕墙。

升级内容：

背景加入“彩色流体光晕”

弹幕条采用玻璃态 + 发光边框

鼠标靠近 → 动态减速 / 停止

弹幕速度智能调节

✅ 任务：

完整重写 TimeCapsuleWall.vue

使用 requestAnimationFrame 控制弹幕

加入吸附、缓动动画

=========================================================
🅕 成长轨迹 Timeline（更高端视觉）
🎯 目标：

更偏向 Apple / GitHub Universe 的流线型时间线。

任务：

每个节点变成玻璃态卡片

左右交错布局

背后加一条发光线（line-gradient）

新逻辑：

节点进入视口时触发“光线从左到右扫过卡片”的动效

=========================================================
🅖 混合款风格系统（全新主题体系）
目标风格：

Dark Lab 2.0

Vision Light

Creator Minimal

Hybrid Super Style（默认）

任务：

在 /styles/theme.css 中新增变量组：

光色系（electric-blue, aurora-green）

雾态玻璃背景

卡片材质变量（glass-bg, glass-border）

后端 theme API 增加新主题：

hybrid-super


useTheme() 支持动态切换（无需刷新）

=========================================================
🅗 动画系统接入 Motion One（推荐）
任务：

建立 /composables/useMotion.js 封装

给每个卡片/标题/滚动交互添加：

入场淡入

hover 微位移

3D tilt

光效 pulse

=========================================================
🅘 组件结构与代码组织规范
任务（Cursor 可自动重构）：

/components/home/ 下放置所有首页模块

所有粒子、光效独立为 /components/effects/

全局样式拆分为：

/styles/tokens.css

/styles/effects.css

/styles/glass.css
