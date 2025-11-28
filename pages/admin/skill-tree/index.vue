<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">技能树管理</h1>
      <div class="flex gap-2">
        <button
          @click="showAddCategoryDialog = true"
          class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
        >
          添加分类
        </button>
        <button
          @click="showAddSkillDialog = true"
          class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition"
        >
          添加技能
        </button>
      </div>
    </div>

    <!-- 加载状态 -->
    <div v-if="loading" class="text-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <!-- 技能树展示 -->
    <div v-else class="space-y-6">
      <div
        v-for="category in skillTree"
        :key="category.id"
        class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6"
      >
        <div class="flex items-center justify-between mb-4">
          <div class="flex items-center gap-3">
            <span class="text-2xl">{{ category.icon }}</span>
            <div>
              <h2 class="text-xl font-semibold text-gray-900 dark:text-white">{{ category.name }}</h2>
              <p class="text-sm text-gray-500 dark:text-gray-400">{{ category.skills?.length || 0 }} 个技能</p>
            </div>
          </div>
          <div
            class="w-4 h-4 rounded-full"
            :style="{ backgroundColor: category.color || '#6b7280' }"
          ></div>
        </div>

        <!-- 技能列表 -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mt-4">
          <div
            v-for="skill in category.skills"
            :key="skill.id"
            class="border border-gray-200 dark:border-gray-700 rounded-lg p-4 hover:shadow-md transition-shadow"
          >
            <div class="flex items-start justify-between mb-2">
              <div class="flex items-center gap-2">
                <span v-if="skill.icon" class="text-lg">{{ skill.icon }}</span>
                <h3 class="font-semibold text-gray-900 dark:text-white">{{ skill.name }}</h3>
              </div>
              <div class="flex gap-1">
                <button
                  @click="openRatingDialog(skill)"
                  class="text-blue-600 hover:text-blue-800 text-sm"
                  title="记录评级"
                >
                  ⭐
                </button>
                <button
                  @click="openLearningLogDialog(skill)"
                  class="text-green-600 hover:text-green-800 text-sm"
                  title="添加学习日志"
                >
                  📚
                </button>
                <button
                  @click="viewSkillDetail(skill.id)"
                  class="text-gray-600 hover:text-gray-800 text-sm"
                  title="查看详情"
                >
                  👁️
                </button>
              </div>
            </div>
            <p v-if="skill.description" class="text-sm text-gray-600 dark:text-gray-400 mb-2 line-clamp-2">
              {{ skill.description }}
            </p>
            <div class="flex items-center gap-2">
              <span class="text-xs text-gray-500">当前评级：</span>
              <div class="flex items-center gap-1">
                <span class="text-lg font-bold" :class="getRatingColor(skill.currentRating)">
                  {{ skill.currentRating || 0 }}
                </span>
                <span class="text-xs text-gray-500">/ 10</span>
              </div>
              <div
                v-if="skill.currentRating > 0"
                class="flex-1 h-2 bg-gray-200 dark:bg-gray-700 rounded-full overflow-hidden"
              >
                <div
                  class="h-full rounded-full transition-all"
                  :class="getRatingBarColor(skill.currentRating)"
                  :style="{ width: (skill.currentRating / 10) * 100 + '%' }"
                ></div>
              </div>
            </div>
            <p v-if="skill.lastRatingDate" class="text-xs text-gray-400 mt-1">
              最后更新：{{ formatDate(skill.lastRatingDate) }}
            </p>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-if="skillTree.length === 0" class="text-center py-12 bg-white dark:bg-gray-800 rounded-lg">
        <p class="text-gray-500 dark:text-gray-400">暂无技能数据，请先添加分类和技能</p>
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
  color: '#3b82f6',
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
  if (rating >= 8) return 'text-green-600 dark:text-green-400'
  if (rating >= 6) return 'text-blue-600 dark:text-blue-400'
  if (rating >= 4) return 'text-yellow-600 dark:text-yellow-400'
  return 'text-red-600 dark:text-red-400'
}

const getRatingBarColor = (rating: number) => {
  if (rating >= 8) return 'bg-green-500'
  if (rating >= 6) return 'bg-blue-500'
  if (rating >= 4) return 'bg-yellow-500'
  return 'bg-red-500'
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
    categoryForm.value = { name: '', icon: '', color: '#3b82f6', sortOrder: 0 }
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

