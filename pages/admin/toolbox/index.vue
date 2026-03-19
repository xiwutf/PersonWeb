<template>
  <div class="space-y-6">
    <div class="page-header">
      <h1 class="page-title">工具商城管理</h1>
      <div class="flex gap-2">
        <NuxtLink to="/admin/toolbox/categories" class="btn-secondary">
          <i class="fas fa-folder mr-2"></i>分类管理
        </NuxtLink>
        <NuxtLink to="/admin/toolbox/collections" class="btn-secondary">
          <i class="fas fa-layer-group mr-2"></i>工具合集
        </NuxtLink>
        <button @click="openModal()" class="btn-primary">
          <i class="fas fa-plus mr-2"></i>新增工具
        </button>
      </div>
    </div>

    <!-- 统计卡片 -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总工具数</div>
        <div class="text-2xl font-bold">{{ stats.totalTools }}</div>
      </div>
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总购买数</div>
        <div class="text-2xl font-bold">{{ stats.totalPurchases }}</div>
      </div>
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总使用数</div>
        <div class="text-2xl font-bold">{{ stats.totalUses }}</div>
      </div>
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总收入</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">¥{{ stats.totalRevenue.toFixed(2) }}</div>
      </div>
    </div>

    <!-- 工具列表 -->
    <div class="card">
      <div class="p-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-xl font-bold">工具列表</h2>
          <div class="flex gap-2">
            <select
              v-model="filterStatus"
              class="px-3 py-2 bg-gray-700 border border-gray-600 rounded-lg text-sm"
            >
              <option value="">全部状态</option>
              <option value="published">已发布</option>
              <option value="draft">草稿</option>
              <option value="archived">已归档</option>
            </select>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="搜索工具..."
              class="px-3 py-2 bg-gray-700 border border-gray-600 rounded-lg text-sm"
            />
          </div>
        </div>

        <div v-if="loading" class="text-center py-12">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
        </div>

        <div v-else class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b border-gray-700">
                <th class="text-left p-3 text-sm font-medium">工具名称</th>
                <th class="text-left p-3 text-sm font-medium">分类</th>
                <th class="text-left p-3 text-sm font-medium">价格</th>
                <th class="text-left p-3 text-sm font-medium">购买数</th>
                <th class="text-left p-3 text-sm font-medium">使用数</th>
                <th class="text-left p-3 text-sm font-medium">评分</th>
                <th class="text-left p-3 text-sm font-medium">状态</th>
                <th class="text-right p-3 text-sm font-medium">操作</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="tool in filteredTools"
                :key="tool.id"
                class="border-b border-gray-700 hover:bg-gray-800/50 transition-colors"
              >
                <td class="p-3">
                  <div class="flex items-center gap-2">
                    <span class="text-xl">{{ tool.icon || '🔧' }}</span>
                    <div>
                      <div class="font-medium">{{ tool.name }}</div>
                      <div class="text-xs text-gray-500">{{ tool.slug }}</div>
                    </div>
                  </div>
                </td>
                <td class="p-3 text-sm">{{ tool.category?.name || '-' }}</td>
                <td class="p-3">
                  <span v-if="tool.isFree" class="text-green-500">免费</span>
                  <span v-else>¥{{ tool.price }}</span>
                </td>
                <td class="p-3 text-sm">{{ tool.purchaseCount }}</td>
                <td class="p-3 text-sm">{{ tool.useCount }}</td>
                <td class="p-3 text-sm">
                  <span v-if="tool.ratingCount > 0">
                    {{ tool.rating.toFixed(1) }} ({{ tool.ratingCount }})
                  </span>
                  <span v-else class="text-gray-500">暂无评分</span>
                </td>
                <td class="p-3">
                  <span
                    :class="[
                      'px-2 py-1 rounded text-xs',
                      tool.status === 'published' ? 'bg-green-500/20 text-green-400' :
                      tool.status === 'draft' ? 'bg-yellow-500/20 text-yellow-400' :
                      'bg-gray-500/20 text-gray-400'
                    ]"
                  >
                    {{ tool.status === 'published' ? '已发布' : tool.status === 'draft' ? '草稿' : '已归档' }}
                  </span>
                </td>
                <td class="p-3 text-right">
                  <div class="flex items-center justify-end gap-2">
                    <button
                      @click="openModal(tool)"
                      class="text-blue-400 hover:text-blue-300 transition-colors"
                      title="编辑"
                    >
                      ✏️
                    </button>
                    <NuxtLink
                      :to="`/admin/toolbox/${tool.id}/analytics`"
                      class="text-purple-400 hover:text-purple-300 transition-colors"
                      title="统计"
                    >
                      📊
                    </NuxtLink>
                    <button
                      @click="handleDelete(tool)"
                      class="text-red-400 hover:text-red-300 transition-colors"
                      title="删除"
                    >
                      🗑删除                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- 编辑/新建弹窗 -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-content max-w-4xl">
        <div class="modal-header">
          <h2 class="modal-title">{{ isEdit ? '编辑工具' : '新增工具' }}</h2>
          <button @click="showModal = false" class="modal-close">✕</button>
        </div>
        <div class="modal-body">
          <!-- 工具表单 -->
          <div class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="form-label">工具名称 *</label>
                <input v-model="form.name" type="text" class="form-input" />
              </div>
              <div>
                <label class="form-label">Slug *</label>
                <input v-model="form.slug" type="text" class="form-input" />
              </div>
            </div>

            <div>
              <label class="form-label">分类</label>
              <select v-model="form.categoryId" class="form-input">
                <option :value="null">无分类</option>
                <option
                  v-for="cat in categories"
                  :key="cat.id"
                  :value="cat.id"
                >
                  {{ cat.icon }} {{ cat.name }}
                </option>
              </select>
            </div>

            <div>
              <label class="form-label">描述</label>
              <textarea v-model="form.description" rows="3" class="form-input"></textarea>
            </div>

            <div>
              <label class="form-label">详细描述（Markdown）</label>
              <textarea v-model="form.detailedDescription" rows="10" class="form-input font-mono text-sm"></textarea>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="form-label">价格</label>
                <input v-model.number="form.price" type="number" step="0.01" class="form-input" />
              </div>
              <div>
                <label class="form-label">原价（用于显示折扣）</label>
                <input v-model.number="form.originalPrice" type="number" step="0.01" class="form-input" />
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="form-label">封面图片URL</label>
                <input v-model="form.coverImage" type="text" class="form-input" />
              </div>
              <div>
                <label class="form-label">演示地址</label>
                <input v-model="form.demoUrl" type="text" class="form-input" />
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="form-label">API接口地址</label>
                <input v-model="form.apiEndpoint" type="text" class="form-input" />
              </div>
              <div>
                <label class="form-label">版本</label>
                <input v-model="form.version" type="text" class="form-input" />
              </div>
            </div>

            <div class="flex gap-4">
              <label class="flex items-center gap-2">
                <input v-model="form.isFree" type="checkbox" class="form-checkbox" />
                <span>免费工具</span>
              </label>
              <label class="flex items-center gap-2">
                <input v-model="form.isPremium" type="checkbox" class="form-checkbox" />
                <span>高级工具</span>
              </label>
            </div>

            <div>
              <label class="form-label">状态</label>
              <select v-model="form.status" class="form-input">
                <option value="draft">草稿</option>
                <option value="published">已发布</option>
                <option value="archived">已归档</option>
              </select>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="showModal = false" class="btn-secondary">取消</button>
          <button @click="handleSave" class="btn-primary">保存</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { success, warning } = useNotification()
const { handleError } = useErrorHandler()

interface Tool {
  id: number
  name: string
  slug: string
  icon?: string
  description?: string
  categoryId?: number
  price: number
  originalPrice?: number
  isFree: boolean
  isPremium: boolean
  purchaseCount: number
  useCount: number
  rating: number
  ratingCount: number
  status: string
  category?: {
    id: number
    name: string
  }
}

interface Category {
  id: number
  name: string
  slug: string
  icon?: string
}

const tools = ref<Tool[]>([])
const categories = ref<Category[]>([])
const loading = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const searchQuery = ref('')
const filterStatus = ref('')
const stats = ref({
  totalTools: 0,
  totalPurchases: 0,
  totalUses: 0,
  totalRevenue: 0
})

const form = ref({
  name: '',
  slug: '',
  categoryId: null as number | null,
  description: '',
  detailedDescription: '',
  coverImage: '',
  demoUrl: '',
  apiEndpoint: '',
  version: '1.0.0',
  price: 0,
  originalPrice: null as number | null,
  isFree: false,
  isPremium: false,
  status: 'draft'
})

const filteredTools = computed(() => {
  let result = tools.value

  if (filterStatus.value) {
    result = result.filter(t => t.status === filterStatus.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(t => 
      t.name.toLowerCase().includes(query) ||
      t.slug.toLowerCase().includes(query)
    )
  }

  return result
})

// 防止重复请求
let isFetchingTools = false

// 获取工具列表
const fetchTools = async () => {
  // 如果正在请求中，直接返回
  if (isFetchingTools) {
    return
  }
  
  isFetchingTools = true
  loading.value = true
  
  try {
    // 管理后台使用 admin/list 接口，可以查看所有状态的工具
    const res = await api.get('/Toolbox/admin/list?pageSize=1000')
    if (res && res.tools) {
      tools.value = res.tools as Tool[]
      stats.value.totalTools = tools.value.length
      stats.value.totalPurchases = tools.value.reduce((sum, t) => sum + t.purchaseCount, 0)
      stats.value.totalUses = tools.value.reduce((sum, t) => sum + t.useCount, 0)
      } else if (res && Array.isArray(res)) {
        // 兼容直接返回数组的情况
        tools.value = res as Tool[]
        stats.value.totalTools = tools.value.length
        stats.value.totalPurchases = tools.value.reduce((sum, t) => sum + t.purchaseCount, 0)
        stats.value.totalUses = tools.value.reduce((sum, t) => sum + t.useCount, 0)
      }
  } catch (e) {
    handleError(e, '获取工具列表失败')
  } finally {
    isFetchingTools = false
    loading.value = false
  }
}

// 获取分类列表
const fetchCategories = async () => {
  try {
    const res = await api.get('/Toolbox/categories')
    if (res) {
      categories.value = res as Category[]
    }
  } catch (e) {
    console.error('获取分类失败', e)
  }
}

const openModal = (item?: Tool) => {
  if (item) {
    isEdit.value = true
    form.value = {
      name: item.name,
      slug: item.slug,
      categoryId: item.categoryId || null,
      description: item.description || '',
      detailedDescription: '',
      coverImage: '',
      demoUrl: '',
      apiEndpoint: '',
      version: '1.0.0',
      price: item.price,
      originalPrice: item.originalPrice || null,
      isFree: item.isFree,
      isPremium: item.isPremium,
      status: item.status
    }
  } else {
    isEdit.value = false
    form.value = {
      name: '',
      slug: '',
      categoryId: null,
      description: '',
      detailedDescription: '',
      coverImage: '',
      demoUrl: '',
      apiEndpoint: '',
      version: '1.0.0',
      price: 0,
      originalPrice: null,
      isFree: false,
      isPremium: false,
      status: 'draft'
    }
  }
  showModal.value = true
}

const handleSave = async () => {
  if (!form.value.name || !form.value.slug) {
    warning('请填写工具名称和Slug')
    return
  }

  try {
    // TODO: 实现保存逻辑（需要后端API支持）
    success('保存成功')
    showModal.value = false
    fetchTools()
  } catch (e) {
    handleError(e, '保存失败')
  }
}

const handleDelete = async (item: Tool) => {
  if (!confirm(`确定要删除工具“${item.name}”吗？`)) {
    return
  }

  try {
    // TODO: 实现删除逻辑（需要后端API支持）
    success('删除成功')
    fetchTools()
  } catch (e) {
    handleError(e, '删除失败')
  }
}

onMounted(() => {
  fetchCategories()
  fetchTools()
})
</script>

<style scoped>
/* 使用 admin layout 的样式 */
</style>
