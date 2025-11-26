<template>
  <footer class="bg-slate-900 text-white relative overflow-hidden">
    <!-- 背景装饰 -->
    <div class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-blue-500 via-purple-500 to-pink-500"></div>
    <div class="absolute -top-20 -right-20 w-96 h-96 bg-purple-500/10 rounded-full blur-3xl pointer-events-none"></div>
    <div class="absolute -bottom-20 -left-20 w-96 h-96 bg-blue-500/10 rounded-full blur-3xl pointer-events-none"></div>

    <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-16 relative z-10">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-12">
        <!-- 品牌信息 -->
        <div class="md:col-span-2 space-y-6">
          <div class="flex items-center space-x-3">
            <div class="w-12 h-12 bg-gradient-to-br from-blue-600 to-purple-600 rounded-xl flex items-center justify-center shadow-lg shadow-blue-500/20">
              <span class="text-white text-xl font-bold font-['Outfit']">XF</span>
            </div>
            <div>
              <span class="text-2xl font-bold font-['Outfit'] tracking-tight">溪午听风</span>
              <p class="text-sm text-slate-400">开发让生活更高效</p>
            </div>
          </div>
          <p class="text-slate-400 leading-relaxed max-w-md">
            专注于Web开发、Revit插件开发和移动端应用开发。用代码构建未来，用技术解决难题。
          </p>
          <div class="flex space-x-4">
            <a v-for="social in socialLinks" :key="social.name" :href="social.link" class="w-10 h-10 rounded-full bg-slate-800 flex items-center justify-center text-slate-400 hover:bg-white hover:text-slate-900 transition-all duration-300 hover:-translate-y-1">
              <i :class="social.icon"></i>
            </a>
          </div>
        </div>

        <!-- 快速链接 -->
        <div>
          <h3 class="text-lg font-bold font-['Outfit'] mb-6 text-white">快速链接</h3>
          <ul class="space-y-3">
            <li v-for="item in quickLinks" :key="item.path">
              <NuxtLink
                :to="item.path"
                class="text-slate-400 hover:text-white transition-colors flex items-center group"
              >
                <span class="w-1.5 h-1.5 rounded-full bg-slate-600 mr-2 group-hover:bg-blue-500 transition-colors"></span>
                {{ item.title }}
              </NuxtLink>
            </li>
          </ul>
        </div>

        <!-- 联系方式 -->
        <div>
          <h3 class="text-lg font-bold font-['Outfit'] mb-6 text-white">联系我</h3>
          <div class="space-y-4">
            <a
              v-for="contact in contactInfo"
              :key="contact.type"
              :href="contact.link"
              class="flex items-center space-x-3 text-slate-400 hover:text-white transition-colors group"
            >
              <span class="w-10 h-10 rounded-lg bg-slate-800 flex items-center justify-center group-hover:bg-blue-600/20 group-hover:text-blue-400 transition-all duration-300">
                {{ contact.icon }}
              </span>
              <span>{{ contact.label }}</span>
            </a>
          </div>
        </div>
      </div>

      <!-- 分割线 -->
      <div class="border-t border-slate-800 mt-16 pt-8">
        <div class="flex flex-col md:flex-row justify-between items-center">
          <p class="text-slate-500 text-sm">
            © {{ currentYear }} 溪午听风. All rights reserved.
          </p>
          <div class="flex items-center space-x-4">
            <span class="text-slate-500 text-sm flex items-center" v-if="stats">
              <span class="mr-3">今日访问: {{ stats.todayVisits }}</span>
              <span>总访问量: {{ stats.totalVisits }}</span>
            </span>
            <span class="text-slate-500 text-sm flex items-center">
              Built with <span class="text-red-500 mx-1">❤</span> & Nuxt 3
            </span>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
// 获取当前年份
const currentYear = new Date().getFullYear()

// 快速链接
const quickLinks = [
  { title: '插件工具', path: '/tools' },
  { title: '项目展示', path: '/projects' },
  { title: '技术博客', path: '/blog' },
  { title: '关于我', path: '/about' }
]

// 社交媒体链接 (需要引入 FontAwesome 或类似图标库，这里暂时用 emoji 或 class 占位)
const socialLinks = [
  { name: 'GitHub', icon: 'fab fa-github', link: 'https://github.com/Lijing327' },
  { name: 'WeChat', icon: 'fab fa-weixin', link: '#' },
  // { name: 'Twitter', icon: 'fab fa-twitter', link: '#' }
]

// 联系方式
const contactInfo = [
  {
    type: 'email',
    icon: '📧',
    label: 'contact@溪午听风.com',
    link: 'mailto:contact@溪午听风.com'
  },
  {
    type: 'github',
    icon: '🐙',
    label: 'Lijing327',
    link: 'https://github.com/Lijing327'
  }
]

// 统计数据
const { data: stats } = await useFetch('/api/stats')
</script>