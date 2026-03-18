# 商业化功能规划

本文档规划了未来可收费的商业化功能，旨在打造一个可持续的SaaS服务模式。

## 📋 功能列表

### 1. 主题商店（Themes Store）🎨

#### 功能描述
- **多风格切换**：提供多个精心设计的主题风格
- **主题市场**：公开的主题商店，用户可以浏览和购买
- **免费/付费模式**：部分主题免费，高级主题付费
- **一键应用**：用户购买后可以一键应用主题

#### 商业模式
- **免费主题**：基础主题，吸引用户
- **付费主题**：高级主题，价格 9.9 - 99.9 元
- **订阅模式**：月度/年度订阅，解锁所有主题
- **定制主题**：为特定用户定制专属主题（高价服务）

#### 技术实现
- **主题系统**：基于现有的主题切换系统扩展
- **支付集成**：微信支付、支付宝、Stripe
- **授权管理**：用户购买记录和授权验证
- **主题预览**：在线预览功能

#### 数据库设计
```sql
-- 主题表
theme_store (
  id, name, description, preview_image,
  price, is_free, category, author_id,
  download_count, rating, status,
  created_at, updated_at
)

-- 用户购买记录
theme_purchases (
  id, user_id, theme_id, purchase_type,
  price, payment_status, purchased_at
)

-- 主题文件
theme_files (
  id, theme_id, file_type, file_path,
  version, created_at
)
```

---

### 2. API Key 管理系统 🔑

#### 功能描述
- **用户注册**：用户可以注册账户
- **API Key 生成**：自动生成和管理 API Key
- **调用统计**：实时查看 API 调用量
- **计费系统**：基于调用量或订阅模式计费
- **独立产品**：可以做成独立的 SaaS 产品

#### 商业模式
- **免费额度**：每月免费 1000 次调用
- **按量付费**：超出部分按调用次数计费
- **订阅套餐**：月度/年度订阅，包含更多调用次数
- **企业版**：定制化服务，专属支持

#### 技术实现
- **API 网关**：统一管理所有 API 请求
- **限流系统**：基于 API Key 的限流
- **监控系统**：实时监控调用量和性能
- **计费引擎**：自动计算费用和账单

#### 数据库设计
```sql
-- API 用户表
api_users (
  id, email, password_hash, name,
  plan_type, status, created_at
)

-- API Key 表
api_keys (
  id, user_id, api_key, api_secret,
  name, rate_limit, expires_at,
  is_active, created_at
)

-- API 调用记录
api_calls (
  id, api_key_id, endpoint, method,
  status_code, response_time, cost,
  called_at
)

-- 计费记录
api_billing (
  id, user_id, period, total_calls,
  free_calls, paid_calls, amount,
  status, created_at
)
```

---

### 3. 付费文章 / 会员专区 💎

#### 功能描述
- **付费阅读**：深度内容需要付费解锁
- **解锁视频**：视频内容需要会员或付费
- **AI 工具使用次数**：限制免费用户的使用次数
- **专属资源**：代码、模板等资源需要会员

#### 商业模式
- **单篇付费**：2.9 - 19.9 元/篇
- **会员订阅**：月度 29.9 元，年度 299 元
- **VIP 会员**：高级会员，解锁所有内容
- **企业会员**：团队订阅，多人共享

#### 技术实现
- **权限系统**：基于用户角色的权限控制
- **支付集成**：支持多种支付方式
- **内容加密**：付费内容加密存储
- **解锁机制**：购买后自动解锁内容

#### 数据库设计
```sql
-- 会员类型
membership_types (
  id, name, price, duration_days,
  features, is_active
)

-- 用户会员
user_memberships (
  id, user_id, membership_type_id,
  start_date, end_date, status,
  purchased_at
)

-- 付费内容
paid_content (
  id, content_type, content_id,
  price, is_member_only, unlock_requirements
)

-- 购买记录
content_purchases (
  id, user_id, content_id, price,
  purchase_type, purchased_at
)
```

---

### 4. 自助定制页面（PageBuilder SaaS）🏗️

#### 功能描述
- **个人主页创建**：让普通用户创建自己的个人主页
- **组件选择**：提供丰富的组件库
- **自定义样式**：可视化样式编辑器
- **主题模板**：使用预设的主题模板
- **轻量级 PageBuilder**：类似 Notion、Webflow 的体验

#### 商业模式
- **免费版**：基础功能，有限组件
- **专业版**：更多组件和功能，月度 39.9 元
- **企业版**：无限制，定制域名，月度 199 元
- **白标服务**：为其他平台提供 PageBuilder 服务

#### 技术实现
- **拖拽编辑器**：基于 Vue Draggable
- **组件系统**：可复用的组件库
- **模板系统**：预设模板和自定义模板
- **发布系统**：一键发布到子域名或自定义域名

#### 数据库设计
```sql
-- 用户页面
user_pages (
  id, user_id, name, slug, domain,
  template_id, status, published_at
)

-- 页面组件
page_components (
  id, page_id, component_type, position,
  config, style, created_at
)

-- 组件库
component_library (
  id, name, type, category, config_schema,
  preview_image, is_premium
)

-- 模板库
page_templates (
  id, name, category, preview_image,
  components, is_premium, price
)
```

---

## 💰 定价策略

### 主题商店
- **免费主题**：3-5 个基础主题
- **付费主题**：9.9 - 99.9 元/个
- **主题包**：199 元/年，解锁所有主题

### API 服务
- **免费版**：1000 次/月
- **基础版**：10,000 次/月，29.9 元
- **专业版**：100,000 次/月，199 元
- **企业版**：无限，定制化，面议

### 会员服务
- **月度会员**：29.9 元/月
- **年度会员**：299 元/年（省 60 元）
- **VIP 会员**：599 元/年（包含所有功能）

### PageBuilder
- **免费版**：基础功能
- **专业版**：39.9 元/月
- **企业版**：199 元/月

---

## 🛠️ 技术栈

### 后端
- **支付系统**：微信支付、支付宝、Stripe
- **权限系统**：RBAC（基于角色的访问控制）
- **API 网关**：Kong / Nginx
- **监控系统**：Prometheus + Grafana

### 前端
- **支付组件**：集成支付 SDK
- **编辑器**：Vue Draggable / SortableJS
- **权限组件**：基于角色的组件显示控制

### 数据库
- **用户数据**：MySQL（项目统一使用 MySQL，所有 SQL 语句遵循 MySQL 语法标准）
- **缓存**：Redis（API 限流、会话管理）
- **文件存储**：OSS / S3（主题文件、资源文件）

---

## 📅 实施计划

### 第一阶段（MVP）
1. ✅ API Key 管理系统基础功能
2. ✅ 付费文章基础功能
3. ✅ 主题商店基础框架

### 第二阶段（增强）
1. 完整的支付系统
2. 会员系统
3. 主题商店完整功能

### 第三阶段（高级）
1. PageBuilder SaaS
2. 企业级功能
3. 数据分析和管理后台

---

## 🔒 安全考虑

1. **API Key 安全**：加密存储，定期轮换
2. **支付安全**：使用官方 SDK，不存储敏感信息
3. **内容保护**：付费内容加密，防止盗用
4. **权限控制**：严格的权限验证

---

## 📊 数据分析

1. **用户行为**：购买行为、使用习惯
2. **收入分析**：各产品收入占比
3. **转化率**：免费到付费的转化
4. **用户留存**：会员续费率

---

**更新时间**：2024-12-XX
**状态**：规划中

