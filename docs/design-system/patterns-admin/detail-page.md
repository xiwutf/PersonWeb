# DetailPage - 详情页面组件

详情页面组件，支持多种数据类型和分组展示。

## 基础用法

```vue
<template>
  <DetailPage
    title="用户详情"
    :data="user"
    :fields="detailFields"
    :loading="loading"
  >
    <template #actions>
      <n-button @click="handleEdit">编辑</n-button>
      <n-button type="error" @click="handleDelete">删除</n-button>
    </template>
  </DetailPage>
</template>

<script setup lang="ts">
import DetailPage from '~/components/admin/patterns/DetailPage.vue'

const user = ref({
  id: 1,
  name: '张三',
  email: 'zhang@example.com',
  role: '管理员',
  status: '正常',
  createdAt: '2024-01-15',
  lastLoginAt: '2024-03-10 10:30:00'
})

const loading = ref(false)

const detailFields = [
  {
    key: 'name',
    label: '用户名',
    type: 'text'
  },
  {
    key: 'email',
    label: '邮箱',
    type: 'text'
  },
  {
    key: 'role',
    label: '角色',
    type: 'tag'
  },
  {
    key: 'status',
    label: '状态',
    type: 'tag',
    variant: 'status'
  },
  {
    key: 'createdAt',
    label: '创建时间',
    type: 'date'
  },
  {
    key: 'lastLoginAt',
    label: '最后登录',
    type: 'datetime'
  }
]

const handleEdit = () => {
  navigateTo(`/users/${user.value.id}/edit`)
}

const handleDelete = async () => {
  const dialog = useDialog()
  dialog.warning({
    title: '确认删除',
    content: '确定要删除该用户吗？',
    positiveText: '删除',
    negativeText: '取消',
    onPositiveClick: async () => {
      await api.delete(`/users/${user.value.id}`)
      message.success('删除成功')
      navigateTo('/users')
    }
  })
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `title` | `string` | - | 页面标题 |
| `data` | `Record<string, any>` | `{}` | 详情数据 |
| `fields` | `DetailField[]` | - | 详情字段配置 |
| `loading` | `boolean` | `false` | 加载状态 |
| `grouped` | `boolean` | `false` | 是否分组显示 |
| `show-back` | `boolean` | `true` | 是否显示返回按钮 |

## Slots

| 插槽名 | 说明 |
|---------|------|
| `title` | 自定义标题内容 |
| `actions` | 操作按钮区域 |
| `field-{key}` | 自定义字段内容 |
| `group-{name}` | 自定义分组内容 |

## 详情字段类型

### Text 类型

```typescript
{
  key: 'name',
  label: '用户名',
  type: 'text'
}
```

### Number 类型

```typescript
{
  key: 'price',
  label: '价格',
  type: 'number',
  prefix: '¥',
  precision: 2
}
```

### Date 类型

```typescript
{
  key: 'createdAt',
  label: '创建时间',
  type: 'date',
  format: 'yyyy-MM-dd'
}
```

### DateTime 类型

```typescript
{
  key: 'updatedAt',
  label: '更新时间',
  type: 'datetime',
  format: 'yyyy-MM-dd HH:mm:ss'
}
```

### Tag 类型

```typescript
{
  key: 'status',
  label: '状态',
  type: 'tag',
  variant: 'status'  // 使用状态颜色
}
```

### Image 类型

```typescript
{
  key: 'avatar',
  label: '头像',
  type: 'image',
  width: 80,
  height: 80
}
```

### Link 类型

```typescript
{
  key: 'website',
  label: '网站',
  type: 'link',
  target: '_blank'
}
```

### JSON 类型

```typescript
{
  key: 'metadata',
  label: '元数据',
  type: 'json',
  collapsed: true
}
```

## 交互式示例

### 基础详情

```vue
<DetailPage
  title="用户详情"
  :data="user"
  :fields="detailFields"
/>
```

### 带操作按钮

```vue
<DetailPage
  title="文章详情"
  :data="article"
  :fields="detailFields"
>
  <template #actions>
    <n-button @click="handleEdit">编辑</n-button>
    <n-button @click="handlePublish">发布</n-button>
  </template>
</DetailPage>
```

### 分组详情

```vue
<DetailPage
  title="订单详情"
  :data="order"
  :fields="detailFields"
  :grouped="true"
/>
```

### 自定义字段内容

```vue
<DetailPage
  title="用户详情"
  :data="user"
  :fields="detailFields"
>
  <template #field-avatar>
    <n-avatar :size="80" :src="user.avatar">
      {{ user.name }}
    </n-avatar>
  </template>

  <template #field-status>
    <n-tag :type="user.status === 'active' ? 'success' : 'error'">
      {{ user.statusText }}
    </n-tag>
  </template>
</DetailPage>
```

## DetailField 类型定义

```typescript
interface DetailField {
  key: string                    // 字段键名
  label: string                  // 字段标签
  type: 'text' | 'number' | 'date' | 'datetime' | 'tag' | 'image' | 'link' | 'json' | 'html'  // 字段类型
  format?: string               // 格式化字符串（date/datetime 类型）
  prefix?: string              // 前缀（number 类型）
  suffix?: string              // 后缀（number 类型）
  precision?: number            // 小数位数（number 类型）
  width?: number               // 图片宽度（image 类型）
  height?: number              // 图片高度（image 类型）
  variant?: string             // 标签变体（tag 类型）
  target?: string              // 链接目标（link 类型）
  collapsed?: boolean          // 是否折叠（json 类型）
  group?: string              // 分组名称
}
```

## 样式定制

```vue
<DetailPage
  :data="user"
  :fields="detailFields"
  class="custom-detail-page"
/>

<style scoped>
.custom-detail-page {
  /* 自定义详情页样式 */
}
</style>
```

## 使用场景

1. **数据详情** - 用户、文章、订单等详情查看
2. **配置详情** - 系统设置、参数配置查看
3. **日志详情** - 操作日志、错误日志详情

## 最佳实践

1. 合理组织字段，按逻辑分组
2. 重要信息放在前面
3. 使用合适的字段类型展示
4. 提供清晰的返回导航
5. 操作按钮明确标注风险

## 相关组件

- [PageHeader](./page-header.md)
- [FormPage](./form-page.md)
- [ConfigPage](./config-page.md)

## 源码位置

`components/admin/patterns/DetailPage.vue`
