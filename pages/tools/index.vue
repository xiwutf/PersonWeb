<template>
  <div class="tools-page">
    <!-- 全局背景噪点 -->
    <div class="tools-background-noise"></div>

    <!-- 动态背景光斑 -->
    <div class="tools-background-container">
      <div class="tools-background-blob tools-background-blob--orange"></div>
      <div class="tools-background-blob tools-background-blob--red"></div>
      <div class="tools-background-blob tools-background-blob--amber"></div>
    </div>

    <div class="tools-content">
      <!-- 页面头部 -->
      <header class="tools-header">
        <div class="tools-header-icon">
          <span>🔧</span>
        </div>
        <h1 class="tools-title">插件工具</h1>
        <p class="tools-subtitle">
          专业的Revit插件和自适应族，用科技提升工作效率，让复杂的设计变得简单
        </p>
      </header>

      <!-- 统计信息 -->
      <div class="tools-stats-grid">
        <div class="tools-stat-card">
          <div class="tools-stat-value tools-stat-value--orange">
            {{ tools.length || 0 }}+
          </div>
          <div class="tools-stat-label">专业工具</div>
        </div>
        <div class="tools-stat-card">
          <div class="tools-stat-value tools-stat-value--red">
            1000+
          </div>
          <div class="tools-stat-label">用户选择</div>
        </div>
        <div class="tools-stat-card">
          <div class="tools-stat-value tools-stat-value--green">
            99%
          </div>
          <div class="tools-stat-label">好评率</div>
        </div>
      </div>

      <!-- 工具列表 -->
      <div v-if="pending" class="tools-loading">
        <div class="tools-loading-spinner"></div>
        <p class="tools-loading-text">加载工具箱中...</p>
      </div>

      <div v-else-if="error" class="tools-error">
        <div class="tools-error-icon">😵</div>
        <h3 class="tools-error-title">加载失败</h3>
        <p class="tools-error-message">{{ error }}</p>
      </div>

      <div v-else class="tools-grid">
        <TransitionGroup name="tools-list">
          <div
            v-for="(tool, index) in tools"
            :key="tool._path"
            class="tools-card"
            :style="{ transitionDelay: `${index * 50}ms` }"
          >
            <!-- 卡片头部 -->
            <div class="tools-card-header">
              <div class="tools-card-top">
                <div class="tools-card-icon">
                  🔧
                </div>
                <div class="tools-card-price">
                  <div class="tools-card-price-current">¥{{ tool.price }}</div>
                  <div class="tools-card-price-original">¥{{ Math.round(tool.price * 1.5) }}</div>
                </div>
              </div>

              <h3 class="tools-card-title">
                {{ tool.title }}
              </h3>
              
              <p class="tools-card-description">
                {{ tool.description }}
              </p>
              
              <!-- 标签 -->
              <div class="tools-card-tags">
                <span
                  v-for="tag in tool.tags"
                  :key="tag"
                  class="tools-card-tag"
                >
                  {{ tag }}
                </span>
              </div>
            </div>

            <!-- 底部按钮 -->
            <div class="tools-card-footer">
              <div class="tools-card-actions">
                <NuxtLink
                  :to="`/tools/detail-${tool.slug || tool._path.split('/').pop()}`"
                  class="tools-card-button tools-card-button--secondary"
                >
                  查看详情
                </NuxtLink>
                <a
                  :href="tool.buy_link"
                  target="_blank"
                  class="tools-card-button tools-card-button--primary"
                >
                  <span>立即购买</span>
                  <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path>
                  </svg>
                </a>
              </div>
            </div>
          </div>
        </TransitionGroup>
      </div>

      <!-- 快速导航 -->
      <div class="tools-navigation">
        <NuxtLink
          to="/tools/marketplace"
          class="tools-nav-button tools-nav-button--primary"
        >
          🛒 工具商城
        </NuxtLink>
        <NuxtLink
          to="/tools/collections"
          class="tools-nav-button tools-nav-button--secondary"
        >
          📦 工具合集
        </NuxtLink>
        <NuxtLink
          to="/tools/my-tools"
          class="tools-nav-button tools-nav-button--secondary"
        >
          🎒 我的工具
        </NuxtLink>
      </div>

      <!-- 底部CTA -->
      <div class="tools-cta">
        <div class="tools-cta-container">
          <div class="tools-cta-overlay"></div>
          
          <div class="tools-cta-content">
            <h3 class="tools-cta-title">需要定制化插件？</h3>
            <p class="tools-cta-description">我们提供专业的Revit插件定制开发服务，为您量身打造提升效率的专属工具。</p>
            <a
              href="mailto:contact@溪午听风.com"
              class="tools-cta-button"
            >
              <span>📧</span>
              联系定制
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const api = useApi()
const tools = ref<any[]>([])
const pending = ref(true)
const error = ref<string | null>(null)

// 从API获取工具数据
const fetchTools = async () => {
  try {
    pending.value = true
    error.value = null
    
    // 优先从API获取
    const res = await api.get<any[]>('/MockData/tools')
    if (res && res.length > 0) {
      tools.value = res
      return
    }
    
    // 如果API没有数据，尝试从 @nuxt/content 获取
    const { data: contentTools } = await useAsyncData('tools', () =>
      queryContent('/tools').sort({ date: -1 }).find()
    )
    
    if (contentTools.value && Array.isArray(contentTools.value) && contentTools.value.length > 0) {
      tools.value = contentTools.value
    } else {
      tools.value = []
    }
  } catch (e: any) {
    console.error('Failed to fetch tools:', e)
    error.value = e.message || '加载失败'
    tools.value = []
  } finally {
    pending.value = false
  }
}

onMounted(() => {
  fetchTools()
})

useHead({
  title: '插件工具 - 溪午听风',
  meta: [
    { name: 'description', content: '专业的Revit插件和自适应族' }
  ]
})
</script>

<style scoped>
/* 页面特有样式已移至 assets/css/tools.css */
/* 这里只保留组件特有的样式（如果有） */
</style>
