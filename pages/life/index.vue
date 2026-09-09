<template>
  <div class="life-home">
    <AdminEditBanner />

    <section class="life-hero" aria-labelledby="life-intro-title">
      <div class="life-hero-copy">
        <div class="life-hero-decorations" aria-hidden="true">
          <aside class="life-scrap">
            <span class="life-tape life-tape--scrap"></span>
            <div class="life-scrap-paper">
              <LifeIcon name="branch" />
            </div>
          </aside>
          <svg class="life-title-seal" viewBox="0 0 88 88">
            <circle cx="44" cy="44" r="40" />
            <circle cx="44" cy="44" r="34" />
            <path d="M32 58c10-4 16-14 18-28-12 2-20 10-22 24Z" />
            <path d="M44 32c6-8 14-12 22-8" />
            <path d="M36 48c8-6 14-10 22-12" />
          </svg>
        </div>

        <div class="life-hero-content">
          <InlineEditableText
            v-model="home.hero.kicker"
            field-path="hero.kicker"
            as="p"
            display-class="life-intro-kicker"
            :save="saveLifeField"
            @saved="onLifeSaved"
          />
          <InlineEditableText
            v-model="home.hero.greeting"
            field-path="hero.greeting"
            as="p"
            display-class="life-intro-hello"
            :save="saveLifeField"
            @saved="onLifeSaved"
          />
          <div class="life-hero-heading">
            <InlineEditableText
              id="life-intro-title"
              v-model="home.hero.name"
              field-path="hero.name"
              as="h1"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
            <span class="life-title-rule">
              <span></span>
              <LifeIcon name="leaf" />
            </span>
          </div>
          <div
            v-for="(_line, index) in home.hero.lines"
            :key="`hero-line-${index}`"
            class="life-hero-line"
          >
            <InlineEditableText
              v-model="home.hero.lines[index]"
              :field-path="`hero.lines.${index}`"
              as="p"
              display-class="life-intro-lead"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
            <button
              v-if="isAdmin && home.hero.lines.length > 1"
              type="button"
              class="life-admin-icon-btn"
              aria-label="删除这句"
              @click="removeHeroLine(index)"
            >
              ×
            </button>
          </div>
          <LifeAdminAddButton label="加一句" @click="addHeroLine" />
        </div>
      </div>

      <div class="life-hero-scene">
        <p class="life-intro-stamp">
          {{ lifeHomeStamp }}
          <span class="life-intro-stamp-rule" aria-hidden="true">
            <span></span>
            <LifeIcon name="leaf" />
          </span>
        </p>
        <div class="life-scene-frame">
          <img
            src="/images/life/hero-desk.webp"
            alt=""
            width="1200"
            height="900"
            decoding="async"
            fetchpriority="high"
          >
          <div class="life-scene-light" aria-hidden="true"></div>
        </div>
      </div>
    </section>

    <div class="life-sheet">
      <section class="life-row life-row--now" aria-labelledby="life-now-title">
        <span class="life-num">{{ home.sections.now.number }}</span>
        <div class="life-row-body">
          <div class="life-row-heading">
            <InlineEditableText
              id="life-now-title"
              v-model="home.sections.now.title"
              field-path="sections.now.title"
              as="h2"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
            <LifeAdminAddButton label="新增一项" @click="openNowComposer" />
          </div>
          <div class="life-now-grid">
            <article
              v-for="(item, index) in nowItems"
              :key="`now-${index}-${item.title}`"
              class="life-now-item"
            >
              <LifeIcon :name="nowIconOf(item)" />
              <template v-if="isAdmin">
                <label class="life-admin-icon-pick">
                  <span class="sr-only">图标</span>
                  <select
                    :value="item.icon || nowIconOf(item)"
                    @change="onNowIconChange(index, $event)"
                  >
                    <option v-for="icon in nowIconOptions" :key="icon" :value="icon">
                      {{ nowIconLabel[icon] || icon }}
                    </option>
                  </select>
                </label>
                <InlineEditableText
                  v-model="nowItems[index].title"
                  :field-path="`items.${index}.title`"
                  as="strong"
                  :save="saveNowField"
                />
                <InlineEditableText
                  v-model="nowItems[index].description"
                  :field-path="`items.${index}.description`"
                  as="p"
                  :save="saveNowField"
                />
                <button
                  type="button"
                  class="life-admin-add"
                  @click="removeNowItem(index)"
                >
                  删除
                </button>
              </template>
              <NuxtLink
                v-else-if="item.href"
                :to="item.href"
                class="life-now-item-link"
              >
                <strong>{{ item.title }}</strong>
                <p>{{ item.description }}</p>
              </NuxtLink>
              <template v-else>
                <strong>{{ item.title }}</strong>
                <p>{{ item.description }}</p>
              </template>
            </article>
          </div>
          <LifeComposer
            :open="nowComposer.open"
            title="新增「最近在」"
            @submit="submitNowItem"
            @cancel="nowComposer.open = false"
          >
            <label>
              标题
              <input v-model="nowComposer.title" type="text" maxlength="40" required>
            </label>
            <label>
              说明
              <textarea v-model="nowComposer.description" maxlength="240" rows="3" required />
            </label>
            <label>
              图标
              <select v-model="nowComposer.icon">
                <option v-for="icon in nowIconOptions" :key="icon" :value="icon">
                  {{ nowIconLabel[icon] || icon }}
                </option>
              </select>
            </label>
          </LifeComposer>
        </div>
      </section>

      <section
        class="life-row"
        :class="{ 'life-row--empty': momentItems.length === 0 }"
        aria-labelledby="life-lately-title"
      >
        <span class="life-num">{{ home.sections.moments.number }}</span>
        <div class="life-row-body">
          <div class="life-row-line">
            <LifeIcon name="bubble" />
            <InlineEditableText
              id="life-lately-title"
              v-model="home.sections.moments.title"
              field-path="sections.moments.title"
              as="h2"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
            <LifeAdminAddButton label="记一条" @click="openMomentComposer" />
            <InlineEditableText
              v-if="momentItems.length === 0"
              v-model="home.empty.moments"
              field-path="empty.moments"
              as="p"
              display-class="life-row-desc"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
          </div>

          <div v-if="momentItems.length" class="life-lately-list">
            <article
              v-for="(item, index) in latelyMoments"
              :key="`${item.date}-${index}`"
              class="life-lately-item"
            >
              <time :datetime="item.date">{{ formatShortDate(item.date) }}</time>
              <div class="life-lately-body">
                <template v-if="isAdmin">
                  <InlineEditableText
                    v-model="momentItems[index].content"
                    :field-path="`items.${index}.content`"
                    as="p"
                    display-class="life-lately-text"
                    multiline
                    :save="saveMomentField"
                  />
                  <button
                    type="button"
                    class="life-admin-add"
                    @click="removeMoment(index)"
                  >
                    删除
                  </button>
                </template>
                <NuxtLink
                  v-else-if="item.note"
                  :to="item.note"
                  class="life-lately-text"
                >
                  {{ item.content }}
                </NuxtLink>
                <p v-else class="life-lately-text">{{ item.content }}</p>
                <img
                  v-if="item.image"
                  :src="item.image"
                  alt=""
                  class="life-lately-photo"
                >
              </div>
            </article>
          </div>
          <LifeComposer
            :open="momentComposer.open"
            title="新增一条最近"
            @submit="submitMoment"
            @cancel="momentComposer.open = false"
          >
            <label>
              日期
              <input v-model="momentComposer.date" type="date" required>
            </label>
            <label>
              内容
              <textarea v-model="momentComposer.content" maxlength="800" rows="3" required />
            </label>
          </LifeComposer>
        </div>
      </section>

      <section
        class="life-row"
        :class="{ 'life-row--empty': !latestNotes.length }"
        aria-labelledby="life-recent-title"
      >
        <span class="life-num">{{ home.sections.notes.number }}</span>
        <div class="life-row-body">
          <div class="life-row-line">
            <LifeIcon name="pencil" />
            <InlineEditableText
              id="life-recent-title"
              v-model="home.sections.notes.title"
              field-path="sections.notes.title"
              as="h2"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
            <LifeAdminAddButton label="写一篇" @click="openNoteComposer" />
            <InlineEditableText
              v-if="!latestNotes.length"
              v-model="home.empty.notes"
              field-path="empty.notes"
              as="p"
              display-class="life-row-desc"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
          </div>

          <div v-if="latestNotes.length" class="life-recent-list">
            <NuxtLink
              v-for="post in latestNotes"
              :key="post._path"
              :to="post._path"
              class="life-recent-item"
            >
              <time>{{ formatShortDate(post.date) }}</time>
              <span>
                <strong>{{ post.title }}</strong>
                <em v-if="post.description">{{ post.description }}</em>
              </span>
            </NuxtLink>
          </div>
          <LifeComposer
            :open="noteComposer.open"
            title="写一篇随笔"
            @submit="submitNote"
            @cancel="noteComposer.open = false"
          >
            <label>
              标题
              <input v-model="noteComposer.title" type="text" maxlength="80" required>
            </label>
            <label>
              摘要
              <input v-model="noteComposer.description" type="text" maxlength="240">
            </label>
            <label>
              正文
              <textarea v-model="noteComposer.content" maxlength="20000" rows="8" required />
            </label>
          </LifeComposer>
        </div>
      </section>

      <section class="life-row life-row--about" aria-labelledby="life-about-title">
        <span class="life-num">{{ home.sections.about.number }}</span>
        <div class="life-row-body">
          <div class="life-row-line">
            <LifeIcon name="vase" />
            <InlineEditableText
              id="life-about-title"
              v-model="home.sections.about.title"
              field-path="sections.about.title"
              as="h2"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
            <InlineEditableText
              v-model="home.about.description"
              field-path="about.description"
              as="p"
              display-class="life-row-desc"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
          </div>
          <NuxtLink
            to="/life/about"
            class="life-end-link"
            :class="{ 'is-admin-edit': isAdmin }"
            @click="onAboutLinkClick"
          >
            <InlineEditableText
              v-model="home.about.linkText"
              field-path="about.linkText"
              as="span"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
          </NuxtLink>
        </div>
      </section>

      <p class="life-end-close">
        <span class="life-tape life-tape--close" aria-hidden="true"></span>
        <InlineEditableText
          v-model="home.closing"
          field-path="closing"
          as="span"
          :save="saveLifeField"
          @saved="onLifeSaved"
        />
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { LIFE_NOW_ICONS } from '~/constants/life-content'

definePageMeta({
  layout: 'life'
})

type LifeHomeContent = {
  hero: {
    kicker: string
    greeting: string
    name: string
    lines: string[]
    current: string
  }
  sections: {
    now: { number: string, title: string }
    moments: { number: string, title: string }
    notes: { number: string, title: string }
    about: { number: string, title: string }
  }
  empty: {
    moments: string
    notes: string
  }
  about: {
    description: string
    linkText: string
  }
  closing: string
}

type LifeNowItem = {
  title: string
  description: string
  href?: string
  icon?: string
}

type LifeNowContent = {
  items: LifeNowItem[]
}

type LifeMoment = {
  date: string
  content: string
  image?: string
  note?: string
}

type LifeNote = {
  _path: string
  title?: string
  description?: string
  date?: string
}

const LATEST_MOMENT_LIMIT = 6

const { isAdmin } = useAdminSession()
const { saveField } = useInlineCopySave('/api/content/life/home')
const { saveNowItems, saveMoments, createNote, saveHeroLines } = useLifeListSave()
const nowIconOptions = LIFE_NOW_ICONS
const nowIconLabel: Record<string, string> = {
  leaf: '叶子',
  branch: '树枝',
  sneaker: '运动鞋',
  cards: '扑克',
  bike: '自行车',
  bubble: '气泡',
  pencil: '铅笔',
  vase: '花瓶',
}

const [{ data: homeData }, { data: now }, { data: moments }, { data: posts }] = await Promise.all([
  useAsyncData('life-home', () => $fetch<LifeHomeContent>('/api/content/life/home')),
  useAsyncData('life-now', () => $fetch<LifeNowContent>('/api/content/life/now')),
  useAsyncData('life-moments', () => $fetch<LifeMoment[]>('/api/content/life/moments')),
  useAsyncData('life-posts', () => $fetch<LifeNote[]>('/api/content/life'))
])

if (!homeData.value) {
  throw createError({ statusCode: 404, statusMessage: 'Life 首页文案不存在' })
}

const home = ref<LifeHomeContent>(structuredClone(toRaw(homeData.value)))
const nowItems = ref<LifeNowItem[]>(structuredClone(toRaw(now.value?.items || [])))
const momentItems = ref<LifeMoment[]>(structuredClone(toRaw(moments.value || [])))

const nowComposer = reactive({
  open: false,
  title: '',
  description: '',
  icon: 'leaf',
})

const momentComposer = reactive({
  open: false,
  date: '',
  content: '',
})

const noteComposer = reactive({
  open: false,
  title: '',
  description: '',
  content: '',
})

function todayIso() {
  const current = new Date()
  const month = String(current.getMonth() + 1).padStart(2, '0')
  const day = String(current.getDate()).padStart(2, '0')
  return `${current.getFullYear()}-${month}-${day}`
}

async function notifySaveError() {
  const { useNotification } = await import('~/composables/useToast')
  useNotification().error('无法写入内容文件')
}

async function saveLifeField(path: string, value: string) {
  return await saveField(path, value)
}

function onLifeSaved(payload: unknown) {
  if (payload && typeof payload === 'object') {
    home.value = payload as LifeHomeContent
    homeData.value = home.value
  }
}

function onAboutLinkClick(event: MouseEvent) {
  if (isAdmin.value) {
    event.preventDefault()
  }
}

const latelyMoments = computed(() => momentItems.value.slice(0, LATEST_MOMENT_LIMIT))
const latestNotes = computed(() => (posts.value || []).slice(0, 5))

const nowIconOf = (item: LifeNowItem) => {
  if (item.icon) return item.icon

  const title = item.title || ''
  if (title.includes('运动')) return 'sneaker'
  if (title.includes('三国') || title.includes('茶')) return 'cards'
  if (title.includes('骑')) return 'bike'
  return 'leaf'
}

async function persistNow(next: LifeNowItem[]) {
  const saved = await saveNowItems(next)
  nowItems.value = saved.items || next
  now.value = saved
}

async function persistMoments(next: LifeMoment[]) {
  const saved = await saveMoments(next)
  momentItems.value = saved
  moments.value = saved
}

async function saveNowField(path: string, value: string) {
  const match = path.match(/^items\.(\d+)\.(title|description|icon)$/)
  if (!match) throw new Error('invalid path')
  const index = Number(match[1])
  const field = match[2] as 'title' | 'description' | 'icon'
  const next = structuredClone(nowItems.value)
  next[index] = { ...next[index], [field]: value }
  await persistNow(next)
}

async function onNowIconChange(index: number, event: Event) {
  const target = event.target as HTMLSelectElement
  try {
    await saveNowField(`items.${index}.icon`, target.value)
  } catch {
    await notifySaveError()
  }
}

function openNowComposer() {
  nowComposer.title = ''
  nowComposer.description = ''
  nowComposer.icon = 'leaf'
  nowComposer.open = true
}

async function submitNowItem() {
  try {
    await persistNow([
      ...nowItems.value,
      {
        title: nowComposer.title,
        description: nowComposer.description,
        icon: nowComposer.icon,
      },
    ])
    nowComposer.open = false
  } catch {
    await notifySaveError()
  }
}

async function removeNowItem(index: number) {
  try {
    const next = nowItems.value.filter((_, itemIndex) => itemIndex !== index)
    await persistNow(next)
  } catch {
    await notifySaveError()
  }
}

async function saveMomentField(path: string, value: string) {
  const match = path.match(/^items\.(\d+)\.content$/)
  if (!match) throw new Error('invalid path')
  const index = Number(match[1])
  const next = structuredClone(momentItems.value)
  next[index] = { ...next[index], content: value }
  await persistMoments(next)
}

function openMomentComposer() {
  momentComposer.date = todayIso()
  momentComposer.content = ''
  momentComposer.open = true
}

async function submitMoment() {
  try {
    await persistMoments([
      {
        date: momentComposer.date,
        content: momentComposer.content,
      },
      ...momentItems.value,
    ])
    momentComposer.open = false
  } catch {
    await notifySaveError()
  }
}

async function removeMoment(index: number) {
  try {
    const next = momentItems.value.filter((_, itemIndex) => itemIndex !== index)
    await persistMoments(next)
  } catch {
    await notifySaveError()
  }
}

function openNoteComposer() {
  noteComposer.title = ''
  noteComposer.description = ''
  noteComposer.content = ''
  noteComposer.open = true
}

async function submitNote() {
  try {
    await createNote({
      title: noteComposer.title,
      description: noteComposer.description,
      content: noteComposer.content,
      date: todayIso(),
    })
    await refreshNuxtData('life-posts')
    noteComposer.open = false
  } catch {
    await notifySaveError()
  }
}

async function addHeroLine() {
  try {
    const payload = await saveHeroLines([...home.value.hero.lines, '新的一句，点这里改'])
    onLifeSaved(payload)
  } catch {
    await notifySaveError()
  }
}

async function removeHeroLine(index: number) {
  try {
    const next = home.value.hero.lines.filter((_, lineIndex) => lineIndex !== index)
    const payload = await saveHeroLines(next)
    onLifeSaved(payload)
  } catch {
    await notifySaveError()
  }
}

const lifeHomeStamp = (() => {
  const current = new Date()
  const year = current.getFullYear()
  const month = current.getMonth() + 1
  const day = current.getDate()

  let season = '冬天'
  if (month === 3 || month === 4 || month === 5) {
    season = month === 5 ? '春末' : month === 3 && day < 20 ? '初春' : '春天'
  } else if (month === 6 || month === 7 || month === 8) {
    season = month === 8 && day >= 20 ? '夏末' : month === 6 ? '初夏' : '夏天'
  } else if (month === 9 || month === 10 || month === 11) {
    season = month === 11 ? '秋末' : month === 9 ? '初秋' : '秋天'
  } else if (month === 12) {
    season = '初冬'
  } else if (month === 2) {
    season = '冬末'
  }

  return `${year} / ${season}`
})()

const formatShortDate = (dateString?: string) => {
  if (!dateString) return ''

  const date = new Date(dateString)
  if (Number.isNaN(date.getTime())) return ''

  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${month}.${day}`
}

usePageSeo(() => ({
  title: `${home.value.hero.name} - 生活`,
  description: home.value.hero.current || home.value.hero.lines[0] || '溪午听风的生活一面。',
  path: '/life',
  world: 'life',
}))
</script>
