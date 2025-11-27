<template>
  <div class="glass-card p-6 h-full flex flex-col relative overflow-hidden group">
    <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
      <span class="text-8xl">📅</span>
    </div>
    
    <div class="relative z-10 flex-1 flex flex-col">
      <div class="flex justify-between items-start mb-4">
        <span class="px-3 py-1 bg-primary-100 text-primary-700 rounded-full text-xs font-bold uppercase tracking-wider">
          Daily Word
        </span>
        <button class="text-gray-400 hover:text-primary-500 transition-colors">
          <i class="fas fa-volume-up"></i>
        </button>
      </div>

      <h3 class="text-4xl font-bold text-gray-800 mb-2 font-['Outfit']">Serendipity</h3>
      <div class="flex items-center space-x-2 text-gray-500 mb-4 font-mono text-sm">
        <span>/ˌser.ənˈdɪp.ə.ti/</span>
        <span class="w-1 h-1 bg-gray-300 rounded-full"></span>
        <span class="italic">noun</span>
      </div>

      <p class="text-gray-600 mb-6 leading-relaxed">
        The occurrence and development of events by chance in a happy or beneficial way.
      </p>

      <div class="bg-primary-50/50 rounded-xl p-4 mb-6 border border-primary-100">
        <p class="text-primary-800 italic text-sm">
          "Finding this book was a pure stroke of <span class="font-bold">serendipity</span>."
        </p>
      </div>

      <div class="mt-auto">
        <button 
          @click="markLearned"
          class="w-full py-2.5 rounded-xl font-medium transition-all duration-300 flex items-center justify-center space-x-2"
          :class="isLearned ? 'bg-green-100 text-green-700' : 'bg-primary-600 text-white hover:bg-primary-700 shadow-lg shadow-primary-500/30'"
        >
          <span v-if="isLearned" class="flex items-center">
            <i class="fas fa-check mr-2"></i> Learned
          </span>
          <span v-else>Mark as Learned</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const isLearned = ref(false)

const markLearned = () => {
  isLearned.value = !isLearned.value
}
</script>
