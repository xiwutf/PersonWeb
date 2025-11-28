# 数据库初始化指南

## 📋 数据库创建

```sql
CREATE DATABASE IF NOT EXISTS personal_site 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;
```

## 📝 执行顺序

按以下顺序执行 SQL 脚本：

### 1. 基础表结构（必需）
```bash
mysql -u root -p personal_site < database/all_tables.sql
```

### 2. 项目表（必需）
```bash
mysql -u root -p personal_site < database/projects_table.sql
```

### 3. 文章版本历史（可选）
```bash
mysql -u root -p personal_site < database/article_version_history.sql
```

### 4. 错误日志表（可选）
```bash
mysql -u root -p personal_site < database/error_log_table.sql
```

### 5. 友情链接表（可选）
```bash
mysql -u root -p personal_site < database/friend_links_table.sql
```

### 6. 知识库表（可选）
```bash
mysql -u root -p personal_site < database/knowledge_base_tables.sql
```

### 7. 技能树表（可选）
```bash
mysql -u root -p personal_site < database/skill_tree_tables.sql
```

### 8. 时间胶囊表（可选）
```bash
mysql -u root -p personal_site < database/time_capsule_table.sql
```

### 9. 投资表（可选）
```bash
mysql -u root -p personal_site < database/investment_tables.sql
```

### 10. 全文索引（可选，提升搜索性能）
```bash
mysql -u root -p personal_site < database/fulltext_index.sql
```

## 🔄 一键执行（推荐）

### Windows PowerShell
```powershell
# 设置数据库信息
$DB_USER = "root"
$DB_PASS = "your_password"
$DB_NAME = "personal_site"

# 执行所有脚本
Get-ChildItem database\*.sql | ForEach-Object {
    Write-Host "执行: $($_.Name)"
    mysql -u $DB_USER -p$DB_PASS $DB_NAME < $_.FullName
}
```

### Linux/macOS
```bash
#!/bin/bash
DB_USER="root"
DB_PASS="your_password"
DB_NAME="personal_site"

for file in database/*.sql; do
    echo "执行: $(basename $file)"
    mysql -u $DB_USER -p$DB_PASS $DB_NAME < "$file"
done
```

## ✅ 验证

执行以下 SQL 验证表是否创建成功：

```sql
USE personal_site;
SHOW TABLES;
```

应该看到以下表：
- Articles
- Categories
- Projects
- Tools
- KnowledgeBase
- SkillCategory
- Skill
- 等等...

## 📚 更多信息

查看 [README.md](./README.md) 了解每个表的详细说明。

