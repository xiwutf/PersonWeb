<template>
  <article class="english-module-card english-module-card--quiz">
    <div class="english-module-glow"></div>

    <div class="english-module-top english-module-top--quiz">
      <span class="english-module-badge english-module-badge--gold">Quick Quiz</span>
      <span class="english-quiz-chip">Vocabulary</span>
    </div>

    <div v-if="!answered" class="english-quiz-body">
      <p class="english-quiz-question">
        What is the synonym of <strong>"Ephemeral"</strong>?
      </p>

      <div class="english-quiz-options">
        <button
          v-for="(option, index) in options"
          :key="option"
          @click="selectAnswer(index)"
          class="english-quiz-option"
          type="button"
        >
          {{ option }}
        </button>
      </div>
    </div>

    <div v-else class="english-quiz-result">
      <div
        class="english-quiz-result-icon"
        :class="isCorrect ? 'english-quiz-result-icon--success' : 'english-quiz-result-icon--error'"
      >
        <i class="fas" :class="isCorrect ? 'fa-check' : 'fa-times'"></i>
      </div>
      <h4
        class="english-quiz-result-title"
        :class="isCorrect ? 'english-quiz-result-title--success' : 'english-quiz-result-title--error'"
      >
        {{ isCorrect ? 'Correct!' : 'Try Again' }}
      </h4>
      <p class="english-quiz-result-text">
        {{ isCorrect ? 'Ephemeral means lasting for a very short time.' : 'Not quite. Keep practicing!' }}
      </p>
      <button
        @click="nextQuestion"
        class="english-module-action english-module-action--secondary"
        type="button"
      >
        Next Question
      </button>
    </div>
  </article>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const options = ['Permanent', 'Short-lived', 'Beautiful', 'Dangerous']
const correctIndex = 1
const answered = ref(false)
const isCorrect = ref(false)

const selectAnswer = (index: number) => {
  answered.value = true
  isCorrect.value = index === correctIndex
}

const nextQuestion = () => {
  answered.value = false
  isCorrect.value = false
}
</script>
