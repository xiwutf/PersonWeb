<template>
  <section class="featured-section py-24 sm:py-32 relative">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12">
      <div class="text-center mb-16">
        <h2 class="text-4xl lg:text-5xl font-bold text-text-main mb-4">精选内容</h2>
        <p class="text-lg text-text-muted">Featured</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div
          v-for="(item, index) in featuredItems"
          :key="index"
          class="featured-card group"
          :ref="el => { if (el) cardRefs[index] = el as HTMLElement }"
        >
          <NuxtLink :to="item.path" class="card-link">
            <div class="card-inner">
              <!-- 背景渐变流动效果 -->
              <div class="card-gradient" :class="item.gradientBg"></div>
              
              <!-- 内容 -->
              <div class="card-content">
                <div class="card-badge" :class="item.badgeBg">
                  {{ item.type }}
                </div>
                
                <h3 class="card-title">{{ item.title }}</h3>
                <p class="card-description">{{ item.description }}</p>
                
                <div class="card-footer">
                  <span class="card-link-text">查看详情</span>
                  <i class="fas fa-arrow-right ml-2 group-hover:translate-x-1 transition-transform"></i>
                </div>
              </div>
            </div>
          </NuxtLink>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { animate, inView } from '@motionone/dom'

const cardRefs = ref<(HTMLElement | null)[]>([])

// 暂时使用静态 mock 数据，后续可接入后端 API
const featuredItems = [
  {
    type: 'Featured Tool',
    title: '在线工具示例',
    description: '一个实用的在线小工具，帮助提升工作效率。',
    path: '/tools',
    gradientBg: 'bg-gradient-to-br from-blue-500/10 to-cyan-500/10',
    badgeBg: 'bg-blue-500/20 text-blue-400 border-blue-400/40'
  },
  {
    type: 'Featured Project',
    title: '当前主网站',
    description: '这个网站本身就是一个项目，展示了完整的全栈开发能力。',
    path: '/projects',
    gradientBg: 'bg-gradient-to-br from-purple-500/10 to-pink-500/10',
    badgeBg: 'bg-purple-500/20 text-purple-400 border-purple-400/40'
  },
  {
    type: 'Featured Article',
    title: '精选博客文章',
    description: '一篇最想别人看到的博客文章，展示技术深度和思考。',
    path: '/blog',
    gradientBg: 'bg-gradient-to-br from-emerald-500/10 to-teal-500/10',
    badgeBg: 'bg-emerald-500/20 text-emerald-400 border-emerald-400/40'
  }
]

onMounted(() => {
  cardRefs.value.forEach((card, index) => {
    if (card) {
      inView(card, () => {
        animate(
          card,
          { opacity: [0, 1], y: [50, 0] },
          { duration: 0.6, delay: index * 0.15, easing: 'ease-out' }
        )
      })
    }
  })
})
</script>

<style scoped>
.featured-section {
  background-color: var(--color-bg-body);
}

.featured-card {
  @apply relative;
}

.card-link {
  @apply block h-full;
}

.card-inner {
  @apply relative bg-bg-card backdrop-blur-xl border border-border-subtle rounded-3xl p-8 h-full transition-all duration-300 overflow-hidden;
  box-shadow: var(--shadow-md);
}

.featured-card:hover .card-inner {
  @apply scale-[1.02] border-border-default;
  box-shadow: var(--shadow-lg);
  transform: translateY(-4px);
}

.card-gradient {
  @apply absolute inset-0 opacity-0 transition-opacity duration-500;
}

.featured-card:hover .card-gradient {
  @apply opacity-100;
}

.card-content {
  @apply relative z-10 flex flex-col h-full;
}

.card-badge {
  @apply inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold mb-4 border backdrop-blur-sm;
}

.card-title {
  @apply text-2xl font-bold text-text-main mb-3;
}

.card-description {
  @apply text-base text-text-muted mb-6 flex-grow leading-relaxed;
}

.card-footer {
  @apply flex items-center text-text-main font-medium text-sm group-hover:text-primary-base transition-colors;
}

.card-link-text {
  @apply mr-2;
}
</style>

