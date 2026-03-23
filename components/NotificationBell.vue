<template>
  <div class="notification-bell-container">
    <n-popover
      v-model:show="showPopover"
      trigger="click"
      placement="bottom-end"
      raw
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
          <button
            class="popover-link"
            @click="handleViewAll"
          >
            查看全部
          </button>
        </div>

        <div v-if="loading" class="popover-loading">
          <n-spin size="small" />
        </div>

        <div v-else-if="recentNotifications.length === 0" class="popover-empty">
          <div class="empty-state">
            <div class="empty-state-icon">
              <i class="fas fa-inbox"></i>
            </div>
            <div class="empty-state-title">暂无未读提醒</div>
            <div class="empty-state-text">新的项目提醒和系统消息会显示在这里</div>
          </div>
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
import { NPopover, NBadge, NTag, NSpin } from 'naive-ui'
import { useApi } from '~/composables/useApi'
import type { SideNotification, NotificationSeverity } from '~/types/api'

// Nuxt 3 中使用 useRouter，不需要从 vue-router 导入
const router = useRouter()
const api = useApi()

const showPopover = ref(false)
const loading = ref(false)
const unreadCount = ref(0)
const recentNotifications = ref<SideNotification[]>([])

let pollTimer: NodeJS.Timeout | null = null
let visibilityHandler: (() => void) | null = null

const fetchUnreadCount = async () => {
  try {
    const res = await api.get<{ count: number }>('/side-notifications/unread-count')
    unreadCount.value = res.count || 0
  } catch (e) {
    console.error('获取未读数量失败:', e)
  }
}

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

const handleBellClick = () => {
  if (showPopover.value) {
    fetchRecentNotifications()
  }
}

const handleViewAll = () => {
  showPopover.value = false
  router.push('/admin/side-projects/notifications')
}

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

const getSeverityTagType = (severity: NotificationSeverity): 'default' | 'success' | 'error' | 'warning' | 'info' => {
  switch (severity) {
    case 3:
      return 'error'
    case 2:
      return 'warning'
    case 1:
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
  if (minutes < 60) return `${minutes} 分钟前`
  if (hours < 24) return `${hours} 小时前`
  if (days < 7) return `${days} 天前`
  return date.toLocaleDateString('zh-CN')
}

const startPolling = () => {
  if (pollTimer) return

  fetchUnreadCount()
  pollTimer = setInterval(() => {
    if (document.hidden) return
    fetchUnreadCount()
  }, 60000)
}

const stopPolling = () => {
  if (pollTimer) {
    clearInterval(pollTimer)
    pollTimer = null
  }
}

onMounted(() => {
  startPolling()

  visibilityHandler = () => {
    if (document.hidden) {
      stopPolling()
      return
    }

    fetchUnreadCount()
    startPolling()
  }

  document.addEventListener('visibilitychange', visibilityHandler)
})

onUnmounted(() => {
  stopPolling()
  if (visibilityHandler) {
    document.removeEventListener('visibilitychange', visibilityHandler)
  }
})
</script>

<style scoped>
.notification-bell-container {
  position: relative;
}

.bell-button {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-sm) var(--spacing-md);
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.bell-button:hover {
  background-color: var(--admin-surface-2, var(--color-bg-elevated));
}

.bell-button i {
  color: var(--color-text-main);
  font-size: var(--text-lg);
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
  overflow: hidden;
  background: var(--admin-surface-1, var(--color-bg-card));
  border: 1px solid var(--color-border);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-xl);
}

.popover-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-md) var(--spacing-lg);
  border-bottom: 1px solid var(--color-border);
}

.popover-header h3 {
  margin: 0;
  color: var(--color-text-main);
  font-size: var(--text-base);
  font-weight: 700;
}

.popover-link {
  border: none;
  background: none;
  color: var(--color-primary-hover);
  font-size: var(--text-sm);
  font-weight: 600;
  cursor: pointer;
}

.popover-loading,
.popover-empty {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 180px;
  padding: var(--spacing-lg);
}

.popover-list {
  max-height: 400px;
  overflow-y: auto;
  padding: var(--spacing-sm) 0;
}

.popover-item {
  padding: var(--spacing-md) var(--spacing-lg);
  border-bottom: 1px solid var(--color-border);
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.popover-item:hover {
  background-color: var(--admin-surface-2, var(--color-bg-elevated));
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
  flex: 1;
  color: var(--color-text-main);
  font-size: var(--text-sm);
  font-weight: 600;
}

.popover-item-content {
  margin-top: var(--spacing-xs);
  color: var(--color-text-muted);
  font-size: var(--text-xs);
  line-height: 1.5;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.popover-item-time {
  margin-top: var(--spacing-xs);
  color: var(--color-text-muted);
  font-size: var(--text-xs);
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  text-align: center;
}

.empty-state-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 3rem;
  height: 3rem;
  border-radius: 999px;
  border: 1px solid var(--color-border);
  background: var(--admin-surface-2, var(--color-bg-elevated));
  color: var(--color-text-muted);
  font-size: 1.2rem;
}

.empty-state-title {
  color: var(--color-text-main);
  font-size: 0.95rem;
  font-weight: 600;
}

.empty-state-text {
  max-width: 240px;
  color: var(--color-text-muted);
  font-size: 0.8rem;
  line-height: 1.6;
}
</style>
