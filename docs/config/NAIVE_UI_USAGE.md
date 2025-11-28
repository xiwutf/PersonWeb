# Naive UI 使用指南

## 📦 已安装的包

- `naive-ui` - UI 组件库
- `vfonts` - 推荐字体（可选）

## 🚀 基本使用

### 1. 在组件中按需引入

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

### 2. 表格组件示例

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
import { NDataTable, NButton, NTag } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'

const loading = ref(false)
const data = ref([])

const columns: DataTableColumns = [
  {
    title: '标题',
    key: 'title'
  },
  {
    title: '状态',
    key: 'status',
    render(row: any) {
      return h(NTag, { type: row.status === 1 ? 'success' : 'default' }, {
        default: () => row.status === 1 ? '已发布' : '草稿'
      })
    }
  },
  {
    title: '操作',
    key: 'actions',
    render(row: any) {
      return h('div', { class: 'flex gap-2' }, [
        h(NButton, { size: 'small', type: 'primary' }, { default: () => '编辑' }),
        h(NButton, { size: 'small', type: 'error' }, { default: () => '删除' })
      ])
    }
  }
]

const pagination = {
  pageSize: 10
}
</script>
```

### 3. 表单组件示例

```vue
<template>
  <n-form
    ref="formRef"
    :model="formData"
    :rules="rules"
    label-placement="left"
    label-width="auto"
  >
    <n-form-item label="标题" path="title">
      <n-input v-model:value="formData.title" placeholder="请输入标题" />
    </n-form-item>
    <n-form-item label="分类" path="category">
      <n-select
        v-model:value="formData.category"
        :options="categoryOptions"
        placeholder="请选择分类"
      />
    </n-form-item>
    <n-form-item>
      <n-button type="primary" @click="handleSubmit">提交</n-button>
      <n-button @click="handleReset">重置</n-button>
    </n-form-item>
  </n-form>
</template>

<script setup lang="ts">
import { NForm, NFormItem, NInput, NSelect, NButton, type FormInst } from 'naive-ui'

const formRef = ref<FormInst | null>(null)
const formData = ref({
  title: '',
  category: null
})

const rules = {
  title: {
    required: true,
    message: '请输入标题',
    trigger: 'blur'
  }
}

const categoryOptions = [
  { label: '技术文章', value: 1 },
  { label: '生活随笔', value: 2 }
]

const handleSubmit = () => {
  formRef.value?.validate((errors) => {
    if (!errors) {
      console.log('提交', formData.value)
    }
  })
}

const handleReset = () => {
  formRef.value?.restoreValidation()
}
</script>
```

### 4. 消息提示

```vue
<script setup lang="ts">
import { useMessage, useDialog } from 'naive-ui'

const message = useMessage()
const dialog = useDialog()

// 成功提示
message.success('操作成功')

// 错误提示
message.error('操作失败')

// 确认对话框
const handleDelete = () => {
  dialog.warning({
    title: '确认删除',
    content: '确定要删除吗？此操作不可恢复。',
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: () => {
      // 执行删除
      message.success('删除成功')
    }
  })
}
</script>
```

### 5. 主题配置（深色模式）

```vue
<template>
  <n-config-provider :theme="theme">
    <n-global-style />
    <!-- 你的内容 -->
  </n-config-provider>
</template>

<script setup lang="ts">
import { NConfigProvider, darkTheme, NGlobalStyle } from 'naive-ui'

const theme = computed(() => {
  // 根据你的主题切换逻辑返回 darkTheme 或 null
  return darkTheme
})
</script>
```

## 🎨 常用组件列表

### 数据展示
- `n-data-table` - 数据表格
- `n-card` - 卡片
- `n-tag` - 标签
- `n-badge` - 徽章
- `n-statistic` - 统计数值

### 数据录入
- `n-input` - 输入框
- `n-select` - 选择器
- `n-date-picker` - 日期选择
- `n-switch` - 开关
- `n-checkbox` - 复选框
- `n-radio` - 单选框
- `n-form` - 表单

### 反馈
- `n-button` - 按钮
- `n-message` - 消息提示
- `n-notification` - 通知
- `n-dialog` - 对话框
- `n-loading-bar` - 加载条
- `n-spin` - 加载中

### 导航
- `n-menu` - 菜单
- `n-pagination` - 分页
- `n-tabs` - 标签页
- `n-breadcrumb` - 面包屑

### 布局
- `n-layout` - 布局容器
- `n-space` - 间距
- `n-divider` - 分割线
- `n-grid` - 网格

## 💡 最佳实践

1. **按需引入**：只引入需要的组件，减少打包体积
2. **使用 TypeScript**：充分利用类型提示
3. **主题定制**：使用 `n-config-provider` 统一管理主题
4. **消息提示**：使用 `useMessage` 和 `useDialog` composable
5. **响应式**：Naive UI 组件自带响应式，配合 Tailwind 使用

## 🔗 参考文档

- [Naive UI 官方文档](https://www.naiveui.com/)
- [Naive UI GitHub](https://github.com/tusen-ai/naive-ui)

