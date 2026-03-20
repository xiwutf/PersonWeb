const updateViewportHeight = () => {
  const viewportHeight = window.visualViewport?.height || window.innerHeight
  document.documentElement.style.setProperty('--app-height', `${viewportHeight}px`)
}

export default defineNuxtPlugin(() => {
  const root = document.documentElement
  const userAgent = window.navigator.userAgent.toLowerCase()
  const isWeChatBrowser = userAgent.includes('micromessenger')
  const isTouchDevice = window.matchMedia('(pointer: coarse)').matches || navigator.maxTouchPoints > 0

  root.classList.toggle('is-wechat-browser', isWeChatBrowser)
  root.classList.toggle('is-touch-device', isTouchDevice)

  updateViewportHeight()

  const handleResize = () => updateViewportHeight()
  const markAppReady = () => {
    window.requestAnimationFrame(() => {
      window.requestAnimationFrame(() => {
        root.classList.add('app-shell-ready')
      })
    })
  }

  window.addEventListener('resize', handleResize, { passive: true })
  window.visualViewport?.addEventListener('resize', handleResize, { passive: true })
  window.visualViewport?.addEventListener('scroll', handleResize, { passive: true })

  if (document.readyState === 'complete') {
    markAppReady()
  } else {
    window.addEventListener('load', markAppReady, { once: true, passive: true })
  }
})
