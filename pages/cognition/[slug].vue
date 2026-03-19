<template>
  <ModuleGuard module-key="cognition">
    <div class="cognition-page">
      <!-- 全局背景噪点 -->
      <div class="cognition-background-noise"></div>

      <!-- 动态背景光斑 -->
      <div class="cognition-background-container">
        <div class="cognition-background-blob cognition-background-blob--blue"></div>
        <div class="cognition-background-blob cognition-background-blob--purple"></div>
        <div class="cognition-background-blob cognition-background-blob--emerald"></div>
      </div>

      <div class="cognition-content">
        <!-- 头部 -->
        <header class="cognition-header">
          <div class="cognition-header-left">
            <div class="cognition-header-icon">🧠</div>
            <h1 class="cognition-title">认知说明书</h1>
          </div>
          <NuxtLink to="/cognition" class="cognition-back-link">
            <i class="fas fa-arrow-left"></i>
            返回列表
          </NuxtLink>
        </header>

        <!-- 加载状态 -->
        <div v-if="loading" class="cognition-loading">
          <div class="cognition-loading-spinner"></div>
          <p class="cognition-loading-text">加载中...</p>
        </div>

        <!-- 404 -->
        <div v-else-if="!doc" class="cognition-empty">
          <div class="cognition-empty-icon">❌</div>
          <h3 class="cognition-empty-title">内容不存在</h3>
          <p class="cognition-empty-text">该认知说明书不存在或未发布</p>
          <NuxtLink to="/cognition" class="cognition-back-link mt-4">
            返回列表
          </NuxtLink>
        </div>

        <!-- 详情内容 -->
        <article v-else class="cognition-article">
          <header class="cognition-article-header">
            <h1 class="cognition-article-title">{{ doc.title }}</h1>
            <div v-if="doc.summary" class="cognition-article-summary">
              {{ doc.summary }}
            </div>
            <div class="cognition-article-meta">
              <span class="cognition-article-date">
                <i class="fas fa-calendar mr-2"></i>
                {{ formatDate(doc.updatedAt) }}
              </span>
            </div>
          </header>

          <div class="cognition-prose" v-html="renderedContent"></div>
        </article>
      </div>
    </div>
  </ModuleGuard>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

const route = useRoute()
const api = useApi()
const { parse } = useMarkdown()

const slug = route.params.slug as string
const doc = ref<any>(null)
const loading = ref(true)
const renderedContent = ref<string>('')

// 获取详情
const fetchDoc = async () => {
  loading.value = true
  try {
    const res = await api.get(`/CognitionDocs/by-slug/${slug}`)
    if (res) {
      doc.value = {
        id: res.Id || res.id,
        title: res.Title || res.title,
        slug: res.Slug || res.slug,
        summary: res.Summary || res.summary,
        contentMd: res.ContentMd || res.contentMd,
        updatedAt: res.UpdatedAt || res.updatedAt
      }

      // 解析 Markdown 为 HTML
      if (doc.value.contentMd) {
        renderedContent.value = parse(doc.value.contentMd)
      }
    } else {
      doc.value = null
    }
  } catch (e) {
    console.error('Failed to fetch cognition doc:', e)
    doc.value = null
  } finally {
    loading.value = false
  }
}

// 格式化日期
const formatDate = (dateString?: string | Date) => {
  if (!dateString) return ''
  const date = typeof dateString === 'string' ? new Date(dateString) : dateString
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// SEO
useHead({
  title: computed(() => `${doc.value?.title || '认知说明书'} - 溪午听风`),
  meta: [
    {
      name: 'description',
      content: computed(() => doc.value?.summary || '个人认知使用说明书')
    }
  ]
})

onMounted(() => {
  fetchDoc()
})
</script>

<style scoped>
/* 样式已移至 assets/css/cognition.css */
</style>
