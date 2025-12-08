<template>
  <div>
    <!-- 动态加载风格组件 -->
    <component :is="currentComponent" v-if="currentComponent" />
    <div v-else class="min-h-screen flex items-center justify-center bg-black text-white">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-400 mx-auto mb-4"></div>
        <p class="text-white/60">加载首页组件中...</p>
        <p class="text-white/40 text-sm mt-2">当前风格: {{ style }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import HomeDarkLab from '~/components/home/HomeDarkLab.vue'
import HomeLightPortfolio from '~/components/home/HomeLightPortfolio.vue'
import HomeHybridSuper from '~/components/home/HomeHybridSuper.vue'

const api = useApi()
const style = ref<string>('hybrid-super') // 默认使用新的混合超级风格

// 风格组件映射
const componentMap: Record<string, any> = {
  'dark-lab': HomeDarkLab,
  'light-portfolio': HomeLightPortfolio,
  'hybrid-super': HomeHybridSuper
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
      // 强制使用 hybrid-super 风格（V2 重构版本）
      // 如果需要使用后端配置，可以取消下面的注释并删除强制设置
      // style.value = res.style
      style.value = 'hybrid-super'
    } else {
      style.value = 'hybrid-super'
    }
  } catch (e) {
    console.error('Failed to fetch home style:', e)
    // 默认使用 hybrid-super（新的混合超级风格）
    style.value = 'hybrid-super'
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
