<template>
  <div class="themes-management-page">
    <div class="page-header">
      <h1 class="page-title">主题风格管理</h1>
      <p class="text-gray-400 text-sm">管理网站主题风格和动态背景效果</p>
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
                v-if="!theme.isDefault"
                @click="setDefaultTheme(theme.id)"
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
              placeholder='{"primaryColor": "#3b82f6", "bgColor": "#ffffff"}'
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

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { handleError } = useErrorHandler()
const message = useSafeMessage()

interface Theme {
  id: number
  name: string
  code: string
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

const fetchThemes = async () => {
  loading.value = true
  try {
    // 重构说明（2024-12-XX）：现在只支持 light 和 dark 两个主题
    // 从后端获取主题列表，但只显示 light 和 dark
    const res = await api.get<Theme[]>('/Theme/admin/themes')
    const allThemes = Array.isArray(res) ? res : []
    
    // 只保留 light 和 dark 主题，其他主题过滤掉
    themes.value = allThemes.filter(theme => 
      theme.code === 'light' || theme.code === 'dark'
    )
    
    // 如果数据库中没有 light 和 dark，创建默认数据
    if (themes.value.length === 0) {
      themes.value = [
        {
          id: 0,
          name: '浅色主题',
          code: 'light',
          displayName: '浅色主题（light）',
          description: '清爽明亮的浅色主题，适合日间使用',
          isEnabled: true,
          isDefault: true,
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
      ]
    }
  } catch (e: unknown) {
    handleError(e, '加载主题失败')
  } finally {
    loading.value = false
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
        defaultTheme: res.default_theme || 'default',
        defaultBackground: res.default_background || 'particles'
      }
    }
  } catch (e: unknown) {
    handleError(e, '加载设置失败')
  }
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

const setDefaultTheme = async (id: number) => {
  try {
    await api.post(`/Theme/admin/themes/${id}/set-default`)
    message.success('设置成功')
    fetchThemes()
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
    // TODO: 实现设置更新API
    console.log('更新设置', key, value)
  } catch (e: unknown) {
    handleError(e, '更新设置失败')
  }
}

onMounted(() => {
  fetchThemes()
  fetchBackgrounds()
  fetchSettings()
})
</script>

<style scoped>
.themes-management-page {
  width: 100%;
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
  color: rgba(255, 255, 255, 0.7);
  cursor: pointer;
  transition: all 0.2s ease;
}

.tab-button:hover {
  color: rgba(255, 255, 255, 0.9);
}

.tab-button-active {
  color: #ffffff;
  border-bottom-color: #3b82f6;
}

.content-section {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
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
  color: #ffffff;
}

.themes-grid,
.backgrounds-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.theme-card,
.background-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  padding: 1.25rem;
  transition: all 0.3s ease;
}

.theme-card:hover,
.background-card:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: rgba(255, 255, 255, 0.2);
}

.theme-card-inactive,
.background-card-inactive {
  opacity: 0.6;
}

.theme-card-default {
  border-color: #3b82f6;
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.3);
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
  color: #ffffff;
  margin-bottom: 0.25rem;
}

.theme-code {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.6);
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
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 1rem;
}

.theme-actions {
  display: flex;
  gap: 0.5rem;
  padding-top: 1rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.badge {
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.badge-success {
  background: rgba(34, 197, 94, 0.3);
  border: 1px solid rgba(34, 197, 94, 0.6);
  color: #a7f3d0;
}

.badge-default {
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: rgba(255, 255, 255, 0.9);
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
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.875rem;
  font-weight: 500;
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.25rem;
  color: #ffffff;
  font-size: 0.875rem;
}

.form-input:focus {
  outline: none;
  border-color: #3b82f6;
  background: rgba(255, 255, 255, 0.15);
}

.form-hint {
  display: block;
  margin-top: 0.25rem;
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: rgba(30, 41, 59, 0.95);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.75rem;
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
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.modal-header h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #ffffff;
}

.modal-close {
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.7);
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
  background: rgba(255, 255, 255, 0.1);
  color: #ffffff;
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  margin-top: 1.5rem;
}

.btn-primary {
  padding: 0.5rem 1rem;
  background: rgba(59, 130, 246, 0.3);
  border: 1px solid rgba(59, 130, 246, 0.5);
  border-radius: 0.25rem;
  color: #ffffff;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-primary:hover {
  background: rgba(59, 130, 246, 0.4);
}

.btn-secondary {
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.25rem;
  color: rgba(255, 255, 255, 0.9);
  cursor: pointer;
}

.btn-link {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 0.875rem;
}

.btn-link-blue {
  color: #60a5fa;
}

.btn-link-red {
  color: #f87171;
}
</style>

