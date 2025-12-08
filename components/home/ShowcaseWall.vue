<template>
  <section class="showcase-wall">
    <div class="showcase-wall-container">
      <div class="showcase-wall-header">
        <h2 class="showcase-wall-title">展示墙</h2>
        <p class="showcase-wall-subtitle">
          时间胶囊与访客留言实时滚动
        </p>
      </div>

      <div class="showcase-wall-content" ref="wallRef">
        <div class="showcase-wall-track" v-for="track in tracks" :key="track" :style="getTrackStyle(track)">
          <div
            v-for="(item, index) in getItemsForTrack(track)"
            :key="`${item.type}-${item.id || index}`"
            class="showcase-wall-item"
            :class="`showcase-wall-item--${item.type}`"
            :style="getItemStyle(item, track, index)"
          >
            <div class="showcase-wall-item-inner" :class="`showcase-wall-item-inner--${item.type}`">
              <div class="showcase-wall-item-icon" :class="`showcase-wall-item-icon--${item.type}`">
                <i :class="getItemIcon(item.type)"></i>
              </div>
              <div class="showcase-wall-item-content">
                <span class="showcase-wall-item-visitor" v-if="item.visitorName || item.visitorId">
                  @{{ item.visitorName || item.visitorId || '访客' }}
                </span>
                <span class="showcase-wall-item-label" :class="`showcase-wall-item-label--${item.type}`">
                  {{ item.type === 'capsule' ? '时间胶囊' : '留言' }}
                </span>
                <span class="showcase-wall-item-text">{{ item.content }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-if="allItems.length === 0" class="showcase-wall-empty">
        <i class="fas fa-inbox showcase-wall-empty-icon"></i>
        <p class="showcase-wall-empty-text">暂无内容</p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import type { TimeCapsule, TimeCapsuleListResponse } from '~/types/api'

const api = useApi()
const wallRef = ref<HTMLElement | null>(null)

interface ShowcaseItem {
  id: number | string
  type: 'capsule' | 'message'
  content: string
  visitorId?: string
  visitorName?: string
  createdAt?: string
  emoji?: string
  color?: string
}

const capsules = ref<TimeCapsule[]>([])
const messages = ref<any[]>([])
const allItems = ref<ShowcaseItem[]>([])
const tracks = ref([1, 2, 3, 4, 5, 6]) // 6条轨道

// 获取时间胶囊数据
const fetchCapsules = async () => {
  try {
    const res = await api.get<TimeCapsuleListResponse>('/TimeCapsule', {
      params: {
        page: 1,
        pageSize: 50
      }
    })
    
    if (import.meta.env.DEV) {
      console.log('[ShowcaseWall] TimeCapsule API Response:', res)
    }
    
    if (res) {
      let list: TimeCapsule[] = []
      
      // 处理不同的响应格式
      if (res.List) {
        list = res.List
      } else if ((res as any).list) {
        list = (res as any).list
      } else if (Array.isArray(res)) {
        list = res
      } else if ((res as any).data) {
        // 如果响应被包装在 data 中
        const data = (res as any).data
        if (data.List) {
          list = data.List
        } else if (data.list) {
          list = data.list
        } else if (Array.isArray(data)) {
          list = data
        }
      }
      
      // /TimeCapsule 端点已经只返回 status == 1 的数据，不需要再过滤
      capsules.value = list
      
      if (import.meta.env.DEV) {
        console.log('[ShowcaseWall] Parsed capsules:', capsules.value.length)
      }
    } else {
      capsules.value = []
    }
  } catch (e) {
    if (import.meta.env.DEV) {
      console.error('[ShowcaseWall] Failed to fetch capsules:', e)
    }
    capsules.value = []
  }
}

// 获取留言板数据
const fetchMessages = async () => {
  try {
    const res = await api.get<any>('/VisitorInteraction/messages/approved', {
      params: {
        limit: 50
      }
    })
    
    if (res) {
      if (res.data) {
        messages.value = Array.isArray(res.data) ? res.data : []
      } else if (Array.isArray(res)) {
        messages.value = res
      } else {
        messages.value = []
      }
    } else {
      messages.value = []
    }
  } catch (e) {
    if (import.meta.env.DEV) {
      console.error('Failed to fetch messages:', e)
    }
    messages.value = []
  }
}

// 合并数据
const mergeItems = () => {
  const items: ShowcaseItem[] = []
  
  // 添加时间胶囊
  capsules.value.forEach(capsule => {
    if (capsule && capsule.content) {
      items.push({
        id: `capsule-${capsule.id}`,
        type: 'capsule',
        content: capsule.content,
        visitorId: capsule.visitorId,
        visitorName: capsule.visitorName,
        createdAt: capsule.createdAt
      })
    }
  })
  
  // 添加留言
  messages.value.forEach(message => {
    if (message && message.content) {
      items.push({
        id: `message-${message.id}`,
        type: 'message',
        content: message.content,
        visitorId: message.visitorId,
        visitorName: message.visitorId, // 留言可能没有visitorName
        createdAt: message.createdAt,
        emoji: message.emoji,
        color: message.color
      })
    }
  })
  
  if (import.meta.env.DEV) {
    console.log('[ShowcaseWall] Merged items:', items.length, {
      capsules: capsules.value.length,
      messages: messages.value.length
    })
  }
  
  // 随机打乱顺序
  allItems.value = items.sort(() => Math.random() - 0.5)
}

// 获取类型图标
const getItemIcon = (type: string) => {
  return type === 'capsule' ? 'fas fa-clock' : 'fas fa-comment'
}

// 获取轨道样式
const getTrackStyle = (track: number) => {
  const height = 100 / tracks.value.length
  return {
    top: `${(track - 1) * height}%`,
    height: `${height}%`
  }
}

// 获取该轨道上的项目
const getItemsForTrack = (track: number) => {
  const itemsPerTrack = Math.ceil(allItems.value.length / tracks.value.length)
  const startIndex = (track - 1) * itemsPerTrack
  const endIndex = startIndex + itemsPerTrack
  return allItems.value.slice(startIndex, endIndex)
}

// 获取项目样式
const getItemStyle = (item: ShowcaseItem, track: number, index: number) => {
  const baseDelay = index * 2 // 每个项目间隔2秒
  const duration = 20 + Math.random() * 10 // 20-30秒滚动时间
  const delay = baseDelay + Math.random() * 2 // 添加随机延迟
  
  return {
    '--animation-duration': `${duration}s`,
    '--animation-delay': `${delay}s`,
    right: `${-300 - Math.random() * 200}px`, // 从右侧外部开始
    color: item.color || undefined
  } as any
}

// 加载数据
const loadData = async () => {
  await Promise.all([fetchCapsules(), fetchMessages()])
  mergeItems()
}

onMounted(() => {
  loadData()
  
  // 定期刷新数据
  const interval = setInterval(() => {
    loadData()
  }, 60000) // 每分钟刷新一次
  
  onUnmounted(() => {
    clearInterval(interval)
  })
})
</script>

<style scoped>
/* 样式已移至 assets/css/home.css */
</style>
