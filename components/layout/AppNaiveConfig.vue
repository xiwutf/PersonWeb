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
  <div v-else>
    <!-- 服务端渲染时的 fallback -->
    <slot />
  </div>
</template>

<script setup lang="ts">
/**
 * AppNaiveConfig - 统一的 Naive UI 配置容器
 * 
 * 职责：
 * - 统一管理 Naive UI 的主题配置
 * - 基于 useTheme().currentTheme 计算 Naive 主题
 * - 使用 CSS 变量作为 Naive 的主题色，与 tokens 联动
 * - 提供 NConfigProvider、NDialogProvider、NMessageProvider、NNotificationProvider
 * 
 * 使用方式：
 * - 在 layouts/default.vue 或 app.vue 中使用 <AppNaiveConfig> 包裹内容
 * - 所有 Naive UI 组件会自动应用统一的主题配置
 */

import { ref, computed, onMounted, defineComponent, h, markRaw } from 'vue'
import type { GlobalTheme, GlobalThemeOverrides } from 'naive-ui'

const ProvidersComponent = ref<any>(null)

// 使用主题管理 composable
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
 * 使用 CSS 变量作为主题色，确保与 tokens 联动
 */
const naiveThemeOverrides = computed<GlobalThemeOverrides>(() => {
  return {
    common: {
      // 主色调：使用 CSS 变量
      primaryColor: 'var(--color-primary)',
      primaryColorHover: 'var(--color-primary-hover)',
      primaryColorPressed: 'var(--color-primary-hover)',
      primaryColorSuppl: 'var(--color-primary-soft)',
      
      // 文字颜色：使用 CSS 变量
      textColorBase: 'var(--color-text-main)',
      textColor1: 'var(--color-text-main)',
      textColor2: 'var(--color-text-muted)',
      textColor3: 'var(--color-text-disabled)',
      
      // 背景色：使用 CSS 变量
      bodyColor: 'var(--color-bg-body)',
      cardColor: 'var(--color-bg-card)',
      hoverColor: 'var(--color-bg-elevated)',
      
      // 边框颜色：使用 CSS 变量
      borderColor: 'var(--color-border-default)',
      
      // 圆角：使用 CSS 变量
      borderRadius: 'var(--radius-md)',
      borderRadiusSmall: 'var(--radius-sm)',
      borderRadiusLarge: 'var(--radius-lg)',
    },
    Button: {
      borderRadiusSmall: 'var(--radius-sm)',
      borderRadiusMedium: 'var(--radius-md)',
      borderRadiusLarge: 'var(--radius-lg)',
      textColor: 'var(--color-text-main)',
      textColorHover: 'var(--color-text-main)',
      textColorPressed: 'var(--color-text-main)',
      textColorFocus: 'var(--color-text-main)',
      textColorDisabled: 'var(--color-text-disabled)',
    },
    Card: {
      borderRadius: 'var(--radius-xl)',
      color: 'var(--color-bg-card)',
      boxShadow: 'var(--shadow-soft)',
      borderColor: 'var(--color-border-subtle)',
    },
    Input: {
      borderRadius: 'var(--radius-md)',
      borderColor: 'var(--color-border-default)',
      borderColorHover: 'var(--color-border-strong)',
      borderColorFocus: 'var(--color-primary)',
      color: 'var(--color-text-main)',
      colorFocus: 'var(--color-text-main)',
      placeholderColor: 'var(--color-text-muted)',
    },
    Select: {
      borderRadius: 'var(--radius-md)',
      borderColor: 'var(--color-border-default)',
      borderColorHover: 'var(--color-border-strong)',
      borderColorActive: 'var(--color-primary)',
      color: 'var(--color-text-main)',
    },
    Form: {
      labelTextColor: 'var(--color-text-main)',
      labelFontWeight: '500',
    },
    DataTable: {
      borderRadius: 'var(--radius-lg)',
      thColor: 'var(--color-bg-elevated)',
      tdColor: 'var(--color-bg-card)',
      thTextColor: 'var(--color-text-main)',
      tdTextColor: 'var(--color-text-main)',
      borderColor: 'var(--color-border-subtle)',
    },
    Menu: {
      borderRadius: 'var(--radius-md)',
      itemColorActive: 'var(--color-primary-soft)',
      itemTextColor: 'var(--color-text-main)',
      itemTextColorActive: 'var(--color-primary)',
      itemTextColorHover: 'var(--color-text-main)',
      itemColorHover: 'var(--color-bg-elevated)',
    },
    Message: {
      borderRadius: 'var(--radius-lg)',
      boxShadow: 'var(--shadow-lg)',
    },
    Notification: {
      borderRadius: 'var(--radius-lg)',
      boxShadow: 'var(--shadow-lg)',
    },
    Dialog: {
      borderRadius: 'var(--radius-xl)',
      boxShadow: 'var(--shadow-lg)',
    },
    Drawer: {
      color: 'var(--color-bg-card)',
      headerBorderBottom: '1px solid var(--color-border-subtle)',
    },
    Tag: {
      borderRadius: 'var(--radius-md)',
    },
    Divider: {
      color: 'var(--color-border-subtle)',
    },
  }
})

// 动态导入 Naive UI 组件，避免 SSR 错误
onMounted(async () => {
  // 双重检查，确保在客户端
  if (typeof window === 'undefined') return
  
  try {
    // 动态导入 Naive UI，避免 SSR 时执行
    const naiveUI = await import('naive-ui')
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

