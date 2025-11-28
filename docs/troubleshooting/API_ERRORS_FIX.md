# API 错误修复指南

## 问题描述

1. `/api/Analytics/track` 返回 500 错误
2. `/api/Module` 返回 500 错误
3. 后台管理无法使用

## 已修复的问题

### 1. AnalyticsController.Track 方法

**问题：**
- 缺少空值检查
- 缺少异常处理

**修复：**
- 添加了 `request` 参数的空值检查
- 添加了 `VisitorId` 的验证
- 添加了 try-catch 异常处理
- 修复了代码缩进问题

### 2. ModuleController 方法

**问题：**
- 缺少异常处理
- 如果数据库表不存在会直接抛出异常

**修复：**
- 为 `GetAllModules` 和 `GetEnabledModules` 添加了 try-catch
- 返回更友好的错误信息

## 数据库表检查

如果仍然出现 500 错误，可能是数据库表不存在。请执行以下 SQL 脚本：

```sql
-- 执行模块系统表创建脚本
source database/module_system_tables.sql
```

或者手动检查表是否存在：

```sql
SHOW TABLES LIKE 'module';
SHOW TABLES LIKE 'module_config';
```

如果表不存在，需要：
1. 运行 `database/module_system_tables.sql` 创建表
2. 确保数据库连接字符串正确
3. 重新启动后端服务

## 验证修复

1. **测试 Analytics API：**
   ```bash
   curl -X POST http://localhost:5234/api/Analytics/track \
     -H "Content-Type: application/json" \
     -d '{"VisitorId":"test-123","Path":"/"}'
   ```

2. **测试 Module API（需要认证）：**
   ```bash
   curl -X GET http://localhost:5234/api/Module/enabled \
     -H "Authorization: Bearer YOUR_TOKEN"
   ```

## 常见错误

### 错误 1: "Table 'xxx.module' doesn't exist"
**解决方案：** 运行数据库初始化脚本

### 错误 2: "Cannot read properties of undefined"
**解决方案：** 检查前端是否正确处理 API 响应

### 错误 3: "500 Internal Server Error"
**解决方案：** 
1. 检查后端日志
2. 确认数据库连接正常
3. 确认表结构正确

