<template>
  <section id="content" class="bento-grid-v3">
    <!-- 装饰性背景光效 -->
    <div class="bento-grid-v3-bg">
      <div class="bento-grid-v3-glow glow-1"></div>
      <div class="bento-grid-v3-glow glow-2"></div>
    </div>

    <div class="bento-grid-v3-container">
      <div class="bento-grid-v3-header" ref="sectionTitleRef">
        <h2 class="bento-grid-v3-title">
          <span class="bento-grid-v3-title-main">探索我的世界</span>
        </h2>
        <p class="bento-grid-v3-subtitle">
          这里有代码、有思考，也有生活
        </p>
      </div>

      <div class="bento-grid-v3-grid" ref="gridRef">
        <!-- 1. 最新博客 - 占据主要视野 -->
        <div class="bento-grid-v3-card bento-grid-v3-card-large bento-grid-v3-card-blog" ref="blogCardRef">
          <div class="bento-grid-v3-card-inner">
            <div class="bento-grid-v3-card-header">
              <div class="bento-grid-v3-card-badge bento-grid-v3-card-badge-blue">
                <i class="fas fa-pen-fancy"></i>
                <span>最新博客</span>
              </div>
              <h3 class="bento-grid-v3-card-title">技术探索与分享</h3>
              <p class="bento-grid-v3-card-description">
                记录学习过程中的洞察，与解决问题的完整思路。
              </p>
            </div>

            <div class="bento-grid-v3-card-content">
              <div v-if="latestPosts && latestPosts.length > 0">
                <NuxtLink
                  v-for="(post, index) in latestPosts"
                  :key="post.id"
                  :to="`/blog/${post.slug || post.id}`"
                  class="bento-grid-v3-post-item"
                >
                  <div class="bento-grid-v3-post-content">
                    <h4 class="bento-grid-v3-post-title">{{ post.title }}</h4>
                    <div class="bento-grid-v3-post-meta">
                      <span>{{ formatDate(post.publishTime || post.createdAt) }}</span>
                      <span class="bento-grid-v3-post-separator">·</span>
                      <span>{{ post.categoryName || '未分类' }}</span>
                    </div>
                  </div>
                  <div class="bento-grid-v3-post-arrow">
                    <i class="fas fa-arrow-right"></i>
                  </div>
                </NuxtLink>
              </div>
              <div v-else class="bento-grid-v3-empty">
                暂无最新文章，敬请期待...
              </div>
            </div>
          </div>
        </div>

        <!-- 2. 效率工具 -->
        <NuxtLink to="/tools" class="bento-grid-v3-card bento-grid-v3-card-tools" ref="toolsCardRef">
          <div class="bento-grid-v3-card-inner">
            <div class="bento-grid-v3-card-icon bento-grid-v3-card-icon-cyan">
              <i class="fas fa-tools"></i>
            </div>
            <div class="bento-grid-v3-card-badge bento-grid-v3-card-badge-cyan">
              <span>效率工具</span>
            </div>
            <h3 class="bento-grid-v3-card-title">效率工具</h3>
            <p class="bento-grid-v3-card-description">
              Revit 插件与自动化脚本，让日常工作更顺滑高效。
            </p>
            <div class="bento-grid-v3-card-link bento-grid-v3-card-link-cyan">
              <span>查看工具库</span>
              <i class="fas fa-arrow-right"></i>
            </div>
          </div>
        </NuxtLink>

        <!-- 3. 精选项目 -->
        <NuxtLink to="/projects" class="bento-grid-v3-card bento-grid-v3-card-projects" ref="projectCardRef">
          <div class="bento-grid-v3-card-inner">
            <div class="bento-grid-v3-card-icon bento-grid-v3-card-icon-purple">
              <i class="fas fa-project-diagram"></i>
            </div>
            <div class="bento-grid-v3-card-badge bento-grid-v3-card-badge-purple">
              <span>精选项目</span>
            </div>
            <h3 class="bento-grid-v3-card-title">精选项目</h3>
            <p class="bento-grid-v3-card-description">
              实战项目与开源尝试，持续拓展技术边界。
            </p>
            <div class="bento-grid-v3-card-link bento-grid-v3-card-link-purple">
              <span>浏览作品集</span>
              <i class="fas fa-arrow-right"></i>
            </div>
          </div>
        </NuxtLink>

        <!-- 4. AI 实验室 -->
        <NuxtLink to="/lab" class="bento-grid-v3-card bento-grid-v3-card-lab" ref="labCardRef">
          <div class="bento-grid-v3-card-inner">
            <div class="bento-grid-v3-card-icon bento-grid-v3-card-icon-pink">
              <i class="fas fa-flask"></i>
            </div>
            <div class="bento-grid-v3-card-badge bento-grid-v3-card-badge-pink">
              <span>AI 实验室</span>
            </div>
            <h3 class="bento-grid-v3-card-title">AI 实验室</h3>
            <p class="bento-grid-v3-card-description">
              3D 场景、AI 小实验与互动体验的集合地。
            </p>
            <div class="bento-grid-v3-card-link bento-grid-v3-card-link-pink">
              <span>进入实验室</span>
              <i class="fas fa-arrow-right"></i>
            </div>
          </div>
        </NuxtLink>

        <!-- 5. 生活随笔 -->
        <NuxtLink to="/life" class="bento-grid-v3-card bento-grid-v3-card-life" ref="lifeCardRef">
          <div class="bento-grid-v3-card-inner">
            <div class="bento-grid-v3-card-icon bento-grid-v3-card-icon-orange">
              <i class="fas fa-coffee"></i>
            </div>
            <div class="bento-grid-v3-card-badge bento-grid-v3-card-badge-orange">
              <span>生活随笔</span>
            </div>
            <h3 class="bento-grid-v3-card-title">生活随笔</h3>
            <p class="bento-grid-v3-card-description">
              记录代码之外的风景，用文字和图片保存一些温度。
            </p>
            <div class="bento-grid-v3-card-link bento-grid-v3-card-link-orange">
              <span>进入生活专栏</span>
              <i class="fas fa-arrow-right"></i>
            </div>
          </div>
        </NuxtLink>
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
const lifeCardRef = ref<HTMLElement | null>(null)

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
  return date.toLocaleDateString('zh-CN', { month: '2-digit', day: '2-digit' })
}

onMounted(() => {
  fetchLatestPosts()
  
  if (sectionTitleRef.value) {
    inView(sectionTitleRef.value, () => {
      animate(
        sectionTitleRef.value!,
        { opacity: [0, 1], y: [20, 0] },
        { duration: 0.8, easing: 'ease-out' }
      )
    })
  }
  
  const cards = [
    blogCardRef.value, 
    toolsCardRef.value, 
    projectCardRef.value, 
    labCardRef.value,
    lifeCardRef.value
  ].filter(Boolean) as HTMLElement[]
  
  if (gridRef.value && cards.length > 0) {
    inView(gridRef.value, () => {
      animate(
        cards,
        { opacity: [0, 1], y: [30, 0], scale: [0.98, 1] },
        { duration: 0.6, delay: stagger(0.1), easing: 'ease-out' }
      )
    })
  }
})
</script>

<style scoped>
/* 样式已移至 assets/css/home.css */
/* 保留组件特有的样式（如果有） */
</style>
