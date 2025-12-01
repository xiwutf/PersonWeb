<template>
  <div class="home-hybrid-super min-h-screen bg-bg-body text-text-main overflow-x-hidden">
    <!-- 调试信息（开发环境） -->
    <div v-if="isDev" class="fixed top-20 right-4 z-[9999] bg-red-500/90 text-white p-2 rounded text-xs">
      组件已加载: HomeHybridSuper V2
    </div>
    
    <!-- 1. Super Hero（平台定位 + 个人介绍） -->
    <HeroSuper />
    
    <!-- 2. 平台价值主张区 -->
    <PlatformValueSection />
    
    <!-- 3. 平台入口 Hub（Blog / Projects / Tools / AI Lab） -->
    <PlatformEntryHub />
    
    <!-- 4. 精选内容区（精选工具 / 项目 / 文章） -->
    <FeaturedSection />
    
    <!-- 5. AI Playground 预览区（可交互小块） -->
    <AIPlaygroundPreview />
    
    <!-- 6. Bento Grid V4（按功能分类展示） -->
    <BentoGridV4 />
    
    <!-- 7. Roadmap / What's New（平台发展路线） -->
    <RoadmapSection />
    
    <!-- 8. TimelineSuper（个人成长时间线，位置靠下） -->
    <TimelineSuper />
    
    <!-- 9. 增强版 Footer -->
    <FooterPro />
  </div>
</template>

<script setup lang="ts">
import HeroSuper from './HeroSuper.vue'
import PlatformValueSection from './PlatformValueSection.vue'
import PlatformEntryHub from './PlatformEntryHub.vue'
import FeaturedSection from './FeaturedSection.vue'
import AIPlaygroundPreview from './AIPlaygroundPreview.vue'
import BentoGridV4 from './BentoGridV4.vue'
import RoadmapSection from './RoadmapSection.vue'
import TimelineSuper from './TimelineSuper.vue'
import FooterPro from '~/components/common/FooterPro.vue'

// 混合超级风格首页主组件 V2
// 从"炫酷个人主页"升级为"个人 × 平台混合的首页"
// 整合所有新组件，实现平台化的创作空间展示
// 注意：首页风格（home style）和全局主题（theme）是分开的
// 这个组件使用自己的样式系统，不依赖全局主题

// 检查是否为开发环境
const isDev = import.meta.env.DEV
</script>

<style scoped>
.home-hybrid-super {
  background: var(--color-bg-body);
}
</style>

