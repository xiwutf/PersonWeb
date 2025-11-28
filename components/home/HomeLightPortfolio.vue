<template>
  <div class="min-h-screen bg-white overflow-x-hidden">
    <!-- 添加到主屏幕提示 -->
    <AddToHomeScreen />

    <!-- Hero Section - 浅色作品集风格 -->
    <section class="relative min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 via-white to-purple-50">
      <div class="absolute inset-0 overflow-hidden pointer-events-none">
        <div class="absolute top-0 right-0 w-96 h-96 bg-blue-100/30 rounded-full blur-3xl"></div>
        <div class="absolute bottom-0 left-0 w-96 h-96 bg-purple-100/30 rounded-full blur-3xl"></div>
      </div>

      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 w-full z-10">
        <div class="grid lg:grid-cols-2 gap-12 items-center">
          <!-- 左侧内容 -->
          <div class="text-center lg:text-left space-y-6">
            <div class="inline-flex items-center px-4 py-2 bg-blue-100 text-blue-700 rounded-full text-sm font-medium mb-4">
              <span class="w-2 h-2 bg-blue-500 rounded-full mr-2"></span>
              全栈开发者 & AI 探索者
            </div>

            <h1 class="text-4xl sm:text-5xl md:text-6xl lg:text-7xl font-bold text-gray-900 leading-tight">
              你好，我是
              <span class="block mt-2 text-transparent bg-clip-text bg-gradient-to-r from-blue-600 to-purple-600">
                溪午听风
              </span>
            </h1>

            <p class="text-lg sm:text-xl text-gray-600 max-w-xl mx-auto lg:mx-0 leading-relaxed">
              专注构建高效、优雅的数字体验。
              无论是企业级业务系统、AI 驱动应用，还是有趣的小工具，我都致力于把复杂问题做得简单好用。
            </p>

            <div class="flex flex-wrap justify-center lg:justify-start gap-3 pt-4">
              <NuxtLink
                to="/projects"
                class="inline-flex items-center px-8 py-4 bg-blue-600 text-white rounded-xl font-semibold hover:bg-blue-700 transition-all shadow-lg shadow-blue-500/30 hover:shadow-blue-500/50 hover:-translate-y-1"
              >
                浏览项目
                <i class="fas fa-arrow-right ml-2"></i>
              </NuxtLink>

              <NuxtLink
                to="/about"
                class="inline-flex items-center px-8 py-4 bg-white text-gray-700 border-2 border-gray-300 rounded-xl font-semibold hover:border-gray-400 hover:bg-gray-50 transition-all"
              >
                关于我
              </NuxtLink>
            </div>
          </div>

          <!-- 右侧头像 -->
          <div class="relative hidden lg:block">
            <div class="relative w-full max-w-md mx-auto">
              <div class="absolute inset-0 bg-gradient-to-br from-blue-200 to-purple-200 rounded-3xl transform rotate-6"></div>
              <div class="relative bg-white rounded-3xl p-8 shadow-2xl">
                <img
                  src="/images/avatar.jpg"
                  alt="溪午听风"
                  class="w-full rounded-2xl mb-4"
                />
                <div class="text-center">
                  <h3 class="text-xl font-bold text-gray-900 mb-1">溪午听风</h3>
                  <p class="text-sm text-gray-600 mb-4">全栈开发 · AI 应用 · Revit 插件</p>
                  <div class="flex flex-wrap justify-center gap-2">
                    <span class="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-xs font-medium">Vue · Nuxt</span>
                    <span class="px-3 py-1 bg-green-100 text-green-700 rounded-full text-xs font-medium">.NET · Node.js</span>
                    <span class="px-3 py-1 bg-purple-100 text-purple-700 rounded-full text-xs font-medium">AI 工具链</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 内容展示区 -->
    <section id="content" class="py-24 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl lg:text-4xl font-bold text-gray-900 mb-3">探索我的世界</h2>
          <p class="text-lg text-gray-600">这里有代码、有思考，也有生活</p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <!-- 最新博客 -->
          <div class="bg-white rounded-2xl shadow-lg border border-gray-100 p-6 hover:shadow-xl transition-all">
            <div class="flex items-center gap-2 mb-4">
              <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center text-blue-600">
                <i class="fas fa-pen-fancy"></i>
              </div>
              <h3 class="text-xl font-bold text-gray-900">最新博客</h3>
            </div>
            <p class="text-gray-600 mb-4">记录学习过程中的洞察，与解决问题的完整思路。</p>
            <div v-if="latestPosts && latestPosts.length > 0" class="space-y-2">
              <NuxtLink
                v-for="post in latestPosts"
                :key="post.id"
                :to="`/blog/${post.slug || post.id}`"
                class="block p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
              >
                <div class="font-medium text-gray-900 text-sm">{{ post.title }}</div>
                <div class="text-xs text-gray-500 mt-1">
                  {{ formatDate(post.publishTime || post.createdAt) }}
                </div>
              </NuxtLink>
            </div>
            <NuxtLink to="/blog" class="inline-flex items-center text-blue-600 hover:text-blue-700 mt-4 text-sm font-medium">
              查看更多
              <i class="fas fa-arrow-right ml-1"></i>
            </NuxtLink>
          </div>

          <!-- 项目展示 -->
          <div class="bg-white rounded-2xl shadow-lg border border-gray-100 p-6 hover:shadow-xl transition-all">
            <div class="flex items-center gap-2 mb-4">
              <div class="w-10 h-10 bg-purple-100 rounded-lg flex items-center justify-center text-purple-600">
                <i class="fas fa-project-diagram"></i>
              </div>
              <h3 class="text-xl font-bold text-gray-900">精选项目</h3>
            </div>
            <p class="text-gray-600 mb-4">实战项目与开源尝试，持续拓展技术边界。</p>
            <NuxtLink
              to="/projects"
              class="inline-flex items-center px-6 py-3 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-all font-medium"
            >
              浏览作品集
              <i class="fas fa-arrow-right ml-2"></i>
            </NuxtLink>
          </div>

          <!-- AI 实验室 -->
          <div class="bg-white rounded-2xl shadow-lg border border-gray-100 p-6 hover:shadow-xl transition-all">
            <div class="flex items-center gap-2 mb-4">
              <div class="w-10 h-10 bg-cyan-100 rounded-lg flex items-center justify-center text-cyan-600">
                <i class="fas fa-flask"></i>
              </div>
              <h3 class="text-xl font-bold text-gray-900">AI 实验室</h3>
            </div>
            <p class="text-gray-600 mb-4">3D 场景、AI 小实验与互动体验的集合地。</p>
            <NuxtLink
              to="/lab"
              class="inline-flex items-center px-6 py-3 bg-cyan-600 text-white rounded-lg hover:bg-cyan-700 transition-all font-medium"
            >
              进入实验室
              <i class="fas fa-arrow-right ml-2"></i>
            </NuxtLink>
          </div>
        </div>
      </div>
    </section>

    <!-- 时间胶囊 -->
    <section class="py-24 bg-gray-50">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl lg:text-4xl font-bold text-gray-900 mb-3">时间胶囊墙</h2>
          <p class="text-lg text-gray-600">访客留下的足迹与祝福</p>
        </div>
        <TimeCapsuleDisplay />
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const api = useApi()
const latestPosts = ref<any[]>([])

const fetchLatestPosts = async () => {
  try {
    const res = await api.get<any>('/Articles', {
      params: {
        page: 1,
        pageSize: 3
      }
    })
    latestPosts.value = res.list || []
  } catch (e) {
    console.error('Failed to fetch latest posts', e)
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('zh-CN')
}

onMounted(() => {
  fetchLatestPosts()
})
</script>

