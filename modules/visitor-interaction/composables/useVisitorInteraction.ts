/**
 * Visitor Interaction Composable
 * 访客互动功能的核心逻辑
 */

import type { InteractionMessage, VisitorLevel } from '~/types/visitor'

// 互动消息类型
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

// 访客等级
interface VisitorLevel {
  level: number
  name: string
  color: string
  privileges: string[]
}

// 互动配置
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

// 快捷消息
const quickMessages = [
  { text: '你好！', type: 'greet', icon: '👋' },
  { text: '厉害！', type: 'praise', icon: '👍' },
  { text: '哈哈哈', type: 'laugh', icon: '😂' },
  { text: '加油！', type: 'encourage', icon: '💪' },
  { text: '666', type: 'cool', icon: '🔥' }
]

// 等级系统
const levels: VisitorLevel[] = [
  { level: 1, name: '访客', color: '#9CA3AF', privileges: ['查看信息'] },
  { level: 2, name: '新秀', color: '#60A5FA', privileges: ['发送弹幕'] },
  { level: 3, name: '活跃', color: '#34D399', privileges: ['自定义头像'] },
  { level: 4, name: '精英', color: '#F59E0B', privileges: ['高级互动'] },
  { level: 5, name: '大佬', color: '#EF4444', privileges: ['VIP特权'] }
]

/**
 * 访客互动Composable
 */
export const useVisitorInteraction = () => {
  const isOpen = ref(false)
  const messages = ref<InteractionMessage[]>([])
  const config = ref<InteractionConfig>(getDefaultConfig())
  const visitorLevel = ref<VisitorLevel>(levels[0])
  const { isModuleEnabled } = useModuleSystem()

  // 获取默认配置
  function getDefaultConfig(): InteractionConfig {
    return {
      enableDanmaku: true,
      enableFootprintMap: true,
      enableInteractionPanel: true,
      danmakuFrequency: 5000,
      maxDanmakuCount: 50,
      enableMapAnimation: true,
      panelPosition: 'right',
      enableSound: true
    }
  }

  // 初始化配置
  onMounted(async () => {
    await loadConfig()
    initializeWebSocket()
  })

  // 加载配置
  async function loadConfig() {
    try {
      const moduleConfig = await getModuleConfig('visitor-interaction')
      if (moduleConfig) {
        config.value = { ...getDefaultConfig(), ...moduleConfig }
      }
    } catch (e) {
      console.error('Failed to load visitor interaction config:', e)
    }
  }

  // 保存配置
  async function saveConfig() {
    try {
      await setModuleConfig('visitor-interaction', 'config', config.value)
    } catch (e) {
      console.error('Failed to save visitor interaction config:', e)
      throw e
    }
  }

  // 初始化WebSocket连接
  function initializeWebSocket() {
    // 模拟WebSocket连接
    // 实际项目中应该使用真实的WebSocket
    setInterval(() => {
      if (config.value.enableDanmaku && Math.random() > 0.7) {
        generateRandomMessage()
      }
    }, config.value.danmakuFrequency)
  }

  // 生成随机消息
  function generateRandomMessage() {
    const names = ['访客小明', '游客小红', '路人甲', '匿名用户']
    const contents = [
      '这个网站真不错！',
      '界面设计很美观',
      '功能很强大',
      '学到了很多',
      '继续加油！'
    ]

    const message: InteractionMessage = {
      id: Date.now().toString(),
      type: 'danmaku',
      content: contents[Math.floor(Math.random() * contents.length)],
      author: names[Math.floor(Math.random() * names.length)],
      timestamp: new Date(),
      color: getRandomColor(),
      level: Math.floor(Math.random() * 5) + 1
    }

    addMessage(message)
  }

  // 添加消息
  function addMessage(message: InteractionMessage) {
    // 限制消息数量
    if (messages.value.length >= config.value.maxDanmakuCount) {
      messages.value.shift()
    }

    messages.value.push(message)

    // 触发消息事件
    const bus = useModuleCore().initEventBus()
    bus.emit('visitor-interaction:message', {
      type: message.type,
      message
    })

    // 播放音效
    if (config.value.enableSound) {
      playSound(message.type)
    }
  }

  // 发送弹幕
  function sendDanmaku(content: string, author: string = '匿名用户', level: number = 1) {
    if (!config.value.enableDanmaku) return

    const message: InteractionMessage = {
      id: Date.now().toString(),
      type: 'danmaku',
      content,
      author,
      timestamp: new Date(),
      color: getLevelColor(level),
      level
    }

    addMessage(message)
  }

  // 发送气泡
  function sendBubble(content: string, author: string = '匿名用户') {
    const message: InteractionMessage = {
      id: Date.now().toString(),
      type: 'bubble',
      content,
      author,
      timestamp: new Date()
    }

    addMessage(message)
  }

  // 更新访客等级
  function updateVisitorLevel(level: number) {
    const levelInfo = levels.find(l => l.level === level)
    if (levelInfo) {
      visitorLevel.value = levelInfo
    }
  }

  // 获取等级信息
  function getLevelInfo(level: number): VisitorLevel | undefined {
    return levels.find(l => l.level === level)
  }

  // 获取当前等级进度
  function getLevelProgress(currentLevel: number, currentExp: number): {
    progress: number
    nextLevelExp: number
    maxLevelExp: number
  } {
    // 简化的经验值计算
    const baseExp = 100
    const expMultiplier = 1.5
    const maxLevel = 5

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

  // 工具函数
  function getRandomColor(): string {
    const colors = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6', '#EC4899']
    return colors[Math.floor(Math.random() * colors.length)]
  }

  function getLevelColor(level: number): string {
    const levelInfo = getLevelInfo(level)
    return levelInfo?.color || '#9CA3AF'
  }

  function playSound(type: string) {
    // 实际项目中应该播放音效
    console.log(`Playing sound for ${type}`)
  }

  // 监听配置变化
  watch(config, () => {
    saveConfig()
  }, { deep: true })

  return {
    // 状态
    isOpen,
    messages,
    config,
    visitorLevel,
    quickMessages,
    levels,

    // 方法
    toggle: () => { isOpen.value = !isOpen.value },
    sendDanmaku,
    sendBubble,
    updateVisitorLevel,
    getLevelInfo,
    getLevelProgress,
    addMessage,

    // 工具
    getRandomColor,
    getLevelColor
  }
}

/**
 * 访客互动工具函数
 */
export const useVisitorInteractionTools = () => {
  /**
   * 获取访客统计信息
   */
  async function getVisitorStats(): Promise<{
    totalVisitors: number
    onlineVisitors: number
    todayVisitors: number
    messageCount: number
  }> {
    try {
      const response = await $fetch('/api/visitor/stats')
      return response.data
    } catch (e) {
      console.error('Failed to fetch visitor stats:', e)
      return {
        totalVisitors: 0,
        onlineVisitors: 0,
        todayVisitors: 0,
        messageCount: 0
      }
    }
  }

  /**
   * 获取热门消息
   */
  async function getHotMessages(limit = 10): Promise<InteractionMessage[]> {
    try {
      const response = await $fetch('/api/visitor/hot-messages', {
        params: { limit }
      })
      return response.messages || []
    } catch (e) {
      console.error('Failed to fetch hot messages:', e)
      return []
    }
  }

  /**
   * 导出互动数据
   */
  function exportInteraction(messages: InteractionMessage[]): string {
    const data = messages.map(m => ({
      type: m.type,
      content: m.content,
      author: m.author,
      timestamp: m.timestamp.toISOString(),
      level: m.level
    }))

    return JSON.stringify(data, null, 2)
  }

  return {
    getVisitorStats,
    getHotMessages,
    exportInteraction
  }
}