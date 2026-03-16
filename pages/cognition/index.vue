<template>
  <ModuleGuard module-key="cognition" :show-fallback="true">
    <div class="cognition-page">
      <!-- å¨å±èæ¯åªç¹ -->
      <div class="cognition-background-noise"></div>

      <!-- å¨æèæ¯åæ?-->
      <div class="cognition-background-container">
        <div class="cognition-background-blob cognition-background-blob--blue"></div>
        <div class="cognition-background-blob cognition-background-blob--purple"></div>
        <div class="cognition-background-blob cognition-background-blob--emerald"></div>
      </div>

      <div class="cognition-content">
        <!-- é¡µé¢å¤´é¨ -->
        <header class="cognition-header">
          <div class="cognition-header-icon">
            <span>ð§ </span>
          </div>
          <h1 class="cognition-title">ä¸ªäººè®¤ç¥ä½¿ç¨è¯´æä¹?/h1>
          <p class="cognition-subtitle">
            åçç§æç»´ãæ¨¡åé©±å¨ãåæ¶æ æè®°å¿çè®¤ç¥ç³»ç»ä½¿ç¨æå
          </p>
          <div class="cognition-back-link-container">
            <NuxtLink to="/about" class="cognition-back-link">
              <i class="fas fa-arrow-left mr-2"></i>
              è¿åå³äºæ?            </NuxtLink>
          </div>
        </header>

        <!-- åè¡¨åå®¹ -->
        <div v-if="loading" class="cognition-loading">
          <div class="cognition-loading-spinner"></div>
          <p class="cognition-loading-text">å è½½ä¸?..</p>
        </div>

        <div v-else-if="!moduleEnabled" class="cognition-empty">
          <div class="cognition-empty-icon">ð</div>
          <h3 class="cognition-empty-title">æ¨¡åæªå¯ç?/h3>
          <p class="cognition-empty-text">è¯·åå¾åå°ãç³»ç»è®¾ç½?â?æ¨¡åç®¡çãå¯ç¨ãè®¤ç¥è¯´æä¹¦ãæ¨¡å?/p>
        </div>
        <div v-else-if="docs.length === 0" class="cognition-empty">
          <div class="cognition-empty-icon">ð</div>
          <h3 class="cognition-empty-title">ææ åå®¹</h3>
          <p class="cognition-empty-text">è®¤ç¥è¯´æä¹¦æ­£å¨åå¤ä¸­...</p>
        </div>

        <div v-else class="cognition-list">
          <article
            v-for="doc in docs"
            :key="doc.id"
            class="cognition-item"
            @click="goToDetail(doc.slug)"
          >
            <div class="cognition-item-header">
              <h2 class="cognition-item-title">{{ doc.title }}</h2>
              <div class="cognition-item-meta">
                <span class="cognition-item-date">{{ formatDate(doc.updatedAt) }}</span>
              </div>
            </div>
            <p v-if="doc.summary" class="cognition-item-summary">{{ doc.summary }}</p>
            <div class="cognition-item-footer">
              <span class="cognition-item-read-more">éè¯»å¨æ â?/span>
            </div>
          </article>
        </div>
      </div>
    </div>
  </ModuleGuard>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

const api = useApi()
const router = useRouter()
const { isModuleEnabled } = useModuleSystem()
const docs = ref<any[]>([])
const loading = ref(true)

// æ£æ¥æ¨¡åæ¯å¦å¯ç?const moduleEnabled = computed(() => isModuleEnabled('cognition'))

// è·åå·²åå¸çè®¤ç¥è¯´æä¹¦åè¡?const fetchDocs = async () => {
  loading.value = true
  try {
    const res = await api.get('/CognitionDocs', {
      params: {
        status: 'published',
        page: 1,
        pageSize: 100
      }
    })

    console.log('åå°è·åæ°æ®ååº:', res) // è°è¯ç?
    // å¼å®¹ PascalCase å?camelCase
    const list = res?.List || res?.list || []
    
    // è½¬æ¢å­æ®µåä¸º camelCaseï¼å¦æåç«¯è¿åçæ?PascalCaseï¼?    docs.value = list.map((item: any) => ({
      id: item.Id || item.id,
      title: item.Title || item.title,
      slug: item.Slug || item.slug,
      summary: item.Summary || item.summary,
      status: item.Status || item.status,
      updatedAt: item.UpdatedAt || item.updatedAt
    }))

    console.log('åå°å¤çåçæ°æ®:', docs.value) // è°è¯ç?  } catch (e) {
    console.error('Failed to fetch cognition docs:', e)
    docs.value = []
  } finally {
    loading.value = false
  }
}

// è·³è½¬å°è¯¦æé¡µ
const goToDetail = (slug: string) => {
  router.push(`/cognition/${slug}`)
}

// æ ¼å¼åæ¥æ?const formatDate = (dateString?: string | Date) => {
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
  title: 'ä¸ªäººè®¤ç¥ä½¿ç¨è¯´æä¹?- æºªåå¬é£',
  meta: [
    { name: 'description', content: 'åçç§æç»´ãæ¨¡åé©±å¨ãåæ¶æ æè®°å¿çè®¤ç¥ç³»ç»ä½¿ç¨æå' }
  ]
})

onMounted(() => {
  console.log('è®¤ç¥æ¨¡åæ¯å¦å¯ç¨:', moduleEnabled.value)
  if (moduleEnabled.value) {
    fetchDocs()
  } else {
    console.warn('è®¤ç¥æ¨¡åæªå¯ç¨ï¼è¯·åå¾åå°å¯ç¨')
    loading.value = false
  }
})
</script>

<style scoped>
.cognition-page {
  @apply min-h-screen relative overflow-hidden;
  background: linear-gradient(135deg, var(--color-bg-body) 0%, var(--color-blue-200) 100%);
}

/* èæ¯åªç¹ */
.cognition-background-noise {
  @apply fixed inset-0 opacity-[0.015] pointer-events-none;
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 400 400' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E");
  z-index: 0;
}

/* å¨æèæ¯åæ?*/
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

/* åå®¹åºå */
.cognition-content {
  @apply relative z-10 max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12;
}

/* é¡µé¢å¤´é¨ */
.cognition-header {
  @apply text-center mb-12;
}

.cognition-header-icon {
  @apply text-6xl mb-6;
  filter: drop-shadow(0 4px 6px var(--shadow));
}

.cognition-title {
  @apply text-4xl md:text-5xl font-bold mb-4;
  background: linear-gradient(135deg, var(--color-indigo-500) 0%, var(--color-purple-600) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.cognition-subtitle {
  @apply text-lg md:text-xl text-gray-700 dark:text-gray-200 mb-6;
  @apply font-medium;
}

.cognition-back-link-container {
  @apply mt-6;
}

.cognition-back-link {
  @apply inline-flex items-center px-4 py-2 rounded-lg;
  @apply bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm;
  @apply border border-gray-200 dark:border-gray-700;
  @apply text-gray-700 dark:text-gray-200;
  @apply hover:bg-white dark:hover:bg-gray-800;
  @apply hover:shadow-lg transition-all duration-200;
  @apply font-medium;
}

.cognition-meta {
  @apply flex items-center justify-center gap-3 text-sm text-gray-500 dark:text-gray-400;
}

.cognition-version {
  @apply px-3 py-1 bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-300 rounded-full font-mono font-semibold;
}

.cognition-divider {
  @apply text-gray-400;
}

.cognition-date {
  @apply text-gray-600 dark:text-gray-400;
}

/* å¯¼èª */
.cognition-nav {
  @apply flex items-center justify-center gap-4 mb-12;
}

.cognition-nav-link {
  @apply px-6 py-3 rounded-lg font-medium transition-all duration-200;
  @apply bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm;
  @apply border border-gray-200 dark:border-gray-700;
  @apply text-gray-700 dark:text-gray-300;
  @apply hover:bg-white dark:hover:bg-gray-800;
  @apply hover:shadow-lg hover:scale-105;
}

.cognition-nav-link--active {
  @apply bg-blue-50 dark:bg-blue-900/30;
  @apply border-blue-300 dark:border-blue-700;
  @apply text-blue-700 dark:text-blue-300;
}

/* æç« åå®¹ */
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
  @apply text-blue-600 dark:text-blue-400;
}

.cognition-prose :deep(h3) {
  @apply text-xl font-semibold mb-3 mt-6;
}

.cognition-prose :deep(p) {
  @apply mb-4 leading-7;
}

.cognition-prose :deep(ul),
.cognition-prose :deep(ol) {
  @apply mb-4 pl-6;
}

.cognition-prose :deep(li) {
  @apply mb-2;
}

.cognition-prose :deep(blockquote) {
  @apply border-l-4 border-blue-500 pl-4 py-2 my-4;
  @apply bg-blue-50 dark:bg-blue-900/20;
  @apply italic text-gray-700 dark:text-gray-300;
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
  @apply font-semibold text-gray-900 dark:text-gray-100;
}

.cognition-prose :deep(em) {
  @apply italic;
}

.cognition-prose :deep(a) {
  @apply text-blue-600 dark:text-blue-400 hover:underline;
}

/* å è½½ç¶æ?*/
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

/* åè¡¨æ ·å¼ */
.cognition-list {
  @apply space-y-6;
}

.cognition-item {
  @apply bg-white/90 dark:bg-gray-900/90 backdrop-blur-sm;
  @apply rounded-2xl shadow-xl;
  @apply border border-gray-200 dark:border-gray-700;
  @apply p-6 md:p-8;
  @apply cursor-pointer transition-all duration-200;
  @apply hover:shadow-2xl hover:scale-[1.02];
}

.cognition-item-header {
  @apply mb-4;
}

.cognition-item-title {
  @apply text-2xl font-bold mb-2;
  @apply text-gray-900 dark:text-gray-100;
  background: linear-gradient(135deg, var(--color-indigo-500) 0%, var(--color-purple-600) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  /* ç¡®ä¿æå­æ¸æ°å¯è§ */
  color: var(--color-primary-start);
}

.dark .cognition-item-title {
  color: var(--color-primary-end);
}

.cognition-item-meta {
  @apply flex items-center gap-2 text-sm text-gray-500 dark:text-gray-400;
}

.cognition-item-date {
  @apply text-gray-500 dark:text-gray-400;
}

.cognition-item-summary {
  @apply text-gray-700 dark:text-gray-200 mb-4 leading-7;
  @apply font-normal;
}

.cognition-item-footer {
  @apply flex justify-end;
}

.cognition-item-read-more {
  @apply text-blue-600 dark:text-blue-400 font-medium;
  @apply hover:underline;
}

/* ç©ºç¶æ?*/
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
  @apply text-gray-500 dark:text-gray-400;
}

/* ååºå¼?*/
@media (max-width: 640px) {
  .cognition-title {
    @apply text-3xl;
  }

  .cognition-item {
    @apply p-4;
  }
}
</style>