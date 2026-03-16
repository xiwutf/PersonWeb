<template>
  <div class="asset-decision-panel">
    <!-- 页面头部 -->
    <div class="panel-header">
      <div class="header-title-section">
        <h1 class="panel-title">个人资产决策面板</h1>
        <p class="panel-subtitle">一个不依赖任何交易平台的个人投资分析系�?/p>
      </div>
      <div class="header-actions">
        <div class="auto-refresh-control">
          <label class="auto-refresh-switch">
            <input 
              type="checkbox" 
              v-model="autoRefreshEnabled"
              class="auto-refresh-checkbox"
            />
            <span class="auto-refresh-label">自动刷新</span>
          </label>
          <span v-if="lastRefreshTime" class="last-refresh-time">
            上次刷新: {{ formatRefreshTime(lastRefreshTime) }}
          </span>
        </div>
        <button @click="refreshPrices" class="btn-action-secondary" :disabled="refreshingPrices">
          <span v-if="refreshingPrices">刷新�?..</span>
          <span v-else>🔄 刷新价格</span>
        </button>
        <div class="export-menu-container">
          <button @click="handleExportClick" class="btn-action-secondary">📥 导出数据</button>
          <div v-if="showExportMenu" class="export-menu">
            <button @click="exportData('investments')" class="export-menu-item">导出投资记录</button>
            <button @click="exportData('transactions')" class="export-menu-item">导出交易记录</button>
            <button @click="exportData('stats')" class="export-menu-item">导出统计数据</button>
          </div>
        </div>
        <button @click="handleAddClick" class="btn-action-primary">+ 新增投资</button>
      </div>
    </div>

    <!-- 模块说明 -->
    <div class="module-description">
      <div class="description-header" @click="toggleDescription">
        <span class="description-icon">ℹ️</span>
        <span class="description-title">模块说明</span>
        <span class="description-toggle">{{ showDescription ? '�? : '�? }}</span>
      </div>
      <div v-show="showDescription" class="description-content">
        <p class="description-text">
          本模块用于记录你的真实投资持仓，并自动同步市场行情，
          帮助你清晰了解资产结构、盈亏情况与长期收益表现�?        </p>
        <p class="description-warning">
          ⚠️ 不提供交易功能，不接入任何第三方账户，仅用于个人资产管理与分析�?        </p>
      </div>
    </div>

    <!-- 功能说明卡片 -->
    <div class="feature-card">
      <h3 class="feature-card-title">核心功能</h3>
      <div class="feature-list">
        <div class="feature-item" :title="'汇总基�?/ ETF / 股票的当前市值与盈亏'">
          <div class="feature-icon">📊</div>
          <div class="feature-content">
            <div class="feature-name">汇总资产与盈亏</div>
          </div>
        </div>
        <div class="feature-item" :title="'自动获取最新市场价格，实时计算收益'">
          <div class="feature-icon">🔄</div>
          <div class="feature-content">
            <div class="feature-name">实时市场行情</div>
          </div>
        </div>
        <div class="feature-item" :title="'用数据，而不是感觉，辅助投资决策'">
          <div class="feature-icon">📈</div>
          <div class="feature-content">
            <div class="feature-name">数据驱动决策</div>
          </div>
        </div>
        <div class="feature-item" :title="'数据完全由用户掌控，不绑定任何平台账�?">
          <div class="feature-icon">🔒</div>
          <div class="feature-content">
            <div class="feature-name">数据完全掌控</div>
          </div>
        </div>
      </div>
    </div>

    <!-- 交易记录导入入口 -->
    <div class="import-section">
      <div class="import-card">
        <div class="import-header">
          <h3 class="import-title">交易记录导入（半自动同步�?/h3>
          <span class="import-info-icon" :title="'由于交易平台未开放接口，本系统不支持自动登录同步。支持导入官方交易记录文件，系统将自动识别并生成持仓数据。不需要账号密码，不涉及隐私授权�?">ℹ️</span>
        </div>
        <div class="import-content">
          <div class="import-formats">
            <span class="format-tag">Excel</span>
            <span class="format-tag">CSV</span>
          </div>
          <button @click="handleImportClick" class="btn-import">
            <span class="import-icon">📥</span>
            选择文件导入
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

    <!-- 统计卡片 -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-label">总成�?/div>
        <div class="stat-value">¥{{ formatMoney(stats.TotalCost || 0) }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">总市�?/div>
        <div class="stat-value-blue">¥{{ formatMoney(stats.TotalMarketValue || 0) }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">总盈�?/div>
        <div :class="(stats.TotalProfitLoss || 0) >= 0 ? 'stat-value-positive' : 'stat-value-negative'">
          ¥{{ formatMoney(stats.TotalProfitLoss || 0) }}
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-label">收益�?/div>
        <div :class="(stats.TotalProfitRate || 0) >= 0 ? 'stat-value-positive' : 'stat-value-negative'">
          {{ formatPercent(stats.TotalProfitRate || 0) }}%
        </div>
      </div>
    </div>

    <!-- 图表分析区域 -->
    <div class="charts-grid">
      <!-- 资产类型分布饼状�?-->
      <div class="chart-container">
        <h2 class="chart-title">资产类型分布</h2>
        <div v-if="stats.ByType && stats.ByType.length > 0" class="chart-wrapper">
          <v-chart :option="typeChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">暂无数据</div>
      </div>

      <!-- 盈亏状态分布饼状图 -->
      <div class="chart-container">
        <h2 class="chart-title">盈亏状态分�?/h2>
        <div v-if="stats.ByProfitStatus && stats.ByProfitStatus.length > 0" class="chart-wrapper">
          <v-chart :option="profitStatusChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">暂无数据</div>
      </div>

      <!-- 资产分布（按代码）饼状图 -->
      <div class="chart-container">
        <h2 class="chart-title">资产分布（Top 10�?/h2>
        <div v-if="assetDistributionChartOption.series && assetDistributionChartOption.series[0].data.length > 0" class="chart-wrapper">
          <v-chart :option="assetDistributionChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">暂无数据</div>
      </div>

      <!-- 收益排行柱状�?-->
      <div class="chart-container">
        <h2 class="chart-title">收益排行（Top 5�?/h2>
        <div v-if="stats.TopByProfit && stats.TopByProfit.length > 0" class="chart-wrapper">
          <v-chart :option="profitRankChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">暂无数据</div>
      </div>
    </div>

    <!-- 统计表格 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- 按类型统计表�?-->
      <div class="stats-table-container">
        <h2 class="chart-title">按类型统�?/h2>
        <div class="overflow-x-auto">
          <table class="stats-table">
            <thead>
              <tr>
                <th>类型</th>
                <th>数量</th>
                <th>总成�?/th>
                <th>总市�?/th>
                <th>盈亏</th>
                <th>收益�?/th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in stats.ByType" :key="index">
                <td>{{ item.TypeName || item.Type }}</td>
                <td>{{ item.Count }}</td>
                <td>¥{{ formatMoney(item.TotalCost) }}</td>
                <td>¥{{ formatMoney(item.TotalMarketValue) }}</td>
                <td :class="item.ProfitLoss >= 0 ? 'profit-positive' : 'profit-negative'">
                  ¥{{ formatMoney(item.ProfitLoss) }}
                </td>
                <td :class="item.ProfitRate >= 0 ? 'profit-positive' : 'profit-negative'">
                  {{ formatPercent(item.ProfitRate) }}%
                </td>
              </tr>
              <tr v-if="!stats.ByType || stats.ByType.length === 0">
                <td colspan="6" class="stats-table-empty">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Top 5 持仓 -->
      <div class="top-holdings-container">
        <h2 class="top-holdings-title">Top 5 持仓（按市值）</h2>
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
                  {{ item.Type === 'stock' ? '股票' : '基金' }}
                </div>
              </div>
            </div>
            <div class="holding-value">
              <div class="holding-value-amount">
                ¥{{ formatMoney(item.MarketValue) }}
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
            暂无数据
          </div>
        </div>
      </div>
    </div>

    <!-- 投资列表 -->
    <div class="table-container">
      <table class="investment-table">
        <thead>
          <tr>
            <th>代码</th>
            <th>名称</th>
            <th>类型</th>
            <th>持仓</th>
            <th>成本�?/th>
            <th>当前�?
              <span class="table-header-hint" title="如果显示为�?.00，请点击上方�?刷新价格'按钮">💡</span>
            </th>
            <th>市�?/th>
            <th>盈亏</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in investments" :key="item.id">
            <td class="font-mono">{{ item.code }}</td>
            <td>{{ item.name }}</td>
            <td>
              <span :class="item.type === 'stock' ? 'badge-stock' : 'badge-fund'">
                {{ item.type === 'stock' ? '股票' : '基金' }}
              </span>
            </td>
            <td>{{ item.quantity }}</td>
            <td>¥{{ formatMoney(item.costPrice) }}</td>
            <td>
              <span v-if="item.currentPrice > 0">¥{{ formatMoney(item.currentPrice) }}</span>
              <span v-else class="price-zero-hint" title="当前价为0，请点击上方�?刷新价格'按钮获取最新价�?>
                ¥0.00
                <span class="hint-icon">⚠️</span>
              </span>
            </td>
            <td>
              <span v-if="item.marketValue > 0">¥{{ formatMoney(item.marketValue) }}</span>
              <span v-else class="price-zero-hint">¥0.00</span>
            </td>
            <td>
              <div :class="item.profitLoss >= 0 ? 'profit-positive' : 'profit-negative'">
                ¥{{ formatMoney(item.profitLoss) }}
              </div>
              <div :class="item.profitRate >= 0 ? 'profit-rate-positive' : 'profit-rate-negative'">
                {{ formatPercent(item.profitRate) }}%
              </div>
            </td>
            <td>
              <div class="action-buttons">
                <button @click="editItem(item)" class="btn-edit">编辑</button>
                <button @click="addTransaction(item)" class="btn-transaction">交易</button>
                <button @click="deleteItem(item.id)" class="btn-delete">删除</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 创建/编辑模态框 -->
    <div v-if="showCreateModal || editingItem" class="modal-overlay">
      <div class="modal-content">
        <div class="modal-body">
          <h2 class="modal-title">{{ editingItem ? '编辑' : '新增' }}投资</h2>
          
          <div class="modal-form">
            <div class="form-group">
              <label class="form-label">代码 <span class="text-red-500">*</span></label>
              <div class="form-input-group">
                <input 
                  v-model="form.code" 
                  type="text" 
                  class="form-input" 
                  placeholder="例如: 000001 (股票) �?005918 (基金)" 
                  @blur="autoDetectType"
                />
                <button 
                  @click="autoFillFromCode" 
                  :disabled="!form.code || !form.type || isAutoFilling"
                  class="btn-auto-fill"
                  type="button"
                >
                  <span v-if="isAutoFilling" class="spinner"></span>
                  <span v-else>🔍 自动获取</span>
                </button>
              </div>
              <div class="form-hint">
                输入6位数字代码，点击"自动获取"可自动填充名称和当前价格
                <br />
                <span class="form-hint-tip">💡 <strong>场外基金（如005918）无法自动获�?/strong>，这是正常的。请手动填写名称，或使用下方�?快速估�?模式直接录入</span>
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">名称 <span class="text-red-500">*</span></label>
              <input v-model="form.name" type="text" class="form-input" placeholder="例如: 天弘沪深300ETF联接C、平安银�? />
              <div class="form-hint">
                投资标的的名�?                <br />
                <span class="form-hint-tip">💡 <strong>场外基金（如005918�?/strong>：可以从支付宝等平台复制基金名称，例�?天弘沪深300ETF联接C"</span>
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">类型 <span class="text-red-500">*</span></label>
              <select v-model="form.type" class="form-select">
                <option value="stock">股票</option>
                <option value="fund">基金</option>
              </select>
              <div class="form-hint">根据代码自动识别，可手动修改</div>
            </div>
            <!-- 录入模式切换 -->
            <div class="form-group">
              <div class="input-mode-switch">
                <label class="switch-label">录入模式�?/label>
                <div class="switch-buttons">
                  <button 
                    :class="['switch-btn', { active: inputMode === 'quick' }]"
                    @click="inputMode = 'quick'"
                    type="button"
                  >
                    🚀 快速估�?                  </button>
                  <button 
                    :class="['switch-btn', { active: inputMode === 'detail' }]"
                    @click="inputMode = 'detail'"
                    type="button"
                  >
                    📊 详细录入
                  </button>
                </div>
              </div>
            </div>

            <!-- 快速估算模�?-->
            <template v-if="inputMode === 'quick'">
              <div class="form-group">
                <label class="form-label">总投资金�?<span class="text-red-500">*</span></label>
                <div class="input-with-unit">
                  <input 
                    v-model.number="quickInput.totalAmount" 
                    type="number" 
                    step="0.01" 
                    min="0.01"
                    class="form-input" 
                    placeholder="例如: 1000" 
                    @input="calculateQuickEstimate"
                  />
                  <span class="input-unit">�?/span>
                </div>
              <div class="form-hint">
                您总共投入的金额（比如买了1000元的基金�?                  <br />
                  <span class="form-hint-tip">💡 <strong>推荐使用此模式！</strong> 只需要知道总投入金额，系统会自动计算持仓数量。可以从支付宝等平台查看您的总投入金�?/span>
                </div>
              </div>
              <div class="form-group">
                <label class="form-label">大概买入价格 <span class="text-red-500">*</span></label>
                <div class="input-with-unit">
                  <input 
                    v-model.number="quickInput.estimatedPrice" 
                    type="number" 
                    step="0.01" 
                    min="0.01"
                    class="form-input" 
                    placeholder="例如: 1.4" 
                    @input="calculateQuickEstimate"
                  />
                  <span class="input-unit">�?�?/span>
                </div>
                <div class="form-hint">
                  您大概记得的买入价格，如果不记得可以用当前价�?                  <br />
                  <span class="form-hint-tip">💡 <strong>对于场外基金（如005918�?/strong>：如果不记得精确价格，可以填写大概价格（�?.4），系统会自动计算持仓数量。稍后可以编辑修�?/span>
                </div>
              </div>
              <div v-if="quickEstimate.quantity > 0" class="form-group">
                <div class="estimate-result">
                  <div class="estimate-item">
                    <span class="estimate-label">自动计算持仓数量�?/span>
                    <span class="estimate-value">{{ formatMoney(quickEstimate.quantity) }} �?/span>
                  </div>
                  <div class="estimate-item">
                    <span class="estimate-label">成本价：</span>
                    <span class="estimate-value">¥{{ formatMoney(quickEstimate.costPrice) }}</span>
                  </div>
                </div>
              </div>
            </template>

            <!-- 详细录入模式 -->
            <template v-else>
              <div class="form-group">
                <label class="form-label">持仓数量 <span class="text-red-500">*</span></label>
                <input 
                  v-model.number="form.quantity" 
                  type="number" 
                  step="0.01" 
                  min="0.01"
                  class="form-input" 
                  placeholder="例如: 1000" 
                  required
                />
                <div class="form-hint">
                  您持有的数量（股数或份额），必须大于0
                  <br />
                  <span class="form-hint-tip">💡 可以从支付宝等平台查看您的持仓份�?/span>
                </div>
              </div>
              <div class="form-group">
                <label class="form-label">成本�?<span class="text-red-500">*</span></label>
                <input 
                  v-model.number="form.costPrice" 
                  type="number" 
                  step="0.01" 
                  min="0.01"
                  class="form-input" 
                  placeholder="例如: 1.4078" 
                  required
                />
                <div class="form-hint">
                  您买入时的价格（�?�?�?�?份），必须大�?
                  <br />
                  <span class="form-hint-tip">💡 如果不记得精确价格，可以填写大概价格，稍后可以编辑修�?/span>
                </div>
              </div>
            </template>
            <div class="form-group">
              <label class="form-label">当前价格 <span class="text-red-500">*</span></label>
              <div class="input-with-unit">
                <input 
                  v-model.number="form.currentPrice" 
                  type="number" 
                  step="0.0001" 
                  min="0"
                  class="form-input" 
                  placeholder="例如: 1.4012" 
                  required
                />
                <span class="input-unit">�?�?/span>
              </div>
              <div class="form-hint">
                当前市场价格（元/�?�?�?份）
                <br />
                <span class="form-hint-tip">💡 <strong>如果自动获取失败</strong>：可以从支付宝等平台查看当前净值，手动填写。例如：金额995.34�?÷ 持仓710.3282�?�?1.4012�?�?/span>
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">备注</label>
              <textarea v-model="form.notes" rows="3" class="form-textarea" placeholder="可选：其他备注信息"></textarea>
            </div>
          </div>

          <div class="modal-actions">
            <button @click="saveItem" class="btn-save">保存</button>
            <button @click="cancelEdit" class="btn-cancel">取消</button>
          </div>
        </div>
      </div>
    </div>

    <!-- 导入模态框 -->
    <div v-if="showImportModal" class="modal-overlay" @click="closeImportModal">
      <div class="modal-content" @click.stop>
        <div class="modal-body">
          <h2 class="modal-title">导入交易记录</h2>
          
          <div v-if="importing" class="import-status">
            <div class="spinner"></div>
            <p>正在导入，请稍�?..</p>
          </div>

          <div v-else-if="importResult" class="import-result">
            <div class="result-summary">
              <div class="result-item success">
                <span class="result-label">成功�?/span>
                <span class="result-value">{{ importResult.successCount }} �?/span>
              </div>
              <div class="result-item error">
                <span class="result-label">失败�?/span>
                <span class="result-value">{{ importResult.failCount }} �?/span>
              </div>
            </div>
            
            <div v-if="importResult.errors.length > 0" class="result-errors">
              <h4>错误详情�?/h4>
              <ul>
                <li v-for="(error, index) in importResult.errors" :key="index">{{ error }}</li>
              </ul>
            </div>
          </div>

          <div v-else class="import-info">
            <p class="import-hint">
              <strong>文件格式要求�?/strong>
            </p>
            <ul class="import-format-list">
              <li>支持 CSV �?Excel 格式�?csv, .xlsx, .xls�?/li>
              <li>CSV 文件格式：代�?名称,类型,交易类型,数量,价格,交易日期,备注</li>
              <li>第一行为表头，从第二行开始为数据</li>
              <li>类型：stock（股票）�?fund（基金）</li>
              <li>交易类型：buy（买入）�?sell（卖出）</li>
            </ul>
            <div class="import-example">
              <p><strong>示例�?/strong></p>
              <pre>代码,名称,类型,交易类型,数量,价格,交易日期,备注
005918,天弘沪深300ETF联接C,fund,buy,1000,1.4078,2024-01-01,支付宝购�?000001,平安银行,stock,buy,100,12.50,2024-01-02,</pre>
            </div>
          </div>

          <div class="modal-actions">
            <button @click="closeImportModal" class="btn-cancel">关闭</button>
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

// 注册 ECharts 组件
use([
  CanvasRenderer,
  PieChart,
  BarChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
])

// 使用 ECharts 主题配置
const { isDark, darkTheme, lightTheme } = useEChartsTheme()

// 注册自定义深色主�?registerTheme('dark-custom', {
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

// 注册自定义浅色主�?registerTheme('light-custom', {
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
const inputMode = ref<'quick' | 'detail'>('quick') // 默认使用快速估算模�?const showDescription = ref(false) // 模块说明默认折叠
const form = ref({
  code: '',
  name: '',
  type: 'stock',
  quantity: 0,
  costPrice: 0,
  currentPrice: 0,
  notes: ''
})

// 快速估算输�?const quickInput = ref({
  totalAmount: 0,      // 总投资金�?  estimatedPrice: 0    // 大概买入价格
})

// 快速估算结�?const quickEstimate = ref({
  quantity: 0,         // 计算出的持仓数量
  costPrice: 0         // 成本价（等于estimatedPrice�?})

// 计算快速估�?const calculateQuickEstimate = () => {
  if (quickInput.value.totalAmount > 0 && quickInput.value.estimatedPrice > 0) {
    quickEstimate.value.quantity = quickInput.value.totalAmount / quickInput.value.estimatedPrice
    quickEstimate.value.costPrice = quickInput.value.estimatedPrice
    
    // 同步到表�?    form.value.quantity = quickEstimate.value.quantity
    form.value.costPrice = quickEstimate.value.costPrice
  } else {
    quickEstimate.value.quantity = 0
    quickEstimate.value.costPrice = 0
  }
}

// 监听当前价格变化，自动填充到快速估�?watch(() => form.value.currentPrice, (newPrice) => {
  if (newPrice > 0 && inputMode.value === 'quick' && quickInput.value.estimatedPrice === 0) {
    quickInput.value.estimatedPrice = newPrice
    calculateQuickEstimate()
  }
})

// 转换后端数据格式（PascalCase -> camelCase�?const transformInvestment = (item: any): Investment => {
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
    
    // 处理投资列表数据
    let investmentList: any[] = []
    if (Array.isArray(res)) {
      investmentList = res
    } else if (res && Array.isArray(res.List)) {
      investmentList = res.List
    } else if (res && Array.isArray(res.data)) {
      investmentList = res.data
    }
    
    // 转换数据格式
    investments.value = investmentList.map(transformInvestment)

    // 获取统计数据
    const statsRes = await api.get<any>('/Investment/stats')
    if (statsRes) {
      // 统计数据字段名转换（后端返回 PascalCase，前端使�?camelCase�?      stats.value = {
        TotalCost: statsRes.TotalCost ?? statsRes.totalCost ?? 0,
        TotalMarketValue: statsRes.TotalMarketValue ?? statsRes.totalMarketValue ?? 0,
        TotalProfitLoss: statsRes.TotalProfitLoss ?? statsRes.totalProfitLoss ?? 0,
        TotalProfitRate: statsRes.TotalProfitRate ?? statsRes.totalProfitRate ?? 0,
        TotalCount: statsRes.TotalCount ?? statsRes.totalCount ?? 0,
        ByType: (statsRes.ByType ?? statsRes.byType ?? []).map((item: any) => ({
          Type: item.Type ?? item.type ?? '',
          TypeName: item.TypeName ?? item.typeName ?? (item.Type === 'stock' || item.type === 'stock' ? '股票' : '基金'),
          Count: item.Count ?? item.count ?? 0,
          TotalCost: item.TotalCost ?? item.totalCost ?? 0,
          TotalMarketValue: item.TotalMarketValue ?? item.totalMarketValue ?? 0,
          ProfitLoss: item.ProfitLoss ?? item.profitLoss ?? 0,
          ProfitRate: item.ProfitRate ?? item.profitRate ?? 0
        })),
        ByProfitStatus: (statsRes.ByProfitStatus ?? statsRes.byProfitStatus ?? []).map((item: any) => ({
          Status: item.Status ?? item.status ?? '',
          StatusName: item.StatusName ?? item.statusName ?? (item.Status === 'profit' || item.status === 'profit' ? '盈利' : '亏损'),
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
    // useApi 已经处理了响应格式，如果成功会返�?data（可能为 null），如果失败会抛出异�?    await api.post('/Investment/refresh-prices')
    // 等待一小段时间确保后端数据已保�?    await new Promise(resolve => setTimeout(resolve, 500))
    // 重新获取列表和统计数�?    await fetchList()
    lastRefreshTime.value = new Date()
    // 只在手动刷新时显示提示，自动刷新时不显示（避免打扰）
    if (!autoRefreshInterval.value || !autoRefreshEnabled.value) {
      success('价格刷新成功，数据已更新')
    } else {
      console.log('[自动刷新] 价格刷新成功')
    }
  } catch (e: unknown) {
    handleError(e, '刷新失败')
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
  // TODO: 实现交易记录功能
  const { info } = useNotification()
  info('交易功能开发中...')
}

const saveItem = async () => {
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  // 前端表单验证
  if (!form.value.code || !form.value.code.trim()) {
    warning('请填写代�?)
    return
  }
  
  if (!form.value.name || !form.value.name.trim()) {
    warning('请填写名�?)
    return
  }
  
  if (!form.value.type) {
    warning('请选择类型')
    return
  }
  
  if (!form.value.quantity || form.value.quantity <= 0) {
    warning('请填写持仓数量，必须大于0')
    return
  }
  
  if (!form.value.costPrice || form.value.costPrice <= 0) {
    warning('请填写成本价，必须大�?')
    return
  }
  
  if (!form.value.currentPrice || form.value.currentPrice <= 0) {
    warning('请填写当前价格，必须大于0')
    return
  }
  
  try {
    const payload: InvestmentRequest = {
      code: form.value.code.trim(),
      name: form.value.name.trim(),
      type: form.value.type,
      quantity: form.value.quantity,
      costPrice: form.value.costPrice,
      currentPrice: form.value.currentPrice, // 包含当前价格
      notes: form.value.notes || undefined
    }
    
    let response
    if (editingItem.value) {
      response = await api.put(`/Investment/${editingItem.value.id}`, payload)
    } else {
      response = await api.post('/Investment', payload)
    }

    // 检查返回的消息，如果是合并持仓的提示，显示特殊消息
    const message = response?.message || '保存成功'
    success(message)
    cancelEdit()
    fetchList()
  } catch (e: unknown) {
    handleError(e, '保存失败')
  }
}

const deleteItem = async (id: number) => {
  if (!confirm('确定要删除吗�?)) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    // 确保 ID 是数字类�?    const investmentId = Number(id)
    if (isNaN(investmentId) || investmentId <= 0) {
      throw new Error('无效的投资记�?ID')
    }
    
    const response = await api.delete(`/Investment/${investmentId}`)
    success('删除成功')
    fetchList()
  } catch (e: unknown) {
    console.error('删除失败:', e)
    handleError(e, '删除失败')
  }
}

const handleAddClick = () => {
  showCreateModal.value = true
  editingItem.value = null
  form.value = { code: '', name: '', type: 'stock', quantity: 0, costPrice: 0, currentPrice: 0, notes: '' }
  isAutoFilling.value = false
  // 重置快速估算输�?  inputMode.value = 'quick' // 默认使用快速估算模�?  quickInput.value = { totalAmount: 0, estimatedPrice: 0 }
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
      throw new Error('导出失败')
    }

    // 获取文件�?    const contentDisposition = response.headers.get('Content-Disposition')
    let fileName = `export_${Date.now()}.csv`
    if (contentDisposition) {
      const fileNameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/)
      if (fileNameMatch && fileNameMatch[1]) {
        fileName = fileNameMatch[1].replace(/['"]/g, '')
      }
    }

    // 下载文件
    const blob = await response.blob()
    const url_blob = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url_blob
    a.download = fileName
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
    window.URL.revokeObjectURL(url_blob)

    success('导出成功')
    showExportMenu.value = false
  } catch (err: any) {
    console.error('导出失败:', err)
    handleError(err)
    showError(err.message || '导出失败')
  }
}

// 根据代码自动识别类型
const autoDetectType = () => {
  const code = form.value.code.trim()
  if (!code) return
  
  // 基金代码通常�?6 位数字，且以 0�?�? 开�?  // 股票代码通常�?6 位数字，�?0�?�? 开�?  if (code.length === 6 && /^\d+$/.test(code)) {
    // 基金代码常见开头：00�?1�?5�?5�?6�?1�?2�?3�?4�?5�?6�?7�?8�?9
    if (code.startsWith('00') || code.startsWith('01') || code.startsWith('05') || 
        code.startsWith('15') || code.startsWith('16') || code.startsWith('51') || 
        code.startsWith('52') || code.startsWith('53') || code.startsWith('54') || 
        code.startsWith('55') || code.startsWith('56') || code.startsWith('57') || 
        code.startsWith('58') || code.startsWith('59')) {
      form.value.type = 'fund'
      
      // 检测到场外基金�?0�?1�?5开头），提示用户使用快速估算模�?      if (code.startsWith('00') || code.startsWith('01') || code.startsWith('05')) {
        const { info } = useNotification()
        // 延迟一下，避免和自动获取的提示冲突
        setTimeout(() => {
          info('检测到场外基金代码，建议使�?快速估�?模式，只需输入总投入金额即�?)
        }, 500)
      }
    } else if (code.startsWith('0') || code.startsWith('3') || code.startsWith('6')) {
      // A股股票代码：0开头（深市）�?开头（创业板）�?开头（沪市�?      form.value.type = 'stock'
    }
  }
}

const cancelEdit = () => {
  showCreateModal.value = false
  editingItem.value = null
  form.value = { code: '', name: '', type: 'stock', quantity: 0, costPrice: 0, currentPrice: 0, notes: '' }
  isAutoFilling.value = false
}

// 处理导入点击
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

  // 验证文件类型
  const allowedExtensions = ['.csv', '.xlsx', '.xls']
  const fileExtension = '.' + file.name.split('.').pop()?.toLowerCase()
  if (!allowedExtensions.includes(fileExtension)) {
    const { error } = useNotification()
    error('不支持的文件类型。请选择 CSV �?Excel 文件')
    return
  }

  // 显示导入模态框
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
      throw new Error(response.message || '导入失败')
    }

    const result = response.code === 0 ? response.data : response
    importResult.value = {
      successCount: result.successCount || 0,
      failCount: result.failCount || 0,
      errors: result.errors || []
    }

    if (result.successCount > 0) {
      success(`导入成功�?{result.successCount} 条记录`)
      // 刷新列表
      await fetchList()
    }

    if (result.failCount > 0) {
      showError(`导入失败�?{result.failCount} 条记录`)
    }
  } catch (err: any) {
    console.error('导入失败:', err)
    handleError(err)
    showError(err.message || '导入失败')
  } finally {
    importing.value = false
    // 清空文件输入
    if (fileInput.value) {
      fileInput.value.value = ''
    }
  }
}

const closeImportModal = () => {
  showImportModal.value = false
  importResult.value = null
}

// 自动获取名称和价�?const autoFillFromCode = async () => {
  if (!form.value.code || !form.value.type) {
    const { warning } = useNotification()
    warning('请先输入代码并选择类型')
    return
  }

  isAutoFilling.value = true
  const { success, warning } = useNotification()
  const { handleError } = useErrorHandler()

  try {
    // 确保代码�?位数�?    const code = form.value.code.trim().padStart(6, '0')
    const type = form.value.type
    
    const res = await api.get<any>(`/Investment/auto-fill?code=${encodeURIComponent(code)}&type=${encodeURIComponent(type)}`)
    
    // useApi 已经自动解包了响应，res 就是 data 部分
    // 注意：后端返回的�?camelCase (name, currentPrice)，不�?PascalCase (Name, CurrentPrice)
    if (res) {
      // 兼容两种格式：camelCase �?PascalCase
      const name = res.name || res.Name || ''
      const currentPrice = res.currentPrice || res.CurrentPrice || 0
      
      if (name) {
        form.value.name = name
      }
      if (currentPrice && currentPrice > 0) {
        form.value.currentPrice = currentPrice
        // 如果成本价为0，可以用当前价格作为参�?        if (form.value.costPrice === 0) {
          form.value.costPrice = currentPrice
        }
        success(`已自动获取：${name}，当前价�?¥${formatMoney(currentPrice)}`)
      } else {
        if (name) {
          success(`已自动获取名称：${name}，但无法获取当前价格（可能不在交易时间或该代码不在API支持范围内）`)
        } else {
          // 场外基金无法自动获取，提供友好的提示和下一步指�?          const isOTC = form.value.code.startsWith('00') || form.value.code.startsWith('01') || form.value.code.startsWith('05')
          if (isOTC && form.value.type === 'fund') {
            warning('这是场外基金，API无法自动获取。请手动填写名称和价格，或使�?快速估�?模式直接录入')
          } else {
            warning('无法从API获取信息。该代码可能不在东方财富API支持范围内，请手动填写名称和价格')
          }
        }
      }
    } else {
      warning('无法获取信息，请检查代码是否正�?)
    }
  } catch (e: unknown) {
    // 如果�?400 错误，提供更友好的提�?    if ((e as any)?.response?.status === 400 || (e as any)?.code === 400) {
      warning('请求参数错误，请检查代码和类型是否正确')
    } else {
      handleError(e, '自动获取失败')
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

// 格式化刷新时�?const formatRefreshTime = (time: Date | null) => {
  if (!time) return ''
  const now = new Date()
  const diff = Math.floor((now.getTime() - time.getTime()) / 1000) // �?  if (diff < 60) return `${diff}秒前`
  if (diff < 3600) return `${Math.floor(diff / 60)}分钟前`
  return time.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })
}

// 生成图表颜色
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

// ECharts 通用配置
const getCommonPieOption = () => {
  const theme = isDark.value ? darkTheme.value : lightTheme.value
  // 安全检查，如果 theme 未定义，使用默认�?  if (!theme) {
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
          return `${name}<br/>¥${value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} (${percent}%)`
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
        return `${name}<br/>¥${value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} (${percent}%)`
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

// 资产类型分布图表配置
const typeChartOption = computed(() => {
  if (!stats.value.ByType || stats.value.ByType.length === 0) {
    return {}
  }
  const theme = isDark.value ? darkTheme.value : lightTheme.value
  const data = stats.value.ByType.map((t: any) => ({
    name: t.TypeName || (t.Type === 'stock' ? '股票' : '基金'),
    value: t.TotalMarketValue || 0
  }))
  const colors = generateColors(data.length)
  
  return {
    ...getCommonPieOption(),
    backgroundColor: theme.backgroundColor,
    textStyle: {
      ...theme.textStyle,
      color: theme.textStyle.color // 确保文字颜色应用
    },
    color: colors,
    series: [{
      type: 'pie',
      radius: ['40%', '70%'], // 环形图，更美�?      avoidLabelOverlap: true,
      itemStyle: {
        borderRadius: 8,
        borderColor: isDark.value ? 'var(--color-gray-800)' : 'var(--color-bg-card)',
        borderWidth: 2
      },
      label: {
        show: true,
        color: theme.textStyle.color, // 使用主题文字颜色
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

// 盈亏状态分布图表配�?const profitStatusChartOption = computed(() => {
  if (!stats.value.ByProfitStatus || stats.value.ByProfitStatus.length === 0) {
    return {}
  }
  const theme = isDark.value ? darkTheme.value : lightTheme.value
  const data = stats.value.ByProfitStatus.map((s: any) => ({
    name: s.StatusName || (s.Status === 'profit' ? '盈利' : '亏损'),
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
      radius: ['40%', '70%'], // 环形�?      avoidLabelOverlap: true,
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
          return `${params.name}\n${params.value}个`
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

// 资产分布图表配置（Top 10�?const assetDistributionChartOption = computed(() => {
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
          return `${params.name}\n¥${params.value.toLocaleString('zh-CN')}`
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

// 收益排行图表配置
const profitRankChartOption = computed(() => {
  if (!stats.value.TopByProfit || stats.value.TopByProfit.length === 0) {
    return {}
  }
  const theme = isDark.value ? darkTheme.value : lightTheme.value
  const labels = stats.value.TopByProfit.map((p: any) => `${p.Name} (${p.Code})`)
  const data = stats.value.TopByProfit.map((p: any) => p.ProfitLoss || 0)
  const colors = data.map((d: number) => d >= 0 ? '#10B981' : '#EF4444')
  
  // 安全检�?  if (!theme) {
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
          return `${param.name}<br/>盈亏: ¥${param.value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
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
          formatter: (value: number) => '¥' + value.toLocaleString('zh-CN')
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
            return value >= 0 ? '+' : '' + '¥' + value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
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
        return `${param.name}<br/>盈亏: ¥${param.value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
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
        formatter: (value: number) => '¥' + value.toLocaleString('zh-CN')
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
          return value >= 0 ? '+' : '' + '¥' + value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
        }
      }
    }]
  }
})

// 自动刷新相关
const autoRefreshEnabled = ref(true) // 默认开启自动刷�?const autoRefreshInterval = ref<NodeJS.Timeout | null>(null)
const lastRefreshTime = ref<Date | null>(null)

// 自动刷新价格和数�?const startAutoRefresh = () => {
  // 清除旧的定时�?  if (autoRefreshInterval.value) {
    clearInterval(autoRefreshInterval.value)
  }
  
  // �?分钟自动刷新一次价�?  autoRefreshInterval.value = setInterval(async () => {
    if (!refreshingPrices.value) {
      console.log('[自动刷新] 开始自动刷新价�?..')
      await refreshPrices()
      lastRefreshTime.value = new Date()
    }
  }, 5 * 60 * 1000) // 5分钟
}

// 停止自动刷新
const stopAutoRefresh = () => {
  if (autoRefreshInterval.value) {
    clearInterval(autoRefreshInterval.value)
    autoRefreshInterval.value = null
  }
}

// 监听自动刷新开�?watch(autoRefreshEnabled, (enabled) => {
  if (enabled) {
    startAutoRefresh()
  } else {
    stopAutoRefresh()
  }
})

onMounted(() => {
  // 首次加载数据
  fetchList()
  
  // 页面加载后自动刷新一次价格（确保看到最新数据）
  setTimeout(async () => {
    if (autoRefreshEnabled.value) {
      console.log('[页面加载] 自动刷新价格...')
      await refreshPrices()
      lastRefreshTime.value = new Date()
      // 然后启动定时刷新
      startAutoRefresh()
    }
  }, 2000) // 延迟2秒，避免页面加载时立即请�?})

// 页面卸载时清理定时器
onUnmounted(() => {
  stopAutoRefresh()
})
</script>

<style scoped>
/* 页面容器 */
.asset-decision-panel {
  padding: 1.5rem;
  max-width: 1400px;
  margin: 0 auto;
}

/* 页面头部 */
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

/* 模块说明 */
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

/* 功能说明卡片 */
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

/* 交易记录导入 */
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

/* 确保模态框可见 */
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

/* 录入模式切换 */
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

/* 带单位的输入�?*/
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

/* 快速估算结�?*/
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

/* 统计卡片网格 */
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

/* 图表网格 */
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

/* 投资列表表格 */
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

/* 导入相关样式 */
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

/* 导出菜单样式 */
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

/* 自动刷新控件 */
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

