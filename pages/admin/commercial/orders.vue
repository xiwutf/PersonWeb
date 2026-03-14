<template>
  <div class="admin-page">
    <div class="page-header">
      <h1 class="page-title">支付订单管理</h1>
    </div>

    <!-- 筛选栏 -->
    <div class="filter-bar">
      <select v-model="filters.status" class="filter-input">
        <option value="">全部状态</option>
        <option value="pending">待支付</option>
        <option value="paid">已支付</option>
        <option value="failed">支付失败</option>
        <option value="refunded">已退款</option>
      </select>
      <select v-model="filters.paymentMethod" class="filter-input">
        <option value="">全部支付方式</option>
        <option value="wechat">微信支付</option>
        <option value="alipay">支付宝</option>
        <option value="stripe">Stripe</option>
      </select>
      <button class="btn-secondary" @click="loadOrders">
        <i class="fas fa-search mr-2"></i>
        搜索
      </button>
    </div>

    <!-- 订单列表 -->
    <div class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>订单ID</th>
            <th>类型</th>
            <th>用户ID</th>
            <th>金额</th>
            <th>支付方式</th>
            <th>支付状态</th>
            <th>创建时间</th>
            <th>支付时间</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in orders" :key="order.id">
            <td>{{ order.orderId }}</td>
            <td>
              <span :class="getTypeClass(order.type)">
                {{ getTypeText(order.type) }}
              </span>
            </td>
            <td>{{ order.userId }}</td>
            <td>¥{{ order.amount }}</td>
            <td>{{ getPaymentMethodText(order.paymentMethod) }}</td>
            <td>
              <span :class="getStatusClass(order.paymentStatus)">
                {{ getStatusText(order.paymentStatus) }}
              </span>
            </td>
            <td>{{ formatDate(order.createdAt) }}</td>
            <td>{{ order.paidAt ? formatDate(order.paidAt) : '-' }}</td>
            <td>
              <button class="btn-sm btn-primary" @click="viewOrder(order)">
                查看详情
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 分页 -->
    <div class="pagination">
      <button
        class="btn-secondary"
        :disabled="page === 1"
        @click="page--; loadOrders()"
      >
        上一页
      </button>
      <span class="page-info">第 {{ page }} 页，共 {{ totalPages }} 页</span>
      <button
        class="btn-secondary"
        :disabled="page >= totalPages"
        @click="page++; loadOrders()"
      >
        下一页
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const orders = ref<any[]>([])
const page = ref(1)
const pageSize = ref(20)
const totalPages = ref(1)
const filters = ref({
  status: '',
  paymentMethod: ''
})

const loadOrders = async () => {
  try {
    const params = new URLSearchParams({
      page: page.value.toString(),
      pageSize: pageSize.value.toString()
    })
    if (filters.value.status) {
      params.append('status', filters.value.status)
    }
    if (filters.value.paymentMethod) {
      params.append('paymentMethod', filters.value.paymentMethod)
    }

    const res = await api.get(`/admin/commercial/orders?${params}`)
    if (res && res.data) {
      orders.value = res.data.orders || []
      totalPages.value = res.data.totalPages || 1
    }
  } catch (error) {
    console.error('加载订单列表失败:', error)
  }
}

const getTypeClass = (type: string) => {
  return type === 'theme' ? 'type-badge type-theme' : 'type-badge type-tool'
}

const getTypeText = (type: string) => {
  return type === 'theme' ? '主题' : '工具'
}

const getPaymentMethodText = (method: string) => {
  const texts: Record<string, string> = {
    wechat: '微信支付',
    alipay: '支付宝',
    stripe: 'Stripe'
  }
  return texts[method] || method
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    pending: 'status-badge status-pending',
    paid: 'status-badge status-paid',
    failed: 'status-badge status-failed',
    refunded: 'status-badge status-refunded'
  }
  return classes[status] || 'status-badge'
}

const getStatusText = (status: string) => {
  const texts: Record<string, string> = {
    pending: '待支付',
    paid: '已支付',
    failed: '支付失败',
    refunded: '已退款'
  }
  return texts[status] || status
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleString('zh-CN')
}

const viewOrder = (order: any) => {
  // TODO: 实现查看详情功能
  console.log('查看订单:', order)
}

onMounted(() => {
  loadOrders()
})
</script>

<style scoped>
.admin-page {
  padding: 2rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-title {
  font-size: 1.5rem;
  font-weight: bold;
  color: var(--color-text-main);
}

.filter-bar {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.filter-input {
  padding: 0.5rem 0.75rem;
  border: 1px solid var(--color-border-default);
  border-radius: 4px;
  background: var(--color-bg-card);
  color: var(--color-text-main);
  font-size: 0.875rem;
}

.filter-input:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px var(--color-primary-soft);
}

.table-container {
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  background: var(--color-bg-card);
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid var(--color-border-subtle);
}

.data-table th,
.data-table td {
  padding: 0.75rem 1rem;
  text-align: left;
  border-bottom: 1px solid var(--color-border-subtle);
}

.data-table th {
  background: var(--color-bg-elevated);
  font-weight: 600;
  color: var(--color-text-main);
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.data-table td {
  color: var(--color-text-main);
  font-size: 0.875rem;
}

.data-table tbody tr:hover {
  background: var(--color-bg-elevated);
}

.type-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.875rem;
  font-weight: 500;
}

.type-theme {
  background: var(--color-primary-soft);
  color: var(--color-primary);
}

.type-tool {
  background: rgba(236, 72, 153, 0.2);
  color: var(--color-pink-500);
}

.status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.875rem;
  font-weight: 500;
}

.status-pending {
  background: rgba(251, 191, 36, 0.2);
  color: var(--color-warning);
}

.status-paid {
  background: rgba(34, 197, 94, 0.2);
  color: var(--color-success);
}

.status-failed {
  background: rgba(239, 68, 68, 0.2);
  color: var(--color-danger);
}

.status-refunded {
  background: var(--color-bg-elevated);
  color: var(--color-text-muted);
  border: 1px solid var(--color-border-subtle);
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 2rem;
}

.page-info {
  padding: 0 1rem;
}
</style>

