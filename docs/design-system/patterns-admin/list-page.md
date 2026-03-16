# ListPage - 列表页面组件

完整的列表页面组件，整合了页面头部、筛选栏、统计卡片和表格。

## 基础用法

```vue
<template>
  <ListPage
    title="用户管理"
    :columns="tableColumns"
    :data="users"
    :loading="loading"
    :pagination="pagination"
    @page-change="handlePageChange"
    @row-click="handleRowClick"
  >
    <template #header-actions>
      <n-button type="primary" @click="handleAdd">
        新增用户
      </n-button>
    </template>
  </ListPage>
</template>

<script setup lang="ts">
import ListPage from '~/components/admin/patterns/ListPage.vue'

const tableColumns = [
  { title: '姓名', key: 'name' },
  { title: '邮箱', key: 'email' },
  { title: '角色', key: 'role' },
  { title: '状态', key: 'status' },
  { title: '操作', key: 'actions' }
]

const users = ref([
  { id: 1, name: '张三', email: 'zhang@example.com', role: '管理员', status: '正常' }
])

const loading = ref(false)
const pagination = ref({
  page: 1,
  pageSize: 20,
  total: 100
})

const handlePageChange = (page: number) => {
  pagination.value.page = page
  fetchUsers()
}

const handleRowClick = (row: any) => {
  navigateTo(`/users/${row.id}`)
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `title` | `string` | - | 页面标题 |
| `description` | `string` | - | 页面描述 |
| `columns` | `DataTableColumn[]` | - | 表格列配置 |
| `data` | `any[]` | `[]` | 表格数据 |
| `loading` | `boolean` | `false` | 加载状态 |
| `pagination` | `PaginationProps \| false` | - | 分页配置，false 表示不显示分页 |
| `show-filter` | `boolean` | `false` | 是否显示筛选栏 |
| `filter-fields` | `FilterField[]` | - | 筛选字段配置 |
| `filter-layout` | `'horizontal' \| 'vertical'` | `'horizontal'` | 筛选栏布局 |

## Slots

| 插槽名 | 说明 |
|---------|------|
| `title` | 自定义标题内容 |
| `description` | 自定义描述内容 |
| `header-actions` | 头部操作按钮区域 |
| `filter` | 自定义筛选栏内容 |
| `stats` | 统计卡片区域 |
| `table-actions` | 表格顶部操作区域 |
| `empty` | 空状态内容 |

## 事件

| 事件 | 说明 |
|------|------|
| `page-change` | 页码变化时触发 `(page: number)` |
| `page-size-change` | 每页数量变化时触发 `(pageSize: number)` |
| `row-click` | 行点击时触发 `(row: any)` |

## 交互式示例

### 基础列表

```vue
<ListPage
  title="用户列表"
  :columns="columns"
  :data="users"
  :loading="loading"
  :pagination="pagination"
  @page-change="handlePageChange"
/>
```

### 带统计卡片

```vue
<ListPage
  title="文章管理"
  :columns="columns"
  :data="articles"
  :pagination="pagination"
>
  <template #stats>
    <n-statistic label="总文章数" :value="articleStats.total" />
    <n-statistic label="已发布" :value="articleStats.published" />
    <n-statistic label="草稿" :value="articleStats.draft" />
  </template>
</ListPage>
```

### 带筛选栏

```vue
<ListPage
  title="用户管理"
  :columns="columns"
  :data="users"
  :show-filter="true"
  :filter-fields="filterFields"
  @filter-change="handleFilter"
  @page-change="handlePageChange"
/>
```

### 自定义表格行

```vue
<ListPage
  title="项目管理"
  :columns="columns"
  :data="projects"
  :pagination="pagination"
>
  <template #table-columns="{ column }">
    <template v-if="column.key === 'name'">
      {{ row.name }}
      <n-tag v-if="row.featured" type="warning" size="small">推荐</n-tag>
    </template>
  </template>
</ListPage>
```

## 统计卡片配置

```vue
<template #stats>
  <div class="stats-grid">
    <StatCard
      title="总用户数"
      :value="stats.total"
      icon="fa-users"
      trend="up"
      :trend-value="12"
    />
    <StatCard
      title="活跃用户"
      :value="stats.active"
      icon="fa-user-check"
      trend="up"
      :trend-value="8"
    />
    <StatCard
      title="本月新增"
      :value="stats.newUsers"
      icon="fa-user-plus"
      trend="down"
      :trend-value="3"
    />
  </div>
</template>
```

## 分页配置

```typescript
const pagination = {
  page: 1,        // 当前页码
  pageSize: 20,    // 每页数量
  total: 100,       // 总条数
  showSizePicker: true, // 显示每页数量选择器
  pageSizes: [10, 20, 50, 100]  // 可选每页数量
}
```

## 筛选字段配置

```typescript
const filterFields = [
  {
    key: 'name',
    label: '用户名',
    type: 'input',
    placeholder: '输入用户名'
  },
  {
    key: 'role',
    label: '角色',
    type: 'select',
    options: [
      { label: '全部', value: '' },
      { label: '管理员', value: 'admin' },
      { label: '普通用户', value: 'user' }
    ]
  },
  {
    key: 'dateRange',
    label: '创建时间',
    type: 'date-range'
  }
]
```

## 样式定制

```vue
<ListPage
  title="自定义列表"
  class="custom-list-page"
/>

<style scoped>
.custom-list-page {
  /* 自定义列表页样式 */
}

:deep(.custom-list-page .n-data-table) {
  /* 深度选择器自定义表格样式 */
}
</style>
```

## 使用场景

1. **数据列表** - 用户、文章、项目等数据展示
2. **表格管理** - 带筛选、排序、分页的数据管理
3. **统计列表** - 带统计指标的数据列表

## 最佳实践

1. 列数不宜过多，建议控制在 6-8 列
2. 主操作放在第一列，次要操作放在最后
3. 加载状态显示骨架屏或加载动画
4. 空状态提供友好的提示和操作引导

## 相关组件

- [PageHeader](./page-header.md)
- [FilterBar](./filter-bar.md)
- [DetailPage](./detail-page.md)

## 源码位置

`components/admin/patterns/ListPage.vue`
