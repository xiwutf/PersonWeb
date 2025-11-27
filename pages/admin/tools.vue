<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">工具管理</h1>
      <button class="px-4 py-2 bg-purple-600 text-white rounded hover:bg-purple-700 transition-colors">
        + 新增工具
      </button>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-12 text-center text-gray-500 dark:text-gray-400">
      <div class="text-4xl mb-4">🛠️</div>
      <p>工具管理功能开发中...</p>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})
</script>
