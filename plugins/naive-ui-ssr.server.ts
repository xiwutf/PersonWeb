/**
 * Naive UI / css-render SSR 适配
 *
 * 必须在服务端调用 setup()，通过 provide 注入 ssrAdapter，
 * 避免 css-render mount 走到 document.head（SSR 下 document 为 undefined）。
 *
 * 注意：不要在服务端 mock document —— vue3-ssr 用
 * `typeof document !== 'undefined'` 判断环境，mock 会导致 adapter 被跳过。
 */
import { setup } from '@css-render/vue3-ssr'
import { defineNuxtPlugin } from '#app'

export default defineNuxtPlugin({
  name: 'naive-ui-ssr',
  enforce: 'pre',
  setup(nuxtApp) {
    const { collect } = setup(nuxtApp.vueApp)

    const head = nuxtApp.ssrContext?.head
    if (head?.hooks) {
      head.hooks.hook('ssr:rendered', (ctx) => {
        const styles = collect()
        if (styles) {
          ctx.html.headTags += styles
        }
      })
    }
  },
})
