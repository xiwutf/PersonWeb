# 访客数据收集说明

## 概述

当访客访问网站时，系统会自动收集以下数据用于分析和统计。所有数据收集均符合隐私保护原则，不收集个人敏感信息。

## 自动收集的数据

### 1. 基础访问信息

#### 1.1 访客标识
- **VisitorId**: 访客唯一标识符
  - 生成方式：首次访问时在浏览器 localStorage 中生成 UUID
  - 存储位置：`localStorage.getItem('visitor_id')`
  - 用途：追踪同一访客的多次访问

#### 1.2 IP 地址
- **IP**: 访客的 IP 地址
  - 获取方式：从 HTTP 请求头自动获取
  - 格式：IPv4 或 IPv6
  - 用途：地理位置解析、访问统计
  - **注意**：内网 IP（127.0.0.1, 192.168.x.x 等）不会进行地理位置解析

#### 1.3 访问路径
- **Path**: 当前访问的页面路径
  - 包含：路径名 + 查询参数
  - 示例：`/blog/article-1?page=2`
  - 用途：统计热门页面、访问路径分析

#### 1.4 访问时间
- **Timestamp**: 访问时间戳
- **SessionStart**: 会话开始时间
- **UpdatedAt**: 最后活跃时间
- **CreatedAt**: 首次访问时间

### 2. 设备信息（从 User-Agent 解析）

#### 2.1 设备类型
- **DeviceType**: 设备类型
  - 可能值：
    - `desktop` - 桌面设备
    - `mobile` - 移动设备
    - `tablet` - 平板设备
  - 解析规则：
    - 包含 "Mobile" 或 "Android" → mobile
    - 包含 "Tablet" 或 "iPad" → tablet
    - 其他 → desktop

#### 2.2 浏览器信息
- **Browser**: 浏览器类型
  - 可能值：
    - `Chrome` - Google Chrome
    - `Firefox` - Mozilla Firefox
    - `Safari` - Apple Safari
    - `Edge` - Microsoft Edge
    - `unknown` - 其他或无法识别
  - 解析规则：从 User-Agent 字符串中匹配浏览器标识

#### 2.3 操作系统信息
- **Os**: 操作系统类型
  - 可能值：
    - `Windows` - Windows 系统
    - `macOS` - macOS 系统
    - `Linux` - Linux 系统
    - `Android` - Android 系统
    - `iOS` - iOS 系统
    - `unknown` - 其他或无法识别
  - 解析规则：从 User-Agent 字符串中匹配操作系统标识

#### 2.4 User-Agent 原始字符串
- **UserAgent**: 完整的 User-Agent 字符串
  - 存储位置：`VisitLogs` 表
  - 用途：详细设备信息分析、问题排查

### 3. 地理位置信息（通过 IP 解析）

#### 3.1 国家/地区
- **Country**: 国家名称
  - 数据来源：ip-api.com 免费 API
  - 示例：`China`, `United States`
  - **注意**：内网 IP 无法获取地理位置信息

#### 3.2 省份/州
- **Region**: 省份或州名称
  - 数据来源：ip-api.com 免费 API
  - 示例：`Guangdong`, `California`
  - **注意**：精度取决于 IP 地理位置数据库

#### 3.3 城市
- **City**: 城市名称
  - 数据来源：ip-api.com 免费 API
  - 示例：`Shenzhen`, `New York`
  - **注意**：精度取决于 IP 地理位置数据库，可能不够精确

### 4. 访问来源信息

#### 4.1 来源页面
- **Referrer**: HTTP Referer 头
  - 获取方式：从 HTTP 请求头 `Referer` 自动获取
  - 用途：了解访客从哪个页面跳转而来
  - 可能值：
    - 外部网站 URL（如：`https://www.google.com`）
    - 内部页面路径（如：`/blog`）
    - `null`（直接访问或隐私模式）

#### 4.2 搜索关键词
- **SearchKeyword**: 搜索关键词
  - 获取方式：从 URL 查询参数中提取
  - 支持的参数名：`q`, `keyword`
  - 示例：URL 为 `/search?q=vue3` → 搜索关键词为 `vue3`
  - 用途：了解访客的搜索意图

### 5. 访问行为数据

#### 5.1 页面浏览量
- **PageViews**: 页面浏览量
  - 统计方式：同一会话内的页面访问次数
  - 更新频率：每次页面访问时递增

#### 5.2 在线状态
- **IsOnline**: 是否在线
  - 判断标准：最近 5 分钟内有访问活动
  - 更新频率：每次访问时更新
  - 用途：实时在线人数统计

#### 5.3 会话信息
- **SessionStart**: 会话开始时间
- **SessionEnd**: 会话结束时间（可选）
- **会话持续时间**：可通过 `SessionEnd - SessionStart` 计算

### 6. 数据存储位置

#### 6.1 VisitLogs 表（基础访问日志）
存储字段：
- `id` - 记录ID
- `visitor_id` - 访客ID
- `ip` - IP地址
- `user_agent` - User-Agent字符串
- `path` - 访问路径
- `timestamp` - 访问时间

#### 6.2 VisitorAnalytics 表（详细分析数据）
存储字段：
- `id` - 记录ID
- `visitor_id` - 访客ID
- `ip` - IP地址
- `country` - 国家
- `region` - 省份/州
- `city` - 城市
- `referrer` - 来源页面
- `search_keyword` - 搜索关键词
- `device_type` - 设备类型
- `browser` - 浏览器
- `os` - 操作系统
- `path` - 当前页面路径
- `session_start` - 会话开始时间
- `session_end` - 会话结束时间
- `page_views` - 页面浏览量
- `is_online` - 是否在线
- `created_at` - 创建时间
- `updated_at` - 更新时间

## 数据收集时机

### 自动触发
1. **页面加载时**：延迟 1 秒后自动发送追踪请求
2. **路由切换时**：页面切换完成后自动发送
3. **定期更新**：每 2 分钟自动更新在线状态

### 手动触发
- 通过 `/api/Analytics/track` 接口手动发送

## 数据收集方式

### 前端发送
```javascript
// plugins/analytics.client.ts
{
  visitorId: "uuid-from-localStorage",
  path: "/current/path?query=params",
  searchKeyword: "keyword-from-url" // 可选
}
```

### 后端自动获取
- **IP 地址**：`HttpContext.Connection.RemoteIpAddress`
- **User-Agent**：`Request.Headers["User-Agent"]`
- **Referer**：`Request.Headers["Referer"]`

### 第三方服务
- **地理位置**：ip-api.com（免费服务）
  - API：`http://ip-api.com/json/{ip}?fields=status,country,regionName,city`
  - 超时时间：3 秒
  - 失败处理：静默失败，不影响主流程

## 隐私保护说明

### 不收集的数据
- ❌ 个人姓名
- ❌ 邮箱地址
- ❌ 电话号码
- ❌ 身份证号
- ❌ 银行卡信息
- ❌ Cookie 中的个人信息
- ❌ 浏览器存储的其他敏感信息

### 数据用途
- ✅ 网站访问统计
- ✅ 热门内容分析
- ✅ 用户行为分析
- ✅ 设备兼容性分析
- ✅ 地理位置分布统计

### 数据存储
- 所有数据存储在服务器数据库中
- 访客ID存储在浏览器 localStorage（可清除）
- 不向第三方共享个人数据

## 数据准确性说明

### 高准确性
- ✅ 访问时间
- ✅ 访问路径
- ✅ 设备类型（基于 User-Agent）
- ✅ 浏览器类型（基于 User-Agent）
- ✅ 操作系统（基于 User-Agent）

### 中等准确性
- ⚠️ 地理位置（IP 解析，可能不够精确）
- ⚠️ 城市信息（取决于 IP 地理位置数据库）

### 可能不准确
- ⚠️ 来源页面（可能被浏览器或扩展程序阻止）
- ⚠️ 搜索关键词（仅当 URL 包含搜索参数时）

## 技术限制

### 无法获取的数据
1. **真实姓名**：除非用户主动填写
2. **精确地理位置**：IP 解析只能到城市级别，且可能不准确
3. **设备品牌/型号**：只能识别设备类型，无法识别具体型号
4. **浏览器版本**：只能识别浏览器类型，无法识别具体版本
5. **屏幕分辨率**：未收集
6. **网络类型**：未收集

### 可能被阻止的数据
1. **Referer**：某些浏览器或扩展程序可能阻止发送
2. **User-Agent**：某些浏览器可能修改或隐藏
3. **IP 地址**：使用代理或 VPN 时可能不准确

## 数据更新频率

- **实时更新**：页面访问、路由切换
- **定期更新**：在线状态（每 2 分钟）
- **延迟更新**：地理位置信息（IP 变化时）

## 管理员可查看的数据

在后台管理系统的"访客分析"页面，管理员可以查看：

1. **统计概览**
   - 今日 PV/UV
   - 昨日 PV/UV
   - 在线人数

2. **数据分布图表**
   - 访问区域分布（饼状图）
   - 设备类型分布（饼状图）
   - 浏览器分布（饼状图）
   - 操作系统分布（饼状图）

3. **详细统计表格**
   - 访问区域统计表
   - 设备类型统计表
   - 浏览器统计表
   - 操作系统统计表
   - 热门文章列表
   - 搜索来源列表

4. **访客列表**
   - 访客ID（部分显示）
   - IP地址
   - 地理位置（国家/省份/城市）
   - 设备信息（设备类型/浏览器/操作系统）
   - 当前页面
   - 浏览量
   - 最后活跃时间
   - 在线状态

## 总结

系统收集的访客数据主要用于：
- 📊 网站访问统计分析
- 🔍 用户行为分析
- 📱 设备兼容性分析
- 🌍 地理位置分布统计
- 📈 内容热度分析

所有数据收集均遵循隐私保护原则，不涉及个人敏感信息，且数据仅用于网站运营分析，不会向第三方共享。

