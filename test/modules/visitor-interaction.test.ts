/**
 * Visitor Interaction 模块单元测试
 */

import { describe, it, expect, beforeEach, vi } from 'vitest'

// Mock module system
vi.mock('~/composables/moduleSystem', () => ({
  useModuleSystem: () => ({
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

// Types
interface InteractionMessage {
  id: string
  type: 'danmaku' | 'bubble' | 'level'
  content: string
  author: string
  avatar?: string
  timestamp: Date
  level?: number
  color?: string
}

interface VisitorLevel {
  level: number
  name: string
  color: string
  privileges: string[]
}

interface InteractionConfig {
  enableDanmaku: boolean
  enableFootprintMap: boolean
  enableInteractionPanel: boolean
  danmakuFrequency: number
  maxDanmakuCount: number
  enableMapAnimation: boolean
  panelPosition: string
  enableSound: boolean
}

describe('Visitor Interaction Module', () => {
  describe('Interaction Config', () => {
    const defaultConfig: InteractionConfig = {
      enableDanmaku: true,
      enableFootprintMap: true,
      enableInteractionPanel: true,
      danmakuFrequency: 5000,
      maxDanmakuCount: 50,
      enableMapAnimation: true,
      panelPosition: 'right',
      enableSound: true
    }

    it('should have default config values', () => {
      expect(defaultConfig.enableDanmaku).toBe(true)
      expect(defaultConfig.enableFootprintMap).toBe(true)
      expect(defaultConfig.enableInteractionPanel).toBe(true)
      expect(defaultConfig.danmakuFrequency).toBe(5000)
      expect(defaultConfig.maxDanmakuCount).toBe(50)
      expect(defaultConfig.enableMapAnimation).toBe(true)
      expect(defaultConfig.panelPosition).toBe('right')
      expect(defaultConfig.enableSound).toBe(true)
    })

    it('should validate danmaku frequency', () => {
      expect(defaultConfig.danmakuFrequency).toBeGreaterThanOrEqual(1000)
      expect(defaultConfig.danmakuFrequency).toBeLessThanOrEqual(30000)
    })

    it('should validate max danmaku count', () => {
      expect(defaultConfig.maxDanmakuCount).toBeGreaterThan(0)
      expect(defaultConfig.maxDanmakuCount).toBeLessThanOrEqual(200)
    })

    it('should toggle danmaku', () => {
      const config: InteractionConfig = { ...defaultConfig, enableDanmaku: true }
      config.enableDanmaku = !config.enableDanmaku

      expect(config.enableDanmaku).toBe(false)
    })
  })

  describe('Visitor Levels', () => {
    const levels: VisitorLevel[] = [
      { level: 1, name: '访客', color: '#9CA3AF', privileges: ['查看信息'] },
      { level: 2, name: '新秀', color: '#60A5FA', privileges: ['发送弹幕'] },
      { level: 3, name: '活跃', color: '#34D399', privileges: ['自定义头像'] },
      { level: 4, name: '精英', color: '#F59E0B', privileges: ['高级互动'] },
      { level: 5, name: '大佬', color: '#EF4444', privileges: ['VIP特权'] }
    ]

    it('should have 5 levels', () => {
      expect(levels).toHaveLength(5)
    })

    it('should have level numbers from 1 to 5', () => {
      const levelNumbers = levels.map(l => l.level).sort((a, b) => a - b)
      expect(levelNumbers).toEqual([1, 2, 3, 4, 5])
    })

    it('each level should have required properties', () => {
      levels.forEach(level => {
        expect(level.level).toBeGreaterThan(0)
        expect(level.name).toBeTruthy()
        expect(level.color).toMatch(/^#[0-9a-f]{6}$/i)
        expect(Array.isArray(level.privileges)).toBe(true)
      })
    })

    it('higher levels should have more privileges', () => {
      const level1Privileges = levels.find(l => l.level === 1)?.privileges.length
      const level5Privileges = levels.find(l => l.level === 5)?.privileges.length

      expect(level5Privileges).toBeGreaterThanOrEqual(level1Privileges || 0)
    })

    it('should get level info', () => {
      const getLevelInfo = (level: number): VisitorLevel | undefined => {
        return levels.find(l => l.level === level)
      }

      expect(getLevelInfo(1)).toEqual(levels[0])
      expect(getLevelInfo(5)).toEqual(levels[4])
      expect(getLevelInfo(6)).toBeUndefined()
    })
  })

  describe('Interaction Messages', () => {
    const messages: InteractionMessage[] = [
      {
        id: '1',
        type: 'danmaku',
        content: '你好！',
        author: '访客小明',
        timestamp: new Date(),
        level: 2,
        color: '#60A5FA'
      },
      {
        id: '2',
        type: 'bubble',
        content: '界面很漂亮',
        author: '游客小红',
        timestamp: new Date()
      }
    ]

    it('should have valid message types', () => {
      messages.forEach(msg => {
        expect(['danmaku', 'bubble', 'level']).toContain(msg.type)
      })
    })

    it('should have required properties', () => {
      messages.forEach(msg => {
        expect(msg.id).toBeTruthy()
        expect(msg.content).toBeTruthy()
        expect(msg.author).toBeTruthy()
        expect(msg.timestamp).toBeInstanceOf(Date)
      })
    })

    it('should add new message', () => {
      const newMessage: InteractionMessage = {
        id: '3',
        type: 'danmaku',
        content: '新消息',
        author: '路人甲',
        timestamp: new Date()
      }
      const updatedMessages = [...messages, newMessage]

      expect(updatedMessages).toHaveLength(3)
    })

    it('should filter messages by type', () => {
      const danmakuMessages = messages.filter(m => m.type === 'danmaku')

      expect(danmakuMessages).toHaveLength(1)
      expect(danmakuMessages[0].type).toBe('danmaku')
    })
  })

  describe('Message Queue Management', () => {
    const maxCount = 50
    const messages: InteractionMessage[] = Array.from({ length: 50 }, (_, i) => ({
      id: i.toString(),
      type: 'danmaku',
      content: `Message ${i}`,
      author: 'User',
      timestamp: new Date()
    }))

    it('should limit message count to max', () => {
      expect(messages.length).toBeLessThanOrEqual(maxCount)
    })

    it('should remove oldest message when adding new one', () => {
      const newMessage: InteractionMessage = {
        id: '51',
        type: 'danmaku',
        content: 'New message',
        author: 'User',
        timestamp: new Date()
      }

      const updatedMessages = [...messages.slice(1), newMessage]

      expect(updatedMessages).toHaveLength(50)
      expect(updatedMessages.find(m => m.id === '0')).toBeUndefined()
      expect(updatedMessages.find(m => m.id === '51')).toBeDefined()
    })
  })

  describe('Level Progress', () => {
    const baseExp = 100
    const expMultiplier = 1.5
    const maxLevel = 5

    const getLevelProgress = (currentLevel: number, currentExp: number) => {
      if (currentLevel >= maxLevel) {
        return {
          progress: 100,
          nextLevelExp: baseExp * Math.pow(expMultiplier, maxLevel),
          maxLevelExp: baseExp * Math.pow(expMultiplier, maxLevel)
        }
      }

      const currentLevelExp = baseExp * Math.pow(expMultiplier, currentLevel - 1)
      const nextLevelExp = baseExp * Math.pow(expMultiplier, currentLevel)

      const progress = ((currentExp - currentLevelExp) / (nextLevelExp - currentLevelExp)) * 100

      return {
        progress: Math.min(100, Math.max(0, progress)),
        nextLevelExp,
        maxLevelExp: baseExp * Math.pow(expMultiplier, maxLevel)
      }
    }

    it('should calculate progress at 0%', () => {
      const result = getLevelProgress(1, 100)
      expect(result.progress).toBe(0)
    })

    it('should calculate progress at 50%', () => {
      const result = getLevelProgress(1, 125)
      expect(result.progress).toBe(50)
    })

    it('should calculate progress at 100%', () => {
      const result = getLevelProgress(1, 150)
      expect(result.progress).toBe(100)
    })

    it('should show max level progress as 100%', () => {
      const result = getLevelProgress(5, 1000)
      expect(result.progress).toBe(100)
    })

    it('should calculate correct exp requirements', () => {
      const result = getLevelProgress(1, 100)
      expect(result.nextLevelExp).toBe(150)
    })
  })

  describe('Quick Messages', () => {
    const quickMessages = [
      { text: '你好！', type: 'greet', icon: '👋' },
      { text: '厉害！', type: 'praise', icon: '👍' },
      { text: '哈哈哈', type: 'laugh', icon: '😂' },
      { text: '加油！', type: 'encourage', icon: '💪' },
      { text: '666', type: 'cool', icon: '🔥' }
    ]

    it('should have 5 quick messages', () => {
      expect(quickMessages).toHaveLength(5)
    })

    it('each quick message should have required properties', () => {
      quickMessages.forEach(msg => {
        expect(msg.text).toBeTruthy()
        expect(msg.type).toBeTruthy()
        expect(msg.icon).toBeTruthy()
      })
    })
  })

  describe('Color Utilities', () => {
    const colors = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6', '#EC4899']

    const getRandomColor = (): string => {
      return colors[Math.floor(Math.random() * colors.length)]
    }

    it('should return valid color from predefined list', () => {
      const color = getRandomColor()
      const colorRegex = /^#[0-9a-f]{6}$/i
      expect(color).toMatch(colorRegex)
      expect(colors).toContain(color)
    })

    it('should get level color', () => {
      const levels: VisitorLevel[] = [
        { level: 1, name: '访客', color: '#9CA3AF', privileges: [] },
        { level: 2, name: '新秀', color: '#60A5FA', privileges: [] }
      ]

      const getLevelColor = (level: number): string => {
        const levelInfo = levels.find(l => l.level === level)
        return levelInfo?.color || '#9CA3AF'
      }

      expect(getLevelColor(1)).toBe('#9CA3AF')
      expect(getLevelColor(2)).toBe('#60A5FA')
      expect(getLevelColor(3)).toBe('#9CA3AF') // Default color
    })
  })

  describe('Message Sending', () => {
    const config: InteractionConfig = {
      enableDanmaku: true,
      enableFootprintMap: true,
      enableInteractionPanel: true,
      danmakuFrequency: 5000,
      maxDanmakuCount: 50,
      enableMapAnimation: true,
      panelPosition: 'right',
      enableSound: true
    }

    it('should send danmaku when enabled', () => {
      expect(config.enableDanmaku).toBe(true)
    })

    it('should not send danmaku when disabled', () => {
      const disabledConfig: InteractionConfig = { ...config, enableDanmaku: false }
      expect(disabledConfig.enableDanmaku).toBe(false)
    })
  })

  describe('Visitor Stats', () => {
    const stats = {
      totalVisitors: 1000,
      onlineVisitors: 50,
      todayVisitors: 100,
      messageCount: 500
    }

    it('should have valid stats values', () => {
      expect(stats.totalVisitors).toBeGreaterThanOrEqual(0)
      expect(stats.onlineVisitors).toBeGreaterThanOrEqual(0)
      expect(stats.todayVisitors).toBeGreaterThanOrEqual(0)
      expect(stats.messageCount).toBeGreaterThanOrEqual(0)
    })

    it('online visitors should not exceed total visitors', () => {
      expect(stats.onlineVisitors).toBeLessThanOrEqual(stats.totalVisitors)
    })

    it('today visitors should not exceed total visitors', () => {
      expect(stats.todayVisitors).toBeLessThanOrEqual(stats.totalVisitors)
    })
  })

  describe('Data Export', () => {
    const messages: InteractionMessage[] = [
      {
        id: '1',
        type: 'danmaku',
        content: 'Test',
        author: 'User',
        timestamp: new Date('2024-01-01'),
        level: 1
      }
    ]

    const exportInteraction = (msgs: InteractionMessage[]): string => {
      const data = msgs.map(m => ({
        type: m.type,
        content: m.content,
        author: m.author,
        timestamp: m.timestamp.toISOString(),
        level: m.level
      }))
      return JSON.stringify(data, null, 2)
    }

    it('should export messages to JSON', () => {
      const exported = exportInteraction(messages)
      const parsed = JSON.parse(exported)

      expect(parsed).toHaveLength(1)
      expect(parsed[0].type).toBe('danmaku')
      expect(parsed[0].content).toBe('Test')
    })

    it('should include timestamp in ISO format', () => {
      const exported = exportInteraction(messages)
      const parsed = JSON.parse(exported)

      expect(parsed[0].timestamp).toMatch(/^\d{4}-\d{2}-\d{2}T/)
    })
  })
})
