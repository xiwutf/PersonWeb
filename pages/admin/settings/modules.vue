<template>
  <div class="modules-management-page">
    <div class="page-header">
      <h1 class="page-title">模块管理</h1>
      <p class="text-gray-400 text-sm">管理功能模块的启用/禁用，实现按需加载</p>
    </div>

    <!-- 模块列表 -->
    <div class="content-section">
      <div class="section-header">
        <h2 class="section-title">功能模块列表</h2>
        <div class="filter-tabs">
          <button
            @click="filterCategory = null"
            :class="['filter-tab', { 'filter-tab-active': filterCategory === null }]"
          >
            全部
          </button>
          <button
            v-for="cat in categories"
            :key="cat"
            @click="filterCategory = cat"
            :class="['filter-tab', { 'filter-tab-active': filterCategory === cat }]"
          >
            {{ getCategoryName(cat) }}
          </button>
        </div>
      </div>

      <div v-if="loading" class="table-loading">加载中...</div>
      <div v-else class="modules-grid">
        <div
          v-for="module in filteredModules"
          :key="module.moduleKey"
          class="module-card"
          :class="{
            'module-card-enabled': module.isEnabled,
            'module-card-disabled': !module.isEnabled,
            'module-card-core': module.isCore
          }"
        >
          <div class="module-card-header">
            <div class="module-card-title">
              <i v-if="module.icon" :class="[module.icon, 'module-icon']"></i>
              <div>
                <h3 class="module-name">{{ module.moduleName }}</h3>
                <span class="module-key">{{ module.moduleKey }}</span>
              </div>
            </div>
            <div class="module-card-badges">
              <span v-if="module.isCore" class="badge badge-warning">核心</span>
              <span v-if="module.isEnabled" class="badge badge-success">已启用</span>
              <span v-else class="badge badge-default">已禁用</span>
            </div>
          </div>

          <div class="module-card-body">
            <p v-if="module.description" class="module-description">
              {{ module.description }}
            </p>

            <div class="module-info">
              <div class="info-item">
                <span class="info-label">版本:</span>
                <span class="info-value">{{ module.moduleVersion }}</span>
              </div>
              <div v-if="module.author" class="info-item">
                <span class="info-label">作者:</span>
                <span class="info-value">{{ module.author }}</span>
              </div>
              <div v-if="module.category" class="info-item">
                <span class="info-label">分类:</span>
                <span class="info-value">{{ getCategoryName(module.category) }}</span>
              </div>
            </div>

            <div v-if="module.dependencies && parseDependencies(module.dependencies).length > 0" class="module-dependencies">
              <span class="dependencies-label">依赖:</span>
              <div class="dependencies-list">
                <span
                  v-for="dep in parseDependencies(module.dependencies)"
                  :key="dep"
                  class="dependency-tag"
                >
                  {{ dep }}
                </span>
              </div>
            </div>
          </div>

          <div class="module-card-footer">
            <div class="module-actions">
              <button
                v-if="!module.isCore"
                @click="toggleModule(module)"
                :class="[
                  'btn-toggle',
                  module.isEnabled ? 'btn-toggle-disable' : 'btn-toggle-enable'
                ]"
              >
                <i :class="module.isEnabled ? 'fas fa-ban' : 'fas fa-check'"></i>
                {{ module.isEnabled ? '禁用' : '启用' }}
              </button>
              <button
                @click="viewModuleDetails(module)"
                class="btn-link btn-link-blue"
              >
                查看详情
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 模块详情模态框 -->
    <div v-if="selectedModule" class="modal-overlay" @click="selectedModule = null">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ selectedModule.moduleName }}</h2>
          <button @click="selectedModule = null" class="modal-close">×</button>
        </div>

        <div class="modal-body">
          <div class="detail-section">
            <h3 class="detail-title">基本信息</h3>
            <div class="detail-grid">
              <div class="detail-item">
                <span class="detail-label">模块标识:</span>
                <span class="detail-value">{{ selectedModule.moduleKey }}</span>
              </div>
              <div class="detail-item">
                <span class="detail-label">版本:</span>
                <span class="detail-value">{{ selectedModule.moduleVersion }}</span>
              </div>
              <div class="detail-item">
                <span class="detail-label">分类:</span>
                <span class="detail-value">{{ getCategoryName(selectedModule.category || '') }}</span>
              </div>
              <div class="detail-item">
                <span class="detail-label">状态:</span>
                <span :class="selectedModule.isEnabled ? 'badge badge-success' : 'badge badge-default'">
                  {{ selectedModule.isEnabled ? '已启用' : '已禁用' }}
                </span>
              </div>
            </div>
          </div>

          <div v-if="selectedModule.description" class="detail-section">
            <h3 class="detail-title">描述</h3>
            <p class="detail-text">{{ selectedModule.description }}</p>
          </div>

          <div v-if="parseRoutes(selectedModule.routes).length > 0" class="detail-section">
            <h3 class="detail-title">路由</h3>
            <div class="routes-list">
              <span
                v-for="route in parseRoutes(selectedModule.routes)"
                :key="route"
                class="route-tag"
              >
                {{ route }}
              </span>
            </div>
          </div>

          <div v-if="parseComponents(selectedModule.components).length > 0" class="detail-section">
            <h3 class="detail-title">组件</h3>
            <div class="components-list">
              <span
                v-for="comp in parseComponents(selectedModule.components)"
                :key="comp"
                class="component-tag"
              >
                {{ comp }}
              </span>
            </div>
          </div>

          <div v-if="parseDependencies(selectedModule.dependencies).length > 0" class="detail-section">
            <h3 class="detail-title">依赖模块</h3>
            <div class="dependencies-list">
              <span
                v-for="dep in parseDependencies(selectedModule.dependencies)"
                :key="dep"
                class="dependency-tag"
              >
                {{ dep }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

interface Module {
  id: number
  moduleKey: string
  moduleName: string
  moduleVersion: string
  description?: string
  author?: string
  icon?: string
  category?: string
  dependencies?: string
  routes?: string
  components?: string
  isEnabled: boolean
  isCore: boolean
  sort: number
}

const api = useApi()
const { handleError } = useErrorHandler()
const message = useSafeMessage()

const modules = ref<Module[]>([])
const loading = ref(false)
const filterCategory = ref<string | null>(null)
const selectedModule = ref<Module | null>(null)

const categories = computed(() => {
  const cats = new Set<string>()
  modules.value.forEach(m => {
    if (m.category) cats.add(m.category)
  })
  return Array.from(cats).sort()
})

const filteredModules = computed(() => {
  if (!filterCategory.value) {
    return modules.value
  }
  return modules.value.filter(m => m.category === filterCategory.value)
})

const fetchModules = async () => {
  loading.value = true
  try {
    const res = await api.get<Module[]>('/Module')
    modules.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    handleError(e, '加载模块列表失败')
  } finally {
    loading.value = false
  }
}

const toggleModule = async (module: Module) => {
  try {
    if (module.isEnabled) {
      await api.post(`/Module/${module.moduleKey}/disable`)
      message.success('模块已禁用')
    } else {
      await api.post(`/Module/${module.moduleKey}/enable`)
      message.success('模块已启用')
    }
    
    // 触发事件通知其他组件
    if (process.client) {
      window.dispatchEvent(new CustomEvent(module.isEnabled ? 'module-disabled' : 'module-enabled', {
        detail: { moduleKey: module.moduleKey }
      }))
    }
    
    await fetchModules()
  } catch (e: unknown) {
    handleError(e, module.isEnabled ? '禁用模块失败' : '启用模块失败')
  }
}

const viewModuleDetails = (module: Module) => {
  selectedModule.value = module
}

const getCategoryName = (category: string): string => {
  const names: Record<string, string> = {
    core: '核心',
    content: '内容',
    tool: '工具',
    experiment: '实验',
    interaction: '交互'
  }
  return names[category] || category
}

const parseDependencies = (deps?: string): string[] => {
  if (!deps) return []
  try {
    return JSON.parse(deps)
  } catch {
    return []
  }
}

const parseRoutes = (routes?: string): string[] => {
  if (!routes) return []
  try {
    const parsed = JSON.parse(routes)
    return Array.isArray(parsed) ? parsed.map((r: any) => typeof r === 'string' ? r : r.path) : []
  } catch {
    return []
  }
}

const parseComponents = (components?: string): string[] => {
  if (!components) return []
  try {
    const parsed = JSON.parse(components)
    return Array.isArray(parsed) ? parsed.map((c: any) => typeof c === 'string' ? c : c.name) : []
  } catch {
    return []
  }
}

onMounted(() => {
  fetchModules()
})
</script>

<style scoped>
.modules-management-page {
  width: 100%;
}

.content-section {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  padding: 1.5rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #ffffff;
}

.filter-tabs {
  display: flex;
  gap: 0.5rem;
}

.filter-tab {
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.25rem;
  color: rgba(255, 255, 255, 0.7);
  cursor: pointer;
  transition: all 0.2s ease;
}

.filter-tab:hover {
  background: rgba(255, 255, 255, 0.15);
}

.filter-tab-active {
  background: rgba(59, 130, 246, 0.3);
  border-color: rgba(59, 130, 246, 0.5);
  color: #ffffff;
}

.modules-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1.5rem;
}

.module-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  padding: 1.25rem;
  transition: all 0.3s ease;
}

.module-card:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.module-card-enabled {
  border-left: 3px solid #22c55e;
}

.module-card-disabled {
  opacity: 0.7;
  border-left: 3px solid rgba(255, 255, 255, 0.2);
}

.module-card-core {
  border-left: 3px solid #f59e0b;
}

.module-card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.module-card-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex: 1;
}

.module-icon {
  font-size: 1.5rem;
  color: rgba(255, 255, 255, 0.7);
}

.module-name {
  font-size: 1rem;
  font-weight: 600;
  color: #ffffff;
  margin-bottom: 0.25rem;
}

.module-key {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
  font-family: monospace;
}

.module-card-badges {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.module-card-body {
  margin-bottom: 1rem;
}

.module-description {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 1rem;
  line-height: 1.5;
}

.module-info {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.info-item {
  display: flex;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.info-label {
  color: rgba(255, 255, 255, 0.5);
}

.info-value {
  color: rgba(255, 255, 255, 0.9);
}

.module-dependencies {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.dependencies-label {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
  margin-bottom: 0.5rem;
  display: block;
}

.dependencies-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.dependency-tag {
  padding: 0.25rem 0.5rem;
  background: rgba(59, 130, 246, 0.2);
  border: 1px solid rgba(59, 130, 246, 0.4);
  border-radius: 0.25rem;
  font-size: 0.75rem;
  color: #93c5fd;
}

.module-card-footer {
  padding-top: 1rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.module-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-toggle {
  padding: 0.5rem 1rem;
  border-radius: 0.25rem;
  border: none;
  cursor: pointer;
  font-size: 0.875rem;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-toggle-enable {
  background: rgba(34, 197, 94, 0.3);
  border: 1px solid rgba(34, 197, 94, 0.5);
  color: #a7f3d0;
}

.btn-toggle-enable:hover {
  background: rgba(34, 197, 94, 0.4);
}

.btn-toggle-disable {
  background: rgba(239, 68, 68, 0.3);
  border: 1px solid rgba(239, 68, 68, 0.5);
  color: #fecaca;
}

.btn-toggle-disable:hover {
  background: rgba(239, 68, 68, 0.4);
}

.badge {
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.badge-success {
  background: rgba(34, 197, 94, 0.3);
  border: 1px solid rgba(34, 197, 94, 0.6);
  color: #a7f3d0;
}

.badge-default {
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: rgba(255, 255, 255, 0.9);
}

.badge-warning {
  background: rgba(251, 191, 36, 0.3);
  border: 1px solid rgba(251, 191, 36, 0.6);
  color: #fde68a;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: rgba(30, 41, 59, 0.95);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.75rem;
  width: 90%;
  max-width: 700px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.modal-header h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #ffffff;
}

.modal-close {
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.7);
  font-size: 1.5rem;
  cursor: pointer;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.25rem;
  transition: all 0.2s ease;
}

.modal-close:hover {
  background: rgba(255, 255, 255, 0.1);
  color: #ffffff;
}

.modal-body {
  padding: 1.5rem;
}

.detail-section {
  margin-bottom: 1.5rem;
}

.detail-title {
  font-size: 1rem;
  font-weight: 600;
  color: #ffffff;
  margin-bottom: 1rem;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.detail-label {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
}

.detail-value {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.9);
}

.detail-text {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.7);
  line-height: 1.6;
}

.routes-list,
.components-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.route-tag,
.component-tag {
  padding: 0.25rem 0.5rem;
  background: rgba(139, 92, 246, 0.2);
  border: 1px solid rgba(139, 92, 246, 0.4);
  border-radius: 0.25rem;
  font-size: 0.75rem;
  color: #c7d2fe;
  font-family: monospace;
}
</style>

