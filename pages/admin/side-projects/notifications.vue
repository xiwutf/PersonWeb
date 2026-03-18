<template>
  <div class="notification-center-page">
    <div class="page-header">
      <h1 class="page-title">站内提醒中心</h1>
      <div class="header-actions">
        <n-button type="primary" @click="handleMarkAllAsRead" :disabled="unreadCount === 0">
          <template #icon>
            <i class="fas fa-check-double"></i>
          </template>
          全部已读
        </n-button>
      </div>
    </div>

    <!-- 筛选条 -->
    <n-card class="filter-card mb-4">
      <div class="filter-row">
        <n-select
          v-model:value="filters.status"
          :options="statusOptions"
          placeholder="状态筛选"
          style="width: 150px"
          clearable
        />
        <n-select
          v-model:value="filters.severity"
          :options="severityOptions"
          placeholder="严重程度"
          style="width: 150px"
          clearable
        />
        <n-select
          v-model:value="filters.type"
          :options="typeOptions"
          placeholder="提醒类型"
          style="width: 180px"
          clearable
        />
        <n-input
          v-model:value="filters.keyword"
          placeholder="搜索标题/内容"
          style="width: 250px"
          clearable
        >
          <template #prefix>
            <i class="fas fa-search"></i>
          </template>
        </n-input>
        <n-button type="primary" @click="handleSearch">搜索</n-button>
        <n-button @click="handleReset">重置</n-button>
      </div>
    </n-card>

    <!-- 提醒列表 -->
    <div v-if="loading" class="loading-container">
      <n-spin size="large" />
    </div>

    <div v-else-if="notifications.length === 0" class="empty-container">
      <n-empty description="暂无提醒">
        <template #icon>
          <i class="fas fa-bell-slash" style="font-size: 48px; opacity: 0.3;"></i>
        </template>
      </n-empty>
    </div>

    <div v-else class="notification-list">
      <n-card
        v-for="notification in notifications"
        :key="notification.id"
        class="notification-card mb-3"
        :class="{
          'notification-unread': !notification.isRead,
          'notification-dismissed': notification.isDismissed
        }"
      >
        <div class="notification-content">
          <div class="notification-header">
            <div class="notification-title-row">
              <n-tag
                :type="getSeverityTagType(notification.severity)"
                size="small"
                class="severity-tag"
              >
                {{ getSeverityText(notification.severity) }}
              </n-tag>
              <span class="notification-title">{{ notification.title }}</span>
              <n-tag
                v-if="!notification.isRead"
                type="error"
                size="small"
                class="unread-badge"
              >
                未读
              </n-tag>
            </div>
            <div class="notification-time">
              {{ formatTime(notification.lastTriggeredAt || notification.createdAt) }}
            </div>
          </div>

          <div v-if="notification.content" class="notification-body">
            {{ notification.content }}
          </div>

          <div class="notification-footer">
            <div class="notification-actions">
              <n-button
                v-if="!notification.isRead"
                size="small"
                @click="handleMarkAsRead(notification.id)"
              >
                标记已读
              </n-button>
              <n-button
                size="small"
                @click="handleViewDetail(notification)"
              >
                查看详情
              </n-button>
              <n-dropdown
                :options="snoozeOptions"
                @select="(key: string) => handleSnooze(notification.id, key)"
              >
                <n-button size="small">
                  延后
                  <template #icon>
                    <i class="fas fa-chevron-down"></i>
                  </template>
                </n-button>
              </n-dropdown>
              <n-button
                size="small"
                type="error"
                quaternary
                @click="handleDismiss(notification.id)"
              >
                忽略
              </n-button>
            </div>
          </div>
        </div>
      </n-card>

      <!-- 分页 -->
      <div class="pagination-container">
        <n-pagination
          v-model:page="pagination.page"
          v-model:page-size="pagination.pageSize"
          :item-count="pagination.total"
          :page-sizes="[10, 20, 50, 100]"
          show-size-picker
          @update:page="handlePageChange"
          @update:page-size="handlePageSizeChange"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { NCard, NButton, NSelect, NInput, NTag, NEmpty, NSpin, NPagination, NDropdown, useNotification } from 'naive-ui'
import { useApi } from '~/composables/useApi'
import type { SideNotification, NotificationQueryParams, NotificationListResponse, NotificationType, NotificationSeverity } from '~/types/api'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const router = useRouter()
const api = useApi()
const notification = useNotification()

// 数据
const loading = ref(false)
const notifications = ref<SideNotification[]>([])
const unreadCount = ref(0)

// 筛选条件
const filters = ref<NotificationQueryParams>({
  status: 'unread',
  severity: undefined,
  type: undefined,
  keyword: undefined
})

// 分页
const pagination = ref({
  page: 1,
  pageSize: 20,
  total: 0
})

// 选项
const statusOptions = [
  { label: '未读', value: 'unread' },
  { label: '全部', value: 'all' },
  { label: '已忽略', value: 'dismissed' }
]

const severityOptions = [
  { label: '信息', value: 'info' },
  { label: '警告', value: 'warning' },
  { label: '危险', value: 'danger' }
]

const typeOptions = [
  { label: '项目即将到期', value: NotificationType.ProjectDueSoon },
  { label: '任务今天到期', value: NotificationType.TaskDueToday },
  { label: '项目卡住超过2周', value: NotificationType.ProjectBlockedTooLong }
]

const snoozeOptions = [
  { label: '延后1天', key: '1d' },
  { label: '延后3天', key: '3d' },
  { label: '延后到下周一', key: 'nextMon' }
]

// 获取提醒列表
const fetchNotifications = async () => {
  loading.value = true
  try {
    const params: any = {
      page: pagination.value.page,
      pageSize: pagination.value.pageSize
    }

    if (filters.value.status) params.status = filters.value.status
    if (filters.value.severity) params.severity = filters.value.severity
    if (filters.value.type !== undefined) params.type = filters.value.type
    if (filters.value.keyword) params.keyword = filters.value.keyword

    const res = await api.get<NotificationListResponse>('/side-notifications', { params })
    notifications.value = res.items || []
    pagination.value.total = res.total || 0
    unreadCount.value = res.unreadCount || 0
  } catch (e: any) {
    console.error('获取提醒列表失败:', e)
    notification.error('获取提醒列表失败: ' + (e.message || '未知错误'))
  } finally {
    loading.value = false
  }
}

// 标记已读
const handleMarkAsRead = async (id: number) => {
  try {
    await api.post(`/side-notifications/${id}/read`)
    notification.success('已标记为已读')
    await fetchNotifications()
  } catch (e: any) {
    notification.error('操作失败: ' + (e.message || '未知错误'))
  }
}

// 全部已读
const handleMarkAllAsRead = async () => {
  try {
    await api.post('/side-notifications/read-all')
    notification.success('已全部标记为已读')
    await fetchNotifications()
  } catch (e: any) {
    notification.error('操作失败: ' + (e.message || '未知错误'))
  }
}

// 忽略
const handleDismiss = async (id: number) => {
  if (!confirm('确定要忽略这条提醒吗?')) return
  try {
    await api.post(`/side-notifications/${id}/dismiss`)
    notification.success('已忽略')
    await fetchNotifications()
  } catch (e: any) {
    notification.error('操作失败: ' + (e.message || '未知错误'))
  }
}

// 延后
const handleSnooze = async (id: number, preset: string) => {
  try {
    await api.post(`/side-notifications/${id}/snooze`, { preset })
    notification.success('已延后')
    await fetchNotifications()
  } catch (e: any) {
    notification.error('操作失败: ' + (e.message || '未知错误'))
  }
}

// 查看详情
const handleViewDetail = (notification: SideNotification) => {
  if (notification.entityType === 'SideProject') {
    router.push(`/admin/side-projects/projects/${notification.entityId}`)
  } else if (notification.entityType === 'SideProjectTask') {
    // 跳转到项目详情页的任务Tab
    const payload = notification.payloadJson ? JSON.parse(notification.payloadJson) : {}
    const projectId = payload.ProjectId || payload.projectId
    if (projectId) {
      router.push(`/admin/side-projects/projects/${projectId}tab=tasks&taskId=${notification.entityId}`)
    }
  }
}

// 搜索
const handleSearch = () => {
  pagination.value.page = 1
  fetchNotifications()
}

// 重置
const handleReset = () => {
  filters.value = {
    status: 'unread',
    severity: undefined,
    type: undefined,
    keyword: undefined
  }
  pagination.value.page = 1
  fetchNotifications()
}

// 分页变化
const handlePageChange = (page: number) => {
  pagination.value.page = page
  fetchNotifications()
}

const handlePageSizeChange = (pageSize: number) => {
  pagination.value.pageSize = pageSize
  pagination.value.page = 1
  fetchNotifications()
}

// 工具函数
const getSeverityTagType = (severity: NotificationSeverity): 'default' | 'success' | 'error' | 'warning' | 'info' => {
  switch (severity) {
    case NotificationSeverity.Danger:
      return 'error'
    case NotificationSeverity.Warning:
      return 'warning'
    case NotificationSeverity.Info:
      return 'info'
    default:
      return 'default'
  }
}

const getSeverityText = (severity: NotificationSeverity): string => {
  switch (severity) {
    case NotificationSeverity.Danger:
      return '紧急'
    case NotificationSeverity.Warning:
      return '警告'
    case NotificationSeverity.Info:
      return '信息'
    default:
      return '未知'
  }
}

const formatTime = (time: string): string => {
  const date = new Date(time)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)

  if (minutes < 1) return '刚刚'
  if (minutes < 60) return `${minutes}分钟前`
  if (hours < 24) return `${hours}小时前`
  if (days < 7) return `${days}天前`
  return date.toLocaleDateString('zh-CN')
}

// 初始化
onMounted(() => {
  fetchNotifications()
})
</script>

<style scoped>
.notification-center-page {
  padding: var(--spacing-lg);
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  margin: 0;
}

.filter-card {
  background: var(--color-bg-card);
}

.filter-row {
  display: flex;
  gap: var(--spacing-md);
  align-items: center;
  flex-wrap: wrap;
}

.notification-list {
  display: flex;
  flex-direction: column;
}

.notification-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  transition: all 0.2s ease;
}

.notification-card:hover {
  box-shadow: var(--shadow-md);
}

.notification-unread {
  border-left: 4px solid var(--color-primary);
  background: rgba(var(--color-primary-rgb), 0.05);
}

.notification-dismissed {
  opacity: 0.6;
}

.notification-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.notification-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.notification-title-row {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  flex: 1;
}

.severity-tag {
  flex-shrink: 0;
}

.notification-title {
  font-weight: 600;
  font-size: 16px;
  flex: 1;
}

.unread-badge {
  flex-shrink: 0;
}

.notification-time {
  font-size: 12px;
  color: var(--color-text-muted);
  flex-shrink: 0;
}

.notification-body {
  color: var(--color-text-secondary);
  line-height: 1.6;
  margin-top: var(--spacing-xs);
}

.notification-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: var(--spacing-sm);
}

.notification-actions {
  display: flex;
  gap: var(--spacing-sm);
}

.loading-container,
.empty-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: var(--spacing-lg);
}
</style>

