#!/bin/bash

# AI 服务修复脚本
set -e

echo "=== AI 服务修复脚本 ==="
echo ""

# 1. 检查并修复服务文件
echo "1. 检查服务文件..."
if [ -f /etc/systemd/system/ai-service.service ]; then
    echo "✓ 服务文件存在，检查内容..."
    
    # 检查 ExecStart 命令是否正确
    if grep -q "^ExecStart=/srv/ai-service/venv/bin/uvicorn" /etc/systemd/system/ai-service.service; then
        echo "⚠️  发现错误的 ExecStart 命令（直接调用 uvicorn），正在修复..."
        sudo sed -i 's|^ExecStart=/srv/ai-service/venv/bin/uvicorn|ExecStart=/srv/ai-service/venv/bin/python -m uvicorn|g' /etc/systemd/system/ai-service.service
        echo "✓ 已修复 ExecStart 命令"
    elif ! grep -q "^ExecStart=/srv/ai-service/venv/bin/python -m uvicorn" /etc/systemd/system/ai-service.service; then
        echo "⚠️  ExecStart 命令格式异常，正在修复..."
        # 备份原文件
        sudo cp /etc/systemd/system/ai-service.service /etc/systemd/system/ai-service.service.bak
        # 使用正确的服务文件替换
        if [ -f /srv/ai-service/ai-service.service ]; then
            sudo cp /srv/ai-service/ai-service.service /etc/systemd/system/ai-service.service
            echo "✓ 已使用正确的服务文件替换"
        else
            echo "✗ 无法找到源服务文件，请手动修复"
        fi
    else
        echo "✓ ExecStart 命令格式正确"
    fi
else
    echo "✗ 服务文件不存在，正在创建..."
    if [ -f /srv/ai-service/ai-service.service ]; then
        sudo cp /srv/ai-service/ai-service.service /etc/systemd/system/
        echo "✓ 服务文件已创建"
    else
        echo "✗ 无法找到源服务文件 /srv/ai-service/ai-service.service"
        exit 1
    fi
fi
echo ""

# 2. 检查工作目录和文件
echo "2. 检查工作目录..."
if [ ! -d /srv/ai-service ]; then
    echo "✗ 工作目录不存在，请先部署代码"
    exit 1
fi
echo "✓ 工作目录存在"
echo ""

# 3. 检查并创建虚拟环境
echo "3. 检查虚拟环境..."
if [ ! -d /srv/ai-service/venv ]; then
    echo "⚠️  虚拟环境不存在，正在创建..."
    cd /srv/ai-service
    python3 -m venv venv
    echo "✓ 虚拟环境已创建"
fi

# 激活虚拟环境并安装依赖
echo "安装/更新依赖..."
cd /srv/ai-service
source venv/bin/activate
pip install --upgrade pip
if [ -f requirements.txt ]; then
    pip install -r requirements.txt
    echo "✓ 依赖已安装"
else
    echo "⚠️  requirements.txt 不存在"
fi
echo ""

# 4. 检查应用文件
echo "4. 检查应用文件..."
if [ ! -f /srv/ai-service/app/main.py ]; then
    echo "✗ app/main.py 不存在"
    exit 1
fi
if [ ! -f /srv/ai-service/app/__init__.py ]; then
    echo "⚠️  app/__init__.py 不存在，正在创建..."
    touch /srv/ai-service/app/__init__.py
    echo "✓ app/__init__.py 已创建"
fi
echo "✓ 应用文件完整"
echo ""

# 5. 创建日志目录
echo "5. 创建日志目录..."
mkdir -p /srv/ai-service/logs
echo "✓ 日志目录已创建"
echo ""

# 6. 修复权限
echo "6. 修复权限..."
sudo chown -R www-data:www-data /srv/ai-service
sudo chmod -R 755 /srv/ai-service
# 确保日志目录可写
sudo chmod 775 /srv/ai-service/logs
echo "✓ 权限已修复"
echo ""

# 7. 测试 Python 导入
echo "7. 测试 Python 模块导入..."
cd /srv/ai-service
if sudo -u www-data /srv/ai-service/venv/bin/python -c "from app.main import app; print('✓ 模块导入成功')" 2>&1; then
    echo "✓ Python 模块导入测试通过"
else
    echo "✗ Python 模块导入失败"
    echo "错误信息："
    sudo -u www-data /srv/ai-service/venv/bin/python -c "from app.main import app" 2>&1 || true
    echo ""
    echo "请检查："
    echo "1. 依赖是否完整安装：pip install -r requirements.txt"
    echo "2. app/__init__.py 文件是否存在"
    echo "3. 所有导入的模块是否都存在"
    exit 1
fi
echo ""

# 8. 测试启动命令
echo "8. 测试启动命令（以 www-data 用户）..."
cd /srv/ai-service
# 先检查端口是否被占用
if sudo netstat -tlnp 2>/dev/null | grep -q ":8001 " || sudo ss -tlnp 2>/dev/null | grep -q ":8001 "; then
    echo "⚠️  端口 8001 已被占用，正在尝试停止占用进程..."
    sudo fuser -k 8001/tcp 2>/dev/null || true
    sleep 2
fi

# 测试启动（后台运行，捕获输出）
TEST_OUTPUT=$(sudo -u www-data /srv/ai-service/venv/bin/python -m uvicorn app.main:app --host 127.0.0.1 --port 8001 2>&1 &)
TEST_PID=$!
sleep 5

if ps -p $TEST_PID > /dev/null 2>&1; then
    echo "✓ 手动启动测试成功（PID: $TEST_PID）"
    kill $TEST_PID 2>/dev/null || true
    sleep 1
    wait $TEST_PID 2>/dev/null || true
else
    echo "✗ 手动启动测试失败"
    wait $TEST_PID 2>/dev/null || true
    EXIT_CODE=$?
    echo "退出码: $EXIT_CODE"
    echo ""
    echo "详细错误信息："
    echo "$TEST_OUTPUT"
    echo ""
    echo "请查看系统日志获取更多信息："
    echo "sudo journalctl -u ai-service -n 50 --no-pager"
    exit 1
fi
echo ""

# 9. 重新加载 systemd 并启动服务
echo "9. 重新加载 systemd 配置..."
sudo systemctl daemon-reload
echo "✓ systemd 配置已重新加载"
echo ""

echo "10. 启动服务..."
sudo systemctl restart ai-service
sleep 2

# 检查服务状态
sleep 3
if sudo systemctl is-active --quiet ai-service; then
    echo "✓ 服务启动成功"
    echo ""
    echo "服务状态："
    sudo systemctl status ai-service --no-pager -l
    echo ""
    echo "测试健康检查："
    sleep 2
    if curl -s http://127.0.0.1:8001/health > /dev/null; then
        echo "✓ 健康检查通过"
    else
        echo "⚠️  健康检查失败，但服务已启动"
    fi
else
    echo "✗ 服务启动失败"
    echo ""
    echo "详细错误信息："
    sudo journalctl -u ai-service -n 50 --no-pager
    echo ""
    echo "请尝试以下命令获取更多信息："
    echo "1. 查看完整日志: sudo journalctl -u ai-service -n 100 --no-pager"
    echo "2. 手动测试启动: cd /srv/ai-service && sudo -u www-data venv/bin/python -m uvicorn app.main:app --host 127.0.0.1 --port 8001"
    echo "3. 检查应用日志: tail -n 50 /srv/ai-service/logs/ai-service.log"
    exit 1
fi

echo ""
echo "=== 修复完成 ==="

