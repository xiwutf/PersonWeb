# GitHub Secrets 设置指南

## 方法一：手动设置（推荐）

### 1. 准备工作
- 登录你的阿里云 OSS 控制台
- 创建一个 bucket（例如：`my-person-web-bucket`）
- 获取你的 AccessKey（点击右上角头像 → 访问控制 → AccessKey 管理）

### 2. 设置 GitHub Secrets

1. **进入 GitHub 仓库**
   - 打开 [https://github.com/Lijing327/PersonWeb](https://github.com/Lijing327/PersonWeb)
   - 点击仓库名称

2. **进入 Settings**
   - 在仓库页面，点击顶部的 "Settings" 标签

3. **找到 Secrets and variables**
   - 在左侧菜单中，点击 "Secrets and variables"
   - 选择 "Actions"

4. **添加 Repository secrets**
   - 点击 "New repository secret"
   - 为每个 secret 添加以下内容：

### 必需的 Secrets：

#### 1. OSS_BUCKET
- **Name**: `OSS_BUCKET`
- **Secret**: 你的 OSS bucket 名称
  - 例如：`my-person-web-bucket-2024`
  - **注意**：不要包含 `oss://` 前缀
  - **注意**：bucket 名称必须全局唯一

#### 2. OSS_ENDPOINT
- **Name**: `OSS_ENDPOINT`
- **Secret**: 你的 OSS endpoint
  - 例如：`oss-cn-hangzhou.aliyuncs.com`
  - 格式：`oss-<region>.aliyuncs.com`
  - region 可以是：beijing, hangzhou, shanghai, shenzhen 等

#### 3. OSS_AK
- **Name**: `OSS_AK`
- **Secret**: 你的 AccessKey ID
  - 从阿里云控制台获取

#### 4. OSS_SK
- **Name**: `OSS_SK`
- **Secret**: 你的 AccessKey Secret
  - 从阿里云控制台获取

#### 5. NUXT_PUBLIC_API_BASE (可选)
- **Name**: `NUXT_PUBLIC_API_BASE`
- **Secret**: 你的 API 基础 URL
  - 生产环境：`https://your-domain.com/api`
  - 如果没有域名，可以使用：`https://github.com/Lijing327/PersonWeb/releases/latest/download` 等

### 3. 验证设置

设置完成后，你可以：

1. **在 GitHub Actions 页面查看日志**
   - 回到仓库主页
   - 点击 "Actions" 标签
   - 找到 "Deploy Frontend to Aliyun OSS" 运行
   - 查看是否成功

2. **检查部署结果**
   - 访问你的 OSS bucket URL
   - 应该能看到静态网站内容

## 方法二：使用脚本设置（需要 Personal Access Token）

### 1. 创建 Personal Access Token
- 访问 https://github.com/settings/tokens
- 点击 "Generate new token" → "Generate new token (classic)"
- 设置 token 名称：`Setup Secrets`
- 选择权限：
  - ✅ repo（完整仓库访问权限）
  - ✅ workflow（工作流权限）
- 点击 "Generate token"
- **复制生成的 token**（只显示一次）

### 2. 运行设置脚本
```bash
# 克隆项目到本地
git clone https://github.com/Lijing327/PersonWeb.git
cd PersonWeb

# 设置 token
export GITHUB_TOKEN=你的token

# 运行脚本（替换为你的用户名和仓库名）
./setup-github-secrets.sh Lijing327 PersonWeb
```

### 3. 修改脚本中的值
打开 `setup-github-secrets.sh`，修改这些值：
```bash
SECRETS=(
    "OSS_BUCKET=your-bucket-name"           # 替换为你的 bucket 名称
    "OSS_ENDPOINT=oss-cn-hangzhou.aliyuncs.com"  # 替换为你的 endpoint
    "OSS_AK=your-access-key-id"             # 替换为你的 AccessKey ID
    "OSS_SK=your-access-key-secret"         # 替换为你的 AccessKey Secret
    "NUXT_PUBLIC_API_BASE=https://your-domain.com/api"  # 替换为你的 API 地址
)
```

## 常见问题

### Q1: OSS_BUCKET 为空或未设置
A: 确保 GitHub Secrets 中 `OSS_BUCKET` 已正确设置，且值不为空。

### Q2: AccessKey 权限不足
A: 确保 AccessKey 有权限：
- 管理对象（PutObject、GetObject、DeleteObject）
- 获取 Bucket 列表（ListBuckets）

### Q3: Endpoint 错误
A: Endpoint 应该是 `oss-<region>.aliyuncs.com`，不是 `http://` 或 `https://` 开头。

### Q4: Bucket 名称冲突
A: Bucket 名称必须全局唯一，尝试添加时间戳或随机字符。

## 如何获取 OSS 配置信息

### 1. 创建阿里云 OSS Bucket

如果没有 OSS，请按以下步骤创建：

1. **登录阿里云控制台**
   - 访问 https://oss.console.aliyun.com/
   - 使用你的阿里云账号登录

2. **创建 Bucket**
   - 点击"创建 Bucket"
   - Bucket 名称：建议使用 `personweb-2024` 或类似格式（必须全局唯一）
   - 选择地域：建议选择离用户最近的地域（如：杭州、上海）
   - 读写权限：选择"公共读"
   - 点击"确定"创建

3. **获取 AccessKey**
   - 访问 https://ram.console.aliyun.com/users
   - 点击"创建用户"
   - 用户名：`oss-deploy-user`
   - 选择"编程访问"
   - 点击"确定"
   - 复制生成的 AccessKey ID 和 AccessKey Secret
   - **注意**：只显示一次，请妥善保存！

### 2. 项目中已有的配置

从你的项目配置中，我发现：
- 数据库已经在阿里云 RDS：`rm-2zereaqi1k536nd38zo.mysql.rds.aliyuncs.com`
- API 基础路径（开发环境）：`http://localhost:5234/api`

### 3. 生产环境 API 地址建议

如果还没有域名，可以考虑：
- 使用 GitHub Pages：`https://Lijing327.github.io/PersonWeb/api`
- 使用 Vercel：`https://your-app-name.vercel.app/api`
- 使用自建域名：`https://your-domain.com/api`

## 安全提示

1. 不要将 AccessKey 写入代码或提交到仓库
2. 定期轮换 AccessKey
3. 使用最小权限原则设置 AccessKey 权限
4. 不要在多个地方共享同一个 AccessKey

## 完整配置示例

```yaml
# .github/workflows/deploy-frontend.yml 已配置好
# 只需在 GitHub 中设置上述 secrets
```

设置完成后，每次向 master 分支推送代码，GitHub Actions 会自动构建并部署到阿里云 OSS。