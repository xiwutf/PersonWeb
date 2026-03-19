<template>
  <div class="intelligence-content-detail-page">
    <PageHeader
      title="内容详情"
      description="查看情报内容原文摘要、分类标签与价值评分。"
    >
      <template #actions>
        <n-space :size="12" wrap>
          <n-button secondary @click="navigateTo('/admin/intelligence/content')">
            返回列表
          </n-button>
          <n-button v-if="content?.originalUrl" type="primary" @click="openOriginalUrl(content.originalUrl)">
            打开原文
          </n-button>
        </n-space>
      </template>
    </PageHeader>

    <n-card v-if="loading" :bordered="false" class="detail-panel">
      <div class="detail-loading">
        <n-spin size="large" />
      </div>
    </n-card>

    <template v-else-if="content">
      <section class="detail-grid">
        <n-card :bordered="false" class="detail-panel detail-panel--main">
          <div class="detail-heading">
            <n-space :size="8" wrap>
              <n-tag :bordered="false" round type="info">{{ content.category || '未分类' }}</n-tag>
              <n-tag
                :bordered="false"
                round
                :type="content.relevanceScore >= 70 ? 'warning' : 'default'"
              >
                相关度 {{ content.relevanceScore ?? 0 }}
              </n-tag>
            </n-space>

            <h2>{{ content.title }}</h2>

            <div class="detail-meta">
              <span>来源：{{ content.sourceName || '-' }}</span>
              <span>创建时间：{{ formatTime(content.createdAt) }}</span>
              <span v-if="content.publishTime">发布时间：{{ formatTime(content.publishTime) }}</span>
            </div>
          </div>

          <div class="detail-section">
            <h3>摘要</h3>
            <p>{{ content.summary || '暂无摘要' }}</p>
          </div>

          <div class="detail-section">
            <h3>标签</h3>
            <n-space v-if="content.tags?.length" :size="8" wrap>
              <n-tag v-for="tag in content.tags" :key="tag" size="small" :bordered="false" round>
                {{ tag }}
              </n-tag>
            </n-space>
            <p v-else>暂无标签</p>
          </div>
        </n-card>

        <n-card :bordered="false" class="detail-panel">
          <template #header>
            <div class="panel-title">
              <h3>价值信息</h3>
            </div>
          </template>

          <div class="metric-list">
            <div class="metric-item">
              <span>学习价值</span>
              <strong>{{ content.learningValue || '-' }}</strong>
            </div>
            <div class="metric-item">
              <span>业务价值</span>
              <strong>{{ content.businessValue || '-' }}</strong>
            </div>
          </div>
        </n-card>
      </section>
    </template>

    <n-card v-else :bordered="false" class="detail-panel">
      <n-empty description="未找到对应内容" />
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import PageHeader from '~/components/admin/patterns/PageHeader.vue'
import { useIntelligenceApi } from '~/composables/useIntelligenceApi'
import type { ContentItemDto } from '~/types/intelligence'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const route = useRoute()
const api = useIntelligenceApi()
const loading = ref(false)
const content = ref<ContentItemDto | null>(null)

const formatTime = (value?: string) => {
  if (!value) return '-'

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return '-'
  return date.toLocaleString('zh-CN', { hour12: false })
}

const openOriginalUrl = (url: string) => {
  if (!url) return
  window.open(url, '_blank', 'noopener,noreferrer')
}

const fetchContentDetail = async () => {
  const id = Number(route.params.id)
  if (!id) {
    content.value = null
    return
  }

  loading.value = true
  try {
    content.value = await api.getContentDetail(id)
  } catch (error) {
    console.error('获取内容详情失败:', error)
    content.value = null
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchContentDetail()
})
</script>

<style scoped>
.intelligence-content-detail-page {
  padding: var(--spacing-lg);
  max-width: 1480px;
  margin: 0 auto;
}

.detail-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.5fr) minmax(280px, 0.8fr);
  gap: var(--spacing-lg);
}

.detail-panel {
  background:
    linear-gradient(180deg, rgba(18, 24, 39, 0.98), rgba(15, 23, 42, 0.92)),
    var(--color-bg-card);
  border: 1px solid var(--color-border-subtle, rgba(148, 163, 184, 0.18));
}

.detail-loading {
  min-height: 280px;
  display: grid;
  place-items: center;
}

.detail-heading h2,
.panel-title h3,
.detail-section h3 {
  color: var(--color-text-main);
}

.detail-heading h2 {
  margin: 16px 0 12px;
  font-size: 1.5rem;
  line-height: 1.35;
}

.detail-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  color: var(--color-text-muted);
  font-size: 0.9rem;
}

.detail-section + .detail-section {
  margin-top: var(--spacing-xl);
}

.detail-section p {
  margin: 12px 0 0;
  line-height: 1.75;
  color: var(--color-text-muted);
}

.metric-list {
  display: grid;
  gap: 12px;
}

.metric-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px;
  border-radius: var(--radius-lg);
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(148, 163, 184, 0.12);
}

.metric-item span {
  color: var(--color-text-muted);
}

.metric-item strong {
  color: var(--color-text-main);
}

@media (max-width: 768px) {
  .intelligence-content-detail-page {
    padding: var(--spacing-md);
  }

  .detail-grid {
    grid-template-columns: 1fr;
  }
}
</style>
