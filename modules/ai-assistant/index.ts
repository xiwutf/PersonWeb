/**
 * AI Assistant Module
 * 智能AI助手模块
 */

import { defineNuxtModule } from '@nuxt/kit'

export default defineNuxtModule({
  meta: {
    name: 'ai-assistant',
    configKey: 'ai-assistant'
  },
  async setup(moduleOptions, nuxt) {
    // 在客户端添加 AIAssistant 组件
    if (process.client) {
      nuxt.hook('app:mounted', () => {
        console.log('AI Assistant module initialized')
      })
    }
  }
})