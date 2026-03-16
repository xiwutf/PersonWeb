<template>
  <div class="space-y-6">
    <div class="page-header">
      <h1 class="page-title">���߹���</h1>
      <button @click="openModal()" class="btn-primary">
        <i class="fas fa-plus mr-2"></i>��������
      </button>
    </div>

    <!-- ͳ�ƿ�Ƭ -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">�ܹ�����</div>
        <div class="text-2xl font-bold text-gray-800 dark:text-var(--color-bg-light, white)">{{ stats.total }}</div>
      </div>
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">�ѷ���</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats.published }}</div>
      </div>
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">�ݸ�</div>
        <div class="text-2xl font-bold text-yellow-600 dark:text-yellow-400">{{ stats.draft }}</div>
      </div>
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">�ѹ鵵</div>
        <div class="text-2xl font-bold text-gray-600 dark:text-gray-400">{{ stats.archived }}</div>
      </div>
    </div>

    <!-- ͼ������ -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
      <!-- ״̬�ֲ���ͼ -->
      <div class="card p-6">
        <h3 class="text-lg font-bold text-gray-800 dark:text-var(--color-bg-light, white) mb-4">状态分布</h3>
        <ClientOnly>
          <div v-if="statusChartOption" class="h-64">
            <v-chart :option="statusChartOption" autoresize class="w-full h-full" />
          </div>
          <div v-else class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
            ��������
          </div>
        </ClientOnly>
      </div>

      <!-- �۸�ֲ���״ͼ -->
      <div class="card p-6">
        <h3 class="text-lg font-bold text-gray-800 dark:text-var(--color-bg-light, white) mb-4">价格分布</h3>
        <ClientOnly>
          <div v-if="priceChartOption" class="h-64">
            <v-chart :option="priceChartOption" autoresize class="w-full h-full" />
          </div>
          <div v-else class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
            ��������
          </div>
        </ClientOnly>
      </div>
    </div>

    <!-- �����б� -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="tool in tools" :key="tool.id" class="card p-6 flex flex-col">
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-lg bg-gray-100 dark:bg-gray-700 flex items-center justify-center text-2xl">
              {{ tool.icon || '???' }}
            </div>
            <div>
              <h3 class="font-bold text-gray-800 dark:text-var(--color-bg-light, white)">{{ tool.name }}</h3>
              <div class="flex items-center gap-2 mt-1">
                <span class="text-xs px-2 py-1 rounded" :class="{
                  'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300': tool.status === 'published',
                  'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-800 dark:text-yellow-300': tool.status === 'draft',
                  'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300': tool.status === 'archived'
                }">
                  {{ tool.status === 'published' ? '�ѷ���' : tool.status === 'draft' ? '�ݸ�' : '�ѹ鵵' }}
                </span>
                <span v-if="tool.category" class="text-xs text-gray-500 dark:text-gray-400">
                  {{ tool.category.name }}
                </span>
              </div>
            </div>
          </div>
          <div class="flex gap-2">
            <button @click="openModal(tool)" class="text-gray-400 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
              <span class="sr-only">�༭</span>
              ??
            </button>
            <button @click="handleDelete(tool)" class="text-gray-400 hover:text-red-600 dark:hover:text-red-400 transition-colors">
              <span class="sr-only">ɾ��</span>
              ???
            </button>
          </div>
        </div>
        <p class="text-gray-600 dark:text-gray-400 text-sm mb-4 flex-1">{{ tool.description || '��������' }}</p>
        <div class="flex items-center justify-between text-xs text-gray-500 dark:text-gray-500">
          <span>�۸�: {{ tool.isFree ? '���' : `��${tool.price}` }}</span>
          <span v-if="tool.demoUrl">
            <a :href="tool.demoUrl" target="_blank" class="btn-link btn-link--blue">��ʾ</a>
          </span>
        </div>
      </div>
      
      <!-- ��״̬ -->
      <div v-if="tools.length === 0" class="col-span-full text-center py-12 empty-state card border-dashed">
        ���޹�������
      </div>
    </div>

    <!-- �༭/�½����� -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-content max-w-lg max-h-[90vh] overflow-y-auto">
        <div class="modal-header">
          <h3 class="modal-title">
            {{ isEdit ? '�༭����' : '��������' }}
          </h3>
        </div>
        
        <div class="modal-body space-y-4">
          <div class="form-group">
            <label class="form-label">���� *</label>
            <input v-model="form.name" type="text" class="form-input" placeholder="���磺JSON ��ʽ������" required>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div class="form-group">
              <label class="form-label">���� (Slug) *</label>
              <input v-model="form.slug" type="text" class="form-input" placeholder="json-formatter" required>
            </div>
            <div class="form-group">
              <label class="form-label">ͼ�� (Emoji)</label>
              <input v-model="form.icon" type="text" class="form-input" placeholder="??">
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">����</label>
            <textarea v-model="form.description" class="form-textarea h-24" placeholder="��������ù��ߵ�����..."></textarea>
          </div>
          <div class="form-group">
            <label class="form-label">��ʾ��ַ</label>
            <input v-model="form.demoUrl" type="text" class="form-input" placeholder="https://...">
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div class="form-group">
              <label class="form-label">�۸�</label>
              <input v-model.number="form.price" type="number" step="0.01" class="form-input" placeholder="0.00">
            </div>
            <div class="form-group">
              <label class="form-label">ԭ��</label>
              <input v-model.number="form.originalPrice" type="number" step="0.01" class="form-input" placeholder="0.00">
            </div>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div class="form-group">
              <label class="form-label">״̬</label>
              <select v-model="form.status" class="form-input">
                <option value="draft">�ݸ�</option>
                <option value="published">�ѷ���</option>
                <option value="archived">�ѹ鵵</option>
              </select>
            </div>
            <div class="form-group">
              <label class="flex items-center gap-2">
                <input v-model="form.isFree" type="checkbox" class="form-checkbox">
                <span>��ѹ���</span>
              </label>
              <label class="flex items-center gap-2 mt-2">
                <input v-model="form.isPremium" type="checkbox" class="form-checkbox">
                <span>�߼�����</span>
              </label>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="showModal = false" class="btn-secondary">ȡ��</button>
          <button @click="handleSave" class="btn-primary">����</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Tool {
  id: number
  name: string
  slug: string
  icon?: string
  description?: string
  coverImage?: string
  demoUrl?: string
  price: number
  originalPrice?: number
  isFree: boolean
  isPremium: boolean
  status: string
  category?: {
    id: number
    name: string
    slug: string
  }
}

import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { PieChart, BarChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components'
import VChart from 'vue-echarts'

// ע�� ECharts ���
use([
  CanvasRenderer,
  PieChart,
  BarChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
])

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // ���� SSR������ ECharts �ڷ������Ⱦʱ����
})

const api = useApi()
const showModal = ref(false)
const isEdit = ref(false)
const editingToolId = ref<number | null>(null)
const form = ref({
  name: '',
  slug: '',
  icon: '',
  description: '',
  demoUrl: '',
  price: 0,
  originalPrice: null as number | null,
  isFree: false,
  isPremium: false,
  status: 'draft'
})

// ʹ�� useAsyncData ���� SSR/�ͻ����ظ�����
const { data: toolsData, pending: toolsPending, refresh: refreshTools } = useAsyncData(
  'admin-tools-list',
  async () => {
    try {
      // ʹ�ú�� API ��ȡ���ݿ��еĹ������ݣ�ȥ�� /api/ ǰ׺���� useApi ��ȷ���Ӻ�� baseURL��
      const res = await api.get('/Toolbox/admin/list?pageSize=1000')
      if (res && res.tools) {
        return res.tools as Tool[]
      } else if (res && res.data && res.data.tools) {
        // ���� ApiResponse ��ʽ
        return res.data.tools as Tool[]
      } else if (Array.isArray(res)) {
        // ����ֱ�ӷ�����������
        return res as Tool[]
      } else {
        return [] as Tool[]
      }
    } catch (e: unknown) {
      if (process.env.NODE_ENV === 'development') {
        console.error('Failed to fetch tools:', e)
      }
      return [] as Tool[]
    }
  },
  {
    server: true,  // �ڷ����Ҳִ��
    default: () => [] as Tool[]  // Ĭ��ֵ
  }
)

// �� useAsyncData ���ص����ݰ󶨵���Ӧʽ����
const tools = computed(() => toolsData.value || [])
const loading = computed(() => toolsPending.value)

// ͳ������
const stats = computed(() => {
  const total = tools.value.length
  const published = tools.value.filter(t => t.status === 'published').length
  const draft = tools.value.filter(t => t.status === 'draft').length
  const archived = tools.value.filter(t => t.status === 'archived').length
  
  return {
    total,
    published,
    draft,
    archived
  }
})

// ��ȡ CSS �����ĸ�������
const getCssVar = (varName: string): string => {
  if (process.client) {
    const root = document.documentElement
    return getComputedStyle(root).getPropertyValue(varName).trim()
  }
  return ''
}

// ״̬�ֲ���ͼ����
const statusChartOption = computed(() => {
  if (stats.value.total === 0) return null
  
  const textColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const bgColor = getCssVar('--color-bg-card') || getCssVar('--n-card-color')
  
  return {
    tooltip: {
      trigger: 'item',
      formatter: '{b}: {c} ({d}%)'
    },
    legend: {
      orient: 'vertical',
      left: 'left',
      textStyle: {
        color: textColor
      }
    },
    series: [
      {
        name: '����״̬',
        type: 'pie',
        radius: ['40%', '70%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 8,
          borderColor: bgColor,
          borderWidth: 2
        },
        label: {
          show: true,
          formatter: '{b}\n{d}%',
          color: textColor
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 14,
            fontWeight: 'bold'
          }
        },
        data: [
          { value: stats.value.published, name: '�ѷ���', itemStyle: { color: getCssVar('--color-success') || 'var(--color-success)' } },
          { value: stats.value.draft, name: '�ݸ�', itemStyle: { color: getCssVar('--color-warning') || 'var(--color-warning)' } },
          { value: stats.value.archived, name: '�ѹ鵵', itemStyle: { color: getCssVar('--color-text-muted') || 'var(--color-text-sec)' } }
        ]
      }
    ]
  }
})

// �۸�ֲ���״ͼ����
const priceChartOption = computed(() => {
  if (tools.value.length === 0) return null
  
  const textColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const gridColor = getCssVar('--color-border-subtle') || getCssVar('--n-border-color')
  
  // ͳ�Ƽ۸�����
  const freeCount = tools.value.filter(t => t.isFree).length
  const paidTools = tools.value.filter(t => !t.isFree && t.price > 0)
  const priceRanges = [
    { name: '���', count: freeCount },
    { name: '0-50Ԫ', count: paidTools.filter(t => t.price <= 50).length },
    { name: '50-100Ԫ', count: paidTools.filter(t => t.price > 50 && t.price <= 100).length },
    { name: '100-200Ԫ', count: paidTools.filter(t => t.price > 100 && t.price <= 200).length },
    { name: '200Ԫ����', count: paidTools.filter(t => t.price > 200).length }
  ].filter(r => r.count > 0)
  
  return {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: priceRanges.map(r => r.name),
      axisLabel: {
        color: textColor
      },
      axisLine: {
        lineStyle: {
          color: gridColor
        }
      }
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        color: textColor
      },
      axisLine: {
        lineStyle: {
          color: gridColor
        }
      },
      splitLine: {
        lineStyle: {
          color: gridColor
        }
      }
    },
    series: [
      {
        name: '��������',
        type: 'bar',
        data: priceRanges.map(r => r.count),
        itemStyle: {
          color: {
            type: 'linear',
            x: 0,
            y: 0,
            x2: 0,
            y2: 1,
            colorStops: [
              { offset: 0, color: getCssVar('--color-primary') || 'var(--color-primary)' },
              { offset: 1, color: getCssVar('--color-secondary') || getCssVar('--color-primary-hover') || 'var(--color-purple-500)' }
            ]
          },
          borderRadius: [4, 4, 0, 0]
        }
      }
    ]
  }
})

// �ֶ�ˢ�º��������ڱ���/ɾ����ˢ�£�
const fetchTools = async () => {
  await refreshTools()
}

const openModal = (item?: Tool) => {
  if (item) {
    isEdit.value = true
    editingToolId.value = item.id
    form.value = {
      name: item.name,
      slug: item.slug,
      icon: item.icon || '',
      description: item.description || '',
      demoUrl: item.demoUrl || '',
      price: item.price,
      originalPrice: item.originalPrice || null,
      isFree: item.isFree,
      isPremium: item.isPremium,
      status: item.status
    }
  } else {
    isEdit.value = false
    editingToolId.value = null
    form.value = {
      name: '',
      slug: '',
      icon: '',
      description: '',
      demoUrl: '',
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
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  if (!form.value.name) {
    warning('�����빤������')
    return
  }
  
  try {
    // ʹ�ú�� API��ȥ�� /api/ ǰ׺���� useApi ��ȷ���Ӻ�� baseURL��
    if (isEdit.value && editingToolId.value) {
      await api.put(`/Toolbox/${editingToolId.value}`, form.value)
    } else {
      await api.post('/Toolbox', form.value)
    }
    success('����ɹ�')
    showModal.value = false
    editingToolId.value = null
    fetchTools()
  } catch (e: unknown) {
    handleError(e, '����ʧ��')
  }
}

const handleDelete = async (item: Tool) => {
  if (!confirm(`ȷ��Ҫɾ������ "${item.name}" ��`)) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    // ʹ�ú�� API��ȥ�� /api/ ǰ׺���� useApi ��ȷ���Ӻ�� baseURL��
    await api.del(`/Toolbox/${item.id}`)
    success('ɾ���ɹ�')
    fetchTools()
  } catch (e: unknown) {
    handleError(e, 'ɾ��ʧ��')
  }
}

// ʹ�� useAsyncData �󣬲���Ҫ�� onMounted �е��ã����ݻ��Զ�����
</script>
