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
    if grep -q "venv/bin/uvicorn" /etc/systemd/system/ai-service.service; then
        echo "⚠️  发现错误的 ExecStart 命令，正在修复..."
        sudo sed -i 's|ExecStart=/srv/ai-service/venv/bin/uvicorn|ExecStart=/srv/ai-service/venv/bin/python -m uvicorn|g' /etc/systemd/system/ai-service.service
        echo "✓ 已修复 ExecStart 命令"
    fi
else
    echo "✗ 服务文件不存在，正在创建..."
    sudo cp /srv/ai-service/ai-service.service /etc/systemd/system/
    echo "✓ 服务文件已创建"
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

# 7. 测试启动命令
echo "7. 测试启动命令（以 www-data 用户）..."
cd /srv/ai-service
sudo -u www-data /srv/ai-service/venv/bin/python -m uvicorn app.main:app --host 127.0.0.1 --port 8001 &
TEST_PID=$!
sleep 3

if ps -p $TEST_PID > /dev/null; then
    echo "✓ 手动启动测试成功"
    kill $TEST_PID 2>/dev/null || true
    wait $TEST_PID 2>/dev/null || true
else
    echo "✗ 手动启动测试失败"
    wait $TEST_PID
    echo "退出码: $?"
    echo "请查看详细错误信息："
    echo "sudo journalctl -u ai-service -n 50 --no-pager"
    exit 1
fi
echo ""

# 8. 重新加载 systemd 并启动服务
echo "8. 重新加载 systemd 配置..."
sudo systemctl daemon-reload
echo "✓ systemd 配置已重新加载"
echo ""

echo "9. 启动服务..."
sudo systemctl restart ai-service
sleep 2

# 检查服务状态
if sudo systemctl is-active --quiet ai-service; then
    echo "✓ 服务启动成功"
    sudo systemctl status ai-service --no-pager -l
else
    echo "✗ 服务启动失败"
    echo "详细错误信息："
    sudo journalctl -u ai-service -n 50 --no-pager
    exit 1
fi

echo ""
echo "=== 修复完成 ==="

