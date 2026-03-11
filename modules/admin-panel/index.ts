/**
 * Admin Panel Module
 * 后台管理模块
 */

import { defineNuxtModule } from '@nuxt/kit'

export default defineNuxtModule({
  meta: {
    name: 'admin-panel',
    configKey: 'admin-panel'
  },
  async setup(moduleOptions, nuxt) {
    // 在客户端初始化后台管理
    if (process.client) {
      nuxt.hook('app:mounted', () => {
        console.log('Admin Panel module initialized')
      })
    }
  }
})