<template>
  <div class="digital-avatar-container" ref="avatarContainer">
    <!-- 3D Avatar 画布容器 -->
    <canvas ref="canvasRef" class="avatar-canvas"></canvas>
    
    <!-- 情绪指示器 -->
    <div v-if="showEmotionIndicator" class="emotion-indicator">
      <span class="emotion-icon">{{ currentEmotion.emoji }}</span>
      <span class="emotion-text">{{ currentEmotion.name }}</span>
    </div>
    
    <!-- 控制面板（开发模式） -->
    <div v-if="isDevMode" class="avatar-controls">
      <button @click="changeEmotion('happy')">😊 开心</button>
      <button @click="changeEmotion('thinking')">🤔 思考</button>
      <button @click="changeEmotion('focused')">😎 专注</button>
      <button @click="changeEmotion('tired')">😴 疲惫</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'

interface Emotion {
  name: string
  emoji: string
  color: string
  animation?: string
}

interface Props {
  showEmotionIndicator?: boolean
  isDevMode?: boolean
  initialEmotion?: string
}

const props = withDefaults(defineProps<Props>(), {
  showEmotionIndicator: true,
  isDevMode: false,
  initialEmotion: 'happy'
})

// 情绪定义
const emotions: Record<string, Emotion> = {
  happy: { name: '开心', emoji: '😊', color: '#FFD700' },
  thinking: { name: '思考', emoji: '🤔', color: '#4A90E2' },
  focused: { name: '专注', emoji: '😎', color: '#7B68EE' },
  tired: { name: '疲惫', emoji: '😴', color: '#95A5A6' },
  excited: { name: '兴奋', emoji: '🎉', color: '#FF6B6B' },
  calm: { name: '平静', emoji: '😌', color: '#51CF66' }
}

const canvasRef = ref<HTMLCanvasElement | null>(null)
const avatarContainer = ref<HTMLDivElement | null>(null)
const currentEmotionKey = ref<string>(props.initialEmotion)
const currentEmotion = computed(() => emotions[currentEmotionKey.value] || emotions.happy)

let animationFrameId: number | null = null
let threeScene: any = null
let threeCamera: any = null
let threeRenderer: any = null

// 初始化3D场景
const initThreeScene = async () => {
  if (!canvasRef.value) return

  // 动态导入 Three.js（按需加载）
  const THREE = await import('three')
  
  // 创建场景
  threeScene = new THREE.Scene()
  threeScene.background = new THREE.Color(0x000000)
  
  // 创建相机
  threeCamera = new THREE.PerspectiveCamera(
    75,
    canvasRef.value.clientWidth / canvasRef.value.clientHeight,
    0.1,
    1000
  )
  threeCamera.position.z = 5
  
  // 创建渲染器
  threeRenderer = new THREE.WebGLRenderer({ 
    canvas: canvasRef.value,
    alpha: true,
    antialias: true
  })
  threeRenderer.setSize(canvasRef.value.clientWidth, canvasRef.value.clientHeight)
  threeRenderer.setPixelRatio(window.devicePixelRatio)
  
  // 添加基础几何体（临时，后续替换为Avatar模型）
  const geometry = new THREE.BoxGeometry(1, 1, 1)
  const material = new THREE.MeshBasicMaterial({ 
    color: new THREE.Color(currentEmotion.value.color)
  })
  const cube = new THREE.Mesh(geometry, material)
  threeScene.add(cube)
  
  // 添加灯光
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.5)
  threeScene.add(ambientLight)
  
  const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8)
  directionalLight.position.set(5, 5, 5)
  threeScene.add(directionalLight)
  
  // 开始动画循环
  animate()
}

// 动画循环
const animate = () => {
  if (!threeScene || !threeCamera || !threeRenderer) return
  
  animationFrameId = requestAnimationFrame(animate)
  
  // 旋转动画（临时）
  if (threeScene.children.length > 0) {
    const mesh = threeScene.children.find((child: any) => child.type === 'Mesh')
    if (mesh) {
      mesh.rotation.x += 0.01
      mesh.rotation.y += 0.01
    }
  }
  
  threeRenderer.render(threeScene, threeCamera)
}

// 改变情绪
const changeEmotion = (emotionKey: string) => {
  if (!emotions[emotionKey]) return
  
  currentEmotionKey.value = emotionKey
  
  // 更新3D场景颜色（临时）
  if (threeScene) {
    const mesh = threeScene.children.find((child: any) => child.type === 'Mesh')
    if (mesh && mesh.material) {
      mesh.material.color.set(currentEmotion.value.color)
    }
  }
  
  // 触发情绪变化事件
  if (process.client) {
    window.dispatchEvent(new CustomEvent('avatar-emotion-changed', {
      detail: { emotion: emotionKey, emotionData: currentEmotion.value }
    }))
  }
}

// 监听访客行为
const setupBehaviorListener = () => {
  if (!process.client) return
  
  // 监听滚动行为
  let lastScrollTime = Date.now()
  window.addEventListener('scroll', () => {
    const now = Date.now()
    if (now - lastScrollTime > 2000) {
      changeEmotion('focused')
      lastScrollTime = now
    }
  })
  
  // 监听鼠标移动（活跃状态）
  let idleTimer: NodeJS.Timeout | null = null
  window.addEventListener('mousemove', () => {
    if (idleTimer) clearTimeout(idleTimer)
    changeEmotion('happy')
    
    idleTimer = setTimeout(() => {
      changeEmotion('tired')
    }, 30000) // 30秒无操作后变疲惫
  })
  
  // 监听时间变化
  const updateByTime = () => {
    const hour = new Date().getHours()
    if (hour >= 6 && hour < 12) {
      changeEmotion('happy') // 早上：开心
    } else if (hour >= 12 && hour < 18) {
      changeEmotion('focused') // 下午：专注
    } else if (hour >= 18 && hour < 22) {
      changeEmotion('calm') // 晚上：平静
    } else {
      changeEmotion('tired') // 深夜：疲惫
    }
  }
  
  updateByTime()
  setInterval(updateByTime, 3600000) // 每小时更新一次
}

// 监听AI对话
const setupAIConversationListener = () => {
  if (!process.client) return
  
  window.addEventListener('ai-message-sent', ((e: CustomEvent) => {
    // 根据消息内容判断情绪
    const message = e.detail?.message?.toLowerCase() || ''
    
    if (message.includes('你好') || message.includes('hi')) {
      changeEmotion('excited')
    } else if (message.includes('谢谢') || message.includes('thank')) {
      changeEmotion('happy')
    } else if (message.includes('问题') || message.includes('question')) {
      changeEmotion('thinking')
    }
  }) as EventListener)
}

// 响应式调整
const handleResize = () => {
  if (!canvasRef.value || !threeCamera || !threeRenderer) return
  
  const width = canvasRef.value.clientWidth
  const height = canvasRef.value.clientHeight
  
  threeCamera.aspect = width / height
  threeCamera.updateProjectionMatrix()
  threeRenderer.setSize(width, height)
}

onMounted(async () => {
  await initThreeScene()
  setupBehaviorListener()
  setupAIConversationListener()
  
  if (process.client) {
    window.addEventListener('resize', handleResize)
  }
})

onUnmounted(() => {
  if (animationFrameId) {
    cancelAnimationFrame(animationFrameId)
  }
  
  if (threeRenderer) {
    threeRenderer.dispose()
  }
  
  if (process.client) {
    window.removeEventListener('resize', handleResize)
  }
})

// 监听情绪变化
watch(currentEmotionKey, (newEmotion) => {
  // 可以在这里添加情绪变化的动画效果
  console.log('情绪变化:', newEmotion, currentEmotion.value)
})
</script>

<style scoped>
.digital-avatar-container {
  position: relative;
  width: 100%;
  height: 100%;
  min-height: 300px;
}

.avatar-canvas {
  width: 100%;
  height: 100%;
  display: block;
}

.emotion-indicator {
  position: absolute;
  top: 1rem;
  left: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(10px);
  border-radius: 2rem;
  color: white;
  font-size: 0.875rem;
  z-index: 10;
}

.emotion-icon {
  font-size: 1.25rem;
}

.emotion-text {
  font-weight: 500;
}

.avatar-controls {
  position: absolute;
  bottom: 1rem;
  left: 1rem;
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  z-index: 10;
}

.avatar-controls button {
  padding: 0.5rem 1rem;
  background: rgba(59, 130, 246, 0.9);
  border: none;
  border-radius: 0.5rem;
  color: white;
  cursor: pointer;
  font-size: 0.875rem;
  transition: all 0.2s ease;
}

.avatar-controls button:hover {
  background: rgba(59, 130, 246, 1);
  transform: translateY(-2px);
}
</style>

