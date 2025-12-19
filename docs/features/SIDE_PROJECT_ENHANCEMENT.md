# 副业项目管理模块增强功能

## 概述

本次增强解决了"项目多会忘、进度不可见、需求不沉淀"的问题，通过扩展数据结构、添加新的API接口和完善前端交互来实现。

## 已完成的工作

### 1. 后端数据结构扩展（EF Core）

#### 1.1 SideProject 实体扩展
- 新增字段：
  - `Stage` (string): 阶段（待开始/进行中/卡住/待验收/已完成）
  - `Progress` (int): 进度 0-100
  - `IsProgressManual` (bool): 进度是否手动覆盖
  - `Priority` (int): 优先级（0=低，1=中，2=高，3=紧急）
  - `DeadlineAt` (DateTime?): 截止时间
  - `NextAction` (string): 下一步行动
  - `Blocked` (bool): 是否阻塞
  - `BlockReason` (string): 阻塞原因
  - `TotalAmount` (decimal?): 总金额
  - `ReceivedAmount` (decimal?): 已收款金额

#### 1.2 新增实体
- **SideProjectRequirement**: 项目需求
  - ScopeIn: 范围内需求
  - ScopeOut: 范围外需求
  - AcceptanceCriteria: 验收标准
  - Deliverables: 交付物

- **SideProjectTask**: 项目任务
  - Title, Description, Status, Priority
  - DueAt: 截止时间
  - EstHours, ActHours: 预计/实际工时
  - SortOrder: 排序顺序

- **SideProjectMilestone**: 项目里程碑
  - Title, DueAt, Status, Notes

- **SideProjectLog**: 沟通记录
  - Channel: 沟通渠道（微信/邮件/电话/会议/其他）
  - Content: 沟通内容
  - NextTodo: 下一步待办

- **SideProjectAttachment**: 附件
  - Type, Name, Url

#### 1.3 数据库迁移
文件位置: `database/side_project_enhancement_migration.sql`

### 2. API 接口（WebAPI）

#### 2.1 Dashboard 接口
- `GET /api/side-projects/dashboard` - 获取副业管理首页数据
  - 返回：今日待办、进行中项目、卡住项目、本周里程碑、收入汇总

#### 2.2 项目列表接口（已扩展）
- `GET /api/side-projects` - 支持新字段返回
- `PATCH /api/side-projects/{id}/status` - 状态流转
- `PATCH /api/side-projects/{id}/progress` - 进度覆盖/取消覆盖

#### 2.3 看板接口
- `GET /api/side-projects/kanban` - 获取看板数据（按状态分组）

#### 2.4 项目详情接口
- `GET /api/side-projects/{id}/detail` - 获取项目详情（包含所有子实体）

#### 2.5 子实体 CRUD 接口
- 需求管理：
  - `GET /api/side-projects/{id}/requirement` - 获取需求
  - `PUT /api/side-projects/{id}/requirement` - 创建/更新需求

- 任务管理：
  - `GET /api/side-projects/{id}/tasks` - 获取任务列表
  - `POST /api/side-projects/{id}/tasks` - 创建任务
  - `PUT /api/side-projects/{id}/tasks/{taskId}` - 更新任务
  - `DELETE /api/side-projects/{id}/tasks/{taskId}` - 删除任务

- 里程碑管理：
  - `GET /api/side-projects/{id}/milestones` - 获取里程碑列表
  - `POST /api/side-projects/{id}/milestones` - 创建里程碑
  - `PUT /api/side-projects/{id}/milestones/{milestoneId}` - 更新里程碑
  - `DELETE /api/side-projects/{id}/milestones/{milestoneId}` - 删除里程碑

- 沟通记录：
  - `GET /api/side-projects/{id}/logs` - 获取沟通记录列表
  - `POST /api/side-projects/{id}/logs` - 创建沟通记录
  - `DELETE /api/side-projects/{id}/logs/{logId}` - 删除沟通记录

- 附件管理：
  - `GET /api/side-projects/{id}/attachments` - 获取附件列表
  - `POST /api/side-projects/{id}/attachments` - 创建附件
  - `DELETE /api/side-projects/{id}/attachments/{attachmentId}` - 删除附件

### 3. 服务层（SideProjectService）

实现了以下业务规则：
- **进度计算**: `CalculateProgressAsync` - 基于任务完成度自动计算项目进度
- **进度更新**: `UpdateProgressIfAutoAsync` - 如果不是手动覆盖，自动更新进度
- **逾期判定**: `IsOverdue`, `IsTaskOverdue` - 判断项目和任务是否逾期
- **阻塞判定**: `ShouldBeBlockedAsync` - 判断项目是否应该被标记为阻塞
- **下一步行动**: `GetNextActionAsync` - 从任务或项目中提取下一步行动
- **状态同步**: `SyncStatusAndStageAsync` - 同步项目状态与阶段

### 4. 前端类型定义（types/api.ts）

已添加所有新字段和子实体的 TypeScript 接口定义：
- `SideProject` (扩展)
- `SideProjectRequirement`
- `SideProjectTask`
- `SideProjectMilestone`
- `SideProjectLog`
- `SideProjectAttachment`
- `SideProjectDetail`
- `SideProjectDashboard`

## 需要完成的前端页面

### 1. Dashboard 页面 (`/admin/side-projects/dashboard`)

**路由**: `/admin/side-projects/dashboard`

**功能模块**:
- 今日待办（跨项目任务）
- 进行中项目
- 卡住项目
- 本周交付
- 收入汇总

**API调用**: `GET /api/side-projects/dashboard`

### 2. 项目列表页改造 (`/admin/side-projects/projects`)

**路由**: `/admin/side-projects/projects` (当前是 `/admin/side-projects/index.vue`)

**新增列**:
- 阶段 (Stage)
- 进度 (Progress, 0-100，显示进度条)
- 下一步 (NextAction)
- 截止 (DeadlineAt)
- 优先级 (Priority)
- 阻塞 (Blocked/BlockReason)

**行内编辑支持**:
- 状态 (Status) - 下拉选择
- 阶段 (Stage) - 下拉选择
- 优先级 (Priority) - 下拉选择
- 截止 (DeadlineAt) - 日期选择器
- 阻塞 (Blocked) - 开关 + 原因输入

**行点击**: 跳转到详情页 `/admin/side-projects/projects/:id`

**筛选保存**:
- 进行中/待开始/本周交付/卡住

### 3. 看板视图 (`/admin/side-projects/kanban`)

**路由**: `/admin/side-projects/kanban`

**列**:
- 待开始
- 进行中
- 卡住
- 待验收
- 已完成

**功能**:
- 支持拖拽变更状态（使用 Naive UI 的拖拽组件或 vue-draggable）
- 拖动卡片即调用 `PATCH /api/side-projects/{id}/status`

**API调用**: `GET /api/side-projects/kanban`

### 4. 项目详情页 (`/admin/side-projects/projects/:id`)

**路由**: `/admin/side-projects/projects/[id].vue`

**Tabs**:
1. **概览**: 项目基本信息，进度展示，下一步行动，阻塞状态
2. **需求与范围**: 
   - ScopeIn/ScopeOut
   - AcceptanceCriteria
   - Deliverables
   - 支持编辑保存
3. **任务**:
   - 任务列表（支持排序）
   - 勾选完成即刷新进度
   - 支持按状态/截止筛选
   - 支持新建/编辑/删除
4. **里程碑**:
   - 里程碑列表
   - 支持新建/编辑/删除
5. **沟通记录**:
   - 快速新增（渠道下拉 + 内容 + 下一步）
   - 默认按时间倒序
   - 显示时间线
6. **财务与附件**:
   - TotalAmount/ReceivedAmount
   - 附件列表
   - 支持上传/删除附件

**进度控制**:
- 显示进度条
- 有开关控制是否手动覆盖进度
- 如果自动，任务完成度变化时自动刷新

**API调用**: `GET /api/side-projects/{id}/detail`

## 新增/修改的文件清单

### 后端文件

#### 新增
- `backend/PersonalSite.Api/Models/SideProjectRequirement.cs`
- `backend/PersonalSite.Api/Models/SideProjectTask.cs`
- `backend/PersonalSite.Api/Models/SideProjectMilestone.cs`
- `backend/PersonalSite.Api/Models/SideProjectLog.cs`
- `backend/PersonalSite.Api/Models/SideProjectAttachment.cs`
- `backend/PersonalSite.Api/Services/SideProjectService.cs`
- `database/side_project_enhancement_migration.sql`

#### 修改
- `backend/PersonalSite.Api/Models/SideProject.cs` - 扩展字段和导航属性
- `backend/PersonalSite.Api/Models/Dto/SideProjectDto.cs` - 扩展DTO和新增子实体DTO
- `backend/PersonalSite.Api/Data/AppDbContext.cs` - 添加DbSet和关系配置
- `backend/PersonalSite.Api/Controllers/SideProjectsController.cs` - 扩展API接口
- `backend/PersonalSite.Api/Program.cs` - 注册SideProjectService

### 前端文件

#### 修改
- `types/api.ts` - 扩展类型定义

#### 待创建/修改
- `pages/admin/side-projects/dashboard.vue` - Dashboard页面（已存在，需改造）
- `pages/admin/side-projects/projects.vue` - 项目列表页（需从index.vue重命名/改造）
- `pages/admin/side-projects/kanban.vue` - 看板视图（需新建）
- `pages/admin/side-projects/projects/[id].vue` - 项目详情页（需新建）

## API 请求示例

### 1. 获取Dashboard数据

```http
GET /api/side-projects/dashboard
Authorization: Bearer {token}
```

**响应**:
```json
{
  "success": true,
  "data": {
    "todayTasks": [...],
    "inProgressProjects": [...],
    "blockedProjects": [...],
    "thisWeekMilestones": [...],
    "totalIncome": 123456.78,
    "thisMonthIncome": 12345.67
  }
}
```

### 2. 更新项目状态（看板拖拽）

```http
PATCH /api/side-projects/1/status
Authorization: Bearer {token}
Content-Type: application/json

0
```

### 3. 更新项目进度（手动覆盖）

```http
PATCH /api/side-projects/1/progress
Authorization: Bearer {token}
Content-Type: application/json

{
  "isManual": true,
  "progress": 75
}
```

### 4. 获取项目详情

```http
GET /api/side-projects/1/detail
Authorization: Bearer {token}
```

### 5. 创建任务

```http
POST /api/side-projects/1/tasks
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "实现用户登录功能",
  "description": "使用JWT认证",
  "status": 0,
  "priority": 2,
  "dueAt": "2025-01-15T00:00:00Z",
  "estHours": 8
}
```

### 6. 创建沟通记录

```http
POST /api/side-projects/1/logs
Authorization: Bearer {token}
Content-Type: application/json

{
  "channel": "微信",
  "content": "客户反馈需要调整UI颜色",
  "nextTodo": "更新UI配色方案"
}
```

## 数据库迁移步骤

1. 执行迁移SQL文件：
```bash
mysql -u username -p database_name < database/side_project_enhancement_migration.sql
```

或在MySQL客户端中直接执行 `database/side_project_enhancement_migration.sql` 文件内容。

## 代码规范说明

- 所有代码注释使用中文
- DTO/实体/枚举命名清晰（使用PascalCase）
- 重要业务规则写在服务层（SideProjectService）
- 进度计算、逾期判定、阻塞判定等业务逻辑都封装在服务层

## 注意事项

1. **进度计算**: 当任务状态变化时，如果项目不是手动覆盖进度，会自动重新计算进度
2. **状态同步**: 项目状态变化时会自动同步阶段（Stage）字段
3. **阻塞判定**: 系统会自动检测是否有逾期任务且没有下一步行动，但不会自动设置Blocked字段，需要手动设置
4. **外键约束**: 所有子实体都设置了CASCADE删除，删除项目时会自动删除所有相关数据

## 下一步工作

1. 完成前端页面开发（Dashboard、列表页、看板、详情页）
2. 实现前端拖拽功能（看板视图）
3. 添加表单校验
4. 添加错误处理和加载状态
5. 测试所有功能

