<template>
  <article class="life-thought-card" :class="[`is-${section.visualType}`]">
    <div class="life-thought-card-meta">
      <p class="life-thought-card-index">{{ section.index }}</p>
      <h2 class="life-thought-card-title">{{ section.title }}</h2>
      <p v-if="section.description" class="life-thought-card-desc">{{ section.description }}</p>
      <p class="life-thought-card-count">{{ section.total }} 则</p>

      <figure
        v-if="section.visualType === 'photo' && section.cover"
        class="life-thought-card-visual is-photo"
      >
        <img :src="section.cover" :alt="`${section.title}配图`" loading="lazy">
      </figure>
      <div
        v-else-if="section.visualType === 'illustration'"
        class="life-thought-card-visual is-illustration"
        aria-hidden="true"
      >
        <span>{{ section.title.slice(0, 1) }}</span>
      </div>
    </div>

    <div class="life-thought-card-featured">
      <p v-if="section.featured" class="life-thought-card-quote">
        {{ section.featured.text }}
      </p>
      <p v-else class="life-thought-card-empty">还没有精选句子。</p>
      <p v-if="section.featured?.note" class="life-thought-card-note">{{ section.featured.note }}</p>
    </div>

    <div class="life-thought-card-side">
      <ul v-if="section.recommendations.length" class="life-thought-card-recs">
        <li v-for="item in section.recommendations" :key="item.id">
          <p>{{ item.text }}</p>
        </li>
      </ul>
      <NuxtLink
        :to="`/life/thoughts/${section.slug}`"
        class="life-thought-card-more"
      >
        更多{{ section.title }} →
      </NuxtLink>
    </div>
  </article>
</template>

<script setup lang="ts">
defineProps<{
  section: {
    slug: string
    index: string
    title: string
    description: string
    cover?: string
    visualType: 'photo' | 'illustration' | 'text-only'
    total: number
    featured: { id: string, text: string, note?: string } | null
    recommendations: Array<{ id: string, text: string }>
  }
}>()
</script>
