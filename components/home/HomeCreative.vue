<template>
  <div class="creative-home">
    <!-- 1. Hero Section: 极简巨型排版 -->
    <section class="creative-hero">
      <div class="hero-content">
        <!-- 装饰性背景文字 -->
        <div class="absolute top-20 right-0 opacity-5 pointer-events-none select-none">
          <span class="text-[20rem] font-bold leading-none">AI</span>
        </div>

        <h1 class="text-display mb-8">
          <span class="block">Digital</span>
          <span class="block text-transparent bg-clip-text bg-gradient-to-r from-primary to-secondary">
            Alchemist
          </span>
        </h1>

        <div class="hero-intro-grid">
          <div class="text-body-large">
            <p class="mb-6">
              我是溪午听风 (XIFG)，一名游走在代码与艺术边缘的数字炼金术士。
            </p>
            <p>
              这里不仅是博客，更是一个融合了 <span class="text-primary font-bold">AI 智能体</span>、<span class="text-secondary font-bold">全栈开发</span> 与 <span class="text-accent font-bold">创造力工具</span> 的数字实验场。
            </p>
          </div>
          <div class="flex flex-col gap-6 items-start lg:items-end">
            <div class="flex gap-4">
              <div class="flex -space-x-4">
                 <!-- 模拟头像组 -->
                 <div class="w-12 h-12 rounded-full border-2 border-bg-app bg-gray-300"></div>
                 <div class="w-12 h-12 rounded-full border-2 border-bg-app bg-gray-400"></div>
                 <div class="w-12 h-12 rounded-full border-2 border-bg-app bg-gray-500 flex items-center justify-center text-xs font-bold text-white">+50</div>
              </div>
              <div class="flex flex-col justify-center">
                <span class="font-bold text-lg">50+</span>
                <span class="text-xs text-text-tertiary">Projects Delivered</span>
              </div>
            </div>
            <button class="btn-pill" @click="scrollToWork">
              Explore Work <i class="fas fa-arrow-down ml-2"></i>
            </button>
          </div>
        </div>

        <div class="scroll-indicator">
          <span class="text-xs tracking-[0.2em] uppercase">Scroll</span>
          <div class="scroll-line"></div>
        </div>
      </div>
    </section>

    <!-- 2. Marquee: 动态技能展示 -->
    <div class="creative-marquee">
      <div class="marquee-content">
        <!-- 重复两次以实现无缝滚动 -->
        <span class="marquee-item" v-for="i in 4" :key="i">
          Full Stack Dev <span class="marquee-star">✦</span>
          AI Agent Design <span class="marquee-star">✦</span>
          System Architecture <span class="marquee-star">✦</span>
          UI/UX Design <span class="marquee-star">✦</span>
        </span>
      </div>
    </div>

    <!-- 3. Stacking Cards: 核心板块堆叠展示 -->
    <div class="stack-container" id="work-section">
      
      <!-- Card 1: AI Laboratory -->
      <article class="stack-card card-bg-1">
        <div class="stack-card-inner">
          <div class="stack-text">
            <div class="stack-tags">
              <span class="stack-tag">Intelligence</span>
              <span class="stack-tag">RAG</span>
              <span class="stack-tag">Agents</span>
            </div>
            <h2 class="text-title">AI Laboratory</h2>
            <p class="text-body-large">
              探索大语言模型的无限可能。从 RAG 知识库问答到自主 Agent 工作流，我正在构建能真正解决问题的 AI 应用。
            </p>
            <div>
              <NuxtLink to="/ai" class="btn-pill">
                Enter Lab
              </NuxtLink>
            </div>
          </div>
          <div class="stack-visual bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center group">
            <!-- 示意图：实际项目中替换为真实图片 -->
            <i class="fas fa-brain text-9xl text-white opacity-50 group-hover:scale-110 transition-transform duration-500"></i>
          </div>
        </div>
      </article>

      <!-- Card 2: Projects (反向布局) -->
      <article class="stack-card card-bg-2 layout-reverse">
        <div class="stack-card-inner">
          <div class="stack-text">
            <div class="stack-tags">
              <span class="stack-tag">SaaS</span>
              <span class="stack-tag">Tools</span>
              <span class="stack-tag">Open Source</span>
            </div>
            <h2 class="text-title">Selected Projects</h2>
            <p class="text-body-large">
              代码是我的画笔。这里展示了我独立开发或参与的核心项目，包括 Revit 插件工具链、效率工具 SaaS 和开源组件库。
            </p>
            <div>
              <NuxtLink to="/projects" class="btn-pill">
                View Portfolio
              </NuxtLink>
            </div>
          </div>
          <div class="stack-visual bg-gradient-to-br from-emerald-500 to-teal-600 flex items-center justify-center group">
             <i class="fas fa-layer-group text-9xl text-white opacity-50 group-hover:scale-110 transition-transform duration-500"></i>
          </div>
        </div>
      </article>

      <!-- Card 3: Blog & Thoughts -->
      <article class="stack-card card-bg-1">
        <div class="stack-card-inner">
          <div class="stack-text">
            <div class="stack-tags">
              <span class="stack-tag">Writing</span>
              <span class="stack-tag">Thinking</span>
              <span class="stack-tag">Sharing</span>
            </div>
            <h2 class="text-title">Insights & Thoughts</h2>
            <p class="text-body-large">
              技术不仅是代码，更是思考的结晶。我在这里记录技术通过、架构复盘以及对 AI 时代的观察与思考。
            </p>
            <div>
              <NuxtLink to="/blog" class="btn-pill">
                Read Articles
              </NuxtLink>
            </div>
          </div>
          <div class="stack-visual bg-gradient-to-br from-orange-400 to-rose-500 flex items-center justify-center group">
             <i class="fas fa-pen-nib text-9xl text-white opacity-50 group-hover:scale-110 transition-transform duration-500"></i>
          </div>
        </div>
      </article>

    </div>

    <!-- 4. Footer Call to Action -->
    <section class="py-32 px-4 text-center bg-bg-app relative z-10">
      <h2 class="text-title mb-8">Ready to create something amazing?</h2>
      <div class="flex gap-4 justify-center">
        <NuxtLink to="/about" class="btn-pill bg-transparent border-2 border-text-primary !text-text-primary hover:!bg-text-primary hover:!text-bg-app">
          About Me
        </NuxtLink>
        <NuxtLink to="/contact" class="btn-pill">
          Get in Touch
        </NuxtLink>
      </div>
    </section>

  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'

const scrollToWork = () => {
  const el = document.getElementById('work-section')
  if (el) {
    el.scrollIntoView({ behavior: 'smooth' })
  }
}

// 简单的视差效果增强（可选）
onMounted(() => {
  // 如果需要更复杂的动画，可以在这里引入 GSAP
})
</script>

<style scoped>
@import url('~/assets/css/home-creative.css');
</style>
