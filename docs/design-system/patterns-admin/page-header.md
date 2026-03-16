# PageHeader - 页面头部组件

统一的页面头部组件，用于管理后台页面的标题区域。

## 基础用法

```vue
<template>
  <PageHeader
    title="用户管理"
    description="管理系统用户账户、权限和角色"
  >
    <template #actions>
      <n-button type="primary" @click="handleAdd">
        新增用户
      </n-button>
    </template>
  </PageHeader>
</template>

<script setup lang="ts">
import PageHeader from '~/components/admin/patterns/PageHeader.vue'

const handleAdd = () => {
  // 处理新增用户逻辑
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `title` | `string` | - | 页面标题 |
| `description` | `string` | - | 页面描述（可选） |
| `showBack` | `boolean` | `false` | 是否显示返回按钮 |
| `backText` | `string` | `'返回'` | 返回按钮文字 |
| `class` | `string` | - | 自定义样式类 |

## Slots

| 插槽名 | 说明 |
|---------|------|
| `title` | 自定义标题内容 |
| `description` | 自定义描述内容 |
| `actions` | 自定义操作按钮区域 |

## 交互式示例

### 基础用法

```vue
<PageHeader title="用户管理" />
```

### 带描述和操作

```vue
<PageHeader
  title="文章管理"
  description="创建、编辑和发布文章内容"
>
  <template #actions>
    <n-button type="primary" @click="handleCreate">
      新建文章
    </n-button>
    <n-button @click="handleImport">
      导入
    </n-button>
  </template>
</PageHeader>
```

### 带返回按钮

```vue
<PageHeader
  title="用户详情"
  description="查看和编辑用户信息"
  :show-back="true"
  back-text="返回列表"
  @back="handleBack"
/>
```

### 自定义头部内容

```vue
<PageHeader>
  <template #title>
    <div class="flex items-center gap-2">
      <n-icon :component="UserIcon" size="24" />
      <span>用户管理</span>
      <n-tag type="info" size="small">Beta</n-tag>
    </div>
  </template>

  <template #actions>
    <n-button-group>
      <n-button type="primary">
        <template #icon>
          <n-icon :component="PlusIcon" />
        </template>
        新增
      </n-button>
      <n-button>
        <template #icon>
          <n-icon :component="FilterIcon" />
        </template>
        筛选
      </n-button>
    </n-button-group>
  </template>
</PageHeader>
```

## 事件

| 事件 | 说明 |
|------|------|
| `back` | 返回按钮点击时触发 |

## 样式定制

```vue
<PageHeader
  title="自定义样式"
  class="custom-header"
/>

<style scoped>
.custom-header {
  /* 自定义头部样式 */
}
</style>
```

## 使用场景

1. **列表页面** - 显示页面标题和批量操作按钮
2. **表单页面** - 显示表单标题和提交/取消按钮
3. **详情页面** - 显示详情标题和编辑/返回按钮

## 最佳实践

1. 标题应简洁明了，不超过 20 个字
2. 描述可省略，用于复杂页面的补充说明
3. 操作按钮不超过 3 个，主操作放在最前面
4. 使用语义化图标，提高可识别性

## 相关组件

- [FilterBar](./filter-bar.md)
- [ListPage](./list-page.md)
- [FormPage](./form-page.md)

## 源码位置

`components/admin/patterns/PageHeader.vue`
