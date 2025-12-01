<template>
  <section class="platform-cards py-24 relative">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12">
      <div class="text-center mb-16">
        <h2 class="text-4xl lg:text-5xl font-bold text-white mb-4">平台入口</h2>
        <p class="text-lg text-white/60">探索不同的数字空间</p>
      </div>

      <div class="grid md:grid-cols-3 gap-8">
        <!-- 博客星球 -->
        <NuxtLink
          to="/blog"
          class="platform-card group"
          ref="blogCardRef"
        >
          <div class="card-inner">
            <!-- 背景光效 -->
            <div class="card-glow bg-gradient-to-br from-blue-500/20 to-cyan-500/20"></div>
            
            <!-- 内容 -->
            <div class="card-content">
              <div class="card-icon-wrapper">
                <div class="card-icon bg-gradient-to-br from-blue-500 to-cyan-500">
                  <i class="fas fa-globe text-3xl"></i>
                </div>
              </div>
              
              <h3 class="card-title">博客星球</h3>
              <p class="card-description">
                围绕技术、思考与实践的记录。点击地球可以跳转至博客列表。
              </p>
              
              <div class="card-action">
                <span>进入博客</span>
                <i class="fas fa-arrow-right ml-2 group-hover:translate-x-1 transition-transform"></i>
              </div>
            </div>
          </div>
        </NuxtLink>

        <!-- 项目飞船 -->
        <NuxtLink
          to="/projects"
          class="platform-card group"
          ref="projectCardRef"
        >
          <div class="card-inner">
            <!-- 背景光效 -->
            <div class="card-glow bg-gradient-to-br from-purple-500/20 to-pink-500/20"></div>
            
            <!-- 内容 -->
            <div class="card-content">
              <div class="card-icon-wrapper">
                <div class="card-icon bg-gradient-to-br from-purple-500 to-pink-500">
                  <i class="fas fa-rocket text-3xl"></i>
                </div>
              </div>
              
              <h3 class="card-title">项目飞船</h3>
              <p class="card-description">
                实战项目与开源尝试。点击飞船进入项目作品集。
              </p>
              
              <div class="card-action">
                <span>查看项目</span>
                <i class="fas fa-arrow-right ml-2 group-hover:translate-x-1 transition-transform"></i>
              </div>
            </div>
          </div>
        </NuxtLink>

        <!-- 数据星球 -->
        <NuxtLink
          to="/dashboard"
          class="platform-card group"
          ref="dataCardRef"
        >
          <div class="card-inner">
            <!-- 背景光效 -->
            <div class="card-glow bg-gradient-to-br from-green-500/20 to-emerald-500/20"></div>
            
            <!-- 内容 -->
            <div class="card-content">
              <div class="card-icon-wrapper">
                <div class="card-icon bg-gradient-to-br from-green-500 to-emerald-500">
                  <i class="fas fa-chart-line text-3xl"></i>
                </div>
              </div>
              
              <h3 class="card-title">数据星球</h3>
              <p class="card-description">
                展示站点访问、文章数据等指标。后续可以接入更多可视化组件。
              </p>
              
              <div class="card-action">
                <span>打开仪表盘</span>
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
import { animate, stagger } from '@motionone/dom'

const blogCardRef = ref<HTMLElement | null>(null)
const projectCardRef = ref<HTMLElement | null>(null)
const dataCardRef = ref<HTMLElement | null>(null)

onMounted(() => {
  const cards = [blogCardRef.value, projectCardRef.value, dataCardRef.value].filter(Boolean) as HTMLElement[]
  
  if (cards.length > 0) {
    animate(
      cards,
      { opacity: [0, 1], y: [50, 0], scale: [0.9, 1] },
      { duration: 0.6, delay: stagger(0.15), easing: 'ease-out' }
    )
  }
})
</script>

<style scoped>
.platform-cards {
  background: linear-gradient(to bottom, #000000, #0a0a0a);
}

.platform-card {
  position: relative;
  display: block;
  height: 100%;
  min-height: 400px;
}

.card-inner {
  position: relative;
  height: 100%;
  background: rgba(20, 20, 30, 0.6);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1.5rem;
  padding: 2.5rem;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
}

.platform-card:hover .card-inner {
  transform: translateY(-8px);
  border-color: rgba(255, 255, 255, 0.2);
  box-shadow: 0 20px 60px rgba(0, 217, 255, 0.2);
}

.card-glow {
  position: absolute;
  inset: -2px;
  border-radius: 1.5rem;
  opacity: 0;
  transition: opacity 0.4s ease;
  z-index: 0;
}

.platform-card:hover .card-glow {
  opacity: 1;
}

.card-content {
  position: relative;
  z-index: 1;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.card-icon-wrapper {
  margin-bottom: 2rem;
}

.card-icon {
  width: 5rem;
  height: 5rem;
  border-radius: 1.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.3);
  transition: transform 0.4s ease;
}

.platform-card:hover .card-icon {
  transform: scale(1.1) rotate(5deg);
}

.card-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: white;
  margin-bottom: 1rem;
  transition: color 0.3s ease;
}

.platform-card:hover .card-title {
  color: rgba(0, 217, 255, 1);
}

.card-description {
  color: rgba(255, 255, 255, 0.7);
  line-height: 1.6;
  flex-grow: 1;
  margin-bottom: 2rem;
}

.card-action {
  display: inline-flex;
  align-items: center;
  color: rgba(0, 217, 255, 1);
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.platform-card:hover .card-action {
  color: rgba(0, 217, 255, 1);
  transform: translateX(4px);
}
</style>

