<template>
  <div class="min-h-screen">
    <DodgeGame />
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

useHead({
  title: '躲避方块游戏 - 溪午听风',
  meta: [
    { name: 'description', content: '一个有趣的躲避方块小游戏，测试你的反应能力！' }
  ]
})
</script>

