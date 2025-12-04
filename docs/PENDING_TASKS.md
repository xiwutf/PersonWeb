# 待处理任务清单

**创建日期**：2024-12-03  
**基于规范**：[开发规范文档](./development/DEVELOPMENT_GUIDELINES.md)

## ✅ 已完成的工作

### 1. 文档整理 ✅
- ✅ 根目录文档整理到 `docs/` 目录
- ✅ 文档索引系统完善
- ✅ 文档使用指南创建

### 2. 组件重组 ✅
- ✅ 创建 9 个功能模块目录
- ✅ 移动 30 个组件到对应目录
- ✅ 组件组织符合规范

### 3. 样式管理（部分完成）✅
- ✅ Header 组件：Tailwind 类替换完成
- ✅ Footer 组件：Tailwind 类替换完成
- ✅ HeroSuper 组件：Tailwind 类替换完成
- ✅ RoadmapSection 组件：Tailwind 类替换完成
- ✅ 创建统一样式文件：`header.css`、`footer.css`、`hero.css`

## ⏳ 待处理任务（按优先级）

### 🔴 优先级 1：高优先级（必须完成）

#### 1. 样式管理规范

##### 1.1 Tailwind 类替换（进行中，29% 完成）

**状态**：12/41 个文件已完成，29 个文件待处理

**已完成**：
- ✅ Header.vue
- ✅ Footer.vue
- ✅ HeroSuper.vue
- ✅ RoadmapSection.vue
- ✅ HomeHybridSuper.vue
- ✅ BentoGridV4.vue
- ✅ BentoCardSection.vue
- ✅ BentoCardItem.vue
- ✅ FeaturedSection.vue
- ✅ PlatformEntryHub.vue
- ✅ PlatformValueSection.vue
- ✅ AIPlaygroundPreview.vue
- ✅ TimeCapsuleSuper.vue
- ✅ TimelineSuper.vue

**待处理文件**（按优先级）：

**首页组件**（中优先级）：
- ⏳ `components/home/HomeDarkLab.vue` - 114 处
- ⏳ `components/home/HomeLightPortfolio.vue` - 61 处
- ⏳ `components/home/BentoGridV3.vue` - 24 处
- ⏳ `components/home/PlatformCards.vue` - 18 处

**访客互动组件**（中优先级）：
- ⏳ `components/VisitorDanmakuWall.vue`
- ⏳ `components/VisitorBubble.vue`
- ⏳ `components/VisitorInteractionPanel.vue`
- ⏳ `components/VisitorFootprintMap.vue`
- ⏳ `components/VisitorLevelDisplay.vue`
- ⏳ `components/VisitorSidebarDrawer.vue`
- ⏳ `components/VisitorBehaviorListener.vue`
- ⏳ `components/VisitorChallengeButton.vue`
- ⏳ `components/VisitorTriggerEffects.vue`

**其他组件**（低优先级）：
- ⏳ `components/effects/*.vue` - 7 个组件
- ⏳ `components/3d/*.vue` - 3 个组件
- ⏳ `components/ai/*.vue` - 2 个组件
- ⏳ `components/time/*.vue` - 3 个组件
- ⏳ `components/content/*.vue` - 4 个组件
- ⏳ `components/admin/*.vue` - 3 个组件
- ⏳ `components/tools/*.vue` - 2 个组件
- ⏳ `components/english/*.vue` - 3 个组件
- ⏳ `components/ui/*.vue` - 2 个组件
- ⏳ `components/common/*.vue` - 1 个组件
- ⏳ `components/futuristic/*.vue` - 1 个组件
- ⏳ `components/games/*.vue` - 1 个组件
- ⏳ `components/layout/ThemeSwitcher.vue`
- ⏳ `components/layout/ThemeToggle.vue`
- ⏳ `components/layout/NaiveUIProviders.vue`

**行动计划**：
1. 优先处理首页组件（创建 `assets/css/home.css`）
2. 处理访客互动组件（扩展 `assets/css/visitor-interaction.css`）
3. 逐步处理其他组件

##### 1.2 样式文件组织检查

**需要检查**：
- [ ] 样式文件是否按功能模块组织
- [ ] 样式文件命名是否符合规范（kebab-case）
- [ ] 是否有重复的样式定义
- [ ] 样式文件是否在 `nuxt.config.ts` 中正确引用

**当前样式文件**：
- ✅ `assets/css/header.css` - Header 样式
- ✅ `assets/css/footer.css` - Footer 样式
- ✅ `assets/css/hero.css` - Hero 样式
- ⏳ `assets/css/home.css` - 首页样式（待创建）
- ✅ `assets/css/visitor-interaction.css` - 访客互动样式
- ✅ `assets/css/about.css` - 关于页面样式
- ✅ `assets/css/tools.css` - 工具页面样式
- ✅ `assets/css/life.css` - 生活页面样式
- ✅ `assets/css/blog.css` - 博客页面样式
- ✅ `assets/css/projects.css` - 项目页面样式
- ✅ `assets/css/investment.css` - 投资页面样式
- ✅ `assets/css/charts.css` - 图表样式

#### 2. 文档维护规范

##### 2.1 文档完整性检查

**需要检查**：
- [ ] 功能文档是否与代码同步
- [ ] 功能实现状态是否更新
- [ ] API 文档是否完整
- [ ] 配置文档是否更新

**待检查文档**：
- ⏳ `docs/features/IMPLEMENTATION_STATUS.md` - 功能实现状态
- ⏳ `docs/config/API_CONFIG.md` - API 配置
- ⏳ `docs/deployment/` - 部署文档
- ⏳ `docs/development/DEVELOPMENT_GUIDELINES.md` - 开发规范

##### 2.2 文档更新检查

**需要检查**：
- [ ] 新功能是否有对应的文档
- [ ] Bug 修复是否记录在故障排除文档
- [ ] 配置变更是否更新文档

### 🟡 优先级 2：中优先级（建议完成）

#### 3. 代码组织规范

##### 3.1 Composables 组织检查

**需要检查**：
- [ ] Composables 是否按功能分类
- [ ] 是否有重复的逻辑可以提取
- [ ] 命名是否符合规范（camelCase，`use` 前缀）
- [ ] 是否有未使用的 composables
- [ ] 是否有使用 `any` 类型的地方

**当前 Composables**（15 个）：
- `useAdminStyle.ts` - 后台样式管理
- `useApi.ts` - API 调用封装（有 `any` 类型）
- `useDevice.ts` - 设备检测
- `useEChartsTheme.ts` - ECharts 主题（有 `any` 类型）
- `useErrorHandler.ts` - 错误处理（有 `any` 类型）
- `useFontStyle.ts` - 字体样式
- `useMarkdown.ts` - Markdown 处理
- `useModuleSystem.ts` - 模块系统（有 `any` 类型）
- `useModuleTheme.ts` - 模块主题
- `useNaiveTheme.ts` - Naive UI 主题
- `useNaiveUI.ts` - Naive UI 封装
- `usePageStyle.ts` - 页面样式（有 `any` 类型）
- `useStyle.ts` - 样式管理
- `useTheme.ts` - 主题管理
- `useToast.ts` - 提示消息

**发现的问题**：
- ⚠️ 5 个 composables 使用了 `any` 类型（需要替换为具体类型）

**建议**：
- 检查是否有功能重复的 composables
- 考虑是否需要按功能模块组织（如 `composables/admin/`、`composables/visitor/`）
- 替换 `any` 类型为具体类型

##### 3.2 类型定义组织检查

**需要检查**：
- [ ] 类型定义是否统一在 `types/` 目录
- [ ] 类型文件命名是否符合规范（kebab-case）
- [ ] 是否有重复的类型定义
- [ ] 类型定义是否完整（避免使用 `any`）
- [ ] 组件中是否有内联类型定义，应提取到 `types/` 目录

**当前类型文件**：
- `types/api.ts` - API 相关类型
- `types/module.ts` - 模块相关类型

**发现的问题**：
- ⚠️ 10 个组件中有内联类型定义（应提取到 `types/` 目录）

**需要提取类型的组件**：
- `components/home/HeroSuper.vue`
- `components/home/AIPlaygroundPreview.vue`
- `components/ai/AIAssistant.vue`
- `components/layout/ThemeSwitcher.vue`
- `components/ui/AppCard.vue`
- `components/ui/AppButton.vue`
- `components/3d/Scene3D.vue`
- `components/futuristic/DigitalAvatar.vue`
- `components/VisitorLevelDisplay.vue`
- `components/VisitorFootprintMap.vue`

#### 4. API 开发规范

##### 4.1 后端 API 检查

**需要检查**：
- [ ] API 控制器是否符合 RESTful 规范
- [ ] API 响应格式是否统一（`ApiResponse<T>`）
- [ ] 错误处理是否完善
- [ ] API 文档是否完整（Swagger）

**待检查目录**：
- ⏳ `backend/PersonalSite.Api/Controllers/` - API 控制器
- ⏳ `backend/PersonalSite.Api/Models/` - 数据模型
- ⏳ `backend/PersonalSite.Api/Data/` - 数据访问层

##### 4.2 前端 API 调用检查

**需要检查**：
- [ ] 是否统一使用 `useApi()` composable
- [ ] 错误处理是否完善
- [ ] API 调用是否有类型定义

**建议**：
- 检查是否有直接使用 `fetch` 的地方，应替换为 `useApi()`
- 检查错误处理是否统一

#### 5. 数据库规范

##### 5.1 数据库脚本组织检查

**需要检查**：
- [ ] 数据库脚本是否按功能模块组织
- [ ] 表命名是否符合规范（snake_case，功能前缀）
- [ ] 字段命名是否符合规范（snake_case）
- [ ] 是否有数据库文档

**待检查目录**：
- ⏳ `database/` - 数据库脚本

**建议**：
- 检查表命名是否统一（如 `visitor_*`、`module_*`、`style_*`）
- 检查是否有数据库设计文档

### 🟢 优先级 3：低优先级（可选优化）

#### 6. Git 提交规范

##### 6.1 提交信息检查

**需要检查**：
- [ ] 提交信息是否符合规范（`<type>(<scope>): <subject>`）
- [ ] 提交信息是否清晰描述变更内容

**建议**：
- 使用 Git hooks 检查提交信息格式
- 在开发规范文档中强调提交规范

#### 7. 其他规范检查

##### 7.1 代码注释检查

**需要检查**：
- [ ] 复杂逻辑是否有中文注释
- [ ] 公共 API 是否有 JSDoc 注释

##### 7.2 性能优化检查

**需要检查**：
- [ ] 是否有性能问题
- [ ] 是否有可以优化的地方

## 📊 任务统计

| 任务类别 | 状态 | 进度 | 优先级 |
|---------|------|------|--------|
| Tailwind 类替换 | ⏳ 进行中 | 10% (4/41) | 🔴 高 |
| 文档完整性检查 | ⏳ 待处理 | 0% | 🔴 高 |
| Composables 组织 | ⏳ 待处理 | 0% | 🟡 中 |
| 类型定义组织 | ⏳ 待处理 | 0% | 🟡 中 |
| API 开发规范 | ⏳ 待处理 | 0% | 🟡 中 |
| 数据库规范 | ⏳ 待处理 | 0% | 🟡 中 |
| Git 提交规范 | ⏳ 待处理 | 0% | 🟢 低 |
| 代码注释 | ⏳ 待处理 | 0% | 🟢 低 |
| 性能优化 | ⏳ 待处理 | 0% | 🟢 低 |

## 📝 建议的处理顺序

### 第一阶段（本周）

1. **继续 Tailwind 类替换**
   - 优先处理首页组件（创建 `home.css`）
   - 处理访客互动组件（扩展 `visitor-interaction.css`）

2. **检查 Composables**
   - 检查是否有重复逻辑
   - 替换 `any` 类型为具体类型

### 第二阶段（下周）

1. **类型定义提取**
   - 提取组件中的内联类型定义到 `types/` 目录

2. **文档完整性检查**
   - 更新功能实现状态
   - 更新 API 配置文档

### 第三阶段（后续）

1. **API 和数据库规范检查**
2. **代码注释和性能优化**

## 🔗 相关文档

- [开发规范文档](./development/DEVELOPMENT_GUIDELINES.md)
- [项目梳理检查清单](./PROJECT_REORGANIZATION_CHECKLIST.md)
- [代码规范检查报告](./quality/CODE_STANDARDS_CHECK.md)
- [代码重构进度报告](./quality/REFACTORING_PROGRESS.md)

---

**创建时间**：2024-12-03  
**下次更新**：完成主要任务后更新

