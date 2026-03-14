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
      <button @click="handleAddClick" class="asset-management-btn-primary">+ 新增定投计划</button>
    </div>

    <!-- 定投计划列表 -->
    <div class="plan-list">
      <div v-if="loading" class="asset-management-loading">加载中...</div>
      <div v-else-if="!plans || plans.length === 0" class="asset-management-empty-state">
        <p>暂无定投计划</p>
        <button @click="handleAddClick" class="asset-management-btn-primary">创建第一个定投计划</button>
      </div>
      <div v-else class="asset-management-assets-grid">
        <div v-for="plan in plans" :key="plan.id" class="asset-management-card">
          <div class="asset-management-card-header">
            <div>
              <h3 class="asset-management-card-title">{{ plan.name }}</h3>
              <span style="font-size: 12px; color: var(--color-text-sec);">{{ plan.code }}</span>
            </div>
            <div>
              <span :class="['asset-management-badge', plan.isActive ? 'asset-management-badge-success' : 'asset-management-badge-warning']">
                {{ plan.isActive ? '启用' : '停用' }}
              </span>
            </div>
          </div>
          <div class="asset-management-asset-details">
            <div class="asset-management-detail-item">
              <span class="asset-management-detail-label">类型：</span>
              <span class="asset-management-detail-value">{{ plan.type === 'fund' ? '基金' : '股票' }}</span>
            </div>
            <div class="asset-management-detail-item">
              <span class="asset-management-detail-label">定投金额：</span>
              <span class="asset-management-detail-value">¥{{ formatMoney(plan.amount) }}</span>
            </div>
            <div class="asset-management-detail-item">
              <span class="asset-management-detail-label">频率：</span>
              <span class="asset-management-detail-value">{{ getFrequencyText(plan.frequency) }}</span>
            </div>
            <div class="asset-management-detail-item">
              <span class="asset-management-detail-label">执行次数：</span>
              <span class="asset-management-detail-value">{{ plan.totalExecutions }} 次</span>
            </div>
            <div class="asset-management-detail-item">
              <span class="asset-management-detail-label">累计投入：</span>
              <span class="asset-management-detail-value">¥{{ formatMoney(plan.totalInvested) }}</span>
            </div>
            <div class="asset-management-detail-item">
              <span class="asset-management-detail-label">下次执行：</span>
              <span class="asset-management-detail-value">{{ formatDate(plan.nextExecutionDate) }}</span>
            </div>
          </div>
          <div class="asset-management-asset-actions">
            <button @click="handleExecuteClick(plan.id)" class="asset-management-btn-primary" :disabled="!plan.isActive">
              立即执行
            </button>
            <button @click="handleEditClick(plan)" class="asset-management-btn-secondary">编辑</button>
            <button @click="handleDeleteClick(plan.id)" class="asset-management-btn-danger">删除</button>
          </div>
        </div>
      </div>
    </div>

    <!-- 新增/编辑模态框 -->
    <div v-if="showModal" class="asset-management-modal-overlay" @click="closeModal">
      <div class="asset-management-modal-content" @click.stop>
        <div class="asset-management-modal-header">
          <h2>{{ editingPlan ? '编辑定投计划' : '新增定投计划' }}</h2>
          <button @click="closeModal" class="asset-management-modal-close">×</button>
        </div>
        <div class="asset-management-modal-body">
          <form @submit.prevent="handleSubmit">
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">代码 <span class="required">*</span></label>
              <input
                v-model="formData.code"
                type="text"
                class="asset-management-form-input"
                placeholder="请输入股票/基金代码"
                required
                maxlength="20"
              />
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">名称</label>
              <input
                v-model="formData.name"
                type="text"
                class="asset-management-form-input"
                placeholder="自动获取或手动输入"
                maxlength="100"
              />
              <button type="button" @click="handleAutoFill" class="btn-auto-fill">自动获取</button>
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">类型 <span class="required">*</span></label>
              <select v-model="formData.type" class="asset-management-form-select" required>
                <option value="fund">基金</option>
                <option value="stock">股票</option>
              </select>
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">定投金额 <span class="required">*</span></label>
              <input
                v-model.number="formData.amount"
                type="number"
                class="asset-management-form-input"
                step="0.01"
                min="0.01"
                placeholder="请输入定投金额"
                required
              />
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">频率 <span class="required">*</span></label>
              <select v-model="formData.frequency" class="asset-management-form-select" required>
                <option value="daily">每日</option>
                <option value="weekly">每周</option>
                <option value="monthly">每月</option>
                <option value="quarterly">每季度</option>
              </select>
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">开始日期</label>
              <input
                v-model="formData.startDate"
                type="date"
                class="asset-management-form-input"
              />
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">结束日期（可选）</label>
              <input
                v-model="formData.endDate"
                type="date"
                class="asset-management-form-input"
              />
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">
                <input
                  v-model="formData.isActive"
                  type="checkbox"
                />
                启用定投计划
              </label>
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">备注</label>
              <textarea
                v-model="formData.notes"
                class="asset-management-form-textarea"
                rows="3"
                placeholder="可选备注信息"
              />
            </div>
            <div class="asset-management-form-actions">
              <button type="button" @click="closeModal" class="asset-management-btn-secondary">取消</button>
              <button type="submit" class="asset-management-btn-primary" :disabled="submitting">
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
    // 确保代码是6位数字
    const code = formData.value.code.trim().padStart(6, '0')
    const res = await api.get<any>(
      `/Investment/auto-fill?code=${encodeURIComponent(code)}&type=${encodeURIComponent(formData.value.type)}`
    )
    
    // useApi 已经自动解包了响应，res 就是 data 部分
    // 兼容两种格式：camelCase 和 PascalCase
    const name = res?.name || res?.Name || ''
    
    if (name) {
      formData.value.name = name
      success(`自动获取成功：${name}`)
    } else {
      // 场外基金可能无法自动获取
      const isOTC = code.startsWith('00') || code.startsWith('01') || code.startsWith('05')
      if (isOTC && formData.value.type === 'fund') {
        warning('这是场外基金，无法自动获取名称。请手动输入基金名称，例如：天弘沪深300ETF联接C')
      } else {
        warning('无法自动获取，请手动输入')
      }
    }
  } catch (err: any) {
    console.error('自动获取失败:', err)
    const errorMessage = err.response?.data?.message || err.message || '获取失败'
    warning(`自动获取失败：${errorMessage}，请手动输入`)
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
/* 使用统一样式类，样式定义在 assets/css/admin-asset-management.css */
/* 只保留组件特有的样式，使用 CSS 变量 */

.dca-plan-page {
  padding: var(--spacing-lg);
  max-width: 1600px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
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
  color: var(--color-text-main);
}

.loading {
  text-align: center;
  padding: 48px;
  color: var(--color-text-muted);
}

/* 状态徽章 */
.asset-management-badge {
  padding: 4px 8px;
  border-radius: var(--radius-sm);
  font-size: 12px;
  font-weight: 500;
}

.asset-management-badge-success {
  background: var(--color-success-bg, rgba(82, 196, 26, 0.1));
  color: var(--color-success, var(--color-green-500));
  border: 1px solid var(--color-success-border, rgba(82, 196, 26, 0.3));
}

.asset-management-badge-warning {
  background: var(--color-warning-bg, rgba(250, 140, 22, 0.1));
  color: var(--color-warning, var(--color-orange-500));
  border: 1px solid var(--color-warning-border, rgba(250, 140, 22, 0.3));
}

/* 模态框样式 - 使用统一样式类 */
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
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: var(--shadow-card);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border);
}

.modal-header h2 {
  margin: 0;
  font-size: 20px;
  color: var(--color-text-main);
}

.modal-close {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: var(--color-text-sec);
  transition: color 0.2s;
}

.modal-close:hover {
  color: var(--color-text-main);
}

.modal-body {
  padding: var(--spacing-lg);
}

.form-group {
  margin-bottom: var(--spacing-md);
}

.form-group label {
  display: block;
  margin-bottom: var(--spacing-sm);
  font-weight: 500;
  font-size: 14px;
  color: var(--color-text-main);
}

.form-group .required {
  color: var(--color-error);
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  padding: var(--spacing-sm) var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: 14px;
  background: var(--color-bg-card);
  color: var(--color-text-main);
  transition: border-color 0.2s;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: var(--color-primary);
}

.form-group input[type="checkbox"] {
  width: auto;
  margin-right: var(--spacing-sm);
}

.btn-auto-fill {
  margin-top: var(--spacing-sm);
  padding: 4px var(--spacing-md);
  background: var(--color-bg-body);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  cursor: pointer;
  font-size: 12px;
  color: var(--color-text-main);
  transition: all 0.2s;
}

.btn-auto-fill:hover {
  background: var(--color-bg-card);
  border-color: var(--color-primary);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: var(--spacing-md);
  margin-top: var(--spacing-lg);
}
</style>
