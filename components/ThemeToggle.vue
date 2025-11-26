<template>
  <button
    @click="toggleTheme"
    class="p-2 rounded-xl transition-all duration-300 hover:bg-gray-100 dark:hover:bg-slate-800"
    :title="isDark ? '切换亮色模式' : '切换深色模式'"
  >
    <span v-if="isDark" class="text-yellow-400 text-lg">☀️</span>
    <span v-else class="text-slate-600 text-lg">🌙</span>
  </button>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const isDark = ref(false)

onMounted(() => {
  // 初始化主题状态
  if (localStorage.theme === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    isDark.value = true
    document.documentElement.classList.add('dark')
  } else {
    isDark.value = false
    document.documentElement.classList.remove('dark')
  }
})

const toggleTheme = () => {
  isDark.value = !isDark.value
  if (isDark.value) {
    document.documentElement.classList.add('dark')
    localStorage.theme = 'dark'
  } else {
    document.documentElement.classList.remove('dark')
    localStorage.theme = 'light'
  }
}
</script>
