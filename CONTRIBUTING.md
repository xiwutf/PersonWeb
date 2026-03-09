# 贡献指南

感谢你对本项目的兴趣和支持！这份文档将指导你如何贡献代码、报告问题和提出建议。

## 目录

- [行为准则](#行为准则)
- [我想如何贡献？](#我想如何贡献)
- [开发流程](#开发流程)
- [编码规范](#编码规范)
- [提交信息规范](#提交信息规范)
- [测试](#测试)
- [文档](#文档)
- [常见问题](#常见问题)

## 行为准则

请遵守 [Code of Conduct](./CODE_OF_CONDUCT.md)，简要总结为：

- 使用友好、包容的语言
- 尊重不同的观点和经验
- 接受建设性的批评
- 关注什么对社区最有利
- 对其他社区成员表示同情

## 我想如何贡献？

### 报告 Bug 🐛

Before creating bug reports, please check the issue list as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible:

**使用标题和清晰的描述**
- 使用描述性的标题
- 提供具体的示例来展示步骤
- **描述实际行为**和**期望行为**
- **包含截图和错误日志**（如果适用）
- **您的环境信息**：操作系统、Node.js 版本、浏览器等

### 提出功能建议 💡

功能建议包括完全新功能和对现有功能的改进。

- **使用清晰的标题和描述**来描述建议
- **提供具体的用例**来展示该建议如何有用
- **列出一些相似的产品或库**（如果存在）

### Pull Request

- 按照下面的步骤操作
- 遵循 [编码规范](#编码规范)
- 更新相关文档
- 确保所有测试通过

## 开发流程

### 1. Fork 和 Clone

```bash
# Fork 本仓库到你的账户
# 然后 clone 到本地
git clone https://github.com/your-username/personal-site.git
cd personal-site

# 添加上游仓库
git remote add upstream https://github.com/original-owner/personal-site.git
```

### 2. 创建分支

```bash
# 更新主分支
git fetch upstream
git checkout main
git rebase upstream/main

# 创建新分支
git checkout -b feature/your-feature-name
# 或 bug fix
git checkout -b fix/bug-name
```

分支名规范：
- `feature/`: 新功能
- `fix/`: 错误修复
- `docs/`: 文档更新
- `style/`: 代码风格修改
- `refactor/`: 代码重构
- `perf/`: 性能优化
- `test/`: 测试相关

### 3. 提交更改

```bash
# 编辑文件后
git add .
git commit -m "feat: add new feature"
```

### 4. 推送到分支

```bash
git push origin feature/your-feature-name
```

### 5. 创建 Pull Request

在 GitHub 上创建 Pull Request，请包含：

- [ ] 清晰的 PR 标题和描述
- [ ] 链接到相关的 Issue
- [ ] 更新的文档和示例
- [ ] 新增或更新的测试
- [ ] 更新的 CHANGELOG
- [ ] 自检清单

## 编码规范

### JavaScript/TypeScript

```typescript
// ✅ 好的例子
const calculateTotal = (items: Item[]): number => {
  return items.reduce((sum, item) => sum + item.price, 0);
};

// ❌ 不好的例子
function calculateTotal(items) {
  let total = 0;
  for (let i = 0; i < items.length; i++) {
    total += items[i].price;
  }
  return total;
}
```

**规范**：

- 使用 TypeScript，避免使用 `any`
- 使用 const/let，避免 var
- 使用箭头函数
- 使用模板字符串而不是字符串连接
- 添加 JSDoc 注释到公共函数
- 一行不超过 100 字符

### 代码检查

在提交前运行：

```bash
# 运行 ESLint
npm run lint

# 修复 lint 错误
npm run lint:fix

# 格式化代码
npm run format
```

### Vue 组件

```vue
<template>
  <div class="component">
    <h1>{{ title }}</h1>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  title: string;
}

const props = defineProps<Props>();

const displayTitle = computed(() => props.title.toUpperCase());
</script>

<style scoped>
.component {
  padding: 1rem;
}
</style>
```

**规范**：

- 使用 `<script setup>` 语法
- 为 Props 和 Emits 添加类型
- 使用 scoped styles
- 命名遵循 PascalCase（组件）
- 事件名使用 kebab-case

### CSS/Tailwind

```css
/* ✅ 好的例子 */
@apply flex items-center justify-between p-4 rounded-lg;

/* ❌ 不好的例子 */
.container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem;
  border-radius: 0.5rem;
}
```

**规范**：

- 优先使用 Tailwind 类
- 避免过度使用自定义 CSS
- 遵循移动优先的响应式设计

## 提交信息规范

遵循 [Conventional Commits](https://www.conventionalcommits.org/) 规范：

```
<type>(<scope>): <subject>

<body>

<footer>
```

### 类型 (type)

- `feat`: 新功能
- `fix`: Bug 修复
- `docs`: 文档
- `style`: 代码风格（不影响功能）
- `refactor`: 重构
- `perf`: 性能优化
- `test`: 测试
- `chore`: 构建、依赖等
- `ci`: CI/CD 相关

### 示例

```
feat(components): add new dashboard widget

- Implement dashboard statistics widget
- Add data visualization with ECharts
- Support dark/light theme switching

Closes #123
```

## 测试

```bash
# 运行所有测试
npm run test

# 运行特定文件的测试
npm run test -- path/to/file.test.ts

# 生成覆盖率报告
npm run test:coverage
```

## 文档

- 更新 README.md 中的相关部分
- 如果添加新功能，请在 docs/ 中添加文档
- 注释复杂的逻辑
- 使用清晰的变量和函数名

## 常见问题

### Q: 如何同步上游仓库的最新代码？

```bash
git fetch upstream
git rebase upstream/main
git push origin main --force-with-lease
```

### Q: 如何清除本地分支？

```bash
# 删除本地分支
git branch -d branch-name

# 强制删除
git branch -D branch-name

# 删除远程分支
git push origin --delete branch-name
```

### Q: PR 有冲突怎么办？

```bash
# 更新分支到最新的主分支
git fetch origin
git rebase origin/main

# 解决冲突后
git add .
git rebase --continue
git push origin branch-name --force-with-lease
```

### Q: 如何预览构建结果？

```bash
npm run build
npm run preview
```

## 许可证

通过贡献，你同意你的贡献将在 MIT License 下许可。

---

感谢你的贡献！🎉
