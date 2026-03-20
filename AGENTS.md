# AI 任务入口手册

AI 在本项目执行任务时，只需要先看这一页，再按类型读取少量核心手册。

## 核心原则

1. 先判断任务类型
2. 先读对应手册
3. 再改代码
4. 改完后只在必要时更新文档

## 任务类型与必读文档

| 任务类型 | 必读文档 |
| --- | --- |
| 项目总览 | `docs/PROJECT_OVERVIEW.md` |
| 定位代码位置 | `docs/PROJECT_STRUCTURE_GUIDE.md` |
| 页面、组件、前端逻辑 | `docs/development/DEVELOPMENT_GUIDELINES.md` |
| 样式、主题、设计系统 | `docs/development/STYLE_ARCHITECTURE.md`、`docs/development/STYLE_MANAGEMENT_REMINDER.md`、`docs/design-system/README.md` |
| 模块功能 | `docs/architecture/README_MODULES.md`、`docs/development/MODULE_DEVELOPMENT_GUIDE.md`、`docs/modules/` |
| API、联调、配置 | `docs/config/API_CONFIG.md`、`docs/api/MODULE_SYSTEM_API.md` |
| 部署、启动、环境 | `docs/deployment/README.md`、`docs/deployment/QUICK_START.md` |
| 低频历史资料 | `docs/archive/` |

## 允许直接修改的前提

只有同时满足下面条件，才直接开始改：

- 已判断任务类型
- 已阅读对应核心手册
- 已定位实际代码落点

如果涉及样式、模块、部署、数据修复，不允许跳过手册直接修改。

## 文档规则

新增文档时：

- 优先放到已有核心分类目录
- 一次性阶段记录优先放 `docs/archive/`
- 不要再创建新的文档索引体系

## 保留的文档层级

当前只保留 3 层：

1. `AGENTS.md`
2. `docs/PROJECT_OVERVIEW.md` 与 `docs/PROJECT_STRUCTURE_GUIDE.md`
3. 少量核心专题手册

其余低频、阶段性、历史性材料统一放在 `docs/archive/legacy/`

## 一句话原则

AI 在本项目里必须“先读最少必要文档，再执行任务”。

## 性能补充原则

涉及首页、公共页面、全局依赖、构建配置、数据读取方式时，AI 必须同时检查性能影响：

- 公共前台页面默认优先保留 SSR，不因为实现方便直接关闭；确需关闭时，必须先确认原因和收益
- Markdown 内容读取、内容聚合、列表整理、统计计算等优先下沉到 `server/api`、Nitro route 或后端接口，页面层优先只消费轻量 JSON
- 图表、Markdown 编辑器、3D、富媒体播放器等重型依赖优先异步加载、动态 `import()` 或进入可视区后再加载，不默认进入首屏公共包
- 新增全局 `head` 资源、全局 CSS、全局插件前，先检查是否可以按页面、按组件或按后台模块加载
- 页面专属样式和脚本不默认挂到全局入口，避免让非相关页面承担下载和解析成本
- 多个独立请求避免串行瀑布流，优先并发、限流或改为服务端聚合
- 新增第三方依赖前，先判断项目里是否已有同类能力，避免同时保留两套重量级方案
- 涉及首页、公共布局、全局资源、构建配置的改动后，至少做一次构建校验，必要时检查 chunk 或产物体积变化

## 样式补充原则

涉及样式修改时，必须先检查现有 `tokens`、全局样式文件、页面样式文件和可复用类：

- 前端页面和布局开发时，优先使用项目内已接入的 UI 库、现有组件库能力和已有通用组件，不先跳过它们自己重写一套
- 后台页面默认优先使用 `Naive UI` 与现有后台 Pattern 组件完成布局和交互，只有现成能力明显不足时才补充少量自定义 UI 与样式
- 有现成 CSS 变量、统一样式类、页面样式时，直接复用，不重复新增
- 如果 UI 库或现有组件已经能满足布局和交互需求，优先通过配置、组合、覆写现有 class 完成，不重复造轮子
- 现有样式不够表达需求时，先在合适的样式层补定义，再在具体页面或组件中调用
- 新增样式优先抽到对应的 `assets/css/*.css`、`assets/styles/*.css` 或现有 token 中，不要先在页面里硬写
- 页面和组件内优先“调用样式变量 / 调用已有类”，只有确认缺失时才新增定义
- 禁止为了图省事在模板里直接写大段内联样式、硬编码颜色 / 圆角 / 阴影 / 间距；确需动态样式时也只能保留最小必要的 `:style`
