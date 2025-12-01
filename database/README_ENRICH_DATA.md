# 网站内容数据丰富脚本说明

## 文件说明

`enrich_content_data.sql` 是一个用于向数据库插入示例数据的 SQL 脚本，包含：

- **文章数据**：8 篇技术博客文章和 2 篇生活随笔
- **项目数据**：5 个项目（包括个人网站、AI 工具等）
- **工具数据**：8 个实用工具（Markdown 转换、JSON 格式化等）
- **分类数据**：5 个分类（技术博客、生活随笔、项目分享、工具推荐、AI探索）

## 执行方法

### 方法一：使用 MySQL 命令行

```bash
mysql -u your_username -p your_database_name < database/enrich_content_data.sql
```

### 方法二：使用 MySQL Workbench 或其他数据库工具

1. 打开 MySQL Workbench 或其他数据库管理工具
2. 连接到你的数据库
3. 打开 `database/enrich_content_data.sql` 文件
4. 执行整个脚本

### 方法三：在 .NET 项目中执行

如果你使用的是 Entity Framework Core，可以通过以下方式执行：

```csharp
// 在 Program.cs 或 Startup.cs 中
var sql = File.ReadAllText("database/enrich_content_data.sql");
await context.Database.ExecuteSqlRawAsync(sql);
```

## 注意事项

1. **备份数据**：执行前请先备份数据库，避免覆盖现有数据
2. **字段名大小写**：
   - `projects` 表的字段名是大写（`Id`, `Title` 等）
   - `article` 表的字段名是小写（`id`, `title` 等）
3. **唯一键冲突**：脚本使用了 `ON DUPLICATE KEY UPDATE`，如果数据已存在会更新而不是插入
4. **分类依赖**：文章和项目需要分类数据，脚本会自动创建分类

## 数据内容概览

### 文章（10 篇）

**技术博客（6 篇）**：
- Vue 3 Composition API 深度解析
- Nuxt 3 服务端渲染性能优化实战
- TypeScript 在大型项目中的应用实践
- Tailwind CSS 设计系统构建指南
- Entity Framework Core 性能优化技巧
- AI 辅助编程：从 Copilot 到自定义 Agent

**生活随笔（2 篇）**：
- 2024 年终总结：技术成长与生活感悟
- 阅读的力量：从技术文档到人文书籍

### 项目（5 个）

- 个人网站 V2
- AI 创作助手
- 工具市场平台
- 访客分析系统
- 主题商店

### 工具（8 个）

- Markdown 转 HTML
- JSON 格式化工具
- Base64 编解码工具
- 颜色选择器
- 二维码生成器
- 时间戳转换工具
- 文本差异对比工具
- URL 参数解析工具

## 验证数据

执行脚本后，可以运行以下 SQL 查询验证数据：

```sql
-- 查看文章数量
SELECT COUNT(*) AS '文章数量' FROM `article` WHERE `status` = 1;

-- 查看项目数量
SELECT COUNT(*) AS '项目数量' FROM `projects` WHERE `Status` = 'Active';

-- 查看工具数量
SELECT COUNT(*) AS '工具数量' FROM `tool` WHERE `status` = 'published';
```

## 自定义数据

如果需要修改或添加数据，可以直接编辑 `enrich_content_data.sql` 文件，然后重新执行。

## 问题排查

如果执行时遇到错误：

1. **表不存在**：请先执行 `database/all_tables.sql` 创建表结构
2. **字段不存在**：检查表结构是否与脚本中的字段名匹配
3. **外键约束**：如果遇到外键约束错误，请检查分类数据是否正确创建

