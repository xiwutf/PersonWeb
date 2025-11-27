# 从 Markdown 文件迁移到数据库

## ✅ 已完成的迁移

### 博客文章
- ✅ **博客列表页面** (`pages/blog/index.vue`) - 从数据库 API `/Articles` 读取
- ✅ **博客详情页面** (`pages/blog/[id].vue`) - 从数据库 API `/Articles/slug/{slug}` 读取
- ✅ **搜索页面** (`pages/search.vue`) - 从数据库 API `/Articles` 读取
- ✅ **首页最新文章** (`pages/index.vue`) - 从数据库 API `/Articles` 读取
- ✅ **管理后台文章管理** (`pages/admin/articles.vue`) - 从数据库 API `/Articles` 读取

### 数据存储
- ✅ 文章内容存储在数据库 `article` 表的 `content_md` 字段
- ✅ 文章元数据（标题、摘要、分类等）存储在数据库
- ✅ 不再依赖 `content/blog/*.md` 文件

## 📝 迁移步骤

### 1. 导入现有 Markdown 文件到数据库

使用导入脚本将 `content/blog/*.md` 文件导入到数据库：

```bash
# 获取管理员 token（在浏览器控制台运行）
localStorage.getItem('admin_token')

# 运行导入脚本
export ADMIN_TOKEN="your_token_here"
node scripts/import-blog-to-db.js
```

### 2. 验证迁移

1. 打开博客页面：`/blog`
2. 检查文章是否正常显示
3. 打开文章详情页，检查内容是否正确
4. 测试搜索功能

### 3. 清理（可选）

迁移完成后，可以选择：
- **保留 Markdown 文件**：作为备份
- **删除 Markdown 文件**：如果确定不再需要
- **移动到备份目录**：`content/blog_backup/`

## 🔄 后续工作流程

### 创建新文章
1. 登录管理后台：`/admin/articles`
2. 点击"新增文章"
3. 在编辑器中编写 Markdown 内容
4. 保存到数据库

### 编辑文章
1. 在管理后台文章列表中找到要编辑的文章
2. 点击"编辑"
3. 修改内容后保存

### 不再需要
- ❌ 不再在 `content/blog/` 目录创建 Markdown 文件
- ❌ 不再使用 `queryContent('/blog')` 读取文件
- ❌ 不再手动管理 Markdown 文件

## 📊 数据源对比

| 功能 | 旧方式（Markdown） | 新方式（数据库） |
|------|-------------------|-----------------|
| 文章列表 | `queryContent('/blog')` | `/Articles` API |
| 文章详情 | `queryContent('/blog/[slug]')` | `/Articles/slug/{slug}` API |
| 搜索 | `queryContent('/blog')` | `/Articles` API |
| 管理后台 | Nuxt Server API | `/Articles` API |
| 内容存储 | `content/blog/*.md` | `article` 表 `content_md` 字段 |

## ⚠️ 注意事项

1. **备份数据**：迁移前建议备份 `content/blog/` 目录
2. **分类映射**：确保 Markdown 文件中的分类在数据库中存在
3. **Slug 唯一性**：确保文章的 slug 不重复
4. **内容格式**：Markdown 内容存储在数据库的 `content_md` 字段中

## 🎯 优势

- ✅ 统一的数据源（数据库）
- ✅ 更好的管理体验（管理后台）
- ✅ 支持更复杂的查询和筛选
- ✅ 更好的 SEO 支持
- ✅ 支持文章状态管理（草稿、已发布、下线）

