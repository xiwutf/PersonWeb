# 布局文件说明

本目录包含 Nuxt 3 项目的所有布局文件。每个布局文件定义了页面的整体结构，包括顶部导航栏、页脚等公共组件。

## 布局文件列表

### 1. `default.vue` - 默认布局（全局使用）
**用途**：所有普通页面的默认布局，包含完整的顶部导航栏和页脚。

**包含组件**：
- `<Header />` - 顶部导航栏（必须）
- `<Footer />` - 页脚
- `<AppNaiveConfig />` - Naive UI 配置容器
- `<MouseTrail />` - 鼠标轨迹特效
- `<ThemeSwitcher />` - 风格切换面板
- `<AIAssistant />` - AI 智能助手
- `<VisitorInteractionPanel />` - 访客互动功能
- `<VisitorBehaviorListener />` - 访客行为监听
- `<VisitorSidebarDrawer />` - 访客侧边栏抽屉
- `<SecretAdminAccess />` - 隐秘的后台入口

**使用场景**：
- 博客页面 (`/blog`)
- 工具页面 (`/tools`)
- 项目展示页面 (`/projects`)
- 关于页面 (`/about`)
- 游戏页面 (`/game`)
- 展示页面 (`/showcase`)
- 实验室页面 (`/lab`)
- 以及其他所有未指定布局的普通页面

**注意**：如果页面没有使用 `definePageMeta` 指定布局，Nuxt 3 会自动使用此默认布局。

---

### 2. `home.vue` - 首页布局
**用途**：首页专用布局，包含顶部导航栏，但主内容区域无顶部内边距（用于沉浸式 Hero 效果）。

**包含组件**：
- `<Header />` - 顶部导航栏（必须）
- `<Footer />` - 页脚
- `<ParticleBackground />` - 动态粒子背景
- `<MouseTrail />` - 鼠标轨迹特效
- `<ThemeSwitcher />` - 风格切换面板
- `<AIAssistant />` - AI 智能助手
- `<VisitorInteractionPanel />` - 访客互动功能
- `<VisitorBehaviorListener />` - 访客行为监听
- `<VisitorSidebarDrawer />` - 访客侧边栏抽屉
- `<SecretAdminAccess />` - 隐秘的后台入口

**使用场景**：
- 首页 (`/`)

**特点**：
- 主内容区域使用 `pt-0`（无顶部内边距），允许 Hero 区域延伸到顶部
- 包含粒子背景特效

---

### 3. `ai.vue` - AI 实验室布局
**用途**：AI 相关页面的专用布局，包含顶部导航栏。

**包含组件**：
- `<Header />` - 顶部导航栏（必须）
- `<Footer />` - 页脚
- `<MouseTrail />` - 鼠标轨迹特效
- `<ThemeSwitcher />` - 风格切换面板
- `<AIAssistant />` - AI 智能助手
- `<VisitorInteractionPanel />` - 访客互动功能
- `<VisitorBehaviorListener />` - 访客行为监听
- `<VisitorSidebarDrawer />` - 访客侧边栏抽屉

**使用场景**：
- AI 实验室首页 (`/ai`)
- AI 相关详情页 (`/ai/[type]/[slug]`)

**特点**：
- 主内容区域使用 `pt-24`（顶部内边距）

---

### 4. `admin.vue` - 后台管理布局
**用途**：后台管理系统的专用布局，**不包含**前台顶部导航栏，使用侧边栏导航。

**包含组件**：
- 侧边栏导航（不包含前台 Header）
- 管理后台专用组件

**使用场景**：
- 所有 `/admin/*` 路径下的页面

**注意**：此布局**不包含**前台顶部导航栏，这是正确的设计，因为后台管理系统有自己的导航体系。

---

## 布局选择规则

### 应该使用 `default` 布局的页面：
- ✅ 所有普通内容页面（博客、工具、项目等）
- ✅ 如果页面没有指定 `definePageMeta({ layout: 'xxx' })`，会自动使用 `default` 布局

### 应该使用 `home` 布局的页面：
- ✅ 首页 (`/`)

### 应该使用 `ai` 布局的页面：
- ✅ AI 实验室相关页面

### 应该使用 `admin` 布局的页面：
- ✅ 所有后台管理页面

### 不应该使用任何布局的页面（`layout: false`）：
- ✅ 登录页面 (`/admin/login`)
- ✅ 某些特殊的全屏页面（如 `/admin/visitors`）

---

## 如何为页面指定布局

在页面文件的 `<script setup>` 部分使用 `definePageMeta`：

```vue
<script setup lang="ts">
// 使用默认布局（包含顶部导航栏）
definePageMeta({
  layout: 'default'
})

// 或者不写，默认就是 default 布局
</script>
```

---

## 重要提示

1. **所有前台页面都应该显示顶部导航栏**，除非是特殊页面（如登录页）。
2. **默认布局 (`default.vue`) 必须始终包含 `<Header />` 组件**。
3. 如果发现某个前台页面没有显示导航栏，请检查：
   - 页面是否指定了 `layout: 'default'` 或没有指定布局
   - `default.vue` 布局文件是否包含 `<Header />` 组件
4. **后台管理页面使用 `admin` 布局，不显示前台导航栏**，这是正确的设计。

---

## 维护建议

- 新增布局文件时，请在此文档中说明用途
- 修改布局文件时，请确保不影响现有页面的显示
- 定期检查是否有页面错误地使用了不合适的布局

