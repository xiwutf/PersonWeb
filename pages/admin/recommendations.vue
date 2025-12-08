<template>
  <div class="recommendations-page">
    <div class="recommendations-header">
      <h1 class="recommendations-title">AI 推荐系统</h1>
      <button @click="fetchRecommendations" class="recommendations-refresh-button">
        刷新推荐
      </button>
    </div>

    <!-- 推荐类型选择 -->
    <div class="recommendation-filters">
      <div class="recommendation-filters-container">
        <button
          v-for="type in recommendationTypes"
          :key="type.value"
          @click="selectedType = type.value; fetchRecommendations()"
          class="recommendation-filter-button"
          :class="{ 'recommendation-filter-button--active': selectedType === type.value }"
        >
          {{ type.label }}
        </button>
      </div>
    </div>

    <!-- 推荐内容 -->
    <div v-if="loading" class="recommendations-loading">AI 正在生成推荐...</div>
    
    <div v-else-if="recommendations" class="recommendations-content">
      <div class="recommendations-text">
        {{ recommendations }}
      </div>
    </div>

    <div v-else class="recommendations-empty">
      点击"刷新推荐"获取 AI 推荐
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

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

<style scoped>
.recommendations-page {
  padding: var(--spacing-xl);
}

.recommendations-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
}

.recommendations-title {
  font-size: var(--font-size-h2);
  font-weight: 700;
  color: var(--color-text-main);
}

.recommendations-refresh-button {
  padding: var(--spacing-sm) var(--spacing-md);
  background: var(--color-primary);
  color: #ffffff;
  border-radius: var(--radius-md);
  border: none;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.recommendations-refresh-button:hover {
  background: var(--color-primary-hover);
  transform: translateY(-1px);
  box-shadow: var(--shadow-md);
}

/* 推荐类型筛选 */
.recommendation-filters {
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--color-border-subtle);
  padding: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
}

.recommendation-filters-container {
  display: flex;
  gap: var(--spacing-sm);
  flex-wrap: wrap;
}

.recommendation-filter-button {
  padding: var(--spacing-sm) var(--spacing-md);
  border-radius: var(--radius-md);
  border: 2px solid var(--color-border-default);
  font-weight: 700;
  font-size: var(--font-size-body);
  cursor: pointer;
  transition: all 0.3s ease;
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}

.recommendation-filter-button:hover {
  background: var(--color-bg-card);
  border-color: var(--color-primary);
  color: var(--color-text-main);
  transform: translateY(-1px);
  box-shadow: var(--shadow-sm);
}

.recommendation-filter-button--active {
  background: var(--color-primary);
  border-color: var(--color-primary);
  color: #ffffff;
  box-shadow: var(--shadow-md);
}

.recommendation-filter-button--active:hover {
  background: var(--color-primary-hover);
  border-color: var(--color-primary-hover);
  color: #ffffff;
}

/* 推荐内容 */
.recommendations-loading {
  text-align: center;
  padding: var(--spacing-2xl) 0;
  color: var(--color-text-muted);
}

.recommendations-content {
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--color-border-subtle);
  padding: var(--spacing-lg);
}

.recommendations-text {
  white-space: pre-wrap;
  color: var(--color-text-main);
  line-height: 1.75;
}

.recommendations-empty {
  text-align: center;
  padding: var(--spacing-2xl) 0;
  color: var(--color-text-muted);
}
</style>

