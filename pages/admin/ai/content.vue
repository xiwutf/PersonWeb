<template>
  <ClientOnly>
    <div class="admin-ai-content-page p-6">
      <div class="mb-6">
        <h1 class="text-2xl font-bold mb-2">内容生成智能�?/h1>
        <p class="text-gray-600 dark:text-gray-400">使用 AI 生成文章、项目介绍、工具介绍等内容</p>
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
          <n-form-item label="内容类型" path="type">
            <n-radio-group v-model:value="form.type">
              <n-radio value="article">文章</n-radio>
              <n-radio value="project_intro">项目介绍</n-radio>
              <n-radio value="tool_intro">工具介绍</n-radio>
            </n-radio-group>
          </n-form-item>

          <n-form-item label="标题" path="title">
            <n-input
              v-model:value="form.title"
              placeholder="输入标题（可选，AI 会自动生成）"
              clearable
            />
          </n-form-item>

          <n-form-item label="语气风格" path="tone">
            <n-select
              v-model:value="form.tone"
              :options="toneOptions"
              placeholder="选择语气风格"
            />
          </n-form-item>

          <n-form-item label="目标受众" path="targetAudience">
            <n-input
              v-model:value="form.targetAudience"
              placeholder="例如：技术开发者、产品经理、普通用户等"
              clearable
            />
          </n-form-item>

          <n-form-item label="字数要求" path="length">
            <n-select
              v-model:value="form.length"
              :options="lengthOptions"
              placeholder="选择字数要求"
            />
          </n-form-item>

          <n-form-item label="额外说明" path="extraNotes">
            <n-input
              v-model:value="form.extraNotes"
              type="textarea"
              :rows="3"
              placeholder="补充说明，帮�?AI 更好地理解你的需�?
              clearable
            />
          </n-form-item>

          <n-form-item label="自动保存草稿">
            <n-switch v-model:value="form.autoSaveDraft" />
            <span class="ml-2 text-sm text-gray-500">生成后自动保存为文章草稿</span>
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
              生成内容
            </n-button>
            <n-button
              class="ml-2"
              @click="handleReset"
              :disabled="generating"
            >
              重置
            </n-button>
          </n-form-item>
        </n-form>
      </n-card>

      <!-- 生成结果 -->
      <div v-if="result" class="mt-6 result-container">
        <n-card class="result-card" :class="{ 'result-visible': result }">
          <template #header>
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-semibold">
                <i class="fas fa-file-alt mr-2"></i>
                生成结果
                <span v-if="result.success" class="ml-2 text-sm text-green-600 dark:text-green-400">�?成功</span>
                <span v-else class="ml-2 text-sm text-red-600 dark:text-red-400">�?失败</span>
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
              保存为文�?            </n-button>
          </div>
        </template>

        <div v-if="result.content" class="space-y-4">
          <div>
            <h4 class="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">标题</h4>
            <p class="text-lg font-semibold">{{ result.content.title || '未命�? }}</p>
          </div>

          <div v-if="result.content.summary">
            <h4 class="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">摘要</h4>
            <p class="text-gray-700 dark:text-gray-300 var(--color-bg-light, white)space-pre-wrap">{{ result.content.summary }}</p>
          </div>

          <div>
            <h4 class="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">正文内容</h4>
            <div v-if="result.content.body" class="prose dark:prose-invert max-w-none">
              <!-- 检查是否是 JSON 字符串（�?```json 开头），如果是则尝试解�?-->
              <div v-if="result.content.body.trim().startsWith('```json')" class="p-4 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded">
                <p class="text-sm text-yellow-800 dark:text-yellow-200 mb-2">
                  ⚠️ 检测到 JSON 格式内容，正在解�?..
                </p>
                <div v-html="markdownToHtml(extractMarkdownFromJson(result.content.body))"></div>
              </div>
              <div v-else>
                <div v-html="markdownToHtml(result.content.body)"></div>
              </div>
            </div>
            <div v-else class="text-gray-500 italic">
              暂无正文内容
            </div>
            <!-- 原始内容预览（调试用�?-->
            <details v-if="isDev && result.content.body" class="mt-4 text-xs">
              <summary class="cursor-pointer text-gray-500">查看原始内容（调试）</summary>
              <pre class="mt-2 p-2 bg-gray-100 dark:bg-gray-800 rounded overflow-auto max-h-40">{{ result.content.body }}</pre>
            </details>
          </div>
        </div>
        
        <!-- 调试信息（开发环境显示） -->
        <div v-if="result && isDev" class="mt-4 p-3 bg-gray-100 dark:bg-gray-800 rounded text-xs">
          <strong>调试信息�?/strong>
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
            <strong class="error-title">生成失败</strong>
            <p class="error-detail">{{ result.errorMessage }}</p>
            <p v-if="result.errorMessage.includes('localhost:8001')" class="error-hint">
              💡 提示：请确保 AI 服务已启动，可以访问 <a href="http://localhost:8001/api/ai/health" target="_blank" class="error-link">http://localhost:8001/api/ai/health</a> 检查服务状�?            </p>
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

// 检查是否为开发环境（�?Nuxt 3 中）
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
  { label: '成熟专业', value: 'mature' },
  { label: '轻松随意', value: 'casual' },
  { label: '技术导�?, value: 'technical' }
]

const lengthOptions = [
  { label: '短篇�?00字左右）', value: 'short' },
  { label: '中篇�?000字左右）', value: 'medium' },
  { label: '长篇�?000字以上）', value: 'long' }
]

const rules = {
  type: {
    required: true,
    message: '请选择内容类型',
    trigger: 'change'
  }
}

const handleGenerate = async () => {
  console.log('=== 开始生成内�?===')
  console.log('handleGenerate 被调�?)
  console.log('generating 状�?', generating.value)
  
  if (!formRef.value) {
    console.error('formRef 为空，无法验证表�?)
    message.error('表单引用未初始化，请刷新页面重试')
    return
  }

  console.log('开始验证表单，表单数据:', form.value)
  try {
    await formRef.value.validate()
    console.log('�?表单验证通过')
  } catch (e) {
    console.error('�?表单验证失败:', e)
    message.error('请检查表单输入，确保已选择内容类型')
    return
  }

  console.log('开始生成内容，表单数据:', JSON.stringify(form.value, null, 2))
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
    console.log('📤 发送请求到 /ai/content/generate')
    console.log('请求数据:', JSON.stringify(requestData, null, 2))
    
    const res = await api.post('/ai/content/generate', requestData)
    console.log('📥 收到 API 响应:', res)
    console.log('响应类型:', typeof res)
    console.log('响应内容:', JSON.stringify(res, null, 2))
    console.log('success 字段:', res?.success, 'Success 字段:', res?.Success)
    console.log('content 字段:', res?.content, 'Content 字段:', res?.Content)
    
    if (res && (res.success || res.Success)) {
      // 兼容大小�?      const content = res.content ?? res.Content ?? null
      console.log('�?解析成功，content:', content)
      console.log('content.title:', content?.title)
      console.log('content.summary:', content?.summary)
      console.log('content.body 长度:', content?.body?.length)
      console.log('content.body �?00字符:', content?.body?.substring(0, 200))
      
      if (!content) {
        console.error('�?content �?null �?undefined')
        message.error('响应中缺少内容数�?)
        return
      }
      
      result.value = {
        success: res.success ?? res.Success ?? true,
        content: {
          title: content.title || '未命�?,
          summary: content.summary || '',
          body: content.body || ''
        },
        errorMessage: res.errorMessage ?? res.ErrorMessage
      }
      console.log('�?设置 result.value:', result.value)
      console.log('result.value.content:', result.value.content)
      message.success('内容生成成功�?)
      
      // 等待 DOM 更新后滚动到结果区域
      await nextTick()
      setTimeout(() => {
        const resultCard = document.querySelector('.result-card')
        if (resultCard) {
          resultCard.scrollIntoView({ behavior: 'smooth', block: 'start' })
        }
      }, 100)
    } else {
      console.warn('⚠️ 响应中没�?success 字段或为 false')
      result.value = {
        success: false,
        errorMessage: res?.errorMessage || res?.ErrorMessage || '生成失败，请重试'
      }
      message.error('内容生成失败')
    }
  } catch (e: any) {
    console.error('�?生成内容失败:', e)
    console.error('错误详情:', {
      message: e.message,
      response: e.response,
      stack: e.stack
    })
    
    // 提取更详细的错误信息
    let errorMsg = '生成失败，请重试'
    if (e.response?.data) {
      const errorData = e.response.data
      console.error('错误数据:', errorData)
      errorMsg = errorData.message || errorData.errorMessage || errorMsg
      
      // 根据错误类型提供更友好的提示
      if (errorData.errorType === 'ConnectionError' || errorData.errorType === 'ServiceConnectionError') {
        errorMsg = 'AI 服务连接失败，请检�?AI 服务是否正常运行（http://localhost:8001�?
      } else if (errorData.errorType === 'TimeoutError') {
        errorMsg = 'AI 服务请求超时，请稍后重试'
      } else if (errorData.errorType === 'ServiceUnavailable') {
        errorMsg = 'AI 服务不可用，请检�?AI 服务是否启动'
      }
    } else if (e.message) {
      errorMsg = e.message
    }
    
    result.value = {
      success: false,
      errorMessage: errorMsg
    }
    message.error(`内容生成失败�?{errorMsg}`)
  } finally {
    console.log('生成流程结束，重�?generating 状�?)
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

// �?JSON 字符串中提取 Markdown 内容
const extractMarkdownFromJson = (jsonStr: string): string => {
  try {
    // 尝试提取 ```json 代码块中的内�?    const jsonBlockStart = jsonStr.indexOf('```json')
    if (jsonBlockStart >= 0) {
      const contentStart = jsonStr.indexOf('\n', jsonBlockStart)
      const start = contentStart >= 0 ? contentStart + 1 : jsonBlockStart + 7
      const blockEnd = jsonStr.indexOf('```', start)
      if (blockEnd > start) {
        const jsonContent = jsonStr.substring(start, blockEnd).trim()
        try {
          const parsed = JSON.parse(jsonContent)
          // 如果解析成功，返�?body 字段的内�?          if (parsed.body) {
            return parsed.body
          }
          // 如果没有 body 字段，返回整�?JSON 的字符串表示
          return JSON.stringify(parsed, null, 2)
        } catch (e) {
          console.warn('解析 JSON 失败:', e)
        }
      }
    }
    
    // 如果提取失败，尝试直接解析整个字符串
    try {
      const parsed = JSON.parse(jsonStr)
      if (parsed.body) {
        return parsed.body
      }
    } catch (e) {
      // 如果都失败了，返回原始字符串
    }
    
    return jsonStr
  } catch (e) {
    console.error('提取 Markdown 失败:', e)
    return jsonStr
  }
}

const handleSaveArticle = async () => {
  if (!result.value?.content) return

  saving.value = true

  try {
    // 提取实际�?body 内容
    let bodyContent = result.value.content.body
    if (bodyContent && bodyContent.trim().startsWith('```json')) {
      bodyContent = extractMarkdownFromJson(bodyContent)
    }
    
    const res = await api.post('/Articles', {
      title: result.value.content.title,
      summary: result.value.content.summary,
      contentMd: bodyContent,
      status: 0 // 草稿状�?    })

    if (res && res.id) {
      message.success('文章已保存为草稿�?)
      // 跳转到编辑页�?      navigateTo(`/admin/articles/edit/${res.id}`)
    } else {
      message.error('保存失败')
    }
  } catch (e: any) {
    console.error('保存文章失败:', e)
    message.error(e.response?.data?.message || e.message || '保存失败')
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

