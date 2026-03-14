<template>
  <div class="min-h-screen bg-gradient-to-br from-indigo-50 to-purple-50">
    <!-- 页面头部 -->
    <section class="py-16 bg-gradient-to-r from-indigo-600 to-purple-600 text-var(--color-bg-light, white)">
      <div class="max-w-6xl mx-auto px-4 text-center">
        <div class="w-20 h-20 bg-var(--color-bg-light, white)/20 rounded-2xl flex items-center justify-center mx-auto mb-6">
          <span class="text-3xl">🔗</span>
        </div>
        <h1 class="text-4xl lg:text-5xl font-bold mb-4">友情链接</h1>
        <p class="text-xl text-indigo-100 max-w-3xl mx-auto">
          与志同道合的朋友们分享链接
        </p>
      </div>
    </section>

    <!-- 链接列表 -->
    <section class="py-20">
      <div class="max-w-6xl mx-auto px-4">
        <div v-if="loading" class="text-center py-16">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mb-4"></div>
          <p class="text-gray-600">加载中...</p>
        </div>

        <div v-else-if="friendLinks.length === 0" class="text-center py-16">
          <div class="text-6xl mb-4">🔗</div>
          <h3 class="text-2xl font-semibold text-gray-700 mb-2">暂无友情链接</h3>
          <p class="text-gray-500">还没有添加任何友情链接</p>
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <a
            v-for="link in friendLinks"
            :key="link.id"
            :href="link.url"
            target="_blank"
            rel="noopener noreferrer"
            class="card-hover p-6 flex items-start gap-4 group"
          >
            <!-- Logo -->
            <div class="flex-shrink-0">
              <img 
                v-if="link.logoUrl" 
                :src="link.logoUrl" 
                :alt="link.name"
                class="h-16 w-16 rounded-lg object-cover border-2 border-gray-200 group-hover:border-indigo-500 transition-colors"
                @error="handleImageError"
              />
              <div v-else class="h-16 w-16 rounded-lg bg-gradient-to-br from-indigo-400 to-purple-500 flex items-center justify-center">
                <span class="text-var(--color-bg-light, white) text-2xl font-bold">{{ link.name.charAt(0).toUpperCase() }}</span>
              </div>
            </div>

            <!-- 内容 -->
            <div class="flex-1 min-w-0">
              <h3 class="text-lg font-semibold text-gray-900 mb-1 group-hover:text-indigo-600 transition-colors">
                {{ link.name }}
              </h3>
              <p v-if="link.description" class="text-sm text-gray-600 mb-2 line-clamp-2">
                {{ link.description }}
              </p>
              <div class="flex items-center gap-2 text-xs text-gray-400">
                <i class="fas fa-external-link-alt"></i>
                <span class="truncate">{{ getDomain(link.url) }}</span>
              </div>
            </div>

            <!-- 箭头图标 -->
            <div class="flex-shrink-0 text-gray-400 group-hover:text-indigo-600 transition-colors">
              <i class="fas fa-chevron-right"></i>
            </div>
          </a>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
// 使用默认布局（包含顶部导航栏）
definePageMeta({
  layout: 'default'
})

import type { FriendLink } from '~/types/api'

const api = useApi()
const friendLinks = ref<FriendLink[]>([])
const loading = ref(true)

const fetchFriendLinks = async () => {
  try {
    loading.value = true
    const res = await api.get<FriendLink[]>('/FriendLinks')
    friendLinks.value = res || []
  } catch (e: unknown) {
    console.error('Failed to fetch friend links:', e)
    friendLinks.value = []
  } finally {
    loading.value = false
  }
}

const getDomain = (url: string): string => {
  try {
    const urlObj = new URL(url)
    return urlObj.hostname.replace('www.', '')
  } catch {
    return url
  }
}

const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement
  img.style.display = 'none'
}

onMounted(() => {
  fetchFriendLinks()
})

useHead({
  title: '友情链接 - 溪午听风',
  meta: [
    { name: 'description', content: '友情链接 - 与志同道合的朋友们分享链接' }
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
</style>

