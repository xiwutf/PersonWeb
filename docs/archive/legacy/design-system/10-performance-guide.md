# 性能优化指南

本文档提供了 Aurora Design System 的完整性能优化指南，包括最佳实践、工具使用和监控方法。

---

## 目录

1. [性能目标](#性能目标)
2. [Web Vitals](#web-vitals)
3. [性能监控](#性能监控)
4. [性能测试](#性能测试)
5. [优化技巧](#优化技巧)
6. [回归检测](#回归检测)
7. [故障排除](#故障排除)

---

## 性能目标

### Core Web Vitals 目标

| 指标 | 目标 | 良好 | 需要改进 |
|------|------|------|----------|
| **FCP** | < 1.8s | < 1.8s | < 3.0s |
| **LCP** | < 2.5s | < 2.5s | < 4.0s |
| **FID** | < 100ms | < 100ms | < 300ms |
| **CLS** | < 0.1 | < 0.1 | < 0.25 |

### 资源大小目标

| 资源类型 | 目标 |
|----------|------|
| JS 总大小 | < 400 KB |
| CSS 总大小 | < 100 KB |
| 单个图片 | < 100 KB |
| 首屏资源 | < 300 KB |

---

## Web Vitals

### 什么是 Web Vitals

Web Vitals 是 Google 提出的一组核心性能指标，用于衡量用户体验质量。

### 核心指标

#### FCP (First Contentful Paint) - 首次内容绘制

首次将任何内容（文本、图像、背景颜色等）绘制到屏幕上的时间。

**优化建议：**
- 减少 JavaScript 阻塞时间
- 优化 CSS 加载
- 延迟加载非关键资源

#### LCP (Largest Contentful Paint) - 最大内容绘制

视口内最大的内容元素（通常是图片或文本块）完全可见的时间。

**优化建议：**
- 优化图片加载（WebP、懒加载）
- 预加载关键资源
- 减少 JavaScript 执行时间

#### FID (First Input Delay) - 首次输入延迟

用户首次与页面交互（点击、点击、按键）到浏览器能够响应的时间。

**优化建议：**
- 减少主线程阻塞
- 拆分长任务
- 使用 Web Workers

#### CLS (Cumulative Layout Shift) - 累积布局偏移

页面加载过程中元素意外移动的总和。

**优化建议：**
- 为图片和视频预留空间
- 避免在现有内容上方插入内容
- 使用 transform 动画

---

## 性能监控

### 使用 Web Vitals Hook

```vue
<script setup lang="ts">
import { useWebVitals } from '~/composables/useWebVitals'

const {
  metrics,        // 性能指标
  isReady,        // 是否准备就绪
  calculateScore, // 计算评分
  reportMetrics,  // 上报指标
  getStats        // 获取统计
} = useWebVitals()

// 获取性能评分
const score = calculateScore()
console.log(`性能评分: ${score.overall} (${score.grade})`)
</script>
```

### 集成到应用

在 `plugins/web-vitals.client.ts` 中添加：

```typescript
import { useWebVitals } from '~/composables/useWebVitals'

export default defineNuxtPlugin(() => {
  const { reportMetrics } = useWebVitals()

  // 页面加载完成后上报
  window.addEventListener('load', () => {
    setTimeout(() => {
      reportMetrics('https://your-analytics-endpoint.com/performance')
    }, 5000)
  })
})
```

### 查看性能统计

```vue
<script setup lang="ts">
import { useWebVitals } from '~/composables/useWebVitals'

const { getStats } = useWebVitals()

const stats = getStats()
if (stats) {
  console.log('FCP P75:', stats.fcp?.p75)
  console.log('LCP P95:', stats.lcp?.p95)
  console.log('总样本数:', stats.totalSamples)
}
</script>
```

---

## 性能测试

### 本地测试

#### Lighthouse 测试

```bash
# 启动开发服务器
npm run dev

# 在另一个终端运行 Lighthouse
npm run perf:lighthouse
```

#### 性能预算检查

```bash
# 构建应用
npm run build

# 检查性能预算
npm run check:budget
```

#### 性能分析

```bash
# 运行性能分析工具
npm run perf:analyze
```

### CI/CD 测试

#### Lighthouse CI

Lighthouse CI 会自动在每次 PR 和推送到 master 时运行：

```yaml
# .github/workflows/lighthouse-ci.yml
- name: Run Lighthouse CI
  run: lhci autorun --collect.url=http://localhost:3000/
```

#### 回归测试

```bash
# 运行回归测试
npm run perf:regression:test

# 更新基线（当预期变化时）
npm run perf:baseline:update
```

---

## 优化技巧

### 图片优化

#### 使用 LazyImage 组件

```vue
<template>
  <LazyImage
    src="/images/photo.jpg"
    alt="示例图片"
    :lazy="true"
    format="webp"
    quality="85"
    :aspect-ratio="16/9"
    placeholder="/images/thumb.jpg"
  />
</template>
```

#### 图片格式选择

| 格式 | 优势 | 适用场景 |
|------|------|----------|
| **WebP** | 文件小 25-35% | 照片、复杂图像 |
| **AVIF** | 文件小 30-50% | 现代浏览器 |
| **JPEG** | 兼容性好 | 降级方案 |
| **PNG** | 透明度支持 | 图标、简单图形 |

### 代码分割

#### 路由级分割

```typescript
// pages/admin/dashboard.vue
import { defineAsyncComponent } from 'vue'

const HeavyChart = defineAsyncComponent(() =>
  import('~/components/HeavyChart.vue')
)
```

#### 动态导入

```typescript
// 按需加载 ECharts
const loadChart = async () => {
  const echarts = await import('echarts')
  const myChart = echarts.init(document.getElementById('chart'))
  // ...
}
```

### 缓存策略

#### Service Worker 缓存

```typescript
// service-worker.ts
self.addEventListener('fetch', (event) => {
  // 缓存静态资源
  if (event.request.url.endsWith('.js') ||
      event.request.url.endsWith('.css')) {
    event.respondWith(
      caches.match(event.request).then((response) => {
        return response || fetch(event.request)
      })
    )
  }
})
```

#### HTTP 缓存头

```nginx
# Nginx 配置
location ~* \.(js|css|png|jpg|jpeg|gif|webp|svg)$ {
  expires 1y;
  add_header Cache-Control "public, immutable";
}
```

### 减少阻塞

#### 使用 defer 和 async

```html
<!-- 非 critical 脚本使用 async -->
<script src="/analytics.js" async></script>

<!-- 依赖其他脚本的脚本使用 defer -->
<script src="/main.js" defer></script>
```

#### 延迟加载非关键资源

```vue
<script setup lang="ts">
import { onMounted } from 'vue'

onMounted(() => {
  // 延迟加载评论组件
  import('~/components/Comments.vue').then(({ default: Comments }) => {
    // 挂载组件
  })
})
</script>
```

---

## 回归检测

### 设置基线

```bash
# 首次运行创建基线
npm run build
npm run perf:regression:test
```

### 检测回归

```bash
# 每次代码变更后运行
npm run build
npm run perf:regression:test
```

### 回归阈值

| 指标 | 轻微 | 中等 | 严重 |
|------|------|------|------|
| FCP | +5% | +15% | +30% |
| LCP | +5% | +15% | +30% |
| JS 大小 | +5KB | +15KB | +50KB |
| CSS 大小 | +5KB | +10KB | +20KB |

---

## 故障排除

### 常见问题

#### LCP 过高

**可能原因：**
- 大图片未优化
- 图片懒加载延迟
- 字体加载阻塞

**解决方案：**
1. 使用 WebP 格式
2. 预加载关键资源
3. 使用 font-display: swap

#### FID 过高

**可能原因：**
- 长任务阻塞主线程
- 大量 JavaScript 执行
- 复杂的初始渲染

**解决方案：**
1. 使用 requestIdleCallback
2. 拆分长任务
3. 使用 Web Workers

#### CLS 过高

**可能原因：**
- 图片未预留空间
- 动态内容插入
- 字体加载引起布局变化

**解决方案：**
1. 为图片设置宽高或 aspect-ratio
2. 预留动态内容空间
3. 使用字体加载 API

#### Bundle 过大

**可能原因：**
- 未启用 Tree Shaking
- 引入整个库而非按需
- 重复依赖

**解决方案：**
1. 配置 Tree Shaking
2. 使用按需引入
3. 检查依赖重复

---

## 工具参考

### 开发工具

```bash
# Bundle 分析
npm run bundle:analyze

# 性能预算检查
npm run check:budget

# 性能分析
npm run perf:analyze

# 回归测试
npm run perf:regression:test
```

### 组件

- `useWebVitals` - Web Vitals 监控
- `usePerformance` - 性能指标收集
- `usePerformanceComparison` - 性能对比
- `PerformanceDashboard` - 性能监控面板
- `BundleAnalyzer` - Bundle 分析可视化

---

## 最佳实践总结

1. **监控优先** - 持续监控性能指标
2. **预防回归** - 使用自动化测试
3. **渐进优化** - 从高频页面开始
4. **用户视角** - 关注真实用户体验
5. **持续改进** - 定期审查和优化
