# Slug 自动生成规律说明

## 📝 Slug 是什么？

Slug 是文章的 URL 友好标识符，用于生成文章的访问地址，例如：
- 文章标题：`时间胶囊`
- 生成的 Slug：`article-1764297159900`
- 最终 URL：`/blog/article-1764297159900`

## 🔄 自动生成规律

### 1. 基本转换规则

当你在文章编辑页面输入标题时，如果 Slug 字段为空，系统会自动生成：

```javascript
// 生成步骤：
1. 将标题转换为小写
2. 将中文字符、空格、标点符号替换为连字符 `-`
3. 移除连续的连字符（多个 `-` 合并为一个）
4. 移除开头和结尾的连字符
```

### 2. 特殊情况处理

如果转换后的 Slug 为空或只包含连字符，系统会使用时间戳：

```javascript
slug = `article-${Date.now()}`
// 例如：article-1764297159900
```

### 3. 实际示例

| 文章标题 | 生成的 Slug |
|---------|------------|
| `时间胶囊` | `article-1764297159900` (时间戳) |
| `Hello World` | `hello-world` |
| `Vue 3 教程` | `vue-3` (中文被替换) |
| `C# 开发指南` | `c` (特殊字符被移除) |
| `React & TypeScript` | `react-typescript` |

### 4. 手动修改

你可以随时手动修改 Slug，系统不会覆盖你的修改。只有在 Slug 为空时才会自动生成。

## ⚙️ 代码位置

生成逻辑在 `pages/admin/articles/edit/index.vue` 的 `generateSlugFromTitle()` 函数中。

