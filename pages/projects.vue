<template>
  <div class="bg-gradient-to-br from-purple-50 via-pink-50 to-indigo-50">
    <!-- 页面头部 -->
    <section class="py-16 bg-gradient-to-r from-purple-600 to-pink-600 text-white">
      <div class="max-w-6xl mx-auto px-4 text-center">
        <div class="w-20 h-20 bg-white/20 rounded-2xl flex items-center justify-center mx-auto mb-6">
          <span class="text-3xl">🧪</span>
        </div>
        <h1 class="text-4xl lg:text-5xl font-bold mb-4">项目展示</h1>
        <p class="text-xl text-purple-100 max-w-3xl mx-auto">
          精选个人项目作品与技术实践，从创意到实现的完整展示
        </p>
      </div>
    </section>

    <!-- 项目展示 -->
    <section class="py-20">
      <div class="max-w-6xl mx-auto px-4">
        <div v-if="pending" class="text-center py-16">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600"></div>
          <p class="mt-4 text-lg text-gray-600">加载中...</p>
        </div>

        <div v-else-if="error" class="text-center py-16">
          <div class="text-6xl mb-4">😵</div>
          <h3 class="text-xl font-semibold text-gray-800 mb-2">加载失败</h3>
          <p class="text-gray-600">{{ error }}</p>
        </div>

        <div v-else>
          <!-- 统计信息 -->
          <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-16">
            <div class="bg-white rounded-2xl shadow-lg p-6 text-center">
              <div class="text-3xl font-bold text-purple-600 mb-2">{{ projects.length }}+</div>
              <div class="text-gray-600">项目作品</div>
            </div>
            <div class="bg-white rounded-2xl shadow-lg p-6 text-center">
              <div class="text-3xl font-bold text-pink-600 mb-2">{{ onlineProjects }}</div>
              <div class="text-gray-600">已上线</div>
            </div>
            <div class="bg-white rounded-2xl shadow-lg p-6 text-center">
              <div class="text-3xl font-bold text-indigo-600 mb-2">{{ uniqueTechCount }}</div>
              <div class="text-gray-600">技术栈</div>
            </div>
            <div class="bg-white rounded-2xl shadow-lg p-6 text-center">
              <div class="text-3xl font-bold text-blue-600 mb-2">100%</div>
              <div class="text-gray-600">开源率</div>
            </div>
          </div>

          <!-- 分类筛选 -->
          <div class="flex flex-wrap justify-center gap-3 mb-12">
            <button
              @click="selectedCategory = 'all'"
              :class="[
                'px-6 py-3 rounded-full font-medium transition-all duration-300',
                selectedCategory === 'all'
                  ? 'bg-purple-600 text-white shadow-lg'
                  : 'bg-white text-gray-600 hover:bg-purple-50 hover:text-purple-600'
              ]"
            >
              全部项目
            </button>
            <button
              v-for="category in categories"
              :key="category"
              @click="selectedCategory = category"
              :class="[
                'px-6 py-3 rounded-full font-medium transition-all duration-300',
                selectedCategory === category
                  ? 'bg-purple-600 text-white shadow-lg'
                  : 'bg-white text-gray-600 hover:bg-purple-50 hover:text-purple-600'
              ]"
            >
              {{ category }}
            </button>
          </div>

          <!-- 项目卡片网格 -->
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
            <div
              v-for="project in filteredProjects"
              :key="project._path"
              class="group bg-white rounded-2xl shadow-lg hover:shadow-xl transition-all duration-300 overflow-hidden transform hover:-translate-y-1"
            >
              <!-- 项目封面 -->
              <div class="relative h-48 bg-gradient-to-br from-purple-400 to-pink-500 flex items-center justify-center">
                <div class="text-center text-white">
                  <div class="text-4xl mb-2">🚀</div>
                  <div class="text-lg font-semibold">{{ project.category }}</div>
                </div>
                
                <!-- 状态标签 -->
                <div class="absolute top-4 right-4">
                  <span
                    :class="[
                      'px-3 py-1 rounded-full text-sm font-medium',
                      project.status === '已上线' 
                        ? 'bg-green-100 text-green-800' 
                        : project.status === '开发中'
                        ? 'bg-yellow-100 text-yellow-800'
                        : project.status === '已完成'
                        ? 'bg-blue-100 text-blue-800'
                        : 'bg-gray-100 text-gray-800'
                    ]"
                  >
                    {{ project.status }}
                  </span>
                </div>
              </div>

              <!-- 项目内容 -->
              <div class="p-6">
                <div class="flex items-center justify-between mb-3">
                  <h3 class="text-xl font-bold text-gray-800 group-hover:text-purple-600 transition-colors">
                    {{ project.title }}
                  </h3>
                  <span class="text-sm text-gray-500">{{ formatDate(project.date) }}</span>
                </div>
                
                <p class="text-gray-600 leading-relaxed mb-4">{{ project.description }}</p>
                
                <!-- 技术栈 -->
                <div class="flex flex-wrap gap-2 mb-6">
                  <span
                    v-for="tech in project.tech"
                    :key="tech"
                    class="px-3 py-1 bg-purple-100 text-purple-800 text-sm rounded-full font-medium"
                  >
                    {{ tech }}
                  </span>
                </div>
                
                <!-- 操作按钮 -->
                <div class="flex gap-3">
                  <NuxtLink
                    :to="project._path.replace('/projects/', '/projects/')"
                    class="flex-1 bg-purple-600 text-white px-4 py-3 rounded-xl hover:bg-purple-700 transition-colors text-center font-medium inline-flex items-center justify-center gap-2"
                  >
                    <i class="fas fa-book-open"></i>
                    查看详情
                  </NuxtLink>
                  <a
                    v-if="project.demo_link"
                    :href="project.demo_link"
                    target="_blank"
                    class="flex-1 border-2 border-purple-600 text-purple-600 px-4 py-3 rounded-xl hover:bg-purple-50 transition-colors text-center font-medium"
                  >
                    在线体验
                  </a>
                  <a
                    v-if="project.source_link"
                    :href="project.source_link"
                    target="_blank"
                    class="px-4 py-3 border-2 border-gray-300 text-gray-600 rounded-xl hover:bg-gray-50 transition-colors font-medium"
                    title="查看源码"
                  >
                    <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M12.316 3.051a1 1 0 01.633 1.265l-4 12a1 1 0 11-1.898-.632l4-12a1 1 0 011.265-.633zM5.707 6.293a1 1 0 010 1.414L3.414 10l2.293 2.293a1 1 0 11-1.414 1.414l-3-3a1 1 0 010-1.414l3-3a1 1 0 011.414 0zm8.586 0a1 1 0 011.414 0l3 3a1 1 0 010 1.414l-3 3a1 1 0 11-1.414-1.414L16.586 10l-2.293-2.293a1 1 0 010-1.414z" clip-rule="evenodd"></path>
                    </svg>
                  </a>
                </div>
              </div>
            </div>
          </div>

          <!-- 底部CTA -->
          <div class="mt-20 text-center">
            <div class="bg-gradient-to-r from-purple-500 to-pink-500 rounded-2xl p-8 text-white">
              <h3 class="text-2xl font-bold mb-4">有项目合作想法？</h3>
              <p class="text-purple-100 mb-6">我们可以一起讨论技术方案，共同创造有价值的产品</p>
              <div class="flex flex-col sm:flex-row gap-4 justify-center">
                <a
                  href="mailto:contact@溪午听风.com"
                  class="inline-flex items-center px-8 py-3 bg-white text-purple-600 rounded-xl font-semibold hover:bg-gray-50 transition-colors"
                >
                  <span class="mr-2">📧</span>
                  项目合作
                </a>
                <NuxtLink
                  to="/about"
                  class="inline-flex items-center px-8 py-3 border-2 border-white text-white rounded-xl font-semibold hover:bg-white hover:text-purple-600 transition-colors"
                >
                  <span class="mr-2">👋</span>
                  了解更多
                </NuxtLink>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

// 使用 @nuxt/content 查询项目数据
const { data: projects, pending, error } = await useAsyncData('projects', () =>
  queryContent('/projects').sort({ date: -1 }).find()
)

// 调试信息
if (process.client) {
  console.log('项目数据:', projects.value)
  console.log('加载状态:', pending.value)
  console.log('错误信息:', error.value)
}

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

const onlineProjects = computed(() => {
  if (!projects.value) return 0
  return projects.value.filter(p => p.status === '已上线').length
})

const uniqueTechCount = computed(() => {
  if (!projects.value) return 0
  const allTech = projects.value.flatMap(p => p.tech || [])
  return new Set(allTech).size
})

// 格式化日期
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// 设置页面标题和SEO
useHead({
  title: '项目展示 - 溪午听风个人作品集',
  meta: [
    { name: 'description', content: '精选个人项目作品与技术实践，从创意到实现的完整展示' },
    { name: 'keywords', content: '项目展示, 作品集, 技术实践, 开源项目, 全栈开发' }
  ]
})
</script>

<style scoped>
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

section {
  animation: fadeInUp 0.8s ease-out;
}

section:nth-child(2) {
  animation-delay: 0.2s;
}

/* 按钮点击效果 */
button:active {
  transform: scale(0.98);
}
</style> 