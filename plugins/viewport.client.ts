const updateViewportHeight = () => {
  const viewportHeight = window.visualViewport?.height || window.innerHeight
  document.documentElement.style.setProperty('--app-height', `${viewportHeight}px`)
}

export default defineNuxtPlugin(() => {
  const userAgent = window.navigator.userAgent.toLowerCase()
  const isWeChatBrowser = userAgent.includes('micromessenger')

  document.documentElement.classList.toggle('is-wechat-browser', isWeChatBrowser)

  updateViewportHeight()

  const handleResize = () => updateViewportHeight()

  window.addEventListener('resize', handleResize, { passive: true })
  window.visualViewport?.addEventListener('resize', handleResize, { passive: true })
  window.visualViewport?.addEventListener('scroll', handleResize, { passive: true })
})
