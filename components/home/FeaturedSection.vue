<template>
  <section class="featured-section">
    <div class="featured-section-container">
      <div class="featured-section-header">
        <h2 class="featured-section-title">精选内容</h2>
        <p class="featured-section-subtitle">Featured</p>
      </div>

      <div class="featured-section-grid">
        <div
          v-for="(item, index) in featuredItems"
          :key="index"
          class="featured-card"
          :ref="el => { if (el) cardRefs[index] = el as HTMLElement }"
        >
          <NuxtLink :to="item.path" class="featured-card-link">
            <div class="featured-card-inner">
              <!-- 背景渐变流动效果 -->
              <div class="featured-card-gradient" :class="item.gradientBg"></div>
              
              <!-- 内容 -->
              <div class="featured-card-content">
                <div class="featured-card-badge" :class="item.badgeBg">
                  {{ item.type }}
                </div>
                
                <h3 class="featured-card-title">{{ item.title }}</h3>
                <p class="featured-card-description">{{ item.description }}</p>
                
                <div class="featured-card-footer">
                  <span class="featured-card-link-text">查看详情</span>
                  <i class="fas fa-arrow-right featured-card-icon"></i>
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
import { loadMotionOneDom } from '~/composables/useMotionOneDom'

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

onMounted(async () => {
  const { animate, inView } = await loadMotionOneDom()

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
/* 样式已移至 assets/css/home.css */
/* 保留组件特有的样式（如果有） */
</style>

