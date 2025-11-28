<template>
  <div
    v-if="showPrompt && !isInstalled"
    class="fixed bottom-4 left-4 right-4 md:left-auto md:right-4 md:w-96 z-50 animate-slide-up"
  >
    <div class="bg-white dark:bg-gray-800 rounded-2xl shadow-2xl border border-gray-200 dark:border-gray-700 p-4">
      <div class="flex items-start justify-between">
        <div class="flex-1">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-1">
            {{ isIOS ? '添加到主屏幕' : '安装应用' }}
          </h3>
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">
            {{ isIOS 
              ? '在 Safari 中点击分享按钮，选择"添加到主屏幕"以获得更好的体验' 
              : '点击浏览器菜单，选择"添加到主屏幕"或"安装应用"' 
            }}
          </p>
          <div class="flex gap-2">
            <button
              @click="dismissPrompt"
              class="flex-1 px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors text-sm font-medium"
            >
              稍后
            </button>
            <button
              @click="dismissPromptPermanently"
              class="flex-1 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors text-sm font-medium"
            >
              知道了
            </button>
          </div>
        </div>
        <button
          @click="dismissPrompt"
          class="ml-4 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
          aria-label="关闭"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const showPrompt = ref(false)
const isIOS = ref(false)
const isInstalled = ref(false)
const isMobile = ref(false)

// 检测是否移动端
const checkMobile = () => {
  if (process.client) {
    const width = window.innerWidth
    isMobile.value = width < 768 // 小于 768px 认为是移动端
  }
}

// 检测是否已安装（从主屏幕打开）
const checkInstalled = () => {
  if (process.client) {
    // iOS Safari
    if ((window.navigator as any).standalone === true) {
      isInstalled.value = true
      return
    }
    
    // Android Chrome / 其他 PWA
    if (window.matchMedia('(display-mode: standalone)').matches) {
      isInstalled.value = true
      return
    }
    
    // 检查是否在浏览器中
    isInstalled.value = false
  }
}

// 检测 iOS
const checkIOS = () => {
  if (process.client) {
    const userAgent = window.navigator.userAgent.toLowerCase()
    isIOS.value = /iphone|ipad|ipod/.test(userAgent)
  }
}

// 检查是否应该显示提示
const shouldShowPrompt = () => {
  if (process.client) {
    // 只在移动端显示
    if (!isMobile.value) {
      return false
    }
    
    // 检查是否已永久关闭
    const dismissed = localStorage.getItem('add-to-home-dismissed')
    if (dismissed === 'permanent') {
      return false
    }
    
    // 检查是否已安装
    if (isInstalled.value) {
      return false
    }
    
    // 检查是否已临时关闭（24小时内不显示）
    const dismissedTime = localStorage.getItem('add-to-home-dismissed-time')
    if (dismissedTime) {
      const now = Date.now()
      const dismissed = parseInt(dismissedTime)
      const hoursPassed = (now - dismissed) / (1000 * 60 * 60)
      if (hoursPassed < 24) {
        return false
      }
    }
    
    return true
  }
  return false
}

// 关闭提示（临时）
const dismissPrompt = () => {
  showPrompt.value = false
  if (process.client) {
    localStorage.setItem('add-to-home-dismissed-time', Date.now().toString())
  }
}

// 永久关闭提示
const dismissPromptPermanently = () => {
  showPrompt.value = false
  if (process.client) {
    localStorage.setItem('add-to-home-dismissed', 'permanent')
  }
}

onMounted(() => {
  checkMobile()
  checkIOS()
  checkInstalled()
  
  // 监听窗口大小变化（响应式）
  const handleResize = () => {
    checkMobile()
    // 如果从桌面切换到移动端，检查是否显示提示
    if (isMobile.value && shouldShowPrompt()) {
      showPrompt.value = true
    } else if (!isMobile.value) {
      showPrompt.value = false
    }
  }
  
  window.addEventListener('resize', handleResize)
  
  // 延迟显示，避免干扰首次加载
  setTimeout(() => {
    if (shouldShowPrompt()) {
      showPrompt.value = true
    }
  }, 3000) // 3秒后显示
  
  onUnmounted(() => {
    window.removeEventListener('resize', handleResize)
  })
})
</script>

<style scoped>
@keyframes slide-up {
  from {
    transform: translateY(100%);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.animate-slide-up {
  animation: slide-up 0.3s ease-out;
}
</style>

