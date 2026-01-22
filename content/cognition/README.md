# 个人认知使用说明书 - 使用指南

## 📁 文件结构

```
content/cognition/
├── index.md          # 主说明书内容
├── changelog.md      # 更新日志
└── README.md         # 本文件

pages/cognition/
├── index.vue         # 主页面组件
└── changelog.vue     # 更新日志页面组件
```

## 🔗 访问路径

- **主页面：** `/cognition`
- **更新日志：** `/cognition/changelog`

## ✏️ 如何更新内容

### 更新主说明书

直接编辑 `content/cognition/index.md` 文件。

**更新原则：**
- ✅ 原文不删、不改逻辑（原结论 = 当时版本的你）
- ✅ 所有变化只做：补充边界、增加条件、新增层级
- ❌ 不允许"推翻旧结论"式重写

### 更新日志

在 `content/cognition/changelog.md` 文件顶部添加新版本记录：

```markdown
## v1.1 (2025-XX-XX)

**更新内容：**
- 补充：模型在强不确定环境下的失效条件
- 修正：英语 L0 层投入上限从 20 分钟 → 15 分钟
```

## 🎨 样式说明

页面样式已内置在 Vue 组件中，使用 Tailwind CSS 和自定义样式。

如需调整样式，编辑：
- `pages/cognition/index.vue` 中的 `<style>` 部分
- `pages/cognition/changelog.vue` 中的 `<style>` 部分

## 📝 Markdown 语法支持

支持标准 Markdown 语法，包括：
- 标题、段落、列表
- 表格
- 代码块
- 引用块
- 链接

## 🔄 版本管理

建议使用 Git 管理版本变更，这样可以：
- 追踪认知演化轨迹
- 回看历史版本
- 保持更新记录的完整性

---

**提示：** 这个页面是你的"认知校准器"，定期回顾和更新它，让它成为你思维系统的真实镜像。