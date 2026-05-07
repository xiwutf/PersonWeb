<template>
  <section id="products" class="home-products-showcase">
    <div class="home-products-frame">
      <div class="products-section-head reveal-up">
        <div>
          <h2>产品与项目</h2>
          <p>正在构建的数字产品与工具，让想法落地，创造价值</p>
        </div>

        <NuxtLink to="/projects" class="products-more-link">
          查看全部项目
          <span aria-hidden="true">→</span>
        </NuxtLink>
      </div>

      <div class="products-row reveal-up reveal-delay-1" aria-label="产品与项目列表">
        <!-- loading 骨架 -->
        <template v-if="loading">
          <span v-for="n in 5" :key="n" class="product-show-card product-show-card--skeleton"></span>
        </template>
        <!-- 真实数据 -->
        <template v-else-if="projects.length > 0">
          <NuxtLink
            v-for="project in projects"
            :key="project.id"
            :to="project.demoUrl || '/projects'"
            class="product-show-card"
          >
            <span class="product-art" :class="getProductArt(project)" aria-hidden="true"></span>
            <strong>{{ project.title }}</strong>
            <span class="product-desc">{{ project.description }}</span>
            <span class="product-tags">
              <em v-for="tag in project.techStack.slice(0, 2)" :key="tag">{{ tag }}</em>
            </span>
            <span class="product-arrow" aria-hidden="true">→</span>
          </NuxtLink>
        </template>
      </div>

      <div class="ai-lab-panel reveal-up reveal-delay-2">
        <div class="lab-panel-head">
          <div>
            <h2>AI 实验室</h2>
            <p>探索 AI 的边界，构建未来的生产力</p>
          </div>
          <NuxtLink to="/lab" class="products-more-link products-more-link-plain">
            进入实验室
            <span aria-hidden="true">→</span>
          </NuxtLink>
        </div>

        <div class="lab-card-grid">
          <article class="lab-feature-card">
            <div class="lab-feature-media" aria-hidden="true">
              <span class="lab-cylinder"></span>
              <span class="lab-brain"></span>
            </div>
            <div class="lab-feature-copy">
              <p class="lab-feature-status"><span></span>当前研究</p>
              <h3>多模态 AI Agent</h3>
              <p>构建具备感知、记忆、规划与执行能力的通用 Agent</p>
              <ul>
                <li>多模态感知与理解</li>
                <li>长期记忆与知识管理</li>
                <li>任务规划与自动化执行</li>
              </ul>
              <NuxtLink to="/lab" class="lab-detail-link">
                了解详情
                <span aria-hidden="true">→</span>
              </NuxtLink>
            </div>
          </article>

          <NuxtLink v-for="item in labItems" :key="item.title" :to="item.href" class="lab-mini-card">
            <span class="lab-mini-art" :class="item.art" aria-hidden="true"></span>
            <strong>{{ item.title }}</strong>
            <span>{{ item.description }}</span>
            <em>探索 →</em>
          </NuxtLink>
        </div>

        <div class="lab-stats-bar" aria-label="实验室数据">
          <dl class="lab-stats">
            <div v-for="stat in stats" :key="stat.label" class="lab-stat">
              <span :class="stat.icon" aria-hidden="true"></span>
              <dt>{{ stat.value }}</dt>
              <dd>{{ stat.label }}</dd>
            </div>
          </dl>

          <blockquote>
            <span aria-hidden="true">“</span>
            <p>在探索中构建，在构建中思考，<br>用技术创造价值，用产品改变世界。</p>
            <cite>Xiuu</cite>
          </blockquote>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import type { HomeProjectCard } from '~/types/home'

const props = withDefaults(defineProps<{
  projects: HomeProjectCard[]
  loading: boolean
}>(), {
  projects: () => [],
  loading: false,
})

const getProductArt = (project: HomeProjectCard): string => {
  const tech = project.techStack.join(' ').toLowerCase()
  const title = project.title.toLowerCase()
  if (tech.includes('ai') || title.includes('ai') || title.includes('智能')) return 'product-art-ai'
  if (title.includes('联谊') || title.includes('社交') || title.includes('心动')) return 'product-art-heart'
  if (tech.includes('.net') || tech.includes('python') || tech.includes('fastapi')) return 'product-art-code'
  if (tech.includes('vue') || tech.includes('nuxt') || tech.includes('react')) return 'product-art-dashboard'
  return 'product-art-cube'
}

const labItems = [
  {
    title: 'AI 工作流',
    description: '自动化流程设计与优化',
    href: '/lab',
    art: 'lab-art-cubes'
  },
  {
    title: 'RAG 知识库',
    description: '企业级知识搜索与问答系统',
    href: '/lab',
    art: 'lab-art-rag'
  },
  {
    title: '实验项目',
    description: '前沿 AI 技术验证与实验',
    href: '/lab',
    art: 'lab-art-planet'
  }
]

const stats = [
  { value: '12+', label: '实验项目', icon: 'stat-icon-flask' },
  { value: '5', label: '进行中', icon: 'stat-icon-cube' },
  { value: '20K+', label: '代码提交', icon: 'stat-icon-code' },
  { value: '30+', label: '技术文章', icon: 'stat-icon-star' },
  { value: '8.2K+', label: '社区关注', icon: 'stat-icon-users' }
]
</script>
<style scoped>
.home-products-showcase {
  padding: 1.7rem 2.4rem 2.1rem;
  background:
    radial-gradient(circle at 70% 12%, rgba(62, 88, 180, 0.13), transparent 35rem),
    #020713;
}

.home-products-frame {
  width: min(100%, 1840px);
  margin: 0 auto;
  min-height: calc(100vh - 3.8rem);
  min-height: calc(100svh - 3.8rem);
  padding: 8rem min(5.2vw, 6rem) 1.9rem;
  overflow: hidden;
  border: 1px solid var(--home-border);
  border-radius: 1.85rem;
  background:
    radial-gradient(circle at 45% 7%, rgba(66, 91, 192, 0.16), transparent 31rem),
    linear-gradient(180deg, rgba(4, 13, 35, 0.95), rgba(2, 8, 25, 0.98));
  box-shadow:
    0 28px 100px rgba(0, 0, 0, 0.32),
    inset 0 1px 0 rgba(255, 255, 255, 0.05);
}

.products-section-head,
.lab-panel-head {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 2rem;
}

.products-section-head h2,
.lab-panel-head h2 {
  margin: 0;
  color: rgba(248, 251, 255, 0.98);
  font-size: 1.72rem;
  font-weight: 780;
  line-height: 1.1;
}

.products-section-head p,
.lab-panel-head p {
  margin: 0.62rem 0 0;
  color: var(--home-text-muted);
  font-size: 1rem;
}

.products-more-link {
  min-height: 3rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.65rem;
  margin-top: 0.2rem;
  padding: 0 1.55rem;
  border: 1px solid rgba(115, 150, 255, 0.32);
  border-radius: 999px;
  color: #9db5ff;
  background: rgba(255, 255, 255, 0.028);
  font-size: 0.95rem;
  font-weight: 680;
  transition: transform 0.24s ease, border-color 0.24s ease, background 0.24s ease;
}

.products-more-link:hover {
  transform: translateY(-0.16rem);
  border-color: rgba(150, 178, 255, 0.5);
  background: rgba(255, 255, 255, 0.06);
}

.products-row {
  display: grid;
  grid-template-columns: repeat(5, minmax(0, 1fr));
  gap: 1.45rem;
  margin-top: 1.65rem;
}

.product-show-card,
.lab-feature-card,
.lab-mini-card {
  position: relative;
  overflow: hidden;
  border: 1px solid rgba(151, 179, 255, 0.18);
  border-radius: 1rem;
  background:
    linear-gradient(180deg, rgba(12, 26, 57, 0.8), rgba(4, 12, 31, 0.72)),
    rgba(8, 18, 42, 0.76);
  box-shadow:
    inset 0 1px 0 rgba(255, 255, 255, 0.05),
    0 18px 42px rgba(0, 0, 0, 0.2);
  transition: transform 0.25s ease, border-color 0.25s ease, background 0.25s ease;
}

.product-show-card:hover,
.lab-mini-card:hover {
  transform: translateY(-0.36rem);
  border-color: rgba(146, 176, 255, 0.42);
  background:
    linear-gradient(180deg, rgba(21, 40, 82, 0.88), rgba(5, 14, 36, 0.82)),
    rgba(8, 18, 42, 0.84);
}

.product-show-card {
  min-height: 17.1rem;
  padding: 1.35rem 1.25rem 1.1rem;
}

.product-art {
  position: relative;
  display: block;
  width: 100%;
  height: 6.45rem;
  margin-bottom: 0.45rem;
}

.product-show-card strong,
.lab-mini-card strong {
  display: block;
  color: rgba(248, 251, 255, 0.96);
  font-size: 1.12rem;
  font-weight: 780;
}

.product-desc {
  display: block;
  margin-top: 0.45rem;
  color: rgba(211, 224, 255, 0.7);
  font-size: 0.86rem;
  line-height: 1.55;
}

.product-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.45rem;
  margin-top: 1rem;
  padding-right: 3rem;
}

.product-tags em {
  padding: 0.34rem 0.58rem;
  border-radius: 999px;
  color: rgba(222, 232, 255, 0.8);
  background: rgba(90, 119, 192, 0.18);
  font-size: 0.78rem;
  font-style: normal;
}

.product-arrow {
  position: absolute;
  right: 1.15rem;
  bottom: 1.15rem;
  width: 2.55rem;
  height: 2.55rem;
  display: grid;
  place-items: center;
  border: 1px solid rgba(118, 150, 255, 0.2);
  border-radius: 50%;
  color: #9bb2ff;
  background: rgba(66, 91, 180, 0.18);
  box-shadow: 0 0 24px rgba(89, 106, 255, 0.16);
}

.product-art-ai::before {
  content: 'AI';
  position: absolute;
  left: 50%;
  top: 50%;
  width: 5.4rem;
  height: 5.4rem;
  display: grid;
  place-items: center;
  border: 1px solid rgba(139, 160, 255, 0.75);
  border-radius: 0.78rem;
  color: rgba(229, 237, 255, 0.82);
  background: linear-gradient(145deg, rgba(139, 102, 255, 0.98), rgba(39, 62, 168, 0.86));
  box-shadow:
    0 0 36px rgba(111, 91, 255, 0.58),
    inset 0 1px 0 rgba(255, 255, 255, 0.28);
  font-size: 2rem;
  font-weight: 820;
  transform: translate(-50%, -50%);
}

.product-art-pet::before {
  content: '';
  position: absolute;
  left: 50%;
  top: 47%;
  width: 5.3rem;
  height: 5.3rem;
  border-radius: 48% 48% 44% 44%;
  background:
    radial-gradient(circle at 35% 48%, #6ee8ff 0 0.38rem, transparent 0.43rem),
    radial-gradient(circle at 65% 48%, #6ee8ff 0 0.38rem, transparent 0.43rem),
    radial-gradient(circle at 48% 62%, rgba(255, 255, 255, 0.76) 0 0.12rem, transparent 0.16rem),
    linear-gradient(145deg, #16294f, #050b18);
  box-shadow:
    0 0 40px rgba(80, 191, 255, 0.38),
    inset 0 1px 0 rgba(255, 255, 255, 0.18);
  transform: translate(-50%, -50%);
}

.product-art-pet::after {
  content: '';
  position: absolute;
  left: 50%;
  top: 19%;
  width: 5.8rem;
  height: 2.8rem;
  background:
    linear-gradient(135deg, transparent 0 38%, #213968 39% 72%, transparent 73%),
    linear-gradient(225deg, transparent 0 38%, #213968 39% 72%, transparent 73%);
  transform: translateX(-50%);
  filter: drop-shadow(0 0 10px rgba(94, 189, 255, 0.6));
}

.product-art-heart::before {
  content: '';
  position: absolute;
  left: 50%;
  top: 48%;
  width: 5.3rem;
  height: 4.7rem;
  background:
    radial-gradient(circle at 38% 48%, rgba(236, 231, 255, 0.92) 0 0.62rem, transparent 0.66rem),
    radial-gradient(circle at 63% 48%, rgba(236, 231, 255, 0.92) 0 0.62rem, transparent 0.66rem),
    radial-gradient(circle at 38% 66%, rgba(236, 231, 255, 0.72) 0 1rem, transparent 1.04rem),
    radial-gradient(circle at 63% 66%, rgba(236, 231, 255, 0.72) 0 1rem, transparent 1.04rem),
    linear-gradient(135deg, #6d42ff, #e255ff);
  clip-path: path('M 50 90 C 18 62 5 43 16 22 C 25 4 45 8 50 24 C 55 8 75 4 84 22 C 95 43 82 62 50 90 Z');
  box-shadow: 0 0 42px rgba(180, 79, 255, 0.58);
  transform: translate(-50%, -50%);
}

.product-art-dashboard::before,
.product-art-code::before {
  content: '';
  position: absolute;
  left: 50%;
  top: 48%;
  width: 8.1rem;
  height: 5rem;
  border: 1px solid rgba(108, 134, 255, 0.52);
  border-radius: 0.45rem;
  background:
    linear-gradient(90deg, rgba(115, 143, 255, 0.52) 0 0.4rem, transparent 0.4rem 100%) 1rem 1rem / 1rem 0.3rem,
    linear-gradient(90deg, rgba(115, 143, 255, 0.28) 0 3.3rem, transparent 3.3rem 100%) 2rem 1rem / 6.2rem 0.38rem,
    linear-gradient(135deg, rgba(36, 56, 140, 0.64), rgba(5, 13, 34, 0.72));
  background-repeat: repeat-y, repeat-y, no-repeat;
  box-shadow: 0 0 34px rgba(70, 91, 220, 0.36);
  transform: translate(-50%, -50%);
}

.product-art-dashboard::after {
  content: '';
  position: absolute;
  left: 48%;
  top: 50%;
  width: 4.8rem;
  height: 2rem;
  border-left: 2px solid #718cff;
  border-bottom: 2px solid #718cff;
  transform: translate(-10%, -35%) skewX(-28deg);
}

.product-art-code::before {
  background: linear-gradient(135deg, rgba(20, 33, 86, 0.64), rgba(4, 12, 31, 0.72));
}

.product-art-code::after {
  content: '</>';
  position: absolute;
  left: 50%;
  top: 48%;
  color: #7f93ff;
  font-size: 2.65rem;
  font-weight: 780;
  transform: translate(-50%, -50%);
  text-shadow: 0 0 28px rgba(95, 111, 255, 0.58);
}

.ai-lab-panel {
  margin-top: 2rem;
  padding: 1.55rem 2rem 1.25rem;
  border: 1px solid rgba(151, 179, 255, 0.15);
  border-radius: 1.4rem;
  background:
    radial-gradient(circle at 34% 2%, rgba(44, 72, 165, 0.12), transparent 25rem),
    rgba(3, 12, 31, 0.72);
}

.products-more-link-plain {
  border-color: transparent;
  background: transparent;
}

.lab-card-grid {
  display: grid;
  grid-template-columns: minmax(28rem, 2.55fr) repeat(3, minmax(0, 1fr));
  gap: 1.25rem;
  margin-top: 1.25rem;
}

.lab-feature-card {
  display: grid;
  grid-template-columns: minmax(13rem, 1.05fr) minmax(16rem, 1fr);
  min-height: 15.4rem;
}

.lab-feature-media {
  position: relative;
  overflow: hidden;
  border-right: 1px solid rgba(151, 179, 255, 0.12);
  background:
    radial-gradient(ellipse at 50% 70%, rgba(49, 92, 204, 0.42), transparent 58%),
    linear-gradient(180deg, rgba(8, 21, 52, 0.96), rgba(2, 8, 24, 0.96));
}

.lab-feature-media::before {
  content: '';
  position: absolute;
  inset: 1.2rem;
  border-radius: 50%;
  background:
    repeating-radial-gradient(ellipse at center, rgba(65, 145, 255, 0.18) 0 2px, transparent 2px 18px);
  opacity: 0.55;
}

.lab-cylinder {
  position: absolute;
  left: 50%;
  top: 17%;
  width: 6.2rem;
  height: 10.7rem;
  border: 2px solid rgba(93, 178, 255, 0.65);
  border-radius: 50% / 10%;
  transform: translateX(-50%);
  box-shadow:
    0 0 34px rgba(38, 145, 255, 0.5),
    inset 0 0 34px rgba(38, 145, 255, 0.2);
}

.lab-cylinder::before,
.lab-cylinder::after {
  content: '';
  position: absolute;
  left: 50%;
  width: 8.4rem;
  height: 1.2rem;
  border: 2px solid rgba(72, 162, 255, 0.74);
  border-radius: 50%;
  transform: translateX(-50%);
  box-shadow: 0 0 24px rgba(58, 152, 255, 0.75);
}

.lab-cylinder::before {
  top: -0.65rem;
}

.lab-cylinder::after {
  bottom: -0.55rem;
}

.lab-brain {
  position: absolute;
  left: 50%;
  top: 49%;
  width: 4.1rem;
  height: 3.1rem;
  border: 2px solid rgba(100, 211, 255, 0.8);
  border-radius: 45% 45% 38% 38%;
  transform: translate(-50%, -50%);
  box-shadow: 0 0 30px rgba(72, 190, 255, 0.8);
}

.lab-brain::before {
  content: '';
  position: absolute;
  inset: 0.45rem 0.65rem;
  border-top: 2px solid rgba(100, 211, 255, 0.65);
  border-left: 2px solid rgba(100, 211, 255, 0.45);
  border-radius: 50%;
}

.lab-feature-copy {
  padding: 1.25rem 1.6rem;
}

.lab-feature-status {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  margin: 0 0 0.8rem;
  color: #e580ff;
  font-size: 0.82rem;
  font-weight: 720;
}

.lab-feature-status span {
  width: 0.46rem;
  height: 0.46rem;
  border-radius: 50%;
  background: #e580ff;
  box-shadow: 0 0 14px rgba(229, 128, 255, 0.8);
}

.lab-feature-copy h3,
.lab-mini-card strong {
  margin: 0;
  color: rgba(248, 251, 255, 0.96);
  font-size: 1.22rem;
  font-weight: 780;
}

.lab-feature-copy p:not(.lab-feature-status) {
  margin: 0.5rem 0 1rem;
  color: rgba(211, 224, 255, 0.72);
  font-size: 0.92rem;
  line-height: 1.65;
}

.lab-feature-copy ul {
  display: grid;
  gap: 0.48rem;
  margin: 0 0 1rem;
  padding: 0;
  list-style: none;
}

.lab-feature-copy li {
  display: flex;
  align-items: center;
  gap: 0.62rem;
  color: rgba(190, 204, 241, 0.76);
  font-size: 0.88rem;
}

.lab-feature-copy li::before {
  content: '';
  width: 0.32rem;
  height: 0.32rem;
  border-radius: 50%;
  background: #7f93ff;
  box-shadow: 0 0 10px rgba(127, 147, 255, 0.7);
}

.lab-detail-link {
  min-height: 2.4rem;
  display: inline-flex;
  align-items: center;
  gap: 0.7rem;
  padding: 0 1.2rem;
  border: 1px solid rgba(151, 179, 255, 0.22);
  border-radius: 999px;
  color: #9db5ff;
  font-weight: 680;
  background: rgba(255, 255, 255, 0.028);
}

.lab-mini-card {
  min-height: 15.4rem;
  padding: 1.3rem 1.4rem;
}

.lab-mini-art {
  position: relative;
  display: block;
  height: 5.8rem;
  margin-bottom: 0.95rem;
}

.lab-mini-card span:not(.lab-mini-art) {
  display: block;
  margin-top: 0.52rem;
  color: rgba(211, 224, 255, 0.72);
  font-size: 0.95rem;
  line-height: 1.6;
}

.lab-mini-card em {
  position: absolute;
  left: 1.55rem;
  bottom: 1.25rem;
  color: #9db5ff;
  font-style: normal;
  font-weight: 680;
}

.lab-art-cubes {
  background:
    linear-gradient(135deg, rgba(190, 214, 255, 0.96), rgba(83, 111, 255, 0.75)) 42% 10% / 2.4rem 2.4rem,
    linear-gradient(135deg, rgba(168, 194, 255, 0.92), rgba(66, 90, 221, 0.72)) 26% 45% / 2.15rem 2.15rem,
    linear-gradient(135deg, rgba(168, 194, 255, 0.86), rgba(66, 90, 221, 0.64)) 58% 45% / 2.15rem 2.15rem,
    radial-gradient(ellipse at center, rgba(76, 97, 255, 0.28), transparent 68%);
  background-repeat: no-repeat;
}

.lab-art-rag::before {
  content: '';
  position: absolute;
  left: 50%;
  top: 50%;
  width: 5.8rem;
  height: 5.8rem;
  border: 0.8rem solid rgba(105, 178, 255, 0.8);
  border-bottom-color: rgba(67, 91, 220, 0.9);
  border-radius: 0.7rem;
  transform: translate(-50%, -50%) rotate(45deg);
  filter: drop-shadow(0 0 28px rgba(95, 150, 255, 0.55));
}

.lab-art-rag::after {
  content: '';
  position: absolute;
  left: 50%;
  top: 51%;
  width: 2.3rem;
  height: 2.3rem;
  background: rgba(5, 13, 34, 0.78);
  transform: translate(-50%, -50%) rotate(45deg);
}

.lab-art-planet::before {
  content: '';
  position: absolute;
  left: 50%;
  top: 50%;
  width: 5.6rem;
  height: 5.6rem;
  border-radius: 50%;
  background: radial-gradient(circle at 35% 25%, rgba(226, 236, 255, 0.95), rgba(61, 88, 226, 0.72) 32%, rgba(15, 30, 82, 0.94) 70%);
  box-shadow: 0 0 42px rgba(93, 112, 255, 0.56);
  transform: translate(-50%, -50%);
}

.lab-art-planet::after {
  content: '';
  position: absolute;
  left: 50%;
  top: 50%;
  width: 8rem;
  height: 2.35rem;
  border: 1px solid rgba(125, 151, 255, 0.74);
  border-radius: 50%;
  transform: translate(-50%, -50%) rotate(-14deg);
}

.lab-stats-bar {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(22rem, 31rem);
  align-items: center;
  gap: 2rem;
  margin-top: 1.25rem;
  padding: 1.1rem 1.45rem;
  border: 1px solid rgba(151, 179, 255, 0.15);
  border-radius: 1rem;
  background: rgba(4, 14, 36, 0.68);
}

.lab-stats {
  display: grid;
  grid-template-columns: repeat(5, minmax(0, 1fr));
  gap: 1rem;
  margin: 0;
}

.lab-stat {
  display: grid;
  grid-template-columns: auto 1fr;
  grid-template-rows: auto auto;
  align-items: center;
  column-gap: 0.8rem;
  margin: 0;
}

.lab-stat span {
  grid-row: 1 / 3;
  width: 2.6rem;
  height: 2.6rem;
  display: grid;
  place-items: center;
  border: 1px solid rgba(151, 179, 255, 0.1);
  border-radius: 50%;
  background: rgba(82, 107, 186, 0.12);
}

.lab-stat span::before {
  content: '';
  width: 1rem;
  height: 1rem;
  border: 1.5px solid rgba(165, 186, 255, 0.82);
}

.stat-icon-flask::before {
  border-top: 0 !important;
  border-radius: 0 0 0.22rem 0.22rem;
}

.stat-icon-cube::before {
  clip-path: polygon(50% 0, 95% 24%, 95% 74%, 50% 100%, 5% 74%, 5% 24%);
  background: transparent;
}

.stat-icon-code::before {
  content: '</>' !important;
  width: auto !important;
  height: auto !important;
  border: 0 !important;
  color: rgba(165, 186, 255, 0.82);
  font-weight: 760;
}

.stat-icon-star::before {
  clip-path: polygon(50% 0, 62% 35%, 100% 35%, 69% 56%, 81% 92%, 50% 70%, 19% 92%, 31% 56%, 0 35%, 38% 35%);
}

.stat-icon-users::before {
  border-radius: 50%;
  box-shadow: -0.55rem 0.58rem 0 -0.2rem rgba(165, 186, 255, 0.7), 0.55rem 0.58rem 0 -0.2rem rgba(165, 186, 255, 0.7);
}

.lab-stat dt {
  color: #86a0ff;
  font-size: 1.5rem;
  font-weight: 780;
  line-height: 1;
}

.lab-stat dd {
  margin: 0.32rem 0 0;
  color: var(--home-text-soft);
  font-size: 0.82rem;
}

.lab-stats-bar blockquote {
  position: relative;
  margin: 0;
  padding-left: 2rem;
  border-left: 1px solid rgba(151, 179, 255, 0.1);
}

.lab-stats-bar blockquote > span {
  position: absolute;
  left: 2rem;
  top: -0.25rem;
  color: rgba(174, 194, 255, 0.5);
  font-family: Georgia, serif;
  font-size: 2.4rem;
  line-height: 1;
}

.lab-stats-bar p {
  margin: 0 0 0 2.4rem;
  color: rgba(211, 224, 255, 0.72);
  font-size: 1rem;
  line-height: 1.65;
}

.lab-stats-bar cite {
  display: block;
  margin-top: -0.2rem;
  color: rgba(225, 234, 255, 0.72);
  text-align: right;
  font-family: "Segoe Script", "Brush Script MT", cursive;
  font-size: 2rem;
  font-style: normal;
}

@media (max-width: 1380px) {
  .products-row {
    grid-template-columns: repeat(3, minmax(0, 1fr));
  }

  .lab-card-grid {
    grid-template-columns: repeat(3, minmax(0, 1fr));
  }

  .lab-feature-card {
    grid-column: 1 / -1;
  }

  .lab-stats-bar {
    grid-template-columns: 1fr;
  }

  .lab-stats-bar blockquote {
    border-left: 0;
    padding-left: 0;
  }
}

@media (max-width: 900px) {
  .home-products-showcase {
    padding: 0.65rem;
  }

  .home-products-frame {
    padding: 6.2rem 1rem 1rem;
  }

  .products-section-head,
  .lab-panel-head {
    flex-direction: column;
  }

  .products-row,
  .lab-card-grid,
  .lab-stats {
    grid-template-columns: 1fr;
  }

  .lab-feature-card {
    grid-template-columns: 1fr;
  }

  .lab-feature-media {
    min-height: 16rem;
    border-right: 0;
    border-bottom: 1px solid rgba(151, 179, 255, 0.12);
  }
}

.product-show-card--skeleton {
  min-height: 14rem;
  background: rgba(255, 255, 255, 0.04);
  border-radius: var(--radius-lg);
  animation: pulse 1.5s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}
</style>
