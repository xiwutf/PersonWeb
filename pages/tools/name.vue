<template>
  <ClientOnly>
    <div class="name-tool-page">
    <!-- ???? -->
    <div class="name-tool-header">
      <h1 class="name-tool-title">??????</h1>
      <p class="name-tool-subtitle">????/ ?? / ???????????</p>
    </div>

    <div class="name-tool-content">
      <!-- ???????-->
      <n-card class="name-tool-form-card">
        <template #header>
          <span>????</span>
        </template>
        <n-form :model="formData" :rules="formRules" ref="formRef" label-placement="top" @submit.prevent>
          <!-- ???? -->
          <n-form-item label="????" path="type">
            <n-select
              v-model:value="formData.type"
              :options="typeOptions"
              placeholder="???????"
            />
          </n-form-item>

          <!-- ?? -->
          <n-form-item label="??" path="style">
            <n-select
              v-model:value="formData.style"
              :options="styleOptions"
              multiple
              placeholder="??????????"
            />
          </n-form-item>

          <!-- ?? -->
          <n-form-item label="??" path="gender">
            <n-select
              v-model:value="formData.gender"
              :options="genderOptions"
              placeholder="??"
              clearable
            />
          </n-form-item>

          <!-- ???? -->
          <n-form-item label="????" path="length">
            <n-select
              v-model:value="formData.length"
              :options="lengthOptions"
              placeholder="??"
              clearable
            />
          </n-form-item>

          <!-- ????-->
          <n-form-item label="???" path="keywords">
            <n-input
              v-model:value="formData.keywords"
              placeholder="??????????????"
              clearable
            />
          </n-form-item>

          <!-- ?? -->
          <n-form-item label="??" path="language">
            <n-select
              v-model:value="formData.language"
              :options="languageOptions"
              placeholder="????"
            />
          </n-form-item>

          <!-- ????-->
          <n-form-item label="???" path="banned">
            <n-input
              v-model:value="formData.banned"
              placeholder="???????????????????"
              clearable
            />
          </n-form-item>

          <!-- ???? -->
          <n-form-item>
            <n-space>
              <n-button
                type="primary"
                :loading="generating"
                @click="handleGenerate"
                :disabled="!canGenerate"
              >
                ??
              </n-button>
              <n-button
                secondary
                :loading="regenerating"
                @click="handleRegenerate"
                :disabled="!canRegenerate"
              >
                ?????              </n-button>
              <n-button quaternary @click="handleReset">??</n-button>
            </n-space>
          </n-form-item>
        </n-form>
      </n-card>

      <!-- ??????-->
      <n-card class="name-tool-result-card">
        <template #header>
          <div class="name-tool-result-header">
            <div>
              <span>????</span>
              <n-text depth="3" class="name-tool-result-count">
                ????{{ results.length }} ??
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
                ????
              </n-button>
            </div>
          </div>
        </template>

        <!-- ?????-->
        <div v-if="generating || regenerating" class="name-tool-loading">
          <n-spin size="large" />
          <div class="name-tool-loading-content">
            <p class="name-tool-loading-text">??????...</p>
            <p class="name-tool-loading-hint">AI ???????????????30-90 ??</p>
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

        <!-- ???? -->
        <div v-else-if="results.length > 0" class="name-tool-results">
          <TransitionGroup name="name-item">
            <div
              v-for="(item, index) in sortedResults"
              :key="`${item.name}-${index}`"
              class="name-item-card"
            >
              <!-- ????? -->
              <div class="name-item-header">
                <h3 class="name-item-name">{{ item.name }}</h3>
                <n-tag :type="getScoreTagType(item.totalScore)" size="large">
                  ??: {{ item.totalScore }}
                </n-tag>
              </div>

              <!-- ??????-->
              <div class="name-item-scores">
                <n-tag size="small" type="info">???? {{ item.scores.memorability }}</n-tag>
                <n-tag size="small" type="success">???? {{ item.scores.uniqueness }}</n-tag>
                <n-tag size="small" type="warning">???? {{ item.scores.fit }}</n-tag>
                <n-tag size="small" type="error">???? {{ item.scores.aesthetics }}</n-tag>
              </div>

              <!-- ?? -->
              <div class="name-item-reason">
                <n-text depth="3">{{ item.reason }}</n-text>
              </div>

              <!-- ?? -->
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

              <!-- ???? -->
              <div class="name-item-actions">
                <n-button size="small" @click="handleCopy(item.name)">
                  ??
                </n-button>
                <n-button
                  size="small"
                  :type="isFavorite(item.name) ? 'primary' : 'default'"
                  @click="handleToggleFavorite(item)"
                >
                  {{ isFavorite(item.name) ? '???' : '??' }}
                </n-button>
              </div>
            </div>
          </TransitionGroup>
        </div>

        <!-- ????-->
        <n-empty v-else description="??????????????" />
      </n-card>
    </div>

    <!-- ???? -->
    <n-drawer v-model:show="showFavorites" :width="400" placement="right">
      <template #header>
        <span>????</span>
      </template>
      <div v-if="favoritesLoading" class="name-tool-loading">
        <n-spin size="large" />
      </div>
      <div v-else-if="favorites.length === 0" class="name-tool-empty">
        <n-empty description="????" />
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
            <n-text depth="3">??: {{ fav.style }}</n-text>
            <n-text depth="3">??: {{ fav.totalScore }}</n-text>
            <n-text depth="3">{{ formatDate(fav.createdAt) }}</n-text>
          </div>
          <div class="name-favorite-reason">
            <n-text depth="3">{{ fav.reason }}</n-text>
          </div>
          <div class="name-favorite-actions">
            <n-button size="small" @click="handleCopy(fav.name)">
              ??
            </n-button>
            <n-button size="small" type="error" @click="handleRemoveFavorite(fav.id)">
              ????
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
          <p>????...</p>
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

// ?? default ??
definePageMeta({
  layout: 'default'
})

const nameTool = useNameTool()
const formRef = ref<FormInst | null>(null)

// ????
const formData = ref<NameGenerateRequest>({
  type: 'game',
  style: ['??'],
  gender: undefined,
  length: undefined,
  keywords: undefined,
  language: '??',
  banned: undefined,
  traceId: undefined
})

// ??????
const formRules = {
  type: {
    required: true,
    message: '???????',
    trigger: 'change'
  },
  style: {
    required: true,
    type: 'array',
    min: 1,
    message: '???????',
    trigger: 'change'
  }
}

// ????
const typeOptions = [
  { label: '???', value: 'game' },
  { label: '??', value: 'nickname' },
  { label: '???', value: 'english' },
  { label: '??', value: 'brand' },
  { label: '??', value: 'product' },
  { label: '??', value: 'team' },
  { label: '??', value: 'project' },
  { label: '??', value: 'character' }
]

const styleOptions = [
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: 'ancient' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: 'poetic' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' },
  { label: '??', value: '??' }
]

const genderOptions = [
  { label: '?', value: 'male' },
  { label: '?', value: 'female' },
  { label: '??', value: 'neutral' }
]

const lengthOptions = [
  { label: '1-2?', value: 'short' },
  { label: '3-4?', value: 'medium' },
  { label: '5-6?', value: 'long' },
  { label: '??', value: 'any' }
]

const languageOptions = [
  { label: '??', value: 'zh' },
  { label: '??', value: 'en' },
  { label: '??', value: 'ja' },
  { label: '??', value: 'ko' },
  { label: '??', value: 'fr' },
  { label: '??', value: 'de' },
  { label: '????', value: 'es' },
  { label: '??', value: 'mixed' }
]

const sortOptions = [
  { label: '??', value: 'totalScore' },
  { label: '???', value: 'uniqueness' },
  { label: '???', value: 'memorability' },
  { label: '???', value: 'fit' },
  { label: '???', value: 'aesthetics' }
]

// ??
const generating = ref(false)
const regenerating = ref(false)
const results = ref<NameItem[]>([])
const traceId = ref<string | undefined>(undefined)
const sortBy = ref<SortBy>('totalScore')
const showFavorites = ref(false)
const favorites = ref<NameFavorite[]>([])
const favoritesLoading = ref(false)
const favoriteNames = ref<Set<string>>(new Set())

// ????
const loadingProgress = ref(0)
const loadingTip = ref('??????...')
const loadingTips = [
  '?????????..',
  '???? Prompt...',
  '???? AI ??...',
  'AI ?????...',
  '????????...',
  '????????..',
  '????...'
]
let loadingInterval: NodeJS.Timeout | null = null

// ??????
const canGenerate = computed(() => {
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

// ??????
const startLoadingAnimation = () => {
  loadingProgress.value = 0
  let tipIndex = 0
  loadingTip.value = loadingTips[tipIndex]
  
  // ????????????????????????
  loadingInterval = setInterval(() => {
    if (loadingProgress.value < 90) {
      loadingProgress.value += Math.random() * 5
      if (loadingProgress.value > 90) {
        loadingProgress.value = 90
      }
    }
    
    // ? 10% ?????
    if (tipIndex < loadingTips.length - 1 && loadingProgress.value > (tipIndex + 1) * 12) {
      tipIndex++
      loadingTip.value = loadingTips[tipIndex]
    }
  }, 1000)
}

// ??????
const stopLoadingAnimation = () => {
  if (loadingInterval) {
    clearInterval(loadingInterval)
    loadingInterval = null
  }
  loadingProgress.value = 100
  loadingTip.value = '??'
  setTimeout(() => {
    loadingProgress.value = 0
    loadingTip.value = '????...'
  }, 500)
}

// ??
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
        console.error('????????', error)
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
      // traceId ????
    }
  } finally {
    stopLoadingAnimation()
    regenerating.value = false
  }
}

const handleReset = () => {
  formData.value = {
    type: 'game',
    style: ['??'],
    gender: undefined,
    length: undefined,
    keywords: undefined,
    language: '??',
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
    // ?? traceId ????
    const favorite = favorites.value.find(f => f.name === item.name)
    if (favorite) {
      await handleRemoveFavorite(favorite.id)
    }
  } else {
    // ????
    const favoriteRequest: FavoriteCreateRequest = {
      name: item.name,
      type: formData.value.type!,
      style: formData.value.style!,
      language: formData.value.language || '??',
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

// ????????
watch(showFavorites, (newVal) => {
  if (newVal) {
    loadFavorites()
  }
})

onMounted(() => {
  // ???????????
  loadFavorites()
})

useHead({
  title: '???? - ????',
  meta: [
    { name: 'description', content: 'AI ????????????????????????' }
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

/* ???? */
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

/* ????*/
@media (max-width: 1024px) {
  .name-tool-content {
    grid-template-columns: 1fr;
  }
}
</style>


