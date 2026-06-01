<template>
  <div class="knowledge-page">
    <div class="knowledge-background-noise" aria-hidden="true"></div>
    <div class="knowledge-background" aria-hidden="true">
      <div class="knowledge-background-blob knowledge-background-blob--violet"></div>
      <div class="knowledge-background-blob knowledge-background-blob--blue"></div>
    </div>

    <div class="knowledge-shell">
      <NuxtLink to="/knowledge" class="knowledge-detail-back">← 返回知识库</NuxtLink>

      <div v-if="pending" class="knowledge-loading">
        <div class="knowledge-loading-spinner" aria-hidden="true"></div>
        <p class="knowledge-loading-text">正在加载内容...</p>
      </div>

      <div v-else-if="fetchError || !item" class="knowledge-empty">
        <p class="knowledge-empty-text">未找到这条知识库内容</p>
        <NuxtLink to="/knowledge" class="knowledge-search-button">返回列表</NuxtLink>
      </div>

      <article v-else class="knowledge-article">
        <header class="knowledge-article-header">
          <span class="knowledge-badge" :class="getCategoryClass(item.category)">
            {{ item.category || '未分类' }}
          </span>
          <h1 class="knowledge-article-title">{{ item.title }}</h1>
          <div class="knowledge-article-meta">
            <span>更新于 {{ formatDate(item.updatedAt) }}</span>
            <span>浏览 {{ item.viewCount }}</span>
          </div>
          <div v-if="parseTags(item.tags).length" class="knowledge-tags">
            <span v-for="tag in parseTags(item.tags)" :key="tag" class="knowledge-tag">
              {{ tag }}
            </span>
          </div>
        </header>

        <div class="knowledge-prose" v-html="renderedContent"></div>
      </article>
    </div>
  </div>
</template>

<script setup lang="ts">
import '~/assets/css/knowledge.css'
import type { KnowledgeItem } from '~/types/knowledge'

definePageMeta({
  layout: 'default',
})

const route = useRoute()
const id = computed(() => String(route.params.id ?? ''))
const { parse } = useMarkdown()

const { data: item, pending, error: fetchError } = await useAsyncData(
  () => `knowledge-detail-${id.value}`,
  () => $fetch<KnowledgeItem>(`/api/knowledge/${id.value}`),
  {
    watch: [id],
  },
)

const renderedContent = computed(() => {
  const content = item.value?.content
  if (!content) {
    return '<p>暂无正文内容</p>'
  }

  const looksLikeMarkdown = /(^|\n)\s{0,3}(#{1,6}\s|[-*]\s|```)/.test(content)
  if (looksLikeMarkdown) {
    return parse(content)
  }

  return content
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/\n/g, '<br>')
})

const getCategoryClass = (category: string | null) => {
  const classes: Record<string, string> = {
    开发笔记: 'knowledge-badge--dev',
    踩坑记录: 'knowledge-badge--pitfall',
    想法灵感: 'knowledge-badge--idea',
  }
  return classes[category || ''] || 'knowledge-badge--default'
}

const parseTags = (tags: string | null) => {
  if (!tags) return []
  try {
    const parsed = JSON.parse(tags)
    return Array.isArray(parsed) ? parsed : []
  } catch {
    return tags.split(',').map(tag => tag.trim()).filter(Boolean)
  }
}

const formatDate = (dateStr: string | null) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('zh-CN')
}

useHead(() => ({
  title: item.value?.title ? `${item.value.title} - 个人知识库` : '知识库详情 - 溪午听风',
}))
</script>
