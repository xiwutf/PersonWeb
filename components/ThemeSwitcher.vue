<template>
  <div class="theme-switcher">
    <!-- 切换按钮 -->
    <button
      @click="togglePanel"
      class="theme-switcher-button"
      :class="{ 'active': isOpen }"
      title="切换主题风格"
    >
      <i class="fas fa-palette"></i>
    </button>

    <!-- 切换面板 -->
    <Transition name="slide-up">
      <div v-if="isOpen" class="theme-switcher-panel">
        <div class="panel-header">
          <h3 class="panel-title">风格切换</h3>
          <button @click="closePanel" class="panel-close">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <!-- 主题选择 -->
        <div class="panel-section">
          <h4 class="section-title">主题风格</h4>
          <div class="theme-grid">
            <div
              v-for="theme in themes"
              :key="theme.code"
              @click="selectTheme(theme.code)"
              class="theme-card"
              :class="{ 'active': currentTheme === theme.code }"
            >
              <div v-if="theme.previewImage" class="theme-preview">
                <img :src="theme.previewImage" :alt="theme.displayName" />
              </div>
              <div v-else class="theme-preview theme-preview-placeholder">
                <i class="fas fa-image"></i>
              </div>
              <div class="theme-info">
                <div class="theme-name">{{ theme.displayName }}</div>
                <div v-if="theme.isDefault" class="theme-badge">默认</div>
              </div>
            </div>
          </div>
        </div>

        <!-- 背景效果选择 -->
        <div class="panel-section">
          <h4 class="section-title">动态背景</h4>
          <div class="background-grid">
            <div
              v-for="bg in backgrounds"
              :key="bg.code"
              @click="selectBackground(bg.code)"
              class="background-card"
              :class="{ 'active': currentBackground === bg.code }"
            >
              <div class="background-preview" :class="`bg-${bg.code}`">
                <i class="fas fa-image"></i>
              </div>
              <div class="background-name">{{ bg.displayName }}</div>
            </div>
          </div>
        </div>

        <!-- 自动切换设置 -->
        <div class="panel-section">
          <div class="setting-item">
            <label class="setting-label">
              <input
                type="checkbox"
                v-model="autoSwitch"
                @change="updateAutoSwitch"
              />
              <span>自动切换主题</span>
            </label>
            <input
              v-if="autoSwitch"
              type="number"
              v-model.number="switchInterval"
              @change="updateSwitchInterval"
              class="interval-input"
              min="1"
              placeholder="分钟"
            />
          </div>
        </div>
      </div>
    </Transition>

    <!-- 遮罩层 -->
    <Transition name="fade">
      <div v-if="isOpen" @click="closePanel" class="theme-switcher-overlay"></div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

interface Theme {
  id: number
  code: string
  displayName: string
  previewImage?: string
  isDefault: boolean
}

interface Background {
  id: number
  code: string
  displayName: string
}

const api = useApi()

const isOpen = ref(false)
const themes = ref<Theme[]>([])
const backgrounds = ref<Background[]>([])
const currentTheme = ref<string>('default')
const currentBackground = ref<string>('particles')
const autoSwitch = ref(false)
const switchInterval = ref(30)

let autoSwitchTimer: NodeJS.Timeout | null = null

const fetchThemes = async () => {
  try {
    const res = await api.get<Theme[]>('/Theme/themes')
    themes.value = Array.isArray(res) ? res : []
  } catch (e) {
    console.error('加载主题失败', e)
  }
}

const fetchBackgrounds = async () => {
  try {
    const res = await api.get<Background[]>('/Theme/backgrounds')
    backgrounds.value = Array.isArray(res) ? res : []
  } catch (e) {
    console.error('加载背景效果失败', e)
  }
}

const loadUserPreference = async () => {
  try {
    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'
    const res = await api.post('/Theme/preference', { visitorId })
    if (res) {
      currentTheme.value = res.themeCode || 'default'
      currentBackground.value = res.backgroundEffectCode || 'particles'
      autoSwitch.value = res.autoSwitch || false
      switchInterval.value = res.switchInterval || 30
    }
  } catch (e) {
    console.error('加载用户偏好失败', e)
  }
}

const selectTheme = async (themeCode: string) => {
  currentTheme.value = themeCode
  applyTheme(themeCode)
  
  try {
    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'
    await api.post('/Theme/preference/update', {
      visitorId,
      themeCode
    })
  } catch (e) {
    console.error('更新主题偏好失败', e)
  }
}

const selectBackground = async (bgCode: string) => {
  currentBackground.value = bgCode
  applyBackground(bgCode)
  
  try {
    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'
    await api.post('/Theme/preference/update', {
      visitorId,
      backgroundEffectCode: bgCode
    })
  } catch (e) {
    console.error('更新背景偏好失败', e)
  }
}

const applyTheme = (themeCode: string) => {
  // 移除所有主题类
  document.documentElement.classList.remove(
    'theme-default',
    'theme-minimal',
    'theme-cyberpunk',
    'theme-dark-tech',
    'theme-cartoon',
    'theme-lab-3d'
  )
  
  // 添加新主题类
  document.documentElement.classList.add(`theme-${themeCode}`)
  
  // 触发主题切换事件
  window.dispatchEvent(new CustomEvent('theme-changed', { detail: { theme: themeCode } }))
}

const applyBackground = (bgCode: string) => {
  // 移除所有背景类
  document.documentElement.classList.remove(
    'bg-particles',
    'bg-noise',
    'bg-waves',
    'bg-stars'
  )
  
  // 添加新背景类
  document.documentElement.classList.add(`bg-${bgCode}`)
  
  // 触发背景切换事件
  window.dispatchEvent(new CustomEvent('background-changed', { detail: { background: bgCode } }))
}

const updateAutoSwitch = async () => {
  try {
    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'
    await api.post('/Theme/preference/update', {
      visitorId,
      autoSwitch: autoSwitch.value,
      switchInterval: switchInterval.value
    })
    
    if (autoSwitch.value) {
      startAutoSwitch()
    } else {
      stopAutoSwitch()
    }
  } catch (e) {
    console.error('更新自动切换设置失败', e)
  }
}

const updateSwitchInterval = async () => {
  try {
    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'
    await api.post('/Theme/preference/update', {
      visitorId,
      switchInterval: switchInterval.value
    })
    
    if (autoSwitch.value) {
      stopAutoSwitch()
      startAutoSwitch()
    }
  } catch (e) {
    console.error('更新切换间隔失败', e)
  }
}

const startAutoSwitch = () => {
  stopAutoSwitch()
  
  if (themes.value.length <= 1) return
  
  autoSwitchTimer = setInterval(() => {
    const currentIndex = themes.value.findIndex(t => t.code === currentTheme.value)
    const nextIndex = (currentIndex + 1) % themes.value.length
    selectTheme(themes.value[nextIndex].code)
  }, switchInterval.value * 60 * 1000)
}

const stopAutoSwitch = () => {
  if (autoSwitchTimer) {
    clearInterval(autoSwitchTimer)
    autoSwitchTimer = null
  }
}

const togglePanel = () => {
  isOpen.value = !isOpen.value
}

const closePanel = () => {
  isOpen.value = false
}

const handleEscape = (e: KeyboardEvent) => {
  if (e.key === 'Escape' && isOpen.value) {
    closePanel()
  }
}

onMounted(async () => {
  await Promise.all([fetchThemes(), fetchBackgrounds(), loadUserPreference()])
  
  // 应用当前主题和背景
  applyTheme(currentTheme.value)
  applyBackground(currentBackground.value)
  
  // 如果启用自动切换，启动定时器
  if (autoSwitch.value) {
    startAutoSwitch()
  }
  
  window.addEventListener('keydown', handleEscape)
})

onUnmounted(() => {
  stopAutoSwitch()
  window.removeEventListener('keydown', handleEscape)
})
</script>

<style scoped>
.theme-switcher {
  position: fixed;
  bottom: 7rem;
  right: 2rem;
  z-index: 1000;
}

.theme-switcher-button {
  width: 3.5rem;
  height: 3.5rem;
  border-radius: 50%;
  background: rgba(59, 130, 246, 0.9);
  backdrop-filter: blur(10px);
  border: 2px solid rgba(255, 255, 255, 0.3);
  color: white;
  font-size: 1.25rem;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.theme-switcher-button:hover {
  background: rgba(59, 130, 246, 1);
  transform: scale(1.1);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.4);
}

.theme-switcher-button.active {
  background: rgba(139, 92, 246, 0.9);
}

.theme-switcher-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(2px);
  z-index: 999;
}

.theme-switcher-panel {
  position: fixed;
  bottom: 11rem;
  right: 2rem;
  width: 90vw;
  max-width: 600px;
  max-height: 80vh;
  background: rgba(30, 41, 59, 0.95);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 1rem;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);
  z-index: 1000;
  overflow-y: auto;
  padding: 1.5rem;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.panel-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: white;
}

.panel-close {
  width: 2rem;
  height: 2rem;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.1);
  border: none;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.panel-close:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: rotate(90deg);
}

.panel-section {
  margin-bottom: 2rem;
}

.section-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 1rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.theme-grid,
.background-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 1rem;
}

.theme-card,
.background-card {
  cursor: pointer;
  border-radius: 0.75rem;
  overflow: hidden;
  transition: all 0.3s ease;
  border: 2px solid transparent;
}

.theme-card:hover,
.background-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.3);
}

.theme-card.active,
.background-card.active {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.3);
}

.theme-preview,
.background-preview {
  width: 100%;
  aspect-ratio: 16 / 9;
  background: rgba(255, 255, 255, 0.1);
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgba(255, 255, 255, 0.5);
}

.theme-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.theme-info {
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.theme-name {
  font-size: 0.875rem;
  font-weight: 500;
  color: white;
}

.theme-badge {
  font-size: 0.75rem;
  padding: 0.25rem 0.5rem;
  background: rgba(59, 130, 246, 0.3);
  border: 1px solid rgba(59, 130, 246, 0.5);
  border-radius: 0.25rem;
  color: #93c5fd;
}

.background-name {
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  font-size: 0.875rem;
  color: white;
  text-align: center;
}

.setting-item {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.setting-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: white;
  cursor: pointer;
  flex: 1;
}

.interval-input {
  width: 80px;
  padding: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.25rem;
  color: white;
}

/* 过渡动画 */
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}

.slide-up-enter-from {
  opacity: 0;
  transform: translateY(20px);
}

.slide-up-leave-to {
  opacity: 0;
  transform: translateY(20px);
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

