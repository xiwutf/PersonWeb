<template>
  <div class="glass-card p-6 h-full flex flex-col">
    <div class="flex justify-between items-center mb-6">
      <h3 class="text-lg font-bold text-gray-800 font-['Outfit']">Study Streak</h3>
      <div class="flex items-center text-orange-500 font-bold">
        <span class="text-2xl mr-1">🔥</span>
        <span class="text-xl">12</span>
        <span class="text-xs text-gray-500 ml-1 font-normal self-end mb-1">days</span>
      </div>
    </div>

    <div class="flex-1 flex flex-col justify-center">
      <div class="flex justify-between items-end h-32 mb-4 px-2">
        <div v-for="(day, index) in weekDays" :key="day.name" class="flex flex-col items-center space-y-2 group">
          <div 
            class="w-8 rounded-lg transition-all duration-500 relative"
            :class="[
              day.completed ? 'bg-gradient-to-t from-primary-600 to-primary-400 shadow-lg shadow-primary-500/30' : 'bg-gray-100',
              isToday(index) ? 'ring-2 ring-primary-300 ring-offset-2' : ''
            ]"
            :style="{ height: day.completed ? '100%' : '15%' }"
          >
            <div v-if="day.completed" class="absolute -top-8 left-1/2 -translate-x-1/2 bg-gray-800 text-white text-xs py-1 px-2 rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap pointer-events-none">
              {{ day.minutes }} mins
            </div>
          </div>
          <span class="text-xs font-medium" :class="isToday(index) ? 'text-primary-600' : 'text-gray-400'">
            {{ day.name }}
          </span>
        </div>
      </div>
    </div>

    <div class="mt-4 pt-4 border-t border-gray-100">
      <div class="flex justify-between text-sm">
        <span class="text-gray-500">Total learned</span>
        <span class="font-bold text-gray-800">452 words</span>
      </div>
      <div class="w-full bg-gray-100 rounded-full h-2 mt-2 overflow-hidden">
        <div class="bg-primary-500 h-full rounded-full" style="width: 65%"></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const weekDays = [
  { name: 'M', completed: true, minutes: 45 },
  { name: 'T', completed: true, minutes: 30 },
  { name: 'W', completed: true, minutes: 60 },
  { name: 'T', completed: false, minutes: 0 },
  { name: 'F', completed: true, minutes: 25 },
  { name: 'S', completed: false, minutes: 0 },
  { name: 'S', completed: false, minutes: 0 },
]

const isToday = (index) => index === 4 // Mocking Friday as today
</script>

