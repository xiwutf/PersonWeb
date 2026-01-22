/**
 * 修复非被动事件监听器问题
 * 
 * 某些第三方库（如 Naive UI）可能会添加非被动的 touchstart 事件监听器，
 * 这会导致浏览器性能警告。此插件通过拦截 addEventListener 来修复这个问题。
 */

export default defineNuxtPlugin(() => {
  if (!process.client) return

  // 保存原始的 addEventListener
  const originalAddEventListener = EventTarget.prototype.addEventListener

  // 重写 addEventListener，自动为 touchstart 和 touchmove 添加 passive 选项
  EventTarget.prototype.addEventListener = function (
    type: string,
    listener: EventListenerOrEventListenerObject | null,
    options?: boolean | AddEventListenerOptions
  ) {
    // 如果是 touchstart 或 touchmove 事件，且没有明确指定 passive
    if ((type === 'touchstart' || type === 'touchmove') && listener) {
      // 如果 options 是布尔值（useCapture），转换为对象
      if (typeof options === 'boolean') {
        options = { capture: options, passive: true }
      } else if (typeof options === 'object' && options !== null) {
        // 如果 options 是对象但没有指定 passive，设置为 true
        if (!('passive' in options)) {
          options = { ...options, passive: true }
        }
      } else {
        // 如果没有 options，设置为 { passive: true }
        options = { passive: true }
      }
    }

    // 调用原始的 addEventListener
    return originalAddEventListener.call(this, type, listener, options)
  }
})
