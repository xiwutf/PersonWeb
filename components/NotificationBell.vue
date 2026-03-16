<template>
  <div class="notification-bell-container">
    <n-popover
      v-model:show="showPopover"
      trigger="click"
      placement="bottom-end"
      :style="{ width: '400px', maxHeight: '500px' }"
    >
      <template #trigger>
        <div class="bell-button" @click="handleBellClick">
          <i class="fas fa-bell"></i>
          <n-badge
            v-if="unreadCount > 0"
            :value="unreadCount > 99 ? '99+' : unreadCount"
            :max="99"
            class="bell-badge"
          />
        </div>
      </template>

      <div class="notification-popover">
        <div class="popover-header">
          <h3>站内提醒</h3>
          <n-button
            text
            size="small"
            @click="handleViewAll"
          >
            查看全部
          </n-button>
        </div>

        <div v-if="loading" class="popover-loading">
          <n-spin size="small" />
        </div>

        <div v-else-if="recentNotifications.length === 0" class="popover-empty">
          <n-empty size="small" description="暂无未读提醒" />
        </div>

        <div v-else class="popover-list">
          <div
            v-for="notification in recentNotifications"
            :key="notification.id"
            class="popover-item"
            @click="handleNotificationClick(notification)"
          >
            <div class="popover-item-header">
              <n-tag
                :type="getSeverityTagType(notification.severity)"
                size="tiny"
              >
                {{ getSeverityText(notification.severity) }}
              </n-tag>
              <span class="popover-item-title">{{ notification.title }}</span>
            </div>
            <div v-if="notification.content" class="popover-item-content">
              {{ notification.content }}
            </div>
            <div class="popover-item-time">
              {{ formatTime(notification.lastTriggeredAt || notification.createdAt) }}
            </div>
          </div>
        </div>
      </div>
    </n-popover>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { NPopover, NBadge, NButton, NTag, NEmpty, NSpin } from 'naive-ui'
import { useApi } from '~/composables/useApi'
import type { SideNotification, NotificationSeverity } from '~/types/api'

const router = useRouter()
const api = useApi()

const showPopover = ref(false)
const loading = ref(false)
const unreadCount = ref(0)
const recentNotifications = ref<SideNotification[]>([])

let pollTimer: NodeJS.Timeout | null = null

// 获取未读数量
const fetchUnreadCount = async () => {
  try {
    const res = await api.get<{ count: number }>('/side-notifications/unread-count')
    unreadCount.value = res.count || 0
  } catch (e) {
    console.error('获取未读数量失败:', e)
  }
}

// 获取最近10条未读提醒
const fetchRecentNotifications = async () => {
  loading.value = true
  try {
    const res = await api.get('/side-notifications', {
      params: {
        status: 'unread',
        page: 1,
        pageSize: 10
      }
    })
    recentNotifications.value = res.items || []
    unreadCount.value = res.unreadCount || 0
  } catch (e) {
    console.error('获取最近提醒失败:', e)
  } finally {
    loading.value = false
  }
}

// 点击铃铛
const handleBellClick = () => {
  if (showPopover.value) {
    fetchRecentNotifications()
  }
}

// 查看全部
const handleViewAll = () => {
  showPopover.value = false
  router.push('/admin/side-projects/notifications')
}

// 点击提醒项
const handleNotificationClick = (notification: SideNotification) => {
  showPopover.value = false
  if (notification.entityType === 'SideProject') {
    router.push(`/admin/side-projects/projects/${notification.entityId}`)
  } else if (notification.entityType === 'SideProjectTask') {
    const payload = notification.payloadJson ? JSON.parse(notification.payloadJson) : {}
    const projectId = payload.ProjectId || payload.projectId
    if (projectId) {
      router.push(`/admin/side-projects/projects/${projectId}?tab=tasks&taskId=${notification.entityId}`)
    }
  }
}

// 工具函数
const getSeverityTagType = (severity: NotificationSeverity): 'default' | 'success' | 'error' | 'warning' | 'info' => {
  switch (severity) {
    case 3: // Danger
      return 'error'
    case 2: // Warning
      return 'warning'
    case 1: // Info
      return 'info'
    default:
      return 'default'
  }
}

const getSeverityText = (severity: NotificationSeverity): string => {
  switch (severity) {
    case 3:
      return '紧急'
    case 2:
      return '警告'
    case 1:
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

// 轮询未读数量（每60秒）
const startPolling = () => {
  fetchUnreadCount()
  pollTimer = setInterval(() => {
    fetchUnreadCount()
  }, 60000) // 60秒
}

// 初始化
onMounted(() => {
  fetchUnreadCount()
  startPolling()
})

onUnmounted(() => {
  if (pollTimer) {
    clearInterval(pollTimer)
  }
})
</script>

<style scoped>
.notification-bell-container {
  position: relative;
}

.bell-button {
  position: relative;
  cursor: pointer;
  padding: var(--spacing-sm) var(--spacing-md);
  border-radius: var(--radius-md);
  transition: background-color 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.bell-button:hover {
  background-color: var(--color-bg-hover);
}

.bell-button i {
  font-size: var(--text-lg);
  color: var(--color-text-main);
}

.bell-badge {
  position: absolute;
  top: var(--spacing-sm);
  right: var(--spacing-sm);
}

.notification-popover {
  display: flex;
  flex-direction: column;
  max-height: 500px;
}

.popover-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-md) var(--spacing-lg);
  border-bottom: 1px solid var(--color-border-subtle);
}

.popover-header h3 {
  margin: 0;
  font-size: var(--text-base);
  font-weight: 600;
}

.popover-loading,
.popover-empty {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: var(--spacing-lg);
  min-height: var(--spacing-xl);
}

.popover-list {
  max-height: 400px;
  overflow-y: auto;
  padding: var(--spacing-sm) 0;
}

.popover-item {
  padding: var(--spacing-md) var(--spacing-lg);
  cursor: pointer;
  transition: background-color 0.2s ease;
  border-bottom: 1px solid var(--color-border-subtle);
}

.popover-item:hover {
  background-color: var(--color-bg-hover);
}

.popover-item:last-child {
  border-bottom: none;
}

.popover-item-header {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-xs);
}

.popover-item-title {
  font-weight: 600;
  font-size: var(--text-sm);
  flex: 1;
}

.popover-item-content {
  font-size: var(--text-xs);
  color: var(--color-text-secondary);
  margin-top: var(--spacing-xs);
  line-height: 1.4;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.popover-item-time {
  font-size: var(--text-xs);
  color: var(--color-text-muted);
  margin-top: var(--spacing-xs);
}
</style>

