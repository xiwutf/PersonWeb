<template>
  <NConfigProvider
    :theme="naiveTheme"
    :theme-overrides="naiveThemeOverrides"
  >
    <NMessageProvider>
      <NDialogProvider>
        <NNotificationProvider>
          <div class="h-full flex flex-col flex-1">
            <slot />
          </div>
        </NNotificationProvider>
      </NDialogProvider>
    </NMessageProvider>
  </NConfigProvider>
</template>

<script setup lang="ts">
/**
 * AppNaiveConfig - 统一的 Naive UI 配置容器
 *
 * 职责：
 * - 统一管理 Naive UI 的主题配置（前台 + 后台共用）
 * - 基于 useTheme().currentTheme 计算 Naive 主题
 * - 确保 data-theme 和 Naive UI 主题同步
 * - 根据模式提供不同层级的 Providers
 */
import { computed, watch } from 'vue'
import { NConfigProvider, NMessageProvider, NDialogProvider, NNotificationProvider, darkTheme, type GlobalTheme, type GlobalThemeOverrides } from 'naive-ui'
import { useTheme } from '~/composables/useTheme'

interface Props {
  /** 模式：theme=仅主题配置 | full=完整Providers */
  mode?: 'theme' | 'full'
}

const props = withDefaults(defineProps<Props>(), {
  mode: 'theme'
})

// 使用全局主题管理 composable
const { currentTheme } = useTheme()
const naiveTheme = computed<GlobalTheme | null>(() => {
  if (currentTheme.value === 'dark' || currentTheme.value === 'hybrid-super-dark') {
    return darkTheme
  }
  return null
})

/**
 * Naive UI 主题覆盖配置
 */
const naiveThemeOverrides = computed<GlobalThemeOverrides>(() => {
  const isDark = currentTheme.value === 'dark' || currentTheme.value === 'hybrid-super-dark'

  // Aurora Design System V3 颜色
  const v3Colors = {
    primary: '#2997FF',
    primaryHover: '#60A5FA',
    primaryPressed: '#1E40AF',
    primarySuppl: 'rgba(41, 151, 255, 0.15)',
    bgBody: '#050510',
    bgCard: 'rgba(20, 25, 40, 0.6)',
    bgElevated: 'rgba(40, 50, 70, 0.6)',
    bgDark: '#0A0A1A',
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
  if (process.client && typeof document !== 'undefined' && document.documentElement) {
    document.documentElement.setAttribute('data-theme', newTheme)
  }
}, { immediate: true })

</script>

<style scoped>
/* 加载中的样式 */
.app-naive-config-loading {
  width: 100%;
  min-height: 100%;
  background: transparent;
}
</style>
