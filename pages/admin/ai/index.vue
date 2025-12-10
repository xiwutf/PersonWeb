<template>
  <ClientOnly>
    <div class="admin-ai-center-page p-6">
      <div class="mb-6">
        <h1 class="text-3xl font-bold mb-2">AI 智能体中心</h1>
        <p class="text-gray-600 dark:text-gray-400">使用 AI 智能体提升工作效率，自动化内容生成和数据分析</p>
      </div>

      <div class="grid gap-6 md:grid-cols-3">
        <!-- 内容生成智能体 -->
        <n-card class="agent-card" hoverable @click="navigateTo('/admin/ai/content')">
          <template #header>
            <div class="flex items-center gap-3">
              <div class="agent-icon bg-blue-100 dark:bg-blue-900">
                <i class="fas fa-file-alt text-blue-600 dark:text-blue-400"></i>
              </div>
              <div>
                <h3 class="text-lg font-semibold">内容生成智能体</h3>
                <p class="text-sm text-gray-500">ContentAgent</p>
              </div>
            </div>
          </template>
          <div class="agent-description">
            <p class="text-gray-600 dark:text-gray-400 mb-4">
              自动生成文章、项目介绍、工具介绍等内容，支持自定义语气、字数、目标受众等参数。
            </p>
            <div class="agent-features">
              <div class="feature-item">
                <i class="fas fa-check-circle text-green-500"></i>
                <span>支持多种内容类型</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle text-green-500"></i>
                <span>可自定义语气和风格</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle text-green-500"></i>
                <span>自动保存为草稿</span>
              </div>
            </div>
          </div>
          <template #action>
            <n-button type="primary" block @click.stop="navigateTo('/admin/ai/content')">
              立即使用
            </n-button>
          </template>
        </n-card>

        <!-- Demo 上架智能体 -->
        <n-card class="agent-card" hoverable>
          <template #header>
            <div class="flex items-center gap-3">
              <div class="agent-icon bg-purple-100 dark:bg-purple-900">
                <i class="fas fa-rocket text-purple-600 dark:text-purple-400"></i>
              </div>
              <div>
                <h3 class="text-lg font-semibold">Demo 上架智能体</h3>
                <p class="text-sm text-gray-500">DemoAgent</p>
              </div>
            </div>
          </template>
          <div class="agent-description">
            <p class="text-gray-600 dark:text-gray-400 mb-4">
              为项目和工具自动生成展示文案，包括标题、亮点、使用场景、目标用户等，提升上架效率。
            </p>
            <div class="agent-features">
              <div class="feature-item">
                <i class="fas fa-check-circle text-green-500"></i>
                <span>一键生成完整文案</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle text-green-500"></i>
                <span>自动保存到项目/工具</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle text-green-500"></i>
                <span>支持项目列表页操作</span>
              </div>
            </div>
          </div>
          <template #action>
            <n-button type="primary" block @click.stop="navigateTo('/admin/projects')">
              在项目列表中使用
            </n-button>
          </template>
        </n-card>

        <!-- 表单线索处理智能体 -->
        <n-card class="agent-card" hoverable>
          <template #header>
            <div class="flex items-center gap-3">
              <div class="agent-icon bg-green-100 dark:bg-green-900">
                <i class="fas fa-chart-line text-green-600 dark:text-green-400"></i>
              </div>
              <div>
                <h3 class="text-lg font-semibold">线索处理智能体</h3>
                <p class="text-sm text-gray-500">LeadAgent</p>
              </div>
            </div>
          </template>
          <div class="agent-description">
            <p class="text-gray-600 dark:text-gray-400 mb-4">
              自动分析客户咨询和需求，生成摘要、标签、评分和推荐建议，帮助快速识别高价值线索。
            </p>
            <div class="agent-features">
              <div class="feature-item">
                <i class="fas fa-check-circle text-green-500"></i>
                <span>自动生成摘要和标签</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle text-green-500"></i>
                <span>线索质量评分（0-100）</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle text-green-500"></i>
                <span>智能推荐处理建议</span>
              </div>
            </div>
          </div>
          <template #action>
            <n-button type="primary" block @click.stop="navigateTo('/admin/consultations')">
              在咨询列表中使用
            </n-button>
          </template>
        </n-card>
      </div>

      <!-- 使用说明 -->
      <n-card class="mt-6">
        <template #header>
          <h3 class="text-lg font-semibold">使用说明</h3>
        </template>
        <div class="space-y-4">
          <div>
            <h4 class="font-semibold mb-2">1. 内容生成智能体</h4>
            <p class="text-gray-600 dark:text-gray-400">
              访问 <NuxtLink to="/admin/ai/content" class="text-blue-600 hover:underline">内容生成页面</NuxtLink>，
              选择内容类型、填写参数，点击生成即可。生成的内容可以一键保存为文章草稿。
            </p>
          </div>
          <div>
            <h4 class="font-semibold mb-2">2. Demo 上架智能体</h4>
            <p class="text-gray-600 dark:text-gray-400">
              在 <NuxtLink to="/admin/projects" class="text-blue-600 hover:underline">项目列表页</NuxtLink> 中，
              点击项目操作列的「AI」按钮，填写项目信息后即可生成展示文案，并自动保存到项目。
            </p>
          </div>
          <div>
            <h4 class="font-semibold mb-2">3. 线索处理智能体</h4>
            <p class="text-gray-600 dark:text-gray-400">
              在 <NuxtLink to="/admin/consultations" class="text-blue-600 hover:underline">咨询列表页</NuxtLink> 中，
              点击咨询操作列的「AI」按钮，系统会自动分析线索并生成摘要、标签、评分和建议。
            </p>
          </div>
        </div>
      </n-card>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { NCard, NButton } from 'naive-ui'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

useHead({
  title: 'AI 智能体中心 - 后台管理',
  meta: [
    { name: 'description', content: 'AI 智能体中心，提供内容生成、Demo 上架、线索处理等 AI 能力' }
  ]
})
</script>

<style scoped>
.admin-ai-center-page {
  max-width: 1400px;
  margin: 0 auto;
}

.agent-card {
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.agent-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
}

.agent-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
}

.agent-description {
  min-height: 180px;
}

.agent-features {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.feature-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #64748b;
}
</style>

