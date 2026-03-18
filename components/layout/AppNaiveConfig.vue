<template>
  <!-- 统一的 Naive UI 配置容器 -->
  <ClientOnly>
    <component
      :is="ProvidersComponent"
      v-if="ProvidersComponent"
      :theme="naiveTheme"
      :theme-overrides="naiveThemeOverrides"
    >
      <slot />
    </component>
    <template #fallback>
      <!-- 服务端渲染时的 fallback，使用与主题一致的背景色 -->
      <div class="app-naive-config-fallback">
        <slot />
      </div>
    </template>
  </ClientOnly>
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
 * 重构说明（2026-03-16）：
 * - 支持 light、dark、hybrid-super-dark 三个主题
 * - light: 使用 Naive 默认主题（null）
 * - dark: 使用 darkTheme
 * - hybrid-super-dark: 使用 hybridSuperDarkTheme（新增）
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
 * 重构说明（2026-03-16）：
 * - 支持 light、dark、hybrid-super-dark 三个主题
 * - light: 使用 Naive 默认主题（null）
 * - dark: 使用 darkTheme
 * - hybrid-super-dark: 使用 hybridSuperDarkTheme（新增）
 * - 注意：这里需要动态导入主题，避免 SSR 错误
 */
const naiveTheme = computed<GlobalTheme | null>(() => {
  // @ts-expect-error - Nuxt's process.client is not in TypeScript types
  if (!process.client) return null

  // 如果当前主题是 light，使用 Naive 默认主题
  if (currentTheme.value === 'light') {
    return null
  }

  // dark 和 hybrid-super-dark 主题使用深色主题
  // 注意：这里需要动态导入主题，避免 SSR 错误
  return null // 将在 onMounted 中设置
})

/**
 * Naive UI 主题覆盖配置
 *
 * Aurora Design System (V3)
 * 光设计系统主题覆盖
 */
const naiveThemeOverrides = computed<GlobalThemeOverrides>(() => {
  const isDark = currentTheme.value === 'dark' || currentTheme.value === 'hybrid-super-dark'

  // Aurora Design System V3 颜色
  const v3Colors = {
    primary: '#2997FF',
    primaryHover: '#60A5FA',
    primaryPressed: '#1E40AF',
    primarySuppl: 'rgba(41, 151, 255, 0.15)',
    bgBody: '#050510',      // Deep Space
    bgCard: 'rgba(20, 25, 40, 0.6)', // Glass Base
    bgElevated: 'rgba(40, 50, 70, 0.6)', // Elevated
    bgDark: '#0A0A1A',      // Darker
    textMain: '#FFFFFF',
    textSec: 'rgba(255, 255, 255, 0.7)',
    border: 'rgba(255, 255, 255, 0.1)',
    borderHover: 'rgba(41, 151, 255, 0.5)',
  }

  // Zen Workshop 颜色
  const zenColors = {
    primary: '#000000',
    primaryHover: '#333333',
    primaryPressed: '#000000',
    primarySuppl: 'rgba(0, 0, 0, 0.08)',
    bgBody: '#F7F7F7',
    bgCard: '#FFFFFF',
    bgElevated: '#F3F4F6',
    bgDark: '#1F2937',
    textMain: '#171717',
    textSec: '#525252',
    border: 'rgba(0, 0, 0, 0.08)',
    borderHover: 'rgba(0, 0, 0, 0.2)',
  }

  // 根据主题选择颜色
  const colors = isDark ? v3Colors : zenColors

  return {
    common: {
      primaryColor: colors.primary,
      primaryColorHover: colors.primaryHover,
      primaryColorPressed: colors.primaryPressed,
      primaryColorSuppl: colors.primarySuppl,
      baseColor: colors.bgBody,
      bodyColor: colors.bgBody,
      cardColor: colors.bgCard,
      siderColor: colors.bgCard,
      headerColor: colors.bgCard,
      footerColor: colors.bgCard,
      textColorBase: colors.textMain,
      textColor1: colors.textMain,
      textColor2: colors.textSec,
      textColor3: colors.textSec,
      textColorDisabled: colors.textSec,
      dividerColor: colors.border,
      borderColor: colors.border,
      scrollColor: colors.border,
    },
    Layout: {
      color: colors.bgBody,
      siderColor: colors.bgCard,
      headerColor: colors.bgCard,
      footerColor: colors.bgCard,
    },
    Card: {
      color: colors.bgCard,
      colorEmbedded: colors.bgCard,
      colorModal: colors.bgCard,
      colorEmbeddedModal: colors.bgBody,
      borderRadius: '16px',
      boxShadow: isDark ? '0 8px 32px rgba(0, 0, 0, 0.6)' : '0 4px 20px rgba(0, 0, 0, 0.1)',
      borderColor: colors.border,
      actionColor: colors.textMain,
    },
    Input: {
      color: colors.bgBody,
      borderColor: colors.border,
      borderHover: colors.borderHover,
      borderFocus: colors.primary,
      placeholderColor: colors.textSec,
      colorFocus: isDark ? 'rgba(0, 0, 0, 0.2)' : '#FFFFFF',
      boxShadowFocus: `0 0 0 2px ${colors.primarySuppl}`,
      caretColor: colors.primary,
      loadingColor: colors.primary,
    },
    Button: {
      borderRadius: '8px',
      fontWeight: '600',
      textColorPrimary: '#FFFFFF',
      textColorTertiary: colors.textMain,
      textColorHoverPrimary: colors.textMain,
      textColorHoverTertiary: colors.bgElevated,
      colorHoverPrimary: colors.primary,
      colorHoverSecondary: colors.bgElevated,
      colorPressedPrimary: colors.primary,
      colorPressedSecondary: colors.textMain,
      colorPressedTertiary: colors.bgElevated,
      opacityDisabled: '0.5',
      borderPrimary: colors.primary,
      borderHover: colors.primaryHover,
      borderFocus: colors.primary,
      borderPressed: colors.primaryPressed,
    },
    DataTable: {
      borderRadius: '12px',
      borderColor: colors.border,
      thColor: colors.bgElevated,
      thTextColor: colors.textMain,
      tdColor: 'transparent',
      tdColorStriped: colors.bgElevated,
      tdColorHover: isDark ? 'rgba(255, 255, 255, 0.03)' : 'rgba(0, 0, 0, 0.02)',
      tdTextColor: colors.textMain,
      thFontWeight: '600',
    },
    Menu: {
      borderRadius: '8px',
      itemColor: colors.textMain,
      itemTextColor: colors.textMain,
      itemColorHover: colors.primarySuppl,
      itemColorActive: colors.primary,
      itemIconColor: colors.textMain,
      itemIconColorHover: colors.textMain,
      itemIconColorActive: colors.textMain,
      itemHeightMedium: '42px',
    },
    Select: {
      peers: { InternalSelection: { textColor: colors.textMain } },
      menuColor: colors.bgCard,
      optionColor: 'transparent',
      optionColorHover: colors.bgElevated,
      optionColorActive: colors.primarySuppl,
      optionTextColor: colors.textMain,
      optionTextColorHover: colors.textMain,
      optionTextColorActive: colors.textMain,
      optionIconColor: colors.textMain,
      optionIconColorHover: colors.textMain,
    },
    DatePicker: {
      panelColor: colors.bgCard,
      itemColorHover: colors.bgElevated,
      itemColorActive: colors.primarySuppl,
      itemTextColor: colors.textMain,
      itemTextColorActive: colors.textMain,
      iconColor: colors.textSec,
      iconColorActive: colors.textMain,
      arrowColor: colors.textSec,
      daysColor: colors.textMain,
      headerDividerColor: colors.border,
    },
    Tabs: {
      tabGapMedium: '24px',
      tabGapLarge: '28px',
      tabPaddingMedium: '16px',
      tabPaddingLarge: '20px',
      panePaddingMedium: '16px',
      panePaddingLarge: '20px',
      barColor: colors.primary,
      tabColor: colors.textSec,
      tabColorHover: colors.textMain,
      tabColorActive: colors.primary,
      tabTextColorActive: colors.primary,
      tabFontWeightActive: '600',
    },
    Pagination: {
      itemColor: colors.textMain,
      itemColorHover: colors.primary,
      itemColorActive: colors.primary,
      itemColorDisabled: colors.textSec,
      itemTextColor: colors.textMain,
      itemTextColorHover: colors.primary,
      itemTextColorActive: '#FFFFFF',
      itemTextColorDisabled: colors.textSec,
      itemBorder: colors.border,
      itemBorderHover: colors.primary,
      itemBorderActive: 'transparent',
      itemBorderDisabled: colors.border,
      itemSizeMedium: '36px',
    },
    Breadcrumb: {
      fontSize: '14px',
      itemTextColor: colors.textSec,
      itemTextColorHover: colors.textMain,
      itemTextColorActive: colors.textMain,
      separatorColor: colors.textSec,
    },
    Dropdown: {
      menuColor: colors.bgCard,
      optionColor: 'transparent',
      optionColorHover: colors.bgElevated,
      optionColorActive: colors.primarySuppl,
      optionTextColor: colors.textMain,
      optionTextColorHover: colors.textMain,
      optionTextColorActive: colors.textMain,
      optionIconColor: colors.textMain,
      optionIconColorHover: colors.textMain,
      dividerColor: colors.border,
    },
    Drawer: {
      color: colors.bgCard,
      headerColor: 'transparent',
      bodyColor: 'transparent',
      footerColor: 'transparent',
      boxShadow: isDark
        ? '0 12px 48px rgba(0, 0, 0, 0.6)'
        : '0 12px 48px rgba(0, 0, 0, 0.12)',
    },
    Tooltip: {
      color: colors.bgDark,
      textColor: '#FFFFFF',
      borderRadius: '8px',
      padding: '8px 12px',
      fontSize: '14px',
    },
    Modal: {
      color: colors.bgCard,
      iconColor: colors.textSec,
      closeIconColor: colors.textSec,
      closeIconColorHover: colors.textMain,
      closeSize: '22px',
      titleFontWeight: '600',
    },
    Message: {
      color: colors.bgCard,
      iconColor: colors.textMain,
      closeIconColor: colors.textSec,
      closeIconColorHover: colors.textMain,
      borderRadius: '8px',
      padding: '12px 16px',
      fontSize: '14px',
    },
    Notification: {
      color: colors.bgCard,
      iconColor: colors.textMain,
      closeIconColor: colors.textSec,
      closeIconColorHover: colors.textMain,
      borderRadius: '8px',
      padding: '16px 20px',
    },
    Popover: {
      textColor: colors.textMain,
      color: colors.bgCard,
    },
    Switch: {
      railColor: colors.border,
      railColorActive: colors.primary,
      buttonColor: '#FFFFFF',
      boxShadowFocus: `0 0 0 2px ${colors.primarySuppl}`,
    },
    Checkbox: {
      borderRadius: '4px',
      borderChecked: colors.primary,
      borderFocus: colors.primary,
      checkMarkColor: '#FFFFFF',
      boxShadowFocus: `0 0 0 2px ${colors.primarySuppl}`,
    },
    Radio: {
      boxShadowFocus: `0 0 0 2px ${colors.primarySuppl}`,
      dotColorActive: colors.primary,
      dotColorDisabled: colors.border,
      borderDisabled: colors.border,
      colorDisabled: colors.textSec,
    },
    Slider: {
      railColor: colors.border,
      railColorHover: colors.border,
      fillColor: colors.primary,
      handleColor: '#FFFFFF',
      handleShadow: `0 2px 4px rgba(0, 0, 0, 0.2)`,
      handleBoxShadowFocus: `0 0 0 2px ${colors.primarySuppl}`,
    },
    Progress: {
      railColor: colors.border,
      fillColor: colors.primary,
      iconColor: colors.textSec,
      fontSize: '14px',
    },
    Spin: {
      color: colors.primary,
    },
    Empty: {
      iconColor: colors.textSec,
      textColor: colors.textSec,
      extraTextColor: colors.textSec,
      descriptionTextColor: colors.textSec,
    },
    Tag: {
      borderRadius: '6px',
      padding: '4px 10px',
      fontSize: '12px',
      colorDefault: colors.bgElevated,
      textColorDefault: colors.textMain,
      borderDefault: '1px solid ' + colors.border,
    },
  }
})

// 确保 data-theme 和 Naive UI 主题同步
watch(currentTheme, (newTheme) => {
  // @ts-expect-error - Nuxt's process.client is not in TypeScript types
  if (process.client && typeof document !== 'undefined' && document.documentElement) {
    // 确保 data-theme 属性与当前主题同步
    document.documentElement.setAttribute('data-theme', newTheme)
  }
}, { immediate: true })

// 动态导入 Naive UI 组件，避免 SSR 错误
onMounted(async () => {
  // 双重检查，确保在客户端
  if (typeof window === 'undefined' || typeof document === 'undefined') return

  // @ts-expect-error - Nuxt's process.client is not in TypeScript types
  if (!process.client) return

  // 确保 data-theme 属性已设置
  if (document && document.documentElement) {
    document.documentElement.setAttribute('data-theme', currentTheme.value)
  }

  try {
    // 动态导入 Naive UI，避免 SSR 时执行
    // 使用动态 import 代码分割，减少初始包大小
    const naiveUI = await import(/* webpackChunkName: "naive-ui" */ 'naive-ui')
    const { NMessageProvider, NDialogProvider, NNotificationProvider, NConfigProvider, darkTheme } = naiveUI

    // 创建包装组件
    ProvidersComponent.value = markRaw(defineComponent({
      name: 'AppNaiveConfigWrapper',
      props: {
        theme: Object,
        themeOverrides: Object
      },
      setup(componentProps, { slots }) {
        // 计算实际的主题
        const actualTheme = computed(() => {
          if (currentTheme.value === 'light') {
            return null
          }
          // dark 和 hybrid-super-dark 主题
          if (currentTheme.value === 'dark') {
            return darkTheme
          }
          if (currentTheme.value === 'hybrid-super-dark') {
            // hybrid-super-dark 深色主题使用深色主题作为基础，叠加玻璃态
            return darkTheme
          }
          return null
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
  background: var(--color-bg-body, #020617);
  color: var(--color-text-main, var(--color-text-sub));
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
