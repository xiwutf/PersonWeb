<template>
  <div class="min-h-screen font-sans pt-20" style="background-color: var(--bg); color: var(--text-main);">
    <!-- 顶部导航栏 -->
    <div class="fixed top-0 left-0 w-full z-50 backdrop-blur-md" style="background-color: var(--header-bg, var(--bg)); border-bottom: 1px solid var(--border-color);">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        <NuxtLink to="/ai" class="flex items-center transition-colors" style="color: var(--text-secondary);" onmouseover="this.style.color='var(--text-main)'" onmouseout="this.style.color='var(--text-secondary)'">
          <i class="fas fa-arrow-left mr-2"></i> 返回实验室
        </NuxtLink>
        <div class="font-mono text-xs" style="color: var(--text-muted);">
          AI LAB /// {{ type.toUpperCase() }} /// {{ item?.title }}
        </div>
      </div>
    </div>

    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <template v-if="item">
        <!-- 头部信息 -->
        <div class="flex items-center gap-6 mb-12" data-aos="fade-down">
          <div class="w-24 h-24 rounded-2xl flex items-center justify-center text-5xl shadow-2xl" style="background-color: var(--bg-card); border: 1px solid var(--border-color);">
            {{ item.icon }}
          </div>
          <div>
            <h1 class="text-4xl font-bold mb-2" style="color: var(--text-main);">{{ item.title }}</h1>
            <p class="text-lg" style="color: var(--text-secondary);">{{ item.description }}</p>
            <div class="flex gap-3 mt-4">
              <span class="px-2 py-1 rounded text-xs font-mono doc-badge" style="background-color: var(--primary-soft-bg); border: 1px solid var(--color-primary-soft, rgba(37, 99, 235, 0.2)); color: var(--primary);">
                STATUS: {{ item.status }}
              </span>
              <span v-if="item.role" class="px-2 py-1 rounded text-xs font-mono doc-badge" style="background-color: var(--primary-soft-bg); border: 1px solid var(--color-primary-soft, rgba(37, 99, 235, 0.2)); color: var(--primary);">
                ROLE: {{ item.role }}
              </span>
              <span v-if="item.duration" class="px-2 py-1 rounded text-xs font-mono doc-badge" style="background-color: var(--primary-soft-bg); border: 1px solid var(--color-primary-soft, rgba(37, 99, 235, 0.2)); color: var(--primary);">
                TIME: {{ item.duration }}
              </span>
            </div>
          </div>
        </div>

        <!-- 主要内容区 -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- 左侧：文档内容 -->
          <div class="lg:col-span-2 space-y-8">
            <div class="rounded-2xl p-8 prose max-w-none" style="background-color: var(--bg-card); border: 1px solid var(--border-color);">
              <ContentRenderer :value="item" />
            </div>
          </div>

          <!-- 右侧：交互/模拟区 -->
          <div class="lg:col-span-1">
            <div class="sticky top-24 space-y-6">
              <!-- 模拟聊天框 (仅针对 Agent) -->
              <div v-if="type === 'agents'" class="rounded-2xl overflow-hidden flex flex-col h-[500px]" style="background-color: var(--bg-card); border: 1px solid var(--border-color);">
                <div class="p-4 backdrop-blur flex justify-between items-center" style="border-bottom: 1px solid var(--border-color); background-color: var(--bg-elevated);">
                  <span class="text-sm font-bold" style="color: var(--text-main);">模拟对话</span>
                  <span class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></span>
                </div>
                
                <div class="flex-1 p-4 overflow-y-auto space-y-4 font-mono text-sm custom-scrollbar" style="background-color: var(--bg);">
                  <div class="flex gap-3">
                    <div class="w-8 h-8 rounded-lg flex items-center justify-center shrink-0" style="background-color: var(--primary-soft-bg);">{{ item.icon }}</div>
                    <div class="p-3 rounded-lg rounded-tl-none" style="background-color: var(--bg-card); color: var(--text-main);">
                      你好！我是{{ item.title }}。有什么我可以帮你的吗？
                    </div>
                  </div>
                </div>

                <div class="p-4" style="border-top: 1px solid var(--border-color); background-color: var(--bg-card);">
                  <div class="flex gap-2">
                    <input type="text" placeholder="输入消息..." class="flex-1 rounded-lg px-3 py-2 text-sm focus:outline-none transition-colors" style="background-color: var(--bg-elevated); border: 1px solid var(--border-color); color: var(--text-main);" disabled>
                    <button class="px-3 py-2 rounded-lg transition-colors disabled:opacity-50 text-var(--color-bg-light, white)" style="background-color: var(--primary);" disabled>
                      <i class="fas fa-paper-plane"></i>
                    </button>
                  </div>
                  <p class="text-[10px] mt-2 text-center" style="color: var(--text-muted);">演示模式：实际接入需配置 API Key</p>
                </div>
              </div>

              <!-- 模拟执行框 (仅针对 Workflow) -->
              <div v-if="type === 'workflows'" class="rounded-2xl p-6" style="background-color: var(--bg-card); border: 1px solid var(--border-color);">
                <h3 class="font-bold mb-4 flex items-center" style="color: var(--text-main);">
                  <i class="fas fa-terminal mr-2" style="color: var(--primary);"></i> 执行日志
                </h3>
                <div class="font-mono text-xs space-y-2" style="color: var(--text-secondary);">
                  <div class="flex items-center" style="color: var(--primary);">
                    <i class="fas fa-check mr-2"></i> [10:00:01] Workflow initialized
                  </div>
                  <div class="flex items-center">
                    <i class="fas fa-spinner fa-spin mr-2" style="color: var(--primary);"></i> [10:00:02] Loading modules...
                  </div>
                  <div class="pl-6 opacity-50">
                    > module: data_collector<br>
                    > module: analyzer
                  </div>
                  <div class="flex items-center" style="color: var(--text-muted);">
                    <i class="far fa-circle mr-2"></i> [WAITING] Awaiting user input
                  </div>
                </div>
                <button class="w-full mt-6 py-2 rounded-lg text-sm font-medium transition-colors text-var(--color-bg-light, white)" style="background-color: var(--primary);">
                  运行演示
                </button>
              </div>
            </div>
          </div>
        </div>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
const type = route.params.type // 'agents' or 'workflows'
const slug = route.params.slug

// 简单的元数据设置（Content v3: queryCollection）
const { data: item } = await useAsyncData(`ai-${type}-${slug}`, () =>
  queryCollection('content').path(`/ai/${type}/${slug}`).first()
)

definePageMeta({
  layout: 'ai'
})

useHead({
  title: `${item.value?.title || 'Detail'} - AI Lab`,
})
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: var(--bg-elevated);
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: 2px;
}
</style>

