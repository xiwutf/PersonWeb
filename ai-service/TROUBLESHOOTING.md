# AI 服务故障排查指南

## 常见问题

### 1. 服务启动失败（退出码 3）

**可能原因：**
- Python 模块导入错误
- 依赖包未安装
- 配置文件错误
- 日志目录权限问题

**排查步骤：**

1. **检查 Python 版本和虚拟环境**
   ```bash
   cd /srv/ai-service
   source venv/bin/activate
   python --version  # 应该是 3.10+ 或 3.11+
   which python      # 应该指向 venv/bin/python
   ```

2. **检查依赖是否安装**
   ```bash
   pip list | grep -E "fastapi|uvicorn|pydantic"
   ```

3. **手动测试启动**
   ```bash
   cd /srv/ai-service
   source venv/bin/activate
   python -m uvicorn app.main:app --host 127.0.0.1 --port 8001
   ```
   查看具体错误信息

4. **检查日志**
   ```bash
   sudo journalctl -u ai-service -n 50 --no-pager
   # 或
   tail -n 50 /srv/ai-service/logs/ai-service.log
   ```

5. **检查文件权限**
   ```bash
   ls -la /srv/ai-service/
   # 确保 www-data 用户有读取权限
   sudo chown -R www-data:www-data /srv/ai-service
   ```

6. **检查环境变量**
   ```bash
   sudo systemctl show ai-service | grep Environment
   ```

### 2. 模块导入错误

**错误示例：**
```
ModuleNotFoundError: No module named 'app'
```

**解决方法：**
- 确保在 `/srv/ai-service` 目录下运行
- 确保 `PYTHONPATH` 正确设置
- 检查 `app/__init__.py` 文件是否存在

### 3. 端口被占用

**检查端口：**
```bash
sudo netstat -tlnp | grep 8001
# 或
sudo lsof -i :8001
```

**解决方法：**
- 停止占用端口的进程
- 或修改 `ai-service.service` 中的端口号

### 4. 权限问题

**检查 systemd 服务用户：**
```bash
sudo systemctl show ai-service | grep User
```

**解决方法：**
```bash
# 确保 www-data 用户有权限
sudo chown -R www-data:www-data /srv/ai-service
sudo chmod -R 755 /srv/ai-service
```

### 5. 依赖版本冲突

**解决方法：**
```bash
cd /srv/ai-service
source venv/bin/activate
pip install --upgrade pip
pip install -r requirements.txt --force-reinstall
```

## 调试命令

### 查看服务状态
```bash
sudo systemctl status ai-service
```

### 查看实时日志
```bash
sudo journalctl -u ai-service -f
```

### 测试健康检查
```bash
curl http://127.0.0.1:8001/health
```

### 手动启动测试
```bash
cd /srv/ai-service
source venv/bin/activate
python -c "from app.main import app; print('Import successful')"
```

## 重新部署步骤

如果服务完全无法启动，可以尝试重新部署：

1. **停止服务**
   ```bash
   sudo systemctl stop ai-service
   ```

2. **清理虚拟环境（可选）**
   ```bash
   cd /srv/ai-service
   rm -rf venv
   ```

3. **重新创建虚拟环境**
   ```bash
   python3.11 -m venv venv
   source venv/bin/activate
   pip install --upgrade pip
   pip install -r requirements.txt
   ```

4. **测试启动**
   ```bash
   python -m uvicorn app.main:app --host 127.0.0.1 --port 8001
   ```

5. **如果测试成功，重启服务**
   ```bash
   sudo systemctl start ai-service
   sudo systemctl status ai-service
   ```

