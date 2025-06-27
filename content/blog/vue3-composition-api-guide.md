---
title: "Vue 3 Composition API 实战指南"
date: "2024-01-15"
tags: ["Vue.js", "JavaScript", "前端开发", "Composition API"]
description: "深入理解Vue 3 Composition API的核心概念和实际应用，提升开发效率和代码质量"
author: "溪午听风"
category: "技术文章"
cover: "/images/vue3-composition-api.jpg"
---

# Vue 3 Composition API 实战指南

Vue 3 带来了许多激动人心的新特性，其中最重要的就是 Composition API。本文将深入探讨如何在实际项目中有效使用 Composition API。

## 🎯 为什么选择 Composition API？

### 传统 Options API 的局限性

在 Vue 2 中，我们习惯使用 Options API：

```javascript
export default {
  data() {
    return {
      count: 0,
      loading: false
    }
  },
  methods: {
    increment() {
      this.count++
    }
  },
  mounted() {
    this.fetchData()
  }
}
```

虽然这种方式直观易懂，但在复杂组件中会遇到以下问题：

1. **逻辑分散**：相关的逻辑被分散在不同的选项中
2. **复用困难**：难以在组件间复用逻辑
3. **类型推导**：TypeScript 支持不够完善

### Composition API 的优势

```javascript
import { ref, onMounted } from 'vue'

export default {
  setup() {
    const count = ref(0)
    const loading = ref(false)
    
    const increment = () => {
      count.value++
    }
    
    const fetchData = async () => {
      loading.value = true
      // 获取数据逻辑
      loading.value = false
    }
    
    onMounted(() => {
      fetchData()
    })
    
    return {
      count,
      loading,
      increment,
      fetchData
    }
  }
}
```

## 🔧 核心 API 详解

### 1. ref() - 响应式引用

`ref()` 用于创建响应式的数据引用：

```javascript
import { ref, computed } from 'vue'

const count = ref(0)
const doubleCount = computed(() => count.value * 2)

// 在模板中自动解包，不需要 .value
// 在 JavaScript 中需要使用 .value
console.log(count.value) // 0
count.value++
console.log(count.value) // 1
```

### 2. reactive() - 响应式对象

`reactive()` 用于创建响应式的对象：

```javascript
import { reactive } from 'vue'

const state = reactive({
  count: 0,
  user: {
    name: '溪午听风',
    email: 'contact@溪午听风.com'
  }
})

// 直接访问属性，无需 .value
state.count++
state.user.name = '新名称'
```

### 3. computed() - 计算属性

```javascript
import { ref, computed } from 'vue'

const firstName = ref('张')
const lastName = ref('三')

const fullName = computed({
  get() {
    return firstName.value + lastName.value
  },
  set(value) {
    [firstName.value, lastName.value] = value.split(' ')
  }
})
```

### 4. watch() 和 watchEffect() - 侦听器

```javascript
import { ref, watch, watchEffect } from 'vue'

const count = ref(0)
const message = ref('')

// watch - 明确指定依赖
watch(count, (newValue, oldValue) => {
  console.log(`count 从 ${oldValue} 变为 ${newValue}`)
})

// watchEffect - 自动收集依赖
watchEffect(() => {
  message.value = `当前计数：${count.value}`
})
```

## 🏗️ 实战案例：用户管理组件

让我们通过一个完整的用户管理组件来看看 Composition API 的实际应用：

```vue
<template>
  <div class="user-manager">
    <div class="header">
      <h2>用户管理</h2>
      <button @click="showAddForm = true" class="btn-primary">
        添加用户
      </button>
    </div>
    
    <!-- 搜索框 -->
    <div class="search-box">
      <input
        v-model="searchQuery"
        placeholder="搜索用户..."
        class="search-input"
      >
    </div>
    
    <!-- 用户列表 -->
    <div v-if="loading" class="loading">
      加载中...
    </div>
    
    <div v-else class="user-list">
      <div
        v-for="user in filteredUsers"
        :key="user.id"
        class="user-card"
      >
        <h3>{{ user.name }}</h3>
        <p>{{ user.email }}</p>
        <div class="actions">
          <button @click="editUser(user)">编辑</button>
          <button @click="deleteUser(user.id)">删除</button>
        </div>
      </div>
    </div>
    
    <!-- 添加/编辑表单 -->
    <UserForm
      v-if="showAddForm || editingUser"
      :user="editingUser"
      @save="handleSave"
      @cancel="handleCancel"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useUsers } from '@/composables/useUsers'
import UserForm from './UserForm.vue'

// 使用自定义组合式函数
const {
  users,
  loading,
  fetchUsers,
  addUser,
  updateUser,
  deleteUser
} = useUsers()

// 本地状态
const searchQuery = ref('')
const showAddForm = ref(false)
const editingUser = ref(null)

// 计算属性
const filteredUsers = computed(() => {
  if (!searchQuery.value) return users.value
  
  return users.value.filter(user =>
    user.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    user.email.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

// 方法
const editUser = (user) => {
  editingUser.value = { ...user }
}

const handleSave = async (userData) => {
  if (editingUser.value) {
    await updateUser(editingUser.value.id, userData)
    editingUser.value = null
  } else {
    await addUser(userData)
    showAddForm.value = false
  }
}

const handleCancel = () => {
  editingUser.value = null
  showAddForm.value = false
}

// 生命周期
onMounted(() => {
  fetchUsers()
})
</script>
```

## 📦 组合式函数（Composables）

将相关逻辑提取到可复用的组合式函数中：

```javascript
// composables/useUsers.js
import { ref } from 'vue'

export function useUsers() {
  const users = ref([])
  const loading = ref(false)
  const error = ref(null)
  
  const fetchUsers = async () => {
    loading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/users')
      users.value = await response.json()
    } catch (err) {
      error.value = err.message
    } finally {
      loading.value = false
    }
  }
  
  const addUser = async (userData) => {
    try {
      const response = await fetch('/api/users', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userData)
      })
      const newUser = await response.json()
      users.value.push(newUser)
    } catch (err) {
      error.value = err.message
    }
  }
  
  const updateUser = async (id, userData) => {
    try {
      const response = await fetch(`/api/users/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userData)
      })
      const updatedUser = await response.json()
      
      const index = users.value.findIndex(u => u.id === id)
      if (index !== -1) {
        users.value[index] = updatedUser
      }
    } catch (err) {
      error.value = err.message
    }
  }
  
  const deleteUser = async (id) => {
    try {
      await fetch(`/api/users/${id}`, { method: 'DELETE' })
      users.value = users.value.filter(u => u.id !== id)
    } catch (err) {
      error.value = err.message
    }
  }
  
  return {
    users,
    loading,
    error,
    fetchUsers,
    addUser,
    updateUser,
    deleteUser
  }
}
```

## 🎨 最佳实践

### 1. 合理使用 ref 和 reactive

```javascript
// ✅ 推荐：基本类型使用 ref
const count = ref(0)
const name = ref('溪午听风')

// ✅ 推荐：对象使用 reactive
const user = reactive({
  name: '溪午听风',
  email: 'contact@溪午听风.com'
})

// ❌ 不推荐：对象使用 ref（需要 .value）
const user = ref({
  name: '溪午听风',
  email: 'contact@溪午听风.com'
})
```

### 2. 组合式函数命名规范

```javascript
// ✅ 使用 use 前缀
export function useCounter() { }
export function useUsers() { }
export function useLocalStorage() { }

// ❌ 避免混淆的命名
export function counter() { }
export function getUsers() { }
```

### 3. 合理组织代码

```javascript
export default {
  setup() {
    // 1. 响应式数据
    const count = ref(0)
    const users = ref([])
    
    // 2. 计算属性
    const doubleCount = computed(() => count.value * 2)
    
    // 3. 方法
    const increment = () => count.value++
    const fetchUsers = async () => { /* ... */ }
    
    // 4. 生命周期钩子
    onMounted(() => {
      fetchUsers()
    })
    
    // 5. 返回需要在模板中使用的内容
    return {
      count,
      doubleCount,
      increment,
      users
    }
  }
}
```

## 🚀 性能优化技巧

### 1. 使用 shallowRef 和 shallowReactive

```javascript
import { shallowRef, shallowReactive } from 'vue'

// 当你只需要对象的第一层是响应式的
const state = shallowReactive({
  user: { name: '溪午听风' }, // 这一层不是响应式的
  count: 0 // 这一层是响应式的
})

// 当你有大型对象但只需要替换整个对象时
const largeObject = shallowRef({ /* 大量数据 */ })
```

### 2. 使用 readonly

```javascript
import { readonly, ref } from 'vue'

const count = ref(0)
const readonlyCount = readonly(count)

// readonlyCount.value++ // 这会警告
```

## 📚 总结

Composition API 为 Vue 3 带来了：

1. **更好的逻辑复用**：通过组合式函数
2. **更好的类型推导**：完美支持 TypeScript
3. **更灵活的代码组织**：按功能而不是选项组织代码
4. **更好的性能**：更精确的依赖追踪

虽然学习曲线稍微陡峭，但掌握后会显著提升开发效率和代码质量。

## 🔗 相关资源

- [Vue 3 官方文档](https://vuejs.org/)
- [Composition API RFC](https://github.com/vuejs/rfcs/blob/master/active-rfcs/0013-composition-api.md)
- [VueUse 组合式函数库](https://vueuse.org/)

---

*希望这篇文章能帮助你更好地理解和使用 Vue 3 Composition API。如果有任何问题，欢迎在评论区讨论！* 