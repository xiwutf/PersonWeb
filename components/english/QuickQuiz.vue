<template>
  <div class="glass-card p-6 h-full flex flex-col relative overflow-hidden">
    <div class="absolute -right-4 -top-4 w-24 h-24 bg-yellow-400/20 rounded-full blur-2xl"></div>
    
    <div class="flex justify-between items-center mb-6 relative z-10">
      <h3 class="text-lg font-bold text-gray-800 font-['Outfit']">Quick Quiz</h3>
      <span class="text-xs font-medium px-2 py-1 bg-yellow-100 text-yellow-700 rounded-lg">
        Vocabulary
      </span>
    </div>

    <div v-if="!answered" class="flex-1 flex flex-col relative z-10">
      <p class="text-gray-600 mb-4 font-medium">
        What is the synonym of <span class="text-gray-900 font-bold">"Ephemeral"</span>?
      </p>

      <div class="space-y-2 mt-auto">
        <button 
          v-for="(option, index) in options" 
          :key="index"
          @click="selectAnswer(index)"
          class="w-full text-left px-4 py-3 rounded-xl border border-gray-200 hover:border-primary-500 hover:bg-primary-50 transition-all duration-200 text-sm text-gray-600 hover:text-primary-700"
        >
          {{ option }}
        </button>
      </div>
    </div>

    <div v-else class="flex-1 flex flex-col items-center justify-center text-center relative z-10 animate-fade-in">
      <div class="w-16 h-16 rounded-full flex items-center justify-center mb-4" :class="isCorrect ? 'bg-green-100 text-green-600' : 'bg-red-100 text-red-600'">
        <i class="fas text-2xl" :class="isCorrect ? 'fa-check' : 'fa-times'"></i>
      </div>
      <h4 class="text-xl font-bold mb-2" :class="isCorrect ? 'text-green-700' : 'text-red-700'">
        {{ isCorrect ? 'Correct!' : 'Try Again' }}
      </h4>
      <p class="text-sm text-gray-500 mb-6">
        {{ isCorrect ? 'Ephemeral means lasting for a very short time.' : 'Not quite. Keep practicing!' }}
      </p>
      <button 
        @click="nextQuestion"
        class="px-6 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors text-sm font-medium"
      >
        Next Question
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const options = ['Permanent', 'Short-lived', 'Beautiful', 'Dangerous']
const correctIndex = 1
const answered = ref(false)
const isCorrect = ref(false)

const selectAnswer = (index) => {
  answered.value = true
  isCorrect.value = index === correctIndex
}

const nextQuestion = () => {
  answered.value = false
  isCorrect.value = false
  // Logic to load next question would go here
}
</script>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
