<template>
  <n-modal
    v-model:show="visible"
    preset="card"
    :title="isEdit ? '编辑互动' : '新增互动'"
    style="width: 700px"
    @close="handleClose"
  >
    <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="100px">
      <!-- 核心字段：摘要（必填，最显眼） -->
      <n-form-item label="摘要" path="summary" required>
        <n-input
          v-model:value="form.summary"
          type="textarea"
          :rows="3"
          placeholder="一句话描述这次互动...（必填，如：今天聊了工作，对方很感兴趣）"
          show-count
          :maxlength="500"
          autofocus
          @keydown.ctrl.enter="handleQuickSubmit"
          @keydown.meta.enter="handleQuickSubmit"
        />
        <template #feedback>
          <span style="font-size: 12px; color: var(--text-color-3);">
            支持 Ctrl/Cmd + Enter 快速保存
          </span>
        </template>
      </n-form-item>

      <!-- 可选字段：折叠显示 -->
      <n-collapse :default-expanded-names="[]">
        <n-collapse-item name="optional" title="更多选项（可选）">
          <div style="margin-top: 12px;">
            <n-form-item label="互动类型" path="type">
              <n-select
                v-model:value="form.type"
                :options="typeOptions"
                placeholder="默认：文字"
              />
            </n-form-item>

            <n-form-item label="发生时间" path="occurredAt">
              <n-date-picker
                v-model:value="form.occurredAt"
                type="datetime"
                format="yyyy-MM-dd HH:mm"
                placeholder="默认：当前时间"
                style="width: 100%"
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
          </div>
        </n-collapse-item>
      </n-collapse>

      <n-divider />

      <div class="ai-section">
        <div class="ai-header">
          <span>AI 智能分析</span>
          <n-button
            type="primary"
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

        <!-- AI 生成依据说明 -->
        <div v-if="props.person" class="ai-data-source">
          <ClientOnly>
            <n-collapse>
              <n-collapse-item title="AI 基于以下信息生成建议（点击展开）" name="source">
                <div class="source-info">
                  <div class="source-item">
                    <strong>对象信息：</strong>
                    <ul>
                      <li>昵称：{{ props.person.nickname }}</li>
                      <li>关系阶段：{{ getStageText(props.person.stage) }}</li>
                      <li v-if="props.person.tags && props.person.tags.length > 0">
                        标签：{{ props.person.tags.join('、') }}
                      </li>
                      <li v-if="props.person.lastContactAt">
                        上次联系：{{ formatTime(props.person.lastContactAt) }}
                      </li>
                      <li v-if="props.person.lastMeetAt">
                        上次见面：{{ formatTime(props.person.lastMeetAt) }}
                      </li>
                      <li v-if="props.person.nextAction">
                        当前下一步：{{ props.person.nextAction }}
                      </li>
                    </ul>
                  </div>
                  <div class="source-item">
                    <strong>本次互动：</strong>
                    <ul>
                      <li>类型：{{ getTypeText(form.type) }}</li>
                      <li>时间：{{ formatDateTime(form.occurredAt!) }}</li>
                      <li>摘要：{{ form.summary || '（未填写）' }}</li>
                    </ul>
                  </div>
                </div>
              </n-collapse-item>
            </n-collapse>
            <template #fallback>
              <div class="source-info">
                <div class="source-item">
                  <strong>对象信息：</strong>
                  <ul>
                    <li>昵称：{{ props.person?.nickname }}</li>
                    <li>关系阶段：{{ props.person ? getStageText(props.person.stage) : '' }}</li>
                  </ul>
                </div>
              </div>
            </template>
          </ClientOnly>
        </div>

        <!-- AI 生成状态提示 -->
        <div v-if="aiLoading" class="ai-loading">
          <n-spin size="small" />
          <span style="margin-left: 8px;">AI 正在分析中，请稍候...</span>
        </div>

        <!-- AI 生成结果 -->
        <div v-if="aiResult && !aiLoading" class="ai-result">
          <n-divider style="margin: 12px 0;">AI 分析结果</n-divider>
          
          <!-- 调试：显示原始数据 -->
          <div v-if="isDev" style="font-size: 12px; color: #999; margin-bottom: 8px;">
            调试：summary = {{ aiResult.summary ? '存在' : '不存在' }}, 
            oneLine = {{ getOneLine() || '(空)' }},
            keyFacts = {{ getKeyFacts().length }} 条
          </div>
          
          <div v-if="getOneLine()" class="ai-summary">
            <strong class="ai-section-title">一句话总结：</strong>
            <div class="ai-summary-content">{{ getOneLine() }}</div>
          </div>
          
          <!-- 如果 summary 存在但没有 oneLine，显示提示 -->
          <div v-else-if="aiResult.summary" class="ai-summary-empty">
            暂无一句话总结
          </div>
          
          <div v-if="getKeyFacts().length > 0" class="ai-key-facts">
            <strong class="ai-section-title">关键事实：</strong>
            <ul class="ai-key-facts-list">
              <li v-for="(fact, index) in getKeyFacts()" :key="index" class="ai-key-fact-item">{{ fact }}</li>
            </ul>
          </div>
          
          <!-- 热度分数变化提示 -->
          <div v-if="getHeatScoreHint()" class="ai-heat-score-hint">
            <strong class="ai-section-title">热度分数变化：</strong>
            <div class="heat-score-content">
              <span class="heat-score-delta" :class="{ 'positive': getHeatScoreHint()?.delta > 0, 'negative': getHeatScoreHint()?.delta < 0 }">
                {{ getHeatScoreHint()?.delta > 0 ? '+' : '' }}{{ getHeatScoreHint()?.delta }}
              </span>
              <span v-if="getHeatScoreHint()?.reason" class="heat-score-reason">
                （{{ getHeatScoreHint()?.reason }}）
              </span>
              <n-button
                size="small"
                type="primary"
                style="margin-left: 8px;"
                @click="applyHeatScoreChange"
              >
                应用变化
              </n-button>
            </div>
          </div>
          
          <div v-if="getNextActions().length > 0" class="ai-actions">
            <strong class="ai-section-title">下一步行动：</strong>
            <div
              v-for="(action, index) in getNextActions()"
              :key="index"
              class="action-item"
            >
              <div class="action-content">
                <strong class="action-title">{{ (action as any).title || (action as any).Title }}</strong> 
                <span v-if="(action as any).when || (action as any).When" class="action-when">({{ (action as any).when || (action as any).When }})</span>
                <span v-if="(action as any).why || (action as any).Why" class="action-why"> - {{ (action as any).why || (action as any).Why }}</span>
              </div>
              <n-button
                size="small"
                type="primary"
                @click="setAsNextAction((action as any).title || (action as any).Title || '')"
              >
                一键设为下一步行动
              </n-button>
            </div>
          </div>
          
          <div v-if="getMessageDrafts().length > 0" class="ai-message-drafts">
            <strong class="ai-section-title">消息草案（可编辑）：</strong>
            <div
              v-for="(draft, index) in getMessageDrafts()"
              :key="index"
              class="message-draft-item"
              :class="{ 'draft-editing': editingDraftIndex === index, 'draft-empty': !getDraftText(draft, index) }"
            >
              <div class="draft-header">
                <div class="draft-scene">{{ (draft as any).scene || (draft as any).Scene }}</div>
                <div class="draft-actions" v-if="editingDraftIndex !== index">
                  <n-button
                    size="tiny"
                    quaternary
                    type="primary"
                    @click.stop="startEditDraft(index, draft)"
                  >
                    <template #icon>
                      <i class="fas fa-edit"></i>
                    </template>
                    编辑
                  </n-button>
                  <n-button
                    size="tiny"
                    quaternary
                    type="info"
                    @click.stop="regenerateDraft(index)"
                    :loading="aiLoading"
                  >
                    <template #icon>
                      <i class="fas fa-redo"></i>
                    </template>
                    换个更随意的说法
                  </n-button>
                </div>
              </div>
              <div v-if="editingDraftIndex === index" class="draft-edit-mode">
                <n-input
                  :value="draftEdits.get(index)"
                  @update:value="(val: string) => draftEdits.set(index, val)"
                  type="textarea"
                  :rows="2"
                  placeholder="输入消息内容..."
                  @keydown.ctrl.enter="saveDraftEdit(index)"
                  @keydown.meta.enter="saveDraftEdit(index)"
                  @keydown.esc="cancelEditDraft(index)"
                />
                <div class="draft-edit-actions">
                  <n-button size="small" @click="cancelEditDraft(index)">取消</n-button>
                  <n-button size="small" type="primary" @click="saveDraftEdit(index)">保存</n-button>
                </div>
              </div>
              <div v-else class="draft-text" @click.stop="copyMessage(getDraftText(draft, index))">
                {{ getDraftText(draft, index) || '（待生成）' }}
                <i v-if="getDraftText(draft, index)" class="fas fa-copy copy-icon"></i>
              </div>
            </div>
          </div>
          
          <!-- 补充问题：轻提示形式，只在 AI 不确定时出现 -->
          <div v-if="getFollowupQuestions().length > 0" class="ai-followup-prompt">
            <div class="followup-prompt-content">
              <i class="fas fa-question-circle"></i>
              <span class="followup-question">{{ getFollowupQuestions()[0] }}</span>
            </div>
            <div class="followup-prompt-actions">
              <n-button size="small" @click="handleFollowupAnswer('skip')">跳过</n-button>
              <n-button size="small" type="primary" @click="handleFollowupAnswer('answer')">回答</n-button>
            </div>
          </div>

          <!-- 调试信息（开发环境显示） -->
          <div v-if="isDev && aiResult" class="ai-debug">
            <n-divider style="margin: 8px 0;" />
            <details style="font-size: 12px; color: #666;">
              <summary style="cursor: pointer;">调试信息（点击展开）</summary>
              <pre style="margin-top: 8px; padding: 8px; background: #f5f5f5; border-radius: 4px; overflow-x: auto;">{{ JSON.stringify(aiResult, null, 2) }}</pre>
            </details>
          </div>
        </div>

        <!-- 没有结果时的提示 -->
        <div v-if="!aiResult && !aiLoading" class="ai-empty">
          <n-empty size="small" description="点击上方「生成建议」按钮获取 AI 分析结果" />
        </div>
      </div>
    </n-form>

    <template #footer>
      <div style="display: flex; justify-content: space-between; align-items: center;">
        <div class="footer-tips">
          <span class="tip-text">
            💡 快速保存：Ctrl/Cmd + Enter
          </span>
        </div>
        <div style="display: flex; gap: 8px;">
          <n-button @click="handleClose">取消</n-button>
          <n-button 
            type="primary" 
            :loading="loading" 
            @click="() => handleSubmit(false)"
                        :disabled="false"
          >
            {{ isEdit ? '更新' : '保存' }}
          </n-button>
          <n-button 
            type="info" 
            :loading="loading || aiLoading" 
            @click="handleSubmitWithAi"
                        :disabled="false"
          >
            <template #icon>
              <i class="fas fa-magic"></i>
            </template>
            保存 + AI 分析
          </n-button>
        </div>
      </div>
    </template>
  </n-modal>
</template>

<script setup lang="ts">
import { ref, watch, reactive, computed, nextTick } from 'vue'
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
  NCollapse,
  NCollapseItem,
  NSpin,
  NEmpty,
  type FormInst
} from 'naive-ui'
import type {
  RelationPerson,
  RelationInteraction,
  CreateInteractionDto,
  UpdateInteractionDto,
  AiSummarizeResponse
} from '~/composables/useRelationsApi'
import { useSafeMessage } from '~/composables/useNaiveUI'

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

// 使用安全的 message composable，避免 Provider 未挂载时的错误
const message = useSafeMessage()

// 开发环境标识（用于模板中）
const isDev = import.meta.dev

const formRef = ref<FormInst | null>(null)
const loading = ref(false)
const aiLoading = ref(false)
const aiResult = ref<AiSummarizeResponse | null>(null)

// 消息草案编辑状态
const editingDraftIndex = ref<number | null>(null)
const draftEdits = ref<Map<number, string>>(new Map())

// 补充问题相关
const followupQuestionAnswer = ref<string>('')
const showFollowupPrompt = ref(false)

const visible = computed({
  get: () => props.show,
  set: (value) => emit('update:show', value)
})

const isEdit = computed(() => !!props.interaction)

// 兼容字段名格式：支持 snake_case 和 camelCase
const getOneLine = () => {
  if (!aiResult.value?.summary) return null
  const summary = aiResult.value.summary as any
  return summary.oneLine || summary.one_line || summary.OneLine || null
}

const getKeyFacts = (): string[] => {
  if (!aiResult.value?.summary) return []
  const summary = aiResult.value.summary as any
  return summary.keyFacts || summary.key_facts || summary.KeyFacts || []
}

const getNextActions = () => {
  if (!aiResult.value) return []
  const result = aiResult.value as any
  return result.nextActions || result.next_actions || result.NextActions || []
}

const getMessageDrafts = () => {
  if (!aiResult.value) return []
  const result = aiResult.value as any
  return result.messageDrafts || result.message_drafts || result.MessageDrafts || []
}

const getHeatScoreHint = () => {
  if (!aiResult.value) return null
  const result = aiResult.value as any
  const hint = result.heatScoreHint || result.heat_score_hint || result.HeatScoreHint
  if (!hint || (hint.delta === undefined && hint.delta === 0)) return null
  return {
    delta: hint.delta || hint.Delta || 0,
    reason: hint.reason || hint.Reason || ''
  }
}

const applyHeatScoreChange = async () => {
  const hint = getHeatScoreHint()
  if (!hint || hint.delta === 0) {
    message.warning('没有热度分数变化可应用')
    return
  }
  
  try {
    const relationsApi = useRelationsApi()
    
    // 获取当前对象信息
    let personInfo = props.person
    if (!personInfo) {
      personInfo = await relationsApi.getPerson(props.personId)
    }
    
    const currentHeatScore = personInfo?.heatScore || 0
    const newHeatScore = Math.max(0, Math.min(100, currentHeatScore + hint.delta))
    
    await relationsApi.updatePerson(props.personId, {
      heatScore: newHeatScore
    })
    
    const deltaText = hint.delta > 0 ? `+${hint.delta}` : `${hint.delta}`
    message.success(`热度分数已更新：${currentHeatScore} → ${newHeatScore} (${deltaText})`)
    
    // 更新本地 person 对象（如果存在）
    if (props.person) {
      props.person.heatScore = newHeatScore
    }
    
    // 触发成功事件，刷新数据
    emit('success')
  } catch (error: any) {
    message.error(error.message || '应用热度分数变化失败')
  }
}

// 获取当前正在编辑的草案文本
const getDraftText = (draft: any, index: number): string => {
  if (draftEdits.value.has(index)) {
    return draftEdits.value.get(index)!
  }
  return (draft as any).text || (draft as any).Text || ''
}

// 开始编辑草案
const startEditDraft = (index: number, draft: any) => {
  editingDraftIndex.value = index
  const currentText = (draft as any).text || (draft as any).Text || ''
  draftEdits.value.set(index, currentText)
}

// 保存草案编辑
const saveDraftEdit = (index: number) => {
  editingDraftIndex.value = null
  // 更新原始数据
  if (aiResult.value) {
    const result = aiResult.value as any
    const drafts = result.messageDrafts || result.message_drafts || result.MessageDrafts || []
    if (drafts[index] && draftEdits.value.has(index)) {
      drafts[index].text = draftEdits.value.get(index)
      drafts[index].Text = draftEdits.value.get(index)
    }
  }
}

// 取消编辑草案
const cancelEditDraft = (index: number) => {
  editingDraftIndex.value = null
  draftEdits.value.delete(index)
}

const getFollowupQuestions = () => {
  if (!aiResult.value) return []
  const result = aiResult.value as any
  const questions = result.followupQuestions || result.followup_questions || result.FollowupQuestions || []
  // 只返回第一个问题（最多1个）
  return questions.slice(0, 1)
}

// 处理补充问题回答
const handleFollowupAnswer = (action: 'skip' | 'answer') => {
  if (action === 'skip') {
    // 跳过补充问题，不显示提示
    return
  }
  // TODO: 如果用户选择回答，可以展开输入框或跳转到相关位置
  // 暂时先隐藏提示
}

// 重新生成单个消息草案（更随意的说法）
const regenerateDraft = async (index: number) => {
  if (!aiResult.value || !props.person) return
  
  try {
    aiLoading.value = true
    const relationsApi = useRelationsApi()
    
    // 获取最近的互动记录
    const interactions = await relationsApi.getInteractions(props.personId)
    const recentInteraction = interactions && interactions.length > 0 ? interactions[0] : null
    
    // 构建请求，添加提示要求更随意的说法
    const request = {
      person: {
        nickname: props.person.nickname,
        stage: props.person.stage,
        tags: props.person.tags || [],
        lastContactAt: props.person.lastContactAt,
        lastMeetAt: props.person.lastMeetAt,
        currentNextAction: props.person.nextAction
      },
      historyKeyPoints: undefined,
      interaction: recentInteraction || {
        type: form.type ?? 0,
        occurredAt: new Date(form.occurredAt ?? Date.now()).toISOString(),
        summary: form.summary || '互动记录',
        chatText: undefined
      },
      userPreference: {
        userStyle: '更随意、更口语化、像真人聊天'
      }
    }
    
    const response = await relationsApi.aiSummarize(request)
    
    // 更新对应位置的草案
    if (aiResult.value) {
      const result = aiResult.value as any
      const drafts = result.messageDrafts || result.message_drafts || result.MessageDrafts || []
      const newDrafts = response.messageDrafts || response.message_drafts || response.MessageDrafts || []
      if (newDrafts[index]) {
        drafts[index] = newDrafts[index]
        message.success('已生成更随意的说法')
      }
    }
  } catch (error: any) {
    console.error('重新生成草案失败:', error)
    message.error('重新生成失败，请稍后重试')
  } finally {
    aiLoading.value = false
  }
}

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
  // 所有字段均为可选，降低使用门槛
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
  // 即使没有摘要也可以生成建议（基于其他信息）
  aiLoading.value = true
  aiResult.value = null // 清空之前的结果
  
  try {
    const relationsApi = useRelationsApi()
    
    // 如果没有 person 信息，需要先获取
    let personInfo = props.person
    if (!personInfo) {
      const response = await relationsApi.getPerson(props.personId)
      if (response && response.data) {
        personInfo = response.data
      } else {
        throw new Error('获取对象信息失败')
      }
    }
    
    // 构建请求数据（按照后端 RelationAiSummarizeRequest 格式）
    const request = {
      person: {
        nickname: personInfo.nickname,
        stage: personInfo.stage,
        tags: personInfo.tags || [],
        lastContactAt: personInfo.lastContactAt,
        lastMeetAt: personInfo.lastMeetAt,
        currentNextAction: personInfo.nextAction
      },
      historyKeyPoints: undefined as string | undefined, // TODO: 可以从历史互动记录中提取
      interaction: {
        type: form.type,
        occurredAt: new Date(form.occurredAt!).toISOString(),
        summary: form.summary?.trim() || '互动记录',
        chatText: undefined as string | undefined // TODO: 如果用户有输入聊天片段，可以添加
      },
      userPreference: undefined as any // TODO: 可以添加用户偏好设置
    }
    
    console.log('发送 AI 请求:', request)
    const response = await relationsApi.aiSummarize(request)
    
    console.log('AI 响应:', response)
    console.log('AI 响应类型:', typeof response)
    console.log('AI 响应 keys:', response ? Object.keys(response) : 'response 为空')
    
    // 处理响应数据（useApi 返回的是 ApiResponse<T> 格式）
    // useApi 会自动提取 data 字段，所以 response 应该直接就是 AiSummarizeResponse 对象
    let resultData: any = null
    
    if (!response) {
      console.warn('响应为空')
      throw new Error('AI 服务返回数据为空')
    }
    
    // 检查是否是错误响应格式
    if ('success' in response && response.success === false) {
      const errorMsg = (response as any).message || '生成建议失败'
      console.error('API 返回错误:', errorMsg)
      throw new Error(errorMsg)
    }
    
    // useApi 通常已经提取了 data 字段，所以 response 应该直接是数据对象
    // 但如果还有包装，尝试提取 data
    if ('data' in response && response.data) {
      resultData = response.data
      console.log('从 response.data 提取数据')
    } 
    // 如果 response 直接就是数据对象（有 summary 或 nextActions 等字段）
    else if ('summary' in response || 'nextActions' in response) {
      resultData = response
      console.log('response 直接是数据对象')
    }
    // 如果是 ApiResponse 包装格式（code, data, message）
    else if ('code' in response && 'data' in response) {
      if ((response as any).code !== 0 && (response as any).code !== 200) {
        throw new Error((response as any).message || '生成建议失败')
      }
      resultData = (response as any).data
      console.log('从 ApiResponse 格式提取数据')
    }
    else {
      console.warn('无法识别的响应格式:', response)
      // 尝试直接使用 response
      resultData = response
    }
    
    if (!resultData) {
      console.error('resultData 为空，原始响应:', response)
      throw new Error('返回数据格式错误或为空，请检查控制台日志')
    }
    
    console.log('解析后的 resultData:', resultData)
    console.log('resultData.summary:', resultData.summary)
    
    // 检查字段名映射：后端可能返回 snake_case 或 camelCase
    // 统一转换为前端使用的 camelCase 格式
    if (resultData.summary) {
      const summary = resultData.summary as any
      // 处理字段名差异：支持 snake_case, PascalCase, camelCase
      if (summary.one_line && !summary.oneLine) {
        summary.oneLine = summary.one_line
      } else if (summary.OneLine && !summary.oneLine) {
        summary.oneLine = summary.OneLine
      }
      
      if (summary.key_facts && !summary.keyFacts) {
        summary.keyFacts = summary.key_facts
      } else if (summary.KeyFacts && !summary.keyFacts) {
        summary.keyFacts = summary.KeyFacts
      }
    }
    
    console.log('转换后的 resultData.summary?.oneLine:', resultData.summary?.oneLine)
    console.log('转换后的 resultData.summary?.keyFacts:', resultData.summary?.keyFacts)
    
    aiResult.value = resultData
    
    // 将 key_facts 写入 keyPoints（用于后续保存到 interaction）
    const keyFacts = (resultData.summary as any)?.keyFacts || (resultData.summary as any)?.key_facts
    if (keyFacts && Array.isArray(keyFacts) && keyFacts.length > 0) {
      form.keyPoints = {
        key_facts: keyFacts
      }
    }
    
    message.success('AI 建议生成成功')
  } catch (error: any) {
    console.error('AI 生成失败:', error)
    console.error('错误堆栈:', error.stack)
    const errorMsg = error.response?.data?.message || error.message || '生成建议失败，请稍后重试'
    message.error(errorMsg)
    
    // 显示详细错误信息（开发环境）
    if (import.meta.dev) {
      console.error('详细错误:', error.response?.data || error)
    }
  } finally {
    console.log('设置 aiLoading 为 false')
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
    message.success('已设为下一步行动')
    
    // 更新本地 person 对象（如果存在）
    if (props.person) {
      props.person.nextAction = actionTitle
    }
  } catch (error: any) {
    message.error(error.message || '设置失败')
  }
}

// 辅助函数：格式化时间
const formatTime = (timeStr: string | undefined) => {
  if (!timeStr) return 'N/A'
  try {
    return new Date(timeStr).toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return timeStr
  }
}

// 辅助函数：格式化日期时间
const formatDateTime = (timestamp: number) => {
  try {
    return new Date(timestamp).toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return 'N/A'
  }
}

// 辅助函数：获取阶段文本
const getStageText = (stage: number) => {
  const stages = ['新认识', '熟悉中', '准备约见', '已见面', '升温中', '观察期', '已结束']
  return stages[stage] || '未知'
}

// 辅助函数：获取互动类型文本
const getTypeText = (type: number) => {
  const types = ['文字', '语音', '通话', '见面', '其他']
  return types[type] || '未知'
}

// 快速提交（Ctrl/Cmd + Enter）
const handleQuickSubmit = () => {
  if (form.summary.trim()) {
    handleSubmit(false)
  }
}

// 普通保存
const handleSubmit = async (withAi: boolean = false) => {
  // 确保有默认值
  const type = form.type ?? 0
  const occurredAt = form.occurredAt ?? Date.now()
  // 如果没有摘要，使用默认值
  const summary = form.summary?.trim() || '互动记录'

  loading.value = true
  try {
    const relationsApi = useRelationsApi()
    const occurredAtDate = new Date(occurredAt)

      if (isEdit.value && props.interaction) {
      const updateData: UpdateInteractionDto = {
        type: type,
        occurredAt: occurredAtDate.toISOString(),
        summary: summary,
        myFeeling: form.myFeeling,
        aiSuggestion: aiResult.value?.suggestion
      }
      await relationsApi.updateInteraction(props.interaction.id, updateData)
      message.success('更新成功')
      } else {
        const createData: CreateInteractionDto = {
          type: type,
          occurredAt: occurredAtDate.toISOString(),
          summary: summary,
        myFeeling: form.myFeeling,
        keyPoints: form.keyPoints
      }
        const created = await relationsApi.createInteraction(props.personId, createData)
        
        // 如果选择了"保存 + AI 分析"，自动触发 AI 分析
        if (withAi && !isEdit.value) {
          // 等待保存完成后再调用 AI
          await nextTick()
          await handleAiGenerateAfterSave(created)
        }
        
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
        
        message.success(withAi ? '保存成功，AI 分析中...' : '保存成功')
      }

      // 如果不需要 AI 分析，直接关闭并刷新
      if (!withAi) {
        emit('success')
        handleClose()
      } else if (!isEdit.value) {
        // 如果需要 AI 分析且是新建，等待 AI 分析完成
        // handleAiGenerateAfterSave 会触发 success 事件并处理关闭逻辑
        // 这里不关闭，等待 AI 完成
      } else {
        // 编辑模式下，直接关闭
        emit('success')
        handleClose()
      }
    } catch (error: any) {
      message.error(error.message || '操作失败')
    } finally {
      loading.value = false
    }
}

// 保存后自动触发 AI 分析
const handleAiGenerateAfterSave = async (createdInteraction?: any) => {
  // 即使没有摘要也可以生成建议
  aiLoading.value = true
  try {
    const relationsApi = useRelationsApi()
    
    // 获取对象信息
    let personInfo = props.person
    if (!personInfo) {
      personInfo = await relationsApi.getPerson(props.personId)
    }
    
    // 构建请求
    const request = {
      person: {
        nickname: personInfo!.nickname,
        stage: personInfo!.stage,
        tags: personInfo!.tags || [],
        lastContactAt: personInfo!.lastContactAt,
        lastMeetAt: personInfo!.lastMeetAt,
        currentNextAction: personInfo!.nextAction
      },
      historyKeyPoints: undefined,
      interaction: {
        type: form.type ?? 0,
        occurredAt: (form.occurredAt ? new Date(form.occurredAt) : new Date()).toISOString(),
        summary: form.summary,
        chatText: undefined
      }
    }
    
    const response = await relationsApi.aiSummarize(request)
    aiResult.value = response
    
    // 自动应用热度分数变化（如果有 AI 建议）
    const heatScoreHint = response?.heatScoreHint || response?.heat_score_hint
    if (heatScoreHint && heatScoreHint.delta !== undefined && heatScoreHint.delta !== 0 && personInfo) {
      try {
        const currentHeatScore = personInfo.heatScore || 0
        const newHeatScore = Math.max(0, Math.min(100, currentHeatScore + heatScoreHint.delta))
        await relationsApi.updatePerson(props.personId, {
          heatScore: newHeatScore
        })
        const deltaText = heatScoreHint.delta > 0 ? `+${heatScoreHint.delta}` : `${heatScoreHint.delta}`
        message.success(`热度分数已更新：${deltaText}（${heatScoreHint.reason || '基于本次互动'}）`)
      } catch (e) {
        console.error('更新热度分数失败:', e)
        // 静默失败，不影响其他功能
      }
    }
    
    // 自动填充 nextAction（如果有 AI 建议）
    const nextActions = getNextActions()
    if (nextActions && nextActions.length > 0) {
      const firstAction = nextActions[0] as any
      const actionTitle = firstAction.title || firstAction.Title || firstAction.title
      if (actionTitle && personInfo) {
        try {
          await relationsApi.updatePerson(props.personId, {
            nextAction: actionTitle
          })
          message.success('AI 已自动更新下一步行动')
        } catch (e) {
          // 静默失败
        }
      }
    }
    
    message.success('AI 分析完成')
    // AI 分析完成后不自动关闭，让用户查看结果
    // 用户可以手动关闭或继续操作
  } catch (error: any) {
    console.error('AI 分析失败:', error)
    message.warning('保存成功，但 AI 分析失败，可稍后重试')
    // 即使 AI 失败，也触发 success 事件刷新列表
    emit('success')
    handleClose()
  } finally {
    aiLoading.value = false
  }
}

// 保存 + AI 分析
const handleSubmitWithAi = async () => {
  await handleSubmit(true)
}
</script>

<style scoped>
.ai-section {
  margin-top: 16px;
  padding: 12px;
  background: var(--color-info-50);
  border-radius: 4px;
}

[data-theme="dark"] .ai-section {
  background: rgba(31, 41, 55, 0.6);
  border: 1px solid #374151;
}

.ai-data-source {
  margin: 12px 0;
  padding: 8px;
  background: rgba(255, 255, 255, 0.5);
  border-radius: 4px;
  font-size: 12px;
}

[data-theme="dark"] .ai-data-source {
  background: rgba(31, 41, 55, 0.8);
}

.source-info {
  padding: 8px 0;
}

.source-item {
  margin: 8px 0;
}

.source-item ul {
  margin: 4px 0 0 20px;
  padding: 0;
  list-style-type: disc;
}

.source-item li {
  margin: 2px 0;
  color: var(--text-color-2);
}

.ai-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  font-weight: 600;
}

.ai-loading {
  margin-top: 12px;
  padding: 16px;
  text-align: center;
  background: rgba(255, 255, 255, 0.5);
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
}

[data-theme="dark"] .ai-loading {
  background: rgba(31, 41, 55, 0.8);
}

.ai-result {
  margin-top: 12px;
  padding: 18px;
  background: #ffffff;
  border-radius: 8px;
  border: 1px solid #d1d5db;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

[data-theme="dark"] .ai-result {
  background: #1f2937;
  border-color: #4b5563;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.ai-summary {
  margin: 12px 0;
}

.ai-key-facts {
  margin: 12px 0;
}

.ai-key-facts ul {
  margin: 8px 0;
  padding-left: 20px;
}

.ai-key-facts li {
  margin: 6px 0;
  line-height: 1.6;
}

.message-draft-item {
  position: relative;
  transition: all 0.2s;
}

.message-draft-item:hover {
  border-color: var(--color-primary) !important;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.draft-empty {
  opacity: 0.6;
  cursor: not-allowed;
}

.ai-empty {
  margin-top: 12px;
  padding: 16px;
  text-align: center;
  background: rgba(255, 255, 255, 0.3);
  border-radius: 4px;
}

[data-theme="dark"] .ai-empty {
  background: rgba(31, 41, 55, 0.6);
}

.ai-suggestion,
.ai-actions {
  margin: 8px 0;
  padding: 8px;
  background: white;
  border-radius: 4px;
}

[data-theme="dark"] .ai-suggestion,
[data-theme="dark"] .ai-actions {
  background: #1f2937;
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
  margin: 12px 0;
  padding: 0;
}

/* AI 结果区域通用样式 - 使用明确的颜色值确保可读性 */
.ai-section-title {
  display: block;
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 12px;
  margin-top: 18px;
}

[data-theme="dark"] .ai-section-title {
  color: #f9fafb;
}

.ai-summary {
  margin: 12px 0;
  padding: 0;
}

.ai-summary-content {
  margin-top: 10px;
  padding: 16px 18px;
  background: #ffffff;
  border-radius: 6px;
  font-size: 16px;
  line-height: 1.8;
  color: #111827;
  border: 1px solid #e5e7eb;
  border-left: 4px solid #3b82f6;
  white-space: pre-wrap;
  word-wrap: break-word;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

[data-theme="dark"] .ai-summary-content {
  background: #111827;
  color: #f9fafb;
  border-color: #374151;
  border-left-color: #60a5fa;
}

.ai-summary-empty {
  margin-top: 10px;
  padding: 12px;
  color: #6b7280;
  font-style: italic;
  font-size: 14px;
}

[data-theme="dark"] .ai-summary-empty {
  color: #9ca3af;
}

.ai-key-facts {
  margin: 18px 0;
  padding: 0;
}

.ai-key-facts-list {
  margin: 12px 0 0 28px;
  padding: 0;
  list-style-type: disc;
}

.ai-key-fact-item {
  margin: 10px 0;
  padding: 8px 12px;
  font-size: 15px;
  line-height: 1.8;
  color: #111827;
  background: #ffffff;
  border-radius: 4px;
  border-left: 3px solid #10b981;
}

[data-theme="dark"] .ai-key-fact-item {
  color: #f9fafb;
  background: #111827;
  border-left-color: #34d399;
}

.ai-actions {
  margin: 18px 0;
  padding: 0;
}

.action-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin: 12px 0;
  padding: 16px 18px;
  background: #ffffff;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  gap: 12px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

[data-theme="dark"] .action-item {
  background: #111827;
  border-color: #374151;
}

.action-content {
  flex: 1;
  font-size: 15px;
  line-height: 1.7;
  color: #111827;
}

[data-theme="dark"] .action-content {
  color: #f9fafb;
}

.action-title {
  font-size: 15px;
  font-weight: 600;
  color: #1e40af;
  margin-right: 8px;
}

[data-theme="dark"] .action-title {
  color: #93c5fd;
}

.action-when,
.action-why {
  font-size: 14px;
  color: #374151;
}

[data-theme="dark"] .action-when,
[data-theme="dark"] .action-why {
  color: #d1d5db;
}

.ai-message-drafts {
  margin: 18px 0;
  padding: 0;
}

.message-draft-item {
  position: relative;
  margin: 12px 0;
  padding: 16px 18px;
  padding-right: 45px;
  background: #ffffff;
  border-radius: 6px;
  cursor: pointer;
  border: 1px solid #e5e7eb;
  transition: all 0.2s;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

[data-theme="dark"] .message-draft-item {
  background: #111827;
  border-color: #374151;
}

.message-draft-item:hover {
  border-color: #3b82f6;
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.25);
  transform: translateY(-1px);
}

[data-theme="dark"] .message-draft-item:hover {
  border-color: #60a5fa;
  background: #1e2937;
}

.message-draft-item.draft-empty {
  opacity: 0.6;
  cursor: not-allowed;
}

.draft-scene {
  font-size: 13px;
  color: #6b7280;
  margin-bottom: 10px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

[data-theme="dark"] .draft-scene {
  color: #9ca3af;
}

.draft-text {
  font-size: 15px;
  line-height: 1.8;
  color: #111827;
  white-space: pre-wrap;
  word-wrap: break-word;
  font-weight: 400;
}

[data-theme="dark"] .draft-text {
  color: #f9fafb;
}

.copy-icon {
  position: absolute;
  top: 14px;
  right: 14px;
  color: #6b7280;
  font-size: 16px;
  transition: color 0.2s;
}

.message-draft-item:hover .copy-icon {
  color: #3b82f6;
}

[data-theme="dark"] .copy-icon {
  color: #9ca3af;
}

[data-theme="dark"] .message-draft-item:hover .copy-icon {
  color: #60a5fa;
}

.ai-heat-score-hint {
  margin: 18px 0;
  padding: 12px;
  background: rgba(59, 130, 246, 0.1);
  border-radius: 6px;
  border-left: 3px solid #3b82f6;
}

[data-theme="dark"] .ai-heat-score-hint {
  background: rgba(59, 130, 246, 0.15);
  border-left-color: #60a5fa;
}

.heat-score-content {
  display: flex;
  align-items: center;
  margin-top: 8px;
  flex-wrap: wrap;
  gap: 8px;
}

.heat-score-delta {
  font-size: 18px;
  font-weight: 600;
  padding: 4px 12px;
  border-radius: 4px;
  background: rgba(59, 130, 246, 0.2);
}

.heat-score-delta.positive {
  color: #10b981;
  background: rgba(16, 185, 129, 0.2);
}

.heat-score-delta.negative {
  color: #ef4444;
  background: rgba(239, 68, 68, 0.2);
}

.heat-score-reason {
  font-size: 14px;
  color: var(--text-color-2);
  flex: 1;
}

.ai-questions {
  margin: 18px 0;
  padding: 0;
}

.questions-list {
  margin: 12px 0;
}

.question-item {
  margin: 10px 0;
  padding: 14px 16px;
  background: #ffffff;
  border-radius: 6px;
  font-size: 15px;
  line-height: 1.7;
  color: #78350f;
  border: 1px solid #fde68a;
  border-left: 4px solid #f59e0b;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

[data-theme="dark"] .question-item {
  background: #1f2937;
  color: #fde68a;
  border-color: #fbbf24;
  border-left-color: #fbbf24;
}

.questions-hint {
  margin-top: 12px;
  font-size: 13px;
  color: #6b7280;
  font-style: italic;
}

[data-theme="dark"] .questions-hint {
  color: #9ca3af;
}

.ai-debug {
  margin-top: 12px;
  font-size: 12px;
}
</style>

