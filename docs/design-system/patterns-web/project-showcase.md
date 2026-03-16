# ProjectShowcase - 项目展示组件

项目展示组件，以卡片网格形式展示项目作品。

## 基础用法

```vue
<template>
  <ProjectShowcase
    :projects="projects"
    title="精选项目"
    subtitle="展示我独立开发或参与的核心项目"
    :columns="3"
    @project-click="handleProjectClick"
  />
</template>

<script setup lang="ts">
import ProjectShowcase from '~/components/web/ProjectShowcase.vue'

const projects = [
  {
    id: 1,
    title: 'AI 写作助手',
    description: '基于大语言模型的智能写作辅助工具',
    coverUrl: '/images/ai-writer.jpg',
    tags: ['AI', 'NLP', 'Vue3'],
    createdAt: '2024-01-15',
    viewCount: 1250,
    demoUrl: 'https://demo.example.com',
    githubUrl: 'https://github.com/example/ai-writer'
  },
  {
    id: 2,
    title: '数据可视化平台',
    description: '企业级数据分析和可视化解决方案',
    coverUrl: '/images/data-viz.jpg',
    tags: ['D3.js', 'React', 'TypeScript'],
    createdAt: '2024-02-20',
    viewCount: 980,
    githubUrl: 'https://github.com/example/data-viz'
  }
]

const handleProjectClick = (project: any) => {
  navigateTo(`/projects/${project.id}`)
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `projects` | `Project[]` | - | 项目列表 |
| `title` | `string` | - | 区域标题 |
| `subtitle` | `string` | - | 区域副标题 |
| `columns` | `2 \| 3 \| 4` | `3` | 列数 |
| `viewAllAction` | `ViewAllAction` | - | 查看全部操作 |
| `class` | `string` | - | 自定义样式类 |

## Slots

| 插槽名 | 说明 |
|---------|------|
| `title` | 自定义标题内容 |
| `subtitle` | 自定义副标题内容 |

## Project 类型

```typescript
interface Project {
  id: string              // 项目 ID
  title: string          // 项目标题
  description?: string    // 项目描述
  coverUrl?: string      // 封面图片地址
  tags?: string[]        // 标签列表
  createdAt?: string     // 创建时间
  viewCount?: number     // 浏览次数
  demoUrl?: string      // 在线演示地址
  githubUrl?: string    // GitHub 仓库地址
}
```

## ViewAllAction 类型

```typescript
interface ViewAllAction {
  text: string        // 按钮文字
  onClick: () => void  // 点击回调
}
```

## 事件

| 事件 | 说明 |
|------|------|
| `project-click` | 项目卡片点击时触发 `(project: Project)` |

## 交互式示例

### 基础展示

```vue
<ProjectShowcase
  :projects="projects"
  :columns="3"
/>
```

### 带标题和副标题

```vue
<ProjectShowcase
  title="精选项目"
  subtitle="展示我独立开发或参与的核心项目"
  :projects="projects"
  :columns="3"
/>
```

### 两列布局

```vue
<ProjectShowcase
  :projects="projects"
  :columns="2"
/>
```

### 带查看全部

```vue
<ProjectShowcase
  :projects="projects"
  :view-all-action="{
    text: '查看全部项目',
    onClick: () => navigateTo('/projects')
  }"
/>
```

### 带操作按钮

```vue
<ProjectShowcase
  :projects="[
    {
      id: '1',
      title: '项目名称',
      description: '项目描述',
      tags: ['Vue3', 'TypeScript'],
      demoUrl: 'https://demo.example.com',
      githubUrl: 'https://github.com/example/project'
    }
  ]"
  :columns="3"
  @project-click="handleProjectClick"
/>
```

## 卡片元素

### 封面图片

```vue
<!-- 自动使用 coverUrl -->
<ProjectShowcase :projects="projects" />
```

### 标签显示

```typescript
{
  tags: ['Vue3', 'TypeScript', 'NaiveUI']
}
<!-- 最多显示 3 个标签，超过显示 +N -->
```

### 操作按钮

- **演示链接** - `demoUrl` 存在时显示
- **GitHub 仓库** - `githubUrl` 存在时显示

## 响应式布局

| 断点 | 列数 |
|--------|------|
| 移动端 | 1 列 |
| 平板端 | 2 列 |
| 桌面端 | 指定列数 |

## 样式定制

```vue
<ProjectShowcase
  :projects="projects"
  class="custom-project-showcase"
/>

<style scoped>
.custom-project-showcase {
  /* 自定义项目展示样式 */
}
</style>
```

## 使用场景

1. **项目作品集** - 个人项目作品展示
2. **开源项目** - GitHub 项目展示
3. **产品展示** - 产品功能介绍

## 最佳实践

1. 封面图片统一尺寸，建议 800×400
2. 标题简洁有力，不超过 20 个字
3. 描述清晰表达项目价值
4. 标签突出技术栈
5. 提供 Demo 和 GitHub 链接

## 相关组件

- [FeatureGrid](./feature-grid.md)
- [HeroSection](./hero-section.md)

## 源码位置

`components/web/ProjectShowcase.vue`
