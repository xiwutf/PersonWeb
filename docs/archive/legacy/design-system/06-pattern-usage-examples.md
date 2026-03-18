# Admin Pattern Usage Examples

> Admin Page Pattern 使用示例文档
>
> 本文档提供了 Admin Page Pattern 组件的实际使用示例。

---

## Table of Contents

- [ListPage - 列表页](#listpage---列表页)
- [FormPage - 表单页](#formpage---表单页)
- [DetailPage - 详情页](#detailpage---详情页)
- [ConfigPage - 配置页](#configpage---配置页)
- [Best Practices](#best-practices)

---

## ListPage - 列表页

### 基础用法

```vue
<template>
  <ListPage
    title="用户管理"
    description="管理系统用户信息和权限"
    :columns="columns"
    :data="users"
    :loading="loading"
  />
</template>

<script setup lang="ts">
import { h } from 'vue'
import { DataTableColumns } from 'naive-ui'
import ListPage from '~/components/admin/patterns/ListPage.vue'

const users = ref([])
const loading = ref(false)

const columns: DataTableColumns = [
  { title: '姓名', key: 'name' },
  { title: '邮箱', key: 'email' },
  { title: '角色', key: 'role' }
]
</script>
```

### 带搜索和新建按钮

```vue
<template>
  <ListPage
    title="分类管理"
    description="管理全站文章分类及其排序"
    :columns="columns"
    :data="categories"
    :loading="loading"
  >
    <!-- 头部操作按钮区域 -->
    <template #header-actions>
      <n-space :size="12">
        <n-input
          v-model:value="searchQuery"
          placeholder="搜索分类..."
          clearable
          style="width: 240px"
        >
          <template #prefix>
            <i class="fas fa-search"></i>
          </template>
        </n-input>
        <n-button type="primary" @click="handleCreate">
          <template #icon>
            <i class="fas fa-plus"></i>
          </template>
          新建分类
        </n-button>
      </n-space>
    </template>
  </ListPage>
</template>
```

### 带统计卡片

```vue
<template>
  <ListPage
    title="分类管理"
    :show-stats="true"
    :stats="stats"
    :columns="columns"
    :data="categories"
  >
    <!-- 自定义统计卡片 -->
    <template #stats>
      <n-grid :x-gap="16" :y-gap="16" :cols="3">
        <n-gi>
          <n-card :bordered="false" class="stat-card">
            <div class="stat-content">
              <div class="stat-icon" style="color: var(--color-blue-500)">
                <i class="fas fa-folder"></i>
              </div>
              <div class="stat-info">
                <div class="stat-value">{{ categories.length }}</div>
                <div class="stat-label">总分类数</div>
              </div>
            </div>
          </n-card>
        </n-gi>
      </n-grid>
    </template>
  </ListPage>
</template>
```

### 自定义操作列

```vue
<script setup lang="ts">
import { h } from 'vue'
import { NButton, NPopconfirm, NSpace } from 'naive-ui'

const columns = [
  // ... 其他列
  {
    title: '操作',
    key: 'actions',
    width: 150,
    fixed: 'right',
    render(row) {
      return h(NSpace, null, {
        default: () => [
          h(NButton, {
            size: 'small',
            type: 'primary',
            onClick: () => handleEdit(row)
          }, { default: () => '编辑' }),
          h(NPopconfirm, {
            onPositiveClick: () => handleDelete(row),
            negativeText: '取消',
            positiveText: '确认删除'
          }, {
            trigger: () => h(NButton, {
              size: 'small',
              type: 'error'
            }, { default: () => '删除' }),
            default: () => h('div', [
              h('div', { class: 'font-bold mb-1' }, '确认删除？'),
              h('div', { class: 'text-xs text-gray-500' }, `删除后不可恢复`)
            ])
          })
        ]
      })
    }
  }
]
</script>
```

### 使用 FilterBar 筛选

```vue
<template>
  <ListPage
    title="用户管理"
    :filters="filters"
    :filter-value="filterValue"
    :columns="columns"
    :data="filteredUsers"
    @filter-change="handleFilterChange"
    @search="handleSearch"
    @reset="handleReset"
  />
</template>

<script setup lang="ts">
const filters = [
  {
    type: 'input',
    field: 'keyword',
    label: '关键词',
    placeholder: '搜索用户名或邮箱'
  },
  {
    type: 'select',
    field: 'role',
    label: '角色',
    options: [
      { label: '全部', value: '' },
      { label: '管理员', value: 'admin' },
      { label: '普通用户', value: 'user' }
    ]
  },
  {
    type: 'date-range',
    field: 'dateRange',
    label: '注册时间'
  }
]
</script>
```

---

## FormPage - 表单页

### 基础用法

```vue
<template>
  <FormPage
    title="新建用户"
    :fields="fields"
    :model-value="form"
    @submit="handleSubmit"
    @reset="handleReset"
  />
</template>

<script setup lang="ts">
import FormPage from '~/components/admin/patterns/FormPage.vue'
import type { FormItemConfig } from '~/components/admin/patterns/FormPage.vue'

const form = ref({
  name: '',
  email: '',
  role: 'user'
})

const fields: FormItemConfig[] = [
  {
    field: 'name',
    label: '用户名',
    type: 'input',
    required: true
  },
  {
    field: 'email',
    label: '邮箱',
    type: 'input',
    required: true
  },
  {
    field: 'role',
    label: '角色',
    type: 'select',
    options: [
      { label: '管理员', value: 'admin' },
      { label: '普通用户', value: 'user' }
    ]
  }
]

const handleSubmit = (value) => {
  console.log('提交:', value)
}

const handleReset = () => {
  console.log('重置')
}
</script>
```

### 分组表单

```vue
<template>
  <FormPage
    title="用户设置"
    :groups="groups"
    :model-value="form"
    @submit="handleSubmit"
  />
</template>

<script setup lang="ts">
import type { FormGroup } from '~/components/admin/patterns/FormPage.vue'

const groups: FormGroup[] = [
  {
    key: 'basic',
    title: '基本信息',
    fields: ['name', 'email', 'avatar']
  },
  {
    key: 'advanced',
    title: '高级设置',
    fields: ['role', 'permissions', 'notifications']
  }
]
</script>
```

### 自定义表单字段

```vue
<template>
  <FormPage
    title="客服智能体配置"
    :model-value="form"
    @submit="handleSubmit"
  >
    <!-- 自定义表单内容 -->
    <template #form="{ value }">
      <n-form :model="value" label-placement="top">
        <!-- 系统提示词 -->
        <n-form-item label="系统提示词">
          <n-input
            v-model:value="value.systemPrompt"
            type="textarea"
            :rows="6"
            placeholder="输入客服智能体的系统提示词"
          />
        </n-form-item>

        <!-- FAQ 列表 -->
        <n-form-item label="FAQ 列表">
          <div class="faq-list">
            <div v-for="(faq, index) in value.faqList" :key="index" class="faq-item">
              <n-input v-model:value="faq.question" placeholder="问题" />
              <n-input v-model:value="faq.answer" type="textarea" placeholder="答案" />
              <n-button @click="removeFaq(index)" type="error" text>删除</n-button>
            </div>
            <n-button @click="addFaq" dashed>添加 FAQ</n-button>
          </div>
        </n-form-item>
      </n-form>
    </template>
  </FormPage>
</template>
```

---

## DetailPage - 详情页

### 基础用法

```vue
<template>
  <DetailPage
    title="用户详情"
    :data="user"
    :fields="fields"
    :loading="loading"
  />
</template>

<script setup lang="ts">
import DetailPage from '~/components/admin/patterns/DetailPage.vue'
import type { DetailItemConfig } from '~/components/admin/patterns/DetailPage.vue'

const user = ref({
  id: 1,
  name: '张三',
  email: 'zhangsan@example.com',
  role: 'admin',
  createdAt: '2024-01-01T00:00:00Z'
})

const fields: DetailItemConfig[] = [
  { field: 'id', label: 'ID', type: 'text' },
  { field: 'name', label: '用户名', type: 'text' },
  { field: 'email', label: '邮箱', type: 'text' },
  { field: 'role', label: '角色', type: 'tag', tagConfig: { type: 'success' } },
  { field: 'createdAt', label: '注册时间', type: 'datetime' }
]
</script>
```

### 分组详情

```vue
<template>
  <DetailPage
    title="订单详情"
    :groups="groups"
    :data="order"
  />
</template>

<script setup lang="ts">
import type { DetailGroup } from '~/components/admin/patterns/DetailPage.vue'

const groups: DetailGroup[] = [
  {
    key: 'basic',
    title: '基本信息',
    fields: ['id', 'status', 'amount', 'createdAt']
  },
  {
    key: 'shipping',
    title: '收货信息',
    fields: ['name', 'phone', 'address']
  }
]
</script>
```

### 自定义字段渲染

```vue
<template>
  <DetailPage
    title="用户详情"
    :data="user"
    :fields="fields"
  >
    <!-- 自定义字段渲染 -->
    <template #field-avatar="{ value }">
      <n-image
        :src="value"
        :width="80"
        :height="80"
        object-fit="cover"
        style="border-radius: 50%"
      />
    </template>
  </DetailPage>
</template>

<script setup lang="ts">
const fields = [
  { field: 'avatar', label: '头像' },
  { field: 'name', label: '用户名' },
  { field: 'email', label: '邮箱' }
]
</script>
```

---

## ConfigPage - 配置页

### 基础用法

```vue
<template>
  <ConfigPage
    title="系统设置"
    :groups="groups"
    :model-value="config"
    :saving="saving"
    @save="handleSave"
  />
</template>

<script setup lang="ts">
import type { ConfigGroup } from '~/components/admin/patterns/ConfigPage.vue'

const config = ref({
  siteName: '我的网站',
  siteDescription: '',
  allowRegistration: true
})

const saving = ref(false)

const groups: ConfigGroup[] = [
  {
    key: 'basic',
    title: '基本设置',
    items: [
      {
        key: 'siteName',
        label: '网站名称',
        type: 'input',
        placeholder: '请输入网站名称'
      },
      {
        key: 'siteDescription',
        label: '网站描述',
        type: 'textarea',
        rows: 3,
        placeholder: '请输入网站描述'
      }
    ]
  },
  {
    key: 'registration',
    title: '注册设置',
    items: [
      {
        key: 'allowRegistration',
        label: '允许注册',
        type: 'switch'
      }
    ]
  }
]
</script>
```

### 带预览的配置页

```vue
<template>
  <ConfigPage
    title="字体设置"
    :show-preview="true"
    :preview-position="side"
    :model-value="fontConfig"
    @save="handleSave"
  >
    <!-- 预览区域 -->
    <template #preview="{ value }">
      <div class="font-preview" :style="{ fontFamily: value.fontFamily, fontSize: `${value.fontSize}px` }">
        <h1>标题示例</h1>
        <p>这是正文内容，用于预览字体效果。</p>
      </div>
    </template>
  </ConfigPage>
</template>

<style scoped>
.font-preview {
  padding: var(--spacing-xl);
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
}
</style>
```

---

## Best Practices

### 1. 使用插槽保持灵活性

优先使用插槽（slot）来自定义组件内容，而不是完全覆盖。

**推荐：**
```vue
<ListPage>
  <template #header-actions>
    <!-- 自定义头部操作按钮 -->
  </template>
</ListPage>
```

**不推荐：**
```vue
<!-- 完全自定义，失去了 Pattern 的统一性 -->
<div class="custom-page">
  <!-- ... -->
</div>
```

### 2. 使用设计变量

所有自定义样式应使用设计变量，确保主题适配。

```css
.custom-style {
  /* 推荐：使用设计变量 */
  color: var(--color-text-main);
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);

  /* 不推荐：硬编码颜色 */
  /* color: #ffffff;
  background: rgba(255, 255, 255, 0.03); */
}
```

### 3. 响应式设计

确保页面在不同设备上都能正常使用。

```css
/* 响应式 */
@media (max-width: 768px) {
  .actions {
    flex-direction: column;
  }
}
```

### 4. 类型安全

使用 TypeScript 类型确保类型安全。

```typescript
import type { FormItemConfig } from '~/components/admin/patterns/FormPage.vue'

const fields: FormItemConfig[] = [
  {
    field: 'name',
    label: '用户名',
    type: 'input',
    required: true
  }
]
```

### 5. 错误处理

确保错误处理和用户反馈。

```typescript
const handleSubmit = async () => {
  try {
    await api.post('/users', form.value)
    message.success('创建成功')
  } catch (e) {
    handleError(e, '创建失败')
  }
}
```

### 6. 加载状态

使用 loading 状态提升用户体验。

```vue
<ListPage
  :loading="loading"
  :data="users"
/>
```

---

## Common Patterns

### 列表页 + 编辑弹窗

```vue
<template>
  <ListPage @row-click="handleRowClick">
    <!-- ... -->
  </ListPage>

  <n-modal v-model="showModal">
    <n-form :model="editForm">
      <!-- 编辑表单 -->
    </n-form>
  </n-modal>
</template>

<script setup lang="ts">
const showModal = ref(false)
const editForm = ref({})

const handleRowClick = (row) => {
  editForm.value = { ...row }
  showModal.value = true
}
</script>
```

### 表单页 + 实时预览

```vue
<template>
  <FormPage
    :model-value="form"
    :show-preview="true"
  >
    <template #preview="{ value }">
      <div class="preview" v-html="generatePreview(value)"></div>
    </template>
  </FormPage>
</template>
```

---

## Troubleshooting

### 问题：表格列自定义渲染不显示

确保使用 `h()` 函数创建 VNode。

```typescript
import { h } from 'vue'

const columns = [
  {
    title: '名称',
    key: 'name',
    render(row) {
      return h('div', {}, row.name)  // 使用 h()
    }
  }
]
```

### 问题：插槽内容不显示

确保插槽名称正确。

```vue
<ListPage>
  <template #header-actions>  <!-- 正确：使用 #header-actions -->
    <!-- ... -->
  </template>
</ListPage>
```

---

*文档生成时间: 2026-03-16*
