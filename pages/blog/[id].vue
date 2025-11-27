<template>
  <div class="container mx-auto px-4 py-8">
    <div class="max-w-3xl mx-auto bg-white rounded-lg shadow-md p-8">
      <!-- 这是一个占位页面，用于展示博客详情结构 -->
      <div class="mb-6">
        <h1 class="text-3xl font-bold text-gray-800 mb-4">博客文章详情 (ID: {{ route.params.id }})</h1>
        <div class="flex items-center text-gray-500 text-sm">
          <span class="mr-4">📅 2024-05-20</span>
          <span>👤 作者: 溪午听风</span>
        </div>
      </div>

      <div class="prose max-w-none">
        <p>这里将通过 API 获取并展示文章的具体内容。</p>
        <p>目前正在重构中，敬请期待...</p>
        <hr />
        <h3>示例内容</h3>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</p>
      </div>

      <div class="mt-8 pt-8 border-t border-gray-200">
        <NuxtLink to="/blog" class="text-blue-600 hover:text-blue-800">
          &larr; 返回博客列表
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
// const api = useApi()

// TODO: 在这里调用 API 获取文章详情
// const { data: article } = await useAsyncData(`article-${route.params.id}`, () => 
//   api.get(`/posts/${route.params.id}`)
// )
</script>

