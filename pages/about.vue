<template>
  <AdminModuleGuard module-key="about">
    <div class="about-page">
      <div class="about-background-noise"></div>

      <div class="about-background-container">
        <div class="about-background-blob about-background-blob--blue"></div>
        <div class="about-background-blob about-background-blob--purple"></div>
        <div class="about-background-blob about-background-blob--emerald"></div>
      </div>

      <div class="about-content">
        <section class="about-hero">
          <div class="about-hero-main">
            <div class="about-avatar-container">
              <div class="about-avatar-glow"></div>
              <div class="about-avatar">
                <img src="/images/avatar.jpg" alt="溪午听风" />
              </div>
              <div class="about-status-indicator" title="Online">
                <span class="about-status-ping"></span>
              </div>
            </div>

            <div class="about-hero-copy">
              <p class="about-hero-kicker">About Me</p>
              <h1 class="about-title">溪午听风</h1>
              <p class="about-subtitle">系统型工程师 · AI 实践者 · 技术创业者</p>
              <p class="about-description">
                我不只是写代码，而是致力于：用 AI 提升软件系统能力、用工程化方法构建长期可复用的系统、
                将技术沉淀为可以持续积累的数字资产。相比单纯做功能开发，我更关注系统是否可复用、
                能否形成长期积累、是否具备产品化与商业化潜力。
              </p>

              <div class="about-social-links">
                <a
                  v-for="social in socialLinks"
                  :key="social.name"
                  :href="social.link"
                  target="_blank"
                  rel="noreferrer"
                  class="about-social-link"
                  :aria-label="social.name"
                >
                  <i :class="social.icon"></i>
                </a>
              </div>
            </div>
          </div>

          <div class="about-hero-aside">
            <div class="about-hero-panel">
              <p class="about-panel-kicker">Focus</p>
              <h2 class="about-panel-title">当前关注方向</h2>
              <div class="about-hero-tags">
                <span class="about-hero-tag">AI 系统</span>
                <span class="about-hero-tag">工业软件</span>
                <span class="about-hero-tag">自动化工作流</span>
                <span class="about-hero-tag">个人数字系统</span>
              </div>
            </div>

            <div class="about-hero-panel">
              <p class="about-panel-kicker">Snapshot</p>
              <h2 class="about-panel-title">个人速写</h2>
              <div class="about-highlight-list">
                <div class="about-highlight-item">
                  <span class="about-highlight-label">角色定位</span>
                  <strong class="about-highlight-value">系统型工程师 + AI 实践者</strong>
                </div>
                <div class="about-highlight-item">
                  <span class="about-highlight-label">工作方式</span>
                  <strong class="about-highlight-value">模块化、系统化、可复用</strong>
                </div>
                <div class="about-highlight-item">
                  <span class="about-highlight-label">擅长场景</span>
                  <strong class="about-highlight-value">架构设计、AI 集成、复杂业务建模</strong>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section class="about-section">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--purple">🎯</span>
            我在做什么
          </h2>
          <div class="about-doing-grid">
            <article class="about-doing-card">
              <div class="about-doing-icon about-doing-icon--purple">🤖</div>
              <h3 class="about-doing-title">AI 系统落地</h3>
              <p class="about-doing-desc">把 AI 从「聊天工具」变成可用的系统能力、可嵌入业务的模块、可复用的基础设施。</p>
              <ul class="about-doing-list">
                <li>AI 知识库（RAG）</li>
                <li>智能客服系统</li>
                <li>Agent 工具系统</li>
                <li>AI 工作流集成</li>
              </ul>
            </article>
            <article class="about-doing-card">
              <div class="about-doing-icon about-doing-icon--blue">🏭</div>
              <h3 class="about-doing-title">工业软件系统</h3>
              <p class="about-doing-desc">围绕真实业务场景开发：AGV 调度系统、售后工单系统、设备数据平台、API 集成系统。</p>
              <ul class="about-doing-list">
                <li>把复杂业务抽象成系统</li>
              </ul>
            </article>
            <article class="about-doing-card">
              <div class="about-doing-icon about-doing-icon--emerald">🌐</div>
              <h3 class="about-doing-title">个人数字系统</h3>
              <p class="about-doing-desc">正在构建 PersonWeb，把个人能力沉淀为工具、系统、产品、可售卖模块，形成技术 → 产品 → 数字资产。</p>
              <ul class="about-doing-list">
                <li>工具 · 系统 · 产品</li>
              </ul>
            </article>
          </div>
        </section>

        <section class="about-section">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--blue">⚡</span>
            技能矩阵
          </h2>
          <div class="about-skills-grid">
            <article
              v-for="category in skillCategories"
              :key="category.name"
              class="about-skill-card"
            >
              <div class="about-skill-icon" :class="`about-skill-icon--${category.color}`">
                {{ category.icon }}
              </div>
              <h3 class="about-skill-name">{{ category.name }}</h3>
              <p class="about-skill-summary">{{ category.summary }}</p>
              <div class="about-skill-tags">
                <span
                  v-for="skill in category.skills"
                  :key="skill"
                  class="about-skill-tag"
                >
                  {{ skill }}
                </span>
              </div>
            </article>
          </div>
        </section>

        <section class="about-timeline-grid">
          <div>
            <h2 class="about-timeline-section-title">
              <span class="about-section-icon about-section-icon--purple">🚀</span>
              项目成就
            </h2>
            <div class="about-timeline-list">
              <div
                v-for="(item, index) in achievements"
                :key="index"
                class="about-timeline-item"
              >
                <div class="about-timeline-dot"></div>
                <h3 class="about-timeline-title">{{ item.title }}</h3>
                <p class="about-timeline-meta">{{ item.type }} · {{ item.year }}</p>
                <p class="about-timeline-desc">{{ item.desc }}</p>
              </div>
            </div>
          </div>

          <div>
            <h2 class="about-timeline-section-title">
              <span class="about-section-icon about-section-icon--emerald">💡</span>
              技术历程
            </h2>
            <div class="about-timeline-list">
              <div
                v-for="(item, index) in experience"
                :key="index"
                class="about-timeline-item about-timeline-item--emerald"
              >
                <div class="about-timeline-dot"></div>
                <h3 class="about-timeline-title">{{ item.title }}</h3>
                <p class="about-timeline-meta">{{ item.duration }}</p>
                <p class="about-timeline-desc">{{ item.desc }}</p>
              </div>
            </div>
          </div>
        </section>

        <section class="about-section about-section--compact">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--pink">🎯</span>
            兴趣爱好
          </h2>
          <div class="about-hobbies-grid">
            <div
              v-for="hobby in hobbies"
              :key="hobby.name"
              class="about-hobby-card"
            >
              <div class="about-hobby-icon">{{ hobby.icon }}</div>
              <div class="about-hobby-name">{{ hobby.name }}</div>
            </div>
          </div>
        </section>

        <section class="about-section about-section--compact">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--purple">🧠</span>
            认知说明书
          </h2>
          <div class="about-cognition-card">
            <p class="about-cognition-desc">
              这里记录我如何理解学习、构建、协作和长期成长，
              也是一份帮助别人更快认识我的工作说明。
            </p>
            <NuxtLink to="/cognition" class="about-cognition-link">
              <i class="fas fa-arrow-right"></i>
              查看认知说明书
            </NuxtLink>
          </div>
        </section>

        <section class="about-section about-section--compact">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--emerald">✨</span>
            我的原则
          </h2>
          <div class="about-values-grid">
            <div class="about-value-item">
              <span class="about-value-icon">→</span>
              <span>不做重复劳动，优先系统化</span>
            </div>
            <div class="about-value-item">
              <span class="about-value-icon">→</span>
              <span>不追求炫技，优先实用</span>
            </div>
            <div class="about-value-item">
              <span class="about-value-icon">→</span>
              <span>不做一次性开发，优先可复用</span>
            </div>
            <div class="about-value-item">
              <span class="about-value-icon">→</span>
              <span>不卖时间，构建资产</span>
            </div>
          </div>
        </section>

        <section class="about-contact-section">
          <div class="about-contact-content">
            <h2 class="about-contact-title">保持联系</h2>
            <div class="about-contact-layout">
              <div class="about-contact-left">
                <div class="about-contact-card">
                  <div class="about-contact-icon about-contact-icon--green">💬</div>
                  <div class="about-contact-info">
                    <div class="about-contact-label">WeChat</div>
                    <div class="about-contact-value about-contact-value--green">LinXi-5152</div>
                  </div>
                </div>
                <div class="about-contact-card">
                  <div class="about-contact-icon about-contact-icon--blue">📧</div>
                  <div class="about-contact-info">
                    <div class="about-contact-label">Email</div>
                    <div class="about-contact-value about-contact-value--blue">linxiwanting@gmail.com</div>
                  </div>
                </div>
              </div>
              <div class="about-contact-qr">
                <div class="about-contact-qr-image">
                  <img src="/images/wechat-qr.png" alt="微信二维码" />
                </div>
                <p class="about-contact-qr-text">扫码加好友，请注明来意。</p>
              </div>
            </div>
          </div>
        </section>
      </div>
    </div>
  </AdminModuleGuard>
</template>

<script setup lang="ts">
import '~/assets/css/about.css'

definePageMeta({
  layout: 'default'
})

const socialLinks = [
  { name: 'GitHub', icon: 'fab fa-github', link: 'https://github.com/Lijing327' },
  { name: 'Email', icon: 'fas fa-envelope', link: 'mailto:linxiwanting@gmail.com' },
  { name: 'Bilibili', icon: 'fas fa-play-circle', link: '#' }
]

const skillCategories = [
  {
    name: '前端',
    icon: '💻',
    color: 'blue',
    summary: 'Vue3 / TypeScript / Nuxt3 / Naive UI，关注可维护性与交互体验。',
    skills: ['Vue 3', 'Nuxt 3', 'TypeScript', 'Naive UI', 'Tailwind CSS']
  },
  {
    name: '后端',
    icon: '⚙️',
    color: 'purple',
    summary: '.NET WebAPI / Python FastAPI，构建可扩展的服务层。',
    skills: ['.NET', 'C#', 'FastAPI', 'Python', 'WebAPI']
  },
  {
    name: 'AI 系统',
    icon: '🤖',
    color: 'orange',
    summary: 'RAG 向量检索、Agent 系统、LLM 集成（DeepSeek / OpenAI）。',
    skills: ['RAG', 'Agent', 'LangChain', '向量检索', 'LLM 集成']
  },
  {
    name: '数据与工具',
    icon: '📊',
    color: 'emerald',
    summary: 'MySQL / SQL Server，长期使用 ChatGPT、Claude、Cursor 做 AI 辅助开发。',
    skills: ['MySQL', 'SQL Server', 'ChatGPT', 'Claude', 'Cursor']
  }
]

const achievements = [
  {
    title: 'PersonWeb 个人数字系统',
    type: '主站项目',
    year: '2025',
    desc: '把个人能力沉淀为工具、系统、产品，形成技术 → 产品 → 数字资产的完整闭环。'
  },
  {
    title: 'AI 知识库与智能客服',
    type: 'AI 系统',
    year: '2024 - 2025',
    desc: 'RAG 向量检索、Agent 工具系统、AI 工作流集成，将 AI 能力嵌入真实业务。'
  },
  {
    title: '工业软件系统',
    type: '业务系统',
    year: '2023 - 2025',
    desc: 'AGV 调度系统、售后工单系统、设备数据平台、API 集成系统，把复杂业务抽象成系统。'
  }
]

const experience = [
  {
    title: '系统构建能力',
    duration: '核心能力',
    desc: '架构设计、AI 系统集成、复杂业务建模、多系统协同，而非单点技术。'
  },
  {
    title: 'AI 辅助开发',
    duration: '日常实践',
    desc: '长期使用 ChatGPT、Claude、Cursor 进行 AI 辅助开发与自动化工程。'
  },
  {
    title: '技术资产化',
    duration: '核心理念',
    desc: '模块化、系统化、可复用、可沉淀，形成可反复使用甚至售卖的能力模块。'
  }
]

const hobbies = [
  { name: '编程开发', icon: '💻' },
  { name: '技术学习', icon: '📚' },
  { name: '投资理财', icon: '📈' },
  { name: '产品思考', icon: '🧩' }
]

useHead({
  title: '关于我 - 溪午听风',
  meta: [
    { name: 'description', content: '溪午听风：AI 工程师、系统构建者、技术创业者。专注于 AI 系统落地、工业软件与个人数字系统，将技术沉淀为可持续积累的数字资产。' }
  ]
})
</script>
