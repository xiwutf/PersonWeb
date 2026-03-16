## 《PersonWeb Module System 完整设计方案》设计与实现任务书

你当前的任务是为 **PersonWeb** 设计并落地一套 **模块系统（Module System）**，使项目从“功能集合”升级为“平台型产品框架”。

本任务允许你进行代码设计与实现，但必须遵循：

- **先设计，后实现**
    
- **先做 MVP，再考虑扩展**
    
- **不追求一步到位的远程插件系统**
    
- **优先兼容现有 PersonWeb 项目结构**
    
- **优先保证可维护、可启停、可扩展、可商业化**
    

---

# 一、项目背景

PersonWeb 当前是一个综合型项目，包含：

- 前台网站
    
- 后台管理端
    
- AI 能力模块
    
- 知识库能力
    
- 资产管理
    
- 模块商店方向
    
- 后续可扩展为可售卖的平台模板
    

当前问题：

- 功能模块大多仍以“项目页面/目录”方式组织
    
- 缺少统一的模块声明协议
    
- 缺少模块注册、启停、权限、菜单挂载机制
    
- 缺少模块生命周期规范
    
- 缺少模块系统的数据库建模
    
- 后续无法支撑模块独立售卖、授权、升级
    

因此，需要构建：

**PersonWeb Module System**

---

# 二、总目标

建立一套适用于 PersonWeb 的模块系统，使平台具备：

1. 模块声明能力
    
2. 模块注册能力
    
3. 模块启用 / 禁用能力
    
4. 模块菜单挂载能力
    
5. 模块权限注册能力
    
6. 模块生命周期能力
    
7. 模块配置能力
    
8. 模块版本与升级预留能力
    
9. 模块商业化（许可证 / 商店）预留能力
    

---

# 三、总体原则

你必须遵循以下原则：

## 1. 先做“项目内模块化”

当前阶段不做真正的远程动态插件加载。

先实现：

- 模块代码仍在主仓库
    
- 但平台通过统一协议识别和管理模块
    
- 模块支持注册、启停、菜单、权限、配置、生命周期
    

也就是：

**编译期存在 + 运行时注册**

---

## 2. 模块系统必须兼容现有技术栈

当前项目现实约束：

- 前端：Vue3 / Nuxt3 / TypeScript / Naive UI
    
- 后端：.NET WebAPI
    
- AI 相关能力：Python 服务
    
- 已有前后台、管理页、AI页、内容页
    

因此你设计的模块系统必须：

- 不强行推翻现有目录
    
- 不引入不必要的复杂框架
    
- 允许逐步迁移现有功能为模块
    

---

## 3. 模块系统优先服务于“平台化”和“商业化”

这不是纯代码结构优化，而是产品架构升级。

最终要为以下能力打基础：

- 官方模块
    
- 业务模块
    
- AI模块
    
- 模块商店
    
- 模块授权
    
- 模块独立售卖
    

---

# 四、你要完成的交付物

你需要输出 **文档 + 数据库设计 + 代码骨架 + MVP 实现**。

---

## 第一部分：设计文档

请先输出以下文档到：

```text
docs/module-system/
```

### 文档 1

`docs/module-system/01-module-system-architecture.md`

内容必须包含：

1. 模块系统目标
    
2. 模块系统边界
    
3. 模块系统总体架构
    
4. 六层结构说明：
    
    - Module Manifest
        
    - Module Runtime
        
    - Module Registry
        
    - Module Permissions
        
    - Module Lifecycle
        
    - Module Marketplace（预留）
        
5. 模块分类建议：
    
    - Core
        
    - Platform
        
    - AI
        
    - Business
        
    - Web
        
6. 当前项目现有功能如何映射为模块
    

---

### 文档 2

`docs/module-system/02-module-specification.md`

内容必须包含：

1. 模块目录规范
    
2. `module.json` / `module.config.ts` 规范
    
3. 模块标准接口定义
    
4. 模块菜单声明规范
    
5. 模块权限声明规范
    
6. 模块设置页规范
    
7. 模块生命周期接口规范
    
8. 模块命名规范
    
9. 模块版本规范
    
10. 哪些字段必填，哪些字段可选
    

必须给出完整示例。

---

### 文档 3

`docs/module-system/03-module-mvp-roadmap.md`

内容必须包含：

1. MVP 范围
    
2. 分阶段实施路线
    
3. 哪些能力现在做
    
4. 哪些能力先预留不做
    
5. 风险点
    
6. 验收标准
    
7. 迁移策略
    
8. 预计修改范围
    

---

## 第二部分：数据库设计

请设计模块系统的数据库模型，并输出：

### 文档 4

`docs/module-system/04-database-design.md`

至少包含以下表设计：

### 核心表

1. `pw_modules`
    
    - 平台已安装模块注册表
        
2. `pw_module_permissions`
    
    - 模块权限注册表
        
3. `pw_module_menu_items`
    
    - 模块菜单注册表
        
4. `pw_module_settings`
    
    - 模块配置定义或配置值
        

### 预留表

5. `pw_module_versions`
    
    - 模块版本记录
        
6. `pw_module_licenses`
    
    - 模块许可证
        
7. `pw_module_catalog`
    
    - 模块市场商品目录
        

如你认为有必要，可补充更多表，但必须解释用途。

---

### SQL 文件

请输出 SQL 脚本：

```text
database/module_system_tables.sql
```

要求：

- 表结构完整
    
- 字段命名清晰
    
- 有索引建议
    
- 有主键、唯一键
    
- 尽量考虑后续扩展
    
- 不要过度设计
    

---

## 第三部分：代码结构设计

请基于现有 PersonWeb 项目，提出并实现模块系统目录骨架。

建议目标目录：

```text
src/
modules/
server/
docs/
database/
```

你需要设计一个适合当前项目的真实结构，而不是纸面理想结构。

请输出：

### 文档 5

`docs/module-system/05-project-structure-proposal.md`

内容包括：

1. 当前项目哪些目录建议保留
    
2. 新增哪些目录
    
3. 模块放在哪里
    
4. 前端模块如何组织
    
5. 后端模块如何组织
    
6. AI模块如何纳入统一模块系统
    
7. 如何兼容 Nuxt 页面与模块页面
    

---

## 第四部分：MVP 代码实现

在文档完成后，再开始实现 MVP。  
MVP 只实现最关键的 6 项能力：

### MVP 必做能力

1. **模块 Manifest 协议**
    
    - 支持 `module.json` 或 `module.config.ts`
        
2. **ModuleManager**
    
    - 扫描模块
        
    - 读取 manifest
        
    - 校验模块合法性
        
    - 注册模块
        
3. **模块注册表**
    
    - 支持将模块信息写入 `pw_modules`
        
4. **模块菜单注册**
    
    - 根据模块配置生成后台菜单数据
        
5. **模块权限注册**
    
    - 根据模块配置生成权限数据
        
6. **模块启用 / 禁用**
    
    - 平台可控制模块状态
        
    - 禁用后菜单不可见
        
    - 禁用后运行时不可挂载
        

---

# 五、代码实现要求

## 1. 模块标准接口

你需要定义统一的 TypeScript 接口，例如：

```ts
export interface PersonWebModuleManifest {
  key: string
  name: string
  version: string
  description?: string
  category: 'core' | 'platform' | 'ai' | 'business' | 'web'
  adminEntry?: string
  webEntry?: string
  enabledByDefault?: boolean
  dependencies?: string[]
  permissions?: string[]
  menus?: ModuleMenuItem[]
  settingsPath?: string
  hasMigration?: boolean
  licenseRequired?: boolean
}
```

还需要定义：

- `ModuleMenuItem`
    
- `ModulePermission`
    
- `PersonWebModuleLifecycle`
    
- `PersonWebModuleDefinition`
    

---

## 2. 必须实现 ModuleManager

建议实现一个模块管理器，例如：

```ts
class ModuleManager {
  loadModules()
  validateManifest()
  registerModules()
  getEnabledModules()
  getAdminMenus()
  getPermissions()
  enableModule()
  disableModule()
}
```

要求：

- 结构清晰
    
- 接口可扩展
    
- 便于未来接入数据库
    
- 便于未来接入许可证和 marketplace
    

---

## 3. 必须提供至少 2 个示例模块

请基于现有业务做两个示例模块：

### 示例模块 1

`asset-manager`

### 示例模块 2

`knowledge-base` 或 `ai-chat`

要求：

- 有 manifest
    
- 有菜单声明
    
- 有权限声明
    
- 有启用/禁用示例
    
- 能证明模块系统不是空架子
    

---

## 4. 前端菜单接入

后台菜单不要再完全写死。  
应支持：

- 基础菜单
    
- 模块菜单动态注入
    
- 可按模块启用状态控制显示
    

如果当前项目菜单系统已经存在，请以**最小侵入**方式接入，不要粗暴推翻。

---

## 5. 权限接入

如果当前项目已有权限体系，请做兼容设计；如果权限体系不完整，也至少完成：

- 模块声明权限
    
- 模块权限注册表
    
- 菜单与权限字段关联的基础结构
    

---

## 6. 生命周期先做接口，不必全量做复杂逻辑

MVP 阶段只要求：

- 预留 install / enable / disable / upgrade / uninstall 接口
    
- 对 enable / disable 实现最小闭环
    
- 其他生命周期可先空实现，但接口必须完整
    

---

# 六、模块目录规范要求

请设计统一模块目录协议，建议参考但不要机械照搬：

```text
modules/
├─ asset-manager/
│  ├─ module.json
│  ├─ index.ts
│  ├─ admin/
│  │  ├─ pages/
│  │  ├─ components/
│  │  └─ routes.ts
│  ├─ web/
│  │  ├─ pages/
│  │  └─ routes.ts
│  ├─ server/
│  │  ├─ api/
│  │  ├─ services/
│  │  └─ permissions.ts
│  ├─ database/
│  │  ├─ migrations/
│  │  └─ seeds/
│  ├─ settings/
│  │  └─ schema.ts
│  └─ README.md
```

但最终必须结合 PersonWeb 当前真实项目结构来决定，不要为了“看起来完整”而过度抽象。

---

# 七、现有功能模块化映射要求

请先分析当前项目中已有能力，给出模块映射建议。

至少要覆盖这些方向：

- asset-manager
    
- ai-chat / ai-platform
    
- knowledge-base
    
- module-store
    
- content-center / blog
    
- analytics
    
- settings
    
- user/auth（如适合则标为 core）
    

你需要在文档中明确：

1. 哪些属于 Core Module
    
2. 哪些属于 Platform Module
    
3. 哪些属于 AI Module
    
4. 哪些属于 Business Module
    
5. 哪些属于 Web Module
    

---

# 八、实施边界

当前阶段 **不要做**：

- 真正的远程模块下载与热加载
    
- 第三方开发者完整生态
    
- 完整许可证支付闭环
    
- 完整应用市场 UI
    
- 动态运行时代码执行沙箱
    

当前阶段 **要做的是为这些能力预留架构位**。

---

# 九、输出顺序要求

必须按以下顺序推进，不允许直接跳到大改代码：

## Step 1

先输出 5 份设计文档

## Step 2

再输出数据库 SQL 设计

## Step 3

再输出代码结构提案

## Step 4

再实现 MVP 代码骨架

## Step 5

再接入 2 个示例模块

## Step 6

最后输出总结报告：

`docs/module-system/06-mvp-delivery-report.md`

内容包括：

1. 已完成内容
    
2. 未完成内容
    
3. 风险点
    
4. 下一阶段建议
    
5. 如何继续演进到：
    
    - 模块版本管理
        
    - 许可证系统
        
    - 模块商店
        
    - 商业化售卖
        

---

# 十、代码质量要求

1. 所有代码必须使用 TypeScript
    
2. 中文注释
    
3. 命名统一、语义清晰
    
4. 不允许写大量临时 hack
    
5. 尽量保持最小侵入式改造
    
6. 如需改现有文件，必须说明原因
    
7. 新建的目录和文件要有明确职责
    
8. 优先写清楚接口和边界，而不是堆实现细节
    

---

# 十一、验收标准

本轮验收通过的最低标准：

## 文档层

- 5 份设计文档完整
    
- 数据库文档完整
    
- SQL 脚本可读、可执行
    

## 架构层

- 模块协议清晰
    
- 模块分类合理
    
- 模块目录规范清晰
    
- 模块生命周期接口完整
    

## 代码层

- 有 `ModuleManager`
    
- 有 manifest 读取与校验
    
- 有模块注册逻辑
    
- 有模块菜单注入能力
    
- 有模块权限注入能力
    
- 有模块启用 / 禁用能力
    
- 至少 2 个示例模块可运行或可演示
    

## 工程层

- 不破坏现有主流程
    
- 尽量兼容当前后台菜单与页面结构
    
- 代码结构可继续扩展
    

---

# 十二、你完成后需要额外回答的问题

在最终交付报告中，请额外回答：

1. 当前 PersonWeb 最适合先模块化的功能有哪些
    
2. 哪些现有页面/功能不建议立即模块化
    
3. 当前模块系统距离“可售卖模块平台”还差哪几步
    
4. 如果下一阶段做模块商店，最小闭环应该是什么
    
5. 如果下一阶段做许可证系统，核心数据模型是什么
    

---

# 十三、工作方式要求

你不需要等待进一步确认。  
请直接开始，但必须严格遵循：

**先文档，后实现；先 MVP，后扩展。**

如果在实现中发现现有项目结构与原设想不一致：

- 不要硬套理想结构
    
- 请基于真实项目调整方案
    
- 但必须在文档中说明差异和取舍
    

---

你可以直接把上面整段发给 Claude Code。

如果你要，我下一步可以继续给你补一份：

**《Claude Code 分阶段执行清单（超细版）》**

那份会把它拆成：

- 第一轮只出文档
    
- 第二轮只做数据库和骨架
    
- 第三轮再接示例模块
    

这样你更容易控节奏。