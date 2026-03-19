<template>
  <article class="english-module-card english-module-card--streak">
    <div class="english-module-top">
      <span class="english-module-badge english-module-badge--amber">Study Streak</span>
      <div class="english-streak-fire">
        <span class="english-streak-fire-icon">🔥</span>
        <strong class="english-streak-fire-value">12</strong>
        <span class="english-streak-fire-text">days</span>
      </div>
    </div>

    <div class="english-streak-chart">
      <div
        v-for="(day, index) in weekDays"
        :key="`${day.name}-${index}`"
        class="english-streak-day"
      >
        <div class="english-streak-column-shell">
          <div
            class="english-streak-column"
            :class="[
              day.completed ? 'english-streak-column--done' : 'english-streak-column--idle',
              isToday(index) ? 'english-streak-column--today' : ''
            ]"
            :style="{ height: day.completed ? '100%' : '16%' }"
          >
            <span v-if="day.completed" class="english-streak-tooltip">
              {{ day.minutes }} mins
            </span>
          </div>
        </div>
        <span
          class="english-streak-day-label"
          :class="isToday(index) ? 'english-streak-day-label--today' : ''"
        >
          {{ day.name }}
        </span>
      </div>
    </div>

    <div class="english-streak-summary">
      <div class="english-streak-summary-row">
        <span class="english-streak-summary-label">Total learned</span>
        <strong class="english-streak-summary-value">452 words</strong>
      </div>
      <div class="english-streak-progress">
        <div class="english-streak-progress-fill" :style="{ width: '65%' }"></div>
      </div>
    </div>
  </article>
</template>

<script setup lang="ts">
const weekDays = [
  { name: 'M', completed: true, minutes: 45 },
  { name: 'T', completed: true, minutes: 30 },
  { name: 'W', completed: true, minutes: 60 },
  { name: 'T', completed: false, minutes: 0 },
  { name: 'F', completed: true, minutes: 25 },
  { name: 'S', completed: false, minutes: 0 },
  { name: 'S', completed: false, minutes: 0 }
]

const isToday = (index: number) => index === 4
</script>
