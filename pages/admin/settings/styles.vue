<template>
  <div class="styles-management-page">
    <div class="page-header">
      <h1 class="page-title">样式管理</h1>
      <p class="text-gray-400 text-sm">统一管理系统样式配置，支持动态修改、删除和新增</p>
    </div>

    <!-- 分类标签�?-->
    <div class="tabs-container">
      <div class="tabs">
        <button
          v-for="category in categories"
          :key="category.id"
          @click="activeCategory = category"
          :class="['tab-button', { 'tab-button-active': activeCategory?.id === category.id }]"
        >
          {{ category.name }}
        </button>
      </div>
    </div>

    <!-- 样式列表 -->
    <div v-if="activeCategory" class="styles-section">
      <div class="section-header">
        <h2 class="section-title">{{ activeCategory.name }}样式</h2>
        <div class="flex gap-2">
          <button 
            v-if="styles.length === 0" 
            @click="initDefaultStyles" 
            class="btn-secondary"
            :disabled="initializing"
          >
            <i class="fas fa-magic mr-2"></i>
            {{ initializing ? '初始化中...' : '初始化默认样�? }}
          </button>
          <button @click="openStyleModal()" class="btn-primary">
            <i class="fas fa-plus mr-2"></i>
            新增样式
          </button>
        </div>
      </div>

      <div v-if="loading" class="table-loading">加载�?..</div>
      <div v-else-if="styles.length === 0" class="table-empty">暂无样式</div>
      <div v-else class="styles-grid">
        <div
          v-for="style in styles"
          :key="style.id"
          class="style-card"
          :class="{ 'style-card-inactive': !style.isActive }"
        >
          <div class="style-card-header">
            <div class="style-card-title">
              <h3>{{ style.name }}</h3>
              <span class="style-code">{{ style.code }}</span>
            </div>
            <div class="style-card-badge" :class="style.isActive ? 'badge-success' : 'badge-default'">
              {{ style.isActive ? '启用' : '禁用' }}
            </div>
          </div>

          <div class="style-preview">
            <span :class="style.cssClass" class="style-preview-item">
              预览文字
            </span>
          </div>

          <div class="style-info">
            <div class="style-info-item">
              <span class="info-label">CSS类名:</span>
              <code class="info-value">{{ style.cssClass }}</code>
            </div>
            <div v-if="style.description" class="style-info-item">
              <span class="info-label">描述:</span>
              <span class="info-value">{{ style.description }}</span>
            </div>
            <div class="style-info-item">
              <span class="info-label">使用统计:</span>
              <span class="info-value">{{ getStyleUsageCount(style.id) }} �?/span>
            </div>
          </div>

          <div class="style-card-actions">
            <button @click="openStyleModal(style)" class="btn-link btn-link-blue">
              编辑
            </button>
            <button @click="viewUsage(style.id)" class="btn-link btn-link-blue">
              查看使用
            </button>
            <button @click="deleteStyle(style.id)" class="btn-link btn-link-red">
              删除
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- 样式编辑模态框 -->
    <div v-if="showStyleModal" class="modal-overlay" @click="showStyleModal = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ editingStyle?.id ? '编辑样式' : '新增样式' }}</h2>
          <button @click="showStyleModal = false" class="modal-close">×</button>
        </div>

        <form @submit.prevent="saveStyle" class="modal-body">
          <div class="form-group">
            <label class="form-label">样式名称 *</label>
            <input v-model="styleForm.name" type="text" class="form-input" required />
          </div>

          <div class="form-group">
            <label class="form-label">样式代码 *</label>
            <input v-model="styleForm.code" type="text" class="form-input" required />
            <small class="form-hint">用于在代码中引用，如：tag-info</small>
          </div>

          <div class="form-group">
            <label class="form-label">CSS类名 *</label>
            <input v-model="styleForm.cssClass" type="text" class="form-input" required />
            <small class="form-hint">实际的CSS类名，如：tag tag-info</small>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label">背景颜色</label>
              <input v-model="styleForm.backgroundColor" type="text" class="form-input" placeholder="var(--theme-primary)" />
            </div>
            <div class="form-group">
              <label class="form-label">边框颜色</label>
              <input v-model="styleForm.borderColor" type="text" class="form-input" placeholder="rgba(59, 130, 246, 0.6)" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label">文字颜色</label>
              <input v-model="styleForm.textColor" type="text" class="form-input" placeholder="var(--color-blue-200)" />
            </div>
            <div class="form-group">
              <label class="form-label">字体大小</label>
              <input v-model="styleForm.fontSize" type="text" class="form-input" placeholder="0.75rem" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label">字体粗细</label>
              <input v-model="styleForm.fontWeight" type="text" class="form-input" placeholder="500" />
            </div>
            <div class="form-group">
              <label class="form-label">内边�?/label>
              <input v-model="styleForm.padding" type="text" class="form-input" placeholder="0.25rem 0.5rem" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label">圆角</label>
              <input v-model="styleForm.borderRadius" type="text" class="form-input" placeholder="0.25rem" />
            </div>
            <div class="form-group">
              <label class="form-label">边框宽度</label>
              <input v-model="styleForm.borderWidth" type="text" class="form-input" placeholder="1px" />
            </div>
          </div>


          <div class="form-group">
            <label class="form-label">描述</label>
            <textarea v-model="styleForm.description" class="form-input" rows="2"></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label">排序</label>
              <input v-model.number="styleForm.sort" type="number" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">
                <input v-model="styleForm.isActive" type="checkbox" />
                启用
              </label>
            </div>
          </div>

          <div class="modal-footer">
            <button type="button" @click="showStyleModal = false" class="btn-secondary">取消</button>
            <button type="submit" class="btn-primary">保存</button>
          </div>
        </form>
      </div>
    </div>

    <!-- 使用统计模态框 -->
    <div v-if="showUsageModal" class="modal-overlay" @click="showUsageModal = false">
      <div class="modal-content modal-content-large" @click.stop>
        <div class="modal-header">
          <h2>样式使用统计</h2>
          <button @click="showUsageModal = false" class="modal-close">×</button>
        </div>

        <div class="modal-body">
          <div v-if="usageStats.length === 0" class="table-empty">暂无使用记录</div>
          <table v-else class="data-table">
            <thead class="table-header">
              <tr>
                <th>页面路径</th>
                <th>组件名称</th>
                <th>使用次数</th>
                <th>最后使�?/th>
              </tr>
            </thead>
            <tbody class="table-body">
              <tr v-for="usage in usageStats" :key="usage.id" class="table-row">
                <td class="table-cell">{{ usage.pagePath }}</td>
                <td class="table-cell">{{ usage.componentName || '-' }}</td>
                <td class="table-cell">{{ usage.usageCount }}</td>
                <td class="table-cell">{{ formatDate(usage.lastUsedAt) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { handleError } = useErrorHandler()
const message = useSafeMessage()

interface StyleCategory {
  id: number
  name: string
  code: string
  description?: string
  sort: number
}

interface StyleDefinition {
  id: number
  categoryId: number
  name: string
  code: string
  cssClass: string
  backgroundColor?: string
  borderColor?: string
  textColor?: string
  fontSize?: string
  fontWeight?: string
  padding?: string
  borderRadius?: string
  borderWidth?: string
  description?: string
  isActive: boolean
  sort: number
}

interface StyleUsage {
  id: number
  styleId: number
  pagePath: string
  componentName?: string
  usageCount: number
  lastUsedAt?: string
}

interface StyleUsageStats {
  styleId: number
  styleCode: string
  styleName: string
  categoryName: string
  totalUsage: number
  pageCount: number
  componentCount: number
  pages: string[]
  components: string[]
  lastUsedAt?: string
}

const categories = ref<StyleCategory[]>([])
const activeCategory = ref<StyleCategory | null>(null)
const styles = ref<StyleDefinition[]>([])
const usageStats = ref<StyleUsage[]>([])
const allUsageStats = ref<StyleUsageStats[]>([])
const loading = ref(false)
const initializing = ref(false)
const showStyleModal = ref(false)
const showUsageModal = ref(false)
const editingStyle = ref<StyleDefinition | null>(null)

const styleForm = ref({
  id: 0,
  categoryId: 0,
  name: '',
  code: '',
  cssClass: '',
  backgroundColor: '',
  borderColor: '',
  textColor: '',
  fontSize: '',
  fontWeight: '',
  padding: '',
  borderRadius: '',
  borderWidth: '',
  description: '',
  isActive: true,
  sort: 0
})

const fetchCategories = async () => {
  try {
    const res = await api.get<StyleCategory[]>('/StyleManagement/categories')
    categories.value = Array.isArray(res) ? res : []
    if (categories.value.length > 0 && !activeCategory.value) {
      activeCategory.value = categories.value[0]
      fetchStyles()
    }
  } catch (e: unknown) {
    handleError(e, '加载分类失败')
  }
}

const fetchStyles = async () => {
  if (!activeCategory.value) return

  loading.value = true
  try {
    const res = await api.get<StyleDefinition[]>(`/StyleManagement/category/${activeCategory.value.id}/styles`)
    styles.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    handleError(e, '加载样式失败')
  } finally {
    loading.value = false
  }
}

const fetchUsageStats = async () => {
  try {
    const res = await api.get<StyleUsageStats[]>('/StyleManagement/usage/stats')
    allUsageStats.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    handleError(e, '加载使用统计失败')
  }
}

const getStyleUsageCount = (styleId: number): number => {
  const stats = allUsageStats.value.find(s => s.styleId === styleId)
  return stats?.totalUsage || 0
}

const openStyleModal = (style?: StyleDefinition) => {
  if (style) {
    editingStyle.value = style
    styleForm.value = {
      id: style.id,
      categoryId: style.categoryId,
      name: style.name,
      code: style.code,
      cssClass: style.cssClass,
      backgroundColor: style.backgroundColor || '',
      borderColor: style.borderColor || '',
      textColor: style.textColor || '',
      fontSize: style.fontSize || '',
      fontWeight: style.fontWeight || '',
      padding: style.padding || '',
      borderRadius: style.borderRadius || '',
      borderWidth: style.borderWidth || '',
      description: style.description || '',
      isActive: style.isActive,
      sort: style.sort
    }
  } else {
    editingStyle.value = null
    styleForm.value = {
      id: 0,
      categoryId: activeCategory.value?.id || 0,
      name: '',
      code: '',
      cssClass: '',
      backgroundColor: '',
      borderColor: '',
      textColor: '',
      fontSize: '',
      fontWeight: '',
      padding: '',
      borderRadius: '',
      borderWidth: '',
      description: '',
      isActive: true,
      sort: 0
    }
  }
  showStyleModal.value = true
}

const saveStyle = async () => {
  try {
    await api.post('/StyleManagement/style', styleForm.value)
    message.success('保存成功')
    showStyleModal.value = false
    fetchStyles()
    fetchUsageStats()
  } catch (e: unknown) {
    handleError(e, '保存失败')
  }
}

const deleteStyle = async (id: number) => {
  if (!confirm('确定要删除这个样式吗？删除后使用该样式的地方可能会显示异常�?)) return

  try {
    await api.del(`/StyleManagement/style/${id}`)
    message.success('删除成功')
    fetchStyles()
    fetchUsageStats()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

const viewUsage = async (styleId: number) => {
  try {
    const res = await api.get<StyleUsage[]>(`/StyleManagement/style/${styleId}/usage`)
    usageStats.value = Array.isArray(res) ? res : []
    showUsageModal.value = true
  } catch (e: unknown) {
    handleError(e, '加载使用记录失败')
  }
}

const formatDate = (dateStr?: string) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleString('zh-CN')
}

const initDefaultStyles = async () => {
  if (!activeCategory.value) return
  
  try {
    initializing.value = true
    const res = await api.post<{ count: number }>('/StyleManagement/init-defaults')
    
    if (res && res.count > 0) {
      // 显示成功提示
      if (process.client) {
        window.dispatchEvent(new CustomEvent('show-toast', {
          detail: {
            message: `成功初始�?${res.count} 个默认样式`,
            type: 'success',
            duration: 3000
          }
        }))
      }
      // 重新加载样式列表
      await fetchStyles()
    } else {
      if (process.client) {
        window.dispatchEvent(new CustomEvent('show-toast', {
          detail: {
            message: '该分类已有样式，无需初始�?,
            type: 'info',
            duration: 3000
          }
        }))
      }
    }
  } catch (e: unknown) {
    handleError(e, '初始化失�?)
  } finally {
    initializing.value = false
  }
}

watch(activeCategory, () => {
  if (activeCategory.value) {
    fetchStyles()
  }
})

onMounted(() => {
  fetchCategories()
  fetchUsageStats()
})
</script>

<style scoped>
.styles-management-page {
  width: 100%;
}

/* 标签�?*/
.tabs-container {
  margin-bottom: var(--spacing-2xl);
}

.tabs {
  display: flex;
  gap: var(--spacing-sm);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.tab-button {
  padding: var(--spacing-md) var(--spacing-xl);
  background: transparent;
  border: none;
  border-bottom: 2px solid transparent;
  color: rgba(255, 255, 255, 0.7);
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: var(--text-sm);
}

.tab-button:hover {
  color: rgba(255, 255, 255, 0.9);
  background: rgba(255, 255, 255, 0.05);
}

.tab-button-active {
  color: var(--color-bg-card);
  border-bottom-color: var(--color-primary);
}

/* 样式区域 */
.styles-section {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-md);
  padding: var(--spacing-xl);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
}

.section-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-bg-card);
}

/* 样式网格 */
.styles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: var(--spacing-xl);
}

.style-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-md);
  padding: var(--spacing-10);
  transition: all 0.3s ease;
}

.style-card:hover {
  background: var(--color-border);
  border-color: rgba(255, 255, 255, 0.2);
}

.style-card-inactive {
  opacity: 0.6;
}

.style-card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-md);
}

.style-card-title h3 {
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--color-bg-card);
  margin-bottom: var(--spacing-xs);
}

.style-code {
  font-size: var(--text-xs);
  color: rgba(255, 255, 255, 0.6);
  font-family: monospace;
}

.style-card-badge {
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--radius-sm);
  font-size: var(--text-xs);
  font-weight: 500;
}

.badge-success {
  background: rgba(34, 197, 94, 0.3);
  border: 1px solid rgba(34, 197, 94, 0.6);
  color: var(--color-purple-300);
}

.badge-default {
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: rgba(255, 255, 255, 0.9);
}

.style-preview {
  padding: var(--spacing-md);
  background: rgba(0, 0, 0, 0.2);
  border-radius: var(--radius-sm);
  margin-bottom: var(--spacing-md);
  text-align: center;
}

.style-preview-item {
  display: inline-block;
}

.style-info {
  margin-bottom: var(--spacing-md);
}

.style-info-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: var(--spacing-sm);
  font-size: var(--text-sm);
}

.info-label {
  color: rgba(255, 255, 255, 0.6);
}

.info-value {
  color: rgba(255, 255, 255, 0.9);
  font-family: monospace;
}

.style-card-actions {
  display: flex;
  gap: var(--spacing-sm);
  padding-top: var(--spacing-md);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

/* 模态框 */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: var(--overlay-color, rgba(0, 0, 0, 0.7));
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: rgba(30, 41, 59, 0.95);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-lg);
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-content-large {
  max-width: 800px;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-xl);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.modal-header h2 {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-bg-card);
}

.modal-close {
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.7);
  font-size: var(--text-3xl);
  cursor: pointer;
  width: var(--spacing-2xl);
  height: var(--spacing-2xl);
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: var(--radius-sm);
  transition: all 0.2s ease;
}

.modal-close:hover {
  background: rgba(255, 255, 255, 0.1);
  color: var(--color-bg-card);
}

.modal-body {
  padding: var(--spacing-xl);
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: var(--spacing-lg);
  padding-top: var(--spacing-xl);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  margin-top: var(--spacing-xl);
}

/* 表单 */
.form-group {
  margin-bottom: var(--spacing-10);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

.form-label {
  display: block;
  margin-bottom: var(--spacing-sm);
  color: rgba(255, 255, 255, 0.9);
  font-size: var(--text-sm);
  font-weight: 500;
}

.form-input {
  width: 100%;
  padding: var(--spacing-md);
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: var(--radius-sm);
  color: var(--color-bg-card);
  font-size: var(--text-sm);
  transition: all 0.2s ease;
}

.form-input:focus {
  outline: none;
  border-color: var(--color-primary);
  background: rgba(255, 255, 255, 0.15);
}

.form-input::placeholder {
  color: rgba(255, 255, 255, 0.4);
}

.form-hint {
  display: block;
  margin-top: var(--spacing-xs);
  font-size: var(--text-xs);
  color: rgba(255, 255, 255, 0.5);
}

/* 按钮 */
.btn-primary {
  padding: var(--spacing-sm) var(--spacing-md);
  background: var(--theme-primary);
  border: 1px solid var(--theme-primary);
  border-radius: var(--radius-sm);
  color: var(--color-bg-card);
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: var(--text-sm);
}

.btn-primary:hover {
  background: rgba(59, 130, 246, 0.4);
  border-color: rgba(59, 130, 246, 0.6);
}

.btn-secondary {
  padding: var(--spacing-sm) var(--spacing-md);
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: var(--radius-sm);
  color: rgba(255, 255, 255, 0.9);
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: var(--text-sm);
}

.btn-secondary:hover {
  background: rgba(255, 255, 255, 0.2);
}

.btn-link {
  background: none;
  border: none;
  cursor: pointer;
  text-decoration: none;
  transition: color 0.2s ease;
  font-size: var(--text-sm);
}

.btn-link-blue {
  color: var(--color-primary-soft);
}

.btn-link-blue:hover {
  color: var(--color-primary-400);
}

.btn-link-red {
  color: var(--color-danger-400);
}

.btn-link-red:hover {
  color: var(--color-danger-300);
}

/* 表格样式 */
.data-table {
  width: 100%;
  text-align: left;
  border-collapse: collapse;
}

.table-header {
  background: rgba(255, 255, 255, 0.05);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.table-header th {
  padding: var(--spacing-md) var(--spacing-lg);
  font-size: var(--text-sm);
  font-weight: 500;
  color: rgba(255, 255, 255, 0.6);
}

.table-body {
  border-collapse: collapse;
}

.table-row {
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.table-row:hover {
  background: rgba(255, 255, 255, 0.05);
}

.table-cell {
  padding: var(--spacing-md);
  color: rgba(255, 255, 255, 0.9);
  font-size: var(--text-sm);
}
</style>

