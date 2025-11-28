/**
 * Toast 通知插件
 * 在客户端初始化 Toast
 */
import { useToast } from 'vue-toast-notification'
import 'vue-toast-notification/dist/theme-sugar.css'

export default defineNuxtPlugin((nuxtApp) => {
  if (process.client) {
    // 初始化 Toast
    const toast = useToast({
      position: 'top-right',
      duration: 3000
    })

    // 可以在全局使用
    nuxtApp.provide('toast', toast)
  }
})

