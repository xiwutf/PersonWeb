<template>
  <ClientOnly>
    <div class="admin-orders-page">
      <div class="page-header">
        <h1 class="page-title">????</h1>
      </div>

    <!-- ??? -->
    <div class="filters-bar">
      <n-input
        v-model:value="searchKeyword"
        placeholder="???????????????????.."
        clearable
        style="width: 300px;"
        @keyup.enter="handleSearch"
      >
        <template #prefix>
          <i class="fas fa-search"></i>
        </template>
      </n-input>
      <n-select
        v-model:value="filterStatus"
        placeholder="????"
        clearable
        style="width: 150px;"
        :options="statusOptions"
      />
      <n-button type="primary" @click="handleSearch">??</n-button>
      <n-button quaternary @click="handleReset">??</n-button>
    </div>

    <!-- ???? -->
    <div class="table-container">
      <div v-if="loading" class="table-loading">????..</div>
      <div v-else-if="orders.length === 0" class="table-empty">??????</div>
      <table v-else class="data-table">
        <thead class="table-header">
          <tr>
            <th>????</th>
            <th>????</th>
            <th>????</th>
            <th>????</th>
            <th>??</th>
            <th>??</th>
            <th>????</th>
            <th>??</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="order in orders" :key="order.id" class="table-row">
            <td class="table-cell font-mono">{{ order.orderNo }}</td>
            <td class="table-cell">{{ order.productNameSnapshot }}</td>
            <td class="table-cell">{{ order.customerName }}</td>
            <td class="table-cell">
              <div class="text-sm">
                <div v-if="order.customerPhone">?? {{ order.customerPhone }}</div>
                <div v-if="order.customerWeChat">?? {{ order.customerWeChat }}</div>
                <div v-if="order.customerEmail">?? {{ order.customerEmail }}</div>
              </div>
            </td>
            <td class="table-cell">
              {{ order.totalAmount ? `?${order.totalAmount.toFixed(2)}` : '??' }}
            </td>
            <td class="table-cell">
              <span :class="getStatusTagClass(order.status)" class="tag">
                {{ getStatusText(order.status) }}
              </span>
            </td>
            <td class="table-cell">{{ formatDate(order.createdAt) }}</td>
            <td class="table-cell">
              <div class="action-buttons">
                <button @click="handleViewDetail(order)" class="btn-link btn-link-blue">??</button>
                <button @click="handleEditStatus(order)" class="btn-link btn-link-green">????</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- ?? -->
      <div v-if="pagination.itemCount > 0" class="table-pagination">
        <div class="pagination-info">?{{ pagination.itemCount }} ???</div>
        <div class="pagination-controls">
          <select v-model="pagination.pageSize" @change="handlePageSizeChange" class="pagination-select">
            <option :value="10">10/?</option>
            <option :value="20">20/?</option>
            <option :value="50">50/?</option>
          </select>
          <div class="pagination-buttons">
            <button @click="pagination.page = 1; fetchOrders()" :disabled="pagination.page === 1" class="pagination-btn">??</button>
            <button @click="pagination.page--; fetchOrders()" :disabled="pagination.page === 1" class="pagination-btn">???</button>
            <span class="pagination-info">{{ pagination.page }} / {{ pagination.totalPages }}</span>
            <button @click="pagination.page++; fetchOrders()" :disabled="pagination.page >= pagination.totalPages" class="pagination-btn">???</button>
            <button @click="pagination.page = pagination.totalPages; fetchOrders()" :disabled="pagination.page >= pagination.totalPages" class="pagination-btn">??</button>
          </div>
        </div>
      </div>
    </div>

    <!-- ??/???? -->
    <n-modal v-model:show="showDetailModal" preset="card" title="????" style="width: 800px;">
      <div v-if="currentOrder" class="order-detail">
        <n-descriptions :column="2" bordered>
          <n-descriptions-item label="????">{{ currentOrder.orderNo }}</n-descriptions-item>
          <n-descriptions-item label="????">{{ currentOrder.productNameSnapshot }}</n-descriptions-item>
          <n-descriptions-item label="????">{{ currentOrder.customerName }}</n-descriptions-item>
          <n-descriptions-item label="????>{{ currentOrder.customerPhone || '-' }}</n-descriptions-item>
          <n-descriptions-item label="????>{{ currentOrder.customerWeChat || '-' }}</n-descriptions-item>
          <n-descriptions-item label="??">{{ currentOrder.customerEmail || '-' }}</n-descriptions-item>
          <n-descriptions-item label="??">{{ currentOrder.quantity }}</n-descriptions-item>
          <n-descriptions-item label="??">{{ currentOrder.totalAmount ? `?${currentOrder.totalAmount.toFixed(2)}` : '??' }}</n-descriptions-item>
          <n-descriptions-item label="??">
            <n-select v-model:value="editForm.status" :options="statusOptions" />
          </n-descriptions-item>
          <n-descriptions-item label="????">{{ formatDate(currentOrder.createdAt) }}</n-descriptions-item>
          <n-descriptions-item label="??" :span="2">
            <div class="whitespace-pre-line">{{ currentOrder.remark || '-' }}</div>
          </n-descriptions-item>
          <n-descriptions-item label="????" :span="2">
            <n-input
              v-model:value="editForm.internalNote"
              type="textarea"
              :rows="3"
              placeholder="????????"
            />
          </n-descriptions-item>
        </n-descriptions>

        <div class="mt-4 flex justify-end gap-2">
          <n-button quaternary @click="showDetailModal = false">??</n-button>
          <n-button type="primary" @click="handleSaveStatus">??</n-button>
        </div>
      </div>
    </n-modal>
    </div>
    <template #fallback>
      <div class="admin-orders-page">
        <div class="page-header">
          <h1 class="page-title">????</h1>
        </div>
        <div class="table-loading">????..</div>
      </div>
    </template>
  </ClientOnly>
</template>

<script setup lang="ts">
import { NInput, NSelect, NButton, NModal, NDescriptions, NDescriptionsItem } from 'naive-ui'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // ?? SSR ??? Naive UI
})

const api = useApi()
const message = useSafeMessage()

const loading = ref(false)
const orders = ref<any[]>([])
const searchKeyword = ref('')
const filterStatus = ref<number | null>(null)

const pagination = ref({
  page: 1,
  pageSize: 20,
  itemCount: 0,
  totalPages: 0
})

const statusOptions = [
  { label: '???', value: 0 },
  { label: '???', value: 1 },
  { label: '???', value: 2 },
  { label: '???', value: 3 }
]

const showDetailModal = ref(false)
const currentOrder = ref<any>(null)
const editForm = ref({
  status: 0,
  internalNote: ''
})

// ??????
const fetchOrders = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/admin/orders', {
      params: {
        status: filterStatus.value,
        keyword: searchKeyword.value,
        page: pagination.value.page,
        pageSize: pagination.value.pageSize
      }
    })

    console.log('????????:', res)
    
    // useApi ?????????????? data ??
    // ???????{ code: 0, data: { Total: 0, List: [], TotalPages: 0, ... } }
    // useApi ????res ?? { Total: 0, List: [], TotalPages: 0, ... }
    if (res) {
      // ?????? PascalCase?Total, List, TotalPages
      // ??????????PascalCase ??camelCase
      const list = res.List || res.list || []
      const total = res.Total || res.total || 0
      const totalPages = res.TotalPages || res.totalPages || 0
      
      orders.value = Array.isArray(list) ? list : []
      pagination.value.itemCount = total
      pagination.value.totalPages = totalPages
      
      console.log('????????:', {
        count: orders.value.length,
        itemCount: pagination.value.itemCount,
        totalPages: pagination.value.totalPages
      })
    } else {
      orders.value = []
      pagination.value.itemCount = 0
      pagination.value.totalPages = 0
    }
  } catch (e: any) {
    console.error('????????:', e)
    message.error('????????')
  } finally {
    loading.value = false
  }
}

// ??
const handleSearch = () => {
  pagination.value.page = 1
  fetchOrders()
}

// ??
const handleReset = () => {
  searchKeyword.value = ''
  filterStatus.value = null
  pagination.value.page = 1
  fetchOrders()
}

// ????
const handleViewDetail = async (order: any) => {
  try {
    const res = await api.get<any>(`/admin/orders/${order.id}`)
    console.log('????????:', res)
    
    // useApi ?????????????? data ??
    if (res && (res.id || res.orderNo)) {
      currentOrder.value = res
      editForm.value = {
        status: res.status,
        internalNote: res.internalNote || ''
      }
      showDetailModal.value = true
    } else {
      message.error('????????')
    }
  } catch (e: any) {
    console.error('????????:', e)
    message.error(e.response?.data?.message || e.message || '????????')
  }
}

// ??????????????
const handleEditStatus = (order: any) => {
  handleViewDetail(order)
}

// ??????
const handleSaveStatus = async () => {
  if (!currentOrder.value) return

  try {
    // useApi ?? data?????????
      await api.put<any>(`/admin/orders/${currentOrder.value.id}/status`, {
      status: editForm.value.status,
      internalNote: editForm.value.internalNote
    })

    // ????????????????    message.success('????')
    showDetailModal.value = false
    fetchOrders()
  } catch (e: any) {
    console.error('?????????', e)
    // ??????????    const errorMessage = e.response?.data?.message || e.message || '????'
    message.error(errorMessage)
  }
}

// ??????
const handlePageSizeChange = () => {
  pagination.value.page = 1
  fetchOrders()
}

// ??????
const getStatusText = (status: number): string => {
  const statusMap: Record<number, string> = {
    0: '???',
    1: '???',
    2: '???',
    3: '???'
  }
  return statusMap[status] || '??'
}

// ???????
const getStatusTagClass = (status: number): string => {
  const classMap: Record<number, string> = {
    0: 'tag tag-warning',
    1: 'tag tag-info',
    2: 'tag tag-success',
    3: 'tag tag-default'
  }
  return classMap[status] || 'tag tag-default'
}

// ?????
const formatDate = (dateString: string) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleString('zh-CN')
}

onMounted(() => {
  fetchOrders()
})

// ??????
useHead({
  title: '???? - ????',
  meta: [
    { name: 'description', content: '??????' }
  ]
})
</script>

<style scoped>
.admin-orders-page {
  padding: 24px;
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
  color: var(--color-text-main);
}

.filters-bar {
  display: flex;
  gap: 12px;
  margin-bottom: 24px;
  flex-wrap: wrap;
}

.table-container {
  background: var(--color-bg-card);
  border-radius: 8px;
  overflow: hidden;
}

.table-loading,
.table-empty {
  padding: 40px;
  text-align: center;
  color: var(--color-text-muted);
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.table-header {
  background: var(--color-bg-elevated);
}

.table-header th {
  padding: 12px;
  text-align: left;
  font-weight: 600;
  color: var(--color-text-main);
  border-bottom: 1px solid var(--color-border-default);
}

.table-body .table-row:hover {
  background: var(--color-bg-elevated);
}

.table-cell {
  padding: 12px;
  border-bottom: 1px solid var(--color-border-default);
  color: var(--color-text-main);
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.btn-link {
  padding: 4px 12px;
  border: none;
  background: transparent;
  cursor: pointer;
  border-radius: 4px;
  transition: all 0.2s;
}

.btn-link-blue {
  color: var(--color-primary);
}

.btn-link-blue:hover {
  background: var(--color-primary-soft);
}

.btn-link-green {
  color: var(--color-success);
}

.btn-link-green:hover {
  background: var(--color-success);
  opacity: 0.1;
}

.tag {
  display: inline-block;
  padding: 4px 10px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  line-height: 1.4;
}

.tag-warning {
  background: rgba(251, 191, 36, 0.2);
  color: var(--color-warning);
  border: 1px solid rgba(251, 191, 36, 0.3);
}

.tag-info {
  background: rgba(59, 130, 246, 0.2);
  color: var(--color-primary);
  border: 1px solid var(--theme-primary);
}

.tag-success {
  background: rgba(34, 197, 94, 0.2);
  color: var(--color-success);
  border: 1px solid rgba(34, 197, 94, 0.3);
}

.tag-default {
  background: rgba(107, 114, 128, 0.2);
  color: var(--color-text-sec);
  border: 1px solid rgba(107, 114, 128, 0.3);
}

.table-pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  border-top: 1px solid var(--color-border-default);
}

.pagination-controls {
  display: flex;
  gap: 12px;
  align-items: center;
}

.pagination-select {
  padding: 4px 8px;
  border: 1px solid var(--color-border-default);
  border-radius: 4px;
}

.pagination-buttons {
  display: flex;
  gap: 8px;
  align-items: center;
}

.pagination-btn {
  padding: 4px 12px;
  border: 1px solid var(--color-border-default);
  background: var(--color-bg-card);
  border-radius: 4px;
  cursor: pointer;
}

.pagination-btn:hover:not(:disabled) {
  background: var(--color-bg-elevated);
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.order-detail {
  padding: 16px 0;
}
</style>

