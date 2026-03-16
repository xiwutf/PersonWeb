<template>
  <div class="price-alert-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-left">
        <NuxtLink to="/admin/asset-management" class="back-button" title="返回资产管理">
          <i class="fas fa-arrow-left"></i>
        </NuxtLink>
        <h1 class="page-title">价格提醒管理</h1>
      </div>
      <div class="header-actions">
        <button @click="refreshPrices" class="btn-secondary" :disabled="refreshing">
          {{ refreshing ? '刷新�?..' : '刷新价格' }}
        </button>
        <button @click="handleAddClick" class="btn-primary">+ 新增提醒</button>
      </div>
    </div>

    <!-- 提醒列表 -->
    <div class="alert-list">
      <div v-if="loading" class="loading">加载�?..</div>
      <div v-else-if="!alerts || alerts.length === 0" class="empty-state">
        <p>暂无价格提醒</p>
        <button @click="handleAddClick" class="btn-primary">创建第一个提�?/button>
      </div>
      <div v-else class="alerts-grid">
        <div v-for="alert in alerts" :key="alert.id" class="alert-card" :class="{ triggered: alert.isTriggered }">
          <div class="alert-header">
            <div class="alert-title">
              <h3>{{ alert.name }}</h3>
              <span class="alert-code">{{ alert.code }}</span>
            </div>
            <div class="alert-status">
              <span v-if="alert.isTriggered" class="status-badge triggered">已触�?/span>
              <span v-else :class="['status-badge', alert.isActive ? 'active' : 'inactive']">
                {{ alert.isActive ? '启用' : '停用' }}
              </span>
            </div>
          </div>
          <div class="alert-details">
            <div class="price-info">
              <div class="price-item">
                <span class="label">目标价格�?/span>
                <span class="value target">¥{{ formatMoney(alert.targetPrice) }}</span>
              </div>
              <div class="price-item">
                <span class="label">当前价格�?/span>
                <span class="value current">¥{{ formatMoney(alert.currentPrice) }}</span>
              </div>
              <div class="price-item">
                <span class="label">提醒类型�?/span>
                <span class="value">{{ getAlertTypeText(alert.alertType) }}</span>
              </div>
            </div>
            <div v-if="alert.isTriggered && alert.triggeredAt" class="trigger-info">
              <span class="trigger-time">触发时间：{{ formatDateTime(alert.triggeredAt) }}</span>
            </div>
          </div>
          <div class="alert-actions">
            <button @click="handleEditClick(alert)" class="btn-secondary">编辑</button>
            <button @click="handleDeleteClick(alert.id)" class="btn-danger">删除</button>
          </div>
        </div>
      </div>
    </div>

    <!-- 新增/编辑模态框 -->
    <div v-if="showModal" class="modal-overlay" @click="closeModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ editingAlert ? '编辑价格提醒' : '新增价格提醒' }}</h2>
          <button @click="closeModal" class="modal-close">×</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="handleSubmit">
            <div class="form-group">
              <label>代码 <span class="required">*</span></label>
              <input
                v-model="formData.code"
                type="text"
                placeholder="请输入股�?基金代码"
                required
                maxlength="20"
              />
            </div>
            <div class="form-group">
              <label>名称</label>
              <input
                v-model="formData.name"
                type="text"
                placeholder="自动获取或手动输�?
                maxlength="100"
              />
              <button type="button" @click="handleAutoFill" class="btn-auto-fill">自动获取</button>
            </div>
            <div class="form-group">
              <label>类型 <span class="required">*</span></label>
              <select v-model="formData.type" required>
                <option value="fund">基金</option>
                <option value="stock">股票</option>
              </select>
            </div>
            <div class="form-group">
              <label>目标价格 <span class="required">*</span></label>
              <input
                v-model.number="formData.targetPrice"
                type="number"
                step="0.01"
                min="0.01"
                placeholder="请输入目标价�?
                required
              />
            </div>
            <div class="form-group">
              <label>提醒类型 <span class="required">*</span></label>
              <select v-model="formData.alertType" required>
                <option value="above">高于目标价格</option>
                <option value="below">低于目标价格</option>
                <option value="both">达到目标价格（上下均可）</option>
              </select>
            </div>
            <div class="form-group">
              <label>
                <input
                  v-model="formData.isActive"
                  type="checkbox"
                />
                启用提醒
              </label>
            </div>
            <div class="form-group">
              <label>备注</label>
              <textarea
                v-model="formData.notes"
                rows="3"
                placeholder="可选备注信�?
              />
            </div>
            <div class="form-actions">
              <button type="button" @click="closeModal" class="btn-secondary">取消</button>
              <button type="submit" class="btn-primary" :disabled="submitting">
                {{ submitting ? '保存�?..' : '保存' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useApi } from '~/composables/useApi'
import { useNotification } from '~/composables/useToast'

// 页面元数�?definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

interface PriceAlert {
  id: number
  code: string
  name: string
  type: string
  targetPrice: number
  alertType: string
  currentPrice: number
  isTriggered: boolean
  triggeredAt: string | null
  isActive: boolean
  notificationSent: boolean
  notes: string | null
  createdAt: string
  updatedAt: string
}

const api = useApi()
const { success, error, warning, info } = useNotification()

const loading = ref(false)
const refreshing = ref(false)
const alerts = ref<PriceAlert[]>([])
const showModal = ref(false)
const editingAlert = ref<PriceAlert | null>(null)
const submitting = ref(false)

const formData = ref({
  code: '',
  name: '',
  type: 'fund',
  targetPrice: 0,
  alertType: 'above',
  isActive: true,
  notes: ''
})

// 加载提醒列表
const loadAlerts = async () => {
  // 只在客户端执�?  if (typeof window === 'undefined') return
  
  try {
    loading.value = true
    const data = await api.get<PriceAlert[]>('/PriceAlert')
    alerts.value = Array.isArray(data) ? data : []
  } catch (err: any) {
    console.error('加载价格提醒失败:', err)
    error(err.message || '加载失败')
    alerts.value = [] // 确保始终是数�?  } finally {
    loading.value = false
  }
}

// 刷新价格
const refreshPrices = async () => {
  try {
    refreshing.value = true
    await api.post('/PriceAlert/refresh-prices', {})
    success('价格刷新成功')
    loadAlerts()
  } catch (err: any) {
    console.error('刷新价格失败:', err)
    error(err.message || '刷新失败')
  } finally {
    refreshing.value = false
  }
}

// 格式化金�?const formatMoney = (value: number) => {
  return value.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

// 格式化日期时�?const formatDateTime = (date: string) => {
  return new Date(date).toLocaleString('zh-CN')
}

// 获取提醒类型文本
const getAlertTypeText = (type: string) => {
  const map: Record<string, string> = {
    above: '高于目标价格',
    below: '低于目标价格',
    both: '达到目标价格'
  }
  return map[type] || type
}

// 处理新增点击
const handleAddClick = () => {
  editingAlert.value = null
  formData.value = {
    code: '',
    name: '',
    type: 'fund',
    targetPrice: 0,
    alertType: 'above',
    isActive: true,
    notes: ''
  }
  showModal.value = true
}

// 处理编辑点击
const handleEditClick = (alert: PriceAlert) => {
  editingAlert.value = alert
  formData.value = {
    code: alert.code,
    name: alert.name,
    type: alert.type,
    targetPrice: alert.targetPrice,
    alertType: alert.alertType,
    isActive: alert.isActive,
    notes: alert.notes || ''
  }
  showModal.value = true
}

// 处理删除点击
const handleDeleteClick = async (id: number) => {
  if (!confirm('确定要删除这个价格提醒吗�?)) {
    return
  }

  try {
    await api.delete(`/PriceAlert/${id}`)
    success('删除成功')
    loadAlerts()
  } catch (err: any) {
    console.error('删除失败:', err)
    error(err.message || '删除失败')
  }
}

// 自动填充名称
const handleAutoFill = async () => {
  if (!formData.value.code || !formData.value.type) {
    warning('请先输入代码和类�?)
    return
  }

  try {
    const data = await api.get<{ Name: string; CurrentPrice: number }>(
      `/Investment/auto-fill?code=${formData.value.code}&type=${formData.value.type}`
    )
    if (data && data.Name) {
      formData.value.name = data.Name
      if (data.CurrentPrice > 0) {
        formData.value.targetPrice = data.CurrentPrice
      }
      success('自动获取成功')
    } else {
      warning('无法自动获取，请手动输入')
    }
  } catch (err: any) {
    console.error('自动获取失败:', err)
    warning('自动获取失败，请手动输入')
  }
}

// 提交表单
const handleSubmit = async () => {
  try {
    submitting.value = true

    const payload = {
      code: formData.value.code,
      name: formData.value.name,
      type: formData.value.type,
      targetPrice: formData.value.targetPrice,
      alertType: formData.value.alertType,
      isActive: formData.value.isActive,
      notes: formData.value.notes || undefined
    }

    if (editingAlert.value) {
      await api.put(`/PriceAlert/${editingAlert.value.id}`, payload)
      success('更新成功')
    } else {
      await api.post('/PriceAlert', payload)
      success('创建成功')
    }

    closeModal()
    loadAlerts()
  } catch (err: any) {
    console.error('保存失败:', err)
    error(err.message || '保存失败')
  } finally {
    submitting.value = false
  }
}

// 关闭模态框
const closeModal = () => {
  showModal.value = false
  editingAlert.value = null
}

// 页面加载时获取数�?onMounted(() => {
  loadAlerts()
})
</script>

<style scoped>
.price-alert-page {
  padding: 24px;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.header-left {
  display: flex;
  align-items: center;
}

.back-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  margin-right: var(--spacing-md);
  color: var(--color-text-sec);
  text-decoration: none;
  border-radius: var(--radius-md);
  transition: all 0.2s;
}

.back-button:hover {
  background: var(--color-bg-body);
  color: var(--color-primary);
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.btn-primary, .btn-secondary {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.btn-primary {
  background: var(--color-primary);
  color: var(--color-bg-light, white);
}

.btn-primary:hover {
  background: var(--color-blue-400);
}

.btn-secondary {
  background: var(--color-border-subtle);
  color: var(--color-text-secondary);
}

.btn-secondary:hover:not(:disabled) {
  background: var(--color-border-default);
}

.btn-secondary:disabled {
  background: var(--color-border-default);
  cursor: not-allowed;
}

.loading, .empty-state {
  text-align: center;
  padding: 48px;
  color: var(--color-text-quaternary);
}

.alerts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 24px;
}

.alert-card {
  border: 1px solid var(--color-border-subtle);
  border-radius: 8px;
  padding: 20px;
  background: var(--color-bg-light, white);
}

.alert-card.triggered {
  border-color: var(--color-success-icon);
  background: var(--color-success-bg);
}

.alert-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 16px;
  padding-bottom: 16px;
  border-bottom: 1px solid var(--color-border-subtle);
}

.alert-title h3 {
  margin: 0 0 4px 0;
  font-size: 18px;
  font-weight: 600;
}

.alert-code {
  color: var(--color-text-quaternary);
  font-size: 12px;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.status-badge.active {
  background: var(--color-success-bg);
  color: var(--color-success-icon);
  border: 1px solid var(--color-success-border);
}

.status-badge.inactive {
  background: var(--color-warning-bg);
  color: var(--color-warning-icon);
  border: 1px solid var(--color-warning-border);
}

.status-badge.triggered {
  background: var(--color-error-bg);
  color: var(--color-error-icon);
  border: 1px solid var(--color-error-border);
}

.alert-details {
  margin-bottom: 16px;
}

.price-info {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.price-item {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
}

.price-item .label {
  color: var(--color-gray-500);
}

.price-item .value {
  font-weight: 500;
}

.price-item .value.target {
  color: var(--color-primary);
}

.price-item .value.current {
  color: var(--color-success-icon);
}

.trigger-info {
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px solid var(--color-border-subtle);
}

.trigger-time {
  font-size: 12px;
  color: var(--color-text-quaternary);
}

.alert-actions {
  display: flex;
  gap: 8px;
}

.btn-danger {
  padding: 6px 12px;
  background: var(--color-error-icon);
  color: var(--color-bg-light, white);
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
}

.btn-danger:hover {
  background: var(--color-error-hover);
}

/* 模态框样式（复用定投计划的样式�?*/
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: var(--overlay-color, rgba(0, 0, 0, 0.5));
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: var(--color-bg-light, white);
  border-radius: 8px;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid var(--color-border-subtle);
}

.modal-header h2 {
  margin: 0;
  font-size: 20px;
}

.modal-close {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: var(--color-text-quaternary);
}

.modal-body {
  padding: 20px;
}

.form-group {
  margin-bottom: 16px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  font-size: 14px;
}

.form-group .required {
  color: var(--color-error-icon);
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid var(--color-border-default);
  border-radius: 4px;
  font-size: 14px;
}

.form-group input[type="checkbox"] {
  width: auto;
  margin-right: 8px;
}

.btn-auto-fill {
  margin-top: 8px;
  padding: 4px 12px;
  background: var(--color-border-subtle);
  border: 1px solid var(--color-border-default);
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 24px;
}
</style>
