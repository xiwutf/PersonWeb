<template>
  <!-- 统一的 Naive UI 配置容器 -->
  <component 
    :is="ProvidersComponent" 
    v-if="ProvidersComponent"
    :theme="naiveTheme" 
    :theme-overrides="naiveThemeOverrides"
  >
    <slot />
  </component>
  <div v-else class="app-naive-config-fallback">
    <!-- 服务端渲染时的 fallback，使用与主题一致的背景色 -->
    <slot />
  </div>
</template>

<script setup lang="ts">
/**
 * AppNaiveConfig - 统一的 Naive UI 配置容器
 * 
 * 职责：
 * - 统一管理 Naive UI 的主题配置（前台 + 后台共用）
 * - 基于 useTheme().currentTheme 计算 Naive 主题
 * - 确保 data-theme 和 Naive UI 主题同步
 * - 提供 NConfigProvider、NDialogProvider、NMessageProvider、NNotificationProvider
 * 
 * 重构说明（2024-12-XX）：
 * - 前台和后台统一使用此组件，不再有独立的 useNaiveTheme
 * - 主题切换时自动同步 data-theme 属性
 * - themeOverrides 优先使用具体颜色值，减少对 CSS 变量的依赖
 * 
 * 使用方式：
 * - 在 layouts/default.vue 和 layouts/admin.vue 中使用 <AppNaiveConfig> 包裹内容
 * - 所有 Naive UI 组件会自动应用统一的主题配置
 */

import { ref, computed, onMounted, watch, defineComponent, h, markRaw } from 'vue'
import type { GlobalTheme, GlobalThemeOverrides } from 'naive-ui'

const ProvidersComponent = ref<any>(null)

// 使用全局主题管理 composable
const { currentTheme } = useTheme()

/**
 * 将项目主题 key 映射到 Naive UI 的 theme
 * 
 * 重构说明（2024-12-XX）：
 * - 现在只支持 light 和 dark 两个主题
 * - light: 使用 Naive 默认主题（null）
 * - dark: 使用 darkTheme
 */
const naiveTheme = computed<GlobalTheme | null>(() => {
  if (!process.client) return null
  
  // 如果当前主题是 light，使用 Naive 默认主题
  if (currentTheme.value === 'light') {
    return null
  }
  
  // dark 主题使用 darkTheme
  // 注意：这里需要动态导入 darkTheme，避免 SSR 错误
  return null // 将在 onMounted 中设置
})

/**
 * Naive UI 主题覆盖配置
 * 
 * Aurora Design System (V3)
 * 极光设计系统覆盖
 */
const naiveThemeOverrides = computed<GlobalThemeOverrides>(() => {
  const isDark = currentTheme.value === 'dark'
  
  // Aurora Design System V3
  const colors = isDark 
    ? {
        // Dark Mode (Deep Space)
        primary: '#2997FF',        // Electric Blue
        primaryHover: '#60A5FA',
        primaryPressed: '#1E40AF',
        primarySuppl: 'rgba(41, 151, 255, 0.15)',
        bgBody: '#050510',         // Abyss Black
        bgCard: 'rgba(20, 25, 40, 0.6)', // Glass Base
        textMain: '#FFFFFF',
        textSec: 'rgba(255, 255, 255, 0.7)',
        border: 'rgba(255, 255, 255, 0.1)',
        borderHover: 'rgba(41, 151, 255, 0.5)',
      }
    : {
        // Light Mode (Zen Workshop)
        primary: '#000000',        // Sure Black
        primaryHover: '#333333',
        primaryPressed: '#000000',
        primarySuppl: 'rgba(0, 0, 0, 0.08)',
        bgBody: '#F7F7F7',         // Off White
        bgCard: '#FFFFFF',         // Pure White
        textMain: '#171717',
        textSec: '#525252',
        border: 'rgba(0, 0, 0, 0.08)',
        borderHover: 'rgba(0, 0, 0, 0.2)',
      }

  return {
    common: {
      primaryColor: colors.primary,
      primaryColorHover: colors.primaryHover,
      primaryColorPressed: colors.primaryPressed,
      primaryColorSuppl: colors.primarySuppl,

      baseColor: colors.bgBody,
      bodyColor: colors.bgBody,
      cardColor: colors.bgCard,
      
      textColorBase: colors.textMain,
      textColor1: colors.textMain,
      textColor2: colors.textSec,
      
      borderColor: colors.border,
      borderRadius: '8px',
      borderRadiusSmall: '6px',
    },
    Card: {
      color: colors.bgCard,
      borderRadius: '16px',
      borderColor: colors.border,
      boxShadow: isDark 
        ? '0 8px 32px rgba(0, 0, 0, 0.4)' 
        : '0 4px 20px rgba(0, 0, 0, 0.03)',
    },
    Button: {
      borderRadius: '8px',
      fontWeight: '600',
      textColorPrimary: '#FFFFFF', // Light mode uses Black bg, so text is White
    },
    Input: {
      borderRadius: '8px',
      borderColor: colors.border,
      color: isDark ? 'rgba(255,255,255,0.03)' : '#FFFFFF',
      colorFocus: isDark ? 'rgba(0,0,0,0.2)' : '#FFFFFF',
      boxShadowFocus: `0 0 0 2px ${colors.primarySuppl}`,
    },
    DataTable: {
      borderRadius: '12px',
      borderColor: colors.border,
      thColor: isDark ? 'rgba(255,255,255,0.02)' : 'rgba(0,0,0,0.02)',
      tdColor: 'transparent',
      tdColorHover: isDark ? 'rgba(255,255,255,0.03)' : 'rgba(0,0,0,0.02)',
    }
  }
})

// 确保 data-theme 和 Naive UI 主题同步
watch(currentTheme, (newTheme) => {
  if (process.client) {
    // 确保 data-theme 属性与当前主题同步
    document.documentElement.setAttribute('data-theme', newTheme)
  }
}, { immediate: true })

// 动态导入 Naive UI 组件，避免 SSR 错误
onMounted(async () => {
  // 双重检查，确保在客户端
  if (typeof window === 'undefined') return
  
  // 确保 data-theme 属性已设置
  document.documentElement.setAttribute('data-theme', currentTheme.value)
  
  try {
    // 动态导入 Naive UI，避免 SSR 时执行
    // 使用动态 import 实现代码分割，减少初始包大小
    const naiveUI = await import(/* webpackChunkName: "naive-ui" */ 'naive-ui')
    const { NMessageProvider, NDialogProvider, NNotificationProvider, NConfigProvider, darkTheme } = naiveUI
    
    // 创建包装组件
    // 使用 markRaw 避免组件被响应式化，提高性能
    ProvidersComponent.value = markRaw(defineComponent({
      name: 'AppNaiveConfigWrapper',
      props: {
        theme: Object,
        themeOverrides: Object
      },
      setup(componentProps, { slots }) {
        // 计算实际的主题（考虑 light 主题使用默认）
        // 重构说明（2024-12-XX）：现在只支持 light 和 dark 两个主题
        const actualTheme = computed(() => {
          if (currentTheme.value === 'light') {
            return null
          }
          // dark 主题使用 darkTheme
          return darkTheme
        })
        
        return () => h(NConfigProvider, {
          theme: actualTheme.value,
          themeOverrides: componentProps.themeOverrides
        }, {
          default: () => h(NMessageProvider, {}, {
            default: () => h(NDialogProvider, {}, {
              default: () => h(NNotificationProvider, {}, {
                default: () => slots.default?.()
              })
            })
          })
        })
      }
    }))
  } catch (error) {
    console.error('Failed to load Naive UI components:', error)
  }
})
</script>

<style scoped>
/* SSR fallback 样式，确保在客户端挂载前有正确的背景色 */
.app-naive-config-fallback {
  width: 100%;
  height: 100%;
  /* 使用与主题一致的背景色，避免蓝屏 */
  background: var(--n-body-color, var(--color-bg-body, #020617));
  color: var(--color-text-main, #e2e8f0);
}
</style>

