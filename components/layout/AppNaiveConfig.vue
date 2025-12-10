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
 * 重构说明（2024-12-XX）：
 * - 优先使用具体颜色值，减少对 CSS 变量的依赖
 * - 根据 currentTheme 动态计算深色/浅色模式的颜色
 * - 保留少量必要的 CSS 变量（如圆角、阴影等）
 */
const naiveThemeOverrides = computed<GlobalThemeOverrides>(() => {
  const isDark = currentTheme.value === 'dark'
  
  // 深色模式颜色
  const darkColors = {
    // 背景色
    bodyColor: '#0a0e1a',
    cardColor: 'rgba(19, 23, 32, 0.8)',
    hoverColor: '#131720',
    // 文字颜色
    textColorBase: 'rgba(255, 255, 255, 0.92)',
    textColor1: 'rgba(255, 255, 255, 0.92)',
    textColor2: 'rgba(255, 255, 255, 0.6)',
    textColor3: 'rgba(255, 255, 255, 0.4)',
    // 边框颜色
    borderColor: 'rgba(255, 255, 255, 0.1)',
    // 主色调
    primaryColor: '#60a5fa',
    primaryColorHover: '#3b82f6',
    primaryColorPressed: '#2563eb',
    primaryColorSuppl: 'rgba(96, 165, 250, 0.15)',
  }
  
  // 浅色模式颜色
  const lightColors = {
    // 背景色
    bodyColor: '#ffffff',
    cardColor: '#ffffff',
    hoverColor: '#f8fafc',
    // 文字颜色
    textColorBase: '#0f172a',
    textColor1: '#0f172a',
    textColor2: '#64748b',
    textColor3: '#94a3b8',
    // 边框颜色
    borderColor: '#e2e8f0',
    // 主色调
    primaryColor: '#2563eb',
    primaryColorHover: '#1d4ed8',
    primaryColorPressed: '#1e40af',
    primaryColorSuppl: '#dbeafe',
  }
  
  const colors = isDark ? darkColors : lightColors
  
  return {
    common: {
      // 主色调
      primaryColor: colors.primaryColor,
      primaryColorHover: colors.primaryColorHover,
      primaryColorPressed: colors.primaryColorPressed,
      primaryColorSuppl: colors.primaryColorSuppl,
      
      // 文字颜色
      textColorBase: colors.textColorBase,
      textColor1: colors.textColor1,
      textColor2: colors.textColor2,
      textColor3: colors.textColor3,
      
      // 背景色
      bodyColor: colors.bodyColor,
      cardColor: colors.cardColor,
      hoverColor: colors.hoverColor,
      
      // 边框颜色
      borderColor: colors.borderColor,
      
      // 圆角：使用 CSS 变量（这些是布局相关的，保留变量）
      borderRadius: 'var(--radius-md)',
      borderRadiusSmall: 'var(--radius-sm)',
      borderRadiusLarge: 'var(--radius-lg)',
    },
    Button: {
      borderRadiusSmall: 'var(--radius-sm)',
      borderRadiusMedium: 'var(--radius-md)',
      borderRadiusLarge: 'var(--radius-lg)',
      // 默认按钮
      textColor: colors.textColor1,
      textColorHover: colors.textColor1,
      textColorPressed: colors.textColor1,
      textColorFocus: colors.textColor1,
      textColorDisabled: colors.textColor3,
      // 主要按钮文字颜色（白色）
      textColorPrimary: '#ffffff',
      textColorPrimaryHover: '#ffffff',
      textColorPrimaryPressed: '#ffffff',
      // 成功按钮文字颜色
      textColorSuccess: '#ffffff',
      textColorSuccessHover: '#ffffff',
      // 警告按钮文字颜色
      textColorWarning: '#ffffff',
      textColorWarningHover: '#ffffff',
      // 错误按钮文字颜色
      textColorError: '#ffffff',
      textColorErrorHover: '#ffffff',
      // 次要按钮（secondary）
      colorSecondary: isDark ? 'rgba(255, 255, 255, 0.1)' : '#f1f5f9',
      colorSecondaryHover: isDark ? 'rgba(255, 255, 255, 0.15)' : '#e2e8f0',
      colorSecondaryPressed: isDark ? 'rgba(255, 255, 255, 0.2)' : '#cbd5e1',
      // 三级按钮（tertiary）
      colorTertiary: 'transparent',
      colorTertiaryHover: isDark ? 'rgba(255, 255, 255, 0.05)' : '#f8fafc',
      colorTertiaryPressed: isDark ? 'rgba(255, 255, 255, 0.1)' : '#e2e8f0',
      // 四级按钮（quaternary）- 用于取消等操作
      colorQuaternary: 'transparent',
      colorQuaternaryHover: isDark ? 'rgba(255, 255, 255, 0.05)' : '#f8fafc',
      colorQuaternaryPressed: isDark ? 'rgba(255, 255, 255, 0.1)' : '#e2e8f0',
    },
    Card: {
      borderRadius: 'var(--radius-xl)',
      color: colors.cardColor,
      boxShadow: isDark 
        ? '0 4px 6px -1px rgb(0 0 0 / 0.4), 0 2px 4px -2px rgb(0 0 0 / 0.4)'
        : '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
      borderColor: colors.borderColor,
      paddingMedium: '16px 20px',
    },
    Input: {
      borderRadius: 'var(--radius-md)',
      // 边框颜色
      borderColor: colors.borderColor,
      borderColorHover: isDark ? 'rgba(255, 255, 255, 0.25)' : '#cbd5e1',
      borderColorFocus: colors.primaryColor,
      borderColorDisabled: isDark ? 'rgba(255, 255, 255, 0.05)' : '#e2e8f0',
      // 背景颜色
      color: isDark ? 'rgba(255, 255, 255, 0.05)' : '#ffffff',
      colorDisabled: isDark ? 'rgba(255, 255, 255, 0.02)' : '#f8fafc',
      colorFocus: isDark ? 'rgba(255, 255, 255, 0.08)' : '#ffffff',
      // 文字颜色
      textColor: colors.textColor1,
      textColorDisabled: colors.textColor3,
      // placeholder 颜色（增强对比度）
      placeholderColor: isDark ? 'rgba(255, 255, 255, 0.5)' : '#94a3b8',
      // 图标颜色
      iconColor: colors.textColor2,
      iconColorHover: colors.textColor1,
    },
    Select: {
      borderRadius: 'var(--radius-md)',
      // 边框颜色
      borderColor: colors.borderColor,
      borderColorHover: isDark ? 'rgba(255, 255, 255, 0.25)' : '#cbd5e1',
      borderColorActive: colors.primaryColor,
      borderColorDisabled: isDark ? 'rgba(255, 255, 255, 0.05)' : '#e2e8f0',
      // 背景颜色
      color: isDark ? 'rgba(255, 255, 255, 0.05)' : '#ffffff',
      colorDisabled: isDark ? 'rgba(255, 255, 255, 0.02)' : '#f8fafc',
      colorActive: isDark ? 'rgba(255, 255, 255, 0.08)' : '#ffffff',
      // 文字颜色
      textColor: colors.textColor1,
      textColorDisabled: colors.textColor3,
      // placeholder 颜色
      placeholderColor: isDark ? 'rgba(255, 255, 255, 0.5)' : '#94a3b8',
      // 箭头颜色
      arrowColor: colors.textColor2,
      arrowColorHover: colors.textColor1,
      arrowColorActive: colors.primaryColor,
    },
    Form: {
      labelTextColor: colors.textColor1,
      labelFontWeight: '500',
      labelFontSize: '14px',
      // 错误提示颜色
      feedbackTextColor: isDark ? '#f87171' : '#ef4444',
      feedbackTextColorWarning: isDark ? '#fbbf24' : '#f59e0b',
      feedbackTextColorError: isDark ? '#f87171' : '#ef4444',
    },
    DataTable: {
      borderRadius: 'var(--radius-lg)',
      thColor: colors.hoverColor,
      tdColor: colors.cardColor,
      thTextColor: colors.textColor1,
      tdTextColor: colors.textColor1,
      borderColor: colors.borderColor,
      // 增强深色模式可读性
      thBorderColor: isDark ? 'rgba(255, 255, 255, 0.15)' : '#cbd5e1',
      tdBorderColor: isDark ? 'rgba(255, 255, 255, 0.1)' : '#e2e8f0',
      // 行 hover 效果
      tdColorHover: isDark ? 'rgba(255, 255, 255, 0.05)' : '#f8fafc',
      // 斑马纹
      tdColorStriped: isDark ? 'rgba(255, 255, 255, 0.02)' : '#fafbfc',
    },
    Layout: {
      color: colors.bodyColor,
      headerColor: colors.cardColor,
      footerColor: colors.cardColor,
      siderColor: isDark ? '#131720' : '#f8fafc',
      siderBorderColor: colors.borderColor,
      // 增强层级感
      headerBorderColor: isDark ? 'rgba(255, 255, 255, 0.1)' : '#e2e8f0',
      footerBorderColor: isDark ? 'rgba(255, 255, 255, 0.1)' : '#e2e8f0',
    },
    Menu: {
      borderRadius: 'var(--radius-md)',
      // 菜单项背景
      itemColor: 'transparent',
      itemColorActive: colors.primaryColorSuppl,
      itemColorHover: colors.hoverColor,
      // 菜单项文字
      itemTextColor: colors.textColor1,
      itemTextColorActive: colors.primaryColor,
      itemTextColorHover: colors.textColor1,
      // 菜单项图标
      itemIconColor: colors.textColor2,
      itemIconColorActive: colors.primaryColor,
      itemIconColorHover: colors.textColor1,
      // 菜单项分组标题
      groupTextColor: colors.textColor2,
      // 增强激活状态
      itemColorActiveHover: isDark 
        ? 'rgba(96, 165, 250, 0.25)' 
        : 'rgba(37, 99, 235, 0.12)',
    },
    Message: {
      borderRadius: 'var(--radius-lg)',
      boxShadow: isDark
        ? '0 10px 15px -3px rgb(0 0 0 / 0.5), 0 4px 6px -4px rgb(0 0 0 / 0.5)'
        : '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
    },
    Notification: {
      borderRadius: 'var(--radius-lg)',
      boxShadow: isDark
        ? '0 10px 15px -3px rgb(0 0 0 / 0.5), 0 4px 6px -4px rgb(0 0 0 / 0.5)'
        : '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
    },
    Dialog: {
      borderRadius: 'var(--radius-xl)',
      boxShadow: isDark
        ? '0 10px 15px -3px rgb(0 0 0 / 0.5), 0 4px 6px -4px rgb(0 0 0 / 0.5)'
        : '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
    },
    Drawer: {
      color: colors.cardColor,
      headerBorderBottom: `1px solid ${colors.borderColor}`,
    },
    Tag: {
      borderRadius: 'var(--radius-md)',
    },
    Divider: {
      color: colors.borderColor,
    },
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

