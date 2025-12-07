<template>
  <div class="ai-lab-page">
    <!-- 背景特效 -->
    <div class="ai-lab-bg">
      <div class="ai-lab-bg-gradient-1"></div>
      <div class="ai-lab-bg-gradient-2"></div>
      <div class="ai-lab-bg-grid"></div>
    </div>

    <div class="ai-lab-container">
      <!-- 1. AI 能力概览 Hero -->
      <section class="ai-lab-hero">
        <div class="ai-lab-hero-content">
          <div class="ai-lab-hero-badge">
            <span class="ai-lab-hero-badge-dot"></span>
            <span>AI 实验室 · 智能体与自动化</span>
          </div>
          
          <h1 class="ai-lab-hero-title">
            AI 实验室
            <span class="ai-lab-hero-subtitle">
              智能体与自动化
            </span>
          </h1>
          
          <p class="ai-lab-hero-description">
            这里是我尝试用 AI 改造工作流、提升效率、做酷东西的地方，也是你可以把 AI 想法变成产品的起点。
          </p>

          <div class="ai-lab-hero-buttons">
            <a href="#projects" class="ai-lab-hero-button ai-lab-hero-button--primary">
              <span>查看 AI 项目案例</span>
              <i class="fas fa-arrow-down"></i>
            </a>
            <a href="#cooperation" class="ai-lab-hero-button ai-lab-hero-button--secondary">
              <span>我想做一个 AI 项目</span>
              <i class="fas fa-arrow-right"></i>
            </a>
          </div>
        </div>
      </section>

      <!-- 2. 智能体 / AI 项目案例列表 -->
      <section id="projects" class="ai-lab-projects">
        <div class="ai-lab-section-header">
          <h2 class="ai-lab-section-title">
            <i class="fas fa-robot"></i>
            智能体 / AI 项目案例
          </h2>
          <p class="ai-lab-section-description">
            以下是我实际开发或实验的 AI 项目，涵盖 RAG、多智能体、自动化等多个方向。
          </p>
        </div>
        <AiProjectList />
      </section>

      <!-- 3. AI 技术栈与工具区块 -->
      <section class="ai-lab-tech-stack">
        <div class="ai-lab-section-header">
          <h2 class="ai-lab-section-title">
            <i class="fas fa-tools"></i>
            我熟悉的 AI 技术栈
          </h2>
          <p class="ai-lab-section-description">
            不是只会"调用一个接口"，而是能完整落地 AI 系统的全栈能力。
          </p>
        </div>

        <div class="ai-lab-tech-grid">
          <div
            v-for="category in techStackCategories"
            :key="category.name"
            class="ai-lab-tech-card"
          >
            <div class="ai-lab-tech-card-header">
              <i :class="category.icon"></i>
              <h3 class="ai-lab-tech-card-title">{{ category.name }}</h3>
            </div>
            <div class="ai-lab-tech-card-tags">
              <span
                v-for="item in category.items"
                :key="item"
                class="ai-lab-tech-tag"
              >
                {{ item }}
              </span>
            </div>
          </div>
        </div>
      </section>

      <!-- 4. 合作流程 / 如何一起做 AI 项目 -->
      <section id="cooperation" class="ai-lab-cooperation">
        <div class="ai-lab-section-header">
          <h2 class="ai-lab-section-title">
            <i class="fas fa-handshake"></i>
            如果你想找我做一个 AI 项目
          </h2>
          <p class="ai-lab-section-description">
            从想法到落地，我会陪你走完整个流程。
          </p>
        </div>

        <div class="ai-lab-cooperation-steps">
          <div
            v-for="(step, index) in cooperationSteps"
            :key="index"
            class="ai-lab-cooperation-step"
          >
            <div class="ai-lab-cooperation-step-number">{{ index + 1 }}</div>
            <div class="ai-lab-cooperation-step-content">
              <h3 class="ai-lab-cooperation-step-title">{{ step.title }}</h3>
              <p class="ai-lab-cooperation-step-description">{{ step.description }}</p>
            </div>
            <div v-if="index < cooperationSteps.length - 1" class="ai-lab-cooperation-step-arrow">
              <i class="fas fa-arrow-down"></i>
            </div>
          </div>
        </div>

        <!-- 联系入口 -->
        <div class="ai-lab-cooperation-footer">
          <a href="/about" class="ai-lab-cooperation-button">
            <span>我有一个 AI 想法</span>
            <span class="ai-lab-cooperation-button-text">联系我</span>
            <i class="fas fa-arrow-right"></i>
          </a>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import AiProjectList from '~/components/ai/AiProjectList.vue'

definePageMeta({
  layout: 'ai'
})

useHead({
  title: 'AI 实验室 - 智能体与自动化 | 溪午听风',
  meta: [
    { name: 'description', content: '了解我的 AI 能力、查看项目案例、技术栈和合作流程。把 AI 想法变成产品。' }
  ]
})

// 技术栈分类
const techStackCategories = [
  {
    name: '模型与平台',
    icon: 'fas fa-brain',
    items: ['OpenAI API', '本地大模型', '各类国内模型', 'LangChain', 'LlamaIndex']
  },
  {
    name: '后端与服务编排',
    icon: 'fas fa-server',
    items: ['.NET', 'Java', 'Python (FastAPI)', 'Node.js', '微服务架构']
  },
  {
    name: '前端与交互',
    icon: 'fas fa-desktop',
    items: ['Vue3', 'Nuxt3', '对话式 UI', '前后端联动', '实时通信']
  },
  {
    name: '工具与能力',
    icon: 'fas fa-cog',
    items: ['RAG 检索', '向量数据库', '多智能体框架', 'API 调用', '任务调度']
  }
]

// 合作流程步骤
const cooperationSteps = [
  {
    title: '沟通需求',
    description: '简单聊聊你现在的场景和想解决的问题。'
  },
  {
    title: '方案设计',
    description: '我会给出可行性建议、技术路线和预估工作量。'
  },
  {
    title: '小步快跑实现',
    description: '先做一个可用的 MVP，再逐步打磨。'
  },
  {
    title: '交付与迭代',
    description: '根据你的反馈持续改进，让 AI 真正落地在你的业务里。'
  }
]
</script>

<style scoped>
.ai-lab-page {
  position: relative;
  min-height: 100vh;
  background: var(--color-bg-body, #050505);
  color: #fff;
  padding-top: 80px;
  padding-bottom: 80px;
}

/* 背景特效 */
.ai-lab-bg {
  position: fixed;
  inset: 0;
  pointer-events: none;
  z-index: 0;
  overflow: hidden;
}

.ai-lab-bg-gradient-1 {
  position: absolute;
  top: -20%;
  left: -10%;
  width: 600px;
  height: 600px;
  background: radial-gradient(circle, rgba(6, 182, 212, 0.15) 0%, transparent 70%);
  border-radius: 50%;
  filter: blur(80px);
}

.ai-lab-bg-gradient-2 {
  position: absolute;
  bottom: -20%;
  right: -10%;
  width: 600px;
  height: 600px;
  background: radial-gradient(circle, rgba(139, 92, 246, 0.15) 0%, transparent 70%);
  border-radius: 50%;
  filter: blur(80px);
}

.ai-lab-bg-grid {
  position: absolute;
  inset: 0;
  background-image: 
    linear-gradient(rgba(6, 182, 212, 0.1) 1px, transparent 1px),
    linear-gradient(90deg, rgba(6, 182, 212, 0.1) 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.3;
}

.ai-lab-container {
  position: relative;
  z-index: 1;
  max-width: 1280px;
  margin: 0 auto;
  padding: 0 24px;
}

/* Hero 区块 */
.ai-lab-hero {
  text-align: center;
  padding: 60px 0 80px;
  margin-bottom: 80px;
}

.ai-lab-hero-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  background: rgba(6, 182, 212, 0.1);
  border: 1px solid rgba(6, 182, 212, 0.3);
  border-radius: 999px;
  font-size: 14px;
  color: #06b6d4;
  margin-bottom: 24px;
}

.ai-lab-hero-badge-dot {
  width: 8px;
  height: 8px;
  background: #06b6d4;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.ai-lab-hero-title {
  font-size: 64px;
  font-weight: 700;
  line-height: 1.2;
  margin: 0 0 16px 0;
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.ai-lab-hero-subtitle {
  display: block;
  font-size: 32px;
  font-weight: 400;
  margin-top: 8px;
  color: rgba(255, 255, 255, 0.7);
}

@media (max-width: 640px) {
  .ai-lab-hero-title {
    font-size: 48px;
  }
  .ai-lab-hero-subtitle {
    font-size: 24px;
  }
}

.ai-lab-hero-description {
  font-size: 18px;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.7);
  max-width: 700px;
  margin: 0 auto 40px;
}

.ai-lab-hero-buttons {
  display: flex;
  gap: 16px;
  justify-content: center;
  flex-wrap: wrap;
}

.ai-lab-hero-button {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  padding: 16px 32px;
  border-radius: 12px;
  font-size: 16px;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.3s ease;
  cursor: pointer;
}

.ai-lab-hero-button--primary {
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 100%);
  color: #fff;
  box-shadow: 0 4px 16px rgba(6, 182, 212, 0.3);
}

.ai-lab-hero-button--primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(6, 182, 212, 0.4);
}

.ai-lab-hero-button--secondary {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: #fff;
}

.ai-lab-hero-button--secondary:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(6, 182, 212, 0.4);
}

/* 区块通用样式 */
.ai-lab-section-header {
  text-align: center;
  margin-bottom: 48px;
}

.ai-lab-section-title {
  font-size: 40px;
  font-weight: 600;
  margin: 0 0 16px 0;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  color: #fff;
}

.ai-lab-section-title i {
  color: #06b6d4;
}

.ai-lab-section-description {
  font-size: 16px;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.6);
  max-width: 600px;
  margin: 0 auto;
}

/* 项目案例区块 */
.ai-lab-projects {
  margin-bottom: 120px;
}

/* 技术栈区块 */
.ai-lab-tech-stack {
  margin-bottom: 120px;
}

.ai-lab-tech-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 24px;
}

.ai-lab-tech-card {
  padding: 24px;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  transition: all 0.3s ease;
}

.ai-lab-tech-card:hover {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(6, 182, 212, 0.3);
  transform: translateY(-4px);
}

.ai-lab-tech-card-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 20px;
}

.ai-lab-tech-card-header i {
  font-size: 24px;
  color: #06b6d4;
}

.ai-lab-tech-card-title {
  font-size: 18px;
  font-weight: 600;
  color: #fff;
  margin: 0;
}

.ai-lab-tech-card-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.ai-lab-tech-tag {
  padding: 6px 14px;
  background: rgba(6, 182, 212, 0.1);
  border: 1px solid rgba(6, 182, 212, 0.3);
  border-radius: 8px;
  font-size: 13px;
  color: #06b6d4;
}

/* 合作流程区块 */
.ai-lab-cooperation {
  margin-bottom: 80px;
}

.ai-lab-cooperation-steps {
  display: flex;
  flex-direction: column;
  gap: 32px;
  max-width: 800px;
  margin: 0 auto 60px;
}

.ai-lab-cooperation-step {
  position: relative;
  display: flex;
  align-items: flex-start;
  gap: 24px;
  padding: 32px;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  transition: all 0.3s ease;
}

.ai-lab-cooperation-step:hover {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(6, 182, 212, 0.3);
}

.ai-lab-cooperation-step-number {
  flex-shrink: 0;
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 100%);
  border-radius: 12px;
  font-size: 20px;
  font-weight: 700;
  color: #fff;
}

.ai-lab-cooperation-step-content {
  flex: 1;
}

.ai-lab-cooperation-step-title {
  font-size: 20px;
  font-weight: 600;
  color: #fff;
  margin: 0 0 8px 0;
}

.ai-lab-cooperation-step-description {
  font-size: 14px;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.6);
  margin: 0;
}

.ai-lab-cooperation-step-arrow {
  position: absolute;
  bottom: -40px;
  left: 50%;
  transform: translateX(-50%);
  color: rgba(6, 182, 212, 0.5);
  font-size: 20px;
}

.ai-lab-cooperation-step:last-child .ai-lab-cooperation-step-arrow {
  display: none;
}

.ai-lab-cooperation-footer {
  display: flex;
  justify-content: center;
}

.ai-lab-cooperation-button {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  padding: 18px 40px;
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 100%);
  border: none;
  border-radius: 12px;
  font-size: 18px;
  font-weight: 600;
  color: #fff;
  text-decoration: none;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 16px rgba(6, 182, 212, 0.3);
}

.ai-lab-cooperation-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(6, 182, 212, 0.4);
}

.ai-lab-cooperation-button-text {
  font-weight: 700;
}
</style>