<template>
  <div class="document-agent-page">
    <!-- 页面标题 -->
    <div class="page-header">
      <h1 class="page-title">文档知识管家</h1>
      <button @click="showUploadModal = true" class="btn-primary">
        <i class="fas fa-upload"></i> 上传文档
      </button>
    </div>

    <!-- 文档列表 -->
    <div class="document-list-section">
      <div class="section-header">
        <h2>文档列表</h2>
        <div class="filter-controls">
          <select v-model="filterStatus" @change="loadDocuments" class="filter-select">
            <option value="">全部状�?/option>
            <option value="pending">待处�?/option>
            <option value="processing">处理�?/option>
            <option value="completed">已完�?/option>
            <option value="failed">处理失败</option>
          </select>
          <button @click="loadDocuments" class="btn-refresh">
            <i class="fas fa-sync-alt"></i> 刷新
          </button>
        </div>
      </div>

      <!-- 文档表格 -->
      <div class="table-container">
        <table class="document-table">
          <thead>
            <tr>
              <th>标题</th>
              <th>文件�?/th>
              <th>类型</th>
              <th>大小</th>
              <th>状�?/th>
              <th>分段�?/th>
              <th>创建时间</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="doc in documents" :key="doc.id" class="document-row">
              <td class="title-cell">
                <span class="doc-title" @click.stop>{{ doc.title }}</span>
                <div v-if="doc.summary" class="doc-summary">{{ doc.summary }}</div>
              </td>
              <td>{{ doc.fileName }}</td>
              <td>
                <span class="file-type-badge" :class="`type-${doc.fileType}`">
                  {{ doc.fileType.toUpperCase() }}
                </span>
              </td>
              <td>{{ formatFileSize(doc.fileSize) }}</td>
              <td>
                <span class="status-badge" :class="`status-${doc.status}`">
                  {{ getStatusText(doc.status) }}
                </span>
                <div v-if="doc.errorMessage" class="error-message" :title="doc.errorMessage">
                  <i class="fas fa-exclamation-triangle"></i>
                  {{ doc.errorMessage.length > 50 ? doc.errorMessage.substring(0, 50) + '...' : doc.errorMessage }}
                </div>
              </td>
              <td>{{ doc.totalChunks }}</td>
              <td>{{ formatDate(doc.createdAt) }}</td>
              <td class="action-cell">
                <button
                  v-if="doc.status === 'failed' || doc.status === 'pending'"
                  @click="retryProcessDocument(doc.id)"
                  class="btn-action btn-retry"
                  title="重试处理"
                >
                  <i class="fas fa-redo"></i>
                </button>
                <button
                  @click="viewDocument(doc)"
                  class="btn-action"
                  :disabled="doc.status !== 'completed'"
                  title="查看详情"
                >
                  <i class="fas fa-eye"></i>
                </button>
                <button
                  @click="queryDocument(doc)"
                  class="btn-action"
                  :disabled="doc.status !== 'completed'"
                  title="问答"
                >
                  <i class="fas fa-comments"></i>
                </button>
                <button
                  @click="deleteDocument(doc.id)"
                  class="btn-action btn-danger"
                  title="删除"
                >
                  <i class="fas fa-trash"></i>
                </button>
              </td>
            </tr>
            <tr v-if="documents.length === 0">
              <td colspan="8" class="empty-cell">暂无文档</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- 分页 -->
      <div v-if="total > pageSize" class="pagination">
        <button
          @click="changePage(currentPage - 1)"
          :disabled="currentPage === 1"
          class="page-btn"
        >
          上一�?        </button>
        <span class="page-info">
          �?{{ currentPage }} / {{ totalPages }} 页，�?{{ total }} �?        </span>
        <button
          @click="changePage(currentPage + 1)"
          :disabled="currentPage >= totalPages"
          class="page-btn"
        >
          下一�?        </button>
      </div>
    </div>

    <!-- 上传文档模态框 -->
    <Teleport to="body">
      <div v-if="showUploadModal" class="modal-overlay" @click.self="showUploadModal = false">
        <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>上传文档</h3>
          <button @click="showUploadModal = false" class="modal-close">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <div class="upload-area" @drop.prevent="handleDrop" @dragover.prevent>
            <input
              ref="fileInput"
              type="file"
              @change="handleFileSelect"
              accept=".pdf,.docx,.txt,.md"
              style="display: none"
            />
            <div v-if="!uploadFile" class="upload-placeholder">
              <i class="fas fa-cloud-upload-alt"></i>
              <p>拖拽文件到此处或点击选择</p>
              <p class="upload-hint">支持 PDF、DOCX、TXT、MD 格式</p>
            </div>
            <div v-else class="upload-file-info">
              <i class="fas fa-file"></i>
              <span>{{ uploadFile.name }}</span>
              <span class="file-size">{{ formatFileSize(uploadFile.size) }}</span>
              <button @click="uploadFile = null" class="btn-remove">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <button
              v-if="!uploadFile"
              @click="$refs.fileInput.click()"
              class="btn-select-file"
            >
              选择文件
            </button>
          </div>
          <div class="form-group">
            <label>文档标题（可选）</label>
            <input
              v-model="uploadTitle"
              type="text"
              placeholder="留空则使用文件名"
              class="form-input"
            />
          </div>
        </div>
        <div class="modal-footer">
          <button 
            @click.stop="showUploadModal = false" 
            class="btn-cancel"
            type="button"
          >
            取消
          </button>
          <button
            @click.stop.prevent="handleUpload"
            :disabled="!uploadFile || uploading"
            class="btn-primary"
            type="button"
          >
            <span v-if="uploading">上传�?..</span>
            <span v-else>上传</span>
          </button>
        </div>
      </div>
    </div>
    </Teleport>

    <!-- 文档详情/问答模态框 -->
    <div v-if="showQueryModal && selectedDocument" class="modal-overlay" @click="showQueryModal = false">
      <div class="modal-content modal-large" @click.stop>
        <div class="modal-header">
          <h3>{{ selectedDocument.title }} - 问答</h3>
          <button @click="showQueryModal = false" class="modal-close">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body query-body">
          <!-- 问答区域 -->
          <div class="query-section">
            <div class="query-input-area">
              <textarea
                v-model="queryText"
                placeholder="输入您的问题..."
                class="query-input"
                rows="3"
                @keydown.ctrl.enter="submitQuery"
              ></textarea>
              <button
                @click="submitQuery"
                :disabled="!queryText.trim() || querying"
                class="btn-query"
              >
                <span v-if="querying">思考中...</span>
                <span v-else>提问</span>
              </button>
            </div>

            <!-- 问答历史 -->
            <div class="query-history" ref="queryHistoryRef">
              <div
                v-for="(item, index) in queryHistory"
                :key="index"
                class="query-item"
              >
                <div class="query-question">
                  <i class="fas fa-user"></i>
                  <span>{{ item.query }}</span>
                </div>
                <div class="query-answer">
                  <i class="fas fa-robot"></i>
                  <div class="answer-content">
                    <!-- 加载状�?-->
                    <div v-if="item.loading" class="loading-answer">
                      <i class="fas fa-spinner fa-spin"></i>
                      <span>正在思考中...</span>
                    </div>
                    <!-- 答案内容 -->
                    <div v-if="!item.loading && item.answer" class="answer-text">
                      <div class="answer-label">
                        <i class="fas fa-lightbulb"></i>
                        <span>AI 回答�?/span>
                      </div>
                      <div class="answer-body" v-html="formatAnswer(item.answer)"></div>
                    </div>
                    <!-- 错误状�?-->
                    <div v-else-if="!item.loading && item.error" class="error-answer">
                      <i class="fas fa-exclamation-triangle"></i>
                      <span>{{ item.error }}</span>
                    </div>
                    <!-- 如果没有答案也没有错误，显示提示 -->
                    <div v-else-if="!item.loading && !item.answer && !item.error" class="no-answer">
                      <i class="fas fa-info-circle"></i>
                      <span>未能获取到答案，请稍后重�?/span>
                    </div>
                    <!-- 相关片段已移除，只显示AI回答 -->
                  </div>
                </div>
              </div>
              <div v-if="queryHistory.length === 0 && !querying" class="empty-history">
                <i class="fas fa-comments"></i>
                <p>暂无问答记录</p>
                <p class="empty-hint">输入问题开始对话吧�?/p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, nextTick } from 'vue'
import { useApi } from '~/composables/useApi'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

// 统一 API 响应接口
interface ApiResponse<T> {
  code: number
  message: string
  data: T
}

// 使用 Composition API
const api = useApi()

// 响应式数�?const documents = ref<any[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)
const filterStatus = ref('')

const showUploadModal = ref(false)
const uploadFile = ref<File | null>(null)
const uploadTitle = ref('')
const uploading = ref(false)

const showQueryModal = ref(false)
const selectedDocument = ref<any>(null)
const queryText = ref('')
const queryHistory = ref<any[]>([])
const querying = ref(false)

// 计算属�?const totalPages = computed(() => Math.ceil(total.value / pageSize.value))

// 加载文档列表
const loadDocuments = async () => {
  try {
    const params: any = {
      page: currentPage.value,
      pageSize: pageSize.value
    }
    if (filterStatus.value) {
      params.status = filterStatus.value
    }

    const result = await api.get('/Document/list', { params })
    documents.value = result.list || []
    total.value = result.total || 0
  } catch (error: any) {
    console.error('加载文档列表失败:', error)
    alert('加载文档列表失败: ' + (error.message || '未知错误'))
  }
}

// 上传文档
const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    uploadFile.value = target.files[0]
  }
}

const handleDrop = (event: DragEvent) => {
  if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
    uploadFile.value = event.dataTransfer.files[0]
  }
}

const handleUpload = async () => {
  if (!uploadFile.value) {
    alert('请先选择要上传的文件')
    return
  }

  console.log('开始上传文�?', uploadFile.value.name, uploadFile.value.size)

  uploading.value = true
  try {
    const formData = new FormData()
    formData.append('file', uploadFile.value)
    if (uploadTitle.value) {
      formData.append('title', uploadTitle.value)
    }

    console.log('FormData 已创建，准备发送请�?..')

    // 使用 $fetch 直接上传，不要手动设�?Content-Type，让浏览器自动处�?    const config = useRuntimeConfig()
    const baseUrl = typeof window !== 'undefined' && (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1')
      ? 'http://localhost:5234/api'
      : config.public.apiBase

    // 获取 token
    const token = typeof window !== 'undefined' ? localStorage.getItem('admin_token') : null

    console.log('API Base URL:', baseUrl)
    console.log('Token:', token ? '已设�? : '未设�?)

    const response = await $fetch<ApiResponse<any>>('/Document/upload', {
      baseURL: baseUrl,
      method: 'POST',
      body: formData,
      headers: token ? {
        Authorization: `Bearer ${token}`
      } : {}
    })

    console.log('上传响应:', response)

    // 处理响应
    if (response.code !== undefined && response.code !== 0) {
      throw new Error(response.message || '上传失败')
    }

    const result = response.code === 0 ? response.data : response

    alert('文档上传成功，正在处理中...')
    showUploadModal.value = false
    uploadFile.value = null
    uploadTitle.value = ''
    await loadDocuments()
  } catch (error: any) {
    console.error('上传文档失败:', error)
    console.error('错误详情:', {
      message: error?.message,
      response: error?.response,
      data: error?.data,
      status: error?.status,
      statusText: error?.statusText
    })
    
    let errorMessage = '未知错误'
    if (error?.response?.data?.message) {
      errorMessage = error.response.data.message
    } else if (error?.data?.message) {
      errorMessage = error.data.message
    } else if (error?.message) {
      errorMessage = error.message
    } else if (error?.response?.status === 401) {
      errorMessage = '未授权，请重新登�?
    } else if (error?.response?.status === 400) {
      errorMessage = '请求参数错误'
    } else if (error?.response?.status === 500) {
      errorMessage = '服务器内部错�?
    }
    
    alert('上传文档失败: ' + errorMessage)
  } finally {
    uploading.value = false
  }
}

// 查看文档详情
const viewDocument = async (doc: any) => {
  try {
    const result = await api.get(`/Document/${doc.id}`)
    console.log('文档详情:', result)
    // TODO: 可以打开详情模态框显示更多信息
    alert(`文档详情:\n标题: ${result.title}\n摘要: ${result.summary || '�?}\n状�? ${result.status}`)
  } catch (error: any) {
    console.error('获取文档详情失败:', error)
    alert('获取文档详情失败: ' + (error.message || '未知错误'))
  }
}

// 问答文档
const queryDocument = (doc: any) => {
  selectedDocument.value = doc
  queryHistory.value = []
  queryText.value = ''
  showQueryModal.value = true
}

// 提交问题
const submitQuery = async () => {
  if (!queryText.value.trim() || !selectedDocument.value || querying.value) return

  const question = queryText.value.trim()
  queryText.value = ''

  // 添加到历史（先显示问题和加载状态）
  const queryItem = {
    query: question,
    answer: '',
    relevantChunks: [],
    loading: true,
    error: null as string | null
  }
  queryHistory.value.push(queryItem)

  // 滚动到底�?  nextTick(() => {
    scrollToBottom()
  })

  querying.value = true
  try {
    const result = await api.post(`/Document/${selectedDocument.value.id}/query`, {
      query: question,
      topK: 5
    })

    // 更新最后一条历史记�?    queryItem.loading = false
    
    // 解析返回的数据（支持多种格式�?    // 注意：API 返回格式可能�?{ data: { answer: "...", relevantChunks: [...] } }
    let answer = ''
    let relevantChunks: any[] = []
    
    // 尝试多种可能的路�?    if (result.data) {
      // 格式: { data: { answer: "...", relevantChunks: [...] } }
      answer = result.data.answer || result.data.Answer || ''
      relevantChunks = result.data.relevantChunks || result.data.RelevantChunks || []
    } else if (result.answer !== undefined) {
      // 格式: { answer: "...", relevantChunks: [...] }
      answer = result.answer || result.Answer || ''
      relevantChunks = result.relevantChunks || result.RelevantChunks || []
    }
    
    // 记录原始答案用于调试
    console.log('问答结果 - 原始数据:', { 
      rawAnswer: answer, 
      answerLength: answer?.length || 0,
      answerType: typeof answer,
      relevantChunksCount: relevantChunks?.length || 0,
      fullResult: result,
      resultKeys: Object.keys(result || {})
    })
    
    queryItem.relevantChunks = relevantChunks || []
    
    // 检查是否是模拟回复（说�?API 调用失败�?    const isMockResponse = String(answer).includes('【模拟模型回复�?) || 
                          String(answer).includes('【模拟流式回复�?) ||
                          String(answer).includes('模拟模型回复') ||
                          String(answer).includes('模拟流式回复')
    
    if (isMockResponse) {
      queryItem.error = '⚠️ DeepSeek API 调用失败，返回了模拟数据。请检查：1) DEEPSEEK_API_KEY 是否配置 2) API Key 是否有效 3) 网络连接是否正常 4) 查看 Python 服务日志'
      queryItem.answer = ''
      console.error('检测到模拟回复，说�?DeepSeek API 调用失败')
      console.error('请检�?ai-service/.env 文件中的 DEEPSEEK_API_KEY �?DEEPSEEK_BASE_URL 配置')
      console.error('完整返回结果:', JSON.stringify(result, null, 2))
    } else if (!answer || (typeof answer === 'string' && answer.trim() === '')) {
      queryItem.error = '未能获取到答案，请检查：1) 文档是否已处理完�?2) 查看控制台日�?
      queryItem.answer = ''
      console.error('答案为空，完整返回结�?', JSON.stringify(result, null, 2))
    } else {
      queryItem.error = null
      // 格式化答案（但不过度清理�?      const formatted = formatAnswer(String(answer))
      console.log('答案格式�?- 原始 (�?00字符):', String(answer).substring(0, 200))
      console.log('答案格式�?- 处理�?(�?00字符):', formatted.substring(0, 200))
      
      // 如果格式化后答案太短，使用原始答�?      if (formatted.length < 20 && String(answer).length > 20) {
        console.warn('格式化后答案过短，使用原始答�?)
        queryItem.answer = String(answer)
      } else {
        queryItem.answer = formatted || String(answer)
      }
    }
  } catch (error: any) {
    console.error('问答失败:', error)
    queryItem.loading = false
    queryItem.error = '问答失败: ' + (error.message || error.response?.data?.message || '未知错误')
  } finally {
    querying.value = false
    // 滚动到底部显示答�?    nextTick(() => {
      scrollToBottom()
    })
  }
}

// 格式化答案（移除系统提示、问题重复等，但不过度清理）
const formatAnswer = (answer: string): string => {
  if (!answer) return ''
  
  let formatted = answer.trim()
  
  // 第一步：移除所有系统提示和模拟回复标记
  formatted = formatted
    .replace(/【系统提示[^】]*�?g, '')
    .replace(/\[系统提示[^\]]*\]/g, '')
    .replace(/系统提示[�?][^\n]*/g, '')
    .replace(/【模拟模型回复�?g, '')
    .replace(/【模拟流式回复�?g, '')
    .replace(/模拟模型回复/g, '')
    .replace(/模拟流式回复/g, '')
  
  // 第二步：移除"用户问题:"及其后面的问题内容（只移除开头的�?  // 匹配模式：用户问�? xxx �?用户问题：xxx
  formatted = formatted.replace(/^用户问题[�?]\s*[^\n]+\n*/i, '')
  formatted = formatted.replace(/^问题[�?]\s*[^\n]+\n*/i, '')
  
  // 第三步：移除"相关文档片段:"及其后面的所有片段内�?  // 匹配�?相关文档片段:"开始到字符串结尾的所有内�?  const relatedChunksIndex = formatted.search(/相关(文档)?片段[�?]/i)
  if (relatedChunksIndex >= 0) {
    formatted = formatted.substring(0, relatedChunksIndex).trim()
  }
  
  // 第四步：移除所有片段标记（�?[片段 0]: ... �?[片段0]: ...�?  // 匹配完整的片段块，包括多行内�?  formatted = formatted.replace(/\[片段\s*\d+\][�?]\s*[^\n]*(?:\n(?!\[片段)[^\n]*)*/g, '')
  
  // 第五步：移除"片段摘要:"等标�?  formatted = formatted.replace(/片段摘要[�?][^\n]*/g, '')
  formatted = formatted.replace(/片段\s*\d+\s*的摘要[�?][^\n]*/g, '')
  
  // 第六步：清理多余的空白和换行
  formatted = formatted.replace(/\n{3,}/g, '\n\n')
  formatted = formatted.replace(/^\s+|\s+$/g, '')
  
  // 第七步：如果清理后答案为空或太短，尝试提取可能的答案部分
  if (!formatted || formatted.length < 10) {
    // 尝试从原始答案中提取可能的答案（�?用户问题"之后�?相关文档片段"之前�?    const userQIndex = answer.search(/用户问题[�?]/i)
    const relatedIndex = answer.search(/相关(文档)?片段[�?]/i)
    
    if (userQIndex >= 0 && relatedIndex > userQIndex) {
      // 提取用户问题和相关片段之间的内容
      let extracted = answer.substring(userQIndex, relatedIndex)
      // 移除"用户问题: xxx"部分
      extracted = extracted.replace(/用户问题[�?]\s*[^\n]+\n*/i, '')
      extracted = extracted.trim()
      
      if (extracted.length > 10) {
        formatted = extracted
      }
    }
  }
  
  // 第八步：如果还是为空，返回提示信�?  if (!formatted || formatted.length < 5) {
    console.warn('答案格式化后为空，原始答�?', answer.substring(0, 200))
    return '<p>抱歉，未能从返回内容中提取到有效答案。这可能是由�?LLM 返回格式异常。请检�?DeepSeek API 配置是否正确�?/p>'
  }
  
  // 第九步：格式化输出（转换�?HTML�?  formatted = formatted.replace(/\n\n+/g, '</p><p>')
  formatted = formatted.replace(/\n/g, '<br>')
  if (!formatted.startsWith('<p>')) {
    formatted = '<p>' + formatted
  }
  if (!formatted.endsWith('</p>')) {
    formatted = formatted + '</p>'
  }
  
  return formatted
}

// 截断文本
const truncateText = (text: string, maxLength: number): string => {
  if (!text) return ''
  if (text.length <= maxLength) return text
  return text.substring(0, maxLength) + '...'
}

// 滚动到底�?const queryHistoryRef = ref<HTMLElement | null>(null)
const scrollToBottom = () => {
  if (queryHistoryRef.value) {
    queryHistoryRef.value.scrollTop = queryHistoryRef.value.scrollHeight
  }
}

// 重试处理文档
const retryProcessDocument = async (id: number) => {
  if (!confirm('确定要重新处理这个文档吗�?)) return

  try {
    await api.post(`/Document/${id}/retry`)
    alert('已开始重新处理文档，请稍后刷新查看状�?)
    await loadDocuments()
  } catch (error: any) {
    console.error('重试处理文档失败:', error)
    alert('重试处理失败: ' + (error.message || '未知错误'))
  }
}

// 删除文档
const deleteDocument = async (id: number) => {
  if (!confirm('确定要删除这个文档吗�?)) return

  try {
    await api.delete(`/Document/${id}`)
    alert('删除成功')
    await loadDocuments()
  } catch (error: any) {
    console.error('删除文档失败:', error)
    alert('删除文档失败: ' + (error.message || '未知错误'))
  }
}

// 分页
const changePage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
    loadDocuments()
  }
}

// 工具函数
const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

const formatDate = (date: string): string => {
  return new Date(date).toLocaleString('zh-CN')
}

const getStatusText = (status: string): string => {
  const statusMap: Record<string, string> = {
    pending: '待处�?,
    processing: '处理�?,
    completed: '已完�?,
    failed: '处理失败'
  }
  return statusMap[status] || status
}

// 生命周期
onMounted(() => {
  loadDocuments()
})
</script>

<style scoped>
/* 全局移除所有文字发光效�?*/
.document-agent-page,
.document-agent-page *,
.modal-content,
.modal-content * {
  text-shadow: none !important;
  filter: none !important; /* 移除可能的滤镜效�?*/
}

.document-agent-page {
  padding: 24px;
  color: var(--color-border-default) !important; /* 确保页面文字是深�?*/
  background: transparent !important;
}

/* 强制覆盖全局深色主题样式 */
.document-agent-page * {
  color: inherit;
}

.document-agent-page .page-header,
.document-agent-page .section-header,
.document-agent-page .document-table th,
.document-agent-page .document-table td,
.document-agent-page .filter-select,
.document-agent-page .btn-refresh {
  color: var(--color-border-default) !important;
  text-shadow: none !important; /* 移除文字发光效果 */
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  color: var(--color-border-default) !important;
  gap: 20px;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  margin: 0;
  color: var(--color-border-default) !important;
  text-shadow: none !important; /* 移除所有文字发光效�?*/
}

.btn-primary {
  background: var(--color-primary);
  color: var(--color-bg-light, white);
  border: none;
  padding: 10px 24px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  position: relative;
  z-index: 10002;
  min-width: 100px;
  font-weight: 500;
  transition: all 0.2s;
  box-shadow: 0 1px 2px 0 var(--color-border);
}

.btn-primary:hover:not(:disabled) {
  background: var(--color-primary-hover);
  box-shadow: 0 4px 6px -1px var(--shadow);
  transform: translateY(-1px);
}

.btn-primary:active:not(:disabled) {
  transform: translateY(0);
}

.btn-primary:disabled {
  background: var(--color-gray-400);
  cursor: not-allowed;
  opacity: 0.6;
  transform: none;
  box-shadow: none;
}

.document-list-section {
  background: var(--color-bg-light, white) !important;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 1px 3px var(--shadow);
  color: var(--color-border-default) !important; /* 确保文字是深�?*/
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.section-header h2 {
  font-size: 18px;
  font-weight: 600;
  color: var(--color-border-default) !important;
  margin: 0;
  text-shadow: none !important; /* 移除文字发光效果 */
}

.filter-controls {
  display: flex;
  gap: 10px;
  align-items: center;
}

.filter-select {
  padding: 8px 12px;
  border: 1px solid var(--color-border-subtle);
  border-radius: 6px;
  font-size: 14px;
  background: var(--color-bg-light) !important;
  color: var(--color-border-default) !important;
}

.filter-select option {
  background: var(--color-bg-light);
  color: var(--color-border-default);
}

.btn-refresh {
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  color: var(--color-border-default) !important;
  transition: all 0.2s;
}

.btn-refresh:hover {
  background: var(--color-border);
  color: var(--color-text-main) !important;
}

.table-container {
  overflow-x: auto;
}

.document-table {
  width: 100%;
  border-collapse: collapse;
}

.document-table th {
  background: var(--color-bg-elevated) !important;
  padding: 12px;
  text-align: left;
  font-weight: 600;
  border-bottom: 2px solid var(--color-border);
  color: var(--color-border-default) !important;
  font-size: 14px;
  text-shadow: none !important; /* 移除文字发光效果 */
}

.document-table td {
  padding: 12px;
  border-bottom: 1px solid var(--color-border);
  color: var(--color-border-default) !important;
  font-size: 14px;
  background: var(--color-bg-light) !important;
  text-shadow: none !important; /* 移除文字发光效果 */
}

.document-row {
  background: var(--color-bg-light) !important;
}

.document-row:hover {
  background: var(--color-bg-elevated) !important;
}

.document-row td {
  color: var(--color-border-default) !important;
  background: inherit !important;
}

.title-cell {
  max-width: 300px;
}

.doc-title {
  font-weight: 600;
  display: block;
  margin-bottom: 4px;
  color: var(--color-text-main);
  font-size: 14px;
  text-decoration: none;
  cursor: default;
  pointer-events: none;
  text-shadow: none !important; /* 移除文字发光效果 */
}

.doc-summary {
  font-size: 12px;
  color: var(--color-text-sec);
  overflow: hidden;
  text-overflow: ellipsis;
  var(--color-bg-light, white)-space: nowrap;
}

.file-type-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-pending {
  background: var(--color-warning-soft);
  color: var(--color-warning-dark);
}

.status-processing {
  background: var(--color-primary-soft);
  color: var(--color-primary-dark);
}

.status-completed {
  background: var(--color-success-soft);
  color: var(--color-success-dark);
}

.status-failed {
  background: var(--color-danger-soft);
  color: var(--color-danger-dark);
}

.error-message {
  margin-top: 4px;
  font-size: 11px;
  color: var(--color-danger-dark);
  display: flex;
  align-items: center;
  gap: 4px;
  max-width: 200px;
  word-break: break-word;
}

.error-message i {
  flex-shrink: 0;
}

.action-cell {
  display: flex;
  gap: 8px;
}

.btn-action {
  background: transparent;
  border: 1px solid var(--color-border-subtle);
  padding: 6px 10px;
  border-radius: 4px;
  cursor: pointer;
  color: var(--color-text-sec);
}

.btn-action:hover:not(:disabled) {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}

.btn-action:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-danger:hover:not(:disabled) {
  background: var(--color-danger-soft);
  color: var(--color-danger-dark);
  border-color: var(--color-danger-400);
}

.btn-retry {
  color: var(--color-primary);
  border-color: var(--color-primary);
}

.btn-retry:hover:not(:disabled) {
  background: var(--color-primary-soft);
  color: var(--color-primary-hover);
  border-color: var(--color-primary-hover);
}

.empty-cell {
  text-align: center;
  color: var(--color-text-muted);
  padding: 40px;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin-top: 20px;
}

.page-btn {
  padding: 8px 16px;
  border: 1px solid var(--color-border-subtle);
  background: var(--color-bg-light);
  border-radius: 6px;
  cursor: pointer;
}

.page-btn:hover:not(:disabled) {
  background: var(--color-bg-elevated);
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* 模态框样式 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  backdrop-filter: blur(4px);
}

.modal-content {
  background: var(--color-bg-light) !important;
  border-radius: 8px;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  display: flex;
  flex-direction: column;
  position: relative;
  z-index: 10000;
  color: var(--color-border-default) !important; /* 确保模态框内所有文字都是深�?*/
}

/* 强制覆盖全局深色主题样式 */
.modal-content * {
  color: inherit;
  text-shadow: none !important; /* 移除所有文字发光效�?*/
}

.modal-content .query-question,
.modal-content .query-answer,
.modal-content .answer-text,
.modal-content .chunk-content,
.modal-content .chunk-index,
.modal-content .chunks-title {
  color: var(--color-border-default) !important;
  text-shadow: none !important; /* 移除文字发光效果 */
}

.modal-large {
  max-width: 900px;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid var(--color-border);
  background: var(--color-bg-light) !important;
  color: var(--color-border-default) !important;
}

.modal-header h3 {
  margin: 0;
  font-size: 20px;
  color: var(--color-border-default) !important;
  font-weight: 600;
  text-shadow: none !important; /* 移除文字发光效果 */
}

.modal-close {
  background: transparent;
  border: none;
  font-size: 20px;
  cursor: pointer;
  color: var(--color-text-sec);
}

.modal-body {
  padding: 20px;
  overflow-y: auto;
  flex: 1;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 20px;
  border-top: 1px solid var(--color-border);
  position: relative;
  z-index: 10001;
  background: var(--color-bg-light);
  flex-shrink: 0;
  margin-top: auto;
}

.btn-cancel {
  padding: 10px 20px;
  border: 1px solid var(--color-border-subtle);
  background: var(--color-bg-light);
  border-radius: 6px;
  cursor: pointer;
}

.btn-cancel:hover {
  background: var(--color-bg-elevated);
}

/* 上传区域 */
.upload-area {
  border: 2px dashed var(--color-border-subtle);
  border-radius: 8px;
  padding: 40px;
  text-align: center;
  margin-bottom: 20px;
}

.upload-placeholder {
  color: var(--color-text-sec);
}

.upload-placeholder i {
  font-size: 48px;
  margin-bottom: 16px;
  display: block;
}

.upload-hint {
  font-size: 12px;
  color: var(--color-text-muted);
  margin-top: 8px;
}

.upload-file-info {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  background: var(--color-bg-elevated);
  border-radius: 6px;
}

.btn-select-file {
  margin-top: 16px;
  padding: 10px 20px;
  background: var(--color-primary);
  color: var(--color-bg-light, white);
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.btn-remove {
  margin-left: auto;
  background: transparent;
  border: none;
  color: var(--color-danger);
  cursor: pointer;
}

.form-group {
  margin-bottom: 16px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
}

.form-input {
  width: 100%;
  padding: 10px;
  border: 1px solid var(--color-border-subtle);
  border-radius: 6px;
  font-size: 14px;
}

/* 问答区域 */
.query-body {
  padding: 0;
}

.query-section {
  height: 600px;
  display: flex;
  flex-direction: column;
}

.query-input-area {
  padding: 20px;
  border-bottom: 2px solid var(--color-border);
  display: flex;
  gap: 12px;
  background: var(--color-bg-elevated) !important;
  color: var(--color-border-default) !important;
}

.query-input {
  flex: 1;
  padding: 12px;
  border: 1px solid var(--color-border-subtle);
  border-radius: 6px;
  font-size: 14px;
  resize: none;
  font-family: inherit;
  transition: border-color 0.2s;
  background: var(--color-bg-light) !important;
  color: var(--color-border-default) !important;
}

.query-input:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px var(--color-primary-soft);
  background: var(--color-bg-light) !important;
  color: var(--color-border-default) !important;
}

.query-input::placeholder {
  color: var(--color-text-muted) !important;
}

.btn-query {
  padding: 12px 24px;
  background: var(--color-primary);
  color: var(--color-bg-light, white);
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  var(--color-bg-light, white)-space: nowrap;
  transition: all 0.2s;
  font-weight: 500;
}

.btn-query:hover:not(:disabled) {
  background: var(--color-primary-hover);
  transform: translateY(-1px);
  box-shadow: 0 4px 6px var(--shadow);
}

.btn-query:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.query-history {
  flex: 1;
  overflow-y: auto;
  padding: 24px;
  background: var(--color-bg-card) !important;
  color: var(--color-border-default) !important;
}

.query-history::-webkit-scrollbar {
  width: 8px;
}

.query-history::-webkit-scrollbar-track {
  background: var(--color-bg-elevated);
  border-radius: 4px;
}

.query-history::-webkit-scrollbar-thumb {
  background: var(--color-border-subtle);
  border-radius: 4px;
}

.query-history::-webkit-scrollbar-thumb:hover {
  background: var(--color-text-muted);
}

.query-item {
  margin-bottom: 24px;
}

.query-question {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  margin-bottom: 16px;
  padding: 16px;
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-text-sub);
  border-radius: 8px;
  box-shadow: 0 1px 2px var(--color-border);
}

.query-question * {
  text-shadow: none !important; /* 移除文字发光效果 */
}

.query-question i {
  color: var(--color-primary);
  margin-top: 4px;
  font-size: 16px;
  flex-shrink: 0;
}

.query-question span {
  color: var(--color-border-default) !important;
  font-size: 15px;
  font-weight: 500;
  line-height: 1.6;
  flex: 1;
  text-shadow: none !important; /* 移除文字发光效果 */
}

.query-answer {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 16px;
  background: var(--color-bg-card);
  border: 1px solid var(--color-text-sub);
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  margin-bottom: 20px;
}

.query-answer i {
  color: var(--color-success);
  margin-top: 4px;
  font-size: 16px;
  flex-shrink: 0;
}

.answer-content {
  flex: 1;
  min-width: 0; /* 防止内容溢出 */
}

.answer-text {
  width: 100%;
}

.answer-label {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
  font-weight: 600;
  color: var(--color-primary-hover) !important;
  font-size: 15px;
}

.answer-label i {
  color: var(--color-warning);
  font-size: 16px;
}

.answer-body {
  line-height: 1.8;
  color: var(--color-text-main) !important;
  font-size: 15px;
  font-weight: 400;
  word-wrap: break-word;
  word-break: break-word;
  padding: 16px;
  background: var(--color-bg-card);
  border-radius: 8px;
  border-left: 4px solid var(--color-primary);
  box-shadow: 0 1px 3px var(--shadow);
  text-shadow: none !important; /* 移除文字发光效果 */
}

.answer-body p {
  margin: 0 0 12px 0;
  color: var(--color-text-main) !important;
  font-weight: 400;
}

.answer-body * {
  color: var(--color-text-main) !important;
}

/* 确保所有文本元素都清晰可见 */
.answer-body span,
.answer-body div,
.answer-body strong,
.answer-body em,
.answer-body b,
.answer-body i:not(.fas):not(.far):not(.fab) {
  color: var(--color-text-main) !important;
  text-shadow: none !important; /* 移除文字发光效果 */
}

.answer-body p:last-child {
  margin-bottom: 0;
}

.no-answer {
  display: flex;
  align-items: center;
  gap: 10px;
  color: var(--color-text-sec);
  font-size: 14px;
  padding: 12px;
  background: var(--color-bg-elevated);
  border-radius: 6px;
  border: 1px dashed var(--color-border-subtle);
}

.no-answer i {
  color: var(--color-text-sec);
}

.loading-answer {
  display: flex;
  align-items: center;
  gap: 10px;
  color: var(--color-text-sec);
  font-size: 14px;
  padding: 8px 0;
}

.loading-answer i {
  color: var(--color-primary);
  font-size: 16px;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.error-answer {
  display: flex;
  align-items: center;
  gap: 10px;
  color: var(--color-danger-dark);
  font-size: 14px;
  font-weight: 500;
  padding: 8px 0;
}

.error-answer i {
  color: var(--color-danger-dark);
  font-size: 16px;
}

.relevant-chunks {
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid var(--color-primary-soft);
}

.chunks-title {
  font-weight: 600;
  margin-bottom: 12px;
  color: var(--color-primary-dark);
}

.chunk-item {
  margin-bottom: 12px;
  padding: 12px;
  background: var(--color-bg-light);
  border-radius: 6px;
  border: 1px solid var(--color-primary-soft);
}

.chunk-index {
  font-weight: 600;
  color: var(--color-primary);
  margin-right: 12px;
}

.chunk-score {
  color: var(--color-text-sec);
  font-size: 12px;
}

.chunk-content {
  margin-top: 8px;
  color: var(--color-text-main);
  line-height: 1.6;
}

.empty-history {
  text-align: center;
  color: var(--color-text-muted);
  padding: 80px 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}

.empty-history i {
  font-size: 48px;
  color: var(--color-border-subtle);
  margin-bottom: 8px;
}

.empty-history p {
  margin: 0;
  font-size: 15px;
}

.empty-hint {
  font-size: 13px;
  color: var(--color-text-muted);
}
</style>

