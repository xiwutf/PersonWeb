# AI Service 部署指南

本文档说明如何在 ECS 服务器上部署 AI Service。

## 📋 前置要求

- Ubuntu 20.04+ 或 CentOS 7+
- Python 3.10+（推荐 3.11+，工作流会自动安装 3.11 如果不存在）
- Nginx（已安装并运行）
- Git（用于拉取代码）

**注意**：如果你的服务器是 Python 3.10，也可以正常运行。但推荐使用 Python 3.11，部署工作流会自动尝试安装。

## 🚀 部署步骤

### 1. 创建项目目录

```bash
sudo mkdir -p /srv/ai-service
sudo chown $USER:$USER /srv/ai-service
cd /srv/ai-service
```

### 2. 拉取项目代码

#### 方式一：使用 Git Clone

```bash
git clone <your-repo-url> .
# 或者如果项目在子目录
git clone <your-repo-url> temp
mv temp/ai-service/* .
rm -rf temp
```

#### 方式二：使用 CI/CD（推荐）⭐

项目已配置 GitHub Actions 自动部署工作流。当你推送代码到 `master` 分支时，会自动部署到 ECS。

**首次部署前准备**：

1. 确保 ECS 上已创建目录：
   ```bash
   sudo mkdir -p /srv/ai-service
   sudo chown $USER:$USER /srv/ai-service
   ```

2. 在 GitHub 仓库的 Settings → Secrets 中配置以下密钥（如果还没有）：
   - `SSH_PRIVATE_KEY`：SSH 私钥
   - `REMOTE_HOST`：ECS 服务器 IP 或域名
   - `REMOTE_USER`：SSH 用户名

3. 首次部署后，在 ECS 上手动创建 `.env` 文件（见步骤 4）

**自动部署流程**：

- 当你推送代码到 `master` 分支，且修改了 `ai-service/` 目录下的文件时
- GitHub Actions 会自动：
  1. 安装 Python 依赖
  2. 将代码同步到 ECS 的 `/srv/ai-service/` 目录
  3. 在服务器上创建/更新虚拟环境并安装依赖
  4. 安装 systemd 服务（如果不存在）
  5. 重启服务
  6. 执行健康检查

**注意**：
- `.env` 文件不会被覆盖（已排除）
- `venv`、`logs` 等目录不会被删除
- 首次部署后需要手动配置 `.env` 文件

### 3. 安装 Python 依赖

```bash
cd /srv/ai-service

# 创建虚拟环境（推荐）
python3 -m venv venv
source venv/bin/activate

# 安装依赖
pip install -r requirements.txt

# 如果使用系统 Python（不推荐）
# pip3 install -r requirements.txt
```

#### 4. 配置环境变量

创建 `.env` 文件：

```bash
cd /srv/ai-service
nano .env
```

配置内容：

```bash
# 内部鉴权 Token（必须修改，与 Nginx 配置中的一致）
AI_INTERNAL_TOKEN=your-secure-token-here

# 大模型配置
OPENAI_API_KEY=sk-your-openai-key
DEFAULT_MODEL_NAME=gpt-3.5-turbo

# 其他模型配置（可选）
QWEN_API_KEY=your-qwen-key
QWEN_BASE_URL=https://dashscope.aliyuncs.com

# 日志配置
LOG_LEVEL=INFO
LOG_FILE=/srv/ai-service/logs/ai-service.log
```

**重要**：确保 `AI_INTERNAL_TOKEN` 与 Nginx 配置中的值一致！

#### 5. 测试运行（前台测试）

```bash
cd /srv/ai-service
source venv/bin/activate  # 如果使用虚拟环境

# 前台运行测试
uvicorn app.main:app --host 127.0.0.1 --port 8001
```

在另一个终端测试：

```bash
curl http://127.0.0.1:8001/health
```

如果看到响应，说明服务正常。按 `Ctrl+C` 停止服务。

#### 6. 配置 Systemd 服务

#### 6.1 复制服务文件

```bash
sudo cp ai-service.service /etc/systemd/system/
```

#### 6.2 修改服务配置

编辑服务文件：

```bash
sudo nano /etc/systemd/system/ai-service.service
```

**重要修改项**：

1. 修改 `User` 和 `Group`（如果不用 www-data）：
   ```ini
   User=your-user
   Group=your-group
   ```

2. 修改环境变量（特别是 Token 和 API Key）：
   ```ini
   Environment="AI_INTERNAL_TOKEN=your-actual-token"
   Environment="OPENAI_API_KEY=your-actual-key"
   ```

3. 如果使用虚拟环境，修改 `ExecStart`：
   ```ini
   ExecStart=/srv/ai-service/venv/bin/uvicorn app.main:app --host 127.0.0.1 --port 8001
   ```

#### 6.3 启动服务

```bash
# 重新加载 systemd 配置
sudo systemctl daemon-reload

# 启动服务
sudo systemctl start ai-service

# 设置开机自启
sudo systemctl enable ai-service

# 查看服务状态
sudo systemctl status ai-service

# 查看日志
sudo journalctl -u ai-service -f
```

### 7. 配置 Nginx 反向代理

#### 7.1 添加配置

将 `nginx-ai-service.conf` 中的配置添加到你的 Nginx 配置中。

**方式一：添加到主配置文件**

```bash
sudo nano /etc/nginx/sites-available/default
# 或
sudo nano /etc/nginx/nginx.conf
```

将 `nginx-ai-service.conf` 中的 `upstream` 和 `location` 块添加到相应的 `server` 块中。

**方式二：作为独立配置文件（推荐）**

```bash
sudo cp nginx-ai-service.conf /etc/nginx/conf.d/ai-service.conf
sudo nano /etc/nginx/conf.d/ai-service.conf
```

然后确保在主配置中包含：

```nginx
include /etc/nginx/conf.d/*.conf;
```

#### 7.2 修改配置

**重要**：修改以下内容：

1. `server_name`：改为你的实际域名
2. `X-Internal-Token`：必须与 `.env` 和 `ai-service.service` 中的 `AI_INTERNAL_TOKEN` 一致
3. 如果使用 HTTPS，取消 SSL 相关注释并配置证书

#### 7.3 测试并重载 Nginx

```bash
# 测试配置
sudo nginx -t

# 如果测试通过，重载配置
sudo nginx -s reload
```

#### 8. 验证部署

#### 8.1 检查服务状态

```bash
# 检查 systemd 服务
sudo systemctl status ai-service

# 检查端口监听
sudo netstat -tlnp | grep 8001
# 或
sudo ss -tlnp | grep 8001
```

#### 8.2 测试接口

```bash
# 测试健康检查（直接访问）
curl http://127.0.0.1:8001/health

# 测试通过 Nginx（需要 Token）
curl -H "X-Internal-Token: your-token" http://api.xifg.com.cn/_internal/ai/health
```

### 9. 查看日志

```bash
# Systemd 日志
sudo journalctl -u ai-service -f

# 应用日志
tail -f /srv/ai-service/logs/ai-service.log
```

## 🔧 常用管理命令

### 服务管理

```bash
# 启动服务
sudo systemctl start ai-service

# 停止服务
sudo systemctl stop ai-service

# 重启服务
sudo systemctl restart ai-service

# 查看状态
sudo systemctl status ai-service

# 查看日志
sudo journalctl -u ai-service -n 100
sudo journalctl -u ai-service -f
```

### 更新代码

```bash
cd /srv/ai-service

# 拉取最新代码
git pull

# 如果有新依赖
source venv/bin/activate
pip install -r requirements.txt

# 重启服务
sudo systemctl restart ai-service
```

## 🔐 安全建议

1. **Token 安全**：
   - 使用强随机字符串作为 `AI_INTERNAL_TOKEN`
   - 不要在代码中硬编码 Token
   - 定期轮换 Token

2. **端口安全**：
   - AI Service 只监听 `127.0.0.1`，不对外暴露
   - 通过 Nginx 反向代理访问，不直接暴露端口

3. **文件权限**：
   ```bash
   # 确保配置文件权限正确
   chmod 600 /srv/ai-service/.env
   sudo chown www-data:www-data /srv/ai-service/logs
   ```

4. **防火墙**：
   - 确保防火墙规则不允许外部直接访问 8001 端口

## 🐛 故障排除

### 服务无法启动

1. 检查日志：
   ```bash
   sudo journalctl -u ai-service -n 50
   ```

2. 检查 Python 环境：
   ```bash
   # 检查系统 Python 版本
   python3 --version
   python3.11 --version  # 如果安装了 3.11
   
   # 检查虚拟环境
   cd /srv/ai-service
   source venv/bin/activate
   python --version
   which uvicorn
   ```

3. 检查端口占用：
   ```bash
   sudo lsof -i :8001
   ```

### Nginx 502 错误

1. 检查 AI Service 是否运行：
   ```bash
   sudo systemctl status ai-service
   curl http://127.0.0.1:8001/health
   ```

2. 检查 Nginx 错误日志：
   ```bash
   sudo tail -f /var/log/nginx/error.log
   ```

3. 检查 Token 是否一致：
   - `.env` 文件中的 `AI_INTERNAL_TOKEN`
   - `ai-service.service` 中的 `Environment="AI_INTERNAL_TOKEN"`
   - Nginx 配置中的 `proxy_set_header X-Internal-Token`

### 接口返回 401

- 检查请求头是否包含 `X-Internal-Token`
- 检查 Token 值是否正确
- 检查 Nginx 是否正确传递了 Token

## 📚 相关文档

- [README.md](./README.md) - 项目说明
- [nginx-ai-service.conf](./nginx-ai-service.conf) - Nginx 配置示例
- [ai-service.service](./ai-service.service) - Systemd 服务文件

