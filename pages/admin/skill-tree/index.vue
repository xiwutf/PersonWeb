<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold skill-tree-title">技能树管理</h1>
      <div class="flex gap-2">
        <n-button type="primary" @click="showAddCategoryDialog = true">
          添加分类
        </n-button>
        <n-button type="success" @click="showAddSkillDialog = true">
          添加技能
        </n-button>
      </div>
    </div>

    <!-- 加载状态 -->
    <div v-if="loading" class="text-center py-12">
      <n-spin size="large" />
    </div>

    <!-- 技能树展示 -->
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
              <p class="text-sm category-count">{{ category.skills?.length || 0 }} 个技能</p>
            </div>
          </div>
          <div
            class="w-4 h-4 rounded-full"
            :style="{ backgroundColor: category.color || defaultCategoryColor }"
          ></div>
        </div>

        <!-- 技能列表 -->
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
                  title="记录评级"
                >
                  ⭐
                </button>
                <button
                  @click="openLearningLogDialog(skill)"
                  class="skill-action-btn skill-action-btn-success"
                  title="添加学习日志"
                >
                  📚
                </button>
                <button
                  @click="viewSkillDetail(skill.id)"
                  class="skill-action-btn skill-action-btn-default"
                  title="查看详情"
                >
                  👁️
                </button>
              </div>
            </div>
            <p v-if="skill.description" class="text-sm skill-description mb-2 line-clamp-2">
              {{ skill.description }}
            </p>
            <div class="flex items-center gap-2">
              <span class="text-xs skill-label">当前评级：</span>
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
              最后更新：{{ formatDate(skill.lastRatingDate) }}
            </p>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-if="skillTree.length === 0" class="text-center py-12 empty-state">
        <p class="empty-state-text">暂无技能数据，请先添加分类和技能</p>
      </div>
    </div>

    <!-- 添加分类对话框 -->
    <div
      v-if="showAddCategoryDialog"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showAddCategoryDialog = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">添加技能分类</h2>
        <form @submit.prevent="addCategory" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">分类名称</label>
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
              placeholder="💻"
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
              取消
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
            >
              添加
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- 添加技能对话框 -->
    <div
      v-if="showAddSkillDialog"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showAddSkillDialog = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">添加技能</h2>
        <form @submit.prevent="addSkill" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">技能名称</label>
            <input
              v-model="skillForm.name"
              type="text"
              required
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">所属分类</label>
            <select
              v-model.number="skillForm.categoryId"
              required
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            >
              <option value="">请选择分类</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.icon }} {{ cat.name }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">技能描述</label>
            <textarea
              v-model="skillForm.description"
              rows="3"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            ></textarea>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">图标</label>
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
              取消
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition"
            >
              添加
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- 评级对话框 -->
    <div
      v-if="showRatingDialog"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showRatingDialog = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">
          记录技能评级 - {{ selectedSkill?.name }}
        </h2>
        <form @submit.prevent="submitRating" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              评级 (1-10分)
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
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">备注</label>
            <textarea
              v-model="ratingForm.notes"
              rows="3"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            ></textarea>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">记录时间</label>
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
              取消
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
            >
              保存
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- 学习日志对话框 -->
    <div
      v-if="showLearningLogDialog"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showLearningLogDialog = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">
          添加学习日志 - {{ selectedSkill?.name }}
        </h2>
        <form @submit.prevent="submitLearningLog" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">学习内容标题</label>
            <input
              v-model="learningLogForm.title"
              type="text"
              required
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">学习内容详情</label>
            <textarea
              v-model="learningLogForm.content"
              rows="4"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            ></textarea>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">学习时长（分钟）</label>
              <input
                v-model.number="learningLogForm.duration"
                type="number"
                class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">资源类型</label>
              <select
                v-model="learningLogForm.resourceType"
                class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
              >
                <option value="">请选择</option>
                <option value="视频">视频</option>
                <option value="文档">文档</option>
                <option value="实践">实践</option>
                <option value="课程">课程</option>
              </select>
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">资源链接</label>
            <input
              v-model="learningLogForm.resourceUrl"
              type="url"
              class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">学习时间</label>
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
              取消
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition"
            >
              保存
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
    handleError(e, '获取技能树失败')
  } finally {
    loading.value = false
  }
}

const fetchCategories = async () => {
  try {
    const res = await api.get<any[]>('/SkillTree/categories')
    categories.value = res || []
  } catch (e: unknown) {
    handleError(e, '获取分类失败')
  }
}

const addCategory = async () => {
  try {
    await api.post('/SkillTree/categories', categoryForm.value)
    success('分类添加成功')
    showAddCategoryDialog.value = false
    categoryForm.value = { name: '', icon: '', color: defaultCategoryColor, sortOrder: 0 }
    await fetchCategories()
    await fetchSkillTree()
  } catch (e: unknown) {
    handleError(e, '添加分类失败')
  }
}

const addSkill = async () => {
  try {
    await api.post('/SkillTree/skills', skillForm.value)
    success('技能添加成功')
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
    success('评级记录成功')
    showRatingDialog.value = false
    await fetchSkillTree()
  } catch (e: unknown) {
    handleError(e, '记录评级失败')
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
    success('学习日志添加成功')
    showLearningLogDialog.value = false
  } catch (e: unknown) {
    handleError(e, '添加学习日志失败')
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
/* 标题样式 - 使用 CSS 变量 */
.skill-tree-title {
  color: var(--color-text-main, var(--color-text-main));
}

/* 分类卡片样式 - 使用 CSS 变量 */
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

/* 技能项卡片样式 - 使用 CSS 变量 */
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
  color: var(--color-text-sub, #4b5563);
}

.skill-label {
  color: var(--color-text-muted, var(--color-text-sec));
}

.skill-rating-bar-bg {
  background: var(--color-bg-elevated, #f3f4f6);
}

/* 技能操作按钮样式 - 使用 CSS 变量 */
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
  color: var(--color-success, #10b981);
}

.skill-action-btn-success:hover {
  color: var(--color-success-hover, #059669);
}

.skill-action-btn-default {
  color: var(--color-text-muted, var(--color-text-sec));
}

.skill-action-btn-default:hover {
  color: var(--color-text-main, var(--color-text-main));
}

/* 空状态样式 - 使用 CSS 变量 */
.empty-state {
  background: var(--color-bg-card, var(--color-bg-card));
  border-radius: 0.5rem;
}

.empty-state-text {
  color: var(--color-text-muted, var(--color-text-sec));
}

/* 深色主题适配 */
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
  color: var(--color-text-muted, #9ca3af);
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
  color: var(--color-text-sub, #9ca3af);
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
  color: var(--color-text-muted, #9ca3af);
}

/* 评级颜色样式 - 使用 CSS 变量 */
.rating-color-excellent {
  color: var(--color-success, #10b981);
}

.rating-color-good {
  color: var(--color-primary, var(--color-primary));
}

.rating-color-fair {
  color: var(--color-warning, #f59e0b);
}

.rating-color-poor {
  color: var(--color-error, #ef4444);
}

/* 评级进度条颜色样式 - 使用 CSS 变量 */
.rating-bar-excellent {
  background: var(--color-success, #10b981);
}

.rating-bar-good {
  background: var(--color-primary, var(--color-primary));
}

.rating-bar-fair {
  background: var(--color-warning, #f59e0b);
}

.rating-bar-poor {
  background: var(--color-error, #ef4444);
}
</style>

