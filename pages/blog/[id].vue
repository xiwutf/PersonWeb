<template>
  <div class="blog-detail-page">
    <div class="blog-shell">
      <div v-if="loading" class="blog-detail-loading">
        <div class="blog-detail-spinner"></div>
      </div>

      <div v-else-if="article" class="blog-layout">
        <!-- 文章内容 -->
        <div class="blog-detail-card">
          <header class="blog-detail-header">
            <h1 class="blog-detail-title">{{ article.title }}</h1>
            <div class="blog-detail-meta">
              <span class="blog-detail-meta-item">
                <span class="blog-detail-meta-icon">📅</span>
                {{ formatDate(article.publishTime || article.createdAt) }}
              </span>
              <span v-if="article.category" class="blog-detail-meta-item">
                <span class="blog-detail-meta-icon">📂</span>
                {{ article.category.name }}
              </span>
            </div>
          </header>

          <!-- 移动端 TOC -->
          <div v-if="toc.length > 0" class="lg:hidden blog-detail-toc-card">
            <header class="blog-detail-toc-header">
              <span class="blog-detail-toc-icon">📋</span>
              <h3 class="blog-detail-toc-title">目录</h3>
            </header>
            <nav class="blog-detail-toc-nav">
              <ul class="blog-detail-toc-list">
                <li v-for="item in toc" :key="item.id" class="blog-detail-toc-item">
                  <a
                    :href="`#${item.id}`"
                    @click.prevent="scrollTo(item.id)"
                    :class="['blog-detail-toc-link', `blog-detail-toc-link--level-${item.level}`]"
                  >
                    {{ item.text }}
                  </a>
                </li>
              </ul>
            </nav>
          </div>

          <article class="blog-detail-content prose dark:prose-invert max-w-none" v-html="renderedContent"></article>
        </div>

        <!-- 侧边栏 (桌面端 TOC) -->
        <aside class="hidden lg:block w-80">
          <div class="sticky top-24">
            <div v-if="toc.length > 0" class="blog-detail-toc-card">
              <header class="blog-detail-toc-header">
                <span class="blog-detail-toc-icon">📋</span>
                <h3 class="blog-detail-toc-title">目录</h3>
              </header>
              <nav class="blog-detail-toc-nav">
                <ul class="blog-detail-toc-list">
                  <li v-for="item in toc" :key="item.id" class="blog-detail-toc-item">
                    <a
                      :href="`#${item.id}`"
                      @click.prevent="scrollTo(item.id)"
                      :class="['blog-detail-toc-link', `blog-detail-toc-link--level-${item.level}`]"
                      :title="item.text"
                    >
                      {{ item.text }}
                    </a>
                  </li>
                </ul>
              </nav>
            </div>

            <div class="blog-detail-back-card">
              <NuxtLink to="/blog" class="blog-detail-back-link">
                ← 返回博客列表
              </NuxtLink>
            </div>
          </div>
        </aside>
      </div>

      <div v-else class="blog-detail-empty">
        <div class="blog-detail-empty-icon">📄</div>
        <h2 class="blog-detail-empty-title">文章未找到</h2>
        <p class="blog-detail-empty-text">抱歉，您访问的文章不存在或已被删除。</p>
        <NuxtLink to="/blog" class="blog-detail-empty-link">返回博客列表</NuxtLink>
      </div>
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

