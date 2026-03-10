# 依赖优化计划 - Node_modules 大小优化

## 当前状态分析

### 基本信息
- **Node_modules 总大小**: ~538MB
- **包数量**: 20619 个 JS 文件
- **嵌套 node_modules**: 128 个
- **源映射文件**: 2277 个
- **原生模块文件**: 6 个

### 大型依赖分布
| 依赖包 | 大小 (KB) | 占比 | 用途 | 优化建议 |
|--------|----------|------|------|----------|
| echarts | 59,002 | ~11% | 数据可视化 | ① 按需导入 ② 替代方案 |
| naive-ui | 42,971 | ~8% | UI 组件库 | ① 按需引入 ② 替代轻量级库 |
| three | 37,463 | ~7% | 3D 渲染 | ① 按需模块 ② 替代方案 |
| @bytemd | 5,116 | ~1% | Markdown 编辑器 | ① 按需插件 |

## 问题分析

### 1. 依赖冗余问题
- **嵌套 node_modules过多** (128个): 表明存在大量子包重复安装
- **npm dedupe 检测到**: 175个包可合并，77个包可更新
- **源映射文件过多** (2277个): 开发阶段源码映射占用大量空间

### 2. 大型依赖使用分析
- **echarts**: 仅在管理后台使用11个组件，但导入整个库
- **three.js**: 仅在4个3D组件中使用，但可能包含未使用模块
- **naive-ui**: UI组件库较大，但可能只使用了部分组件
- **ByteMD**: Markdown编辑器功能完整，但可能未充分利用所有插件

### 3. 潜在未使用依赖
- `@emnapi/runtime`: 标记为 extraneous，可能未使用
- `benchmark`: 开发工具，生产环境不需要
- `cross-env`: 跨平台环境变量设置，开发依赖

## 优化方案

### 🎯 第一阶段：立即优化 (预计减少 30-40%)

#### 1. ECharts 优化
**当前**: 使用完整 echarts 库
**优化**:
```javascript
// 替换
import * as echarts from 'echarts'

// 为
import { useEcharts } from '@/composables/useEcharts'
```

**实现步骤**:
- 创建按需导入的 ECharts 组合式函数
- 分析每个图表使用的具体图表类型
- 使用 `echarts.register()` 只注册需要的图表

#### 2. Three.js 优化
**当前**: 导入整个 three 库
**优化**:
```javascript
// 替换
import * as THREE from 'three'

// 为
import { Scene, PerspectiveCamera, WebGLRenderer } from 'three'
```

**实现步骤**:
- 分析 3D 组件实际使用的 Three.js 模块
- 按需导入特定模块
- 考虑使用更轻量的 3D 库（如 three-stdlib 或 p5.js）

#### 3. Naive UI 优化
**当前**: 导入整个 Naive UI
**优化**:
```javascript
// 在 nuxt.config.ts 中
export defineNuxtConfig({
  vite: {
    // 或者在组件中按需导入
  }
})
```

**实现步骤**:
- 使用 `unplugin-vue-components` 自动导入
- 创建 Naive UI 组件白名单
- 考虑替代方案：
  - Element Plus (更轻量)
  - Ant Design Vue (功能完整)
  - 自定义组件（针对少量使用场景）

### 🚀 第二阶段：深度优化 (预计减少 50-60%)

#### 1. 依赖去重
```bash
# 执行依赖去重
npm dedupe

# 清理重复的依赖
npm prune
```

#### 2. 开发/生产依赖分离
```json
{
  "scripts": {
    "build:prod": "NODE_ENV=production npm run build",
    "clean:dev": "rimraf node_modules && npm install --production=false",
    "clean:prod": "rimraf node_modules && npm install --production=true"
  }
}
```

#### 3. 使用更轻量的替代方案

| 原依赖 | 替代方案 | 减少幅度 | 实现难度 |
|--------|----------|----------|----------|
| ECharts | Chart.js / ApexCharts | ~80% | 中等 |
| Three.js | PixiJS / Zdog | ~60% | 中等 |
| Naive UI | Element Plus / 自定义 | ~50% | 低 |
| ByteMD | Custom Editor / MavonEditor | ~70% | 低 |

#### 4. 图片处理优化
- **sharp**: 当前 640KB，已经是较好的选择
- 考虑使用 `sharp` 的 WebAssembly 版本减少原生依赖

### 📊 第三阶段：长期维护

#### 1. 监控工具
```bash
# 安装包大小分析工具
npm install -D bundle-buddy@latest
npm install -D size-limit@latest
```

#### 2. CI/CD 集成
```yaml
# .github/workflows/size-check.yml
name: Bundle Size Check
on: [push, pull_request]
jobs:
  size-check:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - run: npm ci
      - run: npm run build
      - run: npx bundle-buddy
```

#### 3. 预提交钩子
```json
{
  "husky": {
    "hooks": {
      "pre-commit": "npm run lint:bundle-size"
    }
  }
}
```

## 预期效果

| 优化阶段 | 当前大小 | 预期大小 | 减少量 | 减少比例 |
|----------|----------|----------|--------|----------|
| 第一阶段 | 538MB | ~320MB | 218MB | ~40% |
| 第二阶段 | 320MB | ~220MB | 100MB | ~31% |
| 第三阶段 | 220MB | ~200MB | 20MB | ~9% |
| **总计** | | | **338MB** | **~63%** |

## 实施计划

### 第1周：实施第一阶段优化
- [ ] 创建 ECharts 按需导入方案
- [ ] 优化 Three.js 模块导入
- [ ] 配置 Naive UI 按需引入
- [ ] 执行 npm dedupe

### 第2周：实施第二阶段优化
- [ ] 测试替代方案
- [ ] 实现开发/生产环境分离
- [ ] 清理未使用依赖

### 第3周：实施第三阶段优化
- [ ] 设置监控工具
- [ ] 配置 CI/CD 检查
- [ ] 建立维护流程

## 注意事项

1. **渐进式优化**: 每次修改后进行充分测试
2. **向后兼容**: 确保不影响现有功能
3. **性能测试**: 验证优化后的加载性能提升
4. **文档更新**: 及时更新相关文档和示例

## 监控指标

- 构建时间变化
- 首屏加载时间
- Bundle 大小变化
- 内存使用情况
- 用户体验测试反馈