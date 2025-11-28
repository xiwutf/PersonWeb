# 数据获取问题排查指南

## 问题描述
文章管理、分类管理、站点配置页面都没有获取到数据。

## 可能的原因

### 1. 数据库表不存在或为空
**最可能的原因**：数据库表还没有创建，或者表中没有数据。

**解决方案**：
1. 执行 `database/all_tables.sql` 脚本创建所有表
2. 检查数据库中是否存在以下表：
   - `article` (文章表)
   - `category` (分类表)
   - `site_config` (站点配置表)
   - `projects` (项目表)
   - `visit_logs` (访问日志表)
   - `user` (用户表)

### 2. 表名大小写不匹配
**问题**：后端实体类使用了 `[Table]` 属性指定表名，表名都是小写或下划线格式：
- `Article` → `article`
- `Category` → `category`
- `SiteConfig` → `site_config`
- `VisitLog` → `visit_logs`
- `User` → `user`
- `Project` → `projects` (没有 [Table] 属性，EF Core 默认复数形式)

**解决方案**：确保数据库表名与实体类指定的表名一致。

### 3. API 响应格式问题
**问题**：前端可能没有正确解析 API 响应。

**调试方法**：
1. 打开浏览器开发者工具（F12）
2. 查看 Console 标签页的日志输出
3. 查看 Network 标签页的 API 请求和响应

**已添加的调试日志**：
- `pages/admin/articles.vue`: 会输出 `Articles API Response` 和 `Parsed articles`
- `pages/admin/categories.vue`: 会输出 `Categories API Response` 和 `Categories loaded`
- `pages/admin/config.vue`: 会输出 `Config API Response` 和 `Config keys`
- `composables/useApi.ts`: 会输出每个 API 请求的响应（开发环境）

### 4. API 认证问题
**问题**：某些 API 需要认证（`[Authorize]`），但前端没有发送 Token。

**检查**：
- `CategoriesController.GetList()` - 有 `[AllowAnonymous]`，不需要认证
- `ArticlesController.GetArticles()` - 需要检查是否有 `[Authorize]`
- `ConfigController.GetAll()` - 需要检查是否有 `[Authorize]`

### 5. 数据库连接问题
**问题**：后端无法连接到数据库。

**检查方法**：
1. 查看后端日志
2. 检查 `appsettings.json` 中的连接字符串
3. 测试数据库连接

## 排查步骤

### 步骤 1: 检查数据库表是否存在
```sql
SHOW TABLES;
```

应该看到：
- `article`
- `category`
- `site_config`
- `projects`
- `visit_logs`
- `user`

### 步骤 2: 检查表是否有数据
```sql
SELECT COUNT(*) FROM article;
SELECT COUNT(*) FROM category;
SELECT COUNT(*) FROM site_config;
```

如果都是 0，说明表是空的，这是正常的（新项目）。

### 步骤 3: 检查浏览器控制台
1. 打开管理后台页面
2. 按 F12 打开开发者工具
3. 查看 Console 标签页的日志
4. 查看 Network 标签页的 API 请求

**期望看到的日志**：
```
[API] GET /Articles: { Total: 0, List: [] }
Articles API Response: { Total: 0, List: [] }
Parsed articles: 0 Total: 0
```

### 步骤 4: 检查 API 响应
在 Network 标签页中，点击 API 请求，查看 Response：

**期望的响应格式**：
```json
{
  "code": 0,
  "message": "success",
  "data": {
    "Total": 0,
    "List": []
  }
}
```

或者对于 Categories：
```json
{
  "code": 0,
  "message": "success",
  "data": []
}
```

### 步骤 5: 手动测试 API
使用 Postman 或 curl 测试 API：

```bash
# 测试分类 API（不需要认证）
curl https://api.xifg.com.cn/api/Categories

# 测试文章 API
curl https://api.xifg.com.cn/api/Articles?page=1&pageSize=10

# 测试配置 API
curl https://api.xifg.com.cn/api/Config
```

## 常见问题

### Q: 为什么 Tools 页面有数据，但其他页面没有？
**A**: Tools 页面使用的是 Nuxt Server API (`/api/admin/tools`)，它读取的是 `content/tools/*.md` 文件，不依赖数据库。而 Articles、Categories、Config 都是从数据库读取的。

### Q: 表是空的，这是正常的吗？
**A**: 是的，这是正常的。新项目数据库表是空的，需要：
1. 在管理后台手动添加数据
2. 或者执行 SQL 脚本插入初始数据（见 `database/all_tables.sql` 的初始化数据部分）

### Q: 如何插入测试数据？
**A**: 执行以下 SQL：

```sql
-- 插入测试分类
INSERT INTO `category` (`name`, `slug`, `sort`) VALUES 
('技术文章', 'tech', 1),
('生活随笔', 'life', 2);

-- 插入测试文章
INSERT INTO `article` (`title`, `slug`, `summary`, `status`, `created_at`) VALUES 
('测试文章', 'test-article', '这是一篇测试文章', 1, NOW());

-- 插入测试配置
INSERT INTO `site_config` (`config_key`, `config_value`) VALUES 
('site_title', '溪午听风 - 个人开发者网站'),
('site_subtitle', '开发让生活更高效，代码就是我的魔方');
```

## 下一步
1. 执行 `database/all_tables.sql` 创建表
2. 刷新管理后台页面，查看控制台日志
3. 如果表是空的，这是正常的，需要在管理后台添加数据
4. 如果 API 返回错误，检查后端日志

