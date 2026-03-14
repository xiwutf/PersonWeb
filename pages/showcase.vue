<template>
  <div class="showcase-page min-h-screen py-20">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- 页面标题 -->
      <div class="text-center mb-12">
        <h1 class="text-4xl md:text-5xl font-bold mb-4">个性化展示墙</h1>
        <p class="text-lg text-gray-600 dark:text-gray-400">
          记录想法、灵感、草图和代码片段
        </p>
      </div>

      <!-- 筛选栏 -->
      <div class="mb-8 flex flex-wrap gap-4 justify-center">
        <button
          v-for="category in categories"
          :key="category.value"
          @click="activeCategory = category.value"
          :class="[
            'px-4 py-2 rounded-lg transition-all',
            activeCategory === category.value
              ? 'bg-blue-600 text-var(--color-bg-light, white) shadow-lg'
              : 'bg-var(--color-bg-light, white) dark:bg-gray-800 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
          ]"
        >
          {{ category.label }}
        </button>
      </div>

      <!-- 瀑布流展示 -->
      <div class="showcase-grid">
        <div
          v-for="item in filteredItems"
          :key="item.id"
          class="showcase-item"
          :class="`item-${item.type}`"
          @click="openDetail(item)"
        >
          <div class="item-content">
            <!-- 图片类型 -->
            <div v-if="item.type === 'image'" class="item-image">
              <img :src="item.imageUrl" :alt="item.title" />
            </div>

            <!-- 代码片段类型 -->
            <div v-else-if="item.type === 'code'" class="item-code">
              <pre><code>{{ item.code }}</code></pre>
            </div>

            <!-- 文本类型 -->
            <div v-else class="item-text">
              <h3 class="item-title">{{ item.title }}</h3>
              <p class="item-description">{{ item.description }}</p>
            </div>

            <!-- 标签 -->
            <div v-if="item.tags && item.tags.length > 0" class="item-tags">
              <span
                v-for="tag in item.tags"
                :key="tag"
                class="item-tag"
              >
                {{ tag }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-if="filteredItems.length === 0" class="text-center py-20">
        <i class="fas fa-inbox text-6xl text-gray-300 mb-4"></i>
        <p class="text-gray-500">暂无内容</p>
      </div>
    </div>

    <!-- 详情模态框 -->
    <div v-if="selectedItem" class="modal-overlay" @click="closeDetail">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ selectedItem.title }}</h2>
          <button @click="closeDetail" class="modal-close">×</button>
        </div>
        <div class="modal-body">
          <div v-if="selectedItem.type === 'image'">
            <img :src="selectedItem.imageUrl" :alt="selectedItem.title" class="w-full rounded-lg" />
          </div>
          <div v-else-if="selectedItem.type === 'code'">
            <pre class="code-block"><code>{{ selectedItem.code }}</code></pre>
          </div>
          <div v-else>
            <p>{{ selectedItem.description }}</p>
          </div>
          <div v-if="selectedItem.tags" class="mt-4">
            <span
              v-for="tag in selectedItem.tags"
              :key="tag"
              class="tag"
            >
              {{ tag }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

interface ShowcaseItem {
  id: number
  type: 'idea' | 'inspiration' | 'sketch' | 'code' | 'image'
  title: string
  description?: string
  imageUrl?: string
  code?: string
  tags?: string[]
  createdAt: string
}

const categories = [
  { label: '全部', value: 'all' },
  { label: '想法', value: 'idea' },
  { label: '灵感', value: 'inspiration' },
  { label: '草图', value: 'sketch' },
  { label: '代码', value: 'code' },
  { label: '图片', value: 'image' }
]

const activeCategory = ref('all')
const items = ref<ShowcaseItem[]>([])
const selectedItem = ref<ShowcaseItem | null>(null)

const filteredItems = computed(() => {
  if (activeCategory.value === 'all') {
    return items.value
  }
  return items.value.filter(item => item.type === activeCategory.value)
})

const openDetail = (item: ShowcaseItem) => {
  selectedItem.value = item
}

const closeDetail = () => {
  selectedItem.value = null
}

// TODO: 从API加载数据
onMounted(() => {
  // 示例数据
  items.value = [
    {
      id: 1,
      type: 'idea',
      title: 'AI 助手优化想法',
      description: '为AI助手添加上下文记忆功能',
      tags: ['AI', '优化'],
      createdAt: '2024-01-15'
    },
    {
      id: 2,
      type: 'code',
      title: '优雅的错误处理',
      code: 'try {\n  // code\n} catch (e) {\n  handleError(e)\n}',
      tags: ['代码', '最佳实践'],
      createdAt: '2024-01-20'
    }
  ]
})
</script>

<style scoped>
.showcase-page {
  background: var(--theme-bg, var(--color-bg-card));
  color: var(--theme-text, var(--color-gray-800));
}

.showcase-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
  padding: 1rem 0;
}

.showcase-item {
  background: var(--theme-card-bg, var(--color-bg-card));
  border: 1px solid var(--theme-border, var(--color-border));
  border-radius: 1rem;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s ease;
}

.showcase-item:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px var(--shadow);
}

.item-content {
  padding: 1.5rem;
}

.item-image img {
  width: 100%;
  height: auto;
  border-radius: 0.5rem;
}

.item-code {
  background: var(--color-bg-dark);
  color: var(--color-text-muted);
  padding: 1rem;
  border-radius: 0.5rem;
  overflow-x: auto;
  font-family: 'Courier New', monospace;
  font-size: 0.875rem;
}

.item-text {
  min-height: 120px;
}

.item-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: var(--theme-text, var(--color-gray-800));
}

.item-description {
  color: rgba(0, 0, 0, 0.6);
  line-height: 1.6;
}

.item-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 1rem;
}

.item-tag {
  padding: 0.25rem 0.75rem;
  background: rgba(59, 130, 246, 0.1);
  color: var(--color-primary);
  border-radius: 0.25rem;
  font-size: 0.75rem;
}

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
  background: var(--color-bg-light, white);
  border-radius: 1rem;
  max-width: 90vw;
  max-height: 90vh;
  overflow-y: auto;
  padding: 2rem;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.modal-close {
  width: 2rem;
  height: 2rem;
  border-radius: 50%;
  background: var(--shadow);
  border: none;
  cursor: pointer;
  font-size: 1.5rem;
}

.code-block {
  background: var(--color-gray-900);
  color: var(--color-gray-300);
  padding: 1.5rem;
  border-radius: 0.5rem;
  overflow-x: auto;
}
</style>

