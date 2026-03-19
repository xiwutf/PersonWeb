<template>
  <article class="english-module-card english-module-card--word">
    <div class="english-module-icon">Aa</div>

    <div class="english-module-top">
      <span class="english-module-badge">Daily Word</span>
      <button class="english-module-audio" type="button" aria-label="播放发音">
        <i class="fas fa-volume-up"></i>
      </button>
    </div>

    <div class="english-word-header">
      <h3 class="english-word-title">Serendipity</h3>
      <div class="english-word-meta">
        <span>/ˌser.ənˈdɪp.ə.ti/</span>
        <span class="english-word-meta-dot"></span>
        <span class="english-word-type">noun</span>
      </div>
    </div>

    <p class="english-word-definition">
      The occurrence and development of events by chance in a happy or beneficial way.
    </p>

    <div class="english-word-example">
      <span class="english-word-example-label">Example</span>
      <p class="english-word-example-text">
        "Finding this book was a pure stroke of <strong>serendipity</strong>."
      </p>
    </div>

    <button
      @click="markLearned"
      class="english-module-action"
      :class="isLearned ? 'english-module-action--success' : 'english-module-action--primary'"
      type="button"
    >
      <span v-if="isLearned">
        <i class="fas fa-check"></i>
        已掌握
      </span>
      <span v-else>标记为已学习</span>
    </button>
  </article>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const isLearned = ref(false)

const markLearned = () => {
  isLearned.value = !isLearned.value
}
</script>
