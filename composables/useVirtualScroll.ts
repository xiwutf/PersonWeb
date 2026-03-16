/**
 * 虚拟滚动 Composable
 *
 * 用于优化大列表性能，只渲染可见区域的项目
 */

import { ref, computed, onMounted, onUnmounted, nextTick, watch } from 'vue'

// 虚拟滚动配置
export interface VirtualScrollOptions<T> {
  items: T[]
  itemHeight: number | ((item: T) => number)
  containerHeight?: number
  overscan?: number
  buffer?: number
}

// 虚拟滚动状态
interface VirtualScrollState<T> {
  scrollTop: number
  startIndex: number
  endIndex: number
  visibleItems: T[]
  offsetY: number
}

/**
 * 虚拟滚动 Hook
 */
export function useVirtualScroll<T>(options: VirtualScrollOptions<T>) {
  const {
    items,
    itemHeight,
    containerHeight: number,
    overscan = 5,
    buffer = 3
  } = options

  const containerRef = ref<HTMLElement>()
  const scrollTop = ref(0)
  const isScrolling = ref(false)

  // 获取项目高度
  const getItemHeight = (item: T): number => {
    return typeof itemHeight === 'function' ? itemHeight(item) : itemHeight
  }

  // 计算可见项目的索引范围
  const visibleRange = computed(() => {
    const start = Math.floor(scrollTop.value / getItemHeight(items[0]))
    const end = Math.min(
      start + Math.ceil(containerHeight / getItemHeight(items[0])),
      items.length - 1
    )

    return {
      start: Math.max(0, start - overscan),
      end: Math.min(items.length - 1, end + overscan)
    }
  })

  // 可见项目列表
  const visibleItems = computed(() => {
    const { start, end } = visibleRange.value
    return items.slice(start, end + 1)
  })

  // 总高度
  const totalHeight = computed(() => {
    return items.reduce((sum, item) => sum + getItemHeight(item), 0)
  })

  // Y 轴偏移
  const offsetY = computed(() => {
    const { start } = visibleRange.value
    let offset = 0
    for (let i = 0; i < start; i++) {
      offset += getItemHeight(items[i])
    }
    return offset
  })

  // 滚动处理
  const handleScroll = (event: Event) => {
    const target = event.target as HTMLElement
    scrollTop.value = target.scrollTop

    // 节流处理
    if (!isScrolling.value) {
      isScrolling.value = true
      requestAnimationFrame(() => {
        isScrolling.value = false
      })
    }
  }

  // 滚动到指定项目
  const scrollToItem = (index: number) => {
    let offset = 0
    for (let i = 0; i < index; i++) {
      offset += getItemHeight(items[i])
    }

    if (containerRef.value) {
      containerRef.value.scrollTo({
        top: offset,
        behavior: 'smooth'
      })
    }
  }

  // 滚动到顶部
  const scrollToTop = () => {
    if (containerRef.value) {
      containerRef.value.scrollTo({ top: 0, behavior: 'smooth' })
    }
  }

  // 滚动到底部
  const scrollToBottom = () => {
    if (containerRef.value) {
      containerRef.value.scrollTo({
        top: totalHeight.value - containerHeight,
        behavior: 'smooth'
      })
    }
  }

  // 滚动指定距离
  const scrollBy = (delta: number) => {
    if (containerRef.value) {
      containerRef.value.scrollBy({
        top: delta,
        behavior: 'smooth'
      })
    }
  }

  // 刷新虚拟滚动状态（当项目高度变化时）
  const refresh = () => {
    // 触发重新计算
    scrollTop.value = containerRef.value?.scrollTop || 0
  }

  // 组件挂载
  onMounted(async () => {
    await nextTick()
    // 初始滚动位置
    if (containerRef.value) {
      scrollTop.value = containerRef.value.scrollTop
    }
  })

  // 监听项目变化
  watch(
    () => items.length,
    (newLength, oldLength) => {
      if (newLength !== oldLength) {
        // 项目数量变化时刷新
        refresh()
      }
    }
  )

  return {
    containerRef,
    visibleItems,
    visibleRange,
    totalHeight,
    offsetY,
    scrollTop,
    isScrolling,
    handleScroll,
    scrollToItem,
    scrollToTop,
    scrollToBottom,
    scrollBy,
    refresh
  }
}

/**
 * 动态高度的虚拟滚动 Hook
 */
export function useVirtualDynamicScroll<T>(options: VirtualScrollOptions<T>) {
  const containerRef = ref<HTMLElement>()
  const itemRefs = ref<Map<number, HTMLElement>>(new Map())
  const scrollTop = ref(0)
  const itemPositions = ref<number[]>([])
  const containerHeight = ref(0)

  // 计算项目位置
  const updateItemPositions = () => {
    const positions: number[] = []
    let currentOffset = 0

    options.items.forEach((_, index) => {
      positions[index] = currentOffset
      const itemElement = itemRefs.value.get(index)
      if (itemElement) {
        currentOffset += itemElement.offsetHeight
      }
    })

    itemPositions.value = positions
  }

  // 计算可见项目范围
  const visibleRange = computed(() => {
    if (itemPositions.value.length === 0) {
      return { start: 0, end: 0 }
    }

    let start = 0
    let end = 0

    // 二分查找起始位置
    let low = 0
    let high = options.items.length - 1

    while (low <= high) {
      const mid = Math.floor((low + high) / 2)
      if (itemPositions.value[mid] <= scrollTop.value) {
        start = mid + 1
        low = mid + 1
      } else {
        high = mid - 1
        start = mid
      }
    }

    // 计算结束位置
    for (let i = start; i < options.items.length; i++) {
      if (itemPositions.value[i] >= scrollTop.value + containerHeight.value) {
        end = i - 1
        break
      }
      end = options.items.length - 1
    }

    return {
      start: Math.max(0, start - options.overscan),
      end: Math.min(options.items.length - 1, end + options.overscan)
    }
  })

  // 可见项目
  const visibleItems = computed(() => {
    const { start, end } = visibleRange.value
    return options.items.slice(start, end + 1)
  })

  // 总高度
  const totalHeight = computed(() => {
    if (itemPositions.value.length === 0) return 0
    return itemPositions.value[itemPositions.value.length - 1]
  })

  // Y 轴偏移
  const offsetY = computed(() => {
    const { start } = visibleRange.value
    return start > 0 ? itemPositions.value[start - 1] : 0
  })

  // 设置项目引用
  const setItemRef = (index: number, el: Element | null) => {
    if (el) {
      itemRefs.value.set(index, el as HTMLElement)
    }
  }

  // 滚动处理
  const handleScroll = () => {
    if (containerRef.value) {
      scrollTop.value = containerRef.value.scrollTop
    }
  }

  // 刷新项目位置
  const refresh = () => {
    nextTick(() => {
      updateItemPositions()
      if (containerRef.value) {
        containerHeight.value = containerRef.value.clientHeight
      }
    })
  }

  // 组件挂载
  onMounted(async () => {
    await nextTick()
    if (containerRef.value) {
      containerHeight.value = containerRef.value.clientHeight
    }
    refresh()
  })

  // 监听容器大小变化
  const resizeObserver = ref<ResizeObserver>()

  onMounted(() => {
    if (containerRef.value) {
      resizeObserver.value = new ResizeObserver(() => {
        containerHeight.value = containerRef.value.clientHeight
        refresh()
      })
      resizeObserver.value.observe(containerRef.value)
    }
  })

  // 组件卸载
  onUnmounted(() => {
    resizeObserver.value?.disconnect()
  })

  return {
    containerRef,
    visibleItems,
    visibleRange,
    totalHeight,
    offsetY,
    setItemRef,
    handleScroll,
    refresh
  }
}

// 导出类型
export type { VirtualScrollOptions, VirtualScrollState }
