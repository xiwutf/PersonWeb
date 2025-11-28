# Git SSL/TLS 连接错误解决方案

## 错误信息
```
fatal: unable to access 'https://github.com/Lijing327/PersonWeb.git/': 
TLS connect error: error:0A000126:SSL routines::unexpected eof while reading
```

## 解决方案

### 方案一：配置 Git 代理（如果使用代理）

如果您使用代理（如 Clash、V2Ray 等），需要配置 Git 代理：

```bash
# 设置 HTTP 代理（替换为您的代理地址和端口）
git config --global http.proxy http://127.0.0.1:7890
git config --global https.proxy http://127.0.0.1:7890

# 或者使用 SOCKS5 代理
git config --global http.proxy socks5://127.0.0.1:7890
git config --global https.proxy socks5://127.0.0.1:7890
```

**取消代理**（如果不需要）：
```bash
git config --global --unset http.proxy
git config --global --unset https.proxy
```

### 方案二：使用 SSH 方式（推荐）

将远程仓库 URL 从 HTTPS 改为 SSH：

```bash
# 查看当前远程仓库
git remote -v

# 将 HTTPS 改为 SSH
git remote set-url origin git@github.com:Lijing327/PersonWeb.git

# 验证
git remote -v
```

**首次使用 SSH 需要配置 SSH 密钥**：
1. 生成 SSH 密钥（如果还没有）：
   ```bash
   ssh-keygen -t ed25519 -C "your_email@example.com"
   ```

2. 将公钥添加到 GitHub：
   - 复制 `~/.ssh/id_ed25519.pub` 的内容
   - 在 GitHub → Settings → SSH and GPG keys → New SSH key

### 方案三：临时禁用 SSL 验证（不推荐，仅用于测试）

```bash
# 仅对当前仓库禁用 SSL 验证
git config http.sslVerify false

# 全局禁用（不推荐）
git config --global http.sslVerify false
```

### 方案四：更新 Git 和 CA 证书

```bash
# 更新 Git（如果版本过旧）
# Windows: 下载最新版本 https://git-scm.com/download/win

# 更新 CA 证书（Windows）
git config --global http.sslCAInfo "C:/Program Files/Git/mingw64/ssl/certs/ca-bundle.crt"
```

### 方案五：使用 GitHub CLI（gh）

如果上述方法都不行，可以尝试使用 GitHub CLI：

```bash
# 安装 GitHub CLI
# Windows: winget install GitHub.cli

# 使用 gh 进行 Git 操作
gh auth login
gh repo clone Lijing327/PersonWeb
```

## 快速修复脚本

### Windows PowerShell

```powershell
# 方案 A: 配置代理（如果使用代理）
$proxy = "http://127.0.0.1:7890"  # 修改为您的代理地址
git config --global http.proxy $proxy
git config --global https.proxy $proxy

# 方案 B: 切换到 SSH（推荐）
git remote set-url origin git@github.com:Lijing327/PersonWeb.git

# 方案 C: 临时禁用 SSL 验证（仅用于测试）
git config http.sslVerify false
```

## 验证修复

```bash
# 测试连接
git fetch origin

# 或者
git pull origin master
```

## 推荐方案

**最佳实践**：使用 SSH 方式（方案二）
- 更安全
- 不需要配置代理
- 连接更稳定

