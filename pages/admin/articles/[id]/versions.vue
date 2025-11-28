<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-800 dark:text-white">文章版本历史</h1>
        <p class="text-sm text-gray-500 dark:text-gray-400 mt-1">{{ articleTitle }}</p>
      </div>
      <NuxtLink :to="`/admin/articles/edit/${articleId}`" class="px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition">
        返回编辑
      </NuxtLink>
    </div>

    <div v-if="loading" class="text-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <div v-else-if="versions.length === 0" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-8 text-center">
      <p class="text-gray-500 dark:text-gray-400">暂无版本历史</p>
    </div>

    <div v-else class="space-y-4">
      <!-- 版本列表 -->
      <div
        v-for="version in versions"
        :key="version.id"
        class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 hover:shadow-md transition-shadow"
      >
        <div class="flex items-start justify-between">
          <div class="flex-1">
            <div class="flex items-center gap-3 mb-2">
              <span class="px-3 py-1 bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 rounded-full text-sm font-semibold">
                v{{ version.version }}
              </span>
              <span
                v-if="version.status === 1"
                class="px-2 py-1 bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200 rounded text-xs"
              >
                当前版本
              </span>
              <span
                v-else
                class="px-2 py-1 bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400 rounded text-xs"
              >
                历史版本
              </span>
            </div>
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-1">{{ version.title }}</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              {{ formatDate(version.updatedAt) }}
            </p>
          </div>
          <div class="flex gap-2">
            <button
              @click="viewVersion(version.id)"
              class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
            >
              查看
            </button>
            <button
              v-if="version.status !== 1"
              @click="restoreVersion(version.id)"
              class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition"
            >
              恢复
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- 版本对比对话框 -->
    <div
      v-if="showCompare"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showCompare = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-6xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        <div class="flex justify-between items-center p-6 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-xl font-bold text-gray-900 dark:text-white">版本对比</h2>
          <button
            @click="showCompare = false"
            class="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
          >
            <i class="fas fa-times text-2xl"></i>
          </button>
        </div>
        <div class="flex-1 overflow-auto p-6">
          <div v-if="compareLoading" class="text-center py-12">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          </div>
          <div v-else-if="currentVersion && selectedVersion" class="grid grid-cols-2 gap-6">
            <!-- 当前版本 -->
            <div>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
                当前版本 (v{{ currentVersion.version }})
              </h3>
              <div class="prose dark:prose-invert max-w-none">
                <div v-html="currentVersion.contentHtml || ''"></div>
              </div>
            </div>
            <!-- 选中版本 -->
            <div>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
                历史版本 (v{{ selectedVersion.version }})
              </h3>
              <div class="prose dark:prose-invert max-w-none">
                <div v-html="selectedVersion.contentHtml || ''"></div>
              </div>
            </div>
          </div>
        </div>
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

const route = useRoute()
const api = useApi()
const { success } = useNotification()
const { handleError } = useErrorHandler()

const articleId = computed(() => route.params.id as string)
const articleTitle = ref('')
const versions = ref<any[]>([])
const loading = ref(true)
const showCompare = ref(false)
const compareLoading = ref(false)
const currentVersion = ref<any>(null)
const selectedVersion = ref<any>(null)

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const fetchVersions = async () => {
  loading.value = true
  try {
    const res = await api.get<any[]>(`/Articles/${articleId.value}/versions`)
    versions.value = res || []
    
    // 获取文章标题
    if (versions.value.length > 0) {
      articleTitle.value = versions.value.find(v => v.status === 1)?.title || versions.value[0].title
    }
  } catch (e: unknown) {
    handleError(e, '获取版本历史失败')
  } finally {
    loading.value = false
  }
}

const viewVersion = async (versionId: number) => {
  compareLoading.value = true
  showCompare.value = true
  
  try {
    // 获取当前版本
    const current = await api.get<any>(`/Articles/${articleId.value}`)
    currentVersion.value = current

    // 获取选中版本
    const version = await api.get<any>(`/Articles/${articleId.value}/versions/${versionId}`)
    selectedVersion.value = version
  } catch (e: unknown) {
    handleError(e, '获取版本内容失败')
    showCompare.value = false
  } finally {
    compareLoading.value = false
  }
}

const restoreVersion = async (versionId: number) => {
  if (!confirm('确定要恢复这个版本吗？当前版本将被保存为历史版本。')) return

  try {
    await api.post(`/Articles/${articleId.value}/versions/${versionId}/restore`)
    success('恢复成功')
    await fetchVersions()
  } catch (e: unknown) {
    handleError(e, '恢复失败')
  }
}

onMounted(() => {
  fetchVersions()
})
</script>

<style scoped>
:deep(.prose) {
  @apply text-sm;
}
</style>

