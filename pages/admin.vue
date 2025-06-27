<template>
  <div class="min-h-screen bg-gray-100">
    <!-- 未登录状态 - 登录表单 -->
    <div v-if="!isAuthenticated" class="flex items-center justify-center min-h-screen">
      <div class="bg-white rounded-xl shadow-lg p-8 w-full max-w-md">
        <div class="text-center mb-8">
          <h1 class="text-2xl font-bold text-gray-800">🔐 后台管理</h1>
          <p class="text-gray-600 mt-2">请输入管理员密码</p>
        </div>
        
        <form @submit.prevent="handleLogin">
          <div class="mb-6">
            <label class="block text-sm font-medium text-gray-700 mb-2">密码</label>
            <input
              v-model="password"
              type="password"
              placeholder="请输入管理员密码"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              required
            >
          </div>
          
          <button
            type="submit"
            :disabled="loading"
            class="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
          >
            <span v-if="loading">验证中...</span>
            <span v-else>登录</span>
          </button>
          
          <div v-if="error" class="mt-4 p-3 bg-red-100 text-red-700 rounded-lg text-sm">
            {{ error }}
          </div>
        </form>
      </div>
    </div>

    <!-- 已登录状态 - 管理面板 -->
    <div v-else class="p-6">
      <!-- 头部 -->
      <div class="bg-white rounded-xl shadow-sm p-6 mb-6">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-2xl font-bold text-gray-800">📊 溪午听风 后台管理</h1>
            <p class="text-gray-600">内容管理和网站统计</p>
          </div>
          <button
            @click="handleLogout"
            class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
          >
            退出登录
          </button>
        </div>
      </div>

      <!-- 统计卡片 -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-sm p-6">
          <div class="flex items-center">
            <div class="p-3 bg-blue-100 rounded-lg">
              <span class="text-2xl">🔧</span>
            </div>
            <div class="ml-4">
              <p class="text-sm text-gray-600">工具数量</p>
              <p class="text-2xl font-bold text-blue-600">{{ stats.toolsCount }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white rounded-xl shadow-sm p-6">
          <div class="flex items-center">
            <div class="p-3 bg-purple-100 rounded-lg">
              <span class="text-2xl">🧪</span>
            </div>
            <div class="ml-4">
              <p class="text-sm text-gray-600">项目数量</p>
              <p class="text-2xl font-bold text-purple-600">{{ stats.projectsCount }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white rounded-xl shadow-sm p-6">
          <div class="flex items-center">
            <div class="p-3 bg-green-100 rounded-lg">
              <span class="text-2xl">📝</span>
            </div>
            <div class="ml-4">
              <p class="text-sm text-gray-600">博客文章</p>
              <p class="text-2xl font-bold text-green-600">{{ stats.postsCount }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white rounded-xl shadow-sm p-6">
          <div class="flex items-center">
            <div class="p-3 bg-orange-100 rounded-lg">
              <span class="text-2xl">👁️</span>
            </div>
            <div class="ml-4">
              <p class="text-sm text-gray-600">今日访问</p>
              <p class="text-2xl font-bold text-orange-600">{{ stats.todayViews }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- 快速操作 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- 内容管理 -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h2 class="text-lg font-semibold text-gray-800 mb-4">📁 内容管理</h2>
          <div class="space-y-3">
            <button class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
              <div class="flex items-center">
                <span class="text-lg mr-3">🔧</span>
                <div>
                  <p class="font-medium">管理工具</p>
                  <p class="text-sm text-gray-600">添加、编辑或删除插件工具</p>
                </div>
              </div>
            </button>
            
            <button class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
              <div class="flex items-center">
                <span class="text-lg mr-3">🧪</span>
                <div>
                  <p class="font-medium">管理项目</p>
                  <p class="text-sm text-gray-600">更新项目信息和状态</p>
                </div>
              </div>
            </button>
            
            <button class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
              <div class="flex items-center">
                <span class="text-lg mr-3">📝</span>
                <div>
                  <p class="font-medium">管理博客</p>
                  <p class="text-sm text-gray-600">发布新文章或编辑现有文章</p>
                </div>
              </div>
            </button>
          </div>
        </div>

        <!-- 系统工具 -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h2 class="text-lg font-semibold text-gray-800 mb-4">⚙️ 系统工具</h2>
          <div class="space-y-3">
            <button 
              @click="clearCache"
              class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <div class="flex items-center">
                <span class="text-lg mr-3">🗑️</span>
                <div>
                  <p class="font-medium">清除缓存</p>
                  <p class="text-sm text-gray-600">清除网站缓存，刷新内容</p>
                </div>
              </div>
            </button>
            
            <button class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
              <div class="flex items-center">
                <span class="text-lg mr-3">📊</span>
                <div>
                  <p class="font-medium">访问统计</p>
                  <p class="text-sm text-gray-600">查看详细的访问数据</p>
                </div>
              </div>
            </button>
            
            <button class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
              <div class="flex items-center">
                <span class="text-lg mr-3">⚡</span>
                <div>
                  <p class="font-medium">性能优化</p>
                  <p class="text-sm text-gray-600">检查和优化网站性能</p>
                </div>
              </div>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

// 页面标题
useHead({
  title: '后台管理 - 溪午听风',
  meta: [
    { name: 'robots', content: 'noindex, nofollow' }
  ]
})

// 状态管理
const isAuthenticated = ref(false)
const password = ref('')
const loading = ref(false)
const error = ref('')

// 统计数据
const stats = ref({
  toolsCount: 0,
  projectsCount: 0,
  postsCount: 0,
  todayViews: 0
})

// 检查登录状态
onMounted(() => {
  const authToken = localStorage.getItem('admin_auth')
  if (authToken === 'authenticated') {
    isAuthenticated.value = true
    loadStats()
  }
})

// 登录处理
const handleLogin = () => {
  loading.value = true
  error.value = ''
  
  // 模拟验证过程
  setTimeout(() => {
    // 这里应该是实际的密码验证逻辑
    if (password.value === 'your-admin-password') { // 替换为你的实际密码
      isAuthenticated.value = true
      localStorage.setItem('admin_auth', 'authenticated')
      loadStats()
    } else {
      error.value = '密码错误，请重试'
    }
    loading.value = false
  }, 1000)
}

// 退出登录
const handleLogout = () => {
  isAuthenticated.value = false
  localStorage.removeItem('admin_auth')
  password.value = ''
}

// 加载统计数据
const loadStats = async () => {
  try {
    // 这里应该调用实际的API获取统计数据
    // 现在使用模拟数据
    stats.value = {
      toolsCount: 8,
      projectsCount: 12,
      postsCount: 25,
      todayViews: 156
    }
  } catch (error) {
    console.error('Failed to load stats:', error)
  }
}

// 清除缓存
const clearCache = () => {
  // 这里实现清除缓存的逻辑
  alert('缓存已清除')
}
</script> 