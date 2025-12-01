// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  modules: [
    '@nuxt/content',
    '@nuxtjs/tailwindcss'
  ],

  // 构建配置
  build: {
    transpile: ['naive-ui', 'vueuc', '@css-render/vue3-ssr']
  },

  // 运行时配置
  runtimeConfig: {
    public: {
      // API 基础路径，通过环境变量配置
      // 注意：客户端会根据当前域名自动判断使用哪个 API
      // - localhost/127.0.0.1: 自动使用 http://localhost:5234/api
      // - xifg.com.cn: 自动使用 https://api.xifg.com.cn/api
      // 环境变量仅作为服务端渲染时的默认值
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5234/api'
    }
  },

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
        { name: 'viewport', content: 'width=device-width, initial-scale=1, maximum-scale=5, user-scalable=yes' },
        {
          name: 'description',
          content: '溪午听风的个人网站，展示插件工具、项目作品和技术博客。开发让生活更高效，代码就是我的魔方。'
        },
        { name: 'keywords', content: 'Revit插件,小程序开发,前端开发,技术博客,个人网站' },
        { name: 'author', content: '溪午听风' },
        // 移动端优化
        { name: 'theme-color', content: '#3b82f6' }, // 浏览器地址栏颜色
        { name: 'apple-mobile-web-app-capable', content: 'yes' }, // iOS 全屏模式
        { name: 'apple-mobile-web-app-status-bar-style', content: 'default' }, // iOS 状态栏样式：default/black/black-translucent
        { name: 'apple-mobile-web-app-title', content: '溪午听风' }, // iOS 主屏幕标题
        { name: 'mobile-web-app-capable', content: 'yes' }, // Android 全屏模式
        { name: 'format-detection', content: 'telephone=no' }, // 禁用自动识别电话号码
      ],
      link: [
        { rel: 'icon', type: 'image/png', href: '/favicon.png' },
        { rel: 'stylesheet', href: 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css' },
        // iOS 主屏幕图标
        { rel: 'apple-touch-icon', href: '/favicon.png' },
        { rel: 'apple-touch-icon', sizes: '180x180', href: '/favicon.png' },
        // Android Chrome 图标
        { rel: 'icon', type: 'image/png', sizes: '192x192', href: '/favicon.png' },
        { rel: 'icon', type: 'image/png', sizes: '512x512', href: '/favicon.png' },
      ]
    }
  },

  // CSS 配置
  css: [
    '~/assets/styles/theme.css', // 统一主题样式文件（必须在最前面，定义 CSS 变量）
    '~/assets/css/main.css', // main.css 已经导入了 components.css
    '~/assets/css/themes.css',
    '~/assets/css/visitor-interaction.css',
    '~/assets/css/about.css',
    '~/assets/css/tools.css',
    '~/assets/css/life.css',
    '~/assets/css/blog.css',
    '~/assets/css/projects.css',
    '~/assets/css/investment.css',
    '~/assets/css/charts.css'
  ],

  // Nitro 配置（用于静态生成优化）
  nitro: {
    prerender: {
      // 自动爬取链接，但排除 admin 页面
      crawlLinks: true,
      // 排除需要认证的 admin 页面
      ignore: [
        '/admin/**'
      ],
      // 忽略预渲染错误（admin 页面会失败，这是正常的）
      failOnError: false
    },
  },
  
  // Vite 配置
  vite: {
    css: {
      preprocessorOptions: {
        scss: {
          // 抑制 Sass legacy-js-api 警告
          // 注意：通过环境变量 SASS_SILENCE_DEPRECATIONS=legacy-js-api 来抑制警告
          // 已在 package.json 的脚本中设置
          additionalData: `@use "sass:math";`,
          quietDeps: true
        }
      }
    },
    server: {
      // HMR 配置，减少连接中断
      hmr: {
        protocol: 'ws',
        host: 'localhost'
      }
    }
  },

  compatibilityDate: '2024-04-03'
})
