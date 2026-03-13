<template>
  <div class="upload-module-page">
    <!-- 页面标题 -->
    <div class="page-header">
      <nuxt-link to="/admin/modules" class="back-link">← 返回模块列表</nuxt-link>
      <div class="header-content">
        <h1>上传模块版本</h1>
        <p>上传新的模块包或更新现有版本</p>
      </div>
    </div>

    <!-- 上传表单 -->
    <div class="upload-container">
      <div class="upload-card">
        <h2>模块信息</h2>
        <form @submit.prevent="handleUpload" class="upload-form">
          <div class="form-section">
            <h3>基本信息</h3>
            <div class="form-group">
              <label>选择模块 *</label>
              <select
                v-model="formData.moduleKey"
                @change="handleModuleSelect"
                required
              >
                <option value="">请选择模块</option>
                <option
                  v-for="module in modules"
                  :key="module.moduleKey"
                  :value="module.moduleKey"
                >
                  {{ module.moduleName }} ({{ module.moduleKey }})
                </option>
              </select>
              <p class="form-help">请选择要上传的模块</p>
            </div>

            <div class="form-group">
              <label>版本号 *</label>
              <input
                v-model="formData.version"
                type="text"
                placeholder="例如：1.0.0"
                required
                pattern="\d+\.\d+\.\d+"
              />
              <p class="form-help">
                请使用语义化版本号（主版本号.次版本号.修订号）
              </p>
            </div>

            <div class="form-group">
              <label>文件 *</label>
              <input
                ref="fileInput"
                type="file"
                accept=".zip"
                @change="handleFileSelect"
                required
              />
              <div v-if="selectedFile" class="file-info">
                <span class="file-name">{{ selectedFile.name }}</span>
                <span class="file-size">{{ formatFileSize(selectedFile.size) }}</span>
              </div>
              <p class="form-help">
                上传 ZIP 格式的模块包，最大 50MB
              </p>
            </div>

            <div class="form-group">
              <label>是否稳定版本</label>
              <label class="checkbox-label">
                <input
                  v-model="formData.isStable"
                  type="checkbox"
                />
                <span class="checkbox-text">标记为稳定版本</span>
              </label>
            </div>
          </div>

          <div class="form-section">
            <h3>变更日志</h3>
            <div class="form-group">
              <label>版本变更说明 *</label>
              <textarea
                v-model="formData.changelog"
                rows="8"
                placeholder="请描述此版本的变更内容..."
                required
              ></textarea>
              <p class="form-help">
                记录版本的主要改进、修复和变更内容
              </p>
            </div>
          </div>

          <!-- 文件预览 -->
          <div v-if="showPreview" class="preview-section">
            <h3>包内容预览</h3>
            <div class="file-tree">
              <div class="tree-item">
                <span class="folder">📦 {{ selectedFile?.name }}</span>
                <div class="tree-children">
                  <div class="tree-item">
                    <span class="folder">📁 src/</span>
                    <div class="tree-children">
                      <div class="tree-item leaf">📄 index.ts</div>
                      <div class="tree-item leaf">📄 module.json</div>
                    </div>
                  </div>
                  <div class="tree-item">
                    <span class="folder">📁 components/</span>
                  </div>
                  <div class="tree-item">
                    <span class="folder">📁 composables/</span>
                  </div>
                  <div class="tree-item">
                    <span class="folder">📁 types/</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="form-actions">
            <button
              type="button"
              @click="togglePreview"
              class="btn secondary"
            >
              {{ showPreview ? '隐藏预览' : '查看预览' }}
            </button>
            <button
              type="submit"
              class="btn primary"
              :disabled="isUploading"
            >
              <span v-if="isUploading" class="loading">上传中...</span>
              <span v-else>开始上传</span>
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- 上传进度 -->
    <div v-if="isUploading" class="upload-progress">
      <div class="progress-container">
        <div class="progress-bar">
          <div
            class="progress-fill"
            :style="{ width: uploadProgress + '%' }"
          ></div>
        </div>
        <span class="progress-text">{{ uploadProgress }}%</span>
      </div>
    </div>

    <!-- 提示信息 -->
    <div class="upload-tips">
      <h3>上传注意事项</h3>
      <ul>
        <li>
          <strong>格式要求：</strong>
          请确保上传的是 ZIP 压缩包，包含完整的模块结构
        </li>
        <li>
          <strong>文件限制：</strong>
          单个文件最大 50MB，如需上传更大的包，请联系管理员
        </li>
        <li>
          <strong>版本管理：</strong>
          同一模块的不同版本可以共存，但版本号必须唯一
        </li>
        <li>
          <strong>文件结构：</strong>
          建议包含以下目录：src/, components/, composables/, types/
        </li>
        <li>
          <strong>安全检查：</strong>
          系统会自动扫描文件内容，确保不包含恶意代码
        </li>
      </ul>
    </div>

    <!-- 上传成功提示 -->
    <div v-if="uploadSuccess" class="success-message">
      <div class="success-content">
        <span class="success-icon">✅</span>
        <div class="success-text">
          <h3>上传成功！</h3>
          <p>您的模块已成功上传并发布。</p>
          <div class="success-actions">
            <button @click="viewModule" class="btn">查看模块</button>
            <button @click="uploadAnother" class="btn secondary">继续上传</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// 状态管理
const modules = ref([])
const selectedFile = ref(null)
const isUploading = ref(false)
const uploadProgress = ref(0)
 uploadSuccess = ref(false)
const showPreview = ref(false)

const formData = ref({
  moduleKey: '',
  version: '',
  changelog: '',
  isStable: true
})

// 生命周期钩子
onMounted(async () => {
  // 加载模块列表
  try {
    const res = await fetch('/api/modules')
    const data = await res.json()
    modules.value = data.data
  } catch (error) {
    console.error('加载模块列表失败:', error)
  }
})

// 事件处理
const handleModuleSelect = () => {
  // 可以在这里加载模块的现有版本
}

const handleFileSelect = (event) => {
  const file = event.target.files[0]
  if (file) {
    // 验证文件类型
    if (file.type !== 'application/zip' && !file.name.endsWith('.zip')) {
      alert('请上传 ZIP 格式的文件')
      event.target.value = ''
      return
    }

    // 验证文件大小（50MB）
    if (file.size > 50 * 1024 * 1024) {
      alert('文件大小不能超过 50MB')
      event.target.value = ''
      return
    }

    selectedFile.value = file
  }
}

const togglePreview = () => {
  showPreview.value = !showPreview.value
}

const handleUpload = async () => {
  if (!selectedFile.value) {
    alert('请选择要上传的文件')
    return
  }

  isUploading.value = true
  uploadProgress.value = 0

  try {
    // 模拟上传进度
    const progressInterval = setInterval(() => {
      uploadProgress.value += 10
      if (uploadProgress.value >= 90) {
        clearInterval(progressInterval)
      }
    }, 200)

    // 创建表单数据
    const formDataObj = new FormData()
    formDataObj.append('moduleKey', formData.value.moduleKey)
    formDataObj.append('version', formData.value.version)
    formDataObj.append('changelog', formData.value.changelog)
    formDataObj.append('isStable', formData.value.isStable)
    formDataObj.append('file', selectedFile.value)

    // 发送上传请求
    const res = await fetch('/api/modules/uploads', {
      method: 'POST',
      body: formDataObj
    })

    clearInterval(progressInterval)
    uploadProgress.value = 100

    if (res.ok) {
      const data = await res.json()
      uploadSuccess.value = true
      console.log('上传成功:', data)
    } else {
      const error = await res.json()
      alert(`上传失败: ${error.statusMessage}`)
    }
  } catch (error) {
    console.error('上传失败:', error)
    alert('上传过程中发生错误')
  } finally {
    isUploading.value = false
    setTimeout(() => {
      uploadProgress.value = 0
    }, 1000)
  }
}

const viewModule = () => {
  // 跳转到模块详情页
  navigateTo(`/admin/modules/${formData.value.moduleKey}`)
}

const uploadAnother = () => {
  // 重置表单
  formData.value = {
    moduleKey: '',
    version: '',
    changelog: '',
    isStable: true
  }
  selectedFile.value = null
  uploadSuccess.value = false
}

// 工具函数
const formatFileSize = (bytes) => {
  if (!bytes) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}
</script>

<style scoped>
.upload-module-page {
  padding: 2rem;
  max-width: 1000px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 2rem;
}

.back-link {
  display: inline-block;
  margin-bottom: 1rem;
  color: var(--color-primary);
  text-decoration: none;
}

.back-link:hover {
  text-decoration: underline;
}

.header-content h1 {
  font-size: 2rem;
  margin-bottom: 0.5rem;
}

.header-content p {
  color: #666;
}

.upload-container {
  margin-bottom: 3rem;
}

.upload-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  padding: 2rem;
}

.upload-card h2 {
  margin: 0 0 1.5rem 0;
  font-size: 1.5rem;
}

.upload-form {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.form-section {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-section h3 {
  margin: 0;
  font-size: 1.2rem;
  color: #333;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 500;
  color: #333;
}

.form-group input,
.form-group select,
.form-group textarea {
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  font-family: inherit;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-group textarea {
  resize: vertical;
  min-height: 120px;
}

.form-help {
  margin: 0;
  font-size: 0.85rem;
  color: #666;
}

.file-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 0.5rem;
  padding: 0.5rem;
  background: #f8f9fa;
  border-radius: 4px;
}

.file-name {
  font-weight: 500;
}

.file-size {
  color: #666;
  font-size: 0.85rem;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.checkbox-label input[type="checkbox"] {
  margin: 0;
}

.preview-section {
  background: #f8f9fa;
  border-radius: 4px;
  padding: 1rem;
}

.file-tree {
  font-family: monospace;
  padding-left: 1rem;
}

.tree-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.tree-item > span {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.tree-children {
  padding-left: 1.5rem;
  border-left: 1px solid #ddd;
}

.leaf {
  color: #666;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  transition: all 0.2s;
}

.btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

.btn.primary {
  background: var(--color-primary);
  color: white;
}

.btn.primary:hover {
  background: var(--color-primary-hover);
}

.btn.secondary {
  background: white;
  color: #333;
  border: 1px solid #ddd;
}

.btn.secondary:hover {
  background: #f8f9fa;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.upload-progress {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  background: white;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  padding: 1rem;
  z-index: 1000;
}

.progress-container {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.progress-bar {
  flex: 1;
  height: 8px;
  background: var(--color-border);
  border-radius: 4px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: var(--color-primary);
  transition: width 0.3s ease;
}

.progress-text {
  font-weight: 500;
  color: #666;
}

.upload-tips {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  padding: 2rem;
}

.upload-tips h3 {
  margin: 0 0 1rem 0;
}

.upload-tips ul {
  margin: 0;
  padding-left: 1.5rem;
}

.upload-tips li {
  margin-bottom: 0.75rem;
  line-height: 1.6;
}

.upload-tips li strong {
  color: #333;
}

.success-message {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
  padding: 2rem;
  max-width: 500px;
  width: 90%;
  z-index: 1001;
}

.success-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: 1rem;
}

.success-icon {
  font-size: 3rem;
}

.success-text h3 {
  margin: 0;
  color: #10b981;
}

.success-text p {
  color: #666;
  margin: 0;
}

.success-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
}

.success-actions .btn {
  padding: 0.5rem 1rem;
  font-size: 0.9rem;
}

@media (max-width: 768px) {
  .upload-module-page {
    padding: 1rem;
  }

  .upload-card {
    padding: 1rem;
  }

  .form-actions {
    flex-direction: column;
  }

  .btn {
    width: 100%;
  }

  .success-actions {
    flex-direction: column;
  }
}
</style>