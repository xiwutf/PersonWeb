<template>
  <div class="container mx-auto px-4 py-8">
    <div class="max-w-4xl mx-auto bg-white rounded-xl shadow-lg overflow-hidden">
      <div class="md:flex">
        <!-- 工具封面/预览 -->
        <div class="md:w-1/2 bg-gray-100 flex items-center justify-center p-8">
          <div class="text-6xl">🛠️</div>
        </div>
        
        <!-- 工具信息 -->
        <div class="md:w-1/2 p-8">
          <div class="uppercase tracking-wide text-sm text-indigo-500 font-semibold">工具详情</div>
          <h1 class="mt-2 text-3xl font-bold text-gray-900">工具名称 (Slug: {{ route.params.slug }})</h1>
          <p class="mt-4 text-gray-600">
            这是一个工具详情页面的占位符。这里将展示工具的详细介绍、使用说明和下载/购买链接。
          </p>
          
          <div class="mt-6">
            <button class="bg-indigo-600 text-white px-6 py-2 rounded-lg hover:bg-indigo-700 transition">
              立即获取
            </button>
          </div>
        </div>
      </div>
      
      <!-- 详细说明 -->
      <div class="p-8 border-t border-gray-200">
        <h2 class="text-xl font-bold mb-4">功能特性</h2>
        <ul class="list-disc list-inside text-gray-600 space-y-2">
          <li>特性 1: 高效处理</li>
          <li>特性 2: 简单易用</li>
          <li>特性 3: 自动化工作流</li>
        </ul>
      </div>
      
      <div class="p-8 bg-gray-50">
        <NuxtLink to="/tools" class="text-indigo-600 hover:text-indigo-800 font-medium">
          &larr; 返回工具列表
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
// const api = useApi()

// TODO: 调用 API 获取工具详情
</script>
