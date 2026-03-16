# 非颜色硬编码治理报告

## 概述

本报告总结了"样式系统治理第三阶段"中非颜色类硬编码的扫描和分析情况。

## 执行时间

- 开始时间：2026-03-14
- 结束时间：2026-03-14
- 执行阶段：阶段3 - 治理非颜色类硬编码（间距、圆角、阴影、字体等）

## 扫描结果汇总

### 整体统计

| 类别 | 数量 | 文件数 |
|------|------|--------|
| 间距硬编码 (padding/margin/gap) | 725 | 119 |
| 圆角硬编码 (border-radius) | 352 | 93 |
| 阴影硬编码 (box-shadow) | 1 | 1 |
| 字体大小硬编码 (font-size) | 435 | 96 |
| 总计 | 1513 | 238 |

### 优先级分析

#### 高优先级（>100次）

1. **间距硬编码** - 725 次在 119 个文件
   - 高频使用值：`8px`, `12px`, `16px`, `20px`, `24px`, `32px`, `40px`, `48px`
   - 中频使用值：`4px`, `6px`, `10px`, `14px`, `18px`, `28px`, `56px`, `80px`
   - 低频使用值：`2px`, `1px`, `0.5px`, `0.75px`, `1.25px`, `1.5px`, `2.5px`, `3px`, `5px`, `6px`, `7px`, `96px`, `128px`

2. **字体大小硬编码** - 435 次在 96 个文件
   - 高频使用值：`12px`, `14px`, `16px`, `20px`, `24px`, `32px`
   - 中频使用值：`8px`, `10px`, `18px`, `1rem`, `1.1rem`, `1.25rem`, `1.5rem`
   - 低频使用值：`0.75rem`, `0.875rem`, `0.8125rem`, `2.25rem`, `3rem`

#### 中优先级（50-100次）

1. **圆角硬编码** - 352 次在 93 个文件
   - 高频使用值：`4px`, `8px`, `12px`, `16px`
   - 中频使用值：`0.25rem`, `0.375rem`, `0.5rem`, `6px`, `9999px`, `2px`, `1px`, `24px`
   - 低频使用值：`0px`, `2px`, `32px`, `48px`, `50%`, `50%`, `1px`, `0.5px`, `3px`, `20px`, `6px`, `999px`

#### 低优先级（<50次）

1. **阴影硬编码** - 仅 1 次在 1 个文件

## 现有语义变量系统

### 可用的间距变量

```css
--spacing-0: 0px;
--spacing-px: 1px;
--spacing-0_5: 2px;
--spacing-1: 4px;
--spacing-1_5: 6px;
--spacing-2: 8px;
--spacing-2_5: 10px;
--spacing-3: 12px;
--spacing-3_5: 14px;
--spacing-4: 16px;
--spacing-5: 20px;
--spacing-6: 24px;
--spacing-7: 28px;
--spacing-8: 32px;
--spacing-9: 36px;
--spacing-10: 40px;
--spacing-11: 44px;
--spacing-12: 48px;
--spacing-14: 56px;
--spacing-16: 64px;
--spacing-20: 80px;
--spacing-24: 96px;
--spacing-28: 112px;
--spacing-32: 128px;
--spacing-36: 144px;
--spacing-40: 160px;
--spacing-44: 176px;
--spacing-48: 192px;
--spacing-56: 224px;
--spacing-64: 256px;
```

### 可用的圆角变量

```css
--radius-none: 0px;
--radius-sm: 4px;
--radius-base: 6px;
--radius-md: 8px;
--radius-lg: 12px;
--radius-xl: 16px;
--radius-2xl: 24px;
--radius-3xl: 32px;
--radius-full: 9999px;
```

### 可用的阴影变量

```css
--shadow-none: 0 0 #0000;
--shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.05);
--shadow-base: var(--shadow);
--shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1);
--shadow-lg: 0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1);
--shadow-xl: 0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1);
--shadow-2xl: 0 25px 50px -12px rgb(0 0 0 / 0.25);
--shadow-inner: inset 0 2px 4px 0 rgb(0 0 0 / 0.05);
--shadow-primary: 0 10px 15px -3px rgba(59, 130, 246, 0.2), 0 4px 6px -2px rgba(59, 130, 246, 0.1);
--shadow-text: 0 2px 4px rgba(0, 0, 0, 0.2);
```

### 可用的字体变量

```css
--font-size-base: 1rem;      /* 16px */
--font-size-h1: 2.25rem;    /* 36px */
--font-size-h2: 1.875rem;   /* 30px */
--font-size-h3: 1.5rem;     /* 24px */
--font-size-h4: 1.25rem;    /* 20px */
--text-xs: 0.75rem;    /* 12px */
--text-sm: 0.875rem;   /* 14px */
--text-base: var(--font-size-base);
--text-lg: 1.125rem;   /* 18px */
--text-xl: 1.25rem;    /* 20px */
--text-2xl: 1.5rem;    /* 24px */
```

## 常见硬编码值与语义变量映射

### 间距映射

| 硬编码值 | 语义变量 | 使用频率 | 优先级 |
|-----------|---------|---------|--------|
| `4px` | `var(--spacing-1)` | 高 | P0 |
| `6px` | `var(--spacing-1_5)` | 中 | P1 |
| `8px` | `var(--spacing-2)` | 高 | P0 |
| `10px` | `var(--spacing-2_5)` | 中 | P1 |
| `12px` | `var(--spacing-3)` | 高 | P0 |
| `14px` | `var(--spacing-3_5)` | 中 | P1 |
| `16px` | `var(--spacing-4)` | 高 | P0 |
| `20px` | `var(--spacing-5)` | 高 | P0 |
| `24px` | `var(--spacing-6)` | 高 | P0 |
| `28px` | `var(--spacing-7)` | 中 | P1 |
| `32px` | `var(--spacing-8)` | 高 | P0 |
| `40px` | `var(--spacing-10)` | 高 | P0 |
| `48px` | `var(--spacing-12)` | 高 | P0 |
| `56px` | `var(--spacing-14)` | 中 | P1 |
| `64px` | `var(--spacing-16)` | 低 | P2 |
| `80px` | `var(--spacing-20)` | 中 | P1 |

### 圆角映射

| 硬编码值 | 语义变量 | 使用频率 | 优先级 |
|-----------|---------|---------|--------|
| `4px` | `var(--radius-sm)` | 高 | P0 |
| `6px` | `var(--radius-base)` | 中 | P1 |
| `8px` | `var(--radius-md)` | 高 | P0 |
| `12px` | `var(--radius-lg)` | 中 | P1 |
| `16px` | `var(--radius-xl)` | 高 | P0 |
| `0px` | `var(--radius-none)` | 中 | P1 |

### 字体大小映射

| 硬编码值 | 语义变量 | 使用频率 | 优先级 |
|-----------|---------|---------|--------|
| `12px` | `var(--text-xs)` | 高 | P0 |
| `14px` | `var(--text-sm)` | 高 | P0 |
| `16px` | `var(--font-size-base)` | 高 | P0 |
| `20px` | `var(--text-xl)` | 高 | P0 |
| `24px` | `var(--text-2xl)` | 高 | P0 |
| `32px` | `var(--font-size-h1)` * 0.888 | 中 | P1 |

## 迁移策略与建议

### 1. 优先迁移高频使用值（P0 优先级）

以下高频率使用的值应优先迁移到语义变量：

```css
/* 间距迁移示例 */
padding: 8px;    →    padding: var(--spacing-2);
margin: 16px;     →     margin: var(--spacing-4);
gap: 12px;       →      gap: var(--spacing-3);
padding: 24px;   →    padding: var(--spacing-6);
margin: 32px;     →     margin: var(--spacing-8);

/* 圆角迁移示例 */
border-radius: 8px;  →  border-radius: var(--radius-md);
border-radius: 16px;  →  border-radius: var(--radius-xl);
border-radius: 12px;  →  border-radius: var(--radius-lg);

/* 字体迁移示例 */
font-size: 12px;   →    font-size: var(--text-xs);
font-size: 14px;   →    font-size: var(--text-sm);
font-size: 16px;   →    font-size: var(--font-size-base);
font-size: 20px;   →    font-size: var(--text-xl);
```

### 2. 组件样式迁移策略

对于 Vue 组件，建议使用组件级语义变量：

```css
/* 按钮组件 */
.btn {
  padding: var(--btn-padding-x-md) var(--btn-padding-y-md);
  border-radius: var(--btn-radius);
  font-size: var(--btn-font-size);
}

/* 卡片组件 */
.card {
  padding: var(--card-padding-md);
  border-radius: var(--card-radius);
}

/* 标签组件 */
.tag {
  padding: var(--tag-padding-x-md) var(--tag-padding-y-sm);
  border-radius: var(--tag-radius);
  font-size: var(--tag-font-size);
}
```

### 3. 特殊场景处理

#### 设计特定值

某些硬编码值是设计特定选择，不需要迁移：

```css
/* 特殊动画值 */
transform: translateY(-2px);  /* 设计特定动画值，保留 */

/* 特殊布局值 */
width: 100%;  /* 布局特定，保留 */
height: 100%;  /* 布局特定，保留 */

/* 特殊图标尺寸 */
font-size: 4rem;  /* 图标尺寸，可保留 */
```

#### 框架特定值

某些值由框架控制，应保持原样：

```css
/* Tailwind CSS 框架类 */
@apply ...;  /* 框架类，保持原样 */

/* 框架默认值 */
overflow: hidden;  /* 框架默认行为 */
position: relative;  /* 框架默认行为 */
```

### 4. 渐进式迁移策略

建议采用渐进式迁移方法，而非一次性全部迁移：

#### 阶段 1：优先迁移高频值

1. 间距：`8px`, `12px`, `16px`, `20px`, `24px`, `32px`
2. 圆角：`4px`, `8px`, `12px`, `16px`
3. 字体：`12px`, `14px`, `16px`, `20px`, `24px`

#### 阶段 2：迁移中频值

1. 间距：`4px`, `6px`, `10px`, `14px`, `40px`, `48px`, `80px`
2. 圆角：`6px`, `0px`
3. 字体：`8px`, `10px`, `18px`, `1rem`

#### 阶段 3：评估低频值

对于低频使用的值，逐个评估是否需要迁移。

## 验证命令

### 扫描当前硬编码状态

```bash
# 扫描间距硬编码
grep -r "padding:\s*\d+px\|margin:\s*\d+px\|gap:\s*\d+px" \
  --include="*.vue" \
  --include="*.css" \
  pages/ assets/ components/

# 扫描圆角硬编码
grep -r "border-radius:\s*\d+px" \
  --include="*.vue" \
  --include="*.css" \
  pages/ assets/ components/

# 扫描字体大小硬编码
grep -r "font-size:\s*\d+px" \
  --include="*.vue" \
  --include="*.css" \
  pages/ assets/ components/
```

### 验证语义变量使用

```bash
# 检查间距变量使用
grep -r "var(--spacing-\d)" \
  --include="*.vue" \
  --include="*.css" \
  pages/ assets/ components/

# 检查圆角变量使用
grep -r "var(--radius-\w\)" \
  --include="*.vue" \
  --include="*.css" \
  pages/ assets/ components/
```

### 统计硬编码减少

```bash
# 统计迁移前后的差异
echo "迁移前：$(grep -r "padding:\s*8px" pages/ assets/ components/ | wc -l)"
echo "迁移后：$(grep -r "var(--spacing-2)" pages/ assets/ components/ | wc -l)"
```

## 自动化检查建议

### Stylelint 规则

创建或更新 `.stylelintrc.js` 配置：

```javascript
module.exports = {
  rules: {
    // 禁止硬编码间距值
    'declaration-property-value-no-unknown': [
      true,
      {
        ignoreProperties: [
          // 允许的布局属性
          /^width$/,
          /^height$/,
          /^flex$/,
          /^grid$/,
        ]
      }
    ],

    // 建议使用语义变量
    'scale-unlimited/declaration-strict-value': [
      'error',
      {
        ignore: [
          'var\\(--',
          'calc\\(',
          'clamp\\(',
        ]
      }
    ]
  },

  // 自定义规则检查
  'plugin:no-hardcoded-spacing': true,
  'plugin:no-hardcoded-radius': true,
}
```

### Stylelint 插件规则

创建自定义插件 `stylelint-plugin-design-tokens`：

```javascript
// rules/no-hardcoded-spacing.js
module.exports = {
  ruleName: 'no-hardcoded-spacing',
  message: 'Use design token variable instead of hardcoded spacing value',
  meta: {
    type: 'suggestion',
    docs: 'https://docs.example.com/design-tokens',
    fixable: true,
  },
  create: (context) => {
    return {
      decl: (node) => {
        // 检测硬编码间距值
        if (node.prop === 'padding' || node.prop === 'margin') {
          const value = node.value;
          const hardcodedPattern = /^\d+(px|rem|em)$/;

          if (hardcodedPattern.test(value)) {
            context.report({
              node,
              message: `Use semantic spacing variable instead of ${value}`,
            });
          }
        }
      }
    }
  };
};
```

### ESLint 规则（Vue 文件）

创建 `.eslintrc.js` 配置：

```javascript
module.exports = {
  extends: [
    'plugin:vue/design-tokens/recommended',
  ],
  rules: {
    'vue/design-tokens/no-hardcoded-spacing': 'warn',
    'vue/design-tokens/no-hardcoded-radius': 'warn',
    'vue/design-tokens/no-hardcoded-font-size': 'warn',
  },
};
```

### NPM 脚本

在 `package.json` 中添加脚本：

```json
{
  "scripts": {
    "style:lint": "stylelint \"**/*.{vue,css,scss}\" --max-warnings 0",
    "style:lint:fix": "stylelint \"**/*.{vue,css,scss}\" --fix",
    "style:tokens:scan": "node scripts/scan-design-tokens.js",
    "style:tokens:verify": "node scripts/verify-design-tokens.js"
  }
}
```

## 文件优先级建议

### 高优先级文件（建议优先迁移）

以下文件有大量硬编码值，建议优先处理：

| 文件 | 间距 | 圆角 | 字体 | 总计 | 优先级 |
|------|------|------|------|------|--------|
| `pages/ai/index.vue` | 35 | 12 | 22 | 69 | P0 |
| `pages/admin/document-agent.vue` | 49 | 33 | 22 | 104 | P0 |
| `pages/admin/relations/index.vue` | 15 | 4 | 1 | 20 | P1 |
| `pages/admin/investment.vue` | 15 | 5 | 2 | 22 | P1 |
| `pages/admin/modules/index.vue` | 18 | 12 | 4 | 34 | P1 |
| `components/ai/AiCapabilitySection.vue` | 18 | 6 | 9 | 33 | P1 |
| `components/ai/AiProjectList.vue` | 8 | 6 | 8 | 22 | P1 |
| `components/ai/AIAssistant.vue` | 3 | 5 | 1 | 9 | P2 |
| `pages/admin/intelligence/daily-report/[id].vue` | 9 | 2 | 9 | 20 | P1 |
| `pages/admin/intelligence/daily-report/index.vue` | 9 | 5 | 14 | 28 | P1 |
| `pages/admin/intelligence/content/index.vue` | 6 | 5 | 14 | 25 | P1 |
| `pages/admin/intelligence/index.vue` | 14 | 0 | 8 | 22 | P1 |

### 中优先级文件

以下文件有中等数量的硬编码值：

| 文件 | 间距 | 圆角 | 字体 | 总计 | 优先级 |
|------|------|------|------|------|------|
| `pages/admin/knowledge/index.vue` | 18 | 4 | 4 | 26 | P1 |
| `pages/admin/modules/upload.vue` | 13 | 8 | 4 | 25 | P1 |
| `pages/admin/modules/versions/[version].vue` | 13 | 14 | 4 | 31 | P1 |
| `components/relations/modals/AddInteractionModal.vue` | 44 | 19 | 8 | 71 | P1 |
| `components/relations/AiSuggestionCard.vue` | 16 | 4 | 11 | 31 | P1 |
| `pages/admin/orders.vue` | 14 | 5 | 1 | 20 | P1 |
| `pages/admin/price-alert.vue` | 15 | 7 | 13 | 35 | P1 |
| `pages/admin/settings/change-password.vue` | 2 | 6 | 2 | 10 | P2 |
| `pages/admin/settings/modules.vue` | 3 | 3 | 2 | 8 | P2 |
| `pages/admin/consultations.vue` | 20 | 5 | 3 | 28 | P1 |

## 最佳实践建议

### 1. 新代码开发规范

#### 使用语义变量

```css
/* ✅ 推荐 */
.component {
  padding: var(--spacing-3);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
}

/* ❌ 避免 */
.component {
  padding: 12px;
  border-radius: 8px;
  font-size: 14px;
}
```

#### 使用 calc() 进行动态计算

```css
/* ✅ 推荐 */
.component {
  padding: calc(var(--spacing-3) + var(--spacing-2));
}

/* ❌ 避免 */
.component {
  padding: 20px; /* 12px + 8px */
}
```

#### 使用 clamp() 响应式

```css
/* ✅ 推荐 */
.component {
  padding: clamp(var(--spacing-2), var(--spacing-3), var(--spacing-4));
}

/* ❌ 避免 */
.component {
  padding: 12px; /* 固定值 */
}
```

### 2. 设计系统治理原则

1. **优先使用语义变量**：所有可复用的设计值都应该通过语义变量定义
2. **保持一致性**：相同用途的样式必须使用相同的语义变量
3. **层次化设计**：新设计值应该添加到主题变量系统，而不是直接硬编码
4. **文档化决策**：无法使用语义变量的特殊情况应该有文档说明

### 3. 代码审查检查清单

在代码审查时，检查以下项目：

- [ ] 新增的间距值是否使用了语义变量？
- [ ] 新增的圆角值是否使用了语义变量？
- [ ] 新增的字体大小是否使用了语义变量？
- [ ] 是否有硬编码值可以复用现有语义变量？
- [ ] 特殊设计值是否有文档说明？
- [ ] 是否遵循了现有的设计系统规范？

## 迁移示例

### 示例 1：间距迁移

```css
/* 迁移前 */
.container {
  padding: 16px;
  margin: 24px 0;
  gap: 12px;
}

/* 迁移后 */
.container {
  padding: var(--spacing-4);
  margin: var(--spacing-6) 0;
  gap: var(--spacing-3);
}
```

### 示例 2：圆角迁移

```css
/* 迁移前 */
.card {
  border-radius: 8px;
}

.card-header {
  border-radius: 4px 4px 0 0;
}

/* 迁移后 */
.card {
  border-radius: var(--radius-md);
}

.card-header {
  border-radius: var(--radius-sm) var(--radius-sm) 0 0;
}
```

### 示例 3：字体大小迁移

```css
/* 迁移前 */
.title {
  font-size: 20px;
}

.subtitle {
  font-size: 16px;
}

.caption {
  font-size: 12px;
}

/* 迁移后 */
.title {
  font-size: var(--text-xl);
}

.subtitle {
  font-size: var(--font-size-base);
}

.caption {
  font-size: var(--text-xs);
}
```

### 示例 4：组合迁移

```css
/* 迁移前 */
.btn-primary {
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
}

/* 迁移后 */
.btn-primary {
  padding: var(--spacing-3) var(--spacing-6);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
}
```

## 工作总结

### 已完成的工作

1. ✅ **全面扫描**
   - 扫描了 119 个文件的间距硬编码（725 个实例）
   - 扫描了 93 个文件的圆角硬编码（352 个实例）
   - 扫描了 96 个文件的字体大小硬编码（435 个实例）
   - 总计发现 1513 个硬编码实例

2. ✅ **映射关系建立**
   - 创建了硬编码值到语义变量的完整映射表
   - 按使用频率进行了优先级分类
   - 区分了可迁移和需要保留的场景

3. ✅ **迁移策略制定**
   - 提供了渐进式迁移方案
   - 定义了自动化检查工具配置
   - 给出了文件优先级建议

4. ✅ **文档完善**
   - 包含了验证命令和最佳实践
   - 提供了多个迁移示例
   - 建立了代码审查检查清单

### 下一步建议

#### 短期建议（后续迁移工作）

1. **优先处理高优先级文件**
   - `pages/ai/index.vue` - 69 个实例
   - `pages/admin/document-agent.vue` - 104 个实例
   - `components/ai/AiCapabilitySection.vue` - 33 个实例

2. **实施自动化检查**
   - 配置 Stylelint 自定义规则
   - 集成到 CI/CD 流程
   - 在开发环境中启用实时检查

3. **建立组件库**
   - 创建标准化组件（按钮、卡片、标签等）
   - 在组件内部使用语义变量
   - 减少组件间的样式重复

#### 长期建议（阶段4、5）

1. **建立页面布局规范**
   - 统一页面间距系统
   - 建立响应式断点标准
   - 创建常用布局模式

2. **建立自动化检查机制**
   - Stylelint 完整配置
   - ESLint 插件规则
   - NPM 脚本自动化
   - CI/CD 集成

3. **持续治理**
   - 定期审计新代码
   - 更新设计系统文档
   - 培训团队成员

## 附件

### A. 语义变量完整列表

详见 `assets/styles/theme-variables.css` 中的以下部分：

- **间距系统**（第 488-524 行）
- **圆角系统**（第 522-533 行）
- **阴影系统**（第 535-550 行）
- **字体系统**（第 552-609 行）

### B. 相关文档

- `docs/semantic-variable-mapping.md` - 语义变量映射表
- `docs/semantic-variable-migration-report.md` - 语义变量迁移报告
- `docs/style-fallback-cleanup-report.md` - fallback 清理报告
- `docs/硬编码颜色替换总结报告.md` - 硬编码颜色替换报告

### C. 验证脚本

参见"验证命令"章节的详细脚本。

---

**报告生成时间**: 2026-03-14
**报告版本**: 1.0
**生成工具**: Claude Code
