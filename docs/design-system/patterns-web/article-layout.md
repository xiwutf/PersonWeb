# ArticleLayout - 文章布局组件

完整的文章布局组件，包含头部、内容和页脚区域。

## 基础用法

```vue
<template>
  <ArticleLayout
    title="Aurora Design System 实践"
    author="溪午听风"
    date="2024-03-15"
    :tags="['设计系统', 'Vue3', 'NaiveUI']"
    :prev-article="{ slug: 'previous', title: '上一篇：设计令牌系统' }"
    :next-article="{ slug: 'next', title: '下一篇：组件开发规范' }"
    :social-links="socialLinks"
    @share="handleShare"
  >
    <article>
      <h2>引言</h2>
      <p>设计系统是产品一致性的基础...</p>

      <h2>设计令牌</h2>
      <p>设计令牌是设计系统的核心...</p>

      <p>更多内容...</p>
    </article>
  </ArticleLayout>
</template>

<script setup lang="ts">
import ArticleLayout from '~/components/web/ArticleLayout.vue'

const socialLinks = [
  {
    platform: 'twitter',
    name: 'Twitter',
    icon: 'fab fa-twitter',
    url: 'https://twitter.com/intent/tweet?text=...'
  },
  {
    platform: 'wechat',
    name: '微信',
    icon: 'fab fa-weixin',
    url: 'weixin://...'
  },
  {
    platform: 'link',
    name: '复制链接',
    icon: 'fas fa-link'
  }
]

const handleShare = (platform: string) => {
  // 处理分享逻辑
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `title` | `string` | - | 文章标题 |
| `author` | `string` | - | 作者名称 |
| `date` | `string` | - | 发布日期 |
| `tags` | `string[]` | - | 文章标签 |
| `readTime` | `string` | - | 阅读时间 |
| `prevArticle` | `NavArticle` | - | 上一篇导航 |
| `nextArticle` | `NavArticle` | - | 下一篇导航 |
| `showFooter` | `boolean` | `true` | 是否显示页脚 |
| `socialLinks` | `SocialLink[]` | - | 分享链接列表 |
| `class` | `string` | - | 自定义样式类 |

## NavArticle 类型

```typescript
interface NavArticle {
  slug: string      // 文章路径
  title: string    // 文章标题
}
```

## SocialLink 类型

```typescript
interface SocialLink {
  platform: string              // 平台标识
  name: string                // 平台名称
  icon: string                // 图标类名
  url?: string               // 分享 URL
}
```

## 事件

| 事件 | 说明 |
|------|------|
| `share` | 分享时触发 `(platform: string)` |

## Slots

| 插槽名 | 说明 |
|---------|------|
| `default` | 文章主要内容 |

## 交互式示例

### 基础文章

```vue
<ArticleLayout
  title="文章标题"
  :date="article.date"
>
  <article>
    <!-- 文章内容 -->
  </article>
</ArticleLayout>
```

### 带标签和作者

```vue
<ArticleLayout
  title="Aurora Design System"
  author="溪午听风"
  date="2024-03-15"
  :tags="['设计系统', '前端开发']"
>
  <article>
    <!-- 文章内容 -->
  </article>
</ArticleLayout>
```

### 带阅读时间

```vue
<ArticleLayout
  title="长篇文章"
  date="2024-03-15"
  readTime="10 分钟阅读"
  :tags="['深度文章']"
>
  <article>
    <!-- 文章内容 -->
  </article>
</ArticleLayout>
```

### 带导航和分享

```vue
<ArticleLayout
  title="文章标题"
  :prev-article="{ slug: 'prev', title: '上一篇' }"
  :next-article="{ slug: 'next', title: '下一篇' }"
  :social-links="[
    { platform: 'twitter', name: 'Twitter', icon: 'fab fa-twitter' },
    { platform: 'wechat', name: '微信', icon: 'fab fa-weixin' }
  ]"
  @share="handleShare"
>
  <article>
    <!-- 文章内容 -->
  </article>
</ArticleLayout>
```

### 隐藏页脚

```vue
<ArticleLayout
  title="页面标题"
  :show-footer="false"
>
  <article>
    <!-- 不显示导航和分享 -->
  </article>
</ArticleLayout>
```

## 文章内容样式

组件为文章内容提供样式，包括：

```html
<h1> 文章标题（自动使用 props.title） </h1>
<h2> 二级标题 </h2>
<h3> 三级标题 </h3>
<p> 段落文本 </p>
<ul> / <ol> 列表 </ul> / </ol>
<li> 列表项 </li>
<code> 行内代码 </code>
<pre> 代码块 </pre>
```

### 标题样式

```css
h2 {
  font-size: var(--text-2xl);
  font-weight: 600;
  margin: var(--spacing-2xl) 0 var(--spacing-lg);
}

h3 {
  font-size: var(--text-xl);
  font-weight: 600;
  margin: var(--spacing-xl) 0 var(--spacing-md);
}
```

### 代码块样式

```css
code {
  background: var(--color-bg-code);
  border-radius: var(--radius-md);
  padding: var(--spacing-sm);
  font-family: var(--font-mono);
}
```

## 分享按钮

组件内置了社交分享按钮，支持：

| 平台 | 图标 | 说明 |
|--------|------|------|
| Twitter | `fab fa-twitter` | Twitter 分享 |
| WeChat | `fab fa-weixin` | 微信分享 |
| Link | `fas fa-link` | 复制链接 |
| 其他 | 自定义图标 | 自定义分享平台 |

## 导航链接

页脚显示上一篇和下一篇导航：

```vue
<!-- 有上一篇时显示 -->
<a :href="`/blog/${prevArticle.slug}`" class="nav-link prev">
  <i class="fas fa-arrow-left"></i>
  <span>{{ prevArticle.title }}</span>
</a>

<!-- 有下一篇时显示 -->
<a :href="`/blog/${nextArticle.slug}`" class="nav-link next">
  <span>{{ nextArticle.title }}</span>
  <i class="fas fa-arrow-right"></i>
</a>
```

## 样式定制

```vue
<ArticleLayout
  title="自定义文章"
  class="custom-article"
/>

<style scoped>
.custom-article {
  /* 自定义文章样式 */
}

:deep(.custom-article h2) {
  /* 深度选择器自定义标题样式 */
}
</style>
```

## 使用场景

1. **博客文章** - 技术博客和文章展示
2. **文档页面** - 项目文档和教程
3. **公告文章** - 公告和新闻发布

## 最佳实践

1. 标题简洁有力，表达文章核心
2. 标签突出主题，便于分类浏览
3. 使用语义化标签组织内容
4. 代码块使用 `pre > code` 结构
5. 提供导航和分享功能

## 相关组件

- [HeroSection](./hero-section.md)
- [FeatureGrid](./feature-grid.md)

## 源码位置

`components/web/ArticleLayout.vue`
