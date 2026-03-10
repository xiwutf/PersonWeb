<template>
  <div
    class="lazy-image-container"
    :style="containerStyle"
    @mouseenter="handleMouseEnter"
    @mouseleave="handleMouseLeave"
  >
    <!-- 占位元素 -->
    <div v-if="!loaded" class="lazy-placeholder" :style="placeholderStyle">
      <div class="skeleton-loader">
        <div class="shimmer"></div>
      </div>
    </div>

    <!-- 实际图片 -->
    <img
      v-if="loaded"
      :src="displaySrc"
      :alt="alt"
      :class="imageClass"
      :style="imageStyle"
      @load="handleLoad"
      @error="handleError"
      @mouseenter="handleMouseEnter"
      @mouseleave="handleMouseLeave"
    />

    <!-- 加载失败的占位图 -->
    <div v-if="error && !loaded" class="error-placeholder">
      <slot name="error">
        <div class="error-icon">🖼️</div>
        <div class="error-text">图片加载失败</div>
      </slot>
    </div>

    <!-- 悬停效果遮罩 -->
    <div
      v-if="showHover && loaded"
      class="image-hover-overlay"
      :class="{ 'show': isHovering }"
    >
      <slot name="hover">
        <div class="hover-actions">
          <button
            v-if="downloadUrl"
            class="hover-action"
            @click="handleDownload"
            title="下载图片"
          >
            <i class="fas fa-download"></i>
          </button>
          <button
            class="hover-action"
            @click="handleZoom"
            title="查看大图"
          >
            <i class="fas fa-search-plus"></i>
          </button>
        </div>
      </slot>
    </div>

    <!-- 图片预览模态框 -->
    <Teleport to="body">
      <Transition name="fade">
        <div
          v-if="showPreview"
          class="image-preview-modal"
          @click="closePreview"
        >
          <div class="preview-content">
            <img
              :src="displaySrc"
              :alt="alt"
              @click.stop
            />
            <button class="close-btn" @click="closePreview">
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'

// Props
interface Props {
  src?: string
  alt?: string
  placeholder?: string
  width?: number | string
  height?: number | string
  class?: string
  lazy?: boolean
  threshold?: number
  rootMargin?: string
  downloadUrl?: string
  showHover?: boolean
  placeholderColor?: string
}

const props = withDefaults(defineProps<Props>(), {
  src: '',
  alt: '',
  placeholder: '/images/placeholder.png',
  width: '100%',
  height: 'auto',
  class: '',
  lazy: true,
  threshold: 0.1,
  rootMargin: '50px',
  downloadUrl: '',
  showHover: true,
  placeholderColor: '#e5e7eb'
})

// Emits
const emit = defineEmits<{
  load: []
  error: []
  download: []
  zoom: []
}>()

// 状态
const loaded = ref(false)
const error = ref(false)
const isHovering = ref(false)
const observer = ref<IntersectionObserver | null>(null)
const showPreview = ref(false)

// 计算属性
const containerStyle = computed(() => ({
  position: 'relative',
  overflow: 'hidden',
  width: typeof props.width === 'number' ? `${props.width}px` : props.width,
  height: typeof props.height === 'number' ? `${props.height}px` : props.height,
  backgroundColor: props.placeholderColor
}))

const placeholderStyle = computed(() => ({
  position: 'absolute',
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center'
}))

const imageClass = computed(() => [
  'lazy-image',
  props.class,
  { 'loaded': loaded.value }
])

const imageStyle = computed(() => ({
  width: '100%',
  height: '100%',
  objectFit: 'cover',
  transition: 'opacity 0.3s ease'
}))

const displaySrc = computed(() => {
  if (!loaded.value || error.value) {
    return props.placeholder
  }
  return props.src
})

// 方法
const handleLoad = () => {
  loaded.value = true
  error.value = false
  emit('load')
}

const handleError = () => {
  error.value = true
  loaded.value = false
  emit('error')
}

const handleMouseEnter = () => {
  isHovering.value = true
}

const handleMouseLeave = () => {
  isHovering.value = false
}

const handleDownload = () => {
  if (props.downloadUrl) {
    window.open(props.downloadUrl, '_blank')
  } else {
    const link = document.createElement('a')
    link.href = props.src
    link.download = props.alt || 'image'
    link.click()
  }
  emit('download')
}

const handleZoom = () => {
  showPreview.value = true
  emit('zoom')
}

const closePreview = () => {
  showPreview.value = false
}

// 生命周期
onMounted(() => {
  if (props.lazy) {
    // 创建 Intersection Observer
    observer.value = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          // 图片进入视口，加载图片
          loaded.value = true
          observer.value?.unobserve(entry.target)
        }
      })
    }, {
      root: null,
      rootMargin: props.rootMargin,
      threshold: props.threshold
    })

    // 延迟创建观察者，避免立即触发
    setTimeout(() => {
      const image = document.querySelector('.lazy-image-container img')
      if (image && !loaded.value) {
        observer.value?.observe(image)
      }
    }, 100)
  } else {
    // 立即加载图片
    loaded.value = true
  }
})

onUnmounted(() => {
  observer.value?.disconnect()
})
</script>

<style scoped>
.lazy-image-container {
  position: relative;
  overflow: hidden;
}

.lazy-placeholder {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.skeleton-loader {
  width: 100%;
  height: 100%;
  position: relative;
  overflow: hidden;
}

.shimmer {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.4),
    transparent
  );
  animation: shimmer 2s infinite;
}

@keyframes shimmer {
  0% {
    left: -100%;
  }
  100% {
    left: 100%;
  }
}

.lazy-image {
  opacity: 0;
  transition: opacity 0.3s ease;
}

.lazy-image.loaded {
  opacity: 1;
}

.error-placeholder {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #6b7280;
  font-size: 14px;
  text-align: center;
  padding: 20px;
}

.error-icon {
  font-size: 48px;
  margin-bottom: 8px;
  opacity: 0.5;
}

.error-text {
  font-size: 14px;
  opacity: 0.7;
}

.image-hover-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
  pointer-events: none;
}

.image-hover-overlay.show {
  opacity: 1;
  pointer-events: auto;
}

.hover-actions {
  display: flex;
  gap: 12px;
}

.hover-action {
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #374151;
}

.hover-action:hover {
  background: white;
  transform: scale(1.1);
}

.hover-action i {
  font-size: 16px;
}

.image-preview-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  padding: 20px;
}

.preview-content {
  position: relative;
  max-width: 90vw;
  max-height: 90vh;
}

.preview-content img {
  max-width: 100%;
  max-height: 90vh;
  object-fit: contain;
  border-radius: 8px;
}

.close-btn {
  position: absolute;
  top: -40px;
  right: 0;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  border-radius: 50%;
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  color: white;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .hover-action {
    width: 32px;
    height: 32px;
  }

  .hover-action i {
    font-size: 14px;
  }

  .image-preview-modal {
    padding: 10px;
  }

  .close-btn {
    width: 32px;
    height: 32px;
    top: -36px;
  }
}
</style>