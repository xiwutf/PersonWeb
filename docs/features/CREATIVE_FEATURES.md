# 创意功能使用指南

## 🎨 已实现的功能

### 1. 前端交互特效

#### 动态粒子背景 (`ParticleBackground.vue`)
- **位置**: 自动在 `layouts/default.vue` 中加载
- **效果**: 动态粒子连线背景，增强视觉冲击
- **使用**: 无需额外配置，自动生效

#### 鼠标轨迹特效 (`MouseTrail.vue`)
- **位置**: 自动在 `layouts/default.vue` 中加载
- **效果**: 鼠标移动时显示蓝色轨迹点
- **使用**: 无需额外配置，自动生效

#### 毛玻璃卡片 (`GlassCard.vue`)
- **使用示例**:
```vue
<GlassCard hover-effect>
  <h2>标题</h2>
  <p>内容</p>
</GlassCard>
```

#### 打字机效果 (`TypewriterText.vue`)
- **使用示例**:
```vue
<TypewriterText 
  text="欢迎来到我的网站" 
  :speed="100" 
  :loop="true" 
/>
```

### 2. AI 智能助手 (`AIAssistant.vue`)

#### 功能特性
- 右下角浮动按钮
- 智能对话（集成 ChatGPT API）
- 自动介绍、文章搜索、项目推荐
- 快捷操作按钮

#### 配置要求
1. 在 `backend/PersonalSite.Api/appsettings.json` 中添加：
```json
{
  "OpenAI": {
    "ApiKey": "your-openai-api-key"
  }
}
```

2. 重启后端服务

#### API 端点
- `POST /api/AI/chat` - AI 聊天接口

### 3. 时间胶囊功能 (`TimeCapsuleWall.vue`)

#### 功能特性
- 用户留言投递
- 后台审核管理
- 未来墙展示

#### 数据库设置
运行 `database/time_capsule_table.sql` 创建数据表

#### 后台管理
- 路径: `/admin/time-capsules`
- 功能: 审核、通过、拒绝时间胶囊

#### API 端点
- `POST /api/TimeCapsule` - 提交时间胶囊
- `GET /api/TimeCapsule` - 获取已展示的胶囊列表
- `GET /api/TimeCapsule/pending` - 获取待审核列表（管理员）
- `POST /api/TimeCapsule/{id}/approve` - 通过审核（管理员）
- `POST /api/TimeCapsule/{id}/reject` - 拒绝（管理员）

### 4. 3D 互动场景 (`Scene3D.vue`)

#### 功能特性
- 旋转地球、飞船、数据星球
- 点击跳转到不同页面

#### 安装依赖
```bash
npm install three
npm install @types/three --save-dev
```

#### 使用示例
```vue
<Scene3D type="earth" :show-hint="true" />
```

#### 类型说明
- `earth` - 地球，点击跳转到 `/blog`
- `spaceship` - 飞船，点击跳转到 `/projects`
- `datasphere` - 数据星球，点击跳转到 `/dashboard`

## 📦 安装步骤

### 1. 安装前端依赖（3D 场景）
```bash
npm install three @types/three
```

### 2. 创建数据库表
```sql
-- 运行 database/time_capsule_table.sql
```

### 3. 配置 AI API（可选）
在 `backend/PersonalSite.Api/appsettings.json` 中添加 OpenAI API Key

### 4. 重启服务
- 重启 Nuxt 开发服务器
- 重启 .NET 后端服务

## 🎯 使用建议

1. **粒子背景**: 适合深色主题页面，可以调整粒子数量和颜色
2. **鼠标轨迹**: 适合需要增强交互感的页面
3. **AI 助手**: 可以自定义系统提示词，让 AI 更了解你的网站
4. **时间胶囊**: 增加用户参与度，定期审核展示优质留言
5. **3D 场景**: 适合首页或特殊展示页面，增加科技感

## 🔧 自定义配置

### 调整粒子背景
编辑 `components/ParticleBackground.vue`:
- `particleCount`: 粒子数量
- `color`: 粒子颜色

### 自定义 AI 助手
编辑 `components/AIAssistant.vue`:
- 修改 `quickActions` 添加快捷操作
- 修改系统提示词在 `backend/PersonalSite.Api/Controllers/AIController.cs`

### 自定义 3D 场景
编辑 `components/Scene3D.vue`:
- 修改对象样式和颜色
- 调整跳转路由

## 📝 注意事项

1. AI 功能需要 OpenAI API Key，会产生费用
2. 3D 场景需要安装 three.js，会增加打包体积
3. 时间胶囊需要定期审核，建议设置提醒
4. 所有特效在移动端可能性能较差，建议添加响应式控制

