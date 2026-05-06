<template>
  <div class="home-page">
    <header class="home-header">
      <div class="home-header-inner">
        <NuxtLink to="/" class="home-brand" aria-label="溪午听风首页">
          <span class="home-brand-mark">X</span>
          <span class="home-brand-text">
            <strong>溪午听风</strong>
            <small>AI 开发者 · 数字资产实践者</small>
          </span>
        </NuxtLink>

        <nav class="home-nav" aria-label="首页导航">
          <NuxtLink v-for="item in navItems" :key="item.href" :to="item.href">
            {{ item.label }}
          </NuxtLink>
        </nav>

        <div class="home-header-actions">
          <button type="button" class="home-icon-button" aria-label="切换主题" @click="toggleDark">
            <svg viewBox="0 0 24 24" aria-hidden="true">
              <path d="M12 7a5 5 0 1 0 0 10 5 5 0 0 0 0-10Zm0-2.25a1 1 0 0 0 1-1V2a1 1 0 1 0-2 0v1.75a1 1 0 0 0 1 1ZM12 22a1 1 0 0 0 1-1v-1.75a1 1 0 1 0-2 0V21a1 1 0 0 0 1 1ZM21 11h-1.75a1 1 0 1 0 0 2H21a1 1 0 1 0 0-2ZM4.75 12a1 1 0 0 0-1-1H2a1 1 0 1 0 0 2h1.75a1 1 0 0 0 1-1Zm13.55-5.9 1.23-1.23a1 1 0 0 0-1.42-1.42L16.9 4.7a1 1 0 0 0 1.41 1.41ZM5.7 17.9l-1.23 1.23a1 1 0 0 0 1.42 1.42l1.22-1.24A1 1 0 0 0 5.7 17.9Z" />
            </svg>
          </button>
          <NuxtLink to="/lab" class="home-platform-button">
            进入平台
            <span aria-hidden="true">→</span>
          </NuxtLink>
        </div>
      </div>
    </header>

    <main>
      <HomeHero />
      <HomeProducts />
      <HomeCapabilities />
      <HomeTimeline />
      <HomeAbout />
      <HomeCTA />
    </main>

    <footer class="home-footer">
      <div class="home-shell home-footer-inner">
        <div class="home-footer-brand">
          <span class="home-brand-mark">X</span>
          <div>
            <strong>溪午听风</strong>
            <p>专注 AI 应用、个人产品与数字资产构建。</p>
          </div>
        </div>
        <div class="home-footer-links">
          <a href="https://github.com/Lijing327" target="_blank" rel="noreferrer">GitHub</a>
          <NuxtLink to="/contact">微信</NuxtLink>
          <a href="mailto:linxiwanting@gmail.com">邮箱</a>
          <span>ICP备案：闽ICP备2022001234号-1</span>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import HomeAbout from '~/components/home/HomeAbout.vue'
import HomeCapabilities from '~/components/home/HomeCapabilities.vue'
import HomeCTA from '~/components/home/HomeCTA.vue'
import HomeHero from '~/components/home/HomeHero.vue'
import HomeProducts from '~/components/home/HomeProducts.vue'
import HomeTimeline from '~/components/home/HomeTimeline.vue'

definePageMeta({
  layout: false
})

const { toggleDark } = useTheme()

const navItems = [
  { label: '产品', href: '/products' },
  { label: '案例', href: '/projects' },
  { label: 'AI实验室', href: '/lab' },
  { label: '文章', href: '/blog' },
  { label: '关于', href: '/about' }
]

useHead({
  title: '溪午听风 - 用 AI 构建产品，创造长期价值',
  meta: [
    {
      name: 'description',
      content: '溪午听风的个人品牌官网，专注 AI 应用开发、企业数字化与个人产品构建，持续打造真正有价值的数字资产。'
    },
    { name: 'theme-color', content: '#020617' }
  ]
})
</script>

<style scoped>
.home-page {
  --home-bg: #020617;
  --home-card: rgba(255, 255, 255, 0.052);
  --home-card-hover: rgba(255, 255, 255, 0.075);
  --home-border: rgba(255, 255, 255, 0.1);
  --home-border-strong: rgba(146, 176, 255, 0.32);
  --home-text-main: #f8fbff;
  --home-text-muted: rgba(221, 230, 255, 0.72);
  --home-text-soft: rgba(221, 230, 255, 0.52);
  --home-accent: #6ea0ff;
  --home-radius: 1.35rem;
  --home-radius-lg: 1.5rem;
  --home-shadow-soft: 0 28px 80px rgba(0, 0, 0, 0.22);
  --home-shadow-glow: 0 28px 90px rgba(50, 95, 255, 0.16);
  min-height: 100vh;
  color: var(--home-text-main);
  background:
    radial-gradient(circle at 50% -10%, rgba(48, 86, 170, 0.22), transparent 38rem),
    linear-gradient(180deg, #020617 0%, #04101f 48%, #020617 100%);
  overflow: hidden;
}

.home-page :global(a) {
  color: inherit;
  text-decoration: none;
}

.home-page :deep(.home-shell) {
  width: min(100% - 2rem, 1180px);
  margin-inline: auto;
}

.home-header {
  position: fixed;
  top: 1rem;
  left: 0;
  right: 0;
  z-index: 1000;
  height: 72px;
  pointer-events: none;
}

.home-header-inner {
  width: min(100% - 2rem, 1180px);
  height: 72px;
  display: grid;
  grid-template-columns: auto minmax(0, 1fr) auto;
  align-items: center;
  gap: 2rem;
  margin-inline: auto;
  padding: 0 1rem;
  border: 1px solid var(--home-border);
  border-radius: 999px;
  background: rgba(2, 8, 23, 0.72);
  backdrop-filter: blur(22px);
  -webkit-backdrop-filter: blur(22px);
  box-shadow: 0 22px 60px rgba(0, 0, 0, 0.24);
  pointer-events: auto;
}

.home-brand,
.home-footer-brand {
  display: inline-flex;
  align-items: center;
  gap: 0.8rem;
  min-width: 0;
}

.home-brand-mark {
  width: 2.35rem;
  height: 2.35rem;
  display: grid;
  place-items: center;
  flex: 0 0 auto;
  border: 1px solid rgba(130, 170, 255, 0.36);
  border-radius: 0.82rem;
  color: var(--home-text-main);
  font-weight: 820;
  background: linear-gradient(135deg, rgba(36, 104, 255, 0.95), rgba(105, 78, 220, 0.82));
  box-shadow: 0 0 24px rgba(62, 105, 255, 0.35);
}

.home-brand-text {
  display: grid;
  gap: 0.18rem;
}

.home-brand-text strong,
.home-footer-brand strong {
  color: var(--home-text-main);
  font-size: 1rem;
  font-weight: 780;
  line-height: 1;
}

.home-brand-text small {
  color: var(--home-text-soft);
  font-size: 0.72rem;
  white-space: nowrap;
}

.home-nav {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: clamp(0.8rem, 2vw, 1.55rem);
}

.home-nav a {
  color: var(--home-text-muted);
  font-size: 0.92rem;
  transition: color 0.22s ease;
}

.home-nav a:hover {
  color: var(--home-text-main);
}

.home-header-actions {
  display: flex;
  align-items: center;
  gap: 0.55rem;
}

.home-icon-button,
.home-platform-button,
:deep(.home-button) {
  min-height: 2.7rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  border: 1px solid var(--home-border);
  border-radius: 999px;
  color: var(--home-text-main);
  background: rgba(255, 255, 255, 0.04);
  transition: transform 0.24s ease, border-color 0.24s ease, background 0.24s ease, box-shadow 0.24s ease;
}

.home-icon-button {
  width: 2.7rem;
  padding: 0;
}

.home-icon-button svg {
  width: 1.08rem;
  height: 1.08rem;
  fill: currentColor;
}

.home-platform-button,
:deep(.home-button) {
  padding: 0 1.15rem;
  font-size: 0.9rem;
  font-weight: 690;
}

.home-platform-button,
:deep(.home-button-primary) {
  border-color: rgba(112, 157, 255, 0.44);
  background: linear-gradient(135deg, rgba(39, 105, 255, 0.95), rgba(92, 80, 225, 0.92));
  box-shadow: 0 14px 36px rgba(45, 100, 255, 0.25);
}

:deep(.home-button-secondary) {
  color: var(--home-text-muted);
}

.home-icon-button:hover,
.home-platform-button:hover,
:deep(.home-button:hover) {
  transform: translateY(-0.14rem);
  border-color: var(--home-border-strong);
  background: rgba(255, 255, 255, 0.08);
  box-shadow: 0 18px 38px rgba(36, 82, 210, 0.18);
}

.home-section {
  position: relative;
  padding: clamp(5rem, 9vw, 8rem) 0;
}

:deep(.home-section-heading) {
  max-width: 40rem;
  margin-bottom: 2.6rem;
}

:deep(.home-section-kicker) {
  margin: 0;
  color: var(--home-accent);
  font-size: 0.78rem;
  font-weight: 760;
}

:deep(.home-section-heading h2) {
  margin: 0.8rem 0 0;
  color: var(--home-text-main);
  font-size: clamp(2.3rem, 5vw, 4.6rem);
  font-weight: 770;
  line-height: 1.02;
}

:deep(.home-section-heading p:last-child) {
  margin: 1rem 0 0;
  color: var(--home-text-muted);
  font-size: 1.04rem;
  line-height: 1.8;
}

:deep(.home-eyebrow) {
  width: fit-content;
  display: inline-flex;
  align-items: center;
  gap: 0.55rem;
  margin: 0;
  padding: 0.52rem 0.8rem;
  border: 1px solid var(--home-border);
  border-radius: 999px;
  color: var(--home-text-muted);
  background: rgba(255, 255, 255, 0.035);
  font-size: 0.84rem;
}

:deep(.home-eyebrow-dot) {
  width: 0.42rem;
  height: 0.42rem;
  border-radius: 50%;
  background: #38e6a5;
  box-shadow: 0 0 16px rgba(56, 230, 165, 0.72);
}

:deep(.reveal-up) {
  animation: homeRevealUp 0.8s ease both;
}

:deep(.reveal-delay-1) {
  animation-delay: 0.08s;
}

:deep(.reveal-delay-2) {
  animation-delay: 0.16s;
}

:deep(.reveal-delay-3) {
  animation-delay: 0.24s;
}

:deep(.reveal-delay-4) {
  animation-delay: 0.32s;
}

.home-footer {
  border-top: 1px solid var(--home-border);
  background: rgba(1, 6, 18, 0.58);
}

.home-footer-inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 2rem;
  padding: 2rem 0;
}

.home-footer-brand p {
  margin: 0.35rem 0 0;
  color: var(--home-text-soft);
  font-size: 0.9rem;
}

.home-footer-links {
  display: flex;
  flex-wrap: wrap;
  justify-content: flex-end;
  gap: 1rem;
  color: var(--home-text-soft);
  font-size: 0.9rem;
}

.home-footer-links a:hover {
  color: var(--home-text-main);
}

@keyframes homeRevealUp {
  from {
    opacity: 0;
    transform: translateY(1.4rem);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (max-width: 1100px) {
  .home-header-inner {
    grid-template-columns: auto auto;
  }

  .home-nav {
    display: none;
  }
}

@media (max-width: 680px) {
  .home-header {
    top: 0.65rem;
  }

  .home-header-inner {
    width: min(100% - 1rem, 1180px);
    gap: 0.75rem;
    padding: 0 0.75rem;
  }

  .home-brand-text small {
    display: none;
  }

  .home-icon-button {
    display: none;
  }

  .home-footer-inner {
    align-items: flex-start;
    flex-direction: column;
  }

  .home-footer-links {
    justify-content: flex-start;
  }
}
</style>
