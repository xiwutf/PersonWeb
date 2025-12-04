# 文档组织说明

**最后更新**：2024-12-03

## 📋 文档整理原则

### 1. 统一位置

**所有项目文档统一放在 `docs/` 目录下**，按功能分类组织。

### 2. 分类标准

文档按以下标准分类：

- **development/** - 开发相关（规范、样式、构建等）
- **architecture/** - 架构相关（系统架构、模块系统等）
- **config/** - 配置相关（API、环境、UI组件库等）
- **deployment/** - 部署相关（环境配置、启动指南等）
- **features/** - 功能开发相关（功能状态、开发计划等）
- **improvements/** - 改进相关（改进计划、优化记录等）
- **troubleshooting/** - 故障排除相关（问题排查、Bug修复等）
- **quality/** - 代码质量相关（规范检查、质量报告等）
- **fixes/** - 修复记录相关（数据修复、问题修复等）

### 3. 根目录文档

根目录 `docs/` 下只保留：
- 文档索引和指南（README.md, DOCUMENTATION_INDEX.md 等）
- 项目概览（PROJECT_OVERVIEW.md）
- 文档整理相关（DOCUMENTATION_*.md）

### 4. 特殊位置

以下文档保留在特殊位置：
- `public/images/README.md` - 图片说明（用户可见）
- `public/images/wechat-qr.md` - 微信二维码说明（用户可见）
- `database/*.md` - 数据库相关文档（保留在 database 目录）
- `ai-service/*.md` - AI 服务相关文档（保留在 ai-service 目录）
- `backend/*.md` - 后端相关文档（保留在 backend 目录）

## 📁 文档目录结构

```
docs/
├── README.md                      # 文档目录索引
├── DOCUMENTATION_INDEX.md         # 完整文档索引
├── DOCUMENTATION_GUIDE.md         # 文档使用指南
├── DOCUMENTATION_SUMMARY.md       # 文档整理总结
├── DOCUMENTATION_STATUS.md        # 文档整理状态报告
├── DOCUMENTATION_CLEANUP.md       # 文档整理清理报告
├── DOCUMENTATION_ORGANIZATION.md  # 文档组织说明（本文件）
├── PROJECT_OVERVIEW.md            # 项目概览 ⭐
├── development/                    # 开发文档（6个）
├── architecture/                   # 架构文档（2个）
├── config/                         # 配置文档（5个）
├── deployment/                     # 部署文档（10个）
├── features/                       # 功能开发文档（18个）
├── improvements/                   # 改进文档（5个）
├── troubleshooting/                # 故障排除文档（4个）
├── quality/                        # 代码质量文档（2个）
└── fixes/                          # 修复文档（2个）
```

## 🔄 文档移动记录

### 2024-12-03 整理

**从根目录移动**：
- `QUICK_START.md` → 合并到 `docs/deployment/QUICK_START.md`
- `STYLE_REMINDER.md` → `docs/development/STYLE_REMINDER.md`
- `task-homepage-redesign.md` → `docs/features/task-homepage-redesign.md`
- `THEME_REFACTORING_SUMMARY.md` → `docs/improvements/THEME_REFACTORING_SUMMARY.md`
- `访客分析页面进度报告.md` → `docs/features/visitor-analytics-progress.md`

**从 public 目录移动**：
- `public/内容更新指南.md` → `docs/deployment/content-update-guide.md`

**保留在原位置**：
- `public/images/README.md` - 用户可见的图片说明
- `public/images/wechat-qr.md` - 用户可见的二维码说明

## 📝 文档命名规范

### 文件命名

- **索引文档**：`README.md`、`DOCUMENTATION_*.md`
- **指南文档**：`*_GUIDE.md`、`*_GUIDELINES.md`
- **状态文档**：`*_STATUS.md`、`*_PROGRESS.md`
- **计划文档**：`*_PLAN.md`
- **总结文档**：`*_SUMMARY.md`
- **修复文档**：`*_FIX.md`
- **架构文档**：`*_ARCHITECTURE.md`

### 目录命名

- 使用小写字母
- 使用连字符分隔（kebab-case）
- 名称要清晰描述内容

## 🔍 如何查找文档

### 方法 1：使用文档索引

查看 [完整文档索引](./DOCUMENTATION_INDEX.md)，按分类或使用场景查找。

### 方法 2：使用文档目录

查看 [文档目录](./README.md)，按分类浏览。

### 方法 3：使用文档指南

查看 [文档使用指南](./DOCUMENTATION_GUIDE.md)，了解如何使用文档。

## ⚠️ 注意事项

1. **不要将文档放在根目录**：所有文档应放在 `docs/` 目录下
2. **保持分类清晰**：按功能分类，不要混放
3. **及时更新索引**：移动文档后要更新文档索引
4. **更新链接**：移动文档后要更新所有引用链接

## 🔗 相关文档

- [完整文档索引](./DOCUMENTATION_INDEX.md)
- [文档使用指南](./DOCUMENTATION_GUIDE.md)
- [文档整理总结](./DOCUMENTATION_SUMMARY.md)
- [文档整理清理报告](./DOCUMENTATION_CLEANUP.md)

---

**最后更新**：2024-12-03  
**维护者**：项目团队

