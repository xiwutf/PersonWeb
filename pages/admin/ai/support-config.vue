<template>
  <ClientOnly>
    <div class="admin-support-config-page p-6">
      <div class="mb-6">
        <h1 class="text-2xl font-bold mb-2">客服智能体配置</h1>
        <p class="text-gray-600 dark:text-gray-400">配置客服智能体的系统提示词和 FAQ 列表</p>
      </div>

      <n-card>
        <n-form
          ref="formRef"
          :model="form"
          label-placement="top"
          require-mark-placement="right-hanging"
        >
          <n-form-item label="系统提示词" path="systemPrompt">
            <n-input
              v-model:value="form.systemPrompt"
              type="textarea"
              :rows="6"
              placeholder="输入客服智能体的系统提示词，用于定义客服的身份和回答风格"
            />
            <template #feedback>
              <span class="text-sm text-gray-500">提示词将作为 AI 的基础指令，影响客服的回答风格和内容</span>
            </template>
          </n-form-item>

          <n-form-item label="FAQ 列表">
            <div class="faq-list">
              <div
                v-for="(faq, index) in form.faqList"
                :key="index"
                class="faq-item"
              >
                <n-input
                  v-model:value="faq.question"
                  placeholder="问题"
                  class="mb-2"
                />
                <n-input
                  v-model:value="faq.answer"
                  type="textarea"
                  :rows="2"
                  placeholder="答案"
                />
                <n-select
                  v-model:value="faq.category"
                  placeholder="分类"
                  :options="categoryOptions"
                  class="mt-2"
                  style="width: 150px;"
                />
                <n-button
                  text
                  type="error"
                  @click="removeFaq(index)"
                  class="mt-2"
                >
                  <template #icon>
                    <i class="fas fa-trash"></i>
                  </template>
                  删除
                </n-button>
              </div>
              <n-button
                dashed
                block
                @click="addFaq"
                class="mt-2"
              >
                <template #icon>
                  <i class="fas fa-plus"></i>
                </template>
                添加 FAQ
              </n-button>
            </div>
          </n-form-item>

          <n-form-item>
            <n-button
              type="primary"
              :loading="saving"
              @click="handleSave"
              size="large"
            >
              <template #icon>
                <i class="fas fa-save"></i>
              </template>
              保存配置
            </n-button>
            <n-button
              class="ml-2"
              @click="handleReset"
              :disabled="saving"
            >
              重置
            </n-button>
          </n-form-item>
        </n-form>
      </n-card>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { NCard, NForm, NFormItem, NInput, NButton, NSelect } from 'naive-ui'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

const api = useApi()
const message = useSafeMessage()

const formRef = ref<any>(null)
const saving = ref(false)

const form = ref({
  systemPrompt: '',
  faqList: [] as Array<{ question: string; answer: string; category?: string }>
})

const categoryOptions = [
  { label: '通用', value: 'general' },
  { label: '服务', value: 'service' },
  { label: '价格', value: 'pricing' },
  { label: '工具', value: 'tool' }
]

// 加载配置
const loadConfig = async () => {
  try {
    const res = await api.get('/admin/ai/support-config')
    if (res) {
      form.value.systemPrompt = res.systemPrompt || ''
      form.value.faqList = res.faqList || []
    }
  } catch (e: any) {
    console.error('加载配置失败:', e)
    message.error('加载配置失败')
  }
}

// 保存配置
const handleSave = async () => {
  saving.value = true
  try {
    await api.post('/admin/ai/support-config', {
      systemPrompt: form.value.systemPrompt,
      faqList: form.value.faqList
    })
    message.success('配置保存成功！')
  } catch (e: any) {
    console.error('保存配置失败:', e)
    message.error(e.response?.data?.message || e.message || '保存配置失败')
  } finally {
    saving.value = false
  }
}

// 重置
const handleReset = () => {
  loadConfig()
}

// 添加 FAQ
const addFaq = () => {
  form.value.faqList.push({
    question: '',
    answer: '',
    category: 'general'
  })
}

// 删除 FAQ
const removeFaq = (index: number) => {
  form.value.faqList.splice(index, 1)
}

onMounted(() => {
  loadConfig()
})

useHead({
  title: '客服智能体配置 - 后台管理',
  meta: [
    { name: 'description', content: '配置客服智能体的系统提示词和 FAQ 列表' }
  ]
})
</script>

<style scoped>
.admin-support-config-page {
  max-width: 1200px;
  margin: 0 auto;
}

.faq-list {
  width: 100%;
}

.faq-item {
  padding: 16px;
  border: 1px solid var(--n-border-color);
  border-radius: 8px;
  margin-bottom: 16px;
  background: var(--n-color);
}
</style>

