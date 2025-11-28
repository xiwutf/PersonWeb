<template>
  <div class="min-h-screen flex flex-col relative">
    <!-- 动态背景效果（根据主题切换） -->
    <BackgroundEffects :effect="currentBackground" :config="backgroundConfig" />
    
    <!-- 鼠标轨迹特效 -->
    <MouseTrail />
    
    <!-- 风格切换面板 -->
    <ThemeSwitcher />
    
    <!-- 头部导航 -->
    <Header />
    
    <!-- 主要内容区域 -->
    <main class="flex-1 pt-24 relative z-10">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
    
    <!-- AI 智能助手 -->
    <AIAssistant />
    
    <!-- 时间胶囊墙 -->
    <TimeCapsuleWall />
    
    <!-- 隐秘的后台入口 -->
    <SecretAdminAccess />
  </div>
</template>

<script setup lang="ts">
// 导入组件（Nuxt 3 会自动导入 components 目录下的组件）

// 使用主题组合式函数
const { updateTheme } = useTheme()
const route = useRoute()
const api = useApi()

// 当前背景效果
const currentBackground = ref<string>('particles')
const backgroundConfig = ref<Record<string, any>>({})

// 监听主题和背景切换事件
onMounted(async () => {
  try {
    // 初始化主题
    updateTheme()

    // 获取或生成 Visitor ID
    let visitorId = localStorage.getItem('visitor_id')
    
    // 调用统计 API
    const res = await api.post('/tracking/visit', { 
      visitorId,
      path: route.path 
    })

    // 保存 Visitor ID (如果是新生成的)
    if (res && res.visitorId && res.visitorId !== visitorId) {
      localStorage.setItem('visitor_id', res.visitorId)
    }

    // 加载用户主题偏好
    try {
      const preference = await api.post('/Theme/preference', { visitorId: visitorId || 'anonymous' })
      if (preference) {
        currentBackground.value = preference.backgroundEffectCode || 'particles'
      }
    } catch (e) {
      console.warn('加载主题偏好失败', e)
    }
  } catch (e) {
    console.error('Failed to update stats', e)
  }

  // 监听背景切换事件
  window.addEventListener('background-changed', ((e: CustomEvent) => {
    currentBackground.value = e.detail.background
  }) as EventListener)
})
</script>
