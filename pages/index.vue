<template>
  <div>
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
import { computed, defineAsyncComponent, onMounted, ref } from 'vue'

const api = useApi()
const style = ref<string>('creative')

const componentMap: Record<string, any> = {
  'dark-lab': defineAsyncComponent(() => import('~/components/home/HomeDarkLab.vue')),
  'light-portfolio': defineAsyncComponent(() => import('~/components/home/HomeLightPortfolio.vue')),
  'hybrid-super': defineAsyncComponent(() => import('~/components/home/HomeHybridSuper.vue')),
  creative: defineAsyncComponent(() => import('~/components/home/HomeCreative.vue'))
}

const currentComponent = computed(() => {
  return componentMap[style.value] || componentMap.creative
})

const fetchHomeStyle = async () => {
  try {
    const res = await api.get<{ style: string }>('/config/home-style')
    style.value = res?.style || 'creative'
  } catch (error) {
    console.error('Failed to fetch home style:', error)
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
  title: '溪风听风 - 构建 AI 与软件系统的长期主义者',
  meta: [
    {
      name: 'description',
      content: '溪风听风，AI 工程师、系统构建者、技术创业者。专注于将技术转化为可持续积累的数字资产，构建 AI 系统、工业软件与个人数字系统。'
    }
  ]
})
</script>
