# 任务/目标追踪系统

## ✅ 已完成功能

### 1. 数据库设计
- ✅ `task` 表 - 任务主表
- ✅ `goal` 表 - 年度目标表
- ✅ `monthly_kpi` 表 - 月度 KPI 表
- ✅ `task_time_log` 表 - 任务时间记录表

**SQL 文件**: `database/task_tables.sql`

### 2. 后端 API
- ✅ 任务 CRUD 操作
- ✅ 任务列表查询（支持筛选、搜索、分页）
- ✅ 任务统计接口
- ✅ 任务进度更新接口

**Controller**: `backend/PersonalSite.Api/Controllers/TasksController.cs`

**API 端点**:
- `GET /api/Tasks` - 获取任务列表
- `GET /api/Tasks/{id}` - 获取任务详情
- `POST /api/Tasks` - 创建任务
- `PUT /api/Tasks/{id}` - 更新任务
- `DELETE /api/Tasks/{id}` - 删除任务
- `PATCH /api/Tasks/{id}/progress` - 更新任务进度
- `GET /api/Tasks/stats` - 获取任务统计

### 3. 前端管理界面
- ✅ 任务列表展示
- ✅ 任务创建/编辑模态框
- ✅ 任务筛选（状态、优先级、分类）
- ✅ 任务搜索
- ✅ 任务统计卡片
- ✅ 进度条可视化

**页面**: `pages/admin/tasks/index.vue`

### 4. Dashboard 集成
- ✅ 任务统计卡片（待处理、进行中、已完成、已逾期）
- ✅ 今日任务列表
- ✅ 任务进度可视化

**位置**: `pages/dashboard/index.vue`

---

## 📋 功能特性

### 任务管理
- **任务状态**: 待处理、进行中、已完成、已取消
- **优先级**: 低、中、高、紧急（0-3）
- **分类**: 自定义分类
- **标签**: 支持多个标签
- **截止日期**: 支持设置截止时间
- **进度跟踪**: 0-100% 进度
- **子任务**: 支持父子任务关系（数据库已支持，前端待实现）

### 任务统计
- 总任务数
- 待处理任务数
- 进行中任务数
- 已完成任务数
- 已逾期任务数
- 平均进度

---

## 🚀 使用说明

### 1. 初始化数据库

执行 SQL 脚本创建表：

```bash
mysql -u root -p personal_site < database/task_tables.sql
```

### 2. 访问任务管理

1. 登录后台管理系统
2. 在侧边栏点击"任务管理"
3. 可以创建、编辑、删除任务

### 3. 查看 Dashboard

1. 访问 `/dashboard`
2. 查看任务统计卡片
3. 查看今日任务列表

---

## 📝 待实现功能

### 高优先级
- [ ] 年度目标管理（Goal CRUD）
- [ ] 月度 KPI 管理（MonthlyKpi CRUD）
- [ ] 目标与任务关联

### 中优先级
- [ ] 子任务功能（前端实现）
- [ ] 任务时间记录（Time Log）
- [ ] 任务提醒功能
- [ ] 任务导出功能

### 低优先级
- [ ] 甘特图可视化
- [ ] 任务模板
- [ ] 批量操作
- [ ] 任务评论/备注

---

## 🎯 下一步开发建议

1. **年度目标管理**
   - 创建 GoalsController
   - 实现目标 CRUD
   - 创建目标管理界面

2. **月度 KPI 管理**
   - 创建 MonthlyKpisController
   - 实现 KPI CRUD
   - 自动分解年度目标为月度 KPI

3. **甘特图可视化**
   - 使用 Chart.js 或其他图表库
   - 展示任务时间线
   - 支持拖拽调整

---

## 📊 数据模型

### Task（任务）
```typescript
{
  id: number
  title: string
  description?: string
  status: 'pending' | 'in_progress' | 'completed' | 'cancelled'
  priority: 0 | 1 | 2 | 3  // 低、中、高、紧急
  category?: string
  tags?: string
  dueDate?: string
  progress: number  // 0-100
  parentId?: number  // 父任务ID
  createdAt: string
  updatedAt: string
}
```

### Goal（年度目标）
```typescript
{
  id: number
  year: number
  title: string
  description?: string
  category?: string
  targetValue?: number
  currentValue: number
  unit?: string
  status: 'active' | 'completed' | 'archived'
  progress: number  // 0-100
  startDate?: string
  endDate?: string
}
```

### MonthlyKpi（月度 KPI）
```typescript
{
  id: number
  goalId: number
  year: number
  month: number  // 1-12
  title: string
  targetValue?: number
  currentValue: number
  unit?: string
  status: 'pending' | 'in_progress' | 'completed'
  progress: number  // 0-100
  notes?: string
}
```

---

## 🔧 技术实现

### 后端
- **框架**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core
- **数据库**: MySQL
- **认证**: JWT Bearer

### 前端
- **框架**: Nuxt 3 + Vue 3
- **UI**: Tailwind CSS
- **图表**: Chart.js（Dashboard 使用）
- **状态管理**: Composition API

---

## 📚 相关文件

- 数据库脚本: `database/task_tables.sql`
- 后端模型: `backend/PersonalSite.Api/Models/Task.cs`, `Goal.cs`, `MonthlyKpi.cs`
- 后端控制器: `backend/PersonalSite.Api/Controllers/TasksController.cs`
- 前端管理页面: `pages/admin/tasks/index.vue`
- Dashboard 集成: `pages/dashboard/index.vue`
- 后台导航: `layouts/admin.vue`

---

**最后更新**: 2025-01-XX

