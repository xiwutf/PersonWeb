<template>
  <div class="life-thoughts-detail">
    <AdminEditBanner />

    <p v-if="pending && !page" class="life-empty">加载中…</p>

    <template v-else-if="page">
      <header class="life-thoughts-detail-head">
        <div class="life-thoughts-detail-heading">
          <div>
            <p class="life-thoughts-kicker">{{ page.category.index }}</p>
            <h1>{{ page.category.title }}</h1>
            <p v-if="page.category.description" class="life-thoughts-detail-desc">
              {{ page.category.description }}
            </p>
            <p class="life-thoughts-detail-count">{{ items.length }} 则</p>
          </div>
          <LifeAdminAddButton label="加一句" @click="openComposer" />
        </div>
      </header>

      <p
        v-if="isAdmin && !canEditContent"
        class="life-empty"
        role="status"
      >
        已登录，但线上环境不能改摘句文件。请用本地 <code>npm run dev</code> 打开 localhost:3000 再编辑。
      </p>

      <LifeComposer
        :open="composer.open"
        title="新增一句"
        @submit="submitItem"
        @cancel="closeComposer"
      >
        <label>
          句子
          <textarea v-model="composer.text" maxlength="800" rows="4" required />
        </label>
        <label>
          备注（可选）
          <textarea v-model="composer.note" maxlength="800" rows="2" />
        </label>
      </LifeComposer>

      <p v-if="!items.length && !composer.open" class="life-empty">这个分类还没有句子。</p>

      <div v-else class="life-thoughts-detail-list">
        <LifeThoughtListItem
          v-for="(item, index) in items"
          :key="item.id"
          :item="item"
          :can-edit="canEditContent"
          :field-prefix="`items.${index}`"
          :save="saveField"
          @remove="removeItem(index)"
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

type ThoughtItem = {
  id: string
  text: string
  note?: string
  source?: string
  createdAt?: string
  featured?: boolean
  category?: string
  priority?: number
}

type CategoryPage = {
  category: {
    slug: string
    index: string
    title: string
    description: string
  }
  items: ThoughtItem[]
}

const route = useRoute()
const slug = computed(() => String(route.params.category || ''))
const { isAdmin, canEditContent } = useAdminSession()
const { saveThoughtItems } = useLifeListSave()

const { data: page, pending, error, refresh } = await useAsyncData(
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

const items = computed(() => page.value?.items || [])

const composer = reactive({
  open: false,
  text: '',
  note: '',
})

function todayIsoDate() {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

function cloneItems(source: ThoughtItem[]): ThoughtItem[] {
  return source.map(item => ({ ...item }))
}

async function notifySaveError(error: unknown) {
  const message = error instanceof Error ? error.message : '保存失败'
  if (import.meta.client) {
    window.alert(message)
  }
}

async function persist(nextItems: ThoughtItem[]) {
  const payload = await saveThoughtItems(slug.value, nextItems.map(item => ({
    id: item.id,
    category: slug.value,
    text: item.text,
    featured: item.featured === true,
    priority: item.priority || 0,
    note: item.note,
    source: item.source,
    createdAt: item.createdAt,
  })))
  if (page.value) {
    page.value = {
      ...page.value,
      items: payload.items,
    }
  }
  await refresh()
}

function openComposer() {
  composer.text = ''
  composer.note = ''
  composer.open = true
}

function closeComposer() {
  composer.open = false
  composer.text = ''
  composer.note = ''
}

async function submitItem() {
  const text = composer.text.trim()
  if (!text) return
  const maxPriority = Math.max(0, ...items.value.map(item => item.priority || 0))
  try {
    await persist([
      {
        id: `${slug.value}-${Date.now().toString(36)}`,
        category: slug.value,
        text,
        note: composer.note.trim() || undefined,
        featured: false,
        priority: maxPriority + 1,
        createdAt: todayIsoDate(),
      },
      ...cloneItems(items.value),
    ])
    closeComposer()
  }
  catch (err) {
    await notifySaveError(err)
  }
}

async function saveField(path: string, value: string) {
  const match = path.match(/^items\.(\d+)\.(text|note)$/)
  if (!match) throw new Error('invalid path')
  const index = Number(match[1])
  const field = match[2] as 'text' | 'note'
  const next = cloneItems(items.value)
  if (!next[index]) throw new Error('item missing')
  if (field === 'text') {
    const text = value.trim()
    if (!text) throw new Error('句子不能为空')
    next[index] = { ...next[index], text }
  }
  else {
    next[index] = { ...next[index], note: value.trim() || undefined }
  }
  await persist(next)
}

async function removeItem(index: number) {
  if (!import.meta.client) return
  if (!window.confirm('删除这一句？')) return
  try {
    const next = cloneItems(items.value)
    next.splice(index, 1)
    await persist(next)
  }
  catch (err) {
    await notifySaveError(err)
  }
}

usePageSeo(() => ({
  title: `${page.value?.category.title || '想法'} - 溪午听风 · Life`,
  description: page.value?.category.description || '生活里的想法归档。',
  path: `/life/thoughts/${slug.value}`,
  world: 'life',
}))
</script>
