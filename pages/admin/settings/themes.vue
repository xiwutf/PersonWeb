<template>
  <div class="themes-management-page">
    <div class="page-header">
      <h1 class="page-title">主题风格管理</h1>
      <div class="page-header-meta">
        <p class="page-subtitle">管理网站主题风格和动态背景效果</p>
        <span class="applied-theme-chip">
          当前应用：{{ currentAppliedTheme === 'dark' ? '深色主题' : '浅色主题' }}
        </span>
      </div>
    </div>

    <!-- 标签页 -->
    <div class="tabs-container">
      <div class="tabs">
        <button
          @click="activeTab = 'themes'"
          :class="['tab-button', { 'tab-button-active': activeTab === 'themes' }]"
        >
          主题风格
        </button>
        <button
          @click="activeTab = 'backgrounds'"
          :class="['tab-button', { 'tab-button-active': activeTab === 'backgrounds' }]"
        >
          动态背景
        </button>
        <button
          @click="activeTab = 'settings'"
          :class="['tab-button', { 'tab-button-active': activeTab === 'settings' }]"
        >
          全局设置
        </button>
      </div>
    </div>

    <!-- 主题风格管理 -->
    <div v-if="activeTab === 'themes'" class="content-section">
      <div class="section-header">
        <h2 class="section-title">主题风格列表</h2>
        <div class="flex items-center gap-4">
          <p class="text-sm text-gray-400">
            目前仅保留两套稳定主题，后续如果有需要再新增。
          </p>
          <!-- 暂时禁用新增按钮，因为只支持 light 和 dark -->
          <!-- <button @click="openThemeModal()" class="btn-primary">
            <i class="fas fa-plus mr-2"></i>
            新增主题
          </button> -->
        </div>
      </div>

      <div v-if="loading" class="table-loading">加载中...</div>
      <div v-else-if="themes.length === 0" class="table-empty">暂无主题</div>
      <div v-else class="themes-grid">
        <div
          v-for="theme in themes"
          :key="theme.id"
          class="theme-card"
          :class="{ 'theme-card-inactive': !theme.isEnabled, 'theme-card-default': theme.isDefault }"
        >
          <div class="theme-card-header">
            <div class="theme-card-title">
              <h3>{{ theme.displayName }}</h3>
              <span class="theme-code">{{ theme.code }}</span>
            </div>
            <div class="theme-card-badges">
              <span v-if="isCurrentTheme(theme)" class="badge badge-primary">当前使用</span>
              <span v-if="theme.isDefault" class="badge badge-success">默认</span>
              <span v-else-if="!theme.isEnabled" class="badge badge-default">禁用</span>
            </div>
          </div>

          <div v-if="theme.previewImage" class="theme-preview">
            <img :src="theme.previewImage" :alt="theme.displayName" />
          </div>
          <div v-else class="theme-preview theme-preview-placeholder">
            <i class="fas fa-image"></i>
          </div>

          <div class="theme-info">
            <p v-if="theme.description" class="theme-description">{{ theme.description }}</p>
            <div class="theme-actions">
              <button @click="openThemeModal(theme)" class="btn-link btn-link-blue">编辑</button>
              <button
                v-if="!isCurrentTheme(theme)"
                @click="setDefaultTheme(theme)"
                class="btn-link btn-link-blue"
              >
                设为默认
              </button>
              <button @click="deleteTheme(theme.id)" class="btn-link btn-link-red">删除</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 动态背景管理 -->
    <div v-if="activeTab === 'backgrounds'" class="content-section">
      <div class="section-header">
        <h2 class="section-title">动态背景效果</h2>
      </div>

      <div v-if="loading" class="table-loading">加载中...</div>
      <div v-else-if="backgrounds.length === 0" class="table-empty">暂无背景效果</div>
      <div v-else class="backgrounds-grid">
        <div
          v-for="bg in backgrounds"
          :key="bg.id"
          class="background-card"
          :class="{ 'background-card-inactive': !bg.isEnabled }"
        >
          <div class="background-card-header">
            <h3>{{ bg.displayName }}</h3>
            <span :class="bg.isEnabled ? 'badge badge-success' : 'badge badge-default'">
              {{ bg.isEnabled ? '启用' : '禁用' }}
            </span>
          </div>
          <div class="background-preview" :class="`bg-${bg.code}`">
            <i class="fas fa-image"></i>
          </div>
          <p v-if="bg.description" class="background-description">{{ bg.description }}</p>
        </div>
      </div>
    </div>

    <!-- 全局设置 -->
    <div v-if="activeTab === 'settings'" class="content-section">
      <div class="section-header">
        <h2 class="section-title">全局设置</h2>
      </div>

      <div class="settings-form">
        <div class="form-group">
          <label class="form-label">
            <input
              type="checkbox"
              v-model="settings.allowUserSwitch"
              @change="updateSetting('allow_user_switch', settings.allowUserSwitch)"
            />
            允许用户手动切换主题
          </label>
        </div>

        <div class="form-group">
          <label class="form-label">
            <input
              type="checkbox"
              v-model="settings.autoSwitchEnabled"
              @change="updateSetting('auto_switch_enabled', settings.autoSwitchEnabled)"
            />
            启用自动切换主题
          </label>
        </div>

        <div v-if="settings.autoSwitchEnabled" class="form-group">
          <label class="form-label">自动切换间隔（分钟）</label>
          <input
            type="number"
            v-model.number="settings.autoSwitchInterval"
            @change="updateSetting('auto_switch_interval', settings.autoSwitchInterval)"
            class="form-input"
            min="1"
          />
        </div>

        <div class="form-group">
          <label class="form-label">默认主题</label>
          <select
            v-model="settings.defaultTheme"
            @change="updateSetting('default_theme', settings.defaultTheme)"
            class="form-input"
          >
            <option value="light">浅色主题（light）</option>
            <option value="dark">深色主题（dark）</option>
          </select>
          <small class="form-hint">目前只支持 light 和 dark 两个主题</small>
        </div>

        <div class="form-group">
          <label class="form-label">默认背景效果</label>
          <select
            v-model="settings.defaultBackground"
            @change="updateSetting('default_background', settings.defaultBackground)"
            class="form-input"
          >
            <option v-for="bg in backgrounds" :key="bg.code" :value="bg.code">
              {{ bg.displayName }}
            </option>
          </select>
        </div>
      </div>
    </div>

    <!-- 主题编辑模态框 -->
    <div v-if="showThemeModal" class="modal-overlay" @click="showThemeModal = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ editingTheme?.id ? '编辑主题' : '新增主题' }}</h2>
          <button @click="showThemeModal = false" class="modal-close">×</button>
        </div>

        <form @submit.prevent="saveTheme" class="modal-body">
          <div class="form-group">
            <label class="form-label">主题名称 *</label>
            <input v-model="themeForm.name" type="text" class="form-input" required />
          </div>

          <div class="form-group">
            <label class="form-label">主题代码 *</label>
            <select v-model="themeForm.code" class="form-input" required>
              <option value="light">light（浅色主题）</option>
              <option value="dark">dark（深色主题）</option>
            </select>
            <small class="form-hint">目前只支持 light 和 dark 两个主题</small>
          </div>

          <div class="form-group">
            <label class="form-label">显示名称 *</label>
            <input v-model="themeForm.displayName" type="text" class="form-input" required />
          </div>

          <div class="form-group">
            <label class="form-label">预览图 URL</label>
            <input v-model="themeForm.previewImage" type="text" class="form-input" />
          </div>

          <div class="form-group">
            <label class="form-label">描述</label>
            <textarea v-model="themeForm.description" class="form-input" rows="3"></textarea>
          </div>

          <div class="form-group">
            <label class="form-label">样式配置 (JSON格式)</label>
            <textarea
              v-model="themeForm.styleConfig"
              class="form-input"
              rows="5"
              placeholder='{"primaryColor": "var(--color-primary)", "bgColor": "var(--color-bg-card)"}'
            ></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label">排序</label>
              <input v-model.number="themeForm.sort" type="number" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">
                <input v-model="themeForm.isEnabled" type="checkbox" />
                启用
              </label>
            </div>
            <div class="form-group">
              <label class="form-label">
                <input v-model="themeForm.isDefault" type="checkbox" />
                设为默认
              </label>
            </div>
          </div>

          <div class="modal-footer">
            <button type="button" @click="showThemeModal = false" class="btn-secondary">取消</button>
            <button type="submit" class="btn-primary">保存</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'
import { useTheme } from '~/composables/useTheme'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { handleError } = useErrorHandler()
const message = useSafeMessage()
const { setTheme } = useTheme()

interface Theme {
  id: number
  name: string
  code: 'light' | 'dark' | string
  displayName: string
  description?: string
  previewImage?: string
  isEnabled: boolean
  isDefault: boolean
  sort: number
  styleConfig?: string
}

interface Background {
  id: number
  name: string
  code: string
  displayName: string
  description?: string
  isEnabled: boolean
}

const activeTab = ref('themes')
const themes = ref<Theme[]>([])
const backgrounds = ref<Background[]>([])
const loading = ref(false)
const showThemeModal = ref(false)
const editingTheme = ref<Theme | null>(null)
const currentAppliedTheme = ref<'light' | 'dark'>('dark')

const settings = ref({
  allowUserSwitch: true,
  autoSwitchEnabled: false,
  autoSwitchInterval: 30,
  defaultTheme: 'default',
  defaultBackground: 'particles'
})

const themeForm = ref({
  id: 0,
  name: '',
  code: '',
  displayName: '',
  description: '',
  previewImage: '',
  isEnabled: true,
  isDefault: false,
  sort: 0,
  styleConfig: ''
})

const syncThemesWithAppliedTheme = (source: Theme[]) => {
  return source.map(theme => ({
    ...theme,
    isDefault: theme.code === currentAppliedTheme.value
  }))
}

const fetchThemes = async () => {
  loading.value = true
  try {
    // 重构说明（2024-12-XX）：现在只支持 light 和 dark 两个主题
    // 从后端获取主题列表，但只显示 light 和 dark
    const res = await api.get<Theme[]>('/Theme/admin/themes')
    const allThemes = Array.isArray(res) ? res : []
    
    // 只保留 light 和 dark 主题，其他主题过滤掉
      themes.value = syncThemesWithAppliedTheme(allThemes.filter(theme =>
        theme.code === 'light' || theme.code === 'dark'
      ))
    
    // 如果数据库中没有 light 和 dark，创建默认数据
      if (themes.value.length === 0) {
        themes.value = syncThemesWithAppliedTheme([
          {
            id: 0,
            name: '浅色主题',
            code: 'light',
            displayName: '浅色主题（light）',
            description: '清爽明亮的浅色主题，适合日间使用',
            isEnabled: true,
            isDefault: false,
            sort: 1
          },
          {
            id: 0,
            name: '深色主题',
            code: 'dark',
            displayName: '深色主题（dark）',
            description: '深色科技风格主题，适合夜间使用',
            isEnabled: true,
            isDefault: false,
            sort: 2
          }
        ])
      }
  } catch (e: unknown) {
    handleError(e, '加载主题失败')
  } finally {
    loading.value = false
  }
}

const fetchCurrentAppliedTheme = async () => {
  try {
    const res = await api.get<{ theme?: string }>('/Config/theme')
    const normalizedTheme = res?.theme === 'light' ? 'light' : 'dark'
    currentAppliedTheme.value = normalizedTheme
    settings.value.defaultTheme = normalizedTheme
  } catch (e: unknown) {
    handleError(e, '加载当前主题失败')
  }
}

const fetchBackgrounds = async () => {
  try {
    const res = await api.get<Background[]>('/Theme/backgrounds')
    backgrounds.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    handleError(e, '加载背景效果失败')
  }
}

const fetchSettings = async () => {
  try {
    const res = await api.get<Record<string, string>>('/Theme/settings')
    if (res) {
      settings.value = {
        allowUserSwitch: res.allow_user_switch === 'true',
        autoSwitchEnabled: res.auto_switch_enabled === 'true',
        autoSwitchInterval: parseInt(res.auto_switch_interval || '30'),
        defaultTheme: res.default_theme === 'light' ? 'light' : 'dark',
        defaultBackground: res.default_background || 'particles'
      }
    }
  } catch (e: unknown) {
    handleError(e, '加载设置失败')
  }
}

const isCurrentTheme = (theme: Theme) => {
  return theme.code === currentAppliedTheme.value
}

const openThemeModal = (theme?: Theme) => {
  if (theme) {
    editingTheme.value = theme
    themeForm.value = {
      id: theme.id,
      name: theme.name,
      code: theme.code,
      displayName: theme.displayName,
      description: theme.description || '',
      previewImage: theme.previewImage || '',
      isEnabled: theme.isEnabled,
      isDefault: theme.isDefault,
      sort: theme.sort,
      styleConfig: theme.styleConfig || ''
    }
  } else {
    editingTheme.value = null
    themeForm.value = {
      id: 0,
      name: '',
      code: '',
      displayName: '',
      description: '',
      previewImage: '',
      isEnabled: true,
      isDefault: false,
      sort: 0,
      styleConfig: ''
    }
  }
  showThemeModal.value = true
}

const saveTheme = async () => {
  // 重构说明（2024-12-XX）：只允许保存 light 或 dark 主题
  if (themeForm.value.code !== 'light' && themeForm.value.code !== 'dark') {
    message.error('目前只支持 light 和 dark 两个主题')
    return
  }
  
  try {
    await api.post('/Theme/admin/themes', themeForm.value)
    message.success('保存成功')
    showThemeModal.value = false
    fetchThemes()
  } catch (e: unknown) {
    handleError(e, '保存失败')
  }
}

const setDefaultTheme = async (theme: Theme) => {
  try {
    const nextTheme = theme.code === 'light' ? 'light' : 'dark'
    await api.put('/Config/theme', { theme: nextTheme })
    setTheme(nextTheme)
    currentAppliedTheme.value = nextTheme
    settings.value.defaultTheme = nextTheme

    if (theme.id > 0) {
      try {
        await api.post(`/Theme/admin/themes/${theme.id}/set-default`)
      } catch {
        // 主题样式表的“默认”标记属于辅助元数据，不阻断全局主题切换
      }
    }

    message.success('已设为默认并立即应用')
    await fetchThemes()
  } catch (e: unknown) {
    handleError(e, '设置失败')
  }
}

const deleteTheme = async (id: number) => {
  if (!confirm('确定要删除这个主题吗？')) return

  try {
    await api.del(`/Theme/admin/themes/${id}`)
    message.success('删除成功')
    fetchThemes()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

const updateSetting = async (key: string, value: any) => {
  try {
    if (key === 'default_theme') {
      const nextTheme = value === 'light' ? 'light' : 'dark'
      await api.put('/Config/theme', { theme: nextTheme })
      setTheme(nextTheme)
      currentAppliedTheme.value = nextTheme
      settings.value.defaultTheme = nextTheme
      await fetchThemes()
      message.success('默认主题已更新并立即生效')
      return
    }

    message.warning('该设置项暂未接入保存接口')
  } catch (e: unknown) {
    handleError(e, '更新设置失败')
  }
}

onMounted(async () => {
  await fetchSettings()
  await fetchCurrentAppliedTheme()
  await Promise.all([fetchThemes(), fetchBackgrounds()])
})
</script>

<style scoped>
.themes-management-page {
  width: 100%;
}

.page-header-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.page-subtitle {
  color: var(--color-text-muted);
  font-size: 0.875rem;
}

.applied-theme-chip {
  display: inline-flex;
  align-items: center;
  padding: 0.375rem 0.75rem;
  border-radius: 999px;
  border: 1px solid var(--color-border);
  background: var(--admin-surface-2, var(--color-bg-card));
  color: var(--color-text-main);
  font-size: 0.75rem;
  font-weight: 600;
}

.tabs-container {
  margin-bottom: 2rem;
}

.tabs {
  display: flex;
  gap: 0.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.tab-button {
  padding: 0.75rem 1.5rem;
  background: transparent;
  border: none;
  border-bottom: 2px solid transparent;
  color: var(--color-text-muted);
  cursor: pointer;
  transition: all 0.2s ease;
}

.tab-button:hover {
  color: var(--color-text-main);
}

.tab-button-active {
  color: var(--color-text-main);
  border-bottom-color: var(--color-primary);
}

.content-section {
  background: var(--admin-surface-1, rgba(255, 255, 255, 0.05));
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: 1.5rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--color-text-main);
}

.themes-grid,
.backgrounds-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.theme-card,
.background-card {
  background: var(--admin-surface-2, rgba(255, 255, 255, 0.05));
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: 1.25rem;
  transition: all 0.3s ease;
}

.theme-card:hover,
.background-card:hover {
  background: var(--admin-surface-3, var(--color-border));
  border-color: var(--color-primary-soft);
}

.theme-card-inactive,
.background-card-inactive {
  opacity: 0.6;
}

.theme-card-default {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 1px color-mix(in srgb, var(--color-primary) 40%, transparent);
}

.theme-card-header,
.background-card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.theme-card-title h3 {
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-text-main);
  margin-bottom: 0.25rem;
}

.theme-code {
  font-size: 0.75rem;
  color: var(--color-text-muted);
  font-family: monospace;
}

.theme-preview,
.background-preview {
  width: 100%;
  aspect-ratio: 16 / 9;
  background: rgba(0, 0, 0, 0.2);
  border-radius: 0.25rem;
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgba(255, 255, 255, 0.5);
  overflow: hidden;
}

.theme-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.theme-info {
  margin-top: 1rem;
}

.theme-description,
.background-description {
  font-size: 0.875rem;
  color: var(--color-text-muted);
  margin-bottom: 1rem;
}

.theme-actions {
  display: flex;
  gap: 0.5rem;
  padding-top: 1rem;
  border-top: 1px solid var(--color-border);
}

.badge {
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.badge-success {
  background: color-mix(in srgb, var(--color-success) 18%, transparent);
  border: 1px solid color-mix(in srgb, var(--color-success) 45%, transparent);
  color: var(--color-success);
}

.badge-primary {
  background: color-mix(in srgb, var(--color-primary) 16%, transparent);
  border: 1px solid color-mix(in srgb, var(--color-primary) 45%, transparent);
  color: var(--color-primary-hover);
}

.badge-default {
  background: color-mix(in srgb, var(--color-border) 65%, transparent);
  border: 1px solid var(--color-border);
  color: var(--color-text-main);
}

.settings-form {
  max-width: 600px;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.form-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--color-text-main);
  font-size: 0.875rem;
  font-weight: 500;
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  background: var(--admin-surface-2, rgba(255, 255, 255, 0.1));
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  color: var(--color-text-main);
  font-size: 0.875rem;
}

.form-input:focus {
  outline: none;
  border-color: var(--color-primary);
  background: var(--admin-surface-3, rgba(255, 255, 255, 0.15));
}

.form-hint {
  display: block;
  margin-top: 0.25rem;
  font-size: 0.75rem;
  color: var(--color-text-muted);
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: var(--overlay-color, rgba(0, 0, 0, 0.7));
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: var(--admin-surface-1, rgba(30, 41, 59, 0.95));
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid var(--color-border);
}

.modal-header h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--color-text-main);
}

.modal-close {
  background: none;
  border: none;
  color: var(--color-text-muted);
  font-size: 1.5rem;
  cursor: pointer;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.25rem;
  transition: all 0.2s ease;
}

.modal-close:hover {
  background: var(--admin-surface-2, rgba(255, 255, 255, 0.1));
  color: var(--color-text-main);
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding-top: 1.5rem;
  border-top: 1px solid var(--color-border);
  margin-top: 1.5rem;
}

.btn-primary {
  padding: 0.5rem 1rem;
  background: var(--color-primary);
  border: 1px solid var(--color-primary);
  border-radius: var(--radius-md);
  color: #fff;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-primary:hover {
  background: var(--color-primary-hover);
}

.btn-secondary {
  padding: 0.5rem 1rem;
  background: var(--admin-surface-2, rgba(255, 255, 255, 0.1));
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  color: var(--color-text-main);
  cursor: pointer;
}

.btn-link {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 0.875rem;
}

.btn-link-blue {
  color: var(--color-primary-hover);
}

.btn-link-red {
  color: var(--color-error);
}
</style>

