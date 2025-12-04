<template>
  <section class="platform-entry-hub">
    <div class="platform-entry-hub-container">
      <div class="platform-entry-hub-header">
        <h2 class="platform-entry-hub-title">平台入口 Hub</h2>
        <p class="platform-entry-hub-subtitle">
          探索不同的数字空间，每个入口都是一个独立的功能模块
        </p>
      </div>

      <div class="platform-entry-hub-grid">
        <NuxtLink
          v-for="(entry, index) in entries"
          :key="index"
          :to="entry.path"
          class="platform-entry-card"
          :ref="el => { if (el) cardRefs[index] = el as HTMLElement }"
        >
          <div class="platform-entry-card-inner">
            <!-- 背景光效 -->
            <div class="platform-entry-card-glow" :class="entry.glowBg"></div>
            
            <!-- 内容 -->
            <div class="platform-entry-card-content">
              <div class="platform-entry-card-icon-wrapper">
                <div class="platform-entry-card-icon" :class="entry.iconBg">
                  <i :class="entry.icon"></i>
                </div>
              </div>
              
              <h3 class="platform-entry-card-title">{{ entry.title }}</h3>
              <p class="platform-entry-card-description">{{ entry.description }}</p>
              
              <!-- Tag 区域 -->
              <div class="platform-entry-card-tags">
                <span
                  v-for="tag in entry.tags"
                  :key="tag"
                  class="platform-entry-card-tag"
                >
                  {{ tag }}
                </span>
              </div>
              
              <div class="platform-entry-card-action">
                <span>进入</span>
                <i class="fas fa-arrow-right platform-entry-card-action-icon"></i>
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
  },
  {
    title: 'Side Projects',
    description: '副业项目展示、能力证明',
    icon: 'fas fa-briefcase',
    iconBg: 'bg-gradient-to-br from-amber-500 to-yellow-500',
    glowBg: 'bg-gradient-to-br from-amber-500/20 to-yellow-500/20',
    tags: ['Freelance', 'Projects'],
    path: '/side-projects'
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
/* 样式已移至 assets/css/home.css */
/* 保留组件特有的样式（如果有） */
</style>

