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

type PublicReadingItem = {
  id: string
  text: string
  note?: string
}

const defaultDescription = '把一些反复想起的话，\n按主题慢慢收起来。'

const { data, pending: thoughtsPending } = await useAsyncData('life-thoughts-index', () =>
  $fetch<ThoughtsIndexResponse>('/api/content/life/thoughts'),
)

const { data: readingData, pending: readingPending } = await useAsyncData(
  'life-thoughts-reading-preview',
  () => $fetch<{ items: PublicReadingItem[] }>('/api/reading'),
)

const page = computed(() => data.value)
const pending = computed(() => thoughtsPending.value || readingPending.value)

const webSection = computed((): ThoughtSection | null => {
  const items = readingData.value?.items || []
  const featured = items[0]
    ? { id: items[0].id, text: items[0].text, note: items[0].note }
    : null
  const recommendations = items.slice(1, 3).map(item => ({
    id: item.id,
    text: item.text,
  }))

  return {
    slug: 'reading',
    index: '08',
    title: '来自网页',
    description: 'MindTrace 发布并公开的网页摘录。',
    visualType: 'text-only',
    total: items.length,
    featured,
    recommendations,
  }
})

const sections = computed(() => {
  const base = data.value?.sections || []
  if (!webSection.value) {
    return base
  }
  return [...base, webSection.value]
})

usePageSeo(() => ({
  title: `${page.value?.title || '生活里的想法'} - 溪午听风 · Life`,
  description: (page.value?.description || defaultDescription).replace(/\n/g, ''),
  path: '/life/thoughts',
  world: 'life',
}))
</script>
