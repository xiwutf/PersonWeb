<template>
  <div class="life-margin">
    <AdminEditBanner />

    <header class="life-margin-head">
      <div class="life-margin-head-copy">
        <p class="life-margin-kicker">05 · MARGIN</p>
        <h1>摘句</h1>
        <p class="life-margin-lead">
          先记下，不急着解释，也不急着分类。
        </p>
      </div>
      <div v-if="isAdmin" class="life-margin-head-actions">
        <LifeAdminAddButton label="扫描纸张" @click="scannerOpen = !scannerOpen" />
        <LifeAdminAddButton label="记一句" @click="openComposer" />
      </div>
    </header>

    <LifeMarginScanner
      :open="scannerOpen"
      @close="scannerOpen = false"
      @confirm="importScannedSentences"
    />

    <p v-if="!marginItems.length" class="life-empty">
      还没有摘句。想到一句再记。
    </p>

    <div v-else class="life-margin-collection">
      <section v-if="paperRecords.length" class="life-margin-papers" aria-labelledby="margin-papers-title">
        <header class="life-margin-group-head">
          <div class="life-margin-group-title">
            <span class="life-margin-group-index" aria-hidden="true">P</span>
            <h2 id="margin-papers-title">纸页</h2>
            <span class="life-margin-group-count">{{ paperRecords.length }} 张</span>
          </div>
          <span class="life-margin-group-rule" aria-hidden="true"></span>
        </header>

        <article v-for="paper in paperRecords" :key="paper.batch" class="life-margin-paper-record">
          <figure>
            <img :src="paper.imageUrl" alt="手写摘句原纸">
            <figcaption>
              <span>原纸 · {{ paper.items.length }} 句</span>
              <button
                v-if="isAdmin"
                type="button"
                class="life-admin-add"
                :disabled="deletingPaperBatch === paper.batch"
                @click="deletePaperImage(paper)"
              >
                {{ deletingPaperBatch === paper.batch ? '正在删除…' : '删除原纸' }}
              </button>
            </figcaption>
          </figure>
          <ol>
            <li v-for="(entry, paperIndex) in paper.items" :key="entry.index">
              <span aria-hidden="true">{{ formatEntryIndex(paperIndex) }}</span>
              <div class="life-margin-paper-entry">
                <InlineEditableText
                  v-if="isAdmin"
                  v-model="marginItems[entry.index].text"
                  :field-path="`items.${entry.index}.text`"
                  as="p"
                  display-class="life-margin-text"
                  multiline
                  :save="saveMarginField"
                />
                <p v-else class="life-margin-text">{{ entry.item.text }}</p>

                <aside v-if="isAdmin || entry.item.note" class="life-margin-paper-aside">
                  <InlineEditableText
                    v-if="isAdmin"
                    v-model="marginItems[entry.index].note"
                    :field-path="`items.${entry.index}.note`"
                    as="p"
                    display-class="life-margin-note life-margin-paper-note"
                    multiline
                    :save="saveMarginField"
                  />
                  <p v-else class="life-margin-note life-margin-paper-note">{{ entry.item.note }}</p>
                </aside>
              </div>
              <div v-if="isAdmin" class="life-margin-paper-item-actions">
                <button
                  v-if="paperIndex < paper.items.length - 1"
                  type="button"
                  class="life-admin-add"
                  @click="mergePaperItems(entry.index, paper.items[paperIndex + 1].index)"
                >
                  合并下一句
                </button>
                <button type="button" class="life-admin-add" @click="removeItem(entry.index)">删除</button>
              </div>
            </li>
          </ol>
        </article>
      </section>

      <section v-if="featuredItems.length" class="life-margin-focus" aria-labelledby="margin-focus-title">
        <header class="life-margin-focus-head">
          <div>
            <p class="life-margin-focus-kicker">KEEP CLOSE</p>
            <h2 id="margin-focus-title">常看</h2>
          </div>
          <p>留在眼前，偶尔重读。</p>
        </header>

        <div class="life-margin-focus-stage">
          <article
            v-for="(entry, entryIndex) in featuredItems"
            :key="entry.index"
            class="life-margin-focus-card"
            :class="[`is-card-${entryIndex % 4}`, toneClass(entry.item.tone), { 'is-admin': isAdmin }]"
          >
            <span class="life-tape life-tape--scrap" aria-hidden="true"></span>
            <div class="life-margin-quote">
              <InlineEditableText
                v-if="isAdmin"
                v-model="marginItems[entry.index].text"
                :field-path="`items.${entry.index}.text`"
                as="p"
                display-class="life-margin-text"
                multiline
                :save="saveMarginField"
              />
              <p v-else class="life-margin-text">{{ entry.item.text }}</p>
            </div>
            <aside v-if="isAdmin || entry.item.note" class="life-margin-aside">
              <InlineEditableText
                v-if="isAdmin"
                v-model="marginItems[entry.index].note"
                :field-path="`items.${entry.index}.note`"
                as="p"
                display-class="life-margin-note"
                multiline
                :save="saveMarginField"
              />
              <p v-else class="life-margin-note">{{ entry.item.note }}</p>
            </aside>
            <div v-if="isAdmin" class="life-margin-admin">
              <label>
                <span class="sr-only">笔触</span>
                <select :value="entry.item.tone || 'plain'" @change="onToneChange(entry.index, $event)">
                  <option v-for="tone in toneOptions" :key="tone.value" :value="tone.value">{{ tone.label }}</option>
                </select>
              </label>
              <button type="button" class="life-admin-add" @click="setFeatured(entry.index, false)">移出常看</button>
              <button type="button" class="life-admin-add" @click="setArchived(entry.index, true)">归档</button>
            </div>
          </article>
        </div>
      </section>

      <div v-if="groupedItems.length" class="life-margin-groups">
      <section
        v-for="group in groupedItems"
        :key="group.title"
        class="life-margin-group"
        :class="{ 'is-catchall': group.title === '随手记' }"
        :aria-labelledby="`margin-group-${group.title}`"
      >
        <header class="life-margin-group-head">
          <div class="life-margin-group-title">
            <span class="life-margin-group-index" aria-hidden="true">{{ formatGroupIndex(group.index) }}</span>
            <h2 :id="`margin-group-${group.title}`">{{ group.title }}</h2>
            <span class="life-margin-group-count">{{ group.items.length }} 则</span>
          </div>
          <span class="life-margin-group-rule" aria-hidden="true"></span>
        </header>

        <div class="life-margin-wall">
          <article
            v-for="(entry, entryIndex) in group.items"
            :key="entry.index"
            class="life-margin-scrap"
            :class="{
              'is-featured': !!entry.item.featured,
              'is-brief': entry.item.text.length <= 16,
              [toneClass(entry.item.tone)]: true,
              'has-note': !!entry.item.note,
              'is-admin': isAdmin,
            }"
          >
            <span class="life-margin-entry-index" aria-hidden="true">{{ formatEntryIndex(entryIndex) }}</span>

            <div class="life-margin-quote">
              <InlineEditableText
                v-if="isAdmin"
                v-model="marginItems[entry.index].text"
                :field-path="`items.${entry.index}.text`"
                as="p"
                display-class="life-margin-text"
                multiline
                :save="saveMarginField"
              />
              <p v-else class="life-margin-text">{{ entry.item.text }}</p>
            </div>

            <aside
              v-if="isAdmin || entry.item.note"
              class="life-margin-aside"
            >
              <InlineEditableText
                v-if="isAdmin"
                v-model="marginItems[entry.index].note"
                :field-path="`items.${entry.index}.note`"
                as="p"
                display-class="life-margin-note"
                multiline
                :save="saveMarginField"
              />
              <p v-else class="life-margin-note">{{ entry.item.note }}</p>
            </aside>

            <div v-if="isAdmin" class="life-margin-admin">
              <label>
                <span class="sr-only">笔触</span>
                <select :value="entry.item.tone || 'plain'" @change="onToneChange(entry.index, $event)">
                  <option v-for="tone in toneOptions" :key="tone.value" :value="tone.value">{{ tone.label }}</option>
                </select>
              </label>
              <label>
                <span class="sr-only">分组</span>
                <input
                  :value="marginItems[entry.index].group || ''"
                  type="text"
                  maxlength="24"
                  placeholder="分组"
                  @change="onGroupChange(entry.index, $event)"
                >
              </label>
              <label class="life-margin-featured">
                <input
                  type="checkbox"
                  :checked="!!entry.item.featured"
                  @change="onFeaturedChange(entry.index, $event)"
                >
                常看
              </label>
              <label class="life-margin-featured">
                <input
                  type="checkbox"
                  :checked="!!entry.item.archived"
                  @change="onArchivedChange(entry.index, $event)"
                >
                归档
              </label>
              <button
                type="button"
                class="life-admin-add"
                @click="removeItem(entry.index)"
              >
                删除
              </button>
            </div>
          </article>
        </div>
      </section>
      </div>

      <details v-if="archivedItems.length" class="life-margin-archive">
        <summary>
          <span>归档</span>
          <small>{{ archivedItems.length }} 句，收起来但不丢掉</small>
        </summary>
        <div class="life-margin-archive-list">
          <article
            v-for="entry in archivedItems"
            :key="entry.index"
            class="life-margin-archive-item"
            :class="toneClass(entry.item.tone)"
          >
            <p>{{ entry.item.text }}</p>
            <div v-if="isAdmin" class="life-margin-admin">
              <button type="button" class="life-admin-add" @click="setArchived(entry.index, false)">重新放回</button>
              <button type="button" class="life-admin-add" @click="removeItem(entry.index)">删除</button>
            </div>
          </article>
        </div>
      </details>
    </div>

    <LifeComposer
      :open="composer.open"
      title="记一句摘句"
      @submit="submitItem"
      @cancel="composer.open = false"
    >
      <label>
        句子
        <textarea v-model="composer.text" maxlength="400" rows="3" required />
      </label>
      <label>
        批注（可空）
        <textarea v-model="composer.note" maxlength="240" rows="2" />
      </label>
      <label class="life-margin-featured">
        <input v-model="composer.featured" type="checkbox">
        放进「常看」
      </label>
      <label>
        落笔的感觉
        <select v-model="composer.tone">
          <option v-for="tone in toneOptions" :key="tone.value" :value="tone.value">{{ tone.label }}</option>
        </select>
      </label>
    </LifeComposer>

    <p class="life-about-back">
      <NuxtLink to="/life" class="life-note-back">← 返回 Life</NuxtLink>
    </p>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'life'
})

type LifeMarginTone = 'plain' | 'strong' | 'quiet' | 'curious' | 'passing'

const toneOptions: Array<{ value: LifeMarginTone, label: string }> = [
  { value: 'plain', label: '平写' },
  { value: 'strong', label: '用力' },
  { value: 'quiet', label: '轻声' },
  { value: 'curious', label: '惊奇' },
  { value: 'passing', label: '略过' },
]

type LifeMarginItem = {
  text: string
  note: string
  group?: string
  featured?: boolean
  archived?: boolean
  tone?: LifeMarginTone
  sourceImage?: string
  scanBatch?: string
}

const { isAdmin } = useAdminSession()
const { saveMargin } = useLifeListSave()
const api = useApi()

const normalize = (items: Array<{
  text: string
  note?: string
  group?: string
  featured?: boolean
  archived?: boolean
  tone?: LifeMarginTone
  sourceImage?: string
  scanBatch?: string
}>): LifeMarginItem[] =>
  items.map(item => ({
    text: item.text,
    note: item.note || '',
    group: item.group,
    featured: item.featured,
    archived: item.archived,
    tone: item.tone,
    sourceImage: item.sourceImage,
    scanBatch: item.scanBatch,
  }))

const { data: marginData } = await useAsyncData('life-margin', () =>
  $fetch<Array<{ text: string, note?: string, group?: string, featured?: boolean, archived?: boolean, tone?: LifeMarginTone, sourceImage?: string, scanBatch?: string }>>('/api/content/life/margin')
)

const marginItems = ref<LifeMarginItem[]>(normalize(marginData.value || []))
const scannerOpen = ref(false)
const deletingPaperBatch = ref<string | null>(null)

const composer = reactive({
  open: false,
  text: '',
  note: '',
  featured: false,
  tone: 'plain' as LifeMarginTone,
})

const groupedItems = computed(() => {
  const buckets = new Map<string, { title: string, items: { index: number, item: LifeMarginItem }[] }>()
  marginItems.value.forEach((item, index) => {
    if (item.archived || item.featured || item.scanBatch) return
    const title = item.group?.trim() || '随手记'
    if (!buckets.has(title)) {
      buckets.set(title, { title, items: [] })
    }
    buckets.get(title)!.items.push({ index, item })
  })
  return [...buckets.values()].map((group, index) => ({ ...group, index }))
})

const featuredItems = computed(() => marginItems.value
  .map((item, index) => ({ index, item }))
  .filter(entry => entry.item.featured && !entry.item.archived))

const archivedItems = computed(() => marginItems.value
  .map((item, index) => ({ index, item }))
  .filter(entry => entry.item.archived))

const paperRecords = computed(() => {
  const papers = new Map<string, { batch: string, imageUrl: string, items: { index: number, item: LifeMarginItem }[] }>()
  marginItems.value.forEach((item, index) => {
    if (!item.scanBatch || !item.sourceImage || item.archived) return
    if (!papers.has(item.scanBatch)) {
      papers.set(item.scanBatch, { batch: item.scanBatch, imageUrl: item.sourceImage, items: [] })
    }
    papers.get(item.scanBatch)!.items.push({ index, item })
  })
  return [...papers.values()]
})

const formatGroupIndex = (index: number) => String(index + 1).padStart(2, '0')
const formatEntryIndex = (index: number) => String(index + 1).padStart(2, '0')
const toneClass = (tone?: LifeMarginTone) => `is-tone-${tone || 'plain'}`

async function notifySaveError() {
  const { useNotification } = await import('~/composables/useToast')
  useNotification().error('无法写入内容文件')
}

function toPayload(items: LifeMarginItem[]) {
  return items.map((item) => {
    const row: { text: string, note?: string, group?: string, featured?: boolean, archived?: boolean, tone?: LifeMarginTone, sourceImage?: string, scanBatch?: string } = {
      text: item.text,
    }
    if (item.note.trim()) row.note = item.note.trim()
    if (item.group?.trim()) row.group = item.group.trim()
    if (item.featured) row.featured = true
    if (item.archived) row.archived = true
    if (item.tone && item.tone !== 'plain') row.tone = item.tone
    if (item.sourceImage) row.sourceImage = item.sourceImage
    if (item.scanBatch) row.scanBatch = item.scanBatch
    return row
  })
}

async function persist(next: LifeMarginItem[]) {
  const saved = await saveMargin(toPayload(next))
  marginItems.value = normalize(saved)
  marginData.value = saved
}

async function saveMarginField(path: string, value: string) {
  const match = path.match(/^items\.(\d+)\.(text|note)$/)
  if (!match) throw new Error('invalid path')
  const index = Number(match[1])
  const field = match[2] as 'text' | 'note'
  const next = structuredClone(toRaw(marginItems.value))
  next[index] = { ...next[index], [field]: value }
  await persist(next)
}

async function onGroupChange(index: number, event: Event) {
  const target = event.target as HTMLInputElement
  try {
    const next = structuredClone(toRaw(marginItems.value))
    const group = target.value.trim()
    if (group) {
      next[index] = { ...next[index], group }
    } else {
      const { group: _removed, ...rest } = next[index]
      next[index] = rest as LifeMarginItem
    }
    await persist(next)
  } catch {
    await notifySaveError()
  }
}

async function onFeaturedChange(index: number, event: Event) {
  const target = event.target as HTMLInputElement
  await setFeatured(index, target.checked)
}

async function onToneChange(index: number, event: Event) {
  const target = event.target as HTMLSelectElement
  const tone = target.value as LifeMarginTone
  try {
    const next = structuredClone(toRaw(marginItems.value))
    next[index] = { ...next[index], tone }
    await persist(next)
  } catch {
    await notifySaveError()
  }
}

async function setFeatured(index: number, featured: boolean) {
  try {
    const next = structuredClone(toRaw(marginItems.value))
    if (featured) {
      next[index] = { ...next[index], featured: true, archived: false }
    } else {
      const { featured: _removed, ...rest } = next[index]
      next[index] = rest as LifeMarginItem
    }
    await persist(next)
  } catch {
    await notifySaveError()
  }
}

async function onArchivedChange(index: number, event: Event) {
  const target = event.target as HTMLInputElement
  await setArchived(index, target.checked)
}

async function setArchived(index: number, archived: boolean) {
  try {
    const next = structuredClone(toRaw(marginItems.value))
    if (archived) {
      next[index] = { ...next[index], archived: true, featured: false }
    } else {
      const { archived: _removed, ...rest } = next[index]
      next[index] = rest as LifeMarginItem
    }
    await persist(next)
  } catch {
    await notifySaveError()
  }
}

function openComposer() {
  composer.text = ''
  composer.note = ''
  composer.featured = false
  composer.tone = 'plain'
  composer.open = true
}

async function submitItem() {
  try {
    const item: LifeMarginItem = {
      text: composer.text.trim(),
      note: composer.note.trim(),
      tone: composer.tone,
    }
    if (composer.featured) item.featured = true
    await persist([item, ...marginItems.value])
    composer.open = false
  } catch {
    await notifySaveError()
  }
}

async function importScannedSentences(result: { imageUrl: string, sentences: string[] }) {
  try {
    const batch = `paper-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`
    const scannedItems: LifeMarginItem[] = result.sentences.map(text => ({
      text,
      note: '',
      tone: 'plain',
      sourceImage: result.imageUrl,
      scanBatch: batch,
    }))
    await persist([...scannedItems, ...marginItems.value])
    scannerOpen.value = false
  } catch {
    await notifySaveError()
  }
}

async function deletePaperImage(paper: { batch: string, imageUrl: string }) {
  if (deletingPaperBatch.value || !window.confirm('删除这张原纸吗？识别出的句子和批注会保留，并移到「随手记」。')) return

  deletingPaperBatch.value = paper.batch
  try {
    const next = structuredClone(toRaw(marginItems.value)).map((item) => {
      if (item.scanBatch !== paper.batch) return item
      const { sourceImage: _sourceImage, scanBatch: _scanBatch, ...rest } = item
      return rest as LifeMarginItem
    })
    await persist(next)
    await api.delete('Media/upload', { query: { url: paper.imageUrl }, silent: true })
    const { useNotification } = await import('~/composables/useToast')
    useNotification().success('原纸已删除，句子已移到「随手记」')
  } catch (error) {
    const { useNotification } = await import('~/composables/useToast')
    useNotification().error(error instanceof Error ? error.message : '原纸删除失败')
  } finally {
    deletingPaperBatch.value = null
  }
}

async function mergePaperItems(currentIndex: number, nextIndex: number) {
  try {
    const next = structuredClone(toRaw(marginItems.value))
    const currentItem = next[currentIndex]
    const followingItem = next[nextIndex]
    if (!currentItem || !followingItem || currentItem.scanBatch !== followingItem.scanBatch) return

    currentItem.text = [currentItem.text.trim(), followingItem.text.trim()].filter(Boolean).join('\n')
    currentItem.note = [currentItem.note?.trim(), followingItem.note?.trim()].filter(Boolean).join('\n')
    next.splice(nextIndex, 1)
    await persist(next)
  } catch {
    await notifySaveError()
  }
}

async function removeItem(index: number) {
  try {
    const next = marginItems.value.filter((_, itemIndex) => itemIndex !== index)
    await persist(next)
  } catch {
    await notifySaveError()
  }
}

usePageSeo({
  title: '摘句 - 溪午听风',
  description: '溪午听风记下的短句与感想。',
  path: '/life/margin',
  world: 'life',
})
</script>
