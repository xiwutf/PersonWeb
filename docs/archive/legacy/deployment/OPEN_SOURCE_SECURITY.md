# 开源安全配置指南

本项目使用 **环境变量 + GitHub Secrets** 的方式保护敏感信息，确保开源后不泄露隐私。

## 架构概览

```
┌──────────────────────────────────────────┐
│           GitHub 仓库 (公开)              │
│  - 代码、配置模板 (占位符)                │
│  - GitHub Actions 工作流                  │
│  - .env.example / appsettings.*.example   │
└──────────────┬───────────────────────────┘
               │ 引用
┌──────────────▼───────────────────────────┐
│       GitHub Secrets (加密存储)           │
│  - 数据库连接信息                         │
│  - API 密钥                              │
│  - SSH 密钥                              │
│  - OSS 配置                              │
└──────────────────────────────────────────┘
               │ 注入
┌──────────────▼───────────────────────────┐
│       GitHub Actions 运行时               │
│  - secrets 值作为环境变量注入             │
│  - 日志中自动遮罩 secrets                │
│  - 不会暴露到公开的 Actions 日志          │
└──────────────────────────────────────────┘
```

## GitHub Secrets 配置

在 GitHub 仓库的 **Settings → Secrets and variables → Actions** 中配置以下 Secrets：

### 前端部署 (deploy-frontend.yml)

| Secret 名称 | 说明 | 示例值 |
|---|---|---|
| `NUXT_PUBLIC_API_BASE` | 生产环境 API 地址 | `https://api.your-domain.com/api` |
| `OSS_ENDPOINT` | 阿里云 OSS Endpoint | `oss-cn-hangzhou.aliyuncs.com` |
| `OSS_AK` | OSS Access Key ID | `LTAI5t...` |
| `OSS_SK` | OSS Access Key Secret | `xxxxx` |
| `OSS_BUCKET` | OSS Bucket 名称 | `your-bucket-name` |

### 后端部署 (deploy-backend.yml)

| Secret 名称 | 说明 | 示例值 |
|---|---|---|
| `SSH_PRIVATE_KEY` | 服务器 SSH 私钥 | `-----BEGIN OPENSSH PRIVATE KEY-----...` |
| `REMOTE_HOST` | 服务器 IP 地址 | `1.2.3.4` |
| `REMOTE_USER` | SSH 用户名 | `root` |

### AI 服务部署 (deploy-ai-service.yml)

复用后端部署的 `SSH_PRIVATE_KEY`、`REMOTE_HOST`、`REMOTE_USER`。

## 本地开发配置

### 后端 (.NET)

1. 复制 `appsettings.Development.json.example` 为 `appsettings.Development.json`
2. 填写你的本地数据库连接和 API Key
3. `appsettings.Development.json` 已在 `.gitignore` 中，不会被提交

### AI 服务 (Python)

1. 进入 `ai-service/` 目录
2. 复制 `.env.example` 为 `.env`
3. 填写你的 API Key 和数据库信息
4. `.env` 已在 `.gitignore` 中，不会被提交

### 前端 (Nuxt)

1. 复制根目录的 `.env.example` 为 `.env`
2. 设置 `NUXT_PUBLIC_API_BASE` 为你的后端地址
3. `.env` 已在 `.gitignore` 中，不会被提交

## GitHub Actions 安全说明

**GitHub Secrets 不会泄露的原因：**

1. **加密存储** - Secrets 使用 Libsodium sealed box 加密，存储在 GitHub 服务器上
2. **日志遮罩** - GitHub Actions 自动将 Secrets 值替换为 `***`
3. **不可读取** - 即使是仓库管理员也无法查看已设置的 Secret 值，只能更新
4. **Fork 隔离** - Fork 的仓库无法访问原仓库的 Secrets
5. **PR 隔离** - 来自 Fork 的 Pull Request 无法访问 Secrets

## 被保护的敏感信息清单

| 类型 | 文件 | 保护方式 |
|---|---|---|
| 数据库连接串 | `appsettings.json` | 值设为空，通过 `appsettings.Development.json` 覆盖 |
| DeepSeek API Key | `appsettings.json` / `config.py` | 值设为空，通过环境变量注入 |
| JWT 签名密钥 | `appsettings.json` | 值设为空，通过环境变量注入 |
| 内部服务 Token | `appsettings.json` / `config.py` | 值设为空，通过环境变量注入 |
| OSS 凭证 | `deploy-frontend.yml` | 使用 `${{ secrets.XXX }}` |
| SSH 私钥 | `deploy-backend.yml` | 使用 `${{ secrets.XXX }}` |
| 生产 API 地址 | `deploy-frontend.yml` | 使用 `${{ secrets.XXX }}` |

## 安全检查命令

```bash
# 检查当前代码中是否有遗留的敏感信息
git grep -n "sk-" -- "*.json" "*.py" "*.cs" "*.ts"
git grep -n "Password=" -- "*.json" "*.cs"

# 检查 git 历史中是否有敏感信息
git log --all -p | findstr "your-old-api-key"
```
