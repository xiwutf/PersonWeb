<template>
  <div class="min-h-screen bg-gray-100 flex">
    <!-- 侧边栏 (复用) -->
    <aside class="w-64 bg-slate-800 text-white flex flex-col">
      <div class="p-6 text-xl font-bold border-b border-slate-700">管理后台</div>
      <nav class="flex-1 p-4 space-y-2">
        <NuxtLink to="/admin" class="block px-4 py-2 rounded hover:bg-slate-700">📊 仪表盘</NuxtLink>
        <NuxtLink to="/admin/articles" class="block px-4 py-2 rounded bg-slate-700">📝 文章管理</NuxtLink>
        <NuxtLink to="/admin/tools" class="block px-4 py-2 rounded hover:bg-slate-700">🛠️ 工具管理</NuxtLink>
      </nav>
      <div class="p-4 border-t border-slate-700">
        <button @click="logout" class="w-full px-4 py-2 text-left hover:bg-slate-700 rounded text-red-300">🚪 退出登录</button>
      </div>
    </aside>

    <main class="flex-1 p-8 overflow-y-auto">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold text-gray-800">文章管理</h1>
        <NuxtLink to="/admin/article-edit" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
          + 新增文章
        </NuxtLink>
      </div>

      <!-- 搜索栏 -->
      <div class="bg-white p-4 rounded-lg shadow-sm border border-gray-200 mb-6 flex gap-4">
        <input type="text" placeholder="搜索文章标题..." class="flex-1 border border-gray-300 rounded px-3 py-2" />
        <button class="px-4 py-2 bg-gray-100 text-gray-700 rounded hover:bg-gray-200">搜索</button>
      </div>

      <!-- 文章列表表格 -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        <table class="w-full text-left">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-sm font-medium text-gray-500">标题</th>
              <th class="px-6 py-3 text-sm font-medium text-gray-500">分类</th>
              <th class="px-6 py-3 text-sm font-medium text-gray-500">发布时间</th>
              <th class="px-6 py-3 text-sm font-medium text-gray-500">状态</th>
              <th class="px-6 py-3 text-sm font-medium text-gray-500">操作</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200">
            <tr v-for="article in articles" :key="article.id" class="hover:bg-gray-50">
              <td class="px-6 py-4">{{ article.title }}</td>
              <td class="px-6 py-4">
                <span class="px-2 py-1 bg-blue-100 text-blue-800 rounded text-xs">{{ article.category }}</span>
              </td>
              <td class="px-6 py-4 text-gray-500 text-sm">{{ article.date }}</td>
              <td class="px-6 py-4">
                <span class="text-green-600 text-sm">已发布</span>
              </td>
              <td class="px-6 py-4">
                <div class="flex gap-2">
                  <button class="text-blue-600 hover:text-blue-800">编辑</button>
                  <button class="text-red-600 hover:text-red-800">删除</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: false,
  middleware: 'admin-auth'
})

const router = useRouter()
const logout = () => {
  if (process.client) {
    localStorage.removeItem('admin_token')
    router.push('/admin/login')
  }
}

// 模拟数据
const articles = [
  { id: 1, title: 'Linux 新手入门', category: '运维部署', date: '2024-05-20' },
  { id: 2, title: 'Vue3 组合式 API 指南', category: '技术文章', date: '2024-05-18' },
  { id: 3, title: 'Revit 插件开发心得', category: '项目总结', date: '2024-05-15' },
]
</script>
