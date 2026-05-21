<template>
  <div class="kanban-page">
    <div class="page-header">
      <h1 class="page-title">项目看板</h1>
    </div>

    <div v-if="loading" class="loading-container">
      加载中...
    </div>

    <div v-else-if="!hasData" class="empty-container">
      <div class="empty-content">
        <i class="fas fa-inbox" style="font-size: 48px; opacity: 0.3; margin-bottom: 16px;"></i>
        <p style="font-size: 16px; color: var(--color-text-muted); margin-bottom: 8px;">暂无项目数据</p>
        <p style="font-size: 14px; color: var(--color-text-muted);">请先创建项目或检查数据库连接</p>
        <n-button 
          type="primary" 
          style="margin-top: 16px;"
          @click="router.push('/admin/side-projects')"
        >
          前往项目列表
        </n-button>
      </div>
    </div>

    <div v-else class="kanban-container">
      <!-- 待开�?-->
      <div class="kanban-column">
        <div class="column-header">
          <h3>待开始</h3>
          <span class="count">{{ kanbanData.pending?.length || 0 }}</span>
        </div>
        <div class="column-content">
          <div
            v-for="project in kanbanData.pending"
            :key="project.id"
            class="kanban-card"
            @click="handleCardClick(project.id)"
          >
            <div class="card-header">
              <span class="card-title">{{ project.title }}</span>
              <n-tag v-if="project.priority" :type="getPriorityTagType(project.priority)" size="small">
                {{ getPriorityText(project.priority) }}
              </n-tag>
            </div>
            <div class="card-content">
              <div v-if="project.clientName" class="card-item">客户: {{ project.clientName }}</div>
              <div v-if="project.deadlineAt" class="card-item">截止: {{ formatDate(project.deadlineAt) }}</div>
              <div v-if="project.progress !== undefined" class="card-item">
                <n-progress
                  :percentage="project.progress"
                  :height="8"
                  status="success"
                />
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 进行中 -->
      <div class="kanban-column">
        <div class="column-header">
          <h3>进行中</h3>
          <span class="count">{{ kanbanData.inProgress?.length || 0 }}</span>
        </div>
        <div class="column-content">
          <div
            v-for="project in kanbanData.inProgress"
            :key="project.id"
            class="kanban-card"
            @click="handleCardClick(project.id)"
          >
            <div class="card-header">
              <span class="card-title">{{ project.title }}</span>
              <n-tag v-if="project.priority" :type="getPriorityTagType(project.priority)" size="small">
                {{ getPriorityText(project.priority) }}
              </n-tag>
            </div>
            <div class="card-content">
              <div v-if="project.clientName" class="card-item">客户: {{ project.clientName }}</div>
              <div v-if="project.deadlineAt" class="card-item">截止: {{ formatDate(project.deadlineAt) }}</div>
              <div v-if="project.progress !== undefined" class="card-item">
                <n-progress
                  :percentage="project.progress"
                  :height="8"
                  status="success"
                />
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 卡住 -->
      <div class="kanban-column">
        <div class="column-header blocked-header">
          <h3>卡住</h3>
          <span class="count">{{ kanbanData.blocked?.length || 0 }}</span>
        </div>
        <div class="column-content">
          <div
            v-for="project in kanbanData.blocked"
            :key="project.id"
            class="kanban-card blocked-card"
            @click="handleCardClick(project.id)"
          >
            <div class="card-header">
              <span class="card-title">{{ project.title }}</span>
              <n-tag type="error" size="small">阻塞</n-tag>
            </div>
            <div class="card-content">
              <div v-if="project.clientName" class="card-item">客户: {{ project.clientName }}</div>
              <div v-if="project.blockReason" class="card-item error-text">原因: {{ project.blockReason }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- 待验证 -->
      <div class="kanban-column">
        <div class="column-header">
          <h3>待验证</h3>
          <span class="count">{{ kanbanData.pendingReview?.length || 0 }}</span>
        </div>
        <div class="column-content">
          <div
            v-for="project in kanbanData.pendingReview"
            :key="project.id"
            class="kanban-card"
            @click="handleCardClick(project.id)"
          >
            <div class="card-header">
              <span class="card-title">{{ project.title }}</span>
            </div>
            <div class="card-content">
              <div v-if="project.clientName" class="card-item">客户: {{ project.clientName }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- 已完成 -->
      <div class="kanban-column">
        <div class="column-header">
          <h3>已完成</h3>
          <span class="count">{{ kanbanData.completed?.length || 0 }}</span>
        </div>
        <div class="column-content">
          <div
            v-for="project in kanbanData.completed"
            :key="project.id"
            class="kanban-card completed-card"
            @click="handleCardClick(project.id)"
          >
            <div class="card-header">
              <span class="card-title">{{ project.title }}</span>
            </div>
            <div class="card-content">
              <div v-if="project.clientName" class="card-item">客户: {{ project.clientName }}</div>
              <div v-if="project.priceFinal" class="card-item">金额: ¥{{ formatNumber(project.priceFinal) }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { NTag, NProgress, NButton } from 'naive-ui'
import { useApi } from '~/composables/useApi'
import { useNotification } from '~/composables/useToast'
import type { SideProject } from '~/types/api'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const router = useRouter()
const api = useApi()
const notification = useNotification()

const loading = ref(false)
const kanbanData = ref<{
  pending: SideProject[]
  inProgress: SideProject[]
  blocked: SideProject[]
  pendingReview: SideProject[]
  completed: SideProject[]
}>({
  pending: [],
  inProgress: [],
  blocked: [],
  pendingReview: [],
  completed: []
})

// 检查是否有数据
const hasData = computed(() => {
  return kanbanData.value.pending.length > 0 ||
         kanbanData.value.inProgress.length > 0 ||
         kanbanData.value.blocked.length > 0 ||
         kanbanData.value.pendingReview.length > 0 ||
         kanbanData.value.completed.length > 0
})

// 获取看板数据
const fetchKanban = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/side-projects/kanban')
    // 后端返回的字段名可能是大写开头（Pending, InProgress等）或驼峰命�?    // 兼容两种格式
    const data = res || {}
    kanbanData.value = {
      pending: data.pending || data.Pending || [],
      inProgress: data.inProgress || data.InProgress || [],
      blocked: data.blocked || data.Blocked || [],
      pendingReview: data.pendingReview || data.PendingReview || [],
      completed: data.completed || data.Completed || []
    }
    
    // 调试日志（开发环境）
    if (process.env.NODE_ENV === 'development') {
      console.log('看板数据:', kanbanData.value)
    }
  } catch (e: any) {
    console.error('加载看板数据失败:', e)
    notification.error('加载看板数据失败: ' + (e.message || '请检查网络连接或后端服务'))
  } finally {
    loading.value = false
  }
}

// 卡片点击，跳转到详情页
const handleCardClick = (id: number) => {
  router.push(`/admin/side-projects/projects/${id}?from=kanban`)
}

// 获取优先级标签类型
const getPriorityTagType = (priority?: number): 'default' | 'success' | 'error' | 'warning' | 'info' | 'primary' => {
  if (!priority) return 'default'
  switch (priority) {
    case 3: return 'error' // 紧急
    case 2: return 'warning' // 重要
    case 1: return 'info' // 一般
    default: return 'default' // 未知
  }
}

// 获取优先级文本
const getPriorityText = (priority?: number): string => {
  if (!priority) return ''
  switch (priority) {
    case 3: return '紧急'
    case 2: return '重要'
    case 1: return '一般'
    default: return '未知'
  }
}

// 格式化日期
const formatDate = (dateStr: string): string => {
  if (!dateStr) return '-'
  try {
    const date = new Date(dateStr)
    return date.toLocaleDateString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    })
  } catch {
    return dateStr
  }
}

// 格式化数字
const formatNumber = (num: number): string => {
  if (typeof num !== 'number' || isNaN(num)) return '0'
  return num.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

onMounted(() => {
  fetchKanban()
})
</script>

<style scoped>
.kanban-page {
  padding: var(--spacing-xl);
}

.page-header {
  margin-bottom: var(--spacing-xl);
}

.page-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0;
}

.loading-container {
  padding: var(--spacing-3xl);
  text-align: center;
  color: var(--color-text-muted);
}

.kanban-container {
  display: flex;
  gap: var(--spacing-lg);
  overflow-x: auto;
  padding-bottom: var(--spacing-lg);
  /* 自定义横向滚动条样式 */
  scrollbar-width: thin;
  scrollbar-color: var(--color-border-subtle) transparent;
}

.kanban-container::-webkit-scrollbar {
  height: 8px;
}

.kanban-container::-webkit-scrollbar-track {
  background: transparent;
  border-radius: 4px;
}

.kanban-container::-webkit-scrollbar-thumb {
  background: var(--color-border-subtle);
  border-radius: 4px;
  transition: background 0.2s ease;
}

.kanban-container::-webkit-scrollbar-thumb:hover {
  background: var(--color-text-muted);
}

.kanban-column {
  min-width: 300px;
  width: 300px;
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  border: 1px solid var(--color-border-subtle);
  display: flex;
  flex-direction: column;
  max-height: calc(100vh - 180px);
  min-height: 400px;
  height: calc(100vh - 180px);
}

.column-header {
  padding: var(--spacing-md) var(--spacing-lg);
  border-bottom: 2px solid var(--color-border-subtle);
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: var(--color-bg-elevated);
  border-radius: var(--radius-lg) var(--radius-lg) 0 0;
}

.column-header.blocked-header {
  background: rgba(239, 68, 68, 0.1);
  border-bottom-color: rgba(239, 68, 68, 0.3);
}

.column-header h3 {
  margin: 0;
  font-size: var(--font-size-lg);
  font-weight: 600;
  color: var(--color-text-main);
}

.count {
  background: var(--color-bg-elevated);
  border-radius: 12px;
  padding: 2px 8px;
  font-size: var(--font-size-sm);
  font-weight: 600;
  color: var(--color-text-muted);
}

.column-content {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  padding: var(--spacing-md);
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  min-height: 0; /* 确保 flex 子元素可以正确收�?*/
  /* 自定义滚动条样式 */
  scrollbar-width: thin;
  scrollbar-color: var(--color-border-subtle) transparent;
}

.column-content::-webkit-scrollbar {
  width: 6px;
}

.column-content::-webkit-scrollbar-track {
  background: transparent;
  border-radius: 3px;
}

.column-content::-webkit-scrollbar-thumb {
  background: var(--color-border-subtle);
  border-radius: 3px;
  transition: background 0.2s ease;
}

.column-content::-webkit-scrollbar-thumb:hover {
  background: var(--color-text-muted);
}

.kanban-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  padding: var(--spacing-md);
  cursor: pointer;
  transition: all 0.2s ease;
}

.kanban-card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}

.kanban-card.blocked-card {
  border-color: rgba(239, 68, 68, 0.5);
  background: rgba(239, 68, 68, 0.05);
}

.kanban-card.completed-card {
  opacity: 0.7;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-sm);
  gap: var(--spacing-sm);
}

.card-title {
  font-weight: 600;
  color: var(--color-text-main);
  font-size: var(--font-size-base);
  flex: 1;
}

.card-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.card-item {
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
}

.error-text {
  color: var(--color-danger);
}

.empty-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
  padding: var(--spacing-3xl);
}

.empty-content {
  text-align: center;
}
</style>

