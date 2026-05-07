<template>
  <section id="writing" class="content-thinking-showcase">
    <div class="content-thinking-frame">
      <div class="thinking-stars" aria-hidden="true"></div>

      <div class="thinking-hero reveal-up">
        <div class="thinking-title-block">
          <span class="thinking-kicker">CONTENT &amp; THINKING</span>
          <h2>内容 <span>&amp; 思考</span></h2>
          <p>记录我的思考与实践，分享知识，启发思考，探索更好的解决方案。</p>
        </div>

        <blockquote class="thinking-quote">
          <span aria-hidden="true">“</span>
          <p>知识真正的价值不在于占有，<br>而在于连接与创造。</p>
          <cite>Xiuu</cite>
        </blockquote>
      </div>

      <div class="thinking-divider"></div>

      <div class="thinking-toolbar reveal-up reveal-delay-1">
        <div class="thinking-filters" aria-label="文章分类">
          <button
            v-for="filter in filters"
            :key="filter.label"
            type="button"
            :class="{ 'is-active': filter.label === '全部' }"
          >
            <span :class="filter.icon" aria-hidden="true"></span>
            {{ filter.label }}
          </button>
        </div>

        <NuxtLink to="/blog" class="thinking-more">
          查看全部文章
          <span aria-hidden="true">→</span>
        </NuxtLink>
      </div>

      <div class="thinking-main-grid reveal-up reveal-delay-2">
        <article class="featured-article">
          <span class="featured-badge">
            <span class="icon-star" aria-hidden="true"></span>
            精选文章
          </span>
          <div class="featured-orbit" aria-hidden="true">
            <span class="featured-cube"></span>
          </div>
          <template v-if="featuredArticle">
            <div class="featured-copy">
              <p>
                <span>{{ featuredArticle.categoryName || 'AI' }}</span>
                · {{ formatRelativeTime(featuredArticle.publishTime) }}
              </p>
              <h3>{{ featuredArticle.title }}</h3>
              <p>{{ featuredArticle.summary || '点击查看完整内容。' }}</p>
            </div>
            <div class="featured-meta">
              <span><span class="icon-eye" aria-hidden="true"></span>{{ featuredArticle.viewCount }}</span>
            </div>
            <NuxtLink :to="featuredArticle.path" class="featured-link">
              阅读文章
              <span aria-hidden="true">→</span>
            </NuxtLink>
          </template>
          <template v-else>
            <div class="featured-copy">
              <p><span>AI</span> · 持续更新中</p>
              <h3>探索 AI 与技术的边界</h3>
              <p>更多精选文章即将发布，敬请期待。</p>
            </div>
            <NuxtLink to="/blog" class="featured-link">
              浏览全部文章
              <span aria-hidden="true">→</span>
            </NuxtLink>
          </template>
          <span class="featured-next" aria-hidden="true">›</span>
        </article>

        <aside class="top-article-list" aria-label="热门文章">
          <NuxtLink
            v-for="(article, index) in articles.slice(0, 4)"
            :key="article.id"
            :to="article.path"
            class="top-article-item"
          >
            <span class="top-art" :class="getTopArt(index)" aria-hidden="true"></span>
            <span class="top-copy">
              <em>{{ article.categoryName || '文章' }} · {{ formatRelativeTime(article.publishTime) }}</em>
              <strong>{{ article.title }}</strong>
            </span>
            <span class="top-stats">
              <span><span class="icon-eye" aria-hidden="true"></span>{{ article.viewCount }}</span>
            </span>
          </NuxtLink>
        </aside>
      </div>

      <div class="latest-head reveal-up reveal-delay-3">
        <span></span>
        <strong>最新笔记</strong>
      </div>

      <div class="latest-notes reveal-up reveal-delay-3">
        <NuxtLink
          v-for="(article, index) in articles"
          :key="article.id"
          :to="article.path"
          class="note-card"
        >
          <span class="note-icon" :class="getNoteIcon(index)" aria-hidden="true"></span>
          <time>{{ formatShortDate(article.publishTime) }}</time>
          <strong>{{ article.title }}</strong>
          <p>{{ article.summary || '点击查看完整内容。' }}</p>
          <span class="note-meta">
            <span><span class="icon-eye" aria-hidden="true"></span>{{ article.viewCount }}</span>
            <span aria-hidden="true">→</span>
          </span>
        </NuxtLink>

        <button class="note-next" type="button" aria-label="下一组笔记">
          <span aria-hidden="true">›</span>
        </button>
      </div>

      <div class="thinking-bottom reveal-up reveal-delay-3">
        <span class="bottom-planet" aria-hidden="true"></span>
        <p>保持好奇，持续思考，长期主义，做难而正确的事。</p>
        <NuxtLink to="/blog">
          探索更多思考
          <span aria-hidden="true">→</span>
        </NuxtLink>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import type { HomeArticleCard } from '~/types/home'

const props = withDefaults(defineProps<{
  featuredArticle: HomeArticleCard | null
  articles: HomeArticleCard[]
  loading: boolean
}>(), {
  featuredArticle: null,
  articles: () => [],
  loading: false,
})

const formatRelativeTime = (dateStr: string | null): string => {
  if (!dateStr) return ''
  const diffDays = Math.floor((Date.now() - new Date(dateStr).getTime()) / 86400000)
  if (diffDays === 0) return '今天'
  if (diffDays < 30) return `${diffDays} 天前`
  if (diffDays < 365) return `${Math.floor(diffDays / 30)} 个月前`
  return `${Math.floor(diffDays / 365)} 年前`
}

const formatShortDate = (dateStr: string | null): string => {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  return `${String(d.getMonth() + 1).padStart(2, '0')}.${String(d.getDate()).padStart(2, '0')}`
}

const TOP_ARTS = ['top-art-planet', 'top-art-chart', 'top-art-path', 'top-art-en']
const NOTE_ICONS = ['note-icon-agent', 'note-icon-code', 'note-icon-book', 'note-icon-light']
const getTopArt = (index: number) => TOP_ARTS[index % TOP_ARTS.length]
const getNoteIcon = (index: number) => NOTE_ICONS[index % NOTE_ICONS.length]

const filters = [
  { label: '全部', icon: '' },
  { label: '技术', icon: 'icon-spark' },
  { label: '产品', icon: 'icon-cart' },
  { label: 'AI', icon: 'icon-ai' },
  { label: '财务', icon: 'icon-finance' },
  { label: '成长', icon: 'icon-growth' },
  { label: '随笔', icon: 'icon-pen' },
  { label: '英语', icon: 'icon-en' }
]
</script>

<style scoped>
.content-thinking-showcase {
  padding: 1.7rem 2.4rem 2.1rem;
  background:
    radial-gradient(circle at 28% 18%, rgba(53, 90, 180, 0.12), transparent 30rem),
    radial-gradient(circle at 76% 28%, rgba(86, 70, 182, 0.11), transparent 28rem),
    #020713;
}

.content-thinking-frame {
  position: relative;
  width: min(100%, 1840px);
  min-height: calc(100vh - 3.8rem);
  min-height: calc(100svh - 3.8rem);
  margin: 0 auto;
  padding: 8.45rem min(5.2vw, 6.1rem) 1.55rem;
  overflow: hidden;
  border: 1px solid var(--home-border);
  border-radius: 1.85rem;
  background:
    radial-gradient(circle at 36% 16%, rgba(50, 82, 170, 0.14), transparent 31rem),
    radial-gradient(circle at 62% 52%, rgba(38, 67, 160, 0.07), transparent 32rem),
    linear-gradient(180deg, rgba(4, 13, 34, 0.98), rgba(2, 8, 24, 0.985));
  box-shadow:
    0 28px 100px rgba(0, 0, 0, 0.34),
    inset 0 1px 0 rgba(255, 255, 255, 0.05);
}

.thinking-stars {
  position: absolute;
  inset: 7rem 7rem auto 7rem;
  height: 18rem;
  pointer-events: none;
  background-image:
    radial-gradient(circle, rgba(139, 166, 255, 0.55) 0 1px, transparent 1.4px),
    radial-gradient(circle, rgba(118, 98, 255, 0.38) 0 1px, transparent 1.4px);
  background-position: 0 0, 3.2rem 2.3rem;
  background-size: 8.2rem 5.2rem, 9.4rem 6.4rem;
  opacity: 0.56;
}

.thinking-hero,
.thinking-toolbar,
.thinking-main-grid,
.latest-notes,
.thinking-bottom {
  position: relative;
  z-index: 1;
}

.thinking-hero {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(23rem, 33rem);
  align-items: start;
  gap: 2.5rem;
}

.thinking-kicker {
  display: inline-flex;
  align-items: center;
  min-height: 1.7rem;
  padding: 0 0.75rem;
  border: 1px solid rgba(124, 151, 255, 0.22);
  border-radius: 999px;
  color: #a7b6ff;
  background: rgba(73, 90, 185, 0.12);
  font-size: 0.78rem;
  font-weight: 720;
  letter-spacing: 0.08em;
}

.thinking-title-block h2 {
  margin: 0.78rem 0 0;
  color: rgba(249, 251, 255, 0.98);
  font-size: clamp(2.35rem, 3.2vw, 3.6rem);
  line-height: 1.08;
  font-weight: 840;
}

.thinking-title-block h2 span {
  color: #9d8bff;
  text-shadow: 0 0 24px rgba(108, 104, 255, 0.38);
}

.thinking-title-block p {
  margin: 0.7rem 0 0;
  color: rgba(216, 226, 255, 0.72);
  font-size: 1.05rem;
}

.thinking-quote {
  position: relative;
  min-height: 7.55rem;
  margin: 0;
  padding: 1.6rem 2.1rem 1.2rem 3.3rem;
  border: 1px solid rgba(151, 179, 255, 0.14);
  border-radius: 1.25rem;
  background: linear-gradient(180deg, rgba(7, 18, 43, 0.58), rgba(5, 13, 34, 0.34));
}

.thinking-quote > span {
  position: absolute;
  left: 1.25rem;
  top: 1rem;
  color: rgba(142, 157, 230, 0.68);
  font-size: 3.6rem;
  line-height: 1;
  font-family: Georgia, serif;
}

.thinking-quote p {
  margin: 0;
  color: rgba(239, 244, 255, 0.9);
  font-size: 1.05rem;
  line-height: 1.85;
}

.thinking-quote cite {
  position: absolute;
  right: 4.2rem;
  bottom: -1rem;
  color: #a28cff;
  font-family: "Brush Script MT", "Segoe Script", cursive;
  font-size: 2.5rem;
  font-style: italic;
  font-weight: 300;
}

.thinking-divider {
  position: relative;
  z-index: 1;
  height: 1px;
  margin: 1.6rem 0 1.05rem;
  background: linear-gradient(90deg, transparent, rgba(140, 167, 255, 0.2), transparent);
}

.thinking-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1.5rem;
}

.thinking-filters {
  display: flex;
  align-items: center;
  gap: 1.45rem;
  overflow-x: auto;
  scrollbar-width: none;
}

.thinking-filters::-webkit-scrollbar {
  display: none;
}

.thinking-filters button,
.thinking-more {
  min-height: 3rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.55rem;
  border: 1px solid rgba(134, 162, 255, 0.2);
  border-radius: 1rem;
  color: rgba(222, 230, 255, 0.86);
  background: rgba(5, 14, 34, 0.5);
  font-size: 0.94rem;
  font-weight: 700;
  white-space: nowrap;
}

.thinking-filters button {
  min-width: 5.9rem;
  padding: 0 1.15rem;
}

.thinking-filters button.is-active {
  min-width: 4.6rem;
  color: #fff;
  border-color: rgba(130, 151, 255, 0.45);
  background: linear-gradient(135deg, #6486ff, #6454ff);
  box-shadow: 0 14px 34px rgba(72, 86, 255, 0.34);
}

.thinking-more {
  min-width: 11.8rem;
  border-radius: 999px;
  color: rgba(222, 232, 255, 0.92);
}

.thinking-main-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.4fr) minmax(24rem, 0.96fr);
  gap: 0.95rem;
  margin-top: 1.25rem;
}

.featured-article,
.top-article-list,
.note-card,
.thinking-bottom {
  border: 1px solid rgba(151, 179, 255, 0.16);
  border-radius: 1rem;
  background:
    linear-gradient(180deg, rgba(9, 21, 50, 0.74), rgba(4, 12, 31, 0.68)),
    rgba(5, 13, 32, 0.76);
  box-shadow:
    inset 0 1px 0 rgba(255, 255, 255, 0.05),
    0 18px 42px rgba(0, 0, 0, 0.18);
}

.featured-article {
  position: relative;
  min-height: 22rem;
  display: grid;
  grid-template-columns: minmax(18rem, 0.95fr) minmax(0, 1fr);
  gap: 1.35rem;
  overflow: hidden;
  padding: 1.15rem 1.7rem 1.35rem;
}

.featured-badge {
  position: absolute;
  left: 1.25rem;
  top: 1.1rem;
  z-index: 2;
  display: inline-flex;
  align-items: center;
  gap: 0.48rem;
  padding: 0.5rem 0.72rem;
  border: 1px solid rgba(140, 166, 255, 0.18);
  border-radius: 0.55rem;
  color: rgba(231, 237, 255, 0.9);
  background: rgba(10, 23, 54, 0.72);
  font-size: 0.82rem;
}

.featured-orbit {
  position: relative;
  min-height: 18.6rem;
  align-self: center;
  border-radius: 1rem;
  background:
    radial-gradient(ellipse at center, rgba(83, 88, 255, 0.48) 0 1.2rem, rgba(48, 74, 191, 0.22) 5rem, transparent 9rem),
    radial-gradient(ellipse at bottom, rgba(30, 76, 198, 0.42), transparent 12rem);
}

.featured-orbit::before,
.featured-orbit::after {
  content: '';
  position: absolute;
  left: 50%;
  top: 58%;
  width: 16rem;
  height: 5.3rem;
  border: 1px solid rgba(100, 118, 255, 0.55);
  border-radius: 50%;
  transform: translate(-50%, -50%);
  box-shadow: 0 0 30px rgba(94, 86, 255, 0.35);
}

.featured-orbit::after {
  width: 12.5rem;
  height: 3.4rem;
  border-color: rgba(50, 136, 255, 0.54);
}

.featured-cube {
  position: absolute;
  left: 50%;
  top: 46%;
  width: 7.5rem;
  height: 7.5rem;
  border: 1px solid rgba(155, 183, 255, 0.6);
  background:
    linear-gradient(135deg, rgba(185, 211, 255, 0.65), rgba(72, 98, 255, 0.18)),
    rgba(57, 86, 201, 0.36);
  transform: translate(-50%, -50%) rotate(45deg) skew(-8deg, -8deg);
  box-shadow:
    0 0 45px rgba(99, 105, 255, 0.7),
    inset 0 0 36px rgba(209, 229, 255, 0.44);
}

.featured-copy {
  grid-column: 2;
  align-self: center;
  padding-top: 0.35rem;
}

.featured-copy p:first-child {
  display: flex;
  align-items: center;
  gap: 0.82rem;
  margin: 0 0 0.65rem;
  color: rgba(181, 197, 244, 0.68);
  font-size: 0.94rem;
}

.featured-copy p:first-child span {
  padding: 0.32rem 0.56rem;
  border: 1px solid rgba(130, 152, 255, 0.18);
  border-radius: 0.48rem;
  color: #c7d1ff;
  background: rgba(111, 114, 255, 0.14);
}

.featured-copy h3 {
  margin: 0;
  color: rgba(249, 251, 255, 0.98);
  font-size: clamp(2rem, 2.55vw, 3rem);
  line-height: 1.35;
  font-weight: 840;
}

.featured-copy p:last-child {
  max-width: 29rem;
  margin: 1.15rem 0 0;
  color: rgba(214, 225, 255, 0.68);
  font-size: 1rem;
  line-height: 1.75;
}

.featured-meta {
  position: absolute;
  left: calc(50% + 2.8rem);
  bottom: 1.7rem;
  display: flex;
  gap: 1.65rem;
  color: rgba(199, 214, 255, 0.74);
  font-size: 0.88rem;
}

.featured-meta span,
.top-stats span,
.note-meta span {
  display: inline-flex;
  align-items: center;
  gap: 0.42rem;
}

.featured-link {
  position: absolute;
  right: 1.7rem;
  bottom: 1.25rem;
  min-height: 3rem;
  display: inline-flex;
  align-items: center;
  gap: 0.66rem;
  padding: 0 1.25rem;
  border: 1px solid rgba(137, 164, 255, 0.18);
  border-radius: 999px;
  color: rgba(231, 237, 255, 0.94);
  background: rgba(255, 255, 255, 0.035);
  font-weight: 720;
}

.featured-next {
  position: absolute;
  right: 1.55rem;
  top: 1.35rem;
  color: #9fb0ff;
  font-size: 2rem;
  line-height: 1;
}

.top-article-list {
  padding: 1.05rem 1.55rem;
}

.top-article-item {
  min-height: 5.35rem;
  display: grid;
  grid-template-columns: 5rem minmax(0, 1fr) auto;
  align-items: center;
  gap: 1.1rem;
  color: inherit;
  border-bottom: 1px solid rgba(150, 174, 255, 0.13);
}

.top-article-item:last-child {
  border-bottom: 0;
}

.top-art {
  width: 4.6rem;
  height: 4.6rem;
  border: 1px solid rgba(139, 165, 255, 0.2);
  border-radius: 0.55rem;
  background:
    radial-gradient(circle at 50% 50%, rgba(104, 92, 255, 0.78), transparent 1.6rem),
    linear-gradient(135deg, rgba(82, 106, 255, 0.35), rgba(4, 14, 38, 0.96));
}

.top-art-chart {
  background:
    linear-gradient(135deg, transparent 0 42%, rgba(95, 222, 255, 0.85) 43% 47%, transparent 48%),
    repeating-linear-gradient(90deg, rgba(160, 91, 255, 0.65) 0 0.2rem, transparent 0.2rem 0.78rem),
    rgba(7, 16, 42, 0.92);
}

.top-art-path {
  background:
    radial-gradient(circle at 50% 86%, rgba(158, 180, 255, 0.75) 0 0.38rem, transparent 0.42rem),
    linear-gradient(180deg, rgba(75, 41, 180, 0.5), rgba(5, 17, 47, 0.96));
}

.top-art-en {
  display: grid;
  place-items: center;
  background: linear-gradient(135deg, rgba(41, 80, 200, 0.4), rgba(5, 15, 40, 0.94));
}

.top-art-en::before {
  content: 'EN';
  color: #6684ff;
  font-size: 1.72rem;
  font-weight: 840;
}

.top-copy {
  min-width: 0;
  display: grid;
  gap: 0.55rem;
}

.top-copy em {
  color: #80a2ff;
  font-size: 0.92rem;
  font-style: normal;
}

.top-copy strong {
  overflow: hidden;
  color: rgba(248, 251, 255, 0.95);
  font-size: 1rem;
  font-weight: 760;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.top-stats {
  display: flex;
  gap: 1.2rem;
  color: rgba(190, 206, 255, 0.72);
  font-size: 0.88rem;
}

.latest-head {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-top: 1.35rem;
  color: rgba(248, 251, 255, 0.94);
  font-size: 1.05rem;
}

.latest-head span {
  width: 3px;
  height: 1.35rem;
  border-radius: 999px;
  background: #627fff;
}

.latest-notes {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 1.05rem;
  margin-top: 1rem;
}

.note-card {
  min-height: 11.05rem;
  display: grid;
  grid-template-columns: 3.8rem minmax(0, 1fr);
  grid-template-rows: auto auto 1fr auto;
  gap: 0.46rem 0.9rem;
  padding: 1.25rem 1.4rem 1rem;
  color: inherit;
}

.note-icon {
  grid-row: 1 / 3;
  width: 3.15rem;
  height: 3.15rem;
  border: 1px solid rgba(143, 169, 255, 0.18);
  border-radius: 0.62rem;
  background:
    radial-gradient(circle at 50% 50%, rgba(117, 89, 255, 0.55), transparent 1.3rem),
    rgba(9, 21, 50, 0.92);
}

.note-icon-agent {
  background:
    radial-gradient(circle at 50% 50%, rgba(205, 85, 255, 0.45), transparent 1.35rem),
    rgba(30, 12, 58, 0.9);
}

.note-icon-code {
  background:
    linear-gradient(135deg, rgba(54, 105, 220, 0.5), rgba(6, 18, 45, 0.94));
}

.note-icon-book {
  background:
    linear-gradient(135deg, rgba(52, 211, 153, 0.45), rgba(6, 31, 38, 0.94));
}

.note-icon-light {
  background:
    radial-gradient(circle at 50% 48%, rgba(255, 207, 91, 0.75), transparent 1rem),
    rgba(45, 27, 7, 0.9);
}

.note-card time {
  align-self: center;
  color: rgba(183, 199, 244, 0.68);
  font-size: 0.92rem;
}

.note-card strong {
  color: rgba(249, 251, 255, 0.95);
  font-size: 1.02rem;
  line-height: 1.35;
}

.note-card p {
  grid-column: 1 / -1;
  margin: 0.45rem 0 0;
  color: rgba(208, 220, 255, 0.62);
  font-size: 0.9rem;
  line-height: 1.62;
}

.note-meta {
  grid-column: 1 / -1;
  display: flex;
  align-items: center;
  gap: 1.2rem;
  margin-top: 0.55rem;
  color: rgba(190, 206, 255, 0.72);
  font-size: 0.86rem;
}

.note-meta > span:last-child {
  margin-left: auto;
  color: #8ea4ff;
}

.note-next {
  position: absolute;
  right: min(3.8vw, 4.8rem);
  bottom: 12.55rem;
  width: 3.35rem;
  height: 3.35rem;
  display: grid;
  place-items: center;
  border: 1px solid rgba(140, 166, 255, 0.18);
  border-radius: 999px;
  color: #a6b5ff;
  background: rgba(6, 16, 42, 0.62);
  font-size: 2rem;
}

.thinking-bottom {
  min-height: 4.95rem;
  display: grid;
  grid-template-columns: 5rem minmax(0, 1fr) auto;
  align-items: center;
  gap: 1.2rem;
  margin-top: 1.55rem;
  padding: 0.8rem 1.6rem;
}

.bottom-planet {
  position: relative;
  width: 3.8rem;
  height: 3.8rem;
  border-radius: 50%;
  background: radial-gradient(circle at 36% 32%, #718aff, #1d2b7d 64%, #07113a);
  box-shadow: 0 0 24px rgba(84, 112, 255, 0.36);
}

.bottom-planet::before {
  content: '';
  position: absolute;
  left: -0.65rem;
  top: 1.42rem;
  width: 5rem;
  height: 1.05rem;
  border: 1px solid rgba(92, 126, 255, 0.56);
  border-radius: 50%;
  transform: rotate(-9deg);
}

.thinking-bottom p {
  margin: 0;
  color: rgba(241, 246, 255, 0.9);
  font-size: 1.08rem;
  font-weight: 650;
}

.thinking-bottom a {
  display: inline-flex;
  align-items: center;
  gap: 0.65rem;
  color: #a5b6ff;
  font-weight: 700;
  white-space: nowrap;
}

.thinking-filters span,
.icon-eye,
.icon-comment,
.icon-star {
  position: relative;
  width: 1rem;
  height: 1rem;
  display: inline-block;
  color: currentColor;
}

.icon-eye::before {
  content: '';
  position: absolute;
  inset: 0.16rem 0.02rem;
  border: 1px solid currentColor;
  border-radius: 50% / 60%;
}

.icon-eye::after {
  content: '';
  position: absolute;
  left: 0.38rem;
  top: 0.38rem;
  width: 0.24rem;
  height: 0.24rem;
  border-radius: 50%;
  background: currentColor;
}

.icon-comment::before {
  content: '';
  position: absolute;
  inset: 0.16rem 0.12rem 0.24rem;
  border: 1px solid currentColor;
  border-radius: 0.22rem;
}

.icon-comment::after {
  content: '';
  position: absolute;
  left: 0.34rem;
  bottom: 0.08rem;
  width: 0.3rem;
  height: 0.3rem;
  border-left: 1px solid currentColor;
  border-bottom: 1px solid currentColor;
  transform: skew(-18deg);
}

.icon-star::before {
  content: '☆';
  position: absolute;
  inset: -0.18rem 0 0;
  font-size: 1.12rem;
}

.thinking-filters span::before {
  position: absolute;
  inset: 0;
  display: grid;
  place-items: center;
  font-size: 0.92rem;
}

.icon-spark::before { content: '⌁'; }
.icon-cart::before { content: '⌘'; }
.icon-ai::before { content: '✺'; }
.icon-finance::before { content: '♙'; }
.icon-growth::before { content: '◆'; }
.icon-pen::before { content: '□'; }
.icon-en::before { content: '♛'; }

@media (max-width: 1280px) {
  .thinking-main-grid,
  .thinking-hero {
    grid-template-columns: 1fr;
  }

  .featured-article {
    grid-template-columns: 1fr;
  }

  .featured-copy {
    grid-column: 1;
  }

  .featured-meta {
    left: 1.7rem;
  }

  .latest-notes {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 860px) {
  .content-thinking-showcase {
    padding: 0.8rem;
  }

  .content-thinking-frame {
    padding: 6.2rem 1rem 1rem;
    border-radius: 1.2rem;
  }

  .thinking-toolbar,
  .thinking-bottom {
    align-items: stretch;
    grid-template-columns: 1fr;
    flex-direction: column;
  }

  .top-article-item {
    grid-template-columns: 4.2rem minmax(0, 1fr);
  }

  .top-stats {
    grid-column: 2;
  }

  .latest-notes {
    grid-template-columns: 1fr;
  }

  .note-next {
    display: none;
  }
}
</style>
