<template>
  <div class="life-cognition">
    <AdminEditBanner />

    <div v-if="pending" class="life-empty">加载中…</div>

    <article v-else class="life-cognition-article">
      <header class="life-cognition-masthead">
        <div class="life-cognition-masthead-row">
          <div>
            <p class="life-cognition-kicker">说明书</p>
            <InlineEditableText
              v-if="canEditContent"
              v-model="doc.title"
              field-path="title"
              as="h1"
              :save="saveCognitionField"
            />
            <h1 v-else>{{ doc.title }}</h1>

            <InlineEditableText
              v-if="canEditContent"
              v-model="doc.summary"
              field-path="summary"
              as="p"
              display-class="life-cognition-lead"
              multiline
              :save="saveCognitionField"
            />
            <p v-else-if="doc.summary" class="life-cognition-lead">{{ doc.summary }}</p>

            <p v-if="doc.updatedAt" class="life-cognition-meta">
              更新于 {{ formatDate(doc.updatedAt) }}
            </p>
          </div>
          <LifeAdminAddButton
            v-if="canEditContent"
            label="加一章"
            @click="addChapter"
          />
        </div>
      </header>

      <p
        v-if="isAdmin && !canEditContent"
        class="life-empty"
        role="status"
      >
        已登录，但线上环境不能改说明书文件。请用本地 <code>npm run dev</code> 打开 localhost:3000 再编辑。
      </p>

      <p v-if="!doc.chapters.length" class="life-empty">还没有章节。</p>

      <div v-else class="life-manual-body">
        <section
          v-for="(chapter, index) in doc.chapters"
          :key="`chapter-${index}`"
          class="life-manual-chapter"
          :style="{ '--chapter-i': index }"
        >
          <InlineEditableText
            v-if="canEditContent"
            v-model="doc.chapters[index].title"
            :field-path="`chapters.${index}.title`"
            as="h2"
            :save="saveCognitionField"
          />
          <h2 v-else>{{ chapter.title }}</h2>

          <InlineEditableText
            v-if="canEditContent"
            v-model="doc.chapters[index].body"
            :field-path="`chapters.${index}.body`"
            as="p"
            display-class="life-manual-prose life-cognition-body-edit"
            multiline
            :rows="14"
            :save="saveCognitionField"
          />
          <div
            v-else
            class="life-manual-prose"
            v-html="renderChapterBody(chapter.body)"
          />

          <div v-if="canEditContent" class="life-cognition-chapter-actions">
            <button
              type="button"
              class="life-admin-add"
              :disabled="index === 0"
              @click="moveChapter(index, -1)"
            >
              上移
            </button>
            <button
              type="button"
              class="life-admin-add"
              :disabled="index >= doc.chapters.length - 1"
              @click="moveChapter(index, 1)"
            >
              下移
            </button>
            <button
              type="button"
              class="life-admin-add"
              @click="removeChapter(index)"
            >
              删除本章
            </button>
          </div>
        </section>
      </div>

      <p class="life-about-back">
        <NuxtLink to="/life" class="life-note-back">← 返回 Life</NuxtLink>
      </p>
    </article>
  </div>
</template>

<script setup lang="ts">
import { usePageSeo, useJsonLd, toAbsoluteUrl } from '~/composables/usePageSeo'

definePageMeta({
  layout: 'life',
})

type LifeCognitionChapter = {
  title: string
  body: string
}

type LifeCognitionContent = {
  title: string
  summary: string
  updatedAt: string
  chapters: LifeCognitionChapter[]
}

const emptyDoc = (): LifeCognitionContent => ({
  title: '个人认知使用说明书',
  summary: '',
  updatedAt: '',
  chapters: [],
})

const { isAdmin, canEditContent } = useAdminSession()
const { parse } = useMarkdown()
const { saveCognition } = useLifeListSave()

const { data, pending } = await useAsyncData('life-cognition', () =>
  $fetch<LifeCognitionContent>('/api/content/life/cognition'),
)

function cloneDoc(value: LifeCognitionContent | null | undefined): LifeCognitionContent {
  const source = toRaw(value) || emptyDoc()
  return {
    title: source.title || '',
    summary: source.summary || '',
    updatedAt: source.updatedAt || '',
    chapters: (source.chapters || []).map(chapter => ({
      title: chapter.title || '',
      body: chapter.body || '',
    })),
  }
}

const doc = ref<LifeCognitionContent>(cloneDoc(data.value))

watch(data, (next) => {
  if (next) doc.value = cloneDoc(next)
})

function renderChapterBody(body: string) {
  return parse(body || '')
}

function formatDate(dateString?: string) {
  if (!dateString) return ''
  return new Date(`${dateString}T00:00:00`).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
}

async function notifySaveError(error: unknown) {
  const message = error instanceof Error ? error.message : '保存失败'
  if (import.meta.client) {
    window.alert(message)
  }
}

async function persist(next: LifeCognitionContent) {
  const payload = await saveCognition({
    title: next.title,
    summary: next.summary,
    chapters: next.chapters.map(chapter => ({
      title: chapter.title,
      body: chapter.body,
    })),
  })
  doc.value = cloneDoc(payload as LifeCognitionContent)
  data.value = doc.value
}

async function saveCognitionField(path: string, value: string) {
  const next = cloneDoc(doc.value)
  if (path === 'title') {
    next.title = value
  }
  else if (path === 'summary') {
    next.summary = value
  }
  else {
    const match = path.match(/^chapters\.(\d+)\.(title|body)$/)
    if (!match) throw new Error('invalid path')
    const index = Number(match[1])
    const field = match[2] as 'title' | 'body'
    if (!next.chapters[index]) throw new Error('chapter missing')
    next.chapters[index] = { ...next.chapters[index], [field]: value }
  }
  await persist(next)
}

async function addChapter() {
  try {
    const next = cloneDoc(doc.value)
    next.chapters.push({
      title: '新章节',
      body: '本章解决什么问题：\n\n写在这里。',
    })
    await persist(next)
  }
  catch (error) {
    await notifySaveError(error)
  }
}

async function removeChapter(index: number) {
  if (!import.meta.client) return
  if (!window.confirm('删除这一章？')) return
  try {
    const next = cloneDoc(doc.value)
    next.chapters.splice(index, 1)
    await persist(next)
  }
  catch (error) {
    await notifySaveError(error)
  }
}

async function moveChapter(index: number, delta: number) {
  const target = index + delta
  if (target < 0 || target >= doc.value.chapters.length) return
  try {
    const next = cloneDoc(doc.value)
    const [item] = next.chapters.splice(index, 1)
    next.chapters.splice(target, 0, item)
    await persist(next)
  }
  catch (error) {
    await notifySaveError(error)
  }
}

usePageSeo(() => ({
  title: `${doc.value.title || '认知说明书'} - 溪午听风 · Life`,
  description: doc.value.summary || '个人认知使用说明书。',
  path: '/life/cognition',
  type: 'article',
  world: 'life',
}))

useJsonLd(() => ({
  '@context': 'https://schema.org',
  '@type': 'Article',
  headline: doc.value.title,
  description: doc.value.summary || undefined,
  dateModified: doc.value.updatedAt || undefined,
  author: { '@type': 'Person', name: '溪午听风' },
  mainEntityOfPage: toAbsoluteUrl('/life/cognition'),
}))
</script>
