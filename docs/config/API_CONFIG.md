# API 配置说明

## 🔍 问题说明

如果您在开发模式下看到 API 请求指向 `http://localhost:5234/api`，这是因为：

1. **开发模式** (`npm run dev`) 默认使用 `.env` 文件中的配置
2. `.env` 文件中配置的是本地开发 API：`http://localhost:5234/api`

## ✅ 解决方案

### 方案一：使用 .env.local 文件（推荐）

`.env.local` 文件的优先级高于 `.env`，且不会被提交到 Git。

**已自动创建 `.env.local` 文件，配置为使用生产环境 API。**

如果 `.env.local` 文件已存在，请检查其内容：

```env
NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api
```

**重启开发服务器**后生效：
```bash
# 停止当前服务器 (Ctrl+C)
npm run dev
```

### 方案二：使用开发脚本（临时）

使用新增的 `dev:prod` 脚本，临时使用生产 API：

```bash
npm run dev:prod
```

### 方案三：手动设置环境变量

**Windows (PowerShell):**
```powershell
$env:NUXT_PUBLIC_API_BASE="https://api.xifg.com.cn/api"
npm run dev
```

**Linux/macOS:**
```bash
export NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api
npm run dev
```

### 方案四：修改 .env 文件（不推荐）

直接修改 `.env` 文件，但这会影响所有开发环境：

```env
NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api
```

## 📋 环境变量优先级

Nuxt 3 的环境变量加载优先级（从高到低）：

1. **命令行环境变量** - 最高优先级
2. **`.env.local`** - 本地覆盖（不提交到 Git）
3. **`.env.production`** - 生产环境（构建时使用）
4. **`.env`** - 默认开发环境
5. **`nuxt.config.ts` 中的默认值** - 最低优先级

## 🚀 生产环境构建

生产环境构建会自动使用 `.env.production` 文件：

```bash
npm run build:prod
```

或使用部署脚本：

```bash
# Windows
.\deploy.ps1

# Linux/macOS
./deploy.sh
```

## 🔍 验证配置

### 检查当前使用的 API 地址

在浏览器控制台中运行：

```javascript
// 查看当前 API 配置
console.log(useRuntimeConfig().public.apiBase)
```

或在 Vue 组件中：

```vue
<script setup>
const config = useRuntimeConfig()
console.log('API Base:', config.public.apiBase)
</script>
```

### 检查网络请求

打开浏览器开发者工具 → Network 标签，查看 API 请求的 URL：
- ✅ 应该看到：`https://api.xifg.com.cn/api/auth/login`
- ❌ 不应该看到：`http://localhost:5234/api/auth/login`

## 📝 注意事项

1. **`.env.local` 文件已添加到 `.gitignore`**，不会被提交到仓库
2. **修改环境变量后需要重启开发服务器**才能生效
3. **生产环境构建**会自动使用 `.env.production` 文件
4. **确保生产 API 服务已正确配置 CORS**，允许前端域名访问

## 🔧 故障排查

### 问题：修改后仍然使用本地 API

1. 检查 `.env.local` 文件是否存在且内容正确
2. 重启开发服务器（完全停止后重新启动）
3. 清除 Nuxt 缓存：删除 `.nuxt` 目录后重新运行

### 问题：生产构建后仍使用本地 API

1. 检查 `.env.production` 文件内容
2. 使用 `npm run build:prod` 而不是 `npm run build`
3. 检查构建日志中的环境变量值

