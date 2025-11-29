<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-800 dark:text-white">访客分析</h1>
        <p class="text-sm text-gray-500 dark:text-gray-400 mt-1">
          查看网站访问统计和访客数据
        </p>
      </div>
      <button @click="refreshStats" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
        刷新数据
      </button>
    </div>

    <!-- 数据提示 -->
    <div v-if="stats.Today?.Pv === 0 && stats.Today?.Uv === 0" class="mb-6 bg-yellow-50 dark:bg-yellow-900/30 border-2 border-yellow-300 dark:border-yellow-700 rounded-lg p-4">
      <div class="flex items-start">
        <div class="flex-shrink-0">
          <svg class="h-5 w-5 text-yellow-600 dark:text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
          </svg>
        </div>
        <div class="ml-3 flex-1">
          <h3 class="text-sm font-bold text-yellow-900 dark:text-yellow-100 mb-2">暂无访客数据</h3>
          <div class="mt-2 text-sm text-yellow-800 dark:text-yellow-200 leading-relaxed">
            <p class="mb-2 font-medium">当前没有访客访问记录。可能的原因：</p>
            <ul class="list-disc list-inside mt-1 space-y-1 ml-2">
              <li>网站还没有访客访问</li>
              <li>访客数据存储在 <code class="bg-yellow-200 dark:bg-yellow-800 px-2 py-0.5 rounded font-mono text-xs font-semibold text-yellow-900 dark:text-yellow-100">VisitLogs</code> 表中，请检查数据库</li>
              <li>如果使用代理或VPN，可能无法正确记录IP地址</li>
            </ul>
            <p class="mt-3 mb-2 font-medium">
              <strong class="text-yellow-900 dark:text-yellow-100">提示：</strong>访问网站首页会自动记录访问数据。您可以：
            </p>
            <ul class="list-disc list-inside mt-1 space-y-1 ml-2">
              <li>打开网站首页，系统会自动记录您的访问</li>
              <li>点击"刷新数据"按钮更新统计数据</li>
              <li>检查浏览器控制台的日志信息</li>
            </ul>
          </div>
        </div>
      </div>
    </div>

    <!-- 核心指标 -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">今日 PV</div>
        <div class="text-2xl font-bold text-blue-600 dark:text-blue-400">{{ stats.Today?.Pv || 0 }}</div>
        <div class="text-xs text-gray-500 mt-1">
          昨日: {{ stats.Yesterday?.Pv || 0 }}
        </div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">今日 UV</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats.Today?.Uv || 0 }}</div>
        <div class="text-xs text-gray-500 mt-1">
          昨日: {{ stats.Yesterday?.Uv || 0 }}
        </div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">在线人数</div>
        <div class="text-2xl font-bold text-orange-600 dark:text-orange-400">{{ stats.OnlineCount || 0 }}</div>
        <div class="text-xs text-gray-500 mt-1">最近5分钟活跃</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">热门文章数</div>
        <div class="text-2xl font-bold text-purple-600 dark:text-purple-400">{{ stats.TopArticles?.length || 0 }}</div>
        <div class="text-xs text-gray-500 mt-1">Top 10</div>
      </div>
    </div>

    <!-- 饼状图统计区域 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-4 gap-6 mb-6">
      <!-- 访问区域饼状图 -->
      <div class="chart-container bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="chart-title text-lg font-bold text-gray-800 dark:text-white mb-4">访问区域分布</h2>
        <div v-if="stats.RegionStats && stats.RegionStats.length > 0" class="h-64">
          <Pie :data="regionChartData" :options="chartOptions" />
        </div>
        <div v-else class="text-center text-gray-500 dark:text-gray-400 py-8">暂无数据</div>
      </div>

      <!-- 设备类型饼状图 -->
      <div class="chart-container bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="chart-title text-lg font-bold text-gray-800 dark:text-white mb-4">设备类型分布</h2>
        <div v-if="stats.DeviceStats && stats.DeviceStats.length > 0" class="h-64">
          <Pie :data="deviceChartData" :options="chartOptions" />
        </div>
        <div v-else class="text-center text-gray-500 dark:text-gray-400 py-8">暂无数据</div>
      </div>

      <!-- 浏览器饼状图 -->
      <div class="chart-container bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="chart-title text-lg font-bold text-gray-800 dark:text-white mb-4">浏览器分布</h2>
        <div v-if="stats.BrowserStats && stats.BrowserStats.length > 0" class="h-64">
          <Pie :data="browserChartData" :options="chartOptions" />
        </div>
        <div v-else class="text-center text-gray-500 dark:text-gray-400 py-8">暂无数据</div>
      </div>

      <!-- 操作系统饼状图 -->
      <div class="chart-container bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="chart-title text-lg font-bold text-gray-800 dark:text-white mb-4">操作系统分布</h2>
        <div v-if="stats.OsStats && stats.OsStats.length > 0" class="h-64">
          <Pie :data="osChartData" :options="chartOptions" />
        </div>
        <div v-else class="text-center text-gray-500 dark:text-gray-400 py-8">暂无数据</div>
      </div>
    </div>

    <!-- 统计表格区域 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- 访问区域统计表格 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">访问区域统计</h2>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">排名</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">国家/地区</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">访问次数</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">占比</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
              <tr
                v-for="(region, index) in stats.RegionStats"
                :key="index"
                class="hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                <td class="px-4 py-3 text-gray-900 dark:text-gray-100 font-bold">{{ index + 1 }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">{{ region.Country || '未知' }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">{{ region.Count }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                  {{ ((region.Count / totalRegionCount) * 100).toFixed(2) }}%
                </td>
              </tr>
              <tr v-if="!stats.RegionStats || stats.RegionStats.length === 0">
                <td colspan="4" class="px-4 py-8 text-center text-gray-500">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- 设备类型统计表格 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">设备类型统计</h2>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">设备类型</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">访问次数</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">占比</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
              <tr
                v-for="(device, index) in stats.DeviceStats"
                :key="index"
                class="hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">{{ device.DeviceType || '未知' }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">{{ device.Count }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                  {{ ((device.Count / totalDeviceCount) * 100).toFixed(2) }}%
                </td>
              </tr>
              <tr v-if="!stats.DeviceStats || stats.DeviceStats.length === 0">
                <td colspan="3" class="px-4 py-8 text-center text-gray-500">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- 浏览器统计表格 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">浏览器统计</h2>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">排名</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">浏览器</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">访问次数</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">占比</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
              <tr
                v-for="(browser, index) in stats.BrowserStats"
                :key="index"
                class="hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                <td class="px-4 py-3 text-gray-900 dark:text-gray-100 font-bold">{{ index + 1 }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">{{ browser.Browser || '未知' }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">{{ browser.Count }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                  {{ ((browser.Count / totalBrowserCount) * 100).toFixed(2) }}%
                </td>
              </tr>
              <tr v-if="!stats.BrowserStats || stats.BrowserStats.length === 0">
                <td colspan="4" class="px-4 py-8 text-center text-gray-500">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- 操作系统统计表格 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">操作系统统计</h2>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">排名</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">操作系统</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">访问次数</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">占比</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
              <tr
                v-for="(os, index) in stats.OsStats"
                :key="index"
                class="hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                <td class="px-4 py-3 text-gray-900 dark:text-gray-100 font-bold">{{ index + 1 }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">{{ os.Os || '未知' }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">{{ os.Count }}</td>
                <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                  {{ ((os.Count / totalOsCount) * 100).toFixed(2) }}%
                </td>
              </tr>
              <tr v-if="!stats.OsStats || stats.OsStats.length === 0">
                <td colspan="4" class="px-4 py-8 text-center text-gray-500">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- 其他统计信息 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- 热门文章 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">热门文章</h2>
        <div class="space-y-3">
          <div
            v-for="(article, index) in stats.TopArticles"
            :key="index"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg"
          >
            <div class="flex-1">
              <div class="font-medium text-gray-800 dark:text-gray-200 text-sm truncate">
                {{ article.Path }}
              </div>
              <div class="text-xs text-gray-500 mt-1">{{ article.Views }} 次浏览</div>
            </div>
            <div class="w-8 h-8 rounded-full bg-blue-100 dark:bg-blue-900 flex items-center justify-center text-blue-600 dark:text-blue-400 font-bold text-sm">
              {{ index + 1 }}
            </div>
          </div>
          <div v-if="!stats.TopArticles || stats.TopArticles.length === 0" class="text-center text-gray-500 py-4">
            暂无数据
          </div>
        </div>
      </div>

      <!-- 搜索来源 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">搜索来源</h2>
        <div class="space-y-2">
          <div
            v-for="(source, index) in stats.SearchSources"
            :key="index"
            class="flex items-center justify-between p-2 bg-gray-50 dark:bg-gray-700 rounded"
          >
            <span class="text-sm text-gray-700 dark:text-gray-300">{{ source.Keyword }}</span>
            <span class="text-xs text-gray-500">{{ source.Count }} 次</span>
          </div>
          <div v-if="!stats.SearchSources || stats.SearchSources.length === 0" class="text-center text-gray-500 py-4">
            暂无搜索数据
          </div>
        </div>
      </div>
    </div>

    <!-- 访客列表 -->
    <div class="mt-6 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white">访客列表</h2>
        <div class="flex items-center gap-4">
          <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
            <input
              type="checkbox"
              v-model="onlineOnly"
              @change="fetchVisitors"
              class="rounded"
            />
            仅显示在线访客
          </label>
          <button
            @click="fetchVisitors"
            class="px-3 py-1 text-sm bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600"
          >
            刷新列表
          </button>
        </div>
      </div>

      <div v-if="visitorsLoading" class="text-center py-8 text-gray-500">
        加载中...
      </div>
      <div v-else-if="visitors.length === 0" class="text-center py-8 text-gray-500">
        暂无访客数据
      </div>
      <div v-else class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">访客ID</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">IP地址</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">地理位置</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">设备信息</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">当前页面</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">浏览量</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">最后活跃</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">状态</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr
              v-for="visitor in visitors"
              :key="visitor.Id"
              class="hover:bg-gray-50 dark:hover:bg-gray-700"
            >
              <td class="px-4 py-3 text-gray-900 dark:text-gray-100 font-mono text-xs">
                {{ visitor.VisitorId?.substring(0, 8) }}...
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300 font-mono text-xs">
                {{ visitor.Ip || '-' }}
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                <div class="text-xs">
                  <div v-if="visitor.Country">{{ visitor.Country }}</div>
                  <div v-if="visitor.Region" class="text-gray-500">{{ visitor.Region }}</div>
                  <div v-if="visitor.City" class="text-gray-500">{{ visitor.City }}</div>
                  <div v-if="!visitor.Country && !visitor.Region && !visitor.City" class="text-gray-400">未知</div>
                </div>
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                <div class="text-xs">
                  <div>{{ visitor.DeviceType || '-' }}</div>
                  <div class="text-gray-500">{{ visitor.Browser || '-' }} / {{ visitor.Os || '-' }}</div>
                </div>
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                <div class="text-xs max-w-xs truncate" :title="visitor.Path">
                  {{ visitor.Path || '-' }}
                </div>
                <div v-if="visitor.SearchKeyword" class="text-xs text-blue-600 dark:text-blue-400 mt-1">
                  搜索: {{ visitor.SearchKeyword }}
                </div>
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300 text-center">
                {{ visitor.PageViews || 0 }}
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300 text-xs">
                {{ formatTime(visitor.UpdatedAt) }}
              </td>
              <td class="px-4 py-3">
                <span
                  v-if="visitor.IsOnline"
                  class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200"
                >
                  <span class="w-1.5 h-1.5 bg-green-500 rounded-full mr-1"></span>
                  在线
                </span>
                <span
                  v-else
                  class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300"
                >
                  离线
                </span>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- 分页 -->
        <div v-if="visitorsTotal > pageSize" class="mt-4 flex items-center justify-between">
          <div class="text-sm text-gray-500">
            共 {{ visitorsTotal }} 条记录
          </div>
          <div class="flex gap-2">
            <button
              @click="changePage(visitorsPage - 1)"
              :disabled="visitorsPage <= 1"
              class="px-3 py-1 text-sm bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              上一页
            </button>
            <span class="px-3 py-1 text-sm text-gray-700 dark:text-gray-300">
              第 {{ visitorsPage }} / {{ Math.ceil(visitorsTotal / pageSize) }} 页
            </span>
            <button
              @click="changePage(visitorsPage + 1)"
              :disabled="visitorsPage >= Math.ceil(visitorsTotal / pageSize)"
              class="px-3 py-1 text-sm bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              下一页
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js'
import { Pie } from 'vue-chartjs'

ChartJS.register(ArcElement, Tooltip, Legend)

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

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
const onlineOnly = ref(false)

// 图表配置
// 检测深色模式
const isDark = computed(() => {
  if (process.client) {
    return document.documentElement.classList.contains('dark')
  }
  return false
})

const chartOptions = computed(() => {
  const textColor = isDark.value ? '#ffffff' : '#374151' // 深色模式用白色，浅色模式用深灰色
  const legendColor = isDark.value ? '#e5e7eb' : '#6b7280'
  const tooltipBg = isDark.value ? 'rgba(17, 24, 39, 0.98)' : 'rgba(255, 255, 255, 0.95)'
  const tooltipText = isDark.value ? '#ffffff' : '#111827'
  const tooltipBorder = isDark.value ? 'rgba(156, 163, 175, 0.5)' : 'rgba(209, 213, 219, 0.8)'
  
  return {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom' as const,
        labels: {
          padding: 15,
          usePointStyle: true,
          color: legendColor, // 根据主题调整颜色
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

// 访问区域图表数据
const regionChartData = computed(() => {
  if (!stats.value.RegionStats || stats.value.RegionStats.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = stats.value.RegionStats.map((r: any) => r.Country || '未知')
  const data = stats.value.RegionStats.map((r: any) => r.Count)
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

// 设备类型图表数据
const deviceChartData = computed(() => {
  if (!stats.value.DeviceStats || stats.value.DeviceStats.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = stats.value.DeviceStats.map((d: any) => d.DeviceType || '未知')
  const data = stats.value.DeviceStats.map((d: any) => d.Count)
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

// 浏览器图表数据
const browserChartData = computed(() => {
  if (!stats.value.BrowserStats || stats.value.BrowserStats.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = stats.value.BrowserStats.map((b: any) => b.Browser || '未知')
  const data = stats.value.BrowserStats.map((b: any) => b.Count)
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

// 操作系统图表数据
const osChartData = computed(() => {
  if (!stats.value.OsStats || stats.value.OsStats.length === 0) {
    return { labels: [], datasets: [] }
  }
  const labels = stats.value.OsStats.map((o: any) => o.Os || '未知')
  const data = stats.value.OsStats.map((o: any) => o.Count)
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

// 计算总数用于百分比
const totalRegionCount = computed(() => {
  if (!stats.value.RegionStats || stats.value.RegionStats.length === 0) return 1
  return stats.value.RegionStats.reduce((sum: number, r: any) => sum + r.Count, 0)
})

const totalDeviceCount = computed(() => {
  if (!stats.value.DeviceStats || stats.value.DeviceStats.length === 0) return 1
  return stats.value.DeviceStats.reduce((sum: number, d: any) => sum + d.Count, 0)
})

const totalBrowserCount = computed(() => {
  if (!stats.value.BrowserStats || stats.value.BrowserStats.length === 0) return 1
  return stats.value.BrowserStats.reduce((sum: number, b: any) => sum + b.Count, 0)
})

const totalOsCount = computed(() => {
  if (!stats.value.OsStats || stats.value.OsStats.length === 0) return 1
  return stats.value.OsStats.reduce((sum: number, o: any) => sum + o.Count, 0)
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
    console.log('开始获取访客列表...', {
      page: visitorsPage.value,
      pageSize: pageSize.value,
      onlineOnly: onlineOnly.value
    })
    const res = await api.get<any>('/Analytics/visitors', {
      params: {
        page: visitorsPage.value,
        pageSize: pageSize.value,
        onlineOnly: onlineOnly.value
      }
    })
    console.log('访客列表API响应:', res)
    if (res) {
      visitors.value = res.Visitors || []
      visitorsTotal.value = res.Total || 0
      console.log(`获取到 ${visitors.value.length} 条访客记录，总计 ${visitorsTotal.value} 条`)
    } else {
      console.warn('访客列表API返回空数据')
    }
  } catch (e: any) {
    console.error('Failed to fetch visitors:', e)
    console.error('错误详情:', e.message, e.response)
    // 显示错误提示
    if (process.client) {
      alert(`获取访客列表失败: ${e.message || '未知错误'}\n\n请检查：\n1. 是否已登录管理员账号\n2. 后端服务是否正常运行\n3. 网络连接是否正常`)
    }
  } finally {
    visitorsLoading.value = false
  }
}

const changePage = (page: number) => {
  visitorsPage.value = page
  fetchVisitors()
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

const refreshStats = () => {
  fetchStats()
  fetchVisitors()
}

const autoRefreshInterval = ref<NodeJS.Timeout | null>(null)

onMounted(() => {
  console.log('访客分析页面已加载')
  console.log('检查管理员登录状态...')
  
  if (process.client) {
    const token = localStorage.getItem('admin_token')
    const user = localStorage.getItem('admin_user')
    console.log('管理员Token:', token ? '已存在' : '不存在')
    console.log('管理员信息:', user)
    
    if (!token) {
      console.warn('未检测到管理员登录，请先登录')
      alert('请先登录管理员账号才能查看访客数据！\n\n将跳转到登录页面...')
      navigateTo('/admin/login')
      return
    }
  }
  
  // 延迟一下再加载数据，确保页面完全渲染
  setTimeout(() => {
    console.log('开始加载数据...')
    fetchStats()
    fetchVisitors()
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
      
      fetchStats()
      if (onlineOnly.value) {
        fetchVisitors()
      }
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

