<template>
  <div class="life-thoughts-detail">
    <AdminEditBanner />

    <p v-if="pending && !page" class="life-empty">加载中…</p>

    <template v-else-if="page">
      <header class="life-thoughts-detail-head">
        <p class="life-thoughts-kicker">{{ page.category.index }}</p>
        <h1>{{ page.category.title }}</h1>
        <p v-if="page.category.description" class="life-thoughts-detail-desc">
          {{ page.category.description }}
        </p>
        <p class="life-thoughts-detail-count">{{ page.items.length }} 则</p>
      </header>

      <p v-if="!page.items.length" class="life-empty">这个分类还没有句子。</p>

      <div v-else class="life-thoughts-detail-list">
        <LifeThoughtListItem
          v-for="item in page.items"
          :key="item.id"
          :item="item"
        />
      </div>
    </template>

    <p class="life-about-back">
      <NuxtLink to="/life/thoughts" class="life-note-back">← 返回想法精选</NuxtLink>
    </p>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'life',
})

type CategoryPage = {
  category: {
    slug: string
    index: string
    title: string
    description: string
  }
  items: Array<{
    id: string
    text: string
    note?: string
    source?: string
    createdAt?: string
    featured?: boolean
  }>
}

const route = useRoute()
const slug = computed(() => String(route.params.category || ''))

const { data: page, pending, error } = await useAsyncData(
  `life-thoughts-${slug.value}`,
  async () => {
    try {
      return await $fetch<CategoryPage>(`/api/content/life/thoughts/${slug.value}`)
    }
    catch (err: unknown) {
      const status = typeof err === 'object' && err && 'statusCode' in err
        ? Number((err as { statusCode?: number }).statusCode)
        : 0
      if (status === 404) {
        throw createError({ statusCode: 404, statusMessage: '分类不存在' })
      }
      throw err
    }
  },
  { watch: [slug] },
)

if (error.value) {
  throw createError({
    statusCode: (error.value as { statusCode?: number }).statusCode || 500,
    statusMessage: error.value.message || '加载失败',
  })
}

usePageSeo(() => ({
  title: `${page.value?.category.title || '想法'} - 溪午听风 · Life`,
  description: page.value?.category.description || '生活里的想法归档。',
  path: `/life/thoughts/${slug.value}`,
  world: 'life',
}))
</script>
