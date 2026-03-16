<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 to-var(--color-bg-light, white) dark:from-gray-900 dark:to-gray-800 py-12">
    <div class="container mx-auto px-4 max-w-7xl">
      <!-- ???? -->
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold text-gray-900 dark:text-var(--color-bg-light, white) mb-4">???</h1>
        <p class="text-xl text-gray-600 dark:text-gray-400">????????</p>
      </div>

      <!-- ?????-->
      <div v-if="loading" class="text-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <p class="text-gray-600 dark:text-gray-400">????..</p>
      </div>

      <!-- ????? -->
      <div v-else class="space-y-8">
        <!-- ????-->
        <div class="card p-8">
          <h2 class="text-2xl font-bold text-gray-900 dark:text-var(--color-bg-light, white) mb-6">?????</h2>
          <div class="flex gap-4 mb-4">
            <select
              v-model="selectedCategoryId"
              @change="updateRadarData"
              class="form-select"
            >
              <option :value="null">????</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.icon }} {{ cat.name }}
              </option>
            </select>
            <select
              v-model="timeRange"
              @change="updateRadarData"
              class="form-select"
            >
              <option value="current">????</option>
              <option value="3months">3??</option>
              <option value="6months">6??</option>
              <option value="1year">1?</option>
            </select>
          </div>
          <div v-if="radarData.labels.length > 0" class="h-96">
            <Radar :data="radarData" :options="radarOptions" />
          </div>
          <div v-else class="h-96 flex items-center justify-center text-gray-500 dark:text-gray-400">
            ????
          </div>
        </div>

        <!-- ???????-->
        <div
          v-for="category in skillTree"
          :key="category.id"
          class="card overflow-hidden skill-tree-card"
        >
          <div
            class="p-6 border-b border-gray-200 dark:border-gray-700"
            :style="{ borderLeftColor: category.color, borderLeftWidth: '4px' }"
          >
            <div class="flex items-center gap-3">
              <span class="text-3xl">{{ category.icon }}</span>
              <div>
                <h2 class="text-2xl font-bold text-gray-900 dark:text-var(--color-bg-light, white)">{{ category.name }}</h2>
                <p class="text-sm text-gray-500 dark:text-gray-400">
                  {{ category.skills?.length || 0 }} ????                </p>
              </div>
            </div>
          </div>

          <div class="p-6">
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              <div
                v-for="skill in category.skills"
                :key="skill.id"
                class="border border-gray-200 dark:border-gray-700 rounded-xl p-6 hover:shadow-lg transition-shadow"
              >
                <div class="flex items-start justify-between mb-4">
                  <div class="flex items-center gap-2">
                    <span v-if="skill.icon" class="text-2xl">{{ skill.icon }}</span>
                    <h3 class="text-lg font-semibold text-gray-900 dark:text-var(--color-bg-light, white)">{{ skill.name }}</h3>
                  </div>
                </div>
                <p v-if="skill.description" class="text-sm text-gray-600 dark:text-gray-400 mb-4 line-clamp-2">
                  {{ skill.description }}
                </p>
                <div class="space-y-2">
                  <div class="flex items-center justify-between">
                    <span class="text-sm text-gray-500 dark:text-gray-400">????</span>
                    <span class="text-xl font-bold" :class="getRatingColor(skill.currentRating)">
                      {{ skill.currentRating || 0 }} / 10
                    </span>
                  </div>
                  <div
                    v-if="skill.currentRating > 0"
                    class="w-full h-3 bg-gray-200 dark:bg-gray-700 rounded-full overflow-hidden"
                  >
                    <div
                      class="h-full rounded-full transition-all"
                      :class="getRatingBarColor(skill.currentRating)"
                      :style="{ width: (skill.currentRating / 10) * 100 + '%' }"
                    ></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// ???????????????
definePageMeta({
  layout: 'default'
})

import {
  Chart as ChartJS,
  RadialLinearScale,
  PointElement,
  LineElement,
  Filler,
  Tooltip,
  Legend
} from 'chart.js'
import { Radar } from 'vue-chartjs'

ChartJS.register(
  RadialLinearScale,
  PointElement,
  LineElement,
  Filler,
  Tooltip,
  Legend
)

const api = useApi()
const loading = ref(true)
const skillTree = ref<any[]>([])
const categories = ref<any[]>([])
const selectedCategoryId = ref<number | null>(null)
const timeRange = ref('current')
const radarData = ref({
  labels: [],
  datasets: []
})

const getTimeRange = () => {
  const now = new Date()
  switch (timeRange.value) {
    case '3months':
      return { start: new Date(now.getFullYear(), now.getMonth() - 3, now.getDate()), end: now }
    case '6months':
      return { start: new Date(now.getFullYear(), now.getMonth() - 6, now.getDate()), end: now }
    case '1year':
      return { start: new Date(now.getFullYear() - 1, now.getMonth(), now.getDate()), end: now }
    default:
      return null
  }
}

const updateRadarData = async () => {
  try {
    const timeRangeData = getTimeRange()
    const params: any = {}
    
    if (selectedCategoryId.value) {
      params.categoryId = selectedCategoryId.value
    }
    
    if (timeRangeData) {
      params.startDate = timeRangeData.start.toISOString()
      params.endDate = timeRangeData.end.toISOString()
    }

    const res = await api.get<any>('/SkillTree/radar', { params })
    
    if (res && res.length > 0) {
      radarData.value = {
        labels: res.map((item: any) => item.skillName || item.SkillName),
        datasets: [{
          label: '????',
          data: res.map((item: any) => 
            parseFloat(item.averageRating || item.AverageRating || item.rating || item.Rating || 0)
          ),
          backgroundColor: 'rgba(59, 130, 246, 0.2)',
          borderColor: 'rgb(59, 130, 246)',
          borderWidth: 2,
          pointBackgroundColor: 'rgb(59, 130, 246)',
          pointBorderColor: 'var(--color-bg-light, white)',
          pointHoverBackgroundColor: 'var(--color-bg-light, white)',
          pointHoverBorderColor: 'rgb(59, 130, 246)'
        }]
      }
    } else if (timeRange.value === 'current') {
      // ????????????????      const allSkills: any[] = []
      skillTree.value.forEach(cat => {
        if (!selectedCategoryId.value || cat.id === selectedCategoryId.value) {
          cat.skills?.forEach((skill: any) => {
            if (skill.currentRating > 0) {
              allSkills.push({
                skillName: skill.name,
                rating: skill.currentRating
              })
            }
          })
        }
      })
      
      radarData.value = {
        labels: allSkills.map(s => s.skillName),
        datasets: [{
          label: '????',
          data: allSkills.map(s => parseFloat(s.rating)),
          backgroundColor: 'rgba(59, 130, 246, 0.2)',
          borderColor: 'rgb(59, 130, 246)',
          borderWidth: 2,
          pointBackgroundColor: 'rgb(59, 130, 246)',
          pointBorderColor: 'var(--color-bg-light, white)',
          pointHoverBackgroundColor: 'var(--color-bg-light, white)',
          pointHoverBorderColor: 'rgb(59, 130, 246)'
        }]
      }
    } else {
      radarData.value = { labels: [], datasets: [] }
    }
  } catch (e) {
    console.error('Failed to load radar data:', e)
    radarData.value = { labels: [], datasets: [] }
  }
}

const radarOptions = {
  responsive: true,
  maintainAspectRatio: false,
  scales: {
    r: {
      beginAtZero: true,
      max: 10,
      ticks: {
        stepSize: 2
      }
    }
  },
  plugins: {
    legend: {
      display: true,
      position: 'top' as const
    }
  }
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

// ????????????????// const mockSkillTree = [
//   {
//     id: 1,
//     name: '?????,
//     icon: '??',
//     color: 'var(--color-primary)',
//     skills: [
//       {
//         id: 1,
//         name: 'Vue.js',
//         icon: '??,
//         description: '???JavaScript????????????,
//         currentRating: 8.5
//       },
//       {
//         id: 2,
//         name: 'Nuxt.js',
//         icon: '??',
//         description: '??Vue.js???????',
//         currentRating: 8.0
//       },
//       {
//         id: 3,
//         name: 'TypeScript',
//         icon: '??',
//         description: 'JavaScript?????????????,
//         currentRating: 7.5
//       },
//       {
//         id: 4,
//         name: 'Tailwind CSS',
//         icon: '??',
//         description: '?????CSS??',
//         currentRating: 7.0
//       }
//     ]
//   },
//   {
//     id: 2,
//     name: '?????,
//     icon: '??',
//     color: 'var(--color-success)',
//     skills: [
//       {
//         id: 5,
//         name: '.NET Core',
//         icon: '??',
//         description: '???????????,
//         currentRating: 8.0
//       },
//       {
//         id: 6,
//         name: 'Node.js',
//         icon: '??',
//         description: '??Chrome V8???JavaScript????,
//         currentRating: 7.5
//       },
//       {
//         id: 7,
//         name: 'Entity Framework',
//         icon: '????,
//         description: 'Microsoft?ORM??',
//         currentRating: 7.5
//       },
//       {
//         id: 8,
//         name: 'RESTful API',
//         icon: '??',
//         description: '?????RESTful???API',
//         currentRating: 8.5
//       }
//     ]
//   },
//   {
//     id: 3,
//     name: '????,
//     icon: '??',
//     color: 'var(--color-warning)',
//     skills: [
//       {
//         id: 9,
//         name: 'MySQL',
//         icon: '????,
//         description: '?????????????,
//         currentRating: 7.5
//       },
//       {
//         id: 10,
//         name: 'PostgreSQL',
//         icon: '??',
//         description: '??????????????',
//         currentRating: 6.5
//       },
//       {
//         id: 11,
//         name: 'Redis',
//         icon: '??',
//         description: '??????????',
//         currentRating: 6.0
//       }
//     ]
//   },
//   {
//     id: 4,
//     name: 'AI & ????',
//     icon: '??',
//     color: 'var(--color-purple-500)',
//     skills: [
//       {
//         id: 12,
//         name: 'LangChain',
//         icon: '??',
//         description: '??LLM??????,
//         currentRating: 7.0
//       },
//       {
//         id: 13,
//         name: 'OpenAI API',
//         icon: '??',
//         description: 'OpenAI?API??????,
//         currentRating: 7.5
//       },
//       {
//         id: 14,
//         name: 'Python',
//         icon: '??',
//         description: '???????AI????',
//         currentRating: 7.0
//       }
//     ]
//   }
// ]

// ??????????????????// const mockCategories = [
//   { id: 1, name: '?????, icon: '??' },
//   { id: 2, name: '?????, icon: '??' },
//   { id: 3, name: '????, icon: '??' },
//   { id: 4, name: 'AI & ????', icon: '??' }
// ]

const fetchSkillTree = async () => {
  loading.value = true
  try {
    // ?????MockData API??
    const res = await api.get<any>('/MockData/skill-tree')
    if (res && Array.isArray(res) && res.length > 0) {
      skillTree.value = res
      return
    }
    
    // ???API?????????API??
    const oldRes = await api.get<any>('/SkillTree')
    skillTree.value = oldRes && Array.isArray(oldRes) && oldRes.length > 0 ? oldRes : []
  } catch (e) {
    console.error('Failed to fetch skill tree:', e)
    skillTree.value = []
  } finally {
    loading.value = false
  }
}

const fetchCategories = async () => {
  try {
    // ?????MockData API??
    const res = await api.get<any[]>('/MockData/skill-categories')
    if (res && Array.isArray(res) && res.length > 0) {
      categories.value = res
      return
    }
    
    // ???API?????????API??
    const oldRes = await api.get<any[]>('/SkillTree/categories')
    categories.value = oldRes && Array.isArray(oldRes) && oldRes.length > 0 ? oldRes : []
  } catch (e) {
    console.error('Failed to fetch categories:', e)
    categories.value = []
  }
}

onMounted(async () => {
  await fetchCategories()
  await fetchSkillTree()
  await updateRadarData()
})

// ??????
useHead({
  title: '??? - ????',
  meta: [
    { name: 'description', content: '????????????' }
  ]
})
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* ??????? */
.skill-tree-card {
  transition: all 0.3s ease;
}

:global(.skill-tree-glowing) .skill-tree-card {
  animation: skill-tree-glow 3s ease-in-out;
  box-shadow: 0 0 30px rgba(59, 130, 246, 0.6);
  border-color: var(--theme-primary);
}

@keyframes skill-tree-glow {
  0%, 100% {
    box-shadow: 0 0 0 rgba(59, 130, 246, 0);
    border-color: transparent;
  }
  50% {
    box-shadow: 0 0 40px rgba(59, 130, 246, 0.8);
    border-color: rgba(59, 130, 246, 0.8);
  }
}
</style>

