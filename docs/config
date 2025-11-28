# 生产环境 API 配置总结

## ✅ 已完成的配置

### 1. 环境变量文件

#### `.env` (开发环境)
```env
NUXT_PUBLIC_API_BASE=http://localhost:5234/api
```

#### `.env.production` (生产环境)
```env
NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api
```

#### `.env.example` (示例文件)
已创建示例文件，供团队成员参考。

### 2. Nuxt 配置更新

`nuxt.config.ts` 已更新，支持通过环境变量配置 API 基础路径：

```typescript
runtimeConfig: {
  public: {
    apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5234/api'
  }
}
```

### 3. Package.json 脚本

新增了生产环境构建脚本：

- `npm run build:prod` - 使用生产环境变量构建
- `npm run generate:prod` - 使用生产环境变量静态生成

### 4. 部署脚本

#### `deploy.sh` (Linux/macOS)
- 自动检查 Node.js 版本
- 安装依赖
- 使用生产环境变量构建
- 显示构建信息

#### `deploy.ps1` (Windows PowerShell)
- 功能同 `deploy.sh`
- 适配 Windows 环境

### 5. GitHub Actions 工作流

`.github/workflows/deploy.yml`
- 自动触发：推送到 main/master 分支
- 手动触发：通过 GitHub Actions UI
- 自动构建并上传构建产物

### 6. Git 配置

`.gitignore` 已更新，忽略所有环境变量文件：
- `.env`
- `.env.production`
- `.env.local`
- `.env.*.local`

## 🚀 使用方法

### 开发环境

直接运行，会自动使用 `.env` 文件中的配置：

```bash
npm run dev
```

### 生产环境构建

#### 方式一：使用脚本（推荐）

**Windows:**
```powershell
.\deploy.ps1
```

**Linux/macOS:**
```bash
chmod +x deploy.sh
./deploy.sh
```

#### 方式二：使用 npm 脚本

```bash
npm run build:prod
```

#### 方式三：手动设置环境变量

**Windows (PowerShell):**
```powershell
$env:NUXT_PUBLIC_API_BASE="https://api.xifg.com.cn/api"
npm run build
```

**Linux/macOS:**
```bash
export NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api
npm run build
```

## 🔍 验证配置

构建完成后，可以通过以下方式验证 API 配置：

1. **检查构建产物**: 查看 `.output` 目录
2. **浏览器控制台**: 部署后检查网络请求，确认 API 请求指向 `https://api.xifg.com.cn/api`
3. **环境变量检查**: 在构建日志中查看使用的 API 路径

## 📝 注意事项

1. **环境变量优先级**:
   - 命令行环境变量 > `.env.production` > `.env` > 默认值
   
2. **API 服务要求**:
   - 确保 `https://api.xifg.com.cn/api` 已正确配置 CORS
   - 确保 API 服务支持 HTTPS
   - 确保 API 服务正常运行

3. **部署前检查**:
   - ✅ API 服务可访问
   - ✅ 环境变量已正确设置
   - ✅ 构建产物完整

## 🔗 相关文件

- `nuxt.config.ts` - Nuxt 配置文件
- `composables/useApi.ts` - API 调用封装
- `.env` - 开发环境变量
- `.env.production` - 生产环境变量
- `deploy.sh` - Linux/macOS 部署脚本
- `deploy.ps1` - Windows 部署脚本
- `.github/workflows/deploy.yml` - GitHub Actions 工作流
- `DEPLOY.md` - 详细部署文档

## ✨ 配置完成

所有生产环境 API 配置已完成！现在可以使用以下命令进行生产构建：

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

