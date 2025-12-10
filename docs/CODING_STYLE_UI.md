# UI 编码规范

本文档定义了项目中 UI 相关的编码规范和最佳实践，确保代码质量和视觉一致性。

---

## 1. 颜色使用规范

### 1.1 禁止硬编码颜色

**❌ 禁止：**
```vue
<!-- 禁止直接写颜色值 -->
<div style="background-color: #ffffff;">
<div class="bg-white text-black">
<span style="color: #000000;">
```

**✅ 正确：**
```vue
<!-- 使用 themeOverrides 或 Naive UI 组件 -->
<n-card>
  <n-button type="primary">按钮</n-button>
</n-card>
```

### 1.2 颜色角色定义

新增颜色必须先定义角色，再写入 `themeOverrides`：

1. **确定颜色角色**（背景、文字、边框、状态色等）
2. **在 `AppNaiveConfig.vue` 的 `themeOverrides` 中添加配置**
3. **在组件中使用 Naive UI 组件或通过 themeOverrides 控制**

### 1.3 例外情况

以下情况允许自定义颜色（需添加注释说明）：

- **特殊视觉效果组件**（玻璃态、渐变、特殊装饰）
  ```vue
  <!-- 特殊视觉组件，允许自定义颜色 -->
  <div class="hero-gradient" style="background: linear-gradient(...)">
  ```

- **图表颜色**（通过 `useEChartsTheme.ts` 统一管理）

---

## 2. Naive UI 组件使用规范

### 2.1 禁止覆盖组件样式

**❌ 禁止：**
```vue
<!-- 禁止给 Naive 组件写颜色相关的 class -->
<n-button class="bg-blue-500 text-white">按钮</n-button>
<n-card class="bg-white shadow-lg">卡片</n-card>
<n-input class="border-red-500">输入框</n-input>
```

**✅ 正确：**
```vue
<!-- 使用 Naive UI 的属性 -->
<n-button type="primary">按钮</n-button>
<n-card>卡片</n-card>
<n-input placeholder="请输入" />
```

### 2.2 样式调整方式

如需调整 Naive UI 组件样式：

1. **优先使用组件属性**（`type`, `size`, `bordered` 等）
2. **通过 `themeOverrides` 全局调整**（在 `AppNaiveConfig.vue` 中）
3. **最后才考虑自定义样式**（只控制布局，不控制颜色）

---

## 3. Card 组件使用规范

### 3.1 基本使用

**✅ 正确：**
```vue
<!-- 普通信息卡片，只使用布局类 -->
<n-card class="mb-4">
  <template #header>标题</template>
  内容
</n-card>

<!-- 仪表盘强调卡片 -->
<n-card class="dashboard-card">
  内容
</n-card>
```

### 3.2 禁止事项

**❌ 禁止：**
```vue
<!-- 禁止添加颜色、阴影、边框类 -->
<n-card class="bg-white shadow-lg rounded-xl border border-gray-200">
<n-card class="settings-card"> <!-- 如果 settings-card 定义了颜色样式 -->
```

**❌ 禁止在 CSS 中写：**
```css
.xxx-card {
  background-color: #ffffff;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  border: 1px solid #e5e7eb;
}
```

### 3.3 允许的样式

**✅ 允许：**
```css
/* 只允许布局相关的样式 */
.dashboard-card {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.dashboard-card:hover {
  transform: translateY(-4px); /* 动画效果 */
}
```

---

## 4. Button 组件使用规范

### 4.1 必须指定 type

**❌ 禁止：**
```vue
<n-button>按钮</n-button> <!-- 缺少 type -->
```

**✅ 正确：**
```vue
<n-button type="primary">主要操作</n-button>
<n-button type="success">成功操作</n-button>
<n-button type="error">危险操作</n-button>
<n-button secondary>次级操作</n-button>
<n-button quaternary>取消操作</n-button>
```

### 4.2 使用场景

- **主操作：** `type="primary"` - 保存、提交、确认等
- **成功操作：** `type="success"` - 成功、完成等
- **危险操作：** `type="error"` - 删除、警告等
- **次级操作：** `secondary` - 次要操作
- **取消操作：** `quaternary` - 取消、关闭等

---

## 5. 主题处理规范

### 5.1 统一使用 useTheme

**❌ 禁止：**
```vue
<script setup>
// 禁止自己实现主题检测
const isDark = computed(() => {
  return document.documentElement.classList.contains('dark')
})
</script>
```

**✅ 正确：**
```vue
<script setup>
// 统一使用 useTheme
const { currentTheme, setTheme, toggleDark } = useTheme()
</script>
```

### 5.2 主题切换

所有主题切换必须：

1. 调用 `useTheme` 的方法（`setTheme` 或 `toggleDark`）
2. 自动更新 `document.documentElement.dataset.theme`
3. 自动更新 Naive UI 的 theme & themeOverrides

---

## 6. 代码审查 Checklist

在提交代码前，请检查：

- [ ] 是否新加了 `#fff`、`#000`、`#f3f4f6` 等硬编码颜色？
- [ ] 是否给 `n-button` / `n-card` / `n-input` 写了颜色相关的 class？
- [ ] 是否用 `useTheme` 统一处理主题，而不是自己搞 `isDark`？
- [ ] Card 组件是否只使用了布局类，没有颜色/阴影/边框类？
- [ ] 按钮是否都有明确的 `type` 属性？
- [ ] 是否使用了 Naive UI 组件而不是自定义实现？
- [ ] 自定义样式是否只控制布局，不控制颜色？

---

## 7. 特殊组件处理

### 7.1 特殊视觉效果组件

对于需要特殊视觉效果（玻璃态、渐变、特殊装饰）的组件：

1. **添加注释说明：**
   ```vue
   <!-- 特殊视觉组件，允许自定义颜色 -->
   <div class="hero-gradient">
   ```

2. **限制数量：** 将这类组件数量控制在非常少的几个"重点模块"中

3. **文档记录：** 在组件文档中说明为什么需要自定义颜色

### 7.2 图表组件

图表颜色统一通过 `useEChartsTheme.ts` 管理，不在组件中直接写颜色。

---

## 8. 文件组织

### 8.1 组件文件

- 组件样式应使用 `<style scoped>`
- 只写布局相关的样式
- 颜色相关的样式通过 `themeOverrides` 控制

### 8.2 全局样式

- 全局样式文件：`assets/styles/tokens.css`（只包含结构类 Token）
- 颜色配置：`components/layout/AppNaiveConfig.vue`（themeOverrides）

---

## 9. 迁移指南

### 9.1 从旧代码迁移

如果遇到旧代码使用了硬编码颜色：

1. **识别颜色角色**（背景、文字、边框等）
2. **查找对应的 themeOverrides 配置**
3. **替换为 Naive UI 组件或移除颜色样式**
4. **验证深色和浅色模式**

### 9.2 参考文档

- `docs/DESIGN_SYSTEM_V1.md` - 设计系统文档
- `docs/COLOR_STATISTICS.md` - 颜色统计和迁移指南
- `docs/PHASE3_THEME_OPTIMIZATION.md` - 优化报告

---

## 10. 工具和脚本

### 10.1 颜色检查（计划中）

未来将添加颜色检查脚本：

```bash
# 扫描硬编码颜色
pnpm lint:colors

# 在 CI/本地 pre-commit 中运行
```

### 10.2 代码格式化

使用项目配置的代码格式化工具，确保代码风格一致。

---

**版本：** v1  
**最后更新：** 2024-12-XX  
**维护者：** 项目团队

