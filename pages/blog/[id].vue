<template>
  <div class="container mx-auto px-4 py-8">
    <div v-if="loading" class="text-center py-20">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <div v-else-if="article" class="flex flex-col lg:flex-row gap-8">
      <!-- 文章内容 -->
      <div class="w-full lg:w-3/4">
        <div class="card p-8">
          <div class="mb-6">
            <h1 class="text-3xl font-bold text-gray-800 dark:text-var(--color-bg-light, white) mb-4">{{ article.title }}</h1>
            <div class="flex items-center text-gray-500 dark:text-gray-400 text-sm">
              <span class="mr-4">📅 {{ formatDate(article.publishTime || article.createdAt) }}</span>
              <span v-if="article.category" class="mr-4">📂 {{ article.category.name }}</span>
              <!-- Tags handling if available -->
            </div>
          </div>

          <!-- 移动端 TOC (折叠) -->
          <div v-if="toc.length > 0" class="lg:hidden mb-6 card p-4">
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
          <div v-if="toc.length > 0" class="card p-6">
            <h3 class="font-bold text-gray-800 dark:text-var(--color-bg-light, white) mb-4 text-lg">目录</h3>
            <nav>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-400">
                <li v-for="item in toc" :key="item.id" :class="`pl-${(item.level - 1) * 2}`">
                  <a :href="`#${item.id}`" @click.prevent="scrollTo(item.id)" class="btn-link btn-link--blue block truncate" :title="item.text">{{ item.text }}</a>
                </li>
              </ul>
            </nav>
          </div>
          
          <div class="mt-6 text-center">
             <NuxtLink to="/blog" class="btn-link btn-link--blue text-sm">
              &larr; 返回博客列表
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-10">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-var(--color-bg-light, white) mb-4">文章未找到</h1>
      <p class="text-gray-600 dark:text-gray-400 mb-6">抱歉，您访问的文章不存在或已被删除。</p>
      <NuxtLink to="/blog" class="btn-link btn-link--blue">返回博客列表</NuxtLink>
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
    const idOrSlug = route.params.id as string
    console.log('[Blog] Fetching article with id/slug:', idOrSlug)
    
    let res: any = null
    
    // 判断是数字 ID 还是 slug
    const isNumeric = /^\d+$/.test(idOrSlug)
    
    if (isNumeric) {
      // 如果是数字，直接通过 ID 获取
      console.log('[Blog] Detected numeric ID, fetching by ID')
      res = await api.get<any>(`/Articles/${idOrSlug}`)
      console.log('[Blog] Found article by ID:', res?.title)
    } else {
      // 如果是字符串，先尝试通过 slug 获取
      console.log('[Blog] Detected slug, fetching by slug')
      try {
        res = await api.get<any>(`/Articles/slug/${idOrSlug}`)
        console.log('[Blog] Found article by slug:', res?.title)
      } catch (slugError) {
        // 如果 slug 获取失败，尝试通过 ID 获取（可能是字符串格式的 ID）
        console.log('[Blog] Slug fetch failed, trying by ID as fallback')
        try {
          res = await api.get<any>(`/Articles/${idOrSlug}`)
          console.log('[Blog] Found article by ID (fallback):', res?.title)
        } catch (idError) {
          console.error('[Blog] Failed to fetch article by both slug and ID')
          throw idError
        }
      }
    }
    
    article.value = res
    
    // Render Markdown
    if (res && res.contentMd) {
      // Custom renderer to add IDs to headers for TOC
      const tokens = md.parse(res.contentMd, {})
      
      const tocList: any[] = []
      let firstH1Index = -1 // 记录第一个 H1 标题的位置
      
      // 先找到第一个 H1 标题的位置
      for (let i = 0; i < tokens.length; i++) {
        const token = tokens[i]
        if (token.type === 'heading_open') {
          const level = parseInt(token.tag.slice(1))
          if (level === 1 && firstH1Index === -1) {
            firstH1Index = i
            break // 找到第一个 H1 就退出
          }
        }
      }
      
      // 过滤掉第一个 H1 标题（heading_open, inline, heading_close）
      // 因为页面顶部已经显示了文章标题，避免重复
      const filteredTokens: any[] = []
      for (let i = 0; i < tokens.length; i++) {
        // 如果是第一个 H1 标题及其相关内容，跳过
        if (firstH1Index !== -1) {
          // heading_open
          if (i === firstH1Index) continue
          // inline (内容)
          if (i === firstH1Index + 1) continue
          // heading_close
          if (i === firstH1Index + 2 && tokens[i].type === 'heading_close') continue
        }
        filteredTokens.push(tokens[i])
      }
      
      // 为剩余的标题添加 ID 并生成目录
      filteredTokens.forEach((token: any, index: number) => {
        if (token.type === 'heading_open') {
          const level = parseInt(token.tag.slice(1))
          const contentToken = filteredTokens[index + 1]
          if (contentToken && contentToken.type === 'inline') {
            const text = contentToken.content
            const id = 'h-' + index // Simple ID generation
            
            // Add ID to token attributes
            token.attrSet('id', id)
            
            // 排除H1标题（通常是文章标题），只包含H2-H3
            if (level >= 2 && level <= 3) {
              tocList.push({ id, text, level })
            }
          }
        }
      })
      
      toc.value = tocList
      renderedContent.value = md.renderer.render(filteredTokens, md.options, {})
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

