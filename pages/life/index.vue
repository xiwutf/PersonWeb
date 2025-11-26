<template>
  <div class="bg-gradient-to-br from-orange-50 via-amber-50 to-yellow-50 min-h-screen">
    <!-- 页面头部 -->
    <section class="py-16 bg-gradient-to-r from-orange-500 to-amber-500 text-white">
      <div class="max-w-6xl mx-auto px-4 text-center">
        <div class="w-20 h-20 bg-white/20 rounded-2xl flex items-center justify-center mx-auto mb-6">
          <span class="text-3xl">☕</span>
        </div>
        <h1 class="text-4xl lg:text-5xl font-bold mb-4">生活随笔</h1>
        <p class="text-xl text-orange-100 max-w-3xl mx-auto">
          记录生活点滴，分享思考感悟，在代码之外寻找诗与远方
        </p>
      </div>
    </section>

    <!-- 文章列表 -->
    <section class="py-20">
      <div class="max-w-4xl mx-auto px-4">
        
        <!-- 没有文章时的提示 -->
        <div v-if="!posts || posts.length === 0" class="text-center py-12">
          <div class="text-6xl mb-4">🍃</div>
          <h3 class="text-xl font-semibold text-gray-700 mb-2">暂无随笔</h3>
          <p class="text-gray-500">博主正在酝酿第一篇生活感悟...</p>
        </div>
        
        <!-- 文章列表 -->
        <div v-else class="space-y-8">
          <article
            v-for="post in posts"
            :key="post._path"
            class="bg-white rounded-2xl shadow-sm hover:shadow-md transition-all duration-300 p-8 border border-orange-100"
          >
            <div class="flex flex-col md:flex-row gap-6">
              <div class="flex-1">
                <div class="flex items-center gap-3 text-sm text-gray-500 mb-3">
                  <span class="px-3 py-1 bg-orange-100 text-orange-700 rounded-full text-xs font-medium">
                    {{ post.category || '随笔' }}
                  </span>
                  <span>{{ formatDate(post.date) }}</span>
                </div>
                
                <NuxtLink
                  :to="post._path"
                  class="block group"
                >
                  <h2 class="text-2xl font-bold text-gray-800 mb-3 group-hover:text-orange-600 transition-colors">
                    {{ post.title }}
                  </h2>
                  <p class="text-gray-600 leading-relaxed mb-4 line-clamp-3">
                    {{ post.description }}
                  </p>
                </NuxtLink>
                
                <div class="flex items-center justify-between mt-4">
                  <div class="flex gap-2">
                    <span 
                      v-for="tag in post.tags" 
                      :key="tag"
                      class="text-xs text-gray-400"
                    >
                      #{{ tag }}
                    </span>
                  </div>
                  <NuxtLink
                    :to="post._path"
                    class="text-orange-600 hover:text-orange-700 font-medium text-sm flex items-center"
                  >
                    阅读全文 <span class="ml-1">→</span>
                  </NuxtLink>
                </div>
              </div>
              
              <!-- 如果有封面图 -->
              <div v-if="post.cover" class="w-full md:w-48 h-32 flex-shrink-0 rounded-xl overflow-hidden">
                <img :src="post.cover" :alt="post.title" class="w-full h-full object-cover hover:scale-105 transition-transform duration-500" />
              </div>
            </div>
          </article>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
// 使用 @nuxt/content 查询生活随笔数据
const { data: posts } = await useAsyncData('life-posts', () =>
  queryContent('/life').sort({ date: -1 }).find()
)

// 格式化日期
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// 设置页面标题
useHead({
  title: '生活随笔 - 溪午听风',
  meta: [
    { name: 'description', content: '记录生活点滴，分享思考感悟' }
  ]
})
</script>

<style scoped>
/* 页面淡入动画 */
@keyframes fadeInUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

section {
  animation: fadeInUp 0.6s ease-out;
}
</style>
