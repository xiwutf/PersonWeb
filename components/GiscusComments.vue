<template>
  <div class="mt-12 pt-8 border-t border-gray-200">
    <div class="mb-6">
      <h3 class="text-2xl font-bold text-gray-800 mb-2">💬 评论讨论</h3>
      <p class="text-gray-600">欢迎分享你的想法和建议</p>
    </div>
    
    <!-- Giscus评论区域 -->
    <div class="bg-white rounded-xl p-8 shadow-sm border border-gray-100 text-center">
      <div class="inline-flex items-center justify-center w-12 h-12 rounded-full bg-blue-50 text-blue-500 mb-4">
        <i class="fas fa-code"></i>
      </div>
      <h4 class="text-lg font-medium text-gray-900 mb-2">评论功能开发中</h4>
      <p class="text-gray-500">为了提供更好的互动体验，评论系统正在升级维护中，敬请期待！</p>
    </div>
  </div>
</template>

<script setup>
defineProps({
  identifier: {
    type: String,
    required: true
  },
  title: {
    type: String,
    default: ''
  },
  theme: {
    type: String,
    default: 'light'
  }
})
</script>