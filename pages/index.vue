<template>
  <main class="portal-foyer">
    <div class="portal-worlds">
      <NuxtLink to="/life" class="portal-world portal-world--life">
        <span class="portal-identity">溪午听风</span>

        <div class="portal-world-main">
          <strong class="portal-world-name">生活</strong>
          <p v-if="lifeLine" class="portal-life-line">{{ lifeLine }}</p>

          <img
            class="portal-life-scene"
            src="/images/life/hero-desk.webp"
            alt=""
            width="960"
            height="720"
            decoding="async"
          >

          <dl v-if="lifeNow.length" class="portal-life-now">
            <div v-for="item in lifeNow" :key="item.title">
              <dt>{{ item.title }}</dt>
              <dd>{{ item.description }}</dd>
            </div>
          </dl>

          <p v-if="lifeNote" class="portal-life-note">
            <span>随笔</span>
            {{ lifeNote.title }}
          </p>
        </div>
      </NuxtLink>

      <NuxtLink to="/work" class="portal-world portal-world--work">
        <div class="portal-world-main">
          <strong class="portal-world-name">工作</strong>
          <p v-if="workRole" class="portal-work-role">{{ workRole }}</p>
          <ul v-if="workFocus.length" class="portal-work-focus">
            <li v-for="item in workFocus" :key="item">{{ item }}</li>
          </ul>
        </div>
      </NuxtLink>
    </div>

    <div class="portal-sill">
      <div class="portal-sill-life">
        <ClientOnly>
          <PortalVisitorMessage />
        </ClientOnly>
      </div>
      <p class="portal-sign">认真生活，持续创造</p>
    </div>
  </main>
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent } from 'vue'
import '~/assets/css/portal.css'
import { usePageSeo, useJsonLd, toAbsoluteUrl } from '~/composables/usePageSeo'

definePageMeta({ layout: false })

const PortalVisitorMessage = defineAsyncComponent(() => import('~/components/PortalVisitorMessage.vue'))

type PortalGlimpse = {
  life: {
    line: string
    now: Array<{ title: string, description: string }>
    note: { title: string, description: string } | null
  }
  work: {
    role: string
    focus: string[]
  }
}

const { data: glimpse } = await useAsyncData(
  'portal-glimpse',
  () => $fetch<PortalGlimpse>('/api/content/portal-glimpse'),
)

const lifeLine = computed(() => glimpse.value?.life.line || '')
const lifeNow = computed(() => glimpse.value?.life.now || [])
const lifeNote = computed(() => glimpse.value?.life.note)
const workRole = computed(() => glimpse.value?.work.role || '')
const workFocus = computed(() => glimpse.value?.work.focus || [])

usePageSeo({
  title: '溪午听风 - 生活与工作',
  description: '从这里进入生活，或进入工作与创造。',
  path: '/',
  world: 'portal',
})

useJsonLd(() => ({
  '@context': 'https://schema.org',
  '@type': 'WebSite',
  name: '溪午听风',
  url: toAbsoluteUrl('/'),
  description: '从这里进入生活，或进入工作与创造。',
}))
</script>
