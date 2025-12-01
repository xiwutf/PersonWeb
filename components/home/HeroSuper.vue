<template>
  <section class="hero-super relative min-h-screen flex items-center justify-center overflow-hidden">
    <!-- 背景层 -->
    <div class="hero-background absolute inset-0">
      <!-- 深色渐变背景 -->
      <div class="absolute inset-0 bg-bg-body"></div>
      
      <!-- 动态光效网格 -->
      <div class="absolute inset-0 opacity-20">
        <div class="grid-pattern w-full h-full"></div>
      </div>
      
      <!-- 角落光晕 -->
      <div class="absolute top-0 right-0 w-96 h-96 bg-cyan-500/10 rounded-full blur-3xl"></div>
      <div class="absolute bottom-0 left-0 w-96 h-96 bg-purple-500/10 rounded-full blur-3xl"></div>
      
      <!-- 动态粒子效果（可选） -->
      <div class="absolute inset-0 particle-overlay"></div>
    </div>

    <!-- 主内容 -->
    <div class="hero-content relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12 w-full">
      <div class="grid lg:grid-cols-2 gap-16 lg:gap-24 items-center">
        <!-- 左侧文案区 -->
        <div class="text-center lg:text-left space-y-8" ref="leftContentRef">
          <!-- 欢迎标签 -->
          <div class="inline-flex items-center px-4 py-2 bg-bg-card backdrop-blur-xl rounded-full border border-border-subtle mb-2">
            <span class="flex h-2 w-2 relative mr-3">
              <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-primary-base opacity-75"></span>
              <span class="relative inline-flex rounded-full h-2 w-2 bg-primary-base"></span>
            </span>
            <span class="text-text-main font-medium text-xs sm:text-sm">欢迎来到 AI 创作空间</span>
          </div>

          <!-- 主标题 -->
          <h1 class="hero-title">
            <span class="block text-text-main">XIFG ·</span>
            <span class="block mt-3 text-transparent bg-clip-text bg-gradient-to-r from-primary-base via-primary-base to-primary-base">
              AI 创作空间
            </span>
          </h1>

          <!-- 副标题 -->
          <p class="text-lg sm:text-xl md:text-2xl text-text-main max-w-2xl mx-auto lg:mx-0 leading-relaxed font-medium">
            把个人网站升级为集「内容 · 项目 · AI 工具」于一体的创作者平台
          </p>

          <!-- 平台说明与个人标签（两列布局） -->
          <div class="grid md:grid-cols-2 gap-6 mt-8">
            <!-- 左侧：平台说明 -->
            <div class="bg-bg-card backdrop-blur-xl rounded-xl border border-border-subtle p-6">
              <h3 class="text-sm font-semibold text-primary-base mb-3 flex items-center">
                <i class="fas fa-cube mr-2"></i>
                平台定位
              </h3>
              <p class="text-sm text-text-main leading-relaxed">
                这里是一套围绕个人 IP 构建的创作操作系统：文章 / 项目 / 工具 / AI 实验室
              </p>
            </div>
            
            <!-- 右侧：个人标签 -->
            <div class="bg-bg-card backdrop-blur-xl rounded-xl border border-border-subtle p-6">
              <h3 class="text-sm font-semibold text-primary-base mb-3 flex items-center">
                <i class="fas fa-user mr-2"></i>
                个人标签
              </h3>
              <div class="flex flex-wrap gap-2">
                <span class="px-3 py-1.5 rounded-lg bg-bg-elevated border border-border-subtle text-text-main text-xs font-medium backdrop-blur-sm">
                  软件开发
                </span>
                <span class="px-3 py-1.5 rounded-lg bg-bg-elevated border border-border-subtle text-text-main text-xs font-medium backdrop-blur-sm">
                  算法研发
                </span>
                <span class="px-3 py-1.5 rounded-lg bg-bg-elevated border border-border-subtle text-text-main text-xs font-medium backdrop-blur-sm">
                  AI 工具折腾者
                </span>
              </div>
            </div>
          </div>

          <!-- 按钮区 -->
          <div class="flex flex-wrap justify-center lg:justify-start gap-4 sm:gap-5">
            <NuxtLink to="/projects" class="hero-button-primary group">
              <span class="relative z-10">浏览项目</span>
              <i class="fas fa-arrow-right ml-3 group-hover:translate-x-1 transition-transform relative z-10"></i>
            </NuxtLink>

            <NuxtLink to="/about" class="hero-button-secondary">
              关于我
            </NuxtLink>
          </div>
        </div>

        <!-- 右侧视觉区 -->
        <div class="relative hidden lg:block" ref="rightContentRef">
          <div class="hero-visual-card relative">
            <!-- 玻璃态卡片背景 -->
            <div class="absolute inset-0 bg-bg-card backdrop-blur-2xl rounded-3xl border border-border-subtle shadow-2xl"></div>
            
            <!-- 光效装饰 -->
            <div class="absolute -inset-1 bg-primary-soft rounded-3xl blur-xl opacity-50"></div>
            
            <!-- 内容 -->
            <div class="relative p-8">
              <!-- 头像 -->
              <div class="flex justify-center mb-6">
                <div class="relative group cursor-pointer">
                  <div class="absolute -inset-2 bg-primary-base rounded-2xl opacity-40 group-hover:opacity-80 blur-lg transition-all duration-500"></div>
                  <div class="relative w-32 h-40 group-hover:w-40 group-hover:h-52 rounded-xl group-hover:rounded-2xl overflow-hidden border-2 border-border-default group-hover:border-border-strong shadow-lg backdrop-blur-sm bg-bg-card transition-all duration-500">
                    <img 
                      src="/images/avatar.jpg" 
                      alt="溪午听风" 
                      class="w-full h-full object-cover object-top transform group-hover:scale-110 transition-transform duration-500"
                    />
                  </div>
                </div>
              </div>
              
              <!-- 信息卡片 -->
              <div class="bg-bg-card backdrop-blur-xl rounded-2xl p-6 border border-border-subtle">
                <h3 class="font-bold text-text-main text-xl mb-2">溪午听风</h3>
                <p class="text-sm text-text-muted mb-4">全栈开发 · AI 应用 · Revit 插件</p>
                <div class="flex flex-wrap gap-2">
                  <span class="px-3 py-1.5 rounded-lg bg-primary-soft border border-primary-base text-text-main text-xs font-medium">
                    Vue · Nuxt
                  </span>
                  <span class="px-3 py-1.5 rounded-lg bg-primary-soft border border-primary-base text-text-main text-xs font-medium">
                    .NET · Node.js
                  </span>
                  <span class="px-3 py-1.5 rounded-lg bg-primary-soft border border-primary-base text-text-main text-xs font-medium">
                    AI 工具链
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 滚动提示 -->
      <div
        class="absolute bottom-10 left-1/2 -translate-x-1/2 cursor-pointer group"
        @click="scrollToContent"
      >
        <div class="flex flex-col items-center gap-2">
          <span class="text-[10px] tracking-[0.2em] text-text-muted uppercase group-hover:text-primary-base transition-colors">
            Scroll
          </span>
          <div class="w-5 h-8 border-2 border-border-subtle rounded-full flex justify-center p-1 group-hover:border-primary-base transition-colors">
            <div class="w-1 h-2 bg-text-muted rounded-full animate-scroll-down group-hover:bg-primary-base"></div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { animate, stagger } from '@motionone/dom'

const leftContentRef = ref<HTMLElement | null>(null)
const rightContentRef = ref<HTMLElement | null>(null)

// 角色轮播
const roles = ['全栈开发者', 'AI 应用探索者', 'Revit 插件实践者', '终身学习者']
const currentRoleIndex = ref(0)
let roleInterval: NodeJS.Timeout | null = null

const startRoleCarousel = () => {
  roleInterval = setInterval(() => {
    currentRoleIndex.value = (currentRoleIndex.value + 1) % roles.length
  }, 3000)
}

// 滚动到内容区
const scrollToContent = () => {
  const contentSection = document.getElementById('content')
  if (contentSection) {
    contentSection.scrollIntoView({ behavior: 'smooth' })
  }
}

// 初始化动画
onMounted(() => {
  startRoleCarousel()
  
  // 使用 Motion One 添加进入动画
  if (leftContentRef.value) {
    const children = leftContentRef.value.children
    animate(
      Array.from(children),
      { opacity: [0, 1], y: [30, 0] },
      { duration: 0.6, delay: stagger(0.1), easing: 'ease-out' }
    )
  }
  
  if (rightContentRef.value) {
    animate(
      rightContentRef.value,
      { opacity: [0, 1], x: [50, 0] },
      { duration: 0.8, delay: 0.3, easing: 'ease-out' }
    )
  }
})

onUnmounted(() => {
  if (roleInterval) {
    clearInterval(roleInterval)
  }
})
</script>

<style scoped>
.hero-super {
  background: var(--color-bg-body);
}

.hero-background {
  z-index: 0;
}

.grid-pattern {
  background-image: 
    linear-gradient(rgba(255, 255, 255, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255, 255, 255, 0.03) 1px, transparent 1px);
  background-size: 50px 50px;
}

.particle-overlay {
  background: radial-gradient(circle at 20% 50%, rgba(0, 217, 255, 0.05) 0%, transparent 50%),
              radial-gradient(circle at 80% 80%, rgba(124, 58, 237, 0.05) 0%, transparent 50%);
}

.hero-title {
  font-size: 3rem;
  font-weight: 700;
  line-height: 1.1;
}

@media (min-width: 640px) {
  .hero-title {
    font-size: 3.5rem;
  }
}

@media (min-width: 768px) {
  .hero-title {
    font-size: 4rem;
  }
}

@media (min-width: 1024px) {
  .hero-title {
    font-size: 4.5rem;
  }
}

@media (min-width: 1280px) {
  .hero-title {
    font-size: 5rem;
  }
}

.hero-button-primary {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 1rem 3rem;
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-text-main);
  background: var(--color-primary-soft);
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-primary);
  border-radius: var(--radius-md);
  transition: all 0.3s ease;
  overflow: hidden;
}

.hero-button-primary::before {
  content: '';
  position: absolute;
  inset: 0;
  background: var(--color-primary);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.hero-button-primary:hover::before {
  opacity: 0.2;
}

.hero-button-primary:hover {
  border-color: var(--color-primary-hover);
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}

.hero-button-primary span,
.hero-button-primary i {
  position: relative;
  z-index: 1;
}

.hero-button-secondary {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 1rem 3rem;
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-text-main);
  background: var(--color-bg-card);
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  transition: all 0.3s ease;
}

.hero-button-secondary:hover {
  background: var(--color-bg-elevated);
  border-color: var(--color-border-default);
}

.hero-visual-card {
  perspective: 1000px;
}

@keyframes scroll-down {
  0%, 100% {
    transform: translateY(0);
    opacity: 1;
  }
  50% {
    transform: translateY(12px);
    opacity: 0.5;
  }
}

.animate-scroll-down {
  animation: scroll-down 2s ease-in-out infinite;
}

.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.5s ease;
}

.slide-up-enter-from {
  opacity: 0;
  transform: translateY(20px);
}

.slide-up-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}
</style>

