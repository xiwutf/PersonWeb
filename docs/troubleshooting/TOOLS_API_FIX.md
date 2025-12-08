# 工具管理 API 问题修复说明

## 问题描述

工具管理页面在生产环境报错：`404 Not Found` for `https://api.xifg.com.cn/api/admin/tools`

## 问题原因

1. **工具管理不访问后端 API**：工具管理使用 Nuxt Server API (`server/api/admin/tools.ts`) 读取 `content/tools/*.md` 文件
2. **静态生成导致 Nuxt Server API 不可用**：当前部署使用 `nuxt generate`（静态生成），Nuxt Server API 路由在生产环境中不存在
3. **请求被错误发送到后端**：当 Nuxt Server API 不可用时，请求可能被错误地发送到后端 API

## 解决方案

### 方案一：改为 SSR 模式（推荐）

**优点**：
- Nuxt Server API 路由可用
- 工具管理功能正常工作
- 支持服务端渲染

**缺点**：
- 需要 Node.js 服务器运行
- 不能部署到纯静态托管（如 OSS、GitHub Pages）

**实施步骤**：

1. 修改部署脚本，使用 `nuxt build` 而不是 `nuxt generate`：

```yaml
# .github/workflows/deploy-frontend.yml
- name: Build SSR site
  env:
    NODE_ENV: production
    NUXT_PUBLIC_API_BASE: https://api.xifg.com.cn/api
  run: npm run build  # 使用 build 而不是 generate
```

2. 部署时需要运行 Node.js 服务器：

```bash
# 使用 PM2 或其他进程管理器
pm2 start .output/server/index.mjs --name "xifg-site"
```

3. Nginx 配置需要代理到 Node.js 服务器：

```nginx
server {
    listen 80;
    server_name xifg.com.cn;

    location / {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### 方案二：创建后端 Tools API

**优点**：
- 可以继续使用静态生成
- 工具数据存储在数据库中，便于管理

**缺点**：
- 需要创建后端 API
- 需要修改工具管理页面代码

**实施步骤**：

1. 在后端创建 `ToolsController`：

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ToolsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ToolsController(AppDbContext _context)
    {
        _context = _context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Tool>>>> GetTools()
    {
        var tools = await _context.Tools.ToListAsync();
        return Ok(ApiResponse<List<Tool>>.Success(tools));
    }

    // ... 其他 CRUD 操作
}
```

2. 创建 `Tool` 实体类和数据表

3. 修改前端工具管理页面，使用后端 API：

```typescript
// pages/admin/tools.vue
const fetchTools = async () => {
  try {
    // 改为使用后端 API
    const res = await api.get<any[]>('/Tools')  // 不再是 /api/admin/tools
    tools.value = Array.isArray(res) ? res : []
  } catch (e: any) {
    console.error('Failed to fetch tools:', e)
    tools.value = []
  }
}
```

### 方案三：混合方案（推荐用于当前情况）

**保持静态生成，但工具管理使用后端 API**：

1. 工具管理改为使用后端 API（需要创建后端 API）
2. 其他页面继续使用静态生成
3. 工具数据存储在数据库中

## 当前状态

- ✅ `useApi.ts` 已修复，正确识别 Nuxt Server API 路由
- ⚠️ 如果使用静态生成，工具管理功能不可用
- 💡 建议：改为 SSR 模式或创建后端 Tools API

## 验证方法

1. 检查浏览器控制台，查看 API 请求日志
2. 确认 `[useApi]` 日志中 `isNuxtServerAPI` 的值
3. 确认 `finalUrl` 是否正确（Nuxt Server API 应该是相对路径）

