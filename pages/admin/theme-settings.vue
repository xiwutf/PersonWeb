<template>
  <div class="theme-settings-page">
    <div class="page-header">
      <h1 class="page-title">主题设置</h1>
      <p class="page-subtitle">自定义后台管理的主题和样式</p>
    </div>

    <div class="settings-grid">
      <!-- 主题模式设置 -->
      <n-card title="主题模式" class="settings-card">
        <n-space vertical :size="16">
          <n-radio-group v-model:value="themeModeValue" @update:value="handleThemeModeChange">
            <n-space vertical>
              <n-radio value="light">
                <template #icon>
                  <i class="fas fa-sun text-yellow-500"></i>
                </template>
                浅色模式
              </n-radio>
              <n-radio value="dark">
                <template #icon>
                  <i class="fas fa-moon text-blue-500"></i>
                </template>
                深色模式
              </n-radio>
              <n-radio value="auto">
                <template #icon>
                  <i class="fas fa-adjust text-gray-500"></i>
                </template>
                跟随系统
              </n-radio>
            </n-space>
          </n-radio-group>
          <n-alert type="info" :show-icon="false">
            当前主题：<strong>{{ currentThemeName }}</strong>
          </n-alert>
        </n-space>
      </n-card>

      <!-- 主题色设置 -->
      <n-card title="主题色" class="settings-card">
        <n-space vertical :size="16">
          <div class="preset-colors">
            <div 
              v-for="(color, name) in presetColors" 
              :key="name"
              class="color-item"
              :class="{ active: isPresetColorActive(name) }"
              @click="handlePresetColorClick(name as keyof typeof presetColors)"
            >
              <div class="color-preview" :style="{ backgroundColor: color }"></div>
              <span class="color-name">{{ getColorName(name) }}</span>
            </div>
          </div>
          
          <n-divider />
          
          <div class="custom-color">
            <n-space vertical :size="8">
              <label class="label">自定义主题色</label>
              <n-color-picker 
                v-model:value="customColor" 
                :modes="['hex']"
                @update:value="handleCustomColorChange"
              />
            </n-space>
          </div>
        </n-space>
      </n-card>

      <!-- 预览 -->
      <n-card title="预览" class="settings-card preview-card">
        <div class="preview-content">
          <div class="preview-buttons">
            <n-button type="primary">主要按钮</n-button>
            <n-button>默认按钮</n-button>
            <n-button type="success">成功按钮</n-button>
            <n-button type="warning">警告按钮</n-button>
            <n-button type="error">错误按钮</n-button>
          </div>
          
          <n-divider />
          
          <div class="preview-tags">
            <n-tag type="primary">标签</n-tag>
            <n-tag type="success">成功</n-tag>
            <n-tag type="warning">警告</n-tag>
            <n-tag type="error">错误</n-tag>
          </div>
          
          <n-divider />
          
          <div class="preview-inputs">
            <n-input placeholder="输入框示例" />
            <n-select placeholder="选择器示例" :options="[]" />
          </div>
        </div>
      </n-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { NCard, NSpace, NRadioGroup, NRadio, NAlert, NDivider, NColorPicker, NButton, NTag, NInput, NSelect } from 'naive-ui'
import { useNaiveTheme } from '~/composables/useNaiveTheme'
import { useSafeMessage } from '~/composables/useNaiveUI'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // 禁用 SSR，避免 Naive UI 组件在服务端渲染时出错
})

// 使用安全的 message composable，支持服务端渲染
const message = useSafeMessage()
const { themeMode, currentTheme, themeOverrides, setThemeMode, setThemeColor, applyPresetColor, presetColors } = useNaiveTheme()

// 创建响应式的主题模式值（因为 themeMode 是 computed）
const themeModeValue = ref(themeMode.value)
watch(themeMode, (newVal) => {
  themeModeValue.value = newVal
}, { immediate: true })

const customColor = ref(themeOverrides.value.common?.primaryColor || '#3b82f6')

// 当前主题名称
const currentThemeName = computed(() => {
  const mode = themeModeValue.value
  if (mode === 'light') return '浅色模式'
  if (mode === 'dark') return '深色模式'
  return '跟随系统'
})

// 检查预设颜色是否激活
const isPresetColorActive = (name: string) => {
  const currentColor = themeOverrides.value.common?.primaryColor
  return presetColors[name as keyof typeof presetColors] === currentColor
}

// 获取颜色中文名称
const getColorName = (name: string) => {
  const names: Record<string, string> = {
    blue: '蓝色',
    green: '绿色',
    purple: '紫色',
    orange: '橙色',
    red: '红色',
    cyan: '青色',
    pink: '粉色',
    indigo: '靛蓝'
  }
  return names[name] || name
}

// 处理主题模式变化
const handleThemeModeChange = (mode: 'light' | 'dark' | 'auto') => {
  setThemeMode(mode)
  message.success(`已切换到${currentThemeName.value}`)
}

// 处理自定义颜色变化
const handleCustomColorChange = (color: string) => {
  setThemeColor(color)
  message.success('主题色已更新')
}

// 应用预设颜色
const handlePresetColorClick = (name: keyof typeof presetColors) => {
  applyPresetColor(name)
  customColor.value = presetColors[name]
  message.success(`已应用${getColorName(name)}主题`)
}

// 监听主题色变化，更新自定义颜色选择器
watch(() => themeOverrides.value.common?.primaryColor, (color) => {
  if (color) {
    customColor.value = color
  }
})
</script>

<style scoped>
.theme-settings-page {
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 2rem;
}

.page-title {
  font-size: 2rem;
  font-weight: bold;
  margin-bottom: 0.5rem;
  color: var(--color-text-main);
}

.page-subtitle {
  color: var(--color-text-muted);
  font-weight: 500;
}

.settings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.settings-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
}

.preset-colors {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.color-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  border: 2px solid transparent;
  background: var(--color-bg-elevated);
}

.color-item:hover {
  border-color: var(--color-border-default);
}

.color-item.active {
  border-color: var(--color-primary);
  background: var(--color-primary-soft);
}

.color-preview {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  border: 2px solid var(--color-border-subtle);
}

.color-name {
  font-size: 0.875rem;
  color: var(--color-text-main);
  font-weight: 500;
}

.custom-color {
  margin-top: 1rem;
}

.label {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--color-text-main);
}

.preview-card {
  grid-column: 1 / -1;
}

.preview-content {
  @apply space-y-4;
}

.preview-buttons,
.preview-tags,
.preview-inputs {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.preview-inputs {
  flex-direction: column;
}

.preview-inputs > * {
  max-width: 300px;
}
</style>

