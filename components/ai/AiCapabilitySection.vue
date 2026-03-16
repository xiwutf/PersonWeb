<template>
  <section class="ai-capability-section">
    <!-- 背景装饰 -->
    <div class="ai-capability-bg">
      <div class="ai-capability-bg-gradient"></div>
      <div class="ai-capability-bg-grid"></div>
    </div>

    <div class="ai-capability-container">
      <!-- 上半部分：文案介绍 + 流程图 -->
      <div class="ai-capability-header">
        <div class="ai-capability-header-left">
          <!-- 标签 -->
          <div class="ai-capability-badge">
            <span class="ai-capability-badge-dot"></span>
            <span>AI 能力与智能体服务</span>
          </div>

          <!-- 主标题 -->
          <h2 class="ai-capability-title">
            AI 能力与智能体服务
          </h2>

          <!-- 副标题 -->
          <p class="ai-capability-subtitle">
            我不是只会调 API 的"AI 玩家"，而是能帮你把想法落地成真正可用的 AI 系统。
          </p>

          <!-- 三个卖点卡片 -->
          <div class="ai-capability-features">
            <div 
              v-for="(feature, index) in features" 
              :key="index"
              class="ai-capability-feature-card"
            >
              <div class="ai-capability-feature-icon">
                <i :class="feature.icon"></i>
              </div>
              <div class="ai-capability-feature-content">
                <h3 class="ai-capability-feature-title">{{ feature.title }}</h3>
                <p class="ai-capability-feature-text">{{ feature.description }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- 右侧流程图/卡片 -->
        <div class="ai-capability-header-right">
          <div class="ai-capability-visual-card">
            <div class="ai-capability-visual-card-bg"></div>
            <div class="ai-capability-visual-content">
              <div class="ai-capability-visual-item">
                <div class="ai-capability-visual-icon">🤖</div>
                <p class="ai-capability-visual-text">智能体设计</p>
              </div>
              <div class="ai-capability-visual-arrow">→</div>
              <div class="ai-capability-visual-item">
                <div class="ai-capability-visual-icon">🔧</div>
                <p class="ai-capability-visual-text">系统开发</p>
              </div>
              <div class="ai-capability-visual-arrow">→</div>
              <div class="ai-capability-visual-item">
                <div class="ai-capability-visual-icon">🚀</div>
                <p class="ai-capability-visual-text">落地部署</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 下半部分：可承接项目类型卡片 -->
      <div class="ai-capability-projects">
        <h3 class="ai-capability-projects-title">可承接的 AI 项目类型</h3>
        <div class="ai-capability-projects-grid">
          <div 
            v-for="(project, index) in projectTypes" 
            :key="index"
            class="ai-capability-project-card"
            @click="handleProjectCardClick(project)"
          >
            <div class="ai-capability-project-card-bg"></div>
            <div class="ai-capability-project-card-content">
              <h4 class="ai-capability-project-title">{{ project.title }}</h4>
              <p class="ai-capability-project-description">{{ project.description }}</p>
              <div class="ai-capability-project-tags">
                <span 
                  v-for="tag in project.tags" 
                  :key="tag"
                  class="ai-capability-project-tag"
                >
                  #{{ tag }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 底部按钮 -->
      <div class="ai-capability-footer">
        <NuxtLink to="/ai" class="ai-capability-button">
          <span>了解我的 AI 能力</span>
          <i class="fas fa-arrow-right"></i>
        </NuxtLink>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// 三个核心卖点
const features = ref([
  {
    icon: 'fas fa-users',
    title: '多智能体工作流设计',
    description: '可为个人或小团队设计多智能体协作流程，让 AI 帮你自动整理信息、生成方案、执行任务。'
  },
  {
    icon: 'fas fa-database',
    title: 'RAG 知识库问答系统',
    description: '支持接入你的文档/知识库，实现"像问人一样问资料"的智能检索与问答。'
  },
  {
    icon: 'fas fa-cogs',
    title: '业务自动化与脚本代理',
    description: '结合 AI + 脚本/接口，自动化重复业务流程，比如日报生成、数据分析、邮件回复等。'
  }
])

// 可承接项目类型
const projectTypes = ref([
  {
    title: '个人/团队知识库智能助手',
    description: '基于 RAG 技术，将你的文档、笔记转化为可对话的智能助手',
    tags: ['RAG', '知识库', '问答系统']
  },
  {
    title: '面向业务的 AI 表单 / 客服机器人',
    description: '结合业务场景，打造智能客服、表单填写助手等实用工具',
    tags: ['智能体', '业务自动化']
  },
  {
    title: '开发者工具类 AI 插件',
    description: '为 VSCode、浏览器等开发工具开发 AI 增强插件，提升开发效率',
    tags: ['插件工具', '开发者工具']
  },
  {
    title: '现有系统接入大模型',
    description: '为现有系统接入对话、生成、分析等 AI 能力，实现智能化升级',
    tags: ['系统集成', 'API 接入']
  }
])

// 处理项目卡片点击
const handleProjectCardClick = (project: any) => {
  // 可以跳转到 AI 实验室页面，并高亮相关项目
  navigateTo('/ai')
}
</script>

<style scoped>
.ai-capability-section {
  position: relative;
  padding: var(--spacing-20) 0;
  overflow: hidden;
  /* 背景透明，融入全局背景 */
  background: rgba(255, 255, 255, 0.02);
  backdrop-filter: blur(var(--spacing-lg));
  border-top: 1px solid var(--border-subtle);
  border-bottom: 1px solid var(--border-subtle);
}

.ai-capability-bg {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
  opacity: 0.5; /* 降低不透明度 */
}

.ai-capability-bg-gradient {
  position: absolute;
  top: -50%;
  left: -20%;
  width: 800px;
  height: 800px;
  background: radial-gradient(circle, var(--color-info-soft) 0%, transparent 70%);
  border-radius: 50%;
  filter: blur(calc(var(--spacing-xl) * 2));
}

.ai-capability-bg-grid {
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(var(--color-info-10) 1px, transparent 1px),
    linear-gradient(90deg, var(--color-cyan-500-10, rgba(6, 182, 212, 0.1)) 1px, transparent 1px);
  background-size: var(--spacing-xl) var(--spacing-xl);
  opacity: 0.3;
}

.ai-capability-container {
  position: relative;
  z-index: 1;
  max-width: 1280px;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

/* 上半部分：文案 + 流程图 */
.ai-capability-header {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-16);
  margin-bottom: var(--spacing-20);
  align-items: start;
}

@media (max-width: 968px) {
  .ai-capability-header {
    grid-template-columns: 1fr;
    gap: var(--spacing-xl);
  }
}

.ai-capability-header-left {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.ai-capability-badge {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-sm) var(--spacing-md);
  border-radius: 9999px;
  font-size: var(--text-sm);
  width: fit-content;
  /* 浅色主题优化（2025-01）：使用主色柔和背景，提高可读性 */
  background: var(--color-primary-10);
  border: 1px solid var(--color-primary-20);
  color: var(--color-info);
}

.ai-capability-badge-dot {
  width: var(--spacing-sm);
  height: var(--spacing-sm);
  border-radius: 50%;
  animation: pulse 2s infinite;
  /* 浅色主题优化（2025-01）：使用主色 */
  background: var(--color-info);
}

/* 深色主题保持原有样式 */
:global(.dark) .ai-capability-badge,
:global([data-theme="dark"]) .ai-capability-badge {
  background: var(--color-info-10);
  border: 1px solid var(--color-info-30);
  color: var(--color-info);
}

:global(.dark) .ai-capability-badge-dot,
:global([data-theme="dark"]) .ai-capability-badge-dot {
  background: var(--color-info);
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.ai-capability-title {
  font-size: var(--text-6xl);
  font-weight: 700;
  line-height: 1.2;
  margin: 0;
  /* 浅色主题优化（2025-01）：使用主题文字颜色，避免渐变在浅色背景下看不清 */
  color: var(--color-text-main);
}

/* 深色主题使用渐变效果 */
html[data-theme="dark"] .ai-capability-title {
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  color: transparent;
}

@media (max-width: 640px) {
  .ai-capability-title {
    font-size: var(--text-4xl);
  }
}

.ai-capability-subtitle {
  font-size: var(--text-2xl);
  line-height: 1.6;
  margin: 0;
  /* 浅色主题优化（2025-01）：使用主题文字颜色，确保清晰 */
  color: var(--color-text-secondary);
}

.ai-capability-features {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  margin-top: var(--spacing-sm);
}

.ai-capability-feature-card {
  display: flex;
  gap: var(--spacing-lg);
  padding: var(--spacing-lg);
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-lg);
  transition: all 0.3s ease;
}

.ai-capability-feature-card:hover {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(6, 182, 212, 0.3);
  transform: translateY(-4px);
}

.ai-capability-feature-icon {
  flex-shrink: 0;
  width: var(--spacing-xl);
  height: var(--spacing-xl);
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(6, 182, 212, 0.1);
  border-radius: var(--radius-md);
  color: var(--color-primary);
  font-size: var(--text-2xl);
}

.ai-capability-feature-content {
  flex: 1;
}

.ai-capability-feature-title {
  font-size: var(--text-2xl);
  font-weight: 600;
  margin: 0 0 var(--spacing-sm) 0;
  /* 浅色主题优化（2025-01）：使用主题文字颜色，确保清晰 */
  color: var(--color-text-main);
}

.ai-capability-feature-text {
  font-size: var(--text-sm);
  line-height: 1.6;
  margin: 0;
  /* 浅色主题优化（2025-01）：使用主题文字颜色，确保清晰 */
  color: var(--color-text-secondary);
}

/* 右侧流程图 */
.ai-capability-header-right {
  display: flex;
  align-items: center;
  justify-content: center;
}

.ai-capability-visual-card {
  position: relative;
  width: 100%;
  max-width: 400px;
  padding: var(--spacing-xl);
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-2xl);
  backdrop-filter: blur(var(--spacing-lg));
}

.ai-capability-visual-card-bg {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(6, 182, 212, 0.1) 0%, rgba(139, 92, 246, 0.1) 100%);
  border-radius: var(--radius-2xl);
  opacity: 0.5;
}

.ai-capability-visual-content {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-lg);
}

.ai-capability-visual-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-sm);
  flex: 1;
}

.ai-capability-visual-icon {
  width: var(--spacing-12);
  height: var(--spacing-12);
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(6, 182, 212, 0.15);
  border: 2px solid rgba(6, 182, 212, 0.3);
  border-radius: var(--radius-lg);
  font-size: var(--text-3xl);
}

.ai-capability-visual-text {
  font-size: var(--text-xs);
  color: rgba(255, 255, 255, 0.7);
  margin: 0;
  text-align: center;
}

.ai-capability-visual-arrow {
  color: rgba(6, 182, 212, 0.5);
  font-size: var(--text-2xl);
  flex-shrink: 0;
}

/* 下半部分：项目类型卡片 */
.ai-capability-projects {
  margin-top: var(--spacing-16);
}

.ai-capability-projects-title {
  font-size: var(--text-5xl);
  font-weight: 600;
  margin: 0 0 var(--spacing-xl) 0;
  text-align: center;
  /* 浅色主题优化（2025-01）：使用主题文字颜色，确保清晰 */
  color: var(--color-text-main);
}

.ai-capability-projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(var(--spacing-xl) + var(--spacing-lg), 1fr));
  gap: var(--spacing-lg);
}

.ai-capability-project-card {
  position: relative;
  padding: var(--spacing-lg);
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-xl);
  cursor: pointer;
  transition: all 0.3s ease;
  overflow: hidden;
}

.ai-capability-project-card:hover {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(6, 182, 212, 0.4);
  transform: translateY(-8px);
  box-shadow: 0 var(--spacing-md) var(--spacing-16) rgba(6, 182, 212, 0.2);
}

.ai-capability-project-card-bg {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(6, 182, 212, 0.05) 0%, rgba(139, 92, 246, 0.05) 100%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.ai-capability-project-card:hover .ai-capability-project-card-bg {
  opacity: 1;
}

.ai-capability-project-card-content {
  position: relative;
  z-index: 1;
}

.ai-capability-project-title {
  font-size: var(--text-2xl);
  font-weight: 600;
  margin: 0 0 var(--spacing-md) 0;
  /* 浅色主题优化（2025-01）：使用主题文字颜色，确保清晰 */
  color: var(--color-text-main);
}

.ai-capability-project-description {
  font-size: var(--text-sm);
  line-height: 1.6;
  margin: 0 0 var(--spacing-lg) 0;
  /* 浅色主题优化（2025-01）：使用主题文字颜色，确保清晰 */
  color: var(--color-text-secondary);
}

.ai-capability-project-tags {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
}

.ai-capability-project-tag {
  padding: var(--spacing-xs) var(--spacing-md);
  border-radius: var(--radius-sm);
  font-size: var(--text-xs);
  /* 浅色主题优化（2025-01）：标签使用主色柔和背景，提高可读性 */
  background: var(--primary-soft-bg);
  border: 1px solid rgba(37, 99, 235, 0.2);
  color: var(--color-info);
}

/* 深色主题保持原有样式 */
html[data-theme="dark"] .ai-capability-project-tag {
  background: rgba(6, 182, 212, 0.1);
  border: 1px solid rgba(6, 182, 212, 0.3);
  color: var(--color-primary);
}

/* 底部按钮 */
.ai-capability-footer {
  display: flex;
  justify-content: center;
  margin-top: var(--spacing-16);
}

.ai-capability-button {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-lg) var(--spacing-16);
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 100%);
  border: none;
  border-radius: var(--radius-lg);
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--color-text-main, white);
  text-decoration: none;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 var(--spacing-sm) var(--spacing-lg) rgba(6, 182, 212, 0.3);
}

.ai-capability-button:hover {
  transform: translateY(-4px);
  box-shadow: 0 var(--spacing-md) var(--spacing-16) rgba(6, 182, 212, 0.4);
}

.ai-capability-button i {
  transition: transform 0.3s ease;
}

.ai-capability-button:hover i {
  transform: translateX(8px);
}
</style>
