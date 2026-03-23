<template>
  <AdminModuleGuard module-key="cognition" :show-fallback="true">
    <div class="cognition-page">
      <!-- 全局背景噪点 -->
      <div class="cognition-background-noise"></div>

      <!-- 动态背景光斑 -->
      <div class="cognition-background-container">
        <div class="cognition-background-blob cognition-background-blob--blue"></div>
        <div class="cognition-background-blob cognition-background-blob--purple"></div>
        <div class="cognition-background-blob cognition-background-blob--emerald"></div>
      </div>

      <div class="cognition-content">
        <!-- 页面头部 -->
        <header class="cognition-header">
          <div class="cognition-header-left">
            <div class="cognition-header-icon">🧠</div>
            <h1 class="cognition-title">个人认知使用说明书</h1>
          </div>
          <NuxtLink to="/about" class="cognition-back-link">
            <i class="fas fa-arrow-left"></i>
            返回关于
          </NuxtLink>
        </header>

        <div v-if="loading" class="cognition-loading">
          <div class="cognition-loading-spinner"></div>
          <p class="cognition-loading-text">加载中...</p>
        </div>

        <div v-else-if="!moduleEnabled" class="cognition-empty">
          <div class="cognition-empty-icon">🔒</div>
          <h3 class="cognition-empty-title">模块未启用</h3>
          <p class="cognition-empty-text">请前往后台启用认知说明书模块</p>
        </div>
        <div v-else-if="docs.length === 0" class="cognition-empty">
          <div class="cognition-empty-icon">📝</div>
          <h3 class="cognition-empty-title">暂无内容</h3>
          <p class="cognition-empty-text">认知说明书尚未发布</p>
        </div>

        <div v-else class="cognition-list">
          <article
            v-for="doc in docs"
            :key="doc.id"
            class="cognition-item"
            @click="goToDetail(doc.slug)"
          >
            <div class="cognition-item-header">
              <h2 class="cognition-item-title">{{ doc.title }}</h2>
              <div class="cognition-item-meta">
                <span class="cognition-item-date">{{ formatDate(doc.updatedAt) }}</span>
              </div>
            </div>
            <p v-if="doc.summary" class="cognition-item-summary">{{ doc.summary }}</p>
            <div class="cognition-item-footer">
              <span class="cognition-item-read-more">阅读全文 →</span>
            </div>
          </article>
        </div>
      </div>
    </div>
  </AdminModuleGuard>
</template>

<script setup lang="ts">
import '~/assets/css/cognition.css'

definePageMeta({
  layout: 'default'
})

const api = useApi()
const router = useRouter()
const { isModuleEnabled } = useModuleSystem()
const docs = ref<any[]>([])
const loading = ref(true)

const moduleEnabled = computed(() => isModuleEnabled('cognition'))

// 检查模块是否启用
const fetchDocs = async () => {
  loading.value = true
  try {
    const res = await api.get('/CognitionDocs', {
      params: {
        status: 'published',
        page: 1,
        pageSize: 100
      }
    })

    // 兼容 PascalCase 和 camelCase
    const list = res?.List || res?.list || []
    
    // 转换字段名为 camelCase（如果后端返回的是 PascalCase）
    docs.value = list.map((item: any) => ({
      id: item.Id || item.id,
      title: item.Title || item.title,
      slug: item.Slug || item.slug,
      summary: item.Summary || item.summary,
      status: item.Status || item.status,
      updatedAt: item.UpdatedAt || item.updatedAt
    }))

  } catch (e) {
    console.error('Failed to fetch cognition docs:', e)
    docs.value = []
  } finally {
    loading.value = false
  }
}

// 跳转到详情页
const goToDetail = (slug: string) => {
  router.push(`/cognition/${slug}`)
}

// 跳转到详情页
const formatDate = (dateString?: string | Date) => {
  if (!dateString) return ''
  const date = typeof dateString === 'string' ? new Date(dateString) : dateString
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// SEO
useHead({
  title: '个人认知使用说明书 - 溪午听风',
  meta: [
    { name: 'description', content: '偏理科思维、模型驱动、厌恶无效记忆的认知系统使用指南' }
  ]
})

onMounted(() => {
  console.log('认知模块是否启用:', moduleEnabled.value)
  if (moduleEnabled.value) {
    fetchDocs()
  } else {
    console.warn('认知模块未启用，请前往后台启用')
    loading.value = false
  }
})
</script>

<style scoped>
/* 样式已移至 assets/css/cognition.css */
</style>
