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
        <!-- 返回按钮 -->
        <nav class="cognition-back-nav">
          <NuxtLink to="/cognition" class="cognition-back-link">
            <i class="fas fa-arrow-left mr-2"></i>
            返回列表
          </NuxtLink>
        </nav>

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
.cognition-page {
  @apply min-h-screen relative overflow-hidden;
  background: linear-gradient(135deg, var(--color-bg-body) 0%, var(--color-blue-200) 100%);
}

/* 背景噪点 */
.cognition-background-noise {
  @apply fixed inset-0 opacity-[0.015] pointer-events-none;
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 400 400' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E");
  z-index: 0;
}

/* 动态背景光斑 */
.cognition-background-container {
  @apply fixed inset-0 pointer-events-none overflow-hidden;
  z-index: 0;
}

.cognition-background-blob {
  @apply absolute rounded-full opacity-20 blur-3xl;
  animation: blob 20s infinite;
}

.cognition-background-blob--blue {
  @apply w-96 h-96 bg-blue-400;
  top: -10%;
  left: -10%;
  animation-delay: 0s;
}

.cognition-background-blob--purple {
  @apply w-96 h-96 bg-purple-400;
  top: 50%;
  right: -10%;
  animation-delay: 7s;
}

.cognition-background-blob--emerald {
  @apply w-96 h-96 bg-emerald-400;
  bottom: -10%;
  left: 50%;
  animation-delay: 14s;
}

@keyframes blob {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  33% {
    transform: translate(30px, -50px) scale(1.1);
  }
  66% {
    transform: translate(-20px, 20px) scale(0.9);
  }
}

/* 内容区域 */
.cognition-content {
  @apply relative z-10 max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12;
}

/* 返回导航 */
.cognition-back-nav {
  @apply mb-6;
}

.cognition-back-link {
  @apply inline-flex items-center px-4 py-2 rounded-lg;
  @apply bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm;
  @apply border border-gray-200 dark:border-gray-700;
  @apply text-gray-700 dark:text-gray-300;
  @apply hover:bg-white dark:hover:bg-gray-800;
  @apply hover:shadow-lg transition-all duration-200;
}

/* 文章头部 */
.cognition-article-header {
  @apply mb-8 pb-6 border-b border-gray-200 dark:border-gray-700;
}

.cognition-article-title {
  @apply text-3xl md:text-4xl font-bold mb-4;
  @apply text-gray-900 dark:text-gray-100;
  background: linear-gradient(135deg, var(--color-indigo-500) 0%, var(--color-purple-600) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  /* 确保文字清晰可见 */
  color: var(--color-primary-start);
}

.dark .cognition-article-title {
  color: var(--color-primary-end);
}

.cognition-article-summary {
  @apply text-lg text-gray-700 dark:text-gray-200 mb-4 leading-7;
  @apply font-medium;
}

.cognition-article-meta {
  @apply flex items-center gap-4 text-sm text-gray-500 dark:text-gray-400;
}

.cognition-article-date {
  @apply flex items-center;
}

/* 文章内容 */
.cognition-article {
  @apply bg-white/90 dark:bg-gray-900/90 backdrop-blur-sm;
  @apply rounded-2xl shadow-xl;
  @apply border border-gray-200 dark:border-gray-700;
  @apply p-8 md:p-12;
}

.cognition-prose {
  @apply prose prose-lg prose-slate dark:prose-invert max-w-none;
}

.cognition-prose :deep(h1) {
  @apply text-3xl font-bold mb-6 mt-8;
  @apply border-b border-gray-200 dark:border-gray-700 pb-4;
}

.cognition-prose :deep(h2) {
  @apply text-2xl font-bold mb-4 mt-8;
  @apply text-blue-700 dark:text-blue-300;
  font-weight: 700;
}

.cognition-prose :deep(h3) {
  @apply text-xl font-semibold mb-3 mt-6;
  @apply text-gray-800 dark:text-gray-100;
  font-weight: 600;
}

.cognition-prose :deep(p) {
  @apply mb-4 leading-7;
  @apply text-gray-800 dark:text-gray-100;
  @apply font-normal;
}

.cognition-prose :deep(ul),
.cognition-prose :deep(ol) {
  @apply mb-4 pl-6;
}

.cognition-prose :deep(li) {
  @apply mb-2;
  @apply text-gray-800 dark:text-gray-100;
}

.cognition-prose :deep(blockquote) {
  @apply border-l-4 border-blue-500 pl-4 py-2 my-4;
  @apply bg-blue-50 dark:bg-blue-900/30;
  @apply italic text-gray-800 dark:text-gray-100;
  @apply font-medium;
}

.cognition-prose :deep(table) {
  @apply w-full border-collapse mb-6;
}

.cognition-prose :deep(th),
.cognition-prose :deep(td) {
  @apply border border-gray-300 dark:border-gray-600 px-4 py-2;
}

.cognition-prose :deep(th) {
  @apply bg-gray-100 dark:bg-gray-800 font-semibold;
}

.cognition-prose :deep(code) {
  @apply bg-gray-100 dark:bg-gray-800 px-2 py-1 rounded text-sm;
  @apply font-mono text-red-600 dark:text-red-400;
}

.cognition-prose :deep(pre) {
  @apply bg-gray-900 text-gray-100 p-4 rounded-lg overflow-x-auto mb-4;
}

.cognition-prose :deep(pre code) {
  @apply bg-transparent text-gray-100 p-0;
}

.cognition-prose :deep(strong) {
  @apply font-bold text-gray-900 dark:text-gray-50;
  font-weight: 700;
}

.cognition-prose :deep(em) {
  @apply italic;
}

.cognition-prose :deep(a) {
  @apply text-blue-600 dark:text-blue-400 hover:underline;
}

/* 加载状态 */
.cognition-loading {
  @apply flex flex-col items-center justify-center py-20;
}

.cognition-loading-spinner {
  @apply w-12 h-12 border-4 border-blue-200 border-t-blue-600 rounded-full;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.cognition-loading-text {
  @apply mt-4 text-gray-500 dark:text-gray-400;
}

/* 空状态 */
.cognition-empty {
  @apply flex flex-col items-center justify-center py-20;
}

.cognition-empty-icon {
  @apply text-6xl mb-4;
}

.cognition-empty-title {
  @apply text-xl font-bold mb-2 text-gray-700 dark:text-gray-300;
}

.cognition-empty-text {
  @apply text-gray-500 dark:text-gray-400 mb-4;
}

/* 响应式 */
@media (max-width: 640px) {
  .cognition-article-title {
    @apply text-2xl;
  }

  .cognition-article {
    @apply p-6;
  }
}
</style>
