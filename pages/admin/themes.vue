<template>
  <div>
    <h1 class="page-title mb-6">主题管理</h1>

    <!-- 全局主题配置 -->
    <AppCard class="mb-6">
      <div class="space-y-4">
        <h2 class="text-xl font-bold text-text-main mb-4">
          <i class="fas fa-globe mr-2"></i>
          全局主题
        </h2>
        <p class="text-sm text-text-muted mb-4">
          设置全站默认主题，所有模块默认跟随此主题。
        </p>
        <div class="flex gap-2 items-center">
          <select
            v-model="globalTheme"
            class="form-input flex-1"
            :disabled="savingGlobalTheme"
          >
            <option
              v-for="option in themeOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </option>
          </select>
          <AppButton
            variant="primary"
            @click="saveGlobalTheme"
            :disabled="savingGlobalTheme"
          >
            {{ savingGlobalTheme ? '保存中' : '保存全局主题' }}
          </AppButton>
        </div>
      </div>
    </AppCard>

    <!-- 模块主题配置 -->
    <AppCard class="mb-6">
      <div class="space-y-4">
        <h2 class="text-xl font-bold text-text-main mb-4">
          <i class="fas fa-puzzle-piece mr-2"></i>
          模块主题配置
        </h2>
        <p class="text-sm text-text-muted mb-4">
          为各个模块配置独立主题。选择"跟随全局"表示使用全局主题。
        </p>

        <div v-if="loadingModules" class="text-center py-8 text-text-muted">
          加载中...
        </div>

        <div v-else class="space-y-4">
          <div
            v-for="module in moduleList"
            :key="module.id"
            class="flex items-center gap-4 p-4 border border-border-subtle rounded-lg"
          >
            <div class="flex-1">
              <div class="font-medium text-text-main">{{ module.name }}</div>
              <div class="text-xs text-text-muted mt-1">模块 ID: {{ module.id }}</div>
              <div v-if="module.description" class="text-sm text-text-muted mt-1">
                {{ module.description }}
              </div>
            </div>
            <select
              v-model="moduleThemes[module.id]"
              class="form-input w-48"
              :disabled="savingModules"
            >
              <option :value="null">跟随全局</option>
              <option
                v-for="theme in availableThemes"
                :key="theme"
                :value="theme"
              >
                {{ getThemeDisplayName(theme) }}
              </option>
            </select>
          </div>

          <div class="flex justify-end pt-4 border-t border-border-subtle">
            <AppButton
              variant="primary"
              @click="saveModuleThemes"
              :disabled="savingModules"
            >
              {{ savingModules ? '保存中' : '保存模块主题' }}
            </AppButton>
          </div>
        </div>
      </div>
    </AppCard>

    <!-- 主题 Tokens 编辑 -->
    <AppCard>
      <div class="space-y-4">
        <h2 class="text-xl font-bold text-text-main mb-4">
          <i class="fas fa-sliders-h mr-2"></i>
          主题样式配置（Tokens）
        </h2>
        <p class="text-sm text-text-muted mb-4">
          细粒度管理各个主题的样式变量（颜色、圆角、阴影等）。
        </p>

        <!-- 选择要编辑的主题 -->
        <div class="flex gap-2 items-center mb-4">
          <label class="text-text-main font-medium">选择主题：</label>
          <select
            v-model="selectedThemeForTokens"
            class="form-input w-48"
            @change="loadThemeTokens"
          >
            <option
              v-for="option in themeOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </option>
          </select>
        </div>

        <div v-if="loadingTokens" class="text-center py-8 text-text-muted">
          加载中...
        </div>

        <div v-else class="space-y-4">
          <!-- Tokens 编辑表单 -->
          <div
            v-for="tokenKey in editableTokenKeys"
            :key="tokenKey"
            class="flex items-center gap-4"
          >
            <label class="w-48 text-text-main text-sm font-medium">
              {{ formatTokenKey(tokenKey) }}
            </label>
            <input
              v-model="editingTokens[tokenKey]"
              type="text"
              class="form-input flex-1"
              :placeholder="getDefaultTokenValue(tokenKey)"
            />
            <div
              v-if="isColorToken(tokenKey)"
              class="w-12 h-10 rounded border border-border-subtle"
              :style="{ backgroundColor: editingTokens[tokenKey] || getDefaultTokenValue(tokenKey) }"
            ></div>
          </div>

          <div class="flex justify-end pt-4 border-t border-border-subtle">
            <AppButton
              variant="primary"
              @click="saveThemeTokens"
              :disabled="savingTokens"
            >
              {{ savingTokens ? '保存中' : '保存 Tokens' }}
            </AppButton>
          </div>
        </div>
      </div>
    </AppCard>
  </div>
</template>

<script setup lang="ts">
import { MODULE_DEFINITIONS } from '~/constants/modules'
import { defaultThemeTokens } from '~/constants/design/tokens'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { success } = useNotification()
const { handleError } = useErrorHandler()

/**
 * 主题选项列表
 * 注意：value 必须与后端 ThemeKey 字符串完全一致，与前端 ThemeKey 类型保持一致
 * label 使用中文描述 + 英文 key，方便用户理解
 */
const themeOptions = [
  { label: '浅色（light）', value: 'light' },
  { label: '深色（dark）', value: 'dark' },
  { label: '科技蓝（tech-blue）', value: 'tech-blue' },
  { label: '纸张阅读（paper）', value: 'paper' },
  { label: '墨绿自然（forest）', value: 'forest' }
] as const

// 全局主题
const globalTheme = ref<'light' | 'dark' | 'tech-blue' | 'paper' | 'forest'>('light')
const savingGlobalTheme = ref(false)

// 模块主题
const moduleList = ref(MODULE_DEFINITIONS)
const moduleThemes = ref<Record<string, string | null>>({})
// 默认主题列表（会从后端获取，这里只是初始值）
const availableThemes = ref<string[]>(['light', 'dark', 'tech-blue', 'paper', 'forest'])
const loadingModules = ref(false)
const savingModules = ref(false)

// 主题 Tokens
const selectedThemeForTokens = ref<'light' | 'dark' | 'tech-blue' | 'paper' | 'forest'>('light')
const editingTokens = ref<Record<string, string>>({})
const loadingTokens = ref(false)
const savingTokens = ref(false)

// 可编辑的 token 键列表（作为起步，后续可以扩展）
const editableTokenKeys = [
  'color.bg.body',
  'color.bg.card',
  'color.text.main',
  'color.text.muted',
  'color.primary',
  'color.primary.soft'
]

// 获取主题显示名称
// 注意：这里使用 themeOptions 数组，确保与下拉框选项一致
const getThemeDisplayName = (theme: string) => {
  const option = themeOptions.find(opt => opt.value === theme)
  return option?.label || theme
}

// 格式化 token 键显示名称
const formatTokenKey = (key: string) => {
  return key.replace(/\./g, ' → ')
}

// 判断是否为颜色 token
const isColorToken = (key: string) => {
  return key.includes('color.')
}

// 获取默认 token 值
const getDefaultTokenValue = (key: string) => {
  const theme = selectedThemeForTokens.value
  return defaultThemeTokens[theme]?.[key] || ''
}

// 获取全局主题
const fetchGlobalTheme = async () => {
  try {
    const res = await api.get<{ theme: string }>('/Config/theme')
    if (res && res.theme) {
      // 验证主题值是否在支持的列表中
      const validThemes = ['light', 'dark', 'tech-blue', 'paper', 'forest'] as const
      if (validThemes.includes(res.theme as any)) {
        globalTheme.value = res.theme as typeof globalTheme.value
      }
    }
  } catch (e: unknown) {
    handleError(e, '获取全局主题失败')
  }
}

// 保存全局主题
const saveGlobalTheme = async () => {
  savingGlobalTheme.value = true
  try {
    await api.put<{ theme: string }>('/Config/theme', { theme: globalTheme.value })
    success('全局主题保存成功')
  } catch (e: unknown) {
    handleError(e, '保存全局主题失败')
  } finally {
    savingGlobalTheme.value = false
  }
}

// 获取模块主题配置
const fetchModuleThemes = async () => {
  loadingModules.value = true
  try {
    const res = await api.get<{
      globalTheme: string
      modules: Array<{ moduleId: string; theme: string | null }>
      availableThemes: string[]
    }>('/Config/module-themes')
    
    if (res) {
      // 使用后端返回的主题列表，如果后端没有返回则使用默认值
      availableThemes.value = res.availableThemes || ['light', 'dark', 'tech-blue', 'paper', 'forest']
      
      // 初始化模块主题映射
      const themesMap: Record<string, string | null> = {}
      moduleList.value.forEach(module => {
        const config = res.modules?.find(m => m.moduleId === module.id)
        themesMap[module.id] = config?.theme ?? null
      })
      moduleThemes.value = themesMap
    }
  } catch (e: unknown) {
    handleError(e, '获取模块主题配置失败')
  } finally {
    loadingModules.value = false
  }
}

// 保存模块主题配置
const saveModuleThemes = async () => {
  savingModules.value = true
  try {
    // 转换为后端需要的格式
    const moduleConfigs = moduleList.value.map(module => ({
      moduleId: module.id,
      theme: moduleThemes.value[module.id] || null
    }))

    await api.put('/Config/module-themes', moduleConfigs)
    success('模块主题配置保存成功')
  } catch (e: unknown) {
    handleError(e, '保存模块主题配置失败')
  } finally {
    savingModules.value = false
  }
}

// 加载主题 tokens
const loadThemeTokens = async () => {
  loadingTokens.value = true
  try {
    const res = await api.get<{
      themeKey: string
      tokens: Record<string, string>
    }>(`/Config/theme-tokens/${selectedThemeForTokens.value}`)

    // 合并默认 tokens 和后端返回的 tokens
    const defaultTokens = defaultThemeTokens[selectedThemeForTokens.value] || {}
    const backendTokens = res?.tokens || {}
    
    // 初始化编辑 tokens（使用默认值，后端覆盖的会被覆盖）
    const merged: Record<string, string> = {}
    editableTokenKeys.forEach(key => {
      merged[key] = backendTokens[key] || defaultTokens[key] || ''
    })
    editingTokens.value = merged
  } catch (e: unknown) {
    handleError(e, '加载主题 tokens 失败')
  } finally {
    loadingTokens.value = false
  }
}

// 保存主题 tokens
const saveThemeTokens = async () => {
  savingTokens.value = true
  try {
    // 只提交用户修改的 tokens（与默认值不同的）
    const defaultTokens = defaultThemeTokens[selectedThemeForTokens.value] || {}
    const tokensToSave: Record<string, string> = {}
    
    editableTokenKeys.forEach(key => {
      const currentValue = editingTokens.value[key] || ''
      const defaultValue = defaultTokens[key] || ''
      // 如果与默认值不同，才保存
      if (currentValue && currentValue !== defaultValue) {
        tokensToSave[key] = currentValue
      }
    })

    await api.put(`/Config/theme-tokens/${selectedThemeForTokens.value}`, {
      themeKey: selectedThemeForTokens.value,
      tokens: tokensToSave
    })
    
    success('主题 tokens 保存成功')
  } catch (e: unknown) {
    handleError(e, '保存主题 tokens 失败')
  } finally {
    savingTokens.value = false
  }
}

onMounted(() => {
  fetchGlobalTheme()
  fetchModuleThemes()
  loadThemeTokens()
})
</script>

