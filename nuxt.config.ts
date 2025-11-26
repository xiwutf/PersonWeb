// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  modules: [
    '@nuxt/content',
    '@nuxtjs/tailwindcss'
  ],

  // Content 模块配置
  // @ts-ignore
  content: {
    highlight: {
      theme: {
        default: 'dracula',
        dark: 'dracula'
      },
      langs: [
        'javascript',
        'typescript',
        'vue',
        'html',
        'css',
        'scss',
        'python',
        'java',
        'cpp',
        'c',
        'json',
        'yaml',
        'xml',
        'sql',
        'bash',
        'shell',
        'markdown',
        'php',
        'go',
        'rust',
        'dart',
        'kotlin',
        'swift'
      ]
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
      title: '溪午听风 - 个人开发者网站',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        {
          name: 'description',
          content: '溪午听风的个人网站，展示插件工具、项目作品和技术博客。开发让生活更高效，代码就是我的魔方。'
        },
        { name: 'keywords', content: 'Revit插件,小程序开发,前端开发,技术博客,个人网站' },
        { name: 'author', content: '溪午听风' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'stylesheet', href: 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css' }
      ]
    }
  },

  // CSS 配置
  css: [
    '~/assets/css/main.css'
  ],

  compatibilityDate: '2024-04-03'
})
