<template>
  <div class="min-h-screen bg-[#050505] text-white selection:bg-cyan-500/30 font-sans">
    <!-- 背景特效 -->
    <div class="fixed inset-0 pointer-events-none">
      <div class="absolute top-0 left-0 w-full h-px bg-gradient-to-r from-transparent via-cyan-500/50 to-transparent"></div>
      <div class="absolute bottom-0 left-0 w-full h-px bg-gradient-to-r from-transparent via-purple-500/50 to-transparent"></div>
      <div class="absolute top-1/4 left-1/4 w-96 h-96 bg-cyan-500/10 rounded-full blur-[100px] animate-pulse"></div>
      <div class="absolute bottom-1/4 right-1/4 w-96 h-96 bg-purple-500/10 rounded-full blur-[100px] animate-pulse animation-delay-2000"></div>
      <!-- 网格背景 -->
      <div class="absolute inset-0 bg-[url('/images/grid.svg')] bg-center [mask-image:radial-gradient(ellipse_at_center,black_40%,transparent_70%)] opacity-20"></div>
    </div>

    <div class="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
      <!-- 头部 -->
      <div class="text-center mb-20" data-aos="fade-down">
        <div class="inline-flex items-center px-3 py-1 rounded-full border border-cyan-500/30 bg-cyan-500/10 text-cyan-400 text-xs font-mono mb-6 tracking-widest">
          <span class="w-2 h-2 bg-cyan-500 rounded-full mr-2 animate-ping"></span>
          SYSTEM ONLINE
        </div>
        <h1 class="text-5xl md:text-7xl font-bold mb-6 tracking-tight">
          <span class="text-transparent bg-clip-text bg-gradient-to-r from-cyan-400 via-blue-500 to-purple-600">AI LAB</span>
          <span class="text-slate-700 ml-4 hidden md:inline-block">///</span>
        </h1>
        <p class="text-slate-400 text-lg max-w-2xl mx-auto leading-relaxed">
          探索智能的边界。这里汇集了我的 AI 智能体与自动化工作流，展示未来生产力的无限可能。
        </p>
      </div>

      <!-- 智能体板块 -->
      <section class="mb-24">
        <div class="flex items-center justify-between mb-10 border-b border-slate-800 pb-4">
          <h2 class="text-2xl font-bold flex items-center">
            <i class="fas fa-robot text-cyan-400 mr-3"></i>
            智能体 <span class="text-slate-600 text-sm ml-3 font-mono">AGENTS</span>
          </h2>
          <span class="text-xs text-cyan-500 font-mono border border-cyan-500/30 px-2 py-1 rounded bg-cyan-500/10">
            {{ agents?.length || 0 }} ACTIVE
          </span>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <NuxtLink 
            v-for="agent in agents" 
            :key="agent._path" 
            :to="`/ai/agents/${agent._path.split('/').pop()}`"
            class="group relative bg-slate-900/50 border border-slate-800 rounded-2xl p-6 hover:border-cyan-500/50 transition-all duration-300 hover:-translate-y-1 overflow-hidden"
          >
            <div class="absolute inset-0 bg-gradient-to-br from-cyan-500/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>
            
            <div class="relative z-10">
              <div class="flex justify-between items-start mb-6">
                <div class="w-12 h-12 bg-slate-800 rounded-xl flex items-center justify-center text-2xl border border-slate-700 group-hover:border-cyan-500/30 group-hover:shadow-[0_0_15px_rgba(6,182,212,0.2)] transition-all">
                  {{ agent.icon }}
                </div>
                <div class="px-2 py-1 rounded bg-green-500/10 border border-green-500/20 text-green-400 text-xs font-mono">
                  {{ agent.status }}
                </div>
              </div>
              
              <h3 class="text-xl font-bold mb-2 group-hover:text-cyan-400 transition-colors">{{ agent.title }}</h3>
              <p class="text-slate-400 text-sm mb-6 line-clamp-2">{{ agent.description }}</p>
              
              <div class="flex flex-wrap gap-2">
                <span v-for="cap in agent.capabilities" :key="cap" class="text-xs text-slate-500 bg-slate-800/80 px-2 py-1 rounded border border-slate-700">
                  {{ cap }}
                </span>
              </div>
            </div>
          </NuxtLink>
        </div>
      </section>

      <!-- 工作流板块 -->
      <section>
        <div class="flex items-center justify-between mb-10 border-b border-slate-800 pb-4">
          <h2 class="text-2xl font-bold flex items-center">
            <i class="fas fa-network-wired text-purple-400 mr-3"></i>
            工作流 <span class="text-slate-600 text-sm ml-3 font-mono">WORKFLOWS</span>
          </h2>
          <span class="text-xs text-purple-500 font-mono border border-purple-500/30 px-2 py-1 rounded bg-purple-500/10">
            {{ workflows?.length || 0 }} DEPLOYED
          </span>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <NuxtLink 
            v-for="flow in workflows" 
            :key="flow._path" 
            :to="`/ai/workflows/${flow._path.split('/').pop()}`"
            class="group relative bg-slate-900/50 border border-slate-800 rounded-2xl p-6 hover:border-purple-500/50 transition-all duration-300 hover:-translate-y-1 overflow-hidden"
          >
            <div class="absolute inset-0 bg-gradient-to-br from-purple-500/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>
            
            <div class="relative z-10 flex items-start gap-6">
              <div class="w-16 h-16 bg-slate-800 rounded-xl flex items-center justify-center text-3xl border border-slate-700 group-hover:border-purple-500/30 group-hover:shadow-[0_0_15px_rgba(168,85,247,0.2)] transition-all shrink-0">
                {{ flow.icon }}
              </div>
              
              <div class="flex-1">
                <div class="flex justify-between items-start mb-2">
                  <h3 class="text-xl font-bold group-hover:text-purple-400 transition-colors">{{ flow.title }}</h3>
                  <div class="flex items-center gap-3 text-xs font-mono text-slate-500">
                    <span class="flex items-center"><i class="fas fa-clock mr-1"></i> {{ flow.duration }}</span>
                    <span class="flex items-center"><i class="fas fa-layer-group mr-1"></i> {{ flow.steps }} Steps</span>
                  </div>
                </div>
                
                <p class="text-slate-400 text-sm mb-4">{{ flow.description }}</p>
                
                <div class="w-full bg-slate-800 rounded-full h-1.5 overflow-hidden">
                  <div class="bg-gradient-to-r from-purple-500 to-blue-500 h-full w-3/4 rounded-full animate-pulse"></div>
                </div>
              </div>
            </div>
          </NuxtLink>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
const { data: agents } = await useAsyncData('agents', () => queryContent('/ai/agents').find())
const { data: workflows } = await useAsyncData('workflows', () => queryContent('/ai/workflows').find())

definePageMeta({
  layout: 'ai'
})

useHead({
  title: 'AI Lab - 溪午听风',
  meta: [
    { name: 'description', content: '探索 AI 智能体与自动化工作流' }
  ]
})
</script>

<style scoped>
.animation-delay-2000 {
  animation-delay: 2s;
}
</style>

