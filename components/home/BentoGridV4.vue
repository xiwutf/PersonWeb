<template>
  <section class="bento-grid-v4">
    <div class="bento-grid-v4-container">
      <div class="bento-grid-v4-header" ref="sectionTitleRef">
        <h2 class="bento-grid-v4-title">按功能分类展示</h2>
        <p class="bento-grid-v4-subtitle">探索不同类别的创作内容</p>
      </div>

      <div class="bento-grid-v4-grid">
        <!-- AI Agents / AI 实验 -->
        <BentoCardSection
          title="AI Agents"
          subtitle="AI 实验"
          icon="fas fa-robot"
          iconBg="bg-gradient-to-br from-red-500/20 to-orange-500/20 text-red-400"
          :items="aiItems"
          :ref="el => { if (el) sectionRefs[0] = el as HTMLElement }"
        />

        <!-- Mini Apps / Tools -->
        <BentoCardSection
          title="Mini Apps"
          subtitle="Tools"
          icon="fas fa-toolbox"
          iconBg="bg-gradient-to-br from-emerald-500/20 to-teal-500/20 text-emerald-400"
          :items="toolItems"
          :ref="el => { if (el) sectionRefs[1] = el as HTMLElement }"
        />

        <!-- Articles / Blog -->
        <BentoCardSection
          title="Articles"
          subtitle="Blog"
          icon="fas fa-pen-fancy"
          iconBg="bg-gradient-to-br from-blue-500/20 to-cyan-500/20 text-blue-400"
          :items="articleItems"
          :ref="el => { if (el) sectionRefs[2] = el as HTMLElement }"
        />

        <!-- Projects -->
        <BentoCardSection
          title="Projects"
          subtitle="作品集"
          icon="fas fa-project-diagram"
          iconBg="bg-gradient-to-br from-purple-500/20 to-pink-500/20 text-purple-400"
          :items="projectItems"
          :ref="el => { if (el) sectionRefs[3] = el as HTMLElement }"
        />
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { animate, inView, stagger } from '@motionone/dom'
import BentoCardSection from './BentoCardSection.vue'
import BentoCardItem from './BentoCardItem.vue'

const sectionTitleRef = ref<HTMLElement | null>(null)
const sectionRefs = ref<(HTMLElement | null)[]>([])

// 暂时使用静态 mock 数据，后续可接入后端 API
const aiItems = [
  { id: 1, title: 'AI 对话助手', type: 'AI', link: '/lab', tags: ['AI', 'Agent'] },
  { id: 2, title: 'Prompt 工程', type: 'AI', link: '/lab', tags: ['AI', 'Prompt'] }
]

const toolItems = [
  { id: 1, title: '在线工具 1', type: 'Tool', link: '/tools', tags: ['Tools'] },
  { id: 2, title: '在线工具 2', type: 'Tool', link: '/tools', tags: ['Scripts'] }
]

const articleItems = [
  { id: 1, title: '技术文章 1', type: 'Article', link: '/blog', tags: ['Tech'] },
  { id: 2, title: '技术文章 2', type: 'Article', link: '/blog', tags: ['AI'] }
]

const projectItems = [
  { id: 1, title: '项目 1', type: 'Project', link: '/projects', tags: ['Dev'] },
  { id: 2, title: '项目 2', type: 'Project', link: '/projects', tags: ['Product'] }
]

onMounted(() => {
  if (sectionTitleRef.value) {
    inView(sectionTitleRef.value, () => {
      animate(
        sectionTitleRef.value!,
        { opacity: [0, 1], y: [30, 0] },
        { duration: 0.6, easing: 'ease-out' }
      )
    })
  }

  const validRefs = sectionRefs.value.filter(Boolean) as HTMLElement[]
  if (validRefs.length > 0) {
    inView(validRefs[0], () => {
      animate(
        validRefs,
        { opacity: [0, 1], y: [50, 0], scale: [0.95, 1] },
        { duration: 0.6, delay: stagger(0.1), easing: 'ease-out' }
      )
    })
  }
})
</script>

<style scoped>
.bento-grid-v4 {
  background-color: var(--color-bg-body);
}
</style>

