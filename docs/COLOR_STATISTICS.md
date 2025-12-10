# 硬编码颜色统计报告

## 📊 统计说明

本报告统计了项目中所有硬编码颜色值的使用情况，用于后续迁移到 themeOverrides。

## 🔍 统计方法

使用正则表达式搜索所有颜色值：
- `#[0-9a-fA-F]{3,6}` - 十六进制颜色
- `rgba?\([^)]+\)` - RGB/RGBA 颜色

## 📋 颜色使用统计（Top 30）

由于 PowerShell 命令执行限制，建议使用以下方法手动统计：

### 方法 1：使用 VS Code 搜索

1. 在 VS Code 中打开项目
2. 使用搜索功能（Ctrl+Shift+F）
3. 搜索模式：`#[0-9a-fA-F]{3,6}|rgba?\([^)]+\)`
4. 在结果中按颜色值分组统计

### 方法 2：使用 grep 工具

```bash
# 在项目根目录执行
grep -rE "#[0-9a-fA-F]{3,6}|rgba?\([^)]+\)" --include="*.vue" --include="*.css" --include="*.scss" --include="*.ts" | grep -v node_modules | grep -v dist
```

## 🎨 常见颜色角色映射表

基于项目分析，以下是常见的颜色角色和迁移目标：

| 旧颜色值 | 角色说明 | 迁移目标 | 优先级 |
|---------|---------|---------|--------|
| `#ffffff` / `#fff` | 白色背景 | `themeOverrides.Card.color` (浅色) | 高 |
| `#000000` / `#000` | 黑色文字 | `themeOverrides.common.textColor1` (浅色) | 高 |
| `#f3f4f6` | 浅灰背景（分区） | `themeOverrides.Layout.hoverColor` (浅色) | 中 |
| `#e5e7eb` | 分割线/边框 | `themeOverrides.common.borderColor` (浅色) | 高 |
| `#0f172a` | 深色背景 | `themeOverrides.Layout.color` (深色) | 高 |
| `rgba(255,255,255,0.05)` | 深色模式卡片蒙层 | `themeOverrides.Card.color` (深色) | 高 |
| `rgba(255,255,255,0.1)` | 深色模式边框 | `themeOverrides.common.borderColor` (深色) | 高 |
| `#1e293b` | 深色侧边栏 | `themeOverrides.Layout.siderColor` (深色) | 中 |
| `#3b82f6` | 主色调 | `themeOverrides.common.primaryColor` | 高 |
| `#ef4444` | 错误色 | Naive UI `error` 类型 | 高 |
| `#10b981` | 成功色 | Naive UI `success` 类型 | 高 |
| `#f59e0b` | 警告色 | Naive UI `warning` 类型 | 高 |

## 📝 迁移策略

### 1. 高优先级颜色（立即迁移）

这些颜色在项目中大量使用，应该优先迁移：

- **白色背景** (`#ffffff`, `#fff`)
  - 迁移到：`themeOverrides.Card.color` (浅色模式)
  - 搜索模式：`#fff(fff)?\b`
  
- **黑色文字** (`#000000`, `#000`)
  - 迁移到：`themeOverrides.common.textColor1` (浅色模式)
  - 搜索模式：`#000(000)?\b`

- **边框颜色** (`#e5e7eb`, `rgba(255,255,255,0.1)`)
  - 迁移到：`themeOverrides.common.borderColor`
  - 根据主题模式使用不同值

### 2. 中优先级颜色（逐步迁移）

这些颜色使用频率中等，可以逐步迁移：

- **浅灰背景** (`#f3f4f6`, `#f8fafc`)
  - 迁移到：`themeOverrides.Layout.hoverColor` (浅色模式)
  
- **深色背景** (`#0f172a`, `#1e293b`)
  - 迁移到：`themeOverrides.Layout.color` (深色模式)

### 3. 低优先级颜色（保留或特殊处理）

这些颜色可能是特殊用途，需要单独处理：

- **渐变背景** - 可能需要保留自定义样式
- **特殊效果** - 如玻璃态、毛玻璃效果等

## 🔧 迁移步骤

1. **统计阶段**（当前阶段）
   - 使用搜索工具统计所有颜色值
   - 按使用频率排序
   - 识别颜色角色

2. **映射阶段**
   - 为每个颜色值分配角色
   - 确定迁移目标（themeOverrides 或 Naive UI）
   - 创建映射表

3. **迁移阶段**
   - 按优先级逐个迁移
   - 使用查找替换工具
   - 验证迁移结果

4. **验证阶段**
   - 在深色和浅色模式下测试
   - 确保视觉效果一致
   - 修复迁移错误

## 📌 注意事项

1. **不要盲目替换**
   - 先理解颜色的用途
   - 确认迁移目标正确
   - 保留特殊效果的颜色

2. **测试验证**
   - 每次迁移后都要测试
   - 确保深色和浅色模式都正常
   - 检查对比度和可读性

3. **文档记录**
   - 记录迁移的颜色值
   - 记录迁移目标
   - 记录特殊处理的情况

## 🎯 目标

最终目标：
- ✅ 所有颜色都通过 themeOverrides 控制
- ✅ 深色和浅色模式切换流畅
- ✅ 视觉风格统一，无颜色割裂感
- ✅ 代码简洁，易于维护

