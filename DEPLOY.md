# 部署指南

本文档说明如何配置和部署项目到生产环境。

## 📋 环境变量配置

### 开发环境

开发环境使用 `.env` 文件（已配置，使用本地 API）：

```env
NUXT_PUBLIC_API_BASE=http://localhost:5234/api
```

### 生产环境

生产环境使用 `.env.production` 文件（已配置，使用生产 API）：

```env
NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api
```

## 🚀 部署方式

### 方式一：使用部署脚本（推荐）

#### Windows (PowerShell)

```powershell
.\deploy.ps1
```

#### Linux/macOS

```bash
chmod +x deploy.sh
./deploy.sh
```

### 方式二：手动构建

#### 使用 npm 脚本

```bash
# 生产环境构建
npm run build:prod

# 或静态生成（SSG）
npm run generate:prod
```

#### 使用环境变量

```bash
# Windows (PowerShell)
$env:NUXT_PUBLIC_API_BASE="https://api.xifg.com.cn/api"
npm run build

# Linux/macOS
export NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api
npm run build
```

### 方式三：GitHub Actions 自动部署

项目已配置 GitHub Actions 工作流（`.github/workflows/deploy.yml`），当代码推送到 `main` 或 `master` 分支时会自动构建。

构建产物会保存在 GitHub Actions 的 Artifacts 中，可以下载后部署到服务器。

## 📦 构建产物

构建完成后，所有文件都在 `.output` 目录中：

- **SSR 模式**: `.output/server/` 和 `.output/public/`
- **静态模式**: `.output/public/`

## 🔧 服务器部署

### Nginx 配置示例

#### SSR 模式

```nginx
server {
    listen 80;
    server_name your-domain.com;

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

#### 静态模式

```nginx
server {
    listen 80;
    server_name your-domain.com;
    root /path/to/.output/public;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

### PM2 部署（SSR 模式）

```bash
# 安装 PM2
npm install -g pm2

# 启动应用
pm2 start .output/server/index.mjs --name "xifg-site"

# 保存配置
pm2 save

# 设置开机自启
pm2 startup
```

## ✅ 验证部署

部署完成后，请验证：

1. **API 连接**: 打开浏览器控制台，检查 API 请求是否指向 `https://api.xifg.com.cn/api`
2. **功能测试**: 测试主要功能（登录、数据加载等）
3. **性能检查**: 检查页面加载速度和资源大小

## 🔍 故障排查

### API 请求失败

1. 检查环境变量是否正确设置
2. 检查 API 服务是否可访问
3. 检查 CORS 配置

### 构建失败

1. 检查 Node.js 版本（需要 18+）
2. 清除缓存：`rm -rf .nuxt .output node_modules`
3. 重新安装依赖：`npm ci`

## 📝 注意事项

1. **环境变量**: `.env` 和 `.env.production` 文件已添加到 `.gitignore`，不会被提交到仓库
2. **API 路径**: 确保生产环境的 API 服务 `https://api.xifg.com.cn/api` 已正确配置并运行
3. **HTTPS**: 生产环境建议使用 HTTPS，确保 API 通信安全
4. **缓存**: 部署后可能需要清除浏览器缓存才能看到最新版本

## 🔗 相关链接

- [Nuxt 3 部署文档](https://nuxt.com/docs/getting-started/deployment)
- [环境变量配置](https://nuxt.com/docs/guide/going-further/runtime-config)

