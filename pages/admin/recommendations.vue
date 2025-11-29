<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">AI 推荐系统</h1>
      <button @click="fetchRecommendations" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
        刷新推荐
      </button>
    </div>

    <!-- 推荐类型选择 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 mb-6">
      <div class="flex gap-2">
        <button
          v-for="type in recommendationTypes"
          :key="type.value"
          @click="selectedType = type.value; fetchRecommendations()"
          class="px-4 py-2 rounded transition-colors font-medium"
          :class="selectedType === type.value 
            ? 'bg-blue-600 text-white' 
            : 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-white hover:bg-gray-200 dark:hover:bg-gray-600'"
        >
          {{ type.label }}
        </button>
      </div>
    </div>

    <!-- 推荐内容 -->
    <div v-if="loading" class="text-center py-8 text-gray-500">AI 正在生成推荐...</div>
    
    <div v-else-if="recommendations" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <div class="prose dark:prose-invert max-w-none">
        <div class="whitespace-pre-wrap text-gray-700 dark:text-gray-300">{{ recommendations }}</div>
      </div>
    </div>

    <div v-else class="text-center py-8 text-gray-500">
      点击"刷新推荐"获取 AI 推荐
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const recommendations = ref<string>('')
const loading = ref(false)
const selectedType = ref('all')

const recommendationTypes = [
  { value: 'all', label: '全部推荐' },
  { value: 'books', label: '书籍推荐' },
  { value: 'articles', label: '文章推荐' },
  { value: 'features', label: '功能推荐' }
]

const fetchRecommendations = async () => {
  loading.value = true
  recommendations.value = ''
  
  try {
    const res = await api.get<any>('/Recommendation/recommendations', {
      params: { type: selectedType.value }
    })
    
    if (res && res.recommendations) {
      recommendations.value = res.recommendations
    } else if (typeof res === 'string') {
      recommendations.value = res
    }
  } catch (e: any) {
    console.error('Failed to fetch recommendations:', e)
    recommendations.value = '获取推荐失败，请稍后重试。'
  } finally {
    loading.value = false
  }
}
</script>

