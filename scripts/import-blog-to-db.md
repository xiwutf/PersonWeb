# 将 Markdown 文章导入数据库

## 问题说明

目前项目中有两个数据源：
1. **静态 Markdown 文件**：`content/blog/*.md` - 有 24 个文件
2. **数据库 `article` 表**：这个表是空的

博客页面和管理后台都从数据库读取数据，所以需要将 Markdown 文件导入到数据库。

## 使用方法

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
node scripts/import-blog-to-db.js
```

**Linux/macOS:**
```bash
export ADMIN_TOKEN="your_token_here"
node scripts/import-blog-to-db.js
```

**或使用生产环境 API:**
```bash
export ADMIN_TOKEN="your_token_here"
export API_BASE="https://api.xifg.com.cn/api"
node scripts/import-blog-to-db.js
```

### 步骤 3: 查看结果

脚本会：
- 读取 `content/blog/*.md` 文件
- 解析 frontmatter（标题、日期、分类等）
- 通过 API 导入到数据库
- 显示导入进度和结果

## 注意事项

1. **需要先创建分类**：如果 Markdown 文件中有 `category` 字段，需要先在管理后台创建对应的分类
2. **Slug 唯一性**：如果 slug 已存在，会跳过该文章
3. **日期格式**：确保 frontmatter 中的 `date` 字段格式正确（如 `2024-01-15`）
4. **内容格式**：确保 Markdown 内容格式正确

## 手动导入（如果脚本失败）

如果脚本无法运行，可以手动导入：

1. 打开管理后台的文章管理页面
2. 点击"新增文章"
3. 打开 `content/blog/` 目录中的 Markdown 文件
4. 复制 frontmatter 和内容到编辑器
5. 保存

## 后续建议

导入完成后：
- 新文章直接在管理后台创建，不再使用 Markdown 文件
- 或者继续使用 Markdown 文件，但需要定期同步到数据库
