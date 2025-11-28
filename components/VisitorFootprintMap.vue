<template>
  <div class="footprint-map-container">
    <!-- 足迹项 -->
    <div
      v-for="footprint in footprints"
      :key="footprint.id"
      class="footprint-item"
      :style="getFootprintDynamicStyle(footprint)"
      :title="footprint.message || `${footprint.location || '访客'}的足迹`"
    >
      <span class="footprint-emoji">{{ footprint.emoji }}</span>
      <div v-if="footprint.message" class="footprint-tooltip">
        {{ footprint.message }}
      </div>
    </div>

    <!-- 留下足迹按钮 -->
    <button
      v-if="!hasLeftFootprint"
      @click="showFootprintModal = true"
      class="visitor-button-circle visitor-button-circle-blue footprint-button"
      title="留下你的足迹"
    >
      <i class="fas fa-map-marker-alt"></i>
    </button>

    <!-- 足迹弹窗 -->
    <div v-if="showFootprintModal" class="visitor-modal-overlay" @click="showFootprintModal = false">
      <div class="visitor-modal" @click.stop>
        <div class="visitor-modal-header">
          <h3>留下你的足迹</h3>
          <button @click="showFootprintModal = false" class="visitor-modal-close">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <div class="visitor-modal-body">
          <div class="visitor-form-group">
            <label class="visitor-form-label">选择一个表情或图标</label>
            <div class="visitor-emoji-grid">
              <button
                v-for="emoji in emojiOptions"
                :key="emoji"
                @click="selectedEmoji = emoji"
                :class="['visitor-emoji-option', { 'visitor-emoji-option-selected': selectedEmoji === emoji }]"
              >
                {{ emoji }}
              </button>
            </div>
          </div>

          <div class="visitor-form-group">
            <label class="visitor-form-label">留言（可选）</label>
            <input
              v-model="footprintMessage"
              type="text"
              class="visitor-form-input"
              placeholder="说点什么..."
              maxlength="50"
            />
          </div>

          <div class="visitor-form-group">
            <label class="visitor-form-label">位置（可选）</label>
            <input
              v-model="footprintLocation"
              type="text"
              class="visitor-form-input"
              placeholder="如：杭州"
              maxlength="20"
            />
          </div>

          <button @click="leaveFootprint" class="visitor-button-primary" :disabled="!selectedEmoji || submitting">
            {{ submitting ? '提交中...' : '留下足迹' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Footprint {
  id: number
  visitorId: string
  emoji: string
  iconType?: string
  message?: string
  xPosition?: number
  yPosition?: number
  location?: string
  createdAt: string
}

const api = useApi()
const footprints = ref<Footprint[]>([])
const showFootprintModal = ref(false)
const selectedEmoji = ref('')
const footprintMessage = ref('')
const footprintLocation = ref('')
const submitting = ref(false)
const hasLeftFootprint = ref(false)

const emojiOptions = [
  '👋', '❤️', '👍', '🎉', '⭐', '💫', '🌟', '🔥',
  '💯', '🎊', '✨', '🌈', '🌙', '☀️', '🌊', '🌺',
  '🌸', '🍀', '🎈', '🎁', '🎂', '🍰', '☕', '🚀'
]

// 获取所有足迹
const fetchFootprints = async () => {
  try {
    const res = await api.get<Footprint[]>('/VisitorInteraction/footprints')
    if (res && Array.isArray(res)) {
      footprints.value = res
    }
  } catch (e) {
    console.error('获取足迹失败', e)
  }
}

// 留下足迹
const leaveFootprint = async () => {
  if (!selectedEmoji.value) {
    return
  }

  submitting.value = true
  try {
    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'
    
    // 随机位置（如果没有指定）
    const xPosition = Math.random() * 80 + 10 // 10-90%
    const yPosition = Math.random() * 80 + 10 // 10-90%

    await api.post('/VisitorInteraction/footprint', {
      visitorId,
      emoji: selectedEmoji.value,
      iconType: 'emoji',
      message: footprintMessage.value || null,
      xPosition,
      yPosition,
      location: footprintLocation.value || null
    })

    // 添加新足迹到列表
    const newFootprint: Footprint = {
      id: Date.now(),
      visitorId,
      emoji: selectedEmoji.value,
      message: footprintMessage.value || undefined,
      xPosition,
      yPosition,
      location: footprintLocation.value || undefined,
      createdAt: new Date().toISOString()
    }

    footprints.value.push(newFootprint)
    hasLeftFootprint.value = true
    showFootprintModal.value = false

    // 重置表单
    selectedEmoji.value = ''
    footprintMessage.value = ''
    footprintLocation.value = ''
  } catch (e) {
    console.error('留下足迹失败', e)
  } finally {
    submitting.value = false
  }
}

// 获取足迹动态样式（仅保留必须动态计算的属性）
const getFootprintDynamicStyle = (footprint: Footprint) => {
  return {
    left: footprint.xPosition ? `${footprint.xPosition}%` : `${Math.random() * 80 + 10}%`,
    top: footprint.yPosition ? `${footprint.yPosition}%` : `${Math.random() * 80 + 10}%`,
    animationDelay: `${Math.random() * 2}s`
  }
}

// 检查是否已留下足迹
const checkFootprint = () => {
  const visitorId = localStorage.getItem('visitor_id')
  if (visitorId) {
    hasLeftFootprint.value = footprints.value.some(f => f.visitorId === visitorId)
  }
}

onMounted(() => {
  fetchFootprints().then(() => {
    checkFootprint()
  })
})
</script>

<style scoped>
.footprint-map-container {
  position: relative;
  width: 100%;
  min-height: 200px;
  position: fixed;
  inset: 0;
  pointer-events: none;
  z-index: 50;
  overflow: hidden;
}

.footprint-item {
  position: absolute;
  font-size: 1.5rem;
  cursor: pointer;
  animation: footprint-appear 0.5s ease-out;
  transition: transform 0.3s ease;
  pointer-events: auto;
}

.footprint-item:hover {
  transform: scale(1.3);
  z-index: 10;
}

.footprint-item:hover .footprint-tooltip {
  opacity: 1;
  visibility: visible;
}

.footprint-emoji {
  display: block;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.3));
}

.footprint-tooltip {
  position: absolute;
  bottom: 100%;
  left: 50%;
  transform: translateX(-50%);
  margin-bottom: 0.5rem;
  padding: 0.5rem 0.75rem;
  background: rgba(0, 0, 0, 0.8);
  backdrop-filter: blur(10px);
  color: white;
  font-size: 0.75rem;
  border-radius: 0.5rem;
  white-space: nowrap;
  opacity: 0;
  visibility: hidden;
  transition: all 0.3s ease;
  pointer-events: none;
}

.footprint-tooltip::after {
  content: '';
  position: absolute;
  top: 100%;
  left: 50%;
  transform: translateX(-50%);
  border: 4px solid transparent;
  border-top-color: rgba(0, 0, 0, 0.8);
}

.footprint-button {
  position: relative;
  width: 100%;
  height: 3rem;
  border-radius: 0.5rem;
  background: rgba(59, 130, 246, 0.9);
  backdrop-filter: blur(10px);
  border: 2px solid rgba(255, 255, 255, 0.3);
  color: white;
  font-size: 1rem;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
  pointer-events: auto;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: 1rem;
}

.footprint-button:hover {
  background: rgba(59, 130, 246, 1);
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.4);
}

@keyframes footprint-appear {
  from {
    opacity: 0;
    transform: scale(0) rotate(-180deg);
  }
  to {
    opacity: 1;
    transform: scale(1) rotate(0deg);
  }
}
</style>

