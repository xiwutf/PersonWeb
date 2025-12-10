# MySQL 数据库标准规范

## 📋 概述

本项目**统一使用 MySQL 数据库**，所有 SQL 语句必须遵循 MySQL 语法标准。

## ✅ MySQL 语法要求

### 1. 表创建语法

```sql
CREATE TABLE IF NOT EXISTS `table_name` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '主键ID',
    `name` VARCHAR(100) NOT NULL COMMENT '名称',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='表说明';
```

### 2. 必需的元素

- **存储引擎**：必须使用 `ENGINE=InnoDB`
- **字符集**：必须使用 `DEFAULT CHARSET=utf8mb4`
- **排序规则**：必须使用 `COLLATE=utf8mb4_unicode_ci`
- **表名和字段名**：使用反引号 `` ` `` 包裹（可选但推荐）
- **注释**：使用 `COMMENT` 为表和字段添加说明

### 3. 数据类型规范

#### 整数类型
- `TINYINT` - 小整数（-128 到 127）
- `INT` - 整数（-2,147,483,648 到 2,147,483,647）
- `BIGINT` - 大整数（用于主键和ID）

#### 字符串类型
- `VARCHAR(n)` - 可变长度字符串，n 为最大长度
- `CHAR(n)` - 固定长度字符串，n 为长度（如 GUID 使用 `CHAR(36)`）
- `TEXT` - 中等长度文本（最大 65,535 字节）
- `LONGTEXT` - 长文本（最大 4GB）

#### 日期时间类型
- `DATETIME` - 日期和时间（推荐）
- `DATE` - 仅日期
- `TIMESTAMP` - 时间戳（不推荐，有 2038 年问题）

#### 数值类型
- `DECIMAL(m, d)` - 精确小数，m 为总位数，d 为小数位数
- `FLOAT` - 单精度浮点数
- `DOUBLE` - 双精度浮点数

### 4. 自增主键

```sql
`id` BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '主键ID'
```

### 5. 时间戳字段

```sql
-- 创建时间（自动设置）
`created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',

-- 更新时间（自动更新）
`updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间'
```

### 6. 索引创建

```sql
-- 普通索引
INDEX `idx_field_name` (`field_name`)

-- 唯一索引
UNIQUE KEY `uk_field_name` (`field_name`)

-- 复合索引
INDEX `idx_field1_field2` (`field1`, `field2`)
```

### 7. 插入数据

```sql
-- 使用 ON DUPLICATE KEY UPDATE 处理重复
INSERT INTO `table_name` (`field1`, `field2`) 
VALUES ('value1', 'value2')
ON DUPLICATE KEY UPDATE `field1` = VALUES(`field1`);

-- 或使用 INSERT IGNORE
INSERT IGNORE INTO `table_name` (`field1`, `field2`) 
VALUES ('value1', 'value2');
```

### 8. 更新数据

```sql
UPDATE `table_name` 
SET `field1` = 'new_value', `updated_at` = NOW() 
WHERE `id` = 1;
```

### 9. 删除数据

```sql
DELETE FROM `table_name` WHERE `id` = 1;
```

## ❌ 禁止使用的语法

以下语法来自其他数据库系统，**禁止在项目中使用**：

### SQLite 语法（禁止）
- `INTEGER PRIMARY KEY AUTOINCREMENT` ❌
- `TEXT PRIMARY KEY` ❌
- 使用 `INTEGER` 作为主键类型 ❌

### PostgreSQL 语法（禁止）
- `SERIAL` 类型 ❌
- `BIGSERIAL` 类型 ❌
- `TEXT` 作为主键类型 ❌
- `::` 类型转换语法 ❌

### SQL Server 语法（禁止）
- `IDENTITY(1,1)` ❌
- `NVARCHAR` 类型 ❌
- `DATETIME2` 类型 ❌
- `[field]` 方括号语法 ❌

## 📝 代码中的 SQL

### Entity Framework Core

后端代码使用 Entity Framework Core，已配置为 MySQL：

```csharp
// Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```

### 连接字符串格式

```json
{
  "ConnectionStrings": {
    "Default": "Server=host;Database=database_name;User=username;Password=password;"
  }
}
```

## 🔍 验证 SQL 语法

### 使用 MySQL 客户端验证

```bash
# 连接到 MySQL
mysql -u username -p database_name

# 执行 SQL 文件
source /path/to/script.sql

# 或直接执行
mysql -u username -p database_name < script.sql
```

### 使用 MySQL Workbench

1. 打开 MySQL Workbench
2. 连接到数据库
3. 打开 SQL 文件
4. 执行并检查语法错误

## 📚 参考资源

- [MySQL 官方文档](https://dev.mysql.com/doc/)
- [MySQL 数据类型](https://dev.mysql.com/doc/refman/8.0/en/data-types.html)
- [MySQL 字符集和排序规则](https://dev.mysql.com/doc/refman/8.0/en/charset.html)

## ⚠️ 注意事项

1. **字符集**：始终使用 `utf8mb4` 以支持完整的 Unicode 字符（包括 emoji）
2. **时区**：确保数据库服务器时区设置正确
3. **大小写敏感性**：MySQL 在 Linux 上表名和字段名大小写敏感，在 Windows 上不敏感，建议统一使用小写
4. **外键约束**：根据项目设计原则，不使用外键约束，通过应用层维护关联关系
5. **GUID 存储**：使用 `CHAR(36)` 存储 GUID 格式的字符串

## ✅ 检查清单

在创建或修改 SQL 脚本时，请确认：

- [ ] 使用 `ENGINE=InnoDB`
- [ ] 使用 `DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci`
- [ ] 时间字段使用 `DATETIME` 类型
- [ ] 自增主键使用 `AUTO_INCREMENT`
- [ ] 更新时间字段使用 `ON UPDATE CURRENT_TIMESTAMP`
- [ ] 没有使用其他数据库系统的特定语法
- [ ] 所有表和字段都有 `COMMENT` 注释
- [ ] 为常用查询字段创建了索引

