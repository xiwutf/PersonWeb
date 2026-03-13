# 支付与许可证系统 - 评估与改动说明

## 一、方案合理性评估

### ✅ 整体方案合理

`payment_license_tables.sql` 设计的表结构清晰，覆盖了模块售卖场景的核心需求：

| 表名 | 用途 | 评估 |
|------|------|------|
| `order` | 模块购买订单 | ✅ 与 `orders`（咨询订单）分离，设计合理 |
| `license` | 许可证管理 | ✅ 支持永久/订阅/试用 |
| `license_activation` | 设备激活记录 | ✅ 支持多设备限制 |
| `payment_transaction` | 支付交易流水 | ✅ 便于对账和审计 |
| `refund` | 退款记录 | ✅ 完整闭环 |
| `subscription_plan` | 订阅计划 | ✅ 支持套餐售卖 |
| `user_subscription` | 用户订阅 | ✅ 与 plan 关联 |
| `coupon` | 优惠码 | ✅ 支持营销 |

### 双系统并存说明

项目中存在两套订单相关表，**互不冲突**：

| 表 | 用途 | 后端 |
|----|------|------|
| `orders` | 咨询/定制订单（工具、咨询转化） | C# .NET |
| `order` | 模块购买订单（模块商店、许可证） | Nuxt Server |

---

## 二、已完成的改动

### 1. 修复 database query 返回值解构错误

**问题**：`query()` 返回的是行数组或 `ResultSetHeader`，使用 `const [rows] = await query()` 会错误地只取第一行。

**修复**：
- `server/services/license.ts`：所有 `query` 调用改为直接使用返回值
- `server/services/payment.ts`：同上
- `server/api/payment/orders/index.get.ts`：同上
- `server/api/payment/orders/[id]/index.get.ts`：同上

### 2. 完善支付创建流程

**问题**：`create.post.ts` 未在数据库中创建订单，直接调支付网关，且 `CreateOrderRequest` 缺少 `price`。

**修复**：
- 在 `CreateOrderRequest` 中增加必填字段 `price`
- 新增 `createOrderRecord()`，先落库再调支付
- 流程：验证参数 → 创建 `order` 记录 → 调用支付网关 → 返回支付链接

### 3. 支付成功回调自动创建许可证

**问题**：Webhook 仅更新订单状态，未创建许可证。

**修复**：在 `updateOrderStatus` 中，当 `status === 'paid'` 时：
1. 查询对应订单
2. 调用 `createLicense()` 生成许可证
3. 将 `license_key` 回写到 `order` 表

### 4. 修复 my-licenses 页面

**问题**：使用模拟数据，未调用真实 API。

**修复**：
- 使用 `useFetch('/api/license/my-licenses')` 获取数据
- 激活接口改为调用 `/api/license/activate`

### 5. 许可证 API 增强

- `getUserLicenses`：关联 `order` 表，返回 `version` 字段
- `activateLicense`：支持传入 `event`，记录 IP 和 User-Agent
- `License` 类型：增加可选字段 `version`

---

## 三、SQL 表结构建议

### 当前表结构无需修改

`payment_license_tables.sql` 已执行且结构合理，无需变更。

### 可选优化（非必须）

1. **user_subscription 唯一约束**

   ```sql
   UNIQUE KEY `uk_user_plan` (`user_id`, `plan_key`, `status`)
   ```

   当前设计允许同一用户同一计划存在多条记录（如 active + expired），逻辑正确。若希望同一用户同一计划仅保留一条有效订阅，可考虑改为 `(user_id, plan_key)` 并配合应用层逻辑。

2. **order 与 license 的 user_id 一致性**

   建议在应用层保证：创建 license 时使用的 `userId` 与 `order.user_id` 一致（当前实现已满足）。

---

## 四、待完善事项

### 1. 用户 ID 获取

当前 `getUserIdFromEvent()` 和 `my-licenses.get.ts` 中 `userId` 固定为 `1`。

**建议**：接入真实认证后，从 JWT/session 解析 `userId`，并统一封装到 `server/utils/auth.ts`。

### 2. 支付网关集成

当前支付宝、微信、Stripe 均为占位实现，需：

- 集成官方 SDK
- 实现真实签名校验
- Stripe Webhook 需使用 `readRawBody` 获取原始 body 做签名验证

### 3. 优惠码逻辑

`CreateOrderRequest` 已有 `couponCode`，但创建订单和支付时尚未使用 `calculateDiscount()`，需在 `createOrderRecord` 和支付金额计算中接入优惠逻辑。

### 4. 金额单位

- 支付宝/微信：金额单位为**元**
- Stripe：金额单位为**分**（cents）

集成真实网关时，需按网关要求做单位转换。

---

## 五、数据流概览

```
用户选择模块 → 前端传 moduleKey/version/type/price
    ↓
POST /api/payment/create
    ↓
1. createOrderRecord() → 插入 order 表 (status=pending)
2. createPaymentOrder() → 调支付网关
3. 返回 paymentUrl
    ↓
用户完成支付 → 支付网关回调 Webhook
    ↓
handlePaymentWebhook() → updateOrderStatus()
    ↓
status=paid 时：createLicense() → 插入 license，回写 order.license_key
    ↓
用户可在「我的许可证」查看并激活
```

---

## 六、总结

- SQL 表结构设计合理，与现有 `orders` 系统兼容
- 已修复 query 解构、支付创建流程、许可证自动创建、前端对接等问题
- 后续重点：用户认证、真实支付网关集成、优惠码与金额单位处理
