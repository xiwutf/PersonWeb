# 文章管理数据获取问题排查

## 问题说明

文章管理页面没有获取到数据。

## 重要说明

**文章管理不访问静态数据**，它调用的是后端 API `/Articles`，从数据库的 `article` 表读取数据。

## 排查步骤

### 步骤 1: 检查浏览器控制台

打开浏览器开发者工具（F12），查看 Console 标签页，应该看到以下日志：

```
[Articles] Fetching articles, page: 1 keyword: 
[useApi] GET /Articles { isNuxtServerAPI: false, baseURL: '...', finalUrl: '...' }
[API] GET /Articles: { Total: 0, List: [] }
[Articles] API Response (raw): { "Total": 0, "List": [] }
[Articles] Parsed successfully: 0 articles, Total: 0
```

**如果看到错误**：
- `Failed to fetch articles` - API 调用失败
- `404 Not Found` - API 路由不存在
- `500 Internal Server Error` - 后端服务器错误
- `401 Unauthorized` - 需要登录认证

### 步骤 2: 检查 Network 标签页

1. 打开 Network 标签页
2. 刷新页面
3. 找到 `/Articles` 请求
4. 查看：
   - **Status Code**: 应该是 200
   - **Response**: 应该包含 `{ "code": 0, "data": { "Total": 0, "List": [] } }`

### 步骤 3: 检查数据库表

**确认数据库表是否存在**：

```sql
-- 检查表是否存在
SHOW TABLES LIKE 'article';

-- 检查表结构
DESCRIBE article;

-- 检查是否有数据
SELECT COUNT(*) FROM article;
```

**如果表不存在**，执行 `database/all_tables.sql` 脚本创建表。

**如果表存在但没有数据**，这是正常的（新项目），需要在管理后台手动添加文章。

### 步骤 4: 手动测试 API

使用 Postman 或 curl 测试 API：

```bash
# 测试文章 API（不需要认证）
curl https://api.xifg.com.cn/api/Articles?page=1&pageSize=10

# 或本地测试
curl http://localhost:5234/api/Articles?page=1&pageSize=10
```

**期望的响应**：
```json
{
  "code": 0,
  "message": "ok",
  "data": {
    "Total": 0,
    "List": []
  }
}
```

### 步骤 5: 检查后端日志

查看后端服务器的日志，确认：
- API 是否被调用
- 是否有数据库连接错误
- 是否有其他错误信息

## 常见问题

### Q: 为什么没有数据？

**A**: 最可能的原因是数据库表 `article` 是空的。这是正常的，新项目数据库表是空的，需要：
1. 在管理后台手动添加文章
2. 或执行 SQL 插入测试数据

### Q: API 返回 404？

**A**: 检查：
1. 后端 API 服务是否运行
2. API 路由是否正确：`/api/Articles`（注意大小写）
3. 检查后端 `ArticlesController` 是否存在

### Q: API 返回 500？

**A**: 可能的原因：
1. 数据库表不存在
2. 数据库连接失败
3. 后端代码错误

查看后端日志获取详细错误信息。

### Q: 为什么 Tools 有数据，但 Articles 没有？

**A**: 
- **Tools**: 使用 Nuxt Server API 读取 `content/tools/*.md` 文件（静态文件）
- **Articles**: 使用后端 API 读取数据库 `article` 表（需要数据库有数据）

## 插入测试数据

如果需要测试数据，可以执行以下 SQL：

```sql
-- 插入测试文章（需要先有分类）
INSERT INTO `article` (
    `title`, 
    `slug`, 
    `summary`, 
    `status`, 
    `created_at`, 
    `updated_at`
) VALUES (
    '测试文章',
    'test-article',
    '这是一篇测试文章',
    1,
    NOW(),
    NOW()
);
```

## 下一步

1. 打开浏览器控制台，查看详细的调试日志
2. 检查 Network 标签页，查看 API 请求和响应
3. 如果 API 返回空数组 `[]`，这是正常的（数据库是空的）
4. 在管理后台手动添加文章，或执行 SQL 插入测试数据

