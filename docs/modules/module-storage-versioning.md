# 模块存储和版本管理

本文档详细说明了模块系统的存储机制和版本控制功能。

## 概述

模块存储和版本管理系统提供了完整的模块生命周期管理，包括：
- 模块包的上传和存储
- 版本管理和语义化版本控制
- 下载统计和追踪
- 用户评价和评分系统
- 兼容性管理

## 功能特性

### 1. 模块存储

#### 文件存储
- **格式**：ZIP 压缩包（最大 50MB）
- **位置**：`/uploads/modules/{moduleKey}/{version}/`
- **结构**：保持模块的源码结构
- **安全性**：自动扫描和校验

#### 包结构规范
```
module-package.zip
├── src/
│   ├── index.ts          # 模块入口文件
│   ├── module.json       # 模块配置
│   └── ...
├── components/           # 组件目录
├── composables/          # 组合式函数目录
├── types/               # 类型定义目录
├── styles/              # 样式文件目录
└── README.md            # 说明文档
```

### 2. 版本管理

#### 语义化版本控制
遵循 [Semantic Versioning](https://semver.org/) 规范：
- **主版本号**：不兼容的 API 修改
- **次版本号**：向下兼容的功能性新增
- **修订号**：向下兼容的问题修正

#### 版本状态
- **稳定版（Stable）**：推荐使用的版本
- **测试版（Beta）**：预发布版本，可能包含问题
- **最新版（Latest）**：标记为最新发布的版本

#### 版本操作
- **上传新版本**：创建新的版本记录
- **设为最新版**：将指定版本标记为最新
- **版本对比**：查看不同版本间的差异
- **回滚版本**：恢复到指定版本

### 3. 下载管理

#### 下载追踪
- 记录每次下载的详细信息
- 统计下载次数和趋势
- 支持按时间段统计

#### 下载统计
- **总下载量**：所有版本的总下载次数
- **版本下载量**：每个版本的独立统计
- **时间分布**：按日/周/月统计下载趋势

### 4. 评价系统

#### 用户评价
- **评分**：1-5 星评分
- **标题**：评价的简短描述
- **内容**：详细的使用体验
- **验证标记**：标识已验证用户的评价

#### 评分统计
- **平均评分**：所有评价的平均值
- **评分分布**：各星级的评价数量
- **总评价数**：用户评价的总数

### 5. 兼容性管理

#### 兼容性信息
- **Nuxt.js 版本**：支持的 Nuxt.js 版本范围
- **Node.js 版本**：最低 Node.js 版本要求
- **浏览器兼容性**：支持的主要浏览器及版本
- **依赖项**：模块所需的依赖及其版本

## API 文档

### 版本管理

#### 获取模块版本列表
```http
GET /api/modules/{moduleKey}/versions
```

**参数**：
- `stable`: 仅显示稳定版本（true/false）
- `latest`: 仅显示最新版本（true/false）
- `page`: 页码（默认1）
- `pageSize`: 每页数量（默认10）

**响应**：
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "moduleKey": "ai-assistant",
      "version": "1.2.0",
      "packageUrl": "/uploads/modules/ai-assistant/v1.2.0.zip",
      "packageSize": 1048576,
      "checksum": "sha256:abc123...",
      "changelog": "新增支持Claude模型...",
      "isLatest": true,
      "isStable": true,
      "publishedAt": "2024-01-15T10:00:00Z",
      "createdBy": "admin"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "total": 5,
    "totalPages": 1
  }
}
```

#### 获取版本详情
```http
GET /api/modules/{moduleKey}/versions/{version}
```

#### 设置为最新版本
```http
POST /api/modules/{moduleKey}/versions/{version}/set-latest
```

### 文件上传

#### 上传模块版本
```http
POST /api/modules/uploads
```

**请求体**：
- `moduleKey`: 模块标识（必填）
- `version`: 版本号（必填）
- `file`: 模块包文件（必填）
- `changelog`: 变更日志（必填）
- `isStable`: 是否稳定版（默认true）

#### 下载模块版本
```http
GET /api/modules/{moduleKey}/versions/{version}/download
```

### 评价管理

#### 获取评价列表
```http
GET /api/modules/{moduleKey}/ratings?version={version}
```

**参数**：
- `version`: 版本号（可选）
- `page`: 页码（默认1）
- `pageSize`: 每页数量（默认10）

#### 提交评价
```http
POST /api/modules/{moduleKey}/ratings
```

**请求体**：
```json
{
  "version": "1.2.0",
  "rating": 5,
  "title": "非常棒的模块",
  "content": "使用体验很好，功能强大..."
}
```

## 最佳实践

### 1. 版本规划
- **语义化版本**：严格遵循 semver 规范
- **变更日志**：详细记录每个版本的变更
- **版本发布**：先发布测试版，稳定后再标记为稳定版

### 2. 文件管理
- **包大小优化**：保持模块包尽可能小
- **目录结构**：遵循推荐的目录结构
- **文件命名**：使用清晰的文件命名规范

### 3. 评价处理
- **及时回复**：及时回应用户评价
- **问题修复**：根据用户反馈修复问题
- **版本迭代**：根据用户建议持续改进

### 4. 安全考虑
- **文件校验**：上传前验证文件完整性
- **内容扫描**：自动扫描恶意代码
- **权限控制**：限制上传和管理权限

## 故障排查

### 常见问题

#### 1. 上传失败
- **文件格式错误**：确保上传 ZIP 格式
- **文件过大**：压缩文件或删除无用文件
- **版本重复**：检查版本号是否已存在

#### 2. 下载问题
- **链接失效**：检查文件是否已正确上传
- **权限错误**：确保有下载权限
- **网络问题**：检查网络连接

#### 3. 版本管理
- **无法设为最新版**：检查是否有更高版本
- **版本冲突**：确保版本号唯一
- **回滚失败**：检查目标版本是否存在

### 调试工具

1. **日志查看**：
   ```bash
   # 查看上传日志
   tail -f /var/log/module-upload.log

   # 查看下载统计
   grep "download" /var/log/modules.log
   ```

2. **文件检查**：
   ```bash
   # 检查文件完整性
   sha256sum /uploads/modules/{moduleKey}/{version}/module.zip

   # 检查目录结构
   ls -la /uploads/modules/{moduleKey}/{version}/
   ```

3. **数据库查询**：
   ```sql
   -- 查看版本记录
   SELECT * FROM module_version WHERE module_key = 'module-key';

   -- 查看下载统计
   SELECT * FROM module_download
   WHERE module_key = 'module-key'
   ORDER BY downloaded_at DESC;
   ```

## 性能优化

### 1. 存储优化
- **CDN 加速**：使用 CDN 加速文件下载
- **压缩存储**：压缩历史版本文件
- **清理策略**：定期清理旧版本

### 2. 数据库优化
- **索引优化**：确保常用查询字段有索引
- **分区表**：按时间分区下载统计表
- **缓存策略**：缓存版本信息和评价数据

### 3. 网络优化
- **断点续传**：支持大文件的断点续传
- **并发限制**：限制上传和下载并发数
- **流量控制**：设置下载限流规则

---

*本文档持续更新中，如有问题请联系开发团队。*