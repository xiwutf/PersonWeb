# PersonWeb Design System 执行路线图

**版本**: v1.0
**创建日期**: 2026-03-14
**预计总时长**: 4-6 周

---

## 概述

本路线图分为 5 个阶段，渐进式地建立 PersonWeb Design System：

| 阶段 | 目标 | 预计时长 | 复杂度 | 风险 |
|-----|------|---------|--------|------|
| **Phase 1** ✅ | Token 体系盘点与验证 | 1-2 天 | 🟢 低 | 无 |
| **Phase 2** ✅ | Naive UI Theme 统一 | 2-3 天 | 🟡 中 | 需仔细测试 |
| **Phase 3** | 后台 Pattern 抽象 | 5-7 天 | 🟠 中高 | 需重构多个页面 |
| **Phase 4** | 前台 Section Pattern 建立 | 3-5 天 | 🟡 中 | 保持灵活性 |
| **Phase 5** | 旧页面逐步治理 | 10-15 天 | 🟠 中高 | 逐个迁移 |

---

## Phase 1: Token 体系盘点与验证

### 目标

确保现有的 Token 体系完整、规范，可以作为 Design System 的基础。

### 具体任务

| 任务 | 工作量 | 产出 |
|------|--------|------|
| 1.1 梳理现有 CSS 变量 | 0.5 天 | Token 清单文档 |
| 1.2 验证变量覆盖范围 | 0.5 天 | 缺失变量清单 |
| 1.3 补充缺失变量 | 0.5 天 | 完整的 Token 体系 |
| 1.4 建立 Token 命名规范文档 | 0.5 天 | 命名规范文档 |

### 修改范围

- `assets/styles/theme-variables.css` - 补充缺失变量
- 新建 `docs/design-system/04-token-reference.md` - Token 参考文档

### 风险

| 风险 | 概率 | 影响 | 应对措施 |
|------|------|------|---------|
| 变量命名冲突 | 低 | 低 | 使用明确的命名前缀 |
| 遗漏关键变量 | 中 | 中 | 全面审查现有代码 |

### 验收标准

- ✅ 所有颜色值都有对应的 CSS 变量
- ✅ 所有间距值都使用 `--spacing-*` 变量
- ✅ 所有圆角值都使用 `--radius-*` 变量
- ✅ 所有阴影都使用 `--shadow-*` 变量
- ✅ Token 参考文档完整

### 交付物

1. `docs/design-system/04-token-reference.md` - Token 参考文档
2. 补充后的 `assets/styles/theme-variables.css`

---

## Phase 2: Naive UI Theme 统一

### 目标

完善 Naive UI 的 ThemeOverrides 配置，确保所有 Naive UI 组件都能响应主题切换。

### 具体任务

| 任务 | 工作量 | 产出 |
|------|--------|------|
| 2.1 审查现有 ThemeOverrides | 0.5 天 | 现有配置清单 |
| 2.2 补全缺失组件配置 | 1 天 | 完整的 ThemeOverrides |
| 2.3 修复硬编码样式问题 | 0.5 天 | 移除硬编码 |
| 2.4 测试主题切换 | 1 天 | 测试报告 |

### 修改范围

- `components/layout/AppNaiveConfig.vue` - 补全配置
- `assets/css/components.css` - 清理覆盖规则

### 风险

| 风险 | 概率 | 影响 | 应对措施 |
|------|------|------|---------|
| 部分组件不支持主题覆盖 | 低 | 中 | 使用 CSS 覆盖作为后备 |
| 主题切换导致样式错乱 | 中 | 高 | 建立测试用例覆盖 |

### 验收标准

- ✅ 所有常用 Naive UI 组件都支持主题切换
- ✅ 切换主题时无样式闪烁
- ✅ 无硬编码颜色值残留
- ✅ 测试报告通过

### 交付物

1. 完善后的 `components/layout/AppNaiveConfig.vue`
2. 简化后的 `assets/css/components.css`
3. 主题切换测试报告

---

## Phase 3: 后台 Pattern 抽象

### 目标

建立后台页面统一 Pattern，减少重复代码，提高开发效率。

### 具体任务

| 任务 | 工作量 | 产出 |
|------|--------|------|
| 3.1 设计 Pattern 接口 | 0.5 天 | Pattern 接口定义 |
| 3.2 实现 PageHeader 组件 | 0.5 天 | `PageHeader.vue` |
| 3.3 实现 FilterBar 组件 | 1 天 | `FilterBar.vue` |
| 3.4 实现 ListPage Pattern | 1 天 | `ListPage.vue` |
| 3.5 实现 FormPage Pattern | 1 天 | `FormPage.vue` |
| 3.6 实现 DetailPage Pattern | 0.5 天 | `DetailPage.vue` |
| 3.7 实现 ConfigPage Pattern | 0.5 天 | `ConfigPage.vue` |
| 3.8 迁移 2-3 个列表页 | 1 天 | 迁移示例 |
| 3.9 编写 Pattern 使用文档 | 1 天 | Pattern 指南 |

### 修改范围

**新建文件**:
```
components/patterns/admin/
├── PageHeader.vue
├── FilterBar.vue
├── ListPage.vue
├── FormPage.vue
├── DetailPage.vue
└── ConfigPage.vue
```

**迁移文件** (示例):
- `pages/admin/articles/index.vue` - 迁移到 ListPage
- `pages/admin/projects/index.vue` - 迁移到 ListPage
- `pages/admin/articles/edit.vue` - 迁移到 FormPage

### 风险

| 风险 | 概率 | 影响 | 应对措施 |
|------|------|------|---------|
| Pattern 覆盖度不够 | 中 | 中 | 允许页面自定义扩展 |
| 迁移破坏现有功能 | 低 | 高 | 充分测试迁移的页面 |
| 页面结构差异大 | 中 | 中 | Pattern 支持灵活配置 |

### 验收标准

- ✅ Pattern 接口定义清晰
- ✅ 所有 Pattern 组件通过测试
- ✅ 迁移示例页面功能正常
- ✅ Pattern 使用文档完整
- ✅ 新页面使用 Pattern 开发时间减少 50%

### 交付物

1. `components/patterns/admin/*.vue` - Pattern 组件
2. `docs/design-system/05-admin-pattern-guide.md` - Admin Pattern 指南
3. 迁移示例代码

---

## Phase 4: 前台 Section Pattern 建立

### 目标

建立前台 Section 组件，提升前台页面的一致性，同时保持灵活性。

### 具体任务

| 任务 | 工作量 | 产出 |
|------|--------|------|
| 4.1 设计 Section 接口 | 0.5 天 | Section 接口定义 |
| 4.2 实现 HeroSection 组件 | 0.5 天 | `HeroSection.vue` |
| 4.3 实现 FeatureGrid 组件 | 0.5 天 | `FeatureGrid.vue` |
| 4.4 实现 ProjectShowcase 组件 | 0.5 天 | `ProjectShowcase.vue` |
| 4.5 实现 CTASection 组件 | 0.5 天 | `CTASection.vue` |
| 4.6 实现 SectionHeader 组件 | 0.5 天 | `SectionHeader.vue` |
| 4.7 更新前台页面示例 | 1 天 | 使用 Section 的页面 |
| 4.8 编写 Section 使用文档 | 1 天 | Section 指南 |

### 修改范围

**新建文件**:
```
components/patterns/web/
├── HeroSection.vue
├── FeatureGrid.vue
├── ProjectShowcase.vue
├── CTASection.vue
└── SectionHeader.vue
```

**更新文件** (示例):
- `pages/about.vue` - 使用 HeroSection + FeatureGrid
- `pages/module-store.vue` - 使用 ProjectShowcase

### 风险

| 风险 | 概率 | 影响 | 应对措施 |
|------|------|------|---------|
| Section 限制页面创意 | 低 | 中 | Section 保持高度可配置 |
| 前台风格过于统一 | 低 | 低 | 前台不强求统一使用 |

### 验收标准

- ✅ Section 接口定义清晰
- ✅ 所有 Section 组件支持主题切换
- ✅ Section 可配置性强，不限制创意
- ✅ Section 使用文档完整
- ✅ 示例页面使用 Section 后视觉效果良好

### 交付物

1. `components/patterns/web/*.vue` - Section 组件
2. `docs/design-system/06-web-section-guide.md` - Web Section 指南
3. 更新后的前台页面示例

---

## Phase 5: 旧页面逐步治理

### 目标

将现有后台页面迁移到新的 Pattern，清理硬编码样式。

### 具体任务

| 任务 | 工作量 | 产出 |
|------|--------|------|
| 5.1 制定迁移计划 | 0.5 天 | 迁移优先级清单 |
| 5.2 迁移高优先级列表页 (8个) | 4 天 | 迁移完成 |
| 5.3 迁移表单页 (6个) | 3 天 | 迁移完成 |
| 5.4 迁移详情页 (4个) | 2 天 | 迁移完成 |
| 5.5 清理硬编码样式 | 2 天 | 移除硬编码 |
| 5.6 全量测试 | 2 天 | 测试报告 |
| 5.7 编写迁移文档 | 1 天 | 迁移指南 |

### 5.1 !important 清理策略（分阶段）

根据风险评估，分三批清理 `!important`：

#### 第一批：低风险区域（优先清理）

| 位置 | 涉及文件 | 风险等级 | 说明 |
|------|----------|----------|------|
| **components.css 中的 Tailwind 覆盖** | 352-599 行 | 🟢 低 | 这些是为了修复硬编码，清理后影响可控 |
| **前台页面的硬编码样式** | `assets/css/*.css` | 🟢 低 | 前台不强求主题统一，风险较小 |

**清理方法**:
```css
/* 删除前（禁止） */
.bg-white {
  background: var(--color-bg-elevated) !important;
}

/* 删除后（正确） */
.bg-bg-card {
  background: var(--color-bg-card);
}
```

#### 第二批：中风险区域（仔细评估）

| 位置 | 涉及文件 | 风险等级 | 说明 |
|------|----------|----------|------|
| **Naive UI 内部覆盖** | `ui-patch-naive.css` | 🟡 中 | 需确认是否还有必要保留 |
| **第三方库样式覆盖** | 各页面组件 | 🟡 中 | 需测试替代方案 |

**清理方法**:
1. 先注释掉覆盖规则
2. 测试功能是否正常
3. 如果正常，则删除
4. 如果异常，寻找替代方案

#### 第三批：高风险区域（暂不处理）

| 位置 | 涉及文件 | 风险等级 | 说明 |
|------|----------|----------|------|
| **动态计算的样式** | inline styles | 🟠 高 | 动态计算样式仍需内联 |
| **紧急修复的 hack** | 各处 | 🟠 高 | 标记 TODO，暂不处理 |

#### !important 清理检查清单

- [ ] 第一批低风险区域全部清理完成
- [ ] 第二批中风险区域评估完成
- [ ] 保留的 `!important` 都有明确注释说明
- [ ] 清理后所有页面主题切换正常
- [ ] 清理后所有 Naive UI 组件显示正常

#### 清理时的风险控制

| 风险 | 应对措施 |
|------|---------|
| 清理导致 Naive UI 样式错乱 | 先在单独分支清理，充分测试后合并 |
| 清理后主题切换失效 | 每清理一批后立即测试所有主题 |
| 遗漏必要的覆盖 | 建立自动化测试覆盖关键场景 |

### 5.2 真实页面迁移清单

#### 后台页面完整清单（按 Pattern 分类）

| Pattern | 文件路径 | 优先级 | 复杂度 | 备注 |
|---------|----------|--------|--------|------|
| **ListPage** | `pages/admin/index.vue` | P1 | 🟡 中 | Dashboard，有特殊逻辑，需评估 |
| **ListPage** | `pages/admin/articles/index.vue` | P1 | 🟢 低 | 标准列表页，适合迁移 |
| **ListPage** | `pages/admin/articles/[id]/edit.vue` | P1 | 🟢 低 | 标准编辑页 |
| **ListPage** | `pages/admin/projects/index.vue` | P1 | 🟢 低 | 标准列表页 |
| **ListPage** | `pages/admin/categories.vue` | P1 | 🟢 低 | 简单列表页 |
| **ListPage** | `pages/admin/orders/index.vue` | P1 | 🟡 中 | 有特殊状态展示 |
| **ListPage** | `pages/admin/orders/[id]/index.vue` | P2 | 🟠 中高 | 复杂订单详情页 |
| **ListPage** | `pages/admin/consultations/index.vue` | P1 | 🟢 低 | 标准列表页 |
| **ListPage** | `pages/admin/consultations/[id]/index.vue` | P2 | 🟡 中 | 聊天界面，不适合迁移 |
| **ListPage** | `pages/admin/modules/index.vue` | P1 | 🟢 低 | 标准列表页 |
| **ListPage** | `pages/admin/modules/[moduleKey]/index.vue` | P2 | 🟡 中 | 动态路由，需特殊处理 |
| **ListPage** | `pages/admin/modules/upload.vue` | P2 | 🟠 中高 | 上传页面，特殊交互 |
| **ListPage** | `pages/admin/intelligence/daily-report/index.vue` | P2 | 🟡 中 | 简单列表页 |
| **ListPage** | `pages/admin/intelligence/content/index.vue` | P2 | 🟡 中 | 标准列表页 |
| **ListPage** | `pages/admin/ai/index.vue` | P1 | 🟢 低 | 标准列表页 |
| **ListPage** | `pages/admin/ai/content.vue` | P2 | 🟡 中 | 简单内容页 |
| **ListPage** | `pages/admin/ai/logs.vue` | P2 | 🟢 低 | 简单列表页 |
| **ListPage** | `pages/admin/ai/support-config.vue` | P2 | 🟡 中 | 配置页，适合 ConfigPage |
| **ListPage** | `pages/admin/investment.vue` | P1 | 🟠 中高 | 复杂投资页，图表多 |
| **ListPage** | `pages/admin/price-alert.vue` | P1 | 🟡 中 | 标准列表页 |
| **ListPage** | `pages/admin/asset-management/index.vue` | P2 | 🟡 中 | 标准列表页 |
| **ListPage** | `pages/admin/analytics.vue` | P2 | 🟠 中高 | 数据分析页，图表多 |
| **ListPage** | `pages/admin/error-logs.vue` | P2 | 🟢 低 | 简单列表页 |
| **ListPage** | `pages/admin/document-agent.vue` | P2 | 🟡 中 | 文档管理，适合 ListPage |
| **ListPage** | `pages/admin/dca-plan.vue` | P2 | 🟠 中高 | DCA 计划页，图表多 |
| **ConfigPage** | `pages/admin/config.vue` | P1 | 🟢 低 | 系统配置，适合 ConfigPage |
| **ConfigPage** | `pages/admin/settings/index.vue` | P1 | 🟡 中 | 设置页，分类配置 |
| **ConfigPage** | `pages/admin/settings/styles.vue` | P3 | 🟠 中高 | 样式配置页，特殊交互 |
| **ConfigPage** | `pages/admin/settings/themes.vue` | P3 | 🟠 中高 | 主题配置页，特殊交互 |
| **ConfigPage** | `pages/admin/settings/modules.vue` | P2 | 🟡 中 | 模块设置，配置页 |
| **DashboardPage** | `pages/admin/dashboard/index.vue` | P2 | 🟠 中高 | 复杂 Dashboard，图表多 |
| **特殊** | `pages/admin/search.vue` | P3 | 🟢 低 | 搜索页，特殊交互 |
| **特殊** | `pages/admin/login.vue` | P3 | 🟢 低 | 登录页，特殊交互 |

#### 前台页面迁移清单（使用 Section Pattern）

| 文件路径 | 建议使用的 Section | 优先级 | 备注 |
|----------|-------------------|--------|------|
| `pages/index.vue` | HeroSection + FeatureGrid + ProjectShowcase | P2 | 首页，需保持灵活性 |
| `pages/about.vue` | HeroSection + FeatureGrid | P2 | 关于页，适合迁移 |
| `pages/module-store.vue` | HeroSection + ProjectShowcase | P1 | 模块商店，适合迁移 |
| `pages/ai-intro.vue` | HeroSection + FeatureGrid | P2 | AI 介绍页 |
| `pages/showcase.vue` | ProjectShowcase | P2 | 展示页 |
| `pages/contact.vue` | FormPage (简单表单） | P3 | 联系页，简单表单 |

### 5.3 工程化验收标准（可检查清单）

#### Token 使用检查

```bash
# 检查禁止的十六进制颜色
grep -rn '#[0-9a-fA-F]\{3,8\}' pages/ components/ assets/css/ | grep -v 'var(--color'

# 检查禁止的 Tailwind 颜色类
grep -rn 'class=".*\(bg-\|text-\|border-\)\(white\|black\|gray-\|red-\|blue-\|green-\|yellow-\|purple-\|orange-\|pink-\|cyan-\|indigo-\)' pages/ components/
```

**检查清单**:
- [ ] 新页面/组件 100% 使用 Token 变量（无十六进制颜色）
- [ ] 无新增硬编码 Tailwind 颜色类
- [ ] 无新增 `!important`（除非有明确审批）
- [ ] 所有间距使用 `--spacing-*` 变量或 Tailwind spacing 类
- [ ] 所有圆角使用 `--radius-*` 变量
- [ ] 所有阴影使用 `--shadow-*` 变量

#### Pattern 使用检查

```bash
# 检查是否还有重复的 page-header 结构
grep -rn 'class="page-header"' pages/admin/ | wc -l
# 目标：减少到 0（全部迁移到 PageHeader 组件）

# 检查是否还有重复的 filter-bar 结构
grep -rn 'class="filter-bar"' pages/admin/ | wc -l
# 目标：减少到 0（全部迁移到 FilterBar 组件）
```

**检查清单**:
- [ ] 高优先级页面全部使用对应 Pattern
- [ ] 无重复的 `page-header` 结构
- [ ] 无重复的 `filter-bar` 结构
- [ ] Pattern 组件的 Props 类型定义完整
- [ ] Pattern 组件的 Slots 文档完整

#### 主题切换检查

**手动测试场景**:
- [ ] Light 主题下所有页面显示正常
- [ ] Dark 主题下所有页面显示正常
- [ ] Tech Blue 主题下所有页面显示正常
- [ ] Paper 主题下所有页面显示正常
- [ ] Forest 主题下所有页面显示正常
- [ ] 主题切换时无样式闪烁
- [ ] 主题切换时所有组件都正确更新

#### 响应式检查

**测试断点**:
- [ ] Mobile (320px - 767px) 显示正常
- [ ] Tablet (768px - 1023px) 显示正常
- [ ] Desktop (1024px - 1279px) 显示正常
- [ ] Large Desktop (1280px+) 显示正常

#### 代码质量检查

```bash
# ESLint 检查
npm run lint

# Stylelint 检查
npm run stylelint

# TypeScript 检查
npm run type-check
```

**检查清单**:
- [ ] ESLint 检查无错误
- [ ] Stylelint 检查无错误
- [ ] TypeScript 检查无错误
- [ ] 所有 Pattern 组件有单元测试
- [ ] 单元测试覆盖率 ≥ 80%

### 修改范围

### 目标

将现有后台页面迁移到新的 Pattern，清理硬编码样式。

### 具体任务

| 任务 | 工作量 | 产出 |
|------|--------|------|
| 5.1 制定迁移计划 | 0.5 天 | 迁移优先级清单 |
| 5.2 迁移高优先级列表页 (8个) | 4 天 | 迁移完成 |
| 5.3 迁移表单页 (6个) | 3 天 | 迁移完成 |
| 5.4 迁移详情页 (4个) | 2 天 | 迁移完成 |
| 5.5 清理硬编码样式 | 2 天 | 移除硬编码 |
| 5.6 全量测试 | 2 天 | 测试报告 |
| 5.7 编写迁移文档 | 1 天 | 迁移指南 |

### 修改范围

#### Phase 1-2: Token 和 Theme

| 文件 | 操作 |
|------|------|
| `assets/styles/theme-variables.css` | 补充缺失的 Token 变量 |
| `components/layout/AppNaiveConfig.vue` | 完善 ThemeOverrides 配置 |
| `assets/css/components.css` | 清理第一、二批 !important 覆盖 |
| `docs/design-system/04-token-reference.md` | 新建 Token 参考文档 |

#### Phase 3: Admin Pattern

**新建文件**:
```
components/patterns/admin/
├── PageHeader.vue          # 页面头部组件
├── FilterBar.vue          # 筛选栏组件
├── ListPage.vue           # 列表页模板
├── FormPage.vue           # 表单页模板
├── DetailPage.vue         # 详情页模板
└── ConfigPage.vue         # 配置页模板
```

**迁移文件** (Phase 3 示例):
- `pages/admin/articles/index.vue` - 迁移到 ListPage
- `pages/admin/projects/index.vue` - 迁移到 ListPage
- `pages/admin/articles/[id]/edit.vue` - 迁移到 FormPage

#### Phase 4: Web Section

**新建文件**:
```
components/patterns/web/
├── HeroSection.vue       # Hero 区域
├── FeatureGrid.vue       # 功能网格
├── ProjectShowcase.vue  # 项目展示
├── CTASection.vue       # CTA 区域
└── SectionHeader.vue    # Section 头部
```

**更新文件** (Phase 4 示例):
- `pages/about.vue` - 使用 HeroSection + FeatureGrid
- `pages/module-store.vue` - 使用 ProjectShowcase

#### Phase 5: 页面迁移完整清单

**迁移页面列表** (按优先级排序):

| 优先级 | 页面 | 迁移到 Pattern |
|--------|------|--------------|
| P1 | `pages/admin/articles/index.vue` | ListPage |
| P1 | `pages/admin/projects/index.vue` | ListPage |
| P1 | `pages/admin/orders/index.vue` | ListPage |
| P1 | `pages/admin/articles/edit.vue` | FormPage |
| P1 | `pages/admin/projects/edit.vue` | FormPage |
| P1 | `pages/admin/modules/index.vue` | ListPage |
| P2 | `pages/admin/categories.vue` | ConfigPage |
| P2 | `pages/admin/config.vue` | ConfigPage |
| P2 | `pages/admin/consultations/index.vue` | ListPage |
| P2 | `pages/admin/analytics.vue` | DashboardPage |
| P3 | 其他页面 | 按需迁移 |

### 风险

| 风险 | 概率 | 影响 | 应对措施 |
|------|------|------|---------|
| 迁移破坏功能 | 低 | 高 | 每个页面充分测试 |
| 迁移工作量大 | 中 | 中 | 按优先级逐步进行 |
| 特殊页面无法迁移 | 低 | 低 | 保留特殊处理 |

### 验收标准

#### 自动化检查（脚本可运行）

```bash
#!/bin/bash
# acceptance-check.sh

echo "=== PersonWeb Design System 验收检查 ==="

# 1. Token 使用检查
echo "1. 检查硬编码颜色..."
if grep -rn '#[0-9a-fA-F]\{3,8\}' pages/ components/ assets/css/ | grep -v 'var(--color' | grep -v 'comment'; then
    echo "❌ 发现硬编码颜色"
    exit 1
else
    echo "✅ 无硬编码颜色"
fi

# 2. Tailwind 颜色类检查
echo "2. 检查禁止的 Tailwind 颜色类..."
if grep -rn 'class=".*\(bg-\|text-\|border-\)\(white\|black\|gray-[0-9]\|red-[0-9]\|blue-[0-9]\)' pages/ components/; then
    echo "❌ 发现禁止的 Tailwind 颜色类"
    exit 1
else
    echo "✅ 无禁止的 Tailwind 颜色类"
fi

# 3. Pattern 使用检查
echo "3. 检查重复的 page-header 结构..."
count=$(grep -rn 'class="page-header"' pages/admin/ 2>/dev/null | wc -l)
if [ "$count" -gt 0 ]; then
    echo "⚠️ 发现 $count 处 page-header 结构未迁移"
else
    echo "✅ page-header 结构已全部迁移"
fi

# 4. !important 检查
echo "4. 检查新增的 !important..."
# 保留已标记的 !important，检查新增的（TODO 标记的）
if grep -rn '!important' assets/css/ pages/ components/ | grep -v 'TODO' | grep -v 'Naive UI' | grep -v '第三方库'; then
    echo "⚠️ 发现未标记的 !important"
else
    echo "✅ !important 都有标记"
fi

echo "=== 验收检查完成 ==="
```

#### 功能测试清单（手动检查）

**Theme 验收**:
- [ ] Light 主题: 所有页面显示正常，无文字不可见
- [ ] Dark 主题: 所有页面显示正常，无过度反差
- [ ] Tech Blue 主题: 所有页面显示正常
- [ ] Paper 主题: 所有页面显示正常
- [ ] Forest 主题: 所有页面显示正常
- [ ] 主题切换: 无闪烁，无样式错乱
- [ ] Naive UI 组件: 所有主题下都正确显示

**响应式验收**:
- [ ] Mobile (320px): 布局正常，可操作
- [ ] Mobile (375px): 布局正常，可操作
- [ ] Mobile (414px): 布局正常，可操作
- [ ] Tablet (768px): 布局正常，可操作
- [ ] Desktop (1024px): 布局正常，可操作
- [ ] Desktop (1280px): 布局正常，可操作
- [ ] Desktop (1440px): 布局正常，可操作
- [ ] Desktop (1920px): 布局正常，可操作

**功能验收（按 Pattern）**:

**ListPage 验收**:
- [ ] 页面头部显示正确
- [ ] 筛选栏功能正常
- [ ] 表格数据正确显示
- [ ] 分页功能正常
- [ ] 操作按钮响应正确
- [ ] 行选择功能正常（如果有）

**FormPage 验收**:
- [ ] 表单验证功能正常
- [ ] 提交功能正常
- [ ] 取消功能正常
- [ ] 表单布局正常
- [ ] 错误提示正确显示

**DetailPage 验收**:
- [ ] 数据正确显示
- [ ] 操作按钮响应正确
- [ ] 关联数据正确显示

**ConfigPage 验收**:
- [ ] 配置项正确显示
- [ ] 保存功能正常
- [ ] 配置验证正常

**DashboardPage 验收**:
- [ ] KPI 卡片正确显示
- [ ] 图表正确渲染
- [ ] 待办事项正确显示
- [ ] 数据实时更新

#### 代码质量验收

**ESLint 检查**:
- [ ] `npm run lint` 无错误
- [ ] `npm run lint` 无警告（或有明确理由的警告）

**TypeScript 检查**:
- [ ] `npm run type-check` 无错误
- [ ] 类型定义完整

**单元测试验收**:
- [ ] Pattern 组件有单元测试
- [ ] 单元测试覆盖率 ≥ 80%
- [ ] 所有测试通过 `npm run test`

**样式检查**:
- [ ] `npm run stylelint` 无错误
- [ ] 无未使用的 CSS

### 交付物

1. 迁移后的页面文件
2. 迁移测试报告
3. `docs/design-system/07-migration-guide.md` - 迁移指南

---

## 时间规划

### Week 1-2: Phase 1-2

| 时间 | 阶段 | 任务 |
|------|------|------|
| Day 1-2 | Phase 1 | Token 体系盘点与验证 |
| Day 3-5 | Phase 2 | Naive UI Theme 统一 |

### Week 3-4: Phase 3

| 时间 | 任务 |
|------|------|
| Day 1-2 | 设计 Pattern 接口 + 实现 PageHeader、FilterBar |
| Day 3 | 实现 ListPage Pattern |
| Day 4 | 实现 FormPage Pattern |
| Day 5 | 实现 DetailPage、ConfigPage Pattern + 迁移示例 |

### Week 5: Phase 4

| 时间 | 任务 |
|------|------|
| Day 1-2 | 实现 Section 组件 |
| Day 3 | 更新前台页面示例 |
| Day 4-5 | 编写 Pattern 和 Section 文档 |

### Week 6-10: Phase 5

| 时间 | 任务 |
|------|------|
| Week 6-7 | 迁移高优先级列表页 + 表单页 |
| Week 8 | 迁移详情页 + 配置页 |
| Week 9 | 清理硬编码样式 + 全量测试 |
| Week 10 | 编写迁移文档 + 收尾 |

---

## 资源需求

### 人力资源

| 角色 | 投入时间 | 职责 |
|------|---------|------|
| 前端开发 | 80-100% | 实现所有阶段 |
| 设计师 | 20-30% | 审核 Pattern 设计、视觉规范 |
| 测试 | 50% | 测试每个阶段交付物 |

### 技术资源

- 开发环境：Node.js 18+, npm/pnpm
- 测试环境：CI/CD 环境
- 文档工具：Markdown 静态站点生成器

---

## 里程碑

| 里程碑 | 时间 | 标志 |
|--------|------|------|
| **M1: Token 完成** | Week 2 | Token 参考文档发布 |
| **M2: Theme 完成** | Week 2 | 主题切换无硬编码 |
| **M3: Pattern 完成** | Week 4 | Admin Pattern 组件发布 |
| **M4: Section 完成** | Week 5 | Web Section 组件发布 |
| **M5: 迁移完成** | Week 10 | 所有高优先级页面迁移完成 |

---

## 质量保障

### 测试策略

| 测试类型 | 覆盖范围 | 工具 |
|---------|---------|------|
| 单元测试 | Pattern 组件 | Vitest |
| 视觉回归测试 | 页面样式 | Playwright |
| 主题切换测试 | 主题切换 | 手动 + 自动化 |
| 响应式测试 | 不同屏幕尺寸 | Playwright |

### 代码审查

| 审查项 | 审查者 |
|--------|--------|
| Pattern 接口设计 | 技术负责人 |
| 组件实现质量 | 同行评审 |
| 文档完整性 | 技术写作 |

---

## 沟通计划

| 会议类型 | 频率 | 参与者 |
|---------|------|--------|
| 周进度会议 | 每周 | 全体成员 |
| 设计评审 | 按需 | 设计师 + 开发 |
| 技术评审 | 按需 | 技术负责人 |

---

## 风险管理

### 整体风险

| 风险 | 概率 | 影响 | 应对措施 |
|------|------|------|---------|
| 进度延期 | 中 | 中 | 留出 20% 缓冲时间 |
| 需求变更 | 低 | 中 | 保持 Pattern 灵活性 |
| 人员流动 | 低 | 高 | 完善文档，降低依赖 |

### 应急预案

| 场景 | 应急措施 |
|------|---------|
| Pattern 无法覆盖某些页面 | 保留自定义选项 |
| 迁移破坏生产环境 | 建立回滚机制 |
| 主题切换出现问题 | 保留原有主题方案 |

---

## 后续计划

### Phase 6: 文档与培训 (v1.1)

- 完整的组件库文档
- Design System 网站
- 开发者培训

### Phase 7: 扩展与优化 (v1.2)

- 新增更多 Pattern
- 性能优化
- 可访问性改进

---

**路线图完成** ✅
