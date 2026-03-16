<template>
  <!-- 
    访客分析模块 - Aurora Design System
    moduleId: analytics_dashboard
  -->
  <div 
    :data-module-theme="moduleTheme || undefined"
    class="min-h-screen p-6 lg:p-10 text-text-main transition-colors duration-500"
  >
    <!-- Header: 渐变色标�?+ 实时刷新开关和刷新按钮 -->
    <div class="flex justify-between items-center mb-8">
      <div>
        <h1 class="text-3xl font-bold bg-gradient-to-r from-primary via-purple-500 to-secondary text-transparent bg-clip-text">
          访客分析
        </h1>
        <p class="text-sm text-text-muted mt-2">
          查看网站访问统计和访客数�?        </p>
      </div>
      <div class="flex items-center gap-3">
        <label class="flex items-center gap-2 text-sm text-text-muted cursor-pointer">
          <input
            type="checkbox"
            v-model="autoRefreshEnabled"
            class="rounded"
          />
          实时刷新
        </label>
        <AppButton variant="primary" @click="refreshStats">
          刷新数据
        </AppButton>
      </div>
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

    <!-- 数据提示和主要内容：只在加载完成后显�?-->
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
            <!-- 使用样式组合类简化代�?-->
            <h3 class="text-sm text-heading mb-2">暂无访客数据</h3>
            <div class="mt-2 text-sm text-body leading-relaxed">
              <p class="mb-2 font-medium">当前没有访客访问记录。可能的原因�?/p>
              <ul class="list-disc list-inside mt-1 space-y-1 ml-2">
                <li>网站还没有访客访�?/li>
                <!-- 使用 bg-code 样式组合类，替代多个类名 -->
                <li>访客数据存储�?<code class="bg-code">VisitLogs</code> 表中，请检查数据库</li>
                <li>如果使用代理或VPN，可能无法正确记录IP地址</li>
              </ul>
              <p class="mt-3 mb-2 font-medium">
                <strong class="text-body">提示�?/strong>访问网站首页会自动记录访问数据。您可以�?              </p>
              <ul class="list-disc list-inside mt-1 space-y-1 ml-2">
                <li>打开网站首页，系统会自动记录您的访问</li>
                <li>点击"刷新数据"按钮更新统计数据</li>
                <li>检查浏览器控制台的日志信息</li>
              </ul>
            </div>
          </div>
        </div>
      </AppCard>

    <!-- �?行：概览数据卡片 (Bento Grid) -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
      <!-- PV 卡片 -->
      <AppCard hover class="relative overflow-hidden group backdrop-blur-xl">
        <!-- 装饰性模糊圆�?-->
        <div class="absolute top-0 right-0 w-32 h-32 bg-primary/10 blur-3xl -mr-16 -mt-16 opacity-60 group-hover:opacity-100 transition-opacity"></div>
        <div class="relative z-10 p-6">
          <div class="text-sm text-text-muted mb-2">今日浏览�?/div>
          <div class="text-3xl font-bold mb-2" style="color: var(--color-primary, var(--color-primary));">
            {{ overview.todayPv ?? 0 }}
          </div>
          <div class="text-xs text-text-muted">
            昨日: {{ overview.yesterdayPv ?? 0 }} | 总计: {{ overview.totalPv ?? 0 }}
          </div>
        </div>
      </AppCard>

      <!-- UV 卡片 -->
      <AppCard hover class="relative overflow-hidden group backdrop-blur-xl">
        <div class="absolute top-0 right-0 w-32 h-32 bg-chart-secondary/10 blur-3xl -mr-16 -mt-16 opacity-60 group-hover:opacity-100 transition-opacity"></div>
        <div class="relative z-10 p-6">
          <div class="text-sm text-text-muted mb-2">今日访客�?/div>
          <div class="text-3xl font-bold mb-2" style="color: var(--chart-secondary, var(--color-success));">
            {{ overview.todayUv ?? 0 }}
          </div>
          <div class="text-xs text-text-muted">
            昨日: {{ overview.yesterdayUv || 0 }} | 总计: {{ overview.totalUv || 0 }}
          </div>
        </div>
      </AppCard>

      <!-- 在线人数卡片 -->
      <AppCard hover class="relative overflow-hidden group backdrop-blur-xl">
        <div class="absolute top-0 right-0 w-32 h-32 bg-chart-tertiary/10 blur-3xl -mr-16 -mt-16 opacity-60 group-hover:opacity-100 transition-opacity"></div>
        <div class="relative z-10 p-6">
          <div class="text-sm text-text-muted mb-2">在线人数</div>
          <div class="text-3xl font-bold mb-2" style="color: var(--chart-tertiary, var(--color-warning));">
            {{ overview.onlineUsers ?? 0 }}
          </div>
          <div class="text-xs text-text-muted">最�?分钟活跃</div>
        </div>
      </AppCard>

      <!-- 总访问卡�?-->
      <AppCard hover class="relative overflow-hidden group backdrop-blur-xl">
        <div class="absolute top-0 right-0 w-32 h-32 bg-chart-quinary/10 blur-3xl -mr-16 -mt-16 opacity-60 group-hover:opacity-100 transition-opacity"></div>
        <div class="relative z-10 p-6">
          <div class="text-sm text-text-muted mb-2">热门文章�?/div>
          <div class="text-3xl font-bold mb-2" style="color: var(--chart-quinary, var(--color-purple-500));">
            {{ overview.hotArticleCount ?? 0 }}
          </div>
          <div class="text-xs text-text-muted">访问次数 > 1</div>
        </div>
      </AppCard>
    </div>

    <!-- �?行：趋势�?(全宽) -->
    <AppCard hover class="mb-6 p-6 backdrop-blur-xl">
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-xl font-bold text-text-main">浏览�?访客数趋�?/h2>
        <div class="flex gap-2">
          <AppButton
            :variant="trendRange === '7d' ? 'primary' : 'secondary'"
            size="sm"
            @click="trendRange = '7d'; selectedRange = '7d'"
          >
            7�?          </AppButton>
          <AppButton
            :variant="trendRange === '30d' ? 'primary' : 'secondary'"
            size="sm"
            @click="trendRange = '30d'; selectedRange = '30d'"
          >
            30�?          </AppButton>
          <AppButton
            :variant="trendRange === '90d' ? 'primary' : 'secondary'"
            size="sm"
            @click="trendRange = '90d'; selectedRange = '90d'"
          >
            90�?          </AppButton>
        </div>
      </div>
      <div v-if="trendLoading" class="text-center py-8 text-text-muted">
        加载�?..
      </div>
      <ClientOnly>
        <template v-if="hasTrendData && trendLineOption">
          <div class="h-[500px] relative w-full">
            <v-chart :option="trendLineOption" autoresize class="w-full h-full" />
          </div>
        </template>
        <template v-else>
          <div class="text-center py-8 text-text-muted h-[500px] flex items-center justify-center">
            {{ trendLoading ? '加载�?..' : '暂无趋势数据' }}
          </div>
        </template>
        <template #fallback>
          <div class="h-[500px] flex items-center justify-center">
            <div class="text-center">
              <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary mb-4"></div>
              <p class="text-text-muted">加载�?..</p>
            </div>
          </div>
        </template>
      </ClientOnly>
    </AppCard>

    <!-- �?行：详细数据 (Bento Grid: 左侧4�?+ 右侧8�? -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-6 mb-6">
      <!-- 左侧�?�?- 两个 Donut 图表 -->
      <div class="lg:col-span-4 space-y-6">
        <!-- 来源分析 Donut -->
        <AppCard hover class="p-6 backdrop-blur-xl">
          <h2 class="text-lg font-bold text-text-main mb-4">访问来源</h2>
          <div v-if="sourcesLoading" class="text-center py-4 text-text-muted">加载�?..</div>
          <div v-else-if="!sources.items || sources.items.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
          <div v-else>
            <ClientOnly>
              <template v-if="sourceDonutOption">
                <div class="relative w-full" style="height: var(--analytics-donut-chart-height, 20rem); padding-bottom: var(--analytics-legend-height, 3rem);">
                  <v-chart :option="sourceDonutOption.option" autoresize class="w-full" style="height: var(--analytics-donut-chart-height, 20rem);" />
                  <div class="donut-center">
                    <div class="donut-center-value">{{ sourceDonutOption.mainPercent }}%</div>
                    <div class="donut-center-label">{{ sourceDonutOption.mainLabel }}</div>
                  </div>
                </div>
              </template>
              <template v-else>
                <div class="text-center text-text-muted py-8 h-48 flex items-center justify-center">暂无数据</div>
              </template>
            </ClientOnly>
            
            <div class="space-y-2 max-h-32 overflow-y-auto mt-6">
              <div
                v-for="(item, index) in sources.items"
                :key="index"
                class="analytics-source-item"
              >
                <div class="flex items-center gap-2">
                  <div 
                    class="w-2 h-2 rounded-full" 
                    :style="{ backgroundColor: getDonutColor(index) }"
                  ></div>
                  <span class="text-sm font-medium text-text-main">{{ item.name }}</span>
                </div>
                <span class="text-xs text-text-muted">{{ item.count }}</span>
              </div>
            </div>
          </div>
        </AppCard>

        <!-- 设备分布 Donut -->
        <AppCard hover class="p-6 backdrop-blur-xl">
          <h2 class="text-lg font-bold text-text-main mb-4">设备类型分布</h2>
          <ClientOnly>
            <template v-if="deviceDonutOption">
              <div class="relative w-full" style="height: var(--analytics-donut-chart-height, 20rem); padding-bottom: var(--analytics-legend-height, 3rem);">
                <v-chart :option="deviceDonutOption.option" autoresize class="w-full" style="height: var(--analytics-donut-chart-height, 20rem);" />
                <div class="donut-center">
                  <div class="donut-center-value">{{ deviceDonutOption.mainPercent }}%</div>
                  <div class="donut-center-label">{{ deviceDonutOption.mainLabel }}</div>
                </div>
              </div>
            </template>
            <template v-else>
              <div class="text-center text-text-muted py-8">暂无数据</div>
            </template>
            <template #fallback>
              <div class="h-64 flex items-center justify-center">
                <div class="text-center">
                  <div class="inline-block animate-spin rounded-full h-6 w-6 border-b-2 border-primary"></div>
                </div>
              </div>
            </template>
          </ClientOnly>
        </AppCard>
      </div>

      <!-- 右侧�?�?- 上部双列 + 下部表格 -->
      <div class="lg:col-span-8 space-y-6">
        <!-- 上部：双列布局 -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Top 10 页面 -->
          <AppCard hover class="p-6 backdrop-blur-xl">
            <div class="flex justify-between items-center mb-4">
              <h2 class="text-lg font-bold text-text-main">Top 10 页面</h2>
              <select v-model="selectedRange" class="text-sm px-2 py-1 rounded border border-border-subtle bg-bg-surface-2 text-text-main">
                <option value="today">今日</option>
                <option value="7d">7�?/option>
                <option value="30d">30�?/option>
                <option value="90d">90�?/option>
              </select>
            </div>
            <div v-if="topPagesLoading" class="text-center py-4 text-text-muted">加载�?..</div>
            <div v-else-if="topPages.length === 0" class="text-center py-4 text-text-muted">暂无数据</div>
            <div v-else class="space-y-2 max-h-64 overflow-y-auto">
              <div
                v-for="(page, index) in topPages.slice(0, 10)"
                :key="index"
                class="analytics-page-item"
              >
                <div class="flex-1 min-w-0">
                  <div class="text-sm font-medium text-text-main truncate" :title="formatPageUrl(page.url)">
                    {{ formatPageUrl(page.url) }}
                  </div>
                  <div class="text-xs text-text-muted mt-1.5 flex items-center gap-3">
                    <span>浏览�? <span class="font-semibold text-primary">{{ page.pv }}</span></span>
                    <span>访客�? <span class="font-semibold text-chart-secondary">{{ page.uv }}</span></span>
                  </div>
                </div>
                <div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center text-primary font-bold text-sm ml-3 flex-shrink-0">
                  {{ index + 1 }}
                </div>
              </div>
            </div>
          </AppCard>

          <!-- 地区分布条形�?-->
          <AppCard hover class="p-6 backdrop-blur-xl">
            <h2 class="text-lg font-bold text-text-main mb-4">地区分布</h2>
            <ClientOnly>
              <template v-if="hasRegionData && regionBarOption">
                <div class="h-64 w-full">
                  <v-chart :option="regionBarOption" autoresize class="w-full h-full" />
                </div>
              </template>
              <template v-else>
                <div class="text-center text-text-muted py-8 h-64 flex items-center justify-center">暂无数据</div>
              </template>
              <template #fallback>
                <div class="h-64 flex items-center justify-center">
                  <div class="text-center">
                    <div class="inline-block animate-spin rounded-full h-6 w-6 border-b-2 border-primary"></div>
                  </div>
                </div>
              </template>
            </ClientOnly>
          </AppCard>
        </div>

        <!-- 下部：实时访客表�?-->
        <AppCard hover class="p-6 backdrop-blur-xl">
          <div class="flex justify-between items-center mb-4">
            <h2 class="text-lg font-bold text-text-main">实时访客</h2>
            <div class="flex items-center gap-4">
              <label class="flex items-center gap-2 text-sm text-text-muted">
                <input
                  type="checkbox"
                  v-model="onlineOnly"
                  @change="fetchVisitors"
                  class="rounded"
                />
                仅显示在线访�?              </label>
              <AppButton variant="secondary" size="sm" @click="fetchVisitors">
                刷新列表
              </AppButton>
            </div>
          </div>

          <div v-if="visitorsLoading" class="text-center py-8 text-text-muted">
            加载�?..
          </div>
          <div v-else-if="visitors.length === 0" class="text-center py-8 text-text-muted">
            暂无访客数据
          </div>
          <div v-else class="overflow-x-auto -mx-6 px-6">
            <table class="w-full text-sm border-collapse">
              <thead>
                <tr class="border-b border-border-subtle">
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase var(--color-bg-light, white)space-nowrap">访客ID</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase var(--color-bg-light, white)space-nowrap">IP地址</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase var(--color-bg-light, white)space-nowrap">地理位置</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase var(--color-bg-light, white)space-nowrap">设备信息</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase var(--color-bg-light, white)space-nowrap">当前页面</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase var(--color-bg-light, white)space-nowrap">浏览�?/th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase var(--color-bg-light, white)space-nowrap">最后活�?/th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase var(--color-bg-light, white)space-nowrap">状�?/th>
                </tr>
              </thead>
              <tbody class="divide-y divide-border-subtle">
                <tr
                  v-for="visitor in visitors"
                  :key="visitor.id || visitor.Id"
                  class="border-b border-border-subtle hover:bg-bg-surface-2/50 transition-colors"
                >
                  <td class="px-4 py-3 text-text-main font-mono text-xs">
                    {{ (visitor.visitorId || visitor.VisitorId)?.substring(0, 8) }}...
                  </td>
                  <td class="px-4 py-3 text-text-main font-mono text-xs">
                    {{ (visitor.ip || visitor.Ip) && (visitor.ip || visitor.Ip) !== '-' ? (visitor.ip || visitor.Ip) : '未知' }}
                  </td>
                  <td class="px-4 py-3 text-text-main">
                    <div class="text-xs">
                      <div v-if="visitor.country || visitor.Country">{{ visitor.country || visitor.Country }}</div>
                      <div v-if="visitor.region || visitor.Region" class="text-text-muted">{{ visitor.region || visitor.Region }}</div>
                      <div v-if="visitor.city || visitor.City" class="text-text-muted">{{ visitor.city || visitor.City }}</div>
                      <div v-if="!(visitor.country || visitor.Country) && !(visitor.region || visitor.Region) && !(visitor.city || visitor.City)" class="text-text-disabled">未知</div>
                    </div>
                  </td>
                  <td class="px-4 py-3 text-text-main">
                    <div class="text-xs">
                      <div>{{ (visitor.deviceType || visitor.DeviceType) && (visitor.deviceType || visitor.DeviceType) !== 'unknown' ? (visitor.deviceType || visitor.DeviceType) : '-' }}</div>
                      <div class="text-text-muted">
                        {{ (visitor.browser || visitor.Browser) && (visitor.browser || visitor.Browser) !== 'unknown' ? (visitor.browser || visitor.Browser) : '-' }} / 
                        {{ (visitor.os || visitor.Os) && (visitor.os || visitor.Os) !== 'unknown' ? (visitor.os || visitor.Os) : '-' }}
                      </div>
                    </div>
                  </td>
                  <td class="px-4 py-3 text-text-main">
                    <div class="flex items-center gap-1.5">
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
                      </div>
                    </div>
                    <div v-if="visitor.searchKeyword || visitor.SearchKeyword" class="text-xs text-primary mt-1.5 flex items-center gap-1">
                      <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                      </svg>
                      搜索: {{ visitor.searchKeyword || visitor.SearchKeyword }}
                    </div>
                  </td>
                  <td class="px-4 py-3 text-text-main text-center">
                    {{ ((visitor.pageViews || visitor.PageViews) || 0) > 0 ? (visitor.pageViews || visitor.PageViews) : 1 }}
                  </td>
                  <td class="px-4 py-3 text-text-main text-xs">
                    {{ (visitor.updatedAt || visitor.UpdatedAt) ? formatTime(visitor.updatedAt || visitor.UpdatedAt) : '-' }}
                  </td>
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
                      class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-bg-surface-2 text-text-muted"
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
                �?{{ visitorsTotal }} 条记�?              </div>
              <div class="flex gap-2">
                <AppButton
                  variant="secondary"
                  size="sm"
                  @click="changePage(visitorsPage - 1)"
                  :disabled="visitorsPage <= 1"
                >
                  上一�?                </AppButton>
                <span class="px-3 py-1 text-sm text-text-main">
                  �?{{ visitorsPage }} / {{ Math.ceil(visitorsTotal / pageSize) }} �?                </span>
                <AppButton
                  variant="secondary"
                  size="sm"
                  @click="changePage(visitorsPage + 1)"
                  :disabled="visitorsPage >= Math.ceil(visitorsTotal / pageSize)"
                >
                  下一�?                </AppButton>
              </div>
            </div>
          </div>
        </AppCard>
      </div>
    </div>

    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { LineChart, PieChart, BarChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components'
import VChart from 'vue-echarts'
import { useEChartsTheme } from '~/composables/useEChartsTheme'
import AppCard from '~/components/ui/AppCard.vue'
import AppButton from '~/components/ui/AppButton.vue'
// 不再使用 NNumberAnimation，直接显示数�?
// �?setup 顶层调用 useEChartsTheme，避免在 computed 中重复调�?// 由于页面已设�?ssr: false，这些函数只在客户端使用
const { isDark, applyTheme, buildNeonLineOptions, buildNeonBarOptions, buildNeonDonutOptions, getCssVar } = useEChartsTheme()

// 注册 ECharts 组件
use([
  CanvasRenderer,
  LineChart,
  PieChart,
  BarChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
])



definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // 禁用 SSR，避�?Naive UI 组件在服务端渲染时出�?})

// 注意：AppButton 组件应该�?Nuxt 3 中自动导�?// 如果出现 "Failed to resolve component: AppButton" 错误�?// 可能需要检�?components/ui/AppButton.vue 是否存在
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
const onlineOnly = ref(false) // 默认不勾选，显示所有访�?
// 初始加载完成标志，用于避免页面刚进入时显�?暂无数据"提示
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

// 趋势图相关（�?selectedRange 同步�?const trendRange = ref<'7d' | '30d' | '90d'>('7d')
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

// 搜索关键词数�?const searchKeywords = ref<any[]>([])
const searchKeywordsLoading = ref(false)

// 地区分布数据
const regions = ref<any>({ items: [] })
const regionsLoading = ref(false)

// 客户端分布数�?const clientDistribution = ref<any>({
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


// 趋势图数据（使用新的 trend 接口�?// 趋势图配�?(ECharts) - Aurora DS Neon Style
const trendLineOption = computed(() => {
  const points = trendData.value?.points || trendData.value?.Points || []
  if (!trendData.value || points.length === 0) return null
  
  const labels = points.map((p: any) => {
    const dateStr = p.date || p.Date || ''
    return dateStr.split(' ')[0].slice(5) // MM-DD
  })

  const primaryColor = getCssVar('--chart-primary')
  const secondaryColor = getCssVar('--chart-secondary')

  // 构建基础配置
  const baseConfig = buildNeonLineOptions('--chart-primary', {
    xAxis: {
      type: 'category',
      data: labels,
      boundaryGap: false
    },
    yAxis: {
      type: 'value'
    },
    series: []
  })

  // 手动构建多系列霓虹效�?  const option = {
    ...baseConfig,
    series: [
      // 浏览�?- 主色
      {
        name: '浏览�?,
        data: points.map((p: any) => p.pv || p.Pv || 0),
        type: 'line',
        smooth: true,
        symbol: 'circle',
        symbolSize: 8,
        lineStyle: {
          width: 3,
          color: primaryColor,
          shadowBlur: 12,
          shadowColor: `${primaryColor}aa`
        },
        itemStyle: {
          color: primaryColor,
          shadowBlur: 12,
          shadowColor: `${primaryColor}aa`
        },
        areaStyle: {
          color: {
            type: 'linear',
            x: 0,
            y: 0,
            x2: 0,
            y2: 1,
            colorStops: [
              { offset: 0, color: `${primaryColor}55` },
              { offset: 1, color: 'transparent' }
            ]
          }
        }
      },
      // 访客�?- 次色
      {
        name: '访客�?,
        data: points.map((p: any) => p.uv || p.Uv || 0),
        type: 'line',
        smooth: true,
        symbol: 'circle',
        symbolSize: 8,
        lineStyle: {
          width: 3,
          color: secondaryColor,
          shadowBlur: 12,
          shadowColor: `${secondaryColor}aa`
        },
        itemStyle: {
          color: secondaryColor,
          shadowBlur: 12,
          shadowColor: `${secondaryColor}aa`
        },
        areaStyle: {
          color: {
            type: 'linear',
            x: 0,
            y: 0,
            x2: 0,
            y2: 1,
            colorStops: [
              { offset: 0, color: `${secondaryColor}55` },
              { offset: 1, color: 'transparent' }
            ]
          }
        }
      }
    ]
  }
  
  // 应用主题
  return applyTheme(option)
})

// 访问区域条形图选项 - Aurora Neon
const regionBarOption = computed(() => {
  if (!regions.value?.items?.length) return null
  
  const items = regions.value.items.map((r: any) => ({
    name: r.province ? `${r.country}-${r.province}` : r.country,
    value: r.count || 0
  })).sort((a: any, b: any) => b.value - a.value).slice(0, 10)

  // 使用渐变柱状图辅助函�?  const baseConfig = buildNeonBarOptions('--chart-primary', '--chart-secondary', {
    xAxis: { type: 'value' },
    yAxis: { type: 'category', data: items.map((i: any) => i.name).reverse() },
    series: []
  })

  const option = {
    ...baseConfig,
    series: [{
      ...baseConfig.series,
      data: items.map((i: any) => i.value).reverse(),
      label: { show: true, position: 'right', ...(getCssVar('--color-text-muted') ? { color: getCssVar('--color-text-muted') } : {}) }
    }]
  }
  
  // 应用主题
  return applyTheme(option)
})

// 设备类型 Donut - Aurora Neon
const deviceDonutOption = computed(() => {
  if (!clientDistribution.value?.devices?.length) return null
  const data = clientDistribution.value.devices.map((d: any, idx: number) => ({
    name: d.name || '未知',
    value: d.count || 0,
    colorVar: `--chart-${['primary','secondary','tertiary','quaternary','quinary'][idx % 5]}`
  }))
  
  const total = data.reduce((s: number, i: any) => s + i.value, 0)
  const max = data.reduce((m: any, i: any) => i.value > m.value ? i : m, data[0])
  
  // 构建完整�?ECharts option
  const donutSeries = buildNeonDonutOptions(data)
  const textColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const tooltipBg = getCssVar('--color-bg-card') || getCssVar('--n-card-color')
  const tooltipBorder = getCssVar('--color-border-default') || getCssVar('--n-border-color')
  
  const legendTextColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const legendBottom = getCssVar('--analytics-legend-bottom')
  const legendItemGapStr = getCssVar('--analytics-legend-item-gap')
  const legendFontSizeStr = getCssVar('--analytics-legend-font-size')
  const legendIconWidthStr = getCssVar('--analytics-legend-icon-width')
  const legendIconHeightStr = getCssVar('--analytics-legend-icon-height')
  const legendItemGap = legendItemGapStr ? parseInt(legendItemGapStr, 10) : undefined
  const legendFontSize = legendFontSizeStr ? parseInt(legendFontSizeStr, 10) : undefined
  const legendIconWidth = legendIconWidthStr ? parseInt(legendIconWidthStr, 10) : undefined
  const legendIconHeight = legendIconHeightStr ? parseInt(legendIconHeightStr, 10) : undefined
  
  // 调整 series �?center，为图例留出空间
  const adjustedSeries = {
    ...donutSeries,
    center: ['50%', '45%'], // 向上移动图表，为底部图例留空�?    radius: ['50%', '70%'] // 稍微缩小半径，确保不超出
  }
  
  const fullOption = {
    tooltip: {
      trigger: 'item',
      formatter: '{b}: {c} ({d}%)',
      ...(tooltipBg ? { backgroundColor: tooltipBg } : {}),
      ...(tooltipBorder ? { borderColor: tooltipBorder } : {}),
      textStyle: {
        ...(textColor ? { color: textColor } : {})
      }
    },
    legend: {
      show: true,
      orient: 'horizontal',
      bottom: '5%',
      left: 'center',
      ...(legendItemGap !== undefined ? { itemGap: legendItemGap } : {}),
      textStyle: {
        ...(legendTextColor ? { color: legendTextColor } : {}),
        ...(legendFontSize !== undefined ? { fontSize: legendFontSize } : {})
      },
      icon: 'rect',
      ...(legendIconWidth !== undefined ? { itemWidth: legendIconWidth } : {}),
      ...(legendIconHeight !== undefined ? { itemHeight: legendIconHeight } : {})
    },
    series: [adjustedSeries]
  }
  
  // 应用主题
  const themedOption = applyTheme(fullOption)
  
  return {
    option: themedOption,
    mainLabel: max.name,
    mainPercent: total > 0 ? ((max.value / total) * 100).toFixed(1) : '0'
  }
})

// 浏览�?Donut - Aurora Neon
const browserDonutOption = computed(() => {
  if (!clientDistribution.value?.browsers?.length) return null
  const data = clientDistribution.value.browsers.map((d: any, idx: number) => ({
    name: d.name || '未知',
    value: d.count || 0,
    colorVar: `--chart-${['quaternary','quinary','primary','secondary','tertiary'][idx % 5]}`
  }))
  
  const total = data.reduce((s: number, i: any) => s + i.value, 0)
  const max = data.reduce((m: any, i: any) => i.value > m.value ? i : m, data[0])
  
  // 构建完整�?ECharts option
  const donutSeries = buildNeonDonutOptions(data)
  const textColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const tooltipBg = getCssVar('--color-bg-card') || getCssVar('--n-card-color')
  const tooltipBorder = getCssVar('--color-border-default') || getCssVar('--n-border-color')
  
  const legendTextColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const legendBottom = getCssVar('--analytics-legend-bottom')
  const legendItemGapStr = getCssVar('--analytics-legend-item-gap')
  const legendFontSizeStr = getCssVar('--analytics-legend-font-size')
  const legendIconWidthStr = getCssVar('--analytics-legend-icon-width')
  const legendIconHeightStr = getCssVar('--analytics-legend-icon-height')
  const legendItemGap = legendItemGapStr ? parseInt(legendItemGapStr, 10) : undefined
  const legendFontSize = legendFontSizeStr ? parseInt(legendFontSizeStr, 10) : undefined
  const legendIconWidth = legendIconWidthStr ? parseInt(legendIconWidthStr, 10) : undefined
  const legendIconHeight = legendIconHeightStr ? parseInt(legendIconHeightStr, 10) : undefined
  
  // 调整 series �?center，为图例留出空间
  const adjustedSeries = {
    ...donutSeries,
    center: ['50%', '45%'], // 向上移动图表，为底部图例留空�?    radius: ['50%', '70%'] // 稍微缩小半径，确保不超出
  }
  
  const fullOption = {
    tooltip: {
      trigger: 'item',
      formatter: '{b}: {c} ({d}%)',
      ...(tooltipBg ? { backgroundColor: tooltipBg } : {}),
      ...(tooltipBorder ? { borderColor: tooltipBorder } : {}),
      textStyle: {
        ...(textColor ? { color: textColor } : {})
      }
    },
    legend: {
      show: true,
      orient: 'horizontal',
      bottom: '5%',
      left: 'center',
      ...(legendItemGap !== undefined ? { itemGap: legendItemGap } : {}),
      textStyle: {
        ...(legendTextColor ? { color: legendTextColor } : {}),
        ...(legendFontSize !== undefined ? { fontSize: legendFontSize } : {})
      },
      icon: 'rect',
      ...(legendIconWidth !== undefined ? { itemWidth: legendIconWidth } : {}),
      ...(legendIconHeight !== undefined ? { itemHeight: legendIconHeight } : {})
    },
    series: [adjustedSeries]
  }
  
  // 应用主题
  const themedOption = applyTheme(fullOption)
  
  return {
    option: themedOption,
    mainLabel: max.name,
    mainPercent: total > 0 ? ((max.value / total) * 100).toFixed(1) : '0'
  }
})

// 操作系统 Donut - Aurora Neon
const osDonutOption = computed(() => {
  if (!clientDistribution.value?.os?.length) return null
  const data = clientDistribution.value.os.map((d: any, idx: number) => ({
    name: d.name || '未知',
    value: d.count || 0,
    colorVar: `--chart-${['tertiary','quaternary','quinary','primary','secondary'][idx % 5]}`
  }))
  
  const total = data.reduce((s: number, i: any) => s + i.value, 0)
  const max = data.reduce((m: any, i: any) => i.value > m.value ? i : m, data[0])
  
  // 构建完整�?ECharts option
  const donutSeries = buildNeonDonutOptions(data)
  const textColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const tooltipBg = getCssVar('--color-bg-card') || getCssVar('--n-card-color')
  const tooltipBorder = getCssVar('--color-border-default') || getCssVar('--n-border-color')
  
  const legendTextColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const legendBottom = getCssVar('--analytics-legend-bottom')
  const legendItemGapStr = getCssVar('--analytics-legend-item-gap')
  const legendFontSizeStr = getCssVar('--analytics-legend-font-size')
  const legendIconWidthStr = getCssVar('--analytics-legend-icon-width')
  const legendIconHeightStr = getCssVar('--analytics-legend-icon-height')
  const legendItemGap = legendItemGapStr ? parseInt(legendItemGapStr, 10) : undefined
  const legendFontSize = legendFontSizeStr ? parseInt(legendFontSizeStr, 10) : undefined
  const legendIconWidth = legendIconWidthStr ? parseInt(legendIconWidthStr, 10) : undefined
  const legendIconHeight = legendIconHeightStr ? parseInt(legendIconHeightStr, 10) : undefined
  
  // 调整 series �?center，为图例留出空间
  const adjustedSeries = {
    ...donutSeries,
    center: ['50%', '45%'], // 向上移动图表，为底部图例留空�?    radius: ['50%', '70%'] // 稍微缩小半径，确保不超出
  }
  
  const fullOption = {
    tooltip: {
      trigger: 'item',
      formatter: '{b}: {c} ({d}%)',
      ...(tooltipBg ? { backgroundColor: tooltipBg } : {}),
      ...(tooltipBorder ? { borderColor: tooltipBorder } : {}),
      textStyle: {
        ...(textColor ? { color: textColor } : {})
      }
    },
    legend: {
      show: true,
      orient: 'horizontal',
      bottom: '5%',
      left: 'center',
      ...(legendItemGap !== undefined ? { itemGap: legendItemGap } : {}),
      textStyle: {
        ...(legendTextColor ? { color: legendTextColor } : {}),
        ...(legendFontSize !== undefined ? { fontSize: legendFontSize } : {})
      },
      icon: 'rect',
      ...(legendIconWidth !== undefined ? { itemWidth: legendIconWidth } : {}),
      ...(legendIconHeight !== undefined ? { itemHeight: legendIconHeight } : {})
    },
    series: [adjustedSeries]
  }
  
  // 应用主题
  const themedOption = applyTheme(fullOption)
  
  return {
    option: themedOption,
    mainLabel: max.name,
    mainPercent: total > 0 ? ((max.value / total) * 100).toFixed(1) : '0'
  }
})

// 来源分析 Donut - Aurora Neon
const sourceDonutOption = computed(() => {
  if (!sources.value?.items?.length) return null
  const data = sources.value.items.map((d: any, idx: number) => ({
    name: d.name || '未知',
    value: d.count || 0,
    colorVar: `--chart-${['secondary','tertiary','quaternary','quinary','primary'][idx % 5]}`
  }))
  
  const total = data.reduce((s: number, i: any) => s + i.value, 0)
  const max = data.reduce((m: any, i: any) => i.value > m.value ? i : m, data[0])
  
  // 构建完整�?ECharts option
  const donutSeries = buildNeonDonutOptions(data)
  const textColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const tooltipBg = getCssVar('--color-bg-card') || getCssVar('--n-card-color')
  const tooltipBorder = getCssVar('--color-border-default') || getCssVar('--n-border-color')
  
  const legendTextColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const legendBottom = getCssVar('--analytics-legend-bottom')
  const legendItemGapStr = getCssVar('--analytics-legend-item-gap')
  const legendFontSizeStr = getCssVar('--analytics-legend-font-size')
  const legendIconWidthStr = getCssVar('--analytics-legend-icon-width')
  const legendIconHeightStr = getCssVar('--analytics-legend-icon-height')
  const legendItemGap = legendItemGapStr ? parseInt(legendItemGapStr, 10) : undefined
  const legendFontSize = legendFontSizeStr ? parseInt(legendFontSizeStr, 10) : undefined
  const legendIconWidth = legendIconWidthStr ? parseInt(legendIconWidthStr, 10) : undefined
  const legendIconHeight = legendIconHeightStr ? parseInt(legendIconHeightStr, 10) : undefined
  
  // 调整 series �?center，为图例留出空间
  const adjustedSeries = {
    ...donutSeries,
    center: ['50%', '45%'], // 向上移动图表，为底部图例留空�?    radius: ['50%', '70%'] // 稍微缩小半径，确保不超出
  }
  
  const fullOption = {
    tooltip: {
      trigger: 'item',
      formatter: '{b}: {c} ({d}%)',
      ...(tooltipBg ? { backgroundColor: tooltipBg } : {}),
      ...(tooltipBorder ? { borderColor: tooltipBorder } : {}),
      textStyle: {
        ...(textColor ? { color: textColor } : {})
      }
    },
    legend: {
      show: true,
      orient: 'horizontal',
      bottom: '5%',
      left: 'center',
      ...(legendItemGap !== undefined ? { itemGap: legendItemGap } : {}),
      textStyle: {
        ...(legendTextColor ? { color: legendTextColor } : {}),
        ...(legendFontSize !== undefined ? { fontSize: legendFontSize } : {})
      },
      icon: 'rect',
      ...(legendIconWidth !== undefined ? { itemWidth: legendIconWidth } : {}),
      ...(legendIconHeight !== undefined ? { itemHeight: legendIconHeight } : {})
    },
    series: [adjustedSeries]
  }
  
  // 应用主题
  const themedOption = applyTheme(fullOption)
  
  return {
    option: themedOption,
    mainLabel: max.name,
    mainPercent: total > 0 ? ((max.value / total) * 100).toFixed(1) : '0'
  }
})

// 数据存在性判断（用于显示"暂无数据"�?const hasTrendData = computed(() => {
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

// 计算总数用于百分�?const totalRegionCount = computed(() => {
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

// 判断是否显示"暂无访客数据"提示（只有所有数据都为空时才显示�?const showNoDataAlert = computed(() => {
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
  // 如果正在加载，跳过本次请�?  if (statsLoading.value) {
    return
  }
  
  try {
    statsLoading.value = true
    statsError.value = null
    const res = await api.get<any>('/Analytics/stats')
    
    // 成功获取数据，重置重试计�?    rateLimitRetryCount.value = 0
    
    if (res) {
      // 确保所有字段都有默认�?      stats.value = {
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
      
      // 如果连续遇到速率限制，停止自动刷�?      if (rateLimitRetryCount.value >= 3) {
        if (autoRefreshInterval.value) {
          clearInterval(autoRefreshInterval.value)
          autoRefreshInterval.value = null
        }
        if (process.client) {
          alert('请求过于频繁，已自动停止刷新。请稍后再试或手动刷新�?)
        }
        return
      }
    } else {
      // 其他错误，显示提示但不阻止后续请�?      statsError.value = e.message || '获取数据失败'
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
      // useApi 已经提取�?data 字段，所�?res 应该�?{ total: 39, page: 1, pageSize: 20, visitors: [...] }
      // 注意：后端返回的是小写的 "visitors" �?"total"，优先使用小�?      const visitorsData = res.visitors || res.Visitors || (Array.isArray(res) ? res : [])
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

// 智能转换英文单词为中文（常见词汇映射�?const translateWord = (word: string): string => {
  const wordLower = word.toLowerCase()
  
  // 常见英文单词到中文的映射（作为后备，主要用于无法智能识别的词�?  const wordMap: Record<string, string> = {
    'dashboard': '仪表�?,
    'home': '首页',
    'index': '首页',
    'blog': '博客',
    'article': '文章',
    'tools': '工具',
    'projects': '项目',
    'life': '生活',
    'lab': '实验�?,
    'ai': 'AI实验�?,
    'admin': '管理后台',
    'about': '关于',
    'contact': '联系',
    'search': '搜索',
    'profile': '个人资料',
    'settings': '设置',
    'account': '账户',
    'login': '登录',
    'register': '注册',
    'logout': '退�?,
    'analytics': '分析',
    'articles': '文章',
    'categories': '分类',
    'timeline': '时间�?,
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
  
  // 智能识别：处理连字符和驼峰命�?  // 例如：forgot-password -> 忘记密码, userProfile -> 用户资料
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

// 格式化路径名称，智能识别并转换为友好的中文描�?const formatPathName = (path: string): string => {
  if (!path) return '未知页面'
  
  // 移除前缀和查询参�?  const cleanPath = path.replace('landing:', '').replace('page:', '').trim()
  const pathWithoutQuery = cleanPath.split('?')[0].split('#')[0]
  
  // 处理空路径或根路�?  if (!pathWithoutQuery || pathWithoutQuery === '/') {
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
    // 单级路径：直接返回翻译后的名�?    return translatedParts[0]
  } else if (parts.length === 2) {
    // 二级路径：分类：内容
    // 如果第二部分看起来像 slug（包含连字符、数字等），显示�?分类：内�?
    const secondPart = parts[1]
    const isSlug = /^[a-z0-9-]+$/.test(secondPart.toLowerCase()) && secondPart.length > 10
    
    if (isSlug) {
      // 截断过长�?slug
      const displaySlug = secondPart.length > 25 ? secondPart.substring(0, 25) + '...' : secondPart
      return `${translatedParts[0]}�?{displaySlug}`
    } else {
      // 如果第二部分也是可识别的单词，翻译它
      return `${translatedParts[0]}�?{translatedParts[1]}`
    }
  } else {
    // 多级路径：分类：子分�?内容
    const lastPart = parts[parts.length - 1]
    const isSlug = /^[a-z0-9-]+$/.test(lastPart.toLowerCase()) && lastPart.length > 10
    
    if (isSlug) {
      // 最后一部分�?slug，显示为"分类：路�?内容"
      const displaySlug = lastPart.length > 20 ? lastPart.substring(0, 20) + '...' : lastPart
      const middleParts = translatedParts.slice(1, -1).join('/')
      return `${translatedParts[0]}�?{middleParts ? middleParts + '/' : ''}${displaySlug}`
    } else {
      // 所有部分都可识别，全部翻译
      return translatedParts.join('�?)
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
  
  // 根据路径类型返回友好的名�?  if (cleanUrl.startsWith('blog/')) {
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
    return '实验�?
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
    // 即使出错也设置空数据，避免显示错�?    trendData.value = { points: [] }
  } finally {
    trendLoading.value = false
  }
}

// 获取概览数据
const fetchOverview = async () => {
  try {
    const res = await api.get<any>('/Analytics/overview')
    console.log('概览数据响应:', res) // 调试�?    if (res) {
      // 确保数据正确映射（后端返回的字段名）
      overview.value = {
        todayPv: res.todayPv ?? 0,
        todayUv: res.todayUv ?? 0,
        yesterdayPv: res.yesterdayPv ?? 0,
        yesterdayUv: res.yesterdayUv ?? 0,
        totalPv: res.totalPv ?? 0,
        totalUv: res.totalUv ?? 0,
        onlineUsers: res.onlineUsers ?? 0,
        hotArticleCount: res.hotArticleCount ?? 0
      }
      console.log('概览数据已设�?', overview.value) // 调试�?    } else {
      console.warn('概览数据响应为空')
    }
  } catch (e: any) {
    console.error('获取概览数据失败:', e)
    // 静默失败，保持默认�?0
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

// 获取搜索关键�?const fetchSearchKeywords = async () => {
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

// 获取客户端分�?const fetchClientDistribution = async () => {
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

// 统一刷新所有数�?const refreshAll = async () => {
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
    // 即使出错也标记为完成，避免一直显示加载状�?    initialLoadComplete.value = true
  }
}

// 时间范围变化时刷新数�?watch(selectedRange, (newRange) => {
  // 趋势图使用独立的 range，但需要同步（today 映射�?7d�?  trendRange.value = newRange === 'today' ? '7d' : newRange as any
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
const autoRefreshEnabled = ref(true) // 实时刷新开�?
// 获取 Donut 图表颜色的辅助函�?const getDonutColor = (index: number): string => {
  const colorVars = ['--chart-primary', '--chart-secondary', '--chart-tertiary', '--chart-quaternary', '--chart-quinary']
  const colorVar = colorVars[index % colorVars.length]
  return getCssVar(colorVar)
}

// 监听实时刷新开�?watch(autoRefreshEnabled, (enabled) => {
  if (enabled) {
    // 开启实时刷�?    if (process.client && !autoRefreshInterval.value) {
      autoRefreshInterval.value = setInterval(() => {
        if (rateLimitRetryCount.value >= 3) {
          if (autoRefreshInterval.value) {
            clearInterval(autoRefreshInterval.value)
            autoRefreshInterval.value = null
          }
          return
        }
        fetchOverview()
        fetchStats()
        fetchVisitors()
      }, 60000)
    }
  } else {
    // 关闭实时刷新
    if (autoRefreshInterval.value) {
      clearInterval(autoRefreshInterval.value)
      autoRefreshInterval.value = null
    }
  }
})

onMounted(() => {
  if (process.client) {
    const token = localStorage.getItem('admin_token')
    
    if (!token) {
      alert('请先登录管理员账号才能查看访客数据！\n\n将跳转到登录页面...')
      navigateTo('/admin/login')
      return
    }
  }
  
  // 延迟一下再加载数据，确保页面完全渲�?  setTimeout(() => {
    refreshAll()
  }, 500)
  
  // 如果实时刷新开关开启，启动自动刷新
  if (process.client && autoRefreshEnabled.value) {
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
      // 自动刷新访客列表（无论是否勾�?仅显示在线访�?�?      fetchVisitors()
    }, 60000) // 60 �?  }
})

// 页面卸载时清理定时器
onUnmounted(() => {
  if (autoRefreshInterval.value) {
    clearInterval(autoRefreshInterval.value)
    autoRefreshInterval.value = null
  }
})
</script>

<style scoped>
.donut-center {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  pointer-events: none;
  z-index: 1;
}
.donut-center-value {
  font-size: 24px;
  font-weight: 700;
  color: var(--color-text-main, var(--n-text-color));
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
  line-height: 1.2;
}
.donut-center-label {
  font-size: 13px;
  font-weight: 500;
  color: var(--color-text-main, var(--n-text-color));
  opacity: 0.85;
  margin-top: 6px;
  text-shadow: 0 1px 4px rgba(0, 0, 0, 0.2);
}

/* 分析页面列表项样�?- 使用 CSS 变量 */
:root {
  --analytics-list-item-bg: var(--color-bg-surface-2, var(--n-card-color));
  --analytics-list-item-bg-hover: var(--color-bg-surface-1, var(--n-card-color-hover));
  --analytics-list-item-padding-sm: 0.5rem;
  --analytics-list-item-padding-md: 0.75rem;
  --analytics-list-item-border-radius: 0.5rem;
  --analytics-list-item-gap: 0.5rem;
  
  /* 图表图例样式变量 */
  --analytics-donut-chart-height: 20rem;
  --analytics-legend-height: 3rem;
  --analytics-legend-item-gap: 20;
  --analytics-legend-font-size: 12;
  --analytics-legend-icon-width: 12;
  --analytics-legend-icon-height: 12;
}

[data-theme='dark'] {
  --analytics-list-item-bg: rgba(30, 35, 50, 0.4);
  --analytics-list-item-bg-hover: rgba(40, 45, 60, 0.6);
}

[data-theme='light'] {
  --analytics-list-item-bg: rgba(248, 250, 252, 0.8);
  --analytics-list-item-bg-hover: rgba(241, 245, 249, 0.9);
}

/* 访问来源列表�?*/
.analytics-source-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--analytics-list-item-padding-sm);
  background-color: var(--analytics-list-item-bg);
  border-radius: var(--analytics-list-item-border-radius);
  transition: background-color 0.2s ease;
}

.analytics-source-item:hover {
  background-color: var(--analytics-list-item-bg-hover);
}

/* Top 10 页面列表�?*/
.analytics-page-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--analytics-list-item-padding-md);
  background-color: var(--analytics-list-item-bg);
  border-radius: var(--analytics-list-item-border-radius);
  transition: background-color 0.2s ease;
}

.analytics-page-item:hover {
  background-color: var(--analytics-list-item-bg-hover);
}

</style>

