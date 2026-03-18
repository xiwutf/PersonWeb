# AI 智能体系统部署检查清单

本文档提供了 AI 智能体系统部署前的完整检查清单，确保所有组件正确配置和运行。

## 📋 部署前检查清单

### 1. 数据库迁移脚本

执行以下 SQL 脚本以创建必要的表和字段：

#### 必需脚本（按顺序执行）

```bash
# 1. AI 调用日志表
mysql -u root -p personal_site < database/ai_agent_log_table.sql

# 2. Project 和 Tool 表的 AI 字段
mysql -u root -p personal_site < database/project_tool_ai_fields.sql

# 3. PreSaleConsultation 表的 AI 字段
mysql -u root -p personal_site < database/pre_sale_consultation_ai_fields.sql

# 4. 客服配置表
mysql -u root -p personal_site < database/support_config_table.sql

# 5. 个人助理会话和消息表
mysql -u root -p personal_site < database/assistant_sessions_tables.sql
```

#### 验证数据库表

执行以下 SQL 查询验证表是否创建成功：

```sql
-- 检查表是否存在
SHOW TABLES LIKE 'ai_agent_log';
SHOW TABLES LIKE 'support_config';
SHOW TABLES LIKE 'assistant_sessions';
SHOW TABLES LIKE 'assistant_messages';

-- 检查字段是否添加成功
DESCRIBE project;
DESCRIBE tool;
DESCRIBE pre_sale_consultations;
```

### 2. 后端配置检查

#### appsettings.json 配置

确保 `backend/PersonalSite.Api/appsettings.json` 中包含以下配置：

```json
{
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "InternalToken": "default-internal-token-change-in-production",
    "TimeoutSeconds": 300
  }
}
```

**重要提示**：
- 生产环境必须修改 `InternalToken` 为安全的随机字符串
- `BaseUrl` 应根据实际部署环境调整（开发/生产）

#### 服务注册验证

检查 `backend/PersonalSite.Api/Program.cs` 中是否已注册所有服务：

```csharp
// AI 服务客户端
builder.Services.AddHttpClient<PersonalSite.Api.Services.AiServiceClient>();

// AI 智能体服务
builder.Services.AddScoped<PersonalSite.Api.Services.ContentAgentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.DemoAgentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.LeadAgentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.SupportAgentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.PersonalAssistantService>();
builder.Services.AddScoped<PersonalSite.Api.Services.QuotationAgentService>();
```

#### DbContext 验证

检查 `backend/PersonalSite.Api/Data/AppDbContext.cs` 中是否包含：

```csharp
public DbSet<AiAgentLog> AiAgentLogs { get; set; }
public DbSet<SupportConfig> SupportConfigs { get; set; }
public DbSet<AssistantSession> AssistantSessions { get; set; }
public DbSet<AssistantMessage> AssistantMessages { get; set; }
```

### 3. Python AI Service 检查

#### 服务状态

确保 Python AI Service 运行在配置的地址（默认：`http://localhost:8001/api/ai`）

```bash
# 检查服务是否运行
curl http://localhost:8001/api/ai/health

# 或使用浏览器访问
# http://localhost:8001/api/ai/health
```

#### 服务配置

确保 Python AI Service 的配置与后端 `appsettings.json` 中的配置匹配：
- 端口号
- 内部 Token（如果使用）

### 4. 前端页面检查

#### 后台管理页面

验证以下页面是否存在且可访问：

- ✅ `/admin/ai` - AI 智能体中心
- ✅ `/admin/ai/content` - 内容生成智能体
- ✅ `/admin/ai/support-config` - 客服配置
- ✅ `/admin/ai/assistant` - 个人助理
- ✅ `/admin/ai/logs` - AI 日志

#### 前台组件

验证以下组件是否正确集成：

- ✅ `components/ai/SupportChat.vue` - 智能客服悬浮按钮
- ✅ `layouts/default.vue` - 包含 `SupportChat` 组件

#### 集成入口

验证以下页面中的 AI 功能按钮：

- ✅ `/admin/projects` - "AI 生成展示文案" 按钮
- ✅ `/admin/consultations` - "AI 分析" 和 "AI 生成报价" 按钮

### 5. API 接口测试

#### 健康检查

```bash
# 后端健康检查
curl http://localhost:5000/api/health

# AI 服务健康检查
curl http://localhost:5000/api/ai/health
```

#### 接口测试（需要认证 Token）

```bash
# 内容生成
curl -X POST http://localhost:5000/api/ai/content/generate \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"type":"article","title":"测试文章","tone":"mature","targetAudience":"开发者","length":"medium"}'

# 客服问答
curl -X POST http://localhost:5000/api/ai/support/answer \
  -H "Content-Type: application/json" \
  -d '{"question":"你们提供什么服务？"}'
```

### 6. 日志和监控

#### 检查日志表

```sql
-- 查看最近的 AI 调用日志
SELECT * FROM ai_agent_log 
ORDER BY created_at DESC 
LIMIT 10;

-- 查看各智能体的调用统计
SELECT agent_type, COUNT(*) as count, SUM(success) as success_count
FROM ai_agent_log
GROUP BY agent_type;
```

#### 后端日志

检查后端应用日志中是否有 AI 服务相关的错误：

```bash
# 查看日志文件（根据实际日志配置）
tail -f logs/app.log | grep -i "ai\|agent"
```

### 7. 功能测试清单

#### 内容生成智能体

- [ ] 访问 `/admin/ai/content`
- [ ] 填写表单并生成内容
- [ ] 验证生成的内容格式正确
- [ ] 测试保存为草稿功能

#### Demo 上架智能体

- [ ] 访问 `/admin/projects`
- [ ] 点击项目的 "AI 生成展示文案" 按钮
- [ ] 验证生成的文案保存到数据库
- [ ] 检查项目详情页显示 AI 生成的内容

#### 线索处理智能体

- [ ] 访问 `/admin/consultations`
- [ ] 点击线索的 "AI 分析" 按钮
- [ ] 验证摘要、标签、评分生成
- [ ] 检查详情页显示 AI 推荐

#### 自动报价智能体

- [ ] 访问 `/admin/consultations`
- [ ] 点击线索的 "AI 生成报价" 按钮
- [ ] 验证报价单生成
- [ ] 检查报价单格式和内容

#### 客服问答智能体

- [ ] 访问前台页面
- [ ] 点击右下角 "智能客服" 按钮
- [ ] 测试提问功能
- [ ] 验证回答质量
- [ ] 访问 `/admin/ai/support-config` 配置系统提示词和 FAQ

#### 个人助理智能体

- [ ] 访问 `/admin/ai/assistant`
- [ ] 创建新会话
- [ ] 发送消息并接收回复
- [ ] 测试快捷操作按钮
- [ ] 验证会话历史保存

### 8. 性能和安全检查

#### 性能

- [ ] AI 服务响应时间 < 30 秒（正常情况）
- [ ] 数据库查询性能正常
- [ ] 前端页面加载速度正常

#### 安全

- [ ] 生产环境已修改 `InternalToken`
- [ ] API 接口有适当的认证保护
- [ ] 敏感信息不在日志中暴露
- [ ] CORS 配置正确

### 9. 文档检查

- [ ] `docs/ai-agents/README.md` - 架构文档完整
- [ ] `docs/ai-agents/DEPLOYMENT_CHECKLIST.md` - 本文档
- [ ] 代码注释完整（中文）

## 🚀 部署步骤

### 开发环境

1. 执行数据库迁移脚本
2. 启动 Python AI Service
3. 启动后端 API
4. 启动前端开发服务器
5. 访问 `/admin/ai` 进行功能测试

### 生产环境

1. **数据库迁移**
   ```bash
   # 备份数据库
   mysqldump -u root -p personal_site > backup_$(date +%Y%m%d).sql
   
   # 执行迁移脚本
   mysql -u root -p personal_site < database/ai_agent_log_table.sql
   # ... 其他脚本
   ```

2. **配置更新**
   - 更新 `appsettings.json` 中的 `AiService.BaseUrl`
   - 更新 `InternalToken` 为安全值
   - 检查数据库连接字符串

3. **服务部署**
   - 部署 Python AI Service
   - 部署后端 API
   - 部署前端应用

4. **验证**
   - 运行健康检查
   - 执行功能测试
   - 检查日志

## ❗ 常见问题

### 1. AI 服务连接失败

**症状**：调用 AI 接口时返回 500 错误或超时

**解决方案**：
- 检查 Python AI Service 是否运行
- 检查 `BaseUrl` 配置是否正确
- 检查网络连接和防火墙设置
- 查看后端日志中的详细错误信息

### 2. 数据库字段不存在

**症状**：运行时出现 "Column 'ai_title' doesn't exist" 错误

**解决方案**：
- 执行对应的数据库迁移脚本
- 检查表结构：`DESCRIBE project;`

### 3. 前端页面 404

**症状**：访问 `/admin/ai/*` 页面显示 404

**解决方案**：
- 检查页面文件是否存在
- 检查路由配置
- 清除 Nuxt 缓存：`rm -rf .nuxt`

### 4. 认证失败

**症状**：调用 API 返回 401 Unauthorized

**解决方案**：
- 检查 JWT Token 是否有效
- 检查 `[Authorize]` 属性配置
- 确认用户已登录

## 📞 支持

如遇到问题，请：

1. 查看日志文件
2. 检查数据库表结构
3. 验证配置文件
4. 参考 `docs/ai-agents/README.md` 架构文档

