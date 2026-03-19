<template>
  <div>
    <!-- 动态加载风格组件 -->
    <component :is="currentComponent" v-if="currentComponent" />
    <div v-else class="min-h-screen flex items-center justify-center bg-black text-var(--color-bg-light, white)">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-400 mx-auto mb-4"></div>
        <p class="text-var(--color-bg-light, white)/60">加载首页组件中...</p>
        <p class="text-var(--color-bg-light, white)/40 text-sm mt-2">当前风格: {{ style }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import HomeDarkLab from '~/components/home/HomeDarkLab.vue'
import HomeLightPortfolio from '~/components/home/HomeLightPortfolio.vue'
import HomeHybridSuper from '~/components/home/HomeHybridSuper.vue'
import HomeCreative from '~/components/home/HomeCreative.vue'

const api = useApi()
const style = ref<string>('creative') // 默认使用新的创意堆叠风格

// 风格组件映射
const componentMap: Record<string, any> = {
  'dark-lab': HomeDarkLab,
  'light-portfolio': HomeLightPortfolio,
  'hybrid-super': HomeHybridSuper,
  'creative': HomeCreative // 新增风格
}

// 当前组件
const currentComponent = computed(() => {
  return componentMap[style.value] || componentMap['creative']
})

// 获取当前启用的首页风格
const fetchHomeStyle = async () => {
  try {
    const res = await api.get<{ style: string }>('/config/home-style')
    if (res && res.style) {
      // 优先使用后端配置，如果没有则使用默认
      style.value = res.style || 'creative'
    } else {
      style.value = 'creative'
    }
  } catch (e) {
    console.error('Failed to fetch home style:', e)
    // 默认使用 creative
    style.value = 'creative'
  }
}

onMounted(() => {
  fetchHomeStyle()
})

definePageMeta({
  layout: 'home'
})

useHead({
  title: '溪午听风 - 构建 AI 与软件系统的长期主义者',
  meta: [{ name: 'description', content: '溪午听风：AI 工程师、系统构建者、技术创业者。专注于将技术转化为可持续积累的数字资产，构建 AI 系统、工业软件与个人数字系统。' }]
})
</script>
