<template>
  <div class="reading-inbox-page">
    <div class="page-header">
      <h1 class="page-title">阅读 Inbox</h1>
      <p class="page-desc">MindTrace 主动发来的摘录默认私有。发布后才会成为 public；本页不会自动公开。</p>
    </div>

    <div class="toolbar">
      <n-select
        v-model:value="statusFilter"
        placeholder="全部状态"
        clearable
        class="toolbar-select"
        :options="statusOptions"
      />
      <n-button quaternary @click="fetchItems">刷新</n-button>
    </div>

    <n-empty v-if="!loading && visibleItems.length === 0" description="还没有收到摘录" />

    <div v-else class="entry-list">
      <article v-for="item in visibleItems" :key="item.id" class="entry-card">
        <blockquote v-if="item.quote" class="entry-quote">{{ item.quote }}</blockquote>
        <p v-if="item.note" class="entry-note">{{ item.note }}</p>
        <p class="entry-meta">
          <span>{{ item.sourceTitle || '未标注来源' }}</span>
          <a
            v-if="item.sourceUrl"
            :href="item.sourceUrl"
            target="_blank"
            rel="noopener noreferrer"
          >{{ item.sourceUrl }}</a>
          <span>{{ formatTime(item.sourceCreatedAt || item.importedAt) }}</span>
          <n-tag size="small" :type="statusType(item.status)">{{ statusLabel(item.status) }}</n-tag>
          <n-tag size="small">{{ item.visibility }}</n-tag>
        </p>
        <n-space>
          <n-button
            size="small"
            type="primary"
            :disabled="item.status === 'published'"
            @click="runAction(item.id, 'publish')"
          >
            发布
          </n-button>
          <n-button
            size="small"
            :disabled="item.status === 'archived'"
            @click="runAction(item.id, 'archive')"
          >
            Archive
          </n-button>
          <n-button size="small" type="error" ghost @click="removeItem(item.id)">
            删除
          </n-button>
        </n-space>
      </article>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { NButton, NEmpty, NSelect, NSpace, NTag } from 'naive-ui'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'

type ReadingEntry = {
  id: number
  sourceType: string
  externalId: string
  quote: string
  note: string
  sourceTitle: string
  sourceUrl: string
  sourceCreatedAt?: string | null
  importedAt: string
  updatedAt?: string
  tags: string[]
  status: 'inbox' | 'archived' | 'published'
  visibility: 'private' | 'public'
}

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
})

const api = useApi()
const message = useSafeMessage()
const { handleError } = useErrorHandler()
const loading = ref(false)
const items = ref<ReadingEntry[]>([])
const statusFilter = ref<string | null>(null)

const statusOptions = [
  { label: 'Inbox', value: 'inbox' },
  { label: '已发布', value: 'published' },
  { label: '已归档', value: 'archived' },
]

const visibleItems = computed(() => {
  if (!statusFilter.value) {
    return items.value
  }
  return items.value.filter(item => item.status === statusFilter.value)
})

const statusLabel = (status: string) => {
  if (status === 'published') return '已发布'
  if (status === 'archived') return '已归档'
  return 'Inbox'
}

const statusType = (status: string) => {
  if (status === 'published') return 'success'
  if (status === 'archived') return 'default'
  return 'warning'
}

const formatTime = (value?: string | null) => {
  if (!value) return ''
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  })
}

const unwrapItems = (res: unknown): ReadingEntry[] => {
  if (!res || typeof res !== 'object') return []
  const obj = res as { items?: ReadingEntry[], Items?: ReadingEntry[] }
  if (Array.isArray(obj.items)) return obj.items
  if (Array.isArray(obj.Items)) return obj.Items
  return []
}

const fetchItems = async () => {
  loading.value = true
  try {
    const data = await api.get<unknown>('/admin/reading', { silent: true })
    items.value = unwrapItems(data)
  } catch (error) {
    handleError(error, '无法加载 Inbox')
    items.value = []
  } finally {
    loading.value = false
  }
}

const runAction = async (id: number, action: 'publish' | 'archive') => {
  try {
    const data = await api.patch<{ item?: ReadingEntry, Item?: ReadingEntry }>(
      `/admin/reading/${id}`,
      { action },
    )
    const updated = data?.item || data?.Item
    if (updated) {
      items.value = items.value.map(item => (item.id === id ? updated : item))
    } else {
      await fetchItems()
    }
    message.success(action === 'publish' ? '已发布为公开摘录' : '已归档')
  } catch (error) {
    handleError(error, '操作失败')
  }
}

const removeItem = async (id: number) => {
  if (!window.confirm('确定删除这条摘录？')) {
    return
  }
  try {
    await api.delete(`/admin/reading/${id}`)
    items.value = items.value.filter(item => item.id !== id)
    message.success('已删除')
  } catch (error) {
    handleError(error, '删除失败')
  }
}

onMounted(fetchItems)
</script>

<style scoped>
.reading-inbox-page {
  width: 100%;
}

.page-header {
  margin-bottom: var(--spacing-md);
}

.page-title {
  font-size: var(--font-size-h3, 1.25rem);
  font-weight: 600;
  color: var(--color-text-main);
}

.page-desc {
  margin-top: 0.25rem;
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
}

.toolbar {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-md);
}

.toolbar-select {
  width: 140px;
}

.entry-list {
  display: grid;
  gap: var(--spacing-md);
}

.entry-card {
  padding: 0.9rem 1rem;
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  background: var(--color-bg-card);
}

.entry-quote {
  margin: 0 0 0.5rem;
  padding-left: 0.75rem;
  border-left: 3px solid var(--color-primary);
  color: var(--color-text-main);
  white-space: pre-wrap;
}

.entry-note {
  margin: 0 0 0.5rem;
  color: var(--color-text-main);
  white-space: pre-wrap;
}

.entry-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem 0.75rem;
  margin: 0 0 0.75rem;
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
}

.entry-meta a {
  color: var(--color-primary);
  word-break: break-all;
}
</style>
