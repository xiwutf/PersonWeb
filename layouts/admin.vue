<template>
  <div 
    class="min-h-screen flex admin-layout"
    :style="layoutStyle"
  >
    <!-- 侧边栏 -->
    <aside 
      class="w-64 text-white flex flex-col fixed h-full left-0 top-0 z-50 admin-sidebar"
      :style="sidebarStyle"
    >
      <div class="p-6 text-xl font-bold border-b border-slate-700 flex items-center gap-2">
        <span>管理后台</span>
      </div>
      <nav class="flex-1 p-4 space-y-2 overflow-y-auto">
        <NuxtLink 
          to="/admin" 
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path === '/admin' }"
        >
          <i class="fas fa-chart-line w-6 text-center mr-2"></i>
          仪表盘
        </NuxtLink>
        
        <!-- 内容管理 -->
        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">内容管理</div>
        
        <a 
          href="/admin/articles"
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path.startsWith('/admin/articles') }"
          @click.prevent="() => router.push('/admin/articles')"
        >
          <i class="fas fa-file-alt w-6 text-center mr-2"></i>
          文章管理
        </a>
        
        <a 
          href="/admin/categories"
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path === '/admin/categories' }"
          @click.prevent="() => router.push('/admin/categories')"
        >
          <i class="fas fa-folder w-6 text-center mr-2"></i>
          分类管理
        </a>

        <!-- 项目管理 -->
        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">项目管理</div>
        
        <a 
          href="/admin/projects"
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path.startsWith('/admin/projects') }"
          @click.prevent="() => router.push('/admin/projects')"
        >
          <i class="fas fa-project-diagram w-6 text-center mr-2"></i>
          项目管理
        </a>
        
        <a 
          href="/admin/tools"
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path === '/admin/tools' }"
          @click.prevent="() => router.push('/admin/tools')"
        >
          <i class="fas fa-tools w-6 text-center mr-2"></i>
          工具管理
        </a>

        <!-- 知识管理 -->
        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">知识管理</div>

        <NuxtLink to="/admin/knowledge" class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link" active-class="admin-sidebar-link-active">
          <i class="fas fa-book w-6 text-center mr-2"></i>
          知识库
        </NuxtLink>

        <NuxtLink to="/admin/timeline" class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link" active-class="admin-sidebar-link-active">
          <i class="fas fa-history w-6 text-center mr-2"></i>
          时间线
        </NuxtLink>

        <a 
          href="/admin/time-capsules"
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path === '/admin/time-capsules' }"
          @click.prevent="() => router.push('/admin/time-capsules')"
        >
          <i class="fas fa-clock w-6 text-center mr-2"></i>
          时间胶囊
        </a>

        <!-- 数据分析 -->
        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">数据分析</div>

        <NuxtLink to="/admin/analytics" class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link" active-class="admin-sidebar-link-active">
          <i class="fas fa-chart-bar w-6 text-center mr-2"></i>
          访客分析
        </NuxtLink>

        <NuxtLink to="/admin/investment" class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link" active-class="admin-sidebar-link-active">
          <i class="fas fa-chart-line w-6 text-center mr-2"></i>
          投资仪表盘
        </NuxtLink>

        <!-- 个人管理 -->
        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">个人管理</div>

        <NuxtLink to="/admin/tasks" class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link" active-class="admin-sidebar-link-active">
          <i class="fas fa-tasks w-6 text-center mr-2"></i>
          任务管理
        </NuxtLink>

        <NuxtLink to="/admin/goals" class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link" active-class="admin-sidebar-link-active">
          <i class="fas fa-bullseye w-6 text-center mr-2"></i>
          年度目标
        </NuxtLink>

        <NuxtLink to="/admin/toolbox" class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link" active-class="admin-sidebar-link-active">
          <span class="mr-2">🛒</span>
          <span>工具商城管理</span>
        </NuxtLink>
        <NuxtLink to="/admin/skill-tree" class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link" active-class="admin-sidebar-link-active">
          <i class="fas fa-sitemap w-6 text-center mr-2"></i>
          技能树管理
        </NuxtLink>

        <!-- 智能功能 -->
        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">智能功能</div>

        <NuxtLink to="/admin/recommendations" class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link" active-class="admin-sidebar-link-active">
          <i class="fas fa-brain w-6 text-center mr-2"></i>
          AI 推荐
        </NuxtLink>

        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">设置</div>
        
        <NuxtLink 
          to="/admin/settings" 
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path === '/admin/settings' || route.path.startsWith('/admin/settings') }"
        >
          <i class="fas fa-cog w-6 text-center mr-2"></i>
          系统设置
        </NuxtLink>
        
        <!-- 设置子菜单（可折叠，当前展开显示） -->
        <div v-if="route.path.startsWith('/admin/settings') || route.path === '/admin/config' || route.path === '/admin/users' || route.path === '/admin/error-logs' || route.path === '/admin/friend-links' || route.path === '/admin/home-styles' || route.path === '/admin/admin-styles'" class="ml-4 space-y-1">
          <NuxtLink 
            to="/admin/config" 
            class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
            :class="{ 'admin-sidebar-link-active': route.path === '/admin/config' }"
          >
            <i class="fas fa-cog w-6 text-center mr-2 text-sm"></i>
            <span class="text-sm">站点配置</span>
          </NuxtLink>
          
          <NuxtLink 
            to="/admin/users" 
            class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
            :class="{ 'admin-sidebar-link-active': route.path === '/admin/users' }"
          >
            <i class="fas fa-users w-6 text-center mr-2 text-sm"></i>
            <span class="text-sm">用户管理</span>
          </NuxtLink>
          
          <NuxtLink 
            to="/admin/friend-links" 
            class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
            :class="{ 'admin-sidebar-link-active': route.path.startsWith('/admin/friend-links') }"
          >
            <i class="fas fa-link w-6 text-center mr-2 text-sm"></i>
            <span class="text-sm">友情链接</span>
          </NuxtLink>
          
          <NuxtLink 
            to="/admin/home-styles" 
            class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
            :class="{ 'admin-sidebar-link-active': route.path === '/admin/home-styles' }"
          >
            <i class="fas fa-palette w-6 text-center mr-2 text-sm"></i>
            <span class="text-sm">首页风格管理</span>
          </NuxtLink>
          
          <NuxtLink 
            to="/admin/admin-styles" 
            class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
            :class="{ 'admin-sidebar-link-active': route.path === '/admin/admin-styles' }"
          >
            <i class="fas fa-paint-brush w-6 text-center mr-2 text-sm"></i>
            <span class="text-sm">后台风格管理</span>
          </NuxtLink>
          
          <NuxtLink 
            to="/admin/settings/styles" 
            class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
            :class="{ 'admin-sidebar-link-active': route.path === '/admin/settings/styles' }"
          >
            <i class="fas fa-palette w-6 text-center mr-2 text-sm"></i>
            <span class="text-sm">样式管理</span>
          </NuxtLink>
          
          <NuxtLink 
            to="/admin/settings/themes" 
            class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
            :class="{ 'admin-sidebar-link-active': route.path === '/admin/settings/themes' }"
          >
            <i class="fas fa-paint-brush w-6 text-center mr-2 text-sm"></i>
            <span class="text-sm">主题风格管理</span>
          </NuxtLink>
          
        <NuxtLink 
          to="/admin/settings/modules" 
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path === '/admin/settings/modules' }"
        >
          <i class="fas fa-puzzle-piece w-6 text-center mr-2 text-sm"></i>
          <span class="text-sm">模块管理</span>
        </NuxtLink>
        
        <NuxtLink 
          to="/admin/visitor-messages" 
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path === '/admin/visitor-messages' }"
        >
          <i class="fas fa-comments w-6 text-center mr-2 text-sm"></i>
          <span class="text-sm">访客留言</span>
        </NuxtLink>
          
          <NuxtLink 
            to="/admin/error-logs" 
            class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
            :class="{ 'admin-sidebar-link-active': route.path === '/admin/error-logs' }"
          >
            <i class="fas fa-exclamation-triangle w-6 text-center mr-2 text-sm"></i>
            <span class="text-sm">错误日志</span>
          </NuxtLink>
        </div>
      </nav>
      
      <div class="p-4 border-t border-slate-700">
        <button @click="logout" class="w-full flex items-center px-4 py-2 text-left hover:bg-slate-700 rounded text-red-300 transition-colors">
          <i class="fas fa-sign-out-alt w-6 text-center mr-2"></i>
          退出登录
        </button>
      </div>
    </aside>

    <!-- 主内容区 -->
    <main class="flex-1 ml-64 p-8 admin-main" :style="mainContentStyle">
      <ClientOnly>
        <n-config-provider :theme="darkTheme">
          <n-message-provider>
            <n-dialog-provider>
              <slot />
            </n-dialog-provider>
          </n-message-provider>
        </n-config-provider>
        <template #fallback>
          <slot />
        </template>
      </ClientOnly>
    </main>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed, watch } from 'vue'
import { NMessageProvider, NDialogProvider, NConfigProvider, darkTheme } from 'naive-ui'
import { useAdminGlobalStyle } from '~/composables/useAdminStyle'

const router = useRouter()
const route = useRoute()
const { globalStyle, styleConfig, cssVariables, inlineStyle, fetchGlobalStyle } = useAdminGlobalStyle()

// 侧边栏样式
const sidebarStyle = computed(() => {
  const vars = cssVariables.value
  return {
    backgroundColor: vars['--admin-sidebar-bg'] || '#1e293b',
    color: vars['--admin-sidebar-text'] || '#ffffff'
  }
})

// 布局样式（使用侧边栏背景色）
const layoutStyle = computed(() => {
  const vars = cssVariables.value
  return {
    backgroundColor: vars['--admin-sidebar-bg'] || '#1e293b',
    minHeight: '100vh'
  }
})

// 主内容区样式（使用侧边栏背景色）
const mainContentStyle = computed(() => {
  const vars = cssVariables.value
  return {
    backgroundColor: vars['--admin-sidebar-bg'] || '#1e293b',
    minHeight: '100vh'
  }
})

// 应用 CSS 变量到根元素
onMounted(() => {
  fetchGlobalStyle()
})

// 监听 CSS 变量变化，应用到根元素
watch(cssVariables, (vars) => {
  if (process.client) {
    Object.entries(vars).forEach(([key, value]) => {
      document.documentElement.style.setProperty(key, value)
    })
  }
}, { immediate: true, deep: true })

const handleLinkClick = (e: MouseEvent) => {
  // 确保点击事件正常触发
  if (process.env.NODE_ENV === 'development') {
    console.log('Link clicked:', (e.currentTarget as HTMLElement)?.getAttribute('href'))
  }
  // 不阻止默认行为，让 NuxtLink 正常处理
}

const logout = () => {
  if (confirm('确定要退出登录吗？')) {
    if (process.client) {
      localStorage.removeItem('admin_token')
      localStorage.removeItem('admin_user')
      router.push('/admin/login')
    }
  }
}
</script>

<style scoped>
/* 侧边栏链接样式 */
.admin-sidebar-link {
  color: var(--admin-sidebar-text, #ffffff);
  background-color: transparent;
  cursor: pointer;
  text-decoration: none;
  display: flex;
  align-items: center;
  position: relative;
  z-index: 1;
}

.admin-sidebar-link:hover {
  background-color: var(--admin-sidebar-hover-bg, #334155);
}

.admin-sidebar-link-active {
  background-color: var(--admin-sidebar-active-bg, #3b82f6) !important;
  color: #ffffff !important;
}

/* 确保链接可点击 */
.admin-sidebar-link,
.admin-sidebar-link * {
  pointer-events: auto !important;
  user-select: none;
}

/* 确保侧边栏本身可交互 */
.admin-sidebar {
  pointer-events: auto !important;
}

.admin-sidebar nav {
  pointer-events: auto !important;
}

/* 主内容区背景 - 使用侧边栏背景色 */
.admin-main {
  background: var(--admin-sidebar-bg, #1e293b) !important;
  color: #e5e7eb; /* 柔和的浅灰色文字 */
}

/* 如果配置了网格背景 */
.admin-layout::before {
  content: '';
  position: fixed;
  inset: 0;
  background-image: 
    linear-gradient(var(--admin-sidebar-text, #ffffff) 1px, transparent 1px),
    linear-gradient(90deg, var(--admin-sidebar-text, #ffffff) 1px, transparent 1px);
  background-size: var(--admin-grid-size, 40px) var(--admin-grid-size, 40px);
  opacity: var(--admin-grid-opacity, 0.03);
  pointer-events: none;
  z-index: 0;
}

/* 确保卡片在深色背景下可见 */
.admin-main :deep(.card),
.admin-main :deep(.page-container) {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.admin-main :deep(.card-hover) {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.admin-main :deep(.card-hover:hover) {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
}

/* 文字颜色调整 */
.admin-main :deep(.page-title),
.admin-main :deep(h1),
.admin-main :deep(h2),
.admin-main :deep(h3) {
  color: #ffffff;
}

.admin-main :deep(.text-gray-600),
.admin-main :deep(.text-gray-500) {
  color: rgba(255, 255, 255, 0.7);
}

.admin-main :deep(.text-gray-800) {
  color: #ffffff;
}

.admin-main :deep(.text-gray-900) {
  color: #ffffff;
}

/* 输入框和表单元素在深色背景下的样式 */
.admin-main :deep(.input),
.admin-main :deep(.form-input),
.admin-main :deep(input[type="text"]),
.admin-main :deep(input[type="password"]),
.admin-main :deep(input[type="email"]),
.admin-main :deep(input[type="number"]),
.admin-main :deep(input[type="date"]),
.admin-main :deep(input[type="datetime-local"]),
.admin-main :deep(select),
.admin-main :deep(textarea) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: #ffffff !important;
}

.admin-main :deep(.input::placeholder),
.admin-main :deep(.form-input::placeholder),
.admin-main :deep(input::placeholder),
.admin-main :deep(textarea::placeholder) {
  color: rgba(255, 255, 255, 0.5) !important;
}

/* 按钮在深色背景下的样式 */
.admin-main :deep(.btn-secondary) {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  color: #ffffff;
}

.admin-main :deep(.btn-secondary:hover) {
  background: rgba(255, 255, 255, 0.2);
}

/* 表格样式 */
.admin-main :deep(.table),
.admin-main :deep(table) {
  color: #ffffff;
}

.admin-main :deep(.table-header),
.admin-main :deep(thead) {
  background: rgba(255, 255, 255, 0.05);
}

.admin-main :deep(.table-header-cell),
.admin-main :deep(th) {
  color: rgba(255, 255, 255, 0.9);
  border-color: rgba(255, 255, 255, 0.1);
}

.admin-main :deep(.table-cell),
.admin-main :deep(td) {
  color: rgba(255, 255, 255, 0.8);
  border-color: rgba(255, 255, 255, 0.1);
}

.admin-main :deep(.table-row:hover),
.admin-main :deep(tr:hover) {
  background: rgba(255, 255, 255, 0.05);
}

/* 链接样式 */
.admin-main :deep(.btn-link),
.admin-main :deep(a) {
  color: rgba(255, 255, 255, 0.9);
}

.admin-main :deep(.btn-link--blue),
.admin-main :deep(.btn-link:hover) {
  color: #60a5fa;
}

.admin-main :deep(.btn-link--red) {
  color: #f87171;
}

/* 标签和徽章 */
.admin-main :deep(.badge) {
  color: #ffffff;
}

/* 模态框样式 */
.admin-main :deep(.modal-overlay) {
  background: rgba(0, 0, 0, 0.8);
}

.admin-main :deep(.modal-content),
.admin-main :deep(.modal-content-lg) {
  background: rgba(30, 41, 59, 0.95);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.admin-main :deep(.modal-header),
.admin-main :deep(.modal-title) {
  color: #ffffff;
}

.admin-main :deep(.modal-body) {
  color: rgba(255, 255, 255, 0.9);
}

/* 所有文本颜色统一 */
.admin-main :deep(.text-gray-600),
.admin-main :deep(.text-gray-500),
.admin-main :deep(.text-gray-400) {
  color: rgba(255, 255, 255, 0.7) !important;
}

.admin-main :deep(.text-gray-700) {
  color: rgba(255, 255, 255, 0.8) !important;
}

.admin-main :deep(.text-gray-800),
.admin-main :deep(.text-gray-900) {
  color: #ffffff !important;
}

/* 白色背景的卡片改为半透明 */
.admin-main :deep(.bg-white),
.admin-main :deep(.bg-white\/\*),
.admin-main :deep([class*="bg-white"]) {
  background: rgba(255, 255, 255, 0.05) !important;
  backdrop-filter: blur(10px);
}

/* 深色模式下的背景也统一 */
.admin-main :deep(.dark\:bg-gray-800),
.admin-main :deep(.dark\:bg-gray-900) {
  background: rgba(255, 255, 255, 0.05) !important;
  backdrop-filter: blur(10px);
}

/* 边框颜色调整 */
.admin-main :deep(.border-gray-200),
.admin-main :deep(.border-gray-300) {
  border-color: rgba(255, 255, 255, 0.1) !important;
}

.admin-main :deep(.border-gray-700) {
  border-color: rgba(255, 255, 255, 0.2) !important;
}

/* 分割线 */
.admin-main :deep(.divide-gray-200),
.admin-main :deep(.divide-gray-300) {
  border-color: rgba(255, 255, 255, 0.1) !important;
}

.admin-main :deep(.divide-gray-700) {
  border-color: rgba(255, 255, 255, 0.2) !important;
}

/* 悬停效果 */
.admin-main :deep(.hover\:bg-gray-50:hover) {
  background: rgba(255, 255, 255, 0.05) !important;
}

/* 加载和空状态 */
.admin-main :deep(.loading),
.admin-main :deep(.empty-state) {
  color: rgba(255, 255, 255, 0.7);
}

/* 统计卡片文字 */
.admin-main :deep(.stat-label),
.admin-main :deep(.stat-value) {
  color: #ffffff;
}

/* 所有 h1-h6 标题 */
.admin-main :deep(h1),
.admin-main :deep(h2),
.admin-main :deep(h3),
.admin-main :deep(h4),
.admin-main :deep(h5),
.admin-main :deep(h6) {
  color: #ffffff;
}

/* 段落文字 */
.admin-main :deep(p) {
  color: rgba(255, 255, 255, 0.9);
}

/* 列表文字 */
.admin-main :deep(li) {
  color: rgba(255, 255, 255, 0.9);
}

/* 标签文字 */
.admin-main :deep(label) {
  color: rgba(255, 255, 255, 0.9);
}

/* 选择框选项 */
.admin-main :deep(option) {
  background: #1e293b;
  color: #ffffff;
}

/* 表格容器 */
.admin-main :deep(.table-container) {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

/* 筛选栏 */
.admin-main :deep(.filter-bar) {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(255, 255, 255, 0.1);
}

/* Naive UI 组件深色主题样式 */
.admin-main :deep(.n-card) {
  background: rgba(255, 255, 255, 0.05) !important;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-card .n-card-header) {
  color: #ffffff !important;
  border-bottom-color: rgba(255, 255, 255, 0.1) !important;
}

.admin-main :deep(.n-card .n-card-body) {
  color: rgba(255, 255, 255, 0.9) !important;
}

/* Naive UI 数据表格深色主题 */
.admin-main :deep(.n-data-table) {
  background: transparent !important;
}

.admin-main :deep(.n-data-table .n-data-table-thead) {
  background: rgba(255, 255, 255, 0.05) !important;
}

.admin-main :deep(.n-data-table .n-data-table-thead th) {
  background: rgba(255, 255, 255, 0.05) !important;
  color: rgba(255, 255, 255, 0.9) !important;
  border-color: rgba(255, 255, 255, 0.1) !important;
}

.admin-main :deep(.n-data-table .n-data-table-tbody td) {
  background: transparent !important;
  color: rgba(255, 255, 255, 0.8) !important;
  border-color: rgba(255, 255, 255, 0.1) !important;
}

.admin-main :deep(.n-data-table .n-data-table-tbody tr:hover td) {
  background: rgba(255, 255, 255, 0.05) !important;
}

.admin-main :deep(.n-data-table .n-data-table-tbody tr:nth-child(even) td) {
  background: rgba(255, 255, 255, 0.02) !important;
}

/* Naive UI 输入框深色主题 */
.admin-main :deep(.n-input) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: #ffffff !important;
}

.admin-main :deep(.n-input .n-input__input-el) {
  color: #ffffff !important;
}

.admin-main :deep(.n-input .n-input__placeholder) {
  color: rgba(255, 255, 255, 0.5) !important;
}

/* Naive UI 按钮深色主题 */
.admin-main :deep(.n-button) {
  border-color: rgba(255, 255, 255, 0.2) !important;
}

.admin-main :deep(.n-button--default-type) {
  background: rgba(255, 255, 255, 0.1) !important;
  color: #ffffff !important;
}

.admin-main :deep(.n-button--default-type:hover) {
  background: rgba(255, 255, 255, 0.2) !important;
}

/* Naive UI 标签深色主题 */
.admin-main :deep(.n-tag) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: rgba(255, 255, 255, 0.9) !important;
}

/* Naive UI 分页深色主题 */
.admin-main :deep(.n-pagination) {
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-pagination .n-pagination-item) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-pagination .n-pagination-item:hover) {
  background: rgba(255, 255, 255, 0.2) !important;
}

.admin-main :deep(.n-pagination .n-pagination-item--active) {
  background: rgba(59, 130, 246, 0.3) !important;
  border-color: rgba(59, 130, 246, 0.5) !important;
  color: #ffffff !important;
}

/* Naive UI 模态框深色主题 */
.admin-main :deep(.n-modal) {
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-modal .n-card) {
  background: rgba(30, 41, 59, 0.95) !important;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
}

/* Naive UI 表单深色主题 */
.admin-main :deep(.n-form-item-label) {
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-form-item-feedback-wrapper) {
  color: rgba(255, 255, 255, 0.7) !important;
}

/* Naive UI 表格内嵌组件样式统一 */
/* 表格内的按钮 - quaternary 类型 */
.admin-main :deep(.n-data-table .n-button--quaternary) {
  background: transparent !important;
  border: none !important;
  color: rgba(255, 255, 255, 0.8) !important;
  transition: all 0.2s ease !important;
}

.admin-main :deep(.n-data-table .n-button--quaternary:hover) {
  background: rgba(255, 255, 255, 0.1) !important;
  color: #ffffff !important;
}

.admin-main :deep(.n-data-table .n-button--primary-type.n-button--quaternary) {
  color: #60a5fa !important;
}

.admin-main :deep(.n-data-table .n-button--primary-type.n-button--quaternary:hover) {
  background: rgba(96, 165, 250, 0.2) !important;
  color: #93c5fd !important;
}

.admin-main :deep(.n-data-table .n-button--error-type.n-button--quaternary) {
  color: #f87171 !important;
}

.admin-main :deep(.n-data-table .n-button--error-type.n-button--quaternary:hover) {
  background: rgba(248, 113, 113, 0.2) !important;
  color: #fca5a5 !important;
}

.admin-main :deep(.n-data-table .n-button--success-type.n-button--quaternary) {
  color: #34d399 !important;
}

.admin-main :deep(.n-data-table .n-button--success-type.n-button--quaternary:hover) {
  background: rgba(52, 211, 153, 0.2) !important;
  color: #6ee7b7 !important;
}

/* 表格内的标签样式 */
.admin-main :deep(.n-data-table .n-tag) {
  background: rgba(255, 255, 255, 0.1) !important;
  border: 1px solid rgba(255, 255, 255, 0.2) !important;
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-data-table .n-tag--info) {
  background: rgba(59, 130, 246, 0.2) !important;
  border-color: rgba(59, 130, 246, 0.4) !important;
  color: #93c5fd !important;
}

.admin-main :deep(.n-data-table .n-tag--success) {
  background: rgba(34, 197, 94, 0.2) !important;
  border-color: rgba(34, 197, 94, 0.4) !important;
  color: #6ee7b7 !important;
}

.admin-main :deep(.n-data-table .n-tag--warning) {
  background: rgba(251, 191, 36, 0.2) !important;
  border-color: rgba(251, 191, 36, 0.4) !important;
  color: #fcd34d !important;
}

.admin-main :deep(.n-data-table .n-tag--error) {
  background: rgba(239, 68, 68, 0.2) !important;
  border-color: rgba(239, 68, 68, 0.4) !important;
  color: #fca5a5 !important;
}

/* 表格内的头像 */
.admin-main :deep(.n-data-table .n-avatar) {
  border: 2px solid rgba(255, 255, 255, 0.1) !important;
}

/* 表格内的链接 */
.admin-main :deep(.n-data-table a) {
  color: #60a5fa !important;
  text-decoration: none !important;
  transition: color 0.2s ease !important;
}

.admin-main :deep(.n-data-table a:hover) {
  color: #93c5fd !important;
}

/* Popconfirm 深色主题 */
.admin-main :deep(.n-popover) {
  background: rgba(30, 41, 59, 0.95) !important;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-popover .n-popover__content) {
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-popover .n-button) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-popover .n-button:hover) {
  background: rgba(255, 255, 255, 0.2) !important;
}

.admin-main :deep(.n-popover .n-button--primary-type) {
  background: rgba(59, 130, 246, 0.3) !important;
  border-color: rgba(59, 130, 246, 0.5) !important;
  color: #ffffff !important;
}

.admin-main :deep(.n-popover .n-button--primary-type:hover) {
  background: rgba(59, 130, 246, 0.4) !important;
}

/* 表格内的自定义元素 */
.admin-main :deep(.n-data-table .project-info),
.admin-main :deep(.n-data-table .project-details),
.admin-main :deep(.n-data-table .project-title),
.admin-main :deep(.n-data-table .project-desc),
.admin-main :deep(.n-data-table .view-count),
.admin-main :deep(.n-data-table .action-buttons) {
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-data-table .project-desc) {
  color: rgba(255, 255, 255, 0.6) !important;
}

.admin-main :deep(.n-data-table .view-count) {
  color: rgba(255, 255, 255, 0.7) !important;
}

/* 表格内的图标颜色 */
.admin-main :deep(.n-data-table .fas),
.admin-main :deep(.n-data-table .fa) {
  color: rgba(255, 255, 255, 0.7) !important;
}

/* 表格内的操作按钮容器 */
.admin-main :deep(.n-data-table .action-buttons) {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

/* 确保表格内所有文字都是可见的 */
.admin-main :deep(.n-data-table td) {
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.n-data-table td .n-tag) {
  color: inherit !important;
}

/* 页面头部按钮统一样式 */
.admin-main :deep(.page-header .n-button),
.admin-main :deep(.header-actions .n-button) {
  border-color: rgba(255, 255, 255, 0.2) !important;
}

.admin-main :deep(.page-header .n-button--primary-type) {
  background: rgba(59, 130, 246, 0.3) !important;
  border-color: rgba(59, 130, 246, 0.5) !important;
  color: #ffffff !important;
}

.admin-main :deep(.page-header .n-button--primary-type:hover) {
  background: rgba(59, 130, 246, 0.4) !important;
  border-color: rgba(59, 130, 246, 0.6) !important;
}

.admin-main :deep(.page-header .n-button--success-type) {
  background: rgba(34, 197, 94, 0.3) !important;
  border-color: rgba(34, 197, 94, 0.5) !important;
  color: #ffffff !important;
}

.admin-main :deep(.page-header .n-button--success-type:hover) {
  background: rgba(34, 197, 94, 0.4) !important;
  border-color: rgba(34, 197, 94, 0.6) !important;
}

.admin-main :deep(.page-header .n-button--secondary) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: rgba(255, 255, 255, 0.9) !important;
}

.admin-main :deep(.page-header .n-button--secondary:hover) {
  background: rgba(255, 255, 255, 0.2) !important;
}

/* 筛选栏统一样式 */
.admin-main :deep(.filter-bar) {
  background: rgba(255, 255, 255, 0.05) !important;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
  border-radius: 0.5rem;
  padding: 1rem;
  margin-bottom: 1.5rem;
}

/* 页面标题统一样式 */
.admin-main :deep(.page-title) {
  color: #ffffff !important;
  font-weight: 600;
  margin-bottom: 1rem;
}

.admin-main :deep(.page-header) {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

/* 统一所有按钮的图标颜色 */
.admin-main :deep(.n-button .fas),
.admin-main :deep(.n-button .fa) {
  color: inherit !important;
}

/* 统一加载状态样式 */
.admin-main :deep(.n-spin) {
  color: rgba(255, 255, 255, 0.7) !important;
}

/* 统一空状态样式 */
.admin-main :deep(.n-empty) {
  color: rgba(255, 255, 255, 0.6) !important;
}

.admin-main :deep(.n-empty .n-empty__description) {
  color: rgba(255, 255, 255, 0.6) !important;
}

/* 确保所有页面容器都使用深色背景 */
.admin-main > * {
  background: transparent;
}

/* 覆盖可能的白色背景 */
.admin-main :deep(div[style*="background"]),
.admin-main :deep(div[style*="background-color"]) {
  /* 不强制覆盖内联样式，但确保默认背景透明 */
}

/* 设置子菜单样式 */
.admin-sidebar .ml-4 {
  margin-left: 1rem;
  border-left: 2px solid rgba(255, 255, 255, 0.1);
  padding-left: 0.5rem;
}

.admin-sidebar .ml-4 .admin-sidebar-link {
  font-size: 0.875rem;
  padding-left: 0.75rem;
}
</style>

