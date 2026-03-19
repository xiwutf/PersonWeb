<template>
  <div class="theme-switcher">
    <button
      @click="togglePanel"
      class="theme-switcher-button"
      :class="{ active: isOpen }"
      title="切换主题风格"
    >
      <i class="fas fa-palette"></i>
    </button>

    <Transition name="slide-up">
      <div v-if="isOpen" class="theme-switcher-panel">
        <div class="panel-header">
          <h3 class="panel-title">风格切换</h3>
          <button @click="closePanel" class="panel-close">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <div class="panel-section">
          <h4 class="section-title">主题风格</h4>
          <div class="theme-grid">
            <div
              v-for="theme in themes"
              :key="theme.code"
              @click="selectTheme(theme.code)"
              class="theme-card"
              :class="{ active: currentTheme === theme.code }"
              :data-theme-code="theme.code"
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

        <div class="panel-section">
          <h4 class="section-title">动态背景</h4>
          <div class="background-grid">
            <div
              v-for="bg in backgrounds"
              :key="bg.code"
              @click="selectBackground(bg.code)"
              class="background-card"
              :class="{ active: currentBackground === bg.code }"
            >
              <div class="background-preview" :class="`bg-${bg.code}`">
                <i class="fas fa-image"></i>
              </div>
              <div class="background-name">{{ bg.displayName }}</div>
            </div>
          </div>
        </div>

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

    <Transition name="fade">
      <div v-if="isOpen" @click="closePanel" class="theme-switcher-overlay"></div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useTheme } from '~/composables/useTheme'

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
const { currentTheme: themeState, setTheme } = useTheme()

const isOpen = ref(false)
const themes = ref<Theme[]>([])
const backgrounds = ref<Background[]>([])
const currentTheme = ref<string>('light')
const currentBackground = ref<string>('particles')
const autoSwitch = ref(false)
const switchInterval = ref(30)

let autoSwitchTimer: NodeJS.Timeout | null = null

const fetchThemes = async () => {
  themes.value = [
    {
      id: 1,
      code: 'light',
      displayName: '浅色主题',
      isDefault: false
    },
    {
      id: 2,
      code: 'dark',
      displayName: '深色主题',
      isDefault: true
    }
  ]
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
    currentTheme.value = themeState.value || 'light'

    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'
    const res = await api.post('/Theme/preference', { visitorId })
    if (res) {
      currentBackground.value = res.backgroundEffectCode || 'particles'
      autoSwitch.value = res.autoSwitch || false
      switchInterval.value = res.switchInterval || 30
    }
  } catch (e) {
    console.error('加载用户偏好失败', e)
  }
}

const selectTheme = async (themeCode: string) => {
  const normalizedTheme = themeCode === 'dark' ? 'dark' : 'light'
  currentTheme.value = normalizedTheme
  setTheme(normalizedTheme)

  try {
    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'
    await api.post('/Theme/preference/update', {
      visitorId,
      themeCode: normalizedTheme
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

const applyBackground = (bgCode: string) => {
  document.documentElement.classList.remove(
    'bg-particles',
    'bg-noise',
    'bg-waves',
    'bg-stars'
  )

  document.documentElement.classList.add(`bg-${bgCode}`)
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
    const currentIndex = themes.value.findIndex(theme => theme.code === currentTheme.value)
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
  currentTheme.value = themeState.value === 'dark' ? 'dark' : 'light'
  applyBackground(currentBackground.value)

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
  position: fixed !important;
  right: 0.5rem !important;
  bottom: 4rem !important;
  z-index: 10000 !important;
  isolation: isolate;
}

.theme-switcher-button {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 2.65rem;
  height: 2.65rem;
  border: 1px solid color-mix(in srgb, var(--color-primary) 35%, transparent);
  border-radius: 999px;
  background: var(--color-primary);
  color: #fff;
  box-shadow: var(--shadow-lg);
  transition: transform 0.2s ease, background 0.2s ease;
}

.theme-switcher-button:hover {
  background: var(--color-primary-hover);
  transform: scale(1.06);
}

.theme-switcher-button.active {
  background: var(--color-primary-hover);
}

.theme-switcher-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.36);
  backdrop-filter: blur(2px);
  z-index: 999;
}

.theme-switcher-panel {
  position: fixed;
  right: 2rem;
  bottom: 8rem;
  width: min(600px, calc(100vw - 3rem));
  max-height: 80vh;
  overflow-y: auto;
  padding: 1.5rem;
  background: var(--admin-surface-1, var(--color-bg-card));
  border: 1px solid var(--color-border);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-xl);
  z-index: 1000;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--color-border);
}

.panel-title {
  color: var(--color-text-main);
  font-size: 1.25rem;
  font-weight: 700;
}

.panel-close {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2rem;
  height: 2rem;
  border: 1px solid var(--color-border);
  border-radius: 999px;
  background: var(--admin-surface-2, var(--color-bg-elevated));
  color: var(--color-text-main);
}

.panel-section + .panel-section {
  margin-top: 1.75rem;
}

.section-title {
  margin-bottom: 1rem;
  color: var(--color-text-muted);
  font-size: 0.9rem;
  font-weight: 700;
}

.theme-grid,
.background-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 1rem;
}

.theme-card,
.background-card {
  overflow: hidden;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  background: var(--admin-surface-2, var(--color-bg-elevated));
  cursor: pointer;
  transition: transform 0.2s ease, border-color 0.2s ease, box-shadow 0.2s ease;
}

.theme-card:hover,
.background-card:hover {
  transform: translateY(-2px);
  border-color: var(--color-primary-soft);
}

.theme-card.active,
.background-card.active {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 1px color-mix(in srgb, var(--color-primary) 35%, transparent);
}

.theme-preview,
.background-preview {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  aspect-ratio: 16 / 9;
  background: var(--admin-surface-3, color-mix(in srgb, var(--color-border) 55%, transparent));
  color: var(--color-text-muted);
}

.theme-card[data-theme-code="light"] .theme-preview {
  background: linear-gradient(180deg, #f8fafc 0%, #e2e8f0 100%);
  color: #64748b;
}

.theme-card[data-theme-code="dark"] .theme-preview {
  background: linear-gradient(180deg, #0f172a 0%, #1e293b 100%);
  color: #cbd5e1;
}

.theme-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.theme-info,
.background-name {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem;
  background: var(--admin-surface-2, var(--color-bg-elevated));
  color: var(--color-text-main);
}

.theme-name,
.background-name {
  font-size: 0.86rem;
  font-weight: 600;
}

.theme-badge {
  padding: 0.2rem 0.45rem;
  border-radius: 999px;
  background: color-mix(in srgb, var(--color-primary) 16%, transparent);
  color: var(--color-primary-hover);
  font-size: 0.72rem;
}

.setting-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.setting-label {
  display: flex;
  align-items: center;
  gap: 0.55rem;
  color: var(--color-text-main);
}

.interval-input {
  width: 88px;
  padding: 0.55rem 0.7rem;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  background: var(--admin-surface-2, var(--color-bg-elevated));
  color: var(--color-text-main);
}

.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}

.slide-up-enter-from,
.slide-up-leave-to {
  opacity: 0;
  transform: translateY(18px);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

@media (max-width: 768px) {
  .theme-switcher-panel {
    right: 1rem;
    bottom: 7rem;
    width: calc(100vw - 2rem);
    padding: 1rem;
  }
}
</style>
