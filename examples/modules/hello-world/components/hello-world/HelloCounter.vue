<template>
  <div class="hello-counter">
    <div class="hello-counter__display">
      <span class="hello-counter__count">{{ count }}</span>
      <span class="hello-counter__label">次</span>
    </div>

    <div class="hello-counter__controls">
      <button
        @click="increment"
        class="hello-counter__button hello-counter__button--primary"
        :disabled="isMaxCount"
      >
        <span>+</span>
        <span class="hello-counter__button-text">增加</span>
      </button>

      <button
        @click="decrement"
        class="hello-counter__button hello-counter__button--secondary"
        :disabled="count === 0"
      >
        <span>-</span>
        <span class="hello-counter__button-text">减少</span>
      </button>

      <button
        @click="reset"
        class="hello-counter__button hello-counter__button--reset"
      >
        <span>↺</span>
        <span class="hello-counter__button-text">重置</span>
      </button>
    </div>

    <div class="hello-counter__stats">
      <p>当前计数：{{ count }}</p>
      <p v-if="count > 0">
        已增加 {{ incrementCount }} 次，已减少 {{ decrementCount }} 次
      </p>
      <p v-if="count >= maxCount" class="hello-counter__warning">
        已达到最大值！
      </p>
    </div>

    <!-- 模板插槽 -->
    <div v-if="$slots.footer" class="hello-counter__footer">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'

// 组件属性
interface Props {
  initialCount?: number
  maxCount?: number
  minCount?: number
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  initialCount: 0,
  maxCount: 100,
  minCount: 0,
  label: '计数'
})

// 事件
const emit = defineEmits<{
  countChange: [value: number]
  reachMax: []
  reachMin: []
}>()

// 响应式数据
const count = ref(props.initialCount)
const incrementCount = ref(0)
const decrementCount = ref(0)

// 计算属性
const isMaxCount = computed(() => count.value >= props.maxCount)
const isMinCount = computed(() => count.value <= props.minCount)

// 方法
const increment = () => {
  if (!isMaxCount.value) {
    count.value++
    incrementCount.value++
    emit('countChange', count.value)

    if (count.value === props.maxCount) {
      emit('reachMax')
    }
  }
}

const decrement = () => {
  if (!isMinCount.value) {
    count.value--
    decrementCount.value++
    emit('countChange', count.value)

    if (count.value === props.minCount) {
      emit('reachMin')
    }
  }
}

const reset = () => {
  count.value = props.initialCount
  incrementCount.value = 0
  decrementCount.value = 0
  emit('countChange', count.value)
}

// 监听器
watch(count, (newCount, oldCount) => {
  console.log(`计数从 ${oldCount} 变为 ${newCount}`)
})

// 初始化
console.log(`HelloCounter 初始化，初始值：${count.value}`)

// 导出方法供父组件使用
defineExpose({
  increment,
  decrement,
  reset,
  count
})
</script>

<style scoped>
.hello-counter {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1.5rem;
  background-color: var(--color-background, #f3f4f6);
  border-radius: 8px;
  max-width: 400px;
  margin: 0 auto;
}

.hello-counter__display {
  text-align: center;
  padding: 1rem;
  background-color: var(--color-primary, #3b82f6);
  border-radius: 4px;
  color: white;
}

.hello-counter__count {
  font-size: 3rem;
  font-weight: bold;
  display: inline-block;
  min-width: 3ch;
}

.hello-counter__label {
  font-size: 1.2rem;
  margin-left: 0.5rem;
}

.hello-counter__controls {
  display: flex;
  gap: 0.5rem;
  justify-content: center;
}

.hello-counter__button {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
  font-weight: 500;
}

.hello-counter__button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.hello-counter__button:hover:not(:disabled) {
  transform: translateY(-1px);
}

.hello-counter__button--primary {
  background-color: var(--color-primary, #3b82f6);
  color: white;
}

.hello-counter__button--primary:hover:not(:disabled) {
  background-color: var(--color-primary-dark, #2563eb);
}

.hello-counter__button--secondary {
  background-color: var(--color-secondary, #6b7280);
  color: white;
}

.hello-counter__button--secondary:hover:not(:disabled) {
  background-color: var(--color-secondary-dark, #4b5563);
}

.hello-counter__button--reset {
  background-color: var(--color-gray, #9ca3af);
  color: white;
}

.hello-counter__button--reset:hover:not(:disabled) {
  background-color: var(--color-gray-dark, #6b7280);
}

.hello-counter__stats {
  text-align: center;
  padding: 0.5rem;
  font-size: 0.9rem;
  color: var(--color-secondary, #6b7280);
}

.hello-counter__warning {
  color: var(--color-error, #ef4444);
  font-weight: bold;
}

.hello-counter__footer {
  margin-top: 0.5rem;
  padding-top: 0.5rem;
  border-top: 1px solid var(--color-border, #e5e7eb);
}

/* 深色主题支持 */
.dark {
  --color-background: #374151;
  --color-primary: #60a5fa;
  --color-primary-dark: #3b82f6;
  --color-secondary: #9ca3af;
  --color-secondary-dark: #6b7280;
  --color-gray: #4b5563;
  --color-gray-dark: #374151;
  --color-border: #4b5563;
  --color-error: #f87171;
}
</style>