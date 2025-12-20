// 关系跟进 API 调用统一管理

export interface RelationPerson {
  id: string
  nickname: string
  source?: string
  city?: string
  tags: string[]
  preferences?: string
  stage: number
  heatScore: number
  lastContactAt?: string
  lastMeetAt?: string
  nextAction?: string
  remindAt?: string
  notes?: string
  // 观察期相关字段
  observationStartedAt?: string
  observationExpectedEndAt?: string
  observationLastRemindedAt?: string
  observationReason?: string
  observationDecisionPending?: boolean
  createdAt: string
  updatedAt: string
}

export interface RelationInteraction {
  id: string
  personId: string
  type: number
  occurredAt: string
  summary: string
  keyPoints?: Record<string, any>
  myFeeling?: number
  aiSuggestion?: string
  createdAt: string
}

export interface RelationTask {
  id: string
  personId: string
  title: string
  dueAt?: string
  priority: number
  status: number
  createdAt: string
  updatedAt: string
}

export interface CreatePersonDto {
  nickname: string
  source?: string
  city?: string
  tags?: string[]
  preferences?: string
  stage?: number
  notes?: string
}

export interface UpdatePersonDto {
  nickname?: string
  source?: string
  city?: string
  tags?: string[]
  preferences?: string
  stage?: number
  heatScore?: number
  nextAction?: string
  remindAt?: string
  notes?: string
}

export interface CreateInteractionDto {
  type: number
  occurredAt: string
  summary: string
  keyPoints?: Record<string, any>
  myFeeling?: number
}

export interface UpdateInteractionDto {
  type?: number
  occurredAt?: string
  summary?: string
  keyPoints?: Record<string, any>
  myFeeling?: number
  aiSuggestion?: string
}

export interface CreateTaskDto {
  title: string
  dueAt?: string
  priority?: number
}

export interface UpdateTaskDto {
  title?: string
  dueAt?: string
  priority?: number
  status?: number
}

export interface RelationPersonInfo {
  nickname: string
  stage?: number
  tags?: string[]
  lastContactAt?: string
  lastMeetAt?: string
  currentNextAction?: string
}

export interface RelationInteractionInfo {
  type: number
  occurredAt: string
  summary: string
  chatText?: string
}

export interface RelationUserPreference {
  userGoal?: string
  userStyle?: string
  timeConstraints?: string
}

export interface AiSummarizeRequest {
  person: RelationPersonInfo
  historyKeyPoints?: string
  interaction: RelationInteractionInfo
  userPreference?: RelationUserPreference
}

export interface RelationSummary {
  oneLine?: string
  keyFacts?: string[]
  signals?: {
    positive?: string[]
    neutral?: string[]
    negative?: string[]
  }
  preferencesUpdates?: {
    likes?: string[]
    dislikes?: string[]
  }
  myCommitments?: string[]
  risks?: string[]
}

export interface RelationNextAction {
  title?: string
  why?: string
  when?: string
  priority?: number
}

export interface RelationMessageDraft {
  scene?: string
  text?: string
}

export interface AiSummarizeResponse {
  summary?: RelationSummary
  nextActions?: RelationNextAction[]
  messageDrafts?: RelationMessageDraft[]
  followupQuestions?: string[]
  stageSuggestion?: {
    current?: string
    suggested?: string
    reason?: string
  }
  heatScoreHint?: {
    delta?: number
    reason?: string
  }
  rawText?: string
}

export const useRelationsApi = () => {
  const api = useApi()

  // 获取关系对象列表
  const getPersons = async (params?: {
    stage?: number
    q?: string
    tag?: string
    sort?: string
  }) => {
    const queryParams = new URLSearchParams()
    if (params?.stage !== undefined) queryParams.append('stage', params.stage.toString())
    if (params?.q) queryParams.append('q', params.q)
    if (params?.tag) queryParams.append('tag', params.tag)
    if (params?.sort) queryParams.append('sort', params.sort)
    
    const query = queryParams.toString()
    const url = `/relations/persons${query ? `?${query}` : ''}`
    return await api.get<RelationPerson[]>(url)
  }

  // 获取关系对象详情
  const getPerson = async (id: string) => {
    return await api.get<RelationPerson>(`/relations/persons/${id}`)
  }

  // 创建关系对象
  const createPerson = async (data: CreatePersonDto) => {
    return await api.post<RelationPerson>('/relations/persons', data)
  }

  // 更新关系对象
  const updatePerson = async (id: string, data: UpdatePersonDto) => {
    return await api.put<RelationPerson>(`/relations/persons/${id}`, data)
  }

  // 删除关系对象
  const deletePerson = async (id: string) => {
    return await api.delete(`/relations/persons/${id}`)
  }

  // 获取互动记录列表
  const getInteractions = async (personId: string) => {
    return await api.get<RelationInteraction[]>(`/relations/persons/${personId}/interactions`)
  }

  // 创建互动记录
  const createInteraction = async (personId: string, data: CreateInteractionDto) => {
    return await api.post<RelationInteraction>(`/relations/persons/${personId}/interactions`, data)
  }

  // 更新互动记录
  const updateInteraction = async (id: string, data: UpdateInteractionDto) => {
    return await api.put<RelationInteraction>(`/relations/interactions/${id}`, data)
  }

  // 删除互动记录
  const deleteInteraction = async (id: string) => {
    return await api.delete(`/relations/interactions/${id}`)
  }

  // 获取任务列表
  const getTasks = async (personId: string) => {
    return await api.get<RelationTask[]>(`/relations/persons/${personId}/tasks`)
  }

  // 创建任务
  const createTask = async (personId: string, data: CreateTaskDto) => {
    return await api.post<RelationTask>(`/relations/persons/${personId}/tasks`, data)
  }

  // 更新任务
  const updateTask = async (id: string, data: UpdateTaskDto) => {
    return await api.put<RelationTask>(`/relations/tasks/${id}`, data)
  }

  // 删除任务
  const deleteTask = async (id: string) => {
    return await api.delete(`/relations/tasks/${id}`)
  }

  // AI 生成总结和建议
  const aiSummarize = async (data: AiSummarizeRequest) => {
    return await api.post<AiSummarizeResponse>('/relations/ai/summarize', data)
  }

  // 观察期相关 API
  const getObservationSuggestion = async (personId: string) => {
    return await api.get(`/relations/persons/${personId}/observation/suggestion`)
  }

  const startObservation = async (personId: string, data: { reason?: string; durationDays?: number }) => {
    return await api.post(`/relations/persons/${personId}/observation/start`, data)
  }

  const getObservationReminders = async () => {
    return await api.get<ObservationReminder[]>('/relations/observation/reminders')
  }

  const markReminderViewed = async (personId: string) => {
    return await api.post(`/relations/persons/${personId}/observation/reminder/viewed`)
  }

  const handleObservationDecision = async (personId: string, data: { decision: 'Continue' | 'Downgrade' | 'End'; reason?: string }) => {
    return await api.post(`/relations/persons/${personId}/observation/decision`, data)
  }

  return {
    getPersons,
    getPerson,
    createPerson,
    updatePerson,
    deletePerson,
    getInteractions,
    createInteraction,
    updateInteraction,
    deleteInteraction,
    getTasks,
    createTask,
    updateTask,
    deleteTask,
    aiSummarize,
    getObservationSuggestion,
    startObservation,
    getObservationReminders,
    markReminderViewed,
    handleObservationDecision
  }
}

// 观察期相关类型
export interface ObservationSuggestion {
  personId: string
  reasons: string[]
  suggestedDurationDays: number
}

export interface ObservationReminder {
  personId: string
  personNickname: string
  type: 'OnGoing' | 'EndingSoon' | 'DecisionRequired'
  daysInObservation: number
  daysUntilEnd?: number
  reason?: string
}

