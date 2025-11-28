<template>
  <div class="min-h-screen bg-[#020617] text-white">
    <!-- 顶部 Hero -->
    <section class="relative pt-24 pb-16 border-b border-slate-800 overflow-hidden">
      <div class="absolute inset-0 pointer-events-none">
        <div class="absolute -top-32 -left-32 w-80 h-80 bg-blue-600/30 rounded-full blur-3xl"></div>
        <div class="absolute -bottom-40 right-0 w-96 h-96 bg-purple-600/30 rounded-full blur-3xl"></div>
        <div class="absolute inset-0 bg-[url('/images/grid.svg')] opacity-10"></div>
      </div>

      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <NuxtLink to="/" class="inline-flex items-center text-xs text-slate-400 mb-6 hover:text-slate-200 transition-colors">
          <i class="fas fa-arrow-left mr-2"></i> 返回首页
        </NuxtLink>

        <div class="grid lg:grid-cols-2 gap-10 items-center">
          <div>
            <p class="inline-flex items-center px-3 py-1 rounded-full bg-white/5 border border-white/10 text-xs mb-4">
              <span class="w-1.5 h-1.5 rounded-full bg-emerald-400 mr-2"></span>
              AI · 3D · 交互实验
            </p>
            <h1 class="text-3xl sm:text-4xl lg:text-5xl font-bold mb-4">
              AI 实验室
              <span class="block text-lg sm:text-xl font-normal text-slate-300 mt-3">
                在这里玩一玩有趣的 3D 场景和 AI 小实验。
              </span>
            </h1>
            <p class="text-sm sm:text-base text-slate-300 leading-relaxed mb-4">
              每一个 3D 物体都代表一个入口：博客、项目、数据仪表盘……
              未来会逐步接入 AI 智能体、小工具和实验性功能。
            </p>
            <div class="mt-4">
              <NuxtLink to="/ai" class="inline-flex items-center text-blue-400 hover:text-blue-300 text-sm transition-colors">
                <i class="fas fa-robot mr-2"></i>
                查看 AI 工作台（智能体与工作流）→
              </NuxtLink>
            </div>
          </div>

          <div class="hidden lg:block">
            <!-- 可以放一个缩略 3D 场景或简单插画 -->
            <div class="relative w-full aspect-square">
              <div class="absolute inset-0 bg-gradient-to-br from-cyan-500/20 to-purple-500/20 rounded-3xl blur-3xl"></div>
              <div class="relative bg-slate-900/50 backdrop-blur-xl rounded-3xl border border-slate-700/50 p-8 h-full flex items-center justify-center">
                <div class="text-center">
                  <div class="text-6xl mb-4">🔬</div>
                  <p class="text-slate-400 text-sm">实验性功能展示区</p>
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
          <!-- 博客地球 -->
          <div class="bg-slate-900/80 rounded-2xl border border-slate-700/80 p-6 backdrop-blur-xl">
            <h2 class="text-base font-semibold mb-3 flex items-center justify-between">
              <span>🌍 博客星球</span>
              <NuxtLink to="/blog" class="text-xs text-sky-400 hover:text-sky-300 transition-colors">进入博客</NuxtLink>
            </h2>
            <p class="text-xs text-slate-400 mb-4">
              围绕技术、思考与实践的记录。点击地球可以跳转至博客列表。
            </p>
            <Scene3D type="earth" :show-hint="false" height="260px" />
          </div>

          <!-- 项目飞船 -->
          <div class="bg-slate-900/80 rounded-2xl border border-slate-700/80 p-6 backdrop-blur-xl">
            <h2 class="text-base font-semibold mb-3 flex items-center justify-between">
              <span>🚀 项目飞船</span>
              <NuxtLink to="/projects" class="text-xs text-purple-400 hover:text-purple-300 transition-colors">查看项目</NuxtLink>
            </h2>
            <p class="text-xs text-slate-400 mb-4">
              实战项目与开源尝试。点击飞船进入项目作品集。
            </p>
            <Scene3D type="spaceship" :show-hint="false" height="260px" />
          </div>

          <!-- 数据星球 -->
          <div class="bg-slate-900/80 rounded-2xl border border-slate-700/80 p-6 backdrop-blur-xl">
            <h2 class="text-base font-semibold mb-3 flex items-center justify-between">
              <span>💎 数据星球</span>
              <NuxtLink to="/dashboard" class="text-xs text-emerald-400 hover:text-emerald-300 transition-colors">
                打开仪表盘
              </NuxtLink>
            </h2>
            <p class="text-xs text-slate-400 mb-4">
              展示站点访问、文章数据等指标。后续可以接入更多可视化组件。
            </p>
            <Scene3D type="datasphere" :show-hint="false" height="260px" />
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

useHead({
  title: 'AI 实验室 - 溪午听风',
  meta: [{ name: 'description', content: '3D 场景与 AI 小实验的集合地。' }]
})
</script>

