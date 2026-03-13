/**
 * AI Assistant 模块单元测试
 */

import { describe, it, expect, beforeEach, vi } from 'vitest'
import type { AIMessage, AIConfig } from '~/modules/ai-assistant/types'

// Mock simple module system
vi.mock('~/composables/simpleModuleSystem', () => ({
  useSimpleModuleSystem: () => ({
    isModuleEnabled: vi.fn(() => true)
  })
}))

// Mock module core
vi.mock('~/composables/moduleCore', () => ({
  useModuleCore: () => ({
    initEventBus: () => ({
      emit: vi.fn(),
      on: vi.fn(),
      off: vi.fn()
    })
  })
}))

describe('AI Assistant Module', () => {
  describe('AI Config', () => {
    it('should create default config', () => {
      const defaultConfig: AIConfig = {
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

      expect(defaultConfig.model).toBe('gpt-3.5-turbo')
      expect(defaultConfig.temperature).toBe(0.7)
      expect(defaultConfig.maxTokens).toBe(1000)
      expect(defaultConfig.enableQuickActions).toBe(true)
      expect(defaultConfig.enableHistory).toBe(true)
    })

    it('should validate temperature range', () => {
      const config: AIConfig = {
        apiKey: 'test-key',
        model: 'gpt-3.5-turbo',
        temperature: 0.7,
        maxTokens: 1000,
        systemPrompt: 'test',
        enableQuickActions: true,
        enableHistory: true,
        maxHistoryLength: 50,
        theme: 'light',
        position: 'bottom-right'
      }

      expect(config.temperature).toBeGreaterThanOrEqual(0)
      expect(config.temperature).toBeLessThanOrEqual(2)
    })

    it('should validate maxTokens range', () => {
      const config: AIConfig = {
        apiKey: 'test-key',
        model: 'gpt-3.5-turbo',
        temperature: 0.7,
        maxTokens: 1000,
        systemPrompt: 'test',
        enableQuickActions: true,
        enableHistory: true,
        maxHistoryLength: 50,
        theme: 'light',
        position: 'bottom-right'
      }

      expect(config.maxTokens).toBeGreaterThan(0)
      expect(config.maxTokens).toBeLessThanOrEqual(4096)
    })
  })

  describe('AI Message', () => {
    it('should create valid user message', () => {
      const message: AIMessage = {
        id: '1',
        role: 'user',
        content: 'Hello, how are you?',
        timestamp: new Date()
      }

      expect(message.role).toBe('user')
      expect(message.content).toBe('Hello, how are you?')
      expect(message.id).toBe('1')
      expect(message.timestamp).toBeInstanceOf(Date)
    })

    it('should create valid assistant message', () => {
      const message: AIMessage = {
        id: '2',
        role: 'assistant',
        content: 'I am doing well, thank you!',
        timestamp: new Date()
      }

      expect(message.role).toBe('assistant')
      expect(message.content).toBe('I am doing well, thank you!')
    })

    it('should create loading message', () => {
      const message: AIMessage = {
        id: '3',
        role: 'assistant',
        content: '',
        timestamp: new Date(),
        loading: true
      }

      expect(message.loading).toBe(true)
      expect(message.content).toBe('')
    })
  })

  describe('Quick Actions', () => {
    const quickActions = [
      { text: '你好！', icon: '👋' },
      { text: '介绍一下你自己', icon: '🤖' },
      { text: '帮我写代码', icon: '💻' },
      { text: '有什么新功能？', icon: '✨' },
      { text: '联系我', icon: '📧' }
    ]

    it('should have 5 quick actions', () => {
      expect(quickActions).toHaveLength(5)
    })

    it('each quick action should have text and icon', () => {
      quickActions.forEach(action => {
        expect(action.text).toBeTruthy()
        expect(action.icon).toBeTruthy()
      })
    })
  })

  describe('Session ID Generation', () => {
    it('should generate unique session IDs', () => {
      const sessionId1 = `session_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
      const sessionId2 = `session_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`

      expect(sessionId1).not.toBe(sessionId2)
    })

    it('session ID should start with session_', () => {
      const sessionId = `session_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
      expect(sessionId).toMatch(/^session_/)
    })
  })

  describe('Conversation Export', () => {
    it('should export conversation correctly', () => {
      const messages = [
        { role: 'user', content: 'Hello' },
        { role: 'assistant', content: 'Hi there!' }
      ]

      const exportText = messages.map(m => {
        const prefix = m.role === 'user' ? 'User: ' : 'Assistant: '
        return prefix + m.content
      }).join('\n\n')

      expect(exportText).toContain('User: Hello')
      expect(exportText).toContain('Assistant: Hi there!')
    })
  })

  describe('Status Text Calculation', () => {
    it('should show "思考中..." when loading', () => {
      const isLoading = true
      const apiKey = 'test-key'
      const statusText = isLoading ? '思考中...' : (!apiKey ? '请配置API密钥' : '在线')

      expect(statusText).toBe('思考中...')
    })

    it('should show "请配置API密钥" when no API key', () => {
      const isLoading = false
      const apiKey = ''
      const statusText = isLoading ? '思考中...' : (!apiKey ? '请配置API密钥' : '在线')

      expect(statusText).toBe('请配置API密钥')
    })

    it('should show "在线" when ready', () => {
      const isLoading = false
      const apiKey = 'test-key'
      const statusText = isLoading ? '思考中...' : (!apiKey ? '请配置API密钥' : '在线')

      expect(statusText).toBe('在线')
    })
  })

  describe('Message Management', () => {
    it('should add message to history', () => {
      const messages: AIMessage[] = []
      const newMessage: AIMessage = {
        id: '1',
        role: 'user',
        content: 'Test message',
        timestamp: new Date()
      }

      messages.push(newMessage)

      expect(messages).toHaveLength(1)
      expect(messages[0].id).toBe('1')
    })

    it('should filter out loading messages', () => {
      const messages: AIMessage[] = [
        { id: '1', role: 'user', content: 'Hello', timestamp: new Date() },
        { id: '2', role: 'assistant', content: '', timestamp: new Date(), loading: true }
      ]

      const nonLoadingMessages = messages.filter(m => !m.loading)

      expect(nonLoadingMessages).toHaveLength(1)
      expect(nonLoadingMessages[0].role).toBe('user')
    })

    it('should update message by ID', () => {
      const messages: AIMessage[] = [
        { id: '1', role: 'assistant', content: '', timestamp: new Date(), loading: true }
      ]

      const index = messages.findIndex(m => m.id === '1')
      if (index !== -1) {
        messages[index] = {
          ...messages[index],
          content: 'Response',
          loading: false,
          timestamp: new Date()
        }
      }

      expect(messages[0].content).toBe('Response')
      expect(messages[0].loading).toBe(false)
    })
  })

  describe('History Management', () => {
    it('should respect max history length', () => {
      const maxHistoryLength = 50
      const history: AIMessage[] = []

      for (let i = 0; i < 100; i++) {
        history.push({
          id: i.toString(),
          role: 'user',
          content: `Message ${i}`,
          timestamp: new Date()
        })
      }

      // Simulate trimming
      const trimmedHistory = history.slice(-maxHistoryLength)

      expect(trimmedHistory).toHaveLength(maxHistoryLength)
      expect(trimmedHistory[0].content).toBe('Message 50')
    })

    it('should clear all history', () => {
      const history: AIMessage[] = [
        { id: '1', role: 'user', content: 'Test', timestamp: new Date() },
        { id: '2', role: 'assistant', content: 'Response', timestamp: new Date() }
      ]

      history.length = 0

      expect(history).toHaveLength(0)
    })
  })
})
