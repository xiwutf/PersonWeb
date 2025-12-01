<template>
  <section id="content" class="bento-grid-v3 py-32 relative">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12">
      <div class="text-center mb-20" ref="sectionTitleRef">
        <h2 class="text-4xl lg:text-5xl xl:text-6xl font-bold text-white mb-4">探索我的世界</h2>
        <p class="text-xl lg:text-2xl text-white/60">这里有代码、有思考，也有生活</p>
      </div>

      <div
        class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 auto-rows-[minmax(200px,auto)]"
        ref="gridRef"
      >
        <!-- 最新博客 - 大卡片 -->
        <div class="md:col-span-2 lg:col-span-2 row-span-2 bento-card" ref="blogCardRef">
          <div class="card-glass h-full">
            <div class="card-header">
              <div class="card-badge bg-cyan-500/20 border-cyan-400/40 text-cyan-100">
                <i class="fas fa-pen-fancy mr-2"></i> 最新博客
              </div>
              <h3 class="card-title-large">技术探索与分享</h3>
              <p class="card-description">
                记录学习过程中的洞察，与解决问题的完整思路。
              </p>
            </div>

            <div class="card-content mt-8 space-y-3">
              <div v-if="latestPosts && latestPosts.length > 0">
                <NuxtLink
                  v-for="post in latestPosts"
                  :key="post.id"
                  :to="`/blog/${post.slug || post.id}`"
                  class="post-item group/item"
                >
                  <div class="flex justify-between items-start gap-2">
                    <div class="post-title">
                      {{ post.title }}
                    </div>
                    <i class="fas fa-arrow-right text-white/30 group-hover/item:text-cyan-400 transform group-hover/item:translate-x-1 transition-all"></i>
                  </div>
                  <div class="post-meta">
                    {{ formatDate(post.publishTime || post.createdAt) }} ·
                    {{ post.categoryName || '未分类' }}
                  </div>
                </NuxtLink>
              </div>
              <div v-else class="text-center text-white/40 py-4 text-sm">
                暂无最新文章，敬请期待更新。
              </div>
            </div>
          </div>
        </div>

        <!-- 效率工具 -->
        <div class="md:col-span-1 lg:col-span-1 row-span-1 bento-card" ref="toolsCardRef">
          <div class="card-glass h-full flex flex-col">
            <div class="card-badge bg-blue-500/20 border-blue-400/40 text-blue-100 mb-4">
              <i class="fas fa-tools mr-2"></i> 效率工具
            </div>
            <h3 class="card-title">效率工具</h3>
            <p class="card-description flex-grow">
              Revit 插件与自动化脚本，让日常工作更顺滑高效。
            </p>
            <NuxtLink
              to="/tools"
              class="card-link text-blue-400 hover:text-blue-300 mt-auto"
            >
              查看工具库
              <i class="fas fa-arrow-right ml-2"></i>
            </NuxtLink>
          </div>
        </div>

        <!-- 精选项目 -->
        <div class="md:col-span-1 lg:col-span-1 row-span-1 bento-card" ref="projectCardRef">
          <div class="card-glass h-full flex flex-col">
            <div class="card-badge bg-purple-500/20 border-purple-400/40 text-purple-100 mb-4">
              <i class="fas fa-project-diagram mr-2"></i> 精选项目
            </div>
            <h3 class="card-title">精选项目</h3>
            <p class="card-description flex-grow">
              实战项目与开源尝试，持续拓展技术边界。
            </p>
            <NuxtLink
              to="/projects"
              class="card-link text-purple-400 hover:text-purple-300 mt-auto"
            >
              浏览作品集
              <i class="fas fa-arrow-right ml-2"></i>
            </NuxtLink>
          </div>
        </div>

        <!-- AI 实验室 -->
        <div class="md:col-span-1 lg:col-span-1 row-span-1 bento-card" ref="labCardRef">
          <div class="card-glass h-full flex flex-col">
            <div class="card-badge bg-pink-500/20 border-pink-400/40 text-pink-100 mb-4">
              <i class="fas fa-flask mr-2"></i> AI 实验室
            </div>
            <h3 class="card-title">AI 实验室</h3>
            <p class="card-description flex-grow">
              3D 场景、AI 小实验与互动体验的集合地。
            </p>
            <NuxtLink
              to="/lab"
              class="card-link text-pink-400 hover:text-pink-300 mt-auto"
            >
              进入实验室
              <i class="fas fa-arrow-right ml-2"></i>
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { animate, stagger, inView } from '@motionone/dom'

const api = useApi()
const latestPosts = ref<any[]>([])

const sectionTitleRef = ref<HTMLElement | null>(null)
const gridRef = ref<HTMLElement | null>(null)
const blogCardRef = ref<HTMLElement | null>(null)
const toolsCardRef = ref<HTMLElement | null>(null)
const projectCardRef = ref<HTMLElement | null>(null)
const labCardRef = ref<HTMLElement | null>(null)

const fetchLatestPosts = async () => {
  try {
    const res = await api.get<any>('/Articles', {
      params: {
        page: 1,
        pageSize: 3
      }
    })
    latestPosts.value = res.list || res.List || []
  } catch (e) {
    console.error('Failed to fetch latest posts', e)
    latestPosts.value = []
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  
  if (days === 0) return '今天'
  if (days === 1) return '昨天'
  if (days < 7) return `${days}天前`
  if (days < 30) return `${Math.floor(days / 7)}周前`
  return date.toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' })
}

onMounted(() => {
  fetchLatestPosts()
  
  // 使用 Motion One 添加滚动进入动画
  if (sectionTitleRef.value) {
    inView(sectionTitleRef.value, () => {
      animate(
        sectionTitleRef.value!,
        { opacity: [0, 1], y: [30, 0] },
        { duration: 0.6, easing: 'ease-out' }
      )
    })
  }
  
  const cards = [blogCardRef.value, toolsCardRef.value, projectCardRef.value, labCardRef.value].filter(Boolean) as HTMLElement[]
  
  if (gridRef.value && cards.length > 0) {
    inView(gridRef.value, () => {
      animate(
        cards,
        { opacity: [0, 1], y: [50, 0], scale: [0.95, 1] },
        { duration: 0.6, delay: stagger(0.1), easing: 'ease-out' }
      )
    })
  }
})
</script>

<style scoped>
.bento-grid-v3 {
  background: linear-gradient(to bottom, #0a0a0a, #000000);
}

.bento-card {
  opacity: 0;
}

.card-glass {
  background: rgba(20, 20, 30, 0.6);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1.5rem;
  padding: 2rem;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}

.bento-card:hover .card-glass {
  transform: translateY(-4px);
  border-color: rgba(255, 255, 255, 0.2);
  box-shadow: 0 20px 60px rgba(0, 217, 255, 0.15);
}

.card-badge {
  display: inline-flex;
  align-items: center;
  padding: 0.5rem 1rem;
  border-radius: 9999px;
  font-size: 0.875rem;
  font-weight: 500;
  margin-bottom: 1rem;
}

.card-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: white;
  margin-bottom: 0.75rem;
}

.card-title-large {
  font-size: 2rem;
  font-weight: 700;
  color: white;
  margin-bottom: 0.75rem;
}

.card-description {
  color: rgba(255, 255, 255, 0.7);
  line-height: 1.6;
  font-size: 0.9375rem;
}

.post-item {
  display: block;
  padding: 1rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.75rem;
  transition: all 0.3s ease;
  margin-bottom: 0.5rem;
}

.post-item:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(0, 217, 255, 0.3);
  transform: translateX(4px);
}

.post-title {
  font-weight: 600;
  color: white;
  font-size: 0.9375rem;
  transition: color 0.3s ease;
}

.post-item:hover .post-title {
  color: rgba(0, 217, 255, 1);
}

.post-meta {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
  margin-top: 0.5rem;
}

.card-link {
  display: inline-flex;
  align-items: center;
  font-weight: 600;
  font-size: 0.9375rem;
  transition: all 0.3s ease;
}

.card-link:hover {
  transform: translateX(4px);
}
</style>

