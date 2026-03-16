
项目：**PersonWeb Module System**

原则：

- 每一阶段必须可运行
    
- 每一阶段必须有文档
    
- 每一阶段必须可回滚
    
- 未完成阶段不得进入下一阶段
    

---

# 阶段 0：项目结构扫描（只读）

## 目标

让 Claude Code **先理解项目结构**，避免错误设计。

## 任务

扫描并输出：

```
项目目录结构
前端目录结构
后台页面结构
API目录
AI服务目录
数据库脚本
```

生成文档：

```
docs/module-system/00-project-structure-scan.md
```

内容必须包含：

### 1 项目顶级结构

例如：

```
apps/
pages/
components/
server/
```

### 2 后台页面列表

例如：

```
pages/admin/articles
pages/admin/projects
pages/admin/modules
pages/admin/ai
```

### 3 前台页面列表

例如：

```
pages/index
pages/about
pages/module-store
```

### 4 API 结构

例如：

```
server/api
controllers
services
```

### 5 AI 服务

例如：

```
python-ai
ai-hub
rag
```

### 6 当前功能分类

Claude Code 必须给出建议：

```
哪些功能适合变成模块
哪些暂不适合
```

---

# 阶段 1：模块系统架构设计

⚠️ 此阶段 **只写文档**

## 目标

定义 **Module System 架构和协议**

## 需要输出的文档

### 1 模块系统架构

```
docs/module-system/01-module-system-architecture.md
```

必须包含：

```
Module Manifest
Module Runtime
Module Registry
Module Permissions
Module Lifecycle
Module Marketplace（预留）
```

---

### 2 模块规范

```
docs/module-system/02-module-specification.md
```

必须定义：

```
module.json 结构
模块命名规范
模块目录规范
模块接口定义
权限声明
菜单声明
设置声明
生命周期接口
```

---

### 3 模块路线图

```
docs/module-system/03-module-mvp-roadmap.md
```

必须说明：

```
MVP做什么
暂不做什么
实施阶段
风险点
```

---

# 阶段 2：数据库模型设计

⚠️ 仍然不改代码

## 目标

设计模块系统数据库

## 输出文档

```
docs/module-system/04-database-design.md
```

必须包含表：

### 1 modules 表

```
pw_modules
```

字段示例：

```
module_key
name
version
status
installed_at
updated_at
```

---

### 2 权限表

```
pw_module_permissions
```

---

### 3 菜单表

```
pw_module_menu_items
```

---

### 4 模块配置表

```
pw_module_settings
```

---

### 5 预留表

```
pw_module_versions
pw_module_licenses
pw_module_catalog
```

---

## 同时生成 SQL

```
database/module_system_tables.sql
```

---

# 阶段 3：模块目录结构设计

⚠️ 仍然不写复杂逻辑

## 输出文档

```
docs/module-system/05-project-structure-proposal.md
```

必须说明：

### 1 新增目录

例如：

```
modules/
core/
platform/
ai/
business/
web/
```

### 2 模块结构

例如：

```
modules/asset-manager
modules/knowledge-base
modules/ai-chat
```

---

### 3 模块目录协议

示例：

```
modules/asset-manager
  module.json
  index.ts
  admin/
  web/
  server/
  database/
  settings/
```

---

# 阶段 4：ModuleManager 实现

⚠️ 从这里开始写代码

## 目标

实现 **模块运行时**

新增目录：

```
src/core/module-system
```

实现文件：

```
ModuleManager.ts
ModuleLoader.ts
ModuleTypes.ts
```

---

## 必须实现接口

```ts
loadModules()
validateManifest()
registerModules()
getEnabledModules()
getAdminMenus()
getPermissions()
enableModule()
disableModule()
```

---

## 模块 manifest 类型

```ts
interface ModuleManifest {
  key: string
  name: string
  version: string
  category: string
  permissions?: string[]
  menus?: ModuleMenuItem[]
}
```

---

# 阶段 5：模块注册系统

## 目标

模块信息写入数据库。

实现：

```
ModuleRegistryService
```

职责：

```
注册模块
更新模块
读取模块
查询模块状态
```

---

# 阶段 6：后台菜单动态注册

## 目标

模块自动生成后台菜单。

当前：

```
菜单写死
```

升级为：

```
核心菜单 + 模块菜单
```

实现：

```
ModuleMenuService
```

---

# 阶段 7：模块权限注册

## 目标

模块权限自动注册。

实现：

```
ModulePermissionService
```

支持：

```
权限声明
权限注册
权限查询
```

---

# 阶段 8：模块启停

## 目标

支持：

```
启用模块
禁用模块
```

禁用模块后：

```
菜单消失
API不可访问
功能不可用
```

---

# 阶段 9：示例模块

必须实现两个模块：

### 模块1

```
asset-manager
```

### 模块2

```
knowledge-base
```

每个模块必须有：

```
module.json
菜单
权限
设置
```

---

# 阶段 10：MVP验收

Claude Code 必须生成报告：

```
docs/module-system/06-mvp-delivery-report.md
```

必须回答：

### 1 已完成

```
模块系统核心能力
```

### 2 未完成

```
远程模块
模块市场
许可证
```

---

### 3 下一步

建议：

```
模块版本管理
模块市场
模块授权
```

---

# 最重要的执行规则

Claude Code **必须遵守以下顺序**：

```
阶段0 → 阶段1 → 阶段2 → 阶段3
→ 阶段4 → 阶段5 → 阶段6
→ 阶段7 → 阶段8 → 阶段9
→ 阶段10
```

不允许：

```
直接写大量代码
直接重构项目
跳过设计阶段
```

---

# 你会得到什么

如果 Claude Code 按这份清单执行，你的 PersonWeb 会升级为：

```
PersonWeb Engine
```

拥有：

```
Design System
Module System
Admin Platform
AI Platform
```

这已经不是普通项目。

---

如果你愿意，我可以再给你一个 **更关键的东西**：

**《PersonWeb 最小可售卖版本架构》**

那个会直接回答一个问题：

> PersonWeb 要做到什么程度，才可以开始卖钱。