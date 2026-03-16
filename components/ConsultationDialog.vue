<template>
  <Teleport to="body">
    <div v-if="visible" class="modal-overlay" @click.self="handleClose">
      <div class="modal-content consultation-modal">
        <div class="modal-header">
          <h3 class="modal-title">购买前咨询</h3>
          <button @click="handleClose" class="modal-close">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <div class="modal-body">
          <!-- 商品信息（只读） -->
          <div class="form-group">
            <label class="form-label">商品名称</label>
            <input
              type="text"
              :value="productName"
              readonly
              class="form-input bg-gray-50 cursor-not-allowed"
            />
          </div>

          <!-- 客户姓名 -->
          <div class="form-group">
            <label class="form-label">你的称呼 <span class="text-red-500">*</span></label>
            <input
              v-model="form.customerName"
              type="text"
              placeholder="请输入您的姓名"
              class="form-input"
              :class="{ 'border-red-500': errors.customerName }"
            />
            <p v-if="errors.customerName" class="text-red-500 text-sm mt-1">{{ errors.customerName }}</p>
          </div>

          <!-- 联系方式 -->
          <div class="form-group">
            <label class="form-label">联系方式 <span class="text-red-500">*</span></label>
            <p class="text-sm text-gray-500 mb-2">至少填写一种联系方式</p>
            
            <div class="space-y-3">
              <div>
                <input
                  v-model="form.customerPhone"
                  type="tel"
                  placeholder="手机号"
                  class="form-input"
                />
              </div>
              <div>
                <input
                  v-model="form.customerWeChat"
                  type="text"
                  placeholder="微信号"
                  class="form-input"
                />
              </div>
              <div>
                <input
                  v-model="form.customerEmail"
                  type="email"
                  placeholder="邮箱"
                  class="form-input"
                />
              </div>
            </div>
            <p v-if="errors.contact" class="text-red-500 text-sm mt-1">{{ errors.contact }}</p>
          </div>

          <!-- 预算范围 -->
          <div class="form-group">
            <label class="form-label">预算范围</label>
            <select v-model="form.budgetRange" class="form-select">
              <option value="">请选择</option>
              <option value="<500">小于 500 元</option>
              <option value="500-1000">500 - 1000 元</option>
              <option value="1000-3000">1000 - 3000 元</option>
              <option value="3000-5000">3000 - 5000 元</option>
              <option value=">5000">大于 5000 元</option>
              <option value="待评估">待评估</option>
            </select>
          </div>

          <!-- 期望完成时间 -->
          <div class="form-group">
            <label class="form-label">希望完成时间</label>
            <select v-model="form.expectedDeadline" class="form-select">
              <option value="">请选择</option>
              <option value="3天内">3天内</option>
              <option value="一周内">一周内</option>
              <option value="两周内">两周内</option>
              <option value="一个月内">一个月内</option>
              <option value="不着急">不着急</option>
            </select>
          </div>

          <!-- 需求描述 -->
          <div class="form-group">
            <label class="form-label">你想做什么？ <span class="text-red-500">*</span></label>
            <textarea
              v-model="form.requirementDescription"
              rows="5"
              placeholder="请详细描述您的需求..."
              class="form-textarea"
              :class="{ 'border-red-500': errors.requirementDescription }"
            ></textarea>
            <p v-if="errors.requirementDescription" class="text-red-500 text-sm mt-1">{{ errors.requirementDescription }}</p>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="handleClose" class="btn btn-secondary">取消</button>
          <button @click="handleSubmit" :disabled="submitting" class="btn btn-primary">
            <span v-if="submitting">提交中...</span>
            <span v-else>提交咨询</span>
          </button>
        </div>
      </div>
    </div>

    <!-- 提交成功后的提示弹窗 -->
    <div v-if="showSuccess" class="modal-overlay" @click.self="handleSuccessClose">
      <div class="modal-content success-modal">
        <div class="modal-header">
          <h3 class="modal-title text-green-600">
            <i class="fas fa-check-circle mr-2"></i>
            咨询提交成功！
          </h3>
          <button @click="handleSuccessClose" class="modal-close">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <div class="modal-body">
          <p class="text-gray-700 mb-4">感谢您的咨询，我们会尽快与您联系！</p>
          
          <!-- 显示联系方式 -->
          <div v-if="wechatInfo" class="text-center py-4 border-t border-gray-200">
            <p class="text-sm text-gray-600 mb-3">
              <span v-if="wechatInfo.type === 'wechat'">您也可以直接添加我的微信：</span>
              <span v-else-if="wechatInfo.type === 'phone'">您也可以直接拨打我的电话：</span>
              <span v-else-if="wechatInfo.type === 'email'">您也可以直接发送邮件到：</span>
              <span v-else>您也可以直接联系我：</span>
            </p>
            <!-- 联系方式显示 -->
            <div class="mb-3">
              <p v-if="wechatInfo.type === 'email'" class="text-lg font-semibold text-gray-800 break-all">
                <a :href="`mailto:${wechatInfo.wechat}`" class="text-blue-600 hover:text-blue-800 underline">
                  {{ wechatInfo.wechat }}
                </a>
              </p>
              <p v-else-if="wechatInfo.type === 'phone'" class="text-lg font-semibold text-gray-800 break-all">
                <a :href="`tel:${wechatInfo.wechat}`" class="text-blue-600 hover:text-blue-800 underline">
                  {{ wechatInfo.wechat }}
                </a>
              </p>
              <p v-else class="text-lg font-semibold text-gray-800 mb-2 break-all">
                {{ wechatInfo.wechat }}
              </p>
              
              <!-- 微信复制按钮 -->
              <button
                v-if="wechatInfo.type === 'wechat' || (!wechatInfo.type && wechatInfo.wechat && !wechatInfo.wechat.includes('@') && !wechatInfo.wechat.includes('手机') && !wechatInfo.wechat.includes('邮箱'))"
                @click="copyToClipboard(wechatInfo.wechat)"
                class="mt-2 px-4 py-2 bg-green-500 hover:bg-green-600 text-white rounded-lg text-sm transition-colors"
              >
                <i class="fas fa-copy mr-2"></i>
                复制微信号
              </button>
            </div>
            
            <div v-if="wechatInfo.qrcode" class="flex justify-center mb-3">
              <img :src="wechatInfo.qrcode" alt="微信二维码" class="w-48 h-48 border border-gray-300 rounded-lg" />
            </div>
            <p v-if="wechatInfo.qrcode" class="text-xs text-gray-500">扫描二维码或搜索微信号添加</p>
            <p v-else-if="wechatInfo.type === 'wechat' || (!wechatInfo.type && wechatInfo.wechat && !wechatInfo.wechat.includes('@') && !wechatInfo.wechat.includes('手机') && !wechatInfo.wechat.includes('邮箱'))" class="text-xs text-gray-500">搜索微信号添加</p>
            <p v-else-if="wechatInfo.type === 'phone'" class="text-xs text-gray-500">点击号码直接拨打或发送短信</p>
            <p v-else-if="wechatInfo.type === 'email'" class="text-xs text-gray-500">点击邮箱地址直接发送邮件</p>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="handleSuccessClose" class="btn btn-primary">知道了</button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
interface Props {
  visible: boolean
  productId: number
  productName: string
}

interface Emits {
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const api = useApi()
const message = useSafeMessage()

const form = ref({
  customerName: '',
  customerPhone: '',
  customerWeChat: '',
  customerEmail: '',
  budgetRange: '',
  expectedDeadline: '',
  requirementDescription: ''
})

const errors = ref<Record<string, string>>({})
const submitting = ref(false)
const showSuccess = ref(false)
const wechatInfo = ref<{ wechat?: string; qrcode?: string; type?: 'wechat' | 'phone' | 'email' } | null>(null)

// 获取联系方式信息（从配置接口）
const fetchWechatInfo = async () => {
  try {
    // 尝试从配置接口获取联系方式信息
    const configRes = await api.get<Record<string, string>>('/Config')
    if (configRes && typeof configRes === 'object') {
      // 从配置中获取联系方式，支持多种配置键名
      const wechat = configRes.wechat || configRes.wechat_id || configRes.contact_wechat || configRes.微信 || ''
      const qrcode = configRes.wechatQrcode || configRes.wechat_qrcode || configRes.wechat_qr || configRes.微信二维码 || ''
      const phone = configRes.phone || configRes.contact_phone || configRes.手机 || configRes.mobile || ''
      const email = configRes.email || configRes.contact_email || configRes.邮箱 || ''
      
      // 优先显示微信，如果没有微信则显示手机或邮箱
      if (wechat) {
        wechatInfo.value = {
          wechat: wechat,
          qrcode: qrcode || undefined,
          type: 'wechat'
        }
      } else if (phone) {
        wechatInfo.value = {
          wechat: phone,
          qrcode: undefined,
          type: 'phone'
        }
      } else if (email) {
        wechatInfo.value = {
          wechat: email,
          qrcode: undefined,
          type: 'email'
        }
      } else {
        // 如果都没有配置，使用默认联系方式
        wechatInfo.value = {
          wechat: '请联系网站管理员',
          qrcode: undefined
        }
      }
    } else {
      wechatInfo.value = {
        wechat: '请联系网站管理员',
        qrcode: undefined,
        type: undefined
      }
    }
  } catch (e) {
    console.warn('获取联系方式失败，使用默认值', e)
    wechatInfo.value = {
      wechat: '请联系网站管理员',
      qrcode: undefined,
      type: undefined
    }
  }
}

// 表单验证
const validate = (): boolean => {
  errors.value = {}

  if (!form.value.customerName.trim()) {
    errors.value.customerName = '请输入您的姓名'
    return false
  }

  if (!form.value.customerPhone && !form.value.customerWeChat && !form.value.customerEmail) {
    errors.value.contact = '至少需要填写一种联系方式'
    return false
  }

  if (!form.value.requirementDescription.trim()) {
    errors.value.requirementDescription = '请描述您的需求'
    return false
  }

  return true
}

// 提交表单
const handleSubmit = async () => {
  // 先验证表单
  if (!validate()) {
    console.log('表单验证失败:', errors.value)
    return
  }

  // 检查 productId
  if (!props.productId || props.productId === 0) {
    message.error('商品信息无效，请刷新页面重试')
    return
  }

  submitting.value = true
  try {
    console.log('提交咨询数据:', {
      productId: props.productId,
      customerName: form.value.customerName.trim(),
      customerPhone: form.value.customerPhone.trim() || undefined,
      customerWeChat: form.value.customerWeChat.trim() || undefined,
      customerEmail: form.value.customerEmail.trim() || undefined,
      budgetRange: form.value.budgetRange || undefined,
      expectedDeadline: form.value.expectedDeadline || undefined,
      requirementDescription: form.value.requirementDescription.trim()
    })

    const requestData = {
      productId: props.productId,
      customerName: form.value.customerName.trim(),
      customerPhone: form.value.customerPhone.trim() || undefined,
      customerWeChat: form.value.customerWeChat.trim() || undefined,
      customerEmail: form.value.customerEmail.trim() || undefined,
      budgetRange: form.value.budgetRange || undefined,
      expectedDeadline: form.value.expectedDeadline || undefined,
      requirementDescription: form.value.requirementDescription.trim()
    }
    
    console.log('提交咨询数据:', requestData)

    const res = await api.post<any>('/Consultations', requestData)

    console.log('API 响应:', res)

    // useApi 返回的是 data 部分（如果响应是 { code: 0, data: {...} }）
    // 或者直接返回响应（如果响应格式不同）
    // 检查响应是否成功：有 consultationId 表示成功
    if (res && (res.consultationId || (typeof res === 'object' && Object.keys(res).length > 0))) {
      console.log('咨询提交成功，咨询ID:', res.consultationId || res.id)
      
      // 获取微信信息
      await fetchWechatInfo()
      
      // 显示成功弹窗
      showSuccess.value = true
      
      // 重置表单
      form.value = {
        customerName: '',
        customerPhone: '',
        customerWeChat: '',
        customerEmail: '',
        budgetRange: '',
        expectedDeadline: '',
        requirementDescription: ''
      }
      
      emit('success')
    } else {
      console.error('咨询提交失败，响应:', res)
      message.error(res?.message || '提交失败，请稍后重试')
    }
  } catch (e: any) {
    console.error('提交咨询失败:', e)
    console.error('错误详情:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    
    // 显示更详细的错误信息
    const errorMsg = e.response?.data?.message || e.message || '提交失败，请稍后重试'
    message.error(errorMsg)
  } finally {
    submitting.value = false
  }
}

// 关闭弹窗
const handleClose = () => {
  emit('update:visible', false)
}

// 关闭成功弹窗
const handleSuccessClose = () => {
  showSuccess.value = false
  handleClose()
}

// 复制到剪贴板
const copyToClipboard = async (text: string) => {
  try {
    await navigator.clipboard.writeText(text)
    message.success('已复制到剪贴板')
  } catch (e) {
    console.error('复制失败:', e)
    message.error('复制失败，请手动复制')
  }
}

// 监听 visible 变化，初始化时获取微信信息
watch(() => props.visible, (newVal) => {
  if (newVal) {
    fetchWechatInfo()
  }
})
</script>

<style scoped>
.consultation-modal {
  max-width: 600px;
}

.success-modal {
  max-width: 500px;
}

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
  background: white;
  border-radius: 12px;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  display: flex;
  flex-direction: column;
  position: relative;
  z-index: 10000;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid var(--color-border);
}

.modal-title {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: var(--color-border-default);
}

.modal-close {
  background: transparent;
  border: none;
  font-size: 20px;
  cursor: pointer;
  color: var(--color-text-sec);
  padding: 4px 8px;
  border-radius: 4px;
  transition: all 0.2s;
}

.modal-close:hover {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}

.modal-body {
  padding: 20px;
  overflow-y: auto;
  flex: 1;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 20px;
  border-top: 1px solid var(--color-border);
}

.form-group {
  margin-bottom: 20px;
}

.form-label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: var(--color-text-main);
  margin-bottom: 8px;
}

.form-input,
.form-textarea,
.form-select {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid var(--color-border-default);
  border-radius: 6px;
  font-size: 14px;
  color: var(--color-text-main);
  background: var(--color-bg-card);
  transition: all 0.2s;
}

.form-input:focus,
.form-textarea:focus,
.form-select:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-textarea {
  resize: vertical;
  min-height: 100px;
}

.btn {
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
}

.btn-primary {
  background: var(--color-primary);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: var(--color-primary-hover);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-secondary {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}

.btn-secondary:hover {
  background: var(--color-border);
}
</style>

