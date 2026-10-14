<template>
  <section class="library-shelf">
    <header class="library-shelf-header">
      <div class="library-shelf-heading"><span class="library-shelf-symbol" aria-hidden="true"><NIcon :component="BookOutline" /></span><div><h1>我的书架</h1><p>留一点时间，给下一页。</p></div></div>
      <div class="library-shelf-actions">
        <NButton type="primary" size="large" :loading="uploading" @click="fileInput?.click()"><template #icon><NIcon :component="AddOutline" /></template>{{ uploading ? (converting ? '转换 MOBI 中…' : '上传中…') : '上传书籍' }}</NButton>
        <NButton quaternary size="large" class="library-shelf-exit" @click="logout"><template #icon><NIcon :component="LogOutOutline" /></template>退出</NButton>
      </div>
      <input ref="fileInput" class="library-file-input" type="file" accept=".epub,.mobi,application/epub+zip,application/x-mobipocket-ebook" @change="upload" />
    </header>
    <div class="library-shelf-toolbar">
      <form class="library-search" @submit.prevent="load(1)"><NInput v-model:value="query" size="large" placeholder="搜索书名或作者" :maxlength="100" clearable :input-props="{ 'aria-label': '搜索书架' }"><template #prefix><NIcon :component="SearchOutline" /></template></NInput><NButton attr-type="submit" size="large" secondary>搜索</NButton></form>
      <span class="library-private-label"><NIcon :component="LockClosedOutline" />私人书房</span>
    </div>
    <NAlert v-if="error" type="error" role="alert">{{ error }} <NButton text @click="load(page)">重试</NButton></NAlert>
    <NAlert v-if="notice" type="success" class="library-shelf-notice" role="status" closable @close="notice = ''">{{ notice }}</NAlert>
    <NAlert v-if="uploading && converting" type="info" class="library-shelf-notice" role="status">正在上传并转换 MOBI，完成后自动加入书架。请稍候。</NAlert>
    <section v-if="recent" class="library-continue" aria-label="最近阅读">
      <div><span>最近阅读</span><h2>{{ recent.book.title }}</h2><p>已读 {{ Math.round(recent.progress.progression * 100) }}% · {{ readTime(recent.progress.lastReadAt) }}</p></div>
      <NuxtLink :to="`/library/read/${recent.book.id}`">继续阅读</NuxtLink>
    </section>
    <div class="library-shelf-section"><h2>书架<span v-if="!loading">{{ total }} 本</span></h2><span>点击封面，直接阅读</span></div>
    <div v-if="loading" class="library-shelf-loading"><NSpin description="正在加载书架…" /></div>
    <div v-else-if="!books.length" class="library-empty"><div class="library-empty-symbol" aria-hidden="true"><NIcon :component="BookOutline" /></div><h2>{{ query ? '没有找到这本书' : '书架还是空的' }}</h2><p>{{ query ? '试试其他书名或作者。' : '上传 EPUB 或 MOBI，从第一章开始。' }}</p><NButton v-if="!query" type="primary" size="large" :disabled="uploading" @click="fileInput?.click()">选择书籍</NButton></div>
    <div v-else class="library-book-grid">
      <NuxtLink v-for="book in books" :key="book.id" :to="`/library/read/${book.id}`" class="library-book" :aria-label="`阅读 ${book.title}`">
        <div class="library-book-art"><LibraryBookCover :book="book" /><span class="library-book-read"><NIcon :component="BookOutline" />打开阅读</span></div>
        <div class="library-book-caption"><h2 :title="book.title">{{ book.title }}</h2><p :title="book.author">{{ book.author || '作者未标注' }}</p>
          <span v-if="book.progression !== null" class="library-book-progress">已读 {{ Math.round(book.progression * 100) }}%<template v-if="book.lastReadAt"> · {{ readTime(book.lastReadAt) }}</template></span>
        </div>
      </NuxtLink>
    </div>
    <NPagination v-if="total > 24" :page="page" :page-size="24" :item-count="total" @update:page="load" />
    <footer class="library-shelf-footer"><span>你的书，只属于你。</span><span>支持 EPUB / MOBI · 单本不超过 100 MiB</span></footer>
  </section>
</template>

<script setup lang="ts">
import { NAlert, NButton, NIcon, NInput, NPagination, NSpin } from 'naive-ui'
import { AddOutline, BookOutline, LockClosedOutline, LogOutOutline, SearchOutline } from '@vicons/ionicons5'
import { libraryError, type LibraryBook, type LibraryResponse } from '~/utils/library-reader'
import type { ReadingProgress } from '~/utils/library-progress'
definePageMeta({ layout: 'library' })
useHead({ title: '个人阅读中心', meta: [{ name: 'robots', content: 'noindex, nofollow' }] })
const { request, logout } = useLibraryAccess()
const books = ref<LibraryBook[]>([]), total = ref(0), page = ref(1), query = ref('')
const loading = ref(true), uploading = ref(false), error = ref(''), notice = ref('')
const converting = ref(false)
const fileInput = ref<HTMLInputElement>()
const recent = ref<{ book: LibraryBook; progress: ReadingProgress } | null>(null)
function readTime(value: string) { return new Date(value.endsWith('Z') ? value : `${value}Z`).toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' }) }
let sequence = 0
const abort = new AbortController()
async function load(nextPage = 1) {
  const current = ++sequence
  loading.value = true; error.value = ''
  try {
    const [response, continued] = await Promise.all([
      request<LibraryResponse<{ items: LibraryBook[]; total: number }>>(`/books?page=${nextPage}&pageSize=24&q=${encodeURIComponent(query.value.trim())}`, 'GET', undefined, { signal: abort.signal }),
      request<LibraryResponse<typeof recent.value>>('/continue', 'GET', undefined, { signal: abort.signal }),
    ])
    if (current !== sequence || abort.signal.aborted) return
    books.value = response.data.items; total.value = response.data.total; page.value = nextPage
    recent.value = continued.data
  } catch (cause) { if (current === sequence && !abort.signal.aborted) error.value = libraryError(cause) }
  finally { if (current === sequence) loading.value = false }
}
async function upload(event: Event) {
  const input = event.target as HTMLInputElement, file = input.files?.[0]
  input.value = ''
  if (!file || uploading.value) return
  error.value = ''; notice.value = ''
  if (!/\.(epub|mobi)$/i.test(file.name) || !file.size) { error.value = '请选择非空的 EPUB 或 MOBI 文件。'; return }
  if (file.size > 100 * 1024 * 1024) { error.value = '文件超过 100 MiB。'; return }
  uploading.value = true
  converting.value = /\.mobi$/i.test(file.name)
  try {
    const form = new FormData(); form.append('file', file)
    await request('/books', 'POST', form, { signal: abort.signal, timeout: 180000 })
    query.value = ''; await load(1); notice.value = '书籍已加入书架。'
  } catch (cause) { if (!abort.signal.aborted) error.value = libraryError(cause, converting.value ? 'mobi' : 'epub') }
  finally { uploading.value = false; converting.value = false }
}
onMounted(() => { void load() })
onBeforeUnmount(() => { abort.abort(); sequence++ })
</script>
