<template>
  <div class="min-h-screen bg-[#050505] text-white selection:bg-cyan-500/30 font-sans pt-20">
    <!-- 顶部导航栏 -->
    <div class="fixed top-0 left-0 w-full z-50 bg-[#050505]/80 backdrop-blur-md border-b border-slate-800">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        <NuxtLink to="/ai" class="flex items-center text-slate-400 hover:text-white transition-colors">
          <i class="fas fa-arrow-left mr-2"></i> 返回实验室
        </NuxtLink>
        <div class="font-mono text-xs text-slate-500">
          AI LAB /// {{ type.toUpperCase() }} /// {{ item?.title }}
        </div>
      </div>
    </div>

    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <ContentDoc v-slot="{ doc }">
        <!-- 头部信息 -->
        <div class="flex items-center gap-6 mb-12" data-aos="fade-down">
          <div class="w-24 h-24 bg-slate-900 rounded-2xl flex items-center justify-center text-5xl border border-slate-800 shadow-2xl shadow-cyan-500/10">
            {{ doc.icon }}
          </div>
          <div>
            <h1 class="text-4xl font-bold mb-2">{{ doc.title }}</h1>
            <p class="text-slate-400 text-lg">{{ doc.description }}</p>
            <div class="flex gap-3 mt-4">
              <span class="px-2 py-1 rounded bg-slate-800 border border-slate-700 text-xs font-mono text-cyan-400">
                STATUS: {{ doc.status }}
              </span>
              <span v-if="doc.role" class="px-2 py-1 rounded bg-slate-800 border border-slate-700 text-xs font-mono text-purple-400">
                ROLE: {{ doc.role }}
              </span>
              <span v-if="doc.duration" class="px-2 py-1 rounded bg-slate-800 border border-slate-700 text-xs font-mono text-green-400">
                TIME: {{ doc.duration }}
              </span>
            </div>
          </div>
        </div>

        <!-- 主要内容区 -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- 左侧：文档内容 -->
          <div class="lg:col-span-2 space-y-8">
            <div class="bg-slate-900/50 border border-slate-800 rounded-2xl p-8 prose prose-invert prose-cyan max-w-none">
              <ContentRenderer :value="doc" />
            </div>
          </div>

          <!-- 右侧：交互/模拟区 -->
          <div class="lg:col-span-1">
            <div class="sticky top-24 space-y-6">
              <!-- 模拟聊天框 (仅针对 Agent) -->
              <div v-if="type === 'agents'" class="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden flex flex-col h-[500px]">
                <div class="p-4 border-b border-slate-800 bg-slate-900/80 backdrop-blur flex justify-between items-center">
                  <span class="text-sm font-bold">模拟对话</span>
                  <span class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></span>
                </div>
                
                <div class="flex-1 p-4 overflow-y-auto space-y-4 font-mono text-sm custom-scrollbar">
                  <div class="flex gap-3">
                    <div class="w-8 h-8 bg-cyan-900/50 rounded-lg flex items-center justify-center shrink-0">{{ doc.icon }}</div>
                    <div class="bg-slate-800 p-3 rounded-lg rounded-tl-none text-slate-300">
                      你好！我是{{ doc.title }}。有什么我可以帮你的吗？
                    </div>
                  </div>
                </div>

                <div class="p-4 border-t border-slate-800 bg-slate-900">
                  <div class="flex gap-2">
                    <input type="text" placeholder="输入消息..." class="flex-1 bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-cyan-500 transition-colors" disabled>
                    <button class="bg-cyan-600 text-white px-3 py-2 rounded-lg hover:bg-cyan-500 transition-colors disabled:opacity-50" disabled>
                      <i class="fas fa-paper-plane"></i>
                    </button>
                  </div>
                  <p class="text-[10px] text-slate-600 mt-2 text-center">演示模式：实际接入需配置 API Key</p>
                </div>
              </div>

              <!-- 模拟执行框 (仅针对 Workflow) -->
              <div v-if="type === 'workflows'" class="bg-slate-900 border border-slate-800 rounded-2xl p-6">
                <h3 class="font-bold mb-4 flex items-center">
                  <i class="fas fa-terminal mr-2 text-purple-400"></i> 执行日志
                </h3>
                <div class="font-mono text-xs space-y-2 text-slate-400">
                  <div class="flex items-center text-green-400">
                    <i class="fas fa-check mr-2"></i> [10:00:01] Workflow initialized
                  </div>
                  <div class="flex items-center">
                    <i class="fas fa-spinner fa-spin mr-2 text-blue-400"></i> [10:00:02] Loading modules...
                  </div>
                  <div class="pl-6 opacity-50">
                    > module: data_collector<br>
                    > module: analyzer
                  </div>
                  <div class="flex items-center text-slate-600">
                    <i class="far fa-circle mr-2"></i> [WAITING] Awaiting user input
                  </div>
                </div>
                <button class="w-full mt-6 bg-purple-600 hover:bg-purple-500 text-white py-2 rounded-lg text-sm font-medium transition-colors">
                  运行演示
                </button>
              </div>
            </div>
          </div>
        </div>
      </ContentDoc>
    </div>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
const type = route.params.type // 'agents' or 'workflows'
const slug = route.params.slug

// 简单的元数据设置
const { data: item } = await useAsyncData(`ai-${type}-${slug}`, () => 
  queryContent(`/ai/${type}/${slug}`).findOne()
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
  background: rgba(30, 41, 59, 0.5);
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: rgba(71, 85, 105, 0.8);
  border-radius: 2px;
}
</style>

