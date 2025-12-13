<template>
  <div class="asset-management-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <h1 class="page-title">资产管理</h1>
      <div class="asset-management-header-actions">
        <button @click="refreshAllData" class="asset-management-btn-secondary" :disabled="refreshing">
          {{ refreshing ? '刷新中...' : '🔄 刷新数据' }}
        </button>
      </div>
    </div>

    <!-- 标签页导航 -->
    <div class="asset-management-tabs-container">
      <div class="asset-management-tabs">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          :class="['asset-management-tab-button', { active: activeTab === tab.key }]"
          @click="activeTab = tab.key"
        >
          <i :class="tab.icon"></i>
          <span>{{ tab.label }}</span>
        </button>
      </div>
    </div>

    <!-- 标签页内容 -->
    <div class="asset-management-tab-content">
      <!-- 资产总览 -->
      <div v-show="activeTab === 'overview'" class="asset-management-tab-panel">
        <AdminAssetAssetOverview
          :overview-data="overviewData"
          :loading="overviewLoading"
          @refresh="loadOverview"
        />
      </div>

      <!-- 投资仪表盘 -->
      <div v-show="activeTab === 'investment'" class="asset-management-tab-panel">
        <iframe
          src="/admin/investment?embedded=true"
          class="asset-management-embedded-iframe"
          frameborder="0"
          @load="handleIframeLoad"
        />
      </div>

      <!-- 定投计划 -->
      <div v-show="activeTab === 'dca-plan'" class="asset-management-tab-panel">
        <iframe
          src="/admin/dca-plan?embedded=true"
          class="asset-management-embedded-iframe"
          frameborder="0"
        />
      </div>

      <!-- 价格提醒 -->
      <div v-show="activeTab === 'price-alert'" class="asset-management-tab-panel">
        <iframe
          src="/admin/price-alert?embedded=true"
          class="asset-management-embedded-iframe"
          frameborder="0"
        />
      </div>

      <!-- 数据导入/导出 -->
      <div v-show="activeTab === 'import-export'" class="asset-management-tab-panel">
        <AdminAssetDataImportExport />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useApi } from '~/composables/useApi'
import { useNotification } from '~/composables/useToast'
import AdminAssetAssetOverview from '~/components/admin/asset/AssetOverview.vue'
import AdminAssetDataImportExport from '~/components/admin/asset/DataImportExport.vue'

// 页面元数据
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { success, error } = useNotification()

const activeTab = ref('overview')
const refreshing = ref(false)
const overviewLoading = ref(false)
const overviewData = ref<any>(null)

const tabs = [
  { key: 'overview', label: '资产总览', icon: 'fas fa-chart-pie' },
  { key: 'investment', label: '投资仪表盘', icon: 'fas fa-chart-line' },
  { key: 'dca-plan', label: '定投计划', icon: 'fas fa-calendar-check' },
  { key: 'price-alert', label: '价格提醒', icon: 'fas fa-bell' },
  { key: 'import-export', label: '数据导入/导出', icon: 'fas fa-file-import' }
]

// 加载资产总览
const loadOverview = async () => {
  try {
    overviewLoading.value = true
    const data = await api.get('/Asset/overview')
    overviewData.value = data
  } catch (err: any) {
    console.error('加载资产总览失败:', err)
    error(err.message || '加载失败')
  } finally {
    overviewLoading.value = false
  }
}

// 刷新所有数据
const refreshAllData = async () => {
  try {
    refreshing.value = true
    // 刷新投资价格
    await api.post('/Investment/refresh-prices')
    // 刷新价格提醒
    await api.post('/PriceAlert/refresh-prices')
    // 重新加载总览
    await loadOverview()
    success('数据刷新成功')
  } catch (err: any) {
    console.error('刷新失败:', err)
    error(err.message || '刷新失败')
  } finally {
    refreshing.value = false
  }
}

// 处理 iframe 加载
const handleIframeLoad = () => {
  // iframe 加载完成后的处理
}

// 页面加载时获取数据
onMounted(() => {
  loadOverview()
})
</script>

<style scoped>
/* 使用统一样式类，样式定义在 assets/css/admin-asset-management.css */

/* 嵌入的 iframe 样式 */
.asset-management-embedded-iframe {
  width: 100%;
  height: calc(100vh - 250px);
  min-height: 600px;
  border: none;
  background: var(--color-bg-body);
}
</style>
