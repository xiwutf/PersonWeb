<template>
  <div class="mt-12 pt-8 border-t border-gray-200">
    <div class="mb-6">
      <h3 class="text-2xl font-bold text-gray-800 mb-2">💬 评论讨论</h3>
      <p class="text-gray-600">欢迎分享你的想法和建议</p>
    </div>
    
    <!-- Giscus评论区域 -->
    <div
      ref="giscusContainer"
      class="bg-white rounded-xl p-6 shadow-sm border border-gray-100"
    >
      <div v-if="loading" class="text-center py-8">
        <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <p class="mt-2 text-gray-600">加载评论中...</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue'

const props = defineProps({
  // 页面标识符，用于区分不同页面的评论
  identifier: {
    type: String,
    required: true
  },
  // 页面标题
  title: {
    type: String,
    default: ''
  },
  // 主题色
  theme: {
    type: String,
    default: 'light'
  }
})

const giscusContainer = ref(null)
const loading = ref(true)

onMounted(async () => {
  await nextTick()
  loadGiscus()
})

const loadGiscus = () => {
  // 检查是否已经加载过
  if (document.querySelector('#giscus-script')) {
    return
  }

  const script = document.createElement('script')
  script.id = 'giscus-script'
  script.src = 'https://giscus.app/client.js'
  script.setAttribute('data-repo', 'your-username/your-repo') // 替换为你的GitHub仓库
  script.setAttribute('data-repo-id', 'your-repo-id') // 替换为你的仓库ID
  script.setAttribute('data-category', 'General')
  script.setAttribute('data-category-id', 'your-category-id') // 替换为分类ID
  script.setAttribute('data-mapping', 'specific')
  script.setAttribute('data-term', props.identifier)
  script.setAttribute('data-strict', '0')
  script.setAttribute('data-reactions-enabled', '1')
  script.setAttribute('data-emit-metadata', '0')
  script.setAttribute('data-input-position', 'top')
  script.setAttribute('data-theme', props.theme)
  script.setAttribute('data-lang', 'zh-CN')
  script.setAttribute('data-loading', 'lazy')
  script.crossOrigin = 'anonymous'
  script.async = true

  script.onload = () => {
    loading.value = false
  }

  script.onerror = () => {
    loading.value = false
    console.error('Failed to load Giscus comments')
  }

  if (giscusContainer.value) {
    giscusContainer.value.appendChild(script)
  }
}
</script>

<style scoped>
/* 自定义Giscus样式 */
:deep(.gsc-comments) {
  border-radius: 12px;
}

:deep(.gsc-comment-box) {
  border-radius: 8px;
}
</style> 