# 导入所有 Markdown 文件到数据库

## 📋 功能说明

这个脚本可以将 `content/` 目录下的所有 Markdown 文件导入到数据库，并删除原文件。

### 支持的文件类型

| 目录 | 目标表 | 分类 |
|------|--------|------|
| `content/blog/*.md` | `article` | 技术文章 |
| `content/life/*.md` | `article` | 生活随笔 |
| `content/projects/*.md` | `Projects` | - |
| `content/ai/*.md` | `knowledge_base` | 想法灵感 |
| `content/tools/*.md` | `knowledge_base` | 工具推荐 |
| 其他目录 | `knowledge_base` | 开发笔记 |

## 🚀 使用方法

### 步骤 1: 获取管理员 Token

1. 打开管理后台：`http://localhost:3000/admin/login`（或生产环境地址）
2. 登录后，打开浏览器开发者工具（F12）
3. 在 Console 中运行：
   ```javascript
   localStorage.getItem('admin_token')
   ```
4. 复制返回的 token 值

### 步骤 2: 运行导入脚本

**Windows (PowerShell):**
```powershell
$env:ADMIN_TOKEN="your_token_here"
node scripts/import-all-content-to-db.js
```

**Linux/macOS:**
```bash
export ADMIN_TOKEN="your_token_here"
node scripts/import-all-content-to-db.js
```

**使用生产环境 API:**
```bash
export ADMIN_TOKEN="your_token_here"
export API_BASE="https://api.xifg.com.cn/api"
node scripts/import-all-content-to-db.js
```

**不删除原文件（仅导入）:**
```bash
export ADMIN_TOKEN="your_token_here"
export DELETE_FILES="false"
node scripts/import-all-content-to-db.js
```

**删除已存在的数据后重新导入:**
```bash
export ADMIN_TOKEN="your_token_here"
export DELETE_EXISTING="true"
node scripts/import-all-content-to-db.js
```

## 📝 文件格式要求

### 文章格式 (blog, life)

```markdown
---
title: "文章标题"
date: "2024-01-15"
tags: ["标签1", "标签2"]
description: "文章摘要"
category: "技术文章"
cover: "/images/cover.jpg"
---

# 文章内容

这里是 Markdown 内容...
```

### 项目格式 (projects)

```markdown
---
title: 项目名称
tech: [技术1, 技术2]
description: 项目描述
demo_link: https://demo.com
source_link: https://github.com/user/repo
cover: /images/project.png
status: 开发中
---

# 项目内容

这里是 Markdown 内容...
```

### 知识库格式 (ai, tools)

```markdown
---
title: 知识标题
description: 描述
category: 开发笔记
tags: ["标签1", "标签2"]
---

# 知识内容

这里是 Markdown 内容...
```

## ⚠️ 注意事项

1. **备份数据**：执行前建议备份数据库和 content 目录
2. **Token 有效期**：确保 token 未过期
3. **API 服务**：确保后端 API 服务正在运行
4. **分类创建**：如果分类不存在，脚本会自动创建
5. **重复检查**：脚本会检查 slug/title 是否已存在，避免重复导入
6. **文件删除**：默认会删除已成功导入的文件，可通过 `DELETE_FILES=false` 禁用
7. **删除已存在数据**：设置 `DELETE_EXISTING=true` 会删除数据库中已存在的记录后重新导入（谨慎使用）

## 🔍 导入结果

脚本会显示：
- ✅ 成功导入的文件数量
- ⚠️ 跳过的文件数量（已存在）
- ❌ 失败的文件数量
- 🗑️ 删除的数据库记录数量（如果启用了 DELETE_EXISTING）
- 🗑️ 删除的文件数量（如果启用了 DELETE_FILES）

## 🛠️ 故障排查

### 问题：Token 无效

**解决**：重新登录管理后台，获取新的 token

### 问题：API 连接失败

**解决**：
1. 检查后端服务是否运行
2. 检查 API_BASE 环境变量是否正确
3. 检查网络连接

### 问题：分类不存在

**解决**：脚本会自动创建分类，如果失败，可以手动在管理后台创建

### 问题：导入失败

**解决**：
1. 检查文件格式是否正确
2. 查看错误信息
3. 检查数据库连接

## 📊 导入后验证

导入完成后，可以：

1. **查看文章**：访问 `/admin/articles` 查看导入的文章
2. **查看项目**：访问 `/admin/projects` 查看导入的项目
3. **查看知识库**：访问 `/admin/knowledge` 查看导入的知识

## 🔄 重新导入

如果需要重新导入：

1. 先删除数据库中的相关数据
2. 恢复 content 目录中的文件（如果已删除）
3. 重新运行脚本

## 💡 建议

1. **分批导入**：如果文件很多，可以分批导入
2. **测试导入**：先用 `DELETE_FILES=false` 测试，确认无误后再删除
3. **保留备份**：导入前备份 content 目录

