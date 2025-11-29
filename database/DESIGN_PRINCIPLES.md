# 数据库设计原则

## 📋 设计原则

### 1. 不使用外键约束

**原则**：数据库设计中不使用 `FOREIGN KEY` 约束。

**原因**：
- **灵活性**：便于后期维护和扩展，不受数据库约束限制
- **性能**：减少数据库层面的约束检查，提升性能
- **解耦**：表之间逻辑解耦，便于分库分表
- **迁移**：数据迁移和备份更加灵活
- **微服务**：适合微服务架构，服务间数据独立

**实现方式**：
- 使用逻辑关联：通过字段名和注释说明关联关系
- 应用层维护：在应用代码中维护数据完整性
- 索引优化：为关联字段创建索引以提升查询性能

### 2. 命名规范

**表名**：
- 使用小写字母和下划线：`skill_category`、`dashboard_metric`
- 使用复数形式：`skills`、`categories`（可选）

**字段名**：
- 使用小写字母和下划线：`category_id`、`created_at`
- 关联字段使用 `_id` 后缀：`category_id`、`user_id`

**索引名**：
- 使用 `idx_` 前缀：`idx_category_id`、`idx_sort_order`
- 唯一索引使用 `uk_` 前缀：`uk_date`、`uk_slug`

### 3. 字段设计

**ID字段**：
- 使用 `BIGINT` 类型，`AUTO_INCREMENT`
- 主键字段统一命名为 `id`

**时间字段**：
- 创建时间：`created_at`，默认 `CURRENT_TIMESTAMP`
- 更新时间：`updated_at`，默认 `CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP`

**软删除**：
- 使用 `status` 字段标记状态，而不是物理删除
- 状态值：`active`、`inactive`、`deleted` 等

**JSON字段**：
- 使用 `JSON` 或 `TEXT` 类型存储结构化数据
- 字段名使用复数：`tags`、`features`、`images`

### 4. 索引设计

**主键索引**：
- 每个表必须有主键
- 主键字段统一为 `id`

**普通索引**：
- 为经常查询的字段创建索引
- 为关联字段创建索引：`category_id`、`user_id`
- 为排序字段创建索引：`sort_order`、`created_at`

**唯一索引**：
- 为唯一性字段创建唯一索引：`slug`、`email`、`date`

**组合索引**：
- 为多字段查询创建组合索引
- 注意字段顺序，将选择性高的字段放在前面

### 5. 注释规范

**表注释**：
- 每个表必须有 `COMMENT` 说明表的用途

**字段注释**：
- 每个字段必须有 `COMMENT` 说明字段含义
- 关联字段注释中说明关联关系：`分类ID（关联到 skill_category.id）`

### 6. 字符集和排序规则

**统一使用**：
- 字符集：`utf8mb4`
- 排序规则：`utf8mb4_unicode_ci`

**原因**：
- 支持完整的 UTF-8 字符集（包括 emoji）
- 更好的国际化支持

## 📝 示例

### 正确的表设计

```sql
CREATE TABLE `skill` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '技能ID',
    `category_id` BIGINT NOT NULL COMMENT '分类ID（逻辑关联到 skill_category.id）',
    `name` VARCHAR(100) NOT NULL COMMENT '技能名称',
    `description` TEXT DEFAULT NULL COMMENT '技能描述',
    `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    INDEX `idx_category_id` (`category_id`),
    INDEX `idx_sort_order` (`sort_order`)
    -- 注意：不使用外键约束，category_id 通过逻辑关联到 skill_category.id
    -- 关联关系由应用层维护，便于后期维护和扩展
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='技能表';
```

### 错误的表设计（使用了外键）

```sql
-- ❌ 不推荐：使用外键约束
CREATE TABLE `skill` (
    ...
    CONSTRAINT `fk_skill_category` FOREIGN KEY (`category_id`) REFERENCES `skill_category`(`id`) ON DELETE CASCADE
);
```

## 🔧 应用层维护关联关系

在应用代码中维护数据完整性：

```csharp
// C# 示例
public class SkillService
{
    // 创建技能时，验证分类是否存在
    public async Task CreateSkill(Skill skill)
    {
        var category = await _context.SkillCategories.FindAsync(skill.CategoryId);
        if (category == null)
        {
            throw new ArgumentException("分类不存在");
        }
        
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
    }
    
    // 删除分类时，检查是否有技能关联
    public async Task DeleteCategory(long categoryId)
    {
        var hasSkills = await _context.Skills.AnyAsync(s => s.CategoryId == categoryId);
        if (hasSkills)
        {
            throw new InvalidOperationException("该分类下还有技能，无法删除");
        }
        
        var category = await _context.SkillCategories.FindAsync(categoryId);
        if (category != null)
        {
            _context.SkillCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
```

## 📚 相关文档

- [数据库表结构说明](./README.md)
- [模拟数据迁移说明](./README_MOCK_DATA.md)

