<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold skill-tree-title">ćč˝ć çŽĄç</h1>
      <div class="flex gap-2">
        <n-button type="primary" @click="showAddCategoryDialog = true">
          ćˇťĺ ĺçąť
        </n-button>
        <n-button type="success" @click="showAddSkillDialog = true">
          ćˇťĺ ćč?        </n-button>
      </div>
    </div>

    <!-- ĺ č˝˝çść?-->
    <div v-if="loading" class="text-center py-12">
      <n-spin size="large" />
    </div>

    <!-- ćč˝ć ĺąç¤ş -->
    <div v-else class="space-y-6">
      <div
        v-for="category in skillTree"
        :key="category.id"
        class="skill-category-card"
      >
        <div class="flex items-center justify-between mb-4">
          <div class="flex items-center gap-3">
            <span class="text-2xl">{{ category.icon }}</span>
            <div>
              <h2 class="text-xl font-semibold category-name">{{ category.name }}</h2>
              <p class="text-sm category-count">{{ category.skills?.length || 0 }} ä¸Şćč</p>
            </div>
          </div>
          <div
            class="w-4 h-4 rounded-full"
            :style="{ backgroundColor: category.color || defaultCategoryColor }"
          ></div>
        </div>

        <!-- ćč˝ĺčĄ?-->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mt-4">
          <div
            v-for="skill in category.skills"
            :key="skill.id"
            class="skill-item-card"
          >
            <div class="flex items-start justify-between mb-2">
              <div class="flex items-center gap-2">
                <span v-if="skill.icon" class="text-lg">{{ skill.icon }}</span>
                <h3 class="font-semibold skill-name">{{ skill.name }}</h3>
              </div>
              <div class="flex gap-1">
                <button
                  @click="openRatingDialog(skill)"
                  class="skill-action-btn skill-action-btn-primary"
                  title="čŽ°ĺ˝čŻçş§"
                >
                  â­?                </button>
                <button
                  @click="openLearningLogDialog(skill)"
                  class="skill-action-btn skill-action-btn-success"
                  title="ćˇťĺ ĺ­Śäš ćĽĺż"
                >
                  đ
                </button>
                <button
                  @click="viewSkillDetail(skill.id)"
                  class="skill-action-btn skill-action-btn-default"
                  title="ćĽçčŻŚć"
                >
                  đď¸?                </button>
              </div>
            </div>
            <p v-if="skill.description" class="text-sm skill-description mb-2 line-clamp-2">
              {{ skill.description }}
            </p>
            <div class="flex items-center gap-2">
              <span class="text-xs skill-label">ĺ˝ĺčŻçş§ďź</span>
              <div class="flex items-center gap-1">
                <span class="text-lg font-bold" :class="getRatingColor(skill.currentRating)">
                  {{ skill.currentRating || 0 }}
                </span>
                <span class="text-xs skill-label">/ 10</span>
              </div>
              <div
                v-if="skill.currentRating > 0"
                class="flex-1 h-2 skill-rating-bar-bg rounded-full overflow-hidden"
              >
                <div
                  class="h-full rounded-full transition-all"
                  :class="getRatingBarColor(skill.currentRating)"
                  :style="{ width: (skill.currentRating / 10) * 100 + '%' }"
                ></div>
              </div>
            </div>
            <p v-if="skill.lastRatingDate" class="text-xs skill-last-update mt-1">
              ćĺć´ć°ďź{{ formatDate(skill.lastRatingDate) }}
            </p>
          </div>
        </div>
      </div>

      <!-- çŠşçść?-->
      <div v-if="skillTree.length === 0" class="text-center py-12 empty-state">
        <p class="empty-state-text">ćć ćč˝ć°ćŽďźčŻˇĺćˇťĺ ĺçąťĺćč</p>
      </div>
    </div>

    <!-- ćˇťĺ ĺçąťĺŻščŻćĄ?-->
    <div
      v-if="showAddCategoryDialog"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showAddCategoryDialog = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-var(--color-bg-light, white) mb-4">ćˇťĺ ćč˝ĺçą</h2>
        <form @submit.prevent="addCategory" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺçąťĺç§°</label>
            <input
              v-model="categoryForm.name"
              type="text"
              required
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">图标</label>
            <input
              v-model="categoryForm.icon"
              type="text"
              placeholder="📌"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">颜色</label>
            <input
              v-model="categoryForm.color"
              type="color"
              class="w-full h-10 border border-gray-300 dark:border-gray-600 rounded"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">排序</label>
            <input
              v-model.number="categoryForm.sortOrder"
              type="number"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div class="flex gap-2 justify-end">
            <button
              type="button"
              @click="showAddCategoryDialog = false"
              class="px-4 py-2 bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition"
            >
              ĺćś
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 text-var(--color-bg-light, white) rounded hover:bg-blue-700 transition"
            >
              ćˇťĺ 
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- ćˇťĺ ćč˝ĺŻščŻćĄ -->
    <div
      v-if="showAddSkillDialog"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showAddSkillDialog = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-var(--color-bg-light, white) mb-4">ćˇťĺ ćč</h2>
        <form @submit.prevent="addSkill" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ćč˝ĺç§</label>
            <input
              v-model="skillForm.name"
              type="text"
              required
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ćĺąĺçą</label>
            <select
              v-model.number="skillForm.categoryId"
              required
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            >
              <option value="">čŻˇéćŠĺçąť</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.icon }} {{ cat.name }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ćč˝ćčż</label>
            <textarea
              v-model="skillForm.description"
              rows="3"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            ></textarea>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"></label>
            <input
              v-model="skillForm.icon"
              type="text"
              placeholder="⚡"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div class="flex gap-2 justify-end">
            <button
              type="button"
              @click="showAddSkillDialog = false"
              class="px-4 py-2 bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition"
            >
              ĺćś
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-green-600 text-var(--color-bg-light, white) rounded hover:bg-green-700 transition"
            >
              ćˇťĺ 
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- čŻçş§ĺŻščŻćĄ?-->
    <div
      v-if="showRatingDialog"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showRatingDialog = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-var(--color-bg-light, white) mb-4">
          čŽ°ĺ˝ćč˝čŻçş?- {{ selectedSkill?.name }}
        </h2>
        <form @submit.prevent="submitRating" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              čŻçş§ (1-10ĺ?
            </label>
            <input
              v-model.number="ratingForm.rating"
              type="number"
              min="1"
              max="10"
              step="0.1"
              required
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺ¤ćł¨</label>
            <textarea
              v-model="ratingForm.notes"
              rows="3"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            ></textarea>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">čŽ°ĺ˝ćśé´</label>
            <input
              v-model="ratingForm.recordedAt"
              type="datetime-local"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div class="flex gap-2 justify-end">
            <button
              type="button"
              @click="showRatingDialog = false"
              class="px-4 py-2 bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition"
            >
              ĺćś
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 text-var(--color-bg-light, white) rounded hover:bg-blue-700 transition"
            >
              äżĺ­
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- ĺ­Śäš ćĽĺżĺŻščŻćĄ?-->
    <div
      v-if="showLearningLogDialog"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showLearningLogDialog = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-var(--color-bg-light, white) mb-4">
          ćˇťĺ ĺ­Śäš ćĽĺż - {{ selectedSkill?.name }}
        </h2>
        <form @submit.prevent="submitLearningLog" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺ­Śäš ĺĺŽšć é˘</label>
            <input
              v-model="learningLogForm.title"
              type="text"
              required
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺ­Śäš ĺĺŽščŻŚć</label>
            <textarea
              v-model="learningLogForm.content"
              rows="4"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            ></textarea>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺ­Śäš ćśéżďźĺéďź</label>
              <input
                v-model.number="learningLogForm.duration"
                type="number"
                class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">čľćşçąťĺ</label>
              <select
                v-model="learningLogForm.resourceType"
                class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
              >
                <option value="">čŻˇéćŠ</option>
                <option value="č§é˘">č§é˘</option>
                <option value="ććĄŁ">ććĄŁ</option>
                <option value="ĺŽčˇľ">ĺŽčˇľ</option>
                <option value="čŻžç¨">čŻžç¨</option>
              </select>
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">čľćşéžćĽ</label>
            <input
              v-model="learningLogForm.resourceUrl"
              type="url"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺ­Śäš ćśé´</label>
            <input
              v-model="learningLogForm.learnedAt"
              type="datetime-local"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div class="flex gap-2 justify-end">
            <button
              type="button"
              @click="showLearningLogDialog = false"
              class="px-4 py-2 bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition"
            >
              ĺćś
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-green-600 text-var(--color-bg-light, white) rounded hover:bg-green-700 transition"
            >
              äżĺ­
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { success } = useNotification()
const { handleError } = useErrorHandler()

// 获取默认颜色（从 CSS 变量）
const getDefaultCategoryColor = () => {
  if (process.client) {
    return getComputedStyle(document.documentElement).getPropertyValue('--color-text-muted').trim() || 'var(--color-text-sec)'
  }
  return 'var(--color-text-sec)'
}

const defaultCategoryColor = getDefaultCategoryColor()

const loading = ref(true)
const skillTree = ref<any[]>([])
const categories = ref<any[]>([])

const showAddCategoryDialog = ref(false)
const showAddSkillDialog = ref(false)
const showRatingDialog = ref(false)
const showLearningLogDialog = ref(false)
const selectedSkill = ref<any>(null)

const categoryForm = ref({
  name: '',
  icon: '',
  color: defaultCategoryColor,
  sortOrder: 0
})

const skillForm = ref({
  name: '',
  categoryId: null as number | null,
  description: '',
  icon: '',
  sortOrder: 0
})

const ratingForm = ref({
  rating: 5,
  notes: '',
  recordedAt: new Date().toISOString().slice(0, 16)
})

const learningLogForm = ref({
  title: '',
  content: '',
  duration: null as number | null,
  resourceType: '',
  resourceUrl: '',
  learnedAt: new Date().toISOString().slice(0, 16)
})

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const getRatingColor = (rating: number) => {
  if (rating >= 8) return 'rating-color-excellent'
  if (rating >= 6) return 'rating-color-good'
  if (rating >= 4) return 'rating-color-fair'
  return 'rating-color-poor'
}

const getRatingBarColor = (rating: number) => {
  if (rating >= 8) return 'rating-bar-excellent'
  if (rating >= 6) return 'rating-bar-good'
  if (rating >= 4) return 'rating-bar-fair'
  return 'rating-bar-poor'
}

const fetchSkillTree = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/SkillTree')
    skillTree.value = res || []
  } catch (e: unknown) {
    handleError(e, '添加技能失败')
  } finally {
    loading.value = false
  }
}

const fetchCategories = async () => {
  try {
    const res = await api.get<any[]>('/SkillTree/categories')
    categories.value = res || []
  } catch (e: unknown) {
    handleError(e, 'čˇĺĺçąťĺ¤ąč´Ľ')
  }
}

const addCategory = async () => {
  try {
    await api.post('/SkillTree/categories', categoryForm.value)
    success('添加成功')
    showAddCategoryDialog.value = false
    categoryForm.value = { name: '', icon: '', color: defaultCategoryColor, sortOrder: 0 }
    await fetchCategories()
    await fetchSkillTree()
  } catch (e: unknown) {
    handleError(e, 'ćˇťĺ ĺçąťĺ¤ąč´Ľ')
  }
}

const addSkill = async () => {
  try {
    await api.post('/SkillTree/skills', skillForm.value)
    success('添加成功')
    showAddSkillDialog.value = false
    skillForm.value = { name: '', categoryId: null, description: '', icon: '', sortOrder: 0 }
    await fetchSkillTree()
  } catch (e: unknown) {
    handleError(e, '添加技能失败')
  }
}

const openRatingDialog = (skill: any) => {
  selectedSkill.value = skill
  ratingForm.value = {
    rating: skill.currentRating || 5,
    notes: '',
    recordedAt: new Date().toISOString().slice(0, 16)
  }
  showRatingDialog.value = true
}

const submitRating = async () => {
  if (!selectedSkill.value) return

  try {
    await api.post(`/SkillTree/skills/${selectedSkill.value.id}/ratings`, {
      rating: ratingForm.value.rating,
      notes: ratingForm.value.notes,
      recordedAt: ratingForm.value.recordedAt
    })
    success('添加成功')
    showRatingDialog.value = false
    await fetchSkillTree()
  } catch (e: unknown) {
    handleError(e, 'čŽ°ĺ˝čŻçş§ĺ¤ąč´Ľ')
  }
}

const openLearningLogDialog = (skill: any) => {
  selectedSkill.value = skill
  learningLogForm.value = {
    title: '',
    content: '',
    duration: null,
    resourceType: '',
    resourceUrl: '',
    learnedAt: new Date().toISOString().slice(0, 16)
  }
  showLearningLogDialog.value = true
}

const submitLearningLog = async () => {
  if (!selectedSkill.value) return

  try {
    await api.post(`/SkillTree/skills/${selectedSkill.value.id}/learning-logs`, {
      title: learningLogForm.value.title,
      content: learningLogForm.value.content,
      duration: learningLogForm.value.duration,
      resourceType: learningLogForm.value.resourceType,
      resourceUrl: learningLogForm.value.resourceUrl,
      learnedAt: learningLogForm.value.learnedAt
    })
    success('添加成功')
    showLearningLogDialog.value = false
  } catch (e: unknown) {
    handleError(e, 'ćˇťĺ ĺ­Śäš ćĽĺżĺ¤ąč´Ľ')
  }
}

const viewSkillDetail = (skillId: number) => {
  navigateTo(`/admin/skill-tree/skills/${skillId}`)
}

onMounted(() => {
  fetchCategories()
  fetchSkillTree()
})
</script>

<style scoped>
/* ć é˘ć ˇĺź - ä˝żç¨ CSS ĺé */
.skill-tree-title {
  color: var(--color-text-main, var(--color-text-main));
}

/* ĺçąťĺĄçć ˇĺź - ä˝żç¨ CSS ĺé */
.skill-category-card {
  background: var(--color-bg-card, var(--color-bg-card));
  border: 1px solid var(--color-border-subtle, var(--color-border));
  border-radius: 0.5rem;
  padding: 1.5rem;
  box-shadow: var(--shadow-sm, 0 1px 2px 0 var(--color-border));
}

.category-name {
  color: var(--color-text-main, var(--color-text-main));
}

.category-count {
  color: var(--color-text-muted, var(--color-text-sec));
}

/* ćč˝éĄšĺĄçć ˇĺź - ä˝żç¨ CSS ĺé */
.skill-item-card {
  border: 1px solid var(--color-border-subtle, var(--color-border));
  border-radius: 0.5rem;
  padding: 1rem;
  transition: box-shadow 0.2s ease;
}

.skill-item-card:hover {
  box-shadow: var(--shadow-md, 0 4px 6px -1px var(--shadow));
}

.skill-name {
  color: var(--color-text-main, var(--color-text-main));
}

.skill-description {
  color: var(--color-text-sub, var(--color-gray-600));
}

.skill-label {
  color: var(--color-text-muted, var(--color-text-sec));
}

.skill-rating-bar-bg {
  background: var(--color-bg-elevated, var(--color-gray-100));
}

/* ćč˝ćä˝ćéŽć ˇĺź?- ä˝żç¨ CSS ĺé */
.skill-action-btn {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 0.875rem;
  transition: color 0.2s ease;
}

.skill-action-btn-primary {
  color: var(--color-primary, var(--color-primary));
}

.skill-action-btn-primary:hover {
  color: var(--color-primary-hover, var(--color-primary-hover));
}

.skill-action-btn-success {
  color: var(--color-success, var(--color-success));
}

.skill-action-btn-success:hover {
  color: var(--color-success-hover, var(--color-green-700));
}

.skill-action-btn-default {
  color: var(--color-text-muted, var(--color-text-sec));
}

.skill-action-btn-default:hover {
  color: var(--color-text-main, var(--color-text-main));
}

/* çŠşçśćć ˇĺź?- ä˝żç¨ CSS ĺé */
.empty-state {
  background: var(--color-bg-card, var(--color-bg-card));
  border-radius: 0.5rem;
}

.empty-state-text {
  color: var(--color-text-muted, var(--color-text-sec));
}

/* ćˇąč˛ä¸ťé˘éé */
html[data-theme="dark"] .skill-tree-title,
html.dark .skill-tree-title {
  color: var(--color-text-main, var(--color-bg-card));
}

html[data-theme="dark"] .skill-category-card,
html.dark .skill-category-card {
  background: var(--color-bg-card, rgba(255, 255, 255, 0.05));
  border-color: var(--color-border-subtle, rgba(255, 255, 255, 0.1));
}

html[data-theme="dark"] .category-name,
html.dark .category-name {
  color: var(--color-text-main, var(--color-bg-card));
}

html[data-theme="dark"] .category-count,
html.dark .category-count {
  color: var(--color-text-muted, var(--color-gray-400));
}

html[data-theme="dark"] .skill-item-card,
html.dark .skill-item-card {
  border-color: var(--color-border-subtle, rgba(255, 255, 255, 0.1));
}

html[data-theme="dark"] .skill-name,
html.dark .skill-name {
  color: var(--color-text-main, var(--color-bg-card));
}

html[data-theme="dark"] .skill-description,
html.dark .skill-description {
  color: var(--color-text-sub, var(--color-gray-400));
}

html[data-theme="dark"] .skill-rating-bar-bg,
html.dark .skill-rating-bar-bg {
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.05));
}

html[data-theme="dark"] .empty-state,
html.dark .empty-state {
  background: var(--color-bg-card, rgba(255, 255, 255, 0.05));
}

.skill-last-update {
  color: var(--color-text-muted, var(--color-gray-400));
}

/* čŻçş§é˘č˛ć ˇĺź - ä˝żç¨ CSS ĺé */
.rating-color-excellent {
  color: var(--color-success, var(--color-success));
}

.rating-color-good {
  color: var(--color-primary, var(--color-primary));
}

.rating-color-fair {
  color: var(--color-warning, var(--color-warning));
}

.rating-color-poor {
  color: var(--color-error, var(--color-danger));
}

/* čŻçş§čżĺşŚćĄé˘č˛ć ˇĺź?- ä˝żç¨ CSS ĺé */
.rating-bar-excellent {
  background: var(--color-success, var(--color-success));
}

.rating-bar-good {
  background: var(--color-primary, var(--color-primary));
}

.rating-bar-fair {
  background: var(--color-warning, var(--color-warning));
}

.rating-bar-poor {
  background: var(--color-error, var(--color-danger));
}
</style>

