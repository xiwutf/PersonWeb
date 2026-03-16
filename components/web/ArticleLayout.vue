<template>
  <article class="article-layout" :class="props.class">
    <!-- 文章头部 -->
    <header class="article-header">
      <div v-if="tags && tags.length" class="article-tags">
        <span
          v-for="tag in tags"
          :key="tag"
          class="article-tag"
        >
          {{ tag }}
        </span>
      </div>
      <h1 class="article-title">{{ title }}</h1>
      <div class="article-meta">
        <span v-if="author" class="meta-item meta-author">
          <i class="fas fa-user"></i>
          {{ author }}
        </span>
        <span class="meta-item meta-date">
          <i class="fas fa-calendar"></i>
          {{ formatDate(date) }}
        </span>
        <span v-if="readTime" class="meta-item meta-readtime">
          <i class="fas fa-clock"></i>
          {{ readTime }}
        </span>
      </div>
    </header>

    <!-- 文章内容 -->
    <div class="article-content">
      <slot></slot>
    </div>

    <!-- 文章底部 -->
    <footer v-if="showFooter" class="article-footer">
      <!-- 上一篇/下一篇导航 -->
      <div v-if="prevArticle || nextArticle" class="article-nav">
        <a
          v-if="prevArticle"
          :href="`/blog/${prevArticle.slug}`"
          class="nav-link nav-link-prev"
        >
          <i class="fas fa-arrow-left"></i>
          <span>{{ prevArticle.title }}</span>
        </a>
        <a
          v-if="nextArticle"
          :href="`/blog/${nextArticle.slug}`"
          class="nav-link nav-link-next"
        >
          <span>{{ nextArticle.title }}</span>
          <i class="fas fa-arrow-right"></i>
        </a>
      </div>

      <!-- 分享按钮 -->
      <div class="article-share">
        <button
          v-for="social in socialLinks"
          :key="social.platform"
          class="share-button"
          :title="`分享到 ${social.name}`"
          @click="handleShare(social.platform)"
        >
          <i :class="social.icon"></i>
        </button>
      </div>
    </footer>
  </article>
</template>

<script setup lang="ts">

export interface NavArticle {
  slug: string
  title: string
}

export interface SocialLink {
  platform: string
  name: string
  icon: string
  url?: string
}

interface Props {
  title: string
  date?: string
  author?: string
  tags?: string[]
  readTime?: string
  prevArticle?: NavArticle
  nextArticle?: NavArticle
  showFooter?: boolean
  socialLinks?: SocialLink[]
  class?: string | string[] | Record<string, boolean>
}

const props = withDefaults(defineProps<Props>(), {
  showFooter: true
})

const emit = defineEmits<{
  (event: 'share', platform: string): void
}>()

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString()
}

const handleShare = (platform: string) => {
  emit('share', platform)
}
</script>

<style scoped>
.article-layout {
  max-width: 800px;
  margin: 0 auto;
  padding: var(--article-padding, var(--spacing-20) 0);
}

.article-header {
  margin-bottom: var(--spacing-2xl);
  text-align: center;
}

.article-tags {
  display: flex;
  justify-content: center;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
}

.article-tag {
  display: inline-flex;
  align-items: center;
  padding: var(--spacing-xs) var(--spacing-sm);
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  border-radius: var(--radius-full);
  font-size: var(--text-sm);
  font-weight: 500;
}

.article-title {
  font-size: var(--article-title-size, var(--text-3xl));
  font-weight: 700;
  color: var(--color-text-main);
  line-height: 1.3;
  margin: 0 0 var(--spacing-lg);
}

.article-meta {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: var(--spacing-lg);
  color: var(--color-text-muted);
  font-size: var(--text-sm);
}

.meta-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
}

.meta-item i {
  font-size: var(--text-xs);
}

.article-content {
  color: var(--article-text-color, var(--color-text-main));
  line-height: 1.8;
  font-size: var(--text-base);
}

.article-content :deep(h2) {
  font-size: var(--text-2xl);
  font-weight: 600;
  margin: var(--spacing-2xl) 0 var(--spacing-lg);
  color: var(--color-text-main);
}

.article-content :deep(h3) {
  font-size: var(--text-xl);
  font-weight: 600;
  margin: var(--spacing-xl) 0 var(--spacing-md);
  color: var(--color-text-main);
}

.article-content :deep(p) {
  margin: 0 0 var(--spacing-lg);
}

.article-content :deep(ul),
.article-content :deep(ol) {
  padding-left: var(--spacing-xl);
  margin: 0 0 var(--spacing-lg);
}

.article-content :deep(li) {
  margin: var(--spacing-md) 0;
}

.article-content :deep(code) {
  display: block;
  padding: var(--spacing-md);
  background: var(--color-bg-code, rgba(0, 0, 0, 0.3));
  border-radius: var(--radius-md);
  font-family: var(--font-code, 'SF Mono', Monaco, Consolas, monospace);
  font-size: var(--text-sm);
  overflow-x: auto;
}

.article-content :deep(pre) {
  background: var(--color-bg-code, rgba(0, 0, 0, 0.3));
  border-radius: var(--radius-md);
  padding: var(--spacing-md);
  overflow-x: auto;
}

.article-footer {
  margin-top: var(--spacing-3xl);
  padding-top: var(--spacing-2xl);
  border-top: 1px solid var(--color-border-subtle);
}

.article-nav {
  display: flex;
  justify-content: space-between;
  margin-bottom: var(--spacing-xl);
}

.nav-link {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-md) var(--spacing-xl);
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-default);
  border-radius: var(--radius-md);
  color: var(--color-text-main);
  text-decoration: none;
  transition: all 0.2s;
  max-width: 45%;
}

.nav-link:hover {
  background: var(--color-bg-elevated);
  border-color: var(--color-primary);
}

.nav-link span {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.article-share {
  display: flex;
  justify-content: center;
  gap: var(--spacing-md);
}

.share-button {
  width: 40px;
  height: 40px;
  border-radius: var(--radius-full);
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
}

.share-button:hover {
  background: var(--color-primary);
  color: var(--color-text-on-primary);
  border-color: var(--color-primary);
  transform: translateY(-2px);
}

/* Responsive */
@media (max-width: 768px) {
  .article-layout {
    padding: var(--spacing-lg) 0;
  }

  .article-title {
    font-size: var(--text-2xl);
  }

  .article-meta {
    flex-wrap: wrap;
  }

  .article-nav {
    flex-direction: column;
    gap: var(--spacing-md);
  }

  .nav-link {
    max-width: 100%;
  }
}
</style>
