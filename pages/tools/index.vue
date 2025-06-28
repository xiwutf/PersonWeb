<template>
  <div class="bg-gradient-to-br from-orange-50 via-red-50 to-pink-50">
    <!-- 页面头部 -->
    <section class="py-16 bg-gradient-to-r from-orange-600 to-red-600 text-white">
      <div class="max-w-6xl mx-auto px-4 text-center">
        <div class="w-20 h-20 bg-white/20 rounded-2xl flex items-center justify-center mx-auto mb-6">
          <span class="text-3xl">🔧</span>
        </div>
        <h1 class="text-4xl lg:text-5xl font-bold mb-4">插件工具</h1>
        <p class="text-xl text-orange-100 max-w-3xl mx-auto">
          专业的Revit插件和自适应族，用科技提升工作效率，让复杂的设计变得简单
        </p>
      </div>
    </section>

    <!-- 工具展示 -->
    <section class="py-20">
      <div class="max-w-6xl mx-auto px-4">
        <div v-if="pending" class="text-center py-16">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-orange-600"></div>
          <p class="mt-4 text-lg text-gray-600">加载中...</p>
        </div>

        <div v-else-if="error" class="text-center py-16">
          <div class="text-6xl mb-4">😵</div>
          <h3 class="text-xl font-semibold text-gray-800 mb-2">加载失败</h3>
          <p class="text-gray-600">{{ error }}</p>
        </div>

        <div v-else>
          <!-- 统计信息 -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-16">
            <div class="bg-white rounded-2xl shadow-lg p-6 text-center">
              <div class="text-3xl font-bold text-orange-600 mb-2">{{ tools.length }}+</div>
              <div class="text-gray-600">专业工具</div>
            </div>
            <div class="bg-white rounded-2xl shadow-lg p-6 text-center">
              <div class="text-3xl font-bold text-red-600 mb-2">1000+</div>
              <div class="text-gray-600">用户选择</div>
            </div>
            <div class="bg-white rounded-2xl shadow-lg p-6 text-center">
              <div class="text-3xl font-bold text-pink-600 mb-2">99%</div>
              <div class="text-gray-600">好评率</div>
            </div>
          </div>

          <!-- 工具卡片网格 -->
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
            <div
              v-for="tool in tools"
              :key="tool._path"
              class="group bg-white rounded-2xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden transform hover:-translate-y-2"
            >
              <!-- 卡片头部 -->
              <div class="p-6 pb-4">
                <div class="flex items-start justify-between mb-4">
                  <div class="flex-1">
                    <h3 class="text-xl font-bold text-gray-800 mb-2 group-hover:text-orange-600 transition-colors">
                      {{ tool.title }}
                    </h3>
                    <div class="flex items-center space-x-2">
                      <span class="text-3xl font-bold text-green-600">¥{{ tool.price }}</span>
                      <span class="text-sm text-gray-500 line-through">¥{{ Math.round(tool.price * 1.5) }}</span>
                    </div>
                  </div>
                  <div class="w-16 h-16 bg-gradient-to-br from-orange-100 to-red-100 rounded-xl flex items-center justify-center">
                    <span class="text-2xl">🔧</span>
                  </div>
                </div>
                
                <p class="text-gray-600 leading-relaxed mb-4">{{ tool.description }}</p>
                
                <!-- 标签 -->
                <div class="flex flex-wrap gap-2 mb-6">
                  <span
                    v-for="tag in tool.tags"
                    :key="tag"
                    class="px-3 py-1 bg-orange-100 text-orange-800 text-sm rounded-full font-medium"
                  >
                    {{ tag }}
                  </span>
                </div>
              </div>

              <!-- 卡片底部按钮 -->
              <div class="px-6 pb-6">
                <div class="flex space-x-3">
                  <NuxtLink
                    :to="`/tools/detail-${tool.slug || tool._path.split('/').pop()}`"
                    class="flex-1 bg-gray-100 text-gray-800 px-4 py-3 rounded-xl hover:bg-gray-200 transition-colors text-center font-medium"
                  >
                    查看详情
                  </NuxtLink>
                  <a
                    :href="tool.buy_link"
                    target="_blank"
                    class="flex-1 bg-gradient-to-r from-orange-500 to-red-500 text-white px-4 py-3 rounded-xl hover:from-orange-600 hover:to-red-600 transition-all text-center font-medium shadow-lg"
                  >
                    立即购买
                  </a>
                </div>
              </div>
            </div>
          </div>

          <!-- 底部CTA -->
          <div class="mt-20 text-center">
            <div class="bg-gradient-to-r from-orange-500 to-red-500 rounded-2xl p-8 text-white">
              <h3 class="text-2xl font-bold mb-4">需要定制化插件？</h3>
              <p class="text-orange-100 mb-6">我们提供专业的Revit插件定制开发服务</p>
              <a
                href="mailto:contact@溪午听风.com"
                class="inline-flex items-center px-8 py-3 bg-white text-orange-600 rounded-xl font-semibold hover:bg-gray-50 transition-colors"
              >
                <span class="mr-2">📧</span>
                联系定制
              </a>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
// 使用 @nuxt/content 查询工具数据
const { data: tools, pending, error } = await useAsyncData('tools', () =>
  queryContent('/tools').sort({ date: -1 }).find()
)

// 设置页面标题和SEO
useHead({
  title: '插件工具 - 溪午听风专业Revit插件开发',
  meta: [
    { name: 'description', content: '专业的Revit插件和自适应族，用科技提升工作效率，让复杂的设计变得简单' },
    { name: 'keywords', content: 'Revit插件, 自适应族, BIM工具, 建筑设计, 工程效率' }
  ]
})
</script>

<style scoped>
/* 页面淡入动画 */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

section {
  animation: fadeInUp 0.8s ease-out;
}

section:nth-child(2) {
  animation-delay: 0.2s;
}
</style> 