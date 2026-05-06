<template>
  <section id="timeline" class="home-section">
    <div class="home-shell timeline-shell">
      <div class="home-section-heading reveal-up">
        <p class="home-section-kicker">Latest Updates</p>
        <h2>最新动态</h2>
        <p>保留近期最重要的产品、项目与文章记录。</p>
      </div>

      <div class="timeline-list reveal-up reveal-delay-1">
        <article v-for="item in timelineItems" :key="item.title" class="timeline-item">
          <div class="timeline-dot"></div>
          <div class="timeline-meta">
            <time :datetime="item.date">{{ item.date }}</time>
            <span>{{ item.category }}</span>
          </div>
          <div>
            <h3>{{ item.title }}</h3>
            <p>{{ item.summary }}</p>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
interface TimelineItem {
  title: string
  date: string
  category: string
  summary: string
}

const timelineItems: TimelineItem[] = [
  {
    title: 'AI-Hub 产品体验升级',
    date: '2026-04-22',
    category: '产品更新',
    summary: '围绕工具聚合、智能导航和体验转化优化首页入口。'
  },
  {
    title: '个人网站品牌化重构',
    date: '2026-04-16',
    category: '项目日志',
    summary: '从博客集合转向个人品牌官网，突出产品、AI 能力与合作入口。'
  },
  {
    title: '企业数字化工具实践',
    date: '2026-03-21',
    category: '技术文章',
    summary: '总结从需求、系统设计到交付维护的完整产品化路径。'
  },
  {
    title: 'AI 工单处理系统设计',
    date: '2026-03-18',
    category: 'AI 应用',
    summary: '探索智能分流、自动摘要与知识库问答在服务流程中的落地。'
  }
]
</script>

<style scoped>
.timeline-shell {
  display: grid;
  grid-template-columns: minmax(18rem, 0.38fr) minmax(0, 0.62fr);
  gap: clamp(2rem, 6vw, 5rem);
  align-items: start;
}

.timeline-list {
  position: relative;
  display: grid;
}

.timeline-list::before {
  content: '';
  position: absolute;
  top: 0.8rem;
  bottom: 0.8rem;
  left: 0.43rem;
  width: 1px;
  background: linear-gradient(to bottom, transparent, var(--home-border-strong), transparent);
}

.timeline-item {
  position: relative;
  display: grid;
  grid-template-columns: 8.5rem minmax(0, 1fr);
  gap: 1.6rem;
  padding: 0 0 2.1rem 2.4rem;
}

.timeline-dot {
  position: absolute;
  top: 0.42rem;
  left: 0;
  width: 0.9rem;
  height: 0.9rem;
  border: 1px solid rgba(138, 172, 255, 0.72);
  border-radius: 50%;
  background: var(--home-bg);
  box-shadow: 0 0 18px rgba(75, 120, 255, 0.42);
}

.timeline-meta {
  display: flex;
  flex-direction: column;
  gap: 0.45rem;
  color: var(--home-text-soft);
  font-size: 0.82rem;
}

.timeline-meta span {
  width: fit-content;
  padding: 0.28rem 0.55rem;
  border: 1px solid var(--home-border);
  border-radius: 999px;
}

.timeline-item h3 {
  margin: 0;
  color: var(--home-text-main);
  font-size: 1.05rem;
  font-weight: 710;
}

.timeline-item p {
  margin: 0.6rem 0 0;
  color: var(--home-text-muted);
  line-height: 1.75;
}

@media (max-width: 900px) {
  .timeline-shell {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 640px) {
  .timeline-item {
    grid-template-columns: 1fr;
    gap: 0.75rem;
  }
}
</style>
