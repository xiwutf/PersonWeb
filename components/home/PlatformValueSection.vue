<template>
  <section class="platform-value-section py-24 sm:py-32 relative">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12">
      <div class="text-center mb-16">
        <h2 class="text-4xl lg:text-5xl font-bold text-text-main mb-4">这不是普通个人主页</h2>
        <p class="text-lg text-text-muted max-w-2xl mx-auto">
          这是一个 AI 创作平台，围绕个人 IP 构建的创作操作系统
        </p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div
          v-for="(value, index) in values"
          :key="index"
          class="value-card"
          :ref="el => { if (el) cardRefs[index] = el as HTMLElement }"
        >
          <div class="card-inner">
            <!-- 图标 -->
            <div class="card-icon" :class="value.iconBg">
              <i :class="value.icon" class="text-3xl"></i>
            </div>
            
            <!-- 标题 -->
            <h3 class="card-title">{{ value.title }}</h3>
            
            <!-- 描述 -->
            <p class="card-description">{{ value.description }}</p>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { animate, inView } from '@motionone/dom'

const cardRefs = ref<(HTMLElement | null)[]>([])

const values = [
  {
    icon: 'fas fa-robot',
    iconBg: 'bg-gradient-to-br from-cyan-500/20 to-blue-500/20 text-cyan-400',
    title: 'AI 能力内置',
    description: '在博客、项目、工具中无缝调用 AI，辅助创作与开发。'
  },
  {
    icon: 'fas fa-sitemap',
    iconBg: 'bg-gradient-to-br from-purple-500/20 to-pink-500/20 text-purple-400',
    title: '创作者工作流整合',
    description: '文章、项目、工具、实验室统一在一个空间里管理和展示。'
  },
  {
    icon: 'fas fa-crown',
    iconBg: 'bg-gradient-to-br from-yellow-500/20 to-orange-500/20 text-yellow-400',
    title: '个人品牌 OS',
    description: '围绕你这个人构建的数字资产系统，而不是单一博客或 Landing Page。'
  }
]

onMounted(() => {
  // 使用 inView 实现滚动进入时的动画
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
.platform-value-section {
  background-color: var(--color-bg-body);
}

.value-card {
  @apply relative;
}

.card-inner {
  @apply relative bg-bg-card backdrop-blur-xl border border-border-subtle rounded-3xl p-8 h-full transition-all duration-300;
  box-shadow: var(--shadow-md);
}

.value-card:hover .card-inner {
  @apply scale-105 border-border-default;
  box-shadow: var(--shadow-lg);
  transform: translateY(-4px);
}

.card-icon {
  @apply w-16 h-16 rounded-2xl flex items-center justify-center mb-6;
}

.card-title {
  @apply text-2xl font-bold text-text-main mb-4;
}

.card-description {
  @apply text-base text-text-muted leading-relaxed;
}
</style>

