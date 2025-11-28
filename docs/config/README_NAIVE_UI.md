# Naive UI 集成完成 ✅

## 📦 已安装

- ✅ `naive-ui` - UI 组件库
- ✅ `vfonts` - 推荐字体（可选）

## 🚀 快速开始

### 1. 在组件中使用

```vue
<template>
  <div>
    <n-button type="primary" @click="handleClick">按钮</n-button>
    <n-input v-model:value="inputValue" placeholder="请输入" />
  </div>
</template>

<script setup lang="ts">
import { NButton, NInput } from 'naive-ui'

const inputValue = ref('')
const handleClick = () => {
  console.log('clicked')
}
</script>
```

### 2. 使用消息提示

```vue
<script setup lang="ts">
import { useMessage, useDialog } from 'naive-ui'

const message = useMessage()
const dialog = useDialog()

// 成功提示
message.success('操作成功')

// 确认对话框
dialog.warning({
  title: '确认删除',
  content: '确定要删除吗？',
  positiveText: '确定',
  negativeText: '取消',
  onPositiveClick: () => {
    // 执行操作
  }
})
</script>
```

### 3. 使用数据表格

```vue
<template>
  <n-data-table
    :columns="columns"
    :data="data"
    :loading="loading"
    :pagination="pagination"
  />
</template>

<script setup lang="ts">
import { NDataTable, NTag, NButton, h } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'

const columns: DataTableColumns = [
  { title: '标题', key: 'title' },
  {
    title: '状态',
    key: 'status',
    render(row) {
      return h(NTag, { type: 'success' }, { default: () => '已发布' })
    }
  }
]
</script>
```

## 📚 文档

- [使用指南](./NAIVE_UI_USAGE.md)
- [组件库对比](./UI_COMPONENT_LIBRARY.md)
- [Naive UI 官方文档](https://www.naiveui.com/)

## 💡 提示

1. **按需引入**：只引入需要的组件，减少打包体积
2. **TypeScript 支持**：充分利用类型提示
3. **主题定制**：可以在 layouts/admin.vue 中使用 `NConfigProvider` 统一管理主题

