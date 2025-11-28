<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <div>
        <NuxtLink
          to="/admin/skill-tree"
          class="text-blue-600 hover:text-blue-800 dark:text-blue-400 mb-2 inline-block"
        >
          ← 返回技能树
        </NuxtLink>
        <h1 class="text-2xl font-bold text-gray-800 dark:text-white">{{ skill?.name || '技能详情' }}</h1>
      </div>
    </div>

    <div v-if="loading" class="text-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <div v-else-if="skill" class="space-y-6">
      <!-- 技能基本信息 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-start gap-4">
          <span v-if="skill.icon" class="text-4xl">{{ skill.icon }}</span>
          <div class="flex-1">
            <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">{{ skill.name }}</h2>
            <p v-if="skill.description" class="text-gray-600 dark:text-gray-400 mb-4">
              {{ skill.description }}
            </p>
            <div v-if="skill.category" class="flex items-center gap-2">
              <span class="text-sm text-gray-500">分类：</span>
              <span class="px-3 py-1 bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 rounded-full text-sm">
                {{ skill.category.icon }} {{ skill.category.name }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- 评级历史 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">评级历史</h3>
        <div v-if="skill.ratings && skill.ratings.length > 0" class="space-y-4">
          <div
            v-for="rating in skill.ratings"
            :key="rating.id"
            class="border-l-4 border-blue-500 pl-4 py-2"
          >
            <div class="flex items-center justify-between mb-1">
              <span class="text-lg font-bold text-gray-900 dark:text-white">{{ rating.rating }} / 10</span>
              <span class="text-sm text-gray-500 dark:text-gray-400">
                {{ formatDate(rating.recordedAt) }}
              </span>
            </div>
            <p v-if="rating.notes" class="text-sm text-gray-600 dark:text-gray-400">{{ rating.notes }}</p>
          </div>
        </div>
        <p v-else class="text-gray-500 dark:text-gray-400 text-center py-4">暂无评级记录</p>
      </div>

      <!-- 学习日志 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">学习日志</h3>
        <div v-if="skill.learningLogs && skill.learningLogs.length > 0" class="space-y-4">
          <div
            v-for="log in skill.learningLogs"
            :key="log.id"
            class="border border-gray-200 dark:border-gray-700 rounded-lg p-4"
          >
            <div class="flex items-start justify-between mb-2">
              <h4 class="font-semibold text-gray-900 dark:text-white">{{ log.title }}</h4>
              <span class="text-sm text-gray-500 dark:text-gray-400">
                {{ formatDate(log.learnedAt) }}
              </span>
            </div>
            <p v-if="log.content" class="text-sm text-gray-600 dark:text-gray-400 mb-2">
              {{ log.content }}
            </p>
            <div class="flex items-center gap-4 text-xs text-gray-500 dark:text-gray-400">
              <span v-if="log.duration">时长：{{ log.duration }} 分钟</span>
              <span v-if="log.resourceType">类型：{{ log.resourceType }}</span>
              <a
                v-if="log.resourceUrl"
                :href="log.resourceUrl"
                target="_blank"
                class="text-blue-600 hover:underline"
              >
                查看资源 →
              </a>
            </div>
          </div>
        </div>
        <p v-else class="text-gray-500 dark:text-gray-400 text-center py-4">暂无学习日志</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const route = useRoute()
const api = useApi()
const { handleError } = useErrorHandler()

const skillId = computed(() => route.params.id as string)
const skill = ref<any>(null)
const loading = ref(true)

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const fetchSkill = async () => {
  loading.value = true
  try {
    const res = await api.get<any>(`/SkillTree/skills/${skillId.value}`)
    skill.value = res
  } catch (e: unknown) {
    handleError(e, '获取技能详情失败')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchSkill()
})
</script>

