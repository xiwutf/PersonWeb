<template>
  <footer class="relative bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 text-white overflow-hidden">
    <!-- 动态背景装饰 -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div class="absolute -top-24 -right-24 w-96 h-96 bg-purple-500/20 rounded-full blur-3xl animate-pulse mix-blend-screen"></div>
      <div class="absolute -bottom-24 -left-24 w-96 h-96 bg-blue-500/20 rounded-full blur-3xl animate-pulse animation-delay-2000 mix-blend-screen"></div>
      <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full h-full bg-[url('/images/grid.svg')] bg-center opacity-10 [mask-image:radial-gradient(ellipse_at_center,black_40%,transparent_70%)]"></div>
      <div class="absolute top-0 left-0 w-full h-px bg-gradient-to-r from-transparent via-cyan-500/50 to-transparent"></div>
    </div>

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 relative z-10">
      <div class="grid grid-cols-1 md:grid-cols-12 gap-12">
        <!-- 品牌区域 (占5列) -->
        <div class="md:col-span-5 space-y-6">
          <div class="flex items-center space-x-3">
            <div class="w-10 h-10 bg-gradient-to-br from-blue-600 to-purple-600 rounded-lg flex items-center justify-center shadow-lg shadow-blue-500/20">
              <span class="text-white font-bold font-['Outfit']">XF</span>
            </div>
            <span class="text-2xl font-bold font-['Outfit'] tracking-tight bg-clip-text text-transparent bg-gradient-to-r from-white to-slate-400">
              溪午听风
            </span>
          </div>
          <p class="text-slate-400 leading-relaxed max-w-sm">
            用代码构建未来，用技术解决难题。专注于全栈开发与AI应用探索，致力于创造优雅的数字体验。
          </p>
          <div class="flex space-x-3 pt-2">
            <a v-for="social in socialLinks" :key="social.name" :href="social.link" target="_blank" class="w-10 h-10 rounded-full bg-slate-800/50 border border-slate-700 flex items-center justify-center text-slate-400 hover:bg-blue-600 hover:text-white hover:border-blue-500 transition-all duration-300 hover:-translate-y-1">
              <i :class="social.icon"></i>
            </a>
          </div>
        </div>

        <!-- 链接区域 (占7列) -->
        <div class="md:col-span-7 grid grid-cols-2 sm:grid-cols-3 gap-8">
          <!-- 快速导航 -->
          <div>
            <h3 class="text-sm font-semibold text-slate-200 uppercase tracking-wider mb-6">探索</h3>
            <ul class="space-y-3">
              <li v-for="item in quickLinks" :key="item.path">
                <NuxtLink :to="item.path" class="text-slate-400 hover:text-blue-400 transition-colors flex items-center group text-sm">
                  <i class="fas fa-chevron-right text-[10px] mr-2 opacity-0 -ml-4 group-hover:opacity-100 group-hover:ml-0 transition-all duration-300"></i>
                  {{ item.title }}
                </NuxtLink>
              </li>
            </ul>
          </div>

          <!-- 资源 -->
          <div>
            <h3 class="text-sm font-semibold text-slate-200 uppercase tracking-wider mb-6">资源</h3>
            <ul class="space-y-3">
              <li><a href="#" class="text-slate-400 hover:text-blue-400 transition-colors text-sm">开源仓库</a></li>
              <li><a href="#" class="text-slate-400 hover:text-blue-400 transition-colors text-sm">设计规范</a></li>
              <li><a href="#" class="text-slate-400 hover:text-blue-400 transition-colors text-sm">更新日志</a></li>
            </ul>
          </div>

          <!-- 联系 -->
          <div>
            <h3 class="text-sm font-semibold text-slate-200 uppercase tracking-wider mb-6">联系</h3>
            <ul class="space-y-3">
              <li v-for="contact in contactInfo" :key="contact.type">
                <a :href="contact.link" class="text-slate-400 hover:text-blue-400 transition-colors flex items-center text-sm group">
                  <span class="w-6 h-6 rounded bg-slate-800 flex items-center justify-center mr-2 text-xs group-hover:bg-blue-500/20 group-hover:text-blue-400 transition-colors">
                    {{ contact.icon }}
                  </span>
                  {{ contact.label }}
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <!-- 底部版权与统计 -->
      <div class="border-t border-slate-800 mt-16 pt-8 flex flex-col md:flex-row justify-between items-center gap-4">
        <div class="text-slate-500 text-sm">
          © {{ currentYear }} 溪午听风. All rights reserved.
        </div>
        
        <div class="flex items-center gap-6 text-sm">
          <div v-if="stats" class="flex items-center gap-4 px-4 py-1.5 bg-slate-800/50 rounded-full border border-slate-700/50">
            <div class="flex items-center gap-2 text-slate-400">
              <span class="w-2 h-2 rounded-full bg-green-500 animate-pulse"></span>
              <span>今日: <span class="text-slate-200 font-mono">{{ stats.todayVisits }}</span></span>
            </div>
            <div class="w-px h-3 bg-slate-700"></div>
            <div class="text-slate-400">
              总计: <span class="text-slate-200 font-mono">{{ stats.totalVisits }}</span>
            </div>
          </div>
          
          <div class="text-slate-500 flex items-center gap-1">
            <span>Made with</span>
            <i class="fas fa-heart text-red-500 animate-pulse"></i>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
const currentYear = new Date().getFullYear()

const quickLinks = [
  { title: '插件工具', path: '/tools' },
  { title: '项目展示', path: '/projects' },
  { title: '技术博客', path: '/blog' },
  { title: '生活随笔', path: '/life' },
  { title: '关于我', path: '/about' }
]

const socialLinks = [
  { name: 'GitHub', icon: 'fab fa-github', link: 'https://github.com/Lijing327' },
  { name: 'Bilibili', icon: 'fas fa-play-circle', link: '#' }, // 替换为实际链接
  { name: 'Email', icon: 'fas fa-envelope', link: 'mailto:contact@溪午听风.com' }
]

const contactInfo = [
  {
    type: 'email',
    icon: '📧',
    label: 'Email Me',
    link: 'mailto:contact@溪午听风.com'
  },
  {
    type: 'github',
    icon: '🐙',
    label: 'GitHub',
    link: 'https://github.com/Lijing327'
  }
]

const { data: stats } = await useFetch('/api/stats')
</script>

<style scoped>
.animation-delay-2000 {
  animation-delay: 2s;
}
</style>