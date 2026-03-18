<template>
  <div>
    <div class="page-header">
      <h1 class="page-title">知识库管理</h1>
      <button @click="showCreateModal = true" class="btn-primary">
        + 新建条目
      </button>
    </div>

    <!-- 列表 -->
    <div class="table-container">
      <table class="table">
        <thead class="table-header">
          <tr>
            <th class="table-header-cell">标题</th>
            <th class="table-header-cell">分类</th>
            <th class="table-header-cell">标签</th>
            <th class="table-header-cell">查看</th>
            <th class="table-header-cell">更新时间</th>
            <th class="table-header-cell">操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="item in list" :key="item.id" class="table-row">
            <td class="table-cell">{{ item.title }}</td>
            <td class="table-cell">{{ item.category || '-' }}</td>
            <td class="table-cell text-sm">
              <span v-for="tag in parseTags(item.tags)" :key="tag" class="mr-1 badge badge-gray">
                {{ tag }}
              </span>
            </td>
            <td class="table-cell">{{ item.viewCount }}</td>
            <td class="table-cell text-sm">{{ formatDate(item.updatedAt) }}</td>
            <td class="table-cell">
              <div class="flex gap-2">
                <button @click="editItem(item)" class="btn-link btn-link--blue">编辑</button>
                <button @click="deleteItem(item.id)" class="btn-link btn-link--red">删除</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 创建/编辑模态框 - 侧边滑出-->
    <Transition name="slide-fade">
      <div v-if="showCreateModal || editingItem" class="knowledge-modal-overlay" @click.self="cancelEdit">
        <div class="knowledge-modal-content" @click.stop>
          <!-- 头部 -->
          <div class="knowledge-modal-header">
            <div class="knowledge-modal-header-content">
              <div class="knowledge-modal-icon">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M12 6.042A8.967 8.967 0 006 3.75c-1.052 0-2.062.18-3 .512v14.25A8.987 8.987 0 016 18c2.305 0 4.408.867 6 2.292m0-14.25a8.966 8.966 0 016-2.292c1.052 0 2.062.18 3 .512v14.25A8.987 8.987 0 0018 18a8.967 8.967 0 00-6 2.292m0-14.25v14.25" />
                </svg>
              </div>
              <div>
                <h2 class="knowledge-modal-title">{{ editingItem ? '编辑' : '新建' }}知识库条目</h2>
                <p class="knowledge-modal-subtitle">填写以下信息创建新的知识库条目</p>
              </div>
            </div>
            <button @click="cancelEdit" class="knowledge-modal-close" aria-label="关闭">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
          
          <!-- 表单内容 -->
          <div class="knowledge-modal-body">
            <div class="knowledge-form">
              <!-- 标题 -->
              <div class="knowledge-form-group">
                <label class="knowledge-form-label">
                  <span class="knowledge-form-label-text">标题</span>
                  <span class="knowledge-form-required">*</span>
                </label>
                <div class="knowledge-form-input-wrapper">
                  <input 
                    v-model="form.title" 
                    type="text" 
                    class="knowledge-form-input" 
                    placeholder="请输入知识库条目标题"
                    autofocus
                  />
                </div>
              </div>

              <!-- 分类和标签 - 并排显示 -->
              <div class="knowledge-form-row">
                <div class="knowledge-form-group knowledge-form-group-half">
                  <label class="knowledge-form-label">
                    <span class="knowledge-form-label-text">分类</span>
                  </label>
                  <div class="knowledge-form-select-wrapper">
                    <select v-model="form.category" class="knowledge-form-select">
                      <option value="">选择分类</option>
                      <option value="开发笔记">开发笔记</option>
                      <option value="踩坑记录">踩坑记录</option>
                      <option value="想法灵感">想法灵感</option>
                    </select>
                    <svg class="knowledge-form-select-icon" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
                    </svg>
                  </div>
                </div>

                <div class="knowledge-form-group knowledge-form-group-half">
                  <label class="knowledge-form-label">
                    <span class="knowledge-form-label-text">标签</span>
                    <span class="knowledge-form-hint">（逗号分隔）</span>
                  </label>
                  <div class="knowledge-form-input-wrapper">
                    <input 
                      v-model="form.tags" 
                      type="text" 
                      class="knowledge-form-input" 
                      placeholder="例如: Vue, Nuxt, 前端"
                    />
                  </div>
                </div>
              </div>

              <!-- 内容 -->
              <div class="knowledge-form-group">
                <label class="knowledge-form-label">
                  <span class="knowledge-form-label-text">内容</span>
                  <span class="knowledge-form-hint">（支持Markdown）</span>
                </label>
                <div class="knowledge-form-textarea-wrapper">
                  <textarea 
                    v-model="form.content" 
                    rows="12" 
                    class="knowledge-form-textarea"
                    placeholder="在此输入 Markdown 格式的内容.."
                  ></textarea>
                  <div class="knowledge-form-textarea-footer">
                    <span class="knowledge-form-textarea-hint">
                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="knowledge-form-hint-icon">
                        <path stroke-linecap="round" stroke-linejoin="round" d="M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z" />
                      </svg>
                      支持 Markdown 语法
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- 底部操作-->
          <div class="knowledge-modal-footer">
            <button @click="cancelEdit" class="knowledge-btn knowledge-btn-secondary">
              取消
            </button>
            <button @click="saveItem" class="knowledge-btn knowledge-btn-primary">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="knowledge-btn-icon">
                <path stroke-linecap="round" stroke-linejoin="round" d="M4.5 12.75l6 6 9-13.5" />
              </svg>
              保存
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import type { KnowledgeBase, KnowledgeBaseRequest } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const list = ref<KnowledgeBase[]>([])
const showCreateModal = ref(false)
const editingItem = ref<KnowledgeBase | null>(null)
const form = ref({
  title: '',
  content: '',
  category: '',
  tags: ''
})

const fetchList = async () => {
  try {
    const res = await api.get<{ Total: number; List: KnowledgeBase[] }>('/KnowledgeBase', { params: { page: 1, pageSize: 100 } })
    if (res && res.List) {
      list.value = res.List
    }
  } catch (e) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch knowledge base:', e)
    }
  }
}

const editItem = (item: KnowledgeBase) => {
  editingItem.value = item
  form.value = {
    title: item.title,
    content: item.content || '',
    category: item.category || '',
    tags: Array.isArray(item.tags) ? item.tags.join(',') : (item.tags || '')
  }
}

const saveItem = async () => {
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    if (!form.value.title || !form.value.title.trim()) {
      warning('请输入标题')
      return
    }

    const tagsArray = form.value.tags ? form.value.tags.split(',').map((t: string) => t.trim()).filter((t: string) => t) : []
    const payload: KnowledgeBaseRequest = {
      title: form.value.title.trim(),
      content: form.value.content || undefined,
      category: form.value.category || undefined,
      tags: tagsArray.length > 0 ? JSON.stringify(tagsArray) : undefined
    }

    if (editingItem.value) {
      await api.put(`/KnowledgeBase/${editingItem.value.id}`, payload)
    } else {
      await api.post('/KnowledgeBase', payload)
    }

    success('保存成功')
    cancelEdit()
    fetchList()
  } catch (e: unknown) {
    handleError(e, '保存失败，请检查后端日志')
  }
}

const deleteItem = async (id: number) => {
  if (!confirm('确定要删除吗？')) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    await api.delete(`/KnowledgeBase/${id}`)
    success('删除成功')
    fetchList()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

const cancelEdit = () => {
  showCreateModal.value = false
  editingItem.value = null
  form.value = { title: '', content: '', category: '', tags: '' }
}

const parseTags = (tags: string) => {
  if (!tags) return []
  try {
    return JSON.parse(tags)
  } catch {
    return tags.split(',').map(t => t.trim()).filter(t => t)
  }
}

const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleString('zh-CN')
}

onMounted(() => {
  fetchList()
})
</script>

<style scoped>
/* 模态框遮罩*/
.knowledge-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(4px);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  animation: fadeIn 0.2s ease-out;
}

/* 模态框内容 - 侧边滑出 */
.knowledge-modal-content {
  width: 100%;
  max-width: 680px;
  height: 100vh;
  background: var(--color-bg-card, var(--color-bg-card));
  box-shadow: -4px 0 24px var(--shadow-medium, rgba(0, 0, 0, 0.15));
  display: flex;
  flex-direction: column;
  animation: slideIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  overflow: hidden;
}

[data-theme="dark"] .knowledge-modal-content {
  background: var(--color-bg-dark);
  box-shadow: -4px 0 24px rgba(0, 0, 0, 0.4);
}

/* 头部 */
.knowledge-modal-header {
  padding: 24px 28px;
  border-bottom: 1px solid var(--color-border, var(--color-border));
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.05) 0%, rgba(147, 51, 234, 0.05) 100%);
}

[data-theme="dark"] .knowledge-modal-header {
  border-bottom-color: var(--color-text-main);
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.1) 0%, rgba(147, 51, 234, 0.1) 100%);
}

.knowledge-modal-header-content {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  flex: 1;
}

.knowledge-modal-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  background: linear-gradient(135deg, var(--color-primary) 0%, var(--color-purple-600) 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-bg-light, white);
  flex-shrink: 0;
}

.knowledge-modal-icon svg {
  width: 24px;
  height: 24px;
}

.knowledge-modal-title {
  font-size: 20px;
  font-weight: 600;
  color: var(--color-text-main, var(--color-text-main));
  margin: 0 0 4px 0;
  line-height: 1.4;
}

[data-theme="dark"] .knowledge-modal-title {
  color: var(--color-bg-elevated);
}

.knowledge-modal-subtitle {
  font-size: 14px;
  color: var(--color-text-sec, var(--color-text-sec));
  margin: 0;
  line-height: 1.5;
}

[data-theme="dark"] .knowledge-modal-subtitle {
  color: var(--color-text-muted);
}

.knowledge-modal-close {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  border: none;
  background: transparent;
  color: var(--color-text-sec, var(--color-text-sec));
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  flex-shrink: 0;
}

.knowledge-modal-close:hover {
  background: var(--color-bg-hover, var(--color-gray-100));
  color: var(--color-text-main, var(--color-text-main));
}

[data-theme="dark"] .knowledge-modal-close:hover {
  background: var(--color-text-main);
  color: var(--color-bg-elevated);
}

.knowledge-modal-close svg {
  width: 20px;
  height: 20px;
}

/* 表单内容区域 */
.knowledge-modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 28px;
}

.knowledge-form {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.knowledge-form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.knowledge-form-group-half {
  flex: 1;
}

.knowledge-form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.knowledge-form-label {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
  font-weight: 500;
  color: var(--color-text-main, var(--color-text-main));
}

[data-theme="dark"] .knowledge-form-label {
  color: var(--color-border);
}

.knowledge-form-label-text {
  line-height: 1.5;
}

.knowledge-form-required {
  color: var(--color-danger);
  font-weight: 600;
}

.knowledge-form-hint {
  font-size: 12px;
  font-weight: 400;
  color: var(--color-text-sec, var(--color-gray-400));
}

.knowledge-form-input-wrapper,
.knowledge-form-select-wrapper,
.knowledge-form-textarea-wrapper {
  position: relative;
}

.knowledge-form-input,
.knowledge-form-select,
.knowledge-form-textarea {
  width: 100%;
  padding: 12px 16px;
  font-size: 14px;
  line-height: 1.5;
  color: var(--color-text-main, var(--color-text-main));
  background: var(--color-bg-input, var(--color-bg-card));
  border: 1.5px solid var(--color-border, var(--color-border));
  border-radius: 10px;
  transition: all 0.2s;
  font-family: inherit;
}

[data-theme="dark"] .knowledge-form-input,
[data-theme="dark"] .knowledge-form-select,
[data-theme="dark"] .knowledge-form-textarea {
  background: var(--color-text-main);
  border-color: var(--color-text-main);
  color: var(--color-bg-elevated);
}

.knowledge-form-input:focus,
.knowledge-form-select:focus,
.knowledge-form-textarea:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

[data-theme="dark"] .knowledge-form-input:focus,
[data-theme="dark"] .knowledge-form-select:focus,
[data-theme="dark"] .knowledge-form-textarea:focus {
  border-color: var(--color-primary-soft);
  box-shadow: 0 0 0 3px rgba(96, 165, 250, 0.15);
}

.knowledge-form-input::placeholder,
.knowledge-form-textarea::placeholder {
  color: var(--color-text-placeholder, var(--color-gray-400));
}

.knowledge-form-select {
  appearance: none;
  padding-right: 44px;
  cursor: pointer;
}

.knowledge-form-select-icon {
  position: absolute;
  right: 16px;
  top: 50%;
  transform: translateY(-50%);
  width: 20px;
  height: 20px;
  color: var(--color-text-sec, var(--color-text-sec));
  pointer-events: none;
}

.knowledge-form-textarea {
  resize: vertical;
  min-height: 200px;
  font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', 'Consolas', monospace;
  font-size: 13px;
  line-height: 1.6;
}

.knowledge-form-textarea-footer {
  margin-top: 8px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.knowledge-form-textarea-hint {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: var(--color-text-sec, var(--color-gray-400));
}

.knowledge-form-hint-icon {
  width: 14px;
  height: 14px;
}

/* 底部操作�?*/
.knowledge-modal-footer {
  padding: 20px 28px;
  border-top: 1px solid var(--color-border, var(--color-border));
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 12px;
  background: var(--color-bg-card, var(--color-bg-card));
}

[data-theme="dark"] .knowledge-modal-footer {
  border-top-color: var(--color-text-main);
  background: var(--color-bg-dark);
}

.knowledge-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 10px 20px;
  font-size: 14px;
  font-weight: 500;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  transition: all 0.2s;
  font-family: inherit;
}

.knowledge-btn-secondary {
  background: var(--color-bg-hover, var(--color-gray-100));
  color: var(--color-text-main, var(--color-text-main));
}

[data-theme="dark"] .knowledge-btn-secondary {
  background: var(--color-text-main);
  color: var(--color-border);
}

.knowledge-btn-secondary:hover {
  background: var(--color-bg-hover-active, var(--color-border));
}

[data-theme="dark"] .knowledge-btn-secondary:hover {
  background: var(--color-gray-600);
}

.knowledge-btn-primary {
  background: linear-gradient(135deg, var(--color-primary) 0%, var(--color-purple-600) 100%);
  color: var(--color-bg-light, white);
  box-shadow: 0 2px 8px var(--theme-primary);
}

.knowledge-btn-primary:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
}

.knowledge-btn-primary:active {
  transform: translateY(0);
}

.knowledge-btn-icon {
  width: 18px;
  height: 18px;
}

/* 动画 */
@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
  }
  to {
    transform: translateX(0);
  }
}

.slide-fade-enter-active {
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

.slide-fade-leave-active {
  transition: all 0.25s cubic-bezier(0.7, 0, 0.84, 0);
}

.slide-fade-enter-from {
  opacity: 0;
}

.slide-fade-enter-from .knowledge-modal-content {
  transform: translateX(100%);
}

.slide-fade-leave-to {
  opacity: 0;
}

.slide-fade-leave-to .knowledge-modal-content {
  transform: translateX(100%);
}

/* 响应式设�?*/
@media (max-width: 768px) {
  .knowledge-modal-content {
    max-width: 100%;
  }

  .knowledge-form-row {
    grid-template-columns: 1fr;
    gap: 24px;
  }

  .knowledge-modal-header {
    padding: 20px;
  }

  .knowledge-modal-body {
    padding: 20px;
  }

  .knowledge-modal-footer {
    padding: 16px 20px;
  }
}
</style>

