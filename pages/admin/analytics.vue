<template>
  <!-- 
    ?????? - Aurora Design System
    moduleId: analytics_dashboard
  -->
  <div 
    :data-module-theme="moduleTheme || undefined"
    class="min-h-screen p-6 lg:p-10 text-text-main transition-colors duration-500"
  >
    <!-- Header: ??????+ ??????????? -->
    <div class="flex justify-between items-center mb-8">
      <div>
        <h1 class="text-3xl font-bold bg-gradient-to-r from-primary via-purple-500 to-secondary text-transparent bg-clip-text">
          ????
        </h1>
        <p class="text-sm text-text-muted mt-2">
          ??????????????        </p>
      </div>
      <div class="flex items-center gap-3">
        <label class="flex items-center gap-2 text-sm text-text-muted cursor-pointer">
          <input
            type="checkbox"
            v-model="autoRefreshEnabled"
            class="rounded"
          />
          ????
        </label>
        <AppButton variant="primary" @click="refreshStats">
          ????
        </AppButton>
      </div>
    </div>

    <!-- ????????????????????? -->
    <template v-if="!initialLoadComplete">
      <AppCard class="mb-6">
        <div class="flex items-center justify-center py-12">
          <div class="text-center">
            <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary mb-4"></div>
            <p class="text-text-muted">??????...</p>
          </div>
        </div>
      </AppCard>
    </template>

    <!-- ????????????????????-->
    <template v-else>
      <!-- ???????????????? -->
      <AppCard v-if="showNoDataAlert" class="mb-6 border-2 border-chart-tertiary/50 bg-chart-tertiary/10 p-4">
        <div class="flex items-start">
          <div class="flex-shrink-0">
            <svg class="h-5 w-5 text-chart-tertiary" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
            </svg>
          </div>
          <div class="ml-3 flex-1">
            <!-- ????????????-->
            <h3 class="text-sm text-heading mb-2">??????</h3>
            <div class="mt-2 text-sm text-body leading-relaxed">
              <p class="mb-2 font-medium">?????????????????</p>
              <ul class="list-disc list-inside mt-1 space-y-1 ml-2">
                <li>?????????</li>
                <!-- ?? bg-code ???????????? -->
                <li>????????<code class="bg-code">VisitLogs</code> ?????????</li>
                <li>???????VPN?????????IP??</li>
              </ul>
              <p class="mt-3 mb-2 font-medium">
                <strong class="text-body"></strong>?????????????????????              </p>
              <ul class="list-disc list-inside mt-1 space-y-1 ml-2">
                <li>??????????????????</li>
                <li>??"????"????????</li>
                <li>?????????????</li>
              </ul>
            </div>
          </div>
        </div>
      </AppCard>

    <!-- ?????????? (Bento Grid) -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
      <!-- PV ?? -->
      <AppCard hover class="relative overflow-hidden group backdrop-blur-xl">
        <!-- ????????-->
        <div class="absolute top-0 right-0 w-32 h-32 bg-primary/10 blur-3xl -mr-16 -mt-16 opacity-60 group-hover:opacity-100 transition-opacity"></div>
        <div class="relative z-10 p-6">
          <div class="text-sm text-text-muted mb-2">?????</div>
          <div class="text-3xl font-bold mb-2" style="color: var(--color-primary, var(--color-primary));">
            {{ overview.todayPv ?? 0 }}
          </div>
          <div class="text-xs text-text-muted">
            ??: {{ overview.yesterdayPv ?? 0 }} | ??: {{ overview.totalPv ?? 0 }}
          </div>
        </div>
      </AppCard>

      <!-- UV ?? -->
      <AppCard hover class="relative overflow-hidden group backdrop-blur-xl">
        <div class="absolute top-0 right-0 w-32 h-32 bg-chart-secondary/10 blur-3xl -mr-16 -mt-16 opacity-60 group-hover:opacity-100 transition-opacity"></div>
        <div class="relative z-10 p-6">
          <div class="text-sm text-text-muted mb-2">?????</div>
          <div class="text-3xl font-bold mb-2" style="color: var(--chart-secondary, var(--color-success));">
            {{ overview.todayUv ?? 0 }}
          </div>
          <div class="text-xs text-text-muted">
            ??: {{ overview.yesterdayUv || 0 }} | ??: {{ overview.totalUv || 0 }}
          </div>
        </div>
      </AppCard>

      <!-- ?????? -->
      <AppCard hover class="relative overflow-hidden group backdrop-blur-xl">
        <div class="absolute top-0 right-0 w-32 h-32 bg-chart-tertiary/10 blur-3xl -mr-16 -mt-16 opacity-60 group-hover:opacity-100 transition-opacity"></div>
        <div class="relative z-10 p-6">
          <div class="text-sm text-text-muted mb-2">????</div>
          <div class="text-3xl font-bold mb-2" style="color: var(--chart-tertiary, var(--color-warning));">
            {{ overview.onlineUsers ?? 0 }}
          </div>
          <div class="text-xs text-text-muted">???????</div>
        </div>
      </AppCard>

      <!-- ??????-->
      <AppCard hover class="relative overflow-hidden group backdrop-blur-xl">
        <div class="absolute top-0 right-0 w-32 h-32 bg-chart-quinary/10 blur-3xl -mr-16 -mt-16 opacity-60 group-hover:opacity-100 transition-opacity"></div>
        <div class="relative z-10 p-6">
          <div class="text-sm text-text-muted mb-2">?????</div>
          <div class="text-3xl font-bold mb-2" style="color: var(--chart-quinary, var(--color-purple-500));">
            {{ overview.hotArticleCount ?? 0 }}
          </div>
          <div class="text-xs text-text-muted">???? > 1</div>
        </div>
      </AppCard>
    </div>

    <!-- ????????(??) -->
    <AppCard hover class="mb-6 p-6 backdrop-blur-xl">
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-xl font-bold text-text-main">???/?????</h2>
        <div class="flex gap-2">
          <AppButton
            :variant="trendRange === '7d' ? 'primary' : 'secondary'"
            size="sm"
            @click="trendRange = '7d'; selectedRange = '7d'"
          >
            7?</AppButton>
          <AppButton
            :variant="trendRange === '30d' ? 'primary' : 'secondary'"
            size="sm"
            @click="trendRange = '30d'; selectedRange = '30d'"
          >
            30?</AppButton>
          <AppButton
            :variant="trendRange === '90d' ? 'primary' : 'secondary'"
            size="sm"
            @click="trendRange = '90d'; selectedRange = '90d'"
          >
            90?</AppButton>
        </div>
      </div>
      <div v-if="trendLoading" class="text-center py-8 text-text-muted">
        ????..
      </div>
      <ClientOnly>
        <template v-if="hasTrendData && trendLineOption">
          <div class="h-[500px] relative w-full">
            <v-chart :option="trendLineOption" autoresize class="w-full h-full" />
          </div>
        </template>
        <template v-else>
          <div class="text-center py-8 text-text-muted h-[500px] flex items-center justify-center">
            {{ trendLoading ? '????..' : '??????' }}
          </div>
        </template>
        <template #fallback>
          <div class="h-[500px] flex items-center justify-center">
            <div class="text-center">
              <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary mb-4"></div>
              <p class="text-text-muted">????..</p>
            </div>
          </div>
        </template>
      </ClientOnly>
    </AppCard>

    <!-- ???????? (Bento Grid: ??4??+ ??8?? -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-6 mb-6">
      <!-- ??????- ?? Donut ?? -->
      <div class="lg:col-span-4 space-y-6">
        <!-- ???? Donut -->
        <AppCard hover class="p-6 backdrop-blur-xl">
          <h2 class="text-lg font-bold text-text-main mb-4">????</h2>
          <div v-if="sourcesLoading" class="text-center py-4 text-text-muted">????..</div>
          <div v-else-if="!sources.items || sources.items.length === 0" class="text-center py-4 text-text-muted">????</div>
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
                <div class="text-center text-text-muted py-8 h-48 flex items-center justify-center">????</div>
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

        <!-- ???? Donut -->
        <AppCard hover class="p-6 backdrop-blur-xl">
          <h2 class="text-lg font-bold text-text-main mb-4">??????</h2>
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
              <div class="text-center text-text-muted py-8">????</div>
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

      <!-- ??????- ???? + ???? -->
      <div class="lg:col-span-8 space-y-6">
        <!-- ??????? -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Top 10 ?? -->
          <AppCard hover class="p-6 backdrop-blur-xl">
            <div class="flex justify-between items-center mb-4">
              <h2 class="text-lg font-bold text-text-main">Top 10 ??</h2>
              <select v-model="selectedRange" class="text-sm px-2 py-1 rounded border border-border-subtle bg-bg-surface-2 text-text-main">
                <option value="today">??</option>
                <option value="7d">7?</option>
                <option value="30d">30?</option>
                <option value="90d">90?</option>
              </select>
            </div>
            <div v-if="topPagesLoading" class="text-center py-4 text-text-muted">????..</div>
            <div v-else-if="topPages.length === 0" class="text-center py-4 text-text-muted">????</div>
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
                    <span>???? <span class="font-semibold text-primary">{{ page.pv }}</span></span>
                    <span>???? <span class="font-semibold text-chart-secondary">{{ page.uv }}</span></span>
                  </div>
                </div>
                <div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center text-primary font-bold text-sm ml-3 flex-shrink-0">
                  {{ index + 1 }}
                </div>
              </div>
            </div>
          </AppCard>

          <!-- ????????-->
          <AppCard hover class="p-6 backdrop-blur-xl">
            <h2 class="text-lg font-bold text-text-main mb-4">????</h2>
            <ClientOnly>
              <template v-if="hasRegionData && regionBarOption">
                <div class="h-64 w-full">
                  <v-chart :option="regionBarOption" autoresize class="w-full h-full" />
                </div>
              </template>
              <template v-else>
                <div class="text-center text-text-muted py-8 h-64 flex items-center justify-center">????</div>
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

        <!-- ??????????-->
        <AppCard hover class="p-6 backdrop-blur-xl">
          <div class="flex justify-between items-center mb-4">
            <h2 class="text-lg font-bold text-text-main">????</h2>
            <div class="flex items-center gap-4">
              <label class="flex items-center gap-2 text-sm text-text-muted">
                <input
                  type="checkbox"
                  v-model="onlineOnly"
                  @change="fetchVisitors"
                  class="rounded"
                />
                ????????              </label>
              <AppButton variant="secondary" size="sm" @click="fetchVisitors">
                ????
              </AppButton>
            </div>
          </div>

          <div v-if="visitorsLoading" class="text-center py-8 text-text-muted">
            ????..
          </div>
          <div v-else-if="visitors.length === 0" class="text-center py-8 text-text-muted">
            ??????
          </div>
          <div v-else class="overflow-x-auto -mx-6 px-6">
            <table class="w-full text-sm border-collapse">
              <thead>
                <tr class="border-b border-border-subtle">
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase whitespace-nowrap">??ID</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase whitespace-nowrap">IP??</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase whitespace-nowrap">????</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase whitespace-nowrap">????</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase whitespace-nowrap">????</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase whitespace-nowrap">???</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase whitespace-nowrap">????</th>
                  <th class="px-4 py-3 text-left text-xs font-medium text-text-muted uppercase whitespace-nowrap">??</th>
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
                    {{ (visitor.ip || visitor.Ip) && (visitor.ip || visitor.Ip) !== '-' ? (visitor.ip || visitor.Ip) : '??' }}
                  </td>
                  <td class="px-4 py-3 text-text-main">
                    <div class="text-xs">
                      <div v-if="visitor.country || visitor.Country">{{ visitor.country || visitor.Country }}</div>
                      <div v-if="visitor.region || visitor.Region" class="text-text-muted">{{ visitor.region || visitor.Region }}</div>
                      <div v-if="visitor.city || visitor.City" class="text-text-muted">{{ visitor.city || visitor.City }}</div>
                      <div v-if="!(visitor.country || visitor.Country) && !(visitor.region || visitor.Region) && !(visitor.city || visitor.City)" class="text-text-disabled">??</div>
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
                      ??: {{ visitor.searchKeyword || visitor.SearchKeyword }}
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
                      ??
                    </span>
                    <span
                      v-else
                      class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-bg-surface-2 text-text-muted"
                    >
                      ??
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>

            <!-- ?? -->
            <div v-if="visitorsTotal > pageSize" class="mt-4 flex items-center justify-between">
              <div class="text-sm text-text-muted">
                ??{{ visitorsTotal }} ????              </div>
              <div class="flex gap-2">
                <AppButton
                  variant="secondary"
                  size="sm"
                  @click="changePage(visitorsPage - 1)"
                  :disabled="visitorsPage <= 1"
                >
                  ????                </AppButton>
                <span class="px-3 py-1 text-sm text-text-main">
                  ??{{ visitorsPage }} / {{ Math.ceil(visitorsTotal / pageSize) }} ??                </span>
                <AppButton
                  variant="secondary"
                  size="sm"
                  @click="changePage(visitorsPage + 1)"
                  :disabled="visitorsPage >= Math.ceil(visitorsTotal / pageSize)"
                >
                  ????                </AppButton>
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
// ???? NNumberAnimation????????
// ??setup ???? useEChartsTheme???? computed ??????// ????????ssr: false????????????
const { isDark, applyTheme, buildNeonLineOptions, buildNeonBarOptions, buildNeonDonutOptions, getCssVar } = useEChartsTheme()

// ?? ECharts ??
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
  ssr: false // ?? SSR?Naive UI ?????
})

// ???AppButton ??????Nuxt 3 ??????// ???? "Failed to resolve component: AppButton" ????// ???????components/ui/AppButton.vue ????
// ???????import AppButton from '~/components/ui/AppButton.vue'

// ?????? composable
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
const onlineOnly = ref(false) // ?????????????
// ??????????????????????????"??
const initialLoadComplete = ref(false)


// ????????
const selectedRange = ref<'today' | '7d' | '30d' | '90d'>('7d')

// ????
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

// ????????selectedRange ????const trendRange = ref<'7d' | '30d' | '90d'>('7d')
const trendData = ref<any>({ points: [] })
const trendLoading = ref(false)

// Top ????
const topPages = ref<any[]>([])
const topPagesLoading = ref(false)

// ??????
const sources = ref<any>({
  total: 0,
  items: [],
  topReferrers: []
})
const sourcesLoading = ref(false)

// ????????const searchKeywords = ref<any[]>([])
const searchKeywordsLoading = ref(false)

// ??????
const regions = ref<any>({ items: [] })
const regionsLoading = ref(false)

// ?????/???/????
const clientDistribution = ref<any>({
  devices: [],
  browsers: [],
  os: []
})
const clientDistributionLoading = ref(false)

// ??????
const pageFlow = ref<any>({
  nodes: [],
  edges: []
})
const pageFlowLoading = ref(false)


// ?????????? trend ????// ??????(ECharts) - Aurora DS Neon Style
const trendLineOption = computed(() => {
  const points = trendData.value?.points || trendData.value?.Points || []
  if (!trendData.value || points.length === 0) return null
  
  const labels = points.map((p: any) => {
    const dateStr = p.date || p.Date || ''
    return dateStr.split(' ')[0].slice(5) // MM-DD
  })

  const primaryColor = getCssVar('--chart-primary')
  const secondaryColor = getCssVar('--chart-secondary')

  // ??????
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

  // ???????
  const option = {
    ...baseConfig,
    series: [
      // PV ??
      {
        name: '???',
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
      // UV ??
      {
        name: '???',
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
  
  // ????
  return applyTheme(option)
})

// ????????? - Aurora Neon
const regionBarOption = computed(() => {
  if (!regions.value?.items?.length) return null
  
  const items = regions.value.items.map((r: any) => ({
    name: r.province ? `${r.country}-${r.province}` : r.country,
    value: r.count || 0
  })).sort((a: any, b: any) => b.value - a.value).slice(0, 10)

  // ?????????
  const baseConfig = buildNeonBarOptions('--chart-primary', '--chart-secondary', {
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
  
  // ????
  return applyTheme(option)
})

// ???? Donut - Aurora Neon
const deviceDonutOption = computed(() => {
  if (!clientDistribution.value?.devices?.length) return null
  const data = clientDistribution.value.devices.map((d: any, idx: number) => ({
    name: d.name || '??',
    value: d.count || 0,
    colorVar: `--chart-${['primary','secondary','tertiary','quaternary','quinary'][idx % 5]}`
  }))
  
  const total = data.reduce((s: number, i: any) => s + i.value, 0)
  const max = data.reduce((m: any, i: any) => i.value > m.value ? i : m, data[0])
  
  // ??????ECharts option
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
  
  // ?? series ??center????????
  const adjustedSeries = {
    ...donutSeries,
    center: ['50%', '45%'], // ????????????????    radius: ['50%', '70%'] // ????????????
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
  
  // ????
  const themedOption = applyTheme(fullOption)
  
  return {
    option: themedOption,
    mainLabel: max.name,
    mainPercent: total > 0 ? ((max.value / total) * 100).toFixed(1) : '0'
  }
})

// ????Donut - Aurora Neon
const browserDonutOption = computed(() => {
  if (!clientDistribution.value?.browsers?.length) return null
  const data = clientDistribution.value.browsers.map((d: any, idx: number) => ({
    name: d.name || '??',
    value: d.count || 0,
    colorVar: `--chart-${['quaternary','quinary','primary','secondary','tertiary'][idx % 5]}`
  }))
  
  const total = data.reduce((s: number, i: any) => s + i.value, 0)
  const max = data.reduce((m: any, i: any) => i.value > m.value ? i : m, data[0])
  
  // ??????ECharts option
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
  
  // ?? series ??center????????
  const adjustedSeries = {
    ...donutSeries,
    center: ['50%', '45%'], // ????????????????    radius: ['50%', '70%'] // ????????????
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
  
  // ????
  const themedOption = applyTheme(fullOption)
  
  return {
    option: themedOption,
    mainLabel: max.name,
    mainPercent: total > 0 ? ((max.value / total) * 100).toFixed(1) : '0'
  }
})

// ???? Donut - Aurora Neon
const osDonutOption = computed(() => {
  if (!clientDistribution.value?.os?.length) return null
  const data = clientDistribution.value.os.map((d: any, idx: number) => ({
    name: d.name || '??',
    value: d.count || 0,
    colorVar: `--chart-${['tertiary','quaternary','quinary','primary','secondary'][idx % 5]}`
  }))
  
  const total = data.reduce((s: number, i: any) => s + i.value, 0)
  const max = data.reduce((m: any, i: any) => i.value > m.value ? i : m, data[0])
  
  // ??????ECharts option
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
  
  // ?? series ??center????????
  const adjustedSeries = {
    ...donutSeries,
    center: ['50%', '45%'], // ????????????????    radius: ['50%', '70%'] // ????????????
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
  
  // ????
  const themedOption = applyTheme(fullOption)
  
  return {
    option: themedOption,
    mainLabel: max.name,
    mainPercent: total > 0 ? ((max.value / total) * 100).toFixed(1) : '0'
  }
})

// ???? Donut - Aurora Neon
const sourceDonutOption = computed(() => {
  if (!sources.value?.items?.length) return null
  const data = sources.value.items.map((d: any, idx: number) => ({
    name: d.name || '??',
    value: d.count || 0,
    colorVar: `--chart-${['secondary','tertiary','quaternary','quinary','primary'][idx % 5]}`
  }))
  
  const total = data.reduce((s: number, i: any) => s + i.value, 0)
  const max = data.reduce((m: any, i: any) => i.value > m.value ? i : m, data[0])
  
  // ??????ECharts option
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
  
  // ?? series ??center????????
  const adjustedSeries = {
    ...donutSeries,
    center: ['50%', '45%'], // ????????????????    radius: ['50%', '70%'] // ????????????
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
  
  // ????
  const themedOption = applyTheme(fullOption)
  
  return {
    option: themedOption,
    mainLabel: max.name,
    mainPercent: total > 0 ? ((max.value / total) * 100).toFixed(1) : '0'
  }
})

// ????????"???" ??????
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

// ??????
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

// ????????????????????
const showNoDataAlert = computed(() => {
  // ????????????????????
  if (!initialLoadComplete.value) {
    return false
  }
  
  // ????????????????????
  if (statsLoading.value || topPagesLoading.value || trendLoading.value || 
      regionsLoading.value || clientDistributionLoading.value || 
      visitorsLoading.value || pageFlowLoading.value || 
      sourcesLoading.value || searchKeywordsLoading.value) {
    return false
  }

  // ?????????
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
  // ??????
  if (statsLoading.value) {
    return
  }
  
  try {
    statsLoading.value = true
    statsError.value = null
    const res = await api.get<any>('/Analytics/stats')
    
    // ???????????
    rateLimitRetryCount.value = 0
    
    if (res) {
      // ??????
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
    // ?? 429 ??????
    if (e.response?.status === 429) {
      rateLimitRetryCount.value++
      statsError.value = '????????????'
      
      // ?????????
        if (rateLimitRetryCount.value >= 3) {
        if (autoRefreshInterval.value) {
          clearInterval(autoRefreshInterval.value)
          autoRefreshInterval.value = null
        }
        if (process.client) {
          alert('??????????????')
        }
        return
      }
    } else {
      // ????
      statsError.value = e.message || '????'
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
      // useApi ??????data ??????res ????{ total: 39, page: 1, pageSize: 20, visitors: [...] }
      // ???????????? "visitors" ??"total"????????      const visitorsData = res.visitors || res.Visitors || (Array.isArray(res) ? res : [])
      const totalData = res.total ?? res.Total ?? (Array.isArray(res) ? res.length : 0)
      
      visitors.value = Array.isArray(visitorsData) ? visitorsData : []
      visitorsTotal.value = totalData
    }
  } catch (e: any) {
    // ??????
    if (process.client) {
      alert(`????????: ${e?.message || '????'}\n\n????\n1. ??????????\n2. ??????????\n3. ????????`)
    }
  } finally {
    visitorsLoading.value = false
  }
}

const changePage = (page: number) => {
  visitorsPage.value = page
  fetchVisitors()
}

// ??????
const translateWord = (word: string): string => {
  const wordLower = word.toLowerCase()
  
  // ??????
  const wordMap: Record<string, string> = {
    'dashboard': '???',
    'home': '??',
    'index': '??',
    'blog': '??',
    'article': '??',
    'tools': '??',
    'projects': '??',
    'life': '??',
    'lab': '???',
    'ai': 'AI??',
    'admin': '??',
    'about': '??',
    'contact': '??',
    'search': '??',
    'profile': '??',
    'settings': '??',
    'account': '??',
    'login': '??',
    'register': '??',
    'logout': '??',
    'analytics': '??',
    'articles': '??',
    'categories': '??',
    'timeline': '???',
    'themes': '??',
    'users': '??',
    'comments': '??',
    'media': '??',
    'pages': '??',
    'menus': '??',
    'widgets': '??',
    'backup': '??',
    'logs': '??',
    'security': '??',
    'api': 'API',
    'edit': '??',
    'create': '??',
    'update': '??',
    'delete': '??',
    'list': '??',
    'detail': '??',
    'manage': '??'
  }
  
  if (wordMap[wordLower]) {
    return wordMap[wordLower]
  }
  
  // ????????????????  // ???forgot-password -> ????, userProfile -> ????
  const hyphenParts = wordLower.split('-')
  if (hyphenParts.length > 1) {
    // ??????????????
    const translated = hyphenParts.map(part => wordMap[part] || part).join('')
    if (translated !== wordLower) {
      return translated
    }
  }
  
  // ???????userProfile -> user profile
  const camelCaseParts = wordLower.replace(/([A-Z])/g, ' $1').split(' ').filter(p => p)
  if (camelCaseParts.length > 1) {
    const translated = camelCaseParts.map(part => wordMap[part] || part).join('')
    if (translated !== wordLower) {
      return translated
    }
  }
  
  // ??????????????????
  return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase()
}

// ????????
const formatPathName = (path: string): string => {
  if (!path) return '??'
  
  // ?? landing/page ??
  const cleanPath = path.replace('landing:', '').replace('page:', '').trim()
  const pathWithoutQuery = cleanPath.split('?')[0].split('#')[0]
  
  // ???
  if (!pathWithoutQuery || pathWithoutQuery === '/') {
    return '??'
  }
  
  // ????
  const parts = pathWithoutQuery.split('/').filter(p => p)
  
  if (parts.length === 0) {
    return '??'
  }
  
  // ??????????
  const translatedParts = parts.map(part => translateWord(part))
  
  // ????????????
  if (parts.length === 1) {
    // ????
    return translatedParts[0]
  } else if (parts.length === 2) {
    // ??????????
    // ?????????? slug??????????????????????
    const secondPart = parts[1]
    const isSlug = /^[a-z0-9-]+$/.test(secondPart.toLowerCase()) && secondPart.length > 10
    
    if (isSlug) {
      // ??????slug
      const displaySlug = secondPart.length > 25 ? secondPart.substring(0, 25) + '...' : secondPart
      return `${translatedParts[0]}/${displaySlug}`
    } else {
      // ??????????????????
      return `${translatedParts[0]}/${translatedParts[1]}`
    }
  } else {
    // ??????????????
    const lastPart = parts[parts.length - 1]
    const isSlug = /^[a-z0-9-]+$/.test(lastPart.toLowerCase()) && lastPart.length > 10
    
    if (isSlug) {
      // ???????slug????"????????"
      const displaySlug = lastPart.length > 20 ? lastPart.substring(0, 20) + '...' : lastPart
      const middleParts = translatedParts.slice(1, -1).join('/')
      return `${translatedParts[0]}/${middleParts ? middleParts + '/' : ''}${displaySlug}`
    } else {
      // ?????????????
      return translatedParts.join('/')
    }
  }
}

const formatTime = (timeStr: string) => {
  if (!timeStr) return '-'
  const date = new Date(timeStr)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / 60000)
  
  if (minutes < 1) return '??'
  if (minutes < 60) return `${minutes}???`
  const hours = Math.floor(minutes / 60)
  if (hours < 24) return `${hours}???`
  const days = Math.floor(hours / 24)
  if (days < 7) return `${days}??`
  
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// ?????URL??????
const formatPageUrl = (url: string) => {
  if (!url) return '????'
  
  // ??????
  if (url === '/' || url === '') return '??'
  
  // ???????
  const cleanUrl = url.startsWith('/') ? url.substring(1) : url
  
  // ????
  if (cleanUrl.startsWith('blog/')) {
    const slug = cleanUrl.replace('blog/', '')
    return slug ? `??: ${slug}` : '????'
  }
  if (cleanUrl.startsWith('tools/')) {
    const tool = cleanUrl.replace('tools/', '')
    return tool ? `??: ${tool}` : '????'
  }
  if (cleanUrl.startsWith('ai/')) {
    return 'AI??'
  }
  if (cleanUrl.startsWith('projects/')) {
    const project = cleanUrl.replace('projects/', '')
    return project ? `??: ${project}` : '????'
  }
if (cleanUrl.startsWith('lab')) {
      return '???'
    }
  if (cleanUrl.startsWith('admin')) {
    return '??'
  }
  
  // ????????URL??????
  return cleanUrl || '??'
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
    // ????????
    trendData.value = { points: [] }
  } finally {
    trendLoading.value = false
  }
}

// ??????
const fetchOverview = async () => {
  try {
    const res = await api.get<any>('/Analytics/overview')
    console.log('????:', res)
      if (res) {
      // ??????????????????
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
      console.log('????????', overview.value) // ????    } else {
      console.warn('????????')
    }
  } catch (e: any) {
    console.error('????????:', e)
    // ???????????0
  }
}

// ?? Top ??
const fetchTopPages = async () => {
  try {
    topPagesLoading.value = true
    const res = await api.get<any>(`/Analytics/top-pages?range=${selectedRange.value}`)
    if (res && res.items) {
      topPages.value = res.items
    }
  } catch (e: any) {
    // ????
  } finally {
    topPagesLoading.value = false
  }
}

// ??????
const fetchSources = async () => {
  try {
    sourcesLoading.value = true
    const res = await api.get<any>(`/Analytics/sources?range=${selectedRange.value}`)
    if (res) {
      sources.value = res
    }
  } catch (e: any) {
    // ????
  } finally {
    sourcesLoading.value = false
  }
}

// ???????
const fetchSearchKeywords = async () => {
  try {
    searchKeywordsLoading.value = true
    const res = await api.get<any>(`/Analytics/search-keywords?range=${selectedRange.value}`)
    if (res && res.items) {
      searchKeywords.value = res.items
    }
  } catch (e: any) {
    // ????
  } finally {
    searchKeywordsLoading.value = false
  }
}

// ??????
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

// ???????
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

// ??????
const fetchPageFlow = async () => {
  try {
    pageFlowLoading.value = true
    const res = await api.get<any>(`/Analytics/page-flow?range=${selectedRange.value}`)
    if (res) {
      pageFlow.value = res
    }
  } catch (e: any) {
    // ????
  } finally {
    pageFlowLoading.value = false
  }
}

// ??????
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
    // ????????
    initialLoadComplete.value = true
  } catch (error) {
    // ??????????
      initialLoadComplete.value = true
  }
}

// ????????
watch(selectedRange, (newRange) => {
  // ???????? range???????today ????7d??  trendRange.value = newRange === 'today' ? '7d' : newRange as any
  // ?????????????
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
const autoRefreshEnabled = ref(true) // ???????
// Donut ????
const getDonutColor = (index: number): string => {
  const colorVars = ['--chart-primary', '--chart-secondary', '--chart-tertiary', '--chart-quaternary', '--chart-quinary']
  const colorVar = colorVars[index % colorVars.length]
  return getCssVar(colorVar)
}

// ??????
watch(autoRefreshEnabled, (enabled) => {
  if (enabled) {
    // ????????????
    if (process.client && !autoRefreshInterval.value) {
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
    // ??????
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
      alert('???????????\n\n????????...')
      navigateTo('/admin/login')
      return
    }
  }
  
  // ????????
  setTimeout(() => {
    refreshAll()
  }, 500)
  
  // ?????????????????
  if (process.client && autoRefreshEnabled.value) {
    autoRefreshInterval.value = setInterval(() => {
      // ??????????
      if (rateLimitRetryCount.value >= 3) {
        if (autoRefreshInterval.value) {
          clearInterval(autoRefreshInterval.value)
          autoRefreshInterval.value = null
        }
        return
      }
      
      fetchOverview()
      fetchStats()
      // ??????
      fetchVisitors()
    }, 60000) // 60?
  }
})

// ??????????
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

/* ??????????- ?? CSS ?? */
:root {
  --analytics-list-item-bg: var(--color-bg-surface-2, var(--n-card-color));
  --analytics-list-item-bg-hover: var(--color-bg-surface-1, var(--n-card-color-hover));
  --analytics-list-item-padding-sm: 0.5rem;
  --analytics-list-item-padding-md: 0.75rem;
  --analytics-list-item-border-radius: 0.5rem;
  --analytics-list-item-gap: 0.5rem;
  
  /* ???????? */
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

/* ????????*/
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

/* Top 10 ??????*/
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

