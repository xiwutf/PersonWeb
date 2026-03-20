<template>
  <section class="showcase-wall">
    <div class="showcase-wall-container">
      <div class="showcase-wall-header">
        <h2 class="showcase-wall-title">展示墙</h2>
        <p class="showcase-wall-subtitle">
          时间胶囊与访客留言实时滚动
        </p>
      </div>

      <div ref="wallRef" class="showcase-wall-content">
        <div
          v-for="track in tracks"
          :key="track"
          class="showcase-wall-track"
          :style="getTrackStyle(track)"
        >
          <div
            v-for="(item, index) in getItemsForTrack(track)"
            :key="`${item.type}-${item.id || index}`"
            class="showcase-wall-item"
            :class="`showcase-wall-item--${item.type}`"
            :style="getItemStyle(item, index)"
          >
            <div class="showcase-wall-item-inner" :class="`showcase-wall-item-inner--${item.type}`">
              <div class="showcase-wall-item-icon" :class="`showcase-wall-item-icon--${item.type}`">
                <i :class="getItemIcon(item.type)"></i>
              </div>
              <div class="showcase-wall-item-content">
                <span v-if="item.visitorName || item.visitorId" class="showcase-wall-item-visitor">
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

      <div v-if="allItems.length === 0" class="showcase-wall-empty">
        <i class="fas fa-inbox showcase-wall-empty-icon"></i>
        <p class="showcase-wall-empty-text">暂无内容</p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
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
const tracks = ref([1, 2, 3, 4, 5, 6])

let refreshTimer: NodeJS.Timeout | null = null
let visibilityHandler: (() => void) | null = null

const fetchCapsules = async () => {
  try {
    const res = await api.get<TimeCapsuleListResponse>('/TimeCapsule', {
      params: {
        page: 1,
        pageSize: 50
      }
    })

    if (!res) {
      capsules.value = []
      return
    }

    let list: TimeCapsule[] = []

    if (res.List) {
      list = res.List
    } else if ((res as any).list) {
      list = (res as any).list
    } else if (Array.isArray(res)) {
      list = res
    } else if ((res as any).data) {
      const data = (res as any).data
      if (data.List) {
        list = data.List
      } else if (data.list) {
        list = data.list
      } else if (Array.isArray(data)) {
        list = data
      }
    }

    capsules.value = list
  } catch {
    capsules.value = []
  }
}

const fetchMessages = async () => {
  try {
    const res = await api.get<any>('/VisitorInteraction/messages/approved', {
      params: {
        limit: 50
      }
    })

    if (!res) {
      messages.value = []
      return
    }

    if (res.data) {
      messages.value = Array.isArray(res.data) ? res.data : []
      return
    }

    messages.value = Array.isArray(res) ? res : []
  } catch (e) {
    if (import.meta.env.DEV) {
      console.error('Failed to fetch messages:', e)
    }
    messages.value = []
  }
}

const mergeItems = () => {
  const items: ShowcaseItem[] = []

  capsules.value.forEach(capsule => {
    if (!capsule?.content) return

    items.push({
      id: `capsule-${capsule.id}`,
      type: 'capsule',
      content: capsule.content,
      visitorId: capsule.visitorId,
      visitorName: capsule.visitorName,
      createdAt: capsule.createdAt
    })
  })

  messages.value.forEach(message => {
    if (!message?.content) return

    items.push({
      id: `message-${message.id}`,
      type: 'message',
      content: message.content,
      visitorId: message.visitorId,
      visitorName: message.visitorName || message.visitorId,
      createdAt: message.createdAt,
      emoji: message.emoji,
      color: message.color
    })
  })

  allItems.value = items.sort(() => Math.random() - 0.5)
}

const getItemIcon = (type: string) => {
  return type === 'capsule' ? 'fas fa-clock' : 'fas fa-comment'
}

const getTrackStyle = (track: number) => {
  const height = 100 / tracks.value.length
  return {
    top: `${(track - 1) * height}%`,
    height: `${height}%`
  }
}

const getItemsForTrack = (track: number) => {
  const itemsPerTrack = Math.ceil(allItems.value.length / tracks.value.length)
  const startIndex = (track - 1) * itemsPerTrack
  return allItems.value.slice(startIndex, startIndex + itemsPerTrack)
}

const getItemStyle = (item: ShowcaseItem, index: number) => {
  const baseDelay = index * 2
  const duration = 20 + Math.random() * 10
  const delay = baseDelay + Math.random() * 2

  return {
    '--animation-duration': `${duration}s`,
    '--animation-delay': `${delay}s`,
    right: `${-300 - Math.random() * 200}px`,
    color: item.color || undefined
  } as Record<string, string | undefined>
}

const loadData = async () => {
  await Promise.all([fetchCapsules(), fetchMessages()])
  mergeItems()
}

const startRefresh = () => {
  if (refreshTimer) return

  refreshTimer = setInterval(() => {
    if (document.hidden) return
    loadData()
  }, 60000)
}

const stopRefresh = () => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
    refreshTimer = null
  }
}

onMounted(() => {
  loadData()
  startRefresh()

  visibilityHandler = () => {
    if (document.hidden) {
      stopRefresh()
      return
    }

    loadData()
    startRefresh()
  }

  document.addEventListener('visibilitychange', visibilityHandler)
})

onUnmounted(() => {
  stopRefresh()
  if (visibilityHandler) {
    document.removeEventListener('visibilitychange', visibilityHandler)
  }
})
</script>

<style scoped>
/* 样式已移至 assets/css/home.css */
</style>
