<template>
  <div class="my-licenses">
    <div class="container">
      <h1>??????</h1>

      <div class="license-list" v-if="licenses.length > 0">
        <div v-for="license in licenses" :key="license.id" class="license-card">
          <div class="license-header">
            <h3>{{ license.moduleKey }}</h3>
            <span :class="['status', license.status]">
              {{ getStatusText(license.status) }}
            </span>
          </div>

          <div class="license-details">
            <p><strong>???</strong> v{{ license.version }}</p>
            <p><strong>???</strong> {{ getLicenseTypeText(license.type) }}</p>
            <p><strong>????</strong> {{ formatDate(license.validFrom) }} ? {{ license.validUntil ? formatDate(license.validUntil) : '??' }}</p>
            <p><strong>??????</strong> {{ license.activationsUsed }} / {{ license.maxActivations }}</p>
            <p v-if="license.lastActivatedAt"><strong>??????</strong> {{ formatDate(license.lastActivatedAt) }}</p>
          </div>

          <div class="license-actions">
            <button @click="copyLicenseKey(license.licenseKey)" class="btn btn-sm">
              <i class="fas fa-copy"></i> ????
            </button>
            <button @click="downloadLicense(license)" class="btn btn-sm btn-primary">
              <i class="fas fa-download"></i> ??????
            </button>
            <button @click="activateLicense(license)" class="btn btn-sm btn-secondary">
              <i class="fas fa-plus"></i> ????
            </button>
          </div>
        </div>
      </div>

      <div v-else class="empty-state">
        <i class="fas fa-inbox"></i>
        <p>??????????</p>
        <button @click="goToModules" class="btn btn-primary">
          ???????
        </button>
      </div>
    </div>

    <!-- ??????????-->
    <div v-if="showActivationDialog" class="modal" @click.self="closeActivationDialog">
      <div class="modal-content">
        <h3>??????</h3>
        <div class="form-group">
          <label for="deviceId">??ID</label>
          <input
            type="text"
            id="deviceId"
            v-model="deviceId"
            placeholder="?????????"
          >
        </div>
        <div class="form-group">
          <label for="deviceName">?????????</label>
          <input
            type="text"
            id="deviceName"
            v-model="deviceName"
            placeholder="????????????"
          >
        </div>
        <div class="modal-actions">
          <button @click="closeActivationDialog" class="btn btn-outline">??</button>
          <button @click="confirmActivation" class="btn btn-primary">??</button>
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
    active: '??',
    expired: '???',
    revoked: '???'
  }
  return statusMap[status] || status
}

const getLicenseTypeText = (type) => {
  const typeMap = {
    permanent: '??',
    subscription: '??',
    trial: '??'
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
    alert('???????')
  } catch (error) {
    console.error('Failed to copy license key:', error)
  }
}

const downloadLicense = (license) => {
  const content = `???????================
??????? ${license.licenseKey}
????: ${license.moduleKey}
??: v${license.version}
??: ${getLicenseTypeText(license.type)}
????: ${formatDate(license.validFrom)}
????: ${license.validUntil ? formatDate(license.validUntil) : '??'}
??? ${getStatusText(license.status)}
????: ${formatDate(license.createdAt)}

????????????????????????`

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
    alert('?????ID')
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
      alert(`?? ${deviceName.value || deviceId.value} ??????`)
      closeActivationDialog()
      await fetchLicenses()
    } else {
      alert(data.value?.error || '????')
    }
  } catch (error) {
    console.error('Activation failed:', error)
    alert('????')
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
  border: 1px solid var(--color-border-default);
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
  background: var(--color-error-bg);
  color: var(--color-error-text);
}

.status.revoked {
  background: var(--color-error-bg);
  color: var(--color-error-text);
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
  background: var(--color-primary);
  color: var(--color-bg-light, white);
}

.btn-primary:hover {
  background: var(--color-primary-hover);
}

.btn-secondary {
  background: var(--color-warning);
  color: var(--color-bg-light, white);
}

.btn-secondary:hover {
  background: var(--color-warning-icon);
}

.btn-outline {
  background: transparent;
  border: 1px solid var(--color-primary);
  color: var(--color-primary);
}

.btn-outline:hover {
  background: var(--color-primary);
  color: var(--color-bg-light, white);
}

.empty-state {
  text-align: center;
  padding: 3rem;
  background: var(--color-bg-elevated);
  border-radius: 8px;
}

.empty-state i {
  font-size: 4rem;
  color: var(--color-text-quaternary);
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
  border: 1px solid var(--color-border-default);
  border-radius: 4px;
  font-size: 1rem;
}

.form-group input:focus {
  outline: none;
  border-color: var(--color-border-focus);
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