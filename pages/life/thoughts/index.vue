<template>
  <div class="life-thoughts">
    <AdminEditBanner />

    <LifeThoughtsHero
      :title="page?.title || '生活里的想法'"
      :description="page?.description || defaultDescription"
    />

    <p v-if="pending" class="life-empty">加载中…</p>
    <p v-else-if="!sections.length" class="life-empty">还没有想法分类。</p>

    <div v-else class="life-thoughts-sections">
      <LifeThoughtSectionCard
        v-for="section in sections"
        :key="section.slug"
        :section="section"
      />
    </div>

    <p class="life-about-back">
      <NuxtLink to="/life" class="life-note-back">← 返回 Life</NuxtLink>
    </p>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'life',
})

type ThoughtSection = {
  slug: string
  index: string
  title: string
  description: string
  cover?: string
  visualType: 'photo' | 'illustration' | 'text-only'
  total: number
  featured: { id: string, text: string, note?: string } | null
  recommendations: Array<{ id: string, text: string }>
}

type ThoughtsIndexResponse = {
  title: string
  description: string
  sections: ThoughtSection[]
}

const defaultDescription = '把一些反复想起的话，\n按主题慢慢收起来。'

const { data, pending } = await useAsyncData('life-thoughts-index', () =>
  $fetch<ThoughtsIndexResponse>('/api/content/life/thoughts'),
)

const page = computed(() => data.value)
const sections = computed(() => data.value?.sections || [])

usePageSeo(() => ({
  title: `${page.value?.title || '生活里的想法'} - 溪午听风 · Life`,
  description: (page.value?.description || defaultDescription).replace(/\n/g, ''),
  path: '/life/thoughts',
  world: 'life',
}))
</script>
