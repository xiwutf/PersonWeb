<template>
  <!-- 
    AI 实验室模块
    moduleId: ai_lab
    
    如何在后台配置中控制这个模块的主题：
    1. 登录后台，进入主题管理页面
    2. 找到"AI 实验室模块"配置项
    3. 选择"跟随全局"或指定独立主题（如 "tech-blue"）
    4. 保存后，刷新页面即可看到效果
  -->
  <div 
    class="min-h-screen bg-bg-body text-text-main"
    :data-module-theme="moduleTheme || undefined"
  >
    <!-- 顶部 Hero -->
    <section class="relative pt-24 pb-16 border-b border-border-subtle overflow-hidden">
      <div class="absolute inset-0 pointer-events-none">
        <div class="absolute -top-32 -left-32 w-80 h-80 bg-blue-600/30 rounded-full blur-3xl"></div>
        <div class="absolute -bottom-40 right-0 w-96 h-96 bg-purple-600/30 rounded-full blur-3xl"></div>
        <div class="absolute inset-0 bg-[url('/images/grid.svg')] opacity-10"></div>
      </div>

      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid lg:grid-cols-2 gap-10 items-center">
          <div>
            <!-- 标签和标题：使用主题文字颜色，替换写死的 var(--color-bg-light, white)/slate 颜色 -->
            <p class="inline-flex items-center px-3 py-1 rounded-full bg-primary-soft border border-border-subtle text-xs mb-4 text-text-muted">
              <span class="w-1.5 h-1.5 rounded-full bg-primary mr-2"></span>
              AI · 3D · 交互实验
            </p>
            <h1 class="text-3xl sm:text-4xl lg:text-5xl font-bold mb-4 text-text-main">
              AI 实验室
              <span class="block text-lg sm:text-xl font-normal text-text-muted mt-3">
                在这里玩一玩有趣的 3D 场景和 AI 小实验。
              </span>
            </h1>
            <p class="text-sm sm:text-base text-text-muted leading-relaxed mb-4">
              每一个 3D 物体都代表一个入口：博客、项目、数据仪表盘……
              未来会逐步接入 AI 智能体、小工具和实验性功能。
            </p>
            <div class="mt-4">
              <NuxtLink to="/ai" class="inline-flex items-center text-primary hover:text-primary-hover text-sm transition-colors">
                <i class="fas fa-robot mr-2"></i>
                查看 AI 工作台（智能体与工作流）→
              </NuxtLink>
            </div>
          </div>

          <div class="hidden lg:block">
            <!-- 可以放一个缩略 3D 场景或简单插画 -->
            <div class="relative w-full aspect-square">
              <div class="absolute inset-0 bg-gradient-to-br from-cyan-500/20 to-purple-500/20 rounded-3xl blur-3xl"></div>
              <!-- 3D 场景容器：使用主题背景和边框颜色 -->
              <div class="relative bg-bg-card backdrop-blur-xl rounded-3xl border border-border-subtle p-8 h-full flex items-center justify-center">
                <div class="text-center">
                  <div class="text-6xl mb-4">🔬</div>
                  <p class="text-text-muted text-sm">实验性功能展示区</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 3D 实验区 -->
    <section class="py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <!-- 博客地球：使用主题背景和边框颜色，替换写死的 slate 颜色 -->
          <AppCard class="p-6 backdrop-blur-xl">
            <h2 class="text-base font-semibold mb-3 flex items-center justify-between text-text-main">
              <span>🌍 博客星球</span>
              <NuxtLink to="/blog" class="text-xs text-primary hover:text-primary-hover transition-colors">进入博客</NuxtLink>
            </h2>
            <p class="text-xs text-text-muted mb-4">
              围绕技术、思考与实践的记录。点击地球可以跳转至博客列表。
            </p>
            <Scene3D type="earth" :show-hint="false" height="260px" />
          </AppCard>

          <!-- 项目飞船：使用主题背景和边框颜色 -->
          <AppCard class="p-6 backdrop-blur-xl">
            <h2 class="text-base font-semibold mb-3 flex items-center justify-between text-text-main">
              <span>🚀 项目飞船</span>
              <NuxtLink to="/projects" class="text-xs text-primary hover:text-primary-hover transition-colors">查看项目</NuxtLink>
            </h2>
            <p class="text-xs text-text-muted mb-4">
              实战项目与开源尝试。点击飞船进入项目作品集。
            </p>
            <Scene3D type="spaceship" :show-hint="false" height="260px" />
          </AppCard>

          <!-- 数据星球：使用主题背景和边框颜色 -->
          <AppCard class="p-6 backdrop-blur-xl">
            <h2 class="text-base font-semibold mb-3 flex items-center justify-between text-text-main">
              <span>💎 数据星球</span>
              <NuxtLink to="/dashboard" class="text-xs text-primary hover:text-primary-hover transition-colors">
                打开仪表盘
              </NuxtLink>
            </h2>
            <p class="text-xs text-text-muted mb-4">
              展示站点访问、文章数据等指标。后续可以接入更多可视化组件。
            </p>
            <Scene3D type="datasphere" :show-hint="false" height="260px" />
          </AppCard>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import AppCard from '~/components/ui/AppCard.vue'
import Scene3D from '~/components/three/Scene3D.vue'

// 使用模块主题 composable
const { moduleTheme } = useModuleTheme('ai_lab')

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'AI 实验室 - 溪午听风',
  meta: [{ name: 'description', content: '3D 场景与 AI 小实验的集合地。' }]
})
</script>

<style scoped>
/* 页面样式 */
</style>

