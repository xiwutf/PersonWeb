/**
 * AI Assistant Composable
 * AI助手功能的核心逻辑
 */

import type { AIMessage, AIConfig, QuickAction } from '~/modules/ai-assistant/types'
import { useSimpleModuleSystem } from '~/composables/simpleModuleSystem'

// 快捷操作
const quickActions: QuickAction[] = [
  { text: '你好！', icon: '👋' },
  { text: '介绍一下你自己', icon: '🤖' },
  { text: '帮我写代码', icon: '💻' },
  { text: '有什么新功能？', icon: '✨' },
  { text: '联系我', icon: '📧' }
]

/**
 * AI助手Composable
 */
export const useAIAssistant = () => {
  const isOpen = ref(false)
  const messages = ref<AIMessage[]>([])
  const isLoading = ref(false)
  const config = ref<AIConfig>(getDefaultConfig())
  const { isModuleEnabled } = useSimpleModuleSystem()

  // 获取默认配置
  function getDefaultConfig(): AIConfig {
    return {
      apiKey: '',
      model: 'gpt-3.5-turbo',
      temperature: 0.7,
      maxTokens: 1000,
      systemPrompt: '你是一个智能助手，能够帮助用户解决问题。请以友好、专业的语气回应。',
      enableQuickActions: true,
      enableHistory: true,
      maxHistoryLength: 50,
      theme: 'light',
      position: 'bottom-right'
    }
  }

  // 初始化配置
  onMounted(async () => {
    await loadConfig()
  })

  // 加载配置
  async function loadConfig() {
    try {
      const moduleConfig = await getModuleConfig('ai-assistant')
      if (moduleConfig) {
        config.value = { ...getDefaultConfig(), ...moduleConfig }
      }
    } catch (e) {
      console.error('Failed to load AI assistant config:', e)
    }
  }

  // 保存配置
  async function saveConfig() {
    try {
      await setModuleConfig('ai-assistant', 'config', config.value)
    } catch (e) {
      console.error('Failed to save AI assistant config:', e)
      throw e
    }
  }

  // 获取状态文本
  const statusText = computed(() => {
    if (isLoading.value) return '思考中...'
    if (!config.value.apiKey) return '请配置API密钥'
    return '在线'
  })

  // 切换助手状态
  function toggleAssistant() {
    isOpen.value = !isOpen.value
  }

  // 发送消息
  async function sendAIMessage(content: string) {
    if (!content.trim() || isLoading.value) return

    // 添加用户消息
    const userMessage: AIMessage = {
      id: Date.now().toString(),
      role: 'user',
      content: content,
      timestamp: new Date()
    }

    messages.value.push(userMessage)

    // 添加助手加载消息
    const loadingMessage: AIMessage = {
      id: (Date.now() + 1).toString(),
      role: 'assistant',
      content: '',
      timestamp: new Date(),
      loading: true
    }

    messages.value.push(loadingMessage)

    // 设置加载状态
    isLoading.value = true

    try {
      // 发送消息到AI服务
      const response = await fetchAIResponse(content)

      // 更新消息
      const index = messages.value.findIndex(m => m.id === loadingMessage.id)
      if (index !== -1) {
        messages.value[index] = {
          ...loadingMessage,
          content: response.content,
          loading: false,
          timestamp: new Date()
        }
      }

      // 保存对话历史
      if (config.value.enableHistory) {
        await saveConversationHistory(userMessage, {
          ...response,
          timestamp: new Date()
        })
      }

      // 触发消息事件
      const bus = useModuleCore().initEventBus()
      bus.emit('ai-assistant:message', {
        type: 'assistant',
        content: response.content
      })

    } catch (error) {
      console.error('AI response failed:', error)

      // 更新消息为错误状态
      const index = messages.value.findIndex(m => m.id === loadingMessage.id)
      if (index !== -1) {
        messages.value[index] = {
          ...loadingMessage,
          content: '抱歉，发生了错误。请稍后再试。',
          loading: false,
          timestamp: new Date()
        }
      }
    } finally {
      isLoading.value = false
    }
  }

  // 发送快捷消息
  function sendQuickMessage(text: string) {
    sendAIMessage(text)
  }

  // 获取AI响应
  async function fetchAIResponse(content: string): Promise<{ content: string; usage?: any }> {
    if (!config.value.apiKey) {
      throw new Error('API key is required')
    }

    // 构建消息历史
    const conversationHistory = [
      { role: 'system' as const, content: config.value.systemPrompt },
      ...messages.value
        .filter(m => !m.loading)
        .map(m => ({ role: m.role, content: m.content }))
    ]

    // 调用AI服务
    const response = await $fetch('/api/ai/chat', {
      method: 'POST',
      body: {
        messages: conversationHistory,
        model: config.value.model,
        temperature: config.value.temperature,
        max_tokens: config.value.maxTokens
      }
    })

    return {
      content: response.choices[0]?.message?.content || '抱歉，我没有收到有效的回复。',
      usage: response.usage
    }
  }

  // 保存对话历史
  async function saveConversationHistory(userMessage: AIMessage, assistantMessage: AIMessage) {
    try {
      const sessionId = generateSessionId()

      await $fetch('/api/ai/conversations', {
        method: 'POST',
        body: {
          sessionId,
          messages: [
            {
              role: userMessage.role,
              content: userMessage.content,
              timestamp: userMessage.timestamp
            },
            {
              role: assistantMessage.role,
              content: assistantMessage.content,
              timestamp: assistantMessage.timestamp
            }
          ]
        }
      })
    } catch (e) {
      console.error('Failed to save conversation history:', e)
    }
  }

  // 生成会话ID
  function generateSessionId(): string {
    return `session_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
  }

  // 获取对话历史
  async function getConversationHistory(sessionId?: string) {
    try {
      if (!config.value.enableHistory) return []

      const params = sessionId ? { sessionId } : {}
      const response = await $fetch('/api/ai/conversations', {
        params
      })

      return response.messages || []
    } catch (e) {
      console.error('Failed to fetch conversation history:', e)
      return []
    }
  }

  // 清空对话历史
  async function clearConversationHistory() {
    try {
      await $fetch('/api/ai/conversations', {
        method: 'DELETE'
      })
      messages.value = []
    } catch (e) {
      console.error('Failed to clear conversation history:', e)
    }
  }

  // 自动滚动到底部
  const messagesRef = ref<HTMLElement>()
  const autoScroll = () => {
    nextTick(() => {
      if (messagesRef.value) {
        messagesRef.value.scrollTop = messagesRef.value.scrollHeight
      }
    })
  }

  // 监听消息变化
  watch(messages, () => {
    autoScroll()
  })

  // 监听配置变化
  watch(config, () => {
    saveConfig()
  }, { deep: true })

  return {
    // 状态
    isOpen,
    messages,
    isLoading,
    config,
    statusText,
    quickActions,

    // 方法
    toggleAssistant,
    sendAIMessage,
    sendQuickMessage,
    loadConfig,
    saveConfig,
    getConversationHistory,
    clearConversationHistory,

    // 工具
    messagesRef
  }
}

/**
 * AI助手工具函数
 */
export const useAIAssistantTools = () => {
  /**
   * 检查API密钥是否有效
   */
  async function validateApiKey(apiKey: string): Promise<boolean> {
    try {
      const response = await $fetch('/api/ai/validate-key', {
        method: 'POST',
        body: { apiKey }
      })
      return response.valid
    } catch (e) {
      return false
    }
  }

  /**
   * 获取模型列表
   */
  async function getModels(): Promise<Array<{ id: string; name: string; description: string }>> {
    try {
      const response = await $fetch('/api/ai/models')
      return response.models || []
    } catch (e) {
      console.error('Failed to fetch models:', e)
      return []
    }
  }

  /**
   * 导出对话历史
   */
  function exportConversation(messages: Array<{ role: string; content: string }>): string {
    const conversation = messages.map(m => {
      const prefix = m.role === 'user' ? 'User: ' : 'Assistant: '
      return prefix + m.content
    }).join('\n\n')

    return `AI Assistant Conversation\n\n${conversation}\n\nExported on: ${new Date().toLocaleString()}`
  }

  /**
   * 复制消息到剪贴板
   */
  async function copyToClipboard(text: string): Promise<boolean> {
    try {
      await navigator.clipboard.writeText(text)
      return true
    } catch (e) {
      console.error('Failed to copy to clipboard:', e)
      return false
    }
  }

  return {
    validateApiKey,
    getModels,
    exportConversation,
    copyToClipboard
  }
}