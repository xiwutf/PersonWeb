<template>
  <div class="desktop-pet-page">
    <ProductHero />
    <FeatureGrid />
    <ScreenshotGallery />
    <PricingCompare />
    <ChangelogPreview />
    <ProductCTA />
  </div>
</template>

<script setup lang="ts">
import ProductHero from '~/components/products/ProductHero.vue'
import FeatureGrid from '~/components/products/FeatureGrid.vue'
import ScreenshotGallery from '~/components/products/ScreenshotGallery.vue'
import PricingCompare from '~/components/products/PricingCompare.vue'
import ChangelogPreview from '~/components/products/ChangelogPreview.vue'
import ProductCTA from '~/components/products/ProductCTA.vue'

useHead({
  title: 'AI 桌面陪伴助手 - 打工人的桌面效率搭子',
  meta: [
    {
      name: 'description',
      content: '一款面向打工人的桌面陪伴 + 专注 + AI 工具产品。陪你工作、提醒专注、轻度成长，让冰冷的电脑桌面变成有温度的工作搭子。'
    }
  ]
})
</script>

<style scoped>
.desktop-pet-page {
  min-height: 100vh;
}
</style>
