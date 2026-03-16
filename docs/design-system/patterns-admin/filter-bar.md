# FilterBar - 筛选栏组件

灵活的筛选栏组件，支持多种筛选类型和布局。

## 基础用法

```vue
<template>
  <FilterBar
    :fields="filterFields"
    :layout="'horizontal'"
    @filter-change="handleFilter"
    @filter-reset="handleReset"
  />
</template>

<script setup lang="ts">
import FilterBar from '~/components/admin/patterns/FilterBar.vue'

const filterFields = [
  {
    key: 'name',
    label: '用户名',
    type: 'input',
    placeholder: '输入用户名'
  },
  {
    key: 'status',
    label: '状态',
    type: 'select',
    options: [
      { label: '全部', value: '' },
      { label: '正常', value: 'active' },
      { label: '禁用', value: 'disabled' }
    ]
  },
  {
    key: 'dateRange',
    label: '创建时间',
    type: 'date-range'
  }
]

const handleFilter = (values: Record<string, any>) => {
  console.log('筛选条件：', values)
  // 执行筛选逻辑
}

const handleReset = () => {
  console.log('重置筛选')
  // 重置筛选逻辑
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `fields` | `FilterField[]` | - | 筛选字段配置 |
| `layout` | `'horizontal' \| 'vertical' \| 'grid'` | `'horizontal'` | 布局方式 |
| `collapsible` | `boolean` | `false` | 是否可折叠 |
| `default-collapsed` | `boolean` | `false` | 默认折叠状态 |
| `loading` | `boolean` | `false` | 加载状态 |

## 筛选字段类型

### Input 类型

```typescript
{
  key: 'name',
  label: '用户名',
  type: 'input',
  placeholder: '输入用户名',
  clearable: true
}
```

### Select 类型

```typescript
{
  key: 'status',
  label: '状态',
  type: 'select',
  placeholder: '选择状态',
  options: [
    { label: '全部', value: '' },
    { label: '正常', value: 'active' },
    { label: '禁用', value: 'disabled' }
  ],
  multiple: false,
  clearable: true
}
```

### Date Range 类型

```typescript
{
  key: 'dateRange',
  label: '创建时间',
  type: 'date-range',
  format: 'yyyy-MM-dd',
  clearable: true
}
```

### Number Range 类型

```typescript
{
  key: 'priceRange',
  label: '价格区间',
  type: 'number-range',
  placeholder: ['最小价格', '最大价格']
}
```

## 事件

| 事件 | 说明 |
|------|------|
| `filter-change` | 筛选条件变化时触发 `(values: Record<string, any>)` |
| `filter-reset` | 重置筛选时触发 |

## 交互式示例

### 水平布局（默认）

```vue
<FilterBar
  :fields="filterFields"
  layout="horizontal"
  @filter-change="handleFilter"
/>
```

### 垂直布局

```vue
<FilterBar
  :fields="filterFields"
  layout="vertical"
  @filter-change="handleFilter"
/>
```

### 网格布局

```vue
<FilterBar
  :fields="filterFields"
  layout="grid"
  @filter-change="handleFilter"
/>
```

### 可折叠筛选栏

```vue
<FilterBar
  :fields="filterFields"
  :collapsible="true"
  :default-collapsed="false"
  @filter-change="handleFilter"
/>
```

### 完整示例

```vue
<FilterBar
  :fields="[
    {
      key: 'keyword',
      label: '关键词',
      type: 'input',
      placeholder: '搜索用户名、邮箱...'
    },
    {
      key: 'role',
      label: '角色',
      type: 'select',
      options: [
        { label: '全部', value: '' },
        { label: '管理员', value: 'admin' },
        { label: '编辑', value: 'editor' },
        { label: '用户', value: 'user' }
      ]
    },
    {
      key: 'status',
      label: '状态',
      type: 'select',
      options: [
        { label: '全部', value: '' },
        { label: '正常', value: 'active' },
        { label: '禁用', value: 'disabled' }
      ]
    },
    {
      key: 'createdRange',
      label: '创建时间',
      type: 'date-range',
      format: 'yyyy-MM-dd'
    }
  ]"
  layout="horizontal"
  @filter-change="handleFilter"
  @filter-reset="handleReset"
/>
```

## FilterField 类型定义

```typescript
interface FilterField {
  key: string                          // 字段键名
  label: string                        // 字段标签
  type: 'input' | 'select' | 'date-range' | 'number-range'  // 字段类型
  placeholder?: string | string[]    // 占位符文本
  options?: Array<{label: string, value: any}>  // 选项列表（select 类型）
  multiple?: boolean                 // 是否多选（select 类型）
  clearable?: boolean                 // 是否可清空
  format?: string                    // 格式（date-range 类型）
  width?: string                     // 自定义宽度
  span?: number                     // 网格列数（grid 布局）
}
```

## 样式定制

```vue
<FilterBar
  :fields="filterFields"
  class="custom-filter-bar"
/>

<style scoped>
.custom-filter-bar {
  /* 自定义筛选栏样式 */
}
</style>
```

## 使用场景

1. **列表筛选** - 数据列表的筛选条件
2. **搜索筛选** - 搜索结果页面的筛选
3. **表单筛选** - 复杂查询条件的筛选

## 最佳实践

1. 筛选字段不宜过多，建议控制在 4-6 个
2. 常用筛选条件放在前面
3. 提供清晰的占位符和默认值
4. 支持快捷重置筛选条件

## 相关组件

- [ListPage](./list-page.md)
- [PageHeader](./page-header.md)

## 源码位置

`components/admin/patterns/FilterBar.vue`
