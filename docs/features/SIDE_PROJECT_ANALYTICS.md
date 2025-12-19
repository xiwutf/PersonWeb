# 副业项目数据分析统计功能

## 功能概述

为"副业管理"模块新增数据分析统计功能，让用户一眼看到副业经营情况：收入、项目状态、风险、效率、客户贡献。

## 页面结构

### 访问路径
- `/admin/side-projects/analytics`

### 页面布局

#### A. 顶部筛选条
- **时间范围**：近30天 / 近90天 / 本年 / 自定义（startDate/endDate）
- **项目类型**：全部/软件开发/网站/AI/脚本/投资
- **是否公开**：全部/公开/不公开
- 筛选变化后自动刷新全部图表

#### B. KPI 卡片（第一行 6 个指标）
1. **项目总数**：筛选范围内新增或活跃项目
2. **进行中项目数**：Status = 0 的项目
3. **逾期项目数**：DeadlineAt < Now 且 Status != 已完成(1)/已取消(3)
4. **卡住项目数**：Blocked = true 的项目
5. **本期已收金额**：ReceivedAmount 汇总
6. **本期待收金额**：TotalAmount - ReceivedAmount（未完成/未取消的项目）

#### C. 图表区（4 个图表）
1. **项目状态分布**（饼图）：数量 + 金额（按状态汇总）
2. **月度收入趋势**（折线图）：近 12 个月 "已收金额"
3. **交付周期统计**（柱状图）：平均交付天数（按项目类型分组）
4. **客户贡献 Top10**（条形图）：按已收金额排序

#### D. 风险列表（2 个表格）
1. **即将到期项目**（≤7天）：项目名/客户/截止/剩余天数/下一步/金额
2. **逾期项目**：项目名/逾期天数/阻塞原因/金额

## 后端实现

### 新增文件

1. **DTO**
   - `backend/PersonalSite.Api/Models/Dto/SideProjectAnalyticsDto.cs` - 数据分析相关的 DTO

2. **服务**
   - `backend/PersonalSite.Api/Services/SideProjectAnalyticsService.cs` - 数据分析聚合逻辑

3. **控制器**
   - `backend/PersonalSite.Api/Controllers/SideProjectAnalyticsController.cs` - 数据分析 API 控制器

4. **数据库迁移**
   - `database/side_project_add_completed_at.sql` - 添加 CompletedAt 字段

### 修改文件

1. **实体**
   - `backend/PersonalSite.Api/Models/SideProject.cs` - 新增 `CompletedAt` 字段

2. **DTO**
   - `backend/PersonalSite.Api/Models/Dto/SideProjectDto.cs` - 新增 `CompletedAt` 字段

3. **控制器**
   - `backend/PersonalSite.Api/Controllers/SideProjectsController.cs` - 状态变化时自动设置 `CompletedAt`

4. **程序配置**
   - `backend/PersonalSite.Api/Program.cs` - 注册 `SideProjectAnalyticsService`

## API 接口

### GET `/api/side-projects/analytics/summary`

获取数据分析汇总

**查询参数**：
- `start` (string, 可选): 开始日期（YYYY-MM-DD）
- `end` (string, 可选): 结束日期（YYYY-MM-DD）
- `type` (string, 可选): 项目类型筛选
- `isPublic` (bool, 可选): 是否公开筛选

**响应示例**：
```json
{
  "code": 0,
  "message": "success",
  "data": {
    "kpis": {
      "totals": 50,
      "inProgressCount": 15,
      "overdueCount": 3,
      "blockedCount": 2,
      "receivedSum": 125000.00,
      "receivableSum": 35000.00
    },
    "statusAgg": [
      {
        "status": 0,
        "statusName": "进行中",
        "count": 15,
        "amountSum": 80000.00
      },
      {
        "status": 1,
        "statusName": "已完成",
        "count": 30,
        "amountSum": 200000.00
      }
    ],
    "monthlyRevenue": [
      {
        "month": "2024-01",
        "receivedSum": 10000.00
      },
      {
        "month": "2024-02",
        "receivedSum": 15000.00
      }
    ],
    "deliveryCycle": [
      {
        "groupName": "网站",
        "avgDays": 45.5,
        "count": 10
      },
      {
        "groupName": "AI",
        "avgDays": 30.2,
        "count": 8
      }
    ],
    "customerTop": [
      {
        "customerName": "客户A",
        "projectCount": 5,
        "receivedSum": 50000.00
      }
    ],
    "riskDueSoon": [
      {
        "id": 1,
        "title": "项目A",
        "clientName": "客户A",
        "deadlineAt": "2025-01-15T00:00:00",
        "daysRemaining": 5,
        "nextAction": "完成测试",
        "totalAmount": 10000.00
      }
    ],
    "riskOverdue": [
      {
        "id": 2,
        "title": "项目B",
        "overdueDays": 10,
        "blockReason": "等待客户反馈",
        "totalAmount": 15000.00
      }
    ]
  }
}
```

## 统计口径说明

### 1. 项目总数 (totals)
- **计算方式**：筛选范围内所有项目的数量
- **统计范围**：以项目的 `StartTime` 或 `CreatedAt` 为准（优先使用 `StartTime`，如果为空则使用 `CreatedAt`）

### 2. 进行中项目数 (inProgressCount)
- **计算方式**：`Status = 0` 的项目数量

### 3. 逾期项目数 (overdueCount)
- **计算方式**：`DeadlineAt < Now` 且 `Status != 1（已完成）` 且 `Status != 3（已取消）` 的项目数量

### 4. 卡住项目数 (blockedCount)
- **计算方式**：`Blocked = true` 的项目数量

### 5. 本期已收金额 (receivedSum)
- **计算方式**：筛选范围内所有项目的 `ReceivedAmount` 汇总

### 6. 本期待收金额 (receivableSum)
- **计算方式**：筛选范围内未完成/未取消项目的 `(TotalAmount - ReceivedAmount)` 汇总
- **条件**：`Status != 1（已完成）` 且 `Status != 3（已取消）` 且 `TotalAmount` 不为空

### 7. 项目状态分布 (statusAgg)
- **计算方式**：按 `Status` 分组，统计每个状态的项目数量和金额汇总
- **金额**：使用 `TotalAmount` 字段

### 8. 月度收入趋势 (monthlyRevenue)
- **计算方式**：近 12 个月的数据，按月份分组统计 `ReceivedAmount`
- **时间字段**：优先使用 `CompletedAt`，如果为空则使用 `UpdatedAt`
- **条件**：`ReceivedAmount > 0` 且时间在近 12 个月内

### 9. 交付周期统计 (deliveryCycle)
- **计算方式**：按项目类型（`Category`）分组，计算平均交付天数
- **交付天数**：`(EndTime 或 CompletedAt) - StartTime` 的天数
- **条件**：必须有 `StartTime` 和 `EndTime` 或 `CompletedAt`

### 10. 客户贡献 Top10 (customerTop)
- **计算方式**：按 `ClientName` 分组，统计每个客户的项目数量和已收金额汇总
- **排序**：按已收金额降序
- **条件**：`ClientName` 不为空，`ReceivedAmount > 0`

### 11. 即将到期项目 (riskDueSoon)
- **计算方式**：`DeadlineAt` 在 `Now` 到 `Now + 7天` 之间，且未完成/未取消的项目
- **排序**：按截止日期升序

### 12. 逾期项目 (riskOverdue)
- **计算方式**：`DeadlineAt < Now` 且未完成/未取消的项目
- **排序**：按逾期天数降序

## 数据模型补强

### 新增字段

#### SideProject.CompletedAt
- **类型**：`DateTime?`
- **说明**：完成时间（状态变为已完成时自动写入）
- **迁移脚本**：`database/side_project_add_completed_at.sql`

### 自动设置逻辑

在 `SideProjectsController.Update` 方法中：
- 当项目状态从非已完成变为已完成（Status = 1）时，自动设置 `CompletedAt = DateTime.Now`
- 当项目状态从已完成变为其他状态时，自动清除 `CompletedAt`

## 前端实现

### 新增文件

1. **页面**
   - `pages/admin/side-projects/analytics.vue` - 数据分析页面

2. **类型定义**
   - `types/api.ts` - 新增数据分析相关的 TypeScript 接口

### 修改文件

1. **布局**
   - `layouts/admin.vue` - 在"副业管理"菜单下新增"数据分析"入口

### 图表实现

- **图表库**：使用项目已有的 `echarts` 和 `vue-echarts`
- **主题适配**：使用 `useEChartsTheme` composable 自动适配深色/浅色主题
- **空数据状态**：所有图表都支持空数据状态，显示"暂无数据"

## 使用说明

### 数据库迁移

执行迁移脚本：
```sql
-- 执行 database/side_project_add_completed_at.sql
```

### 访问页面

1. 登录后台管理系统
2. 在左侧菜单找到"副业管理" -> "数据分析"
3. 或直接访问 `/admin/side-projects/analytics`

### 筛选数据

1. 选择时间范围（近30天/近90天/本年/自定义）
2. 选择项目类型（可选）
3. 选择是否公开（可选）
4. 点击"刷新"按钮或筛选条件变化时自动刷新

### 查看风险

- **即将到期项目**：查看 7 天内到期的项目，及时处理
- **逾期项目**：查看已逾期的项目，重点关注阻塞原因

### 点击跳转

- 点击风险列表中的任意行，可跳转到项目详情页

## 注意事项

1. **时间范围**：统计范围以项目的 `StartTime` 或 `CreatedAt` 为准
2. **月度收入**：使用 `CompletedAt` 或 `UpdatedAt` 作为收款时间（近似值）
3. **客户名称**：`ClientName` 为空的客户会被忽略，不参与 Top10 统计
4. **交付周期**：只统计有 `StartTime` 和结束时间的项目
5. **金额显示**：所有金额保留两位小数，单位：元

## 后续优化建议

1. 新增 `SideProjectPayment` 表，记录详细的收款记录，使月度收入趋势更准确
2. 支持导出报表（Excel/PDF）
3. 支持自定义时间范围对比
4. 支持更多维度的分析（如技术栈、来源等）
5. 支持数据钻取（点击图表跳转到详细列表）

