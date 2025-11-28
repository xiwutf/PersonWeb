# 示例数据插入说明

## 📋 文件说明

`sample_data.sql` 包含了为各个数据表添加示例数据的 SQL 脚本。

## 📊 包含的数据表

### 1. 分类表 (category)
- 插入 5 个分类：技术文章、生活随笔、项目总结、学习笔记、工具推荐

### 2. 文章表 (article)
- 插入 3 篇示例文章
- 包含标题、内容、分类关联等完整信息

### 3. 项目表 (Projects)
- 插入 3 个示例项目
- 包含个人网站系统、AI 智能助手、投资管理系统

### 4. 知识库表 (knowledge_base)
- 插入 3 条知识记录
- 包含 MySQL 优化、Docker 部署、前端性能优化等内容

### 5. 时间线事件表 (timeline_event)
- 插入 4 个时间线事件
- 涵盖 2022-2024 年的重要事件

### 6. 投资记录表 (investment)
- 插入 3 条投资记录
- 包含股票和基金示例数据

### 7. 时间胶囊表 (time_capsule)
- 插入 4 条时间胶囊
- 包含已审核和待审核的示例

### 8. 友情链接表 (friend_links)
- 插入 4 个友情链接
- 包含 Vue.js、Nuxt.js、MDN、GitHub 等

### 9. 用户行为记录表 (user_behavior)
- 插入 3 条用户行为记录
- 用于 AI 推荐功能

### 10. 站点配置表 (site_config)
- 插入/更新站点配置
- 包含网站标题、副标题、关键词等

## 🚀 使用方法

### 方法一：使用 MySQL 命令行

```bash
# 连接到数据库
mysql -u your_username -p personal_site

# 执行脚本
source database/sample_data.sql

# 或者直接执行
mysql -u your_username -p personal_site < database/sample_data.sql
```

### 方法二：使用数据库管理工具

1. 打开 MySQL Workbench、Navicat 或其他数据库管理工具
2. 连接到 `personal_site` 数据库
3. 打开 `database/sample_data.sql` 文件
4. 执行整个脚本

### 方法三：使用 .NET 后端执行

如果后端有数据库迁移工具，可以通过代码执行 SQL 脚本。

## ⚠️ 注意事项

1. **数据重复**：脚本使用了 `ON DUPLICATE KEY UPDATE`，如果数据已存在，会更新而不是插入新数据。

2. **外键依赖**：
   - 文章表依赖分类表，确保先有分类数据
   - 脚本已按正确顺序插入数据

3. **UUID 生成**：
   - 项目表和时间胶囊表使用 `UUID()` 生成唯一 ID
   - 如果 MySQL 版本不支持 `UUID()`，需要手动替换为实际的 GUID

4. **字段名大小写**：
   - 不同表可能使用不同的命名约定（PascalCase vs snake_case）
   - 如果执行失败，检查实际的表名和字段名

5. **数据修改**：
   - 可以根据需要修改示例数据
   - 建议先备份数据库

## 🔍 验证数据

插入数据后，可以执行以下 SQL 验证：

```sql
-- 查看各表数据数量
SELECT 'category' AS table_name, COUNT(*) AS count FROM category
UNION ALL
SELECT 'article', COUNT(*) FROM article
UNION ALL
SELECT 'Projects', COUNT(*) FROM Projects
UNION ALL
SELECT 'knowledge_base', COUNT(*) FROM knowledge_base
UNION ALL
SELECT 'timeline_event', COUNT(*) FROM timeline_event
UNION ALL
SELECT 'investment', COUNT(*) FROM investment
UNION ALL
SELECT 'time_capsule', COUNT(*) FROM time_capsule
UNION ALL
SELECT 'friend_links', COUNT(*) FROM friend_links
UNION ALL
SELECT 'user_behavior', COUNT(*) FROM user_behavior
UNION ALL
SELECT 'site_config', COUNT(*) FROM site_config;
```

## 📝 自定义数据

如果需要添加自己的数据，可以：

1. 直接修改 `sample_data.sql` 文件
2. 在脚本末尾添加新的 INSERT 语句
3. 使用管理后台界面添加数据（推荐）

## 🗑️ 清理数据

如果需要清理示例数据，可以执行：

```sql
-- 注意：这会删除所有数据，谨慎使用！
DELETE FROM user_behavior;
DELETE FROM time_capsule;
DELETE FROM friend_links;
DELETE FROM investment;
DELETE FROM timeline_event;
DELETE FROM knowledge_base;
DELETE FROM Projects;
DELETE FROM article;
DELETE FROM category WHERE slug IN ('tech', 'life', 'project', 'study', 'tools');
```

## ✅ 执行成功标志

如果脚本执行成功，最后会显示：

```
message
示例数据插入完成！
```

