# Design System — 溪午听风 / Ming Studio

**日期：** 2026-05-06  
**状态：** 已批准，待实施  
**范围：** 前台公开页面视觉系统治理（Admin 完全隔离）

---

## 1. 背景与目标

### 问题

当前项目存在三套互相竞争的 Token 体系：

| 文件 | 定位 | 问题 |
|---|---|---|
| `assets/styles/tokens.css` | V2 Token | 内容极少，只有 3D / spacing / radius / font-family |
| `assets/styles/theme-variables.css` | V3 极光设计系统 | 完整但偏浅色，命名不统一 |
| `assets/css/design-system.css` | Aurora Design System V3 | 与前者竞争，有独立 light/dark 块 |

后果：
- 同一变量名（如 `--radius-sm`）在不同文件中值不同（4px vs 6px）
- 公开页面默认是浅色（`--color-bg-body: var(--color-gray-50)`）
- 没有深色优先的品牌渐变 token
- 页面间风格割裂，不像同一个设计系统

### 目标

建立单一深色 Studio 设计系统，应用于前台所有公开页面，Admin 完全不受影响。

**视觉定位：** 深色科技风，AI Product Studio，Linear / Vercel / OpenAI 风格。  
**关键词：** 深色、玻璃拟态、微发光、轻边框、大留白、低信息密度、强层级。

---

## 2. 决策记录

| 决策 | 选项 | 选择 | 理由 |
|---|---|---|---|
| 实施方案 | A 增量 / B 全合并 / C 新主题槽 | **B 全合并** | 彻底清除技术债，长期维护成本低 |
| 主题数量 | 保留9套 / 精简2套 / 单套 | **单套深色** | 统一品牌，主题切换按钮保留占位供后期扩展 |
| Admin 处理 | 同步改造 / 完全隔离 | **完全隔离** | Admin 使用现成 UI 库，不自研视觉系统 |

---

## 3. 视觉 Token 体系

### 3.1 文件结构

**唯一 Token 源头：** `assets/styles/tokens.css`

```
assets/styles/
  tokens.css          ← 唯一 Token 源头（重写）
  base.css            ← HTML/body reset（更新引用）
  components.css      ← .studio-* / .site-* 工具类（新建）
  ui-patch-naive.css  ← Naive UI 补丁（保留不动）
  glassmorphism.css   ← 玻璃效果（保留，更新 token 引用）
  index.css           ← 入口（更新 import 列表）

assets/css/
  header.css          ← 更新为胶囊导航样式
  footer.css          ← 更新为极简品牌 Footer
  hero.css            ← 更新 token 引用
  home.css            ← 更新 token 引用
  [admin-*.css]       ← 完全不动
  [dashboard.css]     ← 完全不动
```

**待删除文件**（删除前必须先全局搜索确认无引用）：

| 文件 | 删除原因 |
|---|---|
| `assets/styles/theme-variables.css` | 内容迁移进 tokens.css |
| `assets/css/design-system.css` | Aurora DS 与新体系冲突 |
| `assets/styles/theme.css` | 旧兼容文件 |
| `assets/css/themes.css` | 多主题切换，单主题后废弃 |

> **规则：** 删除前必须执行全局 grep，确认无页面、组件、layout、nuxt.config 依赖。有依赖先迁移再删除。

### 3.2 Token 定义

#### Layer 1 — 背景与表面

```css
:root {
  --color-bg:               #070b14;
  --color-bg-soft:          #0b1220;
  --color-surface:          rgba(15, 23, 42, 0.72);
  --color-surface-strong:   rgba(15, 23, 42, 0.92);
  --color-surface-elevated: rgba(20, 30, 55, 0.85);
}
```

#### Layer 2 — 文字

```css
:root {
  --color-text:             #f8fafc;
  --color-text-muted:       #94a3b8;
  --color-text-subtle:      #64748b;
  --color-text-on-primary:  #ffffff;
}
```

#### Layer 3 — 边框

```css
:root {
  --color-border:           rgba(148, 163, 184, 0.16);
  --color-border-strong:    rgba(148, 163, 184, 0.28);
  --color-border-focus:     rgba(59, 130, 246, 0.6);
}
```

#### Layer 4 — 品牌色 & 渐变

```css
:root {
  --color-primary:          #3b82f6;
  --color-primary-hover:    #60a5fa;
  --color-cyan:             #22d3ee;
  --color-violet:           #8b5cf6;

  --gradient-primary: linear-gradient(135deg, #3b82f6 0%, #22d3ee 100%);
  --gradient-hero:
    radial-gradient(circle at top right, rgba(59,130,246,.22), transparent 34%),
    radial-gradient(circle at bottom left, rgba(139,92,246,.16), transparent 38%),
    #070b14;
}
```

#### Layer 5 — 状态色

```css
:root {
  --color-success:  #22c55e;
  --color-warning:  #f59e0b;
  --color-danger:   #ef4444;
  --color-info:     #38bdf8;
}
```

#### Layer 6 — 尺寸系统

```css
:root {
  /* 圆角 */
  --radius-sm:   10px;
  --radius-md:   16px;
  --radius-lg:   24px;
  --radius-xl:   32px;
  --radius-pill: 999px;

  /* 间距 */
  --space-section:    112px;
  --space-section-sm: 72px;
  --space-container:  1200px;
  --space-card:       28px;
  --space-card-sm:    20px;

  /* 阴影 */
  --shadow-card:      0 0 0 1px var(--color-border), 0 8px 32px rgba(0,0,0,.4);
  --shadow-glow:      0 0 24px rgba(59,130,246,.25);
  --shadow-glow-cyan: 0 0 24px rgba(34,211,238,.2);
}
```

#### Layer 7 — Z-Index

```css
:root {
  --z-header:    50;
  --z-dropdown:  60;
  --z-modal:     80;
  --z-toast:     90;
}
```

#### Layer 8 — Legacy 兼容（文件末尾，仅供旧代码过渡期引用）

```css
/* legacy compatibility only — do not use in new code */
:root {
  --legacy-bg-body:        var(--color-bg);
  --legacy-bg-card:        var(--color-surface);
  --legacy-text-main:      var(--color-text);
  --legacy-text-muted:     var(--color-text-muted);
  --legacy-border:         var(--color-border);
  --legacy-border-subtle:  var(--color-border);
  --legacy-color-primary:  var(--color-primary);
}
```

---

## 4. 组件工具类体系

写入 `assets/styles/components.css`。所有 class 使用 `studio-*` 或 `site-*` 前缀，避免全局污染。

### 命名规范

| Class | 用途 |
|---|---|
| `.site-bg` | 全页深色背景 + radial glow |
| `.site-section` | 区块上下间距（112px） |
| `.site-section-sm` | 小区块间距（72px） |
| `.site-container` | max-width 1200px 居中容器 |
| `.studio-card` | 标准卡片（玻璃 + 边框 + hover） |
| `.studio-panel` | 大容器（Hero / CTA / AI展示区） |
| `.studio-grid` | 响应式卡片网格 |
| `.studio-tag` | 标签/状态徽章 |
| `.btn-primary` | 渐变主按钮（pill） |
| `.btn-secondary` | 透明次按钮（pill） |
| `.btn-text` | 文字箭头按钮 |
| `.text-hero` | 首屏主标题排版 |
| `.text-section` | 区块标题排版 |
| `.text-card` | 卡片标题排版 |
| `.text-body` | 正文排版 |
| `.text-label` | 小字/标签排版 |

### 关键实现

```css
/* 背景 */
.site-bg {
  background:
    radial-gradient(circle at 20% 10%, rgba(59,130,246,.18), transparent 30%),
    radial-gradient(circle at 80% 20%, rgba(34,211,238,.12), transparent 28%),
    var(--color-bg);
  min-height: 100vh;
}

/* 标准卡片 */
.studio-card {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  backdrop-filter: blur(18px);
  padding: var(--space-card);
  transition: transform 220ms ease, border-color 220ms ease, box-shadow 220ms ease;
}
.studio-card:hover {
  transform: translateY(-4px);
  border-color: var(--color-border-strong);
  box-shadow: var(--shadow-glow);
}

/* 大容器 */
.studio-panel {
  background: var(--color-surface-strong);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-xl);
  backdrop-filter: blur(22px);
}

/* 主按钮 */
.btn-primary {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  background: var(--gradient-primary);
  color: var(--color-text-on-primary);
  border-radius: var(--radius-pill);
  height: 46px;
  padding: 0 22px;
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: opacity 200ms ease, transform 200ms ease;
}
.btn-primary:hover { opacity: .88; transform: translateY(-1px); }

/* 次按钮 */
.btn-secondary {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  background: transparent;
  border: 1px solid var(--color-border-strong);
  color: var(--color-text);
  border-radius: var(--radius-pill);
  height: 46px;
  padding: 0 22px;
  font-weight: 500;
  cursor: pointer;
  transition: background 200ms ease;
}
.btn-secondary:hover { background: var(--color-surface); }

/* 文字按钮 */
.btn-text {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  color: var(--color-primary);
  background: none;
  border: none;
  font-weight: 500;
  cursor: pointer;
  transition: gap 200ms ease;
}
.btn-text:hover { gap: 8px; }
```

---

## 5. 导航系统

**组件：** `components/layout/AppHeader.vue`（或对应文件）  
**样式：** `assets/css/header.css`

```
- position: fixed
- top: 16px，居中，max-width: 1180px
- height: 64px
- 背景：rgba(8,12,24,.72) + backdrop-filter: blur(20px)
- border: 1px solid var(--color-border)
- border-radius: var(--radius-pill)
- z-index: var(--z-header)
```

**导航项结构：**  
左：Logo + 站名  
中：首页 / 产品 / 项目 / AI实验室 / 文章 / 关于  
右：搜索 / 主题切换（占位，后期扩展）/ 联系

**状态：**
- 默认：透明背景
- Hover：轻背景（`var(--color-surface)`）
- Active：蓝色胶囊（`var(--color-primary)`）

---

## 6. Footer 系统

**组件：** `components/layout/AppFooter.vue`（或对应文件）  
**样式：** `assets/css/footer.css`

四列结构：

| 左 | 中左 | 中右 | 右 |
|---|---|---|---|
| Logo + 站名 + 定位语 + 简介 | 产品链接 | 内容链接 | 社交链接 |

底部：© 2026 溪午听风 · ICP备案

**禁止：** 大量栏目、复杂站点地图。

---

## 7. 字体层级规范

| 层级 | Class | 规格 |
|---|---|---|
| 首屏主标题 | `.text-hero` | `clamp(48px,6vw,88px)`, weight 800, line-height 0.95, letter-spacing -0.04em |
| 区块标题 | `.text-section` | `clamp(28px,3vw,40px)`, weight 700, line-height 1.1, letter-spacing -0.02em |
| 卡片标题 | `.text-card` | `clamp(17px,1.4vw,22px)`, weight 700 |
| 正文 | `.text-body` | 15px, line-height 1.8, color `--color-text-muted` |
| 小字/标签 | `.text-label` | 12px, letter-spacing 0.08em, color `--color-text-subtle`, uppercase |

**规则：** 层级之间必须有明显视觉差异，禁止所有文字大小接近。

---

## 8. 响应式规则

| 断点 | 布局 |
|---|---|
| 1440px+ | 容器 max-width 1200px，两栏，卡片 grid |
| 768px | 两栏 → 一栏，导航压缩 |
| 375px | 单列，Hero 单列，卡片一列，标题缩小，footer 单列 |

**移动端禁止横向滚动。**  
导航在移动端：Logo + 汉堡菜单。

---

## 9. 页面覆盖范围

### 涉及页面

| 页面 | 路径 | 备注 |
|---|---|---|
| 首页 | `pages/index.vue` | 优先级最高 |
| 产品页 | `pages/products/**` + `components/products/**` | 全覆盖，避免断层 |
| 项目页 | `pages/projects/**` | 统一现有路由，不新增 |
| AI实验室 | `pages/lab.vue` | |
| 文章 | `pages/blog/**` | 列表页 + 详情页 |
| 关于 | `pages/about.vue` | |
| 全局 Layout | `layouts/default.vue`, `layouts/home.vue`, `components/layout/**` | Header/Footer/背景 |

### 完全不动

- `pages/admin/**`
- `layouts/admin.vue`, `layouts/admin-content-only.vue`
- `components/admin/**`
- `assets/css/admin-*.css`, `dashboard.css`

---

## 10. 实施顺序

> 每步完成后必须 `npm run build` 验收，通过后再进入下一步。

### Step 1 — Token 基础层

1. 重写 `assets/styles/tokens.css`（新 Token + legacy 块）
2. 新建 `assets/styles/components.css`（所有工具类）
3. 全局搜索旧 CSS 文件引用，确认无依赖后删除
4. 更新 `assets/styles/index.css` 和 `nuxt.config.ts` CSS 导入
5. **验收：** build 通过，Admin 页面视觉无变化

### Step 2 — Header / Footer / Layout

1. 更新 `assets/css/header.css`（胶囊导航）
2. 更新 `assets/css/footer.css`（极简品牌 Footer）
3. 更新 `layouts/default.vue` 和 `layouts/home.vue`（接入 `.site-bg`）
4. **验收：** 所有公开页 Header/Footer 视觉一致，Admin 不受影响

### Step 3 — 首页

1. 更新 `pages/index.vue`、`assets/css/hero.css`、`assets/css/home.css`
2. 替换硬编码颜色 → 新 token
3. 替换旧 class → 新前缀 class
4. **验收：** 首页三档宽度（375 / 768 / 1440px）无溢出，build 通过

### Step 4 — 其他公开页（逐步）

按页面逐个迁移：产品页 → 项目页 → AI实验室 → 文章 → 关于  
每个页面完成后单独 build 验收。

---

## 11. 验收 Checklist

| 项目 | 标准 |
|---|---|
| 修改文件清单 | 每个 Step 结束后输出 |
| 新增 Token | tokens.css 内注释索引完整 |
| 页面覆盖 | 6 个公开页 + 全局 layout |
| 硬编码颜色 | 公开页内不出现裸 `#xxx` / `rgba()` 直接值 |
| PC | 1440px 正常，无布局崩溃 |
| Mobile | 375 / 768px 无横向滚动、无遮挡、无卡片溢出 |
| Admin 隔离 | Admin 页面视觉前后一致 |
| Build | `npm run build` 0 error |
