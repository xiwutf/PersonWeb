<template>
  <div class="my-licenses">
    <div class="container">
      <h1>我的许可证</h1>

      <div class="license-list" v-if="licenses.length > 0">
        <div v-for="license in licenses" :key="license.id" class="license-card">
          <div class="license-header">
            <h3>{{ license.moduleKey }}</h3>
            <span :class="['status', license.status]">
              {{ getStatusText(license.status) }}
            </span>
          </div>

          <div class="license-details">
            <p><strong>版本：</strong> v{{ license.version }}</p>
            <p><strong>类型：</strong> {{ getLicenseTypeText(license.type) }}</p>
            <p><strong>有效期：</strong> {{ formatDate(license.validFrom) }} 至 {{ license.validUntil ? formatDate(license.validUntil) : '永久' }}</p>
            <p><strong>激活次数：</strong> {{ license.activationsUsed }} / {{ license.maxActivations }}</p>
            <p v-if="license.lastActivatedAt"><strong>最后激活：</strong> {{ formatDate(license.lastActivatedAt) }}</p>
          </div>

          <div class="license-actions">
            <button @click="copyLicenseKey(license.licenseKey)" class="btn btn-sm">
              <i class="fas fa-copy"></i> 复制密钥
            </button>
            <button @click="downloadLicense(license)" class="btn btn-sm btn-primary">
              <i class="fas fa-download"></i> 下载许可证
            </button>
            <button @click="activateLicense(license)" class="btn btn-sm btn-secondary">
              <i class="fas fa-plus"></i> 激活设备
            </button>
          </div>
        </div>
      </div>

      <div v-else class="empty-state">
        <i class="fas fa-inbox"></i>
        <p>您还没有任何许可证</p>
        <button @click="goToModules" class="btn btn-primary">
          浏览模块商店
        </button>
      </div>
    </div>

    <!-- 激活许可证对话框 -->
    <div v-if="showActivationDialog" class="modal" @click.self="closeActivationDialog">
      <div class="modal-content">
        <h3>激活许可证</h3>
        <div class="form-group">
          <label for="deviceId">设备ID</label>
          <input
            type="text"
            id="deviceId"
            v-model="deviceId"
            placeholder="输入设备唯一标识"
          >
        </div>
        <div class="form-group">
          <label for="deviceName">设备名称（可选）</label>
          <input
            type="text"
            id="deviceName"
            v-model="deviceName"
            placeholder="例如：我的笔记本电脑"
          >
        </div>
        <div class="modal-actions">
          <button @click="closeActivationDialog" class="btn btn-outline">取消</button>
          <button @click="confirmActivation" class="btn btn-primary">激活</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const licenses = ref([])
const showActivationDialog = ref(false)
const currentLicense = ref(null)
const deviceId = ref('')
const deviceName = ref('')

onMounted(async () => {
  await fetchLicenses()
})

const fetchLicenses = async () => {
  try {
    const { data } = await useFetch('/api/license/my-licenses')
    if (data.value?.success && Array.isArray(data.value.data)) {
      licenses.value = data.value.data
    } else {
      licenses.value = []
    }
  } catch (error) {
    console.error('Failed to fetch licenses:', error)
    licenses.value = []
  }
}

const getStatusText = (status) => {
  const statusMap = {
    active: '有效',
    expired: '已过期',
    revoked: '已撤销'
  }
  return statusMap[status] || status
}

const getLicenseTypeText = (type) => {
  const typeMap = {
    permanent: '永久',
    subscription: '订阅',
    trial: '试用'
  }
  return typeMap[type] || type
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleString('zh-CN')
}

const copyLicenseKey = async (key) => {
  try {
    await navigator.clipboard.writeText(key)
    alert('许可证密钥已复制到剪贴板')
  } catch (error) {
    console.error('Failed to copy license key:', error)
  }
}

const downloadLicense = (license) => {
  const content = `许可证文件
================
许可证密钥: ${license.licenseKey}
模块标识: ${license.moduleKey}
版本: v${license.version}
类型: ${getLicenseTypeText(license.type)}
生效时间: ${formatDate(license.validFrom)}
到期时间: ${license.validUntil ? formatDate(license.validUntil) : '永久'}
状态: ${getStatusText(license.status)}
创建时间: ${formatDate(license.createdAt)}

此文件请妥善保管，用于激活和使用模块。
`

  const blob = new Blob([content], { type: 'text/plain' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `license-${license.moduleKey}-${license.version}.txt`
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}

const activateLicense = (license) => {
  currentLicense.value = license
  showActivationDialog.value = true
  deviceId.value = ''
  deviceName.value = ''
}

const closeActivationDialog = () => {
  showActivationDialog.value = false
  currentLicense.value = null
}

const confirmActivation = async () => {
  if (!deviceId.value) {
    alert('请输入设备ID')
    return
  }

  try {
    const { data } = await useFetch('/api/license/activate', {
      method: 'POST',
      body: {
        licenseKey: currentLicense.value.licenseKey,
        deviceId: deviceId.value,
        deviceName: deviceName.value
      }
    })

    if (data.value?.success !== false) {
      alert(`设备 ${deviceName.value || deviceId.value} 激活成功！`)
      closeActivationDialog()
      await fetchLicenses()
    } else {
      alert(data.value?.error || '激活失败，请重试')
    }
  } catch (error) {
    console.error('Activation failed:', error)
    alert('激活失败，请重试')
  }
}

const goToModules = () => {
  router.push('/modules')
}
</script>

<style scoped>
.my-licenses {
  padding: 2rem 0;
}

.container {
  max-width: 800px;
  margin: 0 auto;
}

h1 {
  color: var(--color-text-dark);
  margin-bottom: 2rem;
}

.license-list {
  display: grid;
  gap: 1.5rem;
}

.license-card {
  background: var(--color-bg-light, white);
  border: 1px solid var(--color-gray-300);
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 4px var(--shadow);
}

.license-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.license-header h3 {
  margin: 0;
  color: var(--color-text-dark);
  font-size: 1.25rem;
}

.status {
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  font-size: 0.875rem;
  font-weight: 500;
}

.status.active {
  background: var(--color-bg-elevated);
  color: var(--color-success);
}

.status.expired {
  background: var(--color-red-50);
  color: var(--color-red-800);
}

.status.revoked {
  background: var(--color-pink-100);
  color: var(--color-pink-700);
}

.license-details p {
  margin: 0.5rem 0;
  color: var(--color-text-muted);
  font-size: 0.95rem;
}

.license-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 1rem;
  flex-wrap: wrap;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.875rem;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-sm {
  padding: 0.375rem 0.75rem;
  font-size: 0.8125rem;
}

.btn-primary {
  background: var(--color-blue-500);
  color: var(--color-bg-light, white);
}

.btn-primary:hover {
  background: var(--color-blue-700);
}

.btn-secondary {
  background: var(--color-orange-500);
  color: var(--color-bg-light, white);
}

.btn-secondary:hover {
  background: var(--color-orange-600);
}

.btn-outline {
  background: transparent;
  border: 1px solid var(--color-blue-500);
  color: var(--color-blue-500);
}

.btn-outline:hover {
  background: var(--color-blue-500);
  color: var(--color-bg-light, white);
}

.empty-state {
  text-align: center;
  padding: 3rem;
  background: var(--color-gray-100);
  border-radius: 8px;
}

.empty-state i {
  font-size: 4rem;
  color: var(--color-gray-400);
  margin-bottom: 1rem;
}

.empty-state p {
  color: var(--color-text-muted);
  margin-bottom: 1.5rem;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: var(--overlay-color, rgba(0, 0, 0, 0.5));
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: var(--color-bg-light, white);
  border-radius: 8px;
  padding: 2rem;
  max-width: 500px;
  width: 90%;
}

.modal-content h3 {
  margin-top: 0;
  margin-bottom: 1.5rem;
  color: var(--color-text-dark);
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: var(--color-text-muted);
  font-weight: 500;
}

.form-group input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid var(--color-gray-300);
  border-radius: 4px;
  font-size: 1rem;
}

.form-group input:focus {
  outline: none;
  border-color: var(--color-blue-500);
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1.5rem;
}

i {
  font-size: 1rem;
}
</style>