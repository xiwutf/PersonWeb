<template>
  <div class="bg-white rounded-2xl shadow-lg p-6 mb-8">
    <div class="flex flex-col lg:flex-row gap-6">
      <!-- 搜索框 -->
      <div class="flex-1">
        <label class="block text-sm font-medium text-gray-700 mb-2">搜索工具</label>
        <div class="relative">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="输入工具名称或功能关键词..."
            class="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-orange-500 focus:border-orange-500"
          >
          <svg class="absolute left-3 top-3.5 h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
          </svg>
        </div>
      </div>

      <!-- 价格筛选 -->
      <div class="lg:w-48">
        <label class="block text-sm font-medium text-gray-700 mb-2">价格范围</label>
        <select
          v-model="priceRange"
          class="w-full py-3 px-4 border border-gray-300 rounded-xl focus:ring-2 focus:ring-orange-500 focus:border-orange-500"
        >
          <option value="">全部价格</option>
          <option value="0-20">¥0 - ¥20</option>
          <option value="20-50">¥20 - ¥50</option>
          <option value="50-100">¥50 - ¥100</option>
          <option value="100+">¥100+</option>
        </select>
      </div>
    </div>

    <!-- 标签筛选 -->
    <div class="mt-6">
      <label class="block text-sm font-medium text-gray-700 mb-3">按标签筛选</label>
      <div class="flex flex-wrap gap-2">
        <button
          @click="selectedTag = ''"
          :class="[
            'px-4 py-2 rounded-full text-sm font-medium transition-all',
            selectedTag === ''
              ? 'bg-orange-600 text-white'
              : 'bg-gray-100 text-gray-600 hover:bg-orange-100 hover:text-orange-600'
          ]"
        >
          全部
        </button>
        <button
          v-for="tag in availableTags"
          :key="tag"
          @click="selectedTag = tag"
          :class="[
            'px-4 py-2 rounded-full text-sm font-medium transition-all',
            selectedTag === tag
              ? 'bg-orange-600 text-white'
              : 'bg-gray-100 text-gray-600 hover:bg-orange-100 hover:text-orange-600'
          ]"
        >
          {{ tag }}
        </button>
      </div>
    </div>

    <!-- 筛选结果统计 -->
    <div class="mt-4 pt-4 border-t border-gray-200">
      <p class="text-sm text-gray-600">
        找到 <span class="font-semibold text-orange-600">{{ filteredCount }}</span> 个工具
        <button
          v-if="hasActiveFilters"
          @click="clearFilters"
          class="ml-4 text-orange-600 hover:text-orange-800 text-sm underline"
        >
          清除筛选
        </button>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  tools: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['filtered'])

// 筛选状态
const searchQuery = ref('')
const selectedTag = ref('')
const priceRange = ref('')

// 可用标签（从工具数据中提取）
const availableTags = computed(() => {
  const tags = new Set()
  props.tools.forEach(tool => {
    tool.tags?.forEach(tag => tags.add(tag))
  })
  return Array.from(tags).sort()
})

// 筛选逻辑
const filteredTools = computed(() => {
  let filtered = props.tools

  // 搜索筛选
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(tool => 
      tool.title.toLowerCase().includes(query) ||
      tool.description.toLowerCase().includes(query) ||
      tool.tags?.some(tag => tag.toLowerCase().includes(query))
    )
  }

  // 标签筛选
  if (selectedTag.value) {
    filtered = filtered.filter(tool => 
      tool.tags?.includes(selectedTag.value)
    )
  }

  // 价格筛选
  if (priceRange.value) {
    filtered = filtered.filter(tool => {
      const price = parseFloat(tool.price)
      switch (priceRange.value) {
        case '0-20':
          return price >= 0 && price <= 20
        case '20-50':
          return price > 20 && price <= 50
        case '50-100':
          return price > 50 && price <= 100
        case '100+':
          return price > 100
        default:
          return true
      }
    })
  }

  return filtered
})

// 筛选结果数量
const filteredCount = computed(() => filteredTools.value.length)

// 是否有激活的筛选条件
const hasActiveFilters = computed(() => 
  searchQuery.value || selectedTag.value || priceRange.value
)

// 清除所有筛选
const clearFilters = () => {
  searchQuery.value = ''
  selectedTag.value = ''
  priceRange.value = ''
}

// 监听筛选结果变化，向父组件发送事件
watch(filteredTools, (newFiltered) => {
  emit('filtered', newFiltered)
}, { immediate: true })
</script> 