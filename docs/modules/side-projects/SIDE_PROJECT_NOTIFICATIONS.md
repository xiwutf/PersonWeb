# 副业项目站内提醒功能

## 功能概述

为解决"项目多会忘、逾期/卡住不自知"的问题，新增站内提醒中心功能。提醒由后台定时生成，前端可查看、标记已读、忽略/延后。

## 提醒规则

### 1. 项目截止前3天提醒
- **条件**：`SideProject.DeadlineAt` 不为空，且 `Status != 已完成(1)/已取消(3)`
- **触发时机**：当天时间 >= (DeadlineAt - 3天) 且未过期
- **提醒类型**：`ProjectDueSoon`
- **严重程度**：距离0天为 `Danger`，1-3天为 `Warning`

### 2. 任务到期当天提醒
- **条件**：`SideProjectTask.DueAt` 为当天（按本地日期），且 `Status != 已完成(2)`
- **触发时机**：任务到期当天
- **提醒类型**：`TaskDueToday`
- **严重程度**：`Warning`

### 3. 项目卡住超过2天提醒
- **条件**：`SideProject.Blocked=true` 且 `BlockedAt` 不为空 且 (Now - BlockedAt) >= 2天 且 `Status != 已完成(1)/已取消(3)`
- **触发时机**：项目被标记为阻塞且超过2天
- **提醒类型**：`ProjectBlockedTooLong`
- **严重程度**：`Danger`

### 去重策略

- 使用 `(Type + EntityType + EntityId + OccurDate)` 作为唯一标识
- 同一条提醒在同一天只生成一次
- 如果已存在提醒，则更新 `LastTriggeredAt` 和 `TriggerCount`，而不是创建新提醒

## 数据库设计

### 新增表：`side_notification`

| 字段 | 类型 | 说明 |
|------|------|------|
| id | BIGINT | 主键 |
| user_id | INT | 用户ID（可先固定为空） |
| type | INT | 提醒类型：1=项目即将到期, 2=任务今天到期, 3=项目卡住超过2天 |
| title | VARCHAR(100) | 提醒标题 |
| content | VARCHAR(500) | 提醒内容 |
| severity | INT | 严重程度：1=信息, 2=警告, 3=危险/紧急 |
| entity_type | VARCHAR(50) | 实体类型（SideProject / SideProjectTask） |
| entity_id | BIGINT | 实体ID |
| payload_json | TEXT | 负载数据（JSON格式） |
| is_read | TINYINT(1) | 是否已读 |
| read_at | DATETIME | 已读时间 |
| is_dismissed | TINYINT(1) | 是否已忽略 |
| dismissed_at | DATETIME | 忽略时间 |
| snooze_until | DATETIME | 延后到某个时间再出现 |
| occur_date | DATE | 发生日期（用于去重） |
| first_triggered_at | DATETIME | 首次触发时间 |
| last_triggered_at | DATETIME | 最后触发时间 |
| trigger_count | INT | 触发次数 |
| created_at | DATETIME | 创建时间 |

### 修改表：`side_project`

- 新增字段：`blocked_at` (DATETIME) - 阻塞时间（用于计算卡住天数）

### 索引

- `idx_user_id` - 用户ID索引
- `idx_type_entity` - (type, entity_type, entity_id) 复合索引
- `idx_occur_date` - 发生日期索引
- `idx_is_read` - 已读状态索引
- `idx_is_dismissed` - 忽略状态索引
- `idx_snooze_until` - 延后时间索引
- `idx_created_at` - 创建时间索引
- `uk_type_entity_occur` - 唯一索引 (type, entity_type, entity_id, occur_date) - 用于去重

## 后端实现

### 新增文件

1. **枚举类型**
   - `backend/PersonalSite.Api/Models/Enums/NotificationType.cs` - 提醒类型枚举
   - `backend/PersonalSite.Api/Models/Enums/NotificationSeverity.cs` - 严重程度枚举

2. **实体**
   - `backend/PersonalSite.Api/Models/SideNotification.cs` - 站内提醒实体

3. **DTO**
   - `backend/PersonalSite.Api/Models/Dto/SideNotificationDto.cs` - 通知相关的DTO类

4. **服务**
   - `backend/PersonalSite.Api/Services/NotificationService.cs` - 通知生成服务
   - `backend/PersonalSite.Api/Services/NotificationBackgroundService.cs` - 后台定时任务服务

5. **控制器**
   - `backend/PersonalSite.Api/Controllers/SideNotificationsController.cs` - 通知API控制器

6. **数据库迁移**
   - `database/side_notification_migration.sql` - 数据库迁移脚本

### 修改文件

1. **实体**
   - `backend/PersonalSite.Api/Models/SideProject.cs` - 新增 `BlockedAt` 字段

2. **数据上下文**
   - `backend/PersonalSite.Api/Data/AppDbContext.cs` - 新增 `SideNotifications` DbSet

3. **程序配置**
   - `backend/PersonalSite.Api/Program.cs` - 注册 `NotificationService` 和 `NotificationBackgroundService`

## 后端 API

### 基础路径：`/api/side-notifications`

#### 1. GET `/api/side-notifications`
获取提醒列表（分页、筛选）

**查询参数**：
- `status` (string, 可选): `unread` | `all` | `dismissed` - 状态筛选
- `severity` (string, 可选): `info` | `warning` | `danger` - 严重程度筛选
- `type` (int, 可选): 提醒类型（1/2/3）
- `keyword` (string, 可选): 关键字搜索（标题/内容）
- `page` (int, 默认1): 页码
- `pageSize` (int, 默认20): 每页数量

**响应**：
```json
{
  "code": 0,
  "message": "success",
  "data": {
    "items": [...],
    "total": 100,
    "page": 1,
    "pageSize": 20,
    "unreadCount": 5
  }
}
```

#### 2. GET `/api/side-notifications/unread-count`
获取未读数量

**响应**：
```json
{
  "code": 0,
  "message": "success",
  "data": {
    "count": 5
  }
}
```

#### 3. POST `/api/side-notifications/{id}/read`
标记提醒为已读

**响应**：
```json
{
  "code": 0,
  "message": "已标记为已读",
  "data": null
}
```

#### 4. POST `/api/side-notifications/read-all`
一键全部已读

**响应**：
```json
{
  "code": 0,
  "message": "已全部标记为已读",
  "data": {
    "count": 10
  }
}
```

#### 5. POST `/api/side-notifications/{id}/dismiss`
忽略提醒

**响应**：
```json
{
  "code": 0,
  "message": "已忽略",
  "data": null
}
```

#### 6. POST `/api/side-notifications/{id}/snooze`
延后提醒

**请求体**：
```json
{
  "snoozeUntil": "2025-12-20T09:00:00"  // 可选：指定时间
}
```
或
```json
{
  "preset": "1d"  // 可选：预设选项 "1d" | "3d" | "nextMon"
}
```

**响应**：
```json
{
  "code": 0,
  "message": "已延后",
  "data": {
    "snoozeUntil": "2025-12-20T09:00:00"
  }
}
```

#### 7. POST `/api/side-notifications/run-generator`
手动触发一次提醒生成（仅管理员调试用）

**响应**：
```json
{
  "code": 0,
  "message": "提醒生成完成",
  "data": null
}
```

## 前端实现

### 新增文件

1. **类型定义**
   - `types/api.ts` - 新增通知相关的 TypeScript 接口和枚举

2. **页面**
   - `pages/admin/side-projects/notifications.vue` - 提醒中心页面

3. **组件**
   - `components/NotificationBell.vue` - 全局铃铛入口组件

### 修改文件

1. **布局**
   - `layouts/admin.vue` - 添加顶部栏和铃铛入口

## 前端功能

### 提醒中心页面 (`/admin/side-projects/notifications`)

- **筛选功能**：
  - 状态筛选：未读/全部/已忽略
  - 严重程度筛选：信息/警告/危险
  - 类型筛选：项目即将到期/任务今天到期/项目卡住超过2天
  - 关键字搜索：标题/内容

- **操作功能**：
  - 标记已读
  - 查看详情（跳转到项目详情页或任务详情）
  - 延后（+1天/+3天/下周一）
  - 忽略

- **分页**：支持分页和每页数量调整

### 全局铃铛入口

- **位置**：后台布局右上角
- **功能**：
  - 显示未读数量（>=99 显示 99+）
  - 点击弹出 Popover 显示最近10条未读提醒
  - 支持查看全部、点击提醒跳转详情
  - 每60秒自动轮询未读数量

## 定时任务

- **服务**：`NotificationBackgroundService`
- **执行频率**：每10分钟执行一次
- **功能**：自动生成符合条件的提醒

## 提醒文案

### ProjectDueSoon（项目即将到期）
- **Title**：`项目即将到期：{ProjectName}`
- **Content**：`距离截止日期还有 {N} 天（{DeadlineAt}），下一步：{NextAction 或 "请更新下一步"}`

### TaskDueToday（任务今天到期）
- **Title**：`任务今天到期：{TaskTitle}`
- **Content**：`所属项目：{ProjectName}，到期日：{DueAt}，请及时处理`

### ProjectBlockedTooLong（项目卡住超过2天）
- **Title**：`项目卡住超过 {days} 天：{ProjectName}`
- **Content**：`阻塞原因：{BlockReason 或 "未填写"}，建议：记录下一步并推动确认`

## 使用说明

### 数据库迁移

执行迁移脚本：
```sql
-- 执行 database/side_notification_migration.sql
```

### 后端启动

后端服务启动后，`NotificationBackgroundService` 会自动运行，每10分钟生成一次提醒。

### 前端访问

1. **提醒中心**：访问 `/admin/side-projects/notifications`
2. **全局入口**：点击右上角铃铛图标

### 手动触发提醒生成（调试用）

```bash
curl -X POST http://localhost:5000/api/side-notifications/run-generator \
  -H "Authorization: Bearer {token}"
```

## 注意事项

1. **时区处理**：使用服务器本地时间（中国时区），按日期归一化处理
2. **去重机制**：同一天同实体同类型只生成一条提醒，已存在的提醒会更新触发时间和次数
3. **延后功能**：被延后的提醒在 `SnoozeUntil` 时间之前不会显示，也不会重复生成
4. **性能优化**：查询尽量一次拉取，按条件过滤；对任务DueToday使用日期范围查询

## 后续优化建议

1. 支持自定义提醒规则
2. 支持短信/邮件通知
3. 支持提醒模板自定义
4. 支持提醒分组和分类
5. 支持提醒统计和分析

