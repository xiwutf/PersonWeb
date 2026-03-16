<template>
  <div>
    <h1 class="page-title mb-6">????</h1>

    <!-- ?????? -->
    <AppCard class="mb-6">
      <div class="space-y-4">
        <h2 class="text-xl font-bold text-text-main mb-4">
          <i class="fas fa-globe mr-2"></i>
          ????
        </h2>
        <p class="text-sm text-text-muted mb-4">
          ??????????????????????        </p>
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
            {{ savingGlobalTheme ? '???...' : '??????' }}
          </AppButton>
        </div>
      </div>
    </AppCard>

    <!-- ?????? -->
    <AppCard class="mb-6">
      <div class="space-y-4">
        <h2 class="text-xl font-bold text-text-main mb-4">
          <i class="fas fa-puzzle-piece mr-2"></i>
          ??????
        </h2>
        <p class="text-sm text-text-muted mb-4">
          ??????????????"????"??????????        </p>

        <div v-if="loadingModules" class="text-center py-8 text-text-muted">
          ????..
        </div>

        <div v-else class="space-y-4">
          <div
            v-for="module in moduleList"
            :key="module.id"
            class="flex items-center gap-4 p-4 border border-border-subtle rounded-lg"
          >
            <div class="flex-1">
              <div class="font-medium text-text-main">{{ module.name }}</div>
              <div class="text-xs text-text-muted mt-1">?? ID: {{ module.id }}</div>
              <div v-if="module.description" class="text-sm text-text-muted mt-1">
                {{ module.description }}
              </div>
            </div>
            <select
              v-model="moduleThemes[module.id]"
              class="form-input w-48"
              :disabled="savingModules"
            >
              <option :value="null">????</option>
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
              {{ savingModules ? '???...' : '??????' }}
            </AppButton>
          </div>
        </div>
      </div>
    </AppCard>

    <!-- ?? Tokens ?? -->
    <AppCard>
      <div class="space-y-4">
        <h2 class="text-xl font-bold text-text-main mb-4">
          <i class="fas fa-sliders-h mr-2"></i>
          ???????Tokens??        </h2>
        <p class="text-sm text-text-muted mb-4">
          ???????????????????????????        </p>

        <!-- ???????? -->
        <div class="flex gap-2 items-center mb-4">
          <label class="text-text-main font-medium">?????</label>
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
          ????..
        </div>

        <div v-else class="space-y-4">
          <!-- Tokens ???? -->
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
              {{ savingTokens ? '???...' : '?? Tokens' }}
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
import AppCard from '~/components/ui/AppCard.vue'
import AppButton from '~/components/ui/AppButton.vue'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // ?? SSR ??? Naive UI
})

const api = useApi()
const { success } = useNotification()
const { handleError } = useErrorHandler()

/**
 * ??????
 * 
 * ??????024-12-XX??
 * - ??????light ??dark ????
 * - ????tech-blue?paper?forest ?????? */
const themeOptions = [
  { label: '?????light?', value: 'light' },
  { label: '?????dark?', value: 'dark' }
] as const

// ????
// ??????024-12-XX????????light ??dark ????
const globalTheme = ref<'light' | 'dark'>('light')
const savingGlobalTheme = ref(false)

// ????
const moduleList = ref(MODULE_DEFINITIONS)
const moduleThemes = ref<Record<string, string | null>>({})
// ??????????????????????
// ??????024-12-XX????????light ??dark ????
const availableThemes = ref<string[]>(['light', 'dark'])
const loadingModules = ref(false)
const savingModules = ref(false)

// ?? Tokens
// ??????024-12-XX????????light ??dark ????
const selectedThemeForTokens = ref<'light' | 'dark'>('light')
const editingTokens = ref<Record<string, string>>({})
const loadingTokens = ref(false)
const savingTokens = ref(false)

// ???? token ????????????????
const editableTokenKeys = [
  'color.bg.body',
  'color.bg.card',
  'color.text.main',
  'color.text.muted',
  'color.primary',
  'color.primary.soft'
]

// ????????
// ?? themeOptions ??????
const getThemeDisplayName = (theme: string) => {
  const option = themeOptions.find(opt => opt.value === theme)
  return option?.label || theme
}

// ??? token ??
const formatTokenKey = (key: string) => {
  return key.replace(/\./g, ' ??')
}

// ????????token
const isColorToken = (key: string) => {
  return key.includes('color.')
}

// ?? token ???
const getDefaultTokenValue = (key: string) => {
  const theme = selectedThemeForTokens.value
  return defaultThemeTokens[theme]?.[key] || ''
}

// ??????
// ??????
const fetchGlobalTheme = async () => {
  try {
    const res = await api.get<{ theme: string }>('/Config/theme')
    if (res && res.theme) {
      // ???????? light ??dark
      if (res.theme === 'light' || res.theme === 'dark') {
        globalTheme.value = res.theme
      } else {
        // ???????? light
        globalTheme.value = 'light'
      }
    }
  } catch (e: unknown) {
    handleError(e, '??????')
  }
}

// ??????
const saveGlobalTheme = async () => {
  savingGlobalTheme.value = true
  try {
    await api.put<{ theme: string }>('/Config/theme', { theme: globalTheme.value })
    success('????')
  } catch (e: unknown) {
    handleError(e, '??????')
  } finally {
    savingGlobalTheme.value = false
  }
}

// ????????
const fetchModuleThemes = async () => {
  loadingModules.value = true
  try {
    const res = await api.get<{
      globalTheme: string
      modules: Array<{ moduleId: string; theme: string | null }>
      availableThemes: string[]
    }>('/Config/module-themes')
    
    if (res) {
      // ???????????????????????????      availableThemes.value = res.availableThemes || ['light', 'dark', 'tech-blue', 'paper', 'forest']
      
      // ??????????      const themesMap: Record<string, string | null> = {}
      moduleList.value.forEach(module => {
        const config = res.modules?.find(m => m.moduleId === module.id)
        themesMap[module.id] = config?.theme ?? null
      })
      moduleThemes.value = themesMap
    }
  } catch (e: unknown) {
    handleError(e, '????????')
  } finally {
    loadingModules.value = false
  }
}

// ????????
const saveModuleThemes = async () => {
  savingModules.value = true
  try {
    // ??????????
    const moduleConfigs = moduleList.value.map(module => ({
      moduleId: module.id,
      theme: moduleThemes.value[module.id] || null
    }))

    await api.put('/Config/module-themes', moduleConfigs)
    success('??????????')
  } catch (e: unknown) {
    handleError(e, '????????')
  } finally {
    savingModules.value = false
  }
}

// ???? tokens
const loadThemeTokens = async () => {
  loadingTokens.value = true
  try {
    const res = await api.get<{
      themeKey: string
      tokens: Record<string, string>
    }>(`/Config/theme-tokens/${selectedThemeForTokens.value}`)

    // ???? tokens ?????? tokens
    const defaultTokens = defaultThemeTokens[selectedThemeForTokens.value] || {}
    const backendTokens = res?.tokens || {}
    
    // ??????tokens?????????????????
    const merged: Record<string, string> = {}
    editableTokenKeys.forEach(key => {
      merged[key] = backendTokens[key] || defaultTokens[key] || ''
    })
    editingTokens.value = merged
  } catch (e: unknown) {
    handleError(e, '?? tokens ??')
  } finally {
    loadingTokens.value = false
  }
}

// ???? tokens
const saveThemeTokens = async () => {
  savingTokens.value = true
  try {
    // ???????? tokens
    const defaultTokens = defaultThemeTokens[selectedThemeForTokens.value] || {}
    const tokensToSave: Record<string, string> = {}
    
    editableTokenKeys.forEach(key => {
      const currentValue = editingTokens.value[key] || ''
      const defaultValue = defaultTokens[key] || ''
      // ????????
      if (currentValue && currentValue !== defaultValue) {
        tokensToSave[key] = currentValue
      }
    })

    await api.put(`/Config/theme-tokens/${selectedThemeForTokens.value}`, {
      themeKey: selectedThemeForTokens.value,
      tokens: tokensToSave
    })
    
    success('tokens ????')
  } catch (e: unknown) {
    handleError(e, '?? tokens ??')
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

