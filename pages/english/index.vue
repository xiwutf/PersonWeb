<template>
  <div class="min-h-screen py-12 px-4 sm:px-6 lg:px-8 bg-mesh">
    <div class="max-w-7xl mx-auto">
      <!-- Header Section -->
      <div class="text-center mb-16 animate-float">
        <h1 class="text-5xl md:text-7xl font-bold mb-6 font-['Outfit']">
          <span class="text-gradient-primary">English Learning</span>
        </h1>
        <p class="text-xl text-gray-600 max-w-2xl mx-auto">
          Expand your vocabulary, improve your reading, and master the language.
        </p>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
        
        <!-- Daily Word Card -->
        <div class="h-full">
          <DailyWord />
        </div>

        <!-- Streak Tracker -->
        <div class="h-full">
          <StreakTracker />
        </div>

        <!-- Quick Quiz -->
        <div class="h-full">
          <QuickQuiz />
        </div>

      </div>

      <!-- Featured Section -->
      <div class="mt-20">
        <div class="glass p-8 md:p-12 rounded-3xl relative overflow-hidden">
          <div class="absolute top-0 right-0 -mt-20 -mr-20 w-64 h-64 bg-primary-500 rounded-full opacity-10 blur-3xl"></div>
          <div class="absolute bottom-0 left-0 -mb-20 -ml-20 w-64 h-64 bg-primary-500 rounded-full opacity-10 blur-3xl"></div>
          
          <div class="relative z-10 flex flex-col md:flex-row items-center justify-between gap-12">
            <div class="md:w-1/2">
              <h2 class="text-3xl md:text-4xl font-bold mb-6 font-['Outfit']">Why Learn English?</h2>
              <p class="text-gray-600 text-lg leading-relaxed mb-8">
                English is the global language of business, science, and technology. Mastering it opens doors to new opportunities, cultures, and knowledge. Start your journey today with our curated resources.
              </p>
              <button class="btn-primary">
                Get Started Now
              </button>
            </div>
            <div class="md:w-1/2 flex justify-center">
               <div class="relative w-full max-w-md aspect-video bg-primary-100 rounded-2xl flex items-center justify-center overflow-hidden shadow-lg">
                  <span class="text-6xl">??</span>
               </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// 使用默认布局（包含顶部导航栏）
definePageMeta({
  layout: 'default'
})

import DailyWord from '~/components/english/DailyWord.vue'
import StreakTracker from '~/components/english/StreakTracker.vue'
import QuickQuiz from '~/components/english/QuickQuiz.vue'

useHead({
  title: 'English Learning - 溪午听风',
  meta: [
    { name: 'description', content: 'English learning resources, daily words, and reading materials.' }
  ]
})
</script>

