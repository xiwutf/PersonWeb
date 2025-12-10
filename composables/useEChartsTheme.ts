/**
 * ECharts 主题配置 Composable
 * 根据当前主题（深色/浅色）自动配置 ECharts 图表主题
 * 与 Naive UI 主题系统集成
 */
import { computed } from 'vue'

export const useEChartsTheme = () => {
  // 使用全局主题系统（前台和后台共用）
  const { currentTheme } = useTheme()
  
  // 检测深色模式
  const isDark = computed(() => {
    return currentTheme.value === 'dark'
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
      backgroundColor: 'rgba(15, 23, 42, 0.98)', // 更深的背景，更高对比度
      borderColor: 'rgba(255, 255, 255, 0.2)', // 增强边框可见性
      borderWidth: 1,
      textStyle: {
        color: '#ffffff', // 纯白色文字
        fontSize: 13,
        fontWeight: 'normal',
        lineHeight: 20
      },
      padding: [10, 14],
      extraCssText: 'box-shadow: 0 4px 12px rgba(0, 0, 0, 0.6), 0 0 0 1px rgba(255, 255, 255, 0.1);' // 增强阴影和边框
    },
    grid: {
      borderColor: 'rgba(255, 255, 255, 0.15)', // 增强网格线可见性
      borderWidth: 1,
      left: '3%',
      right: '4%',
      bottom: '3%',
      top: '10%',
      containLabel: true
    },
    categoryAxis: {
      axisLine: {
        lineStyle: {
          color: 'rgba(255, 255, 255, 0.2)', // 增强轴线可见性
          width: 1
        }
      },
      axisLabel: {
        color: 'rgba(255, 255, 255, 0.8)', // 增强标签对比度
        fontSize: 12,
        fontWeight: 'normal'
      },
      splitLine: {
        show: false // 隐藏分割线，减少视觉干扰
      }
    },
    valueAxis: {
      axisLine: {
        lineStyle: {
          color: 'rgba(255, 255, 255, 0.2)', // 增强轴线可见性
          width: 1
        }
      },
      axisLabel: {
        color: 'rgba(255, 255, 255, 0.8)', // 增强标签对比度
        fontSize: 12,
        fontWeight: 'normal'
      },
      splitLine: {
        lineStyle: {
          color: 'rgba(255, 255, 255, 0.1)', // 增强分割线可见性
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
  const echartsTheme = computed(() => {
    return isDark.value ? darkTheme : lightTheme
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

  return {
    isDark,
    darkTheme,
    lightTheme,
    echartsTheme,
    applyTheme
  }
}

