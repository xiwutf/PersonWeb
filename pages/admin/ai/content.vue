<template>
  <ClientOnly>
    <div class="admin-ai-content-page p-6">
      <div class="mb-6">
        <h1 class="text-2xl font-bold mb-2">???????</h1>
        <p class="text-gray-600 dark:text-gray-400">?? AI ?????????????????</p>
      </div>

      <n-card>
        <n-form
          ref="formRef"
          :model="form"
          :rules="rules"
          label-placement="left"
          label-width="120"
          require-mark-placement="right-hanging"
        >
          <n-form-item label="????" path="type">
            <n-radio-group v-model:value="form.type">
              <n-radio value="article">??</n-radio>
              <n-radio value="project_intro">????</n-radio>
              <n-radio value="tool_intro">????</n-radio>
            </n-radio-group>
          </n-form-item>

          <n-form-item label="??" path="title">
            <n-input
              v-model:value="form.title"
              placeholder="????????AI ??????"
              clearable
            />
          </n-form-item>

          <n-form-item label="????" path="tone">
            <n-select
              v-model:value="form.tone"
              :options="toneOptions"
              placeholder="??????"
            />
          </n-form-item>

          <n-form-item label="????" path="targetAudience">
            <n-input
              v-model:value="form.targetAudience"
              placeholder="???????????????????"
              clearable
            />
          </n-form-item>

          <n-form-item label="????" path="length">
            <n-select
              v-model:value="form.length"
              :options="lengthOptions"
              placeholder="??????"
            />
          </n-form-item>

          <n-form-item label="????" path="extraNotes">
            <n-input
              v-model:value="form.extraNotes"
              type="textarea"
              :rows="3"
              placeholder="?????????AI ?????????"
              clearable
            />
          </n-form-item>

          <n-form-item label="??????">
            <n-switch v-model:value="form.autoSaveDraft" />
            <span class="ml-2 text-sm text-gray-500">????????????</span>
          </n-form-item>

          <n-form-item>
            <n-button
              type="primary"
              :loading="generating"
              :disabled="generating"
              @click="handleGenerate"
              size="large"
            >
              <template #icon>
                <i class="fas fa-magic"></i>
              </template>
              ????
            </n-button>
            <n-button
              class="ml-2"
              @click="handleReset"
              :disabled="generating"
            >
              ??
            </n-button>
          </n-form-item>
        </n-form>
      </n-card>

      <!-- ???? -->
      <div v-if="result" class="mt-6 result-container">
        <n-card class="result-card" :class="{ 'result-visible': result }">
          <template #header>
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-semibold">
                <i class="fas fa-file-alt mr-2"></i>
                ????
                <span v-if="result.success" class="ml-2 text-sm text-green-600 dark:text-green-400">????</span>
                <span v-else class="ml-2 text-sm text-red-600 dark:text-red-400">????</span>
              </h3>
            <n-button
              v-if="result.content"
              type="primary"
              @click="handleSaveArticle"
              :loading="saving"
            >
              <template #icon>
                <i class="fas fa-save"></i>
              </template>
              ??????            </n-button>
          </div>
        </template>

        <div v-if="result.content" class="space-y-4">
          <div>
            <h4 class="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">??</h4>
            <p class="text-lg font-semibold">{{ result.content.title || '?????' }}</p>
          </div>

          <div v-if="result.content.summary">
            <h4 class="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">??</h4>
            <p class="text-gray-700 dark:text-gray-300 whitespace-pre-wrap">{{ result.content.summary }}</p>
          </div>

          <div>
            <h4 class="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">????</h4>
            <div v-if="result.content.body" class="prose dark:prose-invert max-w-none">
              <!-- ????? JSON ??????```json ?????????????-->
              <div v-if="result.content.body.trim().startsWith('```json')" class="p-4 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded">
                <p class="text-sm text-yellow-800 dark:text-yellow-200 mb-2">
                  ?? ??? JSON ??????????..
                </p>
                <div v-html="markdownToHtml(extractMarkdownFromJson(result.content.body))"></div>
              </div>
              <div v-else>
                <div v-html="markdownToHtml(result.content.body)"></div>
              </div>
            </div>
            <div v-else class="text-gray-500 italic">
              ??????
            </div>
            <!-- ????????????-->
            <details v-if="isDev && result.content.body" class="mt-4 text-xs">
              <summary class="cursor-pointer text-gray-500">??????????</summary>
              <pre class="mt-2 p-2 bg-gray-100 dark:bg-gray-800 rounded overflow-auto max-h-40">{{ result.content.body }}</pre>
            </details>
          </div>
        </div>
        
        <!-- ???????????? -->
        <div v-if="result && isDev" class="mt-4 p-3 bg-gray-100 dark:bg-gray-800 rounded text-xs">
          <strong>?????</strong>
          <pre class="mt-2 overflow-auto">{{ JSON.stringify(result, null, 2) }}</pre>
        </div>

        <n-alert 
          v-else-if="result.errorMessage" 
          type="error" 
          class="mt-4 error-alert"
          :show-icon="true"
          :closable="false"
        >
          <div class="error-message">
            <strong class="error-title">????</strong>
            <p class="error-detail">{{ result.errorMessage }}</p>
            <p v-if="result.errorMessage.includes('localhost:8001')" class="error-hint">
              ?? ?????? AI ?????????? <a href="http://localhost:8001/api/ai/health" target="_blank" class="error-link">http://localhost:8001/api/ai/health</a> ???????            </p>
          </div>
        </n-alert>
        </n-card>
      </div>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { NCard, NForm, NFormItem, NInput, NSelect, NRadioGroup, NRadio, NButton, NSwitch, NAlert } from 'naive-ui'
import { useMarkdown } from '~/composables/useMarkdown'
import { nextTick } from 'vue'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

const api = useApi()
const message = useSafeMessage()
const { parse: markdownToHtml } = useMarkdown()

// ????????????Nuxt 3 ??
const isDev = import.meta.dev

const formRef = ref<any>(null)
const generating = ref(false)
const saving = ref(false)
const result = ref<any>(null)

const form = ref({
  type: 'article',
  title: '',
  tone: 'mature',
  targetAudience: '',
  length: 'medium',
  extraNotes: '',
  autoSaveDraft: false
})

const toneOptions = [
  { label: '????', value: 'mature' },
  { label: '????', value: 'casual' },
  { label: '????', value: 'technical' }
]

const lengthOptions = [
  { label: '????00????', value: 'short' },
  { label: '????000????', value: 'medium' },
  { label: '????000????', value: 'long' }
]

const rules = {
  type: {
    required: true,
    message: '???????',
    trigger: 'change'
  }
}

const handleGenerate = async () => {
  console.log('=== ???????===')
  console.log('handleGenerate ??')
  console.log('generating ???', generating.value)
  
  if (!formRef.value) {
    console.error('formRef ???')
    message.error('???????????')
    return
  }

  console.log('???????????:', form.value)
  try {
    await formRef.value.validate()
    console.log('????????')
  } catch (e) {
    console.error('????????:', e)
    message.error('????????????')
    return
  }

  console.log('???????????:', JSON.stringify(form.value, null, 2))
  generating.value = true
  result.value = null

  try {
    const requestData = {
      type: form.value.type,
      title: form.value.title || undefined,
      tone: form.value.tone,
      targetAudience: form.value.targetAudience || undefined,
      length: form.value.length,
      extraNotes: form.value.extraNotes || undefined,
      autoSaveDraft: form.value.autoSaveDraft
    }
    console.log('?? ????? /ai/content/generate')
    console.log('????:', JSON.stringify(requestData, null, 2))
    
    const res = await api.post('/ai/content/generate', requestData)
    console.log('?? ?? API ??:', res)
    console.log('????:', typeof res)
    console.log('????:', JSON.stringify(res, null, 2))
    console.log('success ??:', res?.success, 'Success ??:', res?.Success)
    console.log('content ??:', res?.content, 'Content ??:', res?.Content)
    
    if (res && (res.success || res.Success)) {
      // ??????      const content = res.content ?? res.Content ?? null
      console.log('???????content:', content)
      console.log('content.title:', content?.title)
      console.log('content.summary:', content?.summary)
      console.log('content.body ??:', content?.body?.length)
      console.log('content.body ??00??:', content?.body?.substring(0, 200))
      
      if (!content) {
        console.error('??content ??null ??undefined')
        message.error('??????????')
        return
      }
      
      result.value = {
        success: res.success ?? res.Success ?? true,
        content: {
          title: content.title || '?????',
          summary: content.summary || '',
          body: content.body || ''
        },
        errorMessage: res.errorMessage ?? res.ErrorMessage
      }
      console.log('???? result.value:', result.value)
      console.log('result.value.content:', result.value.content)
      message.success('??????')
      
      // ?? DOM ??????????
      await nextTick()
      setTimeout(() => {
        const resultCard = document.querySelector('.result-card')
        if (resultCard) {
          resultCard.scrollIntoView({ behavior: 'smooth', block: 'start' })
        }
      }, 100)
    } else {
      console.warn('?? ??????success ???? false')
      result.value = {
        success: false,
        errorMessage: res?.errorMessage || res?.ErrorMessage || '????????'
      }
      message.error('??????')
    }
  } catch (e: any) {
    console.error('????????:', e)
    console.error('????:', {
      message: e.message,
      response: e.response,
      stack: e.stack
    })
    
    // ??????????
    let errorMsg = '????????'
    if (e.response?.data) {
      const errorData = e.response.data
      console.error('????:', errorData)
      errorMsg = errorData.message || errorData.errorMessage || errorMsg
      
      // ??????????????
      if (errorData.errorType === 'ConnectionError' || errorData.errorType === 'ServiceConnectionError') {
        errorMsg = 'AI ?????????? AI ??????http://localhost:8001?'
      } else if (errorData.errorType === 'TimeoutError') {
        errorMsg = 'AI ????????????'
      } else if (errorData.errorType === 'ServiceUnavailable') {
        errorMsg = 'AI ??????????AI ??????'
      }
    } else if (e.message) {
      errorMsg = e.message
    }
    
    result.value = {
      success: false,
      errorMessage: errorMsg
    }
    message.error(`?????${errorMsg}`)
  } finally {
    console.log('handleGenerate ???generating ?? false')
    generating.value = false
  }
}

const handleReset = () => {
  form.value = {
    type: 'article',
    title: '',
    tone: 'mature',
    targetAudience: '',
    length: 'medium',
    extraNotes: '',
    autoSaveDraft: false
  }
  result.value = null
  formRef.value?.restoreValidation()
}

// ??JSON ?????? Markdown ??
const extractMarkdownFromJson = (jsonStr: string): string => {
  try {
    // ???? ```json ????????    const jsonBlockStart = jsonStr.indexOf('```json')
    if (jsonBlockStart >= 0) {
      const contentStart = jsonStr.indexOf('\n', jsonBlockStart)
      const start = contentStart >= 0 ? contentStart + 1 : jsonBlockStart + 7
      const blockEnd = jsonStr.indexOf('```', start)
      if (blockEnd > start) {
        const jsonContent = jsonStr.substring(start, blockEnd).trim()
        try {
          const parsed = JSON.parse(jsonContent)
          // ???? body ??
          if (parsed.body) {
            return parsed.body
          }
          // ???? body ????????JSON ??????
          return JSON.stringify(parsed, null, 2)
        } catch (e) {
          console.warn('?? JSON ??:', e)
        }
      }
    }
    
    // ??????????????????
    try {
      const parsed = JSON.parse(jsonStr)
      if (parsed.body) {
        return parsed.body
      }
    } catch (e) {
      // ??????????????
    }
    
    return jsonStr
  } catch (e) {
    console.error('?? Markdown ??:', e)
    return jsonStr
  }
}

const handleSaveArticle = async () => {
  if (!result.value?.content) return

  saving.value = true

  try {
    // ??????body ??
    let bodyContent = result.value.content.body
    if (bodyContent && bodyContent.trim().startsWith('```json')) {
      bodyContent = extractMarkdownFromJson(bodyContent)
    }
    
    const res = await api.post('/Articles', {
      title: result.value.content.title,
      summary: result.value.content.summary,
      contentMd: bodyContent,
      status: 0 // ??
    })

    if (res && res.id) {
      message.success('?????')
      // ??????
      navigateTo(`/admin/articles/edit/${res.id}`)
    } else {
      message.error('????')
    }
  } catch (e: any) {
    console.error('??????:', e)
    message.error(e.response?.data?.message || e.message || '????')
  } finally {
    saving.value = false
  }
}
</script>

<style scoped>
.admin-ai-content-page {
  max-width: 1200px;
  margin: 0 auto;
}

.error-alert {
  font-size: var(--text-sm);
}

.error-message {
  line-height: 1.6;
}

.error-title {
  display: block;
  font-size: var(--text-base);
  font-weight: 600;
  margin-bottom: var(--spacing-2);
  color: var(--n-error-color);
}

.error-detail {
  font-size: var(--text-sm);
  margin-bottom: var(--spacing-2);
  color: var(--n-text-color);
  word-break: break-word;
}

.error-hint {
  font-size: var(--text-xs);
  margin-top: var(--spacing-3);
  padding-top: var(--spacing-3);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  color: var(--n-text-color-secondary);
}

.error-link {
  color: var(--n-primary-color);
  text-decoration: underline;
  font-weight: 500;
}

.error-link:hover {
  color: var(--n-primary-color-hover);
}

.result-container {
  position: relative;
}

.result-card {
  animation: fadeInUp 0.3s ease-out;
  min-height: var(--spacing-50);
}

.result-visible {
  border: var(--spacing-0_5) solid var(--n-primary-color);
  box-shadow: 0 var(--spacing-1) var(--spacing-3) var(--shadow);
}

.result-card::before {
  content: '';
  position: absolute;
  top: calc(var(--spacing-0_5) * -1);
  left: calc(var(--spacing-0_5) * -1);
  right: calc(var(--spacing-0_5) * -1);
  bottom: calc(var(--spacing-0_5) * -1);
  background: linear-gradient(135deg, var(--n-primary-color), var(--n-primary-color-hover));
  border-radius: inherit;
  z-index: -1;
  opacity: 0.1;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(var(--spacing-5));
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>

