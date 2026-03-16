<template>
  <div class="themes-management-page">
    <div class="page-header">
      <h1 class="page-title">主题风格管理</h1>
      <p class="text-gray-400 text-sm">管理网站主题风格和动态背景效�?/p>
    </div>

    <!-- 标签�?-->
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
          动态背�?        </button>
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
            目前仅保留两套稳定主题，后续如果有需要再新增�?          </p>
          <!-- 暂时禁用新增按钮，因为只支持 light �?dark -->
          <!-- <button @click="openThemeModal()" class="btn-primary">
            <i class="fas fa-plus mr-2"></i>
            新增主题
          </button> -->
        </div>
      </div>

      <div v-if="loading" class="table-loading">加载�?..</div>
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

    <!-- 动态背景管�?-->
    <div v-if="activeTab === 'backgrounds'" class="content-section">
      <div class="section-header">
        <h2 class="section-title">动态背景效�?/h2>
      </div>

      <div v-if="loading" class="table-loading">加载�?..</div>
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
            <option value="light">浅色主题（light�?/option>
            <option value="dark">深色主题（dark�?/option>
          </select>
          <small class="form-hint">目前只支�?light �?dark 两个主题</small>
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
            <small class="form-hint">目前只支�?light �?dark 两个主题</small>
          </div>

          <div class="form-group">
            <label class="form-label">显示名称 *</label>
            <input v-model="themeForm.displayName" type="text" class="form-input" required />
          </div>

          <div class="form-group">
            <label class="form-label">预览�?URL</label>
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
import { onMounted, ref } from 'vue'
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
    // 重构说明�?024-12-XX）：现在只支�?light �?dark 两个主题
    // 从后端获取主题列表，但只显示 light �?dark
    const res = await api.get<Theme[]>('/Theme/admin/themes')
    const allThemes = Array.isArray(res) ? res : []
    
    // 只保�?light �?dark 主题，其他主题过滤掉
    themes.value = allThemes.filter(theme => 
      theme.code === 'light' || theme.code === 'dark'
    )
    
    // 如果数据库中没有 light �?dark，创建默认数�?    if (themes.value.length === 0) {
      themes.value = [
        {
          id: 0,
          name: '浅色主题',
          code: 'light',
          displayName: '浅色主题（light�?,
          description: '清爽明亮的浅色主题，适合日间使用',
          isEnabled: true,
          isDefault: true,
          sort: 1
        },
        {
          id: 0,
          name: '深色主题',
          code: 'dark',
          displayName: '深色主题（dark�?,
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
  // 重构说明�?024-12-XX）：只允许保�?light �?dark 主题
  if (themeForm.value.code !== 'light' && themeForm.value.code !== 'dark') {
    message.error('目前只支�?light �?dark 两个主题')
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
  if (!confirm('确定要删除这个主题吗�?)) return

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
  margin-bottom: var(--spacing-2xl);
}

.tabs {
  display: flex;
  gap: var(--spacing-sm);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.tab-button {
  padding: var(--spacing-md) var(--spacing-xl);
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
  color: var(--color-bg-card);
  border-bottom-color: var(--color-primary);
}

.content-section {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-md);
  padding: var(--spacing-xl);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
}

.section-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-bg-card);
}

.themes-grid,
.backgrounds-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: var(--spacing-xl);
}

.theme-card,
.background-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-md);
  padding: var(--spacing-10);
  transition: all 0.3s ease;
}

.theme-card:hover,
.background-card:hover {
  background: var(--color-border);
  border-color: rgba(255, 255, 255, 0.2);
}

.theme-card-inactive,
.background-card-inactive {
  opacity: 0.6;
}

.theme-card-default {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 2px var(--theme-primary);
}

.theme-card-header,
.background-card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-md);
}

.theme-card-title h3 {
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--color-bg-card);
  margin-bottom: var(--spacing-xs);
}

.theme-code {
  font-size: var(--text-xs);
  color: rgba(255, 255, 255, 0.6);
  font-family: monospace;
}

.theme-preview,
.background-preview {
  width: 100%;
  aspect-ratio: 16 / 9;
  background: rgba(0, 0, 0, 0.2);
  border-radius: var(--radius-sm);
  margin-bottom: var(--spacing-md);
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
  margin-top: var(--spacing-md);
}

.theme-description,
.background-description {
  font-size: var(--text-sm);
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: var(--spacing-md);
}

.theme-actions {
  display: flex;
  gap: var(--spacing-sm);
  padding-top: var(--spacing-md);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.badge {
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--radius-sm);
  font-size: var(--text-xs);
  font-weight: 500;
}

.badge-success {
  background: rgba(34, 197, 94, 0.3);
  border: 1px solid rgba(34, 197, 94, 0.6);
  color: var(--color-purple-300);
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
  margin-bottom: var(--spacing-xl);
}

.form-row {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--spacing-md);
}

.form-label {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  color: rgba(255, 255, 255, 0.9);
  font-size: var(--text-sm);
  font-weight: 500;
  margin-bottom: var(--spacing-sm);
}

.form-input {
  width: 100%;
  padding: var(--spacing-md);
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: var(--radius-sm);
  color: var(--color-bg-card);
  font-size: var(--text-sm);
}

.form-input:focus {
  outline: none;
  border-color: var(--color-primary);
  background: rgba(255, 255, 255, 0.15);
}

.form-hint {
  display: block;
  margin-top: var(--spacing-xs);
  font-size: var(--text-xs);
  color: rgba(255, 255, 255, 0.5);
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
  background: rgba(30, 41, 59, 0.95);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
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
  padding: var(--spacing-xl);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.modal-header h2 {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-bg-card);
}

.modal-close {
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.7);
  font-size: var(--text-3xl);
  cursor: pointer;
  width: var(--spacing-2xl);
  height: var(--spacing-2xl);
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: var(--radius-sm);
  transition: all 0.2s ease;
}

.modal-close:hover {
  background: rgba(255, 255, 255, 0.1);
  color: var(--color-bg-card);
}

.modal-body {
  padding: var(--spacing-xl);
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: var(--spacing-lg);
  padding-top: var(--spacing-xl);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  margin-top: var(--spacing-xl);
}

.btn-primary {
  padding: var(--spacing-sm) var(--spacing-md);
  background: var(--theme-primary);
  border: 1px solid var(--theme-primary);
  border-radius: var(--radius-sm);
  color: var(--color-bg-card);
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-primary:hover {
  background: rgba(59, 130, 246, 0.4);
}

.btn-secondary {
  padding: var(--spacing-sm) var(--spacing-md);
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: var(--radius-sm);
  color: rgba(255, 255, 255, 0.9);
  cursor: pointer;
}

.btn-link {
  background: none;
  border: none;
  cursor: pointer;
  font-size: var(--text-sm);
}

.btn-link-blue {
  color: var(--color-primary-soft);
}

.btn-link-red {
  color: var(--color-danger-400);
}
</style>

