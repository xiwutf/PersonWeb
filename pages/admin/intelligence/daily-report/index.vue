<template>
  <ClientOnly>
    <div class="intelligence-daily-report-page">
      <!-- 页面标题 -->
      <div class="page-header">
        <h1 class="page-title">每日情报简报</h1>
        <n-button type="primary" @click="navigateTo('/admin/intelligence')">
          <template #icon>
            <i class="fas fa-arrow-left"></i>
          </template>
          返回首页
        </n-button>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <n-spin size="large" />
      </div>

      <div v-else>
        <!-- 空状态 -->
        <div v-if="reports.length === 0" class="empty-container">
          <i class="fas fa-file-alt"></i>
          <p>暂无日报数据</p>
          <n-button type="primary" @click="handleGenerateReport">
            生成第一份日报
          </n-button>
        </div>

        <!-- 日报列表 -->
        <div v-else class="reports-list">
          <div
            v-for="report in reports"
            :key="report.id"
            class="report-card"
            @click="navigateTo(`/admin/intelligence/daily-report/${report.id}`)"
          >
            <div class="report-header">
              <div class="report-date">{{ formatDate(report.reportDate) }}</div>
              <div class="report-actions">
                <n-button text size="small" @click.stop="openReport(report.id)">
                  <template #icon>
                    <i class="fas fa-eye"></i>
                  </template>
                </n-button>
              </div>
            </div>
            <div class="report-title">{{ report.title }}</div>
            <div class="report-meta">
              <span><i class="fas fa-list"></i> {{ report.itemCount }} 条内容</span>
              <span><i class="fas fa-clock"></i> {{ formatTime(report.generatedAt) }}</span>
            </div>
          </div>
        </div>

        <!-- 分页 -->
        <div v-if="reports.length > 0" class="pagination-container">
          <n-pagination
            v-model:page="pagination.page"
            v-model:page-size="pagination.pageSize"
            :item-count="pagination.total"
            :page-sizes="[10, 20, 50]"
            show-size-picker
            @update:page="handlePageChange"
            @update:page-size="handlePageSizeChange"
          />
        </div>
      </div>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useIntelligenceApi } from '~/composables/useIntelligenceApi'
import { useNotification } from '~/composables/useToast'
import type { ReportResponseDto } from '~/types/intelligence'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useIntelligenceApi()
const notification = useNotification()

//  加载状态
const reports = ref<ReportResponseDto[]>([])

// 分页
const pagination = reactive({
  page: 1,
  pageSize: 20,
  total: 0
})

// 获取日报列表
const fetchReports = async () => {
  loading.value = true
  try {
    const result = await api.getReportList({
      pageIndex: pagination.page,
      pageSize: pagination.pageSize
    })
    reports.value = result.list
    pagination.total = result.total
  } catch (error) {
    console.error('获取日报列表失败:', error)
  } finally {
    loading.value = false
  }
}

// 翻页
const handlePageChange = (page: number) => {
  pagination.page = page
  fetchReports()
}

// 改变每页数量
const handlePageSizeChange = (pageSize: number) => {
  pagination.pageSize = pageSize
  pagination.page = 1
  fetchReports()
}

// 生成第一份日报
const handleGenerateReport = async () => {
  try {
    await api.runGenerateReport()
    notification.success('日报生成任务已提交')
    await fetchReports()
  } catch (error) {
    console.error('生成日报失败:', error)
    notification.error('日报生成任务提交失败')
  }
}

// 打开日报
const openReport = (id: number) => {
  navigateTo(`/admin/intelligence/daily-report/${id}`)
}

// 格式化日期
const formatDate = (date: string) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}年${d.getMonth() + 1}月${d.getDate()}日`
}

// 格式化时间
const formatTime = (time?: string) => {
  if (!time) return ''
  const d = new Date(time)
  const now = new Date()
  const diff = now.getTime() - d.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)

  if (minutes < 60) return `${minutes}分钟前`
  if (hours < 24) return `${hours}小时前`
  return `${days}天前`
}

// 初始化
onMounted(() => {
  fetchReports()
})
</script>

<style scoped>
.intelligence-daily-report-page {
  padding: var(--spacing-lg);
  max-width: 1000px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-2xl);
}

.page-title {
  font-size: var(--text-2xl);
  font-weight: 600;
  margin: 0;
  color: var(--color-text-main);
}

.loading-container,
.empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: var(--spacing-3xl);
  color: var(--color-text-sub);
}

.empty-container i {
  font-size: var(--text-5xl);
  margin-bottom: var(--spacing-md);
  opacity: 0.5;
}

.empty-container p {
  margin: 0 0 var(--spacing-base) 0;
}

/* 日报列表 */
.reports-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-base);
}

.report-card {
  background: var(--color-bg-card);
  border-radius: var(--radius-md);
  padding: var(--spacing-lg);
  box-shadow: var(--shadow-card);
  cursor: pointer;
  transition: all 0.2s;
}

.report-card:hover {
  transform: translateY(var(--spacing-sm));
  box-shadow: 0 4px var(--spacing-md) rgba(0, 0, 0, 0.12);
}

.report-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-md);
}

.report-date {
  font-size: var(--text-base);
  color: var(--color-primary);
  font-weight: 500;
}

.report-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-md);
}

.report-meta {
  display: flex;
  gap: var(--spacing-base);
  font-size: var(--text-sm);
  color: var(--color-text-sec);
}

.report-meta i {
  margin-right: var(--spacing-xs);
}

/* 分页 */
.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: var(--spacing-2xl);
}
</style>
