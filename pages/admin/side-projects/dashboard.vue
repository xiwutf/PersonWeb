<template>
  <div class="side-project-dashboard">
    <!-- 头部控制面板：深色卡片 + 玻璃态 -->
    <div class="dashboard-header-card">
      <div class="dashboard-header-content">
        <div class="dashboard-header-left">
          <h1 class="dashboard-title">副业数据看板</h1>
          <p class="dashboard-subtitle">最近 12 个月的副业收入、项目类型和技术栈一目了然</p>
        </div>
        <ClientOnly>
          <div class="dashboard-toolbar">
            <div class="toolbar-group">
              <div class="toolbar-label">
                <i class="fas fa-calendar-alt"></i>
                <span>时间范围</span>
              </div>
              <n-date-picker
                v-model:value="dateRange"
                type="daterange"
                clearable
                placeholder="选择时间范围"
                size="small"
                class="toolbar-input"
                @update:value="handleDateRangeChange"
              />
            </div>
            <div class="toolbar-divider"></div>
            <div class="toolbar-group">
              <div class="toolbar-label">
                <i class="fas fa-filter"></i>
                <span>项目类型</span>
              </div>
              <n-select
                v-model:value="selectedCategory"
                placeholder="全部类型"
                clearable
                size="small"
                :options="categoryOptions"
                class="toolbar-input"
                @update:value="handleCategoryChange"
              />
            </div>
            <div class="toolbar-divider"></div>
            <n-button 
              type="primary" 
              size="small"
              class="toolbar-refresh-btn"
              @click="loadAllData" 
              :loading="loading"
              :disabled="loading"
            >
              <template #icon>
                <i class="fas fa-sync-alt" :class="{ 'fa-spin': loading }"></i>
              </template>
              刷新数据
            </n-button>
          </div>
          <template #fallback>
            <div class="dashboard-toolbar">
              <div class="skeleton-item"></div>
              <div class="skeleton-item"></div>
              <div class="skeleton-item"></div>
            </div>
          </template>
        </ClientOnly>
      </div>
    </div>

    <!-- 数据提示 -->
    <ClientOnly>
      <n-alert
        v-if="!loading && hasNoData"
        type="info"
        title="暂无数据"
        class="dashboard-alert"
      >
        <template #header>
          <span>暂无副业项目数据</span>
        </template>
        <div>
          <p>当前没有副业项目数据，可能的原因：</p>
          <ul style="margin-left: 20px; margin-top: 8px;">
            <li>数据库表 <code>side_project</code> 尚未创建</li>
            <li>数据库中还没有副业项目记录</li>
            <li>请先执行数据库脚本 <code>database/side_project_tables.sql</code> 创建表并插入示例数据</li>
          </ul>
        </div>
      </n-alert>
    </ClientOnly>

    <!-- KPI 指标卡片区域 -->
    <div class="kpi-section" v-if="!hasNoData">
      <n-grid :cols="4" :x-gap="16" :y-gap="16">
        <n-grid-item>
          <div class="kpi-card" @click="handleKpiClick('income')">
            <div class="kpi-label">总收入</div>
            <div class="kpi-value">
              <span class="kpi-prefix">¥</span>
              <span class="kpi-number">{{ formatNumber(summary?.totalIncome || 0) }}</span>
            </div>
          </div>
        </n-grid-item>
        <n-grid-item>
          <div class="kpi-card" @click="handleKpiClick('projects')">
            <div class="kpi-label">项目总数</div>
            <div class="kpi-value">
              <span class="kpi-number">{{ summary?.totalProjects || 0 }}</span>
              <span class="kpi-suffix">个</span>
            </div>
          </div>
        </n-grid-item>
        <n-grid-item>
          <div class="kpi-card" @click="handleKpiClick('price')">
            <div class="kpi-label">平均项目价格</div>
            <div class="kpi-value">
              <span class="kpi-prefix">¥</span>
              <span class="kpi-number">{{ formatNumber(summary?.avgProjectPrice || 0) }}</span>
            </div>
          </div>
        </n-grid-item>
        <n-grid-item>
          <div class="kpi-card" @click="handleKpiClick('duration')">
            <div class="kpi-label">平均项目周期</div>
            <div class="kpi-value">
              <span class="kpi-number">{{ formatDuration(summary?.avgDurationDays || 0) }}</span>
              <span class="kpi-suffix">天</span>
            </div>
          </div>
        </n-grid-item>
      </n-grid>
    </div>

    <!-- 主体内容区域 -->
    <div class="main-content" v-if="!hasNoData">
      <n-grid :cols="12" :x-gap="16" :y-gap="16">
        <!-- 左侧：项目列表 -->
        <n-grid-item :span="8">
          <div class="content-card projects-card">
            <div class="card-header">
              <div class="card-header-left">
                <h3 class="card-title">最近项目</h3>
                <n-tag v-if="recentProjects.length > 0" size="small" type="info" class="card-count-tag">
                  {{ recentProjects.length }} 个项目
                </n-tag>
              </div>
              <div class="card-header-right">
                <div class="status-filter-group">
                  <button
                    v-for="option in statusOptions"
                    :key="option.value"
                    @click="selectedStatus = option.value; handleStatusChange()"
                    class="status-filter-btn"
                    :class="{ 'status-filter-btn-active': selectedStatus === option.value }"
                  >
                    {{ option.label }}
                  </button>
                </div>
              </div>
            </div>
            
            <div class="card-content">
              <div v-if="loading" class="loading-container">
                <n-spin size="large" />
                <p class="loading-text">加载中...</p>
              </div>
              <div v-else-if="recentProjects.length === 0" class="empty-container">
                <n-empty description="当前筛选条件下暂无副业项目">
                  <template #extra>
                    <n-button size="small" @click="loadAllData">刷新数据</n-button>
                  </template>
                </n-empty>
              </div>
              <div v-else class="projects-list">
                <div
                  v-for="project in recentProjects"
                  :key="project.id"
                  class="project-item"
                  @click="handleProjectClick(project.id)"
                >
                  <div class="project-main">
                    <div class="project-header">
                      <h4 class="project-title">{{ project.title }}</h4>
                      <div class="project-badges">
                        <n-tag v-if="project.isPublic" size="small" type="success" class="project-badge">
                          可展示
                        </n-tag>
                        <n-tag v-else size="small" type="default" class="project-badge">
                          不展示
                        </n-tag>
                      </div>
                    </div>
                    <p v-if="project.description" class="project-description">
                      {{ truncateText(project.description, 100) }}
                    </p>
                    <div class="project-tags">
                      <n-tag v-if="project.category" size="small" type="info" class="project-tag">
                        {{ project.category }}
                      </n-tag>
                      <template v-if="project.techStack">
                        <n-tag
                          v-for="tech in parseTechStack(project.techStack)"
                          :key="tech"
                          size="small"
                          type="warning"
                          class="project-tag"
                        >
                          {{ tech }}
                        </n-tag>
                      </template>
                    </div>
                  </div>
                  <div class="project-side">
                    <div class="project-price" v-if="project.priceFinal">
                      ¥{{ formatNumber(project.priceFinal) }}
                    </div>
                    <div class="project-status">
                      <n-tag :type="getStatusType(project.status)" size="small" class="status-tag">
                        {{ getStatusText(project.status) }}
                      </n-tag>
                    </div>
                    <div class="project-time" v-if="project.endTime">
                      {{ formatDate(project.endTime) }} 完成
                    </div>
                    <div class="project-time" v-else-if="project.startTime">
                      进行中 · 开始于 {{ formatDate(project.startTime) }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </n-grid-item>

        <!-- 右侧：统计卡片 -->
        <n-grid-item :span="4">
          <div class="stats-column">
            <!-- 项目类型概览 -->
            <div class="content-card stats-card">
              <div class="card-header">
                <h3 class="card-title">项目类型概览</h3>
              </div>
              <div class="card-content">
                <div v-if="categoryDistribution.length === 0" class="empty-stats">
                  <n-empty size="small" description="暂无数据" />
                </div>
                <div v-else class="stats-list">
                  <div
                    v-for="item in categoryDistribution.slice(0, 5)"
                    :key="item.category"
                    class="stats-item"
                  >
                    <div class="stats-item-left">
                      <span class="stats-label">{{ item.category || '未分类' }}</span>
                      <span class="stats-count">{{ item.count }} 个</span>
                    </div>
                    <div class="stats-item-right">
                      <span class="stats-income">¥{{ formatNumber(item.income) }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- 技术栈使用情况 -->
            <div class="content-card stats-card">
              <div class="card-header">
                <h3 class="card-title">技术栈使用情况</h3>
              </div>
              <div class="card-content">
                <div v-if="techDistribution.length === 0" class="empty-stats">
                  <n-empty size="small" description="暂无数据" />
                </div>
                <div v-else class="stats-list">
                  <div
                    v-for="item in techDistribution.slice(0, 5)"
                    :key="item.tech"
                    class="stats-item"
                  >
                    <div class="stats-item-left">
                      <span class="stats-label">{{ item.tech }}</span>
                      <span class="stats-count">{{ item.count }} 个</span>
                    </div>
                    <div class="stats-item-right">
                      <div class="progress-bar">
                        <div
                          class="progress-fill"
                          :style="{ width: `${(item.count / getMaxTechCount()) * 100}%` }"
                        ></div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </n-grid-item>
      </n-grid>
    </div>

    <!-- 图表分析区域 -->
    <div class="charts-section" v-if="!hasNoData">
      <n-grid :cols="12" :x-gap="16" :y-gap="16">
        <!-- 收入趋势折线图 -->
        <n-grid-item :span="8">
          <ClientOnly>
            <DashboardIncomeTrend :data="incomeTrend" :loading="loading" />
          </ClientOnly>
        </n-grid-item>
        
        <!-- 项目类型和技术栈饼图 -->
        <n-grid-item :span="4">
          <ClientOnly>
            <DashboardCategoryTechTabs 
              :category-data="categoryDistribution" 
              :tech-data="techDistribution" 
            />
          </ClientOnly>
        </n-grid-item>
        
        <!-- 客户来源柱状图 -->
        <n-grid-item :span="6">
          <ClientOnly>
            <DashboardClientSource :data="clientSource" />
          </ClientOnly>
        </n-grid-item>
        
        <!-- 项目周期分布柱状图 -->
        <n-grid-item :span="6">
          <ClientOnly>
            <DashboardDurationBuckets :data="durationBuckets" />
          </ClientOnly>
        </n-grid-item>
      </n-grid>
    </div>

    <!-- 数据表格区域 -->
    <div class="table-section" v-if="!hasNoData">
      <div class="content-card">
        <div class="card-header">
          <h3 class="card-title">项目数据表格</h3>
        </div>
        <div class="card-content">
          <ClientOnly>
            <n-data-table
              :columns="tableColumns"
              :data="recentProjects"
              :loading="loading"
              :pagination="false"
              :bordered="false"
              :single-line="false"
              class="projects-table"
            />
          </ClientOnly>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, h } from 'vue'
import { useRouter } from 'vue-router'
import { 
  NDatePicker, 
  NSelect, 
  NButton, 
  NAlert, 
  NGrid, 
  NGridItem, 
  NSpin, 
  NEmpty,
  NCard,
  NStatistic,
  NList,
  NListItem,
  NThing,
  NTag,
  NDataTable,
  zhCN,
  type DataTableColumns
} from 'naive-ui'
import DashboardIncomeTrend from '~/components/side-projects/DashboardIncomeTrend.vue'
import DashboardCategoryTechTabs from '~/components/side-projects/DashboardCategoryTechTabs.vue'
import DashboardClientSource from '~/components/side-projects/DashboardClientSource.vue'
import DashboardDurationBuckets from '~/components/side-projects/DashboardDurationBuckets.vue'
import { useApi } from '~/composables/useApi'
import type {
  ProjectDashboardSummaryDto,
  IncomeTrendPointDto,
  CategoryDistributionItemDto,
  TechStackDistributionItemDto,
  ClientSourceItemDto,
  DurationBucketItemDto,
  SideProject,
  SideProjectListResponse
} from '~/types/api'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // 禁用 SSR，避免 Naive UI 组件在服务端渲染时出错
})

const api = useApi()
const router = useRouter()

// 数据状态
const loading = ref(false)
const summary = ref<ProjectDashboardSummaryDto | null>(null)
const incomeTrend = ref<IncomeTrendPointDto[]>([])
const categoryDistribution = ref<CategoryDistributionItemDto[]>([])
const techDistribution = ref<TechStackDistributionItemDto[]>([])
const clientSource = ref<ClientSourceItemDto[]>([])
const durationBuckets = ref<DurationBucketItemDto[]>([])
const recentProjects = ref<SideProject[]>([])

// 筛选条件
const dateRange = ref<[number, number] | null>(null)
const selectedCategory = ref<string | null>(null)
const selectedStatus = ref<number | null>(null) // null=全部, 0=进行中, 1=已完成, 2=待付款, 3=已取消

// 状态选项
const statusOptions = [
  { label: '全部', value: null },
  { label: '进行中', value: 0 },
  { label: '已完成', value: 1 },
  { label: '待付款', value: 2 },
  { label: '已取消', value: 3 }
]

// 检查是否有数据
const hasNoData = computed(() => {
  const hasSummaryData = summary.value && summary.value.totalProjects > 0
  return !hasSummaryData
})

// 计算时间范围
const timeRange = computed(() => {
  if (dateRange.value) {
    return {
      from: new Date(dateRange.value[0]),
      to: new Date(dateRange.value[1])
    }
  }
  return { from: null, to: null }
})

// 项目类型选项
const categoryOptions = computed(() => {
  const categories = new Set<string>()
  categoryDistribution.value.forEach(item => {
    if (item.category) {
      categories.add(item.category)
    }
  })
  return Array.from(categories).map(cat => ({
    label: cat,
    value: cat
  }))
})

// 获取最大技术栈数量（用于进度条）
const getMaxTechCount = () => {
  if (techDistribution.value.length === 0) return 1
  return Math.max(...techDistribution.value.map(item => item.count))
}

// 解析技术栈字符串
const parseTechStack = (techStack: string | null | undefined): string[] => {
  if (!techStack) return []
  return techStack.split(',').map(t => t.trim()).filter(t => t.length > 0).slice(0, 3)
}

// 截断文本
const truncateText = (text: string, maxLength: number): string => {
  if (!text) return ''
  if (text.length <= maxLength) return text
  return text.substring(0, maxLength) + '...'
}

// 获取状态类型
const getStatusType = (status: number): 'default' | 'info' | 'success' | 'warning' | 'error' => {
  switch (status) {
    case 0: return 'info' // 进行中
    case 1: return 'success' // 已完成
    case 2: return 'warning' // 待付款
    case 3: return 'error' // 已取消
    default: return 'default'
  }
}

// 获取状态文本
const getStatusText = (status: number): string => {
  switch (status) {
    case 0: return '进行中'
    case 1: return '已完成'
    case 2: return '待付款'
    case 3: return '已取消'
    default: return '未知'
  }
}

// API 调用函数
const fetchSummary = async () => {
  try {
    const params: any = {}
    if (timeRange.value.from && timeRange.value.to) {
      params.from = timeRange.value.from.toISOString()
      params.to = timeRange.value.to.toISOString()
    }
    const res = await api.get<any>('/side-projects/dashboard/summary', { params })
    
    if (res && typeof res === 'object') {
      summary.value = {
        totalIncome: res.totalIncome ?? res.TotalIncome ?? 0,
        totalProjects: res.totalProjects ?? res.TotalProjects ?? 0,
        avgProjectPrice: res.avgProjectPrice ?? res.AvgProjectPrice ?? 0,
        avgDurationDays: res.avgDurationDays ?? res.AvgDurationDays ?? 0
      }
    } else {
      summary.value = null
    }
  } catch (e: any) {
    console.error('获取汇总数据失败:', e)
    summary.value = null
  }
}

const fetchIncomeTrend = async () => {
  try {
    const params: any = { granularity: 'month' }
    if (timeRange.value.from && timeRange.value.to) {
      params.from = timeRange.value.from.toISOString()
      params.to = timeRange.value.to.toISOString()
    }
    const res = await api.get<any>('/side-projects/dashboard/income-trend', { params })
    if (Array.isArray(res)) {
      incomeTrend.value = res.map(item => ({
        date: item.date ?? item.Date ?? '',
        income: item.income ?? item.Income ?? 0
      }))
    } else if (res && Array.isArray(res.data)) {
      incomeTrend.value = res.data.map((item: any) => ({
        date: item.date ?? item.Date ?? '',
        income: item.income ?? item.Income ?? 0
      }))
    } else {
      incomeTrend.value = []
    }
  } catch (e) {
    console.error('获取收入趋势失败:', e)
    incomeTrend.value = []
  }
}

const fetchCategoryDistribution = async () => {
  try {
    const params: any = {}
    if (timeRange.value.from && timeRange.value.to) {
      params.from = timeRange.value.from.toISOString()
      params.to = timeRange.value.to.toISOString()
    }
    const res = await api.get<any>('/side-projects/dashboard/category-distribution', { params })
    if (Array.isArray(res)) {
      categoryDistribution.value = res.map((item: any) => ({
        category: item.category ?? item.Category ?? '',
        count: item.count ?? item.Count ?? 0,
        income: item.income ?? item.Income ?? 0
      }))
    } else {
      categoryDistribution.value = []
    }
  } catch (e) {
    console.error('获取类型分布失败:', e)
    categoryDistribution.value = []
  }
}

const fetchTechDistribution = async () => {
  try {
    const params: any = {}
    if (timeRange.value.from && timeRange.value.to) {
      params.from = timeRange.value.from.toISOString()
      params.to = timeRange.value.to.toISOString()
    }
    const res = await api.get<any>('/side-projects/dashboard/tech-distribution', { params })
    if (Array.isArray(res)) {
      techDistribution.value = res.map((item: any) => ({
        tech: item.tech ?? item.Tech ?? '',
        count: item.count ?? item.Count ?? 0,
        income: item.income ?? item.Income ?? 0
      }))
    } else {
      techDistribution.value = []
    }
  } catch (e) {
    console.error('获取技术栈分布失败:', e)
    techDistribution.value = []
  }
}

const fetchClientSource = async () => {
  try {
    const params: any = {}
    if (timeRange.value.from && timeRange.value.to) {
      params.from = timeRange.value.from.toISOString()
      params.to = timeRange.value.to.toISOString()
    }
    const res = await api.get<any>('/side-projects/dashboard/client-source', { params })
    if (Array.isArray(res)) {
      clientSource.value = res.map((item: any) => ({
        source: item.source ?? item.Source ?? '',
        count: item.count ?? item.Count ?? 0,
        income: item.income ?? item.Income ?? 0
      }))
    } else {
      clientSource.value = []
    }
  } catch (e) {
    console.error('获取客户来源失败:', e)
    clientSource.value = []
  }
}

const fetchDurationBuckets = async () => {
  try {
    const params: any = {}
    if (timeRange.value.from && timeRange.value.to) {
      params.from = timeRange.value.from.toISOString()
      params.to = timeRange.value.to.toISOString()
    }
    const res = await api.get<any>('/side-projects/dashboard/duration-buckets', { params })
    if (Array.isArray(res)) {
      durationBuckets.value = res.map((item: any) => ({
        bucketName: item.bucketName ?? item.BucketName ?? '',
        count: item.count ?? item.Count ?? 0
      }))
    } else {
      durationBuckets.value = []
    }
  } catch (e) {
    console.error('获取周期分布失败:', e)
    durationBuckets.value = []
  }
}

const fetchRecentProjects = async () => {
  try {
    const params: any = {
      page: 1,
      pageSize: 10
    }
    if (selectedStatus.value !== null) {
      params.status = selectedStatus.value
    }
    if (selectedCategory.value) {
      params.category = selectedCategory.value
    }
    const res = await api.get<any>('/side-projects', { params })
    
    if (res && typeof res === 'object') {
      if ('list' in res && Array.isArray(res.list)) {
        recentProjects.value = res.list
      } else if ('List' in res && Array.isArray(res.List)) {
        recentProjects.value = res.List
      } else if (Array.isArray(res)) {
        recentProjects.value = res
      } else {
        const arrayKey = Object.keys(res).find(key => Array.isArray(res[key]))
        if (arrayKey) {
          recentProjects.value = res[arrayKey]
        } else {
          recentProjects.value = []
        }
      }
    } else {
      recentProjects.value = []
    }
  } catch (e) {
    console.error('获取最近项目失败:', e)
    recentProjects.value = []
  }
}

// 格式化数字（添加千分位）
const formatNumber = (num: number) => {
  if (typeof num !== 'number' || isNaN(num)) return '0'
  return num.toLocaleString('zh-CN', { minimumFractionDigits: 0, maximumFractionDigits: 2 })
}

// 格式化时长
const formatDuration = (days: number) => {
  if (typeof days !== 'number' || isNaN(days)) return '0'
  return days.toFixed(1)
}

// 格式化日期
const formatDate = (dateStr: string | null | undefined) => {
  if (!dateStr) return '-'
  try {
    const date = new Date(dateStr)
    return date.toLocaleDateString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    })
  } catch {
    return dateStr
  }
}

// 加载所有数据
const loadAllData = async () => {
  loading.value = true
  try {
    await Promise.all([
      fetchSummary(),
      fetchIncomeTrend(),
      fetchCategoryDistribution(),
      fetchTechDistribution(),
      fetchClientSource(),
      fetchDurationBuckets(),
      fetchRecentProjects()
    ])
  } finally {
    loading.value = false
  }
}

// 筛选变化处理
const handleDateRangeChange = () => {
  loadAllData()
}

const handleCategoryChange = () => {
  fetchRecentProjects()
}

const handleStatusChange = () => {
  fetchRecentProjects()
}

// KPI 卡片点击
const handleKpiClick = (type: string) => {
  // 预留：点击 KPI 卡片可以筛选数据
}

// 项目点击
const handleProjectClick = (id: number) => {
  // 预留：跳转到项目详情页
  router.push(`/admin/side-projects/${id}`)
}

// 表格列定义
const tableColumns: DataTableColumns<SideProject> = [
  {
    title: '项目名称',
    key: 'title',
    width: 200,
    ellipsis: {
      tooltip: true
    }
  },
  {
    title: '项目类型',
    key: 'category',
    width: 120,
    render: (row) => row.category || '未分类'
  },
  {
    title: '技术栈',
    key: 'techStack',
    width: 200,
    render: (row) => {
      if (!row.techStack) return '-'
      const techs = parseTechStack(row.techStack)
      return techs.length > 0 ? techs.join(', ') : '-'
    }
  },
  {
    title: '客户名称',
    key: 'clientName',
    width: 150,
    ellipsis: {
      tooltip: true
    },
    render: (row) => row.clientName || '-'
  },
  {
    title: '客户来源',
    key: 'source',
    width: 120,
    render: (row) => row.source || '-'
  },
  {
    title: '项目金额',
    key: 'priceFinal',
    width: 120,
    align: 'right',
    render: (row) => row.priceFinal ? `¥${formatNumber(row.priceFinal)}` : '-'
  },
  {
    title: '状态',
    key: 'status',
    width: 100,
    render: (row) => {
      return h(NTag, {
        type: getStatusType(row.status),
        size: 'small'
      }, { default: () => getStatusText(row.status) })
    }
  },
  {
    title: '开始时间',
    key: 'startTime',
    width: 120,
    render: (row) => formatDate(row.startTime)
  },
  {
    title: '结束时间',
    key: 'endTime',
    width: 120,
    render: (row) => formatDate(row.endTime)
  },
  {
    title: '是否公开',
    key: 'isPublic',
    width: 100,
    render: (row) => {
      return h(NTag, {
        type: row.isPublic ? 'success' : 'default',
        size: 'small'
      }, { default: () => row.isPublic ? '可展示' : '不展示' })
    }
  }
]

// 生命周期
onMounted(() => {
  loadAllData()
})
</script>

<style scoped>
/* 主容器 */
.side-project-dashboard {
  padding: var(--spacing-lg);
  background-color: var(--color-bg-page);
  min-height: 100vh;
}

/* 头部控制面板 - 深色玻璃态卡片 */
.dashboard-header-card {
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
  margin-bottom: var(--spacing-xl);
  box-shadow: var(--shadow-md);
  backdrop-filter: blur(10px);
  background: linear-gradient(135deg, var(--color-bg-elevated) 0%, rgba(30, 41, 59, 0.8) 100%);
}

.dashboard-header-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: var(--spacing-lg);
  flex-wrap: wrap;
}

.dashboard-header-left {
  flex: 1;
  min-width: 300px;
}

.dashboard-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-xs) 0;
  line-height: 1.2;
}

.dashboard-subtitle {
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
  margin: 0;
  line-height: 1.5;
}

.dashboard-toolbar {
  display: flex;
  align-items: center;
  gap: 0;
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xs);
  box-shadow: var(--shadow-sm);
  backdrop-filter: blur(10px);
}

.toolbar-group {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: 0 var(--spacing-sm);
}

.toolbar-label {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
  white-space: nowrap;
  font-weight: 500;
}

.toolbar-label i {
  font-size: var(--font-size-xs);
  opacity: 0.7;
}

.toolbar-input {
  min-width: 180px;
}

.toolbar-divider {
  width: 1px;
  height: 24px;
  background: var(--color-border-subtle);
  margin: 0 var(--spacing-xs);
}

.toolbar-refresh-btn {
  margin-left: var(--spacing-xs);
  padding: 0 var(--spacing-md);
  height: 32px;
  font-weight: 500;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  transition: all 0.2s ease;
}

.toolbar-refresh-btn:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

.toolbar-refresh-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.skeleton-item {
  width: 150px;
  height: 32px;
  background: var(--color-bg-card);
  border-radius: var(--radius-md);
  opacity: 0.3;
}

/* KPI 卡片区域 */
.kpi-section {
  margin-bottom: var(--spacing-xl);
}

.kpi-card {
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.8) 0%, rgba(15, 23, 42, 0.9) 100%);
  border: 1px solid rgba(59, 130, 246, 0.2);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3), 0 0 0 1px rgba(59, 130, 246, 0.1);
  backdrop-filter: blur(10px);
  position: relative;
  overflow: hidden;
}

.kpi-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, #3b82f6, #06b6d4);
  opacity: 0.9;
  transition: opacity 0.3s ease;
}

.kpi-card:nth-child(1)::before {
  background: linear-gradient(90deg, #10b981, #06b6d4);
}

.kpi-card:nth-child(2)::before {
  background: linear-gradient(90deg, #3b82f6, #6366f1);
}

.kpi-card:nth-child(3)::before {
  background: linear-gradient(90deg, #f59e0b, #f97316);
}

.kpi-card:nth-child(4)::before {
  background: linear-gradient(90deg, #8b5cf6, #a855f7);
}

.kpi-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(59, 130, 246, 0.3), 0 0 0 1px rgba(59, 130, 246, 0.3);
  border-color: rgba(59, 130, 246, 0.5);
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.95) 0%, rgba(15, 23, 42, 1) 100%);
}

.kpi-label {
  font-size: var(--font-size-sm);
  color: rgba(203, 213, 225, 0.7);
  margin-bottom: var(--spacing-sm);
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.kpi-value {
  display: flex;
  align-items: baseline;
  gap: var(--spacing-xs);
}

.kpi-prefix,
.kpi-suffix {
  font-size: var(--font-size-h4);
  color: rgba(203, 213, 225, 0.6);
  font-weight: 500;
}

.kpi-number {
  font-size: var(--font-size-h1);
  font-weight: 700;
  background: linear-gradient(135deg, #ffffff 0%, rgba(203, 213, 225, 0.9) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  line-height: 1;
}

/* 主体内容区域 */
.main-content {
  margin-bottom: var(--spacing-xl);
}

/* 内容卡片 - 深色玻璃态 */
.content-card {
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  backdrop-filter: blur(10px);
  overflow: hidden;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border-subtle);
}

.card-header-left {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.card-title {
  font-size: var(--font-size-h4);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

.card-count-tag {
  margin-left: var(--spacing-xs);
}

.card-content {
  flex: 1;
  padding: var(--spacing-lg);
  overflow-y: auto;
  max-height: 600px;
}

/* 项目列表 */
.projects-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.project-item {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  padding: var(--spacing-lg);
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  justify-content: space-between;
  gap: var(--spacing-lg);
}

.project-item:hover {
  background: var(--color-bg-elevated);
  border-color: var(--color-primary);
  box-shadow: var(--shadow-sm);
  transform: translateX(4px);
}

.project-main {
  flex: 1;
  min-width: 0;
}

.project-header {
  display: flex;
  justify-content: space-between;
  align-items: start;
  margin-bottom: var(--spacing-sm);
  gap: var(--spacing-sm);
}

.project-title {
  font-size: var(--font-size-h4);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
  flex: 1;
}

.project-badges {
  display: flex;
  gap: var(--spacing-xs);
  flex-shrink: 0;
}

.project-description {
  font-size: var(--font-size-body);
  color: var(--color-text-muted);
  margin: 0 0 var(--spacing-sm) 0;
  line-height: 1.6;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.project-tags {
  display: flex;
  gap: var(--spacing-xs);
  flex-wrap: wrap;
}

.project-side {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: var(--spacing-sm);
  flex-shrink: 0;
  min-width: 120px;
}

.project-price {
  font-size: var(--font-size-h4);
  font-weight: 700;
  color: var(--color-primary);
}

.project-status {
  display: flex;
  align-items: center;
}

.project-time {
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
}

/* 统计卡片 */
.stats-column {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.stats-card {
  min-height: 200px;
}

.stats-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.stats-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-sm) 0;
  border-bottom: 1px solid var(--color-border-subtle);
}

.stats-item:last-child {
  border-bottom: none;
}

.stats-item-left {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
  flex: 1;
}

.stats-label {
  font-size: var(--font-size-body);
  color: var(--color-text-main);
  font-weight: 500;
}

.stats-count {
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
}

.stats-item-right {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.stats-income {
  font-size: var(--font-size-h4);
  font-weight: 600;
  color: var(--color-primary);
}

.progress-bar {
  width: 80px;
  height: 4px;
  background: var(--color-bg-card);
  border-radius: var(--radius-sm);
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--color-primary), var(--color-primary-hover));
  border-radius: var(--radius-sm);
  transition: width 0.3s ease;
}

/* 加载和空状态 */
.loading-container,
.empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-2xl);
  min-height: 200px;
}

.loading-text {
  margin-top: var(--spacing-md);
  color: var(--color-text-muted);
  font-size: var(--font-size-body);
}

.empty-stats {
  padding: var(--spacing-lg);
  text-align: center;
}

/* 图表区域 */
.charts-section {
  margin-top: var(--spacing-xl);
}

/* 表格区域 */
.table-section {
  margin-top: var(--spacing-xl);
}

.projects-table {
  background: transparent;
}

.projects-table :deep(.n-data-table) {
  background: transparent;
}

.projects-table :deep(.n-data-table-th) {
  background: var(--color-bg-elevated);
  border-bottom: 1px solid var(--color-border-subtle);
  color: var(--color-text-main);
  font-weight: 600;
}

.projects-table :deep(.n-data-table-td) {
  border-bottom: 1px solid var(--color-border-subtle);
  background: var(--color-bg-card);
}

.projects-table :deep(.n-data-table-tr:hover .n-data-table-td) {
  background: var(--color-bg-elevated);
}

.projects-table :deep(.n-data-table-tbody .n-data-table-tr) {
  cursor: pointer;
  transition: all 0.2s ease;
}

.projects-table :deep(.n-data-table-tbody .n-data-table-tr:hover) {
  transform: translateX(2px);
}

/* 响应式 */
@media (max-width: 1200px) {
  .main-content :deep(.n-grid) {
    grid-template-columns: 1fr !important;
  }
  
  .main-content :deep(.n-grid-item[span="8"]) {
    grid-column: 1 !important;
  }
  
  .main-content :deep(.n-grid-item[span="4"]) {
    grid-column: 1 !important;
  }
}

@media (max-width: 768px) {
  .dashboard-header-content {
    flex-direction: column;
  }
  
  .dashboard-toolbar {
    width: 100%;
    flex-direction: column;
  }
  
  .dashboard-toolbar > * {
    width: 100%;
  }
  
  .kpi-section :deep(.n-grid) {
    grid-template-columns: repeat(2, 1fr) !important;
  }
  
  .project-item {
    flex-direction: column;
  }
  
  .project-side {
    align-items: flex-start;
    width: 100%;
  }
}

/* 状态筛选按钮组 */
.status-filter-group {
  display: flex;
  gap: var(--spacing-xs);
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  padding: 2px;
}

.status-filter-btn {
  padding: var(--spacing-xs) var(--spacing-sm);
  border: none;
  background: transparent;
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
  border-radius: var(--radius-sm);
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
  font-weight: 400;
}

.status-filter-btn:hover {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}

.status-filter-btn-active {
  background: var(--color-primary);
  color: #ffffff;
  font-weight: 500;
}

.status-filter-btn-active:hover {
  background: var(--color-primary-hover);
}

/* 滚动条样式 */
.card-content::-webkit-scrollbar {
  width: 6px;
}

.card-content::-webkit-scrollbar-track {
  background: var(--color-bg-card);
  border-radius: var(--radius-sm);
}

.card-content::-webkit-scrollbar-thumb {
  background: var(--color-border-default);
  border-radius: var(--radius-sm);
}

.card-content::-webkit-scrollbar-thumb:hover {
  background: var(--color-border-strong);
}
</style>
