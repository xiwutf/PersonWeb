<template>
  <!-- 隐秘的后台入口 - 完全透明，位于左下角 -->
  <div
    ref="secretAreaRef"
    class="fixed bottom-0 left-0 w-20 h-20 opacity-0 cursor-pointer z-50"
    @click="handleSecretClick"
    @dblclick="handleDoubleClick"
    title=""
  ></div>
</template>

<script setup lang="ts">
const router = useRouter()
const secretAreaRef = ref<HTMLDivElement | null>(null)

let clickCount = 0
let clickTimer: NodeJS.Timeout | null = null
const SECRET_CLICKS = 5 // 需要点击5次
const SECRET_TIMEOUT = 2000 // 2秒内完成

const handleSecretClick = () => {
  clickCount++
  
  // 清除之前的定时器
  if (clickTimer) {
    clearTimeout(clickTimer)
  }
  
  // 如果达到指定次数，跳转到后台
  if (clickCount >= SECRET_CLICKS) {
    router.push('/admin/login')
    clickCount = 0
    return
  }
  
  // 设置定时器，超时重置计数
  clickTimer = setTimeout(() => {
    clickCount = 0
  }, SECRET_TIMEOUT)
}

const handleDoubleClick = () => {
  // 双击也可以快速进入（备用方式）
  clickCount = SECRET_CLICKS
  handleSecretClick()
}

// 键盘快捷键：Ctrl+Shift+A
onMounted(() => {
  const handleKeyPress = (e: KeyboardEvent) => {
    if (e.ctrlKey && e.shiftKey && e.key === 'A') {
      e.preventDefault()
      router.push('/admin/login')
    }
  }
  
  window.addEventListener('keydown', handleKeyPress)
  
  onUnmounted(() => {
    window.removeEventListener('keydown', handleKeyPress)
  })
})
</script>

