/**
 * Visitor Interaction Module
 * 访客互动模块
 */

import { defineNuxtModule } from '@nuxt/kit'

export default defineNuxtModule({
  meta: {
    name: 'visitor-interaction',
    configKey: 'visitor-interaction'
  },
  async setup(moduleOptions, nuxt) {
    // 在客户端初始化访客互动
    if (process.client) {
      nuxt.hook('app:mounted', () => {
        console.log('Visitor Interaction module initialized')
      })
    }
  }
})