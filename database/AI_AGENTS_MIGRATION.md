# AI 智能体系统数据库迁移指南

本文档提供了 AI 智能体系统所需的所有数据库迁移脚本及其执行顺序。

## 📋 迁移脚本列表

### 1. AI 调用日志表

**文件**：`database/ai_agent_log_table.sql`

**用途**：记录所有 AI 智能体的调用请求和响应，用于监控、调试和统计分析。

**表结构**：
- `id` - 主键
- `agent_type` - 智能体类型（Content, Demo, Lead, Support, Assistant, Quotation）
- `request_payload` - 请求内容（JSON）
- `response_payload` - 响应内容（JSON）
- `success` - 是否成功
- `error_message` - 错误信息
- `created_at` - 创建时间

**执行命令**：
```bash
mysql -u root -p personal_site < database/ai_agent_log_table.sql
```

### 2. Project 和 Tool 表的 AI 字段

**文件**：`database/project_tool_ai_fields.sql`

**用途**：为项目和工具表添加 AI 生成的展示文案字段。

**新增字段**：
- `ai_title` - AI 生成的标题
- `ai_highlights` - AI 生成的亮点（JSON 或分号分隔）
- `ai_description` - AI 生成的详情描述（Markdown）
- `ai_scenarios` - AI 生成的使用场景
- `ai_target_users` - AI 生成的目标用户
- `ai_short_card_text` - AI 生成的卡片短文本

**执行命令**：
```bash
mysql -u root -p personal_site < database/project_tool_ai_fields.sql
```

**验证**：
```sql
DESCRIBE project;
DESCRIBE tool;
```

### 3. PreSaleConsultation 表的 AI 字段

**文件**：`database/pre_sale_consultation_ai_fields.sql`

**用途**：为预售咨询（线索）表添加 AI 分析结果字段。

**新增字段**：
- `summary` - AI 生成的摘要
- `tags` - AI 生成的标签（JSON 数组或逗号分隔）
- `score` - AI 评分（0-100）
- `ai_recommendation` - AI 推荐建议

**执行命令**：
```bash
mysql -u root -p personal_site < database/pre_sale_consultation_ai_fields.sql
```

**验证**：
```sql
DESCRIBE pre_sale_consultations;
```

### 4. 客服配置表

**文件**：`database/support_config_table.sql`

**用途**：存储客服智能体的系统提示词和 FAQ 配置。

**表结构**：
- `id` - 主键
- `config_key` - 配置键（system_prompt, faq_list）
- `config_value` - 配置值（JSON 或文本）
- `description` - 配置描述
- `created_at` - 创建时间
- `updated_at` - 更新时间

**执行命令**：
```bash
mysql -u root -p personal_site < database/support_config_table.sql
```

**验证**：
```sql
SELECT * FROM support_config;
```

### 5. 个人助理会话和消息表

**文件**：`database/assistant_sessions_tables.sql`

**用途**：存储个人助理智能体的会话历史和消息记录。

**表结构**：

**assistant_sessions**：
- `id` - 主键
- `user_id` - 用户 ID（管理员）
- `title` - 会话标题
- `created_at` - 创建时间
- `updated_at` - 更新时间

**assistant_messages**：
- `id` - 主键
- `session_id` - 会话 ID（外键）
- `role` - 角色（User / Assistant）
- `content` - 消息内容
- `created_at` - 创建时间

**执行命令**：
```bash
mysql -u root -p personal_site < database/assistant_sessions_tables.sql
```

**验证**：
```sql
SHOW TABLES LIKE 'assistant_%';
DESCRIBE assistant_sessions;
DESCRIBE assistant_messages;
```

## 🚀 一键执行所有迁移

### Windows PowerShell

```powershell
# 设置数据库信息
$DB_USER = "root"
$DB_PASS = "your_password"
$DB_NAME = "personal_site"

# 迁移脚本列表（按顺序）
$scripts = @(
    "database/ai_agent_log_table.sql",
    "database/project_tool_ai_fields.sql",
    "database/pre_sale_consultation_ai_fields.sql",
    "database/support_config_table.sql",
    "database/assistant_sessions_tables.sql"
)

# 执行所有脚本
foreach ($script in $scripts) {
    Write-Host "执行: $script" -ForegroundColor Green
    if (Test-Path $script) {
        $content = Get-Content $script -Raw
        mysql -u $DB_USER -p$DB_PASS $DB_NAME -e $content
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ 成功" -ForegroundColor Green
        } else {
            Write-Host "✗ 失败" -ForegroundColor Red
        }
    } else {
        Write-Host "✗ 文件不存在: $script" -ForegroundColor Red
    }
    Write-Host ""
}
```

### Linux/macOS

```bash
#!/bin/bash
DB_USER="root"
DB_PASS="your_password"
DB_NAME="personal_site"

# 迁移脚本列表（按顺序）
scripts=(
    "database/ai_agent_log_table.sql"
    "database/project_tool_ai_fields.sql"
    "database/pre_sale_consultation_ai_fields.sql"
    "database/support_config_table.sql"
    "database/assistant_sessions_tables.sql"
)

# 执行所有脚本
for script in "${scripts[@]}"; do
    echo "执行: $script"
    if [ -f "$script" ]; then
        mysql -u "$DB_USER" -p"$DB_PASS" "$DB_NAME" < "$script"
        if [ $? -eq 0 ]; then
            echo "✓ 成功"
        else
            echo "✗ 失败"
        fi
    else
        echo "✗ 文件不存在: $script"
    fi
    echo ""
done
```

## ✅ 迁移后验证

执行以下 SQL 查询验证所有表和字段已正确创建：

```sql
-- 1. 检查表是否存在
SELECT TABLE_NAME 
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'personal_site' 
  AND TABLE_NAME IN (
    'ai_agent_log',
    'support_config',
    'assistant_sessions',
    'assistant_messages'
  );

-- 2. 检查 Project 表的 AI 字段
SELECT COLUMN_NAME, DATA_TYPE, COLUMN_COMMENT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'personal_site'
  AND TABLE_NAME = 'project'
  AND COLUMN_NAME LIKE 'ai_%';

-- 3. 检查 Tool 表的 AI 字段
SELECT COLUMN_NAME, DATA_TYPE, COLUMN_COMMENT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'personal_site'
  AND TABLE_NAME = 'tool'
  AND COLUMN_NAME LIKE 'ai_%';

-- 4. 检查 PreSaleConsultation 表的 AI 字段
SELECT COLUMN_NAME, DATA_TYPE, COLUMN_COMMENT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'personal_site'
  AND TABLE_NAME = 'pre_sale_consultations'
  AND COLUMN_NAME IN ('summary', 'tags', 'score', 'ai_recommendation');

-- 5. 检查默认配置数据
SELECT * FROM support_config;
```

## 🔄 回滚脚本（如果需要）

如果需要回滚迁移，可以执行以下 SQL：

```sql
-- 注意：回滚会删除数据，请谨慎操作！

-- 删除个人助理相关表
DROP TABLE IF EXISTS assistant_messages;
DROP TABLE IF EXISTS assistant_sessions;

-- 删除客服配置表
DROP TABLE IF EXISTS support_config;

-- 删除 AI 日志表
DROP TABLE IF EXISTS ai_agent_log;

-- 删除 PreSaleConsultation 表的 AI 字段
ALTER TABLE pre_sale_consultations 
DROP COLUMN IF EXISTS ai_recommendation,
DROP COLUMN IF EXISTS score,
DROP COLUMN IF EXISTS tags,
DROP COLUMN IF EXISTS summary;

-- 删除 Project 表的 AI 字段
ALTER TABLE project 
DROP COLUMN IF EXISTS ai_short_card_text,
DROP COLUMN IF EXISTS ai_target_users,
DROP COLUMN IF EXISTS ai_scenarios,
DROP COLUMN IF EXISTS ai_description,
DROP COLUMN IF EXISTS ai_highlights,
DROP COLUMN IF EXISTS ai_title;

-- 删除 Tool 表的 AI 字段
ALTER TABLE tool 
DROP COLUMN IF EXISTS ai_short_card_text,
DROP COLUMN IF EXISTS ai_target_users,
DROP COLUMN IF EXISTS ai_scenarios,
DROP COLUMN IF EXISTS ai_description,
DROP COLUMN IF EXISTS ai_highlights,
DROP COLUMN IF EXISTS ai_title;
```

## 📝 注意事项

1. **备份数据库**：在执行迁移前，请务必备份数据库
   ```bash
   mysqldump -u root -p personal_site > backup_$(date +%Y%m%d_%H%M%S).sql
   ```

2. **执行顺序**：请按照文档中的顺序执行脚本，某些脚本可能依赖前面的脚本

3. **MySQL 版本**：确保 MySQL 版本 >= 5.7（支持 JSON 类型和 `IF NOT EXISTS` 语法）

4. **字符集**：所有表使用 `utf8mb4` 字符集和 `utf8mb4_unicode_ci` 排序规则

5. **外键约束**：`assistant_messages` 表有外键约束，删除会话时会级联删除消息

## 🆘 故障排除

### 问题 1：字段已存在错误

如果遇到 "Duplicate column name" 错误，说明字段已经存在。可以：
- 跳过该脚本
- 或使用 `ALTER TABLE ... DROP COLUMN` 删除后重新执行

### 问题 2：表已存在错误

如果遇到 "Table already exists" 错误，可以：
- 脚本中使用了 `CREATE TABLE IF NOT EXISTS`，应该不会出现此错误
- 如果仍然出现，检查表名是否正确

### 问题 3：外键约束错误

如果 `assistant_messages` 表创建失败，检查：
- `assistant_sessions` 表是否已创建
- 外键引用的表名和字段名是否正确

## 📚 相关文档

- [AI 智能体系统架构文档](../docs/ai-agents/README.md)
- [部署检查清单](../docs/ai-agents/DEPLOYMENT_CHECKLIST.md)

