# 环境兼容性说明

## 🎯 自动环境判断

项目已配置为**自动根据当前访问域名判断使用哪个 API**，无需手动切换配置。

## 📋 工作原理

### 客户端自动判断

在客户端（浏览器）运行时，系统会根据 `window.location.hostname` 自动选择 API：

| 访问域名 | 使用的 API |
|---------|-----------|
| `localhost` | `http://localhost:5234/api` |
| `127.0.0.1` | `http://localhost:5234/api` |
| `xifg.com.cn` | `https://api.xifg.com.cn/api` |
| `admin.xifg.com.cn` | `https://api.xifg.com.cn/api` |
| 其他域名 | 使用环境变量配置的默认值 |

### 服务端渲染

在服务端渲染（SSR）时，使用环境变量配置：
- 开发环境：`.env` 文件中的 `NUXT_PUBLIC_API_BASE`
- 生产环境：`.env.production` 文件中的 `NUXT_PUBLIC_API_BASE`

## ✅ 使用场景

### 场景一：本地开发

1. 在本地运行 `npm run dev`
2. 访问 `http://localhost:3000`
3. **自动使用** `http://localhost:5234/api`

### 场景二：生产环境

1. 构建并部署到服务器
2. 访问 `https://xifg.com.cn` 或 `https://admin.xifg.com.cn`
3. **自动使用** `https://api.xifg.com.cn/api`

### 场景三：本地测试生产 API

如果需要本地测试生产 API，可以：

**方法一：修改 hosts 文件（推荐）**

在 `C:\Windows\System32\drivers\etc\hosts`（Windows）或 `/etc/hosts`（Linux/macOS）中添加：

```
127.0.0.1 xifg.com.cn
```

然后访问 `http://xifg.com.cn:3000`，会自动使用生产 API。

**方法二：使用环境变量覆盖**

```bash
# Windows PowerShell
$env:NUXT_PUBLIC_API_BASE="https://api.xifg.com.cn/api"
npm run dev

# Linux/macOS
export NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api
npm run dev
```

## 🔍 验证配置

### 在浏览器控制台检查

```javascript
// 查看当前使用的 API 地址
const api = useApi()
console.log('API Base URL:', api.baseUrl) // 需要修改 useApi 暴露 baseUrl
```

### 在 Network 标签检查

打开浏览器开发者工具 → Network 标签：
- 本地开发：应看到 `http://localhost:5234/api/...`
- 生产环境：应看到 `https://api.xifg.com.cn/api/...`

## 📝 代码实现

核心逻辑在 `composables/useApi.ts` 中的 `getApiBaseUrl()` 函数：

```typescript
function getApiBaseUrl(): string {
    if (process.client) {
        const hostname = window.location.hostname
        
        // 本地开发环境
        if (hostname === 'localhost' || hostname === '127.0.0.1') {
            return 'http://localhost:5234/api'
        }
        
        // 生产环境
        if (hostname.includes('xifg.com.cn')) {
            return 'https://api.xifg.com.cn/api'
        }
    }
    
    // 服务端或默认情况
    return useRuntimeConfig().public.apiBase
}
```

## 🚀 优势

1. **零配置**：无需手动切换环境变量
2. **自动适配**：根据访问域名自动选择正确的 API
3. **开发友好**：本地开发自动使用本地 API
4. **生产安全**：生产环境自动使用生产 API
5. **灵活扩展**：可以轻松添加更多域名判断规则

## ⚠️ 注意事项

1. **域名匹配**：确保生产域名包含 `xifg.com.cn`
2. **CORS 配置**：确保生产 API 已正确配置 CORS，允许前端域名访问
3. **HTTPS**：生产环境必须使用 HTTPS
4. **环境变量**：服务端渲染时仍依赖环境变量配置

## 🔧 故障排查

### 问题：本地开发时使用了生产 API

**原因**：可能是通过域名访问（如修改了 hosts 文件）

**解决**：直接使用 `http://localhost:3000` 访问

### 问题：生产环境使用了本地 API

**原因**：域名判断逻辑未匹配到生产域名

**解决**：检查 `getApiBaseUrl()` 函数中的域名判断逻辑

### 问题：服务端渲染时 API 地址错误

**原因**：服务端无法获取客户端域名，依赖环境变量

**解决**：确保 `.env.production` 文件配置正确

