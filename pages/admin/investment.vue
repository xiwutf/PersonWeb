<template>
  <div class="asset-decision-panel">
    <!-- ???? -->
    <div class="panel-header">
      <div class="header-title-section">
        <h1 class="panel-title">???????</h1>
        <p class="panel-subtitle">????????????????????</p>
      </div>
      <div class="header-actions">
        <div class="auto-refresh-control">
          <label class="auto-refresh-switch">
            <input 
              type="checkbox" 
              v-model="autoRefreshEnabled"
              class="auto-refresh-checkbox"
            />
            <span class="auto-refresh-label">????</span>
          </label>
          <span v-if="lastRefreshTime" class="last-refresh-time">
            ????: {{ formatRefreshTime(lastRefreshTime) }}
          </span>
        </div>
        <button @click="refreshPrices" class="btn-action-secondary" :disabled="refreshingPrices">
          <span v-if="refreshingPrices">????..</span>
          <span v-else>?? ????</span>
        </button>
        <div class="export-menu-container">
          <button @click="handleExportClick" class="btn-action-secondary">?? ????</button>
          <div v-if="showExportMenu" class="export-menu">
            <button @click="exportData('investments')" class="export-menu-item">??????</button>
            <button @click="exportData('transactions')" class="export-menu-item">??????</button>
            <button @click="exportData('stats')" class="export-menu-item">??????</button>
          </div>
        </div>
        <button @click="handleAddClick" class="btn-action-primary">+ ????</button>
      </div>
    </div>

    <!-- ???? -->
    <div class="module-description">
      <div class="description-header" @click="toggleDescription">
        <span class="description-icon">??</span>
        <span class="description-title">????</span>
        <span class="description-toggle">{{ showDescription ? '??' : '??' }}</span>
      </div>
      <div v-show="showDescription" class="description-content">
        <p class="description-text">
          ??????????????????????????
          ?????????????????????????        </p>
        <p class="description-warning">
          ?? ?????????????????????????????????        </p>
      </div>
    </div>

    <!-- ?????? -->
    <div class="feature-card">
      <h3 class="feature-card-title">????</h3>
      <div class="feature-list">
        <div class="feature-item" :title="'?? / ETF / ???????'">
          <div class="feature-icon">??</div>
          <div class="feature-content">
            <div class="feature-name">???????</div>
          </div>
        </div>
        <div class="feature-item" :title="'?????????????????'">
          <div class="feature-icon">??</div>
          <div class="feature-content">
            <div class="feature-name">??????</div>
          </div>
        </div>
        <div class="feature-item" :title="'????????????????'">
          <div class="feature-icon">??</div>
          <div class="feature-content">
            <div class="feature-name">??????</div>
          </div>
        </div>
        <div class="feature-item" :title="'??? Excel/CSV ??????'">
          <div class="feature-icon">??</div>
          <div class="feature-content">
            <div class="feature-name">??????</div>
          </div>
        </div>
      </div>
    </div>

    <!-- ???????? -->
    <div class="import-section">
      <div class="import-card">
        <div class="import-header">
          <h3 class="import-title">??????</h3>
          <span class="import-info-icon" :title="'?? Excel/CSV ????????????????????'">??</span>
        </div>
        <div class="import-content">
          <div class="import-formats">
            <span class="format-tag">Excel</span>
            <span class="format-tag">CSV</span>
          </div>
          <button @click="handleImportClick" class="btn-import">
            <span class="import-icon">??</span>
            ??????
          </button>
          <input
            ref="fileInput"
            type="file"
            accept=".csv,.xlsx,.xls"
            style="display: none"
            @change="handleFileSelect"
          />
        </div>
      </div>
    </div>

    <!-- ???? -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-label">???</div>
        <div class="stat-value">?{{ formatMoney(stats.TotalCost || 0) }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">???</div>
        <div class="stat-value-blue">?{{ formatMoney(stats.TotalMarketValue || 0) }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">???</div>
        <div :class="(stats.TotalProfitLoss || 0) >= 0 ? 'stat-value-positive' : 'stat-value-negative'">
          ?{{ formatMoney(stats.TotalProfitLoss || 0) }}
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-label">???</div>
        <div :class="(stats.TotalProfitRate || 0) >= 0 ? 'stat-value-positive' : 'stat-value-negative'">
          {{ formatPercent(stats.TotalProfitRate || 0) }}%
        </div>
      </div>
    </div>

    <!-- ?????? -->
    <div class="charts-grid">
      <!-- ??????????-->
      <div class="chart-container">
        <h2 class="chart-title">??????</h2>
        <div v-if="stats.ByType && stats.ByType.length > 0" class="chart-wrapper">
          <v-chart :option="typeChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">????</div>
      </div>

      <!-- ????????? -->
      <div class="chart-container">
        <h2 class="chart-title">????</h2>
        <div v-if="stats.ByProfitStatus && stats.ByProfitStatus.length > 0" class="chart-wrapper">
          <v-chart :option="profitStatusChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">????</div>
      </div>

      <!-- ???????????? -->
      <div class="chart-container">
        <h2 class="chart-title">???? Top 10</h2>
        <div v-if="assetDistributionChartOption.series && assetDistributionChartOption.series[0].data.length > 0" class="chart-wrapper">
          <v-chart :option="assetDistributionChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">????</div>
      </div>

      <!-- ????????-->
      <div class="chart-container">
        <h2 class="chart-title">?? Top 5</h2>
        <div v-if="stats.TopByProfit && stats.TopByProfit.length > 0" class="chart-wrapper">
          <v-chart :option="profitRankChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">????</div>
      </div>
    </div>

    <!-- ???? -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- ????????-->
      <div class="stats-table-container">
        <h2 class="chart-title">????</h2>
        <div class="overflow-x-auto">
          <table class="stats-table">
            <thead>
              <tr>
                <th>??</th>
                <th>??</th>
                <th>???</th>
                <th>???</th>
                <th>??</th>
                <th>???</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in stats.ByType" :key="index">
                <td>{{ item.TypeName || item.Type }}</td>
                <td>{{ item.Count }}</td>
                <td>?{{ formatMoney(item.TotalCost) }}</td>
                <td>?{{ formatMoney(item.TotalMarketValue) }}</td>
                <td :class="item.ProfitLoss >= 0 ? 'profit-positive' : 'profit-negative'">
                  ?{{ formatMoney(item.ProfitLoss) }}
                </td>
                <td :class="item.ProfitRate >= 0 ? 'profit-positive' : 'profit-negative'">
                  {{ formatPercent(item.ProfitRate) }}%
                </td>
              </tr>
              <tr v-if="!stats.ByType || stats.ByType.length === 0">
                <td colspan="6" class="stats-table-empty">????</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Top 5 ?? -->
      <div class="top-holdings-container">
        <h2 class="top-holdings-title">Top 5 ???????</h2>
        <div class="top-holdings-list">
          <div
            v-for="(item, index) in stats.TopByMarketValue"
            :key="index"
            class="holding-item"
          >
            <div class="holding-info">
              <div
                class="holding-rank"
                :class="{
                  'holding-rank-1': index === 0,
                  'holding-rank-2': index === 1,
                  'holding-rank-3': index === 2,
                  'holding-rank-other': index > 2
                }"
              >
                {{ index + 1 }}
              </div>
              <div class="holding-details">
                <div class="holding-name">
                  {{ item.Name }} ({{ item.Code }})
                </div>
                <div class="holding-type">
                  {{ item.Type === 'stock' ? '??' : '??' }}
                </div>
              </div>
            </div>
            <div class="holding-value">
              <div class="holding-value-amount">
                ?{{ formatMoney(item.MarketValue) }}
              </div>
              <div 
                class="holding-value-rate"
                :class="item.ProfitLoss >= 0 ? 'profit-positive' : 'profit-negative'"
              >
                {{ item.ProfitLoss >= 0 ? '+' : '' }}{{ formatPercent(item.ProfitRate) }}%
              </div>
            </div>
          </div>
          <div v-if="!stats.TopByMarketValue || stats.TopByMarketValue.length === 0" class="empty-state">
            ????
          </div>
        </div>
      </div>
    </div>

    <!-- ???? -->
    <div class="table-container">
      <table class="investment-table">
        <thead>
          <tr>
            <th>??</th>
            <th>??</th>
            <th>??</th>
            <th>??</th>
            <th>???</th>
            <th>????
              <span class="table-header-hint" title="???????.00????????????'??">??</span>
            </th>
            <th>??</th>
            <th>??</th>
            <th>??</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in investments" :key="item.id">
            <td class="font-mono">{{ item.code }}</td>
            <td>{{ item.name }}</td>
            <td>
              <span :class="item.type === 'stock' ? 'badge-stock' : 'badge-fund'">
                {{ item.type === 'stock' ? '??' : '??' }}
              </span>
            </td>
            <td>{{ item.quantity }}</td>
            <td>?{{ formatMoney(item.costPrice) }}</td>
            <td>
              <span v-if="item.currentPrice > 0">?{{ formatMoney(item.currentPrice) }}</span>
              <span v-else class="price-zero-hint" title="??????? 0.00???????"
                ?0.00
                <span class="hint-icon">??</span>
              </span>
            </td>
            <td>
              <span v-if="item.marketValue > 0">?{{ formatMoney(item.marketValue) }}</span>
              <span v-else class="price-zero-hint">?0.00</span>
            </td>
            <td>
              <div :class="item.profitLoss >= 0 ? 'profit-positive' : 'profit-negative'">
                ?{{ formatMoney(item.profitLoss) }}
              </div>
              <div :class="item.profitRate >= 0 ? 'profit-rate-positive' : 'profit-rate-negative'">
                {{ formatPercent(item.profitRate) }}%
              </div>
            </td>
            <td>
              <div class="action-buttons">
                <button @click="editItem(item)" class="btn-edit">??</button>
                <button @click="addTransaction(item)" class="btn-transaction">??</button>
                <button @click="deleteItem(item.id)" class="btn-delete">??</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- ??/????? -->
    <div v-if="showCreateModal || editingItem" class="modal-overlay">
      <div class="modal-content">
        <div class="modal-body">
          <h2 class="modal-title">{{ editingItem ? '??' : '??' }}??</h2>
          
          <div class="modal-form">
            <div class="form-group">
              <label class="form-label">?? <span class="text-red-500">*</span></label>
              <div class="form-input-group">
                <input 
                  v-model="form.code" 
                  type="text" 
                  class="form-input" 
                  placeholder="??: 000001 (??) ??005918 (??)" 
                  @blur="autoDetectType"
                />
                <button 
                  @click="autoFillFromCode" 
                  :disabled="!form.code || !form.type || isAutoFilling"
                  class="btn-auto-fill"
                  type="button"
                >
                  <span v-if="isAutoFilling" class="spinner"></span>
                  <span v-else>?? ????</span>
                </button>
              </div>
              <div class="form-hint">
                ??6????????"????"????????????
                <br />
                <span class="form-hint-tip">?? <strong>??????005918???????</strong>?????????????????????????????????</span>
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">?? <span class="text-red-500">*</span></label>
              <input v-model="form.name" type="text" class="form-input" placeholder="??: ????300ETF??C?????? />
              <div class="form-hint">
                ????????                <br />
                <span class="form-hint-tip">?? <strong>??????005918?</strong>????????????????????????300ETF??C"</span>
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">?? <span class="text-red-500">*</span></label>
              <select v-model="form.type" class="form-select">
                <option value="stock">??</option>
                <option value="fund">??</option>
              </select>
              <div class="form-hint">??????????????</div>
            </div>
            <!-- ?????? -->
            <div class="form-group">
              <div class="input-mode-switch">
                <label class="switch-label">?????</label>
                <div class="switch-buttons">
                  <button 
                    :class="['switch-btn', { active: inputMode === 'quick' }]"
                    @click="inputMode = 'quick'"
                    type="button"
                  >
                    ?? ?????                  </button>
                  <button 
                    :class="['switch-btn', { active: inputMode === 'detail' }]"
                    @click="inputMode = 'detail'"
                    type="button"
                  >
                    ?? ????
                  </button>
                </div>
              </div>
            </div>

            <!-- ???????-->
            <template v-if="inputMode === 'quick'">
              <div class="form-group">
                <label class="form-label">??????<span class="text-red-500">*</span></label>
                <div class="input-with-unit">
                  <input 
                    v-model.number="quickInput.totalAmount" 
                    type="number" 
                    step="0.01" 
                    min="0.01"
                    class="form-input" 
                    placeholder="??: 1000" 
                    @input="calculateQuickEstimate"
                  />
                  <span class="input-unit">?</span>
                </div>
              <div class="form-hint">
                ?????????????1000??????                  <br />
                  <span class="form-hint-tip">?? <strong>????????</strong> ?????????????????????????????????????????</span>
                </div>
              </div>
              <div class="form-group">
                <label class="form-label">?????? <span class="text-red-500">*</span></label>
                <div class="input-with-unit">
                  <input 
                    v-model.number="quickInput.estimatedPrice" 
                    type="number" 
                    step="0.01" 
                    min="0.01"
                    class="form-input" 
                    placeholder="??: 1.4" 
                    @input="calculateQuickEstimate"
                  />
                  <span class="input-unit">???</span>
                </div>
                <div class="form-hint">
                  ????????????????????????                  <br />
                  <span class="form-hint-tip">?? <strong>????????005918?</strong>??????????????????????.4??????????????????????</span>
                </div>
              </div>
              <div v-if="quickEstimate.quantity > 0" class="form-group">
                <div class="estimate-result">
                  <div class="estimate-item">
                    <span class="estimate-label">?????????</span>
                    <span class="estimate-value">{{ formatMoney(quickEstimate.quantity) }} ?</span>
                  </div>
                  <div class="estimate-item">
                    <span class="estimate-label">????</span>
                    <span class="estimate-value">?{{ formatMoney(quickEstimate.costPrice) }}</span>
                  </div>
                </div>
              </div>
            </template>

            <!-- ?????? -->
            <template v-else>
              <div class="form-group">
                <label class="form-label">???? <span class="text-red-500">*</span></label>
                <input 
                  v-model.number="form.quantity" 
                  type="number" 
                  step="0.01" 
                  min="0.01"
                  class="form-input" 
                  placeholder="??: 1000" 
                  required
                />
                <div class="form-hint">
                  ??????????????????0
                  <br />
                  <span class="form-hint-tip">?? ?????????????????</span>
                </div>
              </div>
              <div class="form-group">
                <label class="form-label">????<span class="text-red-500">*</span></label>
                <input 
                  v-model.number="form.costPrice" 
                  type="number" 
                  step="0.01" 
                  min="0.01"
                  class="form-input" 
                  placeholder="??: 1.4078" 
                  required
                />
                <div class="form-hint">
                  ????????????????????????
                  <br />
                  <span class="form-hint-tip">?? ???????????????????????????</span>
                </div>
              </div>
            </template>
            <div class="form-group">
              <label class="form-label">???? <span class="text-red-500">*</span></label>
              <div class="input-with-unit">
                <input 
                  v-model.number="form.currentPrice" 
                  type="number" 
                  step="0.0001" 
                  min="0"
                  class="form-input" 
                  placeholder="??: 1.4012" 
                  required
                />
                <span class="input-unit">???</span>
              </div>
              <div class="form-hint">
                ????????/????????
                <br />
                <span class="form-hint-tip">?? <strong>????????</strong>???????????????????????????995.34??? ??710.3282????1.4012???</span>
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">??</label>
              <textarea v-model="form.notes" rows="3" class="form-textarea" placeholder="?????????"></textarea>
            </div>
          </div>

          <div class="modal-actions">
            <button @click="saveItem" class="btn-save">??</button>
            <button @click="cancelEdit" class="btn-cancel">??</button>
          </div>
        </div>
      </div>
    </div>

    <!-- ????? -->
    <div v-if="showImportModal" class="modal-overlay" @click="closeImportModal">
      <div class="modal-content" @click.stop>
        <div class="modal-body">
          <h2 class="modal-title">??????</h2>
          
          <div v-if="importing" class="import-status">
            <div class="spinner"></div>
            <p>?????????..</p>
          </div>

          <div v-else-if="importResult" class="import-result">
            <div class="result-summary">
              <div class="result-item success">
                <span class="result-label">???</span>
                <span class="result-value">{{ importResult.successCount }} ?</span>
              </div>
              <div class="result-item error">
                <span class="result-label">???</span>
                <span class="result-value">{{ importResult.failCount }} ?</span>
              </div>
            </div>
            
            <div v-if="importResult.errors.length > 0" class="result-errors">
              <h4>?????</h4>
              <ul>
                <li v-for="(error, index) in importResult.errors" :key="index">{{ error }}</li>
              </ul>
            </div>
          </div>

          <div v-else class="import-info">
            <p class="import-hint">
              <strong>???????</strong>
            </p>
            <ul class="import-format-list">
              <li>?? CSV ??Excel ????csv, .xlsx, .xls?</li>
              <li>CSV ??????????,??,????,??,??,????,??</li>
              <li>????????????????</li>
              <li>???stock??????fund????</li>
              <li>?????buy??????sell????</li>
            </ul>
            <div class="import-example">
              <p><strong>???</strong></p>
              <pre>??,??,??,????,??,??,????,??
005918,????300ETF??C,fund,buy,1000,1.4078,2024-01-01,??????000001,????,stock,buy,100,12.50,2024-01-02,</pre>
            </div>
          </div>

          <div class="modal-actions">
            <button @click="closeImportModal" class="btn-cancel">??</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { PieChart, BarChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components'
import VChart from 'vue-echarts'
import { useEChartsTheme } from '~/composables/useEChartsTheme'
import { registerTheme } from 'echarts/core'

// ?? ECharts ??
use([
  CanvasRenderer,
  PieChart,
  BarChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
])

// ?? ECharts ????
const { isDark, darkTheme, lightTheme } = useEChartsTheme()

// ??????????registerTheme('dark-custom', {
  backgroundColor: 'transparent',
  textStyle: {
    color: 'var(--color-bg-card)'
  },
  title: {
    textStyle: {
      color: 'var(--color-bg-card)'
    }
  },
  legend: {
    textStyle: {
      color: 'var(--color-border)'
    }
  },
  tooltip: {
    backgroundColor: 'rgba(17, 24, 39, 0.98)',
    borderColor: 'rgba(156, 163, 175, 0.5)',
    textStyle: {
      color: 'var(--color-bg-card)'
    }
  }
})

// ??????????registerTheme('light-custom', {
  backgroundColor: 'transparent',
  textStyle: {
    color: 'var(--color-text-main)'
  },
  title: {
    textStyle: {
      color: 'var(--color-text-main)'
    }
  },
  legend: {
    textStyle: {
      color: 'var(--color-text-sec)'
    }
  },
  tooltip: {
    backgroundColor: 'rgba(255, 255, 255, 0.95)',
    borderColor: 'rgba(209, 213, 219, 0.8)',
    textStyle: {
      color: 'var(--color-text-main)'
    }
  }
})

import type { Investment, InvestmentRequest } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const investments = ref<Investment[]>([])
const stats = ref<{
  TotalCost?: number
  TotalMarketValue?: number
  TotalProfitLoss?: number
  TotalProfitRate?: number
  TotalCount?: number
  ByType?: Array<{
    Type: string
    TypeName?: string
    Count: number
    TotalCost: number
    TotalMarketValue: number
    ProfitLoss: number
    ProfitRate?: number
  }>
  ByProfitStatus?: Array<{
    Status: string
    StatusName?: string
    Count: number
    TotalCost: number
    TotalMarketValue: number
    ProfitLoss: number
  }>
  TopByMarketValue?: Array<{
    Code: string
    Name: string
    Type: string
    MarketValue: number
    ProfitLoss: number
    ProfitRate: number
  }>
  TopByProfit?: Array<{
    Code: string
    Name: string
    Type: string
    ProfitLoss: number
    ProfitRate: number
  }>
  TopByLoss?: Array<{
    Code: string
    Name: string
    Type: string
    ProfitLoss: number
    ProfitRate: number
  }>
  AssetDistribution?: Array<{
    Code: string
    Name: string
    MarketValue: number
    Percentage: number
  }>
}>({})
const showCreateModal = ref(false)
const editingItem = ref<Investment | null>(null)
const isAutoFilling = ref(false)
const inputMode = ref<'quick' | 'detail'>('quick') // ???????????const showDescription = ref(false) // ????????
const form = ref({
  code: '',
  name: '',
  type: 'stock',
  quantity: 0,
  costPrice: 0,
  currentPrice: 0,
  notes: ''
})

// ???????const quickInput = ref({
  totalAmount: 0,      // ??????  estimatedPrice: 0    // ??????
})

// ???????const quickEstimate = ref({
  quantity: 0,         // ????????
  costPrice: 0         // ??????estimatedPrice??})

// ???????const calculateQuickEstimate = () => {
  if (quickInput.value.totalAmount > 0 && quickInput.value.estimatedPrice > 0) {
    quickEstimate.value.quantity = quickInput.value.totalAmount / quickInput.value.estimatedPrice
    quickEstimate.value.costPrice = quickInput.value.estimatedPrice
    
    // ??????    form.value.quantity = quickEstimate.value.quantity
    form.value.costPrice = quickEstimate.value.costPrice
  } else {
    quickEstimate.value.quantity = 0
    quickEstimate.value.costPrice = 0
  }
}

// ???????????????????watch(() => form.value.currentPrice, (newPrice) => {
  if (newPrice > 0 && inputMode.value === 'quick' && quickInput.value.estimatedPrice === 0) {
    quickInput.value.estimatedPrice = newPrice
    calculateQuickEstimate()
  }
})

// ?????????PascalCase -> camelCase??const transformInvestment = (item: any): Investment => {
  return {
    id: item.id || item.Id || 0,
    code: item.code || item.Code || '',
    name: item.name || item.Name || '',
    type: item.type || item.Type || 'stock',
    quantity: item.quantity || item.Quantity || 0,
    costPrice: item.costPrice || item.CostPrice || 0,
    currentPrice: item.currentPrice || item.CurrentPrice || 0,
    totalCost: item.totalCost || item.TotalCost || 0,
    marketValue: item.marketValue || item.MarketValue || 0,
    profitLoss: item.profitLoss || item.ProfitLoss || 0,
    profitRate: item.profitRate || item.ProfitRate || 0,
    notes: item.notes || item.Notes || '',
    userId: item.userId || item.UserId
  }
}

const loading = ref(false)
const fetchList = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/Investment')
    
    // ????????
    let investmentList: any[] = []
    if (Array.isArray(res)) {
      investmentList = res
    } else if (res && Array.isArray(res.List)) {
      investmentList = res.List
    } else if (res && Array.isArray(res.data)) {
      investmentList = res.data
    }
    
    // ??????
    investments.value = investmentList.map(transformInvestment)

    // ??????
    const statsRes = await api.get<any>('/Investment/stats')
    if (statsRes) {
      // ?????????????? PascalCase??????camelCase??      stats.value = {
        TotalCost: statsRes.TotalCost ?? statsRes.totalCost ?? 0,
        TotalMarketValue: statsRes.TotalMarketValue ?? statsRes.totalMarketValue ?? 0,
        TotalProfitLoss: statsRes.TotalProfitLoss ?? statsRes.totalProfitLoss ?? 0,
        TotalProfitRate: statsRes.TotalProfitRate ?? statsRes.totalProfitRate ?? 0,
        TotalCount: statsRes.TotalCount ?? statsRes.totalCount ?? 0,
        ByType: (statsRes.ByType ?? statsRes.byType ?? []).map((item: any) => ({
          Type: item.Type ?? item.type ?? '',
          TypeName: item.TypeName ?? item.typeName ?? (item.Type === 'stock' || item.type === 'stock' ? '??' : '??'),
          Count: item.Count ?? item.count ?? 0,
          TotalCost: item.TotalCost ?? item.totalCost ?? 0,
          TotalMarketValue: item.TotalMarketValue ?? item.totalMarketValue ?? 0,
          ProfitLoss: item.ProfitLoss ?? item.profitLoss ?? 0,
          ProfitRate: item.ProfitRate ?? item.profitRate ?? 0
        })),
        ByProfitStatus: (statsRes.ByProfitStatus ?? statsRes.byProfitStatus ?? []).map((item: any) => ({
          Status: item.Status ?? item.status ?? '',
          StatusName: item.StatusName ?? item.statusName ?? (item.Status === 'profit' || item.status === 'profit' ? '??' : '??'),
          Count: item.Count ?? item.count ?? 0,
          TotalCost: item.TotalCost ?? item.totalCost ?? 0,
          TotalMarketValue: item.TotalMarketValue ?? item.totalMarketValue ?? 0,
          ProfitLoss: item.ProfitLoss ?? item.profitLoss ?? 0
        })),
        TopByMarketValue: (statsRes.TopByMarketValue ?? statsRes.topByMarketValue ?? []).map((item: any) => ({
          Code: item.Code ?? item.code ?? '',
          Name: item.Name ?? item.name ?? '',
          Type: item.Type ?? item.type ?? '',
          MarketValue: item.MarketValue ?? item.marketValue ?? 0,
          ProfitLoss: item.ProfitLoss ?? item.profitLoss ?? 0,
          ProfitRate: item.ProfitRate ?? item.profitRate ?? 0
        })),
        TopByProfit: (statsRes.TopByProfit ?? statsRes.topByProfit ?? []).map((item: any) => ({
          Code: item.Code ?? item.code ?? '',
          Name: item.Name ?? item.name ?? '',
          Type: item.Type ?? item.type ?? '',
          ProfitLoss: item.ProfitLoss ?? item.profitLoss ?? 0,
          ProfitRate: item.ProfitRate ?? item.profitRate ?? 0
        })),
        TopByLoss: (statsRes.TopByLoss ?? statsRes.topByLoss ?? []).map((item: any) => ({
          Code: item.Code ?? item.code ?? '',
          Name: item.Name ?? item.name ?? '',
          Type: item.Type ?? item.type ?? '',
          ProfitLoss: item.ProfitLoss ?? item.profitLoss ?? 0,
          ProfitRate: item.ProfitRate ?? item.profitRate ?? 0
        })),
        AssetDistribution: (statsRes.AssetDistribution ?? statsRes.assetDistribution ?? []).map((item: any) => ({
          Code: item.Code ?? item.code ?? '',
          Name: item.Name ?? item.name ?? '',
          MarketValue: item.MarketValue ?? item.marketValue ?? 0,
          Percentage: item.Percentage ?? item.percentage ?? 0
        }))
      }
    }
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch investments:', e)
    }
  } finally {
    loading.value = false
  }
}

const refreshingPrices = ref(false)
const refreshPrices = async () => {
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  refreshingPrices.value = true
  try {
    // useApi ??????????????????data???? null????????????    await api.post('/Investment/refresh-prices')
    // ?????????????????    await new Promise(resolve => setTimeout(resolve, 500))
    // ????????????    await fetchList()
    lastRefreshTime.value = new Date()
    // ??????????????????????????
    if (!autoRefreshInterval.value || !autoRefreshEnabled.value) {
      success('????????????')
    } else {
      console.log('[????] ??????')
    }
  } catch (e: unknown) {
    handleError(e, '????')
  } finally {
    refreshingPrices.value = false
  }
}

const editItem = (item: Investment) => {
  editingItem.value = item
  form.value = {
    code: item.code,
    name: item.name,
    type: item.type,
    quantity: item.quantity,
    costPrice: item.costPrice,
    currentPrice: item.currentPrice,
    notes: item.notes || ''
  }
}

const addTransaction = (item: Investment) => {
  // TODO: ????????
  const { info } = useNotification()
  info('???????...')
}

const saveItem = async () => {
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  // ??????
  if (!form.value.code || !form.value.code.trim()) {
    warning('??????)
    return
  }
  
  if (!form.value.name || !form.value.name.trim()) {
    warning('??????)
    return
  }
  
  if (!form.value.type) {
    warning('?????')
    return
  }
  
  if (!form.value.quantity || form.value.quantity <= 0) {
    warning('????????????0')
    return
  }
  
  if (!form.value.costPrice || form.value.costPrice <= 0) {
    warning('????????????')
    return
  }
  
  if (!form.value.currentPrice || form.value.currentPrice <= 0) {
    warning('????????????0')
    return
  }
  
  try {
    const payload: InvestmentRequest = {
      code: form.value.code.trim(),
      name: form.value.name.trim(),
      type: form.value.type,
      quantity: form.value.quantity,
      costPrice: form.value.costPrice,
      currentPrice: form.value.currentPrice, // ??????
      notes: form.value.notes || undefined
    }
    
    let response
    if (editingItem.value) {
      response = await api.put(`/Investment/${editingItem.value.id}`, payload)
    } else {
      response = await api.post('/Investment', payload)
    }

    // ?????????????????????????
    const message = response?.message || '????'
    success(message)
    cancelEdit()
    fetchList()
  } catch (e: unknown) {
    handleError(e, '????')
  }
}

const deleteItem = async (id: number) => {
  if (!confirm('????????)) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    // ?? ID ??????    const investmentId = Number(id)
    if (isNaN(investmentId) || investmentId <= 0) {
      throw new Error('????????ID')
    }
    
    const response = await api.delete(`/Investment/${investmentId}`)
    success('????')
    fetchList()
  } catch (e: unknown) {
    console.error('????:', e)
    handleError(e, '????')
  }
}

const handleAddClick = () => {
  showCreateModal.value = true
  editingItem.value = null
  form.value = { code: '', name: '', type: 'stock', quantity: 0, costPrice: 0, currentPrice: 0, notes: '' }
  isAutoFilling.value = false
  // ?????????  inputMode.value = 'quick' // ???????????  quickInput.value = { totalAmount: 0, estimatedPrice: 0 }
  quickEstimate.value = { quantity: 0, costPrice: 0 }
}

const showExportMenu = ref(false)

const handleExportClick = () => {
  showExportMenu.value = !showExportMenu.value
}

const exportData = async (type: 'investments' | 'transactions' | 'stats') => {
  const { success, error: showError } = useNotification()
  const { handleError } = useErrorHandler()

  try {
    const config = useRuntimeConfig()
    const baseUrl = typeof window !== 'undefined' && (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1')
      ? 'http://localhost:5234/api'
      : config.public.apiBase

    const token = typeof window !== 'undefined' ? localStorage.getItem('admin_token') : null

    const url = `/InvestmentExport/${type}/csv`
    const response = await fetch(`${baseUrl}${url}`, {
      method: 'GET',
      headers: token ? {
        Authorization: `Bearer ${token}`
      } : {}
    })

    if (!response.ok) {
      throw new Error('????')
    }

    // ??????    const contentDisposition = response.headers.get('Content-Disposition')
    let fileName = `export_${Date.now()}.csv`
    if (contentDisposition) {
      const fileNameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/)
      if (fileNameMatch && fileNameMatch[1]) {
        fileName = fileNameMatch[1].replace(/['"]/g, '')
      }
    }

    // ????
    const blob = await response.blob()
    const url_blob = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url_blob
    a.download = fileName
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
    window.URL.revokeObjectURL(url_blob)

    success('????')
    showExportMenu.value = false
  } catch (err: any) {
    console.error('????:', err)
    handleError(err)
    showError(err.message || '????')
  }
}

// ??????????
const autoDetectType = () => {
  const code = form.value.code.trim()
  if (!code) return
  
  // ????????6 ?????? 0???? ???  // ????????6 ??????0???? ???  if (code.length === 6 && /^\d+$/.test(code)) {
    // ?????????00??1??5??5??6??1??2??3??4??5??6??7??8??9
    if (code.startsWith('00') || code.startsWith('01') || code.startsWith('05') || 
        code.startsWith('15') || code.startsWith('16') || code.startsWith('51') || 
        code.startsWith('52') || code.startsWith('53') || code.startsWith('54') || 
        code.startsWith('55') || code.startsWith('56') || code.startsWith('57') || 
        code.startsWith('58') || code.startsWith('59')) {
      form.value.type = 'fund'
      
      // ?????????0??1??5?????????????????      if (code.startsWith('00') || code.startsWith('01') || code.startsWith('05')) {
        const { info } = useNotification()
        // ?????????????????
        setTimeout(() => {
          info('???????????????????????????????????)
        }, 500)
      }
    } else if (code.startsWith('0') || code.startsWith('3') || code.startsWith('6')) {
      // A??????0????????????????????????      form.value.type = 'stock'
    }
  }
}

const cancelEdit = () => {
  showCreateModal.value = false
  editingItem.value = null
  form.value = { code: '', name: '', type: 'stock', quantity: 0, costPrice: 0, currentPrice: 0, notes: '' }
  isAutoFilling.value = false
}

// ??????
const toggleDescription = () => {
  showDescription.value = !showDescription.value
}

const fileInput = ref<HTMLInputElement | null>(null)
const importing = ref(false)
const importResult = ref<{ successCount: number; failCount: number; errors: string[] } | null>(null)
const showImportModal = ref(false)

const handleImportClick = () => {
  fileInput.value?.click()
}

const handleFileSelect = async (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (!file) return

  // ??????
  const allowedExtensions = ['.csv', '.xlsx', '.xls']
  const fileExtension = '.' + file.name.split('.').pop()?.toLowerCase()
  if (!allowedExtensions.includes(fileExtension)) {
    const { error } = useNotification()
    error('???????????? CSV ??Excel ??')
    return
  }

  // ???????
  showImportModal.value = true
  importFile(file)
}

const importFile = async (file: File) => {
  importing.value = true
  importResult.value = null

  const { success, error: showError } = useNotification()
  const { handleError } = useErrorHandler()

  try {
    const formData = new FormData()
    formData.append('file', file)

    const config = useRuntimeConfig()
    const baseUrl = typeof window !== 'undefined' && (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1')
      ? 'http://localhost:5234/api'
      : config.public.apiBase

    const token = typeof window !== 'undefined' ? localStorage.getItem('admin_token') : null

    const response = await $fetch<{ code: number; message: string; data: any }>('/InvestmentImport/import', {
      baseURL: baseUrl,
      method: 'POST',
      body: formData,
      headers: token ? {
        Authorization: `Bearer ${token}`
      } : {}
    })

    if (response.code !== undefined && response.code !== 0) {
      throw new Error(response.message || '????')
    }

    const result = response.code === 0 ? response.data : response
    importResult.value = {
      successCount: result.successCount || 0,
      failCount: result.failCount || 0,
      errors: result.errors || []
    }

    if (result.successCount > 0) {
      success(`??????{result.successCount} ???`)
      // ????
      await fetchList()
    }

    if (result.failCount > 0) {
      showError(`??????{result.failCount} ???`)
    }
  } catch (err: any) {
    console.error('????:', err)
    handleError(err)
    showError(err.message || '????')
  } finally {
    importing.value = false
    // ??????
    if (fileInput.value) {
      fileInput.value.value = ''
    }
  }
}

const closeImportModal = () => {
  showImportModal.value = false
  importResult.value = null
}

// ??????????const autoFillFromCode = async () => {
  if (!form.value.code || !form.value.type) {
    const { warning } = useNotification()
    warning('???????????')
    return
  }

  isAutoFilling.value = true
  const { success, warning } = useNotification()
  const { handleError } = useErrorHandler()

  try {
    // ??????????    const code = form.value.code.trim().padStart(6, '0')
    const type = form.value.type
    
    const res = await api.get<any>(`/Investment/auto-fill?code=${encodeURIComponent(code)}&type=${encodeURIComponent(type)}`)
    
    // useApi ??????????res ?? data ??
    // ??????????camelCase (name, currentPrice)????PascalCase (Name, CurrentPrice)
    if (res) {
      // ???????camelCase ??PascalCase
      const name = res.name || res.Name || ''
      const currentPrice = res.currentPrice || res.CurrentPrice || 0
      
      if (name) {
        form.value.name = name
      }
      if (currentPrice && currentPrice > 0) {
        form.value.currentPrice = currentPrice
        // ??????0?????????????        if (form.value.costPrice === 0) {
          form.value.costPrice = currentPrice
        }
        success(`??????${name}???????${formatMoney(currentPrice)}`)
      } else {
        if (name) {
          success(`????????${name}?????????????????????????API??????`)
        } else {
          // ?????????????????????????          const isOTC = form.value.code.startsWith('00') || form.value.code.startsWith('01') || form.value.code.startsWith('05')
          if (isOTC && form.value.type === 'fund') {
            warning('???????API?????????????????????????????????')
          } else {
            warning('???API????????????????API????????????????')
          }
        }
      }
    } else {
      warning('?????????????????)
    }
  } catch (e: unknown) {
    // ????400 ????????????    if ((e as any)?.response?.status === 400 || (e as any)?.code === 400) {
      warning('???????????????????')
    } else {
      handleError(e, '??????')
    }
  } finally {
    isAutoFilling.value = false
  }
}

const formatMoney = (value: number) => {
  return value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const formatPercent = (value: number) => {
  return value.toFixed(2)
}

// ????????const formatRefreshTime = (time: Date | null) => {
  if (!time) return ''
  const now = new Date()
  const diff = Math.floor((now.getTime() - time.getTime()) / 1000) // ??  if (diff < 60) return `${diff}??`
  if (diff < 3600) return `${Math.floor(diff / 60)}???`
  return time.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })
}

// ??????
const generateColors = (count: number) => {
  const colors = [
    '#3B82F6', // blue
    '#10B981', // green
    '#F59E0B', // amber
    '#EF4444', // red
    '#8B5CF6', // purple
    '#EC4899', // pink
    '#06B6D4', // cyan
    '#84CC16', // lime
    '#F97316', // orange
    '#6366F1'  // indigo
  ]
  return colors.slice(0, count)
}

// ECharts ????
const getCommonPieOption = () => {
  const theme = isDark.value ? darkTheme.value : lightTheme.value
  // ??????? theme ??????????  if (!theme) {
    return {
      backgroundColor: 'transparent',
      textStyle: {
        color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)'
      },
      tooltip: {
        trigger: 'item',
        backgroundColor: isDark.value ? 'rgba(17, 24, 39, 0.98)' : 'rgba(255, 255, 255, 0.95)',
        borderColor: isDark.value ? 'rgba(156, 163, 175, 0.5)' : 'rgba(209, 213, 219, 0.8)',
        textStyle: {
          color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)'
        },
        formatter: (params: any) => {
          const { name, value, percent } = params
          return `${name}<br/>?${value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} (${percent}%)`
        }
      },
      legend: {
        orient: 'vertical',
        left: 'left',
        bottom: 'bottom',
        textStyle: {
          color: isDark.value ? 'var(--color-border)' : 'var(--color-text-sec)',
          fontSize: 12,
          fontWeight: 'normal'
        }
      }
    }
  }
  return {
    backgroundColor: theme.backgroundColor || 'transparent',
    textStyle: theme.textStyle || { color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)' },
    tooltip: {
      trigger: 'item',
      backgroundColor: theme.tooltip?.backgroundColor || (isDark.value ? 'rgba(17, 24, 39, 0.98)' : 'rgba(255, 255, 255, 0.95)'),
      borderColor: theme.tooltip?.borderColor || (isDark.value ? 'rgba(156, 163, 175, 0.5)' : 'rgba(209, 213, 219, 0.8)'),
      textStyle: theme.tooltip?.textStyle || { color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)' },
      formatter: (params: any) => {
        const { name, value, percent } = params
        return `${name}<br/>?${value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} (${percent}%)`
      }
    },
    legend: {
      orient: 'vertical',
      left: 'left',
      bottom: 'bottom',
      textStyle: {
        color: theme.legend?.textStyle?.color || (isDark.value ? 'var(--color-border)' : 'var(--color-text-sec)'),
        fontSize: 12,
        fontWeight: 'normal'
      }
    }
  }
}

// ??????????
const typeChartOption = computed(() => {
  if (!stats.value.ByType || stats.value.ByType.length === 0) {
    return {}
  }
  const theme = isDark.value ? darkTheme.value : lightTheme.value
  const data = stats.value.ByType.map((t: any) => ({
    name: t.TypeName || (t.Type === 'stock' ? '??' : '??'),
    value: t.TotalMarketValue || 0
  }))
  const colors = generateColors(data.length)
  
  return {
    ...getCommonPieOption(),
    backgroundColor: theme.backgroundColor,
    textStyle: {
      ...theme.textStyle,
      color: theme.textStyle.color // ????????
    },
    color: colors,
    series: [{
      type: 'pie',
      radius: ['40%', '70%'], // ????????      avoidLabelOverlap: true,
      itemStyle: {
        borderRadius: 8,
        borderColor: isDark.value ? 'var(--color-gray-800)' : 'var(--color-bg-card)',
        borderWidth: 2
      },
      label: {
        show: true,
        color: theme.textStyle.color, // ????????
        fontSize: 13,
        fontWeight: 'normal',
        formatter: (params: any) => {
          return `${params.name}\n${params.percent}%`
        }
      },
      labelLine: {
        show: true,
        lineStyle: {
          color: theme.textStyle.color
        }
      },
      emphasis: {
        itemStyle: {
          shadowBlur: 15,
          shadowOffsetX: 0,
          shadowColor: isDark.value ? 'rgba(255, 255, 255, 0.5)' : 'var(--overlay-color, rgba(0, 0, 0, 0.5))'
        },
        label: {
          color: theme.textStyle.color,
          fontSize: 15,
          fontWeight: 'bold'
        }
      },
      data
    }]
  }
})

// ???????????const profitStatusChartOption = computed(() => {
  if (!stats.value.ByProfitStatus || stats.value.ByProfitStatus.length === 0) {
    return {}
  }
  const theme = isDark.value ? darkTheme.value : lightTheme.value
  const data = stats.value.ByProfitStatus.map((s: any) => ({
    name: s.StatusName || (s.Status === 'profit' ? '??' : '??'),
    value: s.Count || 0
  }))
  const colors = stats.value.ByProfitStatus.map((s: any) => s.Status === 'profit' ? '#10B981' : '#EF4444')
  
  return {
    ...getCommonPieOption(),
    backgroundColor: theme.backgroundColor,
    textStyle: {
      ...theme.textStyle,
      color: theme.textStyle.color
    },
    color: colors,
    series: [{
      type: 'pie',
      radius: ['40%', '70%'], // ????      avoidLabelOverlap: true,
      itemStyle: {
        borderRadius: 8,
        borderColor: isDark.value ? 'var(--color-gray-800)' : 'var(--color-bg-card)',
        borderWidth: 2
      },
      label: {
        show: true,
        color: theme.textStyle.color,
        fontSize: 13,
        fontWeight: 'normal',
        formatter: (params: any) => {
          return `${params.name}\n${params.value}?`
        }
      },
      labelLine: {
        show: true,
        lineStyle: {
          color: theme.textStyle.color
        }
      },
      emphasis: {
        itemStyle: {
          shadowBlur: 15,
          shadowOffsetX: 0,
          shadowColor: isDark.value ? 'rgba(255, 255, 255, 0.5)' : 'var(--overlay-color, rgba(0, 0, 0, 0.5))'
        },
        label: {
          color: theme.textStyle.color,
          fontSize: 15,
          fontWeight: 'bold'
        }
      },
      data
    }]
  }
})

// ?????????Top 10??const assetDistributionChartOption = computed(() => {
  if (!stats.value.AssetDistribution || stats.value.AssetDistribution.length === 0) {
    return { series: [{ data: [] }] }
  }
  const theme = isDark.value ? darkTheme.value : lightTheme.value
  const top10 = stats.value.AssetDistribution.slice(0, 10)
  const data = top10.map((a: any) => ({
    name: `${a.Name} (${a.Code})`,
    value: a.MarketValue || 0
  }))
  const colors = generateColors(data.length)
  
  return {
    ...getCommonPieOption(),
    backgroundColor: theme.backgroundColor,
    textStyle: {
      ...theme.textStyle,
      color: theme.textStyle.color
    },
    color: colors,
    legend: {
      ...getCommonPieOption().legend,
      orient: 'vertical',
      left: 'left',
      top: 'middle',
      textStyle: {
        color: theme.legend.textStyle.color,
        fontSize: 12
      }
    },
    series: [{
      type: 'pie',
      radius: '55%',
      center: ['60%', '50%'],
      avoidLabelOverlap: true,
      itemStyle: {
        borderRadius: 6,
        borderColor: isDark.value ? 'var(--color-gray-800)' : 'var(--color-bg-card)',
        borderWidth: 2
      },
      label: {
        show: true,
        color: theme.textStyle.color,
        fontSize: 11,
        formatter: (params: any) => {
          return `${params.name}\n?${params.value.toLocaleString('zh-CN')}`
        }
      },
      labelLine: {
        show: true,
        lineStyle: {
          color: theme.textStyle.color
        }
      },
      emphasis: {
        itemStyle: {
          shadowBlur: 15,
          shadowOffsetX: 0,
          shadowColor: isDark.value ? 'rgba(255, 255, 255, 0.5)' : 'var(--overlay-color, rgba(0, 0, 0, 0.5))'
        },
        label: {
          color: theme.textStyle.color,
          fontSize: 13,
          fontWeight: 'bold'
        }
      },
      data
    }]
  }
})

// ????????
const profitRankChartOption = computed(() => {
  if (!stats.value.TopByProfit || stats.value.TopByProfit.length === 0) {
    return {}
  }
  const theme = isDark.value ? darkTheme.value : lightTheme.value
  const labels = stats.value.TopByProfit.map((p: any) => `${p.Name} (${p.Code})`)
  const data = stats.value.TopByProfit.map((p: any) => p.ProfitLoss || 0)
  const colors = data.map((d: number) => d >= 0 ? '#10B981' : '#EF4444')
  
  // ?????  if (!theme) {
    return {
      backgroundColor: 'transparent',
      textStyle: { color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)' },
      tooltip: {
        trigger: 'axis',
        axisPointer: { type: 'shadow' },
        backgroundColor: isDark.value ? 'rgba(17, 24, 39, 0.98)' : 'rgba(255, 255, 255, 0.95)',
        borderColor: isDark.value ? 'rgba(156, 163, 175, 0.5)' : 'rgba(209, 213, 219, 0.8)',
        textStyle: { color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)' },
        formatter: (params: any) => {
          const param = params[0]
          return `${param.name}<br/>??: ?${param.value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
        }
      },
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        outerBounds: true,
        borderColor: isDark.value ? 'rgba(255, 255, 255, 0.1)' : 'var(--shadow)'
      },
      xAxis: {
        type: 'category',
        data: labels,
        axisLabel: {
          rotate: 45,
          interval: 0,
          color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)',
          fontSize: 11
        },
        axisLine: {
          lineStyle: {
            color: isDark.value ? 'rgba(255, 255, 255, 0.1)' : 'var(--shadow)'
          }
        },
        splitLine: { show: false }
      },
      yAxis: {
        type: 'value',
        axisLabel: {
          color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)',
          formatter: (value: number) => '?' + value.toLocaleString('zh-CN')
        },
        axisLine: {
          lineStyle: {
            color: isDark.value ? 'rgba(255, 255, 255, 0.1)' : 'var(--shadow)'
          }
        },
        splitLine: {
          lineStyle: {
            color: isDark.value ? 'rgba(255, 255, 255, 0.1)' : 'var(--shadow)',
            type: 'dashed'
          }
        }
      },
      series: [{
        type: 'bar',
        data: data.map((d: number, index: number) => ({
          value: d,
          itemStyle: { color: colors[index] }
        })),
        label: {
          show: true,
          position: 'top',
          color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)',
          fontSize: 11,
          formatter: (params: any) => {
            const value = params.value
            return value >= 0 ? '+' : '' + '?' + value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
          }
        }
      }]
    }
  }
  
  return {
    backgroundColor: theme.backgroundColor || 'transparent',
    textStyle: theme.textStyle || { color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)' },
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      },
      backgroundColor: theme.tooltip?.backgroundColor || (isDark.value ? 'rgba(17, 24, 39, 0.98)' : 'rgba(255, 255, 255, 0.95)'),
      borderColor: theme.tooltip?.borderColor || (isDark.value ? 'rgba(156, 163, 175, 0.5)' : 'rgba(209, 213, 219, 0.8)'),
      textStyle: theme.tooltip?.textStyle || { color: isDark.value ? 'var(--color-bg-card)' : 'var(--color-text-main)' },
      formatter: (params: any) => {
        const param = params[0]
        return `${param.name}<br/>??: ?${param.value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true,
      borderColor: theme.grid.borderColor
    },
    xAxis: {
      type: 'category',
      data: labels,
      axisLabel: {
        rotate: 45,
        interval: 0,
        color: theme.textStyle.color,
        fontSize: 11
      },
      axisLine: {
        lineStyle: {
          color: theme.grid.borderColor
        }
      },
      splitLine: {
        show: false
      }
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        color: theme.textStyle.color,
        formatter: (value: number) => '?' + value.toLocaleString('zh-CN')
      },
      axisLine: {
        lineStyle: {
          color: theme.grid.borderColor
        }
      },
      splitLine: {
        lineStyle: {
          color: theme.grid.borderColor,
          type: 'dashed'
        }
      }
    },
    series: [{
      type: 'bar',
      data: data.map((d: number, index: number) => ({
        value: d,
        itemStyle: {
          color: colors[index]
        }
      })),
      label: {
        show: true,
        position: 'top',
        color: theme.textStyle.color,
        fontSize: 11,
        formatter: (params: any) => {
          const value = params.value
          return value >= 0 ? '+' : '' + '?' + value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
        }
      }
    }]
  }
})

// ??????
const autoRefreshEnabled = ref(true) // ?????????const autoRefreshInterval = ref<NodeJS.Timeout | null>(null)
const lastRefreshTime = ref<Date | null>(null)

// ??????????const startAutoRefresh = () => {
  // ????????  if (autoRefreshInterval.value) {
    clearInterval(autoRefreshInterval.value)
  }
  
  // ?????????????  autoRefreshInterval.value = setInterval(async () => {
    if (!refreshingPrices.value) {
      console.log('[????] ?????????..')
      await refreshPrices()
      lastRefreshTime.value = new Date()
    }
  }, 5 * 60 * 1000) // 5??
}

// ??????
const stopAutoRefresh = () => {
  if (autoRefreshInterval.value) {
    clearInterval(autoRefreshInterval.value)
    autoRefreshInterval.value = null
  }
}

// ?????????watch(autoRefreshEnabled, (enabled) => {
  if (enabled) {
    startAutoRefresh()
  } else {
    stopAutoRefresh()
  }
})

onMounted(() => {
  // ??????
  fetchList()
  
  // ???????????????????????
  setTimeout(async () => {
    if (autoRefreshEnabled.value) {
      console.log('[????] ??????...')
      await refreshPrices()
      lastRefreshTime.value = new Date()
      // ????????
      startAutoRefresh()
    }
  }, 2000) // ??2??????????????})

// ??????????
onUnmounted(() => {
  stopAutoRefresh()
})
</script>

<style scoped>
/* ???? */
.asset-decision-panel {
  padding: 1.5rem;
  max-width: 1400px;
  margin: 0 auto;
}

/* ???? */
.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid var(--color-border-default, var(--color-border));
}

.header-title-section {
  flex: 1;
}

.panel-title {
  font-size: 2rem;
  font-weight: 600;
  color: var(--color-text-main, var(--color-text-main));
  margin: 0 0 0.5rem 0;
  letter-spacing: -0.02em;
}

.panel-subtitle {
  font-size: 0.95rem;
  color: var(--color-text-muted, var(--color-text-sec));
  margin: 0;
  font-weight: 400;
  letter-spacing: 0.01em;
}

.header-actions {
  display: flex;
  gap: 0.75rem;
  align-items: center;
}

.btn-action-primary,
.btn-action-secondary {
  padding: 0.625rem 1.25rem;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
  var(--color-bg-light, white)-space: nowrap;
}

.btn-action-primary {
  background: var(--color-primary, var(--color-primary));
  color: var(--color-text-on-primary, var(--color-bg-card));
}

.btn-action-primary:hover {
  background: var(--color-primary-hover, var(--color-primary-hover));
  transform: translateY(-1px);
  box-shadow: 0 4px 12px var(--theme-primary);
}

.btn-action-secondary {
  background: var(--color-bg-elevated);
  color: var(--color-text-main, var(--color-text-main));
  border: 1px solid var(--color-border-default, var(--color-border));
}

.btn-action-secondary:hover {
  background: var(--color-bg-card, var(--color-bg-card));
  border-color: var(--color-border-subtle);
}

/* ???? */
.module-description {
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: 0.75rem;
  padding: 0;
  margin-bottom: 2rem;
  line-height: 1.7;
}

.description-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  cursor: pointer;
  user-select: none;
  transition: background-color 0.2s;
}

.description-header:hover {
  background: var(--color-bg-card, var(--color-bg-card));
}

.description-icon {
  font-size: 1rem;
}

.description-title {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--color-text-main, var(--color-text-main));
  flex: 1;
}

.description-toggle {
  font-size: 0.75rem;
  color: var(--color-text-muted, var(--color-text-sec));
}

.description-content {
  padding: 0 1rem 1rem 1rem;
}

.description-text {
  font-size: 0.875rem;
  color: var(--color-text-main, var(--color-text-main));
  margin: 0 0 0.75rem 0;
}

.description-warning {
  font-size: 0.8125rem;
  color: var(--color-text-muted, var(--color-text-sec));
  margin: 0;
  padding-top: 0.75rem;
  border-top: 1px solid var(--color-border-default, var(--color-border));
}

/* ?????? */
.feature-card {
  background: var(--color-bg-card, var(--color-bg-card));
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: 0.75rem;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 1px 3px var(--color-border);
}

.feature-card-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-text-main, var(--color-text-main));
  margin: 0 0 1.25rem 0;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid var(--color-border-default, var(--color-border));
}

.feature-list {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.25rem;
}

.feature-item {
  display: flex;
  gap: 1rem;
  align-items: center;
  cursor: help;
  position: relative;
}

.feature-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-bg-elevated);
  border-radius: 0.5rem;
}

.feature-content {
  flex: 1;
}

.feature-name {
  font-size: 0.9375rem;
  font-weight: 500;
  color: var(--color-text-main, var(--color-text-main));
}

/* ?????? */
.import-section {
  margin-bottom: 2rem;
}

.import-card {
  background: var(--color-bg-card, var(--color-bg-card));
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: 0.75rem;
  padding: 1.5rem;
  box-shadow: 0 1px 3px var(--color-border);
}

.import-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.import-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-text-main, var(--color-text-main));
  margin: 0;
  flex: 1;
}

.import-info-icon {
  font-size: 1rem;
  cursor: help;
  opacity: 0.6;
  transition: opacity 0.2s;
}

.import-info-icon:hover {
  opacity: 1;
}

.import-content {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.import-formats {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.format-tag {
  display: inline-block;
  padding: 0.375rem 0.75rem;
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: 0.375rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: var(--color-text-main, var(--color-text-main));
}


.btn-import {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: var(--color-primary, var(--color-primary));
  color: var(--color-text-on-primary, var(--color-bg-card));
  border: none;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  position: relative;
}

.btn-import:hover:not(:disabled) {
  background: var(--color-primary-hover, var(--color-primary-hover));
  transform: translateY(-1px);
  box-shadow: 0 4px 12px var(--theme-primary);
}

.btn-import:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.import-icon {
  font-size: 1.125rem;
}

.import-badge {
  position: absolute;
  top: -0.5rem;
  right: -0.5rem;
  background: var(--color-text-muted, var(--color-text-sec));
  color: var(--color-text-on-primary, var(--color-bg-card));
  font-size: 0.625rem;
  padding: 0.125rem 0.375rem;
  border-radius: 0.75rem;
  font-weight: 500;
}

/* ??????? */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: var(--color-overlay, var(--overlay-color, rgba(0, 0, 0, 0.5)));
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  backdrop-filter: blur(4px);
}

.modal-content {
  background: var(--color-bg-card, var(--color-bg-card));
  border-radius: 0.5rem;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  width: 100%;
  max-width: 28rem;
  margin: 1rem;
  border: 1px solid var(--color-border-default, var(--color-border));
  max-height: 90vh;
  overflow-y: auto;
}

.modal-body {
  padding: 1.5rem;
}

.modal-title {
  font-size: 1.25rem;
  font-weight: bold;
  margin-bottom: 1rem;
  color: var(--color-text-main, var(--color-text-main));
}

.modal-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  display: block;
}

.form-label {
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  margin-bottom: 0.5rem;
  color: var(--color-text-main, var(--color-text-main));
}

.form-input,
.form-select,
.form-textarea {
  width: 100%;
  padding: 0.5rem 0.75rem;
  border: 1px solid var(--color-border-default, var(--color-gray-300));
  border-radius: 0.375rem;
  background: var(--color-bg-card, var(--color-bg-card));
  color: var(--color-text-main, var(--color-text-main));
  font-size: 0.875rem;
}

.form-input:focus,
.form-select:focus,
.form-textarea:focus {
  outline: none;
  ring: 2px;
  ring-color: var(--color-primary, var(--color-primary));
  border-color: var(--color-primary, var(--color-primary));
}

.form-textarea {
  resize: vertical;
  min-height: 4rem;
}

.modal-actions {
  display: flex;
  gap: 0.75rem;
  justify-content: flex-end;
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid var(--color-border-default, var(--color-border));
}

.btn-save,
.btn-cancel {
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-save {
  background: var(--color-primary, var(--color-primary));
  color: var(--color-text-on-primary, var(--color-bg-card));
  border: none;
}

.btn-save:hover {
  background: var(--color-primary-hover, var(--color-primary-hover));
}

.btn-cancel {
  background: transparent;
  color: var(--color-text-main, var(--color-text-sec));
  border: 1px solid var(--color-border-default, var(--color-gray-300));
}

.btn-cancel:hover {
  background: var(--color-bg-elevated);
}

.form-hint {
  font-size: 0.75rem;
  color: var(--color-text-muted, var(--color-text-sec));
  margin-top: 0.25rem;
  line-height: 1.4;
}

.form-hint-tip {
  display: inline-block;
  margin-top: 0.25rem;
  color: var(--color-text-muted, var(--color-text-sec));
  font-size: 0.7rem;
  opacity: 0.8;
}

.form-input-group {
  display: flex;
  gap: 0.5rem;
  align-items: stretch;
}

.form-input-group .form-input {
  flex: 1;
}

.btn-auto-fill {
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  background: var(--color-primary, var(--color-primary));
  color: var(--color-text-on-primary, var(--color-bg-card));
  border: none;
  var(--color-bg-light, white)-space: nowrap;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.btn-auto-fill:hover:not(:disabled) {
  background: var(--color-primary-hover, var(--color-primary-hover));
}

.btn-auto-fill:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.spinner {
  display: inline-block;
  width: 0.875rem;
  height: 0.875rem;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: var(--color-text-on-primary, var(--color-bg-card));
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.form-info-box {
  padding: 0.75rem;
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: 0.375rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.form-info-label {
  font-size: 0.875rem;
  color: var(--color-text-muted, var(--color-text-sec));
}

.form-info-value {
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-primary, var(--color-primary));
}

.form-info-hint {
  font-size: 0.75rem;
  color: var(--color-text-muted, var(--color-text-sec));
  margin-left: auto;
}

/* ?????? */
.input-mode-switch {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem;
  background: var(--color-bg-elevated);
  border-radius: 0.5rem;
  border: 1px solid var(--color-border-default, var(--color-border));
}

.switch-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--color-text-main, var(--color-text-main));
  var(--color-bg-light, white)-space: nowrap;
}

.switch-buttons {
  display: flex;
  gap: 0.5rem;
  flex: 1;
}

.switch-btn {
  flex: 1;
  padding: 0.5rem 1rem;
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: 0.375rem;
  background: var(--color-bg-card, var(--color-bg-card));
  color: var(--color-text-main, var(--color-text-main));
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.switch-btn:hover {
  background: var(--color-bg-elevated);
  border-color: var(--color-primary, var(--color-primary));
}

.switch-btn.active {
  background: var(--color-primary, var(--color-primary));
  color: var(--color-text-on-primary, var(--color-bg-card));
  border-color: var(--color-primary, var(--color-primary));
  font-weight: 500;
}

/* ????????*/
.input-with-unit {
  position: relative;
  display: flex;
  align-items: center;
}

.input-with-unit .form-input {
  padding-right: 3rem;
}

.input-unit {
  position: absolute;
  right: 0.75rem;
  font-size: 0.875rem;
  color: var(--color-text-muted, var(--color-text-sec));
  pointer-events: none;
  font-weight: 500;
}

/* ???????*/
.estimate-result {
  padding: 1rem;
  background: var(--color-primary-soft, var(--color-blue-50));
  border: 1px solid var(--color-primary, var(--color-primary));
  border-radius: 0.5rem;
  margin-top: 0.5rem;
}

.estimate-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.estimate-item:last-child {
  margin-bottom: 0;
}

.estimate-label {
  font-size: 0.875rem;
  color: var(--color-text-main, var(--color-text-main));
  font-weight: 500;
}

.estimate-value {
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-primary, var(--color-primary));
}

/* ?????? */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: var(--color-bg-card, var(--color-bg-card));
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: 0.75rem;
  padding: 1.25rem;
  box-shadow: 0 1px 3px var(--color-border);
}

.stat-label {
  font-size: 0.875rem;
  color: var(--color-text-muted, var(--color-text-sec));
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.stat-value,
.stat-value-blue,
.stat-value-positive,
.stat-value-negative {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text-main, var(--color-text-main));
}

.stat-value-blue {
  color: var(--color-primary, var(--color-primary));
}

.stat-value-positive {
  color: var(--success, var(--color-success, var(--color-success)));
}

.stat-value-negative {
  color: var(--error, var(--color-error, var(--color-danger)));
}

/* ???? */
.charts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.chart-container {
  background: var(--color-bg-card, var(--color-bg-card));
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: 0.75rem;
  padding: 1.5rem;
  box-shadow: var(--shadow-sm, 0 1px 3px var(--color-border));
}

.chart-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-text-main, var(--color-text-main));
  margin: 0 0 1rem 0;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid var(--color-border-default, var(--color-border));
}

.chart-wrapper {
  min-height: 300px;
}

.chart-empty {
  min-height: 300px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-text-muted, var(--color-text-sec));
  font-size: 0.875rem;
}

/* ?????? */
.investment-table-container {
  background: var(--color-bg-card, var(--color-bg-card));
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: 0.75rem;
  padding: 1.5rem;
  box-shadow: var(--shadow-sm, 0 1px 3px var(--color-border));
  overflow-x: auto;
}

.investment-table {
  width: 100%;
  border-collapse: collapse;
}

.table-header-hint {
  font-size: 0.75rem;
  color: var(--color-text-muted, var(--color-text-sec));
  cursor: help;
  margin-left: 0.25rem;
}

.price-zero-hint {
  color: var(--color-text-muted, var(--color-text-sec));
  position: relative;
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
}

.hint-icon {
  font-size: 0.875rem;
  cursor: help;
}

.investment-table th,
.investment-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid var(--color-border-default, var(--color-border));
}

.investment-table th {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--color-text-muted, var(--color-text-sec));
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.investment-table td {
  font-size: 0.875rem;
  color: var(--color-text-main, var(--color-text-main));
}

.badge-stock,
.badge-fund {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.badge-stock {
  background: var(--color-bg-elevated);
  color: var(--color-primary, var(--color-primary));
  border: 1px solid var(--color-border-default, var(--color-border));
}

.badge-fund {
  background: var(--color-bg-elevated);
  color: var(--success, var(--color-success, var(--color-success)));
  border: 1px solid var(--color-border-default, var(--color-border));
}

.profit-positive,
.profit-negative {
  font-weight: 500;
}

.profit-positive {
  color: var(--success, var(--color-success, var(--color-success)));
}

.profit-negative {
  color: var(--error, var(--color-error, var(--color-danger)));
}

.profit-rate-positive,
.profit-rate-negative {
  font-size: 0.75rem;
  margin-top: 0.25rem;
}

.profit-rate-positive {
  color: var(--success, var(--color-success, var(--color-success)));
}

.profit-rate-negative {
  color: var(--error, var(--color-error, var(--color-danger)));
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.btn-edit,
.btn-transaction,
.btn-delete {
  padding: 0.375rem 0.75rem;
  border-radius: 0.375rem;
  font-size: 0.8125rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
}

.btn-edit {
  background: var(--color-bg-elevated);
  color: var(--color-text-main, var(--color-text-main));
  border: 1px solid var(--color-border-default, var(--color-border));
}

.btn-edit:hover {
  background: var(--color-bg-card, var(--color-bg-card));
}

.btn-transaction {
  background: var(--color-primary, var(--color-primary));
  color: var(--color-text-on-primary, var(--color-bg-card));
}

.btn-transaction:hover {
  background: var(--color-primary-hover, var(--color-primary-hover));
}

.btn-delete {
  background: transparent;
  color: var(--error, var(--color-error, var(--color-danger)));
  border: 1px solid var(--error, var(--color-error, var(--color-danger)));
}

.btn-delete:hover {
  background: var(--error, var(--color-error, var(--color-danger)));
  color: var(--color-text-on-primary, var(--color-bg-card));
}

/* ?????? */
.import-status {
  text-align: center;
  padding: 2rem;
}

.import-status .spinner {
  display: inline-block;
  width: 2rem;
  height: 2rem;
  border: 3px solid #f3f3f3;
  border-top: 3px solid var(--color-blue-500);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.import-result {
  padding: 1rem 0;
}

.result-summary {
  display: flex;
  gap: 2rem;
  margin-bottom: 1.5rem;
}

.result-item {
  flex: 1;
  padding: 1rem;
  border-radius: 4px;
}

.result-item.success {
  background: var(--color-green-50);
  border: 1px solid var(--color-green-200);
}

.result-item.error {
  background: var(--color-orange-50);
  border: 1px solid var(--color-orange-200);
}

.result-label {
  font-weight: 500;
  margin-right: 0.5rem;
}

.result-value {
  font-size: 1.25rem;
  font-weight: 600;
}

.result-errors {
  margin-top: 1rem;
  padding: 1rem;
  background: var(--color-red-50);
  border: 1px solid var(--color-red-200);
  border-radius: 4px;
}

.result-errors h4 {
  margin: 0 0 0.5rem 0;
  font-size: 0.875rem;
  color: var(--color-danger);
}

.result-errors ul {
  margin: 0;
  padding-left: 1.5rem;
  font-size: 0.875rem;
  color: var(--color-gray-500);
}

.result-errors li {
  margin-bottom: 0.25rem;
}

.import-info {
  padding: 1rem 0;
}

.import-hint {
  margin-bottom: 1rem;
  font-weight: 500;
}

.import-format-list {
  margin: 1rem 0;
  padding-left: 1.5rem;
  line-height: 1.8;
}

.import-format-list li {
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
  color: var(--color-gray-500);
}

.import-example {
  margin-top: 1.5rem;
  padding: 1rem;
  background: var(--color-gray-100);
  border-radius: 4px;
}

.import-example p {
  margin: 0 0 0.5rem 0;
  font-size: 0.875rem;
}

.import-example pre {
  margin: 0;
  padding: 0.75rem;
  background: var(--color-bg-light, white);
  border: 1px solid var(--color-gray-200);
  border-radius: 4px;
  font-size: 0.75rem;
  overflow-x: auto;
}

/* ?????? */
.export-menu-container {
  position: relative;
}

.export-menu {
  position: absolute;
  top: 100%;
  right: 0;
  margin-top: 0.5rem;
  background: var(--color-bg-light, white);
  border: 1px solid var(--color-gray-200);
  border-radius: 4px;
  box-shadow: 0 2px 8px var(--shadow);
  z-index: 1000;
  min-width: 160px;
}

.export-menu-item {
  display: block;
  width: 100%;
  padding: 0.75rem 1rem;
  text-align: left;
  border: none;
  background: none;
  cursor: pointer;
  font-size: 0.875rem;
  color: var(--color-gray-700);
}

.export-menu-item:hover {
  background: var(--color-gray-100);
}

.export-menu-item:first-child {
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
}

.export-menu-item:last-child {
  border-bottom-left-radius: 4px;
  border-bottom-right-radius: 4px;
}

/* ?????? */
.auto-refresh-control {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.5rem;
  margin-right: 0.75rem;
}

.auto-refresh-switch {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  user-select: none;
}

.auto-refresh-checkbox {
  width: 18px;
  height: 18px;
  cursor: pointer;
  accent-color: var(--color-primary, var(--color-primary));
}

.auto-refresh-label {
  font-size: 0.875rem;
  color: var(--color-text-main, var(--color-text-main));
  font-weight: 500;
}

.last-refresh-time {
  font-size: 0.75rem;
  color: var(--color-text-muted, var(--color-text-sec));
  font-style: italic;
}
</style>

