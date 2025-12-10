<template>
  <!-- 
    访客分析模块
    moduleId: analytics_dashboard
    
    如何在后台配置中控制这个模块的主题：
    1. 登录后台，进入主题管理页面
    2. 找到"访客分析模块"配置项
    3. 选择"跟随全局"或指定独立主题（如 "tech-blue"）
    4. 保存后，刷新页面即可看到效果
  -->
  <div :data-module-theme="moduleTheme || undefined">
    <div class="flex justify-between items-center mb-6">
      <div>
        <!-- 标题和描述：使用主题文字颜色，替换写死的 gray 和 dark: 前缀 -->
        <h1 class="text-2xl font-bold text-text-main">访客分析</h1>
        <p class="text-sm text-text-muted mt-1">
          查看网站访问统计和访客数据
        </p>
      </div>
      <!-- 按钮：使用 AppButton 或主题主色，替换写死的 bg-blue-600 -->
      <AppButton variant="primary" @click="refreshStats">
        刷新数据
      </AppButton>
    </div>

    <!-- 初始加载状态：只在首次加载时显示，避免闪烁 -->
    <template v-if="!initialLoadComplete">
      <AppCard class="mb-6">
        <div class="flex items-center justify-center py-12">
          <div class="text-center">
            <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary mb-4"></div>
            <p class="text-text-muted">正在加载数据...</p>
          </div>
        </div>
      </AppCard>
    </template>

    <!-- 数据提示和主要内容：只在加载完成后显示 -->
    <template v-else>
      <!-- 数据提示：只在确实没有数据时显示 -->
      <AppCard v-if="showNoDataAlert" class="mb-6 border-2 border-chart-tertiary/50 bg-chart-tertiary/10 p-4">
        <div class="flex items-start">
          <div class="flex-shrink-0">
            <svg class="h-5 w-5 text-chart-tertiary" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
            </svg>
          </div>
          <div class="ml-3 flex-1">
            <!-- 使用样式组合类简化代码 -->
            <h3 class="text-sm text-heading mb-2">暂无访客数据</h3>
            <div class="mt-2 text-sm text-body leading-relaxed">
              <p class="mb-2 font-medium">当前没有访客访问记录。可能的原因：</p>
              <ul class="list-disc list-inside mt-1 space-y-1 ml-2">
                <li>网站还没有访客访问</li>
                <!-- 使用 bg-code 样式组合类，替代多个类名 -->
                <li>访客数据存储在 <code class="bg-code">VisitLogs</code> 表中，请检查数据库</li>
                <li>如果使用代理或VPN，可能无法正确记录IP地址</li>
              </ul>
              <p class="mt-3 mb-2 font-medium">
                <strong class="text-body">提示：</strong>访问网站首页会自动记录访问数据。您可以：
              </p>
              <ul class="list-disc list-inside mt-1 space-y-1 ml-2">
                <li>打开网站首页，系统会自动记录您的访问</li>
                <li>点击"刷新数据"按钮更新统计数据</li>
                <li>检查浏览器控制台的日志信息</li>
              </ul>
            </div>
          </div>
        </div>
      </AppCard>

      <!-- 第一行：概览卡片区 -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <AppCard class="p-4">
        <div class="text-sm text-text-muted mb-1">今日浏览量</div>
        <div class="text-2xl font-bold text-primary">{{ overview.todayPv || 0 }}</div>
        <div class="text-xs text-text-muted mt-1">
          昨日: {{ overview.yesterdayPv || 0 }} | 总计: {{ overview.totalPv || 0 }}
        </div>
      </AppCard>
      <AppCard class="p-4">
        <div class="text-sm text-text-muted mb-1">今日访客数</div>
        <div class="text-2xl font-bold text-chart-secondary">{{ overview.todayUv || 0 }}</div>
        <div class="text-xs text-text-muted mt-1">
          昨日: {{ overview.yesterdayUv || 0 }} | 总计: {{ overview.totalUv || 0 }}
        </div>
      </AppCard>
      <AppCard class="p-4">
        <div class="text-sm text-text-muted mb-1">在线人数</div>
        <div class="text-2xl font-bold text-chart-tertiary">{{ overview.onlineUsers || 0 }}</div>
        <div class="text-xs text-text-muted mt-1">最近5分钟活跃</div>
      </AppCard>
      <AppCard class="p-4">
        <div class="text-sm text-text-muted mb-1">热门文章数</div>
        <div class="text-2xl font-bold text-chart-quinary">{{ overview.hotArticleCount || 0 }}</div>
        <div class="text-xs text-text-muted mt-1">访问次数 > 1</div>
      </AppCard>
    </div>

    <!-- 浏览量/访客数趋势图 -->
    <AppCard class="mb-6 p-6">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-bold text-text-main">浏览量/访客数趋势</h2>
        <div class="flex gap-2">
          <AppButton
            :variant="trendRange === '7d' ? 'primary' : 'secondary'"
            size="sm"
            @click="trendRange = '7d'; selectedRange = '7d'"
          >
            7天
          </AppButton>
          <AppButton
            :variant="trendRange === '30d' ? 'primary' : 'secondary'"
            size="sm"
            @click="trendRange = '30d'; selectedRange = '30d'"
          >
            30天
          </AppButton>
          <AppButton
            :variant="trendRange === '90d' ? 'primary' : 'secondary'"
            size="sm"
            @click="trendRange = '90d'; selectedRange = '90d'"
          >
            90天
          </AppButton>
        </div>
      </div>
      <div v-if="trendLoading" class="text-center py-8 text-text-muted">
        加载中...
      </div>
      <div v-else-if="hasTrendData && trendChartData.labels.length > 0" class="h-80">
        <Line :data="trendChartData" :options="trendChartOptions" />
      </div>
      <div v-else class="text-center py-8 text-text-muted">
        暂无趋势数据
      </div>
    </AppCard>

    <!-- 饼状图统计区域 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-4 gap-6 mb-6">
      <!-- 访问区域饼状图 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">访问区域分布</h2>
        <div v-if="hasRegionData && regionChartData.labels.length > 0" class="h-64">
          <component :is="Pie" v-if="Pie && chartLoaded" :data="regionChartData" :options="chartOptions" />
          <div v-else class="flex items-center justify-center h-full">
            <n-spin size="small" />
          </div>
        </div>
        <div v-else class="text-center text-text-muted py-8">暂无数据</div>
      </AppCard>

      <!-- 设备类型饼状图 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">设备类型分布</h2>
        <div v-if="hasDeviceData && deviceChartData.labels.length > 0" class="h-64">
          <component :is="Pie" v-if="Pie && chartLoaded" :data="deviceChartData" :options="chartOptions" />
          <div v-else class="flex items-center justify-center h-full">
            <n-spin size="small" />
          </div>
        </div>
        <div v-else class="text-center text-text-muted py-8">暂无数据</div>
      </AppCard>

      <!-- 浏览器饼状图 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">浏览器分布</h2>
        <div v-if="hasBrowserData && browserChartData.labels.length > 0" class="h-64">
          <component :is="Pie" v-if="Pie && chartLoaded" :data="browserChartData" :options="chartOptions" />
          <div v-else class="flex items-center justify-center h-full">
            <n-spin size="small" />
          </div>
        </div>
        <div v-else class="text-center text-text-muted py-8">暂无数据</div>
      </AppCard>

      <!-- 操作系统饼状图 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">操作系统分布</h2>
        <div v-if="hasOsData && osChartData.labels.length > 0" class="h-64">
          <component :is="Pie" v-if="Pie && chartLoaded" :data="osChartData" :options="chartOptions" />
          <div v-else class="flex items-center justify-center h-full">
            <n-spin size="small" />
          </div>
        </div>
        <div v-else class="text-center text-text-muted py-8">暂无数据</div>
      </AppCard>
    </div>

    <!-- 统计表格区域 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- 访问区域统计表格 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">访问区域统计</h2>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-bg-elevated">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">排名</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">国家/地区</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">访问次数</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">占比</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-border-subtle">
              <tr
                v-for="(region, index) in (regions.items || [])"
                :key="index"
                class="hover:bg-bg-elevated transition-colors"
              >
                <td class="px-4 py-3 text-text-main font-bold">{{ index + 1 }}</td>
                <td class="px-4 py-3 text-text-main">
                  {{ region.country || '未知' }}{{ region.province ? ' - ' + region.province : '' }}
                </td>
                <td class="px-4 py-3 text-text-main">{{ region.count || 0 }}</td>
                <td class="px-4 py-3 text-text-main">
                  {{ totalRegionCount > 0 ? ((region.count / totalRegionCount) * 100).toFixed(2) : '0.00' }}%
                </td>
              </tr>
              <tr v-if="!regions.items || regions.items.length === 0">
                <td colspan="4" class="px-4 py-8 text-center text-text-muted">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </AppCard>

      <!-- 设备类型统计表格 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">设备类型统计</h2>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-bg-elevated">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">设备类型</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">访问次数</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">占比</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-border-subtle">
              <tr
                v-for="(device, index) in (clientDistribution.devices || [])"
                :key="index"
                class="hover:bg-bg-elevated transition-colors"
              >
                <td class="px-4 py-3 text-text-main">{{ device.name || '未知' }}</td>
                <td class="px-4 py-3 text-text-main">{{ device.count || 0 }}</td>
                <td class="px-4 py-3 text-text-main">
                  {{ totalDeviceCount > 0 ? ((device.count / totalDeviceCount) * 100).toFixed(2) : '0.00' }}%
                </td>
              </tr>
              <tr v-if="!clientDistribution.devices || clientDistribution.devices.length === 0">
                <td colspan="3" class="px-4 py-8 text-center text-text-muted">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </AppCard>

      <!-- 浏览器统计表格 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">浏览器统计</h2>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-bg-elevated">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">排名</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">浏览器</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">访问次数</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">占比</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-border-subtle">
              <tr
                v-for="(browser, index) in (clientDistribution.browsers || [])"
                :key="index"
                class="hover:bg-bg-elevated transition-colors"
              >
                <td class="px-4 py-3 text-text-main font-bold">{{ index + 1 }}</td>
                <td class="px-4 py-3 text-text-main">{{ browser.name || '未知' }}</td>
                <td class="px-4 py-3 text-text-main">{{ browser.count || 0 }}</td>
                <td class="px-4 py-3 text-text-main">
                  {{ totalBrowserCount > 0 ? ((browser.count / totalBrowserCount) * 100).toFixed(2) : '0.00' }}%
                </td>
              </tr>
              <tr v-if="!clientDistribution.browsers || clientDistribution.browsers.length === 0">
                <td colspan="4" class="px-4 py-8 text-center text-text-muted">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </AppCard>

      <!-- 操作系统统计表格 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">操作系统统计</h2>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-bg-elevated">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">排名</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">操作系统</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">访问次数</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">占比</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-border-subtle">
              <tr
                v-for="(os, index) in (clientDistribution.os || [])"
                :key="index"
                class="hover:bg-bg-elevated transition-colors"
              >
                <td class="px-4 py-3 text-text-main font-bold">{{ index + 1 }}</td>
                <td class="px-4 py-3 text-text-main">{{ os.name || '未知' }}</td>
                <td class="px-4 py-3 text-text-main">{{ os.count || 0 }}</td>
                <td class="px-4 py-3 text-text-main">
                  {{ totalOsCount > 0 ? ((os.count / totalOsCount) * 100).toFixed(2) : '0.00' }}%
                </td>
              </tr>
              <tr v-if="!clientDistribution.os || clientDistribution.os.length === 0">
                <td colspan="4" class="px-4 py-8 text-center text-text-muted">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </AppCard>
    </div>

    <!-- 第三行：三列布局 -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
      <!-- 左列：Top 页面 + 来源分析 -->
      <div class="space-y-6">
        <!-- Top 页面列表 -->
        <AppCard class="p-6">
          <div class="flex justify-between items-center mb-4">
            <h2 class="text-lg font-bold text-text-main">热门页面</h2>
            <select v-model="selectedRange" class="text-sm px-2 py-1 rounded border border-border-subtle bg-bg-card text-text-main">
              <option value="today">今日</option>
              <option value="7d">7天</option>
              <option value="30d">30天</option>
              <option value="90d">90天</option>
            </select>
          </div>
          <div v-if="topPagesLoading" class="text-center py-4 text-text-muted">加载中...</div>
          <div v-else-if="topPages.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else class="space-y-2 max-h-64 overflow-y-auto">
            <div
              v-for="(page, index) in topPages.slice(0, 10)"
              :key="index"
              class="flex items-center justify-between p-3 bg-bg-elevated rounded hover:bg-bg-hover transition-colors"
            >
              <div class="flex-1 min-w-0">
                <div class="text-sm font-medium text-text-main truncate" :title="formatPageUrl(page.url)">
                  {{ formatPageUrl(page.url) }}
                </div>
                <div class="text-xs text-text-muted mt-1.5 flex items-center gap-3">
                  <span>浏览量: <span class="font-semibold text-primary">{{ page.pv }}</span></span>
                  <span>访客数: <span class="font-semibold text-chart-secondary">{{ page.uv }}</span></span>
                </div>
              </div>
              <div class="w-8 h-8 rounded-full bg-primary-soft flex items-center justify-center text-primary font-bold text-sm ml-3 flex-shrink-0">
                {{ index + 1 }}
              </div>
            </div>
          </div>
        </AppCard>

        <!-- 来源分析 -->
        <AppCard class="p-6">
          <h2 class="text-lg font-bold text-text-main mb-4">访问来源</h2>
          <div v-if="sourcesLoading" class="text-center py-4 text-text-muted">加载中...</div>
          <div v-else-if="!sources.items || sources.items.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else>
            <div class="h-48 mb-4">
              <component :is="Pie" v-if="Pie && chartLoaded && sourcesChartData && sourcesChartData.labels.length > 0" :data="sourcesChartData" :options="chartOptions" />
              <div v-else-if="!chartLoaded" class="flex items-center justify-center h-full">
                <n-spin size="small" />
              </div>
            </div>
            <div class="space-y-2">
              <div
                v-for="(item, index) in sources.items"
                :key="index"
                class="flex items-center justify-between p-2 bg-bg-elevated rounded"
              >
                <span class="text-sm text-text-main">{{ item.name }}</span>
                <span class="text-xs text-text-muted">{{ item.count }}</span>
              </div>
            </div>
          </div>
        </AppCard>
      </div>

      <!-- 中列：设备/浏览器/操作系统分布 -->
      <div class="space-y-6">
        <!-- 设备类型分布 -->
        <AppCard class="p-6">
          <h2 class="text-lg font-bold text-text-main mb-4">设备类型</h2>
          <div v-if="clientDistributionLoading" class="text-center py-4 text-text-muted">加载中...</div>
          <div v-else-if="!clientDistribution.devices || clientDistribution.devices.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else class="h-48">
            <component :is="Pie" v-if="Pie && chartLoaded && deviceChartData && deviceChartData.labels.length > 0" :data="deviceChartData" :options="chartOptions" />
            <div v-else-if="!chartLoaded" class="flex items-center justify-center h-full">
              <n-spin size="small" />
            </div>
          </div>
        </AppCard>

        <!-- 浏览器分布 -->
        <AppCard class="p-6">
          <h2 class="text-lg font-bold text-text-main mb-4">浏览器</h2>
          <div v-if="clientDistributionLoading" class="text-center py-4 text-text-muted">加载中...</div>
          <div v-else-if="!clientDistribution.browsers || clientDistribution.browsers.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else class="h-48">
            <component :is="Pie" v-if="Pie && chartLoaded && browserChartData && browserChartData.labels.length > 0" :data="browserChartData" :options="chartOptions" />
            <div v-else-if="!chartLoaded" class="flex items-center justify-center h-full">
              <n-spin size="small" />
            </div>
          </div>
        </AppCard>

        <!-- 操作系统分布 -->
        <AppCard class="p-6">
          <h2 class="text-lg font-bold text-text-main mb-4">操作系统</h2>
          <div v-if="clientDistributionLoading" class="text-center py-4 text-text-muted">加载中...</div>
          <div v-else-if="!clientDistribution.os || clientDistribution.os.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else class="h-48">
            <component :is="Pie" v-if="Pie && chartLoaded && osChartData && osChartData.labels.length > 0" :data="osChartData" :options="chartOptions" />
            <div v-else-if="!chartLoaded" class="flex items-center justify-center h-full">
              <n-spin size="small" />
            </div>
          </div>
        </AppCard>
      </div>

      <!-- 右列：地区分布 + 行为路径 -->
      <div class="space-y-6">
        <!-- 地区分布 -->
        <AppCard class="p-6">
          <h2 class="text-lg font-bold text-text-main mb-4">地区分布</h2>
          <div v-if="regionsLoading" class="text-center py-4 text-text-muted">加载中...</div>
          <div v-else-if="!regions.items || regions.items.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else class="space-y-2 max-h-64 overflow-y-auto">
            <div
              v-for="(region, index) in (regions.items || []).slice(0, 10)"
              :key="index"
              class="flex items-center justify-between p-2 bg-bg-elevated rounded"
            >
              <div class="flex-1">
                <div class="text-sm text-text-main">{{ region.country }}{{ region.province ? ' - ' + region.province : '' }}</div>
              </div>
              <span class="text-xs text-text-muted">{{ region.count }}</span>
            </div>
          </div>
        </AppCard>

        <!-- 行为路径 -->
        <AppCard class="p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-bold text-text-main">访问路径</h2>
            <span class="text-xs text-text-muted">了解用户浏览路径</span>
          </div>
          <div v-if="pageFlowLoading" class="text-center py-4 text-text-muted">加载中...</div>
          <div v-else-if="!pageFlow.edges || pageFlow.edges.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else class="space-y-2 max-h-64 overflow-y-auto">
            <div
              v-for="(edge, index) in pageFlow.edges.slice(0, 10)"
              :key="index"
              class="p-3 bg-bg-elevated rounded text-sm hover:bg-bg-hover transition-colors"
            >
              <div class="flex items-center gap-2 text-text-main">
                <span class="font-medium text-primary">{{ formatPathName(edge.from) }}</span>
                <span class="text-text-muted">→</span>
                <span class="font-medium text-primary">{{ formatPathName(edge.to) }}</span>
              </div>
              <div class="text-xs text-text-muted mt-1.5">
                <span class="font-semibold text-text-main">{{ edge.count }}</span> 次访问
              </div>
            </div>
          </div>
        </AppCard>
      </div>
    </div>

    <!-- 搜索关键词（单独一行） -->
    <AppCard class="mb-6 p-6">
      <h2 class="text-lg font-bold text-text-main mb-4">搜索关键词</h2>
      <div v-if="searchKeywordsLoading" class="text-center py-4 text-text-muted">加载中...</div>
      <div v-else-if="searchKeywords.length === 0" class="text-center py-4 text-text-muted">暂无搜索数据</div>
      <div v-else class="flex flex-wrap gap-2">
        <span
          v-for="(keyword, index) in searchKeywords"
          :key="index"
          class="inline-flex items-center px-3 py-1 rounded-full text-sm bg-primary-soft text-primary"
        >
          {{ keyword.keyword }} ({{ keyword.count }})
        </span>
      </div>
    </AppCard>

    <!-- 访客列表 -->
    <AppCard class="mt-6 p-6">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-bold text-text-main">访客列表</h2>
        <div class="flex items-center gap-4">
          <label class="flex items-center gap-2 text-sm text-text-muted">
            <input
              type="checkbox"
              v-model="onlineOnly"
              @change="fetchVisitors"
              class="rounded"
            />
            仅显示在线访客
          </label>
          <AppButton variant="secondary" size="sm" @click="fetchVisitors">
            刷新列表
          </AppButton>
        </div>
      </div>

      <div v-if="visitorsLoading" class="text-center py-8 text-text-muted">
        加载中...
      </div>
      <div v-else-if="visitors.length === 0" class="text-center py-8 text-text-muted">
        暂无访客数据
      </div>
      <div v-else class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead class="bg-bg-elevated">
            <tr>
              <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">访客ID</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">IP地址</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">地理位置</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">设备信息</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">当前页面</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">浏览量</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">最后活跃</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase">状态</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-border-subtle">
            <tr
              v-for="visitor in visitors"
              :key="visitor.id || visitor.Id"
              class="hover:bg-bg-elevated transition-colors"
            >
              <!-- 修复访客列表 IP 一直显示未知的问题：后端返回的是小写字段名 -->
              <td class="px-4 py-3 text-text-main font-mono text-xs">
                {{ (visitor.visitorId || visitor.VisitorId)?.substring(0, 8) }}...
              </td>
              <!-- 修复访客列表 IP 一直显示未知的问题：后端返回的是小写字段名 ip，不是 Ip -->
              <td class="px-4 py-3 text-text-main font-mono text-xs">
                {{ (visitor.ip || visitor.Ip) && (visitor.ip || visitor.Ip) !== '-' ? (visitor.ip || visitor.Ip) : '未知' }}
              </td>
              <!-- 修复访客列表 IP 一直显示未知的问题：后端返回的是小写字段名 -->
              <td class="px-4 py-3 text-text-main">
                <div class="text-xs">
                  <div v-if="visitor.country || visitor.Country">{{ visitor.country || visitor.Country }}</div>
                  <div v-if="visitor.region || visitor.Region" class="text-text-muted">{{ visitor.region || visitor.Region }}</div>
                  <div v-if="visitor.city || visitor.City" class="text-text-muted">{{ visitor.city || visitor.City }}</div>
                  <div v-if="!(visitor.country || visitor.Country) && !(visitor.region || visitor.Region) && !(visitor.city || visitor.City)" class="text-text-disabled">未知</div>
                </div>
              </td>
              <!-- 修复访客列表 IP 一直显示未知的问题：后端返回的是小写字段名 -->
              <td class="px-4 py-3 text-text-main">
                <div class="text-xs">
                  <div>{{ (visitor.deviceType || visitor.DeviceType) && (visitor.deviceType || visitor.DeviceType) !== 'unknown' ? (visitor.deviceType || visitor.DeviceType) : '-' }}</div>
                  <div class="text-text-muted">
                    {{ (visitor.browser || visitor.Browser) && (visitor.browser || visitor.Browser) !== 'unknown' ? (visitor.browser || visitor.Browser) : '-' }} / 
                    {{ (visitor.os || visitor.Os) && (visitor.os || visitor.Os) !== 'unknown' ? (visitor.os || visitor.Os) : '-' }}
                  </div>
                </div>
              </td>
              <!-- 修复访客列表 IP 一直显示未知的问题：后端返回的是小写字段名 -->
              <!-- 优化路径显示：添加图标和更好的格式 -->
              <td class="px-4 py-3 text-text-main">
                <div class="flex items-center gap-1.5">
                  <!-- 根据路径类型显示不同图标 -->
                  <span class="text-text-muted text-xs">
                    <svg v-if="(visitor.path || visitor.Path) === '/'" class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    <svg v-else-if="(visitor.path || visitor.Path)?.includes('/admin')" class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                    </svg>
                    <svg v-else-if="(visitor.path || visitor.Path)?.includes('/blog') || (visitor.path || visitor.Path)?.includes('/article')" class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z" />
                    </svg>
                    <svg v-else class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                    </svg>
                  </span>
                  <div class="flex-1 min-w-0">
                    <div class="text-xs font-medium truncate" :title="(visitor.path || visitor.Path) || '/'">
                      {{ formatPathName((visitor.path || visitor.Path) || '/') }}
                    </div>
                    <!-- 显示原始路径（如果格式化后的名称与原始路径不同） -->
                    <div v-if="(visitor.path || visitor.Path) && formatPathName((visitor.path || visitor.Path) || '/') !== (visitor.path || visitor.Path)?.replace(/^\//, '')" 
                         class="text-xs text-text-muted truncate mt-0.5" 
                         :title="(visitor.path || visitor.Path) || '/'">
                      {{ (visitor.path || visitor.Path)?.replace(/^\//, '') || '/' }}
                    </div>
                  </div>
                </div>
                <div v-if="visitor.searchKeyword || visitor.SearchKeyword" class="text-xs text-primary mt-1.5 flex items-center gap-1">
                  <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                  搜索: {{ visitor.searchKeyword || visitor.SearchKeyword }}
                </div>
              </td>
              <!-- 修复访客列表 IP 一直显示未知的问题：后端返回的是小写字段名 -->
              <td class="px-4 py-3 text-text-main text-center">
                {{ ((visitor.pageViews || visitor.PageViews) || 0) > 0 ? (visitor.pageViews || visitor.PageViews) : 1 }}
              </td>
              <!-- 修复访客列表 IP 一直显示未知的问题：后端返回的是小写字段名 -->
              <td class="px-4 py-3 text-text-main text-xs">
                {{ (visitor.updatedAt || visitor.UpdatedAt) ? formatTime(visitor.updatedAt || visitor.UpdatedAt) : '-' }}
              </td>
              <!-- 修复访客列表 IP 一直显示未知的问题：后端返回的是小写字段名 -->
              <td class="px-4 py-3">
                <span
                  v-if="(visitor.isOnline || visitor.IsOnline) === true"
                  class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-chart-secondary/20 text-chart-secondary"
                >
                  <span class="w-1.5 h-1.5 bg-chart-secondary rounded-full mr-1"></span>
                  在线
                </span>
                <span
                  v-else
                  class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-bg-elevated text-text-muted"
                >
                  离线
                </span>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- 分页 -->
        <div v-if="visitorsTotal > pageSize" class="mt-4 flex items-center justify-between">
          <div class="text-sm text-text-muted">
            共 {{ visitorsTotal }} 条记录
          </div>
          <div class="flex gap-2">
            <AppButton
              variant="secondary"
              size="sm"
              @click="changePage(visitorsPage - 1)"
              :disabled="visitorsPage <= 1"
            >
              上一页
            </AppButton>
            <span class="px-3 py-1 text-sm text-text-main">
              第 {{ visitorsPage }} / {{ Math.ceil(visitorsTotal / pageSize) }} 页
            </span>
            <AppButton
              variant="secondary"
              size="sm"
              @click="changePage(visitorsPage + 1)"
              :disabled="visitorsPage >= Math.ceil(visitorsTotal / pageSize)"
            >
              下一页
            </AppButton>
          </div>
        </div>
      </div>
    </AppCard>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted, defineAsyncComponent } from 'vue'

// 延迟加载 Chart.js，减少初始包大小
let ChartJS: any = null
let Pie: any = null
let Line: any = null
let chartLoaded = false

const loadChartJS = async () => {
  if (chartLoaded) return
  
  const chartModule = await import(/* webpackChunkName: "chartjs" */ 'chart.js')
  const vueChartModule = await import(/* webpackChunkName: "vue-chartjs" */ 'vue-chartjs')
  
  ChartJS = chartModule.Chart
  const {
    ArcElement,
    Tooltip,
    Legend,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement
  } = chartModule
  
  ChartJS.register(
    ArcElement,
    Tooltip,
    Legend,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement
  )
  
  Pie = vueChartModule.Pie
  Line = vueChartModule.Line
  chartLoaded = true
}

// 在组件挂载时加载 Chart.js
onMounted(() => {
  loadChartJS()
})

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // 禁用 SSR，避免 Naive UI 组件在服务端渲染时出错
})

// 注意：AppButton 组件应该在 Nuxt 3 中自动导入
// 如果出现 "Failed to resolve component: AppButton" 错误，
// 可能需要检查 components/ui/AppButton.vue 是否存在
// 或者手动导入：import AppButton from '~/components/ui/AppButton.vue'

// 使用模块主题 composable
const { moduleTheme } = useModuleTheme('analytics_dashboard')

const api = useApi()

const stats = ref<any>({
  Today: { Pv: 0, Uv: 0 },
  Yesterday: { Pv: 0, Uv: 0 },
  OnlineCount: 0,
  TopArticles: [],
  RegionStats: [],
  SearchSources: [],
  DeviceStats: [],
  BrowserStats: [],
  OsStats: []
})

const visitors = ref<any[]>([])
const visitorsLoading = ref(false)
const visitorsPage = ref(1)
const visitorsTotal = ref(0)
const pageSize = ref(20)
const onlineOnly = ref(false) // 默认不勾选，显示所有访客

// 初始加载完成标志，用于避免页面刚进入时显示"暂无数据"提示
const initialLoadComplete = ref(false)


// 统一时间范围选择
const selectedRange = ref<'today' | '7d' | '30d' | '90d'>('7d')

// 概览数据
const overview = ref<any>({
  todayPv: 0,
  todayUv: 0,
  yesterdayPv: 0,
  yesterdayUv: 0,
  totalPv: 0,
  totalUv: 0,
  onlineUsers: 0,
  hotArticleCount: 0
})

// 趋势图相关（与 selectedRange 同步）
const trendRange = ref<'7d' | '30d' | '90d'>('7d')
const trendData = ref<any>({ points: [] })
const trendLoading = ref(false)

// Top 页面数据
const topPages = ref<any[]>([])
const topPagesLoading = ref(false)

// 来源分析数据
const sources = ref<any>({
  total: 0,
  items: [],
  topReferrers: []
})
const sourcesLoading = ref(false)

// 搜索关键词数据
const searchKeywords = ref<any[]>([])
const searchKeywordsLoading = ref(false)

// 地区分布数据
const regions = ref<any>({ items: [] })
const regionsLoading = ref(false)

// 客户端分布数据
const clientDistribution = ref<any>({
  devices: [],
  browsers: [],
  os: []
})
const clientDistributionLoading = ref(false)

// 行为路径数据
const pageFlow = ref<any>({
  nodes: [],
  edges: []
})
const pageFlowLoading = ref(false)

// 图表配置
// 获取主题变量中的颜色值
const getThemeColor = (cssVar: string): string => {
  if (!process.client) return '#3b82f6' // 默认值
  return getComputedStyle(document.documentElement).getPropertyValue(cssVar).trim() || '#3b82f6'
}

const chartOptions = computed(() => {
  // 使用主题变量获取颜色
  const textColor = getThemeColor('--color-text-main')
  const legendColor = getThemeColor('--color-text-muted')
  const tooltipBg = getThemeColor('--color-bg-card')
  const tooltipText = getThemeColor('--color-text-main')
  const tooltipBorder = getThemeColor('--color-border-subtle')
  
  return {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom' as const,
        labels: {
          padding: 15,
          usePointStyle: true,
          color: legendColor,
          font: {
            size: 13,
            weight: 'normal' as const
          }
        }
      },
      tooltip: {
        backgroundColor: tooltipBg,
        borderColor: tooltipBorder,
        borderWidth: 1,
        titleColor: tooltipText,
        bodyColor: tooltipText,
        padding: 12,
        callbacks: {
          label: function(context: any) {
            const label = context.label || ''
            const value = context.parsed || 0
            const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0)
            const percentage = ((value / total) * 100).toFixed(2)
            return `${label}: ${value} (${percentage}%)`
          }
        }
      }
    }
  }
})

// 标准配色（与图片中的配色一致）
const standardColors = [
  '#3b82f6', // 蓝色
  '#10b981', // 绿色
  '#f59e0b', // 橙色
  '#ef4444', // 红色
  '#8b5cf6', // 紫色
  '#ec4899', // 粉色
  '#06b6d4', // 青色
  '#84cc16', // 黄绿色
  '#f97316', // 深橙色
  '#6366f1'  // 靛蓝色
]

const generateColors = (count: number): string[] => {
  // 如果颜色不够，循环使用标准配色
  const colors: string[] = []
  for (let i = 0; i < count; i++) {
    colors.push(standardColors[i % standardColors.length])
  }
  return colors
}

// 访问区域图表数据（使用新的 regions 接口）
const regionChartData = computed(() => {
  if (!regions.value || !regions.value.items || regions.value.items.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = regions.value.items.map((r: any) => {
    const country = r.country || '未知'
    const province = r.province || ''
    return province ? `${country} - ${province}` : country
  })
  const data = regions.value.items.map((r: any) => r.count || 0)
  const colors = generateColors(labels.length)
  
  return {
    labels,
    datasets: [{
      data,
      backgroundColor: colors,
      borderColor: '#ffffff',
      borderWidth: 2
    }]
  }
})

// 来源分析图表数据
const sourcesChartData = computed(() => {
  if (!sources.value.items || sources.value.items.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = sources.value.items.map((item: any) => item.name)
  const data = sources.value.items.map((item: any) => item.count)
  const colors = generateColors(labels.length)
  
  return {
    labels,
    datasets: [{
      data,
      backgroundColor: colors,
      borderColor: '#ffffff',
      borderWidth: 2
    }]
  }
})

// 设备类型图表数据（使用新的 client-distribution 接口）
const deviceChartData = computed(() => {
  if (!clientDistribution.value || !clientDistribution.value.devices || clientDistribution.value.devices.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = clientDistribution.value.devices.map((d: any) => d.name || '未知')
  const data = clientDistribution.value.devices.map((d: any) => d.count || 0)
  const colors = generateColors(labels.length)
  
  return {
    labels,
    datasets: [{
      data,
      backgroundColor: colors,
      borderColor: '#ffffff',
      borderWidth: 2
    }]
  }
})

// 浏览器图表数据（使用新的 client-distribution 接口）
const browserChartData = computed(() => {
  if (!clientDistribution.value || !clientDistribution.value.browsers || clientDistribution.value.browsers.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = clientDistribution.value.browsers.map((b: any) => b.name || '未知')
  const data = clientDistribution.value.browsers.map((b: any) => b.count || 0)
  const colors = generateColors(labels.length)
  
  return {
    labels,
    datasets: [{
      data,
      backgroundColor: colors,
      borderColor: '#ffffff',
      borderWidth: 2
    }]
  }
})

// 操作系统图表数据（使用新的 client-distribution 接口）
const osChartData = computed(() => {
  if (!clientDistribution.value || !clientDistribution.value.os || clientDistribution.value.os.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = clientDistribution.value.os.map((o: any) => o.name || '未知')
  const data = clientDistribution.value.os.map((o: any) => o.count || 0)
  const colors = generateColors(labels.length)
  
  return {
    labels,
    datasets: [{
      data,
      backgroundColor: colors,
      borderColor: '#ffffff',
      borderWidth: 2
    }]
  }
})

// 趋势图数据（使用新的 trend 接口）
const trendChartData = computed(() => {
  // 后端返回的是 points（小写），不是 Points
  const points = trendData.value?.points || trendData.value?.Points || []
  if (!trendData.value || points.length === 0) {
    return { labels: [], datasets: [] }
  }
  // 辅助函数：格式化日期标签
  const formatDateLabel = (dateStr: string, fallbackIndex: number): string => {
    if (!dateStr || dateStr.trim() === '') {
      return `第${fallbackIndex + 1}天`
    }
    
    // 尝试解析 yyyy-MM-dd 格式
    const dateParts = dateStr.split('-')
    if (dateParts.length >= 3) {
      const year = parseInt(dateParts[0], 10)
      const month = parseInt(dateParts[1], 10)
      const day = parseInt(dateParts[2], 10)
      
      // 验证日期有效性
      if (!isNaN(year) && !isNaN(month) && !isNaN(day) && 
          year > 0 && month >= 1 && month <= 12 && day >= 1 && day <= 31) {
        // 格式化：M月D日
        return `${month}月${day}日`
      }
    }
    
    // 如果解析失败，尝试使用 Date 构造函数
    const date = new Date(dateStr)
    if (!isNaN(date.getTime()) && date.getFullYear() > 1970) {
      // 使用 Date 对象格式化，确保不会返回 NaN
      const month = date.getMonth() + 1
      const day = date.getDate()
      if (!isNaN(month) && !isNaN(day)) {
        return `${month}月${day}日`
      }
    }
    
    // 如果所有方法都失败，返回索引标签
    return `第${fallbackIndex + 1}天`
  }

  const labels = points.map((p: any, index: number) => {
    // 兼容 date（小写）和 Date（大写）
    const dateStr = p.date || p.Date || ''
    if (!dateStr || dateStr.trim() === '') {
      return `第${index + 1}天`
    }
    
    try {
      // 格式化日期显示
      if (dateStr.includes(' ')) {
        // 包含时间（小时粒度），格式：yyyy-MM-dd HH:mm
        const parts = dateStr.split(' ')
        if (parts.length >= 2 && parts[1]) {
          // 只显示时间部分（HH:mm）
          const timePart = parts[1]
          const timeMatch = timePart.match(/^(\d{1,2}):(\d{2})/)
          if (timeMatch) {
            return timeMatch[0] // 返回 "HH:mm" 格式
          }
        }
        // 如果时间部分解析失败，尝试解析日期部分
        const datePart = parts[0]
        return formatDateLabel(datePart, index)
      } else {
        // 只有日期，格式：yyyy-MM-dd
        return formatDateLabel(dateStr, index)
      }
    } catch (e) {
      // 如果解析出错，返回索引标签
      console.warn('日期格式化失败:', dateStr, e)
      return `第${index + 1}天`
    }
  })

  // 使用霓虹色板
  // 使用标准配色
  const pvColor = '#06b6d4' // 青色
  const uvColor = '#10b981' // 绿色

  return {
    labels,
    datasets: [
      {
        label: '浏览量',
        data: points.map((p: any) => p.pv || p.Pv || 0),
        borderColor: pvColor,
        backgroundColor: (() => {
          // 创建渐变填充（上深下浅）
          if (!process.client) return pvColor + '33'
          const canvas = document.createElement('canvas')
          const ctx = canvas.getContext('2d')
          if (!ctx) return pvColor + '33'
          const gradient = ctx.createLinearGradient(0, 0, 0, 400)
          gradient.addColorStop(0, pvColor + '55') // 33% 透明度
          gradient.addColorStop(1, 'rgba(15,23,42,0.0)')
          return gradient
        })(),
        borderWidth: 3,
        tension: 0.4,
        fill: true,
        pointRadius: 6,
        pointHoverRadius: 8,
        pointBackgroundColor: pvColor,
        pointBorderColor: pvColor,
        pointBorderWidth: 0,
        // 添加阴影效果（霓虹发光）
        shadowOffsetX: 0,
        shadowOffsetY: 0,
        shadowBlur: 12,
        shadowColor: pvColor + 'aa'
      },
      {
        label: '访客数',
        data: points.map((p: any) => p.uv || p.Uv || 0),
        borderColor: uvColor,
        backgroundColor: (() => {
          // 创建渐变填充（上深下浅）
          if (!process.client) return uvColor + '33'
          const canvas = document.createElement('canvas')
          const ctx = canvas.getContext('2d')
          if (!ctx) return uvColor + '33'
          const gradient = ctx.createLinearGradient(0, 0, 0, 400)
          gradient.addColorStop(0, uvColor + '55') // 33% 透明度
          gradient.addColorStop(1, 'rgba(15,23,42,0.0)')
          return gradient
        })(),
        borderWidth: 3,
        tension: 0.4,
        fill: true,
        pointRadius: 6,
        pointHoverRadius: 8,
        pointBackgroundColor: uvColor,
        pointBorderColor: uvColor,
        pointBorderWidth: 0,
        // 添加阴影效果（霓虹发光）
        shadowOffsetX: 0,
        shadowOffsetY: 0,
        shadowBlur: 12,
        shadowColor: uvColor + 'aa'
      }
    ]
  }
})

// 趋势图配置
const trendChartOptions = computed(() => {
  const textColor = getThemeColor('--color-text-main')
  const legendColor = getThemeColor('--color-text-muted')
  const gridColor = getThemeColor('--color-border-subtle')
  const tooltipBg = getThemeColor('--color-bg-card')
  const tooltipText = getThemeColor('--color-text-main')
  const tooltipBorder = getThemeColor('--color-border-subtle')

  return {
    responsive: true,
    maintainAspectRatio: false,
    interaction: {
      mode: 'index' as const,
      intersect: false
    },
    plugins: {
      legend: {
        display: true,
        position: 'top' as const,
        labels: {
          color: legendColor,
          padding: 15,
          usePointStyle: true,
          font: {
            size: 13
          }
        }
      },
      tooltip: {
        backgroundColor: tooltipBg,
        borderColor: tooltipBorder,
        borderWidth: 1,
        titleColor: tooltipText,
        bodyColor: tooltipText,
        padding: 12,
        callbacks: {
          label: function(context: any) {
            const label = context.dataset.label || ''
            const value = context.parsed.y || 0
            return `${label}: ${value}`
          }
        }
      }
    },
    scales: {
      x: {
        grid: {
          display: false // 霓虹风格：隐藏 X 轴网格
        },
        ticks: {
          color: getThemeColor('--color-text-muted'),
          maxRotation: 45,
          minRotation: 0,
          font: {
            size: 11
          }
        },
        border: {
          display: false // 隐藏轴线
        }
      },
      y: {
        beginAtZero: true,
        grid: {
          color: 'rgba(148, 163, 184, 0.18)', // 弱网格
          display: true,
          lineWidth: 1,
          drawBorder: false
        },
        ticks: {
          color: getThemeColor('--color-text-muted'),
          stepSize: 1,
          font: {
            size: 11
          }
        },
        border: {
          display: false // 隐藏轴线
        }
      }
    }
  }
})

// 数据存在性判断（用于显示"暂无数据"）
const hasTrendData = computed(() => {
  const points = trendData.value?.points || trendData.value?.Points || []
  return points.length > 0
})

const hasRegionData = computed(() => {
  return (regions.value?.items?.length ?? 0) > 0
})

const hasDeviceData = computed(() => {
  return (clientDistribution.value?.devices?.length ?? 0) > 0
})

const hasBrowserData = computed(() => {
  return (clientDistribution.value?.browsers?.length ?? 0) > 0
})

const hasOsData = computed(() => {
  return (clientDistribution.value?.os?.length ?? 0) > 0
})

const hasClientDistributionData = computed(() => {
  return hasDeviceData.value || hasBrowserData.value || hasOsData.value
})

// 计算总数用于百分比
const totalRegionCount = computed(() => {
  if (!regions.value?.items || regions.value.items.length === 0) return 1
  return regions.value.items.reduce((sum: number, r: any) => sum + (r.count || 0), 0)
})

const totalDeviceCount = computed(() => {
  if (!clientDistribution.value?.devices || clientDistribution.value.devices.length === 0) return 1
  return clientDistribution.value.devices.reduce((sum: number, d: any) => sum + (d.count || 0), 0)
})

const totalBrowserCount = computed(() => {
  if (!clientDistribution.value?.browsers || clientDistribution.value.browsers.length === 0) return 1
  return clientDistribution.value.browsers.reduce((sum: number, b: any) => sum + (b.count || 0), 0)
})

const totalOsCount = computed(() => {
  if (!clientDistribution.value?.os || clientDistribution.value.os.length === 0) return 1
  return clientDistribution.value.os.reduce((sum: number, o: any) => sum + (o.count || 0), 0)
})

// 判断是否显示"暂无访客数据"提示（只有所有数据都为空时才显示）
const showNoDataAlert = computed(() => {
  // 如果初始加载未完成，不显示提示，避免闪烁
  if (!initialLoadComplete.value) {
    return false
  }
  
  // 如果数据还在加载中，不显示黄框，避免闪烁
  if (statsLoading.value || topPagesLoading.value || trendLoading.value || 
      regionsLoading.value || clientDistributionLoading.value || 
      visitorsLoading.value || pageFlowLoading.value || 
      sourcesLoading.value || searchKeywordsLoading.value) {
    return false
  }

  // 检查是否有任何数据
  const hasOverviewData = (overview.value?.todayPv ?? 0) > 0 || (overview.value?.todayUv ?? 0) > 0 ||
                          (overview.value?.totalPv ?? 0) > 0 || (overview.value?.totalUv ?? 0) > 0
  const hasAnyData = hasOverviewData || hasTrendData.value || hasRegionData.value || 
                     hasClientDistributionData.value || (topPages.value?.length ?? 0) > 0 || 
                     (sources.value?.items?.length ?? 0) > 0 || (searchKeywords.value?.length ?? 0) > 0 ||
                     (pageFlow.value?.edges?.length ?? 0) > 0 || (visitors.value?.length ?? 0) > 0

  return !hasAnyData
})

const statsLoading = ref(false)
const statsError = ref<string | null>(null)
const rateLimitRetryCount = ref(0)

const fetchStats = async () => {
  // 如果正在加载，跳过本次请求
  if (statsLoading.value) {
    return
  }
  
  try {
    statsLoading.value = true
    statsError.value = null
    const res = await api.get<any>('/Analytics/stats')
    
    // 成功获取数据，重置重试计数
    rateLimitRetryCount.value = 0
    
    if (res) {
      // 确保所有字段都有默认值
      stats.value = {
        Today: res.Today || { Pv: 0, Uv: 0 },
        Yesterday: res.Yesterday || { Pv: 0, Uv: 0 },
        OnlineCount: res.OnlineCount || 0,
        TopArticles: res.TopArticles || [],
        RegionStats: res.RegionStats || [],
        SearchSources: res.SearchSources || [],
        DeviceStats: res.DeviceStats || [],
        BrowserStats: res.BrowserStats || [],
        OsStats: res.OsStats || []
      }
    }
  } catch (e: any) {
    // 处理 429 速率限制错误
    if (e.response?.status === 429) {
      rateLimitRetryCount.value++
      statsError.value = '请求过于频繁，请稍后再试'
      
      // 如果连续遇到速率限制，停止自动刷新
      if (rateLimitRetryCount.value >= 3) {
        if (autoRefreshInterval.value) {
          clearInterval(autoRefreshInterval.value)
          autoRefreshInterval.value = null
        }
        if (process.client) {
          alert('请求过于频繁，已自动停止刷新。请稍后再试或手动刷新。')
        }
        return
      }
    } else {
      // 其他错误，显示提示但不阻止后续请求
      statsError.value = e.message || '获取数据失败'
    }
  } finally {
    statsLoading.value = false
  }
}

const fetchVisitors = async () => {
  try {
    visitorsLoading.value = true
    
    const res = await api.get<any>('/Analytics/visitors', {
      params: {
        page: visitorsPage.value,
        pageSize: pageSize.value,
        onlineOnly: onlineOnly.value
      }
    })
    
    if (res) {
      // useApi 已经提取了 data 字段，所以 res 应该是 { total: 39, page: 1, pageSize: 20, visitors: [...] }
      // 注意：后端返回的是小写的 "visitors" 和 "total"，优先使用小写
      const visitorsData = res.visitors || res.Visitors || (Array.isArray(res) ? res : [])
      const totalData = res.total ?? res.Total ?? (Array.isArray(res) ? res.length : 0)
      
      visitors.value = Array.isArray(visitorsData) ? visitorsData : []
      visitorsTotal.value = totalData
    }
  } catch (e: any) {
    // 显示错误提示
    if (process.client) {
      alert(`获取访客列表失败: ${e?.message || '未知错误'}\n\n请检查：\n1. 是否已登录管理员账号\n2. 后端服务是否正常运行\n3. 网络连接是否正常`)
    }
  } finally {
    visitorsLoading.value = false
  }
}

const changePage = (page: number) => {
  visitorsPage.value = page
  fetchVisitors()
}

// 智能转换英文单词为中文（常见词汇映射）
const translateWord = (word: string): string => {
  const wordLower = word.toLowerCase()
  
  // 常见英文单词到中文的映射（作为后备，主要用于无法智能识别的词）
  const wordMap: Record<string, string> = {
    'dashboard': '仪表盘',
    'home': '首页',
    'index': '首页',
    'blog': '博客',
    'article': '文章',
    'tools': '工具',
    'projects': '项目',
    'life': '生活',
    'lab': '实验室',
    'ai': 'AI实验室',
    'admin': '管理后台',
    'about': '关于',
    'contact': '联系',
    'search': '搜索',
    'profile': '个人资料',
    'settings': '设置',
    'account': '账户',
    'login': '登录',
    'register': '注册',
    'logout': '退出',
    'analytics': '分析',
    'articles': '文章',
    'categories': '分类',
    'timeline': '时间线',
    'themes': '主题',
    'users': '用户',
    'comments': '评论',
    'media': '媒体',
    'pages': '页面',
    'menus': '菜单',
    'widgets': '组件',
    'backup': '备份',
    'logs': '日志',
    'security': '安全',
    'api': 'API',
    'edit': '编辑',
    'create': '创建',
    'update': '更新',
    'delete': '删除',
    'list': '列表',
    'detail': '详情',
    'manage': '管理'
  }
  
  if (wordMap[wordLower]) {
    return wordMap[wordLower]
  }
  
  // 智能识别：处理连字符和驼峰命名
  // 例如：forgot-password -> 忘记密码, userProfile -> 用户资料
  const hyphenParts = wordLower.split('-')
  if (hyphenParts.length > 1) {
    // 处理连字符：尝试翻译每个部分
    const translated = hyphenParts.map(part => wordMap[part] || part).join('')
    if (translated !== wordLower) {
      return translated
    }
  }
  
  // 处理驼峰命名：userProfile -> user profile
  const camelCaseParts = wordLower.replace(/([A-Z])/g, ' $1').split(' ').filter(p => p)
  if (camelCaseParts.length > 1) {
    const translated = camelCaseParts.map(part => wordMap[part] || part).join('')
    if (translated !== wordLower) {
      return translated
    }
  }
  
  // 如果无法识别，返回原词（首字母大写）
  return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase()
}

// 格式化路径名称，智能识别并转换为友好的中文描述
const formatPathName = (path: string): string => {
  if (!path) return '未知页面'
  
  // 移除前缀和查询参数
  const cleanPath = path.replace('landing:', '').replace('page:', '').trim()
  const pathWithoutQuery = cleanPath.split('?')[0].split('#')[0]
  
  // 处理空路径或根路径
  if (!pathWithoutQuery || pathWithoutQuery === '/') {
    return '首页'
  }
  
  // 分割路径
  const parts = pathWithoutQuery.split('/').filter(p => p)
  
  if (parts.length === 0) {
    return '首页'
  }
  
  // 智能翻译每个路径部分
  const translatedParts = parts.map(part => translateWord(part))
  
  // 根据路径层级返回不同格式
  if (parts.length === 1) {
    // 单级路径：直接返回翻译后的名称
    return translatedParts[0]
  } else if (parts.length === 2) {
    // 二级路径：分类：内容
    // 如果第二部分看起来像 slug（包含连字符、数字等），显示为"分类：内容"
    const secondPart = parts[1]
    const isSlug = /^[a-z0-9-]+$/.test(secondPart.toLowerCase()) && secondPart.length > 10
    
    if (isSlug) {
      // 截断过长的 slug
      const displaySlug = secondPart.length > 25 ? secondPart.substring(0, 25) + '...' : secondPart
      return `${translatedParts[0]}：${displaySlug}`
    } else {
      // 如果第二部分也是可识别的单词，翻译它
      return `${translatedParts[0]}：${translatedParts[1]}`
    }
  } else {
    // 多级路径：分类：子分类/内容
    const lastPart = parts[parts.length - 1]
    const isSlug = /^[a-z0-9-]+$/.test(lastPart.toLowerCase()) && lastPart.length > 10
    
    if (isSlug) {
      // 最后一部分是 slug，显示为"分类：路径/内容"
      const displaySlug = lastPart.length > 20 ? lastPart.substring(0, 20) + '...' : lastPart
      const middleParts = translatedParts.slice(1, -1).join('/')
      return `${translatedParts[0]}：${middleParts ? middleParts + '/' : ''}${displaySlug}`
    } else {
      // 所有部分都可识别，全部翻译
      return translatedParts.join('：')
    }
  }
}

const formatTime = (timeStr: string) => {
  if (!timeStr) return '-'
  const date = new Date(timeStr)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / 60000)
  
  if (minutes < 1) return '刚刚'
  if (minutes < 60) return `${minutes}分钟前`
  const hours = Math.floor(minutes / 60)
  if (hours < 24) return `${hours}小时前`
  const days = Math.floor(hours / 24)
  if (days < 7) return `${days}天前`
  
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 格式化页面URL，使其更易读
const formatPageUrl = (url: string) => {
  if (!url) return '未知页面'
  
  // 如果是根路径
  if (url === '/' || url === '') return '首页'
  
  // 移除开头的斜杠
  const cleanUrl = url.startsWith('/') ? url.substring(1) : url
  
  // 根据路径类型返回友好的名称
  if (cleanUrl.startsWith('blog/')) {
    const slug = cleanUrl.replace('blog/', '')
    return slug ? `博客: ${slug}` : '博客列表'
  }
  if (cleanUrl.startsWith('tools/')) {
    const tool = cleanUrl.replace('tools/', '')
    return tool ? `工具: ${tool}` : '工具列表'
  }
  if (cleanUrl.startsWith('ai/')) {
    return 'AI助手'
  }
  if (cleanUrl.startsWith('projects/')) {
    const project = cleanUrl.replace('projects/', '')
    return project ? `项目: ${project}` : '项目列表'
  }
  if (cleanUrl.startsWith('lab')) {
    return '实验室'
  }
  if (cleanUrl.startsWith('admin')) {
    return '管理后台'
  }
  
  // 其他情况返回原始URL（去掉斜杠）
  return cleanUrl || '首页'
}

const fetchTrend = async () => {
  try {
    trendLoading.value = true
    const res = await api.get<any>('/Analytics/trend', {
      params: {
        range: trendRange.value,
        granularity: 'day'
      }
    })
    if (res) {
      trendData.value = res
    } else {
      trendData.value = { points: [] }
    }
  } catch (e: any) {
    // 即使出错也设置空数据，避免显示错误
    trendData.value = { points: [] }
  } finally {
    trendLoading.value = false
  }
}

// 获取概览数据
const fetchOverview = async () => {
  try {
    const res = await api.get<any>('/Analytics/overview')
    if (res) {
      overview.value = res
    }
  } catch (e: any) {
    // 静默失败
  }
}

// 获取 Top 页面
const fetchTopPages = async () => {
  try {
    topPagesLoading.value = true
    const res = await api.get<any>(`/Analytics/top-pages?range=${selectedRange.value}`)
    if (res && res.items) {
      topPages.value = res.items
    }
  } catch (e: any) {
    // 静默失败
  } finally {
    topPagesLoading.value = false
  }
}

// 获取来源分析
const fetchSources = async () => {
  try {
    sourcesLoading.value = true
    const res = await api.get<any>(`/Analytics/sources?range=${selectedRange.value}`)
    if (res) {
      sources.value = res
    }
  } catch (e: any) {
    // 静默失败
  } finally {
    sourcesLoading.value = false
  }
}

// 获取搜索关键词
const fetchSearchKeywords = async () => {
  try {
    searchKeywordsLoading.value = true
    const res = await api.get<any>(`/Analytics/search-keywords?range=${selectedRange.value}`)
    if (res && res.items) {
      searchKeywords.value = res.items
    }
  } catch (e: any) {
    // 静默失败
  } finally {
    searchKeywordsLoading.value = false
  }
}

// 获取地区分布
const fetchRegions = async () => {
  try {
    regionsLoading.value = true
    const res = await api.get<any>(`/Analytics/regions?range=${selectedRange.value}`)
    if (res && res.items) {
      regions.value = { items: res.items }
    } else {
      regions.value = { items: [] }
    }
  } catch (e: any) {
    regions.value = { items: [] }
  } finally {
    regionsLoading.value = false
  }
}

// 获取客户端分布
const fetchClientDistribution = async () => {
  try {
    clientDistributionLoading.value = true
    const res = await api.get<any>(`/Analytics/client-distribution?range=${selectedRange.value}`)
    if (res) {
      clientDistribution.value = {
        devices: res.devices || [],
        browsers: res.browsers || [],
        os: res.os || []
      }
    } else {
      clientDistribution.value = { devices: [], browsers: [], os: [] }
    }
  } catch (e: any) {
    clientDistribution.value = { devices: [], browsers: [], os: [] }
  } finally {
    clientDistributionLoading.value = false
  }
}

// 获取行为路径
const fetchPageFlow = async () => {
  try {
    pageFlowLoading.value = true
    const res = await api.get<any>(`/Analytics/page-flow?range=${selectedRange.value}`)
    if (res) {
      pageFlow.value = res
    }
  } catch (e: any) {
    // 静默失败
  } finally {
    pageFlowLoading.value = false
  }
}

// 统一刷新所有数据
const refreshAll = async () => {
  try {
    await Promise.all([
      fetchOverview(),
      fetchStats(),
      fetchTrend(),
      fetchTopPages(),
      fetchSources(),
      fetchSearchKeywords(),
      fetchRegions(),
      fetchClientDistribution(),
      fetchPageFlow(),
      fetchVisitors()
    ])
    // 标记初始加载完成
    initialLoadComplete.value = true
  } catch (error) {
    // 即使出错也标记为完成，避免一直显示加载状态
    initialLoadComplete.value = true
  }
}

// 时间范围变化时刷新数据
watch(selectedRange, (newRange) => {
  // 趋势图使用独立的 range，但需要同步（today 映射为 7d）
  trendRange.value = newRange === 'today' ? '7d' : newRange as any
  // 刷新所有依赖时间范围的数据
  fetchTopPages()
  fetchSources()
  fetchSearchKeywords()
  fetchRegions()
  fetchClientDistribution()
  fetchPageFlow()
  fetchTrend()
})

const refreshStats = () => {
  refreshAll()
}

const autoRefreshInterval = ref<NodeJS.Timeout | null>(null)

onMounted(() => {
  if (process.client) {
    const token = localStorage.getItem('admin_token')
    
    if (!token) {
      alert('请先登录管理员账号才能查看访客数据！\n\n将跳转到登录页面...')
      navigateTo('/admin/login')
      return
    }
  }
  
  // 延迟一下再加载数据，确保页面完全渲染
  setTimeout(() => {
    refreshAll()
  }, 500)
  
  // 每60秒自动刷新（从30秒改为60秒，减少请求频率）
  // 如果遇到速率限制，会自动停止
  if (process.client) {
    autoRefreshInterval.value = setInterval(() => {
      // 检查是否遇到速率限制
      if (rateLimitRetryCount.value >= 3) {
        if (autoRefreshInterval.value) {
          clearInterval(autoRefreshInterval.value)
          autoRefreshInterval.value = null
        }
        return
      }
      
      fetchOverview()
      fetchStats()
      // 自动刷新访客列表（无论是否勾选"仅显示在线访客"）
      fetchVisitors()
    }, 60000) // 改为 60 秒
  }
})

// 页面卸载时清理定时器
onUnmounted(() => {
  if (autoRefreshInterval.value) {
    clearInterval(autoRefreshInterval.value)
    autoRefreshInterval.value = null
  }
})
</script>

