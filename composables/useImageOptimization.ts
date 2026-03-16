/**
 * 图片优化 Composable
 *
 * 提供图片懒加载、占位符、错误处理等功能
 */

import { ref, onMounted, onUnmounted } from 'vue'

// 图片优化配置
export interface ImageOptimizationOptions {
  src?: string
  placeholder?: string
  lazy?: boolean
  threshold?: number
  format?: 'webp' | 'jpeg' | 'png' | 'auto'
  quality?: number
  fallback?: string
  alt?: string
  onLoad?: () => void
  onError?: () => void
}

// Intersection Observer 配置
interface IntersectionObserverOptions {
  root?: Element | null
  rootMargin?: string
  threshold?: number | number[]
}

// 图片状态类型
type ImageState = 'loading' | 'loaded' | 'error' | 'idle'

/**
 * 图片懒加载 Hook
 */
export function useImageOptimization(options: ImageOptimizationOptions = {}) {
  const {
    src = '',
    placeholder,
    lazy = true,
    threshold = 0.1,
    format = 'auto',
    quality = 85,
    fallback = '',
    alt = '',
    onLoad,
    onError
  } = options

  const state = ref<ImageState>('idle')
  const isLoaded = ref(false)
  const hasError = ref(false)
  const imageRef = ref<HTMLImageElement>()
  const observerRef = ref<IntersectionObserver>()

  // 优化图片 URL（转换为 WebP）
  const optimizedSrc = computed(() => {
    if (!src) return ''

    // 已经过优化或不需要优化
    if (format === 'auto' || src.includes('.webp') || !src.match(/\.(jpg|jpeg|png)$/i)) {
      return src
    }

    // 转换为 WebP
    return src.replace(/\.(jpg|jpeg|png)$/i, '.webp')
  })

  // 处理图片加载
  const handleLoad = () => {
    state.value = 'loaded'
    isLoaded.value = true
    hasError.value = false
    onLoad?.()
  }

  // 处理图片错误
  const handleError = () => {
    state.value = 'error'
    hasError.value = true

    // 如果使用 WebP 失败，尝试回退到原格式
    if (src.includes('.webp')) {
      const originalSrc = src.replace('.webp', '.jpg')
      if (imageRef.value) {
        imageRef.value.src = originalSrc
      }
    } else if (fallback && imageRef.value) {
      imageRef.value.src = fallback
    }

    onError?.()
  }

  // 初始化 Intersection Observer
  const initIntersectionObserver = () => {
    if (!lazy || !imageRef.value || !('IntersectionObserver' in window)) {
      // 不需要懒加载或浏览器不支持，直接加载
      if (imageRef.value) {
        imageRef.value.src = optimizedSrc.value
      }
      return
    }

    const observerOptions: IntersectionObserverOptions = {
      root: null,
      rootMargin: '50px',
      threshold
    }

    observerRef.value = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          // 图片进入视口，开始加载
          if (imageRef.value) {
            state.value = 'loading'
            imageRef.value.src = optimizedSrc.value
          }
          // 停止观察
          observerRef.value?.unobserve(entry.target)
        }
      })
    }, observerOptions)

    // 开始观察
    observerRef.value.observe(imageRef.value)
  }

  // 预加载图片
  const preloadImage = (url: string): Promise<void> => {
    return new Promise((resolve, reject) => {
      const img = new Image()
      img.onload = () => resolve()
      img.onerror = () => reject(new Error(`Failed to preload: ${url}`))
      img.src = url
    })
  }

  // 批量预加载
  const preloadImages = async (urls: string[]) => {
    try {
      await Promise.all(urls.map(preloadImage))
      console.log(`Successfully preloaded ${urls.length} images`)
    } catch (error) {
      console.error('Failed to preload images:', error)
    }
  }

  // 生成占位符 URL（使用 Base64）
  const getPlaceholder = (width = 100, height = 100, color = '#e5e7eb'): string => {
    const canvas = document.createElement('canvas')
    canvas.width = width
    canvas.height = height
    const ctx = canvas.getContext('2d')

    if (ctx) {
      ctx.fillStyle = color
      ctx.fillRect(0, 0, width, height)
    }

    return canvas.toDataURL('image/png')
  }

  // 检查图片格式支持
  const checkWebPSupport = (): boolean => {
    const canvas = document.createElement('canvas')
    return canvas.toDataURL('image/webp').indexOf('data:image/webp') === 0
  }

  // 清理资源
  const cleanup = () => {
    observerRef.value?.disconnect()
    observerRef.value = undefined
  }

  // 组件挂载时初始化
  onMounted(() => {
    if (imageRef.value) {
      // 设置初始占位符
      if (placeholder) {
        imageRef.value.src = placeholder
      } else if (!lazy) {
        // 不使用懒加载时，直接设置 src
        imageRef.value.src = optimizedSrc.value
      }

      // 初始化 Intersection Observer
      initIntersectionObserver()
    }
  })

  // 组件卸载时清理
  onUnmounted(() => {
    cleanup()
  })

  return {
    imageRef,
    state,
    isLoaded,
    hasError,
    optimizedSrc,
    handleLoad,
    handleError,
    preloadImage,
    preloadImages,
    getPlaceholder,
    checkWebPSupport
  }
}

/**
 * 图片懒加载指令
 */
export const vLazy = {
  mounted(el: HTMLImageElement, binding: any) {
    const { src, placeholder, threshold = 0.1 } = binding.value || {}

    if (!src) {
      el.src = placeholder || ''
      return
    }

    el.dataset.src = src
    el.src = placeholder || getPlaceholder(100, 100)

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          el.src = src
          el.classList.add('lazy-loaded')
          observer.unobserve(entry.target)
        }
      })
    }, {
      rootMargin: '50px',
      threshold
    })

    observer.observe(el)

    // 保存 observer 引用用于清理
    ;(el as any)._lazyObserver = observer
  },

  unmounted(el: HTMLImageElement) {
    const observer = (el as any)._lazyObserver
    if (observer) {
      observer.disconnect()
      delete (el as any)._lazyObserver
    }
  }
}

/**
 * 响应式图片指令
 */
export const vResponsive = {
  mounted(el: HTMLImageElement, binding: any) {
    const { src, sizes = '100vw' } = binding.value || {}

    if (!src) return

    const updateSrc = () => {
      const width = el.offsetWidth
      if (!width) return

      // 根据容器宽度选择合适的图片
      const breakpoints = [
        { width: 320, suffix: '@1x' },
        { width: 640, suffix: '@2x' },
        { width: 1024, suffix: '@3x' }
      ]

      const selected = breakpoints.reduce((prev, curr) =>
        width >= curr.width ? curr : prev
      )

      const url = src.replace(/\.(jpg|jpeg|png|webp)$/i, `${selected.suffix}.$1`)

      if (el.src !== url) {
        el.src = url
      }
    }

    // 监听容器大小变化
    const resizeObserver = new ResizeObserver(updateSrc)
    resizeObserver.observe(el)

    ;(el as any)._resizeObserver = resizeObserver
  },

  unmounted(el: HTMLImageElement) {
    const observer = (el as any)._resizeObserver
    if (observer) {
      observer.disconnect()
      delete (el as any)._resizeObserver
    }
  }
}
