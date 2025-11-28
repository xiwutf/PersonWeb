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
            <th class="table-header-cell">查看数</th>
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

    <!-- 创建/编辑模态框 -->
    <div v-if="showCreateModal || editingItem" class="modal-overlay">
      <div class="modal-content-lg">
        <div class="modal-header">
          <h2 class="modal-title">{{ editingItem ? '编辑' : '新建' }}知识库条目</h2>
        </div>
        
        <div class="modal-body space-y-4">
          <div class="form-group">
            <label class="form-label">标题</label>
            <input v-model="form.title" type="text" class="form-input" />
          </div>
          <div class="form-group">
            <label class="form-label">分类</label>
            <select v-model="form.category" class="form-select">
              <option value="">选择分类</option>
              <option value="开发笔记">开发笔记</option>
              <option value="踩坑记录">踩坑记录</option>
              <option value="想法灵感">想法灵感</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">标签（逗号分隔）</label>
            <input v-model="form.tags" type="text" class="form-input" placeholder="例如: Vue, Nuxt, 前端" />
          </div>
          <div class="form-group">
            <label class="form-label">内容（Markdown）</label>
            <textarea v-model="form.content" rows="10" class="form-textarea"></textarea>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="saveItem" class="btn-primary">保存</button>
          <button @click="cancelEdit" class="btn-secondary">取消</button>
        </div>
      </div>
    </div>
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

