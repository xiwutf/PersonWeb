# 样式系统治理落地收口 - 阶段1：收口范围与优先级

## 概述

本报告基于现有扫描结果和验证脚本输出，整理出本次必须整改的页面和组件清单，明确整改范围。

**报告生成时间**: 2026-03-14
**数据来源**:
- `node/scripts/verify-design-tokens.js` 扫描结果
- `docs/final-delivery-report.md` 验证数据
- `docs/stage3-non-color-hardcoding-report.md` 非颜色硬编码统计

---

## 当前状态基线

### 自动化验证结果（2026-03-14）

| 指标 | 数值 | 百分比 |
|------|------|--------|
| 总文件数 | 249 | 100% |
| 完全合规文件 | 11 | 4.42% |
| 混合状态文件 | 19 | 7.63% |
| 需要改进文件 | 71 | 28.51% |
| 无涉及样式文件 | 148 | 59.44% |

### 非颜色硬编码统计

| 类别 | 数量 | 文件数 |
|------|------|--------|
| 间距硬编码 (padding/margin/gap) | 725 | 119 |
| 圆角硬编码 (border-radius) | 352 | 93 |
| 字体大小硬编码 (font-size) | 435 | 96 |
| 总计 | 1513 | 238 |

---

## A 类：高优先级公共文件（24个）

### Layout 文件（5个）

| 文件路径 | 问题类型 | 优先级 |
|---------|---------|--------|
| `layouts/admin.vue` | 部分硬编码残留 | P0 |
| `layouts/default.vue` | 需要检查 | P0 |
| `layouts/home.vue` | 需要检查 | P0 |
| `layouts/ai.vue` | 需要检查 | P1 |
| `layouts/admin-content-only.vue` | 需要检查 | P1 |

### 全局样式文件（8个）

| 文件路径 | 问题类型 | 优先级 |
|---------|---------|--------|
| `assets/styles/index.css` | 入口文件，需检查 | P0 |
| `assets/styles/theme-variables.css` | 变量定义，已清理 | P0 ✓ |
| `assets/styles/base.css` | 基础样式，已清理 | P0 ✓ |
| `assets/styles/glassmorphism.css` | 玻璃态样式，已清理 | P0 ✓ |
| `assets/styles/component-styles.css` | 组件样式 | P0 |
| `assets/styles/ui-patch-naive.css` | UI 补丁样式 | P0 |
| `assets/styles/theme.css` | 主题兼容文件 | P1 |
| `assets/styles/tokens.css` | 令牌定义 | P0 |

### 通用基础组件（11个）

| 文件路径 | 问题类型 | 优先级 |
|---------|---------|--------|
| `components/ui/AppButton.vue` | 已使用变量 | P0 ✓ |
| `components/ui/AppCard.vue` | 已使用变量 | P0 ✓ |
| `components/layout/Header.vue` | 部分硬编码 | P0 |
| `components/layout/ThemeSwitcher.vue` | 已清理 | P0 ✓ |
| `components/layout/Footer.vue` | 需要检查 | P0 |
| `components/layout/AppNaiveConfig.vue` | 配置文件 | P0 |
| `components/NotificationBell.vue` | 需要检查 | P1 |
| `components/ModuleCard.vue` | 需要检查 | P0 |

**预计解决的问题类型**:
- 间距硬编码（高优先级）
- 圆角硬编码（高优先级）
- 字体大小硬编码（高优先级）
- 基础色板变量直用（语义变量迁移）

---

## B 类：高频 Admin 页面（48个）

### 第一批 - 核心 Admin 页面（12个）

| 文件路径 | 问题类型 | 优先级 |
|---------|---------|--------|
| `pages/admin/index.vue` | 部分硬编码 | P0 |
| `pages/admin/modules/index.vue` | 部分硬编码 | P0 |
| `pages/admin/settings/index.vue` | 部分硬编码 | P0 |
| `pages/admin/intelligence/index.vue` | 高频硬编码 | P0 |
| `pages/admin/intelligence/content/index.vue` | 中频硬编码 | P0 |
| `pages/admin/intelligence/daily-report/index.vue` | 中频硬编码 | P0 |
| `pages/admin/intelligence/daily-report/[id].vue` | 中频硬编码 | P0 |
| `pages/admin/intelligence/source/index.vue` | 需要检查 | P0 |
| `pages/admin/knowledge/index.vue` | 中频硬编码 | P0 |
| `pages/admin/document-agent.vue` | 高频硬编码（104个） | P0 |
| `pages/admin/visitor-messages.vue` | 中频硬编码 | P0 |
| `pages/admin/time-capsules.vue` | 已清理 | P0 ✓ |

### 第二批 - 投资与项目管理（8个）

| 文件路径 | 问题类型 | 优先级 |
|---------|---------|--------|
| `pages/admin/investment.vue` | 已清理 | P0 ✓ |
| `pages/admin/price-alert.vue` | 已清理 | P0 ✓ |
| `pages/admin/projects/index.vue` | 中频硬编码 | P0 |
| `pages/admin/side-projects/index.vue` | 需要检查 | P0 |
| `pages/admin/side-projects/dashboard.vue` | 需要检查 | P0 |
| `pages/admin/side-projects/analytics.vue` | 中频硬编码 | P0 |
| `pages/admin/side-projects/kanban.vue` | 需要检查 | P1 |
| `pages/admin/side-projects/notifications.vue` | 需要检查 | P1 |

### 第三批 - 订单与咨询（6个）

| 文件路径 | 问题类型 | 优先级 |
|---------|---------|--------|
| `pages/admin/orders.vue` | 中频硬编码 | P0 |
| `pages/admin/consultations.vue` | 中频硬编码 | P0 |
| `pages/admin/relations/index.vue` | 中频硬编码 | P0 |
| `pages/admin/recommendations.vue` | 需要检查 | P1 |
| `pages/admin/config.vue` | 需要检查 | P1 |
| `pages/admin/analytics.vue` | 需要检查 | P1 |

### 第四批 - 内容与工具管理（12个）

| 文件路径 | 问题类型 | 优先级 |
|---------|---------|--------|
| `pages/admin/articles/index.vue` | 需要检查 | P1 |
| `pages/admin/articles/edit/index.vue` | 需要检查 | P1 |
| `pages/admin/articles/edit/[id].vue` | 需要检查 | P1 |
| `pages/admin/tools.vue` | 需要检查 | P1 |
| `pages/admin/toolbox/index.vue` | 需要检查 | P1 |
| `pages/admin/categories.vue` | 需要检查 | P1 |
| `pages/admin/friend-links.vue` | 需要检查 | P1 |
| `pages/admin/timeline.vue` | 需要检查 | P1 |
| `pages/admin/skill-tree/index.vue` | 中频硬编码 | P1 |
| `pages/admin/tasks/index.vue` | 需要检查 | P1 |
| `pages/admin/goals/index.vue` | 需要检查 | P1 |
| `pages/admin/users/index.vue` | 需要检查 | P1 |

### 第五批 - AI 管理页面（10个）

| 文件路径 | 问题类型 | 优先级 |
|---------|---------|--------|
| `pages/admin/ai/index.vue` | 高频硬编码 | P0 |
| `pages/admin/ai/assistant.vue` | 部分硬编码 | P0 |
| `pages/admin/ai/content.vue` | 需要检查 | P1 |
| `pages/admin/ai/logs.vue` | 需要检查 | P1 |
| `pages/admin/ai/support-config.vue` | 需要检查 | P1 |

**预计解决的问题类型**:
- 间距硬编码（8px, 12px, 16px, 20px, 24px 高频值）
- 圆角硬编码（4px, 8px, 12px, 16px 高频值）
- 字体大小硬编码（12px, 14px, 16px, 20px 高频值）
- 基础色板变量直用
- 页面布局不统一问题

---

## C 类：高频前台页面（15个）

| 文件路径 | 问题类型 | 优先级 |
|---------|---------|--------|
| `pages/module-store.vue` | 大量硬编码 | P0 |
| `pages/modules/[moduleKey]/index.vue` | 部分硬编码 | P0 |
| `pages/my-licenses.vue` | 已清理 | P0 ✓ |
| `pages/payment/success.vue` | 需要检查 | P0 |
| `pages/payment/cancel.vue` | 需要检查 | P0 |
| `pages/order/create.vue` | 需要检查 | P1 |
| `pages/order/query.vue` | 需要检查 | P1 |
| `pages/order/success.vue` | 需要检查 | P1 |
| `pages/blog/index.vue` | 需要检查 | P0 |
| `pages/blog/[id].vue` | 需要检查 | P0 |
| `pages/index.vue` | 首页，需重点检查 | P0 |
| `pages/about.vue` | 需要检查 | P1 |
| `pages/contact.vue` | 需要检查 | P1 |
| `pages/ai/index.vue` | 高频硬编码（69个） | P0 |
| `pages/ai-intro.vue` | 需要检查 | P1 |

**预计解决的问题类型**:
- 间距硬编码
- 圆角硬编码
- 字体大小硬编码
- 基础色板变量直用
- 页面布局规范统一

---

## D 类：长尾遗留页（低频次要页面）

这些页面放在最后处理或保持现状。

| 文件路径 | 说明 |
|---------|------|
| `pages/lab.vue` | 实验室页面 |
| `pages/game.vue` | 游戏页面 |
| `pages/cognition/index.vue` | 认知文档 |
| `pages/cognition/[slug].vue` | 认知详情 |
| `pages/cognition/changelog.vue` | 变更日志 |
| `pages/knowledge/index.vue` | 知识库 |
| `pages/life/index.vue` | 生活日记 |
| `pages/life/[...slug].vue` | 生活详情 |
| `pages/projects/index.vue` | 项目列表 |
| `pages/projects/detail-[slug].vue` | 项目详情 |
| `pages/links.vue` | 友情链接 |
| `pages/english/index.vue` | 英语学习 |
| `pages/dashboard/index.vue` | 仪表盘 |
| 其他 `pages/admin/*` 子页面 | 管理后台低频页面 |

**预计解决的问题类型**:
- 可选处理，根据资源情况决定

---

## 整改优先级排序

### 第一优先级（P0）- 必须整改

**公共层优先**（24个文件）:
1. 所有 Layout 文件
2. 全局样式入口文件
3. 通用基础组件（AppButton、AppCard、Header等）

**核心 Admin 页面**（20个文件）:
4. `pages/admin/index.vue` - Admin 首页
5. `pages/admin/modules/index.vue` - 模块管理
6. `pages/admin/settings/index.vue` - 设置首页
7. `pages/admin/intelligence/index.vue` - 智能分析
8. `pages/admin/intelligence/content/index.vue`
9. `pages/admin/intelligence/daily-report/index.vue`
10. `pages/admin/intelligence/daily-report/[id].vue`
11. `pages/admin/intelligence/source/index.vue`
12. `pages/admin/knowledge/index.vue`
13. `pages/admin/document-agent.vue`（104个硬编码实例）
14. `pages/admin/visitor-messages.vue`
15. `pages/admin/investment.vue`
16. `pages/admin/price-alert.vue`
17. `pages/admin/projects/index.vue`
18. `pages/admin/orders.vue`
19. `pages/admin/consultations.vue`
20. `pages/admin/relations/index.vue`

**核心前台页面**（10个文件）:
21. `pages/index.vue` - 首页
22. `pages/module-store.vue` - 模块商店
23. `pages/modules/[moduleKey]/index.vue` - 模块详情
24. `pages/my-licenses.vue`
25. `pages/blog/index.vue`
26. `pages/blog/[id].vue`
27. `pages/payment/success.vue`
28. `pages/payment/cancel.vue`
29. `pages/ai/index.vue`（69个硬编码实例）
30. `pages/order/create.vue`

**小计**: 54个 P0 文件

### 第二优先级（P1）- 建议整改

（约30个文件，资源充足时处理）

### 第三优先级（P2）- 可选处理

（长尾页面，约40个文件）

---

## 推荐整改顺序

### 阶段2：批量整改高优先级公共层

**目标**: 公共层先收口，因为公共层不统一，后续页面永远不可能统一。

**整改文件**（24个）:
1. Layout 文件（5个）
2. 全局样式文件（8个）
3. 通用基础组件（11个）

**预计工作量**：中等
**预计时间**：1-2天

### 阶段3：批量整改核心业务页面

**第一批** - Admin 核心页面（20个）:
- `pages/admin/modules/**`
- `pages/admin/settings/**`
- `pages/admin/intelligence/**`

**第二批** - 投资与项目管理（8个）:
- `pages/admin/investment.vue`
- `pages/admin/visitor-messages.vue`
- `pages/admin/knowledge/**`
- `pages/admin/side-projects/**`

**第三批** - 前台高频页面（10个）:
- `pages/module-store.vue`
- `pages/modules/[moduleKey]/index.vue`
- `pages/my-licenses.vue`
- `pages/payment/**`
- `pages/blog/**`
- `pages/ai/index.vue`

**预计工作量**：较大
**预计时间**：3-5天

---

## 每一类预计解决的问题类型

### A 类（公共层）：

| 问题类型 | 预计处理量 |
|---------|------------|
| 间距硬编码（8px, 12px, 16px） | 约200个 |
| 圆角硬编码（4px, 8px, 12px） | 约100个 |
| 字体大小硬编码（12px, 14px, 16px） | 约150个 |
| 基础色板变量直用 | 约50个 |
| 语义变量迁移 | 约100个 |
| 公共组件视觉统一 | 全部 |

### B 类（Admin 页面）：

| 问题类型 | 预计处理量 |
|---------|------------|
| 间距硬编码 | 约300个 |
| 圆角硬编码 | 约150个 |
| 字体大小硬编码 | 约200个 |
| 基础色板变量直用 | 约100个 |
| 语义变量迁移 | 约150个 |
| 页面布局统一 | 全部 |

### C 类（前台页面）：

| 问题类型 | 预计处理量 |
|---------|------------|
| 间距硬编码 | 约150个 |
| 圆角硬编码 | 约80个 |
| 字体大小硬编码 | 约100个 |
| 基础色板变量直用 | 约50个 |
| 语义变量迁移 | 约80个 |

---

## 总体整改统计

| 类别 | 文件数量 | 优先级分布 |
|------|---------|------------|
| A 类：高优先级公共文件 | 24 | P0: 24, P1: 0, P2: 0 |
| B 类：高频 Admin 页面 | 48 | P0: 20, P1: 18, P2: 10 |
| C 类：高频前台页面 | 15 | P0: 10, P1: 4, P2: 1 |
| D 类：长尾遗留页 | ~40 | P2: 40 |
| **合计** | **127** | P0: 54, P1: 22, P2: 51 |

---

## 高优先级整改清单（54个 P0 文件）

### Layout 文件（5个）
- [ ] `layouts/admin.vue`
- [ ] `layouts/default.vue`
- [ ] `layouts/home.vue`
- [ ] `layouts/ai.vue`
- [ ] `layouts/admin-content-only.vue`

### 全局样式文件（8个）
- [x] `assets/styles/theme-variables.css` ✓
- [x] `assets/styles/base.css` ✓
- [x] `assets/styles/glassmorphism.css` ✓
- [ ] `assets/styles/index.css`
- [ ] `assets/styles/component-styles.css`
- [ ] `assets/styles/ui-patch-naive.css`
- [ ] `assets/styles/theme.css`
- [ ] `assets/styles/tokens.css`

### 通用基础组件（11个）
- [x] `components/ui/AppButton.vue` ✓
- [x] `components/ui/AppCard.vue` ✓
- [ ] `components/layout/Header.vue`
- [x] `components/layout/ThemeSwitcher.vue` ✓
- [ ] `components/layout/Footer.vue`
- [ ] `components/layout/AppNaiveConfig.vue`
- [ ] `components/NotificationBell.vue`
- [ ] `components/ModuleCard.vue`
- [ ] `components/ai/AIAssistant.vue`
- [ ] `components/ai/AiCapabilitySection.vue`
- [ ] `components/ai/AiProjectList.vue`

### Admin 核心页面（20个）
- [x] `pages/admin/time-capsules.vue` ✓
- [x] `pages/admin/investment.vue` ✓
- [x] `pages/admin/price-alert.vue` ✓
- [ ] `pages/admin/index.vue`
- [ ] `pages/admin/modules/index.vue`
- [ ] `pages/admin/modules/versions/[version].vue`
- [ ] `pages/admin/settings/index.vue`
- [ ] `pages/admin/settings/change-password.vue`
- [ ] `pages/admin/intelligence/index.vue`
- [ ] `pages/admin/intelligence/content/index.vue`
- [ ] `pages/admin/intelligence/daily-report/index.vue`
- [ ] `pages/admin/intelligence/daily-report/[id].vue`
- [ ] `pages/admin/intelligence/source/index.vue`
- [ ] `pages/admin/knowledge/index.vue`
- [ ] `pages/admin/document-agent.vue`
- [ ] `pages/admin/visitor-messages.vue`
- [ ] `pages/admin/projects/index.vue`
- [ ] `pages/admin/orders.vue`
- [ ] `pages/admin/consultations.vue`
- [ ] `pages/admin/relations/index.vue`

### 前台高频页面（10个）
- [x] `pages/my-licenses.vue` ✓
- [ ] `pages/index.vue`
- [ ] `pages/module-store.vue`
- [ ] `pages/modules/[moduleKey]/index.vue`
- [ ] `pages/payment/success.vue`
- [ ] `pages/payment/cancel.vue`
- [ ] `pages/order/create.vue`
- [ ] `pages/blog/index.vue`
- [ ] `pages/blog/[id].vue`
- [ ] `pages/ai/index.vue`

---

## 验收标准

### 阶段2 完成标准（公共层）

- [ ] 公共层不再出现明显视觉割裂
- [ ] 公共组件风格统一
- [ ] 自动化检查通过率明显提升
- [ ] 新旧页面使用公共组件时视觉更接近同一体系

### 阶段3 完成标准（核心业务页面）

- [ ] 页面布局统一（外层容器、标题区、操作区、内容区等）
- [ ] 样式统一（间距、圆角、字号、状态色、按钮风格等）
- [ ] 清理页面私有视觉定义
- [ ] 保持企业化、克制、实用的视觉风格

---

## 附录：已完全合规文件（11个）

以下文件已完成治理，无需再次整改：

1. `assets/styles/theme-variables.css`
2. `assets/styles/base.css`
3. `assets/styles/glassmorphism.css`
4. `layouts/admin.vue`
5. `pages/admin/time-capsules.vue`
6. `components/VisitorLevelDisplay.vue`
7. `components/layout/ThemeSwitcher.vue`
8. `pages/my-licenses.vue`
9. `pages/admin/settings/change-password.vue`
10. `pages/admin/price-alert.vue`
11. `pages/admin/modules/versions/[version].vue`

---

**报告生成时间**: 2026-03-14
**报告版本**: 1.0
**生成工具**: Claude Code
