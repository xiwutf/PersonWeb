<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100">
    <div class="max-w-md w-full bg-white rounded-lg shadow-md p-8">
      <h2 class="text-2xl font-bold text-center text-gray-800 mb-8">后台管理系统登录</h2>
      
      <form @submit.prevent="handleLogin" class="space-y-6">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">用户名</label>
          <input 
            v-model="username"
            type="text" 
            class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="admin"
          />
        </div>
        
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">密码</label>
          <input 
            v-model="password"
            type="password" 
            class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="123456"
          />
        </div>
        
        <div v-if="error" class="text-red-500 text-sm text-center">
          {{ error }}
        </div>
        
        <button 
          type="submit"
          class="w-full bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 transition"
          :disabled="loading"
        >
          {{ loading ? '登录中...' : '登录' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: false // 不使用默认布局（包含前台 Header/Footer）
})

const username = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)
const router = useRouter()

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  
  // 模拟登录请求
  setTimeout(() => {
    if (username.value === 'admin' && password.value === '123456') {
      // 登录成功，设置 Token
      if (process.client) {
        localStorage.setItem('admin_token', 'mock-token-123456')
      }
      router.push('/admin')
    } else {
      error.value = '用户名或密码错误'
    }
    loading.value = false
  }, 1000)
  
  // 实际项目中调用 API:
  // const api = useApi()
  // const res = await api.post('/auth/login', { username, password })
}
</script>
