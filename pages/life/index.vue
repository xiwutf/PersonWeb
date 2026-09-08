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
          <InlineEditableText
            v-for="(_line, index) in home.hero.lines"
            :key="`hero-line-${index}`"
            v-model="home.hero.lines[index]"
            :field-path="`hero.lines.${index}`"
            as="p"
            display-class="life-intro-lead"
            :save="saveLifeField"
            @saved="onLifeSaved"
          />
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
          <InlineEditableText
            id="life-now-title"
            v-model="home.sections.now.title"
            field-path="sections.now.title"
            as="h2"
            :save="saveLifeField"
            @saved="onLifeSaved"
          />
          <div class="life-now-grid">
            <template v-for="item in lifeNow" :key="item.title">
              <NuxtLink
                v-if="item.href"
                :to="item.href"
                class="life-now-item"
              >
                <LifeIcon :name="nowIconOf(item)" />
                <strong>{{ item.title }}</strong>
                <p>{{ item.description }}</p>
              </NuxtLink>
              <article
                v-else
                class="life-now-item"
              >
                <LifeIcon :name="nowIconOf(item)" />
                <strong>{{ item.title }}</strong>
                <p>{{ item.description }}</p>
              </article>
            </template>
          </div>
        </div>
      </section>

      <section
        class="life-row"
        :class="{ 'life-row--empty': latelyMoments.length === 0 }"
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
            <InlineEditableText
              v-if="latelyMoments.length === 0"
              v-model="home.empty.moments"
              field-path="empty.moments"
              as="p"
              display-class="life-row-desc"
              :save="saveLifeField"
              @saved="onLifeSaved"
            />
          </div>

          <div v-if="latelyMoments.length" class="life-lately-list">
            <article
              v-for="item in latelyMoments"
              :key="`${item.date}-${item.content}`"
              class="life-lately-item"
            >
              <time :datetime="item.date">{{ formatShortDate(item.date) }}</time>
              <div class="life-lately-body">
                <NuxtLink
                  v-if="item.note"
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

const lifeNow = computed(() => now.value?.items || [])
const latelyMoments = computed(() => (moments.value || []).slice(0, LATEST_MOMENT_LIMIT))
const latestNotes = computed(() => (posts.value || []).slice(0, 5))

const nowIconOf = (item: LifeNowItem) => {
  if (item.icon) return item.icon

  const title = item.title || ''
  if (title.includes('运动')) return 'sneaker'
  if (title.includes('三国') || title.includes('茶')) return 'cards'
  if (title.includes('骑')) return 'bike'
  return 'leaf'
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
