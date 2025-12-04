<template>
  <div>
    <h1 class="page-title mb-6">站点配置</h1>

    <div class="card p-6 max-w-2xl">
      <div v-if="loading" class="text-center py-8 loading">加载中...</div>
      
      <form v-else class="space-y-6" @submit.prevent>
        <!-- 站点主题配置区域 -->
        <div class="form-group bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
          <label class="form-label font-semibold">
            <i class="fas fa-palette mr-2"></i>
            站点主题
          </label>
          <p class="text-sm mb-3 form-description">
            此设置为全站主题，访客刷新页面后会看到新的主题风格。
          </p>
          <div class="flex gap-2 items-center">
            <select
              v-model="siteTheme"
              class="form-input flex-1"
              :disabled="savingTheme"
            >
              <option value="light">浅色 (Light)</option>
              <option value="dark">暗色 (Dark)</option>
              <option value="lab">实验室 (Lab)</option>
              <option value="tech-blue">科技蓝 (Tech Blue)</option>
              <option value="paper">纸张阅读 (Paper)</option>
              <option value="forest">墨绿自然 (Forest)</option>
              <option value="hybrid-super">Hybrid Super</option>
              <option value="hybrid-super-dark">Hybrid Super（深色）</option>
              <option value="hybrid-super-light">Hybrid Super（浅色）</option>
            </select>
            <button
              type="button"
              @click="saveTheme"
              class="btn-primary disabled:opacity-50 whitespace-nowrap"
              :disabled="savingTheme"
            >
              {{ savingTheme ? '保存中' : '保存主题' }}
            </button>
          </div>
        </div>

        <!-- 分隔线 -->
        <div class="border-t border-gray-200 dark:border-gray-700 my-6"></div>

        <!-- 其他配置项 -->
        <div v-for="(value, key) in configs" :key="key" class="form-group">
          <label class="form-label">{{ formatKey(key) }} ({{ key }})</label>
          <div class="flex gap-2">
            <input 
              v-model="configs[key]" 
              type="text" 
              class="form-input flex-1"
              :disabled="savingKey === key"
            />
            <button 
              @click="saveConfig(key as string)" 
              class="btn-primary disabled:opacity-50"
              :disabled="savingKey === key"
            >
              {{ savingKey === key ? '保存中' : '保存' }}
            </button>
          </div>
        </div>

        <!-- 添加新配置项 (可选) -->
        <div class="pt-6 border-t border-gray-100 dark:border-gray-700">
          <h3 class="text-sm font-bold mb-4 form-section-title">添加配置</h3>
          <div class="flex gap-2">
            <input v-model="newKey" type="text" placeholder="Key (e.g. site_title)" class="form-input flex-1" />
            <input v-model="newValue" type="text" placeholder="Value" class="form-input flex-1" />
            <button @click="addConfig" class="btn-success">添加</button>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const router = useRouter()
const api = useApi()

const loading = ref(false)
const savingKey = ref<string | null>(null)
const configs = ref<Record<string, string>>({})

const newKey = ref('')
const newValue = ref('')

// 站点主题相关状态
const siteTheme = ref<'light' | 'dark' | 'lab' | 'tech-blue' | 'paper' | 'forest' | 'hybrid-super' | 'hybrid-super-dark' | 'hybrid-super-light'>('light')
const savingTheme = ref(false)

const formatKey = (key: string | number) => {
  const k = String(key)
  const map: Record<string, string> = {
    site_title: '网站标题',
    site_subtitle: '网站副标题',
    icp_record: '备案号',
    home_page_size: '首页文章数'
  }
  return map[k] || k
}

const fetchConfigs = async () => {
  loading.value = true
  try {
    const res = await api.get<Record<string, string>>('/Config')
    if (res && typeof res === 'object') {
      configs.value = res
    } else {
      configs.value = {}
    }
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch config:', e)
    }
    configs.value = {}
  } finally {
    loading.value = false
  }
}

// 获取当前主题
const fetchTheme = async () => {
  try {
    const res = await api.get<{ theme: string }>('/Config/theme')
    if (res && res.theme) {
      const validThemes = ['light', 'dark', 'lab', 'tech-blue', 'paper', 'forest', 'hybrid-super', 'hybrid-super-dark', 'hybrid-super-light']
      if (validThemes.includes(res.theme)) {
        siteTheme.value = res.theme as any
      }
    }
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch theme:', e)
    }
    // 失败时使用默认值 light
    siteTheme.value = 'light'
  }
}

// 保存主题
const saveTheme = async () => {
  savingTheme.value = true
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    // 调用 PUT /api/Config/theme，传入 { theme }
    await api.put<{ theme: string }>('/Config/theme', { theme: siteTheme.value })
    
    // 保存成功后，立即切换前端主题
    const { setTheme } = useTheme()
    setTheme(siteTheme.value as any)
    
    success('主题保存成功，已立即切换到新主题')
  } catch (e: unknown) {
    handleError(e, '保存主题失败')
  } finally {
    savingTheme.value = false
  }
}

const saveConfig = async (key: string) => {
  savingKey.value = key
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    // 后端 API 使用 PUT /Config/{key}，body 为 { value: string }
    await api.put(`/Config/${key}`, { value: configs.value[key] })
    success('保存成功')
  } catch (e: unknown) {
    handleError(e, '保存失败')
  } finally {
    savingKey.value = null
  }
}

const addConfig = async () => {
  if (!newKey.value) return
  configs.value[newKey.value] = newValue.value
  await saveConfig(newKey.value)
  newKey.value = ''
  newValue.value = ''
}

onMounted(() => {
  fetchConfigs()
  fetchTheme()
})
</script>

<style scoped>
/* 确保表单标签文字清晰可见 */
.form-label {
  color: var(--color-text-main, #ffffff) !important;
  display: block;
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
}

.form-description {
  color: var(--color-text-muted, rgba(255, 255, 255, 0.7)) !important;
}

.form-section-title {
  color: var(--color-text-main, #ffffff) !important;
}

/* 确保下拉框文字清晰可见 */
.form-input {
  color: var(--color-text-main, #ffffff) !important;
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
}

.form-input:focus {
  background: rgba(255, 255, 255, 0.15) !important;
  border-color: var(--color-primary, #3b82f6) !important;
  outline: none;
}

/* 下拉框选项文字 - 增强样式确保清晰可见 */
select.form-input {
  color: var(--color-text-main, #ffffff) !important;
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  /* 确保下拉箭头可见 */
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23ffffff' d='M6 9L1 4h10z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  padding-right: 2.5rem;
}

/* 下拉框选项样式 - 使用深色背景和白色文字 */
select.form-input option {
  background: #1e293b !important;
  background-color: #1e293b !important;
  color: #ffffff !important;
  padding: 0.75rem 0.5rem !important;
  font-size: 0.875rem !important;
  font-weight: 400 !important;
  /* 确保文字不透明 */
  opacity: 1 !important;
}

/* 下拉框打开时的样式 */
select.form-input:focus {
  background: rgba(255, 255, 255, 0.15) !important;
  border-color: var(--color-primary, #3b82f6) !important;
}

/* 选中状态的选项 */
select.form-input option:checked {
  background: var(--color-primary, #3b82f6) !important;
  background-color: var(--color-primary, #3b82f6) !important;
  color: #ffffff !important;
}

/* 悬停状态的选项（某些浏览器支持） */
select.form-input option:hover {
  background: rgba(59, 130, 246, 0.3) !important;
  background-color: rgba(59, 130, 246, 0.3) !important;
  color: #ffffff !important;
}

/* 加载文字 */
.loading {
  color: var(--color-text-muted, rgba(255, 255, 255, 0.7)) !important;
}

/* 站点主题区域的背景和边框 */
.form-group.bg-blue-50 {
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.05)) !important;
  border-color: var(--color-border-subtle, rgba(255, 255, 255, 0.1)) !important;
}
</style>

