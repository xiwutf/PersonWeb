<template>
  <div class="admin-visitors-page">
    <PageHeader
      title="实时访客概览"
      description="用统一后台视图快速判断当前网站热度、热门页面和最近访问行为。更深的趋势、来源和画像分析请查看访客分析页。"
      class="admin-visitors-header"
    >
      <template #actions>
        <n-space align="center" :size="12" wrap>
          <n-button :loading="loading" type="primary" @click="fetchData">
            刷新数据
          </n-button>
        </n-space>
      </template>
    </PageHeader>

    <n-alert type="info" :bordered="false" class="visitors-purpose-alert">
      这个页面更适合做“实时巡检”：
      看现在有没有人在访问、哪些页面最热、最近访问里有没有异常路径或异常 IP。
    </n-alert>

    <n-grid class="visitors-kpi-grid" cols="1 s:2 m:4" responsive="screen" :x-gap="16" :y-gap="16">
      <n-gi v-for="card in summaryCards" :key="card.label">
        <n-card :bordered="false" class="visitors-kpi-card" hoverable>
          <div class="visitors-kpi-head">
            <span class="visitors-kpi-label">{{ card.label }}</span>
            <n-tag v-if="card.tag" size="small" round :type="card.tagType">
              {{ card.tag }}
            </n-tag>
          </div>
          <div class="visitors-kpi-value" :class="card.tone">{{ card.value }}</div>
          <div class="visitors-kpi-caption">{{ card.caption }}</div>
        </n-card>
      </n-gi>
    </n-grid>

    <section class="visitors-main-grid">
      <n-card :bordered="false" class="visitors-panel visitors-panel--insights">
        <template #header>
          <div class="panel-heading">
            <div>
              <p class="panel-kicker">页面价值</p>
              <h2>你能在这里快速确认什么</h2>
            </div>
          </div>
        </template>

        <div class="insight-list">
          <div v-for="item in insightCards" :key="item.title" class="insight-item">
            <div class="insight-title">{{ item.title }}</div>
            <div class="insight-desc">{{ item.description }}</div>
          </div>
        </div>
      </n-card>

      <n-card :bordered="false" class="visitors-panel">
        <template #header>
          <div class="panel-heading">
            <div>
              <p class="panel-kicker">热门页面</p>
              <h2>当前最常被访问的路径</h2>
            </div>
          </div>
        </template>

        <n-spin :show="loading">
          <div v-if="topPaths.length" class="ranked-list">
            <div v-for="(item, index) in topPaths.slice(0, 6)" :key="`${item.path}-${index}`" class="ranked-item">
              <div class="ranked-index">{{ index + 1 }}</div>
              <div class="ranked-content">
                <div class="ranked-title">{{ formatPath(item.path) }}</div>
                <div class="ranked-subtitle">{{ item.path }}</div>
              </div>
              <div class="ranked-value">{{ item.count }}</div>
            </div>
          </div>
          <n-empty v-else class="panel-empty" description="暂无热门页面数据" />
        </n-spin>
      </n-card>
    </section>

    <n-card :bordered="false" class="visitors-panel visitors-panel--table">
      <template #header>
        <div class="panel-heading">
          <div>
            <p class="panel-kicker">最近访问</p>
            <h2>最新访客记录</h2>
          </div>
          <n-tag size="small" round type="default">
            共 {{ recentVisits.length }} 条
          </n-tag>
        </div>
      </template>

      <n-data-table
        :columns="columns"
        :data="recentVisits"
        :loading="loading"
        :pagination="false"
        :row-key="rowKey"
        :scroll-x="960"
      >
        <template #empty>
          <n-empty description="暂无访问记录" />
        </template>
      </n-data-table>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { computed, h, onBeforeUnmount, onMounted, ref } from 'vue'
import type { DataTableColumns } from 'naive-ui'
import { NAlert, NButton, NCard, NDataTable, NEmpty, NGi, NGrid, NSpace, NSpin, NTag } from 'naive-ui'
import PageHeader from '~/components/admin/patterns/PageHeader.vue'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

interface TopPathItem {
  path: string
  count: number
}

interface VisitItem {
  id: string | number
  timestamp: string
  ip: string
  path: string
  visitorId: string
}

interface StatsResponse {
  totalVisits?: number
  uniqueVisitors?: number
  todayVisits?: number
  yesterdayVisits?: number
  onlineCount?: number
  topPaths?: Array<{ path?: string; Path?: string; count?: number; Count?: number }>
  recentVisits?: Array<{ id?: string | number; Id?: string | number; timestamp?: string; Timestamp?: string; ip?: string; Ip?: string; path?: string; Path?: string; visitorId?: string; VisitorId?: string }>
  TotalVisits?: number
  UniqueVisitors?: number
  TodayVisits?: number
  YesterdayVisits?: number
  OnlineCount?: number
  TopPaths?: Array<{ path?: string; Path?: string; count?: number; Count?: number }>
  RecentVisits?: Array<{ id?: string | number; Id?: string | number; timestamp?: string; Timestamp?: string; ip?: string; Ip?: string; path?: string; Path?: string; visitorId?: string; VisitorId?: string }>
}

const api = useApi()
const { formatPath } = usePathDisplayName()

const loading = ref(false)
const stats = ref<Required<Pick<StatsResponse, 'totalVisits' | 'uniqueVisitors' | 'todayVisits' | 'yesterdayVisits' | 'onlineCount'>>>({
  totalVisits: 0,
  uniqueVisitors: 0,
  todayVisits: 0,
  yesterdayVisits: 0,
  onlineCount: 0
})
const topPaths = ref<TopPathItem[]>([])
const recentVisits = ref<VisitItem[]>([])

const hottestPath = computed(() => topPaths.value[0] ?? null)
const todayDelta = computed(() => stats.value.todayVisits - stats.value.yesterdayVisits)
const todayTrendTag = computed(() => {
  if (todayDelta.value > 0) return '较昨日上升'
  if (todayDelta.value < 0) return '较昨日回落'
  return '与昨日持平'
})

const summaryCards = computed(() => [
  {
    label: '今日访问量',
    value: stats.value.todayVisits,
    caption: `昨日 ${stats.value.yesterdayVisits}，用于判断今天是否更热`,
    tone: 'tone-primary',
    tag: todayTrendTag.value,
    tagType: todayDelta.value > 0 ? 'success' : todayDelta.value < 0 ? 'warning' : 'default'
  },
  {
    label: '累计独立访客',
    value: stats.value.uniqueVisitors,
    caption: '帮助区分“同一个人反复访问”还是“真的来了更多人”',
    tone: 'tone-info',
    tag: null,
    tagType: 'default' as const
  },
  {
    label: '当前在线',
    value: stats.value.onlineCount,
    caption: '近 5 分钟内仍有活跃行为的访客数',
    tone: 'tone-success',
    tag: '实时',
    tagType: 'success' as const
  },
  {
    label: '最热页面',
    value: hottestPath.value ? formatPath(hottestPath.value.path) : '暂无',
    caption: hottestPath.value ? `${hottestPath.value.count} 次访问` : '当前没有热点页面数据',
    tone: 'tone-accent',
    tag: hottestPath.value ? '重点关注' : null,
    tagType: 'info' as const
  }
])

const insightCards = computed(() => [
  {
    title: '站点现在有没有热度',
    description: `看“今日访问量”和“当前在线”即可快速判断现在是否有人在看站点。累计访问 ${stats.value.totalVisits} 次。`
  },
  {
    title: '大家主要在看什么',
    description: hottestPath.value
      ? `当前最热的是“${formatPath(hottestPath.value.path)}”，说明这个页面最近最容易被点开。`
      : '暂时还没有形成明显的热点页面。'
  },
  {
    title: '最近访问是否正常',
    description: '通过最近访问记录可以快速扫一眼后台路径、异常 IP、重复刷新等可疑行为。'
  }
])

const formatDateTime = (value: string) => {
  if (!value) return '-'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('zh-CN', {
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

const shortenVisitorId = (value: string) => {
  if (!value) return '-'
  return value.length > 12 ? `${value.slice(0, 8)}...${value.slice(-4)}` : value
}

const columns = computed<DataTableColumns<VisitItem>>(() => [
  {
    title: '访问时间',
    key: 'timestamp',
    width: 180,
    render: (row) => formatDateTime(row.timestamp)
  },
  {
    title: '访问页面',
    key: 'path',
    minWidth: 260,
    ellipsis: { tooltip: true },
    render: (row) =>
      h('div', { class: 'visit-path-cell' }, [
        h('div', { class: 'visit-path-title' }, formatPath(row.path)),
        h('div', { class: 'visit-path-subtitle' }, row.path || '/')
      ])
  },
  {
    title: 'IP 地址',
    key: 'ip',
    width: 150,
    ellipsis: { tooltip: true },
    render: (row) => h('span', { class: 'visit-mono-text' }, row.ip || '-')
  },
  {
    title: '访客标识',
    key: 'visitorId',
    width: 160,
    ellipsis: { tooltip: true },
    render: (row) => h('span', { class: 'visit-mono-text visit-mono-text--muted' }, shortenVisitorId(row.visitorId))
  }
])

const rowKey = (row: VisitItem) => row.id

const normalizeTopPath = (item: { path?: string; Path?: string; count?: number; Count?: number }): TopPathItem | null => {
  const path = item.path || item.Path || ''
  if (!path) return null
  return {
    path,
    count: item.count ?? item.Count ?? 0
  }
}

const normalizeVisit = (item: { id?: string | number; Id?: string | number; timestamp?: string; Timestamp?: string; ip?: string; Ip?: string; path?: string; Path?: string; visitorId?: string; VisitorId?: string }, index: number): VisitItem => ({
  id: item.id ?? item.Id ?? `${item.visitorId ?? item.VisitorId ?? 'visitor'}-${index}`,
  timestamp: item.timestamp ?? item.Timestamp ?? '',
  ip: item.ip ?? item.Ip ?? '-',
  path: item.path ?? item.Path ?? '/',
  visitorId: item.visitorId ?? item.VisitorId ?? '-'
})

const fetchData = async () => {
  loading.value = true

  try {
    const res = await api.get<StatsResponse>('/Stats')

    stats.value = {
      totalVisits: res.TotalVisits ?? res.totalVisits ?? 0,
      uniqueVisitors: res.UniqueVisitors ?? res.uniqueVisitors ?? 0,
      todayVisits: res.TodayVisits ?? res.todayVisits ?? 0,
      yesterdayVisits: res.YesterdayVisits ?? res.yesterdayVisits ?? 0,
      onlineCount: res.OnlineCount ?? res.onlineCount ?? 0
    }

    const rawTopPaths = res.TopPaths ?? res.topPaths ?? []
    topPaths.value = rawTopPaths.map(normalizeTopPath).filter((item): item is TopPathItem => item !== null)

    const rawRecentVisits = res.RecentVisits ?? res.recentVisits ?? []
    recentVisits.value = rawRecentVisits.map(normalizeVisit).slice(0, 12)
  } catch (error) {
    console.error('Failed to fetch visitor stats', error)
    topPaths.value = []
    recentVisits.value = []
  } finally {
    loading.value = false
  }
}

let refreshTimer: ReturnType<typeof setInterval> | null = null

onMounted(() => {
  fetchData()
  refreshTimer = setInterval(() => {
    fetchData()
  }, 60000)
})

onBeforeUnmount(() => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
    refreshTimer = null
  }
})
</script>

<style scoped>
.admin-visitors-page {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  padding-bottom: var(--spacing-2xl);
}

.admin-visitors-header {
  padding-bottom: 0;
}

.visitors-purpose-alert,
.visitors-kpi-card,
.visitors-panel {
  overflow: hidden;
  border: 1px solid var(--color-border-subtle);
  background: linear-gradient(180deg, var(--color-bg-card), var(--color-bg-elevated));
}

.visitors-purpose-alert {
  border-radius: var(--radius-xl);
}

.visitors-kpi-grid {
  margin-top: 4px;
}

.visitors-kpi-card {
  min-height: 164px;
}

.visitors-kpi-head,
.panel-heading {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--spacing-md);
}

.visitors-kpi-label,
.panel-kicker {
  color: var(--color-text-muted);
  font-size: var(--font-size-xs);
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.visitors-kpi-value {
  margin: var(--spacing-lg) 0 var(--spacing-sm);
  color: var(--color-text-main);
  font-size: clamp(2rem, 3vw, 2.5rem);
  line-height: 1.08;
  font-weight: 800;
}

.visitors-kpi-value.tone-primary {
  color: var(--color-primary);
}

.visitors-kpi-value.tone-info {
  color: var(--color-info, #38bdf8);
}

.visitors-kpi-value.tone-success {
  color: var(--color-success, #22c55e);
}

.visitors-kpi-value.tone-accent {
  font-size: clamp(1.15rem, 2.2vw, 1.5rem);
  color: var(--color-text-main);
}

.visitors-kpi-caption {
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
  line-height: 1.6;
}

.visitors-main-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.05fr) minmax(320px, 0.95fr);
  gap: var(--spacing-lg);
}

.panel-heading h2 {
  margin: 4px 0 0;
  color: var(--color-text-main);
  font-size: var(--font-size-h3);
  line-height: 1.25;
}

.insight-list,
.ranked-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.insight-item,
.ranked-item {
  display: flex;
  gap: var(--spacing-md);
  padding: 14px 16px;
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  background: var(--color-bg-card);
}

.insight-item {
  flex-direction: column;
}

.insight-title,
.ranked-title,
.visit-path-title {
  color: var(--color-text-main);
  font-size: var(--font-size-sm);
  font-weight: 600;
}

.insight-desc,
.ranked-subtitle,
.visit-path-subtitle {
  color: var(--color-text-muted);
  font-size: var(--font-size-xs);
  line-height: 1.6;
}

.ranked-index {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  border-radius: 999px;
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  font-size: var(--font-size-sm);
  font-weight: 700;
  flex-shrink: 0;
}

.ranked-content {
  display: flex;
  min-width: 0;
  flex: 1;
  flex-direction: column;
  gap: 4px;
}

.ranked-value {
  color: var(--color-text-main);
  font-size: 1.125rem;
  font-weight: 700;
  flex-shrink: 0;
}

.panel-empty {
  min-height: 220px;
}

.visitors-panel--table :deep(.n-data-table-th) {
  font-size: var(--font-size-xs);
  letter-spacing: 0.06em;
  text-transform: uppercase;
}

.visitors-panel--table :deep(.n-data-table-td) {
  vertical-align: top;
}

.visit-path-cell {
  display: flex;
  min-width: 0;
  flex-direction: column;
  gap: 4px;
}

.visit-mono-text {
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace;
}

.visit-mono-text--muted {
  color: var(--color-text-muted);
}

@media (max-width: 1200px) {
  .visitors-main-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .panel-heading {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
