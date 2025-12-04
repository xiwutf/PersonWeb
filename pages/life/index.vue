<template>
  <div class="life-page">
    <!-- 全局背景噪点 -->
    <div class="life-background-noise"></div>

    <!-- 动态背景光斑 -->
    <div class="life-background-container">
      <div class="life-background-blob life-background-blob--amber"></div>
      <div class="life-background-blob life-background-blob--orange"></div>
    </div>

    <div class="life-content">
      <!-- 页面头部 -->
      <header class="life-header">
        <div class="life-header-icon">
          <span>☕</span>
        </div>
        <h1 class="life-title">生活随笔</h1>
        <p class="life-subtitle">
          "记录生活点滴，分享思考感悟，在代码之外寻找诗与远方"
        </p>
      </header>

      <!-- 文章列表 -->
      <div v-if="!posts || posts.length === 0" class="life-empty">
        <div class="life-empty-icon">🍃</div>
        <h3 class="life-empty-title">暂无随笔</h3>
        <p class="life-empty-text">博主正在酝酿第一篇生活感悟...</p>
      </div>

      <div v-else class="life-posts">
        <TransitionGroup name="life-list">
          <article
            v-for="(post, index) in posts"
            :key="post._path"
            class="life-post"
            :style="{ transitionDelay: `${index * 100}ms` }"
          >
            <!-- 连接线 -->
            <div v-if="index !== posts.length - 1" class="life-post-connector"></div>

            <div class="life-post-container">
              <!-- 时间轴节点 (桌面端) -->
              <div class="life-post-dot"></div>

              <!-- 内容卡片 -->
              <div 
                class="life-post-card" 
                :class="index % 2 === 0 ? 'life-post-card--left' : 'life-post-card--right'"
              >
                <NuxtLink 
                  :to="post._path"
                  class="life-post-link"
                >
                  <!-- 封面图 -->
                  <div v-if="post.cover" class="life-post-cover">
                    <img :src="post.cover" :alt="post.title" />
                    <div class="life-post-cover-overlay"></div>
                    <div class="life-post-cover-date">
                      {{ formatDate(post.date) }}
                    </div>
                  </div>
                  <div v-else class="life-post-header">
                    <div class="life-post-header-date">
                      {{ formatDate(post.date) }}
                    </div>
                  </div>

                  <div class="life-post-body">
                    <div class="life-post-category">
                      <span class="life-post-category-tag">
                        {{ post.category || '随笔' }}
                      </span>
                    </div>

                    <h2 class="life-post-title">
                      {{ post.title }}
                    </h2>
                    
                    <p class="life-post-description">
                      {{ post.description }}
                    </p>

                    <div class="life-post-footer">
                      <div class="life-post-tags">
                        <span v-for="tag in post.tags" :key="tag" class="life-post-tag">#{{ tag }}</span>
                      </div>
                      <span class="life-post-read-more">阅读全文 &rarr;</span>
                    </div>
                  </div>
                </NuxtLink>
              </div>
            </div>
          </article>
        </TransitionGroup>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// 使用默认布局（包含顶部导航栏）
definePageMeta({
  layout: 'default'
})

const api = useApi()
const posts = ref<any[]>([])
const pending = ref(true)
const error = ref<string | null>(null)

// 从API获取生活随笔数据
const fetchPosts = async () => {
  try {
    pending.value = true
    error.value = null
    
    // 优先从API获取
    const res = await api.get<any[]>('/MockData/life-posts')
    if (res && res.length > 0) {
      posts.value = res.map(p => ({
        ...p,
        date: new Date(p.date)
      }))
      return
    }
    
    // 如果API没有数据，尝试从 @nuxt/content 获取
    const { data: contentPosts } = await useAsyncData('life-posts', () =>
      queryContent('/life').sort({ date: -1 }).find()
    )
    
    if (contentPosts.value && Array.isArray(contentPosts.value) && contentPosts.value.length > 0) {
      posts.value = contentPosts.value
    } else {
      posts.value = []
    }
  } catch (e: any) {
    console.error('Failed to fetch life posts:', e)
    error.value = e.message || '加载失败'
    posts.value = []
  } finally {
    pending.value = false
  }
}

onMounted(() => {
  fetchPosts()
})

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = dateString instanceof Date ? dateString : new Date(dateString)
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

useHead({
  title: '生活随笔 - 溪午听风',
  meta: [
    { name: 'description', content: '记录生活点滴，分享思考感悟' }
  ]
})
</script>

<style scoped>
/* 页面特有样式已移至 assets/css/life.css */
/* 这里只保留组件特有的样式（如果有） */
</style>
