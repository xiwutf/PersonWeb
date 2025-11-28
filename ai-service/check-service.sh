#!/bin/bash

# AI 服务诊断脚本
echo "=== AI 服务诊断 ==="
echo ""

# 1. 检查服务文件
echo "1. 检查服务文件..."
if [ -f /etc/systemd/system/ai-service.service ]; then
    echo "✓ 服务文件存在"
    echo "服务文件内容："
    cat /etc/systemd/system/ai-service.service
else
    echo "✗ 服务文件不存在"
fi
echo ""

# 2. 检查工作目录
echo "2. 检查工作目录..."
if [ -d /srv/ai-service ]; then
    echo "✓ 工作目录存在"
    ls -la /srv/ai-service/ | head -10
else
    echo "✗ 工作目录不存在"
fi
echo ""

# 3. 检查虚拟环境
echo "3. 检查虚拟环境..."
if [ -d /srv/ai-service/venv ]; then
    echo "✓ 虚拟环境目录存在"
    if [ -f /srv/ai-service/venv/bin/python ]; then
        echo "✓ Python 可执行文件存在"
        /srv/ai-service/venv/bin/python --version
    else
        echo "✗ Python 可执行文件不存在"
    fi
    if [ -f /srv/ai-service/venv/bin/uvicorn ]; then
        echo "✓ uvicorn 可执行文件存在"
    else
        echo "✗ uvicorn 可执行文件不存在"
    fi
else
    echo "✗ 虚拟环境目录不存在"
fi
echo ""

# 4. 检查应用文件
echo "4. 检查应用文件..."
if [ -f /srv/ai-service/app/main.py ]; then
    echo "✓ app/main.py 存在"
else
    echo "✗ app/main.py 不存在"
fi
if [ -f /srv/ai-service/app/__init__.py ]; then
    echo "✓ app/__init__.py 存在"
else
    echo "✗ app/__init__.py 不存在"
fi
echo ""

# 5. 检查权限
echo "5. 检查权限..."
if [ -d /srv/ai-service ]; then
    echo "目录权限："
    ls -ld /srv/ai-service
    echo "www-data 用户权限测试："
    sudo -u www-data test -r /srv/ai-service/app/main.py && echo "✓ www-data 可以读取 main.py" || echo "✗ www-data 无法读取 main.py"
    sudo -u www-data test -x /srv/ai-service/venv/bin/python && echo "✓ www-data 可以执行 Python" || echo "✗ www-data 无法执行 Python"
fi
echo ""

# 6. 检查日志
echo "6. 最近的系统日志："
sudo journalctl -u ai-service -n 30 --no-pager
echo ""

# 7. 检查端口
echo "7. 检查端口占用："
sudo netstat -tlnp | grep 8001 || echo "端口 8001 未被占用"
echo ""

# 8. 手动测试启动（以 www-data 用户）
echo "8. 尝试手动启动（以 www-data 用户）："
cd /srv/ai-service
sudo -u www-data /srv/ai-service/venv/bin/python -m uvicorn app.main:app --host 127.0.0.1 --port 8001 &
TEST_PID=$!
sleep 3
if ps -p $TEST_PID > /dev/null; then
    echo "✓ 手动启动成功"
    kill $TEST_PID
else
    echo "✗ 手动启动失败"
    wait $TEST_PID
    echo "退出码: $?"
fi
echo ""

echo "=== 诊断完成 ==="

