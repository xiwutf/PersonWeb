/**
 * ECharts 主题配置 Composable
 * 根据当前主题（深色/浅色）自动配置 ECharts 图表主题
 * 与 Naive UI 主题系统集成
 * 
 * 新增：霓虹渐变玻璃风格支持
 */
import { computed } from 'vue'

// 获取 CSS 变量的辅助函数
const getCssVar = (varName: string): string => {
  if (process.client) {
    const root = document.documentElement
    return getComputedStyle(root).getPropertyValue(varName).trim() || ''
  }
  return ''
}

export const useEChartsTheme = () => {
  // 使用全局主题管理 composable（前台和后台共用）
  const { isDark } = useNaiveTheme()



  // ECharts 深色主题配置（Vision Pro × 柔光感）
  // 使用 CSS 变量，确保与主题系统一致
  const darkTheme = computed(() => ({
    backgroundColor: 'transparent',
    textStyle: {
      color: getCssVar('--color-text-main') || 'rgba(255, 255, 255, 0.92)',
      fontSize: 13,
      fontWeight: 'normal'
    },
    title: {
      textStyle: {
        color: getCssVar('--color-text-main') || 'rgba(255, 255, 255, 0.92)',
        fontSize: 16,
        fontWeight: 'bold'
      }
    },
    legend: {
      textStyle: {
        color: getCssVar('--color-text-sec') || 'rgba(255, 255, 255, 0.7)',
        fontSize: 13,
        fontWeight: 'normal'
      }
    },
    tooltip: {
      backgroundColor: getCssVar('--color-bg-card') || 'rgba(15, 23, 42, 0.95)',
      borderColor: getCssVar('--color-border-default') || 'rgba(148, 163, 184, 0.35)',
      borderWidth: 1,
      textStyle: {
        color: getCssVar('--color-text-main') || 'rgba(255, 255, 255, 0.92)',
        fontSize: 13,
        fontWeight: 'normal',
        lineHeight: 20
      },
      padding: [10, 14],
      extraCssText: `box-shadow: ${getCssVar('--shadow-lg') || '0 24px 60px rgba(15, 23, 42, 0.85)'}, 0 0 0 1px ${getCssVar('--color-border-subtle') || 'rgba(148, 163, 184, 0.2)'}; backdrop-filter: blur(12px);`
    },
    grid: {
      borderColor: getCssVar('--color-border-subtle') || 'rgba(148, 163, 184, 0.25)',
      borderWidth: 1,
      left: '3%',
      right: '4%',
      bottom: '3%',
      top: '10%',
      containLabel: true
    },
    categoryAxis: {
      axisLine: {
        show: false
      },
      axisTick: {
        show: false
      },
      axisLabel: {
        color: getCssVar('--color-text-muted') || 'rgba(148, 163, 184, 0.8)',
        fontSize: 11
      },
      splitLine: {
        show: false
      }
    },
    valueAxis: {
      axisLine: {
        show: false
      },
      axisTick: {
        show: false
      },
      axisLabel: {
        color: getCssVar('--color-text-muted') || 'rgba(148, 163, 184, 0.7)',
        fontSize: 11
      },
      splitLine: {
        show: true,
        lineStyle: {
          color: getCssVar('--chart-grid') || 'rgba(148, 163, 184, 0.18)',
          type: 'dashed',
          width: 1
        }
      }
    }
  }))

  // ECharts 浅色主题配置
  // 使用 CSS 变量，确保与主题系统一致
  const lightTheme = computed(() => ({
    backgroundColor: 'transparent',
    textStyle: {
      color: getCssVar('--color-text-main') || '#374151',
      fontSize: 12
    },
    title: {
      textStyle: {
        color: getCssVar('--color-text-main') || '#111827',
        fontSize: 14,
        fontWeight: 'bold'
      }
    },
    legend: {
      textStyle: {
        color: getCssVar('--color-text-muted') || '#6b7280',
        fontSize: 12
      }
    },
    tooltip: {
      backgroundColor: getCssVar('--color-bg-card') || 'rgba(255, 255, 255, 0.95)',
      borderColor: getCssVar('--color-border-default') || 'rgba(209, 213, 219, 0.8)',
      borderWidth: 1,
      textStyle: {
        color: getCssVar('--color-text-main') || '#111827',
        fontSize: 12
      },
      padding: [8, 12]
    },
    grid: {
      borderColor: getCssVar('--color-border-subtle') || 'rgba(209, 213, 219, 0.5)',
      borderWidth: 1
    },
    categoryAxis: {
      axisLine: {
        lineStyle: {
          color: getCssVar('--color-border-default') || 'rgba(209, 213, 219, 0.8)'
        }
      },
      axisLabel: {
        color: getCssVar('--color-text-muted') || '#6b7280'
      },
      splitLine: {
        lineStyle: {
          color: getCssVar('--chart-grid') || 'rgba(209, 213, 219, 0.3)',
          type: 'dashed'
        }
      }
    },
    valueAxis: {
      axisLine: {
        lineStyle: {
          color: getCssVar('--color-border-default') || 'rgba(209, 213, 219, 0.8)'
        }
      },
      axisLabel: {
        color: getCssVar('--color-text-muted') || '#6b7280'
      },
      splitLine: {
        lineStyle: {
          color: getCssVar('--chart-grid') || 'rgba(209, 213, 219, 0.3)',
          type: 'dashed'
        }
      }
    }
  }))

  // 获取当前主题配置
  const echartsTheme = computed(() => {
    return isDark.value ? darkTheme.value : lightTheme.value
  })

  // 应用主题到图表配置
  const applyTheme = (option: any) => {
    const theme = echartsTheme.value
    return {
      ...option,
      backgroundColor: theme.backgroundColor,
      textStyle: theme.textStyle,
      tooltip: {
        ...theme.tooltip,
        ...option.tooltip
      },
      legend: {
        ...option.legend,
        textStyle: {
          ...theme.legend.textStyle,
          ...option.legend?.textStyle
        }
      },
      grid: {
        ...theme.grid,
        ...option.grid
      },
      xAxis: Array.isArray(option.xAxis)
        ? option.xAxis.map((axis: any) => ({
          ...axis,
          axisLine: { ...theme.categoryAxis.axisLine, ...axis.axisLine },
          axisLabel: { ...theme.categoryAxis.axisLabel, ...axis.axisLabel },
          splitLine: { ...theme.categoryAxis.splitLine, ...axis.splitLine }
        }))
        : option.xAxis ? {
          ...option.xAxis,
          axisLine: { ...theme.categoryAxis.axisLine, ...option.xAxis.axisLine },
          axisLabel: { ...theme.categoryAxis.axisLabel, ...option.xAxis.axisLabel },
          splitLine: { ...theme.categoryAxis.splitLine, ...option.xAxis.splitLine }
        } : undefined,
      yAxis: Array.isArray(option.yAxis)
        ? option.yAxis.map((axis: any) => ({
          ...axis,
          axisLine: { ...theme.valueAxis.axisLine, ...axis.axisLine },
          axisLabel: { ...theme.valueAxis.axisLabel, ...axis.axisLabel },
          splitLine: { ...theme.valueAxis.splitLine, ...axis.splitLine }
        }))
        : option.yAxis ? {
          ...option.yAxis,
          axisLine: { ...theme.valueAxis.axisLine, ...option.yAxis.axisLine },
          axisLabel: { ...theme.valueAxis.axisLabel, ...option.yAxis.axisLabel },
          splitLine: { ...theme.valueAxis.splitLine, ...option.yAxis.splitLine }
        } : undefined
    }
  }

  // ==================== 霓虹渐变玻璃风格辅助函数 ====================

  /**
   * 构建霓虹折线/面积图配置
   * @param colorVar CSS 变量名，如 '--chart-neon-red'
   * @param options 额外的 ECharts 配置
   */
  const buildNeonLineOptions = (colorVar: string, options: any = {}) => {
    const color = getCssVar(colorVar) || colorVar
    const colorWithAlpha = `${color}55` // 33% 透明度

    return {
      type: 'line',
      smooth: true,
      symbol: 'circle',
      symbolSize: 8, // 更大的圆点
      lineStyle: {
        width: 3,
        color,
        shadowBlur: 12,
        shadowColor: `${color}aa` // 发光边缘
      },
      itemStyle: {
        color,
        shadowBlur: 12, // 圆点发光效果
        shadowColor: `${color}aa` // 霓虹光点
      },
      areaStyle: {
        // 上深下浅渐变（发光波形效果）
        color: {
          type: 'linear',
          x: 0,
          y: 0,
          x2: 0,
          y2: 1,
          colorStops: [
            { offset: 0, color: colorWithAlpha },
            { offset: 1, color: 'rgba(15,23,42,0.0)' }
          ]
        }
      },
      ...options
    }
  }

  /**
   * 构建霓虹柱状图配置
   * @param colorStartVar 起始颜色 CSS 变量名
   * @param colorEndVar 结束颜色 CSS 变量名
   * @param options 额外的 ECharts 配置
   */
  const buildNeonBarOptions = (colorStartVar: string, colorEndVar: string, options: any = {}) => {
    const start = getCssVar(colorStartVar) || colorStartVar
    const end = getCssVar(colorEndVar) || colorEndVar

    return {
      type: 'bar',
      barWidth: 10,
      itemStyle: {
        borderRadius: [8, 8, 8, 8], // 圆角柱状图
        color: {
          type: 'linear',
          x: 0,
          y: 0,
          x2: 0,
          y2: 1,
          colorStops: [
            { offset: 0, color: start },
            { offset: 1, color: end }
          ]
        },
        shadowBlur: 10,
        shadowColor: `${start}aa` // 光晕效果
      },
      ...options
    }
  }

  /**
   * 构建霓虹环形图（Donut）配置
   * @param data 数据数组，每个元素包含 { value, name, colorVar }
   * @param options 额外的 ECharts 配置
   */
  const buildNeonDonutOptions = (data: Array<{ value: number; name: string; colorVar: string }>, options: any = {}) => {
    const seriesData = data.map(item => ({
      value: item.value,
      name: item.name,
      itemStyle: {
        color: getCssVar(item.colorVar) || item.colorVar,
        borderWidth: 4,
        borderColor: 'rgba(15, 23, 42, 1)' // 深色背景卡片
      }
    }))

    return {
      type: 'pie',
      radius: ['58%', '78%'], // 中间留空，外环较粗
      avoidLabelOverlap: false,
      label: {
        show: false
      },
      labelLine: {
        show: false
      },
      data: seriesData,
      ...options
    }
  }

  return {
    isDark,
    darkTheme,
    lightTheme,
    echartsTheme,
    applyTheme,
    // 霓虹渐变玻璃风格辅助函数
    buildNeonLineOptions,
    buildNeonBarOptions,
    buildNeonDonutOptions,
    getCssVar
  }
}

