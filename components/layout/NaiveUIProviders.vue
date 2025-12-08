<template>
  <component 
    :is="ProvidersComponent" 
    v-if="ProvidersComponent"
    :theme="theme" 
    :theme-overrides="themeOverrides"
  >
    <slot />
  </component>
  <div v-else>
    <!-- 服务端渲染时的 fallback -->
    <slot />
  </div>
</template>

<script setup lang="ts">
/**
 * Naive UI Providers 包装组件
 * 只在客户端渲染，避免 SSR 错误
 * 
 * 这个组件使用动态导入，确保 Naive UI 只在客户端加载
 */

import { ref, onMounted, defineComponent, h, markRaw } from 'vue'
import type { GlobalTheme, GlobalThemeOverrides } from 'naive-ui'

const props = defineProps<{
  theme?: GlobalTheme | null
  themeOverrides?: GlobalThemeOverrides
}>()

const ProvidersComponent = ref<any>(null)
const isMounted = ref(false)

// 使用 onMounted 确保只在客户端执行
onMounted(async () => {
  // 双重检查，确保在客户端
  if (typeof window === 'undefined') return
  
  try {
    // 动态导入 Naive UI，避免 SSR 时执行
    const naiveUI = await import('naive-ui')
    const { NMessageProvider, NDialogProvider, NConfigProvider } = naiveUI
    
    // 创建包装组件
    // 使用 markRaw 避免组件被响应式化，提高性能
    ProvidersComponent.value = markRaw(defineComponent({
      name: 'NaiveUIProvidersWrapper',
      props: {
        theme: Object,
        themeOverrides: Object
      },
      setup(componentProps, { slots }) {
        return () => h(NConfigProvider, {
          theme: componentProps.theme,
          themeOverrides: componentProps.themeOverrides
        }, {
          default: () => h(NMessageProvider, {}, {
            default: () => h(NDialogProvider, {}, {
              default: () => slots.default?.()
            })
          })
        })
      }
    }))
    
    isMounted.value = true
  } catch (error) {
    console.error('Failed to load Naive UI components:', error)
  }
})
</script>

