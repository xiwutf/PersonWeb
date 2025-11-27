<template>
  <div class="min-h-screen bg-gray-100 flex">
    <!-- 侧边栏 -->
    <aside class="w-64 bg-slate-800 text-white flex flex-col fixed h-full left-0 top-0 z-10">
      <div class="p-6 text-xl font-bold border-b border-slate-700 flex items-center gap-2">
        <span>管理后台</span>
      </div>
      <nav class="flex-1 p-4 space-y-2 overflow-y-auto">
        <NuxtLink to="/admin" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-chart-line w-6 text-center mr-2"></i>
          仪表盘
        </NuxtLink>
        
        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">内容管理</div>
        
        <NuxtLink to="/admin/articles" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-file-alt w-6 text-center mr-2"></i>
          文章管理
        </NuxtLink>
        
        <NuxtLink to="/admin/categories" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-folder w-6 text-center mr-2"></i>
          分类管理
        </NuxtLink>
        
        <NuxtLink to="/admin/tools" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-tools w-6 text-center mr-2"></i>
          工具管理
        </NuxtLink>

        <div class="text-xs text-slate-400 px-4 mt-4 mb-2 uppercase font-semibold">系统设置</div>
        
        <NuxtLink to="/admin/config" class="flex items-center px-4 py-2 rounded hover:bg-slate-700 transition-colors" active-class="bg-blue-600 hover:bg-blue-600">
          <i class="fas fa-cog w-6 text-center mr-2"></i>
          站点配置
        </NuxtLink>
      </nav>
      
      <div class="p-4 border-t border-slate-700">
        <button @click="logout" class="w-full flex items-center px-4 py-2 text-left hover:bg-slate-700 rounded text-red-300 transition-colors">
          <i class="fas fa-sign-out-alt w-6 text-center mr-2"></i>
          退出登录
        </button>
      </div>
    </aside>

    <!-- 主内容区 -->
    <main class="flex-1 ml-64 p-8">
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts">
const router = useRouter()

const logout = () => {
  if (confirm('确定要退出登录吗？')) {
    if (process.client) {
      localStorage.removeItem('admin_token')
      localStorage.removeItem('admin_user')
      router.push('/admin/login')
    }
  }
}
</script>

