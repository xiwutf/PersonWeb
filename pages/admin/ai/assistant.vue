<template>
  <ClientOnly>
    <div class="admin-assistant-page">
      <div class="assistant-container">
        <!-- 左侧：会话列�?-->
        <div class="sessions-sidebar">
          <div class="sidebar-header">
            <h3 class="sidebar-title">会话列表</h3>
            <n-button
              type="primary"
              size="small"
              @click="createNewSession"
            >
              <template #icon>
                <i class="fas fa-plus"></i>
              </template>
              新建会话
            </n-button>
          </div>
          <div class="sessions-list">
            <div
              v-for="session in sessions"
              :key="session.id"
              class="session-item"
              :class="{ 'session-item-active': currentSessionId === session.id }"
              @click="loadSession(session.id)"
            >
              <div class="session-title">{{ session.title }}</div>
              <div class="session-meta">
                <span class="session-time">{{ formatDate(session.updatedAt) }}</span>
                <span class="session-count">{{ session.messageCount }} 条消�?/span>
              </div>
            </div>
            <div v-if="sessions.length === 0" class="sessions-empty">
              暂无会话，点击「新建会话」开�?            </div>
          </div>
        </div>

        <!-- 右侧：对话区�?-->
        <div class="chat-area">
          <div class="chat-header">
            <h2 class="chat-title">个人助理智能�?/h2>
            <p class="chat-subtitle">这是只为你服务的后台 AI 助理，可以帮你分析线索、规划学习、安排项目优先级等�?/p>
          </div>

          <!-- 消息列表 -->
          <div class="messages-container" ref="messagesContainer">
            <div v-if="messages.length === 0" class="welcome-message">
              <div class="welcome-content">
                <i class="fas fa-robot welcome-icon"></i>
                <p class="welcome-text">你好！我是你的个�?AI 助理，有什么可以帮你的吗？</p>
              </div>
            </div>

            <div
              v-for="(message, index) in messages"
              :key="index"
              class="message-wrapper"
              :class="{ 'message-user': message.role === 'User', 'message-assistant': message.role === 'Assistant' }"
            >
              <div class="message-avatar">
                <i v-if="message.role === 'User'" class="fas fa-user"></i>
                <i v-else class="fas fa-robot"></i>
              </div>
              <div class="message-bubble">
                <div class="message-content" v-html="formatMessage(message.content)"></div>
                <div class="message-time">{{ formatTime(message.createdAt) }}</div>
              </div>
            </div>

            <div v-if="loading" class="message-wrapper message-assistant">
              <div class="message-avatar">
                <i class="fas fa-robot"></i>
              </div>
              <div class="message-bubble">
                <div class="message-content">
                  <n-spin size="small" />
                  <span class="ml-2">正在思�?..</span>
                </div>
              </div>
            </div>
          </div>

          <!-- 快捷按钮 -->
          <div v-if="suggestions.length > 0" class="quick-suggestions">
            <n-button
              v-for="(suggestion, index) in suggestions"
              :key="index"
              secondary
              size="small"
              @click="sendQuickMessage(suggestion)"
              :disabled="loading"
            >
              {{ suggestion }}
            </n-button>
          </div>

          <!-- 输入区域 -->
          <div class="input-area">
            <n-input
              v-model:value="inputMessage"
              type="textarea"
              :rows="3"
              placeholder="输入你的问题或指�?.."
              @keydown.enter.ctrl="sendMessage"
              @keydown.enter.exact.prevent="sendMessage"
            />
            <div class="input-actions">
              <span class="input-hint">�?Enter 发送，Ctrl+Enter 换行</span>
              <n-button
                type="primary"
                :loading="loading"
                :disabled="!inputMessage.trim()"
                @click="sendMessage"
              >
                <template #icon>
                  <i class="fas fa-paper-plane"></i>
                </template>
                发�?              </n-button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { NCard, NButton, NInput, NSpin } from 'naive-ui'
import { useMarkdown } from '~/composables/useMarkdown'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

const api = useApi()
const message = useSafeMessage()
const { markdownToHtml } = useMarkdown()

const sessions = ref<any[]>([])
const currentSessionId = ref<number | null>(null)
const messages = ref<any[]>([])
const inputMessage = ref('')
const loading = ref(false)
const suggestions = ref<string[]>([])
const messagesContainer = ref<HTMLElement | null>(null)

// 当前用户 ID（应从登录状态获取）
const currentUserId = ref(1) // TODO: 从认证状态获�?
// 加载会话列表
const loadSessions = async () => {
  try {
    const res = await api.get('/ai/assistant/sessions', {
      params: { userId: currentUserId.value }
    })
    if (res && res.sessions) {
      sessions.value = res.sessions
    }
  } catch (e: any) {
    console.error('加载会话列表失败:', e)
  }
}

// 创建新会�?const createNewSession = () => {
  currentSessionId.value = null
  messages.value = []
  suggestions.value = []
  inputMessage.value = ''
}

// 加载会话
const loadSession = async (sessionId: number) => {
  currentSessionId.value = sessionId
  try {
    const res = await api.get(`/ai/assistant/sessions/${sessionId}/messages`, {
      params: { userId: currentUserId.value }
    })
    if (res && res.messages) {
      messages.value = res.messages
      suggestions.value = []
    }
    scrollToBottom()
  } catch (e: any) {
    console.error('加载会话失败:', e)
    message.error('加载会话失败')
  }
}

// 发送消�?const sendMessage = async () => {
  if (!inputMessage.value.trim() || loading.value) return

  const userMessage = inputMessage.value.trim()
  inputMessage.value = ''

  // 添加用户消息到界�?  messages.value.push({
    role: 'User',
    content: userMessage,
    createdAt: new Date().toISOString()
  })

  scrollToBottom()

  loading.value = true
  try {
    const res = await api.post('/ai/assistant/chat', {
      sessionId: currentSessionId.value,
      message: userMessage,
      contextType: 'general',
      userId: currentUserId.value
    })

    if (res && res.success) {
      // 更新会话 ID
      if (!currentSessionId.value && res.sessionId) {
        currentSessionId.value = res.sessionId
        await loadSessions() // 刷新会话列表
      }

      // 添加 AI 回复
      messages.value.push({
        role: 'Assistant',
        content: res.reply || '',
        createdAt: new Date().toISOString()
      })

      // 更新建议
      if (res.suggestions && res.suggestions.length > 0) {
        suggestions.value = res.suggestions
      }
    } else {
      message.error(res?.errorMessage || '发送消息失�?)
    }
  } catch (e: any) {
    console.error('发送消息失�?', e)
    message.error(e.response?.data?.message || e.message || '发送消息失�?)
  } finally {
    loading.value = false
    scrollToBottom()
  }
}

// 发送快捷消�?const sendQuickMessage = (text: string) => {
  inputMessage.value = text
  sendMessage()
}

// 格式化消息（Markdown �?HTML�?const formatMessage = (content: string): string => {
  return markdownToHtml(content)
}

// 格式化日�?const formatDate = (dateString: string): string => {
  if (!dateString) return '-'
  const date = new Date(dateString)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  
  if (days === 0) return '今天'
  if (days === 1) return '昨天'
  if (days < 7) return `${days} 天前`
  return date.toLocaleDateString('zh-CN')
}

// 格式化时�?const formatTime = (dateString: string): string => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })
}

// 滚动到底�?const scrollToBottom = () => {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
    }
  })
}

// 监听消息变化，自动滚�?watch(messages, () => {
  scrollToBottom()
}, { deep: true })

onMounted(() => {
  loadSessions()
})

useHead({
  title: '个人助理 - 后台管理',
  meta: [
    { name: 'description', content: '个人 AI 助理，帮助管理员分析线索、规划学习、安排项目优先级' }
  ]
})
</script>

<style scoped>
.admin-assistant-page {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.assistant-container {
  display: flex;
  height: 100%;
  gap: 0;
}

/* 左侧会话列表 */
.sessions-sidebar {
  width: var(--spacing-75);
  border-right: 1px solid var(--n-border-color);
  display: flex;
  flex-direction: column;
  background: var(--n-color);
}

.sidebar-header {
  padding: var(--spacing-xl);
  border-bottom: 1px solid var(--n-border-color);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.sidebar-title {
  font-size: var(--text-lg);
  font-weight: 600;
  margin: 0;
  color: var(--n-text-color);
}

.sessions-list {
  flex: 1;
  overflow-y: auto;
  padding: var(--spacing-2);
}

.session-item {
  padding: var(--spacing-3);
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: all 0.2s;
  margin-bottom: var(--spacing-2);
  border: 1px solid transparent;
}

.session-item:hover {
  background: var(--n-color-hover);
}

.session-item-active {
  background: var(--n-color-active);
  border-color: var(--n-primary-color);
}

.session-title {
  font-size: var(--text-sm);
  font-weight: 500;
  color: var(--n-text-color);
  margin-bottom: var(--spacing-1);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.session-meta {
  display: flex;
  justify-content: space-between;
  font-size: var(--text-xs);
  color: var(--n-text-color-2);
}

.sessions-empty {
  padding: var(--spacing-6);
  text-align: center;
  color: var(--n-text-color-3);
  font-size: var(--text-sm);
}

/* 右侧对话区域 */
.chat-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: var(--n-color);
}

.chat-header {
  padding: var(--spacing-20) var(--spacing-6);
  border-bottom: 1px solid var(--n-border-color);
  background: var(--n-color);
}

.chat-title {
  font-size: var(--text-3xl);
  font-weight: 600;
  margin: 0 0 var(--spacing-2) 0;
  color: var(--n-text-color);
}

.chat-subtitle {
  font-size: var(--text-sm);
  color: var(--n-text-color-2);
  margin: 0;
  line-height: 1.6;
}

.messages-container {
  flex: 1;
  overflow-y: auto;
  padding: var(--spacing-6);
  background: var(--n-color);
}

.welcome-message {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  min-height: var(--spacing-75);
}

.welcome-content {
  text-align: center;
}

.welcome-icon {
  font-size: var(--text-3xl);
  color: var(--n-text-color-3);
  margin-bottom: var(--spacing-xl);
}

.welcome-text {
  font-size: var(--text-base);
  color: var(--n-text-color-2);
  margin: 0;
}

.message-wrapper {
  display: flex;
  gap: var(--spacing-3);
  margin-bottom: var(--spacing-20);
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(var(--spacing-2_5));
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.message-user {
  flex-direction: row-reverse;
}

.message-avatar {
  width: var(--spacing-9);
  height: var(--spacing-9);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-size: var(--text-base);
  background: var(--n-color);
  border: 1px solid var(--n-border-color);
  color: var(--n-text-color-2);
}

.message-user .message-avatar {
  background: linear-gradient(135deg, var(--color-primary, var(--color-indigo-500)) 0%, var(--color-purple, var(--color-purple-600)) 100%);
  border-color: transparent;
  color: var(--color-text-main, var(--color-bg-light, white));
}

.message-bubble {
  max-width: 70%;
  display: flex;
  flex-direction: column;
}

.message-user .message-bubble {
  align-items: flex-end;
}

.message-assistant .message-bubble {
  align-items: flex-start;
}

.message-content {
  padding: var(--spacing-3) var(--spacing-4);
  border-radius: var(--radius-lg);
  word-wrap: break-word;
  line-height: 1.6;
}

.message-user .message-content {
  background: linear-gradient(135deg, var(--color-primary, var(--color-indigo-500)) 0%, var(--color-purple, var(--color-purple-600)) 100%);
  color: var(--color-text-main, var(--color-bg-light, white));
  border-bottom-right-radius: var(--radius-sm);
}

.message-assistant .message-content {
  background: var(--n-color);
  border: 1px solid var(--n-border-color);
  color: var(--n-text-color);
  border-bottom-left-radius: var(--radius-sm);
}

.message-time {
  font-size: var(--text-xs);
  color: var(--n-text-color-3);
  margin-top: var(--spacing-1);
  padding: 0 var(--spacing-1);
}

.quick-suggestions {
  padding: var(--spacing-3) var(--spacing-6);
  border-top: 1px solid var(--n-border-color);
  display: flex;
  gap: var(--spacing-2);
  flex-wrap: wrap;
  background: var(--n-color);
}

.input-area {
  padding: var(--spacing-xl) var(--spacing-6);
  border-top: 1px solid var(--n-border-color);
  background: var(--n-color);
}

.input-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: var(--spacing-2);
}

.input-hint {
  font-size: var(--text-xs);
  color: var(--n-text-color-3);
}

@media (max-width: 1024px) {
  .sessions-sidebar {
    width: var(--spacing-62);
  }
}

@media (max-width: 768px) {
  .assistant-container {
    flex-direction: column;
  }

  .sessions-sidebar {
    width: 100%;
    height: 200px;
    border-right: none;
    border-bottom: 1px solid var(--n-border-color);
  }
}
</style>

