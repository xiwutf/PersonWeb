<template>
  <div class="min-h-screen bg-gray-100 flex">
    <!-- 侧边栏 (复用) -->
    <aside class="w-64 bg-slate-800 text-white flex flex-col">
      <div class="p-6 text-xl font-bold border-b border-slate-700">管理后台</div>
      <nav class="flex-1 p-4 space-y-2">
        <NuxtLink to="/admin" class="block px-4 py-2 rounded hover:bg-slate-700">📊 仪表盘</NuxtLink>
        <NuxtLink to="/admin/articles" class="block px-4 py-2 rounded hover:bg-slate-700">📝 文章管理</NuxtLink>
        <NuxtLink to="/admin/tools" class="block px-4 py-2 rounded bg-slate-700">🛠️ 工具管理</NuxtLink>
      </nav>
      <div class="p-4 border-t border-slate-700">
        <button @click="logout" class="w-full px-4 py-2 text-left hover:bg-slate-700 rounded text-red-300">🚪 退出登录</button>
      </div>
    </aside>

    <main class="flex-1 p-8 overflow-y-auto">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold text-gray-800">工具管理</h1>
        <button class="px-4 py-2 bg-purple-600 text-white rounded hover:bg-purple-700">
          + 新增工具
        </button>
      </div>

      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center text-gray-500">
        <div class="text-4xl mb-4">🛠️</div>
        <p>工具管理功能开发中...</p>
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
</script>
