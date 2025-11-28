<template>
  <div class="min-h-screen bg-[#0f172a] text-slate-200 relative overflow-hidden font-['Outfit']">
    <!-- 全局背景噪点 -->
    <div class="fixed inset-0 opacity-[0.03] mix-blend-overlay pointer-events-none z-50"
         style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);">
    </div>

    <!-- 动态背景光斑 -->
    <div class="fixed inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-0 left-1/3 w-[600px] h-[600px] bg-orange-600/20 rounded-full blur-[120px] animate-blob mix-blend-screen"></div>
      <div class="absolute bottom-0 right-1/3 w-[600px] h-[600px] bg-red-600/20 rounded-full blur-[120px] animate-blob animation-delay-2000 mix-blend-screen"></div>
      <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[900px] h-[900px] bg-amber-900/10 rounded-full blur-[150px] animate-pulse mix-blend-screen"></div>
    </div>

    <div class="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- 页面头部 -->
      <header class="text-center mb-16 relative">
        <div class="inline-flex items-center justify-center w-20 h-20 rounded-2xl bg-gradient-to-br from-orange-500/20 to-red-500/20 backdrop-blur-xl border border-white/10 mb-6 shadow-lg shadow-orange-500/10 animate-float">
          <span class="text-4xl">🔧</span>
        </div>
        <h1 class="text-4xl md:text-5xl font-bold mb-6 bg-clip-text text-transparent bg-gradient-to-r from-orange-200 via-white to-red-200 tracking-tight">
          插件工具
        </h1>
        <p class="text-lg text-slate-400 max-w-2xl mx-auto leading-relaxed">
          专业的Revit插件和自适应族，用科技提升工作效率，让复杂的设计变得简单
        </p>
      </header>

      <!-- 统计信息 -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-16">
        <div class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-2xl p-6 text-center hover:bg-slate-800/50 transition-colors group">
          <div class="text-4xl font-bold mb-2 bg-clip-text text-transparent bg-gradient-to-r from-orange-400 to-amber-400">
            {{ tools?.length || 0 }}+
          </div>
          <div class="text-sm text-slate-400 group-hover:text-slate-300 transition-colors">专业工具</div>
        </div>
        <div class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-2xl p-6 text-center hover:bg-slate-800/50 transition-colors group">
          <div class="text-4xl font-bold mb-2 bg-clip-text text-transparent bg-gradient-to-r from-red-400 to-pink-400">
            1000+
          </div>
          <div class="text-sm text-slate-400 group-hover:text-slate-300 transition-colors">用户选择</div>
        </div>
        <div class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-2xl p-6 text-center hover:bg-slate-800/50 transition-colors group">
          <div class="text-4xl font-bold mb-2 bg-clip-text text-transparent bg-gradient-to-r from-green-400 to-emerald-400">
            99%
          </div>
          <div class="text-sm text-slate-400 group-hover:text-slate-300 transition-colors">好评率</div>
        </div>
      </div>

      <!-- 工具列表 -->
      <div v-if="pending" class="text-center py-20">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-orange-500"></div>
        <p class="mt-4 text-slate-400">加载工具箱中...</p>
      </div>

      <div v-else-if="error" class="text-center py-20">
        <div class="text-6xl mb-4 opacity-50">😵</div>
        <h3 class="text-xl font-semibold text-slate-300 mb-2">加载失败</h3>
        <p class="text-slate-500">{{ error }}</p>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
        <TransitionGroup name="list">
          <div
            v-for="(tool, index) in tools"
            :key="tool._path"
            class="group relative bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl overflow-hidden hover:bg-slate-800/50 transition-all duration-500 hover:border-orange-500/30 hover:shadow-[0_0_30px_rgba(249,115,22,0.15)] flex flex-col"
            :style="{ transitionDelay: `${index * 50}ms` }"
          >
            <!-- 卡片头部 -->
            <div class="p-8 pb-4 flex-1">
              <div class="flex items-start justify-between mb-6">
                <div class="w-14 h-14 rounded-2xl bg-gradient-to-br from-orange-500/20 to-red-500/20 border border-white/10 flex items-center justify-center text-2xl shadow-inner group-hover:scale-110 transition-transform duration-500">
                  🔧
                </div>
                <div class="text-right">
                  <div class="text-2xl font-bold text-emerald-400">¥{{ tool.price }}</div>
                  <div class="text-xs text-slate-500 line-through">¥{{ Math.round(tool.price * 1.5) }}</div>
                </div>
              </div>

              <h3 class="text-xl font-bold text-slate-100 mb-3 group-hover:text-orange-400 transition-colors">
                {{ tool.title }}
              </h3>
              
              <p class="text-slate-400 leading-relaxed mb-6 text-sm">
                {{ tool.description }}
              </p>
              
              <!-- 标签 -->
              <div class="flex flex-wrap gap-2">
                <span
                  v-for="tag in tool.tags"
                  :key="tag"
                  class="px-2.5 py-1 bg-slate-700/50 text-slate-300 text-xs rounded-md border border-white/5"
                >
                  {{ tag }}
                </span>
              </div>
            </div>

            <!-- 底部按钮 -->
            <div class="p-6 pt-0 mt-auto">
              <div class="flex gap-3">
                <NuxtLink
                  :to="`/tools/detail-${tool.slug || tool._path.split('/').pop()}`"
                  class="flex-1 bg-slate-700/50 hover:bg-slate-600/50 text-slate-200 px-4 py-3 rounded-xl transition-all text-center font-medium text-sm border border-white/5 hover:border-white/10"
                >
                  查看详情
                </NuxtLink>
                <a
                  :href="tool.buy_link"
                  target="_blank"
                  class="flex-1 bg-gradient-to-r from-orange-600 to-red-600 hover:from-orange-500 hover:to-red-500 text-white px-4 py-3 rounded-xl transition-all text-center font-medium text-sm shadow-lg shadow-orange-900/20 hover:shadow-orange-600/40 flex items-center justify-center gap-1"
                >
                  <span>立即购买</span>
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path></svg>
                </a>
              </div>
            </div>
          </div>
        </TransitionGroup>
      </div>

      <!-- 快速导航 -->
      <div class="mt-16 flex justify-center gap-4">
        <NuxtLink
          to="/tools/marketplace"
          class="px-6 py-3 bg-gradient-to-r from-orange-600 to-red-600 hover:from-orange-500 hover:to-red-500 text-white rounded-xl transition-all font-medium"
        >
          🛒 工具商城
        </NuxtLink>
        <NuxtLink
          to="/tools/collections"
          class="px-6 py-3 bg-slate-700/50 hover:bg-slate-600/50 text-slate-200 rounded-xl transition-all font-medium"
        >
          📦 工具合集
        </NuxtLink>
        <NuxtLink
          to="/tools/my-tools"
          class="px-6 py-3 bg-slate-700/50 hover:bg-slate-600/50 text-slate-200 rounded-xl transition-all font-medium"
        >
          🎒 我的工具
        </NuxtLink>
      </div>

      <!-- 底部CTA -->
      <div class="mt-24 text-center">
        <div class="bg-gradient-to-r from-orange-900/50 to-red-900/50 backdrop-blur-md border border-white/10 rounded-3xl p-10 md:p-16 relative overflow-hidden group">
          <div class="absolute inset-0 bg-gradient-to-r from-orange-500/10 to-red-500/10 opacity-0 group-hover:opacity-100 transition-opacity duration-700"></div>
          
          <div class="relative z-10">
            <h3 class="text-3xl font-bold mb-4 text-white">需要定制化插件？</h3>
            <p class="text-orange-200 mb-8 max-w-xl mx-auto">我们提供专业的Revit插件定制开发服务，为您量身打造提升效率的专属工具。</p>
            <a
              href="mailto:contact@溪午听风.com"
              class="inline-flex items-center justify-center px-8 py-4 bg-white text-orange-900 rounded-xl font-bold hover:bg-orange-50 transition-colors shadow-lg shadow-white/10"
            >
              <span class="mr-2">📧</span>
              联系定制
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// 使用 @nuxt/content 查询工具数据
const { data: tools, pending, error } = await useAsyncData('tools', () =>
  queryContent('/tools').sort({ date: -1 }).find()
)

useHead({
  title: '插件工具 - 溪午听风',
  meta: [
    { name: 'description', content: '专业的Revit插件和自适应族' }
  ]
})
</script>

<style scoped>
.list-enter-active,
.list-leave-active {
  transition: all 0.5s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateY(30px);
}

.animate-blob {
  animation: blob 7s infinite;
}
.animation-delay-2000 {
  animation-delay: 2s;
}
@keyframes blob {
  0% { transform: translate(0px, 0px) scale(1); }
  33% { transform: translate(30px, -50px) scale(1.1); }
  66% { transform: translate(-20px, 20px) scale(0.9); }
  100% { transform: translate(0px, 0px) scale(1); }
}

.animate-float {
  animation: float 6s ease-in-out infinite;
}
@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}
</style>
