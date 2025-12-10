# 第四阶段 · 规范固化 & 防回退

## 📋 阶段目标

防止以后新代码再乱用颜色 / 乱写组件样式，固化设计系统规范。

---

## ✅ 已完成

1. ✅ 创建设计系统文档（`docs/DESIGN_SYSTEM_V1.md`）
2. ✅ 创建 UI 编码规范（`docs/CODING_STYLE_UI.md`）
3. ✅ 更新文档索引（在 `docs/README.md` 中添加链接）

---

## 🔄 待完成

### 1. 新增颜色使用守卫

**目标：** 在代码审查和开发过程中自动检测硬编码颜色

**任务：**
- [ ] 创建颜色检查脚本 `scripts/lint-colors.js`
- [ ] 在 `package.json` 中添加 `lint:colors` 命令
- [ ] 配置 pre-commit hook（可选）
- [ ] 在 CI 中添加颜色检查（可选）

**实现方案：**
```javascript
// scripts/lint-colors.js
// 扫描项目中的硬编码颜色值
// 输出警告和建议
```

### 2. Lint / 检查规则

**目标：** 自动化检测违反编码规范的情况

**任务：**
- [ ] 配置 ESLint 规则（如果适用）
- [ ] 创建自定义检查脚本
- [ ] 在开发文档中说明如何运行检查

**检查项：**
- 硬编码颜色（`#fff`, `#000`, `#f3f4f6` 等）
- Naive UI 组件的颜色 class
- Card 组件的颜色/阴影/边框类
- Button 组件缺少 type 属性

### 3. 组件封装

**目标：** 将后台常用的布局/卡片组合封装为组件

**任务：**
- [ ] 创建 `AdminPageCard` 组件
  - 用途：后台页面的标准卡片容器
  - 特性：统一的 header、body、footer 结构
  - 样式：由 themeOverrides.Card 控制

- [ ] 创建 `DashboardSectionCard` 组件
  - 用途：仪表盘区域的卡片
  - 特性：支持标题、操作按钮、内容区
  - 样式：由 themeOverrides.Card 控制

- [ ] 创建 `AdminPageLayout` 组件
  - 用途：后台页面的标准布局
  - 特性：统一的 header、content、footer 结构
  - 样式：由 themeOverrides.Layout 控制

**使用规范：**
```vue
<!-- 以后后台页面统一使用这些封装组件 -->
<AdminPageLayout>
  <template #header>
    <h1>页面标题</h1>
    <n-button type="primary">操作</n-button>
  </template>
  
  <AdminPageCard title="卡片标题">
    卡片内容
  </AdminPageCard>
</AdminPageLayout>
```

### 4. 文档引用

**目标：** 确保开发者能够快速找到设计系统和编码规范

**任务：**
- [x] 在 `docs/README.md` 中添加设计系统链接
- [x] 在 `docs/README.md` 中添加编码规范链接
- [ ] 在主 `README.md` 中添加设计系统说明
- [ ] 在项目根目录创建 `CONTRIBUTING.md`（可选）

---

## 📝 实施步骤

### 步骤 1：创建颜色检查脚本

创建 `scripts/lint-colors.js`：

```javascript
/**
 * 颜色检查脚本
 * 扫描项目中的硬编码颜色值，输出警告
 */

const fs = require('fs')
const path = require('path')
const { execSync } = require('child_process')

// 需要检查的文件类型
const fileExtensions = ['.vue', '.css', '.scss', '.ts', '.js']

// 需要检查的颜色模式
const colorPatterns = [
  /#[0-9a-fA-F]{3,6}\b/g,  // 十六进制颜色
  /rgba?\([^)]+\)/g,       // RGB/RGBA 颜色
]

// 允许的颜色（特殊组件使用）
const allowedColors = [
  '#3b82f6',  // 主色调（图表等）
  '#10b981',  // 成功色（图表等）
  // ... 其他允许的颜色
]

// 需要排除的文件/目录
const excludePatterns = [
  /node_modules/,
  /dist/,
  /\.git/,
  /tokens\.css$/,  // tokens.css 中的颜色是兼容变量
  /useEChartsTheme\.ts$/,  // ECharts 主题文件
]

function scanFile(filePath) {
  const content = fs.readFileSync(filePath, 'utf-8')
  const issues = []
  
  colorPatterns.forEach(pattern => {
    const matches = content.matchAll(pattern)
    for (const match of matches) {
      const color = match[0]
      const lineNumber = content.substring(0, match.index).split('\n').length
      
      // 检查是否是允许的颜色
      if (!allowedColors.includes(color)) {
        issues.push({
          file: filePath,
          line: lineNumber,
          color: color,
          context: content.split('\n')[lineNumber - 1].trim()
        })
      }
    }
  })
  
  return issues
}

function scanDirectory(dir, issues = []) {
  const files = fs.readdirSync(dir)
  
  files.forEach(file => {
    const filePath = path.join(dir, file)
    const stat = fs.statSync(filePath)
    
    // 检查是否需要排除
    if (excludePatterns.some(pattern => pattern.test(filePath))) {
      return
    }
    
    if (stat.isDirectory()) {
      scanDirectory(filePath, issues)
    } else if (fileExtensions.some(ext => file.endsWith(ext))) {
      const fileIssues = scanFile(filePath)
      issues.push(...fileIssues)
    }
  })
  
  return issues
}

// 主函数
function main() {
  const projectRoot = path.resolve(__dirname, '..')
  const issues = scanDirectory(projectRoot)
  
  if (issues.length === 0) {
    console.log('✅ 未发现硬编码颜色问题')
    process.exit(0)
  }
  
  console.log(`\n⚠️  发现 ${issues.length} 个硬编码颜色问题：\n`)
  
  issues.forEach(issue => {
    console.log(`  ${issue.file}:${issue.line}`)
    console.log(`    颜色: ${issue.color}`)
    console.log(`    上下文: ${issue.context}`)
    console.log('')
  })
  
  console.log('💡 建议：')
  console.log('  - 使用 themeOverrides 或 Naive UI 组件')
  console.log('  - 参考 docs/DESIGN_SYSTEM_V1.md')
  console.log('  - 参考 docs/CODING_STYLE_UI.md\n')
  
  process.exit(1)
}

main()
```

### 步骤 2：添加 package.json 脚本

在 `package.json` 中添加：

```json
{
  "scripts": {
    "lint:colors": "node scripts/lint-colors.js"
  }
}
```

### 步骤 3：创建组件封装

创建 `components/admin/AdminPageCard.vue`：

```vue
<template>
  <n-card :class="cardClass">
    <template v-if="title" #header>
      <div class="admin-page-card-header">
        <h3 class="admin-page-card-title">{{ title }}</h3>
        <slot name="header-actions" />
      </div>
    </template>
    
    <slot />
    
    <template v-if="$slots.footer" #footer>
      <slot name="footer" />
    </template>
  </n-card>
</template>

<script setup lang="ts">
interface Props {
  title?: string
  class?: string
}

const props = defineProps<Props>()

const cardClass = computed(() => {
  return props.class || ''
})
</script>

<style scoped>
/* 只保留布局相关的样式 */
.admin-page-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.admin-page-card-title {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
}
</style>
```

### 步骤 4：更新文档索引

在 `docs/README.md` 中添加：

```markdown
## 🎨 设计系统

- [设计系统 v1](./DESIGN_SYSTEM_V1.md) - 完整的设计系统文档
- [UI 编码规范](./CODING_STYLE_UI.md) - UI 开发编码规范
- [颜色统计和迁移指南](./COLOR_STATISTICS.md) - 颜色迁移参考
```

---

## 🎯 验收标准

完成第四阶段后，应该达到：

- [ ] 有完整的设计系统文档
- [ ] 有清晰的编码规范文档
- [ ] 有颜色检查脚本（可选）
- [ ] 有封装的组件供后台使用（可选）
- [ ] 文档索引中包含设计系统链接
- [ ] 新代码遵循设计系统规范

---

## 📌 注意事项

1. **渐进式实施**
   - 不需要一次性完成所有任务
   - 可以先完成文档，再逐步添加工具和组件

2. **保持灵活性**
   - 检查脚本应该是警告而不是错误
   - 允许特殊组件使用自定义颜色（需注释说明）

3. **持续改进**
   - 根据实际使用情况调整规范
   - 定期更新文档和工具

---

**状态：** 进行中  
**最后更新：** 2024-12-XX

