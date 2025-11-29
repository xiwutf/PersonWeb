<template>
  <div>
    <!-- 动态加载风格组件 -->
    <component :is="currentComponent" v-if="currentComponent" />
    <div v-else class="min-h-screen flex items-center justify-center">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <p class="text-gray-600">加载中...</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import HomeDarkLab from '~/components/home/HomeDarkLab.vue'
import HomeLightPortfolio from '~/components/home/HomeLightPortfolio.vue'

const api = useApi()
const style = ref<string>('dark-lab')

// 风格组件映射
const componentMap: Record<string, any> = {
  'dark-lab': HomeDarkLab,
  'light-portfolio': HomeLightPortfolio
}

// 当前组件
const currentComponent = computed(() => {
  return componentMap[style.value] || componentMap['dark-lab']
})

// 获取当前启用的首页风格
const fetchHomeStyle = async () => {
  try {
    const res = await api.get<{ style: string }>('/config/home-style')
    if (res && res.style) {
      style.value = res.style
    }
  } catch (e) {
    console.error('Failed to fetch home style:', e)
    // 默认使用 dark-lab
    style.value = 'dark-lab'
  }
}

onMounted(() => {
  fetchHomeStyle()
})

definePageMeta({
  layout: 'home'
})

useHead({
  title: '溪午听风 - 数字花园',
  meta: [{ name: 'description', content: '溪午听风的个人网站，分享技术、生活与思考。' }]
})
</script>
