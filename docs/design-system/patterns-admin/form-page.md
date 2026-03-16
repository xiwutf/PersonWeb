# FormPage - 表单页面组件

完整的表单页面组件，支持分组表单和自定义布局。

## 基础用法

```vue
<template>
  <FormPage
    title="新增用户"
    description="填写用户基本信息"
    :fields="formFields"
    :loading="submitting"
    @submit="handleSubmit"
    @cancel="handleCancel"
  />
</template>

<script setup lang="ts">
import FormPage from '~/components/admin/patterns/FormPage.vue'

const formFields = [
  {
    key: 'name',
    label: '用户名',
    type: 'input',
    required: true,
    rules: { required: true, message: '请输入用户名' }
  },
  {
    key: 'email',
    label: '邮箱',
    type: 'input',
    required: true,
    rules: { required: true, type: 'email', message: '请输入正确的邮箱' }
  },
  {
    key: 'role',
    label: '角色',
    type: 'select',
    required: true,
    options: [
      { label: '管理员', value: 'admin' },
      { label: '用户', value: 'user' }
    ]
  }
]

const submitting = ref(false)

const handleSubmit = async (formData: any) => {
  submitting.value = true
  try {
    await api.post('/users', formData)
    message.success('创建成功')
    navigateTo('/users')
  } catch (error) {
    message.error('创建失败')
  } finally {
    submitting.value = false
  }
}

const handleCancel = () => {
  navigateTo('/users')
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `title` | `string` | - | 表单标题 |
| `description` | `string` | - | 表单描述 |
| `fields` | `FormField[]` | - | 表单字段配置 |
| `initial-values` | `Record<string, any>` | `{}` | 初始值（编辑模式） |
| `loading` | `boolean` | `false` | 提交加载状态 |
| `submit-text` | `string` | `'提交'` | 提交按钮文字 |
| `cancel-text` | `string` | `'取消'` | 取消按钮文字 |
| `show-cancel` | `boolean` | `true` | 是否显示取消按钮 |
| `layout` | `'horizontal' \| 'vertical'` | `'vertical'` | 表单布局 |
| `grouped` | `boolean` | `false` | 是否分组显示 |

## Slots

| 插槽名 | 说明 |
|---------|------|
| `title` | 自定义标题内容 |
| `description` | 自定义描述内容 |
| `form` | 自定义表单内容 |
| `actions` | 自定义操作按钮区域 |

## 事件

| 事件 | 说明 |
|------|------|
| `submit` | 提交表单时触发 `(formData: Record<string, any>)` |
| `cancel` | 取消时触发 |

## 表单字段类型

### Input 类型

```typescript
{
  key: 'name',
  label: '用户名',
  type: 'input',
  required: true,
  placeholder: '输入用户名',
  clearable: true
}
```

### Select 类型

```typescript
{
  key: 'role',
  label: '角色',
  type: 'select',
  required: true,
  options: [
    { label: '管理员', value: 'admin' },
    { label: '用户', value: 'user' }
  ],
  clearable: false
}
```

### Textarea 类型

```typescript
{
  key: 'description',
  label: '描述',
  type: 'textarea',
  placeholder: '输入描述信息',
  rows: 4,
  maxlength: 500
}
```

### Switch 类型

```typescript
{
  key: 'active',
  label: '启用状态',
  type: 'switch',
  default: true
}
```

### Checkbox Group 类型

```typescript
{
  key: 'permissions',
  label: '权限',
  type: 'checkbox-group',
  options: [
    { label: '用户管理', value: 'user:read' },
    { label: '内容管理', value: 'content:read' },
    { label: '系统设置', value: 'system:read' }
  ]
}
```

## 交互式示例

### 基础表单

```vue
<FormPage
  title="新增用户"
  :fields="formFields"
  @submit="handleSubmit"
  @cancel="handleCancel"
/>
```

### 编辑模式

```vue
<FormPage
  title="编辑用户"
  description="修改用户信息"
  :fields="formFields"
  :initial-values="user"
  :loading="submitting"
  @submit="handleSubmit"
  @cancel="handleCancel"
/>
```

### 分组表单

```vue
<FormPage
  title="新增文章"
  :fields="formFields"
  :grouped="true"
  @submit="handleSubmit"
/>
```

### 自定义表单内容

```vue
<FormPage
  title="高级设置"
  description="配置系统参数"
>
  <template #form>
    <n-form ref="formRef" :model="formData">
      <n-form-item label="API Key" path="apiKey">
        <n-input v-model:value="formData.apiKey" type="password" />
      </n-form-item>
      <n-form-item label="启用调试" path="debug">
        <n-switch v-model:value="formData.debug" />
      </n-form-item>
    </n-form>
  </template>
</FormPage>
```

## 表单验证规则

```typescript
const formFields = [
  {
    key: 'email',
    label: '邮箱',
    type: 'input',
    required: true,
    rules: {
      required: true,
      type: 'email',
      message: '请输入正确的邮箱地址'
    }
  },
  {
    key: 'phone',
    label: '手机号',
    type: 'input',
    required: true,
    rules: {
      required: true,
      pattern: /^1[3-9]\d{9}$/,
      message: '请输入正确的手机号'
    }
  }
]
```

## FormField 类型定义

```typescript
interface FormField {
  key: string                                    // 字段键名
  label: string                                  // 字段标签
  type: 'input' | 'select' | 'textarea' | 'switch' | 'checkbox-group' | 'radio-group'  // 字段类型
  required?: boolean                             // 是否必填
  rules?: FormRule                               // 验证规则
  placeholder?: string                            // 占位符文本
  options?: Array<{label: string, value: any}>  // 选项列表
  default?: any                                 // 默认值
  clearable?: boolean                            // 是否可清空
  disabled?: boolean                             // 是否禁用
  rows?: number                                 // 文本域行数
  maxlength?: number                             // 最大长度
  span?: number                                 // 网格列数
  group?: string                                // 分组名称
}
```

## 样式定制

```vue
<FormPage
  :fields="formFields"
  class="custom-form-page"
/>

<style scoped>
.custom-form-page {
  /* 自定义表单样式 */
}
</style>
```

## 使用场景

1. **新增表单** - 创建新记录的表单
2. **编辑表单** - 修改现有记录的表单
3. **设置表单** - 系统设置和配置

## 最佳实践

1. 字段按逻辑分组，提高可读性
2. 必填字段明确标注
3. 提供友好的验证提示
4. 合理设置字段默认值
5. 提交前进行完整验证

## 相关组件

- [PageHeader](./page-header.md)
- [DetailPage](./detail-page.md)
- [ConfigPage](./config-page.md)

## 源码位置

`components/admin/patterns/FormPage.vue`
