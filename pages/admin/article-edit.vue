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
        <h1 class="text-2xl font-bold text-gray-800">编辑文章</h1>
        <NuxtLink to="/admin/articles" class="text-gray-600 hover:text-gray-800">
          取消
        </NuxtLink>
      </div>

      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <form class="space-y-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">文章标题</label>
            <input type="text" class="w-full border border-gray-300 rounded px-3 py-2" placeholder="输入标题" />
          </div>

          <div class="grid grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">分类</label>
              <select class="w-full border border-gray-300 rounded px-3 py-2">
                <option>技术文章</option>
                <option>运维部署</option>
                <option>生活随笔</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">标签 (逗号分隔)</label>
              <input type="text" class="w-full border border-gray-300 rounded px-3 py-2" placeholder="Vue, Nuxt, Tutorial" />
            </div>
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">摘要</label>
            <textarea class="w-full border border-gray-300 rounded px-3 py-2 h-20" placeholder="文章简短描述..."></textarea>
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">内容 (Markdown)</label>
            <textarea class="w-full border border-gray-300 rounded px-3 py-2 h-96 font-mono" placeholder="# Hello World..."></textarea>
          </div>

          <div class="flex justify-end gap-4">
            <button type="button" class="px-6 py-2 border border-gray-300 rounded text-gray-700 hover:bg-gray-50">存草稿</button>
            <button type="submit" class="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">发布文章</button>
          </div>
        </form>
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
