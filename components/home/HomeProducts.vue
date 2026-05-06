<template>
  <section id="products" class="home-section">
    <div class="home-shell">
      <div class="home-section-heading reveal-up">
        <p class="home-section-kicker">Product Center</p>
        <h2>产品中心</h2>
        <p>把 AI 能力、业务场景和长期维护沉淀成可复用的产品资产。</p>
      </div>

      <div class="home-products">
        <article class="product-card product-card-featured reveal-up">
          <div class="product-icon product-icon-featured">AI</div>
          <div class="product-content">
            <p class="product-label">主产品</p>
            <h3>AI-Hub</h3>
            <p>一站式 AI 工具聚合平台，面向知识、内容与工作流场景提供轻量可扩展的智能能力。</p>
            <div class="product-tags">
              <span v-for="tag in featuredProduct.tags" :key="tag">{{ tag }}</span>
            </div>
          </div>
          <NuxtLink :to="featuredProduct.link" class="product-action">
            在线体验
            <span aria-hidden="true">→</span>
          </NuxtLink>
        </article>

        <div class="product-grid">
          <article
            v-for="(product, index) in products"
            :key="product.name"
            class="product-card reveal-up"
            :class="`reveal-delay-${index + 1}`"
          >
            <div class="product-icon">{{ product.initial }}</div>
            <h3>{{ product.name }}</h3>
            <p>{{ product.description }}</p>
            <div class="product-tags">
              <span v-for="tag in product.tags" :key="tag">{{ tag }}</span>
            </div>
            <NuxtLink :to="product.link" class="product-action">
              {{ product.action }}
              <span aria-hidden="true">→</span>
            </NuxtLink>
          </article>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
interface ProductItem {
  name: string
  initial: string
  description: string
  tags: string[]
  action: string
  link: string
}

const featuredProduct = {
  tags: ['AI 聚合', '工具导航', '工作流'],
  link: '/products'
}

const products: ProductItem[] = [
  {
    name: '桌宠',
    initial: 'D',
    description: '陪伴式智能桌面伙伴，让 AI 更自然地进入日常工作。',
    tags: ['Electron', 'AI 对话'],
    action: '下载使用',
    link: '/download'
  },
  {
    name: 'PersonWeb',
    initial: 'P',
    description: '个人数字资产管理平台，用内容、项目和数据沉淀长期价值。',
    tags: ['Vue3', 'Nuxt3'],
    action: '在线体验',
    link: '/'
  },
  {
    name: '团体平台',
    initial: 'T',
    description: '面向小团队协作与事务管理的轻量系统底座。',
    tags: ['.NET 8', 'SQL Server'],
    action: '了解更多',
    link: '/projects'
  },
  {
    name: 'AI 工单系统',
    initial: 'W',
    description: '用 AI 辅助分流、总结与处理，让服务流程更高效。',
    tags: ['AI', '工作流'],
    action: '在线体验',
    link: '/products'
  }
]
</script>

<style scoped>
.home-products {
  display: grid;
  gap: 1.25rem;
}

.product-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 1.25rem;
}

.product-card {
  position: relative;
  display: flex;
  min-height: 17rem;
  flex-direction: column;
  gap: 1.1rem;
  padding: 1.45rem;
  border: 1px solid var(--home-border);
  border-radius: var(--home-radius);
  background: var(--home-card);
  box-shadow: var(--home-shadow-soft);
  transition: transform 0.28s ease, border-color 0.28s ease, box-shadow 0.28s ease, background 0.28s ease;
}

.product-card:hover {
  transform: translateY(-0.35rem);
  border-color: var(--home-border-strong);
  background: var(--home-card-hover);
  box-shadow: var(--home-shadow-glow);
}

.product-card-featured {
  min-height: 24rem;
  display: grid;
  grid-template-columns: auto minmax(0, 1fr) auto;
  align-items: end;
  gap: 2rem;
  padding: clamp(2rem, 5vw, 4rem);
  overflow: hidden;
}

.product-card-featured::before {
  content: '';
  position: absolute;
  inset: auto -10% -42% 32%;
  height: 75%;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(54, 97, 255, 0.26), transparent 64%);
  filter: blur(2.5rem);
}

.product-icon {
  width: 3rem;
  height: 3rem;
  display: grid;
  place-items: center;
  border-radius: 1rem;
  color: var(--home-text-main);
  font-size: 0.92rem;
  font-weight: 760;
  background: linear-gradient(135deg, rgba(84, 128, 255, 0.92), rgba(95, 67, 210, 0.72));
  box-shadow: 0 0 30px rgba(64, 100, 255, 0.28);
}

.product-icon-featured {
  width: 5rem;
  height: 5rem;
  border-radius: 1.35rem;
  font-size: 1.35rem;
}

.product-label {
  margin: 0 0 0.75rem;
  color: var(--home-accent);
  font-size: 0.8rem;
  font-weight: 720;
}

.product-card h3 {
  margin: 0;
  color: var(--home-text-main);
  font-size: 1.25rem;
  font-weight: 720;
}

.product-card-featured h3 {
  font-size: clamp(2.2rem, 4vw, 3.8rem);
}

.product-card p {
  margin: 0;
  color: var(--home-text-muted);
  line-height: 1.8;
}

.product-card-featured p {
  max-width: 43rem;
  font-size: 1.05rem;
}

.product-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.55rem;
  margin-top: auto;
}

.product-tags span {
  padding: 0.42rem 0.68rem;
  border: 1px solid var(--home-border);
  border-radius: 999px;
  color: var(--home-text-soft);
  background: rgba(255, 255, 255, 0.035);
  font-size: 0.78rem;
}

.product-action {
  position: relative;
  z-index: 1;
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  width: fit-content;
  margin-top: 0.45rem;
  color: var(--home-accent);
  font-size: 0.9rem;
  font-weight: 700;
}

.product-action:hover {
  color: var(--home-text-main);
}

@media (max-width: 1100px) {
  .product-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .product-card-featured {
    grid-template-columns: 1fr;
    align-items: start;
  }
}

@media (max-width: 640px) {
  .product-grid {
    grid-template-columns: 1fr;
  }

  .product-card {
    min-height: auto;
  }
}
</style>
