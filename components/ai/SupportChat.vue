<template>
  <div class="support-chat-container">
    <!-- 悬浮按钮 -->
    <div
      v-if="!showChat"
      class="support-chat-button"
      @click.stop="openChat"
      role="button"
      tabindex="0"
      @keydown.enter="openChat"
    >
      <i class="fas fa-comments"></i>
      <span class="button-text">智能客服</span>
    </div>

    <!-- 聊天窗口 -->
    <n-drawer
      v-model:show="showChat"
      :width="400"
      placement="right"
      :mask-closable="true"
    >
      <template #header>
        <div class="chat-header">
          <div class="header-info">
            <h3 class="header-title">智能客服</h3>
            <p class="header-subtitle">可以向我咨询服务内容、项目开发、工具使用等问题</p>
          </div>
          <n-button
            text
            size="small"
            @click="closeChat"
          >
            <template #icon>
              <i class="fas fa-times"></i>
            </template>
          </n-button>
        </div>
      </template>

      <div class="chat-content">
        <!-- 消息列表 -->
        <div class="messages-list" ref="messagesContainer">
          <div
            v-for="(message, index) in messages"
            :key="index"
            class="message-item"
            :class="{ 'message-user': message.role === 'user', 'message-assistant': message.role === 'assistant' }"
          >
            <div class="message-avatar">
              <i
                v-if="message.role === 'user'"
                class="fas fa-user"
              ></i>
              <i
                v-else
                class="fas fa-robot"
              ></i>
            </div>
            <div class="message-content">
              <div class="message-text" v-html="formatMessage(message.content)"></div>
              <div v-if="message.relatedLinks && message.relatedLinks.length > 0" class="message-links">
                <div
                  v-for="(link, linkIndex) in message.relatedLinks"
                  :key="linkIndex"
                  class="link-item"
                >
                  <NuxtLink :to="parseLink(link)" class="link-text">
                    <i class="fas fa-link"></i>
                    {{ getLinkText(link) }}
                  </NuxtLink>
                </div>
              </div>
              <div v-if="message.needHuman" class="message-human-tip">
                <i class="fas fa-exclamation-circle"></i>
                建议人工跟进
              </div>
            </div>
          </div>
          <div v-if="loading" class="message-item message-assistant">
            <div class="message-avatar">
              <i class="fas fa-robot"></i>
            </div>
            <div class="message-content">
              <div class="message-text">
                <n-spin size="small" />
                <span class="ml-2">正在思考...</span>
              </div>
            </div>
          </div>
        </div>

        <!-- 输入区域 -->
        <div class="chat-input-area">
          <n-input
            v-model:value="inputMessage"
            type="textarea"
            :rows="2"
            placeholder="输入您的问题..."
            @keydown.enter.ctrl="sendMessage"
            @keydown.enter.exact.prevent="sendMessage"
          />
          <div class="input-actions">
            <span class="input-hint">按 Enter 发送，Ctrl+Enter 换行</span>
            <n-button
              type="primary"
              :loading="loading"
              :disabled="!inputMessage.trim()"
              @click="sendMessage"
            >
              <template #icon>
                <i class="fas fa-paper-plane"></i>
              </template>
              发送
            </n-button>
          </div>
        </div>
      </div>
    </n-drawer>
  </div>
</template>

<script setup lang="ts">
import { NDrawer, NInput, NButton, NSpin } from 'naive-ui'
import { useMarkdown } from '~/composables/useMarkdown'

const api = useApi()
const message = useSafeMessage()
const { markdownToHtml } = useMarkdown()

const showChat = ref(false)
const inputMessage = ref('')
const loading = ref(false)
const messages = ref<Array<{
  role: 'user' | 'assistant'
  content: string
  relatedLinks?: string[]
  needHuman?: boolean
}>>([])
const messagesContainer = ref<HTMLElement | null>(null)

// 打开聊天窗口
const openChat = () => {
  showChat.value = true
  // 添加欢迎消息
  if (messages.value.length === 0) {
    messages.value.push({
      role: 'assistant',
      content: '你好！我是智能客服，可以帮你解答关于服务内容、项目开发、工具使用等问题。有什么可以帮你的吗？'
    })
  }
}

// 关闭聊天窗口
const closeChat = () => {
  showChat.value = false
}

// 发送消息
const sendMessage = async () => {
  if (!inputMessage.value.trim() || loading.value) return

  const userMessage = inputMessage.value.trim()
  inputMessage.value = ''

  // 添加用户消息
  messages.value.push({
    role: 'user',
    content: userMessage
  })

  // 滚动到底部
  nextTick(() => {
    scrollToBottom()
  })

  // 调用 API
  loading.value = true
  try {
    const res = await api.post('/AiAgent/support/answer', {
      question: userMessage,
      category: 'general',
      pageContext: getPageContext(),
      userMeta: getUserMeta()
    })

    if (res && res.success) {
      messages.value.push({
        role: 'assistant',
        content: res.answer || '',
        relatedLinks: res.relatedLinks || [],
        needHuman: res.needHuman || false
      })
    } else {
      message.error('获取回答失败，请稍后重试')
      messages.value.push({
        role: 'assistant',
        content: '抱歉，我暂时无法回答这个问题，请稍后重试或联系人工客服。'
      })
    }
  } catch (e: any) {
    console.error('发送消息失败:', e)
    message.error('发送消息失败，请稍后重试')
    messages.value.push({
      role: 'assistant',
      content: '抱歉，服务暂时不可用，请稍后重试。'
    })
  } finally {
    loading.value = false
    nextTick(() => {
      scrollToBottom()
    })
  }
}

// 获取页面上下文
const getPageContext = (): string => {
  if (typeof window === 'undefined') return ''
  try {
    const route = useRoute()
    const path = route.path
    
    if (path.startsWith('/projects')) return '项目展示页'
    if (path.startsWith('/tools')) return '工具详情页'
    if (path.startsWith('/blog')) return '博客文章页'
    return '首页'
  } catch {
    return ''
  }
}

// 获取用户元信息
const getUserMeta = (): string => {
  if (typeof window === 'undefined') return '未登录访客'
  const token = localStorage.getItem('admin_token')
  return token ? '已登录用户' : '未登录访客'
}

// 格式化消息（Markdown 转 HTML）
const formatMessage = (content: string): string => {
  return markdownToHtml(content)
}

// 解析链接（格式：链接文本|链接地址）
const parseLink = (link: string): string => {
  const parts = link.split('|')
  return parts.length > 1 ? parts[1] : link
}

// 获取链接文本
const getLinkText = (link: string): string => {
  const parts = link.split('|')
  return parts.length > 1 ? parts[0] : link
}

// 滚动到底部
const scrollToBottom = () => {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

// 监听消息变化，自动滚动
watch(messages, () => {
  nextTick(() => {
    scrollToBottom()
  })
}, { deep: true })
</script>

<style scoped>
.support-chat-container {
  position: fixed;
  bottom: 24px;
  right: 24px;
  z-index: 1000;
}

.support-chat-button {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 50px;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
  transition: all 0.3s ease;
  font-weight: 500;
}

.support-chat-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(102, 126, 234, 0.5);
}

.support-chat-button i {
  font-size: 1.25rem;
}

.button-text {
  font-size: 0.9375rem;
}

.chat-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding-bottom: 16px;
  border-bottom: 1px solid var(--n-border-color);
}

.header-info {
  flex: 1;
}

.header-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin: 0 0 4px 0;
  color: var(--n-text-color);
}

.header-subtitle {
  font-size: 0.875rem;
  color: var(--n-text-color-2);
  margin: 0;
}

.chat-content {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.messages-list {
  flex: 1;
  overflow-y: auto;
  padding: 16px 0;
  min-height: 300px;
  max-height: calc(100vh - 200px);
}

.message-item {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
  animation: fadeIn 0.3s ease;
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

.message-user {
  flex-direction: row-reverse;
}

.message-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-size: 1rem;
}

.message-user .message-avatar {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.message-assistant .message-avatar {
  background: var(--n-color);
  border: 1px solid var(--n-border-color);
  color: var(--n-text-color-2);
}

.message-content {
  flex: 1;
  max-width: 75%;
}

.message-user .message-content {
  text-align: right;
}

.message-text {
  padding: 10px 14px;
  border-radius: 12px;
  word-wrap: break-word;
  line-height: 1.6;
}

.message-user .message-text {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-bottom-right-radius: 4px;
}

.message-assistant .message-text {
  background: var(--n-color);
  border: 1px solid var(--n-border-color);
  color: var(--n-text-color);
  border-bottom-left-radius: 4px;
}

.message-links {
  margin-top: 8px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.message-user .message-links {
  align-items: flex-end;
}

.link-item {
  font-size: 0.875rem;
}

.link-text {
  color: #667eea;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 4px;
  transition: color 0.2s;
}

.link-text:hover {
  color: #764ba2;
  text-decoration: underline;
}

.message-human-tip {
  margin-top: 8px;
  padding: 6px 10px;
  background: rgba(239, 68, 68, 0.1);
  border-left: 3px solid #ef4444;
  border-radius: 4px;
  font-size: 0.8125rem;
  color: #dc2626;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.chat-input-area {
  padding-top: 16px;
  border-top: 1px solid var(--n-border-color);
}

.input-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 8px;
}

.input-hint {
  font-size: 0.75rem;
  color: var(--n-text-color-3);
}

@media (max-width: 768px) {
  .support-chat-button {
    padding: 10px 16px;
  }

  .button-text {
    display: none;
  }
}
</style>

