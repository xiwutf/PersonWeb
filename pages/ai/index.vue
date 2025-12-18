<template>
  <div class="ai-solutions-page">
    <!-- 背景特效 -->
    <div class="ai-solutions-bg">
      <div class="ai-solutions-bg-gradient-1"></div>
      <div class="ai-solutions-bg-gradient-2"></div>
      <div class="ai-solutions-bg-grid"></div>
    </div>

    <div class="ai-solutions-container">
      <!-- 页面定位（写在页面最顶部） -->
      <div class="ai-solutions-badge">
        <span class="ai-solutions-badge-dot"></span>
        <span>AI / 智能体解决方案</span>
      </div>

      <!-- 主标题（H1） -->
      <h1 class="ai-solutions-title">
        AI / 智能体解决方案
      </h1>

      <!-- 副标题（H2） -->
      <h2 class="ai-solutions-subtitle">
        为个人与中小团队打造可落地的 AI 应用与智能体系统
      </h2>

      <!-- 引导说明（小段文案） -->
      <p class="ai-solutions-description">
        我专注于将 AI 技术落地为真实可用的工具与系统，
        覆盖智能体构建、知识库问答、流程自动化与 AI 工具开发，
        适用于个人创作者、小团队与中小企业的实际业务场景。
      </p>

      <!-- 模块一：我能做什么（能力展示） -->
      <section id="capabilities" class="ai-solutions-section">
        <div class="ai-solutions-section-header">
          <h2 class="ai-solutions-section-title">
            <i class="fas fa-lightbulb"></i>
            我能为你构建哪些 AI 能力
          </h2>
        </div>

        <div class="ai-solutions-capabilities-grid">
          <div
            v-for="capability in capabilities"
            :key="capability.id"
            class="ai-solutions-capability-card"
          >
            <div class="ai-solutions-capability-icon">
              <i :class="capability.icon"></i>
            </div>
            <h3 class="ai-solutions-capability-title">{{ capability.title }}</h3>
            <ul class="ai-solutions-capability-features">
              <li v-for="feature in capability.features" :key="feature">
                {{ feature }}
              </li>
            </ul>
          </div>
        </div>
      </section>

      <!-- 模块二：代表性 AI 项目（案例） -->
      <section id="projects" class="ai-solutions-section">
        <div class="ai-solutions-section-header">
          <h2 class="ai-solutions-section-title">
            <i class="fas fa-project-diagram"></i>
            部分 AI 项目与实践
          </h2>
          <p class="ai-solutions-section-description">
            （可持续新增，不求多，但求真实）
          </p>
        </div>

        <div class="ai-solutions-projects-grid">
          <div
            v-for="project in featuredProjects"
            :key="project.id"
            class="ai-solutions-project-card"
            @click="handleProjectClick(project)"
          >
            <div class="ai-solutions-project-header">
              <div class="ai-solutions-project-icon">
                <i :class="project.icon"></i>
              </div>
              <h3 class="ai-solutions-project-title">{{ project.title }}</h3>
            </div>
            <p class="ai-solutions-project-description">{{ project.description }}</p>
            <div class="ai-solutions-project-highlights">
              <h4 class="ai-solutions-project-highlights-title">能力亮点：</h4>
              <ul class="ai-solutions-project-highlights-list">
                <li v-for="highlight in project.highlights" :key="highlight">
                  {{ highlight }}
                </li>
              </ul>
            </div>
          </div>
        </div>

        <p class="ai-solutions-projects-note">
          部分项目仍在持续迭代中，但核心能力已具备并可复用。
        </p>
      </section>

      <!-- 模块三：技术栈与架构能力（专业信任） -->
      <section id="tech-stack" class="ai-solutions-section">
        <div class="ai-solutions-section-header">
          <h2 class="ai-solutions-section-title">
            <i class="fas fa-code"></i>
            技术栈与架构能力
          </h2>
        </div>

        <div class="ai-solutions-tech-grid">
          <div
            v-for="category in techStackCategories"
            :key="category.name"
            class="ai-solutions-tech-card"
          >
            <div class="ai-solutions-tech-header">
              <i :class="category.icon"></i>
              <h3 class="ai-solutions-tech-title">{{ category.name }}</h3>
            </div>
            <ul class="ai-solutions-tech-list">
              <li v-for="item in category.items" :key="item">
                {{ item }}
              </li>
            </ul>
          </div>
        </div>
      </section>

      <!-- 模块四：合作方式（商业闭环） -->
      <section id="cooperation" class="ai-solutions-section">
        <div class="ai-solutions-section-header">
          <h2 class="ai-solutions-section-title">
            <i class="fas fa-handshake"></i>
            合作流程
          </h2>
        </div>

        <div class="ai-solutions-cooperation-steps">
          <div
            v-for="(step, index) in cooperationSteps"
            :key="index"
            class="ai-solutions-cooperation-step"
          >
            <div class="ai-solutions-cooperation-step-number">
              {{ index + 1 }}️⃣
            </div>
            <div class="ai-solutions-cooperation-step-content">
              <h3 class="ai-solutions-cooperation-step-title">{{ step.title }}</h3>
              <p class="ai-solutions-cooperation-step-description">{{ step.description }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- 模块五：结尾 CTA（非常重要） -->
      <section id="cta" class="ai-solutions-cta">
        <div class="ai-solutions-cta-content">
          <p class="ai-solutions-cta-text">
            如果你正在寻找一个<br>
            能真正把 AI 做成"工具"和"系统"的开发者<br>
            欢迎与我交流你的想法。
          </p>
          <div class="ai-solutions-cta-buttons">
            <NuxtLink to="/about" class="ai-solutions-cta-button ai-solutions-cta-button--primary">
              <span>联系我</span>
              <i class="fas fa-arrow-right"></i>
            </NuxtLink>
            <a href="#projects" class="ai-solutions-cta-button ai-solutions-cta-button--secondary">
              <span>查看项目</span>
              <i class="fas fa-arrow-down"></i>
            </a>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'ai'
})

useHead({
  title: 'AI / 智能体解决方案 | 溪午听风',
  meta: [
    { name: 'description', content: '为个人与中小团队打造可落地的 AI 应用与智能体系统。智能体构建、知识库问答、流程自动化与 AI 工具开发。' }
  ]
})

const router = useRouter()

// 能力展示数据
const capabilities = [
  {
    id: 'agent-system',
    title: '智能体系统开发',
    icon: 'fas fa-robot',
    features: [
      '多角色智能体设计',
      '任务拆解与自动执行',
      '工具调用与流程编排',
      '支持长期记忆与上下文管理'
    ]
  },
  {
    id: 'ai-tools',
    title: 'AI 工具 / 助手定制',
    icon: 'fas fa-magic',
    features: [
      '智能取名、内容生成',
      '表单 / 数据处理自动化',
      '规则 + AI 混合逻辑',
      '私有化部署支持'
    ]
  },
  {
    id: 'knowledge-qa',
    title: '知识库智能问答',
    icon: 'fas fa-database',
    features: [
      '文档 / 网站 / 私有数据接入',
      '向量化与语义检索',
      '多轮对话与精准引用',
      '适用于企业内部或个人知识管理'
    ]
  },
  {
    id: 'integration',
    title: 'AI + 网站 / 系统集成',
    icon: 'fas fa-plug',
    features: [
      'AI 客服 / 网站聊天助手',
      '管理后台 AI 辅助',
      '与现有系统无缝集成',
      '前后端完整交付'
    ]
  }
]

// 代表性项目案例
const featuredProjects = [
  {
    id: 'name-tool',
    title: '智能取名助手',
    icon: 'fas fa-sparkles',
    description: '基于规则与大模型的智能取名系统，支持行业、风格、禁用词等多维度控制。',
    highlights: [
      'Prompt 模板化管理',
      '多轮生成与去重逻辑',
      '可扩展为品牌命名 / 产品命名工具'
    ],
    path: '/tools/name'
  },
  {
    id: 'website-chat',
    title: '网站 AI 聊天助手',
    icon: 'fas fa-comments',
    description: '为个人网站与产品站点提供定制化 AI 问答能力。',
    highlights: [
      '系统 Prompt 与用户 Prompt 分离',
      '支持上下文与历史消息',
      '可对接知识库与业务接口'
    ],
    path: null
  },
  {
    id: 'finance-assistant',
    title: '个人理财 / 资产分析智能助手（规划中）',
    icon: 'fas fa-chart-line',
    description: '用于个人资产统计、分析与长期理财规划的 AI 辅助系统。',
    highlights: [
      '多资产类型建模',
      '数据分析 + AI 解释',
      '长期可演进为个人理财助手'
    ],
    path: null
  }
]

// 技术栈分类
const techStackCategories = [
  {
    name: 'AI / 模型层',
    icon: 'fas fa-brain',
    items: [
      '主流大模型（OpenAI / 国产模型等）',
      'Prompt Engineering',
      '多模型策略与降本方案'
    ]
  },
  {
    name: '后端与服务编排',
    icon: 'fas fa-server',
    items: [
      'Python / .NET WebAPI',
      'LangChain / 向量数据库',
      '工具调用、任务流设计',
      '权限与安全控制'
    ]
  },
  {
    name: '前端与交互',
    icon: 'fas fa-desktop',
    items: [
      'Vue 3 / Nuxt 3',
      '管理后台与用户交互设计',
      'AI 结果可视化'
    ]
  }
]

// 合作流程步骤
const cooperationSteps = [
  {
    title: '需求沟通',
    description: '明确你的业务场景与目标'
  },
  {
    title: '方案设计',
    description: 'AI 能力拆解与技术方案制定'
  },
  {
    title: '开发实现',
    description: '敏捷开发，阶段性可验收'
  },
  {
    title: '交付与迭代',
    description: '支持后期优化与功能扩展'
  }
]

// 处理项目点击
const handleProjectClick = (project: any) => {
  if (project.path) {
    router.push(project.path)
  }
}
</script>

<style scoped>
.ai-solutions-page {
  position: relative;
  min-height: 100vh;
  background: var(--bg);
  color: var(--text-main);
  padding-top: 100px;
  padding-bottom: 80px;
}

/* 背景特效 */
.ai-solutions-bg {
  position: fixed;
  inset: 0;
  pointer-events: none;
  z-index: 0;
  overflow: hidden;
}

.ai-solutions-bg-gradient-1 {
  position: absolute;
  top: -20%;
  left: -10%;
  width: 600px;
  height: 600px;
  background: radial-gradient(circle, rgba(6, 182, 212, 0.15) 0%, transparent 70%);
  border-radius: 50%;
  filter: blur(80px);
}

.ai-solutions-bg-gradient-2 {
  position: absolute;
  bottom: -20%;
  right: -10%;
  width: 600px;
  height: 600px;
  background: radial-gradient(circle, rgba(139, 92, 246, 0.15) 0%, transparent 70%);
  border-radius: 50%;
  filter: blur(80px);
}

.ai-solutions-bg-grid {
  position: absolute;
  inset: 0;
  background-image: 
    linear-gradient(rgba(6, 182, 212, 0.1) 1px, transparent 1px),
    linear-gradient(90deg, rgba(6, 182, 212, 0.1) 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.3;
}

.ai-solutions-container {
  position: relative;
  z-index: 1;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 24px;
}

/* 页面定位 Badge */
.ai-solutions-badge {
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

.ai-solutions-badge-dot {
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

/* 主标题 */
.ai-solutions-title {
  font-size: 56px;
  font-weight: 700;
  line-height: 1.2;
  margin: 0 0 24px 0;
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

@media (max-width: 640px) {
  .ai-solutions-title {
    font-size: 40px;
  }
}

/* 副标题 */
.ai-solutions-subtitle {
  font-size: 24px;
  font-weight: 500;
  line-height: 1.4;
  color: var(--text-main);
  margin: 0 0 24px 0;
}

@media (max-width: 640px) {
  .ai-solutions-subtitle {
    font-size: 20px;
  }
}

/* 引导说明 */
.ai-solutions-description {
  font-size: 18px;
  line-height: 1.8;
  color: var(--text-secondary);
  max-width: 800px;
  margin: 0 0 80px 0;
}

@media (max-width: 640px) {
  .ai-solutions-description {
    font-size: 16px;
  }
}

/* 区块通用样式 */
.ai-solutions-section {
  margin-bottom: 120px;
}

.ai-solutions-section-header {
  text-align: center;
  margin-bottom: 48px;
}

.ai-solutions-section-title {
  font-size: 36px;
  font-weight: 600;
  margin: 0 0 16px 0;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  color: var(--text-main);
}

.ai-solutions-section-title i {
  color: #06b6d4;
}

.ai-solutions-section-description {
  font-size: 14px;
  color: var(--text-muted);
  margin: 0;
}

@media (max-width: 640px) {
  .ai-solutions-section-title {
    font-size: 28px;
  }
}

/* 能力展示卡片 */
.ai-solutions-capabilities-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 24px;
}

.ai-solutions-capability-card {
  padding: 32px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  transition: all 0.3s ease;
}

.ai-solutions-capability-card:hover {
  background: var(--bg-elevated);
  border-color: var(--primary);
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(6, 182, 212, 0.15);
}

.ai-solutions-capability-icon {
  width: 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(6, 182, 212, 0.1);
  border-radius: 12px;
  margin-bottom: 20px;
}

.ai-solutions-capability-icon i {
  font-size: 24px;
  color: #06b6d4;
}

.ai-solutions-capability-title {
  font-size: 20px;
  font-weight: 600;
  color: var(--text-main);
  margin: 0 0 16px 0;
}

.ai-solutions-capability-features {
  list-style: none;
  padding: 0;
  margin: 0;
}

.ai-solutions-capability-features li {
  font-size: 14px;
  line-height: 1.8;
  color: var(--text-secondary);
  padding-left: 20px;
  position: relative;
  margin-bottom: 8px;
}

.ai-solutions-capability-features li::before {
  content: '•';
  position: absolute;
  left: 0;
  color: #06b6d4;
  font-weight: bold;
}

/* 项目案例卡片 */
.ai-solutions-projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
  gap: 24px;
  margin-bottom: 32px;
}

.ai-solutions-project-card {
  padding: 32px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  transition: all 0.3s ease;
  cursor: pointer;
}

.ai-solutions-project-card:hover {
  background: var(--bg-elevated);
  border-color: var(--primary);
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(6, 182, 212, 0.15);
}

.ai-solutions-project-header {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 16px;
}

.ai-solutions-project-icon {
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(6, 182, 212, 0.1);
  border-radius: 12px;
}

.ai-solutions-project-icon i {
  font-size: 20px;
  color: #06b6d4;
}

.ai-solutions-project-title {
  font-size: 20px;
  font-weight: 600;
  color: var(--text-main);
  margin: 0;
}

.ai-solutions-project-description {
  font-size: 14px;
  line-height: 1.6;
  color: var(--text-secondary);
  margin: 0 0 20px 0;
}

.ai-solutions-project-highlights {
  padding-top: 20px;
  border-top: 1px solid var(--border-color);
}

.ai-solutions-project-highlights-title {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-main);
  margin: 0 0 12px 0;
}

.ai-solutions-project-highlights-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.ai-solutions-project-highlights-list li {
  font-size: 13px;
  line-height: 1.6;
  color: var(--text-secondary);
  padding-left: 20px;
  position: relative;
  margin-bottom: 6px;
}

.ai-solutions-project-highlights-list li::before {
  content: '✓';
  position: absolute;
  left: 0;
  color: #06b6d4;
  font-weight: bold;
}

.ai-solutions-projects-note {
  text-align: center;
  font-size: 14px;
  color: var(--text-muted);
  margin: 0;
}

/* 技术栈卡片 */
.ai-solutions-tech-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 24px;
}

.ai-solutions-tech-card {
  padding: 32px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  transition: all 0.3s ease;
}

.ai-solutions-tech-card:hover {
  background: var(--bg-elevated);
  border-color: var(--primary);
  transform: translateY(-4px);
}

.ai-solutions-tech-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 20px;
}

.ai-solutions-tech-header i {
  font-size: 24px;
  color: #06b6d4;
}

.ai-solutions-tech-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--text-main);
  margin: 0;
}

.ai-solutions-tech-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.ai-solutions-tech-list li {
  font-size: 14px;
  line-height: 1.8;
  color: var(--text-secondary);
  padding-left: 20px;
  position: relative;
  margin-bottom: 8px;
}

.ai-solutions-tech-list li::before {
  content: '→';
  position: absolute;
  left: 0;
  color: #06b6d4;
}

/* 合作流程 */
.ai-solutions-cooperation-steps {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 24px;
  max-width: 1000px;
  margin: 0 auto;
}

.ai-solutions-cooperation-step {
  padding: 24px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  transition: all 0.3s ease;
}

.ai-solutions-cooperation-step:hover {
  background: var(--bg-elevated);
  border-color: var(--primary);
}

.ai-solutions-cooperation-step-number {
  font-size: 32px;
  margin-bottom: 16px;
}

.ai-solutions-cooperation-step-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--text-main);
  margin: 0 0 8px 0;
}

.ai-solutions-cooperation-step-description {
  font-size: 14px;
  line-height: 1.6;
  color: var(--text-secondary);
  margin: 0;
}

/* CTA 区块 */
.ai-solutions-cta {
  text-align: center;
  padding: 80px 24px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: 24px;
  margin-top: 80px;
}

.ai-solutions-cta-content {
  max-width: 600px;
  margin: 0 auto;
}

.ai-solutions-cta-text {
  font-size: 20px;
  line-height: 1.8;
  color: var(--text-main);
  margin: 0 0 40px 0;
}

.ai-solutions-cta-buttons {
  display: flex;
  gap: 16px;
  justify-content: center;
  flex-wrap: wrap;
}

.ai-solutions-cta-button {
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

.ai-solutions-cta-button--primary {
  background: linear-gradient(135deg, var(--primary) 0%, var(--primary-hover) 100%);
  color: #fff;
  box-shadow: 0 4px 16px rgba(37, 99, 235, 0.3);
}

.ai-solutions-cta-button--primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(6, 182, 212, 0.4);
}

.ai-solutions-cta-button--secondary {
  background: var(--primary-soft-bg);
  border: 1px solid var(--border-color);
  color: var(--primary);
}

.ai-solutions-cta-button--secondary:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(6, 182, 212, 0.4);
}
</style>
