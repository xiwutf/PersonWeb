<template>
  <div class="visitor-messages-page">
    <div class="page-header">
      <h1 class="page-title">访客留言管理</h1>
      <p class="text-gray-400 text-sm">审核和管理访客发送的弹幕、留言、心情和祝福</p>
    </div>

    <!-- 统计信息 -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon stat-icon-pending">
          <i class="fas fa-clock"></i>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ stats.pending }}</div>
          <div class="stat-label">待审核</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon stat-icon-approved">
          <i class="fas fa-check-circle"></i>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ stats.approved }}</div>
          <div class="stat-label">已通过</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon stat-icon-rejected">
          <i class="fas fa-times-circle"></i>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ stats.rejected }}</div>
          <div class="stat-label">已拒绝</div>
        </div>
      </div>
    </div>

    <!-- 筛选栏 -->
    <div class="filter-bar">
      <select v-model="statusFilter" @change="fetchMessages" class="filter-select">
        <option value="">全部状态</option>
        <option value="pending">待审核</option>
        <option value="approved">已通过</option>
        <option value="rejected">已拒绝</option>
      </select>
      <select v-model="typeFilter" @change="fetchMessages" class="filter-select">
        <option value="">全部类型</option>
        <option value="message">留言</option>
        <option value="mood">心情</option>
        <option value="blessing">祝福</option>
      </select>
    </div>

    <!-- 留言列表 -->
    <div class="messages-container">
      <div v-if="loading" class="table-loading">加载中...</div>
      <div v-else-if="messages.length === 0" class="table-empty">暂无留言</div>
      <div v-else class="messages-list">
        <div
          v-for="message in messages"
          :key="message.id"
          class="message-card"
          :class="`message-card-${message.status}`"
        >
          <div class="message-header">
            <div class="message-type-badge" :class="`type-${message.messageType}`">
              <i :class="getTypeIcon(message.messageType)"></i>
              {{ getTypeName(message.messageType) }}
            </div>
            <div class="message-status-badge" :class="`status-${message.status}`">
              {{ getStatusName(message.status) }}
            </div>
          </div>

          <div class="message-body">
            <div class="message-content">
              <span v-if="message.emoji" class="message-emoji">{{ message.emoji }}</span>
              <span class="message-text">{{ message.content }}</span>
            </div>
          </div>

          <div class="message-footer">
            <div class="message-info">
              <span class="info-item">
                <i class="fas fa-user"></i>
                {{ message.visitorId.substring(0, 8) }}...
              </span>
              <span v-if="message.location" class="info-item">
                <i class="fas fa-map-marker-alt"></i>
                {{ message.location }}
              </span>
              <span class="info-item">
                <i class="fas fa-clock"></i>
                {{ formatTime(message.createdAt) }}
              </span>
            </div>

            <div class="message-actions">
              <button
                v-if="message.status === 'pending'"
                @click="approveMessage(message.id)"
                class="action-button action-button-approve"
              >
                <i class="fas fa-check"></i>
                通过
              </button>
              <button
                v-if="message.status === 'pending'"
                @click="rejectMessage(message.id)"
                class="action-button action-button-reject"
              >
                <i class="fas fa-times"></i>
                拒绝
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

interface VisitorMessage {
  id: number
  visitorId: string
  messageType: string
  content: string
  emoji?: string
  color?: string
  status: string
  location?: string
  createdAt: string
  approvedAt?: string
}

const api = useApi()
const { handleError } = useErrorHandler()
const message = useSafeMessage()

const messages = ref<VisitorMessage[]>([])
const allMessages = ref<VisitorMessage[]>([]) // 存储所有留言，用于统计
const loading = ref(false)
const statusFilter = ref('')
const typeFilter = ref('')

// 统计信息：基于所有留言计算
const stats = computed(() => {
  const all = allMessages.value
  return {
    pending: all.filter(m => m.status === 'pending').length,
    approved: all.filter(m => m.status === 'approved').length,
    rejected: all.filter(m => m.status === 'rejected').length
  }
})

// 根据筛选器过滤后的留言列表
const filteredMessages = computed(() => {
  let result = [...allMessages.value]
  
  if (statusFilter.value) {
    result = result.filter(m => m.status === statusFilter.value)
  }
  
  if (typeFilter.value) {
    result = result.filter(m => m.messageType === typeFilter.value)
  }
  
  return result
})

const fetchMessages = async () => {
  loading.value = true
  try {
    
    const res = await api.get<VisitorMessage[]>('/VisitorInteraction/messages/all')
    
     let messageList: VisitorMessage[] = []
    if (Array.isArray(res)) {
      messageList = res
    } else if (res && typeof res === 'object' && 'data' in res) {
      // 如果还是包装�data 中（双重包装的情况）
      messageList = Array.isArray((res as any).data) ? (res as any).data : []
    }
    
    allMessages.value = messageList
    messages.value = filteredMessages.value
  } catch (e) {
    console.error('加载留言失败:', e)
    handleError(e, '加载留言失败')
    allMessages.value = []
    messages.value = []
  } finally {
    loading.value = false
  }
}

// 监听筛选器变化，更新显示列�
watch([statusFilter, typeFilter], () => {
  messages.value = filteredMessages.value
})

const approveMessage = async (id: number) => {
  try {
    await api.post(`/VisitorInteraction/message/${id}/approve`)
    message.success('已通过')
    await fetchMessages()
  } catch (e) {
    handleError(e, '审核失败')
  }
}

const rejectMessage = async (id: number) => {
  try {
    await api.post(`/VisitorInteraction/message/${id}/reject`)
    message.success('已拒绝')
    await fetchMessages()
  } catch (e) {
    handleError(e, '操作失败')
  }
}

const getTypeName = (type: string): string => {
  const names: Record<string, string> = {
    message: '留言',
    mood: '心情',
    blessing: '祝福'
  }
  return names[type] || type
}

const getTypeIcon = (type: string): string => {
  const icons: Record<string, string> = {
    message: 'fas fa-comment',
    mood: 'fas fa-smile',
    blessing: 'fas fa-heart'
  }
  return icons[type] || 'fas fa-comment'
}

const getStatusName = (status: string): string => {
  const names: Record<string, string> = {
    pending: '待审核',
    approved: '已通过',
    rejected: '已拒绝'
  }
  return names[status] || status
}

const formatTime = (time: string): string => {
  const date = new Date(time)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(minutes / 60)
  const days = Math.floor(hours / 24)

  if (days > 0) return `${days}天前`
  if (hours > 0) return `${hours}小时前`
  if (minutes > 0) return `${minutes}分钟前`
  return '刚刚'
}

onMounted(() => {
  fetchMessages()
})
</script>

<style scoped>
.visitor-messages-page {
  width: 100%;
}

.page-header {
  margin-bottom: 1.5rem;
}

.page-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text-main);
  margin-bottom: 0.5rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.stat-card {
  background: var(--admin-surface-base, var(--color-bg-elevated, var(--color-bg-card)));
  border: 1px solid var(--admin-surface-border, var(--color-border-subtle));
  border-radius: 0.5rem;
  padding: 1rem;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.stat-icon {
  width: 3rem;
  height: 3rem;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
}

.stat-icon-pending {
  background: rgba(251, 191, 36, 0.2);
  color: var(--color-warning);
}

.stat-icon-approved {
  background: rgba(34, 197, 94, 0.2);
  color: var(--color-success);
}

.stat-icon-rejected {
  background: rgba(239, 68, 68, 0.2);
  color: var(--color-error);
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text-main);
}

.stat-label {
  font-size: 0.875rem;
  color: var(--color-text-muted);
}

.filter-bar {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.filter-select {
  padding: 0.5rem 1rem;
  background: var(--admin-surface-soft, var(--color-bg-elevated));
  border: 1px solid var(--admin-surface-border, var(--color-border-subtle));
  border-radius: 0.25rem;
  color: var(--color-text-main);
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.filter-select:focus {
  outline: none;
  border-color: var(--color-primary);
  background: var(--admin-surface-base, var(--color-bg-card));
}

.messages-container {
  background: var(--admin-surface-base, var(--color-bg-elevated, var(--color-bg-card)));
  border: 1px solid var(--admin-surface-border, var(--color-border-subtle));
  border-radius: 0.5rem;
  padding: 1.5rem;
}

.messages-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.message-card {
  background: var(--admin-surface-soft, var(--color-bg-elevated));
  border: 1px solid var(--admin-surface-border, var(--color-border-subtle));
  border-radius: 0.5rem;
  padding: 1rem;
  transition: all 0.2s ease;
}

.message-card:hover {
  background: var(--admin-surface-base, var(--color-bg-card));
  border-color: var(--color-border-default);
}

.message-card-pending {
  border-left: 3px solid var(--color-warning-400);
}

.message-card-approved {
  border-left: 3px solid var(--color-success);
}

.message-card-rejected {
  border-left: 3px solid var(--color-error);
}

.message-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.message-type-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.25rem 0.75rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.type-message {
  background: rgba(59, 130, 246, 0.2);
  color: var(--color-primary-hover);
  border: 1px solid rgba(59, 130, 246, 0.4);
}

.type-mood {
  background: rgba(236, 72, 153, 0.2);
  color: #f9a8d4;
  border: 1px solid rgba(236, 72, 153, 0.4);
}

.type-blessing {
  background: rgba(251, 191, 36, 0.2);
  color: var(--color-warning);
  border: 1px solid rgba(251, 191, 36, 0.4);
}

.message-status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.status-pending {
  background: rgba(251, 191, 36, 0.2);
  color: var(--color-warning);
  border: 1px solid rgba(251, 191, 36, 0.4);
}

.status-approved {
  background: rgba(34, 197, 94, 0.2);
  color: var(--color-success);
  border: 1px solid rgba(34, 197, 94, 0.4);
}

.status-rejected {
  background: rgba(239, 68, 68, 0.2);
  color: var(--color-error);
  border: 1px solid rgba(239, 68, 68, 0.4);
}

.message-body {
  margin-bottom: 0.75rem;
}

.message-content {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--color-text-main);
  font-size: 0.875rem;
  line-height: 1.6;
}

.message-emoji {
  font-size: 1.25rem;
}

.message-text {
  flex: 1;
}

.message-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 0.75rem;
  border-top: 1px solid var(--admin-surface-border, var(--color-border-subtle));
}

.message-info {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.75rem;
  color: var(--color-text-muted);
}

.message-actions {
  display: flex;
  gap: 0.5rem;
}

.action-button {
  padding: 0.5rem 1rem;
  border-radius: 0.25rem;
  border: none;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.action-button-approve {
  background: rgba(34, 197, 94, 0.3);
  border: 1px solid rgba(34, 197, 94, 0.5);
  color: #dcfce7;
}

.action-button-approve:hover {
  background: rgba(34, 197, 94, 0.4);
}

.action-button-reject {
  background: rgba(239, 68, 68, 0.3);
  border: 1px solid rgba(239, 68, 68, 0.5);
  color: #fee2e2;
}

.action-button-reject:hover {
  background: rgba(239, 68, 68, 0.4);
}

.table-loading,
.table-empty {
  text-align: center;
  padding: 3rem;
  color: var(--color-text-muted);
}
</style>
