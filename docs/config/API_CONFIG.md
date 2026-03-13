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

生产环境构建会自动使用 `.env.production` 文件（通过 `NODE_ENV=production` 自动加载）：

```bash
npm run build
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
2. 确保使用 `npm run build`（会自动设置 `NODE_ENV=production` 并加载 `.env.production`）
3. 检查构建日志中的环境变量值

### 问题：部署后「加载列表失败」或首页展示不对

**现象**：管理后台分类管理等页面报「加载列表失败」，或首页一直显示默认风格、内容不符合预期。

**原因**：前端请求的后端 API（如 `https://api.xing.com.cn/api` 或 `https://api.xifg.com.cn/api`）无法访问或未返回正常响应。

**排查步骤**：

1. **确认 API 服务已启动且可访问**
   - 在服务器上确认 .NET 后端进程正在运行（如 `PersonalSite.Api`）。
   - 在浏览器或本机用 curl 访问：`https://api.xing.com.cn/api/health`（或你的 API 域名），应返回 `Healthy`。
   - 若使用 Nginx 反向代理，检查代理到后端端口的配置是否正确（如 `proxy_pass http://127.0.0.1:5234`）。

2. **确认前端使用的 API 域名**
   - 前端会根据当前站点域名自动选择 API：
     - 访问 `xifg.com.cn` → 使用 `https://api.xifg.com.cn/api`
     - 访问 `xing.com.cn` → 使用 `https://api.xing.com.cn/api`
   - 若部署域名不在上述之列，需在构建时设置 `NUXT_PUBLIC_API_BASE`（例如在 CI/部署脚本中）为你的 API 完整地址（含 `/api`）。

3. **CORS**
   - 本项目后端已配置 `AllowAnyOrigin()`，一般不会因 CORS 导致浏览器报错。若你改成了白名单，需把前端站点域名（如 `https://xing.com.cn`）加入允许来源。

4. **首页「展示不对」**
   - 首页会请求 `/config/home-style` 决定展示风格；若该接口失败，会回退为默认「creative」风格。
   - 若希望展示其他风格，需保证该接口可访问且返回正确配置；同时检查 API 可访问性（同上 1、2）。

5. **开发者工具辅助**
   - 打开 F12 → Network，筛选 Fetch/XHR，看失败请求的 URL 和状态码（如 0、502、404），便于区分「网络/代理错误」还是「接口未部署/路径错误」。

