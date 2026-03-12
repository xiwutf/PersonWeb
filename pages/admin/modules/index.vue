<template>
  <div class="module-management-container">
    <!-- 页面标题 -->
    <div class="page-header">
      <h1>模块管理</h1>
      <p>管理系统中的所有功能模块</p>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon">📦</div>
        <div class="stat-content">
          <h3>{{ stats.totalModules }}</h3>
          <p>总模块数</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon enabled">✅</div>
        <div class="stat-content">
          <h3>{{ stats.enabledModules }}</h3>
          <p>已启用</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon disabled">❌</div>
        <div class="stat-content">
          <h3>{{ stats.disabledModules }}</h3>
          <p>已禁用</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon">⭐</div>
        <div class="stat-content">
          <h3>{{ stats.avgRating.toFixed(1) }}</h3>
          <p>平均评分</p>
        </div>
      </div>
    </div>

    <!-- 筛选和搜索 -->
    <div class="filter-section">
      <div class="search-box">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="搜索模块..."
          @input="handleSearch"
        />
      </div>
      <div class="filter-group">
        <select v-model="selectedCategory" @change="handleFilterChange">
          <option value="">所有分类</option>
          <option value="content">内容</option>
          <option value="tool">工具</option>
          <option value="ai">AI</option>
          <option value="experiment">实验</option>
          <option value="interaction">交互</option>
        </select>
        <select v-model="enabledFilter" @change="handleFilterChange">
          <option value="">全部状态</option>
          <option :value="true">已启用</option>
          <option :value="false">已禁用</option>
        </select>
      </div>
    </div>

    <!-- 模块列表 -->
    <div class="modules-list">
      <div
        v-for="module in filteredModules"
        :key="module.moduleKey"
        class="module-card"
        :class="{ 'disabled': !module.isEnabled }"
      >
        <div class="module-header">
          <div class="module-icon">{{ module.icon }}</div>
          <div class="module-info">
            <h3>{{ module.moduleName }}</h3>
            <p class="version">v{{ module.moduleVersion }}</p>
          </div>
          <div class="module-status">
            <span
              class="status-badge"
              :class="{ 'enabled': module.isEnabled, 'disabled': !module.isEnabled }"
            >
              {{ module.isEnabled ? '启用' : '禁用' }}
            </span>
            <span v-if="module.isCore" class="core-badge">核心</span>
          </div>
        </div>

        <div class="module-content">
          <p class="description">{{ module.description }}</p>
          <div class="meta-info">
            <span class="author">作者: {{ module.author }}</span>
            <span class="category">{{ module.category }}</span>
          </div>
        </div>

        <div class="module-actions">
          <button
            v-if="!module.isCore"
            @click="toggleModule(module)"
            :class="['toggle-btn', module.isEnabled ? 'disable' : 'enable']"
          >
            {{ module.isEnabled ? '禁用' : '启用' }}
          </button>
          <button class="config-btn" @click="openConfig(module)">
            配置
          </button>
          <button class="details-btn" @click="showDetails(module)">
            详情
          </button>
        </div>

        <!-- 依赖信息 -->
        <div v-if="module.dependencies && module.dependencies.length > 0" class="dependencies">
          <h4>依赖:</h4>
          <div class="dependency-list">
            <span
              v-for="dep in module.dependencies"
              :key="dep"
              class="dependency-tag"
            >
              {{ dep }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- 加载状态 -->
    <div v-if="loading" class="loading">
      加载中...
    </div>

    <!-- 空状态 -->
    <div v-if="!loading && filteredModules.length === 0" class="empty-state">
      没有找到匹配的模块
    </div>

    <!-- 配置模态框 -->
    <div v-if="showConfigModal" class="modal" @click="closeConfig">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ selectedModule?.moduleName }} 配置</h2>
          <button class="close-btn" @click="closeConfig">✕</button>
        </div>
        <div class="modal-body">
          <div
            v-for="config in moduleConfigs"
            :key="config.configKey"
            class="config-item"
          >
            <label>{{ config.configKey }}</label>
            <input
              v-if="config.configKey !== 'apiKey'"
              v-model="config.configValue"
              type="text"
              @input="updateConfig(config)"
            />
            <input
              v-else
              v-model="config.configValue"
              type="password"
              @input="updateConfig(config)"
            />
            <span class="config-description">{{ config.description }}</span>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="saveConfig" class="save-btn">保存配置</button>
          <button @click="closeConfig" class="cancel-btn">取消</button>
        </div>
      </div>
    </div>

    <!-- 详情模态框 -->
    <div v-if="showDetailsModal" class="modal" @click="closeDetails">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ selectedModule?.moduleName }} 详情</h2>
          <button class="close-btn" @click="closeDetails">✕</button>
        </div>
        <div class="modal-body">
          <div class="detail-section">
            <h3>基本信息</h3>
            <p><strong>标识:</strong> {{ selectedModule?.moduleKey }}</p>
            <p><strong>版本:</strong> {{ selectedModule?.moduleVersion }}</p>
            <p><strong>分类:</strong> {{ selectedModule?.category }}</p>
            <p><strong>作者:</strong> {{ selectedModule?.author }}</p>
          </div>

          <div v-if="selectedModule?.description" class="detail-section">
            <h3>描述</h3>
            <p>{{ selectedModule.description }}</p>
          </div>

          <div v-if="selectedModule?.routes && selectedModule.routes.length > 0" class="detail-section">
            <h3>路由</h3>
            <ul>
              <li v-for="route in selectedModule.routes" :key="route.name">
                {{ route.path }} - {{ route.meta?.title || route.name }}
              </li>
            </ul>
          </div>

          <div v-if="selectedModule?.components && selectedModule.components.length > 0" class="detail-section">
            <h3>组件</h3>
            <ul>
              <li v-for="component in selectedModule.components" :key="component.name">
                {{ component.name }} ({{ component.global ? '全局' : '局部' }})
              </li>
            </ul>
          </div>

          <div v-if="selectedModule?.permissions && selectedModule.permissions.length > 0" class="detail-section">
            <h3>权限</h3>
            <ul>
              <li v-for="permission in selectedModule.permissions" :key="permission.key">
                {{ permission.key }} - {{ permission.name }} ({{ permission.level }})
              </li>
            </ul>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="closeDetails" class="close-btn">关闭</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'

// 状态管理
const modules = ref([])
const stats = ref({})
const loading = ref(true)
const searchQuery = ref('')
const selectedCategory = ref('')
const enabledFilter = ref('')
const showConfigModal = ref(false)
const showDetailsModal = ref(false)
const selectedModule = ref(null)
const moduleConfigs = ref([])

// 模拟数据 - 实际项目中从API获取
onMounted(async () => {
  try {
    // 加载模块列表
    const modulesResponse = await fetch('/api/modules')
    const modulesData = await modulesResponse.json()
    modules.value = modulesData.data

    // 加载统计信息
    const statsResponse = await fetch('/api/modules/stats')
    const statsData = await statsResponse.json()
    stats.value = statsData.data
  } catch (error) {
    console.error('加载模块数据失败:', error)
  } finally {
    loading.value = false
  }
})

// 计算属性
const filteredModules = computed(() => {
  let result = modules.value

  // 搜索过滤
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter((m: any) =>
      m.moduleName.toLowerCase().includes(query) ||
      m.description?.toLowerCase().includes(query) ||
      m.category.toLowerCase().includes(query)
    )
  }

  // 分类过滤
  if (selectedCategory.value) {
    result = result.filter((m: any) => m.category === selectedCategory.value)
  }

  // 状态过滤
  if (enabledFilter.value !== '') {
    result = result.filter((m: any) => m.isEnabled === enabledFilter.value)
  }

  // 排序
  result.sort((a: any, b: any) => {
    if (a.isCore && !b.isCore) return -1
    if (!a.isCore && b.isCore) return 1
    return a.sort - b.sort
  })

  return result
})

// 方法
const handleSearch = () => {
  // 搜索逻辑已在计算属性中处理
}

const handleFilterChange = () => {
  // 筛选逻辑已在计算属性中处理
}

const toggleModule = async (module: any) => {
  try {
    const response = await fetch(`/api/modules/${module.moduleKey}/enable`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        enabled: !module.isEnabled
      })
    })

    if (response.ok) {
      module.isEnabled = !module.isEnabled
      // 更新统计信息
      stats.value.enabledModules = module.isEnabled ?
        stats.value.enabledModules + 1 :
        stats.value.enabledModules - 1
      stats.value.disabledModules = module.isEnabled ?
        stats.value.disabledModules - 1 :
        stats.value.disabledModules + 1
    }
  } catch (error) {
    console.error('切换模块状态失败:', error)
  }
}

const openConfig = async (module: any) => {
  selectedModule.value = module
  try {
    const response = await fetch(`/api/modules/${module.moduleKey}/config`)
    const data = await response.json()
    moduleConfigs.value = data.data.map((config: any) => ({
      ...config,
      configValue: String(config.configValue)
    }))
    showConfigModal.value = true
  } catch (error) {
    console.error('加载模块配置失败:', error)
  }
}

const closeConfig = () => {
  showConfigModal.value = false
  selectedModule.value = null
  moduleConfigs.value = []
}

const updateConfig = (config: any) => {
  // 实时更新配置值
}

const saveConfig = async () => {
  try {
    // 保存配置到后端
    const response = await fetch(`/api/modules/${selectedModule.value.moduleKey}/config`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(moduleConfigs.value)
    })

    if (response.ok) {
      closeConfig()
    }
  } catch (error) {
    console.error('保存配置失败:', error)
  }
}

const showDetails = (module: any) => {
  selectedModule.value = module
  showDetailsModal.value = true
}

const closeDetails = () => {
  showDetailsModal.value = false
  selectedModule.value = null
}
</script>

<style scoped>
.module-management-container {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2rem;
  margin-bottom: 0.5rem;
}

.page-header p {
  color: #666;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  display: flex;
  align-items: center;
  gap: 1rem;
}

.stat-icon {
  font-size: 2rem;
}

.stat-icon.enabled {
  color: #10b981;
}

.stat-icon.disabled {
  color: #ef4444;
}

.stat-content h3 {
  font-size: 1.5rem;
  margin: 0;
}

.stat-content p {
  margin: 0;
  color: #666;
}

.filter-section {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.search-box {
  flex: 1;
  min-width: 250px;
}

.search-box input {
  width: 100%;
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.filter-group {
  display: flex;
  gap: 0.5rem;
}

.filter-group select {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  background: white;
}

.modules-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}

.module-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  overflow: hidden;
  transition: transform 0.2s;
}

.module-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}

.module-card.disabled {
  opacity: 0.7;
}

.module-header {
  padding: 1rem;
  border-bottom: 1px solid #eee;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.module-icon {
  font-size: 1.5rem;
}

.module-info h3 {
  margin: 0;
  font-size: 1.1rem;
}

.version {
  font-size: 0.9rem;
  color: #666;
}

.module-status {
  margin-left: auto;
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.8rem;
}

.status-badge.enabled {
  background: #d1fae5;
  color: #065f46;
}

.status-badge.disabled {
  background: #fee2e2;
  color: #991b1b;
}

.core-badge {
  background: #fbbf24;
  color: #78350f;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.8rem;
}

.module-content {
  padding: 1rem;
}

.description {
  margin: 0 0 1rem 0;
  color: #666;
}

.meta-info {
  display: flex;
  gap: 1rem;
  font-size: 0.9rem;
  color: #666;
}

.module-actions {
  padding: 1rem;
  border-top: 1px solid #eee;
  display: flex;
  gap: 0.5rem;
}

.toggle-btn {
  flex: 1;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background-color 0.2s;
}

.toggle-btn.enable {
  background: #3b82f6;
  color: white;
}

.toggle-btn.enable:hover {
  background: #2563eb;
}

.toggle-btn.disable {
  background: #ef4444;
  color: white;
}

.toggle-btn.disable:hover {
  background: #dc2626;
}

.config-btn,
.details-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background-color 0.2s;
}

.config-btn:hover,
.details-btn:hover {
  background: #f3f4f6;
}

.dependencies {
  padding: 1rem;
  border-top: 1px solid #eee;
  background: #f9fafb;
}

.dependencies h4 {
  margin: 0 0 0.5rem 0;
  font-size: 0.9rem;
}

.dependency-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.dependency-tag {
  background: #e5e7eb;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.8rem;
}

.loading {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  max-width: 600px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
}

.modal-header {
  padding: 1rem;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h2 {
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
}

.modal-body {
  padding: 1rem;
}

.modal-footer {
  padding: 1rem;
  border-top: 1px solid #eee;
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}

.save-btn,
.cancel-btn,
.close-btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background-color 0.2s;
}

.save-btn {
  background: #3b82f6;
  color: white;
}

.save-btn:hover {
  background: #2563eb;
}

.cancel-btn,
.close-btn {
  background: #e5e7eb;
  color: #666;
}

.cancel-btn:hover,
.close-btn:hover {
  background: #d1d5db;
}

.config-item {
  margin-bottom: 1rem;
}

.config-item label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 500;
}

.config-item input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.config-description {
  display: block;
  margin-top: 0.25rem;
  font-size: 0.8rem;
  color: #666;
}

.detail-section {
  margin-bottom: 1.5rem;
}

.detail-section h3 {
  margin: 0 0 0.5rem 0;
  font-size: 1.1rem;
}

.detail-section ul {
  margin: 0;
  padding-left: 1.5rem;
}
</style>