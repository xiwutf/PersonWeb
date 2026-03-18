<template>
  <div class="dca-plan-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-left">
        <NuxtLink to="/admin/asset-management" class="back-button" title="返回资产管理">
          <i class="fas fa-arrow-left"></i>
        </NuxtLink>
        <h1 class="page-title">定投计划管理</h1>
      </div>
      <button @click="handleAddClick" class="btn-primary">+ 新增定投计划</button>
    </div>

    <!-- 定投计划列表 -->
    <div class="plan-list">
      <div v-if="loading" class="loading">加载中...</div>
      <div v-else-if="!plans || plans.length === 0" class="empty-state">
        <p>暂无定投计划</p>
        <button @click="handleAddClick" class="btn-primary">创建第一个定投计划</button>
      </div>
      <div v-else class="plans-grid">
        <div v-for="plan in plans" :key="plan.id" class="plan-card">
          <div class="plan-header">
            <div class="plan-title">
              <h3>{{ plan.name }}</h3>
              <span class="plan-code">{{ plan.code }}</span>
            </div>
            <div class="plan-status">
              <span :class="['status-badge', plan.isActive ? 'active' : 'inactive']">
                {{ plan.isActive ? '启用' : '停用' }}
              </span>
            </div>
          </div>
          <div class="plan-details">
            <div class="detail-item">
              <span class="label">类型：</span>
              <span class="value">{{ plan.type === 'fund' ? '基金' : '股票' }}</span>
            </div>
            <div class="detail-item">
              <span class="label">定投金额：</span>
              <span class="value">¥{{ formatMoney(plan.amount) }}</span>
            </div>
            <div class="detail-item">
              <span class="label">频率：</span>
              <span class="value">{{ getFrequencyText(plan.frequency) }}</span>
            </div>
            <div class="detail-item">
              <span class="label">执行次数：</span>
              <span class="value">{{ plan.totalExecutions }} 次</span>
            </div>
            <div class="detail-item">
              <span class="label">累计投入：</span>
              <span class="value">¥{{ formatMoney(plan.totalInvested) }}</span>
            </div>
            <div class="detail-item">
              <span class="label">下次执行：</span>
              <span class="value">{{ formatDate(plan.nextExecutionDate) }}</span>
            </div>
          </div>
          <div class="plan-actions">
            <button @click="handleExecuteClick(plan.id)" class="btn-action" :disabled="!plan.isActive">
              立即执行
            </button>
            <button @click="handleEditClick(plan)" class="btn-secondary">编辑</button>
            <button @click="handleDeleteClick(plan.id)" class="btn-danger">删除</button>
          </div>
        </div>
      </div>
    </div>

    <!-- 新增/编辑模态框 -->
    <div v-if="showModal" class="modal-overlay" @click="closeModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ editingPlan ? '编辑定投计划' : '新增定投计划' }}</h2>
          <button @click="closeModal" class="modal-close">×</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="handleSubmit">
            <div class="form-group">
              <label>代码 <span class="required">*</span></label>
              <input
                v-model="formData.code"
                type="text"
                placeholder="请输入股票/基金代码"
                required
                maxlength="20"
              />
            </div>
            <div class="form-group">
              <label>名称</label>
              <input
                v-model="formData.name"
                type="text"
                placeholder="自动获取或手动输入"
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
              <label>定投金额 <span class="required">*</span></label>
              <input
                v-model.number="formData.amount"
                type="number"
                step="0.01"
                min="0.01"
                placeholder="请输入定投金额"
                required
              />
            </div>
            <div class="form-group">
              <label>频率 <span class="required">*</span></label>
              <select v-model="formData.frequency" required>
                <option value="daily">每日</option>
                <option value="weekly">每周</option>
                <option value="monthly">每月</option>
                <option value="quarterly">每季度</option>
              </select>
            </div>
            <div class="form-group">
              <label>开始日期</label>
              <input
                v-model="formData.startDate"
                type="date"
              />
            </div>
            <div class="form-group">
              <label>结束日期（可选）</label>
              <input
                v-model="formData.endDate"
                type="date"
              />
            </div>
            <div class="form-group">
              <label>
                <input
                  v-model="formData.isActive"
                  type="checkbox"
                />
                启用定投计划
              </label>
            </div>
            <div class="form-group">
              <label>备注</label>
              <textarea
                v-model="formData.notes"
                rows="3"
                placeholder="可选备注信息"
              />
            </div>
            <div class="form-actions">
              <button type="button" @click="closeModal" class="btn-secondary">取消</button>
              <button type="submit" class="btn-primary" :disabled="submitting">
                {{ submitting ? '保存中...' : '保存' }}
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

// 页面元数据
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

interface DcaPlan {
  id: number
  code: string
  name: string
  type: string
  amount: number
  frequency: string
  nextExecutionDate: string | null
  lastExecutionDate: string | null
  totalExecutions: number
  totalInvested: number
  isActive: boolean
  startDate: string
  endDate: string | null
  notes: string | null
  createdAt: string
  updatedAt: string
}

const api = useApi()
const { success, error, warning, info } = useNotification()

const loading = ref(false)
const plans = ref<DcaPlan[]>([])
const showModal = ref(false)
const editingPlan = ref<DcaPlan | null>(null)
const submitting = ref(false)

const formData = ref({
  code: '',
  name: '',
  type: 'fund',
  amount: 0,
  frequency: 'monthly',
  startDate: new Date().toISOString().split('T')[0],
  endDate: '',
  isActive: true,
  notes: ''
})

// 加载定投计划列表
const loadPlans = async () => {
  // 只在客户端执行
  if (typeof window === 'undefined') return
  
  try {
    loading.value = true
    const data = await api.get<DcaPlan[]>('/DcaPlan')
    plans.value = Array.isArray(data) ? data : []
  } catch (err: any) {
    console.error('加载定投计划失败:', err)
    error(err.message || '加载失败')
    plans.value = [] // 确保始终是数组
  } finally {
    loading.value = false
  }
}

// 格式化金额
const formatMoney = (value: number) => {
  return value.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

// 格式化日期
const formatDate = (date: string | null) => {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('zh-CN')
}

// 获取频率文本
const getFrequencyText = (frequency: string) => {
  const map: Record<string, string> = {
    daily: '每日',
    weekly: '每周',
    monthly: '每月',
    quarterly: '每季度'
  }
  return map[frequency] || frequency
}

// 处理新增点击
const handleAddClick = () => {
  editingPlan.value = null
  formData.value = {
    code: '',
    name: '',
    type: 'fund',
    amount: 0,
    frequency: 'monthly',
    startDate: new Date().toISOString().split('T')[0],
    endDate: '',
    isActive: true,
    notes: ''
  }
  showModal.value = true
}

// 处理编辑点击
const handleEditClick = (plan: DcaPlan) => {
  editingPlan.value = plan
  formData.value = {
    code: plan.code,
    name: plan.name,
    type: plan.type,
    amount: plan.amount,
    frequency: plan.frequency,
    startDate: plan.startDate ? plan.startDate.split('T')[0] : new Date().toISOString().split('T')[0],
    endDate: plan.endDate ? plan.endDate.split('T')[0] : '',
    isActive: plan.isActive,
    notes: plan.notes || ''
  }
  showModal.value = true
}

// 处理删除点击
const handleDeleteClick = async (id: number) => {
  if (!confirm('确定要删除这个定投计划吗？')) {
    return
  }

  try {
    await api.delete(`/DcaPlan/${id}`)
    success('删除成功')
    loadPlans()
  } catch (err: any) {
    console.error('删除失败:', err)
    error(err.message || '删除失败')
  }
}

// 处理执行点击
const handleExecuteClick = async (id: number) => {
  if (!confirm('确定要立即执行这个定投计划吗？')) {
    return
  }

  try {
    await api.post(`/DcaPlan/${id}/execute`, {})
    success('执行成功')
    loadPlans()
  } catch (err: any) {
    console.error('执行失败:', err)
    error(err.message || '执行失败')
  }
}

// 自动填充名称
const handleAutoFill = async () => {
  if (!formData.value.code || !formData.value.type) {
    warning('请先输入代码和类型')
    return
  }

  try {
    const data = await api.get<{ Name: string; CurrentPrice: number }>(
      `/Investment/auto-fill?code=${formData.value.code}&type=${formData.value.type}`
    )
    if (data && data.Name) {
      formData.value.name = data.Name
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
      amount: formData.value.amount,
      frequency: formData.value.frequency,
      startDate: formData.value.startDate || undefined,
      endDate: formData.value.endDate || undefined,
      isActive: formData.value.isActive,
      notes: formData.value.notes || undefined
    }

    if (editingPlan.value) {
      await api.put(`/DcaPlan/${editingPlan.value.id}`, payload)
      success('更新成功')
    } else {
      await api.post('/DcaPlan', payload)
      success('创建成功')
    }

    closeModal()
    loadPlans()
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
  editingPlan.value = null
}

// 页面加载时获取数据
onMounted(() => {
  loadPlans()
})
</script>

<style scoped>
.dca-plan-page {
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

.btn-primary {
  padding: 8px 16px;
  background: #1890ff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.btn-primary:hover {
  background: #40a9ff;
}

.btn-primary:disabled {
  background: #d9d9d9;
  cursor: not-allowed;
}

.loading, .empty-state {
  text-align: center;
  padding: 48px;
  color: #999;
}

.plans-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 24px;
}

.plan-card {
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  padding: 20px;
  background: white;
}

.plan-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 16px;
  padding-bottom: 16px;
  border-bottom: 1px solid #f0f0f0;
}

.plan-title h3 {
  margin: 0 0 4px 0;
  font-size: 18px;
  font-weight: 600;
}

.plan-code {
  color: #999;
  font-size: 12px;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.status-badge.active {
  background: #f6ffed;
  color: #52c41a;
  border: 1px solid #b7eb8f;
}

.status-badge.inactive {
  background: #fff2e8;
  color: #fa8c16;
  border: 1px solid #ffd591;
}

.plan-details {
  margin-bottom: 16px;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.detail-item .label {
  color: #666;
}

.detail-item .value {
  font-weight: 500;
}

.plan-actions {
  display: flex;
  gap: 8px;
}

.btn-action, .btn-secondary, .btn-danger {
  padding: 6px 12px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
}

.btn-action {
  background: #1890ff;
  color: white;
}

.btn-action:hover:not(:disabled) {
  background: #40a9ff;
}

.btn-action:disabled {
  background: #d9d9d9;
  cursor: not-allowed;
}

.btn-secondary {
  background: #f0f0f0;
  color: #333;
}

.btn-secondary:hover {
  background: #e0e0e0;
}

.btn-danger {
  background: #ff4d4f;
  color: white;
}

.btn-danger:hover {
  background: #ff7875;
}

/* 模态框样式 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
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
  border-bottom: 1px solid #e8e8e8;
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
  color: #999;
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
  color: #ff4d4f;
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #d9d9d9;
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
  background: #f0f0f0;
  border: 1px solid #d9d9d9;
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
