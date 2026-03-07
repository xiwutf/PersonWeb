<template>
  <ClientOnly>
    <div class="intelligence-daily-report-detail-page">
      <!-- 页面标题 -->
      <div class="page-header">
        <h1 class="page-title">{{ report?.title || '日报详情' }}</h1>
        <div class="header-actions">
          <n-button @click="navigateTo('/admin/intelligence/daily-report')">
            <template #icon>
              <i class="fas fa-arrow-left"></i>
            </template>
            返回列表
          </n-button>
          <n-button v-if="report?.contentMarkdown" type="primary" @click="copyContent">
            <template #icon>
              <i class="fas fa-copy"></i>
            </template>
            复制内容
          </n-button>
        </div>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <n-spin size="large" />
      </div>

      <!-- 空状态 -->
      <div v-else-if="!report" class="empty-container">
        <i class="fas fa-file-alt"></i>
        <p>日报不存在</p>
      </div>

      <!-- 日报内容 -->
      <div v-else class="report-content">
        <!-- 元信息 -->
        <div class="report-meta-bar">
          <span><i class="fas fa-calendar"></i> {{ formatDate(report.reportDate) }}</span>
          <span><i class="fas fa-list"></i> {{ report.itemCount }} 条内容</span>
          <span><i class="fas fa-clock"></i> {{ formatDateTime(report.generatedAt) }}</span>
        </div>

        <!-- Markdown 内容 -->
        <div v-if="report.contentMarkdown" class="markdown-content">
          <div v-html="renderedContent"></div>
        </div>

        <div v-else class="empty-content">
          <i class="fas fa-file-exclamation"></i>
          <p>暂无报告内容</p>
        </div>
      </div>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { useIntelligenceApi } from '~/composables/useIntelligenceApi'
import { useNotification } from '~/composables/useToast'
import { useMarkdown } from '~/composables/useMarkdown'
import type { ReportResponseDto } from '~/types/intelligence'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const route = useRoute()
const api = useIntelligenceApi()
const { parse } = useMarkdown()
const notification = useNotification()

// 状态
const loading = ref(false)
const report = ref<ReportResponseDto | null>(null)

// 渲染后的 Markdown 内容
const renderedContent = computed(() => {
  if (!report.value?.contentMarkdown) return ''
  return parse(report.value.contentMarkdown)
})

// 获取日报详情
const fetchReport = async () => {
  loading.value = true
  try {
    const id = Number(route.params.id)
    report.value = await api.getReportDetail(id)
  } catch (error) {
    console.error('获取日报详情失败:', error)
  } finally {
    loading.value = false
  }
}

// 复制内容
const copyContent = async () => {
  if (!report.value?.contentMarkdown) return
  try {
    await navigator.clipboard.writeText(report.value.contentMarkdown)
    notification.success('内容已复制到剪贴板')
  } catch (error) {
    console.error('复制失败:', error)
    notification.error('复制失败')
  }
}

// 格式化日期
const formatDate = (date: string) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}年${d.getMonth() + 1}月${d.getDate()}日`
}

// 格式化日期时间
const formatDateTime = (datetime?: string) => {
  if (!datetime) return ''
  const d = new Date(datetime)
  return d.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 初始化
onMounted(() => {
  fetchReport()
})
</script>

<style scoped>
.intelligence-daily-report-detail-page {
  padding: 20px;
  max-width: 900px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  margin: 0;
  color: var(--color-text-main);
}

.header-actions {
  display: flex;
  gap: 12px;
}

.loading-container,
.empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 400px;
  color: var(--color-text-sub);
}

.empty-container i {
  font-size: 48px;
  margin-bottom: 12px;
  opacity: 0.5;
}

/* 日报内容 */
.report-content {
  background: var(--color-bg-card);
  border-radius: 12px;
  padding: 24px;
    box-shadow: var(--shadow-card);
}

/* 元信息栏 */
.report-meta-bar {
  display: flex;
  gap: 20px;
  padding-bottom: 16px;
  margin-bottom: 24px;
  border-bottom: 1px solid var(--color-border);
  font-size: 14px;
  color: var(--color-text-sec);
}

.report-meta-bar i {
  margin-right: 4px;
}

/* Markdown 内容 */
.markdown-content {
  line-height: 1.8;
  color: var(--color-text-main);
}

.markdown-content :deep(h1),
.markdown-content :deep(h2),
.markdown-content :deep(h3),
.markdown-content :deep(h4) {
  margin-top: 24px;
  margin-bottom: 12px;
  font-weight: 600;
}

.markdown-content :deep(h1) {
  font-size: 24px;
  border-bottom: var(--spacing-md) solid var(--color-border);
  padding-bottom: 8px;
}

.markdown-content :deep(h2) {
  font-size: 20px;
}

.markdown-content :deep(h3) {
  font-size: 18px;
}

.markdown-content :deep(h4) {
  font-size: 16px;
}

.markdown-content :deep(p) {
  margin-bottom: 16px;
}

.markdown-content :deep(ul),
.markdown-content :deep(ol) {
  margin-bottom: 16px;
  padding-left: 24px;
}

.markdown-content :deep(li) {
  margin-bottom: 8px;
}

.markdown-content :deep(code) {
  background: var(--color-bg-card);
  padding: 2px 6px;
  border-radius: 4px;
  font-family: 'Courier New', monospace;
  font-size: 14px;
}

.markdown-content :deep(pre) {
  background: var(--color-bg-card);
  color: var(--color-text-sub);
  padding: 16px;
  border-radius: 8px;
  overflow-x: auto;
  margin-bottom: 16px;
}

.markdown-content :deep(pre code) {
  background: none;
  padding: 0;
  color: inherit;
}

.markdown-content :deep(a) {
  color: var(--color-primary);
  text-decoration: none;
}

.markdown-content :deep(a:hover) {
  text-decoration: underline;
}

.markdown-content :deep(blockquote) {
  border-left: var(--spacing-md) solid var(--color-primary);
  padding-left: 16px;
  margin: 16px 0;
  color: var(--color-text-sec);
  background: var(--color-bg-card);
  padding: 12px 16px;
  border-radius: 0 8px 8px 0;
}

.markdown-content :deep(table) {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 16px;
}

.markdown-content :deep(th),
.markdown-content :deep(td) {
  border: 1px solid var(--color-primary);
  padding: 8px 12px;
  text-align: left;
}

.markdown-content :deep(th) {
  background: var(--color-bg-card);
  font-weight: 600;
}

.markdown-content :deep(hr) {
  border: none;
  border-top: var(--spacing-sm) solid var(--color-border);
  margin: 24px 0;
}

/* 空内容 */
.empty-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  color: var(--color-text-sub);
}

.empty-content i {
  font-size: 48px;
  margin-bottom: 12px;
  opacity: 0.5;
}

.empty-content p {
  margin: 0;
}
</style>
