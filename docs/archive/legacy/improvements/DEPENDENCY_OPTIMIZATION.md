# 依赖包优化计划

## 📊 当前状态

### node_modules 大小
- **当前大小**: 538MB
- **主要占用**: Three.js (~600KB), ECharts (~800KB), Chart.js (~300KB), Naive UI (~400KB)

## 🎯 优化目标

1. **减少 node_modules 大小**：目标减少 30-40%
2. **提升启动速度**：启动时间减少 40-50%
3. **优化热重载**：HMR 速度提升 50%

## 🔧 优化方案

### 1. 依赖精简

#### 当前分析
项目中存在以下大型依赖包：

| 依赖包 | 大小 | 用途 | 优化建议 |
|--------|------|------|----------|
| three | ~600KB | 3D 场景 | 使用 CDN 按需加载 |
| echarts | ~800KB | 数据可视化 | 使用 ECharts Mini 版本 |
| chart.js | ~300KB | 图表 | 考虑移除（已有 ECharts） |
| naive-ui | ~400KB | UI 组件 | 按需导入 |
| bytemd | ~200KB | Markdown 编辑器 | 只在需要的页面加载 |

#### 优化措施

##### 1.1 使用 CDN 加载大型库
```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  vite: {
    // 外部化大型库，使用 CDN
    build: {
      rollupOptions: {
        external: ['three', 'echarts'],
        output: {
          globals: {
            three: 'THREE',
            echarts: 'echarts'
          }
        }
      }
    }
  }
})

// 在 HTML 中引入 CDN
// <script src="https://cdnjs.cloudflare.com/ajax/libs/three.js/0.181.2/three.min.js"></script>
// <script src="https://cdnjs.cloudflare.com/ajax/libs/echarts/6.0.0/echarts.min.js"></script>
```

##### 1.2 移除重复的图表库
- **问题**: 同时使用 ECharts 和 Chart.js，功能重复
- **建议**: 统一使用 ECharts，移除 Chart.js 和 vue-chartjs
- **预期**: 减少 ~350KB

##### 1.3 Naive UI 按需导入
```typescript
// 从完整导入改为按需导入
import { Button, Card } from 'naive-ui'
// 替代
import naiveUI from 'naive-ui'
```

##### 1.4 ByteMD 按需加载
```typescript
// 只在需要 Markdown 编辑器的页面加载
const ByteMD = defineAsyncComponent(() => import('@bytemd/vue-next').then(m => m.Editor))
```

### 2. Nuxt 性能优化

#### 2.1 模块懒加载
```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  modules: [
    ['@nuxtjs/tailwindcss', {
      // 延迟加载 Tailwind
      viewer: false,
    }]
  ]
})
```

#### 2.2 构建缓存
```json
// package.json
{
  "scripts": {
    "dev": "nuxt dev --port=3000",
    "build:ci": "nuxt build --cache",
    "clean": "rm -rf .nuxt dist"
  }
}
```

### 3. 开发环境优化

#### 3.1 禁用不必要的功能
```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  devtools: {
    enabled: false, // 开发时禁用 Nuxt DevTools
  },
  content: {
    // 禁用开发时的某些功能
    highlight: {
      preloadLanguages: ['javascript', 'typescript', 'vue']
    }
  }
})
```

#### 3.2 优化文件监听
```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  vite: {
    server: {
      watch: {
        // 排除不必要的监听目录
        ignored: ['**/node_modules/**', '**/.git/**', '**/.nuxt/**']
      }
    }
  }
})
```

## 🚀 实施计划

### 第一阶段：快速优化（立即实施）
- [x] 配置代码分割
- [x] 移除 Chart.js（与 ECharts 重复）
- [x] 配置 Naive UI 按需导入
- [x] 优化 nuxt.config.ts 构建配置

### 第二阶段：深度优化（需要测试）
- [ ] 使用 CDN 加载 Three.js 和 ECharts
- [ ] 实现 ByteMD 按需加载
- [ ] 配置构建缓存
- [ ] 优化文件监听

### 第三阶段：长期优化
- [ ] 考虑替换大型依赖包
- [ ] 实现微前端架构
- [ ] 使用 Vite 的预构建优化

## 📈 预期效果

| 指标 | 优化前 | 优化后 | 提升 |
|------|--------|--------|------|
| node_modules 大小 | 538MB | ~350MB | -35% |
| 启动时间 | ~8s | ~4s | -50% |
| HMR 速度 | ~2s | ~1s | -50% |
| 首屏加载 | ~3.5s | ~2s | -43% |

## 🎬 注意事项

1. **测试覆盖**: 每次优化后需要全面测试功能
2. **渐进优化**: 不要一次性修改太多，分步骤进行
3. **回滚准备**: 保留优化前的配置，便于回滚
4. **性能监控**: 使用 PerformanceMonitor 组件持续监控

## 📚 相关文档

- [性能优化总结](./PERFORMANCE_OPTIMIZATION_SUMMARY.md)
- [构建配置](../../nuxt.config.ts)
- [开发规范](../development/DEVELOPMENT_GUIDELINES.md)