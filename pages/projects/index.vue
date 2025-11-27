<template>
  <div class="min-h-screen bg-[#0f172a] text-slate-200 relative overflow-hidden font-['Outfit']">
    <!-- 全局背景噪点 -->
    <div class="fixed inset-0 opacity-[0.03] mix-blend-overlay pointer-events-none z-50"
         style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);">
    </div>

    <!-- 动态背景光斑 -->
    <div class="fixed inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-0 right-0 w-[600px] h-[600px] bg-purple-600/20 rounded-full blur-[120px] animate-blob mix-blend-screen"></div>
      <div class="absolute bottom-0 left-0 w-[600px] h-[600px] bg-pink-600/20 rounded-full blur-[120px] animate-blob animation-delay-2000 mix-blend-screen"></div>
      <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[900px] h-[900px] bg-indigo-900/20 rounded-full blur-[150px] animate-pulse mix-blend-screen"></div>
    </div>

    <div class="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- 页面头部 -->
      <header class="text-center mb-16 relative">
        <div class="inline-flex items-center justify-center w-20 h-20 rounded-2xl bg-gradient-to-br from-purple-500/20 to-pink-500/20 backdrop-blur-xl border border-white/10 mb-6 shadow-lg shadow-purple-500/10 animate-float">
          <span class="text-4xl">🧪</span>
        </div>
        <h1 class="text-4xl md:text-5xl font-bold mb-6 bg-clip-text text-transparent bg-gradient-to-r from-purple-200 via-white to-pink-200 tracking-tight">
          项目展示
        </h1>
        <p class="text-lg text-slate-400 max-w-2xl mx-auto leading-relaxed">
          精选个人项目作品与技术实践，从创意到实现的完整展示
        </p>
      </header>

      <!-- 统计信息 -->
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4 md:gap-6 mb-16">
        <div v-for="(stat, index) in stats" :key="index" 
             class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-2xl p-6 text-center hover:bg-slate-800/50 transition-colors group">
          <div class="text-3xl font-bold mb-2 bg-clip-text text-transparent bg-gradient-to-r" :class="stat.colorClass">
            {{ stat.value }}
          </div>
          <div class="text-sm text-slate-400 group-hover:text-slate-300 transition-colors">{{ stat.label }}</div>
        </div>
      </div>

      <!-- 分类筛选 -->
      <div class="flex flex-wrap justify-center gap-3 mb-12">
        <button
          @click="selectedCategory = 'all'"
          class="px-6 py-2.5 rounded-full text-sm font-medium transition-all duration-300 border backdrop-blur-md"
          :class="selectedCategory === 'all'
            ? 'bg-purple-500/20 border-purple-500/50 text-purple-200 shadow-[0_0_15px_rgba(168,85,247,0.3)]'
            : 'bg-slate-800/40 border-white/5 text-slate-400 hover:bg-slate-700/50 hover:text-slate-200 hover:border-white/10'"
        >
          全部项目
        </button>
        <button
          v-for="category in categories"
          :key="category"
          @click="selectedCategory = category"
          class="px-6 py-2.5 rounded-full text-sm font-medium transition-all duration-300 border backdrop-blur-md"
          :class="selectedCategory === category
            ? 'bg-purple-500/20 border-purple-500/50 text-purple-200 shadow-[0_0_15px_rgba(168,85,247,0.3)]'
            : 'bg-slate-800/40 border-white/5 text-slate-400 hover:bg-slate-700/50 hover:text-slate-200 hover:border-white/10'"
        >
          {{ category }}
        </button>
      </div>

      <!-- 项目列表 -->
      <div v-if="pending" class="text-center py-20">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-purple-500"></div>
        <p class="mt-4 text-slate-400">加载精彩项目中...</p>
      </div>

      <div v-else class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <TransitionGroup name="list">
          <div
            v-for="(project, index) in filteredProjects"
            :key="project._path"
            class="group relative bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl overflow-hidden hover:bg-slate-800/50 transition-all duration-500 hover:border-purple-500/30 hover:shadow-[0_0_30px_rgba(168,85,247,0.15)] flex flex-col"
            :style="{ transitionDelay: `${index * 50}ms` }"
          >
            <!-- 项目封面 -->
            <div class="relative h-56 overflow-hidden">
              <div class="absolute inset-0 bg-gradient-to-br from-purple-600/80 to-pink-600/80 group-hover:scale-105 transition-transform duration-700"></div>
              
              <!-- 装饰图案 -->
              <div class="absolute inset-0 opacity-20 bg-[url('/images/grid.svg')] bg-center"></div>
              
              <div class="absolute inset-0 flex items-center justify-center">
                <div class="text-center transform group-hover:scale-110 transition-transform duration-500">
                  <div class="text-5xl mb-3 drop-shadow-lg">🚀</div>
                  <div class="text-xl font-bold text-white drop-shadow-md tracking-wide">{{ project.category }}</div>
                </div>
              </div>

              <!-- 状态标签 -->
              <div class="absolute top-4 right-4">
                <span class="px-3 py-1 rounded-full text-xs font-bold backdrop-blur-md border shadow-lg"
                  :class="getStatusClass(project.status)">
                  {{ project.status }}
                </span>
              </div>
            </div>

            <!-- 内容区域 -->
            <div class="p-8 flex-1 flex flex-col">
              <div class="flex items-center justify-between mb-4">
                <h3 class="text-2xl font-bold text-slate-100 group-hover:text-purple-400 transition-colors">
                  {{ project.title }}
                </h3>
                <span class="text-xs text-slate-500 font-mono">{{ formatDate(project.date) }}</span>
              </div>
              
              <p class="text-slate-400 leading-relaxed mb-6 flex-1">
                {{ project.description }}
              </p>
              
              <!-- 技术栈 -->
              <div class="flex flex-wrap gap-2 mb-8">
                <span
                  v-for="tech in project.tech"
                  :key="tech"
                  class="px-2.5 py-1 bg-slate-700/50 text-slate-300 text-xs rounded-md border border-white/5"
                >
                  {{ tech }}
                </span>
              </div>
              
              <!-- 操作按钮 -->
              <div class="flex gap-4 mt-auto">
                <NuxtLink
                  :to="`/projects/detail-${project.slug || project._path.split('/').pop()}`"
                  class="flex-1 bg-purple-600 hover:bg-purple-500 text-white px-4 py-3 rounded-xl transition-all text-center font-medium shadow-lg shadow-purple-900/20 hover:shadow-purple-600/40 flex items-center justify-center gap-2 group/btn"
                >
                  <span>查看详情</span>
                  <svg class="w-4 h-4 group-hover/btn:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 5l7 7m0 0l-7 7m7-7H3"></path></svg>
                </NuxtLink>
                
                <a
                  v-if="project.demo_link"
                  :href="project.demo_link"
                  target="_blank"
                  class="px-4 py-3 border border-purple-500/30 text-purple-300 rounded-xl hover:bg-purple-500/10 hover:border-purple-500/50 transition-all font-medium flex items-center justify-center gap-2"
                >
                  <span>在线体验</span>
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"></path></svg>
                </a>
              </div>
            </div>
          </div>
        </TransitionGroup>
      </div>

      <!-- 底部CTA -->
      <div class="mt-24 text-center">
        <div class="bg-gradient-to-r from-purple-900/50 to-pink-900/50 backdrop-blur-md border border-white/10 rounded-3xl p-10 md:p-16 relative overflow-hidden group">
          <div class="absolute inset-0 bg-gradient-to-r from-purple-500/10 to-pink-500/10 opacity-0 group-hover:opacity-100 transition-opacity duration-700"></div>
          
          <div class="relative z-10">
            <h3 class="text-3xl font-bold mb-4 text-white">有项目合作想法？</h3>
            <p class="text-purple-200 mb-8 max-w-xl mx-auto">我们可以一起讨论技术方案，共同创造有价值的产品。无论是全栈开发、AI应用还是工具定制。</p>
            <div class="flex flex-col sm:flex-row gap-4 justify-center">
              <a
                href="mailto:contact@溪午听风.com"
                class="inline-flex items-center justify-center px-8 py-4 bg-white text-purple-900 rounded-xl font-bold hover:bg-purple-50 transition-colors shadow-lg shadow-white/10"
              >
                <span class="mr-2">📧</span>
                项目合作
              </a>
              <NuxtLink
                to="/about"
                class="inline-flex items-center justify-center px-8 py-4 border border-white/20 text-white rounded-xl font-bold hover:bg-white/10 transition-colors"
              >
                <span class="mr-2">👋</span>
                了解更多
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

// 使用 @nuxt/content 查询项目数据
const { data: projects, pending } = await useAsyncData('projects', () =>
  queryContent('/projects').sort({ date: -1 }).find()
)

// 分类筛选状态
const selectedCategory = ref('all')

// 计算属性
const categories = computed(() => {
  if (!projects.value) return []
  return [...new Set(projects.value.map(p => p.category))]
})

const filteredProjects = computed(() => {
  if (!projects.value) return []
  if (selectedCategory.value === 'all') return projects.value
  return projects.value.filter(p => p.category === selectedCategory.value)
})

const stats = computed(() => {
  if (!projects.value) return []
  const online = projects.value.filter(p => p.status === '已上线').length
  const allTech = projects.value.flatMap(p => p.tech || [])
  const uniqueTech = new Set(allTech).size
  
  return [
    { value: `${projects.value.length}+`, label: '项目作品', colorClass: 'from-purple-400 to-pink-400' },
    { value: online, label: '已上线', colorClass: 'from-green-400 to-emerald-400' },
    { value: uniqueTech, label: '技术栈', colorClass: 'from-blue-400 to-cyan-400' },
    { value: '100%', label: '开源率', colorClass: 'from-orange-400 to-yellow-400' }
  ]
})

// 格式化日期
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long'
  })
}

// 状态样式
const getStatusClass = (status) => {
  const map = {
    '已上线': 'bg-green-500/20 text-green-300 border-green-500/30',
    '开发中': 'bg-yellow-500/20 text-yellow-300 border-yellow-500/30',
    '已完成': 'bg-blue-500/20 text-blue-300 border-blue-500/30',
    '维护中': 'bg-orange-500/20 text-orange-300 border-orange-500/30'
  }
  return map[status] || 'bg-slate-500/20 text-slate-300 border-slate-500/30'
}

useHead({
  title: '项目展示 - 溪午听风',
  meta: [
    { name: 'description', content: '精选个人项目作品与技术实践' }
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
