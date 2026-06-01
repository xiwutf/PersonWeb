<template>
  <div class="knowledge-page">
    <div class="knowledge-background-noise" aria-hidden="true"></div>
    <div class="knowledge-background" aria-hidden="true">
      <div class="knowledge-background-blob knowledge-background-blob--violet"></div>
      <div class="knowledge-background-blob knowledge-background-blob--blue"></div>
    </div>

    <div class="knowledge-shell">
      <section class="knowledge-hero">
        <div class="knowledge-hero-badge">
          <span class="knowledge-hero-badge-dot"></span>
          Knowledge Base
        </div>
        <h1 class="knowledge-title">个人知识库</h1>
        <p class="knowledge-subtitle">记录学习、思考与成长，把零散笔记整理成可检索、可复用的知识卡片。</p>
      </section>

      <div class="knowledge-toolbar">
        <select v-model="categoryFilter" class="knowledge-select" @change="fetchList">
          <option value="">全部分类</option>
          <option value="开发笔记">开发笔记</option>
          <option value="踩坑记录">踩坑记录</option>
          <option value="想法灵感">想法灵感</option>
        </select>

        <input
          v-model="searchKeyword"
          type="search"
          placeholder="搜索关键词..."
          class="knowledge-input"
          @keyup.enter="fetchList"
        />

        <button type="button" class="knowledge-search-button" @click="fetchList">
          搜索
        </button>
      </div>

      <div v-if="loading" class="knowledge-loading">
        <div class="knowledge-loading-spinner" aria-hidden="true"></div>
        <p class="knowledge-loading-text">正在加载知识库...</p>
      </div>

      <div v-else-if="loadError" class="knowledge-error">
        <p class="knowledge-error-text">知识库暂时无法访问，请稍后再试。</p>
        <button type="button" class="knowledge-search-button" @click="fetchList">重试</button>
      </div>

      <div v-else-if="knowledgeList.length === 0" class="knowledge-empty">
        <p class="knowledge-empty-text">暂无知识库内容</p>
      </div>

      <div v-else class="knowledge-grid">
        <NuxtLink
          v-for="item in knowledgeList"
          :key="item.id"
          :to="`/knowledge/${item.id}`"
          class="knowledge-card"
        >
          <div class="knowledge-card-head">
            <span class="knowledge-badge" :class="getCategoryClass(item.category)">
              {{ item.category || '未分类' }}
            </span>
            <span class="knowledge-card-date">{{ formatDate(item.updatedAt) }}</span>
          </div>

          <h2 class="knowledge-card-title">{{ item.title }}</h2>

          <div class="knowledge-card-meta">
            <span>浏览 {{ item.viewCount }}</span>
            <div v-if="parseTags(item.tags).length" class="knowledge-tags">
              <span v-for="tag in parseTags(item.tags).slice(0, 2)" :key="tag" class="knowledge-tag">
                {{ tag }}
              </span>
            </div>
          </div>
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import '~/assets/css/knowledge.css'
import type { KnowledgeListResponse } from '~/types/knowledge'

definePageMeta({
  layout: 'default',
})

const categoryFilter = ref('')
const searchKeyword = ref('')

const { data, pending, error, refresh } = await useAsyncData(
  'knowledge-list',
  () => $fetch<KnowledgeListResponse>('/api/knowledge', {
    query: {
      category: categoryFilter.value || undefined,
      keyword: searchKeyword.value || undefined,
      page: 1,
      pageSize: 20,
    },
  }),
  {
    default: () => ({ total: 0, list: [] }),
  },
)

const knowledgeList = computed(() => data.value?.list ?? [])
const loading = computed(() => pending.value)
const loadError = computed(() => error.value)

const fetchList = () => refresh()

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

useHead({
  title: '个人知识库 - 溪午听风',
  meta: [
    {
      name: 'description',
      content: '记录学习、思考与成长，整理开发笔记、踩坑记录与想法灵感。',
    },
  ],
})
</script>
