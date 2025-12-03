# GitHub Actions 快速修复指南

## 问题：服务启动失败（退出码 3）

当看到以下错误时：
```
Process: 186410 ExecStart=/srv/ai-service/venv/bin/uvicorn app.main:app --host 127.0.0.1 --port 8001 (code=exited, status=3)
```

这通常是因为服务文件中的启动命令格式不正确。

## 快速修复命令

在 GitHub Actions 的 SSH action 中，可以使用以下命令快速修复：

```bash
# 方法 1: 使用快速修复脚本（推荐）
bash /srv/ai-service/quick-fix.sh

# 方法 2: 手动修复
sudo sed -i 's|^ExecStart=/srv/ai-service/venv/bin/uvicorn|ExecStart=/srv/ai-service/venv/bin/python -m uvicorn|g' /etc/systemd/system/ai-service.service && \
sudo systemctl daemon-reload && \
sudo systemctl restart ai-service && \
sleep 3 && \
sudo systemctl status ai-service --no-pager

# 方法 3: 完整诊断和修复
bash /srv/ai-service/diagnose-startup.sh && \
bash /srv/ai-service/fix-service.sh
```

## 在 GitHub Actions Workflow 中使用

```yaml
- name: 修复 AI 服务
  uses: appleboy/ssh-action@v1.0.3
  with:
    host: ${{ secrets.SSH_HOST }}
    username: ${{ secrets.SSH_USERNAME }}
    key: ${{ secrets.SSH_KEY }}
    script: |
      # 检查服务是否存在
      if systemctl list-unit-files | grep -q ai-service.service; then
        # 修复启动命令
        sudo sed -i 's|^ExecStart=/srv/ai-service/venv/bin/uvicorn|ExecStart=/srv/ai-service/venv/bin/python -m uvicorn|g' /etc/systemd/system/ai-service.service
        # 确保日志目录可写
        sudo mkdir -p /srv/ai-service/logs
        sudo chown www-data:www-data /srv/ai-service/logs
        sudo chmod 775 /srv/ai-service/logs
        # 重新加载并重启
        sudo systemctl daemon-reload
        sudo systemctl restart ai-service
        sleep 3
        sudo systemctl status ai-service --no-pager
      else
        echo "⚠️  ai-service.service 未找到，请手动安装服务文件"
      fi
```

## 常见问题排查

### 1. 如果修复后仍然失败

运行诊断脚本：
```bash
bash /srv/ai-service/diagnose-startup.sh
```

### 2. 检查详细错误信息

```bash
sudo journalctl -u ai-service -n 100 --no-pager
```

### 3. 手动测试启动

```bash
cd /srv/ai-service
sudo -u www-data /srv/ai-service/venv/bin/python -m uvicorn app.main:app --host 127.0.0.1 --port 8001
```

### 4. 检查模块导入

```bash
cd /srv/ai-service
sudo -u www-data /srv/ai-service/venv/bin/python -c "from app.main import app; print('OK')"
```

## 根本原因

服务文件中的 `ExecStart` 命令应该使用：
```
ExecStart=/srv/ai-service/venv/bin/python -m uvicorn app.main:app --host 127.0.0.1 --port 8001
```

而不是：
```
ExecStart=/srv/ai-service/venv/bin/uvicorn app.main:app --host 127.0.0.1 --port 8001
```

使用 `python -m uvicorn` 可以确保：
1. 使用正确的 Python 解释器
2. 正确设置 Python 路径
3. 避免权限和路径问题

