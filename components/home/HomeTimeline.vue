<template>
  <section id="cases" class="home-section">
    <div class="home-shell">
      <div class="home-section-heading reveal-up">
        <p class="home-section-kicker">Case Studies</p>
        <h2>案例精选</h2>
        <p>真实项目、真实挑战、真实结果——不是作品集，是工程师的解题记录。</p>
      </div>

      <div class="case-grid reveal-up reveal-delay-1">
        <article
          v-for="item in cases"
          :key="item.title"
          class="case-card"
        >
          <div class="case-card-header">
            <span class="case-type-badge">{{ item.type }}</span>
            <div class="case-tech-tags">
              <span v-for="tech in item.tech" :key="tech">{{ tech }}</span>
            </div>
          </div>

          <h3 class="case-title">{{ item.title }}</h3>

          <dl class="case-facts">
            <div class="case-fact">
              <dt>业务挑战</dt>
              <dd>{{ item.challenge }}</dd>
            </div>
            <div class="case-fact">
              <dt>解决方案</dt>
              <dd>{{ item.solution }}</dd>
            </div>
            <div class="case-fact case-fact-result">
              <dt>结果</dt>
              <dd>{{ item.result }}</dd>
            </div>
          </dl>

          <NuxtLink to="/projects" class="case-link">
            查看详情 →
          </NuxtLink>
        </article>
      </div>

      <div class="case-view-all reveal-up reveal-delay-2">
        <NuxtLink to="/projects" class="home-button home-button-secondary">
          查看全部案例 →
        </NuxtLink>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const cases = [
  {
    title: 'AI 工单智能分流系统',
    type: 'AI 应用',
    challenge: '客服工单量大、人工分类效率低，高峰期积压严重。',
    solution: '基于意图识别模型实现工单自动分类，结合知识库 RAG 给出处理建议，减少人工介入。',
    result: '分流准确率 91%，平均处理时长降低 38%。',
    tech: ['FastAPI', 'LLM', 'RAG', '.NET']
  },
  {
    title: '企业内部知识库问答平台',
    type: '知识工程',
    challenge: '企业文档分散、检索效率低，新员工上手成本高。',
    solution: '构建向量化知识索引，用 RAG 框架接入多源文档，支持自然语言问答与来源溯源。',
    result: '知识检索时间从分钟级降至秒级，员工满意度显著提升。',
    tech: ['向量数据库', 'RAG', 'Python', 'Nuxt3']
  },
  {
    title: '个人品牌数字化平台',
    type: '全栈产品',
    challenge: '内容分散于不同平台，缺乏统一入口与个人品牌沉淀。',
    solution: '基于 Nuxt3 + .NET 8 构建完整 CMS，整合博客、项目、工具与 AI 助手。',
    result: '统一内容管理，支持 SEO 优化与多主题切换，访问体验大幅提升。',
    tech: ['Nuxt3', 'Vue3', '.NET 8', 'MySQL']
  },
  {
    title: '小团队事务协作系统',
    type: '企业软件',
    challenge: '小团队缺少正式流程工具，任务追踪依赖微信群和 Excel。',
    solution: '开发轻量事务管理平台，支持任务分配、审批流与进度追踪，部署简单、运维低成本。',
    result: '上线后团队协作效率提升，工单丢失率接近 0。',
    tech: ['.NET 8', 'SQL Server', 'Vue3', 'Docker']
  }
]
</script>

<style scoped>
.case-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
}

.case-card {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1.6rem;
  border: 1px solid var(--home-border);
  border-radius: var(--home-radius);
  background: var(--home-card);
  transition: border-color 0.24s ease, background 0.24s ease;
}

.case-card:hover {
  border-color: var(--home-border-strong);
  background: var(--home-card-hover);
}

/* ── Header ── */
.case-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.case-type-badge {
  padding: 0.28rem 0.65rem;
  border-radius: 999px;
  border: 1px solid var(--home-border);
  color: var(--home-accent);
  font-size: 0.72rem;
  font-weight: 620;
  white-space: nowrap;
}

.case-tech-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.35rem;
}

.case-tech-tags span {
  padding: 0.24rem 0.52rem;
  border-radius: 999px;
  border: 1px solid var(--home-border);
  color: var(--home-text-soft);
  background: rgba(255, 255, 255, 0.028);
  font-size: 0.68rem;
}

/* ── Title ── */
.case-title {
  margin: 0;
  color: var(--home-text-main);
  font-size: 1.12rem;
  font-weight: 720;
  line-height: 1.35;
}

/* ── Facts ── */
.case-facts {
  display: flex;
  flex-direction: column;
  gap: 0.62rem;
  flex: 1;
}

.case-fact {
  display: grid;
  grid-template-columns: 5rem 1fr;
  gap: 0.5rem;
  align-items: baseline;
}

.case-fact dt {
  color: var(--home-text-soft);
  font-size: 0.75rem;
  font-weight: 600;
  white-space: nowrap;
}

.case-fact dd {
  margin: 0;
  color: var(--home-text-muted);
  font-size: 0.88rem;
  line-height: 1.65;
}

.case-fact-result dd {
  color: #34d399;
  font-weight: 580;
}

/* ── Link ── */
.case-link {
  color: var(--home-accent);
  font-size: 0.85rem;
  font-weight: 660;
  transition: color 0.2s ease;
}

.case-link:hover {
  color: var(--home-text-main);
}

/* ── View All ── */
.case-view-all {
  display: flex;
  justify-content: center;
  margin-top: 2.4rem;
}

@media (max-width: 860px) {
  .case-grid {
    grid-template-columns: 1fr;
  }
}
</style>
