<template>
  <div class="min-h-screen bg-slate-50 overflow-x-hidden selection:bg-blue-500/30">
    <!-- 全局噪点纹理 -->
    <div class="fixed inset-0 pointer-events-none opacity-[0.03] z-50 mix-blend-overlay" style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);"></div>

    <!-- Hero Banner with Immersive 3D -->
    <section class="relative min-h-[100vh] flex items-center justify-center overflow-hidden bg-[#0f172a]">
      <!-- 流体背景 -->
      <FluidBackground :intensity="0.3" color="#3b82f6" :interactive="true" />
      
      <!-- 沉浸式3D背景 -->
      <div class="absolute inset-0 z-0">
        <Immersive3DScene :show-hint="false" :scroll-enabled="true" />
      </div>
      
      <!-- 动态背景装饰 -->
      <div class="absolute inset-0 overflow-hidden pointer-events-none z-[1]">
        <div class="absolute top-[-10%] left-[-10%] w-[50%] h-[50%] bg-blue-600/20 rounded-full mix-blend-screen filter blur-[100px] animate-blob"></div>
        <div class="absolute top-[20%] right-[-10%] w-[40%] h-[40%] bg-purple-600/20 rounded-full mix-blend-screen filter blur-[100px] animate-blob animation-delay-2000"></div>
        <div class="absolute bottom-[-10%] left-[20%] w-[40%] h-[40%] bg-cyan-600/20 rounded-full mix-blend-screen filter blur-[100px] animate-blob animation-delay-4000"></div>
        
        <!-- 网格背景 -->
        <div class="absolute inset-0 bg-[url('/images/grid.svg')] bg-center [mask-image:linear-gradient(180deg,white,rgba(255,255,255,0))] opacity-20"></div>
      </div>
      
      <!-- 主要内容 -->
      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 w-full z-10">
        <div class="grid lg:grid-cols-2 gap-12 items-center">
          <!-- 左侧文本 -->
          <div class="text-center lg:text-left space-y-8" data-aos="fade-right">
            <div class="inline-flex items-center px-4 py-2 bg-white/5 backdrop-blur-md rounded-full border border-white/10 mb-4 animate-float hover:bg-white/10 transition-colors cursor-default">
              <span class="flex h-2 w-2 relative mr-3">
                <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-green-400 opacity-75"></span>
                <span class="relative inline-flex rounded-full h-2 w-2 bg-green-500"></span>
              </span>
              <span class="text-blue-200 font-medium text-sm">欢迎来到我的数字花园</span>
            </div>
            
            <h1 class="text-5xl lg:text-7xl font-bold text-white tracking-tight leading-tight">
              你好，我是 <br/>
              <span class="text-transparent bg-clip-text bg-gradient-to-r from-blue-400 via-purple-400 to-cyan-400 animate-gradient-x">溪午听风</span>
            </h1>
            
            <!-- 轮播文字 -->
            <div class="h-12 text-2xl lg:text-3xl text-slate-400 font-medium flex items-center justify-center lg:justify-start overflow-hidden">
              <span class="mr-3">我是</span>
              <div class="relative h-full w-64 text-left">
                <transition-group name="slide-up" tag="div" class="absolute top-0 left-0 w-full">
                  <span v-if="currentRoleIndex === 0" key="0" class="block text-blue-400 font-bold">全栈开发者</span>
                  <span v-if="currentRoleIndex === 1" key="1" class="block text-purple-400 font-bold">AI应用探索者</span>
                  <span v-if="currentRoleIndex === 2" key="2" class="block text-cyan-400 font-bold">Revit插件专家</span>
                  <span v-if="currentRoleIndex === 3" key="3" class="block text-orange-400 font-bold">终身学习者</span>
                </transition-group>
              </div>
            </div>
            
            <p class="text-lg text-slate-400 max-w-2xl mx-auto lg:mx-0 leading-relaxed">
              专注于构建高效、优雅的数字体验。无论是复杂的企业级应用，还是有趣的创意小工具，我都乐在其中。
            </p>
            
            <div class="flex flex-wrap justify-center lg:justify-start gap-4 pt-4">
              <NuxtLink to="/projects" class="relative inline-flex group">
                <div class="absolute transition-all duration-1000 opacity-70 -inset-px bg-gradient-to-r from-[#44BCFF] via-[#FF44EC] to-[#FF675E] rounded-xl blur-lg group-hover:opacity-100 group-hover:-inset-1 group-hover:duration-200 animate-tilt"></div>
                <button class="relative inline-flex items-center justify-center px-8 py-4 text-lg font-bold text-white transition-all duration-200 bg-slate-900 font-pj rounded-xl focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-900">
                  浏览项目
                  <i class="fas fa-arrow-right ml-2 group-hover:translate-x-1 transition-transform"></i>
                </button>
              </NuxtLink>
              
              <NuxtLink to="/about" class="inline-flex items-center justify-center px-8 py-4 text-lg font-bold text-slate-300 transition-all duration-200 bg-transparent border border-slate-700 rounded-xl hover:bg-slate-800 hover:text-white hover:border-slate-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-900">
                关于我
              </NuxtLink>
            </div>
          </div>
          
          <!-- 右侧视觉元素 - 3D粒子头像 -->
          <div class="relative hidden lg:block" data-aos="fade-left">
            <div class="relative w-full max-w-sm mx-auto aspect-square">
              <!-- 光晕背景 -->
              <div class="absolute inset-0 bg-gradient-to-tr from-blue-500 to-purple-500 rounded-[2rem] rotate-6 opacity-30 blur-2xl animate-pulse"></div>
              
              <GlassCard class="w-full h-full rounded-[2rem] overflow-hidden">
                <div class="w-full h-full relative">
                  <ParticleAvatar
                    :image-url="'/images/avatar.jpg'"
                    :text="'溪午听风'"
                    :subtitle="'全栈开发者'"
                    :particle-count="2000"
                    :interactive="true"
                    class="w-full h-full"
                  />
                  
                  <!-- 悬浮标签 -->
                  <div class="absolute bottom-6 left-6 right-6 p-4 bg-white/10 backdrop-blur-md rounded-xl border border-white/20 shadow-lg z-20">
                    <div class="flex items-center justify-between">
                      <div>
                        <h3 class="font-bold text-white">全栈开发者</h3>
                        <p class="text-xs text-slate-300">Vue / Nuxt / Node.js</p>
                      </div>
                      <div class="w-10 h-10 bg-blue-500/20 rounded-full flex items-center justify-center text-blue-400 border border-blue-500/30">
                        <i class="fas fa-code"></i>
                      </div>
                    </div>
                  </div>
                </div>
              </GlassCard>
              
              <!-- 装饰性悬浮卡片 -->
              <div class="absolute -right-8 top-12 p-3 bg-slate-800/80 backdrop-blur-md rounded-xl shadow-xl border border-white/10 animate-float-delayed w-40 z-20">
                <div class="flex items-center gap-2 mb-1">
                  <div class="w-6 h-6 bg-green-500/20 rounded-md flex items-center justify-center text-green-400 text-xs border border-green-500/30">
                    <i class="fas fa-check"></i>
                  </div>
                  <span class="font-semibold text-xs text-slate-200">项目已上线</span>
                </div>
                <div class="w-full bg-slate-700 rounded-full h-1">
                  <div class="bg-green-500 h-1 rounded-full w-full shadow-[0_0_10px_rgba(34,197,94,0.5)]"></div>
                </div>
              </div>
              
              <div class="absolute -left-6 bottom-24 p-3 bg-slate-800/80 backdrop-blur-md rounded-xl shadow-xl border border-white/10 animate-float w-36 z-20">
                <div class="flex items-center gap-2">
                  <div class="w-6 h-6 bg-purple-500/20 rounded-md flex items-center justify-center text-purple-400 text-xs border border-purple-500/30">
                    <i class="fas fa-magic"></i>
                  </div>
                  <div>
                    <div class="font-semibold text-xs text-slate-200">AI 驱动</div>
                    <div class="text-[10px] text-slate-400">智能助手</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 滚动提示 -->
        <div class="absolute bottom-10 left-1/2 -translate-x-1/2 cursor-pointer group" @click="scrollToContent">
          <div class="flex flex-col items-center gap-2">
            <span class="text-xs text-slate-500 uppercase tracking-widest group-hover:text-blue-400 transition-colors">Scroll</span>
            <div class="w-5 h-8 border-2 border-slate-600 rounded-full flex justify-center p-1 group-hover:border-blue-400 transition-colors">
              <div class="w-1 h-2 bg-slate-500 rounded-full animate-scroll-down group-hover:bg-blue-400"></div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 成长轨迹时间线 -->
    <Timeline />

    <!-- 3D 互动场景 -->
    <section class="py-16 bg-gradient-to-b from-[#0f172a] to-slate-900 relative overflow-hidden">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <h2 class="text-3xl lg:text-4xl font-bold text-white mb-4">互动探索</h2>
          <p class="text-lg text-slate-400">点击 3D 对象，探索不同内容</p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <!-- 地球 - 跳转到博客 -->
          <div class="bg-slate-800/50 backdrop-blur-md rounded-2xl p-6 border border-white/10 hover:border-blue-500/50 transition-all">
            <h3 class="text-white font-bold mb-4 text-center">🌍 博客文章</h3>
            <Scene3D type="earth" :show-hint="false" height="300px" />
            <p class="text-slate-400 text-sm text-center mt-4">点击地球查看技术博客</p>
          </div>
          
          <!-- 飞船 - 跳转到项目 -->
          <div class="bg-slate-800/50 backdrop-blur-md rounded-2xl p-6 border border-white/10 hover:border-purple-500/50 transition-all">
            <h3 class="text-white font-bold mb-4 text-center">🚀 项目作品</h3>
            <Scene3D type="spaceship" :show-hint="false" height="300px" />
            <p class="text-slate-400 text-sm text-center mt-4">点击飞船查看项目作品</p>
          </div>
          
          <!-- 数据星球 - 跳转到仪表盘 -->
          <div class="bg-slate-800/50 backdrop-blur-md rounded-2xl p-6 border border-white/10 hover:border-green-500/50 transition-all">
            <h3 class="text-white font-bold mb-4 text-center">💎 数据仪表盘</h3>
            <Scene3D type="datasphere" :show-hint="false" height="300px" />
            <p class="text-slate-400 text-sm text-center mt-4">点击数据星球查看数据</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Bento Grid 内容展示 -->
    <section id="content" class="py-24 bg-slate-50 relative">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16" data-aos="fade-up">
          <h2 class="text-3xl lg:text-4xl font-bold text-slate-900 mb-4">探索我的世界</h2>
          <p class="text-lg text-slate-600">这里有代码、有思考、也有生活</p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 auto-rows-[minmax(200px,auto)]">
          <!-- 1. 最新博客 (大卡片) -->
          <div class="md:col-span-2 lg:col-span-2 row-span-2" data-aos="fade-up">
            <TiltCard class="h-full">
              <div class="group relative h-full overflow-hidden rounded-3xl shadow-lg bg-white p-8 flex flex-col justify-between border border-slate-100 hover:border-blue-200 transition-all duration-300">
                <!-- 渐变背景 -->
                <div class="absolute inset-0 bg-gradient-to-br from-blue-50 to-indigo-50 opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
                
                <div class="relative z-10">
                  <div class="inline-flex items-center px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-sm mb-4 font-medium">
                    <i class="fas fa-pen-fancy mr-2"></i> 最新博客
                  </div>
                  <h3 class="text-3xl font-bold text-slate-900 mb-2 group-hover:text-blue-700 transition-colors">技术探索与分享</h3>
                  <p class="text-slate-600 line-clamp-2">记录学习过程中的点滴，分享解决问题的思路与方案。</p>
                </div>
                
                  <div class="mt-8 space-y-3 relative z-10">
                    <div v-if="latestPosts && latestPosts.length > 0">
                      <NuxtLink 
                        v-for="post in latestPosts" 
                        :key="post.id" 
                        :to="`/blog/${post.slug || post.id}`" 
                        class="block p-4 bg-white rounded-xl shadow-sm border border-slate-100 hover:shadow-md hover:border-blue-200 transition-all group/item mb-3"
                      >
                        <div class="flex justify-between items-start">
                          <div class="font-semibold text-slate-800 group-hover/item:text-blue-600 transition-colors truncate pr-4">{{ post.title }}</div>
                          <i class="fas fa-arrow-right text-slate-300 group-hover/item:text-blue-500 transform group-hover/item:translate-x-1 transition-all"></i>
                        </div>
                        <div class="text-xs text-slate-500 mt-1">{{ formatDate(post.publishTime || post.createdAt) }} · {{ post.categoryName || '未分类' }}</div>
                      </NuxtLink>
                    </div>
                    <div v-else class="text-center text-slate-500 py-4">
                      暂无最新文章
                    </div>
                  </div>
                </div>
              </TiltCard>
            </div>

          <!-- 2. 工具箱 -->
          <div class="md:col-span-1 lg:col-span-1 row-span-1" data-aos="fade-up" data-aos-delay="100">
            <TiltCard class="h-full">
              <div class="h-full bg-white rounded-3xl shadow-lg p-6 border border-slate-100 group hover:border-blue-200 hover:shadow-xl transition-all duration-300 flex flex-col relative overflow-hidden">
                <div class="absolute -right-6 -top-6 w-24 h-24 bg-blue-50 rounded-full group-hover:scale-150 transition-transform duration-500"></div>
                
                <div class="relative z-10 flex flex-col h-full">
                  <div class="w-12 h-12 bg-blue-100 rounded-2xl flex items-center justify-center text-blue-600 text-2xl mb-4 group-hover:rotate-12 transition-transform shadow-sm">
                    <i class="fas fa-tools"></i>
                  </div>
                  <h3 class="text-xl font-bold text-slate-800 mb-2">效率工具</h3>
                  <p class="text-sm text-slate-500 mb-4 flex-grow">Revit插件与自动化脚本，提升工作效率。</p>
                  <NuxtLink to="/tools" class="text-blue-600 font-medium text-sm hover:underline flex items-center mt-auto group/link">
                    查看工具库 <i class="fas fa-arrow-right ml-1 text-xs group-hover/link:translate-x-1 transition-transform"></i>
                  </NuxtLink>
                </div>
              </div>
            </TiltCard>
          </div>

          <!-- 3. 项目展示 -->
          <div class="md:col-span-1 lg:col-span-1 row-span-1" data-aos="fade-up" data-aos-delay="200">
            <TiltCard class="h-full">
              <div class="h-full bg-slate-900 rounded-3xl shadow-lg p-6 text-white overflow-hidden relative group flex flex-col border border-slate-800 hover:border-purple-500/50 transition-colors">
                <div class="absolute top-0 right-0 w-32 h-32 bg-purple-500/20 rounded-full blur-2xl -mr-10 -mt-10 group-hover:bg-purple-500/30 transition-colors"></div>
                <div class="absolute bottom-0 left-0 w-24 h-24 bg-blue-500/10 rounded-full blur-xl -ml-10 -mb-10 group-hover:bg-blue-500/20 transition-colors"></div>
                
                <div class="relative z-10 flex flex-col h-full">
                  <div class="w-12 h-12 bg-white/10 rounded-2xl flex items-center justify-center text-purple-300 text-2xl mb-4 group-hover:scale-110 transition-transform backdrop-blur-sm border border-white/10">
                    <i class="fas fa-project-diagram"></i>
                  </div>
                  <h3 class="text-xl font-bold mb-2">精选项目</h3>
                  <p class="text-sm text-slate-400 mb-4 flex-grow">实战项目与开源贡献，探索技术边界。</p>
                  <NuxtLink to="/projects" class="text-purple-300 font-medium text-sm hover:text-purple-200 flex items-center mt-auto group/link">
                    浏览作品集 <i class="fas fa-arrow-right ml-1 text-xs group-hover/link:translate-x-1 transition-transform"></i>
                  </NuxtLink>
                </div>
              </div>
            </TiltCard>
          </div>

          <!-- 4. 生活随笔 -->
          <div class="md:col-span-2 lg:col-span-2 row-span-1" data-aos="fade-up" data-aos-delay="300">
            <TiltCard class="h-full">
              <div class="h-full bg-gradient-to-r from-orange-50 to-amber-50 rounded-3xl shadow-lg p-6 border border-orange-100 flex flex-col md:flex-row items-center gap-6 group hover:shadow-xl transition-all duration-300">
                <div class="flex-1">
                  <div class="w-12 h-12 bg-orange-100 rounded-2xl flex items-center justify-center text-orange-600 text-2xl mb-4 shadow-sm group-hover:scale-110 transition-transform">
                    <i class="fas fa-coffee"></i>
                  </div>
                  <h3 class="text-xl font-bold text-slate-800 mb-2">生活随笔</h3>
                  <p class="text-sm text-slate-500 mb-4">代码之外的诗与远方，记录生活中的美好瞬间。</p>
                  <NuxtLink to="/life" class="inline-flex items-center px-5 py-2.5 bg-orange-500 text-white rounded-xl text-sm font-medium hover:bg-orange-600 transition-all shadow-lg shadow-orange-500/30 hover:shadow-orange-500/40 hover:-translate-y-0.5">
                    进入生活专栏
                  </NuxtLink>
                </div>
                <div class="w-full md:w-1/2 space-y-3">
                   <div class="p-3 bg-white rounded-xl shadow-sm border border-orange-100/50 flex items-center gap-3 hover:shadow-md transition-shadow cursor-pointer">
                      <div class="w-12 h-12 bg-gray-100 rounded-lg flex-shrink-0 overflow-hidden">
                         <img src="/images/blog/thermal-circulation.png" class="w-full h-full object-cover transform hover:scale-110 transition-transform duration-500" alt="cover">
                      </div>
                      <div>
                        <div class="text-[10px] text-orange-500 font-bold uppercase tracking-wider mb-0.5">最近更新</div>
                        <div class="font-medium text-slate-700 text-sm line-clamp-1 group-hover:text-orange-600 transition-colors">地理科普：一文读懂热力环流</div>
                      </div>
                   </div>
                </div>
              </div>
            </TiltCard>
          </div>

          <!-- 5. 关于我 -->
          <div class="md:col-span-2 lg:col-span-2 row-span-1" data-aos="fade-up" data-aos-delay="400">
            <TiltCard class="h-full">
              <div class="h-full bg-white rounded-3xl shadow-lg p-8 border border-slate-100 flex flex-col justify-center items-center text-center gap-4 group hover:border-blue-200 transition-colors relative overflow-hidden">
                <div class="absolute inset-0 bg-slate-50 opacity-0 group-hover:opacity-100 transition-opacity duration-500 pointer-events-none"></div>
                
                <div class="relative z-10">
                  <h3 class="text-2xl font-bold text-slate-800 mb-2">想要了解更多？</h3>
                  <p class="text-slate-600 text-sm max-w-md mx-auto">无论是技术交流、项目合作，还是交个朋友，都随时欢迎。</p>
                </div>
                <div class="flex gap-4 mt-2 relative z-10">
                  <a href="https://github.com" target="_blank" class="w-10 h-10 bg-slate-100 rounded-full flex items-center justify-center text-slate-700 hover:bg-slate-900 hover:text-white transition-all duration-300 hover:-translate-y-1">
                    <i class="fab fa-github"></i>
                  </a>
                  <a href="mailto:contact@example.com" class="w-10 h-10 bg-blue-50 rounded-full flex items-center justify-center text-blue-600 hover:bg-blue-600 hover:text-white transition-all duration-300 hover:-translate-y-1">
                    <i class="fas fa-envelope"></i>
                  </a>
                  <NuxtLink to="/about" class="px-6 py-2 bg-slate-900 text-white rounded-full text-sm font-medium hover:bg-slate-800 transition-all shadow-lg shadow-slate-900/20 hover:shadow-slate-900/40 hover:-translate-y-1">
                    查看详细介绍
                  </NuxtLink>
                </div>
              </div>
            </TiltCard>
          </div>
        </div>
      </div>
    </section>

    <!-- 时间胶囊墙展示区域 -->
    <section class="py-24 bg-gradient-to-b from-slate-50 to-white relative">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16" data-aos="fade-up">
          <h2 class="text-3xl lg:text-4xl font-bold text-slate-900 mb-4">时间胶囊墙</h2>
          <p class="text-lg text-slate-600">访客留下的足迹与祝福</p>
        </div>
        
        <TimeCapsuleDisplay />
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const api = useApi()
const latestPosts = ref<any[]>([])

// Fetch latest posts
const fetchLatestPosts = async () => {
  try {
    const res = await api.get<any>('/Articles', {
      params: {
        page: 1,
        pageSize: 2
      }
    })
    latestPosts.value = res.list
  } catch (e) {
    console.error('Failed to fetch latest posts', e)
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('zh-CN')
}

// 轮播文字逻辑
const currentRoleIndex = ref(0)
let roleInterval = null

const startRoleRotation = () => {
  roleInterval = setInterval(() => {
    currentRoleIndex.value = (currentRoleIndex.value + 1) % 4
  }, 3000) // 每3秒切换一次
}

// 滚动功能
const scrollToContent = () => {
  const contentSection = document.getElementById('content')
  if (contentSection) {
    contentSection.scrollIntoView({ behavior: 'smooth' })
  }
}

// 简单的滚动动画观察器 (模拟 AOS)
let observer = null

onMounted(() => {
  fetchLatestPosts()
  startRoleRotation()

  // 设置 Intersection Observer
  observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('aos-animate')
      }
    })
  }, { threshold: 0.1 })

  document.querySelectorAll('[data-aos]').forEach(el => {
    observer.observe(el)
  })
})

onUnmounted(() => {
  if (roleInterval) clearInterval(roleInterval)
  if (observer) observer.disconnect()
})

definePageMeta({
  layout: 'home'
})

useHead({
  title: '溪午听风 - 数字花园',
  meta: [
    { name: 'description', content: '溪午听风的个人网站，分享技术、生活与思考。' }
  ]
})
</script>

<style scoped>
/* 动画关键帧定义 */
@keyframes blob {
  0% { transform: translate(0px, 0px) scale(1); }
  33% { transform: translate(30px, -50px) scale(1.1); }
  66% { transform: translate(-20px, 20px) scale(0.9); }
  100% { transform: translate(0px, 0px) scale(1); }
}

@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}

@keyframes scroll-down {
  0% { transform: translateY(0); opacity: 1; }
  100% { transform: translateY(6px); opacity: 0; }
}

@keyframes gradient-x {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

@keyframes tilt {
  0%, 50%, 100% { transform: rotate(0deg); }
  25% { transform: rotate(0.5deg); }
  75% { transform: rotate(-0.5deg); }
}

/* 实用类 */
.animate-blob {
  animation: blob 7s infinite;
}

.animation-delay-2000 {
  animation-delay: 2s;
}

.animation-delay-4000 {
  animation-delay: 4s;
}

.animate-float {
  animation: float 3s ease-in-out infinite;
}

.animate-float-delayed {
  animation: float 3s ease-in-out 1.5s infinite;
}

.animate-scroll-down {
  animation: scroll-down 1.5s infinite;
}

.animate-gradient-x {
  background-size: 200% 200%;
  animation: gradient-x 3s ease infinite;
}

.animate-tilt {
  animation: tilt 10s infinite linear;
}

/* 文字轮播动画 */
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

/* AOS 初始状态 */
[data-aos] {
  opacity: 0;
  transform: translateY(20px);
  transition: opacity 0.6s ease-out, transform 0.6s ease-out;
}

[data-aos].aos-animate {
  opacity: 1;
  transform: translateY(0);
}

[data-aos="fade-right"] {
  transform: translateX(-20px);
}

[data-aos="fade-left"] {
  transform: translateX(20px);
}

[data-aos].aos-animate[data-aos="fade-right"],
[data-aos].aos-animate[data-aos="fade-left"] {
  transform: translateX(0);
}
</style>

