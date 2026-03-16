<template>
  <div
    ref="containerRef"
    class="virtual-list"
    :style="{ height: containerHeight }"
    @scroll="handleScroll"
  >
    <!-- 上方缓冲区占位 -->
    <div
      class="virtual-list-spacer"
      :style="{ height: `${offsetY}px` }"
    ></div>

    <!-- 可见项目列表 -->
    <div
      v-for="item in visibleItems"
      :key="itemKey(item)"
      class="virtual-list-item"
      :style="{ height: `${itemHeight}px` }"
    >
      <slot name="item" :item="item" :index="getIndex(item)" />
    </div>

    <!-- 下方缓冲区占位 -->
    <div
      class="virtual-list-spacer"
      :style="{ height: `${totalHeight - offsetY - visibleHeight}px` }"
    ></div>

    <!-- 滚动指示器 -->
    <div
      v-if="showScrollIndicator"
      class="virtual-list-scrollbar"
    :style="{ opacity: isScrolling ? 1 : 0 }"
    >
      <div
        class="virtual-list-scrollbar-thumb"
        :style="{ height: scrollThumbHeight, top: scrollThumbTop }"
      ></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useVirtualScroll } from '~/composables/useVirtualScroll'

export interface VirtualListProps<T> {
  items: T[]
  itemKey: (item: T) => string | number
  itemHeight: number | ((item: T) => number)
  containerHeight?: number
  overscan?: number
  buffer?: number
  showScrollIndicator?: boolean
}

const props = withDefaults(defineProps<VirtualListProps<any>>(), {
  itemHeight: 50,
  containerHeight: 600,
  overscan: 5,
  buffer: 3,
  showScrollIndicator: true
})

// 获取项目键值
const getIndex = (item: any): number => {
  const key = props.itemKey(item)
  if (typeof key === 'number') {
    return key
  }
  // 假设 items 是数组，通过 index 查找
  return props.items.indexOf(item)
}

// 使用虚拟滚动
const {
  containerRef,
  visibleItems,
  visibleRange,
  totalHeight,
  offsetY,
  isScrolling,
  handleScroll
} = useVirtualScroll({
  items: props.items,
  itemHeight: props.itemHeight,
  containerHeight: props.containerHeight,
  overscan: props.overscan,
  buffer: props.buffer
})

// 计算可见高度
const visibleHeight = computed(() => {
  const { start, end } = visibleRange.value
  let height = 0
  for (let i = start; i <= end; i++) {
    height += props.itemHeight
  }
  return height
})

// 滚动指示器
const scrollThumbHeight = computed(() => {
  const containerH = props.containerHeight
  const totalH = totalHeight.value
  if (totalH <= containerH) return containerH
  return (containerH / totalH) * containerH
})

const scrollThumbTop = computed(() => {
  const containerH = props.containerHeight
  const totalH = totalHeight.value
  const offset = offsetY.value
  if (totalH <= containerH) return 0
  const maxScroll = totalH - containerH
  return (offset / maxScroll) * (containerH - scrollThumbHeight.value)
})
</script>

<style scoped>
.virtual-list {
  position: relative;
  overflow-y: auto;
  overflow-x: hidden;
  scroll-behavior: smooth;
}

.virtual-list-spacer {
  flex-shrink: 0;
}

.virtual-list-item {
  display: flex;
  align-items: center;
  flex-shrink: 0;
  box-sizing: border-box;
}

/* 滚动条样式 */
.virtual-list-scrollbar {
  position: absolute;
  right: 8px;
  top: 8px;
  bottom: 8px;
  width: 8px;
  pointer-events: none;
  opacity: 0;
  transition: opacity 0.2s;
}

.virtual-list-scrollbar-thumb {
  position: absolute;
  right: 0;
  width: 100%;
  background: var(--color-bg-elevated);
  border-radius: var(--radius-full);
  transition: top 0.1s ease-out;
}

/* 自定义滚动条 */
.virtual-list::-webkit-scrollbar {
  width: 8px;
}

.virtual-list::-webkit-scrollbar-track {
  background: var(--color-bg-dark);
  border-radius: var(--radius-full);
}

.virtual-list::-webkit-scrollbar-thumb {
  background: var(--color-bg-elevated);
  border-radius: var(--radius-full);
}

.virtual-list::-webkit-scrollbar-thumb:hover {
  background: var(--color-text-muted);
}

/* 优化性能的渲染 */
.virtual-list-item {
  will-change: transform;
  contain: layout style paint;
}

/* 虚拟滚动动画 */
.virtual-list-item {
  animation: fadeIn 0.2s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
