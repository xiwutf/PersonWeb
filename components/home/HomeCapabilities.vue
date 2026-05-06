<template>
  <section id="capabilities" class="home-section home-capabilities-section">
    <div class="home-shell">
      <div class="home-section-heading reveal-up">
        <p class="home-section-kicker">AI Capability</p>
        <h2>AI 能力展示</h2>
        <p>以产品化方式组合模型、知识、工作流与业务数据，解决真实场景。</p>
      </div>

      <div class="capability-wall reveal-up reveal-delay-1" aria-label="AI 能力标签墙">
        <NuxtLink
          v-for="capability in capabilities"
          :key="capability"
          to="/ai"
          class="capability-chip"
        >
          {{ capability }}
        </NuxtLink>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const capabilities = [
  'AI 对话',
  'RAG',
  '向量检索',
  '文档问答',
  '工作流',
  '知识库',
  '内容生成',
  '数据分析',
  '工单系统',
  '多智能体',
  '企业搜索',
  '自动摘要',
  '意图识别',
  '低代码编排'
]
</script>

<style scoped>
.home-capabilities-section {
  padding-top: 5rem;
}

.capability-wall {
  display: flex;
  flex-wrap: wrap;
  gap: 0.85rem;
  max-width: 58rem;
}

.capability-chip {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 2.85rem;
  padding: 0.76rem 1.1rem;
  border: 1px solid var(--home-border);
  border-radius: 999px;
  color: var(--home-text-muted);
  background: rgba(255, 255, 255, 0.035);
  transition: transform 0.24s ease, border-color 0.24s ease, color 0.24s ease, box-shadow 0.24s ease, background 0.24s ease;
}

.capability-chip:hover {
  transform: translateY(-0.18rem);
  border-color: var(--home-border-strong);
  color: var(--home-text-main);
  background: rgba(82, 126, 255, 0.1);
  box-shadow: 0 0 32px rgba(64, 102, 255, 0.16);
}
</style>
