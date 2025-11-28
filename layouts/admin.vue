<template>
  <div 
    class="min-h-screen bg-gray-100 flex admin-layout"
    :style="inlineStyle"
  >
    <!-- 侧边栏 -->
    <aside 
      class="w-64 text-white flex flex-col fixed h-full left-0 top-0 z-10 admin-sidebar"
      :style="sidebarStyle"
    >
      <div class="p-6 text-xl font-bold border-b border-slate-700 flex items-center gap-2">
        <span>管理后台</span>
      </div>
      <nav class="flex-1 p-4 space-y-2 overflow-y-auto">
        <NuxtLink 
          to="/admin" 
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': $route.path === '/admin' }"
        >
          <i class="fas fa-chart-line w-6 text-center mr-2"></i>
          仪表盘
        </NuxtLink>
        
        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">内容管理</div>
        
        <NuxtLink 
          to="/admin/articles" 
          class="flex items-center px-4 py-2 rounded transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': $route.path.startsWith('/admin/articles') }"
        >
          <i class="fas fa-file-alt w-6 text-center mr-2"></i>
          文章管理
        </NuxtLink>
        
        <NuxtLink to="/admin/categories" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-folder w-6 text-center mr-2"></i>
          分类管理
        </NuxtLink>
        
        <NuxtLink to="/admin/projects" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-project-diagram w-6 text-center mr-2"></i>
          项目管理
        </NuxtLink>
        
        <NuxtLink to="/admin/tools" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-tools w-6 text-center mr-2"></i>
          工具管理
        </NuxtLink>

        <NuxtLink to="/admin/time-capsules" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-clock w-6 text-center mr-2"></i>
          时间胶囊
        </NuxtLink>

        <NuxtLink to="/admin/knowledge" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-book w-6 text-center mr-2"></i>
          知识库
        </NuxtLink>

        <NuxtLink to="/admin/timeline" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-history w-6 text-center mr-2"></i>
          时间线
        </NuxtLink>

        <NuxtLink to="/admin/analytics" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-chart-bar w-6 text-center mr-2"></i>
          访客分析
        </NuxtLink>

        <NuxtLink to="/admin/recommendations" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-brain w-6 text-center mr-2"></i>
          AI 推荐
        </NuxtLink>

        <NuxtLink to="/admin/investment" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-chart-line w-6 text-center mr-2"></i>
          投资仪表盘
        </NuxtLink>

        <NuxtLink to="/admin/skill-tree" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-sitemap w-6 text-center mr-2"></i>
          技能树管理
        </NuxtLink>

        <NuxtLink to="/admin/tasks" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-tasks w-6 text-center mr-2"></i>
          任务管理
        </NuxtLink>

        <NuxtLink to="/admin/goals" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-bullseye w-6 text-center mr-2"></i>
          年度目标
        </NuxtLink>

        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">系统设置</div>
        
        <NuxtLink to="/admin/users" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-users w-6 text-center mr-2"></i>
          用户管理
        </NuxtLink>
        
        <NuxtLink to="/admin/error-logs" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-exclamation-triangle w-6 text-center mr-2"></i>
          错误日志
        </NuxtLink>
        
        <NuxtLink to="/admin/friend-links" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-link w-6 text-center mr-2"></i>
          友情链接
        </NuxtLink>
        
        <NuxtLink to="/admin/config" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-cog w-6 text-center mr-2"></i>
          站点配置
        </NuxtLink>
        
        <NuxtLink to="/admin/home-styles" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-palette w-6 text-center mr-2"></i>
          首页风格管理
        </NuxtLink>
        
        <NuxtLink to="/admin/admin-styles" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-paint-brush w-6 text-center mr-2"></i>
          后台风格管理
        </NuxtLink>
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
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed, watch } from 'vue'
import { useAdminGlobalStyle } from '~/composables/useAdminStyle'

const router = useRouter()
const { globalStyle, styleConfig, cssVariables, inlineStyle, fetchGlobalStyle } = useAdminGlobalStyle()

// 侧边栏样式
const sidebarStyle = computed(() => {
  const vars = cssVariables.value
  return {
    backgroundColor: vars['--admin-sidebar-bg'] || '#1e293b',
    color: vars['--admin-sidebar-text'] || '#ffffff'
  }
})

// 主内容区样式
const mainContentStyle = computed(() => {
  const vars = cssVariables.value
  const config = styleConfig.value
  
  // 如果配置了背景渐变，使用渐变
  if (config.background?.type === 'gradient' && config.background.colors) {
    return {
      background: `linear-gradient(135deg, ${config.background.colors.join(', ')})`,
      minHeight: '100vh'
    }
  } else if (config.background?.color) {
    return {
      backgroundColor: config.background.color,
      minHeight: '100vh'
    }
  }
  
  // 默认使用与侧边栏协调的深色背景
  return {
    backgroundColor: vars['--admin-sidebar-bg'] || '#1e293b',
    background: `linear-gradient(135deg, ${vars['--admin-sidebar-bg'] || '#1e293b'}, ${vars['--admin-sidebar-bg'] || '#1e293b'}dd)`,
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
}

.admin-sidebar-link:hover {
  background-color: var(--admin-sidebar-hover-bg, #334155);
}

.admin-sidebar-link-active {
  background-color: var(--admin-sidebar-active-bg, #3b82f6) !important;
  color: #ffffff !important;
}

/* 主内容区背景 */
.admin-main {
  background: var(--admin-bg-gradient, #1e293b);
  color: #ffffff;
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
.admin-main :deep(.bg-white) {
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
</style>

