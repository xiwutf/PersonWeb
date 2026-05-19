<template>
  <section class="hero-super">
    <!-- 背景层 -->
    <div class="hero-background">
      <!-- 深色渐变背景 -->
      <div class="hero-background-bg"></div>
      
      <!-- 动态光效网格 -->
      <div class="hero-background-grid">
        <div class="hero-background-grid-pattern"></div>
      </div>
      
      <!-- 角落光晕 -->
      <div class="hero-glow-cyan"></div>
      <div class="hero-glow-purple"></div>
      
      <!-- 动态粒子效果（可选） -->
      <div class="hero-particle-overlay"></div>
    </div>

    <!-- 主内容 -->
    <div class="hero-content">
      <div class="hero-grid">
        <!-- 左侧文案区 -->
        <div class="hero-left-content hero-left-spacing" ref="leftContentRef">
          <!-- 欢迎标签 -->
          <div class="hero-welcome-badge">
            <span class="hero-welcome-indicator">
              <span class="hero-welcome-indicator-ping"></span>
              <span class="hero-welcome-indicator-dot"></span>
            </span>
            <span class="hero-welcome-text">欢迎来到 AI 创作空间</span>
          </div>

          <!-- 主标题 -->
          <h1 class="hero-title">
            <span class="hero-title-block">XIFG ·</span>
            <span class="hero-title-gradient">
              {{ heroTitle }}
            </span>
          </h1>

          <!-- 副标题 -->
          <p class="hero-subtitle">
            {{ heroSubtitle }}
          </p>

          <!-- 平台说明与个人标签（两列布局） -->
          <div class="hero-info-grid">
            <!-- 左侧：平台说明 -->
            <div class="hero-info-card">
              <h3 class="hero-info-card-title">
                <i class="fas fa-cube hero-info-card-title-icon"></i>
                平台定位
              </h3>
              <p class="hero-info-card-text">
                {{ platformDesc }}
              </p>
            </div>
            
            <!-- 右侧：个人标签 -->
            <div class="hero-info-card">
              <h3 class="hero-info-card-title">
                <i class="fas fa-user hero-info-card-title-icon"></i>
                个人标签
              </h3>
              <div class="hero-tags-container">
                <span
                  v-for="tag in personalTags"
                  :key="tag"
                  class="hero-tag"
                >
                  {{ tag }}
                </span>
              </div>
            </div>
          </div>

          <!-- 按钮区 -->
          <div class="hero-buttons">
            <NuxtLink to="/projects" class="hero-button-primary">
              <span class="hero-button-primary-content">浏览项目</span>
              <i class="fas fa-arrow-right hero-button-primary-icon"></i>
            </NuxtLink>

            <NuxtLink to="/about" class="hero-button-secondary">
              关于我
            </NuxtLink>
          </div>
        </div>

        <!-- 右侧视觉区 -->
        <div class="hero-right-content" ref="rightContentRef">
          <div class="hero-visual-card">
            <!-- 玻璃态卡片背景 -->
            <div class="hero-visual-card-bg"></div>
            
            <!-- 光效装饰 -->
            <div class="hero-visual-card-glow"></div>
            
            <!-- 内容 -->
            <div class="hero-visual-card-content">
              <!-- 头像 -->
              <div class="hero-avatar-container">
                <div class="hero-avatar-wrapper">
                  <div class="hero-avatar-glow"></div>
                  <div class="hero-avatar-image">
                    <img 
                      src="/images/avatar.jpg" 
                      alt="溪午听风"
                    />
                  </div>
                </div>
              </div>
              
              <!-- 信息卡片 -->
              <div class="hero-info-card-large">
                <h3 class="hero-info-card-large-title">溪午听风</h3>
                <p class="hero-info-card-large-subtitle">AI 工程师 · 系统构建者 · 技术创业者</p>
                <div class="hero-info-card-large-tags">
                  <span class="hero-info-card-large-tag">
                    RAG · Agent
                  </span>
                  <span class="hero-info-card-large-tag">
                    .NET · Python
                  </span>
                  <span class="hero-info-card-large-tag">
                    数字资产
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 滚动提示 -->
      <div
        class="hero-scroll-hint"
        @click="scrollToContent"
      >
        <div class="hero-scroll-hint-content">
          <span class="hero-scroll-hint-text">
            Scroll
          </span>
          <div class="hero-scroll-hint-indicator">
            <div class="hero-scroll-hint-dot"></div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { loadMotionOneDom } from '~/composables/useMotionOneDom'

const api = useApi()
const leftContentRef = ref<HTMLElement | null>(null)
const rightContentRef = ref<HTMLElement | null>(null)

// 站点配置
const siteConfig = ref<Record<string, string>>({})
const loading = ref(false)

// 从配置中获取内容
const heroTitle = computed(() => {
  return siteConfig.value.home_hero_title || '构建 AI 与软件系统的长期主义者'
})

const heroSubtitle = computed(() => {
  return siteConfig.value.home_hero_intro || '专注于将技术转化为可持续积累的数字资产。'
})

const platformDesc = computed(() => {
  return siteConfig.value.home_platform_desc || 'AI 系统 · 工业软件 · 个人数字系统 · 可复用模块'
})

const personalTags = computed(() => {
  const tagsStr = siteConfig.value.home_tags || 'AI 应用探索者 · 独立产品探索者 · Chrome 扩展开发者 · 产品化实践'
  return tagsStr.split(' · ').filter(t => t.trim())
})

// 获取站点配置
const fetchSiteConfig = async () => {
  try {
    loading.value = true
    const res = await api.get<Record<string, string>>('/Config')
    if (res && typeof res === 'object') {
      siteConfig.value = res
    }
  } catch (e) {
    console.error('获取站点配置失败:', e)
  } finally {
    loading.value = false
  }
}

// 角色轮播
const roles = ['AI 应用探索者', '独立产品探索者', 'Chrome 扩展开发者', '产品化实践']
const currentRoleIndex = ref(0)
let roleInterval: NodeJS.Timeout | null = null

const startRoleCarousel = () => {}

// 滚动到内容区
const scrollToContent = () => {
  const contentSection = document.getElementById('content')
  if (contentSection) {
    contentSection.scrollIntoView({ behavior: 'smooth' })
  }
}

const shouldSkipIntroAnimation = () => {
  if (!process.client) return false

  const reducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches
  const coarsePointer = window.matchMedia?.('(pointer: coarse)').matches
  const narrowScreen = window.innerWidth < 1024
  const saveData = navigator.connection?.saveData === true
  const lowMemory = typeof navigator.deviceMemory === 'number' && navigator.deviceMemory <= 4

  return Boolean(reducedMotion || coarsePointer || narrowScreen || saveData || lowMemory)
}

// 初始化动画
onMounted(async () => {
  // 先获取配置
  await fetchSiteConfig()
  
  if (shouldSkipIntroAnimation()) {
    return
  }

  const { animate, stagger } = await loadMotionOneDom()
  
  // 使用 Motion One 添加进入动画
  if (leftContentRef.value) {
    const children = leftContentRef.value.children
    animate(
      Array.from(children),
      { opacity: [0, 1], y: [30, 0] },
      { duration: 0.6, delay: stagger(0.1), easing: 'ease-out' }
    )
  }
  
  if (rightContentRef.value) {
    animate(
      rightContentRef.value,
      { opacity: [0, 1], x: [50, 0] },
      { duration: 0.8, delay: 0.3, easing: 'ease-out' }
    )
  }
})

onUnmounted(() => {
  if (roleInterval) {
    clearInterval(roleInterval)
  }
})
</script>

<style scoped>
/* 样式已移至 assets/css/hero.css */
/* 保留组件特有的样式（如果有） */
</style>

