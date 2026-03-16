<template>
  <div class="life-page">
    <!-- ?????? -->
    <div class="life-background-noise"></div>

    <!-- ???????-->
    <div class="life-background-container">
      <div class="life-background-blob life-background-blob--amber"></div>
      <div class="life-background-blob life-background-blob--orange"></div>
    </div>

    <div class="life-content">
      <!-- ???? -->
      <header class="life-header">
        <div class="life-header-icon">
          <span>??</span>
        </div>
        <h1 class="life-title">????</h1>
        <p class="life-subtitle">
          "??????????????????????????
        </p>
      </header>

      <!-- ???? -->
      <div v-if="!posts || posts.length === 0" class="life-empty">
        <div class="life-empty-icon">??</div>
        <h3 class="life-empty-title">????</h3>
        <p class="life-empty-text">??????????????..</p>
      </div>

      <div v-else class="life-posts">
        <TransitionGroup name="life-list">
          <article
            v-for="(post, index) in posts"
            :key="post._path"
            class="life-post"
            :style="{ transitionDelay: `${index * 100}ms` }"
          >
            <!-- ????-->
            <div v-if="index !== posts.length - 1" class="life-post-connector"></div>

            <div class="life-post-container">
              <!-- ??????(???? -->
              <div class="life-post-dot"></div>

              <!-- ???? -->
              <div 
                class="life-post-card" 
                :class="index % 2 === 0 ? 'life-post-card--left' : 'life-post-card--right'"
              >
                <NuxtLink 
                  :to="post.path || post._path"
                  class="life-post-link"
                >
                  <!-- ????-->
                  <div v-if="post.cover" class="life-post-cover">
                    <img :src="post.cover" :alt="post.title" />
                    <div class="life-post-cover-overlay"></div>
                    <div class="life-post-cover-date">
                      {{ formatDate(post.date) }}
                    </div>
                  </div>
                  <div v-else class="life-post-header">
                    <div class="life-post-header-date">
                      {{ formatDate(post.date) }}
                    </div>
                  </div>

                  <div class="life-post-body">
                    <div class="life-post-category">
                      <span class="life-post-category-tag">
                        {{ post.category || '??' }}
                      </span>
                    </div>

                    <h2 class="life-post-title">
                      {{ post.title }}
                    </h2>
                    
                    <p class="life-post-description">
                      {{ post.description }}
                    </p>

                    <div class="life-post-footer">
                      <div class="life-post-tags">
                        <span v-for="tag in post.tags" :key="tag" class="life-post-tag">#{{ tag }}</span>
                      </div>
                      <span class="life-post-read-more">???? &rarr;</span>
                    </div>
                  </div>
                </NuxtLink>
              </div>
            </div>
          </article>
        </TransitionGroup>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// ???????????????
definePageMeta({
  layout: 'default'
})

const api = useApi()
const posts = ref<any[]>([])
const pending = ref(true)
const error = ref<string | null>(null)

// ?API????????
const fetchPosts = async () => {
  try {
    pending.value = true
    error.value = null
    
    // ???API??
    const res = await api.get<any[]>('/MockData/life-posts')
    if (res && res.length > 0) {
      posts.value = res.map(p => ({
        ...p,
        date: new Date(p.date)
      }))
      return
    }
    
    // ??API???????? @nuxt/content ???Content v3: queryCollection?
    const { data: contentPosts } = await useAsyncData('life-posts', () =>
      queryCollection('content').where('path', 'LIKE', '/life%').order('date', 'DESC').all()
    )
    
    if (contentPosts.value && Array.isArray(contentPosts.value) && contentPosts.value.length > 0) {
      posts.value = contentPosts.value
    } else {
      posts.value = []
    }
  } catch (e: any) {
    console.error('Failed to fetch life posts:', e)
    error.value = e.message || '????'
    posts.value = []
  } finally {
    pending.value = false
  }
}

onMounted(() => {
  fetchPosts()
})

// ?????
const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = dateString instanceof Date ? dateString : new Date(dateString)
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

useHead({
  title: '???? - ????',
  meta: [
    { name: 'description', content: '?????????????' }
  ]
})
</script>

<style scoped>
/* ??????????assets/css/life.css */
/* ??????????????????*/
</style>
