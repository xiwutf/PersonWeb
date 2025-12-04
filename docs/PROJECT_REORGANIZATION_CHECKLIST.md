# 项目梳理检查清单

**创建日期**：2024-12-03  
**基于规范**：[开发规范文档](./development/DEVELOPMENT_GUIDELINES.md)

## 📋 梳理目标

根据开发规范，全面梳理项目，确保代码组织、样式管理、文档维护等方面符合规范要求。

## ✅ 已完成的工作

### 1. 文档整理 ✅

- ✅ 根目录文档整理到 `docs/` 目录
- ✅ 文档索引系统完善
- ✅ 文档使用指南创建
- ✅ 文档组织说明完善

### 2. 样式管理（部分完成）✅

- ✅ Header 组件：Tailwind 类替换完成
- ✅ Footer 组件：Tailwind 类替换完成
- ✅ 内联样式检查：20 个组件已检查
- ✅ 创建统一样式文件：`header.css`、`footer.css`

## ⏳ 待梳理的工作

### 1. 样式管理规范 ⚠️ **高优先级**

#### 1.1 Tailwind 类替换（进行中）

**状态**：完成 2/41 个文件（约 5%）

**待处理文件**（按优先级）：
- ⏳ `components/home/HeroSuper.vue` - 41 处 Tailwind 类
- ⏳ `components/home/RoadmapSection.vue` - 7 处 Tailwind 类
- ⏳ `components/home/HomeHybridSuper.vue` - 1 处 Tailwind 类
- ⏳ `components/home/` 目录下的其他组件
- ⏳ `components/Visitor*.vue` 系列组件
- ⏳ `components/admin/` 目录下的组件
- ⏳ 其他 39 个文件

**行动计划**：
1. 优先处理首页高频组件（HeroSuper、RoadmapSection）
2. 创建 `assets/css/home.css` 统一样式文件
3. 逐步替换其他组件
4. 更新文档记录进度

#### 1.2 内联样式检查（已完成检查，需持续监控）

**状态**：✅ 已检查 20 个组件

**需要持续监控**：
- 新开发的组件是否使用内联样式
- 动态样式是否符合规范（仅用于必须运行时计算的属性）

**建议**：
- 在代码审查时检查内联样式使用
- 在开发规范文档中强调禁止事项

### 2. 代码组织规范 ⚠️ **中优先级**

#### 2.1 组件组织检查

**需要检查**：
- [ ] 组件是否按功能模块组织（`components/home/`、`components/admin/` 等）
- [ ] 是否有组件需要移动到更合适的目录
- [ ] 是否有重复的组件可以合并
- [ ] 组件命名是否符合规范（PascalCase）

**待检查目录**：
- `components/` - 所有组件
- `components/home/` - 首页组件
- `components/admin/` - 后台管理组件
- `components/english/` - 英语学习组件
- `components/common/` - 公共组件

#### 2.2 Composables 组织检查

**需要检查**：
- [ ] Composables 是否按功能分类
- [ ] 是否有重复的逻辑可以提取
- [ ] 命名是否符合规范（camelCase，`use` 前缀）
- [ ] 是否有未使用的 composables

**当前 Composables**：
- `useAdminStyle.ts` - 后台样式管理
- `useApi.ts` - API 调用封装
- `useDevice.ts` - 设备检测
- `useEChartsTheme.ts` - ECharts 主题
- `useErrorHandler.ts` - 错误处理
- `useFontStyle.ts` - 字体样式
- `useMarkdown.ts` - Markdown 处理
- `useModuleSystem.ts` - 模块系统
- `useModuleTheme.ts` - 模块主题
- `useNaiveTheme.ts` - Naive UI 主题
- `useNaiveUI.ts` - Naive UI 封装
- `usePageStyle.ts` - 页面样式
- `useStyle.ts` - 样式管理
- `useTheme.ts` - 主题管理
- `useToast.ts` - 提示消息

**建议**：
- 检查是否有功能重复的 composables
- 考虑是否需要按功能模块组织（如 `composables/admin/`、`composables/visitor/`）

#### 2.3 类型定义组织检查

**需要检查**：
- [ ] 类型定义是否统一在 `types/` 目录
- [ ] 类型文件命名是否符合规范（kebab-case）
- [ ] 是否有重复的类型定义
- [ ] 类型定义是否完整（避免使用 `any`）

**当前类型文件**：
- `types/api.ts` - API 相关类型
- `types/module.ts` - 模块相关类型

**建议**：
- 检查组件中是否有内联类型定义，应提取到 `types/` 目录
- 检查是否有使用 `any` 类型的地方，应替换为具体类型

#### 2.4 样式文件组织检查

**需要检查**：
- [ ] 样式文件是否按功能模块组织
- [ ] 样式文件命名是否符合规范（kebab-case）
- [ ] 是否有重复的样式定义
- [ ] 样式文件是否在 `nuxt.config.ts` 中正确引用

**当前样式文件**：
- `assets/css/main.css` - 基础样式
- `assets/css/themes.css` - 主题样式
- `assets/css/visitor-interaction.css` - 访客互动样式
- `assets/css/header.css` - Header 样式（新建）
- `assets/css/footer.css` - Footer 样式（新建）
- `assets/css/about.css` - 关于页面样式
- `assets/css/tools.css` - 工具页面样式
- `assets/css/life.css` - 生活页面样式
- `assets/css/blog.css` - 博客页面样式
- `assets/css/projects.css` - 项目页面样式
- `assets/css/investment.css` - 投资页面样式
- `assets/css/charts.css` - 图表样式

**建议**：
- 检查是否有未使用的样式文件
- 检查是否有样式可以合并
- 确保所有样式文件都在 `nuxt.config.ts` 中引用

### 3. API 开发规范 ⚠️ **中优先级**

#### 3.1 后端 API 检查

**需要检查**：
- [ ] API 控制器是否符合 RESTful 规范
- [ ] API 响应格式是否统一（`ApiResponse<T>`）
- [ ] 错误处理是否完善
- [ ] API 文档是否完整（Swagger）

**待检查目录**：
- `backend/PersonalSite.Api/Controllers/` - API 控制器
- `backend/PersonalSite.Api/Models/` - 数据模型
- `backend/PersonalSite.Api/Data/` - 数据访问层

#### 3.2 前端 API 调用检查

**需要检查**：
- [ ] 是否统一使用 `useApi()` composable
- [ ] 错误处理是否完善
- [ ] API 调用是否有类型定义

**建议**：
- 检查是否有直接使用 `fetch` 的地方，应替换为 `useApi()`
- 检查错误处理是否统一

### 4. 数据库规范 ⚠️ **中优先级**

#### 4.1 数据库脚本组织检查

**需要检查**：
- [ ] 数据库脚本是否按功能模块组织
- [ ] 表命名是否符合规范（snake_case，功能前缀）
- [ ] 字段命名是否符合规范（snake_case）
- [ ] 是否有数据库文档

**待检查目录**：
- `database/` - 数据库脚本

**建议**：
- 检查表命名是否统一（如 `visitor_*`、`module_*`、`style_*`）
- 检查是否有数据库设计文档

### 5. 文档维护规范 ⚠️ **高优先级**

#### 5.1 文档完整性检查

**需要检查**：
- [ ] 功能文档是否与代码同步
- [ ] 功能实现状态是否更新
- [ ] API 文档是否完整
- [ ] 配置文档是否更新

**待检查文档**：
- `docs/features/IMPLEMENTATION_STATUS.md` - 功能实现状态
- `docs/config/API_CONFIG.md` - API 配置
- `docs/deployment/` - 部署文档
- `docs/development/DEVELOPMENT_GUIDELINES.md` - 开发规范

#### 5.2 文档更新检查

**需要检查**：
- [ ] 新功能是否有对应的文档
- [ ] Bug 修复是否记录在故障排除文档
- [ ] 配置变更是否更新文档

**建议**：
- 建立文档更新检查清单
- 在代码审查时检查文档更新

### 6. Git 提交规范 ⚠️ **低优先级**

#### 6.1 提交信息检查

**需要检查**：
- [ ] 提交信息是否符合规范（`<type>(<scope>): <subject>`）
- [ ] 提交信息是否清晰描述变更内容

**建议**：
- 使用 Git hooks 检查提交信息格式
- 在开发规范文档中强调提交规范

### 7. 其他规范检查 ⚠️ **低优先级**

#### 7.1 代码注释检查

**需要检查**：
- [ ] 复杂逻辑是否有中文注释
- [ ] 公共 API 是否有 JSDoc 注释

#### 7.2 性能优化检查

**需要检查**：
- [ ] 是否有性能问题
- [ ] 是否有可以优化的地方

## 📊 梳理优先级

### 优先级 1：高优先级（必须完成）

1. **Tailwind 类替换** - 继续处理高频组件
2. **文档维护规范** - 确保文档与代码同步

### 优先级 2：中优先级（建议完成）

1. **代码组织规范** - 检查组件、composables、类型定义组织
2. **API 开发规范** - 检查 API 规范和错误处理
3. **数据库规范** - 检查数据库脚本组织

### 优先级 3：低优先级（可选优化）

1. **Git 提交规范** - 检查提交信息格式
2. **代码注释** - 添加必要的注释
3. **性能优化** - 检查性能问题

## 📝 行动计划

### 短期（1-2 周）

1. ⏳ 继续 Tailwind 类替换（HeroSuper、RoadmapSection 等）
2. ⏳ 检查代码组织（组件、composables、类型定义）
3. ⏳ 检查文档完整性

### 中期（1 个月）

1. ⏳ 完成 Tailwind 类替换（所有组件）
2. ⏳ 完善代码组织
3. ⏳ 完善 API 和数据库规范

### 长期（持续）

1. ⏳ 定期代码规范检查
2. ⏳ 持续优化和改进
3. ⏳ 更新开发规范文档

## 🔗 相关文档

- [开发规范文档](./development/DEVELOPMENT_GUIDELINES.md)
- [代码规范检查报告](./quality/CODE_STANDARDS_CHECK.md)
- [代码重构进度报告](./quality/REFACTORING_PROGRESS.md)
- [项目概览](./PROJECT_OVERVIEW.md)

---

**创建时间**：2024-12-03  
**下次更新**：完成梳理任务后更新

