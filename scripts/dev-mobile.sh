#!/bin/bash
# 移动端开发服务器启动脚本
# 使用方法：chmod +x scripts/dev-mobile.sh && ./scripts/dev-mobile.sh

# 获取本机 IP 地址
IP=$(ifconfig 2>/dev/null | grep "inet " | grep -v 127.0.0.1 | awk '{print $2}' | head -n 1)

# 如果 ifconfig 不可用，尝试使用 ip 命令
if [ -z "$IP" ]; then
    IP=$(ip addr show 2>/dev/null | grep "inet " | grep -v 127.0.0.1 | awk '{print $2}' | cut -d'/' -f1 | head -n 1)
fi

# 如果还是获取不到，尝试使用 hostname
if [ -z "$IP" ]; then
    IP=$(hostname -I 2>/dev/null | awk '{print $1}')
fi

if [ -z "$IP" ]; then
    echo "无法获取 IP 地址，请手动检查网络连接"
    exit 1
fi

echo "========================================"
echo "  移动端开发服务器"
echo "========================================"
echo ""
echo "本机 IP 地址: $IP"
echo ""
echo "在手机上访问: http://$IP:3000"
echo ""
echo "确保："
echo "  1. 手机和电脑连接到同一个 WiFi"
echo "  2. 防火墙允许 Node.js 通过"
echo "  3. 如果 API 请求失败，请修改 .env 文件中的 API 地址"
echo ""
echo "按 Ctrl+C 停止服务器"
echo ""

# 启动服务器
HOST=0.0.0.0 npm run dev

