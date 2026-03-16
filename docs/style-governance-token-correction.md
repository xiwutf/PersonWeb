# 样式系统治理落地收口 - 阶段2.5：Token 体系梳理报告

## 概述

**报告生成时间**: 2026-03-14
**阶段**: 阶段2.5 - Token 体系梳理与纠偏
**目标**: 检查并补齐缺失的 token，建立正确的语义化变量体系

---

## 当前 Token 定义状况

### 1. 两个 Token 定义文件

项目中存在两个主要的 CSS 变量定义文件：

| 文件路径 | 作用 | 状态 |
|---------|------|------|
| `assets/styles/tokens.css` | V2 简化版 token，主要用于结构层 | 不完整 |
| `assets/styles/theme-variables.css` | V3 完整版 token，包含完整的设计系统 | 完整 |

**建议**：统一使用 `theme-variables.css` 作为唯一权威 token 源。

---

## Token 体系现状分析

### 字号 Token (Typography)

#### 当前定义（theme-variables.css）

```css
:root {
  --text-xs: 0.75rem;    /* 12px */
  --text-sm: 0.875rem;   /* 14px */
  --text-base: var(--font-size-base);  /* 16px */
  --text-lg: 1.125rem;   /* 18px */
  --text-xl: 1.25rem;    /* 20px */
  --text-2xl: 1.5rem;    /* 24px */
  --text-3xl: 1.875rem;  /* 30px */
  --text-4xl: 2.25rem;   /* 36px */
  --text-5xl: 3rem;      /* 48px */
  --text-6xl: 3.75rem;   /* 60px */
  --text-7xl: 4.5rem;    /* 72px */
  --text-8xl: 6rem;      /* 96px */
  --text-9xl: 8rem;      /* 128px */
}
```

#### 状态：✅ 完整

字号 token 已经非常完整，覆盖了从 12px 到 128px 的所有常用字号。

---

### 圆角 Token (Border Radius)

#### 当前定义（theme-variables.css）

```css
:root {
  --radius-none: 0px;
  --radius-sm: 4px;       /* 小圆角 */
  --radius-base: 6px;     /* 基础圆角 */
  --radius-md: 8px;      /* 中等圆角 */
  --radius-lg: 12px;     /* 大圆角 */
  --radius-xl: 16px;     /* 超大圆角 */
  --radius-2xl: 24px;    /* 特大圆角 */
  --radius-3xl: 32px;    /* 超特大圆角 */
  --radius-full: 9999px;  /* 完全圆形 */
}
```

#### 状态：✅ 完整

圆角 token 已经完整，覆盖了从 0px 到完全圆形的所有常用值。

---

### 间距 Token (Spacing)

#### 当前定义（theme-variables.css）

```css
:root {
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
  /* ... 更多 */
}
```

#### tokens.css 中的简化定义（已被部分组件使用）

```css
:root {
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;
}
```

#### 状态：⚠️ 部分完整，存在命名不一致

**问题**：
1. tokens.css 使用语义化命名（xs/sm/md/lg/xl）
2. theme-variables.css 使用数字命名（0/1/2/3...）
3. 部分组件使用了不存在的 token（如 `--spacing-xxl`, `--spacing-xxxl`）

**建议**：
1. 统一使用 theme-variables.css 的数字命名系统
2. 或者扩展 tokens.css 的语义化命名系统，覆盖所有间距值
3. 清除不存在的 token 引用（如 `--spacing-xxl`, `--spacing-xxxl`）

---

## 组件中发现的 Token 使用问题

### 1. 使用 spacing 代替字号（违反硬性规则）

| 组件文件 | 错误用法 | 应该使用 | 影响 |
|---------|---------|---------|------|
| AiCapabilitySection.vue | `font-size: var(--spacing-lg)` | `var(--text-2xl)` | 语义错误 |
| AiCapabilitySection.vue | `font-size: var(--spacing-xl)` | `var(--text-3xl)` | 语义错误 |
| AiCapabilitySection.vue | `font-size: var(--spacing-md)` | `var(--text-base)` | 语义错误 |
| AiProjectList.vue | `font-size: var(--spacing-xl)` | `var(--text-3xl)` | 语义错误 |
| AiProjectList.vue | `font-size: var(--spacing-md)` | `var(--text-base)` | 语义错误 |

### 2. 使用 spacing 代替圆角（违反硬性规则）

| 组件文件 | 错误用法 | 应该使用 | 影响 |
|---------|---------|---------|------|
| AiCapabilitySection.vue | `border-radius: var(--spacing-lg)` | `var(--radius-lg)` | 语义错误 |
| AiCapabilitySection.vue | `border-radius: var(--spacing-md)` | `var(--radius-md)` | 语义错误 |
| AiCapabilitySection.vue | `border-radius: var(--spacing-sm)` | `var(--radius-sm)` | 语义错误 |
| AiCapabilitySection.vue | `border-radius: var(--spacing-xl)` | `var(--radius-xl)` | 语义错误 |

### 3. 使用不存在的 token

| 组件文件 | 不存在的 token | 应该使用 |
|---------|---------------|---------|
| ModuleCard.vue | `--spacing-xxl` | `--spacing-20` (80px) |
| AiCapabilitySection.vue | `--spacing-xxl` | `--spacing-20` (80px) |
| AiCapabilitySection.vue | `--spacing-xxxl` | `--spacing-16` (64px) 或 `--spacing-2xl` (建议新增) |
| AiProjectList.vue | `--spacing-xxl` | `--spacing-20` (80px) |

### 4. 使用未定义的 token

| 组件文件 | 未定义的 token | 建议操作 |
|---------|---------------|---------|
| ModuleCard.vue | `--radius` | 改为 `--radius-md` 或 `--radius-base` |
| ModuleCard.vue | `--card-radius` | 改为 `--radius-lg` 或定义新 token |
| AiProjectList.vue | `--card-radius` | 改为 `--radius-lg` 或定义新 token |

### 5. 滥用 calc() 拼接 token

| 组件文件 | 错误用法 | 建议 |
|---------|---------|------|
| AiCapabilitySection.vue | `font-size: calc(var(--spacing-xl) * 3)` | 定义 `--text-4xl` (96px) |
| AiCapabilitySection.vue | `font-size: calc(var(--spacing-xl) * 2.25)` | 定义 `--text-4xl` (96px) |
| AiCapabilitySection.vue | `font-size: calc(var(--spacing-xl) + var(--spacing-sm))` | 定义 `--text-3xl` (42px) |
| AiCapabilitySection.vue | `transform: translateY(calc(var(--spacing-xs) * -1))` | 定义 `--transform-sm-up` 或保留 |
| AiProjectList.vue | `padding: calc(var(--spacing-xxl) * 2)` | 定义新 token 或使用 `--spacing-32` |

---

## Token 体系补齐建议

### 1. 统一间距命名（二选一）

**方案A：完全使用数字命名（推荐）**

优点：与 Tailwind CSS 一致，更精确
缺点：不够直观

**方案B：扩展语义化命名（推荐用于公共组件）**

```css
:root {
  /* 扩展间距 token */
  --spacing-0: 0px;
  --spacing-px: 1px;
  --spacing-2xs: 2px;
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;
  --spacing-2xl: 48px;
  --spacing-3xl: 64px;
  --spacing-4xl: 96px;
  --spacing-5xl: 128px;
}
```

### 2. 扩展字号 token

已有 token 已经完整，无需补齐。

### 3. 扩展圆角 token

已有 token 已经完整，无需补齐。

### 4. 新增特定场景 token（可选）

```css
:root {
  /* 卡片专用 token */
  --card-radius: var(--radius-lg);
  --card-padding-sm: var(--spacing-md);
  --card-padding-md: var(--spacing-lg);
  --card-padding-lg: var(--spacing-xl);

  /* 按钮专用 token */
  --btn-radius-sm: var(--radius-sm);
  --btn-radius-md: var(--radius-md);
  --btn-radius-lg: var(--radius-lg);
  --btn-radius-full: var(--radius-full);

  /* 变换专用 token（用于动画） */
  --transform-sm-up: -4px;
  --transform-md-up: -8px;
  --transform-lg-up: -12px;
  --transform-sm-right: 4px;
  --transform-md-right: 8px;
}
```

---

## 阶段2整改中的主要问题总结

### 问题1：变量语义误用

**问题描述**：将 spacing 变量用于 font-size 和 border-radius

**影响**：
- 破坏了设计的语义化
- 后续维护困难
- 不符合设计系统最佳实践

**示例**：
```css
/* ❌ 错误 */
font-size: var(--spacing-lg);   /* 24px */
border-radius: var(--spacing-md);  /* 16px */

/* ✅ 正确 */
font-size: var(--text-2xl);    /* 24px */
border-radius: var(--radius-lg);    /* 16px */
```

### 问题2：使用不存在的 token

**问题描述**：使用了代码中没有定义的 token

**示例**：
```css
/* ❌ 错误 */
--spacing-xxl  /* 不存在 */
--spacing-xxxl /* 不存在 */

/* ✅ 正确 */
--spacing-20   /* 80px */
--spacing-16   /* 64px */
```

### 问题3：滥用 calc() 表达式

**问题描述**：使用 calc() 拼接 token 来创建新值

**影响**：
- 降低代码可读性
- 难以维护
- 逃避了 token 定义的责任

**示例**：
```css
/* ❌ 错误 */
font-size: calc(var(--spacing-xl) * 3);
transform: translateY(calc(var(--spacing-xs) * -1));

/* ✅ 正确 */
font-size: var(--text-4xl);
transform: translateY(var(--transform-sm-up));
```

### 问题4：硬编码值仍然存在

**问题描述**：部分组件仍使用 rem 或 px 硬编码值

**示例**：
```css
/* ❌ 错误 */
padding: 1rem;
font-size: 1.125rem;
border-radius: 0.5rem;

/* ✅ 正确 */
padding: var(--spacing-md);
font-size: var(--text-lg);
border-radius: var(--radius-md);
```

---

## 后续行动建议

### 立即行动（阶段2.5-2）

1. 修正 5 个公共组件中的变量误用
2. 替换不存在的 token 为正确的 token
3. 减少 calc() 使用，定义专用 token
4. 替换剩余的硬编码值为 token

### 短期行动（阶段2.5-3）

1. 更新验证脚本，识别变量语义误用
2. 将 spacing 代替字号和圆角标记为违规
3. 识别不存在的 token 引用

### 中期行动

1. 统一项目中的 token 命名规范
2. 统一使用 theme-variables.css 作为唯一 token 源
3. 定义组件级专用 token（如 --card-radius）

---

## 附录：Token 映射表

### 常用 px 值对应的 token

| px 值 | spacing token | text token | radius token |
|-------|--------------|------------|--------------|
| 4px | --spacing-1 / --spacing-xs | - | --radius-sm |
| 6px | --spacing-1_5 | - | --radius-base |
| 8px | --spacing-2 / --spacing-sm | - | - |
| 12px | --spacing-3 | --text-xs | - |
| 14px | - | --text-sm | - |
| 16px | --spacing-4 / --spacing-md | --text-base | - |
| 18px | - | --text-lg | - |
| 20px | --spacing-5 | --text-xl | - |
| 24px | --spacing-6 / --spacing-lg | --text-2xl | --radius-lg |
| 32px | --spacing-8 / --spacing-xl | --text-3xl | --radius-3xl |
| 48px | --spacing-12 | - | --radius-2xl |
| 64px | --spacing-16 | - | - |
| 80px | --spacing-20 | - | - |
| 96px | --spacing-24 | --text-8xl | - |

---

**报告版本**: 1.0
**生成工具**: Claude Code
**生成时间**: 2026-03-14
