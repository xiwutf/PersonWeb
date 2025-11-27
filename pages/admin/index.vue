<template>
  <div class="min-h-screen bg-gray-100 flex">
    <!-- 侧边栏 -->
    <aside class="w-64 bg-slate-800 text-white flex flex-col">
      <div class="p-6 text-xl font-bold border-b border-slate-700">
        管理后台
      </div>
      <nav class="flex-1 p-4 space-y-2">
        <NuxtLink to="/admin" class="block px-4 py-2 rounded hover:bg-slate-700" :class="{ 'bg-slate-700': route.path === '/admin' }">
          📊 仪表盘
        </NuxtLink>
        <NuxtLink to="/admin/articles" class="block px-4 py-2 rounded hover:bg-slate-700" :class="{ 'bg-slate-700': route.path.startsWith('/admin/articles') }">
          📝 文章管理
        </NuxtLink>
        <NuxtLink to="/admin/tools" class="block px-4 py-2 rounded hover:bg-slate-700" :class="{ 'bg-slate-700': route.path.startsWith('/admin/tools') }">
          🛠️ 工具管理
        </NuxtLink>
      </nav>
      <div class="p-4 border-t border-slate-700">
        <button @click="logout" class="w-full px-4 py-2 text-left hover:bg-slate-700 rounded text-red-300">
          🚪 退出登录
        </button>
      </div>
    </aside>

    <!-- 主内容区 -->
    <main class="flex-1 p-8 overflow-y-auto">
      <header class="flex justify-between items-center mb-8">
        <h1 class="text-2xl font-bold text-gray-800">仪表盘</h1>
        <div class="text-gray-600">欢迎回来，Admin</div>
      </header>

      <!-- 统计卡片 -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div class="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
          <div class="text-gray-500 text-sm mb-2">总文章数</div>
          <div class="text-3xl font-bold text-blue-600">12</div>
        </div>
        <div class="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
          <div class="text-gray-500 text-sm mb-2">总工具数</div>
          <div class="text-3xl font-bold text-purple-600">5</div>
        </div>
        <div class="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
          <div class="text-gray-500 text-sm mb-2">今日访问</div>
          <div class="text-3xl font-bold text-green-600">128</div>
        </div>
      </div>

      <!-- 快捷操作 -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <h2 class="text-lg font-bold mb-4">快捷操作</h2>
        <div class="flex gap-4">
          <NuxtLink to="/admin/article-edit" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
            + 发布新文章
          </NuxtLink>
          <button class="px-4 py-2 bg-gray-100 text-gray-700 rounded hover:bg-gray-200">
            刷新缓存
          </button>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: false,
  middleware: 'admin-auth'
})

const route = useRoute()
const router = useRouter()

const logout = () => {
  if (process.client) {
    localStorage.removeItem('admin_token')
    router.push('/admin/login')
  }
}
</script>
