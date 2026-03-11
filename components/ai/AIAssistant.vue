<template>
  <ClientOnly>
    <!-- AI 助手按钮 -->
    <button
      v-if="!isOpen"
      @click="toggleAssistant"
      class="ai-assistant-button"
      :class="[
        `ai-assistant-button-${config.position}`,
        `ai-assistant-button-${config.theme}`
      ]"
      aria-label="打开 AI 助手"
    >
      <svg class="ai-assistant-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
      </svg>
      <span
        v-if="statusText !== '离线'"
        class="ai-assistant-status-dot"
        :class="{ 'animate-pulse': statusText === '在线' }"
      ></span>
    </button>

    <!-- AI 助手对话框 -->
    <Transition name="slide-up">
      <div
        v-if="isOpen"
        class="ai-assistant-dialog"
        :class="`ai-assistant-dialog-${config.theme}`"
      >
        <!-- 头部 -->
        <div class="ai-assistant-header">
          <div class="ai-assistant-header-content">
            <div class="ai-assistant-avatar">
              <svg class="ai-assistant-avatar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
              </svg>
            </div>
            <div>
              <h3 class="ai-assistant-title">AI 小智</h3>
              <p class="ai-assistant-status">{{ statusText }}</p>
            </div>
          </div>
          <button
            @click="toggleAssistant"
            class="ai-assistant-close-btn"
            aria-label="关闭"
          >
            <svg class="ai-assistant-close-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- 消息列表 -->
        <div ref="messagesRef" class="ai-assistant-messages">
          <!-- 欢迎消息 -->
          <div v-if="messages.length === 0" class="ai-assistant-welcome">
            <div class="ai-assistant-message ai-assistant-message-assistant">
              <p class="ai-assistant-message-text">👋 你好！我是 AI 小智，有什么可以帮你的吗？</p>
            </div>
          </div>

          <!-- 消息列表 -->
          <div
            v-for="(message, index) in messages"
            :key="index"
            :class="[
              'ai-assistant-message-wrapper',
              message.role === 'user' ? 'ai-assistant-message-user-wrapper' : 'ai-assistant-message-assistant-wrapper'
            ]"
          >
            <div
              :class="[
                'ai-assistant-message',
                message.role === 'user' ? 'ai-assistant-message-user' : 'ai-assistant-message-assistant'
              ]"
            >
              <p class="ai-assistant-message-text">{{ message.content }}</p>
              <p v-if="message.role === 'assistant' && message.loading" class="ai-assistant-message-loading">正在思考...</p>
            </div>
          </div>

          <!-- 加载中 -->
          <div v-if="isLoading" class="ai-assistant-message-wrapper ai-assistant-message-assistant-wrapper">
            <div class="ai-assistant-message ai-assistant-message-assistant">
              <div class="ai-assistant-loading">
                <div class="ai-assistant-loading-dot" style="animation-delay: 0s"></div>
                <div class="ai-assistant-loading-dot" style="animation-delay: 0.2s"></div>
                <div class="ai-assistant-loading-dot" style="animation-delay: 0.4s"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- 输入区域 -->
        <div class="ai-assistant-input-area">
          <!-- 快捷操作 -->
          <div v-if="config.enableQuickActions" class="ai-assistant-quick-actions">
            <button
              v-for="quickAction in quickActions"
              :key="quickAction.text"
              @click="sendQuickMessage(quickAction.text)"
              class="ai-assistant-quick-action-btn"
              :disabled="isLoading"
            >
              {{ quickAction.text }}
            </button>
          </div>

          <!-- 输入框 -->
          <div class="ai-assistant-input-wrapper">
            <input
              v-model="inputText"
              @keyup.enter="handleSendMessage"
              @keyup.escape="inputText = ''"
              type="text"
              :placeholder="inputPlaceholder"
              class="ai-assistant-input"
              :disabled="isLoading"
              ref="inputRef"
            />
            <button
              @click="handleSendMessage"
              :disabled="isLoading || !inputText.trim()"
              class="ai-assistant-send-btn"
            >
              <svg class="ai-assistant-send-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </ClientOnly>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, nextTick } from 'vue'
import { useAIAssistant, useAIAssistantTools } from '~/modules/ai-assistant/composables'
import { useSimpleModuleSystem } from '~/composables/simpleModuleSystem'

const { isOpen, messages, isLoading, config, statusText, quickActions, toggleAssistant, sendAIMessage, sendQuickMessage, messagesRef } = useAIAssistant()
const { validateApiKey } = useAIAssistantTools()
const { isModuleEnabled } = useModuleSystem()

// 输入相关
const inputText = ref('')
const inputRef = ref<HTMLInputElement>()

// 计算属性
const inputPlaceholder = computed(() => {
  if (!config.value.apiKey) return '请先配置API密钥'
  return '输入消息...'
})

// 方法定义
const handleSendMessage = () => {
  if (!inputText.value.trim() || isLoading.value) return

  sendAIMessage(inputText.value)
  inputText.value = ''
}

// 自动聚焦输入框
watch(isOpen, (newVal) => {
  if (newVal && inputRef.value) {
    nextTick(() => {
      inputRef.value?.focus()
    })
  }
})

// 初始化
onMounted(async () => {
  if (isModuleEnabled('ai-assistant')) {
    // 加载配置
    // 配置会在 useAIAssistant 中自动加载
  }
})
</script>

<style scoped>
/* 按钮样式 */
.ai-assistant-button {
  position: fixed !important;
  z-index: 10000 !important;
  display: flex !important;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.3s ease;
  isolation: isolate;
  transform: translateZ(0);
  pointer-events: auto !important;
  visibility: visible !important;
  opacity: 1 !important;
}

.ai-assistant-button.bottom-right {
  bottom: 0.5rem !important;
  right: 0.5rem !important;
}

.ai-assistant-button.bottom-left {
  bottom: 0.5rem !important;
  left: 0.5rem !important;
}

.ai-assistant-button.top-right {
  top: 0.5rem !important;
  right: 0.5rem !important;
}

.ai-assistant-button.top-left {
  top: 0.5rem !important;
  left: 0.5rem !important;
}

.ai-assistant-button-light {
  background: linear-gradient(to bottom right, #3b82f6, #8b5cf6);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
}

.ai-assistant-button-light:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 20px rgba(59, 130, 246, 0.5);
}

.ai-assistant-button-dark {
  background: linear-gradient(to bottom right, #1e293b, #475569);
  box-shadow: 0 4px 12px rgba(30, 41, 59, 0.4);
}

.ai-assistant-button-dark:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 20px rgba(30, 41, 59, 0.5);
}

.ai-assistant-icon {
  width: 24px;
  height: 24px;
  color: white;
}

.ai-assistant-status-dot {
  position: absolute;
  top: -4px;
  right: -4px;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  border: 2px solid white;
}

.ai-assistant-status-dot.animate-pulse {
  animation: pulse 2s infinite;
}

/* 对话框样式 */
.ai-assistant-dialog {
  position: fixed !important;
  z-index: 10000 !important;
  display: flex !important;
  flex-direction: column;
  pointer-events: auto !important;
  visibility: visible !important;
  opacity: 1 !important;
  isolation: isolate;
  transform: translateZ(0);
}

.ai-assistant-dialog.bottom-right {
  bottom: 4rem !important;
  right: 0.5rem !important;
}

.ai-assistant-dialog.bottom-left {
  bottom: 4rem !important;
  left: 0.5rem !important;
}

.ai-assistant-dialog.top-right {
  top: 0.5rem !important;
  right: 0.5rem !important;
}

.ai-assistant-dialog.top-left {
  top: 0.5rem !important;
  left: 0.5rem !important;
}

.ai-assistant-dialog-light {
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
  width: 400px;
  max-width: 90vw;
}

.ai-assistant-dialog-dark {
  background: #1e293b;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  width: 400px;
  max-width: 90vw;
}

/* 头部样式 */
.ai-assistant-header {
  padding: 16px;
  border-bottom: 1px solid var(--border-color);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.ai-assistant-header-content {
  display: flex;
  align-items: center;
  gap: 12px;
}

.ai-assistant-avatar {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(59, 130, 246, 0.1);
  border-radius: 50%;
}

.ai-assistant-avatar-icon {
  width: 24px;
  height: 24px;
  color: #3b82f6;
}

.ai-assistant-title {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: var(--text-color);
}

.ai-assistant-status {
  margin: 0;
  font-size: 14px;
  color: var(--text-secondary);
}

.ai-assistant-close-btn {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  cursor: pointer;
  border-radius: 50%;
  transition: background 0.2s;
}

.ai-assistant-close-btn:hover {
  background: rgba(0, 0, 0, 0.05);
}

.ai-assistant-close-icon {
  width: 20px;
  height: 20px;
  color: var(--text-secondary);
}

/* 消息列表样式 */
.ai-assistant-messages {
  flex: 1;
  overflow-y: auto;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  max-height: 400px;
}

.ai-assistant-welcome {
  text-align: center;
  padding: 20px;
  color: var(--text-secondary);
}

.ai-assistant-message-wrapper {
  display: flex;
  justify-content: flex-start;
  animation: fadeIn 0.3s ease;
}

.ai-assistant-message-user-wrapper {
  justify-content: flex-end;
}

.ai-assistant-message {
  max-width: 80%;
  padding: 12px 16px;
  border-radius: 16px;
  word-wrap: break-word;
}

.ai-assistant-message-user {
  background: var(--primary-color);
  color: white;
  border-bottom-right-radius: 4px;
}

.ai-assistant-message-assistant {
  background: var(--background-secondary);
  color: var(--text-color);
  border-bottom-left-radius: 4px;
}

.ai-assistant-message-text {
  margin: 0;
  line-height: 1.5;
}

.ai-assistant-message-loading {
  margin-top: 8px;
  font-size: 14px;
  color: var(--text-secondary);
}

.ai-assistant-loading {
  display: flex;
  gap: 4px;
}

.ai-assistant-loading-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--text-secondary);
  animation: loading 1.4s infinite ease-in-out;
}

/* 输入区域样式 */
.ai-assistant-input-area {
  padding: 16px;
  border-top: 1px solid var(--border-color);
}

.ai-assistant-quick-actions {
  display: flex;
  gap: 8px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.ai-assistant-quick-action-btn {
  padding: 6px 12px;
  border-radius: 20px;
  background: var(--background-secondary);
  border: 1px solid var(--border-color);
  color: var(--text-color);
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
}

.ai-assistant-quick-action-btn:hover {
  background: var(--background-hover);
  border-color: var(--primary-color);
  color: var(--primary-color);
}

.ai-assistant-quick-action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.ai-assistant-input-wrapper {
  display: flex;
  gap: 8px;
  align-items: center;
}

.ai-assistant-input {
  flex: 1;
  padding: 10px 16px;
  border: 1px solid var(--border-color);
  border-radius: 24px;
  background: var(--background-secondary);
  color: var(--text-color);
  font-size: 14px;
  outline: none;
  transition: border-color 0.2s;
}

.ai-assistant-input:focus {
  border-color: var(--primary-color);
}

.ai-assistant-input:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.ai-assistant-send-btn {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: var(--primary-color);
  border: none;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.ai-assistant-send-btn:hover {
  background: var(--primary-color-dark);
  transform: scale(1.05);
}

.ai-assistant-send-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: scale(1);
}

.ai-assistant-send-icon {
  width: 20px;
  height: 20px;
}

/* 动画 */
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}

.slide-up-enter-from {
  transform: translateY(20px);
  opacity: 0;
}

.slide-up-leave-to {
  transform: translateY(20px);
  opacity: 0;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes loading {
  0%, 80%, 100% {
    transform: scale(0.8);
    opacity: 0.5;
  }
  40% {
    transform: scale(1);
    opacity: 1;
  }
}

@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(59, 130, 246, 0.7);
  }
  70% {
    box-shadow: 0 0 0 4px rgba(59, 130, 246, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(59, 130, 246, 0);
  }
}

/* 响应式设计 */
@media (max-width: 480px) {
  .ai-assistant-dialog {
    width: 100% !important;
    height: 100vh !important;
    border-radius: 0 !important;
  }

  .ai-assistant-dialog.bottom-right,
  .ai-assistant-dialog.bottom-left {
    bottom: 0 !important;
    right: 0 !important;
    left: 0 !important;
  }

  .ai-assistant-dialog.top-right,
  .ai-assistant-dialog.top-left {
    top: 0 !important;
    right: 0 !important;
    left: 0 !important;
  }

  .ai-assistant-messages {
    max-height: calc(100vh - 200px);
  }
}
</style>