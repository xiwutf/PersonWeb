<template>
  <!-- 
    后台管理布局（admin.vue）
    用途：后台管理系统的专用布局，不包含前台顶部导航栏，使用侧边栏导航
    使用场景：所有 /admin/* 路径下的页面
    注意：此布局不包含前台 Header 组件，这是正确的设计，因为后台管理系统有自己的导航体系
  -->
  <div 
    class="min-h-screen flex admin-layout"
    :style="layoutStyle"
  >
    <!-- 侧边栏：使用主题颜色，替换写死的 text-white 和 border-slate-700 -->
    <!-- 注意：后台管理布局不包含前台 Header 组件，使用侧边栏导航 -->
    <!-- 如果在 iframe 中嵌入，则隐藏侧边栏 -->
    <aside 
      v-if="!isEmbedded"
      class="w-64 flex flex-col fixed h-full left-0 top-0 z-50 admin-sidebar"
      :style="sidebarStyle"
    >
      <div class="p-6 text-xl font-bold border-b flex items-center gap-2 admin-sidebar-header">
        <span>管理后台</span>
      </div>
      <nav class="flex-1 p-4 space-y-1 overflow-y-auto">
        <!-- 仪表盘 -->
        <NuxtLink 
          to="/admin" 
          class="flex items-center px-4 py-2.5 rounded-lg transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path === '/admin' }"
        >
          <i class="fas fa-chart-line w-5 text-center mr-3"></i>
          <span class="text-sm font-medium">仪表盘</span>
        </NuxtLink>

        <!-- AI 智能体中心 -->
        <NuxtLink 
          to="/admin/ai" 
          class="flex items-center px-4 py-2.5 rounded-lg transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path.startsWith('/admin/ai') }"
        >
          <i class="fas fa-magic w-5 text-center mr-3"></i>
          <span class="text-sm font-medium">AI 智能体</span>
        </NuxtLink>
        
        <!-- 内容管理 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('content')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('content') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.content }"></i>
            <span class="text-xs uppercase font-semibold tracking-wider menu-group-title">内容管理</span>
          </button>
          <div v-show="expandedMenus.content" class="menu-group-items">
            <a 
              href="/admin/articles"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path.startsWith('/admin/articles') }"
              @click.prevent="() => router.push('/admin/articles')"
            >
              <i class="fas fa-file-alt w-5 text-center mr-3"></i>
              <span>文章</span>
            </a>
            <a 
              href="/admin/categories"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/categories' }"
              @click.prevent="() => router.push('/admin/categories')"
            >
              <i class="fas fa-folder w-5 text-center mr-3"></i>
              <span>分类</span>
            </a>
          </div>
        </div>

        <!-- 项目管理 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('project')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('project') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.project }"></i>
            <span class="text-xs text-text-muted uppercase font-semibold tracking-wider">项目管理</span>
          </button>
          <div v-show="expandedMenus.project" class="menu-group-items">
            <a 
              href="/admin/projects"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path.startsWith('/admin/projects') }"
              @click.prevent="() => router.push('/admin/projects')"
            >
              <i class="fas fa-project-diagram w-5 text-center mr-3"></i>
              <span>项目</span>
            </a>
            <a 
              href="/admin/tools"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/tools' }"
              @click.prevent="() => router.push('/admin/tools')"
            >
              <i class="fas fa-tools w-5 text-center mr-3"></i>
              <span>工具</span>
            </a>
          </div>
        </div>

        <!-- 知识管理 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('knowledge')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('knowledge') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.knowledge }"></i>
            <span class="text-xs text-text-muted uppercase font-semibold tracking-wider">知识管理</span>
          </button>
          <div v-show="expandedMenus.knowledge" class="menu-group-items">
            <NuxtLink to="/admin/knowledge" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-book w-5 text-center mr-3"></i>
              <span>知识库</span>
            </NuxtLink>
            <NuxtLink to="/admin/timeline" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-history w-5 text-center mr-3"></i>
              <span>时间线</span>
            </NuxtLink>
            <a 
              href="/admin/time-capsules"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/time-capsules' }"
              @click.prevent="() => router.push('/admin/time-capsules')"
            >
              <i class="fas fa-clock w-5 text-center mr-3"></i>
              <span>时间胶囊</span>
            </a>
            <NuxtLink 
              to="/admin/document-agent" 
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              active-class="admin-sidebar-link-active"
            >
              <i class="fas fa-file-alt w-5 text-center mr-3"></i>
              <span>文档知识管家</span>
            </NuxtLink>
          </div>
        </div>

        <!-- 数据分析 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('analytics')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('analytics') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.analytics }"></i>
            <span class="text-xs text-text-muted uppercase font-semibold tracking-wider">数据分析</span>
          </button>
          <div v-show="expandedMenus.analytics" class="menu-group-items">
            <NuxtLink to="/admin/analytics" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-chart-bar w-5 text-center mr-3"></i>
              <span>访客分析</span>
            </NuxtLink>
            <NuxtLink to="/admin/investment" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-chart-line w-5 text-center mr-3"></i>
              <span>投资仪表盘</span>
            </NuxtLink>
            <a 
              href="/admin/asset-management"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/asset-management' || route.path.startsWith('/admin/asset-management') }"
              @click.prevent="() => router.push('/admin/asset-management')"
            >
              <i class="fas fa-wallet w-5 text-center mr-3"></i>
              <span>资产管理</span>
            </a>
          </div>
        </div>

        <!-- 副业管理 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('sideBusiness')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('sideBusiness') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.sideBusiness }"></i>
            <span class="text-xs text-text-muted uppercase font-semibold tracking-wider">副业管理</span>
          </button>
          <div v-show="expandedMenus.sideBusiness" class="menu-group-items">
            <a 
              href="/admin/side-projects/dashboard"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/side-projects/dashboard' }"
              @click.prevent="() => router.push('/admin/side-projects/dashboard')"
            >
              <i class="fas fa-home w-5 text-center mr-3"></i>
              <span>副业管理首页</span>
            </a>
            <a 
              href="/admin/side-projects"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/side-projects' && !route.path.includes('/kanban') && !route.path.includes('/projects/') && route.path !== '/admin/side-projects/dashboard' }"
              @click.prevent="() => router.push('/admin/side-projects')"
            >
              <i class="fas fa-list w-5 text-center mr-3"></i>
              <span>项目列表</span>
            </a>
            <a 
              href="/admin/side-projects/kanban"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/side-projects/kanban' }"
              @click.prevent="() => router.push('/admin/side-projects/kanban')"
            >
              <i class="fas fa-columns w-5 text-center mr-3"></i>
              <span>看板视图</span>
            </a>
            <a 
              href="/admin/side-projects/analytics"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/side-projects/analytics' }"
              @click.prevent="() => router.push('/admin/side-projects/analytics')"
            >
              <i class="fas fa-chart-bar w-5 text-center mr-3"></i>
              <span>数据分析</span>
            </a>
          </div>
        </div>

        <!-- 个人管理 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('personal')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('personal') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.personal }"></i>
            <span class="text-xs text-text-muted uppercase font-semibold tracking-wider">个人管理</span>
          </button>
          <div v-show="expandedMenus.personal" class="menu-group-items">
            <NuxtLink to="/admin/tasks" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-tasks w-5 text-center mr-3"></i>
              <span>任务</span>
            </NuxtLink>
            <NuxtLink to="/admin/goals" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-bullseye w-5 text-center mr-3"></i>
              <span>年度目标</span>
            </NuxtLink>
            <a
              href="/admin/relations"
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/relations' || route.path.startsWith('/admin/relations') }"
              @click.prevent="() => router.push('/admin/relations')"
            >
              <i class="fas fa-heart w-5 text-center mr-3"></i>
              <span>关系跟进</span>
            </a>
            <NuxtLink to="/admin/toolbox" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-shopping-cart w-5 text-center mr-3"></i>
              <span>工具商城</span>
            </NuxtLink>
          </div>
        </div>

        <!-- 商业化 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('commercial')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('commercial') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.commercial }"></i>
            <span class="text-xs text-text-muted uppercase font-semibold tracking-wider">商业化</span>
          </button>
          <div v-show="expandedMenus.commercial" class="menu-group-items">
            <NuxtLink to="/admin/commercial/themes" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-palette w-5 text-center mr-3"></i>
              <span>主题商店</span>
            </NuxtLink>
            <NuxtLink to="/admin/commercial/api-keys" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-key w-5 text-center mr-3"></i>
              <span>API密钥</span>
            </NuxtLink>
            <NuxtLink to="/admin/commercial/memberships" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-crown w-5 text-center mr-3"></i>
              <span>会员</span>
            </NuxtLink>
            <NuxtLink to="/admin/orders" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-shopping-cart w-5 text-center mr-3"></i>
              <span>订单管理</span>
            </NuxtLink>
            <NuxtLink to="/admin/consultations" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-comments w-5 text-center mr-3"></i>
              <span>咨询管理</span>
            </NuxtLink>
            <NuxtLink to="/admin/skill-tree" class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm" active-class="admin-sidebar-link-active">
              <i class="fas fa-sitemap w-5 text-center mr-3"></i>
              <span>技能树</span>
            </NuxtLink>
          </div>
        </div>

        <!-- AI推荐（直接显示，不折叠） -->
        <NuxtLink 
          to="/admin/recommendations" 
          class="flex items-center px-4 py-2.5 rounded-lg transition-colors admin-sidebar-link"
          :class="{ 'admin-sidebar-link-active': route.path === '/admin/recommendations' }"
        >
          <i class="fas fa-brain w-5 text-center mr-3"></i>
          <span class="text-sm font-medium">AI推荐</span>
        </NuxtLink>

        <!-- 系统设置 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('system')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('system') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.system }"></i>
            <span class="text-xs text-text-muted uppercase font-semibold tracking-wider">系统设置</span>
          </button>
          <div v-show="expandedMenus.system" class="menu-group-items">
            <NuxtLink 
              to="/admin/settings" 
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/settings' && !route.path.startsWith('/admin/settings/') }"
            >
              <i class="fas fa-cog w-5 text-center mr-3"></i>
              <span>系统配置</span>
            </NuxtLink>
            <NuxtLink 
              to="/admin/config" 
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/config' }"
            >
              <i class="fas fa-server w-5 text-center mr-3"></i>
              <span>站点配置</span>
            </NuxtLink>
            <NuxtLink 
              to="/admin/settings/change-password" 
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/settings/change-password' }"
            >
              <i class="fas fa-key w-5 text-center mr-3"></i>
              <span>修改密码</span>
            </NuxtLink>
          </div>
        </div>

        <!-- 主题样式 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('theme')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('theme') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.theme }"></i>
            <span class="text-xs text-text-muted uppercase font-semibold tracking-wider">主题样式</span>
          </button>
          <div v-show="expandedMenus.theme" class="menu-group-items">
            <NuxtLink 
              to="/admin/home-styles" 
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/home-styles' }"
            >
              <i class="fas fa-home w-5 text-center mr-3"></i>
              <span>首页风格</span>
            </NuxtLink>
            <NuxtLink 
              to="/admin/theme-settings" 
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/theme-settings' }"
            >
              <i class="fas fa-adjust w-5 text-center mr-3"></i>
              <span>UI主题</span>
            </NuxtLink>
          </div>
        </div>

        <!-- 其他 -->
        <div class="menu-group">
          <button 
            @click="toggleMenu('other')"
            class="menu-group-header"
            :class="{ 'menu-group-active': isMenuActive('other') }"
          >
            <i class="fas fa-chevron-right transition-transform duration-200 mr-2 text-xs" :class="{ 'rotate-90': expandedMenus.other }"></i>
            <span class="text-xs text-text-muted uppercase font-semibold tracking-wider">其他</span>
          </button>
          <div v-show="expandedMenus.other" class="menu-group-items">
            <NuxtLink 
              to="/admin/friend-links" 
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path.startsWith('/admin/friend-links') }"
            >
              <i class="fas fa-link w-5 text-center mr-3"></i>
              <span>友情链接</span>
            </NuxtLink>
            <NuxtLink 
              to="/admin/visitor-messages" 
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/visitor-messages' }"
            >
              <i class="fas fa-comments w-5 text-center mr-3"></i>
              <span>访客留言</span>
            </NuxtLink>
            <NuxtLink 
              to="/admin/error-logs" 
              class="flex items-center px-4 py-2 rounded-md transition-colors admin-sidebar-link text-sm"
              :class="{ 'admin-sidebar-link-active': route.path === '/admin/error-logs' }"
            >
              <i class="fas fa-exclamation-triangle w-5 text-center mr-3"></i>
              <span>错误日志</span>
            </NuxtLink>
          </div>
        </div>
      </nav>
      
      <!-- 退出登录区域：使用主题边框颜色，替换写死的 border-slate-700 -->
      <div class="p-4 border-t border-border-subtle">
        <button @click="logout" class="w-full flex items-center px-4 py-2 text-left rounded transition-colors admin-sidebar-link">
          <i class="fas fa-sign-out-alt w-5 text-center mr-3"></i>
          <span class="text-sm font-medium">退出登录</span>
        </button>
      </div>
    </aside>

    <!-- 主内容区：使用主题背景色和文字颜色 -->
    <main 
      class="flex-1 admin-main flex flex-col" 
      :class="{ 'ml-64': !isEmbedded }"
      :style="mainContentStyle"
    >
      <!-- 顶部栏（包含铃铛入口） -->
      <ClientOnly>
        <div class="admin-topbar border-b border-border-subtle bg-bg-elevated px-6 py-4 flex items-center justify-end gap-4">
          <NotificationBell />
        </div>
      </ClientOnly>

      <!-- 使用统一的 AppNaiveConfig，确保前台和后台共用同一套主题配置 -->
      <!-- 使用 ClientOnly 避免 SSR 时的闪烁 -->
      <div class="flex-1 overflow-auto">
        <ClientOnly>
          <AppNaiveConfig>
            <slot />
          </AppNaiveConfig>
          <template #fallback>
            <!-- SSR 时的占位内容，使用与主题一致的背景色 -->
            <div class="admin-main-fallback">
              <slot />
            </div>
          </template>
        </ClientOnly>
      </div>
    </main>

    <!-- 鼠标轨迹特效 -->
    <ClientOnly>
      <MouseTrail />
    </ClientOnly>
    
    <!-- 风格切换面板 -->
    <ClientOnly>
      <ThemeSwitcher />
    </ClientOnly>
    
    <!-- AI 智能助手 -->
    <ClientOnly>
      <!-- 后台管理中隐藏 AI 助手 -->
      <!-- <AIAssistant /> -->
    </ClientOnly>
    
    <!-- 访客互动功能 -->
    <ClientOnly>
      <!-- 后台管理中隐藏访客互动面板 -->
      <!-- <VisitorInteractionPanel /> -->
    </ClientOnly>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed, watch, ref, nextTick } from 'vue'
import { useAdminGlobalStyle } from '~/composables/useAdminStyle'
import AppNaiveConfig from '~/components/layout/AppNaiveConfig.vue'
import MouseTrail from '~/components/effects/MouseTrail.vue'
import ThemeSwitcher from '~/components/layout/ThemeSwitcher.vue'

const router = useRouter()
const route = useRoute()

// 检测是否在 iframe 中嵌入（通过 URL 参数或 window 检测）
const isEmbedded = ref(false)
const { globalStyle, styleConfig, cssVariables, inlineStyle, fetchGlobalStyle } = useAdminGlobalStyle()

// 使用全局主题系统（前台和后台共用）
const { currentTheme } = useTheme()

// 菜单折叠状态
const expandedMenus = ref<Record<string, boolean>>({
  content: false,
  project: false,
  knowledge: false,
  analytics: false,
  sideBusiness: false,
  personal: false,
  commercial: false,
  system: false,
  theme: false,
  other: false
})

// 切换菜单展开/折叠
const toggleMenu = (menuKey: string) => {
  const currentValue = expandedMenus.value[menuKey]
  expandedMenus.value[menuKey] = !currentValue
}

// 检查菜单是否应该高亮（包含活动路由）
const isMenuActive = (menuKey: string): boolean => {
  const path = route.path
  switch (menuKey) {
    case 'content':
      return path.startsWith('/admin/articles') || path === '/admin/categories'
    case 'project':
      return path.startsWith('/admin/projects') || path === '/admin/tools'
    case 'knowledge':
      return path === '/admin/knowledge' || path === '/admin/timeline' || path === '/admin/time-capsules' || path === '/admin/document-agent'
    case 'analytics':
      return path === '/admin/analytics' || path === '/admin/investment' || path === '/admin/asset-management' || path.startsWith('/admin/asset-management')
    case 'sideBusiness':
      return path.startsWith('/admin/side-projects')
    case 'personal':
      return path === '/admin/tasks' || path === '/admin/goals' || path === '/admin/relations' || path.startsWith('/admin/relations') || path === '/admin/toolbox'
    case 'commercial':
      return path.startsWith('/admin/commercial') || path === '/admin/skill-tree' || path === '/admin/orders' || path === '/admin/consultations'
    case 'system':
      return path.startsWith('/admin/settings') && !path.startsWith('/admin/settings/styles') && !path.startsWith('/admin/settings/themes') && path !== '/admin/settings/modules' ||
             path === '/admin/config' || 
             path === '/admin/settings/change-password'
    case 'theme':
      return path === '/admin/home-styles' || 
             path === '/admin/theme-settings'
    case 'other':
      return path.startsWith('/admin/friend-links') || 
             path === '/admin/visitor-messages' ||
             path === '/admin/error-logs'
    default:
      return false
  }
}

// 根据当前路由自动展开对应的菜单
const autoExpandMenu = () => {
  const path = route.path
  if (path.startsWith('/admin/articles') || path === '/admin/categories') {
    expandedMenus.value.content = true
  }
  if (path.startsWith('/admin/projects') || path === '/admin/tools') {
    expandedMenus.value.project = true
  }
  if (path === '/admin/knowledge' || path === '/admin/timeline' || path === '/admin/time-capsules' || path === '/admin/document-agent') {
    expandedMenus.value.knowledge = true
  }
  if (path === '/admin/analytics' || path === '/admin/investment' || path === '/admin/asset-management' || path.startsWith('/admin/asset-management')) {
    expandedMenus.value.analytics = true
  }
  if (path.startsWith('/admin/side-projects')) {
    expandedMenus.value.sideBusiness = true
  }
  if (path === '/admin/tasks' || path === '/admin/goals' || path === '/admin/relations' || path.startsWith('/admin/relations') || path === '/admin/toolbox') {
    expandedMenus.value.personal = true
  }
  if (path.startsWith('/admin/commercial') || path === '/admin/skill-tree' || path === '/admin/orders' || path === '/admin/consultations') {
    expandedMenus.value.commercial = true
  }
  if (path.startsWith('/admin/settings') && !path.startsWith('/admin/settings/styles') && !path.startsWith('/admin/settings/themes') && path !== '/admin/settings/modules' ||
      path === '/admin/config' || 
      path === '/admin/settings/change-password') {
    expandedMenus.value.system = true
  }
  if (path === '/admin/home-styles' || 
      path === '/admin/theme-settings') {
    expandedMenus.value.theme = true
  }
  if (path.startsWith('/admin/friend-links') || 
      path === '/admin/visitor-messages' ||
      path === '/admin/error-logs') {
    expandedMenus.value.other = true
  }
}

// 监听路由变化，自动展开对应菜单
// 使用 flush: 'post' 确保在 DOM 更新后执行
watch(() => route.path, (newPath, oldPath) => {
  // 只有当路径真正改变时才执行
  if (newPath !== oldPath) {
    autoExpandMenu()
  }
}, { immediate: true, flush: 'post' })

// 侧边栏样式 - 使用 memo 避免不必要的重新计算
const sidebarStyle = computed(() => {
  const vars = cssVariables.value
  const bg = vars['--admin-sidebar-bg'] || '#1e293b'
  const text = vars['--admin-sidebar-text'] || '#ffffff'
  return {
    backgroundColor: bg,
    color: text
  }
})

// 布局样式（使用侧边栏背景色）
const layoutStyle = computed(() => {
  const vars = cssVariables.value
  const bg = vars['--admin-sidebar-bg'] || '#1e293b'
  return {
    backgroundColor: bg,
    minHeight: '100vh'
  }
})

// 主内容区样式（Vision Pro 风格 - 深色模式下使用渐变）
const mainContentStyle = computed(() => {
  // 如果在 iframe 中，减少 padding
  if (isEmbedded.value) {
    return {
      padding: '0'
    }
  }
  const vars = cssVariables.value
  const isDark = currentTheme.value === 'dark'
  
  // 深色模式下使用渐变背景（通过 themeOverrides.Layout.color 控制）
  // 浅色模式使用纯色背景
  return {
    minHeight: '100vh',
    position: 'relative',
    zIndex: 1
  }
})

// 应用 CSS 变量到根元素
onMounted(() => {
  fetchGlobalStyle()
  
  // 检测是否在 iframe 中嵌入（通过 URL 参数或 window 检测）
  // 检测 URL 参数
  if (route.query.embedded === 'true') {
    isEmbedded.value = true
  } else if (process.client) {
    // 检测是否在 iframe 中
    try {
      isEmbedded.value = window.self !== window.top
    } catch (e) {
      // 跨域情况下会抛出异常，此时也认为是在 iframe 中
      isEmbedded.value = true
    }
  }
})

// 监听 CSS 变量变化，应用到根元素
// 使用 shallowRef 存储上一次的值，避免不必要的更新
const lastCssVars = ref<Record<string, string>>({})

watch(cssVariables, (vars) => {
  if (!process.client) return
  
  // 检查是否有真正的变化
  const hasChanged = Object.keys(vars).some(key => {
    return lastCssVars.value[key] !== vars[key]
  }) || Object.keys(lastCssVars.value).some(key => {
    return !(key in vars)
  })
  
  if (!hasChanged) return
  
  // 批量更新，减少 DOM 操作
  requestAnimationFrame(() => {
    Object.entries(vars).forEach(([key, value]) => {
      if (lastCssVars.value[key] !== value) {
        document.documentElement.style.setProperty(key, value)
        lastCssVars.value[key] = value
      }
    })
    
    // 清理不再存在的变量
    Object.keys(lastCssVars.value).forEach(key => {
      if (!(key in vars)) {
        document.documentElement.style.removeProperty(key)
        delete lastCssVars.value[key]
      }
    })
  })
}, { immediate: true, flush: 'post' })

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
/* 侧边栏基础样式 */
.admin-sidebar {
  background-color: var(--color-bg-card) !important;
  border-right: 1px solid var(--color-border-subtle);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  box-shadow: var(--shadow-lg);
  color: var(--color-text-main) !important;
}

.admin-sidebar * {
  color: inherit;
}

.admin-sidebar-header {
  color: var(--color-text-main) !important;
  border-color: var(--color-border-subtle) !important;
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.02));
}

.admin-sidebar-header span {
  color: var(--color-text-main) !important;
  font-weight: 700;
  letter-spacing: 0.05em;
}

/* 侧边栏链接样式 */
.admin-sidebar-link {
  color: var(--color-text-muted) !important;
  background-color: transparent;
  cursor: pointer;
  text-decoration: none;
  display: flex;
  align-items: center;
  position: relative;
  z-index: 1;
  font-size: 0.875rem;
  line-height: 1.5;
  min-height: 2.25rem;
  margin: 0.25rem 0.5rem;
  border-radius: var(--radius-md);
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.admin-sidebar-link span {
  color: inherit !important;
  font-weight: 500;
}

.admin-sidebar-link i {
  color: var(--color-text-muted) !important;
  opacity: 0.7;
  transition: all 0.2s;
}

.admin-sidebar-link:hover {
  background-color: var(--color-bg-elevated) !important;
  color: var(--color-text-main) !important;
  transform: translateX(4px);
}

.admin-sidebar-link:hover i {
  color: var(--color-primary-hover) !important;
  opacity: 1;
}

.admin-sidebar-link-active {
  background: linear-gradient(90deg, var(--color-primary-soft), transparent) !important;
  color: var(--color-primary) !important;
  font-weight: 600;
  border-left: 3px solid var(--color-primary);
  border-radius: 0 var(--radius-md) var(--radius-md) 0;
  margin-left: 0; 
  padding-left: calc(1rem + 3px); /* Compensate for margin removal */
}

.admin-sidebar-link-active i {
  color: var(--color-primary) !important;
  opacity: 1;
}

.admin-sidebar-link-active span {
  color: var(--color-primary) !important;
}

/* 菜单组标题样式 */
.menu-group-title {
  color: var(--color-text-muted) !important;
  font-weight: 600;
  font-size: 0.75rem;
  opacity: 0.8;
}

.menu-group-header {
  color: var(--color-text-main) !important;
  padding: 0.5rem 1rem;
  margin-top: 0.5rem;
  border-radius: var(--radius-sm);
  transition: background-color 0.2s;
}

.menu-group-header:hover {
  background-color: var(--color-bg-elevated);
}

.menu-group-header:hover .menu-group-title {
  color: var(--color-text-main) !important;
  opacity: 1;
}

.menu-group-active .menu-group-title {
  color: var(--color-text-main) !important;
  opacity: 1;
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
  scrollbar-width: thin;
  scrollbar-color: var(--color-border-default) transparent;
}

/* 主内容区背景 - 根据主题自动适配 */
.admin-main,
.admin-main-fallback {
  background: var(--n-body-color, var(--color-bg-body)) !important;
  color: var(--color-text-main) !important;
  position: relative;
}

/* 噪点纹理（可选，性能可接受前提下） */
.admin-main::before {
  content: '';
  position: fixed;
  inset: 0;
  background-image: 
    radial-gradient(circle at 20% 30%, var(--color-primary-soft, rgba(59, 130, 246, 0.02)) 0%, transparent 30%),
    radial-gradient(circle at 80% 70%, var(--color-primary-soft, rgba(96, 165, 250, 0.015)) 0%, transparent 30%);
  pointer-events: none;
  z-index: 0;
  opacity: 0.6;
}

/* 覆盖 Admin 内的 Naive UI 全局样式，使其更贴合后台风格 */
.admin-main {
  /* 可以在这里微调 Admin 专属的变量覆盖 */
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

/* 确保后台布局不影响前端导航 */
.admin-layout {
  position: relative;
  z-index: 1;
}

.admin-sidebar {
  z-index: 50;
}

.admin-main {
  position: relative;
  z-index: 1;
  max-width: 100%;
  overflow-x: hidden; /* 防止水平滚动 */
  box-sizing: border-box;
}

/* 响应式：小屏幕时减少左边距 */
@media (max-width: 768px) {
  .admin-main {
    margin-left: 0 !important;
    padding: 1rem !important;
  }
}

/* 确保卡片可见 - 使用 CSS 变量，自动适配主题 */
.admin-main :deep(.card),
.admin-main :deep(.page-container),
.admin-main :deep(.app-card) {
  background: var(--color-bg-elevated, var(--color-bg-card)) !important;
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.card-hover) {
  background: var(--color-bg-elevated, var(--color-bg-card));
  border: 1px solid var(--color-border-subtle);
}

.admin-main :deep(.card-hover:hover) {
  background: var(--color-bg-elevated);
  border-color: var(--color-border-default);
}

/* 确保主题类正常工作 - 使用 CSS 变量，自动适配主题 */
.admin-main :deep(.text-text-main) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.text-text-muted) {
  color: var(--color-text-muted) !important;
}

.admin-main :deep(.text-text-disabled) {
  color: var(--color-text-disabled) !important;
}

/* 文字颜色调整 - 使用 CSS 变量 */
.admin-main :deep(.page-title),
.admin-main :deep(h1),
.admin-main :deep(h2),
.admin-main :deep(h3) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.text-gray-600),
.admin-main :deep(.text-gray-500) {
  color: var(--color-text-muted) !important;
}

.admin-main :deep(.text-gray-800) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.text-gray-900) {
  color: var(--color-text-main) !important;
}

/* 输入框和表单元素样式 - 使用 CSS 变量，自动适配主题 */
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
  background: var(--color-bg-elevated, var(--color-bg-card)) !important;
  border-color: var(--color-border-default) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.input::placeholder),
.admin-main :deep(.form-input::placeholder),
.admin-main :deep(input::placeholder),
.admin-main :deep(textarea::placeholder) {
  color: var(--color-text-muted) !important;
}

/* 按钮样式 - 使用 CSS 变量，自动适配主题 */
.admin-main :deep(.btn-secondary) {
  background: var(--color-bg-elevated, var(--color-bg-card));
  border-color: var(--color-border-default);
  color: var(--color-text-main);
}

.admin-main :deep(.btn-secondary:hover) {
  background: var(--color-bg-elevated);
  border-color: var(--color-border-strong);
}

/* 表格样式 - 使用 CSS 变量，自动适配主题 */
.admin-main :deep(.table),
.admin-main :deep(table) {
  color: var(--color-text-main);
}

.admin-main :deep(.table-header),
.admin-main :deep(thead) {
  background: var(--color-bg-elevated, var(--color-bg-card));
}

.admin-main :deep(.table-header-cell),
.admin-main :deep(th) {
  color: var(--color-text-main);
  border-color: var(--color-border-subtle);
}

.admin-main :deep(.table-cell),
.admin-main :deep(td) {
  color: var(--color-text-main);
  border-color: var(--color-border-subtle);
}

.admin-main :deep(.table-row:hover),
.admin-main :deep(tr:hover) {
  background: var(--color-bg-elevated);
}

/* 链接样式 - 使用 CSS 变量 */
.admin-main :deep(.btn-link),
.admin-main :deep(a) {
  color: var(--color-text-main, rgba(255, 255, 255, 0.9));
}

.admin-main :deep(.btn-link--blue),
.admin-main :deep(.btn-link:hover) {
  color: var(--color-primary, #60a5fa);
}

.admin-main :deep(.btn-link--red) {
  color: var(--color-error, #f87171);
}

/* 标签和徽章 - 使用 CSS 变量 */
.admin-main :deep(.badge) {
  color: var(--color-text-main, #ffffff);
}

/* 模态框样式 - 使用 CSS 变量 */
.admin-main :deep(.modal-overlay) {
  background: var(--color-overlay, rgba(0, 0, 0, 0.8));
}

.admin-main :deep(.modal-content),
.admin-main :deep(.modal-content-lg) {
  background: var(--color-bg-card, rgba(30, 41, 59, 0.95));
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle, rgba(255, 255, 255, 0.1));
}

.admin-main :deep(.modal-header),
.admin-main :deep(.modal-title) {
  color: var(--color-text-main, #ffffff);
}

.admin-main :deep(.modal-body) {
  color: var(--color-text-main, rgba(255, 255, 255, 0.9));
}

/* 所有文本颜色统一 - 使用 CSS 变量 */
.admin-main :deep(.text-gray-600),
.admin-main :deep(.text-gray-500),
.admin-main :deep(.text-gray-400) {
  color: var(--color-text-muted, rgba(255, 255, 255, 0.7)) !important;
}

.admin-main :deep(.text-gray-700) {
  color: var(--color-text-sub, rgba(255, 255, 255, 0.8)) !important;
}

.admin-main :deep(.text-gray-800),
.admin-main :deep(.text-gray-900) {
  color: var(--color-text-main, #ffffff) !important;
}

/* 白色背景的卡片 - 使用 CSS 变量，自动适配主题 */
.admin-main :deep(.bg-white),
.admin-main :deep(.bg-white\/\*),
.admin-main :deep([class*="bg-white"]) {
  background: var(--color-bg-card) !important;
}

/* 深色模式下的背景 - 使用 CSS 变量 */
.admin-main :deep(.dark\:bg-gray-800),
.admin-main :deep(.dark\:bg-gray-900) {
  background: var(--color-bg-elevated) !important;
}

/* 边框颜色调整 - 使用 CSS 变量 */
.admin-main :deep(.border-gray-200),
.admin-main :deep(.border-gray-300) {
  border-color: var(--color-border-subtle) !important;
}

.admin-main :deep(.border-gray-700) {
  border-color: var(--color-border-default) !important;
}

/* 分割线 - 使用 CSS 变量 */
.admin-main :deep(.divide-gray-200),
.admin-main :deep(.divide-gray-300) {
  border-color: var(--color-border-subtle) !important;
}

.admin-main :deep(.divide-gray-700) {
  border-color: var(--color-border-default) !important;
}

/* 悬停效果 - 使用 CSS 变量 */
.admin-main :deep(.hover\:bg-gray-50:hover) {
  background: var(--color-bg-elevated) !important;
}

/* 加载和空状态 - 使用 CSS 变量，自动适配主题 */
.admin-main :deep(.loading),
.admin-main :deep(.empty-state) {
  color: var(--color-text-muted);
}

/* 统计卡片文字 - 使用 CSS 变量 */
.admin-main :deep(.stat-label),
.admin-main :deep(.stat-value) {
  color: var(--color-text-main);
}

/* 所有 h1-h6 标题 - 使用 CSS 变量 */
.admin-main :deep(h1),
.admin-main :deep(h2),
.admin-main :deep(h3),
.admin-main :deep(h4),
.admin-main :deep(h5),
.admin-main :deep(h6) {
  color: var(--color-text-main);
}

/* 段落文字 - 使用 CSS 变量 */
.admin-main :deep(p) {
  color: var(--color-text-main);
}

/* 列表文字 - 使用 CSS 变量 */
.admin-main :deep(li) {
  color: var(--color-text-main);
}

/* 标签文字 - 使用 CSS 变量 */
.admin-main :deep(label) {
  color: var(--color-text-main);
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

/* 筛选栏 - 使用 CSS 变量，自动适配主题 */
.admin-main :deep(.filter-bar) {
  background: var(--color-bg-elevated, var(--color-bg-card));
  border-color: var(--color-border-subtle);
}

/* Naive UI 组件样式 - 使用 CSS 变量，自动适配主题 */
.admin-main :deep(.n-card) {
  background: var(--color-bg-card) !important;
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-card .n-card-header) {
  color: var(--color-text-main) !important;
  border-bottom-color: var(--color-border-subtle) !important;
}

.admin-main :deep(.n-card .n-card-body) {
  color: var(--color-text-main) !important;
}

/* Naive UI 数据表格 - 使用 CSS 变量 */
.admin-main :deep(.n-data-table) {
  background: transparent !important;
}

.admin-main :deep(.n-data-table .n-data-table-thead) {
  background: var(--color-bg-elevated) !important;
}

.admin-main :deep(.n-data-table .n-data-table-thead th) {
  background: var(--color-bg-elevated) !important;
  color: var(--color-text-main) !important;
  border-color: var(--color-border-subtle) !important;
}

.admin-main :deep(.n-data-table .n-data-table-tbody td) {
  background: transparent !important;
  color: var(--color-text-main) !important;
  border-color: var(--color-border-subtle) !important;
}

.admin-main :deep(.n-data-table .n-data-table-tbody tr:hover td) {
  background: var(--color-bg-elevated) !important;
}

.admin-main :deep(.n-data-table .n-data-table-tbody tr:nth-child(even) td) {
  background: var(--color-bg-elevated);
  opacity: 0.5;
}

/* Naive UI 输入框 - 使用 CSS 变量 */
.admin-main :deep(.n-input) {
  background: var(--color-bg-elevated, var(--color-bg-card)) !important;
  border-color: var(--color-border-default) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-input .n-input__input-el) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-input .n-input__placeholder) {
  color: var(--color-text-muted) !important;
}

/* Naive UI 按钮 - 使用 CSS 变量 */
.admin-main :deep(.n-button) {
  border-color: var(--color-border-default) !important;
}

.admin-main :deep(.n-button--default-type) {
  background: var(--color-bg-elevated, var(--color-bg-card)) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-button--default-type:hover) {
  background: var(--color-bg-elevated) !important;
}

/* Naive UI 标签 - 使用 CSS 变量 */
.admin-main :deep(.n-tag) {
  background: var(--color-bg-elevated) !important;
  border-color: var(--color-border-default) !important;
  color: var(--color-text-main) !important;
}

/* Naive UI 分页 - 使用 CSS 变量 */
.admin-main :deep(.n-pagination) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-pagination .n-pagination-item) {
  background: var(--color-bg-elevated) !important;
  border-color: var(--color-border-default) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-pagination .n-pagination-item:hover) {
  background: var(--color-bg-elevated) !important;
}

.admin-main :deep(.n-pagination .n-pagination-item--active) {
  background: var(--color-primary-soft) !important;
  border-color: var(--color-primary) !important;
  color: var(--color-primary) !important;
}

/* Naive UI 模态框 - 使用 CSS 变量 */
.admin-main :deep(.n-modal) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-modal .n-card) {
  background: var(--color-bg-card) !important;
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle) !important;
}

/* Naive UI 表单 - 使用 CSS 变量 */
.admin-main :deep(.n-form-item-label) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-form-item-feedback-wrapper) {
  color: var(--color-text-muted) !important;
}

/* Naive UI 表格内嵌组件样式统一 - 使用 CSS 变量 */
/* 表格内的按钮 - quaternary 类型 */
.admin-main :deep(.n-data-table .n-button--quaternary) {
  background: transparent !important;
  border: none !important;
  color: var(--color-text-main) !important;
  transition: all 0.2s ease !important;
}

.admin-main :deep(.n-data-table .n-button--quaternary:hover) {
  background: var(--color-bg-elevated) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-data-table .n-button--primary-type.n-button--quaternary) {
  color: var(--color-primary) !important;
}

.admin-main :deep(.n-data-table .n-button--primary-type.n-button--quaternary:hover) {
  background: var(--color-primary-soft) !important;
  color: var(--color-primary-hover) !important;
}

.admin-main :deep(.n-data-table .n-button--error-type.n-button--quaternary) {
  color: var(--color-error, #f87171) !important;
}

.admin-main :deep(.n-data-table .n-button--error-type.n-button--quaternary:hover) {
  background: var(--color-error);
  opacity: 0.1;
  color: var(--color-error) !important;
}

.admin-main :deep(.n-data-table .n-button--success-type.n-button--quaternary) {
  color: var(--color-success, #34d399) !important;
}

.admin-main :deep(.n-data-table .n-button--success-type.n-button--quaternary:hover) {
  background: var(--color-success);
  opacity: 0.1;
  color: var(--color-success) !important;
}

/* 表格内的标签样式 - 使用 CSS 变量 */
.admin-main :deep(.n-data-table .n-tag) {
  background: var(--color-bg-elevated) !important;
  border: 1px solid var(--color-border-default) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-data-table .n-tag--info) {
  background: var(--color-primary-soft) !important;
  border-color: var(--color-primary) !important;
  color: var(--color-primary) !important;
}

.admin-main :deep(.n-data-table .n-tag--success) {
  background: var(--color-success);
  opacity: 0.1;
  border-color: var(--color-success) !important;
  color: var(--color-success) !important;
}

.admin-main :deep(.n-data-table .n-tag--warning) {
  background: var(--color-warning, rgba(251, 191, 36, 0.2));
  border-color: var(--color-warning, rgba(251, 191, 36, 0.4)) !important;
  color: var(--color-warning, #fcd34d) !important;
}

.admin-main :deep(.n-data-table .n-tag--error) {
  background: var(--color-error);
  opacity: 0.1;
  border-color: var(--color-error) !important;
  color: var(--color-error) !important;
}

/* 表格内的头像 - 使用 CSS 变量 */
.admin-main :deep(.n-data-table .n-avatar) {
  border: 2px solid var(--color-border-subtle) !important;
}

/* 表格内的链接 - 使用 CSS 变量 */
.admin-main :deep(.n-data-table a) {
  color: var(--color-primary) !important;
  text-decoration: none !important;
  transition: color 0.2s ease !important;
}

.admin-main :deep(.n-data-table a:hover) {
  color: var(--color-primary-hover) !important;
}

/* Popconfirm - 使用 CSS 变量 */
.admin-main :deep(.n-popover) {
  background: var(--color-bg-card) !important;
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-popover .n-popover__content) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-popover .n-button) {
  background: var(--color-bg-elevated) !important;
  border-color: var(--color-border-default) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-popover .n-button:hover) {
  background: var(--color-bg-elevated) !important;
}

.admin-main :deep(.n-popover .n-button--primary-type) {
  background: var(--color-primary-soft) !important;
  border-color: var(--color-primary) !important;
  color: var(--color-primary) !important;
}

.admin-main :deep(.n-popover .n-button--primary-type:hover) {
  background: var(--color-primary-soft) !important;
  opacity: 0.8;
}

/* 表格内的自定义元素 - 使用 CSS 变量 */
.admin-main :deep(.n-data-table .project-info),
.admin-main :deep(.n-data-table .project-details),
.admin-main :deep(.n-data-table .project-title),
.admin-main :deep(.n-data-table .project-desc),
.admin-main :deep(.n-data-table .view-count),
.admin-main :deep(.n-data-table .action-buttons) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-data-table .project-desc) {
  color: var(--color-text-muted) !important;
}

.admin-main :deep(.n-data-table .view-count) {
  color: var(--color-text-muted) !important;
}

/* 表格内的图标颜色 - 使用 CSS 变量 */
.admin-main :deep(.n-data-table .fas),
.admin-main :deep(.n-data-table .fa) {
  color: var(--color-text-muted) !important;
}

/* 表格内的操作按钮容器 */
.admin-main :deep(.n-data-table .action-buttons) {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

/* 确保表格内所有文字都是可见的 - 使用 CSS 变量 */
.admin-main :deep(.n-data-table td) {
  color: var(--color-text-main) !important;
}

.admin-main :deep(.n-data-table td .n-tag) {
  color: inherit !important;
}

/* 页面头部按钮统一样式 - 使用 CSS 变量 */
.admin-main :deep(.page-header .n-button),
.admin-main :deep(.header-actions .n-button) {
  border-color: var(--color-border-default) !important;
}

.admin-main :deep(.page-header .n-button--primary-type) {
  background: var(--color-primary-soft) !important;
  border-color: var(--color-primary) !important;
  color: var(--color-primary) !important;
}

.admin-main :deep(.page-header .n-button--primary-type:hover) {
  background: var(--color-primary-soft) !important;
  opacity: 0.8;
  border-color: var(--color-primary-hover) !important;
}

.admin-main :deep(.page-header .n-button--success-type) {
  background: var(--color-success);
  opacity: 0.1;
  border-color: var(--color-success) !important;
  color: var(--color-success) !important;
}

.admin-main :deep(.page-header .n-button--success-type:hover) {
  background: var(--color-success);
  opacity: 0.2;
  border-color: var(--color-success) !important;
}

.admin-main :deep(.page-header .n-button--secondary) {
  background: var(--color-bg-elevated) !important;
  border-color: var(--color-border-default) !important;
  color: var(--color-text-main) !important;
}

.admin-main :deep(.page-header .n-button--secondary:hover) {
  background: var(--color-bg-elevated) !important;
}

/* 筛选栏统一样式 - 使用 CSS 变量 */
.admin-main :deep(.filter-bar) {
  background: var(--color-bg-elevated, var(--color-bg-card)) !important;
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle) !important;
  border-radius: 0.5rem;
  padding: 1rem;
  margin-bottom: 1.5rem;
}

/* 页面标题统一样式 - 使用 CSS 变量 */
.admin-main :deep(.page-title) {
  color: var(--color-text-main) !important;
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

/* 统一加载状态样式 - 使用 CSS 变量 */
.admin-main :deep(.n-spin) {
  color: var(--color-text-muted) !important;
}

/* 统一空状态样式 - 使用 CSS 变量 */
.admin-main :deep(.n-empty) {
  color: var(--color-text-muted) !important;
}

.admin-main :deep(.n-empty .n-empty__description) {
  color: var(--color-text-muted) !important;
}

/* 确保所有页面容器背景透明，由主题变量控制 */
.admin-main > * {
  background: transparent;
}

/* 覆盖可能的白色背景 */
.admin-main :deep(div[style*="background"]),
.admin-main :deep(div[style*="background-color"]) {
  /* 不强制覆盖内联样式，但确保默认背景透明 */
}

/* 设置子菜单样式 - 使用 CSS 变量 */
.admin-sidebar .ml-4 {
  margin-left: 1rem;
  border-left: 2px solid var(--color-border-subtle);
  padding-left: 0.5rem;
}

.admin-sidebar .ml-4 .admin-sidebar-link {
  font-size: 0.875rem;
  padding-left: 0.75rem;
}

/* 折叠菜单组样式 */
.menu-group {
  margin-top: 0.5rem;
}

.menu-group-header {
  width: 100%;
  display: flex;
  align-items: center;
  padding: 0.625rem 1rem;
  text-align: left;
  background: transparent;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
  border-radius: 0.5rem;
  color: var(--color-text-main) !important;
  margin-bottom: 0.25rem;
}

.menu-group-header:hover {
  background-color: var(--color-bg-elevated) !important;
}

.menu-group-header .fa-chevron-right {
  font-size: 0.625rem;
  width: 0.875rem;
  transition: transform 0.2s ease;
  opacity: 0.6;
  color: var(--color-text-muted) !important;
}

.menu-group-header:hover .fa-chevron-right {
  opacity: 0.8;
  color: var(--color-text-main) !important;
}

.menu-group-header .fa-chevron-right.rotate-90 {
  transform: rotate(90deg);
}

.menu-group-active {
  background-color: var(--color-bg-elevated) !important;
}

.menu-group-active .fa-chevron-right {
  opacity: 1;
  color: var(--color-text-main) !important;
}

.menu-group-items {
  margin-top: 0.25rem;
  margin-left: 0.75rem;
  padding-left: 0.75rem;
  border-left: 2px solid var(--color-border-subtle);
  animation: slideDown 0.2s ease;
  padding-bottom: 0.25rem;
}

.menu-group-items .admin-sidebar-link {
  color: var(--color-text-main) !important;
  font-weight: 400;
}

.menu-group-items .admin-sidebar-link:hover {
  color: var(--color-text-main) !important;
}

.menu-group-items .admin-sidebar-link i {
  color: var(--color-text-muted) !important;
}

.menu-group-items .admin-sidebar-link:hover i,
.menu-group-items .admin-sidebar-link-active i {
  color: inherit !important;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>

