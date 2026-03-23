<template>
  <div class="admin-dashboard" style="padding: 20px; color: white;">
    <h1>测试：后台首页</h1>
    <p>如果你能看到这段文字，说明页面组件已正常加载</p>
    <p>当前时间: {{ currentTime }}</p>
    <button @click="testClick" style="padding: 10px 20px; margin-top: 20px;">
      点击测试按钮
    </button>
    <p v-if="clicked">按钮已点击！</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const currentTime = ref('')
const clicked = ref(false)

const updateTime = () => {
  const now = new Date()
  currentTime.value = now.toLocaleString('zh-CN')
}

const testClick = () => {
  clicked.value = true
  console.log('按钮被点击了！')
}

onMounted(() => {
  console.log('[Admin Index] 组件已挂载')
  updateTime()
  setInterval(updateTime, 1000)
})
</script>
