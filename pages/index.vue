<template>
  <component :is="currentComponent" />
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent, type Component } from 'vue'

const api = useApi()
const DEFAULT_HOME_STYLE = 'dark-lab'

const componentMap: Record<string, Component> = {
  'dark-lab': defineAsyncComponent(() => import('~/components/home/HomeDarkLab.vue')),
  'light-portfolio': defineAsyncComponent(() => import('~/components/home/HomeLightPortfolio.vue')),
  'hybrid-super': defineAsyncComponent(() => import('~/components/home/HomeHybridSuper.vue')),
  creative: defineAsyncComponent(() => import('~/components/home/HomeCreative.vue'))
}

const { data: homeStyle } = await useAsyncData(
  'home-style',
  async () => {
    try {
      const res = await api.get<{ style?: string; Style?: string }>('/config/home-style')
      return res?.style || res?.Style || DEFAULT_HOME_STYLE
    } catch {
      return DEFAULT_HOME_STYLE
    }
  },
  {
    default: () => DEFAULT_HOME_STYLE
  }
)

const style = computed(() => homeStyle.value || DEFAULT_HOME_STYLE)

const currentComponent = computed(() => {
  return componentMap[style.value] || componentMap[DEFAULT_HOME_STYLE]
})

definePageMeta({
  layout: 'home'
})

useHead({
  title: '溪午听风 - AI 与软件系统构建者',
  meta: [
    {
      name: 'description',
      content: 'AI 工程、软件系统、自动化工具与长期主义实践的个人展示站。'
    }
  ]
})
</script>
