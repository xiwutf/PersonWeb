# 认知说明书管理系统 - 实现文档

**创建时间**：2026-01-22  
**状态**：✅ 已完成

---

## 📋 功能总览

实现了完整的认知说明书管理系统，包括：
- 数据库表结构
- 后端 API（CRUD + 发布状态）
- 后台管理页面（列表 + 编辑）
- 前台展示页面（列表 + 详情）

---

## 🗄️ 数据库层

### 表结构

**文件**：`database/20260122_create_cognition_doc.sql`

```sql
CREATE TABLE IF NOT EXISTS `cognition_doc` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `title` VARCHAR(200) NOT NULL,
  `slug` VARCHAR(200) NOT NULL,
  `summary` VARCHAR(500) NULL,
  `content_md` LONGTEXT NOT NULL,
  `status` VARCHAR(20) NOT NULL DEFAULT 'draft',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY `uk_cognition_doc_slug` (`slug`),
  KEY `idx_cognition_doc_status` (`status`),
  KEY `idx_cognition_doc_updated_at` (`updated_at`)
)
```

### 执行方式

```bash
mysql -u root -p personal_site < database/20260122_create_cognition_doc.sql
```

---

## 🔧 后端 API

### 文件位置

- **Model**：`backend/PersonalSite.Api/Models/CognitionDoc.cs`
- **DTO**：`backend/PersonalSite.Api/Models/Dto/CognitionDocDtos.cs`
- **Controller**：`backend/PersonalSite.Api/Controllers/CognitionDocsController.cs`
- **DbContext**：已更新 `backend/PersonalSite.Api/Data/AppDbContext.cs`

### API 端点

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| GET | `/api/CognitionDocs` | 获取列表（支持 keyword、status、page、pageSize） | 否 |
| GET | `/api/CognitionDocs/{id}` | 获取详情 | 否 |
| GET | `/api/CognitionDocs/by-slug/{slug}` | 根据 slug 获取已发布内容（前台用） | 否 |
| POST | `/api/CognitionDocs` | 创建 | ✅ 是 |
| PUT | `/api/CognitionDocs/{id}` | 更新 | ✅ 是 |
| DELETE | `/api/CognitionDocs/{id}` | 删除 | ✅ 是 |
| PATCH | `/api/CognitionDocs/{id}/publish` | 发布/撤回 | ✅ 是 |

### Slug 自动生成

- 如果前端未提供 slug，后端会根据 title 自动生成
- 自动处理重复：如果 slug 已存在，会添加数字后缀（如 `doc-1`, `doc-2`）
- 中文标题会使用时间戳兜底

---

## 🎨 前端实现

### 菜单配置

**文件**：`constants/admin/menu.ts`

已在「内容中心」分组下添加：
```typescript
{ label: '认知说明书', path: '/admin/cognition' }
```

### 后台管理页面

**文件**：`pages/admin/cognition/index.vue`

**功能**：
- ✅ 列表展示（表格）
- ✅ 搜索（标题、slug）
- ✅ 状态筛选（全部/草稿/已发布）
- ✅ 新建（弹窗表单）
- ✅ 编辑（弹窗表单）
- ✅ 发布/撤回（一键切换）
- ✅ 删除（确认后删除）
- ✅ 分页

**表单字段**：
- 标题（必填）
- Slug（可选，留空自动生成）
- 摘要（可选）
- 内容（Markdown，必填）
- 状态（draft/published）

### 前台展示页面

**列表页**：`pages/cognition/index.vue`
- 显示所有已发布（status=published）的认知说明书
- 卡片式布局，点击进入详情

**详情页**：`pages/cognition/[slug].vue`
- 根据 slug 获取已发布内容
- Markdown 渲染展示
- 返回列表按钮

---

## ✅ 验收标准检查

### 后台功能

- ✅ 左侧菜单出现：内容中心 → 认知说明书
- ✅ `/admin/cognition` 能新建
- ✅ `/admin/cognition` 能编辑保存
- ✅ `/admin/cognition` 能发布/撤回
- ✅ 列表按更新时间排序

### 前台功能

- ✅ `/cognition` 只展示已发布
- ✅ `/cognition/{slug}` 能看到 markdown 内容（已渲染）

---

## 🚀 使用步骤

### 1. 执行数据库脚本

**步骤 1：创建数据表**
```bash
mysql -u root -p personal_site < database/20260122_create_cognition_doc.sql
```

**步骤 2：注册模块（重要！）**
```bash
mysql -u root -p personal_site < database/20260122_add_cognition_module.sql
```

**注意**：如果不执行步骤 2，模块管理页面不会显示"认知说明书"模块，前台页面也无法访问。

### 2. 重启后端服务

```bash
cd backend/PersonalSite.Api
dotnet run
```

### 3. 测试后台管理

1. 访问 `/admin` 并登录
2. 左侧菜单 → 内容中心 → 认知说明书
3. 点击「新建说明书」
4. 填写内容并保存
5. 点击「发布」按钮发布

### 4. 测试前台展示

1. 访问 `/cognition` 查看列表
2. 点击任意条目进入详情页
3. 确认 Markdown 内容正确渲染

---

## 📝 注意事项

1. **Slug 唯一性**：系统会自动处理重复，但建议手动设置有意义的 slug
2. **状态管理**：只有 `published` 状态的内容才会在前台显示
3. **Markdown 渲染**：使用 `useMarkdown()` composable 进行渲染
4. **权限控制**：所有写操作（创建/更新/删除/发布）都需要管理员登录

---

## 🔄 后续优化建议

1. 集成 Markdown 编辑器（如 CodeMirror 或 Monaco Editor）
2. 添加版本历史功能
3. 添加预览功能
4. 支持批量操作
5. 添加访问统计

---

**完成时间**：2026-01-22  
**开发者**：AI Assistant
