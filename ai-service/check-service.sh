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

# 8. 检查服务文件中的 ExecStart 命令
echo "8. 检查服务文件中的启动命令..."
if [ -f /etc/systemd/system/ai-service.service ]; then
    EXEC_START=$(grep "^ExecStart=" /etc/systemd/system/ai-service.service | head -1)
    echo "当前 ExecStart: $EXEC_START"
    if echo "$EXEC_START" | grep -q "venv/bin/uvicorn[[:space:]]"; then
        echo "⚠️  发现错误的启动命令格式（直接调用 uvicorn）"
        echo "   应该使用: ExecStart=/srv/ai-service/venv/bin/python -m uvicorn ..."
    elif echo "$EXEC_START" | grep -q "venv/bin/python -m uvicorn"; then
        echo "✓ 启动命令格式正确"
    else
        echo "⚠️  启动命令格式异常"
    fi
fi
echo ""

# 9. 测试 Python 模块导入
echo "9. 测试 Python 模块导入..."
cd /srv/ai-service
if sudo -u www-data /srv/ai-service/venv/bin/python -c "from app.main import app" 2>&1; then
    echo "✓ Python 模块导入成功"
else
    echo "✗ Python 模块导入失败"
    echo "错误详情："
    sudo -u www-data /srv/ai-service/venv/bin/python -c "from app.main import app" 2>&1 || true
fi
echo ""

# 10. 手动测试启动（以 www-data 用户）
echo "10. 尝试手动启动（以 www-data 用户）："
cd /srv/ai-service
# 检查端口占用
if sudo netstat -tlnp 2>/dev/null | grep -q ":8001 " || sudo ss -tlnp 2>/dev/null | grep -q ":8001 "; then
    echo "⚠️  端口 8001 已被占用"
    sudo netstat -tlnp 2>/dev/null | grep ":8001 " || sudo ss -tlnp 2>/dev/null | grep ":8001 "
else
    echo "✓ 端口 8001 未被占用"
fi

# 尝试启动
sudo -u www-data /srv/ai-service/venv/bin/python -m uvicorn app.main:app --host 127.0.0.1 --port 8001 > /tmp/ai-service-test.log 2>&1 &
TEST_PID=$!
sleep 5
if ps -p $TEST_PID > /dev/null 2>&1; then
    echo "✓ 手动启动成功（PID: $TEST_PID）"
    kill $TEST_PID 2>/dev/null || true
    wait $TEST_PID 2>/dev/null || true
else
    echo "✗ 手动启动失败"
    wait $TEST_PID 2>/dev/null || true
    EXIT_CODE=$?
    echo "退出码: $EXIT_CODE"
    echo "启动日志："
    cat /tmp/ai-service-test.log 2>/dev/null || echo "无法读取日志文件"
    rm -f /tmp/ai-service-test.log
fi
echo ""

echo "=== 诊断完成 ==="

