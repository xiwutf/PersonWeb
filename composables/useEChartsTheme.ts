/**
 * ECharts 主题配置 Composable
 * 根据当前主题（深色/浅色）自动配置 ECharts 图表主题
 * 与 Naive UI 主题系统集成
 */
import { computed } from 'vue'

export const useEChartsTheme = () => {
  // 检测深色模式（优先使用 Naive UI 主题系统）
  const isDark = computed(() => {
    if (process.client) {
      // 检查是否有 Naive UI 主题系统
      try {
        const { useNaiveTheme } = require('~/composables/useNaiveTheme')
        const { isDark: naiveIsDark } = useNaiveTheme()
        return naiveIsDark.value
      } catch {
        // 如果没有，回退到检查 DOM
        return document.documentElement.classList.contains('dark')
      }
    }
    return false
  })

  // ECharts 深色主题配置（增强对比度）
  const darkTheme = {
    backgroundColor: 'transparent',
    textStyle: {
      color: '#ffffff', // 纯白色文字，最高对比度
      fontSize: 13,
      fontWeight: 'normal'
    },
    title: {
      textStyle: {
        color: '#ffffff', // 纯白色标题
        fontSize: 16,
        fontWeight: 'bold'
      }
    },
    legend: {
      textStyle: {
        color: '#e5e7eb', // 浅灰色，但更亮
        fontSize: 13,
        fontWeight: 'normal'
      }
    },
    tooltip: {
      backgroundColor: 'rgba(17, 24, 39, 0.98)', // 更深的背景，更高对比度
      borderColor: 'rgba(156, 163, 175, 0.5)',
      borderWidth: 1,
      textStyle: {
        color: '#ffffff', // 纯白色文字
        fontSize: 13,
        fontWeight: 'normal'
      },
      padding: [10, 14],
      extraCssText: 'box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5);' // 添加阴影增强可见性
    },
    grid: {
      borderColor: 'rgba(75, 85, 99, 0.3)', // 深色网格线
      borderWidth: 1
    },
    categoryAxis: {
      axisLine: {
        lineStyle: {
          color: 'rgba(156, 163, 175, 0.6)', // 更亮的轴线
          width: 1
        }
      },
      axisLabel: {
        color: '#e5e7eb', // 更亮的标签
        fontSize: 12
      },
      splitLine: {
        show: false // 隐藏分割线，减少视觉干扰
      }
    },
    valueAxis: {
      axisLine: {
        lineStyle: {
          color: 'rgba(156, 163, 175, 0.6)', // 更亮的轴线
          width: 1
        }
      },
      axisLabel: {
        color: '#e5e7eb', // 更亮的标签
        fontSize: 12
      },
      splitLine: {
        lineStyle: {
          color: 'rgba(156, 163, 175, 0.3)', // 更明显的分割线
          type: 'dashed',
          width: 1
        }
      }
    }
  }

  // ECharts 浅色主题配置
  const lightTheme = {
    backgroundColor: 'transparent',
    textStyle: {
      color: '#374151', // 深灰色文字
      fontSize: 12
    },
    title: {
      textStyle: {
        color: '#111827', // 深色文字
        fontSize: 14,
        fontWeight: 'bold'
      }
    },
    legend: {
      textStyle: {
        color: '#6b7280', // 中等深度的文字
        fontSize: 12
      }
    },
    tooltip: {
      backgroundColor: 'rgba(255, 255, 255, 0.95)', // 浅色背景
      borderColor: 'rgba(209, 213, 219, 0.8)',
      borderWidth: 1,
      textStyle: {
        color: '#111827', // 深色文字
        fontSize: 12
      },
      padding: [8, 12]
    },
    grid: {
      borderColor: 'rgba(209, 213, 219, 0.5)', // 浅色网格线
      borderWidth: 1
    },
    categoryAxis: {
      axisLine: {
        lineStyle: {
          color: 'rgba(209, 213, 219, 0.8)'
        }
      },
      axisLabel: {
        color: '#6b7280'
      },
      splitLine: {
        lineStyle: {
          color: 'rgba(209, 213, 219, 0.3)',
          type: 'dashed'
        }
      }
    },
    valueAxis: {
      axisLine: {
        lineStyle: {
          color: 'rgba(209, 213, 219, 0.8)'
        }
      },
      axisLabel: {
        color: '#6b7280'
      },
      splitLine: {
        lineStyle: {
          color: 'rgba(209, 213, 219, 0.3)',
          type: 'dashed'
        }
      }
    }
  }

  // 获取当前主题配置
  const currentTheme = computed(() => {
    return isDark.value ? darkTheme : lightTheme
  })

  // 应用主题到图表配置
  const applyTheme = (option: any) => {
    const theme = currentTheme.value
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

  return {
    isDark,
    darkTheme,
    lightTheme,
    currentTheme,
    applyTheme
  }
}

