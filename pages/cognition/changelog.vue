<template>
  <ModuleGuard module-key="cognition">
    <div class="cognition-page">
      
      <div class="cognition-background-noise"></div>

        
      <div class="cognition-background-container">
        <div class="cognition-background-blob cognition-background-blob--blue"></div>
        <div class="cognition-background-blob cognition-background-blob--purple"></div>
        <div class="cognition-background-blob cognition-background-blob--emerald"></div>
      </div>

      <div class="cognition-content">
        
        <header class="cognition-header">
          <div class="cognition-header-icon">
            <span>ð</span>
          </div>
          <h1 class="cognition-title">æ´æ°æ¥å¿</h1>
          <p class="cognition-subtitle">
            ä¸ªäººè®¤ç¥ä½¿ç¨è¯´æä¹¦ççæ¬æ´æ°è®°å½
          </p>
        </header>

        <!-- å¯¼èªé¾æ¥ -->
        <nav class="cognition-nav">
          <NuxtLink to="/cognition" class="cognition-nav-link">
            <i class="fas fa-book mr-2"></i>
            è¯´æä¹?          </NuxtLink>
          <NuxtLink to="/cognition/changelog" class="cognition-nav-link cognition-nav-link--active">
            <i class="fas fa-history mr-2"></i>
            æ´æ°æ¥å¿
          </NuxtLink>
        </nav>

        <!-- ä¸»è¦åå®¹ -->
        <article v-if="doc" class="cognition-article">
          <div class="cognition-prose" v-html="renderedContent"></div>
        </article>

        <!-- å è½½ç¶æ?-->
        <div v-else class="cognition-loading">
          <div class="cognition-loading-spinner"></div>
          <p class="cognition-loading-text">å è½½ä¸?..</p>
        </div>
      </div>
    </div>
  </ModuleGuard>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

// 获取更新日志内容
const { parse } = useMarkdown()

const { data: doc } = await useAsyncData('cognition-changelog', () =>
  $fetch('/api/content/cognition/changelog')
)

const renderedContent = computed(() => parse(doc.value?.content || ''))

// 404 处理
if (!doc.value) {
  throw createError({ statusCode: 404, statusMessage: '内容不存在' })
}

// SEO
useHead({
  title: '更新日志 - 个人认知使用说明 - 溪午听风',
  meta: [
    { name: 'description', content: '个人认知使用说明书的版本更新记录' }
  ]
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
  @apply text-lg md:text-xl text-gray-600 dark:text-gray-300 mb-6;
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

/* ååºå¼?*/
@media (max-width: 640px) {
  .cognition-title {
    @apply text-3xl;
  }

  .cognition-article {
    @apply p-6;
  }

  .cognition-nav {
    @apply flex-col;
  }

  .cognition-nav-link {
    @apply w-full text-center;
  }
}
</style>
