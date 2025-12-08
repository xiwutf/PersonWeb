<template>
  <div class="charts-container">
    <!-- 访问趋势图表 -->
    <div class="chart-card">
      <div class="chart-header">
        <h3 class="chart-title">访问趋势</h3>
        <div class="chart-tabs">
          <button
            :class="['chart-tab', { active: selectedPeriod === '7d' }]"
            @click="selectedPeriod = '7d'"
          >
            最近 7 天
          </button>
          <button
            :class="['chart-tab', { active: selectedPeriod === '30d' }]"
            @click="selectedPeriod = '30d'"
          >
            最近 30 天
          </button>
        </div>
      </div>
      <div class="chart-wrapper">
        <canvas ref="trendChartRef"></canvas>
      </div>
    </div>

    <!-- 访问来源分布 -->
    <div class="chart-card">
      <div class="chart-header">
        <h3 class="chart-title">访问来源</h3>
      </div>
      <div class="chart-wrapper">
        <canvas ref="sourceChartRef"></canvas>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, nextTick } from 'vue'

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
const sourceChartRef = ref<HTMLCanvasElement | null>(null)

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

  // 绘制背景
  ctx.fillStyle = 'rgba(255, 255, 255, 0.02)'
  ctx.fillRect(0, 0, logicalWidth, logicalHeight)

  const maxValue = Math.max(...visitData, ...visitorData, 1)
  const padding = 40
  const chartWidth = logicalWidth - padding * 2
  const chartHeight = logicalHeight - padding * 2

  // 绘制网格线
  ctx.strokeStyle = 'rgba(255, 255, 255, 0.08)'
  ctx.lineWidth = 1
  for (let i = 0; i <= 5; i++) {
    const y = padding + (chartHeight / 5) * i
    ctx.beginPath()
    ctx.moveTo(padding, y)
    ctx.lineTo(logicalWidth - padding, y)
    ctx.stroke()
  }

  // 绘制访问量折线（带动画）
  ctx.strokeStyle = '#60a5fa'
  ctx.lineWidth = 2.5
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

  // 绘制访客数折线
  ctx.strokeStyle = '#86efac'
  ctx.lineWidth = 2.5
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

  // 绘制标签
  ctx.fillStyle = '#9ca3af'
  ctx.font = '12px sans-serif'
  ctx.textAlign = 'center'
  labels.forEach((label, index) => {
    const x = padding + (chartWidth / (labels.length - 1 || 1)) * index
    ctx.fillText(label, x, logicalHeight - 10)
  })
}

// 渲染来源分布图（占位数据）
const renderSourceChart = () => {
  if (!sourceChartRef.value) return

  const ctx = sourceChartRef.value.getContext('2d')
  if (!ctx) return

  // 占位数据
  const sourceData = [
    { label: 'PC', value: 65, color: '#60a5fa' },
    { label: 'Mobile', value: 30, color: '#86efac' },
    { label: 'Tablet', value: 5, color: '#a78bfa' }
  ]

  const container = sourceChartRef.value.parentElement
  const containerWidth = container ? container.clientWidth : 400
  const containerHeight = 300

  const dpr = window.devicePixelRatio || 1
  const width = containerWidth
  const height = containerHeight

  sourceChartRef.value.width = width * dpr
  sourceChartRef.value.height = height * dpr
  sourceChartRef.value.style.width = `${width}px`
  sourceChartRef.value.style.height = `${height}px`

  ctx.scale(dpr, dpr)

  const centerX = width / 2
  const centerY = height / 2
  const radius = Math.min(width, height) / 2 - 40

  let currentAngle = -Math.PI / 2
  const total = sourceData.reduce((sum, item) => sum + item.value, 0)

  sourceData.forEach((item, index) => {
    const sliceAngle = (item.value / total) * 2 * Math.PI

    // 绘制扇形
    ctx.beginPath()
    ctx.moveTo(centerX, centerY)
    ctx.arc(centerX, centerY, radius, currentAngle, currentAngle + sliceAngle)
    ctx.closePath()
    ctx.fillStyle = item.color
    ctx.fill()

    // 绘制标签
    const labelAngle = currentAngle + sliceAngle / 2
    const labelX = centerX + Math.cos(labelAngle) * (radius * 0.7)
    const labelY = centerY + Math.sin(labelAngle) * (radius * 0.7)

    ctx.fillStyle = '#f3f4f6'
    ctx.font = 'bold 14px sans-serif'
    ctx.textAlign = 'center'
    ctx.textBaseline = 'middle'
    ctx.fillText(`${item.value}%`, labelX, labelY)

    currentAngle += sliceAngle
  })

  // 绘制图例
  const legendY = height - 60
  sourceData.forEach((item, index) => {
    const legendX = 20 + index * 100

    ctx.fillStyle = item.color
    ctx.fillRect(legendX, legendY, 12, 12)

    ctx.fillStyle = '#9ca3af'
    ctx.font = '12px sans-serif'
    ctx.textAlign = 'left'
    ctx.textBaseline = 'top'
    ctx.fillText(item.label, legendX + 18, legendY)
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
  // TODO: 可以在这里调用不同的 API 获取不同时间段的数据
  renderTrendChart()
})

onMounted(() => {
  nextTick(() => {
    setTimeout(() => {
      renderTrendChart()
      renderSourceChart()
    }, 100)
  })

  // 监听窗口大小变化
  const handleResize = () => {
    setTimeout(() => {
      renderTrendChart()
      renderSourceChart()
    }, 200)
  }
  window.addEventListener('resize', handleResize)

  onUnmounted(() => {
    window.removeEventListener('resize', handleResize)
  })
})
</script>

<style scoped>
.charts-container {
  display: grid;
  grid-template-columns: repeat(1, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
}

@media (min-width: 1024px) {
  .charts-container {
    grid-template-columns: repeat(2, 1fr);
  }
}

.chart-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  padding: 1.5rem;
  transition: all 0.3s ease;
}

.chart-card:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: rgba(255, 255, 255, 0.15);
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.chart-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #f3f4f6;
}

.chart-tabs {
  display: flex;
  gap: 0.5rem;
}

.chart-tab {
  padding: 0.375rem 0.75rem;
  border-radius: 0.5rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #9ca3af;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.chart-tab:hover {
  background: rgba(255, 255, 255, 0.08);
}

.chart-tab.active {
  background: rgba(59, 130, 246, 0.2);
  border-color: rgba(59, 130, 246, 0.4);
  color: #60a5fa;
}

.chart-wrapper {
  position: relative;
  width: 100%;
  height: 300px;
}
</style>

