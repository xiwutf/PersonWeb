# AI 智能体系统快速启动指南

本文档提供 AI 智能体系统的快速启动步骤，帮助您快速部署和测试系统。

## 🚀 快速启动（5 分钟）

### 前置条件

1. ✅ MySQL 数据库已安装并运行
2. ✅ .NET 8 SDK 已安装
3. ✅ Node.js 18+ 已安装
4. ✅ Python AI Service 已配置（可选，用于实际 AI 调用）

### 步骤 1：数据库迁移（2 分钟）

```bash
# 进入项目根目录
cd D:\00-Project\AI\PersonWeb

# 执行数据库迁移脚本（按顺序）
mysql -u root -p personal_site < database/ai_agent_log_table.sql
mysql -u root -p personal_site < database/project_tool_ai_fields.sql
mysql -u root -p personal_site < database/pre_sale_consultation_ai_fields.sql
mysql -u root -p personal_site < database/support_config_table.sql
mysql -u root -p personal_site < database/assistant_sessions_tables.sql
```

**验证**：
```sql
-- 检查表是否创建成功
SHOW TABLES LIKE 'ai_agent_log';
SHOW TABLES LIKE 'support_config';
```

### 步骤 2：配置后端（1 分钟）

编辑 `backend/PersonalSite.Api/appsettings.json`：

```json
{
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "InternalToken": "your-secure-token-here",
    "TimeoutSeconds": 300
  }
}
```

**注意**：
- 如果 Python AI Service 未运行，可以暂时使用模拟响应（需要修改代码）
- 生产环境必须修改 `InternalToken`

### 步骤 3：启动服务（2 分钟）

#### 启动后端 API

```bash
cd backend/PersonalSite.Api
dotnet run
```

后端将在 `http://localhost:5000` 启动。

#### 启动前端（新终端）

```bash
cd D:\00-Project\AI\PersonWeb
npm run dev
```

前端将在 `http://localhost:3000` 启动。

#### 启动 Python AI Service（可选，新终端）

```bash
cd ai-service
python -m uvicorn app.main:app --host 0.0.0.0 --port 8001
```

AI Service 将在 `http://localhost:8001` 启动。

### 步骤 4：测试功能（1 分钟）

1. **访问 AI 智能体中心**
   - 打开浏览器访问：`http://localhost:3000/admin/ai`
   - 登录后台（如果需要）

2. **测试内容生成智能体**
   - 点击 "内容生成智能体" 卡片
   - 填写表单并生成内容
   - 验证生成结果

3. **测试客服问答智能体**
   - 访问前台页面：`http://localhost:3000`
   - 点击右下角 "智能客服" 按钮
   - 输入问题并测试回答

## 📋 已实现的智能体

### 1. 内容生成智能体 (ContentAgent)

**功能**：自动生成文章、项目介绍、工具说明等内容

**入口**：
- 后台：`/admin/ai/content`
- API：`POST /api/ai/content/generate`

**使用场景**：
- 生成博客文章草稿
- 生成项目介绍文案
- 生成工具使用说明

### 2. Demo 上架智能体 (DemoAgent)

**功能**：为项目/工具自动生成展示级文案

**入口**：
- 后台：`/admin/projects` → 点击项目的 "AI 生成展示文案" 按钮
- API：`POST /api/ai/demo/describe`

**使用场景**：
- 快速为项目生成卖点、场景、目标用户等展示文案
- 自动填充项目详情页的 AI 字段

### 3. 线索处理智能体 (LeadAgent)

**功能**：自动分析客户需求，生成摘要、标签和评分

**入口**：
- 后台：`/admin/consultations` → 点击线索的 "AI 分析" 按钮
- API：`POST /api/ai/leads/analyze`

**使用场景**：
- 自动分析咨询表单提交的需求
- 生成线索摘要和优先级评分
- 提取关键标签便于分类管理

### 4. 自动报价智能体 (QuotationAgent)

**功能**：根据客户需求自动生成报价方案

**入口**：
- 后台：`/admin/consultations` → 点击线索的 "AI 生成报价" 按钮
- API：`POST /api/ai/quotation/generate`

**使用场景**：
- 快速生成专业报价单
- 包含明细项、总价、付款方式等

### 5. 客服问答智能体 (SupportAgent)

**功能**：为网站访客提供智能客服问答

**入口**：
- 前台：页面右下角悬浮按钮 "智能客服"
- 后台配置：`/admin/ai/support-config`
- API：`POST /api/ai/support/answer`

**使用场景**：
- 访客咨询常见问题
- 自动回答服务内容、项目开发等问题
- 可配置系统提示词和 FAQ

### 6. 个人助理智能体 (PersonalAssistantAgent)

**功能**：为管理员提供 AI 个人助理

**入口**：
- 后台：`/admin/ai/assistant`
- API：`POST /api/ai/assistant/chat`

**使用场景**：
- 分析最近的线索情况
- 规划项目优先级
- 生成学习计划
- 提供业务建议

## 🔧 配置说明

### 后端配置

**文件**：`backend/PersonalSite.Api/appsettings.json`

```json
{
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",      // AI 服务地址
    "InternalToken": "your-token",                   // 内部调用 Token
    "TimeoutSeconds": 300                            // 超时时间（秒）
  }
}
```

### 客服配置

访问 `/admin/ai/support-config` 可以配置：
- **系统提示词**：定义客服的身份和回答风格
- **FAQ 列表**：常见问题及答案，用于上下文增强

### 数据库配置

确保 `appsettings.json` 中的数据库连接字符串正确：

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=personal_site;User=root;Password=your_password;"
  }
}
```

## 🧪 测试清单

### 基础功能测试

- [ ] 访问 `/admin/ai` 看到 AI 智能体中心页面
- [ ] 内容生成智能体可以生成内容
- [ ] Demo 上架智能体可以为项目生成文案
- [ ] 线索处理智能体可以分析线索
- [ ] 自动报价智能体可以生成报价
- [ ] 客服问答智能体可以回答问题
- [ ] 个人助理智能体可以对话

### 数据验证

- [ ] AI 调用日志记录到 `ai_agent_log` 表
- [ ] 项目 AI 字段保存到 `project` 表
- [ ] 线索 AI 字段保存到 `pre_sale_consultations` 表
- [ ] 客服配置保存到 `support_config` 表
- [ ] 助理会话保存到 `assistant_sessions` 和 `assistant_messages` 表

### 前端验证

- [ ] 所有页面可以正常访问
- [ ] 表单提交功能正常
- [ ] 数据展示正确
- [ ] 错误提示友好

## 🐛 常见问题

### 1. AI 服务连接失败

**错误**：`Failed to connect to AI service`

**解决**：
- 检查 Python AI Service 是否运行：`curl http://localhost:8001/api/ai/health`
- 检查 `BaseUrl` 配置是否正确
- 检查防火墙和网络设置

### 2. 数据库字段不存在

**错误**：`Column 'ai_title' doesn't exist`

**解决**：
- 执行数据库迁移脚本
- 检查表结构：`DESCRIBE project;`

### 3. 前端页面 404

**错误**：访问 `/admin/ai/*` 显示 404

**解决**：
- 检查页面文件是否存在
- 清除 Nuxt 缓存：`rm -rf .nuxt`
- 重启开发服务器

### 4. 认证失败

**错误**：`401 Unauthorized`

**解决**：
- 确保已登录后台
- 检查 JWT Token 是否有效
- 检查 API 路由的 `[Authorize]` 属性

## 📚 相关文档

- [AI 智能体系统架构文档](./README.md)
- [部署检查清单](./DEPLOYMENT_CHECKLIST.md)
- [数据库迁移指南](../database/AI_AGENTS_MIGRATION.md)

## 🎯 下一步

1. **配置生产环境**
   - 修改 `InternalToken` 为安全值
   - 配置生产环境的 `BaseUrl`
   - 设置数据库连接字符串

2. **优化 AI 提示词**
   - 根据实际需求调整各智能体的 Prompt
   - 配置客服系统提示词和 FAQ

3. **监控和日志**
   - 定期查看 `ai_agent_log` 表
   - 分析 AI 调用成功率和响应时间
   - 根据日志优化系统

4. **扩展功能**
   - 参考现有智能体实现新的智能体
   - 集成更多 AI 能力（如文档分析、代码生成等）

## 💡 提示

- 开发环境可以暂时不启动 Python AI Service，使用模拟响应测试前端功能
- 所有 AI 调用都会记录到 `ai_agent_log` 表，便于调试和优化
- 建议在生产环境启用日志监控，及时发现和解决问题

