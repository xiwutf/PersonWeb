<template>
  <ClientOnly>
    <div class="name-tool-page">
    <!-- 页面头部 -->
    <div class="name-tool-header">
      <h1 class="name-tool-title">智能取名助手</h1>
      <p class="name-tool-subtitle">游戏�?/ 网名 / 英文名，一键生成可收藏</p>
    </div>

    <div class="name-tool-content">
      <!-- 左侧筛选表�?-->
      <n-card class="name-tool-form-card">
        <template #header>
          <span>筛选条�?/span>
        </template>
        <n-form :model="formData" :rules="formRules" ref="formRef" label-placement="top" @submit.prevent>
          <!-- 取名类型 -->
          <n-form-item label="取名类型" path="type">
            <n-select
              v-model:value="formData.type"
              :options="typeOptions"
              placeholder="请选择取名类型"
            />
          </n-form-item>

          <!-- 风格 -->
          <n-form-item label="风格" path="style">
            <n-select
              v-model:value="formData.style"
              :options="styleOptions"
              multiple
              placeholder="请选择风格（可多选）"
            />
          </n-form-item>

          <!-- 性别 -->
          <n-form-item label="性别" path="gender">
            <n-select
              v-model:value="formData.gender"
              :options="genderOptions"
              placeholder="可�?
              clearable
            />
          </n-form-item>

          <!-- 名字长度 -->
          <n-form-item label="名字长度" path="length">
            <n-select
              v-model:value="formData.length"
              :options="lengthOptions"
              placeholder="可�?
              clearable
            />
          </n-form-item>

          <!-- 关键�?-->
          <n-form-item label="关键�? path="keywords">
            <n-input
              v-model:value="formData.keywords"
              placeholder="多个关键词用逗号分隔（可选）"
              clearable
            />
          </n-form-item>

          <!-- 语言 -->
          <n-form-item label="语言" path="language">
            <n-select
              v-model:value="formData.language"
              :options="languageOptions"
              placeholder="默认自动"
            />
          </n-form-item>

          <!-- 禁用�?-->
          <n-form-item label="禁用�? path="banned">
            <n-input
              v-model:value="formData.banned"
              placeholder="不希望出现的词，多个用逗号分隔（可选）"
              clearable
            />
          </n-form-item>

          <!-- 操作按钮 -->
          <n-form-item>
            <n-space>
              <n-button
                type="primary"
                :loading="generating"
                @click="handleGenerate"
                :disabled="!canGenerate"
              >
                生成
              </n-button>
              <n-button
                secondary
                :loading="regenerating"
                @click="handleRegenerate"
                :disabled="!canRegenerate"
              >
                再来一�?              </n-button>
              <n-button quaternary @click="handleReset">重置</n-button>
            </n-space>
          </n-form-item>
        </n-form>
      </n-card>

      <!-- 右侧结果�?-->
      <n-card class="name-tool-result-card">
        <template #header>
          <div class="name-tool-result-header">
            <div>
              <span>结果列表</span>
              <n-text depth="3" class="name-tool-result-count">
                （本�?{{ results.length }} 个）
              </n-text>
            </div>
            <div class="name-tool-result-actions">
              <n-select
                v-model:value="sortBy"
                :options="sortOptions"
                style="width: 150px; margin-right: 12px;"
                size="small"
              />
              <n-button size="small" @click="showFavorites = true">
                我的收藏
              </n-button>
            </div>
          </div>
        </template>

        <!-- 加载状�?-->
        <div v-if="generating || regenerating" class="name-tool-loading">
          <n-spin size="large" />
          <div class="name-tool-loading-content">
            <p class="name-tool-loading-text">正在生成名字...</p>
            <p class="name-tool-loading-hint">AI 正在思考中，请稍候（通常需�?30-90 秒）</p>
            <n-progress
              type="line"
              :percentage="loadingProgress"
              :show-indicator="false"
              status="success"
              :height="4"
              style="max-width: 400px; margin-top: 16px;"
            />
            <p class="name-tool-loading-tip">{{ loadingTip }}</p>
          </div>
        </div>

        <!-- 结果列表 -->
        <div v-else-if="results.length > 0" class="name-tool-results">
          <TransitionGroup name="name-item">
            <div
              v-for="(item, index) in sortedResults"
              :key="`${item.name}-${index}`"
              class="name-item-card"
            >
              <!-- 名字和总分 -->
              <div class="name-item-header">
                <h3 class="name-item-name">{{ item.name }}</h3>
                <n-tag :type="getScoreTagType(item.totalScore)" size="large">
                  总分: {{ item.totalScore }}
                </n-tag>
              </div>

              <!-- 四个维度�?-->
              <div class="name-item-scores">
                <n-tag size="small" type="info">好记�? {{ item.scores.memorability }}</n-tag>
                <n-tag size="small" type="success">独特�? {{ item.scores.uniqueness }}</n-tag>
                <n-tag size="small" type="warning">贴合�? {{ item.scores.fit }}</n-tag>
                <n-tag size="small" type="error">美观�? {{ item.scores.aesthetics }}</n-tag>
              </div>

              <!-- 理由 -->
              <div class="name-item-reason">
                <n-text depth="3">{{ item.reason }}</n-text>
              </div>

              <!-- 标签 -->
              <div v-if="item.tags && item.tags.length > 0" class="name-item-tags">
                <n-tag
                  v-for="tag in item.tags"
                  :key="tag"
                  size="small"
                  type="default"
                >
                  {{ tag }}
                </n-tag>
              </div>

              <!-- 操作按钮 -->
              <div class="name-item-actions">
                <n-button size="small" @click="handleCopy(item.name)">
                  复制
                </n-button>
                <n-button
                  size="small"
                  :type="isFavorite(item.name) ? 'primary' : 'default'"
                  @click="handleToggleFavorite(item)"
                >
                  {{ isFavorite(item.name) ? '已收�? : '收藏' }}
                </n-button>
              </div>
            </div>
          </TransitionGroup>
        </div>

        <!-- 空状�?-->
        <n-empty v-else description="请填写筛选条件并点击生成按钮" />
      </n-card>
    </div>

    <!-- 收藏抽屉 -->
    <n-drawer v-model:show="showFavorites" :width="400" placement="right">
      <template #header>
        <span>我的收藏</span>
      </template>
      <div v-if="favoritesLoading" class="name-tool-loading">
        <n-spin size="large" />
      </div>
      <div v-else-if="favorites.length === 0" class="name-tool-empty">
        <n-empty description="暂无收藏" />
      </div>
      <div v-else class="name-tool-favorites">
        <div
          v-for="fav in favorites"
          :key="fav.id"
          class="name-favorite-item"
        >
          <div class="name-favorite-header">
            <h4>{{ fav.name }}</h4>
            <n-tag size="small">{{ fav.type }}</n-tag>
          </div>
          <div class="name-favorite-meta">
            <n-text depth="3">风格: {{ fav.style }}</n-text>
            <n-text depth="3">评分: {{ fav.totalScore }}</n-text>
            <n-text depth="3">{{ formatDate(fav.createdAt) }}</n-text>
          </div>
          <div class="name-favorite-reason">
            <n-text depth="3">{{ fav.reason }}</n-text>
          </div>
          <div class="name-favorite-actions">
            <n-button size="small" @click="handleCopy(fav.name)">
              复制
            </n-button>
            <n-button size="small" type="error" @click="handleRemoveFavorite(fav.id)">
              取消收藏
            </n-button>
          </div>
        </div>
      </div>
    </n-drawer>
    </div>

    <template #fallback>
      <div class="name-tool-page">
        <div class="name-tool-loading">
          <n-spin size="large" />
          <p>正在加载...</p>
        </div>
      </div>
    </template>
  </ClientOnly>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import type { FormInst } from 'naive-ui'
import {
  NCard,
  NForm,
  NFormItem,
  NSelect,
  NInput,
  NButton,
  NSpace,
  NTag,
  NText,
  NEmpty,
  NSpin,
  NDrawer,
  NProgress
} from 'naive-ui'
import type {
  NameGenerateRequest,
  NameGenerateResponse,
  NameItem,
  NameFavorite,
  FavoriteCreateRequest,
  SortBy
} from '~/types/name-tool'
import { useNameTool } from '~/composables/useNameTool'

// 使用 default 布局
definePageMeta({
  layout: 'default'
})

const nameTool = useNameTool()
const formRef = ref<FormInst | null>(null)

// 表单数据
const formData = ref<NameGenerateRequest>({
  type: 'game',
  style: ['霸气'],
  gender: undefined,
  length: undefined,
  keywords: undefined,
  language: '自动',
  banned: undefined,
  traceId: undefined
})

// 表单验证规则
const formRules = {
  type: {
    required: true,
    message: '请选择取名类型',
    trigger: 'change'
  },
  style: {
    required: true,
    type: 'array',
    min: 1,
    message: '请至少选择一个风�?,
    trigger: 'change'
  }
}

// 选项数据
const typeOptions = [
  { label: '游戏�?, value: 'game' },
  { label: '网名', value: 'nickname' },
  { label: '英文�?, value: 'english' },
  { label: '品牌�?, value: 'brand' },
  { label: '产品�?, value: 'product' },
  { label: '团队�?, value: 'team' },
  { label: '项目�?, value: 'project' },
  { label: '角色�?, value: 'character' }
]

const styleOptions = [
  { label: '霸气', value: '霸气' },
  { label: '可爱', value: '可爱' },
  { label: '文艺', value: '文艺' },
  { label: '搞笑', value: '搞笑' },
  { label: '克制', value: '克制' },
  { label: '科幻', value: '科幻' },
  { label: '二次�?, value: '二次�? },
  { label: '古风', value: '古风' },
  { label: '赛博', value: '赛博' },
  { label: '优雅', value: '优雅' },
  { label: '简�?, value: '简�? },
  { label: '神秘', value: '神秘' },
  { label: '温暖', value: '温暖' },
  { label: '冷酷', value: '冷酷' },
  { label: '活泼', value: '活泼' },
  { label: '沉稳', value: '沉稳' },
  { label: '浪漫', value: '浪漫' },
  { label: '现代', value: '现代' },
  { label: '复古', value: '复古' },
  { label: '未来', value: '未来' },
  { label: '自然', value: '自然' },
  { label: '梦幻', value: '梦幻' }
]

const genderOptions = [
  { label: '�?, value: '�? },
  { label: '�?, value: '�? },
  { label: '中�?, value: '中�? }
]

const lengthOptions = [
  { label: '短（1-2字）', value: '�? },
  { label: '中（3-4字）', value: '�? },
  { label: '长（5-6字）', value: '�? },
  { label: '超长�?字以上）', value: '超长' }
]

const languageOptions = [
  { label: '自动', value: '自动' },
  { label: '中文', value: '中文' },
  { label: '英文', value: '英文' },
  { label: '混合', value: '混合' },
  { label: '日文', value: '日文' },
  { label: '韩文', value: '韩文' },
  { label: '数字', value: '数字' },
  { label: '符号', value: '符号' }
]

const sortOptions = [
  { label: '总分从高到低', value: 'totalScore' },
  { label: '独特性从高到�?, value: 'uniqueness' },
  { label: '好记度从高到�?, value: 'memorability' },
  { label: '贴合度从高到�?, value: 'fit' },
  { label: '美观度从高到�?, value: 'aesthetics' }
]

// 状�?const generating = ref(false)
const regenerating = ref(false)
const results = ref<NameItem[]>([])
const traceId = ref<string | undefined>(undefined)
const sortBy = ref<SortBy>('totalScore')
const showFavorites = ref(false)
const favorites = ref<NameFavorite[]>([])
const favoritesLoading = ref(false)
const favoriteNames = ref<Set<string>>(new Set())

// 加载进度和提�?const loadingProgress = ref(0)
const loadingTip = ref('正在准备生成...')
const loadingTips = [
  '正在分析你的需�?..',
  '正在构建 Prompt...',
  '正在调用 AI 模型...',
  'AI 正在思考中...',
  '正在生成名字列表...',
  '正在评分和筛�?..',
  '即将完成...'
]
let loadingInterval: NodeJS.Timeout | null = null

// 计算属�?const canGenerate = computed(() => {
  return formData.value.type && formData.value.style && formData.value.style.length > 0
})

const canRegenerate = computed(() => {
  return canGenerate.value && traceId.value !== undefined && results.value.length > 0
})

const sortedResults = computed(() => {
  const sorted = [...results.value]
  switch (sortBy.value) {
    case 'totalScore':
      return sorted.sort((a, b) => b.totalScore - a.totalScore)
    case 'uniqueness':
      return sorted.sort((a, b) => b.scores.uniqueness - a.scores.uniqueness)
    case 'memorability':
      return sorted.sort((a, b) => b.scores.memorability - a.scores.memorability)
    case 'fit':
      return sorted.sort((a, b) => b.scores.fit - a.scores.fit)
    case 'aesthetics':
      return sorted.sort((a, b) => b.scores.aesthetics - a.scores.aesthetics)
    default:
      return sorted
  }
})

// 开始加载动�?const startLoadingAnimation = () => {
  loadingProgress.value = 0
  let tipIndex = 0
  loadingTip.value = loadingTips[tipIndex]
  
  // 模拟进度（实际进度由后端控制，这里只是视觉反馈）
  loadingInterval = setInterval(() => {
    if (loadingProgress.value < 90) {
      loadingProgress.value += Math.random() * 5
      if (loadingProgress.value > 90) {
        loadingProgress.value = 90
      }
    }
    
    // �?10 秒切换一次提�?    if (tipIndex < loadingTips.length - 1 && loadingProgress.value > (tipIndex + 1) * 12) {
      tipIndex++
      loadingTip.value = loadingTips[tipIndex]
    }
  }, 1000)
}

// 停止加载动画
const stopLoadingAnimation = () => {
  if (loadingInterval) {
    clearInterval(loadingInterval)
    loadingInterval = null
  }
  loadingProgress.value = 100
  loadingTip.value = '完成�?
  setTimeout(() => {
    loadingProgress.value = 0
    loadingTip.value = '正在准备生成...'
  }, 500)
}

// 方法
const handleGenerate = async (e?: Event) => {
  if (e) {
    e.preventDefault()
    e.stopPropagation()
  }
  if (!formRef.value) return
  await formRef.value.validate(async (errors) => {
    if (!errors) {
      generating.value = true
      startLoadingAnimation()
      try {
        const response = await nameTool.generateNames(formData.value)
        if (response) {
          results.value = response.items
          traceId.value = response.traceId
          formData.value.traceId = response.traceId
        }
      } catch (error) {
        console.error('生成名字时出�?', error)
      } finally {
        stopLoadingAnimation()
        generating.value = false
      }
    }
  })
}

const handleRegenerate = async () => {
  if (!traceId.value) return
  regenerating.value = true
  startLoadingAnimation()
  try {
    const request: NameGenerateRequest = {
      ...formData.value,
      traceId: traceId.value
    }
    const response = await nameTool.regenerateNames(request)
    if (response) {
      results.value = response.items
      // traceId 保持不变
    }
  } finally {
    stopLoadingAnimation()
    regenerating.value = false
  }
}

const handleReset = () => {
  formData.value = {
    type: 'game',
    style: ['霸气'],
    gender: undefined,
    length: undefined,
    keywords: undefined,
    language: '自动',
    banned: undefined,
    traceId: undefined
  }
  results.value = []
  traceId.value = undefined
  formRef.value?.restoreValidation()
}

const handleCopy = async (name: string) => {
  await nameTool.copyName(name)
}

const handleToggleFavorite = async (item: NameItem) => {
  if (isFavorite(item.name)) {
    // 取消收藏：需要从收藏列表中找到对应的ID
    const favorite = favorites.value.find(f => f.name === item.name)
    if (favorite) {
      await handleRemoveFavorite(favorite.id)
    }
  } else {
    // 添加收藏
    const favoriteRequest: FavoriteCreateRequest = {
      name: item.name,
      type: formData.value.type!,
      style: formData.value.style!,
      language: formData.value.language || '自动',
      totalScore: item.totalScore,
      reason: item.reason,
      scores: item.scores,
      requestMeta: {
        type: formData.value.type,
        style: formData.value.style,
        gender: formData.value.gender,
        length: formData.value.length,
        keywords: formData.value.keywords,
        language: formData.value.language,
        banned: formData.value.banned
      }
    }
    const favorite = await nameTool.addFavorite(favoriteRequest)
    if (favorite) {
      favoriteNames.value.add(item.name)
      await loadFavorites()
    }
  }
}

const handleRemoveFavorite = async (id: number) => {
  const success = await nameTool.removeFavorite(id)
  if (success) {
    await loadFavorites()
  }
}

const isFavorite = (name: string): boolean => {
  return favoriteNames.value.has(name)
}

const loadFavorites = async () => {
  favoritesLoading.value = true
  try {
    const response = await nameTool.getFavorites(1, 100)
    if (response) {
      favorites.value = response.items
      favoriteNames.value = new Set(response.items.map(f => f.name))
    }
  } finally {
    favoritesLoading.value = false
  }
}

const getScoreTagType = (score: number): 'success' | 'warning' | 'error' | 'info' => {
  if (score >= 80) return 'success'
  if (score >= 60) return 'warning'
  if (score >= 40) return 'info'
  return 'error'
}

const formatDate = (dateString: string): string => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 监听收藏抽屉打开
watch(showFavorites, (newVal) => {
  if (newVal) {
    loadFavorites()
  }
})

onMounted(() => {
  // 页面加载时预加载收藏列表（用于判断是否已收藏�?  loadFavorites()
})

useHead({
  title: '智能取名助手 - 溪午听风',
  meta: [
    { name: 'description', content: '游戏名、网名、英文名一键生成，支持评分和收�? }
  ]
})
</script>

<style scoped>
.name-tool-page {
  min-height: 100vh;
  padding: var(--spacing-xl);
  background-color: var(--color-bg-page);
}

.name-tool-header {
  text-align: center;
  margin-bottom: var(--spacing-xl);
}

.name-tool-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-sm);
}

.name-tool-subtitle {
  font-size: var(--font-size-body);
  color: var(--color-text-muted);
}

.name-tool-content {
  display: grid;
  grid-template-columns: 400px 1fr;
  gap: var(--spacing-lg);
  max-width: 1400px;
  margin: 0 auto;
}

.name-tool-form-card {
  height: fit-content;
}

.name-tool-result-card {
  min-height: 600px;
}

.name-tool-result-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.name-tool-result-count {
  margin-left: var(--spacing-sm);
}

.name-tool-result-actions {
  display: flex;
  align-items: center;
}

.name-tool-loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-2xl);
  color: var(--color-text-muted);
  min-height: 400px;
}

.name-tool-loading-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
}

.name-tool-loading-text {
  font-size: 16px;
  font-weight: 500;
  color: var(--color-text-main);
  margin: 0;
}

.name-tool-loading-hint {
  font-size: 14px;
  color: var(--color-text-muted);
  margin: 0;
}

.name-tool-loading-tip {
  font-size: 12px;
  color: var(--color-text-muted);
  margin: 8px 0 0 0;
  opacity: 0.8;
}

.name-tool-results {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.name-item-card {
  padding: var(--spacing-md);
  background-color: var(--color-bg-elevated);
  border-radius: var(--radius-lg);
  border: 1px solid var(--color-border-subtle);
  transition: all 0.2s ease;
}

.name-item-card:hover {
  border-color: var(--color-border-default);
  box-shadow: var(--shadow-md);
}

.name-item-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-sm);
}

.name-item-name {
  font-size: var(--font-size-h3);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

.name-item-scores {
  display: flex;
  gap: var(--spacing-xs);
  margin-bottom: var(--spacing-sm);
  flex-wrap: wrap;
}

.name-item-reason {
  margin-bottom: var(--spacing-sm);
  font-size: var(--font-size-sm);
}

.name-item-tags {
  display: flex;
  gap: var(--spacing-xs);
  margin-bottom: var(--spacing-sm);
  flex-wrap: wrap;
}

.name-item-actions {
  display: flex;
  gap: var(--spacing-sm);
  margin-top: var(--spacing-sm);
}

.name-tool-favorites {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.name-favorite-item {
  padding: var(--spacing-md);
  background-color: var(--color-bg-elevated);
  border-radius: var(--radius-lg);
  border: 1px solid var(--color-border-subtle);
}

.name-favorite-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xs);
}

.name-favorite-header h4 {
  margin: 0;
  font-size: var(--font-size-h4);
  color: var(--color-text-main);
}

.name-favorite-meta {
  display: flex;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-xs);
  font-size: var(--font-size-sm);
}

.name-favorite-reason {
  margin-bottom: var(--spacing-sm);
  font-size: var(--font-size-sm);
}

.name-favorite-actions {
  display: flex;
  gap: var(--spacing-sm);
}

.name-tool-empty {
  padding: var(--spacing-2xl);
  text-align: center;
}

/* 过渡动画 */
.name-item-enter-active,
.name-item-leave-active {
  transition: all 0.3s ease;
}

.name-item-enter-from {
  opacity: 0;
  transform: translateY(-10px);
}

.name-item-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

/* 响应�?*/
@media (max-width: 1024px) {
  .name-tool-content {
    grid-template-columns: 1fr;
  }
}
</style>

