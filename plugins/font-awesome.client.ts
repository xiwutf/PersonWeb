const FONT_AWESOME_HREF = 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css'

export default defineNuxtPlugin(() => {
  if (!process.client) {
    return
  }

  const existingLink = document.querySelector<HTMLLinkElement>('link[data-font-awesome="true"]')
  if (existingLink) {
    return
  }

  const preloadLink = document.createElement('link')
  preloadLink.rel = 'preload'
  preloadLink.as = 'style'
  preloadLink.href = FONT_AWESOME_HREF
  preloadLink.setAttribute('data-font-awesome', 'true')
  preloadLink.onload = () => {
    preloadLink.rel = 'stylesheet'
  }

  document.head.appendChild(preloadLink)
})
