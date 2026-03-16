# ConfigPage - 配置页面组件

配置页面组件，支持实时预览和分组配置。

## 基础用法

```vue
<template>
  <ConfigPage
    title="主题设置"
    description="自定义网站外观和主题"
    :items="configItems"
    :loading="saving"
    @save="handleSave"
  />
</template>

<script setup lang="ts">
import ConfigPage from '~/components/admin/patterns/ConfigPage.vue'

const configItems = [
  {
    key: 'primaryColor',
    label: '主色调',
    type: 'color',
    default: '#2997FF',
    group: '颜色'
  },
  {
    key: 'fontSize',
    label: '基础字号',
    type: 'font-size',
    default: 16,
    group: '排版'
  },
  {
    key: 'enableAnimations',
    label: '启用动画',
    type: 'switch',
    default: true,
    group: '交互'
  }
]

const saving = ref(false)

const handleSave = async (config: Record<string, any>) => {
  saving.value = true
  try {
    await api.post('/settings/theme', config)
    message.success('保存成功')
  } catch (error) {
    message.error('保存失败')
  } finally {
    saving.value = false
  }
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `title` | `string` | - | 页面标题 |
| `description` | `string` | - | 页面描述 |
| `items` | `ConfigItem[]` | - | 配置项列表 |
| `initial-values` | `Record<string, any>` | `{}` | 初始配置值 |
| `loading` | `boolean` | `false` | 保存加载状态 |
| `preview-position` | `'none' \| 'side' \| 'bottom' \| 'modal'` | `'none'` | 预览位置 |
| `collapsible-groups` | `boolean` | `true` | 分组可折叠 |
| `show-reset` | `boolean` | `true` | 是否显示重置按钮 |

## Slots

| 插槽名 | 说明 |
|---------|------|
| `title` | 自定义标题内容 |
| `preview` | 预览内容 |
| `item-{key}` | 自定义配置项内容 |

## 事件

| 事件 | 说明 |
|------|------|
| `save` | 保存配置时触发 `(config: Record<string, any>)` |
| `reset` | 重置配置时触发 |

## 配置项类型

### Color 类型

```typescript
{
  key: 'primaryColor',
  label: '主色调',
  type: 'color',
  default: '#2997FF'
}
```

### Font Family 类型

```typescript
{
  key: 'fontFamily',
  label: '字体',
  type: 'font-family',
  default: 'Inter',
  options: [
    { label: 'Inter', value: 'Inter' },
    { label: 'Georgia', value: 'Georgia' }
  ]
}
```

### Font Size 类型

```typescript
{
  key: 'baseFontSize',
  label: '基础字号',
  type: 'font-size',
  default: 16,
  min: 12,
  max: 24
}
```

### Radio Group 类型

```typescript
{
  key: 'theme',
  label: '主题',
  type: 'radio-group',
  default: 'light',
  options: [
    { label: '浅色', value: 'light' },
    { label: '深色', value: 'dark' }
  ]
}
```

### Checkbox Group 类型

```typescript
{
  key: 'features',
  label: '启用功能',
  type: 'checkbox-group',
  default: ['animations', 'effects'],
  options: [
    { label: '动画效果', value: 'animations' },
    { label: '视觉效果', value: 'effects' },
    { label: '高亮模式', value: 'highlight' }
  ]
}
```

### Switch 类型

```typescript
{
  key: 'enableDarkMode',
  label: '深色模式',
  type: 'switch',
  default: false
}
```

## 交互式示例

### 基础配置

```vue
<ConfigPage
  title="主题设置"
  :items="configItems"
  @save="handleSave"
/>
```

### 带预览

```vue
<ConfigPage
  title="主题设置"
  :items="configItems"
  preview-position="side"
  @save="handleSave"
>
  <template #preview>
    <PreviewComponent :config="previewConfig" />
  </template>
</ConfigPage>
```

### 分组配置

```vue
<ConfigPage
  title="系统设置"
  :items="configItems"
  :collapsible-groups="true"
  @save="handleSave"
  @reset="handleReset"
/>
```

### 自定义配置项

```vue
<ConfigPage
  title="自定义设置"
  :items="configItems"
>
  <template #item-customKey>
    <n-input v-model:value="config.customKey" placeholder="自定义配置" />
  </template>
</ConfigPage>
```

## ConfigItem 类型定义

```typescript
interface ConfigItem {
  key: string                                    // 配置项键名
  label: string                                  // 配置项标签
  type: 'color' | 'font-family' | 'font-size' | 'radio-group' | 'checkbox-group' | 'switch'  // 配置项类型
  default?: any                                 // 默认值
  options?: Array<{label: string, value: any}>  // 选项列表
  min?: number                                  // 最小值（font-size 类型）
  max?: number                                  // 最大值（font-size 类型）
  group?: string                                // 分组名称
  description?: string                            // 配置项说明
  disabled?: boolean                             // 是否禁用
}
```

## 样式定制

```vue
<ConfigPage
  :items="configItems"
  class="custom-config-page"
/>

<style scoped>
.custom-config-page {
  /* 自定义配置页样式 */
}
</style>
```

## 使用场景

1. **主题设置** - 颜色、字体、间距等主题配置
2. **系统设置** - 功能开关、系统参数配置
3. **个性化设置** - 用户偏好设置

## 最佳实践

1. 相关配置项分组展示
2. 提供默认值和说明
3. 支持实时预览效果
4. 提供重置到默认值功能
5. 保存时进行完整验证

## 相关组件

- [PageHeader](./page-header.md)
- [DetailPage](./detail-page.md)
- [FormPage](./form-page.md)

## 源码位置

`components/admin/patterns/ConfigPage.vue`
