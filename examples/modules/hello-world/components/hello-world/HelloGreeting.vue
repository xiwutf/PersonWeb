<template>
  <div class="hello-greeting">
    <div class="hello-greeting__message">
      {{ greeting }}, {{ name }}! 🎉
    </div>

    <div v-if="showTime" class="hello-greeting__time">
      <p>现在时间：{{ currentTime }}</p>
      <p>今天日期：{{ currentDate }}</p>
    </div>

    <div class="hello-greeting__actions">
      <button
        @click="changeGreeting"
        class="hello-greeting__button"
      >
        切换问候语
      </button>

      <button
        @click="toggleTime"
        class="hello-greeting__button"
      >
        {{ showTime ? '隐藏时间' : '显示时间' }}
      </button>

      <button
        @click="greet"
        class="hello-greeting__button hello-greeting__button--primary"
      >
        问候
      </button>
    </div>

    <!-- 插槽用于自定义内容 -->
    <div v-if="$slots.default" class="hello-greeting__custom">
      <slot></slot>
    </div>

    <!-- 问候历史 -->
    <div v-if="greetingHistory.length > 0" class="hello-greeting__history">
      <h4>问候历史：</h4>
      <ul>
        <li v-for="(item, index) in greetingHistory" :key="index">
          {{ item.greeting }}, {{ item.name }}! （{{ item.time }}）
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'

// 组件属性
interface Props {
  greeting?: string
  name?: string
  showTime?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  greeting: 'Hello',
  name: 'World',
  showTime: true
})

// 事件
const emit = defineEmits<{
  greet: [greeting: string, name: string]
  greetingChange: [newGreeting: string]
}>()

// 响应式数据
const currentGreeting = ref(props.greeting)
const currentName = ref(props.name)
const showTime = ref(props.showTime)
const currentTime = ref('')
const currentDate = ref('')
const greetingHistory = ref<Array<{
  greeting: string
  name: string
  time: string
}>>([])

// 预定义的问候语列表
const greetings = [
  'Hello',
  'Hi',
  '你好',
  'Bonjour',
  'Hola',
  'Ciao',
  'こんにちは',
  '안녕하세요',
  'Привет'
]

// 方法
const changeGreeting = () => {
  const currentIndex = greetings.indexOf(currentGreeting.value)
  const nextIndex = (currentIndex + 1) % greetings.length
  currentGreeting.value = greetings[nextIndex]
  emit('greetingChange', currentGreeting.value)

  // 添加到历史记录
  addToHistory(currentGreeting.value, currentName.value)
}

const changeName = () => {
  // 可以扩展为让用户输入新名字
  const names = ['Alice', 'Bob', 'Charlie', 'David', 'Eve']
  const currentIndex = names.indexOf(currentName.value)
  const nextIndex = (currentIndex + 1) % names.length
  currentName.value = names[nextIndex]

  // 添加到历史记录
  addToHistory(currentGreeting.value, currentName.value)
}

const toggleTime = () => {
  showTime.value = !showTime.value
}

const greet = () => {
  // 触发问候事件
  emit('greet', currentGreeting.value, currentName.value)

  // 添加到历史记录
  addToHistory(currentGreeting.value, currentName.value)

  // 显示问候动画
  showGreetingAnimation()
}

const addToHistory = (greeting: string, name: string) => {
  const time = new Date().toLocaleTimeString('zh-CN')
  greetingHistory.value.unshift({
    greeting,
    name,
    time
  })

  // 只保留最近5条记录
  if (greetingHistory.value.length > 5) {
    greetingHistory.value.pop()
  }
}

const showGreetingAnimation = () => {
  // 添加动画类
  const messageEl = document.querySelector('.hello-greeting__message')
  if (messageEl) {
    messageEl.classList.add('hello-greeting__message--animate')
    setTimeout(() => {
      messageEl.classList.remove('hello-greeting__message--animate')
    }, 1000)
  }
}

// 更新时间
const updateTime = () => {
  currentTime.value = new Date().toLocaleTimeString('zh-CN')
  currentDate.value = new Date().toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    weekday: 'long'
  })
}

// 定时器
let timeInterval: NodeJS.Timeout

// 生命周期
onMounted(() => {
  updateTime()
  timeInterval = setInterval(updateTime, 1000)

  // 初始化历史记录
  addToHistory(currentGreeting.value, currentName.value)
})

onUnmounted(() => {
  if (timeInterval) {
    clearInterval(timeInterval)
  }
})

// 监听props变化
watch(() => props.greeting, (newGreeting) => {
  if (newGreeting !== currentGreeting.value) {
    currentGreeting.value = newGreeting
  }
})

watch(() => props.name, (newName) => {
  if (newName !== currentName.value) {
    currentName.value = newName
  }
})

// 计算属性
const message = computed(() => `${currentGreeting.value}, ${currentName.value}!`)

// 导出方法供父组件使用
defineExpose({
  changeGreeting,
  changeName,
  toggleTime,
  greet,
  addToHistory,
  message
})
</script>

<style scoped>
.hello-greeting {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1.5rem;
  background-color: var(--color-background, #f3f4f6);
  border-radius: 8px;
}

.hello-greeting__message {
  font-size: 1.5rem;
  font-weight: bold;
  text-align: center;
  color: var(--color-primary, #3b82f6);
  padding: 1rem;
  background-color: var(--color-primary-light, #dbeafe);
  border-radius: 4px;
  transition: all 0.3s;
}

.hello-greeting__message--animate {
  animation: bounce 0.5s ease;
}

@keyframes bounce {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.1); }
}

.hello-greeting__time {
  text-align: center;
  padding: 0.5rem;
  background-color: var(--color-secondary-light, #f3f4f6);
  border-radius: 4px;
  font-family: monospace;
  font-size: 0.9rem;
}

.hello-greeting__actions {
  display: flex;
  gap: 0.5rem;
  justify-content: center;
  flex-wrap: wrap;
}

.hello-greeting__button {
  padding: 0.5rem 1rem;
  background-color: var(--color-gray, #9ca3af);
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 0.9rem;
}

.hello-greeting__button:hover {
  background-color: var(--color-gray-dark, #6b7280);
  transform: translateY(-1px);
}

.hello-greeting__button--primary {
  background-color: var(--color-primary, #3b82f6);
}

.hello-greeting__button--primary:hover {
  background-color: var(--color-primary-dark, #2563eb);
}

.hello-greeting__custom {
  padding: 1rem;
  background-color: var(--color-background, #f3f4f6);
  border: 1px dashed var(--color-border, #d1d5db);
  border-radius: 4px;
}

.hello-greeting__history {
  padding: 1rem;
  background-color: var(--color-background, #f3f4f6);
  border-radius: 4px;
  max-height: 200px;
  overflow-y: auto;
}

.hello-greeting__history h4 {
  margin-top: 0;
  color: var(--color-primary, #3b82f6);
}

.hello-greeting__history ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.hello-greeting__history li {
  padding: 0.25rem 0;
  border-bottom: 1px solid var(--color-border, #d1d5db);
  font-size: 0.85rem;
  color: var(--color-secondary, #6b7280);
}

.hello-greeting__history li:last-child {
  border-bottom: none;
}

/* 深色主题支持 */
.dark {
  --color-background: #374151;
  --color-primary: #60a5fa;
  --color-primary-dark: #3b82f6;
  --color-primary-light: #1e40af;
  --color-secondary: #9ca3af;
  --color-secondary-light: #4b5563;
  --color-gray: #4b5563;
  --color-gray-dark: #374151;
  --color-border: #4b5563;
}
</style>