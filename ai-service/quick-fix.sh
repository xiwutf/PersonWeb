#!/bin/bash

# 快速修复脚本 - 用于 GitHub Actions SSH 部署
# 专门修复退出码 3 的问题

set -e

echo "=== 快速修复 AI 服务启动问题 ==="

# 1. 修复服务文件中的启动命令
echo "1. 检查并修复服务文件..."
if [ -f /etc/systemd/system/ai-service.service ]; then
    # 检查并修复错误的 ExecStart 命令
    if grep -q "^ExecStart=/srv/ai-service/venv/bin/uvicorn[[:space:]]" /etc/systemd/system/ai-service.service; then
        echo "⚠️  发现错误的启动命令，正在修复..."
        sudo sed -i 's|^ExecStart=/srv/ai-service/venv/bin/uvicorn|ExecStart=/srv/ai-service/venv/bin/python -m uvicorn|g' /etc/systemd/system/ai-service.service
        echo "✓ 已修复启动命令"
    fi
fi

# 2. 确保日志目录存在且可写
echo "2. 检查日志目录..."
sudo mkdir -p /srv/ai-service/logs
sudo chown www-data:www-data /srv/ai-service/logs
sudo chmod 775 /srv/ai-service/logs
echo "✓ 日志目录已准备"

# 3. 重新加载 systemd
echo "3. 重新加载 systemd..."
sudo systemctl daemon-reload
echo "✓ systemd 已重新加载"

# 4. 重启服务
echo "4. 重启服务..."
sudo systemctl restart ai-service
sleep 3

# 5. 检查服务状态
echo "5. 检查服务状态..."
if sudo systemctl is-active --quiet ai-service; then
    echo "✓ 服务启动成功！"
    sudo systemctl status ai-service --no-pager -l
    exit 0
else
    echo "❌ 服务启动失败"
    echo ""
    echo "详细错误信息："
    sudo journalctl -u ai-service -n 50 --no-pager
    echo ""
    echo "请运行诊断脚本获取更多信息："
    echo "  bash /srv/ai-service/diagnose-startup.sh"
    exit 1
fi

