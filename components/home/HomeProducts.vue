<template>
  <section id="products" class="home-section">
    <div class="home-shell">
      <div class="home-section-heading reveal-up">
        <p class="home-section-kicker">Product Matrix</p>
        <h2>产品矩阵</h2>
        <p>将 AI 能力、业务场景与工程积累沉淀为可持续迭代的产品资产。</p>
      </div>

      <div class="product-matrix reveal-up reveal-delay-1">
        <!-- Row 1: 2 primary products -->
        <article
          v-for="product in primaryProducts"
          :key="product.name"
          class="product-card product-card-primary"
        >
          <div class="product-card-top">
            <div class="product-status" :class="`product-status--${product.statusKey}`">
              <span class="product-status-dot"></span>
              {{ product.status }}
            </div>
            <div class="product-icon">{{ product.icon }}</div>
          </div>
          <div class="product-body">
            <h3>{{ product.name }}</h3>
            <p>{{ product.description }}</p>
          </div>
          <div class="product-card-bottom">
            <div class="product-tags">
              <span v-for="tag in product.tags" :key="tag">{{ tag }}</span>
            </div>
            <NuxtLink :to="product.link" class="product-action">
              {{ product.action }}<span aria-hidden="true"> →</span>
            </NuxtLink>
          </div>
        </article>
      </div>

      <!-- Row 2: 3 secondary products -->
      <div class="product-grid reveal-up reveal-delay-2">
        <article
          v-for="product in secondaryProducts"
          :key="product.name"
          class="product-card product-card-secondary"
        >
          <div class="product-card-top">
            <div class="product-status" :class="`product-status--${product.statusKey}`">
              <span class="product-status-dot"></span>
              {{ product.status }}
            </div>
            <div class="product-icon product-icon-sm">{{ product.icon }}</div>
          </div>
          <div class="product-body">
            <h3>{{ product.name }}</h3>
            <p>{{ product.description }}</p>
          </div>
          <div class="product-card-bottom">
            <div class="product-tags">
              <span v-for="tag in product.tags" :key="tag">{{ tag }}</span>
            </div>
            <NuxtLink :to="product.link" class="product-action">
              {{ product.action }}<span aria-hidden="true"> →</span>
            </NuxtLink>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
interface Product {
  name: string
  icon: string
  description: string
  tags: string[]
  status: string
  statusKey: 'live' | 'beta' | 'dev'
  action: string
  link: string
}

const primaryProducts: Product[] = [
  {
    name: 'AI-Hub',
    icon: '⬡',
    description: '一站式 AI 工具聚合平台，面向知识检索、内容生成与工作流场景，轻量可扩展。',
    tags: ['RAG', '向量检索', '工作流', '多模型'],
    status: '内测中',
    statusKey: 'beta',
    action: '申请体验',
    link: '/lab'
  },
  {
    name: 'AI 工单系统',
    icon: '⬢',
    description: '用 AI 实现工单智能分流、自动摘要与知识库问答，让服务流程降本提效。',
    tags: ['意图识别', '自动摘要', 'AI 分流', '.NET'],
    status: '开发中',
    statusKey: 'dev',
    action: '了解方案',
    link: '/projects'
  }
]

const secondaryProducts: Product[] = [
  {
    name: 'AI 桌宠',
    icon: '◈',
    description: '陪伴式智能桌面助手，让 AI 对话自然融入日常工作流，支持本地模型。',
    tags: ['Electron', 'AI 对话', '本地模型'],
    status: '开发中',
    statusKey: 'dev',
    action: '敬请期待',
    link: '/products'
  },
  {
    name: 'PersonWeb',
    icon: '◇',
    description: '个人数字资产管理平台，内容、项目、数据三位一体，沉淀长期价值。',
    tags: ['Nuxt3', 'Vue3', '.NET 8'],
    status: '已上线',
    statusKey: 'live',
    action: '查看源码',
    link: '/projects'
  },
  {
    name: '企业协作平台',
    icon: '◉',
    description: '面向中小团队的事务管理与协作底座，支持流程定制与权限分层。',
    tags: ['.NET 8', 'SQL Server', 'Docker'],
    status: '已上线',
    statusKey: 'live',
    action: '查看案例',
    link: '/projects'
  }
]
</script>

<style scoped>
.product-matrix {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
  margin-bottom: 1.25rem;
}

.product-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.25rem;
}

/* ── Card Base ── */
.product-card {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1.5rem;
  border: 1px solid var(--home-border);
  border-radius: var(--home-radius);
  background: var(--home-card);
  transition: transform 0.26s ease, border-color 0.26s ease, box-shadow 0.26s ease, background 0.26s ease;
  overflow: hidden;
}

.product-card::before {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: inherit;
  background: radial-gradient(circle at 80% 120%, rgba(54, 97, 255, 0.12), transparent 55%);
  opacity: 0;
  transition: opacity 0.26s ease;
  pointer-events: none;
}

.product-card:hover {
  transform: translateY(-0.3rem);
  border-color: var(--home-border-strong);
  background: var(--home-card-hover);
  box-shadow: var(--home-shadow-glow);
}

.product-card:hover::before {
  opacity: 1;
}

.product-card-primary {
  min-height: 18rem;
}

.product-card-secondary {
  min-height: 15rem;
}

/* ── Card Top ── */
.product-card-top {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
}

/* ── Status Badge ── */
.product-status {
  display: inline-flex;
  align-items: center;
  gap: 0.42rem;
  padding: 0.3rem 0.65rem;
  border-radius: 999px;
  font-size: 0.72rem;
  font-weight: 620;
  border: 1px solid transparent;
}

.product-status-dot {
  width: 0.38rem;
  height: 0.38rem;
  border-radius: 50%;
  flex-shrink: 0;
}

.product-status--live {
  color: #34d399;
  background: rgba(52, 211, 153, 0.1);
  border-color: rgba(52, 211, 153, 0.22);
}
.product-status--live .product-status-dot {
  background: #34d399;
  box-shadow: 0 0 8px rgba(52, 211, 153, 0.72);
}

.product-status--beta {
  color: #818cf8;
  background: rgba(129, 140, 248, 0.1);
  border-color: rgba(129, 140, 248, 0.22);
}
.product-status--beta .product-status-dot {
  background: #818cf8;
  box-shadow: 0 0 8px rgba(129, 140, 248, 0.72);
}

.product-status--dev {
  color: #fbbf24;
  background: rgba(251, 191, 36, 0.1);
  border-color: rgba(251, 191, 36, 0.22);
}
.product-status--dev .product-status-dot {
  background: #fbbf24;
  box-shadow: 0 0 8px rgba(251, 191, 36, 0.6);
}

/* ── Icon ── */
.product-icon {
  width: 3.2rem;
  height: 3.2rem;
  display: grid;
  place-items: center;
  border-radius: 1rem;
  color: var(--home-text-main);
  font-size: 1.45rem;
  background: linear-gradient(135deg, rgba(84, 128, 255, 0.32), rgba(95, 67, 210, 0.18));
  border: 1px solid rgba(120, 160, 255, 0.22);
}

.product-icon-sm {
  width: 2.6rem;
  height: 2.6rem;
  border-radius: 0.8rem;
  font-size: 1.15rem;
}

/* ── Body ── */
.product-body {
  flex: 1;
}

.product-body h3 {
  margin: 0 0 0.55rem;
  color: var(--home-text-main);
  font-size: 1.2rem;
  font-weight: 720;
}

.product-card-secondary .product-body h3 {
  font-size: 1.05rem;
}

.product-body p {
  margin: 0;
  color: var(--home-text-muted);
  font-size: 0.93rem;
  line-height: 1.75;
}

/* ── Bottom ── */
.product-card-bottom {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.product-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.42rem;
}

.product-tags span {
  padding: 0.32rem 0.6rem;
  border: 1px solid var(--home-border);
  border-radius: 999px;
  color: var(--home-text-soft);
  background: rgba(255, 255, 255, 0.032);
  font-size: 0.72rem;
}

.product-action {
  white-space: nowrap;
  color: var(--home-accent);
  font-size: 0.88rem;
  font-weight: 680;
  transition: color 0.2s ease;
  flex-shrink: 0;
}

.product-action:hover {
  color: var(--home-text-main);
}

/* ── Responsive ── */
@media (max-width: 1100px) {
  .product-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 760px) {
  .product-matrix {
    grid-template-columns: 1fr;
  }
  .product-card-primary {
    min-height: auto;
  }
}

@media (max-width: 580px) {
  .product-grid {
    grid-template-columns: 1fr;
  }
}
</style>
