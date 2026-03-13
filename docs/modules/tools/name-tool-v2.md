# 智能取名助手（V2）使用说明

## 功能概述

智能取名助手是一个基于 AI 的名字生成工具，支持生成游戏名、网名、英文名，并提供评分、收藏、复制等功能。

## 访问地址

- 前台页面：`/tools/name`

## 功能特性

### 1. 名字生成
- 支持三种类型：游戏名、网名、英文名
- 支持多种风格：霸气、可爱、文艺、搞笑、克制、科幻、二次元、古风、赛博
- 可设置性别、名字长度、关键词、语言、禁用词等条件
- 每次生成 20 个名字

### 2. 名字评分
每个名字提供四项评分（0-100）：
- **好记度**（memorability）：名字是否容易记忆
- **独特性**（uniqueness）：名字的独特程度
- **贴合度**（fit）：名字是否符合用户设定的条件
- **美观度**（aesthetics）：名字的美观程度
- **总分**：综合评分

### 3. 再来一批
- 保留用户输入条件
- 使用 traceId 保持风格一致性
- 生成同风格但不同的名字

### 4. 收藏功能
- 支持收藏喜欢的名字
- 收藏列表持久化到数据库
- 当前支持匿名收藏（后续可扩展用户体系）

### 5. 复制功能
- 一键复制名字到剪贴板
- 复制成功后显示提示

## 技术实现

### 前端
- **页面**：`pages/tools/name.vue`
- **Composable**：`composables/useNameTool.ts`
- **类型定义**：`types/name-tool.ts`
- **技术栈**：Nuxt3 + Vue3 + TypeScript + Naive UI

### 后端
- **Controller**：`NameToolController.cs`
- **Service**：`NameToolAiService.cs`
- **Model**：`NameFavorite.cs`
- **DTO**：`NameToolDto.cs`
- **技术栈**：.NET 8 WebAPI + EF Core + MySQL

### AI 服务
- 通过 `AiServiceClient` 调用 Python AI 服务
- 使用结构化 Prompt 确保 JSON 输出
- 支持去重、补生成、禁用词过滤

## API 接口

### 1. 生成名字
```
POST /api/Name/generate
```

请求体：
```json
{
  "type": "game",
  "style": ["霸气", "古风"],
  "gender": "男",
  "length": "中",
  "keywords": "剑,江湖",
  "language": "中文",
  "banned": "禁用词1,禁用词2"
}
```

响应：
```json
{
  "code": 0,
  "message": "ok",
  "data": {
    "traceId": "xxx",
    "items": [
      {
        "name": "名字",
        "totalScore": 85,
        "scores": {
          "memorability": 80,
          "uniqueness": 90,
          "fit": 85,
          "aesthetics": 85
        },
        "reason": "评分理由",
        "tags": ["古风", "短", "偏霸气"]
      }
    ]
  }
}
```

### 2. 再来一批
```
POST /api/Name/regenerate
```

请求体同生成接口，但必须包含 `traceId`。

### 3. 获取收藏列表
```
GET /api/Name/favorites?page=1&pageSize=20
```

### 4. 收藏名字
```
POST /api/Name/favorites
```

### 5. 取消收藏
```
DELETE /api/Name/favorites/{id}
```

## 数据库

### 表结构
- **name_favorite**：名字收藏表

### 迁移脚本
执行 `database/name_tool_tables.sql` 创建表结构。

## 配置说明

### AI 服务配置
确保 `appsettings.json` 中配置了 AI 服务：
```json
{
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "InternalToken": "your-token"
  }
}
```

### 数据库配置
确保数据库连接字符串正确配置。

## 使用流程

1. **访问页面**：打开 `/tools/name`
2. **设置条件**：选择类型、风格等筛选条件
3. **生成名字**：点击"生成"按钮
4. **查看结果**：查看生成的名字列表和评分
5. **排序筛选**：可按不同维度排序
6. **收藏名字**：点击"收藏"按钮保存喜欢的名字
7. **复制名字**：点击"复制"按钮复制到剪贴板
8. **再来一批**：点击"再来一批"生成更多同风格名字

## 注意事项

1. **AI 服务**：需要确保 Python AI 服务正常运行
2. **数据库**：需要先执行数据库迁移脚本
3. **匿名收藏**：当前实现为匿名收藏，后续可扩展用户体系
4. **去重处理**：系统会自动去重，确保名字不重复
5. **禁用词**：如果设置了禁用词，生成的名字不会包含这些词

## 后续优化

- [ ] 支持用户登录后的个人收藏
- [ ] 支持导出收藏列表
- [ ] 支持批量操作
- [ ] 优化 AI Prompt 提升生成质量
- [ ] 添加更多风格选项
- [ ] 支持自定义评分权重

