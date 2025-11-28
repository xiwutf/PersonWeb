<template>
  <div class="challenge-button-container">
    <button
      @click="handleChallengeClick"
      :disabled="participating"
      class="challenge-button"
      :class="{ 'participating': participating, 'completed': challenge?.completed }"
    >
      <span class="button-icon">🎯</span>
      <span class="button-text">{{ challenge?.challengeName || '参与挑战' }}</span>
      <span v-if="challenge" class="button-progress">
        {{ challenge.currentCount }} / {{ challenge.targetCount }}
      </span>
    </button>

    <!-- 挑战完成提示 -->
    <Transition name="fade">
      <div v-if="showCompletion" class="completion-overlay">
        <div class="completion-content">
          <div class="completion-icon">🎉</div>
          <div class="completion-title">挑战完成！</div>
          <div class="completion-reward">{{ challenge?.rewardDescription }}</div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
const api = useApi()

interface Challenge {
  challengeCode: string
  challengeName: string
  challengeType: string
  targetCount: number
  currentCount: number
  progress: number
  rewardDescription?: string
  completed?: boolean
}

const challenge = ref<Challenge | null>(null)
const participating = ref(false)
const showCompletion = ref(false)

// 获取挑战信息
const fetchChallenge = async () => {
  try {
    const res = await api.get('/VisitorGamification/challenges')
    if (Array.isArray(res) && res.length > 0) {
      // 获取第一个活跃挑战
      challenge.value = res[0] as Challenge
    }
  } catch (e) {
    console.error('获取挑战失败', e)
  }
}

// 参与挑战
const handleChallengeClick = async () => {
  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId || !challenge.value || participating.value) return

  participating.value = true
  try {
    const res = await api.post('/VisitorGamification/challenge/participate', {
      visitorId,
      challengeCode: challenge.value.challengeCode,
      actionType: 'button_press',
      contributedCount: 1
    })

    if (res?.challenge) {
      challenge.value = res.challenge as Challenge

      // 如果完成，显示完成提示并触发烟花效果
      if (res.challenge.completed) {
        showCompletion.value = true
        triggerFireworks()
        
        setTimeout(() => {
          showCompletion.value = false
        }, 5000)
      }
    }
  } catch (e) {
    console.error('参与挑战失败', e)
  } finally {
    participating.value = false
  }
}

// 触发全屏烟花效果
const triggerFireworks = () => {
  if (process.client) {
    window.dispatchEvent(new CustomEvent('trigger-fireworks'))
  }
}

onMounted(() => {
  fetchChallenge()
  
  // 定期刷新挑战进度
  const interval = setInterval(() => {
    fetchChallenge()
  }, 10000) // 每10秒刷新一次

  onUnmounted(() => {
    clearInterval(interval)
  })
})
</script>

<style scoped>
.challenge-button-container {
  position: relative; /* 改为相对定位，由父容器控制位置 */
  width: 100%;
}

.challenge-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 2rem;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
  transition: all 0.3s ease;
}

.challenge-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(102, 126, 234, 0.6);
}

.challenge-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.challenge-button.participating {
  animation: pulse 1s ease-in-out infinite;
}

.challenge-button.completed {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.button-icon {
  font-size: 1.25rem;
}

.button-text {
  flex: 1;
}

.button-progress {
  font-size: 0.75rem;
  opacity: 0.9;
  background: rgba(255, 255, 255, 0.2);
  padding: 0.25rem 0.5rem;
  border-radius: 0.5rem;
}

.completion-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.8);
  backdrop-filter: blur(10px);
  z-index: 10000;
  display: flex;
  align-items: center;
  justify-content: center;
}

.completion-content {
  text-align: center;
  color: white;
  padding: 2rem;
  background: rgba(30, 41, 59, 0.95);
  border-radius: 1rem;
  border: 2px solid rgba(255, 255, 255, 0.2);
}

.completion-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  animation: bounce 1s ease-in-out;
}

.completion-title {
  font-size: 2rem;
  font-weight: bold;
  margin-bottom: 1rem;
}

.completion-reward {
  font-size: 1rem;
  opacity: 0.9;
}

@keyframes pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.05); }
}

@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-20px); }
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>

