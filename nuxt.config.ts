export default defineNuxtConfig({
  modules: [
    '@nuxt/content',
    '@nuxtjs/tailwindcss'
  ],
  
  // Content 模块配置
  content: {
    highlight: {
      theme: {
        default: 'github-light',
        dark: 'github-dark'
      }
    },
    markdown: {
      toc: {
        depth: 3,
        searchDepth: 3
      }
    }
  },
  
  // 应用配置
  app: {
    head: {
      title: '溪午听风 - 个人网站',
      meta: [
        { name: 'description', content: '全栈开发者与Revit插件专家的个人网站' },
        { name: 'keywords', content: '溪午听风, 全栈开发, Revit插件, 技术博客, 个人网站' }
      ]
    }
  },
  
  // CSS 配置
  css: ['~/assets/css/main.css']
})
