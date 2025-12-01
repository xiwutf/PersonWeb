<template>
  <section class="platform-entry-hub py-24 sm:py-32 relative">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12">
      <div class="text-center mb-16">
        <h2 class="text-4xl lg:text-5xl font-bold text-text-main mb-4">平台入口 Hub</h2>
        <p class="text-lg text-text-muted max-w-2xl mx-auto">
          探索不同的数字空间，每个入口都是一个独立的功能模块
        </p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <NuxtLink
          v-for="(entry, index) in entries"
          :key="index"
          :to="entry.path"
          class="entry-card group"
          :ref="el => { if (el) cardRefs[index] = el as HTMLElement }"
        >
          <div class="card-inner">
            <!-- 背景光效 -->
            <div class="card-glow" :class="entry.glowBg"></div>
            
            <!-- 内容 -->
            <div class="card-content">
              <div class="card-icon-wrapper">
                <div class="card-icon" :class="entry.iconBg">
                  <i :class="entry.icon" class="text-3xl"></i>
                </div>
              </div>
              
              <h3 class="card-title">{{ entry.title }}</h3>
              <p class="card-description">{{ entry.description }}</p>
              
              <!-- Tag 区域 -->
              <div class="card-tags">
                <span
                  v-for="tag in entry.tags"
                  :key="tag"
                  class="tag"
                >
                  {{ tag }}
                </span>
              </div>
              
              <div class="card-action">
                <span>进入</span>
                <i class="fas fa-arrow-right ml-2 group-hover:translate-x-1 transition-transform"></i>
              </div>
            </div>
          </div>
        </NuxtLink>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { animate, inView } from '@motionone/dom'

const cardRefs = ref<(HTMLElement | null)[]>([])

const entries = [
  {
    title: 'Blog Planet',
    description: '技术、思考与随笔',
    icon: 'fas fa-globe-americas',
    iconBg: 'bg-gradient-to-br from-blue-500 to-cyan-500',
    glowBg: 'bg-gradient-to-br from-blue-500/20 to-cyan-500/20',
    tags: ['Tech', 'Life', 'AI'],
    path: '/blog'
  },
  {
    title: 'Project Dock',
    description: '个人项目 / 产品 / Demo',
    icon: 'fas fa-rocket',
    iconBg: 'bg-gradient-to-br from-purple-500 to-pink-500',
    glowBg: 'bg-gradient-to-br from-purple-500/20 to-pink-500/20',
    tags: ['Dev', 'Product'],
    path: '/projects'
  },
  {
    title: 'Toolbox',
    description: '小工具、脚本、在线工具',
    icon: 'fas fa-toolbox',
    iconBg: 'bg-gradient-to-br from-emerald-500 to-teal-500',
    glowBg: 'bg-gradient-to-br from-emerald-500/20 to-teal-500/20',
    tags: ['Tools', 'Scripts'],
    path: '/tools'
  },
  {
    title: 'AI Lab',
    description: 'AI Agent、Prompt、实验性功能',
    icon: 'fas fa-flask',
    iconBg: 'bg-gradient-to-br from-red-500 to-orange-500',
    glowBg: 'bg-gradient-to-br from-red-500/20 to-orange-500/20',
    tags: ['AI', 'Agent'],
    path: '/lab'
  }
]

onMounted(() => {
  cardRefs.value.forEach((card, index) => {
    if (card) {
      inView(card, () => {
        animate(
          card,
          { opacity: [0, 1], y: [50, 0] },
          { duration: 0.6, delay: index * 0.1, easing: 'ease-out' }
        )
      })
    }
  })
})
</script>

<style scoped>
.platform-entry-hub {
  background-color: var(--color-bg-body);
}

.entry-card {
  @apply relative block;
}

.card-inner {
  @apply relative bg-bg-card backdrop-blur-xl border border-border-subtle rounded-3xl p-8 h-full transition-all duration-300 overflow-hidden;
  box-shadow: var(--shadow-md);
}

.entry-card:hover .card-inner {
  @apply scale-105 border-border-default;
  box-shadow: var(--shadow-lg);
  transform: translateY(-4px);
}

.card-glow {
  @apply absolute inset-0 opacity-0 transition-opacity duration-300;
}

.entry-card:hover .card-glow {
  @apply opacity-100;
}

.card-content {
  @apply relative z-10 flex flex-col h-full;
}

.card-icon-wrapper {
  @apply mb-6;
}

.card-icon {
  @apply w-16 h-16 rounded-2xl flex items-center justify-center text-text-main;
}

.card-title {
  @apply text-2xl font-bold text-text-main mb-3;
}

.card-description {
  @apply text-base text-text-muted mb-4 flex-grow;
}

.card-tags {
  @apply flex flex-wrap gap-2 mb-6;
}

.tag {
  @apply px-3 py-1 rounded-full bg-bg-elevated border border-border-default text-text-muted text-xs font-medium;
}

.card-action {
  @apply inline-flex items-center text-text-main font-medium text-sm group-hover:text-primary-base transition-colors;
}
</style>

