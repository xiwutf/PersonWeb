/**
 * 全站页面过渡动画插件
 * 提供流畅的 SPA 页面切换效果
 */
export default defineNuxtPlugin(() => {
  if (process.server) return

  const router = useRouter()
  let isTransitioning = false

  const stripTransitionClass = () => {
    document.querySelector('#__nuxt')?.classList.remove('page-transitioning')
    isTransitioning = false
  }

  // 页面进入动画
  router.beforeEach((to, from, next) => {
    if (isTransitioning) {
      next()
      return
    }

    // 后台整页与 SPA 切换均不使用根节点透明度动画，避免主内容区异常空白
    if (to.path.startsWith('/admin') || from.path.startsWith('/admin')) {
      next()
      return
    }

    // 首次进入或整页刷新：from.matched 为空
    if (from.matched.length === 0) {
      next()
      return
    }

    isTransitioning = true

    const app = document.querySelector('#__nuxt')
    if (app) {
      app.classList.add('page-transitioning')
    }

    next()
  })

  router.afterEach((to, from) => {
    // 创建光速线效果
    if (from.path !== to.path) {
      const lightLine = document.createElement('div')
      lightLine.className = 'light-speed-line'
      document.body.appendChild(lightLine)
      
      setTimeout(() => {
        if (lightLine.parentNode) {
          lightLine.parentNode.removeChild(lightLine)
        }
      }, 500)
    }

    setTimeout(() => {
      stripTransitionClass()
    }, 300)
  })

  // 添加全局样式
  if (process.client) {
    const style = document.createElement('style')
    style.textContent = `
      /* 页面过渡动画 */
      #__nuxt {
        position: relative;
      }

      #__nuxt.page-transitioning {
        animation: pageFadeIn 0.3s ease-out;
      }

      @keyframes pageFadeIn {
        from {
          transform: translateY(10px);
        }
        to {
          transform: translateY(0);
        }
      }

      /* 路由视图过渡 */
      .page-enter-active,
      .page-leave-active {
        transition: all 0.3s ease;
      }

      .page-enter-from {
        opacity: 0;
        transform: translateX(20px);
      }

      .page-leave-to {
        opacity: 0;
        transform: translateX(-20px);
      }

      /* 内容淡入 */
      .content-enter-active {
        transition: all 0.6s ease-out;
      }

      .content-enter-from {
        opacity: 0;
        transform: translateY(20px);
      }

      /* 光速线效果 */
      @keyframes lightSpeed {
        0% {
          transform: translateX(-100%) skewX(-15deg);
          opacity: 0;
        }
        50% {
          opacity: 1;
        }
        100% {
          transform: translateX(200%) skewX(-15deg);
          opacity: 0;
        }
      }

      .light-speed-line {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 2px;
        background: linear-gradient(90deg, transparent, #3b82f6, transparent);
        z-index: 9999;
        animation: lightSpeed 0.5s ease-out;
        pointer-events: none;
      }
    `
    document.head.appendChild(style)
  }
})

