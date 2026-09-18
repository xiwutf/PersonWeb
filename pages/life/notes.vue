<template>
  <div class="life-notes">
    <NuxtLink to="/life" class="life-note-back">← 返回 Life</NuxtLink>

    <header class="life-note-masthead">
      <p class="life-note-kicker">NOTE / 随笔</p>
      <h1>随笔</h1>
      <p class="life-note-dek">想到什么，就写一点下来。</p>
      <p v-if="notes.length" class="life-note-meta">{{ notes.length }} 篇</p>
      <span class="life-title-rule">
        <span></span>
        <LifeIcon name="pencil" />
      </span>
      <svg class="life-note-seal" viewBox="0 0 88 88" aria-hidden="true">
        <circle cx="44" cy="44" r="40" />
        <circle cx="44" cy="44" r="34" />
        <path d="M32 58c10-4 16-14 18-28-12 2-20 10-22 24Z" />
        <path d="M44 32c6-8 14-12 22-8" />
        <path d="M36 48c8-6 14-10 22-12" />
      </svg>
    </header>

    <div v-if="!notes.length" class="life-notes-empty">
      <p>这里还没写东西。</p>
      <p>以后想到什么，再慢慢放进来。</p>
    </div>

    <template v-else>
      <NuxtLink
        v-if="featured"
        :to="featured._path"
        class="life-notes-feature"
      >
        <p class="life-notes-feature-meta">
          <time>{{ formatDate(featured.date) }}</time>
          <template v-if="featured.category"> · {{ featured.category }}</template>
        </p>
        <h2>{{ featured.title }}</h2>
        <p v-if="featured.description">{{ featured.description }}</p>
        <div v-if="featured.tags?.length" class="life-note-tags">
          <span v-for="tag in featured.tags" :key="tag">#{{ tag }}</span>
        </div>
        <span class="life-notes-feature-go">读下去 →</span>
      </NuxtLink>

      <div v-if="rest.length" class="life-note-list">
        <NuxtLink
          v-for="(post, index) in rest"
          :key="post._path"
          :to="post._path"
          class="life-note-item"
        >
          <span class="life-note-item-index">{{ padIndex(index + 2) }}</span>
          <div class="life-note-item-body">
            <time>{{ formatDate(post.date) }}</time>
            <h2>{{ post.title }}</h2>
            <p v-if="post.description">{{ post.description }}</p>
          </div>
        </NuxtLink>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'life'
})

type LifeNote = {
  _path: string
  title?: string
  description?: string
  date?: string
  category?: string
  tags?: string[]
}

const { data: posts } = await useAsyncData('life-notes-archive', () =>
  $fetch<LifeNote[]>('/api/content/life', { cache: 'no-store' })
)

const notes = computed(() => (Array.isArray(posts.value) ? posts.value : []))
const featured = computed(() => notes.value[0] || null)
const rest = computed(() => notes.value.slice(1))

const padIndex = (value: number) => String(value).padStart(2, '0')

const formatDate = (dateString?: string) => {
  if (!dateString) return ''

  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

usePageSeo({
  title: '随笔 - 溪午听风',
  description: '溪午听风写下的生活随笔。',
  path: '/life/notes',
  world: 'life',
})
</script>
