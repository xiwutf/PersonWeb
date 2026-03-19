<template>
  <div class="modules-management-page">
    <PageHeader
      title="模块管理"
      description="管理功能模块的启用状态、分类分布和依赖关系，按需控制后台与前台能力范围。"
      class="modules-header"
    >
      <template #actions>
        <div class="modules-header-badge">
          <span class="modules-header-badge__label">当前概览</span>
          <strong>{{ enabledModulesCount }} / {{ modules.length }} 已启用</strong>
        </div>
      </template>
    </PageHeader>

    <section class="modules-overview">
      <article class="overview-card">
        <span class="overview-card__label">模块总数</span>
        <strong class="overview-card__value">{{ modules.length }}</strong>
        <p class="overview-card__hint">覆盖内容、工具、实验和互动等功能域</p>
      </article>
      <article class="overview-card">
        <span class="overview-card__label">已启用模块</span>
        <strong class="overview-card__value overview-card__value--success">{{ enabledModulesCount }}</strong>
        <p class="overview-card__hint">当前正在参与站点渲染与交互</p>
      </article>
      <article class="overview-card">
        <span class="overview-card__label">核心模块</span>
        <strong class="overview-card__value overview-card__value--warning">{{ coreModulesCount }}</strong>
        <p class="overview-card__hint">核心模块不可直接停用</p>
      </article>
      <article class="overview-card">
        <span class="overview-card__label">分类数量</span>
        <strong class="overview-card__value">{{ categories.length }}</strong>
        <p class="overview-card__hint">支持快速按分类筛选与排查</p>
      </article>
    </section>

    <section class="content-section">
      <div class="section-header">
        <div>
          <h2 class="section-title">功能模块列表</h2>
          <p class="section-description">点击分类标签快速收窄范围，卡片里可直接启用、禁用或查看详情。</p>
        </div>
        <div class="filter-tabs">
          <button
            @click="filterCategory = null"
            :class="['filter-tab', { 'filter-tab-active': filterCategory === null }]"
          >
            全部模块
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

      <div v-if="loading" class="table-loading">加载模块中...</div>
      <div v-else-if="filteredModules.length === 0" class="table-empty">当前筛选条件下没有模块</div>
      <div v-else class="modules-grid">
        <article
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
              <span class="module-icon-wrap">
                <i v-if="module.icon" :class="[module.icon, 'module-icon']"></i>
                <i v-else class="fas fa-cube module-icon"></i>
              </span>
              <div>
                <h3 class="module-name">{{ module.moduleName }}</h3>
                <span class="module-key">{{ module.moduleKey }}</span>
              </div>
            </div>
            <div class="module-card-badges">
              <span v-if="module.isCore" class="badge badge-warning">核心模块</span>
              <span v-if="module.isEnabled" class="badge badge-success">已启用</span>
              <span v-else class="badge badge-default">已禁用</span>
            </div>
          </div>

          <div class="module-card-body">
            <p v-if="module.description" class="module-description">
              {{ module.description }}
            </p>

            <dl class="module-info">
              <div class="info-item">
                <dt class="info-label">版本</dt>
                <dd class="info-value">{{ module.moduleVersion || '-' }}</dd>
              </div>
              <div v-if="module.author" class="info-item">
                <dt class="info-label">作者</dt>
                <dd class="info-value">{{ module.author }}</dd>
              </div>
              <div v-if="module.category" class="info-item">
                <dt class="info-label">分类</dt>
                <dd class="info-value">{{ getCategoryName(module.category) }}</dd>
              </div>
            </dl>

            <div
              v-if="module.dependencies && parseDependencies(module.dependencies).length > 0"
              class="module-dependencies"
            >
              <span class="dependencies-label">依赖模块</span>
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
                {{ module.isEnabled ? '禁用模块' : '启用模块' }}
              </button>
              <button
                @click="viewModuleDetails(module)"
                class="btn-link btn-link-blue"
              >
                查看详情
              </button>
            </div>
          </div>
        </article>
      </div>
    </section>

    <div v-if="selectedModule" class="modal-overlay" @click="selectedModule = null">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <div>
            <h2>{{ selectedModule.moduleName }}</h2>
            <p class="modal-subtitle">模块标识：{{ selectedModule.moduleKey }}</p>
          </div>
          <button @click="selectedModule = null" class="modal-close">×</button>
        </div>

        <div class="modal-body">
          <div class="detail-section">
            <h3 class="detail-title">基本信息</h3>
            <div class="detail-grid">
              <div class="detail-item">
                <span class="detail-label">模块标识</span>
                <span class="detail-value">{{ selectedModule.moduleKey }}</span>
              </div>
              <div class="detail-item">
                <span class="detail-label">版本</span>
                <span class="detail-value">{{ selectedModule.moduleVersion || '-' }}</span>
              </div>
              <div class="detail-item">
                <span class="detail-label">分类</span>
                <span class="detail-value">{{ getCategoryName(selectedModule.category || '') }}</span>
              </div>
              <div class="detail-item">
                <span class="detail-label">状态</span>
                <span :class="selectedModule.isEnabled ? 'badge badge-success' : 'badge badge-default'">
                  {{ selectedModule.isEnabled ? '已启用' : '已禁用' }}
                </span>
              </div>
            </div>
          </div>

          <div v-if="selectedModule.description" class="detail-section">
            <h3 class="detail-title">模块说明</h3>
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
import { computed, onMounted, ref } from 'vue'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'
import PageHeader from '~/components/admin/patterns/PageHeader.vue'

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
  modules.value.forEach(module => {
    if (module.category) cats.add(module.category)
  })
  return Array.from(cats).sort()
})

const enabledModulesCount = computed(() => modules.value.filter(module => module.isEnabled).length)
const coreModulesCount = computed(() => modules.value.filter(module => module.isCore).length)

const filteredModules = computed(() => {
  if (!filterCategory.value) {
    return modules.value
  }
  return modules.value.filter(module => module.category === filterCategory.value)
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
    interaction: '互动'
  }
  return names[category] || category || '未分类'
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
    return Array.isArray(parsed) ? parsed.map((route: any) => typeof route === 'string' ? route : route.path) : []
  } catch {
    return []
  }
}

const parseComponents = (components?: string): string[] => {
  if (!components) return []
  try {
    const parsed = JSON.parse(components)
    return Array.isArray(parsed) ? parsed.map((component: any) => typeof component === 'string' ? component : component.name) : []
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

.modules-header-badge {
  display: inline-flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.25rem;
  padding: 0.65rem 0.9rem;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  background: var(--admin-surface-2, rgba(255, 255, 255, 0.04));
}

.modules-header-badge__label {
  color: var(--color-text-muted);
  font-size: 0.75rem;
}

.modules-header-badge strong {
  color: var(--color-text-main);
  font-size: 0.95rem;
}

.modules-overview {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.overview-card {
  padding: 1.25rem;
  background: var(--admin-surface-1, rgba(255, 255, 255, 0.05));
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
}

.overview-card__label {
  display: block;
  margin-bottom: 0.6rem;
  color: var(--color-text-muted);
  font-size: 0.8rem;
}

.overview-card__value {
  display: block;
  color: var(--color-text-main);
  font-size: 2rem;
  font-weight: 700;
  line-height: 1;
}

.overview-card__value--success {
  color: var(--color-success);
}

.overview-card__value--warning {
  color: var(--color-warning);
}

.overview-card__hint {
  margin-top: 0.75rem;
  color: var(--color-text-muted);
  font-size: 0.85rem;
  line-height: 1.5;
}

.content-section {
  background: var(--admin-surface-1, rgba(255, 255, 255, 0.05));
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: 1.5rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--color-text-main);
}

.section-description {
  margin-top: 0.4rem;
  color: var(--color-text-muted);
  font-size: 0.9rem;
}

.filter-tabs {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.filter-tab {
  padding: 0.6rem 0.95rem;
  background: var(--admin-surface-2, rgba(255, 255, 255, 0.08));
  border: 1px solid var(--color-border);
  border-radius: 999px;
  color: var(--color-text-muted);
  cursor: pointer;
  transition: all 0.2s ease;
}

.filter-tab:hover {
  color: var(--color-text-main);
  border-color: var(--color-primary-soft);
}

.filter-tab-active {
  background: color-mix(in srgb, var(--color-primary) 16%, transparent);
  border-color: color-mix(in srgb, var(--color-primary) 42%, transparent);
  color: var(--color-primary-hover);
}

.modules-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1rem;
}

.module-card {
  position: relative;
  background: var(--admin-surface-2, rgba(255, 255, 255, 0.05));
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: 1.35rem;
  transition: transform 0.2s ease, border-color 0.2s ease, background 0.2s ease;
}

.module-card::before {
  content: '';
  position: absolute;
  inset: 0 auto 0 0;
  width: 3px;
  border-radius: var(--radius-lg) 0 0 var(--radius-lg);
  background: var(--color-border);
}

.module-card:hover {
  transform: translateY(-2px);
  background: var(--admin-surface-3, rgba(255, 255, 255, 0.08));
  border-color: var(--color-primary-soft);
}

.module-card-enabled::before {
  background: var(--color-success);
}

.module-card-disabled::before {
  background: var(--color-border);
  opacity: 0.75;
}

.module-card-core::before {
  background: var(--color-warning);
}

.module-card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1rem;
}

.module-card-title {
  display: flex;
  align-items: center;
  gap: 0.9rem;
  min-width: 0;
}

.module-icon-wrap {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 3rem;
  height: 3rem;
  flex-shrink: 0;
  border-radius: 0.9rem;
  background: var(--admin-surface-3, rgba(255, 255, 255, 0.08));
  border: 1px solid var(--color-border);
}

.module-icon {
  font-size: 1.45rem;
  color: var(--color-primary-hover);
}

.module-name {
  color: var(--color-text-main);
  font-size: 1.15rem;
  font-weight: 700;
  line-height: 1.2;
}

.module-key {
  display: inline-block;
  margin-top: 0.25rem;
  color: var(--color-text-muted);
  font-size: 0.75rem;
  font-family: monospace;
}

.module-card-badges {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  justify-content: flex-end;
}

.module-card-body {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.module-description {
  min-height: 3rem;
  color: var(--color-text-main);
  font-size: 0.92rem;
  line-height: 1.6;
}

.module-info {
  display: grid;
  gap: 0.7rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  gap: 1rem;
  padding-bottom: 0.55rem;
  border-bottom: 1px solid color-mix(in srgb, var(--color-border) 70%, transparent);
}

.info-label {
  color: var(--color-text-muted);
  font-size: 0.82rem;
}

.info-value {
  color: var(--color-text-main);
  font-size: 0.88rem;
  text-align: right;
}

.module-dependencies {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.dependencies-label {
  color: var(--color-text-muted);
  font-size: 0.8rem;
}

.dependencies-list,
.routes-list,
.components-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.dependency-tag,
.route-tag,
.component-tag {
  display: inline-flex;
  align-items: center;
  padding: 0.35rem 0.65rem;
  border-radius: 999px;
  border: 1px solid color-mix(in srgb, var(--color-primary) 42%, transparent);
  background: color-mix(in srgb, var(--color-primary) 12%, transparent);
  color: var(--color-primary-hover);
  font-size: 0.78rem;
}

.module-card-footer {
  margin-top: 1.1rem;
  padding-top: 1rem;
  border-top: 1px solid var(--color-border);
}

.module-actions {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.btn-toggle {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  padding: 0.6rem 0.9rem;
  border-radius: var(--radius-md);
  border: 1px solid transparent;
  cursor: pointer;
  font-size: 0.85rem;
  font-weight: 600;
  transition: all 0.2s ease;
}

.btn-toggle-enable {
  background: color-mix(in srgb, var(--color-success) 15%, transparent);
  border-color: color-mix(in srgb, var(--color-success) 45%, transparent);
  color: var(--color-success);
}

.btn-toggle-enable:hover {
  background: color-mix(in srgb, var(--color-success) 22%, transparent);
}

.btn-toggle-disable {
  background: color-mix(in srgb, var(--color-error) 15%, transparent);
  border-color: color-mix(in srgb, var(--color-error) 45%, transparent);
  color: var(--color-error);
}

.btn-toggle-disable:hover {
  background: color-mix(in srgb, var(--color-error) 22%, transparent);
}

.btn-link {
  display: inline-flex;
  align-items: center;
  padding: 0.6rem 0.2rem;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 0.85rem;
  font-weight: 600;
}

.btn-link-blue {
  color: var(--color-primary-hover);
}

.badge {
  display: inline-flex;
  align-items: center;
  padding: 0.28rem 0.55rem;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 600;
}

.badge-success {
  background: color-mix(in srgb, var(--color-success) 15%, transparent);
  border: 1px solid color-mix(in srgb, var(--color-success) 45%, transparent);
  color: var(--color-success);
}

.badge-default {
  background: color-mix(in srgb, var(--color-border) 65%, transparent);
  border: 1px solid var(--color-border);
  color: var(--color-text-main);
}

.badge-warning {
  background: color-mix(in srgb, var(--color-warning) 16%, transparent);
  border: 1px solid color-mix(in srgb, var(--color-warning) 46%, transparent);
  color: var(--color-warning);
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: var(--overlay-color, rgba(0, 0, 0, 0.72));
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1.5rem;
  z-index: 1000;
}

.modal-content {
  width: min(760px, 100%);
  max-height: 90vh;
  overflow-y: auto;
  background: var(--admin-surface-1, rgba(15, 23, 42, 0.96));
  border: 1px solid var(--color-border);
  border-radius: var(--radius-xl);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  padding: 1.5rem;
  border-bottom: 1px solid var(--color-border);
}

.modal-header h2 {
  color: var(--color-text-main);
  font-size: 1.25rem;
  font-weight: 700;
}

.modal-subtitle {
  margin-top: 0.35rem;
  color: var(--color-text-muted);
  font-size: 0.85rem;
}

.modal-close {
  width: 2.25rem;
  height: 2.25rem;
  border: none;
  border-radius: 999px;
  background: var(--admin-surface-2, rgba(255, 255, 255, 0.08));
  color: var(--color-text-main);
  font-size: 1.2rem;
  cursor: pointer;
}

.modal-body {
  padding: 1.5rem;
}

.detail-section + .detail-section {
  margin-top: 1.5rem;
}

.detail-title {
  margin-bottom: 0.85rem;
  color: var(--color-text-main);
  font-size: 1rem;
  font-weight: 700;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 0.9rem;
}

.detail-item {
  padding: 1rem;
  background: var(--admin-surface-2, rgba(255, 255, 255, 0.04));
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
}

.detail-label {
  display: block;
  margin-bottom: 0.4rem;
  color: var(--color-text-muted);
  font-size: 0.78rem;
}

.detail-value {
  color: var(--color-text-main);
  font-size: 0.92rem;
}

.detail-text {
  color: var(--color-text-main);
  line-height: 1.7;
}

@media (max-width: 1200px) {
  .modules-overview {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 768px) {
  .modules-overview,
  .detail-grid {
    grid-template-columns: 1fr;
  }

  .content-section,
  .modal-body,
  .modal-header {
    padding: 1rem;
  }

  .module-card-header {
    flex-direction: column;
  }

  .module-card-badges,
  .module-actions {
    justify-content: flex-start;
  }
}
</style>
