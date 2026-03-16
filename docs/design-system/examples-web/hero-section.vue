<!-- HeroSection 交互式示例 -->
<template>
  <div class="example-container">
    <div class="example-header">
      <h2>HeroSection 示例</h2>
      <p>英雄区块组件交互式演示</p>
    </div>

    <div class="variant-selector">
      <n-space vertical :size="12">
        <div>
          <span class="label">选择变体：</span>
          <n-radio-group v-model:value="selectedVariant" :name="'variant'">
            <n-radio-button
              v-for="variant in variants"
              :key="variant.value"
              :value="variant.value"
            >
              {{ variant.label }}
            </n-radio-button>
          </n-radio-group>
        </div>

        <div>
          <span class="label">显示徽章：</span>
          <n-switch v-model:value="showBadge" />
          <span v-if="showBadge" class="ml-2">
            <n-select v-model:value="badgeVariant" size="small" style="width: 120px">
              <n-option v-for="v in badgeVariants" :key="v.value" :value="v.value">
                {{ v.label }}
              </n-option>
            </n-select>
          </span>
        </div>
      </n-space>
    </div>

    <div class="preview-area">
      <HeroSection
        :variant="selectedVariant"
        :badge="showBadge ? badgeText : undefined"
        :badge-variant="badgeVariant"
        :title="titles[selectedVariant]"
        :description="descriptions[selectedVariant]"
        :actions="actions"
        class="hero-preview"
      >
        <template #visual v-if="selectedVariant !== 'gradient'">
          <div class="hero-visual">
            <n-icon :size="80" :component="RocketIcon" />
          </div>
        </template>
      </HeroSection>
    </div>

    <div class="code-example">
      <n-card title="代码示例" size="small">
        <pre><code>{{ codeExample }}</code></pre>
      </n-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Rocket as RocketIcon } from '@vicons/ionicons5'
import HeroSection from '~/components/web/HeroSection.vue'

const variants = [
  { value: 'default', label: '默认' },
  { value: 'gradient', label: '渐变' },
  { value: 'dark', label: '深色' },
  { value: 'light', label: '浅色' }
]

const badgeVariants = [
  { value: 'info', label: 'Info' },
  { value: 'success', label: 'Success' },
  { value: 'warning', label: 'Warning' },
  { value: 'error', label: 'Error' }
]

const titles = {
  default: '欢迎来到 Aurora',
  gradient: '构建无限可能',
  dark: '深色模式体验',
  light: '浅色清爽风格'
}

const descriptions = {
  default: '探索设计系统的强大功能，创建出色的用户体验',
  gradient: 'AI 驱动的智能应用，让您的创意变为现实',
  dark: '沉浸式深色主题，保护您的眼睛，提升专注度',
  light: '简洁清爽的视觉风格，轻松愉悦的使用体验'
}

const selectedVariant = ref<'default' | 'gradient' | 'dark' | 'light'>('default')
const showBadge = ref(true)
const badgeVariant = ref<'info' | 'success' | 'warning' | 'error'>('info')

const badgeText = computed(() => {
  if (!showBadge.value) return ''
  return 'NEW'
})

const actions = computed(() => [
  {
    text: '开始使用',
    icon: 'fa-rocket',
    variant: 'primary',
    to: '#'
  },
  {
    text: '了解更多',
    icon: 'fa-info-circle',
    variant: 'secondary',
    to: '#'
  }
])

const codeExample = computed(() => `<HeroSection
  ${selectedVariant.value !== 'default' ? `variant="${selectedVariant.value}"` : ''}
  ${showBadge.value ? `badge="${badgeText.value}" badge-variant="${badgeVariant.value}"` : ''}
  title="${titles[selectedVariant.value]}"
  description="${descriptions[selectedVariant.value]}"
  :actions="actions"
/>`)
</script>

<style scoped>
.example-container {
  min-height: 100vh;
  background: var(--color-bg-body);
  padding: var(--spacing-xl);
}

.example-header {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
}

.example-header h2 {
  font-size: var(--text-3xl);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.example-header p {
  font-size: var(--text-lg);
  color: var(--color-text-sec);
  margin: 0;
}

.variant-selector {
  max-width: 600px;
  margin: 0 auto var(--spacing-2xl);
  padding: var(--spacing-lg);
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  border: 1px solid var(--color-border-default);
}

.label {
  font-weight: 600;
  color: var(--color-text-main);
}

.preview-area {
  margin-bottom: var(--spacing-2xl);
}

.hero-visual {
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-primary);
}

.code-example {
  max-width: 800px;
  margin: 0 auto;
}

.code-example pre {
  margin: 0;
  padding: var(--spacing-md);
  background: var(--color-bg-code);
  border-radius: var(--radius-md);
  font-family: var(--font-mono);
  font-size: var(--text-sm);
  overflow-x: auto;
}
</style>
