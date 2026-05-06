# Ming Studio Design System Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 建立单一深色 Studio 视觉体系，替换三套互相冲突的 Token 系统，并统一应用到所有前台公开页面，Admin 完全隔离。

**Architecture:** 新 `assets/styles/tokens.css` 作为唯一 Token 源头，通过 import 顺序（后者覆盖前者）确保新 Token 优先于旧变量。新 `assets/styles/components.css` 提供 `.studio-*` / `.site-*` 命名空间的工具类。旧 CSS 文件保留至所有依赖迁移完成后再删除。

**Tech Stack:** Nuxt 3, Vue 3, TypeScript, Tailwind CSS, Naive UI, CSS Custom Properties

**Spec:** `docs/superpowers/specs/2026-05-06-design-system-design.md`

---

## 文件结构

**新建：**
- `assets/styles/tokens.css` — 完全重写，单一 Token 源头
- `assets/styles/components.css` — 新建，`.studio-*` / `.site-*` 工具类

**修改（不删除）：**
- `assets/styles/index.css` — 调整 import 顺序，tokens.css 最后加载
- `assets/styles/base.css` — 颜色引用改为新 token 名
- `assets/css/header.css` — 改写为胶囊导航样式
- `assets/css/footer.css` — 改写为极简品牌 Footer
- `assets/css/hero.css` — 更新 token 引用
- `assets/css/home.css` — 更新 token 引用
- `nuxt.config.ts` — CSS 导入移除 `themes.css`
- `components/layout/Header.vue` — 导航项重构（移除更多，扁平6项）
- `components/layout/Footer.vue` — 极简品牌结构
- `layouts/default.vue` — 接入新 token，移除旧颜色类
- `layouts/home.vue` — 接入新 token
- `pages/index.vue` — token cleanup
- `pages/products/**` — token cleanup
- `pages/projects/**` — token cleanup
- `pages/lab.vue` — token cleanup
- `pages/blog/**` — token cleanup
- `pages/about.vue` — token cleanup

**延迟删除（grep 确认无引用后）：**
- `assets/styles/theme-variables.css`
- `assets/css/design-system.css`
- `assets/styles/theme.css`
- `assets/css/themes.css`

**绝对不动（Admin 隔离）：**
- `pages/admin/**`, `layouts/admin.vue`, `layouts/admin-content-only.vue`
- `components/admin/**`, `assets/css/admin-*.css`, `assets/css/dashboard.css`

---

## STEP 1 — Token 基础层

### Task 1: 重写 assets/styles/tokens.css

**Files:**
- Modify: `assets/styles/tokens.css`

- [ ] **Step 1: 替换 tokens.css 全部内容**

```css
/* ============================================================
   Ming Studio Design System — Token Source of Truth v3
   All public-site values live here. No overrides elsewhere.
   Admin system uses its own variables (see legacy block below).
   ============================================================ */

/* ── Layer 1: Background & Surfaces ── */
:root {
  --color-bg:               #070b14;
  --color-bg-soft:          #0b1220;
  --color-surface:          rgba(15, 23, 42, 0.72);
  --color-surface-strong:   rgba(15, 23, 42, 0.92);
  --color-surface-elevated: rgba(20, 30, 55, 0.85);
}

/* ── Layer 2: Typography ── */
:root {
  --color-text:             #f8fafc;
  --color-text-muted:       #94a3b8;
  --color-text-subtle:      #64748b;
  --color-text-on-primary:  #ffffff;
}

/* ── Layer 3: Borders ── */
:root {
  --color-border:        rgba(148, 163, 184, 0.16);
  --color-border-strong: rgba(148, 163, 184, 0.28);
  --color-border-focus:  rgba(59, 130, 246, 0.6);
}

/* ── Layer 4: Brand & Gradients ── */
:root {
  --color-primary:       #3b82f6;
  --color-primary-hover: #60a5fa;
  --color-cyan:          #22d3ee;
  --color-violet:        #8b5cf6;

  --gradient-primary: linear-gradient(135deg, #3b82f6 0%, #22d3ee 100%);
  --gradient-hero:
    radial-gradient(circle at top right, rgba(59,130,246,.22), transparent 34%),
    radial-gradient(circle at bottom left, rgba(139,92,246,.16), transparent 38%),
    #070b14;
}

/* ── Layer 5: Status Colors ── */
:root {
  --color-success: #22c55e;
  --color-warning: #f59e0b;
  --color-danger:  #ef4444;
  --color-info:    #38bdf8;
}

/* ── Layer 6: Dimensions ── */
:root {
  --radius-sm:   10px;
  --radius-md:   16px;
  --radius-lg:   24px;
  --radius-xl:   32px;
  --radius-pill: 999px;

  --space-section:    112px;
  --space-section-sm: 72px;
  --space-container:  1200px;
  --space-card:       28px;
  --space-card-sm:    20px;

  --shadow-card:      0 0 0 1px var(--color-border), 0 8px 32px rgba(0,0,0,.4);
  --shadow-glow:      0 0 24px rgba(59,130,246,.25);
  --shadow-glow-cyan: 0 0 24px rgba(34,211,238,.2);

  --font-size-base:    1rem;
  --font-weight-base:  400;
  --line-height-base:  1.6;
  --font-family-base:  'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI",
                       "Microsoft YaHei", "PingFang SC", sans-serif;
  --font-family-mono:  'SF Mono', Monaco, 'Cascadia Code', Consolas, monospace;
}

/* ── Layer 7: Z-Index ── */
:root {
  --z-header:   50;
  --z-dropdown: 60;
  --z-modal:    80;
  --z-toast:    90;
}

/* ── Legacy Compatibility ────────────────────────────────────
   Maps old variable names → new values.
   Used by: Tailwind config, base.css, hero.css, home.css
   during migration. Remove a line only after all references
   to that old name are gone from public pages.
   DO NOT use these names in new code.
   ──────────────────────────────────────────────────────────── */
:root {
  /* Backgrounds */
  --color-bg-body:         var(--color-bg);
  --bg-app:                var(--color-bg);
  --color-bg-elevated:     var(--color-surface-elevated);
  --color-bg-light:        var(--color-bg-soft);
  --bg-surface-1:          var(--color-surface);
  --bg-surface-2:          var(--color-surface-strong);
  --color-bg-card:         var(--color-surface);
  --color-bg-page:         var(--color-bg);

  /* Text */
  --color-text-main:       var(--color-text);
  --text-primary:          var(--color-text);
  --text-secondary:        var(--color-text-muted);
  --text-tertiary:         var(--color-text-subtle);
  --color-text-sec:        var(--color-text-muted);
  --color-text-sub:        var(--color-text-muted);
  --color-text-disabled:   var(--color-text-subtle);

  /* Borders */
  --color-border-subtle:   var(--color-border);
  --color-border-default:  var(--color-border-strong);
  --color-border-muted:    var(--color-border);
  --border-subtle:         var(--color-border);
  --border-focus:          var(--color-border-focus);

  /* Brand */
  --primary:               var(--color-primary);
  --secondary:             var(--color-violet);
  --accent:                var(--color-cyan);
  --color-primary-soft:    rgba(59, 130, 246, 0.12);
  --color-primary-10:      rgba(59, 130, 246, 0.1);
  --color-primary-20:      rgba(59, 130, 246, 0.2);

  /* Status */
  --success:               var(--color-success);
  --warning:               var(--color-warning);
  --error:                 var(--color-danger);

  /* Shadows */
  --shadow-sm:             0 1px 2px rgba(0,0,0,.3);
  --shadow-md:             0 4px 6px -1px rgba(0,0,0,.3);
  --shadow-lg:             0 10px 15px -3px rgba(0,0,0,.3);
  --shadow-glow:           0 0 24px rgba(59,130,246,.25);
}
```

- [ ] **Step 2: 确认 tokens.css 可被正确解析**

```powershell
# 只检查语法错误，不需要 build
Select-String -Path "assets\styles\tokens.css" -Pattern "var\(--[^,)]+$" | Head 5
```
预期：无输出（无未闭合的 var()）

---

### Task 2: 新建 assets/styles/components.css

**Files:**
- Create: `assets/styles/components.css`

- [ ] **Step 1: 创建文件，写入完整工具类**

```css
/* ============================================================
   Ming Studio — Public Site Component Utilities
   Namespace: .studio-* and .site-*
   DO NOT use generic names like .card / .section / .container
   DO NOT import this file into admin layouts
   ============================================================ */

/* ── Page Background ── */
.site-bg {
  background:
    radial-gradient(circle at 20% 10%, rgba(59,130,246,.18), transparent 30%),
    radial-gradient(circle at 80% 20%, rgba(34,211,238,.12), transparent 28%),
    var(--color-bg);
  min-height: 100vh;
}

/* ── Section Spacing ── */
.site-section {
  padding-top: var(--space-section);
  padding-bottom: var(--space-section);
  padding-left: clamp(16px, 4vw, 48px);
  padding-right: clamp(16px, 4vw, 48px);
}

.site-section-sm {
  padding-top: var(--space-section-sm);
  padding-bottom: var(--space-section-sm);
  padding-left: clamp(16px, 4vw, 48px);
  padding-right: clamp(16px, 4vw, 48px);
}

@media (max-width: 768px) {
  .site-section    { padding-top: 64px; padding-bottom: 64px; }
  .site-section-sm { padding-top: 48px; padding-bottom: 48px; }
}

/* ── Container ── */
.site-container {
  max-width: var(--space-container);
  margin-left: auto;
  margin-right: auto;
  padding-left: clamp(16px, 4vw, 48px);
  padding-right: clamp(16px, 4vw, 48px);
  width: 100%;
}

/* ── Card ── */
.studio-card {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  backdrop-filter: blur(18px);
  -webkit-backdrop-filter: blur(18px);
  padding: var(--space-card);
  transition: transform 220ms ease, border-color 220ms ease, box-shadow 220ms ease;
}

.studio-card:hover {
  transform: translateY(-4px);
  border-color: var(--color-border-strong);
  box-shadow: var(--shadow-glow);
}

/* ── Panel (Hero / CTA / AI showcase) ── */
.studio-panel {
  background: var(--color-surface-strong);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-xl);
  backdrop-filter: blur(22px);
  -webkit-backdrop-filter: blur(22px);
  padding: var(--space-card);
}

/* ── Grid ── */
.studio-grid {
  display: grid;
  gap: 24px;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
}

@media (max-width: 640px) {
  .studio-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
}

/* ── Tag / Badge ── */
.studio-tag {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 3px 10px;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-pill);
  font-size: 12px;
  font-weight: 500;
  letter-spacing: 0.04em;
  color: var(--color-text-muted);
  background: var(--color-surface);
}

/* ── Buttons ── */
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
  font-size: 15px;
  border: none;
  cursor: pointer;
  transition: opacity 200ms ease, transform 200ms ease;
  white-space: nowrap;
}

.btn-primary:hover {
  opacity: 0.88;
  transform: translateY(-1px);
}

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
  font-size: 15px;
  cursor: pointer;
  transition: background 200ms ease, border-color 200ms ease;
  white-space: nowrap;
}

.btn-secondary:hover {
  background: var(--color-surface);
  border-color: var(--color-border-strong);
}

.btn-text {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  color: var(--color-primary);
  background: none;
  border: none;
  font-weight: 500;
  font-size: 15px;
  cursor: pointer;
  transition: gap 200ms ease;
  padding: 0;
}

.btn-text:hover {
  gap: 8px;
}

/* ── Typography Utilities ── */
.text-hero {
  font-size: clamp(48px, 6vw, 88px);
  font-weight: 800;
  line-height: 0.95;
  letter-spacing: -0.04em;
  color: var(--color-text);
}

.text-section {
  font-size: clamp(28px, 3vw, 40px);
  font-weight: 700;
  line-height: 1.1;
  letter-spacing: -0.02em;
  color: var(--color-text);
}

.text-card-title {
  font-size: clamp(17px, 1.4vw, 22px);
  font-weight: 700;
  color: var(--color-text);
}

.text-body {
  font-size: 15px;
  line-height: 1.8;
  color: var(--color-text-muted);
}

.text-label {
  font-size: 12px;
  letter-spacing: 0.08em;
  color: var(--color-text-subtle);
  text-transform: uppercase;
}
```

- [ ] **Step 2: 确认文件已创建**

```powershell
Test-Path "assets\styles\components.css"
```
预期：`True`

---

### Task 3: 更新 assets/styles/index.css

**Files:**
- Modify: `assets/styles/index.css`

- [ ] **Step 1: 将 tokens.css 和 components.css 作为最后两个 import**

将文件内容替换为：

```css
/* ============================================================
   Style System Entry Point
   Import order matters: tokens.css last = highest priority.
   ============================================================ */

/* Legacy system — kept for admin compat, will be removed
   after all public pages are migrated */
@import './theme-variables.css';
@import './component-styles.css';

/* Core resets */
@import './base.css';
@import './ui-patch-naive.css';
@import './glassmorphism.css';
@import './theme.css';

/* NEW Ming Studio Design System
   These imports MUST remain last so new tokens override
   conflicting legacy values above. */
@import './tokens.css';
@import './components.css';
```

---

### Task 4: 移除 themes.css（nuxt.config.ts）

**Files:**
- Modify: `nuxt.config.ts`

- [ ] **Step 1: 确认 themes.css 的内容范围**

```powershell
Get-Content "assets\css\themes.css" | Select-Object -First 30
```
预期：仅包含多主题 CSS 变量覆盖块（data-theme='light/dark/lab' 等）。

- [ ] **Step 2: 从 nuxt.config.ts 的 css 数组中移除 themes.css 一行**

定位 `nuxt.config.ts:88-98`，在 css 数组中删除这一行：
```
'~/assets/css/themes.css',
```

更新后的 css 数组：
```ts
css: [
  '~/assets/styles/index.css',
  '~/assets/css/main.css',
  '~/assets/css/header.css',
  '~/assets/css/footer.css',
  '~/assets/css/hero.css',
  '~/assets/css/home.css',
  '~/assets/css/visitor-interaction.css',
  '~/assets/css/charts.css'
],
```

---

### Task 5: Step 1 构建验收

**Files:** 无新文件

- [ ] **Step 1: 运行构建**

```powershell
npm run build
```
预期：0 errors。允许出现 Sass deprecation warnings（已知，用 `SASS_SILENCE_DEPRECATIONS=legacy-js-api` 抑制，已在 npm script 中配置）。

- [ ] **Step 2: 确认旧 CSS 文件仍被正确加载（Tailwind vars 不丢失）**

```powershell
# 检查 design-system.css 是否被任何文件显式 @import
Select-String -Path "assets" -Pattern "design-system.css" -Recurse
Select-String -Path "nuxt.config.ts" -Pattern "design-system"
```
预期：若有 import，记录下来，后续迁移时处理。

- [ ] **Step 3: 提交 Step 1**

```powershell
git add assets/styles/tokens.css assets/styles/components.css assets/styles/index.css nuxt.config.ts
git commit -m "feat(ds): Step 1 — Studio token layer + component utilities"
```

---

## STEP 2 — Header / Footer / Layout

### Task 6: 重写 assets/css/header.css（胶囊导航）

**Files:**
- Modify: `assets/css/header.css`

- [ ] **Step 1: 替换 header.css 全部内容**

```css
/* Header — Ming Studio pill navigation */

/* ── Floating Container ── */
.header-container {
  position: fixed !important;
  top: calc(var(--safe-area-top, 0px) + 16px) !important;
  left: 50% !important;
  transform: translateX(-50%) !important;
  z-index: var(--z-header, 50) !important;
  width: calc(100% - 32px);
  max-width: 1180px;
  visibility: visible !important;
  opacity: 1 !important;
}

/* ── Nav Pill ── */
.header-nav-pill {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 64px;
  padding: 0 20px;
  background: rgba(8, 12, 24, 0.72);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-pill);
}

/* ── Logo ── */
.header-logo-link {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-shrink: 0;
  cursor: pointer;
  text-decoration: none;
}

.header-logo-avatar {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid var(--color-border);
  transition: border-color 220ms ease;
}

.header-logo-avatar:hover {
  border-color: var(--color-border-strong);
}

.header-logo-avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.header-logo-text {
  font-size: 15px;
  font-weight: 700;
  color: var(--color-text);
  white-space: nowrap;
  letter-spacing: -0.01em;
}

/* ── Desktop Nav ── */
.header-nav-desktop {
  display: flex;
  align-items: center;
  gap: 2px;
}

@media (max-width: 768px) {
  .header-nav-desktop { display: none; }
}

.header-nav-link {
  display: flex;
  align-items: center;
  padding: 6px 14px;
  border-radius: var(--radius-pill);
  font-size: 14px;
  font-weight: 500;
  text-decoration: none;
  transition: background 180ms ease, color 180ms ease;
  white-space: nowrap;
}

.header-nav-link-inactive {
  color: var(--color-text-muted);
}

.header-nav-link-inactive:hover {
  color: var(--color-text);
  background: rgba(148, 163, 184, 0.08);
}

.header-nav-link-active {
  color: var(--color-text);
  background: var(--color-primary);
}

/* ── Right Actions ── */
.header-actions {
  display: flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;
}

.header-action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: var(--radius-pill);
  color: var(--color-text-muted);
  background: transparent;
  border: none;
  cursor: pointer;
  transition: color 180ms ease, background 180ms ease;
  text-decoration: none;
}

.header-action-btn:hover {
  color: var(--color-text);
  background: rgba(148, 163, 184, 0.08);
}

.header-action-btn svg {
  width: 18px;
  height: 18px;
}

.header-contact-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 14px;
  border-radius: var(--radius-pill);
  border: 1px solid var(--color-border);
  color: var(--color-text-muted);
  background: transparent;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: border-color 180ms ease, color 180ms ease;
  text-decoration: none;
  white-space: nowrap;
}

.header-contact-btn:hover {
  color: var(--color-text);
  border-color: var(--color-border-strong);
}

/* ── Mobile Menu Button ── */
.header-mobile-menu-button {
  display: none;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: var(--radius-pill);
  color: var(--color-text-muted);
  background: transparent;
  border: 1px solid var(--color-border);
  cursor: pointer;
}

@media (max-width: 768px) {
  .header-mobile-menu-button { display: flex; }
}

.header-mobile-menu-icon {
  width: 18px;
  height: 18px;
}

/* ── Mobile Dropdown ── */
.header-mobile-menu {
  position: absolute;
  top: calc(100% + 8px);
  left: 0;
  right: 0;
  background: rgba(8, 12, 24, 0.92);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: 8px;
  z-index: var(--z-dropdown, 60);
}

.header-mobile-menu-content {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.header-mobile-menu-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 14px;
  border-radius: var(--radius-md);
  font-size: 14px;
  font-weight: 500;
  text-decoration: none;
  transition: background 180ms ease, color 180ms ease;
}

.header-mobile-menu-item-inactive {
  color: var(--color-text-muted);
}

.header-mobile-menu-item-inactive:hover {
  color: var(--color-text);
  background: var(--color-surface);
}

.header-mobile-menu-item-active {
  color: var(--color-text);
  background: var(--color-primary);
}

/* ── Transitions ── */
.slide-down-enter-active,
.slide-down-leave-active {
  transition: opacity 180ms ease, transform 180ms ease;
}

.slide-down-enter-from,
.slide-down-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
```

---

### Task 7: 更新 Header.vue 导航结构

**Files:**
- Modify: `components/layout/Header.vue`

- [ ] **Step 1: 更新模板 — 将 `.header-main` 替换为 `.header-nav-pill`，移除「更多」下拉，扁平化导航**

找到 `<div class="header-main">` 到 `</div>` 的第一个外层闭合，替换为：

```html
<div class="header-nav-pill">
  <!-- Logo -->
  <NuxtLink to="/" class="header-logo-link">
    <div
      @click.stop="handleLogoClick"
      @mouseenter="handleAvatarHover"
      class="header-logo-avatar"
    >
      <img src="/images/avatar.jpg" alt="Ming Studio" />
    </div>
    <span class="header-logo-text">Ming Studio</span>
  </NuxtLink>

  <!-- Desktop Nav -->
  <nav class="header-nav-desktop">
    <NuxtLink
      v-for="item in navigationItems"
      :key="item.path"
      :to="item.path"
      class="header-nav-link"
      :class="isActiveRoute(item.path) ? 'header-nav-link-active' : 'header-nav-link-inactive'"
    >
      {{ item.title }}
    </NuxtLink>
  </nav>

  <!-- Right Actions -->
  <div class="header-actions">
    <!-- Search -->
    <NuxtLink to="/search" class="header-action-btn" title="搜索">
      <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
          d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
      </svg>
    </NuxtLink>

    <!-- Theme Toggle (placeholder, future multi-theme) -->
    <div class="header-action-btn">
      <ThemeToggle />
    </div>

    <!-- Contact -->
    <NuxtLink to="/contact" class="header-contact-btn">
      联系
    </NuxtLink>

    <!-- Mobile menu -->
    <button
      @click="toggleMobileMenu"
      class="header-mobile-menu-button"
      aria-label="菜单"
    >
      <svg class="header-mobile-menu-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
          d="M4 6h16M4 12h16M4 18h16"/>
      </svg>
    </button>
  </div>

  <!-- Mobile dropdown -->
  <Transition name="slide-down">
    <div v-if="isMobileMenuOpen" class="header-mobile-menu">
      <nav class="header-mobile-menu-content">
        <NuxtLink
          v-for="item in navigationItems"
          :key="item.path"
          :to="item.path"
          @click="closeMobileMenu"
          class="header-mobile-menu-item"
          :class="isActiveRoute(item.path)
            ? 'header-mobile-menu-item-active'
            : 'header-mobile-menu-item-inactive'"
        >
          {{ item.title }}
        </NuxtLink>
      </nav>
    </div>
  </Transition>
</div>
```

- [ ] **Step 2: 更新 `<script setup>` 中的 navigationItems computed**

找到 `mainNavigationItems` computed 和 `moreNavigationItems` computed，替换为：

```ts
const navigationItems = computed(() => {
  // @ts-ignore - Nuxt 3 auto-imports
  const { isModuleEnabled } = useModuleSystem()
  const isSSR = process.server

  const items: Array<{ title: string; path: string }> = [
    { title: '首页', path: '/' },
    { title: '产品', path: '/products' },
  ]

  if (isSSR || isModuleEnabled('projects')) {
    items.push({ title: '案例', path: '/projects' })
  }

  items.push({ title: 'AI实验室', path: '/lab' })

  if (isSSR || isModuleEnabled('blog')) {
    items.push({ title: '文章', path: '/blog' })
  }

  items.push({ title: '关于', path: '/about' })

  return items
})
```

同时删除原有的 `mainNavigationItems`、`moreNavigationItems`、`navigationItems`（旧版）三个 computed。删除 `isMoreMenuOpen`、`toggleMoreMenu`、`closeMoreMenu` 的 ref 和方法。

- [ ] **Step 3: 删除 `<div class="header-content-wrapper">` 包装层**

原模板在 `<header>` 内有 `<div class="header-content-wrapper">` 包一层。新模板直接用 `.header-nav-pill` 即可，移除 `header-content-wrapper`。

---

### Task 8: 重写 assets/css/footer.css（极简品牌 Footer）

**Files:**
- Modify: `assets/css/footer.css`

- [ ] **Step 1: 替换 footer.css 全部内容**

```css
/* Footer — Ming Studio minimal brand */

.footer-container {
  background: var(--color-bg-soft);
  border-top: 1px solid var(--color-border);
  color: var(--color-text-muted);
}

.footer-inner {
  max-width: var(--space-container);
  margin: 0 auto;
  padding: 64px clamp(16px, 4vw, 48px) 32px;
}

/* ── Main Grid ── */
.footer-grid {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr 1fr;
  gap: 48px;
  margin-bottom: 48px;
}

@media (max-width: 960px) {
  .footer-grid {
    grid-template-columns: 1fr 1fr;
    gap: 32px;
  }
}

@media (max-width: 600px) {
  .footer-grid {
    grid-template-columns: 1fr;
    gap: 32px;
  }
}

/* ── Brand Column ── */
.footer-brand-logo {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 16px;
}

.footer-brand-logo-icon {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: var(--gradient-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 800;
  color: #fff;
  flex-shrink: 0;
}

.footer-brand-name {
  font-size: 16px;
  font-weight: 700;
  color: var(--color-text);
  letter-spacing: -0.01em;
}

.footer-brand-tagline {
  font-size: 13px;
  font-weight: 500;
  color: var(--color-primary);
  margin-bottom: 12px;
  letter-spacing: 0.02em;
}

.footer-brand-desc {
  font-size: 14px;
  line-height: 1.7;
  color: var(--color-text-muted);
  max-width: 280px;
}

/* ── Nav Columns ── */
.footer-nav-title {
  font-size: 12px;
  font-weight: 600;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: var(--color-text-subtle);
  margin-bottom: 16px;
}

.footer-nav-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
  list-style: none;
  margin: 0;
  padding: 0;
}

.footer-nav-link {
  font-size: 14px;
  color: var(--color-text-muted);
  text-decoration: none;
  transition: color 180ms ease;
}

.footer-nav-link:hover {
  color: var(--color-text);
}

/* ── Social Links ── */
.footer-social-links {
  display: flex;
  gap: 8px;
  margin-top: 20px;
}

.footer-social-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: var(--radius-pill);
  border: 1px solid var(--color-border);
  color: var(--color-text-muted);
  text-decoration: none;
  transition: border-color 180ms ease, color 180ms ease;
  font-size: 14px;
}

.footer-social-btn:hover {
  border-color: var(--color-border-strong);
  color: var(--color-text);
}

/* ── Bottom Bar ── */
.footer-bottom {
  border-top: 1px solid var(--color-border);
  padding-top: 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 12px;
}

.footer-copyright {
  font-size: 13px;
  color: var(--color-text-subtle);
}

.footer-icp {
  font-size: 13px;
  color: var(--color-text-subtle);
  text-decoration: none;
  transition: color 180ms ease;
}

.footer-icp:hover {
  color: var(--color-text-muted);
}

/* ── Modals (WeChat / Email) ── */
.wechat-qr-modal,
.email-modal {
  position: fixed;
  inset: 0;
  z-index: var(--z-modal, 80);
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
}

.wechat-qr-content,
.email-modal-content {
  background: var(--color-surface-strong);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-xl);
  padding: 32px;
  max-width: 320px;
  width: 90%;
  position: relative;
}

.wechat-qr-close,
.email-modal-close {
  position: absolute;
  top: 16px;
  right: 16px;
  width: 28px;
  height: 28px;
  border-radius: var(--radius-pill);
  border: 1px solid var(--color-border);
  background: transparent;
  color: var(--color-text-muted);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  transition: background 180ms ease;
}

.wechat-qr-close:hover,
.email-modal-close:hover {
  background: var(--color-surface);
}
```

---

### Task 9: 更新 Footer.vue 模板结构

**Files:**
- Modify: `components/layout/Footer.vue`

- [ ] **Step 1: 替换 `<template>` 内容，保留原有 WeChat/Email 弹窗逻辑和 `<script setup>`**

```html
<template>
  <footer class="footer-container">
    <div class="footer-inner">
      <!-- Main Grid -->
      <div class="footer-grid">
        <!-- Brand -->
        <div>
          <div class="footer-brand-logo">
            <div class="footer-brand-logo-icon">XF</div>
            <span class="footer-brand-name">溪午听风</span>
          </div>
          <div class="footer-brand-tagline">AI Product Studio</div>
          <p class="footer-brand-desc">
            构建 AI 应用、数字产品与长期数字资产。<br>
            把想法变成可上线、可使用、可迭代的成果。
          </p>
          <div class="footer-social-links">
            <a href="https://github.com" target="_blank" class="footer-social-btn" title="GitHub">
              <i class="fab fa-github"></i>
            </a>
            <button class="footer-social-btn" title="邮箱" @click="showEmailModal = true">
              <i class="fas fa-envelope"></i>
            </button>
            <button class="footer-social-btn" title="微信" @click="showWeChatQR = true">
              <i class="fab fa-weixin"></i>
            </button>
          </div>
        </div>

        <!-- Products -->
        <div>
          <div class="footer-nav-title">产品</div>
          <ul class="footer-nav-list">
            <li><NuxtLink to="/products" class="footer-nav-link">全部产品</NuxtLink></li>
            <li><NuxtLink to="/tools" class="footer-nav-link">工具插件</NuxtLink></li>
            <li><NuxtLink to="/module-store" class="footer-nav-link">模块商店</NuxtLink></li>
          </ul>
        </div>

        <!-- Content -->
        <div>
          <div class="footer-nav-title">内容</div>
          <ul class="footer-nav-list">
            <li><NuxtLink to="/projects" class="footer-nav-link">案例</NuxtLink></li>
            <li><NuxtLink to="/lab" class="footer-nav-link">AI实验室</NuxtLink></li>
            <li><NuxtLink to="/blog" class="footer-nav-link">文章</NuxtLink></li>
          </ul>
        </div>

        <!-- About -->
        <div>
          <div class="footer-nav-title">关于</div>
          <ul class="footer-nav-list">
            <li><NuxtLink to="/about" class="footer-nav-link">关于我</NuxtLink></li>
            <li><NuxtLink to="/contact" class="footer-nav-link">联系合作</NuxtLink></li>
            <li><NuxtLink to="/changelog" class="footer-nav-link">更新日志</NuxtLink></li>
          </ul>
        </div>
      </div>

      <!-- Bottom -->
      <div class="footer-bottom">
        <span class="footer-copyright">© 2026 溪午听风</span>
        <a href="https://beian.miit.gov.cn" target="_blank" class="footer-icp">
          豫ICP备XXXXXXXX号
        </a>
      </div>
    </div>

    <!-- WeChat QR Modal -->
    <div v-if="showWeChatQR" class="wechat-qr-modal" @click="showWeChatQR = false">
      <div class="wechat-qr-content" @click.stop>
        <button class="wechat-qr-close" @click="showWeChatQR = false">✕</button>
        <div style="text-align:center">
          <img src="/images/wechat-qr.png" alt="微信二维码"
            style="width:180px;height:180px;border-radius:12px;margin:0 auto 12px" />
          <p class="footer-copyright">扫码加好友，请注明来意</p>
        </div>
      </div>
    </div>

    <!-- Email Modal -->
    <div v-if="showEmailModal" class="email-modal" @click="showEmailModal = false">
      <div class="email-modal-content" @click.stop>
        <button class="email-modal-close" @click="showEmailModal = false">✕</button>
        <p class="footer-nav-title" style="margin-bottom:8px">联系邮箱</p>
        <p style="font-size:15px;color:var(--color-text);font-family:var(--font-family-mono)">
          linxiwanting@gmail.com
        </p>
      </div>
    </div>
  </footer>
</template>
```

- [ ] **Step 2: 保留 `<script setup>` 中的 `showWeChatQR` 和 `showEmailModal` ref，删除其余旧业务逻辑**

`<script setup>` 简化为：
```ts
<script setup lang="ts">
import { ref } from 'vue'
const showWeChatQR = ref(false)
const showEmailModal = ref(false)
</script>
```

---

### Task 10: 更新 layouts/default.vue 和 layouts/home.vue

**Files:**
- Modify: `layouts/default.vue`
- Modify: `layouts/home.vue`

- [ ] **Step 1: default.vue — 更新最外层 div，移除旧 Tailwind 颜色类**

找到第 12 行：
```html
<div class="default-layout-shell min-h-screen flex flex-col relative bg-bg-body text-text-main">
```
替换为：
```html
<div class="default-layout-shell min-h-screen flex flex-col relative"
  style="background-color: var(--color-bg); color: var(--color-text);">
```

- [ ] **Step 2: default.vue — 更新 `<style scoped>` 中的硬编码颜色**

找到 `default-layout-backdrop-orb--primary` 和 `--secondary` 样式，把 `rgba(59, 130, 246, 0.22)` 改为引用 token（已经是正确值，保持不变即可）。

找到 `html[data-theme='dark']` 等条件块：

```css
html[data-theme='dark'] .default-layout-backdrop-grid,
html[data-theme='hybrid-super-dark'] .default-layout-backdrop-grid,
html.dark .default-layout-backdrop-grid {
  opacity: 0.14;
}
```

这些主题条件不影响新 Studio 主题（无 data-theme 属性时默认走 `:root`），**保留不动**。

- [ ] **Step 3: home.vue — 更新 style attribute**

找到第 9 行：
```html
<div class="min-h-screen flex flex-col relative" style="background-color: var(--color-bg-body); color: var(--color-text-main);">
```
替换为：
```html
<div class="min-h-screen flex flex-col relative"
  style="background-color: var(--color-bg); color: var(--color-text);">
```

---

### Task 11: Step 2 构建验收

- [ ] **Step 1: 运行构建**

```powershell
npm run build
```
预期：0 errors。

- [ ] **Step 2: 确认 Admin 不受影响**

启动 `npm run dev`，访问 `/admin`，检查：
- 侧边栏颜色正常（非深色 Studio 风格）
- 表格、卡片、按钮显示正常
- 无样式崩溃

- [ ] **Step 3: 提交 Step 2**

```powershell
git add assets/css/header.css assets/css/footer.css components/layout/Header.vue components/layout/Footer.vue layouts/default.vue layouts/home.vue
git commit -m "feat(ds): Step 2 — Studio header / footer / layout"
```

---

## STEP 3 — 首页

### Task 12: 更新 hero.css + home.css token 引用

**Files:**
- Modify: `assets/css/hero.css`
- Modify: `assets/css/home.css`

- [ ] **Step 1: hero.css — 替换旧 token 引用**

找到 `.hero-background-bg`：
```css
.hero-background-bg {
  background: radial-gradient(circle at 50% 0%, var(--bg-surface-2), var(--bg-app));
}
```
替换为：
```css
.hero-background-bg {
  background: var(--gradient-hero);
}
```

找到所有 `var(--bg-app)` 替换为 `var(--color-bg)`。
找到所有 `var(--bg-surface-1)` 替换为 `var(--color-surface)`。
找到所有 `var(--bg-surface-2)` 替换为 `var(--color-surface-strong)`。
找到所有 `var(--text-primary)` 替换为 `var(--color-text)`。
找到所有 `var(--text-secondary)` 替换为 `var(--color-text-muted)`。
找到所有 `var(--border-subtle)` 替换为 `var(--color-border)`。

- [ ] **Step 2: home.css — 替换 token 引用**

找到所有 `var(--color-bg-body)` 替换为 `var(--color-bg)`。
找到所有 `var(--color-text-main)` 替换为 `var(--color-text)`。
找到所有 `var(--color-text-sec)` 替换为 `var(--color-text-muted)`。
找到所有 `var(--color-border-subtle)` 替换为 `var(--color-border)`。

---

### Task 13: 首页 pages/index.vue token cleanup

**Files:**
- Modify: `pages/index.vue`

- [ ] **Step 1: 查找首页中的硬编码颜色**

```powershell
Select-String -Path "pages\index.vue" -Pattern "#[0-9a-fA-F]{3,6}|rgba?\([^)]+\)" | Select-Object LineNumber, Line
```
记录所有命中行。

- [ ] **Step 2: 替换内联 style 中的 bg/color 硬编码值**

对于每个命中项，根据语义替换：
- 深色背景 `#070b14` / `#0b1220` → `var(--color-bg)` / `var(--color-bg-soft)`
- 卡片背景 → `var(--color-surface)`
- 白色文字 `#f8fafc` / `#ffffff` → `var(--color-text)`
- 灰色文字 → `var(--color-text-muted)`
- 边框 → `var(--color-border)`
- 蓝色 `#3b82f6` → `var(--color-primary)`

- [ ] **Step 3: 将首页中使用的 `.card` / `.container` 类替换为 `.studio-card` / `.site-container`**

```powershell
# 查找旧 class 名
Select-String -Path "pages\index.vue" -Pattern 'class="[^"]*\bcard\b[^"]*"'
Select-String -Path "pages\index.vue" -Pattern 'class="[^"]*\bcontainer\b[^"]*"'
```
逐一替换为新前缀类名。

---

### Task 14: Step 3 构建验收

- [ ] **Step 1: 运行构建**

```powershell
npm run build
```
预期：0 errors。

- [ ] **Step 2: 三档响应式检查**

启动 `npm run dev`，访问 `/`，用浏览器 DevTools 分别切换到：
- 375px：导航 Logo+汉堡，Hero 单列，无横向滚动
- 768px：内容两列或单列均可，导航正常
- 1440px：Hero 标题明显最大，container 居中，卡片网格

- [ ] **Step 3: 提交 Step 3**

```powershell
git add assets/css/hero.css assets/css/home.css pages/index.vue
git commit -m "feat(ds): Step 3 — 首页接入 Studio 视觉系统"
```

---

## STEP 4 — 其他公开页面（逐步迁移）

每个页面单独迁移，完成一个提交一次。操作模式相同：

1. `Select-String` 扫描硬编码颜色
2. 替换为新 token
3. 将旧 class（`.card` / `.container` / `.section`）替换为 `.studio-card` / `.site-container` / `.site-section`
4. `npm run build`
5. 375 / 768 / 1440px 检查
6. Commit

### Task 15: 产品页 pages/products/** + components/products/**

- [ ] **Step 1: 扫描产品页和产品组件**

```powershell
Select-String -Path "pages\products" -Pattern "#[0-9a-fA-F]{3,6}|rgba?\([^)]+\)" -Recurse | Select-Object Path, LineNumber, Line
Select-String -Path "components\products" -Pattern "#[0-9a-fA-F]{3,6}|rgba?\([^)]+\)" -Recurse | Select-Object Path, LineNumber, Line
```

- [ ] **Step 2: 替换 token 引用（同 Task 13 Step 2 规则）**

- [ ] **Step 3: 构建 + 响应式检查 + 提交**

```powershell
npm run build
git add pages/products components/products
git commit -m "feat(ds): Step 4a — 产品页接入 Studio 视觉系统"
```

---

### Task 16: 项目/案例页 pages/projects/**

- [ ] **Step 1: 扫描**

```powershell
Select-String -Path "pages\projects" -Pattern "#[0-9a-fA-F]{3,6}|rgba?\([^)]+\)" -Recurse | Select-Object Path, LineNumber, Line
```

- [ ] **Step 2: 替换 + 构建 + 响应式检查 + 提交**

```powershell
npm run build
git add pages/projects
git commit -m "feat(ds): Step 4b — 案例页接入 Studio 视觉系统"
```

---

### Task 17: AI实验室 pages/lab.vue

- [ ] **Step 1: 扫描 + 替换 + 构建 + 提交**

```powershell
Select-String -Path "pages\lab.vue" -Pattern "#[0-9a-fA-F]{3,6}|rgba?\([^)]+\)" | Select-Object LineNumber, Line
npm run build
git add pages/lab.vue
git commit -m "feat(ds): Step 4c — AI实验室接入 Studio 视觉系统"
```

---

### Task 18: 文章页 pages/blog/**

- [ ] **Step 1: 扫描 + 替换 + 构建 + 提交**

```powershell
Select-String -Path "pages\blog" -Pattern "#[0-9a-fA-F]{3,6}|rgba?\([^)]+\)" -Recurse | Select-Object Path, LineNumber, Line
npm run build
git add pages/blog
git commit -m "feat(ds): Step 4d — 文章页接入 Studio 视觉系统"
```

---

### Task 19: 关于页 pages/about.vue

- [ ] **Step 1: 扫描 + 替换 + 构建 + 提交**

```powershell
Select-String -Path "pages\about.vue" -Pattern "#[0-9a-fA-F]{3,6}|rgba?\([^)]+\)" | Select-Object LineNumber, Line
npm run build
git add pages/about.vue
git commit -m "feat(ds): Step 4e — 关于页接入 Studio 视觉系统"
```

---

## STEP 5 — 清理旧 CSS 文件

### Task 20: 搜索确认无引用后删除旧文件

**Files:**
- Delete: `assets/styles/theme-variables.css` (after confirmation)
- Delete: `assets/css/design-system.css` (after confirmation)
- Delete: `assets/styles/theme.css` (after confirmation)
- Delete: `assets/css/themes.css` (after confirmation)

- [ ] **Step 1: 检查 theme-variables.css 剩余引用**

```powershell
Select-String -Path "." -Pattern "theme-variables" -Recurse -Include "*.vue","*.ts","*.css","*.js" | Where-Object { $_.Path -notmatch "node_modules" }
```
预期：只有 `assets/styles/index.css`（我们控制的 import）。

- [ ] **Step 2: 检查 design-system.css 引用**

```powershell
Select-String -Path "." -Pattern "design-system\.css" -Recurse -Include "*.vue","*.ts","*.css","*.js" | Where-Object { $_.Path -notmatch "node_modules" }
```
预期：0 结果（不在 nuxt.config.ts 中，不在 index.css 中）。
若有引用，先迁移再删除。

- [ ] **Step 3: 检查 theme.css 引用**

```powershell
Select-String -Path "." -Pattern "theme\.css" -Recurse -Include "*.vue","*.ts","*.css","*.js" | Where-Object { $_.Path -notmatch "node_modules" }
```
预期：只有 `assets/styles/index.css`。

- [ ] **Step 4: 检查 themes.css 引用**

```powershell
Select-String -Path "." -Pattern "themes\.css" -Recurse -Include "*.vue","*.ts","*.css","*.js" | Where-Object { $_.Path -notmatch "node_modules" }
```
预期：0 结果（已在 Task 4 中从 nuxt.config.ts 移除）。

- [ ] **Step 5: 从 index.css 移除旧文件 import，然后删除旧文件**

更新 `assets/styles/index.css`，移除以下行：
```css
@import './theme-variables.css';
@import './component-styles.css';
@import './theme.css';
```

然后删除文件：
```powershell
Remove-Item "assets\styles\theme-variables.css"
Remove-Item "assets\styles\theme.css"
Remove-Item "assets\css\design-system.css"
Remove-Item "assets\css\themes.css"
```

- [ ] **Step 6: 最终构建验收**

```powershell
npm run build
```
预期：0 errors。

- [ ] **Step 7: 最终提交**

```powershell
git add -A
git commit -m "feat(ds): Step 5 — 清理旧 CSS 文件，完成 Design System 迁移"
```

---

## 验收 Checklist

完成全部 Steps 后确认：

- [ ] `npm run build` 0 errors
- [ ] `/admin` 页面视觉前后一致（Admin 无影响）
- [ ] 首页 375px / 768px / 1440px 无溢出、无遮挡
- [ ] 导航为胶囊样式，含 首页/产品/案例/AI实验室/文章/关于
- [ ] Footer 为四列极简品牌结构
- [ ] 公开页内无裸 `#xxx` / `rgba()` 硬编码颜色
- [ ] `assets/styles/tokens.css` 是唯一 Token 源头
- [ ] 旧 CSS 文件已删除（Step 5 完成后）
