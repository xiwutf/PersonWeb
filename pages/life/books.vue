<template>
  <div class="life-books">
    <header class="life-books-header">
    <div class="life-books-toolbar">
      <div class="life-books-views" role="tablist" aria-label="笔记视图">
        <button type="button" role="tab" :aria-selected="view === 'book'" :class="{ 'is-active': view === 'book' }" @click="view = 'book'">按书</button>
        <button type="button" role="tab" :aria-selected="view === 'time'" :class="{ 'is-active': view === 'time' }" @click="view = 'time'">按时间</button>
      </div>
      <button type="button" class="life-books-refresh" :disabled="pending" @click="refresh">刷新</button>
    </div>
    </header>

    <div v-if="pending" class="life-books-status">正在加载笔记…</div>
    <div v-else-if="!items.length" class="life-notes-empty">
      <p>还没有阅读笔记。</p>
      <p>在书架里选中原文保存笔记后，会出现在这里。</p>
    </div>

    <template v-else-if="view === 'time'">
      <ul class="life-books-timeline">
        <li v-for="item in items" :key="item.id">
          <article class="life-books-card">
            <p class="life-books-card-meta">
              <time>{{ formatDate(item.createdAt) }}</time>
              <span :title="item.bookTitle">{{ displayBookTitle(item.bookTitle) }}</span>
            </p>
            <h2>{{ notePreview(item.note, 48) }}</h2>
            <p v-if="item.quote" class="life-books-quote">{{ notePreview(item.quote, 120) }}</p>
          </article>
        </li>
      </ul>
    </template>

    <template v-else>
      <section v-for="group in bookGroups" :key="group.bookId" class="life-books-group">
        <header class="life-books-group-head">
          <div>
            <h2 :title="group.bookTitle">{{ displayBookTitle(group.bookTitle, 36) }}</h2>
            <p v-if="group.bookAuthor">{{ notePreview(group.bookAuthor, 40) }}</p>
          </div>
          <span>{{ group.notes.length }} 条</span>
        </header>
        <ul class="life-books-timeline">
          <li v-for="item in group.notes" :key="item.id">
            <article class="life-books-card">
              <p class="life-books-card-meta"><time>{{ formatDate(item.createdAt) }}</time></p>
              <h2>{{ notePreview(item.note, 48) }}</h2>
              <p v-if="item.quote" class="life-books-quote">{{ notePreview(item.quote, 120) }}</p>
            </article>
          </li>
        </ul>
      </section>
    </template>
  </div>
</template>

<script setup lang="ts">
import {
  displayBookTitle,
  groupNotesByBook,
  notePreview,
  type ReadingNoteFeedItem,
} from '~/utils/library-companion'

definePageMeta({ layout: 'life' })

const view = ref<'book' | 'time'>('book')

const { data, pending, refresh } = await useAsyncData('life-book-notes', () =>
  $fetch<{ items: ReadingNoteFeedItem[] }>('/api/reading/book-notes', { cache: 'no-store' }),
)

const items = computed(() => (Array.isArray(data.value?.items) ? data.value!.items : []))
const bookGroups = computed(() => groupNotesByBook(items.value))

function formatDate(value: string) {
  const raw = value.endsWith('Z') || value.includes('+') ? value : `${value}Z`
  const date = new Date(raw)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleDateString('zh-CN', { year: 'numeric', month: 'long', day: 'numeric' })
}

usePageSeo({
  title: '读书 - 溪午听风',
  description: '阅读笔记，按书或按时间回看。',
  path: '/life/books',
  world: 'life',
})
</script>
