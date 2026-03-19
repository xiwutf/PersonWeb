<template>
  <div class="english-page">
    <div class="english-page-noise"></div>
    <div class="english-page-background">
      <div class="english-page-blob english-page-blob--blue"></div>
      <div class="english-page-blob english-page-blob--amber"></div>
      <div class="english-page-grid"></div>
    </div>

    <div class="english-shell">
      <section class="english-hero">
        <div class="english-hero-copy">
          <span class="english-hero-kicker">English Lab</span>
          <h1 class="english-title">英语学习实验室</h1>
          <p class="english-subtitle">
            把词汇、练习和学习节奏放进同一块面板里，让每天的英语输入更轻、更稳，也更容易坚持。
          </p>
          <div class="english-hero-actions">
            <button class="english-button english-button--primary">
              开始今日练习
            </button>
            <button class="english-button english-button--secondary">
              查看学习路径
            </button>
          </div>
        </div>

        <div class="english-hero-panel">
          <div class="english-hero-stat">
            <span class="english-hero-stat-label">Daily Focus</span>
            <strong class="english-hero-stat-value">20 min</strong>
            <span class="english-hero-stat-note">适合碎片时间启动</span>
          </div>
          <div class="english-hero-stat">
            <span class="english-hero-stat-label">Learning Loop</span>
            <strong class="english-hero-stat-value">词汇 + 测验 + 复盘</strong>
            <span class="english-hero-stat-note">比单独背单词更有推进感</span>
          </div>
          <div class="english-hero-highlight">
            <span class="english-hero-highlight-dot"></span>
            今天先记住一个词，再做一道题，再把连续性守住。
          </div>
        </div>
      </section>

      <section class="english-card-grid">
        <div class="english-card-shell">
          <DailyWord />
        </div>
        <div class="english-card-shell">
          <StreakTracker />
        </div>
        <div class="english-card-shell">
          <QuickQuiz />
        </div>
      </section>

      <section class="english-focus-card">
        <div class="english-focus-copy">
          <span class="english-focus-kicker">Why This Page</span>
          <h2 class="english-focus-title">把英语学习从“偶尔打开”变成“每天推进一点”</h2>
          <p class="english-focus-text">
            这页不只是放一个单词卡片，而是把最常用的三个动作放在一起：
            先接触新词，再做快速判断，最后用连续记录把学习留在日常节奏里。
          </p>
        </div>

        <div class="english-focus-points">
          <div class="english-focus-point">
            <span class="english-focus-point-index">01</span>
            <div>
              <h3 class="english-focus-point-title">低门槛开始</h3>
              <p class="english-focus-point-text">先学一个词，不需要先准备完整课程。</p>
            </div>
          </div>
          <div class="english-focus-point">
            <span class="english-focus-point-index">02</span>
            <div>
              <h3 class="english-focus-point-title">即时反馈</h3>
              <p class="english-focus-point-text">做题后马上知道对错，学习感受更明确。</p>
            </div>
          </div>
          <div class="english-focus-point">
            <span class="english-focus-point-index">03</span>
            <div>
              <h3 class="english-focus-point-title">持续积累</h3>
              <p class="english-focus-point-text">用 streak 和总词量，让成长不是只靠感觉。</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import '~/assets/css/english.css'

import DailyWord from '~/components/english/DailyWord.vue'
import QuickQuiz from '~/components/english/QuickQuiz.vue'
import StreakTracker from '~/components/english/StreakTracker.vue'

definePageMeta({
  layout: 'default'
})

usePageStyle('english')

useHead({
  title: 'English Learning - 溪午听风',
  meta: [
    { name: 'description', content: 'English learning resources, daily words, quizzes, and habit-building tools.' }
  ]
})
</script>
