# 🎨 样式管理快速提醒

## ⚠️ 修改页面样式前必读

**所有样式必须统一管理，禁止在 template 中使用内联样式！**

## 📋 快速检查清单

- [ ] 是否查看了 `docs/development/STYLE_MANAGEMENT_REMINDER.md`？
- [ ] 是否检查了 `assets/css/` 目录下是否有可复用的样式？
- [ ] 是否避免了在 template 中使用内联样式（`:style` 除外）？

## ✅ 正确的做法

### 1. 使用统一样式文件（推荐）

```vue
<!-- ✅ 正确 -->
<div class="admin-card">
  <button class="admin-button-primary">按钮</button>
</div>
```

**样式文件位置**：`assets/css/admin-*.css`

### 2. 使用组件级样式

```vue
<style scoped>
.analytics-container {
  padding: 1rem;
  background: rgba(255, 255, 255, 0.1);
}
</style>
```

### 3. 动态样式（仅用于必须动态计算的属性）

```vue
<!-- ✅ 正确：位置需要动态计算 -->
<div :style="{ left: `${x}%`, top: `${y}%` }">
  内容
</div>
```

## ❌ 禁止的做法

```vue
<!-- ❌ 错误：固定样式使用内联 -->
<div style="padding: 1rem; background: #fff;">
  内容
</div>

<!-- ❌ 错误：固定值使用 :style -->
<div :style="{ padding: '1rem', background: '#fff' }">
  内容
</div>
```

## 📚 详细文档

- [样式管理开发提醒](./docs/development/STYLE_MANAGEMENT_REMINDER.md) - 完整开发流程和示例
- [开发规范文档](./docs/development/DEVELOPMENT_GUIDELINES.md) - 项目开发规范
- [前端样式系统文档](./docs/development/FRONTEND_STYLE_SYSTEM.md) - 样式管理系统

## 🛠️ 样式文件位置

- **管理后台**：`assets/css/admin-*.css`
- **访客页面**：`assets/css/visitor-*.css`
- **通用样式**：`assets/css/common-*.css` 或 `assets/css/main.css`

---

**记住**：每次修改页面样式时，先问自己：
1. 这个样式是否可以在其他地方复用？→ 提取到统一样式文件
2. 这个样式是否只在这个组件使用？→ 使用 `<style scoped>`
3. 这个样式是否需要动态计算？→ 使用 `:style` 绑定（最小化）

