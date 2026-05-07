<template>
  <section id="about" class="journey-showcase">
    <div class="journey-frame">
      <div class="journey-stars" aria-hidden="true"></div>

      <div class="journey-head reveal-up">
        <div class="journey-title">
          <span>JOURNEY</span>
          <h2>我的旅程 <em>&amp;</em> <strong>构建之路</strong></h2>
          <p>持续构建，不断进化，把想法变成可落地的数字产品与系统。</p>
        </div>

        <blockquote class="journey-quote">
          <span aria-hidden="true">“</span>
          <p>所有的构建，都是为了<br>更自由地创造价值，解决问题，帮助更多人。</p>
          <cite>Xiuu</cite>
        </blockquote>
      </div>

      <div class="journey-timeline reveal-up reveal-delay-1">
        <svg class="journey-wave" viewBox="0 0 1560 86" preserveAspectRatio="none" aria-hidden="true">
          <path d="M0 54 C 110 54, 120 35, 220 35 S 330 68, 455 49 S 595 27, 710 48 S 840 71, 955 48 S 1090 30, 1210 48 S 1350 66, 1450 47 S 1510 41, 1560 51" />
          <path class="journey-wave-dash" d="M1450 47 C 1495 43, 1525 45, 1560 51" />
        </svg>

        <article
          v-for="(item, index) in displayTimeline"
          :key="item.id ?? item.year"
          class="journey-step"
          :class="{ 'is-current': item.isNow }"
        >
          <span v-if="item.isNow" class="journey-now">NOW</span>
          <div class="journey-step-copy">
            <strong>{{ item.year }}</strong>
            <h3>{{ item.title }}</h3>
            <p>{{ item.description }}</p>
          </div>
          <span class="journey-dot" :class="getDotTheme(item, index)" aria-hidden="true"></span>
          <div class="journey-card">
            <span class="journey-icon" :class="item.icon || 'icon-cube'" aria-hidden="true"></span>
          </div>
        </article>
      </div>

      <div class="journey-bottom reveal-up reveal-delay-2">
        <div class="journey-planet" aria-hidden="true">
          <span></span>
        </div>

        <div class="journey-bottom-copy">
          <h3>构建没有终点</h3>
          <p>每一步都是新的起点，每一个项目都是一次成长。<br>未来的路还很长，一起探索，一起创造。</p>
        </div>

        <dl class="journey-stats">
          <div v-for="stat in stats" :key="stat.label">
            <span :class="stat.icon" aria-hidden="true"></span>
            <dt>{{ stat.value }}</dt>
            <dd>{{ stat.label }}</dd>
          </div>
        </dl>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import type { HomeJourneyItem } from '~/types/home'

const props = withDefaults(defineProps<{
  journey: HomeJourneyItem[]
  loading: boolean
}>(), {
  journey: () => [],
  loading: false,
})

const FALLBACK_TIMELINE: HomeJourneyItem[] = [
  { id: 0, year: 2022, title: '探索开始', description: '接触编程与自动化，开启数字世界的探索之旅。', icon: 'icon-terminal', color: 'blue', isNow: false },
  { id: 1, year: 2024, title: '产品探索', description: '从想法到原型，开始构建自己的产品。', icon: 'icon-cube', color: 'blue', isNow: false },
  { id: 2, year: 2025, title: '系统构建', description: '专注产品化与系统化，打造可复用的解决方案。', icon: 'icon-layers', color: 'purple', isNow: true },
]

const displayTimeline = computed<HomeJourneyItem[]>(() =>
  props.journey.length > 0 ? props.journey : FALLBACK_TIMELINE
)

const getDotTheme = (item: HomeJourneyItem, index: number): string => {
  if (item.color) return `dot-${item.color}`
  return index % 2 === 0 ? 'dot-blue' : 'dot-purple'
}

const stats = [
  { value: '3+', label: '年探索与构建', icon: 'stat-cube' },
  { value: '10+', label: '项目与产品', icon: 'stat-folder' },
  { value: '20K+', label: '代码提交', icon: 'stat-code' },
  { value: '1000+', label: '用户与读者', icon: 'stat-users' }
]
</script>

<style scoped>
.journey-showcase {
  padding: 1.7rem 2.4rem 2.1rem;
  background:
    radial-gradient(circle at 30% 10%, rgba(54, 88, 171, 0.13), transparent 31rem),
    radial-gradient(circle at 75% 30%, rgba(91, 66, 180, 0.1), transparent 30rem),
    #020713;
}

.journey-frame {
  position: relative;
  width: min(100%, 1840px);
  min-height: calc(100vh - 3.8rem);
  min-height: calc(100svh - 3.8rem);
  margin: 0 auto;
  padding: 8.65rem min(4.6vw, 5.8rem) 2.25rem;
  overflow: hidden;
  border: 1px solid var(--home-border);
  border-radius: 1.85rem;
  background:
    radial-gradient(circle at 26% 16%, rgba(44, 82, 174, 0.13), transparent 32rem),
    radial-gradient(circle at 70% 48%, rgba(65, 49, 162, 0.09), transparent 35rem),
    linear-gradient(180deg, rgba(4, 13, 34, 0.98), rgba(2, 8, 24, 0.99));
  box-shadow:
    0 28px 100px rgba(0, 0, 0, 0.34),
    inset 0 1px 0 rgba(255, 255, 255, 0.05);
}

.journey-stars {
  position: absolute;
  inset: 6.8rem 4rem auto 4rem;
  height: 22rem;
  pointer-events: none;
  background-image:
    radial-gradient(circle, rgba(129, 158, 255, 0.6) 0 1px, transparent 1.3px),
    radial-gradient(circle, rgba(149, 106, 255, 0.42) 0 1px, transparent 1.3px);
  background-position: 0 0, 4.5rem 2.5rem;
  background-size: 9.5rem 5.9rem, 11.3rem 7.1rem;
  opacity: 0.52;
}

.journey-head,
.journey-timeline,
.journey-bottom {
  position: relative;
  z-index: 1;
}

.journey-head {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(24rem, 33rem);
  gap: 2.5rem;
  align-items: start;
}

.journey-title > span {
  display: inline-flex;
  min-height: 1.7rem;
  align-items: center;
  padding: 0 0.72rem;
  border: 1px solid rgba(128, 151, 255, 0.22);
  border-radius: 999px;
  color: #a997ff;
  background: rgba(83, 72, 184, 0.13);
  font-size: 0.78rem;
  font-weight: 760;
  letter-spacing: 0.08em;
}

.journey-title h2 {
  margin: 0.75rem 0 0;
  color: rgba(250, 252, 255, 0.98);
  font-size: clamp(2.3rem, 3.35vw, 3.7rem);
  line-height: 1.08;
  font-weight: 860;
}

.journey-title h2 em {
  color: rgba(219, 226, 255, 0.92);
  font-style: normal;
}

.journey-title h2 strong {
  color: #8f72ff;
  text-shadow: 0 0 25px rgba(118, 92, 255, 0.42);
}

.journey-title p {
  margin: 1rem 0 0;
  color: rgba(216, 226, 255, 0.72);
  font-size: 1.04rem;
}

.journey-quote {
  position: relative;
  min-height: 7.5rem;
  margin: 0;
  padding: 1.6rem 2rem 1.2rem 3.25rem;
  border: 1px solid rgba(151, 179, 255, 0.13);
  border-radius: 1.25rem;
  background: linear-gradient(180deg, rgba(7, 18, 43, 0.54), rgba(5, 13, 34, 0.28));
}

.journey-quote > span {
  position: absolute;
  left: 1.18rem;
  top: 0.96rem;
  color: rgba(142, 157, 230, 0.68);
  font-family: Georgia, serif;
  font-size: 3.65rem;
  line-height: 1;
}

.journey-quote p {
  margin: 0;
  color: rgba(235, 241, 255, 0.86);
  font-size: 1.05rem;
  line-height: 1.85;
}

.journey-quote cite {
  position: absolute;
  right: 4.2rem;
  bottom: -1rem;
  color: #a277ff;
  font-family: "Brush Script MT", "Segoe Script", cursive;
  font-size: 2.5rem;
  font-style: italic;
  font-weight: 300;
}

.journey-timeline {
  display: grid;
  grid-template-columns: repeat(6, minmax(0, 1fr));
  gap: clamp(1.4rem, 2.8vw, 3.6rem);
  margin-top: 4.9rem;
}

.journey-wave {
  position: absolute;
  left: -3.4rem;
  right: -3.4rem;
  top: 12.55rem;
  width: calc(100% + 6.8rem);
  height: 5.5rem;
  pointer-events: none;
}

.journey-wave path:first-child {
  fill: none;
  stroke: #5f79ff;
  stroke-width: 2;
  filter: drop-shadow(0 0 8px rgba(85, 98, 255, 0.45));
}

.journey-wave-dash {
  fill: none;
  stroke: rgba(139, 108, 255, 0.36);
  stroke-width: 2;
  stroke-dasharray: 9 9;
}

.journey-step {
  position: relative;
  min-height: 30rem;
  display: grid;
  grid-template-rows: 8.7rem 4.8rem 1fr;
  justify-items: center;
}

.journey-step.is-current {
  margin-top: -2.4rem;
  padding: 2.2rem 1.15rem 1.35rem;
  border: 1px solid rgba(154, 103, 255, 0.62);
  border-radius: 1.35rem;
  background:
    radial-gradient(circle at 50% 18%, rgba(119, 76, 255, 0.24), transparent 11rem),
    linear-gradient(180deg, rgba(38, 25, 82, 0.76), rgba(16, 20, 51, 0.65));
  box-shadow:
    0 0 40px rgba(101, 73, 225, 0.18),
    inset 0 1px 0 rgba(255, 255, 255, 0.08);
}

.journey-now {
  position: absolute;
  top: -0.8rem;
  left: 50%;
  transform: translateX(-50%);
  min-width: 4.7rem;
  min-height: 1.8rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 999px;
  color: #fff;
  background: linear-gradient(135deg, #8d72ff, #a05cff);
  font-size: 0.78rem;
  font-weight: 760;
  box-shadow: 0 10px 24px rgba(127, 86, 255, 0.42);
}

.journey-step-copy {
  display: grid;
  justify-items: center;
  align-content: start;
  text-align: center;
}

.journey-step-copy strong {
  color: #5f98ff;
  font-size: 1.82rem;
  line-height: 1;
  font-weight: 820;
}

.journey-step:nth-of-type(2n) .journey-step-copy strong,
.journey-step.is-current .journey-step-copy strong,
.journey-step:last-child .journey-step-copy strong {
  color: #a66cff;
}

.journey-step-copy h3 {
  margin: 0.95rem 0 0;
  color: rgba(250, 252, 255, 0.96);
  font-size: 1.12rem;
  font-weight: 780;
}

.journey-step-copy p {
  margin: 0.75rem 0 0;
  color: rgba(216, 226, 255, 0.7);
  font-size: 0.92rem;
  line-height: 1.75;
}

.journey-dot {
  position: relative;
  z-index: 2;
  align-self: center;
  width: 1.58rem;
  height: 1.58rem;
  border-radius: 50%;
  background: #68a0ff;
  box-shadow:
    0 0 0 1.1rem rgba(67, 117, 255, 0.11),
    0 0 0 2rem rgba(67, 117, 255, 0.05),
    0 0 28px rgba(93, 143, 255, 0.88);
}

.journey-dot.dot-purple {
  background: #b37cff;
  box-shadow:
    0 0 0 1.1rem rgba(145, 80, 255, 0.11),
    0 0 0 2rem rgba(145, 80, 255, 0.05),
    0 0 28px rgba(168, 100, 255, 0.88);
}

.journey-card {
  width: 100%;
  min-height: 13.8rem;
  display: grid;
  align-content: start;
  justify-items: center;
  padding: 1.7rem 1.35rem 1.15rem;
  border: 1px solid rgba(145, 172, 255, 0.17);
  border-radius: 0.95rem;
  background:
    linear-gradient(180deg, rgba(11, 25, 56, 0.72), rgba(5, 14, 34, 0.62)),
    rgba(6, 16, 39, 0.7);
  box-shadow:
    inset 0 1px 0 rgba(255, 255, 255, 0.05),
    0 18px 42px rgba(0, 0, 0, 0.2);
}

.journey-step.is-current .journey-card {
  border-color: rgba(164, 122, 255, 0.5);
  background:
    linear-gradient(180deg, rgba(38, 26, 79, 0.78), rgba(16, 20, 51, 0.64)),
    rgba(6, 16, 39, 0.7);
}

.journey-icon {
  position: relative;
  width: 4.1rem;
  height: 4.1rem;
  display: grid;
  place-items: center;
  margin-bottom: 1.1rem;
  border-radius: 50%;
  background:
    radial-gradient(circle at center, rgba(95, 127, 255, 0.34), transparent 2.2rem),
    rgba(21, 36, 86, 0.38);
  color: #6fa1ff;
}

.journey-card ul {
  width: 100%;
  display: grid;
  gap: 0.7rem;
  margin: 0;
  padding: 0;
  list-style: none;
}

.journey-card li {
  position: relative;
  padding-left: 1rem;
  color: rgba(218, 228, 255, 0.72);
  font-size: 0.9rem;
  line-height: 1.3;
}

.journey-card li::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0.5rem;
  width: 0.32rem;
  height: 0.32rem;
  border-radius: 50%;
  background: #678eff;
  box-shadow: 0 0 10px rgba(103, 142, 255, 0.72);
}

.journey-step:nth-of-type(2n) .journey-card li::before,
.journey-step.is-current .journey-card li::before,
.journey-step:last-child .journey-card li::before {
  background: #a26cff;
  box-shadow: 0 0 10px rgba(162, 108, 255, 0.72);
}

.journey-bottom {
  min-height: 12.5rem;
  display: grid;
  grid-template-columns: 23rem minmax(20rem, 1fr) minmax(34rem, 1.3fr);
  align-items: center;
  gap: 2.3rem;
  margin: 2.2rem 2rem 0;
  overflow: hidden;
  border: 1px solid rgba(151, 179, 255, 0.18);
  border-radius: 1rem;
  background:
    linear-gradient(90deg, rgba(16, 35, 82, 0.62), rgba(6, 15, 38, 0.72)),
    rgba(7, 18, 42, 0.76);
}

.journey-planet {
  position: relative;
  align-self: stretch;
  min-height: 12.5rem;
  overflow: hidden;
  background:
    radial-gradient(circle at 14% 90%, rgba(96, 102, 255, 0.78), transparent 8rem),
    radial-gradient(circle at 0% 108%, #0e1e5f 0 10rem, transparent 10.2rem),
    linear-gradient(180deg, rgba(34, 55, 128, 0.18), rgba(4, 13, 35, 0.14));
}

.journey-planet::before {
  content: '';
  position: absolute;
  left: -5.4rem;
  bottom: -7.4rem;
  width: 20rem;
  height: 20rem;
  border-radius: 50%;
  background:
    radial-gradient(circle at 48% 30%, rgba(127, 148, 255, 0.95), transparent 0.45rem),
    radial-gradient(circle at 50% 50%, rgba(50, 74, 171, 0.95), #06153d 63%);
  box-shadow:
    0 0 60px rgba(87, 112, 255, 0.48),
    inset 1rem 1rem 2.6rem rgba(193, 211, 255, 0.16);
}

.journey-planet::after {
  content: '';
  position: absolute;
  left: -3.6rem;
  bottom: 4.8rem;
  width: 17rem;
  height: 2px;
  background: linear-gradient(90deg, transparent, rgba(185, 205, 255, 0.92), transparent);
  transform: rotate(10deg);
  filter: blur(0.2px);
}

.journey-bottom-copy h3 {
  margin: 0;
  color: rgba(250, 252, 255, 0.97);
  font-size: 1.3rem;
  font-weight: 820;
}

.journey-bottom-copy p {
  margin: 0.9rem 0 0;
  color: rgba(213, 224, 255, 0.67);
  font-size: 0.95rem;
  line-height: 1.75;
}

.journey-stats {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  margin: 0;
  padding-right: 2.2rem;
}

.journey-stats div {
  display: grid;
  grid-template-columns: 2.5rem auto;
  grid-template-rows: auto auto;
  gap: 0 0.8rem;
  align-items: center;
}

.journey-stats span {
  grid-row: 1 / 3;
  position: relative;
  width: 2.3rem;
  height: 2.3rem;
  color: #8b72ff;
}

.journey-stats dt {
  color: rgba(224, 233, 255, 0.96);
  font-size: 1.75rem;
  font-weight: 820;
  line-height: 1;
}

.journey-stats dd {
  margin: 0.6rem 0 0;
  color: rgba(196, 210, 249, 0.62);
  font-size: 0.86rem;
}

.journey-icon::before,
.journey-icon::after,
.journey-stats span::before,
.journey-stats span::after {
  content: '';
  position: absolute;
}

.icon-terminal::before {
  inset: 1rem 0.9rem;
  border: 1px solid currentColor;
  border-radius: 0.22rem;
  box-shadow: inset 0 0 16px rgba(92, 136, 255, 0.22);
}

.icon-terminal::after {
  left: 1.42rem;
  top: 1.62rem;
  width: 1.15rem;
  height: 0.75rem;
  border-right: 2px solid currentColor;
  border-bottom: 2px solid currentColor;
  transform: rotate(-45deg);
}

.icon-brain::before {
  inset: 0.98rem;
  border: 2px solid #a06cff;
  border-radius: 52% 48% 44% 56%;
  box-shadow: 0 0 18px rgba(160, 108, 255, 0.55);
}

.icon-brain::after {
  left: 1.45rem;
  top: 1.15rem;
  width: 1.2rem;
  height: 1.55rem;
  border-left: 2px solid #a06cff;
  border-bottom: 2px solid #a06cff;
  border-radius: 0 0 0 1rem;
}

.icon-cube::before {
  left: 1.25rem;
  top: 1.12rem;
  width: 1.65rem;
  height: 1.65rem;
  border: 2px solid currentColor;
  transform: rotate(30deg) skew(-8deg, -8deg);
}

.icon-cube::after {
  left: 2.05rem;
  top: 1.35rem;
  height: 1.7rem;
  border-left: 2px solid currentColor;
  transform: rotate(30deg);
}

.icon-layers::before,
.icon-layers::after {
  left: 1.06rem;
  width: 2rem;
  height: 2rem;
  border: 2px solid #a06cff;
  border-radius: 0.28rem;
  transform: rotate(45deg) scaleY(0.55);
}

.icon-layers::before {
  top: 0.9rem;
}

.icon-layers::after {
  top: 1.55rem;
  opacity: 0.74;
}

.icon-orbit::before {
  left: 1rem;
  top: 1.1rem;
  width: 2rem;
  height: 2rem;
  border-radius: 50%;
  background: radial-gradient(circle at 32% 26%, rgba(162, 194, 255, 0.95), #1e58ff 64%, #08266d);
  box-shadow: 0 0 20px rgba(81, 128, 255, 0.58);
}

.icon-orbit::after {
  left: 0.68rem;
  top: 1.75rem;
  width: 2.85rem;
  height: 0.9rem;
  border: 2px solid currentColor;
  border-radius: 50%;
  transform: rotate(-22deg);
}

.icon-future::before {
  content: '☆';
  inset: 0.58rem 0 0;
  display: grid;
  place-items: center;
  color: #a06cff;
  font-size: 3rem;
  line-height: 1;
  text-shadow: 0 0 18px rgba(160, 108, 255, 0.6);
}

.stat-cube::before {
  inset: 0.42rem;
  border: 2px solid currentColor;
  transform: rotate(30deg) skew(-8deg, -8deg);
}

.stat-folder::before {
  left: 0.2rem;
  top: 0.7rem;
  width: 1.9rem;
  height: 1.25rem;
  border: 2px solid currentColor;
  border-radius: 0.2rem;
}

.stat-folder::after {
  left: 0.32rem;
  top: 0.46rem;
  width: 0.82rem;
  height: 0.38rem;
  border: 2px solid currentColor;
  border-bottom: 0;
  border-radius: 0.2rem 0.2rem 0 0;
}

.stat-code::before {
  content: '</>';
  inset: 0.32rem 0 0;
  display: grid;
  place-items: center;
  color: currentColor;
  font-size: 1.45rem;
  font-weight: 820;
}

.stat-users::before {
  left: 0.28rem;
  top: 0.35rem;
  width: 0.64rem;
  height: 0.64rem;
  border: 2px solid currentColor;
  border-radius: 50%;
  box-shadow: 0.78rem 0.1rem 0 -0.08rem transparent, 0.78rem 0.1rem 0 0 currentColor;
}

.stat-users::after {
  left: 0.12rem;
  bottom: 0.28rem;
  width: 1.85rem;
  height: 0.82rem;
  border: 2px solid currentColor;
  border-radius: 1rem 1rem 0.2rem 0.2rem;
}

@media (max-width: 1320px) {
  .journey-timeline {
    grid-template-columns: repeat(3, minmax(0, 1fr));
  }

  .journey-wave {
    display: none;
  }

  .journey-bottom {
    grid-template-columns: 16rem 1fr;
  }

  .journey-stats {
    grid-column: 1 / -1;
    padding: 0 2rem 1.5rem;
  }
}

@media (max-width: 860px) {
  .journey-showcase {
    padding: 0.8rem;
  }

  .journey-frame {
    padding: 6.2rem 1rem 1rem;
    border-radius: 1.2rem;
  }

  .journey-head,
  .journey-timeline,
  .journey-bottom,
  .journey-stats {
    grid-template-columns: 1fr;
  }

  .journey-step,
  .journey-step.is-current {
    min-height: auto;
    margin-top: 0;
    grid-template-rows: auto auto auto;
  }

  .journey-bottom {
    margin-inline: 0;
  }
}
</style>
