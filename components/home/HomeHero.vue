<template>
  <section id="home" class="home-hero home-section">
    <div class="home-shell home-hero-grid">
      <div class="home-hero-copy reveal-up">
        <p class="home-eyebrow">
          <span class="home-eyebrow-dot"></span>
          构建有价值的数字资产
        </p>

        <h1 class="home-hero-title">
          用 <span>AI</span> 构建产品
          <br />
          创造长期价值
        </h1>

        <p class="home-hero-description">
          专注 AI 应用开发、企业数字化与个人产品构建，持续打造真正有价值的数字资产。
        </p>

        <div class="home-hero-actions" aria-label="首页主要操作">
          <NuxtLink to="/products" class="home-button home-button-primary">
            探索我的产品
            <span aria-hidden="true">→</span>
          </NuxtLink>
          <NuxtLink to="/about" class="home-button home-button-secondary">
            关于我
          </NuxtLink>
        </div>

        <dl class="home-hero-stats" aria-label="个人网站关键数据">
          <div v-for="stat in stats" :key="stat.label" class="home-stat">
            <dt>{{ stat.value }}</dt>
            <dd>{{ stat.label }}</dd>
          </div>
        </dl>
      </div>

      <div class="home-hero-visual reveal-up reveal-delay-2" aria-hidden="true">
        <div class="ai-orbit ai-orbit-one"></div>
        <div class="ai-orbit ai-orbit-two"></div>
        <div class="ai-particle particle-one"></div>
        <div class="ai-particle particle-two"></div>
        <div class="ai-particle particle-three"></div>

        <div class="ai-glass-frame">
          <div class="ai-glass-core">
            <div class="ai-core-face">
              <span>AI</span>
            </div>
          </div>
          <div class="ai-glass-base"></div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
interface HeroStat {
  value: string
  label: string
}

const stats: HeroStat[] = [
  { value: '12+', label: '产品项目' },
  { value: '50K+', label: 'GitHub 提交' },
  { value: '5年+', label: '开发年限' },
  { value: '1000+', label: '用户访问' }
]
</script>

<style scoped>
.home-hero {
  min-height: 100vh;
  display: flex;
  align-items: center;
  padding-top: 8rem;
}

.home-hero-grid {
  display: grid;
  grid-template-columns: minmax(0, 0.92fr) minmax(28rem, 1.08fr);
  align-items: center;
  gap: clamp(3rem, 7vw, 7rem);
}

.home-hero-copy {
  max-width: 42rem;
}

.home-hero-title {
  margin: 1.5rem 0 0;
  color: var(--home-text-main);
  font-size: clamp(3.5rem, 7vw, 6.8rem);
  font-weight: 780;
  line-height: 0.98;
}

.home-hero-title span {
  background: linear-gradient(135deg, #4f8cff 10%, #8c6dff 64%, #d8d4ff 100%);
  background-clip: text;
  -webkit-background-clip: text;
  color: transparent;
}

.home-hero-description {
  max-width: 34rem;
  margin: 1.75rem 0 0;
  color: var(--home-text-muted);
  font-size: 1.12rem;
  line-height: 1.9;
}

.home-hero-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.9rem;
  margin-top: 2.4rem;
}

.home-hero-stats {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 1.25rem;
  max-width: 38rem;
  margin: 3.2rem 0 0;
}

.home-stat {
  margin: 0;
}

.home-stat dt {
  color: var(--home-text-main);
  font-size: 1.35rem;
  font-weight: 760;
  line-height: 1;
}

.home-stat dd {
  margin: 0.55rem 0 0;
  color: var(--home-text-soft);
  font-size: 0.84rem;
}

.home-hero-visual {
  position: relative;
  min-height: 36rem;
  display: grid;
  place-items: center;
  perspective: 1200px;
}

.ai-glass-frame {
  position: relative;
  width: min(88vw, 34rem);
  aspect-ratio: 1 / 0.84;
  display: grid;
  place-items: center;
  border: 1px solid var(--home-border);
  border-radius: 2rem;
  background:
    linear-gradient(135deg, rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0.025)),
    rgba(9, 18, 38, 0.28);
  box-shadow:
    0 42px 120px rgba(14, 41, 95, 0.32),
    inset 0 1px 0 rgba(255, 255, 255, 0.16);
  transform: rotateX(10deg) rotateY(-12deg);
}

.ai-glass-frame::before {
  content: '';
  position: absolute;
  inset: 12%;
  border-radius: 1.5rem;
  background: radial-gradient(circle, rgba(70, 120, 255, 0.28), transparent 64%);
  filter: blur(2rem);
}

.ai-glass-core {
  position: relative;
  width: 14.8rem;
  aspect-ratio: 1;
  display: grid;
  place-items: center;
  border: 1px solid rgba(156, 190, 255, 0.3);
  border-radius: 2rem;
  background:
    linear-gradient(145deg, rgba(80, 120, 255, 0.42), rgba(85, 53, 183, 0.22)),
    rgba(16, 29, 67, 0.72);
  box-shadow:
    0 0 58px rgba(70, 116, 255, 0.42),
    inset 0 1px 0 rgba(255, 255, 255, 0.28),
    inset 0 -22px 42px rgba(11, 19, 42, 0.34);
  transform: translateZ(5rem) rotateZ(-2deg);
}

.ai-core-face {
  width: 10.2rem;
  aspect-ratio: 1;
  display: grid;
  place-items: center;
  border-radius: 1.5rem;
  background: linear-gradient(145deg, rgba(255, 255, 255, 0.18), rgba(255, 255, 255, 0.035));
  border: 1px solid rgba(255, 255, 255, 0.12);
  box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.22);
}

.ai-core-face span {
  color: var(--home-text-main);
  font-size: 4.8rem;
  font-weight: 860;
  text-shadow: 0 0 28px rgba(166, 148, 255, 0.62);
}

.ai-glass-base {
  position: absolute;
  bottom: 14%;
  width: 15rem;
  height: 1.2rem;
  border-radius: 999px;
  background: rgba(65, 104, 240, 0.18);
  filter: blur(0.2rem);
  transform: translateZ(2rem);
}

.ai-orbit {
  position: absolute;
  width: 92%;
  max-width: 43rem;
  aspect-ratio: 1 / 0.34;
  border-radius: 50%;
  border: 1px solid rgba(102, 148, 255, 0.34);
  box-shadow: 0 0 22px rgba(64, 115, 255, 0.24);
}

.ai-orbit-one {
  transform: rotate(-14deg);
}

.ai-orbit-two {
  width: 76%;
  transform: rotate(20deg);
  border-color: rgba(146, 115, 255, 0.24);
}

.ai-particle {
  position: absolute;
  width: 0.42rem;
  height: 0.42rem;
  border-radius: 50%;
  background: rgba(120, 160, 255, 0.95);
  box-shadow: 0 0 22px rgba(80, 125, 255, 0.92);
}

.particle-one {
  top: 20%;
  right: 12%;
}

.particle-two {
  left: 12%;
  bottom: 26%;
}

.particle-three {
  top: 34%;
  left: 22%;
  background: rgba(162, 126, 255, 0.95);
}

@media (max-width: 1024px) {
  .home-hero {
    padding-top: 7rem;
  }

  .home-hero-grid {
    grid-template-columns: 1fr;
    gap: 4rem;
  }

  .home-hero-visual {
    min-height: 28rem;
  }
}

@media (max-width: 640px) {
  .home-hero {
    min-height: auto;
    padding-top: 6.6rem;
  }

  .home-hero-title {
    font-size: clamp(3rem, 16vw, 4.2rem);
  }

  .home-hero-description {
    font-size: 1rem;
  }

  .home-hero-actions {
    flex-direction: column;
  }

  .home-hero-stats {
    grid-template-columns: repeat(2, minmax(0, 1fr));
    margin-top: 2.4rem;
  }

  .home-hero-visual {
    min-height: 22rem;
  }

  .ai-glass-core {
    width: 11rem;
    border-radius: 1.5rem;
  }

  .ai-core-face {
    width: 7.8rem;
  }

  .ai-core-face span {
    font-size: 3.6rem;
  }
}
</style>
