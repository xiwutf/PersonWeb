# 样式系统治理落地收口 - 阶段2.5：验证器调整报告

## 概述

**报告生成时间**: 2026-03-14
**阶段**: 阶段2.5-3 - 更新验证逻辑规则
**目标**: 更新验证脚本，使其正确识别合规 token 使用、合理的 calc 场景和特定白名单场景

---

## 验证脚本

### 脚本路径

`node/scripts/validate-style-governance.js`

### 验证规则

脚本实现了以下四项硬性规则：

#### 规则1：禁止使用 spacing 变量代替字号

```javascript
const fontSizeSpacingRegex = /font-size:\s*var\((--spacing-[\w_-]+)\)/g
```

**检测**：
- 查找 `font-size: var(--spacing-xxx)` 模式

**建议**：
- 应使用 `--text-xs`, `--text-sm`, `--text-base` 等 text-* 系列变量

---

#### 规则2：禁止使用 spacing 变量代替圆角

```javascript
const radiusSpacingRegex = /border-radius:\s*var\((--spacing-[\w_-]+)\)/g
```

**检测**：
- 查找 `border-radius: var(--spacing-xxx)` 模式

**建议**：
- 应使用 `--radius-sm`, `--radius-md`, `--radius-lg` 等 radius-* 系列变量

---

#### 规则3：禁止用大量 calc() 代替缺失 token

```javascript
// 使用字符串查找避免复杂的正则表达式
const calcStarts = []
let pos = content.indexOf('calc(')
while (pos !== -1) {
  // 找到匹配的闭合括号
  let depth = 1
  let endPos = pos + 5
  while (endPos < content.length && depth > 0) {
    if (content[endPos] === '(') depth++
    else if (content[endPos] === ')') depth--
    endPos++
  }

  if (depth === 0) {
    const calcExpr = content.substring(pos, endPos)
    // 检查是否包含 var(--spacing-xx)
    if (calcExpr.includes('var(--spacing-')) {
      // 检查是否有简单的算术运算
      if (calcExpr.match(/var\(--spacing-[^)]+\)\s*[\+\-\*]\s*(?:var\(--spacing-[^)]+\)|\d+px)/)) {
        violations.push({
          type: VIOLATION_TYPES.UNNECESSARY_CALC,
          expression: calcExpr,
          line: getLineNumber(content, pos),
          suggestion: '不必要的 calc() 表达式，应定义专用 token 或使用直接值'
        })
      }
    }
  }

  pos = content.indexOf('calc(', endPos)
}
```

**检测**：
- 查找包含 `var(--spacing-xxx)` 且包含简单算术运算（+、-、*）的 calc() 表达式

**建议**：
- 不必要的 calc() 表达式，应定义专用 token 或使用直接值

---

#### 规则4：替换必须保证数值等价

```javascript
const hardcodedPatterns = [
  { regex: /(?:padding|margin|gap):\s*\d+(?:\.\d+)?px(?![^}]*var\()/g, prop: 'spacing' },
  { regex: /border-radius:\s*(?!9999px|8px|4px)\d+(?:\.\d+)?px(?![^}]*var\()/g, prop: 'radius' },
  { regex: /font-size:\s*\d+px(?![^}]*var\()/g, prop: 'fontSize' },
  { regex: /font-size:\s*(\d+(?:\.\d+)?rem)(?![^}]*var\()/g, prop: 'fontSize', isRem: true }
]
```

**检测**：
- padding、margin、gap 的硬编码 px 值
- border-radius 的硬编码 px 值（豁免 9999px、8px、4px）
- font-size 的硬编码 px 值
- font-size 的硬编码 rem 值

**豁免**：
- 特定布局约束（如 `max-width`, `min-width`）
- 完全圆形的 `border-radius: 9999px`
- 小圆角 `border-radius: 8px`、`border-radius: 4px`

**建议**：
- 应使用对应的 CSS 变量

---

## 有效 Token 定义

### Spacing Token

```javascript
spacing: [
  // tokens.css 中的语义化命名
  '--spacing-xs', '--spacing-sm', '--spacing-md', '--spacing-lg', '--spacing-xl',
  // theme-variables.css 中的数字命名
  '--spacing-0', '--spacing-px', '--spacing-0_5',
  '--spacing-1', '--spacing-1_5', '--spacing-2', '--spacing-2_5',
  '--spacing-3', '--spacing-3_5', '--spacing-4', '--spacing-5',
  '--spacing-6', '--spacing-7', '--spacing-8', '--spacing-9',
  '--spacing-10', '--spacing-11', '--spacing-12', '--spacing-14',
  '--spacing-16', '--spacing-20', '--spacing-24', '--spacing-28',
  '--spacing-32', '--spacing-36', '--spacing-40', '--spacing-44',
  '--spacing-48', '--spacing-56', '--spacing-64'
]
```

### Text Token

```javascript
text: [
  // 字号 token（font-size 属性应使用这些）
  '--text-xs', '--text-sm', '--text-base', '--text-lg',
  '--text-xl', '--text-2xl', '--text-3xl', '--text-4xl',
  '--text-5xl', '--text-6xl', '--text-7xl', '--text-8xl', '--text-9xl',
  // 兼容旧命名
  '--font-size-base', '--font-size-h1', '--font-size-h2',
  '--font-size-h3', '--font-size-h4'
]
```

### Radius Token

```javascript
radius: [
  '--radius-none', '--radius-xs', '--radius-sm', '--radius-base', '--radius-md',
  '--radius-lg', '--radius-xl', '--radius-2xl', '--radius-3xl',
  '--radius-full'
]
```

### 其他 Token

```javascript
// 颜色 token（用于检测不存在的 token）
color: [
  '--color-text-main', '--color-text-primary', '--color-text-secondary',
  '--color-text-tertiary', '--color-text-quaternary',
  '--color-text-muted', '--color-text-sub', '--color-text-disabled',
  '--color-text-on-primary', '--color-text-success', '--color-text-warning',
  '--color-text-error', '--color-text-info'
],

// 背景 token
bg: [
  '--color-bg-body', '--color-bg-page', '--color-bg-page-alt',
  '--color-bg-light', '--color-bg-dark', '--color-bg-elevated',
  '--color-bg-overlay', '--color-bg-card', '--color-bg-card-elevated',
  '--color-bg-card-muted', '--color-bg-surface', '--color-bg-surface-elevated',
  '--color-bg-surface-muted', '--color-bg-input', '--color-bg-input-disabled',
  '--color-bg-input-hover', '--color-bg-disabled'
],

// 边框 token
border: [
  '--color-border-subtle', '--color-border-default', '--color-border-strong',
  '--color-border-focus', '--color-border-muted', '--color-border-hover',
  '--color-border-active'
]
```

---

## Token 检查逻辑

```javascript
// 跳过以 --color 开头的 token（这些是颜色 token，不在此检查范围）
if (token.startsWith('--color-')) {
  continue
}

// 跳过以 --shadow 开头的 token
if (token.startsWith('--shadow')) {
  continue
}

// 检查是否为 spacing token
if (token.startsWith('--spacing-')) {
  if (!VALID_TOKENS.spacing.includes(token)) {
    violations.push(...)
  }
}
// 检查是否为 text token（字号，如 --text-xs）
else if (token.startsWith('--text-') && !token.includes('-text-') && !token.includes('-bg-') && !token.includes('-border-')) {
  if (!VALID_TOKENS.text.includes(token)) {
    violations.push(...)
  }
}
// 检查是否为 radius token
else if (token.startsWith('--radius-')) {
  if (!VALID_TOKENS.radius.includes(token)) {
    violations.push(...)
  }
}
```

---

## 违规类型

```javascript
const VIOLATION_TYPES = {
  SPACING_FOR_FONT_SIZE: 'spacing-for-font-size',      // 使用 spacing 代替字号
  SPACING_FOR_RADIUS: 'spacing-for-radius',            // 使用 spacing 代替圆角
  INVALID_TOKEN: 'invalid-token',                      // 不存在的 token
  UNNECESSARY_CALC: 'unnecessary-calc',              // 不必要的 calc()
  HARDCODED_VALUE: 'hardcoded-value'                  // 硬编码值
}
```

---

## 使用方法

### 扫描公共层

```bash
node node/scripts/validate-style-governance.js scan-public-layer
```

### 扫描所有组件

```bash
node node/scripts/validate-style-governance.js scan-components
```

### 扫描所有页面

```bash
node node/scripts/validate-style-governance.js scan-pages
```

### 扫描所有目录

```bash
node node/scripts/validate-style-governance.js scan-all
```

---

## 输出说明

### 退出码

- `0`：完全合规（无硬编码）
- `1`：存在需要改进的问题

### 输出格式

```
=== 样式系统治理验证结果（扫描类型）===

总文件数: X
完全合规: X (XX.XX%)
需要改进: X (XX.XX%)
扫描错误: X

--- 违规类型统计 ---
  [类型]: X 处

--- 需要改进的文件 ---
  [文件] (X 个问题)
    Line X: [类型] 建议
      -> 详细信息

--- 完全合规的文件 ---
  ✓ [文件]
```

---

## 与阶段2验证脚本的主要差异

| 特性 | 阶段2 脚本 | 阶段2.5 脚本 |
|------|-------------|---------------|
| 检查 spacing 代替字号 | ❌ 否 | ✅ 是 |
| 检查 spacing 代替圆角 | ❌ 否 | ✅ 是 |
| 检查不存在的 token | ❌ 否 | ✅ 是 |
| 检查不必要的 calc() | ❌ 否 | ✅ 是 |
| 豁免特定布局约束 | ⚠️ 部分 | ✅ 完整 |
| 豁免特定值（如 9999px）| ❌ 否 | ✅ 是 |
| 详细的违规说明 | ⚠️ 简单 | ✅ 详细 |

---

## 验收标准检查

| 验收标准 | 状态 | 说明 |
|---------|------|------|
| 明确什么算合规 | ✅ | 使用正确的 token（text-*, radius-*, spacing-*） |
| 明确什么算例外 | ✅ | 特定布局约束、完全圆形、特定值 |
| 明确什么仍然违规 | ✅ | 硬编码值、语义误用、不存在的 token、不必要的 calc |

---

**报告版本**: 1.0
**生成工具**: Claude Code
**生成时间**: 2026-03-14
