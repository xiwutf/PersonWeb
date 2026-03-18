# UI 组件库集成指南

## 📦 推荐的 UI 组件库

### 1. **Naive UI** ⭐ 推荐
- **优势**：
  - 现代化设计，TypeScript 友好
  - 组件丰富，适合后台管理系统
  - 可以按需引入，体积小
  - 样式可定制，支持主题切换
  - 文档完善，社区活跃

- **适用场景**：后台管理系统、数据展示页面

### 2. **Element Plus**
- **优势**：
  - 成熟稳定，企业级应用常用
  - 组件功能强大
  - 文档完善，中文支持好
  - 社区庞大

- **适用场景**：企业级后台管理系统

### 3. **Ant Design Vue**
- **优势**：
  - 功能全面，组件丰富
  - 设计规范统一
  - 适合复杂的企业应用

- **适用场景**：大型企业应用

## 🚀 安装和配置

### 方案一：Naive UI（推荐）

#### 1. 安装
```bash
npm install naive-ui
```

#### 2. 在 `nuxt.config.ts` 中配置
```typescript
export default defineNuxtConfig({
  // ... 其他配置
  build: {
    transpile: ['naive-ui']
  }
})
```

#### 3. 创建插件 `plugins/naive-ui.client.ts`
```typescript
import { setup } from 'naive-ui'

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.use(setup)
})
```

#### 4. 使用示例
```vue
<template>
  <n-data-table
    :columns="columns"
    :data="data"
    :loading="loading"
  />
</template>

<script setup>
import { NDataTable } from 'naive-ui'
</script>
```

### 方案二：Element Plus

#### 1. 安装
```bash
npm install element-plus
```

#### 2. 在 `nuxt.config.ts` 中配置
```typescript
export default defineNuxtConfig({
  css: ['element-plus/dist/index.css']
})
```

#### 3. 创建插件 `plugins/element-plus.client.ts`
```typescript
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.use(ElementPlus)
})
```

## 📝 使用建议

### 可以替换的组件

1. **表格** → `n-data-table` (Naive UI) 或 `el-table` (Element Plus)
2. **按钮** → `n-button` 或 `el-button`
3. **输入框** → `n-input` 或 `el-input`
4. **选择器** → `n-select` 或 `el-select`
5. **对话框** → `n-modal` 或 `el-dialog`
6. **消息提示** → `n-message` 或 `el-message`
7. **分页** → `n-pagination` 或 `el-pagination`
8. **标签** → `n-tag` 或 `el-tag`
9. **卡片** → `n-card` 或 `el-card`
10. **表单** → `n-form` 或 `el-form`

### 保留自定义样式的场景

- 特殊的视觉效果（如玻璃拟态、3D效果）
- 品牌特定的设计元素
- 动画效果
- 响应式布局（使用 Tailwind 的 grid/flex）

## 🎨 样式定制

### Naive UI 主题定制
```typescript
import { create, NButton } from 'naive-ui'

const naive = create({
  components: [NButton],
  theme: {
    common: {
      primaryColor: '#3b82f6'
    }
  }
})
```

### Element Plus 主题定制
使用 `element-plus` 的主题定制工具或 CSS 变量覆盖。

## 📊 对比

| 特性 | Naive UI | Element Plus | Ant Design Vue |
|------|----------|--------------|----------------|
| 体积 | 小 | 中 | 大 |
| TypeScript | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| 组件数量 | 多 | 非常多 | 非常多 |
| 定制性 | 高 | 中 | 中 |
| 学习曲线 | 低 | 低 | 中 |
| 文档 | 好 | 非常好 | 好 |

## 💡 建议

对于本项目，**推荐使用 Naive UI**，因为：
1. 更现代化，设计简洁
2. TypeScript 支持更好
3. 可以按需引入，减少打包体积
4. 适合后台管理系统
5. 与 Tailwind CSS 兼容性好

