<template>
  <section class="platform-value-section">
    <div class="platform-value-section-container">
      <div class="platform-value-section-header">
        <h2 class="platform-value-section-title">这不是普通个人主页</h2>
        <p class="platform-value-section-subtitle">
          这是一个 AI 创作平台，围绕个人 IP 构建的创作操作系统
        </p>
      </div>

      <div class="platform-value-section-grid">
        <div
          v-for="(value, index) in values"
          :key="index"
          class="platform-value-card"
          :ref="el => { if (el) cardRefs[index] = el as HTMLElement }"
        >
          <div class="platform-value-card-inner">
            <!-- 图标 -->
            <div class="platform-value-card-icon" :class="value.iconBg">
              <i :class="value.icon"></i>
            </div>
            
            <!-- 标题 -->
            <h3 class="platform-value-card-title">{{ value.title }}</h3>
            
            <!-- 描述 -->
            <p class="platform-value-card-description">{{ value.description }}</p>
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
/* 样式已移至 assets/css/home.css */
/* 保留组件特有的样式（如果有） */
</style>

