<template>
  <div class="min-h-screen">
    <AICharacter
      :introduction-text="introText"
      :enable-chat="true"
    />
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

const introText = [
  '你好！我是溪午听风',
  '我是一名全栈开发者',
  '专注于 Vue.js、Nuxt.js 和 Node.js',
  '也喜欢探索 AI 和机器学习',
  '欢迎来到我的个人网站！',
  '这里展示我的项目、文章和技术分享',
  '希望你能在这里找到有趣的内容！'
]

useHead({
  title: 'AI 自我介绍 - 溪午听风',
  meta: [
    { name: 'description', content: 'AI 驱动的自我介绍动画，了解我的技术背景和兴趣' }
  ]
})
</script>

