<template>
  <div class="module-store">
    <!-- 页面头部 -->
    <div class="store-header">
      <h1>模块商店</h1>
      <p>发现和安装丰富的功能模块，让您的网站更加强大</p>
    </div>

    <!-- 搜索和筛选 -->
    <div class="store-controls">
      <div class="search-box">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="搜索模块..."
          class="search-input"
        />
        <button class="search-btn">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z" clip-rule="evenodd" />
          </svg>
        </button>
      </div>

      <div class="filter-controls">
        <select v-model="selectedCategory" class="category-select">
          <option value="">所有分类</option>
          <option value="ai">AI</option>
          <option value="visitor">访客互动</option>
          <option value="3d">3D展示</option>
          <option value="admin">后台管理</option>
          <option value="performance">性能监控</option>
          <option value="i18n">多语言</option>
          <option value="tools">工具集</option>
          <option value="ui">UI组件</option>
        </select>

        <select v-model="sortBy" class="sort-select">
          <option value="popular">热门</option>
          <option value="newest">最新</option>
          <option value="price-low">价格从低到高</option>
          <option value="price-high">价格从高到低</option>
        </select>
      </div>
    </div>

    <!-- 模块网格 -->
    <div class="modules-grid">
      <!-- 加载状态 -->
      <div v-if="isLoading" class="loading-state">
        <div class="loading-spinner"></div>
        <p>正在加载模块...</p>
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="error-state">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 text-red-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <p>{{ error }}</p>
        <button @click="loadModules" class="retry-btn">重试</button>
      </div>

      <!-- 模块列表 -->
      <div v-else class="modules-list">
        <ModuleCard
          v-for="module in filteredModules"
          :key="module.key"
          :module="module"
          @install="handleInstall"
          @preview="handlePreview"
        />
      </div>
    </div>

    <!-- 分页 -->
    <div v-if="!isLoading && !error && filteredModules.length > 0" class="pagination">
      <button
        @click="currentPage = Math.max(1, currentPage - 1)"
        :disabled="currentPage === 1"
        class="page-btn"
      >
      </button>

      <div class="page-numbers">
        <button
          v-for="page in totalPages"
          :key="page"
          @click="currentPage = page"
          :class="['page-number', { active: currentPage === page }]"
        >
          {{ page }}
        </button>
      </div>

      <button
        @click="currentPage = Math.min(totalPages, currentPage + 1)"
        :disabled="currentPage === totalPages"
        class="page-btn"
      >
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import ModuleCard from '~/components/ModuleCard.vue'
import { useModuleStore } from '~/composables/useModuleStore'
import { useModuleManager } from '~/composables/useModuleManager'

const searchQuery = ref('')
const selectedCategory = ref('')
const sortBy = ref('popular')
const currentPage = ref(1)
const pageSize = ref(12)

const {
  isLoading,
  error,
  getModules,
} = useModuleStore()

const { installModule, installMultipleModules } = useModuleManager()

// 状态
const modules = ref([])

// 数据
const filteredModules = computed(() => {
  let result = [...modules.value]
  const q = searchQuery.value.trim().toLowerCase()

  if (q) {
    result = result.filter(
      (m) =>
        m.name.toLowerCase().includes(q) ||
        m.description.toLowerCase().includes(q) ||
        m.key.toLowerCase().includes(q) ||
        (m.tags && m.tags.some((t) => String(t).toLowerCase().includes(q))),
    )
  }

  if (selectedCategory.value) {
    result = result.filter((m) => m.category === selectedCategory.value)
  }

  result.sort((a, b) => {
    switch (sortBy.value) {
      case 'popular':
        return b.downloads - a.downloads
      case 'newest':
        return String(b.version).localeCompare(String(a.version))
      case 'price-low':
        return (a.price || 0) - (b.price || 0)
      case 'price-high':
        return (b.price || 0) - (a.price || 0)
      default:
        return 0
    }
  })

  return result
})

const totalPages = computed(() => {
  return Math.ceil(filteredModules.value.length / pageSize.value)
})

    // 排序
async function loadModules() {
  try {
    modules.value = await getModules(true)
  } catch (e) {
    console.error('Failed to load modules:', e)
  }
}

async function handleInstall(moduleKey: string) {
  try {
    const success = await installModule(moduleKey, {
      activate: true
    })

    if (success) {
      alert('模块安装成功！')
    } else {
      alert('模块安装成功！')
    }
  } catch (e: unknown) {
    console.error('Failed to install module:', e)
    alert('安装失败：' + (e instanceof Error ? e.message : String(e)))
  }
}

function handlePreview(moduleKey: string) {
// 方法
  console.log('Preview module:', moduleKey)
}

  // 这里可以打开预览弹窗
onMounted(() => {
  loadModules()
})
</script>

<style scoped>
.module-store {
  max-width: var(--space-container);
  margin: 0 auto;
  padding: 2rem;
}

.store-header {
  text-align: center;
  margin-bottom: 3rem;
}

.store-header h1 {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--color-text-dark);
  margin-bottom: 1rem;
}

.store-header p {
  font-size: 1.1rem;
  color: var(--color-text-muted);
  max-width: 600px;
  margin: 0 auto;
}

.store-controls {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.search-box {
  flex: 1;
  min-width: 300px;
  display: flex;
  gap: 0.5rem;
}

.search-input {
  flex: 1;
  padding: 0.75rem 1rem;
  border: 2px solid var(--color-text-sub);
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.search-input:focus {
  outline: none;
  border-color: var(--color-primary);
}

.search-btn {
  padding: 0.75rem;
  background: var(--color-primary);
  color: var(--color-bg-light, white);
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: background 0.2s;
}

.search-btn:hover {
  background: var(--color-primary-hover);
}

.filter-controls {
  display: flex;
  gap: 1rem;
}

.category-select,
.sort-select {
  padding: 0.75rem;
  border: 2px solid var(--color-text-sub);
  border-radius: 8px;
  font-size: 1rem;
  background: var(--color-bg-light, white);
  cursor: pointer;
}

.modules-grid {
  margin-bottom: 3rem;
}

.loading-state {
  text-align: center;
  padding: 4rem;
}

.loading-spinner {
  width: 48px;
  height: 48px;
  border: 4px solid var(--color-bg-light);
  border-top: 4px solid var(--color-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-state {
  text-align: center;
  padding: 4rem;
  color: var(--color-danger);
}

.retry-btn {
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background: var(--color-primary);
  color: var(--color-bg-light, white);
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.modules-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
}

.page-btn {
  padding: 0.5rem 1rem;
  border: 1px solid var(--color-text-sub);
  background: var(--color-bg-light, white);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-btn:hover:not(:disabled) {
  background: var(--color-gray-100);
}

.page-numbers {
  display: flex;
  gap: 0.25rem;
}

.page-number {
  padding: 0.5rem 0.75rem;
  border: 1px solid var(--color-text-sub);
  background: var(--color-bg-light, white);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.page-number.active {
  background: var(--color-primary);
  color: var(--color-bg-light, white);
  border-color: var(--color-primary);
}

.page-number:hover:not(.active) {
  background: var(--color-gray-100);
}

@media (max-width: 768px) {
  .module-store {
    padding: 1rem;
  }

  .store-header h1 {
    font-size: 2rem;
  }

  .store-controls {
    flex-direction: column;
  }

  .search-box {
    min-width: 100%;
  }

  .modules-list {
    grid-template-columns: 1fr;
  }
}
</style>