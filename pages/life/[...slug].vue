<template>
  <article class="life-note-article">
    <NuxtLink to="/life/notes" class="life-note-back">← 返回随笔</NuxtLink>

    <header class="life-note-masthead">
      <p class="life-note-kicker">NOTE / {{ post.category || '随笔' }}</p>
      <p class="life-note-meta">
        <time>{{ formatDate(post.date) }}</time>
      </p>
      <h1>{{ post.title }}</h1>
      <p v-if="post.description" class="life-note-dek">{{ post.description }}</p>
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

    <div v-if="post.cover" class="life-note-cover">
      <img :src="post.cover" :alt="post.title" />
    </div>

    <div class="life-note-body" v-html="renderedContent"></div>

    <div v-if="post.tags?.length" class="life-note-tags">
      <span v-for="tag in post.tags" :key="tag">#{{ tag }}</span>
    </div>
  </article>
</template>

<script setup lang="ts">
import { isLifeNoteSlug } from '~/constants/life-content'
import { usePageSeo, useJsonLd } from '~/composables/usePageSeo'

definePageMeta({
  layout: 'life'
})

const route = useRoute()
const slug = route.params.slug
const { parse } = useMarkdown()

const slugString = Array.isArray(slug) ? slug[0] : String(slug || '')

if (!isLifeNoteSlug(slugString)) {
  throw createError({ statusCode: 404, statusMessage: 'Not Found' })
}

const { data: post } = await useAsyncData(`life-${slugString}`, async () => {
  try {
    return await $fetch(`/api/content/life/${slugString}`)
  } catch (error: unknown) {
    const status = typeof error === 'object' && error !== null
      ? Number((error as { statusCode?: number, status?: number }).statusCode
        ?? (error as { statusCode?: number, status?: number }).status
        ?? 0)
      : 0
    if (status === 404) return null
    throw error
  }
})

const stripHtml = (value: string) => value.replace(/<[^>]+>/g, '').trim()

const enhanceLifeNoteHtml = (html: string) => {
  const titles = [...html.matchAll(/<h3\b[^>]*>([\s\S]*?)<\/h3>/gi)]
    .map(match => stripHtml(match[1]))
    .filter(Boolean)

  let enhanced = html
  if (titles.length >= 3) {
    const toc = `<ol class="life-note-toc">${titles.map((title, index) => {
      const n = String(index + 1).padStart(2, '0')
      const label = title.replace(/^第[一二三四五六七八九十]+层：/, '')
      return `<li><span>${n}</span>${label}</li>`
    }).join('')}</ol>`
    enhanced = enhanced.replace(/<\/h2>/i, `</h2>${toc}`)
  }

  const headings = [...enhanced.matchAll(/<h3\b[^>]*>[\s\S]*?<\/h3>/gi)]
  if (!headings.length) return enhanced

  let result = ''
  let cursor = 0
  headings.forEach((match, index) => {
    const start = match.index ?? 0
    result += enhanced.slice(cursor, start)
    const next = index + 1 < headings.length
      ? (headings[index + 1].index ?? enhanced.length)
      : enhanced.length
    result += `<section class="life-note-layer">${enhanced.slice(start, next)}</section>`
    cursor = next
  })
  return result + enhanced.slice(cursor)
}

const renderedContent = computed(() => enhanceLifeNoteHtml(parse(post.value?.content || '')))

if (!post.value) {
  throw createError({ statusCode: 404, statusMessage: 'Not Found' })
}

const formatDate = (dateString?: string) => {
  if (!dateString) return ''

  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

usePageSeo(() => ({
  title: `${post.value.title} - 溪午听风 · Life`,
  description: post.value.description || '溪午听风的一篇生活随笔。',
  path: `/life/${slugString}`,
  image: post.value.cover || null,
  type: 'article',
  world: 'life',
}))

useJsonLd(() => {
  if (!post.value) return null
  return {
    '@context': 'https://schema.org',
    '@type': 'Article',
    headline: post.value.title,
    description: post.value.description || undefined,
    datePublished: post.value.date || undefined,
    image: post.value.cover || undefined,
    author: { '@type': 'Person', name: '溪午听风' },
  }
})
</script>
