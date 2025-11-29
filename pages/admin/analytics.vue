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

    <!-- 数据提示 -->
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
          <Pie :data="regionChartData" :options="chartOptions" />
        </div>
        <div v-else class="text-center text-text-muted py-8">暂无数据</div>
      </AppCard>

      <!-- 设备类型饼状图 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">设备类型分布</h2>
        <div v-if="hasDeviceData && deviceChartData.labels.length > 0" class="h-64">
          <Pie :data="deviceChartData" :options="chartOptions" />
        </div>
        <div v-else class="text-center text-text-muted py-8">暂无数据</div>
      </AppCard>

      <!-- 浏览器饼状图 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">浏览器分布</h2>
        <div v-if="hasBrowserData && browserChartData.labels.length > 0" class="h-64">
          <Pie :data="browserChartData" :options="chartOptions" />
        </div>
        <div v-else class="text-center text-text-muted py-8">暂无数据</div>
      </AppCard>

      <!-- 操作系统饼状图 -->
      <AppCard class="p-6">
        <h2 class="text-lg font-bold text-text-main mb-4">操作系统分布</h2>
        <div v-if="hasOsData && osChartData.labels.length > 0" class="h-64">
          <Pie :data="osChartData" :options="chartOptions" />
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
              <Pie
                v-if="sourcesChartData && sourcesChartData.labels.length > 0"
                :data="sourcesChartData"
                :options="chartOptions"
              />
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
            <Pie
              v-if="deviceChartData && deviceChartData.labels.length > 0"
              :data="deviceChartData"
              :options="chartOptions"
            />
          </div>
        </AppCard>

        <!-- 浏览器分布 -->
        <AppCard class="p-6">
          <h2 class="text-lg font-bold text-text-main mb-4">浏览器</h2>
          <div v-if="clientDistributionLoading" class="text-center py-4 text-text-muted">加载中...</div>
          <div v-else-if="!clientDistribution.browsers || clientDistribution.browsers.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else class="h-48">
            <Pie
              v-if="browserChartData && browserChartData.labels.length > 0"
              :data="browserChartData"
              :options="chartOptions"
            />
          </div>
        </AppCard>

        <!-- 操作系统分布 -->
        <AppCard class="p-6">
          <h2 class="text-lg font-bold text-text-main mb-4">操作系统</h2>
          <div v-if="clientDistributionLoading" class="text-center py-4 text-text-muted">加载中...</div>
          <div v-else-if="!clientDistribution.os || clientDistribution.os.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else class="h-48">
            <Pie
              v-if="osChartData && osChartData.labels.length > 0"
              :data="osChartData"
              :options="chartOptions"
            />
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
          <AppButton variant="secondary" size="sm" @click="() => { console.log('🖱️ [Analytics] 手动点击刷新列表按钮'); fetchVisitors(); }">
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
              :key="visitor.Id"
              class="hover:bg-bg-elevated transition-colors"
            >
              <td class="px-4 py-3 text-text-main font-mono text-xs">
                {{ visitor.VisitorId?.substring(0, 8) }}...
              </td>
              <!-- 修复线上访问仍然显示未知的问题：确保 IP 字段正确显示 -->
              <td class="px-4 py-3 text-text-main font-mono text-xs">
                {{ visitor.Ip && visitor.Ip !== '-' ? visitor.Ip : '未知' }}
              </td>
              <td class="px-4 py-3 text-text-main">
                <div class="text-xs">
                  <div v-if="visitor.Country">{{ visitor.Country }}</div>
                  <div v-if="visitor.Region" class="text-text-muted">{{ visitor.Region }}</div>
                  <div v-if="visitor.City" class="text-text-muted">{{ visitor.City }}</div>
                  <div v-if="!visitor.Country && !visitor.Region && !visitor.City" class="text-text-disabled">未知</div>
                </div>
              </td>
              <td class="px-4 py-3 text-text-main">
                <div class="text-xs">
                  <div>{{ visitor.DeviceType || '-' }}</div>
                  <div class="text-text-muted">{{ visitor.Browser || '-' }} / {{ visitor.Os || '-' }}</div>
                </div>
              </td>
              <td class="px-4 py-3 text-text-main">
                <!-- 修复线上访问仍然显示未知的问题：确保 Path 字段正确显示 -->
                <div class="text-xs max-w-xs truncate" :title="visitor.Path || '/'">
                  {{ formatPathName(visitor.Path || '/') }}
                </div>
                <div v-if="visitor.SearchKeyword" class="text-xs text-primary mt-1">
                  搜索: {{ visitor.SearchKeyword }}
                </div>
              </td>
              <!-- 修复线上访问仍然显示未知的问题：确保浏览量字段正确显示 -->
              <td class="px-4 py-3 text-text-main text-center">
                {{ visitor.PageViews > 0 ? visitor.PageViews : 1 }}
              </td>
              <!-- 修复线上访问仍然显示未知的问题：确保最后活跃时间字段正确显示 -->
              <td class="px-4 py-3 text-text-main text-xs">
                {{ visitor.UpdatedAt ? formatTime(visitor.UpdatedAt) : '-' }}
              </td>
              <!-- 修复线上访问仍然显示未知的问题：确保在线状态字段正确显示 -->
              <td class="px-4 py-3">
                <span
                  v-if="visitor.IsOnline === true"
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
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from 'vue'
import {
  Chart as ChartJS,
  ArcElement,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement
} from 'chart.js'
import { Pie, Line } from 'vue-chartjs'

ChartJS.register(
  ArcElement,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement
)

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
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

// 调试：监听访客数据变化
watch(visitors, (newVal) => {
  console.log('[Analytics] visitors 数据变化:', newVal.length, '条')
}, { deep: true })

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

// 生成图表颜色（使用主题 tokens 中的 chart 色板）
const generateColors = (count: number): string[] => {
  if (!process.client) {
    // SSR 时的默认颜色
    return [
      '#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6',
      '#ec4899', '#06b6d4', '#84cc16', '#f97316', '#6366f1'
    ].slice(0, count)
  }
  
  // 从 CSS 变量中获取 chart 颜色
  const chartColors = [
    getThemeColor('--chart-primary'),
    getThemeColor('--chart-secondary'),
    getThemeColor('--chart-tertiary'),
    getThemeColor('--chart-quaternary'),
    getThemeColor('--chart-quinary'),
    getThemeColor('--chart-senary'),
    getThemeColor('--chart-septenary'),
    getThemeColor('--chart-octonary'),
    getThemeColor('--chart-nonary'),
    getThemeColor('--chart-denary')
  ]
  
  return chartColors.slice(0, count)
}

// 访问区域图表数据（使用新的 regions 接口）
const regionChartData = computed(() => {
  console.log('[Analytics] regionChartData computed, regions.value:', regions.value)
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
      borderColor: colors.map((c: string) => c + '80'),
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
      borderColor: colors.map((c: string) => c + '80'),
      borderWidth: 2
    }]
  }
})

// 设备类型图表数据（使用新的 client-distribution 接口）
const deviceChartData = computed(() => {
  console.log('[Analytics] deviceChartData computed, clientDistribution.value:', clientDistribution.value)
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
      borderColor: colors.map((c: string) => c + '80'),
      borderWidth: 2
    }]
  }
})

// 浏览器图表数据（使用新的 client-distribution 接口）
const browserChartData = computed(() => {
  console.log('[Analytics] browserChartData computed, clientDistribution.value:', clientDistribution.value)
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
      borderColor: colors.map((c: string) => c + '80'),
      borderWidth: 2
    }]
  }
})

// 操作系统图表数据（使用新的 client-distribution 接口）
const osChartData = computed(() => {
  console.log('[Analytics] osChartData computed, clientDistribution.value:', clientDistribution.value)
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
      borderColor: colors.map((c: string) => c + '80'),
      borderWidth: 2
    }]
  }
})

// 趋势图数据（使用新的 trend 接口）
const trendChartData = computed(() => {
  console.log('[Analytics] trendChartData computed, trendData.value:', trendData.value)
  // 后端返回的是 points（小写），不是 Points
  const points = trendData.value?.points || trendData.value?.Points || []
  if (!trendData.value || points.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = points.map((p: any) => {
    // 兼容 date（小写）和 Date（大写）
    const dateStr = p.date || p.Date || ''
    if (!dateStr) return ''
    // 格式化日期显示
    if (dateStr.includes(' ')) {
      // 包含时间（小时粒度）
      return dateStr.split(' ')[1] // 只显示时间部分
    } else {
      // 只有日期
      const date = new Date(dateStr)
      return date.toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' })
    }
  })

  const pvColor = getThemeColor('--chart-primary')
  const uvColor = getThemeColor('--chart-secondary')

  return {
    labels,
    datasets: [
      {
        label: '浏览量',
        data: points.map((p: any) => p.pv || p.Pv || 0),
        borderColor: pvColor,
        backgroundColor: pvColor + '20', // 20% 透明度
        tension: 0.4,
        fill: true,
        pointRadius: 3,
        pointHoverRadius: 5
      },
      {
        label: '访客数',
        data: points.map((p: any) => p.uv || p.Uv || 0),
        borderColor: uvColor,
        backgroundColor: uvColor + '20', // 20% 透明度
        tension: 0.4,
        fill: true,
        pointRadius: 3,
        pointHoverRadius: 5
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
          color: gridColor,
          display: true
        },
        ticks: {
          color: legendColor,
          maxRotation: 45,
          minRotation: 0
        }
      },
      y: {
        beginAtZero: true,
        grid: {
          color: gridColor,
          display: true
        },
        ticks: {
          color: legendColor,
          stepSize: 1
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
  // 如果数据还在加载中，不显示黄框，避免闪烁
  if (statsLoading.value || topPagesLoading.value || trendLoading.value || 
      regionsLoading.value || clientDistributionLoading.value) return false

  // 检查是否有任何数据
  const hasOverviewData = (overview.value?.todayPv ?? 0) > 0 || (overview.value?.todayUv ?? 0) > 0 ||
                          (overview.value?.totalPv ?? 0) > 0 || (overview.value?.totalUv ?? 0) > 0
  const hasAnyData = hasOverviewData || hasTrendData.value || hasRegionData.value || 
                     hasClientDistributionData.value || (topPages.value?.length ?? 0) > 0 || 
                     (sources.value?.items?.length ?? 0) > 0 || (searchKeywords.value?.length ?? 0) > 0 ||
                     (pageFlow.value?.edges?.length ?? 0) > 0

  return !hasAnyData
})

const statsLoading = ref(false)
const statsError = ref<string | null>(null)
const rateLimitRetryCount = ref(0)

const fetchStats = async () => {
  // 如果正在加载，跳过本次请求
  if (statsLoading.value) {
    console.log('统计数据正在加载中，跳过本次请求')
    return
  }

  try {
    statsLoading.value = true
    statsError.value = null
    console.log('开始获取统计数据...')
    const res = await api.get<any>('/Analytics/stats')
    console.log('API 响应:', res)
    
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
      console.log('统计数据加载成功:', stats.value)
      console.log('今日PV:', stats.value.Today?.Pv)
      console.log('今日UV:', stats.value.Today?.Uv)
      console.log('访问区域数据:', stats.value.RegionStats?.length || 0, '条')
      console.log('设备数据:', stats.value.DeviceStats?.length || 0, '条')
      
      // 获取调试信息
      try {
        const debugRes = await api.get<any>('/Analytics/debug-status')
        console.log('=== 访客分析调试信息 ===')
        console.log('最近7天访问日志数:', debugRes.visitLogsCountLast7Days)
        console.log('今日访问日志数:', debugRes.visitLogsCountToday)
        console.log('是否有 VisitorAnalytics 数据:', debugRes.hasVisitorAnalyticsData)
        console.log('VisitorAnalytics 记录数:', debugRes.visitorAnalyticsCount)
        console.log('今日PV (从Logs):', debugRes.todayPvFromLogs)
        console.log('今日UV (从Logs):', debugRes.todayUvFromLogs)
        console.log('今日PV (从Analytics):', debugRes.todayPvFromAnalytics)
        console.log('今日UV (从Analytics):', debugRes.todayUvFromAnalytics)
        console.log('最后一条访问记录:', debugRes.lastVisitSample)
        console.log('========================')
      } catch (debugErr) {
        console.warn('获取调试信息失败:', debugErr)
      }
    } else {
      console.warn('API 返回空数据')
    }
  } catch (e: any) {
    console.error('Failed to fetch analytics:', e)
    console.error('错误详情:', e.message, e.response)
    
    // 处理 429 速率限制错误
    if (e.response?.status === 429) {
      rateLimitRetryCount.value++
      statsError.value = '请求过于频繁，请稍后再试'
      console.warn('速率限制错误，重试次数:', rateLimitRetryCount.value)
      
      // 如果连续遇到速率限制，停止自动刷新
      if (rateLimitRetryCount.value >= 3) {
        console.warn('连续遇到速率限制，停止自动刷新')
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
      if (process.client && rateLimitRetryCount.value === 0) {
        // 只在第一次错误时显示 alert，避免频繁弹窗
        console.warn('获取统计数据失败:', e.message || '未知错误')
      }
    }
  } finally {
    statsLoading.value = false
  }
}

const fetchVisitors = async () => {
  try {
    visitorsLoading.value = true
    console.log('🔍 [Analytics] ========== 开始获取访客列表 ==========')
    console.log('🔍 [Analytics] 请求参数:', {
      page: visitorsPage.value,
      pageSize: pageSize.value,
      onlineOnly: onlineOnly.value
    })
    console.log('🔍 [Analytics] 准备发送 API 请求...')
    console.log('🔍 [Analytics] API 请求开始时间:', new Date().toISOString())
    
    let res: any = null
    try {
      res = await api.get<any>('/Analytics/visitors', {
        params: {
          page: visitorsPage.value,
          pageSize: pageSize.value,
          onlineOnly: onlineOnly.value
        }
      })
      console.log('✅ [Analytics] 访客列表API响应成功！')
      console.log('✅ [Analytics] API 响应时间:', new Date().toISOString())
      console.log('📦 [Analytics] API响应原始数据:', res)
      console.log('📦 [Analytics] API响应数据类型:', typeof res)
      console.log('📦 [Analytics] API响应是否为数组:', Array.isArray(res))
      console.log('📦 [Analytics] API响应是否为 null:', res === null)
      console.log('📦 [Analytics] API响应是否为 undefined:', res === undefined)
      console.log('📦 [Analytics] API响应数据结构:', JSON.stringify(res, null, 2))
    } catch (apiError: any) {
      console.error('❌ [Analytics] API 请求失败（在 fetchVisitors 内部捕获）:', apiError)
      throw apiError // 重新抛出，让外层的 catch 处理
    }
    if (res) {
      console.log('📦 [Analytics] 开始处理响应数据...')
      console.log('📦 [Analytics] res 的所有键:', Object.keys(res || {}))
      
      // useApi 已经提取了 data 字段，所以 res 应该是 { total: 39, page: 1, pageSize: 20, visitors: [...] }
      // 注意：后端返回的是小写的 "visitors" 和 "total"，优先使用小写
      const visitorsData = res.visitors || res.Visitors || (Array.isArray(res) ? res : [])
      const totalData = res.total ?? res.Total ?? (Array.isArray(res) ? res.length : 0)
      
      console.log('📦 [Analytics] 提取的 visitorsData:', visitorsData)
      console.log('📦 [Analytics] 提取的 totalData:', totalData)
      console.log('📦 [Analytics] visitorsData 是否为数组:', Array.isArray(visitorsData))
      
      visitors.value = Array.isArray(visitorsData) ? visitorsData : []
      visitorsTotal.value = totalData
      
      console.log('📦 [Analytics] 数据已赋值到 visitors.value，长度:', visitors.value.length)
      
      console.log(`[Analytics] 获取到 ${visitors.value.length} 条访客记录，总计 ${visitorsTotal.value} 条`)
      console.log(`[Analytics] onlineOnly=${onlineOnly.value}`)
      console.log(`[Analytics] visitors.value 详细数据:`, JSON.stringify(visitors.value, null, 2))
      
      // 详细调试信息
      if (visitors.value.length > 0) {
        console.log('[Analytics] ✅ 访客数据已获取，第一条记录:', {
          id: visitors.value[0].Id,
          visitorId: visitors.value[0].VisitorId,
          ip: visitors.value[0].Ip,
          path: visitors.value[0].Path,
          isOnline: visitors.value[0].IsOnline,
          updatedAt: visitors.value[0].UpdatedAt,
          pageViews: visitors.value[0].PageViews
        })
      } else {
        console.warn('[Analytics] ⚠️ 访客列表为空，可能的原因：')
        console.warn(`  1. onlineOnly=${onlineOnly.value}，如果为 true 则只显示最近5分钟内的在线访客`)
        console.warn('  2. VisitorAnalytics 表为空且 VisitLogs 表也为空')
        console.warn('  3. 时间范围过滤导致没有数据')
        console.warn('  4. 建议：取消勾选"仅显示在线访客"查看所有访客')
        console.warn('  5. 请检查后端控制台的 [Analytics Visitors] 日志，确认查询结果')
      }
    } else {
      console.warn('[Analytics] 访客列表API返回空数据')
    }
  } catch (e: any) {
    console.error('❌❌❌ [Analytics] ========== 获取访客列表失败! ==========')
    console.error('❌ [Analytics] 错误类型:', e?.constructor?.name || typeof e)
    console.error('❌ [Analytics] 错误信息:', e?.message || String(e))
    console.error('❌ [Analytics] 错误对象:', e)
    if (e?.response) {
      console.error('❌ [Analytics] HTTP 状态码:', e.response.status)
      console.error('❌ [Analytics] 响应数据:', e.response.data)
    }
    console.error('❌ [Analytics] 完整错误堆栈:', e?.stack)
    // 显示错误提示
    if (process.client) {
      alert(`获取访客列表失败: ${e?.message || '未知错误'}\n\n请检查：\n1. 是否已登录管理员账号\n2. 后端服务是否正常运行\n3. 网络连接是否正常`)
    }
  } finally {
    visitorsLoading.value = false
    console.log('🔍 [Analytics] fetchVisitors 执行完成，loading 状态已重置')
  }
}

const changePage = (page: number) => {
  visitorsPage.value = page
  fetchVisitors()
}

// 格式化路径名称，转换为友好的中文描述（智能识别，不依赖写死的映射表）
const formatPathName = (path: string): string => {
  if (!path) return '未知页面'
  
  // 移除前缀
  const cleanPath = path.replace('landing:', '').replace('page:', '').trim()
  
  // 处理空路径或根路径
  if (!cleanPath || cleanPath === '/') {
    return '首页'
  }
  
  // 移除查询参数和锚点
  const pathWithoutQuery = cleanPath.split('?')[0].split('#')[0]
  
  // 智能识别路径结构
  const parts = pathWithoutQuery.split('/').filter(p => p)
  
  if (parts.length === 0) {
    return '首页'
  }
  
  // 第一级路径映射（常用路径）
  const firstLevelMap: Record<string, string> = {
    'blog': '博客',
    'tools': '工具',
    'projects': '项目',
    'life': '生活',
    'lab': '实验室',
    'ai': 'AI实验室',
    'admin': '管理后台',
    'about': '关于',
    'contact': '联系',
    'search': '搜索'
  }
  
  const firstPart = parts[0]
  const firstLevelName = firstLevelMap[firstPart] || firstPart
  
  // 如果只有一级路径，直接返回
  if (parts.length === 1) {
    return firstLevelName
  }
  
  // 处理二级路径
  const secondPart = parts[1]
  
  // 管理后台的特殊处理
  if (firstPart === 'admin') {
    const adminPageMap: Record<string, string> = {
      'analytics': '访客分析',
      'articles': '文章管理',
      'categories': '分类管理',
      'projects': '项目管理',
      'timeline': '时间线',
      'themes': '主题设置',
      'theme-settings': '主题设置',
      'edit': '编辑',
      'login': '登录'
    }
    return adminPageMap[secondPart] || `${firstLevelName}：${secondPart}`
  }
  
  // 处理动态路由（如 /blog/[slug], /tools/[slug] 等）
  if (parts.length === 2) {
    const slug = secondPart
    
    // 智能截断过长的 slug
    const displaySlug = slug.length > 25 ? slug.substring(0, 25) + '...' : slug
    
    // 根据第一级路径返回对应的描述
    if (firstPart === 'blog' || firstPart === 'article') {
      return `${firstLevelName}：${displaySlug}`
    }
    if (firstPart === 'tools') {
      return `工具：${displaySlug}`
    }
    if (firstPart === 'projects') {
      return `项目：${displaySlug}`
    }
    if (firstPart === 'life') {
      return `生活：${displaySlug}`
    }
    if (firstPart === 'ai') {
      // AI 实验室可能有 type 和 slug
      if (parts.length === 3) {
        return `AI：${parts[1]}/${displaySlug}`
      }
      return `AI：${displaySlug}`
    }
    
    // 默认格式：分类/内容
    return `${firstLevelName}：${displaySlug}`
  }
  
  // 处理三级或更多级路径（如 /ai/[type]/[slug]）
  if (parts.length >= 3) {
    const lastPart = parts[parts.length - 1]
    const displaySlug = lastPart.length > 20 ? lastPart.substring(0, 20) + '...' : lastPart
    return `${firstLevelName}：${parts.slice(1).join('/')}`
  }
  
  // 默认情况：返回路径的友好格式
  const displayPath = pathWithoutQuery.length > 35 
    ? pathWithoutQuery.substring(0, 35) + '...' 
    : pathWithoutQuery
  
  // 如果路径以 / 开头，移除它
  return displayPath.startsWith('/') ? displayPath.substring(1) : displayPath
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
    console.log('[Analytics] fetchTrend called, range:', trendRange.value)
    const res = await api.get<any>('/Analytics/trend', {
      params: {
        range: trendRange.value,
        granularity: 'day'
      }
    })
    console.log('[Analytics] trend API response:', res)
    if (res) {
      trendData.value = res
      const points = res.points || res.Points || []
      console.log('[Analytics] trendData assigned, points count:', points.length)
      console.log('[Analytics] trendData.value:', trendData.value)
    } else {
      console.warn('[Analytics] 趋势数据API返回空数据')
      trendData.value = { points: [] }
    }
  } catch (e: any) {
    console.error('[Analytics] Failed to fetch trend:', e)
    console.error('[Analytics] 错误详情:', e.message, e.response)
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
    console.error('Failed to fetch overview:', e)
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
    console.error('Failed to fetch top pages:', e)
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
    console.error('Failed to fetch sources:', e)
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
    console.error('Failed to fetch search keywords:', e)
  } finally {
    searchKeywordsLoading.value = false
  }
}

// 获取地区分布
const fetchRegions = async () => {
  try {
    regionsLoading.value = true
    console.log('[Analytics] fetchRegions called, range:', selectedRange.value)
    const res = await api.get<any>(`/Analytics/regions?range=${selectedRange.value}`)
    console.log('[Analytics] regions API response:', res)
    if (res && res.items) {
      regions.value = { items: res.items }
      console.log('[Analytics] regions.value assigned, items count:', res.items.length)
    } else {
      console.warn('[Analytics] 地区分布API返回空数据')
      regions.value = { items: [] }
    }
  } catch (e: any) {
    console.error('[Analytics] Failed to fetch regions:', e)
    regions.value = { items: [] }
  } finally {
    regionsLoading.value = false
  }
}

// 获取客户端分布
const fetchClientDistribution = async () => {
  try {
    clientDistributionLoading.value = true
    console.log('[Analytics] fetchClientDistribution called, range:', selectedRange.value)
    const res = await api.get<any>(`/Analytics/client-distribution?range=${selectedRange.value}`)
    console.log('[Analytics] client-distribution API response:', res)
    if (res) {
      clientDistribution.value = {
        devices: res.devices || [],
        browsers: res.browsers || [],
        os: res.os || []
      }
      console.log('[Analytics] clientDistribution.value assigned:')
      console.log('  - devices:', clientDistribution.value.devices.length)
      console.log('  - browsers:', clientDistribution.value.browsers.length)
      console.log('  - os:', clientDistribution.value.os.length)
    } else {
      console.warn('[Analytics] 客户端分布API返回空数据')
      clientDistribution.value = { devices: [], browsers: [], os: [] }
    }
  } catch (e: any) {
    console.error('[Analytics] Failed to fetch client distribution:', e)
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
    console.error('Failed to fetch page flow:', e)
  } finally {
    pageFlowLoading.value = false
  }
}

// 统一刷新所有数据
const refreshAll = async () => {
  console.log('🔄 [Analytics] ========== refreshAll() 被调用 ==========')
  console.log('🔄 [Analytics] 开始并行加载所有数据...')
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
    console.log('✅ [Analytics] 所有数据加载完成')
  } catch (error) {
    console.error('❌ [Analytics] 加载数据时出错:', error)
  }
}

// 时间范围变化时刷新数据
watch(selectedRange, (newRange) => {
  console.log('[Analytics] selectedRange changed to:', newRange)
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
  console.log('🚀🚀🚀 [Analytics] ========== 访客分析页面开始加载 ==========')
  console.log('🚀 [Analytics] 检查管理员登录状态...')
  
  if (process.client) {
    const token = localStorage.getItem('admin_token')
    const user = localStorage.getItem('admin_user')
    console.log('🚀 [Analytics] 管理员Token:', token ? '已存在' : '不存在')
    console.log('🚀 [Analytics] 管理员信息:', user)
    
    if (!token) {
      console.warn('⚠️ [Analytics] 未检测到管理员登录，请先登录')
      alert('请先登录管理员账号才能查看访客数据！\n\n将跳转到登录页面...')
      navigateTo('/admin/login')
      return
    }
    
    console.log('✅ [Analytics] 管理员已登录，准备加载数据')
  }
  
  // 延迟一下再加载数据，确保页面完全渲染
  setTimeout(() => {
    console.log('🚀 [Analytics] 页面已加载，开始初始化数据...')
    console.log('🚀 [Analytics] 调用 refreshAll() 函数...')
    refreshAll()
  }, 500)
  
  // 每60秒自动刷新（从30秒改为60秒，减少请求频率）
  // 如果遇到速率限制，会自动停止
  if (process.client) {
    autoRefreshInterval.value = setInterval(() => {
      // 检查是否遇到速率限制
      if (rateLimitRetryCount.value >= 3) {
        console.warn('已停止自动刷新（速率限制）')
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

