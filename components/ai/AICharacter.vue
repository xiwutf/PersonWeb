<template>
  <div class="relative w-full h-screen min-h-[600px] flex items-center justify-center bg-gradient-to-b from-blue-50 to-purple-50 dark:from-gray-900 dark:to-gray-800">
    <canvas ref="canvasRef" class="absolute inset-0 w-full h-full" />
    
    <!-- 对话气泡 -->
    <div
      v-if="currentText"
      class="absolute top-20 left-1/2 transform -translate-x-1/2 bg-white dark:bg-gray-800 rounded-2xl p-4 shadow-2xl max-w-md z-10 animate-fade-in"
    >
      <div class="flex items-start gap-3">
        <div class="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-purple-500 flex items-center justify-center text-white font-bold">
          AI
        </div>
        <div class="flex-1">
          <p class="text-gray-800 dark:text-gray-200">{{ currentText }}</p>
          <div v-if="isSpeaking" class="flex gap-1 mt-2">
            <span class="w-2 h-2 bg-blue-500 rounded-full animate-bounce" style="animation-delay: 0s"></span>
            <span class="w-2 h-2 bg-blue-500 rounded-full animate-bounce" style="animation-delay: 0.2s"></span>
            <span class="w-2 h-2 bg-blue-500 rounded-full animate-bounce" style="animation-delay: 0.4s"></span>
          </div>
        </div>
      </div>
    </div>
    
    <!-- 控制按钮 -->
    <div class="absolute bottom-8 left-1/2 transform -translate-x-1/2 flex gap-4 z-10">
      <button
        @click="startIntroduction"
        class="px-6 py-3 bg-blue-600 text-white rounded-full hover:bg-blue-700 transition shadow-lg font-bold"
      >
        {{ isSpeaking ? '停止' : '开始介绍' }}
      </button>
      <button
        v-if="showChatInput"
        @click="toggleChat"
        class="px-6 py-3 bg-purple-600 text-white rounded-full hover:bg-purple-700 transition shadow-lg font-bold"
      >
        对话
      </button>
    </div>
    
    <!-- 聊天输入 -->
    <div
      v-if="showChat"
      class="absolute bottom-8 left-1/2 transform -translate-x-1/2 bg-white dark:bg-gray-800 rounded-2xl p-4 shadow-2xl max-w-md z-10"
    >
      <div class="flex gap-2">
        <input
          v-model="chatInput"
          @keyup.enter="sendMessage"
          type="text"
          placeholder="输入消息..."
          class="flex-1 px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <button
          @click="sendMessage"
          class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
        >
          发送
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'

const props = withDefaults(defineProps<{
  introductionText?: string[]
  enableChat?: boolean
}>(), {
  introductionText: () => [
    '你好！我是溪午听风',
    '我是一名全栈开发者',
    '专注于 Vue.js、Nuxt.js 和 Node.js',
    '欢迎来到我的个人网站！',
    '这里展示我的项目、文章和技术分享'
  ],
  enableChat: false
})

const canvasRef = ref<HTMLCanvasElement | null>(null)
const currentText = ref('')
const isSpeaking = ref(false)
const showChat = ref(false)
const showChatInput = ref(props.enableChat)
const chatInput = ref('')

let animationId: number | null = null
let blinkTimer: NodeJS.Timeout | null = null
let introTimer: number | null = null
let visibilityHandler: (() => void) | null = null
let isPaused = false
let lastFrameTime = 0
let eyeState = { left: { open: true }, right: { open: true } }
let mouthState = { open: false, frame: 0 }
let speechIndex = 0

// 绘制圆角矩形的辅助函数
const drawRoundRect = (ctx: CanvasRenderingContext2D, x: number, y: number, w: number, h: number, r: number) => {
  if (w < 2 * r) r = w / 2
  if (h < 2 * r) r = h / 2
  ctx.beginPath()
  ctx.moveTo(x + r, y)
  ctx.arcTo(x + w, y, x + w, y + h, r)
  ctx.arcTo(x + w, y + h, x, y + h, r)
  ctx.arcTo(x, y + h, x, y, r)
  ctx.arcTo(x, y, x + w, y, r)
  ctx.closePath()
}

const initCanvas = () => {
  if (!canvasRef.value) return
  
  const canvas = canvasRef.value
  const container = canvas.parentElement
  
  // 确保容器有尺寸
  if (container) {
    const rect = container.getBoundingClientRect()
    canvas.width = rect.width || window.innerWidth
    canvas.height = rect.height || window.innerHeight
  } else {
    canvas.width = window.innerWidth
    canvas.height = window.innerHeight
  }
  
  // 如果尺寸仍然为0，使用默认值
  if (canvas.width === 0) canvas.width = 800
  if (canvas.height === 0) canvas.height = 600
  
  console.log('AI Character canvas initialized:', canvas.width, canvas.height)
  
  draw()
  
  // 眨眼动画
  blinkTimer = setInterval(() => {
    eyeState.left.open = false
    eyeState.right.open = false
    setTimeout(() => {
      eyeState.left.open = true
      eyeState.right.open = true
    }, 150)
  }, 4500)
}

const draw = (timestamp = 0) => {
  if (!canvasRef.value) return
  if (isPaused) {
    animationId = requestAnimationFrame(draw)
    return
  }
  
  const canvas = canvasRef.value
  const ctx = canvas.getContext('2d')
  if (!ctx) return
  if (timestamp - lastFrameTime < 33) {
    animationId = requestAnimationFrame(draw)
    return
  }
  lastFrameTime = timestamp
  
  // 清空画布
  ctx.clearRect(0, 0, canvas.width, canvas.height)
  
  const centerX = canvas.width / 2
  const centerY = canvas.height / 2 - 30 // 稍微上移
  
  // 缩放比例 - 让角色更大
  const scale = Math.min(canvas.width / 500, canvas.height / 700, 2)
  const headRadius = 140 * scale
  const bodyWidth = 200 * scale
  const bodyHeight = 180 * scale
  
  // 保存上下文
  ctx.save()
  ctx.translate(centerX, centerY)
  
  // 绘制身体（现代服装风格 - 衬衫）
  const bodyGradient = ctx.createLinearGradient(-bodyWidth/2, 0, bodyWidth/2, bodyHeight)
  bodyGradient.addColorStop(0, 'var(--color-bg-card)')
  bodyGradient.addColorStop(0.3, '#f0f0f0')
  bodyGradient.addColorStop(0.7, '#e0e0e0')
  bodyGradient.addColorStop(1, '#d0d0d0')
  ctx.fillStyle = bodyGradient
  
  // 圆角矩形身体
  const bodyX = -bodyWidth / 2
  const bodyY = headRadius * 0.6
  const cornerRadius = 20 * scale
  ctx.beginPath()
  ctx.moveTo(bodyX + cornerRadius, bodyY)
  ctx.lineTo(bodyX + bodyWidth - cornerRadius, bodyY)
  ctx.quadraticCurveTo(bodyX + bodyWidth, bodyY, bodyX + bodyWidth, bodyY + cornerRadius)
  ctx.lineTo(bodyX + bodyWidth, bodyY + bodyHeight - cornerRadius)
  ctx.quadraticCurveTo(bodyX + bodyWidth, bodyY + bodyHeight, bodyX + bodyWidth - cornerRadius, bodyY + bodyHeight)
  ctx.lineTo(bodyX + cornerRadius, bodyY + bodyHeight)
  ctx.quadraticCurveTo(bodyX, bodyY + bodyHeight, bodyX, bodyY + bodyHeight - cornerRadius)
  ctx.lineTo(bodyX, bodyY + cornerRadius)
  ctx.quadraticCurveTo(bodyX, bodyY, bodyX + cornerRadius, bodyY)
  ctx.closePath()
  ctx.fill()
  
  // 身体阴影
  ctx.shadowColor = 'rgba(0, 0, 0, 0.3)'
  ctx.shadowBlur = 20 * scale
  ctx.shadowOffsetX = 0
  ctx.shadowOffsetY = 10 * scale
  ctx.fill()
  ctx.shadowBlur = 0
  
  // 绘制手臂（真实肤色）
  const armWidth = 35 * scale
  const armHeight = 90 * scale
  const armGradient = ctx.createLinearGradient(0, 0, 0, armHeight)
  armGradient.addColorStop(0, '#fdbcb4')
  armGradient.addColorStop(0.5, '#e8a87c')
  armGradient.addColorStop(1, '#d4a574')
  ctx.fillStyle = armGradient
  
  // 左手臂
  drawRoundRect(ctx, -bodyWidth/2 - armWidth, bodyY + 20 * scale, armWidth, armHeight, 15 * scale)
  ctx.fill()
  
  // 右手臂
  drawRoundRect(ctx, bodyWidth/2, bodyY + 20 * scale, armWidth, armHeight, 15 * scale)
  ctx.fill()
  
  // 绘制头发（在头部之前）
  const hairGradient = ctx.createLinearGradient(0, -headRadius * 1.2, 0, -headRadius * 0.5)
  hairGradient.addColorStop(0, '#1a1a1a')
  hairGradient.addColorStop(0.5, '#2d2d2d')
  hairGradient.addColorStop(1, '#1a1a1a')
  ctx.fillStyle = hairGradient
  
  // 头发形状（椭圆形，稍微超出头部）
  ctx.beginPath()
  ctx.ellipse(0, -headRadius * 0.6, headRadius * 1.1, headRadius * 0.7, 0, 0, Math.PI * 2)
  ctx.fill()
  
  // 绘制头部（真实肤色渐变）
  const headGradient = ctx.createRadialGradient(0, -headRadius * 0.3, 0, 0, 0, headRadius)
  headGradient.addColorStop(0, '#fdbcb4') // 亮肤色
  headGradient.addColorStop(0.5, '#e8a87c') // 中等肤色
  headGradient.addColorStop(1, '#d4a574') // 深肤色
  ctx.fillStyle = headGradient
  
  // 头部阴影
  ctx.shadowColor = 'rgba(0, 0, 0, 0.25)'
  ctx.shadowBlur = 20 * scale
  ctx.shadowOffsetX = 0
  ctx.shadowOffsetY = 8 * scale
  
  ctx.beginPath()
  ctx.arc(0, -headRadius * 0.2, headRadius, 0, Math.PI * 2)
  ctx.fill()
  ctx.shadowBlur = 0
  
  // 添加面部高光（让脸部更有立体感）
  const faceHighlight = ctx.createRadialGradient(0, -headRadius * 0.5, 0, 0, -headRadius * 0.3, headRadius * 0.6)
  faceHighlight.addColorStop(0, 'rgba(255, 255, 255, 0.3)')
  faceHighlight.addColorStop(1, 'rgba(255, 255, 255, 0)')
  ctx.fillStyle = faceHighlight
  ctx.beginPath()
  ctx.arc(0, -headRadius * 0.3, headRadius * 0.6, 0, Math.PI * 2)
  ctx.fill()
  
  // 绘制眼睛（更真实的数字人眼睛）
  const eyeY = -headRadius * 0.25
  const eyeSpacing = 40 * scale
  const eyeWidth = 28 * scale
  const eyeHeight = 16 * scale
  
  const drawEye = (x: number, isOpen: boolean) => {
    if (isOpen) {
      // 眼白
      ctx.fillStyle = 'var(--color-bg-card)'
      ctx.beginPath()
      ctx.ellipse(x, eyeY, eyeWidth, eyeHeight, 0, 0, Math.PI * 2)
      ctx.fill()
      
      // 眼白阴影
      ctx.fillStyle = 'var(--shadow)'
      ctx.beginPath()
      ctx.ellipse(x, eyeY + eyeHeight * 0.3, eyeWidth * 0.9, eyeHeight * 0.5, 0, 0, Math.PI * 2)
      ctx.fill()
      
      // 虹膜（棕色）
      const irisSize = 12 * scale
      const irisGradient = ctx.createRadialGradient(x, eyeY, 0, x, eyeY, irisSize)
      irisGradient.addColorStop(0, '#8b4513')
      irisGradient.addColorStop(0.7, '#654321')
      irisGradient.addColorStop(1, '#3d2817')
      ctx.fillStyle = irisGradient
      ctx.beginPath()
      ctx.arc(x, eyeY, irisSize, 0, Math.PI * 2)
      ctx.fill()
      
      // 瞳孔
      ctx.fillStyle = '#000000'
      ctx.beginPath()
      ctx.arc(x, eyeY, irisSize * 0.6, 0, Math.PI * 2)
      ctx.fill()
      
      // 眼睛高光（两个高光点，更真实）
      ctx.fillStyle = 'var(--color-bg-card)'
      ctx.beginPath()
      ctx.arc(x - 3 * scale, eyeY - 3 * scale, 3 * scale, 0, Math.PI * 2)
      ctx.fill()
      ctx.beginPath()
      ctx.arc(x + 2 * scale, eyeY - 2 * scale, 2 * scale, 0, Math.PI * 2)
      ctx.fill()
      
      // 上眼皮
      ctx.strokeStyle = 'rgba(0, 0, 0, 0.2)'
      ctx.lineWidth = 2 * scale
      ctx.beginPath()
      ctx.ellipse(x, eyeY - eyeHeight * 0.4, eyeWidth * 0.95, eyeHeight * 0.3, 0, 0, Math.PI * 2)
      ctx.stroke()
    } else {
      // 闭眼（更自然的曲线）
      ctx.strokeStyle = '#654321'
      ctx.lineWidth = 4 * scale
      ctx.beginPath()
      ctx.moveTo(x - eyeWidth, eyeY)
      ctx.quadraticCurveTo(x, eyeY - 3 * scale, x + eyeWidth, eyeY)
      ctx.stroke()
    }
  }
  
  drawEye(-eyeSpacing, eyeState.left.open)
  drawEye(eyeSpacing, eyeState.right.open)
  
  // 眉毛
  ctx.strokeStyle = '#3d2817'
  ctx.lineWidth = 3 * scale
  ctx.lineCap = 'round'
  
  // 左眉毛
  ctx.beginPath()
  ctx.moveTo(-eyeSpacing - eyeWidth * 0.5, eyeY - eyeHeight * 1.2)
  ctx.quadraticCurveTo(-eyeSpacing, eyeY - eyeHeight * 1.4, -eyeSpacing + eyeWidth * 0.5, eyeY - eyeHeight * 1.2)
  ctx.stroke()
  
  // 右眉毛
  ctx.beginPath()
  ctx.moveTo(eyeSpacing - eyeWidth * 0.5, eyeY - eyeHeight * 1.2)
  ctx.quadraticCurveTo(eyeSpacing, eyeY - eyeHeight * 1.4, eyeSpacing + eyeWidth * 0.5, eyeY - eyeHeight * 1.2)
  ctx.stroke()
  
  // 绘制嘴巴（说话时动画）
  const mouthY = headRadius * 0.4
  
  if (isSpeaking.value) {
    mouthState.frame++
    const openAmount = Math.sin(mouthState.frame * 0.3) * 0.5 + 0.5
    const mouthWidth = 25 * scale + openAmount * 20 * scale
    const mouthHeight = 8 * scale + openAmount * 15 * scale
    
    ctx.fillStyle = '#1e2937'
    ctx.beginPath()
    ctx.ellipse(0, mouthY, mouthWidth, mouthHeight, 0, 0, Math.PI * 2)
    ctx.fill()
  } else {
    // 微笑
    ctx.strokeStyle = '#1e2937'
    ctx.lineWidth = 5 * scale
    ctx.beginPath()
    ctx.arc(0, mouthY, 20 * scale, 0.2, Math.PI - 0.2)
    ctx.stroke()
  }
  
  // 绘制腿部（裤子 - 深色）
  const legWidth = 45 * scale
  const legHeight = 100 * scale
  const legGradient = ctx.createLinearGradient(0, 0, 0, legHeight)
  legGradient.addColorStop(0, '#2d3748')
  legGradient.addColorStop(0.5, '#1a202c')
  legGradient.addColorStop(1, '#171923')
  ctx.fillStyle = legGradient
  
  const legY = bodyY + bodyHeight
  const legSpacing = 25 * scale
  
  // 左腿
  drawRoundRect(ctx, -legWidth - legSpacing, legY, legWidth, legHeight, 10 * scale)
  ctx.fill()
  
  // 右腿
  drawRoundRect(ctx, legSpacing, legY, legWidth, legHeight, 10 * scale)
  ctx.fill()
  
  // 恢复上下文
  ctx.restore()
  
  animationId = requestAnimationFrame(draw)
}

const speak = async (text: string) => {
  currentText.value = text
  isSpeaking.value = true
  
  // 模拟语音时长
  const duration = text.length * 100
  
  await new Promise((resolve) => setTimeout(resolve, duration))
  
  isSpeaking.value = false
}

const startIntroduction = async () => {
  if (isSpeaking.value) {
    isSpeaking.value = false
    currentText.value = ''
    speechIndex = 0
    return
  }
  
  speechIndex = 0
  for (const text of props.introductionText) {
    if (!isSpeaking.value && speechIndex > 0) break
    await speak(text)
    await new Promise((resolve) => setTimeout(resolve, 500))
    speechIndex++
  }
  
  isSpeaking.value = false
}

const toggleChat = () => {
  showChat.value = !showChat.value
}

const sendMessage = async () => {
  if (!chatInput.value.trim()) return
  
  const userMessage = chatInput.value
  chatInput.value = ''
  
  // 显示用户消息
  currentText.value = `你: ${userMessage}`
  await new Promise((resolve) => setTimeout(resolve, 1000))
  
  // 简单的回复逻辑（可以集成 ChatGPT API）
  const responses = [
    '很有趣的问题！',
    '让我想想...',
    '这是一个很好的话题！',
    '我理解你的意思。',
    '谢谢你的提问！'
  ]
  
  const response = responses[Math.floor(Math.random() * responses.length)]
  await speak(response)
}

const handleResize = () => {
  if (canvasRef.value) {
    const container = canvasRef.value.parentElement
    if (container) {
      const rect = container.getBoundingClientRect()
      canvasRef.value.width = rect.width || window.innerWidth
      canvasRef.value.height = rect.height || window.innerHeight
    } else {
      canvasRef.value.width = window.innerWidth
      canvasRef.value.height = window.innerHeight
    }
  }
}

onMounted(() => {
  // 延迟初始化，确保 DOM 已完全渲染
  nextTick(() => {
    introTimer = window.setTimeout(() => {
      initCanvas()
    }, 100)
  })
  
  // 监听窗口大小变化
  visibilityHandler = () => {
    isPaused = document.hidden
  }
  document.addEventListener('visibilitychange', visibilityHandler)
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  if (animationId) {
    cancelAnimationFrame(animationId)
  }
  if (blinkTimer) {
    clearInterval(blinkTimer)
  }
  if (introTimer) {
    clearTimeout(introTimer)
  }
  if (visibilityHandler) {
    document.removeEventListener('visibilitychange', visibilityHandler)
  }
  window.removeEventListener('resize', handleResize)
})
</script>

<style scoped>
@keyframes fade-in {
  from {
    opacity: 0;
    transform: translate(-50%, -10px);
  }
  to {
    opacity: 1;
    transform: translate(-50%, 0);
  }
}

.animate-fade-in {
  animation: fade-in 0.3s ease-out;
}
</style>

