<template>
  <section id="building" class="building-showcase">
    <div class="building-frame">
      <div class="building-stars" aria-hidden="true"></div>

      <div class="building-head reveal-up">
        <div class="building-title">
          <span>NOW BUILDING</span>
          <h2>正在构建的<strong>项目</strong></h2>
          <p>专注、迭代、交付，把想法变成可用的产品与系统。</p>
        </div>

        <blockquote class="building-quote">
          <span aria-hidden="true">“</span>
          <p>真正的成长，来自于持续构建<br>和在真实反馈中不断迭代。</p>
          <cite>Xiuu</cite>
        </blockquote>
      </div>

      <div class="building-content reveal-up reveal-delay-1">
        <div class="project-grid">
          <!-- loading 骨架 -->
          <template v-if="loading">
            <article v-for="n in 4" :key="n" class="project-card project-card--skeleton"></article>
          </template>
          <!-- 真实数据 -->
          <template v-else-if="projects.length > 0">
            <article v-for="(project, index) in projects" :key="project.id" class="project-card">
              <span class="project-status" :class="getStatusTheme(project.status)">
                <i aria-hidden="true"></i>
                {{ getStatusText(project.status) }}
              </span>
              <span class="project-art" :class="getArtClass(index)" aria-hidden="true"></span>
              <div class="project-progress-head">
                <h3>{{ project.title }}</h3>
                <strong>{{ project.progress }}%</strong>
              </div>
              <span class="project-progress-label">开发进度</span>
              <span class="project-progress">
                <i :style="{ width: `${project.progress}%` }"></i>
              </span>
              <p>{{ project.description }}</p>
              <div class="project-tags">
                <span v-for="tag in project.techStack.slice(0, 3)" :key="tag">{{ tag }}</span>
              </div>
            </article>
          </template>
          <!-- 空状态：骨架占位 -->
          <template v-else>
            <article v-for="n in 4" :key="n" class="project-card project-card--skeleton"></article>
          </template>
        </div>

        <aside class="building-side">
          <section class="today-panel">
            <div class="panel-head">
              <h3>今日进展</h3>
              <span>2025-05-18 <i aria-hidden="true"></i></span>
            </div>
            <ol>
              <li v-for="item in today" :key="item.text">
                <span aria-hidden="true"></span>
                <p>{{ item.text }}</p>
                <time>{{ item.time }}</time>
              </li>
            </ol>
          </section>

          <section class="week-panel">
            <div class="panel-head">
              <h3>本周数据</h3>
              <span>05.12 - 05.18 <i aria-hidden="true"></i></span>
            </div>
            <div class="week-grid">
              <article v-for="stat in weekStats" :key="stat.label">
                <span :class="stat.icon" aria-hidden="true"></span>
                <p>{{ stat.label }}</p>
                <strong>{{ stat.value }}</strong>
              </article>
            </div>
          </section>
        </aside>
      </div>

      <div class="building-focus reveal-up reveal-delay-2">
        <div class="orbit-panel" aria-hidden="true">
          <span class="orbit-logo">
            <span></span>
            <span></span>
            <span></span>
          </span>
        </div>

        <div class="goal-panel">
          <h3><span class="icon-target" aria-hidden="true"></span>当前目标</h3>
          <ul>
            <li v-for="goal in goals" :key="goal">{{ goal }}</li>
          </ul>
        </div>

        <div class="keyword-panel">
          <h3><span class="icon-keyword" aria-hidden="true"></span>本月关键词</h3>
          <div class="keyword-list">
            <span v-for="keyword in keywords" :key="keyword">{{ keyword }}</span>
          </div>
          <div class="month-progress">
            <div>
              <span>月度进度</span>
              <strong>72%</strong>
            </div>
            <span><i></i></span>
          </div>
        </div>
      </div>

      <div class="building-bottom reveal-up reveal-delay-3">
        <span class="bottom-orbit" aria-hidden="true"></span>
        <strong>每一天，都是未来的起点。</strong>
        <p>保持好奇，持续构建，让想法落地，让价值发生。</p>
        <NuxtLink to="/projects">
          查看所有项目动态
          <span aria-hidden="true">→</span>
        </NuxtLink>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import type { HomeNowBuildingItem } from '~/types/home'

const props = withDefaults(defineProps<{
  projects: HomeNowBuildingItem[]
  loading: boolean
}>(), {
  projects: () => [],
  loading: false,
})

const getStatusText = (status: string): string => {
  const s = status.toLowerCase()
  if (s.includes('active')) return '进行中'
  if (s.includes('complete')) return '已上线'
  if (s.includes('plan')) return '规划中'
  return '进行中'
}

const getStatusTheme = (status: string): string => {
  const s = status.toLowerCase()
  if (s.includes('active')) return 'status-blue'
  if (s.includes('complete')) return 'status-teal'
  return 'status-indigo'
}

const PROJECT_ARTS = ['art-match', 'art-workflow', 'art-pet', 'art-knowledge']
const getArtClass = (index: number) => PROJECT_ARTS[index % PROJECT_ARTS.length]

const today = [
  { text: 'AI Hub 新增资源聚合模块', time: '10:30' },
  { text: '联谊平台数据库设计优化', time: '14:20' },
  { text: 'AI Workflow 系统流程编辑器开发', time: '16:45' },
  { text: '阅读：《深度工作》', time: '21:10' }
]

const weekStats = [
  { label: '提交代码', value: '23 次', icon: 'week-code' },
  { label: '项目迭代', value: '7 次', icon: 'week-cube' },
  { label: '阅读时间', value: '12.6 h', icon: 'week-book' },
  { label: '学习笔记', value: '18 篇', icon: 'week-note' }
]

const goals = [
  '完成心动联谊平台 MVP 上线',
  'AI Workflow 系统实现核心编排能力',
  '构建个人内容与知识体系',
  '持续学习与输出，提升认知与影响力'
]

const keywords = ['专注', '迭代', '交付', '学习', '影响力']
</script>

<style scoped>
.building-showcase {
  padding: 1.7rem 2.4rem 2.1rem;
  background:
    radial-gradient(circle at 36% 12%, rgba(54, 88, 171, 0.13), transparent 31rem),
    radial-gradient(circle at 72% 30%, rgba(83, 62, 175, 0.1), transparent 30rem),
    #020713;
}

.building-frame {
  position: relative;
  width: min(100%, 1840px);
  min-height: calc(100vh - 3.8rem);
  min-height: calc(100svh - 3.8rem);
  margin: 0 auto;
  padding: 8.35rem min(4.6vw, 5.8rem) 2.05rem;
  overflow: hidden;
  border: 1px solid var(--home-border);
  border-radius: 1.85rem;
  background:
    radial-gradient(circle at 43% 18%, rgba(44, 82, 174, 0.15), transparent 32rem),
    radial-gradient(circle at 67% 42%, rgba(65, 49, 162, 0.09), transparent 36rem),
    linear-gradient(180deg, rgba(4, 13, 34, 0.98), rgba(2, 8, 24, 0.99));
  box-shadow:
    0 28px 100px rgba(0, 0, 0, 0.34),
    inset 0 1px 0 rgba(255, 255, 255, 0.05);
}

.building-stars {
  position: absolute;
  inset: 6.9rem 3rem auto 3rem;
  height: 35rem;
  pointer-events: none;
  background-image:
    radial-gradient(circle, rgba(129, 158, 255, 0.62) 0 1px, transparent 1.3px),
    radial-gradient(circle, rgba(149, 106, 255, 0.42) 0 1px, transparent 1.3px);
  background-position: 0 0, 4.8rem 2.8rem;
  background-size: 10rem 6.2rem, 12rem 7.4rem;
  opacity: 0.5;
}

.building-head,
.building-content,
.building-focus,
.building-bottom {
  position: relative;
  z-index: 1;
}

.building-head {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(25rem, 34rem);
  gap: 2.5rem;
  align-items: start;
}

.building-title > span {
  display: inline-flex;
  min-height: 1.7rem;
  align-items: center;
  padding: 0 0.72rem;
  border: 1px solid rgba(128, 151, 255, 0.22);
  border-radius: 999px;
  color: #afa4ff;
  background: rgba(83, 72, 184, 0.13);
  font-size: 0.78rem;
  font-weight: 760;
  letter-spacing: 0.08em;
}

.building-title h2 {
  margin: 0.75rem 0 0;
  color: rgba(250, 252, 255, 0.98);
  font-size: clamp(2.7rem, 4vw, 4.2rem);
  line-height: 1.08;
  font-weight: 860;
}

.building-title h2 strong {
  color: #8f72ff;
  text-shadow: 0 0 25px rgba(118, 92, 255, 0.42);
}

.building-title p {
  margin: 1.1rem 0 0;
  color: rgba(216, 226, 255, 0.74);
  font-size: 1.04rem;
}

.building-quote {
  position: relative;
  min-height: 8.3rem;
  margin: 0;
  padding: 1.62rem 2.25rem 1.2rem 3.35rem;
  border: 1px solid rgba(151, 179, 255, 0.13);
  border-radius: 1.25rem;
  background: linear-gradient(180deg, rgba(7, 18, 43, 0.54), rgba(5, 13, 34, 0.28));
}

.building-quote > span {
  position: absolute;
  left: 1.18rem;
  top: 0.96rem;
  color: rgba(142, 157, 230, 0.68);
  font-family: Georgia, serif;
  font-size: 3.65rem;
  line-height: 1;
}

.building-quote p {
  margin: 0;
  color: rgba(235, 241, 255, 0.86);
  font-size: 1.05rem;
  line-height: 1.85;
}

.building-quote cite {
  position: absolute;
  right: 4.2rem;
  bottom: -1rem;
  color: #a277ff;
  font-family: "Brush Script MT", "Segoe Script", cursive;
  font-size: 2.5rem;
  font-style: italic;
  font-weight: 300;
}

.building-content {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(23rem, 0.32fr);
  gap: 1.2rem;
  margin-top: 2.1rem;
}

.project-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 1.15rem;
}

.project-card,
.today-panel,
.week-panel,
.building-focus,
.building-bottom {
  border: 1px solid rgba(151, 179, 255, 0.17);
  border-radius: 1rem;
  background:
    linear-gradient(180deg, rgba(11, 25, 56, 0.74), rgba(5, 14, 34, 0.64)),
    rgba(6, 16, 39, 0.72);
  box-shadow:
    inset 0 1px 0 rgba(255, 255, 255, 0.05),
    0 18px 42px rgba(0, 0, 0, 0.2);
}

.project-card {
  min-height: 28.1rem;
  display: grid;
  grid-template-rows: auto 9rem auto auto auto 1fr auto;
  padding: 1.25rem 1.25rem 1.35rem;
}

.project-status {
  justify-self: start;
  min-height: 1.8rem;
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  padding: 0 0.72rem;
  border-radius: 999px;
  color: #d1c7ff;
  background: rgba(122, 72, 255, 0.14);
  font-size: 0.82rem;
  font-weight: 700;
}

.project-status i {
  width: 0.46rem;
  height: 0.46rem;
  border-radius: 50%;
  background: #b65cff;
  box-shadow: 0 0 12px rgba(182, 92, 255, 0.7);
}

.status-blue {
  color: #a8c9ff;
  background: rgba(63, 130, 255, 0.14);
}

.status-blue i {
  background: #5597ff;
}

.status-indigo {
  color: #b7b0ff;
  background: rgba(84, 73, 221, 0.16);
}

.status-indigo i {
  background: #645dff;
}

.status-teal {
  color: #a8eee3;
  background: rgba(65, 195, 184, 0.13);
}

.status-teal i {
  background: #58d5cc;
}

.project-art {
  position: relative;
  display: block;
  height: 9rem;
  margin: 0.6rem 0 0.65rem;
}

.project-progress-head {
  display: flex;
  align-items: end;
  justify-content: space-between;
  gap: 1rem;
}

.project-progress-head h3 {
  margin: 0;
  color: rgba(250, 252, 255, 0.98);
  font-size: 1.42rem;
  line-height: 1.18;
  font-weight: 820;
}

.project-progress-head strong {
  color: rgba(250, 252, 255, 0.92);
  font-size: 0.92rem;
}

.project-progress-label {
  margin-top: 1rem;
  color: rgba(202, 216, 255, 0.68);
  font-size: 0.82rem;
}

.project-progress {
  height: 0.44rem;
  margin-top: 0.36rem;
  overflow: hidden;
  border-radius: 999px;
  background: rgba(77, 102, 158, 0.26);
}

.project-progress i {
  display: block;
  height: 100%;
  border-radius: inherit;
  background: linear-gradient(90deg, #5a88ff, #ad65ff);
  box-shadow: 0 0 18px rgba(124, 94, 255, 0.54);
}

.project-card p {
  margin: 1.15rem 0 0;
  color: rgba(207, 219, 255, 0.68);
  font-size: 0.92rem;
  line-height: 1.78;
}

.project-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.48rem;
  margin-top: 1.1rem;
}

.project-tags span {
  padding: 0.42rem 0.64rem;
  border: 1px solid rgba(142, 168, 255, 0.18);
  border-radius: 999px;
  color: rgba(213, 224, 255, 0.76);
  background: rgba(255, 255, 255, 0.025);
  font-size: 0.78rem;
}

.building-side {
  display: grid;
  gap: 1.25rem;
}

.today-panel,
.week-panel {
  padding: 1.55rem 1.45rem;
}

.panel-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1.25rem;
}

.panel-head h3 {
  margin: 0;
  color: rgba(250, 252, 255, 0.96);
  font-size: 1.1rem;
  font-weight: 820;
}

.panel-head span {
  color: rgba(196, 209, 246, 0.62);
  font-size: 0.86rem;
}

.today-panel ol {
  display: grid;
  gap: 1.22rem;
  margin: 0;
  padding: 0;
  list-style: none;
}

.today-panel li {
  display: grid;
  grid-template-columns: 1.35rem minmax(0, 1fr) auto;
  align-items: center;
  gap: 0.7rem;
}

.today-panel li > span {
  width: 1.1rem;
  height: 1.1rem;
  border: 1px solid #a68cff;
  border-radius: 50%;
  box-shadow: 0 0 12px rgba(166, 140, 255, 0.28);
}

.today-panel li > span::before {
  content: '';
  display: block;
  width: 0.42rem;
  height: 0.22rem;
  margin: 0.34rem 0 0 0.31rem;
  border-left: 1px solid #d8d0ff;
  border-bottom: 1px solid #d8d0ff;
  transform: rotate(-45deg);
}

.today-panel p {
  margin: 0;
  color: rgba(226, 234, 255, 0.82);
  font-size: 0.9rem;
}

.today-panel time {
  color: rgba(194, 206, 242, 0.62);
  font-size: 0.86rem;
}

.week-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.55rem;
}

.week-grid article {
  min-height: 4.65rem;
  display: grid;
  grid-template-columns: 2.8rem 1fr;
  grid-template-rows: auto auto;
  align-items: center;
  gap: 0 0.7rem;
  padding: 0.8rem;
  border: 1px solid rgba(142, 168, 255, 0.12);
  border-radius: 0.6rem;
  background: rgba(48, 78, 162, 0.08);
}

.week-grid article > span {
  grid-row: 1 / 3;
  position: relative;
  width: 2.45rem;
  height: 2.45rem;
  border-radius: 0.52rem;
  background: rgba(65, 101, 255, 0.16);
}

.week-grid p {
  margin: 0;
  color: rgba(199, 213, 249, 0.62);
  font-size: 0.78rem;
}

.week-grid strong {
  color: rgba(250, 252, 255, 0.96);
  font-size: 1.25rem;
  line-height: 1;
}

.building-focus {
  min-height: 13.2rem;
  display: grid;
  grid-template-columns: 28rem minmax(24rem, 0.9fr) minmax(34rem, 1.35fr);
  gap: 2.5rem;
  align-items: center;
  margin-top: 1.55rem;
  padding: 1.35rem 2.1rem;
}

.orbit-panel {
  position: relative;
  min-height: 10.5rem;
  overflow: hidden;
}

.orbit-panel::before,
.orbit-panel::after {
  content: '';
  position: absolute;
  left: 50%;
  top: 50%;
  border: 1px solid rgba(94, 126, 255, 0.44);
  border-radius: 50%;
  transform: translate(-50%, -50%);
  box-shadow: 0 0 28px rgba(84, 104, 255, 0.18);
}

.orbit-panel::before {
  width: 18rem;
  height: 8.1rem;
}

.orbit-panel::after {
  width: 13rem;
  height: 6rem;
}

.orbit-logo {
  position: absolute;
  left: 50%;
  top: 50%;
  width: 4.3rem;
  height: 4.3rem;
  display: block;
  border-radius: 50%;
  background:
    radial-gradient(circle at center, rgba(47, 87, 196, 0.9), rgba(11, 28, 82, 0.92));
  transform: translate(-50%, -50%);
  box-shadow:
    0 0 50px rgba(76, 112, 255, 0.42),
    inset 0 1px 0 rgba(255, 255, 255, 0.16);
}

.orbit-logo span {
  position: absolute;
  left: 1.15rem;
  height: 0.52rem;
  background: linear-gradient(90deg, #637cff, #7da6ff);
}

.orbit-logo span:nth-child(1) {
  top: 1.08rem;
  width: 1.95rem;
}

.orbit-logo span:nth-child(2) {
  top: 1.78rem;
  width: 1.45rem;
}

.orbit-logo span:nth-child(3) {
  top: 2.48rem;
  width: 0.92rem;
}

.goal-panel,
.keyword-panel {
  min-height: 9rem;
}

.goal-panel {
  border-right: 1px solid rgba(142, 168, 255, 0.12);
}

.goal-panel h3,
.keyword-panel h3 {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  margin: 0 0 1.2rem;
  color: rgba(250, 252, 255, 0.96);
  font-size: 1.12rem;
  font-weight: 820;
}

.goal-panel ul {
  display: grid;
  gap: 0.62rem;
  margin: 0;
  padding: 0;
  list-style: none;
}

.goal-panel li {
  position: relative;
  padding-left: 1rem;
  color: rgba(207, 219, 255, 0.72);
  font-size: 0.93rem;
}

.goal-panel li::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0.45rem;
  width: 0.32rem;
  height: 0.32rem;
  border-radius: 50%;
  background: #758fff;
  box-shadow: 0 0 10px rgba(117, 143, 255, 0.72);
}

.keyword-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.keyword-list span {
  min-width: 4.7rem;
  min-height: 2.3rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0 1rem;
  border: 1px solid rgba(142, 168, 255, 0.13);
  border-radius: 999px;
  color: rgba(225, 234, 255, 0.82);
  background: rgba(255, 255, 255, 0.035);
  font-weight: 700;
}

.month-progress {
  margin-top: 1.25rem;
}

.month-progress div {
  display: flex;
  align-items: center;
  justify-content: space-between;
  color: rgba(232, 238, 255, 0.94);
  font-size: 0.98rem;
}

.month-progress > span {
  height: 0.55rem;
  display: block;
  margin-top: 0.72rem;
  overflow: hidden;
  border-radius: 999px;
  background: rgba(57, 79, 132, 0.32);
}

.month-progress i {
  width: 72%;
  height: 100%;
  display: block;
  border-radius: inherit;
  background: linear-gradient(90deg, #5c89ff, #b164ff);
  box-shadow: 0 0 20px rgba(128, 97, 255, 0.5);
}

.building-bottom {
  min-height: 5.25rem;
  display: grid;
  grid-template-columns: 4.6rem auto minmax(0, 1fr) auto;
  align-items: center;
  gap: 1.5rem;
  margin-top: 1.45rem;
  padding: 0.8rem 1.35rem;
}

.bottom-orbit {
  position: relative;
  width: 3.35rem;
  height: 3.35rem;
  border-radius: 50%;
  background: radial-gradient(circle at 36% 32%, #718aff, #1d2b7d 64%, #07113a);
  box-shadow: 0 0 24px rgba(84, 112, 255, 0.36);
}

.bottom-orbit::before {
  content: '';
  position: absolute;
  left: -0.52rem;
  top: 1.25rem;
  width: 4.45rem;
  height: 1rem;
  border: 1px solid rgba(92, 126, 255, 0.56);
  border-radius: 50%;
  transform: rotate(-16deg);
}

.building-bottom strong {
  color: rgba(250, 252, 255, 0.96);
  font-size: 1.2rem;
  font-weight: 820;
}

.building-bottom p {
  margin: 0;
  color: rgba(200, 214, 249, 0.66);
  font-size: 0.98rem;
}

.building-bottom a {
  min-height: 3.05rem;
  display: inline-flex;
  align-items: center;
  gap: 0.7rem;
  padding: 0 1.45rem;
  border: 1px solid rgba(142, 168, 255, 0.18);
  border-radius: 999px;
  color: #aab8ff;
  background: rgba(255, 255, 255, 0.028);
  font-weight: 760;
  white-space: nowrap;
}

.art-match::before,
.art-match::after,
.art-workflow::before,
.art-workflow::after,
.art-pet::before,
.art-pet::after,
.art-knowledge::before,
.art-knowledge::after,
.week-grid article > span::before,
.week-grid article > span::after,
.icon-target::before,
.icon-target::after,
.icon-keyword::before,
.icon-keyword::after {
  content: '';
  position: absolute;
}

.art-match::before,
.art-workflow::before {
  left: 13%;
  right: 6%;
  top: 1rem;
  height: 7rem;
  border: 1px solid rgba(109, 135, 255, 0.34);
  border-radius: 0.38rem;
  background:
    linear-gradient(90deg, rgba(87, 110, 255, 0.15) 0 1px, transparent 1px 100%),
    linear-gradient(180deg, rgba(33, 57, 130, 0.55), rgba(7, 16, 47, 0.42));
  transform: perspective(26rem) rotateY(-17deg) rotateX(7deg);
  box-shadow: 0 0 36px rgba(87, 79, 255, 0.32);
}

.art-match::after {
  left: 39%;
  top: 3.05rem;
  width: 4rem;
  height: 4rem;
  border-radius: 0.75rem;
  background:
    radial-gradient(circle at 50% 48%, rgba(255, 255, 255, 0.92), transparent 0.62rem),
    radial-gradient(circle at 50% 60%, rgba(177, 90, 255, 0.92), transparent 2.1rem);
  box-shadow: 0 0 30px rgba(166, 92, 255, 0.58);
}

.art-workflow::after {
  left: 18%;
  top: 3rem;
  width: 68%;
  height: 0.48rem;
  border-radius: 999px;
  background: rgba(92, 132, 255, 0.58);
  box-shadow:
    0 1.3rem 0 rgba(92, 132, 255, 0.28),
    0 2.6rem 0 rgba(92, 132, 255, 0.38);
}

.art-pet::before {
  left: 50%;
  top: 1.25rem;
  width: 6.6rem;
  height: 6.6rem;
  border-radius: 48% 48% 44% 44%;
  background:
    radial-gradient(circle at 34% 46%, #66e3ff 0 0.4rem, transparent 0.44rem),
    radial-gradient(circle at 66% 46%, #66e3ff 0 0.4rem, transparent 0.44rem),
    radial-gradient(circle at 50% 34%, rgba(92, 143, 255, 0.2), transparent 2.8rem),
    #07162c;
  transform: translateX(-50%);
  box-shadow:
    0 0 28px rgba(70, 152, 255, 0.36),
    inset 0 0 22px rgba(59, 118, 199, 0.36);
}

.art-pet::after {
  left: calc(50% - 3rem);
  top: 0.65rem;
  width: 1.8rem;
  height: 1.8rem;
  border-top: 0.4rem solid #2f7fdc;
  border-left: 0.4rem solid #2f7fdc;
  transform: rotate(18deg);
  filter: drop-shadow(4.4rem -0.2rem 0 #2f7fdc);
}

.art-knowledge::before {
  left: 50%;
  top: 1.5rem;
  width: 4.7rem;
  height: 4.7rem;
  border-radius: 50%;
  background:
    radial-gradient(circle at 42% 35%, rgba(173, 219, 255, 0.88), transparent 0.65rem),
    radial-gradient(circle at center, rgba(73, 122, 255, 0.72), rgba(22, 49, 142, 0.35) 58%, transparent 60%);
  transform: translateX(-50%);
  box-shadow: 0 0 32px rgba(73, 122, 255, 0.54);
}

.art-knowledge::after {
  left: 50%;
  top: 3.4rem;
  width: 8rem;
  height: 2.2rem;
  border: 1px solid rgba(91, 132, 255, 0.52);
  border-radius: 50%;
  transform: translateX(-50%) rotate(-18deg);
}

.week-code::before {
  content: '</>';
  inset: 0.56rem 0 0;
  display: grid;
  place-items: center;
  color: #79a0ff;
  font-size: 1.12rem;
  font-weight: 820;
}

.week-cube::before {
  inset: 0.75rem;
  border: 2px solid #59b6ff;
  transform: rotate(30deg) skew(-8deg, -8deg);
}

.week-book {
  background: rgba(126, 87, 255, 0.18) !important;
}

.week-book::before {
  inset: 0.7rem 0.62rem;
  border: 2px solid #aa83ff;
  border-radius: 0.18rem;
}

.week-note {
  background: rgba(40, 178, 160, 0.16) !important;
}

.week-note::before {
  inset: 0.68rem;
  border: 2px solid #65dbc6;
  border-radius: 0.18rem;
}

.icon-target,
.icon-keyword {
  position: relative;
  width: 1.45rem;
  height: 1.45rem;
  color: #8ea4ff;
}

.icon-target::before {
  inset: 0.2rem;
  border: 2px solid currentColor;
  border-radius: 50%;
}

.icon-target::after {
  left: 0.62rem;
  top: 0;
  width: 0.2rem;
  height: 1.45rem;
  background: currentColor;
  box-shadow: 0.52rem 0.54rem 0 -0.04rem currentColor;
  transform: rotate(45deg);
}

.icon-keyword::before {
  inset: 0.2rem;
  border: 2px solid currentColor;
  border-radius: 50%;
}

.icon-keyword::after {
  content: '✺';
  inset: -0.12rem 0 0;
  display: grid;
  place-items: center;
  color: currentColor;
  font-size: 0.85rem;
}

@media (max-width: 1320px) {
  .building-head,
  .building-content,
  .building-focus {
    grid-template-columns: 1fr;
  }

  .project-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .goal-panel {
    border-right: 0;
  }
}

@media (max-width: 860px) {
  .building-showcase {
    padding: 0.8rem;
  }

  .building-frame {
    padding: 6.2rem 1rem 1rem;
    border-radius: 1.2rem;
  }

  .project-grid,
  .week-grid,
  .building-bottom {
    grid-template-columns: 1fr;
  }
}

.project-card--skeleton {
  background: rgba(255, 255, 255, 0.03) !important;
  box-shadow: none !important;
  animation: building-pulse 1.5s ease-in-out infinite;
}

@keyframes building-pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.4; }
}
</style>
