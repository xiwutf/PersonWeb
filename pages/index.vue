<template>
  <div class="bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50">
    <!-- Hero Banner -->
    <section class="relative overflow-hidden">
      <!-- 背景装饰 -->
      <div class="absolute inset-0">
        <div class="absolute top-10 left-10 w-72 h-72 bg-blue-200 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob"></div>
        <div class="absolute top-0 right-4 w-72 h-72 bg-purple-200 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob animation-delay-2000"></div>
        <div class="absolute -bottom-8 left-20 w-72 h-72 bg-pink-200 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob animation-delay-4000"></div>
      </div>
      
      <!-- 主要内容 -->
      <div class="relative max-w-6xl mx-auto px-4 py-20 sm:py-32">
              <!-- 头部介绍 -->
        <div class="text-center">
          <div class="w-32 h-32 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full mx-auto mb-8 flex items-center justify-center shadow-xl">
            <span class="text-white text-4xl font-bold">XF</span>
          </div>
          <h1 class="text-5xl lg:text-7xl font-bold text-gray-800 mb-6">
            你好，我是 <span class="text-gradient bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">溪午听风</span>
          </h1>
          <div class="text-2xl lg:text-3xl text-gray-600 mb-6 font-medium">
            全栈开发者 · Revit插件专家 · 技术博主
          </div>
          <p class="text-xl text-gray-500 max-w-3xl mx-auto mb-8 leading-relaxed">
            <span class="block mb-2">🚀 开发让生活更高效，代码就是我的魔方</span>
            专注于Web开发、Revit插件开发和移动端应用开发，用技术解决实际问题，分享知识与经验
          </p>
          
          <!-- 快速导航按钮 -->
          <div class="flex flex-wrap justify-center gap-4 mt-12">
            <NuxtLink
              v-for="item in quickNavItems"
              :key="item.path"
              :to="item.path"
              class="group inline-flex items-center px-6 py-3 bg-white/80 backdrop-blur-sm rounded-full shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 border border-gray-200"
            >
              <span class="text-2xl mr-3 group-hover:scale-110 transition-transform">{{ item.icon }}</span>
              <span class="font-semibold text-gray-700 group-hover:text-gray-900">{{ item.title }}</span>
            </NuxtLink>
          </div>
        </div>
      </div>
    </section>

    <!-- 最新内容展示 -->
    <section class="py-20 bg-white">
      <div class="max-w-6xl mx-auto px-4">
        <div class="text-center mb-16">
          <h2 class="text-4xl font-bold text-gray-800 mb-4">最新内容</h2>
          <p class="text-xl text-gray-600">探索我的最新作品与技术分享</p>
        </div>
        
        <div class="grid lg:grid-cols-3 gap-8">
          <!-- 最新工具 -->
          <div class="bg-white rounded-2xl shadow-lg hover:shadow-xl transition-shadow duration-300 p-8 border border-gray-100">
            <div class="flex items-center mb-6">
              <div class="w-12 h-12 bg-blue-100 rounded-xl flex items-center justify-center mr-4">
                <span class="text-2xl">🔧</span>
              </div>
              <h3 class="text-xl font-bold text-gray-800">最新工具</h3>
            </div>
            <div class="space-y-4">
              <div class="border-l-4 border-blue-500 pl-4 py-2">
                <h4 class="font-semibold text-gray-800">批量族加载器</h4>
                <p class="text-sm text-gray-600 mt-1">快速批量加载Revit族文件，提升工作效率</p>
                <span class="inline-block mt-2 px-3 py-1 bg-blue-100 text-blue-800 text-xs rounded-full">¥19.9</span>
              </div>
              <div class="border-l-4 border-blue-300 pl-4 py-2">
                <h4 class="font-semibold text-gray-800">智能标注助手</h4>
                <p class="text-sm text-gray-600 mt-1">自动化标注生成工具，精准高效</p>
                <span class="inline-block mt-2 px-3 py-1 bg-blue-100 text-blue-800 text-xs rounded-full">¥29.9</span>
              </div>
            </div>
            <NuxtLink to="/tools" class="inline-flex items-center mt-6 text-blue-600 hover:text-blue-800 font-medium group">
              查看全部工具
              <svg class="w-4 h-4 ml-1 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
              </svg>
            </NuxtLink>
          </div>

          <!-- 最新项目 -->
          <div class="bg-white rounded-2xl shadow-lg hover:shadow-xl transition-shadow duration-300 p-8 border border-gray-100">
            <div class="flex items-center mb-6">
              <div class="w-12 h-12 bg-purple-100 rounded-xl flex items-center justify-center mr-4">
                <span class="text-2xl">🧪</span>
              </div>
              <h3 class="text-xl font-bold text-gray-800">最新项目</h3>
            </div>
            <div class="space-y-4">
              <div class="border-l-4 border-purple-500 pl-4 py-2">
                <h4 class="font-semibold text-gray-800">恋爱魔方小程序</h4>
                <p class="text-sm text-gray-600 mt-1">创新的交友互动平台，重新定义社交体验</p>
                <span class="inline-block mt-2 px-3 py-1 bg-green-100 text-green-800 text-xs rounded-full">已上线</span>
              </div>
              <div class="border-l-4 border-purple-300 pl-4 py-2">
                <h4 class="font-semibold text-gray-800">智能理财助手</h4>
                <p class="text-sm text-gray-600 mt-1">个性化理财规划工具，助力财富增长</p>
                <span class="inline-block mt-2 px-3 py-1 bg-yellow-100 text-yellow-800 text-xs rounded-full">开发中</span>
              </div>
            </div>
            <NuxtLink to="/projects" class="inline-flex items-center mt-6 text-purple-600 hover:text-purple-800 font-medium group">
              查看全部项目
              <svg class="w-4 h-4 ml-1 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
              </svg>
            </NuxtLink>
          </div>

          <!-- 最新博客 -->
          <div class="bg-white rounded-2xl shadow-lg hover:shadow-xl transition-shadow duration-300 p-8 border border-gray-100">
            <div class="flex items-center mb-6">
              <div class="w-12 h-12 bg-green-100 rounded-xl flex items-center justify-center mr-4">
                <span class="text-2xl">📝</span>
              </div>
              <h3 class="text-xl font-bold text-gray-800">最新博客</h3>
            </div>
            <div class="space-y-4">
              <div class="border-l-4 border-green-500 pl-4 py-2">
                <h4 class="font-semibold text-gray-800">Nuxt 3 项目实战</h4>
                <p class="text-sm text-gray-600 mt-1">从零搭建现代化个人网站的完整指南</p>
                <span class="inline-block mt-2 px-3 py-1 bg-gray-100 text-gray-800 text-xs rounded-full">技术教程</span>
              </div>
              <div class="border-l-4 border-green-300 pl-4 py-2">
                <h4 class="font-semibold text-gray-800">程序员理财指南</h4>
                <p class="text-sm text-gray-600 mt-1">技术人的投资思考与实战经验分享</p>
                <span class="inline-block mt-2 px-3 py-1 bg-gray-100 text-gray-800 text-xs rounded-full">理财心得</span>
              </div>
            </div>
            <NuxtLink to="/blog" class="inline-flex items-center mt-6 text-green-600 hover:text-green-800 font-medium group">
              查看全部文章
              <svg class="w-4 h-4 ml-1 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
              </svg>
            </NuxtLink>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA 区域 -->
    <section class="py-20 bg-gradient-to-r from-blue-600 to-purple-600">
      <div class="max-w-4xl mx-auto px-4 text-center">
        <h2 class="text-3xl sm:text-4xl font-bold text-white mb-6">
          让我们一起创造更高效的解决方案
        </h2>
        <p class="text-xl text-blue-100 mb-8 max-w-2xl mx-auto">
          无论是定制化插件开发、项目合作还是技术咨询，我都乐意与您交流探讨
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <NuxtLink
            to="/about"
            class="inline-flex items-center px-8 py-4 bg-white text-blue-600 rounded-xl font-semibold hover:bg-gray-50 transition-colors shadow-lg"
          >
            <span class="mr-2">👋</span>
            了解更多关于我
          </NuxtLink>
          <a
            href="mailto:contact@溪午听风.com"
            class="inline-flex items-center px-8 py-4 border-2 border-white text-white rounded-xl font-semibold hover:bg-white hover:text-blue-600 transition-colors"
          >
            <span class="mr-2">📧</span>
            联系合作
          </a>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
// 快速导航项配置
const quickNavItems = [
  {
    title: '插件工具',
    path: '/tools',
    icon: '🔧'
  },
  {
    title: '项目展示',
    path: '/projects',
    icon: '🧪'
  },
  {
    title: '技术博客',
    path: '/blog',
    icon: '📝'
  },
  {
    title: '关于我',
    path: '/about',
    icon: '👤'
  }
]

// 设置页面标题和SEO
useHead({
  title: '溪午听风 - 全栈开发者与Revit插件专家',
  meta: [
    { name: 'description', content: '溪午听风个人网站，专注于Web开发、Revit插件开发和移动端应用开发，分享技术心得与项目经验' },
    { name: 'keywords', content: '溪午听风, 全栈开发, Revit插件, 技术博客, 个人网站, 前端开发, 后端开发' }
  ]
})
</script>

<style scoped>
.text-gradient {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* 流动的blob动画 */
@keyframes blob {
  0% {
    transform: translate(0px, 0px) scale(1);
  }
  33% {
    transform: translate(30px, -50px) scale(1.1);
  }
  66% {
    transform: translate(-20px, 20px) scale(0.9);
  }
  100% {
    transform: translate(0px, 0px) scale(1);
  }
}

.animate-blob {
  animation: blob 7s infinite;
}

.animation-delay-2000 {
  animation-delay: 2s;
}

.animation-delay-4000 {
  animation-delay: 4s;
}

/* 页面淡入动画 */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 让整个页面有淡入效果 */
section {
  animation: fadeInUp 0.8s ease-out;
}

/* 为不同section添加延迟 */
section:nth-child(2) {
  animation-delay: 0.2s;
}

section:nth-child(3) {
  animation-delay: 0.4s;
}
</style>
