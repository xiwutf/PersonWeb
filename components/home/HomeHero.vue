<template>
  <section id="home" class="home-hero">
    <div class="home-hero-frame">
      <div class="home-space-scene" aria-hidden="true">
        <div class="home-starfield"></div>
        <div class="home-planet"></div>
        <div class="home-orbit home-orbit-one"></div>
        <div class="home-orbit home-orbit-two"></div>
        <div class="home-horizon"></div>
        <div class="home-traveler">
          <span class="home-traveler-head"></span>
          <span class="home-traveler-body"></span>
          <span class="home-traveler-legs"></span>
        </div>
      </div>

      <div class="home-hero-inner">
        <div class="home-hero-copy reveal-up">
          <p class="home-hero-kicker">WELCOME TO MY DIGITAL SPACE</p>

          <h1 class="home-hero-title">溪午听风</h1>

          <p class="home-hero-subtitle">个人数字资产 <span>×</span> AI 产品实验室</p>

          <p class="home-hero-description">
            专注于 AI、自动化与产品构建，<br>
            持续创造有价值的数字资产与解决方案。
          </p>

          <div class="home-hero-actions" aria-label="主要操作">
            <NuxtLink to="/projects" class="home-button home-button-primary">
              探索我的项目
              <span aria-hidden="true">→</span>
            </NuxtLink>
            <NuxtLink to="/about" class="home-button home-button-secondary">
              了解更多
              <span class="home-button-dot" aria-hidden="true">›</span>
            </NuxtLink>
          </div>

          <div class="home-hero-collab">
            <span class="home-avatar-stack" aria-hidden="true">
              <span></span>
              <span></span>
              <span></span>
            </span>
            <p>
              <strong>合作 / 咨询 / 项目交流</strong>
              <small>一起探索 AI 与数字化的无限可能</small>
            </p>
          </div>
        </div>

        <aside class="home-hero-quote reveal-up reveal-delay-2" aria-label="个人理念">
          <figure class="home-hero-portrait" aria-label="溪午听风个人照片">
            <img
              src="/images/avatar.jpg"
              alt="溪午听风个人照片"
              width="180"
              height="225"
              loading="eager"
              decoding="async"
              fetchpriority="high"
            >
          </figure>
          <span class="home-quote-mark">“</span>
          <p>用技术创造价值<br>用产品改变世界<br>用内容记录思考</p>
          <span class="home-signature">Xiuu</span>
        </aside>

        <div class="home-feature-grid reveal-up reveal-delay-3" aria-label="入口卡片">
          <NuxtLink v-for="card in featureCards" :key="card.title" :to="card.href" class="home-feature-card">
            <span class="home-feature-icon" :class="card.icon" aria-hidden="true"></span>
            <strong>{{ card.title }}</strong>
            <span>{{ card.description }}</span>
            <em aria-hidden="true">→</em>
            <span class="home-feature-art" :class="`${card.icon}-art`"></span>
          </NuxtLink>
        </div>

        <div class="home-bottom-bar reveal-up reveal-delay-4">
          <div class="home-build-state">
            <strong>持续构建中</strong>
            <span>Never Stop Building</span>
          </div>

          <dl class="home-hero-stats" aria-label="关键数据">
            <div v-for="stat in displayStats" :key="stat.label" class="home-stat">
              <dt>{{ stat.value }}</dt>
              <dd>{{ stat.label }}</dd>
            </div>
          </dl>

          <form class="home-subscribe" aria-label="订阅更新" @submit.prevent>
            <span class="home-mail-icon" aria-hidden="true"></span>
            <label>
              <strong>订阅我的更新</strong>
              <span>获取最新的项目与文章</span>
            </label>
            <div class="home-email-field">
              <input type="email" placeholder="输入你的邮箱">
              <button type="submit" aria-label="提交订阅">→</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import type { HomeStats } from '~/types/home'

const props = withDefaults(defineProps<{
  stats: HomeStats
  loading: boolean
}>(), {
  stats: () => ({ projects: 20, articles: 50, tools: 8 }),
  loading: false,
})

const displayStats = computed(() => [
  { value: props.loading ? '—' : `${props.stats.projects}+`, label: '产品与项目' },
  { value: props.loading ? '—' : `${props.stats.articles}+`, label: '文章分享' },
  { value: props.loading ? '—' : `${props.stats.tools}+`, label: '工具数量' },
  { value: '∞', label: '探索可能' },
])

const featureCards = [
  {
    title: '产品与工具',
    description: '我构建的数字产品与效率工具 提升效率，创造价值',
    href: '/products',
    icon: 'home-icon-cube'
  },
  {
    title: 'AI 实验室',
    description: '探索 AI 的边界与可能性 实验、创新、落地',
    href: '/lab',
    icon: 'home-icon-flask'
  },
  {
    title: '案例与项目',
    description: '精选项目与解决方案 从 0 到 1 的实践',
    href: '/projects',
    icon: 'home-icon-folder'
  },
  {
    title: '文章与思考',
    description: '记录我的思考与实践 分享知识，启发思考',
    href: '/blog',
    icon: 'home-icon-pen'
  },
  {
    title: '关于我',
    description: '了解更多关于我的故事 我的理念与追求',
    href: '/about',
    icon: 'home-icon-person'
  }
]
</script>

<style scoped>
.home-hero {
  box-sizing: border-box;
  min-height: 100vh;
  min-height: 100svh;
  height: 100vh;
  height: 100svh;
  padding: 1.7rem 2.4rem 2.1rem;
  background:
    radial-gradient(circle at 50% 38%, rgba(103, 137, 235, 0.24), transparent 43rem),
    radial-gradient(circle at 14% 18%, rgba(34, 211, 238, 0.08), transparent 28rem),
    var(--home-hero-bg, #07152f);
}

.home-hero-frame {
  position: relative;
  min-height: 100%;
  overflow: hidden;
  border: 1px solid var(--home-border);
  border-radius: 1.85rem;
  background:
    radial-gradient(circle at 74% 22%, rgba(116, 151, 255, 0.24), transparent 35rem),
    radial-gradient(circle at 15% 12%, rgba(34, 211, 238, 0.08), transparent 32rem),
    linear-gradient(180deg, var(--home-hero-frame-top, rgba(12, 31, 72, 0.94)) 0%, var(--home-hero-frame-bottom, rgba(7, 18, 45, 0.96)) 100%);
  box-shadow:
    0 28px 100px rgba(3, 12, 30, 0.28),
    inset 0 1px 0 rgba(255, 255, 255, 0.05);
  isolation: isolate;
}

.home-space-scene,
.home-starfield,
.home-space-scene::before,
.home-space-scene::after {
  position: absolute;
  inset: 0;
  pointer-events: none;
}

.home-space-scene {
  z-index: -1;
}

.home-starfield {
  opacity: 0.76;
  background-image:
    radial-gradient(circle, rgba(173, 196, 255, 0.92) 0 1px, transparent 1.5px),
    radial-gradient(circle, rgba(130, 100, 255, 0.7) 0 1px, transparent 1.4px),
    radial-gradient(circle, rgba(255, 255, 255, 0.58) 0 1px, transparent 1.2px);
  background-position: 9% 23%, 58% 14%, 88% 30%;
  background-size: 260px 220px, 360px 300px, 190px 180px;
}

.home-space-scene::before {
  content: '';
  background:
    radial-gradient(ellipse at 52% 92%, rgba(34, 60, 120, 0.78) 0 14%, rgba(18, 38, 82, 0.58) 28%, transparent 56%),
    linear-gradient(180deg, transparent 55%, rgba(8, 20, 48, 0.58) 82%, rgba(7, 18, 43, 0.82) 100%);
}

.home-space-scene::after {
  content: '';
  background:
    radial-gradient(ellipse at 82% 88%, rgba(7, 18, 43, 0.72) 0 11%, transparent 35%),
    radial-gradient(ellipse at 17% 82%, rgba(7, 18, 43, 0.66) 0 15%, transparent 38%);
}

.home-planet {
  position: absolute;
  top: -18.5rem;
  left: 43%;
  width: min(64vw, 58rem);
  aspect-ratio: 1;
  border-radius: 50%;
  background:
    radial-gradient(circle at 24% 72%, rgba(255, 255, 255, 0.72), transparent 0.35rem),
    radial-gradient(circle at 56% 68%, rgba(136, 161, 255, 0.34), transparent 0.22rem),
    radial-gradient(circle at 70% 37%, rgba(255, 255, 255, 0.28), transparent 0.18rem),
    radial-gradient(circle at 75% 70%, rgba(136, 108, 255, 0.38), transparent 0.3rem),
    radial-gradient(circle at 78% 45%, rgba(218, 231, 255, 0.92) 0 2%, rgba(143, 178, 255, 0.9) 8%, rgba(67, 97, 185, 0.76) 24%, rgba(16, 31, 68, 0.9) 55%, rgba(7, 17, 40, 0.94) 76%);
  box-shadow:
    -22px 28px 86px rgba(108, 149, 255, 0.42),
    inset -20px 10px 64px rgba(222, 235, 255, 0.26),
    inset 70px -70px 110px rgba(7, 17, 42, 0.76);
  opacity: 0.95;
}

.home-planet::before {
  content: '';
  position: absolute;
  inset: 9% 7% 16% 13%;
  border-radius: 50%;
  background:
    linear-gradient(120deg, transparent 0 38%, rgba(233, 241, 255, 0.42) 46%, transparent 54%),
    repeating-radial-gradient(ellipse at 58% 48%, rgba(255, 255, 255, 0.11) 0 2px, transparent 2px 16px);
  filter: blur(1px);
  opacity: 0.46;
}

.home-orbit {
  position: absolute;
  border: 1px solid rgba(94, 128, 220, 0.24);
  border-radius: 50%;
}

.home-orbit-one {
  top: -7%;
  left: 42%;
  width: 49rem;
  height: 72rem;
  transform: rotate(10deg);
}

.home-orbit-two {
  top: 5%;
  right: 4%;
  width: 44rem;
  height: 56rem;
  transform: rotate(-14deg);
}

.home-horizon {
  position: absolute;
  right: 5%;
  bottom: 19%;
  width: 71%;
  height: 24rem;
  background:
    linear-gradient(165deg, transparent 0 22%, rgba(28, 51, 104, 0.5) 23% 26%, transparent 27%),
    linear-gradient(152deg, transparent 0 32%, rgba(22, 41, 88, 0.68) 33% 38%, transparent 39%),
    linear-gradient(170deg, transparent 0 50%, rgba(12, 23, 55, 0.82) 51% 58%, transparent 59%);
  clip-path: polygon(0 60%, 12% 43%, 21% 52%, 33% 36%, 45% 56%, 57% 42%, 67% 58%, 78% 39%, 92% 55%, 100% 48%, 100% 100%, 0 100%);
  filter: drop-shadow(0 -12px 36px rgba(83, 122, 235, 0.2));
  opacity: 0.86;
}

.home-traveler {
  position: absolute;
  left: 61.5%;
  bottom: 34.5%;
  width: 3.2rem;
  height: 8.2rem;
  filter: drop-shadow(0 16px 16px rgba(0, 0, 0, 0.55));
}

.home-traveler-head,
.home-traveler-body,
.home-traveler-legs {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
  display: block;
}

.home-traveler-head {
  top: 0;
  width: 1rem;
  height: 1rem;
  border-radius: 50%;
  background: #0d1732;
  box-shadow: inset 0 0 0 2px rgba(134, 167, 255, 0.16);
}

.home-traveler-body {
  top: 1rem;
  width: 1.8rem;
  height: 4.6rem;
  border-radius: 0.75rem 0.75rem 0.42rem 0.42rem;
  background: linear-gradient(180deg, #22396d, #0c1838 68%, #071126);
}

.home-traveler-legs {
  bottom: 0;
  width: 1.45rem;
  height: 3rem;
  border-left: 0.42rem solid #081126;
  border-right: 0.42rem solid #081126;
}

.home-hero-inner {
  min-height: calc(100vh - 3.8rem);
  min-height: calc(100svh - 3.8rem);
  display: grid;
  grid-template-columns: minmax(0, 1fr) 21rem;
  grid-template-rows: 1fr auto auto;
  gap: 1.4rem;
  padding: clamp(8.8rem, 13vh, 11.2rem) min(5.4vw, 6.6rem) 1.25rem;
}

.home-hero-copy {
  max-width: 43rem;
  align-self: center;
}

.home-hero-kicker {
  margin: 0 0 1.55rem;
  color: rgba(212, 222, 255, 0.7);
  font-size: 0.88rem;
  letter-spacing: 0.12em;
}

.home-hero-title {
  margin: 0;
  width: fit-content;
  color: var(--home-text-main);
  font-size: clamp(4.2rem, 7.8vw, 6.65rem);
  font-weight: 820;
  line-height: 0.94;
  background: linear-gradient(98deg, #f7fbff 0%, #dcecff 42%, #ad91ff 88%);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  text-shadow: 0 0 40px rgba(116, 147, 255, 0.14);
}

.home-hero-subtitle {
  margin: 1.2rem 0 0;
  color: rgba(238, 243, 255, 0.88);
  font-size: clamp(1.25rem, 2vw, 1.72rem);
  font-weight: 680;
  letter-spacing: 0.05em;
}

.home-hero-subtitle span {
  margin-inline: 0.6rem;
  color: rgba(142, 156, 255, 0.76);
}

.home-hero-description {
  margin: 1.8rem 0 0;
  color: var(--home-text-muted);
  font-size: 1.1rem;
  line-height: 1.9;
}

.home-hero-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  margin-top: 2.45rem;
}

.home-button-dot {
  width: 1.25rem;
  height: 1.25rem;
  display: inline-grid;
  place-items: center;
  border: 1px solid rgba(150, 178, 255, 0.5);
  border-radius: 50%;
  line-height: 1;
}

.home-hero-collab {
  display: flex;
  align-items: center;
  gap: 0.95rem;
  margin-top: 2rem;
}

.home-avatar-stack {
  display: flex;
  width: 4.35rem;
}

.home-avatar-stack span {
  width: 1.72rem;
  height: 1.72rem;
  border: 2px solid rgba(9, 19, 44, 0.9);
  border-radius: 50%;
  background:
    radial-gradient(circle at 50% 35%, rgba(255, 236, 220, 0.9) 0 20%, transparent 21%),
    linear-gradient(135deg, #c0c7d9, #364665);
  box-shadow: 0 0 18px rgba(126, 150, 255, 0.22);
}

.home-avatar-stack span + span {
  margin-left: -0.55rem;
}

.home-avatar-stack span:nth-child(2) {
  background:
    radial-gradient(circle at 50% 35%, rgba(255, 236, 220, 0.9) 0 20%, transparent 21%),
    linear-gradient(135deg, #aab7d9, #263558);
}

.home-avatar-stack span:nth-child(3) {
  background:
    radial-gradient(circle at 50% 35%, rgba(255, 236, 220, 0.9) 0 20%, transparent 21%),
    linear-gradient(135deg, #bfc4d7, #513d62);
}

.home-hero-collab p {
  display: grid;
  gap: 0.18rem;
  margin: 0;
}

.home-hero-collab strong {
  color: rgba(242, 246, 255, 0.88);
  font-size: 0.96rem;
}

.home-hero-collab small {
  color: var(--home-text-soft);
  font-size: 0.84rem;
}

.home-hero-quote {
  position: relative;
  align-self: center;
  justify-self: end;
  width: min(100%, 18rem);
  margin-top: 3rem;
  color: rgba(221, 231, 255, 0.82);
  text-shadow: 0 0 24px rgba(4, 10, 28, 0.55);
}

.home-hero-portrait {
  position: absolute;
  top: -6.15rem;
  right: 0.35rem;
  width: 8.1rem;
  aspect-ratio: 4 / 5;
  margin: 0;
  overflow: hidden;
  border: 1px solid rgba(210, 224, 255, 0.62);
  border-radius: 1rem;
  background: rgba(248, 250, 252, 0.96);
  box-shadow:
    0 22px 58px rgba(8, 18, 45, 0.34),
    0 0 0 0.45rem rgba(97, 132, 255, 0.1),
    inset 0 1px 0 rgba(255, 255, 255, 0.72);
  transform: rotate(1.5deg);
}

.home-hero-portrait::before {
  content: '';
  position: absolute;
  inset: -32% -38% auto auto;
  width: 6rem;
  height: 6rem;
  border-radius: 50%;
  background: rgba(127, 147, 255, 0.24);
  filter: blur(18px);
  pointer-events: none;
  z-index: 1;
}

.home-hero-portrait img {
  position: relative;
  z-index: 2;
  display: block;
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.home-quote-mark {
  display: block;
  color: rgba(205, 219, 255, 0.5);
  font-size: 5rem;
  font-family: Georgia, serif;
  line-height: 0.7;
}

.home-hero-quote p {
  margin: 1rem 0 1.8rem;
  font-size: 1.02rem;
  line-height: 1.8;
}

.home-signature {
  color: rgba(229, 237, 255, 0.82);
  font-family: "Segoe Script", "Brush Script MT", cursive;
  font-size: 2.55rem;
  font-weight: 300;
}

.home-feature-grid {
  grid-column: 1 / -1;
  display: grid;
  grid-template-columns: repeat(5, minmax(0, 1fr));
  gap: 1.45rem;
}

.home-feature-card {
  position: relative;
  min-height: 14.4rem;
  overflow: hidden;
  padding: 1.65rem 1.75rem;
  border: 1px solid rgba(151, 179, 255, 0.22);
  border-radius: 1rem;
  background:
    linear-gradient(180deg, rgba(48, 76, 145, 0.74), rgba(14, 31, 70, 0.62)),
    var(--home-card);
  box-shadow:
    inset 0 1px 0 rgba(255, 255, 255, 0.09),
    0 20px 44px rgba(4, 13, 34, 0.2);
  transition: transform 0.25s ease, border-color 0.25s ease, background 0.25s ease;
}

.home-feature-card:hover {
  transform: translateY(-0.35rem);
  border-color: rgba(153, 178, 255, 0.42);
  background:
    linear-gradient(180deg, rgba(60, 90, 166, 0.82), rgba(18, 38, 84, 0.72)),
    var(--home-card-hover);
}

.home-feature-icon {
  position: relative;
  z-index: 2;
  display: block;
  width: 1.45rem;
  height: 1.45rem;
  margin-bottom: 0.8rem;
}

.home-feature-card strong {
  position: relative;
  z-index: 2;
  display: block;
  color: rgba(248, 251, 255, 0.96);
  font-size: 1.1rem;
  font-weight: 760;
}

.home-feature-card > span:not(.home-feature-icon):not(.home-feature-art) {
  position: relative;
  z-index: 2;
  display: block;
  max-width: 13rem;
  margin-top: 0.7rem;
  color: rgba(211, 224, 255, 0.7);
  font-size: 0.87rem;
  line-height: 1.65;
}

.home-feature-card em {
  position: absolute;
  z-index: 2;
  left: 1.75rem;
  bottom: 1.65rem;
  color: #a7bdff;
  font-size: 1.55rem;
  font-style: normal;
}

.home-feature-art {
  position: absolute;
  right: 1.15rem;
  bottom: 0.9rem;
  width: 8.4rem;
  height: 6.2rem;
  opacity: 0.9;
}

.home-icon-cube::before {
  content: '';
  position: absolute;
  inset: 0.15rem;
  background: linear-gradient(135deg, #c7d8ff, #5f7bff);
  clip-path: polygon(50% 0, 95% 24%, 95% 74%, 50% 100%, 5% 74%, 5% 24%);
}

.home-icon-flask::before {
  content: '';
  position: absolute;
  left: 0.42rem;
  top: 0.05rem;
  width: 0.62rem;
  height: 1.25rem;
  border: 2px solid #bcd0ff;
  border-top: 0;
  border-radius: 0 0 0.28rem 0.28rem;
}

.home-icon-folder::before {
  content: '';
  position: absolute;
  inset: 0.35rem 0.05rem 0.22rem;
  border-radius: 0.22rem;
  background: linear-gradient(180deg, #8da5ff, #455ce6);
  box-shadow: 0 -0.35rem 0 -0.12rem #88a1ff;
}

.home-icon-pen::before {
  content: '';
  position: absolute;
  left: 0.55rem;
  top: 0.05rem;
  width: 0.42rem;
  height: 1.45rem;
  border-radius: 999px;
  background: linear-gradient(180deg, #d8e2ff, #6c78ff);
  transform: rotate(38deg);
}

.home-icon-person::before {
  content: '';
  position: absolute;
  left: 0.38rem;
  top: 0.08rem;
  width: 0.72rem;
  height: 0.72rem;
  border-radius: 50%;
  background: #bcd0ff;
  box-shadow: 0 0.75rem 0 0.2rem #6c78ff;
}

.home-icon-cube-art {
  background:
    linear-gradient(135deg, rgba(192, 212, 255, 0.96), rgba(84, 109, 255, 0.72)) 38% 18% / 2.25rem 2.25rem,
    linear-gradient(135deg, rgba(192, 212, 255, 0.9), rgba(84, 109, 255, 0.62)) 18% 48% / 2.1rem 2.1rem,
    linear-gradient(135deg, rgba(166, 188, 255, 0.88), rgba(88, 80, 224, 0.64)) 58% 47% / 2.25rem 2.25rem,
    radial-gradient(ellipse at center, rgba(86, 108, 255, 0.34), transparent 68%);
  background-repeat: no-repeat;
  filter: drop-shadow(0 12px 22px rgba(88, 103, 255, 0.42));
}

.home-icon-flask-art::before {
  content: '';
  position: absolute;
  left: 2.15rem;
  bottom: 0.5rem;
  width: 4.4rem;
  height: 5.3rem;
  border: 2px solid rgba(165, 192, 255, 0.72);
  border-radius: 0.8rem 0.8rem 2.15rem 2.15rem;
  background: radial-gradient(ellipse at 50% 72%, rgba(91, 91, 255, 0.8), transparent 52%);
  transform: rotate(-7deg);
  box-shadow: 0 0 24px rgba(91, 110, 255, 0.48);
}

.home-icon-folder-art::before {
  content: '';
  position: absolute;
  inset: 1.15rem 0.45rem 0.7rem;
  border: 1px solid rgba(162, 185, 255, 0.6);
  border-radius: 0.7rem;
  background: linear-gradient(135deg, rgba(102, 113, 255, 0.85), rgba(24, 36, 92, 0.8));
  transform: rotate(3deg);
  box-shadow: -1rem 0.55rem 0 rgba(93, 106, 255, 0.22);
}

.home-icon-pen-art::before {
  content: '';
  position: absolute;
  right: 1rem;
  bottom: 0.1rem;
  width: 4.5rem;
  height: 5.8rem;
  border: 1px solid rgba(166, 190, 255, 0.52);
  border-radius: 0.55rem;
  background:
    repeating-linear-gradient(180deg, rgba(178, 197, 255, 0.85) 0 0.26rem, transparent 0.26rem 0.9rem),
    linear-gradient(135deg, rgba(96, 112, 255, 0.7), rgba(20, 32, 82, 0.86));
  transform: rotate(12deg);
  box-shadow: 0 0 24px rgba(91, 110, 255, 0.34);
}

.home-icon-person-art::before {
  content: '';
  position: absolute;
  right: 1rem;
  bottom: 0.2rem;
  width: 4.1rem;
  height: 4.9rem;
  border-radius: 50% 50% 42% 42%;
  background:
    radial-gradient(circle at 50% 19%, rgba(210, 224, 255, 0.96) 0 23%, transparent 24%),
    radial-gradient(ellipse at 50% 72%, rgba(125, 148, 255, 0.94) 0 44%, transparent 45%);
  box-shadow: 0 0 30px rgba(103, 132, 255, 0.58);
}

.home-bottom-bar {
  grid-column: 1 / -1;
  display: grid;
  grid-template-columns: minmax(13rem, 17rem) minmax(24rem, 1fr) minmax(28rem, 39rem);
  align-items: center;
  gap: 1.5rem;
  min-height: 6.6rem;
  margin: 0 0.75rem;
  padding: 1.1rem 1.7rem;
  border: 1px solid rgba(151, 179, 255, 0.14);
  border-radius: 999px;
  background: rgba(11, 27, 62, 0.68);
  box-shadow:
    inset 0 1px 0 rgba(255, 255, 255, 0.06),
    0 20px 50px rgba(4, 13, 34, 0.2);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
}

.home-build-state {
  padding-left: 1.1rem;
}

.home-build-state strong,
.home-subscribe label strong {
  display: block;
  color: rgba(247, 250, 255, 0.94);
  font-size: 0.98rem;
}

.home-build-state span,
.home-subscribe label span {
  display: block;
  margin-top: 0.35rem;
  color: var(--home-text-soft);
  font-size: 0.83rem;
}

.home-hero-stats {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 1rem;
  margin: 0;
  padding: 0;
}

.home-stat {
  margin: 0;
  border-left: 1px solid rgba(151, 179, 255, 0.1);
  padding-left: 1.3rem;
}

.home-stat dt {
  color: #7f93ff;
  font-size: 1.75rem;
  font-weight: 760;
  line-height: 1;
}

.home-stat dd {
  margin: 0.35rem 0 0;
  color: var(--home-text-soft);
  font-size: 0.78rem;
}

.home-subscribe {
  display: grid;
  grid-template-columns: auto minmax(9rem, 1fr) minmax(15rem, 20rem);
  align-items: center;
  gap: 1rem;
  margin: 0;
  padding-left: 1.5rem;
  border-left: 1px solid rgba(151, 179, 255, 0.1);
}

.home-mail-icon {
  width: 1.45rem;
  height: 1rem;
  border: 1px solid rgba(194, 210, 255, 0.72);
  border-radius: 0.14rem;
}

.home-mail-icon::before {
  content: '';
  display: block;
  width: 0.82rem;
  height: 0.82rem;
  margin: -0.1rem auto 0;
  border-right: 1px solid rgba(194, 210, 255, 0.72);
  border-bottom: 1px solid rgba(194, 210, 255, 0.72);
  transform: rotate(45deg);
}

.home-email-field {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto;
  align-items: center;
  min-height: 3.2rem;
  overflow: hidden;
  border: 1px solid rgba(151, 179, 255, 0.12);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.035);
}

.home-email-field input {
  width: 100%;
  min-width: 0;
  border: 0;
  outline: 0;
  padding: 0 1rem 0 1.3rem;
  color: rgba(245, 248, 255, 0.92);
  background: transparent;
  font: inherit;
}

.home-email-field input::placeholder {
  color: rgba(190, 202, 235, 0.46);
}

.home-email-field button {
  width: 2.65rem;
  height: 2.65rem;
  margin-right: 0.28rem;
  border: 0;
  border-radius: 50%;
  color: #ffffff;
  background: linear-gradient(135deg, #6377ff, #5867ff);
  box-shadow: 0 0 24px rgba(93, 107, 255, 0.52);
  cursor: pointer;
}

@media (max-width: 1280px) {
  .home-hero-inner {
    grid-template-columns: 1fr;
    padding-top: 8rem;
  }

  .home-hero-quote {
    display: none;
  }

  .home-feature-grid {
    grid-template-columns: repeat(3, minmax(0, 1fr));
  }

  .home-bottom-bar {
    grid-template-columns: 1fr;
    border-radius: 1.25rem;
  }

  .home-subscribe {
    border-left: 0;
    padding-left: 0;
  }
}

@media (min-width: 821px) and (max-height: 980px) {
  .home-hero-inner {
    width: 116%;
    min-height: calc((100vh - 3.8rem) / 0.86);
    min-height: calc((100svh - 3.8rem) / 0.86);
    padding-top: 7.7rem;
    transform: scale(0.86);
    transform-origin: top left;
  }
}

@media (max-width: 820px) {
  .home-hero {
    height: auto;
    padding: 0.65rem;
  }

  .home-hero-frame {
    min-height: auto;
  }

  .home-hero-inner {
    min-height: auto;
    padding: 6.2rem 1rem 1rem;
  }

  .home-planet {
    top: -6rem;
    left: 30%;
    width: 36rem;
  }

  .home-traveler,
  .home-horizon {
    display: none;
  }

  .home-hero-title {
    font-size: clamp(3.4rem, 17vw, 4.5rem);
  }

  .home-hero-subtitle {
    font-size: 1.05rem;
  }

  .home-feature-grid {
    grid-template-columns: 1fr;
    gap: 0.9rem;
  }

  .home-feature-card {
    min-height: 10.8rem;
  }

  .home-hero-stats {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .home-subscribe {
    grid-template-columns: auto 1fr;
  }

  .home-email-field {
    grid-column: 1 / -1;
  }
}
</style>
