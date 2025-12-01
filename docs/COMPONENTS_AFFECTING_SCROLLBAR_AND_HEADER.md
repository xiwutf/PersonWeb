# 影响滚动条和导航栏的组件清单

## 🔴 高优先级 - 最可能影响导航栏点击的组件

### 1. **VisitorBubble** (访客气泡)
- **位置**: `components/VisitorBubble.vue`
- **问题**: `z-index: 200` - **比导航栏 (z-100) 高，会遮挡导航栏**
- **影响**: 导航栏点击失效
- **建议**: 降低 z-index 或删除

### 2. **VisitorInteractionPanel** (访客互动面板)
- **位置**: `components/VisitorInteractionPanel.vue`
- **问题**: `z-index: 150` - **比导航栏 (z-100) 高，会遮挡导航栏**
- **影响**: 导航栏点击失效
- **建议**: 降低 z-index 或删除

### 3. **VisitorDanmakuWall** (弹幕墙)
- **位置**: `components/VisitorDanmakuWall.vue`
- **问题**: `z-index: 100` - **和导航栏同级，可能冲突**
- **影响**: 导航栏点击可能失效
- **建议**: 降低 z-index 或删除

## 🟡 中优先级 - 可能影响滚动条闪烁的组件

### 4. **MouseTrail** (鼠标跟踪特效)
- **位置**: `components/MouseTrail.vue`
- **问题**: 
  - 使用 `requestAnimationFrame` 持续动画
  - `fixed` 定位覆盖整个视口
  - Canvas 不断重绘
- **影响**: 滚动条闪烁、性能问题
- **建议**: 禁用或删除

### 5. **BackgroundEffects** (背景效果)
- **位置**: `components/BackgroundEffects.vue`
- **问题**: 
  - 使用 `requestAnimationFrame` 持续动画
  - 粒子效果不断重绘 canvas
- **影响**: 滚动条闪烁、性能问题
- **建议**: 禁用或删除

### 6. **FireworksEffect** (烟花效果)
- **位置**: `components/FireworksEffect.vue`
- **问题**: 
  - 使用 `requestAnimationFrame` 持续动画
  - `fixed` 定位
- **影响**: 滚动条闪烁
- **建议**: 禁用或删除

## 🟢 低优先级 - 可能影响但概率较低

### 7. **VisitorTriggerEffects** (访客触发效果)
- **位置**: `components/VisitorTriggerEffects.vue`
- **问题**: `fixed` 定位
- **影响**: 可能影响布局
- **建议**: 检查是否需要

### 8. **VisitorSidebarDrawer** (访客侧边栏抽屉)
- **位置**: `components/VisitorSidebarDrawer.vue`
- **问题**: `z-index: 9999` (非常高)
- **影响**: 可能遮挡其他元素
- **建议**: 检查是否需要

## 📋 快速禁用方案

在 `layouts/default.vue` 中，可以临时注释掉这些组件来测试：

```vue
<template>
  <div class="min-h-screen flex flex-col relative bg-bg-body text-text-main">
    <!-- 动态背景效果（根据主题切换） -->
    <!-- <BackgroundEffects :effect="currentBackground" :config="backgroundConfig" /> -->
    
    <!-- 鼠标轨迹特效 -->
    <!-- <MouseTrail /> -->
    
    <!-- 访客互动功能 -->
    <!-- <VisitorDanmakuWall /> -->
    <!-- <VisitorBubble /> -->
    <!-- <VisitorInteractionPanel /> -->
    
    <!-- 访客互动式玩法（包含在抽屉中） -->
    <!-- <VisitorBehaviorListener /> -->
    <!-- <VisitorSidebarDrawer /> -->
    <!-- <VisitorTriggerEffects /> -->
    <!-- <FireworksEffect /> -->
    
    <!-- 其他组件保持不变 -->
  </div>
</template>
```

## 🎯 推荐操作顺序

1. **先禁用 MouseTrail** - 最可能影响滚动条
2. **再禁用 VisitorBubble** - z-index 最高，最可能遮挡导航栏
3. **然后禁用 VisitorInteractionPanel** - z-index 较高
4. **最后禁用 BackgroundEffects** - 如果还有问题

测试每个组件禁用后的效果，找出真正的问题源头。

