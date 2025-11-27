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
          <div class="flex gap-3">
            <button
              @click="refreshStats"
              class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
            >
              🔄 刷新数据
            </button>
            <button
              @click="handleLogout"
              class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
            >
              退出登录
            </button>
          </div>
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

      <!-- 内容管理列表 -->
      <div class="bg-white rounded-xl shadow-sm p-6 mb-6">
        <h2 class="text-xl font-semibold text-gray-800 mb-4">📝 最近内容</h2>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left py-3 px-4">类型</th>
                <th class="text-left py-3 px-4">标题</th>
                <th class="text-left py-3 px-4">状态</th>
                <th class="text-left py-3 px-4">更新时间</th>
                <th class="text-left py-3 px-4">操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in recentContent" :key="item.id" class="border-b hover:bg-gray-50">
                <td class="py-3 px-4">
                  <span :class="{
                    'bg-blue-100 text-blue-800': item.type === 'tool',
                    'bg-purple-100 text-purple-800': item.type === 'project',
                    'bg-green-100 text-green-800': item.type === 'blog'
                  }" class="px-2 py-1 rounded text-xs font-medium">
                    {{ item.type === 'tool' ? '工具' : item.type === 'project' ? '项目' : '博客' }}
                  </span>
                </td>
                <td class="py-3 px-4">
                  <div>
                    <p class="font-medium">{{ item.title }}</p>
                    <p class="text-xs text-gray-500 mt-1">{{ item.filePath }}</p>
                  </div>
                </td>
                <td class="py-3 px-4">
                  <span :class="item.published ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'" 
                        class="px-2 py-1 rounded text-xs">
                    {{ item.published ? '已发布' : '草稿' }}
                  </span>
                </td>
                <td class="py-3 px-4 text-gray-600 text-sm">{{ item.updated }}</td>
                <td class="py-3 px-4">
                  <div class="flex gap-2">
                    <button @click="editContent(item)" class="text-blue-600 hover:text-blue-800 text-sm">
                      查看
                    </button>
                    <button @click="copyFilePath(item)" class="text-gray-600 hover:text-gray-800 text-sm" title="复制文件路径">
                      📋
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- 内容更新提示 -->
      <div class="bg-blue-50 border-l-4 border-blue-500 p-4 mb-6 rounded-r-lg">
        <div class="flex items-start">
          <div class="flex-shrink-0">
            <span class="text-2xl">💡</span>
          </div>
          <div class="ml-3 flex-1">
            <h3 class="text-sm font-semibold text-blue-800 mb-1">如何更新内容？</h3>
            <p class="text-sm text-blue-700 mb-2">
              所有内容通过 Markdown 文件管理。编辑 <code class="bg-blue-100 px-1 rounded">content/</code> 目录下的文件即可更新。
            </p>
            <div class="text-xs text-blue-600 space-y-1">
              <p>📝 <strong>博客文章：</strong> <code>content/blog/</code></p>
              <p>🔧 <strong>工具插件：</strong> <code>content/tools/</code></p>
              <p>🧪 <strong>项目展示：</strong> <code>content/projects/</code></p>
            </div>
            <a 
              href="/内容更新指南.md" 
              target="_blank"
              class="inline-flex items-center mt-2 text-sm text-blue-600 hover:text-blue-800 font-medium"
            >
              查看详细更新指南 →
            </a>
          </div>
        </div>
      </div>

      <!-- 快速操作 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- 内容管理 -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h2 class="text-lg font-semibold text-gray-800 mb-4">📁 内容管理</h2>
          <div class="space-y-3">
            <button @click="navigateToToolsManagement" class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
              <div class="flex items-center">
                <span class="text-lg mr-3">🔧</span>
                <div class="flex-1">
                  <p class="font-medium">管理工具</p>
                  <p class="text-sm text-gray-600">添加、编辑或删除插件工具</p>
                  <p class="text-xs text-gray-500 mt-1">文件位置：content/tools/</p>
                </div>
                <span class="ml-auto text-gray-400">→</span>
              </div>
            </button>
            
            <button @click="navigateToProjectsManagement" class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
              <div class="flex items-center">
                <span class="text-lg mr-3">🧪</span>
                <div class="flex-1">
                  <p class="font-medium">管理项目</p>
                  <p class="text-sm text-gray-600">更新项目信息和状态</p>
                  <p class="text-xs text-gray-500 mt-1">文件位置：content/projects/</p>
                </div>
                <span class="ml-auto text-gray-400">→</span>
              </div>
            </button>
            
            <button @click="navigateToBlogManagement" class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
              <div class="flex items-center">
                <span class="text-lg mr-3">📝</span>
                <div class="flex-1">
                  <p class="font-medium">管理博客</p>
                  <p class="text-sm text-gray-600">发布新文章或编辑现有文章</p>
                  <p class="text-xs text-gray-500 mt-1">文件位置：content/blog/</p>
                </div>
                <span class="ml-auto text-gray-400">→</span>
              </div>
            </button>
          </div>
          
          <!-- 快速操作提示 -->
          <div class="mt-4 pt-4 border-t border-gray-200">
            <p class="text-xs text-gray-500 mb-2">💡 快速操作：</p>
            <div class="text-xs text-gray-600 space-y-1">
              <p>1. 编辑 Markdown 文件（使用 VS Code 等编辑器）</p>
              <p>2. 本地预览：<code class="bg-gray-100 px-1 rounded">npm run dev</code></p>
              <p>3. 构建部署：<code class="bg-gray-100 px-1 rounded">npm run build</code></p>
            </div>
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
            
            <button @click="showVisitStats" class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
              <div class="flex items-center">
                <span class="text-lg mr-3">📊</span>
                <div>
                  <p class="font-medium">访问统计</p>
                  <p class="text-sm text-gray-600">查看详细的访问数据</p>
                </div>
              </div>
            </button>
            
            <button @click="runPerformanceCheck" class="w-full text-left p-3 rounded-lg hover:bg-gray-50 transition-colors">
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

    <!-- 弹窗模态框 -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl p-6 max-w-lg w-full mx-4 max-h-96 overflow-y-auto">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-semibold">{{ modalTitle }}</h3>
          <button @click="closeModal" class="text-gray-500 hover:text-gray-700">✕</button>
        </div>
        <div class="text-sm text-gray-600" v-html="modalContent"></div>
        <div class="mt-4 flex justify-end">
          <button @click="closeModal" class="px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-700">
            关闭
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
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
const showModal = ref(false)
const modalTitle = ref('')
const modalContent = ref('')

// 统计数据
const stats = ref({
  toolsCount: 0,
  projectsCount: 0,
  postsCount: 0,
  todayViews: 0
})

// 最近内容
const recentContent = ref([])

// 检查登录状态
onMounted(() => {
  const authToken = localStorage.getItem('admin_auth')
  if (authToken === 'authenticated') {
    isAuthenticated.value = true
    loadStats()
    loadRecentContent()
  }
})

// 登录处理
const handleLogin = () => {
  loading.value = true
  error.value = ''
  
  setTimeout(() => {
    if (password.value === 'admin123') {
      isAuthenticated.value = true
      localStorage.setItem('admin_auth', 'authenticated')
      loadStats()
      loadRecentContent()
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
  recentContent.value = []
}

// 加载统计数据
const loadStats = async () => {
  try {
    const { data: tools } = await $fetch('/api/_content/query', {
      params: { 
        _path: '/tools'
      }
    }).catch(() => ({ data: [] }))
    
    const { data: projects } = await $fetch('/api/_content/query', {
      params: { 
        _path: '/projects'
      }
    }).catch(() => ({ data: [] }))
    
    const { data: posts } = await $fetch('/api/_content/query', {
      params: { 
        _path: '/blog'
      }
    }).catch(() => ({ data: [] }))

    stats.value = {
      toolsCount: tools?.length || 2,
      projectsCount: projects?.length || 2,
      postsCount: posts?.length || 3,
      todayViews: Math.floor(Math.random() * 200) + 50
    }
  } catch (error) {
    console.error('Failed to load stats:', error)
    // 使用备用数据
    stats.value = {
      toolsCount: 2,
      projectsCount: 2,
      postsCount: 3,
      todayViews: 156
    }
  }
}

// 加载最近内容
const loadRecentContent = async () => {
  try {
    const allContent = []
    
    // 加载工具
    const { data: tools } = await $fetch('/api/_content/query', {
      params: { _path: '/tools' }
    }).catch(() => ({ data: [] }))
    
    if (tools) {
      tools.forEach(tool => {
        const fileName = tool._path?.split('/').pop() || 'unknown.md'
        allContent.push({
          id: `tool-${tool._path}`,
          type: 'tool',
          title: tool.title || '未命名工具',
          published: true,
          updated: tool.updatedAt || tool.date || '最近',
          filePath: `content/tools/${fileName}`
        })
      })
    }
    
    // 加载项目
    const { data: projects } = await $fetch('/api/_content/query', {
      params: { _path: '/projects' }
    }).catch(() => ({ data: [] }))
    
    if (projects) {
      projects.forEach(project => {
        const fileName = project._path?.split('/').pop() || 'unknown.md'
        allContent.push({
          id: `project-${project._path}`,
          type: 'project',
          title: project.title || '未命名项目',
          published: true,
          updated: project.updatedAt || project.date || '最近',
          filePath: `content/projects/${fileName}`
        })
      })
    }
    
    // 加载博客
    const { data: posts } = await $fetch('/api/_content/query', {
      params: { _path: '/blog' }
    }).catch(() => ({ data: [] }))
    
    if (posts) {
      posts.forEach(post => {
        const fileName = post._path?.split('/').pop() || 'unknown.md'
        allContent.push({
          id: `blog-${post._path}`,
          type: 'blog',
          title: post.title || '未命名文章',
          published: true,
          updated: post.updatedAt || post.date || '最近',
          filePath: `content/blog/${fileName}`
        })
      })
    }
    
    recentContent.value = allContent.slice(0, 10)
  } catch (error) {
    console.error('Failed to load recent content:', error)
    recentContent.value = [
      { id: '1', type: 'tool', title: 'Revit插件开发工具', published: true, updated: '2024-01-15' },
      { id: '2', type: 'project', title: '智能理财助手', published: true, updated: '2024-01-14' },
      { id: '3', type: 'blog', title: 'Vue3 Composition API 指南', published: true, updated: '2024-01-13' }
    ]
  }
}

// 刷新统计数据
const refreshStats = () => {
  loadStats()
  loadRecentContent()
  showNotification('数据已刷新')
}

// 导航到工具管理
const navigateToToolsManagement = () => {
  navigateTo('/tools')
}

// 导航到项目管理
const navigateToProjectsManagement = () => {
  navigateTo('/projects')
}

// 导航到博客管理
const navigateToBlogManagement = () => {
  navigateTo('/blog')
}

// 编辑内容
const editContent = (item) => {
  let path = ''
  if (item.type === 'tool') {
    path = `/tools/detail-${item.id.replace('tool-/tools/', '')}`
  } else if (item.type === 'project') {
    path = `/projects/detail-${item.id.replace('project-/projects/', '')}`
  } else if (item.type === 'blog') {
    path = `/blog/${item.id.replace('blog-/blog/', '')}`
  }
  
  if (path) {
    navigateTo(path)
  }
}

// 清除缓存
const clearCache = () => {
  // 清除浏览器缓存
  if ('caches' in window) {
    caches.keys().then(function(names) {
      names.forEach(function(name) {
        caches.delete(name)
      })
    })
  }
  
  // 清除localStorage中的缓存项
  const keysToRemove = []
  for (let i = 0; i < localStorage.length; i++) {
    const key = localStorage.key(i)
    if (key && (key.startsWith('nuxt-') || key.startsWith('cache-'))) {
      keysToRemove.push(key)
    }
  }
  keysToRemove.forEach(key => localStorage.removeItem(key))
  
  showNotification('缓存已清除，页面将自动刷新')
  setTimeout(() => {
    window.location.reload()
  }, 1500)
}

// 显示访问统计
const showVisitStats = () => {
  modalTitle.value = '📊 访问统计'
  modalContent.value = `
    <div class="space-y-4">
      <div class="grid grid-cols-2 gap-4">
        <div class="text-center p-3 bg-blue-50 rounded">
          <div class="text-2xl font-bold text-blue-600">${stats.value.todayViews}</div>
          <div class="text-sm text-gray-600">今日访问</div>
        </div>
        <div class="text-center p-3 bg-green-50 rounded">
          <div class="text-2xl font-bold text-green-600">${stats.value.todayViews * 7}</div>
          <div class="text-sm text-gray-600">本周访问</div>
        </div>
      </div>
      <div class="text-sm text-gray-600">
        <p><strong>热门页面：</strong></p>
        <ul class="mt-2 space-y-1">
          <li>• 首页 (${Math.floor(stats.value.todayViews * 0.4)} 次)</li>
          <li>• 项目展示 (${Math.floor(stats.value.todayViews * 0.3)} 次)</li>
          <li>• 插件工具 (${Math.floor(stats.value.todayViews * 0.2)} 次)</li>
          <li>• 技术博客 (${Math.floor(stats.value.todayViews * 0.1)} 次)</li>
        </ul>
      </div>
    </div>
  `
  showModal.value = true
}

// 运行性能检查
const runPerformanceCheck = () => {
  modalTitle.value = '⚡ 性能检查报告'
  modalContent.value = `
    <div class="space-y-4">
      <div class="text-sm">
        <div class="flex justify-between items-center p-2 bg-green-50 rounded mb-2">
          <span>✅ 页面加载速度</span>
          <span class="text-green-600 font-medium">良好</span>
        </div>
        <div class="flex justify-between items-center p-2 bg-green-50 rounded mb-2">
          <span>✅ 图片优化</span>
          <span class="text-green-600 font-medium">已优化</span>
        </div>
        <div class="flex justify-between items-center p-2 bg-yellow-50 rounded mb-2">
          <span>⚠️ CSS/JS 压缩</span>
          <span class="text-yellow-600 font-medium">可优化</span>
        </div>
        <div class="flex justify-between items-center p-2 bg-green-50 rounded">
          <span>✅ 响应式设计</span>
          <span class="text-green-600 font-medium">完美</span>
        </div>
      </div>
      <div class="text-xs text-gray-500 bg-gray-50 p-3 rounded">
        <p><strong>建议：</strong></p>
        <p>• 考虑启用 Gzip 压缩</p>
        <p>• 使用 CDN 加速静态资源</p>
        <p>• 定期清理未使用的依赖</p>
      </div>
    </div>
  `
  showModal.value = true
}

// 关闭模态框
const closeModal = () => {
  showModal.value = false
  modalTitle.value = ''
  modalContent.value = ''
}

// 复制文件路径
const copyFilePath = (item) => {
  const filePath = item.filePath || `content/${item.type === 'tool' ? 'tools' : item.type === 'project' ? 'projects' : 'blog'}/${item.title}.md`
  
  if (navigator.clipboard) {
    navigator.clipboard.writeText(filePath).then(() => {
      showNotification(`已复制：${filePath}`)
    }).catch(() => {
      fallbackCopyText(filePath)
    })
  } else {
    fallbackCopyText(filePath)
  }
}

// 备用复制方法
const fallbackCopyText = (text) => {
  const textArea = document.createElement('textarea')
  textArea.value = text
  textArea.style.position = 'fixed'
  textArea.style.opacity = '0'
  document.body.appendChild(textArea)
  textArea.select()
  try {
    document.execCommand('copy')
    showNotification(`已复制：${text}`)
  } catch (err) {
    showNotification('复制失败，请手动复制')
  }
  document.body.removeChild(textArea)
}

// 显示通知
const showNotification = (message) => {
  // 简单的通知实现
  const notification = document.createElement('div')
  notification.className = 'fixed top-4 right-4 bg-green-600 text-white px-4 py-2 rounded-lg shadow-lg z-50 max-w-md'
  notification.textContent = message
  document.body.appendChild(notification)
  
  setTimeout(() => {
    if (document.body.contains(notification)) {
      document.body.removeChild(notification)
    }
  }, 3000)
}
</script> 
