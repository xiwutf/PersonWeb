/**
 * 设备检测 Composable
 * 用于检测当前设备类型（移动端/平板/桌面）
 */
export const useDevice = () => {
  const isMobile = ref(false)
  const isTablet = ref(false)
  const isDesktop = ref(false)
  const isTouch = ref(false)

  if (process.client) {
    const checkDevice = () => {
      const width = window.innerWidth
      isMobile.value = width < 768
      isTablet.value = width >= 768 && width < 1024
      isDesktop.value = width >= 1024
      
      // 检测触摸设备
      isTouch.value = 'ontouchstart' in window || navigator.maxTouchPoints > 0
    }

    checkDevice()
    
    // 监听窗口大小变化
    window.addEventListener('resize', checkDevice)
    
    // 监听方向变化（移动端）
    window.addEventListener('orientationchange', () => {
      setTimeout(checkDevice, 100)
    })

    onUnmounted(() => {
      window.removeEventListener('resize', checkDevice)
      window.removeEventListener('orientationchange', checkDevice)
    })
  }

  return {
    isMobile,
    isTablet,
    isDesktop,
    isTouch
  }
}

