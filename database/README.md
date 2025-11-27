# 数据库表结构 SQL 脚本

本目录包含手动维护数据库表的 SQL 脚本。

## Projects 表

### 创建表

执行 `projects_table.sql` 文件中的 SQL 语句：

```sql
CREATE TABLE IF NOT EXISTS `Projects` (
    `Id` CHAR(36) NOT NULL COMMENT '项目ID (GUID)',
    `Title` VARCHAR(100) NOT NULL COMMENT '项目标题',
    `Description` VARCHAR(500) DEFAULT '' COMMENT '项目描述',
    `CoverUrl` VARCHAR(200) DEFAULT NULL COMMENT '封面图片URL',
    `DemoUrl` VARCHAR(200) DEFAULT NULL COMMENT '演示地址',
    `GithubUrl` VARCHAR(200) DEFAULT NULL COMMENT 'GitHub仓库地址',
    `Status` VARCHAR(50) DEFAULT 'Active' COMMENT '项目状态',
    `TechStack` TEXT DEFAULT NULL COMMENT '技术栈',
    `Content` LONGTEXT DEFAULT NULL COMMENT '项目内容',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    INDEX `idx_created_at` (`CreatedAt`),
    INDEX `idx_status` (`Status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 字段说明

| 字段名 | 类型 | 说明 | 约束 |
|--------|------|------|------|
| `Id` | CHAR(36) | 项目ID (GUID格式) | PRIMARY KEY, NOT NULL |
| `Title` | VARCHAR(100) | 项目标题 | NOT NULL |
| `Description` | VARCHAR(500) | 项目描述 | DEFAULT '' |
| `CoverUrl` | VARCHAR(200) | 封面图片URL | NULL |
| `DemoUrl` | VARCHAR(200) | 演示地址 | NULL |
| `GithubUrl` | VARCHAR(200) | GitHub仓库地址 | NULL |
| `Status` | VARCHAR(50) | 项目状态 | DEFAULT 'Active' |
| `TechStack` | TEXT | 技术栈 (JSON或逗号分隔) | NULL |
| `Content` | LONGTEXT | 项目内容 (Markdown) | NULL |
| `CreatedAt` | DATETIME | 创建时间 | DEFAULT CURRENT_TIMESTAMP |
| `UpdatedAt` | DATETIME | 更新时间 | DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP |

### 索引

- 主键索引：`Id`
- 普通索引：`CreatedAt` (用于排序)
- 普通索引：`Status` (用于筛选)

### 使用示例

#### 插入数据

```sql
INSERT INTO `Projects` (`Id`, `Title`, `Description`, `Status`, `TechStack`, `CreatedAt`, `UpdatedAt`) 
VALUES 
(UUID(), '我的项目', '项目描述', 'Active', '["Vue.js", "Nuxt 3"]', NOW(), NOW());
```

#### 查询数据

```sql
-- 查询所有项目
SELECT * FROM `Projects` ORDER BY `CreatedAt` DESC;

-- 查询活跃项目
SELECT * FROM `Projects` WHERE `Status` = 'Active';

-- 查询指定ID的项目
SELECT * FROM `Projects` WHERE `Id` = 'your-guid-here';
```

#### 更新数据

```sql
UPDATE `Projects` 
SET `Title` = '新标题', `UpdatedAt` = NOW() 
WHERE `Id` = 'your-guid-here';
```

#### 删除数据

```sql
DELETE FROM `Projects` WHERE `Id` = 'your-guid-here';
```

## 注意事项

1. **GUID 格式**：`Id` 字段使用 `CHAR(36)` 存储 GUID，格式为 `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
2. **字符集**：使用 `utf8mb4` 以支持完整的 Unicode 字符（包括 emoji）
3. **时区**：`CreatedAt` 和 `UpdatedAt` 使用服务器时区
4. **自动更新**：`UpdatedAt` 字段会在记录更新时自动更新为当前时间

