<template>
  <section id="about" class="home-section">
    <div class="home-shell home-about">
      <div class="home-about-copy reveal-up">
        <p class="home-section-kicker">About</p>
        <h2>溪午听风</h2>
        <div class="about-text">
          <p>我是一名长期关注 AI 应用、全栈开发与数字资产建设的产品型开发者。</p>
          <p>现在主要在做三件事：把 AI 能力接入真实业务场景，构建可持续迭代的个人产品，并用工程化方式沉淀可复用的系统能力。</p>
          <p>我做这些产品，不是为了堆功能，而是为了让技术最终变成可使用、可维护、可增长的价值。</p>
          <p>长期目标是建立一套属于个人开发者的数字资产体系，让产品、内容、工具和经验彼此复利。</p>
        </div>

        <div class="about-tags" aria-label="技术标签">
          <span v-for="tag in tags" :key="tag">{{ tag }}</span>
        </div>
      </div>

      <div class="home-about-portrait reveal-up reveal-delay-2">
        <div class="portrait-frame">
          <img src="/images/avatar.jpg" alt="溪午听风头像" />
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const tags = [
  'Vue3',
  'Nuxt3',
  '.NET 8',
  'TypeScript',
  'Python',
  'FastAPI',
  'SQL Server',
  'Docker',
  'AI / RAG'
]
</script>

<style scoped>
.home-about {
  display: grid;
  grid-template-columns: minmax(0, 0.62fr) minmax(18rem, 0.38fr);
  gap: clamp(2.5rem, 7vw, 6rem);
  align-items: center;
  padding-block: 3rem;
}

.home-about-copy h2 {
  margin: 0.7rem 0 0;
  color: var(--home-text-main);
  font-size: clamp(2.5rem, 5vw, 4.8rem);
  font-weight: 780;
  line-height: 1;
}

.about-text {
  display: grid;
  gap: 1.15rem;
  max-width: 47rem;
  margin-top: 2rem;
}

.about-text p {
  margin: 0;
  color: var(--home-text-muted);
  font-size: 1.04rem;
  line-height: 1.95;
}

.about-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.62rem;
  margin-top: 2.3rem;
}

.about-tags span {
  padding: 0.5rem 0.74rem;
  border: 1px solid var(--home-border);
  border-radius: 999px;
  color: var(--home-text-soft);
  background: rgba(255, 255, 255, 0.035);
  font-size: 0.82rem;
}

.home-about-portrait {
  display: flex;
  justify-content: center;
}

.portrait-frame {
  position: relative;
  width: min(72vw, 22rem);
  aspect-ratio: 1;
  padding: 0.8rem;
  border: 1px solid var(--home-border);
  border-radius: 2rem;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.12), rgba(255, 255, 255, 0.035));
  box-shadow: var(--home-shadow-soft);
}

.portrait-frame::before {
  content: '';
  position: absolute;
  inset: 14% -10% -10% 14%;
  border-radius: 50%;
  background: rgba(69, 108, 255, 0.2);
  filter: blur(2.4rem);
}

.portrait-frame img {
  position: relative;
  z-index: 1;
  width: 100%;
  height: 100%;
  display: block;
  object-fit: cover;
  border-radius: 1.45rem;
  filter: saturate(0.92) contrast(1.04);
}

@media (max-width: 860px) {
  .home-about {
    grid-template-columns: 1fr;
  }

  .home-about-portrait {
    justify-content: flex-start;
  }
}
</style>
