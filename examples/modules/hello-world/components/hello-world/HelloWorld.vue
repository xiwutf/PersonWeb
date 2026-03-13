<template>
  <div class="hello-world">
    <div class="hello-world__header">
      <h1 class="hello-world__title">{{ title }}</h1>
      <p class="hello-world__description">{{ description }}</p>
    </div>

    <div class="hello-world__content">
      <!-- 问候语组件 -->
      <HelloGreeting
        :greeting="greeting"
        :name="name"
        class="hello-world__greeting"
      />

      <!-- 计数器组件（可选） -->
      <div v-if="showCounter" class="hello-world__counter">
        <HelloCounter class="hello-world__counter-component" />
      </div>

      <!-- 快速操作 -->
      <div class="hello-world__actions">
        <button
          @click="changeTheme"
          class="hello-world__button"
          :class="{ 'hello-world__button--dark': theme === 'dark' }"
        >
          切换到{{ theme === 'light' ? '深色' : '浅色' }}主题
        </button>
        <button
          @click="showTime = !showTime"
          class="hello-world__button"
        >
          {{ showTime ? '隐藏时间' : '显示时间' }}
        </button>
      </div>

      <!-- 显示时间 -->
      <div v-if="showTime" class="hello-world__time">
        <p>当前时间：{{ currentTime }}</p>
        <p>今天是：{{ formattedDate }}</p>
      </div>
    </div>

    <!-- 模块信息 -->
    <div class="hello-world__info">
      <h3>模块信息</h3>
      <ul>
        <li>版本：{{ version }}</li>
        <li>作者：{{ author }}</li>
        <li>分类：{{ category }}</li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useModuleConfig } from '@personweb/module-system'
import HelloGreeting from './HelloGreeting.vue'
import HelloCounter from './HelloCounter.vue'

// 组件属性
const props = defineProps<{
  title?: string
  description?: string
}>()

// 从模块配置获取设置
const config = useModuleConfig('hello-world')

// 响应式数据
const greeting = ref(config.greeting || 'Hello')
const name = ref(config.name || 'World')
const showCounter = ref(config.showCounter !== false)
const theme = ref(config.theme || 'light')
const showTime = ref(false)
const currentTime = ref('')

// 计算属性
const title = computed(() => props.title || 'Hello World')
const description = computed(() => props.description || '这是一个简单的示例模块，展示模块开发的基本概念')
const version = ref('1.0.0')
const author = ref('PersonWeb Team')
const category = ref('tools')

// 时间更新计时器
let timeInterval: NodeJS.Timeout

// 格式化日期
const formattedDate = computed(() => new Date().toLocaleDateString('zh-CN'))

// 方法
const changeTheme = () => {
  theme.value = theme.value === 'light' ? 'dark' : 'light'
  // 更新配置
  config.theme = theme.value
}

const updateTime = () => {
  currentTime.value = new Date().toLocaleTimeString('zh-CN')
}

// 生命周期
onMounted(() => {
  // 初始化时间
  updateTime()

  // 设置定时器更新时间
  timeInterval = setInterval(updateTime, 1000)
})

onUnmounted(() => {
  // 清理定时器
  if (timeInterval) {
    clearInterval(timeInterval)
  }
})

// 导出方法供父组件使用
defineExpose({
  changeTheme,
  showTime,
  updateTime
})
</script>

<style scoped>
.hello-world {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.hello-world__header {
  text-align: center;
  margin-bottom: 2rem;
}

.hello-world__title {
  font-size: 2.5rem;
  color: var(--color-primary, #3b82f6);
  margin-bottom: 0.5rem;
}

.hello-world__description {
  font-size: 1.1rem;
  color: var(--color-secondary, #6b7280);
}

.hello-world__content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.hello-world__greeting {
  text-align: center;
  font-size: 1.5rem;
  padding: 1rem;
  background-color: var(--color-background, #f3f4f6);
  border-radius: 4px;
}

.hello-world__counter {
  display: flex;
  justify-content: center;
}

.hello-world__counter-component {
  padding: 1rem;
  background-color: var(--color-background, #f3f4f6);
  border-radius: 4px;
}

.hello-world__actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
}

.hello-world__button {
  padding: 0.5rem 1rem;
  background-color: var(--color-primary, #3b82f6);
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.hello-world__button:hover {
  background-color: var(--color-primary-dark, #2563eb);
  transform: translateY(-1px);
}

.hello-world__button--dark {
  background-color: var(--color-dark, #1f2937);
}

.hello-world__button--dark:hover {
  background-color: var(--color-dark-dark, #111827);
}

.hello-world__time {
  text-align: center;
  padding: 1rem;
  background-color: var(--color-background, #f3f4f6);
  border-radius: 4px;
  font-family: monospace;
}

.hello-world__info {
  margin-top: 2rem;
  padding: 1rem;
  background-color: var(--color-background, #f3f4f6);
  border-radius: 4px;
}

.hello-world__info h3 {
  margin-top: 0;
  color: var(--color-primary, #3b82f6);
}

.hello-world__info ul {
  list-style: none;
  padding: 0;
}

.hello-world__info li {
  padding: 0.25rem 0;
}

/* 深色主题支持 */
:root {
  --color-primary: #3b82f6;
  --color-primary-dark: #2563eb;
  --color-secondary: #6b7280;
  --color-background: #f3f4f6;
  --color-dark: #1f2937;
  --color-dark-dark: #111827;
}

.dark {
  --color-primary: #60a5fa;
  --color-primary-dark: #3b82f6;
  --color-secondary: #9ca3af;
  --color-background: #374151;
  --color-dark: #111827;
  --color-dark-dark: #000000;
}
</style>