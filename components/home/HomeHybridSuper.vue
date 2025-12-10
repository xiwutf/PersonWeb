<template>
  <div class="home-hybrid-super">
    <!-- 全局背景装饰 -->
    <div class="global-ambient-bg">
      <div class="ambient-glow glow-1"></div>
      <div class="ambient-glow glow-2"></div>
      <div class="ambient-grid"></div>
    </div>

    <!-- 1. Super Hero（平台定位 + 个人介绍） -->
    <HeroSuper />
    
    <!-- 2. Bento Grid V4（核心功能导航 - 紧随 Hero 提供视觉索引） -->
    <div class="section-wrapper bento-wrapper">
      <BentoGridV4 />
    </div>
    
    <!-- 3. AI 能力与智能体服务区块（核心特色展示） -->
    <div class="section-wrapper ai-capability-wrapper">
      <AiCapabilitySection />
    </div>

    <!-- 4. AI Playground 预览区（互动体验） -->
    <div class="section-wrapper playground-wrapper">
      <AIPlaygroundPreview />
    </div>
    
    <!-- 5. 精选内容区（精选工具 / 项目 / 文章） -->
    <div class="section-wrapper featured-wrapper">
      <FeaturedSection />
    </div>
    
    <!-- 6. TimelineSuper（个人成长时间线） -->
    <div class="section-wrapper timeline-wrapper">
      <TimelineSuper />
    </div>
    
    <!-- 7. 作品展示墙（沉浸式结尾） -->
    <div class="section-wrapper showcase-wrapper">
      <ShowcaseWall />
    </div>
    
    <!-- 8. Roadmap (放在最后作为未来展望) -->
    <RoadmapSection />
  </div>
</template>

<script setup lang="ts">
import HeroSuper from './HeroSuper.vue'
import AiCapabilitySection from '~/components/ai/AiCapabilitySection.vue'
// PlatformValueSection 已移除，融入整体叙事
import FeaturedSection from './FeaturedSection.vue'
import AIPlaygroundPreview from './AIPlaygroundPreview.vue'
import BentoGridV4 from './BentoGridV4.vue'
import ShowcaseWall from './ShowcaseWall.vue'
import RoadmapSection from './RoadmapSection.vue'
import TimelineSuper from './TimelineSuper.vue'

// 混合超级风格首页主组件 V2
// 布局逻辑重构：Hero -> 导航(Bento) -> 特色(AI) -> 互动(Playground) -> 内容(Featured) -> 故事(Timeline)
</script>

<style scoped>
.home-hybrid-super {
  background: var(--bg-app);
  position: relative;
  overflow: hidden;
}

/* 全局背景装饰 */
.global-ambient-bg {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
  overflow: hidden;
}

.ambient-glow {
  position: absolute;
  border-radius: 50%;
  filter: blur(120px);
  opacity: 0.4;
  animation: float-glow 20s infinite alternate;
}

.glow-1 {
  top: 10%;
  left: -10%;
  width: 50vw;
  height: 50vw;
  background: radial-gradient(circle, var(--primary) 0%, transparent 70%);
  opacity: 0.15;
}

.glow-2 {
  bottom: 20%;
  right: -10%;
  width: 60vw;
  height: 60vw;
  background: radial-gradient(circle, var(--secondary) 0%, transparent 70%);
  opacity: 0.1;
  animation-delay: -10s;
}

.ambient-grid {
  position: absolute;
  inset: 0;
  background-image: 
    linear-gradient(rgba(128, 128, 128, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(128, 128, 128, 0.03) 1px, transparent 1px);
  background-size: 100px 100px;
  mask-image: linear-gradient(to bottom, transparent 0%, black 20%, black 80%, transparent 100%);
  z-index: -1;
}

@keyframes float-glow {
  0% { transform: translate(0, 0) scale(1); }
  100% { transform: translate(5%, 10%) scale(1.1); }
}

/* Section Wrapper 用于控制层级和间距 */
.section-wrapper {
  position: relative;
  z-index: 1;
}

/* 针对不同板块的微调 */
.bento-wrapper {
  margin-top: -2rem; /* 稍微向上偏移，衔接 Hero */
}

.ai-capability-wrapper {
  /* 使用深色背景区分 */
  background: linear-gradient(to bottom, transparent, rgba(var(--primary-rgb, 0,0,0), 0.02), transparent);
}

.playground-wrapper {
  padding: 4rem 0;
}
</style>

