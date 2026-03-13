# 关系跟进模块 · Phase 6 独立性分析

> **目标**：确保模块可以被视为"一个独立产品"，未来可拆出去  
> **状态**：📋 分析完成

---

## 一、当前状态分析

### ✅ 1.1 Prompt 独立性

**状态**：✅ **已独立**

**位置**：
- `ai-service/app/prompts/relation_followup/system_v1.txt`
- `ai-service/app/prompts/relation_followup/user_v1.txt`

**分析**：
- ✅ Prompt 文件独立存放在 `relation_followup` 目录下
- ✅ 内容通用，没有硬编码个人信息
- ✅ 使用变量占位符，不依赖特定用户信息
- ✅ 可以使用现有的 `PromptLoader` 加载

**建议**：
- ✅ 保持现状，无需修改

---

### ✅ 1.2 API 独立性

**状态**：✅ **基本独立**（需要小调整）

**位置**：
- `backend/PersonalSite.Api/Controllers/RelationsController.cs`
- 路由：`[Route("api/relations")]`

**分析**：
- ✅ Controller 独立存在
- ✅ 路由独立：`/api/relations/*`
- ✅ API 端点结构清晰：
  - `/api/relations/persons` - 对象管理
  - `/api/relations/interactions` - 互动管理
  - `/api/relations/tasks` - 任务管理
  - `/api/relations/ai/summarize` - AI 建议
- ⚠️ 目前使用 `[Authorize]` 属性，未来可能需要支持 API Key 或多种认证方式

**建议**：
- ✅ 保持独立路由结构
- 📝 未来可扩展支持多种认证方式（当前使用 JWT 是合理的）

---

### ⚠️ 1.3 模块命名中性

**状态**：⚠️ **基本中性，但有一些注释可优化**

**当前命名**：
- 类名：`RelationPerson`, `RelationInteraction`, `RelationTask` ✅
- Controller：`RelationsController` ✅
- 路由：`/api/relations` ✅
- 表名：`relation_person`, `relation_interaction`, `relation_task` ✅

**需要检查的地方**：
- Prompt 文件中的描述：提到"潜在交往对象/相亲对象" - 这是功能描述，还算中性
- 注释中的说明：基本中性

**建议**：
- ✅ 命名已足够中性，可保持现状
- 📝 如需更通用，可以考虑：
  - `RelationPerson` → `Contact` 或 `RelationContact`
  - 但当前命名已经很通用，不建议改动

---

### ❌ 1.4 数据模型可自然加 UserId

**状态**：❌ **需要添加 UserId 字段**

**当前模型**：
```csharp
public class RelationPerson
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    // ... 其他字段 ...
    
    // ❌ 缺少 UserId 字段
}
```

**分析**：
- ❌ 当前模型没有 `UserId` 字段
- ❌ 无法直接支持多租户（多用户）
- ✅ 表结构可以很容易添加 `UserId` 字段

**建议**：
- ✅ 添加 `UserId` 字段（可为 `Guid` 或 `string`，根据用户系统类型）
- ✅ 添加索引以提升查询性能
- ✅ 在所有查询中自动过滤 `UserId`

---

## 二、需要修改的内容

### 2.1 数据模型添加 UserId

**修改文件**：
1. `backend/PersonalSite.Api/Models/RelationPerson.cs`
2. `backend/PersonalSite.Api/Models/RelationInteraction.cs`
3. `backend/PersonalSite.Api/Models/RelationTask.cs`
4. `database/relation_tables.sql`

**修改内容**：

#### 1. 模型类添加 UserId

```csharp
/// <summary>
/// 用户ID（支持多租户）
/// </summary>
[Column("user_id")]
public string? UserId { get; set; } // 可为 string 或 Guid，根据用户系统类型
```

#### 2. 数据库表添加字段

```sql
ALTER TABLE `relation_person` 
ADD COLUMN `user_id` VARCHAR(100) NULL COMMENT '用户ID（支持多租户）' AFTER `id`,
ADD INDEX `idx_user_id` (`user_id`);

ALTER TABLE `relation_interaction` 
ADD COLUMN `user_id` VARCHAR(100) NULL COMMENT '用户ID（支持多租户）' AFTER `id`,
ADD INDEX `idx_user_id` (`user_id`);

ALTER TABLE `relation_task` 
ADD COLUMN `user_id` VARCHAR(100) NULL COMMENT '用户ID（支持多租户）' AFTER `id`,
ADD INDEX `idx_user_id` (`user_id`);
```

#### 3. Controller 中添加 UserId 过滤

```csharp
// 从 JWT 或其他认证方式获取 UserId
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

// 查询时自动过滤
query = query.Where(p => p.UserId == userId);
```

---

## 三、可选优化建议

### 3.1 更通用的命名（可选）

如果希望命名更通用，可以考虑：
- `RelationPerson` → `Contact` 或 `RelationContact`
- `RelationsController` → `ContactsController`

**建议**：当前命名已经很通用，不建议改动

### 3.2 API 认证方式（可选）

当前使用 JWT `[Authorize]`，未来可以考虑：
- 支持 API Key 认证
- 支持 OAuth2
- 支持多租户 SaaS 模式

**建议**：当前实现合理，未来按需扩展

---

## 四、独立性检查清单

### ✅ 已满足

- [x] **Prompt 独立**：存放在独立目录，内容通用
- [x] **API 独立**：独立 Controller 和路由
- [x] **模块命名中性**：类名、路由、表名都是中性的
- [x] **代码结构清晰**：易于拆分

### ❌ 需要修改

- [ ] **数据模型 UserId**：需要添加 `UserId` 字段
- [ ] **查询过滤**：需要在所有查询中添加 `UserId` 过滤
- [ ] **创建数据**：创建数据时需要设置 `UserId`

---

## 五、实施建议

### 优先级 1：必须做（为独立性做准备）

1. **添加 UserId 字段到数据模型**
   - 修改三个模型类（RelationPerson, RelationInteraction, RelationTask）
   - 创建数据库迁移脚本
   - 在 Controller 中添加 UserId 过滤逻辑

2. **确保所有查询包含 UserId 过滤**
   - GetPersons
   - GetPerson
   - GetInteractions
   - GetTasks
   - 所有创建/更新操作

### 优先级 2：可选优化

1. 文档化模块的独立性
2. 考虑更通用的命名（如果需要）
3. 支持多种认证方式（如果需要）

---

## 六、总结

**当前状态**：
- ✅ Prompt 已独立
- ✅ API 已独立
- ✅ 命名已中性
- ❌ 缺少 UserId 字段（需要添加）

**建议**：
- 立即添加 `UserId` 字段到所有模型和表
- 在所有查询中添加 `UserId` 过滤
- 其他方面已满足独立性要求

**拆出难度**：⭐⭐☆☆☆（低）
- 只需要添加 `UserId` 字段和相关逻辑
- 其他方面已经足够独立

---

**文档生成时间**：2025-01-XX  
**Phase 6 状态**：📋 分析完成，待实施

