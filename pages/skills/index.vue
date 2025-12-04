<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 to-white dark:from-gray-900 dark:to-gray-800 py-12">
    <div class="container mx-auto px-4 max-w-7xl">
      <!-- 页面头部 -->
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold text-gray-900 dark:text-white mb-4">技能树</h1>
        <p class="text-xl text-gray-600 dark:text-gray-400">我的技术能力展示</p>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="text-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <p class="text-gray-600 dark:text-gray-400">加载中...</p>
      </div>

      <!-- 技能树内容 -->
      <div v-else class="space-y-8">
        <!-- 雷达图 -->
        <div class="card p-8">
          <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">技能雷达图</h2>
          <div class="flex gap-4 mb-4">
            <select
              v-model="selectedCategoryId"
              @change="updateRadarData"
              class="form-select"
            >
              <option :value="null">全部分类</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.icon }} {{ cat.name }}
              </option>
            </select>
            <select
              v-model="timeRange"
              @change="updateRadarData"
              class="form-select"
            >
              <option value="current">当前评级</option>
              <option value="3months">3个月</option>
              <option value="6months">6个月</option>
              <option value="1year">1年</option>
            </select>
          </div>
          <div v-if="radarData.labels.length > 0" class="h-96">
            <Radar :data="radarData" :options="radarOptions" />
          </div>
          <div v-else class="h-96 flex items-center justify-center text-gray-500 dark:text-gray-400">
            暂无数据
          </div>
        </div>

        <!-- 技能分类展示 -->
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
                <h2 class="text-2xl font-bold text-gray-900 dark:text-white">{{ category.name }}</h2>
                <p class="text-sm text-gray-500 dark:text-gray-400">
                  {{ category.skills?.length || 0 }} 个技能
                </p>
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
                    <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ skill.name }}</h3>
                  </div>
                </div>
                <p v-if="skill.description" class="text-sm text-gray-600 dark:text-gray-400 mb-4 line-clamp-2">
                  {{ skill.description }}
                </p>
                <div class="space-y-2">
                  <div class="flex items-center justify-between">
                    <span class="text-sm text-gray-500 dark:text-gray-400">当前评级</span>
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
// 使用默认布局（包含顶部导航栏）
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
          label: '技能评级',
          data: res.map((item: any) => 
            parseFloat(item.averageRating || item.AverageRating || item.rating || item.Rating || 0)
          ),
          backgroundColor: 'rgba(59, 130, 246, 0.2)',
          borderColor: 'rgb(59, 130, 246)',
          borderWidth: 2,
          pointBackgroundColor: 'rgb(59, 130, 246)',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: 'rgb(59, 130, 246)'
        }]
      }
    } else if (timeRange.value === 'current') {
      // 如果没有时间范围，使用当前评级
      const allSkills: any[] = []
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
          label: '技能评级',
          data: allSkills.map(s => parseFloat(s.rating)),
          backgroundColor: 'rgba(59, 130, 246, 0.2)',
          borderColor: 'rgb(59, 130, 246)',
          borderWidth: 2,
          pointBackgroundColor: 'rgb(59, 130, 246)',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
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

// 保留模拟数据作为注释，方便参考
// const mockSkillTree = [
//   {
//     id: 1,
//     name: '前端开发',
//     icon: '💻',
//     color: '#3b82f6',
//     skills: [
//       {
//         id: 1,
//         name: 'Vue.js',
//         icon: '⚡',
//         description: '渐进式JavaScript框架，用于构建用户界面',
//         currentRating: 8.5
//       },
//       {
//         id: 2,
//         name: 'Nuxt.js',
//         icon: '🚀',
//         description: '基于Vue.js的通用应用框架',
//         currentRating: 8.0
//       },
//       {
//         id: 3,
//         name: 'TypeScript',
//         icon: '📘',
//         description: 'JavaScript的超集，提供静态类型检查',
//         currentRating: 7.5
//       },
//       {
//         id: 4,
//         name: 'Tailwind CSS',
//         icon: '🎨',
//         description: '实用优先的CSS框架',
//         currentRating: 7.0
//       }
//     ]
//   },
//   {
//     id: 2,
//     name: '后端开发',
//     icon: '⚙️',
//     color: '#10b981',
//     skills: [
//       {
//         id: 5,
//         name: '.NET Core',
//         icon: '🔷',
//         description: '跨平台的开源开发框架',
//         currentRating: 8.0
//       },
//       {
//         id: 6,
//         name: 'Node.js',
//         icon: '🟢',
//         description: '基于Chrome V8引擎的JavaScript运行时',
//         currentRating: 7.5
//       },
//       {
//         id: 7,
//         name: 'Entity Framework',
//         icon: '🗄️',
//         description: 'Microsoft的ORM框架',
//         currentRating: 7.5
//       },
//       {
//         id: 8,
//         name: 'RESTful API',
//         icon: '🌐',
//         description: '设计和开发RESTful风格的API',
//         currentRating: 8.5
//       }
//     ]
//   },
//   {
//     id: 3,
//     name: '数据库',
//     icon: '💾',
//     color: '#f59e0b',
//     skills: [
//       {
//         id: 9,
//         name: 'MySQL',
//         icon: '🗄️',
//         description: '开源关系型数据库管理系统',
//         currentRating: 7.5
//       },
//       {
//         id: 10,
//         name: 'PostgreSQL',
//         icon: '🐘',
//         description: '功能强大的开源对象关系数据库',
//         currentRating: 6.5
//       },
//       {
//         id: 11,
//         name: 'Redis',
//         icon: '🔴',
//         description: '内存数据结构存储系统',
//         currentRating: 6.0
//       }
//     ]
//   },
//   {
//     id: 4,
//     name: 'AI & 机器学习',
//     icon: '🤖',
//     color: '#8b5cf6',
//     skills: [
//       {
//         id: 12,
//         name: 'LangChain',
//         icon: '🔗',
//         description: '构建LLM应用的框架',
//         currentRating: 7.0
//       },
//       {
//         id: 13,
//         name: 'OpenAI API',
//         icon: '🧠',
//         description: 'OpenAI的API集成和使用',
//         currentRating: 7.5
//       },
//       {
//         id: 14,
//         name: 'Python',
//         icon: '🐍',
//         description: '通用编程语言，AI领域常用',
//         currentRating: 7.0
//       }
//     ]
//   }
// ]

// 保留模拟分类数据作为注释，方便参考
// const mockCategories = [
//   { id: 1, name: '前端开发', icon: '💻' },
//   { id: 2, name: '后端开发', icon: '⚙️' },
//   { id: 3, name: '数据库', icon: '💾' },
//   { id: 4, name: 'AI & 机器学习', icon: '🤖' }
// ]

const fetchSkillTree = async () => {
  loading.value = true
  try {
    // 优先从新的MockData API获取
    const res = await api.get<any>('/MockData/skill-tree')
    if (res && Array.isArray(res) && res.length > 0) {
      skillTree.value = res
      return
    }
    
    // 如果新API没有数据，尝试从旧API获取
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
    // 优先从新的MockData API获取
    const res = await api.get<any[]>('/MockData/skill-categories')
    if (res && Array.isArray(res) && res.length > 0) {
      categories.value = res
      return
    }
    
    // 如果新API没有数据，尝试从旧API获取
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

// 设置页面标题
useHead({
  title: '技能树 - 溪午听风',
  meta: [
    { name: 'description', content: '我的技术能力展示和技能树' }
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

/* 技能树发光效果 */
.skill-tree-card {
  transition: all 0.3s ease;
}

:global(.skill-tree-glowing) .skill-tree-card {
  animation: skill-tree-glow 3s ease-in-out;
  box-shadow: 0 0 30px rgba(59, 130, 246, 0.6);
  border-color: rgba(59, 130, 246, 0.5);
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

