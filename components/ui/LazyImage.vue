<template>
  <div
    ref="containerRef"
    class="lazy-image"
    :class="{
      'lazy-image--loaded': state === 'loaded',
      'lazy-image--error': state === 'error',
      'lazy-image--clickable': clickable
    }"
    :style="{ aspectRatio, width, height }"
    @click="handleClick"
  >
    <!-- 占位符 -->
    <div
      v-if="state !== 'loaded'"
      class="lazy-image-placeholder"
      :style="{ background: placeholderStyle }"
    >
      <!-- 加载动画 -->
      <div v-if="state === 'loading'" class="lazy-image-loader"></div>

      <!-- 占位符内容 -->
      <slot name="placeholder">
        <span v-if="placeholderText" class="lazy-image-placeholder-text">
          {{ placeholderText }}
        </span>
      </slot>

      <!-- 默认占位符图标 -->
      <div v-if="!$slots.placeholder && !placeholderStyle" class="lazy-image-icon">
        <n-icon :component="ImageIcon" />
      </div>
    </div>

    <!-- 实际图片 -->
    <img
      v-show="state === 'loaded'"
      ref="imageRef"
      :src="displaySrc"
      :alt="alt"
      :loading="lazy ? 'lazy' : 'eager'"
      :decoding="decoding"
      class="lazy-image-img"
      :style="{ objectFit, objectPosition }"
      @load="handleLoad"
      @error="handleError"
    />

    <!-- 错误状态 -->
    <div v-if="state === 'error'" class="lazy-image-error">
      <n-icon :component="ErrorIcon" size="48" />
      <p class="lazy-image-error-text">加载失败</p>
      <button
        v-if="retryable"
        class="lazy-image-retry"
        type="button"
        @click="handleRetry"
      >
        重试
      </button>
    </div>

    <!-- 图片加载完成后的内容覆盖 -->
    <div v-if="state === 'loaded' && $slots.overlay" class="lazy-image-overlay">
      <slot name="overlay" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useImageOptimization, vLazy, vResponsive } from '~/composables/useImageOptimization'
import { NIcon } from 'naive-ui'
import { Image as ImageIcon, Refresh as ErrorIcon } from '@vicons/ionicons5'

export interface LazyImageProps {
  src: string
  alt?: string
  placeholder?: string
  placeholderText?: string
  placeholderStyle?: string
  width?: string | number
  height?: string | number
  aspectRatio?: string
  objectFit?: 'cover' | 'contain' | 'fill' | 'none' | 'scale-down'
  objectPosition?: string
  lazy?: boolean
  eager?: boolean
  decoding?: 'sync' | 'async' | 'auto'
  retryable?: boolean
  clickable?: boolean
  quality?: number
  format?: 'webp' | 'jpeg' | 'png' | 'auto'
  onLoad?: () => void
  onError?: () => void
}

const props = withDefaults(defineProps<LazyImageProps>(), {
  alt: '',
  lazy: true,
  eager: false,
  decoding: 'async',
  retryable: true,
  clickable: false,
  quality: 85,
  format: 'auto'
})

// 使用图片优化
const {
  imageRef,
  state,
  optimizedSrc,
  handleLoad,
  handleError,
  checkWebPSupport
} = useImageOptimization({
  src: props.src,
  lazy: props.lazy,
  format: props.format,
  quality: props.quality,
  fallback: props.placeholder,
  alt: props.alt,
  onLoad: props.onLoad,
  onError: props.onError
})

// 显示的图片源
const displaySrc = computed(() => {
  // 如果 eager 或不是 lazy，使用优化后的源
  if (props.eager || !props.lazy) {
    return optimizedSrc.value
  }
  // 否则由 vLazy 指令控制
  return props.src
})

// 样式计算
const width = computed(() => {
  if (props.width) return typeof props.width === 'number' ? `${props.width}px` : props.width
  return undefined
})

const height = computed(() => {
  if (props.height) return typeof props.height === 'number' ? `${props.height}px` : props.height
  return undefined
})

const aspectRatio = computed(() => {
  if (props.aspectRatio) return props.aspectRatio
  return undefined
})

// 容器引用（用于 vLazy 和 vResponsive）
const containerRef = ref<HTMLElement>()

// 点击处理
const handleClick = () => {
  if (props.clickable && state.value === 'loaded') {
    // 可以在这里添加点击处理逻辑
  }
}

// 重试加载
const handleRetry = () => {
  if (imageRef.value) {
    imageRef.value.src = ''
    setTimeout(() => {
      imageRef.value.src = optimizedSrc.value
    }, 100)
  }
}

// 检查 WebP 支持并设置优化格式
checkWebPSupport()
</script>

<style scoped>
.lazy-image {
  position: relative;
  overflow: hidden;
  background: var(--color-bg-dark);
  border-radius: var(--radius-lg);
}

.lazy-image--clickable {
  cursor: pointer;
  transition: transform 0.2s;
}

.lazy-image--clickable:hover {
  transform: scale(1.02);
}

.lazy-image--clickable:active {
  transform: scale(0.98);
}

/* 占位符 */
.lazy-image-placeholder {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background-size: cover;
  background-position: center;
}

.lazy-image-placeholder-text {
  color: var(--color-text-muted);
  font-size: var(--text-sm);
}

.lazy-image-icon {
  color: var(--color-text-muted);
  font-size: 32px;
}

/* 加载动画 */
.lazy-image-loader {
  width: 40px;
  height: 40px;
  border: 3px solid var(--color-border-subtle);
  border-top-color: var(--color-primary);
  border-radius: var(--radius-full);
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

/* 实际图片 */
.lazy-image-img {
  width: 100%;
  height: 100%;
  display: block;
  transition: opacity 0.3s;
}

.lazy-image--loaded .lazy-image-img {
  opacity: 1;
}

/* 错误状态 */
.lazy-image-error {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: var(--color-error);
  background: var(--color-bg-elevated);
}

.lazy-image-error-text {
  margin-top: var(--spacing-md);
  font-size: var(--text-sm);
}

.lazy-image-retry {
  margin-top: var(--spacing-lg);
  padding: var(--spacing-sm) var(--spacing-lg);
  background: var(--color-bg-card);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: all 0.2s;
}

.lazy-image-retry:hover {
  background: var(--color-primary);
  color: white;
  border-color: var(--color-primary);
}

/* 覆盖层 */
.lazy-image-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* 性能优化 */
.lazy-image-img {
  contain: layout style;
}

.lazy-image-placeholder {
  contain: strict;
}
</style>
