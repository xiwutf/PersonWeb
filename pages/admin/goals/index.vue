<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-var(--color-bg-light, white)">ĺš´ĺşŚçŽć çŽĄç</h1>
      <button @click="showCreateModal = true" class="px-4 py-2 bg-blue-600 text-var(--color-bg-light, white) rounded hover:bg-blue-700 transition-colors">
        + ć°ĺťşçŽć 
      </button>
    </div>

    <!-- çťčŽĄĺĄç -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">ćťçŽć ?</div>
        <div class="text-2xl font-bold text-gray-800 dark:text-var(--color-bg-light, white)">{{ stats?.Total || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">čżčĄä¸?</div>
        <div class="text-2xl font-bold text-blue-600 dark:text-blue-400">{{ stats?.Active || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">ĺˇ˛ĺŽć?</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats?.Completed || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">ĺšłĺčżĺşŚ</div>
        <div class="text-2xl font-bold text-purple-600 dark:text-purple-400">{{ stats?.AverageProgress || 0 }}%</div>
      </div>
    </div>

    <!-- ç­éć  -->
    <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-6 flex gap-4 flex-wrap">
      <select v-model.number="filterYear" @change="fetchGoals" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
        <option :value="null">ĺ¨é¨ĺš´äť˝</option>
        <option v-for="y in years" :key="y" :value="y">{{ y }}ĺš?</option>
      </select>
      <select v-model="filterStatus" @change="fetchGoals" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
        <option value="">ĺ¨é¨çść?</option>
        <option value="active">čżčĄä¸?</option>
        <option value="completed">ĺˇ˛ĺŽć?</option>
        <option value="archived">ĺˇ˛ĺ˝ćĄ?</option>
      </select>
      <button @click="fetchGoals" class="px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">ĺˇć°</button>
    </div>

    <!-- çŽć ĺčĄ¨ -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <div v-if="loading" class="p-8 text-center text-gray-500 dark:text-gray-400">ĺ č˝˝ä¸?..</div>
      <div v-else-if="goals.length === 0" class="p-8 text-center text-gray-500 dark:text-gray-400">ćć çŽć </div>
      <div v-else class="divide-y divide-gray-200 dark:divide-gray-700">
        <div v-for="goal in goals" :key="goal.id" class="p-6 hover:bg-gray-50 dark:hover:bg-gray-700/30 transition-colors">
          <div class="flex items-start justify-between">
            <div class="flex-1">
              <div class="flex items-center gap-3 mb-2">
                <h3 class="text-lg font-semibold text-gray-800 dark:text-var(--color-bg-light, white)">{{ goal.title }}</h3>
                <span class="px-2 py-1 bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300 rounded text-xs font-medium">
                  {{ goal.year }}ĺš?                </span>
                <span :class="getStatusClass(goal.status)" class="px-2 py-1 rounded text-xs font-medium">
                  {{ getStatusText(goal.status) }}
                </span>
                <span v-if="goal.category" class="px-2 py-1 bg-gray-100 dark:bg-gray-700 rounded text-xs">
                  {{ goal.category }}
                </span>
              </div>
              <p v-if="goal.description" class="text-sm text-gray-600 dark:text-gray-400 mb-3">{{ goal.description }}</p>
              
              <!-- çŽć ć°ĺ?-->
              <div class="flex items-center gap-4 text-sm text-gray-500 dark:text-gray-400 mb-3">
                <span v-if="goal.targetValue">
                  çŽć : <span class="font-semibold text-gray-800 dark:text-var(--color-bg-light, white)">{{ goal.targetValue }}{{ goal.unit || '' }}</span>
                </span>
                <span>
                  ĺ˝ĺ: <span class="font-semibold text-gray-800 dark:text-var(--color-bg-light, white)">{{ goal.currentValue }}{{ goal.unit || '' }}</span>
                </span>
                <span>
                  čżĺşŚ: <span class="font-semibold text-gray-800 dark:text-var(--color-bg-light, white)">{{ goal.progress }}%</span>
                </span>
              </div>

              <!-- čżĺşŚć?-->
              <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-3 mb-3">
                <div 
                  :class="getProgressColor(goal.progress)"
                  class="h-3 rounded-full transition-all duration-300"
                  :style="{ width: goal.progress + '%' }"
                ></div>
              </div>

              <!-- ćĺşŚ KPI é˘č§ -->
              <div v-if="goal.monthlyKpis && goal.monthlyKpis.length > 0" class="mt-3">
                <div class="text-xs text-gray-500 dark:text-gray-400 mb-2">ćĺşŚ KPI ({{ goal.monthlyKpis.length }})</div>
                <div class="grid grid-cols-6 gap-2">
                  <div 
                    v-for="kpi in goal.monthlyKpis.slice(0, 6)" 
                    :key="kpi.id"
                    :class="getKpiStatusClass(kpi.status)"
                    class="px-2 py-1 rounded text-xs text-center"
                    :title="`${kpi.month}ć? ${kpi.progress}%`"
                  >
                    {{ kpi.month }}ć?                  </div>
                </div>
              </div>
            </div>
            <div class="flex gap-2 ml-4">
              <NuxtLink :to="`/admin/goals/${goal.id}/kpis`" class="px-3 py-1 text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">KPI</NuxtLink>
              <button @click="editGoal(goal)" class="px-3 py-1 text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">çźčž</button>
              <button @click="deleteGoal(goal.id)" class="px-3 py-1 text-sm text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300">ĺ é¤</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ĺĺťş/çźčžçŽć ć¨ĄććĄ -->
    <div v-if="showCreateModal || editingGoal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" @click.self="closeModal">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h2 class="text-xl font-bold text-gray-800 dark:text-var(--color-bg-light, white) mb-4">
            {{ editingGoal ? 'çźčžçŽć ' : 'ć°ĺťşĺš´ĺşŚçŽć ' }}
          </h2>
          <form @submit.prevent="saveGoal" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">çŽć ĺš´äť˝ *</label>
                <input v-model.number="goalForm.year" type="number" required min="2020" max="2100" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺçąť</label>
                <input v-model="goalForm.category" type="text" placeholder="ĺˇĽä˝/ĺ­Śäš /çć´ťç­?" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">çŽć ć é˘ *</label>
              <input v-model="goalForm.title" type="text" required class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">çŽć ćčż°</label>
              <textarea v-model="goalForm.description" rows="3" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"></textarea>
            </div>
            <div class="grid grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">çŽć ć°ĺ?</label>
                <input v-model.number="goalForm.targetValue" type="number" step="0.01" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺ˝ĺć°ĺ?</label>
                <input v-model.number="goalForm.currentValue" type="number" step="0.01" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺä˝</label>
                <input v-model="goalForm.unit" type="text" placeholder="ä¸?çŻ?ĺ°ćśç­?" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">ĺźĺ§ćĽć?</label>
                <input v-model="goalForm.startDate" type="date" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">çťććĽć</label>
                <input v-model="goalForm.endDate" type="date" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">çść?</label>
              <select v-model="goalForm.status" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
                <option value="active">čżčĄä¸?</option>
                <option value="completed">ĺˇ˛ĺŽć?</option>
                <option value="archived">ĺˇ˛ĺ˝ćĄ?</option>
              </select>
            </div>
            <div class="flex justify-end gap-3 pt-4">
              <button 
                type="button" 
                @click="closeModal" 
                class="px-4 py-2 bg-gray-200 dark:bg-gray-700 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors font-bold border-2 border-gray-400 dark:border-gray-500 shadow-sm text-gray-900 dark:text-gray-100"
              >
                <span class="dark:text-gray-100" style="color: inherit;">ĺćś</span>
              </button>
              <button type="submit" class="px-4 py-2 bg-blue-600 text-var(--color-bg-light, white) rounded hover:bg-blue-700 transition-colors font-medium shadow-md">äżĺ­</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Goal {
  id: number
  year: number
  title: string
  description?: string
  category?: string
  targetValue?: number
  currentValue: number
  unit?: string
  status: string
  progress: number
  startDate?: string
  endDate?: string
  monthlyKpis?: any[]
}

interface GoalStats {
  Total: number
  Active: number
  Completed: number
  Archived: number
  AverageProgress: number
}

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const toast = useNotification()

const goals = ref<Goal[]>([])
const stats = ref<GoalStats | null>(null)
const loading = ref(false)
const filterYear = ref<number | null>(null)
const filterStatus = ref('')
const showCreateModal = ref(false)
const editingGoal = ref<Goal | null>(null)

const years = computed(() => {
  const currentYear = new Date().getFullYear()
  return Array.from({ length: 5 }, (_, i) => currentYear - i)
})

const goalForm = ref({
  year: new Date().getFullYear(),
  title: '',
  description: '',
  category: '',
  targetValue: null as number | null,
  currentValue: 0,
  unit: '',
  status: 'active',
  startDate: '',
  endDate: ''
})

const fetchGoals = async () => {
  loading.value = true
  try {
    const params: any = {}
    if (filterYear.value) params.year = filterYear.value
    if (filterStatus.value) params.status = filterStatus.value

    const res = await api.get('/Goals', { params })
    if (res?.data?.Items) {
      goals.value = res.data.Items
    }
  } catch (error) {
    console.error('čˇĺçŽć ĺčĄ¨ĺ¤ąč´Ľ:', error)
    toast.error('čˇĺçŽć ĺčĄ¨ĺ¤ąč´Ľ')
  } finally {
    loading.value = false
  }
}

const fetchStats = async () => {
  try {
    const params: any = {}
    if (filterYear.value) params.year = filterYear.value
    const res = await api.get('/Goals/stats', { params })
    if (res?.data) {
      stats.value = res.data
    }
  } catch (error) {
    console.error('čˇĺçťčŽĄĺ¤ąč´Ľ:', error)
  }
}

const saveGoal = async () => {
  try {
    const data = {
      ...goalForm.value,
      startDate: goalForm.value.startDate || null,
      endDate: goalForm.value.endDate || null
    }

    if (editingGoal.value) {
      await api.put(`/Goals/${editingGoal.value.id}`, data)
      toast.success('çŽć ć´ć°ćĺ')
    } else {
      await api.post('/Goals', data)
      toast.success('çŽć ĺĺťşćĺ')
    }

    closeModal()
    fetchGoals()
    fetchStats()
  } catch (error) {
    console.error('äżĺ­çŽć ĺ¤ąč´Ľ:', error)
    toast.error('äżĺ­çŽć ĺ¤ąč´Ľ')
  }
}

const editGoal = (goal: Goal) => {
  editingGoal.value = goal
  goalForm.value = {
    year: goal.year,
    title: goal.title,
    description: goal.description || '',
    category: goal.category || '',
    targetValue: goal.targetValue || null,
    currentValue: goal.currentValue,
    unit: goal.unit || '',
    status: goal.status,
    startDate: goal.startDate ? goal.startDate.split('T')[0] : '',
    endDate: goal.endDate ? goal.endDate.split('T')[0] : ''
  }
}

const deleteGoal = async (id: number) => {
  if (!confirm('确定要删除这个目标吗？删除后关联的月度KPI也会被删除。')) return

  try {
    await api.delete(`/Goals/${id}`)
    toast.success('çŽć ĺ é¤ćĺ')
    fetchGoals()
    fetchStats()
  } catch (error) {
    console.error('ĺ é¤çŽć ĺ¤ąč´Ľ:', error)
    toast.error('ĺ é¤çŽć ĺ¤ąč´Ľ')
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingGoal.value = null
  goalForm.value = {
    year: new Date().getFullYear(),
    title: '',
    description: '',
    category: '',
    targetValue: null,
    currentValue: 0,
    unit: '',
    status: 'active',
    startDate: '',
    endDate: ''
  }
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    active: 'bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300',
    completed: 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300',
    archived: 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300'
  }
  return classes[status] || classes.active
}

const getStatusText = (status: string) => {
  const texts: Record<string, string> = {
    active: 'čżčĄä¸?',
    completed: 'ĺˇ˛ĺŽć?',
    archived: 'ĺˇ˛ĺ˝ćĄ?'
  }
  return texts[status] || status
}

const getProgressColor = (progress: number) => {
  if (progress === 100) return 'bg-green-500'
  if (progress >= 50) return 'bg-blue-500'
  if (progress >= 25) return 'bg-yellow-500'
  return 'bg-gray-400'
}

const getKpiStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    pending: 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400',
    in_progress: 'bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300',
    completed: 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300'
  }
  return classes[status] || classes.pending
}

onMounted(() => {
  fetchGoals()
  fetchStats()
})
</script>

