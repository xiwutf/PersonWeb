<template>
  <div class="container mx-auto px-4 py-8">
    <div v-if="loading" class="text-center py-20">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <div v-else-if="article" class="flex flex-col lg:flex-row gap-8">
      <!-- 文章内容 -->
      <div class="w-full lg:w-3/4">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-8">
          <div class="mb-6">
            <h1 class="text-3xl font-bold text-gray-800 dark:text-white mb-4">{{ article.title }}</h1>
            <div class="flex items-center text-gray-500 dark:text-gray-400 text-sm">
              <span class="mr-4">📅 {{ formatDate(article.publishTime || article.createdAt) }}</span>
              <span v-if="article.category" class="mr-4">📂 {{ article.category.name }}</span>
              <!-- Tags handling if available -->
            </div>
          </div>

          <!-- 移动端 TOC (折叠) -->
          <div v-if="toc.length > 0" class="lg:hidden mb-6 bg-gray-50 dark:bg-gray-700/50 p-4 rounded-lg">
            <details>
              <summary class="font-bold text-gray-700 dark:text-gray-300 cursor-pointer">目录</summary>
              <ul class="mt-2 ml-4 list-disc text-sm text-gray-600 dark:text-gray-400">
                <li v-for="item in toc" :key="item.id" :class="`ml-${(item.level - 1) * 4}`">
                  <a :href="`#${item.id}`" @click.prevent="scrollTo(item.id)">{{ item.text }}</a>
                </li>
              </ul>
            </details>
          </div>

          <div class="prose dark:prose-invert max-w-none" v-html="renderedContent"></div>
        </div>
      </div>

      <!-- 侧边栏 (桌面端 TOC) -->
      <div class="hidden lg:block w-1/4">
        <div class="sticky top-24">
          <div v-if="toc.length > 0" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
            <h3 class="font-bold text-gray-800 dark:text-white mb-4 text-lg">目录</h3>
            <nav>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-400">
                <li v-for="item in toc" :key="item.id" :class="`pl-${(item.level - 1) * 2}`">
                  <a :href="`#${item.id}`" @click.prevent="scrollTo(item.id)" class="hover:text-blue-600 dark:hover:text-blue-400 block truncate transition-colors" :title="item.text">{{ item.text }}</a>
                </li>
              </ul>
            </nav>
          </div>
          
          <div class="mt-6 text-center">
             <NuxtLink to="/blog" class="text-blue-600 hover:text-blue-800 text-sm">
              &larr; 返回博客列表
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-10">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white mb-4">文章未找到</h1>
      <p class="text-gray-600 dark:text-gray-400 mb-6">抱歉，您访问的文章不存在或已被删除。</p>
      <NuxtLink to="/blog" class="text-blue-600 hover:text-blue-800">返回博客列表</NuxtLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import MarkdownIt from 'markdown-it'

const route = useRoute()
const api = useApi()
const md = new MarkdownIt({
  html: true,
  linkify: true,
  typographer: true
})

const article = ref<any>(null)
const loading = ref(true)
const renderedContent = ref('')
const toc = ref<any[]>([])

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString()
}

const scrollTo = (id: string) => {
  const el = document.getElementById(id)
  if (el) {
    el.scrollIntoView({ behavior: 'smooth' })
  }
}

const fetchArticle = async () => {
  loading.value = true
  try {
    const slug = route.params.id as string
    // Try fetch by slug
    const res = await api.get<any>(`/Articles/slug/${slug}`)
    article.value = res
    
    // Render Markdown
    if (res.contentMd) {
      // Custom renderer to add IDs to headers for TOC
      const tokens = md.parse(res.contentMd, {})
      
      const tocList: any[] = []
      
      tokens.forEach((token, index) => {
        if (token.type === 'heading_open') {
          const level = parseInt(token.tag.slice(1))
          const contentToken = tokens[index + 1]
          if (contentToken && contentToken.type === 'inline') {
            const text = contentToken.content
            const id = 'h-' + index // Simple ID generation
            
            // Add ID to token attributes
            token.attrSet('id', id)
            
            if (level <= 3) { // Only H1-H3
              tocList.push({ id, text, level })
            }
          }
        }
      })
      
      toc.value = tocList
      renderedContent.value = md.renderer.render(tokens, md.options, {})
    }
  } catch (e) {
    console.error('Failed to fetch article', e)
    article.value = null
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchArticle()
})
</script>

<style>
/* 简单的平滑滚动 */
html {
  scroll-behavior: smooth;
}
</style>

