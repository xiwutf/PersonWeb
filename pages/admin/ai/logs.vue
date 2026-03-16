<template>
  <ClientOnly>
    <div class="admin-ai-logs-page p-6">
      <div class="mb-6">
        <h1 class="text-2xl font-bold mb-2">AI 智能体调用日�?/h1>
        <p class="text-gray-600 dark:text-gray-400">查看所�?AI 智能体的调用记录和状�?/p>
      </div>

      <!-- 筛选栏 -->
      <n-card class="mb-6">
        <div class="filters-bar">
          <n-select
            v-model:value="filterAgentType"
            placeholder="智能体类�?
            clearable
            style="width: 200px;"
            :options="agentTypeOptions"
          />
          <n-select
            v-model:value="filterSuccess"
            placeholder="状�?
            clearable
            style="width: 150px;"
            :options="successOptions"
          />
          <n-button type="primary" @click="handleSearch">搜索</n-button>
          <n-button quaternary @click="handleReset">重置</n-button>
        </div>
      </n-card>

      <!-- 日志列表 -->
      <n-card>
        <template #header>
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold">调用记录</h3>
            <n-button text size="small" @click="fetchLogs" :loading="loading">
              <template #icon>
                <i class="fas fa-sync-alt"></i>
              </template>
              刷新
            </n-button>
          </div>
        </template>

        <div v-if="loading" class="text-center py-8">
          <n-spin size="large" />
        </div>
        <div v-else-if="logs.length === 0" class="text-center py-8 text-gray-500">
          暂无日志记录
        </div>
        <div v-else class="logs-list">
          <div
            v-for="log in logs"
            :key="log.id"
            class="log-item"
            :class="{ 'log-item-error': !log.success }"
          >
            <div class="log-header">
              <div class="log-meta">
                <n-tag :type="log.success ? 'success' : 'error'" size="small">
                  {{ log.success ? '成功' : '失败' }}
                </n-tag>
                <n-tag type="info" size="small">{{ log.agentType }}</n-tag>
                <span class="log-time">{{ formatDate(log.createdAt) }}</span>
              </div>
            </div>
            <div v-if="log.errorMessage" class="log-error">
              <i class="fas fa-exclamation-circle"></i>
              {{ log.errorMessage }}
            </div>
            <div class="log-details">
              <n-collapse>
                <n-collapse-item title="查看请求详情" name="request">
                  <pre class="log-json">{{ formatJson(log.requestPayload) }}</pre>
                </n-collapse-item>
                <n-collapse-item title="查看响应详情" name="response">
                  <pre class="log-json">{{ formatJson(log.responsePayload) }}</pre>
                </n-collapse-item>
              </n-collapse>
            </div>
          </div>
        </div>

        <!-- 分页 -->
        <div v-if="pagination.itemCount > 0" class="mt-4 flex justify-between items-center">
          <div class="text-sm text-gray-500">
            �?{{ pagination.itemCount }} 条记�?          </div>
          <n-pagination
            v-model:page="pagination.page"
            :page-size="pagination.pageSize"
            :item-count="pagination.itemCount"
            show-size-picker
            :page-sizes="[10, 20, 50, 100]"
            @update:page="handlePageChange"
            @update:page-size="handlePageSizeChange"
          />
        </div>
      </n-card>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { NCard, NSelect, NButton, NTag, NCollapse, NCollapseItem, NPagination, NSpin } from 'naive-ui'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

const api = useApi()
const message = useSafeMessage()

const loading = ref(false)
const logs = ref<any[]>([])
const filterAgentType = ref<string | null>(null)
const filterSuccess = ref<boolean | null>(null)

const pagination = ref({
  page: 1,
  pageSize: 20,
  itemCount: 0
})

const agentTypeOptions = [
  { label: '内容生成', value: 'Content' },
  { label: 'Demo 上架', value: 'Demo' },
  { label: '线索处理', value: 'Lead' }
]

const successOptions = [
  { label: '成功', value: true },
  { label: '失败', value: false }
]

// 获取日志列表
const fetchLogs = async () => {
  loading.value = true
  try {
    // 注意：这里需要后端提供日志查询接�?    // 暂时使用模拟数据或直接查询数据库
    const res = await api.get('/ai/logs', {
      params: {
        agentType: filterAgentType.value,
        success: filterSuccess.value,
        page: pagination.value.page,
        pageSize: pagination.value.pageSize
      }
    }).catch(() => ({ list: [], total: 0 }))

    if (res && Array.isArray(res)) {
      logs.value = res
      pagination.value.itemCount = res.length
    } else if (res && res.list) {
      logs.value = res.list
      pagination.value.itemCount = res.total || res.list.length
    } else {
      logs.value = []
      pagination.value.itemCount = 0
    }
  } catch (e: any) {
    console.error('获取日志失败:', e)
    message.error('获取日志失败')
    logs.value = []
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.value.page = 1
  fetchLogs()
}

// 重置
const handleReset = () => {
  filterAgentType.value = null
  filterSuccess.value = null
  pagination.value.page = 1
  fetchLogs()
}

// 分页变化
const handlePageChange = (page: number) => {
  pagination.value.page = page
  fetchLogs()
}

const handlePageSizeChange = (pageSize: number) => {
  pagination.value.pageSize = pageSize
  pagination.value.page = 1
  fetchLogs()
}

// 格式化日�?const formatDate = (dateString: string) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleString('zh-CN')
}

// 格式�?JSON
const formatJson = (jsonStr: string | null) => {
  if (!jsonStr) return ''
  try {
    const obj = JSON.parse(jsonStr)
    return JSON.stringify(obj, null, 2)
  } catch {
    return jsonStr
  }
}

onMounted(() => {
  fetchLogs()
})

useHead({
  title: 'AI 智能体日�?- 后台管理',
  meta: [
    { name: 'description', content: 'AI 智能体调用日志查�? }
  ]
})
</script>

<style scoped>
.admin-ai-logs-page {
  max-width: 1400px;
  margin: 0 auto;
}

.filters-bar {
  display: flex;
  gap: var(--spacing-3);
  align-items: center;
  flex-wrap: wrap;
}

.logs-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-4);
}

.log-item {
  padding: var(--spacing-4);
  border: var(--spacing-px) solid var(--n-border-color);
  border-radius: var(--radius-md);
  background: var(--n-color);
  transition: all 0.2s;
}

.log-item:hover {
  box-shadow: var(--shadow-sm, 0 var(--spacing-0_5) var(--spacing-3) var(--shadow));
}

.log-item-error {
  border-color: var(--color-error, var(--color-danger));
  background: var(--color-error-soft, rgba(239, 68, 68, 0.05));
}

.log-header {
  margin-bottom: var(--spacing-3);
}

.log-meta {
  display: flex;
  align-items: center;
  gap: var(--spacing-3);
  flex-wrap: wrap;
}

.log-time {
  font-size: var(--text-sm);
  color: var(--n-text-color-2);
}

.log-error {
  padding: var(--spacing-3);
  background: var(--color-error-soft, rgba(239, 68, 68, 0.1));
  border-left: var(--spacing-0_5) solid var(--color-error, var(--color-danger));
  border-radius: var(--radius-sm);
  color: var(--color-error-hover, var(--color-danger-600));
  margin-bottom: var(--spacing-3);
  display: flex;
  align-items: center;
  gap: var(--spacing-2);
}

.log-details {
  margin-top: var(--spacing-3);
}

.log-json {
  background: var(--n-color);
  padding: var(--spacing-3);
  border-radius: var(--radius-sm);
  font-size: var(--text-xs);
  overflow-x: auto;
  max-height: 400px;
  overflow-y: auto;
}
</style>

