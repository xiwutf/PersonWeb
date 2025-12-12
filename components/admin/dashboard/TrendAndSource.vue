<template>
  <div class="chart-content">
    <div class="chart-section">
      <div class="chart-tabs">
        <AppButton
          :variant="selectedPeriod === '7d' ? 'primary' : 'secondary'"
          size="sm"
          @click="selectedPeriod = '7d'"
        >
          最近 7 天
        </AppButton>
        <AppButton
          :variant="selectedPeriod === '30d' ? 'primary' : 'secondary'"
          size="sm"
          @click="selectedPeriod = '30d'"
        >
          最近 30 天
        </AppButton>
      </div>
      <div class="chart-wrapper">
        <canvas ref="trendChartRef"></canvas>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, nextTick } from 'vue'
import AppButton from '~/components/ui/AppButton.vue'

interface VisitTrendItem {
  Date?: string
  date?: string
  Count?: number
  count?: number
  UniqueVisitors?: number
  uniqueVisitors?: number
}

interface Props {
  visitTrend: VisitTrendItem[]
}

const props = withDefaults(defineProps<Props>(), {
  visitTrend: () => []
})

const selectedPeriod = ref<'7d' | '30d'>('7d')
const trendChartRef = ref<HTMLCanvasElement | null>(null)

// 渲染访问趋势图表
const renderTrendChart = () => {
  if (!trendChartRef.value || props.visitTrend.length === 0) return

  const ctx = trendChartRef.value.getContext('2d')
  if (!ctx) return

  const formatDateLabel = (dateStr: string | null | undefined, index: number): string => {
    if (!dateStr || dateStr.trim() === '') {
      return `${index + 1}日`
    }

    try {
      if (typeof dateStr === 'string' && dateStr.includes('-')) {
        const parts = dateStr.split('-')
        if (parts.length >= 3) {
          const month = parseInt(parts[1], 10)
          const day = parseInt(parts[2], 10)
          if (!isNaN(month) && !isNaN(day) && month >= 1 && month <= 12 && day >= 1 && day <= 31) {
            return `${month}/${day}`
          }
        }
      }

      const date = new Date(dateStr)
      if (!isNaN(date.getTime()) && date.getFullYear() > 1970) {
        const month = date.getMonth() + 1
        const day = date.getDate()
        if (!isNaN(month) && !isNaN(day)) {
          return `${month}/${day}`
        }
      }
    } catch (e) {
      console.warn('日期格式化失败:', dateStr, e)
    }

    return `${index + 1}日`
  }

  const labels = props.visitTrend.map((item: any, index: number) => {
    const dateStr = item.Date || item.date || ''
    return formatDateLabel(dateStr, index)
  })
  const visitData = props.visitTrend.map((item: any) => item.Count || item.count || 0)
  const visitorData = props.visitTrend.map((item: any) => item.UniqueVisitors || item.uniqueVisitors || 0)

  const container = trendChartRef.value.parentElement
  const containerWidth = container ? container.clientWidth : 800
  const containerHeight = 300

  const dpr = window.devicePixelRatio || 1
  const width = containerWidth
  const height = containerHeight

  trendChartRef.value.width = width * dpr
  trendChartRef.value.height = height * dpr
  trendChartRef.value.style.width = `${width}px`
  trendChartRef.value.style.height = `${height}px`

  ctx.scale(dpr, dpr)

  const logicalWidth = width
  const logicalHeight = height

  ctx.clearRect(0, 0, logicalWidth, logicalHeight)

  // 使用霓虹色板 CSS 变量获取颜色
  const root = document.documentElement
  const primaryColor = getComputedStyle(root).getPropertyValue('--color-primary').trim() || '#0052FF'
  const successColor = getComputedStyle(root).getPropertyValue('--color-success').trim() || '#00D18B'
  const textMutedColor = getComputedStyle(root).getPropertyValue('--color-text-muted').trim() || '#9ca3af'
  const borderColor = 'rgba(148, 163, 184, 0.18)' // 弱网格颜色

  // 绘制背景
  ctx.fillStyle = 'transparent'
  ctx.fillRect(0, 0, logicalWidth, logicalHeight)

  const maxValue = Math.max(...visitData, ...visitorData, 1)
  const padding = 40
  const chartWidth = logicalWidth - padding * 2
  const chartHeight = logicalHeight - padding * 2

  // 绘制网格线（Vision Pro 风格 - 更暗，让主线更突出）
  ctx.strokeStyle = 'rgba(148, 163, 184, 0.18)'
  ctx.lineWidth = 1
  ctx.setLineDash([4, 4]) // 虚线
  for (let i = 0; i <= 5; i++) {
    const y = padding + (chartHeight / 5) * i
    ctx.beginPath()
    ctx.moveTo(padding, y)
    ctx.lineTo(logicalWidth - padding, y)
    ctx.stroke()
  }
  ctx.setLineDash([]) // 重置为实线

  // 绘制访问量折线（霓虹渐变效果）
  // 先绘制渐变填充区域
  ctx.beginPath()
  visitData.forEach((value, index) => {
    const x = padding + (chartWidth / (visitData.length - 1 || 1)) * index
    const y = logicalHeight - padding - (value / maxValue) * chartHeight
    if (index === 0) {
      ctx.moveTo(x, logicalHeight - padding) // 从底部开始
      ctx.lineTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  // 闭合路径到底部
  const lastX = padding + (chartWidth / (visitData.length - 1 || 1)) * (visitData.length - 1)
  ctx.lineTo(lastX, logicalHeight - padding)
  ctx.closePath()
  
  // 创建渐变填充（上深下浅）
  const gradient1 = ctx.createLinearGradient(0, padding, 0, logicalHeight - padding)
  gradient1.addColorStop(0, primaryColor + '55') // 33% 透明度
  gradient1.addColorStop(1, 'rgba(15,23,42,0.0)')
  ctx.fillStyle = gradient1
  ctx.fill()
  
  // 绘制折线（霓虹发光效果）
  ctx.shadowBlur = 12
  ctx.shadowColor = primaryColor + 'aa' // 发光边缘
  ctx.strokeStyle = primaryColor
  ctx.lineWidth = 3
  ctx.lineCap = 'round'
  ctx.lineJoin = 'round'
  ctx.beginPath()
  visitData.forEach((value, index) => {
    const x = padding + (chartWidth / (visitData.length - 1 || 1)) * index
    const y = logicalHeight - padding - (value / maxValue) * chartHeight
    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  ctx.stroke()
  ctx.shadowBlur = 0 // 重置阴影

  // 绘制访客数折线（霓虹渐变效果）
  // 先绘制渐变填充区域
  ctx.beginPath()
  visitorData.forEach((value, index) => {
    const x = padding + (chartWidth / (visitorData.length - 1 || 1)) * index
    const y = logicalHeight - padding - (value / maxValue) * chartHeight
    if (index === 0) {
      ctx.moveTo(x, logicalHeight - padding) // 从底部开始
      ctx.lineTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  // 闭合路径到底部
  const lastX2 = padding + (chartWidth / (visitorData.length - 1 || 1)) * (visitorData.length - 1)
  ctx.lineTo(lastX2, logicalHeight - padding)
  ctx.closePath()
  
  // 创建渐变填充（上深下浅）
  const gradient2 = ctx.createLinearGradient(0, padding, 0, logicalHeight - padding)
  gradient2.addColorStop(0, successColor + '55') // 33% 透明度
  gradient2.addColorStop(1, 'rgba(15,23,42,0.0)')
  ctx.fillStyle = gradient2
  ctx.fill()
  
  // 绘制折线（霓虹发光效果）
  ctx.shadowBlur = 12
  ctx.shadowColor = successColor + 'aa' // 发光边缘
  ctx.strokeStyle = successColor
  ctx.lineWidth = 3
  ctx.beginPath()
  visitorData.forEach((value, index) => {
    const x = padding + (chartWidth / (visitorData.length - 1 || 1)) * index
    const y = logicalHeight - padding - (value / maxValue) * chartHeight
    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  ctx.stroke()
  ctx.shadowBlur = 0 // 重置阴影

  // 绘制标签
  ctx.fillStyle = textMutedColor
  ctx.font = '12px sans-serif'
  ctx.textAlign = 'center'
  labels.forEach((label, index) => {
    const x = padding + (chartWidth / (labels.length - 1 || 1)) * index
    ctx.fillText(label, x, logicalHeight - 10)
  })
}

watch(() => props.visitTrend, () => {
  nextTick(() => {
    setTimeout(() => {
      renderTrendChart()
    }, 100)
  })
}, { deep: true })

watch(selectedPeriod, () => {
  renderTrendChart()
})

onMounted(() => {
  nextTick(() => {
    setTimeout(() => {
      renderTrendChart()
    }, 100)
  })

  const handleResize = () => {
    setTimeout(() => {
      renderTrendChart()
    }, 200)
  }
  window.addEventListener('resize', handleResize)

  onUnmounted(() => {
    window.removeEventListener('resize', handleResize)
  })
})
</script>

<style scoped>
/* 只保留布局相关的样式，不写颜色 */
.chart-content {
  width: 100%;
}

.chart-section {
  width: 100%;
}

.chart-tabs {
  display: flex;
  gap: 8px;
  margin-bottom: 16px;
}

.chart-wrapper {
  position: relative;
  width: 100%;
  height: 300px;
}
</style>
