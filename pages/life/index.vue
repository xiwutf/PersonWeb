<template>
  <div class="min-h-screen bg-[#0f172a] text-slate-200 relative overflow-hidden font-['Outfit']">
    <!-- 全局背景噪点 -->
    <div class="fixed inset-0 opacity-[0.03] mix-blend-overlay pointer-events-none z-50"
         style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);">
    </div>

    <!-- 动态背景光斑 -->
    <div class="fixed inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-0 right-1/4 w-[500px] h-[500px] bg-amber-600/10 rounded-full blur-[100px] animate-blob mix-blend-screen"></div>
      <div class="absolute bottom-0 left-1/4 w-[500px] h-[500px] bg-orange-600/10 rounded-full blur-[100px] animate-blob animation-delay-2000 mix-blend-screen"></div>
    </div>

    <div class="relative z-10 max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- 页面头部 -->
      <header class="text-center mb-20 relative">
        <div class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-amber-500/10 backdrop-blur-xl border border-amber-500/20 mb-6 animate-float">
          <span class="text-3xl">☕</span>
        </div>
        <h1 class="text-4xl md:text-5xl font-bold mb-6 bg-clip-text text-transparent bg-gradient-to-r from-amber-200 via-white to-orange-200 tracking-tight font-serif">
          生活随笔
        </h1>
        <p class="text-lg text-slate-400 max-w-xl mx-auto leading-relaxed font-serif italic">
          "记录生活点滴，分享思考感悟，在代码之外寻找诗与远方"
        </p>
      </header>

      <!-- 文章列表 -->
      <div v-if="!posts || posts.length === 0" class="text-center py-24">
        <div class="text-6xl mb-6 opacity-30 grayscale">🍃</div>
        <h3 class="text-xl font-medium text-slate-400 mb-2">暂无随笔</h3>
        <p class="text-slate-600">博主正在酝酿第一篇生活感悟...</p>
      </div>

      <div v-else class="space-y-16">
        <TransitionGroup name="list">
          <article
            v-for="(post, index) in posts"
            :key="post._path"
            class="group relative"
            :style="{ transitionDelay: `${index * 100}ms` }"
          >
            <!-- 连接线 -->
            <div v-if="index !== posts.length - 1" class="absolute left-8 top-24 bottom-[-64px] w-px bg-gradient-to-b from-white/10 to-transparent md:left-1/2 md:-ml-px z-0"></div>

            <div class="relative z-10 flex flex-col md:flex-row gap-8 items-start">
              <!-- 时间轴节点 (桌面端) -->
              <div class="hidden md:flex absolute left-1/2 -translate-x-1/2 w-4 h-4 rounded-full bg-slate-900 border-2 border-amber-500/50 mt-8 group-hover:scale-125 group-hover:border-amber-400 transition-all duration-500 shadow-[0_0_10px_rgba(245,158,11,0.3)]"></div>

              <!-- 内容卡片 -->
              <div class="w-full md:w-[calc(50%-2rem)]" :class="index % 2 === 0 ? 'md:mr-auto' : 'md:ml-auto'">
                <NuxtLink 
                  :to="post._path"
                  class="block bg-slate-800/40 backdrop-blur-md border border-white/5 rounded-2xl overflow-hidden hover:bg-slate-800/60 transition-all duration-500 hover:border-amber-500/30 hover:shadow-[0_0_30px_rgba(245,158,11,0.1)] group/card"
                >
                  <!-- 封面图 -->
                  <div v-if="post.cover" class="h-48 overflow-hidden relative">
                    <img :src="post.cover" :alt="post.title" class="w-full h-full object-cover group-hover/card:scale-105 transition-transform duration-700" />
                    <div class="absolute inset-0 bg-gradient-to-t from-slate-900/80 to-transparent opacity-60"></div>
                    <div class="absolute bottom-4 left-4 text-white font-serif text-lg font-medium drop-shadow-md">
                      {{ formatDate(post.date) }}
                    </div>
                  </div>
                  <div v-else class="h-24 bg-gradient-to-br from-slate-800 to-slate-900 flex items-center px-6 border-b border-white/5">
                     <div class="text-amber-400/80 font-serif text-lg">
                      {{ formatDate(post.date) }}
                    </div>
                  </div>

                  <div class="p-6 md:p-8">
                    <div class="flex items-center gap-3 mb-4">
                      <span class="px-2 py-0.5 rounded text-xs font-medium bg-amber-500/10 text-amber-300 border border-amber-500/20">
                        {{ post.category || '随笔' }}
                      </span>
                    </div>

                    <h2 class="text-2xl font-bold text-slate-100 mb-4 font-serif group-hover/card:text-amber-200 transition-colors">
                      {{ post.title }}
                    </h2>
                    
                    <p class="text-slate-400 leading-relaxed mb-6 line-clamp-3 font-light">
                      {{ post.description }}
                    </p>

                    <div class="flex items-center justify-between pt-4 border-t border-white/5">
                      <div class="flex gap-2">
                        <span v-for="tag in post.tags" :key="tag" class="text-xs text-slate-500">#{{ tag }}</span>
                      </div>
                      <span class="text-amber-400/80 text-sm group-hover/card:translate-x-1 transition-transform">阅读全文 &rarr;</span>
                    </div>
                  </div>
                </NuxtLink>
              </div>
            </div>
          </article>
        </TransitionGroup>
      </div>
    </div>
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

useHead({
  title: '生活随笔 - 溪午听风',
  meta: [
    { name: 'description', content: '记录生活点滴，分享思考感悟' }
  ]
})
</script>

<style scoped>
.list-enter-active,
.list-leave-active {
  transition: all 0.8s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateY(50px);
}

.animate-blob {
  animation: blob 10s infinite;
}
.animation-delay-2000 {
  animation-delay: 5s;
}
@keyframes blob {
  0% { transform: translate(0px, 0px) scale(1); }
  33% { transform: translate(30px, -30px) scale(1.1); }
  66% { transform: translate(-20px, 20px) scale(0.9); }
  100% { transform: translate(0px, 0px) scale(1); }
}

.animate-float {
  animation: float 6s ease-in-out infinite;
}
@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-5px); }
}
</style>
