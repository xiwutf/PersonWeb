<template>
  <section id="lab" class="home-section">
    <div class="home-shell">
      <div class="home-section-heading reveal-up">
        <p class="home-section-kicker">AI Lab</p>
        <h2>AI 实验室</h2>
        <p>以工程化方式探索 AI 能力的产品化落地——从模型调用到系统集成，持续验证真实场景。</p>
      </div>

      <div class="lab-grid reveal-up reveal-delay-1">
        <NuxtLink
          v-for="category in labCategories"
          :key="category.title"
          :to="category.link"
          class="lab-card"
        >
          <div class="lab-card-header">
            <span class="lab-card-icon" :style="{ color: category.color }">{{ category.icon }}</span>
            <span class="lab-card-title">{{ category.title }}</span>
          </div>
          <p class="lab-card-desc">{{ category.desc }}</p>
          <ul class="lab-experiments">
            <li v-for="exp in category.experiments" :key="exp">{{ exp }}</li>
          </ul>
          <div class="lab-card-footer">
            <span class="lab-link">查看实验 →</span>
          </div>
        </NuxtLink>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const labCategories = [
  {
    icon: '⬡',
    title: 'LLM 应用',
    desc: '基于大语言模型构建对话、摘要与内容生成系统。',
    experiments: ['多轮对话系统', '自动摘要 & 提炼', '内容生成管道', '意图识别分流'],
    color: '#6ea0ff',
    link: '/lab'
  },
  {
    icon: '◈',
    title: '知识工程',
    desc: '向量检索与 RAG 架构，让模型真正理解私有知识库。',
    experiments: ['RAG 文档问答', '向量索引优化', '多源知识融合', '企业知识搜索'],
    color: '#a78bfa',
    link: '/lab'
  },
  {
    icon: '⬢',
    title: '工作流自动化',
    desc: '以 AI 驱动流程节点，减少重复人工操作。',
    experiments: ['工单智能分流', 'AI 低代码编排', '数据清洗管道', '自动化报告生成'],
    color: '#34d399',
    link: '/lab'
  },
  {
    icon: '◉',
    title: '多智能体',
    desc: '多 Agent 协同框架，实现任务拆解、规划与工具调用。',
    experiments: ['任务规划 Agent', '工具调用链', 'ReAct 推理循环', 'Agent 评估框架'],
    color: '#fbbf24',
    link: '/lab'
  }
]
</script>

<style scoped>
.lab-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1.25rem;
}

.lab-card {
  display: flex;
  flex-direction: column;
  gap: 0.9rem;
  padding: 1.5rem;
  border: 1px solid var(--home-border);
  border-radius: var(--home-radius);
  background: var(--home-card);
  text-decoration: none;
  transition: transform 0.24s ease, border-color 0.24s ease, background 0.24s ease, box-shadow 0.24s ease;
}

.lab-card:hover {
  transform: translateY(-0.3rem);
  border-color: var(--home-border-strong);
  background: var(--home-card-hover);
  box-shadow: 0 16px 48px rgba(50, 90, 200, 0.14);
}

.lab-card-header {
  display: flex;
  align-items: center;
  gap: 0.65rem;
}

.lab-card-icon {
  font-size: 1.4rem;
  line-height: 1;
}

.lab-card-title {
  color: var(--home-text-main);
  font-size: 1rem;
  font-weight: 700;
}

.lab-card-desc {
  margin: 0;
  color: var(--home-text-muted);
  font-size: 0.88rem;
  line-height: 1.7;
}

.lab-experiments {
  margin: 0;
  padding: 0;
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 0.42rem;
  flex: 1;
}

.lab-experiments li {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--home-text-soft);
  font-size: 0.82rem;
}

.lab-experiments li::before {
  content: '–';
  color: var(--home-border-strong);
  flex-shrink: 0;
}

.lab-card-footer {
  padding-top: 0.75rem;
  border-top: 1px solid var(--home-border);
}

.lab-link {
  color: var(--home-accent);
  font-size: 0.85rem;
  font-weight: 660;
}

@media (max-width: 1024px) {
  .lab-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 580px) {
  .lab-grid {
    grid-template-columns: 1fr;
  }
}
</style>
