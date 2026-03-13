# 资产管理功能扩展实现总结

**最后更新**：2025-01-XX  
**版本**：1.0.0

## 📋 概述

本文档记录了资产管理模块的扩展功能实现情况，包括定投计划管理、价格提醒功能等。

## ✅ 已实现功能

### 1. 定投计划管理 ✅

#### 功能描述
允许用户创建和管理定投计划，支持自动执行定投操作。

#### 实现内容

**后端实现**：
- ✅ 数据模型：`DcaPlan.cs`、`DcaExecution.cs`
- ✅ 数据库表：`dca_plan`、`dca_execution`
- ✅ API 控制器：`DcaPlanController.cs`
  - `GET /api/DcaPlan` - 获取定投计划列表
  - `GET /api/DcaPlan/{id}` - 获取定投计划详情
  - `POST /api/DcaPlan` - 创建定投计划
  - `PUT /api/DcaPlan/{id}` - 更新定投计划
  - `DELETE /api/DcaPlan/{id}` - 删除定投计划
  - `POST /api/DcaPlan/{id}/execute` - 手动执行定投
  - `GET /api/DcaPlan/{id}/executions` - 获取执行记录

**前端实现**：
- ✅ 页面：`pages/admin/dca-plan.vue`
- ✅ 功能：
  - 定投计划列表展示
  - 新增/编辑/删除定投计划
  - 手动执行定投
  - 自动获取名称和价格
  - 支持多种定投频率（每日/每周/每月/每季度）

**数据库脚本**：
- ✅ `database/dca_plan_tables.sql`

#### 核心特性
- 支持多种定投频率：每日、每周、每月、每季度
- 自动计算下次执行日期
- 执行定投时自动更新投资记录
- 支持启用/停用定投计划
- 记录每次执行的历史

### 2. 价格提醒功能 ✅

#### 功能描述
允许用户为资产设置价格提醒，当价格达到目标值时触发提醒。

#### 实现内容

**后端实现**：
- ✅ 数据模型：`PriceAlert.cs`
- ✅ 数据库表：`price_alert`
- ✅ API 控制器：`PriceAlertController.cs`
  - `GET /api/PriceAlert` - 获取价格提醒列表
  - `GET /api/PriceAlert/{id}` - 获取价格提醒详情
  - `POST /api/PriceAlert` - 创建价格提醒
  - `PUT /api/PriceAlert/{id}` - 更新价格提醒
  - `DELETE /api/PriceAlert/{id}` - 删除价格提醒
  - `POST /api/PriceAlert/refresh-prices` - 刷新所有提醒的价格

**前端实现**：
- ✅ 页面：`pages/admin/price-alert.vue`
- ✅ 功能：
  - 价格提醒列表展示
  - 新增/编辑/删除价格提醒
  - 刷新价格功能
  - 自动获取名称和价格
  - 支持多种提醒类型（高于/低于/达到目标价格）

**数据库脚本**：
- ✅ `database/price_alert_tables.sql`

#### 核心特性
- 支持三种提醒类型：高于目标价格、低于目标价格、达到目标价格
- 自动刷新当前价格
- 触发状态跟踪
- 支持启用/停用提醒

## ⚠️ 待实现功能

### 1. 定时任务配置 ⚠️

**定投计划自动执行**：
- 需要配置 Hangfire 或 Cron 定时任务
- 定时检查需要执行的定投计划
- 自动执行定投操作

**价格提醒监控**：
- 定时刷新价格提醒的当前价格
- 检查是否触发提醒条件
- 发送通知（邮件/Web 通知）

**实现建议**：
1. 安装 Hangfire NuGet 包
2. 配置 Hangfire 服务
3. 创建后台任务服务
4. 设置定时任务（如：每天检查一次）

### 2. 交易记录导入 ⚠️

**功能描述**：
- 支持上传 CSV/Excel 文件
- 解析交易记录
- 自动更新投资记录

**实现建议**：
1. 使用 EPPlus 或 CsvHelper 解析文件
2. 创建文件上传接口
3. 实现数据解析和导入逻辑
4. 前端文件上传界面

### 3. 更详细的收益分析图表 ⚠️

**功能描述**：
- 按时间段展示收益（日/周/月/季度）
- 更多图表类型（K线图、面积图等）
- 收益率对比分析

**实现建议**：
1. 扩展后端统计接口，支持时间段查询
2. 使用 ECharts 实现更多图表类型
3. 前端图表组件优化

### 4. 数据导出功能 ⚠️

**功能描述**：
- 导出投资记录为 Excel/CSV
- 导出交易记录
- 导出收益分析报告

**实现建议**：
1. 使用 EPPlus 生成 Excel 文件
2. 使用 CsvHelper 生成 CSV 文件
3. 创建导出接口
4. 前端导出按钮和下载功能

## 📁 文件结构

### 后端文件
```
backend/PersonalSite.Api/
├── Models/
│   ├── DcaPlan.cs                    # 定投计划模型
│   ├── DcaExecution.cs               # 定投执行记录模型
│   └── PriceAlert.cs                 # 价格提醒模型
├── Controllers/
│   ├── DcaPlanController.cs          # 定投计划控制器
│   └── PriceAlertController.cs       # 价格提醒控制器
└── Data/
    └── AppDbContext.cs                # 已更新，添加新的 DbSet
```

### 前端文件
```
pages/admin/
├── dca-plan.vue                      # 定投计划管理页面
└── price-alert.vue                    # 价格提醒管理页面
```

### 数据库脚本
```
database/
├── dca_plan_tables.sql                # 定投计划表结构
└── price_alert_tables.sql             # 价格提醒表结构
```

## 🚀 使用说明

### 1. 数据库初始化

执行数据库脚本：

```bash
# 定投计划表
mysql -u root -p personal_site < database/dca_plan_tables.sql

# 价格提醒表
mysql -u root -p personal_site < database/price_alert_tables.sql
```

### 2. 访问页面

- **定投计划管理**：`/admin/dca-plan`
- **价格提醒管理**：`/admin/price-alert`

### 3. 创建定投计划

1. 点击"新增定投计划"按钮
2. 填写代码、名称、类型、金额、频率等信息
3. 点击"自动获取"按钮获取名称和价格（可选）
4. 保存定投计划

### 4. 创建价格提醒

1. 点击"新增提醒"按钮
2. 填写代码、名称、类型、目标价格、提醒类型等信息
3. 点击"自动获取"按钮获取名称和当前价格（可选）
4. 保存价格提醒

### 5. 手动执行定投

1. 在定投计划列表中，点击"立即执行"按钮
2. 系统会自动获取当前价格并执行定投
3. 执行结果会更新投资记录

### 6. 刷新价格提醒

1. 在价格提醒页面，点击"刷新价格"按钮
2. 系统会刷新所有启用的价格提醒的当前价格
3. 如果价格达到目标，会自动标记为已触发

## 🔧 技术细节

### API 接口

#### 定投计划 API

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/DcaPlan` | 获取定投计划列表 |
| GET | `/api/DcaPlan/{id}` | 获取定投计划详情 |
| POST | `/api/DcaPlan` | 创建定投计划 |
| PUT | `/api/DcaPlan/{id}` | 更新定投计划 |
| DELETE | `/api/DcaPlan/{id}` | 删除定投计划 |
| POST | `/api/DcaPlan/{id}/execute` | 手动执行定投 |
| GET | `/api/DcaPlan/{id}/executions` | 获取执行记录 |

#### 价格提醒 API

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/PriceAlert` | 获取价格提醒列表 |
| GET | `/api/PriceAlert/{id}` | 获取价格提醒详情 |
| POST | `/api/PriceAlert` | 创建价格提醒 |
| PUT | `/api/PriceAlert/{id}` | 更新价格提醒 |
| DELETE | `/api/PriceAlert/{id}` | 删除价格提醒 |
| POST | `/api/PriceAlert/refresh-prices` | 刷新所有提醒的价格 |

### 数据模型

#### DcaPlan（定投计划）
- `Id`: 主键
- `Code`: 股票/基金代码
- `Name`: 名称
- `Type`: 类型（stock/fund）
- `Amount`: 每次定投金额
- `Frequency`: 频率（daily/weekly/monthly/quarterly）
- `NextExecutionDate`: 下次执行日期
- `LastExecutionDate`: 上次执行日期
- `TotalExecutions`: 总执行次数
- `TotalInvested`: 累计投入金额
- `IsActive`: 是否启用
- `StartDate`: 开始日期
- `EndDate`: 结束日期（可选）
- `Notes`: 备注

#### PriceAlert（价格提醒）
- `Id`: 主键
- `Code`: 股票/基金代码
- `Name`: 名称
- `Type`: 类型（stock/fund）
- `TargetPrice`: 目标价格
- `AlertType`: 提醒类型（above/below/both）
- `CurrentPrice`: 当前价格
- `IsTriggered`: 是否已触发
- `TriggeredAt`: 触发时间
- `IsActive`: 是否启用
- `NotificationSent`: 是否已发送通知
- `Notes`: 备注

## 📝 注意事项

1. **定时任务**：目前定投计划和价格提醒需要手动执行，定时任务功能待实现
2. **通知功能**：价格提醒触发后，通知功能待实现（邮件/Web 通知）
3. **场外基金**：部分场外基金可能无法自动获取价格，需要手动输入
4. **API 限制**：东方财富 API 有频率限制，建议不要过于频繁刷新

## 🔮 未来扩展

1. **定时任务集成**：使用 Hangfire 实现自动执行
2. **通知系统**：集成邮件或 Web 推送通知
3. **交易记录导入**：支持 CSV/Excel 文件导入
4. **收益分析增强**：更多图表类型和时间段分析
5. **数据导出**：支持导出 Excel/CSV 格式

---

**维护者**：开发团队  
**相关文档**：
- [投资管理模块](./INVESTMENT_MODULE.md)
- [投资数据来源说明](./INVESTMENT_DATA_SOURCE.md)
- [东方财富 API 使用指南](./EASTMONEY_API_GUIDE.md)
