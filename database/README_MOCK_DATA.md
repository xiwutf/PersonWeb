# 示例数据插入说明

## 📋 概述

本目录包含将前端写死的模拟数据迁移到数据库的脚本和说明。所有数据直接插入到对应的业务表中，不创建单独的模拟数据表。

## 📁 文件说明

- `sample_data_insert.sql` - 插入示例数据到对应表的SQL脚本
- `skill_table_update.sql` - 更新技能表结构（添加评分字段）
- `dashboard_metric_tables.sql` - 创建仪表盘指标表（如果不存在）
- `README_MOCK_DATA.md` - 本说明文档

## 🗄️ 数据表说明

### 1. 技能树数据

- **技能分类**：插入到 `skill_category` 表（已存在）
- **技能**：插入到 `skill` 表（已存在，需要先执行 `skill_table_update.sql` 添加评分字段）

### 2. 仪表盘指标数据

- **指标数据**：插入到 `dashboard_metric` 表
- 如果表不存在，先执行 `dashboard_metric_tables.sql` 创建表

### 3. 其他数据

- **工具数据**：插入到 `tool` 表（已存在）
- **生活随笔**：插入到 `article` 表，使用"生活随笔"分类

## 🚀 使用方法

### 步骤1：更新表结构（如果需要）

```bash
# 更新技能表，添加评分字段
mysql -u your_username -p personal_site < database/skill_table_update.sql

# 创建仪表盘指标表（如果不存在）
mysql -u your_username -p personal_site < database/dashboard_metric_tables.sql
```

### 步骤2：插入示例数据

```bash
mysql -u your_username -p personal_site < database/sample_data_insert.sql
```

或在MySQL客户端中执行：
```sql
source database/sample_data_insert.sql
```

### 步骤3：验证数据

```sql
-- 查看技能分类
SELECT * FROM skill_category;

-- 查看技能
SELECT * FROM skill;

-- 查看仪表盘指标
SELECT * FROM dashboard_metric ORDER BY date DESC LIMIT 7;

-- 查看工具数据
SELECT name, slug, price FROM tool WHERE status = 'published';

-- 查看生活随笔
SELECT title, slug FROM article WHERE category_id = (SELECT id FROM category WHERE name = '生活随笔');
```

## 🔧 后端API

后端已创建 `MockDataController`，提供以下API：

- `GET /api/MockData/skill-tree` - 获取技能树数据（从 skill_category 和 skill 表）
- `GET /api/MockData/skill-categories` - 获取技能分类列表（从 skill_category 表）
- `GET /api/MockData/dashboard-metrics?days=7` - 获取仪表盘指标（从 dashboard_metric 表，默认7天）
- `GET /api/MockData/tools` - 获取工具列表（从 tool 表）
- `GET /api/MockData/life-posts` - 获取生活随笔列表（从 article 表）

## 🗑️ 数据管理

所有数据都可以通过前端管理界面进行：
- **查看**：在对应的管理页面查看数据
- **编辑**：点击编辑按钮修改数据
- **删除**：点击删除按钮删除数据

数据存储在对应的业务表中，与正式数据使用相同的表结构，便于统一管理。

## 📝 前端修改

以下页面已修改为从API获取数据：

1. **`pages/tools/index.vue`** - 工具页面
2. **`pages/life/index.vue`** - 生活随笔页面
3. **`pages/skills/index.vue`** - 技能树页面
4. **`pages/dashboard/index.vue`** - 仪表盘页面

所有页面都优先从新的 `MockData` API获取数据，如果API没有数据，会尝试从旧API或 `@nuxt/content` 获取。

## ✏️ 修改数据

### 修改技能树数据

```sql
-- 更新技能评分
UPDATE skill SET current_rating = 9.0 WHERE name = 'Vue.js';

-- 添加新技能
INSERT INTO skill (category_id, name, icon, description, current_rating, sort_order)
VALUES (1, 'React', '⚛️', '用于构建用户界面的JavaScript库', 7.5, 5);
```

### 修改仪表盘指标

```sql
-- 更新今日指标
UPDATE dashboard_metric 
SET steps = 10000, sleep = 8.0, energy = 85 
WHERE date = CURDATE();

-- 插入新日期指标
INSERT INTO dashboard_metric (date, steps, sleep, weight, net_worth, energy)
VALUES (CURDATE(), 10000, 8.0, 70.0, 52000.00, 85);
```

### 修改工具数据

```sql
-- 更新工具价格
UPDATE tool SET price = 199.00 WHERE slug = 'smart-wall-generator';

-- 添加新工具
INSERT INTO tool (name, slug, description, price, status, tags)
VALUES ('新工具', 'new-tool', '工具描述', 299.00, 'published', '["Revit", "新功能"]');
```

### 修改生活随笔

通过文章管理后台或直接修改 `article` 表。

## ⚠️ 注意事项

1. **数据备份**：修改数据前建议先备份数据库
2. **外键约束**：技能表依赖技能分类表，删除分类前需先删除相关技能
3. **日期格式**：仪表盘指标表的日期字段使用 `DATE` 类型，格式为 `YYYY-MM-DD`
4. **JSON字段**：工具表的 `tags` 字段为JSON格式，插入时需确保格式正确

## 🔄 数据同步

如果需要在多个环境（开发、测试、生产）之间同步数据，可以：

1. 导出数据：
```bash
mysqldump -u username -p personal_site skill_category skill dashboard_metric > mock_data_backup.sql
```

2. 导入数据：
```bash
mysql -u username -p personal_site < mock_data_backup.sql
```

## 📚 相关文档

- [数据库表结构说明](../database/README.md)
- [示例数据插入说明](../database/README_SAMPLE_DATA.md)

