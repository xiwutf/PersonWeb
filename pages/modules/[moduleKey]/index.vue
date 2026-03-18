<template>
  <div class="module-marketplace">
    <!-- 页面标题 -->
    <div class="page-header">
      <h1>{{ module?.moduleName || '模块详情' }}</h1>
      <p>{{ module?.description }}</p>
    </div>

    <!-- 模块信息卡片 -->
    <div class="module-info-card">
      <div class="module-header">
        <div class="module-icon">
          <span v-if="module?.icon">{{ module.icon }}</span>
          <span v-else>📦</span>
        </div>
        <div class="module-title">
          <h2>{{ module?.moduleName }}</h2>
          <div class="module-meta">
            <span class="badge" :class="module?.category">
              {{ getCategoryName(module?.category) }}
            </span>
            <span class="version">v{{ currentVersion }}</span>
            <span class="author">作者: {{ module?.author }}</span>
          </div>
        </div>
        <div class="module-actions">
          <button
            v-if="!isInstalled"
            @click="installModule"
            class="btn primary"
          >
            安装模块
          </button>
          <button
            v-else
            class="btn secondary"
            disabled
          >
            已安装
          </button>
          <button
            @click="toggleFavorite"
            class="btn favorite"
            :class="{ 'favorited': isFavorited }"
          >
            {{ isFavorited ? '❤️' : '🤍' }}
          </button>
        </div>
      </div>

      <div class="module-stats">
        <div class="stat-item">
          <span class="stat-value">{{ totalDownloads }}</span>
          <span class="stat-label">下载次数</span>
        </div>
        <div class="stat-item">
          <span class="stat-value">{{ averageRating.toFixed(1) }}</span>
          <span class="stat-label">平均评分</span>
        </div>
        <div class="stat-item">
          <span class="stat-value">{{ totalReviews }}</span>
          <span class="stat-label">用户评价</span>
        </div>
        <div class="stat-item">
          <span class="stat-value">{{ moduleDependencies.length }}</span>
          <span class="stat-label">依赖项</span>
        </div>
      </div>

      <div v-if="module?.configSchema" class="module-config">
        <h3>配置项</h3>
        <div class="config-list">
          <div
            v-for="(schema, key) in Object.entries(module.configSchema)"
            :key="key"
            class="config-item"
          >
            <span class="config-name">{{ key }}</span>
            <span class="config-type">{{ schema.type }}</span>
            <span class="config-desc">{{ schema.description }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- 标签页导航 -->
    <div class="tabs">
      <button
        v-for="tab in tabs"
        :key="tab.key"
        @click="activeTab = tab.key"
        :class="{ 'active': activeTab === tab.key }"
        class="tab-btn"
      >
        {{ tab.label }}
      </button>
    </div>

    <!-- 标签页内容 -->
    <div class="tab-content">
      <!-- 版本列表 -->
      <div v-if="activeTab === 'versions'" class="versions-tab">
        <div class="version-filters">
          <label>
            <input
              v-model="showOnlyStable"
              type="checkbox"
            />
            仅显示稳定版本
          </label>
          <label>
            <input
              v-model="showOnlyLatest"
              type="checkbox"
            />
            仅显示最新版本
          </label>
        </div>

        <div class="versions-list">
          <div
            v-for="version in filteredVersions"
            :key="version.version"
            class="version-item"
            :class="{ 'is-latest': version.isLatest }"
          >
            <div class="version-header">
              <div class="version-info">
                <h3>
                  v{{ version.version }}
                  <span
                    v-if="version.isLatest"
                    class="badge latest"
                  >最新版</span>
                  <span
                    v-if="version.isStable"
                    class="badge stable"
                  >稳定版</span>
                </h3>
                <p>{{ formatDate(version.publishedAt) }}</p>
              </div>
              <div class="version-actions">
                <button
                  @click="downloadVersion(version)"
                  class="btn primary"
                >
                  下载
                </button>
                <button
                  @click="viewChangelog(version)"
                  class="btn secondary"
                >
                  变更日志
                </button>
              </div>
            </div>

            <div v-if="version.packageSize" class="version-details">
              <span class="package-size">
                文件大小: {{ formatFileSize(version.packageSize) }}
              </span>
              <span class="checksum">
                SHA256: {{ version.checksum.substring(0, 20) }}...
              </span>
            </div>
          </div>
        </div>

        <!-- 变更日志模态框 -->
        <div v-if="showChangelog" class="modal" @click="closeChangelog">
          <div class="modal-content" @click.stop>
            <div class="modal-header">
              <h3>{{ selectedVersion?.version }} 变更日志</h3>
              <button @click="closeChangelog" class="close-btn">✕</button>
            </div>
            <div class="modal-body">
              <pre>{{ selectedVersion?.changelog }}</pre>
            </div>
          </div>
        </div>
      </div>

      <!-- 评价列表 -->
      <div v-if="activeTab === 'reviews'" class="reviews-tab">
        <div class="rating-summary">
          <div class="rating-score">
            <span class="score">{{ averageRating.toFixed(1) }}</span>
            <span class="max">/ 5.0</span>
          </div>
          <div class="stars">
            <span
              v-for="i in 5"
              :key="i"
              class="star"
              :class="{ 'filled': i <= Math.round(averageRating) }"
            >⭐</span>
          </div>
          <div class="rating-stats">
            {{ totalReviews }} 条评价
            <span class="rating-distribution">
              <span
                v-for="i in 5"
                :key="i"
                class="rating-bar"
              >
                {{ i }}⭐
                <div class="bar-container">
                  <div
                    class="bar-fill"
                    :style="{ width: `${(ratingDistribution[i] / totalReviews * 100) || 0}%` }"
                  ></div>
                </div>
                <span>{{ ratingDistribution[i] || 0 }}</span>
              </span>
            </span>
          </div>
        </div>

        <div class="write-review" v-if="isLoggedIn">
          <button @click="showReviewForm = true" class="btn primary">
            写评价
          </button>
        </div>

        <div v-if="showReviewForm" class="review-form">
          <h3>写评价</h3>
          <form @submit.prevent="submitReview">
            <div class="form-group">
              <label>评分</label>
              <div class="rating-input">
                <span
                  v-for="i in 5"
                  :key="i"
                  @click="reviewForm.rating = i"
                  class="star"
                  :class="{ 'filled': i <= reviewForm.rating }"
                >⭐</span>
              </div>
            </div>
            <div class="form-group">
              <label>标题（可选）</label>
              <input
                v-model="reviewForm.title"
                type="text"
                placeholder="评价标题"
              />
            </div>
            <div class="form-group">
              <label>评价内容</label>
              <textarea
                v-model="reviewForm.content"
                rows="4"
                placeholder="分享您的使用体验..."
              ></textarea>
            </div>
            <div class="form-actions">
              <button type="submit" class="btn primary">提交评价</button>
              <button
                type="button"
                @click="showReviewForm = false"
                class="btn secondary"
              >
                取消
              </button>
            </div>
          </form>
        </div>

        <div class="reviews-list">
          <div
            v-for="review in reviews"
            :key="review.id"
            class="review-item"
          >
            <div class="review-header">
              <div class="reviewer-info">
                <span class="reviewer-avatar">👤</span>
                <div class="reviewer-details">
                  <span class="reviewer-name">用户{{ review.userId }}</span>
                  <span class="review-date">{{ formatDate(review.createdAt) }}</span>
                </div>
              </div>
              <div class="review-rating">
                <span
                  v-for="i in 5"
                  :key="i"
                  class="star"
                  :class="{ 'filled': i <= review.rating }"
                >⭐</span>
              </div>
            </div>

            <div v-if="review.title" class="review-title">
              {{ review.title }}
            </div>

            <div v-if="review.content" class="review-content">
              {{ review.content }}
            </div>

            <div class="review-footer">
              <span
                v-if="review.isVerified"
                class="verified-badge"
              >✅ 已验证</span>
            </div>
          </div>

          <div v-if="reviews.length === 0" class="empty-state">
            暂无评价
          </div>
        </div>

        <!-- 分页 -->
        <div v-if="pagination.totalPages > 1" class="pagination">
          <button
            v-for="page in pagination.totalPages"
            :key="page"
            @click="loadReviews(page)"
            :class="{ 'active': page === pagination.page }"
            class="page-btn"
          >
            {{ page }}
          </button>
        </div>
      </div>

      <!-- 依赖信息 -->
      <div v-if="activeTab === 'dependencies'" class="dependencies-tab">
        <h3>模块依赖</h3>
        <div v-if="moduleDependencies.length === 0" class="empty-state">
          无依赖
        </div>
        <div v-else class="dependencies-list">
          <div
            v-for="dep in moduleDependencies"
            :key="dep"
            class="dependency-item"
          >
            <div class="dependency-info">
              <span class="dependency-name">{{ dep }}</span>
              <span class="dependency-type">核心依赖</span>
            </div>
            <button class="btn secondary">查看详情</button>
          </div>
        </div>

        <h3 class="compatibility-title">兼容性信息</h3>
        <div class="compatibility-info">
          <div class="compatibility-item">
            <span class="label">Nuxt.js</span>
            <span class="value">^3.0.0</span>
          </div>
          <div class="compatibility-item">
            <span class="label">Node.js</span>
            <span class="value">>=16.0.0</span>
          </div>
          <div class="compatibility-item">
            <span class="label">浏览器</span>
            <span class="value">Chrome 80+, Firefox 75+, Safari 13+, Edge 80+</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const moduleKey = route.params.moduleKey as string

// 状态管理
const module = ref(null)
const versions = ref([])
const reviews = ref([])
const currentVersion = ref('')
const isInstalled = ref(false)
const isFavorited = ref(false)
const isLoggedIn = ref(false)
const activeTab = ref('versions')
const showChangelog = ref(false)
const showReviewForm = ref(false)
const selectedVersion = ref(null)

const showOnlyStable = ref(false)
const showOnlyLatest = ref(false)
const pagination = ref({
  page: 1,
  pageSize: 10,
  total: 0,
  totalPages: 1
})

const reviewForm = ref({
  rating: 0,
  title: '',
  content: ''
})

// 标签页配置
const tabs = [
  { key: 'versions', label: '版本' },
  { key: 'reviews', label: '评价' },
  { key: 'dependencies', label: '依赖' }
]

// 计算属性
const filteredVersions = computed(() => {
  let filtered = versions.value

  if (showOnlyStable.value) {
    filtered = filtered.filter(v => v.isStable)
  }

  if (showOnlyLatest.value) {
    filtered = filtered.filter(v => v.isLatest)
  }

  return filtered
})

const moduleDependencies = computed(() => {
  return module.value?.dependencies || []
})

const totalDownloads = ref(0)
const averageRating = ref(0)
const totalReviews = ref(0)
const ratingDistribution = ref({ 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 })

// 生命周期钩子
onMounted(async () => {
  await loadModule()
  await loadVersions()
  await loadReviews()
})

// 数据加载
const loadModule = async () => {
  try {
    const res = await fetch(`/api/modules/${moduleKey}`)
    const data = await res.json()
    module.value = data.data
    currentVersion.value = module.value.moduleVersion
  } catch (error) {
    console.error('加载模块信息失败:', error)
  }
}

const loadVersions = async () => {
  try {
    const res = await fetch(`/api/modules/${moduleKey}/versions`)
    const data = await res.json()
    versions.value = data.data

    // 更新统计信息
    totalDownloads.value = versions.value.reduce((sum, v) => sum + (v.downloadCount || 0), 0)
  } catch (error) {
    console.error('加载版本列表失败:', error)
  }
}

const loadReviews = async (page = 1) => {
  try {
    const res = await fetch(`/api/modules/${moduleKey}/ratings?page=${page}&pageSize=${pagination.value.pageSize}`)
    const data = await res.json()
    reviews.value = data.data
    pagination.value = data.pagination

    // 计算评分统计
    if (reviews.value.length > 0) {
      const totalRating = reviews.value.reduce((sum, review) => sum + review.rating, 0)
      averageRating.value = totalRating / reviews.value.length
      totalReviews.value = reviews.value.length

      // 计算评分分布
      ratingDistribution.value = { 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 }
      reviews.value.forEach(review => {
        ratingDistribution.value[review.rating] = (ratingDistribution.value[review.rating] || 0) + 1
      })
    }
  } catch (error) {
    console.error('加载评价失败:', error)
  }
}

// 事件处理
const getCategoryName = (category) => {
  const names = {
    content: '内容',
    tool: '工具',
    ai: 'AI',
    experiment: '实验',
    interaction: '交互'
  }
  return names[category] || category
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString()
}

const formatFileSize = (bytes) => {
  if (!bytes) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const toggleFavorite = () => {
  isFavorited.value = !isFavorited.value
  // 这里可以添加保存收藏的逻辑
}

const installModule = () => {
  alert('开始安装模块...')
  // 这里可以添加安装逻辑
}

const downloadVersion = (version) => {
  window.location.href = `/api/modules/${moduleKey}/versions/${version.version}/download`
}

const viewChangelog = (version) => {
  selectedVersion.value = version
  showChangelog.value = true
}

const closeChangelog = () => {
  showChangelog.value = false
  selectedVersion.value = null
}

const submitReview = async () => {
  if (!reviewForm.value.rating) {
    alert('请选择评分')
    return
  }

  try {
    const res = await fetch(`/api/modules/${moduleKey}/ratings`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        version: currentVersion.value,
        rating: reviewForm.value.rating,
        title: reviewForm.value.title,
        content: reviewForm.value.content
      })
    })

    if (res.ok) {
      alert('评价提交成功')
      showReviewForm.value = false
      reviewForm.value = { rating: 0, title: '', content: '' }
      await loadReviews()
    }
  } catch (error) {
    console.error('提交评价失败:', error)
  }
}
</script>

<style scoped>
.module-marketplace {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
}

.page-header p {
  color: var(--color-text-tertiary);
  font-size: 1.1rem;
}

.module-info-card {
  background: var(--color-bg-light, white);
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  padding: 2rem;
  margin-bottom: 2rem;
}

.module-header {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.module-icon {
  font-size: 3rem;
  width: 80px;
  height: 80px;
  background: var(--color-bg-elevated);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.module-title h2 {
  margin: 0 0 0.5rem 0;
  font-size: 1.8rem;
}

.module-meta {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.badge {
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
}

.badge.content { background: var(--color-primary-light); color: var(--color-primary-hover); }
.badge.tool { background: var(--color-yellow-100); color: var(--color-yellow-800); }
.badge.ai { background: var(--color-indigo-100); color: var(--color-indigo-700); }
.badge.experiment { background: var(--color-pink-100); color: var(--color-pink-700); }
.badge.interaction { background: var(--color-success-bg); color: var(--color-success-dark); }

.version {
  color: var(--color-text-tertiary);
  font-size: 0.9rem;
}

.author {
  color: var(--color-text-tertiary);
  font-size: 0.9rem;
}

.module-actions {
  display: flex;
  gap: 1rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 1rem;
  transition: all 0.2s;
}

.btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

.btn.primary {
  background: var(--color-primary);
  color: var(--color-bg-light, white);
}

.btn.primary:hover {
  background: var(--color-primary-hover);
}

.btn.secondary {
  background: var(--color-bg-light, white);
  color: var(--color-text-secondary);
  border: 1px solid var(--color-border-default);
}

.btn.secondary:hover {
  background: var(--color-bg-elevated);
}

.btn.favorite {
  background: var(--color-bg-light, white);
  color: var(--color-text-tertiary);
  border: 1px solid var(--color-border-default);
  padding: 0.75rem;
}

.btn.favorited {
  color: var(--color-danger);
  border-color: var(--color-danger);
}

.module-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.stat-item {
  text-align: center;
}

.stat-value {
  display: block;
  font-size: 1.8rem;
  font-weight: 600;
  color: var(--color-primary);
}

.stat-label {
  color: var(--color-text-tertiary);
  font-size: 0.9rem;
}

.module-config h3 {
  margin: 0 0 1rem 0;
}

.config-list {
  display: grid;
  gap: 0.75rem;
}

.config-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: var(--color-bg-elevated);
  border-radius: 6px;
}

.config-name {
  font-weight: 500;
  font-family: monospace;
}

.config-type {
  background: var(--color-border);
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.85rem;
  color: var(--color-text-tertiary);
}

.config-desc {
  color: var(--color-text-tertiary);
  font-size: 0.9rem;
}

.tabs {
  display: flex;
  border-bottom: 2px solid var(--color-border);
  margin-bottom: 2rem;
}

.tab-btn {
  padding: 1rem 2rem;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1rem;
  color: var(--color-text-tertiary);
  position: relative;
  transition: color 0.2s;
}

.tab-btn:hover {
  color: var(--color-primary);
}

.tab-btn.active {
  color: var(--color-primary);
}

.tab-btn.active::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  right: 0;
  height: 2px;
  background: var(--color-primary);
}

.tab-content {
  background: var(--color-bg-light, white);
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  padding: 2rem;
}

.version-filters {
  display: flex;
  gap: 2rem;
  margin-bottom: 2rem;
}

.version-filters label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.versions-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.version-item {
  border: 1px solid var(--color-border);
  border-radius: 8px;
  padding: 1.5rem;
  transition: all 0.2s;
}

.version-item:hover {
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

.version-item.is-latest {
  border-color: var(--color-primary);
  background: var(--color-primary-lighter);
}

.version-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.version-info h3 {
  margin: 0 0 0.5rem 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.badge.latest {
  background: var(--color-primary-light);
  color: var(--color-primary-hover);
}

.badge.stable {
  background: var(--color-success-bg);
  color: var(--color-success-text);
}

.version-details {
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: var(--color-text-tertiary);
  font-size: 0.9rem;
}

.package-size,
.checksum {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.reviews-tab .rating-summary {
  text-align: center;
  margin-bottom: 2rem;
  padding: 2rem;
  background: var(--color-bg-elevated);
  border-radius: 8px;
}

.rating-score {
  font-size: 3rem;
  font-weight: 600;
  color: var(--color-primary);
}

.rating-score .max {
  font-size: 1.5rem;
  color: var(--color-text-tertiary);
}

.stars {
  margin: 1rem 0;
}

.review-form {
  background: var(--color-bg-elevated);
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
}

.rating-input {
  display: flex;
  gap: 0.5rem;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1rem;
}

.reviews-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.review-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.reviewer-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.reviewer-avatar {
  width: 40px;
  height: 40px;
  background: var(--color-border-subtle);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
}

.reviewer-details {
  display: flex;
  flex-direction: column;
}

.reviewer-date {
  font-size: 0.85rem;
  color: var(--color-text-tertiary);
}

.review-title {
  font-weight: 500;
  margin-bottom: 0.5rem;
}

.review-content {
  color: var(--color-text-tertiary);
  line-height: 1.6;
}

.dependencies-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 2rem;
}

.dependency-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: var(--color-bg-elevated);
  border-radius: 8px;
}

.dependency-type {
  color: var(--color-text-tertiary);
  font-size: 0.85rem;
}

.compatibility-title {
  margin-top: 2rem;
  margin-bottom: 1rem;
}

.compatibility-info {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
}

.compatibility-item {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem;
  background: var(--color-bg-elevated);
  border-radius: 6px;
}

.label {
  font-weight: 500;
}

.value {
  color: var(--color-text-tertiary);
}

.pagination {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
  margin-top: 2rem;
}

.page-btn {
  padding: 0.5rem 1rem;
  border: 1px solid var(--color-border-default);
  background: var(--color-bg-light, white);
  border-radius: 4px;
  cursor: pointer;
}

.page-btn.active {
  background: var(--color-primary);
  color: var(--color-bg-light, white);
  border-color: var(--color-primary);
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: var(--color-bg-light, white);
  border-radius: 8px;
  padding: 2rem;
  max-width: 600px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
}

.modal-body pre {
  white-space: pre-wrap;
  word-break: break-word;
  margin: 0;
  background: var(--color-bg-elevated);
  padding: 1rem;
  border-radius: 4px;
}

.empty-state {
  text-align: center;
  padding: 3rem;
  color: var(--color-text-tertiary);
}

@media (max-width: 768px) {
  .module-header {
    flex-direction: column;
    text-align: center;
  }

  .module-actions {
    justify-content: center;
  }

  .module-stats {
    grid-template-columns: repeat(2, 1fr);
  }

  .tabs {
    overflow-x: auto;
  }

  .tab-btn {
    padding: 1rem;
  }

  .tab-content {
    padding: 1rem;
  }
}
</style>