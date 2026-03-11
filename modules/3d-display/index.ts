/**
 * 3D Display Module
 * 3D展示模块
 */

import { defineNuxtModule } from '@nuxt/kit'

export default defineNuxtModule({
  meta: {
    name: '3d-display',
    configKey: '3d-display'
  },
  async setup(moduleOptions, nuxt) {
    // 在客户端加载 Three.js
    if (process.client) {
      nuxt.hook('app:mounted', () => {
        const script = document.createElement('script')
        script.src = 'https://cdnjs.cloudflare.com/ajax/libs/three.js/r128/three.min.js'
        script.async = true
        document.head.appendChild(script)
        console.log('3D Display module initialized')
      })
    }
  }
})