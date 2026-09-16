<template>
  <div class="life-thoughts-detail">
    <header class="life-thoughts-detail-head">
      <div class="life-thoughts-detail-heading">
        <div>
          <p class="life-thoughts-kicker">08</p>
          <h1>来自网页</h1>
          <p class="life-thoughts-detail-desc">
            从 MindTrace 主动发布、并在 Inbox 里公开的摘录。
          </p>
          <p class="life-thoughts-detail-count">{{ items.length }} 则</p>
        </div>
      </div>
    </header>

    <p v-if="pending" class="life-empty">加载中…</p>
    <p v-else-if="!items.length" class="life-empty">还没有公开的网页摘录。</p>

    <div v-else class="life-thoughts-detail-list">
      <LifeThoughtListItem
        v-for="item in items"
        :key="item.id"
        :item="item"
      />
    </div>

    <p class="life-about-back">
      <NuxtLink to="/life/thoughts" class="life-note-back">← 返回想法精选</NuxtLink>
    </p>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'life',
})

type PublicReadingItem = {
  id: string
  text: string
  note?: string
  source?: string
  sourceUrl?: string
  createdAt?: string
}

const { data, pending } = await useAsyncData('life-thoughts-reading', () =>
  $fetch<{ items: PublicReadingItem[] }>('/api/reading'),
)

const items = computed(() => data.value?.items || [])

usePageSeo(() => ({
  title: '来自网页 - 溪午听风 · Life',
  description: '从 MindTrace 主动发布并公开的网页摘录。',
  path: '/life/thoughts/reading',
  world: 'life',
}))
</script>
