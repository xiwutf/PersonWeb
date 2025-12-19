<template>
  <n-modal
    v-model:show="visible"
    preset="card"
    :title="isEdit ? '编辑互动' : '新增互动'"
    style="width: 700px"
    @close="handleClose"
  >
    <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="100px">
      <n-form-item label="互动类型" path="type">
        <n-select
          v-model:value="form.type"
          :options="typeOptions"
          placeholder="请选择互动类型"
        />
      </n-form-item>

      <n-form-item label="发生时间" path="occurredAt">
        <n-date-picker
          v-model:value="form.occurredAt"
          type="datetime"
          format="yyyy-MM-dd HH:mm"
          placeholder="请选择发生时间"
          style="width: 100%"
        />
      </n-form-item>

      <n-form-item label="摘要" path="summary">
        <n-input
          v-model:value="form.summary"
          type="textarea"
          :rows="4"
          placeholder="简要描述这次互动的内容..."
          show-count
          :maxlength="500"
        />
      </n-form-item>

      <n-form-item label="我的感受" path="myFeeling">
        <n-radio-group v-model:value="form.myFeeling">
          <n-radio :value="0">
            <i class="fas fa-smile"></i> 正面
          </n-radio>
          <n-radio :value="1">
            <i class="fas fa-meh"></i> 中性
          </n-radio>
          <n-radio :value="2">
            <i class="fas fa-frown"></i> 负面
          </n-radio>
        </n-radio-group>
      </n-form-item>

      <n-divider />

      <div class="ai-section">
        <div class="ai-header">
          <span>AI 智能分析</span>
          <n-button
            size="small"
            :loading="aiLoading"
            @click="handleAiGenerate"
          >
            <template #icon>
              <i class="fas fa-magic"></i>
            </template>
            生成建议
          </n-button>
        </div>
        
        <div v-if="aiResult" class="ai-result">
          <div v-if="aiResult.summary?.oneLine" class="ai-summary">
            <strong>一句话总结：</strong>
            <div>{{ aiResult.summary.oneLine }}</div>
          </div>
          
          <div v-if="aiResult.summary?.keyFacts && aiResult.summary.keyFacts.length > 0" class="ai-key-facts">
            <strong>关键事实：</strong>
            <ul>
              <li v-for="(fact, index) in aiResult.summary.keyFacts" :key="index">{{ fact }}</li>
            </ul>
          </div>
          
          <div v-if="aiResult.nextActions && aiResult.nextActions.length > 0" class="ai-actions">
            <div class="actions-header">
              <strong>下一步行动：</strong>
            </div>
            <div
              v-for="(action, index) in aiResult.nextActions"
              :key="index"
              class="action-item"
            >
              <div class="action-content">
                <strong>{{ action.title }}</strong> ({{ action.when }}) - {{ action.why }}
              </div>
              <n-button
                size="small"
                type="primary"
                @click="setAsNextAction(action.title || '')"
              >
                一键设为下一步行动
              </n-button>
            </div>
          </div>
          
          <div v-if="aiResult.messageDrafts && aiResult.messageDrafts.length > 0" class="ai-message-drafts">
            <strong>消息草案（点击复制）：</strong>
            <div
              v-for="(draft, index) in aiResult.messageDrafts"
              :key="index"
              class="message-draft-item"
              :class="{ 'draft-empty': !draft.text }"
              @click="draft.text && copyMessage(draft.text)"
            >
              <div class="draft-scene">{{ draft.scene }}</div>
              <div class="draft-text">{{ draft.text || '（待生成）' }}</div>
              <i v-if="draft.text" class="fas fa-copy copy-icon"></i>
            </div>
          </div>
          
          <div v-if="aiResult.followupQuestions && aiResult.followupQuestions.length > 0" class="ai-questions">
            <strong>补充问题：</strong>
            <div class="questions-list">
              <div
                v-for="(question, index) in aiResult.followupQuestions"
                :key="index"
                class="question-item"
              >
                <span>{{ question }}</span>
              </div>
            </div>
            <div class="questions-hint">
              请手动补充这些信息后，可再次点击"生成建议"按钮
            </div>
          </div>
        </div>
      </div>
    </n-form>

    <template #footer>
      <div style="display: flex; justify-content: flex-end; gap: 8px;">
        <n-button @click="handleClose">取消</n-button>
        <n-button type="primary" :loading="loading" @click="handleSubmit">
          {{ isEdit ? '更新' : '创建' }}
        </n-button>
      </div>
    </template>
  </n-modal>
</template>

<script setup lang="ts">
import { ref, watch, reactive, computed } from 'vue'
import {
  NModal,
  NForm,
  NFormItem,
  NInput,
  NSelect,
  NDatePicker,
  NRadioGroup,
  NRadio,
  NButton,
  NDivider,
  useMessage,
  type FormInst
} from 'naive-ui'
import type {
  RelationPerson,
  RelationInteraction,
  CreateInteractionDto,
  UpdateInteractionDto,
  AiSummarizeResponse
} from '~/composables/useRelationsApi'

interface Props {
  show: boolean
  personId: string
  person?: RelationPerson
  interaction?: RelationInteraction
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:show': [value: boolean]
  success: []
}>()

const message = useMessage()
const formRef = ref<FormInst | null>(null)
const loading = ref(false)
const aiLoading = ref(false)
const aiResult = ref<AiSummarizeResponse | null>(null)

const visible = computed({
  get: () => props.show,
  set: (value) => emit('update:show', value)
})

const isEdit = computed(() => !!props.interaction)

const typeOptions = [
  { label: '文字', value: 0 },
  { label: '语音', value: 1 },
  { label: '通话', value: 2 },
  { label: '见面', value: 3 },
  { label: '其他', value: 4 }
]

const form = reactive<CreateInteractionDto & { occurredAt?: number; keyPoints?: Record<string, any> }>({
  type: 0,
  occurredAt: Date.now(),
  summary: '',
  myFeeling: undefined,
  keyPoints: undefined
})

const rules = {
  type: {
    required: true,
    message: '请选择互动类型',
    trigger: 'change'
  },
  occurredAt: {
    required: true,
    message: '请选择发生时间',
    trigger: 'change'
  },
  summary: {
    required: true,
    message: '请输入摘要',
    trigger: 'blur'
  }
}

watch(() => props.show, (show) => {
  if (show) {
    if (props.interaction) {
      // 编辑模式
      form.type = props.interaction.type
      form.occurredAt = new Date(props.interaction.occurredAt).getTime()
      form.summary = props.interaction.summary
      form.myFeeling = props.interaction.myFeeling
      aiResult.value = null
    } else {
      // 新增模式
      form.type = 0
      form.occurredAt = Date.now()
      form.summary = ''
      form.myFeeling = undefined
      aiResult.value = null
    }
  }
})

const handleClose = () => {
  visible.value = false
}

const handleAiGenerate = async () => {
  if (!form.summary.trim()) {
    message.warning('请先输入互动摘要')
    return
  }

  aiLoading.value = true
  try {
    const relationsApi = useRelationsApi()
    
    // 如果没有 person 信息，需要先获取
    let personInfo = props.person
    if (!personInfo) {
      personInfo = await relationsApi.getPerson(props.personId)
    }
    
    // 构建请求数据
    const request = {
      person: {
        nickname: personInfo.nickname,
        stage: personInfo.stage,
        tags: personInfo.tags,
        lastContactAt: personInfo.lastContactAt,
        lastMeetAt: personInfo.lastMeetAt,
        currentNextAction: personInfo.nextAction
      },
      historyKeyPoints: undefined as string | undefined, // TODO: 可以从历史互动记录中提取
      interaction: {
        type: form.type,
        occurredAt: new Date(form.occurredAt!).toISOString(),
        summary: form.summary,
        chatText: undefined as string | undefined // TODO: 如果用户有输入聊天片段，可以添加
      },
      userPreference: undefined as any
    }
    
    const result = await relationsApi.aiSummarize(request)
    
    aiResult.value = result
    
    // 将 key_facts 写入 keyPoints（用于后续保存到 interaction）
    if (result.summary?.keyFacts) {
      form.keyPoints = {
        key_facts: result.summary.keyFacts
      }
    }
    
    message.success('AI 建议生成成功')
  } catch (error: any) {
    message.error(error.message || '生成建议失败')
  } finally {
    aiLoading.value = false
  }
}

const copyMessage = async (text: string) => {
  if (!text) return
  try {
    await navigator.clipboard.writeText(text)
    message.success('已复制到剪贴板')
  } catch (error) {
    message.error('复制失败')
  }
}

const setAsNextAction = async (actionTitle: string) => {
  if (!actionTitle) {
    message.warning('行动标题为空')
    return
  }
  
  try {
    const relationsApi = useRelationsApi()
    await relationsApi.updatePerson(props.personId, {
      nextAction: actionTitle
    })
    message.success('已设为下一步行动，可在提交后查看')
  } catch (error: any) {
    message.error(error.message || '设置失败')
  }
}

const handleSubmit = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (errors) => {
    if (errors) return

    loading.value = true
    try {
      const relationsApi = useRelationsApi()
      const occurredAt = new Date(form.occurredAt!)

      if (isEdit.value && props.interaction) {
        const updateData: UpdateInteractionDto = {
          type: form.type,
          occurredAt: occurredAt.toISOString(),
          summary: form.summary,
          myFeeling: form.myFeeling,
          aiSuggestion: aiResult.value?.suggestion
        }
        await relationsApi.updateInteraction(props.interaction.id, updateData)
        message.success('更新成功')
      } else {
        const createData: CreateInteractionDto = {
          type: form.type,
          occurredAt: occurredAt.toISOString(),
          summary: form.summary,
          myFeeling: form.myFeeling,
          keyPoints: form.keyPoints
        }
        const created = await relationsApi.createInteraction(props.personId, createData)
        
        // 如果有 AI 建议的下一步行动，自动更新到对象的 nextAction
        if (aiResult.value?.nextActions && aiResult.value.nextActions.length > 0) {
          const firstAction = aiResult.value.nextActions[0]
          if (firstAction?.title) {
            try {
              await relationsApi.updatePerson(props.personId, {
                nextAction: firstAction.title
              })
            } catch (e) {
              // 静默失败，不影响互动记录的创建
            }
          }
        }
        
        message.success('创建成功')
      }

      emit('success')
      handleClose()
    } catch (error: any) {
      message.error(error.message || '操作失败')
    } finally {
      loading.value = false
    }
  })
}
</script>

<style scoped>
.ai-section {
  margin-top: 16px;
  padding: 12px;
  background: var(--color-info-50);
  border-radius: 4px;
}

.ai-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  font-weight: 600;
}

.ai-result {
  margin-top: 12px;
}

.ai-suggestion,
.ai-actions {
  margin: 8px 0;
  padding: 8px;
  background: white;
  border-radius: 4px;
}

.ai-suggestion strong,
.ai-actions strong {
  display: block;
  margin-bottom: 4px;
}

.ai-actions ul {
  margin: 4px 0 0 20px;
  padding: 0;
}

.ai-summary {
  margin: 8px 0;
  padding: 8px;
  background: white;
  border-radius: 4px;
}

.ai-key-facts {
  margin: 8px 0;
  padding: 8px;
  background: white;
  border-radius: 4px;
}

.ai-key-facts ul {
  margin: 4px 0 0 20px;
  padding: 0;
}

.ai-key-facts li {
  margin: 4px 0;
}

.ai-actions {
  margin: 8px 0;
  padding: 8px;
  background: white;
  border-radius: 4px;
}

.actions-header {
  margin-bottom: 8px;
}

.action-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin: 8px 0;
  padding: 8px;
  background: var(--color-info-50);
  border-radius: 4px;
  gap: 8px;
}

.action-content {
  flex: 1;
}

.ai-message-drafts {
  margin-top: 12px;
}

.message-draft-item {
  margin: 8px 0;
  padding: 12px;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  position: relative;
  transition: background-color 0.2s;
}

.message-draft-item:hover {
  background-color: var(--color-info-50);
}

.draft-scene {
  font-size: 12px;
  color: var(--text-color-2);
  margin-bottom: 4px;
}

.draft-text {
  color: var(--text-color);
  line-height: 1.5;
}

.copy-icon {
  position: absolute;
  top: 8px;
  right: 8px;
  color: var(--text-color-3);
  font-size: 14px;
}

.message-draft-item.draft-empty {
  opacity: 0.6;
  cursor: not-allowed;
}

.ai-questions {
  margin: 8px 0;
  padding: 8px;
  background: white;
  border-radius: 4px;
}

.questions-list {
  margin: 8px 0;
}

.question-item {
  margin: 4px 0;
  padding: 6px;
  background: var(--color-warning-50);
  border-radius: 4px;
  border-left: 3px solid var(--color-warning);
}

.questions-hint {
  margin-top: 8px;
  font-size: 12px;
  color: var(--text-color-2);
  font-style: italic;
}
</style>

