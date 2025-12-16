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
  { key: 'dca-plan', label: '定投计划', icon: 'fas fa-calendar-check' },
  { key: 'price-alert', label: '价格提醒', icon: 'fas fa-bell' },
  { key: 'import-export', label: '数据导入/导出', icon: 'fas fa-file-import' }
]

// 加载资产总览
const loadOverview = async () => {
  // 防止重复请求
  if (overviewLoading.value) {
    return
  }
  
  try {
    overviewLoading.value = true
    const response = await api.get('/Asset/overview')
    
    // useApi 已经提取了 data 字段，所以 response 就是数据对象
    // 但需要检查是否有嵌套的 data
    let data = response
    
    // 如果 response 有 data 字段，说明可能被嵌套了
    if (response && typeof response === 'object' && 'data' in response && !('TotalAssets' in response) && !('totalAssets' in response)) {
      data = response.data
    }
    
    // 确保数据是对象
    if (!data || typeof data !== 'object') {
      console.error('[资产总览] 数据格式错误:', data)
      error('数据格式错误')
      return
    }
    
    // 后端返回的是小写驼峰格式，需要兼容两种格式
    const totalAssets = data.TotalAssets ?? data.totalAssets ?? 0
    const totalInvestments = data.TotalInvestments ?? data.totalInvestments ?? 0
    const totalNetWorth = data.TotalNetWorth ?? data.totalNetWorth ?? 0
    const assetsByType = data.AssetsByType ?? data.assetsByType ?? []
    const investmentStats = data.InvestmentStats ?? data.investmentStats ?? {}
    const assetDistribution = data.AssetDistribution ?? data.assetDistribution ?? {}
    
    // 确保数值类型正确，统一使用大写开头的字段名（组件期望的格式）
    // 同时处理 InvestmentStats 内部的字段名
    const normalizedInvestmentStats = investmentStats && typeof investmentStats === 'object' ? {
      TotalCost: Number(investmentStats.TotalCost ?? investmentStats.totalCost ?? 0),
      TotalMarketValue: Number(investmentStats.TotalMarketValue ?? investmentStats.totalMarketValue ?? 0),
      TotalProfitLoss: Number(investmentStats.TotalProfitLoss ?? investmentStats.totalProfitLoss ?? 0),
      TotalProfitRate: Number(investmentStats.TotalProfitRate ?? investmentStats.totalProfitRate ?? 0),
      Count: Number(investmentStats.Count ?? investmentStats.count ?? 0)
    } : {}
    
    overviewData.value = {
      TotalAssets: Number(totalAssets),
      TotalInvestments: Number(totalInvestments),
      TotalNetWorth: Number(totalNetWorth),
      AssetsByType: assetsByType,
      InvestmentStats: normalizedInvestmentStats,
      AssetDistribution: assetDistribution
    }
  } catch (err: any) {
    console.error('加载资产总览失败:', err)
    error(err.message || '加载失败')
    overviewData.value = null
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
    // 等待后端数据保存完成
    await new Promise(resolve => setTimeout(resolve, 800))
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
