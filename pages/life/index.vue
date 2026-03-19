<template>
  <div class="life-page">
    <div class="life-background-container" aria-hidden="true">
      <div class="life-background-blob life-background-blob--amber"></div>
      <div class="life-background-blob life-background-blob--orange"></div>
    </div>
    <div class="life-background-noise" aria-hidden="true"></div>

    <div class="life-content">
      <header class="life-header">
        <div class="life-header-icon">
          <span>☕</span>
        </div>
        <h1 class="life-title">生活随笔</h1>
        <p class="life-subtitle">
          记录生活点滴，分享思考感悟，在代码之外寻找诗与远方。
        </p>
      </header>

      <section v-if="!posts || posts.length === 0" class="life-empty">
        <div class="life-empty-icon">🍃</div>
        <h2 class="life-empty-title">暂无随笔</h2>
        <p class="life-empty-text">博主正在酝酿第一篇生活感悟...</p>
      </section>

      <section v-else>
        <div class="life-posts">
          <article
            v-for="(post, index) in posts"
            :key="post._path"
            class="life-post"
          >
            <div
              v-if="index !== posts.length - 1"
              class="life-post-connector"
              aria-hidden="true"
            ></div>
            <div class="life-post-dot" aria-hidden="true"></div>

            <div class="life-post-container">
              <div
                class="life-post-card"
                :class="index % 2 === 0 ? 'life-post-card--left' : 'life-post-card--right'"
              >
                <NuxtLink :to="post._path" class="life-post-link">
                  <div v-if="post.cover" class="life-post-cover">
                    <img :src="post.cover" :alt="post.title" />
                    <div class="life-post-cover-overlay"></div>
                    <div class="life-post-cover-date">{{ formatDate(post.date) }}</div>
                  </div>

                  <div v-else class="life-post-header">
                    <div class="life-post-header-date">{{ formatDate(post.date) }}</div>
                  </div>

                  <div class="life-post-body">
                    <div class="life-post-category">
                      <span class="life-post-category-tag">{{ post.category || '随笔' }}</span>
                    </div>

                    <h2 class="life-post-title">{{ post.title }}</h2>
                    <p class="life-post-description">{{ post.description }}</p>

                    <div class="life-post-footer">
                      <div v-if="post.tags?.length" class="life-post-tags">
                        <span
                          v-for="tag in post.tags"
                          :key="tag"
                          class="life-post-tag"
                        >
                          #{{ tag }}
                        </span>
                      </div>
                      <div v-else></div>

                      <span class="life-post-read-more">
                        阅读全文 →
                      </span>
                    </div>
                  </div>
                </NuxtLink>
              </div>
            </div>
          </article>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import '~/assets/css/life.css'

const { data: posts } = await useAsyncData('life-posts', () =>
  queryContent('/life').sort({ date: -1 }).find()
)

const formatDate = (dateString) => {
  if (!dateString) return '待更新'

  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

useHead({
  title: '生活随笔 - 溪午听风',
  meta: [
    { name: 'description', content: '记录生活点滴，分享思考感悟，在代码之外寻找诗与远方。' }
  ]
})
</script>
