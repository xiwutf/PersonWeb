# 观察期提醒机制实现文档

## 概述

观察期提醒机制是一个主动追踪和管理关系对象观察状态的系统，能够在合适条件下建议进入观察期，并在观察期过程中提供持续提醒和结束决策支持。

## 数据模型

### 新增字段

在 `relation_person` 表中添加了以下字段：

- `observation_started_at`: 观察期开始时间
- `observation_expected_end_at`: 观察期预计结束时间（默认开始后7天）
- `observation_last_reminded_at`: 观察期上次提醒时间
- `observation_reason`: 进入观察期的原因
- `observation_decision_pending`: 是否等待观察期结束决策（布尔值）

## 后端实现

### 服务：`ObservationPeriodService`

#### 核心方法

1. **`CheckObservationSuggestionAsync`**: 检查是否应该建议进入观察期
   - 触发条件：
     - 长时间未联系（14天以上）
     - 创建7天后仍未联系
     - 最近互动质量下降（负面感受增多）
     - 热度分数持续下降（已见面但热度<30）
   
2. **`StartObservationPeriodAsync`**: 开始观察期
   - 设置阶段为5（观察期）
   - 记录开始时间和预计结束时间
   - 保存进入观察期的原因

3. **`GetObservationRemindersAsync`**: 获取观察期提醒列表
   - 返回三种类型的提醒：
     - `OnGoing`: 持续观察中（每3天提醒一次）
     - `EndingSoon`: 即将结束（剩余2天或更少，每1天提醒一次）
     - `DecisionRequired`: 需要决策（观察期已结束）

4. **`MarkReminderViewedAsync`**: 标记提醒已查看

5. **`HandleObservationDecisionAsync`**: 处理观察期结束决策
   - `Continue`: 退出观察期，恢复到之前的阶段
   - `Downgrade`: 延长观察期7天
   - `End`: 标记为已结束

6. **`MarkDecisionPendingAsync`**: 标记观察期需要决策（当观察期结束时自动调用）

### API 端点

- `GET /api/relations/persons/{id}/observation/suggestion` - 检查观察期建议
- `POST /api/relations/persons/{id}/observation/start` - 开始观察期
- `GET /api/relations/observation/reminders` - 获取观察期提醒列表
- `POST /api/relations/persons/{id}/observation/reminder/viewed` - 标记提醒已查看
- `POST /api/relations/persons/{id}/observation/decision` - 处理观察期结束决策

## 前端实现

### 组件：`ObservationReminder.vue`

显示观察期提醒的卡片组件，支持三种提醒类型：

1. **持续观察中**：显示观察天数和剩余天数
2. **即将结束**：提醒用户观察期即将结束
3. **需要决策**：显示决策按钮（继续/降级/结束）

### 列表页集成

在 `pages/admin/relations/index.vue` 中：

- 页面加载时自动获取观察期提醒
- 在列表顶部显示所有需要关注的观察期提醒
- 支持关闭提醒和进行决策操作
- 决策后自动刷新列表

## 工作流程

### 1. 建议进入观察期

系统会在以下情况下建议进入观察期：
- 对象长时间未联系
- 互动质量下降
- 热度分数持续下降

（当前为被动检查，可通过API主动检查）

### 2. 观察期进行中

- 系统每3天提醒一次观察期状态
- 显示观察天数和剩余天数
- 显示进入观察期的原因

### 3. 观察期即将结束

- 当剩余时间≤2天时，每1天提醒一次
- 提醒用户准备做出决策

### 4. 观察期结束决策

观察期结束时，系统会要求用户做出决策：

- **继续**：退出观察期，恢复到之前的阶段，继续正常跟进
- **降级**：延长观察期7天，继续观察
- **结束**：标记为已结束，关系结束

## 使用说明

### 后端使用

```csharp
// 检查观察期建议
var suggestion = await _observationService.CheckObservationSuggestionAsync(userId, personId);
if (suggestion != null)
{
    // 显示建议，让用户决定是否进入观察期
}

// 开始观察期
await _observationService.StartObservationPeriodAsync(userId, personId, "长时间未联系", 7);

// 获取提醒列表
var reminders = await _observationService.GetObservationRemindersAsync(userId);

// 处理决策
await _observationService.HandleObservationDecisionAsync(userId, personId, ObservationDecision.Continue);
```

### 前端使用

```typescript
// 获取观察期提醒
const reminders = await relationsApi.getObservationReminders()

// 开始观察期
await relationsApi.startObservation(personId, {
  reason: '长时间未联系',
  durationDays: 7
})

// 处理决策
await relationsApi.handleObservationDecision(personId, {
  decision: 'Continue', // 或 'Downgrade'、'End'
  reason: '观察期结束后决定继续跟进'
})
```

## 数据库迁移

执行以下SQL脚本添加观察期字段：

```sql
-- 执行 database/relation_tables_add_observation_fields.sql
ALTER TABLE `relation_person`
ADD COLUMN `observation_started_at` DATETIME NULL COMMENT '观察期开始时间',
ADD COLUMN `observation_expected_end_at` DATETIME NULL COMMENT '观察期预计结束时间（默认开始后7天）',
ADD COLUMN `observation_last_reminded_at` DATETIME NULL COMMENT '观察期上次提醒时间',
ADD COLUMN `observation_reason` VARCHAR(500) NULL COMMENT '进入观察期的原因',
ADD COLUMN `observation_decision_pending` BOOLEAN NOT NULL DEFAULT FALSE COMMENT '是否等待观察期结束决策',
ADD INDEX `idx_observation_started_at` (`observation_started_at`),
ADD INDEX `idx_observation_decision_pending` (`observation_decision_pending`);
```

## 特性

1. **主动追踪**：系统持续追踪观察期状态，不依赖用户主动查看
2. **智能提醒**：根据观察期阶段提供不同频率和类型的提醒
3. **明确决策**：观察期结束时强制要求做出决策，避免关系状态不明确
4. **状态管理**：观察期作为独立的状态被系统追踪，有明确的开始、进行、结束流程

## 未来扩展

- 可以在PersonCard中添加"建议进入观察期"的主动入口
- 可以为观察期设计专门的聚合视图
- 可以添加观察期的历史记录和统计

