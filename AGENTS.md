# AI 任务入口手册

AI 在本项目执行任务时，只需要先看这一页，再按类型读取少量核心手册。

## 核心原则

1. 先判断任务类型
2. 先读对应手册
3. 再改代码
4. 改完后只在必要时更新文档

## 任务类型与必读文档

| 任务类型 | 必读文档 |
| --- | --- |
| 项目总览 | `docs/PROJECT_OVERVIEW.md` |
| 定位代码位置 | `docs/PROJECT_STRUCTURE_GUIDE.md` |
| 页面、组件、前端逻辑 | `docs/development/DEVELOPMENT_GUIDELINES.md` |
| 样式、主题、设计系统 | `docs/development/STYLE_ARCHITECTURE.md`、`docs/development/STYLE_MANAGEMENT_REMINDER.md`、`docs/design-system/README.md` |
| 模块功能 | `docs/architecture/README_MODULES.md`、`docs/development/MODULE_DEVELOPMENT_GUIDE.md`、`docs/modules/` |
| API、联调、配置 | `docs/config/API_CONFIG.md`、`docs/api/MODULE_SYSTEM_API.md` |
| 部署、启动、环境 | `docs/deployment/README.md`、`docs/deployment/QUICK_START.md` |
| 低频历史资料 | `docs/archive/` |

## 允许直接修改的前提

只有同时满足下面条件，才直接开始改：

- 已判断任务类型
- 已阅读对应核心手册
- 已定位实际代码落点

如果涉及样式、模块、部署、数据修复，不允许跳过手册直接修改。

## 文档规则

新增文档时：

- 优先放到已有核心分类目录
- 一次性阶段记录优先放 `docs/archive/`
- 不要再创建新的文档索引体系

## 保留的文档层级

当前只保留 3 层：

1. `AGENTS.md`
2. `docs/PROJECT_OVERVIEW.md` 与 `docs/PROJECT_STRUCTURE_GUIDE.md`
3. 少量核心专题手册

其余低频、阶段性、历史性材料统一放在 `docs/archive/legacy/`

## 一句话原则

AI 在本项目里必须“先读最少必要文档，再执行任务”。
